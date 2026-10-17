---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Static-methods.md"
ref: HEAD
---

# Static methods

Static methods or class methods do not use an instance, they are actually like functions but since they are declared
inside the struct they can use private members.

```sql
 struct encap
    real a=0.0
    private real b=0.0
    public real c=4.5

    private method dosomething takes nothing returns nothing
        if (this.a==5) then
            set this.a=56
        endif
    endmethod

    static method altcreate takes real a, real b, real c returns encap
        local encap r=encap.create()
        set r.a=a
        set r.b=b
        set r.c=c
        call r.dosomething() //even though it is private you can use
                             //it since we are inside the struct declaration
        return r
    endmethod

    method randomize takes nothing returns nothing
        // All legal:
        set this.a= GetRandomReal(0,45.0)
        set this.b= GetRandomReal(0,45.0)
        set this.c= GetRandomReal(0,45.0)
    endmethod

endstruct

function test takes nothing returns nothing
    local encap e=encap.altcreate(5,12.4,78.0)
    call BJDebugMsg(R2S(e.a)+" , "+R2S(e.c))
endfunction
```

You might notice that the usual `create()` syntax works like an static method, and `destroy()` can work as static or
instance method

You can override the static method create by declaring your own one, once you do it, you might require another method
just to allocate a unique id for the struct, this is the `allocate()` static method which is added by default to all
structs, it is a private method. When a struct does not have an specific create method declared, jasshelper will use
allocate directly when .create is called.

Since [](Changelog.md#0.9.Z.1) you may also override the destroy method by declaring your own one. Then use deallocate
to call the normal destroy method.

```sql
struct vec
    real x
    real y
    real z
    
    // static method create must return a value of the struct's type
    // create may have arguments.
    static method create takes real ax, real ay, real az returns vec
        local vec r= vec.allocate() //allocate() is private and
                                    //it gets a unique id for the struct
        set r.x=ax
        set r.y=ay
        set r.z=az
    
        return r
    endmethod

endstruct

function test takes nothing returns nothing
    local vec v= vec.create(1.0 , 0.0 , -1.0 )

    call BJDebugMsg( R2S(v.z) )

    call v.destroy()
endfunction
```

Static methods that take nothing can also be used as code values

```sql
struct something
    static method bb takes nothing returns nothing
        call BJDebugMsg("!!")
    endmethod
endstruct

function atest takes nothing returns nothing
    local trigger t=CreateTrigger()
    call TriggerAddAction(t, function something.bb)
    call TriggerExecute(t)
endfunction
```