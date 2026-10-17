---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/while.md"
ref: HEAD
---

# while

Conditional constructs are one thing, we also need looping structs. The `while` statement has a syntax:
`while (condition) {statements}`. It will repeat the statements as long as the condition is true.

The following code will print the numbers from 5 to 1 in decreasing order:

```C++
library WhileTest
{
    function onInit()
    {
        integer x = 5;
        while( x > 0)
        {
            BJDebugMsg( I2S(x) );
            //decrease x
            x = x - 1;
        }
    }
}
```