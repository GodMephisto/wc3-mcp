---
type: vendor-doc
source_repo: "https://github.com/Pinta365/vsce-blp-viewer"
source_path: "package.json"
ref: HEAD
---

{
  "name": "blp-viewer",
  "displayName": "BLP Image Preview",
  "description": "Preview Blizzard .blp texture files in VS Code",
  "publisher": "Pinta365",
  "version": "0.0.5",
  "icon": "icon.png",
  "engines": {
    "vscode": "^1.95.0"
  },
  "license": "MIT",
  "repository": {
    "type": "git",
    "url": "https://github.com/Pinta365/vsce-blp-viewer.git"
  },
  "categories": [
    "Other"
  ],
  "activationEvents": [],
  "main": "./out/extension.js",
  "contributes": {
    "customEditors": [
      {
        "viewType": "blpviewer.preview",
        "displayName": "BLP Preview",
        "selector": [
          {
            "filenamePattern": "*.blp"
          }
        ],
        "priority": "default"
      }
    ]
  },
  "scripts": {
    "vscode:prepublish": "npm run compile",
    "compile": "tsc -p ./",
    "watch": "tsc -watch -p ./",
    "release": "node scripts/release.js"
  },
  "devDependencies": {
    "@types/node": "^25.4.0",
    "@types/vscode": "^1.95.0",
    "@vscode/vsce": "^3.7.1",
    "typescript": "^5.9.3"
  },
  "dependencies": {
    "@pinta365/blp": "npm:@jsr/pinta365__blp@^0.2.0"
  }
}
