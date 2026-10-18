---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "generate-items.js"
ref: HEAD
---

import fs from 'fs';
import { FourCC } from "./lib.js";
const path = "object-data/itemsdata.json";

const data = Object.values(JSON.parse(fs.readFileSync(path)));

const chosenKeys = [
  "id",
  "code",
  "name",
  "level",
  "classification",
  "description",
];

const keys = {};
for (const row of data) {
  row.code = row.oldId;
  row.id = FourCC(row.code);
  delete row.oldId;
  Object.keys(row).forEach(k => {
    const type = typeof row[k];
    if (!(k in keys)) {
      keys[k] = new Set();
    }
    keys[k].add(type);
  });
}

let def = "";
Object.keys(keys).forEach(key => {
  if (chosenKeys.includes(key)) {
    const types = Array.from(keys[key]).join(" | ");
    def += `  ${key}: ${types}\n`;
  }
});

function pickKeysFromObject(obj, keys) {
  // Initialize a new empty object
  const result = {};

  // Iterate over the array of keys
  for (const key of keys) {
      // Check if the key exists in the original object
      if (key in obj) {
          // Add the key and its value to the result object
          result[key] = obj[key];
      }
  }

  // Return the new object with the chosen keys
  return result;
}

const code = `
export interface ItemData {
  ${def.trim()}
};
${data.map(d => `export const ITEM_${d.name.replace(/[^a-zA-Z0-9]/g, '')}_${d.code}: ItemData = ${JSON.stringify(pickKeysFromObject(d, chosenKeys))}`).join("\n")}
`;

fs.writeFileSync("output/item-data.ts", code);