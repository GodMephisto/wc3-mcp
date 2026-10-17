---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Instance-members.md"
ref: HEAD
---

# Instance members

So, you can declare struct members of any type, even struct types. You cannot however declare array members, this
limitation should be removed in later versions.

```sql
struct pairpair
    pair x=0 //you cannot use pair.create() and should actually only use constants for default initial values.
    pair y=0
endstruct

function testpairs takes nothing returns nothing
    local pairpair A=pairpair.create()
    local pair x

    set A.x=pair.create()
    set A.y=pair.create()

    set x=A.y //notice we are saving A.y in a backup variable so we can destroy it.
    set A.y= pair_sum(A.x,A.y) //this replaces A.y that's the reason we saved it

    call BJDebugMsg(I2S(  A.y.x )+" : "+I2S( A.y.y )) //notice the nesting of the . operator

    call A.x.destroy()
    call A.y.destroy()
    call A.destroy()
    call x.destroy()
endfunction
```