---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Private-members.md"
ref: HEAD
---

# Private members

With the adition of libraries it was a good idea to add some scope control, private members are a great way of
protecting users from themselves and to avoid collisions.

```sql
library privatetest
    globals
        private integer N=0
    endglobals
    private function x takes nothing returns nothing
        set N=N+1
    endfunction

    function privatetest takes nothing returns nothing
        call x()
        call x()
    endfunction
endlibrary

library otherprivatetest
    globals
        private integer N=5
    endglobals
    private function x takes nothing returns nothing
        set N=N+1
    endfunction

    function otherprivatetest takes nothing returns nothing
        call x()
        call x()
    endfunction
endlibrary
```

Notice how both libraries have private globals and functions with the same names, this wouldn&apos;t cause any syntax
errors since the private preprocessor will make sure that private members are only available for that scope and
don&apos;t conflict with things named the same present in other scopes. In this case private members are only to be used
by the libraries in which they are declared.

Sometimes, you don&apos;t want the code to go to the top of your script (it is not really a function library) yet
you&apos; still want to use the private keyword for a group of globals and functions. This is the reason we defined the
`scope` keyword

The `scope` keyword has this syntax: `scope NAME [...script block...] endscope`

So, functions and other declarations inside an scope can freely use the private members of the scope, but code outside
won&apos;t be able to. (Notice that a library is to be considered to have an internal scope with its name)

There are many applications for this feature:

```sql
scope GetUnitDebugStr

    private function H2I takes handle h returns integer
        return h
        return 0
    endfunction

    function GetUnitDebugStr takes unit u returns string
        return GetUnitName(u)+"_"+I2S(H2I(u))
    endfunction
    
endscope
```

In this case, the function uses `H2I`, but `H2I` is a very common function name, so there could be conflicts with other
scripts that might declare it as well, you could add a whole preffix to the `H2I` function yourself, or make the function
a library that requires another library `H2I`, but that can be sometimes too complicated, by using an scope and private
you can freely use `H2I` in that function without worrying. It doesn&apos; matter if another `H2I` is declared elsewhere and
it is not a private function, private also makes the scope keep a priority for its members.

It is more important for globals because if you make a function pack you might want to disallow direct access to globals
but just allow access to some functions, to keep a sense of encapsullation, for example.

The way private work is actually by automatically prefixing `scopename(random digit)__` to the identifier names of the
private members. The random digit is a way to let it be truly private so people can not even use them by adding the
preffix themselves. A double `_` is used because we decided that it is the way to recognize preprocessor-generated
variables/functions, so you should avoid to use double `__` in your human-declarated identifier names. Being able to
recognize preprocessor-generated identifiers is useful when reading the output file (for example when PJass returns
syntax errors).

In order to use private members `ExecuteFunc` or real value change events you have to use [SCOPE_PRIVATE](SCOPE-PREFIX-and-SCOPE-PRIVATE.md).

> Scopes support `initializer` just like libraries, there is a difference in implementation and it is that they use
> a normal call rather than an ExecuteFunc call, if you need a heavy process to init a scope, better use a library
> initializer or call a subfunction using ExecuteFunc from the scope initializer.

> In a similar way to libraries, scopes used to have a syntax that required `//!` , that old syntax is deprecated and will
> cause a syntax error.
