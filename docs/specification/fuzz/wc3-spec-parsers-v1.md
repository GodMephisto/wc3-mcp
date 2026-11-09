# Fuzz corpus wc3-spec-parsers-v1

This names the corpus SEC-08 asks for, and states what it does not cover. The cases live in `wc3-spec-parsers-v1.json`. The runner is `../tools/fuzz_corpus.py`.

## What the corpus is

Twenty three malformed or boundary inputs, one per reader call, against the readers reachable from this folder.

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

Each case runs in its own interpreter. A case that raises a named exception is a tool error. A case whose interpreter ends without its result line is a crash and fails the run, whatever the exit code. The run record is appended to `fuzz-runs.jsonl` in the state folder, outside this corpus, one line per run.

## The residual risk

The corpus exercises the readers of the specification's own tools. It does not exercise the parsers the product ships, and those are the parsers SEC-08 is about. Four things are outside this corpus.

- The archive readers, MPQ and CASC, reached through the native CascLib. A fault there ends the process and no Python case can see it.
- The map readers for `war3map.w3i`, `war3map.w3e`, `war3map.w3u`, `war3map.w3t`, `war3map.w3a`, `war3map.w3b`, `war3map.w3d`, `war3map.w3q` and `war3map.doo`.
- The trigger reader for `war3map.j` and the script compiler path.
- The `.mcpb` bundle reader and the npm launcher.

Running those needs the built server, which needs `dotnet publish`. This folder runs no dotnet. Until a person runs the corpus against the built exe through the hermetic suite, the statement in SEC-08 is proven for nine readers of the tools and not for the product's parsers, and the gap row G-59 stays open.

## How to run it

```
python tools/fuzz_corpus.py
python tools/fuzz_corpus.py --json
python tools/fuzz_corpus.py --no-record
python tools/fuzz_corpus.py --selftest
```

The state folder is `WC3_SPEC_STATE` when it is set, else `<LOCALAPPDATA>/wc3-mcp/spec-state`. The lock beside the log carries an owner, a token and an expiry, so a run that dies leaves a lock a later run can take.
