---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Function-interfaces.md"
ref: HEAD
---

# Function interfaces

If functions are objects then we may as well have interfaces for them.

The syntax for function interfaces is: `function interface name takes (arguments) returns (return
value)` It is actually similar to a function declaration.

Variables/values of a function interface type may be called using `execute()` and `evaluate()` as defined above:

To assign to variables of a function interface type you first need to get a function&apos;s pointer. The syntax to
get them is understandable if you assume that every declared function interface will get as static members the
functions found in the map script that follow its argument/return value rules.

```sql
function interface Arealfunction takes real x returns real

function double takes real x returns real
    return x*2.0
endfunction

function triple takes real x returns real
    return x*2.0
endfunction

function Test1 takes real x, Arealfunction F returns real
    return F.evaluate(F.evaluate(x)*F.evaluate(x))
endfunction

function Test2 takes nothing returns nothing
    local Arealfunction fun = Arealfunction.double //syntax to get pointer to function

    call BJDebugMsg( R2S(  Test1(1.2, fun) ))

    call BJDebugMsg( R2S(  Test1(1.2, Arealfunction.triple ) )) //also possible...
endfunction
```

In this example we are actually having functions as arguments and as a variable. You may also typecast(see bellow) a
function pointer to integer and then back to the interface function it originated.

It is also possible to get a function pointer without typing the interface&apos;s name, notice that this will not
allow you to validate the function as following the interface&apos;s declaration, but it is simpler `nontheless.`:

```sql
//repeat a call of the same function on a real variable thrice!

function double takes real x returns real
    return 2*x
endfunction

function square takes real x returns real
    return x*x
endfunction

function interface realfunc takes real x returns real

function repeater3 takes real x, realfunc F returns real
    set x=F.evaluate(x)
    set x=F.evaluate(x)
    set x=F.evaluate(x)
    return x
endfunction

function test takes nothing returns nothing
    local real x = repeater3( 2.0, double) //notice we are just using the functions' names as if they were values.
    local real y = repeater3( 2.0, square)

   //The results are x=16 and y=256.
   //explanation: the first is equivalent to:
   //       set x=2
   //       set x=2*x
   //       set x=2*x
   //       set x=2*x
  
   // While the second is:
   //       set y=2
   //       set y=y*y
   //       set y=y*y
   //       set y=y*y

 // yep, function interfaces allow you to use functions as if they were just another sort of variable.
endfunction
```

You may also use methods for function interfaces, the static method gets converted into a function pointer.

```sql
function interface myFunc takes integer a, integer b returns nothing

struct try
    static method AAAA takes integer a, integer b returns nothing
       call BJDebugMsg( I2S(a)+", "+I2S(b) )
    endmethod

    method ooz takes nothing returns nothing
       local myFunc fun = try.AAAA
          call fun.evaluate(5,6)
          call fun.evaluate(7,8)
    endmethod
endstruct
```

Notice that non-static methods are usable as well for function interfaces. They behave as functions that take an
integer in the first argument though.

When using non-static methods as function pointers, you are just converting them into functions that take an integer
as first argument, the function pointer instance will require one of the struct&apos;s instances to get called.

```sql
function interface myFunc takes integer x returns nothing

struct TestStruct
    string msg
    method AAAA takes nothing returns nothing
       call BJDebugMsg(msg)
    endmethod
endstruct

function test takes nothing returns nothing
    local TestStruct ts = TestStruct.create()
    local myFunc mf
    set ts.msg = "!!! !!! !!! !!! "

    set mf = TestStruct.AAAA

    call mf.evaluate(ts ) //notice that we are explicitly feeding the function a instance

endfunction
```