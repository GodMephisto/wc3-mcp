---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Array-structs.md"
ref: HEAD
---

# Array structs

Sometimes, you&apos;d like to have a global array of a struct type, just to be able to have that field syntax we all
like so much, it can be more complicated than it is supposed to, for example you have to manually initialize all the
indexes to create the unique indexes, etc. Another issue is when you do not really want to use `.allocate()` and
`.destroy()` you would like to have your own ways for allocation. Array structs are a small syntax enhancement that is
equivalent to an array of a struct type, you would be able to use the members for each index and you will not have
to worry about `.create()`.

```sql
//Array structs are hard to explain, but should be simple to understand with an example

struct playerdata extends array //syntax to declare an array struct
    integer a
    integer b
    integer c
endstruct

function init takes nothing returns nothing
    local playerdata pd

    set playerdata[3].a=12  //modifying player 3's fields.
    set playerdata[3].b=34  //notice it behaves as a global array
    set playerdata[3].c=500

    set pd=playerdata[4]
    set pd.a=17             //modifying player 4's fields.
    set pd.b=111            //yep, this is also valid
    set pd.c=501
endfunction

function updatePlayerStuff takes player p returns nothing
    local integer i=GetPlayerId(p)

    //some random function.
    set playerdata[i].a=playerdata[i].b

endfunction
```

Certain issues with array structs: You cannot declare default values (they would automatically be zero, null or false
depending on the type of the member) , you cannot declare onDestroy (it would be pointless), you cannot use
`.allocate` or `.destroy`, you cannot have array members. Notice that the problem with default values and array members
are likely to be fixed in a next version.

Notice that you can use operator declarations to override the get `[]` operator, in this case, to be able to use ids
you would be able to use the typecast operator, e.g. `playerdata(4)` to get the instances. If you did not understand
this last paragraph, don&apos;t worry, you probably did not need to know this anyway.
