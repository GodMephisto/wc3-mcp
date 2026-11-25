# The print palettes and the stylesheet, one dark edition and one light edition.
#
# This began inside build_pdf.py, which had grown past the file length the
# linter allows while carrying three hundred lines of colour values that no
# other line of the build reads. Nothing here imports the build, so the two
# files are a palette and a script that spends it.
#
# One palette per edition. The stylesheet, the mermaid theme, the colour a
# diagram states in its own classDef lines and the PyMuPDF stamp all read from
# here, so the two editions cannot drift apart and a new colour is added in one
# place. The dark values are the palette this build has always used, so a build
# with no flag prints exactly what it printed before.

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
