---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Script-optimization.md"
ref: HEAD
---

# Script optimization

Since version 0.9.A.0 script optimization is available in jasshelper, it is currently enabled by default, in order to
disable optimization you can either enable debug mode or use the `--nooptimize` argument (`jasshelper.exe`), newgen
should get a menu entry for toggling this option added soon.

At the moment the only available optimization is function inlining. More methods shall be added later including some
improved versions of some of wc3mapoptimizer&apos;s options.

## Function inlining

Function inlining will look for function calls that can be inlined and then just convert their calls to a direct
usage of the function&apos;s contents. In order not to break the normal execution of the map, this is done only in
few cases. An example of inlining follows.

```sql
function MyFunction takes integer a, integer b returns integer
    return myarray[a]*b
endfunction

function MyOtherFunction takes nothing returns integer
    return MyFunction(3,4)
endfunction

//becomes:

function MyOtherFunction takes nothing returns integer
    return myarray[3]*4
endfunction
```

Inlining is important because it will reduce the number of function calls and make certain parts of the map script
faster, while at the same time it will allow you to write readable code.

How to make a function inlineable? The current algorithm basically follows these rules (which are subject to change
to allow more functions to be considered inlineable in the future):

- The function is a one-liner
- If the function is called by `call` the function&apos;s contents must begin with
  `set` or `call` or be a `return` of a single function.
- If the inlined function is an assigment (`set`) it should not assign one of its arguments.
- Every argument must be evaluated once and only once by the function, in the same order as they appear in the
  arguments list.
- If the function contains function calls, they should all be evaluated after the arguments UNLESS the function is
  marked as non-state changing, at the moment some very few native functions and also return bug exploiters are
  considered as non-state changing.
