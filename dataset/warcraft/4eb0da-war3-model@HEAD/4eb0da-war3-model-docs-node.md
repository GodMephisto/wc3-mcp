---
type: vendor-doc
source_repo: "https://github.com/4eb0da/war3-model"
source_path: "docs/node.md"
ref: HEAD
---

## Usage in Node.js or with bundler (e.g. webpack)

`war3-model` can be used either as common-js module or as es-module:

### ES Module

```js
import { parseMDL } from 'war3-model';

console.log(parseMDL('...'));
```

This should be bundled or requires Node.js >= 13

### Common js module

```js
const { parseMDL } = require('war3-model');

console.log(parseMDL('...'));
```