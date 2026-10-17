---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Struct-usage.md"
ref: HEAD
---

# Struct usage

Just declare struct values the way you declare variables/functions/arguments of normal types.

Once an struct is declared and you create it and you want to access it, you have to access its members, the way is
often `(struct value).(member name)` , in the case of pairs you use pair.x to access the x member and pair.y to access
the
y member.

Once a member is accessed the usage is quite similar to the usage of a variable. You can use it in set statements or as
a value inside expressions.

```sql
struct pair
    integer x=1
    integer y=2
endstruct

function pair_sum takes pair A, pair B returns pair
    local pair C=pair.create()
    set C.x=A.x+B.x
    set C.y=A.y+B.y
    return C
endfunction

function testpairs takes nothing returns nothing
    local pair A=pair.create()
    local pair B=pair_sum(A, A)
    local pair C=pair_sum(A,B)

    call BJDebugMsg(I2S(C.x)+" : "+I2S(C.y))

    //Dont forget, if you are not using an struct instance anymore, you destroy it
    call B.destroy()
    call C.destroy()
    call pair.destroy(A)

endfunction
```

It would display `3 : 6`;