---
type: vendor-doc
source_repo: "https://github.com/doggan/mpq-extract"
source_path: "package.json"
ref: HEAD
---

{
  "name": "mpq-extract",
  "version": "1.0.1",
  "description": "A tool for extracting files from MPQ archives.",
  "bin": "lib/cli.js",
  "repository": {
    "type": "git",
    "url": "git+https://github.com/doggan/mpq-extract.git"
  },
  "keywords": [
    "mpq",
    "archive",
    "extract"
  ],
  "author": "Shyam Guthikonda",
  "license": "MIT",
  "bugs": {
    "url": "https://github.com/doggan/mpq-extract/issues"
  },
  "homepage": "https://github.com/doggan/mpq-extract#readme",
  "dependencies": {
    "async": "^1.5.2",
    "cli-spinner": "^0.2.2",
    "commander": "^2.9.0",
    "lodash.map": "^4.0.1",
    "mech-mpq": "^1.1.0",
    "mkdirp": "^0.5.1"
  }
}
