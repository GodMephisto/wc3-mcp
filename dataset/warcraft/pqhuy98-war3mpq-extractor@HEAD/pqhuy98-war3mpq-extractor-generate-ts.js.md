---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "generate-ts.js"
ref: HEAD
---

import fs from 'fs';
const path = "object-data/unitsdata.json";

const data = Object.values(JSON.parse(fs.readFileSync(path)));

const keys = {};
for (const row of data) {
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
  const types = Array.from(keys[key]).join(" | ");
  def += `  ${key}: ${types}\n`;
});

const code = `
interface UnitData {
${def}
};
`;

fs.writeFileSync("output/war3-object-data-types.ts", code);