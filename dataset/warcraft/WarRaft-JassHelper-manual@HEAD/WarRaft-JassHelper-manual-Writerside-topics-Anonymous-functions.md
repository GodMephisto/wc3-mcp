---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Anonymous-functions.md"
ref: HEAD
---

# Anonymous functions

Many natives, BJ functions and also functions made by the community, ask for you to use a function as an argument to
pass to do some actions or conditions. Which is cool and all.

Anonymous functions are just functions that have no name. They are also &quot;expressions&quot; so you use them in a
place where an expression is expected, which basically means you declare these functions inside another function... They
get converted into either a Jass **code** or a function pointer (see above) depending on their number of
arguments (it becomes a code value if and only if it has zero arguments). Zinc will give it a name and place the
function in an appropriate place for you.

Do notice that anonymous functions cannot use their parent&apos;s local variables (it will possibly cause a PJass
error). They will be able to receive a read-only version of these variables, but that comes later. For now they are
meant for quicker syntax and better structure (refer to the CountUnitsInGroup example, sometimes giving names to actions
just overcomplicates the thing).

```C++
library Test
{
    function test1()
    {
        //Also refer to the CountUnitsInGroup example ...
        TimerStart(CreateTimer(), 5.0, false, function() {
             BJDebugMsg("5 seconds since you called test1");
        });
    }


    type unitFunc extends function(unit);
    function AllyEnemyFunctionPicker(player p, unit u, unitFunc F, unitFunc G)
    {
        if (IsUnitAlly(u, p) ) {
            F.evaluate(u);
        } else {
            G.evaluate(u);
        }

    }
    /* those things up there are just the function pointers example */

    function test2(unit u)
    {
        unitFunc F,  G;

        F = function(unit u) {
            BJDebugMsg("We are torturing "+GetUnitName(u)+" again");
            KillUnit(u);
        };

        G = function(unit u) { SetWidgetLife(u, 100); };
        
        AllyEnemyFunctionPicker( Player(0), u, F, G);
        // will torture the unit if it is an ally, or heal it otherwise.
        // notice this does the same as the function pointers example.
    }

}
```