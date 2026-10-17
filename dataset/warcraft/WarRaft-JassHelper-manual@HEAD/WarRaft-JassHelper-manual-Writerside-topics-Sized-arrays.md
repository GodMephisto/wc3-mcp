---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Sized-arrays.md"
ref: HEAD
---

# Sized arrays

Global arrays might sometimes require more index space, jasshelper introduces syntax for sized arrays, it serves two
purposes: It will allow you to request more space, and it also allows you to
place a `.size` field on global arrays.

```sql
globals
    integer array myArray [500]
endglobals

function test takes nothing returns nothing
    local integer i=0

    call BJDebugMsg(I2S(myArray.size)) //prints 500

    loop
        exitwhen i>=myArray.size
        set myArray[i]=i
        set i=i+1
    endloop
endfunction
```

Of course, you can bypass the `8191` array size limit:

```sql
globals
    integer array myArray [9000]
endglobals
```

You can use a constant as well:

```sql
globals
    constant integer Q= 60000
    integer array myArray [Q]
endglobals
```

You can use this on struct [static member](Static-members.md) arrays. (`static integer A[10000]`)