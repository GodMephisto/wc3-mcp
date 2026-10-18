---
type: vendor-doc
source_repo: "https://github.com/stijnherfst/HiveWE"
source_path: "overlay-ports/casclib/vcpkg.json"
ref: HEAD
---

{
  "name": "casclib",
  "version-date": "2024-06-05",
  "description": "An open-source implementation of library for reading CASC storage from Blizzard games since 2014",
  "homepage": "http://www.zezula.net/en/casc/casclib.html",
  "dependencies": [
    {
      "name": "vcpkg-cmake",
      "host": true
    },
    {
      "name": "vcpkg-cmake-config",
      "host": true
    },
    "zlib"
  ]
}
