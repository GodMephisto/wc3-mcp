---
type: vendor-doc
source_repo: "https://github.com/lep/jassjass"
source_path: "README.md"
ref: HEAD
---

# Jass² - A Jass Parser written in vJass

A very quick Overview.

How to parse all toplevel declarations:
````
local string j = "

type bla extends integer

globals
    constant integer x = 3
    integer y
    integer array z
endglobals

function foo takes integer x, bla foo returns real
    set x = y
    return lol
endfunction
"

local Jass_Parser p = Jass_Parser.create(j)
local Jass_List e = p.parseProgram()

loop
exitwhen e == Jass_Nil
    call BJDebugMsg(Jass_Toplevel(e.head).toString())
    set e = e.tail
endloop
````

Parsing a statement:
````
local string j = "set arr[3+x*foo()] = .123\n"
local Jass_Parser p = Jass_Parser.create(j)

call BJDebugMsg(p.parseStatement().toString())
````

Parsing a single expression:
````
local string j = "a * b[3] + foo(bar, $ff)"
local Jass_Parser p = Jass_Parser.create(j)

call BJDebugMsg(p.parseExpr().toString())
````
