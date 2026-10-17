---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Libraries-and-Scopes.md"
ref: HEAD
---

# Libraries and Scopes

Yet another issue with World editor and Jass was that it is impossible to control the order of
triggers in the map's script when saved. The custom script partially solved the problem but it is
really problematic to ask users to paste things there, it is also anti-modular programming to keep
it full of unrelated stuff.

The library preprocessor allows you to keep your top functions in the top and being able to
control where each one goes. It also has an smart requirement support so it will sort the function
packs for you.

The syntax is simple, you use `library LIBRARYNAME`
or `library LIBRARYNAME requires ONEREQUIREMENT` or `library LIBRARYNAME requires REQ1, REQ2 ...`

Remember to mark the end of a library using the `endlibrary` keyword.

```sql
library B
    function Bfun takes nothing returns nothing
    endfunction
endlibrary

library A
    function Afun takes nothing returns nothing
    endfunction
endlibrary
```

If JassHelper finds this command, it will make sure to move the `Afun` and `Bfun` functions to the top of the map&apos;s
script, so the rest of the map&apos;s script can freely call `Afun()` or `Bfun()`.

Notice that it would be uncertain to know what would happen if `Afun()` was called from a
function inside the `B` library. The command is to move libraries to the top, but we wouldn't
know if `B` went before or after `A`.

If a function inside library `B` needed to call a function inside library `A` we should let
JassHelper know that `A` must be added before `B`. That's the reason the `requires` keyword exists:

```sql
library C requires A, B, D
    function Cfun takes nothing returns nothing
        call Afun()
        call Bfun()
        call Dfun()
    endfunction
endlibrary

library D
    function Dfun takes nothing returns nothing
    endfunction
endlibrary

library B requires A
    function Bfun takes nothing returns nothing
        call Afun()
    endfunction
endlibrary

library A
    function Afun takes nothing returns nothing
    endfunction
endlibrary
```

> For senseless reasons: `requires`, `needs` and `uses` all work correctly and have the same function in
> the library syntax, but please use `requires`, the other ones may be gone one day...

The result in the top of the map would be:

```sql
function Afun takes nothing returns nothing
endfunction

function Dfun takes nothing returns nothing
endfunction

function Bfun takes nothing returns nothing
    call Afun()
endfunction

function Cfun takes nothing returns nothing
    call Afun()
    call Bfun()
    call Dfun()
endfunction
```

Well, not necessarily. It would depend on the order WE saves the scripts in the input file, if two libraries do not
require each other, then they may get added on a random order depending on the order in which they are
declared in the map. It is always a good idea to require the libraries you are going to use in your library.

Make sure to remember that:
- Library names are Case Sensitive.
- If library `A` requires library `B` and library `B` requires library `A` , there&apos;s a cycle and JassHelper will
  popup a syntax error.
- If library `A` requires a library that requires `B` and library `B` requires `A`, the cycle is still there.
- You can&apos;t nest libraries.
- Libraries might have globals blocks, as of version `0.9.B.0` library requirements determine the order in which these
  variables are added, notice that this warranty does not exist in prior versions.

It is also difficult to control what code is executed first, that&apos;s the reason libraries also
have an initializer keyword, you can add initializer `FUNCTION_NAME` after the name of a library and
it will make it be executed with priority using ExecuteFunc , ExecuteFunc is forced so it uses another
thread, most libraries require heavy operations on init so we better prevent the init thread from
crashing. After the initializer keyword the 'needs' statement might be used as well.

The initializers are added to the script in the same order libraries are added. So if library `A`
needs `B` and both have initializers then `B`&apos;s inititalizer will be called before `A`&apos;s.

Notice the initializer must be a function that takes nothing.

```sql
library A initializer InitA requires B
    
    function InitA takes nothing returns nothing
       call StoreInteger(B_gamecache , "a_rect" , Rect(-100.0 , 100.0 , -100.0 , 100  ) )
    endfunction

endlibrary

library B initializer InitB
    globals
        gamecache B_gamecache
    endglobals

    function InitB takes nothing returns nothing
        set B_gamecache=InitGameCache("B")
    endfunction
endlibrary
```

`B`&apos;s initializer will be called on init before `A`&apos;s initializer.

> The `library_once` keyword works exactly like `library` but you can declare the same library name twice, it would just
> ignore the second declaration and avoid to add its contents instead of showing a syntax error, it is useful in
> combination with textmacros.

> Older versions of vJass had a different syntax for libraries, that started with `//!` this was eventually deprecated
> and will now pop a syntax error out.

> As of `0.9.Z.0`, the declaration of a library will create a true boolean constant called `LIBRARY_libraryname`.
> Requirements may also be optional (prefix an `optional` keyword before the requirement name) which means that the
> library will be moved bellow that requirement, but if the requirement is not found, no syntax error will appear. These
> may be useful with another new `0.9.Z.0` feature: [static ifs](Static-ifs.md).




