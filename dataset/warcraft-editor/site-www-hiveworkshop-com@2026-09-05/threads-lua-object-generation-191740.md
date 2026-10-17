---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/lua-object-generation.191740/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Lua Object Generation
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/lua-object-generation.191740/>
#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Lua Object Generation

Disclaimer:This tutorial was written for patch 1.26, prior to Lua being introduced as a scripting language option in Reforged. If you would like to run a legacy Lua script and it is not working for your reforged map, it may be worth trying to run the script on a separate map (using an older patch/set of tools) and then importing the generated object data into your reforged map.

Description:

What is Lua?

Lua is a programming language that was made to be light, simple, and flexible. It was made to be embeddable into application programs to create a simple interface for scripting, and that is exactly what it will serve as for this tutorial.​

How does it relate to Warcraft III?

With the introduction of jAPI, by xttocs, came a long line of hacks for Warcraft III. Thanks to PipeDream and PitzerMike, Jass NewGen Pack now has a pack of extensions integrated into its editor which enables several unique features for map making. The one we'll be looking at is the Object Merger.​

What is the Object Merger?

The object merger is a plugin that allows you to manage object data between maps as well as create scripts that will automatically generate objects. By objects, I mean those available in the object editor; units, items, destructables, doodads, abilities, buffs, and upgrades. While standard object data editing can prove to be faster, the object merger can make implementation a breeze for systems which require custom object data, and it can allow mass production of objects very quickly.​

​

Note that the GrimExt tools can only be used for editing. They are not able to be used during the game, only for ease of map making and implementation.​

Requirements:

Jass NewGen Pack

I recommend that you download this hotfix for GrimExt that allows the ObjectMerger to work properly on certain fields that involve checkboxes. [Link]

Basic knowledge of the Object Editor.

Make sure you have the object editor hack enabled. (Grimoire -> Enable object editor hack)

Table of Contents:

Part I - Setup

Part II - The Object Merger

Part III - Integrating Lua

Part IV - Multiple Objects

Part V - Advanced Lua

Part VI - Lua Scripts

Part VII - GetVarObject

Part VIII - Writing Custom Lua Scripts

FAQ

Known Bugs

Part I - Setup

There may be many questions running through your head at this point. How do you access Lua scripting? Where do you put the code? How do you generate the data?

We will be doing all of our scripting in the Trigger Editor. First, I recommend you create a new trigger. Go to Edit -> Convert to Custom Text.

Erase all the code that appears. It should be completely blank.

Part II - The Object Merger

Now let's get started! First of all, what let's the compiler know that you are not scripting normal JASS anymore? This is it:

 JASS:

//!

This is the command to access the external preprocessor. It will allow us to execute scripts using specific tools/extensions that we specify. To access a specific tool or extension, we use this syntax:

 JASS:

//! external <TOOL_NAME> <ARGUMENTS>

For the object merger, "<TOOL_NAME>" will be replaced with "ObjectMerger".

 JASS:

//! external ObjectMerger

Now we must input the arguments. As explained before, the object merger can be used to generate objects. However, the syntax is a little complicated. Open the object editor and you will notice that there are several object types: units, items, destructables, doodads, abilities, buffs, and upgrades. Each type has a rawcode that is associated with it:

w3u Units

w3t Items

w3b Destructables

w3d Doodads

w3a Abilities

w3h Buffs

w3q Upgrades

This will tell the object merger what we are going to make. In this example, I will tell the compiler that I am going to make an ability.

 JASS:

//! external ObjectMerger w3a

Each space represents the starting of a new argument. In this case, the first argument we put is the object type, w3a (abilities).

The next argument will be the rawcode of the base object and the rawcode of the object we are making. This is the basic syntax so far:

 JASS:

//! external ObjectMerger <OBJECT TYPE> <BASE OBJECT ID> <CUSTOM OBJECT ID>

If the custom object ID is already used, then it will replace the object using that ID. For example, let's say we want to base an ability off of Aerial Shackles.

Our concern is for 'Amls', also known as the rawcode ID of the object. We will replace <BASE OBJECT ID> with this.

 JASS:

//! external ObjectMerger w3a Amls <CUSTOM OBJECT ID>

Note that there is no ' ' denotation for these rawcodes. The next argument determines the custom object ID, which will be the rawcode ID of the object we will be creating. I recommend you use a unique one, but it can be anything you'd like. In this example, I'll use 'A000'.

 JASS:

//! external ObjectMerger w3a Amls A000

Next, open the object in the object editor and you'll notice it has several fields. These fields modify a certain aspect of an object, whether it be its scaling or its name.

The first part we want to pay attention to is the left column. You might notice that each field will have a 4 characters following the field name enclosed in parenthesis. This is the rawcode of the field.

When creating objects, we can modify fields by inputting the rawcode of the field and following it with its corresponding value. The syntax for the object creation so far will be:

 JASS:

//! external ObjectMerger <OBJECT TYPE> <BASE OBJECT ID> <CUSTOM OBJECT ID> <FIELD ID 1> <FIELD VALUE 1> <FIELD ID 2> <FIELD VALUE 2> ...

The first thing you will probably want to modify is the name. The rawcode of this field is 'anam', so it would look like this:

 JASS:

//! external ObjectMerger w3a Amls A000 anam

However, that is missing the corresponding field value.

When we are entering the name, we will enclose it in quotations "". You will input most things in quotations, aside from integers and reals. Anyway, let's name our ability Fire.

 JASS:

//! external ObjectMerger w3a Amls A000 anam "Fire"

Now it gets a bit more complicated. You may get some errors from saving for certain fields if you just follow that syntax. Remember how abilities have different levels? Well, for one level, the tooltip may be something, but for another, it may be something else. This external call requires us to input the level that we want to modify before modifying the field. This will apply to only certain fields. (the ones that gain extra fields when you change the amount of levels) Even if there is only one level, you must input a level to modify. The syntax is this:

 JASS:

<FIELD ID> <LEVEL> <VALUE>

For example, if I wanted to modify the targets allowed:

 JASS:

//! external ObjectMerger w3a Amls A000 anam "Fire" atar 1 "Air,Enemy,Organic,Neutral"

It modifies the field for level 1 of the ability to allow "Air", "Enemy", "Organic", and "Neutral" as targets. Notice how each value is separated by a comma. Now let's finish this spell off by making it a mana cost of 15.

 JASS:

//! external ObjectMerger w3a Amls A000 anam "Fire" atar 1 "Air,Enemy,Organic,Neutral" amcs 1 15

This sets the mana cost of "Fire" (Level 1) to 15. To process this script, simply save the map and reopen it. (You do not have to close the entire editor, just the map) When you reopen it, you will notice the object is there. However, you may notice that the mana cost wasn't changed. For some odd reason, the object merger may neglect the final command, so it is best to add a | symbol at the end to let it know that you are done.

 JASS:

//! external ObjectMerger w3a Amls A000 anam "Fire" atar 1 "Air,Enemy,Organic,Neutral" amcs 1 15 |

Now you can save, reopen, and the ability should be there! You can add more and more fields, but the more you add, the more lengthy it gets. Overall, this is an ugly and long method of modifying objects. Not only that, but it is very slow as well. That is why you will learn how to do this with Lua.

Part III - Integrating Lua

Lua will allow you to create the scripts shown above, but in a readable, efficient, and more flexible manner. Not only that, but this can allow you to utilize simple functions, such as loops and variables.

First of all, Lua is created within external block commands.

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //All the lua script goes within this block
//! endexternalblock

This will allow you to use Lua where the comment is. However, we will not simply use //!, we will have to use //! i instead, as that denotes the start of a Lua call.

Creating objects in Lua has its differences with the single-line external call, but for the most part, it is very similar. First off, let us define what object type we will be creating.

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! i setobjecttype("abilities")
 // You can also use "units", "items", "upgrades", "buffs", or "destructables"
//! endexternalblock

In the case above, I will be creating an ability. The function //! i setobjecttype("") will determine what object type the succeeding code will concern.

With Lua, you have a function to modify an object and a function to create an object.

 JASS:

//! i modifyobject("baseID")
//! i createobject("baseID","customID")

When modifying an object, you simply need to provide the rawcode ID of the object to modify. For example, if you wanted to modify Control Magic, you would use:

 JASS:

//! i modifyobject("Acmg")

Creating an object is the same as we did with the single-line external call. It will create a new object based off of an existing one, and provide a custom ID. For the example we did for the single line, it would be:

 JASS:

//! i createobject("Amls","A000")

That will create an ability with the rawcode 'A000' based off of Aerial Shackles (Amls).

After that, it is just a matter of a single function:

 JASS:

//! i makechange( object , field , value )

The object in that function is special. You can have several different input for that:

current - Whenever you use "createobject" or "modifyobject", current will equal the object returned. So if you created an object based off of Aerial Shackles, current would now represent that object that was created. For modifying, it is the same thing.

custom - This refers to all custom objects of a type in the map. The type that it chooses from is determined by //! i setobjecttype("").

original - This refers to all original (non-custom) objects of a type in the map. The type that it chooses from is determined by setobjectype as well.

modded - This refers to all original objects of a type in a map that were modified. The type that it chooses from is determined by setobjectype as well.

unmodded - This refers to all original objects of a type in a map that were not modified. The type that it chooses from is determined by setobjectype as well.

That argument will not be enclosed in quotations.

The next argument determines the rawcode ID of the field it will change. The same rules apply as that of the ones in the external call. However, the rawcode will be placed in quotes. Example:

 JASS:

//! i makechange(current,"anam",value)

The next field may or may not be the level. If it has different values depending on the level, then it will have the level as a field. Otherwise, you do not include that as an argument. For example:

 JASS:

//! i makechange(current,"amcs","1","15")

That will change the mana cost of level 1 of the ability to 15.

The final field is the actual value itself. You can always enclose this within quotations, which makes it less of a hassle to know whether to use or not to use them.

 JASS:

//! i makechange(current,"anam","Fire")

Now let's recreate the ability that we made for the external call.

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! i setobjecttype("abilities")
 //! i createobject("Amls","A000")
 //! i makechange(current,"anam","Fire")
 //! i makechange(current,"atar","1","Air,Enemy,Organic,Neutral")
 //! i makechange(current,"amcs","1","15")
//! endexternalblock

Simple, right? For every field you want to change, you simply add another line with the appropriate values. After you have generated the object, closed the map and reopened it (assuming all went fine), you can feel free to remove the script. Once it is in the object editor, it is fine to remove the script, because otherwise it will regenerate it each time upon saving, which can increase save times by a great deal.

Now there are still some things to note. Let's say you wanted to change the icon of the ability:

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! i setobjecttype("abilities")
 //! i createobject("Amls","A000")
 //! i makechange(current,"anam","Fire")
 //! i makechange(current,"aart","ReplaceableTextures\CommandButtons\BTNAntiMagicShell.blp")
 //! i makechange(current,"atar","1","Air,Enemy,Organic,Neutral")
 //! i makechange(current,"amcs","1","15")
//! endexternalblock

If you try that, you'll notice that your ability now has no icon. The thing about entering strings is that "\" counts as a character escape sequence. (look up string literals) For the backslash to be properly input, we must put a double-backslash "\\" instead:

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! i setobjecttype("abilities")
 //! i createobject("Amls","A000")
 //! i makechange(current,"anam","Fire")
 //! i makechange(current,"aart","ReplaceableTextures\\CommandButtons\\BTNAntiMagicShell.blp")
 //! i makechange(current,"atar","1","Air,Enemy,Organic,Neutral")
 //! i makechange(current,"amcs","1","15")
//! endexternalblock

That should work properly.

Part IV - Multiple Objects

In case you were wondering, it is possible to create multiple objects in a single block. As I said before, the function //! i sectobjecttype("") will modify the type for all succeeding functions. However, calling it again will override the last one and change the type for all of its succeeding functions. For things like //! i createobject("","") or //! i modifyobject(""), the same thing applies. Here is an example that creates one unit and one item:

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! i setobjecttype("units")
 //sets the object type to units
 //! i createobject("hfoo","h005")
 //creates a unit with ID 'h005' based off of a footman
 //! i makechange(current,"unam","Soldier")
 //names it "Soldier"
 //! i makechange(current,"ua1c","2.32")
 //sets the attack cooldown to 2.32
 //! i makechange(current,"umvs","360")
 //sets the movement speed to 360

 //! i setobjecttype("items")
 //sets the object type to items
 //! i createobject("rat9","I043")
 //creates an item with ID 'I043' based off of claws of attack +9
 //! i makechange(current,"unam","Armor +4")
 //sets the name to "Armor +4"
 //! i makechange(current,"iabi","AId4")
 //sets the abilities as "Item Armor Bonus (+4)"
 //! i makechange(current,"ides","Increases armor by 4.")
 //sets the item description
//! endexternalblock

Part V - Advanced Lua

This all seems pretty simple, but one thing is that it is kind of long. It is easy to copy and paste over and over, but is there any way to make it become more compact?

Well, yes there is. Lua is a standalone language, so it shares many of the features that normal programming languages involve.

Variables - Lua allows you to make variables wherever you want. Unlike JASS, you do not specify a type.

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! i local someVar = 0
 //! i local otherVar = "whee"
 //! i lalalulu = "this works too, this is a global variable though"
//! endexternalblock

In Lua, you can also make arrays.

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! i myArrayVariable = {}
 // = {} will make the variable an array
 //! i myArrayVariable[1] = 3
 //! i myArrayVariable[2] = "this works as well"
//! endexternalblock

Looping - For object creating, sometimes it is nicer to make just a long loop to do things for you. Of course, there are both "for" and "while" loops.

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! i local var = 5
 //! i for i=1, 5 do
 //! i var = var * 2
 //! i end
 //! i var = 0
 //! i while (var < 5) do
 //! i var = var + 1
 //! i end
//! endexternalblock

One thing to note is that if you run an infinite loop with Lua, your map will be dead. Always keep a backup before executing Lua scripts, especially when it comes to your own scripts. For safety, you may want to stick with "for" loops unless you need to do a "while" loop, because the latter is a bit more prone to infinite loops compared to the first one.

Functions - You can also make functions to call to simplify things. Many people seem to just use textmacros, but functions exist, so why not use it?

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! i function whee( argument, argument2 )
 //! i argument = argument + 1
 //! i return argument
 //! i end

 //! i print(tostring(whee(5)))
//! endexternalblock

The arguments are enclosed within parenthesis () and have commas as separators for individual arguments. To execute a function, just call its name and input the appropriate arguments.

If/Then/Else Blocks - These are pretty much self explanatory.

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! i local var = math.random(0,100)
 //! i if var < 10 then
 //! i print("var is less than 10.")
 //! i elseif var < 60 then
 //! i print ("var is less than 60 but greater than or equal to 10.")
 //! i else
 //! i print ("var is greater than or equal to 60.")
 //! i end
//! endexternalblock

For more information on Lua itself, see Programming in Lua.

All of the above can be put to use for object creation.

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! i function createmonsoon(name, damage)
 //! i local rawcodes = {}
 //! i setobjecttype("abilities")

 //! i rawcodes[1] = "A000"
 //! i rawcodes[2] = "A001"
 //! i rawcodes[3] = "A002"

 //! i for i=1, 3 do
 //! i createobject("ACmo", rawcodes[i])
 //! i makechange(current, "anam", name .. " [" .. i .. "]")
 //! i makechange(current, "Esf1", "1", damage)
 //! i end
 //! i end

 //! i createmonsoon("Monsoon", 5)
//! endexternalblock

This will create 3 monsoon abilities with names "Monsoon [1]", "Monsoon [2]", and "Monsoon[3]" all with 5 damage. It is a rather useless example, but it shows what is possible when doing this.

You might have noticed the  name .. " [" . Unlike JASS, concatenation of strings follows the syntax:

 JASS:

//! i string = "He" .. "ll" .. "o" .. " W" .. "or" .. "ld" .. "!"

Which would have a value of "Hello World!". Two periods between two strings will concatenate the two together into one.

Part VI - Lua Scripts

Nestharus has developed several scripts which can be used to create dynamic scripts, delete them, and execute them all with great ease! The installation is a breeze, and once it is installed, it can be used in any map. (for most scripts)

The first thing you will want to do is get LUA_FILE_HEADER. Follow the directions he posted at the top. Basically, change the FILE_NAME to whatever the name of your map is. For example, if my file name was Lua Fun Time.w3x, then it would be:

 JASS:

 //! i local FILENAME = "Lua_Fun_Time"

 //! i function getfilename()
 //! i return FILENAME
 //! i end

And this line would be:

 JASS:

//! import "luajass.Lua_Fun_Time.j"

If you follow the instructions correctly, you then simply need to save for initialization, and then comment out the initialization and uncomment the import line once you have saved the map.

To make sure everything went correctly, check your jassnewgenpack5d -> grimext -> luadir folder. If done correctly, it should show something along the lines of FILE_NAME_dir as a folder. For the one I showed above, it should simply be Lua_Fun_Time_dir.

After that, keep the header in your map. Although it is not necessary, you will usually want this in your map so you can access/install external scripts when needed. If you have external JASS scripts/globals to use in your map, then the header must be in your map for everything to work properly.

How do you install a script? Well, it is very simple. I am going to use [Snippet] LUA_OBJECT_ID as an example.

 JASS:

//GetObjectId 1.0.0.5
//! externalblock extension=lua FileExporter $FILENAME$
 //! runtextmacro LUA_FILE_HEADER()
 //! i writelua("GetObjectId", [[
 //////////////////////////////////////////////////////////////////
 //code

 //! i function getobjectid(obj, objecttype)
 //obj refers to the base object
 //"hpea", "Amov", "Bphx", etc
 //objectType refers to the type of object to create

 //! i if (currentobjecttype() ~= objecttype) then
 //! i setobjecttype(objecttype)
 //! i end
 //! i local object = generateid(obj)
 //! i while (
 //! i objectexists(object) or
 //! i string.find(object, "'", 1, true) ~= nil or
 //! i string.find(object, '\\', 1, true) ~= nil or
 //! i string.find(object, ',', 1, true) ~= nil or
 //! i string.find(object, '/', 1, true) ~= nil) do

 //! i object = generateid(obj)

 //! i end
 //! i return object
 //! i end

 //end code
 //////////////////////////////////////////////////////////////////
 //! i ]])
//! endexternalblock

Just copy that text and paste it into an empty trigger. Make sure you have the LUA_FILE_HEADER in your map before installing. Afterward, just save and you can feel free to remove the code afterward! Yes, it is that simple. You can check if it is installed by going to the jassnewgenpack5d -> grimext -> luadir folder and seeing if it is installed there:

After that, simply proceed to install all the other scripts. Here I have a list of the Lua scripts posted on this site so far:

[Snippet] LUA_FILE_HEADER

[Snippet] LUA_OBJECT_ID

[Snippet] LUA_GET_VAR_OBJECT

[Snippet] LUA_TRIM

[Snippet] LUA_STRING_EXPLODE

[Snippet] LUA_FIND_LINE

[Snippet] LUA_FILTER_LINE

[Snippet] LUA_SERIALIZE_TABLE

[Snippet] LUA_DUMMY_PHYSICAL_ABILITY

[Snippet] LUA_NEW_BUFF

I recommend that you install all of those scripts if possible, because then you won't need to later on.

Part VII - GetVarObject

This script will be used a lot, so it is best to go over it to explain how it works.

[Snippet] LUA_GET_VAR_OBJECT

This library should be used for object generation, as it will ensure that all rawcode ID's used will be unique. This will prevent existing abilities from being modified by mistake, and can make object generation for systems become more fool-proof. Not only that, but it removes the need for any rawcode input for functions, which can simplify things even further.

 JASS:

//! i function getvarobject(base, objtype, varname, import)

This function will return a unique ID. However, it has arguments that we must input values for, so I'll explain them:

base - When creating a new object, you first must base it off of another object. That "base" object is the one referred to in the parameters, so we simply input the rawcode of it. For example:

//! i local myVar = getvarobject("hfoo", objtype, varname, import)

That first argument will specify that we will be basing the new object off of the one associated with the rawcode 'hfoo' (footman).

objtype - This is the object type to be created. This parameter essentially replaces //! i setobjecttype("type"), as the function will do it for you. For the example above, we would continue with the argument "units", since we are creating a new unit based off of the footman.

//! i local myVar = getvarobject("hfoo", "units", varname, import)

varname - When creating an object, the rawcodes can become quite messy and annoying to look up. By inputting a value for varname, a new constant global integer will be made, set to the new rawcode. So if you input "UNITS_FOOTMAN", it will create a global with that name. Generally, you will want to use all uppercase, and follow the convention of TYPE_VARNAME. There is no difference, as it is all personal preference, but it should be that format when submitting lua scripts.

//! i local myVar = getvarobject("hfoo", "units", "UNITS_FOOTMAN", import)

import - The final parameter will determine whether or not to import the global variable of the name you input for varname. "true" will have the global imported, while "false" will not.

//! i local myVar = getvarobject("hfoo", "units", "UNITS_FOOTMAN", "true")

//! i updateobjects() is pretty important as well. After generation, you will want to be sure to update the objects, which will make sure that the globals are made and imported. Just call that function at the end of the generation to successfully make the globals.

Part VIII - Writing Custom Lua Scripts

Writing custom Lua scripts are pretty easy as well. Let's make a simple script that includes a function that creates several destructables with different pitch/roll angles.

First, we'll start off with the preliminary setup for writing a script:

 JASS:

//! externalblock extension=lua FileExporter $FILENAME$
 //! runtextmacro LUA_FILE_HEADER()
 //! i writelua("PitchRollDests", [[

 //! i ]])
//! endexternalblock

"PitchRollDests" is the script name. Note that the extension block actually accesses the FileExporter, as it has a lighter overhead than the object merger. So let's make a simple function that creates a destructable based off of 'LTbx' (Barrel) and allows you to modify the pitch/roll/name.

However, let's say you want to have it automatically generate a unique rawcode. Luckily, the script GetVarObject exists. So how do we include it? We simply use dofile(""), like so:

 JASS:

//! externalblock extension=lua FileExporter $FILENAME$
 //! runtextmacro LUA_FILE_HEADER()
 //! i writelua("PitchRollDests", [[

 //! i dofile("GetVarObject")

 //! i ]])
//! endexternalblock

This will allow us to use its contents when running the script. So let's finish up the function I just talked about:

 JASS:

//! externalblock extension=lua FileExporter $FILENAME$
 //! runtextmacro LUA_FILE_HEADER()
 //! i writelua("PitchRollDests", [[

 //! i dofile("GetVarObject")

 //! i function barrelroll(pitch, roll, name)
 //! i local dest = getvarobject("LTbx", "destructables", "DESTRUCTABLES_" .. name, false)
 //! i createobject("LTbx", dest)
 //! i makechange(current, "bnam", name)
 //! i makechange(current, "bmap", pitch)
 //! i makechange(current, "bmar", roll)
 //! i end
 //! i ]])
//! endexternalblock

Note the getvarobject use. It will automatically use "setobjecttype" to define what type we will be creating. It takes the base ID of the object we are basing our custom one off of, then it takes the category, then it takes the global name, and finally whether or not to import the globals. I figured that importing the globals for this would be unnecessary in this case, so I put "false" as the input for that field.

Now let's create a function that will automatically make several different objects of that destructable to have different pitch and roll values up to -6.28 (-2*pi), which is one full rotation all the way back to the default pitch/roll.

 JASS:

//! externalblock extension=lua FileExporter $FILENAME$
 //! runtextmacro LUA_FILE_HEADER()
 //! i writelua("PitchRollDests", [[

 //! i dofile("GetVarObject")

 //! i function barrelroll(pitch, roll, name)
 //! i local dest = getvarobject("LTbx", "destructables", "DESTRUCTABLES_" .. name, false)
 //! i createobject("LTbx", dest)
 //! i makechange(current, "bnam", name)
 //! i makechange(current, "bmap", pitch)
 //! i makechange(current, "bmar", roll)
 //! i end

 //! i function barrelgen(name, count)
 //! i local factor = 6.28/(count-1)
 //! i local angle = 0
 //! i for i=1, count do
 //! i barrelroll(angle, angle, name .. " [" .. i .. "]")
 //! i angle = angle - factor
 //! i end
 //! i end
 //! i ]])
//! endexternalblock

There we go, now we should be done. Simply save, and it should install the script. Afterward, just remove the text (or disable the trigger it is in) and then test it out. Note that the script above just provides functions for the object generation, it does not create the objects themselves. Thus, you have to make your own script which calls the function to actually generate objects. This is the script I have used, which creates 10 of the barrels with different pitch/roll angles:

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! runtextmacro LUA_FILE_HEADER()
 //! i dofile("PitchRollDests")

 //! i barrelgen("BarrelRolls","10")
 //! i updateobjects()
//! endexternalblock

This one now uses the ObjectMerger as the tool, instead of the file exporter. Once you have done this, simply save, close the map, and reopen. Delete the script for generation and then check if it is in the object editor. If everything went correctly, then this should be the result:

If it did work, then pat yourself on the back. If not, then do not worry. Simply debug the script until it works the way it should.

These dynamic scripts make it very easy to do things very quickly. In my example above, you can generate destructables with different pitch/rolls very easily, allowing for some cool animations if you link them together. If you have a spell that requires something along these lines, it is a pain for the user to have to copy and paste each individual object into their map. With Lua scripts, that becomes a thing of the past.

You may be wondering why the lua scripts have all uppercase titles. Before, the lua resources used to be all textmacros. Now that LUA_FILE_HEADER exists (which is still a textmacro), there was no longer a need for other textmacros. It is just a convention for the scripts, and recommended to be used if you are submitting a lua script.

FAQ:

Can I create objects during the game by utilizing Lua scripts?

Sadly, no, that is not possible. Lua scripts are strictly for map editing purposes. It is not able to affect gameplay.

I keep getting this error:

That is the generic error message for when the script does not follow proper syntax. The actual error log will appear in jassnewgenpack5d -> logs -> grimex.txt. However, the error messages are usually very generic, so you have to painfully debug and make sure that everything is written correctly.

The file exists!

This error caused me a lot of distress, as well as about 2 other people total. I am not sure of what causes it to occur, but it has to do with improper Lua scripting. However, the fix is simple, so have no fear. When the lua scripts are made, they are made in the "temp" folder. (X:\Users\$USERNAME$\AppData\Local\Temp) Some file within that folder ends up causing the error. The fix is to simply delete that folder. It may take a while, as there are usually around ~70k files in there. Another fix could be to get a PC Clean tool, which usually will clean out your temp folder of unwanted things. That would probably be the best route, as sometimes you may get halfway through deleting until you see an error that says "This program is in use by another program", in which it will restore all the data you were trying to delete. In terms of safety, it should be fine to delete your temp folder. If you have any worries, just use a PC Clean tool, and it will do it for you.

Do I have to keep Lua scripts in my map?

It is suggested to keep the file header in your map (or else syntax errors may occur), but for the most part, the rest can be deleted. It depends on the script, but normally things can be deleted after you have saved and reopened the map.

How safe is using Lua?

Use it with caution. I recommend using it on blank test maps prior to using it on the actual map, and creating backups of the map before running lua. This is just a caution-measure in case an infinite loop is ran, which will destroy your map. An even better solution is to use a source code editor that supports Lua, such as NotePad ++.

What are the other tools I can use for GrimExt?

Go to jassnewgenpack5d -> grimext and open up GrimExManual.html. It should explain some of the other tools to give a brief overview.

Are there any more functions I can use for Lua?

As stated in the tutorial, Lua is a whole other programming language. Thus, it has a lot of features similar to other programming languages. See Programming in Lua for more information.

Why doesn't my object editor show rawcodes next to the fields?

Make sure you have the map opened in NewGen, and have Grimoire -> Enable object editor hack checkmarked.

Known Bugs:

So far, the only bug I have encountered is the very bug WaterKnight mentioned in this thread: ObjectMerger Channelability

The ability "Channel" has checkmark boxes for its options. By default, it is invisible. You have to pull some strings to make the object editor consider it "visible", but sadly it does not work in game, even if it is clearly marked in the object editor as visible.

There is a hotfix included in that thread, linking to this post:

http://www.wc3c.net/showpost.php?p=1050064&postcount=7

That should allow you to create the channel abilities. That link is the same one listed at the top of the thread, in the section Requirements.

Credits:

PipeDream - Grimoire.

Azlier - He made a tutorial on the object merger, which is where I first learned of its abilities.

PitzerMike - GrimExt and its documentation.

Nestharus - Getting Lua to become more popular, assisting me in my weird problems, and for convincing me to write this tutorial.

WaterKnight - ObjectMerger bug.


 Last edited: Dec 26, 2021

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Wow, it looks awesome Oo


You should submit this to TH and wc3c as well ;o

edit


Except this

//! i function getfilename()


That returns a variable, you don't have to edit what it returns. Just change the variable's value : P


This portion

 JASS:

//! i local FILENAME = "Lua_Fun_Time"

//! i function getfilename()
 //! i return "Lua_Fun_Time"
//! i end


Should just be this

//! i local FILENAME = "Lua_Fun_Time"


It says that in the little mini tut at the top of LUA_FILE_HEADER as well ; D

edit

 Just copy that text and paste it into an empty trigger. Make sure you have the LUA_FILE_HEADER in your map before installing. Afterward, just save and you can feel free to remove the code afterward! Yes, it is that simple. You can check if it is installed by going to the jassnewgenpack5d -> grimext -> luadir folder and seeing if it is installed there:


Click to expand...


You should note that Lua scripts are shared across all maps and JASS scripts are local to a map. Check the mini tut on the LUA_FILE_HEADER to get full scope specifics = P.


Also, you should never ever remove the LUA_FILE_HEADER from the map as that imports generated JASS scripts.


->//! import "luajass.Lua_Fun_Time.j"

edit


Also, please don't link to graveyarded Lua resources. There is a reason they were graveyarded. For example, the IO script shouldn't be used and the JASS globals thing shouldn't be used either.

 "PitchRollDests" is the script name. Note that the extension block actually accesses the FileExporter, because it is writing lua to a file, not creating the object data itself. So let's make a simple function that creates a destructable based off of 'LTbx' (Barrel) and allows you to modify the pitch/roll/name.


Click to expand...


That's incorrect. That's not what FileExporter does. Most Lua installation scripts are written using FileExporter because it has a lighter overhead than the ObjectMerger. If you save the map and compare ObjectMerger speed to FileExporter speed, FileExporter will be faster.

 There we go, now we should be done. Simply save, and it should install the script. Afterward, just remove the text (or disable the trigger it is in) and then test it out. This is the script I have used, which creates 10 of the barrels with different pitch/roll angles.


Click to expand...


That's not how you run object generating scripts. See the mini tut on LUA_FILE_HEADER to see how to properly run an object editor script.

 Use it with caution. I recommend using it on blank test maps prior to using it on the actual map, and creating backups of the map before running lua. This is just a caution-measure in case an infinite loop is ran, which will destroy your map.


Click to expand...


An even better option is to get something like Notepad++ and link up an Lua interpreter with it so that you can run Lua scripts. I don't recommend developing Lua scripts inside of WE, that's just a pain. I know I don't do that =p.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Doh, I'll fix that.

 You should note that Lua scripts are shared across all maps and JASS scripts are local to a map. Check the mini tut on the LUA_FILE_HEADER to get full scope specifics = P.


Click to expand...


I'll add that as well.

 edit


Also, please don't link to graveyarded Lua resources. There is a reason they were graveyarded. For example, the IO script shouldn't be used and the JASS globals thing shouldn't be used either.


Click to expand...


Oh, could've sworn I removed those. Anyway, I'll remove them asap.

 That's incorrect. That's not what FileExporter does. Most Lua installation scripts are written using FileExporter because it has a lighter overhead than the ObjectMerger. If you save the map and compare ObjectMerger speed to FileExporter speed, FileExporter will be faster.


Click to expand...


Fixed.

 That's not how you run object generating scripts. See the mini tut on LUA_FILE_HEADER to see how to properly run an object editor script.


Click to expand...


Well, the thing is that the script just has functions for object gen, kind of like LUA_NEW_BUFF. It doesn't actually create the objects unless the user executes the functions. I added clarification though, so that the reader knows he has to execute the script.


Thanks for the feedback. ;D


 Last edited: Mar 17, 2011

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Added more feedback ;o

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Can you provide test map or some resource related to this

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

-Kobas- said:

 Can you provide test map or some resource related to this

Click to expand...


Good idea. I'll whip something up tomorrow.

#### Raven0

#### Raven0

 Level 15

JoinedOct 16, 2010Messages941

Resources: 3

#### Resources
Maps 2Spells 1

epic tut


I haven't finished muddling through it all yet, but this should make mass item generation for my socketing system alot less tedious.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Oh, you forgot the updateobjects() when using getvarobject. You shouldn't use it in the function, but rather place it at the end of the script that calls the function.

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! runtextmacro LUA_FILE_HEADER()
 //! i dofile("PitchRollDests")

 //! i barrelgen("BarrelRolls","10")
//! endexternalblock


updateobjects is rather critical ;P


You should go over the API of LUA_GET_VAR_OBJECT since that script will be used a lot.

 JASS:

//function getvarobject(base, objtype, varname, import)
 //base: base id of object
 //"hpea", "Amov", "Bphx", etc

 //objtype: type of object
 //"units", "abilities", "items", etc

 //varname: name assigned to variable
 //OBJECTTYPE_NAME
 //"UNITS_MY_UNIT", "ABILITIES_RAIN_OF_CHAOS", etc

 //import: should the variable be imported into the map as a global?
 //true, false, nil

//function getvarobjectname(value)
 //retrieve name given value ("hpea", etc)

//function getvarobjectvalue(objectname)
 //retrieve value given name ("UNITS_MY_UNIT", etc)

//function updateobjects()
 //call at end of script


Also, you should talk about why Lua scripts are in all uppercase. It seems like a nutty convention now, especially considering that the Lua scripts themselves are not all uppercase, but back when Lua resources were first really starting up, there was a reason for the uppercase ;P. Remember that before LUA_FILE_HEADER, everything was a macro. LUA_FILE_HEADER is still a macro and it's the only macro you have to run.


You should also go over the other object generating scripts and things used for generating objects, like the dummy physical ability one and the object id one.


Also, this hasn't yet been update to use the script you wrote


->https://www.hiveworkshop.com/forums/submissions-414/snippet-lua_new_buff-180920/


My suggestion is just to submit your script linking back to the original and link to your updated script here =).


You should let people know that Lua scripts that don't run through LUA_FILE_HEADER and object generating scripts that don't use LUA_GET_VAR_OBJECT are no longer approved at THW. You can look through Submissions and Graveyard to see plenty of scripts that were unapproved and etc because of those 2 probs, some of mine included (I have yet to update them >.<).


 Last edited: Mar 19, 2011

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nestharus said:

 Oh, you forgot the updateobjects() when using getvarobject. You shouldn't use it in the function, but rather place it at the end of the script that calls the function.

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
 //! runtextmacro LUA_FILE_HEADER()
 //! i dofile("PitchRollDests")

 //! i barrelgen("BarrelRolls","10")
//! endexternalblock


updateobjects is rather critical ;P


Click to expand...


Whoops, I'll fix that. Thanks.

 You should go over the API of LUA_GET_VAR_OBJECT since that script will be used a lot.


Click to expand...


Will do.

 Also, you should talk about why Lua scripts are in all uppercase. It seems like a nutty convention now, especially considering that the Lua scripts themselves are not all uppercase, but back when Lua resources were first really starting up, there was a reason for the uppercase ;P. Remember that before LUA_FILE_HEADER, everything was a macro. LUA_FILE_HEADER is still a macro and it's the only macro you have to run.


Click to expand...


Okay, I'll add some of those fun facts. xP

 You should also go over the other object generating scripts and things used for generating objects, like the dummy physical ability one and the object id one.


Also, this hasn't yet been update to use the script you wrote


->https://www.hiveworkshop.com/forums/submissions-414/snippet-lua_new_buff-180920/


My suggestion is just to submit your script linking back to the original and link to your updated script here =).


You should let people know that Lua scripts that don't run through LUA_FILE_HEADER and object generating scripts that don't use LUA_GET_VAR_OBJECT are no longer approved at THW. You can look through Submissions and Graveyard to see plenty of scripts that were unapproved and etc because of those 2 probs, some of mine included (I have yet to update them >.<).


Click to expand...


Will do.


------


I'll get around to updating this tomorrow or the day after (or Monday), I've got some work to do this weekend. Again, thanks everyone for the feedback.

 epic tut


I haven't finished muddling through it all yet, but this should make mass item generation for my socketing system alot less tedious.


Click to expand...


Thanks.  You can feel free to ask me any questions if you get stuck. They might help me clarify some things in the tutorial.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

I built my first LUA script over the weekend thanks to this tutorial, really covers some useful ground.

I was having trouble finding the four-character names for the object editor data fields because I had "Display as Raw Data" turned on and that strangely hides the four-character field names. Thank you for writing this.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Bribe said:

 I built my first LUA script over the weekend thanks to this tutorial, really covers some useful ground.


I was having trouble finding the four-character names for the object editor data fields because I had "Display as Raw Data" turned on and that strangely hides the four-character field names. Thank you for writing this.


Click to expand...


Good to hear, and thanks for the feedback.


----


Updated.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

You going to let yourself approve this any time soon?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Yeah, I sometimes forget about this tutorial. lol.

I suppose I got enough positive replies. If anyone has any objections or anything I need to fix, PM me or post in this thread.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

I like this, very useful indeed...wanna learn moreeeee...


EDIT:


I just have one question...when you're done making generating object or make the script, where do you put it?...is it accessible to all scipts like libraries?...

#### watermelon_1234

#### watermelon_1234

 Level 14

JoinedNov 18, 2007Messages1,084

Resources: 10

#### Resources
Spells and Systems 9JASS 1

This was a really good, informative tutorial. It definitely helped me understand Lua (and the Object Merger) much better. =D

I only don't understand one little thing:

 However, the rawcode will be placed in parentheses.


Click to expand...

Do you mean quotes or something else?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

watermelon_1234 said:

 Do you mean quotes or something else?


Click to expand...


Sorry, I meant to say quotes. I don't know why I said parentheses, thanks for pointing that out, lol. Fixed.

#### a2581701

A

#### a2581701

i'm Taiwnaness , so my English not very well


Can it pick the value out?


As GMSL out as able to use some functions of the value we want to


Example: local str = A000.unam


Will be able to know the value of str

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

As far as I know, Lua doesn't offer that. I am not sure though, I'll check soon. However, I don't think it offers that kind of retrieval.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

It doesn't offer that kind of retrieval.


However, what you can do is store it into a global via Lua and then read the global.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

It would be helpful to note that in "external ObjectMerger" commands, unlike in "lua ObjectMerger" commands, you do not have to escape the backslash.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

k, I will add that and some other updates later on.

#### Barade

#### Barade

 Level 29

JoinedFeb 2, 2006Messages1,692

Resources: 5

#### Resources
Tools 1Maps 2Spells and Systems 2

Very useful tutorial but how do I perform arithmetic operations on object IDs while keeping the hexadecimal representation in LUA?


For example I have the ID "A000" and want to use 5 more IDs starting from this one: "A001", "A002" ...


I need something like tohexstring(tonumber(baseId) + 1).


If someone like me has many abilities to generate what would be the best way to choose their IDs. I thought of choosing a baseId and using a range from that by arithmetic operations.


For example https://www.hiveworkshop.com/forums/graveyard-418/snippet-lua_object_id-176943/ says it may overwrite custom object ids and does it change the ID everytime I save the map? I want to keep some IDs constant for my generated abilities.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

https://www.hiveworkshop.com/forums/jass-ai-scripts-tutorials-280/lua-object-generation-191740/#VII

https://github.com/nestharus/JASS/tree/master/lua/LUA_GET_VAR_OBJECT

edit

The tutorial doesn't mention that you only need to do Lua script installation once per machine, not per map ; ).

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

I think that object w3b is bugged, also for the ability field amcs

Why?

whenever I try to create a custom buff with w3b, I always receive an error, where it says that the base object cannot be found. I tried doing this on other buffs and it still gives me error.

For the case of amcs (Mana Cost), no matter what value you put, it won't change :/

Try this(if this works for you, then I think it is my ObjectMerger that is bugged):

//! external ObjectMerger w3a AHtb TEST amcs 1 0

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Almia said:

 I think that object w3b is bugged, also for the ability field amcs


Why?


whenever I try to create a custom buff with w3b, I always receive an error, where it says that the base object cannot be found. I tried doing this on other buffs and it still gives me error.


Click to expand...


This might be your problem:


 Tutorial said:



w3b Destructables

w3h Buffs

Click to expand...


Make sure you use w3h, not w3b. In your test line, you also used w3a.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

so that's why. I hate that

In the test line, it was a demo how amcs (Mana Cost) remains unchanged. It uses Thunder Bolt as a a base ability.

#### EdwardElric

#### EdwardElric

 Level 4

JoinedJun 19, 2010Messages49

i'm here to report a bug of the ObjectMerger. it seems that unit 'hpea', field "uaen" can not be set to 0 through LUA trigger.

here's a recent thread reply of mine showing my LUA code (post #3):

link1:[vJASS] - LUA: Ascii typecasting on ObjectMerger

after some chat with WaterKnight i tried to inspect ObjectMerger source code to fix the problem. got official source code from author PitzerMike from here:

link2:https://www.hiveworkshop.com/pastebin/e10b1021af8b2b19368786869def5c9c2343/

( link3:Source code of all my work - Wc3C.net )

however, im already stuck at "...\War3ToolsSource\ObjectMerger\main.cpp" at following line in main method:

 Code:

 if (argc < 4 ) { // check parameter count
 fprintf(stderr, "Wrong parameters for object merger, usage:\nmap.w3x lookuppaths [ m | r | i ] mergefile { moremergefiles }\nmap.w3x lookuppaths ( w3u | w3t | w3b | w3d | w3a | w3h | w3q ) originalid newid { changeid [ level | variation ] value }\n");
 exit(-1);
 }

the thing i do not understand is, that argc should already be < 4 for following command in JASS:

//! external ObjectMerger Objects\war3map.w3u

how exactly is this command interpreted?

if it just runs ObjectMerger.exe & passes argument "Objects\war3map.w3u", then argc should be 1, which would cause ObjectMerger.exe to exit, because of the quoted if-block above... what am i missing?

edit1:

from the code comment example in "...\ObjectMerger\main.cpp" it seems, that argument passing can be explained like following:

 Code:

EXAMPLE CALL:
//! external ObjectMerger Objects\war3map.w3u
SHOULD PASS 4 ARGUMENTS LIKE:
//argv[0] = ""; <-- ALWAYS NULL?!
//argv[1] = "F:\\Sweeper.w3x"; <-- PATH TO MAP (AUTOMATICALLY!? BUT HOW & WHERE???)
//argv[2] = ""; <-- LOOKUP PATHS (FOR RELATIVE PATHS) (AUTOMATICALLY!? BUT HOW & WHERE???)
//argv[3] = "Objects\war3map.w3u"; <-- PASSED PATH TO W3U OBJECT DATA FILE FOR UNITS

so, about the how & where: it happens on map saving, but is it hardcoded into JNGP (= Jass New Gen Pack) or can i view the responsible code anywhere?

edit2:

in "...\JNGP\wehack.lua":

 Code:

function runobjectmerger(mode)
 curmap = wehack.findmappath()
 if curmap ~= "" then
 source = wehack.openfiledialog("Unit files (*.w3u)|*.w3u|Item files (*.w3t)|*w3t|Doodad files (*.w3d)|*.w3d|Destructable files (*.w3b)|*.w3b|Ability files (*.w3a)|*.w3a|Buff files (*.w3h)|*.w3h|Upgrade files (*.w3q)|*.w3q|", "w3a", "Select files to import ...", true)
grim.log("got in lua: " .. source)
 if source ~= "" then
 list = strsplit("|", source);
-- cmdargs = "ObjectMerger \""..curmap.."\" "..wehack.getlookupfolders().." "..mode..fileargsjoin(list)
 cmdargs = "grimext\\ObjectMerger.exe \""..curmap.."\" "..wehack.getlookupfolders().." "..mode..fileargsjoin(list)
grim.log("assembled cmdline: " .. cmdargs)
-- wehack.messagebox(cmdargs,"Grimoire",false)
 wehack.savemap()
grim.log("called saved map")
 end
 else
 showfirstsavewarning()
 end
end

but this function is only called via WE GUI (= World Editor Graphical User Interface) menu entries:

- "World Editor/Extensions/Merge Object Editor Data"

- "World Editor/Extensions/Replace Object Editor Data"

- "World Editor/Extensions/Import Object Editor Data"

so still I have no clue where to find the code which controls the passing of arguments to ObjectMerger.exe from trigger code calls & not from WE GUI menu entries.

edit3:

still in "...\JNGP\wehack.lua":

 Code:

function compilemap_path(mappath)
...
 toolresult = wehack.runprocess2(cmdline)
...

function "compilemap_path" runs each map save & within, the function "wehack.runprocess2" runs for map triggers (including external tool calls)! at top of this LUA file following is written as comment:

 Code:

-- wehack.runprocess2: Wait for exit code, don't report errors (jasshelper)

so the magic parsing of arguments for trigger based ObjectMerger code is done here i guess. looked into "...\JNGP\vexorianjasshelper\" & saw "pjass.exe". "pjass.exe" means "Parse JASS" i guess. found its source code in tools from PitzerMike (see "link2" above)! but it doesn't contain words from error when external calls to ObjectMerger are wrong. there must be something like: "Invalid object id ...".

so "pjass.exe" isn't passing arguments to ObjectMerger from trigger code!

what ramains?

--> "...\JNGP\vexorianjasshelper\jasshelper.exe"!

official link to app & source code of latest JassHelper from author Vexorian (see attachment in post #1 in following thread):

link4:JassHelper 0.A.2.B - A vJass and Zinc 2 Jass compiler - Wc3C.net

edit4:

the comment i found in source code of PitzerMike's Grimex (= GRIMoire EXtension pack) under "...\War3ToolsSource\ObjectMerger\main.cpp" which describes ObjectMerger argument usage, also is explained in the documentation under "...\JNGP\grimext\GrimexManual.html" under "Quickstart/Object Merger:":

 Code:

map.w3x lookuppaths [ m | r | i ] mergefile { moremergefiles }
map.w3x lookuppaths ( w3u | w3t | w3b | w3d | w3a | w3h | w3q ) originalid newid { changeid [ level | variation ] value }

edit5:

i was able to compile ObjectMerger.exe from source code (see "link2" above). sadly this source code isn't the latest, as the author PitzerMike lost the current:


 PitzerMike said:



 Unfortunately it's not the most up-to-date version of the code. The latest parts of the grim extensions are missing (mainly the code for PatchGenerator.exe which is used to generate the UMSWE patch). If anyone has downloaded the code from the old repository, please post it here. You will know it's the latest verson if it has a sub-folder called PatchGenerator.


Click to expand...

the compiled ObjectMerger.exe file is 465 KB in size, while the channel ability hotfixed ObjectMerger.exe file is 548 KB in size:

link5:Wc3C.net - View Single Post - Grim Extension Pack

this makes me wonder how much PitzerMike must have been added to its source to make the file increase by 83 KB only by code, especially what else did he add to ObjectMerger than fixing channel ability???

so even if i fix the problem with the unit 'hpea' with the field "uaen", some current features may be missing... maybe someone still has the latest source code of ObjectMerger?

all what i found is a thread where PitzerMike linked his source code from 2011_10_16, but unfortunately it says "404 PAGE NOT FOUND" (last post #15):

link6:Grim Extension Pack - Wc3C.net

edit6:

2 notes about version of latest available source code of ObjectMerger from author PitzerMike (see "link2" above):

note 1:

the compiled "ObjectMerger.exe" from source code needs "MpqCom.dll" in the same folder to run properly, otherwise ObjectMerger doesn't work & immediately terminates with an appcrash. mentioned "MpqCom.dll" can be found after compilation of ObjectMerger source code in folder "MpqCom". to explain the difference in file size between ObjectMerger.exe of compiled source code & channel ability hotfix, user WaterKnight gave me the idea, that PitzerMike could have compiled the DLL as static linked library into the EXE. after some tests, i could negate this theory. latest JNGP still contains this "MpqCom.dll", it just is a bit smaller & has another name! equivalent of "MpqCom.dll" is: "...\JNGP\grimext\grimex.dll" & 383 KB insize, while my compiled "MpqCom.dll" is 394 KB in size. to proof that, i just compiled ObjectMerger to use the "grimex.dll" instead of the "MpqCom.dll". i replaced "...\JNGP\grimext\ObjectMerger.exe" with my compiled one & removed the "MpqCom.dll" i formerly put in "...\JNGP\grimext\". result: ObjectMerger.exe didn't crash & worked successfully. everything i changed to source code was in "...\MpqCom\mpqcomapi.cpp" at line #77: mpqCom = LoadLibrary("MpqCom.dll"); to: mpqCom = LoadLibrary("grimex.dll");.

note 2:

source code of ObjectMerger from PitzerMike doesn't support LUA commands from triggers as it seems. simple JASS trigger commands like //! external ObjectMerger w3u hpea 1d1A unam "test" work fine, but ObjectMerger breaks for scripts inside:

 JASS:

//! externalblock extension=lua ObjectMerger $FILENAME$
...
//! endexternalblock

the reason is, that a LUA file will be created by using an "externalblock" command in JASS. this LUA file can be found in a path like: "C:\Users\<user>\AppData\Local\Temp\V706D.tmp.lua". i stumbled across this path, as the compiled ObjectMerger from source code will error report it in a message box as soon as preprocessor command "externalblock" is used in JASS in a trigger & the map is saved. message box example: "Could not open: C:\Users\<user>\AppData\Local\Temp\V706D.tmp.lua". the LUA file gets created properly, because that's the job of "JassHelper", but the compiled ObjectMerger from source code isn't yet supporting LUA as this source code isn't the latest which current ObjectMerger.exe from JNGP uses. these LUA files contain all the code from the JASS code block "externalblock", without the leading //! i, which just tells JassHelper to save everything next to it into a LUA file.

work of reference for inner workings:

1 the entry point in the JassHelper source code (coding language = delphi) on map save:

from my understanding, the actual source code file which runs "JassHelper" from WE is "grimoirejasshelper.dpr". the actual source code file which runs "JassHelper" from "clijasshelper.exe" should be "clijasshelper.dpr".

2 the process where the LUA file gets prepared (parse JASS code):

for WE, on map save, "JassHelper 0.A.2.B." starts in source code file "grimoirejasshelper.dpr" with function at line #326: function doWEWarlock(const m:string):boolean;. inside this function the if-block at line #569: if(not nopre) then begin is entered when "nopre" is "false".

"nopre" is a variable of type boolean, declared inside "grimoirejasshelper.dpr" at line #26: debug:boolean=false;nopre:boolean=false;noopt:boolean=false;. "nopre" obviously stands for no preprocessor. "nopre" is initialized with value "false". if "JassHelper" runs without additional parameters, the value of "nopre" stays false, otherwise, if parameter "'--nopreprocessor" is added, value of "nopre" is changed to "true" at line #435: end else if(ParamStr(temi)='--nopreprocessor') then begin & line #436: nopre:=true;. note that "ParamStr(...)" is a native function in coding language pascal.​
so, by default the if-block gets computed. the first line inside the if-block, line #570: jasshelper.DoJasserMagic('logs\currentmapscript.j',compiled,debug); calls a procedure from souce code file "jasshelper.pas". in "jasshelper.pas" this procedure is declared as a public procedure (in coding language pascal, declaring something as public is done at the file header in the interface section) at line #305: procedure DoJASSerMagic(f1:string; f2:string; debug:boolean);overload; & the actual procedure is declared at line #2.494: procedure DoJASSerMagic(f1:string; f2:string; debug:boolean);Overload;. inside this procedure the identically named function "DoJASSerMagic(...)" gets called at line #2.540: Write(ff2,DoJASSerMagic(inp,debug));. it's also made public at line #308: function DoJASSerMagic(sinput:string; debug:boolean):string; overload; & declared at line #2.616: function DoJASSerMagic(sinput:string; debug:boolean):string;overload;. here, also note, that identifiers are case-insensitive in coding language delphi/pascal! (DoJasserMagic = DoJASSerMagic) in this function "DoJasserMagic(...)" procedure "parseInput(...)" gets triggered at line #2.936: parseInput(debug);. procedure "parseInput(...)" is declared at line #1.447: procedure parseInput(debug:boolean);:

- JASS code for lines with "externalblock" gets recognized in if-block at line #1.762: end else if(word='externalblock') then begin.

- JASS code for everything between "externalblock" & "endexternalblock" gets recognized in if-block at line #1.790: end else if(word='i') and (ExternalBlockName <> '') then begin.

- JASS code for lines with "endexternalblock" gets recognized in if-block at line #1.793: end else if(word='endexternalblock') then begin.

so, everything between JASS lines with "externalblock" & "endexternalblock" is saved into a variable of type string called "ExternalBlockStdin" at line #1.791: SWriteLn(ExternalBlockStdin,Copy(input[k], wordend+1, Length(input[k]) ) );. "SWriteLn(...)" is no default function of coding language pascal, but a custom one written by the author to store every line inside the string variable "ExternalBlockStdin" (name of variable can be written out like: external block standard input). when JASS line with "endexternalblock" is reached, the concatenated string "ExternalBlockStdIn" gets passed & saved at line #1.794: Exter.add(ExternalBlockName, ExternalBlockArgs, ExternalBlockLn, ExternalBlockExtension, ExternalBlockStdIn);.

3 the process where the LUA file is created (apply collected data):

later in the JassHelper source code file "grimoirejasshelper.dpr" inside function at line #326: function doWEWarlock(const m:string):boolean; a procedure is called at line #699: doExternalThings;. "doExternalThings" is declared at line #272: procedure doExternalThings;.

this procedure "doExternalThings" looks for entries in a variable called "Exter". "Exter" derives from JassHelper source code file "jasshelper.pas". in "jasshelper.pas" it's initialized at line #449: Exter: Texternalusage=nil;. "TexternalUsage" is a custom variable of type "class(TObject)" (TObject = Type Object => TexternalUsage = Type External Usage), defined at line #229: TexternalUsage=class(TObject) .... entries for this variable are added with the procedure at line #505: procedure Texternalusage.add(const na:string; const a:string; const i:integer; const ex:string; const si:string);. the last argument "si" (probably stands for string input or standard input) is for the already mentioned variable of type string named "ExternalBlockStdIn". in other words, each entry in "Exter" holds the whole JASS trigger code of lines between "externalblock" inside "Exter.stdin[N]". "Exter.N" returns the entry count.​
inside "doExternalThings" each entry for "Exter" is passed to another procedure called "doTool" at line #292: doTool(EXTERNAL_Names[j],EXTERNAL_PROGRAMS[j],Exter.args, Exter.ext, Exter.stdin);. "doTool" is the actual procedure which creates the LUA files. "doTool" is declared at line #231: procedure doTool(const name:string; const prog:string; args:string; const ext:string; const stdin:string);. at line #238: tem:=TempFile; the variable "tem" of type string is set to a global function called "TempFile". "TempFile" comes from JassHelper source code file "vxFilesys.pas". "TempFile" is declared public at line #11: function TempFile: TFileName;. the actual function is declared at line #113: function TempFile: TFileName;. from what i understood, what it does is returning the absolute path to a new empty temporary file. at line #118: if GetTempFileName(PChar(GetTempDir),'V', 0, NomArchTemp) = 0 then "GetTempFileName" is a default function of coding language pascal with syntax: "function GetTempFileName(Dir: PChar;Prefix: PChar;uUnique: DWord;TempFileName: PChar):DWord;". "PChar" means Pointer to an array of Char. "GetTempDir" is a function declared at line #85: function GetTempDir: TFileName;. it returns the actual absolute path to a temporary file with function at line #90: SetString(Result, TmpDir, GetTempPath(MAX_PATH, TmpDir));. "GetTempPath" is a function from WinAPI. it is included in coding language pascal by using "uses" & adding "Windows". it retrieves the path of the directory designated for temporary files, like: "C:\Users\<user>\AppData\Local\Temp\". back in JassHelper source code file "grimoirejasshelper.dpr" inside procedure "doTool" at line #242: Assign(ftem,tem); "Assign" assigns the name from "tem" to the file variable "ftem" of type Textfile. "Assign" is a default function of coding language pascal. at line #243: Rewrite(ftem); the file "ftem" gets opened for writing. "Rewrite" is a default function of coding language pascal. finally, at line #244: Write(ftem,stdin); the contents of the variable "stdin" are written to the file "ftem". also "Write" is a default function of coding language pascal. the LUA file is created.

4 the argument passing to ObjectMerger.exe (of JASS "externalblock"/ LUA files):

only a few lines later in the JassHelper source code file "grimoirejasshelper.dpr" inside procedure "doTool", following command sends appropriate arguments to ObjectMerger.exe in case of used JASS code "externalblock" at line #257: temi:= WinExec.StartApp(prog,'"'+map+'" "'+JasshelperConfigFile.WORK_PATHS+'" '+args,GetCurrentDir,0, tem4,tem2,tem3);.

- "prog" is a variable of type string. "prog" contains the path to the application which should get executed. "prog" was passed from procedure "doExternalThings" from variable "EXTERNAL_PROGRAMS[j]" of type array of string from line #292: doTool(EXTERNAL_Names[j],EXTERNAL_PROGRAMS[j],Exter.args, Exter.ext, Exter.stdin);. "EXTERNAL_PROGRAMS[...]" comes from JassHelper source code file "jasshelperconfigfile.pas". what it does is reading the file "...\JNGP\jasshelper.conf" & fill "EXTERNAL_PROGRAMS[...]" with found application entries under section "[externaltools]" in that file.

for example the line: "ObjectMerger","grimext\ObjectMerger.exe"

is already explained at top: ...the syntax is "NAME","executable path"

now in JassHelper source code file "jasshelperconfigfile.pas" the variable "a" of type string is set to the executable name, in this case: ObjectMerger & the variable "b" of type string is set to the executable path, in this case: grimext\ObjectMerger.exe. all this happens inside if-block at line #102: if(s_ext) then begin. "EXTERNAL_PROGRAMS[...]" is set to "b" at line #125: EXTERNAL_PROGRAMS[EXTERNAL_N]:=b;.

- "map" is a variable of type string. "map" contains the path to the map currently saved from WE. "map" is set at line #469: map:=ParamStr(temi+2);.​
"WinExec.StartApp" is declared in JassHelper source code file "winexec.pas" at line #15: function StartApp(AppName, ArgStr, workdir :String; Visibility : integer; input,output,error:string):integer;.

example for a JASS "externalblock":

WinExec.StartApp(

grimext\ObjectMerger.exe,

''C:\Spiele\Warcraft III\Maps\_SourceSeeker\test_objectMergerW3u.w3m" ".\jass\" "C:\Users\<user>\AppData\Local\Temp\V758A.tmp.lua",

C:\Spiele\JNGP2.0 2.0.0.7 (TriggerHappy) [2013_12_13]\vexorianjasshelper,

0,

C:\Users\<user>\AppData\Local\Temp\V758D.tmp,

C:\Users\<user>\AppData\Local\Temp\V758B.tmp,

C:\Users\<user>\AppData\Local\Temp\V758C.tmp​
);

5 the entry point in the ObjectMerger source code (coding language = C/C++) on map save:

when ObjectMerger.exe gets executed from JassHelper (see above), its entry point in source code is in file "...\ObjectMerger\main.cpp" (it has 332 lines of code) in function at line #247: int main(int argc, char **argv) {. first thing ObjectMerger source code does, is checking the number of passed arguments at line #257: if (argc < 4 ) { // check parameter count.

"argc" is of type int & stands for "ARGgument Count". "argc" automatically returns the number of arguments passed to the program + 1. "+1" because the programs name always is the first argument. lets compare this with "argv". "argv" is of type char & stands for "ARGument Vector" (vector in meaning of array). "**argv" means pointer of pointer of char, you could also write "*char[]". the "*..." in cpp is called "indirection operator". "*expression" is the way to create a pointer to an expression. however, argv[0] contains the program's name like described for "argc" & argv[1...] contains the passed arguments.

when you save a map with an "externalblock", JassHelper creates a LUA file & passes arguments to ObjectMerger.exe.

example:

argc=4

argv[0]=grimext\ObjectMerger.exe

this path comes from: "...\JNGP\jasshelper.conf" from section: "[externaltools]" from line with: ""ObjectMerger","grimext\ObjectMerger.exe"". JassHelper retrieves it from there & runs ObjectMerger by using this path.​
argv[1]=C:\Spiele\Warcraft III\Maps\_SourceSeeker\test_objectMergerW3u.w3m

this path is the path to the map currently saved. JassHelper detects it & runs ObjectMerger by passing this path as 1st argument.​
argv[2]=.\jass\

this path comes from: "...\JNGP\jasshelper.conf" from section: "[lookupfolders]" from line with: "".\jass\"". JassHelper retrieves it from there & runs ObjectMerger by passing this path as 2nd argument.​
argv[3]=C:\Users\<user>\AppData\Local\Temp\V758A.tmp.lua

this path leads to the LUA file containing the "externalblock" code. JassHelper created it & runs ObjectMerger by passing this path as 3rd argument (explained above).​
so, although LUA isn't supported yet in the ObjectMerger source code (in the following we will proof this by stepping through the code), source code continues with code execution, because "argc" isn't < 4 (it is 4, see example)!​
after this if-block there is a for-block at line #261: for (int i = 0; i < strlen(argv[2]); i++) // convert paths.

here, "argv[2]" is checked for passed lookup paths from: "...\JNGP\jasshelper.conf" from section: "[lookupfolders]". found paths with slashes ("/") are replaced by paths with 2 backslashes ("\\").​
after this for-block the next if-block is at line #271: if (strlen(argv[2])) {.

in coding language C++ the if-condition is true, as long as the condition value is not 0 or null. in our case we have "strlen(argv[2])" as condition, which means that as long as "argv[2]" has at least 1 char, the condition is true & the if-block is entered. by default "argv[2]" contains ".\jass" as string, so the if-block is executed. the value of "argv[2]" derives from JassHelper source code from file "grimoirejasshelper.dpr" at line #257: temi:= WinExec.StartApp(prog,'"'+map+'" "'+JasshelperConfigFile.WORK_PATHS+'" '+args,GetCurrentDir,0, tem4,tem2,tem3);. "JasshelperConfigFile.WORK_PATHS" derives from file "jasshelperconfigfile.pas" where it is set at line #138: if (WORK_PATHS='') then WORK_PATHS:=a & line #139: else WORK_PATHS:=WORK_PATHS+';'+a;. the variable "a" is of type string & contains the paths which were found in file "...\JNGP\jasshelper.conf" from section: "[lookupfolders]".

in this if-block the paths from "argv[2]" are saved to variable "paths" of type "LookupTable". "LookupTable" is a custom type defined at line #12: typedef set< string > LookupTable;. "LookupTable" is a set for elements of type string. the operator "<" stands for the sorting criterion, it makes sorting the elements in ascending order. for descending order you could do something like: typedef set< string, greater<string> > LookupTable;.​
right after this if-block there comes another if-block at line #277: if (strcmpi(argv[3], "w3u") == 0 || strcmpi(argv[3], "w3t") == 0 || strcmpi(argv[3], "w3b") == 0 || strcmpi(argv[3], "w3d") == 0 || strcmpi(argv[3], "w3a") == 0 || strcmpi(argv[3], "w3h") == 0 || strcmpi(argv[3], "w3q") == 0) {.

"strcmpi" is a default function of coding language C++. "strcmpi" compares 2 strings without sensitivity to case. "strcmpi" returns 0 when both strings are equal.

here, ObjectMerger checks 3rd passed argument "argv[3]" for objecttype w3u, w3t, w3b, w3d, w3a, w3h & w3q. this would be the case for example for JASS preprocessor code //! external ObjectMerger w3q Remg Rgl2 glvl 2 gar1 2 "ReplaceableTextures\CommandButtons\BTNImpale.blp" (example taken from "...\JNGP\grimext\GrimexManual.html"). in our case, where "argv[3]" contains the path to a LUA file, the if-block isn't entered.​
so, next comes the else-block at line #295: } else {.

a new if-block checks whether "argv[3]" contains the merge mode ("m", "r" or "i") at next line #296: if (strlen(argv[3]) == 1) {.

in our case "argv[3]" contains the path to the LUA file, hence this if-block isn't entered.​
after that if-block there comes a for-block at line #305: for (int i = start; i < argc; i++) { // find files.

as far as i understood this for-block checks each entry in "argv" for file existance. if the file couldn't be found, a subroutine checks file existance for incomplete paths. if the file still wasn't found an error window is returned & ObjectMerger is quit.​
the last important command under the else-block & in this script is at line #326: if (!dofiles(argc, argv, mode, start)).

"dofiles" is a function declared at line #74: bool dofiles(int argc, char *argv[], char mode, int start) {. passed variable "start" of type int contains value 3 in our case of LUA file.​
"dofiles" loads a custom variable "OBJ" at line #79: OBJ temp(firstfile[strlen(firstfile) - 1]);.

variable "firstfile" is a pointer to a char, it is declared at line #77: char* firstfile = argv[start];. in our case "firstfile" points to "argv[3]", the path to the LUA file. so, the content of the brackets of "OBJ temp(...)" points to the last char from "argv[3]". in cases with no LUA, but preprocessor code like: //! external ObjectMerger Objects\war3map.w3u (example taken from "...\JNGP\grimext\GrimexManual.html") it would be: "OBJ temp(u)". it is designed to receive the objecttype (like already mentioned a few lines above: w3u, w3t, w3b, w3d, w3a, w3h or w3q) which would be u, t, b, d, a, h or q for the bracket's content. in our case of a LUA file it receives "a". the custom variable "OBJ" is of type class & derives from source code file "...\common\obj.h".​
the last step before the error for LUA files pops up an if-block is entered at line #90: if(!temp.load(argv[i], wts)) {.

"wts" is another custom variable like "OBJ". "wts" is of type class & derives from "...\common\wts.h". "wts" is used at line #82: if (!loadpresentfiles(mpq, argv[1], wts, obj, OnlyExtension(argv[start]))) {.

- argument 1 "mpq" stands for the map archive curretnly being saved, for example archive to "C:\Spiele\Warcraft III\Maps\_SourceSeeker\test_objectMergerW3u.w3m".

- argument 2 "argv[1]" contains the path to the map currently saved, like: "C:\Spiele\Warcraft III\Maps\_SourceSeeker\test_objectMergerW3u.w3m".

- argument 3 "wts" is null.

- argument 4 "obj" is null.

- argument 5 "OnlyExtension(argv[start])" in our case is "OnlyExtension(argv[3])" which can be "OnlyExtension("C:\Users\<user>\AppData\Local\Temp\V758A.tmp.lua")" which would be "lua".

in sum it could look like this: "if (!loadpresentfiles(mpq, "C:\Spiele\Warcraft III\Maps\_SourceSeeker\test_objectMergerW3u.w3m", null, null, "lua")) {". what it does there is passing the map's "war3map.wts" file to "wts" & "OBJ".

back to if-block at line #90, "temp.load(...)" is a function in source code file "...\common\obj.cpp". the passed argument "argv[i]" from "temp.load(argv[i], wts)" contains the LUA file path. "temp.load(argv[i], wts)" is defined at line #638: bool OBJ::load(const char* filename, WTS &wts, bool inlinewts) {. there it opens the LUA file as an "ifstream" (= Input File STREAM) & passes the stream & the wts to another function at line #644: bool result = load(infile, wts, inlinewts);. "load(infile, wts, inlinewts)" is defined at line #614: bool OBJ::load(istream &stream, WTS &wts, bool inlinewts) {.​
finally, the last check before the error report happens: first the "version" variable of the passed "stream" gets written at line #616: stream.read((char*)&version, sizeof(version));. variable "version" is of type unsigned int & defined in source code file "...\common\obj.h" at line #68: unsigned int version;. "sizeof(...)" is a default function of coding language C++ & it returns the size in bytes of the object representation of type. "sizeof(version)" is like "sizeof(unsigned int)" & this would be the value "4". so, "stream.read((char*)&version, sizeof(version));" can be read like "stream.read((char*)&version, 4);". now this can be read like: extract 4 characters from the "stream" & store them in the array pointed to by "version". and the last line, right before the error window will be showed, is at line #617: if (version != 1 && version != 2) {. because the first 4 bytes in the header of the LUA file aren't representing value 1 or 2 like warcraft objecttype files (*.w3u, *.w3t, etc.) have, the if-block returns "false" at line #618: return false;. just for reference, the first 4 bytes in the header of the LUA file returns the value 1668183398.

back to source code file "...\ObjectMerger\main.cpp" the error message is reported at line #91: fprintf(stderr, "Could not open %s\n", argv[i]);.

what arguments does ObjectMerger from source code accept?

- either a path to an objecttype file (or before the path a merge mode like "m", "r" or "i") or objecttype.

so when the path to a LUA file is passed as argument to compiled ObjectMerger.exe (that happens automatically when "externalblock" is used in JASS & JassHelper is activated in JNGP when map is saved), ObjectMerger exits with error popup, that it couldn't open the LUA file. the reason is, that ObjectMerger expected a file with a header with first 4 bytes being 1 or 2 like warcraft objecttype files (*.w3u, *.w3t, etc.) have.

what is "preprocessor" code?

- JASS code starting with //!. it calls external apps like ObjectMerger.exe & passes appropriate arguments. it must not be interpreted for the actual warcraft map, hence it gets removed again from the final trigger script which is saved into the map file (map file = file with extension .w3m or .w3x).

so, ObjectMerger from source code misses LUA support, what would be the fix for supported preprocessor code like //! external ObjectMerger w3u hpea 1d1A uaen 0?

- in this case in source code file "...\ObjectMerger\main.cpp" the main function at line #247: int main(int argc, char **argv) { runs function "docreate" at line #293: if (!docreate(argc, argv, increment)). "docreate" is defined at line #177: bool docreate(int argc, char *argv[], int increment) { & runs function "makecopy" at line #210: if (!obj.makecopy(slk, War3Id(argv[4]), War3Id(argv[5]), increment, argc, argv)) {. function "makecopy" can be found in source code file "...\common\obj.cpp" at line 389: bool OBJ::makecopy(SLK &metadata, War3Id templateid, War3Id newid, int increment, int argc, char *argv[]) {. in this function "makecopy" another function gets executed at line #441: unsigned int vartype = getvartype(metadata.getValue(argv[i], "type").getString());. function "getvartype" is defined at line #379: unsigned int OBJ::getvartype(string name) {. it looks for "int", "bool", "real", "unreal", "string", but misses a definition for type "attackBits". "attackBits" derives from "...\Warcraft III\War3Patch.mpq", "Units\\UnitMetaData.slk". in SLK row 264 in column 1 ("ID") there should be the object editor field id "uaen". now look to the right until column 8 ("type"), there should be the "attackBits". so if you want to fix this, you just could change line #380: if (name == "int" || name == "bool") to: if (name == "int" || name == "bool" || name == "attackBits") & it should work. i didn't test it, because you still lack the feature for LUA support, hence i won't make any changes to this latest available source code of ObjectMerger, because it misses big parts of code of current ObjectMerger.exe.

now that i know all this, i will try to write my own tool to fix ObjectMerger problems by adding it to: ""...\JNGP\jasshelper.conf" under section: "[externaltools]".

edit7:

finally, i was able to publish such a tool, here's the link:

ObjectMergerFixer: fixing some known problems with ObjectMerger


 Last edited: Oct 23, 2016

#### Barade

#### Barade

 Level 29

JoinedFeb 2, 2006Messages1,692

Resources: 5

#### Resources
Tools 1Maps 2Spells and Systems 2

Hey, if you still want to write your own ObjectMerger. I got the problem that Umlaute like ä or ß in the beginning of ability names were cut off by the current Object Merger.

My project wc3lib has support for reading and writing object data and getting default fields etc. There's also an alpha GUI object editor. Since I am interested myself in writing such a tool (to fix the other bug as well) just write me if you're still working on this.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

The functionality of the ObjectMerger is rather straightforward, I too have (java) libs to process the .w3u, .wts, slks, etc. But what do you want the input to look like? Equal to PitzerMike's ObjectMerger?

 You must log in or register to reply here.


### Similar threads

Locked

 lua loadObjectEditorData


Wietlol

Oct 28, 2015

Warcraft Editing Tools

Replies6Views2K

Oct 28, 2015
PurgeandFire

Locked

 [Lua] How to make globals throughout script?


sethmachine

Jan 6, 2014

Triggers & Scripts

Replies1Views2K

Jan 6, 2014
Nestharus

Locked

[Solved]
 Issues using Lua extension with ObjectMerger


0x4b

Mar 26, 2021

World Editor Help Zone

Replies0Views1K

Mar 26, 2021
0x4b

Locked

 Getting help on using ObjectMerger


yxq1122

Jun 6, 2020

Warcraft Editing Tools

Replies4Views2K

Jun 30, 2020
PurgeandFire

Locked

 Streamlined object generation


Xonok

Aug 31, 2016

Advanced Scripting

Replies2Views2K

Aug 31, 2016
Xonok

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