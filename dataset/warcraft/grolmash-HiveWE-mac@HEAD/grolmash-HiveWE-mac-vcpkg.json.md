---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "vcpkg.json"
ref: HEAD
---

{
  "$schema": "https://raw.githubusercontent.com/microsoft/vcpkg/master/scripts/vcpkg.schema.json",
  "vcpkg-configuration": {
    "overlay-ports": [
      "overlay-ports"
    ]
  },
  "name": "hivewe",
  "version": "0.11.0",
  "builtin-baseline": "d015e31e90838a4c9dfa3eed45979bc70d9357fc",
  "dependencies": [
    "abseil",
    "bullet3",
    "bzip2",
    "casclib",
    {
      "name": "glad",
      "default-features": false,
      "features": [
        "extensions",
        "gl-api-46",
        "loader"
      ]
    },
    "glm",
    "imgui",
    "libjpeg-turbo",
    {
      "name": "mimalloc",
      "features": [
        "override"
      ]
    },
    "nlohmann-json",
    "outcome",
    "qt-advanced-docking-system",
    {
      "name": "qtbase",
      "default-features": false,
      "features": [
        "gui",
        "jpeg",
        "opengl",
        "png",
        "widgets"
      ]
    },
    "soil2",
    "stormlib",
    "tbb",
    "unordered-dense",
    "qscintilla",
    "tracy",
    "doctest"
  ]
}
