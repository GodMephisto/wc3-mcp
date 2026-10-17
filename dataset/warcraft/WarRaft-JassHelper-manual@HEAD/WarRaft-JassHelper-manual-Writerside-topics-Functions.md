---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Functions.md"
ref: HEAD
---

# Functions

A function is a piece of code you can call, it may take some arguments and also have a return value. The syntax is : (
`function name ([arguments]) (-> returntype { contents }`)

The arguments list is a comma separated list containing an item per argument, each argument is represented as the
argument&apos;s type followed by its name.

If the function does not have a return type, then the -> is not included. If the function has a return type, then you
must include a `->` and the return type towards the end.

The contents of the function is a series of Zinc statements. It may include local variables, return statements,
if-then-else, while, for, breaks, assignments, function calls and method calls.

To call a function, simply use `functionname(argumentslist)`. The argument list when called is similar to the one when
functions are declared, it is a comma separated list of expressions, one for each argument. If the function has a return
value, you can use the function call in place of a value.

Notice that calling a function is... complicated. It requires the function to get declared in a required library or in
the same library in a line above the line from which you are calling. Else there are alternatives, like the
`.evaluate()`
and `.execute()` methods which may get into detail later if I have time (they are act like in vJass')

There are Jass functions that come either from common.j or from blizzard.j that you may use following these rules. You
may also use functions from vJass libraries that are required by yours.

```C++
library FunctionTest
{
    integer x = 0;
    function increaseX()
    {
        // a lame function that merely increases the
        // value of the x global
        x = x + 1;
    }

    function sum3(integer a, integer b, integer c) -> integer
    {
       //returns the sum of 3 numbers
       return a+b+c;
    }

    function onInit()
    {
        increaseX();
        x = sum3( x, x, 7);
        increaseX();
        /* BJDebugMsg = blizzard.j function  and I2S = wc3 native  */
        BJDebugMsg( I2S(x) );

        //should show a message containing "10" in game.
    }

}
```