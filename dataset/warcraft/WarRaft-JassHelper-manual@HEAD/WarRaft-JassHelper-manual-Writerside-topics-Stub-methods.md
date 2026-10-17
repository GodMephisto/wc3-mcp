---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Stub-methods.md"
ref: HEAD
---

# Stub methods

`stub` methods can simply be rewriten by child structs. An example should help:

```sql
struct Parent

    stub method xx takes nothing returns nothing
        call BJDebugMsg("Parent")
    endmethod

    method doSomething takes nothing returns nothing
        call this.xx()
        call this.xx()
    endmethod

endstruct

struct ChildA extends Parent
    method xx takes nothing returns nothing
        call BJDebugMsg("- Child A -")
    endmethod
endstruct

struct ChildB extends Parent
    method xx takes nothing returns nothing
        call BJDebugMsg("- Child B --")
    endmethod
endstruct

function test takes nothing returns nothing
    local Parent P = Parent.create()
    local Parent A = ChildA.create()
    local Parent B = ChildB.create()
    //notice the variables are of the 'Parent' type.
    call P.doSomething() //Shows 'Parent' twice
    call A.doSomething() //Shows 'Child A' twice
    call B.doSomething() //Shows 'Child B' twice
endfunction
```

Just notice there are differences between these and interfaces, first of all, interfaces require you to make the
methods. They also allow the `.exists()`.
