---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "extract-unit-category.js"
ref: HEAD
---

import fs, { writeFileSync } from 'fs';
import { FourCC } from "./lib.js";

import csv from 'csv-parser';

function readUnitCategory(path) {
    return new Promise((resolve) => {
        const units = [];
        fs.createReadStream(path)
            .pipe(csv())
            .on('data', (data) => {
                data["unitId"] = data["﻿unitId"];
                delete data["﻿unitId"];
                units.push(data);
            })
            .on('end', () => {
                resolve(units);
            });
    });
}

const allUnits = await readUnitCategory('manual-data/unitBalance.csv');

const categories = [];
const previousCategory = "";
allUnits.forEach((u, i) => {
    if (categories.length === 0 || u.category !== allUnits[i-1].category) {
        categories.push([]);
    }
    categories.at(-1).push({code: u.unitId, id: FourCC(u.unitId), name: u.name, level: parseInt(u.level), hp: parseInt(u.HP)});
});

console.log(categories.length);
writeFileSync("output/unit-category.json", JSON.stringify(categories, null, 2));