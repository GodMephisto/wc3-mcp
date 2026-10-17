---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Text-Macros.md"
ref: HEAD
---

# Text Macros

Let&apos;s accept it, sometimes we want very complex things added to Jass but other times, the only thing we actually
need is an automatic text copy+paste+replace, textmacros were added because they can be really useful in a lot of very
different cases

The syntax is simple, `//! textmacro NAME [takes argument1, argument2, ..., argument n]` then `//! endtextmacro` to
finish.
And just a `runtextmacro` to run. It is easier to understand after an example:

```sql
//! textmacro Increase takes TYPEWORD
function IncreaseStored$TYPEWORD$ takes gamecache g, string m, string l returns nothing
    call Store$TYPEWORD$(g,m,l,GetStored$TYPEWORD$(g,m,l)+1)
endfunction
//! endtextmacro

//! runtextmacro Increase("Integer")
//! runtextmacro Increase("Real")
```

The result of the example is:

```sql
function IncreaseStoredInteger takes gamecache g, string m, string l returns nothing
    call StoreInteger(g,m,l,GetStoredInteger(g,m,l)+1)
endfunction
function IncreaseStoredReal takes gamecache g, string m, string l returns nothing
    call StoreReal(g,m,l,GetStoredReal(g,m,l)+1)
endfunction
```

The `$$` delimiters are required because the replace tokens could require to be together to other symbols.

Notice that strings and comments are not protected from the text replacement. So if there is a match for any of the
arguments delimited by `$$` it will always get replaced.

textmacros don&apos;t need arguments, in that case you simply remove the takes keyword.

```sql
//! textmacro bye
    call BJDebugMsg("1")
    call BJDebugMsg("2")
    call BJDebugMsg("3")
//! endtextmacro

function test takes nothing returns nothing
    //! runtextmacro bye()
    //! runtextmacro bye()
endfunction
```

Textmacros add just a lot of fake dynamism to the language, the next is the typical attach/handle vars call for handles:

```sql
 //! textmacro GetSetHandle takes TYPE, TYPENAME
    function GetHandle$TYPENAME$ takes handle h, string k returns $TYPE$
        return GetStoredInteger(udg_handlevars, I2S(H2I(h)), k)
        return null
    endfunction
    function SetHandle$TYPENAME$ takes handle h, string k, $TYPE$ v returns nothing
        call StoredInteger(udg_handlevars,I2S(H2I(h)),k, H2I(v))
    endfunction
//! endtextmacro

//! runtextmacro GetSetHandle("unit","Unit")
//! runtextmacro GetSetHandle("location","Loc")
//! runtextmacro GetSetHandle("item","Item")
```

The development time has beed reduced significantly

Textmacros and scopes/libraries can become best friends by allowing some kind of static object orientedness:

```sql
 //! textmacro STACK takes NAME, TYPE, TYPE2STRING
    scope $NAME$
        globals
            private $TYPE$ array V
            private integer N=0
        endglobals
        public function push takes $TYPE$ val returns nothing
            set V[N]=val
            set N=N+1
        endfunction

        public function pop takes nothing returns $TYPE$
            set N=N-1
            return V[N]
        endfunction

        public function print takes nothing returns nothing
         local integer a=N-1
            call BJDebugMsg("Contents of $TYPE$ stack $NAME$:")
            loop
                exitwhen a<0
                call BJDebugMsg(" "+$TYPE2STRING$(V[a]))
                set a=a-1
            endloop
        endfunction
    endscope
//! endtextmacro

//! runtextmacro STACK("StackA","integer","I2S")
//! runtextmacro STACK("StackB","integer","I2S")
//! runtextmacro STACK("StackC","string","")
function Test takes nothing returns nothing
    call StackA_push(4)
    call StackA_push(5)
    call StackB_push(StackA_pop())
    call StackA_push(7)
    call StackA_print()
    call StackB_print()
    call StackC_push("A")
    call StackC_push("B")
    call StackC_push("C")
    call StackC_print()
endfunction
```

> You can use `textmacro_once` in a similar way to `library_once`.

> If you  `//! runtextmacro` optional `textmacroname(args)`, the textmacro line will not cause a syntax error if the
> textmacro does not exist.    

