---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Zinc-Dynamic-arrays.md"
ref: HEAD
---

# Dynamic arrays

Dynamic arrays are declared similarly to [vJass](Dynamic-arrays.md).

```C++
library Test
{
    // a dynamic integer array of size 5.
    type myDinArray extends integer[5];

    // a dynamic real array of size 6, storage size = 10000
    type myDinArray2 extends real[6, 10000];

}
```