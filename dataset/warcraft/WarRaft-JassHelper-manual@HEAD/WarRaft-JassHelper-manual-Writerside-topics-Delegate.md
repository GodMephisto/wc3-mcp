---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Delegate.md"
ref: HEAD
---

# Delegate

So far, we&apos;ve seen many things, interfaces, structs extending other structs, operators, dynamic arrays. You
might be asking yourself, is it possible he would add <i>another</i> way to confuse me like heck? Do not despair!
Delegate is the answer.

`delegate` is a strange feature, a delegate is just a member of the struct that does stuff for it. The struct
just delegates the work to another object, in this case, work would mean &apos;methods&apos;. It would appear as
pointless or just an abbreviation, however it can be very useful and a interesting alternative to extends. The whole
delegate idea is in use in some other languages, just notice that Jass is not very dynamic and vJass does inherit a
lot of its flaws. For the better or the worse, delegation is a completely compile-time deal in the case of vJass.

A delegate does the struct&apos;s job, that is a very simple way to put it, a more complicated way would be, that
during compile, if jasshelper cannot find a certain requested member of method, it will begin to look up for that
member in the struct&apos;s delegates, if it finds this member in one of the delegates it will then compile it as a
call to the delegate&apos;s member instead.

```sql
//Array structs are hard to explain, but should be simple to understand with an example

struct A
    private real x
    private real y

    public method performAction takes nothing returns nothing
        call DestroyEffect( AddSpecialEffect("path\\model.mdl", this.x, this.y) )
    endmethod

endstruct

struct B
    delegate A deleg

    static method create takes nothing returns B
     local B b = B.allocate()
        set B.deleg = A.create()
    endmethod

endstruct

function testsomething takes nothing returns nothing
    local B myB = B.create()

    call myB.performAction()

    //Since performAction() is not a member of struct B, jasshelper will check out the
    //delegator, it does have a method called performAction, so it will just try to call
    //it, the result would be the same as:

    call myB.deleg.performAction()

endfunction
```

Some considerations:

* You can have multiple delegates, however that should probably be the exception rather than the rule.
* jasshelper gives priority to the struct's members before its delegates' this means that if both the struct and a
  delegate have the same member, jasshelper will always consider the struct's over the delegate's.
* Between delegates in the same struct, the priorities are the same as the declaration order.
* You can do a lot of quacky things like delegating to an array member, you will even be able to use `.size()` and `[]` on
  the struct in that case.
* You can also do non-sense as making a integer member a delegate, this will not cause a syntax error but does not
  really do much by itself, considering that integers have no members.
* Right now, you cannot make a delegate's method fulfill a interface's rules, for example if struct B was extending a
  certain interface that required a method called performAction, jasshelper would not recognize the delegate's method
  and will cause a syntax error, this might change in the future.
* You would usually have to initialize the delegate if you do not want bugs in your code.
* You can have a private delegate, it would only be accessible by outside code in cases where a member is necessary.
* If you try to call/use a delegate's private member, it will probably appear as a syntax error about not being able to
  find it in the struct rather than telling you that it is a private member of the delegate.
