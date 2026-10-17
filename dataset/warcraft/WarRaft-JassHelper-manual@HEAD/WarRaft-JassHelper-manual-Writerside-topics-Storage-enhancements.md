---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Storage-enhancements.md"
ref: HEAD
---

# Storage enhancements

There is an internal limit in Jass regarding array sizes, jasshelper is widely affected by it since it directly
affects struct instance limits, for example. There is a way to
virtually increase the limit by combining together a number of Jass arrays and translate indexes of the bigger array
into indexes of the smaller arrays.

By using storage enhancer syntax you can make Jasshelper do this trick. But there is a catch, by increasing the limit
of available indexes you make sacrifices of many kinds:

- Operations that would usually just need an array lookup would require a function call instead, function calls
  are very slow in Jass in comparison to array lookups.
- The function requires to do some extra operations itself, currently the number of operations these functions
  take depends on
  (`index_limit / 8191`), soon a jasshelper improvement might allowe them to do log_2(index_limit/8191), notice that
  for smaller index limits this improvement is not important.
- The script size can increase significantly if you use very big index limits, on a lot of objects, for example if
  your struct got 20 fields and you use an index limit of `60000`,
  the compiled script will require 40 new functions each with a little more than 16 lines.

Some Jass applications will require more space which means you would have to use enhancers regardless of the
limitations, in case you do not really need more space, using these enhancers is
discouraged because of the cons described above, if you are making a flexible system that might or might not require
these enhancements you can make the enhancer usage optional,
because space syntax allows you to use constant variables in its declaration you can make the user able to determine
it, if you use size enhancer syntax to
specify a size not bigger than 8191 (or 8190 in the case of structs, since you also need the 0 instance) nothing
will happen and the penalties described above will not apply.

An internal jasshelper limit forbids a declaration that would require the script to use more than 8 arrays for the
same big array, leading to an index space limit
of around `408000`, if you need more space, request so but include a good description of the (rather crazy) thing you
are doing that requires so much space, I am interested in learning about it...