---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "list-extensions.js"
ref: HEAD
---

import fs from 'fs';
import { extract } from "./lib.js";

function listExtensions() {
    const files = extract("**/*");

    const exts = {};
    const ex = {};
    files.forEach(file => {
        exts[file.extension] = ((exts[file.extension] + 1) || 1);
        if (!ex[file.extension]) ex[file.extension] = [];
        if (ex[file.extension].length < 5) {
            ex[file.extension].push(file.path);
        }
    });
    console.log(ex);
    console.log(exts);

    files.filter(f => f.extension === "txt" || f.extension === "slk")
        .forEach(file => console.log("./extracted-mpq/" + file.path));
}
listExtensions();

