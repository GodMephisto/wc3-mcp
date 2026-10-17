---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Global-declaration-Freedom.md"
ref: HEAD
---

# Global declaration Freedom

Warcraft III world editor always made everything harder for us, inluding declaring globals, you
needed to use that dialog and it was really difficult to recreate global variables from a map or
another and you were forced to use GUI for that. It was also impossible to declare globals of some types
without modding world editor and then make your map completelly unopenable by normal world editor.

Global declaration freedom simply allows you to write globals blocks wherever you want, for example
in the custom script section or in a 'trigger', and because of this you can even use the constant
prefix which can even make finalizers able to inline constants and stuff.

The JassHelper preprocessor will simply merge all the global blocks found around the map script and move
them to the top of the map, all merged in a single globals block.

```sql
function something takes nothing returns nothing
    set somearray[SOMETHING_INDEX]=4
endfunction

globals
    constant integer SOMETHING_INDEX = 45
    integer array somearray
endglobals
```

Will now work without any error, there is one limitation though, you can't use functions or
non-constant values in the default value, for example you can make a global start on `null`, `1` ,
`19923` , `0xFFF`, `true`, `false`, `"Hi"` , etc. But you can't make them initialize with any function call
or with a native (although it is possible to use a native, most natives tend to crash the thread
when used in globals declaration). You can't also assign to another global variable, because there
isn't really a way to control to what position of the map would a global declaration go.

> Please try to keep a good global naming method, some programs use pure caps for global names,
> Jass&apos; standard set by [common.j](https://github.com/WarRaft/war3mpq/blob/main/extract/Scripts/common.j) and
[blizzard.j](https://github.com/WarRaft/war3mpq/blob/main/extract/Scripts/Blizzard.j) is that constants should be
> uppercase, you can then
> use system_variable for a good variable name, although you can stick to the `udg_` preffix as well.