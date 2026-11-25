"""Build a print PDF from the wc3-mcp specification markdown.

Two passes. The first prints the document with a placeholder table of contents, the second
fills in each heading's real page number found by searching the first PDF, then the footer,
metadata and bookmark outline are stamped with PyMuPDF.

Chrome renders the HTML, including the mermaid diagrams, from a local mermaid build. The PDF
is produced over the DevTools protocol, because the headless --print-to-pdf flag does nothing
on this Chrome build.

Before rendering, measure_surface.py compares the numbers the document states in sections
3.1, 3.3 and 3.5 with what the published exe reports, and check_counts.py compares the
counts the document and the README state about themselves with the exe, requirements.json
and the markdown. The build refuses on either drift. Pass --skip-measure to render a
document edit with no published server.

Pass --light for the light printing edition. It renders the same markdown, the same diagrams
and the same bookmarks from the same stylesheet, with a paper palette, dark text, table text at
8.5pt instead of 7.2pt, and the file name ending -light. The dark edition is what a build with
no flag writes, and neither edition removes the other.
"""

import base64
import hashlib
import html
import json
import os
import re
import socket
import subprocess
import sys
import tempfile
import time
from pathlib import Path
from urllib.parse import quote

import fitz  # PyMuPDF
import markdown
import requests
import websocket

HERE = Path(__file__).resolve().parent
# The palette lives beside this script, and the build is run from the tools directory or from
# the repository root depending on who calls it, so the script's own directory goes on the path.
sys.path.insert(0, str(HERE))
from spec_theme import DARK, PALETTES, stylesheet  # noqa: E402  (after the path insert)
DOCS = HERE.parent
MD = DOCS / "wc3-mcp-specification-v1.md"


# The light printing edition's suffix, used by the output name and by the prune.
LIGHT_SUFFIX = "-light"


def output_path(version: str, light: bool = False) -> Path:
    """The dark print edition's name carries the revision, so a reader can tell two builds apart.

    The light printing edition sits beside it at a fixed name, so a reader can always find the
    current one, and a dark build never removes it.
    """
    if light:
        return DOCS / f"wc3-mcp-specification-v1{LIGHT_SUFFIX}.pdf"
    return DOCS / f"wc3-mcp-specification-v{version}.pdf"


def prune_old(output: Path) -> None:
    """Remove the superseded files of this edition, and leave the other edition alone."""
    light = output.stem.endswith(LIGHT_SUFFIX)
    for stale in sorted(DOCS.glob("wc3-mcp-specification-v*.pdf")):
        if stale == output:
            continue
        if stale.stem.endswith(LIGHT_SUFFIX) != light:
            continue  # the other edition is not superseded by this build
        stale.unlink()
        print("removed the superseded", stale.name)


CACHE = HERE / ".cache"
MEASURE = HERE / "measure_surface.py"
COUNTS = HERE / "check_counts.py"
MERMAID_URL_VAR = "MERMAID_URL"
MERMAID_SHA256 = "61b335a46df05a7ce1c98378f60e5f3e77a7fb608a1056997e8a649304a936d6"

# Filled from the markdown by document_facts, so the cover, the page footer and the
# body can never disagree about how many requirements or gaps the document holds.
FACTS = {"tools": 0, "requirements": 0, "acs": 0, "gaps": 0, "version": "1"}


def document_facts(text: str) -> dict:
    """Count what the document says about itself, from the document."""
    tools = sum(int(n) for n in re.findall(r"^\*\*.+?\*\* \((\d+)\)\.", text, re.M))
    reqs = len([1 for m in re.finditer(r"^\| ([A-Z]{2,4}-\d{2}) \|", text, re.M)
                if not m.group(1).startswith(("AC-", "G-"))])
    acs = len(re.findall(r"^\| AC-\d{2} \|", text, re.M))
    gaps = len(re.findall(r"^\| G-\d{2,3} \|", text, re.M))
    versions = re.findall(r"^\| (\d+(?:\.\d+)?) \| \d+ \w+ \d{4} \|", text, re.M)
    version = versions[-1] if versions else "1"
    return {"tools": tools, "requirements": reqs, "acs": acs, "gaps": gaps, "version": version}


def sha256(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def resolve_mermaid() -> Path:
    """The pinned mermaid build, from tools/.cache, downloaded and verified once."""
    target = CACHE / "mermaid-10.9.1.min.js"
    if target.exists() and sha256(target) == MERMAID_SHA256:
        return target
    url = os.environ.get(MERMAID_URL_VAR)
    if not url:
        raise SystemExit(
            f"the pinned mermaid build is not in {CACHE}. Set {MERMAID_URL_VAR} to the url "
            "of the mermaid 10.9.1 min.js release file. The bytes are checked against the "
            f"pinned sha256 {MERMAID_SHA256}, so any mirror of that release will do"
        )
    CACHE.mkdir(parents=True, exist_ok=True)
    print("fetching", url)
    r = requests.get(url, timeout=60)
    r.raise_for_status()
    target.write_bytes(r.content)
    actual = sha256(target)
    if actual != MERMAID_SHA256:
        target.unlink(missing_ok=True)
        raise SystemExit(f"mermaid checksum mismatch, expected {MERMAID_SHA256}, got {actual}")
    return target


def find_chrome() -> Path:
    candidates = [
        os.environ.get("CHROME"),
        r"C:\Program Files\Google\Chrome\Application\chrome.exe",
        r"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
        os.path.join(os.environ.get("LOCALAPPDATA", ""), "Google", "Chrome", "Application", "chrome.exe"),
        "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
        "/usr/bin/google-chrome",
        "/usr/bin/chromium",
    ]
    for c in candidates:
        if c and Path(c).exists():
            return Path(c)
    raise SystemExit("no Chrome found, set CHROME to the browser binary")


WORK = Path(tempfile.mkdtemp(prefix="wc3spec-"))
HTML = WORK / "spec.html"
FIRST = WORK / "first.pdf"
SECOND = WORK / "second.pdf"
FINAL = WORK / "final.pdf"
PORT = 9337


def free_port(start: int) -> int:
    """The first port that binds, or a failure that carries the last refusal."""
    last_error = "nothing was tried"
    for p in range(start, start + 40):
        with socket.socket() as s:
            try:
                s.bind(("127.0.0.1", p))
                return p
            except OSError as error:
                last_error = str(error)
                continue
    raise SystemExit(f"no free port in {start} to {start + 39}, the last bind said {last_error}")


class Chrome:
    def __init__(self, binary: Path) -> None:
        self.port = free_port(PORT)
        self.proc = subprocess.Popen(
            [
                str(binary),
                "--headless=new",
                "--disable-gpu",
                "--no-first-run",
                "--no-default-browser-check",
                "--allow-file-access-from-files",
                "--disable-dev-shm-usage",
                f"--user-data-dir={WORK / 'profile'}",
                f"--remote-debugging-port={self.port}",
                "about:blank",
            ],
            stdout=subprocess.DEVNULL,
            stderr=subprocess.DEVNULL,
        )
        deadline = time.time() + 40
        opened = False
        while time.time() < deadline:
            try:
                opened = requests.get(
                    f"http://127.0.0.1:{self.port}/json/version", timeout=1
                ).ok
            except requests.RequestException:
                opened = False
            if opened:
                break
            time.sleep(0.25)
        if not opened:
            raise SystemExit("chrome did not open a debugging port")

    def new_tab(self, url: str) -> "Tab":
        r = requests.put(
            f"http://127.0.0.1:{self.port}/json/new?{quote(url, safe=':/')}", timeout=10
        )
        r.raise_for_status()
        return Tab(r.json()["webSocketDebuggerUrl"])

    def close(self) -> None:
        try:
            self.proc.terminate()
            self.proc.wait(timeout=10)
        except Exception:
            self.proc.kill()


class Tab:
    def __init__(self, ws_url: str) -> None:
        self.ws = websocket.create_connection(ws_url, timeout=180)
        self.i = 0
        self.cmd("Page.enable")
        self.cmd("Runtime.enable")

    def cmd(self, method: str, **params):
        self.i += 1
        self.ws.send(json.dumps({"id": self.i, "method": method, "params": params}))
        while True:
            msg = json.loads(self.ws.recv())
            if msg.get("id") == self.i:
                if "error" in msg:
                    raise RuntimeError(f"{method}: {msg['error']}")
                return msg.get("result", {})

    def expr_value(self, expr: str):
        """The value the page computes for one expression, read back by value."""
        r = self.cmd(
            "Runtime.evaluate",
            expression=expr,
            returnByValue=True,
            awaitPromise=True,
        )
        return r.get("result", {}).get("value")

    def open(self, url: str) -> None:
        self.cmd("Page.navigate", url=url)
        deadline = time.time() + 60
        while time.time() < deadline:
            if self.expr_value("document.readyState") == "complete":
                return
            time.sleep(0.2)
        raise RuntimeError("page did not load")

    def wait_ready(self, expect_diagrams: int) -> dict:
        deadline = time.time() + 180
        while time.time() < deadline:
            state = self.expr_value(
                """(() => ({
                  title: document.title,
                  svg: document.querySelectorAll('.mermaid svg').length,
                  errors: Array.from(document.querySelectorAll('.mermaid')).filter(e =>
                     e.querySelector('text.error-text') !== null).length,
                  messages: Array.from(document.querySelectorAll('.mermaid text.error-text'))
                     .map(e => e.textContent).slice(0, 3)
                }))()"""
            )
            if state and state.get("title", "").startswith("READY"):
                return state
            time.sleep(0.4)
        return state or {}

    def print_pdf(self, target: Path, timeout_ms: int = 180000) -> None:
        r = self.cmd(
            "Page.printToPDF",
            printBackground=True,
            preferCSSPageSize=True,
            displayHeaderFooter=False,
            marginTop=0,
            marginBottom=0,
            marginLeft=0,
            marginRight=0,
            transferMode="ReturnAsBase64",
        )
        target.write_bytes(base64.b64decode(r["data"]))


def esc(s: str) -> str:
    return html.escape(s, quote=False)


def extract_mermaid(text: str) -> tuple[str, list[str]]:
    blocks: list[str] = []

    def repl(m):
        blocks.append(m.group(1))
        return "\n\n@@MERMAID%d@@\n\n" % (len(blocks) - 1)

    text = re.sub(r"```mermaid\n(.*?)```", repl, text, flags=re.S)
    return text, blocks


def render_markdown(text: str, mermaid_blocks: list[str]) -> str:
    md = markdown.Markdown(
        extensions=["extra", "sane_lists", "md_in_html", "toc"],
        output_format="html5",
    )
    body = md.convert(text)

    def put(m):
        i = int(m.group(1))
        return (
            '<div class="diagram"><pre class="mermaid">'
            + html.escape(mermaid_blocks[i])
            + "</pre></div>"
        )

    body = re.sub(r"<p>@@MERMAID(\d+)@@</p>", put, body)
    body = re.sub(r"@@MERMAID(\d+)@@", put, body)
    body = re.sub(r'(<h4[^>]*>.*?</h4>\s*)<table>', lambda m: m.group(1) + '<table class="inv">', body, flags=re.S)
    body = re.sub(r'<table>(\s*<thead>\s*<tr>\s*<th>(?:ID|Gap)</th>)', lambda m: '<table class="tight">' + m.group(1), body)
    body = re.sub(
        r'(<h3 id="the-model-in-one-picture">.*?</h3>\s*<div class="diagram">.*?</div>\s*<p><em>.*?</em></p>)',
        lambda m: '<section class="widepage">' + m.group(1) + '</section>',
        body, flags=re.S,
    )
    return body


def headings(body: str):
    out = []
    for m in re.finditer(r'<h([23]) id="([^"]+)">(.*?)</h\1>', body, re.S):
        level = int(m.group(1))
        hid = m.group(2)
        label = re.sub(r"<[^>]+>", "", m.group(3))
        label = html.unescape(label).strip()
        out.append((level, hid, label))
    return out


def toc_html(items, pages=None) -> str:
    rows = []
    for idx, (level, hid, label) in enumerate(items):
        page = ""
        if pages and idx < len(pages) and pages[idx]:
            page = str(pages[idx])
        rows.append(
            f'<div class="toc-row lvl{level}">'
            f'<a href="#{hid}">{esc(label)}</a>'
            f'<span class="fill"></span><span class="pg">{page}</span></div>'
        )
    return '<div class="toc-cols">' + "\n".join(rows) + "</div>"


def recolour_mermaid(source: str, mapping: dict) -> str:
    """Rewrite the hex colours a diagram states for a light page, leaving the diagram itself alone.

    The markdown classDef lines carry the dark fills, and the light edition cannot read
    dark-on-white. An empty mapping is the dark edition, where every source is untouched.
    """
    if not mapping:
        return source
    return re.sub(
        r"#[0-9a-fA-F]{6}\b",
        lambda m: mapping.get(m.group(0).lower(), m.group(0)),
        source,
    )


def build_html(body: str, items, pages=None, theme: dict = DARK, mermaid: Path | None = None) -> str:
    return f"""<!doctype html>
<html lang="en"><head><meta charset="utf-8">
<title>wc3-mcp server specification</title>
<style>{stylesheet(theme)}</style></head>
<body>
<section class="cover">
  <svg class="crest" viewBox="0 0 120 120" xmlns="http://www.w3.org/2000/svg">
    <path d="M60 6 L108 26 V62 C108 88 88 104 60 114 C32 104 12 88 12 62 V26 Z" fill="none" stroke="#d8b45a" stroke-width="1.8"/>
    <path d="M60 18 L96 33 V62 C96 82 81 94 60 102 C39 94 24 82 24 62 V33 Z" fill="none" stroke="#a9822f" stroke-width="0.9"/>
    <path d="M50 46 V74 M60 41 V79 M70 46 V74" stroke="#d8b45a" stroke-width="2.4" stroke-linecap="round"/>
    <path d="M36 60 H45 M75 60 H84" stroke="#a9822f" stroke-width="1.2"/>
  </svg>
  <div class="kicker">Specification</div>
  <h1>wc3-mcp server specification</h1>
  <p class="sub">The MCP surface, its safety rules, its result contract, and the requirements for the next version.</p>
  <div class="rule"></div>
  <div class="meta">
    <div><span>Version</span>{FACTS["version"]}, draft for review</div>
    <div><span>Describes</span>wc3ctl 0.4.13 and wc3-mcp 0.5.16</div>
    <div><span>Measured</span>8 November 2026</div>
    <div><span>Owner</span>the wc3ctl maintainer</div>
  </div>
  <div class="stats">{FACTS["tools"]} tools &nbsp;·&nbsp; {FACTS["requirements"]} requirements &nbsp;·&nbsp; {FACTS["acs"]} acceptance checks &nbsp;·&nbsp; {FACTS["gaps"]} recorded gaps</div>
</section>
<section class="toc">
  <h2>Contents</h2>
  {toc_html(items, pages)}
</section>
{body}
<script src="{mermaid.as_uri()}"></script>
<script>
  mermaid.initialize({{
    startOnLoad: false,
    securityLevel: "loose",
    theme: "base",
    fontFamily: "Segoe UI, Arial, sans-serif",
    themeVariables: {json.dumps(theme["mermaid"], indent=6)},
    flowchart: {{ htmlLabels: true, curve: "basis" }},
    sequence: {{ useMaxWidth: true, wrap: true, actorFontSize: 11, noteFontSize: 11, messageFontSize: 11 }}
  }});
  mermaid.run().then(() => {{ document.title = "READY wc3-mcp server specification"; }});
</script>
</body></html>"""


def find_pages(pdf: Path, items) -> list[int]:
    doc = fitz.open(pdf)
    pages = []
    cursor = 0
    texts = [doc[i].get_text() for i in range(len(doc))]
    cursor = 0
    marker = "This section defines the words this document uses"
    for i, t in enumerate(texts):
        if marker in re.sub(r"\s+", " ", t):
            cursor = i
            break
    for level, hid, label in items:
        needle = re.sub(r"\s+", " ", label).strip()
        found = None
        for i in range(cursor, len(texts)):
            flat = re.sub(r"\s+", " ", texts[i])
            if needle and needle in flat:
                found = i + 1
                cursor = i
                break
        if found is None and len(needle) > 24:
            short = needle[:24]
            for i in range(cursor, len(texts)):
                if short in re.sub(r"\s+", " ", texts[i]):
                    found = i + 1
                    cursor = i
                    break
        pages.append(found or 0)
    doc.close()
    return pages


def stamp(pdf: Path, out: Path, items, pages, theme: dict = DARK) -> None:
    doc = fitz.open(pdf)
    total = len(doc)
    ink = theme["stamp"]
    page_color = ink["page"]
    for i, page in enumerate(doc):
        page.draw_rect(page.rect, color=None, fill=page_color, width=0, overlay=False)
        if i == 0:
            continue
        w, h = page.rect.width, page.rect.height
        page.insert_text((42, 24), "wc3-mcp server specification", fontsize=7, fontname="helv", color=ink["head"])
        rw = fitz.get_text_length(f"version {FACTS['version']}", fontname="helv", fontsize=7)
        page.insert_text((w - 42 - rw, 24), f"version {FACTS['version']}", fontsize=7, fontname="helv", color=ink["head"])
        page.draw_line(fitz.Point(42, 30), fitz.Point(w - 42, 30), color=ink["rule"], width=0.5)
        y = h - 30
        page.draw_line(
            fitz.Point(42, y - 9), fitz.Point(w - 42, y - 9),
            color=ink["rule"], width=0.6,
        )
        left = f"wc3-mcp server specification  |  version {FACTS['version']}"
        right = f"page {i + 1} of {total}"
        page.insert_text((42, y), left, fontsize=7.2, fontname="helv", color=ink["foot"])
        rw = fitz.get_text_length(right, fontname="helv", fontsize=7.2)
        page.insert_text((w - 42 - rw, y), right, fontsize=7.2, fontname="helv", color=ink["foot"])
    toc = []
    for idx, (level, hid, label) in enumerate(items):
        p = pages[idx] if idx < len(pages) and pages[idx] else 1
        toc.append([1 if level == 2 else 2, label, p])
    doc.set_toc(toc)
    doc.set_metadata(
        {
            "title": "wc3-mcp server specification",
            "author": "wc3ctl",
            "subject": "MCP server surface, safety rules, result contract and requirements",
            "keywords": "MCP, warcraft III, wc3ctl, wc3-mcp, specification",
            "creator": "wc3ctl specification build",
            "producer": "Chrome and PyMuPDF",
        }
    )
    doc.save(out, garbage=4, deflate=True)
    doc.close()


def run_guard(script: Path) -> subprocess.CompletedProcess:
    """Run a guard script and return it whole.

    The guard's exit code is its answer, 0 for agree and 2 for drift, so a
    non-zero code must reach the caller rather than raise here.
    """
    return subprocess.run(
        [sys.executable, str(script), "--json"],
        cwd=HERE,
        capture_output=True,
        text=True,
        check=False,
    )


def check_surface() -> None:
    """Refuse to print a document whose body numbers disagree with the exe."""
    result = run_guard(MEASURE)
    if result.returncode == 1:
        raise SystemExit(
            "the surface measurement could not run, so the PDF was not built.\n"
            + (result.stderr or result.stdout).strip()
        )
    if result.returncode not in (0, 2):
        raise SystemExit(
            "the surface measurement failed with exit "
            f"{result.returncode}, so the PDF was not built.\n"
            + (result.stderr or result.stdout).strip()
        )
    try:
        data = json.loads(result.stdout)
    except json.JSONDecodeError:
        raise SystemExit(
            "the surface measurement printed no usable JSON, so the PDF was not built."
        )

    if result.returncode == 0:
        print("surface measurement agrees with the document")
        return

    def word(value) -> str:
        if isinstance(value, bool):
            return "true" if value else "false"
        return f"{value:,}" if isinstance(value, int) else str(value)

    lines = [
        f"  section {row['section']} says {row['label']} is {word(row['declared'])}, "
        f"the server reports {word(row['measured'])}"
        for row in data["rows"]
        if not row["ok"]
    ]
    raise SystemExit(
        "the document disagrees with the published server, so the PDF was not built.\n"
        + "\n".join(lines)
        + f"\nChange those values in the Value column of {MD.name}, then build again.\n"
        "To render a document edit with no published server, pass --skip-measure."
    )


def check_counts() -> None:
    """Refuse to print a document whose own counts disagree with its data."""
    result = run_guard(COUNTS)
    if result.returncode == 1:
        raise SystemExit(
            "the count check could not run, so the PDF was not built.\n"
            + (result.stderr or result.stdout).strip()
        )
    if result.returncode not in (0, 2):
        raise SystemExit(
            "the count check failed with exit "
            f"{result.returncode}, so the PDF was not built.\n"
            + (result.stderr or result.stdout).strip()
        )
    try:
        data = json.loads(result.stdout)
    except json.JSONDecodeError:
        raise SystemExit(
            "the count check printed no usable JSON, so the PDF was not built."
        )

    if result.returncode == 0:
        print("document counts agree with the data and the exe")
        return

    def word(value) -> str:
        if isinstance(value, bool):
            return "true" if value else "false"
        return f"{value:,}" if isinstance(value, int) else str(value)

    lines = [
        f"  {row['source']} says {word(row['declared'])} {row['noun']}, "
        f"{row['holds']} {word(row['actual'])}"
        for row in data["rows"]
        if not row["ok"]
    ]
    raise SystemExit(
        "the document's own counts disagree with the data, so the PDF was not built.\n"
        + "\n".join(lines)
        + "\nChange each stale figure where it is stated, in the markdown or the README."
        + "\nTo render a document edit without these checks, pass --skip-measure."
    )


def main() -> None:
    light = "--light" in sys.argv[1:]
    theme = PALETTES["light" if light else "dark"]
    if "--skip-measure" in sys.argv[1:]:
        print(
            "surface measurement and count checks skipped, "
            "the document may not match the exe"
        )
    else:
        check_surface()
        check_counts()
    mermaid = resolve_mermaid()
    browser_binary = find_chrome()
    text = MD.read_text(encoding="utf-8")
    FACTS.update(document_facts(text))
    print("document facts", FACTS)
    print("edition", theme["name"])
    out = output_path(FACTS["version"], light)
    # The print edition has a cover, so the markdown title block is dropped from the body.
    text = re.sub(r"\A# .*?\n(?=## )", "", text, flags=re.S)
    text, blocks = extract_mermaid(text)
    blocks = [recolour_mermaid(block, theme["mermaid_hexes"]) for block in blocks]
    body = render_markdown(text, blocks)
    items = headings(body)
    toc_items = [x for x in items if x[0] == 2]

    HTML.write_text(
        build_html(body, toc_items, theme=theme, mermaid=mermaid),
        encoding="utf-8", newline="\n")

    chrome = Chrome(browser_binary)
    try:
        tab = chrome.new_tab(HTML.as_uri())
        state = tab.wait_ready(len(blocks))
        print("render", state)
        if state.get("errors"):
            raise SystemExit("mermaid reported an error")
        if state.get("svg", 0) < len(blocks):
            print("warning: expected", len(blocks), "diagrams, saw", state.get("svg"))
        tab.print_pdf(FIRST)
        pages = find_pages(FIRST, items)
        print("first pass pages", len(fitz.open(FIRST)), "toc entries", len(items))
        miss = [(items[i][2], pages[i]) for i in range(len(items)) if not pages[i]]
        if miss:
            print("headings not found", miss[:8])

        HTML.write_text(
            build_html(body, toc_items, [pages[items.index(x)] for x in toc_items],
                       theme=theme, mermaid=mermaid),
            encoding="utf-8", newline="\n")
        tab.open(HTML.as_uri())
        state = tab.wait_ready(len(blocks))
        tab.print_pdf(FINAL)
        pages2 = find_pages(FINAL, items)
        if pages2 != pages:
            print("pagination shifted on the second pass")
        stamp(FINAL, out, items, pages2, theme)
        prune_old(out)
        print("wrote", out, out.stat().st_size, "bytes")
    finally:
        chrome.close()


if __name__ == "__main__":
    main()
