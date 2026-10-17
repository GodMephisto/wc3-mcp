---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Keys.md"
ref: HEAD
---

# Keys

`key` is a special vJass type that is meant to generate unique integer constants you can use in various ways, it is
mostly intended to be used for key generation for warcraft 3&apos;s hashtable handle type.

Whenever you use the key type to declare a variable, a unique integer number is assigned to it. You may add the
constant keyword for extra readability if you want.

```sql
scope Tester initializer test

    globals
                 key AAAA
        private  key BBBB // yes it is just another type, so you can have
        public   key CCCC // public or private ones...
    
        constant key DDDD  //correctly describe it as a constant (not necessary)
    endglobals
    
    private function test takes nothing returns nothing
        local hashtable ht = InitHashtable()
        call SaveInteger(ht, AAAA, BBBB, 5)
        call SaveInteger(ht, AAAA, CCCC, 7)
        call SaveReal(ht, AAAA, DDDD, LoadInteger(ht,AAAA, BBBB) * 0.05 )
        call BJDebugMsg( R2S( LoadReal(ht,AAAA,DDDD) ) )
        
        call BJDebugMsg( I2S(BBBB) ) // will show two numbers, and
        call BJDebugMsg( I2S(CCCC) ) // the numbers will be different...
    endfunction
    
endscope
```