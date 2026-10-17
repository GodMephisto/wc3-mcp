---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/a-comprehensive-guide-to-code-and-trigger-optimization.353229/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# A Comprehensive Guide to Code and Trigger Optimization
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/a-comprehensive-guide-to-code-and-trigger-optimization.353229/>
#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

I've searched for similar topics here in the tutorial forum, but all I could find were threads which only focused on one singular, minor aspect of optimization. So I thought it would be a good idea to make a tutorial that condenses all the important ways in which you can improve the performance of your code or triggers.

The tutorial is divided into two parts: Basics and Advanced. Not following the basics can really break the performance of your map, while the tips in advanced will make your map perform just a little more smoothly. For many of them, there's not really a reason not to follow them, so even if the difference isn't as great, it's good practice anyway. I'll be using GUI examples in the Basic section and JASS examples in the Advanced section.

Since I'm covering a lot of different topics, I won't go into any of them in a lot of detail, but I'll provide links to other tutorials on those subjects when possible. Please suggest good tutorials if you know any! If you can think of any tips that I've missed, please also post them in the comments!

Everything I make claims about in this tutorial has been tested. Optimization has been a hobbyhorse of mine for quite a while now. Trying to build complex engines in a language as painfully slow as JASS is akin to training at 300 times Earth's gravity, and after returning to normal gravity by switching to Lua, I've finished building my highly optimized interaction engine. Of course there could still be errors, so if you find any, please post them in the comments.

Content

Basics

Remove Memory Leaks​

Set your language to Lua​

Identify Bottlenecks​

Use specific unit events instead of generic ones​

Avoid group filter functions and ForGroup​

Store data in a smart way​

Use SetUnitX instead of SetUnitPosition​

Advanced

Arrange the terms in your conditions​

Precalculate repeating expressions​

Avoid using Pow​

Compare the square of distances​

Use Lua libraries instead of natives​

Inline functions​

Basics

Remove Memory Leaks

Leaked handles not only consume memory, but they also slow down the game. If you have a considerable amount of memory leaks in your map, you should go here and not concern yourself with any of the tips in this tutorial until you have eliminated them.

Set your language to Lua

As a GUI user, you have the option to either use JASS or Lua as your scripting language, with JASS being the default. If you switch to Lua, not much will change for you except that upon saving your map, your map script will be transpiled to Lua, which is considerably faster than JASS (about 10x). There is the caveat that, if you choose to use external resources, you're restricted to those written in Lua. However, many creators provide both JASS and Lua versions these days or might be willing to provide a Lua translation upon request. You have to make the decision if this increase in performance is worth the downside.

To switch your language to Lua, go to Scenario -> Map Options, then set the Script Language to Lua. Your map script must be empty or the field will be disabled. If you already have triggers in your map, make a backup of your map, export your triggers in the Trigger Editor, delete all your triggers, switch the language to Lua, then reimport your triggers from your exported file.

The switch will only work as long as you have no JASS code imported in your map already. You will also have to edit any custom script lines by removing the "call". This might be the most annoying part.

WARNING: There are some issues with the Lua/GUI implementation and, due to the lack of support from Blizzard, it means that any problems Lua has will probably never get fixed. However, there are resources to help with some of the issues, such as Lua infused GUI.

Identify Bottlenecks

When optimizing your map, it is important to identify the parts of your map script that are the major strains on performance. There is no point in optimizing parts of your script that get executed only once or only a few times. The major bottlenecks, the parts which we have to optimize, are those that are run continuously in short intervals and/or run loops over a large number of objects. In most maps, those sections are few and far between, whereas most of the script doesn't really matter when it comes to optimization.

Use specific unit events instead of generic ones

One way to create a new spell for a hero is to create a trigger that fires on any spell cast, then check if the spell is the spell we've just created.

OurSpellTrigger
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to OurNewSpell

Actions
-------- Spell Actions --------

The problem here is that, as we add more and more heroes, each of those triggers will fire everytime anyone casts a spell anywhere. In a map with 100+ heroes, this could easily be ~500 triggers firing each time. A better way to set up your spell triggers is to give them no event, but add the event when the hero that uses the spell is being selected by adding the line:

Trigger - Add to OurSpellTrigger <gen> the event (Unit - SelectedHero Starts the effect of an ability)
Avoid group filter functions and ForGroup

For whatever reason, the natural way of handling unit groups is incredibly slow and there is a way that is better in both GUI and JASS/Lua. Let's say we want to kill all undead units in 500 range of a point. In GUI, this would look like:

Unit Group - Pick every unit in (Units within 500.00 of myLocation matching ((((Matching unit) is Undead) Equal to True) and (((Matching unit) is alive) Equal to True)).) and do (Actions)
Loop - Actions
Unit - Kill (Picked unit)

But, because filter functions are slow, this is faster, and as a bonus, also more readable when the conditions become long:

Unit Group - Pick every unit in (Units within 500.00 of myLocation and do (Actions)
Loop - Actions
Set VariableSet tempUnit = (Picked unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(tempUnit is Undead) Equal to True

(tempUnit is alive) Equal to True

Then - Actions
Unit - Kill tempUnit

Else - Actions

Unfortunately, in GUI, there is no way to avoid the ForGroup call, but in JASS we can. The most performant way to write this in JASS is:

 JASS:

local group G = CreateGroup()
local integer i = 0
local unit u
call GroupEnumUnitsInRange(G, x, y, 500, null)
loop
 set u = BlzGroupUnitAt(G, i)
 exitwhen u == null
 if UnitAlive(u) and IsUnitType(u, UNIT_TYPE_UNDEAD) then
 call KillUnit(u)
 endif
 set i = i + 1
endloop

Store data in a smart way

Let's say we want to create a disease represented by a debuff. Each unit that has the debuff has a chance to spread it to another nearby unit that doesn't have the debuff. However, each unit that already had the disease is immune and can't get affected by it again.

Here's how past-me would have coded this:

ApplyDisease
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Set VariableSet tempunitgroup = (Units in (Playable map area) matching (((Matching unit) has buff Spreading Disease) Equal to True))

Unit Group - Pick every unit in tempunitgroup and do (Actions)
Loop - Actions
Set VariableSet temppoint = (Position of (Picked unit))

Set VariableSet diseasegroup = (Units within 300.00 of temppoint matching (((Matching unit) has buff Spreading Disease) Equal to False) and ((Matching unit) is in immunegroup.) Equal to False.)

Unit Group - Pick every unit in diseasegroup and do (Actions)
Loop - Actions
Unit Group - Add (Picked unit) to immunegroup

-------- Apply Buff --------

There's so much wrong here. First of all, I'm enumerating all units on the entire map with a filter function. That alone will probably be a small lag spike every time this trigger is executed. Because we apply the buff only within our trigger, we can register each time a new unit gets the buff and add that unit to a list of all units that have it. That way, we only have to loop over all units of which we know they have the buff instead of all units in the entire map. Just make sure to remove the unit from the list when it loses the buff!

The second problem here is that for every unit to which the disease could spread, the game has to to search through a unit group that could grow pretty large just to see if it is immune or not. This could be solved in a much better way with hashtables by checking if a flag for the unit we want to check is set to true.

For a tutorial on hashtables, see here.

Here is the improved version, which should be at least 10x faster:

Apply Disease
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in diseasegroup and do (Actions)
Loop - Actions
Set VariableSet tempunit = (Picked unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(tempunit has buff Spreading Disease) Equal to True

Then - Actions
Set VariableSet temppoint = (Position of tempunit)

Set VariableSet tempunitgroup = (Units within 300.00 of temppoint.)

Unit Group - Pick every unit in tempunitgroup and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Load isImmune of (Key (Picked unit).) from myHashtable.) Equal to False

Then - Actions
Hashtable - Save True as isImmune of (Key (Picked unit).) in myHashtable.

-------- Apply Buff --------

Else - Actions

Custom script: call DestroyGroup(udg_tempunitgroup)

Custom script: call RemoveLocation(udg_temppoint)

Else - Actions
Unit Group - Remove tempunit from diseasegroup.

When writing your code, try to find if you're asking the computer the same question over and over again and if there's a way to fix that. In the example I gave above, I'm asking the computer every second, for every unit on the map, if it has gained the disease buff within the last second.

Use SetUnitX/Y instead of SetUnitPosition

There are two ways to move units, one by using SetUnitX followed by SetUnitY, and the other is with SetUnitPosition. SetUnitPosition is much slower because it checks the unit's pathing and collision, while SetUnitX/Y does not. If you're sliding a lot of units across ice or applying a knockback etc. using SetUnitPosition, it might cause significant lag.

Unfortunately, SetUnitX/Y do not exist in GUI, so you'd have to use custom script. Instead of

Unit - Move myUnit instantly to myLocation.
do

Custom script: call SetUnitX(udg_myUnit, GetLocationX(udg_myLocation))

Custom script: call SetUnitY(udg_myUnit, GetLocationY(udg_myLocation))

Advanced

Arrange the terms in your conditions

Conditions in both JASS and Lua are short-circuit, which means that, if by evaluating one of the terms in the condition, the result is already determined, the rest of the terms are skipped. For example, in a condition "A or B", if A is evaluated to be true, B is not evaluated because the result of the condition is already known. This means that, to improve performance, you should put the terms that most likely are to determine the outcome of the condition first so that the rest can be skipped.

In a statement "A and B", put the condition that most often will return false first.

In a statement "A or B", put the condition that most often will return true first.
An exception would be if the other term is much faster to evaluate.

Precalculate repeating expressions

When you're repeatedly using the same expression in a trigger, it might make sense to store the result in a variable first. This is especially true if the expression involves function calls.

 JASS:

//slow
local integer i = 1
loop
 exitwhen i > 10
 call AddSpecialEffect("effectPath.mdx", GetUnitX(myUnit) + 50*i*Cos(myAngle), GetUnitY(myUnit) + 50*i*Sin(myAngle))
 set i = i + 1
endloop

//fast
local integer i = 1
local real x = GetUnitX(myUnit)
local real y = GetUnitY(myUnit)
local real cosAngle = Cos(myAngle)
local real sinAngle = Sin(myAngle)
loop
 exitwhen i > 10
 call AddSpecialEffect("effectPath.mdx", x + 50*i*cosAngle, y + 50*i*sinAngle)
 set i = i + 1
endloop

The same is true for array variables. In JASS, if you're accessing an array variable more than four times within your code, it is faster to save it to a regular variable first. In Lua, the break-even point is 3.

Avoid using Pow

In JASS and GUI, Pow is a slow function and for squaring or cubing a variable, writing it out explicitly is always faster.

 JASS:

local real x = 1.25
local real xSquared1 = Pow(x, 2) //slow
local real xSquared2 = x*x //fast

In Lua, the difference between x^2 and x*x is minimal.

Compare the square of distances

We all know the formula for calculating the distance between two objects. The fastest way to calculate it is:

 JASS:

local real dx = object1.x - object2.x
local real dy = object1.y - object2.y
local real distance = SquareRoot(dx*dx + dy*dy)

However, often we don't need to know the exact value of the distance, but only if it is smaller or greater than a constant threshold value, such as when we want to check if something is in range. In these comparisons, it is completely sufficient and more performant to compare the square of the distance with the square of the threshold value. This is more performant because SquareRoot is a slow function.

 JASS:

local real dx = object1.x - object2.x
local real dy = object1.y - object2.y
if dx*dx + dy*dy < THRESHOLD*THRESHOLD then
 //Do stuff
endif

Use Lua libraries instead of natives

When coding in Lua, for many functions, you have the option to either use the Lua incorporated function or the Warcraft 3 native. In most situations, the Lua function will be faster. For example, math.cos is about 5x faster than the Cos native. One of the exceptions is math.random, which is internally replaced with GetRandomReal/GetRandomInt.

In general, a native call and a JASS function call are about as fast, while in Lua, a Lua function call is significantly faster than a native call.

Inline functions

I'm going to end this tutorial with a somewhat controversial one: A way to increase performance is to stop making everything into its own function! Having the various parts of your systems buried under a cascade of function calls does not only make your code less readable, in a slow language like JASS, it is also a recipe for bad performance.

 vJASS:

//This is making the code slower for no reason.
method Move takes nothing returns nothing
 this.x = this.x + this.velocityX
 this.y = this.y + this.velocityY
endmethod

static method MoveMissiles takes nothing returns nothing
 local integer i = 1
 loop
 exitwhen i > numMissiles
 call missile[i].Move() //Just inline it!
 set i = i + 1
 endloop
endmethod

Some of the functions that might get overlooked when it comes to inlining are RAbsBJ, RMaxBJ etc. Inlining these in parts of your script that are performance bottlenecks can also help significantly.

Since function calls are ~20x faster in Lua, this is only really an issue for JASS.


 Last edited: Jul 14, 2024

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Will give this a proper read later but from a glance the obvious question is how much this matters? is there any numbers or ballpark guesses?

For example maybe I would not be willing to convert my map to lua for a 5% performance benefit but I would for 20%. It might not be that simple but just to put advice in perspective I think it's quite key.

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

In Lua, conditions, loops, function calls, and arithmetic are about 20 times faster than they are in JASS, while native calls, so anything from Create Unit to Pick Every Unit in X etc., are about the same speed. This averages out to about a 5-10x speed increase in Lua, depending on the type of map.

But, like I mentioned in that section, Lua GUI has a few issues. Some of them are being alleviated by resources here on hive. Maybe @Uncle and @Bribe can chime in if suggesting Lua GUI is good advice at this point.

I can add some more lines about the rough ballparks of the benefits. In general, it matters the most for complex maps where there's a lot going on, such as AoS maps.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I wouldn't say it's a 'must'

For me I cannot make an informed decision unless I know how big the difference, after all there is a time to payoff cost to everything.

That said I think ballpark is the keyword here, a useful footnote should be plenty it should not be the focus.

I think I read wurst having a lua mode so might as well turn that on, 0 downside if it's considered stable.

 You must log in or register to reply here.


### Similar threads

 The Importance of Efficiency in Coding


Septimus

Mar 11, 2009

Misc/General Mapping Tutorials

Replies21Views11K

Sep 24, 2009
Dreadnought[dA]

 Ralle's Tower Defence Guide


Ralle

Aug 12, 2006

Misc/General Mapping Tutorials

Replies15Views27K

Jun 24, 2007
Horn

 A comprehensive guide to Mapping in Lua


Eikonium

Sep 18, 2022

Lua Tutorials
2

Replies81Views37K

Apr 19, 2026
KuWe

 Custom XP System : From Preparations to Creation


Vladadamm

Jan 6, 2013

Misc/General Mapping Tutorials

Replies15Views13K

Mar 3, 2018
Rheiko

 Complete list of things that leak


busterkomo

May 8, 2009

Misc/General Mapping Tutorials

Replies21Views28K

May 26, 2015
SAUS

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top