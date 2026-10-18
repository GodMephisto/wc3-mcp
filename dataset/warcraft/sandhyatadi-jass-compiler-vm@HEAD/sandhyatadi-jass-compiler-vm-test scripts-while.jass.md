---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/jass-compiler-vm"
source_path: "test scripts/while.jass"
ref: HEAD
---

{
int count=0;
print("Even numbers from 0 to 20:"#);
while ( count <= 20 )
{
print(count#);
count=count+2;
}
}