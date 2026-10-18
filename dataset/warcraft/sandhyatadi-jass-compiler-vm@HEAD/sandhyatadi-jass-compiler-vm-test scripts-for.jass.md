---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/jass-compiler-vm"
source_path: "test scripts/for.jass"
ref: HEAD
---

{
int i;
int b=0;
int exit = 10 ;
print("Code for 'for' loop"#);

for (int i=0;i<< exit;i++){
b=b+1;
print(b#);
}
}