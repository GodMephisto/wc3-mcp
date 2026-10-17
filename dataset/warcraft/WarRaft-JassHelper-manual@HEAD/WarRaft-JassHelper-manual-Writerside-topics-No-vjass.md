---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/No-vjass.md"
ref: HEAD
---

# No vjass!

The `//! novjass` and `//! endnovjass` preprocessor directives allow you to make vjass compilers (like jasshelper)
completely ignore the code between them.

```sql
function VerifyVJass takes nothing returns nothing
  local boolean b=true
    //! novjass
        set b=false
    //! endnovjass
    if(b) then
        call BJDebugMsg("You got vJass")
    else
        call BJDebugMsg("Where's vJass?")
    endif
 endfunction
```

If that code is parsed by a vJass compiler, it will remove what is inside the `//! novjass` blocks. If the function is
just saved in normal World Editor, it will just ignore the `//! novjass` tags (since it will think they are comments) so
it will still consider their contents.