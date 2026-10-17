---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Arithmetic-assignments.md"
ref: HEAD
---

# Arithmetic assignments

Zinc now includes statements to quickly assign and perform artithmetic operations at the same time.

```C++
library ForTest
{
    function onInit(){
        integer x = 5;
        x += 3; // x = 8
        x -= 6; // x = 2
        x *= 32;// x = 64
        x /= 8; // x = 8
    }
}
```

> Note that these are assignments and not usable in places where expressions are expected.