---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/thistype.md"
ref: HEAD
---

# thistype

The `thistype` keyword behaves exactly as the struct&apos;s name in code that is inside a struct.

```sql
//The next code,
struct test 
    thistype array ts
    method tester takes nothing returns thistype
        return thistype.allocate()
    endmethod
endstruct

//Is equivalent to:

struct test 
    test array ts
    method tester takes nothing returns test
        return test.allocate()
    endmethod
endstruct
```

The intended usage for thistype, is when it is actually necessary, e.g: textmacros, modules. I do not endorse the
idea of people using this so they can rename the struct later, but I guess they are allowed to.
