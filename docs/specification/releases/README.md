# Release records

One file per tag, written by `../tools/release_record.py`. A record names the version, the tag, the source commit and a sha256 for every artifact. `verify` recomputes each digest from the bytes on disk, and for a record whose artifacts are release URLs it reads the digest published beside each asset instead. `resolve --version` answers which record explains a published version and, with `--repo`, whether the record's commit is the one the tag points at.

## What is here

| Record | What it explains |
|---|---|
| `v0.5.12.json` | The in-tree exe at `dist-mcp/wc3-mcp.exe`, which states version 0.5.12 and bakes commit `93e07ce` in its informational version. The record was written on 10 November 2026 from the artifact and the commit the artifact names. |
| `v0.5.21.json` | The release at tag v0.5.21, which serves `wc3-mcp-v0.5.21-win-x64.zip` and `wc3-mcp-v0.5.21-win-x64.mcpb`. The record names commit `42a9e23`, the commit the tag points at, and the byte count and sha256 of each asset, and both digests agree with the `.sha256` published beside the asset. |

## No record here was written by the release workflow

`release.yml` does not call `release_record.py`, and the v0.5.21 record carries no `written` stamp, which `write` always records, so the record was written beside the release rather than by it. `resolve --version 0.5.22` exits 2 for that reason, because the version npm and the release serve now has no record in this folder. A record is still one per tag, and `write` refuses a second one unless `--force` is passed.

## The v0.5.12 record is not provenance for tag v0.5.12

`git rev-list -n 1 v0.5.12` is `c3d9bf5`, and the exe bakes `93e07ce`, so the artifact was built after the tag that names its version. `tools/check_provenance.py --tag v0.5.12 --assets <a folder holding the exe, its .sha256 and this record as release.json>` refuses it for that reason. `tools/release_record.py verify` passes, because the bytes are the bytes that were recorded. `verify --repo` names the disagreement as one more claim, since the record's commit is not the one the tag points at.

Both verdicts are correct. A record proves which bytes it describes. It does not prove the tag names the commit those bytes came from, and that second claim is the one PKG-13 asks for.

## The rule this folder follows

A record is written once per tag and never edited. The tool refuses a second write for the same tag unless `--force` is passed, and it refuses a tag that does not name the version. The run log of the fuzz corpus is runtime state and lives outside this folder, in `WC3_SPEC_STATE` or under `LOCALAPPDATA`. A record is release evidence, so it is kept beside the document.
