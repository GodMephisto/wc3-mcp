---
type: vendor-doc
source_repo: "https://github.com/mtgrosser/jass-react-jsx"
source_path: "examples/reakt/config/importmap.rb"
ref: HEAD
---

pin "application", preload: true

pin "react", to: "https://cdn.esm.sh/v45/react@17.0.2/es2021/react.js"
pin "react-dom", to: "https://cdn.esm.sh/v45/react-dom@17.0.2/es2021/react-dom.js"
pin "react/jsx-runtime", to: "https://cdn.esm.sh/v45/react@17.0.2/es2021/jsx-runtime.js"

pin_all_from "app/javascript/components", under: "components"
