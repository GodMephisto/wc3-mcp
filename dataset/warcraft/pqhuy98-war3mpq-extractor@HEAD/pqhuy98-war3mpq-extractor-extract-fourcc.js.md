---
type: vendor-doc
source_repo: "https://github.com/pqhuy98/war3mpq-extractor"
source_path: "extract-fourcc.js"
ref: HEAD
---

import fs from 'fs';

// List of .slk files we want to scan for FourCC codes
const SOURCE_FILES = [
  "extracted-mpq/war3.w3mod/doodads/doodads.slk",
  "extracted-mpq/war3.w3mod/units/unitdata.slk",
  "extracted-mpq/war3.w3mod/units/upgradedata.slk",
  "extracted-mpq/war3.w3mod/units/abilitybuffdata.slk",
  "extracted-mpq/war3.w3mod/units/abilitydata.slk",
  "extracted-mpq/war3.w3mod/units/itemdata.slk",
  "extracted-mpq/war3.w3mod/units/destructabledata.slk",
];

// Regular expression that captures a quoted 4-character code e.g. K"ABCD"
const FOURCC_REGEX = /K"([A-Za-z0-9]{4})"/g;

const fourccSet = new Set();

SOURCE_FILES.forEach((path) => {
  const data = fs.readFileSync(path, "utf8");

  let match;
  while ((match = FOURCC_REGEX.exec(data)) !== null) {
    fourccSet.add(match[1]);
  }
});

// Sort the codes for reproducible output
const sortedCodes = Array.from(fourccSet).sort();

// Ensure output directory exists
fs.mkdirSync("output", { recursive: true });

// Generate the TypeScript file contents
let tsCode = "export const FOURCC_SET: ReadonlySet<string> = new Set<string>([\n";
for (const code of sortedCodes) {
  tsCode += `  \"${code}\",\n`;
}
tsCode += "]);\n\nexport default FOURCC_SET;\n";

// Write to output/war3-fourcc.ts
fs.writeFileSync("output/war3-fourcc.ts", tsCode);

console.log(`Extracted ${sortedCodes.length} distinct FourCC codes → output/war3-fourcc.ts`);
