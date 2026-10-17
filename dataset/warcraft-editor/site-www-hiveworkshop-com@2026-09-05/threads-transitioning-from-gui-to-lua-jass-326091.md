---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/transitioning-from-gui-to-lua-jass.326091/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Transitioning from GUI to Lua JASS
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/transitioning-from-gui-to-lua-jass.326091/>
#### Ouroel

#### Ouroel

 Level 3

JoinedJul 12, 2020Messages12

Resources: 1

#### Resources
Tutorials 1

Table of Contents

Prerequisites

Introduction

Why should I use JASS/Lua

How do I switch from JASS to Lua?

What do I type?

Triggers

Actions

Events

Conditions

Operators

Loops

Variables

Extras - Lua Hashtables

Extras - Compartmentalization

Prerequisites

Knowing how to use GUI.

Introduction

Sick of selecting everything from a drop-box? Want to code like a real coder (tm)? Look no further than JASS/Lua!

This guide serves two purposes: to illustrate why you should transition to Lua/JASS, and how to easily do so.

This is not a guide on how to be a super coder or something like that. There are a thousand other guides on how to do stuff like that. Besides, you should already know some basic programming concepts like if/else and loops, given that they are literally in GUI.

You will not require any programming background aside from your experience tinkering with Warcraft III GUI Triggering.

Why should I use JASS/Lua?

Say you want to make a bleed stat which scales off your technology transfer stat, both of which does not exist in Warcraft III. By using JASS/Lua, you can easily hook up some update function which updates your technology transfer stat while simultaneously updating your bleed stat. This allows you to just type the function name instead of copy-pasting the actions every single time.

Also, something about typing speed, efficiency, lag, 32768 array limits, BJs, etc.

You just look muh cool and professional while typing in your code like a real man, okay? Also, now, you too can join the JASS/Lua club!

How do I switch from JASS to Lua?

What do I type?

 There are three answers to this:

1). Use an API lol git gud

2). Do it in a new trigger in GUI, right click your trigger, click "Convert to Custom Script". Unavailable to LUA users.

3). Type "ASD" in anywhere you can enter custom script, and scour the error message for the code.

Triggers

Triggers are literally just functions that get called at certain times. I'll teach you how to give it an event later.

Functions are basically just Triggers, but with just things you can put in "Actions", may take in information to process, and may give you an output.

To create a function,

 Lua:

function YOUR_FUNCTION (whatever)
--put actions here
--example: whatever = whatever + 2
--example: DisplayTextToPlayer(Player(0), 0, 0, I2S(whatever))
--maybe put "return X" here, replace X with whatever variable you want to return. NOT FOR TRIGGERS!
--example: return whatever
end

 vJASS:

function YOUR_FUNCTION takes whatever returns type //usually "whatever" is nothing, but it really depends. "type" can be any type.
//put actions here
//example: whatever = whatever + 2
//example: call DisplayTextToPlayer(Player(0), 0, 0, I2S(whatever))
//maybe put "return X" here, replace X with whatever variable you want to return. NOT FOR TRIGGERS!
//example: return whatever
end

 To use (call) a function,

 Lua:

YOUR_FUNCTION (whatever) --I just wanna call a function. You don't actually have to put something in whatever

local VARIABLE = YOUR_FUNCTION(whatever) --I want something out of the function because the function is how I find out the square root of two pi x times the player's score and the height of his character times the number of trees or whatever

 vJASS:

call YOUR_FUNCTION (whatever) //I just wanna call a function. You don't actually have to put something in whatever

set tree = YOUR_FUNCTION (whatever) //I want something out of the function because the function is how I find out the square root of two pi x times the player's score and the height of his character times the number of trees or whatever

Actions

 Actions are literally function calls. Don't believe me? Right-click your trigger, click "Convert to Custom Script". You will get a function call.

What does a function call look like?

function_name(args) or function_name()

(will have a call in front if you are using JASS)

That's literally your action. Just a function call.

Example: Melee Initialization actions:

 Lua:

function Trig_Melee_Initialization_Actions()

 MeleeStartingVisibility()
 MeleeStartingHeroLimit()
 MeleeGrantHeroItems()
 MeleeStartingResources()
 MeleeClearExcessUnits()
 MeleeStartingUnits()
 MeleeStartingAI()
 MeleeInitVictoryDefeat()
end

 vJASS:

function Trig_Melee_Initialization_Actions takes nothing returns nothing

 call MeleeStartingVisibility()
 call MeleeStartingHeroLimit()
 call MeleeGrantHeroItems()
 call MeleeStartingResources()
 call MeleeClearExcessUnits()
 call MeleeStartingUnits()
 call MeleeStartingAI()
 call MeleeInitVictoryDefeat()
endfunction

 As you can see, they are all function calls.

Events

 For Map Initialization, just do it in GUI and use Custom Scripts to call functions lol, I'm not going to teach you how to metatable and detect changes to a variable.

For literally everything else,

1). Create a Trigger

2). Call a function that adds an event to the trigger

3). Link it to function of choice

 Lua:

function InitTrig_everyxsecs()
 local gg_trg_everyxsecs = CreateTrigger() -- Create a Trigger
 TriggerRegisterTimerEventPeriodic(gg_trg_everyxsecs, 2) -- Call a function that adds an event to the trigger. In this case it's the Time - Periodic Event event.
 TriggerAddAction(gg_trg_everyxsecs, Trig_everyxsecs_Actions) --Link it to function of choice
end

function Trig_everyxsecs_Actions()
 -- Go back to Actions to find out how to put actions in here
end

InitTrig_everyxsecs() -- Create the trigger

 vJASS:

function InitTrig_everyxsecs takes nothing returns nothing
 local trigger gg_trg_everyxsecs = CreateTrigger() // Create a Trigger
 call TriggerRegisterTimerEventPeriodic(gg_trg_everyxsecs, 2) // Call a function that adds an event to the trigger. In this case it's the Time - Periodic Event event.
 call TriggerAddAction(gg_trg_everyxsecs, Trig_everyxsecs_Actions) // Link it to function of choice
end

function Trig_everyxsecs_Actions takes nothing returns nothing
 // Go back to Actions to find out how to put actions in here
end

call InitTrig_everyxsecs () // Create the trigger

Conditions

Conditions in Lua/JASS are, well, if-conditions. Except that you put them in Actions and you put the Actions inside them. Applies to If-Then-Else (Multiple Conditions)

 Lua:

if (CONDITION) then
 --Actions
elseif (CONDITION) then --if you don't have other actions don't type this
 --Other actions
else --if you don't have other actions don't type this
 --Other actions 2
end

 vJASS:

if CONDITION then
 //Actions
elseif CONDITION then //if you don't have other actions don't type this
 //Other actions 2
else //if you don't have other actions don't type this
 //Other actions 2
endif

Operators

 Now, you might be wondering: I've looked up the API and can't find an "equals" function! How do I do basic additions and comparisons? Where is the "Equals To" analog?

 Lua:

--[[ Conditions ]]--

== -- Equals To
~= -- Not Equals To
<= -- Less Than or Equals To
>= -- Greater Than or Equals To
< -- Less Than
> -- Greater Than Than

--[[ Actions ]]--

= -- Set Variable
+ -- +
- -- -
* -- *
/ -- /
% -- modulo
^ -- power

 vJASS:

/* Conditions */

== // Equals To
!= // Not Equals To
<= // Less Than or Equals To
>= // Greater Than or Equals To
< // Less Than
> // Greater Than Than

/* Actions */

= // Set Variable
+ // +
- // -
* // *
/ // /
+= // add to self
-= // subtract from self
*= // multiply self
/= // divide self

Loops

Loops are basically For Each Integer A from a to b, do (Actions).

 Lua:

for A=a,b do
 -- actions
end

 vJASS:

local integer A = a
loop
 exitwhen A > b
 // actions
 set i = i + 1
endloop

BONUS: While Loops, while not found in GUI, will repeat itself until the condition is fulfilled.

 Lua:

while CONDITION do
 --actions
end

 vJASS:

loop
 exitwhen CONDITION
 // actions
endloop

Variables

The Variables in GUI are Global Variables - that means everything can access them. Not very good if you just want to run 3 spells at once and they all rely on the same actions and triggers and such.

Here is how you make a Global Variable:

 Lua:

VARIABLE_NAME = whatever --put outside functions!

 vJASS:

globals
 type Your variable = whatever --Go search up the API for types!
endglobals

 Now, here is how you add a local variable!

 Lua:

local VARIABLE_NAME = whatever --put inside functions!

 vJASS:

local type Your variable = whatever //Go search up the API for types!

 Here is how you add an array:

 Lua:

local VARIABLE_NAME = {} -- can be global too

 vJASS:

local type array Your variable = whatever //Go search up the API for types!

Congratulations, that should be literally everything that you can do in GUI!

Extras - Lua Hashtables

Lua arrays can have their own variables, including arrays. This allows you to store your data in data. For example:

 Lua:

local VARIABLE_NAME = {
 variable = a, --no need local, it's already specific to this
 variable2 = { b = 100 }, --no need local, it's already specific to this
}

 To access the nested variable,

 Lua:

VARIABLE_NAME.variable2.b --100
VARIABLE_NAME.variable --whatever a is
VARIABLE_NAME --hashtable containing variable and variable2[/LEFT]
VARIABLE_NAME["variable2"]["b"] --100

asd = "b"
VARIABLE_NAME["variable2"][asd] --100

 Additionally, you can set the variable name of Lua table variables to whatever you want - even a literal unit. Not a string, mind you, a literal unit.

 Lua:

Health = {}
Health[Player(0)] = 2
Health[GetEventTargetUnit()] = 10

Extras - Compartmentalization

So, remember what I said about being able to add stats to whatever unit and have it auto-update?

For this example, I will only be using LUA, because with LUA it is pretty brain-dead to do it. JASS, being a data-oriented programming language, requires far more skill to achieve the same goal.

There are better ways to do it, such as with metatables, but I will only use things I taught in this tutorial.

 Lua:

my_stats = {}

function init_stats(unit)
 if not my_stats[unit] then --check if exists
 my_stats[unit] = { --create the stats for the unit
 bleed = 0,
 tech_transfer = 0,
 }
 end
end

function updateTechTransfer(unit, newValue) --call when updating stat to new value, like if you get armor or something
 init_stats(unit) --to init custom data if nonexistent
 my_stats[unit].tech_transfer = newValue --the set
 calculateBleed(unit) --the update
end

function calculateBleed(unit) --calculation
 local unit_stats = my_stats[unit]
 unit_stats.bleed = unit_stats.tech_transfer ^ 3
end

Now you can do whatever you want with it. To prove that it works, I'm going to use a trigger every X seconds mockup to test this:

everyxsecs
Events
Time - Every 2.00 seconds of game time

Conditions

Actions
Custom script: updateTechTransfer(Player(0), 10)

Custom script: DisplayTextToPlayer(Player(0),0,0,my_stats[Player(0)].bleed)


 Last edited: Jul 26, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I find that centered texts are more difficult to read (exception being titles) but that's easily fixed.

To avoid duplicate code entries, perhaps use hidden tags for each version or if you want to be fancy, a table with two columns to be more space effective

A more core 'problem' though; there are already tutorials similar to this.

Beginning JASS Tutorial Series

Learning JASS

JASS: A Concise Introduction

Crash Course

I am 99% sure there are more of them that I missed.

I am not sure if I want to graveyard it because of that.. will have to take another look later I think.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Decided to graveyard this two two reasons.

1. Other tutorials exist that cover this topic already

2. I don't think it covers enough, especially for lua

 You must log in or register to reply here.


### Similar threads

 Magic of JASS [Reviewed: Ghan_04]


forzamoto

Jun 27, 2008

Tutorial Archive

Replies6Views2K

Jul 18, 2008
Ralle

 Quickstart Guide to Installing Lua Scripts (easy)


Nestharus

May 17, 2011

Tutorial Archive

Replies4Views15K

Dec 7, 2013
PurgeandFire

 Local Variables


jonadrian619

May 19, 2007

Tutorial Archive

Replies7Views28K

Jun 27, 2007
Wolverabid

Locked

[Lua]
 How to convert GetUnitGoldCost to LUA?


Bloodheaven_

Oct 19, 2023

Triggers & Scripts

Replies10Views2K

Oct 20, 2023
HerlySQR

 Attack-damage detection engine (JASS)


paskovich

Apr 29, 2007

Tutorial Archive

Replies15Views17K

Nov 28, 2021
Dr Super Good

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