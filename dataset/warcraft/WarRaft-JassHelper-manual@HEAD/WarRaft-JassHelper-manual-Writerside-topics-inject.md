---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/inject.md"
ref: HEAD
---

# inject

Certain advanced users might use the world editor yet prefer to have more control over the map script, namely making
their own main or config functions, the inject preprocessors allows to replace such functions.

The syntax is: `//! inject main/config (...) //! endinject`

For example:

```sql
//! inject main
   //some function calls may go here

   // this places vjass initializations there, notice structs are first initialized then library initializers
   // are called
   //! dovjassinit


   //other calls may go here

   call InitCustomTriggers() //maybe you want to exploit that world editor function...
//! endinject
```

The `dovjassinit` preprocessor may prove very helpful, it is only necessary if there is no call to `InitBlizzard` in the
custom main or if you need to control the position of such initializing of structs and libraries.

`//! inject` config works the same way only that there is no `//! dovjassinit` for that case.
