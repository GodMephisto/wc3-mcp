# dataset

The Warcraft III corpus, downloaded rather than committed. `wc3ctl rag install` fetches it from the release and extracts it here, so the MCP package stays small and the corpus is versioned on its own.

3,522 markdown documents across three groups.

- `warcraft/` GitHub repositories and tools at HEAD.
- `wc3/` the core libraries, StormLib, CascLib, War3Net, MDX, pjass, HiveWE.
- `warcraft-editor/` sites, forums and tutorials.

Release asset `wc3-corpus.tgz`, sha256 `32bf2562c3133f704a00208c60596e0450591c4be530d2db699f938a3ed5da16`, 61,088,850 bytes. The installer verifies the digest before extracting.

This folder's contents are not in git. Only this file and `.gitignore` are tracked.

Licensing. Third-party material. Each document carries its source in frontmatter, a `source_url`, a `source_repo` or a `source_page`. Redistribute only where the source license allows, and keep the attribution in place.
