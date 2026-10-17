---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ai-manual.238350/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# AI Manual
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ai-manual.238350/>
#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

First and foremost, a hello is in order,


As always Blizzard left us bare stumbling in the dark, so I took the liberty to shed some light on something that has been abandoned for too long, AI natives, there are natives which you can guess what they do by just reading their name but there are more that don't and there were no clue or hint were given to what those native do.


I run lots of tests to determine those native usability and still their are more that are still mysterious.


I will now show the natives that I have determined their functionality.

 JASS:

native CommandsWaiting takes nothing returns integer

Every time you use the trigger action 'Send AI Command' the command queue(counter) increase by 1, this has nothing to do with the value of the command in the argument of the trigger action; so sending an AI signal with (1,0) is as (5,0) everyone will increase the counter by 1.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native PopLastCommand takes nothing returns nothing

Reduce the command queue(counter) by 1. Its important to pop last command because many sleep function in the common.ai exits when the value of the command counter not equal to zero.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetLastCommand takes nothing returns integer

When an AI signal is send via triggers, this native returns the command value. So sending an AI signal of (5,3) returns 5


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetLastData takes nothing returns integer

The same as GetLastCommand() but it returns the data value. For the previous example, it will return 3.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native AddGuardPost takes integer id,real x,real y returns nothing

This marks a spot at point (x,y) as a guard post and a unit of unit-type id(rawcode) will be guarding that spot. The spot will remain empty until the function FillGuardPosts() is called, after that the specified unit will go to guard that spot. If the unit is killed another one will replace it. However, this process is not unlimited, by default, the unit is replaced 3 times but if the native SetReplacementCount(N) is called in the AI script the unit will be replaced N times. Note that if this is used on heroes they will be trained only but will not go to the post.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native FillGuardPosts takes nothing returns nothing

When called, a unit of the same type of the last killed unit that belong to the AI player will be trained and replace the previous one. Note that this native is instant and will not queue, so in order to keep replacing killed units this native have to be nested inside a continuous loop.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native ReturnGuardPosts takes nothing returns nothing

Order every unit that is assigned a guard post to return to it. This native works for pre-placed units and newly trained ones. Note that there is a small tolerance of 82.006 radius offset to the guard post.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native StopGathering takes nothing returns nothing

This native must be called at the start of an AI file so that the following native FillGuardPosts works, if not then FillGuardPosts will continuosly train units if it is nested in a loop and also will cause the trained units not to go to replace the killed ones, instead they will remain near the building that trained them.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetReplacementCount takes integer qty returns nothing

Replaces every AI's player pre-placed units or units that are assigned to a guard posts N times when it is killed. Note that this work for units that are not classified as workers and will not work for peon-type (Peasents,Peons,Acolytes,Wisps and Mur'gul Slaves) even if not classified as workers


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native AttackMoveXY takes integer x,integer y returns nothing

This native gives a signal on the mini-map at location x,y to attack-move to that spot. If the native AddAssault is not called before this one then the units -if any- will remain at their position and will not attack-move the signaled location. Once the units go to the specified location they will return to the base if they didn't find any enemy on their route or it if is destroyed and at least one unit survived in the group.


Important Note: in order to use this native, the native CreateCaptains must be called first or the game will crash. It is already added in the function CampaignAI found at common.ai


Note that coordinates are in integer not real.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native AttackMoveKill takes unit target returns nothing

This native gives a signal on the mini-map at the location where 'unit' stands to attack-move this unit. If the native AddAssault is not called before this one then the units -if any- will remain at their position and will not attack-move the signaled unit. Once the units go to the unit they will return to the base when the unit is killed


if they didn't find any enemy on their route or it if is destroyed and at least one unit survived in the group.


Important Note: in order to use this native, the native CreateCaptains must be called first or the game will crash. It is already added in the function CampaignAI found at common.ai


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native CreepsOnMap takes nothing returns boolean

Checks if there are creeps of any level on the map or not.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native MergeUnits takes integer qty, integer a, integer b, integer make returns boolean

This native will make two units that can be merged to merge, like the hippogrygh rider. The use of this native gives better response if called from inside a loop.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native ConvertUnits takes integer qty, integer id returns boolean

This native have the same effect on units like the obsidian statue when converted to destroyer. The use of this native gives better response if called from inside a loop.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

UnitAlive takes unit id returns boolean

Returns if the unit is alive or not.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native AddAssault takes integer qty,integer id returns boolean

Creates a group from already-existing units that can be assigned to attack a specific target, if AttackMoveXY is called then this group will attack the specified target.


Important Note: in order to use this native, the native CreateCaptains must be called first or the game will crash. It is already added in the function CampaignAI found at common.ai


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native AddDefenders takes integer qty,integer id returns boolean

Creates a group from already-existing units to guard the AI player town. The units assembel around the most important building in the town with no specific formation.


Important Note: in order to use this native, the native CreateCaptains must be called first or the game will crash. It is already added in the function CampaignAI found at common.ai


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetPeonsRepair takes boolean state returns nothing

Turn the feature of making peons to repair damaged structures and mechanical units on or off (state= true for on and false for off)


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetHeroesFlee takes boolean state returns nothing

Turn the feature of making the heroes flee when they are seriously damaged on or off (state= true for on and false for off).


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetUnitsFlee takes boolean state returns nothing

Turn the feature of making the units flee when they are seriously damaged on or off (state= true for on and false for off).


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetGroupsFlee takes boolean state returns nothing

Turn the feature of making the entire group flee when it is seriously damaged on or off (state= true for on and false for off).


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetHeroesBuyItems takes boolean state returns nothing

--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetHeroesTakeItems takes boolean state returns nothing

--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetTargetHeroes takes boolean state returns nothing

--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native PurchaseZeppelin takes nothing returns nothing

Purchace a zeppelin, must be called when a hero is standing near a goblin lab


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetSlowChopping takes boolean state returns nothing

If true the gold harvested by the AI player peons will be 1 per period, instead of 10 per period for false state


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetTargetHeroes takes boolean state returns nothing

Gives priority to target heroes if set to true


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native HarvestGold takes integer town,integer peons returns nothing

Order peons to harvest a gold mine in the town. The peons number is 'peons' and the town index is 'town' (town system)


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native HarvestWood takes integer town,integer peons returns nothing

Order peons to harvest trees near the town. The peons number is 'peons' and the town index is 'town' (town system)


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetUnitCount takes integer unitid returns integer

This native count units of type 'unitid' (rawcode) for the AI player wheather it is trained/built or in progress.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetUnitCountDone takes integer unitid returns integer

This native count units of type 'unitid' (rawcode) for the AI player that it is trained/built or in progress.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetTownUnitCount takes integer id,integer tn,boolean dn returns integer

This native count units of type id (rawcode) for the AI player in a town with ID 'tn', the boolean dn when set to true will count done-only units. Note that this native will return 0 for tn= -1 and will only work for buildings; normal units will always return 0


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetPlayerUnitTypeCount takes player p,integer unitid returns integer

This native returns the count of units of type 'unitid' (rawcode) for the given player 'p'


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetEnemyBase takes nothing returns unit

This native will return the enemy base unit, must use the native StartGetEnemyBase first or it will return null


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native StartGetEnemyBase takes nothing returns nothing

Must be called before using the native GetEnemyBase in order to make it work


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetEnemyExpansion takes nothing returns unit

This native returns the enemy expansion base.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetProduce takes integer qty,integer id,integer town returns boolean

Trains a unit of type id (rawcode) at town with index 'town', the integer 'qty' will work if the player has 'qty' building that can train this unit however this action will not queue, example, the player has 1 barracks and qty= 3 then 1 unit will be trained. Lets suppose that another barracks is built then 2 units will be trained. Will return true if the unit can be trained successfully, i.e. the AI player has all the resources and requirments of the training as well as the building that will start the training


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetUpgrade takes integer id returns boolean

Starts an upgrade for the AI player, the integer 'id' is the rawcode of the upgrade, will return true if the upgrade has started successfully, i.e. the AI player has all the resources and requirments of the upgrade as well as the building that will start the upgrade


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native SetHeroLevels takes code func returns nothing

This native will set how the heroes of the AI player will allocate their skill points as defined in the code function 'func'


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetHeroId takes nothing returns integer

Returns the leveling hero id so it can be used in the heroes levels code function


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetHeroLevelAI takes nothing returns integer

Returns the level of the leveling hero so it can be used in the heroes levels code function


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetBuilding takes player p returns unit

Get the last constructed or pre-placed building for the given player, for pre-placed buildings the order is the number that you find in the WE, example, Blacksmith 0023, Workshop 0065 the last building will be Workshop 0065 but if a peasent in game constructed an Altar then that will be the last building.


Note: player p can be a human controlled player not nessecarily computer


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetCreepCamp takes integer min,integer max,boolean flyers_ok returns unit

This native detects nearest creep camp, the parameters defines the power of the camp.


The power is the total level of all creeps, example, lets say min= 3 and max= 3 then the camp that will be detected will have 3 possibilities:


1) 3 creeps of level 1


2) 1 creep of level 3


3) 1 creep of level 2 and 1 creep of level 1


The boolean parameter allows the detect of flying units in the camp, set to true to allow flying and false for ground units only


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetExpansionPeon takes nothing returns unit

This native will return the peon unit that will start a new expansion for the AI player, however, it will return null if there is no gold mine (excluding a gold mine the AI player already own) found on the map.


Note: this native requires the native CreateCaptains() to be called first or the game will crash.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetExpansionX takes nothing returns integer

Returns the X co-ordinate of the AI player expansion location wheather it is constructed or not, co-ordinates in integer not real


Note: this native requires the native CreateCaptains() to be called first or the game will crash.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetExpansionY takes nothing returns integer

Returns the Y co-ordinate of the AI player expansion location wheather it is constructed or not, co-ordinates in integer not real


Note: this native requires the native CreateCaptains() to be called first or the game will crash.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native GetExpansionFoe takes nothing returns unit

Returns an enemy unit near the expansion location, wheather this unit is a creep or belong to an enemy to the AI player


Note: this native requires the native CreateCaptains() to be called first or the game will crash.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

SetStagePoint takes real x, real y returns nothing
GetMegaTarget takes nothing returns unit
GetEnemyPower takes nothing returns integer
GetAllianceTarget takes nothing returns unit
SetNewHeroes takes boolean state returns nothing
SetCampaignAI takes nothing returns nothing
SetMeleeAI takes nothing returns nothing
SetRandomPaths takes boolean state returns nothing
SetDefendPlayer takes boolean state returns nothing
SetWatchMegaTargets takes boolean state returns nothing
SetIgnoreInjured takes boolean state returns nothing
SetCaptainChanges takes boolean allow returns nothing
SetSmartArtillery takes boolean state returns nothing
GroupTimedLife takes boolean allow returns nothing
RemoveInjuries takes nothing returns nothing
RemoveSiege takes nothing returns nothing
InitAssault takes nothing returns nothing
WaitGetEnemyBase takes nothing returns boolean
LoadZepWave takes integer x, integer y returns nothing
SuicidePlayer takes player id, boolean check_full returns boolean
SuicidePlayerUnits takes player id, boolean check_full returns boolean
ClearHarvestAI takes nothing returns nothing
CreateCaptains takes nothing returns nothing
SetCaptainHome takes integer which, real x, real y returns nothing
ResetCaptainLocs takes nothing returns nothing
ShiftTownSpot takes real x, real y returns nothing
TeleportCaptain takes real x, real y returns nothing
ClearCaptainTargets takes nothing returns nothing
CaptainAttack takes real x, real y returns nothing
CaptainVsUnits takes player id returns nothing
CaptainVsPlayer takes player id returns nothing
CaptainGoHome takes nothing returns nothing
CaptainIsHome takes nothing returns boolean
CaptainIsFull takes nothing returns boolean
CaptainIsEmpty takes nothing returns boolean
CaptainInCombat takes boolean attack_captain returns boolean
CaptainGroupSize takes nothing returns integer
CaptainReadiness takes nothing returns integer
CaptainRetreating takes nothing returns boolean
CaptainReadinessHP takes nothing returns integer
CaptainReadinessMa takes nothing returns integer
CaptainAtGoal takes nothing returns boolean
SuicideUnit takes integer count, integer unitid returns nothing
SuicideUnitEx takes integer ct, integer uid, integer pid returns nothing
UnitInvis takes unit id returns boolean
IgnoredUnits takes integer unitid returns integer
DisablePathing takes nothing returns nothing
SetAmphibious takes nothing returns nothing
MeleeDifficulty takes nothing returns integer

--------------------------------------------------------------------------------------------------------------------------------------

Update 1: 5:42 PM 10/11/2013

 JASS:

native IsTowered takes unit target returns boolean

This native returns either true if the parameter target is guarded by a tower, the tower can be a custom-made unit but there are rules


when checking if the target is guarded or not; the target has to be in the proximity of a base and a tower belonging to the AI player


however aquisition range and attack range of the tower doesn't affect whether the target is defended or not.


--------------------------------------------------------------------------------------------------------------------------------------

 JASS:

native TownThreatened takes nothing returns boolean

Although the name of the native suggests otherwise but this native returns true if any unit (building or unit) for the AI player is


being attacked.


--------------------------------------------------------------------------------------------------------------------------------------


 Last edited: Oct 11, 2013

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

This is wonderful. Many of the AI natives have been a mystery for quite some time. The only documentation I could find on the AI natives were written by AIAndy on wc3jass.com, but sadly the old site when down and thus a lot of his work went with it. This is all that I had salvaged from it (a long time ago), in case it helps at all:

http://wiki.thehelper.net/wc3/jass/common.j/CommandAI

Although, I think yours says something similar. I haven't the time to test every single function, so I'll take your word for it. I think this deserves an approval, and I look forward to any more updates if you have the time.

#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

Thanks, for the approval.


I will look forward to explain more and also improve the formatting of the tutorial as it is written in a plain text. Sorry about that.

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

Awesome tutorial!!!! I haven't noticed this until now. Big hugs to you!!!!

#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

Update 1

Added 2 natives:

IsTowered and TownThreatened

#### JoHnyW

#### JoHnyW

 Level 7

JoinedOct 8, 2007Messages154

Resources: 5

#### Resources
Models 1Icons 2Maps 2

It's amazing people still mod this game I think about comming back to it after so many years of absence

#### Apelli

#### Apelli

 Level 2

JoinedJan 18, 2014Messages5

I would be very grateful,if someone could explain to me,how do I implement only one action loop in my map (only heroes there) for AI heroes,I want them to pick any items visible,and set the priority of this action higher than simple autoattack on enemy heroes?

Please,help!

#### Starquizer

#### Starquizer

 Level 16

JoinedMar 3, 2006Messages1,564

Resources: 7

#### Resources
Maps 1Spells and Systems 3Tutorials 3

Apelli said:

 I would be very grateful,if someone could explain to me,how do I implement only one action loop in my map (only heroes there) for AI heroes,I want them to pick any items visible,and set the priority of this action higher than simple autoattack on enemy heroes?

Please,help!


Click to expand...

Remember that AI is still limited and is not something that will split the Red Sea, the native

 JASS:

SetHeroesTakeItems takes boolean state returns nothing

will make AI behave normally; do the dirty work, pick item then go home or bother someone else.

You will need a custom trigger to do the trick, that is AI, *cough* Blizzard AI

#### Shawwy

#### Shawwy

 Level 3

JoinedMar 6, 2022Messages20

I am trying to impliment the

 ConvertUnits


Click to expand...

function but for some reason none of my druids transform into their bearform

heres the code

 function BuildOrderUnit takes nothing returns nothing

 call GetUnitCountDone(Bear)

 set Bearmen = GetUnitCountDone(Bear)

 if Bearmen < 5 then

 call ConvertUnits (1, Bear)

 elseif Bearmen > 5 then

 call DisplayTextToPlayer(user,0,0,"Need More Bears")

 endif

endfunction

function ConditionLoopUnit takes nothing returns nothing

 loop

 exitwhen Bearmen == 5

 loop

 call BuildOrderUnit()

 call Sleep(5)

 endloop

 endloop

endfunction


Click to expand...

bearmen = number of units

bear = druid of the claw (bearform)

 You must log in or register to reply here.


### Similar threads

 Intermidiate AI concepts - boring no longer


Nowow

Jun 28, 2017

AI Tutorials

Replies11Views11K

Jun 13, 2022
Acolite333

Locked

 WarSocAPI


MindWorX

Jul 9, 2007

Reinventing the Craft

Replies15Views7K

May 22, 2009
Catch_ya

Locked

[JASS]
 Questions about some Reforged natives


Barade

Feb 11, 2021

Triggers & Scripts

Replies1Views2K

Feb 11, 2021
Drake53

Locked

[JASS]
 Native Questions


Nestharus

Jan 10, 2011

Triggers & Scripts

Replies10Views9K

Jan 13, 2011
PurgeandFire

Locked

 KeyboardAPI


MindWorX

Nov 24, 2007

Reinventing the Craft

Replies16Views7K

Feb 19, 2008
PurplePoot

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

AI Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top