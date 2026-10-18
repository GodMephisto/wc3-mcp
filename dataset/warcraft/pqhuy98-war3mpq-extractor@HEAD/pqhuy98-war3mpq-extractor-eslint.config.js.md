---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "eslint.config.js"
ref: HEAD
---

export default {
  "rules": {
    "linebreak-style": 0,
    "no-continue": "off",
    "semi": 2,
    "prefer-const": 2,
    "import/prefer-default-export": "off",
    "no-new": "off",
    "no-plusplus": "off",
    "max-len": [
      "error",
      {
        "code": 200
      }
    ],
    "no-restricted-globals": "off",
    "no-unused-vars": "off",
    "camelcase": "off",
    "no-param-reassign": [
      "error",
      {
        "props": false
      }
    ],
    "no-mixed-operators": "off",
    "no-use-before-define": "off",
  },
  "settings": {
    "import/resolver": {
      "node": {
        "extensions": [
          ".js",
          ".jsx",
          ".ts",
          ".tsx"
        ]
      }
    }
  }
};