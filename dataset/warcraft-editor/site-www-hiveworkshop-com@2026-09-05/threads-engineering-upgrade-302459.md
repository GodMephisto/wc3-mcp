---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/engineering-upgrade.302459/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Engineering Upgrade
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/engineering-upgrade.302459/>
#### Rogue Kessler

#### Rogue Kessler

 Level 10

JoinedApr 4, 2010Messages288

Resources: 5

#### Resources
Maps 4Tutorials 1

Engineering Upgrade

Engineering Upgrade is a versatile ability that allows the player to improve or customize the output of their other spells. The base version comes from the Goblin Tinker hero and improves each of his other hero abilities, essentially allowing the player to level up those abilities in two directions rather than just one.

Engineering Upgrade (Passive)

Improves other Tinker abilities with each level learned:

Cluster Rockets - Larger Area.

Pocket Factory - Builds Clockwerk Goblins more quickly.

Robo-Goblin - Increases armor and Strength; enhances Demolish.


Click to expand...

In this tutorial, I’ll walk through how to use Engineering Upgrade for custom abilities, using a simple example and then a more complex example.

Simple Example

To start, let’s use Engineering Upgrade to just modify one skill. We’ll base our placeholder skill on Storm Bolt.

The way that Engineering Upgrade works is that there are actually 4 versions of each of the affected abilities: A base version, and then three “upgraded” versions that correspond with the current learned level of Engineering Upgrade.

So, we need to create 4 versions of Storm Bolt (and of course, we’ll need our version of Engineering Upgrade):

We’ll set up the Storm Bolt abilities first. What we’re going to do is have Storm Bolt’s damage go up as the player levels up Storm Bolt, and the stun duration go up as the player levels up Engineering Upgrade.

Storm Bolt’s base values already scale up with level, so we don’t have to do any work there. For the stun duration, we’ll leave everything alone on Storm Bolt Copy (Base), and make everything a little bit better on Storm Bolt Copy (Alt Lvl 1), Storm Bolt Copy (Alt Lvl 2), and Storm Bolt Copy (Alt Lvl 3):

Storm Bolt Copy (Base)

Level 1 - Data - Damage: 100

Level 2 - Data - Damage: 225

Level 3 - Data - Damage: 350

Level 1 - Stats - Duration - Hero: 3.0

Level 2 - Stats - Duration - Hero: 3.0

Level 3 - Stats - Duration - Hero: 3.0

Level 1 - Stats - Duration - Normal: 5.0

Level 2 - Stats - Duration - Normal: 5.0

Level 3 - Stats - Duration - Normal: 5.0


Click to expand...

Storm Bolt Copy (Alt Lvl 1)

Level 1 - Data - Damage: 100

Level 2 - Data - Damage: 225

Level 3 - Data - Damage: 350

Level 1 - Stats - Duration - Hero: 4.0

Level 2 - Stats - Duration - Hero: 4.0

Level 3 - Stats - Duration - Hero: 4.0

Level 1 - Stats - Duration - Normal: 7.0

Level 2 - Stats - Duration - Normal: 7.0

Level 3 - Stats - Duration - Normal: 7.0


Click to expand...

Storm Bolt Copy (Alt Lvl 2)

Level 1 - Data - Damage: 100

Level 2 - Data - Damage: 225

Level 3 - Data - Damage: 350

Level 1 - Stats - Duration - Hero: 5.0

Level 2 - Stats - Duration - Hero: 5.0

Level 3 - Stats - Duration - Hero: 5.0

Level 1 - Stats - Duration - Normal: 9.0

Level 2 - Stats - Duration - Normal: 9.0

Level 3 - Stats - Duration - Normal: 9.0


Click to expand...

Storm Bolt Copy (Alt Lvl 3)

Level 1 - Data - Damage: 100

Level 2 - Data - Damage: 225

Level 3 - Data - Damage: 350

Level 1 - Stats - Duration - Hero: 6.0

Level 2 - Stats - Duration - Hero: 6.0

Level 3 - Stats - Duration - Hero: 6.0

Level 1 - Stats - Duration - Normal: 12.0

Level 2 - Stats - Duration - Normal: 12.0

Level 3 - Stats - Duration - Normal: 12.0


Click to expand...

Now it’s time to tell Engineering Upgrade how to use all these different abilities.

First of all, let’s get rid of the bonus damage and movement speed buffs since they aren’t necessary for the replacement effects to work. Simply set these values to zero.

Next, for each of these fields, you want to list two skills. The first skill listed is the one being replaced, while the second skill listed is the replacement. As you can see, Engineering Upgrade Level 1 replaces our base skill with the Alt Level 1 version, Level 2 replaces the Alt Level 1 version with the Alt Level 2 version, etc.

You can think of these replacements as being applied sequentially: first the Level 1 replacement, then the Level 2 replacement, and so on for as many levels as the player has learned. This means the first skill listed on each line (the one being replaced) should be same as the second skill from the previous line, not just the base skill over and over (which wouldn’t do anything).

WRONG

Level 1: Storm Bolt Copy (Base), Storm Bolt Copy (Alt Lvl 1)

Level 2: Storm Bolt Copy (Base), Storm Bolt Copy (Alt Lvl 2)

Level 3: Storm Bolt Copy (Base), Storm Bolt Copy (Alt Lvl 3)

RIGHT

Level 1: Storm Bolt Copy (Base), Storm Bolt Copy (Alt Lvl 1)

Level 2: Storm Bolt Copy (Alt Lvl 1), Storm Bolt Copy (Alt Lvl 2)

Level 3: Storm Bolt Copy (Alt Lvl 2), Storm Bolt Copy (Alt Lvl 3)


Click to expand...

You can also repeat this process if you want to do the same thing for more skills. (One instance of Engineering Upgrade can “upgrade” up to four separate abilities at once.) Otherwise, if you're not going to use them, I recommend deleting all of the values for these fields since they won’t do anything.

And that’s functionally it. You’ll want to make sure you update all the text and tooltip fields for each version of your abilities to accurately describe what they’ll do - only the tooltip for the currently active version of the skill is ever shown in-game.

Complex Example

This example is a lot more complicated than the one above, and is recommended for people who already have a solid grasp of the Object Editor (including the basics of how to use both Engineering Upgrade and Spellbook). If you have questions about particular parts of this feel free to ask in the comments, but I’m not going to go through every step of this system. I put this here as an example of one of the more ambitious ways you can use Engineering Upgrade.

On my map, I have a hero who can actively swap between different “aspects”, with each aspect modifying the outputs of the hero’s skills (think similar to Warrior stances or Death Knight presences in WoW). I use Engineering Upgrade for this, combined with a Spellbook ability which contains the options for choosing an aspect.

My full system involves at least 16 separate abilities:

4x Spellbook ability (Base version plus three Alt versions)

4x Replaced spell 1 (Base version plus three Alt versions)

4x Replaced spell 2 (Base version plus three Alt versions)

3x Choice buttons inside Spellbook (one for each choice)

Engineering Upgrade ability
The choice buttons and Engineering Upgrade are Unit abilities; all of the others are Hero abilities.

In addition to the above, which can all be handled in the Object Editor, I use a handful of triggers to detect when the player makes a choice and switch to the appropriate versions of all of their abilities. We do this by setting the current level of Engineering Upgrade for the player (remember that our version of Engineering Upgrade is a unit ability, not a hero ability).

Storm Aspect on
Events
Unit - A unit Finishes casting an ability

Conditions
(Ability being cast) Equal to Choose Storm Aspect

Actions
Unit - Set level of Elemental Aspect (Engineering Upgrade) to 2

Game - Force (Owner of (Casting unit)) to press Escape/Cancel

Earth Aspect on
Events
Unit - A unit Finishes casting an ability

Conditions
(Ability being cast) Equal to Choose Earth Aspect

Actions
Unit - Set level of Elemental Aspect (Engineering Upgrade) to 3

Game - Force (Owner of (Casting unit)) to press Escape/Cancel

Fire Aspect on
Events
Unit - A unit Finishes casting an ability

Conditions
(Ability being cast) Equal to Choose Fire Aspect

Actions
Unit - Set level of Elemental Aspect (Engineering Upgrade) to 4

Game - Force (Owner of (Casting unit)) to press Escape/Cancel

Because I have a default state for the hero where no aspects are active, Level 1 of my Engineering Upgrade doesn’t actually make any replacements. Levels 2, 3, and 4 correspond with the three “aspects” the player can pick and the associated Alt versions of each spell.

Here’s how it looks in action:

(Credit goes to @Stefan.K for the sweet Draka model! Also, thanks to @assasin_lord, @Sin'dorei300, @~Nightmare, @PeeKay, @Darkfang and @Mc ! whose resources also appear in the video.)

If you want to take a look at what these two examples (or something close to them) look like in practice, check out the test map attached below.

Notes and Best Practices

Here are some additional points about how best to use (and not use) Engineering Upgrade. These are just based on my own experience of using Engineering Upgrade and some of the issues I’ve run into.

(This section subject to future edits/additions)

Engineering Upgrade only allows you to replace Hero abilities, not unit (or item) abilities. However if you need to replace non-Hero abilities there are probably easier ways to do it anyway using triggers. (The Engineering Upgrade itself can be a unit or Hero ability, either works fine.)

Engineering Upgrade cannot be added to non-Hero units (whether EU is a hero ability or not). Doing so will cause the map to crash. (@leandrotp has developed a workaround which you can read about here.)

Whenever you’re replacing abilities using Engineering Upgrade, ALWAYS use the same base ability for all versions of the skill being replaced. If you don’t, you can end up with weird combinations of effects resulting from the replaced ability not being uninstalled properly. (This is how I accidently ended up with a 2 million damage Storm Bolt in my map.)

Similarly, don’t use Engineering Upgrade to replace itself. (This should be common sense.)

Weird things can happen when you're triggering the Engineering Upgrade level to change from a spell being cast inside a spellbook (like in my system above). Specifically, when shifting down from a higher EU level to a lower EU level, the EU tooltip can break - moving position, defaulting to a placeholder image, and losing its text. I have no idea why this happens, but you can avoid it if the spellbook is itself one of the abilities Engineering Upgrade is replacing. (The spellbook versions don't actually have to be any different from each other, there just have to be different versions "behind the scenes". Weird!)

The Engineering Upgrade buff model of the spikes sticking out of the hero's mount are notoriously hard to remove or change. (You can see them sticking out of Thrall's wolf in the test map, attached below.) I've yet to find a good workaround for this, other than just using a hero that doesn't have a mount.

I've run into trouble when using Engineering Upgrade on a hero I want to save/restore using the game cache. When restoring the saved hero, they won't have any abilities. I was able to work around this by setting Engineering Upgrade's level to 1 (or 0) so that it's not replacing any abilities, right before saving the hero. (You can use an integer in game cache to save the level Engineering Upgrade should be, and then set it to that again after restoring the hero in the next map.)
Thanks for reading!

#### Attachments

Engineering Upgrade Test Map.w3x

 32.3 KB

 · Views: 451




 Last edited: Jan 23, 2018

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Subscribing for later reading. Very exciting. : )

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

I think it would be a good idea to attach the test map so people can fiddle around with what they know is a working version of Engineering Upgrade and the complex example you posted. The combination of the spellbook and engineering upgrade is also extremely clever. I'll have to keep that in mind in the future.

#### Rogue Kessler

#### Rogue Kessler

 Level 10

JoinedApr 4, 2010Messages288

Resources: 5

#### Resources
Maps 4Tutorials 1

KILLCIDE said:

 I think it would be a good idea to attach the test map so people can fiddle around with what they know is a working version of Engineering Upgrade and the complex example you posted. The combination of the spellbook and engineering upgrade is also extremely clever. I'll have to keep that in mind in the future.


Click to expand...

It's a good idea, and something I might do in the future. For right now unfortunately my WIP (shown in the video) is part of a campaign so it has a bunch of other stuff going on. It would be a longer process to cut the pieces together into a new map.


 Last edited: Jan 20, 2018

#### Rogue Kessler

#### Rogue Kessler

 Level 10

JoinedApr 4, 2010Messages288

Resources: 5

#### Resources
Maps 4Tutorials 1

I went ahead and threw together a test map that has (something close to) the examples described in the tutorial. I removed the custom resources, so it's just icons and models that come with the game. And hey, I even learned some new things while making it!

EDIT: Edited the test map to also include the Targeting Dummy. XD

EDIT 2: Gave Muradin a Tome of Endless Retraining.


 Last edited: Jan 20, 2018

#### MasterBlaster

#### MasterBlaster

 Level 21

JoinedFeb 23, 2014Messages1,265

A really, really useful guide and as mentioned, the combination of the Spellbook and Engineering Upgrade is just plain amazing.

Obviously +rep, subscribe and a sincere hope that this tutorial gets approved asap

Also, if you have any other clever ideas like this one that you'd like to share, please do!

#### Rogue Kessler

#### Rogue Kessler

 Level 10

JoinedApr 4, 2010Messages288

Resources: 5

#### Resources
Maps 4Tutorials 1

MasterBlaster said:

 A really, really useful guide and as mentioned, the combination of the Spellbook and Engineering Upgrade is just plain amazing.

Obviously +rep, subscribe and a sincere hope that this tutorial gets approved asap

Also, if you have any other clever ideas like this one that you'd like to share, please do!

Click to expand...

Thanks!

#### MyPad

#### MyPad

 Level 31

JoinedMay 9, 2014Messages1,878

Resources: 27

#### Resources
Models 1Icons 4Maps 6Spells and Systems 7JASS 7Lua 1Tutorials 1

Something about Engineering Upgrade! Nice topic.

There is this issue about giving the Engineering Upgrade ability to units, which results into a crash on the game. It had already been explained by @WaterKnight and @leandrotp, but I could bring this up, a null pointer exception (due to attempt to access the 'AHer' ability of a unit) is the cause behind it.

Jake Kessler said:

 Whenever you’re replacing abilities using Engineering Upgrade, ALWAYS use the same base ability for all versions of the skill being replaced. If you don’t, you can end up with weird combinations of effects resulting from the replaced ability not being uninstalled properly. (This is how I accidently ended up with a 2 million damage Storm Bolt in my map.)


Click to expand...

Can attest to the weird side effects of EU (or ANeg).

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

It crashes the game when added to non-hero as it doesn't check if unit IS a hero prior to that. Stupid thing.

In order to remove FX just edit the buff u're using with skill. By default its always BNeg. Even tho buff is never visible on the hud, it's always there, as long as you're using valid buff. If you put invalid buff id default will be used instead (BNeg). Remove attachments there to get rid of spikes

Also empty fields in upgrade strings may cause crash in case if Bear form obtained. Empty field equal to 0, and 0 is the ability of back-morphing into non-bear form. DO NOT leave those fields empty. This bug is only exists in newest patches, 1.26a just fine with that.

MS bonus being provided in %%, which is useful for single unit speed-up, in case if thats hero-only buff or debuff. because no aura buff in the status

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

MyPad said:

 a null pointer exception (due to attempt to access the 'AHer' ability of a unit) is the cause behind it.


Click to expand...

Can this ability be added to normal units in that sense?

#### Rogue Kessler

#### Rogue Kessler

 Level 10

JoinedApr 4, 2010Messages288

Resources: 5

#### Resources
Maps 4Tutorials 1

WaterKnight said:

 Can this ability be added to normal units in that sense?


Click to expand...

I haven't tested this, but I assume it would be fine if your version of Engineering Upgrade is itself a non-hero ability. I can test it later if people want.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

-_____-

flags u mark 'hero' - 'non-hero' is pure bullshit for editor purposes only

ANeg crashes on non-hero, AHer cannot be obtained by non-heroes, period

#### Rogue Kessler

#### Rogue Kessler

 Level 10

JoinedApr 4, 2010Messages288

Resources: 5

#### Resources
Maps 4Tutorials 1

WelcomeBack said:

 -_____-

flags u mark 'hero' - 'non-hero' is pure bullshit for editor purposes only

ANeg crashes on non-hero, AHer cannot be obtained by non-heroes, period


Click to expand...

Tested and confirmed. I'll add this to the section at the end. Thanks!

#### MyPad

#### MyPad

 Level 31

JoinedMay 9, 2014Messages1,878

Resources: 27

#### Resources
Models 1Icons 4Maps 6Spells and Systems 7JASS 7Lua 1Tutorials 1

WaterKnight said:

 Can this ability be added to normal units in that sense?


Click to expand...

I think @leandrotp already resolved that part, along with @Zwiebelchen. Source for the thread: Here

#### Rogue Kessler

#### Rogue Kessler

 Level 10

JoinedApr 4, 2010Messages288

Resources: 5

#### Resources
Maps 4Tutorials 1

MyPad said:

 I think @leandrotp already resolved that part, along with @Zwiebelchen. Source for the thread: Here

Click to expand...

Awesome! Added a link in the OP!

#### Rogue Kessler

#### Rogue Kessler

 Level 10

JoinedApr 4, 2010Messages288

Resources: 5

#### Resources
Maps 4Tutorials 1

Added an additional issue I ran into using Engineering Upgrade with the game cache:

 I've run into trouble when using Engineering Upgrade on a hero I want to save/restore using the game cache. When restoring the saved hero, they won't have any abilities. I was able to work around this by setting Engineering Upgrade's level to 1 (or 0) so that it's not replacing any abilities, right before saving the hero. (You can use an integer in game cache to save the level Engineering Upgrade should be, and then set it to that again after restoring the hero in the next map.)


Click to expand...

#### Darkfang

#### Darkfang

 Level 57

JoinedJun 25, 2008Messages2,382

Resources: 408

#### Resources
Models 17Icons 352Skins 32Maps 4Ported Icons 1Tutorials 2

If u ask me, it is just better to add a damage detection system and trigger the whole thing, it suposes less abilities per aspect.

Edit: but yeah, changing the icons is cool though.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great tutorial and reference. I love the video + examples!

Approved.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Love it. Extremely well-written & informative, with lots of helpful images. Even a video & a test map!

One suggestion: the section #3 reads

 Whenever you’re replacing abilities using Engineering Upgrade, ALWAYS use the same base ability for all versions of the skill being replaced. If you don’t, you can end up with weird combinations of effects resulting from the replaced ability not being uninstalled properly. (This is how I accidently ended up with a 2 million damage Storm Bolt in my map.)


Click to expand...

Thing is, this is one of the most fascinating elements of using Engineering Upgrade to me, and one I want to encourage people to explore, test, and document! While it's true that some, perhaps even most of the time, the effects will be useless/detrimental (e.g. your 2mil-Storm-Bolt), I've heard rumors that some incredibly cool combinations can be attained.

Basically, just rewriting it to be less about "NEVER DO THIS" to "Be careful when upgrading to different abilities; use the same base ability to ensure a seamless transition". Perhaps even to "There's anecdotal evidence that certain abilities can be combined in interesting ways to create entirely new abilities! Test it out & post your results here!"

#### Tipsilon

#### Tipsilon

 Level 6

JoinedSep 22, 2011Messages85

Hi everyone, you guys seem very knowledgeable about this subject, and i really need help with this ability(namely with the last issue jake mentioned - heroes not having abilities when restored from the game cache in campaigns).

[Spell] - Engineering Upgrade in Campaigns

#### Yevhen Snizhko

#### Yevhen Snizhko

 Level 12

JoinedMay 13, 2023Messages157

Resources: 4

#### Resources
Models 1Maps 2Tutorials 1

About restoring hero with EU from cache - I've managed to do this on base Tinker with triggers, although it is somewhat over-the-top.

Storing:

Store Tinker

Events

Player - Player 1 (Red) types a chat message containing coco as An exact match

Conditions

Actions

Game Cache - Create a game cache from Testing.w3v

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Unit-type of HeroTinker) Equal to Tinker

(Unit-type of HeroTinker) Equal to Tinker

Then - Actions
Game Cache - Store (Level of Engineering Upgrade for HeroTinker) as Engineering of Tinker in (Last created game cache)

Unit - Remove Engineering Upgrade from HeroTinker

Game Cache - Store HeroTinker as HeroTinker of Heroic in (Last created game cache)

Game Cache - Save (Last created game cache).

Else - Actions

Restoring:

Restore Tinker

Events

Player - Player 1 (Red) types a chat message containing dodo as An exact match

Conditions

Actions

Game Cache - Create a game cache from Testing.w3v

Game Cache - Restore HeroTinker of Heroic from (Last created game cache) for Player 1 (Red) at (Center of cacher <gen>) facing 0.00

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Unit-type of (Last restored unit)) Equal to Tinker

(Unit-type of (Last restored unit)) Equal to Tinker

Then - Actions
Hero - Modify unspent skill points of (Last restored unit): Add (Load Engineering of Tinker from (Last created game cache)) points

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Load Engineering of Tinker from (Last created game cache)) Not equal to 0

Then - Actions
For each (Integer A) from 1 to (Load Engineering of Tinker from (Last created game cache)), do (Actions)
Loop - Actions
Hero - Learn skill for (Last restored unit): Engineering Upgrade

Else - Actions

Else - Actions

Explanation:

1. Current version of WC3 editor does not allow to set ability level below 1, so setting it to 0 is impossible. Setting it to something when restoring also does not work.

2. I check for both versions of Tinker (they have no suffixes, but IDs are different - one is neutral melee hero and the other one is neutral melee special).

3. To work around the non-0 level of EU, I had to remove it - no ability on unit to break it, no problem

4. Restored unit automatically has the ability (and random proper name is also changed, which means restored unit is fully reset to starting state). I add missing points and spend them on EU.

5. Checking for non-0 level of EU is a safeguard, it works fine without it.


 Last edited: Jul 18, 2023

#### Regno

#### Regno

 Level 9

JoinedSep 16, 2016Messages248

I guess you are limited to changing 4 abilities per tinker upgrade, if you want to swap 5, then I guess you need 2x tinkers ability on the hero?

#### Yevhen Snizhko

#### Yevhen Snizhko

 Level 12

JoinedMay 13, 2023Messages157

Resources: 4

#### Resources
Models 1Maps 2Tutorials 1

Regno said:

 I guess you are limited to changing 4 abilities per tinker upgrade, if you want to swap 5, then I guess you need 2x tinkers ability on the hero?


Click to expand...

In the core World Editor, you can't have more than 5 hero abilities, and Engineering Upgrade is one of them.

Maybe if you want to somehow change Engineering Upgrade using Engineering Upgrade, you will need a lot more Engineering Upgrade copies - but I don't know how that will work.

And I'm not sure how it would work if you use it to upgrade non-hero abilities, or make a non-hero version of it.

All of these cases would be cool experiments though.

#### Regno

#### Regno

 Level 9

JoinedSep 16, 2016Messages248

Yevhen Snizhko said:

 In the core World Editor, you can't have more than 5 hero abilities, and Engineering Upgrade is one of them.

Maybe if you want to somehow change Engineering Upgrade using Engineering Upgrade, you will need a lot more Engineering Upgrade copies - but I don't know how that will work.

And I'm not sure how it would work if you use it to upgrade non-hero abilities, or make a non-hero version of it.

All of these cases would be cool experiments though.


Click to expand...

Yeah for sure. I use it quite simply, just swap Q W E R T to alternative variant, all hero abilities

Sadly Engineer Upgrade is not enough, since it can only do up to 4

#### mappynooby

#### mappynooby

 Level 3

JoinedSep 8, 2025Messages15

Rogue Kessler said:

 Weird things can happen when you're triggering the Engineering Upgrade level to change from a spell being cast inside a spellbook (like in my system above). Specifically, when shifting down from a higher EU level to a lower EU level, the EU tooltip can break - moving position, defaulting to a placeholder image, and losing its text. I have no idea why this happens, but you can avoid it if the spellbook is itself one of the abilities Engineering Upgrade is replacing. (The spellbook versions don't actually have to be any different from each other, there just have to be different versions "behind the scenes". Weird!)


Click to expand...

Anyone know what to do about the tooltips spazzing? I'm not even using spell book and mine are going haywire like making extra space at the top, and so the words actually appear down below, outside of the text window. Then it will move around.

 You must log in or register to reply here.


### Similar threads

A

Locked

 Transformation ability and Engine Upgrade


archimonde92

Feb 20, 2011

World Editor Help Zone

Replies7Views2K

Feb 22, 2011
defskull

Locked

 Engineering Upgrade


Ravecharmer

Feb 23, 2020

World Editor Help Zone

Replies2Views2K

Feb 24, 2020
Ravecharmer

Locked

 De-Engineering Upgrade?


Holla Atcha Boy

Jan 28, 2006

Triggers & Scripts

Replies0Views1K

Jan 28, 2006
Holla Atcha Boy

 Basic Upgrades


daxtreme

Jun 2, 2007

Misc/General Mapping Tutorials

Replies7Views16K

Sep 17, 2007
pig_ninja

I

Locked

 Engineering Upgrade icon wrong :(


InvalidCola

Sep 5, 2005

Triggers & Scripts

Replies4Views1K

Sep 6, 2005
Daelin

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