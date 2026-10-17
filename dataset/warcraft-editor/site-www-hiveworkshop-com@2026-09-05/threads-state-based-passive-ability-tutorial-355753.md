---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/state-based-passive-ability-tutorial.355753/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# State Based Passive Ability Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/state-based-passive-ability-tutorial.355753/>
#### Blightsower

#### Blightsower

 Level 18

JoinedJun 20, 2014Messages556

Resources: 19

#### Resources
Packs 1Spells and Systems 17Tutorials 1

State-based Passive Ability Tutorial

By: Blightsower

Introduction

State-based passive abilities are passive abilities that take effect based on the state the unit is in, hence the name. A great example is Berserker Blood from Dota 2, an ability that increases the attack speed of a unit when its health drops below a certain threshold. State-based passives extend beyond HP checks; they can also relate to mana, the cooldown of a different ability, whether the unit is standing on blight, if the unit has a specific buff, proximity to a certain structure, time of day, Hero level, movement speed, and many other conditions as you might imagine.

The Way it was Done in the Past

When you ask people in the forum, they will usually suggest having a variable and tracking it using timers to constantly check the state of the unit, or unit groups if there are multiple. This approach is reasonable, but you will need to have many triggers to account for various factors such as pre-placed units, units learning the ability, units unlearning the ability, and the actual condition for the state you’re trying to track.

The Hypothesis

The tutorial on passives with cooldown has revealed that the Undead ability Exhume Corpses actually triggers a cast event. We now have an easy way to check for unit states by hijacking the cast trigger of Exhume Corpses. Everything can be contained within that trigger since all of the caster's states are available from the event response—Triggering Unit. The key part of the spell is to set the number of corpses to 0, the unit type to nothing, and the duration for all levels to 0.03 (the same time interval you'd use for triggered spells).

What We Need

We need the following to create a successful unit state check passive without having too much triggers:

1. A custom Exhume Corpses with its number of corpses modified to 0, unit-type to nothing and duration of 0.03

2. A disabled spell book which will contain the bonuses our unit will receive once the desired state is reached (Note: For this tutorial the disabled spell books are named Bonuses 1, Bonuses 2, Bonuses 3, Bonuses 4, and Bonuses 5. Each spell book has 4 levels and each has their own passives granted to the unit per level). More information about spell books can be found here.

3. A custom Cargo Hold (Meat Wagon) or the default one if you don't need it elsewhere. (You need to add your custom Cargo Hold to the unit for it to work)

4. Two Triggers, one to set our configurable and disable the spell book. The other trigger handles when the bonus is going to get applied.

Examples of Common State-based Passives

### HP is over 70%

### HP is under 30%

### Mana is higher than 70%

### Mana is lower than 30%

### When A different Spell is on Cooldown

### When a spell is not on Cooldown

Unit gets the bonus if its Health is above 70%

Health is Higher Init
Events
Map initialization

Conditions

Actions
Set VariableSet HealthIsHigher_Ability = When HP is Higher (Ability)

Set VariableSet HealthIsHigher_Bonus = Bonuses 1

Set VariableSet HealthIsHigher_Threshold[1] = 70.00

Set VariableSet HealthIsHigher_Threshold[2] = 70.00

Set VariableSet HealthIsHigher_Threshold[3] = 70.00

Set VariableSet HealthIsHigher_Threshold[4] = 70.00

For each (Integer Temp_Looper) from 1 to 24, do (Actions)
Loop - Actions
Player - Disable HealthIsHigher_Bonus for (Player(Temp_Looper))

Health is Higher Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to HealthIsHigher_Ability

Actions
Set VariableSet HealthIsHigher_Caster = (Triggering unit)

Set VariableSet HealthIsHigher_AbilityLevel = (Level of HealthIsHigher_Ability for HealthIsHigher_Caster)

Set VariableSet HealthIsHigher_BonusLevel = (Level of HealthIsHigher_Bonus for HealthIsHigher_Caster)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of HealthIsHigher_Caster) Greater than or equal to HealthIsHigher_Threshold[HealthIsHigher_AbilityLevel]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HealthIsHigher_BonusLevel Greater than 0

Then - Actions
Unit - Set level of HealthIsHigher_Bonus for HealthIsHigher_Caster to HealthIsHigher_AbilityLevel

Else - Actions
Unit - Add HealthIsHigher_Bonus to HealthIsHigher_Caster

Unit - Set level of HealthIsHigher_Bonus for HealthIsHigher_Caster to HealthIsHigher_AbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HealthIsHigher_BonusLevel Greater than 0

Then - Actions
Unit - Remove HealthIsHigher_Bonus from HealthIsHigher_Caster

Else - Actions

Unit gets the bonus if its health is lower than 30%

Health is Lower Init
Events
Map initialization

Conditions

Actions
Set VariableSet HealthIsLower_Ability = When HP is Lower (Ability)

Set VariableSet HealthIsLower_Bonus = Bonuses 2

Set VariableSet HealthIsLower_Threshold[1] = 30.00

Set VariableSet HealthIsLower_Threshold[2] = 30.00

Set VariableSet HealthIsLower_Threshold[3] = 30.00

Set VariableSet HealthIsLower_Threshold[4] = 30.00

For each (Integer Temp_Looper) from 1 to 24, do (Actions)
Loop - Actions
Player - Disable HealthIsLower_Bonus for (Player(Temp_Looper))

Health is Lower Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to HealthIsLower_Ability

Actions
Set VariableSet HealthIsLower_Caster = (Triggering unit)

Set VariableSet HealthIsLower_AbilityLevel = (Level of HealthIsLower_Ability for HealthIsLower_Caster)

Set VariableSet HealthIsLower_BonusLevel = (Level of HealthIsLower_Bonus for HealthIsLower_Caster)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of HealthIsLower_Caster) Less than or equal to HealthIsLower_Threshold[HealthIsLower_AbilityLevel]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HealthIsLower_BonusLevel Greater than 0

Then - Actions
Unit - Set level of HealthIsLower_Bonus for HealthIsLower_Caster to HealthIsLower_AbilityLevel

Else - Actions
Unit - Add HealthIsLower_Bonus to HealthIsLower_Caster

Unit - Set level of HealthIsLower_Bonus for HealthIsLower_Caster to HealthIsLower_AbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HealthIsLower_BonusLevel Greater than 0

Then - Actions
Unit - Remove HealthIsLower_Bonus from HealthIsLower_Caster

Else - Actions

Unit gets the bonus if its mana is higher than 70%

Mana is Higher Init
Events
Map initialization

Conditions

Actions
Set VariableSet ManaIsHigher_Ability = When MP is Higher (Ability)

Set VariableSet ManaIsHigher_Bonus = Bonuses 4

Set VariableSet ManaIsHigher_Threshold[1] = 70.00

Set VariableSet ManaIsHigher_Threshold[2] = 70.00

Set VariableSet ManaIsHigher_Threshold[3] = 70.00

Set VariableSet ManaIsHigher_Threshold[4] = 70.00

For each (Integer Temp_Looper) from 1 to 24, do (Actions)
Loop - Actions
Player - Disable ManaIsHigher_Bonus for (Player(Temp_Looper))

Mana is Higher Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to ManaIsHigher_Ability

Actions
Set VariableSet ManaIsHigher_Caster = (Triggering unit)

Set VariableSet ManaIsHigher_AbilityLevel = (Level of ManaIsHigher_Ability for ManaIsHigher_Caster)

Set VariableSet ManaIsHigher_BonusLevel = (Level of ManaIsHigher_Bonus for ManaIsHigher_Caster)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage mana of ManaIsHigher_Caster) Greater than or equal to ManaIsHigher_Threshold[ManaIsHigher_AbilityLevel]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ManaIsHigher_BonusLevel Greater than 0

Then - Actions
Unit - Set level of ManaIsHigher_Bonus for ManaIsHigher_Caster to ManaIsHigher_AbilityLevel

Else - Actions
Unit - Add ManaIsHigher_Bonus to ManaIsHigher_Caster

Unit - Set level of ManaIsHigher_Bonus for ManaIsHigher_Caster to ManaIsHigher_AbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ManaIsHigher_BonusLevel Greater than 0

Then - Actions
Unit - Remove ManaIsHigher_Bonus from ManaIsHigher_Caster

Else - Actions

Unit gets the bonus if its mana is lower than 30%

Mana is Lower Init
Events
Map initialization

Conditions

Actions
Set VariableSet ManaIsLower_Ability = When MP is Lower (Ability)

Set VariableSet ManaIsLower_Bonus = Bonuses 3

Set VariableSet ManaIsLower_Threshold[1] = 30.00

Set VariableSet ManaIsLower_Threshold[2] = 30.00

Set VariableSet ManaIsLower_Threshold[3] = 30.00

Set VariableSet ManaIsLower_Threshold[4] = 30.00

For each (Integer Temp_Looper) from 1 to 24, do (Actions)
Loop - Actions
Player - Disable ManaIsLower_Bonus for (Player(Temp_Looper))

Mana is Lower Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to ManaIsLower_Ability

Actions
Set VariableSet ManaIsLower_Caster = (Triggering unit)

Set VariableSet ManaIsLower_AbilityLevel = (Level of ManaIsLower_Ability for ManaIsLower_Caster)

Set VariableSet ManaIsLower_BonusLevel = (Level of ManaIsLower_Bonus for ManaIsLower_Caster)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage mana of ManaIsLower_Caster) Less than or equal to ManaIsLower_Threshold[ManaIsLower_AbilityLevel]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ManaIsLower_BonusLevel Greater than 0

Then - Actions
Unit - Set level of ManaIsLower_Bonus for ManaIsLower_Caster to ManaIsLower_AbilityLevel

Else - Actions
Unit - Add ManaIsLower_Bonus to ManaIsLower_Caster

Unit - Set level of ManaIsLower_Bonus for ManaIsLower_Caster to ManaIsLower_AbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ManaIsLower_BonusLevel Greater than 0

Then - Actions
Unit - Remove ManaIsLower_Bonus from ManaIsLower_Caster

Else - Actions

Unit gets the bonus if the remaining cooldown of Thunder Clap is greater than 2

Spell On Cooldown Init
Events
Map initialization

Conditions

Actions
Set VariableSet SpellOnCooldown_Ability = When Spell is CD (Ability)

Set VariableSet SpellOnCooldown_Bonus = Bonuses 3

Set VariableSet SpellOnCooldown_AbilityOnCD = Thunder Clap

Set VariableSet SpellOnCooldown_Threshold[1] = 2.00

Set VariableSet SpellOnCooldown_Threshold[2] = 2.00

Set VariableSet SpellOnCooldown_Threshold[3] = 2.00

Set VariableSet SpellOnCooldown_Threshold[4] = 2.00

For each (Integer Temp_Looper) from 1 to 24, do (Actions)
Loop - Actions
Player - Disable SpellOnCooldown_Bonus for (Player(Temp_Looper))

Spell On Cooldown Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to SpellOnCooldown_Ability

Actions
Set VariableSet SpellOnCooldown_Caster = (Triggering unit)

Set VariableSet SpellOnCooldown_AbilityLevel = (Level of SpellOnCooldown_Ability for SpellOnCooldown_Caster)

Set VariableSet SpellOnCooldown_BonusLevel = (Level of SpellOnCooldown_Bonus for SpellOnCooldown_Caster)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability Cooldown Remaining of SpellOnCooldown_Caster for ability SpellOnCooldown_AbilityOnCD..) Greater than SpellOnCooldown_Threshold[SpellOnCooldown_AbilityLevel]

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SpellOnCooldown_BonusLevel Greater than 0

Then - Actions
Unit - Set level of SpellOnCooldown_Bonus for SpellOnCooldown_Caster to SpellOnCooldown_AbilityLevel

Else - Actions
Unit - Add SpellOnCooldown_Bonus to SpellOnCooldown_Caster

Unit - Set level of SpellOnCooldown_Bonus for SpellOnCooldown_Caster to SpellOnCooldown_AbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SpellOnCooldown_BonusLevel Greater than 0

Then - Actions
Unit - Remove SpellOnCooldown_Bonus from SpellOnCooldown_Caster

Else - Actions

Unit gets the bonus if Thunder Clap is not on cooldown

Spell not on CD Init
Events
Map initialization

Conditions

Actions
Set VariableSet SpellNotCD_Ability = When Spell is not CD (Ability)

Set VariableSet SpellNotCD_Bonus = Bonuses 5

Set VariableSet SpellNotCD_AbilityNotCD = Thunder Clap

For each (Integer Temp_Looper) from 1 to 24, do (Actions)
Loop - Actions
Player - Disable SpellNotCD_Bonus for (Player(Temp_Looper))

Spell not on CD Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to SpellNotCD_Ability

Actions
Set VariableSet SpellNotCD_Caster = (Triggering unit)

Set VariableSet SpellNotCD_AbilityLevel = (Level of SpellNotCD_Ability for SpellNotCD_Caster)

Set VariableSet SpellNotCD_BonusLevel = (Level of SpellNotCD_Bonus for SpellNotCD_Caster)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability Cooldown Remaining of SpellNotCD_Caster for ability SpellNotCD_AbilityNotCD..) Equal to 0.00

(Level of SpellNotCD_AbilityNotCD for SpellNotCD_Caster) Greater than 0

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SpellNotCD_BonusLevel Greater than 0

Then - Actions
Unit - Set level of SpellNotCD_Bonus for SpellNotCD_Caster to SpellNotCD_AbilityLevel

Else - Actions
Unit - Add SpellNotCD_Bonus to SpellNotCD_Caster

Unit - Set level of SpellNotCD_Bonus for SpellNotCD_Caster to SpellNotCD_AbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SpellNotCD_BonusLevel Greater than 0

Then - Actions
Unit - Remove SpellNotCD_Bonus from SpellNotCD_Caster

Else - Actions

Let's Make a Template

After looking at some of the examples, we have observed that there is a pattern that keeps repeating. Let's save ourselves from the laborious task of creating the same variables over and over every time we need a new state-based passive. Let's make a template where we only need to plug in our Main_Ability, Bonus_Ability, and the Conditions that needs to be met in order for our bonuses to be applied. We'll use the prefix "SBP" to prevent it from colliding with other systems. Here is what I propose:

### Template

The first trigger of our template is where we will disable the bonuses spell books for all players since we don't want it to be visible when we add it to the unit. It will also help create the illusion that the bonuses just appear when conditions are met and its didn't come from a visible ability with an icon.

State Based Passive Disabler
Events
Map initialization

Conditions

Actions
Set VariableSet SBP_BonusesCount = 5

Set VariableSet SBP_Bonuses[1] = Bonuses 1

Set VariableSet SBP_Bonuses[2] = Bonuses 2

Set VariableSet SBP_Bonuses[3] = Bonuses 3

Set VariableSet SBP_Bonuses[4] = Bonuses 4

Set VariableSet SBP_Bonuses[5] = Bonuses 5

For each (Integer SBP_PlayerLooper) from 1 to 24, do (Actions)
Loop - Actions
For each (Integer SBP_BonusesLooper) from 1 to SBP_BonusesCount, do (Actions)
Loop - Actions
Player - Disable SBP_Bonuses[SBP_BonusesLooper] for (Player(SBP_PlayerLooper))

The second part of our template is where we will actually define our conditions and which abilities are getting checked.

 * Replace all instances of "Your_Ability" with your custom main ability variable

 * Replace all instances of "Your_Bonus_Ability" with your custom bonus ability variable

 * Replace all instances of "Your_Condition" with your custom condition

State Based Passive Template
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Your_Ability

Actions
Set VariableSet SBP_Caster = (Triggering unit)

-------- Edit only the parts where it says Your_Ability, Your_Bonus_Ability and Your_Condition --------

-------- and the rest should work just fine --------

Set VariableSet SBP_MainAbility = Your_Ability

Set VariableSet SBP_BonusAbility = Your_Bonus_Ability

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

Set VariableSet SBP_MainAbilityLevel = (Level of SBP_MainAbility for SBP_Caster)

Set VariableSet SBP_BonusAbilityLevel = (Level of SBP_BonusAbility for SBP_Caster)

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Your_Condition Equal to True

Then - Actions
Set VariableSet SBP_IsValid = True

Else - Actions
Set VariableSet SBP_IsValid = False

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_IsValid Equal to True

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
Unit - Add SBP_BonusAbility to SBP_Caster

Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Remove SBP_BonusAbility from SBP_Caster

Else - Actions

Example Template Usage

### Unit is on blight

### Unit has Frost Armor

### Unit is near the Altar

### Unit is Fast

### Unit level is even

### Daytime

Unit gets the bonus when its on blight.

Unit on Blight Init
Events
Map initialization

Conditions

Actions
Set VariableSet Sample_UnitOnBlightAbility = Unit on Blight (Ability)

Set VariableSet Sample_UnitOnBlightBonus = Bonuses 5

Unit on Blight Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Sample_UnitOnBlightAbility

Actions
Set VariableSet SBP_Caster = (Triggering unit)

-------- Edit only the parts where it says Your_Ability, Your_Bonus_Ability and Your_Condition --------

-------- and the rest should work just fine --------

Set VariableSet SBP_MainAbility = Sample_UnitOnBlightAbility

Set VariableSet SBP_BonusAbility = Sample_UnitOnBlightBonus

-------- --------

Set VariableSet Sample_UnitOnBlightPoint = (Position of SBP_Caster)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Sample_UnitOnBlightPoint is blighted) Equal to True

Then - Actions
Set VariableSet SBP_IsValid = True

Else - Actions
Set VariableSet SBP_IsValid = False

Custom script: call RemoveLocation(udg_Sample_UnitOnBlightPoint)

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

Set VariableSet SBP_MainAbilityLevel = (Level of SBP_MainAbility for SBP_Caster)

Set VariableSet SBP_BonusAbilityLevel = (Level of SBP_BonusAbility for SBP_Caster)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_IsValid Equal to True

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
Unit - Add SBP_BonusAbility to SBP_Caster

Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Remove SBP_BonusAbility from SBP_Caster

Else - Actions

Unit gets the bonus when it has Frost Armor

Unit has Frost Armor Init
Events
Map initialization

Conditions

Actions
Set VariableSet Sample_UnitHasFArmorAbility = Unit has Frost Armor (Ability)

Set VariableSet Sample_UnitHasFArmorBonus = Bonuses 3

Set VariableSet Sample_UnitHasFArmorBuff = Frost Armor

Unit has Frost Armor Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Sample_UnitHasFArmorAbility

Actions
Set VariableSet SBP_Caster = (Triggering unit)

-------- Edit only the parts where it says Your_Ability, Your_Bonus_Ability and Your_Condition --------

-------- and the rest should work just fine --------

Set VariableSet SBP_MainAbility = Sample_UnitHasFArmorAbility

Set VariableSet SBP_BonusAbility = Sample_UnitHasFArmorBonus

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(SBP_Caster has buff Sample_UnitHasFArmorBuff) Equal to True

Then - Actions
Set VariableSet SBP_IsValid = True

Else - Actions
Set VariableSet SBP_IsValid = False

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

Set VariableSet SBP_MainAbilityLevel = (Level of SBP_MainAbility for SBP_Caster)

Set VariableSet SBP_BonusAbilityLevel = (Level of SBP_BonusAbility for SBP_Caster)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_IsValid Equal to True

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
Unit - Add SBP_BonusAbility to SBP_Caster

Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Remove SBP_BonusAbility from SBP_Caster

Else - Actions

Unit gets the bonus when its near an Altar of Darkness

Unit near the Altar Init
Events
Map initialization

Conditions

Actions
Set VariableSet Sample_UnitNearAltarAbility = Unit near Altar (Ability)

Set VariableSet Sample_UnitNearAltarBonus = Bonuses 4

Set VariableSet Sample_UnitNearAltarType = Altar of Darkness

Set VariableSet Sample_UnitNearAltarRange[1] = 500.00

Set VariableSet Sample_UnitNearAltarRange[2] = 500.00

Set VariableSet Sample_UnitNearAltarRange[3] = 500.00

Set VariableSet Sample_UnitNearAltarRange[4] = 500.00

Unit near the Altar Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Sample_UnitNearAltarAbility

Actions
Set VariableSet SBP_Caster = (Triggering unit)

-------- Edit only the parts where it says Your_Ability, Your_Bonus_Ability and Your_Condition --------

-------- and the rest should work just fine --------

Set VariableSet SBP_MainAbility = Sample_UnitNearAltarAbility

Set VariableSet SBP_BonusAbility = Sample_UnitNearAltarBonus

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

Set VariableSet SBP_MainAbilityLevel = (Level of SBP_MainAbility for SBP_Caster)

Set VariableSet SBP_BonusAbilityLevel = (Level of SBP_BonusAbility for SBP_Caster)

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

Set VariableSet Sample_UnitNearAltarPoint = (Position of SBP_Caster)

Custom script: set bj_wantDestroyGroup = true

Set VariableSet Sample_UnitNearAltarCount = (Number of units in (Units within Sample_UnitNearAltarRange[SBP_MainAbilityLevel] of Sample_UnitNearAltarPoint matching (((Unit-type of (Matching unit)) Equal to Sample_UnitNearAltarType) and (((Matching unit) belongs to an ally of (Owner of SBP_Caster).) Equ

Custom script: call RemoveLocation(udg_Sample_UnitNearAltarPoint)

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Sample_UnitNearAltarCount Greater than 0

Then - Actions
Set VariableSet SBP_IsValid = True

Else - Actions
Set VariableSet SBP_IsValid = False

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_IsValid Equal to True

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
Unit - Add SBP_BonusAbility to SBP_Caster

Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Remove SBP_BonusAbility from SBP_Caster

Else - Actions

Unit gets the bonus if its movement speed is higher than the threshold

Unit is Fast Init
Events
Map initialization

Conditions

Actions
Set VariableSet Sample_UnitIsFastAbility = Unit is Fast (Ability)

Set VariableSet Sample_UnitIsFastBonus = Bonuses 2

Set VariableSet Sample_UnitIsFastThreshold[1] = 300.00

Set VariableSet Sample_UnitIsFastThreshold[2] = 300.00

Set VariableSet Sample_UnitIsFastThreshold[3] = 300.00

Set VariableSet Sample_UnitIsFastThreshold[4] = 300.00

Unit is Fast Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Sample_UnitIsFastAbility

Actions
Set VariableSet SBP_Caster = (Triggering unit)

-------- Edit only the parts where it says Your_Ability, Your_Bonus_Ability and Your_Condition --------

-------- and the rest should work just fine --------

Set VariableSet SBP_MainAbility = Sample_UnitIsFastAbility

Set VariableSet SBP_BonusAbility = Sample_UnitIsFastBonus

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

Set VariableSet SBP_MainAbilityLevel = (Level of SBP_MainAbility for SBP_Caster)

Set VariableSet SBP_BonusAbilityLevel = (Level of SBP_BonusAbility for SBP_Caster)

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Current movement speed of SBP_Caster) Greater than Sample_UnitIsFastThreshold[SBP_MainAbilityLevel]

Then - Actions
Set VariableSet SBP_IsValid = True

Else - Actions
Set VariableSet SBP_IsValid = False

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_IsValid Equal to True

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
Unit - Add SBP_BonusAbility to SBP_Caster

Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Remove SBP_BonusAbility from SBP_Caster

Else - Actions

Unit gets the bonus only when its level is even.

Unit Level is Even Init
Events
Map initialization

Conditions

Actions
Set VariableSet Sample_UnitLevelEvenAbility = Unit Level is Even (Ability)

Set VariableSet Sample_UnitLevelEvenBonus = Bonuses 1

Unit Level is Even Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Sample_UnitLevelEvenAbility

Actions
Set VariableSet SBP_Caster = (Triggering unit)

-------- Edit only the parts where it says Your_Ability, Your_Bonus_Ability and Your_Condition --------

-------- and the rest should work just fine --------

Set VariableSet SBP_MainAbility = Sample_UnitLevelEvenAbility

Set VariableSet SBP_BonusAbility = Sample_UnitLevelEvenBonus

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

Set VariableSet SBP_MainAbilityLevel = (Level of SBP_MainAbility for SBP_Caster)

Set VariableSet SBP_BonusAbilityLevel = (Level of SBP_BonusAbility for SBP_Caster)

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Hero level of SBP_Caster) mod 2) Equal to 0

Then - Actions
Set VariableSet SBP_IsValid = True

Else - Actions
Set VariableSet SBP_IsValid = False

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_IsValid Equal to True

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
Unit - Add SBP_BonusAbility to SBP_Caster

Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Remove SBP_BonusAbility from SBP_Caster

Else - Actions

Unit gets bonuses when its daytime.

Daytime Init
Events
Map initialization

Conditions

Actions
Set VariableSet Sample_DaytimeAbility = Its Daytime (Ability)

Set VariableSet Sample_DaytimeBonus = Bonuses 4

Set VariableSet Sample_DaytimeStart = 6.00

Set VariableSet Sample_DaytimeEnd = 18.00

Daytime Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Sample_DaytimeAbility

Actions
Set VariableSet SBP_Caster = (Triggering unit)

-------- Edit only the parts where it says Your_Ability, Your_Bonus_Ability and Your_Condition --------

-------- and the rest should work just fine --------

Set VariableSet SBP_MainAbility = Sample_DaytimeAbility

Set VariableSet SBP_BonusAbility = Sample_DaytimeBonus

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

Set VariableSet SBP_MainAbilityLevel = (Level of SBP_MainAbility for SBP_Caster)

Set VariableSet SBP_BonusAbilityLevel = (Level of SBP_BonusAbility for SBP_Caster)

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(In-game time of day) Greater than or equal to Sample_DaytimeStart

(In-game time of day) Less than Sample_DaytimeEnd

Then - Actions
Set VariableSet SBP_IsValid = True

Else - Actions
Set VariableSet SBP_IsValid = False

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_IsValid Equal to True

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
Unit - Add SBP_BonusAbility to SBP_Caster

Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Remove SBP_BonusAbility from SBP_Caster

Else - Actions

Multiple State-Based Passives

So you want multiple passives in one unit but noticed that placing two custom Exhume Corpses in one unit breaks our template. Lucky for you I have a solution. It involves having a disabled spell book which will hold our custom Exhume Corpses and hijacking the trigger using our template. Its hard to convey exactly through words so let me show you what i mean through triggers.

### Disabling Our New Spell book

### Initialization

### Plugging the Values to our Template

Explanation for the Trigger Below:

 * State Pinger (Spell book) - contains our custom Exhume Corpses and our custom Cargo Load. (The State Pinger (Spell book), Exhume Corpses, and Cargo Load only has 1 level).

 * We are going to disable our new State Pinger (Spell book) so that our unit will trigger the Starts the effect of an ability trigger without showing the icon on our unit (which we will use to plug our template into later on)

 * Unholy Aura and Endurance Aura are both spell books which will be used to represent the bonuses

State Based Passive Disabler
Events
Map initialization

Conditions

Actions
-------- Spell books that will be used for bonuses --------

Set VariableSet SBP_BonusesCount = 3

Set VariableSet SBP_Bonuses[1] = State Pinger (Spell book)

Set VariableSet SBP_Bonuses[2] = Unholy Aura (Spell book)

Set VariableSet SBP_Bonuses[3] = Endurance Aura (Spell book)

-------- --------

For each (Integer SBP_PlayerLooper) from 1 to 24, do (Actions)
Loop - Actions
For each (Integer SBP_BonusesLooper) from 1 to SBP_BonusesCount, do (Actions)
Loop - Actions
Player - Disable SBP_Bonuses[SBP_BonusesLooper] for (Player(SBP_PlayerLooper))

Explanation for the trigger below:

 * Sample_MultistateAbility pertains to State Pinger, our custom Exhume Corpses ability which is inside our new spell book called State Pinger (Spell book) described at "Disabling Our New Spell book" tab

 * For simplicity, "First" will refer to our first passive which will give the unit Unholy Aura when on blight

 * "Second" will refer to our second passive which will give the unit Endurance Aura when there are two or more allies within 500 range of our unit.

Two Passives Init
Events
Map initialization

Conditions

Actions
Set VariableSet Sample_MultiStateAbility = State Pinger (Exhume Corpses)

-------- --------

Set VariableSet Sample_FirstAbility = On Blight

Set VariableSet Sample_FirstBonus = Unholy Aura (Spell book)

-------- --------

Set VariableSet Sample_SecondAbility = Has Allies

Set VariableSet Sample_SecondBonus = Endurance Aura (Spell book)

Set VariableSet Sample_AlliesDetectRange[1] = 500.00

Set VariableSet Sample_AlliesDetectRange[2] = 500.00

Set VariableSet Sample_AlliesDetectRange[3] = 500.00

Set VariableSet Sample_AlliesCountRequired[1] = 2

Set VariableSet Sample_AlliesCountRequired[2] = 2

Set VariableSet Sample_AlliesCountRequired[3] = 2

Explanation for the trigger below:

 * Notice that the trigger condition is checking for Sample_MultiStateAbility which is our custom Exhume Corpses. (which is inside the disabled spell book called State Pinger (Spell book))

 * Also notice the values that we plugged in to the main and the bonus ability.

On Blight Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Sample_MultiStateAbility

Actions
Set VariableSet SBP_Caster = (Triggering unit)

-------- Edit only the parts where it says Your_Ability, Your_Bonus_Ability and Your_Condition --------

-------- and the rest should work just fine --------

Set VariableSet SBP_MainAbility = Sample_FirstAbility

Set VariableSet SBP_BonusAbility = Sample_FirstBonus

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

Set VariableSet SBP_MainAbilityLevel = (Level of SBP_MainAbility for SBP_Caster)

Set VariableSet SBP_BonusAbilityLevel = (Level of SBP_BonusAbility for SBP_Caster)

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

Set VariableSet Sample_Point = (Position of SBP_Caster)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Sample_FirstAbility for SBP_Caster) Greater than 0

(Sample_Point is blighted) Equal to True

Then - Actions
Set VariableSet SBP_IsValid = True

Else - Actions
Set VariableSet SBP_IsValid = False

Custom script: call RemoveLocation(udg_Sample_Point)

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_IsValid Equal to True

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
Unit - Add SBP_BonusAbility to SBP_Caster

Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Remove SBP_BonusAbility from SBP_Caster

Else - Actions

Our Second Trigger

With Allies Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Sample_MultiStateAbility

Actions
Set VariableSet SBP_Caster = (Triggering unit)

-------- Edit only the parts where it says Your_Ability, Your_Bonus_Ability and Your_Condition --------

-------- and the rest should work just fine --------

Set VariableSet SBP_MainAbility = Sample_SecondAbility

Set VariableSet SBP_BonusAbility = Sample_SecondBonus

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

Set VariableSet SBP_MainAbilityLevel = (Level of SBP_MainAbility for SBP_Caster)

Set VariableSet SBP_BonusAbilityLevel = (Level of SBP_BonusAbility for SBP_Caster)

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

Set VariableSet Sample_Point = (Position of SBP_Caster)

Custom script: set bj_wantDestroyGroup = true

Set VariableSet Sample_AlliesCount = (Number of units in (Units within Sample_AlliesDetectRange[SBP_MainAbilityLevel] of Sample_Point matching ((((Matching unit) is alive) Equal to True) and (((Matching unit) belongs to an ally of (Triggering player).) Equal to True)).))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Sample_SecondAbility for SBP_Caster) Greater than 0

Sample_AlliesCount Greater than or equal to Sample_AlliesCountRequired[SBP_MainAbilityLevel]

Then - Actions
Set VariableSet SBP_IsValid = True

Else - Actions
Set VariableSet SBP_IsValid = False

Custom script: call RemoveLocation(udg_Sample_Point)

-------- -------------------------------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_IsValid Equal to True

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
Unit - Add SBP_BonusAbility to SBP_Caster

Unit - Set level of SBP_BonusAbility for SBP_Caster to SBP_MainAbilityLevel

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SBP_BonusAbilityLevel Greater than 0

Then - Actions
Unit - Remove SBP_BonusAbility from SBP_Caster

Else - Actions

Conclusion

As you can see, creating a state-based passive ability is straightforward. However, there is one key limitation: a unit can only have one state-based ability at a time using this method, since only one Exhume Corpses ability can be cast per unit. To work around this, you can either combine the conditions of multiple state-based checks into a single trigger or follow the alternative method I mentioned earlier.

If your unit requires multiple state-based passives, you may want to stick with the traditional approach. Another limitation of this method is that state-based checks stop functioning when the unit is stunned or silenced. For example, if you want your unit to gain increased armor while stunned, this approach won't work.

This tutorial isn't intended to replace the traditional method, but rather to offer an alternative solution to the same problem. Nothing beats a dedicated system to achieve a state based effect, but this method might suffice if you only need the state based checks for a few units and if you think the effect does not warrant an entire library to implement.

I have included a test map so you can try it out.

#### Attachments

Blightcaster State Based Passive Template.w3m

 59.7 KB

 · Views: 88



Blightcaster State Based Passives Template Examples.w3m

 35.7 KB

 · Views: 89



Multiple State Based Passive.w3m

 25.1 KB

 · Views: 92




 Last edited: Nov 15, 2024

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

To begin with, I'll say that I am quite rusty so feel free to correct me.

So the gist is that the ability is basically functioning as a straight up alternative to a timer event?

If I understand this correctly, what are the benefits of this approach? is it more efficient? or is it just an alternative?

edit: also depending on the state you don't need this approach at all. If its hp related you can use damage engine which you likely have in your map running anyway. If its time of day there are already events for that.

Right away I am thinking that I could make all of my passives run on a single timer but I don't know how much this impacts compared to having 100 units with exhume.

Because it seems to me that from a lazy perspective you put in more work for what at my initial thought is a side-grade unless you supposedly really need that extra performance which is probably rare for most maps.

This isn't a problem, I am not here to tell someone that my preferences are correct or better I am just asking because it might be worth including in the tutorial for others to make an informed decision.

#### Blightsower

#### Blightsower

 Level 18

JoinedJun 20, 2014Messages556

Resources: 19

#### Resources
Packs 1Spells and Systems 17Tutorials 1

Excellent question, the way it was done normally is you set up triggers which will catch the moment a unit gets into that state and then tracking it with a timer. The point of the tutorial is for you to skip the part where you need to catch those instances and track the change as they happen. Im not entirely aware about the capabilities of damage engines to track a unit's health. As far as i know, it relies on the instance where a unit gets damaged and then checking if the unit's health is within the threshold. This method skips that part and lets you check the health and perform appropriate actions. Also yes, youre right about the gist of what im trying to demonstrate.

Edit: This method encapsulates the problem and lets you handle it in 1 trigger without needing to design full systems to address the check that you want to make.


 Last edited: Sep 23, 2024

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

Blightcaster said:

The Hypothesis

The tutorial on passives with cooldown has revealed that the Undead ability Exhume Corpses actually triggers a cast event. We now have an easy way to check for unit states by hijacking the cast trigger of Exhume Corpses. Everything can be contained within that trigger since all of the caster's states are available from the event response—Triggering Unit. The key part of the spell is to set the number of corpses to 0, the unit type to nothing, and the duration for all levels to 0.03 (the same time interval you'd use for triggered spells).

Click to expand...

In the mentioned tutorial it also says one has to disable cargo hold ability, which can be achieved in a few ways. Is this not needed anymore?

The discovery of Exhume Corpses is impressive, and your tutorial takes it to another level. Although you say this makes unit state checks without so many triggers, it sure does require a lot of abilities

Besides this not working when the unit is stunned, I suspect it also won't work if the unit is cycloned. Worth testing that.

It's still looks like a good alternative to a full fledged state passive ability system, specially when the amount abilities and bonuses aren't so high. I have no clue about how performant it is vs triggering it all, it depends on optimized the system would be.

#### Blightsower

#### Blightsower

 Level 18

JoinedJun 20, 2014Messages556

Resources: 19

#### Resources
Packs 1Spells and Systems 17Tutorials 1

Wrda said:

 In the mentioned tutorial it also says one has to disable cargo hold ability, which can be achieved in a few ways. Is this not needed anymore?

The discovery of Exhume Corpses is impressive, and your tutorial takes it to another level. Although you say this makes unit state checks without so many triggers, it sure does require a lot of abilities

Besides this not working when the unit is stunned, I suspect it also won't work if the unit is cycloned. Worth testing that.

It's still looks like a good alternative to a full fledged state passive ability system, specially when the amount abilities and bonuses aren't so high. I have no clue about how performant it is vs triggering it all, it depends on optimized the system would be.


Click to expand...

For our purposes, we need it to be on all the time.

Cyclone and polymorph will most likely stop the checks too. Mana flare is also another one although im not sure if it affects abilities with no manacost.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Wrda said:

 it sure does require a lot of abilities


Click to expand...

This is why I also said I personally would this this "annoying".

HOWEVER, if you're using Wurst you can auto generate abilities to make this pretty smooth.

I don't know if typescript and similar alternatives also allow for this these days.

#### Blightsower

#### Blightsower

 Level 18

JoinedJun 20, 2014Messages556

Resources: 19

#### Resources
Packs 1Spells and Systems 17Tutorials 1

its actually not too much. its 2 (Cargo Hold, Exhume) + the spell book for the bonuses vs 1 (passive) + the spell book for bonuses. That's under the assumption that you will implement your bonuses via spell books.


 Last edited: Sep 23, 2024

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

It's a pet peeve of mine, I hate when I can't just copy the code.

Anyway that's not relevant to the tutorial, approved.

#### Blightsower

#### Blightsower

 Level 18

JoinedJun 20, 2014Messages556

Resources: 19

#### Resources
Packs 1Spells and Systems 17Tutorials 1

Thank you @Chaosy. I've added a new section for multiple state-based passives


 Last edited: Sep 24, 2024

 You must log in or register to reply here.


### Similar threads

Locked

 Hash tables split for optimization: is it worth it ?


Helga

Jul 14, 2022

Triggers & Scripts

Replies5Views1K

Jul 14, 2022
Helga

[General]
 Trouble with Hashtable adding abilities to units


SnowYell

Apr 21, 2025

World Editor Help Zone

Replies8Views1K

Apr 25, 2025
SnowYell

[Trigger]
 How to properly build a trigger that ends a duel round


Kapharna

Jul 18, 2026

Triggers & Scripts

Replies1Views91

Jul 18, 2026
polardude

Locked

[Solved]
 Looping trigger with AoE Damage


Xzere

Apr 16, 2020

World Editor Help Zone

Replies8Views2K

Apr 16, 2020
Xzere

Locked

[Trigger]
 Trigger Causing Crash


CollectorofStoriesUntold

May 28, 2023

Triggers & Scripts

Replies2Views1K

May 29, 2023
Tristronic

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