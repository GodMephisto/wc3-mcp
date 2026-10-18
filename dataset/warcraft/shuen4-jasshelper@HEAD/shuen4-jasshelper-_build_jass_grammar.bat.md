---
type: vendor-doc
source_repo: "https://github.com/shuen4/jasshelper"
source_path: "_build_jass_grammar.bat"
ref: HEAD
---

@echo off
cd GOLD-Builder-5.2.0-Cmd
goldbuild ..\jasshelper.grm ..\jasshelper.cgt
goldprog ..\jasshelper.cgt _jasssymbol.pgt ..\jasshelpersymbols.pas
del jasshelper.log