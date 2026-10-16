# context

The Warcraft III documents an AI should be able to consult in full. Not summaries, the whole text, so a model can point at the exact rule, field or warning.

- `ui-frames/ui-frames.md` the Big UI-Frame Tutorial by Tasyen, full text, each diagram linked to its source, and the original page under `raw/`.
- `abilities/ability-insight.md` the Warcraft 3 Editor Ability Insight reference, full text, and the original export under `raw/`.
- `BUILDING_A_RAG.md` how to serve these, or build a local RAG over a larger corpus in one install.

Served by the MCP server through resources and a search tool, so a client reads a whole document or finds the passage it needs. No images are bundled. The originals link them at the source, which keeps the package small and keeps the credit and license with the host.

Attribution. Both are community work. The UI-Frame tutorial is Tasyen's, on Hive Workshop. The Ability Insight doc names its creator and maintainer in its own header. Each folder keeps its source. Check the maintainers' terms before republishing, and keep the headers and the `raw` copies in place.
