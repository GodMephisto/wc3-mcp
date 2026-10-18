---
type: vendor-doc
source_repo: "https://github.com/w4454962/jass-parser-luajit"
source_path: ".vscode/tasks.json"
ref: HEAD
---

{
    // See https://go.microsoft.com/fwlink/?LinkId=733558
    // for the documentation about the tasks.json format
    "version": "2.0.0",
    "tasks": [
        {
            "label": "运行",
            "type": "shell",
            "command": "${workspaceRoot}\\run.bat",
            "args": [
                "${workspaceRoot}"
            ],
            "group": {
                "kind": "test",
                "isDefault": true
            }
        },
    ],

}