---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Dynamic-arrays.md"
ref: HEAD
---

# Dynamic arrays

Dynamic arrays are arrays you can instanciate dynamically, EACH custom array type has got a limit of `8190` TOTAL
indexes, that means that an array type of size 100 has got an 81 instances limit.

They are kind of easy to declare and use, and somehow share syntax with structs.

You simply make a line outside any function/struct declaration: `type <nameoftype> extends <nameofbasetype>
array [<size>]` Where size is an integer value or constant global.

Then you can simply create/usem them in a similar way to structs and use the `[]` operator to access its indexes,
dynamic arrays have also got an static size constant

```sql
type arsample extends integer array[8]

function test takes nothing returns arsample
 local arsample r=arsample.create()
 local integer i=0
     loop
         exitwhen i==arsample.size //holds size of the array type
         set r[i]=i
         set i=i+1
     endloop
 return r
endfunction

function test2 takes nothing returns arsample
 local arsample r=test()
 local integer i=0
     loop
         exitwhen i==arsample.size
         call BJDebugMsg(I2S(r[i]))
         set i=i+1
     endloop
 return r
endfunction
```

You can extend arrays of any type, even of custom types (structs, interfaces, other dynamic arrays) thus making a
dynamic array of dynamic arrays, a matrix like syntax is possible:

```sql
type iar extends integer array[3]
type iar_ar extends iar array[3]

function test takes nothing returns arsample
    local iar_ar r=iar_ar.create()
    local integer i=0
    local integer j
    loop
        exitwhen i==iar_ar.size //holds size of the array type
        set r[i]=iar.create()
        set j=0
        loop
        exitwhen j==iar.size
        set r[i][j]=j*i
        set j=j+1
        endloop
        
        set i=i+1
    endloop
    return r
endfunction
```

And structs may have these arrays as members thus allowing array members for instances (non-static)

```sql
type stackarray extends integer array [20]

struct stack
   private stackarray V
   private integer N=0

   method Create takes nothing returns stack
    local stack s=stack.create()
       set s.V=stackarray.create()
       if (s.V==0) then
           debug call BJDebugMsg("Warning: not enough space for stack array")
           return 0
       endif
   endmethod

   method push takes integer i returns nothing
       if (this.N==stackarray.size) then
           debug call BJDebugMsg("Warning: stack is full")
       else
           set this.V[this.N]=i
           set .N = .N +1 //remember this syntax is valid as well
       endif
   endmethod

   method pop takes nothing returns nothing
       if (this.N>0) then
           set this.N=this.N-1
       else
           debug call BJDebugMsg("Warning: attempt to pop an empty stack");
       endif
   endmethod

   method top takes nothing returns integer
       return .V[.N-1]
   endmethod

   method empty takes nothing returns boolean
       return (.N==0)
   endmethod

   method onDestroy takes nothing returns nothing
       call this.V.destroy()
   endmethod
endstruct
```

As you may notice, if there is no space for a new instance, the create method of arrays returns `0`. It will also warn
you automatically if compiled in debug mode

Dynamic arrays got the `.size` member that allows you to easily access the size you used to declare the array type.
