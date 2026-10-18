---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "lib.js"
ref: HEAD
---

import globSync from "glob/sync.js";


export function last(arr) {
    return arr[arr.length - 1];
}

export function print(arr, size) {
    for (let i = 0; i < size; i++) console.log(arr[i]);
}

export function extract(pattern) {
    let filenames = globSync("extracted-mpq/" + pattern);
    filenames = filenames.map((s) => s.replace("extracted-mpq/", ""));

    const files = [];
    filenames.forEach((filename) => {
        const name = last(filename.split("/")).split(".")[0];
        const extension = last(filename.split("."));
        files.push({
            path: filename,
            name, extension: (extension !== filename ? extension : null)
        });
    });

    return files;
}

export function FourCC(str) {
    if (str.length !== 4) {
        throw new Error("FourCC string must be exactly 4 characters long");
    }

    return (str.charCodeAt(0) << 24) |
        (str.charCodeAt(1) << 16) |
        (str.charCodeAt(2) << 8) |
        (str.charCodeAt(3));
}