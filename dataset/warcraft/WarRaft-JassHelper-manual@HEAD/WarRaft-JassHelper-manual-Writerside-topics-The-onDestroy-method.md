---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/The-onDestroy-method.md"
ref: HEAD
---

# The onDestroy method

There is no actual syntax for destructors, but there is a rule and it is that if the struct has a method called
`onDestroy`, it is always automatically called when `.destroy()` is issued on an instance.

It is useful to have onDestroy when an instance of the type may hold things that have to be correctly cleaned, it
saves time and even makes things safer.

```sql
struct sta
    real a
    real b
endstruct

struct stb
    sta H=0
    sta K=0

    method onDestroy takes nothing returns nothing
        if (H!=0) then
            call sta.destroy(H)
        endif
        if (K!=0) then
            call sta.destroy(K)
        endif
    endmethod
endstruct
```

In the above example, it is only needed to destroy the object of type `stb` and it would automatically destroy the
attached objects of type sta if present.