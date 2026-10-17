---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/hook.md"
ref: HEAD
---

# hook

There are functions that are outside of our control most of the times, like natives and those in blizzard.j. The
`hook` keyword allows us to detect them.

Use hook, the name of the native/bj function and the name of a function or static method and you will be able to
detect when that native is called and also capture the arguments given to it.

```sql
function onRemoval takes unit u returns nothing
    call BJDebugMsg("unit is being removed!")
endfunction

struct err
    static method onrem takes unit u returns nothing 
       call BJDebugMsg("This also knows that a unit is being removed!")
    endmethod
endstruct

hook RemoveUnit onRemoval
hook RemoveUnit err.onrem //works as well
```

Try the code in some map, in which RemoveUnit is called sometimes and see what happens next time it is called.

There are some limitations for now, if the native/bj function is called by another bj function, the hook does not
work when that other bj function gets called.
