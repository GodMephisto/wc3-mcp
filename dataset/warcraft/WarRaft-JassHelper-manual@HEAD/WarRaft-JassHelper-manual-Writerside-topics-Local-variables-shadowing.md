---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Local-variables-shadowing.md"
ref: HEAD
---

# Local variables shadowing

Starting with version [](Changelog.md#0.9.K.0), local variable shadowing is part of vJass syntax, unlike Jass syntax in
which it has
always been a problem. Up to patch `1.24`, it was not even possible to shadow more than one variable, and it turned the
whole map script into case-insensitive. These glitches were apparently fixed around patch `1.24`, but they were fixed
barely to a level of helping GUI users do local var tricks. Issues persist, and the issues are of the kind that could
eventually cause some scripts issues in unpredictable places.

If the vJass code is used correctly and things are correctly scoped, this should not be a problem. However, when using
code from multiple places, one of the codes may not be correctly scoped, which threatens to cause a conflict with a
local variable and thus then making your map mysteriously unopenable in wc3. Jass' weakness with variable shadowing was
too unpredictable in nature. So I decided to add a small preprocessor phase to jasshelper that will simulate correct
local variable shadowing. This means that the local variables in your functions may now have the same name as any global
in the map (even globals you do not know about) without causing further issues or unknown, unpredictable bugs (unlike
normal Jass in which this is an active possibility). Although the vJass code allows variable shadowing, it is compiled
to Jass code in which there is no shadowing (some `l__` prefixes are added to conflicting local variables).
