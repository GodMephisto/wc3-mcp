---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/static-if.md"
ref: HEAD
---

# static if

A static if is similar to a normal if, except that it can only use constant booleans, and the `&&` and `!` logical
operators. Also, non-existant constant booleans can be used, and instead of giving a syntax error, the compiler will
assume they are false.

More importantly, static ifs are evaluated during compilation time, code is either added or not to the script depending
on what the static ifs evaluate.

Something to consider is that if a library called `libraryname` exists in the map, the compiler will add a constant
boolean called `LIBRARY_libraryname` and set it to true.

```C++
library OptionalCode requires optional UnitKiller
{
    constant boolean DO_KILL_LIB = true

    function fun()
    {
        unit u = GetTriggerUnit();
        //the following code may need to kill the unit
        //but is alternatively able to use the external
        //'UnitKiller' library to do the library.
        // ONLY when DO_KILL_LIB is true AND the
        // library UnitKiller is in the map.
        static if(DO_KILL_LIB && LIBRARY_UnitKiller )
        {
            //static if because if the UnitKiller
            // library was not in the map, using a normal
            // if would not remove this line of code and
            // therefore it would cause a syntax error.
            // (unable to find function UnitKiller)
            UnitKiller(u);
        }
        else
        {
            KillUnit(u);
        }
    }

}

library UnitKiller
{
    function UnitKiller(unit u)
    {
        BJDebugMsg("Unit kill!");
        KillUnit(u);
    }
}
```