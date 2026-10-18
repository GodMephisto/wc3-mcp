---
type: vendor-doc
source_repo: "https://github.com/aesteve/w3rs"
source_path: "w3info/replays-w3info/download-replay.sh"
ref: HEAD
---

#!/bin/bash
echo "Downloading replay #$1"
curl -XGET https://warcraft3.info/api/v1/replays/$1 -H 'Accept:application/json' >> ./analysed.jsonl
echo "\n" >> ./analysed.jsonl
curl -XGET https://warcraft3.info/api/v1/replays/$1/parse -H 'Accept:application/json' >> ./analysed.jsonl
echo "\n" >> ./analysed.jsonl
curl -XGET https://warcraft3.info/api/v1/replays/72665/download > ./analysed/$1.w3g
