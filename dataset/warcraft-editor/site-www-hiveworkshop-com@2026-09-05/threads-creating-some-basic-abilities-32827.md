---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-some-basic-abilities.32827/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating Some Basic Abilities
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-some-basic-abilities.32827/>
#### paskovich

#### paskovich

 Level 11

JoinedJul 12, 2005Messages764

Resources: 3

#### Resources
Maps 1Tutorials 2

Creating some basic abilities/spell-types


In this tutorial i’ll show how to make some basic, well-known abilities. This is not a step-by-step tutorial, and is not made to beginners, so don’t ask how to use variables and such. I only show a way how to make them to avoid those repeatative threads in the forums.


I attach a map that contains a sample for all abilities that are included here.


Basics needed:


1. Knowledge of the Object Editor.


2. Knowledge of the spell ’Channel’.


3. Knowledge of triggers/Trigger Editor, variables, removing leaks.


Contents:


I. Deal damage based on hero’s attribute points (GUI) – easy


II. A simple blink-strike spell (GUI) – easy


III. Custom auras (GUI) – medium


IV. Only day/night passive abilities (GUI) – hard


V. A simple knockback spell (GUI) – medium


VI. Activateable passive abilities (GUI) – medium


VII. Passive Mirror Image (GUI) - medium

I. Deal damage based on hero’s attribute points


It’s pretty easy but many people do not know how to make it. You have to use the action:


Unit – Damage target


After you declare the units that deals and takes the damage, you have to set the damage value. We want this to be one of the hero’s attribute points.


In this example, I preset this value in a variable first to make it clearer to understand.

AttributeDamage
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Attribute Damage

Actions
Set TempReal = (Real((Strength of (Triggering unit) (Include bonuses))))

Unit - Cause (Triggering unit) to damage (Target unit of ability being cast), dealing TempReal damage of attack type Spells and damage type Normal

The only problem here can be that an attribute point is an integer, but the damage value must be a real number. This means, we have to convert the attribute point into a real.


Finally, you set the attack and damage types, and there you go.

II. A simple blink-strike spell


We want our hero to teleport to the target, and deal some damage to it. For teleporting, we use the action ‘Unit – Move Unit (Instantly)’.


Look at my trigger first:

BlinkStrike
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Blink Strike

Actions
Set TempLoc00 = (Position of (Target unit of ability being cast))

Unit - Cause (Triggering unit) to damage (Target unit of ability being cast), dealing 50.00 damage of attack type Spells and damage type Normal

Unit - Move (Triggering unit) instantly to TempLoc00

Unit - Order (Triggering unit) to Attack (Target unit of ability being cast)

Custom script: call RemoveLocation(udg_TempLoc00)

First, we store the target unit’s position into a variable to avoid a location leak. We move the caster to the target and deal damage.


And here, I found a bug: if I tried the combination move-and-damage, it did not deal damage, but if I changed the order to damage-and-move, it worked. Strange…


After that, we order the caster to attack the target.


Finally, we remove the location (leak).

III. Custom auras


What about making an aura that gives a special skill for nearby friendly/enemy units? In this example we’ll make an aura that gives 4%/8%/12% chance for nearby friendly units to regenerate 15/30/45 hit points on each attack. The trigger is simple but it has a problem:

RegenAura
Events
Unit - A unit Is attacked

Conditions
((Attacking unit) has buff <aura’s buff>) Equal to True

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
<chance> Greater than or equal to (Random integer number between 1 and 100)

Then - Actions
Unit - Set life of (Attacking unit) to ((Life of (Attacking unit)) + <regen bonus>)

Else - Actions

Maybe you already see the problem: we cannot detect the level of the aura! There’s no such an action to do so. This is the main issue why I included this type of ability in this tutorial. We have to make a trick.


Make separate buffs for each level!! That’s the key. This way, we can detect which buff the affected unit has, so we can get the exact level of the aura. Example:


Level 1 – Stats – Buffs Regen Aura (level 1)


Level 2 – Stats – Buffs Regen Aura (level 2)


Level 3 – Stats – Buffs Regen Aura (level 3)


The trigger must be modified this way:

RegenAura
Events
Unit - A unit Is attacked

Conditions
Or - Any (Conditions) are true
Conditions
((Attacking unit) has buff Regen Aura (level 1)) Equal to True

((Attacking unit) has buff Regen Aura (level 2)) Equal to True

((Attacking unit) has buff Regen Aura (level 3)) Equal to True

Actions
If (((Attacking unit) has buff Regen Aura (level 1)) Equal to True) then do (Set Level = 1) else do (Do nothing)

If (((Attacking unit) has buff Regen Aura (level 2)) Equal to True) then do (Set Level = 2) else do (Do nothing)

If (((Attacking unit) has buff Regen Aura (level 3)) Equal to True) then do (Set Level = 3) else do (Do nothing)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level x 4) Greater than or equal to (Random integer number between 1 and 100)

Then - Actions
Unit - Set life of (Attacking unit) to ((Life of (Attacking unit)) + ((Real(Level)) x 15.00))

Else - Actions

 After detecting which buff the unit has, we set the variable (Level) to the appropriate value, and we can use it to calculate other values like the chance and the regen value to fit level-stats. And there you go!

IV. Only day/night passive abilities (not MUI!)


To those who don’t know what they are, these are passive abilities that have effects only at day (or at night).


In this example, we will make an aura that decreases nearby enemies’ armor by 1/2/3 at night!


The trigger part is not hard. The trick must be made in the object editor:


1. We need 2 (!) hero abilities. The first one will be a dummy “learning-ability” (based on Channel, and the second will make the effect (in this case, based on Devotion Aura).


2. The ability based on Channel is the main trick here. It has an option “Visible”, that makes its icon to appear if the hero learns the ability. If it’s not ticked, the icon will not appear, and will not take up space. Configure this ability (Icon - research, Hotkey - Learn, Tooltip – Learn (Extended) fields for sure), as it’ll be the ability that appears on the hero’s “learnable abilities” list. Make it look like a normal ability! Add THIS ability to the hero’s ability list (in the object editor of course).


3. Here it comes to the second ability, the one that appears on the hero’s “in-game ability-list”. This one must reduce armor, so we base it on Devotion Aura. Now for configuration:


 If you want your ability to have N levels, make this ability to have N+1 levels, as it must have a level that does no effect (when it’s day). Let’s declare level 1 to be the one that makes no effect. In this example, our ability must look like this:


Level 1 – Data – Armor Bonus 0.00


Level 2 – Data – Armor Bonus -1.00


Level 3 – Data – Armor Bonus -2.00


Level 4 – Data – Armor Bonus -3.00


 Set the icon, tooltip texts, targets allowed fields and such. An example:


Level 1 – Text – Tooltip – Normal – Extended It’s day! – No effect


Level 2 – Text – Tooltip – Normal – Extended Reduces armor by 1.


Level 3 – Text – Tooltip – Normal – Extended Reduces armor by 2.


Level 4 – Text – Tooltip – Normal – Extended Reduces armor by 3.


4. Now that we have both abilities, we arrived to the trigger part. We need 3 triggers. One for storing the learning hero into a variable, one for detecting day, and one for detecting night. When the hero learns the ability based on Channel (that disappears), we add the normal ability (with 4 levels) to the hero. At day, we want the ability to have no effect, so if it’s day, we set its level to 1. At night, we must set it back to its normal level. Here, the first ability will store the information about the exact level of the passive ability, as it was only learned by the hero, but its level hasn’t been modified. But remember, the passive ability has N+1 levels, because level 1 makes no effect, so we have to add 1 to the level of the “dummy-learning-ability”.


Here, ‘DayNight Passive’ is the ability based on Channel, and ‘DayNight Passive (for ability)’ is the normal one that makes the effect.

DayNightLearn
Events
Unit - A unit Learns a skill

Conditions
(Learned Hero Skill) Equal to DayNight Passive

Actions
Set DayNightHero = (Triggering unit)

Unit - Add DayNight Passive (for ability) to DayNightHero

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(In-game time of day) Greater than or equal to 6.00

(In-game time of day) Less than 18.00

Then - Actions
Unit - Set level of DayNight Passive (for ability) for DayNightHero to 1

Else - Actions
Unit - Set level of DayNight Passive (for ability) for DayNightHero to ((Level of DayNight Passive for DayNightHero) + 1)

 We store the learning hero, add the passive ability, check the day state and set the passive’s level to the apropriate value.


The trigger for detecting day:

Day
Events
Game - The in-game time of day becomes Greater than or equal to 6.00

Conditions
(In-game time of day) Less than 18.00

Actions
Unit - Set level of DayNight Passive (for ability) for DayNightHero to 1

 As you see, this trigger runs when night turns to day. We must remove the ability’s effect. We do this by setting its level to 1 (we set level 1 values to make no effect).


Trigger for detecting night:

Night
Events
Game - The in-game time of day becomes Less than 6.00

Game - The in-game time of day becomes Greater than or equal to 18.00

Conditions

Actions
Unit - Set level of DayNight Passive (for ability) for DayNightHero to ((Level of DayNight Passive for DayNightHero) + 1)

The level of the ability based on Channel remains normal all day, so we use it to set the passive ability’s level back to normal if it turns to night.

V. A simple knockback spell (not MUI!)


You need a unit-targeted spell only from the object editor, so I’ll describe the trigger part only.


We need two triggers for this:

Knockback
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Knockback

Actions
Set Caster = (Triggering unit)

Set Target = (Target unit of ability being cast)

Set LoopCounter = 0

Trigger - Turn on KnockbackMove <gen>

 This trigger fires when the spell is cast, stores the two participants (caster, target) into a variable, and turns on the second trigger that moves the target away from the caster.


LoopCounter is an integer variable that counts how many times the “movement-trigger” runs. Be sure to set it to 0 in this trigger!

KnockbackMove
Events
Time - Every 0.02 seconds of game time

Conditions

Actions
Set TempLoc00 = (Position of Caster)

Set TempLoc01 = (Position of Target)

Set TempLoc02 = (TempLoc01 offset by 5.00 towards (Angle from TempLoc00 to TempLoc01) degrees)

Unit - Move Target instantly to TempLoc02

Custom script: call RemoveLocation(udg_TempLoc00)

Custom script: call RemoveLocation(udg_TempLoc01)

Custom script: call RemoveLocation(udg_TempLoc02)

Set LoopCounter = (LoopCounter + 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
LoopCounter Greater than 50

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

This may be confusing at first sight. As you see, this trigger runs every 0.02 seconds. It’ll move the target from point to point, so it will perform a smooth slide.


Line-by-line description:


1. line: We store the location of the caster.


2. line: We store the location of the target.


3. line: We calculate the point where the unit should be moved. For this, we use ‘Point With Polar Offset’. It declares a point from the following parameters: a base point, an offset value, a degree.


4. line: With that action, we move the target to the next point (TempLoc[2]) in the slide movement.


5/6/7. line: Removing location leaks.


8. line: Add 1 to the counter variable (LoopCounter)


9. line: Checks if LoopCounter reached the appropriate value (here 50) and disables the movement-trigger to stop the unit sliding.


Each time this trigger runs (50 times a second), the target is moved away from the caster with 5 units. So we must count how many times this trigger should run:


max distance of the move / move-range per one trigger-run


(In this case: 250/5 = 50)

VI. Activateable passive abilities


For this, we use an activatable ability, Immolation seems to be the best, because it leaves a buff on the caster, and drains mana while it’s active.


The buff should be based on „Immolation” and not „Immolation (Caster)”. You can even delete the second one, there’s no need for it (and i don’t see if it had any effect btw).


In my sample, i named both the ability and the buff „Boost”.


The second thing we need is the desired passive ability. For this, I use a custom item ability based on „Item Armor Bonus”.


Let’s see the trigger part:

Boost
Events
Unit - A unit Is issued an order with no target

Conditions
(Level of Boost for (Triggering unit)) Greater than 0

Or - Any (Conditions) are true
Conditions
(Issued order) Equal to (Order(immolation))

(Issued order) Equal to (Order(unimmolation))

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Issued order) Equal to (Order(immolation))

Then - Actions
Unit - Add BoostArmor (+7) to (Triggering unit)

Unit - Set level of BoostArmor (+7) for (Triggering unit) to (Level of Boost for (Triggering unit))

Wait 0.50 seconds

Wait until (((Triggering unit) has buff Boost ) Equal to False), checking every 0.50 seconds

Unit - Remove BoostArmor (+7) from (Triggering unit)

Else - Actions
Unit - Remove BoostArmor (+7) from (Triggering unit)

As you see, we need only one trigger for this. To catch the event when the hero turns on Boost, we catch the order instead of the cast. Immolation has two orders, one for activation („immolation”), and one for deactivation („unimmolation”).


So when it’s activated, we add, when deactivated, we remove the passive ability.


But, there’s one thing that we must take care of. When the unit runs out of mana while the ability is active, the trigger does not fire, so the passive ability reamains on the hero. That’s why we have to include that buff-check. Tricky isn’t it?

VII. Passive Mirror Image


This ability allows our hero to passively create illusions of itself if it’s attacked.


We need:


-a pure passive ability (i used Evasion)


-a dummy spellcaster


-an item based on „Wand of Illusion” (it’s a charged item) - optional


-a modified item ability based on „Item Illusions” - optional


-a trigger


Unfortunately, a good „illusion-creating” unit ability does not exist, so we have to use an item (by a dummy unit) to create the illusion.


The new item is needed if you want to modify the normal Wand of Illusion’s stats. In my example, i used a custom item, because i want the illusions to deal damage, and i want them to have only a 20 second duration.


So modify the item, and its ability to fit your wishes.


We don’t have to do anything else in the object editor, so let’s go on to triggering. Here it is:

PassiveMirrorImage
Events
Unit - A unit Is attacked

Conditions
(Level of Passive MI for (Triggering unit)) Greater than 0

Actions
Set TempGroup = (Units owned by (Owner of (Triggering unit)) matching ((((Matching unit) is an illusion) Equal to True) and ((Level of Passive MI for (Matching unit)) Greater than 0)))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Number of units in TempGroup) Less than 5

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is an illusion) Equal to False

Then - Actions
Set Chance = ((Level of Passive MI for (Triggering unit)) x 10)

Else - Actions
Set Chance = ((Level of Passive MI for (Triggering unit)) x 6)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Chance Greater than or equal to (Random integer number between 1 and 100)

Then - Actions
Set TempLoc00 = (Position of (Triggering unit))

Unit - Create 1 Dummy for (Owner of (Triggering unit)) at TempLoc00 facing 0.00 degrees

Unit - Add Inventory (Hero) to (Last created unit)

Hero - Create Item Passive MI and give it to (Last created unit)

Hero - Order (Last created unit) to use (Last created item) on (Triggering unit)

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Custom script: call RemoveLocation(udg_TempLoc00)

Else - Actions

Else - Actions

Custom script: call DestroyGroup(udg_TempGroup)

The things to pay attention to:


-We want to limit the number of illusions to 5, that’s why we count their number first.


-Illusions also have the hero ability, so we have to check if the attacked unit is an illusion or not, and calculate the proper chances to create a new illusion. (The hero and the illusions have different chances to create illusions!)


-If the „chance-check” was done, we create the dummy spellcaster. We have to give him an inventory - Inventory (Hero) -, otherwise we couldn’t give him the „illusion-creating” item. The actions ’Create item for unit’ and ’Order unit to use item’ are not under ’Unit’, but under ’Hero’!


-After it, we remove the leaks.

#### Attachments

SpellsTutorial.w3x

 22.2 KB

 · Views: 3,729




 Last edited: May 30, 2007

#### grille

G

#### grille

Knockback is shitty cause when u start moving while the enemy is knockbacked the enemy will change his knock route too.

#### Arab4life

#### Arab4life

 Level 10

JoinedMar 30, 2007Messages447

Resources: 12

#### Resources
Skins 9Maps 2Tutorials 1

 nearby friendly units to regenerate


Click to expand...


Your custom aura only regenerates the life of the Attacking unit. Not nearby friendly units. This makes it obsolete because no it is just a complicated version of Vampirisim aura. The only difference is that this doesnt work as an aura.

 A simple blink-strike spell


Click to expand...


In the spell, if you deal damage and then order the unit to attack, wouldnt it deal 50 damage + the normal attack of the hero at once? Or is that what you intended.


Good job on the other spells. I didnt test them but by looking at them they should work.


 Last edited: Jul 24, 2007

#### l3lack-Knight

L

#### l3lack-Knight

GJ

Hey i tried to make an ability simmilar to Pudge's " Rot " from DotA and here is how i made it before you told me the great ideea about making the buffs diffrent on each level :

Stygian Aura On
Events
Unit - A unit Is issued an order with no target

Conditions
(Issued order) Equal to (Order(immolation))

Actions
Set StygianCaster = (Ordered unit)

Set StygianActivate = True

This is how i turned it on .

Stygian Aura Off
Events
Unit - A unit Is issued an order with no target

Conditions
(Issued order) Equal to (Order(unimmolation))

Actions
Set StygianCaster = No unit

Set StygianActivate = False

Custom script: call RemoveLocation(udg_StygianPosition)

This is how i turned if off .

Stygian Aura l3lackKnight
Events
Time - Every 1.00 seconds of game time

Conditions
StygianActivate Equal to True

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of |cffffcc00S|rtygian Aura for (Triggering unit)) Equal to 1

Then - Actions
Set StygianDamage = (((Life of StygianCaster) / 100.00) x 5.00)

Unit - Set life of StygianCaster to ((Life of StygianCaster) - StygianDamage)

Set StygianPosition = (Position of StygianCaster)

Unit Group - Pick every unit in (Units within 600.00 of StygianPosition matching ((((Matching unit) is A structure) Equal to False) and ((((Matching unit) is alive) Equal to True) and ((((Matching unit) is Magic Immune) Equal to False) and (((Matching unit) belongs to an enemy of (Owner of and do (Actions)
Loop - Actions
Custom script: local texttag StygianDamageText

Custom script: local effect StygianDamageEffect

Special Effect - Create a special effect attached to the chest of (Picked unit) using Objects\Spawnmodels\Human\HumanBlood\BloodElfSpellThiefBlood.mdl

Custom script: set StygianDamageEffect = GetLastCreatedEffectBJ()

Unit - Cause StygianCaster to damage (Picked unit), dealing StygianDamage damage of attack type Spells and damage type Normal

Floating Text - Create floating text that reads (String((Integer(StygianDamage)))) above (Picked unit) with Z offset 10.00, using font size 10.00, color (100.00%, 0.00%, 0.00%), and 0.00% transparency

Custom script: set StygianDamageText = GetLastCreatedTextTag()

Custom script: call SetTextTagVelocityBJ( StygianDamageText, 64, 90 )

Custom script: call SetTextTagPermanentBJ( StygianDamageText, false )

Custom script: call SetTextTagLifespanBJ( StygianDamageText, 1.00 )

Wait 1.00 seconds

Custom script: call DestroyEffect(StygianDamageEffect)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of |cffffcc00S|rtygian Aura for (Triggering unit)) Equal to 2

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of |cffffcc00S|rtygian Aura for (Triggering unit)) Equal to 2

Then - Actions
Set StygianDamage = (((Life of StygianCaster) / 100.00) x 10.00)

Unit - Set life of StygianCaster to ((Life of StygianCaster) - StygianDamage)

Set StygianPosition = (Position of StygianCaster)

Unit Group - Pick every unit in (Units within 600.00 of StygianPosition matching ((((Matching unit) is A structure) Equal to False) and ((((Matching unit) is alive) Equal to True) and ((((Matching unit) is Magic Immune) Equal to False) and (((Matching unit) belongs to an enemy of (Owner of and do (Actions)
Loop - Actions
Custom script: local texttag StygianDamageText

Custom script: local effect StygianDamageEffect

Special Effect - Create a special effect attached to the chest of (Picked unit) using Objects\Spawnmodels\Human\HumanBlood\BloodElfSpellThiefBlood.mdl

Custom script: set StygianDamageEffect = GetLastCreatedEffectBJ()

Unit - Cause StygianCaster to damage (Picked unit), dealing StygianDamage damage of attack type Spells and damage type Normal

Floating Text - Create floating text that reads (String((Integer(StygianDamage)))) above (Picked unit) with Z offset 10.00, using font size 10.00, color (100.00%, 0.00%, 0.00%), and 0.00% transparency

Custom script: set StygianDamageText = GetLastCreatedTextTag()

Custom script: call SetTextTagVelocityBJ( StygianDamageText, 64, 90 )

Custom script: call SetTextTagPermanentBJ( StygianDamageText, false )

Custom script: call SetTextTagLifespanBJ( StygianDamageText, 1.00 )

Wait 1.00 seconds

Custom script: call DestroyEffect(StygianDamageEffect)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of |cffffcc00S|rtygian Aura for (Triggering unit)) Equal to 3

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions
Do nothing

Custom script: call RemoveLocation(udg_StygianPosition)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of |cffffcc00S|rtygian Aura for (Triggering unit)) Equal to 3

Then - Actions
Set StygianDamage = (((Life of StygianCaster) / 100.00) x 15.00)

Unit - Set life of StygianCaster to ((Life of StygianCaster) - StygianDamage)

Set StygianPosition = (Position of StygianCaster)

Unit Group - Pick every unit in (Units within 600.00 of StygianPosition matching ((((Matching unit) is A structure) Equal to False) and ((((Matching unit) is alive) Equal to True) and ((((Matching unit) is Magic Immune) Equal to False) and (((Matching unit) belongs to an enemy of (Owner of and do (Actions)
Loop - Actions
Custom script: local texttag StygianDamageText

Custom script: local effect StygianDamageEffect

Special Effect - Create a special effect attached to the chest of (Picked unit) using Objects\Spawnmodels\Human\HumanBlood\BloodElfSpellThiefBlood.mdl

Custom script: set StygianDamageEffect = GetLastCreatedEffectBJ()

Unit - Cause StygianCaster to damage (Picked unit), dealing StygianDamage damage of attack type Spells and damage type Normal

Floating Text - Create floating text that reads (String((Integer(StygianDamage)))) above (Picked unit) with Z offset 10.00, using font size 10.00, color (100.00%, 0.00%, 0.00%), and 0.00% transparency

Custom script: set StygianDamageText = GetLastCreatedTextTag()

Custom script: call SetTextTagVelocityBJ( StygianDamageText, 64, 90 )

Custom script: call SetTextTagPermanentBJ( StygianDamageText, false )

Custom script: call SetTextTagLifespanBJ( StygianDamageText, 1.00 )

Wait 1.00 seconds

Custom script: call DestroyEffect(StygianDamageEffect)

Else - Actions
Do nothing

And this is the actual spell wich has 3 levels cause its an ulti .Not very good right ? I also tried splitting it into 3 diffrent triggers wich would turn off when you learn another level of the spell ( if you learn Stygian Aura level 2 then the trigger 4 the level 1 would be turned off ) . Well tell me what you think .........

#### Just_Spectating

#### Just_Spectating

 Level 39

JoinedMay 16, 2007Messages7,272

Resources: 49

#### Resources
Models 13Icons 10Skins 26

pudges rot is only an immolation and slow aura(tornado slow aura thingie). the immolation targets self aswell.

#### Yunero

Y

#### Yunero

@Just_Spectating


yeah your right but trigger is still needed to give pudge the tornado slow aura when he activates rot

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

grille said:

 Knockback is shitty cause when u start moving while the enemy is knockbacked the enemy will change his knock route too.


Click to expand...

 its actually an angle between 2 points now, but maybe he hadn't fixed that when you posted this

#### Scotty_grun

S

#### Scotty_grun

hey im REALLY noobie at map making and im trying to make a passive ability that reduces damage and slows attackers.


can you help me??


either post here or prefered email me at [email protected]


please help, and any other map making advise would be very helpful!

#### paskovich

#### paskovich

 Level 11

JoinedJul 12, 2005Messages764

Resources: 3

#### Resources
Maps 1Tutorials 2

Arab4life said:

 Your custom aura only regenerates the life of the Attacking unit. Not nearby friendly units. This makes it obsolete because no it is just a complicated version of Vampirisim aura. The only difference is that this doesnt work as an aura.


Click to expand...

Read the whole description carefully...

Arab4life said:

 In the spell, if you deal damage and then order the unit to attack, wouldnt it deal 50 damage + the normal attack of the hero at once? Or is that what you intended.


Click to expand...

The damage is dealt when the attack starts, so they are not summed.

Please guys, if you have problems with your spells, go to the Spells & Systems forum!

#### Yunero

Y

#### Yunero

okey I have a question how do I make an amplify effect with the bonus damage being shown and a damage bonus per physical damage the hero deals

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

Yunero said:

 okey I have a question how do I make an amplify effect with the bonus damage being shown and a damage bonus per physical damage the hero deals


Click to expand...

 well, you can start by asking in the Spells and Systems section

#### vicktor.hebat

#### vicktor.hebat

 Level 3

JoinedSep 20, 2007Messages30

Can i ask something? How can i make the blink-strike abilty blinked exactly behind the enemy?

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

Look at above post

#### ticonwc3

T

#### ticonwc3

Go ahead and correct me im wrong about this.... but would'nt you need to make 3 triggers for the knockback spell, 1 to activate the other 2 the 2 you poted could be changed to where the variables are set by the 3rd trigger so if the unit moves the hit unit wont move with it, The third trigger's actions should look like this if im not wrong


 Set (triggering unit) = casting unt


 Set (targeted unt) = (target unit of the ability being cast)


 Trigger - Turn this trigger off

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

if you turn a trigger off, its not MUI, remember that


oh, and that DayNight spell is easily fixed into MUI


turn the Unit Variable into an Array


Set UNITVARIABLE[Player number of (Owner of (Triggering unit))] = (Triggering unit)


then just add a loop to the two other triggers and i dont think there need to be more explaining

#### Lucianus

#### Lucianus

 Level 7

JoinedJul 29, 2007Messages172

Check it in Unit - Makes unit damage unit.


 Last edited: Jan 27, 2008

#### Aschy

#### Aschy

 Level 8

JoinedJan 23, 2008Messages334

Resources: 2

#### Resources
Maps 2

TurboJyri said:

 Could you explain how to make that attribute damage trigger to do for example 2x agility in damage + 100?


Click to expand...


Make a real varible that will store the damage for you


event - unit casts spell


condition - spell cast equal to X


event - set real varible = ((real((agility of (casting unit)(include bonuses)))) x 2.00)


event - set real varible = ((real varible ) +100)


event - cause (casting unit) to damage (target unit of ability being cast), equal to real varible with damage of attack type (user choice) with attack type (user choice)


if you need more help I can attach a file with an example

#### darkalex13

#### darkalex13

 Level 3

JoinedFeb 20, 2008Messages54

I've managed creating a night passive ability but there is a problem, the dummy ability apears to and I don't know where to find the Visible option to untick it so it will not appear. Can anyone help?

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

what ability did you use to make the dummy ability?

#### darkalex13

#### darkalex13

 Level 3

JoinedFeb 20, 2008Messages54

Life Drain, that was the first ability that i thought of.

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

use Channel instead

when changing the stats, there is a Option alternative

there you can decide if you want it to be Visible or not

#### darkalex13

#### darkalex13

 Level 3

JoinedFeb 20, 2008Messages54

Yeah, sorry, I was pretty sleepy last night when I read the tutorial, I managed making it, thanks for all your help and please excuse me.

#### shadoye

#### shadoye

 Level 4

JoinedFeb 9, 2008Messages33

Resources: 1

#### Resources
Icons 1

Anyone know how to make a permanent invisibility spell? ive tried heaps of time and it all works but wen i learn the skill with my hero theres no DISBTN icon in my heros selected abilities

#### darkalex13

#### darkalex13

 Level 3

JoinedFeb 20, 2008Messages54

I have a question: How can I make a trigger based spell to damage an area without damaging the casting hero and its allies?

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

make a unit group that consists of all enemies within an area

pick every unit in group

damage target

#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

I dont think this tutorial was intended to be bombarded with spell questions. Thats why the hive has a help zone and gui/jass forum.

#### Simon559

#### Simon559

 Level 4

JoinedApr 11, 2007Messages97

Set TempReal = (Real((Strength of (Triggering unit) (Include bonuses))))


what Varible Type should it be?

#### Bombader

#### Bombader

 Level 3

JoinedOct 15, 2007Messages43

I don't quite understand some lines in the "Knockback spell" area, I think some language may have been updated since this was up.


Set TempLoc01 = (Position of Target)


I know it's nitpicking but I think it's now "Position of Targeted unit"


Set TempLoc02 = (TempLoc01 offset by 5.00 towards (Angle from TempLoc00 to TempLoc01) degrees)


I just cannot find this, Well I have found something similar, but lacks the "towards" and "degrees."

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

I believe Target is a Variable

on the other note, try Point with Polar Offset

#### Bombader

#### Bombader

 Level 3

JoinedOct 15, 2007Messages43

Ya it is a variable just under a slightly different name


Ok I'll try that, I assume the tempLoc variable's are points correct?

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

yes, they are Point Variables

#### Bombader

#### Bombader

 Level 3

JoinedOct 15, 2007Messages43

Ok with the assumption that I did this correctly, the unit vanishes from thin air rather than being knocked back. I'm trying to use it with a modified storm bolt melee range.

I assume the effect is keen to something like a few feet away.

Edit: Solved, "Position of Target/caster" I mistook as something else, didn't think it meant the variable "Caster/Target" lol.

#### Fellup

#### Fellup

 Level 2

JoinedMay 29, 2007Messages11

I want the Blink Strike ability in 3 levels, but I don't know how to, help anyone?

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

well, that depends. Im gonna go ahead and guess that its the Damage of the Blink Strike that is changing by each level, so

how much damage does each level do?

#### Fellup

#### Fellup

 Level 2

JoinedMay 29, 2007Messages11

I was think a 1x agility + 50 to start with, and with the next level it could be 2x agility + 100. And the final level (3) would be 3x agility + 200.


Any ideas? Would be greatful, thanks in advance...

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

you are going to need a Arithmetic calculation for that

something like

Blablabla

Actions
Set Real = (Real((((Agility of (Triggering unit) (Include bonuses)) + 50) x (Level of Blinkstrike for (Triggering unit)))))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Blinkstrike for (Triggering unit)) Equal to 3

Then Actions
Set Real = (Real + 50)

Else Actions

now, this goes before the Unit - Damage Target action

Real is a Real Variable

#### Fellup

#### Fellup

 Level 2

JoinedMay 29, 2007Messages11

Ah okay, thank you very much =) Will try this and see if it works proper...

Edit: It worked just fine but, I was thinking of a chain lighting. How do I make all the units take damage equal to the attributes. Been thinking of it for a long time but can't really find teh right code.


 Last edited: Jun 25, 2008

#### KillmegodsComments

#### KillmegodsComments

 Level 2

JoinedMay 17, 2008Messages13

one question. Does anyone know how to make a AOE knockback... like i order my unit to warstomp and the ones that are damaged are also knockbacked... anyone knows?

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

try something like

Unit casts spell

Pick all units within (the AoE of the Warstomp)

Make them get pushed back

Done

#### Dalaran_Guard

#### Dalaran_Guard

 Level 8

JoinedSep 13, 2006Messages431

Resources: 1

#### Resources
Maps 1

KillmegodsComments said:

 one question. Does anyone know how to make a AOE knockback... like i order my unit to warstomp and the ones that are damaged are also knockbacked... anyone knows?


Click to expand...


Ya pask, you may want to add something like this. If you want, I have an old but effective basic one of these. I'll post it, and you are welcome to use/modify it if you so wish...


The Sanctity trigger is triggered by a warstomp-type ability. It basically picks all enemies within a defined area and adds them to a global unit group called 'Pushed'. Custom values are used to store the angle between the caster and the pushed units.


The second, periodic trigger, picks all units in 'Pushed' and pushes them back


until the unit either no longer has the buff 'Sanctity' or is dead.


The spell is (technically) entirely MUI, so it is (in my opinion at least) quite useful as a base AoE knockback.

Sanctity
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Sanctity

Actions
Set Point = (Position of (Casting unit))

Set Unit = (Triggering unit)

Set Real = 450.00

Set FlyingExcluded = True

Trigger - Run Pick Enemies <gen> (ignoring conditions)

Unit Group - Pick every unit in UnitGroup and do (Actions)
Loop - Actions
Set Point2 = (Position of (Picked unit))

Set Facing = (Angle from Point to Point2)

Set Unit = (Picked unit)

Unit Group - Add (Picked unit) to Pushed

Unit - Set the custom value of (Picked unit) to (Integer(Facing))

Unit Group - Remove all units from UnitGroup

Custom script: call RemoveLocation( udg_Point )

Custom script: call RemoveLocation( udg_Point2 )

Projectiles and Movement
Events
Time - Every 0.05 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in Pushed and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
((Picked unit) has buff Sanctity ) Equal to False

((Picked unit) is dead) Equal to True

Then - Actions
Unit Group - Remove (Picked unit) from Pushed

Unit - Set the custom value of (Picked unit) to 0

Else - Actions
Set Point = (Position of (Picked unit))

Set Facing = (Real((Custom value of (Picked unit))))

Set Point2 = (Point offset by 35.00 towards Facing degrees)

Unit - Move (Picked unit) instantly to Point2, facing (Facing of (Picked unit)) degrees

Unit Group - Remove all units from UnitGroup

Custom script: call RemoveLocation( udg_Point )

Custom script: call RemoveLocation( udg_Point2 )

#### Nichita_00

#### Nichita_00

 Level 7

JoinedJul 11, 2007Messages509

Resources: 17

#### Resources
Icons 8Skins 9

Hmm i wonder can i adapt the day/night passive thingy to transform units? (Eaxample:Human at day and werewolf at night???)

#### Arree

#### Arree

 Level 2

JoinedJul 14, 2008Messages13

The Trigger "Blink Strike" buggs for me at the last line


"call RemoveLocation(udg_TempLoc00)"


it says "Expected a name", eh? help.

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

is your point variable called TempLoc00?

#### Raziel_br

#### Raziel_br

 Level 12

JoinedOct 13, 2008Messages560

Resources: 17

#### Resources
Models 2Skins 1Maps 14

very confused tutorial...


I don't undestand how make one ability based in atribute =\

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

really? the first trigger shows how to do it

#### Palaslayer

#### Palaslayer

 Level 24

JoinedJun 16, 2008Messages1,936

Resources: 74

#### Resources
Icons 68Packs 1Spells and Systems 2Tutorials 3

custom aura

i have q question @ ur custom aura trigger


can't we use real(level of abiltiy for a unit) [set the unit variable in another trigger with a unit learns an ability - event]


i think this is lees work than doing seperated buffs


if not i just want to know if thats possible

#### Raziel_br

#### Raziel_br

 Level 12

JoinedOct 13, 2008Messages560

Resources: 17

#### Resources
Models 2Skins 1Maps 14

Ok,I was make, thanks, but how can I make one area demage based in atribute not deling demage in my units?

#### Razorbrain

#### Razorbrain

 Level 12

JoinedAug 18, 2006Messages1,193

Resources: 1

#### Resources
Maps 1

Pick every unit in (some unit group)

- Deal damage to picked unit

#### Duhcake

#### Duhcake

 Level 7

JoinedOct 13, 2008Messages299

Hmmh... How to make Knockback-skill to knockback every unit the missile touches? Im using skill based on shockwave, and i want to make it so that it would knock the units back it touches w/o target..

#### Raziel_br

#### Raziel_br

 Level 12

JoinedOct 13, 2008Messages560

Resources: 17

#### Resources
Models 2Skins 1Maps 14

can I make one spell demage deal the TempReal???


I want make the war stomp make this


and deal demage in picked unit, don't works, the attack demages my unit too


=\

 You must log in or register to reply here.


### Similar threads

 State-based Passive Ability Tutorial


Blightsower

Sep 22, 2024

Trigger (GUI) Editor Tutorials

Replies8Views3K

Sep 23, 2024
Blightsower

 How to create an artificial level-up system


frettory

Nov 22, 2012

Trigger (GUI) Editor Tutorials

Replies14Views9K

Mar 13, 2013
frettory

 Hero Selection Systems


Tonks

Mar 23, 2007

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jun 8, 2023
dantles1992

 General Spell Creation


Chaosy

Jun 11, 2013

Trigger (GUI) Editor Tutorials

Replies35Views10K

Feb 8, 2014
PurgeandFire

 Simple Triggered Hero Artificial Intelligence in GUI


DenZel94

May 2, 2007

Trigger (GUI) Editor Tutorials

Replies22Views26K

Jul 1, 2021
aloH

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