---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "extract-neutral-units.js"
ref: HEAD
---

import fs, { writeFileSync } from 'fs';
import path from 'path';

// Path to your input file
const filePath = 'manual-data/neutralunitstrings.txt';

// Function to read and parse the file
function parseFile(filePath) {
  const data = fs.readFileSync(filePath, 'utf-8');
  const sections = data.split(/(\r\n){2,}/).filter(s => s!=="\r\n"); // Split by empty lines

  const objects = sections.map(section => {
    const lines = section.split('\r\n').filter(l => !l.startsWith("//"));
    if (lines.length === 0) return;
    const obj = {
      code: lines[0].replace("[", "").replace("]", "").trim()
    };
    if (obj.code.length !== 4) {
      console.error("error", obj, obj.code.length, Number(obj.code[0]));
      throw obj;
    }

    lines.slice(1).forEach(line => {
      let [key, value] = line.split('=');
      if (!key || !value) return;
      key = key.trim().split(":")[0];
      value = value.trim().replace(/^"|"$/g, '');
      if (!key || !value) return;

      if (!obj[key] || obj[key].length < value.length) {
        obj[key] = value;
        return;
      }
    });

    return obj;
  });
  return objects;
}

const parsedObjects = parseFile(filePath);
console.log(parsedObjects.length);
writeFileSync("output/neutral-units.json", JSON.stringify(parsedObjects, null, 2));