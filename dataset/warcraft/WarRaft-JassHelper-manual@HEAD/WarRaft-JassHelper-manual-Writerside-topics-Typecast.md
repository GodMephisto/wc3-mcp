---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Typecast.md"
ref: HEAD
---

# Typecast

For the moment, assigning a value of an struct type to an integer variable / or variable of any other struct type
will already change the type of that reference for the compiler

Notice that sometimes using a variable might get tedious or even create unneeded overhead, that is the reason the
type cast operator was added.

Its syntax is mostly like a function but the name of the function is the name of a custom type. It will soon allow
native types as well.

```sql
interface  wack
    //... some declarations
endinterface

struct wek extends wack
   integer x
   // some more declarations
endstruct

function test takes wack W returns nothing
    //You are certain that W is of type wek, a way to cast the value is:
    local wek jo = W
    set jo.x= 5 //done
    
    // but sometimes creating a variable is too much work for the virtual machine and you
    // are only accessing it once 
    
    set wek(W).x=5 //also works.
endfunction

type anarrayofdata extends integer array [6]

function getdata5 takes unit u returns integer
    //a reference to an object of type anarrayofdata is saved as the unit's custom value
    return anarrayofdata(GetUnitUserData(u))[5]
endfunction

function setdata5 takes unit u, anarrayofdata x returns nothing
    // Here we are doing the opposite, notice that integer may be used as a type cast
    // operator for struct and dynamic array types.
    call SetUnitUserData(u,  integer(x))
endfunction
```