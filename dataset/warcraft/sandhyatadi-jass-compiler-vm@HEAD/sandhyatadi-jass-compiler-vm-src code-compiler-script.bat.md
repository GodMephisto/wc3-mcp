---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/jass-compiler-vm"
source_path: "src code/compiler/script.bat"
ref: HEAD
---

python lexer.py %1
swipl -q -t main main.pl