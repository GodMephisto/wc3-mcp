---
type: vendor-doc
source_repo: "https://github.com/shuen4/jasshelper"
source_path: "_build_zinc_grammar.bat"
ref: HEAD
---

@echo off
cd GOLD-Builder-5.2.0-Cmd
goldbuild ..\zinc.grm ..\zinc.cgt
goldprog ..\zinc.cgt _zinc.pgt ..\ZincSymbols.pas
del zinc.log