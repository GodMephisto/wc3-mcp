---
type: vendor-doc
source_repo: "https://github.com/rufreakde/Wc3MapWorker"
source_path: ".vscode/tasks.json"
ref: HEAD
---

{
    "version": "2.0.0",
    "tasks": [
        {
            "label": "build_ts",
            "command": "tsc", // Could be any other shell command
            "args": [
                "--build"
            ],
            "type": "shell"
        }
    ]
}