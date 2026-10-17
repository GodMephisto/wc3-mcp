---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Module.md"
ref: HEAD
---

# Module

A module is like a code package you can place in a struct to gain extra methods or members, etc. module, ...,
endmodule are used to declare a module and implement is used to _copy_ the module&apos;s members to the struct.
Consider this as a high level textmacro.

methods in modules can call/use methods/members that belong to the struct (which could be private), just notice that
if the struct does not have such members, a syntax error would pop up as if you were pasting the module&apos;s code
into the struct. A module&apos;s private members will not be visible to the calling struct and their names will not
conflict with other members in the struct, there are some exceptions, however: create, and onDestroy which will be
handled differently later. You cannot have private operators in modules (operators are often meant for public APIs
so it does not make any sense to make them private anyway).

Since Jasshelper [](Changelog.md#0.9.Z.1), private onInit methods inside a module will be executed on init once per
struct implementing it. Multiple onInit methods from multiple modules can coexist with the struct&apos;s `onInit` method
as well.

```sql
///
// Declare the module, similar to a struct declaration
//
module MyRepeatModule

    method repeat1000 takes nothing returns nothing
     local integer i=0
        loop
            exitwhen i==1000
            call this.sub() //a method that is expected
                            //to exist in the struct
            set i=i+1
        endloop
    endmethod

endmodule

// the struct :
struct MyStruct
    method sub takes nothing returns nothing
        call BJDebugMsg("Hello world")
    endmethod
    implement MyRepeatModule //adds the module.

endstruct

function MyTest takes MyStruct ms returns nothing
    call ms.repeat1000() //will call ms.sub 1000 times.
endfunction
```

You can call other modules from inside a module using implement, adding the keyword optional after implement will
make them module implementation optional, that is if the module cannot be found, no error will show up, vJass will
just ignore the implement call. Another useful idea is to use [](thistype.md) when necessary. If implement attempts to
implement a module that has already been implemented in a struct, the call is ignored as well.

A module&apos;s contents obey the scope rules from the scope/library in which it is declared (if any).

```sql
module MyOtherModule

    method uhOh takes nothing returns nothing
    endmethod
endmodule

///
// Declare the module, similar to a struct declaration
//
module MyModule

    //next line adds a member uhOh that does nothing
    implement optional MyOtherModule

    //since OptionalModule is not declared, next line is ignored
    implement optional OptionalModule

    // This method call requires that the struct had
    //  a copy() method
    static method swap takes thistype A , thistype B returns nothing
        local thistype C = thistype.allocate()
        //we are from the inside, so can use allocate, even though it is private

        call C.copy(A)
        call A.copy(B)
        call B.copy(C)
        call C.destroy()
        
    endmethod

endmodule

// the struct :
struct MyStruct
    integer a
    integer b
    integer c

    //code a copy method
    method copy takes MyStruct x returns nothing
        set this.a = x.a
        set this.b = x.b
        set this.c = x.c
    endmethod

    //get the swap method "for free"
    implement MyModule
    implement MyOtherModule //this module was already include by MyModule, so this line is ignored

endstruct

function MyTest takes MyStruct A, MyStruct B returns nothing
    call MyStruct.swap(A,B) //it now got that method afterall
endfunction
```
