---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Operator-making.md"
ref: HEAD
---

# Operator making

Jasshelper allows you to declare custom operators for your structs, these operators would then be converted to method
calls, vJass currently allows operators for `<`, `>` , array set and array get.

The official name for this process (In wikipedia and books) is operator overloading. In the case of vJass, An
overloaded operator is a method, but it gets operator as name, after the operator keyword you specify the operator
being overloaded.

An example is worth 1000 words:

```sql
struct operatortest
    string str=""

    method operator [] takes integer i returns string
        return SubString(.str,i,i+1)
    endmethod

    method operator[]= takes integer i, string ch returns nothing
        set .str=SubString(.str,0,i)+ch+SubString(.str,i+1,StringLength(.str)-i)
    endmethod

endstruct


function test takes nothing returns nothing
    local operatortest x=operatortest.create()
    set x.str="Test"
    call BJDebugMsg( x[1])
    call BJDebugMsg( x[0]+x[3])

    set x[1] = "."
    call BJDebugMsg( x.str)
endfunction
```

By this example we are overloading the `[]` operator and giving it a new function for the objects of type
`operatortest`.
The operator `[]` specifies the replacement for array get and `[]=` is the replacement for array get.

After inspecting the code you may notice that we are making the string function as an array of strings (or actually
characters)

The `[]` operator requires 1 argument (index), the `[]=` operator requires 2 arguments (index and value to assign). `[]`
must return a value.

`[]` and `[]=` operators can also be declared as static. This might have some uses, the struct name is going to be
allowed to use index operators.

There is a lot of criticism towards operator overloading since it allows programmers to make code that does not make
sense, please use this feature with responsibility.

You can also overload `<` and `>` , notice that there is only syntax to declare `<` by declaring it you are
forcefully determining an order relation for structs of that type. So it automatically makes `>` based on your `<`
declaration.

```sql
struct operatortest
    string str=""

    method operator [] takes integer i returns string
        return SubString(.str,i,i+1)
    endmethod

    method operator[]= takes integer i, string ch returns nothing
        set .str=SubString(.str,0,i)+ch+SubString(.str,i+1,StringLength(.str)-i)
    endmethod

    method operator< takes  operatortest b returns boolean
        return StringLength(this.str) < StringLength(b.str)
    endmethod

endstruct


function test takes nothing returns nothing
    local operatortest x=operatortest.create()
    local operatortest y=operatortest.create()

    set x.str="Test..."
    set y.str=".Test"

    if (x<y) then
        call BJDebugMsg("Less than")
    endif
    if (x>y) then
        call BJDebugMsg("Greater than")
    endif
endfunction
```

In the example, an object of type `operatortest` is considered greater than another object of that type if the length
of its str member is greater than the length of the other object&apos;s str member.

`operator<` must return a boolean value and take an argument of the same type as the struct.

Operators are interface friendly meaning that an interface may declare operators, there is a catch and it is that the
`operator<` must be declared without signature in an interface. Also when using `>` or `<` to compare interface
objects both instances must have the same type, otherwise the function would halt before performing the comparisson,
if debug mode was enabled when compiling, it will also show a warning message.

```sql
interface ordered
    method operator <
endinterface

interface indexed
    method operator [] takes integer index returns ordered
    method operator []= takes integer index, ordered v returns nothing
endinterface

function sort takes indexed a, integer from, integer to returns nothing
    local integer i
    local integer j
    local ordered aux

    set i=from
    loop
        exitwhen (i>=to)
        set j=i+1
        loop
            exitwhen (j>to)
            if (a[j]<a[i]) then
                set aux = a[i]
                set a[i] = a[j]
                set a[j] = aux
            endif
            set j=j+1
        endloop

        set i=i+1
    endloop
endfunction
```

This is an interface for a sorting algorithm. We may now declare custom types that work to sort stuff:

```sql
truct integerpair extends ordered
    integer x
    integer y

    method operator< takes integerpair b returns boolean
        if (b.x==this.x) then
            return (this.y<b.y)
        endif
        return (this.x<b.x)
    endmethod
endstruct

type ipairarray extends integerpair array [400]

struct integerpairarray extends indexed
    ipairarray data

    method operator[] takes integer index returns ordered
        return ordered( this.data[index] )
    endmethod

    method operator[]= takes integer index, ordered value returns nothing
        set this.data[index] = integerpair( value)
    endmethod
endstruct
```

Of course, it is just an example, the logical way would be using quicksort, operators are also good since they would
also allow textmacros to use them, the same sorting textmacro might then be compatible with integer, real and any
struct with overloaded `<` operator.

You may also declare a custom ==, works same as `<` if you declare this operator, `!=` will be translated to `not(your
method)`. Also, notice that to do pointer comparisons you will need to use `integer(var1)==integer(var2)`

## More things we can do with custom operators

One thing is to `overload` `[]`, `>`, `<`, you can also make a method mimic a field, to keep abstraction and
simplify syntax.

```sql
struct X
    integer a=2
    integer b=2

    method operator x takes nothing returns integer
        return this.a*this.b
    endmethod

    method operator x= takes integer v returns nothing
        set this.a=v/this.b
    endmethod
endstruct

function test takes nothing returns nothing
    local X obj= X.create()

    set obj.x= obj.x + 4

    call BJDebugMsg(I2S( obj.x) ) //outputs 8
    set obj.b=4

    call BJDebugMsg(I2S( obj.x) ) //outputs 16

endfunction
```

You can use this to implement read only fields:

```sql
struct X
    private integer va=2

    method operator a takes nothing returns integer
        return this.a
    endmethod

endstruct

function test takes nothing returns nothing
    local X obj= X.create()

    call BJDebugMsg(I2S( obj.a) ) //This is legal

    set obj.a=2 //this is not

endfunction
```

More importantly, you can use it to implement fields that take extra provisions for assigments:

```sql
struct movableEffect

    private unit dummy
    private string rfx
    private effect uniteffect

    //...
        //(A lot of code implementing other actions and creation)
    //...

    method operator x takes nothing returns real
        return GetUnitX(this.dummy)
    endmethod

    method operator y takes nothing returns real
        return GetUnitY(this.dummy)
    endmethod

    method operator x= takes real value returns nothing
        call SetUnitX(this.dummy, value)
    endmethod

    method operator y= takes real value returns nothing
        call SetUnitY(this.dummy, value)
    endmethod


    method operator effectpath takes nothing returns string
        return this.rfx
    endmethod

    method operator effectpath= takes string path returns nothing
        set this.rfx=path
        call DestroyEffect( this.uniteffect)
        set this.uniteffect = AddSpecialEffectTarget(this.dummy, path, "origin")
    endmethod

endstruct


function moveRandom takes movableEffect me returns nothing
    set me.x= me.x + GetRandomReal(-50,50)
    set me.y= me.y + GetRandomReal(-50,50)
endfunction

function toFire takes movableEffect me returns nothing
    set me.effectpath ="war3mapimporte\\cutefireeffect.mdl"
endfunction
```

> With operators like `.fieldname=` and `[]=` it is possible to have a return value in the method, however
> this return value would almost always be impossible to get from outside the function, there is an exception, and it
> is when these methods return a value of the struct's type, then it will get translated to an assignment. For
> example, instead of call `var_set(object,45)`, the result would be set `object=var_set(object,45)`

> Since [](Changelog.md#0.9.Z.1), this syntax is also supported for static members.
