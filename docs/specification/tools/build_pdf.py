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
MERMAID_URL = "https://cdn.jsdelivr.net/npm/mermaid@10.9.1/dist/mermaid.min.js"
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
    CACHE.mkdir(parents=True, exist_ok=True)
    print("fetching", MERMAID_URL)
    r = requests.get(MERMAID_URL, timeout=60)
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


MERMAID = Path()
CHROME = Path()

WORK = Path(tempfile.mkdtemp(prefix="wc3spec-"))
HTML = WORK / "spec.html"
FIRST = WORK / "first.pdf"
SECOND = WORK / "second.pdf"
FINAL = WORK / "final.pdf"
PORT = 9337


def free_port(start: int) -> int:
    for p in range(start, start + 40):
        with socket.socket() as s:
            try:
                s.bind(("127.0.0.1", p))
                return p
            except OSError:
                continue
    raise SystemExit("no free port")


class Chrome:
    def __init__(self) -> None:
        self.port = free_port(PORT)
        self.proc = subprocess.Popen(
            [
                str(CHROME),
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
        while time.time() < deadline:
            try:
                r = requests.get(f"http://127.0.0.1:{self.port}/json/version", timeout=1)
                if r.ok:
                    break
            except requests.RequestException:
                time.sleep(0.25)
        else:
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

    def eval(self, expr: str):
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
            if self.eval("document.readyState") == "complete":
                return
            time.sleep(0.2)
        raise RuntimeError("page did not load")

    def wait_ready(self, expect_diagrams: int) -> dict:
        deadline = time.time() + 180
        while time.time() < deadline:
            state = self.eval(
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


# One palette per edition. The stylesheet, the mermaid theme, the colour a diagram states in
# its own classDef lines and the PyMuPDF stamp all read from here, so the two editions can
# never drift apart and a new colour is added in one place. The dark values are the palette
# this build has always used, so a build with no flag prints exactly what it printed before.
DARK = {
    "name": "dark",
    "css": {
        "page": "#0a1320",
        "fg": "#dbe4f2",
        "h1": "#f4e8c8",
        "h2-fg": "#f0d99b",
        "h2-bg-a": "#0f1e31",
        "h2-bg-b": "#17304a",
        "accent": "#c8a24a",
        "h3": "#cfe0f5",
        "h4": "#e2c47d",
        "rule-soft": "#223a56",
        "strong": "#f2e6c4",
        "link": "#8fb8e8",
        "code-bg": "#16263c",
        "code-fg": "#cfe0f5",
        "pre-bg": "#08111c",
        "pre-fg": "#d7e3f5",
        "pre-edge": "#5f8fc4",
        "th-bg": "#122338",
        "th-fg": "#e8c877",
        "td-rule": "#1e334c",
        "row-alt": "#0f1c2d",
        "code-table": "#a9c7e8",
        "diagram-bg": "#0b1626",
        "node-bg": "#14263d",
        "node-edge": "#36587e",
        "entity-bg": "#122338",
        "quote": "#c2d2e6",
        "cover-sub": "#b9c9de",
        "cover-meta": "#c8d6e8",
        "cover-label": "#8fa6c4",
        "muted": "#a9bcd4",
        "toc-rule": "#2a4566",
        "cover-frame-a": "rgba(200, 162, 74, 0.65)",
        "cover-frame-b": "rgba(95, 143, 196, 0.45)",
        "cover-stats-rule": "rgba(200, 162, 74, 0.55)",
        "table-pt": "7.2pt",
        "table-code-pt": "7.4pt",
    },
    "mermaid": {
        "background": "#0b1626",
        "primaryColor": "#14263d", "primaryTextColor": "#e8eef7",
        "primaryBorderColor": "#5f8fc4",
        "secondaryColor": "#1a3350", "tertiaryColor": "#0f1c2d",
        "lineColor": "#8fb8e8", "textColor": "#dbe4f2",
        "mainBkg": "#14263d", "nodeBorder": "#5f8fc4",
        "clusterBkg": "#101f33", "clusterBorder": "#36587e",
        "edgeLabelBackground": "#0b1626",
        "actorBkg": "#14263d", "actorBorder": "#5f8fc4", "actorTextColor": "#e8eef7",
        "actorLineColor": "#36587e", "signalColor": "#8fb8e8", "signalTextColor": "#dbe4f2",
        "labelBoxBkgColor": "#14263d", "labelBoxBorderColor": "#5f8fc4",
        "labelTextColor": "#e8eef7",
        "noteBkgColor": "#16324a", "noteBorderColor": "#5f8fc4", "noteTextColor": "#e8eef7",
        "activationBkgColor": "#1d3a5c", "activationBorderColor": "#5f8fc4",
        "pie1": "#5f8fc4", "pie2": "#c8a24a", "pie3": "#3f8f78", "pie4": "#8a6bb5",
        "pie5": "#c07a4a", "pie6": "#4f7fb5", "pie7": "#8a6a2f", "pie8": "#5f9aa8",
        "pie9": "#a8668a", "pie10": "#7a9a4a", "pie11": "#9fb3cc",
        "fontSize": "12px",
    },
    # Diagrams state their own dark fills in classDef lines, so the light edition needs a
    # translation for each of them. The dark edition leaves every diagram source alone.
    "mermaid_hexes": {},
    "stamp": {
        "page": (0.039, 0.075, 0.125),
        "head": (0.55, 0.63, 0.75),
        "rule": (0.13, 0.23, 0.36),
        "foot": (0.62, 0.70, 0.82),
    },
}

# The light printing edition, for a reader who prints the document or annotates it. Every
# colour is chosen for dark text on paper, and the table text grows from 7.2pt to 8.5pt.
LIGHT = {
    "name": "light",
    "css": {
        "page": "#fdfcf7",
        "fg": "#1c2b3a",
        "h1": "#5c4108",
        "h2-fg": "#40300a",
        "h2-bg-a": "#f7eed6",
        "h2-bg-b": "#efe1bf",
        "accent": "#a8801f",
        "h3": "#22364b",
        "h4": "#7a5a11",
        "rule-soft": "#d9cfb9",
        "strong": "#3d2c06",
        "link": "#1b4f86",
        "code-bg": "#efe8d8",
        "code-fg": "#22303f",
        "pre-bg": "#f5f2ea",
        "pre-fg": "#22303f",
        "pre-edge": "#4b6f96",
        "th-bg": "#eee3c9",
        "th-fg": "#40300a",
        "td-rule": "#ddd3bf",
        "row-alt": "#f8f4ea",
        "code-table": "#2f5580",
        "diagram-bg": "#faf7ef",
        "node-bg": "#eaf0f7",
        "node-edge": "#7b9cbd",
        "entity-bg": "#e6ecf4",
        "quote": "#3d4f63",
        "cover-sub": "#3a4c60",
        "cover-meta": "#26394d",
        "cover-label": "#5b6d82",
        "muted": "#4f6278",
        "toc-rule": "#cfc6b0",
        "cover-frame-a": "rgba(168, 128, 31, 0.55)",
        "cover-frame-b": "rgba(75, 111, 150, 0.35)",
        "cover-stats-rule": "rgba(168, 128, 31, 0.45)",
        "table-pt": "8.5pt",
        "table-code-pt": "8.7pt",
    },
    "mermaid": {
        "background": "#faf7ef",
        "primaryColor": "#e8eef7", "primaryTextColor": "#16283c",
        "primaryBorderColor": "#3d6a9c",
        "secondaryColor": "#dfe8f2", "tertiaryColor": "#f4f1e8",
        "lineColor": "#4a627e", "textColor": "#1c2b3a",
        "mainBkg": "#e8eef7", "nodeBorder": "#3d6a9c",
        "clusterBkg": "#f4f1e8", "clusterBorder": "#b9c6d4",
        "edgeLabelBackground": "#faf7ef",
        "actorBkg": "#e8eef7", "actorBorder": "#3d6a9c", "actorTextColor": "#16283c",
        "actorLineColor": "#b9c6d4", "signalColor": "#4a627e", "signalTextColor": "#1c2b3a",
        "labelBoxBkgColor": "#e8eef7", "labelBoxBorderColor": "#3d6a9c",
        "labelTextColor": "#16283c",
        "noteBkgColor": "#fbf0cf", "noteBorderColor": "#a8801f", "noteTextColor": "#40300a",
        "activationBkgColor": "#dbe5ef", "activationBorderColor": "#3d6a9c",
        "pie1": "#3d6a9c", "pie2": "#a8801f", "pie3": "#2f6f5c", "pie4": "#5c4f96",
        "pie5": "#a05c2a", "pie6": "#345f8c", "pie7": "#7a5a11", "pie8": "#3f7a86",
        "pie9": "#8a4f70", "pie10": "#5c7a30", "pie11": "#6a7d93",
        "fontSize": "12px",
    },
    "mermaid_hexes": {
        "#12243a": "#e6eef8", "#4f7fb5": "#3d6a9c", "#dbe7f6": "#16293c",
        "#0f2a2a": "#e3f1ea", "#3f8f78": "#2f6f5c", "#cfeee2": "#14342b",
        "#2a2312": "#f7ecd0", "#c8a24a": "#8a6a1f", "#f2dfae": "#3f2f0a",
        "#1d1a33": "#ebe7f7", "#7a6bb5": "#5c4f96", "#e2dcf5": "#241d3f",
        "#0f1a28": "#e9eef5", "#5b7a9c": "#45637f", "#cddced": "#1b2b3a",
        "#101c2a": "#eef2f6", "#405a78": "#526a83", "#b9cbe0": "#1f3145",
        "#14263d": "#eaf0f8", "#5f8fc4": "#3d6a9c", "#dbe7f2": "#1a2b3d",
    },
    "stamp": {
        "page": (0.992, 0.988, 0.969),
        "head": (0.361, 0.400, 0.451),
        "rule": (0.796, 0.824, 0.855),
        "foot": (0.412, 0.451, 0.502),
    },
}

PALETTES = {"dark": DARK, "light": LIGHT}

# The stylesheet, shared by both editions. Every colour and the table size is a custom
# property, so the palette lives in DARK and LIGHT and not in the rules.
CSS = r"""
@page { size: A4; margin: 15mm 14mm 16mm 14mm; }
@page wide { size: A4 landscape; margin: 16mm 14mm 18mm 14mm; }
.widepage { page: wide; break-before: page; }
.widepage h3 { margin-top: 0; }
* { box-sizing: border-box; }
html { font-size: 9pt; background: var(--page); }
body {
  font-family: "Palatino Linotype", "Book Antiqua", Palatino, Georgia, serif;
  color: var(--fg); line-height: 1.32; margin: 0;
  -webkit-print-color-adjust: exact; print-color-adjust: exact;
}
h1, h2, h3, h4, .kicker, .meta, .stats, table, .toc-row {
  font-family: "Palatino Linotype", "Book Antiqua", Palatino, Georgia, serif;
}
h1 { font-size: 26pt; line-height: 1.1; color: var(--h1); margin: 0 0 10pt; letter-spacing: -0.3pt; }
h2 {
  font-size: 14pt; color: var(--h2-fg); margin: 15pt 0 8pt; padding: 6pt 8pt 5pt;
  background: linear-gradient(90deg, var(--h2-bg-a) 0%, var(--h2-bg-b) 100%);
  border-left: 5pt solid var(--accent);
  letter-spacing: 0.25pt; break-before: auto; break-after: avoid;
  box-shadow: 0 1.5pt 0 var(--accent);
}
h3 {
  font-size: 11pt; color: var(--h3); margin: 11pt 0 4pt; break-after: avoid;
  font-variant: small-caps; letter-spacing: 0.4pt;
}
h3::before { content: "\25C6"; color: var(--accent); font-size: 8.5pt; margin-right: 6pt; vertical-align: 1pt; }
h4 {
  font-size: 10.4pt; color: var(--h4); margin: 13pt 0 4pt; break-after: avoid;
  border-bottom: 0.6pt solid var(--rule-soft); padding-bottom: 2pt;
}
p { margin: 0 0 6pt; text-align: justify; }
ul, ol { margin: 0 0 8pt; padding-left: 17pt; }
li { margin: 0 0 3.4pt; }
li::marker { color: var(--accent); }
strong { color: var(--strong); }
a { color: var(--link); }
code {
  font-family: Consolas, "Cascadia Mono", monospace; font-size: 8.2pt;
  background: var(--code-bg); color: var(--code-fg); padding: 0 1.5pt; border-radius: 2px;
  overflow-wrap: anywhere;
}
pre {
  font-family: Consolas, "Cascadia Mono", monospace; font-size: 7.6pt; line-height: 1.3;
  background: var(--pre-bg); color: var(--pre-fg);
  border-left: 3pt solid var(--pre-edge); padding: 8pt 10pt;
  margin: 0 0 10pt; white-space: pre-wrap; overflow-wrap: anywhere; break-inside: avoid;
}
pre code { background: none; color: inherit; padding: 0; font-size: inherit; }
table {
  width: 100%; border-collapse: collapse; margin: 0 0 8pt;
  font-size: var(--table-pt); line-height: 1.28;
}
thead { display: table-header-group; }
tr { break-inside: avoid; }
th {
  background: var(--th-bg); color: var(--th-fg); text-align: left; font-weight: 600;
  border-bottom: 1pt solid var(--accent); padding: 2.4pt 3pt;
  font-variant: small-caps; letter-spacing: 0.3pt;
}
td { border-bottom: 0.5pt solid var(--td-rule); padding: 2.2pt 3pt; vertical-align: top; }
tbody tr:nth-child(even) td { background: var(--row-alt); }
td code, th code { font-size: var(--table-code-pt); background: none; padding: 0; color: var(--code-table); }
table.inv { table-layout: fixed; }
table.inv th:nth-child(1), table.inv td:nth-child(1) { width: 19%; }
table.inv th:nth-child(2), table.inv td:nth-child(2) { width: 46%; }
table.inv th:nth-child(3), table.inv td:nth-child(3) { width: 19%; }
table.inv th:nth-child(4), table.inv td:nth-child(4) { width: 6%; }
table.inv th:nth-child(5), table.inv td:nth-child(5) { width: 10%; }
table.inv td:nth-child(1) code { white-space: nowrap; overflow-wrap: normal; }
table.tight th:first-child, table.tight td:first-child { white-space: nowrap; width: 9%; }
.diagram {
  break-inside: avoid; margin: 4pt 0 9pt; text-align: center;
  background: var(--diagram-bg); border: 0.6pt solid var(--rule-soft); padding: 7pt 5pt;
  border-radius: 2px;
}
.diagram svg { max-width: 100% !important; max-height: 195mm !important; width: auto !important; height: auto !important; display: block; margin: 0 auto; }
.er.attributeBoxOdd, .er.attributeBoxEven, .er.attributeBackground,
.attributeBoxOdd, .attributeBoxEven { fill: var(--node-bg) !important; stroke: var(--node-edge) !important; }
.er.entityBox, .entityBox { fill: var(--entity-bg) !important; stroke: var(--accent) !important; }
.er.entityLabel, .er.attributeLabel, .er.relationshipLabel,
.entityLabel, .attributeLabel, .relationshipLabel { fill: var(--fg) !important; }
.er.relationshipLine, .relationshipLine { stroke: var(--link) !important; }
.mermaid { background: none; border: none; padding: 0; margin: 0; }
blockquote { margin: 0 0 9pt; padding: 2pt 0 2pt 10pt; border-left: 2.5pt solid var(--accent); color: var(--quote); }

/* cover, the framed plate, one palette per edition */
.cover {
  position: relative; margin: 0; padding: 20mm 18mm 14mm;
  width: auto; height: 259mm; display: flex; flex-direction: column;
  background: var(--page);
  color: var(--fg); break-after: page;
}
.cover::before {
  content: ""; position: absolute; inset: 6mm; border: 1pt solid var(--cover-frame-a);
}
.cover::after {
  content: ""; position: absolute; inset: 8mm; border: 0.5pt solid var(--cover-frame-b);
}
.cover .crest { width: 30mm; height: 30mm; margin: 2mm 0 0; }
.cover .kicker {
  font-size: 9pt; letter-spacing: 3pt; text-transform: uppercase; color: var(--accent);
  margin-top: 9mm; font-variant: small-caps;
}
.cover h1 {
  font-size: 33pt; margin: 7mm 0 5mm; color: var(--strong); letter-spacing: 0.2pt;
}
.cover .sub {
  font-size: 12.5pt; line-height: 1.45; color: var(--cover-sub); max-width: 140mm; text-align: left;
}
.cover .rule { border-top: 1.6pt solid var(--accent); margin: 12mm 0 7mm; width: 52mm; }
.cover .meta { font-size: 9.2pt; color: var(--cover-meta); margin-top: 2mm; }
.cover .meta div { margin: 0 0 4mm; }
.cover .meta span {
  display: inline-block; width: 30mm; color: var(--cover-label); text-transform: uppercase;
  letter-spacing: 1.4pt; font-size: 7.4pt; font-variant: small-caps;
}
.cover .stats {
  margin-top: auto; font-size: 9pt; color: var(--muted);
  border-top: 0.8pt solid var(--cover-stats-rule); padding-top: 4mm;
}

/* contents */
.toc { break-after: page; }
.toc-cols { column-count: 2; column-gap: 10mm; }
.toc h2 { break-before: avoid; }
.toc-row { display: flex; align-items: baseline; gap: 5pt; font-size: 9.4pt; margin: 0 0 2.6pt; }
.toc-row.lvl3 { margin-left: 9mm; font-size: 8.8pt; color: var(--muted); }
.toc-row a { color: inherit; text-decoration: none; }
.toc-row .fill { flex: 1; border-bottom: 0.7pt dotted var(--toc-rule); transform: translateY(-2px); }
.toc-row .pg { min-width: 8mm; text-align: right; color: var(--accent); font-variant-numeric: tabular-nums; }
"""


def stylesheet(theme: dict) -> str:
    """The print stylesheet for one palette, with the palette declared as custom properties."""
    root = "\n".join(f"  --{name}: {value};" for name, value in theme["css"].items())
    return f":root {{\n{root}\n}}\n{CSS}"


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


def build_html(body: str, items, pages=None, theme: dict = DARK) -> str:
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
<script src="{MERMAID.as_uri()}"></script>
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


def check_surface() -> None:
    """Refuse to print a document whose body numbers disagree with the exe."""
    result = subprocess.run(
        [sys.executable, str(MEASURE), "--json"],
        cwd=HERE,
        capture_output=True,
        text=True,
    )
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
    result = subprocess.run(
        [sys.executable, str(COUNTS), "--json"],
        cwd=HERE,
        capture_output=True,
        text=True,
    )
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
    global MERMAID, CHROME
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
    MERMAID = resolve_mermaid()
    CHROME = find_chrome()
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

    HTML.write_text(build_html(body, toc_items, theme=theme), encoding="utf-8", newline="\n")

    chrome = Chrome()
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

        HTML.write_text(build_html(body, toc_items, [pages[items.index(x)] for x in toc_items], theme=theme), encoding="utf-8", newline="\n")
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
