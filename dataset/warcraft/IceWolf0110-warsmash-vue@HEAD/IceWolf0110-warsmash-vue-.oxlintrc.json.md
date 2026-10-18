---
type: vendor-doc
source_repo: "https://github.com/IceWolf0110/warsmash-vue"
source_path: ".oxlintrc.json"
ref: HEAD
---

{
  "$schema": "./node_modules/oxlint/configuration_schema.json",
  "plugins": ["eslint", "typescript", "unicorn", "oxc", "vue"],
  "env": {
    "browser": true
  },
  "categories": {
    "correctness": "error"
  }
}
