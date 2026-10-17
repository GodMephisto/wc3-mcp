---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Function-pointers-interfaces.md"
ref: HEAD
---

# Function pointers (interfaces)

This are declared very differently from the vJass ones.

```C++
library Test
{
    //a function type with a single unit argument
    type unitFunc extends function(unit);

    //a function type with two integer arguments that returns boolean
    type evFunction extends function(integer,integer) -> boolean;

    function TortureUnit(unit u)
    {
        BJDebugMsg(GetUnitName(u)+" is being tortured!");
        KillUnit(u);
    }

    function HealUnit(unit u)
    {
        SetWidgetLife(u, 1000);
    }

    /* This function calls F(u) if the unit is an ally or G(u) if the unit is an enemy
     the example sucks as it is much slower than an if-else but should be good
     to explain it... */
    function AllyEnemyFunctionPicker(player p, unit u, unitFunc F, unitFunc G)
    {
        if (IsUnitAlly(u, p) ) {
            F.evaluate(u);
        } else {
            G.evaluate(u);
        }

    }

    function test(unit u)
    {
        // We are using the functions as arguments here...
        AllyEnemyFunctionPicker( Player(0), u,  TortureUnit, HealUnit );

        // will torture the unit if it is an ally, or heal it otherwise.
    }

}
```