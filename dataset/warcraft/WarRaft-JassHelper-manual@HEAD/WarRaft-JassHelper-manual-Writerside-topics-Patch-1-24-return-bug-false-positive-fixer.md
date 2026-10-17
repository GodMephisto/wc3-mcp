---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Patch-1-24-return-bug-false-positive-fixer.md"
ref: HEAD
---

# Patch 1.24 return bug false positive fixer

> This feature is disabled by default as the problems were fixed by patch `1.24b`.

Retail patch 1.24 brought a lot of doom and destruction to the world of Jass, thanks to some new blizzard bugs, certain
functions with more than one return statement would silent cause syntax errors or crash the game, effectively making
innocent maps unplayable, even if they do not use the now infamous &aquot;Return bug exploiters&aquot;. This was the
reason, that I had to add a quick fix to jasshelper, this compiler mod will replace all your functions with multiple
return values into two separate functions that will be safe from the false positive.

## Side effects and limitations {id="Side-effects-and-limitations"}

This fixer is only meant as a temporary measure while blizzard fixes this terrible bug or while you make your functions
safe from having multiple return statements, whatever happens first. The technique is a little dumb, for each function
that is suspect, it will create a new dummy one that returns nothing, and just assigns a global before returning, then
the old function will actually call this dummy function. In short words, this means that this fixer will make calling
certain functions (specifically those that have a return value and more than 1 return statements) slightly slower by
adding an extra function call.

This method does not work correctly with recursive functions (as it is still not possible to call a function from above
its declaration). It is likely that if a function is recursive and has multiple return statements, it will cause a
compiler error. In this case, it is better to modify the function, as it is most likely also a return bug false
positive (so it probably causes your map to be unplayable in patch `1.24`). To prevent this, jasshelper will try to
detect
the recursion, and if possible, take care of it. But there are some recursion patterns that jasshelper cannot fix at
this moment.

## Enabling the return fixer {id="Enabling-the-return-fixer"}

This feature must be manually enabled, so if you for some reason need to make your map work specifically with patch
1.24, or if maybe you think that some return bug false positive is still affecting your map, you can enable the feature.
In order to enable the return fixer, take `jasshelper.conf` and replace `[noreturnfixer]` with `[doreturnfixer]` if
there is
no tag `[noreturnfixer]` in `jasshelper.conf`, then simply add the `[doreturnfixer]` tag to it.
