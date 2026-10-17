---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/reviving-heroes-at-a-changeable-region.161913/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Reviving Heroes at a Changeable Region
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/reviving-heroes-at-a-changeable-region.161913/>
#### dOT145

#### dOT145

 Level 3

JoinedJun 22, 2009Messages18

Resources: 1

#### Resources
Tutorials 1

# Reviving Heroes at a Changeable Region

## Table of Contents

Table of Contents

Top

Table of Contents

Introduction

What You Need

Creating the Regions and Triggers

Conclusion

## Introduction:

Hi, and welcome to my tutorial. This is my first tutorial, so sorry if it's not too great.

I looked through the tutorials a few times and I never saw a hero revival system (or at least one that revives in this manner), so I thought I might give tutorial-making a try .

If you have ever played an RPG or ORPG like Twilight's Eve ORPG, you might know what I'm talking about.

Basically, this tutorial will teach you how to set a region for a hero to respawn in, but when a hero enters another "respawnable" region (checkpoint, anyone?), it will set their respawn point to that region.

 This tutorial uses custom scripts in the triggers, but they are only used to prevent memory leaks (if you have no idea I'm talking about, check out Ralle's tutorial: Basic Memory Leaks; however, knowledge of memory leaks is not needed for this.)

## What you need:

Basic World Editor knowledge

World Editor (duh)

## Creating the Regions and Triggers:

###
 Creating the Regions:

What we will do in this section is very simple. Open up your map and press the [R] key to open the Region Palette. Create a region wherever you want the heroes to be able to respawn. You don't have to do this, but I like placing a Resurrection Stone and/or an Energy Field in/around the region. Create as many as you like (Although the more you create, the longer it will take later.)

If you are creating a map where your main unit is a hero and your hero spawns somewhere other than that very location upon selection, you may want to have the hero spawn at the first respawn region (a.k.a. checkpoint.)

### Creating the Triggers:

This is the hardest part. Hopefully, it's not too hard to understand.

First, I will explain the variables that are needed.

Variable NameVariable TypeArray? (size)Initial Value

loc*PointNo- None -

Player_Group*Player GroupYes (1)Empty Player Group

Respawn_Region*RegionYes (1)- None -

unit*UnitYes (1)- None -

*Names can be anything you want, but these are what I will be using in the triggers.

Now, the triggers:

When you pick your hero in your map, you will need to add an extra line to it. This is:

Set unit[(Player number of (Owner of (Triggering unit)))] = (Last created unit)
So, in my trigger, it comes out as:

Actions
Set loc = (Center of spawn <gen>)

Unit - Remove (Triggering unit) from the game

Unit - Create 1 Paladin for Player 1 (Red) at loc facing Default building facing degrees

Set unit[(Player number of (Owner of (Triggering unit)))] = (Last created unit)

Camera - Pan camera for Player 1 (Red) to loc over 1.00 seconds

Custom script: call RemoveLocation(udg_loc)

You don't need to copy this down, this is just an example (it is used in the example map provided)

Now, let's look at the Initalization:

Initalization
Events
Map initialization

Conditions

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
Set Respawn_Region[(Player number of (Picked player))] = Respawn1 <gen>

Set Player_Group[(Player number of (Picked player))] = (Player group((Picked player)))

What does this do?

Events
Map initialization

Pretty simple, runs when the map loads

Conditions
No conditions here!

Now for the actions:

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions

Very simple, just picking all of the players

This also does the actions for players like Player 12 (Brown). While, as far as I know, this does not change the outcome, you could change it to (All players matching (((Picked player) controller) equal to User) to be more precise. But it's your map, so do as you wish .

Set Respawn_Region[(Player number of (Picked player))] = Respawn1 <gen>

Set Player_Group[(Player number of (Picked player))] = (Player group((Picked player)))
The first one sets the region that every player's hero respawns in, just in case their hero dies before they enter a respawn region (somehow.) The second action sets a Player Group variable to a player. This just helps with the triggers later.

Now, we will create the trigger that sets or changes the respawn region for a hero:

Enter Region 1
Events
Unit - A unit enters Respawn1 <gen>

Conditions
((Owner of (Triggering unit)) controller) Equal to User

Actions
Set Respawn_Region[(Player number of (Owner of (Triggering unit)))] = Respawn1 <gen>

Game - Display to Player_Group[(Player number of (Owner of (Triggering unit)))] the text: Your Revival Point ...

I hope that wasn't quite as hard to digest, but just incase:

Events
Unit - A unit enters Respawn1 <gen>

This makes the trigger fire whenever a unit enters one of the possible respawn regions.

Conditions
((Owner of (Triggering unit)) controller) Equal to User

This condition checks if the owner of the unit that is entering the respawn region is controlled by a human. This prevents, say, a creep or NPC, from reviving there (if it is a hero) when it dies.

Actions
Set Respawn_Region[(Player number of (Owner of (Triggering unit)))] = Respawn1 <gen>

This action changes the respawn region for the player who owns the unit who entered the region.

Game - Display to Player_Group[(Player number of (Owner of (Triggering unit)))] the text: Your Revival Point ...
This action tells the player that their revival point was changed. Not required, but, as a player, is very helpful. This also, if you didn't notice, uses that player group variable that we set in the previous trigger.

Now, we just Copy and Paste that trigger and change the regions:

Enter Region 2
Events
Unit - A unit enters Respawn2 <gen>

Conditions
((Owner of (Triggering unit)) controller) Equal to User

Actions
Set Respawn_Region[(Player number of (Owner of (Triggering unit)))] = Respawn2 <gen>

Game - Display to Player_Group[(Player number of (Owner of (Triggering unit)))] the text: Your Revival Point ...

Do this for all of your regions.

Finally, the Respawn trigger:

Revive
Events
Unit - A unit Dies

Conditions
((Triggering unit) is A Hero) Equal to True

((Owner of (Triggering unit)) controller) Equal to User

Actions
Wait ((Real((Hero level of unit[(Player number of (Owner of (Triggering unit)))]))) x 5.00) seconds

Set loc = (Random point in Respawn_Region[(Player number of (Owner of (Triggering unit)))])

Hero - Instantly revive (Triggering unit) at loc, Hide revival graphics

Custom script: call RemoveLocation(udg_loc)

This might be harder than the other ones to understand, so:

Events
Unit - A unit Dies

Very simple, the trigger fires when a unit dies

Conditions
((Triggering unit) is A Hero) Equal to True

((Owner of (Triggering unit)) controller) Equal to User

These check two things: First, it checks is the dying unit is a hero. Not only is this needed for the first action, it also wouldn't make too much sense if a summoned unit was also killed and later revived. The second condition checks if the player who owns the unit who died was controlled by a human. We don't want Neutral and Computer heroes to respawn in places that should be safe!

Actions
Wait ((Real((Hero level of unit[(Player number of (Owner of (Triggering unit)))]))) x 5.00) seconds

This action is fairly simple. What it is is a wait action. I looks somewhat confusing, but all it does is check for the level of the hero that died, multiplies that number by 5, and waits that long. This is the respawn time, so you can change it to whatever you want (for example: [Wait 30.00 seconds])

Set loc = (Random point in Respawn_Region[(Player number of (Owner of (Triggering unit)))])
This action is purely to prevent memory leaks. It sets a variable, in my case "loc", to a random point in the region that the hero for that player is supposed to respawn in. We set it to a variable so we can destroy it later, again preventing memory leaks.

Hero - Instantly revive (Triggering unit) at loc, Hide revival graphics
This action revives the hero that died at "loc" (the location that we set in the previous action.) It also hides the revival graphics  (pointless.)

And finally:

Custom script: call RemoveLocation(udg_loc)
We destroy "loc", or the location inside the respawn region that the players' hero revives at. This is custom script used to prevent memory leaks. Just create a new action, select Custom Script, and C&P it from here into the box.

## Conclusion:

Well, that concludes my tutorial. I hope it didn't have too many mistakes and wasn't too hard to understand. Please correct me where I'm wrong and offer constructive criticism instead of flaming .

3.23.2010 - Created tutorial, fixed table so it actually was a table, added the changelog and credits, and created Table of Contents

3.24.2010 Changed "Dying unit" and "Entering unit" to Triggering unit

Me (dOT145) for creating tutorial

The makers of Twilight's Eve ORPG (don't their names) for not only a great ORPG but I also got the idea to do this from their map.

I have included a map to help you, if you need it. It is in the attachments.

--dOT145

#### Attachments

HeroRez pic.JPG

 12.6 KB

 · Views: 2,976



Example Hero Respawn.w3x

 17.8 KB

 · Views: 650




 Last edited: Mar 24, 2010

#### Darkness-4ever

#### Darkness-4ever

 Level 9

JoinedNov 4, 2007Messages932

Don't know how useful this would really be...

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Few things:


Change entering unit and dying unit to triggering unit.


Do the revive like this:

Hero - Instantly revive (Triggering unit)
All players doesn't leak.


Initial array sizes can be left to their default values.

#### dOT145

#### dOT145

 Level 3

JoinedJun 22, 2009Messages18

Resources: 1

#### Resources
Tutorials 1


 Darkness-4ever said:



 Don't know how useful this would really be...


Click to expand...

Two things:

First, it's actually a hero revive system. I have checked in the tutorials, and I didn't find any hero revive systems.

Second, how do you find a hero revive system less useful than an English tutorial, especially on a site about WC3?

Maker said:

 Few things:

Change entering unit and dying unit to triggering unit.

Do the revive like this:

Hero - Instantly revive (Triggering unit)
All players doesn't leak.

Initial array sizes can be left to their default values.


Click to expand...

Fixed.

Where did I use All players and destroy the group? I can only see my using All Players once, and I never destroyed it.

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

dOT145 said:

 Where did I use All players and destroy the group? I can only see my using All Players once, and I never destroyed it.


Click to expand...


You were just speculating about the leak in the hidden note.

#### dOT145

#### dOT145

 Level 3

JoinedJun 22, 2009Messages18

Resources: 1

#### Resources
Tutorials 1

Maker said:

 You were just speculating about the leak in the hidden note.


Click to expand...

I thought All Players Matching Condition leaked...

I guess not, I'll change that.

#### Darkness-4ever

#### Darkness-4ever

 Level 9

JoinedNov 4, 2007Messages932

dOT145 said:

 Second, how do you find a hero revive system less useful than an English tutorial, especially on a site about WC3?


Click to expand...

In case you failed to notice, a large portion of Hive users and even a few moderators didn't learn English as their primary language, and I find hardly any threads or posts in the modding sections pertaining to what your tutorial teaches, I'm sorry but I just don't consider it very useful.

#### dOT145

#### dOT145

 Level 3

JoinedJun 22, 2009Messages18

Resources: 1

#### Resources
Tutorials 1

Darkness-4ever said:

 In case you failed to notice, a large portion of Hive users and even a few moderators didn't learn English as their primary language, and I find hardly any threads or posts in the modding sections pertaining to what your tutorial teaches, I'm sorry but I just don't consider it very useful.


Click to expand...

I see that.

But that's your opinion about its usefulness, not mine or somebody else's.

#### Darkness-4ever

#### Darkness-4ever

 Level 9

JoinedNov 4, 2007Messages932

All I can give you is an opinion about usefulness as that is what you asked for, but thats enough of that, no tutorials will be getting approved anytime soon.

#### dOT145

#### dOT145

 Level 3

JoinedJun 22, 2009Messages18

Resources: 1

#### Resources
Tutorials 1

Yeah, sucks.

#### Grey Nightmare

#### Grey Nightmare

 Level 20

JoinedFeb 24, 2009Messages2,999

Resources: 1

#### Resources
Tutorials 1

Darkness-4ever said:

 All I can give you is an opinion about usefulness as that is what you asked for, but thats enough of that, no tutorials will be getting approved anytime soon.


Click to expand...


Why not?


I can see this being quite useful - not something I've seen in any map to come but I can think of several ideas that could make use of 'changing region'.  good job!

#### Darkness-4ever

#### Darkness-4ever

 Level 9

JoinedNov 4, 2007Messages932

Grey Nightmare said:

 Why not?


Click to expand...


Because the tutorial moderator was abducted by aliens last year and hive hasn't gotten around to getting a new one.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice... This tutorial is actually well formatted. Well done! Nice explanations too. I like how you broke down the trigger into parts and added an explanation for each line.


Overall, you covered everything including a proper timed respawn. So props to you. ~Approved.

 You must log in or register to reply here.


### Similar threads

 Hero Selection Systems


Tonks

Mar 23, 2007

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jun 8, 2023
dantles1992

K

 PSN Hero Selection


Kieve

Nov 23, 2007

Trigger (GUI) Editor Tutorials

Replies2Views8K

Nov 26, 2007
Kieve

K

 Making an Open Rpg


Ralle

Jan 12, 2006

Trigger (GUI) Editor Tutorials

Replies0Views47K

Jan 12, 2006
Ralle

 Hero Selection Systems


Eleandor

Aug 20, 2007

Trigger (GUI) Editor Tutorials
2

Replies51Views81K

Feb 28, 2021
Caalanuaa

Locked

[Trigger]
 Hero Reviving


greasontim

Mar 26, 2009

Triggers & Scripts

Replies2Views2K

Mar 26, 2009
greasontim

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