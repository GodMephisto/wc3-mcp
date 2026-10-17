---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Declaring-structs.md"
ref: HEAD
---

# Declaring structs

Before using an struct you need to declare it, duh. The syntax is simply using the struct `<name>` and `endstruct`
keyword, notice how similar they are to global blocks

To declare a member you simply use `<type> <name> [= initial value]`

In the above example we are declaring an struct type named pair, which has 2 members: x and y, they do not have an
initial value set.

It is usually a good idea to assign initial values to the members, so that you don&apos;t have to manually initialize
them after creating an object of the struct type, the usual default values are the null ones, but depending on the
problem you want to solve you could need any other value.
