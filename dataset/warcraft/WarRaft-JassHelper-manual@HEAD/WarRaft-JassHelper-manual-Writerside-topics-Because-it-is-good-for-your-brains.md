---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Because-it-is-good-for-your-brains.md"
ref: HEAD
---

# Because it is good for your brains

Zinc is good for your brain&apos;s development. Likewise, the Zinc scripting language is good for ...your code
development. So let&apos;s see some examples of code...

## Hello world

```C++
library HelloWorld
{
    function onInit()
    {
         BJDebugMsg("Hello World");
    }
}
```

For better or worse, Zinc is completely library-based. So we begin by declaring a Hello World library. Unlike vJass,
there is no need to explicitly declare an initializer function name, and the function called onInit will be used
directly (if it exists), The function has no arguments, and no return value. It simply calls a _jass function_
`BJDebugMsg` to do the text display.

## 99 bottles of beer

```C++
library Bottles99
{
    /* 99 Bottles of beer sample,
       prints the lyrics at: http://99-bottles-of-beer.net/lyrics.html
    */
    function onInit()
    {
        string bot = "99 bottles";
        integer i=99;
        while (i>=0)
        {
            BJDebugMsg(bot+" of beer on the wall, "+bot+" of beer");
            i=i-1;
            if      (i== 1) bot = "1 bottle";
            else if (i== 0) bot = "No more bottles";
            else            bot = I2S(i)+" bottles";
            //Lazyness = "No more" is always capitalized.

            if(i>=0)
            {
                BJDebugMsg("Take one down and pass it around, "+bot+" of beer on the wall.\n");
            }
        }
        BJDebugMsg("Go to the store and buy some more, 99 bottles of beer on the wall.");
    }
}
```

This sample illustrates yet more syntax elements, you can see the while and if constructs, also how `{}` is actually
optional when it is a single statement. Also some comments. There is no `elseif` construct in Zinc, because as you
can see, nesting an if inside an else is just as easy.

## Factorial

```C++
library Factorial
{
    /*
    *   Calculates the factorial of a number
    *
    */
    public function Factorial(integer n) -> integer
    {
        integer result=1, i;
        for ( 1 <= i <= n)
            result = result * i;

        return result;
    }
}
```

This simple code is a function that calculates the factorial of a number n, that is `1*2*3*...n` . Notice the for loop
construct. It basically means that the code should be repeated for all numbers i between 1 and n. Also notice that two
integer variables are declared in the same line. We probably wish to call this function in other libraries, so we
declare the function as public.

## Instant Kill

```C++
 library InstantKill
{
    /*
    *   INSTANT K1LL !
    *   This custom spell Kills the target unit!
    *
    */
    constant integer SPELL_ID = 'A001';

    function onSpellCast()
    {
        KillUnit(  GetSpellTargetUnit() );
    }

    function spellIdMatch() -> boolean
    {
        return (GetSpellAbilityId() == SPELL_ID);
    }

    function onInit()
    {
        trigger t = CreateTrigger();
        TriggerAddAction(t, function onSpellCast);
        TriggerAddCondition(t, Condition(function spellIdMatch) );
    }
}
```

This is a quick custom spell that will kill the target unit instantly. You may notice the syntax for return values and
also that constant global declaration not inside a globals block.

## AddSpecialEffectTimed

```C++
library TimedEffect requires TimerUtils
{
    /*
    *   Adds a special effect to the ground, destroys it after
    *
    */
    struct data
    {
        effect fx;
    }
    function destroyEffect()
    {
        timer t=GetExpiredTimer();
        data dt = data( GetTimerData(t));
        DestroyEffect(dt.fx);
        ReleaseTimer(t);
    }

    public function AddSpecialEffectTimed( string fxpath, real x, real y, real duration)
    {
        timer t=NewTimer();
        data  dt = data.create();
        dt.fx = AddSpecialEffect(fxpath, x, y);

        SetTimerData(t, integer(dt));
        TimerStart(t, duration , false, function destroyEffect);
    }
}
```

This is a library that comes with the AddSpecialEffectTimed, just an example. TimerUtils is a vJass library, but that is
not a problem for Zinc. Do notice that all members of a library are private by default, which means that the data struct
and the destroyEffect function are private. Public library members also work differently in Zinc than in vJass, it
will **not** add a prefix to the name, therefore the way to call this function is `AddSpecialEffectTimed`, not
TimedEffect_AddSpecialEffectTimed.

As you can see, things like type casts work the same as in vJass. There is also a struct which follows the same
declaration rules as local variables and global variables.

## CountUnitsInGroup

```C++
library CountUnitsInGroup
{
    integer count;
    public function CountUnitsInGroup( group g ) -> integer
    {
        count = 0;
        ForGroup(g, function() { count= count + 1; });
        return count;
    }
}
```

This example replicates a blizzard.j function, as an excersise I invite you to check blizzard&apos;s implementation of
it in Jass. This example features an `anonymous function` which are simply a way to allow you to
declare functions on the go in cases like this one (very frequent in Jass) where the `count=count+1` action is not
really part of another function&apos;s logic but something inherent to the `CountUnitsOne`.
