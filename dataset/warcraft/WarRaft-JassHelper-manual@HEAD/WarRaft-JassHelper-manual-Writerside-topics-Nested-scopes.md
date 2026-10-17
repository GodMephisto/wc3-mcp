---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Nested-scopes.md"
ref: HEAD
---

# Nested scopes

Scopes can be nested, don&apos;t confuse this statement with &quot;libraries can be nested&quot;, in fact, you cannot
even place a `library` inside an `scope` definition. You can however, have `scope` inside either `library` or `scope`
definitions.

An scope inside another scope is considered a child scope. A child scope is considered to be a public member of the
parent scope (?).

A child scope cannot declare members that were previously declared as private or global by a parent scope.

A child scope behaves in relation to its parent in the same way as a normal scope behaves in relation to the whole map
script.

Since child scopes are always public members, you can access a child scope&apos; public members froum outside the parent
scope, but it needs a prefix for the parent and a prefix for the child.

An example :

```sql
library nestedtest
    scope A
      globals
        private integer N=4
      endglobals

      public function display takes nothing returns nothing
        call BJDebugMsg(I2S(N))
      endfunction
    endscope

    scope B
        globals
            public integer N=5
        endglobals

        public function display takes nothing returns nothing
            call BJDebugMsg(I2S(N))
        endfunction
    endscope

    function nestedDoTest takes nothing returns nothing
        call B_display()
        call A_display()
    endfunction

endlibrary

public function outside takes nothing returns nothing
    set nestedtest_B_N= -4
    call nestedDoTest()
    call nestedtest_A_display()
endfunction
```

The next example will cause a syntax error:

```sql
library nestedtest
    globals
        private integer N=3
    endglobals

    scope A
      globals
        private integer N=4 //Error: 'N' redeclared
      endglobals
    endscope

endlibrary
```

It is actually caused by a limitation in the parser, there is a conflict caused by using `N` for the parent and then
declaring it for the child. However this version does not cause syntax errors:

```sql
library nestedtest
    scope A
      globals
        private integer N=4
      endglobals
    endscope

    globals
        private integer N=3
    endglobals

endlibrary
```

It does kind of the same thing, but since the child's `N` was declared before the parent&apos;s the parser no
longer
gets in a confusion.

Another thing to keep in mind is that unlike normal global variables, private/public global variables cannot be used
before declared, otherwise JassHelper will think they are just normal variables.

Scopes cannot be redeclared, there cannot be 2 scopes with the same name. But 2 child scopes might have the same name if
they are children of different scopes, the reason is that they actually don&apos;t have the same name, they have
different names given by their public child scope situation.

```sql
library nestedtest
  scope A
    function kkk takes nothing returns nothing
        set N=N+5  // By the time JassHelper gets to this line, it did not see the private integer N
                   // declaration yet, so it assumes N is an attempt to use a global variable and does not
                   // do any replacement
    endfunction
  endscope
endlibrary

scope X
   scope A
      //Declaring scope A again does not cause any problem, it is because the scope is actually X_A , the
      // previous declaration of A was actually nestedtest_A
      function DoSomething takes nothing returns nothing
      endfunction
   endscope
endscope
```

There is no nesting limit, but notice that resulting variable and function names of private/public members get big and
bigger depending of the depth of the scope nesting. Bigger variable names may affect performance. Not a lot but they do,
and this efficiency issue is to be prevented by an obfuscator/finalizer that renames every identifier in the map script
a.k.a the map optimizer's shortest names possible method.