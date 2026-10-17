---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Method-exists.md"
ref: HEAD
---

# Method exists

Another field used by methods is the exists field, it is a boolean field that returns true if the method has been
declared or false otherwise. Most of the times it would be true, the only case whatsoever in which it could be false
is if the struct is extending a interface that uses defaults for the method.

```sql
interface myInterface
    method myMethod1 takes nothing returns nothing
    method myMethod2 takes nothing returns nothing
endinterface

struct myStruct
    method myMethod1 takes nothing returns nothing
        call BJDebugMsg("er")
    endmethod
endstruct

function test takes nothing returns nothing
    local myInterface mi = myStruct.create()
    //outputs:
    // yes
    // no
    if( mi.myMethod1.exists) then
        call BJDebugMsg("Yes")
    else
        call BJDebugMsg("No")
    endif
    if( mi.myMethod2.exists) then
        call BJDebugMsg("Yes")
    else
        call BJDebugMsg("No")
    endif

endfunction
```