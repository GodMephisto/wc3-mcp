---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/jass-compiler-vm"
source_path: "test scripts/for_range.jass"
ref: HEAD
---

{
int num = 1 ;
print("Initially Number = " num#);
for i in range(1,10) {
num=num+1;
}
print("After Iteration Number = "num#);
}
