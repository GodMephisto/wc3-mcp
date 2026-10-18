---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/jass-compiler-vm"
source_path: "test scripts/ternary.jass"
ref: HEAD
---

{
int a = 1;
int b = 5;
int d;
int c;
print(" a is "a#);
print(" b is "b#);
c = (a<<b)?{d=(b-a)}:{d=(a-b)};
print("The difference is "d#);
}
