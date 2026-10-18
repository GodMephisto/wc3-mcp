---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/JASS_Compiler"
source_path: "SER502-Spring2021-Team21-main/data/ternary.jass"
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
