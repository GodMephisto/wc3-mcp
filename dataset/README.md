# dataset

The Warcraft III corpus, downloaded rather than committed. `wc3ctl rag install` fetches it from the release and extracts it here, so the MCP package stays small and the corpus is versioned on its own.

2,381 markdown documents across three groups.

- `warcraft/` GitHub repositories and tools at HEAD.
- `wc3/` the core libraries, StormLib, CascLib, War3Net, MDX, pjass, HiveWE.
- `warcraft-editor/` sites, forums and tutorials.

Release asset `wc3-corpus.tgz`, sha256 `40fb3c94a4f3c85c85f4f10b27f4bf3fb2868a2918ef7dceba1511d4fa2b4e1a`, 30,303,265 bytes. The installer verifies the digest before extracting.

## What changed from the first corpus

The first corpus held 3,521 documents and 982 of them were `.mdx.md`, the binary
Warcraft III model format read as text, so their bodies were MDLX headers and raw
bytes. `mdx` is markdown-JSX on the web and a binary model in a game repository,
and the extractor keyed on the extension rather than the content.

This corpus drops the binary model and asset files, 1,140 fewer documents, and
keeps the 8 `.mdx` that really are prose, WC3 Forge site content. Every body is
also decoded strictly, since a lossy `utf-8, replace` had turned GBK Chinese into
mojibake.

`_v1-bak/` holds the previous set until the new one is published, and is not part
of the corpus.

This folder's contents are not in git. Only this file and `.gitignore` are tracked.

Licensing. Third-party material. Each document carries its source in frontmatter, a `source_url`, a `source_repo` or a `source_page`. Redistribute only where the source license allows, and keep the attribution in place.
