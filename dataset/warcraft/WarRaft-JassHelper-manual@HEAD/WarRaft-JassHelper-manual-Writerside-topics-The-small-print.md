---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/The-small-print.md"
ref: HEAD
---

# The small print

Many details about Zinc, some of which are not nice.

## Textmacros?

For better or worse, due to an implementaton detail, Zinc inherits many vJass preprocessors, for example `//! external`
and `loaddata`. But most importantly, textmacros. These vJass preprocessors ought to keep the same syntax. So textmacros
in Zinc works exactly as they do in vJass.

## The compile error abomination

jasshelper is a multi-phase process. To list some of its processes in the correct order I will mention:

* **Phase 1:** import/novjass/delimited comments
* **Phase 2:** text macros
* **Phase 3:** Zinc
* **Phase 4:** Libraries
* **Phase 5:** Static ifs
* **Phase 6:** Modules
* **Phase 7:** Structs and many other things
* **Phase 8:** PJass
* **Phase 9:** Shadowhelper
* **Phase 10:** PJass
* **Phase 11:** Optimization (inline)

The problem is that each phase receives a different input, the output of the previous phase. import phase begins by
joining all the files into a same source, after that (and this is nice, textmacros, zinc, libraries, work basically
using the same source, which means that any syntax error found before the static ifs phase will look all right to you (
except it will not tell you the exact file from which it found the error line).

Problem begins afterwards. Although Modules and structs share the same input file, the remaining phases each use a
different file, and give errors in a different input file. In vJass, it kind of works, because the compiled code in
which you find the errors is not that different to what you wrote. Problem with Zinc is that if the error is not found
before the static if phase, you will see your syntax erros in some vJass code that is completely different to your
original Zinc code. This is excessively unfriendly, and sucks, so be warned.

My priority is fixing this issue so that each phase will give you errors on the original code. It is complicated, as
requires rewriting of many things, but it is possible.

## Comment eating

The Zinc compiler eats all your comments, in case of the delimited ones, that&apos;s necessary for them to work. But as
for the line comments, it is not really necessary for it to happen. The problem is with the way parsing is done right
now, Gold grammar is set to ignore the whitespace and comments completely. A hacky fix, tries to recover the comments
but it may miss some and it puts them in odd places.

## how is while compiled?

Now on to good things, compiling while to Jass&apos; exitwhen requires to negate the condition. However, the compiler
does some magic such that the condition sometimes does not take more time or space. For example, instead of converting
`(a==b)` into `not (a==b)` it will convert it into a!=b. Of course, sometimes the expression is very complicated and
just adding the not is better.
