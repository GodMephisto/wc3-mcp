---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Public-members.md"
ref: HEAD
---

# Public members

Public members are closely related to private members in that they do mostly the same, the difference is that public
members don&apos;t get their names randomized, and can be used outside the scope. For a variable/function declared as
public in an scope called `SCP` you can just use the declared function/variable name inside the scope, but to use it
outside of the scope you call it with an `SCP_preffix`.

An example should be easier to understand:

```sql
library cookiesystem
    public function ko takes nothing returns nothing
        call BJDebugMsg("a")
    endfunction

    function thisisnotpublicnorprivate takes nothing returns nothing
         call ko()
         call cookiesystem_ko() //cookiesystem_ preffix is optional
    endfunction
endlibrary

function outside takes nothing returns nothing
     call cookiesystem_ko() //cookiesystem_ preffix is required
endfunction
```

Public function members can be used by `ExecuteFunc` or real variable value events, but they always need the scope
prefix when used as string:

```sql
library cookiesystem
    public function ko takes nothing returns nothing
        call BJDebugMsg("a")
    endfunction

    function thisisnotpublicnorprivate takes nothing returns nothing
         call ExecuteFunc("cookiesystem_ko") //Needs the prefix no matter it is inside the scope

         call ExecuteFunc("ko") //This will most likely crash the game.
         call cookiesystem_ko() //Does not need the prefix but can use it.
         call ko() //since it doesn't need the prefix, the line works correctly.
    endfunction
endlibrary
```

Alternatively, you may use [SCOPE_PREFIX](SCOPE-PREFIX-and-SCOPE-PRIVATE.md).

> If you use public on a function called `InitTrig`, it is handled in an special way, instead of becoming
> `ScopeName_InitTrig` it will become `InitTrig_ScopeName`, so you could have an scope/library in a trigger with the same
> scope name and use this public function instead of manually making `InitTrig_Correctname`.
