---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/Antlr/Config/Build_CS.cmd"
ref: HEAD
---

antlr-4.8-complete.jar -Dlanguage=CSharp JassLexer.g4 JassParser.g4 -o "../CS" -visitor