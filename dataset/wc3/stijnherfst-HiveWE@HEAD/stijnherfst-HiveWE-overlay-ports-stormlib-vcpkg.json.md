---
type: vendor-doc
source_repo: "https://github.com/stijnherfst/HiveWE"
source_path: "overlay-ports/stormlib/vcpkg.json"
ref: HEAD
---

{
  "name": "stormlib",
  "version": "9.30",
  "description": "StormLib is a library for opening and manipulating Blizzard MPQ files",
  "dependencies": [
    "bzip2",
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
