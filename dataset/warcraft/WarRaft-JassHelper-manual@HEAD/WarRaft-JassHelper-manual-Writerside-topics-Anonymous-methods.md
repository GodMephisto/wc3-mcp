---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Anonymous-methods.md"
ref: HEAD
---

# Anonymous methods

Well, just like anonymous functions, except you cannot use them outside structs/modules. An anonymous method will be
converted to a method so it implicitly takes this in the arguments list.

Anonymous methods are always converted to a function pointer. You can call the struct&apos;s static and non-static
members from anonymous methods.

```C++
library Test
{
    type IntFunc extends function(integer);
    struct abuse
    {
        IntFunc fn;
        string msg;

        method cantThinkOfAName()
        {
            fn = method()
            {
                BJDebugMsg(msg);
            }

            fn.evaluate(this);
        }
      
    }

}
```

Anonymous static methods
: You may prefix anonymous methods with a static keyword, but that would just be an anonymous function in disguise.