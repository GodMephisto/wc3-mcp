---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/simple-jass-tutorial-for-beginners.251698/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Simple JASS Tutorial for Beginners
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/simple-jass-tutorial-for-beginners.251698/>
#### Deleted member 219079

D

#### Deleted member 219079

Last time updated : 21.10.2014

Your guide, la

trustworthy cat!

Required :

Jass NewGen WE

Knowledge of GUI

Time
[r]Recommended :

Experience with coding
[r]Table of Contents :

Basics of JASS I

vJASS features

Basics of JASS II

Using JASS/vJASS

Efficient JASS coding tips

End of tutorial

1 : Basics of JASS I

JASS is a scripting language for Warcraft III. It is rumored to stand for Just Another Scripting Syntax, although there's no validation for it. GUI stands for Graphical User Interface, which represents basically JASS but in more graphical way. People who have learned/mastered JASS might say: "don't use gui", and call it bad, but it's totally up to you whether you use GUI or not. Do you recognize these GUI lines?

Trigger
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Custom script: //line

Custom script: //another line

Custom script: call RemoveUnit(CreateUnit('u000', Player(0), 0, 0, 270))

Yes, custom scripts. Now you might be familiar with call RemoveLocation(udg_p) , call DestroyGroup(udg_ug) and call DestroyForce(udg_pg), especially if you're a spell maker. Now what these do is they release memory held by objects, for example call RemoveLocation() removes Location(x,y) from it, so you can't use it anymore as it doesn't know what you're referring to. This is essential for keeping your RAM free. If you don't do this, it's called leaking. Removing leaks is what mods require from you before you can get your spell approved.

1.1 : Our first JASS trigger

Here's a simple JASS code for you to create:

 JASS:

function myCondition takes nothing returns boolean
 return false
endfunction

//===========================================================================
function InitTrig_myTrigger takes nothing returns nothing
 local trigger t = CreateTrigger( )
 call TriggerAddCondition( t, Condition(function myCondition) )
 set t = null
endfunction

Let's see what I did there. First I converted empty trigger called myTrigger to JASS code via Edit > Convert to Custom text.

Then I removed everything before commented line, and cleared code in InitTrig_myTrigger.

Now let's take a look at local trigger t = CreateTrigger(), we create local variable (not accessible outside the function) trigger t, and assign function CreateTrigger() to it. Why can we assign function to it? Because native CreateTrigger takes nothing returns trigger returns trigger. We don't need any arguments to this function because it says it takes nothing (only fill parentheses when the function asks for it). Simple, isn't it?

Then comes the next line call TriggerAddCondition( t, function myCondition ), it's another function which does what it says: assigns a condition to a specific trigger. We tell it to assign function myCondition, which we have above, to trigger which we just created locally. Condition of course has to return a boolean, so we make myCondition return false. Why? Because if a condition (any of them) returns false, it'll not trigger the actions. And since we have no actions, we'll simply return false.

As a side note, JASSers often put all their "Actions" into the condition function, instead of creating one function for a condition and one function for actions. It saves handles and is faster.

You might wonder, why do we create trigger locally? The trigger isn't actually local, we can't decide (in vanilla JASS), whether trigger is local or not, it's just the variable which is local, trigger stays trigger. If you're confused, let's make it more simple:

Let's imagine there's a variable type "door". We create new variable of door called handle. If we use set handle = null, we don't actually remove the door. The door still exists, but we just removed our way of using the door, by removing a handle for it. This is the reason why you need to use call RemoveLocation(udg_myLocation), even in gui, because if you just assign myLocation to new location, you have a leak in your code.

Hive user chobibo explained this further, expand this if you want to study the role of variables a bit more:

Variables are really just identifiers, so when you create a new variable, it really points to nothing [useful] (null on global handle types but undefined on local handle types), now when you create an "object" (an object is a data that we use, like regions, units, etc.) using a "function" (CreateUnit, etc..) you use data, that data can be "linked to" by a variable. e.g.:

 JASS:

local location point // This is a declaration, it tells the compiler
 // (warcraft) to create a variable. The variable
 // is empty, it does not link to anything useful.

local location point = Location(0,0) // This time, the declaration
 // and use is done simultaeneously
 // The variable now links to
 // something useful.

Now, that "point" uses memory, if were done with it (no longer need it) we must destroy it to free the memory space it uses. The first step is to tell warcraft that we no longer need it using a function Blizzard supplied.

 JASS:

call RemoveLocation(point) // This function tells warcraft that the
 // object linked to by "point" is no longer
 // useful. This is the first step to remove
 // this unuseful data.

Now, the variable links to (or points to) something useless, to avoid errors, Blizzard made it so that when a variable still links to something in memory (the now useless data, subjective of course), that said memory will not be freed. The next, albeit important and last, step is to make our variable point to "null".

 JASS:

set point = null // This tells warcraft to make the variable
 // point link to the "null" value. This in
 // effect allows warcraft to recycle the
 // memory that the previously pointed object
 // by the variable "point" is linking to
 // (pointing to).

Failing to do this crucial step will result in the memory being used by the object our variable links to (points to) unreachable, hence it will not be recycled by warcraft, *cue scary music ♫dum dum dumdam ♫* that is called a "memory leak". Too much memory leaks will cause your map to lag and eventually crash the game...

Now that we've done that, warcraft will recycle that chunk of memory for us!

Now, we've just freed a memory block thanks to that!

So after we've done assigning condition to the trigger, we can nullify the variable, without touching the trigger itself. We do this by typing set t = null.

1.2 : Functioning Trigger

Now let's make the trigger actually do something:

 JASS:

function myCondition takes nothing returns boolean
 call BJDebugMsg( "lol" )
 return false
endfunction

//===========================================================================
function InitTrig_myTrigger takes nothing returns nothing
 local trigger t = CreateTrigger( )
 call TriggerRegisterTimerEvent( t, 0, false )
 call TriggerAddCondition( t, Condition(function myCondition) )
 set t = null
endfunction

Let me clear the functions I added for you:

call BJDebugMsg( "lol" ) this will outprint message lol.

call TriggerRegisterTimerEvent( t, 0, false ) let me get the declaration for you: native TriggerRegisterTimerEvent takes trigger whichTrigger, real timeout, boolean periodic returns event we just connect things together by using values we want as arguments. We want it to affect trigger t, timeout at 0 seconds, and not loop itself. This is pretty self-evident.

You may test the trigger in your map now. If it doesn't print out lol after 0 seconds of game-time, make sure the trigger name is myTrigger and repeat the steps if necessary.

2 : vJASS features

vJASS stands for very-JASS. It was made by Vexorian, who is a god of vJASS who you'll never outperform. vJASS syntax adds additional features to JASS, this is why you'll need your own syntax checker for it. Make sure you have your JNGP (jasnewgenpack) working and ready before going further in this section. Remember because it uses its own syntax checker, you need to save your map before testing it.

2.1 : How to get vJASS

Download link for user made WE:http://blizzardmodding.info/4263/the-jass-newgen-pack-jngp-2-0/

Extract jassnewgenpack2XX.7z to somewhere on you PC

Start NewGen WE.exe, if it doesn't boot up, try to contact the program creator

(optional) Attach the New WE to your Start Menu for faster access

2.2 : Scopes and private functions

Hopefully you haven't removed myTrigger which we made in the last chapter, we're going to transform it to vJASS world

 JASS:

scope myScope initializer init
 function myCondition takes nothing returns boolean
 call BJDebugMsg( "lol" )
 return false
 endfunction

 //===========================================================================
 function init takes nothing returns nothing
 local trigger t = CreateTrigger( )
 call TriggerRegisterTimerEvent( t, 0, false )
 call TriggerAddCondition( t, function myCondition )
 set t = null
 endfunction
endscope

We added scope <name> initializer <function name> to the top row and endscope to the bottom row.

Initializer is function that's ran in the loading screen, just like Map Initialization Event in GUI.

Scope is something I recommend you to use when you're making a code which is not used elsewhere, a code which you want to work as an individual. Scopes can have private members, let's explore that feature now.

If you save and test (Ctrl + S and Ctrl + F9) the map, it does exactly the same thing, outprints lol. Why? Because vJASS is just like JASS, the syntax is just a little bit more advanced. Here's a situation where in normal JASS would happen a collision: We create a new trigger myProof:

 JASS:

scope myProof initializer init
 function myCondition takes nothing returns boolean
 call BJDebugMsg( "lol" )
 return false
 endfunction

 //===========================================================================
 function init takes nothing returns nothing
 local trigger t = CreateTrigger( )
 call TriggerRegisterTimerEvent( t, 0, false )
 call TriggerAddCondition( t, function myCondition )
 set t = null
 endfunction
endscope

If we try to save it the syntax checker will nag at us about redeclaring and stuff like that. To prevent it, we make functions inside these two scopes local to them. Just make function myFunction --> private function myFunction, add private in front of each function. Now you can't use the function outside it. But what if we want to, but not to cause collisions?

2.3 : Libraries and public functions

Make the triggers look like this (the function lines):

 JASS:

scope myScope initializer init
 public function myCondition takes nothing returns boolean
 call BJDebugMsg( "lol" )
 return false
 endfunction

 //===========================================================================
 private function init takes nothing returns nothing
 local trigger t = CreateTrigger( )
 call TriggerRegisterTimerEvent( t, 0, false )
 call TriggerAddCondition( t, function myCondition )
 set t = null
 endfunction
endscope

Try to save it. It should not nag about functions, even when we made them public. Calling the functions outside them is not that simple, you need to do it in syntax of scopeName_functionName. Here's an example trigger:

asd
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Custom script: call myScope_myCondition()

Custom script: call myProof_myCondition()

Libraries are scopes which can require other libraries. Libraries are directly connected to the map header code, and requiring means simply this: If lib A requires lib B, computer puts lib B above lib A in the map header code. Let's make our first library:

 JASS:

library myLib initializer init
 private function init takes nothing returns nothing
 call BJDebugMsg("hello!")
 endfunction
endlibrary

Save and test the map, you should see hello! , lol , lol , lol and lol printed on the screen.

As libraries are inserted to the map header code and scopes' role is to be "individual", scopes can use any non-private function of any library in your map.

For library to use other library's functions you need to use library <name> requires <name>, as explained before.

2.4 : Globals and private variables

Local variable is private to that function by default, but what if we want to use our variable in another function? You can use udg, which stands for user defined global. But this is slow and requires you to use GUI's interface. This is why I suggest you to use globals, variables which are defined like GUI's variables, at initialization. They are linked to your map via your trigger, so you don't need to remove them manually after you've quit using a system, unlike GUI's variables. Let's practice:

 JASS:

scope myGlobals initializer init
 globals
 string MYSTRING = "asd"
 endglobals
 private function init takes nothing returns nothing
 call BJDebugMsg(MYSTRING)
 endfunction
endscope

Its name is written in caps lock for clarity. Your possible questions reaching your mind might be: 1. Where do I place them? 2. Do I have to assign them to something? 3. How to make array?

1; You place globals on top of your trigger 2; You don't have to assign them to anything (you can just define MYSTRING like string MYSTRING) 3; You make array like this: string array MYSTRING, but then you can't assign them at globals section.

We talked about collisions between triggers in out previous segment. There can be collisions between globals too. To prevent this, type private in front of your global. Now your trigger should look something like this:

 JASS:

scope myGlobals initializer init
 globals
 private string MYSTRING = "asd"
 endglobals
 private function init takes nothing returns nothing
 call BJDebugMsg(MYSTRING)
 endfunction
endscope

You can use non-private globals outside your trigger. This is because they're generated on top of map code:

 JASS:

globals
 // User-defined
string udg_MYSTRING

 // Generated
string MYSTRING= "asd"

endglobals

Above you can see map code going through syntax checker. I have global variable MYSTRING in my scope, and GUI's variable MYSTRING in GUI. They're created at the same segment in reality, but in our editing environment they're placed elsewhere.

2.5 (optional) : Structs

Structs are inspired by object oriented programming (OOP). They can make your life a lot easier.

Here's some concept explanations:

struct is an object defined by coder

instance is an individual object of struct made with allocate()

member is a variable which is used by struct

method is a function which belongs to struct

static means same amongst all instances of struct

allocate means giving instance its own index

deallocate means destroying an instance

extends is something I'm not gonna cover

extends array is awesome

Let's make trigger which does absolutely nothing in game:

 JASS:

scope myScope initializer init
 struct myStruct

 endstruct
 private function init takes nothing returns nothing
 local myStruct A=myStruct.create()
 call A.destroy()
 endfunction
endscope

myStruct is the name of our struct type. In function init we create a pointer, and instantly issue it to point to newly created struct of type myStruct.

<structtypename>.create() gives our struct a unique id. Structs use arrays, with maximum of 8191 values. This is actually 8190, index of 0 is used for null.

You can use structs as globals, but you can't initialize them in globals section. You need to do this in init function.

Let's create a member str for our struct:

 JASS:

scope myScope initializer init
 struct myStruct
 string str
 endstruct
 private function init takes nothing returns nothing
 local myStruct A=myStruct.create()
 set A.str="lol"
 call BJDebugMsg(A.str)
 call A.destroy()
 endfunction
endscope

When I added string str in the struct, I added a member for it. Then we declared it in our init function. After that we called the member via BJDebugMsg using <pointername>.<membername>. Let's give our struct a function which runs on creation:

 JASS:

scope myScope initializer init
 struct myStruct
 string str
 static method create takes nothing returns thistype
 local thistype returned = thistype.allocate()
 set returned.str = "lol"
 return returned
 endmethod
 endstruct
 private function init takes nothing returns nothing
 local myStruct A=myStruct.create()
 call BJDebugMsg(A.str)
 call A.destroy()
 endfunction
endscope

thistype = is the name of the struct. But you can use your struct's name (myStruct in this example).

Static methods can't be called with <instance>.<method>(), they must be called with <structtype>.<method>().

We can use keywords private and public in structs too. Try to add keyword public in front of static method create, it doesn't nag about it. But when we add private, we can't use the method outside of struct anymore. Let's modify the struct so that its str can be private:

 JASS:

scope myScope initializer init
 struct myStruct
 private string str
 public static method create takes nothing returns thistype
 local thistype returned = thistype.allocate()
 set returned.str = "lol"
 return returned
 endmethod
 public method debugMsg takes nothing returns nothing
 call BJDebugMsg(str)
 endmethod
 endstruct
 private function init takes nothing returns nothing
 local myStruct A=myStruct.create()
 call A.debugMsg()
 call A.destroy()
 endfunction
endscope

I made a new method in our struct which is no static, so we can call it in out init function with <instancename>.<methodname>.

Now, members can be static (non-indexed) too:

 JASS:

scope myScope initializer init
 struct myStruct
 private static string str = "lol"
 public static method create takes nothing returns thistype
 local thistype returned = thistype.allocate()
 return returned
 endmethod
 public method debugMsg takes nothing returns nothing
 call BJDebugMsg(str)
 endmethod
 endstruct
 private function init takes nothing returns nothing
 local myStruct A=myStruct.create()
 call A.debugMsg()
 call A.destroy()
 endfunction
endscope

It doesn't nag about declaring static member in our create method, but it makes no sense to do so. So we simply declare it in the members section.

If you want to go more in-depth about them, follow the link at the last chapter.

2.6 (optional) : Textmacros

//! textmacro is your personal CnP tool made possible by vJASS. Let's imagine you want to loop different texts many times:

 JASS:

scope myScope initializer init
 //! textmacro LoopText takes STR
 call BJDebugMsg("$STR$")
 call BJDebugMsg("$STR$")
 call BJDebugMsg("$STR$")
 //! endtextmacro

 private function init takes nothing returns nothing
 //! runtextmacro LoopText("asd")
 //! runtextmacro LoopText("lol")
 endfunction
endscope

Textmacros are begun with //! textmacro <name> takes <parameter>, <parameter>, and ended with //! endtextmacro, the parameters are attached to the macro via $-marks. Textmacros are called with //! runtextmacro <name>(<parameters>)

Attaching textmacros to your code is made first on progress of converting your vJASS code to something WC III can handle. Now, you can even create scopes with textmacros:

 JASS:

//! textmacro NewScope takes NAME, STR
 scope $NAME$
 globals
 private string STR = "$STR$"
 endglobals
 public function loop10 takes nothing returns nothing
 local integer i = 0
 loop
 call BJDebugMsg(STR)
 exitwhen i == 10
 set i = i + 1
 endloop
 endfunction
 endscope
//! endtextmacro

//! runtextmacro NewScope("asd","lol")
scope myScope initializer init

 private function init takes nothing returns nothing
 call asd_loop10()
 endfunction
endscope

This loops "lol" 10 times for you. There's so much you can create with textmacros, explore the features yourself and you can become the next JASS god

3 : Basics of JASS II

We've learned functions, scopes, libraries, locals, globals, privates and publics. That's a lot of coverage, but there's a lot more to be covered. We'll now learn a couple of new events and how to use them.

3.1 : Periodic Loop

Let's make a looping trigger which prints out "hi" every 2 seconds:

 JASS:

function myFunc takes nothing returns boolean
 call BJDebugMsg("hi")
 return false
endfunction

//===========================================================================
function InitTrig_myTrig takes nothing returns nothing
 local trigger t = CreateTrigger( )
 call TriggerRegisterTimerEvent( t, 2, true )
 call TriggerAddCondition( t, Condition( function myFunc ) )
 set t = null
endfunction

You can recognize a function call TriggerRegisterTimerEvent( t, 2, true ) which we used in part 1 also. Part 1's trigger didn't loop itself, however. Let's take a look at the declaration: native TriggerRegisterTimerEvent takes trigger whichTrigger, real timeout, boolean periodic returns event we changed timeout from 0 to 2 and boolean from false to true. This way we got ourselves a 2 second loop.

3.2 : Timer Loop

Do you know how you can create timers and timer windows in GUI? We can use those timers in JASS to trigger functions. Let's modify our trigger by adding globals, which we learned from previous chapter:

 JASS:

globals
 timer TIMER = CreateTimer()
endglobals

function myFunc takes nothing returns nothing
 call BJDebugMsg("hi")
endfunction

//===========================================================================
function InitTrig_myTrig takes nothing returns nothing
 call TimerStart( TIMER, 2, true, function myFunc )
endfunction

Timer can be stopped, so it's more advanced. You can stop the timer by calling call PauseTimer( TIMER )

3.3 : Ability Event

Just like in GUI, you can get event for ability cast. In GUI it'd be like this:

GUI
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Thunder Clap

Actions
Custom script: call BJDebugMsg("lol")

In JASS the equivalent of this would be:

 JASS:

function myFunc takes nothing returns boolean
 if GetSpellAbilityId() == 'AHtc' then
 call BJDebugMsg("lol")
 endif
 return false
endfunction

//===========================================================================
function InitTrig_JASS takes nothing returns nothing
 local trigger t = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition( t, Condition( function myFunc ) )
 set t = null
endfunction

'AHtc' is a rawcode, I'll cover it up later in "4.5 : Unit IDs".

There's a couple of new functions for me to cover:

GetSpellAbilityId() == 'AHtc' this is a boolean, the real function here is;

GetSpellAbilityId(), this is a native which gets the ID of an ability, which triggered the function.

I'll talk about if then endif tags and 'AHtc' ID in the next chapter more.

call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_SPELL_EFFECT ) when there's a BJ at the end of a function, it means there's a script connected to it. For example the function here is actually the script below:

 JASS:

function TriggerRegisterAnyUnitEventBJ takes trigger trig, playerunitevent whichEvent returns nothing
 local integer index

 set index = 0
 loop
 call TriggerRegisterPlayerUnitEvent(trig, Player(index), whichEvent, null)

 set index = index + 1
 exitwhen index == bj_MAX_PLAYER_SLOTS
 endloop
endfunction

This loops the event (unit owned by <player> casts a spell) for every player.

Place a Mountain King on the map, give him thunder clap (set the level from 0 to 1 at the terrain editor) and save+run the map. It should outprint "lol" everytime you cast thunder clap. If it doesn't CnP my trigger to your map and make sure you have no other triggers on your map (after all we haven't made it private or anything.)

3.4 : Scope transformation

Let's transform the JASS trigger we made into vJASS:

 JASS:

scope mySpell initializer init
 private function myFunc takes nothing returns boolean
 if GetSpellAbilityId() == 'AHtc' then
 call BJDebugMsg("lol")
 endif
 return false
 endfunction

 //===========================================================================
 private function init takes nothing returns nothing
 local trigger t = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition( t, Condition( function myFunc ) )
 set t = null
 endfunction
endscope

Now the trigger can't collide with other triggers, as it has private functions. But if you have two scopes called mySpell, you're in for a nagging by syntax checker. Save and test run, it should do exactly same thing as before.

In the next chapter you'll learn advanced methods to use inside functions. Combine them with stuff learned in this section, and you'll able to create full JASS triggers

4 : Using JASS/vJASS

You may now remove the triggers you've made before, they're not necessary anymore. Now we'll learn how to use JASS. I will show you multiple functions, which you can combine in different ways for awesome triggers

4.1 : Unit Groups

Create couple of footmen and couple of peasants on the map.

In this code I trigger every unit pre-placed on the map editor to be colored blue. I've added comments for clarifying stuff.

 JASS:

scope myScope initializer init
 private function groupAction takes nothing returns nothing
 call SetUnitColor(GetEnumUnit(), PLAYER_COLOR_BLUE) //we change the color for picked unit
 endfunction
 private function init takes nothing returns nothing
 local group g = CreateGroup() //create variable of type group, assign it to newly created group
 call GroupEnumUnitsInRect(g, bj_mapInitialPlayableArea, null) //assign units in playable map area to group g
 call ForGroup(g, function groupAction) //we assign every unit in the group through a function
 call DestroyGroup(g) //no use for the unit group anymore, destroy it
 set g = null //nullifying, I've covered this step in chapter 1
 endfunction
endscope

We get the picked unit (the unit which is currently going through groupAction) with GetEnumUnit().

Now do notice how I've placed the groupAction above init function. Because we need the groupAction function in init function, we have to place it above it.

4.2 : Loops

Loop is something that repeats given code until it can be concluded.

We use loop exitwhen true endloop tags for this structure;

 JASS:

function GetStrength takes unit u returns integer
 local integer i
 loop //loop, just like in gui
 set i = i + 1
 exitwhen i == GetUnitLevel(u) //exitwhen (true) quits the loop
 endloop //end the loop
 return i
endfunction

Don't use this function in your map, it's worthless  Let's use the loop a bit differently.

exitwhen <boolexpr> is necessary for the loop to conclude successfully. If you don't exit your loop, you'll reach OP (operation) limit and crash the thread (simply put a process). exitwhen true works fine if you want to exit the loop without expections.

4.3 : Group Loop

Hope you didn't remove the footmen and peasants, we'll now recolor them via different kind of method

 JASS:

scope myScope initializer init
 private function init takes nothing returns nothing
 local group g = CreateGroup()
 local unit FoG //create handler for unit, don't assign it to anything
 call GroupEnumUnitsInRect(g, bj_mapInitialPlayableArea, null)
 loop //loop, just like in gui
 set FoG = FirstOfGroup(g) //assign FoG to first unit in g
 exitwhen FoG == null //exitwhen (true) quits the loop
 call SetUnitColor(FoG, PLAYER_COLOR_BLUE) //custom function, not necessary for FoG loop
 call GroupRemoveUnit(g, FoG) //remove FoG from g to not to cause infinite loop
 endloop //end the loop
 call DestroyGroup(g) //destroy g
 set g = null //nullify g
 endfunction
endscope

Save and test the map, you should see your units turn blue. If they did, good job, you're doing great!

But be careful when using this kind of loop, while it has no problems on-demand filled groups (groups that are filled by enumerations within the local block where you perform the FoG iteration), it has issues with groups holding units for a period of time. See GroupUtils for more details.

4.4 : If / Then / Else

if then else elseif endif, these are the elements used for creating conditions inside functions. We'll have native GetRandomInt takes integer lowBound, integer highBound returns integer to do us the condition. Modify the loop part on your trigger to look something like this:

 JASS:

scope myScope initializer init
 private function init takes nothing returns nothing
 local group g = CreateGroup()
 local unit FoG
 call GroupEnumUnitsInRect(g, bj_mapInitialPlayableArea, null)
 loop
 set FoG = FirstOfGroup(g)
 exitwhen FoG == null
 if GetRandomInt(0,1)==0 then //if (true) then
 call SetUnitColor(FoG, PLAYER_COLOR_BLUE) //do this
 else //if not (true) then
 call SetUnitColor(FoG, PLAYER_COLOR_CYAN) //do this instead
 endif //remember to always end your if:s!
 call GroupRemoveUnit(g, FoG)
 endloop
 call DestroyGroup(g)
 set g = null
 endfunction
endscope

Again save and test the map, you should see that some of the peasants turned teal instead of blue.

4.4.1 : Elseif

elseif is only checked at if the first if didn't trigger. Example:

 JASS:

if myInt==3 then
 call BJDebugMsg("it's 3!")
elseif myInt==1 then
 call BJDebugMsg("it's 1!")
else
 call BJDebugMsg("it's alien!")
endif

This will come handy at some point of your code.​

4.5 : Unit IDs

Go to object editor, find footman. Press hotkey Ctrl + D, you should see the name of footman giving you a code "hfoo". This is actually integer. The format in JASS is 'hfoo'. Now did I call it integer, it looks more like a string to us right? call BJDebugMsg(I2S('hfoo')) will print out 1751543663, so it is integer after all, it's just in disguise. Let's turn footmen blue, and peasants teal.

 JASS:

scope myScope initializer init
 private function init takes nothing returns nothing
 local group g = CreateGroup()
 local unit FoG
 call GroupEnumUnitsInRect(g, bj_mapInitialPlayableArea, null)
 loop
 set FoG = FirstOfGroup(g)
 exitwhen FoG == null
 if GetUnitTypeId(FoG)=='hfoo' then //if the picked unit has this integer as id, it'll return true
 call SetUnitColor(FoG, PLAYER_COLOR_BLUE)
 else
 call SetUnitColor(FoG, PLAYER_COLOR_CYAN)
 endif
 call GroupRemoveUnit(g, FoG)
 endloop
 call DestroyGroup(g)
 set g = null
 endfunction
endscope

Save and test, if footmen turned blue and peasants teal, I'm damn proud of you

4.6 : Group filters

Let's fix the null in call GroupEnumUnitsInRect(g, bj_mapInitialPlayableArea, null). The declaration for the native is native GroupEnumUnitsInRect takes group whichGroup, rect r, boolexpr filter returns nothing so you can see the null is boolexpr. It filters whether an unit can be added to the group. Let's make a simple filter which checks if unit is dead:

 JASS:

private function groupFilter takes nothing returns boolean
 return not IsUnitType(GetFilterUnit(), UNIT_TYPE_DEAD)
endfunction

Naturally you need to change the line where you define your group to this --> call GroupEnumUnitsInRect(g, bj_mapInitialPlayableArea, Filter(function groupFilter))

Save and test, it shouldn't affect the map in any way.

I only scratched the surface of JASS features. Why? Because when you discover stuff by yourself, you get the most out of it. When editing JASS code in JNGP, you should see "Function List" button above your code. Click and explore it well, and you can become the next JASS god  And I really recommend messing with FoG loop, it's very effective way of browsing through unit groups in a single function.

5 : Efficient JASS Coding Tips

We all know our code can cause major performance impact on the map we work on. Sometimes the performance drop can be felt only after an hour of gameplay, due to thousands of leaks in your map.

I don't want my dear student to make badly written maps, so here's some tips to avoid faulty scripts.

5.1 : Always null abstract locals

Certain non-nullied locals can cause performance impact. Abstract is something conceptual, for example an unit; we define unit by certain properties and unit is always a subject to change in-game.

Something not abstract is integer, we are certain 3 is 3.

To null local use set myLocal = null

For further learning about which variable types should be nullied, open up this page. Null everything which extends widget or agent; these have abstract properties so you need to null them.

5.2 : Avoid extra native calls

Native functions are super useful and without them you basically couldn't do anything visual or physical in-game. But as these are often visual functions, they have their performance impact.

By extra I mean like this:

 JASS:

call BJDebugMsg(GetUnitName(GetTriggerUnit())+" just killed someone!")
call BJDebugMsg("he's level "+I2S(GetUnitLevel(GetTriggerUnit())))

You could just store the result gained from GetTriggerUnit() - native into a variable.

For example, hashtables have more flexible structure than variable arrays or plain variables, but take more memory AND are slower on delivering data.

Everytime you're planning to use only one dimension of hashtable, you should consider; could it be possible with only arrays?

E.g. set myString = LoadStr(HASHTABLE, 0, i) --> set myString = STRING[i]

Other example is hidden native calls. Most often BJs are functions like:

 JASS:

function AttachSoundToUnitBJ takes sound soundHandle, unit whichUnit returns nothing
 call AttachSoundToUnit(soundHandle, whichUnit)
endfunction

Avoid hidden native calls to ensure efficient code.

5.3 : Pre-calculate for loops

Example right away:

 JASS:

function loop
 ...
 set x = GetUnitX(missile)
 set y = GetUnitY(missile)
 set angle = GetUnitFacing(missile)
 set x = x + 5 * Cos(angle*bj_DEGTORAD)
 set y = y + 5 * Sin(angle*bj_DEGTORAD)
 call SetUnitX(missile, x)
 call SetUnitY(missile, y)
 ...
endfunction
function cast
 ...
 set missile = CreateUnit(...)
 ...
endfunction

-->

 JASS:

function loop
 ...
 set x = x + 5 * Cos(angle)
 set y = y + 5 * Sin(angle)
 call SetUnitX(missile, x)
 call SetUnitY(missile, y)
 ...
endfunction
function cast
 ...
 set x = GetUnitX(caster)
 set y = GetUnitY(caster)
 set angle = GetUnitFacing(caster)*bj_DEGTORAD
 set missile = CreateUnit(...)
 ...
endfunction

It's good to reduce function calls for your periodic loops, it makes your spell run smoother and stuff like unit angle, which won't change, can be pre-calculated.

5.4 : Use Proper Indexing

Proper indexing is for example dynamic indexing. It's a bit hard to visualize, so I recommend reading Visualize: Dynamic Indexing by PurgeandFire.

For alternative solution and ease of use, download TimerUtils made by Vexorian.

A quick guide:

local timer t = NewTimer() - creates a new timer

call SetTimerData(t, myInteger) - defines a number you want to attach to a timer

call GetTimerData(t) - obtains the number you attached before

call ReleaseTimer(t) - destroys the timer

Instead of looping through all of the instances via one loop - as you probably got used to in GUI world - use timers which function on their own times. This increases performance too.

Remember to release the timer after you don't need it!

Downside of TimerUtils is that it uses more memory. Downside of dynamic indexing is that it forces the computer to go through all of the instances at once.

5.5 : Use variables efficiently

Example:

 JASS:

function myLoop takes nothing returns nothing
 ...
 local group g = CreateGroup()
 call GroupEnumUnits(g)
 loop
 set FoG = FirstOfGroup(g)
 exitwhen FoG == null
 call GroupRemoveUnit(g, FoG)
 endloop
 call DestroyGroup(g)
 set g = null
 ...
endfunction

This is a standard FoG loop, what's bad about it? The function name, it's a looping function, and looping functions gotta be as fast as possible.

Avoid 2 native calls by using global group:

 JASS:

globals
 private constant group loopGroup = CreateGroup()
endglobals
function myLoop takes nothing returns nothing
 ...
 call GroupEnumUnits(loopGroup)
 loop
 set FoG = FirstOfGroup(loopGroup)
 exitwhen FoG == null
 call GroupRemoveUnit(loopGroup, FoG)
 endloop
 ...
endfunction

By adding constant in front of the group, you ensure you won't null it in accident.

As a side note, you don't have to null FoG, loop concludes only when the FoG is null so the loop takes care of it.

5.6 : Avoid interfaces

When you call a method of a struct which extends a certain interface, vJass compiler will convert it to TriggerExecute which is another native function, which isn't one of the faster ones.

Avoid interface usage especially on looping functions which are supposed to be fast. I bet you can come up with a workaround, my dear JASS student!

5.6.1 Avoid struct a extends b

You can avoid a major mess by not using extends at all. I recommend you to do so.

​

5.7 : Use Unit Indexer

I recommend using Perfect Unit Indexing by Cohadar. It indexes only units you need, and is pretty lightweight. There's some other good indexers too.

A quick guide:

local integer index = GetUnitIndex(u) - gets unit's personal number, creates it if not created yet

local unit u = GetIndexUnit(index) - gets a unit bound to a certain number, if there is any

There's a native call behind GetUnitIndex, so I recommend indexing e.g. caster's index number instead of calling it in a looping function

5.8 : Don't do stuff which is already done

Here's libraries (beside PUI and TimerUtils) I often use and recommend you to also use:

GeometryLibBase by moyack - Proper way of getting an angle from point to point

Table by Bribe - One hashtable for your whole map, requires a bit of brainwork to get started

Table by Vexorian - Alternative to the better Table

IsDestructableTree by BPower - depends on your map whether you need it, but is certainly useful

StructuredDD by Cokemonkey11 - Good for certain systems and spells, when you need "is attacked" event

6 : End of tutorial

Hope I taught you something, just PM me if there's something you're having troubles with  Just remember to complete your map before pumping it to maximum with cool systems, you never know when you're going to lose interest in your project :/ But that's not up to me to teach, go learn about map-making somewhere else ^^

Sources

Wikipedia

Malhorne

Own experience​

For advanced tutorials, I recommend:

Hive user iAyanami has written a very good tutorial about structs

Hive user Earth-Fury has made a marvelous explanation of how computer checks your booleans

Hive user Silvenon has written very in-depth tutorial about knockbacks, check it out if you're planning on creating a kb system (requires knowledge of structs)

Thanks to:

edo494, for fix suggestions

Malhorne, for helping to finish the tutorial and teaching me JASS

chobibo, for helping with tutorial and fix suggestions

Magtheridon96, who reviewed my first GUI spell and gave me motivation to continue learning GUI

PurgeandFire, for reviewing this tutorial

#### Attachments

asdfasdfasdf.PNG

 2 MB

 · Views: 848



avatar219079_6.gif

 59.6 KB

 · Views: 2,333



jasstutorial.png

 77.1 KB

 · Views: 2,351



firstJassTrigger.png

 69.5 KB

 · Views: 2,737




 Last edited by a moderator: Oct 21, 2014

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

1. Jass is scripting language, not programming

2. TriggerAddCondition(trig, function T) is not valid Jass. Its valid if you have Vexorian's JassHelper(dont know if Cohadars allows this).

This is, because if you pass only function T inside, it must return nothing. If it is meant to be used in TriggerAddCondition, it must be passed to Condition or Filter.

This is implicitly done when your vJass compiles to Jass(you can look at the source, and you will see it will have Condition call)

You could at least mention this, especially because you have it inside Basics of Jass, and this is illegal Jass code.

3.

 Condition of course has to return a boolean, so we make myCondition return false, we don't want the computer to continue further in the trigger, so it'll exit the trigger after return false and won't look for actions anymore.


Click to expand...

This may sound for new users, as when the end of the function is reached, nothing else is ever checked. All conditions are checked, and if and only if they all return true, the body is checked. You could reformulate that a little bit maybe.

4.

 The trigger isn't actually local, we can't decide (in vanilla JASS), whether trigger is local or not, it's just the handle which is local


Click to expand...

While right, this kind of masks the real concept of the things. The variable is local, the object is global nevertheless.

You could mention something like this in there(we also null the variable, not the handle, the handle is as local as the trigger object, not at all)

5. You could maybe explain why we must null all local agents, not just say to do that

6.

 Vexorian is a god of vJASS who you'll never outperform.


Click to expand...

This is very subjective. And since he wrote code that uses inheritance and stuff like that, as long as you dont use that, you outperform his code already. Yes, you dont outperform him directly, but you cant outperform someone who you dont know how good he is at writing stuff. And pretty much noone knows that, maybe someone that knows him IRL or worked with him on something

7. //create handler for group, assign it to newly created group

this is quite wrong. You dont really define handler for group, you define local variable, which points to the newly created group. Pointer != handler

the same goes for //create handler for unit, assign it to nothing, you dont assign it to nothing(null), you dont assign it at all(again, not handler, but pointer)

The handler, pointer thingy is just a play of a word and if you think that new Jassers will understand the jass better by reading it as handler, feel free to keep it there. We dont need to call things exactly for sake of calling it exactly.

I ran out of time, but I dont really see anything that I could nitpick

#### Deleted member 219079

D

#### Deleted member 219079

It says you need jass newgen we at beginning, sorry this is my first tutorial I'm kind of bad at explaining as of now...

To be honest I don't know what a pointer is. However I've saw a list where wc3 engine defines handle trigger, handle unit, handle location etc. So that's why I thought they're handles, stupid me...

Thanks A LOT for the feedback though, will fix the things you pointed out tomorrow

#### Cokemonkey11

#### Cokemonkey11

Spell Reviewer

 Level 30

JoinedMay 9, 2006Messages3,572

Resources: 25

#### Resources
Tools 4Maps 8Spells and Systems 3JASS 7Tutorials 2Wurst 1

 People who have learned/mastered JASS might say: "don't use gui", I say this is wrong


Click to expand...

You're wrong

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

The fact that you have jngp doesnt mean you have JassHelper enabled.

Trigger, unit and all this are handles, but local variable is not handler

#### Deleted member 219079

D

#### Deleted member 219079

Oh, didn't know that  thanks for clarifying!

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

jondrean said:

 People who have learned/mastered JASS might say: "don't use gui", I say this is wrong, just because GUI is simple and fast way to create triggers. My golden suggestion is to create simple stuff and most of the map events in GUI for simplyness, and create loops and systems in JASS for performance.


Click to expand...

GUI is not fast at all when creating triggers.

Most people on here type much faster in Jass than they can click everything needed in GUI. And the ones that can't will learn to very shortly.

Map events in GUI are inefficient and should not be used when using Jass.

The only thing that should ever be done in GUI is configurations. This means anything that is used in a spell or system that would make it easier for GUIers to understand.

jondrean said:

 Ah, finally  I shall teach you the secrets of effective GUI! Have you learned libraries, public functions and private functions? Good, because you'll need it a lot when mixing JASS with GUI.


Click to expand...

Actually you do not need libraries / structs / scopes at all when mixing GUI and Jass. Most of my systems are in Jass that mix with GUI to make it easy for GUIers and do not require vJass at all.

Also if this tutorial is about Jass it should stay about Jass and not cover vJass at all.

jondrean said:

 Simple spell example in JASS

Simple JASS/GUI system in JASS/GUI


Click to expand...

Just link to spells / jass/gui systems instead of making new ones. There are several out there. For Example: My spells are in Jass along with a vJass counterpart. Almost all of my systems are in Jass/GUI to allow for easy use of GUIers.

jondrean said:

 Fix possible mistakes I've made (post first, think later  )


Click to expand...

If you read the rules it says do not post until you think the tutorial is complete. So please follow that next time.

#### Deleted member 219079

D

#### Deleted member 219079

@edo494 Did the suggestions you posted, thanks for pointing them out  Just say if I missed any! Also added couple of things.

@deathismyfriend ;

 GUI is not fast at all when creating triggers.

Most people on here type much faster in Jass than they can click everything needed in GUI. And the ones that can't will learn to very shortly.

Map events in GUI are inefficient and should not be used when using Jass.

The only thing that should ever be done in GUI is configurations. This means anything that is used in a spell or system that would make it easier for GUIers to understand.


Click to expand...

This is meant for GUI users who want to learn JASS. I don't want to say that they can't use GUI anymore, after all I enjoy using GUI for simple triggers in my project

 Actually you do not need libraries / structs / scopes at all when mixing GUI and Jass. Most of my systems are in Jass that mix with GUI to make it easy for GUIers and do not require vJass at all.


Click to expand...

You're 100% right about this one, I fixed that sentence in the tutorial now

 Also if this tutorial is about Jass it should stay about Jass and not cover vJass at all.


Click to expand...

But I hate vanilla JASS... D:

 Just link to spells / jass/gui systems instead of making new ones. There are several out there. For Example: My spells are in Jass along with a vJass counterpart. Almost all of my systems are in Jass/GUI to allow for easy use of GUIers.


Click to expand...

No no, I want to be able to explain everything and use my own format of doing stuff. After all, the people who learn from this should be presented to JASS as a tool which makes things simple, I want to show it via simple system/spell or equivalent.

 If you read the rules it says do not post until you think the tutorial is complete. So please follow that next time.


Click to expand...

I read that post, if you're referring to this. I thought it would be good to get feedback as the tutorial is in development, but okay won't do so again ^^

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

jondrean said:

 But I hate vanilla JASS... D:


Click to expand...


You should write a vJass tutorial then not a Jass tutorial.

#### Deleted member 219079

D

#### Deleted member 219079

Is it enough if I change the title to

Simple JASS tutorial - for beginners --> Simple vJASS tutorial - for beginners

xD

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

jondrean said:

 Is it enough if I change the title to


Simple JASS tutorial - for beginners --> Simple vJASS tutorial - for beginners


xD


Click to expand...


I'm not sure that is up to the mods.


But It is just too simple for explaining Jass. It also would need to explain vJass a lot more.

#### Deleted member 219079

D

#### Deleted member 219079

That's why I have that disclaimer on top ^^ "NOT FINISHED YET"

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

jondrean said:

 That's why I have that disclaimer on top ^^ "NOT FINISHED YET"


Click to expand...


Yes and I said the above because of this statement you also made.


 jondrean said:



 but comments and heavy criticism are appreciated lol


Click to expand...

#### Deleted member 219079

D

#### Deleted member 219079

Oh okay

Thanks for feedback, will add more content tomorrow.

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

• Section 4's mastering Jass should be changed to mastering vJass

• Be careful when using FoG loops, while it has no problems on-demand filled groups (groups that are filled by enumerations within the local block where you perform the FoG iteration), it has issues with groups holding units for a period of time. See GroupUtils for details

• Your description of native functions are a bit off (minor).

native GroupEnumUnitsInRect takes group whichGroup, rect r, boolexpr is the declaration of the native function.

call GroupEnumUnitsInRect(parameters) is the native function itself.
Done! The tutorial is great, not much technical terms, newb friendly.


 Last edited: May 7, 2014

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

I'd say use Hidden tags or Tab tags to make navigating your tutorials easier..

#### Deleted member 219079

D

#### Deleted member 219079

@chobibo thanks for review, will fix those later today

@adiktuz I'm planning on adding pointer tags here and there ^^

Edit: fixes applied, made the last chapter, added pointers


 Last edited by a moderator: May 8, 2014

#### _apPie_

#### _apPie_

 Level 3

JoinedFeb 28, 2013Messages30

this will help newbies in jass like me

#### Garodar

#### Garodar

 Level 8

JoinedDec 11, 2012Messages412

Resources: 2

#### Resources
Maps 2

I'll take a deeper look into the guide later but from what I see, you've put a lot of effort into it. Good job.

#### Deleted member 219079

D

#### Deleted member 219079

Thanks, just tell me if you get lost on some section, so I can change or add info to it.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

 JASS stands for Just Another Script Syntax.


Click to expand...

[citation needed]. It was just a guess/rumor that was spread around the mapping community. There isn't any proof of it though.

 I say this is wrong, just because GUI is simple and fast way to create triggers. My golden suggestion is to create simple stuff and most of the map events in GUI for simplyness, and create loops and systems in JASS for performance. When I say using GUI, I mean using a hybrid between JASS and GUI, it's GUI which uses a lot of these:


Click to expand...


That feels a little off-topic. I would probably remove it. It seems more like you're forcing your agenda in this part. Ultimately, it is up to you, but it is best to keep everything relevant to the topic at hand: learning JASS!


After all, you have a section that later explains your technique. It is a bit weird to have that in your introduction.


---


"Our First JASS Trigger" starts off rather quickly. When someone converts a blank trigger, they'll end up with a trigger action and an initialization function. But you changed it to a more "modern" trigger style. However, the issue with it is that you end up jumping to a whole bunch of new concepts at once: locals, returning a value, nulling, natives, etc.


Generally, JASS tutorials are heavily dependent on structure. You must give simple examples to start off so that people get the concept down. From there, they can move on to creating "efficient" or "modern" JASS-type codes. See Ghan's tutorial: it starts off by explaining every aspect of an empty converted trigger. Or look at GhostWolf's, his starts off with an isolated function and goes through the process of explaining the entirety of what functions are about.


Even though we use combined conditions and actions in our current scripts, GUI users won't necessarily understand that right away. They might question: "why are our actions placed inside condition functions?" You have to make sure you explain that properly.

 vJASS syntax makes JASS more modifiable, this is why you'll need your own syntax checker for it.


Click to expand...


I'm not sure what you mean by "more modifiable". It'd probably be better said as "it adds additional features to JASS" (which is why it needs its own syntax checker).

 Drag the contents of jasshelper.7z\executable to vexorianjasshelper


Click to expand...


jassnewgenpack 2.0 already has the latest jasshelper, so you don't have to do any of that. That used to be a required step because the JNGP on wc3c had an older jasshelper, so people had to replace it with the latest jasshelper 0.A.2.B. 2 years ago, Mag uploaded the "standard" updated version to our tools section "JassNewGenPack 5d", and that problem has generally subsided since then. JNGP 2.0 also came out, which has the latest jasshelper, so that step isn't necessary (as long as they download it from our site or moyack's version).

 2.2 : Scopes and private functions


Click to expand...


You have to be sure to both show and explain. You showed scopes and an "initializer", but you didn't mention it in the text. You have to explain what scopes are used for and what initializers are. It is good that you explained encapsulation (private/public), but I might add a little more information on it. Right now, you show what causes an error, but you need to give it some context in order for it to make sense. For example:


"Normally, JASS won't allow you to have two functions with the same name. Scopes (and libraries) allow you to encapsulate them by adding the keyword "private" or "public". This will allow you to have common names such as "Act", "Init", etc. without worrying about colliding with other functions' names. However, those keywords have additional purposes: "private" will prevent the function from being called outside the scope. "public" will allow the function to be called outside the scope, but you must add the scope name as a prefix, followed by the underscore."


Then you could provide examples or whatever. That is a rough example of a thorough explanation. There is quite a bit of information that can be covered. (you did a good job of covering the "public" keyword though, so you don't need to change much of that)

 Simple, but makes coding, yet again, easier, as you no longer need to write functions again and again.


Click to expand...


It might be better if you explain a little bit more about how libraries are useful. This is how I always thought of it: in normal JASS, if you wanted to call a function from any trigger, you would have to place that function in the map's header. That can be inconvenient, unorganized, and it can clutter the map header really quickly. vJASS's libraries move those functions to the top of the script for you, allowing you to use them anywhere!


Then you could explain a bit more on what "requiring" does. It allows you to "order" libraries, so if you want to use a function from library "A" in library "B", you simply need to add "requires A" to library B's declaration.

 but then you can't define them at globals section


Click to expand...


Change the word "define" to "assign".

 2.5 : Structs


Click to expand...


This section suffers a bit from information overload. So many concepts are thrown out so rapidly, but the user won't know how to do anything with any of it. He will know what a struct looks like, but where should he use it? What is it used for? What are instances? What does it mean to overwrite the "create" method?


Structs are a rather large topic, sufficient for an entire tutorial. There is so much to them.

GetSpellAbilityId() == 'AHtc' this is a boolean, the real function here is;


Click to expand...


You should explain what rawcodes are. GUI users won't be familiar with it (most likely).

 4.2 : Group Loop


Click to expand...


You should go over loops in a much earlier section. Just the general syntax of looping. In GUI, you're familiar with loops (e.g. 1 to 10), but JASS behaves differently, particularly with the use of exitwhen. At this point, users might not know how to do a simple loop, so you should go over that first before showing a first of group loop.

 loop exitwhen true endif


Click to expand...


endif -> endloop


Also, in the first of group loop section, you should mention that it empties the group. If you want to preserve the units in the group, you should use ForGroup (in general). If you just need to perform an instant action over some group of units (where you don't need the units stored later), you can use the FirstOfGroup loop.

 4.3 : If / Then / Else


Click to expand...


You should show the syntax of elseif. It will be a new feature to GUI users, and since it has its own syntax, you should include it in the example.

 4.4 : Unit IDs


Click to expand...


It is good that you made a section for it. I think you should link to this section or write "see below" from the GetSpellAbilityId() == 'AHtc' line, otherwise readers might be confused.

 Naturally you need to change the line where you define your group to this --> call GroupEnumUnitsInRect(g, bj_mapInitialPlayableArea, function groupFilter)

Click to expand...


You should change the last part to Condition(function groupFilter). Yours won't cause an error in vJASS (because jasshelper will automatically wrap it in Condition() for you. it is a weird feature), but it will cause an error in regular JASS.


------


Ultimately, I agree with edo and death's reviews. You try to cover everything, but by being too ambitious with the topic, you end up having only a little information in each section. It is nice as a "light" tutorial, or an overview, but IMO this wouldn't help new JASSers much. It would serve as a reference for beginner JASSers so they can look back and see how to do things.


If I were you, I'd try to just stick with basic JASS or basic vJASS. Doing both is a ton of information.


Sorry if this review was a little harsh at times. As such, I want to let you know that this tutorial still has a lot of redeeming qualities. The layout of the tutorial is great and pretty. The code examples are pretty good. Sometimes I like that it is brief. It serves as a good reference to check back on how to use particular features. In fact, I used a similar tutorial when I was learning vJASS. The manual is rather dense and technical, so I just needed a quick overview so I could get the gist of things (it was this tutorial to be exact).


Still, this tutorial needs to decide what it wants to be. Does it want to be a reference? Does it want to be an in-depth tutorial? Does it want to cover JASS or vJASS (or both?)? Is it for moving on to JASS, or is it for mixing JASS with GUI? It is important for tutorials (and any writing in general) to have a clear focus. Being too broad and far-reaching is usually detrimental.

#### Deleted member 219079

D

#### Deleted member 219079

That wasn't too harsh at all ^^ if someone is to learn from this, it needs to be as good as possible

Atm I'm doing multiple projects (including the timelined zephyr contest entry) and have work in real life too so update might take a bit long, but please don't but this to graveyard in that time, I've put a lot of time to this.

And thanks for a big review

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

+1 for amazing layout and structure

however "for beginners" doesn't really suit in my opinion since the tutorial is a f*cking bible I don't know about you but structs are not simple, nor easy to learn so I don't why it is a section in the first place. Same with text macros.

edit: libs shouldn't be there either since they don't give any benefit for starters. I feel kinda the same way about scopes.

edit2: I think a tutorial for starters should include:

1. creating variables

2. calling functions

3. JNGP function list

4. if blocks

5. loops

you're done, with that knowledge you can start to make progress in a map, go and create a SPRPG so you wont have an issue with MUI.

edit3: wow, I sound so negative take it with a grain of salt xd


 Last edited: Jul 14, 2014

#### Deleted member 219079

D

#### Deleted member 219079

I replaced the GUI/JASS chapter with JASS efficiency tips chapter. Thanks to Malhorne for helping with it.

I've not yet fixed the previous chapter which pnf highlighted, it's hard to fix thing instead of just creating.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Good job, the length and content of this tutorial is incredible.

#### Deleted member 219079

D

#### Deleted member 219079

I'm glad you liked it

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

 About the Just Another Scripting Syntax thing, it is just a rumor. Whoever wrote that Wiki page didn't cite it. It was a joke that spread over the years, but there is no source. I'd rather not perpetuate that rumor.

So just change:

 JASS stands for Just Another Script Syntax. It's a scripting language for Warcraft III.


Click to expand...

To:

 JASS is a scripting language for Warcraft III.


Click to expand...

Or you can say something like "It is rumored to stand for Just Another Scripting Syntax".

 Now what these do is they release data from variables


Click to expand...

*Release data from objects. Or rather, it "removes memory related to an object". It doesn't actually deal with the variable, since variables are just pointers to objects in memory.

 Nice diagram.

 As a side note, jassers most often use conditions as their actions, just as we did, just to get that extra bit of speed.


Click to expand...

You might want to rephrase that a little. Maybe:

"As a side note, JASSers often put all their "Actions" into the condition function, instead of creating one function for a condition and one function for actions. It saves handles and is faster."

 this will outprint message lol.


Click to expand...

Add: "this will outprint message lol after 0 seconds of game-time has elapsed." It is a nice summary of the trigger as a whole.

 In the "Scopes" section, add a sentence to explain the keyword initializer.

 instance is an instance of a struct


Click to expand...

A zebra is a zebra. Maybe change it to something along the lines of: instance is an individual struct "object", created through .create() or .allocate().

 thistype = struct type name, in case you're wondering.


Click to expand...

A lot of people get confused about "thistype" when they're learning vJASS. Perhaps add a sentence saying that you can also write "myStruct". ("thistype" was originally intended for modules, but it is useful since it allows you to easily change the struct's name in one location).

 Perhaps in "Basics of JASS", you should add a section that teaches you how to look up a function using TESH's function list. Most people have to use it at some point to become familiar with the natives, and it really helps you find what you're looking for (or if you are converting GUI to JASS, it helps you find the native equivalents).

 In "Loops":

 loop exitwhen true endif endloop


Click to expand...

Remove the "endif". Also, your function GetStrength has a typo -> endfunciton should be endfunction

 In "Loops", you should add a sentence to explain "exitwhen". It is a new concept for GUI users.

 5.2 : Avoid many native calls


Click to expand...

This is a little bit of a weird tip. I think you should separate it into "Use arrays when you can" and "Avoid useless "wrapper" BJ's".

 Pre-calculating for loops - that is true, but the example you gave is a little too vague.

 5.7 : Use PUI


Click to expand...

PUI isn't for everyone. I think you should probably remove that section, or just change it to "Use Unit Indexers". But that is just fluff. I don't think you need to have that there.

 Vexorian's table doesn't use gamecache. That is the pre-1.23 version. The post 1.23 version uses hashtables. Bribe extended Vexorian's "Table" to integrate a struct interface for all the hashtable natives.

 I wouldn't recommend 5.9. It is just bad for beginners. Trying to make your own custom allocation is mostly useless (er.. struct extends array is very useful, but not in the way it is commonly used), and new JASSers won't understand it. It is okay to recommend Alloc, but IMO, it is better to just stick with regular allocation. If they find it to be insufficient, or if they are curious as to why so many resources have "struct extends array", then they can go and learn about custom allocation/deallocation.

Well, that is it for now. It is definitely getting better.  I suppose this could be nice as a general overview of things.

#### Deleted member 219079

D

#### Deleted member 219079

Those are good points, I'll update it when I've got time.

Avoid native calls is a good suggestion, but I should change it to "avoid native calls in looping functions". Ye it might sound weird tip but the thing is native calls have their impact.

 Well, that is it for now. It is definitely getting better.  I suppose this could be nice as a general overview of things.


Click to expand...

I'll never get this approved at this rate >_< I've got high school starting next week so the next update comes this week or after a long time ..

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nah, these are mostly the last stuff. My first review asked for a lot of revision and stuff. Since then, you've added more topics--so I had to add more to my second review. If you add more topics, it might require a 3rd review. But if you focus on fixing what I mentioned, it should be good to go!

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

if you never call native function, you never get to do anything. Also impact of natives is very small, you can even call wrapper with no visible inpact

#### Deleted member 219079

D

#### Deleted member 219079

 if you never call native function, you never get to do anything.


Click to expand...

"Native functions are super useful and without them you basically couldn't do anything visual or physical in-game."

 Also impact of natives is very small, you can even call wrapper with no visible inpact


Click to expand...

Ye it is. I just though that if someone is to learn from this tut, he'd better now all these little things told in ch 5.

But the last thing is so geeky I added optional tag to it. But I added it cuz by default structs do security checks on whether they've exceeded the array limit. Jass starter might not know this but now he does.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Haven't read all posts now, but I would wanna know if you're still caring for it?

It's not really "simple" jass tutorial. And also covers vjass. I think the goal should be made clear of what it is about, so you and user can more easy know what to expect.

Macros and other vjass tequniques are not really "basic" jass. There are some other things are could be changed or improved, but we first would need to know what you're currently thinking about it.

I would probably suggest to focus only on some points, which you think are very important.

#### Deleted member 219079

D

#### Deleted member 219079

This being from 2014, I would rather write it from scratch. But I don't have time. Maybe after I turn in my mini mapping submission.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Yes. I can also undersand wanting to rewrite, at least parts, from scratch. Take time you need and feel free to inform us about progress.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Hm, I still also share the thoughts of reqrite some parts, though approaches are pretty good.

But there's also covered very much info and some specified tequniques, that makes it just harder to maintain for you as it's pretty big.

But anyways, what's the current state? Are you working on it, or don't you care anymore?

#### Deleted member 219079

D

#### Deleted member 219079

This is something https://www.hiveworkshop.com/pastebin/0321a7f713fe638bfccb49202deb1f299624/

I have big exams in first half of March after which I have plenty of time. I will include visuals and stuff, to make it pleasant to read.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Ok, then I will graveyard it for now.

Ping us on update. By the way (from pastebin link), I think you should write more noob friendly if you want it to have a "Simple Jass Tutorial For Beginners". They also don't necessairly need to know about byte code, for example.

 You must log in or register to reply here.


### Similar threads

 Multi-unit Instanceability With Timers (vJASS)


busterkomo

Jul 28, 2009

Tutorial Archive

Replies8Views2K

Jan 18, 2010
Kwah

 Attack-damage detection engine (JASS)


paskovich

Apr 29, 2007

Tutorial Archive

Replies15Views17K

Nov 28, 2021
Dr Super Good

Locked

 Replacing GetUnitsSelectedAll inside condition?


Pinzu

Feb 4, 2019

Triggers & Scripts

Replies0Views1K

Feb 4, 2019
Pinzu

 Slightly different approach to H2I/H2S


Lord_BoNes

Sep 21, 2008

Tutorial Archive

Replies1Views2K

Sep 21, 2008
PurplePoot

 Check If Point Is Walkable


Dynasti

Dec 22, 2008

Tutorial Archive

Replies17Views3K

Apr 7, 2009
Pyritie

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top