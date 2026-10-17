---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Interfaces.md"
ref: HEAD
---

# Interfaces

Polymorphism is an OOP concept in which different object classes may have the same action, although the action is
different, the action gets the same name. For example both an ant and a person run, but they are pretty different
objects and the run action is implemented in different ways.

An interface is like a set of rules struct types follow and allow you to call actions of an struct without really
knowing the exact type of the struct.

```sql
interface printable
    method toString takes nothing returns string
endinterface

struct singleint extends printable
    integer v
    method toString takes nothing returns string
        return I2S(this.v)
    endmethod
endstruct

struct intpair extends printable
    integer a
    integer b

    method toString takes nothing returns string
        return "("+I2S(this.a)+","+I2S(this.b)+")"
    endmethod
endstruct

function printmany takes printable a, printable b, printable c returns nothing
    call BJDebugMsg( a.toString()+" - "+b.toString()+" - "+c.toString())
endfunction


function test takes nothing returns nothing
    local singleint x=singleint.create()
    local singleint y=singleint.create()
    local intpair z=intpair.create()
    
    set x.v=56
    set y.v=12
    set z.a=45
    set z.b=12
    
    call printmany(x,y,z)
endfunction
```

The printmany function takes three arguments of type printable, it does not know exactly which types the objects are,
only that they are of struct types that extend printable

The rule for an struct to follow the printable interface is that it has a `toString()` method, the `printmany` function
can
use that method even though it does not know the exact types of the arguments

The `toString()` method is different for `singleint` and `intpair`, when `printmany` is called it calls the correct
version of
`toString()` for each argument, the result for the above sample is : `56 - 12 - (45,12)`.

Interfaces can have any number of methods and structs that extend them should implement all those methods, but can ther
methods implemented as well.

It is illegal to declare onDestroy for an interface declaration, you can consider it to be automatically declared, you
can use `.destroy()` on a variable of interface type and it will call the appropiate onDestroy method when necessary.

Interfaces can also implement variables, in this case interfaces allow some pseudo inheritance

```sql
interface withpos
    real x
    real y
endinterface

struct rectangle extends withpos
    real a
    real b

    static method from takes real x, real y, real a, real b returns rectangle
     local rectangle r=rectangle.create()
        set r.x=x
        set r.y=y
        set r.a=a
        set r.b=b
        return r
    endmethod

endstruct

struct circle extends withpos
    real radius=67.0

    static method from takes real x, real y, real rad returns circle
     local circle r=circle.create()
        set r.x=x
        set r.y=y
        set r.radius=rad
        return r
    endmethod

endstruct


function distance takes withpos A, withpos B returns real
    local real dy=A.y-B.y
    local real dx=A.x-B.x

    return SquareRoot( dy*dy+dx*dx)
endfunction

function test takes nothing returns nothing
    local circle c= circle.from(12.0, 45.0 , 13.0)
    local rectangle r = rectangle.from ( 12.3 , 67.8, 12.0 , 10.0)
    
    call BJDebugMsg(R2S(distance(c,r)))

endfunction
```

It is possible to acquire the type id of an instance of an struct that extends an interface, this type id is an
integer number that is unique per struct type that extends that interface.

```sql
interface A
    integer x
endinterface

struct B extends A
    integer y
endstruct

struct C extends A
    integer y
    integer z
endstruct

function test takes A inst returns nothing
   if (inst.getType()==C.typeid) then
     // We know for sure inst is actually an instance of type C
       set C(inst).z=5 //notice the typecast operator
   endif
   if (inst.getType()==B.typeid) then
       call BJDebugMsg("It was of type B with value "+I2S( B(inst).y  ) )
   endif
endfunction
```

In short, `.getType()` is a method that you use on instances of an object whose type extends an interface. And `.typeid`
is an static constant set for struct types that extend an interface.

So, in the example we get to recognize that the given inst argument is of type C, then we can do the typecast and
assignment.

There is another feature that uses typeids got another feature, and it is that interfaces got a constructor method
that would create a new object given a correct typeid.

For example:

```sql
interface myinterface
    method msg takes nothing returns string
endinterface

struct mystructA extends myinterface
    method msg takes nothing returns string
        return "oranges"
    endmethod
endstruct

struct mystructB extends myinterface

   string x
   static method create takes nothing returns mystructB
    local mystructB m=mystructB.allocate()

       set m.x="apples"

       return m
   endmethod

   method msg takes nothing returns string
       return this.x
   endmethod
endstruct

struct mystructC extends myinterface
   string x

   //myinterface.create(...) can only use the default allocator or custom create
   //methods that take nothing.
   //
   //this declaration is not going to be taken into account if mystructC
   //is used in myinterface.create(...)
   //
   static method create takes string astring returns mystructC
    local mystructB m=mystructB.allocate()

       set m.x=astring

       return m
   endmethod

   method msg takes nothing returns string
       return this.x
   endmethod
endstruct

function test takes nothing returns nothing
    local integer T = mystructB.typeid
    local myinterface A


    set A=myinterface.create(mystructA.typeid) //this is not that useful since mystructA.create() does the same
    call BJDebugMsg(A.msg())

    set A=myinterface.create(T) //this is more useful, we can create objects of variable types...
    call BJDebugMsg(B.msg())

    set A=myinterface.create(122345) //using invalid values or 0 will make .create return 0 (no object)

    set A=myinterface.create(mystructC.typeid) //note that this will not use mystructC.create, just 
                                               //mystructC.allocate, possibly causing errors, if this
                                               //happens you can an error message in-game if you compile under debug mode

    call BJDebugMsg(C.msg())

endfunction
```

If you plan using this feature, always be careful to handle 0 return values and also specify that it is better to use
constructors without arguments on the interface&apos;s children.

It is also possible to declare the interface in a way that all the childs use a custom create method that returns
nothing.

```sql
interface myinterface
    static method create takes nothing

    method qr takes unit u returns nothing
endinterface

struct st1 extends myinterface

    static method create takes nothing returns st1 //legal
        return st1.allocate()
    endmethod

    method qr takes unit u returns nothing
        call KillUnit(u)
    endmethod
endstruct

struct st2 extends myinterface
    integer k

    static method create takes integer f, integer k returns st2 //not legal
     local st2 s=st2.allocate()
        set st2.k=f+k*f
     return st2
    endmethod

    method qr takes unit u returns nothing
        call ExplodeUnitBJ(u)
    endmethod
endstruct
```

Interface methods allow you to use the `defaults` keyword. The `defaults` keyword allows methods to be
optional when implementing the child struct. So if the method is not present in the child struct it will not show
syntax errors requesting the method to be implemented. Instead we will implement a default empty method.

`defaults` is followed by `nothing` or by a value depending on the return type of the method (if the
method returns nothing then it should default nothing, else it should default a value). `defaults` only
supports constant values.

```sql
interface whattodo
    method onStrike takes real x, real y returns boolean defaults false
    method onBegin  takes real x, real y returns nothing defaults nothing

    method onFinish takes nothing returns nothing
endinterface

struct A extends whattodo //don't forget the extends...

    method onFinish takes nothing returns nothing //must be implemented
        //.. code
    endmethod

    // We are allowed to add onBegin, but not forced to
    method onBegin takes real x, real y returns nothing

        //.. code 
    endmethod

    // when somebody calls .onStrike on a whattodo of type A, it will return false
endstruct

struct B extends whattodo
    method onFinish takes nothing returns nothing //must be implemented
        //.. code
    endmethod

    // when somebody calls .onBegin on a whattodo of type A, it will do nothing
endstruct
```