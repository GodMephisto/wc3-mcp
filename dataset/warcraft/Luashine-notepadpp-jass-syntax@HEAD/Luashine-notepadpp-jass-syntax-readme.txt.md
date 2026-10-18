---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "readme.txt"
ref: HEAD
---

Original release on Hive: https://www.hiveworkshop.com/threads/notepad-jass-vjass-syntax-highlighter-2023-update.341147/

[HEADING=2][B]Features:[/B][/HEADING]
[LIST]
[*]100% Jass2 syntax support
[*]95% for vJass (see below)
[*]Partial support for JassHelper directives: //!
[*]All current natives added for highlighting (v1.33)
[*]All functions from common.j, blizzard.j, variables and constants
[*]Different colors for blizzard.j, common.j's natives and variables
[*]Colors made specifically for Notepad++ Obsidian theme (dark)
[*]Default extensions: .j, .jass and .ai for Jass, .vjass for vJass
[/LIST]

[B]Jass syntax:[/B] only vanilla Jass2 keywords are highlighted. Good for learning basic Jass.
[B]vJass syntax:[/B] extended syntax with vJass keywords and JassHelper. Good if you want to write/view vJass code.

There's no auto-completion support. (a) you can add it yourself (b) you should [URL='https://www.hiveworkshop.com/threads/jassdoc.275521/post-3530293']contribute to jassdoc[/URL] to make it a reality: help describe the natives!

Big image (140KB), test code:
[URL='https://www.hiveworkshop.com/data/ratory-images/271/271282-ed106ade19f1c9ee798f32b82e9a3764.png'][IMG]https://www.hiveworkshop.com/data/ratory-images/271/271282-ed106ade19f1c9ee798f32b82e9a3764_tn.jpg[/IMG][/URL]

Code repository (development): https://github.com/Luashine/notepadpp-jass-syntax

[HR][/HR]
[HEADING=2][B]Installation[/B][/HEADING]

[LIST=1]
[*](Recommended): Set Notepad++ theme to Obsidian:
[LIST=1]
[*]Notepad++ toolbar (top) > Settings > Style configurator > Make sure "Global Styles" is selected on the left > Choose "Select theme: Obsidian" above
[/LIST]
[*]Notepad++ toolbar (top) > Language > (at the bottom) User Defined Language > Open User Defined Language Folder...
[LIST=1]
[*]Extract the Jass_and_vJass-Luashine-v*.xml file to this folder (XML file has both Jass and vJass)
[/LIST]
[*]Restart Notepad++
[*]Now you can select Jass/vJass syntax highlighting under Language
[/LIST]
PS: If someone creates a color scheme for the white background/other themes, send it to me and I will include it.

[HR][/HR]
[HEADING=2][B]Introduction:[/B][/HEADING]

There were a few Notepad++ syntax highlighters made over the years to support Jass. The last one I used was [URL='https://wc3modding.info/172/vjass-syntax-highlighter-for-notepad/']by Moyack[/URL], it was alright but broke in various places like natives (very important to me). Also it's not made with a dark theme in mind. However it was last updated in 2020.

Before that, there was [URL='https://www.hiveworkshop.com/threads/jass-in-notepad.252872/post-2538251']Nestharus' Notepad++ plugin[/URL] to support Jass (2014). I don't remember if I got it working at all or didn't like something about it, but I didn't use it.

The reason Moyack updated his syntax highlighter was new "User Defined Language" features in Notepad++ and I can attest, that's very powerful (and frustrating to setup). But I did it! 12 hours spent or wasted ;)

PS: If you know/used other syntax highlighters, PM me the link so I can create a list of older stuff here.
[HR][/HR]
[HEADING=2][B]vJass: What does not work?[/B][/HEADING]

I've gone through [URL='https://wc3modding.info/pages/vjass-documentation/']the entire vJass manual[/URL] and copied examples from there. There's so much stuff, I doubt anyone actually knows all of it. However this piece will totally break the syntax highlighting:
[CODE=jass]function interface Arealfunction takes real x returns real[/CODE]
This is a function interface prototype, after this line Notepad++'s code block detection will break. The colors will still work though (see big screenshot at the very bottom). I couldn't apply the same hack that I did for the regular Blizzard natives... that was pain too!

method/endmethod is a little broken. The colors are broken for "endmethod" and method's name. Further, methods cannot be folded as blocks (not recognized by Notepad++)

Finally I didn't know what to do with vJass'es special keywords: [I]onInit, onDestroy, destroy, allocate, getType, typeid, create, evaluate, execute, .size, .width, .height[/I]. I left them without colored highlighting.

[HR][/HR]
[HEADING=2][B]JassHelper directives[/B][/HEADING]

What you can see in the big screenshot is the best I could get. The syntax definitions of User Defined Languages weren't supposed to work with three different languages at once (Jass, vJass, JassHelper's macros). I had to sacrifice comment folding to highlight JassHelper's directives.

For example, "//! inject main" is the only valid directive, "//!inject main" is not. But Notepad++ will not let me do anything with spaces or multiple words. If you wanted to really improve upon that result, update the syntax plugin by Nestharus or write a new one.

[HR][/HR]
[HEADING=2][B]Source included![/B][/HEADING]
Code repository on Github (development): [url=https://github.com/Luashine/notepadpp-jass-syntax]https://github.com/Luashine/notepadpp-jass-syntax[/url]

I have left my work notes and scripts inside. If this ever becomes out-of-date, it'll be easy to update. Especially the native and functions lists.

I consider the Jass syntax file to be primary, but I had to change a lot of stuff to make vJass work. Although I left notes what keywords to add for vJass, please compare the two configurations yourself to see what had to be changed between the two.

[HR][/HR]
[HEADING=2][B]How I made the "native" keyword work[/B][/HEADING]
[B][IMG]https://www.hiveworkshop.com/data/ratory-images/225/225346-5a709714ea1989bc31e5005fac1de871.png[/IMG][/B]

It'll be only of interest, if you want to update this syntax highlighter or create your own for Notepad++.

[code=jass]native ForForce takes force whichForce, code callback returns nothing
// versus
function receivesCallback takes code someFunc returns nothing
    call someFunc()
endfunction

function passAFunction takes nothing returns nothing
    call receivesCallback(function someGlobalFunction)
endfunction
[/code]

This was one of the reasons I started creating this highlighter and turned out to be the hardest. The issue here, you want Notepad++ to recognize function ... endfunction as a code block. You will encounter two conflicts:
[LIST=1]
[*]"function" can be used as a standalone keyword, when passing a function as an argument
[*]native does not start a block, it ends on the same line
[/LIST]
So then you only have the two keywords at your disposal "takes" and "returns". Both are present in function... and native function... So if you start a new function block with either "takes" or "returns", then any native definition will break code blocks in the entire file, because they'd start a new block but never end it (with endfunction).

The workaround was to abuse delimiters. "Takes" is registered to start a new code block. However if "takes" is captured as part of a delimiter, it will NOT start a new code block. With this trick, natives are actually just highlighted "delimiter 7" keywords that grab the next "takes" word and thus suppress the creation of a code-block for native definitions.

[HEADING=2]Changelog[/HEADING]
[SPOILER="Version 9, 10"]
Added a bunch of scripts to automate extracting of keywords.

[b]Version 10:[/b]
Updated keyword lists for Reforged 3.0.0
(Internal) keyword lists use spaces again

[b]Version 9:[/b]
Added 'array' to highlighting.
Added ".jass" extension for Jass.
[/SPOILER]
[SPOILER="Version 8"]
Actually fix HEX numbers (did not recognize A-F,a-f characters because I put them in the wrong "Extra" field)
Make Jass comments non-italic (vJass untouched)
[/SPOILER]
[SPOILER="Version 7.5"]
Test file: Fix actual syntax errors in the plain jass example file
Update the screenshot accordingly
[/SPOILER]
[SPOILER="Version 7"]
Jass & vJass: Fix incorrect hexadecimal prefix (only 0x, 0X and $ are valid).
[/SPOILER]
[SPOILER="Version 6"]
Jass & vJass: bump version to 6, fix the hardcoded background style in this language style. I.e. unless "Enable global background colour" is checked, the letter background appeared white. Thanks @DrTema

vJass: Added to definitions of external preprocessors: "external, externalblock, endexternalblock"

If I remember correctly: removed incorrectly set Bold style from deep-blue colored natives.[/SPOILER]
