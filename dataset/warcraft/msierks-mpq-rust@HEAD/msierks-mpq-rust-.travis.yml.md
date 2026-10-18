---
type: vendor-doc
source_repo: "https://github.com/msierks/mpq-rust"
source_path: ".travis.yml"
ref: HEAD
---

language: rust
rust:
  - stable
  - beta
  - nightly
matrix:
  allow_failures:
    - rust: nightly
notifications:
  email:
    - msierks117@gmail.com
sudo: false
cache: cargo
before_script:
  - rustc --version
  - cargo --version
script:
  - cargo test
branches:
  only:
    - master
