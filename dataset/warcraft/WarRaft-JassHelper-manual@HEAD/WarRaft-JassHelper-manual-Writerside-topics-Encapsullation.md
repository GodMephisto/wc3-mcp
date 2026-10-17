---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Encapsullation.md"
ref: HEAD
---

# Encapsullation

Encapsullation is an object oriented programming concept in which you only give access to things that need to have
access, in other words it is private and public for structs

```sql
struct encap
    real a=0.0
    private real b=0.0
    public real c=4.5

    method randomize takes nothing returns nothing
        // All legal:
        set this.a= GetRandomReal(0,45.0)
        set this.b= GetRandomReal(0,45.0)
        set this.c= GetRandomReal(0,45.0)
    endmethod

endstruct

function test takes nothing returns nothing
    local encap e=encap.create()

     call BJDebugMsg(R2S(e.a)) //legal
     call BJDebugMsg(R2S(e.c)) //legal
     call BJDebugMsg(R2S(e.b)) //syntax error

endfunction
```

private members can only be used inside the struct declaration. Public and private are options for both variables and
methods.

All struct members are public by default, so the public keyword is not necessary, on the other hand you must specify
private. Something to point out is the existance of the readonly keyword, it allows code outside the struct to read
the variable but not to assign it. It is a nonstandard at the moment, so you shouldn&apos;t use it on public
releases.