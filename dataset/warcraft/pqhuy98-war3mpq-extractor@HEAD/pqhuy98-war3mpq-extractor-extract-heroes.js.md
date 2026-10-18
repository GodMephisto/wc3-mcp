---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "extract-heroes.js"
ref: HEAD
---

import fs from 'fs';

const lines = fs.readFileSync("manual-data/hero-ability.txt")
    .toString().replaceAll("\r", "").split("\n");

const heroes = lines.map((line) => {
    const strs = line.split("\t");
    return {
        name: `HERO_${strs[1]}`,
        code: strs[0],
        unitAbilities: strs[2].split(","),
        heroAbilities: strs[3].split(",")
    };
});

let code = `
export interface Hero {
    code: string;
    unitAbilities: string[];
    heroAbilities: string[];
}
`;
let code2 = "export const ALL_HEROES = [\n";
heroes.forEach((hero) => {
    code += `export const ${hero.name}: Hero = ${JSON.stringify({
        code: hero.code,
        unitAbilities: hero.unitAbilities,
        heroAbilities: hero.heroAbilities,
    }, null, 2)};\n`;

    code2 += `  ${hero.name},\n`;
});

fs.writeFileSync("./output/war3-heroes.ts", code + code2 + "];\n");
console.log(`Extracted ${lines.length} heroes.`);