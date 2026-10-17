---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Debug-preprocessor.md"
ref: HEAD
---

# Debug preprocessor

Jass includes a debug keyword which compiles correctly but makes the rest of the code be
ignored. It seemed this keyword was used for debugging purposes like a switch in a debug build
of warcraft III that enabled those calls.

We can now take advantage of this hidden Jass feature. Saving a map in debug mode using
JassHelper will simply remove the debug keyword so the calls are enabled, if debug mode is not
enabled, JassHelper will remove the lines that begin with debug.

```sql
function Something takes integer a returns nothing
   debug call BJDebugMsg("a is "+I2S(a))
   call KillNUnits(a)
endfunction
```

If we use this function in a map saved with debug mode, we will see &quot;a is value&quot; each time this
function is called, otherwise it will only call `KillNUnits` silently.

You may also use the constant boolean `DEBUG_MODE` which is set to true or false depending on whether the debug mode is enabled.
