---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/SCOPE-PREFIX-and-SCOPE-PRIVATE.md"
ref: HEAD
---

# SCOPE_PREFIX and SCOPE_PRIVATE

Whenever you are inside an scope/library declaration, `SCOPE_PREFIX` and `SCOPE_PRIVATE` are enabled string constants
that
you could use.

`SCOPE_PREFIX` will return the name (as a Jass string) of the current scope concatenated with an underscode. (The prefix
added for public memebers)

`SCOPE_PRIVATE` will return the name (as a Jass string) of the current prefix for private members.

```sql
scope test

    private function kol takes nothing returns nothing
        call BJDebugMsg("...")
    endfunction

    function lala takes nothing returns nothing
         call ExecuteFunc(SCOPE_PRIVATE+"kol")
    endfunction

endscope
```

In the example, we are allowing `lala()` to call the private function `kol` via `ExecuteFunc`.