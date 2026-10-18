---
type: vendor-doc
source_repo: "https://github.com/devovh/MPQ_Extractor"
source_path: "extractor/convert.sh"
ref: HEAD
---

#!/bin/bash
find -name "*.wav" | xargs -I % sh -c 'ffmpeg -hide_banner -y -i "%" -acodec libvorbis -f ogg "%_"; rm "%";' && find -name "*.mp3" | xargs -I % sh -c 'ffmpeg -hide_banner -y -i "%" -acodec libmp3lame -f mp3 "%_"; rm "%";'