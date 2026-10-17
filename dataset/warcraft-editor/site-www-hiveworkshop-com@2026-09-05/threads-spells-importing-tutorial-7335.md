---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/spells-importing-tutorial.7335/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Spells Importing Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/spells-importing-tutorial.7335/>
#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Spells - Importing Tutorial:

Introduction


This tutorial is for those who make spells and those who want to import them. Why for spellmakers? Because if they make importing efficient, their spells will be more appreciated.

1. Main issues


What exactly is to be known about importing spells from a map into another? It’s good to know what must be imported in order to make the map works. It’s not enough to simply copy-paste the main spell and the trigger. Complicated spells will involve units, other spells, buffs, perhaps custom models or items… And so, how do you know which to download? It’s pretty complicated because you don’t really know what exactly to import. Objects needed to be imported are different from one spell to another. For non-triggered spells it is mostly simple. If they use a single spell, just use sub point a) and if they are summoned, copy the summoned units as well. But what if they are JASS/Trigger enchanced?

2. Importing Part


Now we’ll take each type of object (spells, buffs, units, items) and discuss their importing.


a) importing the major spell


How do you know which spell to import? Well, into the map you’ve seen the spell you want to import for your hero. Go to Object Editor – Abilities and search for it under custom spells. Once you’ve found it go Click with your mouse on it into the left panel and then go up into the menu at Edit – Copy Ability. Then open your map, go again to Object Editor – Abilities, into the upper menu at Edit – Paste Ability.


Note: The following objects, triggers and models are not always easy to find. If you can’t find any information into the map or you can’t figure out which are needed for the spell, PM the author of the spell and ask him/her about them.


b) Importing micro spells


It is possible that the main spell is not the only custom but instead, the map still contains other micro spells, which are going to be eventually used by the dummy units.


c) Importing dummy units


Read the FAQ first to find out what dummy units are. Not all enchanced spells have dummy units but if it has, o the same copy-paste method for each of them, but this time in the Object Editor – Units section. Now into your map look at the abilities of the dummy units in the Abilities – Normal field. If it’s different from the original map, make sure to add the custom micro or normal abilities (non-custom) needed.


d) Importing buffs


If the map contains any custom buffs, make sure to check if any of them belongs to the spell you want to import. If it does, use the technique as before to copy-paste the buffs necessary. After that go into the original map and look at the spells’ (micro or major) Stats – Buffs for any buffs. If there are, change those fields accordingly into your map as well.


Warning! Even though the micro spells/buffs are in your map, other buffs/spells might show up into the unit’s/spell’s stats. This is because of the raw code for buffs/spells is different from a map to another. They are numbered in the order in which they are created and if that order is different into your map, then things mix up. We’ll return to the raw code chapter later.


e) Importing items


Check for custom items which might used by the spell. If there are any, copy-paste them into your map as well.


f) Importing custom models


There might be custom models used for the dummies, items, buffs, effects etc. If there are you will need to go into the import manager, export each of them, import them into your map, and put the correct paths.


g) Importing scripts


And here we get to the hard part. If the spell is enchanced it will obviously contain scripts, either in GUI or JASS triggers. We will go separately for GUI and JASS because the methods of implementation are obviously different as well.


I. GUI implementation


These are just normal triggers, with events/conditions/actions. Copy-paste all the spell-related triggers into your map. Now most of you will say: “Ok, we’ve copy-pasted the triggers, things will work out, end of story” but it isn’t so simple. Why? Because we might have raw codes involved as well. And so, how do you solve this problem? Well, let’s take a simple example.


We have the following condition into the original map:

 “Ability Being Cast” equal to Mass Sleep


Click to expand...

but which in your map will look like this:

 “Ability Being Cast” equal to Strafe


Click to expand...


In this case you will have to go and change this condition. In this case we change the Strafe with Mass Sleep and problem is solved. But how do you know in how many places you need to do this change? Usually they will appear when you refer to custom abilities/units/doodads/items/buffs/models. You will have to check the trigger event by event, condition by condition and action by action and change these accordingly so they look like into the original map.


Keep in mind that you have to refer to this when you are talking about a general unit type, doodad type, item type, unit type, buff, ability. So problems cannot appear to constants like Casting Unit, Ability Being Cast, Item Being Manipulated and so on. Also, the problems do not appear when you refer to these objects with variables. They will usually appear when you Create Units (and you refer to a certain unit type), add an ability, remove an ability, create an item or even creating a special effect (it if involves custom models).


II. JASS Implementation


This time, we are talking about a custom script. It means that in the lower right panel of the trigger editor there won’t be the general events/conditions/actions but instead, some writings. And for those who don’t understand JASS, it is very hard to implement spells. We are having the same rawcode problem and this time it will be very hard to scan for the rawcodes themselves since you will problem not get a thing. But I will try to guide you the best I can.


If you are lucky, the spellmaker made the JASS easier but allowing you to change the raw code in a single place and with that, problem solved. I (and many other spellmakers) use the following structure at the beginning of the script. This is just an example:

 JASS:

function x takes nothing returns integer
return ‘A001’
endfunction


Note: “x” can be anything else as well as ‘A001’. The script can also differ but usually that’s how it looks.


You will notice more one such structure since JASS spellmakers who care for implementation include one script/rawcode. In this case, I’ve made a raw code for an ability since it begins with A. If into your map the ability has a different raw code you can replace A001 with anything else. Just watch out to leave the ‘ ’ alone.


Now, how can you see the rawcode? Go to your Object Editor, and then View – Display Values As Raw Data. Instead of the name of the objects you will see a raw code. Then replace that raw code into the script.

WARNING: The raw code is case sensitive.


But what if the spellmaker either is lazy, doesn’t know or simply doesn’t care to add these functions then things will be tougher. But don’t worry. Again I will try to guide you. Switch between the spell’s map and your map so you can see the raw code of the objects you imported. I suggest you to take a piece of paper and write on it something like this:

 Mass Sleep - ‘A001’ -> ‘A004’


Dummy - ‘h000’ -> ‘h000’


Sleep (dummy) - ‘A002’ -> ‘A005’


Click to expand...


This is a pure example but it will probably give you an idea about what I mean. A001 and A004 are the same ability (Mass Sleep) but in the spell’s map it has the rawcode A001 and in your map it has the rawcode A004. In the case of the sleep dummy spell you need to change the script


Do the same sketch with your own spells. Things will be very easy after that. Now, we’ll still continue to work on this example but in your case, make sure to work with your own rawcodes.


We are going to take the following script. It doesn’t matter what it does, you just need learn how to work on rawcodes. Nothing else!

 JASS:

function Trig_Mass_Sleep_Conditions takes nothing returns boolean
return GetSpellAbilityId() == 'A001'
endfunction

function Trig_Mass_Sleep_Actions takes nothing returns nothing

local unit cast
local location p
local group g
local unit u
local unit dumb

set cast = GetTriggerUnit()
set g = GetUnitsInRangeOfLocAll(600.00, GetSpellTargetLoc())
set u = FirstOfGroup(g)

loop
 exitwhen u==null
 set u = FirstOfGroup(g)
 call GroupRemoveUnit(g,u)
 set dumb = CreateUnitAtLoc(GetOwningPlayer(cast), 'h000', GetUnitLoc(u), 0.00)
 call UnitAddAbilityBJ( 'A002', GetTriggerUnit() )
 call UnitApplyTimedLifeBJ( 1, 'BTLF', dumb )
 call IssueTargetOrderBJ( dumb, "sleep", u )
endloop

set g = null
set dumb= null
set cast = null
set p = null

endfunction

//===========================================================================
function InitTrig_Mass_Sleep takes nothing returns nothing
 set gg_trg_Mass_Sleep = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( gg_trg_Mass_Sleep, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition( gg_trg_Mass_Sleep, Condition( function Trig_Mass_Sleep_Conditions ) )
 call TriggerAddAction( gg_trg_Mass_Sleep, function Trig_Mass_Sleep_Actions )
endfunction


Freaky, eh? Don’t worry, things are easier than they seem. Now, let’s have a look at the sketch before the script. We have to search for ‘A001’ rawcode first and change it with ‘A004’. Where do you see it first?

 JASS:

return GetSpellAbilityId() == 'A001'

You simply change the ‘A001’ with ‘A004’. Search for it into the rest of the script. None others? Excellent, then you are done with.


Going further, you will see on the list the dummy unit. But it has the same rawcode so we don’t have to bother about it.


Last but not least the ability ‘A002’. Search for it. Where do you see it?

 JASS:

call UnitAddAbility(GetTriggerUnit(), 'A002')

And this time you replace the ‘A002’ with ‘A005’. You don’t find it anywhere else so that’s kinda all. Do we have other rawcodes to modify? No… problem solved.


NOTE: In some cases the author may also include custom scripts that apply to all the functions. In this case, you will need to copy-paste that script as well into the head of your map. Go into the trigger editor and look at the picture here.


Note: If two maps have different codes you will need to paste the second code in the tail of the first one and so on.


I hope this clarified the problem of rawcodes and I hope you will no longer be afraid of JASS, atleast when you must import it..

3. Making Importing Easier


This chapter involves spellmakers. Hopefully this will make your spells easier to import. There are a couple of important steps you should follow.


a) Mention for each spell which objects must be imported


b) Try to avoid rawcode problem in triggers through instructions (comments) or through variable if your spell is not multi-instanceable.


c) Assure that usually your events involve general units.


Pro Example:

Events

An Unit Starts the Effect of an Ability

Conditions

Ability Being Cast Equal to Mass Sleep

Actions
Negative Example:

Events

Unit – Knight 0001 <gen> Starts the Effect of an Ability

Conditions

Ability Being Cast Equal to Mass Sleep

Actions

Events

An Unit Owned By Player 1 (Red) Starts the Effect of an Ability

Conditions

Ability Being Cast Equal to Mass Sleep

Actions
d) If you have JASS scripts use the function I mentioned before and then whenever you need to use the rawcode just call that function.


And this concludes our tutorial. If you have any other questions or are still confused, state your problem.


~Daelin

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Nice tutorial

Approved

#### FranzLiszt

#### FranzLiszt

 Level 4

JoinedJul 23, 2014Messages73

Maybe I'm just a dumb, but I don't understand...

 You must log in or register to reply here.


### Similar threads

Locked

 Spells - Importing Tutorial


Daelin

Mar 19, 2005

Tutorial Archive

Replies4Views8K

Mar 5, 2007
Wolverabid

 Spells - Importing Tutorial


Daelin

Aug 21, 2005

Misc/General Mapping Tutorials

Replies4Views23K

Jan 14, 2022
Jimbo123

 Making any Spell Autocasting


Ralle

Mar 15, 2006

Trigger (GUI) Editor Tutorials

Replies12Views59K

Dec 22, 2017
Rogue Kessler

 Introduction To Trigger-Enhancing Spells


whitenights

Jul 27, 2004

Trigger (GUI) Editor Tutorials

Replies6Views36K

Jun 8, 2016
TheLordOfChaos201

Locked

 Messy situations importing object data...


Lomacar

Aug 3, 2004

Map Development

Replies5Views2K

Aug 6, 2004
Lomacar

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