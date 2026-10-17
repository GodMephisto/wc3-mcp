---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-give-a-unit-splash-damage-via-item-spell-or-aura.311818/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Give a Unit Splash Damage via Item Spell or Aura
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-give-a-unit-splash-damage-via-item-spell-or-aura.311818/>
#### Eric Jensen

#### Eric Jensen

 Level 4

JoinedOct 11, 2018Messages6

Resources: 4

#### Resources
Maps 1Tutorials 3

##
 Introduction

This is a problem that I ran into while creating my Tower Defense map Azure TD Remastered; I wanted to create an item Orb of Fire that would give a Hero splash damage when equipped. However, the Warcraft 3 Orb of Fire that is in the melee game does not actually give a Hero true splash damage; rather, it merely does a paltry 5 damage to enemy units in a 140 radius, regardless of the Hero's actual damage. Not only does this make the Orb of Fire the worst orb in the game, it is frustrating that Blizzard did not bother to create an item that gives the Hero true splash damage as a percentage of its original damage.

As I tried Googling a solution to my problem I found that I was not alone in wondering if there was a way to have a spell, ability, or item give a unit or Hero splash damage. Fortunately, I was able to create a solution!

##
 How Splash Damage Works

Units with splash damage in Warcraft 3 deal area damage in 3 radii. For example, a  Mortar Team  deals full damage within a 25 radius, 40% damage within a 150 radius, and 10% damage within a 250 radius. A  Frost Wyrm , on the other hand, deals full damage within a 25 radius, 20% damage in a 50 radius, and 10% damage in a 200 radius.

Splash damage parameters are established in the Combat - Attack fields of the Object Editor.

Combat - Area of Effect - Full Damage The radius in which the unit deals full damage.

Combat - Area of Effect - Medium Damage The radius in which the unit deals medium damage.

Combat - Area of Effect - Small Damage The radius in which the unit deals small damage.

Combat - Damage Factor - Medium Establishes the ratio of "medium damage" to full damage. For example, typing 0.33 in this field sets the "medium damage" of the unit at 33% of the full damage.

Combat - Damage Factor - Small Establishes the ratio of "small damage" to full damage.

Combat - Area of Effect Targets Establishes which unit types can and cannot be hit by this unit's splash damage.

These radii overlap each other without stacking, with the Full Damage radius taking having highest priority and the Small Damage radius having the least priority. Thus, if you make a unit with splash radii of 200 (full), 100 (medium), and 50 (small), the unit will simply deal full damage in the 200 radius, and the medium and small damage radii won't apply.

If you wanted to create a unit that dealt more splash damage to further away targets, you can do this by setting the Medium and Small Damage Factors higher than 1. For example a unit with splash radii 50, 150, and 250 and 1.5 and 2.5 medium and small damage factors will deal 100% damage within a 50 radius, but 150% damage to units between 50 and 150 radii, and 250% damage to units within 150 to 250 radii.

It is worth noting that sometimes in the World Editor, if a unit has a full damage radius of zero, it will not deal full damage even to its initial target. For this reason, it is usually best to set the Full Damage radius to 25. That way, it will deal full damage to its target without dealing full damage to other targets. At least this has been my experience when creating maps - I did notice that the Destroyer has a Full Damage radius of 0 yet I assume it still deals damage since it's in the melee game... anyways, if you're having trouble, set the Full Damage radius to 25 just to be safe.

##
 Creating a Splash Damage Ability

Whether we want our splash damage to come through an item, ability, aura, or spell, we will need to create a splash damage ability.

As I mentioned before, Warcraft 3's Orb of Fire does not give true splash damage; rather, it only gives a flat damage amount to units in a single radius (the Orb of Fire deals 5 damage to units within a 140 radius). However, there is an ability in Warcraft 3 that happens to give actual splash damage: Orb of Annihilation.

If we view the ability Orb of Annihilation in the Object Editor, we will see some parameters that are very similar to the Splash Damage parameters:

 Art - Missile Art This sets the projectile that will be used while the unit is using this ability.

 Data - Damage Bonus Gives the unit a bonus to base damage while using this ability (this value can be 0).

 Data - Full Damage Radius The radius in which the unit will deal full damage.

 Data - Half Damage Radius The radius in which the unit will deal medium damage. Even though it says Half Damage, the actual factor can be changed.

 Data - Medium Damage Factor The ratio of damage dealt in the "Half Damage Radius"

 Data - Small Damage Factor The ratio of damage dealt in the total Area of Effect.

 Data - Area of Effect This is the total Area of Effect of the Spell. It is effectively the "Small Damage Radius".

 Data - Targets Allowed Sets which types of units can and cannot be hit by the ability's splash damage.

The original Orb of Annihilation gives the Destroyer +20 damage, and a Full Damage Radius of 1 (see I'm still confused as to why you need a radius here sometimes and sometimes not...), a 45% damage radius of 75, and a 25% damage radius of 150. Adjusting these value will change the parameters of your ability's splash damage.

However, in order to make our new Splash Damage ability compatible with any unit, we need to change a few more things:

 Data - Cast Range The default range is 450, which is the attack range of the Destroyer. Setting this value to 0 causes it to take the same range as the base attack range of the unit or Hero that carries this ability. Set this value to 0.

 Data - Mana Cost The default mana cost is 25. Leaving this unchanged will make the ability cost mana, and also prevent non-mana units from using this ability. Change the mana cost to 0.

You should also leave the Stats - Hero Ability and Stats - Item Ability as false unless you want the ability to be a Hero-Specific ability with 3 levels, in which case you should check the Hero box only. Checking the Hero or Item boxes will prevent the spell from working on normal units.

Lastly, we need to make sure that any unit or Hero that has or will possibly have this ability is compatible with splash damage. In order for a unit to work with splash damage, it must have either a Missile (Splash) or Artillery weapon type. The weapon type can be changed in the Combat - Weapon Type field of the unit in the Object Editor. Both weapon types work the same with splash damage, but setting the weapon type to Artillery will give the unit the Attack Ground ability. I recommend simply setting the weapon type to Missile (Splash).

Also, remember that because our ability was based off of the Orb of Annihilation ability, autocast must be turned ON for the ability to work. Also, the ability won't stack with any other attack-modifying effects from Orbs or abilities (like Frost Arrow).

When the ability is active, it also overrides any splash damage that the unit may have already had. Casting this on a Mortar Team, for example, will change the Mortar Team's splash radii to the parameters set in your ability.

##
 Giving the Splash Damage ability to a unit

If all you needed was an ability giving a unit or Hero splash damage, then you are finished here! Simply give the unit that ability in the Object Editor using Abilities - Normal. However, if we are creating a splash damage ability that is given through an item, spell, or aura, we will need to use triggers to add our ability onto the target unit at the appropriate time.

###
 Splash Damage Item

With items this is easy: simply create a trigger that detects when a unit or Hero acquires an item, and gives the Splash Damage Ability to the triggering unit.

Give Item Splash Damage
Events
Unit - A unit Acquires an item

Conditions
(Item-type of (Item being manipulated)) equal to Splash Damage Item

Actions
Unit - Add Splash Damage Ability to (Triggering unit)

If we wanted this ability to work only on Heroes, and not on normal units that happen to pick up the item using Backpack, we can simply create a condition that the unit must be a Hero for the trigger to work:

Give Item Splash Damage (Hero only)
Events
Unit - A unit Acquires an item

Conditions
(Item-type of (Item being manipulated)) equal to Splash Damage Item

Actions
If ((Triggering unit) is A Hero) Equal to True) then do (Unit - Add Splash Damage Ability to (Triggering unit)) else do (Do nothing)

To remove the ability, all we need is a similar trigger:

Remove Item Splash Damage
Events
Unit - A unit Loses an item

Conditions
(Item-type of (Item being manipulated)) Equal to Splash Damage Item

Actions
Unit - Remove Splash Damage Ability from (Triggering unit))

This set of triggers could be used for example to create a true Orb of Fire item that gives a Hero actual splash damage with 3 radii and damage factors as a percentage of base damage.

###
 Splash Damage Spell

Now, if we wanted to create a spell that would give a unit our Splash Damage Ability, we would likewise have to add and remove the ability through a trigger. But first, we would have to create a dummy spell that we can use our trigger on. Basing the spell off of Unholy Frenzy is a good idea because its effects can be removed by setting the effect parameters to 0, and it is a non-autocast spell (if you wanted an autocast spell it would be better to use Inner Fire as your base).

Before we create our spell, we need to create a new custom buff that our spell will apply to a unit it is cast on. Buffs themselves don't actually have any effect on units - they are there to show that the unit is under a certain effect, in this case our Splash Damage Spell. It doesn't matter what buff we base our new buff on - we just need to specify 4 fields:

Art - Icon The icon that will appear in the Status bar of a unit under this spell's effect. I like to set this to a fire or explosion-themed icon, such as Firebolt or Barrage.

Art - Target This sets what will appear over a unit that is under your spell's effect. You can make this just about anything. I like to use the Siege Engine Barrage missile. While units are under the effect of my Splash Damage Spell, they will have a Siege Engine bullet hanging over their head.

Text - Tooltip The name of your buff that will appear if you hover over the buff icon in the unit's status bar. Let's name it Splash Damage Ability

Text - Tooltip - Extended The description of your buff that appears when you hover over the buff icon in the unit's status bar. An example could be "This unit temporarily has splash damage".

Now we can create our spell. Even though our spell is a dummy spell that has no effect on a unit, we will still want to change a few parameters of the spell in order to make it look nice and work on the appropriate units. You will need to adjust these fields:

Art - Icon - Normal sets the icon that will be used in the game's user interface (the picture that appears in the unit order menu that you click on to cast the spell). You should probably set this to the same icon that you set your custom buff as.

Data - Attack Speed Bonus this is the attack speed bonus given to the unit through Unholy Frenzy. The default is 0.75 (75% bonus). Set this field to 0.

Data - Damage Per Second  Damage per second dealt to the target of Unholy Frenzy. Set this field to zero.

Stats - Buffs  This is the buff that will be given to the target of this spell. Select your new custom buff.

Stats - Targets Allowed  Specifies which unit types can and cannot be targeted by this spell. For example, Unholy Frenzy cannot be cast on invulnerable units, as it deals damage per second. I recommend setting this field to Air, Ground, Friend, Self as this will allow the spell to be cast on any friendly unit.

Techtree - RequirementsThe original spell requires Necromancer Adept Training to be researched before this spell can be used. Remove this requirement to allow this spell to be cast.

You can also change the Name, Tooltip, Hotkey, Mana Cost, Cooldown, Duration, etc. of the spell, customizing these as you see fit.

Again, you should leave the Stats - Hero Ability and Stats - Item Ability boxes false, unless you want a Hero-specific ability, in which case check the Hero box only.

Now that we have created our Splash Damage Spell, we need to make a trigger that will give the Splash Damage Ability to units while they are under this spell's effect. As I mentioned above, make sure that every unit that you intend to receive this spell has Missile (Splash) weapon type so that it can work with splash damage.

First we make our trigger that gives a unit our Splash Damage Ability when the Splash Damage Spell is cast on it:

Splash Damage Spell
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to Splash Damage Spell

Actions
Unit - Add Splash Damage Ability to (Target unit of ability being cast)

Now, remember that the Splash Damage Ability is an autocast ability that is off by default. This would make our spell somewhat inconvenient as we would have to cast the spell on a unit, and then go to the target unit and manually activate that ability. Fortunately, there is a way we can make a trigger activate our spell automatically for us:

Splash Damage Spell
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to Splash Damage Spell

Actions
Unit - Add Splash Damage Ability to (Target unit of ability being cast)

Unit - Order (Target unit of ability being cast) to Undead Destroyer - Activate Orb Of Annihilation

You'll notice that Orb of Annihilation is not necessarily the Splash Damage Ability we created; however, because the ability was based off of Orb of Annihilation, this trigger will turn the autocast on the new ability ON. This works even on Destroyers that already have Orb of Annihilation.

The last step to creating our spell is to make the trigger remove the Splash Damage ability after the duration of the spell is over. Originally when trying to create a Splash Damage Spell I thought that I could simply make the trigger wait the duration of my spell and then remove the Splash Damage Ability:

Splash Damage Spell
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to Splash Damage Spell

Actions
Unit - Add Splash Damage Ability to (Target unit of ability being cast)

Unit - Order (Target unit of ability being cast) to Undead Destroyer - Activate Orb Of Annihilation

Wait 30.00 game-time seconds

Unit - Remove Splash Damage Ability from (Target unit of ability being cast)

However, this trigger did not work, because the "target unit of ability being cast" is no longer so after the instant that the spell is cast. The ability would stay on my units forever, even though the duration was supposed to be only 30 seconds.

The way we need to get our trigger to remove the ability properly is by detecting when units under the Splash Damage Spell lose their Splash Damage Ability buff. To do this we need to add all the units who receive the Splash Damage Spell to a unit group, and then periodically check to make sure all units in the group still carry the Splash Damage Ability buff:

First create a new unit group to put the units in. Let's call it SplashDamageAbilityUnits. Make the first trigger add units to SplashDamageAbilityUnits when they receive the Splash Damage Spell.

Splash Damage spell
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to Give Splash Damage

Actions
Unit - Add Splash Damage Ability to (Target unit of ability being cast)

Unit - Order (Target unit of ability being cast) to Undead Destroyer - Activate Orb Of Annihilation

Unit Group - Add (Target unit of ability being cast) to SplashDamageAbilityUnits

Next create a trigger that scans every unit in SplashDamageAbilityUnits for whether they still have the Splash Damage Ability buff. The trigger will remove their ability and remove the unit from SplashDamageAbilityUnits if they are found to not have the buff.

Remove Splash Damage
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in SplashDamageAbilityUnits and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked unit) has buff Ability Splash Damage) Equal to False

Then - Actions
Unit - Remove Splash Damage Ability from (Picked unit)

Unit Group - Remove (Picked unit) from SplashDamageAbilityUnits

Else - Actions
Do nothing

These two above triggers work together to add, activate, and remove the Splash Damage Ability from units under the effect of the Splash Damage Spell. Casting the Splash Damage Spell on a friendly unit will give it Splash Damage according to your Splash Damage Ability, and remove it's splash damage once the duration of the spell runs out. We've created a spell that gives units splash damage!

###
 Splash Damage Aura

You can even use similar triggers to create a Splash Damage Aura that gives units around say, a Hero, splash damage! To do this, create a dummy aura, customize its name/range/etc., set all effect parameters to 0, and set Stats - Buffs to your Splash Damage Ability buff.

Then, use a trigger to detect which units have the Splash Damage Ability buff, and add them into SplashDamageAbilityUnits. Then you can use the same trigger we used in our Splash Damage Spell to remove the effect once the unit loses the buff (aura).

Apply Splash Damage Aura
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in (Units in (Playable map area)) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked unit) has buff Splash Damage Ability) Equal to True

Then - Actions
Unit - Add Orb of Fire Splash Effect to (Target unit of ability being cast)

Unit - Order (Target unit of ability being cast) to Undead Destroyer - Activate Orb Of Annihilation

Unit Group - Add (Target unit of ability being cast) to SplashDamageAbilityUnits

Else - Actions
Do nothing

Remove Splash Damage
Events
Time - Every 3.00 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in SplashDamageAbilityUnits and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked unit) has buff Ability Splash Damage) Equal to False

Then - Actions
Unit - Remove Splash Damage Ability from (Picked unit)

Unit Group - Remove (Picked unit) from SplashDamageAbilityUnits

Else - Actions
Do nothing

Doing this will create an aura that gives units within the aura's range the Splash Damage Ability, and remove the ability once units travel out of the aura's range and lose the buff. A Splash Damage Aura does sound pretty cool, to be honest!

Again, remember to change all the potential affected units' weapon type to Missile (Splash) so that the splash damage works with them!

## Conclusion

So there you have it! Now you can create an ability, spell, item, or even an aura that gives normal units splash damage according to custom parameters!

I hope this tutorial was helpful.

-Eric Jensen [BYU_FOOTBALL]


 Last edited: Jan 15, 2019

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Out of your three tutorials, this is the one I think is the least useful one.

Not that the end-goal is not desirable, but the path there is not a good one in my opinion.

If you are willing to use a DDS system in your map it is as simple as:

event: unit takes damage

condition: damageSource has item (or has buff etc)

action: deal damage to all enemy units within X radius

Not only is this less work, but it is also way more flexible.

I'm sure there is the occasional person who refuses to use a DDS for whatever reason, but I think that is the minority and that most people use one anyway.

So for the sake of not misleading others into thinking this is the only way... graveyarded.

#### Bennoloth

#### Bennoloth

 Level 5

JoinedApr 29, 2019Messages45

Resources: 2

#### Resources
Maps 2

Hi, I just wanted to say thank you so much! This is a simple and minimalist way to add a working Orb of Fire - the way it should have been since 2002 - into the game. I added it to my Balin's Tomb map. I did find you through Google but unfortunately it was not the first result - there is another Hive thread that has faulty information and no ultimate solution. This one works!!!

#### Lamer

#### Lamer

 Level 3

JoinedDec 30, 2021Messages21

Resources: 1

#### Resources
Maps 1

Fair enough, but how to make splash damage on meele unit?

Seting unit's weapon to missile (splash) will consider it as ranged (no vampiric aura buff and trueshot aura buff).

Orb of Fire is a spell damage non based on unit's attack damage.

Triggering it through ,,damage taken" will increase it's damage when attacked unit's armor is reduced.

Blizzard just add ,,meele (splash)" factor pls.

#### Bennoloth

#### Bennoloth

 Level 5

JoinedApr 29, 2019Messages45

Resources: 2

#### Resources
Maps 2

It does work on melee units, Lamer. You just have to use Cleave from Pit Lord. Need 2 different items, 1 for melee and 1 for ranged. I bet you could dynamically give the ability based on the unit that picked up but, I didn't bother in my map.


 Last edited: Dec 19, 2025

#### Chaosium

#### Chaosium

 Level 39

JoinedAug 29, 2012Messages1,890

Resources: 10

#### Resources
Models 10

##
 Insurgent Cry


Cleave is the closest bet yes, although it has the extra effect of ignoring armor of the targets other than the main hit

#### Lamer

#### Lamer

 Level 3

JoinedDec 30, 2021Messages21

Resources: 1

#### Resources
Maps 1

Cleaving Attack is problematic:

1. it ignores armor (cleaved damage can be higher than on primary target)

2. it plays attack slam animation

 You must log in or register to reply here.


### Similar threads

Locked

 Give both ranged/melee splash damage of their basic attack damage


gurgamel

Jul 2, 2015

World Editor Help Zone

Replies6Views5K

Jul 2, 2015
Dr Super Good

 Making A Tower / Changing A Unit To a Tower By: -Prodigy-


-Prodigy-

Feb 27, 2008

Tutorial Archive

Replies3Views2K

Mar 1, 2008
Tekal

D

Locked

 AoE kills not giving experience


doubtless1

Jul 30, 2009

World Editor Help Zone

Replies2Views1K

Aug 2, 2009
Ninja_Duels

 Creating/Changing A Unit To A Tower


-Prodigy-

Aug 16, 2009

Tutorial Archive

Replies8Views2K

Sep 26, 2009
Pyritie

 Making a REAL Grenade Spell


Kidd|WhiteHeaven|

Jun 18, 2007

Tutorial Archive

Replies2Views3K

Jul 11, 2007
Wolverabid

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