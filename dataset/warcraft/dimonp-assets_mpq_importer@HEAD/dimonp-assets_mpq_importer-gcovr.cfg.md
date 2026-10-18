---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "gcovr.cfg"
ref: HEAD
---

root = .
search-path = out

filter = src/*
filter = include/*

exclude-directories = install
exclude-directories = out/*/*/_deps
exclude-directories = test

gcov-ignore-parse-errors = all
print-summary = yes

html-details = ./out/coverage/index.html

cobertura-pretty = yes
cobertura = out/cobertura.xml

#TBD delete-gcov-files = yes
