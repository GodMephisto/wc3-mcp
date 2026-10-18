---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/JASS_Compiler"
source_path: "SER502-Spring2021-Team21-main/src/compiler/script.bat"
ref: HEAD
---

python lexer.py %1
swipl -q -t main main.pl