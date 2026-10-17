---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/wow-mount-system-for-semi-n00b.266313/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Wow Mount System for Semi N00b
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/wow-mount-system-for-semi-n00b.266313/>
#### Arad MNK

#### Arad MNK

 Level 17

JoinedDec 11, 2014Messages2,003

Resources: 6

#### Resources
Maps 3Spells and Systems 2Tutorials 1

 Mounts


Arad MNK's Mount System

 Introduction


What are Mounts in WoW?


Mounts are Some Mobs which spawn and after a Walk, they despawn. sometimes they are earned by some Special quests or Achieved by an Item. In This tutorial, it will be Bondstone. also, in this tutorial, you may only have 1 Mount.

 Requirements



Requirements

Basic Knowledge of Variables,Trigger EditorandObject Editor.

 Part I


Part I:Setting up The Unit, Abilities and Model.

First, Go to OE and Base the Unit on Stag.

Then, Select Custom Unit and Set the Name to Whatever you want.


In this Case, I named it ''Horse''.


Now You see the Mount on the Left side of OE.


You May Want to Change the Model and Icon of the Mount. If you Couldn't Find a fitting one, try Importing One! in this case, I'll use ''Riderless Horse''.

Done!


Now, The Unit is Ready Itself. If you Like, you can Add Combat Possiblities Or add more health to it. (Default 15)Now, We Go to the Ability Category, So we Could Edit the Mounting Ability. Head to  Special.


Find Load (Goblin Zeppelin)and Then Hit New Ability and Name itLoad.


NOTE: USING SHIPS LOAD MIGHT CAUSE WEIRD LOAD THINGS.

Then Find Text ->Editor Suffix . Change it to (Mounts).


Do the Same with''Cargo Hold (Goblin Zeppelin)''and ''Unload (Air Transports).

Then, EditCargo Hold With:Data -> Cargo Capacity

Change it to 1.

Then, Head back toCategory Units, and give the Mount The Abilities you Just Edited+Cargo Hold Death (Neutral Hostile).

Now we Have to create an item:Bondstone (Name it whatever you like)

Base it from whatever you like. I Will use:

Wirt's Other Leg.

Put A Fitting Description, Icon, Cost and Stock Interval.The Result will be Close to the Picture.

IMPORTANT NOTE: Change the Stats -> Actively used to True!

Then, you will have to create an Ability. Base it on Shadow Strike and create another buff named ''Bond''and put a Fitting Description, and Icon. Don't forget to Change the Ability Damage (0), Level (1),Buff (Bond), check the item ability and uncheck hero ability.


Then give the Item The Ability.

Now, Our Unit and Item are ready!!! Now, Time fortriggers...

 Part II


Part II:Variables and Triggers

Variables Needed:

Horse (Name it Whatever you Want) Of Type Unit array

Integer_Horse (Name it Whatever you want again) Of Type Integer

Temp_Point Of Type Point

Player_Has_Mount Of Type Boolean

Player_Has_Mount Of Type Boolean Array

Temp_Unit Of Type Unit

Point_HorseDespawn Of Type Point


Regions Needed:

Horse Spawn


Horse Despawn

And Now, Triggers start.

Init
Events
Map initialization

Conditions

Actions
Set Point_HorseDespawn = (Center of Horse Despawn <gen>)

Horse Spawn
Events
Time - Every 45.00 seconds of game time

Conditions

Actions
-------- Checks if there is any Horse on the Map --------
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Horse[Integer_Horse] is alive) Equal to (==) True

Then - Actions
-------- Removing It --------

Unit - Remove Horse[Integer_Horse] from the game

-------- --------

-------- Sets the Unit Variable to Null so there wouldn't be leaks. --------

Set Horse[Integer_Horse] = No unit

-------- --------

Else - Actions
Do nothing

-------- These Integers are Used to make the Mounts MUI --------

Set Integer_Horse = (Integer_Horse + 1)

-------- --------

-------- Setting Temp_Point to avoid leaks --------

Set Temp_Point = (Center of Horse Spawn <gen>)

-------- --------

-------- Creating a Horse --------

Unit - Create 1 Horse for Neutral Passive at Temp_Point facing Default building facing (270.0) degrees

-------- --------

-------- Storing the horse into a Variable --------

Set Horse[Integer_Horse] = (Last created unit)

-------- --------

-------- Slowing the Movement Of Horse --------

Unit - Set Horse[Integer_Horse] movement speed to 150.00

-------- --------

-------- a Special Effect to make it look better --------

Special Effect - Create a special effect at Temp_Point using Abilities\Spells\Other\Charm\CharmTarget.mdl

-------- --------

-------- Destroying it --------

Special Effect - Destroy (Last created special effect)

-------- --------

-------- Removing the Location to Avoid Possible Leaks --------

Custom script: call RemoveLocation(udg_Temp_Point)

-------- --------

-------- Ordering The Horse to Move --------

Unit - Order Horse[Integer_Horse] to Move To Point_HorseDespawn

-------- --------

Horse Going
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
-------- Ordering The Horse to Move --------

Unit - Order Horse[Integer_Horse] to Move To Point_HorseDespawn

-------- --------

Horse Catching
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to (==) Bondstone Bonding Power

(Target unit of ability being cast) Equal to (==) Horse[Integer_Horse]

Actions
-------- Storing the Caster into a Varable --------

Set Temp_Unit = (Triggering unit)

-------- --------

-------- Checks if the Caster has a Mount --------

-------- --------
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Player_Has_Mount[(Player number of (Owner of Temp_Unit))] Equal to (==) False

Then - Actions
-------- This Makes the Caster own The Mount --------

Unit - Change ownership of Horse[Integer_Horse] to (Owner of Temp_Unit) and Change color

-------- --------

-------- Gives a High Speed to the Horse --------

Unit - Set Horse[Integer_Horse] movement speed to 300.00

-------- --------

-------- Sets the Unit Variable to Null so there wouldn't be leaks. --------

Set Horse[Integer_Horse] = No unit

-------- --------

-------- This Makes the Players don't have more than 1 mount --------

Set Player_Has_Mount[(Player number of (Owner of Temp_Unit))] = True

-------- --------

-------- Takes the Bondstone --------

Item - Remove (Item carried by Temp_Unit of type Bondstone)

-------- --------

-------- Says Officially --------

Game - Display to (All players) the text: ((Name of (Owner of Temp_Unit)) + Has catched the Mount: Horse)

-------- --------

-------- Setting Temp_Unit to Null so there wouldn't be leaks --------

Set Temp_Unit = No unit

-------- --------

Else - Actions
-------- Warns the Player --------

Game - Display to (Player group((Owner of Temp_Unit))) the text: You already have a ...

-------- --------

-------- Takes the Bondstone --------

Item - Remove (Item carried by Temp_Unit of type Bondstone)

-------- --------

-------- Setting Temp_Unit to Null so there wouldn't be leaks --------

Set Temp_Unit = No unit

-------- --------

Mount Death
Events
Unit - A unit Dies

Conditions
(Unit-type of (Triggering unit)) Equal to (==) Horse

(Owner of (Triggering unit)) Not equal to (!=) Neutral Passive

Actions
-------- Makes the Player have Mount --------

Set Player_Has_Mount[(Player number of (Owner of (Triggering unit)))] = False

-------- --------

Horse Despawn
Events
Unit - A unit enters Horse Despawn <gen>

Conditions
(Unit-type of (Triggering unit)) Equal to (==) Horse

(Owner of (Triggering unit)) Equal to (==) Neutral Passive

Actions
-------- Setting Temp_Point to avoid leaks --------

Set Temp_Point = (Center of Horse Despawn <gen>)

-------- --------

-------- a Special Effect to make it look better --------

Special Effect - Create a special effect at Temp_Point using Abilities\Spells\Other\Charm\CharmTarget.mdl

-------- --------

-------- Destroying it --------

Special Effect - Destroy (Last created special effect)

-------- --------

-------- Removing the Location to Avoid Possible Leaks --------

Custom script: call RemoveLocation(udg_Temp_Point)

-------- --------

-------- Removing The Horse --------

Unit - Remove Horse[Integer_Horse] from the game

-------- --------

-------- Sets the Unit Variable to Null so there wouldn't be leaks. --------

Set Horse[Integer_Horse] = No unit

-------- --------

-------- Says Officially --------

Game - Display to (All players) the text: Horse has despawned...

-------- --------

 Credits


Credits:

Venger07: Some Help with my system

Blizzard Entertaiment: For making Warcraft 3 and it's World Editor

ACDSee Pro: I Used it to Edit the Pictures

Snipping Tool: My clipboard Didn't work, so I Used this.

JassNewGenPack: I Used it

#### Attachments

OE 01.PNG

 334.6 KB

 · Views: 679



OE 2.PNG

 137.8 KB

 · Views: 695



OE 3.PNG

 131 KB

 · Views: 640



OE 4.PNG

 137.1 KB

 · Views: 638



OE 5.PNG

 154 KB

 · Views: 640



OE 6.PNG

 140.2 KB

 · Views: 676



OE 7.PNG

 121.9 KB

 · Views: 645



OE 8.PNG

 130 KB

 · Views: 566



OE 10.PNG

 86.5 KB

 · Views: 565



OE 11.PNG

 85.9 KB

 · Views: 600



OE 9.PNG

 153.3 KB

 · Views: 656



Mount System By AMNK.w3x

 37.8 KB

 · Views: 206




 Last edited: Jun 6, 2015

#### Duke

#### Duke

 Level 10

JoinedNov 23, 2006Messages592

Resources: 1

#### Resources
Maps 1

the pictures aren't showing m8

#### Pinzu

#### Pinzu

 Level 15

JoinedNov 30, 2007Messages1,202

Resources: 7

#### Resources
Spells and Systems 5JASS 1Tutorials 1

if you have a variable to store TriggeringUnit() it would be slightly more efficient. Basically anything you call repeated times is preferably placed in a temporarily variable.

TriggeringPlayer

TriggerUnit

etc

So declare this before all you have right now:

Set tempUnit = (Triggering unit)
 and replace every part where you have TriggeringUnit with tempUnit.

My memory don't serve me well, but I think you have to destroy special effects to prevent leaks.

___________

You've done a great you on the layout of the tutorial, except for the images not working. ^^

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Good things:

Neat layout and good use of colors that doesn't hurt the eyes.

Pictures

Bad things:

The pictures are broken and can't be viewed. If the images are from one of your albums make sure it's public and not private.

There are quite a few minor triggering mistakes.

Game - Display to (All players matching ((Matching player) Equal to (==) (Owner of (Triggering unit)))) the text: You already have a
should be

Game - Display to (Owner of (Triggering unit)) the text: You already have a

Temp_Point = (Center of Horse Despawn <gen>)
If you're going so store the same location into a variable every seconds it's better to set the location at an init trigger and don't destroy the location, ever.

In the horse catching trigger you should use a variable for triggering unit since you use it multiple times.

The special effect can be stored into an variable at init to gain some speed and increase readability.

Unit - Remove Horse[Integer_Horse] from the game
 leaks

AND PLEASE DON'T POST TRIGGER WITH COMMENTS IN! IT LOOKS UGLY! *end of rage*

#### Arad MNK

#### Arad MNK

 Level 17

JoinedDec 11, 2014Messages2,003

Resources: 6

#### Resources
Maps 3Spells and Systems 2Tutorials 1

Duke said:

 the pictures aren't showing m8


Click to expand...

Pinzu said:

 if you have a variable to store TriggeringUnit() it would be slightly more efficient. Basically anything you call repeated times is preferably placed in a temporarily variable.

TriggeringPlayer

TriggerUnit

etc

So declare this before all you have right now:

Set tempUnit = (Triggering unit)
 and replace every part where you have TriggeringUnit with tempUnit.

My memory don't serve me well, but I think you have to destroy special effects to prevent leaks.

___________

You've done a great you on the layout of the tutorial, except for the images not working. ^^


Click to expand...

Chaosy said:

 Good things:

Neat layout and good use of colors that doesn't hurt the eyes.

Pictures

Bad things:

The pictures are broken and can't be viewed. If the images are from one of your albums make sure it's public and not private.

There are quite a few minor triggering mistakes.

Game - Display to (All players matching ((Matching player) Equal to (==) (Owner of (Triggering unit)))) the text: You already have a
should be

Game - Display to (Owner of (Triggering unit)) the text: You already have a

Temp_Point = (Center of Horse Despawn <gen>)
If you're going so store the same location into a variable every seconds it's better to set the location at an init trigger and don't destroy the location, ever.

In the horse catching trigger you should use a variable for triggering unit since you use it multiple times.

The special effect can be stored into an variable at init to gain some speed and increase readability.

Unit - Remove Horse[Integer_Horse] from the game
 leaks

AND PLEASE DON'T POST TRIGGER WITH COMMENTS IN! IT LOOKS UGLY! *end of rage*


Click to expand...

Thanks for the feedback, Duke, Pinzu and Chaosy! Gonna Fix These Tomorrow. (Like 6AM GMT)

Chaosy said:

 AND PLEASE DON'T POST TRIGGER WITH COMMENTS IN! IT LOOKS UGLY! *end of rage*


Click to expand...

Read the Title: ''WoW Mount System for Semi-n00b'', it means all triggers must be Defined as this is a Tutorial Fitting for the N00bs, Not Experts.

Pinzu said:

 You've done a great you on the layout of the tutorial, except for the images not working. ^^


Click to expand...

Thanks ^^! Gonna Fix the Images, just found out why it doesn't show for others. (Attachments again...)

Edit: Done!


 Last edited: Jun 6, 2015

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

This is a neat approach to mounts. But I do have a couple of issues with the tutorial:

 Having too many colors and too many fonts is distracting. See this link:

https://www.hiveworkshop.com/forums...t-tutorial-please-read-before-posting-119320/

So you should stick to the regular font, regular colors, and just use one special color (not something too vibrant) for headings or for emphasis.

 This doesn't really seem to be like WoW. I expected it to be something where you have an ability or an item that summons a mount. You can probably rename the tutorial to just "A Mount System".

 With tutorials, it is better to teach a variety of approaches to tackle a particular problem. If you have too specific of an implementation, then it is less likely that the tutorial will be useful. In this case, you have a "mount catching system". It will only be useful to those who want to implement that exact system. But I personally haven't heard of it before (don't get me wrong, it is a cool idea), but I feel like this tutorial could be expanded so it would be useful for anyone looking for mount implementations.

 For the text in the tutorial, you should only use capitalization at the start of a sentence or for the highlighted parts.

The tutorial is decent so far and the test map is fine. But to summarize my point above, it feels a lot more like a system rather than a tutorial (it has good explanations, but the implementation is very specific).

#### Arad MNK

#### Arad MNK

 Level 17

JoinedDec 11, 2014Messages2,003

Resources: 6

#### Resources
Maps 3Spells and Systems 2Tutorials 1

PurgeandFire said:

 This is a neat approach to mounts. But I do have a couple of issues with the tutorial:

 Having too many colors and too many fonts is distracting. See this link:

https://www.hiveworkshop.com/forums...t-tutorial-please-read-before-posting-119320/


So you should stick to the regular font, regular colors, and just use one special color (not something too vibrant) for headings or for emphasis.

 This doesn't really seem to be like WoW. I expected it to be something where you have an ability or an item that summons a mount. You can probably rename the tutorial to just "A Mount System".

 With tutorials, it is better to teach a variety of approaches to tackle a particular problem. If you have too specific of an implementation, then it is less likely that the tutorial will be useful. In this case, you have a "mount catching system". It will only be useful to those who want to implement that exact system. But I personally haven't heard of it before (don't get me wrong, it is a cool idea), but I feel like this tutorial could be expanded so it would be useful for anyone looking for mount implementations.

 For the text in the tutorial, you should only use capitalization at the start of a sentence or for the highlighted parts.


The tutorial is decent so far and the test map is fine. But to summarize my point above, it feels a lot more like a system rather than a tutorial (it has good explanations, but the implementation is very specific).


Click to expand...


Thanks for your Detailed Review. I'll try to do whatever you said.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

The image's quality is poor (for me) such that I can't read the text in your screenshot.

The comment spacing is too much and do not use

Do Nothing

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Shadow Flux said:

 The image's quality is poor (for me) such that I can't read the text in your screenshot.


Click to expand...


The quality is good for me. No issues reading.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

^Hmm strange, maybe viewing it in mobile phone is the reason.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Shadow Flux said:

 ^Hmm strange, maybe viewing it in mobile phone is the reason.


Click to expand...


I don't know if the image would go from crap to normal by just changing screen but w/e.

#### Arad MNK

#### Arad MNK

 Level 17

JoinedDec 11, 2014Messages2,003

Resources: 6

#### Resources
Maps 3Spells and Systems 2Tutorials 1

Shadow Flux said:

 The comment spacing is too much and do not use

Do Nothing

Click to expand...


Added to the To do List

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Will you update the tutorial regarding given critique?

edit: waiting for answer, gy for now.


 Last edited: Feb 27, 2017

 You must log in or register to reply here.


### Similar threads

Locked

 Mount


Klann

Jan 17, 2012

World Editor Help Zone

Replies5Views2K

Jan 17, 2012
shadowvzs

Locked

 Need help with mount spell!


Giggles

Jun 6, 2005

Triggers & Scripts

Replies2Views2K

Jun 6, 2005
Giggles

Locked

 Distance between


Chewii

Mar 17, 2010

Triggers & Scripts

Replies8Views2K

Mar 19, 2010
M_Vegeta

Locked

 Making a spell MUI


Quetzalcotl

Apr 25, 2009

Triggers & Scripts

Replies8Views1K

Apr 26, 2009
Yixx

I

Locked

[Trigger]
 Sunbeam


ImBamboo

May 31, 2008

Triggers & Scripts

Replies3Views1K

Jun 1, 2008
Squiggy

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