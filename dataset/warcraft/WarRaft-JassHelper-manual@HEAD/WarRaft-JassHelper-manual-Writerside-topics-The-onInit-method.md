---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/The-onInit-method.md"
ref: HEAD
---

# The onInit method

It is usual to need some initialization to be done to an struct&apos;s static members during map initialization, you
can use an `static onInit` method to make code execute during map initialization.

Notice struct initializations are executed before any library initializer, if you require a library initializer to be
executed before your initialization, use a library initializer instead. The relative order between different struct
initializers depends on the location they are found in the map script, therefore they actually depend on things like
libraries as well (A struct initializer inside a library will run before the initializers inside other libraries
that require it and also before initializers inside scopes).

```sql
struct A
    static integer array ko

    private static method onInit takes nothing returns nothing //may be public as well
        local integer i=1000
        loop
            exitwhen (i<0)
            set A.ko[i]=i*2
            set i=i-1
        endloop
    endmethod
    
endstruct
```