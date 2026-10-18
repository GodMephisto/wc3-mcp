---
type: vendor-doc
source_repo: "https://github.com/invoker-bot/war3map"
source_path: ".eslintrc.json"
ref: HEAD
---

{
    "env": {
      "browser": false,
      "es6": true,
      "node": true
    },
    "parser": "@typescript-eslint/parser",
    "parserOptions": {
      "project": "tsconfig.json",
      "sourceType": "module"
    },
    "plugins": [
      "@typescript-eslint",
      "jest"
    ],
    "extends": [
      "eslint:recommended",
      "plugin:@typescript-eslint/eslint-recommended",
      "plugin:@typescript-eslint/recommended",
      "plugin:jest/recommended",
      "prettier",
      "prettier/@typescript-eslint"
    ],
    "rules": {}
  }