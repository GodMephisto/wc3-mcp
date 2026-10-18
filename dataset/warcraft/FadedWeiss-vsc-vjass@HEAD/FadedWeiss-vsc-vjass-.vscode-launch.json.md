---
type: vendor-doc
source_repo: "https://github.com/FadedWeiss/vsc-vjass"
source_path: ".vscode/launch.json"
ref: HEAD
---

// A launch configuration that launches the extension inside a new window
{
    "version": "0.1.0",
    "configurations": [
        {
            "name": "Extension",
            "type": "extensionHost",
            "request": "launch",
            "runtimeExecutable": "${execPath}",
            "args": ["--extensionDevelopmentPath=${workspaceRoot}" ],
            "stopOnEntry": false
        },
        {
            "name": "Extension Tests",
            "type": "extensionHost",
            "request": "launch",
            "runtimeExecutable": "${execPath}",
            "args": ["--extensionDevelopmentPath=${workspaceRoot}", "--extensionTestsPath=${workspaceRoot}/test" ],
            "stopOnEntry": false
        }
    ]
}