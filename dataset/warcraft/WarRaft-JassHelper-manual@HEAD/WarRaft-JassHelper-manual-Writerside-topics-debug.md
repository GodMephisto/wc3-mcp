---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/debug.md"
ref: HEAD
---

# debug

Another control structure, its syntax is `debug { statements }`. It will only add those statements when
jasshelper&apos;s debug mode is enabled.

```C++
library Test
{
    function onInit()
    {
        debug
        {
            BJDebugMsg("Debug mode is enabled");
        }
    }
}
```