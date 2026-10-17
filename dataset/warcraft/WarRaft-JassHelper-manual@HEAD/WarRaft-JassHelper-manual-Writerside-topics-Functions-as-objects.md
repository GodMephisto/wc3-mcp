---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Functions-as-objects.md"
ref: HEAD
---

# Functions as objects

For vJass functions may behave as objects with 2 methods: `evaluate` and `execute`, both methods got the same arguments
list as the function, and evaluate got its return value as well.

Using functions as objects has a couple of advantages,` evaluate()` allows you to call the function even from code that
is above its function declaration, execute allows the same but it is also able to run the function in another
thread.

The disadvantages are: Functions that are used with `evaluate()`, should not use `GetTriggeringTrigger()` (but you may
use any other event response) or any sync native, `evaluate()` does not support waits, and evaluate() is slower than a
normal function call.

`.execute()` is actually faster than good old `ExecuteFunc`, and in later versions it might actually get even faster.
evaluate halves the duration of ExecuteFunc and it may get much better later.

For functions that have function arguments you would have to use global variables to pass arguments when using
`ExecuteFunc`, `.execute` and `.evaluate` will pass the arguments directly.

```sql
function A takes real x returns real
    if(GetRandomInt(0,1)==0) then
        return B(x*0.02)
    endif
    return x
endfunction

function B takes real x returns real
    if(GetRandomInt(0,1)==1) then
        return A(x*1000.)
    endif
    return x
endfunction
```

These are mutually recursive functions, and with normal Jass this would give a syntax error, in order to prevent
this issue you may use evaluate:

```sql
function A takes real x returns real
    if(GetRandomInt(0,1)==0) then
        return B.evaluate(x*0.02)
    endif
    return x
endfunction

function B takes real x returns real
    if(GetRandomInt(0,1)==1) then
        return A(x*1000.)
    endif
    return x
endfunction
```

Let us say you need to destroy an special effect after waiting x seconds using a wait. You could use a timer but for
example sake we are going to use a normal wait, we do not want to stop execution of the function calling this effect
destroying function so we need a new thread:

```sql
function DestroyEffectAfter takes effect fx, real t returns nothing
    call TriggerSleepAction(t)
    call DestroyEffect(fx)
endfunction

function test takes nothing returns nothing
    local unit u=GetTriggerUnit()
    local effect f=AddSpecialEffectTarget("Abilities\\Spells\\Undead\\Cripple\\CrippleTarget.mdl",u,"chest")
    
    call DestroyEffectAfter.execute(f,3.0)
    
    set u=null
    set f=null
endfunction
```

> This feature is currently limited to functions declared in the map script, you cannot use it with
common.j natives or blizzard.j functions yet.

The `.name` member in functions will return a string containing the function&apos;s compiled name, useful when you want
to use a scope function in things like `ExecuteFunc`.

```sql
scope test
    public function xxx takes nothing returns nothing
        call BJDebugMsg(xxx.name) //will show "test_xxx"
    endfunction
endscope
```