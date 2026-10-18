---
type: vendor-doc
source_repo: "https://github.com/ChiefOfGxBxL/WC3MapTranslator"
source_path: ".codeclimate.yml"
ref: HEAD
---

plugins:
  tslint:
    enabled: true
    config: tslint.json
  fixme:
    enabled: true
ratings:
  paths:
  - "**.ts"
  - "**.tsx"
exclude_paths:
  - test/
