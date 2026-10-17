---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-a-spell-in-vjass-practice-session-1.114519/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making a Spell in vJASS Practice Session 1
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-a-spell-in-vjass-practice-session-1.114519/>
#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

#
 Making a spell in vJASS with good practice session 1

Hi all, this is the first tutorial of a serie of tutorial I plan to do if I have enough time. In this tutorial you will learn the basics of vJASS as well as good coding practices that took me years to learn in the wc3campaigns website. This tutorial was specially designed to aid my students, I have so many now that I can’t help them all, so I decided to make a tutorial to aid them all. If you have any questions please post comments for me to help. However, please note I don’t want this tutorial to be a “performance freak” tutorial where everything is highly optimized. I want is to make people learn as much as possible and so I decided to make some things less efficient so they could be easier to understand. Please understand that when reading my tutorial I want people to learn the basics and be able to make simple spells as this one and not to make super efficient machines, the learning process is far more interesting form my point of view. I must also say that I am very scared about this tutorial, mainly because I am submitting it first to wc3c so it can have a “guarantee” symbol for a manner of speaking. This tutorial is something like 53 pages long and it took me many days to create and finish. I would like people to please take that in mind when evaluating my work here. One of the reasons I made this tutorial so long is because I was on Christmas holiday and so I had time to spend with what I like doing – helping people. However with school coming back, I don’t know when I will have the same amount of free time, and so session 2 can take a few months or may never be released. Finally, I would like the moderators from the wc3c to please approve this huge tutorial since I gave a lot of myself to it. Again, please note I made a choice here – learning process is more important than a highly efficient process.


Finally I would like to ask people to make comments and give me reputation points, even if you are not an expert, know that feedback is of EXTREME importance for those who help people like I do.


In the end of the map there is a bibliography section and the map of the spell. I strongly advise people to read it and to download the map. There is also a PDF version of the tutorial which you can download if you don’t have time to read it all on the internet.


Hoping it helps, and hoping it gets approved, Flame_Phoenix.

##
 Index

1 - Introduction


2 - Differences between JASS and vJASS and JESP


3 - Basics of vJASS
3a - Free Global declaration


3b - Scopes and Libraries


3c - The use of “private”​
4 - Starting our spell


5 - Bibliography


6 - Credits


7 - Goodbye

##
 Introduction


Hi, in this first tutorial of mine I am going to teach my readers the basics of vJASS to make a SIMPLE spell. In further tutorials I plan to extend to other things such as structures with timers and the use of Table. In the end, you will be an experienced vJASSER, although you will possess all current knowledge I have, know that to become a great vJASSER you will have to make your own spells and to carry your own scars. The most important thing will be the spells you will do, they will be your real teacher.


In this tutorial I will introduce the following basic concepts of VJASS:


- Free global declaration


- Scope and Library


- The use of “private”


I will also teach you some good coding practice I know of and I am going to do all of this by making and explaining how to create a SIMPLE vJASS spell that follows the JESP standard.


To follow this tutorial you must know the basics of JASS language and you must have JNGP, which you can download here:

Jass NewGen Pack v5b - Wc3campaigns

##
 Differences between JASS and vJASS and JESP


When I first talk to people about vJASS, most of them don’t know what it is or the differences between JASS or vJASS. It is a normal question though.


As any JASSER knows, there are 2 levels defined as scripting languages that blizzard made:


- GUI


- JASS


The first one, GUI, provides the first experience for the beginner. It is easy to use and quite intuitive, however it is highly inefficient and leak and error prone.


JASS is converted GUI into a scripting language. Good JASSERS have more freedom to make spells and they have more control over the leaks and errors, so a good JASS spell is obviously better than a good GUI spell.


However, discovering JASS was still not enough for some people. Vexorian was one of those people. He though JASS was still not good enough, had many limitations and could need more features. So, Vexorian decided to expand JASS and to create another version for the language. After a few years of work he created an expansion for JASS called vJASS. So, vJASS is an extension to the JASS language that does everything JASS can do and a lot more. vJASS goal is to make JASS more object oriented and easier to use. Although true and real object oriented will never be able due the initial architecture of JASS, I must say Vexorian did a hell of a job and I advice everyone to learn it.


The "v" is for very and not for "Vexorian". Vexorian wanted to call it JASS++ however Zoxc suggested vJASS for veryJASS and Vexorian liked the idea. Thanks to Vexorian for explaining me the origin of the name.


Now, JESP is neither a language nor an expansion to JASS, vJASS or GUI. The reason why I explain JESP here is because many people (especially beginners) think that it is some sort of language, when it has nothing to do with it.


JESP is an agreement between the coding community that says “all spell must have a SETUP section in the code. Inside this SETUP section the user will be able to modify the spell without having the need to enter the spell’s core or without having the need to even understand it. A SETUP section can be anywhere inside the spell” among other small rules. Personally I like it better when my SETUP section is in the beginning of the spell.


JESP stands for “JASS Enhanced Spell Pseudotemplate”. Both spells made in JASS and in vJASS can (and in my opinion should) follow this JESP standard and one of the reasons why vJASS is so good is because it is easier to follow this standard using it. However, I never saw any GUI spell following it, and I don’t even think such thing is possible.


Here is the JESP official document that all spells following this standard should have:


 This is the JESP standard document, if a map contains this document it means that there are


 spells that follow this standard.


 Spells of this map that follow the standard:


¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯


- “Spell Name 1”


- "Spell Name 2"


- "Spell Name N"


 Advantages of the Standard


¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯


 - Implementing spells that follow the standard is relatively easier than implementing JASS


 spells that don't follow the standard.


 - Configuring/Balancing spells that follow the standard is relatively easier than


 implementing JASS spells that don't follow the standard.


 - Users may do the following procedure to make a new ability that uses the spell's script :


 * Create a new Trigger with a name (case sensitive)


 * Convert that trigger to custom text.


 * Copy the spell's script to a text editor like Notepad or your OS equivalent.


 * Replace the spell's Code name with the name you used on the trigger.


 * Copy the new text to the new trigger


 * Duplicate the Spell's original objects to have new ones for the new spell script.


 You are now able to use that new version of the spell.


 - In case two guys give the same name to 2 different spells, there are no conflict problems


 because you can easily change the name of one of them


 What is the JESP Standard?


¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯


 The JESP standard was designed to make spell sharing much better. And to make sure JASS


 enhanced spells follow a rule, to prevent chaos.


 What does JESP Standard stands for?


¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯


 JASS


 Enhanced


 Spell


 Pseudotemplate


 Requirements for a spell to follow the JESP Standard


¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯


 - The spell is written in JASS


 - The spell is 100% multi instanceable.


 - The spell script is ready to support spells of any number of levels.


 (default config header is not required to support all of them)


 - The Spell has an specific code name.


 - The Spell's trigger must have the spell's codename as name


 - The Spell's InitTrig function must be named: InitTrig_<CodeName>


 - The spell has a configuration header.


 - It is mandatory that rawcodes of objects are configurable in the header.


 - All the spell's specific code is inside the spell's "Trigger" (Trigger== that custom text


 slot that world editor calls Trigger, the spell may use as many 'trigger' OBJECTS as needed)


 - Every spell-specific single identifier or key works in such a way that reproducing the


 spell's trigger but after performing a text-replace of codename with another name (and thus


 renaming the cloned trigger to the new code name) it won't cause compile errors / conflicts


 when playing the map.


 - There is no code inside the spell's "Trigger" that is not specific to the spell.


 - There are no requirements for GUI variables that are specific to the spell. If a system


 used by the spell requires GUI variables the code for the system must be outside the "Trigger"


 - Eyecandy and spell's balance have to be easy to configure


 - The name of the author should be included in the spell's script.


 - The reason to exist of this standard is spell sharing. This document should be included within the map. And it should specify which spell follows the standard, in the top list.

##
 Basics of vJASS

###
 Free Global declaration


I will start with a very easy feature of vJASS, the free global declaration. If you are a decent JASSER, you know that when you want to create a global variable you have to use the Variable editor pressing Ctrl+B and every time you want to use the global you create you have to use it by typing “udg_globalName”. This is a pain in the ass, with multiple spells and multiple globals this will get pretty confusing pretty fast and you will eventually not know which spell uses which variable. Well, with vJASS and free global declaration the variable editor becomes completely useless.


To quote Vexorian:

 Warcraft III world editor always made everything harder for us, inluding declaring globals, you needed to use that dialog and it was really difficult to recreate global variables from a map or another and you were forced to use GUI for that. It was also impossible to declare globals of some types without modding world editor and then make your map completelly unopenable by normal world editor.


Global declaration freedom simply allows you to write globals blocks wherever you want, for example in the custom script section or in a 'trigger', and because of this you can even use the constant prefix which can even make finalizers able to inline constants and stuff.


Click to expand...


This is an example of a block declaration:

 JASS:

globals
 integer numberInt = 10
 real numberReal = 5. //5. = 5.00
 string text = "helloWorld!"
 boolean done = false
 timer t //this variable is not initialized with a value
 boolexpr condition //boolexpr stands for "boolen expression" and it allows us to use filters the easy way
endglobals


We use this declaration feature especially inside scopes (I will explain what a scope is in the next section) when making spells but as Vexorian said you can use it anywhere you want (including outside scopes).


As Vexorian said globals can also have the prefix “constant”. What does this mean? This means that our variables that are constant can NOT change their values ever. This is very useful when making a JESP spell as you will see in the end. Globals can also be “private” (I will discuss what private is, in the next sections). So I will give you an example of the use of globals:

 JASS:

globals
 constant integer SPELL_ID = 'A000' //constant variables should be in CAPS_LOCK, to indicate they don't change. This is a common practice among other programming languages such as C or Java.
 string text = "this is a string!"
 boolean b //not initialized
endglobals

function Actions takes nothing returns nothing
 set text = "Hello World is a better string dummy!"
 call BJDebugMsg(text)

 set SPELL_ID = 'A001' //ERROR, a constant variable can never change!

 set b = true //our global Boolean is now true
endfunction


This is very simple to use as you see. Soon things will get more and more complicated, in an exponential way, but when I show you my SIMPLE JESP vJASS spell you will see how all this information will be applied and you will see how easy this actually is.

###
 Scopes and Libraries


When I first teach my students the concept of a scope, they always ask me the concept of a library, and if should use it instead of a scope. This is a natural question, people usually hear the word “library” many times and they think that professional vJASSERS always use them. This is not truth however, scopes and libraries although similar, have different contexts to be used most of the time.

Scopes:


Scopes delimit a set of functions that share common purpose. Scopes are extremely useful and easy to use once we understand them.


We use scopes like this:

 JASS:

scope MySpell
 private function Conditions takes nothing returns boolean
 //return something
 endfunction

 private function Actions takes nothing returns nothing
 //code and stuff here
 endfunction
endscope


So what do scopes do?


Well, before I answer that, note the keyword “private” before the declaration of the function. I will explain in the next section what we use private for, but know for now that, for the following rules to apply, the functions MUST be private.


Scopes take action after you compile your project (by saving it). When you save your project, your function will be compiled, and during that stage all functions inside the tags “scope” and “endscope” will be renamed to “scopeName +randomDigit + __functionName” (as long as they are private). In this specific example of mine, after the compilation of the map we will have the following results:

 JASS:

function MySpell654__Conditions takes nothing returns boolean
 //return something
endfunction

function MySpell79__Actions takes nothing returns nothing
endfunction


This is extremely useful; in JASS you always must be careful with the names of your functions, because if inside a project with 10.000 functions, if there are only 2 functions with the same name, the map won’t compile correctly (this using JASS and regular WE). The use of scopes renames your functions when you press button save(remember, they must be private), so it gets really easy. This means you can have 100 functions with equal names, as long as they belong to scopes with different names. Note the random digits I picked are in reality random. This makes functions truly private in the meaning of the word because this way you can't access them from outside the scope. To quote the manual:

 The random digit is a way to let it be truly private so people can not even use them by adding the preffix themselves


Click to expand...


However know that we will go into further detail about the use of "private" in the next section.


Another example:


When writing your code, you write this:

 JASS:

scope MyFirstSpell
 private function Conditions takes nothing returns Boolean
 //return something
 endfunction

 private function Actions takes nothing returns nothing
 //stuff and code here
 endfunction
endscope

scope MySecondSpell
 private function Conditions takes nothing returns Boolean
 //return something
 endfunction

 private function Actions takes nothing returns nothing
 //bla bla bla xD
 endfunction
endscope


When you press save, the code is compiled, and what the computer will see is this:

 JASS:

function MyFirstSpell23__Conditions takes nothing returns boolean
 //return something
endfunction

function MyFirstSpell21__Actions takes nothing returns nothing
 //stuff and code here
endfunction

function MySecondSpell345__Conditions takes nothing returns boolean
 //return something
endfunction

function MySecondSpell56__Actions takes nothing returns nothing
 //bla bla bla xD
endfunction


So the functions actually have different names and everything will compile and run nice and easy.


Another very important feature of scopes is to define an Initializer. This is very important, in a JASS spell you always have a trigger that starts everything like “InitTrig_SpellName”. This trigger will be the first thing that will run when the map starts and so it must exist, and so it can be called an initializer.


With vJASS you can define initializers like this:

 JASS:

scope MySpell initializer InitFunctionName
 private function Conditions takes nothing returns boolean
 //return something
 endfunction

 private function Actions takes nothing returns nothing
 endfunction

 private function InitFunctionName takes nothing returns nothing
 endfunction
endscope


This is important when we use private functions, per example. However, for now just know we will use this important feature, you will understand the reason once I explain the use of “Private”.

Library:


A library does the exact same thing as a scope, except in a crucial difference – when you compile a library, after renaming the functions; it moves them to the map’s header . This feature is used because sometimes, when making general system, you want to make a specific set of functions to be assessable to all other functions of your map.


In our case, we are making a spell, therefore you do NOT want (or need) our functions to have such a feature. Thus, we will use a scope and not a library.


Libraries can also manage requirements which must be explicitly declared on the library. To define a library that requires other libraries we write:

 JASS:

library LibName requires REQ1, REQ2
 //code and functions and globals blocks
endlibrary

library REQ1
 //stuff here
endlibrary

library REQ2
 //stuff here
endlibrary


Note how I use the keyword "requires". Actually "requires" is just one of the three keywords you can use for this effect. You can replace "requires" with "uses" or "needs" that the final result will be the same; they all do the same job.


Libraries can also have initializers. If a library has an initializer and requirements then we do:

 JASS:

library LibName initializer Init requires REQ1, REQ2
 //code and functions and globals blocks
 function Init takes nothing returns nothing
 //note Init is NOT private so other functions from the map can access it
 endfunction
endlibrary

library REQ1
 //stuff here
endlibrary

library REQ2
 //stuff here
endlibrary


I won't go in much detail with libraries because I don't find them useful for what I plan to teach you. However you are free to visit the manual if you are curious or if you really want to learn.


But, when to use libraries or scopes? I usually tell my students the following rule:


- Scopes are for making spells


- Libraries are for making systems


Sure there can be some exceptions some times, but if you follow this simple set of rules, you will find yourself using scopes and libraries correctly in most situations .

###
 The use of “private”


Another very important thing when coding is protection. A good coder always protects his code from, per example, collisions. Using “private” is an excellent of doing so.


Before I explain what Private is, will explain how to use it.


Private keyword can be used in functions and global variables; at least this is how I use it most.


Example:

 JASS:

scope MySPell initialize Init
 globals
 private integer NUMBER = 10
 endglobals

 private function Actions takes nothing returns nothing
 endfunction

 private function Conditions takes nothing returns boolean
 return true //just to return something =P
 endfunction

 private function Init takes nothing returns nothing
 endfunction
endscope


Everything that contains the keyword “private” is confined to be used on the scope they belong to. This means that our global NUMBER and our functions “Actions, Conditions and Init” can only be used inside the scope “MySpell”. An attempt to call these functions from outside this scope will result in an error. This “encloses” a set of code with goal. This allows us to avoid collisions very easily. An example with globals will make this clear:

 JASS:

scope SpellOne initializer Init
 globals
 private constant integer ABILITY_ID = ‘A000’
 endglobals

 private function RawOne takes nothing returns nothing
 call BJDebugMsg(I2S(ABILITY_ID))
 endfunction

 private function Init takes nothing returns nothing
 call RawOne()
 endfunction
endscope

scope SpellTwo initializer Init
 globals
 private constant integer ABILITY_ID = ‘A001’
 endglobals

 private function RawTwo takes nothing returns nothing
 call BJDebugMsg(I2S(ABILITY_ID))
 endfunction

 private function Init takes nothing returns nothing
 call RawTwo()
 endfunction
endscope

scope SpellThree initializer Init
 private function Init takes nothing returns nothing
 call RawOne()
 call RawTwo()
 endfunction
endscope


So, if I call the function “RawOne” from outside the scope “SpellOne” I will get an error. If I call “RawTwo” from outside “SpellTwo” I will also get an error. This means that the piece of code I showed before will not compile, because the Init function inside the scope “SpellThree” does NOT have the authority to call “RawOne” and “RawTwo”. These two functions are “blocked” to the Init function inside “SpellThree”.


However, if I call “RawOne” from inside scope “SpellOne” the message “A000” will be printed in the screen. This happens with the Init function of “SpellOne”, this function has the authority to call “RawOne” because they are inside the same scope. Note that “RawOne” will print “A000” and NOT “A001”. “RawOne” can NOT access any global variables from outside his scope (because in this case all global variables are private), everything else is invisible for “RawOne” (unless, like in systems, we have public functions, but I will not mention those here I am ONLY considering the piece of code I created). So, for “SpellTwo” it’s “Init” function will call “RawTwo” and this function will print on the screen “A001”, because it is the only variable inside his scope which has the name “ABILITY_ID”.


I hope this helps you understand better what “private” does and what it is useful for.


Note that, although the variables are private, they are still global. Although other scopes can NOT access these variables, if 2 units cast the same spell at the same time, because you are running the same scope two times, you will have MUI problems. A global is always a global and it will always behave as one. The only difference private brings, is that you can give any name to them, that they won’t conflict.


Besides "private" there is nother important keyword called "public". "Public" is currently most used in systems together with libraries. Public has the inverse functionality of private, it makes a function accessable to all other functions of your map, except with a some differences, sepecialy in the "after compliation" part. I will not talk about his keyword as I don't find it important in spell making (I never used when making my spells) but, if you are interested, you should check the manual made by Vexorian.


PS: I2S(integer i) converts and integer to a string. This is a JASS function very useful for debug purposes.


 Last edited by a moderator: Apr 24, 2009

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

##
 Starting our spell


Well, to start I am going to make a SIMPLE spell using everything I tough you behind. I have in mind an AOE spell. I am a fan of AOE spells because in ladder they are essential to kill armies, which will grant you the final bonus. My AOE spell will have a different effect depending on the number of units in the AOE. I intend to do the following:


- If there are more allies than enemies, we heal all allies by X


- If there are more enemies, we damage the enemies for Y


- If the number of allies is equal to the number of enemies, we damage the enemies and heal the allies


So, the spell will be JESP and will use vJASS. To start I decided to use Silence. I want the spell to be instant so I will set its duration to 0.01. I will make this ability as dummy as possible, so I will also change the “Attacks prevented” field so they don’t prevent any attack at all. Instead of using silence you could use the "Channel" ability. To quote HINDYhat:

 Channel is the ultimate dummy ability. You can change the order id so if you have two spells based on Channel on the same caster, they won't conflict. Within the Channel spell definition, you can make it a Unit target spell, a Point target, an AoE or a Unit-Point target spell. You can also make it a unique cast... anyway you can do pretty much anything with it. You'd better check it out yourself. It's at default a neutral hostile hero spell.


Click to expand...

So, this ability allows you to create any kind of ability for your heroes with just a few twists. The channel ability is quite important for a coder's life so, although I don't use it on my spell, I think it is important enough for me to mention it. I also added the channel ability to my test map, right next to Instant Justice spell. You can have fun trying to find out its multiple uses.


With these fields set it is time to move to the coding.


First I press “CTRL + T” to create a new “trigger” (this is not an actual trigger but a trigger window, triggers are more complex things but for now that really doesn’t matter), and I will give it the name of “InstantJustice” (I think this is a cool name).


Now we convert it to custom text as usual and we delete everything inside it.


First let’s create our scope and our initializer. Inside our Init function we create our local trigger and set the actions and conditions. Why do we create a local trigger and why don’t we just use the global one?


Globals trigger:

 JASS:

function InitTrig_Untitled_Trigger_001 takes nothing returns nothing
 set gg_trg_Untitled_Trigger_001 = CreateTrigger( )
 call TriggerAddAction( gg_trg_Untitled_Trigger_001, function Trig_Untitled_Trigger_001_Actions )
endfunction


I create a local trigger because it is more portable. Vexorian himself suggested so. Local triggers in this case (making spells) are easier to deal with.


So, after creating the function and the trigger, we add the condition and the function to it.


After creating our Init function we create our empty functions “Actions and Conditions”. Note that everything will be inside our scope “InstantJustice”.


So, now we have something like this:

 JASS:

scope InstantJustice initializer Init
 private function Conditions takes nothing returns boolean
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals

 //preloading effects
 endfunction
endscope


Note that the name of the scope MUST much EXACTLY the name of the trigger, else it won’t work!


Also, you should see in my Init function I added two comments in the end. These comments will define a small area, which we will use soon.


Now, this may seem hard so far, stop, re-read, if you don’t understand make a post, I am here to help. Try to understand the logic, if need, review the previous chapters. The notions of scope and private are very important here and soon the notion of free global declaration will come in handy.


Now, let us start making the easy part – completing the Conditions function. In this function we check if our spell is the one being cast. Because we also want to make a JESP spell, it is the perfect time to make our SETUP section. This setup section will be the first thing to appear and it will have what we want to make customizable. I have in mind some things I would like the user to change:


1 - Rawcode of the spell


2 - Range of the spell


3 - Damage of the spell


4 - Healing of the spell


The 1st point is the rawcode of the spell. This rawcode will NEVER change during the game so it is a good idea to save it into a constant global. I want all other fields to change according to the level of the spell, so I need to use functions because globals can’t take arguments. Note that all functions and globals of our spell will be private.


So, now I am going to help, you should have something like this:

 JASS:

scope InstantJustice initializer Init
//===========================================================================
//=============================SETUP START===================================
//===========================================================================
 globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 endglobals

 private function Range takes integer level returns real
 //returns the range the spell will affect
 return level * 150.
 endfunction

 private function Heal takes integer level returns real
 //returns the heal allies will take
 return level * 75.
 endfunction

 private function Damage takes integer level returns real
 //returns the damage enemies will take
 return level * 50.
 endfunction
//===========================================================================
//=============================SETUP END=====================================
//===========================================================================

 private function Conditions takes nothing returns boolean
 return GetSpellAbilityId() == SPELL_ID
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )


 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals

 //preloading effects
 endfunction
endscope


Note I have delimited my SETUP section using comments. It is good to do this because this way our user will know what he needs to change and therefore he won’t go the spell’s core. I also took the freedom of choosing the return values and to complete our Conditions function. Now we he want to damage, heal, or pick units we will call these functions and give them level as a parameter.


When importing the spell, the only the user will have to change, will be the rawcode and the functions if he wants to.


So, I will give you an idea of the function Actions:


- Select all units in the target AOE


- Save those units into a group


- loop through the group counting allied units and enemy units


- enter an if-then-else statement that will do the actions


- end spell


However before selecting the units we must get the spell location. Blizzard screwed up here, there is no way to get spellTargetX and spelltargetY, we must first get the location and then extract the information from it. We must all nullify the location so it won’t leak.


It would also be a good idea to save the level into an integer local, and the caster into a local unit variable. Remember we will nullify caster in the end.


Here is the code so far:

 JASS:

scope InstantJustice initializer Init
//===========================================================================
//=============================SETUP START===================================
//===========================================================================
 globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 endglobals

 private function Range takes integer level returns real
 //returns the range the spell will affect
 return level * 150.
 endfunction

 private function Heal takes integer level returns real
 //returns the heal allies will take
 return level * 75.
 endfunction

 private function Damage takes integer level returns real
 //returns the damage enemies will take
 return level * 50.
 endfunction
//===========================================================================
//=============================SETUP END=====================================
//===========================================================================
 private function Conditions takes nothing returns boolean
 return GetSpellAbilityId() == SPELL_ID
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 local location spellLoc = GetSpellTargetLoc()
 local real spellX = GetLocationX(spellLoc)
 local real spellY = GetLocationY(spellLoc)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)


 call RemoveLocation(spellLoc)
 set spellLoc = null
 set caster = null
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals

 //preloading effects
 endfunction
endscope


Function Actions is not getting complicated so far. We will loop through a group so we are going to need a variable “f” to save the current unit (I use the FirstOfGroup way of doing things). We will also need a counter for the allies and the enemies, and this counters must start with zero. Let’s give a look to function Actions again:

 JASS:

private function Actions takes nothing returns nothing
 local location spellLoc = GetSpellTargetLoc()
 local real spellX = GetLocationX(spellLoc)
 local real spellY = GetLocationY(spellLoc)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)
 local unit f
 local integer allies = 0
 local integer enemies = 0

 call RemoveLocation(spellLoc)
 set spellLoc = null
 set caster = null
endfunction


Now things will complicate. This is the hard part of the spell, I hope you can understand the way I will explain things. If not, remember to make posts!


So as I said before we will need a group to save all units in the AOE. However, we have two problems:


- We don’t want to add all units to or group, per example, buildings and dead units are not a good choice


- We first need to loop thourgh the group to count the number of allies and enemies. Because we are using the FirstOfGroup method of doing things, this means that to move on we are forced to call “GroupRemoveUnit”. This way, in the end of the countdown, our group would have no units at all, which would be bad. Thus we conclude, we need an auxiliary group that will be a copy of our first group.


First things first. Lets create our group, and call it “all”. Many people just do “local group groupName = CreateGroup()” however every time someone casts a spell we will be creating a group. Let’s enter a world of magic where our spell becomes super famous. Imagine we have it on a footies map, and each footman has this ability. Imagine also that the map has 12 players, each one with 30 footmans. If all units cast the spell at the same time we would be forced to create and destroy 360 groups in a matter of milliseconds. This will obviously cause huge and massive lag, and we don’t want that. We can instead use 1 single global group. This group will be used in the core, and we have no intention to make it accessible to the user, so it will be in the core of our spell. This has a simple translation though – after your SETUP section end, create another globals block with a private group called “all”. It’s truth; a spell can have several global blocks. Now, as good practice, we do NOT initialize our group in the block. Thing is some natives can and will crash the game if settled automatically. CreateGroup is one of those evil natives. But don’t worry, we will initialize this global in our Init function. There is no problem because the game has already started.


Now, the code for you to understand it:

 JASS:

scope InstantJustice initializer Init
//===========================================================================
//=============================SETUP START===================================
//===========================================================================
 globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 endglobals

 private function Range takes integer level returns real
 //returns the range the spell will affect
 return level * 150.
 endfunction

 private function Heal takes integer level returns real
 //returns the heal allies will take
 return level * 75.
 endfunction

 private function Damage takes integer level returns real
 //returns the damage enemies will take
 return level * 50.
 endfunction
//===========================================================================
//=============================SETUP END=====================================
//===========================================================================
 globals
 private group all
 endglobals
//===========================================================================
 private function Conditions takes nothing returns boolean
 return GetSpellAbilityId() == SPELL_ID
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 local location spellLoc = GetSpellTargetLoc()
 local real spellX = GetLocationX(spellLoc)
 local real spellY = GetLocationY(spellLoc)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)
 local unit f
 local integer allies = 0
 local integer enemies = 0


 call RemoveLocation(spellLoc)
 set spellLoc = null
 set caster = null
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals
 set all = CreateGroup()

 //preloading effects
 endfunction
endscope


You see now in the Init function our Group is being set to “CreateGroup”. Now our spell will be more efficient and lighter. Some of you may think, is this spell still MUI? Well, yes it is. Why? There is an answer for this question which is quite complicated. As far as I understand Warcraft3 runs in threads, in a very low level. In this case two threads can’t run at the same time, so this spell is MUI enough to be playable.


Now that we have our global group created, we need a Boolean expression, so we can call “GroupEnumUnitsInRange”.


Again some people create and destroy boolexpr for no reason. According to my example in the magical world, we would have to create and destroy 360 local Boolean expressions. This is even more stupid, having in mind a Boolean expression is quite of a constant which will NEVER change in game, we can and should make of if a global as well. We use the same procedure. Inside the globals block we created earlier we add another variable of type “boolexpr” and name it “b”. We initialize this variable in the Init function as we did with the group. Now a boolean expression needs a function, a condition to call. I decided to create one called “Pick” and placed it above the function “Conditions”. For now function “Pick” only returns true, we shall change that value soon.


With our boolexpr created and our main group created as well, we can now call the function “GroupEnumUnitsInRange”.


So far the code is getting more complex. We have this:

 JASS:

scope InstantJustice initializer Init
//===========================================================================
//=============================SETUP START===================================
//===========================================================================
 globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 endglobals

 private function Range takes integer level returns real
 //returns the range the spell will affect
 return level * 150.
 endfunction

 private function Heal takes integer level returns real
 //returns the heal allies will take
 return level * 75.
 endfunction

 private function Damage takes integer level returns real
 //returns the damage enemies will take
 return level * 50.
 endfunction
//===========================================================================
//=============================SETUP END=====================================
//===========================================================================
 globals
 private group all
 private boolexpr b
 endglobals
//===========================================================================
 private function Pick takes nothing returns boolean
 return true
 endfunction
//===========================================================================
 private function Conditions takes nothing returns boolean
 return GetSpellAbilityId() == SPELL_ID
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 local location spellLoc = GetSpellTargetLoc()
 local real spellX = GetLocationX(spellLoc)
 local real spellY = GetLocationY(spellLoc)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)
 local unit f
 local integer allies = 0
 local integer enemies = 0

 call GroupEnumUnitsInRange(all, spellX, spellY, Range(level), b)

 call RemoveLocation(spellLoc)
 set spellLoc = null
 set caster = null
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals
 set all = CreateGroup()
 set b = Condition(function Pick)

 //preloading effects
 endfunction
endscope


Note how we set our boolexpr in the Init function. This is very handy, having a function that can initialize every global. In function Actions we added the call “GroupEnumUnitsInRange”. Note how I define the Range of the spell, by passing level as an argument to the SETUP function “Range”. This is how we make spells easy to edit. It isn’t that hard is it? However the hard part only begins now. Note that so far, we are picking ALL units inside the AOE, and as I said before, I want to make a selection. I think it is always a good idea to allow the user to select which units he wants the spell to affect. To fix this we could place function Pick inside the SETUP section, however by doing that we would force the user to deal with “GetFilterUnit()”. So, I decided to make the life easier for the user. Function “Pick” will call another function inside the SETUP section. This function will be called Targets and will take as an argument a unit “target” which is the filtered unit. If it seems complicated don’t worry, it is actually quite simple and I will show It now:

 JASS:

scope InstantJustice initializer Init
//===========================================================================
//=============================SETUP START===================================
//===========================================================================
 globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 endglobals

 private function Range takes integer level returns real
 //returns the range the spell will affect
 return level * 150.
 endfunction

 private function Heal takes integer level returns real
 //returns the heal allies will take
 return level * 75.
 endfunction

 private function Damage takes integer level returns real
 //returns the damage enemies will take
 return level * 50.
 endfunction

 private function Targets takes unit target returns boolean
 //the units the spell will affect
 return true
 endfunction
//===========================================================================
//=============================SETUP END=====================================
//===========================================================================
 globals
 private group all
 private boolexpr b
 endglobals
//===========================================================================
 private function Pick takes nothing returns boolean
 return Targets(GetFilterUnit())
 endfunction
//===========================================================================
 private function Conditions takes nothing returns boolean
 return GetSpellAbilityId() == SPELL_ID
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 local location spellLoc = GetSpellTargetLoc()
 local real spellX = GetLocationX(spellLoc)
 local real spellY = GetLocationY(spellLoc)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)
 local unit f
 local integer allies = 0
 local integer enemies = 0

 call GroupEnumUnitsInRange(all, spellX, spellY, Range(level), b)



 call RemoveLocation(spellLoc)
 set spellLoc = null
 set caster = null
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals
 set all = CreateGroup()
 set b = Condition(function Pick)

 //preloading effects
 endfunction
endscope


Note that now function Pick only makes a simple call. It will be in function Targets that we will actually pick our targets. However if you look right you see function Targets returns true, which means we still pick all units. Well, we don’t want that do we? So it’s time to choose which units we want to affect, or maybe easier, which units we do NOT want to affect. The negation is many times easier to deal with and in the case more simple.


So a quick list of units we don’t want to affect:


- Dead units


- Buildings


- Magic Immune units


- Mechanical Units


And I think this is enough, all other units will be affected.


Another thing I saw when teaching my students is that many of them use “GetUnitState(unit, state)” to know the life of a unit. Truth is that there is an easier and faster way, the “GetWidgetLife(widget)” function. A widget can be many things like a destructible or, in this case a unit. So we will use this function to check the life of a unit by typing “return (GetWidgetLife(target) > 0.405)”. Another common mistake is to think that dead units have <= 0 hitpoints. In Warcraft a unit actually dies when its life becomes < 0.405 and not 0. This is quite important and I am sure it will aid you in the rest of your coding life in wc3. Now the buildings, the magic immune units and the mechanical units are quite easy to deal with, and I am not going to explain them in detail.


Therefore I present you the final code of the function Targets:

 JASS:

private function Targets takes unit target returns boolean
 //the units the spell will affect
 return (GetWidgetLife(target) > 0.405) and (IsUnitType(target, UNIT_TYPE_STRUCTURE) == false) and (IsUnitType(target, UNIT_TYPE_MAGIC_IMMUNE) == false) and (IsUnitType(target, UNIT_TYPE_MECHANICAL) == false)
 endfunction


So, now that we pick the units we want to, it is time to go back to the function “Actions” where things will now get a little bit more complicated.


If you remember our second problem, you will know we need to make a copy of the group all. Well, for this effect we need to create an auxiliary group, also a global group, and name it “copy”. The initialization will be done in function Init as well. Now I present you with the code we have so far:

 JASS:

scope InstantJustice initializer Init
//===========================================================================
//=============================SETUP START===================================
//===========================================================================
 globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 endglobals

 private function Range takes integer level returns real
 //returns the range the spell will affect
 return level * 150.
 endfunction

 private function Heal takes integer level returns real
 //returns the heal allies will take
 return level * 75.
 endfunction

 private function Damage takes integer level returns real
 //returns the damage enemies will take
 return level * 50.
 endfunction

 private function Targets takes unit target returns boolean
 //the units the spell will affect
 return (GetWidgetLife(target) > 0.405) and (IsUnitType(target, UNIT_TYPE_STRUCTURE) == false) and (IsUnitType(target, UNIT_TYPE_MAGIC_IMMUNE) == false) and (IsUnitType(target, UNIT_TYPE_MECHANICAL) == false)
 endfunction
//===========================================================================
//=============================SETUP END=====================================
//===========================================================================
 globals
 private group all
 private group copy
 private boolexpr b
 endglobals
//===========================================================================
 private function Pick takes nothing returns boolean
 return Targets(GetFilterUnit())
 endfunction
//===========================================================================
 private function Conditions takes nothing returns boolean
 return GetSpellAbilityId() == SPELL_ID
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 local location spellLoc = GetSpellTargetLoc()
 local real spellX = GetLocationX(spellLoc)
 local real spellY = GetLocationY(spellLoc)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)
 local unit f
 local integer allies = 0
 local integer enemies = 0

 call GroupEnumUnitsInRange(all, spellX, spellY, Range(level), b)



 call RemoveLocation(spellLoc)
 set spellLoc = null
 set caster = null
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals
 set all = CreateGroup()
 set copy = CreateGroup()
 set b = Condition(function Pick)

 //preloading effects
 endfunction
endscope


Now, to make the copy, I am going to use a function made by a well known warcraft coder called Blade.dk. This function is called CopyGroup and is available in one of his tutorials, which belongs to a session he never finished. The link to his tutorial is in the bibliography. The function is the following:

 JASS:

 function CopyGroup takes group g returns group
 set bj_groupAddGroupDest = CreateGroup()
 call ForGroup(g, function GroupAddGroupEnum)
 return bj_groupAddGroupDest
 endfunction


So what does this function do? Well, this function receives a group as a parameter. Than it uses a bj global variable (BJ variables are NOT evil as many people think, they are actually faster. Only some BJ functions are evil) called “bj_groupAddGroupDest” which will become a group. This group will then receive all units the group passed as an argument has due the ForGroup. To finish Blade returns the bj global, now an exact copy of the group we passed as an argument.


Some people don’t like it, but truth is that we can and should use other people work if we find it good in order to make our OWN work better. However, remember an important thing, always give credits, else you are stealing. I will add this nice function to our code but I will comment that it was not made by me, but by Blade.dk and a place so people can know where to find more information about it.


Finally, remember we want this function to be renamed when we compile, we also don’t want other functions from outside the scope “InstantJustice” to access this function we are importing. So, we must make the function private.


Here is the code so far:

 JASS:

scope InstantJustice initializer Init
//===========================================================================
//=============================SETUP START===================================
//===========================================================================
 globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 endglobals

 private function Range takes integer level returns real
 //returns the range the spell will affect
 return level * 150.
 endfunction

 private function Heal takes integer level returns real
 //returns the heal allies will take
 return level * 75.
 endfunction

 private function Damage takes integer level returns real
 //returns the damage enemies will take
 return level * 50.
 endfunction

 private function Targets takes unit target returns boolean
 //the units the spell will affect
 return (GetWidgetLife(target) > 0.405) and (IsUnitType(target, UNIT_TYPE_STRUCTURE) == false) and (IsUnitType(target, UNIT_TYPE_MAGIC_IMMUNE) == false) and (IsUnitType(target, UNIT_TYPE_MECHANICAL) == false)
 endfunction
//===========================================================================
//=============================SETUP END=====================================
//===========================================================================
 globals
 private group all
 private group copy
 private boolexpr b
 endglobals
//===========================================================================
//Function made by Blade.dk; Search for [url=http://www.wc3campaigns.com]wc3campaigns.com[/url] for more info
 private function CopyGroup takes group g returns group
 set bj_groupAddGroupDest = CreateGroup()
 call ForGroup(g, function GroupAddGroupEnum)
 return bj_groupAddGroupDest
 endfunction
//===========================================================================
 private function Pick takes nothing returns boolean
 return Targets(GetFilterUnit())
 endfunction
//===========================================================================
 private function Conditions takes nothing returns boolean
 return GetSpellAbilityId() == SPELL_ID
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 local location spellLoc = GetSpellTargetLoc()
 local real spellX = GetLocationX(spellLoc)
 local real spellY = GetLocationY(spellLoc)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)
 local unit f
 local integer allies = 0
 local integer enemies = 0

 call GroupEnumUnitsInRange(all, spellX, spellY, Range(level), b)


 call RemoveLocation(spellLoc)
 set spellLoc = null
 set caster = null
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals
 set all = CreateGroup()
 set copy = CreateGroup()
 set b = Condition(function Pick)

 //preloading effects
 endfunction
endscope


Now, that we have everything we need, I say let’s finish the damn spell. The tutorial is already 29 pages long and I am getting tired of writing this much =P (actually I am not tired but meh lol).


So, remember we have a group ready to be used, the “copy” group. So before we do the effect to the units we have to count them, and this is why we need the auxiliary group.


All this information results in the following:


1 –Copy group “all” to group “copy”


2 - Enter a “FirstOfGroup” loop where we make the counting


3 – For each allied unit we increment the allies’ integer, and for each enemy unit we increment the enemies integer.


4 - We exit the loop and we make the effect


So, this pseudo code now transforms into the following:

 JASS:

scope InstantJustice initializer Init
//===========================================================================
//=============================SETUP START===================================
//===========================================================================
 globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 endglobals

 private function Range takes integer level returns real
 //returns the range the spell will affect
 return level * 150.
 endfunction

 private function Heal takes integer level returns real
 //returns the heal allies will take
 return level * 75.
 endfunction

 private function Damage takes integer level returns real
 //returns the damage enemies will take
 return level * 50.
 endfunction

 private function Targets takes unit target returns boolean
 //the units the spell will affect
 return (GetWidgetLife(target) > 0.405) and (IsUnitType(target, UNIT_TYPE_STRUCTURE) == false) and (IsUnitType(target, UNIT_TYPE_MAGIC_IMMUNE) == false) and (IsUnitType(target, UNIT_TYPE_MECHANICAL) == false)
 endfunction
//===========================================================================
//=============================SETUP END=====================================
//===========================================================================
 globals
 private group all
 private group copy
 private boolexpr b
 endglobals
//===========================================================================
//Function made by Blade.dk; Search for [url=http://www.wc3campaigns.com]wc3campaigns.com[/url] for more info
 private function CopyGroup takes group g returns group
 set bj_groupAddGroupDest = CreateGroup()
 call ForGroup(g, function GroupAddGroupEnum)
 return bj_groupAddGroupDest
 endfunction
//===========================================================================
 private function Pick takes nothing returns boolean
 return Targets(GetFilterUnit())
 endfunction
//===========================================================================
 private function Conditions takes nothing returns boolean
 return GetSpellAbilityId() == SPELL_ID
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 local location spellLoc = GetSpellTargetLoc()
 local real spellX = GetLocationX(spellLoc)
 local real spellY = GetLocationY(spellLoc)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)
 local unit f
 local integer allies = 0
 local integer enemies = 0

 call GroupEnumUnitsInRange(all, spellX, spellY, Range(level), b)
 set copy = CopyGroup(all)
 loop
 set f = FirstOfGroup(copy)
 exitwhen(f == null)
 call GroupRemoveUnit(copy, f)
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 set allies = allies + 1
 else
 set enemies = enemies + 1
 endif
 endloop



 call RemoveLocation(spellLoc)
 set spellLoc = null
 set caster = null
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals
 set all = CreateGroup()
 set copy = CreateGroup()
 set b = Condition(function Pick)

 //preloading effects
 endfunction
endscope


It is very important for you to understand how the loop works. I assume that as a JASSER you know this basic trick, if you don’t remember to post for help. Note that when we exit the loop the unit variable “f” is null, so we never need to nullify it, and even better we can reuse it.


Now that we made the counting, we can do the effects, heal and damage. For this effect I decided to use an if statement with several loops inside it. It is quite simple:

 JASS:

if allies > enemies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //heal allies
 endloop
 elseif enemies > allies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //damage enemies
 endloop
 elseif allies == enemies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //heal allies and damage enemies
 endloop
 endif


Now the only thing we need to do is to simulate the healing and the damage. I will start by the healing.


Again, many people use “SetUnitState” do simulate healing. Although it is not wrong, it is not the best way. I use “SetWidgetLife”because it is easier to understand and it is faster. So, our healing line will be the following:

 JASS:

call SetWidgetLife(f, GetWidgetLife(f) + Heal(level))

You see I use GetWidgetLife here. I also use the function in the SETUP “Heal” and pass it level as a parameter so it is easy to change.


Now moving to the next if, we want to damage the enemies. Some of you may think “Hey we can use SetWIdgetLife” again. Answer is NO, you should never use “SetWidgetLife” to cause damage. Why? Well, there is a reason for that. If a unit dies due the damage “SetWidgetLife” caused, the computer will not know the killer. This is especially bad when you need a multiboard to count the number of kills per example, in such a case, the counter wouldn’t increment unless you wanted to add the multiboards code to all your spells, which would be a very bad idea. To simulate damage the best function I always use is the “UnitDamageTarget”. This function follows me since GUI and it is very useful in most cases. This way, our damaging line:

 JASS:

call UnitDamageTarget(caster, f, Damage(level), true, false, ATTACK_TYPE_MAGIC, DAMAGE_TYPE_NORMAL, null)

Finally and moving to the last case, we heal and damage using this same lines.


Now, do not forget we need to use another if statement inside the loops, to verify if we are healing allies and damaging enemies. So, with no further delay, I show you all how my if statement goes:

 JASS:

if allies > enemies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //heal allies
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 call SetWidgetLife(f, GetWidgetLife(f) + Heal(level))
 endif
 endloop
 elseif enemies > allies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //damage enemies
 if IsUnitEnemy(f, GetOwningPlayer(caster)) then
 call UnitDamageTarget(caster, f, Damage(level), true, false, ATTACK_TYPE_MAGIC, DAMAGE_TYPE_NORMAL, null)
 endif
 endloop
 elseif allies == enemies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //heal allies
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 call SetWidgetLife(f, GetWidgetLife(f) + Heal(level))
 //if an unit is not an ally than it is an enemy
 else
 call UnitDamageTarget(caster, f, Damage(level), true, false, ATTACK_TYPE_MAGIC, DAMAGE_TYPE_NORMAL, null)
 endif
 endloop
 endif


It may look very complicated at first, but note it is quite easy. In every case we always clean the “all” group and f always stays null at the end of the iteration.


Now the heart of our spell is done. You can test it right away if you want. However you should know that most users are newbs and so they don’t care if a spell is good coded, they don’t even know how to see that. They care only about how “cool” the spell is. A way of making our spells cool is by adding effects. For the sake of learning, let0s add some effects!


I have an idea of what I want to use and when:


- When casting the AOE spell, I want to use the Wisp detonate model


- When healing the allies think the holly bolt model is nice


- When damaging the enemies I think a thunder effect will fit


Now, because this is a JESP spell we want the user to have the ability to choose the effects. How to do that? Well, we save the path of the effects in a string. This string will never change inside game, so it can be (and should be) a private constant string. We will place this string in our SETUP section because we want the user to change it.


So now the global block from our SETUP section looks like this:

 JASS:

globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 private constant string AOE_EFFECT = "Units\\NightElf\\Wisp\\WispExplode.mdl"
 private constant string HEAL_EFFECT = "Abilities\\Spells\\Human\\HolyBolt\\HolyBoltSpecialArt.mdl"
 private constant string DAMAGE_EFFECT = "Abilities\\Weapons\\Bolt\\BoltImpact.mdl"
 endglobals


Now we need to go to function Actions and add the call to create the effects.


For a matter of efficiency, I will create and destroy the effects in one line:

 JASS:

call DestroyEffect(AddSpecialEffect(AOE_EFFECT, spellX, spellY))


This will create and destroy the special effect for the AOE in one line.


For the healing and damaging effect I will use something similar, but not equal:

 JASS:

call DestroyEffect(AddSpecialEffectTarget(effectName, f, “origin”))


Now we just have to add the lines to the correct places and “voalá” our spell becomes cool =P


With no further delay, here is the code I have done so far:

 JASS:

scope InstantJustice initializer Init
//===========================================================================
//=============================SETUP START===================================
//===========================================================================
 globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 private constant string AOE_EFFECT = "Units\\NightElf\\Wisp\\WispExplode.mdl"
 private constant string HEAL_EFFECT = "Abilities\\Spells\\Human\\HolyBolt\\HolyBoltSpecialArt.mdl"
 private constant string DAMAGE_EFFECT = "Abilities\\Weapons\\Bolt\\BoltImpact.mdl"
 endglobals

 private function Range takes integer level returns real
 //returns the range the spell will affect
 return level * 150.
 endfunction

 private function Heal takes integer level returns real
 //returns the heal allies will take
 return level * 75.
 endfunction

 private function Damage takes integer level returns real
 //returns the damage enemies will take
 return level * 50.
 endfunction

 private function Targets takes unit target returns boolean
 //the units the spell will affect
 return (GetWidgetLife(target) > 0.405) and (IsUnitType(target, UNIT_TYPE_STRUCTURE) == false) and (IsUnitType(target, UNIT_TYPE_MAGIC_IMMUNE) == false) and (IsUnitType(target, UNIT_TYPE_MECHANICAL) == false)
 endfunction
//===========================================================================
//=============================SETUP END=====================================
//===========================================================================
 globals
 private group all
 private group copy
 private boolexpr b
 endglobals
//===========================================================================
//Function made by Blade.dk; Search for [url=http://www.wc3campaigns.com]wc3campaigns.com[/url] for more info
 private function CopyGroup takes group g returns group
 set bj_groupAddGroupDest = CreateGroup()
 call ForGroup(g, function GroupAddGroupEnum)
 return bj_groupAddGroupDest
 endfunction
//===========================================================================
 private function Pick takes nothing returns boolean
 return Targets(GetFilterUnit())
 endfunction
//===========================================================================
 private function Conditions takes nothing returns boolean
 return GetSpellAbilityId() == SPELL_ID
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 local location spellLoc = GetSpellTargetLoc()
 local real spellX = GetLocationX(spellLoc)
 local real spellY = GetLocationY(spellLoc)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)
 local unit f
 local integer allies = 0
 local integer enemies = 0

 //create the AOE effect
 call DestroyEffect(AddSpecialEffect(AOE_EFFECT, spellX, spellY))

 //counting the units
 call GroupEnumUnitsInRange(all, spellX, spellY, Range(level), b)
 set copy = CopyGroup(all)
 loop
 set f = FirstOfGroup(copy)
 exitwhen(f == null)

 call GroupRemoveUnit(copy, f)
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 set allies = allies + 1
 else
 set enemies = enemies + 1
 endif
 endloop

 //making the effect of the spell
 if allies > enemies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //heal allies
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 call DestroyEffect(AddSpecialEffectTarget(HEAL_EFFECT, f, "origin")) //the healing effect
 call SetWidgetLife(f, GetWidgetLife(f) + Heal(level))
 endif
 endloop
 elseif enemies > allies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //damage enemies
 if IsUnitEnemy(f, GetOwningPlayer(caster)) then
 call DestroyEffect(AddSpecialEffectTarget(DAMAGE_EFFECT, f, "origin")) //the damaging effect
 call UnitDamageTarget(caster, f, Damage(level), true, false, ATTACK_TYPE_MAGIC, DAMAGE_TYPE_NORMAL, null)
 endif
 endloop
 elseif allies == enemies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //heal allies
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 call DestroyEffect(AddSpecialEffectTarget(HEAL_EFFECT, f, "origin"))
 call SetWidgetLife(f, GetWidgetLife(f) + Heal(level))
 //if an unit is not an ally than it is an enemy
 else
 call DestroyEffect(AddSpecialEffectTarget(DAMAGE_EFFECT, f, "origin"))
 call UnitDamageTarget(caster, f, Damage(level), true, false, ATTACK_TYPE_MAGIC, DAMAGE_TYPE_NORMAL, null)
 endif
 endloop
 endif

 call RemoveLocation(spellLoc)
 set spellLoc = null
 set caster = null
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals
 set all = CreateGroup()
 set copy = CreateGroup()
 set b = Condition(function Pick)

 //preloading effects
 endfunction
endscope


Now our spell is cool but, it will lag the first time we cast it. To solve this we must preload everything! We must preload:


1 – the effects


2 – the ability


Preloading the effects will be quite easy. The difficult part of the job is done. To preload an effect we just need to type:

 JASS:

call Preload(effectName)

In the Init function.


So, preloading our effect will look like this:

 JASS:

 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals
 set all = CreateGroup()
 set copy = CreateGroup()
 set b = Condition(function Pick)

 //preloading effects
 call Preload(AOE_EFFECT)
 call Preload(HEAL_EFFECT)
 call Preload(DAMAGE_EFFECT)

 endfunction


Now we need to preload the ability. Preloading an ability is quite complex. You can do that by using an auxiliary system such as “xe” (see links in Bibliography) or doing it manually, as I will do. To preload an ability we must add it to an unit in the Init function. So, this means we need a dummy unit. We can use anything as a dummy unit, I will create one by using a peon and removing all abilities he has. The dummy will also have the ability locust. The use of a dummy unit also implies that we must place its rawcode in the SETUP function, so it is easy to change.


The code will be this:

 JASS:

scope InstantJustice initializer Init
//===========================================================================
//=============================SETUP START===================================
//===========================================================================
 globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 private constant integer DUMMY_ID = 'h000' //rw of the dummy unit
 private constant string AOE_EFFECT = "Units\\NightElf\\Wisp\\WispExplode.mdl"
 private constant string HEAL_EFFECT = "Abilities\\Spells\\Human\\HolyBolt\\HolyBoltSpecialArt.mdl"
 private constant string DAMAGE_EFFECT = "Abilities\\Weapons\\Bolt\\BoltImpact.mdl"
 endglobals

 private function Range takes integer level returns real
 //returns the range the spell will affect
 return level * 150.
 endfunction

 private function Heal takes integer level returns real
 //returns the heal allies will take
 return level * 75.
 endfunction

 private function Damage takes integer level returns real
 //returns the damage enemies will take
 return level * 50.
 endfunction

 private function Targets takes unit target returns boolean
 //the units the spell will affect
 return (GetWidgetLife(target) > 0.405) and (IsUnitType(target, UNIT_TYPE_STRUCTURE) == false) and (IsUnitType(target, UNIT_TYPE_MAGIC_IMMUNE) == false) and (IsUnitType(target, UNIT_TYPE_MECHANICAL) == false)
 endfunction
//===========================================================================
//=============================SETUP END=====================================
//===========================================================================
 globals
 private group all
 private group copy
 private boolexpr b
 endglobals
//===========================================================================
//Function made by Blade.dk; Search for [url=http://www.wc3campaigns.com]wc3campaigns.com[/url] for more info
 private function CopyGroup takes group g returns group
 set bj_groupAddGroupDest = CreateGroup()
 call ForGroup(g, function GroupAddGroupEnum)
 return bj_groupAddGroupDest
 endfunction
//===========================================================================
 private function Pick takes nothing returns boolean
 return Targets(GetFilterUnit())
 endfunction
//===========================================================================
 private function Conditions takes nothing returns boolean
 return GetSpellAbilityId() == SPELL_ID
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 local location spellLoc = GetSpellTargetLoc()
 local real spellX = GetLocationX(spellLoc)
 local real spellY = GetLocationY(spellLoc)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)
 local unit f
 local integer allies = 0
 local integer enemies = 0

 //create the AOE effect
 call DestroyEffect(AddSpecialEffect(AOE_EFFECT, spellX, spellY))

 //counting the units
 call GroupEnumUnitsInRange(all, spellX, spellY, Range(level), b)
 set copy = CopyGroup(all)
 loop
 set f = FirstOfGroup(copy)
 exitwhen(f == null)
 call GroupRemoveUnit(copy, f)
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 set allies = allies + 1
 else
 set enemies = enemies + 1
 endif
 endloop

 //making the effect of the spell
 if allies > enemies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //heal allies
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 call DestroyEffect(AddSpecialEffectTarget(HEAL_EFFECT, f, "origin")) //the healing effect
 call SetWidgetLife(f, GetWidgetLife(f) + Heal(level))
 endif
 endloop
 elseif enemies > allies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //damage enemies
 if IsUnitEnemy(f, GetOwningPlayer(caster)) then
 call DestroyEffect(AddSpecialEffectTarget(DAMAGE_EFFECT, f, "origin")) //the damaging effect
 call UnitDamageTarget(caster, f, Damage(level), true, false, ATTACK_TYPE_MAGIC, DAMAGE_TYPE_NORMAL, null)
 endif
 endloop
 elseif allies == enemies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //heal allies
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 call DestroyEffect(AddSpecialEffectTarget(HEAL_EFFECT, f, "origin"))
 call SetWidgetLife(f, GetWidgetLife(f) + Heal(level))
 //if an unit is not an ally than it is an enemy
 else
 call DestroyEffect(AddSpecialEffectTarget(DAMAGE_EFFECT, f, "origin"))
 call UnitDamageTarget(caster, f, Damage(level), true, false, ATTACK_TYPE_MAGIC, DAMAGE_TYPE_NORMAL, null)
 endif
 endloop
 endif

 call RemoveLocation(spellLoc)
 set spellLoc = null
 set caster = null
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals
 set all = CreateGroup()
 set copy = CreateGroup()
 set b = Condition(function Pick)

 //preloading effects
 call Preload(AOE_EFFECT)
 call Preload(HEAL_EFFECT)
 call Preload(DAMAGE_EFFECT)

 //preloading the ability
 set bj_lastCreatedUnit = CreateUnit(Player(PLAYER_NEUTRAL_PASSIVE), DUMMY_ID, 0, 0, 0)
 call UnitAddAbility(bj_lastCreatedUnit, SPELL_ID)
 call KillUnit(bj_lastCreatedUnit)

 endfunction
endscope


Now the important thing to note is that the globals block in the SETUP section now has another variable “DUMMY_ID” and that inside the Init function I’ve added a preloading ability area. Note that I am using a bj variable that many of us know, the bj_lastCreateUnit. Preloading an ability is as simple as that:


- Create a unit


- Add the ability to the unit


- Kill the unit


Now our spell is DONE!!!! (Finally, the tutorial is 43 pages long so far).


Now we have a cool efficient spell.


There are more things we can change though, per example some people like to choose the damage type or the attack type the spell will cause. We can easily allow this by using globals in the SETUP section:

 JASS:

globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 private constant integer DUMMY_ID = 'h000' //rw of the dummy unit
 private constant string AOE_EFFECT = "Units\\NightElf\\Wisp\\WispExplode.mdl"
 private constant string HEAL_EFFECT = "Abilities\\Spells\\Human\\HolyBolt\\HolyBoltSpecialArt.mdl"
 private constant string DAMAGE_EFFECT = "Abilities\\Weapons\\Bolt\\BoltImpact.mdl"
 private constant damagetype D_TYPE = DAMAGE_TYPE_NORMAL
 private constant attacktype A_TYPE = ATTACK_TYPE_MAGIC
 endglobals


Magic of vJASS; you can0t have this variables in the variable editor of WE =D


Now the only thing we have to change is the “UnitDamageTarget” function to use these variables.


Here is the code:

 JASS:

scope InstantJustice initializer Init
//===========================================================================
//=============================SETUP START===================================
//===========================================================================
 globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 private constant integer DUMMY_ID = 'h000' //rw of the dummy unit
 private constant string AOE_EFFECT = "Units\\NightElf\\Wisp\\WispExplode.mdl" //effect that will be created when we cast the spell on the AOE
 private constant string HEAL_EFFECT = "Abilities\\Spells\\Human\\HolyBolt\\HolyBoltSpecialArt.mdl" //effect that will be created when we heal units
 private constant string DAMAGE_EFFECT = "Abilities\\Weapons\\Bolt\\BoltImpact.mdl" //effect that will be created when we damage units
 private constant damagetype D_TYPE = DAMAGE_TYPE_NORMAL //the attack type of the spell
 private constant attacktype A_TYPE = ATTACK_TYPE_MAGIC //the damage type of the spell
 endglobals

 private function Range takes integer level returns real
 //returns the range the spell will affect
 return level * 150.
 endfunction

 private function Heal takes integer level returns real
 //returns the heal allies will take
 return level * 75.
 endfunction

 private function Damage takes integer level returns real
 //returns the damage enemies will take
 return level * 50.
 endfunction

 private function Targets takes unit target returns boolean
 //the units the spell will affect
 return (GetWidgetLife(target) > 0.405) and (IsUnitType(target, UNIT_TYPE_STRUCTURE) == false) and (IsUnitType(target, UNIT_TYPE_MAGIC_IMMUNE) == false) and (IsUnitType(target, UNIT_TYPE_MECHANICAL) == false)
 endfunction
//===========================================================================
//=============================SETUP END=====================================
//===========================================================================
 globals
 private group all
 private group copy
 private boolexpr b
 endglobals
//===========================================================================
//Function made by Blade.dk; Search for [url=http://www.wc3campaigns.com]wc3campaigns.com[/url] for more info
 private function CopyGroup takes group g returns group
 set bj_groupAddGroupDest = CreateGroup()
 call ForGroup(g, function GroupAddGroupEnum)
 return bj_groupAddGroupDest
 endfunction
//===========================================================================
 private function Pick takes nothing returns boolean
 return Targets(GetFilterUnit())
 endfunction
//===========================================================================
 private function Conditions takes nothing returns boolean
 return GetSpellAbilityId() == SPELL_ID
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 local location spellLoc = GetSpellTargetLoc()
 local real spellX = GetLocationX(spellLoc)
 local real spellY = GetLocationY(spellLoc)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)
 local unit f
 local integer allies = 0
 local integer enemies = 0

 //create the AOE effect
 call DestroyEffect(AddSpecialEffect(AOE_EFFECT, spellX, spellY))

 //counting the units
 call GroupEnumUnitsInRange(all, spellX, spellY, Range(level), b)
 set copy = CopyGroup(all)
 loop
 set f = FirstOfGroup(copy)
 exitwhen(f == null)
 call GroupRemoveUnit(copy, f)
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 set allies = allies + 1
 else
 set enemies = enemies + 1
 endif
 endloop

 //making the effect of the spell
 if allies > enemies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //heal allies
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 call DestroyEffect(AddSpecialEffectTarget(HEAL_EFFECT, f, "origin")) //the healing effect
 call SetWidgetLife(f, GetWidgetLife(f) + Heal(level))
 endif
 endloop
 elseif enemies > allies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //damage enemies
 if IsUnitEnemy(f, GetOwningPlayer(caster)) then
 call DestroyEffect(AddSpecialEffectTarget(DAMAGE_EFFECT, f, "origin")) //the damaging effect
 call UnitDamageTarget(caster, f, Damage(level), true, false, A_TYPE, D_TYPE, null)
 endif
 endloop
 elseif allies == enemies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //heal allies
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 call DestroyEffect(AddSpecialEffectTarget(HEAL_EFFECT, f, "origin"))
 call SetWidgetLife(f, GetWidgetLife(f) + Heal(level))
 //if an unit is not an ally than it is an enemy
 else
 call DestroyEffect(AddSpecialEffectTarget(DAMAGE_EFFECT, f, "origin"))
 call UnitDamageTarget(caster, f, Damage(level), true, false, A_TYPE, D_TYPE, null)
 endif
 endloop
 endif

 call RemoveLocation(spellLoc)
 set spellLoc = null
 set caster = null
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals
 set all = CreateGroup()
 set copy = CreateGroup()
 set b = Condition(function Pick)

 //preloading effects
 call Preload(AOE_EFFECT)
 call Preload(HEAL_EFFECT)
 call Preload(DAMAGE_EFFECT)

 //preloading the ability
 set bj_lastCreatedUnit = CreateUnit(Player(PLAYER_NEUTRAL_PASSIVE), DUMMY_ID, 0, 0, 0)
 call UnitAddAbility(bj_lastCreatedUnit, SPELL_ID)
 call KillUnit(bj_lastCreatedUnit)

 endfunction
endscope


Now, at last but not least, the most important thing in a spell to prevent stealing: a header.


Many people don’t use headers, this is a mistake. Although we cannot prevent other people to delete them, it is always good to have them, because we can place there important information about our spells. If people try to delete it and steal the spell, they will lose the important information. Non less it always good to know because it also makes it easier to get credits for the work done. I decided to use this stylish header I made:

 JASS:

//===========================================================================
//A vJASS and JESP spell that will have an effetc depending on the number
//of units in the AOE. If there are more allies than enemies, we heal allies
//if there are more enemies that allies we damage enemies, if the number of
//allies equals the number of enemies we heal the allies and damage the enemies
//
//@author Flame_Phoenix
//
//@credits
//- Blade.dk, for the CopyCroup function
//
//@version 1.0
//===========================================================================


 Last edited by a moderator: Apr 24, 2009

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Using headers is always good, don’t you forget about that!


Now with no further delay I will present you all the final version of the code:

 JASS:

//===========================================================================
//A vJASS and JESP spell that will have an effetc depending on the number
//of units in the AOE. If there are more allies than enemies, we heal allies
//if there are more enemies that allies we damage enemies, if the number of
//allies equals the number of enemies we heal the allies and damage the enemies
//
//@author Flame_Phoenix
//
//@credits
//- Blade.dk, for the CopyCroup function
//
//@version 1.0
//===========================================================================
scope InstantJustice initializer Init
//===========================================================================
//=============================SETUP START===================================
//===========================================================================
 globals
 private constant integer SPELL_ID = 'ANsi' //the rawcode of the spell
 private constant integer DUMMY_ID = 'h000' //rw of the dummy unit
 private constant string AOE_EFFECT = "Units\\NightElf\\Wisp\\WispExplode.mdl" //effect that will be created when we cast the spell on the AOE
 private constant string HEAL_EFFECT = "Abilities\\Spells\\Human\\HolyBolt\\HolyBoltSpecialArt.mdl" //effect that will be created when we heal units
 private constant string DAMAGE_EFFECT = "Abilities\\Weapons\\Bolt\\BoltImpact.mdl" //effect that will be created when we damage units
 private constant damagetype D_TYPE = DAMAGE_TYPE_NORMAL //the attack type of the spell
 private constant attacktype A_TYPE = ATTACK_TYPE_MAGIC //the damage type of the spell
 endglobals

 private function Range takes integer level returns real
 //returns the range the spell will affect
 return level * 150.
 endfunction

 private function Heal takes integer level returns real
 //returns the heal allies will take
 return level * 75.
 endfunction

 private function Damage takes integer level returns real
 //returns the damage enemies will take
 return level * 50.
 endfunction

 private function Targets takes unit target returns boolean
 //the units the spell will affect
 return (GetWidgetLife(target) > 0.405) and (IsUnitType(target, UNIT_TYPE_STRUCTURE) == false) and (IsUnitType(target, UNIT_TYPE_MAGIC_IMMUNE) == false) and (IsUnitType(target, UNIT_TYPE_MECHANICAL) == false)
 endfunction
//===========================================================================
//=============================SETUP END=====================================
//===========================================================================
 globals
 private group all
 private group copy
 private boolexpr b
 endglobals
//===========================================================================
//Function made by Blade.dk; Search for [url=http://www.wc3campaigns.com]wc3campaigns.com[/url] for more info
 private function CopyGroup takes group g returns group
 set bj_groupAddGroupDest = CreateGroup()
 call ForGroup(g, function GroupAddGroupEnum)
 return bj_groupAddGroupDest
 endfunction
//===========================================================================
 private function Pick takes nothing returns boolean
 return Targets(GetFilterUnit())
 endfunction
//===========================================================================
 private function Conditions takes nothing returns boolean
 return GetSpellAbilityId() == SPELL_ID
 endfunction
//===========================================================================
 private function Actions takes nothing returns nothing
 local location spellLoc = GetSpellTargetLoc()
 local real spellX = GetLocationX(spellLoc)
 local real spellY = GetLocationY(spellLoc)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)
 local unit f
 local integer allies = 0
 local integer enemies = 0

 //create the AOE effect
 call DestroyEffect(AddSpecialEffect(AOE_EFFECT, spellX, spellY))

 //counting the units
 call GroupEnumUnitsInRange(all, spellX, spellY, Range(level), b)
 set copy = CopyGroup(all)
 loop
 set f = FirstOfGroup(copy)
 exitwhen(f == null)
 call GroupRemoveUnit(copy, f)
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 set allies = allies + 1
 else
 set enemies = enemies + 1
 endif
 endloop

 //making the effect of the spell
 if allies > enemies then


 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //heal allies
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 call DestroyEffect(AddSpecialEffectTarget(HEAL_EFFECT, f, "origin")) //the healing effect
 call SetWidgetLife(f, GetWidgetLife(f) + Heal(level))
 endif
 endloop
 elseif enemies > allies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //damage enemies
 if IsUnitEnemy(f, GetOwningPlayer(caster)) then
 call DestroyEffect(AddSpecialEffectTarget(DAMAGE_EFFECT, f, "origin")) //the damaging effect
 call UnitDamageTarget(caster, f, Damage(level), true, false, A_TYPE, D_TYPE, null)
 endif
 endloop
 elseif allies == enemies then
 loop
 set f = FirstOfGroup(all)
 exitwhen (f == null)
 call GroupRemoveUnit(all, f)
 //heal allies
 if IsUnitAlly(f, GetOwningPlayer(caster)) then
 call DestroyEffect(AddSpecialEffectTarget(HEAL_EFFECT, f, "origin"))
 call SetWidgetLife(f, GetWidgetLife(f) + Heal(level))
 //if an unit is not an ally than it is an enemy
 else
 call DestroyEffect(AddSpecialEffectTarget(DAMAGE_EFFECT, f, "origin"))
 call UnitDamageTarget(caster, f, Damage(level), true, false, A_TYPE, D_TYPE, null)
 endif
 endloop
 endif

 call RemoveLocation(spellLoc)
 set spellLoc = null
 set caster = null
 endfunction
//===========================================================================
 private function Init takes nothing returns nothing
 local trigger InstantJusticeTrg = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(InstantJusticeTrg, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(InstantJusticeTrg, Condition( function Conditions ) )
 call TriggerAddAction( InstantJusticeTrg, function Actions )

 //setting globals
 set all = CreateGroup()
 set copy = CreateGroup()
 set b = Condition(function Pick)

 //preloading effects
 call Preload(AOE_EFFECT)
 call Preload(HEAL_EFFECT)
 call Preload(DAMAGE_EFFECT)

 //preloading the ability
 set bj_lastCreatedUnit = CreateUnit(Player(PLAYER_NEUTRAL_PASSIVE), DUMMY_ID, 0, 0, 0)
 call UnitAddAbility(bj_lastCreatedUnit, SPELL_ID)
 call KillUnit(bj_lastCreatedUnit)

 endfunction
endscope


Congratulations on your first vJASS spell! =D


Please comment and enjoy. Also post questions if you have any.

NOTE: As you may have already seen the spirit of JESP is right in the heart of vJASS. The reason for this is because the creator of JESP standard is also the creator of vJASS, this is, Vexorian. Truth is that JESP is now quite old and deprecated and so many people think that when you make a spell using vJASS the spell is automaticaly in JESP standard. Such thing is not truth however, although vJASS spells are easier to make JESP the thing is that to make a real spell following JESP the coder always has to give it a touch. I hope you now understand "the touch" I had to give. Adding some extra globals or functions in a SETUP section will only make your spell easier to configure, and the easier a spell is to change, the more people will actually use it. Just think of the newbs that will be grateful for making an easy SETUP section, I usually do and so far I don't regret it. I hope that by following this tutorial you learned how to give the "touch" so your spells can in fact become easier to understand and read by others. Also, JESP spells are often seen as mini-templates, so you really have nothign to lose.


PS: AOE stands for Area of Effect

##
 Sources

The vJASS manual by Vexorian:


- JassHelper 0.9.D.0


The NewGeneration World Editor (or as some people call it JNGP – Jass New


Generation Pack):


- Jass NewGen Pack v5b - Wc3campaigns


The tutorial from blade.dk where the original GroupCopy function is available:


 - Spell Making Course: Part 1: Making a simple stomp spell. - Wc3campaigns


Since I talk about xe, here is the link to the system's page:


 - xe0.5 - Wc3campaigns

##
 Credits

 - Moyack, for helping me find errors and for the tip about the magical footman world


 - Blade.dk, for the CopyGroup function


 - Vexorian, for telling me the story of vJASS


 - PitzerMike, for correcting some of the error of the tutorial


 - HINDYhat, for helping me improve the tutorial with tips and by finding mistakes

##
 Goodbye

Well, I really hope you enjoyed the tutorial and the main thing that you learned from it. This took me a lot of work to do and I hope it gets you to the stage of vJASS and that it shows you how vJASS is easy and simple. In my next tutorial I intend to explain structures and timers but as I said in the note at the beginning I don't know if I will make it due time.


Truth is that I hope this gets approved. I intend to make a match for my "Everything about Icons tutorial". So far this one only has 59 pages, but if I manage to reach the last session I am sure the set of tutorial will pass 100 or maybe 150 easy.


At the end you can and should download the map and the pdf of this file. I also attached the version of JNGP I was using, in case you need it.


Hoping it helps, Goodbye, Flame_Phoenix.

 Copyright 2008 Flame_Phoenix

#### Attachments

InstantJustice.w3x

 21.5 KB

 · Views: 832




 Last edited by a moderator: Apr 24, 2009

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Ok guys, here is the complete tutorial. I just have to add the PDF file now and a few more attachments (damn THW for not accepting 7z files xD) but I think the hard part is done and I would like to read some comments from mods now.

This is my second super tutorial, the fact that today I am still receiving reputation thx to my icon tutorial, greatly motivated me to make the next generation of super tutorials, but this time related to code. I can only hope that this tutorial gets as much successful as the other one. I intend to release lesson 2 and 3, but so far I am having a few problems with lesson 2 because I am trying to learn a new method of using timers that I want to include.

Hoping it helps all kinds of people, Flame_Phoenix.

#### HINDYhat

#### HINDYhat

 Level 20

JoinedApr 22, 2007Messages1,960

Resources: 4

#### Resources
Maps 3Spells 1

Your terminology is wrong, your indenting is [still] horrible, and you overuse caps. This tutorial is also unnecessarily huge. I really can't see why people don't just use the vJass manual that Vexorian himself wrote, instead of reading a monster tutorial that teaches much less.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 Your terminology is wrong


Click to expand...

You mean [JASS/AI] thing ?? Well, there is not [vJASS/JASS] thing so I though this would be the best that describes it.

 your indenting is [still] horrible


Click to expand...

Actually no it isn't. Some examples may be have 8 spaces instead of 4 (I think that is THW indentation thing, since in wc3c the identation is Ok) and I really don't see anything wrong with it. Seriously I bet you didn't read 25% of the tutorial.

 and you overuse caps


Click to expand...

I always make my constants caps lock and I type important things in caps. This is seriously not an argument for not approving, it is a personal opinion.

 I really can't see why people don't just use the vJass manual that Vexorian himself wrote, instead of reading a monster tutorial that teaches much less.


Click to expand...

It turns out I was right you didn't read it at all. I teach some stuff of the manual my way and I then apply it to the spell example. If people already know what the basic concepts are then fine, they can just jump, but I teach a lot more stuff. I did this because I saw that most my students, although knowing this BASIC concepts, they didn't know how to use them. So I decided to make this tutorial to help them. I teach a lot more stuff and you would see that if you wanted to.

I seriously don't get your point, when you [obviously] didn't even tried to read it. If the problem is I explaining the basic concepts of vJASS fine, I just Hide them, no one dies, but I insist I explain the concepts I use on my tutorials. Wanna know why it is huge? because I do it for newbs and I only advance after making sure people understood every little step. If you don't believe me, see my icon tutorial.

I was expecting a different [better] critic from you...

Anyway, if any one else has a critic or a suggestion be at will.

#### HINDYhat

#### HINDYhat

 Level 20

JoinedApr 22, 2007Messages1,960

Resources: 4

#### Resources
Maps 3Spells 1

You're right, I didn't read most of it because it's painstakingly long, and I doubt especially newbies would go through this much.

 You mean [JASS/AI] thing ?? Well, there is not [vJASS/JASS] thing so I though this would be the best that describes it.


Click to expand...

No, I mean:

 As any JASSER knows, there are 2 levels defined as programming languages that blizzard made


Click to expand...

And you use 'magic quotes' in your scripts which makes the THW syntax highlighter go all wrong. For example, “this is a string!” is not a string. "this is a string!" is a string.

 Actually no it isn't.


Click to expand...

Actually, yes it is. It doesn't matter if I read 25% of the tutorial. 25% of the indenting was horrible, and that's way too much of an ugly fraction of the tutorial for it to be approved IMO.

 (about caps) This is seriously not an argument for not approving, it is a personal opinion.


Click to expand...

No it's not. One of the most important parts of making a tutorial is making it look nice and be readable. You overuse caps everywhere for emphasis instead of using italics which is what you should be using. It makes the text ugly. And I agree with the use of caps for constants. That's what I do anyway.


And so I'll read through another part of your tutorial if you want me to critique more accurately. You also can't keep me away from showing you what's wrong. Besides, all I'm trying to do is help... FINALLY DONE READING.

 However you are free to visit the manual if you are curious or if you really want to learn.


Click to expand...

Well said.


Firstly, about the private keyword:

 When you press save, the code is compiled, and what the computer will see is this:


Click to expand...

WRONG. Do not say random stuff. The compiled script is different from what you wrote. It involves a random integer and two underscores. And why didn't you mention anything about the public keyword? I consider it just about as important as the private one.


You forgot to mention that the requires keyword can be replaced with uses or another one I can't remember which is noted somewhere in the vJass manual.

 JASS:

private function Init takes nothing returns nothing
 call RawOne()
 call rawTwo()
endfunction

Click to expand...

rawTwo isn't even a function. The function is RawTwo.


For your spell, you should've used the Channel spell. It's most definitely the best dummy spell for any triggered spell and is a very important part to spell making.

 JASS:

set InstantJusticeTrg = null

Click to expand...

This line is unneeded.

 Thing is some natives can and will crash the game if settled automatically. CreateGroup is one of those evil natives.


Click to expand...

AFAIK that never happened to me.

 (about 0.405 HP) This is quite important and I am sure it will aid you in the rest of your coding life in wc3.


Click to expand...

Not really. In fact, when the unit's HP hits <0.405, it is instantly set to 0, so this observation isn't quite important at all.


Also, I don't know why you used two groups to count and stuff. You could've just used one group and ForGroup. I know you're going to say that this is your 'style', but the thing is there is a part of coding called efficiency which is more important than style.

 Hoping it helps and hoping it gets approved in the hell fires of wc3c


Click to expand...

You should've changed that.


And I hope you didn't really copyright this...

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 You're right, I didn't read most of it because it's painstakingly long, and I doubt especially newbies would go through this much.


Click to expand...

I also had that fear at start, but for some reason experience has been telling me that newbs read these kind of tutorials. My icon tutorial is an example, but about coding, I already have people inviting me to msn asking me to give them support on the tutorial and saying that they appreciate it and it is not even approved. This factor strongly motivated me to make this tutorial as detailed as possible, thus being so big. I know it is a pain in the ass for any mod to review, but i made this with the newbie users in mind (that some how appreciate this). If you think this is to big though, you should check Daelin tutorials, which are also very big. As I said, I can place some "hidden" tags if you find it necessary.

 No, I mean:


Click to expand...

Elaborate please.

 And you use 'magic quotes' in your scripts which makes the THW syntax highlighter go all wrong. For example, “this is a string!” is not a string. "this is a string!" is a string.


Click to expand...

ARrghh God curse Office 2007... Somehow it uses those tags, I assume it wouldn't make a difference but now I see I have to change them ... Thx for pointing that out.

 Actually, yes it is. It doesn't matter if I read 25% of the tutorial. 25% of the indenting was horrible, and that's way too much of an ugly fraction of the tutorial for it to be approved IMO.


Click to expand...

Tell me what you want me to fix, (what you think is wrong) and I will do it as fast as a lightning. I see I have to paste this codes in JNGP and correct the indentation there ... somehow Word Office 2007 is not as good as I though at first -.-

 No it's not. One of the most important parts of making a tutorial is making it look nice and be readable. You overuse caps everywhere for emphasis instead of using italics which is what you should be using. It makes the text ugly. And I agree with the use of caps for constants.


Click to expand...

So, you say that using caps i ugly and that I should use italics. Isn't this a matter of personal opinion? I kinda think the opposite, which is why I use caps instead of italics ...

 You also can't keep me away from showing you what's wrong.


Click to expand...

Wermmm, that is the exact reason why I want some one to review this ...I want to know what may be wrong so I can fix it.

 WRONG. Do not say random stuff. The compiled script is different from what you wrote. It involves a random integer and two underscores.


Click to expand...

Wermm, actually I don't get what's wrong, since I know it is a random integer (I explain it I think) and and use random integers in my code examples. Please specify, what the part of the text you think needs change.

 And why didn't you mention anything about the public keyword? I consider it just about as important as the private one.


Click to expand...

I also explain that in the tutorial I think. Public is more for systems and use with libraries, Private is more for spells, this is a general rule that is applied in MOST cases (there is always an exception and I am sure you will shoot one at me, so to avoid that, this is a generalization that has few exceptions). Besides I don't use Public keyword on my spell example so I don't need to explain it. Please note, I only explain what I use in the SIMPLE spell example (I always say it is simple, so people don't say "hey that is really simple"... kinda the objective is to be simple). Which is why i say "I insist that I explain everything I use".

 You forgot to mention that the requires keyword can be replaced with uses or another one I can't remember which is noted somewhere in the vJass manual.


Click to expand...

Yes, I know that, but again, note that the reason why I mention libraries is because most my students often confuse them with "scopes" (I think I even say this on the tutorial). My intention is to clear their mind, saying where and when to use each one. To clear the mind of my students, they must know what a library is, so I decided to give it a very small brief introduction. The point is not to teach how to use it, but to let them know the differences between a scope and a library and when to use them. I see no need to add such information, since the objective is not to cover libraries in depth, but if you really find it necessary, I will add it (Please note I am expecting an answer to this quote such as "I find it necessary" or "Ok, if that is your point than I agree it doesn't need").

 rawTwo isn't even a function. The function is RawTwo.


Click to expand...

Arghh, a typo. Thx for finding that out.

 For your spell, you should've used the Channel spell. It's most definitely the best dummy spell for any triggered spell and is a very important part to spell making.


Click to expand...

Somehow no one ever explained me that. If I make a hero with many "Channel" based spells, they won't conflict? If I remember I think I use dispel as base spell (don't remember exactly) but what are the real advantages of using channel?

(PS: Since you talk about that I would like to know, because if I add "channel" to the tutorial I also have to explain why it is better than disppell or Blizzard or something else).

 This line is unneeded.


Click to expand...

Agreed.

 AFAIK that never happened to me.


Click to expand...

But it can happen. I have people who confirm this theory back in wc3c, if you need more explanation I can try to get it. Since Anitarf, Griffen and Pyrogasm advice me to care about this, I assume they are correct (they are all great and wise coders).

 Not really. In fact, when the unit's HP hits <0.405, it is instantly set to 0, so this observation isn't quite important at all.


Click to expand...

It is, because in some comparisons if you use "0" and not "0.405" the return will not be true and so the spell won't work due a stupid cause. I had this problem myself, but since I started using 0.405 I never had it again. This is proved in lost posts in wc3c, but I am afraid I can't show them because I lost the links =(

 Also, I don't know why you used two groups to count and stuff. You could've just used one group and ForGroup. I know you're going to say that this is your 'style', but the thing is there is a part of coding called efficiency which is more important than style.


Click to expand...

it has nothing to do with style. 1st I never use ForGroup. 2nd, as I say in the Creators note, the main objective of this tutorial is not to make a speed freak spell, but a spell people can easily [very easily] understand and a spell in which people can use the knowledge of the previous sections. Besides, i can't say the spell is highly inefficient, because the spell has a "medium" level of efficiency, it is not a speed freak, and it is not a slow freak. It is in the middle. I also decided to use the "FirstOfGroup" method and not "ForGroup" because for most JASS people that are new to JASS this approach is easier. Truth is that I considered "ForGroup", but in the end I decided not to use it.

 You should've changed that.


Click to expand...

Agreed.

 And I hope you didn't really copyright this...


Click to expand...

No, I just added to make it look nice and important xD

Ok, I will try to fix all typos, the indentation problems, and the null thing on the code. Expect an update soon. Please answer to the quotes that requires answers xD

Oh, and btw, since you had quite an effort seeing this: THX for your review. I mean Thanks, I appreciate it.

#### HINDYhat

#### HINDYhat

 Level 20

JoinedApr 22, 2007Messages1,960

Resources: 4

#### Resources
Maps 3Spells 1

When I was a newb before, I tried reading through huge tutorials. Oh yes I remember reading through Vexorian's monster of a Jass introduction tutorial, but it was so large that I never managed to finish it. Same with Blade.dk's stomp spell tutorial.


Well, you say programming language when in fact Jass is just a scripting language. Important terminology right there. You don't want people going around asking how to program a spell.


Most of the indenting issues were in fact in the introduction post before the spell. I'd advise you to read through that again and fix all of the indentation issues so that statements are properly aligned.


I might've skipped over the part about the public keyword because I don't remember reading it at all. I use the public keyword all the time for spells since I don't like scope initialiazers. Instead, I use public function InitTrig takes nothing returns nothing.


About the requires keyword, well I like using one of them while other people might prefer another. I know it really isn't important at all, but it might confuse people if they see different keywords in different scripts, since your tutorial isn't the only of its kind.


Can't believe you don't know about the Channel spell. Channel is the ultimate dummy ability. You can change the order id so if you have two spells based on Channel on the same caster, they won't conflict. Within the Channel spell definition, you can make it a Unit target spell, a Point target, an AoE or a Unit-Point target spell. You can also make it a unique cast... anyway you can do pretty much anything with it. You'd better check it out yourself. It's at default a neutral hostile hero spell.


You're right, but it's important to push on efficiency even when dealing with a basic spell so that coders don't develop techniques that are prone to leaks and inefficiency.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 Oh yes I remember reading through Vexorian's monster of a Jass introduction tutorial, but it was so large that I never managed to finish it. Same with Blade.dk's stomp spell tutorial.


Click to expand...

I also read those tutorials, I never finished vexorian's tutorial because I was missing "a mission" or an objective. However I enjoyed and learned a lot reading Blade.dk tutorial, and i must say I already read that tutorial several times.

My tutorial is huge, but I try to think "If i was a newb, what would I like to know?" Thus the reason why I explain so many things. In the end I make a spell "the objective" so people don't think "now that I learned all this stuff, how do I apply it?".

 Well, you say programming language when in fact Jass is just a scripting language. Important terminology right there. You don't want people going around asking how to program a spell.


Click to expand...

Yes, I am also aware of this fact. However I never understood the difference between a scripting laguage and a programming language ... Java is a scripting language too and it is widely used, although C, I am not sure if it is a programming language.

Anyway, I assume GUI is a scripting language as well. Thx for pointing this out, I will fix it soon.

 Most of the indenting issues were in fact in the introduction post before the spell. I'd advise you to read through that again and fix all of the indentation issues so that statements are properly aligned.


Click to expand...

Agreed.

 About the requires keyword, well I like using one of them while other people might prefer another. I know it really isn't important at all, but it might confuse people if they see different keywords in different scripts, since your tutorial isn't the only of its kind.


Click to expand...

Now that I think of it, you make a good point. I shall add the extra information.

 Can't believe you don't know about the Channel spell. Channel is the ultimate dummy ability. You can change the order id so if you have two spells based on Channel on the same caster, they won't conflict. Within the Channel spell definition, you can make it a Unit target spell, a Point target, an AoE or a Unit-Point target spell. You can also make it a unique cast... anyway you can do pretty much anything with it. You'd better check it out yourself. It's at default a neutral hostile hero spell.


Click to expand...

You get me wrong, I know of the channel spell, however I never use it because I see no need. I can do pretty much anything with other spells and change orderID as well (although I don't like to ..)

However, since I can not deny the fact that this ability is in deed important for starters, I will also talk about it. However, is there any hardcore advantage of using channel, instead of using dispell ?? Like, using channel give people more work so far as I can see (while dispell is a AOE ability, in channel you have to make it AOE; per example), that is why I never use it.

 You're right, but it's important to push on efficiency even when dealing with a basic spell so that coders don't develop techniques that are prone to leaks and inefficiency.


Click to expand...

Truth, but the speed that we would get from using ForGroup wouldn't be noticeable in game, a few nanoseconds wouldn't make the difference. So far i prefer to keep it simple. Besides, my spell does not leak and is not deficient in efficiency. As I said, it is in the middle of acceptable things xD

When I make session 2, I will have efficiency in a higher place. The reason why I didn't made it yet is because I am trying to learn a new way of using timers, or better saying, only 1 timer.

Thx again, expect an update tomorrow, since now is night in my country, I will see this with my head fresh in the morning.

EDIT EDIT EDIT EDIT

Updates:

1 - Corrected all indentation problems (I think)

2 - Fixed the weird "" with regular "" in the codes so now they look better

3 - Fixed "programming" language with "scripting" language

4 - Removed "set trigger = null" thing from all codes and from the map

5 - Now I mention keywords "needs" and "uses" as well as "requires" in the library brief introduction

6 - Fixed the "in the hell fires of wc3c" sentence by simply removing it (as suggested) xD

Ok, these are the changes I made so far. If you think that anything else needs to be improved, just tell me and I will considerate it. Note that I am still awaiting some answers from you though xD

Anyway, thx for all help so far (again). When this gets approved here, I am thinking of adding your name to the credits list. Hope you don't mind, since you are the only THW moderator that actually cares about this work of mine.

#### scorpion182

#### scorpion182

 Level 16

JoinedJun 9, 2008Messages734

Resources: 23

#### Resources
Models 1Maps 5Spells and Systems 17

@Flame_Phoenix

the tutorial is good, and it's clear for me  and this is called "learning by doing" =)

#### HINDYhat

#### HINDYhat

 Level 20

JoinedApr 22, 2007Messages1,960

Resources: 4

#### Resources
Maps 3Spells 1

FP, I'm actually not a moderator

And what answers do you need from me? Ask and ye shall receive.

#### chaoslord301

#### chaoslord301

 Level 4

JoinedSep 18, 2007Messages104

I read this over at wc3c and thought it was pretty good, yes I am a noob, and I read the whole thing in one sitting. Only complaint was some gramatical mistakes, but I could still understand what you said pretty easy, and I know english isn't your first language.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 @Flame_Phoenix

the tutorial is good, and it's clear for me  and this is called "learning by doing" =)


Click to expand...

=P That is the point, learning and then doing or learning by doing. Important thing is that you learn xD

 FP, I'm actually not a moderator

Click to expand...

You will always be that moderator teaching me mathematics and polar projection. Things some one doesn't forget =D

 And what answers do you need from me? Ask and ye shall receive.


Click to expand...

Ok, I would like to know your opinion about this matter:

 You get me wrong, I know of the channel spell, however I never use it because I see no need. I can do pretty much anything with other spells and change orderID as well (although I don't like to ..)

However, since I can not deny the fact that this ability is in deed important for starters, I will also talk about it. However, is there any hardcore advantage of using channel, instead of using dispell ?? Like, using channel give people more work so far as I can see (while dispell is a AOE ability, in channel you have to make it AOE; per example), that is why I never use it.


Click to expand...

I still didn't find this error:

 WRONG. Do not say random stuff. The compiled script is different from what you wrote. It involves a random integer and two underscores.


Click to expand...

And, my curiosity would still like to know this difference:

 Yes, I am also aware of this fact. However I never understood the difference between a scripting language and a programming language ... Java is a scripting language too and it is widely used, although C, I am not sure if it is a programming language.

Anyway, I assume GUI is a scripting language as well. Thx for pointing this out, I will fix it soon.


Click to expand...

Well, I think this is all for now. This are things I would like to know because they may lead to improvements on the tutorial, but first I need more information about it, and since you were the first person talking about it xD

 I read this over at wc3c and thought it was pretty good, yes I am a noob, and I read the whole thing in one sitting. Only complaint was some gramatical mistakes, but I could still understand what you said pretty easy, and I know english isn't your first language.


Click to expand...

Yeeee! =D

Well, yes English is not my natural language, I am amazed you already knew that xD

Yet, if you find that grammar is a problem, just tell me where to fix, and I will do it.

Thx for reading and commenting this guys!

#### Shanghai

#### Shanghai

 Level 19

JoinedFeb 15, 2008Messages2,174

Resources: 6

#### Resources
Maps 6

the tutorial is to long.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 the tutorial is to long.


Click to expand...

ORLY!?

If you don't wanna learn, then don't read it. Just don't make spam posts with 15 characters.

The tutorial is big because it has many code examples. Without them it would be significantly shorter. I won't change the way I do things.

#### HINDYhat

#### HINDYhat

 Level 20

JoinedApr 22, 2007Messages1,960

Resources: 4

#### Resources
Maps 3Spells 1

 However, is there any hardcore advantage of using channel


Click to expand...

You can do ANYTHING you want with Channel. That means making it any target, making it any order ID, making it work in special ways... it's just great to base all of your spells off it.

 The compiled script is different from what you wrote. It involves a random integer and two underscores.


Click to expand...

You'll find that compiled private stuff would look something like this:

 JASS:

library SomeLib
private function Actions takes nothing returns nothing
// bla bla bla

// compiles to
function SomeLib9__Actions takes nothing returns nothing
// bla bla bla

Anyway, afaik. You should just not mention anything about the actual compiling and just say that it's impossible to predict the function name. For the public keyword, the function just becomes:

 JASS:

library SomeLib
public function Actions takes nothing returns nothing
// bla bla bla

//compiles to
function SomeLib_Actions takes nothing returns nothing


With the exception of the InitTrig function which would compile to:

 JASS:

scope SomeScope
public function InitTrig takes nothing returns nothing
// bla bla bla

// compiles to
function InitTrig_SomeScope takes nothing returns nothing

That's quite useful for spells and such back in the day when scope initializers didn't exist.

 However I never understood the difference between a scripting language and a programming language


Click to expand...


 Wikipedia said:



 A scripting language, script language or extension language, is a programming language that controls a software application.


Click to expand...


 Wikipedia said:



 Programming languages can be used to create programs that specify the behavior of a machine, to express algorithms precisely, or as a mode of human communication.


Click to expand...

So a scripting language controls a program, which is written in a programming language.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 You can do ANYTHING you want with Channel. That means making it any target, making it any order ID, making it work in special ways... it's just great to base all of your spells off it.


Click to expand...

Right ... I see no hardcore advantage, but since it is in fact important to learning I will talk about it!

About the rest, well, I won't mention public keyword, nowadays using public Inits is not acceptable according to mods in wc3c, so because I want this to teach people acceptable things, I won't mention it.

I may speak some stuff about the public keyword, but I won't go in depth.

I will also correct the codes that are created "after the compilation".

Thx for all help so far, I will soon update the tutorial one last time.

EDIT EDIT EDIT

Updated:

1 - Fixed the "underscore thing" in the private functions

2 - Added a few lines about keyword "public"

3 - Added information about Channel spell and added Channel spell to the map

4 - Added HINDYhat to the credits section

I hope this is ok for approval now. I think it is very complete and that most typos and mistakes were fixed. I also hope this is the last update to this tutorial, since I would really like to see it approved xD

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

48 hours passed. Bumb, so any news on this ? is this Ok now? can I ask some one to move it?

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Really nice tutorial Flame_Phoenix


Before I read this I didn't know squat about libraries, scopes or even vJASS, just how to make simple AoE spells with leak cleaning.


This really deserves an approval, more than the one I made (yay I have an approved tutorial ).


Maybe make the headers colored? Moderators tend to like those


Well this will get approved with or without headers, when someone have the time.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 Before I read this I didn't know squat about libraries, scopes or even vJASS, just how to make simple AoE spells with leak cleaning.

This really deserves an approval, more than the one I made (yay I have an approved tutorial


Click to expand...

Thx, this really means a lot to me. However mods are just too lazy and they just won't read it =S

Besides, there is no code mods for tutorials so ... this can't get approved =S

Still thx for your comment =D

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Flame_Phoenix said:

 Thx, this really means a lot to me. However mods are just too lazy and they just won't read it =S


Besides, there is no code mods for tutorials so ... this can't get approved =S


Still thx for your comment =D


Click to expand...


Don't give up hope. Mine is a lot more simple but it met all the requirements, and it got approved around 2-3 months after the posted date.


I'm sure yours will get approved as soon as a code moderator notice it

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 I'm sure yours will get approved as soon as a code moderator notice it


Click to expand...

I told them it is here and they know it. This tutorial is here since last year ... if your tutorial is small and took 3 months to get approved, how ling will this take? 3 years ?!

Anyway, thx for the reply =P

I am thinking of making session 2 ... but only when this one gets approved =P

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Flame_Phoenix said:

 I told them it is here and they know it. This tutorial is here since last year ... if your tutorial is small and took 3 months to get approved, how ling will this take? 3 years ?!


Anyway, thx for the reply =P


I am thinking of making session 2 ... but only when this one gets approved =P


Click to expand...


That makes me sad  I really learned something from this tutorial. Would be nice if I learned more.


But of course, I would feel the same way. Wasting precious time on making a tutorial that doesn't get approved sucks.


I wish you luck in getting it approved though


Maybe I can help in some way?

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 Maybe I can help in some way?


Click to expand...

You could help me by asking mods to do their job and moderate this =P

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Flame_Phoenix said:

 You could help me by asking mods to do their job and moderate this =P


Click to expand...

I sent Pyritie a message and I hope he notes it (would be a shame if he just scraped it into the recycle bin )

If you don't get any response by him I'll send another, until he notices it.

Really awesome tutorial. Just had to say that again

Note: Pyritie's name might have been mispelled.

EDIT: Apperently Pyritie doesn't do the JASS/AI tutorial approvals, so he passed it on to someone who does.

I don't know the name of this moderator so I'm not sure if he's gonna read it or not.


 Last edited: Apr 11, 2009

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 I don't know the name of this moderator so I'm not sure if he's gonna read it or not.


Click to expand...

Thx for the effort. rep++

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

god damn this tutorial's massive

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

hEY Py ! sup? =D

 god damn this tutorial's massive


Click to expand...

You bet it is. I made it for the nobbiest newb possible.

My intention is to teach people, this is supposed to be a match for my "all about icons tutorial" =D

However mods are too lazy to mod this, even though this was already reviewed by some people ...

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

I'm gonna move you guys' posts to a new thread in the scripting forum since it's not really about the tutorial any more.

Also there isn't really any reason to reject this, and it's just sitting here. So I might as well approve it.

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Pyritie said:

 I'm gonna move you guys' posts to a new thread in the scripting forum since it's not really about the tutorial any more.


Click to expand...

Oh yeah, sure. No way we can do it ourselves?

Pyritie said:

 Also there isn't really any reason to reject this, and it's just sitting here. So I might as well approve it.


Click to expand...

How splendid! Now he might begin working on session 2

I really wanna learn timers, and it would be cool to know what those 'tables' he talked about are.

This tutorial is excellent, I hope we at the hive recieve yet another wonderful tutorial in a few months

+Rep to Pyritie for approving this (not sure if it's needed though  it's your job)

And I'll try to +Rep Flame_Phoenix for the awesome help he's given me, would've done it long time ago but I gotta "throw some rep around" before I can

(I really shouldn't be the first guy answering, too bad I saw this before Flame)

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

https://www.hiveworkshop.com/forums/f269/posts-moved-fps-tutorial-thingy-125222/

There ya go.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 I'm gonna move you guys' posts to a new thread in the scripting forum since it's not really about the tutorial any more.


Click to expand...

Seems fair.

 Also there isn't really any reason to reject this, and it's just sitting here. So I might as well approve it.


Click to expand...

Although I admit I would like to see my work recognized by someone in the area of jass, I am still very happy by you approving it. Thx, for your action.

 How splendid! Now he might begin working on session 2


Click to expand...

It is going to take time, I am still trying to learn a few tricks with timers ... like stacks and that stuff xD

 +Rep to Pyritie for approving this (not sure if it's needed though it's your job)


Click to expand...

I my opinion he deserves it. Notice he is a model moderator, he is putting himself at great risk by approving this resource since he is not 100% qualified for it. Also notice that THW has many other mods, some of them more important that Pyrite, and none of them decided to even read this tutorial. For these reasons, he deserves a reward.

 (I really shouldn't be the first guy answering, too bad I saw this before Flame)


Click to expand...

When you wake up at 6am and get home at 9pm, it is hard to keep track of posts =P

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Flame_Phoenix said:

 I my opinion he deserves it. Notice he is a model moderator, he is putting himself at great risk by approving this resource since he is not 100% qualified for it. Also notice that THW has many other mods, some of them more important that Pyrite, and none of them decided to even read this tutorial. For these reasons, he deserves a reward.


Click to expand...


When was the last time you looked at my staff title?

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 When was the last time you looked at my staff title?

Click to expand...

Lol Pyrite, so you know vJass now? I though your main attribute was models =P

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Replace all the syntax stuff with a link to the vJass manual.

--

How would doing something you are completely unqualified to do be a good thing deserving respect? Now, Py didn't do this of course, but had he I should think it would merit the opposite.

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

PurplePoot said:

 How would doing something you are completely unqualified to do be a good thing deserving respect? Now, Py didn't do this of course, but had he I should think it would merit the opposite.


Click to expand...

Not that I have anything to do with it, but I'd like that explained since I didn't get a single phrase of what you said in the second sentence (no offence or so, I think my vocabulary is too limited)

= =

Hey, I found this in your tutorial:

 When writing your code, you write this:

 JASS:

scope MyFirstSpell
private function Conditions takes nothing returns Boolean
 //return something
endfunction

private function Actions takes nothing returns nothing
//stuff and code here
endfunction
endscope

scope MySecondSpell
private function Conditions takes nothing returns Boolean
//return something
endfunction

private function Actions takes nothing returns nothing
//bla bla bla xD
endfunction
endscope

When you press save, the code is compiled, and what the computer will see is this:

 JASS:

function MyFirstSpell23__Conditions takes nothing returns boolean
//return something
endfunction

function MyFirstSpell21__Actions takes nothing returns nothing
//stuff and code here
endfunction

function MySecondSpell345__Conditions takes nothing returns boolean
//return something
endfunction

function MySecondSpell56__Actions takes nothing returns nothing
//bla bla bla xD
endfunction

So the functions actually have different names and everything will compile and run nice and easy.


Click to expand...

I'm pretty sure that in the version we use (the one you linked us in the begining) the functions gets renamed into the following:

 JASS:

function MyFirstSpell__Conditions takes nothing returns boolean
 //return something
endfunction

function MyFirstSpell__Actions takes nothing returns nothing
 //stuff and code here
endfunction

function MySecondSpell__Conditions takes nothing returns boolean
 //return something
endfunction

function MySecondSpell__Actions takes nothing returns nothing
 //bla bla bla xD
endfunction

We could just generalize this into scopeName__privateFunctionName

(note the double _ between the two)

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Flame_Phoenix said:

 Lol Pyrite, so you know vJass now? I though your main attribute was models =P


Click to expand...


I do all tutorials, but I usually get poot to look over the jass ones.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 Replace all the syntax stuff with a link to the vJass manual.


Click to expand...

No. I provide a link to the vJass manual you refer too. However I explain this things my way with examples of my experience and vJass manual doesn't do that. It is this fact of offering something that vJass manual doesn't offer that made me write all that stuff.

As people usually say: "Don't like it, don't eat it".

I don't force people to read my explanation. The tutorial is also divided into sections, so this means that they can just skip it.

I offer something to people, you can choose to either read it or not, but I am not removing it.

This thing was approved yesterday and I already started receiving rep++ from newbs to thank me for the tutorial. I am not going to harm others (they like my explanation) because someone doesn't like it.

@Cheezeman:

I don't get your point, what is wrong?

 I do all tutorials, but I usually get poot to look over the jass ones.


Click to expand...

Still, thanks for doing that.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Just doing my job.

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Flame_Phoenix said:

 @Cheezeman:


I don't get your point, what is wrong?


Click to expand...


In the tutorial you state that all functions inside a scope gets renamed into "scopeName + randomDigit + __ + functionName"


However when I press save the functions gets renamed into "scopeName + ___ + functionName"


I just thought you might change that (perhaps it's just something new from newer versions of JassHelper?)


(Yeah it was 3x "_", not 2x like I said  sorry about that)

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 However when I press save the functions gets renamed into "scopeName + ___ + functionName"

I just thought you might change that (perhaps it's just something new from newer versions of JassHelper?)


Click to expand...

No ... it depends if the function is private or public. Only public functions obey to your methodology. Private functions get the random digit.

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Flame_Phoenix said:

 No ... it depends if the function is private or public. Only public functions obey to your methodology. Private functions get the random digit.


Click to expand...

No dude.

Here's an example from a code I just made.

This is what it looks like in normal form (vJass form)

 JASS:

scope TestFunction initializer Init

 private function Conditions takes nothing returns boolean
 return true
 endfunction

 private function Actions takes nothing returns nothing
 //No actions yet
 endfunction

//===========================================================================

 private function Init takes nothing returns nothin
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition( t, Condition( function Conditions ) )
 call TriggerAddAction( t, function Actions )
 set t = null
 endfunction

endscope

Now, by simply adding "endfunction" in the end of the script, I will be able to see the entire scope.

Here it is:

 JASS:

// Trigger: TestFunction
//===========================================================================
//TESH.scrollpos=0
//TESH.alwaysfold=0
// scope TestFunction begins

 function TestFunction___Conditions takes nothing returns boolean
 return true
 endfunction

 function TestFunction___Actions takes nothing returns nothing
 //No actions yet
 endfunction

//===========================================================================

 function TestFunction___Init takes nothing returns nothing
 local trigger t= CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t , EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(t , Condition(function TestFunction___Conditions))
 call TriggerAddAction(t , function TestFunction___Actions)
 set t = null
 endfunction

// scope TestFunction ends

endfunction //This is the line I added to see the map's code (or whatever it's called)

To me, this looks like private functions get the name "scopeName___privateFunctionName"

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

This is NOT what the compiler sees after pressing "save" button... do not mix things up dude, my tutorial talks about what the warcraft game will actually see as regular jass. Also, that wouldn't even compile ... you are causing errors ...

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Flame_Phoenix said:

 This is NOT what the compiler sees after pressing "save" button... do not mix things up dude, my tutorial talks about what the warcraft game will actually see as regular jass.


Click to expand...

Tell me then, how can I see what the warcraft game will see?


And how do you know, did you ask Vexorian about this?


Cuase this looks a lot like normal Jass to me, so I don't see why it would add random digits after the code.


Then again I'm not a programer and this isn't by topic, I might aswell be wrong.

Flame_Phoenix said:

 Also, that wouldn't even compile ... you are causing errors ...


Click to expand...

Well yes I know, that was my point. I even pointed it out to show you where I added the line that errors the whole thing.


I can remove it but it doesn't change that this is what the JassHelper Syntax Check tells me.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 Tell me then, how can I see what the warcraft game will see?


Click to expand...

I don't know.

 And how do you know, did you ask Vexorian about this?


Click to expand...

yes.

If you want to know more, you really should follow the link to the vJass manual I point at the end:

JassHelper 0.9.G.0

#### Darkness-4ever

#### Darkness-4ever

 Level 9

JoinedNov 4, 2007Messages932

FINALLY, I find a tutorial that actually helps me fully understand a concept, instead of 40 - 75% like I've had with the JASS tutorial before and left many parts that I had to figure out by trial and error and most importantly spellcheck, thanks for the tutorial, it looked really tedious to make. +Rep

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Thx =D

I am happy you like this tutorial, I intend to make a lesson 2, some people are giving me incentive to do it, but I lack time...

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Flame_Phoenix said:

 Thx =D


I am happy you like this tutorial, I intend to make a lesson 2, some people are giving me incentive to do it, but I lack time...


Click to expand...


Wow that sucks


Perhaps you can do it over some summer holiday?


That is, if you still have summer holidays

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 Wow that sucks

Perhaps you can do it over some summer holiday?

That is, if you still have summer holidays


Click to expand...

Lol ... well, I have a few ideas ... I would like to do something before summer holidays, but yes, it is more likely that I will release it there ...

Anyway, Timers ... do what do you think of ... Mmmmm a spell like Ranger arrow from Dota?

xD

 You must log in or register to reply here.


### Similar threads

Locked

[Spell]
 vJass beginning


kawas11

Dec 25, 2014

World Editor Help Zone

Replies6Views2K

Dec 25, 2014
IcemanBo

Locked

[vJASS]
 Private isn't working as it should


Starquizer

Feb 15, 2012

Triggers & Scripts

Replies13Views2K

Feb 16, 2012
Bribe

 [Lua] Switching from vJass to Lua (structs, methods, modules and more)


Bribe

May 6, 2022

Scripting Tutorials

Replies4Views5K

Aug 10, 2022
Bribe

Locked

[vJASS]
 What does Scope initializer do? no need for initializer if scope is for non-MUI spells?


qemorak

Dec 5, 2024

Triggers & Scripts

Replies7Views1K

Dec 7, 2024
qemorak

 UI: Save&Load Frames


Tasyen

Oct 11, 2020

Scripting Tutorials

Replies10Views5K

May 12, 2025
Tasyen

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top