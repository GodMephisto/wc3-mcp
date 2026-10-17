---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Usage.md"
ref: HEAD
---

# Usage

## Newgen Pack

After installing the new jasshelper executable, just open Newgen World Editor as usual, it should now call the new
jasshelper version. In order to know more about newgen's jasshelper menu you should probably take a look to newgen's
readme file.

## Grimoire

Simply use `we.bat` to run grimoire, then you'd have to disable the syntax checker and enable the map compiler using the
grimoire menus. To use debug mode simply use compiler\\compiler debug mode.

Currently, compiler is not called when using testmap, so you must save the map before using the testmap button.

## Command line

`jasshelper.exe` combined with the `sfmpq.dll` and pjass is able to compile maps without any aid from an editor hack,
you
might want to know about this if you are on Linux (where WINE allows you to use WorldEditor and jasshelper but not
grimoire) or for example if you cannot run grimoire for whatever reason.

The basic command line syntax is:

```Console
jasshelper.exe <path\_to\_common.j> <path\_to\_blizzard.j> <path\_to\_map.w3x>
```

This will make jasshelper to process the source map, and update the map with a new compiled script. You can extract
`common.j` and `blizzard.j` from the scripts folder in `war3patch.mpq`.

If instead of three arguments you pass four file arguments to the program, the behavior changes:

```Console
jasshelper.exe <path\_to\_common.j> <path\_to\_blizzard.j> <path\_to\_mapscript.j> <path\_to\_map.w3x>
```

It will ignore the map's script file, and instead consider the given script file as the vJass source. Since the 3 files
syntax removes the original vJass source code from the map, this method is more useful, you can generate the source map
by exporting the map's script from the editor.

> Use `//! import` and `//! novjass` in combination to command line jasshelper and World Editor

Of course, that is not enough flexibility, so jasshelper supports a couple of options you can place before the path to
`blizzard.j`:

--debug
: This flag will make jasshelper compile in debug mode (more information in the quite extense vJass section). It also
turns --nooptimize on

--nopreprocessor
: If for some reason you just want to check normal Jass syntax, and call PJass using jasshelper as proxy, so you can use
this option.

--nooptimize
: Disables optimization, refer to the [script optimization](Script-optimization.md) section for more information.

--scriptonly
: This one changes the behavior of the next arguments, it forces you to provide four files:
```Console
jasshelper.exe --scriptonly <path\_to\_common.j> <path\_to\_blizzard.j> <path\_to\_input.j> <path\_to\_output.j>
```
This syntax requires no map to be provided, will simply evaluate the input .j file and show syntax errors if
necessary. If the compiling is successful, jasshelper will write the output script to the file path you provided.

--warcity
: This setting will automatically turn `--scriptonly` on, it makes jasshelper evaluate the input script
file as if it was a "warcity script file", WarCiTy is a program that converts a map's custom trigger data into a
special sort of `.j` script. This setting will simply make jasshelper evaluate only the `//! import` and `//! novjass`
preprocessors, it will also prevent the adition of guide comments specifying it just imported the file. There is no
syntax checking feature for this mode.

--zinconly
: This setting will automatically turn --scriptonly on, it makes jasshelper evaluate the input script
file assuming it is a single zinc source file. It will then output the compiled vJass code after the zinc phase.

--macromode
: Like warcity but also evaluates textmacros.

--about
: This just displays the about dialog (Do you want to know what jasshelper version you got?) Notice that it ignores the
file arguments provided.

--showerrors
: Shows the previous syntax error(s) (Without compiling). `clijasshelper.exe` behaves exactly as `jasshelper.exe` but it
does not use/need windows GUI, (it
is still a windows-WINE app though), it may be useful sometimes (for example if you want to use jasshelper from a ssh
sesion), it just outputs stuff to stdout, if for some reason stdout does not work, it will output to `stdout.txt` in the
work folder.