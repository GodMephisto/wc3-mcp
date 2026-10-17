---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Method-function-name.md"
ref: HEAD
---

# Method function name

Methods may work as objects to use the `.execute()`/`.evaluate()` feature, they may also work as objects to allow access
to the name field. This .name field will return the function name given to a method after the compiling.

This is specially useful in case you want to use an struct&apos;s static method on an `ExecuteFunc` based system.

```sql
struct mystruct
    static method mymethod takes nothing returns nothing
        call BJDebugMsg("this works")
    endmethod
endstruct

function myfunction takes nothing returns nothing
    call ExecuteFunc(mystruct.mymethod.name) //ExecuteFunc compatibility

    call OnAbilityCast('A000',mystruct.mymethod.name)
    //for example, caster system's OnAbilityCast, requires a function name
endfunction
```