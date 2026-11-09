"""Report every place the product version is stated, and every disagreement.

IDN-02 says one product version is the source of every version a caller can
read. The specification names six of them, the published exe, the value
`version_info` reports, the npm package, the `.mcpb` manifest, `server.json`
and the release tag. This reads all six, plus the nested versions `server.json`
carries for its packages, and prints one row per value.

The exe is read from its PE version resource, the same bytes Explorer shows.
`version_info` is the `Version` constant the build bakes into that resource, so
it is read from `Directory.Build.props`, which is where the assembly attribute
comes from. The tag is the newest `vX.Y.Z` tag the repository holds.

    python tools/check_versions.py
    python tools/check_versions.py --json
    python tools/check_versions.py --repo C:\\path\\to\\wc3-mcp
    python tools/check_versions.py --git "C:\\Program Files\\Git\\bin\\git.exe"
    python tools/check_versions.py --selftest

Exit 0 when every source states one value, 2 when they disagree, naming each
source and its value, 1 when a source exists but cannot be read. A source that
is not in the tree, the exe before a publish, is reported absent and does not
count as a disagreement.

The reference row is the exe when the tree holds one, because IDN-02 makes the
release refuse a tag that differs from the exe. With no exe the newest tag is
the reference. Every other row is marked against it.
"""

from __future__ import annotations

import argparse
import json
import os
import re
import subprocess
import sys
import tempfile
from pathlib import Path

HERE = Path(__file__).resolve().parent
REPO = HERE.parent.parent.parent

DEFAULT_GIT = r"C:\Program Files\Git\bin\git.exe"

TAG_RE = re.compile(r"^v?(\d+)\.(\d+)\.(\d+)$")
VERSION_RE = re.compile(r"(\d+)\.(\d+)\.(\d+)")


class VersionError(RuntimeError):
    """A source that is present but cannot be read."""


def normalise(text: str) -> str:
    """0.5.16.0, 0.5.16+abc and v0.5.16 all mean 0.5.16."""
    match = VERSION_RE.search(text or "")
    if not match:
        return (text or "").strip()
    return ".".join(match.groups())


def sort_key(text: str) -> tuple[int, int, int]:
    match = TAG_RE.match(text.strip())
    return tuple(int(part) for part in match.groups()) if match else (-1, -1, -1)


def git_binary(explicit: str | None) -> str:
    if explicit:
        return explicit
    if os.name == "nt" and Path(DEFAULT_GIT).is_file():
        return DEFAULT_GIT
    return "git"


def newest_tag(repo: Path, git: str) -> str | None:
    """The newest vX.Y.Z tag, by number and not by name."""
    try:
        result = subprocess.run(
            [git, "-C", str(repo).replace("\\", "/"), "tag", "--list"],
            capture_output=True,
            text=True,
            timeout=60,
        )
    except (OSError, subprocess.SubprocessError) as exc:
        raise VersionError(f"git could not be run at {git}, {exc}") from exc
    if result.returncode != 0:
        raise VersionError(
            "git tag failed in " + str(repo) + ", " + result.stderr.strip()
        )
    tags = [line.strip() for line in result.stdout.splitlines() if line.strip()]
    versioned = [tag for tag in tags if TAG_RE.match(tag)]
    return max(versioned, key=sort_key) if versioned else None


def read_exe_version(path: Path) -> tuple[str, str]:
    """The file version and the informational version from the PE resource."""
    if os.name != "nt":
        raise VersionError("the PE version resource can only be read on Windows")
    import ctypes
    from ctypes import wintypes

    version = ctypes.WinDLL("version")
    version.GetFileVersionInfoSizeW.restype = wintypes.DWORD
    version.GetFileVersionInfoSizeW.argtypes = [wintypes.LPCWSTR, ctypes.c_void_p]
    version.GetFileVersionInfoW.restype = wintypes.BOOL
    version.GetFileVersionInfoW.argtypes = [
        wintypes.LPCWSTR,
        wintypes.DWORD,
        wintypes.DWORD,
        ctypes.c_void_p,
    ]
    version.VerQueryValueW.restype = wintypes.BOOL
    version.VerQueryValueW.argtypes = [
        ctypes.c_void_p,
        wintypes.LPCWSTR,
        ctypes.POINTER(ctypes.c_void_p),
        ctypes.POINTER(wintypes.UINT),
    ]

    text = str(path)
    size = version.GetFileVersionInfoSizeW(text, None)
    if not size:
        raise VersionError(f"{path} carries no version resource")
    buffer = ctypes.create_string_buffer(size)
    if not version.GetFileVersionInfoW(text, 0, size, buffer):
        raise VersionError(f"{path} version resource could not be opened")

    block = ctypes.cast(buffer, ctypes.c_void_p)
    pointer = ctypes.c_void_p()
    length = wintypes.UINT()
    if not version.VerQueryValueW(
        block, "\\VarFileInfo\\Translation", ctypes.byref(pointer), ctypes.byref(length)
    ):
        raise VersionError(f"{path} version resource has no translation table")

    class LangAndCodePage(ctypes.Structure):
        _fields_ = [("language", wintypes.WORD), ("codepage", wintypes.WORD)]

    pair = ctypes.cast(pointer, ctypes.POINTER(LangAndCodePage))[0]

    def string(name: str) -> str:
        sub = "\\StringFileInfo\\%04x%04x\\%s" % (pair.language, pair.codepage, name)
        value = ctypes.c_void_p()
        chars = wintypes.UINT()
        if not version.VerQueryValueW(
            block, sub, ctypes.byref(value), ctypes.byref(chars)
        ):
            return ""
        return ctypes.wstring_at(value.value)

    file_version = string("FileVersion")
    product_version = string("ProductVersion")
    if not file_version and not product_version:
        raise VersionError(f"{path} carries no FileVersion or ProductVersion string")
    return file_version, product_version


def json_versions(document: object, where: str, label: str | None = None) -> list[dict]:
    """Every string under a key named version, labelled by the path that holds it."""
    found: list[dict] = []
    if isinstance(document, dict):
        for key, value in document.items():
            path = f"{where}.{key}" if where else str(key)
            if key == "version" and isinstance(value, str):
                found.append({"where": path, "value": value})
            else:
                found.extend(json_versions(value, path))
    elif isinstance(document, list):
        for index, value in enumerate(document):
            found.extend(json_versions(value, f"{where}[{index}]"))
    return found


def load_json(path: Path) -> object:
    try:
        return json.loads(path.read_text(encoding="utf-8-sig"))
    except OSError as exc:
        raise VersionError(f"{path} could not be read, {exc}") from exc
    except ValueError as exc:
        raise VersionError(f"{path} is not valid JSON, {exc}") from exc


def first_existing(candidates: list[Path]) -> Path | None:
    for candidate in candidates:
        if candidate.is_file():
            return candidate
    return None


def label(prefix: str, where: str) -> str:
    """A file name and the JSON path inside it, joined by one dot."""
    return f"{prefix}.{where}" if where else prefix


def collect(repo: Path, tags: list[str] | None = None, git: str | None = None) -> list[dict]:
    """One row per place a version is stated, in the order the spec names them."""
    rows: list[dict] = []

    def row(source: str, where: str, value: str | None, note: str = "") -> None:
        rows.append({"source": source, "where": where, "value": value, "note": note})

    exe = first_existing(
        [repo / "dist-mcp" / "Wc3.Mcp.exe", repo / "dist-mcp" / "wc3-mcp.exe"]
    )
    if exe is None:
        row("exe", "dist-mcp/Wc3.Mcp.exe", None, "absent")
    else:
        try:
            file_version, product_version = read_exe_version(exe)
        except VersionError as exc:
            raise VersionError(str(exc)) from exc
        row("exe", exe.relative_to(repo).as_posix(), normalise(file_version), file_version)
        if product_version:
            row(
                "exe product",
                exe.relative_to(repo).as_posix() + " ProductVersion",
                normalise(product_version),
                product_version,
            )

    props = repo / "Directory.Build.props"
    if not props.is_file():
        row("version_info", "Directory.Build.props", None, "absent")
    else:
        try:
            text = props.read_text(encoding="utf-8-sig")
        except OSError as exc:
            raise VersionError(f"{props} could not be read, {exc}") from exc
        found = re.findall(r"<Version>\s*([^<\s]+)\s*</Version>", text)
        if not found:
            raise VersionError(f"{props} states no <Version>")
        row("version_info", "Directory.Build.props <Version>", normalise(found[0]), found[0])

    npm = first_existing([repo / "npm" / "wc3-mcp" / "package.json", repo / "npm" / "package.json"])
    if npm is None:
        row("npm package", "npm/wc3-mcp/package.json", None, "absent")
    else:
        document = load_json(npm)
        stated = json_versions(document, "")
        if not stated:
            raise VersionError(f"{npm} states no version")
        for entry in stated:
            row("npm package", label(npm.relative_to(repo).as_posix(), entry["where"]), normalise(entry["value"]), entry["value"])

    manifest = repo / "mcpb" / "manifest.json"
    if not manifest.is_file():
        row("mcpb manifest", "mcpb/manifest.json", None, "absent")
    else:
        document = load_json(manifest)
        stated = json_versions(document, "")
        if not stated:
            raise VersionError(f"{manifest} states no version")
        for entry in stated:
            row("mcpb manifest", label("mcpb/manifest.json", entry["where"]), normalise(entry["value"]), entry["value"])

    server = repo / "server.json"
    if not server.is_file():
        row("server.json", "server.json", None, "absent")
    else:
        document = load_json(server)
        stated = json_versions(document, "")
        if not stated:
            raise VersionError(f"{server} states no version")
        for entry in stated:
            row("server.json", label("server.json", entry["where"]), normalise(entry["value"]), entry["value"])

    if tags is None:
        tags = []
        found = newest_tag(repo, git_binary(git))
        if found:
            tags = [found]
    if tags:
        tag = max(tags, key=sort_key)
        row("release tag", "newest git tag", normalise(tag), tag)
    else:
        row("release tag", "newest git tag", None, "no vX.Y.Z tag")

    return rows


def reference_of(rows: list[dict]) -> tuple[str | None, str]:
    """The row every other row is marked against, and why."""
    for source in ("exe", "release tag"):
        for row in rows:
            if row["source"] == source and row["value"]:
                return row["value"], row["source"]
    return None, "no reference"


def build_rows(rows: list[dict]) -> list[dict]:
    reference, why = reference_of(rows)
    built = []
    for row in rows:
        if row["value"] is None:
            verdict = "absent"
        elif reference is None:
            verdict = "unreferenced"
        elif row["value"] == reference:
            verdict = "ok"
        else:
            verdict = "differs"
        built.append({**row, "verdict": verdict, "reference": reference, "reference_from": why})
    return built


def render_table(rows: list[dict], reference: str | None, reference_from: str) -> str:
    widths = {
        "source": max([len("Source")] + [len(r["source"]) for r in rows]),
        "where": max([len("Where")] + [len(r["where"]) for r in rows]),
        "value": max([len("Value")] + [len(r["value"] or "absent") for r in rows]),
    }
    head = (
        f"{'Source'.ljust(widths['source'])}  "
        f"{'Where'.ljust(widths['where'])}  "
        f"{'Value'.ljust(widths['value'])}  Verdict"
    )
    lines = [head, "-" * len(head)]
    for row in rows:
        value = row["value"] or "absent"
        note = row["note"] if row["note"] and row["note"] != value else ""
        detail = ""
        if row["verdict"] == "differs":
            detail = f"  differs from {reference}"
        elif row["verdict"] == "absent" and note:
            detail = f"  {note}"
        elif note and normalise(note) != value:
            detail = f"  reads {note}"
        lines.append(
            f"{row['source'].ljust(widths['source'])}  "
            f"{row['where'].ljust(widths['where'])}  "
            f"{value.ljust(widths['value'])}  {row['verdict']}{detail}"
        )
    return "\n".join(lines)


def summarise(rows: list[dict]) -> str:
    stated = [r for r in rows if r["value"]]
    distinct = sorted({r["value"] for r in stated}, key=sort_key)
    if not stated:
        return "no version could be read at all"
    if len(distinct) == 1:
        return f"{len(stated)} sources state one value, {distinct[0]}"
    groups = []
    for value in distinct:
        names = [r["source"] for r in stated if r["value"] == value]
        groups.append(f"{value} in {len(names)}")
    return (
        f"{len(stated)} sources carry {len(distinct)} values, "
        + ", ".join(groups)
        + "; no source can be called authoritative"
    )


def check(repo: Path, git: str | None = None) -> tuple[list[dict], str | None, str]:
    rows = build_rows(collect(repo, git=git))
    reference, why = reference_of(rows)
    return rows, reference, why


def selftest() -> int:
    """Build two trees and prove one fails on a disagreement and one passes."""
    failures: list[str] = []
    with tempfile.TemporaryDirectory(prefix="check-versions-") as folder:
        root = Path(folder)

        def tree(name: str, version: str = "0.5.16") -> Path:
            repo = root / ("repo-" + name)
            (repo / "npm").mkdir(parents=True)
            (repo / "mcpb").mkdir()
            (repo / "Directory.Build.props").write_text(
                f"<Project><PropertyGroup><Version>{version}</Version></PropertyGroup></Project>\n",
                encoding="utf-8",
            )
            (repo / "npm" / "package.json").write_text(
                json.dumps({"name": "wc3-mcp", "version": version}), encoding="utf-8"
            )
            (repo / "mcpb" / "manifest.json").write_text(
                json.dumps({"version": version}), encoding="utf-8"
            )
            (repo / "server.json").write_text(
                json.dumps({"version": version, "packages": [{"version": version}]}),
                encoding="utf-8",
            )
            return repo

        agreeing = tree("agreeing")
        rows = build_rows(collect(agreeing, tags=["v0.5.16"]))
        if any(row["verdict"] not in ("ok",) for row in rows if row["value"]):
            failures.append("an agreeing tree was reported as a disagreement")
        if len(rows) != 7:
            failures.append(f"expected 7 source rows, got {len(rows)}")

        drifting = tree("drifting")
        (drifting / "npm" / "package.json").write_text(
            json.dumps({"name": "wc3-mcp", "version": "0.5.8"}), encoding="utf-8"
        )
        rows = build_rows(collect(drifting, tags=["v0.5.16"]))
        if not any(row["verdict"] == "differs" for row in rows):
            failures.append("a drifting npm package was not reported")
        if not any(row["value"] == "0.5.8" for row in rows):
            failures.append("the drifting value was not named")

        missing = build_rows(collect(tree("missing"), tags=[]))
        if not any(row["verdict"] == "absent" for row in missing):
            failures.append("a missing tag was not reported absent")

    if failures:
        for failure in failures:
            print("selftest failed, " + failure, file=sys.stderr)
        return 1
    print("selftest ok, six places read, a disagreement named and an absent source marked")
    return 0


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(description="Report every place the product version is stated.")
    parser.add_argument("--repo", default=str(REPO), help="the wc3-mcp checkout to read")
    parser.add_argument("--git", default=None, help="the git executable, the full path when PATH is broken")
    parser.add_argument("--json", action="store_true", help="print the rows as JSON")
    parser.add_argument("--selftest", action="store_true", help="run the built in selftest")
    args = parser.parse_args(argv)

    if args.selftest:
        return selftest()

    repo = Path(args.repo).resolve()
    if not repo.is_dir():
        print(f"no repository at {repo}", file=sys.stderr)
        return 1

    try:
        rows, reference, why = check(repo, args.git)
    except VersionError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    summary = summarise(rows)
    if args.json:
        print(
            json.dumps(
                {
                    "ok": all(row["verdict"] in ("ok", "absent") for row in rows),
                    "reference": reference,
                    "reference_from": why,
                    "summary": summary,
                    "rows": [
                        {k: r[k] for k in ("source", "where", "value", "verdict", "note")}
                        for r in rows
                    ],
                },
                indent=2,
                ensure_ascii=False,
            )
        )
    else:
        print(render_table(rows, reference, why))
        print()
        if reference:
            print(f"reference is {reference}, from the {why}")
        print(summary)

    ok = all(row["verdict"] in ("ok", "absent") for row in rows)
    return 0 if ok else 2


if __name__ == "__main__":
    sys.exit(main())
