---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/learning-jass.104271/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Learning JASS
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/learning-jass.104271/>
#### Vestras

#### Vestras

 Level 9

JoinedDec 26, 2007Messages202

Resources: 13

#### Resources
Models 9Tools 1Maps 2Tutorials 1

Learning JASS


Contents:

Introduction

Intermediate JASS

Subchapter: Coding Conventions

Advanced JASS (Game Cache Usage, Spell Making.)

Goodbye


Requires:


 - Advanced GUI knowledge.


 - Advanced WE knowledge.


 - NewGen. (The easiest way, but you can also use JASSCraft.)


I hope you have learned something from this tutorial!

home - next


 Last edited: Nov 1, 2008

#### Vestras

#### Vestras

 Level 9

JoinedDec 26, 2007Messages202

Resources: 13

#### Resources
Models 9Tools 1Maps 2Tutorials 1

Introduction

First part - why JASS?


Why JASS and not GUI? Because JASS is:


 - easy to make MUI.


 - more efficient.


 - you get more control over your code.


 - easier to use. (yes, actually.)


And to you who doesn't know: you can't say that GUI is better and another thing than JASS. Why? Because GUI is JASS.

Second part - Goodies and badies?


Yes, there is goodies and badies - the goodies are stuff named "natives" and the badies are stuff named "BJs". BJs are functions (explained below) that are created by Blizzard (well, everything in JASS is.), which they used for GUI. The reason BJs is badies is:


 - they use natives to work.


 - they are slower than natives.

NOTICE - don't worry! I will explain what natives, BJs and functions is below this piece!


But, there is also good BJs! The BJ-variables. Now, in GUI, when you use "Get Last Created Unit", you actually use the function named "GetLastCreatedUnit()" in JASS. Now, all that GetLastCreatedUnit() does, is that it returns a BJ variable - this one:

 JASS:

bj_lastCreatedUnit

This is a variable, also created by Blizzard, but because it doesn't call another function, it isn't slow. Therefore: goodie.

NOTICE - there is also good BJs! When debuging, many people uses this function:

 JASS:

call BJDebugMsg("myMsg")

this displays a message to all players, and can be used to check which triggers run and which doesn't. There is of course also much more good BJs, try looking around in the function list in TESH!

Third part - JASS basics.


JASS is basicly made up by functions and variables. Now, in functions, there is "subcategories", named natives and BJs.

Functions.


A function would be the following:

 JASS:

call UnitApplyTimedLife(myUnit, myBuff, myDuration)

Now, this is a function - this would be adding a life timer in GUI. So, this function requires:


 - myUnit - a unit variable. (doesn't have to be a variable though.)


 - myBuff - a buff raw code. (in object editor, press ctrl + d to view raw codes)


 - myDuration - this is a real. If I placed "2" in this one, the unit (myUnit) would die after 2 seconds. (just an example.)

NOTICE - a function calling another function that you have made, can't be over the function you call!

 JASS:

function A takes nothing returns nothing
 call B ()
endfunction

function B takes nothing returns nothing
endfunction
// That will cause syntax errors.

Creating your own function.


Now, to create your own function for use, you'll have to do this:

 JASS:

function myFunction takes nothing returns nothing
endfunction

This will create a function for use. Now, this function can content other function calls, which will make this function do something.


Then there are something named arguments, like this:

 JASS:

function myFunction takes unit u returns nothing
endfunction


There, you could do something with unit u, and when calling the function, you would do this:

 JASS:

call myFunction(myUnit)

Just like when calling the native function above!


If you about to get this, I suggest you to read that all over.

NOTICE - you can have more arguments by adding commas, like this:

 JASS:

function myFunc takes unit u, integer i returns nothing
endfunction


Now, we do miss something, don't we? Returns..?

 JASS:

function myFunc takes unit u, integer i returns integer
 return 0
endfunction

The "returns integer" part - the integer is the returned variable type. This could have been boolean, unit, group, everything as well though. Now, what is returning? A lot of functions, native and BJ, returns stuff, example:

 JASS:

constant native function GetTriggerUnit takes nothing returns unit

This is a function you use for getting the triggering unit, right? Now, for making this function have an effect, it has to return something. Basicly, if a function doesn't take anything and doesn't return anything and doesn't set some variable, it pretty much does nothing. (Isn't always true though.) Now, set can actually set a variable to a return. Example:

 JASS:

set myUnit = GetTriggerUnit()


Now, actually, myUnit isn't set to the function GetTriggerUnit(), it is set to the variable the function returns. Get it? Good!

Calling a function.


Now, every single time you need a function, you'll have to call it - what does that mean? Basicly calling is the same as executing something, like if you wanted to take a step, your mind would "call" a function that made you walk.


When using a function, you'll always need to place call before the function name.

 JASS:

call UnitApplyTimedLife(...)

NOTICE - when calling functions, you cannot have

 JASS:

call UnitApplyTimedLife

You'll have to use (), where there needs to be the variables you want to call the function with inside the ()s! If the function doesn't take any arguments, you'll just go straigth ()!

Variables.

Locals.


In JASS variables, there are two types of variables - locals and globals. (globals requires vJASS, which is found in NewGen.) Locals are variable types which only are available for use in the function they were declared in.

 JASS:

function myFunc takes nothing returns nothing
 local integer int = 0
 local unit u = GetTriggerUnit()
endfunction

NOTICE - locals will cause syntax errors if another variable or function has the same name as the local!


If I used the variables int and u in other functions, I would have syntax errors.

Globals. (Requires vJASS!)


Globals, as the opposite of locals, can be accessed in every function in the map, and will, like globals, cause syntax errors if a variable or function is declared with the same name as the global.


Globals always needs to be in global "tags", like this:

 JASS:

globals
 integer i = 1
 unit u = null
 real r = 2.00
 // and so on...
endglobals


These variables will be able to be accessed all over the map.

Arrays.


Now, an arrayed variable is a variable "with more of the same type in it". If you got an arrayed variable, you can set more of the same variable, example:

 JASS:

local integer array i
set i[1] = 0
set i[2] = 1
set i[3] = 2
set i[4] = 3
// and so on...

NOTICE - arrayed variables can't be initialized!

Constants.


A constant variable can't be changed throughout the game, and will cause syntax errors if you try.

 JASS:

globals
 constant integer i = 0 // This will stay 0 throughout the whole game.
 unit u = null // This one can be changed, because it's non-constant
 constant real r = 2.00 // Again, can't be changed
endglobals


Now, locals can't be constants.


Why is this useful? This can be useful for ability raw codes and condition functions.

NOTICE - functions can be constant too!

previous - home - next

#### Vestras

#### Vestras

 Level 9

JoinedDec 26, 2007Messages202

Resources: 13

#### Resources
Models 9Tools 1Maps 2Tutorials 1

 Intermediate JASS


At this part, we are going to expand what we just learned. We are going to create a little more advanced functions, events, conditions and actions.

Creating Events.


When you convert your trigger into custom text, you will always have a function named InitTrig_TriggerName, and in there, the events, conditions and actions is.

 JASS:

function InitTrig_MyTrig takes nothing returns nothing
 set gg_trg_MyTrig = CreateTrigger()
// gg_trg_MyTrig is a trigger variable which is created for each trigger
 call TriggerRegisterAnyUnitEventBJ(gg_trg_MyTrig, EVENT_PLAYER_UNIT_SPELL_EFFECT)
// here we register when any unit starts the effect of an ability
// and so on...
endfunction


This will register when a unit starts the effect of an ability, and then, to add conditions and actions, we would do this:

 JASS:

function InitTrig_MyTrig takes nothing returns nothing
 set gg_trg_MyTrig = CreateTrigger()
// gg_trg_MyTrig is a trigger variable which is created for each trigger
 call TriggerRegisterAnyUnitEventBJ(gg_trg_MyTrig, EVENT_PLAYER_UNIT_SPELL_EFFECT)
// here we register when any unit starts the effect of an ability
 call TriggerAddCondition(gg_trg_MyTrig, Condition(function Conditions))
// adding conditions... Condition(...) is a function which causes syntax errors if not the function, here Conditions, doesn't return a boolean. Basicly, this function takes a trigger argument and a boolexpr (boolean expression) argument
 call TriggerAddAction(gg_trg_MyTrig, function Actions)
// adds actions, very basic
// and so on...
endfunction

NOTICE - remember, that you can add more events, conditions and actions by having more calls of the same function names!


To find out new events, just convert some GUI events into JASS. There are some rather tricky ones out there, so watch it!


Remember, that in normal JASS, without NewGen, you will always have to have a function like:

 JASS:

function InitTrig_MyTriggerName takes nothing returns nothing


Else it won't compile! And unless you are calling the functions in the trigger, you will always need an InitTrig to add events and stuff!

Function Names and Indentation


This is a little bit unnecessary you might think, but no - if you want people to read your code easilier, you will have to have good function names and indentation!

NOTICE - I might not be the best example at indentation, but just create your own indentation methods if you want.

Indentation


Now, what is indentation? Basicly, it's the spaces in each line of code;

 JASS:

function no_indentation takes nothing returns nothing
local unit u
local real x
local real y
local real angle
local unit t
local location l
endfunction


Pretty hard to read, right? This is a bit easier:

 JASS:

function with_indentation takes nothing returns nothing
 local unit u
 local real x
 local real y
 local real angle
 local unit t
 local location l

 call SetUnitAnimation(u,"spell")
 // and so on...


A bit easier to read, right?


Well, it can get too much sometimes though:

 JASS:

function toomuch_indentation takes nothing returns nothing
 local unit u
 local real x
 local real y
local real angle
 local unit t
 local location l
 // and so on...
endfunction


Got it? Great!

Function Names


Like indentation, these can be important too.


People probably won't read your code if your function names are like "MyFunctionName_lololo995830__75" (just an example of a pretty stupid name.), but more people will probably read it if it's like "Actions_Init" or "onSpellEffect" and stuff like that. This was very short, but very important if you would like to get better.

previous - home - next

#### Vestras

#### Vestras

 Level 9

JoinedDec 26, 2007Messages202

Resources: 13

#### Resources
Models 9Tools 1Maps 2Tutorials 1

 Subchapter: Coding Conventions

By Eleandor.


Coding conventions are "rules" made to make code easier to be read. When you're making a jass script, at some point other people will be reading it as well. Don't let them have a hard time reading a chaotic script. Or even if the script is for personal use: avoid being confused because of bad writing habits.


A script that doesn't follow such conventions will work just as well as a script that does. However, it's like writing a letter: you start with introducing yourself and end with saying "greetings, name". It's just the way it is and makes a letter easier to read.

1) Identifiers


Identifiers are the "names" you give to variables, functions and constants.

* Constants: constants are written in capital letters. Spaces are replaced by underscores. Example: UNIT_STATE_LIFE


* Variables: variables are written entirely in lower case. Spaces are either left out or replaced by underscores. Example: player1gold


* Functions: A functionname is written lowercase, and each word starts with a capital letter. Example: GroupAddUnit

​


Small variations on these naming conventions are possible. Some programmers start each function with a lower case character. For instance: groupAddUnit. This is a matter of habit. However, since the original jass functions start with capital letters, it's encouraged to do the same.


The name of a function or variable should be obvious. If you only have 1 unit variable, it's enough to say:

 JASS:

local unit u


If you have 2 units, naming them u1 and u2 is confusing. Name them wisely. For instance, say I have 2 units, an attacker and an attacked unit, I will call them:

 JASS:

local unit attacker = GetAttacker()
local unit target = GetTriggerUnit()

2) Indentation


Indentation makes "groups" of code more visible. For instance:

 JASS:

if booleanvar then
 Statements
endif

It is clear that the Statements are part of the "larger" if/then/else statement. For that reason, it's easier to see they are part of the statement by indenting them.


Following statements, among others, need to be indented:

* function / endfunction and method / endmethod


* if / else / elseif / endif


* loop / endloop


* struct / endstruct


* globals / endglobals


* More of such examples

​

3) Interlinear Spacing


Interlinear spacing is leaving some space between 2 lines of code. Such space is left between:

* 2 functions


* local variable declaration and the actual code


* ...​

4) Spacing


Sometimes, leaving additional spaces where they're not required can make the code easier to read.


Example:

 JASS:

set a=b+1
set a = b + 1
call DisplayTextToPlayer(p,0,0,"lol")
call DisplayTextToPlayer(p, 0, 0, "lol")

An example

 JASS:

globals
 constant boolean ELEANDOR_IS_COOL = true

 integer array score
 boolean score10isvictory = false
endglobals

function UpdateScore takes player p returns nothing
 // This function increases the score of a player.

 local integer playerid = GetPlayerId(p)

 set score[playerid] = score[playerid] + 1
 if score[playerid] >= 10 then
 if score10isvictory then
 call DisplayTextToPlayer(p, 0, 0, "Good job, you've won the game!").
 endif
 endif
endfunction

previous - home - next

#### Vestras

#### Vestras

 Level 9

JoinedDec 26, 2007Messages202

Resources: 13

#### Resources
Models 9Tools 1Maps 2Tutorials 1

Advanced JASS (Game Cache Usage, Spell Making.)

Game Cache Usage - Warning


First of all, I would like to warn you - Game Cache usage is an outdated method of storing data, newer methods would be vJASS, struct using, stuff like that. These stuff will not be included in this tutorial however, if you want to learn about that, I suggest you to read Moving From JASS To vJASS by Art.

Game Cache Usage - Starting


Before we can start using our game cache, which will be named gc for the rest of this chapter, we need 2 things;


 - A global game cache variable, created via the GUI Variable Editor. (CTRL + B)


 - A function for initializing and returning the game cache variable, like this:

 JASS:

function gc takes nothing returns gamecache
 if udg_cache==null then
 call FlushGameCache(InitGameCache("data.gc"))
 set udg_cache=InitGameCache("data.gc")
 endif
 return udg_cache
endfunction


The "data.gc" could be any name you would want.


This is the function we call every single time we use our gc.


Now, we will create our function for usage of the cache.


Basically, everything that you store in a cache you store with a name, so you will have to remember that name...

 JASS:

function gc takes nothing returns gamecache
 if udg_cache==null then
 call FlushGameCache(InitGameCache("data.gc"))
 set udg_cache=InitGameCache("data.gc")
 endif
 return udg_cache
endfunction

function Get takes nothing returns integer
 return GetStoredInteger(gc(),"gc","myInt")
endfunction

function Set takes integer i returns nothing
 call StoreInteger(gc(),"gc","myInt",i)
endfunction


It's really simple. Now, storing and getting units is a pain in the ass, and everything else than boolean, string, integer and real is nearly impossible, or very annoying to do all the time, to do without a system.


Therefore, we got Local Handle Vars or Attachable Vars.


Those are both game cache systems, which can be very useful when using game caches. I myself have never used LHV, only AV.

Local Handle Vars.

Attachable Vars.


I have attached maps where I use LHV and AV.


That was basically it, ready to go on for some game cache spell making? Great, let's go!

Spell Making - The Beginning


We are going to make a simple damage over time spell, where will need a game cache.


It is suggested that you don't use this spell in your map.


So, we know that we need our gc function... Then we need an inittrig, condition function and action function.

 JASS:

function gc takes nothing returns gamecache
 if udg_cache==null then
 call FlushGameCache(InitGameCache("data.gc"))
 set udg_cache=InitGameCache("data.gc")
 endif
 return udg_cache
endfunction

constant function DoT_Conditions takes nothing returns boolean
 return GetSpellAbilityId()==DoT_ID() // DoT_ID() is a configuration function that we haven't created yet
endfunction

function DoT_Actions takes nothing returns nothing
endfunction

function InitTrig_DoT takes nothing returns nothing
 local trigger t=CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t,EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(t,Condition(function DoT_Conditions))
 call TriggerAddAction(t,DoT_Actions)
endfunction


Now we need the configuring functions -

 JASS:

// !!!! CONFIGURATION !!!! \\

constant function DoT_ID takes nothing returns integer
 // The raw code of spell
 return 'A000'
endfunction

constant function DoT_Damage takes integer lvl returns real
 // The damage dealt depending on level
 if lvl==1 then
 return 4.
 elseif lvl==2 then
 return 7.
 elseif lvl==3 then
 return 10.
 endif
 return 0. // This one just... has to be there (well, probably not, I just do it)
endfunction

constant function DoT_Effect takes nothing returns string
 // The special effect of the spell
 return "Abilities\\Spells\\Other\\Stampede\\StampedeMissileDeath.mdl"
endfunction

constant function DoT_Interval takes nothing returns real
 // The timer interval
 return 0.75
endfunction

// !!!! SPELL CODE !!!! \\

function gc takes nothing returns gamecache
 if udg_cache==null then
 call FlushGameCache(InitGameCache("data.gc"))
 set udg_cache=InitGameCache("data.gc")
 endif
 return udg_cache
endfunction

constant function DoT_Conditions takes nothing returns boolean
 return GetSpellAbilityId()==DoT_ID()
endfunction

function DoT_Actions takes nothing returns nothing
endfunction

function InitTrig_DoT takes nothing returns nothing
 set gg_trg_DoT=CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(gg_trg_DoT,EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(gg_trg_DoT,Condition(function DoT_Conditions))
 call TriggerAddAction(gg_trg_DoT,function DoT_Actions)
endfunction


Got everything? Great!


Then we go on to creating the actual actions.

 JASS:

// WARNING!!!! THIS SPELL IS UNSAFE!!!!

// !!!! CONFIGURATION !!!! \\

constant function DoT_ID takes nothing returns integer
 // The raw code of spell
 return 'A000'
endfunction

constant function DoT_DummyID takes nothing returns integer // Notice this - we now need a dummy!
 // The raw code of the dummy unit
 return 'h000'
endfunction

constant function DoT_Damage takes integer lvl returns real
 // The damage dealt depending on level
 if lvl==1 then
 return 4.
 elseif lvl==2 then
 return 7.
 elseif lvl==3 then
 return 10.
 endif
 return 0. // This one just... has to be there (well, probably not, I just do it)
endfunction

constant function DoT_Effect takes nothing returns string
 // The special effect of the spell
 return "Abilities\\Spells\\Other\\Stampede\\StampedeMissileDeath.mdl"
endfunction

constant function DoT_Attach takes nothing returns string
 // The special effect attachment point
 return "chest"
endfunction

constant function DoT_Interval takes nothing returns real
 // The timer interval
 return 0.75
endfunction

// !!!! SPELL CODE !!!! \\

function gc takes nothing returns gamecache
 if udg_cache==null then
 call FlushGameCache(InitGameCache("data.gc"))
 set udg_cache=InitGameCache("data.gc")
 endif
 return udg_cache
endfunction

function H2I takes handle h returns integer
 return h
 return 0
endfunction

function I2H takes integer i returns handle
 return i
 return null
endfunction

function H2U takes handle h returns unit
 return h
 return null
endfunction

constant function DoT_Conditions takes nothing returns boolean
 return GetSpellAbilityId()==DoT_ID()
endfunction

function DoT_Timer takes nothing returns nothing
 local unit t=H2U(I2H(GetStoredInteger(gc(),"gc","t")))
 local integer id=GetStoredInteger(gc(),"gc","p")
 local integer lvl=GetStoredInteger(gc(),"gc","lvl")
 local real x=GetUnitX(t)
 local real y=GetUnitY(t)
 local unit d=CreateUnit(Player(id),DoT_DummyID(),x,y,0)

 call UnitDamageTarget(d,t,DoT_Damage(lvl),false,false,ATTACK_TYPE_NORMAL,DAMAGE_TYPE_NORMAL,null)
 call DestroyEffect(AddSpecialEffectTarget(DoT_Effect(),t,DoT_Attach()))
endfunction

function DoT_Actions takes nothing returns nothing
 local unit u=GetTriggerUnit()
 local unit t=GetSpellTargetUnit()
 local integer i=GetPlayerId(GetOwningPlayer(u))
 local integer lvl=GetUnitAbilityLevel(u,DoT_ID())
 local timer tmr=CreateTimer()
 call StoreInteger(gc(),"gc","p",i) // Storing the player id,
 // so that we can create the dummy and the caster will still get credit
 call StoreInteger(gc(),"gc","lvl",lvl) // Storing the unit ability level,
 // so that we can deal damage depending on level
 call StoreInteger(gc(),"gc","t",H2I(t)) // Storing the target unit via the unsafe
 // H2I to I2H functions. Do never use those in your map
 call TimerStart(tmr,DoT_Interval,true,function DoT_Timer)
 set tmr=null
 set u=null
 set t=null
endfunction

function InitTrig_DoT takes nothing returns nothing
 set gg_trg_DoT=CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(gg_trg_DoT,EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(gg_trg_DoT,Condition(function DoT_Conditions))
 call TriggerAddAction(gg_trg_DoT,function DoT_Actions)
endfunction


Now the spell actually does something... Now, we don't want the spell to last forever, do we? No, we don't, therefore we create the counting part.

 JASS:

// WARNING!!!! THIS SPELL IS UNSAFE!!!!

// !!!! CONFIGURATION !!!! \\

constant function DoT_ID takes nothing returns integer
 // The raw code of spell
 return 'A000'
endfunction

constant function DoT_DummyID takes nothing returns integer // Notice this - we now need a dummy!
 // The raw code of the dummy unit
 return 'h000'
endfunction

constant function DoT_Damage takes integer lvl returns real
 // The damage dealt depending on level
 if lvl==1 then
 return 4.
 elseif lvl==2 then
 return 7.
 elseif lvl==3 then
 return 10.
 endif
 return 0. // This one just... has to be there (well, probably not, I just do it)
endfunction

constant function DoT_Effect takes nothing returns string
 // The special effect of the spell
 return "Abilities\\Spells\\Other\\Stampede\\StampedeMissileDeath.mdl"
endfunction

constant function DoT_Attach takes nothing returns string
 // The special effect attachment point
 return "chest"
endfunction

constant function DoT_Interval takes nothing returns real
 // The timer interval
 return 0.40
endfunction

constant function DoT_Duration takes nothing returns real
 // The duration of the spell
 return 5.
endfunction

// !!!! SPELL CODE !!!! \\

function gc takes nothing returns gamecache
 if udg_cache==null then
 call FlushGameCache(InitGameCache("data.gc"))
 set udg_cache=InitGameCache("data.gc")
 endif
 return udg_cache
endfunction

function H2I takes handle h returns integer
 return h
 return 0
endfunction

function I2H takes integer i returns handle
 return i
 return null
endfunction

function H2U takes handle h returns unit
 return h
 return null
endfunction

constant function DoT_Conditions takes nothing returns boolean
 return GetSpellAbilityId()==DoT_ID()
endfunction

function DoT_Timer takes nothing returns nothing
 local unit t=H2U(I2H(GetStoredInteger(gc(),"gc","t")))
 local integer id=GetStoredInteger(gc(),"gc","p")
 local integer lvl=GetStoredInteger(gc(),"gc","lvl")
 local real ti=GetStoredReal(gc(),"gc","ti")
 local real x=GetUnitX(t)
 local real y=GetUnitY(t)
 local unit d=CreateUnit(Player(id),DoT_DummyID(),x,y,0)

 call UnitDamageTarget(d,t,DoT_Damage(lvl),false,false,ATTACK_TYPE_NORMAL,DAMAGE_TYPE_NORMAL,null)
 call DestroyEffect(AddSpecialEffectTarget(DoT_Effect(),t,DoT_Attach()))
 set ti=ti+DoT_Interval()
 call FlushStoredMission(gc(),"gc") // This clears our category "gc", so that we store the new values
 if ti>=DoT_Duration() then
 call PauseTimer(GetExpiredTimer())
 call DestroyTimer(GetExpiredTimer())
 return
 else
 call StoreInteger(gc(),"gc","t",H2I(t))
 call StoreInteger(gc(),"gc","p",id)
 call StoreInteger(gc(),"gc","lvl",lvl)
 call StoreReal(gc(),"gc","ti",ti)
 endif
endfunction

function DoT_Actions takes nothing returns nothing
 local unit u=GetTriggerUnit()
 local unit t=GetSpellTargetUnit()
 local integer i=GetPlayerId(GetOwningPlayer(u))
 local integer lvl=GetUnitAbilityLevel(u,DoT_ID())
 local timer tmr=CreateTimer()
 local real ti=0
 call StoreInteger(gc(),"gc","p",i) // Storing the player id,
 // so that we can create the dummy and the caster will still get credit
 call StoreInteger(gc(),"gc","lvl",lvl) // Storing the unit ability level,
 // so that we can deal damage depending on level
 call StoreInteger(gc(),"gc","t",H2I(t)) // Storing the target unit via the unsafe
 // H2I to I2H functions. Do never use those in your map
 call StoreReal(gc(),"gc","ti",ti) // Storing our counter
 call TimerStart(tmr,DoT_Interval,true,function DoT_Timer)
 set tmr=null
 set u=null
 set t=null
endfunction

function InitTrig_DoT takes nothing returns nothing
 set gg_trg_DoT=CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(gg_trg_DoT,EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(gg_trg_DoT,Condition(function DoT_Conditions))
 call TriggerAddAction(gg_trg_DoT,function DoT_Actions)
endfunction


That was it. It works, nothing is really wrong with it, except that it's unsafe. Because it's unsafe, I recommend you not to use this spell.


I have attached the map where it is implemented.


Now, you pretty much learned everything. Go get yourself some exercises, and yeah... enjoy what you've learned!

previous - home - next

#### Vestras

#### Vestras

 Level 9

JoinedDec 26, 2007Messages202

Resources: 13

#### Resources
Models 9Tools 1Maps 2Tutorials 1

previous - home - next

#### Vestras

#### Vestras

 Level 9

JoinedDec 26, 2007Messages202

Resources: 13

#### Resources
Models 9Tools 1Maps 2Tutorials 1

Goodbye


That was it, wasn't as hard as it seemed was it? Nah, really, it's getting easier after some time. Just keep on learning, and the step right now would probably be to learn vJASS.


This can be done via the following links;

JASSHelper Manual

Art's Moving From JASS to vJASS Tutorial


I hope that you learned something from this tutorial!

previous - home

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Really ownage! You deserve +rep  Even when I know jass

#### TheBlooddancer

#### TheBlooddancer

 Level 22

JoinedJun 24, 2008Messages3,041

Resources: 2

#### Resources
Maps 1Tutorials 1

Way better than your Blood models

But who is that Eleandor person mentioned?

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Clan CBS guy (I'm also in that Clan )

#### Vestras

#### Vestras

 Level 9

JoinedDec 26, 2007Messages202

Resources: 13

#### Resources
Models 9Tools 1Maps 2Tutorials 1

Thanks guys!

#### SmartGoodBrother

#### SmartGoodBrother

 Level 7

JoinedJul 4, 2008Messages105

Resources: 11

#### Resources
Maps 11

Eh, i think that the toturial is propably very usfull! anyhow it doesn't help me, i'm unable to learn JASS after all the 10 toturials i have read more times, i'm still in zero with JASS..


I gues it's best for me to stay at GUI after all this..

#### Arusar

#### Arusar

 Level 4

JoinedNov 9, 2008Messages112

I'm still don't understand using JASS

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Handle Vars and Gamecaches are considered "rusty" now. (but they still work)


They are just kind of deprecated. I think if they consider themselves past intermediate, they should move to vjass. Or even in the beginning too. You eventually learn it. =P


Anywho, I think that the topics should be discussed in a basic trigger and what functions are, then discussing BJs. Instead of the other way around.


Not bad though, I appreciate that a lot of people are trying to make JASS tutorials. It makes more links for us to put when someone asks how to code in jass. =D


I am not necessarily giving it "my suggestion" or "approval" though at the moment, but that doesn't mean that I am going to dis it or say that it shouldn't be in the repository. I will read more of it later.

#### Vestras

#### Vestras

 Level 9

JoinedDec 26, 2007Messages202

Resources: 13

#### Resources
Models 9Tools 1Maps 2Tutorials 1

IMO knowing that BJs isn't always good should be first. It's important, and can save you from a lot of trouble when posting resources.


Thanks for the 'review' though.

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Vestras said:

 Bump.


Click to expand...


I do this too..

#### Kanage

#### Kanage

 Level 2

JoinedJan 3, 2009Messages13

Warning!

WARNING!


In the NewGen pack, you have a link to, are there two trojan horses, inside the map "bin" (without them, you can't open any of the programs. Because of this, you can't open any of the programs without open the trojan horses and maybe release hell on your computer).


The files are both in the the map named "bin":


pip.dll


w3jdebug.exe


They where detected by my AVG Anti-Virus Free progran.

To all there have the programs from this site:  Jass NewGen Pack v5b - Wc3campaigns, should run a virus check on there computer with a anti-virus program.

#### Deaod

#### Deaod

 Level 14

JoinedNov 18, 2007Messages816

Resources: 12

#### Resources
Maps 1Spells and Systems 11

@Kanage: Yeah, right, and why does AVG identify JNGP as a trojan? Because it uses inaccurate heuristics (this is true for every Anti-Malware program that identifies JNGP as a trojan or w/e). I suggest you actually READ the thread you linked to.

First Paragraph:

 [...]

Does this tool contain viruses?

No.[...]


Click to expand...

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Deaod said:

 @Kanage: Yeah, right, and why does AVG identify JNGP as a trojan? Because it uses inaccurate heuristics (this is true for every Anti-Malware program that identifies JNGP as a trojan or w/e). I suggest you actually READ the thread you linked to.


First Paragraph:


Click to expand...


QTF, but look, hes new (1 post) and maybe he just didn't want to read all the stuff, registered and posted there. Who knows

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Uh, approved.

#### Mr.Wafflez

#### Mr.Wafflez

 Level 4

JoinedFeb 28, 2009Messages105

I still dont get jass... It was too confusing for me at the begining. I still dont understand exactly what it is. I also cant find a guide where they dont expect you too know a bit of GUI.

#### reatek

#### reatek

 Level 7

JoinedApr 13, 2008Messages183

Resources: 1

#### Resources
Maps 1

I've seen every tutorial and all of them are different and this makes it harder to understand Jass - I've been trying to use it for 1 month and I still don't know ANYTHING!

#### calle287

#### calle287

 Level 3

JoinedMar 7, 2009Messages45

Did you know that when i downloaded it my anitvirus blocked a backdoor trjona from the file Hmm....


i dont download that freaking virus file!


 Last edited by a moderator: Oct 29, 2009

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Its normal, thats because of the normal WE had been hacked in order to give such functionality. Just ignore it.

And btw, don't double post, press EDIT instead.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Yeah, lots of antivirs mark newgen as a virus. Don't worry, it's a false positive

#### kennyman94

#### kennyman94

 Level 5

JoinedNov 22, 2009Messages181

i thought functions could only return one value. or can it only return one type of value? im still very new to jass.

#### Hydramancer

H

#### Hydramancer

I'm new and i don't know GUI or WE.where can i learn

#### ClanOOKARep

#### ClanOOKARep

 Level 4

JoinedApr 14, 2009Messages118

Jass is still very confusing to me, do any of those links you put in have descriptions of like everything? im sure your tut is useful after i learn it.

#### Hodge-Podge

#### Hodge-Podge

 Level 6

JoinedMay 6, 2009Messages89

Resources: 3

#### Resources
Tutorials 3

Dude... um the link that says "Art's moving from JASS to vJASS Tutorial" takes me to some Christian website  i think you broked it some how? if not then my pc is broked...

#### Randomface

#### Randomface

 Level 2

JoinedApr 22, 2008Messages13

Im quite sure its brokded  i clicked the link to see what i would get and i got this >_< (i think your surprise is better than mine >_<)

#### vAnKaTa_G.G.

#### vAnKaTa_G.G.

 Level 4

JoinedFeb 16, 2010Messages114

I really liked the tutorial. I find it very helpful but mostly if you know GUI. I'm new to programming in general and this tutorial is kind of hard for me but I got most of the stuff.

#### Deon

#### Deon

 Level 12

JoinedFeb 13, 2009Messages388

Resources: 30

#### Resources
Icons 30

I've got everthing but this:

 function H2I takes handle h returns integer

 return h

 return 0

endfunction

function I2H takes integer i returns handle

 return i

 return null

endfunction

function H2U takes handle h returns unit

 return h

 return null

endfunction


Click to expand...

And this usage of these functions:

 local unit t=H2U(I2H(GetStoredInteger(gc(),"gc","t")))


Click to expand...

First day learning JASS .

I mean I understand what's going on, but where can I look what a "handle" type is and how does the thing work in this example?

It looks like we take some integer from gc (gamecache), then turn it to handle (what is it?) and then turn handle to a unit type.

I would like some answers instead of links to foreign resources if you can . If not, any help is appreciated!

P.S. Also we have (gc(), "gc", "t") which means "look for t in "gc"", but does it mean that we can have multiple "game caches" and how does that work? I.e. could I have:

 JASS:

local unit t=H2U(I2H(GetStoredInteger(gc(),"gc1","t")))
local unit u=H2U(I2H(GetStoredInteger(gc(),"gc2","u")))

???

>.>

#### Miss_Foxy

#### Miss_Foxy

 Level 19

JoinedOct 15, 2008Messages3,228

I have a question about 'Parameters', I really hope that you guys wouldn't mind but what's the list of 'Parameters'? It seems like many Tutorials I try to read don't really state every one of them, so I'm a bit confused about how many more of them are there, because I might get a little clearer on it once I understand the different types. Thank you in advance

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

List of types from the common.j:

 JASS:

//============================================================================
// Native types. All native functions take extended handle types when
// possible to help prevent passing bad values to native functions
//
type agent extends handle // all reference counted objects
type event extends agent // a reference to an event registration
type player extends agent // a single player reference
type widget extends agent // an interactive game object with life
type unit extends widget // a single unit reference
type destructable extends widget
type item extends widget
type ability extends agent
type buff extends ability
type force extends agent
type group extends agent
type trigger extends agent
type triggercondition extends agent
type triggeraction extends handle
type timer extends agent
type location extends agent
type region extends agent
type rect extends agent
type boolexpr extends agent
type sound extends agent
type conditionfunc extends boolexpr
type filterfunc extends boolexpr
type unitpool extends handle
type itempool extends handle
type race extends handle
type alliancetype extends handle
type racepreference extends handle
type gamestate extends handle
type igamestate extends gamestate
type fgamestate extends gamestate
type playerstate extends handle
type playerscore extends handle
type playergameresult extends handle
type unitstate extends handle
type aidifficulty extends handle

type eventid extends handle
type gameevent extends eventid
type playerevent extends eventid
type playerunitevent extends eventid
type unitevent extends eventid
type limitop extends eventid
type widgetevent extends eventid
type dialogevent extends eventid
type unittype extends handle

type gamespeed extends handle
type gamedifficulty extends handle
type gametype extends handle
type mapflag extends handle
type mapvisibility extends handle
type mapsetting extends handle
type mapdensity extends handle
type mapcontrol extends handle
type playerslotstate extends handle
type volumegroup extends handle
type camerafield extends handle
type camerasetup extends handle
type playercolor extends handle
type placement extends handle
type startlocprio extends handle
type raritycontrol extends handle
type blendmode extends handle
type texmapflags extends handle
type effect extends agent
type effecttype extends handle
type weathereffect extends handle
type terraindeformation extends handle
type fogstate extends handle
type fogmodifier extends agent
type dialog extends agent
type button extends agent
type quest extends agent
type questitem extends agent
type defeatcondition extends agent
type timerdialog extends agent
type leaderboard extends agent
type multiboard extends agent
type multiboarditem extends agent
type trackable extends agent
type gamecache extends agent
type version extends handle
type itemtype extends handle
type texttag extends handle
type attacktype extends handle
type damagetype extends handle
type weapontype extends handle
type soundtype extends handle
type lightning extends handle
type pathingtype extends handle
type image extends handle
type ubersplat extends handle
type hashtable extends agent

 You must log in or register to reply here.


### Similar threads

Locked

 Learning Jass


kingkwong

Jan 10, 2016

World Editor Help Zone

Replies8Views2K

Jan 10, 2016
kingkwong

Locked

 GUI to jass, impossible


subressor

Jan 27, 2008

World Editor Help Zone

Replies13Views2K

Jan 29, 2008
overload119

 JASS: Moving From GUI to Jass, the Start


Rheias

Sep 26, 2007

Scripting Tutorials
2

Replies61Views32K

Jul 22, 2008
Vercas

 JASS: A Concise Introduction


Ghan_04

Feb 27, 2008

Scripting Tutorials

Replies19Views14K

Jun 20, 2009
Foopad

Locked

 Advice please. I'm going to learn JASS...


Greenwhy

Mar 8, 2012

World Editor Help Zone

Replies12Views3K

Mar 10, 2012
Greenwhy

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