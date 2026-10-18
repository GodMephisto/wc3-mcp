---
type: vendor-doc
source_repo: "https://github.com/Kingwl/w3x-google-translate"
source_path: ".vscode/launch.json"
ref: HEAD
---

{
    // Use IntelliSense to learn about possible attributes.
    // Hover to view descriptions of existing attributes.
    // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Launch Program",
            "program": "${workspaceFolder}/dist/index.js",
            "request": "launch",
            "cwd": "${workspaceFolder}",
            "skipFiles": [
                "<node_internals>/**"
            ],
            "type": "node"
        }
    ]
}