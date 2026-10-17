---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/taazs-item-system.20959/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Taazs Item System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/taazs-item-system.20959/>
#### TaaZ

#### TaaZ

 Level 3

JoinedJul 27, 2005Messages22

Resources: 1

#### Resources
Tutorials 1

TaaZ's ITEM SYSTEM

Well, I know some of you have been looking for a decent item system that lets people only use one weapon at a time or one armor, or whatever, so this tutorial was produced from being bored.

This tut might give you beginners more knowledge with triggers and/or variables.

What you need:

A. Basic Knowledge of the Object Editor

B. Basic Knowledge of Variables

C. Basic Knowledge of the Trigger Editor

-----------------------------------------------------------

First and foremost, we are going to start with a few variables...

If you are going to have 5 players in your map that cant pick up two weapons (I'll be using weapons as an example) then you need 5 variables.

I named my variables

Only1Weapon1

Only1Weapon2

Only1Weapon3

Only1Weapon4

Only1Weapon5
The numbers at the end represents the player number, ex. player 1 red = 1

These variables need to be boolean and false as default

Lets get started on the triggers...

Make a trigger called "Only1Weapon1"

And another trigger called "Able2PickUpAfterDrop1"

NOTE: these triggers will be for player 1 red

Go into the trigger Only1Weapon1 and create a new Event:

Unit - A unit owned by Player 1 (Red) Acquires an item
Simple Event, no need to explain it.

Now create a few Actions, this is what it should look like after I explain some of it:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-class of (Item being manipulated)) Equal to Artifact

Only1Weapon1 Equal to False

Then - Actions
Set Only1Weapon1 = True

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-class of (Item being manipulated)) Equal to Artifact

Only1Weapon1 Equal to True

Then - Actions
Trigger - Turn off able to pick up after drop1 <gen>

Item - Move (Item being manipulated) to (Position of (Triggering unit))

Game - Display to Player Group - Player 1 (Red) the text: You can only wield ...

Wait 0.01 seconds

Trigger - Turn on able to pick up after drop1 <gen>

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-class of (Item being manipulated)) Equal to Artifact

Only1Weapon1 Equal to False

Then - Actions
Set Only1Weapon1 = True

Else - Actions
Do nothing

Let's get started on it.

First make an if/then/else Action.

This helps it when a unit picks up an item, if the unit already has a weapon it wont pick up the new weapon.

Now for the If - Conditions you need to make:

(Item-class of (Item being manipulated)) Equal to Artifact

Only1Weapon1 Equal to False
What this does is it lets the trigger know if the item being picked up is a weapon and checks if there is already a weapon being wielded.

Now we got that down we need to make a Then - Action this is what it should be:

Set Only1Weapon1 = True
This simply tells it that the unit has picked up a weapon.

Now for the Else - Actions you need another if/then/else.

After you make another if/then/else in the else action you now make some If - Conditions.

(Item-class of (Item being manipulated)) Equal to Artifact

Only1Weapon1 Equal to True
This will tell it that there is a weapon in the inventory yet and doesn't not let the unit pick up the weapon, goody? nope, its not over yet

now for the Then - Actions you make the following action

Then - Actions
Trigger - Turn off able to pick up after drop1 <gen>

Item - Move (Item being manipulated) to (Position of (Triggering unit))

Game - Display to Player Group - Player 1 (Red) the text: You can only wield one weapon at a time.

Wait 0.01 seconds

Trigger - Turn on able to pick up after drop1 <gen>

Well, this is just a helpful thing for the second trigger you made, "Able2PickUpAfterDrop1"

and tells it what to do with a weapon if the unit is not able to pick it up, also a little user friendly text message that informs the owner of the unit trying to pick up 2 weapons that you can only wield one weapon at a time.

in the Else - Action create another if/then/else action.

For the If - Conditions in the newly created if/then/else, do this:

(Item-class of (Item being manipulated)) Equal to Artifact

Only1Weapon1 Equal to False
This lets it know that there is no weapon in the inventory and that the item being picked up is a weapon, easy?

Now for the Then - Actions:

Set Only1Weapon1 = True
Nice huh? This tells it that a weapon has been picked up and put into the inventory of unit owned by player 1 red.

And in the Else - Actions:

Do nothing
Short and sweet.

Now lets get to that second trigger. "Able2PickUpAfterDrop1"

Create an Event:

Unit - A unit owned by Player 1 (Red) Loses an item
Now create an if/then/else action, and do the following:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-class of (Item being manipulated)) Equal to Artifact

Only1Weapon1 Equal to True

Then - Actions
Set Only1Weapon1 = False

Wait 0.01 seconds

Else - Actions
Set Only1Weapon1 = False

NOTE: My weapon classes are artifacts, you can also use permanent or charged or whichever class you want, but all weapon have to be in one class and armors in another class, don't use anything BUT weapons in class artifact if your weapon class is an artifact.

To use this trigger for all 5 players you need to use different variables and more triggers. EX: Player 2 blue's trigger - One1Weapon2, P2 Blues Variable - Only1Weapon2

Credits:World Editor (Of course )

Written by TaaZ


 Last edited: Mar 26, 2007

#### Hazop / Sarven

#### Hazop / Sarven

 Level 15

JoinedJul 2, 2015Messages880

Resources: 6

#### Resources
Maps 6

Can you post a map that has all these triggers for easy transfer?

#### DylanSpencer

#### DylanSpencer

 Level 1

JoinedMay 16, 2019Messages3

This looks useful and simple and I'm going to try it out

Edit - Works like a charm, gonna use it in my custom game; Breath of Reforged, adding you to the credits


 Last edited: May 16, 2019

#### Cespie

#### Cespie

 Level 8

JoinedMay 21, 2019Messages435

TaaZ said:

TaaZ's ITEM SYSTEM

First and foremost, we are going to start with a few variables...

If you are going to have 5 players in your map that cant pick up two weapons (I'll be using weapons as an example) then you need 5 variables.

I named my variables

Only1Weapon1

Only1Weapon2

Only1Weapon3

Only1Weapon4

Only1Weapon5
The numbers at the end represents the player number, ex. player 1 red = 1

These variables need to be boolean and false as default


Click to expand...

It would make a lot more sense to simply make an array variable called Only1Weapon[].

That way, you could loop through all players in a single trigger instead of making separate triggers for each player.

 You must log in or register to reply here.


### Similar threads

 Item-drop-system


SlayerII

Mar 6, 2009

Trigger (GUI) Editor Tutorials

Replies24Views20K

Feb 28, 2010
SlayerII

 IDS (Infinite Number of Item Classes)


-Kobas-

Mar 29, 2010

Trigger (GUI) Editor Tutorials

Replies25Views47K

Oct 7, 2015
BPower

 Making an Open Rpg


Ralle

Jan 12, 2006

Trigger (GUI) Editor Tutorials

Replies0Views47K

Jan 12, 2006
Ralle

 RDZ's Incredible Item Stacking System of Doom


Rao Dao Zao

Oct 5, 2005

Trigger (GUI) Editor Tutorials

Replies1Views22K

Feb 15, 2015
emperor_d3st

Locked

[Solved]
 Item Limit Trigger (Needs Fix)


part207

Mar 29, 2012

Triggers & Scripts

Replies4Views2K

Mar 29, 2012
part207

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