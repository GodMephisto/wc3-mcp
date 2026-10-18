---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JASS-Tree-sitter-Rust"
source_path: ".eslintrc.json"
ref: HEAD
---

{
  "env": {
    "node": true,
    "browser": false,
    "es2021": true
  },
  "extends": [
    "eslint:recommended"
  ],
  "parserOptions": {
    "ecmaVersion": "latest",
    "sourceType": "module"
  },
  "rules": {
    "curly": "off",
    "semi": [
      "error",
      "never"
    ],
    "quotes": [
      "error",
      "single"
    ],
    "no-unsafe-optional-chaining": "off",
    "no-console": "off",
    "no-eq-null": "warn",
    "no-case-declarations": "off",
    "no-constant-condition": "off"
  }
}
