---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Installation.md"
ref: HEAD
---

# Installation

## Supported platforms

This program is currently developped for windows XP SP2, WINE 1.0 (or greater) . It probably works well on older
versions of windows in wich warcraft III also runs. Windows XP SP3 probably works as well. It might work on vista but
there is no testing for that OS or ability to debug there. Please, notice that though you are able to run in platforms
other than windows XP and WINE>=1.0 I might not able to fix bugs you report in them that I cannot reproduce in XP or
WINE.

## Jass New Gen Pack

Jass New Gen Pack already comes with JassHelper, although it might not come with the most recent version, in order to
update you can simply copy executable\\jasshelper.exe to the NewGen Pack"s jasshelper subfolder. You may also update the
JassHelper documentation if you wish

It is suggested that you install new gen pack first and then update jasshelper, it may be the case that newgen pack's
jasshelper holds the same version of jasshelper then it would not be necessary to update.

In order to get Jass New Gen Pack
visit: [http://www.wc3campaigns.net/showthread.php?t=90999](http://www.wc3campaigns.net/showthread.php?t=90999)

## WEHelper Plugin

I decided to stop including the WEHelper plugin in jasshelper's distribution package. If you still use WEHelper you
should consider moving to newgen, else you can request me the .dll file for jasshelper. You may also make your own
WEHelper plugin and make it use the standalone jasshelper.exe to compile the map. Or you can compile the .DLL file from
the sourcecode if you have a delphi compiler.

## Grimoire

You need [grimoire 1.2 or greater](http://www.wc3campaigns.net/showthread.php?t=86652) which includes wehack.dll. Then
you have to copy `executable\\jasshelper.exe` to a jasshelper subfolder in grimoire's folder. You also
need [pJass](http://www.wc3campaigns.net/showthread.php?t=75239), locate pJass.exe on grimoire's folder as well. As of
grimoire 1.2 you may have to update wehack.lua yourself or wait for a newer grimoire version that will be compatible
with the new way jasshelper works.

You will have to copy SFMPQ.dll to the jasshelper folder

## Standalone

`executable\\jasshelper.exe` may also work as an standalone compiler, you require `bin\\SFMPQ.dll` and pjass in the same
folder (notice `SFMPQ.dll` needs to be inside a subfolder called bin), after that simply refer to the command line
subsection in the usage section.

If you are gonna call `jasshelper.exe` from another tool, notice that jasshelper will create and use logs and backups
subfolders inside its work folder. jasshelper.conf has priority in the work folder and if it is not found the one in
jasshelper's folder is gonna be created and used.