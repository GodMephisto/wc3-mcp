---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/jass-compiler-vm"
source_path: "test scripts/unary.jass"
ref: HEAD
---

{
int a=5;
int temp;
temp=a;
print("Initial value of a is "a#);
a++;
print("Increment of a is " a#);
temp--;
print("Decrement of a is " temp#);
}