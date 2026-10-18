---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "extract-abilities.js"
ref: HEAD
---

import fs from 'fs';
import { FourCC } from "./lib.js";

const allLines = fs.readFileSync("manual-data/ability-id.txt")
    .toString().replaceAll("\r", "")
    .split("\n")
    .map(s => s.trim())
    .filter(s => s.length);

const headers = allLines[0].split("\t");
const lines = allLines.slice(1);

const nameSet = new Map();

const abilities = lines.map((line, i) => {
    const strs = line.split("\t");
    const data = Object.fromEntries(headers.map((h, i) => [h, strs[i]]));

    let name = `ABILITY_${formatName(data.comments)}`;
    if (nameSet.has(name)) {
        const idx = nameSet.get(name) + 1;
        nameSet.set(name, idx);
        name += "_" + idx;
    } else {
        nameSet.set(name, 1);
    }

    return {
        name,
        code: data.alias,
        id: FourCC(data.alias),
        sort: data.sort,
        race: data.race,
        levels: parseInt(data.levels, 10),
        requiredLevel: parseInt(data.reqLevel, 10),
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
export interface ABILITY_TYPE {
    code: string
    id: number
    sort: "hero" | "unit" | "item"
    race: "human" | "orc" | "undead" | "nightelf" | "naga" | "creeps" | "other"
    levels: number
    requiredLevel: number
}
`;
let code2 = "export const ALL_ABILITIES = [\n";

abilities.forEach((ability) => {
    const { name, ...rest } = ability;

    code += `export const ${ability.name}: ABILITY_TYPE = ${JSON.stringify(rest, null, 2)};\n`;

    code2 += `  ${ability.name},\n`;
});

fs.writeFileSync("./output/war3-abilities.ts", code + code2 + "];\n");
console.log(`Extracted ${lines.length} abilities.`);