---
type: vendor-doc
source_repo: "https://github.com/msierks/mpq-rust"
source_path: ".github/dependabot.yml"
ref: HEAD
---

version: 2
updates:
- package-ecosystem: cargo
  directory: "/"
  schedule:
    interval: daily
    time: "11:00"
  open-pull-requests-limit: 10
