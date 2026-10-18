---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/jass-compiler-vm"
source_path: "test scripts/factorial.jass"
ref: HEAD
---

{
int number=5;
int temp=1;
for(int i=2 ; i<=number; i++){

temp= temp*i;
}
print("factorial of 5 is "temp);
}