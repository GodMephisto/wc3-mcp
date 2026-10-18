---
type: vendor-doc
source_repo: "https://github.com/lichenhao/war3map_parser"
source_path: "src/main.ts"
ref: HEAD
---

import Map from './parsers/w3x/index'

document.getElementById('map')!.addEventListener('change', async (event) => {
  const input = event.target as HTMLInputElement;

  if (input.files && input.files.length > 0) {
    const file = input.files[0];
    const arrayBuffer = await file.arrayBuffer();
    const map = new Map(new Uint8Array(arrayBuffer));
    console.log(map);
  }
});
