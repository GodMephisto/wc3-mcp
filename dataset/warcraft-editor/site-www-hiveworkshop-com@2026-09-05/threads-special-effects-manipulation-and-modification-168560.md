---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/special-effects-manipulation-and-modification.168560/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Special Effects Manipulation and Modification
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/special-effects-manipulation-and-modification.168560/>
#### N1ghthawk

#### N1ghthawk

 Level 10

JoinedOct 31, 2009Messages352

Resources: 3

#### Resources
Maps 1Tutorials 2

Special Effects - Manipulation and Modification (WIP)


In this tutorial I will go over some basic ways you can manipulate and modify special effects using only the WC3 editor. This tutorial was meant to teach beginners or people who are new to the World Editor tips and tricks regarding effects and was not meant for advanced users (though even the most advanced designers may learn something new).

Requirements


1. A basic understanding of effects


2. A basic knowledge of how to trigger


3. A basic knowledge of how to use the World Editor


4. A basic understanding of how to edit units and abilities


5. A basic knowledge of lightning effects and how they work

Table of Contents


1. Leaks


2. Dummy Units


3. Dummy Casts


4. Scale


5. Angle


6. Custom AoE Effects


7. Lightning Effects

 Leaks



Special effects created in the WC3 editor need to be destroyed otherwise they can cause immense lag end-game. In order to remove special effects you must use the destroy command in the trigger editor. Additionally, if you create a special effect at a point, you must apply the position to a variable and then clear it after you are done so it does not permanently take up memory space.


The following is a function that creates a special effect at a point and clears both the effect and the point variable:

Removing Point and Effect Leaks
Events
Unit - A unit Starts the effect of an ability

Conditions

Actions
Set TempPos1 = (Position of (Triggering unit))

Special Effect - Create a special effect at TempPos1 using Abilities\Spells\Human\ThunderClap\ThunderClapCaster.mdl

Special Effect - Destroy (Last created special effect)

Custom script: call RemoveLocation(udg_TempPos1)

When you destroy an effect it doesn't immediately dissapear. It iether plays the death animation of the effect or approx. 5 seconds of its birth/stand animation. This becomes a problem if an effect model has a death animation but it displays nothing; as destroying the effect will give the appearance that the effect has dissapeared. The easiest way to deal with this is to set up a timer and destroy the effect after a pre-determined amount of time; like so:

Timed Effect Removal
Events
Unit - A unit Starts the effect of an ability

Conditions

Actions
Set TempPos1 = (Target point of ability being cast)

Special Effect - Create a special effect at TempPos1 using Abilities\Spells\Human\ThunderClap\ThunderClapCaster.mdl

Set TempEffect = (Last created special effect)

Countdown Timer - Start EffectRemoval as a One-shot timer that will expire in 5.00 seconds

Custom script: call RemoveLocation(udg_TempPos1)

Timed Effect Removal 2
Events
Time - EffectRemoval expires

Conditions

Actions
Special Effect - Destroy TempEffect

Dummy units are an important aspect of effect manipulation that I will go over later in this tutorial. In order to clear dummy units you need to either remove the unit (which you will rarely do because most dummy units have locust which makes them difficult to refer to over extended periods of time) or give the unit an expiration timer as follows:

Clearing Dummy Units
Events
Unit - A unit Starts the effect of an ability

Conditions

Actions
Set TempPos1 = (Target point of ability being cast)

Unit - Create 1 Dummy Unit for Neutral Passive at TempPos1 facing Default building facing degrees

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Custom script: call RemoveLocation(udg_TempPos1)

When a units expiration timer expires it will immediately remove the unit from the map, thus ensuring that dummy units don't stack up all over your map over the duration of the game.

 Dummy Units



Much of what I will go over in this tutorial requires a knowledge of what dummy units are, how to create them, and how to use them. In order to facilitate your understanding I will go over step-by-step how to create a dummy unit.

What are dummy units?


Dummy units are a way of "cheating" in the world editor and going beyond the limited control that Blizzard gave us over the game. They are generally used in the following ways:


1. Casting a spell from an non-visible source


2. Creating an effect that can be manipulated in more ways than is allowed via effect-related triggers

How do I create a basic dummy unit?

Step 1


Create a custom unit in the unit tab of the object editor

Step 2


Remove any abilities

Step 3


Add the locust ability to the unit

Step 4


Disable the shadow of the unit

Step 5


Disable the soundset of the unit

Step 6


Disable the movement speed of the unit

Step 7


Modify the name of the unit

Step 8


Change the model of the unit (If your going to use the dummy unit to cast a spell just change it to [Abilities -> Tree of Life Upgrade])

Step 9


Disable the attack of the unit

 Dummy Casts



So now you know how to create a dummy unit, but how do you use it? Lets say you wanted to make it so that a point-targeted ability now shoots out a poisoned arrow (yes you could just change the missile art of the ability but for the sake of this tutorial lets use a dummy unit cast instead). Choose any hero ability that targets a position on the ground, remove its missile art, and then do the following to create the dummy unit effect:

Step 1


Create a custom ability (base this particular one off carrion swarm)

Step 2


Set the level of your new custom ability to 1

Step 3


Disable the mana cost of the ability

Step 4


Disable any damage done by the ability

Step 5


Modify the name of the ability (for now call it "dummy cast")

Step 6


Change the missile art of the ability (for now change it to the gnoll assassin/poisoned arrow missile found under missiles)

Step 7


Make the ability a unit ability (uncheck the "Stats - Hero Ability" box)

Step 8


Give the ability to a dummy unit (in this case give it to the dummy unit you created previously in this tutorial)

Step 9


Now you must create the dummy unit and order it to cast the carrion swarm ability at whatever position your hero cast his/her spell at. Use your knowledge of leaks gained at the start of this tutorial to clear up any leaks you may generate in this function.

Basic Dummy Cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to INSERT ABILITY HERE

Actions
Set TempPos1 = (Position of (Triggering unit))

Set TempPos2 = (Target point of ability being cast)

Unit - Create 1 Dummy Unit for Neutral Passive at TempPos1 facing (Angle from TempPos1 to TempPos2) degrees

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Unit - Order (Last created unit) to Undead Dreadlord - Carrion Swarm TempPos2

Custom script: call RemoveLocation(udg_TempPos1)

Custom script: call RemoveLocation(udg_TempPos2)

This should create the following effect when you use whatever hero ability you started with:

Alternative Course (optional)


Let's say you wanted to make it so that the human Paladin's "divine shield" ability now shoots out poisoned arrows in all directions. You can still use the same dummy unit and dummy ability as before seeing as the actual base effect hasn't change. You must, however, change the triggering of the effect as you are both creating more than one effect in multiple directions and also using an ability that does not target a position. In this case, you can use the "For Each Integer A" function to create the effect at multiple angles as follows:

Dummy Cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Divine Shield

Actions
For each (Integer A) from 1 to 20, do (Actions)
Loop - Actions
Set TempPos1 = (Position of (Triggering unit))

Set TempPos2 = (TempPos1 offset by 500.00 towards (18.00 x (Real((Integer A)))) degrees)

Unit - Create 1 Dummy Unit for Neutral Passive at TempPos1 facing (Angle from TempPos1 to TempPos2) degrees

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Unit - Order (Last created unit) to Undead Dreadlord - Carrion Swarm TempPos2

Custom script: call RemoveLocation(udg_TempPos1)

Custom script: call RemoveLocation(udg_TempPos2)

This should create an effect whenever you cast "divine shield" that looks like the following:

 Scale



Aside from using dummy units to cast spells you can also use them to display effects themselves. This is particularily useful if you want to increase or decrease the scale of an effect. Take a look at this flamestrike spell:


Any noob can see that flame eruption isn't NEARLY big enough. What you want to do here is change the model file of a dummy unit (you can still use the one you made earlier in this tutorial) to the flamestrike effect and then increase its scale.

Step 1


Change the model of the dummy to the flamestrike effect (it is under "abilities)

Step 2


Increase the scale of the dummy unit (for now set its scale to 3x normal size)

Step 3


Whenever a unit casts the flamestrike ability, create the dummy unit at the target point

Timed Effect Removal
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Flame Strike

Actions
Set TempPos1 = (Target point of ability being cast)

Unit - Create 1 Dummy Unit for Neutral Passive at TempPos1 facing (Random angle) degrees

Unit - Add a 5.00 second Generic expiration timer to (Last created unit)

Animation - Play (Last created unit)'s birth animation

Custom script: call RemoveLocation(udg_TempPos1)

The result should be a satisfyingly huge flame eruption whenever a unit casts flamestrike:

 Angle



You can manipulate the roll and pitch angles of effects by applying them to a dummy unit (as seen above in the "scale" section) and then editing that unit's angle properties).


Lets say you wanted to take the "Tome of Retraining" effect and make it into a beam that can be shot at your enemies instead of into the air. You can do this quite simply by applying the effect to a dummy unit and then modifying the pitch angle of the unit.

Step 1


Create a dummy unit

Step 2


Change the model file of the dummy unit to "Abilities -> Tome of Retraining <caster>"

Step 3


Modify the pitch angle of the unit (by how much differs from model to model, but for this particular effect set it to -90)


You can modify the pitch angle by changing the data in the "Art - Maximum Pitch Angle (degrees)" field that is 19 lines below the top of the unit editor. In order to enter a negative value into the field, you can either shift+click it or go to "File -> Preferences -> General -> Allow Negative Values in the Object Editor".

Step 4


Now you have correctly modified the angle of the effect, but its halfway in the ground. Just change the flying height of the unit to approx. 200 and it will look fine.

Step 5


Now the problem is that the effect is all smashed together. Increase the scale to about 2.5x its normal size and it will look much better.


And now you have an angled effect! To implement it into a spell, just create the dummy unit at the correct position and angle as you have learned to do above. The following is a look at what you can do if you edit the angles of multiple effects:

 Custom AoE Effect



One cool thing you can do with triggers is create custom AoE effects by spamming smaller effects in an area. Lets say you wanted to give the Dark Ranger's "silence" ability a custom AoE effect by spawning tons of small eruptions in the area it is cast.


By simply taking the target point of the ability and creating multiple effects around it, you can make what appears in-game to be an AoE blast:

Silence AoE Effect
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Silence

Actions
Set TempPos1 = (Target point of ability being cast)

Set EffectCount = 0

Trigger - Turn on Silence AoE Effect Tick <gen>

Silence AoE Effect Tick
Events
Time - Every 0.05 seconds of game time

Conditions

Actions
Set EffectCount = (EffectCount + 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
EffectCount Equal to 15

Then - Actions
Trigger - Turn off (This trigger)

Custom script: call RemoveLocation(udg_TempPos1)

Else - Actions
Set TempPos2 = (TempPos1 offset by (Random real number between 0.00 and 600.00) towards (Random angle) degrees)

Special Effect - Create a special effect at TempPos2 using Units\NightElf\Wisp\WispExplode.mdl

Special Effect - Destroy (Last created special effect)

Custom script: call RemoveLocation(udg_TempPos2)

This should create an effect that looks similar to this whenever you cast the Dark Ranger's "silence" ability:

 Lightning Effects



Let's say you wanted to make it so the Mountain King's "storm bolt" ability shoots out a bolt of lightning instead of a hammer.

Step 1


Remove the current missile art of the ability

Step 2


In order to create the lightning effect via triggers you will have to do the following things:


1. Create the lightning


2. Move the lightning as the target unit moves


3. Destroy the lightning after a set amount of time


You can do this by using the "Create Lightning", "Destroy Lightning", and "Move Lightning" commands in the GUI-trigger editor as follows:

Storm Bolt Cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Storm Bolt

Actions
Set StormBolt_Target = (Target unit of ability being cast)

Set TempPos1 = (Position of (Triggering unit))

Set TempPos2 = (Position of StormBolt_Target)

Lightning - Create a Chain Lightning - Primary lightning effect from source TempPos1 to target TempPos2

Set CustomLightning = (Last created lightning effect)

Custom script: call RemoveLocation(udg_TempPos2)

Set CustomLightning_Count = 0

Trigger - Turn on Storm Bolt Move <gen>

Storm Bolt Move
Events
Time - Every 0.03 seconds of game time

Conditions

Actions
Set CustomLightning_Count = (CustomLightning_Count + 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
CustomLightning_Count Equal to 35

Then - Actions
Custom script: call RemoveLocation(udg_TempPos1)

Lightning - Destroy CustomLightning

Trigger - Turn off (This trigger)

Else - Actions
Set TempPos2 = (Position of StormBolt_Target)

Lightning - Move CustomLightning to source TempPos1 and target TempPos2

Custom script: call RemoveLocation(udg_TempPos2)

Now the Mountain King's "storm bolt" spell should shoot out a burst of lightning instead of a hammer:

Alternative Course (optional)


You can also edit the color of the lightning via triggers. In order to make the lightning red instead of blue, you would modify the first trigger as follows:

Storm Bolt Cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Storm Bolt

Actions
Set StormBolt_Target = (Target unit of ability being cast)

Set TempPos1 = (Position of (Triggering unit))

Set TempPos2 = (Position of StormBolt_Target)

Lightning - Create a Chain Lightning - Primary lightning effect from source TempPos1 to target TempPos2

Set CustomLightning = (Last created lightning effect)

Lightning - Change color of (Last created lightning effect) to (1.00 0.00 0.00) with 1.00 alpha

Custom script: call RemoveLocation(udg_TempPos2)

Set CustomLightning_Count = 0

Trigger - Turn on Storm Bolt Move <gen>

Which would create the following effect:


 Last edited: May 31, 2010

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Images aren't loading for me. Are they working for you?

#### N1ghthawk

#### N1ghthawk

 Level 10

JoinedOct 31, 2009Messages352

Resources: 3

#### Resources
Maps 1Tutorials 2

They are all working fine for me :0


The only difference between this and any other image postings i've done is that these are JPEG, but I doubt that has anything to do with it :\


EDIT - I think I may know why, try it now


 Last edited: May 31, 2010

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Thank you, they work just fine now. =)

I have to say, this tutorial is very nice. Some of the examples aren't really MUI, but that is okay, since it is generally to show how to perform the effect. [Although, you might want to state that they aren't MUI]

Might also want to mention that you can add elevation to dummy effects:

Unit - Add Storm Crow Form to Dummy

Unit - Remove Storm Crow Form from Dummy

Animation - Change Dummy's flying height to 500.00 at 0.00
In case they want to modify it during the spell.

Overall, nice job though. That is only optional to add. ~Approved.

EDIT: I've moved this to mapping. Usually Misc is reserved for external things related to wc3 or this forum.

#### X3n0nX

#### X3n0nX

 Level 6

JoinedAug 1, 2009Messages159

I cant see the images why?

#### arsenio118

#### arsenio118

 Level 5

JoinedMar 30, 2010Messages75

Resources: 5

#### Resources
Icons 3Skins 1Spells and Systems 1

Very nice tutorial although not every detail is complete i don't know what ability to use for the angle effect is it carrion swarm? well anyways this is really helpful rep+

#### N1ghthawk

#### N1ghthawk

 Level 10

JoinedOct 31, 2009Messages352

Resources: 3

#### Resources
Maps 1Tutorials 2

 Very nice tutorial although not every detail is complete i don't know what ability to use for the angle effect is it carrion swarm? well anyways this is really helpful rep+


Click to expand...


If your basing it off an object editor spell, then yes carrion swarm works best. However, there are ways to make a custom-coded/triggered spell using angles.

#### arsenio118

#### arsenio118

 Level 5

JoinedMar 30, 2010Messages75

Resources: 5

#### Resources
Icons 3Skins 1Spells and Systems 1

thanks for the info

#### deepstrasz

#### deepstrasz

Map Reviewer

 Level 84

JoinedJun 4, 2009Messages22,555

Resources: 7

#### Resources
Maps 4Spells and Systems 2Sounds 1

Hey-ya!


I've gots meself a problem! When I load a game where there are permanent lightning effects between some items and units, they just don't appear anymore. The bigger issue is that when the hero picks one of those items up (the idea is that the lightning should disappear) the game crashes telling about a memory 000etc. error.


Do you have any ideas of how could I fix this? Please.

#### Ziggycuff

#### Ziggycuff

 Level 2

JoinedDec 29, 2017Messages16

Hello - Thank you for making this guide.

I use it alot as a reference to making abilities for bosses in my campaign map (Just made an ability that shoots 3 tornadoes out and knocks units around randomly when units are in range of tornado based on your triggers)

Its very helpful and educational. good job.

#### bruunk

#### bruunk

 Level 9

JoinedMar 17, 2016Messages156

Resources: 7

#### Resources
Icons 5Maps 2

Can you no longer override the Art - Pitch angle (Degrees) field? I have it set to -90 yet the beam effect is completely upright.

Edit: even tried creating a special effect at point and changing its pitch, yaw, and roll. Still nothing, completely upright.


 Last edited: May 22, 2020

#### ravellani

#### ravellani

 Level 5

JoinedMay 24, 2017Messages93

This is a great tutorial, I have been wondering how to do this kind of thing for a while and this thread did it.

#### Mekanical

#### Mekanical

 Level 8

JoinedFeb 9, 2009Messages99

Resources: 3

#### Resources
Tools 1Maps 2

bruunk said:

 Can you no longer override the Art - Pitch angle (Degrees) field? I have it set to -90 yet the beam effect is completely upright.

Edit: even tried creating a special effect at point and changing its pitch, yaw, and roll. Still nothing, completely upright.


Click to expand...

It works but the angles are quite hard to config when you've never done it before.

Special Effect - Create a special effect at ((Position of (Triggering unit)) offset by (0.00, 65.00)) using war3mapImported\Living StatuPedestal.mdx

Special Effect - Set Orientation of (Last created special effect) to yaw: 90.00, pitch: 0.00, roll: 0.00


 Last edited: May 20, 2024

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 Poison Cloud Special Effects[ Need Help]


Jstan

Jan 30, 2014

Triggers & Scripts

Replies17Views3K

Feb 1, 2014
Jstan

Locked

 Need Help - War stomp + more war stomp


Paulus

Jan 19, 2014

Triggers & Scripts

Replies1Views2K

Jan 19, 2014
Nichilus

 The Wonderful World of Special effect Modifications!


Devalut

Nov 8, 2019

Misc/General Mapping Tutorials

Replies11Views6K

Oct 2, 2020
Chaosy

Locked

[Trigger]
 Lightning Effects


Pietje

Jun 10, 2011

Triggers & Scripts

Replies2Views2K

Jun 10, 2011
Pietje

Locked

[Trigger]
 Lightning Dash Ability


BETABABY

Nov 13, 2015

Triggers & Scripts

Replies20Views5K

Nov 15, 2015
Bribe

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