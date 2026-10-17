---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/if.md"
ref: HEAD
---

# if

The syntax is : `if (condition) { statements1 } else { statements2 }` . If the condition is true, it will execute
`statements1`, else it will execute `statements2`. Notice that the `{ }` sorrounding the statements are only necessary
if the
statements are more than one. The else part is optional.

```C++
library IfTest
{
    integer x = 0;

    function onInit()
    {
        if(x==0)
        {
            BJDebugMsg("it is 0");
        }
        else
        {
            BJDebugMsg("it is not 0");
        }

        x = GetRandomInt(0,3);
        
        //shorter way (take advantage that the blocks are one liners
        // (does the same as above)
        if(x==0)  BJDebugMsg("it is 0");
        else      BJDebugMsg("it is not 0");

        x = GetRandomInt(0,6);
        if(x==5)
        {
            BJDebugMsg("Today is your lucky day, because you got a 5");
        }
        else if (x==0)
        {
            //There is no such thing as an elseif in Zinc, but as you
            // can see, since the only thing inside this else block is
            // an if statement, it can work the same.
            BJDebugMsg("Today is your unlucky day,")
            BJDebugMsg(" you got a 0");
        }
        else
        {
            BJDebugMsg("Normal day");
            //ifs can be nested.
            if( x==4) {
                BJDebugMsg("But hey, at least it is a 4, that is good");
            }
        }

    }

}
```

The condition is expected to be of a boolean type, the boolean type can either be true or false. e.g: Comparisons like
`==` return `true` or `false`.