---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Import-external-script-files.md"
ref: HEAD
---

# Import external script files

JassHelper also includes a `//! import` preprocessor like WEHelper&apos;s, it allows you to import external files.

The usage is `//! import "scriptfile"` , you can use fixed paths or relative paths.

For example: `//! import "c:\x.j"` will include `x.j` file located on `c:\` in your map script. It is effectively like
pasting
the file there before compiling it, so the file can include globals, libraries, textmacros, etc. It can even use
`//! import` itself

If you use `//! import` twice or more on the same file name , the command is ignored.

Relative paths are allowed, for example: `//! import "x.j"` or `//! import "subfolder\f.j"` will use relative paths.
When
the path is relative JassHelper will look for the script file in the map&apos;s folder or in the lookup folders set in
JassHelper&apos;s configuration.

Grimoire&apos;s version uses jasshelper.conf to determine the lookup folders, WEHelper&apos;s version comes with a
dialog that allows you to set them. Also the version for WEHelper will automatically add wehelper&apos;s import path to
the lookup folders list.

WEHelper&apos;s internal import preprocessor is most likely to override JassHelper&apos;s import since the default is to
call it before JassHelper, in case you want to use JassHelper&apos;s import you would have to disable WEHelper&apos;s,
the only advantage the jasshelper one has over the wehelper one is the ability to configure lookup folders.

Grimoire&apos;s `jassehlper.conf` already determines grimoire&apos; Jass folder as a position of scripts. You can add
whatever folders you find necessary. Notice that there is a priority system, if a file is found on the map&apos;s folder
it will be imported no matter the same file exists in another folder used in `jasshelper.conf`

There is a problem with using the map&apos;s folder and it is that testmap saves the map in another path. So you would
preffer to use custom paths for this option.

Test map is often problematic with this kind of features. The best way to deal with this is to ALWAYS SAVE BEFORE USING
TESTMAP, this will force the map to go to its path and then will use it for testmap, in case you need to test temporary
changes, you can save it with another name in the same folder and then do testmap.

As of jasshelper `0.9.C.0` , relative paths used by import an already-imported file will be able to also support the
file&apos;s path as possible container. Notice that the files in the same folder as the current file have priority over
files on the configured paths or even the map&apos;s location.

import syntax has been extended to have an extra script type option:

```sql
//! import vjass/zinc/comment "filename"
```

vjass is assumed to be the default. zinc makes the file to be considered a zinc script. comment makes the import command
get ignored unless we compile under the warcity mode.