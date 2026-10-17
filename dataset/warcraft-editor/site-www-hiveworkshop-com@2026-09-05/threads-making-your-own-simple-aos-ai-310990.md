---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-your-own-simple-aos-ai.310990/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making Your Own Simple AoS AI
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-your-own-simple-aos-ai.310990/>
#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

#
 INTRODUCTION

Greetings everyone, this is Daffa the Mage bringing you an AI related tutorial. Before moving on, this tutorial is a theory-typed tutorial, thus there will be a lot of logic and... MATH involved. Some text rain too. There will also be some sample codes to help to understand in Warcraft 3 context.

For this tutorial, I will use DotA Template from TheHelperNet for GUI triggers in this tutorial.

#
 GETTING STARTED

Before proceeding, I hope you have a basic understanding of maths (addition, subtraction, multiplication, and division) as well as logic (>,<,>=,<=,==,!=) beforehand.

For Warcraft 3 materials :

World Editor (1.24+)

Decent Understanding of Trigger Editor (this tutorial uses GUI, for Wurst, Jass and vJass user who's allergic to GUI, please pardon me)

I also assume you understand that this is exclusively focused on AoS genre. For other genres, I will have a different tutorial approaching them.

#
 WHAT IS AI?

I'll quote Wikipedia here :

 In computer science, Artificial intelligence (AI), sometimes called machine intelligence, is intelligence demonstrated by machines, in contrast to the natural intelligence displayed by humans and other animals.


Click to expand...

In Warcraft 3 context, this means that AI is the part where Computer players act and display a sense of intelligence as an attempt to actively partake in the game.

A map with AI greatly increases the enjoyment of the map itself, and the replayability will increase with the existence of AI. The more dynamic the AI is, the further replayability it gives. In addition, it allows a map to be played in singleplayer (the poor people who are too busy to arrange schedule with others or afford Warcraft 3). This makes AI one of the most rewarding things to do for a mapper.

Truth be told, the difficulty justifies this

#
 AI IN AOS

So, now, we'll get started with the theories. Let's go!

##
 RESTRICTIONS

In AoS, there are several basic restrictions to remember :

Each player only has one 'main' hero

Gold is gained by 3 methods: Killing, Assist and Auto-Generation

There are items to buy

Each hero has it's own skill set

There are some stat attributes, different per hero
There are differences between AoS, but I think this is quite general. Some maps can have more than one 'main' hero, some other maps have extra or less gain gold method, some maps doesn't use items and unique skill set but selectable skill tree, some maps has unique attributes.

For now however, we presume all those restrictions above are followed.

##
 ACTIONS

In AoS, there are actions that a hero can do :

Attack

Heal

Standby

Skill Selection

Skill-Cast

Purchase Items

Use Items
Outside of these exist the likes of Ambush, Jungle and so on. But for now, this 7 basic will be the ones we cover. Now, let's get down to the approaches we can use.

#
 APPROACH TO AOS AI

There are a lot of approaches for AoS AI. The simplest one would be the suicidal AI approach. With Skill-Cast handled by Warcraft 3 (unless Channel is used), one would only need to construct the Skill Selection and Purchase Items. This is by far the fastest to code between them (even faster if Item Purchasing is skipped out). One main flaw is the fact AI dies easily, and in AoS with punishing death, it's a no-go.

The next approach is the 'state' approach. This approach makes the user involved in Attack, Heal, Skill Selection and Item Purchasing parts. Skill-Cast and Use Items is usually parted to Warcraft 3 default mechanic (though some code it themselves) and Standby is usually scrapped out to simplify the code. This AI is flawed when it comes to denying kills and it lacks complex decision making.

The last approach I found, which is mostly theory but surely applicable to DotA's successor, DotA 2, according to one paper I read is the Fuzzy Logic approach. This approach allows a player to have more dynamic gameplay and makes the AI adapt to the player. This one is very complex, and since I still study this approach, I will mostly focus on the theory (lots of math here).

Other than these 3 approaches, they usually are still within bound of one of these three, so I suppose these should be sufficient. For this tutorial, I will focus on Suicidal AI only. Finite State improves from Suicidal by adding decision on what 'state' the AI is in. Fuzzy further improves it by giving AI the weighing of available options.

#
 WHAT'S NEXT?

Making Your Own AoS AI - Suicidal AI Approach


 Last edited: Jun 25, 2019

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

INTRODUCTION

The first approach is the Suicidal AI approach. Here, AI will ignore its current conditions and go full suicide on the enemies.

WHAT ACTIONS THEY DO?

Suicidal AI has a list of activities they do as following, based on 7 actions from the previous part of the tutorial :

Attack: Utilizes a simple Go-Attack command, then reset when AI uses skill

Heal: Ignore

Standby: Ignore

Skill Selection: Used

Skill-Cast: Utilizes default Warcraft system

Purchase Items: Used

Use Items: Utilizes default Warcraft system
As you can see above, the AI only need triggers for skill selection, purchase items, and attack command. Assuming it's a two side AoS with only 1 path, attacking would be VERY easy to do. Skill selection would only need a bunch of variables to handle the data and item purchasing unless we want a complex build, can be simplified into 1 generic build.

In our case, however, there are 3 possible paths the AI can choose. Two of the paths also have a redirection in it which means there's more to code than meets the eye.

NOW, HOW DO I MAKE THEM?

CONFIGURATION

There are several configurations we need to handle. Let's narrow them down:

The side they're on.

Target Position and Base Position.

Hero List.

Skill Tree.

Item Purchase Tree.
For this tutorial, I'll keep it simple. I got this simple configuration that should do enough of the job:

Configuration
Events
Map initialization

Conditions

Actions
-------- Side --------

For each (Integer AI_Loop) from 2 to 6, do (Actions)
Loop - Actions
Set AI_Sentinel[AI_Loop] = True

For each (Integer AI_Loop) from 8 to 12, do (Actions)
Loop - Actions
Set AI_Sentinel[AI_Loop] = False

-------- Base --------

Set AI_BaseSentinel = (Center of Hero Creation Sentinel <gen>)

Set AI_BaseScourge = (Center of Hero Creation Scourge <gen>)

-------- Hero List (Sentinel) --------

Set AI_HeroListSentinel[1] = Beastmaster

Set AI_HeroListSentinel[2] = Paladin

-------- Hero List (Scourge) --------

Set AI_HeroListScourge[1] = Dreadlord

Set AI_HeroListScourge[2] = Pit Lord

-------- Skill List (Sentinel) --------

-------- Array denotes the hero number from the hero list --------

Set AI_Skill1Sentinel[1] = Summon Bear

Set AI_Skill2Sentinel[1] = Summon Quilbeast

Set AI_Skill3Sentinel[1] = Summon Hawk

Set AI_Skill4Sentinel[1] = Stampede

Set AI_Skill1Sentinel[2] = Holy Light

Set AI_Skill2Sentinel[2] = Divine Shield

Set AI_Skill3Sentinel[2] = Devotion Aura

Set AI_Skill4Sentinel[2] = Resurrection

-------- Skill List (Scourge) --------

-------- Array denotes the hero number from the hero list --------

Set AI_Skill1Scourge[1] = Carrion Swarm

Set AI_Skill2Scourge[1] = Sleep

Set AI_Skill3Scourge[1] = Vampiric Aura

Set AI_Skill4Scourge[1] = Inferno

Set AI_Skill1Scourge[2] = Rain of Fire

Set AI_Skill2Scourge[2] = Howl of Terror

Set AI_Skill3Scourge[2] = Cleaving Attack

Set AI_Skill4Scourge[2] = Doom

As you can see, Skill Tree IS annoying. The more the hero, the merrier the skill list need to be registered. I wanted to include Item Purchasing, but I suppose I'll skip it for now. I simplify the base targeting, as they would be inverse to each other here.

One big problem is AI CANNOT purchase hero, so sadly we have to simulate them purchasing the hero. I'll skip gold reduction and stuff for now. Oh, this also breaks -ar mode, as well as AI heroes can get duplicate (can be mitigated but not now). To be fair, this relative to each AoS system.

Force Purchase
Events
Time - Elapsed game time is 30.00 seconds

Conditions

Actions
Player Group - Pick every player in (All players controlled by a Computer player) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Picked player) Not equal to Player 1 (Red)

(Picked player) Not equal to Player 7 (Green)

Then - Actions
Set Random_Hero = (Random integer number between 1 and 2)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
AI_Sentinel[(Player number of (Picked player))] Equal to True

Then - Actions
Unit - Create 1 AI_HeroListSentinel[Random_Hero] for (Picked player) at ((Picked player) start location) facing Default building facing degrees

Else - Actions
Unit - Create 1 AI_HeroListScourge[Random_Hero] for (Picked player) at ((Picked player) start location) facing Default building facing degrees

Set AI_Hero[(Player number of (Picked player))] = (Last created unit)

Set HeroNum[(Player number of (Picked player))] = Random_Hero

Quest - Display to (All enemies of (Picked player)) the Hint message: (A player has randomed + (Name of (Last created unit)))

Set Player_Already_Picked[(Player number of (Picked player))] = (Player_Already_Picked[(Player number of (Picked player))] + 1)

Player Group - Pick every player in Players and do (Actions)
Loop - Actions
Player - Make (Unit-type of (Last created unit)) Unavailable for training/construction by (Picked player)

Else - Actions

ATTACK

Now, let's commence the assault operation.

Attack Begin
Events
Time - Elapsed game time is 45.00 seconds

Conditions

Actions
Trigger - Turn on Attack <gen>

For each (Integer AI_Loop) from 2 to 12, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
AI_Sentinel[AI_Loop] Equal to True

Then - Actions
Unit - Order AI_Hero[AI_Loop] to Attack-Move To AI_BaseScourge

Else - Actions
Unit - Order AI_Hero[AI_Loop] to Attack-Move To AI_BaseSentinel

Attack
Events
Unit - A unit enters (Playable map area)

Unit - A unit Starts the effect of an ability

Unit - A unit Finishes reviving

Conditions
((Owner of (Triggering unit)) controller) Equal to Computer

(Owner of (Triggering unit)) Not equal to Player 1 (Red)

(Owner of (Triggering unit)) Not equal to Player 7 (Green)

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
AI_Sentinel[(Player number of (Triggering player))] Equal to True

Then - Actions
Unit - Order AI_Hero[(Player number of (Triggering player))] to Attack-Move To AI_BaseScourge

Else - Actions
Unit - Order AI_Hero[(Player number of (Triggering player))] to Attack-Move To AI_BaseSentinel

SKILL SELECTION

Skill Selection
Events
Unit - A unit enters (Playable map area)

Unit - A unit Gains a level

Conditions
((Owner of (Triggering unit)) controller) Equal to Computer

(Owner of (Triggering unit)) Not equal to Player 1 (Red)

(Owner of (Triggering unit)) Not equal to Player 7 (Green)

Actions
Set AI_Num = (Player number of (Triggering player))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
AI_Sentinel[AI_Num] Equal to False

Then - Actions
Custom script: call SelectHeroSkill( GetTriggerUnit(), udg_AI_Skill1Scourge[udg_HeroNum[udg_AI_Num]] )

Custom script: call SelectHeroSkill( GetTriggerUnit(), udg_AI_Skill2Scourge[udg_HeroNum[udg_AI_Num]] )

Custom script: call SelectHeroSkill( GetTriggerUnit(), udg_AI_Skill3Scourge[udg_HeroNum[udg_AI_Num]] )

Custom script: call SelectHeroSkill( GetTriggerUnit(), udg_AI_Skill4Scourge[udg_HeroNum[udg_AI_Num]] )

Else - Actions
Custom script: call SelectHeroSkill( GetTriggerUnit(), udg_AI_Skill1Sentinel[udg_HeroNum[udg_AI_Num]] )

Custom script: call SelectHeroSkill( GetTriggerUnit(), udg_AI_Skill2Sentinel[udg_HeroNum[udg_AI_Num]] )

Custom script: call SelectHeroSkill( GetTriggerUnit(), udg_AI_Skill3Sentinel[udg_HeroNum[udg_AI_Num]] )

Custom script: call SelectHeroSkill( GetTriggerUnit(), udg_AI_Skill4Sentinel[udg_HeroNum[udg_AI_Num]] )

Custom script: call SelectHeroSkill( GetTriggerUnit(), 'Aamk' )

'Aamk' is for attribute bonus.

ITEM PURCHASING

I'll be skipping this for now. It's quite a complicated thing, so I prefer to savor it for another tutorial.

WHY THIS METHOD?

Simple

Gives the bare minimum of AI for a player to test the map

WHY NOT THIS METHOD?

AI is dumb and easy to kill

AI doesn't give a proper challenge

In a proper AoS, death will yield penalty or reward the opposition, which is a distaste for Suicidal AI

Did I mention I intentionally made this only for 1 way? I'll leave the way choosing to you lad
Below is the map attached with trigger sample under 'Suicidal AI' block.

#### Attachments

Dota Template 1.0 - Suicidal AI.w3x

 369.1 KB

 · Views: 302




 Last edited: Apr 16, 2019

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Uploaded the Suicidal AI Approach. I apologize for the LOOOOOOONG delay. I'll be moving to Finite State once I had the time. Fuzzy AI will mostly be theoretical and I will rely mostly on some papers about artificial intelligence I've read for the most part.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Removed Finite State and Fuzzy. I decided this would be the basic and I prefer to not bloat this tutorial. @PurgeandFire it's ready for checking.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

My main concern here is actually not the tutorial itself but the core concept.

AI is incredibly complex and even professional developers struggle to create competitive bots in PvP games without resorting to giving them more hp/damage to compensate for the stupid decisions.

If you take the most difficult standard AI in wc3 it gets absolutely destroyed by most people who play melee ladder.

Because to address every single scenario you need an absurd amount of code.

AI should probably dodge skillshots, maintain stacking buffs if they have such abilities. Use every single item in a clever way (not using a 200 healing potion at 50 hp, using a teleport dagger to jump across walls to finish an opponent, use CC to interrupt channeling abilities, last hit minions if you have such a mechanic in an AoS)

I don't want to say such a tutorial is completely useless, I think it might be sufficient in certain campaigns/boss fights etc as hero AI.

I strongly believe that an competent AoS AI is not something you can do unless you put A LOT of dev time down, and even then it would be too complex to be a tutorial and I would rather see it in the spell section.

Thoughts?

#### Planetary

#### Planetary

 Level 15

JoinedFeb 7, 2020Messages399

Resources: 3

#### Resources
Maps 2Spells and Systems 1

Chaosy said:

 My main concern here is actually not the tutorial itself but the core concept.

AI is incredibly complex and even professional developers struggle to create competitive bots in PvP games without resorting to giving them more hp/damage to compensate for the stupid decisions.

If you take the most difficult standard AI in wc3 it gets absolutely destroyed by most people who play melee ladder.

Because to address every single scenario you need an absurd amount of code.

AI should probably dodge skillshots, maintain stacking buffs if they have such abilities. Use every single item in a clever way (not using a 200 healing potion at 50 hp, using a teleport dagger to jump across walls to finish an opponent, use CC to interrupt channeling abilities, last hit minions if you have such a mechanic in an AoS)

I don't want to say such a tutorial is completely useless, I think it might be sufficient in certain campaigns/boss fights etc as hero AI.

I strongly believe that an competent AoS AI is not something you can do unless you put A LOT of dev time down, and even then it would be too complex to be a tutorial and I would rather see it in the spell section.

Thoughts?


Click to expand...

I do have one comment as it relates to AoS agents: I created an AI for my first submitted map that was very stupid but proved to be undeniably valuable in testing the map flow, player-dependent systems and interactions, newly-created spells, etc. I'd wager for this reason any tutorial on AI could be valuable, and I'd personally place less emphasis on the competence of the end result as a fully-fledged map feature.

My recommendation for this tutorial, however, is that it include a basic state engine for making sure agents can at least make pseudo-informed decisions (return to base when low, group move to an objective point, attack a nearby hero if present, etc.). When ordered correctly, it's relatively simple to get something working for single player testing purposes (whether that's developmental, or if you want to allow players to mess around in single player i.e. to test character builds).

Ideally, each of these action items listed in the first post could be met with a simple timer chain and reading an agent's unit data (health, mana, position, assigned lane, et al.):

 Attack

Heal

Standby

Skill Selection

Skill-Cast

Purchase Items

Use Items


Click to expand...

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I agree. Can be used for testing purposes.

I will also say it can be used in a clever way. for example HotS makes great use of AI by taking control over player heroes that disconnect.

In a lot of cases a simple AI is considerably better than nothing just by casting a shield or CC even if the timing and target is not ideal.

I am not super invested in the game, but from my eye test the AI was not feeding. But HotS is also a relatively slow game in terms of time to kill a hero which gives a lot of wiggle room when it comes to running away and not giving the enemy team XP.

As I hinted at in my post, I can see this being useful if it is marketed differently.

This is not a replacement for a player and IF you want it to be so you can play a multiplayer map single player you would need to use the cheap solution of giving the bots more hp/damage or whatever to hide the sup-par decision making.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Well, without overhauling it here, doubt it can be of much improvement. I'm fine with Graveyarding.

 You must log in or register to reply here.


### Similar threads

 The AI Editor


Av3n

Dec 29, 2006

Tutorial Archive
2

Replies60Views83K

Aug 7, 2023
Gnuoy

[GUI]
 Triggering AI for an RPG


Jaijaibinx

Jul 24, 2010

Tutorial Archive

Replies5Views3K

Dec 11, 2010
PurgeandFire

[GUI]
 AI Editor: A graphical tutorial


moyackx

Mar 7, 2012

Tutorial Archive

Replies3Views6K

Jun 9, 2012
PurgeandFire

Locked

 AoS AI System


Kazeon

Apr 15, 2016

Advanced Scripting

Replies10Views4K

Apr 18, 2016
pred1980

 Making a AI


dhk_undead_lord

Nov 28, 2006

Tutorial Archive

Replies7Views2K

Dec 8, 2006
dhk_undead_lord

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