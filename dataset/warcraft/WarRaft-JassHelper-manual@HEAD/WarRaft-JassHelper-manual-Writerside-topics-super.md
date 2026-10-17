---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/super.md"
ref: HEAD
---

# super

When you are extending another struct, it could happen that the struct is extending an interface, or that the method
you are coding is replacing a stub method. What happens if you want to call the parent&apos;s version of the method?
It is not possible without specifying that you want to do it. (Else it will end up calling the child&apos;s method
instead).

`super` is meant to allow that, it works in the same way as this, but it forces the parent&apos;s
method to be called:

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
        call super.xx()
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
    call A.doSomething() //Shows 'Child A|nParent' twice
    call B.doSomething() //Shows 'Child B' twice
endfunction
```