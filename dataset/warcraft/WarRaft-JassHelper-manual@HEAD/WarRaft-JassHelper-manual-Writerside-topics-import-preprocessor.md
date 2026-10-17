---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/import-preprocessor.md"
ref: HEAD
---

# import preprocessor

A saner way is this extension to the good, old and reliable import preprocessor in vJass. It is now
`[//! import [vjass/zinc] "filename"]`, which means that while importing the file you may also specify the language in
the file. This language specification is optional and implied from the place in which you call it. In example, if you
call it from a vJass area, it will assume the imported file is in vJass format unless you explicitely include the zinc
word.
Likewise, if you perform the import from a Zinc area, it will assume the imported file is in zinc language.

```C++
//imports the file main.zn from code folder, consider it in zinc language
//! import zinc "code/main.zn"

//imports the file libTimerUtils.j from code/libs folder, consider it in vJassc language
//! import vjass "code/libs/libTimerUtils.j"

//imports the file spell1.j from code folder, use the current language.
//! import "code/spell1.j"
```