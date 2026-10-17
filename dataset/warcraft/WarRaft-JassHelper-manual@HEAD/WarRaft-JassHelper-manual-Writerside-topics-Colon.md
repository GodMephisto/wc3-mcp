---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Colon.md"
ref: HEAD
---

# Colon

This is a new operator that basically allows you to use `[]` differently, call it a reverse `[]`. Sometimes the logic of
a script requires the order to be different in order to make more sense.

```sql
function test takes nothing returns nothing
    local integer a=3
    local integer array X

    set X[a]=10 //both of these statements do the same
    set a:X =10

    set X[a] = X[a] + 10 //The same.
    set a:X = a:X +10

    set X[3]=1000
    set 3:X =1000 //this is invalid syntax, sorry, only use : on variables and stuff like that.

endfunction
```