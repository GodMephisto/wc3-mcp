---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Extending-structs.md"
ref: HEAD
---

# Extending structs

It is possible to base an struct from a previously declared struct, by doing this your new type is going to acquire
methods and variable members of the base struct, it is also able to use instances of this type with functions and
variables of the base type.

Consider doing this as a way to add code and properties to a previous type.

```sql
struct A
   integer x
   integer y

   method setxy takes integer cx, integer cy returns nothing
       set this.x=cx
       set this.y=cy
   endmethod
endstruct

struct B extends A
   integer z
   method setxyz takes integer cx, integer cy, integer cz returns nothing
       call this.setxy(cx,cy) //we can use A's members
       set this.z=cz
   endmethod
endstruct
```

Internally, `B.allocate()` is actually calling `A`&apos;s constructor and B.destroy will also call `B`&apos;s
deconstructor. If a base struct got a custom create method, the structs extending it will have to use it for
`allocate()`. If the custom create method requires arguments, the allocate method for child structs will require the
same arguments:

```sql
struct A
   integer x
   static method create takes integer k returns A
    local A s=A.allocate()
       set A.x=k
    return s
   endmethod
endstruct

struct B extends A
    static method create takes nothing returns B
        return s= B.allocate(445)  //notice that B.allocate requires the same arguments as A.create()
    endmethod
endstruct

struct C extends B //yep, it is possible to extend an struct that is extending another one
    static method create takes nothing returns B
     local C s=C.allocate() //C is a child of B that got a custom create method that takes nothing, so allocate takes nothing as well.
       set s.x=s.x*s.x //once again reusing the parents' members.
     return s
    endmethod
endstruct
```

If an struct has declared create to be private, it is impossible to extend it. Structs cannot use private members
from parent structs.

The behaviour of the onDestroy method is special in this case, if in the last example, `A`,`B` and `C` had an onDestroy
method each, destroying an instance of a `C` would call `C.onDestroy()`, `B.onDestroy()` and `A.onDestroy()` (in that
order)

It is possible to extend an struct that extends an interface, in this case, the child that extends the interface
directly is forced to implement the interface&apos;s methods, but its childs are not. But it is possible for them to
replace them again.

```sql
interface myinterface
   method processunit takes unit u returns nothing
   method onAnEvent takes nothing returns boolean
endinterface

struct A extends myinterface

   method processunit takes unit u returns nothing
       call KillUnit(u)
   endmethod

   method onAnEvent takes nothing returns boolean
       return false
   endmethod
endstruct

struct B extends A

   method processunit takes unit u returns nothing
      //we have just replaced A&apos;s processunit method,
      //if an interface variable of type myinterface holds an instance of 
      //type B it will explode the unit.
      call ExplodeUnitBJ(u)
   endmethod
   // we are implementing processunit but we do not have to implement onAnEvent   

endstruct
```

If you plan using `interface.create()` you will have to be careful once again about constructors with arguments, if an
interface is declared with the condition that create takes nothing every child (,grandchild, etc) of the interface
will be affected by this condition.