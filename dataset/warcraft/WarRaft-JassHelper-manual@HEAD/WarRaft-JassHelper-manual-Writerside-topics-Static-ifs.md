---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Static-ifs.md"
ref: HEAD
---

# Static ifs

static ifs are like normal ifs, except that a) the condition must contain only constant booleans, the `and`
operator and the `not` operator and b) They are evaluated during compile time. Which means that the code that is
not matched to its condition is simply ignored.

```sql
library OptionalCode requires optional UnitKiller

    globals
        constant boolean DO_KILL_LIB = true
    endglobals

    function fun takes nothing returns nothing
        local unit u = GetTriggerUnit()
        //the following code may need to kill the unit
        //but is alternatively able to use the external
        //'UnitKiller' library to do the library.
        // ONLY when DO_KILL_LIB is true AND the
        // library UnitKiller is in the map.
        static if DO_KILL_LIB and LIBRARY_UnitKiller then
            //static if because if the UnitKiller
            // library was not in the map, using a normal
            // if would not remove this line of code and
            // therefore it would cause a syntax error.
            // (unable to find function UnitKiller)
            call UnitKiller(u)
        else
            call KillUnit(u)
        endif
    endfunction

endlibrary

library UnitKiller

    function UnitKiller(unit u)
        call BJDebugMsg("Unit kill!")
        call KillUnit(u)
    endfunction

endfunction
```