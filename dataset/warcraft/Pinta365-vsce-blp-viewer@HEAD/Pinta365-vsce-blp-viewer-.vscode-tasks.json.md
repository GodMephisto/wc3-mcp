---
type: vendor-doc
source_repo: "https://github.com/Pinta365/vsce-blp-viewer"
source_path: ".vscode/tasks.json"
ref: HEAD
---

{
  "version": "2.0.0",
  "tasks": [
    {
      "type": "npm",
      "script": "compile",
      "group": "build",
      "problemMatcher": ["$tsc"],
      "label": "npm: compile",
      "detail": "tsc -p ./"
    },
    {
      "type": "npm",
      "script": "watch",
      "group": "build",
      "problemMatcher": ["$tsc-watch"],
      "label": "npm: watch",
      "detail": "tsc -watch -p ./",
      "isBackground": true
    }
  ]
}
