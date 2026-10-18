# dataset

The Warcraft III corpus, downloaded rather than committed. `wc3ctl rag install` fetches it from the release and extracts it here, so the MCP package stays small and the corpus is versioned on its own.

12,571 documents across three groups, code and configuration included, not only prose.

- `warcraft/` GitHub repositories and tools at HEAD, 11,255 documents.
- `wc3/` the core libraries, StormLib, CascLib, War3Net, MDX, pjass, HiveWE, 607 documents.
- `warcraft-editor/` sites, forums and tutorials, 709 documents.

Release asset `wc3-corpus.tgz`, sha256 `e0b05042af853e46d2d72c9a95d3c4ef7d938c5145d56a248a6ee18903ce7716`, 51,629,461 bytes. The installer verifies the digest before extracting.

## The history of this corpus

The first held 3,521 documents and 982 of them were `.mdx.md`, the binary Warcraft III model format read as text, so their bodies were MDLX headers and raw bytes.

The second dropped the binary, 2,381 documents, and every body decoded strictly.

This one adds the code and configuration, `.cs`, `.java`, `.h`, `.ts`, `.py`, `.json` and the rest. A docs-only corpus described a repository by its README, and 290 of about 400 packages had only a README. A `W3x.Sort` that was one file is now fourteen. The acquirer keeps text files and still drops the real assets, so the model bytes never return.

This folder's contents are not in git. Only this file and `.gitignore` are tracked.

Licensing. Third-party material. Each document carries its source in frontmatter, a `source_url`, a `source_repo` or a `source_page`. Redistribute only where the source license allows, and keep the attribution in place.
