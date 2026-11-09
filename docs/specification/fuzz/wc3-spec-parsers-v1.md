# Fuzz corpus wc3-spec-parsers-v1

This names the corpus SEC-08 asks for, and states what it does not cover. The cases live in `wc3-spec-parsers-v1.json`. The runner is `../tools/fuzz_corpus.py`.

## What the corpus is

Thirty six malformed or boundary inputs in two groups. Twenty three run against the readers reachable from this folder, one per reader call. Thirteen start the published exe, speak MCP over stdio, and call one tool with one malformed map, so a case reaches the map, object, trigger and script parsers the product ships.

| Reader | What it reads | Cases |
|---|---|---|
| `json_file` | any JSON document, through `check_versions.load_json` | empty, truncated, not utf-8, a nul byte, 5000 deep |
| `record_file` | a release record, through `release_record.load` | a top level list |
| `provenance_record` | the provenance a record names, through `check_provenance.record_provenance` | a version and no commit, a commit and no digest |
| `requirements_file` | `requirements.json`, through `check_counts.load_requirements` | a bare list, a gaps field that is not a list, a row that is not an object |
| `digest_sidecar` | a `.sha256` file, through `check_provenance.read_digest` | 63 hex characters, an empty file, an uppercase digest |
| `pe_version` | the version resource of an exe, through `check_versions.read_exe_version` | an empty file, an MZ stub, 4 KB of noise, a text file named as an exe |
| `markdown_tables` | the requirement tables, through `sync_requirements.parse` | a row 20000 cells wide, markdown that is not utf-8 |
| `prose_counts` | the counts sentence, through `check_counts.prose_counts` | a sentence with no counts in it |
| `version_scan` | every `version` string in a manifest, through `check_versions.json_versions` | a version held as a number, a manifest cut off mid key |
| `exe_tool` | one tool of the published exe, with a map folder or a map file built from the case | map 5, object 3, trigger 2, script 3 |

Each case runs in its own interpreter. A case that raises a named exception is a tool error. A case whose interpreter ends without its result line is a crash and fails the run, whatever the exit code. A tool call answered with `isError` is a tool error. A server that exits, or stops answering, before the call returns is a crash, because the child then ends without its result line and its reason goes to stderr. The run record is appended to `fuzz-runs.jsonl` in the state folder, outside this corpus, one line per run.

## What each case through the exe returned

Measured on 10 November 2026 against `dist-mcp/Wc3.Mcp.exe`, tool surface 276, server name `wc3ctl` 0.4.13.

| Case | Family | Tool | Returned |
|---|---|---|---|
| `map-w3i-unknown-version` | map | `map_info` | tool error, war3map.w3i could not be parsed, Value 2147483647 is not defined for enum of type MapInfoFormatVersion |
| `map-w3i-count-past-end` | map | `map_info` | tool error, war3map.w3i could not be parsed, Unable to read beyond the end of the stream |
| `map-empty-archive` | map | `map_overview` | tool error, No MPQ archive magic found in file |
| `map-truncated-archive` | map | `map_overview` | tool error, No MPQ archive magic found in file |
| `map-wts-truncated-table` | map | `map_overview` | value, diagnostic names war3map.wts, the table is truncated, and the reader returned |
| `object-w3u-unknown-version` | object | `map_overview` | value, diagnostic names war3map.w3u, Value 99 is not defined for enum of type ObjectDataFormatVersion, parsed false |
| `object-w3u-truncated` | object | `map_overview` | value, diagnostic names war3map.w3u, reached end of the stream inside a string, parsed false |
| `object-list-w3u-truncated` | object | `object_list` | value, an empty items list, no error and no diagnostic |
| `triggers-read-wtg-truncated` | trigger | `triggers_read` | value, empty categories, triggers and variables, no error |
| `trigger-wtg-overview-truncated` | trigger | `map_overview` | value, diagnostic names war3map.wtg, the trigger reader refused the tree, parsed false |
| `script-check-binary-jass` | script | `script_check` | value, ok true with no issues on a file holding every byte value |
| `script-format-unterminated-lua` | script | `script_format` | tool error, line 2, unfinished string near the opening quote |
| `script-check-unterminated-jass` | script | `script_check` | value, ok false, issues name a missing config function and a missing main function |

The run is 36 cases, 23 tool errors, 13 values and 0 crashes. Every case behaved as the corpus says, and no interpreter and no server ended without its result line.

## What the run found

Three readings of the SEC-08 sentence do not hold.

- A malformed object table returns a value, not a tool error. `map_overview` names the entry in a diagnostic and marks it unparsed, which is a fair report, while `object_list` answers an empty list with no error at all. A caller that only reads the item list cannot tell a map with no custom objects from a broken table.
- A malformed trigger tree returns a value, not a tool error. `triggers_read` answers empty categories, triggers and variables. `map_overview` does name the entry.
- A script file holding every byte value passes `script_check` with `ok` true. The checker reads a script as Latin-1 text and reports a missing entry point as an error, so a binary file should not pass. The unterminated JASS case does report issues, so the checker runs, and this acceptance is about what it accepts.

The truncated string table is the input that once hung the reader. The guard in `DefaultParsers.ValidateWts` now refuses it with a named error, and the tool returns with a diagnostic, so that regression still holds.

## The residual risk

The corpus now reaches the four product parser families through the exe, but it does not cover all of them, and four things stay outside it.

- The archive readers, MPQ and CASC, reached through the native CascLib. The two archive cases cover a missing header. A crafted header or a protected archive is not covered, and a fault there ends the process in native code.
- The map readers other than `war3map.w3i`, `war3map.w3u`, `war3map.wtg`, `war3map.wts` and `war3map.j`, which are `war3map.w3e`, `war3map.wpm`, `war3map.doo`, `war3mapUnits.doo`, `war3map.w3r`, `war3map.w3c`, `war3map.w3s`, `war3map.wct`, `war3map.imp`, `war3map.w3a`, `war3map.w3t`, `war3map.w3b`, `war3map.w3d`, `war3map.w3h`, `war3map.w3q` and the `war3mapSkin.*` twins.
- Object data with a legal version and absurd counts, which is the shape that would make a reader allocate before it reads.
- The `.mcpb` bundle reader and the npm launcher.

Running the corpus needs the published exe, and this folder runs no dotnet. The run above used the exe already in `dist-mcp`, and the corpus is not yet a step in the hermetic suite, so G-59 stays open.

## How to run it

```
python tools/fuzz_corpus.py
python tools/fuzz_corpus.py --json
python tools/fuzz_corpus.py --no-record
python tools/fuzz_corpus.py --exe C:\path\to\Wc3.Mcp.exe
```

The state folder is `WC3_SPEC_STATE` when it is set, else `<LOCALAPPDATA>/wc3-mcp/spec-state`. The lock beside the log carries an owner, a token and an expiry, so a run that dies leaves a lock a later run can take.
