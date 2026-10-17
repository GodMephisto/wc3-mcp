---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-ai-workflow.290101/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating AI Workflow
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-ai-workflow.290101/>
#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

# Or How I Learned to Stop Worrying and Read AI

This tutorial serves one primary purpose: teaching you how to learn ai by yourself.

In order to achieve that we'll go through two steps:

First, we'll read through an ai script from one of blizzard's campaign maps and use it to understand how ai works. We will then use our lessons in order to create an ai script which upgrades a tier when prompted.

What you need for this tutorial:

A basic understanding of JASS/other programming language. Nothing too major, but you should know what a function is, global and local variables, looping, etcetera...

A JASS syntax highlighter able to highlight and show native ai functions. I use jasscraft, but whatever floats your boat is fine. You can also keep this page, which has a list of all ai functions, open for reference.

Some ai files to read (see attached files at the bottom of the post).

Reading moyack's tutorial about making a campaign ai is not a must, but it is a good run through the basic functions, and will definitely help you here.

Patience: this tutorial will eventually give you a functioning human ai, but it's worthless without understanding how it works.

##

A short introduction to AI:

In warcraft 3, ai is a piece of code which manages the aspect of player actions. The ai is in fact an external script that can be created in any normal text editor (I suggested jasscraft because it has highlights and it shows the inner workings of some of blizzard's functions, but any notepad will do). The script is then imported to the map and assigned to a computer player with triggers.

Why is ai good:

It doesn't rely on the map to provide it with information (at least not for basic actions). If you want to take an existing ai for your map, you just need to make sure it's an ai of the same race, specify which player to attack, and you're good to go.

It has a lot of built-in functions that really simplify the process of player simulation.

It does not use a lot of leaking variables. The ai works primarily with integers, such as unit type representation instead of units, X/Y coordinates instead of locations etc.

Why is ai bad:

The ai is not specific. If you want the ai to order a specific unit, or build something at a specific location, doing it will be very clumsy.

AI has very little documentation, meaning if you want to create something special, you'll probably need to make it yourself via testing, trial & error, and (god forbid) some creative thinking.

The ai is difficult to debug. So far I've encountered 4 kinds of "crashes":
A real crash when loading the map (i.e. the game quits when loading and displays an error message). This usually happens because of a glaring syntax error, like starting an if clause without an endif, or something else. The other possibility is creating a loop which runs without stopping at all (we will get to that later).

The ai does nothing: can be due to one of many errors such as referencing to a non-existent variable or writing a function without call before it.

The ai doesn't do a specific task: could be a problem with the order of called functions. Check what the ai was supposed to do right after the last thing it did.

Miscellaneous other bugs and quirks, one of which we will see during this tutorial.

Unfortunately for us, we will mostly encounter the second "crash" (at least I did so far), which will require going over the entire script and search for tiny flaws.

All of the problems listed above now bring me to the golden rule of ai: Tread carefully! when writing ai, start small. Write a little piece of code, see if it works, rinse, repeat. When writing an ai with a lot of functionalities, it is even preferred to work with 2 versions just in case you mess something up. Every time you reach a point where all of your code works, save in a different file and keep going.

So, after a long introduction, let's read some ai!

## Campaign ai basics

For our example we will use the ai for both yellow and purple chaos orc players from the fifth blood-elf mission “Gates of the abyss”. Grab it from the attachment below and take a look.

So that's a campaign ai. It's very long, but don't worry, we'll break it up into three parts: Initialization, Base building and Attack strategy.

 JASS:

 call CampaignAI(CHAOS_BURROW,null)
 call SetReplacements(3,3,5)
 call DoCampaignFarms(false)

 JASS:

 call SetBuildUnitEx ( 0,0,1, GREAT_HALL )
 call SetBuildUnit ( 1, CHAOS_PEON )
 call SetBuildUnitEx ( 0,0,2, ORC_BARRACKS )
 call SetBuildUnitEx ( 0,0,5, CHAOS_BURROW )
 call SetBuildUnitEx ( 0,0,1, FORGE )
 call SetBuildUnit ( 8, CHAOS_PEON )
 call SetBuildUnitEx ( 0,0,1, STRONGHOLD )
 call SetBuildUnitEx ( 0,0,2, BESTIARY )
 call SetBuildUnitEx ( 0,0,2, LODGE )

 call CampaignDefenderEx( 0,0,2, CHAOS_GRUNT )
 call CampaignDefenderEx( 0,0,2, CHAOS_WARLOCK )
 call CampaignDefenderEx( 1,1,1, CHAOS_RAIDER )
 call CampaignDefenderEx( 1,1,1, CHAOS_GROM )
 call CampaignDefenderEx( 1,1,3, ORC_DRAGON )

 call SetBuildUpgrEx( 1,1,1, UPG_ORC_BURROWS )
 call SetBuildUpgrEx( 0,0,1, UPG_ORC_ARMOR )
 call SetBuildUpgrEx( 0,0,1, UPG_ORC_MELEE )
 call SetBuildUpgrEx( 1,1,1, UPG_ORC_ENSNARE )
 call SetBuildUpgrEx( 3,3,3, UPG_ORC_SPIKES )

 JASS:

 call WaitForSignal()

 //*** WAVE 1 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 1,1,1, ORC_DRAGON )
 call SuicideOnPlayer(M5,user)

 //*** WAVE 2 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 3,3,5, CHAOS_GRUNT )
 call CampaignAttackerEx( 1,1,1, CHAOS_GROM )
 call SuicideOnPlayerEx(M6,M6,M5,user)

 //*** WAVE 3 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 1,1,2, ORC_DRAGON )
 call SuicideOnPlayerEx(M7,M7,M6,user)

 call SetBuildUpgrEx( 0,0,1, UPG_ORC_BERSERK )

 //*** WAVE 4 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 7,7,9, CHAOS_RAIDER )
 call SuicideOnPlayerEx(M6,M6,M5,user)

 call SetBuildUpgrEx( 0,0,1, UPG_ORC_WAR_DRUMS )

 //*** WAVE 5 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 1,1,2, ORC_DRAGON )
 call SuicideOnPlayerEx(M7,M7,M6,user)

 call SetBuildUpgrEx( 1,1,2, UPG_ORC_ARMOR )
 call SetBuildUpgrEx( 1,1,2, UPG_ORC_MELEE )

 //*** WAVE 6 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 1,1,1, CHAOS_GROM )
 call CampaignAttackerEx( 3,3,4, CHAOS_WARLOCK )
 call SuicideOnPlayerEx(M6,M6,M5,user)

 //*** WAVE 7 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 5,5,7, CHAOS_RAIDER )
 call CampaignAttackerEx( 1,1,1, CHAOS_KODO )
 call SuicideOnPlayerEx(M7,M7,M6,user)

 call SetBuildUpgrEx( 1,1,1, UPG_ORC_WAR_DRUMS )

 //*** WAVE 8 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 1,1,2, ORC_DRAGON )
 call CampaignAttackerEx( 2,2,3, CHAOS_GRUNT )
 call SuicideOnPlayerEx(M6,M6,M5,user)

 //*** WAVE 9 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 3,3,5, CHAOS_GRUNT )
 call CampaignAttackerEx( 1,1,2, CHAOS_KODO )
 call SuicideOnPlayerEx(M7,M7,M6,user)

 call SetBuildUpgrEx( 1,1,3, UPG_ORC_ARMOR )
 call SetBuildUpgrEx( 1,1,3, UPG_ORC_MELEE )

 //*** WAVE 10 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 5,5,7, CHAOS_RAIDER )
 call CampaignAttackerEx( 1,1,1, CHAOS_GROM )
 call SuicideOnPlayerEx(M6,M6,M5,user)

 loop
 //*** WAVE 11 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 1,1,2, ORC_DRAGON )
 call CampaignAttackerEx( 2,2,3, CHAOS_GRUNT )
 call SuicideOnPlayerEx(M7,M7,M6,user)

 //*** WAVE 12 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 1,1,1, CHAOS_GROM )
 call CampaignAttackerEx( 3,3,4, CHAOS_WARLOCK )
 call SuicideOnPlayerEx(M6,M6,M5,user)

 //*** WAVE 13 ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 1,1,2, ORC_DRAGON )
 call CampaignAttackerEx( 2,2,3, CHAOS_RAIDER )
 call SuicideOnPlayerEx(M7,M7,M6,user)

 //*** WAVE 14+ ***
 call InitAssaultGroup()
 call CampaignAttackerEx( 5,5,7, CHAOS_RAIDER )
 call CampaignAttackerEx( 1,1,1, CHAOS_GROM )
 call SuicideOnPlayerEx(M6,M6,M5,user)
 endloop

Ignore the attack strategy and defenders, we'll deal with those in time. At the moment focus yourself on the two functions SetBuildUnitEx (in Base building) and CampaignAI (in Initialization).

Look at the structure building function SetBuildUnitEx(e, m, h, unitid). As you'll see, this function actually calls the function SetBuildAll(BUILD_UNIT, level, unitid, TownNumber) with different parameters for every difficulty:

 JASS:

function SetBuildUnitEx takes integer easy, integer med, integer hard, integer unitid returns nothing
 if difficulty == EASY then
 call SetBuildAll(BUILD_UNIT,easy,unitid,-1)
 elseif difficulty == NORMAL then
 call SetBuildAll(BUILD_UNIT,med,unitid,-1)
 else
 call SetBuildAll(BUILD_UNIT,hard,unitid,-1)
 endif
endfunction

==============================================================

function SetBuildAll takes integer t, integer qty, integer unitid, integer town returns nothing
 if qty > 0 then
 set build_qty[build_length] = qty
 set build_type[build_length] = t
 set build_item[build_length] = unitid
 set build_town[build_length] = town
 set build_length = build_length + 1
 endif
endfunction

Well... That's it? Is that the building part? Doesn't look like it, all I see is variables being assigned. Also, you'll notice that only in hard difficulty the ai orders to build structures with more then zero quantity. Now, if you'll look closely at SetBuildAll(), you'll see that if the quantity given is less than 1, the ai doesn't even assign any variable.

So, where is the building part?

Here we finally reach our first lesson: the ai has a composite workflow. And what that means is that the ai does more than one thing at a time. If you know JASS, or even if you have a solid understanding of triggers, you'll know that functions (or actions) are done one at a time, only really really fast. If you want to make two things happen simultaneously, you'll call two triggers at the same time. But in ai you can't do that cause everything is just one big trigger. So what can you do?

For the answer, we'll look again at the starting point of the ai: CampaignAI(), and see what it does.

 JASS:

function CampaignAI takes integer farms, code heroes returns nothing
 if GetGameDifficulty() == MAP_DIFFICULTY_EASY then
 set difficulty = EASY

 call SetTargetHeroes(false)
 call SetUnitsFlee(false)

 elseif GetGameDifficulty() == MAP_DIFFICULTY_NORMAL then
 set difficulty = NORMAL

 call SetTargetHeroes(false)
 call SetUnitsFlee(false)

 elseif GetGameDifficulty() == MAP_DIFFICULTY_HARD then
 set difficulty = HARD

 call SetPeonsRepair(true)
 else
 set difficulty = INSANE
 endif

 call InitAI()
 call InitBuildArray()
 call InitAssaultGroup()
 call CreateCaptains()

 call SetNewHeroes(false)
 if heroes != null then
 call SetHeroLevels(heroes)
 endif

 call SetHeroesFlee(false)
 call SetGroupsFlee(false)
 call SetSlowChopping(true)
 call GroupTimedLife(false)
 call SetCampaignAI()
 call Sleep(0.1)

 set racial_farm = farms
 call StartThread(function CampaignBasics)
 call StartBuildLoop()
endfunction

You'll notice that at the end of the function, there are two very interesting lines:

 JASS:

 call StartThread(function CampaignBasics)
 call StartBuildLoop()

And inside StartBuildLoop() there's this:

 JASS:

function StartBuildLoop takes nothing returns nothing
 call StartThread(function BuildLoop)
endfunction

This is where the magic happens. StartThread takes a function as an argument, and starts it simultaneously with the other functions. This might not be immediately clear, but bear with me. Let's look at the functions called by StartThread: BuildLoop() & CampaignBasics().

 JASS:

function BuildLoop takes nothing returns nothing
 call OneBuildLoop()
 call StaggerSleep(1,2)
 loop
 call OneBuildLoop()
 call Sleep(2)
 endloop
endfunction

 JASS:

function CampaignBasics takes nothing returns nothing
 call Sleep(1)
 call CampaignBasicsA()
 call StaggerSleep(1,5)
 loop
 call CampaignBasicsA()
 call Sleep(campaign_basics_speed)
 endloop
endfunction

Both functions are loops which call the same function every couple of seconds (With the Sleep function being the waiting time). Nevermind what they do now, it's how they do it: by calling a function constantly, checking and updating its parameters.

Imagine if all of those functions were in the same thread. The first one will be called, and that's it. It's an infinite loop! the ai will stay there and not move to the other functions. The ai enables a sort of branching pattern which constantly checks and updates conditions.

Consider the following chart (each square is the start of a thread, and each circle is an infinite loop):

“But what does that help?” you may ask. Let's wait with that question for a bit, because with our newfound knowledge, we can answer how the ai builds things. For that, let's look at the function called by BuildLoop: OneBuildLoop().

 JASS:

function OneBuildLoop takes nothing returns nothing
 local integer index = 0
 local integer qty
 local integer id
 local integer tp

 set total_gold = GetGold() - gold_buffer
 set total_wood = GetWood()

 loop
 exitwhen index == build_length

 set qty = build_qty [index]
 set id = build_item[index]
 set tp = build_type[index]

 //--------------------------------------------------------------------
 if tp == BUILD_UNIT then
 if not StartUnit(qty,id,build_town[index]) then
 return
 endif

 //--------------------------------------------------------------------
 elseif tp == BUILD_UPGRADE then
 call StartUpgrade(qty,id)

 //--------------------------------------------------------------------
 else // tp == BUILD_EXPAND
 if not StartExpansion(qty,id) then
 return
 endif
 endif

 set index = index + 1
 endloop
endfunction

Now we're getting somewhere! This function takes the parameters you've set in SetBuildUnitEx(), and checks if they can be built, be it unit upgrade or expension. But this isn't it yet, let's go just one level deeper – into StartUnit():

 JASS:

function StartUnit takes integer ask_qty, integer unitid, integer town returns boolean
 local integer have_qty
 local integer need_qty
 local integer afford_gold
 local integer afford_wood
 local integer afford_qty
 local integer gold_cost
 local integer wood_cost

 //------------------------------------------------------------------------
 // if we have all we're asking for then make nothing
 //
 if town == -1 then
 set have_qty = TownCount(unitid)
 else
 set have_qty = TownCountTown(unitid,town)
 endif

 if have_qty >= ask_qty then
 return true
 endif
 set need_qty = ask_qty - have_qty

 //------------------------------------------------------------------------
 // limit the qty we're requesting to the amount of resources available
 //
 set gold_cost = GetUnitGoldCost(unitid)
 set wood_cost = GetUnitWoodCost(unitid)

 if gold_cost == 0 then
 set afford_gold = need_qty
 else
 set afford_gold = total_gold / gold_cost
 endif
 if afford_gold < need_qty then
 set afford_qty = afford_gold
 else
 set afford_qty = need_qty
 endif

 if wood_cost == 0 then
 set afford_wood = need_qty
 else
 set afford_wood = total_wood / wood_cost
 endif
 if afford_wood < afford_qty then
 set afford_qty = afford_wood
 endif

 // if we're waiting on gold/wood; pause build orders
 if afford_qty < 1 then
 return false
 endif

 //------------------------------------------------------------------------
 // whether we make right now what we're requesting or not, assume we will
 // and deduct the cost of the units from our fake gold total right away
 //
 set total_gold = total_gold - gold_cost * need_qty
 set total_wood = total_wood - wood_cost * need_qty

 if total_gold < 0 then
 set total_gold = 0
 endif
 if total_wood < 0 then
 set total_wood = 0
 endif

 //------------------------------------------------------------------------
 // give the AI a chance to make the units (it may not be able to right now
 // but that doesn't stop us from trying other units after this as long
 // as we have enough money to make this AND the needed, unbuilt ones)
 //
 return SetProduce(afford_qty,unitid,town)
endfunction

And there you have it! A detailed, well explained function by Blizzard itself, describing the internal process done in order to determine if and when to build a unit. And if you're still skeptical, let's try seeing what the function SetProduce() does: native SetProduce takes integer qty, integer id, integer town returns boolean.

Nothing. No detailed function, no checked conditions, only an explicit order for the game to build your unit. It was a long road, but damn worth it.

Now we can answer what good are the different threads: if all building functions were on the same thread, the ai would pause entirely every time it will build something. With multiple threads the ai can store your building priorities for later, and goes over it “somewhere else” so it does not interrupt the rest of the ai.

Recap: The ai takes a set of integer arrays which store the building order (indexed by the variable Build_Length), goes over it periodically, and builds whichever unit is not yet built. That is why the ai only rebuilds in hard mode - in hard mode the ai loads the building priorities to its internal build array, then builds everything which is not already built, i.e. everything Keal and Lady Vashj destroy in the orc base. In easy/normal mode, the building priorities are all 0, and the ai rebuilds nothing.

## Building your own AI
Now that we're done with theory, let's move on to the practice. Before you start hacking away at your ai, you need somewhere to test it. a sandbox - if you will.

For that, take some melee map of your choosing, copy it and do the following:

Make sure your ai player is assigned to the ai race.

Give your test subject a lot of resources (over 50,000 of both, don't be cheap).

Make the whole map visible.

Give yourself some OP units and be prepared to use cheat-codes, because testing the ai by playing a normal game will take you a about three times as long.

Delete the melee initialization trigger (or just the "Melee Game - Run melee AI scripts (for computer players)" action). Instead you'll import your own ai file via the import manager (remember, for that to work you'll need your script to have .ai extension) and set it up to the computer player with this action: "AI - Start campaign AI script for someplayer: yourscript.ai".

Melee Initialization
Events
Map initialization

Conditions

Actions
Melee Game - Use melee time of day (for all players)

Melee Game - Limit Heroes to 1 per Hero-type (for all players)

Melee Game - Set starting resources (for all players)

Melee Game - Remove creeps and critters from used start locations (for all players)

Melee Game - Create starting units (for all players)

AI - Start campaign AI script for Player 2 (Blue): war3mapImported\Gold Digger.ai

Player - Set Player 2 (Blue) Current gold to 75000

Player - Set Player 2 (Blue) Current lumber to 75000

Visibility - Create an initially Enabled visibility modifier for Player 1 (Red) emitting Visibility across (Playable map area)

### What's Init
We'll start the our AI script the same as the example we were reading: a global declaration and and the initialization part. The global declaration simple enough - you set the target player, as well as a variable integer named "Tier", which will be initially set to 1.

The initialization is composed of 3 functions: CampaignAI(), SetReplacements(), DoCampaignFarms(). We're already familiar with the CampaignAI() function, which starts our building and campaign basic functions. But before it does that, CampaignAI() determines the AI difficulty, then sets its attributes accordingly (peons repair buildings and injured units flee from damage only on hard difficulty for instance), other characteristics include:

SetSlowChopping(true) - and causes the ai to harvest 1 gold and 1 lumber each time.

SetHeroesFlee(false) - if true heroes will flee.

SetGroupsFlee(false) - if true unit groups will flee.

GroupTimedLife(false) - if this function is set to true summoned units will join the summoners group. In the blood elf ai script of the last undead mission, this function is set to true - meaning that if Keal summons a phoenix during an attack, it will stay with him instead of just standing there attacking anyone in its aggro-range.

InitAI() and SetCampaignAI() - these two functions state the start of ai actions and declere that it is a campaign ai. These are just init natives that must be called at the beginning of the ai, not much more to say about them.

CreateCaptains() -This is another init function which must be called at the start of the ai, and we will discuss what it does later in this tutorial.

If a hero was assigned to the ai, it will save its leveling function in SetHeroLevels(heroes). We will not discuss that function in this tutorial, but you can go to any melee ai and see how it is done.

Setting the ai's farm building according to the argument assigned to CampaignAI (line set racial_farm = farms). We'll see why that's important in a second.
Moving over to CampaignBasicsA(), this function resets the harvest priorities every time it is called, and is responsible for building attackers and defenders. One interesting point about this function, is that it can create new farm buildings outside of the building priorities if the food cap is about to be reached:

 JASS:

if do_campaign_farms and FoodUsed()+food_each-1 > food_each*(TownCount(racial_farm)+1) then
 call StartUnit(TownCount(racial_farm)+1,racial_farm,-1)
endif

the first condition "do_campaign_farms" is actually determined in the third init function - DoCampaignFarms(false). In other words, the ai does not allow building extra farms in this case.

* It is worth noting that apparently CampaignBasicsA() does not consider the max food cap when its building farms, making the ai build another farm before stopping (i.e. according to the ai the food cap is 110, so if you're changing the gameplay constants of food cap the ai has got you covered).

The last part of initializing the ai is SetReplacements() - which determines how many times preplaced units will be produced after being killed. We will not use that in our ai, but this can be very helpful when creating ai for missions, so keep that in mind.

 JASS:

globals
 player user = Player(0)
 integer Tier = 1
endglobals

function main takes nothing returns nothing
 call CampaignAI('hhou',null)
 call DoCampaignFarms(false)
endfunction

Moving along, we'll write our building priorities. For that, we'll use the function SetBuildUnitEx(e,m,h,u) that we already looked into earlier. The function takes the number of built units for each level - easy, medium, hard, and the built unit.

The units types in the editor are written with 4 letter representations like this - peasant = 'hpea', footman = 'hfoo' and so on, to see all unit type representations go to the object editor and press Ctrl+d. Another option is to set your unit representation to a variable at the beginning of the script. Blizzard did that in the common ai file, so peasent = PEASENT, footman =FOOTMAN/FOOTMEN and a red chaos grunt = CHAOS_GRUNT (as you can see in the script of "Gates of the Abyss").

Last thing, the amount arguments of this function account for the total amount of units which should be built.

So, if you want to build a peasant but you already have 5 from the start of the game you'll need to write SetBuildUnitEx(6,6,6, 'hpea') or SetBuildUnitEx(6,6,6, PEASENT).

I trust you to create your own build order so that it will suit the human techtree dependencies (if you won't the ai will just stop where it can't build the next structure), just put it in a function of its own and condition whatever is in a higher tier to concur with our Tier variable, as in upgrading to a keep and all tier 2 structures will be inside an if Tier > 1 then clause. This is what my function looks like at this point:

 JASS:

//GLOBAL DECELERATION
//------------------------------------------------
globals
 player user = Player(0)
 integer Tier = 1
endglobals

//BUILD PRIORITIES
//------------------------------------------------
function BuildOrder takes nothing returns nothing
 call SetBuildUnitEx( 1,1,1, 'hpea') // peasent
 call SetBuildUnitEx( 1,1,1, 'htow') // town hall
 call SetBuildUnitEx( 5,5,5, 'hpea') // peasent
 call SetBuildUnitEx( 1,1,1, 'hbar') // barracks
 call SetBuildUnitEx( 2,2,2, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'halt') // hero altar
 call SetBuildUnitEx( 8,8,8, 'hpea') // peasent
 call SetBuildUnitEx( 5,5,5, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'hlum') // lumber mill
 call SetBuildUnitEx( 1,1,1, 'hbla') // blacksmith

 if Tier > 1 then
 call SetBuildUnitEx( 1,1,1, 'hkee') // Keep upgrade
 call SetBuildUnitEx( 1,1,1, 'hars') // arcane sanctum
 endif

 call SetBuildUnitEx( 1,1,1, 'hvlt') // item shop
 call SetBuildUnitEx( 6,6,6, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'hwtw') // watch tower

 if Tier > 1 then
 call SetBuildUnitEx( 1,1,1, 'harm') // workshop
 call SetBuildUnitEx( 11,11,11, 'hhou') // farm
 call SetBuildUnitEx( 3,3,3, 'hwtw') // watch tower
 endif

 if Tier > 2 then
 call SetBuildUnitEx( 1,1,1, 'hcas') // Castle upgrade
 call SetBuildUnitEx( 1,1,1, 'hatw') // arcane tower
 call SetBuildUnitEx( 1,1,1, 'hgra') // gryphon aviery
 call SetBuildUnitEx( 1,1,1, 'hgtw') // guard tower
 call SetBuildUnitEx( 1,1,1, 'hctw') // canon tower
 call SetBuildUnitEx( 14,14,14, 'hhou') // farm
 endif
endfunction
//MAIN FUNCTION
//------------------------------------------------
function main takes nothing returns nothing
 call CampaignAI('hhou',null)
 call DoCampaignFarms(false)

 call BuildOrder()
endfunction

So now we have a building priorities, but if you try to run that the peons will only build tier 1 structures. Let's solve that by using another functionality of the ai: commands.

### Commands
AI commands are triggers which send info to the ai, and are used to make the ai more reactive. Each ai command can send two integers to the ai script: command integer and data integer. Go to your test map's trigger editor and add the following trigger:

Command
Events
Player - Player 1 (Red) types a chat message containing -COM as An exact match

Conditions

Actions
AI - Send Player 2 (Blue) the AI Command (CommandInteger, DataInteger)

For now, both command and data integers can be 0, and after learning what commands do and how to use them we will see if that needs changing.

You've already seen one of the ways the ai receives a command like that: WaitForSignal() in the gates of the abyss ai we're using in this tutorial, it's called right before the attack waves start. Opening up WaitForSignal() will reveal that it is in fact an infinite loop, which exits only what a different function called CommandsWaiting() returns a value greater than 0.

 JASS:

function WaitForSignal takes nothing returns integer
 local integer cmd
 local boolean display = false //xxx
 loop
 exitwhen CommandsWaiting() != 0

 //xxx
 call Trace("waiting for a signal to begin AI script...\n")
 set display = true
 call Sleep(2)
 exitwhen CommandsWaiting() != 0
 call Sleep(2)
 exitwhen CommandsWaiting() != 0
 call Sleep(2)
 exitwhen CommandsWaiting() != 0
 call Sleep(2)
 exitwhen CommandsWaiting() != 0
 call Sleep(2)
 //xxx

 endloop

 //xxx
 if display then
 call Trace("signal received, beginning AI script\n")
 endif
 //xxx

 set cmd = GetLastCommand()
 call PopLastCommand()
 return cmd
endfunction

Don't trouble yourself with the display variable and Trace command, these are probably remnants of the game developers' version which displayed a lot of the game's internal works. what you should care about is what this function actually does: it jams the ai script in an infinite loop until it receives an order. A lot of blizzard's campaign ais use WaitForSignal() in order to wait with the attacking part of the ai until they send it a command. Most of the times the command comes when the starting cinematic ends, in "gates of the abyss" the command waits until Illidan closes the first of four portals.

However that's not the end of WaitForSignal(). It does two very important things:

At the start of the function it sets a local integer cmd. Then, when a command is received and breaks the loop, it will assign the commandInteger (that's the first integer you can specify in the command trigger, remember?) to the cmd local variable and returns it.

At the end of the function it calls the PopLastCommand() function, which deletes the last command. That part is mandatory if you want to use more than one command (which you need for this tutorial).
So we'll go back to the map and create two triggers, both send a different integer as a command number to the ai, each trigger for every tier:

Tier 2
Events
Player - Player 1 (Red) types a chat message containing -Tier2 as An exact match

Conditions

Actions
AI - Send Player 2 (Blue) the AI Command (2, 0)

Tier 2
Events
Player - Player 1 (Red) types a chat message containing -Tier3 as An exact match

Conditions

Actions
AI - Send Player 2 (Blue) the AI Command (3, 0)

So we want the ai to catch the command when it happens and change the player's behaviour accordingly. In other words, we want something that constantly checks if there is a command waiting, without stopping the entire ai script. Sounds familier? it should, because that's exactly what threads are for. We'll create 2 functions, a loop function and a function that does the actual command catching. Our command catching function will assign the command integer to the "Tier" variable, and then empty the building priorities with InitBuildArray() and start it again. like this:

 JASS:

function TierCondition takes nothing returns nothing
 if CommandsWaiting() > 0 then
 set Tier = GetLastCommand()
 call PopLastCommand()

 call InitBuildArray()

 call BuildOrder()
 endif
endfunction

As for the loop function, well you saw how blizzard did it, so let's base our loop function on that:

 JASS:

function ConditionLoop takes nothing returns nothing
 call TierCondition()
 call StaggerSleep(1,5)
 loop
 call TierCondition()
 call Sleep(2)
 endloop
endfunction

We'll integrate that into our main function with StartThread(), and see what we've got.

 JASS:

//GLOBAL DECELERATION
//------------------------------------------------
globals
 player user = Player(0)
 integer Tier = 1
endglobals

//BUILD PRIORITIES
//------------------------------------------------
function BuildOrder takes nothing returns nothing
 call SetBuildUnitEx( 1,1,1, 'hpea') // peasent
 call SetBuildUnitEx( 1,1,1, 'htow') // town hall
 call SetBuildUnitEx( 5,5,5, 'hpea') // peasent
 call SetBuildUnitEx( 1,1,1, 'hbar') // barracks
 call SetBuildUnitEx( 2,2,2, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'halt') // hero altar
 call SetBuildUnitEx( 8,8,8, 'hpea') // peasent
 call SetBuildUnitEx( 5,5,5, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'hlum') // lumber mill
 call SetBuildUnitEx( 1,1,1, 'hbla') // blacksmith

 if Tier > 1 then
 call SetBuildUnitEx( 1,1,1, 'hkee') // Keep upgrade
 call SetBuildUnitEx( 1,1,1, 'hars') // arcane sanctum
 endif

 call SetBuildUnitEx( 1,1,1, 'hvlt') // item shop
 call SetBuildUnitEx( 6,6,6, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'hwtw') // watch tower

 if Tier > 1 then
 call SetBuildUnitEx( 1,1,1, 'harm') // workshop
 call SetBuildUnitEx( 11,11,11, 'hhou') // farm
 call SetBuildUnitEx( 3,3,3, 'hwtw') // watch tower
 endif

 if Tier > 2 then
 call SetBuildUnitEx( 1,1,1, 'hcas') // Castle upgrade
 call SetBuildUnitEx( 1,1,1, 'hatw') // arcane tower
 call SetBuildUnitEx( 1,1,1, 'hgra') // gryphon aviery
 call SetBuildUnitEx( 1,1,1, 'hgtw') // guard tower
 call SetBuildUnitEx( 1,1,1, 'hctw') // canon tower
 call SetBuildUnitEx( 14,14,14, 'hhou') // farm
 endif
endfunction

//COMMAND FUNCTIONS
//------------------------------------------------

function TierCondition takes nothing returns nothing
 if CommandsWaiting() > 0 then
 set Tier = GetLastCommand()
 call PopLastCommand()

 call InitBuildArray()

 call BuildOrder()
 endif
endfunction

function ConditionLoop takes nothing returns nothing
 call TierCondition()
 call StaggerSleep(1,5)
 loop
 call TierCondition()
 call Sleep(5)
 endloop
endfunction

//MAIN FUNCTION
//------------------------------------------------
function main takes nothing returns nothing
 call CampaignAI('hhou',null)
 call DoCampaignFarms(false)

 call BuildOrder()

 call StartThread(function ConditionLoop)
endfunction

Now this is a nice base we've got going there, it'd be a shame if something (like your group of preplaced OP units) will come over and attack this base, wouldn't it? So let's go on to build some defenders for the base.

### Base defenders
There are only 2 functions you should know about when creating base defenders: CampaignDefenderEx and InitDefenseGroup. The first function stores your defending units in a variable array (like SetBuildUnitEx for buildings), and the second one empties said array (like InitBuildArray for buildings). Pretty simple right? Well there is one catch: defending units are saved in 2 arrays. Observe:

 JASS:

function CampaignDefender takes integer level, integer qty, integer unitid returns nothing
 if qty > 0 and difficulty >= level then
 set defense_qty[defense_length] = qty
 set defense_units[defense_length] = unitid
 set defense_length = defense_length + 1
 call Conversions(qty,unitid)
 call SetBuildUnit(qty,unitid) //<==AN ILLUSION! WHAT ARE YOU HIDING?!
 endif
endfunction

=========================================================

function CampaignDefenderEx takes integer easy, integer med, integer hard, integer unitid returns nothing
 if difficulty == EASY then
 call CampaignDefender(EASY,easy,unitid)
 elseif difficulty == NORMAL then
 call CampaignDefender(NORMAL,med,unitid)
 else
 call CampaignDefender(HARD,hard,unitid)
 endif
endfunction

What the function SetBuildUnit hides is that it stores the defending units in the building array, the same array where the structures of your base are stored. In this particular script, we shouldn't care about that, but if you want to change your defenders mid-game without changing your building strategy remember to empty your building list with InitBuildArray as well, otherwise the ai will build the old defending units which will just stand there doing nothing. So we will create a function which will set the defenders according to the tier, and we'll update the function TierCondition which already calls the building function again after changing the Tier variable. Something like this:

 JASS:

function CampaignDefenses takes nothing returns nothing
 if Tier == 1 then
 call CampaignDefenderEx( 2,2,2, 'hfoo' ) // footman
 call CampaignDefenderEx( 2,2,2, 'hrif' ) // rifleman

 elseif Tier == 2 then
 call CampaignDefenderEx( 2,2,2, 'hmpr' ) // priest
 call CampaignDefenderEx( 1,1,1, 'hrif' ) // rifleman
 call CampaignDefenderEx( 4,4,4, 'hfoo' ) // footman

 elseif Tier == 3 then
 call CampaignDefenderEx( 2,2,2, 'hgyr' ) // gyrocopter
 call CampaignDefenderEx( 3,3,3, 'hkni' ) // knight
 call CampaignDefenderEx( 2,2,2, 'hmpr' ) // priest
 call CampaignDefenderEx( 1,1,1, 'hsor' ) // sorceress
 call CampaignDefenderEx( 1,1,1, 'hgry' ) // gryphon rider
 endif
endfunction

//================================================

function TierCondition takes nothing returns nothing
 if CommandsWaiting() > 0 then
 set Tier = GetLastCommand()
 call PopLastCommand()

 call InitBuildArray()
 call InitDefenseGroup()

 call BuildOrder()
 call CampaignDefenses()
 endif
endfunction

Now, your base will build defending units, and will change them when changing a tier. You'll be able to see the changes when you try to attack your ai base (which I strongly suggest, it's very fun).

 JASS:

//GLOBAL DECELERATION
//------------------------------------------------
globals
 player user = Player(0)
 integer Tier = 1
endglobals

//BUILD PRIORITIES
//------------------------------------------------
function BuildOrder takes nothing returns nothing
 call SetBuildUnitEx( 1,1,1, 'hpea') // peasent
 call SetBuildUnitEx( 1,1,1, 'htow') // town hall
 call SetBuildUnitEx( 5,5,5, 'hpea') // peasent
 call SetBuildUnitEx( 1,1,1, 'hbar') // barracks
 call SetBuildUnitEx( 2,2,2, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'halt') // hero altar
 call SetBuildUnitEx( 8,8,8, 'hpea') // peasent
 call SetBuildUnitEx( 5,5,5, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'hlum') // lumber mill
 call SetBuildUnitEx( 1,1,1, 'hbla') // blacksmith

 if Tier > 1 then
 call SetBuildUnitEx( 1,1,1, 'hkee') // Keep upgrade
 call SetBuildUnitEx( 1,1,1, 'hars') // arcane sanctum
 endif

 call SetBuildUnitEx( 1,1,1, 'hvlt') // item shop
 call SetBuildUnitEx( 6,6,6, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'hwtw') // watch tower

 if Tier > 1 then
 call SetBuildUnitEx( 1,1,1, 'harm') // workshop
 call SetBuildUnitEx( 11,11,11, 'hhou') // farm
 call SetBuildUnitEx( 3,3,3, 'hwtw') // watch tower
 endif

 if Tier > 2 then
 call SetBuildUnitEx( 1,1,1, 'hcas') // Castle upgrade
 call SetBuildUnitEx( 1,1,1, 'hatw') // arcane tower
 call SetBuildUnitEx( 1,1,1, 'hgra') // gryphon aviery
 call SetBuildUnitEx( 1,1,1, 'hgtw') // guard tower
 call SetBuildUnitEx( 1,1,1, 'hctw') // canon tower
 call SetBuildUnitEx( 14,14,14, 'hhou') // farm
 endif
endfunction

//BASE DEFENSE
//------------------------------------------------
function CampaignDefenses takes nothing returns nothing
 if Tier == 1 then
 call CampaignDefenderEx( 2,2,2, 'hfoo') // footman
 call CampaignDefenderEx( 2,2,2, 'hrif') // rifleman

 elseif Tier == 2 then
 call CampaignDefenderEx( 2,2,2, 'hmpr') // priest
 call CampaignDefenderEx( 1,1,1, 'hrif') // rifleman
 call CampaignDefenderEx( 4,4,4, 'hfoo') // footman

 elseif Tier == 3 then
 call CampaignDefenderEx( 2,2,2, 'hgyr') // gyrocopter
 call CampaignDefenderEx( 3,3,3, 'hkni') // knight
 call CampaignDefenderEx( 2,2,2, 'hmpr') // priest
 call CampaignDefenderEx( 1,1,1, 'hsor') // sorceress
 call CampaignDefenderEx( 1,1,1, 'hgry') // gryphon rider
 endif
endfunction

//COMMAND FUNCTIONS
//------------------------------------------------

function TierCondition takes nothing returns nothing
 if CommandsWaiting() > 0 then
 set Tier = GetLastCommand()
 call PopLastCommand()
 call InitBuildArray()
 call InitDefenseGroup()
 call BuildOrder()
 call CampaignDefenses()
 endif
endfunction

function ConditionLoop takes nothing returns nothing
 call TierCondition()
 call StaggerSleep(1,5)
 loop
 call TierCondition()
 call Sleep(5)
 endloop
endfunction

//MAIN FUNCTION
//------------------------------------------------
function main takes nothing returns nothing
 call CampaignAI('hhou',null)
 call DoCampaignFarms(false)

 call BuildOrder()
 call CampaignDefenses()

 call StartThread(function ConditionLoop)
endfunction

We're done with all the necessary fluff, and the base is ready to crank out attack waves at any foe you wish.

### Attack strategy

To start building the attack wave strategy, we will need another thread. But before you go spamming StartThread away, remember that we already have a perfectly good thread free to use: the main function. So you'll use that, and start creating the attack wave functions.

This is how one attack wave by Blizzard looks like:

 JASS:

call InitAssaultGroup()
call CampaignAttackerEx( 1,1,1, DEATH_KNIGHT)
call CampaignAttackerEx( 4,4,6, GHOUL )
call CampaignAttackerEx( 2,2,4, ABOMINATION )
call CampaignAttackerEx( 0,0,1, MEAT_WAGON )
call SuicideOnPlayerEx(M7,M7,M5,user)

Yes, this is from another campaign mission, but all waves follow the same template: empty the attacking units array (like we did with the defenders and the build priorities), then set new attacking units, then start the attack.

The attacking function looks like this: SuicideOnPlayerEx(e,n,h,target), with e=time the script waits before attacking in easy difficulty, n=time the script waits before attacking in normal difficulty, h=time the script waits before attacking in hard difficulty, and target=attacked player.

So we make a couple of attack waves and then what? Even if you create 50 attack waves, the ai will go over all of them in a really long game, and you'll run out of ideas for attack groups by wave 25 tops. So we start looping after a certain wave, just like the example "Gates of the abyss".

We don't need just one wave strategy though, we need one for every tier, and a way to change between them. We'll do that by creating 3 wave strategy functions. Each of those functions will start with setting a local integer equal to the Tier global variable, lets call it AttackTier. We'll use that to check if Tier changed every now and then.

To implement this exit strategy, we'll put our entire attack wave strategy inside a loop and write after every wave this line exitwhen AttackTier != Tier. This way, whenever the tier changes the attack strategy will change when the current attack wave is done. Inside that, we will nest another function which will act as the actual loop of the attack waves. The exit condition to this loop will also be exitwhen AttackTier != Tier.

Alright, this was a bit much, so let's recap with an example:

 JASS:

function Tier2Waves takes nothing returns nothing
 local integer AttackTier = Tier

 loop
 //This is the "outer loop". It has no purpose except providing us with comfortable exit points from the function.
 //As such, it will not run more then once.

 //Wave 1
 call InitAssaultGroup()
 call CampaignAttackerEx(3,4,6,FOOTMAN) //Writing units like this is also fine.
 call CampaignAttackerEx(1,2,2,PRIEST)
 call SuicideOnPlayerEx(M4,M4,M3,user)
 exitwhen AttackTier != Tier

 //Wave 2
 call InitAssaultGroup()
 call CampaignAttackerEx(5,5,8,RIFLEMAN)
 call SuicideOnPlayerEx(M4,M4,M4,user)
 exitwhen AttackTier != Tier

 //Wave 3
 call InitAssaultGroup()
 call CampaignAttackerEx(3,4,4,FOOTMAN)
 call CampaignAttackerEx(1,1,2,PRIEST)
 call CampaignAttackerEx(3,3,3,SORCERESS)
 call SuicideOnPlayerEx(M5,M5,M4,user)
 exitwhen AttackTier != Tier

 //This is the inner loop, which will run as much as necessary.
 //Note it has the same "exitwhen" as the outer loop.
 //This way when the condition is fulfilled both layers of the loop will break.
 loop
 //Wave 4+
 call InitAssaultGroup()
 call CampaignAttackerEx(6,9,8,FOOTMAN)
 call CampaignAttackerEx(1,2,3,MORTAR)
 call SuicideOnPlayerEx(M7,M7,M5,user)
 exitwhen AttackTier != Tier

 //Wave 5+
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,6,GYRO)
 call SuicideOnPlayerEx(M6,M6,M5,user)
 exitwhen AttackTier != Tier

 //Wave 6+
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,3,RIFLEMAN)
 call CampaignAttackerEx(0,2,4,SORCERESS)
 call CampaignAttackerEx(1,1,3,PRIEST)
 call SuicideOnPlayerEx(M6,M6,M6,user)
 exitwhen AttackTier != Tier

 //Wave 7+
 call InitAssaultGroup()
 call CampaignAttackerEx(6,6,8,FOOTMAN)
 call CampaignAttackerEx(1,2,2,MORTAR)
 call CampaignAttackerEx(1,1,3,PRIEST)
 call SuicideOnPlayerEx(M7,M7,M7,user)
 exitwhen AttackTier != Tier
 endloop
 //Don't forget to add another line of exitwhen after the inner loop,
 //otherwise the ai will send the first wave again before exiting the function.
 exitwhen AttackTier != Tier
 endloop
endfunction

Now the last thing we need to put here are upgrades - those will go in between waves using the function SetBuildUpgrEx(e,m,h, upgrade). This function works just like any other build unit function, except that instead of representing the amount of units built, e,m and h represent the level of the upgrade. The upgrade function is also like a normal build function because once you're done researching the upgrade, you can call the function as much as you want, it won't do anything - so there's no problem to put it in a loop. So after adding the upgrades, the attack wave functions should look like this:

 JASS:

//ATTACK WAVE STRATEGY
//------------------------------------------------

function Tier1Waves takes nothing returns nothing
 local integer AttackTier = Tier

 loop
 call SetBuildUpgrEx(1,1,1, 'Rhde')

 //Wave 1
 call InitAssaultGroup()
 call CampaignAttackerEx(3,4,6,FOOTMAN)
 call SuicideOnPlayerEx(15,15,15,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhra')
 call SetBuildUpgrEx(1,1,1, 'Rhla')

 //Wave 2
 call InitAssaultGroup()
 call CampaignAttackerEx(2,3,5,RIFLEMAN)
 call SuicideOnPlayerEx(15,15,15,user)
 exitwhen AttackTier != Tier

 loop
 //Wave 3
 call InitAssaultGroup()
 call CampaignAttackerEx(3,3,8,FOOTMAN)
 call CampaignAttackerEx(2,4,2,RIFLEMAN)
 call SuicideOnPlayerEx(20,20,20,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhme')
 call SetBuildUpgrEx(1,1,1, 'Rhar')

 //Wave 4
 call InitAssaultGroup()
 call CampaignAttackerEx(6,7,4,FOOTMAN)
 call CampaignAttackerEx(2,3,6,RIFLEMAN)
 call SuicideOnPlayerEx(25,25,25,user)
 exitwhen AttackTier != Tier
 endloop
 exitwhen AttackTier != Tier
 endloop
endfunction

function Tier2Waves takes nothing returns nothing
 local integer AttackTier = Tier
 //Research all upgrades of earlier tier.
 call SetBuildUpgrEx(1,1,1, 'Rhra')
 call SetBuildUpgrEx(1,1,1, 'Rhla')
 call SetBuildUpgrEx(1,1,1, 'Rhme')
 call SetBuildUpgrEx(1,1,1, 'Rhar')
 call SetBuildUpgrEx(1,1,1, 'Rhde')

 loop
 call SetBuildUpgrEx(1,1,1, 'Rhpt')

 //Wave 1
 call InitAssaultGroup()
 call CampaignAttackerEx(3,4,6,FOOTMAN)
 call CampaignAttackerEx(1,2,2,PRIEST)
 call SuicideOnPlayerEx(20,20,20,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhri')

 //Wave 2
 call InitAssaultGroup()
 call CampaignAttackerEx(5,5,8,RIFLEMAN)
 call SuicideOnPlayerEx(25,25,25,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(2,2,2, 'Rhme')
 call SetBuildUpgrEx(2,2,2, 'Rhar')
 call SetBuildUpgrEx(1,1,1, 'Rhst')

 //Wave 3
 call InitAssaultGroup()
 call CampaignAttackerEx(3,4,4,FOOTMAN)
 call CampaignAttackerEx(1,1,2,PRIEST)
 call CampaignAttackerEx(3,3,3,SORCERESS)
 call SuicideOnPlayerEx(30,35,35,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(2,2,2, 'Rhra')
 call SetBuildUpgrEx(2,2,2, 'Rhla')

 loop
 //Wave 4+
 call InitAssaultGroup()
 call CampaignAttackerEx(6,9,8,FOOTMAN)
 call CampaignAttackerEx(1,2,3,MORTAR)
 call SuicideOnPlayerEx(40,40,40,user)
 exitwhen AttackTier != Tier

 //Wave 5+
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,6,GYRO)
 call SuicideOnPlayerEx(30,30,30,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhcd')

 //Wave 6+
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,3,RIFLEMAN)
 call CampaignAttackerEx(0,2,4,SORCERESS)
 call CampaignAttackerEx(1,1,3,PRIEST)
 call SuicideOnPlayerEx(35,35,35,user)
 exitwhen AttackTier != Tier

 //Wave 7+
 call InitAssaultGroup()
 call CampaignAttackerEx(6,6,8,FOOTMAN)
 call CampaignAttackerEx(1,2,2,MORTAR)
 call CampaignAttackerEx(1,1,3,PRIEST)
 call SuicideOnPlayerEx(45,45,45,user)
 exitwhen AttackTier != Tier
 endloop
 exitwhen AttackTier != Tier
 endloop
endfunction

function Tier3Waves takes nothing returns nothing
 local integer AttackTier = Tier
 //Research all upgrades of earlier tier.
 call SetBuildUpgrEx(1,1,1, 'Rhpt')
 call SetBuildUpgrEx(1,1,1, 'Rhri')
 call SetBuildUpgrEx(2,2,2, 'Rhme')
 call SetBuildUpgrEx(2,2,2, 'Rhar')
 call SetBuildUpgrEx(1,1,1, 'Rhst')
 call SetBuildUpgrEx(2,2,2, 'Rhra')
 call SetBuildUpgrEx(2,2,2, 'Rhla')
 call SetBuildUpgrEx(1,1,1, 'Rhcd')

 loop
 call SetBuildUpgrEx(2,2,2, 'Rhpt')

 //Wave 1
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,7,FOOTMAN)
 call CampaignAttackerEx(1,2,2,PRIEST)
 call CampaignAttackerEx(0,0,2,TANK)
 call SuicideOnPlayerEx(40,40,40,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhan')

 //Wave 2
 call InitAssaultGroup()
 call CampaignAttackerEx(4,5,8,KNIGHT)
 call SuicideOnPlayerEx(45,45,45,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(2,2,2, 'Rhst')

 //Wave 3
 call InitAssaultGroup()
 call CampaignAttackerEx(8,8,8, SORCERESS)
 call CampaignAttackerEx(0,0,3, HUMAN_DRAGON_HAWK)
 call SuicideOnPlayerEx(35,35,35,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(3,3,3, 'Rhme')
 call SetBuildUpgrEx(3,3,3, 'Rhar')

 //Wave 4
 call InitAssaultGroup()
 call CampaignAttackerEx(6,8,12, SORCERESS)
 call SuicideOnPlayerEx(30,30,30,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(3,3,3, 'Rhla')

 loop
 //Wave 5
 call InitAssaultGroup()
 call CampaignAttackerEx(4,6,5,KNIGHT)
 call CampaignAttackerEx(1,3,6,PRIEST)
 call SuicideOnPlayerEx(50,50,50,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(3,3,3, 'Rhra')

 //Wave 6
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,7,GRYPHON)
 call SuicideOnPlayerEx(55,55,55,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhrt')

 //Wave 7
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,6,FOOTMAN)
 call CampaignAttackerEx(4,4,4,RIFLEMAN)
 call CampaignAttackerEx(1,2,4,PRIEST)
 call CampaignAttackerEx(0,2,2,MORTAR)
 call SuicideOnPlayerEx(60,60,60,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhhb')

 //Wave 8
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,7,GRYPHON)
 call CampaignAttackerEx(4,4,7,KNIGHT)
 call SuicideOnPlayerEx(70,70,70,user)
 exitwhen AttackTier != Tier
 endloop
 exitwhen AttackTier != Tier
 endloop
endfunction

So we've added an attack wave function for every tier, and now we have just one thing left to do: choose the attack wave function according to the current tier. To do that we'll need, you guessed it, another infinite loop function. This one will call an attack wave function according to the value of the variable Tier, so it'll look like that:

 JASS:

Function AttackSwitch takes nothing returns nothing
 loop
 if Tier == 1 then
 call Tier1Waves()
 elseif Tier == 2 then
 call Tier2Waves()
 elseif Tier == 3 then
 call Tier3Waves()
 else
 call DisplayTextToPlayer(user,0,0,"Something's not right")
 endif
 //We don't need to call Sleep() here because every time one of
 //the attack wave functions end this function will immediately call another one.
 endloop
endfunction

That last bit over there is your debugging function in ai, and it does what the name suggests: it displays to a player (first argument) some text (last argument). The 2nd and 3rd argument can always stay at 0, as they don't affect the displayed text (AFAIK). DisplayTextToPlayer is especially useful to see where your script fails: put it right after  CampaignAI() to see if the game actually received the script, spam it in new functions to see where they fail. Go nuts.

Anyway, now that the attack strategy is done, your script is ready to go. So just for reference:

 JASS:

//GLOBAL DECELERATION
//------------------------------------------------
globals
 player user = Player(0)
 integer Tier = 1
endglobals

//BUILD PRIORITIES
//------------------------------------------------
function BuildOrder takes nothing returns nothing
 call SetBuildUnitEx( 1,1,1, 'hpea') // peasent
 call SetBuildUnitEx( 1,1,1, 'htow') // town hall
 call SetBuildUnitEx( 5,5,5, 'hpea') // peasent
 call SetBuildUnitEx( 1,1,1, 'hbar') // barracks
 call SetBuildUnitEx( 2,2,2, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'halt') // hero altar
 call SetBuildUnitEx( 8,8,8, 'hpea') // peasent
 call SetBuildUnitEx( 5,5,5, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'hlum') // lumber mill
 call SetBuildUnitEx( 1,1,1, 'hbla') // blacksmith

 if Tier > 1 then
 call SetBuildUnitEx( 1,1,1, 'hkee') // Keep upgrade
 call SetBuildUnitEx( 1,1,1, 'hars') // arcane sanctum
 endif

 call SetBuildUnitEx( 1,1,1, 'hvlt') // item shop
 call SetBuildUnitEx( 6,6,6, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'hwtw') // watch tower

 if Tier > 1 then
 call SetBuildUnitEx( 1,1,1, 'harm') // workshop
 call SetBuildUnitEx( 11,11,11, 'hhou') // farm
 call SetBuildUnitEx( 3,3,3, 'hwtw') // watch tower
 endif

 if Tier > 2 then
 call SetBuildUnitEx( 1,1,1, 'hcas') // Castle upgrade
 call SetBuildUnitEx( 1,1,1, 'hatw') // arcane tower
 call SetBuildUnitEx( 1,1,1, 'hgra') // gryphon aviery
 call SetBuildUnitEx( 1,1,1, 'hgtw') // guard tower
 call SetBuildUnitEx( 1,1,1, 'hctw') // canon tower
 call SetBuildUnitEx( 14,14,14, 'hhou') // farm
 endif
endfunction

//BASE DEFENSE
//------------------------------------------------
function CampaignDefenses takes nothing returns nothing
 if Tier == 1 then
 call CampaignDefenderEx( 2,3,4, 'hfoo') // footman
 call CampaignDefenderEx( 1,2,2, 'hrif') // rifleman
 elseif Tier == 2 then
 call CampaignDefenderEx( 1,2,3, 'hmpr') // priest
 call CampaignDefenderEx( 1,1,2, 'hrif') // rifleman
 call CampaignDefenderEx( 3,4,6, 'hfoo') // footman
 elseif Tier == 3 then
 call CampaignDefenderEx( 0,2,2, 'hgyr') // gyrocopter
 call CampaignDefenderEx( 2,3,4, 'hkni') // knight
 call CampaignDefenderEx( 2,3,2, 'hmpr') // priest
 call CampaignDefenderEx( 1,1,2, 'hsor') // sorceress
 call CampaignDefenderEx( 1,0,1, 'hgry') // gryphon rider
 endif
endfunction

//ATTACK WAVE STRATEGY
//------------------------------------------------

function Tier1Waves takes nothing returns nothing
 local integer AttackTier = Tier

 loop
 call SetBuildUpgrEx(1,1,1, 'Rhde')

 //Wave 1
 call InitAssaultGroup()
 call CampaignAttackerEx(3,4,6,FOOTMAN)
 call SuicideOnPlayerEx(15,15,15,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhra')
 call SetBuildUpgrEx(1,1,1, 'Rhla')

 //Wave 2
 call InitAssaultGroup()
 call CampaignAttackerEx(2,3,5,RIFLEMAN)
 call SuicideOnPlayerEx(15,15,15,user)
 exitwhen AttackTier != Tier

 loop
 //Wave 3
 call InitAssaultGroup()
 call CampaignAttackerEx(3,3,8,FOOTMAN)
 call CampaignAttackerEx(2,4,2,RIFLEMAN)
 call SuicideOnPlayerEx(20,20,20,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhme')
 call SetBuildUpgrEx(1,1,1, 'Rhar')

 //Wave 4
 call InitAssaultGroup()
 call CampaignAttackerEx(6,7,4,FOOTMAN)
 call CampaignAttackerEx(2,3,6,RIFLEMAN)
 call SuicideOnPlayerEx(25,25,25,user)
 exitwhen AttackTier != Tier
 endloop
 exitwhen AttackTier != Tier
 endloop
endfunction

function Tier2Waves takes nothing returns nothing
 local integer AttackTier = Tier
 //Research all upgrades of earlier tier.
 call SetBuildUpgrEx(1,1,1, 'Rhra')
 call SetBuildUpgrEx(1,1,1, 'Rhla')
 call SetBuildUpgrEx(1,1,1, 'Rhme')
 call SetBuildUpgrEx(1,1,1, 'Rhar')
 call SetBuildUpgrEx(1,1,1, 'Rhde')

 loop
 call SetBuildUpgrEx(1,1,1, 'Rhpt')

 //Wave 1
 call InitAssaultGroup()
 call CampaignAttackerEx(3,4,6,FOOTMAN)
 call CampaignAttackerEx(1,2,2,PRIEST)
 call SuicideOnPlayerEx(20,20,20,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhri')

 //Wave 2
 call InitAssaultGroup()
 call CampaignAttackerEx(5,5,8,RIFLEMAN)
 call SuicideOnPlayerEx(25,25,25,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(2,2,2, 'Rhme')
 call SetBuildUpgrEx(2,2,2, 'Rhar')
 call SetBuildUpgrEx(1,1,1, 'Rhst')

 //Wave 3
 call InitAssaultGroup()
 call CampaignAttackerEx(3,4,4,FOOTMAN)
 call CampaignAttackerEx(1,1,2,PRIEST)
 call CampaignAttackerEx(3,3,3,SORCERESS)
 call SuicideOnPlayerEx(30,35,35,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(2,2,2, 'Rhra')
 call SetBuildUpgrEx(2,2,2, 'Rhla')

 loop
 //Wave 4+
 call InitAssaultGroup()
 call CampaignAttackerEx(6,9,8,FOOTMAN)
 call CampaignAttackerEx(1,2,3,MORTAR)
 call SuicideOnPlayerEx(40,40,40,user)
 exitwhen AttackTier != Tier

 //Wave 5+
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,6,GYRO)
 call SuicideOnPlayerEx(30,30,30,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhcd')

 //Wave 6+
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,3,RIFLEMAN)
 call CampaignAttackerEx(0,2,4,SORCERESS)
 call CampaignAttackerEx(1,1,3,PRIEST)
 call SuicideOnPlayerEx(35,35,35,user)
 exitwhen AttackTier != Tier

 //Wave 7+
 call InitAssaultGroup()
 call CampaignAttackerEx(6,6,8,FOOTMAN)
 call CampaignAttackerEx(1,2,2,MORTAR)
 call CampaignAttackerEx(1,1,3,PRIEST)
 call SuicideOnPlayerEx(45,45,45,user)
 exitwhen AttackTier != Tier
 endloop
 exitwhen AttackTier != Tier
 endloop
endfunction

function Tier3Waves takes nothing returns nothing
 local integer AttackTier = Tier
 //Research all upgrades of earlier tier.
 call SetBuildUpgrEx(1,1,1, 'Rhpt')
 call SetBuildUpgrEx(1,1,1, 'Rhri')
 call SetBuildUpgrEx(2,2,2, 'Rhme')
 call SetBuildUpgrEx(2,2,2, 'Rhar')
 call SetBuildUpgrEx(1,1,1, 'Rhst')
 call SetBuildUpgrEx(2,2,2, 'Rhra')
 call SetBuildUpgrEx(2,2,2, 'Rhla')
 call SetBuildUpgrEx(1,1,1, 'Rhcd')

 loop
 call SetBuildUpgrEx(2,2,2, 'Rhpt')

 //Wave 1
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,7,FOOTMAN)
 call CampaignAttackerEx(1,2,2,PRIEST)
 call CampaignAttackerEx(0,0,2,TANK)
 call SuicideOnPlayerEx(40,40,40,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhan')

 //Wave 2
 call InitAssaultGroup()
 call CampaignAttackerEx(4,5,8,KNIGHT)
 call SuicideOnPlayerEx(45,45,45,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(2,2,2, 'Rhst')

 //Wave 3
 call InitAssaultGroup()
 call CampaignAttackerEx(8,8,8, SORCERESS)
 call CampaignAttackerEx(0,0,3, HUMAN_DRAGON_HAWK)
 call SuicideOnPlayerEx(35,35,35,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(3,3,3, 'Rhme')
 call SetBuildUpgrEx(3,3,3, 'Rhar')

 //Wave 4
 call InitAssaultGroup()
 call CampaignAttackerEx(6,8,12, SORCERESS)
 call SuicideOnPlayerEx(30,30,30,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(3,3,3, 'Rhla')

 loop
 //Wave 5
 call InitAssaultGroup()
 call CampaignAttackerEx(4,6,5,KNIGHT)
 call CampaignAttackerEx(1,3,6,PRIEST)
 call SuicideOnPlayerEx(50,50,50,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(3,3,3, 'Rhra')

 //Wave 6
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,7,GRYPHON)
 call SuicideOnPlayerEx(55,55,55,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhrt')

 //Wave 7
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,6,FOOTMAN)
 call CampaignAttackerEx(4,4,4,RIFLEMAN)
 call CampaignAttackerEx(1,2,4,PRIEST)
 call CampaignAttackerEx(0,2,2,MORTAR)
 call SuicideOnPlayerEx(60,60,60,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhhb')

 //Wave 8
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,7,GRYPHON)
 call CampaignAttackerEx(4,4,7,KNIGHT)
 call SuicideOnPlayerEx(70,70,70,user)
 exitwhen AttackTier != Tier
 endloop
 exitwhen AttackTier != Tier
 endloop
endfunction

function AttackLoop takes nothing returns nothing
 loop
 if Tier == 1 then
 call Tier1Waves()

 elseif Tier == 2 then
 call Tier2Waves()

 elseif Tier == 3 then
 call Tier3Waves()
 else
 call DisplayTextToPlayer(user,0,0,"Something's not right")
 endif
 endloop
endfunction

//COMMAND FUNCTIONS
//------------------------------------------------

function TierCondition takes nothing returns nothing
 if CommandsWaiting() > 0 then
 set Tier = GetLastCommand()
 call PopLastCommand()
 call InitBuildArray()
 call InitDefenseGroup()
 call BuildOrder()
 call CampaignDefenses()
 endif
endfunction

function ConditionLoop takes nothing returns nothing
 call TierCondition()
 call StaggerSleep(1,5)
 loop
 call TierCondition()
 call Sleep(5)
 endloop
endfunction

//MAIN FUNCTION
//------------------------------------------------
function main takes nothing returns nothing
 call CampaignAI('hhou',null)
 call DoCampaignFarms(false)

 call BuildOrder()
 call CampaignDefenses()

 call StartThread(function ConditionLoop)

 call AttackLoop()
endfunction

Now that we're done, import the script to your map, open it, punch "warpten" and "whosyourdaddy" on the keypad (that's right, I endorse cheating), and enjoy yourself!

What's that? The attack waves are too small? I know [evilgrin].

You might vaguely recall I talked about a bug which is not a syntax error at the beginning of the tutorial. Well, this is it:

The problem is that the attack waves are cut short, but what causes that? It can't be CampaignAttackerEx, because that function just assigns values to variables to be used in a thread opened by CampaignAI () at the start of the script, and we'll assume that Blizzard didn't mess up something so basic in its script, so what's left is the attack function itself - SuicideOnPlayerEx. Let's open it up. After opening a few wrapper functions we reach this:

 JASS:

function CommonSuicideOnPlayer takes boolean standard, boolean bldgs, integer seconds, player p, integer x, integer y returns nothing
 local integer save_peons

 if not PrepSuicideOnPlayer(seconds) then
 return
 endif

 set save_peons = campaign_wood_peons
 set campaign_wood_peons = 0

 loop
 //xxx
 if allow_signal_abort and CommandsWaiting() != 0 then
 call Trace("ABORT -- attack wave override\n")
 endif
 //xxx

 exitwhen allow_signal_abort and CommandsWaiting() != 0

 loop
 exitwhen allow_signal_abort and CommandsWaiting() != 0

 call FormGroup(5,true)
 exitwhen sleep_seconds <= 0
 call TraceI("waiting %d seconds before suicide\n",sleep_seconds) //xxx
 endloop

 if standard then
 if bldgs then
 exitwhen SuicidePlayer(p,sleep_seconds >= -60)
 else
 exitwhen SuicidePlayerUnits(p,sleep_seconds >= -60)
 endif
 else
 call AttackMoveXYA(x,y)
 endif

 call TraceI("waiting %d seconds before timeout\n",60+sleep_seconds) //xxx
 call SuicideSleep(5)
 endloop

 set campaign_wood_peons = save_peons
 set harass_length = 0

 call SuicideOnPlayerWave()
endfunction

Yep, it's complicated... But you don't need to understand all of it right now, so I'll cut to the chase: what happened can be described as a sort of thread desync. The ai builds the attacking units in one thread, and gets ready for the attack in a different thread. Because we specified such a small waiting time in SuicideOnPlayerEx, it just started the attack wave without waiting for all of the units. This is a part of what it does, and it's a good thing, too. Otherwise once you've destroyed one unit factory, the ai will stop attacking, because it wouldn't be able to build some of the units.

Fortunately for us, there is a very simple solution - making the waiting time longer, but as you move on to more complicated workflows, you'll find yourself in a need to maintain a lot of checks and balances in order to keep your ai in line. otherwise, you might find the ai stuck in some infinite loop (and you know you'll have plenty of those) where it does nothing. This brings me to the second golden rule of ai: thread carefully! Don't spam StartThread and use each thread wisely. In this ai we've created there's an entire thread designed to catch 2 commands, meaning it can run twice at bast in what can be an hour long game. This is fine in our simple example, but if you had another conditions you'd like to check periodically, that would be the place to put them. Also, make sure to make your threads work in harmony with one another.

This is the ai script with working waiting times between waves, and this time I assure you this really is a working version:

 JASS:

//GLOBAL DECELERATION
//------------------------------------------------
globals
 player user = Player(0)
 integer Tier = 1
endglobals

//BUILD PRIORITIES
//------------------------------------------------
function BuildOrder takes nothing returns nothing
 call SetBuildUnitEx( 1,1,1, 'hpea') // peasent
 call SetBuildUnitEx( 1,1,1, 'htow') // town hall
 call SetBuildUnitEx( 5,5,5, 'hpea') // peasent
 call SetBuildUnitEx( 1,1,1, 'hbar') // barracks
 call SetBuildUnitEx( 2,2,2, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'halt') // hero altar
 call SetBuildUnitEx( 8,8,8, 'hpea') // peasent
 call SetBuildUnitEx( 5,5,5, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'hlum') // lumber mill
 call SetBuildUnitEx( 1,1,1, 'hbla') // blacksmith

 if Tier > 1 then
 call SetBuildUnitEx( 1,1,1, 'hkee') // Keep upgrade
 call SetBuildUnitEx( 1,1,1, 'hars') // arcane sanctum
 endif

 call SetBuildUnitEx( 1,1,1, 'hvlt') // item shop
 call SetBuildUnitEx( 6,6,6, 'hhou') // farm
 call SetBuildUnitEx( 1,1,1, 'hwtw') // watch tower

 if Tier > 1 then
 call SetBuildUnitEx( 1,1,1, 'harm') // workshop
 call SetBuildUnitEx( 11,11,11, 'hhou') // farm
 call SetBuildUnitEx( 3,3,3, 'hwtw') // watch tower
 endif

 if Tier > 2 then
 call SetBuildUnitEx( 1,1,1, 'hcas') // Castle upgrade
 call SetBuildUnitEx( 1,1,1, 'hatw') // arcane tower
 call SetBuildUnitEx( 1,1,1, 'hgra') // gryphon aviery
 call SetBuildUnitEx( 1,1,1, 'hgtw') // guard tower
 call SetBuildUnitEx( 1,1,1, 'hctw') // canon tower
 call SetBuildUnitEx( 14,14,14, 'hhou') // farm
 endif
endfunction

//BASE DEFENSE
//------------------------------------------------
function CampaignDefenses takes nothing returns nothing
 if Tier == 1 then
 call CampaignDefenderEx( 2,3,4, 'hfoo') // footman
 call CampaignDefenderEx( 1,2,2, 'hrif') // rifleman
 elseif Tier == 2 then
 call CampaignDefenderEx( 1,2,3, 'hmpr') // priest
 call CampaignDefenderEx( 1,1,2, 'hrif') // rifleman
 call CampaignDefenderEx( 3,4,6, 'hfoo') // footman
 elseif Tier == 3 then
 call CampaignDefenderEx( 0,2,2, 'hgyr') // gyrocopter
 call CampaignDefenderEx( 2,3,4, 'hkni') // knight
 call CampaignDefenderEx( 2,3,2, 'hmpr') // priest
 call CampaignDefenderEx( 1,1,2, 'hsor') // sorceress
 call CampaignDefenderEx( 1,0,1, 'hgry') // gryphon rider
 endif
endfunction

//ATTACK WAVE STRATEGY
//------------------------------------------------

function Tier1Waves takes nothing returns nothing
 local integer AttackTier = Tier

 loop
 call SetBuildUpgrEx(1,1,1, 'Rhde')

 //Wave 1
 call InitAssaultGroup()
 call CampaignAttackerEx(3,4,6,FOOTMAN)
 call SuicideOnPlayerEx(M3,M3,M2,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhra')
 call SetBuildUpgrEx(1,1,1, 'Rhla')

 //Wave 2
 call InitAssaultGroup()
 call CampaignAttackerEx(2,3,5,RIFLEMAN)
 call SuicideOnPlayerEx(M4,M3,M3,user)
 exitwhen AttackTier != Tier

 loop
 //Wave 3
 call InitAssaultGroup()
 call CampaignAttackerEx(3,3,8,FOOTMAN)
 call CampaignAttackerEx(2,4,2,RIFLEMAN)
 call SuicideOnPlayerEx(M4,M3,M4,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhme')
 call SetBuildUpgrEx(1,1,1, 'Rhar')

 //Wave 4
 call InitAssaultGroup()
 call CampaignAttackerEx(6,7,4,FOOTMAN)
 call CampaignAttackerEx(2,3,6,RIFLEMAN)
 call SuicideOnPlayerEx(M5,M5,M4,user)
 exitwhen AttackTier != Tier
 endloop
 exitwhen AttackTier != Tier
 endloop
endfunction

function Tier2Waves takes nothing returns nothing
 local integer AttackTier = Tier
 //Research all upgrades of earlier tier.
 call SetBuildUpgrEx(1,1,1, 'Rhra')
 call SetBuildUpgrEx(1,1,1, 'Rhla')
 call SetBuildUpgrEx(1,1,1, 'Rhme')
 call SetBuildUpgrEx(1,1,1, 'Rhar')
 call SetBuildUpgrEx(1,1,1, 'Rhde')

 loop
 call SetBuildUpgrEx(1,1,1, 'Rhpt')

 //Wave 1
 call InitAssaultGroup()
 call CampaignAttackerEx(3,4,6,FOOTMAN)
 call CampaignAttackerEx(1,2,2,PRIEST)
 call SuicideOnPlayerEx(M3,M3,M3,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhri')

 //Wave 2
 call InitAssaultGroup()
 call CampaignAttackerEx(5,5,8,RIFLEMAN)
 call SuicideOnPlayerEx(M4,M4,M4,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(2,2,2, 'Rhme')
 call SetBuildUpgrEx(2,2,2, 'Rhar')
 call SetBuildUpgrEx(1,1,1, 'Rhst')

 //Wave 3
 call InitAssaultGroup()
 call CampaignAttackerEx(3,4,4,FOOTMAN)
 call CampaignAttackerEx(1,1,2,PRIEST)
 call CampaignAttackerEx(3,3,3,SORCERESS)
 call SuicideOnPlayerEx(M5,M5,M4,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(2,2,2, 'Rhra')
 call SetBuildUpgrEx(2,2,2, 'Rhla')

 loop
 //Wave 4+
 call InitAssaultGroup()
 call CampaignAttackerEx(6,9,8,FOOTMAN)
 call CampaignAttackerEx(1,2,3,MORTAR)
 call SuicideOnPlayerEx(M7,M7,M5,user)
 exitwhen AttackTier != Tier

 //Wave 5+
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,6,GYRO)
 call SuicideOnPlayerEx(M6,M6,M5,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhcd')

 //Wave 6+
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,3,RIFLEMAN)
 call CampaignAttackerEx(0,2,4,SORCERESS)
 call CampaignAttackerEx(1,1,3,PRIEST)
 call SuicideOnPlayerEx(M6,M6,M6,user)
 exitwhen AttackTier != Tier

 //Wave 7+
 call InitAssaultGroup()
 call CampaignAttackerEx(6,6,8,FOOTMAN)
 call CampaignAttackerEx(1,2,2,MORTAR)
 call CampaignAttackerEx(1,1,3,PRIEST)
 call SuicideOnPlayerEx(M7,M7,M7,user)
 exitwhen AttackTier != Tier
 endloop
 exitwhen AttackTier != Tier
 endloop
endfunction

function Tier3Waves takes nothing returns nothing
 local integer AttackTier = Tier
 //Research all upgrades of earlier tier.
 call SetBuildUpgrEx(1,1,1, 'Rhpt')
 call SetBuildUpgrEx(1,1,1, 'Rhri')
 call SetBuildUpgrEx(2,2,2, 'Rhme')
 call SetBuildUpgrEx(2,2,2, 'Rhar')
 call SetBuildUpgrEx(1,1,1, 'Rhst')
 call SetBuildUpgrEx(2,2,2, 'Rhra')
 call SetBuildUpgrEx(2,2,2, 'Rhla')
 call SetBuildUpgrEx(1,1,1, 'Rhcd')

 loop
 call SetBuildUpgrEx(2,2,2, 'Rhpt')

 //Wave 1
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,7,FOOTMAN)
 call CampaignAttackerEx(1,2,2,PRIEST)
 call CampaignAttackerEx(0,0,2,TANK)
 call SuicideOnPlayerEx(M4,M4,M5,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhan')

 //Wave 2
 call InitAssaultGroup()
 call CampaignAttackerEx(4,5,8,KNIGHT)
 call SuicideOnPlayerEx(M3,M4,M6,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(2,2,2, 'Rhst')

 //Wave 3
 call InitAssaultGroup()
 call CampaignAttackerEx(8,8,8, SORCERESS)
 call CampaignAttackerEx(0,0,3, HUMAN_DRAGON_HAWK)
 call SuicideOnPlayerEx(M5,M5,M5,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(3,3,3, 'Rhme')
 call SetBuildUpgrEx(3,3,3, 'Rhar')

 //Wave 4
 call InitAssaultGroup()
 call CampaignAttackerEx(6,8,12, SORCERESS)
 call SuicideOnPlayerEx(M3,M4,M4,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(3,3,3, 'Rhla')

 loop
 //Wave 5
 call InitAssaultGroup()
 call CampaignAttackerEx(4,6,5,KNIGHT)
 call CampaignAttackerEx(1,3,6,PRIEST)
 call SuicideOnPlayerEx(M4,M5,M5,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(3,3,3, 'Rhra')

 //Wave 6
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,7,GRYPHON)
 call SuicideOnPlayerEx(M4,M3,M5,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhrt')

 //Wave 7
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,6,FOOTMAN)
 call CampaignAttackerEx(4,4,4,RIFLEMAN)
 call CampaignAttackerEx(1,2,4,PRIEST)
 call CampaignAttackerEx(0,2,2,MORTAR)
 call SuicideOnPlayerEx(M5,M5,M6,user)
 exitwhen AttackTier != Tier

 call SetBuildUpgrEx(1,1,1, 'Rhhb')

 //Wave 8
 call InitAssaultGroup()
 call CampaignAttackerEx(4,4,7,GRYPHON)
 call CampaignAttackerEx(4,4,7,KNIGHT)
 call SuicideOnPlayerEx(M6,M6,M7,user)
 exitwhen AttackTier != Tier
 endloop
 exitwhen AttackTier != Tier
 endloop
endfunction

function AttackLoop takes nothing returns nothing
 loop
 if Tier == 1 then
 call Tier1Waves()

 elseif Tier == 2 then
 call Tier2Waves()

 elseif Tier == 3 then
 call Tier3Waves()
 else
 call DisplayTextToPlayer(user,0,0,"Something's not right")
 endif
 endloop
endfunction

//COMMAND FUNCTIONS
//------------------------------------------------

function TierCondition takes nothing returns nothing
 if CommandsWaiting() > 0 then
 set Tier = GetLastCommand()
 call PopLastCommand()
 call InitBuildArray()
 call InitDefenseGroup()
 call BuildOrder()
 call CampaignDefenses()
 endif
endfunction

function ConditionLoop takes nothing returns nothing
 call TierCondition()
 call StaggerSleep(1,5)
 loop
 call TierCondition()
 call Sleep(5)
 endloop
endfunction

//MAIN FUNCTION
//------------------------------------------------
function main takes nothing returns nothing
 call CampaignAI('hhou',null)
 call DoCampaignFarms(false)

 call BuildOrder()
 call CampaignDefenses()

 call StartThread(function ConditionLoop)

 call AttackLoop()
endfunction

This is it. I wanted to write a lot more concerning how ai fights and how to test your work, but the tutorial is long enough already.

###  What's next?
There's much more to learn about ai. For starters, I kept 2 minor faults in the ai for you to exercise on:

When upgrading a tier the ai will build an entirely different group of defenders, but the defenders of the old tier will stay in the base until killed or added to an attack wave. This is bad, and can potentially make the ai player reach the food cap. Find a way to dispose of those excess defenders.

Right now, the ai will research upgrades before building Attack waves units. Instead of that, try making the ai research after building the attack wave.
After you're done with that, you can go on to try reading the built-in melee ai scripts and those of more complicated campaign missions ai-wise, like "the culling" from RoC.

#### Attachments

Gates of the abyss.txt

 4.5 KB

 · Views: 675



AI tutorial sandbox.w3m

 118.6 KB

 · Views: 625




 Last edited: Mar 3, 2017

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Really amazing tutorial! There is so little information about the AI natives themselves that most people just ignore them. I love the idea of following the campaign as an example--it reminds me of when I first learned JASS.

Great detail, great examples, and the tutorial itself is pretty fun and entertaining to read.

I learned quite a bit from reading it. I'm sure others will too. Approved!

#### Map Designer

#### Map Designer

 Level 17

JoinedMay 2, 2011Messages1,351

Resources: 6

#### Resources
Maps 2Spells and Systems 4

Hello,

If there is no replacement, does that make AI replace indefinitely or does it make never replace? I noticed Grom AI at chapter 3 does not have that function at all

//============================================================================

// Orc 3 -- Grom Ally -- AI Script

//============================================================================

globals

 constant integer GO_AGRO = 1 // no data

 constant integer GO_KILL = 2 // no data

 constant integer PLAYER_DIED = 3 // data = player ID

 constant integer PLAYER_ASS = 4 // no data

 constant integer CLEAR_AGRO = 5 // data = player ID

 constant integer USER = 0

 constant integer BLUE = 1

 constant integer GRAY = 8

 constant integer LIGHT_BLUE = 9

 constant integer GREEN = 10

 constant integer EASY_AGRO = 120

 constant integer NORMAL_AGRO = 120

 constant integer HARD_AGRO = 120

 integer grom_target = -1

 integer wave_index = 0

 integer strength = 1

 boolean agro_mode = true

 boolean array alive

 boolean array needs_agro

endglobals

//============================================================================

// set_build_units

//============================================================================

function set_build_units takes boolean fplayer returns nothing

 if not fplayer then

 call SetBuildUnit( 1, PEON )

 call SetBuildUnit( 1, GREAT_HALL )

 call SetBuildUnit( 1, ORC_BARRACKS )

 call SetBuildUnit( 1, STRONGHOLD )

 call SetBuildUnit( 1, ORC_ALTAR )

 call SetBuildUnit( 1, FORGE )

 call SetBuildUnit( 1, BESTIARY )

 call SetBuildUnit( 7, PEON )

 else

 call SetBuildUnit( 2, ORC_BARRACKS )

 call SetBuildUnit( 2, BESTIARY )

 call SetBuildUnit( 4, ORC_WATCH_TOWER )

 endif

endfunction

//============================================================================

// set_defenders

//============================================================================

function set_defenders takes boolean fplayer returns nothing

 if not fplayer then

 call CampaignDefenderEx( 1,1,1, GROM )

 call CampaignDefenderEx( 2,2,2, GRUNT )

 call CampaignDefenderEx( 2,2,2, HEAD_HUNTER )

 call CampaignDefenderEx( 4,4,4, RAIDER )

 else

 call CampaignDefenderEx( 2,2,2, GRUNT )

 call CampaignDefenderEx( 1,1,1, HEAD_HUNTER )

 call CampaignDefenderEx( 1,1,2, RAIDER )

 endif

endfunction

//============================================================================

// assault_wave

//============================================================================

function assault_wave takes nothing returns nothing

 //------------------------------------------------------------------------

 if grom_target == USER then

 //------------------------------------------------------------------------

 call CampaignAttackerEx ( 3,3,4, GRUNT )

 call CampaignAttackerEx ( 3,3,4, HEAD_HUNTER )

 call CampaignAttackerEx ( 1,1,2, CATAPULT )

 call CampaignAttackerEx ( 2,2,4, RAIDER )

 call SuicideOnPlayer(M5,Player(grom_target))

 //------------------------------------------------------------------------

 elseif strength == 1 then

 //------------------------------------------------------------------------

 call CampaignAttackerEx ( 4,4,5, GRUNT )

 call SuicideOnPlayer(M5,Player(grom_target))

 set strength = 2

 //------------------------------------------------------------------------

 elseif strength == 2 then

 //------------------------------------------------------------------------

 call CampaignAttackerEx ( 3,3,4, GRUNT )

 call CampaignAttackerEx ( 2,2,2, HEAD_HUNTER )

 call SuicideOnPlayer(M5,Player(grom_target))

 set strength = 3

 //------------------------------------------------------------------------

 else // strength >= 3

 //------------------------------------------------------------------------

 call CampaignAttackerEx ( 3,3,4, RAIDER )

 call CampaignAttackerEx ( 2,2,2, HEAD_HUNTER )

 call SuicideOnPlayer(M5,Player(grom_target))

 set strength = 1

 endif

endfunction

//============================================================================

// agro_wave

//============================================================================

function agro_wave takes nothing returns nothing

 //------------------------------------------------------------------------

 if strength==1 then

 //------------------------------------------------------------------------

 call CampaignAttackerEx ( 4,4,5, GRUNT )

 call SuicideOnPlayer(0,Player(grom_target))

 //------------------------------------------------------------------------

 elseif strength==2 then

 //------------------------------------------------------------------------

 call CampaignAttackerEx ( 3,3,4, GRUNT )

 call CampaignAttackerEx ( 3,3,4, HEAD_HUNTER )

 call SuicideOnPlayer(M3,Player(grom_target))

 //------------------------------------------------------------------------

 else // strength >= 3

 //------------------------------------------------------------------------

 call CampaignAttackerEx ( 3,3,4, GRUNT )

 call CampaignAttackerEx ( 3,3,4, HEAD_HUNTER )

 call CampaignAttackerEx ( 3,3,4, RAIDER )

 call SuicideOnPlayer(M3,Player(grom_target))

 endif

endfunction

//============================================================================

// init_arrays

//============================================================================

function init_arrays takes nothing returns nothing

 local integer index = 0

 loop

 set alive [index] = false

 set needs_agro [index] = false

 set index = index + 1

 exitwhen index == 11

 endloop

 set alive [ BLUE ] = true

 set alive [ GRAY ] = true

 set alive [ LIGHT_BLUE ] = true

 set alive [ GREEN ] = true

 set needs_agro [ BLUE ] = true

 set needs_agro [ GRAY ] = true

 set needs_agro [ LIGHT_BLUE ] = true

 set needs_agro [ GREEN ] = true

endfunction

//============================================================================

// wait_for_start

//============================================================================

function wait_for_start takes nothing returns nothing

 loop

 call Trace("waiting for first command...\n")

 exitwhen CommandsWaiting() != 0

 call Sleep(5)

 endloop

 call TraceI("...first command (%d) received.\n",GetLastCommand())

endfunction

//============================================================================

// possible_agro

//============================================================================

function possible_agro takes integer target returns nothing

 if grom_target == -1 and alive[target] and needs_agro[target] then

 set grom_target = target

 set needs_agro[target] = false

 call TraceI("NOTICE: SET NEEDS_AGRO[%d] = FALSE\n",target)

 endif

endfunction

//============================================================================

// next_alive

//============================================================================

function next_alive takes nothing returns nothing

 loop

 set grom_target = wave_index

 set wave_index = wave_index + 1

 if wave_index == 11 then

 set wave_index = 0

 call Sleep(1)

 endif

 exitwhen alive[grom_target]

 endloop

 call TraceI("Grom setting normal attack wave target = %d\n",grom_target)

endfunction

//============================================================================

// go_agro

//============================================================================

function go_agro takes nothing returns nothing

 if grom_target != -1 then

 call Trace("Grom successful, sleeping for a while\n")

 if difficulty==EASY then

 call Sleep(EASY_AGRO)

 elseif difficulty==NORMAL then

 call Sleep(NORMAL_AGRO)

 else

 call Sleep(HARD_AGRO)

 endif

 set grom_target = -1

 set strength = 1

 endif

 call possible_agro( BLUE )

 call possible_agro( GRAY )

 call possible_agro( LIGHT_BLUE )

 call possible_agro( GREEN )

 call TraceI("changing agro target to %d\n",grom_target)

endfunction

//============================================================================

// process_commands

//============================================================================

function process_commands takes nothing returns nothing

 local integer cmd

 local integer data

 loop

 exitwhen CommandsWaiting() == 0

 set cmd = GetLastCommand()

 set data = GetLastData()

 call PopLastCommand()

 call TraceI("COMMAND = %d\n",cmd)

 call TraceI("DATA = %d\n",data)

 //====================================================================

 if cmd == GO_AGRO then

 //====================================================================

 call go_agro()

 //====================================================================

 elseif cmd == GO_KILL then

 //====================================================================

 call Trace("agro waves complete, starting assault waves\n")

 set agro_mode = false

 set strength = 1

 //====================================================================

 elseif cmd == PLAYER_DIED then

 //====================================================================

 call TraceI("NOTICE: TOWN %d JUST DIED\n",data)

 set alive[data] = false

 //====================================================================

 elseif cmd == PLAYER_ASS then

 //====================================================================

 call Trace("player gonna get punished now!\n")

 call set_build_units(true)

 call set_defenders(true)

 set alive[USER] = true // ha ha!

 set wave_index = USER

 //====================================================================

 elseif cmd == CLEAR_AGRO then

 //====================================================================

 if data == grom_target then

 call TraceI("player agro'd Grom's target (%d) first\n",data)

 call go_agro()

 else

 call TraceI("player agro'd %d (not Grom's current target)\n",data)

 endif

 call TraceI("NOTICE: SET NEEDS_AGRO[%d] = FALSE\n",data)

 set needs_agro[data] = false

 //====================================================================

 else // UNKNOWN COMMAND

 //====================================================================

 call TraceI("WARNING: UNKNOWN COMMAND (%d)\n",cmd)

 endif

 endloop

endfunction

//============================================================================

// agro_loop

//============================================================================

function agro_loop takes nothing returns nothing

 loop

 call process_commands()

 exitwhen not agro_mode

 if grom_target == -1 then

 call Trace("ERROR: Grom has no agro target!\n")

 return

 endif

 call InitAssaultGroup()

 call CampaignAttacker( EASY, 1, GROM )

 call agro_wave()

 set strength = strength + 1

 endloop

endfunction

//============================================================================

// wave_loop

//============================================================================

function wave_loop takes nothing returns nothing

 loop

 call process_commands()

 call next_alive()

 call InitAssaultGroup()

 call CampaignAttacker( EASY, 1, GROM )

 call assault_wave()

 endloop

endfunction

//============================================================================

// main

//============================================================================

function main takes nothing returns nothing

 call CampaignAI(BURROW,null)

 call init_arrays()

 call set_build_units(false)

 call set_defenders(false)

 call wait_for_start()

 call agro_loop()

 call wave_loop()

endfunction

EDIT: I found that grom AI does replace at least 4 times. But how can this be inferred from the code while AI does not have any replace function written?


 Last edited: Jun 11, 2017

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

I think you're misunderstanding the concept of replacement a bit. The AI operates using 2 unit groups: one for attack (the attack group) and one for defense (the defense group), both with a very creative and original name. Each unit group has "infinite replacements" - as long as the AI player can build this unit, and it's missing, it will be built.

The whole functionality of replacements is relevant to pre-placed or trigger generated units, as well as guard posts. I won't talk about guard posts now because I haven't tested it enough myself, but there is an additional tutorial about JASS AI you can refer to. When you pre-place units owned by the AI player in your map (i.e. place them in the map editor), they are considered guard posts. These are the units which SetReplacements refers to. You can actually see that very well in the mission gates of the abyss which I used as a code example. When you get to the dreanai village side quest, the village is under attack by orc units generated by triggers, this is how the trigger looks like:

Unit - Create 1 Fel Orc Grunt for Player 5 (Yellow) at (Center of Mid Orc 01 <gen>) facing 180.00 degrees

AI - Ignore (Last created unit)'s guard position

Unit Group - Add (Last created unit) to MidCinOrcGroup

Unit - Create 1 Fel Orc Grunt for Player 5 (Yellow) at (Center of Mid Orc 02 <gen>) facing 180.00 degrees

AI - Ignore (Last created unit)'s guard position

Unit Group - Add (Last created unit) to MidCinOrcGroup

Unit - Create 1 Fel Orc Raider for Player 5 (Yellow) at (Center of Mid Orc 03 <gen>) facing 180.00 degrees

AI - Ignore (Last created unit)'s guard position

Unit Group - Add (Last created unit) to MidCinOrcGroup

Unit - Create 1 Fel Orc Warlock for Player 5 (Yellow) at (Center of Mid Orc 04 <gen>) facing 180.00 degrees

AI - Ignore (Last created unit)'s guard position
Note the "ignore unit's guard position" action, it tells the map script to ignore the generated unit as a guard post, so these units will not be replaced when they die.


 Last edited: Jan 10, 2018

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Very useful, it's pretty complicated though

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

If you make JASS AI scripts, I found a tool that helps debug it. Can't remember the exact name, but I recall it was @Darky29 tool.

EDIT :

JASS Syntax Checker 0.1.7

#### Standhaft

#### Standhaft

 Level 9

JoinedDec 2, 2015Messages254

Resources: 1

#### Resources
Maps 1

I skimmed this and it looks like somewhere to start for me on learning how to make AI for my custom campaign.

However, in the start you mention requiring some basic knowledge which I don't really have. Is there somewhere even noobier for me to start?

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

Standhaft said:

 However, in the start you mention requiring some basic knowledge which I don't really have. Is there somewhere even noobier for me to start?


Click to expand...

That depends on what you want to make. If you want to create an AI with JASS, then you really should know a little JASS. But you will be creating something veeery simple, so you need to know very little, and even knowing the bare basics from a different programming language will be enough.

You could read moyackx's toturial linked above, it follows the template of Blizzard's scripts more closely, but you will still need the same level of JASS (which is again very basic). The downside is that you'll be limited to this exact same template and won't be able to do much else.

Alternatively, you can work with the AI editor, but I would really suggest against it, because it hides most of the work from you and uses the same base for both melee and campaign ai created by it (which is not good). Not to mention that it is even more limited than usual.

In short I suggest you open some tutorial, or JASS class, get to know variables, functions, if statements and loops. After that, try reading this tutorial or moyackx's one and see how that goes.

#### Standhaft

#### Standhaft

 Level 9

JoinedDec 2, 2015Messages254

Resources: 1

#### Resources
Maps 1

I want to make a campaign of comparable quality to the official campaign so I think I need to learn a lot.

I'll see what I can figure out from your advice and the guides. Thanks.

#### Astrella

#### Astrella

 Level 8

JoinedOct 3, 2008Messages189

Resources: 1

#### Resources
Maps 1

Love how detailed this is considering how little documentation there is about the AI.

How would you approach your two little questions at the end out of curiosity?

1. Extract the current defense group and have an option attack wave that adds them? So you suicide the attackers away? Not sure if it's possible to add prebuilt units to an attack wave.

2. Have a seperate thread that handles research?

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

Thanks, always nice to see people using this tutorial.

About the questions:

1.

Yes that is exactly what I'd do. Moreover, the AI will always use whatever unit is available for an attack wave, especially prebuilt ones which are in the defense group.

You can see this behavior very easily. Open any base building campaign map and look at the enemy base over a while (iseedeadpeople for the win, always cheat). You'll notice that there are units standing around a base in a group, and every once in a while a part of them will leave the group and wait somewhere else. This is where the attack group gathers. From there (after maybe more units are built) those units will go and attack your base.

2.

Using another thread for research alone is very wasteful, and a direct violation of the "thread carefully" rule. My point was that if the AI switches tiers before finishing all of that tiers upgrades, those upgrades are "dumped" at the start of the next tier waves function, which can potentially jam the next attack wave (if a bunch of updates are started on unit buildings, you might not be able to create units).

A possible solution is to create an array for upgrades, filling it a little bit more at the start of each wave function, then starting a set amount of updates (usually 1 or 2) between waves, going up the array. That is a general solution that should work well, albeit making for slower updates).

#### Standhaft

#### Standhaft

 Level 9

JoinedDec 2, 2015Messages254

Resources: 1

#### Resources
Maps 1

Hi, so I am back after a long break from WC3. Reforged has reinvigorated my interest and now I would like to try to learn this whole AI thing again. My understanding is that the same editor is being used so that this tutorial still applies. (Please confirm if this is not the case.)

Nowow said:

 A basic understanding of JASS/other programming language. Nothing too major, but you should know what a function is, global and local variables, looping, etcetera...


Click to expand...

I have no idea what JASS is. Given that, would you suggest that I start somewhere else? Where would I learn about JASS that is most related to WC3 map/campaign creation?

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

Standhaft said:

 I have no idea what JASS is. Given that, would you suggest that I start somewhere else? Where would I learn about JASS that is most related to WC3 map/campaign creation?


Click to expand...

Err... seriously? JASS is quite a big part of what people are doing here. To put it simply, JASS is a programming language created by blizzard which allows us to interact with the game engine. If you created a trigger, what happens behind the scene is that the world editor converts that trigger into a script which it can run. So any JASS you learn will be most related to mapmaking.

There is a multitude of tutorials for JASS. in-fact, this tutorial is in the JASS/AI script section. To name a few:

JASS: Moving From GUI to Jass, the Start

JASS Tutorial

JASS: A Concise Introduction

JASS: A Better Understanding

Learning JASS

And there are more, all around the section. Even more so, since JASS is a programming language, most of it's basic structures (controlled execution of if statements and loops, functions, variables etc.) are very much similar to those of most known programming language (mostly C and Python come to mind). The good thing about that is that those languages provide great documentation, maybe better than the hive. So, you can go learn the start of a real programming language (which will do you good anyways) then come back here.

JASS and JASS AI are not going away anytime soon, they're too ingrained in the community (you can learn LUA and use it instead though), no fears there.

#### tlstkwjr

#### tlstkwjr

 Level 7

JoinedMay 29, 2013Messages152

This is a very helpful explanation.

But I have a question, How can preplace units or units summoned by abilities be included in an attack group?

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

For summoned units, you can add the line call groupTimedLife(true) at the start of your script, in the main function. This automatically adds all timed life summons to your attack group, this means all summoned units, but not possessed or charmed units (if that's really important to you, there's a workaround talked about here).

IIRC if you want preplaced units to behave like normal AI units and not stand at some location use a trigger on them, explained here:

Nowow said:

 I think you're misunderstanding the concept of replacement a bit. The AI operates using 2 unit groups: one for attack (the attack group) and one for defense (the defense group), both with a very creative and original name. Each unit group has "infinite replacements" - as long as the AI player can build this unit, and it's missing, it will be built.

The whole functionality of replacements is relevant to pre-placed or trigger generated units, as well as guard posts. I won't talk about guard posts now because I haven't tested it enough myself, but there is an additional tutorial about JASS AI you can refer to. When you pre-place units owned by the AI player in your map (i.e. place them in the map editor), they are considered guard posts. These are the units which SetReplacements refers to. You can actually see that very well in the mission gates of the abyss which I used as a code example. When you get to the dreanai village side quest, the village is under attack by orc units generated by triggers, this is how the trigger looks like:

Unit - Create 1 Fel Orc Grunt for Player 5 (Yellow) at (Center of Mid Orc 01 <gen>) facing 180.00 degrees

AI - Ignore (Last created unit)'s guard position

Unit Group - Add (Last created unit) to MidCinOrcGroup

Unit - Create 1 Fel Orc Grunt for Player 5 (Yellow) at (Center of Mid Orc 02 <gen>) facing 180.00 degrees

AI - Ignore (Last created unit)'s guard position

Unit Group - Add (Last created unit) to MidCinOrcGroup

Unit - Create 1 Fel Orc Raider for Player 5 (Yellow) at (Center of Mid Orc 03 <gen>) facing 180.00 degrees

AI - Ignore (Last created unit)'s guard position

Unit Group - Add (Last created unit) to MidCinOrcGroup

Unit - Create 1 Fel Orc Warlock for Player 5 (Yellow) at (Center of Mid Orc 04 <gen>) facing 180.00 degrees

AI - Ignore (Last created unit)'s guard position
Note the "ignore unit's guard position" action, it tells the map script to ignore the generated unit as a guard post, so these units will not be replaced when they die.


Click to expand...

If you want them to stand at some location, then I don't think they will join the attack group, but I'm not sure. you should probably test it.

#### tlstkwjr

#### tlstkwjr

 Level 7

JoinedMay 29, 2013Messages152

Thanks for the reply. It was really helpful and my AI work is progressing.

But I have another questions. On my map, the tower of the enemy player is guard by the creep. When AI attacking this base, AI ignores creep. And it only attacks towers. Is there an native to control this?

And when setting up an attack group, how do I set the maximum and minimum unit numbers rather than the fixed numbers?

also Can I toggle AI's formation movement?


 Last edited: Mar 31, 2020

#### Wayshan

#### Wayshan

 Level 11

JoinedOct 28, 2012Messages230

Resources: 1

#### Resources
Maps 1

Thank you so much!

I have two questions:

1) In the Send AI Command there are two variables; however I have never seen set the DataInteger variable to anything else than zero. What is it for?

2) Let's say I want to keep the tier scaling. But I also want the AI to attack a certain unit at some point. Or even start attacking it in looped waves. How do I do that?

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

tlstkwjr said:

 Thanks for the reply. It was really helpful and my AI work is progressing.

But I have another questions. On my map, the tower of the enemy player is guard by the creep. When AI attacking this base, AI ignores creep. And it only attacks towers. Is there an native to control this?

And when setting up an attack group, how do I set the maximum and minimum unit numbers rather than the fixed numbers?

also Can I toggle AI's formation movement?


Click to expand...

That depends. If the creeps really are creeps (neutral player), then the attack group is supposed to focus the tower first (this is what it is attacking), and then the creeps (if the creeps are attacking your units). You can attack creep camps of the neutral player with a different function: GetCreepCamp(min, max, flyers), you just don't know which camp you'll get. This function gets a creep camp between the min and max levels, with or without flyers (your choice). Regarding movement formation, I don't think you can change it.

For a random amount of units in an attack, you can simply replace a fixed number of an amount with GetRandomInt(min, max).

Wayshan said:

 Thank you so much!

I have two questions:

1) In the Send AI Command there are two variables; however I have never seen set the DataInteger variable to anything else than zero. What is it for?

2) Let's say I want to keep the tier scaling. But I also want the AI to attack a certain unit at some point. Or even start attacking it in looped waves. How do I do that?


Click to expand...

1) The DataInteger is useful when you're trying to implement more complex commands. For example, You want an AI which will attack a specific player under one command, hold a point of your choice on a different command, and hold in the base in a third. In this case, the CommandInteger would indicate which command it is, and the DataInteger would supply additional data, for example:

- command: attack player. data: 4 => attack player 4

- command: hold in base. data: 2 => hold in base for 2 minutes

- command: hold point X/Y. data: 1740 => hold point with X/Y coordinate value.

In this simple case, I didn't want to introduce more than necessary, so I didn't use the DataInteger. In fact, it would be "better" to use only the data integer in this tutorial as well, because we only send one command (change tier), while changing the data (tier number).

2. Just like SuicideOnPlayerEx, there are SuicideOnPointEx and SuicideOnUnitEx which you can use. See here.

#### tlstkwjr

#### tlstkwjr

 Level 7

JoinedMay 29, 2013Messages152

Thank you very much. I have too many questions, but I hope you understand. I was very interested in AI, but I couldn't try because there are no wise users like you.

Anyway, using call AnyPlayerAttack() to build the artificial intelligence I want. When using this function, I like all the movements of AI, but the problem is that it is too slow.

I want to speed up the interval at which they act and the interval at which units are produced.

What can I make so if I tweak it? Can you provide an analysis for this function?

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

tlstkwjr said:

 Thank you very much. I have too many questions, but I hope you understand. I was very interested in AI, but I couldn't try because there are no wise users like you.

Anyway, using call AnyPlayerAttack() to build the artificial intelligence I want. When using this function, I like all the movements of AI, but the problem is that it is too slow.

I want to speed up the interval at which they act and the interval at which units are produced.

What can I make so if I tweak it? Can you provide an analysis for this function?


Click to expand...

 JASS:

function AnyPlayerAttack takes nothing returns nothing
 local unit hall

 set hall = GetEnemyExpansion()
 if hall == null then
 call StartGetEnemyBase()
 loop
 exitwhen not WaitGetEnemyBase()
 call SuicideSleep(1)
 endloop
 set hall = GetEnemyBase()
 endif

 call SetAllianceTarget(hall)
 call FormGroup(3,true)
 call AttackMoveKillA(hall)
endfunction

Before AnyPlayerAttack actually sends out units using AttackMoveKillA, it does 3 things:

1. Tries to get an enemy hall unit in a loop.

2. Sets the hall as an alliance target.

3. calls FormGroup.

You say this function works too slowly for you, so you should see which of those 3 parts takes the most time before sending out the troops. SetAllianceTarget pings the unit on the map, and works instantly, so it's one of the other two. You can copy the whole AnyPlayerAttack function to your script and add debug statements to see which part is the slowest. Maybe something like this:

 JASS:

function AnyPlayerAttack takes nothing returns nothing
 local unit hall

 set hall = GetEnemyExpansion()
 call DisplayTextToPlayer(PUT_USER_HERE,0,0,"AnyPlayerAttack::getting enemy base")
 if hall == null then
 call StartGetEnemyBase()
 loop
 exitwhen not WaitGetEnemyBase()
 call SuicideSleep(1)
 endloop
 set hall = GetEnemyBase()
 endif

 call SetAllianceTarget(hall)
 call DisplayTextToPlayer(PUT_USER_HERE,0,0,"AnyPlayerAttack::enemy base found, starting FormGroup")
 call FormGroup(3,true)
 call DisplayTextToPlayer(PUT_USER_HERE,0,0,"AnyPlayerAttack::FormGroup done, attacking!")
 call AttackMoveKillA(hall)
endfunction

I will say this, though: If I recall correctly from playing, the AI works slowly, and it feels even slower when you test it. There are things like building units (as you noticed), which are simply not done in a smart way in the AI: When building units, the AI doesn't queue them. So let's say you want to build 4 ghouls and 2 banshees, instead of building the ghouls and the banshees at the same time, the AI will first build 4 ghouls, then 2 banshees (or the other way around). Needless to say, this takes a lot more time.

So I'm not sure there's something to do to speed up the AI.

If the problem really is in AnyPlayerAttack, you can maybe find an enemy hall in advance, in a different thread for example, if part 1 of AnyPlayerAttack is the problem, or try to speed up FormGroup somehow if part 3 of AnyPlayerAttack is the problem.

#### tlstkwjr

#### tlstkwjr

 Level 7

JoinedMay 29, 2013Messages152

In the meantime, when there was ai, a crash occurred. so I stopped working for a while.

But I found the cause. When using anyplayerattack(), if the owner of the townhall type building is changed, there is a possibility of causing a crash. be careful when using it.

 - command: attack player. data: 4 => attack player 4

- command: hold in base. data: 2 => hold in base for 2 minutes

- command: hold point X/Y. data: 1740 => hold point with X/Y coordinate value.


Click to expand...

and now I am wondering how to give ai a command to attack a certain point like this quote. can you give me an concrete example? How can I indicate the coordinates?


 Last edited: May 17, 2020

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

tlstkwjr said:

 In the meantime, when there was ai, a crash occurred. so I stopped working for a while.

But I found the cause. When using anyplayerattack(), if the owner of the townhall type building is changed, there is a possibility of causing a crash. be careful when using it.


Click to expand...

You mean like changing with triggers? I can see how that can cause problems.

tlstkwjr said:

 and now I am wondering how to give ai a command to attack a certain point like this quote. can you give me an concrete example? How can I indicate the coordinates?


Click to expand...

You can see here an example script which handles multiple commands and acts in accordance. Sending the actual coordinates from the map to the script is done with triggers, for example an ability directed to a point. Only note is you get coordinates as real numbers but you should convert them to integers.

#### Map Designer

#### Map Designer

 Level 17

JoinedMay 2, 2011Messages1,351

Resources: 6

#### Resources
Maps 2Spells and Systems 4

Nowow said:

 JASS:

function CampaignAI takes integer farms, code heroes returns nothing
 if GetGameDifficulty() == MAP_DIFFICULTY_EASY then
 set difficulty = EASY

 call SetTargetHeroes(false)
 call SetUnitsFlee(false)

 elseif GetGameDifficulty() == MAP_DIFFICULTY_NORMAL then
 set difficulty = NORMAL

 call SetTargetHeroes(false)
 call SetUnitsFlee(false)

 elseif GetGameDifficulty() == MAP_DIFFICULTY_HARD then
 set difficulty = HARD

 call SetPeonsRepair(true)
 else
 set difficulty = INSANE
 endif

 call InitAI()
 call InitBuildArray()
 call InitAssaultGroup()
 call CreateCaptains()

 call SetNewHeroes(false)
 if heroes != null then
 call SetHeroLevels(heroes)
 endif

 call SetHeroesFlee(false)
 call SetGroupsFlee(false)
 call SetSlowChopping(true)
 call GroupTimedLife(false)
 call SetCampaignAI()
 call Sleep(0.1)

 set racial_farm = farms
 call StartThread(function CampaignBasics)
 call StartBuildLoop()
endfunction

Click to expand...

NICE

I have been looking for this for sometimes to know what is the difference between hard and medium difficulty.

#### DEXTER01

#### DEXTER01

 Level 3

JoinedMar 8, 2020Messages20

Two questions:

1:I understand that the 3 numbers represents difficulty but why sometimes looks like this

call SetBuildUnit ( 8, CHAOS_PEON )

It has only one difficulty (I believe the easy one) ,what the difference between that one and this one

call SetBuildUnit ( 8,9,10,CHAOS_PEON )

And number 2:

//* WAVE 1 *

 call InitAssaultGroup()

 call CampaignAttackerEx( 1,1,1, ORC_DRAGON )

 call SuicideOnPlayer(M5,user)

 //* WAVE 2 *

 call InitAssaultGroup()

 call CampaignAttackerEx( 3,3,5, CHAOS_GRUNT )

 call CampaignAttackerEx( 1,1,1, CHAOS_GROM )

 call SuicideOnPlayerEx(M6,M6,M5,user)

In the first wave there is only one reference "M5" in the second wave there are 3 "M6,M6,M5" why is that as well , I tried țo modified that line by adding additional references like

/* WAVE 1 *

 call InitAssaultGroup()

 call CampaignAttackerEx( 1,1,1, ORC_DRAGON )

 call SuicideOnPlayer(M5,M4,M3,user)

I tested the map to see if its any differente and in my surprise the ai does not do anything except for gathers all of the peons to gold mine

Ty for your time!


 Last edited: Apr 13, 2021

#### Map Designer

#### Map Designer

 Level 17

JoinedMay 2, 2011Messages1,351

Resources: 6

#### Resources
Maps 2Spells and Systems 4

DEXTER01 said:

 Two questions:

1:I understand that the 3 numbers represents difficulty but why sometimes looks like this

call SetBuildUnit ( 8, CHAOS_PEON )

It has only one difficulty (I believe the easy one) ,what the difference between that one and this one

call SetBuildUnit ( 8,9,10,CHAOS_PEON )


Click to expand...

Hello,

as far as I know, setBuildUnit has one integer input and does not depend on difficulty, while setBuildUnitEx has 3 integer inputs, and the integer used will depend on game difficulty.

you can see how each function work in the jass manual JASS Manual: API Browser - common.ai

DEXTER01 said:

 In the first wave there is only one reference "M5" in the second wave there are 3 "M6,M6,M5" why is that as well , I tried țo modified that line by adding additional references like


Click to expand...

the answer is the same to question 1:

Suicide takes input minutes between waves, regardless of difficulty. It will work the same for Easy, normal hard.

SuicideEx Takes 3 input integers, and it will select depending on difficulty.

Note that the trigger below is wrong

DEXTER01 said:

 call SetBuildUnit ( 8,9,10,CHAOS_PEON )


Click to expand...

You should make it SetBuildUnitEx instead.

#### DEXTER01

#### DEXTER01

 Level 3

JoinedMar 8, 2020Messages20

Map Designer said:

 Hello,

as far as I know, setBuildUnit has one integer input and does not depend on difficulty, while setBuildUnitEx has 3 integer inputs, and the integer used will depend on game difficulty.

you can see how each function work in the jass manual JASS Manual: API Browser - common.ai

the answer is the same to question 1:

Suicide takes input minutes between waves, regardless of difficulty. It will work the same for Easy, normal hard.

SuicideEx Takes 3 input integers, and it will select depending on difficulty.

Note that the trigger below is wrong

You should make it SetBuildUnitEx instead.


Click to expand...

Map Designer said:

 Hello,

as far as I know, setBuildUnit has one integer input and does not depend on difficulty, while setBuildUnitEx has 3 integer inputs, and the integer used will depend on game difficulty.

you can see how each function work in the jass manual JASS Manual: API Browser - common.ai

the answer is the same to question 1:

Suicide takes input minutes between waves, regardless of difficulty. It will work the same for Easy, normal hard.

SuicideEx Takes 3 input integers, and it will select depending on difficulty.

Note that the trigger below is wrong

You should make it SetBuildUnitEx instead.


Click to expand...

Ohh Ty very much now I understand!

#### Gnuoy

#### Gnuoy

 Level 23

JoinedMar 16, 2008Messages1,043

Resources: 3

#### Resources
Maps 3

is there a way to convert GUI AI script to JASS, as a starting template? i'm fairly happy with the GUI script but I just want to change some hero/item behaviors.

Also, is there a way I could just import all the object data to the JASS script instead of typing it all out?


 Last edited: Oct 7, 2021

#### Shawwy

#### Shawwy

 Level 3

JoinedMar 6, 2022Messages20

@Nowow how can you make the ai build on a specific location.

the townshiftspot function is currently not working for me.

#### Acolite333

#### Acolite333

 Level 6

JoinedDec 11, 2014Messages93

I was wondering if I could get some help making an AI ally help defend another AI ally when their base is attacked.

I have a human AI and Nightelf AI on the same team, and I want to make it so that when one of them gets attacked, the other will send units (preferably their defenders) to help protect their allies base. How could I go about doing that?

#### Gnuoy

#### Gnuoy

 Level 23

JoinedMar 16, 2008Messages1,043

Resources: 3

#### Resources
Maps 3

 But I found the cause. When using anyplayerattack(), if the owner of the townhall type building is changed, there is a possibility of causing a crash. be careful when using it.


Click to expand...

So i'm changing the owner of the AI's heroes to a seperate computer player that is not running a script, to get them to use vendors with triggers. I'm wondering if this could cause a crash and if there's any condition I could add to prevent that. Maybe current order = nothing?

#### Acolite333

#### Acolite333

 Level 6

JoinedDec 11, 2014Messages93

Nowow said:

 When upgrading a tier the ai will build an entirely different group of defenders, but the defenders of the old tier will stay in the base until killed or added to an attack wave. This is bad, and can potentially make the ai player reach the food cap. Find a way to dispose of those excess defenders.


Click to expand...

Nowow said:

1.

Yes that is exactly what I'd do. Moreover, the AI will always use whatever unit is available for an attack wave, especially prebuilt ones which are in the defense group.

You can see this behavior very easily. Open any base building campaign map and look at the enemy base over a while (iseedeadpeople for the win, always cheat). You'll notice that there are units standing around a base in a group, and every once in a while a part of them will leave the group and wait somewhere else. This is where the attack group gathers. From there (after maybe more units are built) those units will go and attack your base.

Click to expand...

How would one go about doing this without messing with any prior attack waves? Could you post an example script of where it would fit in nicely and exactly what commands you'd need to do in order to do it?

I'm not entirely sure how to get that included in here to be honest, still a bit of a novice

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

You would definitely mess the next attack wave, as you will add more units to it.

I won't give you a neat script, but I will give you some more hints on how to do that:

1. I suggest creating a function to handle this (one function with an if block to set different tiers, or two functions).

2. You need to add defending units only when changing a tier, so however you choose to add them, you only need to do that for the first wave in each tier.

[[this is the important one]]

3. The function CampaignAttackerEx, updates a set of arrays starting with harass_ (I'll leave you to understand how it works). You need to update those arrays, either through more calls to CampaignAttackerEx or directly.

Good luck

#### Shawwy

#### Shawwy

 Level 3

JoinedMar 6, 2022Messages20

i am using a custom townhall and after the townhalls upgrades to tier 2 the ai orders to build another tier one townhall help

#### Gnuoy

#### Gnuoy

 Level 23

JoinedMar 16, 2008Messages1,043

Resources: 3

#### Resources
Maps 3

Use a condition that adds all 3 tiers of those builds together. There may be a better solution out there.

I can elaborate and give an example of such a condition later.

@Shawwy UPDATE:

something like this. This is pretty much how the GUI AI Editor generates it:

 JASS:

globals /// this creates the boolean variable
 boolean gCond_Need_Hall = false
endglobals

function UpdateConditions takes nothing returns nothing /// this is defining the condition
 set gCond_Needs_Hall = ( ( GetUnitCount( CUSTOM_HALL_1 ) + ( GetUnitCount( CUSTOM_HALL_2 ) + GetUnitCount( CUSTOM_HALL_1 ) ) ) == 0 ) /// change this number to the max amount of expansions. seems limitless can crash game??? idk
endfunction

... /// in your build priorities
 if gCond_Need_Hall then
 call BuildExpansion (CUSTOM_HALL_1, CUSTOM_HALL_1) /// this is the function I currently uses but seems to cause a crash sometimes.
 endif /// good luck figuring out exactly the proper way to make a melee type AI to build an expansion.
 /// use whatever expansion function you think is best here is my point.
...

The update condition is part of other conditions which are Threads, which seem to be looping functions that run as long as the script is.

"call StartThread( function NAME )"

I think for the 2nd and 3rd tier you won't need conditions and the AI can figure it out without crashing. Not 100% sure.

Also this slightly different method explained by Warseeker seems to accomplish the same thing:

###
 [JASS] - Campaign AI, weird issue with custom buildings order


As far as I know, AI has issue with custom Town Hall and all. With classic buildings, if you order: call SetBuildUnit(1, TOWN_HALL) call SetBuildUnit(1, KEEP) call SetBuildUnit(1, CASTLE) It will build a single Town Hall and then upgrade it until Castle. But, if you do the same with custom...

 www.hiveworkshop.com


With all that being said, idk if this is compatible with the AI in this tutorial. I'd assume it is.


 Last edited: Aug 5, 2022

#### komarovba

#### komarovba

 Level 18

JoinedSep 1, 2018Messages157

Resources: 29

#### Resources
Models 24Icons 2Ported Icons 3

Good afternoon!

I created a new thread and added improvements to it, and I can't understand why it doesn't work! There is also a feeling that the condition variables are not updated. Please tell me what I'm doing wrong? Thank you in advance for your help.

#### Attachments

LudiT1_jass.ai

 39.2 KB

 · Views: 99



#### Gnuoy

#### Gnuoy

 Level 23

JoinedMar 16, 2008Messages1,043

Resources: 3

#### Resources
Maps 3

komarovba said:

 Good afternoon!

I created a new thread and added improvements to it, and I can't understand why it doesn't work! There is also a feeling that the condition variables are not updated. Please tell me what I'm doing wrong? Thank you in advance for your help.


Click to expand...

Things like 'hhou' and 'hbar' are already defined in the blizzard.j file or blizzard.ai file, or one of those, so if you define them again then it will bug out the script. Maybe there's some other problem but that's the first thing I noticed.

#### komarovba

#### komarovba

 Level 18

JoinedSep 1, 2018Messages157

Resources: 29

#### Resources
Models 24Icons 2Ported Icons 3

Gnuoy said:

 Things like 'hhou' and 'hbar' are already defined in the blizzard.j file or blizzard.ai file, or one of those, so if you define them again then it will bug out the script. Maybe there's some other problem but that's the first thing I noticed.


Click to expand...

The script does not give an error. He works. But for some reason the third thread does not work out and for some reason the redefinition of variables does not work.

#### Gnuoy

#### Gnuoy

 Level 23

JoinedMar 16, 2008Messages1,043

Resources: 3

#### Resources
Maps 3

Get rid of all the redefinitions and let me know if there are further problems, if that wasn't clear from my previous post.

#### komarovba

#### komarovba

 Level 18

JoinedSep 1, 2018Messages157

Resources: 29

#### Resources
Models 24Icons 2Ported Icons 3

Good afternoon. I made your recommendations, but unfortunately it didn't work. The third thread, namely UpdateForgeAssignment, never started. He starts running it very late, although there are opportunities to start doing call SetBuildUpgr( 1, UPG_MELEE ) much earlier! Please tell me what I'm doing wrong.

#### Attachments

LudiT1_jass.ai

 39.2 KB

 · Views: 78



#### komarovba

#### komarovba

 Level 18

JoinedSep 1, 2018Messages157

Resources: 29

#### Resources
Models 24Icons 2Ported Icons 3

I also checked. It ignores the UpdateConditions method. Namely, it does not update the gCond_Pehotinec variable, according to the logic of which, it should stop hiring footmen if there is a keep. In a regular AI editor, this logic worked, but I can't understand why it doesn't work if I start writing in jass. Tell me please.

#### komarovba

#### komarovba

 Level 18

JoinedSep 1, 2018Messages157

Resources: 29

#### Resources
Models 24Icons 2Ported Icons 3

now I have been experimenting for a long time and came to a sad conclusion for myself. For some reason, the AI can't run multiple infinite threads. When running in the main function ,

 call StartThread( function UpdateForgeAssignment )

 call StartThread( function WorkerBuildPriorities )

 call StartThread( function WorkerAssignment )

 call StartThread( function StartHarvestPriorities )

 call StartThread( function AttackAssignment )

In which there are infinite loops, it runs only the first 3-4. The rest are just ignored. This is very sad, I will think about another solution.

#### Gnuoy

#### Gnuoy

 Level 23

JoinedMar 16, 2008Messages1,043

Resources: 3

#### Resources
Maps 3

I'm really busy IRL and can't get into it sorry. Maybe try making a forum post.

If the thread is running but it stops after 3-4 times then it makes me think the AI is not taking those actions, building/workers/attacking, because of some other reason like insufficient resources or something.

#### Shawwy

#### Shawwy

 Level 3

JoinedMar 6, 2022Messages20

How to stop ghouls from defending a base and make them focus on harvesting lumber.

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

Resource gathering is kind of scuffed in the AI. You could use the HarvestWood(0, X) to make sure there are at least X ghouls harvesting lumber (if you have them trained and alive). Of course, if you want to change that number you'll need to call HarvestWood again with a new number.

You can see how this works in the common.ai file, and follow how the variable campaign_wood_peons changes.

#### Shawwy

#### Shawwy

 Level 3

JoinedMar 6, 2022Messages20

I have a problem with my script

 JASS:

//GLOBAL DECELERATION
//------------------------------------------------
globals
 player EnemyA = Player(0)

 //Units
 //------------------------------------------------
 //Crimson_Wth_Pyromancer
 integer WorkerA = 'hpea'

 integer Crusader = 'h60P'
 integer Hunter = 'h60R'
 integer Leutenant = 'h60Q'

 integer Conqueror = 'h60S'
 integer Merciless = 'h60T'

 integer Catapult = 'o600'
 integer WarGolem = 'n602'

 integer RedPriest = 'n60L'
 integer Conjurer = 'u600'
 integer Firemage = 'n60H'
 integer FlameKeeper = 'n60N'

 //Buildings

 integer THa = 'htow'
 integer THb = 'hkee'
 integer THc = 'hcas'

 integer Farm = 'hhou'
 integer BlackSmith = 'hbla'
 integer Mill = 'hlum'
 integer ItemShop = 'hvlt'

 integer BarracksA = 'hbar'
 integer BarracksB = 'hars'
 integer BarracksC = 'harm'
 integer BarracksD = 'hgra'

 integer Altar = 'halt'

 integer Exemplar = 'h618'

 integer TowerA = 'hwtw'
 integer TowerB = 'hgtw'
 integer TowerC = 'hctw'
 integer TowerD = 'hatw'

 //Heroes
 integer HeroA = 'Hpb1'
 integer HeroB = 'Hgam'



endglobals

//BUILD PRIORITIES
//------------------------------------------------
function BuildOrder takes nothing returns nothing

 //Tier 1
 call SetBuildUnitEx( 1,1,1, WorkerA)
 call SetBuildUnitEx( 1,1,1, THa)
 call SetBuildUnitEx( 5,5,5, WorkerA)
 call SetBuildUnitEx( 2,2,2, BarracksA)
 call SetBuildUnitEx( 2,2,2, Farm)
 call SetBuildUnitEx( 2,2,2, TowerA)
 call SetBuildUnitEx( 1,1,1, Altar)
 call SetBuildUnitEx( 9,9,9, WorkerA)
 call SetBuildUnitEx( 1,1,1, BlackSmith)
 call SetBuildUnitEx( 5,5,5, Farm)
 call SetBuildUnitEx( 1,1,1, Mill)
 call SetBuildUnitEx( 4,4,4, TowerA)

 //Tier 2
 call SetBuildUnitEx( 1,1,1, THb)
 call SetBuildUnitEx( 2,2,2, BarracksB)
 call SetBuildUnitEx( 1,1,1, ItemShop)
 call SetBuildUnitEx( 6,6,6, Farm)
 call SetBuildUnitEx( 2,2,2, TowerB)
 call SetBuildUnitEx( 2,2,2, BarracksC)
 call SetBuildUnitEx( 10,10,10, Farm)

 //Tier 3
 call SetBuildUnitEx( 1,1,1, THc)
 call SetBuildUnitEx( 1,1,1, Exemplar)
 call SetBuildUnitEx( 14,14,14, Farm)
 call SetBuildUnitEx( 6,6,6, TowerA)
 call SetBuildUnitEx( 2,2,2, TowerC)
 call SetBuildUnitEx( 2,2,2, TowerD)
endfunction

//BASE DEFENSE
//------------------------------------------------
function CampaignDefenses takes nothing returns nothing
 call CampaignDefenderEx( 1, 1, 1, Leutenant)
 call CampaignDefenderEx( 3, 3, 3, Crusader)
 call CampaignDefenderEx( 3, 3, 3, Conqueror)
 call CampaignDefenderEx( 2, 2, 2, Merciless)
 call CampaignDefenderEx( 2, 2, 2, FlameKeeper)
 call CampaignDefenderEx( 2, 2, 2, RedPriest)
 call CampaignDefenderEx( 2, 2, 2, Firemage)
 call CampaignDefenderEx( 3, 3, 3, Hunter)
 call CampaignDefenderEx( 1, 1, 1, HeroA)
 call CampaignDefenderEx( 1, 1, 1, HeroB)
endfunction

//COMMAND FUNCTIONS
//------------------------------------------------

//MAIN FUNCTION
//------------------------------------------------
function main takes nothing returns nothing
 call CampaignAI(Farm,null)
 call DoCampaignFarms(false)
 call SetReplacements(1,1,1)
 call GroupTimedLife(true)
 call SetPeonsRepair(true)
 call SetSlowChopping(true)
 set campaign_wood_peons = 4

 call BuildOrder()
 call CampaignDefenses()

endfunction

Let's say i have 2 production buildings and one preplaced crusader it dies

the ai will attempt to replace but using the production buildings but instead of queuing once it ended queueing on both productions building leaving me with an extra crusader

How could i fix this

#### Gnuoy

#### Gnuoy

 Level 23

JoinedMar 16, 2008Messages1,043

Resources: 3

#### Resources
Maps 3

Shawwy said:

 How could i fix this


Click to expand...

Try adding a condition for the production building.

Something like this?

 JASS:

function UpdateConditions takes nothing returns nothing
 set gCond_Needs_Prod_Bldg = ( ( GetUnitCount( PRODUCTION_BUILDING ) == 0 ) )
endfunction

 JASS:

 if gCond_Needs_Prod_Bldg = true then
 call SetBuildAll( BUILD_UNIT, 1, PRODUCTION_BUILDING, -1 )
 endif

#### Shawwy

#### Shawwy

 Level 3

JoinedMar 6, 2022Messages20

how do i detect an event when multiple barracks are queuing the same unit or a preplaced unit dies

and what if it needs more than one barracks for example 2 crusaders die so it needs only 2 barracks to queue

And i will restate my problem using chatgpt because my english is not that great

#### Shawwy

#### Shawwy

 Level 3

JoinedMar 6, 2022Messages20

Did anyone did any testing to the SetReplacements native?

And I could not fix my problem still

Could be this line of code I aint sure tho

 JASS:

function OneBuildLoop takes nothing returns nothing
 local integer index = 0
 local integer qty
 local integer id
 local integer tp

 set total_gold = GetGold() - gold_buffer
 set total_wood = GetWood()

 loop
 exitwhen index == build_length

 set qty = build_qty [index]
 set id = build_item[index]
 set tp = build_type[index]

 //--------------------------------------------------------------------
 if tp == BUILD_UNIT then
 if not StartUnit(qty,id,build_town[index]) then
 return
 endif

 //--------------------------------------------------------------------
 elseif tp == BUILD_UPGRADE then
 call StartUpgrade(qty,id)

 //--------------------------------------------------------------------
 else // tp == BUILD_EXPAND
 if not StartExpansion(qty,id) then
 return
 endif
 endif

 set index = index + 1
 endloop
endfunction

#### Gnuoy

#### Gnuoy

 Level 23

JoinedMar 16, 2008Messages1,043

Resources: 3

#### Resources
Maps 3

Not sure if this answers your question but I think SetReplacement only works in "campaign" type AIs, which is what this guide is for.

#### Superfrycook

#### Superfrycook

 Level 47

JoinedApr 12, 2019Messages832

Resources: 291

#### Resources
Packs 1Skins 1Maps 29Reforged HD Models 231Reforged HD Skins 18Reforged HD Icons 8Sounds 3

##
 Republic Anthology


What is M2,M3,M4 anyway? Player color?

 You must log in or register to reply here.


### Similar threads

D

Locked

[JASS]
 AI script won't run


Deleted member 238589

Jul 8, 2018

Triggers & Scripts

Replies4Views2K

Jul 8, 2018
Deleted member 238589

D

Locked

 why my ai not working?


Trktnor

Oct 1, 2021

World Editor Help Zone

Replies3Views1K

Oct 2, 2021
Trktnor

 Create an AI Final Wave


Griffith

Oct 16, 2025

World Editor Help Zone

Replies9Views788

Oct 17, 2025
GIMLI_2

Locked

 AI not atacking


Abextadoo

Jul 22, 2013

World Editor Help Zone

Replies0Views1K

Jul 22, 2013
Abextadoo

Locked

[JASS]
 Difficulty AI failure


RexusMax52

Oct 7, 2021

Triggers & Scripts

Replies2Views2K

Oct 10, 2021
RexusMax52

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