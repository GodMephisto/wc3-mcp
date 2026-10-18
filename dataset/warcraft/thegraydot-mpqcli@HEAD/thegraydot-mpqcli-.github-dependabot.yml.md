---
type: vendor-doc
source_repo: "https://github.com/thegraydot/mpqcli"
source_path: ".github/dependabot.yml"
ref: HEAD
---

version: 2

updates:

  - package-ecosystem: github-actions
    directory: /
    schedule:
      interval: weekly
    assignees:
      - "thomaslaurenson"

  - package-ecosystem: pip
    directory: /test
    schedule:
      interval: weekly
    open-pull-requests-limit: 0
    assignees:
      - "thomaslaurenson"

  - package-ecosystem: docker
    directory: /
    schedule:
      interval: weekly
    open-pull-requests-limit: 0
    assignees:
      - "thomaslaurenson"
