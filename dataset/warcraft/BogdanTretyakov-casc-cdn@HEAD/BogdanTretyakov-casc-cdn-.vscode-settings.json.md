---
type: vendor-doc
source_repo: "https://github.com/BogdanTretyakov/casc-cdn"
source_path: ".vscode/settings.json"
ref: HEAD
---

{
  "explorer.fileNesting.enabled": true,
  "explorer.fileNesting.patterns": {
    "*.ts": "${capture}.spec.ts, ${capture}.test.ts, ${capture}.d.ts, ${capture}.js"
  },
  "editor.codeActionsOnSave": {
    "source.organizeImports.biome": "explicit"
  },
  "[typescript]": {
    "editor.defaultFormatter": "esbenp.prettier-vscode"
  },
  "[javascript]": {
    "editor.defaultFormatter": "biomejs.biome"
  },
  "[json]": {
    "editor.defaultFormatter": "biomejs.biome"
  },
  "cSpell.words": ["BLTE", "Casc"]
}
