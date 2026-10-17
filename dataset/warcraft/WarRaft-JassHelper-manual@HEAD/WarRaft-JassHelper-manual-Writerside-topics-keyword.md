---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/keyword.md"
ref: HEAD
---

# keyword

The keyword statement allows you to declare a replacement directive for an scope without declaring an actual
function/variable/etc. It is useful for many reasons, the most important of the reasons being that you cannot use a
private/public member in an scope before it is declared, in most cases this limitation is no more than an annoyance
requiring you to change the position of declarations, in other situations though this is a limitator of other features.

For example two mutually recursive functions may use .evaluate (keep reading this readme) to call each other, but if you
also want the functions to be private it is impossible to do it without using keyword:

```sql
scope myScope

   private keyword B //we were able to declare B as private without having to actually
                     //include the statement of the function which would cause conflicts.

   private function A takes integer i returns nothing
       if(i!=0) then
           return B.evaluate(i-1)*2 //we can safely use B since it was already
                                    //declared as a private member of the scope
       endif
       return 0
   endfunction

   private function B takes integer i returns nothing
       if(i!=0) then
           return A(i-1)*3
       endif
       return 0
   endfunction

endscope
```