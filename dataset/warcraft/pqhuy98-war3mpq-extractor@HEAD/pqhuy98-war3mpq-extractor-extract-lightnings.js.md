---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "extract-lightnings.js"
ref: HEAD
---

import fs from 'fs';

const allLines = fs.readFileSync("manual-data/lightning.txt")
    .toString().replaceAll("\r", "")
    .split("\n")
    .map(s => s.trim())
    .filter(s => s.length);

const headers = allLines[0].split("\t");
const lines = allLines.slice(1);

const nameSet = new Map();

const lightnings = lines.map((line, i) => {
    const strs = line.split("\t");
    const data = Object.fromEntries(headers.map((h, i) => [h, strs[i]]));
    console.log(data);

    let name = `LIGHTNING_${formatName(data.comment)}`;
    if (nameSet.has(name)) {
        const idx = nameSet.get(name) + 1;
        nameSet.set(name, idx);
        name += "_" + idx;
    } else {
        nameSet.set(name, 1);
    }

    return {
        name,
        code: data.Name,
    };
});

function formatName(str) {
    return str
        .replace(/[^\w]/g, " ")
        .split(" ")
        .filter(s => s.length > 0)
        .map(s => s[0].toUpperCase() + s.slice(1))
        .join("");
}

let code = `
export interface LIGHTNING_TYPE {
    code: string
}
`;
let code2 = "export const ALL_LIGHTNINGS = [\n";

lightnings.forEach((lightning) => {
    const { name, ...rest } = lightning;

    code += `export const ${lightning.name}: LIGHTNING_TYPE = ${JSON.stringify(rest, null, 2)};\n`;

    code2 += `  ${lightning.name},\n`;
});

fs.writeFileSync("./output/war3-lightnings.ts", code + code2 + "];\n");
console.log(`Extracted ${lines.length} lightnings.`);