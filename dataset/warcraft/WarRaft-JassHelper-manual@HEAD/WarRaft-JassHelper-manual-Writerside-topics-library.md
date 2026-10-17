---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/library.md"
ref: HEAD
---

# library

Libraries are the main, mandatory construction blocks in Zinc, they are defined as a group of code, variables, and data
structures that do something for your own good. Libraries may use stuff from other libraries, in which case they need to
explicitely say so by mentioning a `requires`. Libraries are also your way into running code. If a library has a
function called `onInit`, this function will get called on startup.

```C++
library CodeGoesHereo
{
    // code goes here:
    // * global variables
    // * functions
    // * structs
    // * interfaces
    // * other types (dynamic arrays, function pointer types)

    function onInit()
    {
        //Your onInit() function gets called when the map is starting.
    }
}
```

If we wish to use stuff from library `BB` inside our library `AA`, we need to tell the compiler that the library `AA`,
`requires` library `BB`. Basically, when your library requires other libraries, first the `onInit` functions of the
other
libraries get executed before yours. Their code is also moved to a place that is above yours in the output code.

```C++
 library AA
{
    public function AABoom()
    {
        BJDebugMsg("AA BOOM");
    }
    function onInit()
    {
        BJDebugMsg("AA");
    }
}

library BB requires AA
{
    function onInit()
    {
        BJDebugMsg("BB");
        AABoom();
        AABoom();
    }
}
```

Do notice that a library may have many requirements, in that case, separate each requirement by a comma (
`library name requires requirement1, requirement2, ... requirementN` ).

This last sample illustrates a public library member:

requirements can be made `optional` by adding the `optional` keyword before the requirement&apos;s name, this makes the
compiler just consider the requirement if the required library exists, without giving any syntax errors if the
requirement is not found. This is useful when combined with `static ifs`.