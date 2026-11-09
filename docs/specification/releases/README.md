# Release records

One file per tag, written by `../tools/release_record.py`. A record names the version, the tag, the source commit and a sha256 for every artifact, and `verify` recomputes each digest from the bytes on disk.

## What is here

| Record | What it explains |
|---|---|
| `v0.5.12.json` | The in-tree exe at `dist-mcp/wc3-mcp.exe`, which states version 0.5.12 and bakes commit `93e07ce` in its informational version. The record was written on 10 November 2026 from the artifact and the commit the artifact names. |

No record here was written by a release. `release.yml` does not call `release_record.py`, so every record in this folder was written by hand from a local artifact.

## The v0.5.12 record is not provenance for tag v0.5.12

`git rev-list -n 1 v0.5.12` is `c3d9bf5`, and the exe bakes `93e07ce`, so the artifact was built after the tag that names its version. `tools/check_provenance.py --tag v0.5.12 --assets <a folder holding the exe, its .sha256 and this record as release.json>` refuses it for that reason. `tools/release_record.py verify` passes, because the bytes are the bytes that were recorded.

Both verdicts are correct. A record proves which bytes it describes. It does not prove the tag names the commit those bytes came from, and that second claim is the one PKG-13 asks for.

## The rule this folder follows

A record is written once per tag and never edited. The tool refuses a second write for the same tag unless `--force` is passed, and it refuses a tag that does not name the version. The run log of the fuzz corpus is runtime state and lives outside this folder, in `WC3_SPEC_STATE` or under `LOCALAPPDATA`. A record is release evidence, so it is kept beside the document.
