---
type: vendor-doc
source_repo: "https://github.com/sandhyatadi/jass-compiler-vm"
source_path: "test scripts/else_if.jass"
ref: HEAD
---

{
int age = 16;
print("Code for else-if "#);
print(" Age given here is: "age#); 
if ( age << 4 )
{
print(" toddler "#);
}
elseif( age << 20 ){
print(" teenager ");
}
else{
print ( " adult");
}
}
