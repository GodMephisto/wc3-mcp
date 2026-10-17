---
type: vendor-doc
source_repo: "https://github.com/4eb0da/war3-model"
source_path: "docs/browser-global.md"
ref: HEAD
---

## Usage in browser as external script

```html
<script src="./node_modules/war3-model/dist/war3-model.browser.js"></script>
<script>
    console.log(war3model.parseMDL('...'));
</script>
```

Also there is typings for browser:

```json
// tsconfig.json
{
    "include": [
        "node_modules/war3-model/dist/war3-model.browser.d.ts",
        ...
    ]
}
```