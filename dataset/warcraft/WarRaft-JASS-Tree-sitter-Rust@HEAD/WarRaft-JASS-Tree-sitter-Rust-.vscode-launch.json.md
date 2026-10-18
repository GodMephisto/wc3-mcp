---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: ".vscode/launch.json"
ref: HEAD
---

{
  "version": "0.0.1",
  "configurations": [
    {
      "name": "Launch Client",
      "type": "extensionHost",
      "request": "launch",
      "runtimeExecutable": "${execPath}",
      "args": [
        "--extensionDevelopmentPath=${workspaceRoot}"
      ],
      "outFiles": [
        "${workspaceRoot}/extension/**/*.js"
      ],
      "autoAttachChildProcesses": true

    }
  ]
}