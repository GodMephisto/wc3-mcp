---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/convenient-unit-group-filtering-in-gui.278412/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Convenient Unit Group Filtering in GUI
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/convenient-unit-group-filtering-in-gui.278412/>
#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Convenient Unit Group Filtering in GUI

Introduction

Common Structure

Recomended Structure

Conclusion

## Introduction

This short tutorial will show you the best and most efficient way to filter out units enumerated from a unit group function. It will not show you what usage the function offers (excluding the example trigger), nor how to use them. With that in mind, I recommend that you have moderate knowledge and experience with the trigger editor before reading on.​

The unit group function is probably one of the most useful functions in the Trigger Editor. A handful of the submitted spells and systems would simply not exist without it. Being an extremely common function, I can guarantee that any trigger that involves the manipulation of multiple units will use it.​

NOTE: The example triggers you will see throughout this tutorial will all achieve to do the same thing: deal 100 damage to enemy units, that are alive, within 300 range of the triggering unit.​

## Common Structure

As someone who lurks the Triggers & Scripts and World Editor Help Zone forums, I see a lot of people filtering units for a unit group like this:

Set Caster = (Triggering unit)

Set Player = (Triggering player)

Set CasterLoc = (Position of Caster)

-------- --------

Set UnitGroup = (Units within 300.00 of CasterLoc matching ((((Matching unit) is alive) Equal to True) and (((Matching unit) belongs to an enemy of Player) Equal to True)))

Unit Group - Pick every unit in UnitGroup and do (Actions)
Loop - Actions
Set TempUnit = (Picked unit)

Unit - Cause Caster to damage TempUnit, dealing 100.00 damage of attack type Spells and damage type Normal

Custom script: call DestroyGroup(udg_UnitGroup)

Custom script: call RemoveLocation(udg_CasterLoc)

Set Caster = (Triggering unit)

Set Player = (Triggering player)

Set CasterLoc = (Position of Caster)

-------- --------

Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick every unit in (Units within 300.00 of CasterLoc matching ((((Matching unit) is alive) Equal to True) and (((Matching unit) belongs to an enemy of Player) Equal to True))) and do (Actions)
Loop - Actions
Set TempUnit = (Picked unit)

Unit - Cause Caster to damage TempUnit, dealing 100.00 damage of attack type Spells and damage type Normal

Custom script: call RemoveLocation(udg_CasterLoc)

Both will achieve the same thing, as explained in the introduction. All though there is nothing entirely wrong with it, there are a few issues that I

personally have with it:

(Matching unit) will constantly call the function GetFilterUnit(), which means that the execution will be slower.

They can get excessively long when you have multiple filters (ex: magic immune, not a structure), making it a hassle to read, and when copy-pasted as text it will actually cut off at a certain point:

Set UnitGroup = (Units within 300.00 of CasterLoc matching (((((Matching unit) is A structure) Equal to False) and (((Matching unit) is Magic Immune) Equal to False)) and ((((Matching unit) is alive) Equal to True) and (((Matching unit) belongs to an enemy of Player) Equal t

It is a complete pain in the a** to add, change, and or remove filters:

## Recommended Structure

If that GIF traumatized you from ever using unit groups again, fret not, for now I will show a much better way of doing it. Instead of using the (Matching unit) function, use an If/Then/Else, Multiple Actions function that directly compares the (Picked unit) to a filter:

Set Caster = (Triggering unit)

Set Player = (Triggering player)

Set CasterLoc = (Position of Caster)

-------- --------

Set UnitGroup = (Units within 300.00 of CasterLoc)

Unit Group - Pick every unit in UnitGroup and do (Actions)
Loop - Actions
Set TempUnit = (Picked unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(TempUnit is alive) Equal to True

(TempUnit belongs to an enemy of Player) Equal to True

Then - Actions
Unit - Cause Caster to damage TempUnit, dealing 100.00 damage of attack type Spells and damage type Normal

Else - Actions

Custom script: call DestroyGroup(udg_UnitGroup)

Custom script: call RemoveLocation(udg_CasterLoc)

Set Caster = (Triggering unit)

Set Player = (Triggering player)

Set CasterLoc = (Position of Caster)

-------- --------

Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick every unit in (Units within 300.00 of CasterLoc) and do (Actions)
Loop - Actions
Set TempUnit = (Picked unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(TempUnit is alive) Equal to True

(TempUnit belongs to an enemy of Player) Equal to True

Then - Actions
Unit - Cause Caster to damage TempUnit, dealing 100.00 damage of attack type Spells and damage type Normal

Else - Actions

Custom script: call RemoveLocation(udg_CasterLoc)

## Conclusion

Voila! Your unit group loop is now efficient, extremely easy to read, and requires little to no effort when you want to add and or remove filters. You also get the added benefit of me not scolding you in the Triggers & Scripts or Spell Section!

​


 Last edited: Jan 9, 2018

#### Meatmuffin

#### Meatmuffin

 Level 11

JoinedJul 25, 2014Messages490

Resources: 10

#### Resources
Maps 2Spells and Systems 7Tutorials 1

A really simple tutorial, but I see this done by amateur users all the time and it

does make me cringe as well. Good job, would have never thought

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

That gif really did traumatize. =P

And nice tutorial, nice work.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

That is definitely more convenient to configure.

Approved.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

A proper structure is indeed very useful.

#### A]mun

#### A]mun

Patron

 Level 27

JoinedDec 4, 2007Messages1,821

Good to know, just something:

Isn't it advisable, to use (matching unit) as a first and quick overall outsourcing, especially in multiple if/then/else filters?

It would seem rather odd to use if/then/else clauses exlusively, always adding (TempUnit is alive) Equal to True, instead of outsourcing it once via matching unit?!

Would like some insight on this.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1


 A]mun;2833529 said:



 Isn't it advisable, to use (matching unit) as a first and quick overall outsourcing, especially in multiple if/then/else filters?

It would seem rather odd to use if/then/else clauses exlusively, always adding (TempUnit is alive) Equal to True, instead of outsourcing it once via matching unit?!


Click to expand...

"Outsourcing" doesn't really make any sense. Whether or not you use an If/Then/Else with (Picked unit) or (Matching unit), they will all be checked the same exact way. Every single unit enumerated by the unit group will all be checked to each filter you have one at a time. There is no outsourcing.

This is what the JASS code will look like when the GUI is compiled:

Set TempLoc = (Center of (Playable map area))

Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick every unit in (Units within 100.00 of TempLoc matching ((((Matching unit) is A structure) Equal to True) and (((Matching unit) is alive) Equal to True))) and do (Actions)
Loop - Actions
-------- do stuff --------

Custom script: call RemoveLocation(udg_TempLoc)

 JASS:

//Using (Matching unit)
function Filter1 takes nothing returns boolean
 return ( IsUnitType(GetFilterUnit(), UNIT_TYPE_STRUCTURE) == true )
endfunction

function Filter2 takes nothing returns boolean
 return ( IsUnitAliveBJ(GetFilterUnit()) == true )
endfunction

function FilterResult takes nothing returns boolean
 return GetBooleanAnd( Filter1(), Filter2() )
endfunction

function UnitPassesFilters takes nothing returns nothing
 // do stuff
endfunction

function Actions takes nothing returns nothing
 set udg_TempLoc = GetRectCenter(GetPlayableMapRect())

 set bj_wantDestroyGroup = true
 call ForGroupBJ( GetUnitsInRangeOfLocMatching(100.00, udg_TempLoc, Condition(function Filter1)), function Filter2 )
 call RemoveLocation(udg_TempLoc)
endfunction

Set TempLoc = (Center of (Playable map area))

Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick every unit in (Units within 100.00 of TempLoc) and do (Actions)
Loop - Actions
Set TempUnit = (Picked unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(TempUnit is A structure) Equal to True

(TempUnit is alive) Equal to True

Then - Actions
-------- do stuff --------

Else - Actions

Custom script: call RemoveLocation(udg_TempLoc)

 JASS:

//Using an If/Then/Else with (Picked unit) stored into a variable
function Filters takes nothing returns boolean
 if ( not ( IsUnitType(udg_TempUnit, UNIT_TYPE_STRUCTURE) == true ) ) then
 return false
 endif
 if ( not ( IsUnitAliveBJ(udg_TempUnit) == true ) ) then
 return false
 endif
 return true
endfunction

function UnitGroupLoop takes nothing returns nothing
 set udg_TempUnit = GetEnumUnit()
 if ( Filters() ) then
 // do stuff
 else
 endif
endfunction

function Actions takes nothing returns nothing
 set udg_TempLoc = GetRectCenter(GetPlayableMapRect())

 set bj_wantDestroyGroup = true
 call ForGroupBJ( GetUnitsInRangeOfLocAll(100.00, udg_TempLoc), function UnitGroupLoop )
 call RemoveLocation(udg_TempLoc)
endfunction

To repeat, you don't magically filter out the units you want to in an instant using (Matching unit). They individually get checked by each filter you have, and if they don't pass, they are removed from the group. So it is done the same exact way as an If/Then/Else, but with ugly results as I describe in the tutorial.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Might be worth mentioning that some standard unit group functions leak due to the LDLHVRCLOR (Local Declared Local Handle Variable Reference Counter Leak On Return) bug.

These include...

 JASS:

function EnumUnitsSelected takes player whichPlayer,boolexpr enumFilter,code enumAction returns nothing
function GetRandomSubGroup takes integer count,group sourceGroup returns group
function GetUnitsInRangeOfLocMatching takes real radius,location whichLocation,boolexpr filter returns group
function GetUnitsInRectMatching takes rect r,boolexpr filter returns group
function GetUnitsInRectOfPlayer takes rect r,player whichPlayer returns group
function GetUnitsOfPlayerAndTypeId takes player whichPlayer,integer unitid returns group
function GetUnitsOfPlayerMatching takes player whichPlayer,boolexpr filter returns group
function GetUnitsOfTypeIdAll takes integer unitid returns group
function GetUnitsSelectedAll takes player whichPlayer returns group

The standard implementation of the above functions can leak handle indices. Either custom implementations must be used, or alternatives. This is yet another reason why GUI in WC3 is so terrible.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Well the tutorial was aimed at GUI users, so a custom implementation and or alternative is out of the question. The tutorial is also not about leaks, it's about making readable and slightly more efficient unit group loops.

I do recall a similar topic being brought up in IcemanBo's Memory Leaks  tutorial.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Is this really better?

Sure it's looks fancier but isn't it more ineffective as well? Somehow it feels like filtering units out before they are added to the group is better.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

As I explained in this post, they are filtered the same exact way. Each enumerated unit goes through each individual filter, one by one. There is no "outsourcing" as A[mun described it.

I see no benefit in ever using (Matching unit) other than having hate to how the GUI If/Then/Else looks. A fair enough excuse to not use it, but what kind of sane person would ever think this looks any better:

Set UnitGroup = (Units within 300.00 of CasterLoc matching (((((Matching unit) is A structure) Equal to False) and (((Matching unit) is Magic Immune) Equal to False)) and ((((Matching unit) is alive) Equal to True) and (((Matching unit) belongs to an enemy of Player) Equal t

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Fair enough.

I guess I'll start to filter this way in the future then.

Does that apply to jass as well though? Regarding ForGroup that is.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

If you want to group units in a range or rect via JASS, it is best to just stick to GroupEnumUnitsInRange (or InRect) and FirstOfGroup.

I can never see ForGroup being used to filter out units. I've only seen it used when people directly add/remove units into the unit group itself.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

If you want good looking code use JASS... WC3 GUI is absolutely terrible and hurts my eyes no matter what one tries to do with it.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Tutorial Title: Proper Unit Group Filtering in GUI

...Proper Unit Group Filtering in GUI​

...Unit Group Filtering in GUI​

...Filtering in GUI​

...in GUI​

The tutorial is for making GUI triggers look clean, not code in general. Regardless, if you wanted good looking code, you would use vJASS. JASS is too damn raw to look good.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Except what you are teaching might not be able to be used because a lot of the GUI group functions leak internally...

At least add a footnote warning of the leaks. The more aware people are about them the fewer leak related problems will surface on the message boards.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

What you are talking about is irrelevant to what the tutorial is aimed to do.

 This short tutorial will show you the best and most efficient way to filter out units enumerated from a unit group function. It will not show you


what usage the function offers (excluding the example trigger), nor how to use them.


Click to expand...


As far as I know, the bug is not game breaking, and no mod has ever enforced such a miniscule issue to GUI users. I will consider adding a footnote when I see it as an issue to worry about. A user would not search for and or read a tutorial called "Proper Unit Group Filtering" if they want to figure out if the unit group function leaks; they would go here, where the exact issue is addressed in the thread.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

People read roughly around what they are looking for. What you are saying is that if someone would look for "how to bake a cake" they are not at all interested in the sort of oven to use despite the sort of oven being very important to the quality of the result. All the "efficiency" in the world does not matter if it still leaks as eventually it can still lead to a crash or poor performance.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Efficiency is such a small reason to do what I suggested in the tutorial. I highly doubt one will be able to recognize the faster execution time from storing (Picked unit) into a variable rather than constantly calling GetFilterUnit(). I only threw it in as a reason to not using (Matching unit) because three reasons are better than two.

The main points I really wanted to emphasize on is readability and the ease of configuring.

#### LimitingBounds

#### LimitingBounds

 Level 3

JoinedMay 12, 2016Messages51

Resources: 1

#### Resources
Spells 1

I think the right word to use to describe this tutorial is "convenient".

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Good idea! I will update it when I decide to fix the tutorial up a little bit more.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The recommented way is also more efficent in question of performance.

Cause the matching And/Or which you can't avoid when using matching, will test each insert condition regardless, if the result is already final.

While the if conditions will skip further conditions if the result is final.

Proof in attached map.

#### Attachments

Test ifs.w3x

 17.4 KB

 · Views: 201



#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Does nobody want to recommend the two trigger solution version?

At least that would be useful for Spell Section.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Daffa the Mage said:

 Does nobody want to recommend the two trigger solution version?

At least that would be useful for Spell Section.


Click to expand...

Some time ago I also played around with this second trigger as condition Idea it can be a nice structed approach, if one is capable of such structed triggering. But I personaly had 2 problems with it.

The first problem is it floots the trigger variableslist(probably also a problem of spell system), this problem can be weakened/countered by having a good naming for everything.

You can't use all filters in any situation in one you check for friendship of trigger Unit to matching unit in the other you need attacking <> picked unit. This problem now requires either 2 filterTriggers or by introtrucing an unit variable. That same thing expands to you compared currently to picking or matching unit. Both require a different filter

But after you found a good structed way to create all that your filters in the group filters are quite small and not as redundant as normaly.

An example when casting holy light near organic non spell immune allies also should be healed by 100 Life.

This is the filter for types it does not include the ally test so it can be used inside any matching filter:

Filter matching not Struct not Mech not SpellImmune
Events

Conditions
((Matching unit) is A structure) Equal to False

((Matching unit) is Mechanical) Equal to False

((Matching unit) is Magic Immune) Equal to False

Actions

Reuse the matching and check for allies

Filter Spell Holy Light
Events

Conditions
((Matching unit) belongs to an ally of (Triggering player)) Equal to True

(Evaluate Filter matching not Struct not Mech not SpellImmune <gen> conditions) Equal to True

Actions

The spellcast Trigger

SpellCast Holy Light
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Holy Light

Actions
Set Loc = (Position of (Target unit of ability being cast))

Set Group = (Units within 512.00 of Loc matching ((Evaluate Filter Spell Holy Light <gen> conditions) Equal to True))

Unit Group - Remove (Target unit of ability being cast) from Group

Unit Group - Pick every unit in Group and do (Actions)
Loop - Actions
Unit - Set life of (Picked unit) to ((Life of (Picked unit)) + 100.00)

Custom script: call RemoveLocation(udg_Loc)

Custom script: call DestroyGroup(udg_Group)

I think it looks pretty nice when one is capable of doing it and can handle so many triggers.

Performance wise it is slower then the other 2 versions (tested using Lua os.clock() with 100 executions), although that does not matter much if it does not happen dozens of times a second.


 Last edited: Nov 9, 2019

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Where has this been all my life...

I've always hated doing the giant nested GUI for Matching Units; so much so I've kinda actively avoided it. But this! This is wonderful.

Many thanks.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

THIS HAS BEEN HERE FOREVER, KYRBI0.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

KILLCIDE said:

 THIS HAS BEEN HERE FOREVER, KYRBI0.


Click to expand...

I KNOW AND I'M SORRY

#### Darth Atroxa

#### Darth Atroxa

 Level 2

JoinedOct 8, 2019Messages9

Ayo, this helps me a lot for the triggers I need for some custom maps, even if I'm hella late. Thanks Kyrbi.

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 Unit not adding in group?


Krakenn99

Apr 4, 2019

Triggers & Scripts

Replies2Views1K

Apr 4, 2019
Pyrogasm

 MUI Spells Using Artificial Waits


Tank-Commander

Sep 28, 2012

Trigger (GUI) Editor Tutorials

Replies32Views20K

Oct 16, 2015
BETABABY

Locked

 Better Filtering for this Action.


JJhonJames

Dec 19, 2024

Triggers & Scripts

Replies2Views730

Dec 20, 2024
Pyrogasm

Locked

[Solved]
 Knockback isn't working for group


Jay the Editor

Mar 6, 2012

Triggers & Scripts

Replies10Views2K

Mar 7, 2012
Jay the Editor

Locked

[Trigger]
 Unit Group - Fail?


TheWhiteWolf

May 3, 2014

Triggers & Scripts

Replies7Views2K

May 5, 2014
TheWhiteWolf

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Trigger (GUI) Editor Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top