---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-fix-attack-speed-reset-in-custom-campaigns.371508/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Fix Attack Speed Reset in Custom Campaigns
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-fix-attack-speed-reset-in-custom-campaigns.371508/>
#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Introduction:

This tutorial is intended for RPG custom campaigns similar to the Founding of Durotar where the player can transition between maps and power-up their heroes with tomes and items that boosts their attributes. But it is also useful for other custom campaigns that uses Game Cache. I assume that by reading this tutorial you are aware of how Game Cache works, if not check this tutorial.

You may have noticed when playing Rexxar's campaign that the heroes' attack speed resets to default after you transition to a new map or return to the old map even though their agility is high. That is because the game does not save the current attack speed of the heroes inside the Game Cache. Instead, the game loads their default attack speed based on their level after you restore them from the Game Cache. This bug becomes more visible when you power-up the heroes with tomes and items that boost their agility. In other words, having 28 or 100 agility points doesn't make a difference.

And that's not the worst part: if you unequip those items, the heroes wiil be stuck with a permanent debuff that decreases their base attack speed!

Example: Give a hero items that boosts agility (e.g: Boots of Quel'Thalas) and check his attack speed (Fast) Now save/load this hero and check his attack speed (Average) Now drop these items and check his attack speed (Slow)

Notice: Items that directly boosts attack speed (e.g: Gloves of Haste) are not affected by this bug. Only those that modify attributes are.

While this bug may not seem like a big issue in normal custom campaigns with limited tomes and items, in RPG custom campaigns, it will ruin the experience of players wishing to maximize their heroes as their bonus atttributes become useless.

So how do we fix this? Since the Game Cache ignores bonus attributes, we will need to save the heroes without their bonus attributes and without their items. Instead, we will save them with their base attributes and save their bonus attributes and items separatly.

I/ Save Game Cache

Step 1: Saving Attributes:

The first thing we want to do is to save the current attributes of the hero excluding bonuses gained from items (green text) so we can load them later on.

Save Heroes
Events

Conditions

Actions
-------- Create Cache --------

Game Cache - Create a game cache from Campaigns.w3v

Set MyCache = (Last created game cache)

-------- Save Attributes --------

Game Cache - Store (Agility of Hero (Exclude bonuses)) as HeroAgi of Heroes in MyCache

Game Cache - Store (Strength of Hero (Exclude bonuses)) as HeroStr of Heroes in MyCache

Game Cache - Store (Intelligence of Hero (Exclude bonuses)) as HeroInt of Heroes in MyCache

Step 2: Saving Items:

The next thing we will do is to create a dummy hero and use it as a stash to carry the real hero's items into another map. We also want to make sure this dummy hero has the same base attributes and level as the real hero so we can take advantage of them in Step 3.

Save Heroes
Events

Conditions

Actions
-------- Create Cache --------

Game Cache - Create a game cache from Campaigns.w3v

Set MyCache = (Last created game cache)

-------- Save Attributes --------

Game Cache - Store (Agility of Hero (Exclude bonuses)) as HeroAgi of Heroes in MyCache

Game Cache - Store (Strength of Hero (Exclude bonuses)) as HeroStr of Heroes in MyCache

Game Cache - Store (Intelligence of Hero (Exclude bonuses)) as HeroInt of Heroes in MyCache

-------- Save Items --------

Unit - Create 1 (Unit-type of Hero) for Neutral Passive at TempPoint facing 0.00 degrees

Set StashHero = (Last created unit)

Unit - Hide StashHero

Hero - Set StashHero Hero-level to (Hero level of Hero), Hide level-up graphics

For each (Integer TempInteger) from 1 to 6, do (Hero - Give (Item carried by Hero in slot TempInteger) to StashHero)

Game Cache - Store StashHero as StashHero of Heroes in MyCache

Step 3: Saving Heroes:

The last thing we will do is reset the hero's attributes to default value and then save him inside the Game Cache. Since there is no direct way to get the base attributes of heroes, we will have to copy the dummy hero's attributes instead.

Save Heroes
Events

Conditions

Actions
-------- Create Cache --------

Game Cache - Create a game cache from Campaigns.w3v

Set MyCache = (Last created game cache)

-------- Save Attributes --------

Game Cache - Store (Agility of Hero (Exclude bonuses)) as HeroAgi of Heroes in MyCache

Game Cache - Store (Strength of Hero (Exclude bonuses)) as HeroStr of Heroes in MyCache

Game Cache - Store (Intelligence of Hero (Exclude bonuses)) as HeroInt of Heroes in MyCache

-------- Save Items --------

Unit - Create 1 (Unit-type of Hero) for Neutral Passive at TempPoint facing 0.00 degrees

Set StashHero = (Last created unit)

Unit - Hide StashHero

Hero - Set StashHero Hero-level to (Hero level of Hero), Hide level-up graphics

For each (Integer TempInteger) from 1 to 6, do (Hero - Give (Item carried by Hero in slot TempInteger) to StashHero)

Game Cache - Store StashHero as StashHero of Heroes in MyCache

-------- Save Heroes --------

Hero - Modify Agility of Hero: Set to (Agility of StashHero (Exclude bonuses))

Hero - Modify Strength of Hero: Set to (Strength of StashHero (Exclude bonuses))

Hero - Modify Intelligence of Hero: Set to (Intelligence of StashHero (Exclude bonuses))

Game Cache - Store Hero as Hero of Heroes in MyCache

-------- Save Cache --------

Game Cache - Save MyCache

II/ Load Game Cache

Step 1: Load Heroes:

After we save the hero in the previous map, we will restore him in the next map as normal.

Load Heroes
Events

Conditions

Actions
-------- Create Cache --------

Game Cache - Create a game cache from Campaigns.w3v

Set MyCache = (Last created game cache)

-------- Load Heroes --------

Game Cache - Restore Hero of Heroes from MyCache for Player 1 (Red) at RestorePoint facing 0.00

Set Hero = (Last restored unit)

Step 2: Load Attributes:

After that, we will restore his bonus attributes from the previous map.

Load Heroes
Events

Conditions

Actions
-------- Create Cache --------

Game Cache - Create a game cache from Campaigns.w3v

Set MyCache = (Last created game cache)

-------- Load Heroes --------

Game Cache - Restore Hero of Heroes from MyCache for Player 1 (Red) at RestorePoint facing 0.00

Set Hero = (Last restored unit)

-------- Load Attributes --------

Hero - Modify Agility of Hero: Set to (Load HeroAgi of Heroes from MyCache)

Hero - Modify Strength of Hero: Set to (Load HeroStr of Heroes from MyCache)

Hero - Modify Intelligence of Hero: Set to (Load HeroInt of Heroes from MyCache)

Step 3: Load Items:

Finally, we will restore the dummy hero and transfer his items to the real hero.

Load Heroes
Events

Conditions

Actions
-------- Create Cache --------

Game Cache - Create a game cache from Campaigns.w3v

Set MyCache = (Last created game cache)

-------- Load Heroes --------

Game Cache - Restore Hero of Heroes from MyCache for Player 1 (Red) at RestorePoint facing 0.00

Set Hero = (Last restored unit)

-------- Load Attributes --------

Hero - Modify Agility of Hero: Set to (Load HeroAgi of Heroes from MyCache)

Hero - Modify Strength of Hero: Set to (Load HeroStr of Heroes from MyCache)

Hero - Modify Intelligence of Hero: Set to (Load HeroInt of Heroes from MyCache)

-------- Load Items --------

Game Cache - Restore StashHero of Heroes from MyCache for Neutral Passive at RestorePoint facing 0.00

Set StashHero = (Last restored unit)

Unit - Hide StashHero

For each (Integer TempInteger) from 1 to 6, do (Hero - Give (Item carried by StashHero in slot TempInteger) to Hero)

Unit - Remove StashHero from the game

And that's it! The heroes are now restored with their current attack speed from the previous map. And removing their items will no longer cause their attack speed to decrease.

III/ FAQ

Why not simply save the attack speed?

In theory, you could save the attack speed of the heroes with the "BlzGetUnitAttackCooldown" native but there is a catch: This native returns the base attack speed rather than the current attack speed. So in order to calculate the current attack speed, you'd have to consider every single item, ability, and buff the hero acquired to get the exact value. Not really practical. Also, this native exist only in patch 1.29 or newer, so it's not an option for older patches.

What if I replace/morph the hero instead?

It won't work because even if you replace the hero after restoring him, his bugged attack speed will also transfer to the replaced hero.

What if I create a new hero instead?

Yes, this will work also. You could create a fresh hero and transfer the old hero's items and attributes to him, but there is a minor issue: Abilities. You'll have to manually re-learn them every time the hero is restored. If you're on Reforged, you could work around this using the "BlzGetAbilityId" to detect which abilities a hero has and transfer them. If not, then you'd have to check every single ability and store them in the Game Cache. Not a good idea if you have multiple heroes with replaceable abilities.

I have attached a demo campaign below for testing. Thanks to @EdgeSky for reporting this bug and @Starquizer for the map transition system used in the demo campaign.

#### Attachments

HeroAttackSpeed.w3n

 102.4 KB

 · Views: 21




 Last edited: Apr 16, 2026

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Good discovery.

There are some pet peeves however, they don't matter (realistically) but for the sake of showing best practices it should be changed.

Using "last created X" should be a variable, game cache in this case.

You have a location leak

Don't use integer A/B

I also have speculative suggestions.

If I understand correctly, the only purpose of the unit copy is to transfer the items why not save the items directly instead of saving and manipulating the unit and then transfer the items.

Alternatively, what if I transfer the original unit with the items. Then drop the items and give them back which should 'reload' the attack speed calculation

I don't know if these work I just feel like logically it should.

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Thanks for the review, @Chaosy !

Chaosy said:

 Using "last created X" should be a variable, game cache in this case.

You have a location leak

Don't use integer A/B


Click to expand...

Fixed.

Chaosy said:

 If I understand correctly, the only purpose of the unit copy is to transfer the items why not save the items directly instead of saving and manipulating the unit and then transfer the items.


Click to expand...

The purpose of the unit copy is to store items and get the current attributes of the hero without bonuses gained from tomes. I'm not sure how to save items directly though. Game Cache doesn't store items directly unless maybe I convert them to integers or strings.

Chaosy said:

 Alternatively, what if I transfer the original unit with the items. Then drop the items and give them back which should 'reload' the attack speed calculation

I don't know if these work I just feel like logically it should.


Click to expand...

I've tried that but it doesn't work sadly. None of these alternatives worked after restoring the hero:

Drop/Pick items.

Increment/Subtract attributes.

Leveling the hero.

Removing buffs for the Hero.

Bear Form / Chaos ability.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

That puzzles me.

I am not gonna throw away the tutorial over it but I do kinda want to dig into it.

I feel you should be able to mimic the original flow of events.

(on load)

create 1 unit of type X

set hero = last created unit

set level of unit

set stats of unit

give items to unit, for testing purposes you could even spawn them on the ground and order the unit to pick them up.

If that doesn't work I am mind blown.

Obviously even if this DID work it would just be a mildly cleaner it doesn't actually make the solution better functionally.

So approved despite the intriguing mystery.

As a 'side suggestion' maybe it's worth adding the other things that should be saved just to have a complete demo.

Gold, hp/mana, position etc

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Chaosy said:

 I feel you should be able to mimic the original flow of events.

(on load)

create 1 unit of type X

set hero = last created unit

set level of unit

set stats of unit

give items to unit, for testing purposes you could even spawn them on the ground and order the unit to pick them up.


Click to expand...

Yes, this was the alternative solution I had pointed out in the last question of FAQ. It will work if you don't want to save the hero's stats and items inside the Game Cache. The only downside is abilities. Unless you don't mind having to manually re-learn them after loading your heroes, you'll have to either store them inside the Game Cache or find a way to detect which abilities a hero has learned and transfer them. It's also worth noting that the Max HP cannot be changed prior to patch 1.29 so any bonus HP cannot be transfered to the new unit.

 https://www.hiveworkshop.com/threads/list-unit-abilities.313104/


 https://www.hiveworkshop.com/threads/hero-ability-list.351993/post-3605448



 Last edited: Apr 19, 2026

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Nice, then I can sleep well knowing things still make sense

 You must log in or register to reply here.


### Similar threads

 Making a Basic Campaign


Bob27

Mar 21, 2006

Misc/General Mapping Tutorials
2

Replies50Views63K

Jan 29, 2017
BooRadly

 Transitions Among Campaign Maps


Ralle

Mar 20, 2006

Misc/General Mapping Tutorials

Replies34Views42K

Feb 2, 2025
Warseeker

 Transitions in Campaigns Guide


Starquizer

Jul 20, 2012

Misc/General Mapping Tutorials

Replies2Views14K

Jul 30, 2012
Magtheridon96

Locked

[General]
 How to bind maps in campaign?


vlekje5

Aug 20, 2015

World Editor Help Zone

Replies8Views3K

Aug 21, 2015
vlekje5

Locked

[Trigger]
 Need help.. trigger for campaign


dardas

Mar 19, 2010

Triggers & Scripts

Replies6Views1K

Mar 19, 2010
Wezthal

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