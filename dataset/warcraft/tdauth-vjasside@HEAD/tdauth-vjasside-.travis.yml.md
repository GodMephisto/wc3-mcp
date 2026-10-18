---
type: vendor-doc
source_repo: "https://github.com/tdauth/vjasside"
source_path: ".travis.yml"
ref: HEAD
---

language: cpp
compiler:
  - gcc
  - clang
addons:
  apt:
    packages:
      - qmake
      - qtbase5-dev

script:
     - mkdir -p build
     - cd build
     - qmake ../
     - make
