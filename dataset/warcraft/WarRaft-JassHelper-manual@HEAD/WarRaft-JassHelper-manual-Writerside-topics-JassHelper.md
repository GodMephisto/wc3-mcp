---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/JassHelper.md"
ref: HEAD
---

# JassHelper

Although World Editor&apos;s Jass compiler was finally replaced by PJass using WEHelper , there were
a couple of other annoyances that still needed fixing, that&apos;s the reason this project begand.

Later I felt like going further and restarted the idea of extending Jass to <tooltip term="OOP">OOP</tooltip> thus
JassHelper is a compiler for the
vJass language extension which includes structs, libraries, textmacros and more.

Although this is not really <tooltip term="OOP">OOP</tooltip> the syntax is powerful enough I hope, there is no
inheritance and that&apos;s the reason
I am not calling the objects classes but structs. There are however, interfaces which allow polymorphism and since they
can declare attributes you can have some kind of pseudo inheritance. Pseudo inheritance can be accomplished in many
ways.

The design of vJass ought to stay static eventually so I stop adding syntax construct cause that&apos; the right thing
to do. After version `1.0.0` there should not be any addition, so if you have requests hurry up cause it would not be
healthy to change the syntax after `1.0.0`.

Version `Z.0` introduces the addition of the Zinc language to jasshelper, it is just a less verbose alternative to vJass
that is also more rigid on some aspects.
