---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/intermidiate-ai-concepts-boring-no-longer.294890/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Intermidiate AI Concepts Boring No Longer
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/intermidiate-ai-concepts-boring-no-longer.294890/>
#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

Intermediate AI workflow concepts - boring no longer

If you've played a campaign or two from this site, you'd know that the AI in most maps is made the same way: there's a base with some defenses, which will occasionally send out groups of units to your base. And that's about it. This causes numerous problems, mainly being extremely boring, as well as restricting difficulty variations to the size and frequency of the attack waves.

With this tutorial, we will hopefully be able to break the mold, and create a more diverse range of AI scripts. To do that, we will cover the nitty-gritty aspects of how AI moves units, and build upon that several simplistic scripts to answer certain map scenarios.

What you'll need for this tutorial:

First of all, you must read and understand my previous tutorial about creating an AI script. This is not just an advertisement (but seriously it's great read it here), it is necessary to understand the bare basics of how an AI is created.

Basic trigger work knowledge:Some of the AI script/map interaction is done via command triggers.

An investigative disposition and being ready to test things. A lot of the conclusions presented in this tutorial are not very well documented (yet...), and are built on testing and experimentation. Keep exploring, and exceed your own expectations!
Notes from the last tutorial

To start off, I want to clear a few things regarding threads and their execution from the last tutorial. If you'll recall the ultra hi-res figure about thread execution from the previous tutorial, you'll see that threads are generally independent from one another. The exception to that rule is global variable - which can carry changes in execution across different threads. We saw a small example for that in the AI script created in the previous tutorial, where changing the Tier global variable changed how the AI attacked, and what it built and rebuilt in the town. This might not tell you much right now, but we'll see how the changing of global variables become immensely important when creating a more reactive AI script.

Another thing which should be stressed is how the AI processes commands, and the limitations derived from it. Commands are stored as a stack (for more on that see this explanation). In short, what this means is that if you send 3 commands at the same time to the AI script, you will only be able to access the last one. This can be seen very well by the names of the command natives:

 JASS:

native GetLastCommand takes nothing returns integer
// Returns the command number of the last received command (i.e. top of the stack)
native GetLastData takes nothing returns integer
// Returns the data number of the last received command (i.e. top of the stack)
native PopLastCommand takes nothing returns nothing
// Removes the command at the top of the stack, allowing you to access older commands

Another limitation of the commands is that there is a maximum amount of 12 commands which can wait at the stack. That means that if you send more than 12 commands to the AI script at the same time, or forget to delete old processed commands, your AI will stop taking orders at some point. Due to those two limitations we will strive to follow two rules:

State your business: the script should know what each command is supposed to do, independent of other commands.

Kill the stragglers: every processed command is dead and done, and should be removed with PopLastCommand. This allows you to access other commands if they were given, and keeps the command stack empty.

On to the main show: moving units!

Captains and Unit groups

Barring worker units and some special cases, the AI moves units using the attack and defense captains. A captain, according to this source:

 Captains are special invisible widgets that the hard coded AI uses to coordinate movement. In game terms they are widgets that are neither items, destructables nor units.


Click to expand...

* I generally suggest reading all of that thread after the tutorial and see that you understand all of the concepts presented in it.

Now, what AI captains do is act as a middleman to coordinate the actions of unit groups. There are two AI captains: attack captain and defense captain. Each AI captain has a bunch of units assigned to it: the attack group assigned to the attack captain, and the defense group assigned to the defense captain. When the AI script sends an order for units to attack something, it actually sends the captain widget there, and the units follow.

We will soon test some captain AI natives in order to get to know them better, but first we will examine how captains work in something that you're familiar with: an attack wave of a campaign AI. The normal attack function of a campaign AI is SuicideOnPlayerEx, which eventually calls this function:

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

First, you can see just how versatile CommonSuicideOnPlayer can be - it can be used to attack an enemy base, enemy units, or a specific point on the map. And looking a bit closer at this section, we can also see what makes it so robust:

 JASS:

 if standard then
 if bldgs then
 exitwhen SuicidePlayer(p,sleep_seconds >= -60)
 else
 exitwhen SuicidePlayerUnits(p,sleep_seconds >= -60)
 endif
 else
 call AttackMoveXYA(x,y)
 endif

We can now see that what changes the target of the attack functions changes due to calling a different native. But before we continue, it might be prudent to examine CommonSuicideOnPlayer more in depth:

The first thing you'll notice is that SuicideOnPlayerEx, as well as several other unused attack functions eventually call CommonSuicideOnPlayer - this is our starting point, no matter the target, and we'll see the reason for that soon. Another thing to note is the heavy use of the global variable sleep_seconds: this variable acts as a time manager during the control flow of the attacking function via the function SuicideSleep, which acts exactly like a normal sleep function while keeping track of the sleep_seconds variable.

At the start CommonSuicideOnPlayer calls another function,  PrepSuicideOnPlayer. In this function, the AI performs an estimate check of the amount of time it will take to build all of the attacking units in a function named PrepTime. It does so by iterating over the all attacking units and returning the unit it will take the most time to build. Then, PrepSuicideOnPlayer adds the amount of time we gave to this wave to sleep_seconds, and if that amount of time is longer than the needed time to build the wave, the thread will sleep until the wave is just about built.

There's another very important thing PrepSuicideOnPlayer does, which is to check if there are even any units specified for this wave. This actually dictates if the function returns true or false, but the way it does it is even more interesting for us. Take a look at the use of save_length - it stores harass_length and sets it to 0 at the start of the function, then after the whole waiting section, it sets harass_length back to the stored integer. The reason the script is built that way is security. We need to make sure there are actually units in the wave we build, hence the if statement at the end of PrepSuicideOnPlayer. But what if someone reset harrass_length in a different thread? It is a global variable, after all, and can be changed from anywhere in the script. Storing its value in a local variable actually protects the value from outside influence until the time is right, restricting access to the value of harrass_length to the scope of the function only.

Straight after PrepSuicideOnPlayer we can see another case of saving a variable, this time the amount of wood gatharers. This value will be restored only later along the control flow, which begs the questions: why is it necessary? And how does set campaign_wood_peons = 0 affect wood gathering for the AI player? Both these questions will be answered by creating a test map later on. Our next point of interest is the double nested loop:

 JASS:

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

First of all, let's look at this line, which is going to pop up quite a lot:

 JASS:

exitwhen allow_signal_abort and CommandsWaiting() != 0

This is our first exit condition. The campaign attack function utilizes this line in a number of cases, most notably in defense missions: once there are a few minutes left to defend, and it's time to full on suicide with all units. Such AIs have the variable allow_signal_abort set to true at the start. Then, as long as they have a command waiting, all attack waves will be cancelled prematurely. This is used to jumpstart the full on suicide (see attached script from "March of the Scourge" mission).

Now, to the inner loop in CommonSuicideOnPlayer: it calls the function FormGroup continuously until sleep_seconds <= 0 (from that we can infer that the function FormGroup affects sleep_seconds). We will not dig deep into how FormGroup  works (yet...). For now, suffice to say it assigns units to the attack captain, and finishes when the attack group is ready or something unexpected happens (like the group being attacked). Once FormGroup is done, and we have our attack group, the order to attack is sent via one of the natives we have already seen, and the saved value of campaign_wood_peons is restored.

After all of that, there is still a single function left to unravel: SuicideOnPlayerWave, but this one is actually much simpler then you'd think, all it does is make sure something actually happened with the captain: either every unit died, the captain got into a fight, or it just took to long for him to find something to kill, and an error happened.

With that, we are done explaining CommonSuicideOnPlayer. It's quite a bit to digest, so let's reiterate the important points:

1. We learned how to secure the value of a global variable (confining to a local variable then reverting back).

2. We learned to escort and check our attacks on each step - from time estimates to making sure the captain found its target.

3. We found some delicious new natives to test

Now, back to the natives. And oh boy have we got a lot of those:

 JASS:

native CreateCaptains takes nothing returns nothing
native SetCaptainHome takes integer which, real x, real y returns nothing
native SuicidePlayer takes player id, boolean check_full returns boolean
native SuicidePlayerUnits takes player id, boolean check_full returns boolean
native CaptainInCombat takes boolean attack_captain returns boolean
native ResetCaptainLocs takes nothing returns nothing
native ShiftTownSpot takes real x, real y returns nothing
native TeleportCaptain takes real x, real y returns nothing
native ClearCaptainTargets takes nothing returns nothing
native CaptainAttack takes real x, real y returns nothing
native CaptainVsUnits takes player id returns nothing
native CaptainVsPlayer takes player id returns nothing
native CaptainGoHome takes nothing returns nothing
native CaptainIsHome takes nothing returns boolean
native CaptainIsFull takes nothing returns boolean
native CaptainIsEmpty takes nothing returns boolean
native CaptainGroupSize takes nothing returns integer
native CaptainReadiness takes nothing returns integer
native CaptainRetreating takes nothing returns boolean
native CaptainReadinessHP takes nothing returns integer
native CaptainReadinessMa takes nothing returns integer
native CaptainAtGoal takes nothing returns boolean
...

As you can see, there are quite a lot of those, which will make testing them quite a chore. So let's see if we can notice anything peculiar. So, for starters, what bugged me most is this: the function SetCaptainHome takes the argument integer which. Looking at the global variables in the common.ai file (yep, that awful thing) it's clear which refers to which captain's home you want to set:

 JASS:

 constant integer ATTACK_CAPTAIN = 1
 constant integer DEFENSE_CAPTAIN = 2
 constant integer BOTH_CAPTAINS = 3

That's all fair and good, you can set the home location (we'll get to what that means later) of the attack captain, the defense captain, or both at the same time. The thing that bothers me here is that almost no other captain native takes such argument!

And this is the first thing you should know about captains: most native function to the attack captain only - this is going to be our main tool for moving groups of units. With that in mind, we will take a bunch of AI natives regarding captains, and test each function in the following method:

Assume total ignorance of the native function and it's use.

Search for references to said native function in composite AI functions (like CommonSuicideOnPlayer) and blizzard AI scripts (see bottom of the post).

Using the references, form an idea of what might that native function do.

Test assumptions in our on test map (basically the scientific method).

To Be Continued

This is it for now. Work on the tutorial will (hopefully) continue come September. In the mean time, you can post suggestions or view some AI test maps I created along the last couple of months.

Checking how DoCampaignFarms works in two races.

 JASS:

//==================================================================================================
// BUILD CAMPAIGN FARMS TEST - HUMAN
//==================================================================================================

//--------------------------------------------------------------------------------------------------
// GLOBAL DECLERATION
//--------------------------------------------------------------------------------------------------

globals
 player user = Player(0)
 integer i = 1
endglobals

function BuildPriorities takes nothing returns nothing
 call SetBuildUnitEx( 1,1,1, 'hpea' )
 call SetBuildUnitEx( 1,1,1, 'htow' )
 call SetBuildUnitEx( 5,5,5, 'hpea' )
 call SetBuildUnitEx( 1,1,1, 'hbar' )
 call SetBuildUnitEx( 2,2,2, 'hhou' )
 call SetBuildUnitEx( 1,1,1, 'halt' )
 call SetBuildUnitEx( 8,8,8, 'hpea' )
 call SetBuildUnitEx( 5,5,5, 'hhou' )
 call SetBuildUnitEx( 1,1,1, 'hlum' )
 call SetBuildUnitEx( 1,1,1, 'hbla' )
 call SetBuildUnitEx( 1,1,1, 'hvlt' )
 call SetBuildUnitEx( 6,6,6, 'hhou' )
 call SetBuildUnitEx( 1,1,1, 'hwtw' )
endfunction

function main takes nothing returns nothing
 call CampaignAI('hhou', null)
 call DoCampaignFarms(false)
 call SetHarvestLumber(true)
 call SetReplacements(3,3,3)

 call Sleep(2)

 call BuildPriorities()

 call WaitForSignal()

 call DoCampaignFarms(true)

 loop
 call WaitForSignal()

 call CampaignAttackerEx( i,i,i, 'hfoo')
 set i = i + 1
 endloop
endfunction

 JASS:

//==================================================================================================
// BUILD CAMPAIGN FARMS TEST - ORC
//==================================================================================================

//--------------------------------------------------------------------------------------------------
// GLOBAL DECLERATION
//--------------------------------------------------------------------------------------------------

globals
 player user = Player(0)
 integer i = 1
endglobals

function BuildPriorities takes nothing returns nothing
 call SetBuildUnitEx( 1,1,1, 'ogre')
 call SetBuildUnitEx( 1,1,1, 'opeo')
 call SetBuildUnitEx( 2,2,2, 'opeo')
 call SetBuildUnitEx( 3,3,3, 'opeo')
 call SetBuildUnitEx( 4,4,4, 'opeo')
 call SetBuildUnitEx( 5,5,5, 'opeo')
 //call SetBuildUnitEx( 6,6,6, 'opeo')
 call SetBuildUnitEx( 1,1,1, 'oalt')
 call SetBuildUnitEx( 1,1,1, 'obar')
 call SetBuildUnitEx( 1,1,1, 'otrb')
 //call SetBuildUnitEx( 7,7,7, 'opeo')
 call SetBuildUnitEx( 2,2,2, 'otrb')
 //call SetBuildUnitEx( 8,8,8, 'opeo')
 call SetBuildUnitEx( 1,1,1, 'ofor')
endfunction

function main takes nothing returns nothing
 call CampaignAI('otrb', null)
 call DoCampaignFarms(false)
 call SetHarvestLumber(true)
 call SetReplacements(3,3,3)

 call Sleep(2)

 call BuildPriorities()

 call WaitForSignal()

 call DoCampaignFarms(true)

 loop
 call WaitForSignal()

 call CampaignAttackerEx( i,i,i, 'ogru')
 set i = i + 1
 endloop
endfunction

An attempt to better understand how units are added to each captain, and to see if the Attack Captain can be emptied. A test map with a different version of this script is attached at the bottom of the post.

 JASS:

//=================================================================================================
// EMPTY ATTACK GROUP TEST
//=================================================================================================

globals
 player user = Player(0)

 //DEFENSE HOME
 constant real DEFX = -3200.0
 constant real DEFY = -4900.0

 constant real DEFX2 = -4000.0
 constant real DEFY2 = -6750.0

 //ATTACK HOME
 constant real ATTX = -2250.0
 constant real ATTY = -5900.0

 //ATTACK TARGET
 constant real TARX = -2500.0
 constant real TARY = -4500.0
endglobals

//-------------------------------------------------------------------------------------------------
// BUILD ORDER
//-------------------------------------------------------------------------------------------------

function BuildOrder takes nothing returns nothing
 call SetBuildUnitEx( 1, 1, 1, TOWN_HALL )
 call SetBuildUnitEx( 5,5,5, PEASANT )
 call SetBuildUnitEx( 4, 4, 6, HOUSE )
 call SetBuildUnitEx( 8,8,8, PEASANT )
 call SetBuildUnitEx( 1, 1, 1, BARRACKS )
 call SetBuildUnitEx( 1, 1, 1, BLACKSMITH )
endfunction

//-------------------------------------------------------------------------------------------------
// ATTACK TEST
//-------------------------------------------------------------------------------------------------

function PerformCommand takes nothing returns nothing
 local integer cmd = GetLastCommand()

 if cmd == 1 then
 call DisplayTextToPlayer(user,0,0,"Adding units to attack captain")
 call AddAssault(4, FOOTMAN)
 elseif cmd == 2 then
 call DisplayTextToPlayer(user,0,0,"Attacking")
 call CaptainAttack(TARX,TARY)
 elseif cmd == 3 then
 call DisplayTextToPlayer(user,0,0,"Truncating attack captain")
 call InitAssault()
 elseif cmd == 4 then
 call DisplayTextToPlayer(user,0,0,"Adding units to defense captain")
 call AddDefenders(4, FOOTMAN)
 elseif cmd == 5 then

 if CaptainGroupSize() == 4 then
 call DisplayTextToPlayer(user,0,0,"Attack captain full")
 elseif CaptainGroupSize() == 0 then
 call DisplayTextToPlayer(user,0,0,"Attack captain empty")
 else
 call DisplayTextToPlayer(user,0,0,"Something strange is afoot")
 endif

 elseif cmd == 6 then
 call DisplayTextToPlayer(user,0,0,"Relocating the defense captain for 5 seconds")
 call SetCaptainHome(DEFENSE_CAPTAIN, DEFX2, DEFY2)
 call Sleep(5)
 call SetCaptainHome(DEFENSE_CAPTAIN, DEFX, DEFY)

 elseif cmd == 7 then // FINAL SOLUTION
 call DisplayTextToPlayer(user,0,0,"recreating the captains")
 call CreateCaptains()

 call CampaignDefenderEx(5,5,5, FOOTMAN)
 call CampaignAttackerEx(2,2,2, RIFLEMAN)
 call BuildAttackers()
 call Sleep(10)
 call DisplayTextToPlayer(user,0,0,"Sleep is over, try using a captain")
 call AddAssault(2,RIFLEMAN)
 endif

 call PopLastCommand()
endfunction

function TestLoop takes nothing returns nothing
 if CommandsWaiting()>0 then
 call PerformCommand()
 endif
 call StaggerSleep(3,3)
 loop
 if CommandsWaiting()>0 then
 call PerformCommand()
 endif

 call Sleep(0.5)
 endloop
endfunction

//-------------------------------------------------------------------------------------------------
// MAIN FUNCTION
//-------------------------------------------------------------------------------------------------

function main takes nothing returns nothing
 call CampaignAI(HOUSE, null)
 call DoCampaignFarms(false)

 call DisplayTextToPlayer(user,0,0,"Script received")
 call SetCaptainHome(DEFENSE_CAPTAIN, DEFX, DEFY)
 call SetCaptainHome(ATTACK_CAPTAIN, ATTX, ATTY)

 call BuildOrder()
 call CampaignDefenderEx(4,4,4, FOOTMAN)

 call TestLoop()
endfunction

An attempt to determine when a TownThreatened() evaluates to true, how long does it stay true, and in what conditions.

 JASS:

//=================================================================================================
// TOWN THREATENED DURATION TEST HUMAN
//=================================================================================================
globals
 player user = Player(0)
endglobals
//=================================================================================================

function B2S takes boolean b returns string
 if b then
 return "true"
 else
 return "false"
 endif
endfunction

function Dig2Str takes integer i returns string
 if i == 1 then
 return "1"
 elseif i == 2 then
 return "2"
 elseif i == 3 then
 return "3"
 elseif i == 4 then
 return "4"
 elseif i == 5 then
 return "5"
 elseif i == 6 then
 return "6"
 elseif i == 7 then
 return "7"
 elseif i == 8 then
 return "8"
 elseif i == 9 then
 return "9"
 else
 return "0"
 endif
endfunction

// Courtesy of AIAndy and Tommi. See source:
// https://www.hiveworkshop.com/threads/two-custom-campaign-ais-examples.8939/
function Int2Str takes integer ic returns string
 local string s = ""
 local integer i = ic
 local integer ialt = 0
 local boolean neg = false

 if i == 0 then
 return "0"
 endif
 if i < 0 then
 set neg = true
 set i = (-1)*i
 endif
 loop
 exitwhen i == 0
 set ialt = i
 set i = i / 10
 set s = Dig2Str( ialt - 10*i ) + s
 endloop
 if neg then
 set s = "-"+s
 endif
 return s
endfunction

//=================================================================================================

function Defcon takes nothing returns nothing
 local integer DefSleep = 0

 loop
 set DefSleep = DefSleep + 2
 exitwhen TownThreatened() == false
 call DisplayTextToPlayer(Player(0),0,0,("Town threatened for: " + Int2Str(DefSleep)))
 call Sleep(2.0)
 endloop
endfunction

function BuildPriorities takes nothing returns nothing
 call SetBuildUnitEx( 1,1,1, 'hpea' )
 call SetBuildUnitEx( 1,1,1, 'htow' )
 call SetBuildUnitEx( 5,5,5, 'hpea' )
 call SetBuildUnitEx( 1,1,1, 'hbar' )
 call SetBuildUnitEx( 2,2,2, 'hhou' )
 call SetBuildUnitEx( 1,1,1, 'halt' )
 call SetBuildUnitEx( 8,8,8, 'hpea' )
 call SetBuildUnitEx( 5,5,5, 'hhou' )
 call SetBuildUnitEx( 1,1,1, 'hlum' )
 call SetBuildUnitEx( 1,1,1, 'hbla' )
 call SetBuildUnitEx( 1,1,1, 'hvlt' )
 call SetBuildUnitEx( 6,6,6, 'hhou' )
 call SetBuildUnitEx( 1,1,1, 'hwtw' )
endfunction

function main takes nothing returns nothing
 call CampaignAI('hhou',null)
 call DoCampaignFarms(false)

 call BuildPriorities()
 call Sleep(2)

 call DisplayTextToPlayer(Player(0),0,0,"Ready to begin testing")

 loop
 if TownThreatened() == true then
 call Defcon()
 endif
 call Sleep(1.5)
 endloop
endfunction

The good thing about such testing method is that you are the controlling player before the game (setting up the AI) and during the game (sending commands or stress testing the AI player by playing). Try looking at these scripts, import them to test maps and see what conclusions you reach. If you like the idea, suggest more tests, or better yet, create your own.

#### Attachments

Manual captain commands - test map.w3x

 118.8 KB

 · Views: 316




 Last edited: Aug 17, 2017

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

If they kept the terminology the same for StarCraft II then AI natives involving "suicide" mean that units will mercilessly hunt down and kill all accessible enemies with no regard for their own life until they are dead. Generally used to get rid of bully units once a base falls, or if an attack wave is no longer needed. Suicide disables unit health management and other such decisions, they will literally fight until they die or run out of targets.

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

Dr Super Good said:

 If they kept the terminology the same for StarCraft II then AI natives involving "suicide" mean that units will mercilessly hunt down and kill all accessible enemies with no regard for their own life until they are dead. Generally used to get rid of bully units once a base falls, or if an attack wave is no longer needed. Suicide disables unit health management and other such decisions, they will literally fight until they die or run out of targets.


Click to expand...

That's one. There are actually two kinds of suicide functions with wc3 AI: two functions which send units to suicide regardless of captains (these two SuicideUnit

 and SuicideUnitEx), and other functions. One of the things I haven't tested yet for example is the difference between all of the following functions:

 JASS:

native CaptainVsUnits takes player id returns nothing
native CaptainVsPlayer takes player id returns nothing
native SuicidePlayer takes player id, boolean check_full returns boolean
native SuicidePlayerUnits takes player id, boolean check_full returns boolean

// The upper two functions seem identical to the lower 2 functions with the exception of extra arguments and a return value.
// What interests me for example in all these functions is what is the difference between Player and PlayerUnits (what units are the preferred targets of each?
// what happens if you try calling a function against PlayerUnits when there are only buildings? that kinda things) and also,
// which boolean do the two suicide functions return and why.

By the way, are you up for some testing?

#### moyackx

#### moyackx

 Level 15

JoinedFeb 15, 2006Messages851

Resources: 9

#### Resources
Maps 4Spells and Systems 2Tutorials 3

Great work inquiring in the world of jass AI. I'll try to get some spare time to test your findings.

BTW, I share with you my tutorial about developing a campaign AI: http://wc3jass.com/4238/how-to-make-a-campaign-ai/

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Nowow said:

 what happens if you try calling a function against PlayerUnits when there are only buildings? that kinda things) and also,


Click to expand...

In WC3 I believe units refers to buildings and non buildings.

The difference is likely that suicide against player will search for player bases and attack towards those, possibly only seeking individual units after all bases are dead. Suicide against units probably search for individual units to seek and destroy, making no distinction between base units and rouge units. Or at least that is my theory.

It could also be an artefact left in from early development.

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

Added a more detailed explanation of CommonSuicideOnPlayer. Perhaps too detailed... Views about how clear the explanation is and how much it disrupts the natives explanation will be appreciated.

 (Yes I'm supposed to be inactive. What are you gonna do, sue me?)

#### ZiBitheWand3r3r

#### ZiBitheWand3r3r

 Level 18

JoinedNov 21, 2012Messages835

Resources: 20

#### Resources
Maps 8Spells and Systems 12

AI prefers enemy warriors/workers/structures (in such order) when calling SuicidePlayerUnits

AI may attack enemy unit/group of units which are outside the base.

SuicidePlayer however gives AI order to attack enemy base area in my tests

in CommonSuicideOnPlayer bldgs means "buildings" I guess.

#### ZiBitheWand3r3r

#### ZiBitheWand3r3r

 Level 18

JoinedNov 21, 2012Messages835

Resources: 20

#### Resources
Maps 8Spells and Systems 12

During tests I tryed to track captains, and other AI behavior.

But what makes me wonder is why the hell such a simple natives like I2S GetUnitName GetObjectName do not work in AI script. And other ones like set bj_lastCreatedUnit = CreateUnit(Player(0), 'hrgy', 0.00, 0.00, 0.00) or call BJDebugMsg("Base is null") crashing ai script? Do you know a list of working/not working natives in AI ?

#### Nowow

#### Nowow

 Level 12

JoinedJun 15, 2016Messages474

Resources: 2

#### Resources
Tutorials 2

ZiBitheWand3r3r said:

 But what makes me wonder is why the hell such a simple natives like

I2SGetUnitNameGetObjectName

do not work in AI script.


Click to expand...

No idea. I do know that the AI script works with units, albeit rarely, and there are some old examples for using groups in an AI script. Except for that, I really can't tell.

ZiBitheWand3r3r said:

 And other ones like

setbj_lastCreatedUnit=CreateUnit(Player(0),'hrgy',0.00,0.00,0.00)

or

callBJDebugMsg("Base is null")

crashing ai script?


Click to expand...

I believe these two crashes are specifically related to how the map script is compiled JASS comes from 3 code files: common.j, common.ai and blizzard.j . common.j is responsible for the most basic operations, natives and type declarations. common.ai does everything AI related, and blizzard.j contains BJ functions and variables, such as BJDebugMsg and bj_lastCreatedUnit. I believe the problem is the AI itself being compiled before blizzard.j is compiled, i.e. before the definition/declaration of BJDebugMsg and bj_lastCreatedUnit. Note that trying to create a unit not assigned to a bj (only CreateUnit(Player(0), 'hrgy', 0.00, 0.00, 0.00)) works when I test it.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

AI and triggers likely use separate JASS virtual machines which have separate hooks and resource tables. As such a function like I2S might not have a binding in the AI virtual machine but does in the trigger virtual machine. Even if the bindings are common between the two, AI natives might reference resources not bound to the trigger virtual machine so cause a crash.

StarCraft II solved this by using the same Galaxy virtual machine to run both AI and trigger scripts allowing all natives to be called anywhere, even if not meaningful (eg printing text inside a tactical AI function).

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Love it.  I'm going to go ahead and approve this, especially since this info is so hard to find elsewhere. Feel free to make the updates whenever you see fit. Approved!

#### Acolite333

#### Acolite333

 Level 6

JoinedDec 11, 2014Messages93

Sorry to necro this, but I was just wondering if there were any plans to continue updating this guide at some point? The first AI workflow has been tremendously helpful and I am so excited to be able to learn and master more, perhaps with a bit more of a complex AI?

 You must log in or register to reply here.


### Similar threads

 Creating AI workflow


Nowow

Nov 27, 2016

AI Tutorials
2

Replies51Views32K

Jul 7, 2025
Superfrycook

Locked

[AI]
 End or stop AI of eliminated CPU?


FlorianGeyerGiebelstadt

May 31, 2023

Triggers & Scripts

Replies1Views1K

Jun 4, 2023
Gnuoy

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

 Can't seem to get 'AttackMoveKillA' to work properly - Campaign AI


DarkIllidan

Nov 17, 2022

Triggers & Scripts

Replies1Views821

Nov 20, 2022
DarkIllidan

 This AI does not start


nicktus

Aug 30, 2025

Triggers & Scripts

Replies3Views790

Sep 2, 2025
nicktus

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