---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/jass-compiler-vm"
source_path: "test scripts/sumofnumbers.jass"
ref: HEAD
---

{
int a = 10;
int b = 0;
for (int i=1;i <= a;i++){
b=b+i;
}
print("sum of first 10 natural numbers is " b#);
}