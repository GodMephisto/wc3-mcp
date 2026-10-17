---
type: vendor-doc
source_repo: "https://github.com/Luashine/notepadpp-jass-syntax"
source_path: "readme.txt"
ref: HEAD
---

Original release on Hive: https://www.hiveworkshop.com/threads/notepad-jass-vjass-syntax-highlighter-2023-update.341147/

[b]Features:[/b]
[list]
[li]100% Jass2 syntax support[/li]
[li]95% for vJass (see below)[/li]
[li]Partial support for JassHelper directives: //![/li]
[li]All current natives added for highlighting (v1.33)[/li]
[li]All functions from common.j, blizzard.j, variables and constants[/li]
[li]Different colors for blizzard.j, common.j's natives and variables[/li]
[li]Colors made specifically for Notepad++ Obsidian theme (dark)[/li]
[li]Default extensions: .j and .ai for Jass, .vjass for vJass[/li]
[/list]

Jass syntax: only vanilla Jass2 keywords are highlighted. Good for learning basic Jass.
vJass syntax: extended syntax with vJass keywords and JassHelper. Good if you want to write/view vJass code.

 [attachment id=2 msg=new] 

---

[b]Installation[/b]

[list type=decimal]
[li](Recommended): Set Notepad++ theme to Obsidian: Notepad++ toolbar (top) > Settings > Style configurator > Make sure "Global Styles" is selected on the left > Choose "Select theme: Obsidian" above[/li]
[li]Notepad++ toolbar (top) > Language > (at the bottom) User Defined Language > Open User Defined Language Folder... > extract the .xml file to this folder (XML file has both Jass and vJass).[/li]
[li]Restart Notepad++[/li]
[li]Now you can select Jass/vJass syntax highlighting under Language[/li]
[/list]

---

There were a few Notepad++ syntax highlighters made over the years to support Jass. The last one I used was [url=https://wc3modding.info/172/vjass-syntax-highlighter-for-notepad/]by Moyack[/url], it was alright but broke in various places like natives (very important to me). Also it's not made with a dark theme in mind. However it was last updated in 2020.

Before that, there was [url=https://www.hiveworkshop.com/threads/jass-in-notepad.252872/post-2538251]Nestharus' Notepad++ plugin[/url] to support Jass (2014). I don't remember if I got it working at all or didn't like something about it, but I didn't use it.

The reason Moyack updated his syntax highlighter was new "User Defined Language" features in Notepad++ and I can attest, that's very powerful (and frustrating to setup). But I did it! 12 hours spent or wasted ;)

---

vJass: What does not work?

I've gone through [url=https://wc3modding.info/pages/vjass-documentation/]the entire vJass manual[/url] and copied examples from there. There's so much stuff, I doubt anyone actually knows all of it. However this piece will totally break the syntax highlighting:
[code=jass]function interface Arealfunction takes real x returns real[/code]
This is a function interface prototype, after this line Notepad++'s code block detection will break. The colors will still work though (see big screenshot at the very bottom). I couldn't apply the same hack that I did for the regular Blizzard natives... that was pain too!

method/endmethod is a little broken. The colors are broken for "endmethod" and method's name. Further, methods cannot be folded as blocks (not recognized by Notepad++)

Finally I didn't know what to do with vJass'es special keywords: onInit, onDestroy, destroy, allocate, getType, typeid, create, evaluate, execute, .size, .width, .height. I left them without colored highlighting.

---

JassHelper directives

What you can see in the big screenshot is the best I could get. The syntax definitions of User Defined Languages weren't supposed to work with three different languages at once (Jass, vJass, JassHelper's macros). I had to sacrifice comment folding to highlight JassHelper's directives.

For example, "//! inject main" is the only valid directive, "//!inject main" is not. But Notepad++ will not let me do anything with spaces or multiple words. If you wanted to really improve upon that result, update the syntax plugin by Nestharus or write a new one.

---

Source included! I have left my work notes and scripts inside. If this ever becomes out-of-date, it'll be easy to update. Especially the native and functions lists.

I consider the Jass syntax file to be primary, but I had to change a lot of stuff to make vJass work. Although I left notes what keywords to add for vJass, please compare the two configurations yourself to see what had to be changed between the two.

---

How I made the "native" keyword work

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
[list type=decimal]
[li]"function" can be used as a standalone keyword, when passing a function as an argument[/li]
[li]native does not start a block, it ends on the same line[/li]
[/list]
So then you only have the two keywords at your disposal "takes" and "returns". Both are present in function... and native function... So if you start a new function block with either "takes" or "returns" then any native definition will break code blocks in the entire file, because they'd start a new block but never end it (with endfunction).
The workaround was to abuse delimiters. "Takes" is registered to start a new code block. However if "takes" is captured as part of a delimiter, it will NOT start a new code block. With this trick, natives are actually just highlighted "delimiter 7" keywords that grab the next "takes" word and thus suppress the creation of a code-block for native definitions.

[HEADING=2]Changelog[/HEADING]
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
Jass & vJass: bump version to 6, fix the hardcoded background style in this language style. I.e. unless "Enable global background colour" is checked, the letter background appeared white.

vJass: Added to definitions of external preprocessors: "external, externalblock, endexternalblock"

If I remember correctly: removed incorrectly set Bold style from deep-blue colored natives.[/SPOILER]
