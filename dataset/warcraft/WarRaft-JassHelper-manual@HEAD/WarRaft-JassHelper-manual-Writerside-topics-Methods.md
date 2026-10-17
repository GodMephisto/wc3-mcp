---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Methods.md"
ref: HEAD
---

# Methods

Methods are just like functions, the difference is that they are associated with the class, also `[normal]` methods are
associated with an instance (in this case `this`)

Once again an example is needed

```sql
struct point
    real x=0.0
    real y=0.0

    method move takes real tx, real ty returns nothing
        set this.x=tx
        set this.y=ty
    endmethod

endstruct

function testpoint takes nothing returns nothing
    local point p=point.create()
    call p.move(56,89)

    call BJDebugMsg(R2S(p.x))
endfunction
```

<deflist>
    <def title="this">

A keyword that denotes a pointer to current instance, `[normal]` methods are instance methods which means
that they are called from an already assigned variable/value of that type, and this will point to it. In the above
example we use this inside the method to assign `x` and `y`, when calling `p.move()` it ends up modiffying x and y
attributes for the struct pointed by `p`.
</def>
    <def title="method syntax">You might notice that method syntax is really similar to function syntax.</def>
    <def title="this is optional">

You can use a single `.` instead of `this.` when you are inside an instance method. (For
example `set .member = value`)
</def>
</deflist>

Methods are different to normal functions in that they can be called from any place (except global declarations) and
that you are not necessarily able to use waits, sync natives or `GetTriggeringTrigger()` inside them (but you may use
any other event response), you might be able to use them but it depends on various factors, it is not recommended to
use them at all. In next versions the compiler might even raise a syntax error when it finds them.