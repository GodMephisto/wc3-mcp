---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Zinc-structs.md"
ref: HEAD
---

# structs

structs, well, it is a long story, and I am in a rush to release Zinc, let us say that they are very similar to vJass
structs, except with this syntax. To declare members, use the same syntax we&apos;ve been using for globals and locals,
except there is also a `static` keyword to consider. Do notice that unlike libraries, struct members are public by
default.

There are some differences regarding struct storage limit and array structs, mostly a side effect of killing the array
keyword:

There are many things to say, and little time, so I'll just post many examples:

```C++
library WhileTest
{
    //a struct
    struct A
    {
        integer x,y,z;

    }

    //a interface
    interface B
    {
        method move();
    }

    //a struct with 20000 storage space:
    struct[20000] C
    {
        real a,b,c;
        string s,t;
    }

    //a interface with 30000 storage space:
    interface[30000] D
    {
        integer meh;
        method transform(integer a) -> integer;
    }

    //an "array struct"
    struct myArr[]
    {
        static constant integer meh = 30004;
        integer a;
        integer b;
    }

    //an array struct of size 10000:
    struct myErr[10000]
    {
        integer x,y,z;
    }

    interface F
    {
        method balls(integer x) = null; //this method 'defaults nothing'
        method bells(integer y) -> real = 0.0 ; //this method defaults a 0.0 for the real return

        //a static create method for the interface
        static method create();
    }

    struct G
    {
        static method onCast()
        {
            KillUnit( GetTriggerUnit() );
        }

        static method onInit() {
            //a static method, onInit too.
            trigger t= CreateTrigger();

            //unlike vJass in which it uses function G.onCast.
            TriggerAddAction(t, static method G.onCast);
        }
        integer x;

        //operator overloading:

        method operator< (G other) -> boolean
        {
            return (this.x < other.x)
        }

        method operator[](integer x) -> real
        {
            return x*2.5;
        }

        method operator readOnly() -> integer
        {
            return 0;
        }
    }

    module H
    {
        method kill() { BJDebugMsg("Kill it"); }
    }

    struct K
    {
        module H; //implements module H
        optional module XX; //optionally-implements module XX


        delegate G myg; //a delegate
    }

}
```