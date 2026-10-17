---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Globals.md"
ref: HEAD
---

# Globals

Global variables, are those things that hold a state and any function can use. If these variables are public, every
function outside the library will also be able to use. There is also the aspect of global constants, which behave
exactly like global variables except they are read-only.

They are declared inside a library, and the syntax is: ( `(constant) type varname ( = initialvalue)` ). Adding the
constant prefix makes the global a constant (which means it is read-only, in that case the initial value is mandatory,
else it is optional. Another variation is when you want arrays. You can use `name[]` for an array with default size (
8191), `name[SIZE]` for an array with another size value (which may be bigger, but if it is bigger than 8191, it will
need
function calls to be read), `name[SIZE1][SIZE2]` for a 2D array, the total storage needed for this array is
`SIZE1*SIZE2`.
Arrays cannot be assigned to an initial value.

You may also separate globals of the same type using a comma.

```C++
library PublicTest
{
    //the following are private constants:
    constant integer SPELL_ID = 'A000';
    constant real    HEIGHT = 10.0;
    constant integer UNIT_COUNT = 7, ITEM_COUNT = UNIT_COUNT*6;
    
    // A, B, C, ... E are global variables of integer type,
    // they are public
    public integer A;
    public integer B = 1;
    public integer C,D = 1,E;
    
    // B and D begin assigned to 1
    real X[], Y[16000];
    real Z[200][200];
    //X: real array.
    //Y: real array of size 16000
    //Z: real 2D array 200x200.
    // They are private
    
    function onInit()
    {
        A = B;
        X[0] = A;
        X[1] = B+1;
        E = D;
        D = 7;
        C = E*3;
        Y[10000] = D;
        Z[4][4] = 10;
    }
}
```
