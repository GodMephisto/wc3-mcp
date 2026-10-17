---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/for.md"
ref: HEAD
---

# for

Another looping construct, but this one is specialized for iterations. e.g. That previous example is better as a for.
The `for` control structure has syntax `for ( range ) { statements }` . It will repeat the statements based on
the range sounds confusing.

Let&apos;s say you want a variable i to go from 0 to n-1 , this means that you want to repeat the statements for i such
that (i >= 0) and (i<n) , another way to write this is : ( 0 &lt;= i &lt; n) which is how the for construct works.
Notice that it can only work with a normal variable (do not use arrays or struct members as the loop variable).

```C++
 library WhileTest
{
    function onInit()
    {
        integer x, n=7;
        //numbers from 0 to 9,
        // ascending order
        for ( 0 <= x < 10)
        {
            BJDebugMsg( I2S(x) );
        }

        //numbers from 9 to 0,
        // descending order
        for ( 10 > x >= 0)
        {
            BJDebugMsg( I2S(x) );
        }

        //numbers from 1 to 10,
        // ascending order:

        for( 1 <= x <= 10 )
        {
            BJDebugMsg( I2S(x) );
            //decrease x
            x = x - 1;
        }

        //numbers from 0 to n-1,
        // ascending number
        for ( 0 <= x < n )
        {
            BJDebugMsg( I2S(x) );
        }
    }

}
```

`for` also has an alternate, more C-like syntax: `for( assignment1; condition; assignment2) { statements }`. Allowed
assignment operations are `=`, `+=`, `/=`, `*=` and `-=`. The condition is a boolean expression which must be true,
otherwise the
for execution is stopped. Assignment1 is called at the beginning of the loop. assignment2 is done just after each
iteration...