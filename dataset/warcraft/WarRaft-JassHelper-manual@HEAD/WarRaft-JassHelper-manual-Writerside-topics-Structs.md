---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Structs.md"
ref: HEAD
---

# Structs

Structs introduce Jass to the object oriented programming paradigm.

I am unable to explain them without making an example first:

```sql
struct pair
    integer x
    integer y
endstruct

function testpairs takes nothing returns nothing
    local pair A=pair.create()
    set A.x=5
    set A.x=8

    call BJDebugMsg(I2S(A.x)+" : "+I2S(A.y))

    call pair.destroy(A)

endfunction
```

As you can see, you can store multiple values in a single struct, then you can just use the struct as if it was another
Jass type, notice the `.` syntax which is used for members on most common languages.