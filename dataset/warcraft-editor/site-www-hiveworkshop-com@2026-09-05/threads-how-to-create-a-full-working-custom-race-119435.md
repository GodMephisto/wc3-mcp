---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-create-a-full-working-custom-race.119435/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Create a Full Working Custom Race
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-create-a-full-working-custom-race.119435/>
#### Interceptor

#### Interceptor

 Level 9

JoinedFeb 14, 2009Messages316

Resources: 2

#### Resources
Tutorials 2

###
 How to:

#
 Create a Full-working Custom Race

Okay, here we go.

You have played wc3 for very long, beaten 1vs8 insane blindfold and so on. You are just bored from the ordinary 4 races: Human, Orc, Night Elf and Undead.

Maybe what you need is to try another race? If that's right, this tutorial is for you.

Enough talk, let's go straight to the topic.

##
 What does a race contain?

Each race in wc3 is unique by itself. However, there are things which are similar in all races:

###
 ♦ User Interface (UI)

This is the interface that surrounds the game environment for each race - the frames of the mini map and the game map, the background of the inventory slot, etc. Although you does not necessarily notice, a part of the UI are also the text messages, icon for Idle Workers (F8) and the very important sound theme. Why is this important? It does not affect gameplay. What then? Well, it's pretty annoying when you play a Demon race and the frames are tree branches and butterflies, right? Or, if you run out of food, you get a message and hear "Create more Moonwells" while your food is produced by some kind of portals, for example. And, of course, it is especially annoying to see a Wisp idle worker icon, when your workers are everything but not wisps! What is also important to know when you create a custom race is that it MUST replace an existing one, preferably the most similar one. However, this SHOULD NOT AFFECT other players who are choosing the same race! I will explain this later, but what you have to understand is that the UI cannot be modified via triggers so choose wisely what race to replace and be sure that there is no other human player that plays a non-custom race since computer players are not annoyed by the UI disorders. It is not recommended to base your race on undead unless your buildings use blight terrain.

***To solve the problem, you need to do the following***

- For Idle Worker Icon, open Advanced > Game Interface..., then select Use Custom Game Interface, find Icon - Idle Worker and change it from Peon to the desired one.

- For the Text Messages, open again Advanced > Game Interface..., then scroll down to Text - General - '|cffffcc00You will be revealed to your opponents until you build a [...]|' and change it, along with the other similar messages (A gold mine has collapsed, A gold mine is running low, Build more [...] to continue unit production, etc. What is important to know here is that commas in text fields sort the text messages in their normal race order. For example, if you have chosen to replace the Night Elf race, the custom text should be after the third comma.

- For the Sounds, in the same panel Advanced > Game Interface..., scroll to the Sound options and edit all, except Sound - Music - Defeat, Sound - Music - Victory and Sound - Place Building. If you have better ideas for the three unedited options, you can use them, but it's really not necessary to show such puritanism. In my map I have replaced all of them with the Naga equivalents used in the Sentinel campaign in The Frozen Throne. They really do work not only with Naga, but with most custom races, with the exception of the "Build more Coral Beds" which is too obvious. Anyway, since sounds are (almost) impossible to find, you can either use the naga ones, to record your own, or mute the volume (lol). Up to you

- For the Frame (UI) in the Game Interface you must edit the two Image - Console Inventory options and the four Image - Console Textures. Hell, if you find textures for these, PLEASE write me an e-mail! Else, if you're unlucky, like me, the choises are either crying to a good friend to make you one (lucky you) or make one yourself. Again, your choise.

###
 ♦ 4 Heroes

Yeah... after trying to record a weird sound in the place of "Summon More Ziggurats" you realise that you don't have the 4 Heroes yet! Thinking of it as an easy part is a heavy delusion! You must very carefully design models, icons, stats and skills for your 4 Heroes, keeping the ballance between their skill strenght, unique role and being careful not to make them too weak or strong compared to the other races' heroes. How to design your Hero is entirely up to you and there are plenty of other tutorials about that. However, I would like to stress you the importance of these:

- DO NOT base your custom Hero on a TAVERN or CAMPAIGN Hero! This will cause a really bad bug when you train the Hero in the altar and will finally lead to the recreation of the Hero. [highlight]NOTE: This was lately proved wrong. I still need to redo my tests. Until then, keep following the rule.[/code]

- Pay special attention to all Arts, Animations and Tooltips. Ignoring that may cause the really annoying green icons when pressing F10, the damn Tool tip missing! message, the really weird thing that your Hero is 3 times bigger than his shadow/selecting circle, or the stupid fact that he fires arrows from his legs while holding an axe. So, really, be careful. Also notice that all heroes use X0-Y1, X0-Y2, X1-Y2 and X2-Y2 buttons when they are trained in the Altar.

###
 ♦ 10-11 Buildings

Every race must contain:

- 3-level Town Hall,

- Basic War Unit Creation Building,

- Lumber Mill Type Building (Upgrader),

- Defensive Structure,

- Altar,

- Food Producer,

- Advanced War Unit Creation Building,

- Caster Creation Building,

- Item Seller Building and

- Ultimate War Unit Creation Building.

Orcs are a good example. Humans contain a Blacksmith for upgrades, which is separate from their Lumber Mill, Night Elves's Lumber Mill is not exactly a Lumber Mill since they do not cut trees, and Undead have a Haunted Gold Mine instead of Defensive Structure (they use their Ziggurats for defense) and have an additional Sacrifical Pit. It is good to add an original and exceptional structure to your race. If you also get the idea of another unique way to transport recourses, it will be excellent. However, always remember that you cannot have more than 11 buildings.

###
 ♦ Units

Each race has:

- minimum 1 worker,

- minimum 1 type of invisibility detection,

- 3 casters,

- 2-3 air units,

- minumum 2 siege units (one of them must be mechanical) and

- minimum 1 summoned unit.

Following this scheme keeps the ballance of the game or at least make its fragility less sensible. Note that there is strict table of tier levels and upgrades that unlock higher level units. And when we speak about upgrades...

###
 ♦ Upgrades

An upgrade building's presence is a must! It should contain 4 kinds of upgrades:

- A 3-level attack upgrade for the 1st kind of units (usually melee)

- A 3-level defense upgrade for the 1st kind of units

- A 3-level attack upgrade for the 2nd kind of units (usually ranged)

- A 3-level defense upgrade for the 2nd kind of units (usually ranged)

Again, adding one additional upgrade that does not ruin balance may be a good idea (like Ultravision, Spiked Barricades, Corpse Spawning in Graveyard, etc.)

Upgrades must also include the backpack upgrade (for inventory in the Town Hall) and the Town Hall upgrades themselves.

The Caster Trainer must contain basic and advanced training for each caster where is necessary (but at least 2 such upgrades).

Also there must be upgrades for units in the other war buildings (2-3 in the basic, 3-4 in the advanced, 1-2 in the ultimate). The upgrades must require the proper tier levels.

###
 ♦ Items

There is also a rule for the items that are contained in the Item Shop building. The 2nd row is available in tier 2 town hall and the 3rd row - in tier 3.

The 1st row should contain:

- Level 1 race-unique item (like regeneration ones for human/orc, skeleton summoner for undead and eclipser for elf)

- Lesser Clarity Potion

- Random level 1 item (mechanical critter for human, flag of speed for orc, blight creator for undead, detect-invisibility consumable for elf)

The 2nd row should contain:

- Healing Potion

- Mana Potion

- Scroll of Town Portal

- Optional: random level 2 item (similar to level 2 items in goblin shop)

The 3rd row should contain:

- Orb attack item

- Random level 3 item (similar to level 3 items in goblin shop)

##
 Implementing the race

OK, you have finished the object stuff (finally!). What is next is how to make your race actually work.

The effects of UI edition in game interface apply to all races or at least to the race you have replaced. That's why it is NOT recommended to play other race than the custom one when using the map if you're not a computer player. Also, since it's a custom race, you have to make a lot changes here and there in order to make it a completely playable race. Here is the Step-by-Step guide.

1. Open Advanced > Gameplay Constants and scroll to the Techtree section. Add your Altar, Heroes and Tier Halls in the appropriate places (Techtree - Dependency Equivalents - Hero/Altar/Tier ... Hall) and modify the text tooltips (Techtree - Dependency Equivalents Text - Tier 1/2/3 Hall) adding the names of your custom race's Tier halls.

2. Make the Melee Initialization trigger look like this (do not add the functions in the -------- ... --------, they are comments to make understanding the trigger more easily):

Melee Initialization
Events
Map initialization

Contitions

Actions
-------- Remove the Victory/Defeat conditions --------

Melee Game - Use melee time of day (for all players)

Melee Game - Limit Heroes to 1 per Hero-type (for all players)

Melee Game - Give trained Heroes a Scroll of Town Portal (for all players)

Melee Game - Set starting resources (for all players)

Melee Game - Remove creeps and critters from used start locations (for all players)

Melee Game - Create starting units (for all players)

Melee Game - Run melee AI scripts (for computer players)

Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked player) controller) Equal to User

-------- Add here the replaced race, in my case Orc --------

(Race of (Picked player)) Equal to Orc

Then - Actions
Unit Group - Pick every unit in (Units owned by (Picked player)) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
-------- Here you add the unit type of the original race's town hall, in my case Great Hall --------

(Unit-type of (Picked unit)) Equal to Great Hall

Then - Actions
-------- Here you add the Town Hall of your race, here my race is goblin --------

Unit - Replace (Picked unit) with a Hall of Knowledge using The old unit's relative life and mana

Else - Actions
-------- Here you add the Worker units of your race --------

Unit - Replace (Picked unit) with a Engineer using The old unit's relative life and mana

Player - Limit training of Heroes to 3 for (Picked player)

-------- Here you add each of your custom heroes, and be sure that no one of them is based on tavern or campaign hero --------

-------- Some of mine may sound as if they are based on such heroes, but they're not --------

-------- I have based them on Blademaster and Far Seer and changed all animations --------

Player - Limit training of Tinker to 1 for (Picked player)

Player - Limit training of Alchemist to 1 for (Picked player)

Player - Limit training of Exterminator to 1 for (Picked player)

Player - Limit training of Goblin Miner to 1 for (Picked player)

Else - Actions
Do Nothing

POSSIBLE ISSUES:

- If at the beginning all town halls are at the center of the map, check player properties in scenario menu - usually it's starting location problem

- If you can train all heroes without upgrading town hall, you have not added tier requirements via the object editor or [highlight]you have based them on tavern/campaign heroes <- This was proved wrong, but I still need to redo my tests on it, until then, keep following the rule.[/code]

- If you see blight and your buildings don't use it, you have based your race on undead and the blight is from their mine and town hall

- If you see weird animations like dying entangled gold mine, hide units before replacing them

- If you get a message that you will be revealed unless you build a ... you have not deleted victory/defeat conditions from the melee game actions

- If a problem still exists, send me the map or ask in the tutorial thread (this one) but avoid pm-ing me

#
 What's to come?

OK, let's be fair. You should have noticed that there are no Victory/Defeat conditions. What does that mean? Simply:

- There won't be "revealings" if someone runs out of town halls;

- If you lose all your buildings you won't get a defeat message;

- If you kill all your enemy's buildings you won't get a victory message

The problem is that Melee V/D conditions consider only basic town hall units as real town halls, no matter what you have typed in gameplay constants. That means if you enable that function you will lose/win when you destroy/run out of buildings, but you will be also revealed 2 mins after the beginning of the game, getting unusual annoying messages.

So now, I'm working on a, unfortunately very complicated, trigger to enable players win/lose the game.

It is working (with very rare exceptions) but I'm looking for a better solution.

Any ideas would be greatly appreciated!

If you want a trigger to circumvent this issue, then see this post by Retera:

https://www.hiveworkshop.com/forums/2378104-post50.html

I hope I have helped you.

~Interceptor


 Last edited by a moderator: Oct 21, 2013

#### xul1ganas

#### xul1ganas

 Level 4

JoinedFeb 20, 2009Messages51

Resources: 1

#### Resources
Maps 1

Thanks for nice tutorial. Very helpfull. I will add u reputation.

#### M4stah

#### M4stah

 Level 11

JoinedFeb 16, 2009Messages757

Resources: 2

#### Resources
Spells 1Tutorials 1

For every main base type a player builds, add a number to a integer array and hwen it reaches zero use visibility and possible defeat triggers.

#### Interceptor

#### Interceptor

 Level 9

JoinedFeb 14, 2009Messages316

Resources: 2

#### Resources
Tutorials 2

A player is defeated when s/he loses all his/her buildings. That's why current triggers watch the number of buildings via variables that change when a building is began or canceled/destroyed.

#### Element of Water

#### Element of Water

 Level 18

JoinedAug 3, 2008Messages2,345

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

It is not possible to make the proper melee Victory/Defeat conditions with altered races in GUI (without lots of custom script). You can have a good attempt, but it will never be perfect.

#### Super-Sheep

#### Super-Sheep

 Level 20

JoinedOct 21, 2006Messages3,224

Resources: 2

#### Resources
Maps 2

"Shop should have

 - ...

 - Lesser Clarity Potion

 - ..."

Undeads dont have this.

#### LN.Angel

#### LN.Angel

 Level 3

JoinedJun 9, 2008Messages36

i like it

i tryed it myself to crate a naga race

but i got a better project now

#### Traxamillion

#### Traxamillion

 Level 19

JoinedSep 14, 2007Messages1,527

Resources: 10

#### Resources
Models 2Icons 7Tutorials 1

Very useful.

+rep

#### 3rgytr54

#### 3rgytr54

 Level 11

JoinedApr 6, 2008Messages562

Resources: 12

#### Resources
Models 10Packs 1Tutorials 1

About the town halls, there is a condition in the object editor called classified asright?) and it works.

#### Interceptor

#### Interceptor

 Level 9

JoinedFeb 14, 2009Messages316

Resources: 2

#### Resources
Tutorials 2

It does not. Don't think I haven't tried. Gameplay constants, classifications, etc. do not help.

#### 3rgytr54

#### 3rgytr54

 Level 11

JoinedApr 6, 2008Messages562

Resources: 12

#### Resources
Models 10Packs 1Tutorials 1

Ok, just making sure you didn't over look anything. I have used them before with a unit based off a regular town hall and it worked. I'm going to make sure that I remember what I think I remember is that it worked. If it works I'll attach a map that it works on.

#### Interceptor

#### Interceptor

 Level 9

JoinedFeb 14, 2009Messages316

Resources: 2

#### Resources
Tutorials 2

It works if you modify an existing town hall, but not if you create a new, add the classification, add it in gameplay constants, don't delete the V/D conditions melee action and use it in your map like that.

#### Deathcom3s

#### Deathcom3s

 Level 15

JoinedDec 24, 2007Messages1,403

Resources: 2

#### Resources
Models 1Icons 1

Nice tutorial. I see no reason for it to not be approved.

+rep.


 Last edited: Mar 26, 2009

#### utror12

#### utror12

 Level 5

JoinedNov 2, 2008Messages125

very nice thank you so god damm much i + rep XD

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Approved lol

#### Teelo

#### Teelo

 Level 11

JoinedMar 31, 2009Messages732

Resources: 2

#### Resources
Maps 1Spells 1

 The problem is that Melee V/D conditions consider only basic town hall units as real town halls, no matter what you have typed in gameplay constants. That means if you enable that function you will lose/win when you destroy/run out of buildings, but you will be also revealed 2 mins after the beginning of the game, getting unusual annoying messages.


Click to expand...

Are you sure? I was doing something similar (but no way near as vast) recently, and solved that problem by making my new Town Hall a tech-tree equivalent of the replaced town hall (in your case, Great Hall).

EDIT: Never mind.

 It works if you modify an existing town hall, but not if you create a new, add the classification, add it in gameplay constants, don't delete the V/D conditions melee action and use it in your map like that.


Click to expand...

#### Tleno

#### Tleno

 Level 27

JoinedJul 6, 2008Messages11,316

Resources: 3

#### Resources
Maps 1StarCraft II Resources 2

I have a question: Can you make an technology upgrade instead of town hall upgrading?


Something like in Company of Heroes German tech tree?

#### Interceptor

#### Interceptor

 Level 9

JoinedFeb 14, 2009Messages316

Resources: 2

#### Resources
Tutorials 2

I don't know what are you talking about, but I assume you mean either that advanced buildings require basic ones (in that case - no, because it's overpowered and you will progress much faster than ordinary races) or that instead of upgrading the town hall you research a major technology there (in that case - yes, it won't change the ordinary gameplay, but graphically it's better to upgrade the town hall =P).


Correct me if I'm wrong.

#### 3rgytr54

#### 3rgytr54

 Level 11

JoinedApr 6, 2008Messages562

Resources: 12

#### Resources
Models 10Packs 1Tutorials 1

tleno said:

 I have a question: Can you make an technology upgrade instead of town hall upgrading?


Something like in Company of Heroes German tech tree?


Click to expand...


I personally have never played it but, you can make the town hall it upgrades into have the same model file. (not town hall type file (all three steps in one file, that kind won't work))

#### Ricardo Irving

#### Ricardo Irving

 Level 9

JoinedMay 10, 2009Messages542

0101

Element of Water said:

 It is not possible to make the proper melee Victory/Defeat conditions with altered races in GUI (without lots of custom script). You can have a good attempt, but it will never be perfect.


Click to expand...


Didn't read most of it, so is the problem that its units aren't modified versions of already existing ones?

#### VeljkoM

#### VeljkoM

 Level 21

JoinedJul 27, 2008Messages14,361

Resources: 4

#### Resources
Maps 3Tutorials 1

I think you need to mention units more.

It is very important to know what type of attack and armor suits which unit.

Like:

-Melee unit:

attack-normal ;armor-heavy

-range unit:

attack- piecing;armor-normal

-spelcasting unit:

attack-magic ;armor-light or none

-air unit

attack-piercing ;armor-light

Note that air units also can have magic or normal melee attack.

Also spellcasters should have healing and dispelling magic (not necessary for one to have both)

EDIT:For victory condition you should make trigger that counts enemy buildings.Like if enemy has 0 buildings he is defeted.


 Last edited: Jun 25, 2009

#### Edagras

#### Edagras

 Level 2

JoinedJul 3, 2009Messages13

So if I make custom race, in the custom game I can choose race from five fields. It will be human, orc, undead. night elf and my custom race?

#### VeljkoM

#### VeljkoM

 Level 21

JoinedJul 27, 2008Messages14,361

Resources: 4

#### Resources
Maps 3Tutorials 1

You need triggers to do that and only in game you can chose custom race. Exception is if you make custom race in place of already existing race.

#### Piccolo

P

#### Piccolo

Can IA use a custom race?

#### Element of Water

#### Element of Water

 Level 18

JoinedAug 3, 2008Messages2,345

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Grr I hate the subscribed threads page! (It kept this thread in subscribed threads and I couldn't delete it or do anything with it so I need to post in it to make it go away.

#### OMonaxos

#### OMonaxos

 Level 6

JoinedAug 12, 2008Messages267

Nice tutorial +rep!


And I hope it gets reported...for sticky

#### Edagras

#### Edagras

 Level 2

JoinedJul 3, 2009Messages13

About text messages, so when I find this text message |cffffcc00You will be revealed to your opponents unless you build a Great Hall.|r , I just need to telete it and put this maessage (A gold mine has collapsed, A gold mine is running low, Build more [...] to continue unit production, etc. ??? and thats all?

#### Interceptor

#### Interceptor

 Level 9

JoinedFeb 14, 2009Messages316

Resources: 2

#### Resources
Tutorials 2

You have to replace the Great Hall with your race's town hall name, and the Farms with your race's food producers' name, etc.


For example


|cffffcc00You will be revealed to your opponents unless you build a Temple of the Tides.|r


Build more Coral Beds


etc.

#### Edagras

#### Edagras

 Level 2

JoinedJul 3, 2009Messages13

how I should do this trigger "Add here the replaced race, in my case Orc"? I cant write a simple comment, because there is condition.

#### Edagras

#### Edagras

 Level 2

JoinedJul 3, 2009Messages13

so can anyone help me???

SOLVED


 Last edited: Jul 15, 2009

#### GTR_MajorBlood

#### GTR_MajorBlood

 Level 2

JoinedAug 4, 2009Messages13

Thanks a lot this really helped now I can make a Blood Elf Campaign without to much problems. 5/5!

#### GGwarcarft

G

#### GGwarcarft

thanks re your infom

#### Jrock907

#### Jrock907

 Level 2

JoinedNov 16, 2009Messages14

Resources: 1

#### Resources
Maps 1

I tried doing this and it failed miserably...when the game starts, it gives me a bunch of town halls and no workers... there are so many town halls it covers up the gold mine! I probably did something wrong since it's working for everyone else...but I'm not sure what I could have done. for the places where it says (picked unit) do I need to put something there? it's trying to make me choose a town hall/worker that is already on the map...

EDIT: nvm..I think I fixed it. now it just gives me 6 workers...it's better than 28. i probably don't make that many in 1 game. lol


 Last edited: Jan 10, 2010

#### fcastor

#### fcastor

 Level 1

JoinedDec 28, 2009Messages9

Nice mocro-tutorial.


This contain the exactly essence of WC3 races similarities..


vlw!

#### Spudmuffin

S

#### Spudmuffin

DAMNIT! i have created a WHOLE race in the "Custom Unit" Tile


i found out u need to replace it a while later, so i read this, at first time, i created it by replacing but i changed it

#### Inzeen

I

#### Inzeen

Hello,


Im kinda new to this all, but I´m trying to make a WC2 human and orc race, with all the troubles it entails.


I tried making the trigger you provided, but i cant seem to find the fields to make this part of it: If (All Conditions are True) then do (Then Actions) else do (Else Actions).


How do you set them?


Could you, or anyone who knows this, help this noob out?

#### Shockwaver

#### Shockwaver

 Level 4

JoinedSep 29, 2010Messages76

Resources: 1

#### Resources
Skins 1

Very nice tutorial 5/5

#### Avengerwest

A

#### Avengerwest

OMG is everything for TfT I have ROC world editor i need help importing and makeing new races wtf

#### Warcraft_Tester

#### Warcraft_Tester

 Level 5

JoinedSep 21, 2010Messages142

Resources: 1

#### Resources
Spells 1

Dude there is some things which may need changing but there are some easier ways and no the campaign units won't bug up i tested it once and you can edit the units, heros and buildings, instead of making custom units and edit them and make custom triggers its a lot simpler than doing most of this. Ps I am the Tester of warcraft III

#### hayatesasaki

#### hayatesasaki

 Level 4

JoinedDec 16, 2010Messages102

Thx soooooooooooooo much!! So when can we expect the victory/defeat trigger?

#### Interceptor

#### Interceptor

 Level 9

JoinedFeb 14, 2009Messages316

Resources: 2

#### Resources
Tutorials 2

Inzeen said:

 Hello,


Im kinda new to this all, but I´m trying to make a WC2 human and orc race, with all the troubles it entails.


I tried making the trigger you provided, but i cant seem to find the fields to make this part of it: If (All Conditions are True) then do (Then Actions) else do (Else Actions).


How do you set them?


Could you, or anyone who knows this, help this noob out?


Click to expand...


It is in General, If / Then / Else (Multiple Functions)

Warcraft_Tester said:

 Dude there is some things which may need changing but there are some easier ways and no the campaign units won't bug up i tested it once and you can edit the units, heros and buildings, instead of making custom units and edit them and make custom triggers its a lot simpler than doing most of this. Ps I am the Tester of warcraft III

Click to expand...


Yes you can, but that would mean the replaced units will be unplayable by AI, effectively eliminating one entire race from the game.

hayatesasaki said:

 Thx soooooooooooooo much!! So when can we expect the victory/defeat trigger?


Click to expand...


Er... no idea o.o


I'm too lazy to learn JASS, and I've already been told it's impossible to work perfectly in GUI.


You can give it a try if you are eager

#### hayatesasaki

#### hayatesasaki

 Level 4

JoinedDec 16, 2010Messages102

Er... no idea o.o

I'm too lazy to learn JASS, and I've already been told it's impossible to work perfectly in GUI.

You can give it a try if you are eager [/QUOTE]

OOOOOOOOOOOOOOOOOOOOMG NOOOOOO!!!! First, i haven't the first clue about triggering, 2 : I want to be a modeler/map maker , and to do THAT it would need to learn triggering! but...... no -_- I do feel all special inside because you thought I knew how to trigger though  hense my title ^^

#### Warcraft_Tester

#### Warcraft_Tester

 Level 5

JoinedSep 21, 2010Messages142

Resources: 1

#### Resources
Spells 1

Dude its possible when I tested it the ai were happy and used it freely like it was a human or a undead, it was so awesome.

Edited:

I Didn't know much about the AI but i figure out of a way which you can play a race using The AI triggers and the editor.


 Last edited: Jan 15, 2011

#### Warcraft23130

#### Warcraft23130

 Level 6

JoinedOct 8, 2010Messages60

Resources: 7

#### Resources
Icons 1Skins 5Maps 1

Arg!!!!i did the trigger, AND IN GAME I STILL HAVE PEASENTS AND A TOWN HALL!!


AAAAAAAAAAAAAHHHHHHHHHHHHHHHHHHHHHHHHH Going MAD!!!!!!!!!!!!

#### abyss69

A

#### abyss69

Edagras said:

 So if I make custom race, in the custom game I can choose race from five fields. It will be human, orc, undead. night elf and my custom race?


Click to expand...


Hey, guys, sorry if this is a very noobish question, i'm really new to this. Is the thing that Edagras said possible? I would like to make the user chos from the menu list which race to play with, and not just human/orc/undead/night elf. Keep up the great work and sorry for my bad english.

#### Vengeancekael

#### Vengeancekael

 Level 37

JoinedAug 11, 2009Messages6,521

Resources: 16

#### Resources
Tools 1Maps 13Spells 1StarCraft II Resources 1

Re-opened.

#### Rafal

#### Rafal

 Level 3

JoinedJul 1, 2010Messages31

Resources: 1

#### Resources
Spells 1

If there are still people having problems creating an additional town hall ... here´s the solution:

Adding an upgrade to a building which can not be build, the "not buildable building" should be upgradeable to the new custom town hall

let me show it you in an example:

i want a new Main Building for Naga (Temple of Tides)

Town Hall (Human) -> FakeMain (This Building is Disabled = Not Buildable) -> Temple of Tides

@Vengeancekael thank you for re-opening^^

#### MrHobby53

#### MrHobby53

 Level 6

JoinedDec 21, 2012Messages166

Resources: 2

#### Resources
Maps 2

 but is that posible with AI?

#### MegaDeath11

#### MegaDeath11

 Level 2

JoinedMar 6, 2013Messages4

Some one please! make a Youtube video of this?  i dont really understand


___________


New Here

#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


Hello; I was reading this thread and I couldn't resist the one hour of work that it would take me to get you guys a JASS script system for this. With the attached map, any unit that has "Stats - Unit Classification" set to include "Town Hall" will count for Blizzard's reveal system. It's a little hack of their original JASS code that I've been using for mod-making, myself. (The entire victory/defeat system is already written in JASS for you! It hardly seems worthwhile to me to even attempt re-writing in GUI what you already have available!)


So, anyway, the attached map worked in all of my tests and is just a copy of all of the important victory/defeat code, then hacked to allow any town center-type unit to count for the annoying reveal system.


To use this system, just copy & paste all of the code from this map's "Custom Script" section into your map. Then, copy or create the String-type variable "RevealWarning" into your map. After that, enabling the system is as easy as this:


In your "Melee Initialization" trigger that was created inherently by the World Editor, you'll see an Action looking like this:


Just remove it, and instead put in this line of custom script:


If you copied the custom script correctly, this will activate the customized melee victory/defeat conditions that I made instead of the normal ones.


I overwrote the reference to the "You will be revealed to your opponents unless you build an X" message, too. It's just a trigger GUI variable that has to be named "RevealWarning". Now, changing it is as simple as this:

Click the "X" to open up variables.


Double-click the "RevealWarning" variable -- it's in my map, or if you're working in your own map you should have copied it in.


You'll see a popup that looks like this. One part of it says "Initial Value:" and has blue text -- the next step will be clicking on that text.


This brings up the popup that lets you edit the "You will be revealed..." message. From there, you can make your changes and then push "OK" about three times to save the changes.


I know that was probably an overly elaborate description of dealing with the GUI variables -- I'm sure a lot of you already knowing everything there is to know about using them -- but I figured I'd try to make it all as easy as possible for everyone to use.

#### Attachments

CustomMelee1.png

 1.5 KB

 · Views: 3,290



CustomMelee2.png

 1.7 KB

 · Views: 3,218



CustomMelee3.png

 132.3 KB

 · Views: 3,184



CustomMelee5b.png

 17.1 KB

 · Views: 3,112



CustomMelee6b.png

 20.2 KB

 · Views: 3,066



(8)DeadlockCustomRace.w3x

 275.7 KB

 · Views: 1,644



CustomMelee4b.png

 17.4 KB

 · Views: 3,125



 You must log in or register to reply here.


### Similar threads

 Custom Race Creation: Advanced Mechanics


Alok

Sep 29, 2016

Misc/General Mapping Tutorials

Replies12Views17K

Sep 13, 2018
Warew0lf###(omega)

 How to create a unit that can level up


Alok

Jul 3, 2016

Misc/General Mapping Tutorials

Replies8Views5K

Sep 22, 2016
Alok

Locked

 Melee AI for custom race


Erkki2

May 26, 2013

World Editor Help Zone

Replies9Views6K

Jul 1, 2013
Footman16

Locked

 Open slot for players get custom race starting units


tdrsmynm

Feb 6, 2021

World Editor Help Zone

Replies1Views1K

Feb 8, 2021
hexhound

 How To Create A Hero Tavern


Flame_Phoenix

May 4, 2007

Misc/General Mapping Tutorials
23

Replies125Views84K

Jun 2, 2017
Wardota2 Gamer

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