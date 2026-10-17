---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Globals-of-struct-types.md"
ref: HEAD
---

# Globals of struct types

You can have globals of struct types. Because of Jass&apos; limitations you cannot initialize them directly.

```sql
globals
     pair globalpair=0 //legal
     pair globalpair2= pair.create() //not legal
endglobals
```

You would have to assign them in an init function instead.