---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Structs-with-more-index-space.md"
ref: HEAD
---

# Structs with more index space

We got a struct X:

```sql
struct X
    integer a
    integer b
endstruct
```

For some reason, the `8190` instances limit is not enough for us, we need `10000` instances ! so:

```sql
struct X[10000]
    integer a
    integer b
endstruct
```

Not to be confused with an instance limit improvement, it is an improvement for index space, both terms are usually
equivalent unless there are array members involved:

```sql
struct X[10000]
    integer a[2]
    integer b[5]
endstruct
```

This struct got a maximum instance count of `2000`.

You cannot specify index space enhancers on structs that extend other structs or interfaces.

```sql
struct X[10000] extends Y //bad
    integer a[2]
    integer b[5]
endstruct

interface A[20000] //good
    method a takes nothing returns nothing
endinterface

struct B extends A
    method a takes nothing returns nothing
       call BJDebugMsg("...")
    endmethod
endstruct

struct C[20000] //good
    integer x
endstruct

struct D extends C
    integer y
endstruct
```

Notice that `A`,`B`,`C` and `D` got a limit of `20000` indexes.

It is a little different for array structs, since as you can see, you cannot use the [] storage size specifier and
extends at the same time. Since [](Changelog.md#0.9.E.1), it is possible to use array structs with enhanced storage
specifying the size after the array keyword:

```sql
struct aBigOne extends array [20000]
   integer a
   integer b
   integer c
endstruct

function meh takes nothing returns nothing
   set aBigOne[19990].a = 12
endfunction
```