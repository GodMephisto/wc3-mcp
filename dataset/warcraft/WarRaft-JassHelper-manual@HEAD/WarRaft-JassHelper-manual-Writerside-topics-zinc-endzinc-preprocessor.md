---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/zinc-endzinc-preprocessor.md"
ref: HEAD
---

# zinc..endzinc preprocessor

Zinc and vJass code must be kept separate from each other. One way is using a vJass preprocessor command to tell it
where zinc code begins and ends. I do not prefer this method over zinc import, but it should be good for people that
can&apos;t adapt to working outside of WE (and therefore require newgen pack).

It is easy, if you wish to code in Zinc, you must first use a `//! zinc` preprocessor. However, you must also specify
where the code ends, and thus add a `//! endzinc` preprocessor.

```C++
//! zinc

library HelloWorld
{
    function onInit()
    {
         BJDebugMsg("Hello World");
    }
}

//! endzinc
```

I assume you got newgen pack installed (and jasshelper [](Changelog.md#0.9.Z.0) or greater), try to put this code in
some &quot;trigger&quot;, then test the map and see your first Zinc program at work.