---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/hero-defense-template.52225/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Hero Defense Template
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/hero-defense-template.52225/>
#### vnakira

#### vnakira

 Level 9

JoinedDec 13, 2005Messages102

Resources: 13

#### Resources
Maps 12Tutorials 1

HERO DEFENSE TEMPLATE.


$BY VNAKIRA$
…………………………………………………………


……………………………………………………………

+This tutorial and demo map contains:


+Respawn system.

……………........


+Hero morph system


…………………


+Item combination system


…………………


+Hero selection


…………………


+Heo AI. [see the map]


………………..


+Though AOS style is becoming more and more popular. Hero defense is still very awesome to some people. This includes “Hero Survival” and “Castle Defense”. Believe or not but I still play “Fan’s Castle defense” of Fanatik and Benji which created long ago. Basically, you may want to play a map with your friends with overpower heroes, high levels, and brilliant spells. Its actually fun, but that way is for noobs, here I am going to introdute to you all a deeper and somewhat more role-playing game. It is a fusion of Hero survival and RPG.


+Respawn system:


………………………………………………………………………………


-As far as we know, this is one of most important things we must to do first. Spawning mean different levels of creep. And they will become stronger aftwer a while. Here are the main requirements for spawning level.


-Must have at least 10 levels so that people don’t feel boring after a few minutes playing.


-The creep must be stronger by the time following higher spawning level.


-The creep, actually need some spells or abilities. You should do this carefully, cause this affects the difficulty level and of the game and make it interesting or not. Keep in mind that, There will be many monsters at a same time, so don’t try to add such too powerful abilities for them, AOE spells example. I think you should try with some passvie spells and some more which we can see that they are actually exist [for example: bash, curse, critical strike, frenzy, …], don’t forget their MP and levels


-The creep should not to be too over-scaling and do gives people the shiver each time they face them. You need to use your models wisely and this may depends on your sense.


-Every x time, there should be a boss spawning, the boss must be:


+Stronger than the mobs


+Have individual spels and primary attribute.


+Abit over-scaling.


+Have a lot of HP and MP.


+Have cool and awsome spells.


-There, now after you have know what should you do. Its time to make some trigger.


-In old defense style maps. People seem use only this “Every x game time, Create y group of enemy units, wait until they all die, then do it again”. This is not bad but as I said, it’s old and may cause some leak. Remember, each time you create a unit, there r always a chance to cause this base on the number of units on the map. But in our game, a Hero defense, we always have many of units on the map, right?


-OK, my idea is: Place a group of creeps on the map first. Here, I will place 18 hostile units at the Spawning point. Then, when the game starts, Order them to attack, and turn on attcking stances that we made in other triggers.


-There will be many levels of spawning, right. We wil make one trigger for each, and a general trigger to switch between them.


-Here is the first spawning trigger: It determine if a hostile unit dies, then wait 10 secs and create a Ghoul.


+Required player: Neutral hostile or another as our enemy player.


+Required region: Spawning point.


+Required variable: Victory/Defeat [Boolean]


 Spawn1

/[Array1]

//////////////////////////////////////////////////////////////////

[QUOTE]+[B][COLOR="blue"]Event[/COLOR][/B]: A unit dies

+[B][COLOR="blue"]Condition[/COLOR][/B]: Owner of triggering unit equal to Neutral Hostile.
Triggering unit is a Hero equal to false.

Triggering unit is summoned equal to false

(Spawn 2 is on) equal to fase

Defeat equal to false

Victory equal to false.

+[B][COLOR="blue"]Action[/COLOR][/B]: Wait 10 seconds.
Create a ghoul for Neutral Hostile at Spawning point.[/QUOTE]////////////////////////////////////////////////////////////////

-There, this will create a mob each time one of them die. We no longer have to create all 18 units at one time and this, at least, doesn’t cause leak if ur map is not full of them.

The Defeat and Victory are used in your “Victory/Defeat” trigger, [as true]. It ensure that the spawning will stop immediately when you want the game to end.

+Back to the “Map ini” trigger, I created a “Var setup” in my map, and set this trigger as the “Spawn[1]”.

+Do the same to all remain levels of your map, then do another trigger to switch between them. Here is the code.

----------------------------------------------------------

+Requires variable: Modmob [integer]

+Required region: Spawning point.

+Required trigger: Spawn2

////////////////////////////////////////////////////////////////

[QUOTE]-[B][COLOR="blue"]Event[/COLOR][/B]: Time – Elapsed game time is 200 seconds.

-[B][COLOR="blue"]Condition[/COLOR][/B]: WIN equal to false.
DEFEAT equal to flase.

-[B][COLOR="blue"]Action[/COLOR][/B]: Quest – display to all players the message “Enemy is advancing”
Custom script call DestroyTrigger(udg_Spawn[1])

If [all conditions are true] then do [then actions] else do [else actions]
If- conditions:
Number of units in [units owned by Neutral hostile matching (matching unit is a hero equal to false) and (matching unit is alive equal to true) and (matching unit is summoned equal to false) less than 18.

Then - actions:

Set Modmob = Number of units in [units owned by Neutral hostile matching (matching unit is a hero equal to false) and (matching unit is alive equal to true) and (matching unit is summoned equal to false)

Units – Create (18 – Modmob) Ghoul for Neutral hostile at Spawning point facing default building.

Trigger: turn on Spawn 2[/QUOTE]//////////////////////////////////////////////////////////////////////////////////////

-Do you understand? It’s simple. We first placed 18 ghouls on the map, then after 400 seconds, we want to change spawning level to 2, Then simply destroy the “Spawn 1” and turn on the “Spawn 2”. But remember that, we still must have all 18 hostile units on our map, so “set variable” and then “create 18 – modmob” to ensure this.

……………………………………………………..

-There, now we have already created necessary triggers for the creep. The last thing we should do is to make them their path, to force them attack our King/Castle/…

-This could be done by this “a unit enters region x, order it to attack-move to region y”.\

But I’m not sure this will not cause any bugs, so according to me. You should you another, more wisely trigger to replace it. Here, use this in SD_Ryoko Dota Template.

………………………………………………….

+First, creat a Dummy unit, name it “Feed Me”.

+Place it on necessary point of the map.

+Trigger for the last:

………………………………………………….

+Required region: KING/or something else. [as WORLD TREE] in my map.

///////////////////////////////////////////////////////////////////////////////////////

[QUOTE]-[B][COLOR="blue"]Event[/COLOR][/B]: A unit come within [600] of “Feed Me”

-[B][COLOR="blue"]Condition[/COLOR][/B]: Owner of triggering unit equal to Neutral Hostile

-[B][COLOR="blue"]Action[/COLOR][/B]: Order triggering unit to attack, move to center of KING.[/QUOTE]//////////////////////////////////////////////////////////////////////////////////////

-Do this again depending on your maps’ terrain, AS there are 3 in my demo map.

[B][COLOR="green"]+HERO MORPH+[/COLOR][/B]…………………………………………………………………………….

.Alright, we made it… The next thing I want to introduce is the “Hero Morphing” system.

Those this is not necessary, but, you can use it to make your game more intereting.

-Basically, this is the system made by SD_Ryoko, you can find it here easily. But I have expierienced with it, and finally, edited a bit, so that this will act fluently,…

-My idea is instead of creating a new hero each time our hero reach requiree level, we will place all our heroes on a untouchable region, than move it instantly and change owne..Well, I am proud to say that this does not cause any leak at all, but you should creadit Syoko for this, not me.

-Here is the code, our hero will be morphed by level 10, u can do it again for higher level,

I will morph the Guardian to a Paladin. [Don’t forget to place the Paladin on the map first]

+Required variable:
Mpaladin_loc [region]

Mitem_array [item array]

///////////////////////////////////////////////////////////////////////////////////////////////////

[QUOTE]+[B][COLOR="blue"]Event[/COLOR][/B]: Unit- a unit Gains a level

+[B][COLOR="blue"]Condition[/COLOR][/B]: Unit – type of (triggering unit) equal to Guardian
Hero level of (triggering unit) equal to 10

+[B][COLOR="blue"]Action[/COLOR][/B]: Set Mpaladin_loc = position of (triggering unit)
For each (integer A) from 1 to 6 do (Actions)
Loop – Actions:

Set Mitem-array(integer A) = item carried by triggering unit in slot (integer A)

Hide triggering unit

Quest – display to all players the message : (Name of Owner of (triggering unit) has become more powerful)

Trigger – Turn off this trigger

Unit – move “Paladin” instantly to Mpaladin_loc

Set “Paladin” hero level to 10, show level-up graphics.

Change onwership of Paladin to onwer of (triggering unit) and change color

Selection – select “Paladin” for owner of (triggering unit)

Trigger – Turn on this trigger

For each (integer A) from 1 to 6 do (Actions)
Loop – Actions:

Hero – Give Mitem-array (integer A) to Paladin.

Unit – remove triggering unit from the game.

Custom script call RemoveLocation(udg_MPaladin_loc)[/QUOTE]

////////////////////////////////////////////////////////////////////////////////////////////////////

-That’s it, its simple, isn’t it.?

[B][COLOR="green"]ITEM COMBINATION SYSTEM.[/COLOR][/B]……………………………………………………………………………..

………………………………………………………………………………

The third thing I want to mention is the item fusiob system.. You should really try this, cause, it’s simple but powerful and actually help make your map greater.

-Firstly, you may want to you an awsome system, it is the item charge combination. Find it in the “Dota template”, it was done in JASS, but very simple for implement.

Also I included it in my demo map, so if you want to use, just give Ryoko the credit he deserves.

-Then, I will show you how to upgrade your item with Orbs/Gems.

It’s simply left click on the Gem and the left click on the compatible item.

-How to do that ?

////////////////////////////////////////////////////////////////////////

+Go to object editor/Abilities.

+Find the ability “Finger of Death”

+Rename it to “Fuse” and change all other datum as follow:
-Data - damage: 0

-Data - Graphic dealy: 0

-Data - Gaphic dration: 0

-Stats - cast range: 0

-Stats - cooldown: 0

-Stats – Targets allowed: item

+Go to Object/Items and take the “Wand of the Wind” item, its located in charge class. Use can use another charged item, Rename it to “Gem of Chaos” or something else and give it the ability “Fuse”.

+Now, create our trigger to finish it.

………………………………………………………….

………………………………………………………….

+Here I will upgrade “Scale Armor” to “Mithril armor”

///////////////////////////////////////////////////////////////////////////////

[QUOTE]+[B][COLOR="blue"]Event[/COLOR][/B]: Unit – a unit begins casting an ability.

+[B][COLOR="blue"]Action[/COLOR][/B]:

If (All conditions are true) then do (then actions) else do (else actions)
If – Conditions:

(Ability being cast) equal to “Fuse”

(Item – type of (Target item of ability being cats)) equal to “Scale Armor”

Then – Actions:

Remove – (Target item of ability being cast)

Item – Create “Mithril Armor” at (Position of (Target item of ability being cast))

Hero – Gives last created item to (triggering unit)

Quest – display to (Player group – (Owner of trigger unit)) the hint message “Congartulations, you have obtained + (Name of last created item)

Sound – play “Divine shield” at 100% volume attached to triggering unit.

Else – Actions.

Do nothing.[/QUOTE]/////////////////////////////////////////////////////////////////////////////

That’s it. You can continue with the “If/Then/Else” action for more items. Good luck…

[B][COLOR="green"]HERO SELECTION[/COLOR][/B]…………………………………………………………………………..

………………………………………………………………………….

+Now, I will introduce a simplest hero selection you’ve ever met. In Hero Defense, this is not too importanr, you can still you Hero Tavern or other methods, but if you like this, feel free to use it. My selection is based on the “Arena selection”, it means you can place all of your heroes on theArena, then, use a Wisp to choose it. Here are the steps to get it.

………………………………………………..

……………………………………………….

+Create a Wisp first, disable all building it can construc, and give to “Locust/Invulnerable” abilities.

+Go to Object/Abilities and find the “Channel” ability, it located in the Neutral hostile category.

+Rename it to “Select Hero” and change all fields are as follow:

……………………………………………………………

-Icon

-Data – Art duration: 0

-Data – Options: Visible.

-Data – Target type: Unit target.

-Stats – Cast range: 100

-Stats – Hero ability: False

…………………………………………………………….

+Now, give the “Select Hero” to the Wisp.

+Place all of your heroes and wisps on the map, Finally, do this:

//////////////////////////////////////////////////////////////////////////////////

[QUOTE][B][COLOR="Blue"]Event[/COLOR][/B]: Unit – a unit begins channeling an ability.

+[B][COLOR="blue"]Condition[/COLOR][/B]: Ability being cast equal to “Select Hero”
(Unit – type of(Casting unit)) equal to “Wisp”

+[B][COLOR="blue"]Action[/COLOR][/B]: Unit – change Ownership of (Target unit of ability being cast) to (Owner of casting unit) and change color.
Selection – select (target unit of ability being cast) for (Owner of casting unit)

Unit – Kill (casting unit)[/QUOTE]

/////////////////////////////////////////////////////////////////////////////////

-It’s rather simple, isn’t it..^^

+OK. My tutorial is ended here. There are still too many things I want to say, but I am pleased that finally I can complete it, hope someone can find my tuto useful and never abandon WE, abandon Warcraft..

+Thanks for your endurance to read my thread…^^

……………………………………………………………………………………………………………………………………………………………..

..I will attach the demo map later, cause i can't do it now...


 Last edited: Jan 26, 2008

#### Bobo_The_Kodo

#### Bobo_The_Kodo

 Level 5

JoinedJan 15, 2007Messages199

 +Event: A unit dies

+Condition: Owner of triggering unit equal to Neutral Hostile.

Triggering unit is a Hero equal to false.

Triggering unit is summoned equal to false

(Spawn 2 is on) equal to fase

Defeat equal to false

Victory equal to false.

+Action: Wait 10 seconds.

Create a ghoul for Neutral Hostile at Spawning point.


Click to expand...

Trigger tags needed... For example:

+Event: A unit dies

+Condition: Owner of triggering unit equal to Neutral Hostile.

Triggering unit is a Hero equal to false.

Triggering unit is summoned equal to false

(Spawn 2 is on) equal to fase

Defeat equal to false

Victory equal to false.

+Action: Wait 10 seconds.

Create a ghoul for Neutral Hostile at Spawning point.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

However, they should be in proper trigger format (CnP from the Trigger Editor by means of "Copy as Text")

Also, use lists, please, and fix some of those tags that aren't working. In general, formatting!

#### troylowyee

#### troylowyee

 Level 5

JoinedApr 13, 2008Messages184

Resources: 3

#### Resources
Maps 3

Where can i download the map??

 You must log in or register to reply here.


### Similar threads

 Hero Selection Systems


Tonks

Mar 23, 2007

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jun 8, 2023
dantles1992

 Simple Triggered Hero Artificial Intelligence in GUI


DenZel94

May 2, 2007

Trigger (GUI) Editor Tutorials

Replies22Views26K

Jul 1, 2021
aloH

 Creating some basic abilities


paskovich

Apr 22, 2007

Trigger (GUI) Editor Tutorials
23

Replies113Views104K

May 26, 2021
McPhisto2051

 Hero Selection Systems


Eleandor

Aug 20, 2007

Trigger (GUI) Editor Tutorials
2

Replies51Views81K

Feb 28, 2021
Caalanuaa

S

 How to make an AI for your map


sentrywiz

Dec 18, 2013

Trigger (GUI) Editor Tutorials

Replies48Views38K

Nov 19, 2019
GAcrewkent

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