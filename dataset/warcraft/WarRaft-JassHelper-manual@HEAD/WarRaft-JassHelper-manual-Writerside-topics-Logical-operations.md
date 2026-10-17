---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Logical-operations.md"
ref: HEAD
---

# Logical operations

There are some operations that can be done to booleans to combine or change their results.

`!`
: The not operator, it negates the boolean ( `!true` equal `false` and `!false` equals `true`).

`&&`
: The and operator, it requires both booleans to be `true`, else it returns `false` ( `true&&true` equals `true`,
`true&&false` and any other combination equals `false`).

`||`
: The or operator, it requires any of the booleans to be true, it returns `false` if both are `false` ( `false||false`
equals `false`, `true&&false` and any other combination equals `true`).