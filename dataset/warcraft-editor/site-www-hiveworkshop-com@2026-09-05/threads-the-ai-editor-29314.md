---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/the-ai-editor.29314/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# The AI Editor
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/the-ai-editor.29314/>
#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

This tutorial is about the different parts of the .wai AI editor, how to configure conditions, the condition explaination. and also it will give a explaination about the functions. (How I'm going to write it is by the left)

How to configure conditions

To configure conditions right click where you want to configure it and bam there the option!

The Conditions

Well this part is currently being written so all things might not be there

Base Conditions

Boolean Condition: Condtion then ture or flase. Similar to the Trigger Editor one.

Integer Condition: Condition then the amount. Similar to the Trigger Editor one.

And Condition: Condition 1 and Condition 2. Both required to activate the string assigned to. Similar to the Trigger Editor one.

Or Condition: Condition 1 or Condition 2. Any required to activate the string assigned to. Similar to the Trigger Editor one.

Boolean Condition Triggers

Most of these are pretty straight forward only the ones that are tricky I'll give an explaination for.

Preset - Current game is single player

Hero - Attack Captian is at Home: It means that the leader of an attack group is at base.

Hero - Attack Captian is Retreating

Hero - Attack Captian is at Goal

Hero - Attack Group is Complete

Hero - Attack Group is Empty

Hero - Captian is in Combat: Any captian (Attack or Defense) is in combat.

Misc - Creeps exist anywhere

Town - Town is under attack

Town - Town has mine: You have to put down the town number not an other trigger.

Town - Town has Hall: You have to put down the town number not an other trigger.

Town - Expansion Spot Needed

Integer Condition Triggers

Most of these are pretty straight forward only the ones that are tricky I'll give an explaination for.

Preset - Maxium number of inventory slots (6)

Preset - Maxium number of players (12)

Preset - Maxium number of player slots(16)

Arithmetric: Okay this is the combonation Integer Trigger and operator (+,-,x,/) and an other Integer Trigger. Simlilar to the Trigger editor one.

Note that Commands are sent from the trigger editor

Commands - Commands Waiting

Commands - Last command sent: You can either remove or leave the last sent command.

Commands - Last command data: You can either remove or leave the last sent command data.

Hero -Attack Group Size

Hero - Attack Group Health Level

Hero - Attack Group Mana Level

Misc - Current Attack Wave

Misc - Melee Diffuculty Level

Resources - Current Gold Amount

Resources - Current Lumber Amount

Resources - Total unmined Gold Owned

Resources - Food used

Resources - Food Produced

Resources - Food Produced by unit type: The unit must produe food.

Town - Number of Mines owned

Town - Town with Mine: Using Town id.

Town - Next expansion town: Using Town id.

Unit - Total units of type:Owns an certian amount of an unit type.

Unit - Total units of type - Completed Only: Owns an certian amount of an unit type completed.

Unit - Total units of type - By Town: Owns an certian amount of an unit type completed or uncompleted in an town.

Unit - Gold cost of Unit Type

Unit - Lumber Cost of Unit Type

Unit - Build time of Unit

Upgrade - Current Upgrade level

Upgrade - Gold cost of upgrade

Upgrade - Lumber cost of upgrade

The First Page

On the first page there will be a AI name, race selector, behaviors, import custom data, and where to add conditions.

Name:This sets the name of your AI. (Check the box "Set Player Name" for this to work if your map has fixed player names.)

Races: You can pick between Human, Orc, Undead and Night Elf. If you are using an custom race there is an Custom option for that. (For Custom import your Object Data from your map.)

Behaviors: These are the Behaviors.

Set Player Name: This shows the name of your AI even if it has a set player name on the map.

Melee: This states if it is an melee AI.

Defend Users: The AI will protect other players if they are attacked.

Random Paths: The AI will pick paths in a more random fashon.

Target Heroes: The AI will target enemy heroes more.

Repair Structures: The AI will repair strcutures.

Heroes Flee: The AI will retreat its hero if they are in a crictical condition.

Units Flee: The AI will retreat its units if they are in a crictical condition.

Groups Flee: The AI will retreat attacking groups if their are in a disadvantage.

Have no Mercy: The AI will attack if the enemy has weaken units or at a disadvantage.

Ignore Injured: The AI will ignore units under 50% when assembling attack forces.

Remove Injuries: The AI will send injured units back to base or somewhere where it could heal.

Take Items: Heroes will pick up useful items.

Buy Items: Heroes will buy useful items.

Slow Harvesting: Workers will harvest 1 gold or 1 lumber depending on their mining type.

Allow Home Changes: The AI will pick new locations for an home base and retreating locations.

Smart Artillery: The Ai siege units will seige enemy bases when possible.

Custom Object Data Import/Export: This option allows you to Import and Export custom object data. The file type is .w3o.

Conditions: This allows you to create conditions for building orders and attacking groups. It alsos allows you to edit and delete them as well.

The Second Page

This allows you to pick the heroes that your AI uses, the chance of getting them, and skill seletion.

Hero Selection: Allows you to pick the heroes your hero uses.

Chances: The many combinations of the hero order compiled into chances. There are 6 chances for different hero orders.

Skill Selection: The skill selection order for your hero. This allows you to select what skills they learn.

The Thrid Page

This page allows you to pick what is their town hall, thier mine building, harvesters, building order, and harvest priorities

Town Hall, Mine building, Harvesters: This allows you to pick thier Town Hall, Mine Building, and Harvesters.

Building Order: The order of construction of your AI. Conditions are used here. The first building should be your town hall then your starting units.

Harvest Priorities: These Prioirities is how much harvesters are to mine in its Towns.

The Fourth Page

This page allows you to add attack groups, order attack waves, and target priorities.

Attack Groups: This allows you to order the attack groups of the AI allowing you to order thier conditions and how much they send of each unit.

Attack Waves: This makes the Attack Groups to be ordered in waves with a delay time, to add multiple waves of the same group add it again. Also it allows you to pick its minium forces, delay time before the first wave and repeat the attack wave.

Attack Priorities: These priorities will order the AI which in want to do the top one will be its first priority, conditions will also be configued and minium units and maxium units to be used as well top enable flyers to join.

The Last Page

Lastly this page where you can observe your AI but if it has custom object data this cannot be done since it was designed for Melee AI observing.

Game Speed: Adjusts the game speed for observing your AI.

Game Options: Allows you to disable fog of war and victory/defeat conditions.

Map File: Sets the melee map for testing.

Players: Sets the User (you) as an observer or you can play against your AI, at least 1 computer player must use your AI for testing if you want to see if you done everything correctly.

Now I hope you got an better understanding of the .wai editor!'

Other AI editing links

Might help for those interested in triggering an AI via triggers, more advance detailed guide on the AI Editor or editing the .ai file directly.

http://wc3campaigns.net/showthread.php?t=79638 (Arena: Trigger AI)

http://wc3campaigns.net/showthread.php?t=87999 (Campaign AI)

http://wc3campaigns.net/showthread.php?t=89199 (AI Editor)

http://wc3campaigns.net/showthread.php?t=92915 (Incomplete but still good, AI Editor)

-Av3n


 Last edited: Dec 11, 2008

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Sorry for my slow response.


I Approve.

#### Jaakko

#### Jaakko

 Level 19

JoinedApr 25, 2006Messages1,309

Resources: 4

#### Resources
Maps 4

I have a guestion, can AI editor make AIs only for melee maps ? Can't they be used in custom maps?

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Well the A.I editor...


I'll but it this way. The .ai file can be maulipulated for custom maps such as hero defenses, custom melee. Simple maps like that. Arenas and AoS's you'll have to controll the A.I with triggers as well because the ai might nt behave how would you wan it to behave.


-Av3n

#### Jaakko

#### Jaakko

 Level 19

JoinedApr 25, 2006Messages1,309

Resources: 4

#### Resources
Maps 4

I mean that can the same AI also be run in a custom map, not only melee?

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Yea an a.i cn aso b runned on any map


-Av3n

#### Ham Ham

#### Ham Ham

 Level 10

JoinedOct 2, 2006Messages1,316

Resources: 29

#### Resources
Models 20Maps 6Spells and Systems 1Tutorials 2

In fact, an AI of the AI Editor just runs when the computer player have a base. AIs are only used on Melees and Custom Melees


Nice tutorial for newcomers on the AI editor.

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Well try put an Firelord on the map. Add Firelord as the only unit in everything.(except the building section) and c if it still works


-Av3n

#### valkemiere

#### valkemiere

 Level 13

JoinedNov 20, 2006Messages553

Resources: 9

#### Resources
Models 6Maps 3

How to make AI in custom map AOS type?

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Well if you want to make a Custom AI for a AoS thts something tht you shouldn't ask me, my A.I fields haven't widen yet.


-Av3n

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

In a non-melee map, you should trigger the AI as the AI Editor has its limitations.

#### valkemiere

#### valkemiere

 Level 13

JoinedNov 20, 2006Messages553

Resources: 9

#### Resources
Models 6Maps 3

oh, i just wanna learn making AI in aos map. Because it more harder than make AI in hero arena.

#### LudwigVonDrake

#### LudwigVonDrake

 Level 4

JoinedFeb 10, 2007Messages107

valkemiere said:

 oh, i just wanna learn making AI in aos map. Because it more harder than make AI in hero arena.


Click to expand...


eh si chuji wkwk


yeah i wanna to learn to make AI for some of my games  but that things make my brain suffer

#### killacow

K

#### killacow

Hero Selection

I have been using the AI editor for a custom melee but....


How would I fix the total number of hero's to 4


I can get the AI to get 4 heros but....


I cant choose the skills on the fourth hero how do i?

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Sorry for the late reply, unfortunately the AI Editor is very limited and doesn't allow the use of four heroes.

Your only option here is the JASS AI it

-Av3n

#### axleion

#### axleion

 Level 9

JoinedDec 12, 2007Messages489

Resources: 2

#### Resources
Maps 1Spells and Systems 1

Is there any other way to make AI for AOS other than using JASS?

cause I think that will cause a heavy lag..

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

GUI if you really think JASS is ineffecient

-Av3n

#### lordolly

#### lordolly

 Level 2

JoinedSep 5, 2007Messages14

AI Heros

you can make AI just for or a set of heros heros, if youi remove the starting units for the player (townhall,builders etc) you can add the hero you want in a attack group of its own, you might have to use triggers to initialize the attack wave but if you put the waves on with a wait time the hero will keep attacking after retreating for healing or defending users etc. i have tryed my own AI for a set of heros to go creeping even tho the AI doesnt have a base they retreat to a healing point then gop kill more creeps learning there abilities by themselves.

#### Ardnived

#### Ardnived

 Level 11

JoinedAug 16, 2007Messages846

Resources: 4

#### Resources
Maps 1Tutorials 3

How do you use the conditions I cant seem to find any button to add the conditions to unit types or building priorities

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Right CLICK the field and go down the till you find Modify Condition and click it, voila and you can add pre-set cnds or custom ones

Hopefully that helps

-Av3n

#### Ardnived

#### Ardnived

 Level 11

JoinedAug 16, 2007Messages846

Resources: 4

#### Resources
Maps 1Tutorials 3

ah excellent, thank

#### CyberDuelX16

C

#### CyberDuelX16

Finally! Ages have i been looking how to make this almost freaking impossible to make custom AI for my map! I have made a custom Town AI for my map and can anyone help me to test it if it can work Online by hosting it? Such a pitty i cant host, thank you for this tutorial!

#### Ardnived

#### Ardnived

 Level 11

JoinedAug 16, 2007Messages846

Resources: 4

#### Resources
Maps 1Tutorials 3

A better place to ask for help testing is the Chat or the Map Development Forum

#### Dark Hunter1357

#### Dark Hunter1357

 Level 20

JoinedFeb 20, 2007Messages843

Resources: 92

#### Resources
Models 63Icons 25Maps 2Ported Models 1Tutorials 1

How to make the ai upgrate a building ? I make a build oder: Scout Tower and after that a build oder Guard Tower, but it doesn't work... The ai build the scout tower only.

And how to export custom data from a map, if its not in a campaing ?

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Exporting object data from a map is in the [file] tab and there should be export Object Data option.

For the guard tower... Did you get the AI to build a lumber Mill first?

-Av3n

#### Dark Hunter1357

#### Dark Hunter1357

 Level 20

JoinedFeb 20, 2007Messages843

Resources: 92

#### Resources
Models 63Icons 25Maps 2Ported Models 1Tutorials 1

Ops.... I forgot... sorry then and tank you.

#### 3rgytr54

#### 3rgytr54

 Level 11

JoinedApr 6, 2008Messages562

Resources: 12

#### Resources
Models 10Packs 1Tutorials 1

Great! just one thing i caught...

 Boolean Condition: Condtion then ture or flase. Similar to the Trigger Editor one.


Click to expand...


Condtion then ture or flase --> should be --> Condition then true or false

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Spelling errors bleh, but at least it represents Conditions in a way. Cause people ain't complaining

-Av3n

#### Section

#### Section

 Level 4

JoinedJul 23, 2008Messages99

I want to ask something. Are AI functions/conditions (such as Hero Retreat and Slow Gathering) can be used in custom AI?

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Define what mean by "custom AI"

-Av3n

#### SanKakU

#### SanKakU

 Level 13

JoinedMay 11, 2008Messages1,198

Resources: 6

#### Resources
Icons 2Maps 3Spells 1

i want to learn the making ai for a tag style map...but now i'm not in a hurry, and by the way, don't know if anyone mentioned it, but some of your spelling in the tutorial is really bad, please fix it! like captain and true and false, you put captian over and over again, and ture and flase.

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Yeah I'm too lazy to change it, no one has really mind about the spelling errors.

For making an ai for a tag map you'll have to use "triggered AI" and not wc3 one, because the wc3 one is designed for melee. Though you can import a .ai script customized with both common.j and common.ai (Not sure about this one) functions to run on a computer player.

-Av3n

#### SoTAL

S

#### SoTAL

I've made a custom melee map, also made an AI in the editor. My AI builds up a base, gets units, then does nothing but harvest. There's 2 things about it that buggers me.


1: Why doesn't the AI build more than ~15-20 units? I've made it so that all the units they're supposed to build costs 0 food, and all the right buildings are in place...


2: Whatever I do, they won't attack. Me or eachother. I use the standard Undead race, so that shouldn't be a problem. What do I do? Attack waves? Priorities?


Please...

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Attack waves... You forgot to add attack waves.

Maybe perhaps you should see how 'fast' the AI gathers, or perhaps its your attack group which is 20 units.

Overall _it_ should work if your custom melee map uses the _warcraft 3 mechanics_

-Av3n

#### SoTAL

S

#### SoTAL

Yeah, but when i try to add more attack waves, it just crashes when i try to import it into the map...


I've never understood how attack waves works, really.


How many attack waves should i have? I currently just use the "all units"group, is it necessary to have more of 'em?


 Last edited by a moderator: Nov 25, 2008

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Not really, but you should check if you got the right unit types in the all units group.

I suggest you look at the AIs that blizzard included in warcraft. (Gruntmaster + Wyrm Moger (However the last word is spelt)). To see how it works, it has been years since I worked with the WE AI Editor

-Av3n

#### SoTAL

S

#### SoTAL

Yeah, i have the right units.


OK, I'll check those out, thanks.

#### DEA

D

#### DEA

I have problem with AI path. All is working just fine, but I hate when Wave are attacking first what they see and then are returning to base. Why they are not continue the path and fighting to the end of the wave, but kill first enemy at path and going home?

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Do you have Group Flees option turned on? Try disabling that and turn on Units Flee and Heroes Flee option instead and see what happens.

-Av3n

#### DEA

D

#### DEA

Av3n said:

 Do you have Group Flees option turned on? Try disabling that and turn on Units Flee and Heroes Flee option instead and see what happens.

-Av3n


Click to expand...

Nothing happen! May please me tell how you're doing AI for Custom map. I tried use flees, but too nothing happen (usually I dont use flees at all). I dont know or I'm wrong in "Attacking" or in "General".

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

It's kind of hard when you don't specify what kind of Custom map it is...

-Av3n

#### DEA

D

#### DEA

It is a Campaign

#### Av3n

#### Av3n

 Level 10

JoinedJul 18, 2005Messages324

Resources: 3

#### Resources
Maps 1Tutorials 2

Hmm... How to make a Campaign AI - Wc3campaigns, that tutorialp is dedicated for campaign ais. If you still don't understand, pm me

-Av3n

#### lordsmell

#### lordsmell

 Level 4

JoinedFeb 23, 2008Messages102

i made a custom map and i want an AI in it, how do i get it in it, cuz it says only melee map so is there a way or no..?

#### Burningbladex5

#### Burningbladex5

 Level 3

JoinedJul 17, 2009Messages17

Resources: 2

#### Resources
Maps 2

one Qestion for your start AI tutorial (the comments are locked) how do you start the Custom AI for 1 player but ceep the melee AI for the rest?

#### Burningbladex5

#### Burningbladex5

 Level 3

JoinedJul 17, 2009Messages17

Resources: 2

#### Resources
Maps 2

i made a custom AI for my map but i cant get them to attack, they train and upgrade but they will not attack

#### avpchin

#### avpchin

 Level 5

JoinedOct 3, 2008Messages58

Resources: 2

#### Resources
Maps 2

I need help with my AI. The AI is created for custom races that I've made. Everything seems to be fine except that the AI would not build expansions. I've tried to fix it but it doesn't seem to work.

#### Bloody_Turds

#### Bloody_Turds

 Level 18

JoinedJan 23, 2007Messages1,302

Resources: 25

#### Resources
Models 20Icons 1Packs 1Tools 1Maps 2

I noticed in basic trigger GUI:


AI - Send Player 1 (Red) the AI Command (string, integer)


I was wondering if anyone knows/has a list of "commands", or knowledge on how to use this. (or maybe a jass workaround) to control an AI's attack destination


(Ive heard it was a dummy unit, they follow around, and if its a unit it must have a rawcode, and be programmable?)

#### Jeremy

J

#### Jeremy

Hmmm that would definitely explain a lot. I must have found the human dummy unit while editing by accident, but somehow I did not find the orcs get the orcs.

#### D-Mos

#### D-Mos

 Level 1

JoinedSep 24, 2015Messages4

I can't get my custom Murloc and Mur'gul races to build expansion towns. They just end up going on a potentially-infinite loop of building another town hall in the main base.

 You must log in or register to reply here.


### Similar threads

[GUI]
 AI Editor: A graphical tutorial


moyackx

Mar 7, 2012

Tutorial Archive

Replies3Views6K

Jun 9, 2012
PurgeandFire

 AI Editor


VeljkoM

Apr 1, 2010

Misc/General Mapping Tutorials
2

Replies99Views84K

Sep 16, 2018
Fabrizio

Locked

[Trigger]
 Help Detecting when fighting is over


RinaDeithee

Nov 29, 2020

Triggers & Scripts

Replies1Views1K

Dec 4, 2020
Dr Super Good

Locked

[AI]
 Ai editor help


EVILMAN

Apr 25, 2010

Triggers & Scripts

Replies2Views1K

Apr 25, 2010
Septimus

[GUI]
 Triggering AI for an RPG


Jaijaibinx

Jul 24, 2010

Tutorial Archive

Replies5Views3K

Dec 11, 2010
PurgeandFire

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