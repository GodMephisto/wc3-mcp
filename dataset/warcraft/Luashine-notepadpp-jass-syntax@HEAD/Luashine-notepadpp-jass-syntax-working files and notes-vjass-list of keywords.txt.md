---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "working files and notes/vjass-list of keywords.txt"
ref: HEAD
---

Only additional keywords from vJass

1. JassHelper directives: done.
Open: ((import loaddata beginusercode endusercode novjass inject endnovjass endinject runtextmacro))
Close: ((EOL))

Highlight-only:
---------------

initializer requires optional static private public SCOPE_PRIVATE SCOPE_PREFIX keyword this delegate implement key hook



Starts block:
-------------

library library_once scope struct method interface module



Middle block:
-------------




Closes block:
-------------

endlibrary endscope endstruct endmethod endinterface endmodule



What to do?
-----------
onInit, onDestroy, destroy, allocate, getType, typeid, create, evaluate, execute, .size, .width, .height

method/endmethod: very poor workaround, resolved like natives.
   Although method is serves both uses: as a "function" starter with takes and as a
   function prototype specifier (with takes but on a single line)
   
function interface Arealfunction takes real x returns real
   This totally breaks, yes it is a single line prototype again. I give up on vJass.
   
