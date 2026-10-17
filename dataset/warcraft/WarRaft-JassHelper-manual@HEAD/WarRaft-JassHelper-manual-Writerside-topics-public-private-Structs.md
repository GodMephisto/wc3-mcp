---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/public-private-Structs.md"
ref: HEAD
---

# public/private Structs

You can declare scope&apos;s public or private structs and struct variables with all freedom

```sql
scope cool
    public struct a
        integer x
    endstruct

    globals
        a x
        public a b
    endglobals


    public function test takes nothing returns nothing
        set b = a.create()
        set b.x = 3
        call b.destroy()
    endfunction

endscope

function test takes nothing returns nothing
    local cool_a x=cool_a.create()
    set a.x=6
    call a.destroy()
endfunction
```