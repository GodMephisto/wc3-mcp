"""Guard the specification's body numbers against the published server.

Sections 3.1, 3.3 and 3.5 state the tool surface as measured numbers. Those numbers
drifted from the binary twice, the document said 271 while the exe said 272, then 272
against 273, because nothing compared them. This starts the published exe, speaks MCP
over stdio, measures the same figures, and prints declared against measured.

The resource count is guarded the same way. It went from 3 to 6 when the three skills
landed and nothing noticed, because only the tool count, the description total and the
catalog size were compared. So the guard now also calls `resources/list`, compares the
count against the `Resources` row of section 3.1, and names any of the three skill URIs
that the server stops serving.

    python tools/measure_surface.py
    python tools/measure_surface.py --json
    python tools/measure_surface.py --exe C:\\path\\to\\Wc3.Mcp.exe
    python tools/measure_surface.py --markdown C:\\path\\to\\spec.md

Exit 0 when every figure agrees, 2 when one drifted or a skill resource is missing, 1
when the server could not be measured at all. The PDF build runs this first and refuses
to render on a drift.
"""

import argparse
import collections
import json
import os
import queue
import re
import subprocess
import sys
import threading
import time
from pathlib import Path

HERE = Path(__file__).resolve().parent
SPEC = HERE.parent / "wc3-mcp-specification-v1.md"
ROOT = HERE.parent.parent.parent
EXE_NAME = "Wc3.Mcp.exe" if os.name == "nt" else "Wc3.Mcp"
PUBLISH = (
    "dotnet publish src\\Wc3.Mcp\\Wc3.Mcp.csproj -c Release --self-contained "
    "-r win-x64 -o dist-mcp"
)

# The three skill resources the surface promises, by URI, so a dropped one is named.
SKILL_URIS = (
    "wc3://skills/map-safety",
    "wc3://skills/hero-package",
    "wc3://skills/publish",
)

# key, section, the label printed in the table
FIELDS = (
    ("tools", "3.1", "Tools"),
    ("read_only", "3.1", "Read-only tools"),
    ("writing", "3.1", "Writing tools"),
    ("output_schemas", "3.1", "Tools publishing an output schema"),
    ("resources", "3.1", "Resources"),
    ("skill_resources", "3.5", "Skill resources present"),
    ("capability_tools", "3.1", "tools collection, listChanged"),
    ("capability_resources", "3.1", "resources collection, listChanged"),
    ("capability_prompts", "3.1", "prompts collection, listChanged"),
    ("description_chars", "3.3", "Total description characters"),
    ("titles", "3.3", "Tools with a title"),
    ("catalog_bytes", "3.5", "Catalog resource wc3://tools, bytes"),
    ("prompts", "3.5", "Prompts"),
)


class MeasureError(RuntimeError):
    """The server or the document could not be read well enough to compare."""


def find_exe(explicit: str | None) -> Path:
    """The published server, from the flag, the environment, or a usual folder."""
    if explicit:
        path = Path(explicit)
        if not path.is_file():
            raise MeasureError(f"no server at {path}")
        return path

    from_env = os.environ.get("WC3_MCP_EXE")
    if from_env:
        path = Path(from_env)
        if not path.is_file():
            raise MeasureError(f"WC3_MCP_EXE points at {path}, which is not a file")
        return path

    candidates = (
        ROOT / "dist-mcp" / EXE_NAME,
        ROOT.parent / "Wc3_CLI" / "dist-mcp" / EXE_NAME,
    )
    for candidate in candidates:
        if candidate.is_file():
            return candidate

    looked = "\n  ".join(str(c) for c in candidates)
    raise MeasureError(
        "no published server found. Looked for\n  "
        + looked
        + "\n\nPublish it first, from the wc3ctl repository root\n  "
        + PUBLISH
    )


class Session:
    """One MCP stdio session, read on a thread so a stuck server cannot hang us."""

    def __init__(self, exe: Path, timeout: float) -> None:
        self.timeout = timeout
        self._inbox: "queue.Queue[dict | None]" = queue.Queue()
        self._stderr: "collections.deque[str]" = collections.deque(maxlen=30)
        self._next_id = 0
        flags = subprocess.CREATE_NO_WINDOW if os.name == "nt" else 0
        try:
            self.proc = subprocess.Popen(
                [str(exe)],
                stdin=subprocess.PIPE,
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE,
                text=True,
                encoding="utf-8",
                errors="replace",
                bufsize=1,
                creationflags=flags,
            )
        except OSError as exc:
            raise MeasureError(f"the server did not start, {exc}") from exc
        threading.Thread(target=self._pump_stdout, daemon=True).start()
        threading.Thread(target=self._pump_stderr, daemon=True).start()

    def _pump_stdout(self) -> None:
        assert self.proc.stdout is not None
        try:
            for line in self.proc.stdout:
                line = line.strip()
                if not line:
                    continue
                try:
                    self._inbox.put(json.loads(line))
                except json.JSONDecodeError:
                    continue
        finally:
            self._inbox.put(None)

    def _pump_stderr(self) -> None:
        assert self.proc.stderr is not None
        for line in self.proc.stderr:
            self._stderr.append(line.rstrip())

    def _failure(self, text: str) -> str:
        if not self._stderr:
            return text
        return text + "\nthe server wrote to stderr\n  " + "\n  ".join(self._stderr)

    def _write(self, message: dict) -> None:
        if self.proc.stdin is None:
            raise MeasureError("the server closed its input")
        try:
            self.proc.stdin.write(json.dumps(message) + "\n")
            self.proc.stdin.flush()
        except (BrokenPipeError, OSError) as exc:
            raise MeasureError(self._failure(f"the server stopped reading, {exc}")) from exc

    def notify(self, method: str) -> None:
        self._write({"jsonrpc": "2.0", "method": method})

    def request(self, method: str, params: dict) -> dict:
        self._next_id += 1
        rid = self._next_id
        self._write({"jsonrpc": "2.0", "id": rid, "method": method, "params": params})
        deadline = time.monotonic() + self.timeout
        while True:
            remaining = deadline - time.monotonic()
            if remaining <= 0:
                raise MeasureError(
                    self._failure(f"no reply to {method} within {self.timeout:g} seconds")
                )
            try:
                message = self._inbox.get(timeout=remaining)
            except queue.Empty:
                raise MeasureError(
                    self._failure(f"no reply to {method} within {self.timeout:g} seconds")
                ) from None
            if message is None:
                raise MeasureError(
                    self._failure(f"the server exited before replying to {method}")
                )
            if message.get("id") != rid:
                continue
            if "error" in message:
                error = message["error"]
                detail = error.get("message", error) if isinstance(error, dict) else error
                raise MeasureError(f"{method} returned an error, {detail}")
            return message.get("result") or {}

    def close(self) -> None:
        try:
            if self.proc.stdin is not None:
                self.proc.stdin.close()
        except OSError:
            pass
        try:
            self.proc.wait(timeout=5)
        except subprocess.TimeoutExpired:
            self.proc.kill()


def measure(exe: Path, timeout: float) -> dict:
    """What the published exe actually reports."""
    session = Session(exe, timeout)
    try:
        init = session.request(
            "initialize",
            {
                "protocolVersion": "2025-06-18",
                "capabilities": {},
                "clientInfo": {"name": "measure_surface", "version": "1"},
            },
        )
        caps = init.get("capabilities") or {}
        session.notify("notifications/initialized")

        tools: list[dict] = []
        cursor = None
        while True:
            page = session.request("tools/list", {"cursor": cursor} if cursor else {})
            tools.extend(page.get("tools") or [])
            cursor = page.get("nextCursor")
            if not cursor:
                break

        resources: list[dict] = []
        cursor = None
        while True:
            page = session.request(
                "resources/list", {"cursor": cursor} if cursor else {}
            )
            resources.extend(page.get("resources") or [])
            cursor = page.get("nextCursor")
            if not cursor:
                break

        prompts: list[dict] = []
        cursor = None
        while True:
            page = session.request("prompts/list", {"cursor": cursor} if cursor else {})
            prompts.extend(page.get("prompts") or [])
            cursor = page.get("nextCursor")
            if not cursor:
                break

        catalog = session.request("resources/read", {"uri": "wc3://tools"})
    finally:
        session.close()

    contents = catalog.get("contents") or []
    if not contents:
        raise MeasureError("the catalog resource wc3://tools came back empty")
    entry = contents[0]
    if "text" in entry:
        catalog_text = entry["text"]
    elif "blob" in entry:
        import base64

        catalog_text = base64.b64decode(entry["blob"]).decode("utf-8", "replace")
    else:
        raise MeasureError("the catalog resource wc3://tools carried no text or blob")

    read_only = sum(
        1 for t in tools if (t.get("annotations") or {}).get("readOnlyHint") is True
    )
    uris = [str(r.get("uri") or "") for r in resources]
    return {
        "tools": len(tools),
        "read_only": read_only,
        "writing": len(tools) - read_only,
        "resources": len(resources),
        "resource_uris": uris,
        "skill_resources": sum(1 for uri in SKILL_URIS if uri in uris),
        "description_chars": sum(len(t.get("description") or "") for t in tools),
        "titles": sum(
            1
            for t in tools
            if "title" in t or "title" in (t.get("annotations") or {})
        ),
        "output_schemas": sum(1 for t in tools if "outputSchema" in t),
        "capability_tools": bool((caps.get("tools") or {}).get("listChanged")),
        "capability_resources": bool((caps.get("resources") or {}).get("listChanged")),
        "capability_prompts": bool((caps.get("prompts") or {}).get("listChanged")),
        "catalog_bytes": len(catalog_text.encode("utf-8")),
        "prompts": len(prompts),
        "prompt_names": [str(p.get("name") or "") for p in prompts],
        "server": init.get("serverInfo") or {},
    }


def section(md: str, number: str) -> str:
    """The text of one numbered section, up to the next heading."""
    m = re.search(r"^### " + re.escape(number) + r"\b.*$", md, re.M)
    if not m:
        raise MeasureError(
            f"the document has no section {number}. The guard reads its figures from "
            "sections 3.1, 3.3 and 3.5, so a renumber fails it rather than passing it"
        )
    rest = md[m.end() :]
    nxt = re.search(r"^#{2,3} ", rest, re.M)
    return rest[: nxt.start()] if nxt else rest


def row_text(text: str, label: str, number: str) -> str:
    """Everything after the leading cell of a table row whose first cell is label."""
    pattern = r"^\|\s*" + re.escape(label) + r"\s*\|(.*?)\|\s*$"
    m = re.search(pattern, text, re.M)
    if not m:
        raise MeasureError(
            f"section {number} has no row labelled {label!r}. The guard will not pass a "
            "figure the document no longer states"
        )
    return m.group(1)


def first_int(value: str, label: str) -> int:
    m = re.search(r"\d[\d,]*", value)
    if not m:
        raise MeasureError(f"the row for {label!r} carries no number, it says {value.strip()!r}")
    return int(m.group(0).replace(",", ""))


def listed(caps_text: str, name: str) -> bool:
    """Whether the capabilities row advertises listChanged for one collection."""
    for clause in caps_text.split(","):
        if re.search(r"\b" + name + r"\b", clause):
            return "listChanged" in clause
    raise MeasureError(f"the capabilities row does not mention {name}")


def declared_from(md: str) -> dict:
    """The same figures as the document states them."""
    sec31 = section(md, "3.1")
    sec33 = section(md, "3.3")
    sec35 = section(md, "3.5")
    caps = row_text(sec31, "Capabilities declared", "3.1")

    catalog = re.search(r"^\|\s*`wc3://tools`\s*\|(.*)\|\s*$", sec35, re.M)
    if not catalog:
        raise MeasureError(
            "section 3.5 has no row for the `wc3://tools` catalog resource"
        )

    return {
        "tools": first_int(row_text(sec31, "Tools", "3.1"), "Tools"),
        "read_only": first_int(
            row_text(sec31, "Read-only tools", "3.1"), "Read-only tools"
        ),
        "writing": first_int(row_text(sec31, "Writing tools", "3.1"), "Writing tools"),
        "output_schemas": first_int(
            row_text(sec31, "Tools publishing an output schema", "3.1"),
            "Tools publishing an output schema",
        ),
        "resources": first_int(row_text(sec31, "Resources", "3.1"), "Resources"),
        "skill_resources": sum(
            1 for uri in SKILL_URIS if re.search(r"`" + re.escape(uri) + r"`", sec35)
        ),
        "capability_tools": listed(caps, "tools"),
        "capability_resources": listed(caps, "resources"),
        "capability_prompts": listed(caps, "prompts"),
        "description_chars": first_int(
            row_text(sec33, "Total description text across the surface", "3.3"),
            "Total description text across the surface",
        ),
        "titles": first_int(
            row_text(sec33, "Tools with a `title` annotation", "3.3"),
            "Tools with a `title` annotation",
        ),
        "catalog_bytes": first_int(
            catalog.group(1).split("|")[-1], "the wc3://tools resource size"
        ),
        "prompts": first_int(row_text(sec35, "Prompts", "3.5"), "Prompts"),
    }


def compare(declared: dict, measured: dict) -> list[dict]:
    rows = []
    for key, number, label in FIELDS:
        before = declared[key]
        after = measured[key]
        rows.append(
            {
                "key": key,
                "section": number,
                "label": label,
                "declared": before,
                "measured": after,
                "ok": before == after,
            }
        )
    return rows


def skill_verdict(measured: dict) -> dict:
    """Which of the three promised skill resources the server actually serves."""
    served = measured.get("resource_uris") or []
    present = [uri for uri in SKILL_URIS if uri in served]
    missing = [uri for uri in SKILL_URIS if uri not in served]
    return {"ok": not missing, "present": present, "missing": missing}


def fmt(value) -> str:
    if isinstance(value, bool):
        return "true" if value else "false"
    if isinstance(value, int):
        return f"{value:,}"
    return str(value)


def render_table(rows: list[dict], skills: dict) -> str:
    header = ("Measure", "Section", "Declared", "Measured", "Verdict")
    body = [
        (
            f"{r['label']} ({r['section']})",
            r["section"],
            fmt(r["declared"]),
            fmt(r["measured"]),
            "ok" if r["ok"] else "DRIFT",
        )
        for r in rows
    ]
    widths = [max(len(str(x[i])) for x in [header] + body) for i in range(5)]
    out = [
        f"{header[0]:<{widths[0]}}  {header[1]:<{widths[1]}}  "
        f"{header[2]:>{widths[2]}}  {header[3]:>{widths[3]}}  {header[4]}",
        "-" * (sum(widths) + 8),
    ]
    for line in body:
        out.append(
            f"{line[0]:<{widths[0]}}  {line[1]:<{widths[1]}}  "
            f"{line[2]:>{widths[2]}}  {line[3]:>{widths[3]}}  {line[4]}"
        )
    drifted = [r for r in rows if not r["ok"]]
    out.append("")
    if drifted:
        out.append(
            f"{len(drifted)} of {len(rows)} measures drifted. The document no longer "
            "describes the published server."
        )
        out.append("Change the Declared value of each DRIFT row so it matches Measured.")
    else:
        out.append(f"All {len(rows)} measures agree with the published server.")
    if skills["missing"]:
        out.append(
            "Missing skill resources, the server no longer serves "
            + ", ".join(skills["missing"])
        )
    else:
        out.append(f"All {len(SKILL_URIS)} skill resources are served.")
    return "\n".join(out)


def main(argv: list[str] | None = None) -> int:
    parser = argparse.ArgumentParser(
        description="Compare the specification's stated surface with the published exe."
    )
    parser.add_argument("--exe", help="path to the published Wc3.Mcp executable")
    parser.add_argument("--markdown", default=str(SPEC), help="the specification markdown")
    parser.add_argument("--timeout", type=float, default=180.0, help="seconds per request")
    parser.add_argument("--json", action="store_true", help="print the comparison as JSON")
    args = parser.parse_args(argv)

    try:
        exe = find_exe(args.exe)
        markdown = Path(args.markdown)
        if not markdown.is_file():
            raise MeasureError(f"no specification markdown at {markdown}")
        md = markdown.read_text(encoding="utf-8")
        declared = declared_from(md)
        measured = measure(exe, args.timeout)
        rows = compare(declared, measured)
        skills = skill_verdict(measured)
    except MeasureError as exc:
        print(str(exc), file=sys.stderr)
        return 1

    ok = all(r["ok"] for r in rows) and skills["ok"]
    if args.json:
        print(
            json.dumps(
                {
                    "ok": ok,
                    "drifted": sum(1 for r in rows if not r["ok"]),
                    "exe": str(exe),
                    "markdown": str(markdown),
                    "server": measured.get("server", {}),
                    "resources": measured.get("resources"),
                    "skills": skills,
                    "rows": rows,
                    "declared": declared,
                    "measured": {k: v for k, v in measured.items() if k != "server"},
                },
                indent=2,
                ensure_ascii=False,
            )
        )
    else:
        print(render_table(rows, skills))
    return 0 if ok else 2


if __name__ == "__main__":
    sys.exit(main())
