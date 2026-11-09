# wc3-mcp server specification

This folder holds the specification for the wc3-mcp server, the PDF printed from it, and the tool that prints the PDF.

| File | What it is |
|---|---|
| `wc3-mcp-specification-v1.md` | The specification itself. This is the source |
| `wc3-mcp-specification-v<revision>.pdf` | The print edition, generated from the markdown. The name carries the revision, so the file is `-v1.12.pdf` for revision 1.12, and the build removes the superseded file of the same edition |
| `wc3-mcp-specification-v<revision>-light.pdf` | The light printing edition, from `--light`, with a white background and larger table text |
| `tools/build_pdf.py` | The build that turns the markdown into the PDF |
| `tools/measure_surface.py` | The guard that compares sections 3.1, 3.3 and 3.5 with the published exe |
| `tools/sync_requirements.py` | Regenerates `requirements.json` and the Status column |
| `requirements.json` | Every requirement, its gap link, its status and its test |

The document describes wc3ctl 0.4.13 and wc3-mcp 0.5.16, with the tool surface measured on 8 November 2026 and re-measured after each slice. It states 179 requirements in 20 groups, 41 acceptance checks, the 275-tool inventory, 63 gap rows, the hardening designs in section 20, the anime-map workstreams in section 21, the content pipeline and its pjass gate in section 22, the permission model in section 23, and the sources and verification log in Appendices D and E.

## Rebuild the PDF

Prerequisites.

- Python 3.11 or later, with `markdown`, `PyMuPDF`, `requests` and `websocket-client`.
- Google Chrome. Set `CHROME` if it is not in a usual place.
- Network access on the first build only, to fetch the pinned mermaid build. It is cached in `tools/.cache` and verified against a pinned SHA-256 on every run.

From this folder.

```
python tools/build_pdf.py
python tools/build_pdf.py --light    # the light printing edition
```

The build runs `tools/measure_surface.py` and `tools/check_counts.py` first. That tool starts the published server, reads sections 3.1, 3.3 and 3.5 out of the markdown, and refuses to print when a figure disagrees with the exe. It looks for `dist-mcp/Wc3.Mcp.exe` in `../Wc3_CLI` and can be pointed anywhere with `--exe` or `WC3_MCP_EXE`. To print a document edit with no server at hand, pass `--skip-measure`, and record which exe the numbers came from.

The build does this.

1. Converts the markdown to HTML, with the mermaid diagrams kept as diagram sources.
2. Renders the HTML to PDF through Chrome over the DevTools protocol, so the diagrams are vector and the CSS page rules apply.
3. Reads the first PDF back, finds the page of every heading, and prints a second pass with a real page number beside each contents entry.
4. Stamps the footer, the document metadata and the bookmark outline with PyMuPDF.

The markdown is the file to edit. The PDF is the artifact, and both are kept in step.

## Keeping the status

`requirements.json` holds every requirement, its gap link, its status and its test. It is the source. The Status column in the markdown is generated from it.

```
python tools/sync_requirements.py    # refresh the data and the column
python tools/build_pdf.py            # print the PDF
```

A new requirement starts as `gap` when a row in section 10 names it, and `unassessed` otherwise. To mark one met, set `"status": "met"` and `"test": "Test_Name"` in the JSON and run the sync again. Do not edit the Status column by hand.
