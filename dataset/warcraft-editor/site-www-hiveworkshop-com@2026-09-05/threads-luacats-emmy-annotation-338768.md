---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/luacats-emmy-annotation.338768/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Luacats Emmy Annotation
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/luacats-emmy-annotation.338768/>
#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

What this guide is about

This guide will show you how to annotate Lua code, i.e. how to add object types to your function parameters and variables.

Annotations are basically a special type of comment. They don't influence the execution of your code by any means, but just add documentation.

This is especially useful for function parameter types. Lua is not type-safe on its own, so documenting those yourself is a smart option, if you still want to understand today's code by next week.

Good thing, there are standards for Lua annotation, such as Emmy Annotation and it's successor LuaCATS.

The well-known sumneko.lua-extension for Visual Studio Code provides Intellisense features like automatic Syntax Checks and parameter preview based on your annotation.

Hence, this guide will be especially useful for you, if you plan to use the mentioned extension, but even if not, it would still drastically increase documentation quality of your code. Honestly, everybody should use it, including you.

See here, if you'd like to see a code resource that's fully annotated.

### Overview

### Quick Reference

### Function params

### Types and Classes

### Table- and Function-type params

### Union-Types and Values

### Generics

Motivation

Code-writing in Lua offers great opportunities, but also hat its flaws. Especially the missing type safety can lead to a frustrating experience for the unexperienced user, i.e. the fact that you cannot denote input and output parameter types in function definitions. World Editor Lua Syntax check will not and can not inform you about passing wrong-typed parameters or a wrong number of arguments to a function.

If you do such a mistake, it will just silently break your code. Ingame, when you expect your code to be executed, it might look like "nothing happened".

Even worse, sometimes after we come back to our Lua code after a break, we might not even remember what type a particular parameter in a given function was supposed to have. Maybe we have chosen a poor name for the parameter. Or the parameter needs to be a table with certain attributes, but what was it again? Now we end up reading through the function code again to re-catch that missing information.

The solution to this problem is, surprise, proper code annotation. Once annotated, IDE extensions like sumneko.lua will check your syntax for you and warn you, when you do something wrong.

Recommended: Installing VSCode + sumneko extension

A comprehensive guide to Mapping in Lua will show you how to properly setup Visual Studio Code and the sumneko-extension in its chapter IDE Setup. The same thread has definition files attached containing all blizzard natives with Emmy annotation / LuaCATS. Keep those in your VSCode workspace to enable function and parameter previews:

Sneak Peek

Consider the following function, which converts a boolean to a string:

 Lua:

---Boolean2String
---@param b boolean
---@return string
function B2S(b)
 if b then
 return "TRUE"
 end
 return "FALSE"
end

You can see that 3 lines of comments have been added on top of the function, providing a small description as well as input and output parameter types. That's how an annotation looks like.

You could argue that the function looks self-explanatory on its own and doesn't need further annotation, but trust me, this assumption fails every so often. If it does, you spend way more time being confused than annotation would have cost you in the first place.

Using VSCode with the mentioned extension will not only show the Boolean2String example with colorful syntax highlighting, but also provide parameter preview and other intellisense features:

Note that the grey :boolean-box in the function-brackets is not part of the code, but rather displayed by the sumneko-extension.

Below: Parameter preview based on the provided annotation.

It is definitely best practice to annotate every single function in your code, even local ones. It can feel annoying at first, but quickly becomes a satisfying routine that eventually will save a lot of time.

Again and to be clear, writing annotations does not influence execution of your code at all, because it is just a bunch of comments. It rather helps you (and your development environment) to see, what is going on.

Remember: The better your annotation is, the fewer bugs you will cause.

Refer to the other tabs to see all annotations available.

The following list summarizes the most useful annotations.

A complete list can be found here.

Please refer to the other tabs for more detailed descriptions and example code.

Function Params

Classes and Types

The following annotations are not restricted to ---@type, but also work with ---@param, ---@return and ---@field.

Advanced Table annotations

Advanced Function annotation

Union-type and Value annotation

Generics

Annotating function parameters is the most important part of the whole annotation thing, because erroneous function calls are the main reason for broken Lua code.

In JASS, parameter types were just part of every function definition and the Syntax Check effectively warned you, when you tried to call it with a wrong-type parameter. Lua syntax check doesn't do that, so we need to rely on our IDE (VSCode + sumneko) instead, which in turn relies on your annotation.

You see, how not producing type-based bugs pretty much boils down to proper annotation!

Annotation

Example:

Further Notes:Just skip the ---@return-annotation, if the function doesn't have return values.

You can specify multiple return values with multiple ---@return-statements (or one statement with comma-separated types).

Older versions of the sumneko-extension (2.X) didn't support ---@param ... type annotation. Instead, you had to use ---@vararg type.

If any parameter is a function itself, you might want to annotate that function's parameters as well instead of just writing ---@param name function. The same is true for tables with named entries. For that, please refer to the section for advanced typing.

By default, VSCode + sumneko extension knows about the standard Lua types (string, number, integer, float, boolean, function, table, userdata, thread, nil and the generic any).

All other types must be added by you, before they can be used in any annotation (see ---@class-statement below). That's also true for Warcraft native types like unit, location, etc.

As mentioned in the overview, A comprehensive guide to Mapping in Lua has definition files for common.lua and blizzard.lua attached, which also include ---@class-statements for all Warcraft types. So better use those instead of declaring them yourself

Annotation

Example:To comply with standard examples from university, let us assume we want to create Car-objects in your code. Cars are a special type of Vehicle and each Vehicle has a VehicleBrand among other attributes.

These object types can be annotated in way that is shown below.

Note how each declared class can immediately be used as a type in other annotations, like ---@type.

The same example denoted in a different flavor:

The second variant will have the same effect on the Intellisense features, but might have additional advantages for object oriented programming, if you plan to use the __index-metamethod.

Further Notes:The ---@type is only necessary to use, when the variable type is not clear by context. For example,

 is clear, because the IDE knows it is a string by the quotation marks.

 is clear, when the return value of the function f has been properly annotated.

The ---@field-annotation allows to specify the scope of the attribute (private, public, protected and package). This influences, where in your code the field will show up in attribute previews.

Example:

Not specifying a scope is the same as public scope.

The examples above are using the ---@type-annotation on the right of the table entries, but that's not the only way.

Using the line directly above is also fine and many people prefer to do so:

 Lua:

---@class Vehicle
Vehicle = {
 ---@type number
 id = 0,
 ---@type VehicleBrand
 brand = nil
}

Tables and functions are complex things on their own, so using the existing types function and table might not be enough to suit your needs. In fact, written code often depends on that the table passed to a certain function has certain attributes or that the passed function takes and returns a certain type.

The advanced methods below show, how these additional information can be integrated into annotations.

They are presented with the ---@type-annotation, but the same logic applies for ---@param, ---@vararg, ---@return and ---@field.

Annotations for tables

Annotations for functions:

Example:

We have cases in coding, where some parameter can have the one type or the other. For instance, assume a function that either takes a unit or a destructable. You can use the widget-type, but then how do you specify that items are not allowed?

This problem is even more likely to occur in Lua code than it was in Jass, due to the missing type-safety (or should I say due to the existing type-freedom at this point?  ).

Plus, we do have functions that don't take any value of a certain type, but you need to pass very specific values for it to work.

Like, consider a function that lets you play Rock, Scissors Paper with the computer - and you need to pass exactly one of the values "Rock", "Scissors" or "Paper".

The annotations below show how to specify that.

Again, the annotation style is shown with the ---@type-annotation, but the same logic applies for ---@param, ---@vararg, ---@return and ---@field.

Annotation

Example:

The example is obviously poorly coded, but still very educative.

Further Notes:In older versions of the sumneko extension (2.X), values always had to be engulfed by single quotation marks. That means the number 0 had to be annotated as '0' and the string "Hello World" as '"Hello World"'.

As shown in the example above, values and types can be mixed in the same union.

Specifying values will let VSCode prompt you the available options in a drop-down upon using the function, which is very useful to prevent typos:

Generics are a bit special. You basically define a name that denotes any type. You can use that name instead of the existing any-type for other annotations in the same context to show that they all require the same type.

A good example is the sorting-function in the example below. It can sort an array of any type, but several parameters are required to have the exact same type.

Annotation

Example:

Further Notes:As of sumneko-extension 3.X, you can also use generics in a class-context rather than just within functions:

 declares a LinkedList-Class that supports any type of element, which later allows you to write

 for a LinkedList of units.

This feature is not yet fully developed by the sumneko-extension, though. You will definitely encounter bugs here and there, such as missing method-preview for objects tagged with the type as above.

Let's hope that sumneko continues to stabilize over time.

Generic classes were unavailable in extension versions 2.X.


 Last edited: Feb 4, 2024

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Top tier structure.

Not written Lua before but will give it a read later today.

edit:

Looks good to me, I personally hate the idea of having to document my code to turn a not-strongly-typed language into one, it looks incredibly ugly.

Neither would I suggest editing in VS Code in any WC3 modding language unless there is automation to inject the code back into the map to avoid endless copy paste.

I guess that explains why I have not given lua a proper shot.

Anyway, that's not your fault.

The tutorial is helpful, neatly structured and written well, so approved.


 Last edited: Mar 25, 2022

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

I absolutely had the same feelings as you in the beginning.

Code looked more lengthy and clumsy with all the annotations, at least on first glance.

But trust me, this "ugly" feeling goes away incredibly quick and you start to really enjoy it once you see all the advantages you get from it.

For me personally, it even became nicer to read than a JASS function, because the parameters are so well organized line by line.

Regarding VSCode, what would you suggest as an alternative, even for JASSers?

Coding in the Wc3 editor itself clearly is not an option. And to my knowledge, we don't have Wc3 integrated development environments for Reforged. A bit of copy-pasting can be annoying, but it's far the least evil from my point of view.

If you want to automize the build process, there is Ceres, which seems to work great despite no longer being maintained.

#### Midnighters

#### Midnighters

 Level 22

JoinedJun 2, 2009Messages2,012

Resources: 35

#### Resources
Models 4Icons 22Packs 1Maps 3Spells and Systems 4Tutorials 1

Chaosy said:

 Top tier structure.

Not written Lua before but will give it a read later today.

edit:

Looks good to me, I personally hate the idea of having to document my code to turn a not-strongly-typed language into one, it looks incredibly ugly.

Neither would I suggest editing in VS Code in any WC3 modding language unless there is automation to inject the code back into the map to avoid endless copy paste.

I guess that explains why I have not given lua a proper shot.

Anyway, that's not your fault.

The tutorial is helpful, neatly structured and written well, so approved.


Click to expand...

Just as an FYI, you can save the map in folder format and directly edit the war3 lua file, saving it in vocode immediately updates world editor

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Midnighters said:

 Just as an FYI, you can save the map in folder format and directly edit the war3 lua file, saving it in vocode immediately updates world editor


Click to expand...

That's good info. I guess, you can't get around working in one single big file with your method?

Apart from having your own custom build process of course that is able to check for dependencies.

#### Midnighters

#### Midnighters

 Level 22

JoinedJun 2, 2009Messages2,012

Resources: 35

#### Resources
Models 4Icons 22Packs 1Maps 3Spells and Systems 4Tutorials 1

Eikonium said:

 That's good info. I guess, you can't get around working in one single big file with your method?

Apart from having your own custom build process of course that is able to check for dependencies.


Click to expand...

Basically, yeah. That's the unfortunate part, as the entirety of the map code is in one gigantic file. Every direction has a drawback it seems. I choose the copy and paste into editor route.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Midnighters said:

 Just as an FYI, you can save the map in folder format and directly edit the war3 lua file, saving it in vocode immediately updates world editor


Click to expand...

That's super useful, wont convince me to go lua since wurst and typescript exist but for those that do, this is a great quality of life.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Midnighters said:

 Just as an FYI, you can save the map in folder format and directly edit the war3 lua file, saving it in vocode immediately updates world editor


Click to expand...

Sounds wierd. war3map.lua is the resulting map script file. Won't one lose the changes, after one presses the save button in world editor. Which one needs to do when one wants it as map.

#### Midnighters

#### Midnighters

 Level 22

JoinedJun 2, 2009Messages2,012

Resources: 35

#### Resources
Models 4Icons 22Packs 1Maps 3Spells and Systems 4Tutorials 1

Tasyen said:

 Sounds wierd. war3map.lua is the resulting map script file. Won't one lose the changes, after one presses the save button in world editor. Which one needs to do when one wants it as map.


Click to expand...

From what I found, as long as you save the map, then edit the file, it should update in the map, but I don't mess with it enough to know for certain. I will fiddle with it tomorrow and see what I find.

#### Jampion

#### Jampion

 Level 16

JoinedMar 25, 2016Messages1,327

Resources: 1

#### Resources
Spells and Systems 1

Thanks, I have been using the vscode extension for a while, but didn't know about the Emmy Annotation. Very useful!

One question though, it seems inheritance isn't fully understood:

...

This is from Definitive Doubly-Linked List, I think you should be familiar with it. from is LinkedListHead, which inherits from LinkedList.

loop is from LinkedList, but I get the warning that LinkedListHead does not have loop.

Do I need to change some settings or maybe add loop in the Emmy Annotation?

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Jampion said:

 Do I need to change some settings or maybe add loop in the Emmy Annotation?


Click to expand...

Good find. I personally consider this a bug. You can report sumneko extension bugs here, if you like.

I'm not aware of any settings related to the problem.

What instead seems to fix the issue, is to move the LinkedList = {}-line directly below the class definition:

 Lua:

---@class LinkedList
---@field head LinkedListHead
---@field next LinkedList
---@field prev LinkedList
LinkedList = {}

---@class LinkedListNode : LinkedList
---@field value any

---@class LinkedListHead : LinkedList
---@field n integer

LinkedList.__index = LinkedList

I reckon that separating LinkedList = {} from the ---@class-annotation overloads the meaning of LinkedList instead of merging the contents, which lets inheritance refer to the class and its fields instead of the table and its methods.

It's still a bug, of course.

Funny enough, I haven't ever encountered that problem before, because I tend to use the object-oriented table style for defining fields:

 Lua:

---@class LinkedList
LinkedList = {
 head = nil ---@type LinkedListHead
 , next = nil ---@type LinkedList
 , prev = nil ---@type LinkedList
}

This method doesn't yield the error you brought up, because properties and methods are part of the same table and there is no chance to accidently overload.

Edit: For everyone's interest, sumneko's Git repo also contains a wiki mentioning all annotations available. It includes annotations that I haven't covered in my tutorial, although several of them are more edge-case-related and not really useful for Wc3 modding. I need to credit that in my tutorial (didn't know the wiki at the time I wrote it) and also adapt to the changes made in sumneko version 3.X. Don't have much time currently, but will try to update by end of month.

Edit 2: Trivia: I just realized that sumneko himself is part of the chinese Warcraft mapping community and even stores the code of his map in his git


 Last edited: May 7, 2022

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

Today I went and updated a common library I had when I noticed that my custom global functions in a "do end" block don't have any emmy annotation, nor even the description.

This is what I mean:

The "end" keyword is far below in the script.

It doesn't show the type of the parameters when I open brackets, only their names. Doesn't even recognise it as a function.

The only time it functions as it should is if I disable the "do end" block.

Feels like it might have been an update of the extension? v3.5.3 of sumneko. Or some settings messed up?

Please give me some insight, annotation master @Eikonium

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

@Wrda

Looks like an extension bug to me. Those things happened fairly often to me in the past (basically every second version in the 2.x era was heavily bugged).

sumneko was fairly good in reintroducing bugs he had already solved before

Of course, a few bugs here and there are pretty much expected for an extension in active development.

I eventually decided to stay on stable version 2.3.7. That version is very fast in processing my code, doesn't show any bugs (at least I haven't found any) and has different syntax highlighting for local and global variables (which is really helpful, but was deprecated in version 3.x).

You could try going back some versions (or just to 2.3.7) and see, if it solves your issue (you need to restart VSCode afterwards).

Apart from that, the error you show can also occur, if your working file and definition file are located in different workspaces (different file is ok, different workspace is not).

Lastly, there are settings for how much data the extension can load at once, which might theoretically be too small in your case. But I don't think that's the issue. I'd rather start with trying version 2.3.7.

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

Eikonium said:

 You could try going back some versions (or just to 2.3.7) and see, if it solves your issue (you need to restart VSCode afterwards).

Apart from that, the error you show can also occur, if your working file and definition file are located in different workspaces (different file is ok, different workspace is not).

Lastly, there are settings for how much data the extension can load at once, which might theoretically be too small in your case. But I don't think that's the issue. I'd rather start with trying version 2.3.7.


Click to expand...

By definition file, you mean blizzard.lua and common.lua? My definition files at the same level as the working file.

I did revert to 2.3.7, and this error occured:

Too large file: common.lua skipped. The currently set size limit is: 100KB, and the file size is: 392.918 KB.

So it's not recognising half or more of the natives, and bjs.

It might be just what you described on last paragraph, but I'm not so sure where to fix it. Is it Preload file size?

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

By defnition file, I was referring to the file containing the function definition of Parabola. But I just noticed by the screenshots that it was the same file you were testing the parameter preview in, so different file storage was probably not the reason for your error.

Try the following settings:

 Code:

 "json.maxItemsComputed": 30000,
 "Lua.workspace.preloadFileSize": 2000,
 "Lua.workspace.maxPreload": 1000

(I have probably randomly chosen these numbers in the past, lol)

This post shows how to get to the settings file of VSCode, if you are unsure

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

Beautiful! Now just a slight inconvenience, but it is possible to modify this greyed words?

Also FourCC doesn't seen to be integrated in common.lua, so it gives error as undefined global.

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Wrda said:

 Now just a slight inconvenience, but it is possible to modify this greyed words?


Click to expand...

Nice! The grey ones are variables in global scope, the light blue ones are local scope. This difference is exactly what I favor in version 2.3.7 in contrast to 3.x

I can often spot spelling or scoping issues real quick just because the color is wrong (like you mistype on a local variable name and the result is grey, so you immediately know something is not right).

If you just don't like grey color, I suppose it's coming from the VSCode color scheme you have chosen (probably the default scheme).

I haven't tried to change single colors in a scheme so far (just the scheme in total), but would probably go for this guide.

Wrda said:

 Also FourCC doesn't seen to be integrated in common.lua, so it gives error as undefined global.


Click to expand...

Yeah, FourCC is a hidden native, just like __jarray, UnitAlive etc. I should probably attach an emmy annotated definition file for all hidden natives to Debug Utils (and update the existing definition files to patch 1.33.X), but I don't actually have a complete list (wasn't there one on hive? I can't seem to find it) and am prioritizing other ToDo's currently

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

I guess I'm just too used to the 3x series xD, but it doesn't mind me that much anyway.

About the hidden natives, yeah I think there was one list going around, but hard to find now. Luashine or Drake53 most likely can give it quickly

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Forgot to mention that you can add this FourCC definition to common.lua, until I've uploaded a proper definition file for hidden natives.

 Lua:

---Converts a 4-digit string representing an object id (e.g 'a0cd') to an integer.
---@param typeId string
---@return integer
function FourCC(typeId) end


 Last edited: Aug 26, 2022

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Guide update:

Updated annotations, descriptions and screenshots to reflect the standard now used by version 3.X of the sumneko extension (was 2.X when I wrote the guide).

Updated links to other resources, such as the sumneko-extension's annotation wiki.

Improved examples, writing and overall presentation.

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Small Update:

The guide and it's title now reflect the new name LuaCATS that sumneko has chosen to succeed Emmy Annotation.

 You must log in or register to reply here.


### Similar threads

Locked

 [Lua] Teal - typed dialect of Lua


Aniki

Jun 2, 2020

Advanced Scripting

Replies8Views5K

Jun 27, 2020
WaterKnight

Locked

 Text inside a .highlight tag still parses "@soemthing" as a user tag


HappyTauren

Feb 2, 2018

Site Discussion

Replies5Views2K

Feb 3, 2018
Kyrbi0

[Lua]
 Cache


Insanity_AI

Nov 28, 2023

Old Submissions

Replies1Views2K

Nov 29, 2023
Bribe

 [Lua] Switching from vJass to Lua (structs, methods, modules and more)


Bribe

May 6, 2022

Scripting Tutorials

Replies4Views5K

Aug 10, 2022
Bribe

 A comprehensive guide to Mapping in Lua


Eikonium

Sep 18, 2022

Lua Tutorials
2

Replies81Views37K

Apr 19, 2026
KuWe

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

Lua Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top