---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/README.md"
ref: HEAD
---

# wc3-forge website

The marketing + documentation site for [wc3-forge](https://github.com/StephenSHorton/wc3-forge),
deployed to GitHub Pages.

- **Framework:** [Next.js](https://nextjs.org) (static export) + [Fumadocs](https://fumadocs.dev)
- **Theme:** [warcraftcn-ui](https://github.com/TheOrcDev/warcraftcn-ui) (MIT) — Warcraft III-styled components, cursors, and the Cinzel display font
- **Content:** MDX under `content/docs/`

## Develop

```bash
cd site
npm install
npm run dev      # http://localhost:3000
```

## Build (static export)

```bash
npm run build    # emits ./out
```

For a GitHub Pages **project** site (served under `/wc3-forge`), set the base path:

```bash
NEXT_PUBLIC_BASE_PATH=/wc3-forge npm run build
```

Leave `NEXT_PUBLIC_BASE_PATH` unset for local dev or a custom domain at the root.

## Deploy

Pushes to `main` that touch `site/**` trigger
[`.github/workflows/site.yml`](../.github/workflows/site.yml), which builds the
static export and publishes it to GitHub Pages.

## Editing docs

Each page is an `.mdx` file in `content/docs/`. The sidebar order lives in
`content/docs/meta.json`. Fumadocs components (`<Callout>`, `<Tabs>`, `<Cards>`,
`<Accordions>`) and warcraftcn components (exposed as `<Wc*>` in
[`mdx-components.tsx`](mdx-components.tsx)) are available in any page.

## Credits

UI theme derived from [warcraftcn-ui](https://github.com/TheOrcDev/warcraftcn-ui)
by TheOrcDev, used under the MIT License (see
`components/ui/warcraftcn/LICENSE`). Warcraft III is a trademark of Blizzard
Entertainment; this is a fan project with no affiliation.
