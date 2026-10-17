---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/hero-selection-systems.38699/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Hero Selection Systems
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/hero-selection-systems.38699/>
#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

As Tonks has left the Hive, I decided to recreate this tutorial.

Introduction:

This tutorial will show you various methods of hero selection systems.

Many AoS or orpg's use hero selection systems which allow you to pick a hero out of one of the various heroes available in the game. In this tutorial I'll try to explain as good as possible on how to create such a system.

A map will be attached with all triggers. They work so you can test them yourself.

Proceed to the Contents to see the index of this tutorial.

Every trigger is written in GUI (pro's: easy to understand for everybody, cons: possible lag) on the exception of a few custom scripts.

VERY IMPORTANT:  these triggers have not been de-leaked. The thought behind this is that if you need a tutorial for something as simple as this, you probably don't bother about them anyway. In any way, you can check out a tutorial on leaks on this website to learn more about them.

Index:

Hero Taverns

Circle of Power Heroes

Dialog menu

Extended Dialog (further explanation)

Clicking/Doubleclicking

Next/Previous Hero Viewing(completelly made by Tonks, thanks to him)

Hero Taverns

Description: If you ever played an AoS (such as Defence of the Ancients), you have certainly seen this system. Basically, you have a circle of power near a tavern. In this tavern you can select one of the various heroes. Every tavern can only sell up to 12 heroes, but you can add multiple taverns to your map ofcourse.

Important note: This system is slightly different from those seen in the AoS maps. In case you want to add randomize modes etc., I recommend you to download the DotA template at thehelper.net

Recommended: Basic knowledge of both the trigger editor and the Object editor. Works well with 12 or more heroes.

Part 1: The Tavern Tutorial

The Object Data:

We will start with editing the necessary object data.

First, we create our first hero and we change the following values:

* Stats - Food Cost: 0

* Stats - Gold Cost: 0

* Stats - Lumber Cost: 0

* Stats - Stock Maximum: 1

* Stats - Stock Replenish Interval: 0

* Stats - Stock Start Delay: 0

We repeat this process for every hero in the game. In this tutorial, I'll do this to 4 heroes: The 4 human heroes.

Next we will create a new Tavern. We create a new unit and base it on the Tavern. The unit is located under "Neutral, Building".

We change the following value:

Techtree - Units Sold: Add all heroes to this value. If you have more than 12 heroes, simply create a second tavern and repeat this tutorial after finishing it.

The Map Data:

You need to place the tavern you just made on your map. Now add circles of power (they're located in the unit palette under Neutral, Campaign) for every player and place them next to the tavern.

You must create a region (in the region palette) on the map where you want your heroes to spawn. I've named this region to "HeroSpawn".

The Trigger Data:

Following Variables will be used in this tutorial:

NameTypeInitial ValueArray

Tavern1HeroesUnit-typeEmpty100

HeroChosenByPlayerBooleanFalse12

TempPointPointnono

You can open the Variable Editor by going to the trigger editor and pressing "Cntrl + B" or alternativelly going to "Edit, Variables".

First we create a new folder. Call it "Tavern Heroes".

We create a new trigger in this folder, we call it "Tavern Initialization".

This is how my trigger looks like:

Tavern Initialization
Events
Map initialization

Conditions

Actions
Set Tavern1Heroes[1] = Paladin (Tavern)

Set Tavern1Heroes[2] = Archmage (Tavern)

Set Tavern1Heroes[3] = Mountain King (Tavern)

Set Tavern1Heroes[4] = Blood Mage (Tavern)

-------- If you wish to add more than 4 units to the Tavern, you must add them here. --------

This trigger will add my 4 hero-types to a variable. This will be used later. I have only made 4 heroes for this tutorial, in case you have more heroes, just repeat the actions for all heroes.

Now we need to create the trigger where you buy a hero and move it to the "HeroSpawn" region.

Events
Unit - A unit Sells a unit

Conditions
And - All (Conditions) are true
Conditions
((Sold unit) is A Hero) Equal to True

Or - Any (Conditions) are true
Conditions
(Selling unit) Equal to Tavern 0000 <gen>

First we add the events. This trigger will run whenever a unit (such as a tavern) sells another unit (such as a hero).

In the conditions we check if the SOLD unit is a hero, and if the SELLING unit is the tavern you placed on the map. If you have multiple taverns, you add those to the "OR - CONDITIONS".

For example: if I have multiple taverns, my trigger will be looking like this:

Events
Unit - A unit Sells a unit

Conditions
And - All (Conditions) are true
Conditions
((Sold unit) is A Hero) Equal to True

Or - Any (Conditions) are true
Conditions
(Selling unit) Equal to Moon Tavern 0000 <gen>

(Selling unit) Equal to Sun Tavern 0001 <gen>

(Selling unit) Equal to Earth Tavern 0002 <gen>

Next, we will add actions. Right now, our trigger will run when a tavern sells a hero. So what should happen then?

We need to check if you already bought a hero. When you already bought a hero, you're not allowed to buy a second hero! We will instantly remove the second hero in the "Then - Actions".

If this is your first hero, you should buy the hero and instantly move it to "HeroSpawn", where you can start the game.

Tavern Hero Chosen
Events
Unit - A unit Sells a unit

Conditions
And - All (Conditions) are true
Conditions
((Sold unit) is A Hero) Equal to True

Or - Any (Conditions) are true
Conditions
(Selling unit) Equal to Tavern 0000 <gen>

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Herochosenbyplayer[(Player number of (Owner of (Sold unit)))] Equal to True

Then - Actions
Unit - Remove (Sold unit) from the game

Game - Display to (All players) the text: You already chose a...

Else - Actions
Set Herochosenbyplayer[(Player number of (Owner of (Sold unit)))] = True

Unit - Move (Sold unit) instantly to (Center of HeroSpawn <gen>)

Camera - Pan camera for (Owner of (Sold unit)) to (Center of HeroSpawn <gen>) over 1.00 seconds

Selection - Clear selection for (Owner of (Sold unit))

Selection - Add (Sold unit) to selection for (Owner of (Sold unit))

Wait a second. Right now everyone can buy the same hero. So every player will be able to play with for example a Paladin. Now, we probably do not want to see 12 Paladins fighting on the battlefield, do we? If we do, your trigger is finished. Else, go on with the tutorial.

So let's add the following action:

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
Player - Make (Unit-type of (Sold unit)) Unavailable for training/construction by (Picked player)

Thanks to this action, whenever someone buys a hero, this hero will no longer be available for all players.

Your final trigger should look like this:

Tavern Hero Chosen
Events
Unit - A unit Sells a unit

Conditions
And - All (Conditions) are true
Conditions
((Sold unit) is A Hero) Equal to True

Or - Any (Conditions) are true
Conditions
(Selling unit) Equal to Tavern 0000 <gen>

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Herochosenbyplayer[(Player number of (Owner of (Sold unit)))] Equal to True

Then - Actions
Unit - Remove (Sold unit) from the game

Game - Display to (All players) the text: You already chose a...

Else - Actions
Set Herochosenbyplayer[(Player number of (Owner of (Sold unit)))] = True

Unit - Move (Sold unit) instantly to (Center of HeroSpawn <gen>)

Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
Player - Make (Unit-type of (Sold unit)) Unavailable for training/construction by (Picked player)

Camera - Pan camera for (Owner of (Sold unit)) to (Center of HeroSpawn <gen>) over 1.00 seconds

Selection - Clear selection for (Owner of (Sold unit))

Selection - Add (Sold unit) to selection for (Owner of (Sold unit))

Voila, I have explained how to create a Tavern. In the rest of this tutorial, I'll explain to you how to repick your hero and how to randomise it.

Part 2: Randomising

When randomising, you make the computer pick for you. In other words: you don't know what hero you'll be playing. Everything in the previous tutorial should be working already, as we will build on it.

The Object Data:

Go to the unit editor and select the Night Elf Wisp. We will edit this unit (or, in case you'll use a wisp in the game, create a new one based on it).

Following Values will be changed:

 Abilities - Normal - none

 Stats - Food Cost: 0

 Stats - Gold Cost: 0

 Stats - Lumber Cost: 0

 Stats - Stock Maximum: 0

 Stats - Stock Replenish Interval

 Text - Name: Hero Chooser

 Text - Tooltip - Basic : Hero Chooser

 Text - Tooltip - Extended: Randomise your hero!
Make sure the "Stats - Stock start delay" is 0

Finally, add your "wisp" (a.k.a Hero Chooser) to your Tavern. Note that the tavern can only have 11 heroes now. If you use more heroes, add them to another tavern.

The Trigger Data:

We will make a trigger that runs whenever you "buy" a "Hero Chooser" unit.

Events
Unit - A unit Sells a unit

Conditions
(Unit-type of (Sold unit)) Equal to Hero Chooser

In the actions, we will be REMOVING the "Hero Chooser" unit because ofcourse we do not wish to play with a wisp.

Next, we will create a random hero.

Ofcourse we NEED to check first if you already bought a hero before, so we will create an IF/THEN/ELSE action.

If you have not chosen a hero yet, you will do the "Then - Actions". In those actions we create a "Tavern1Heroes" (those you set up at the "Tavern Initialization" trigger at the start of the tutorial). In the array I'll put a RANDOM number between 1 and 4, because I have 4 heroes (the paladin, mountain king, archmage and bloodmage). If I have for example 6 heroes, I will but a RANDOM number between 1 and 6.

This is how the trigger looks like:

Actions
Unit - Remove (Sold unit) from the game

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Herochosenbyplayer[(Player number of (Owner of (Sold unit)))] Equal to False

Then - Actions
Unit - Create 1 Tavern1Heroes[(Random integer number between 1 and 4)] for (Owner of (Sold unit)) at (Center of HeroSpawn <gen>) facing Default building facing degrees

Set Herochosenbyplayer[(Player number of (Owner of (Sold unit)))] = True

Else - Actions
Do nothing

Voila, that's it. Next in the tutorial? Repicking your hero.

Part 3: Repicking your hero.

In many games (such as DotA, I often refer to this not because I like it, but because everyone knows this map, at least by name) you can repick your hero after choosing it. Usually it randomises your hero. I will, for once, not randomise your hero and allow you to independantly choose a new hero.

The Map Data

We'll need another region centered on the Tavern. I'll call it "Tavern"

The Trigger Data:

We will need some more variables.

NameTypeInitial ValueArray

PlayerRepickedBooleanFalse12

TempUnitGroupUnit Groupemptynone

Ok, so we want to be able to REPICK your hero. I'll make a trigger that repicks your hero when any player writes "-repick". Ofcourse, I will add conditions so ou cannot "repick" when you have not chosen a hero yet or when you ALREADY repicked.

Events
Player - Player 1 (Red) types a chat message containing -repick as An exact match

Player - Player 2 (Blue) types a chat message containing -repick as An exact match

Player - Player 3 (Teal) types a chat message containing -repick as An exact match

Player - Player 4 (Purple) types a chat message containing -repick as An exact match

Player - Player 5 (Yellow) types a chat message containing -repick as An exact match

Player - Player 6 (Orange) types a chat message containing -repick as An exact match

Player - Player 7 (Green) types a chat message containing -repick as An exact match

Player - Player 8 (Pink) types a chat message containing -repick as An exact match

Player - Player 9 (Gray) types a chat message containing -repick as An exact match

Player - Player 10 (Light Blue) types a chat message containing -repick as An exact match

Player - Player 11 (Dark Green) types a chat message containing -repick as An exact match

Player - Player 12 (Brown) types a chat message containing -repick as An exact match

Conditions
And - All (Conditions) are true
Conditions
Herochosenbyplayer[(Player number of (Triggering player))] Equal to True

PlayerRepicked[(Player number of (Triggering player))] Equal to False

I will use the following action

Actions
Set TempUnitGroup = (Units owned by (Triggering player) matching (((Matching unit) is A Hero) Equal to True))

Unit Group - Pick every unit in TempUnitGroup and do (Actions)
Loop - Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
Player - Make (Unit-type of (Picked unit)) Available for training/construction by (Picked player)

Unit - Remove (Picked unit) from the game

Custom script: call DestroyGroup(udg_TempUnitGroup)

so every player will be allowed to pick the hero you just removed.

The custom script is required for removing a memory leak.

Next, your hero should be removed from the game and you should be able to pick a new one.

Actions
Game - Display to (All players) the text: ((Name of (Triggering player)) + has repicked a hero!)

Set Herochosenbyplayer[(Player number of (Owner of (Sold unit)))] = False

Selection - Clear selection for (Triggering player)

Selection - Add Tavern 0000 <gen> to selection for (Triggering player)

Camera - Pan camera for (Triggering player) to (Center of Tavern <gen>) over 1.00 seconds

-------- If you only allow 1 repick per player, keep the action below --------

Set PlayerRepicked[(Player number of (Triggering player))] = True

-------- If you allow unlimited repicks per player, remove the action above --------

In total, you should have the following trigger:

Tavern Hero Repick
Events
Player - Player 1 (Red) types a chat message containing -repick as An exact match

Player - Player 2 (Blue) types a chat message containing -repick as An exact match

Player - Player 3 (Teal) types a chat message containing -repick as An exact match

Player - Player 4 (Purple) types a chat message containing -repick as An exact match

Player - Player 5 (Yellow) types a chat message containing -repick as An exact match

Player - Player 6 (Orange) types a chat message containing -repick as An exact match

Player - Player 7 (Green) types a chat message containing -repick as An exact match

Player - Player 8 (Pink) types a chat message containing -repick as An exact match

Player - Player 9 (Gray) types a chat message containing -repick as An exact match

Player - Player 10 (Light Blue) types a chat message containing -repick as An exact match

Player - Player 11 (Dark Green) types a chat message containing -repick as An exact match

Player - Player 12 (Brown) types a chat message containing -repick as An exact match

Conditions
And - All (Conditions) are true
Conditions
Herochosenbyplayer[(Player number of (Triggering player))] Equal to True

PlayerRepicked[(Player number of (Triggering player))] Equal to False

Actions
Set TempUnitGroup = (Units owned by (Triggering player) matching (((Matching unit) is A Hero) Equal to True))

Unit Group - Pick every unit in TempUnitGroup and do (Actions)
Loop - Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
Player - Make (Unit-type of (Picked unit)) Available for training/construction by (Picked player)

Unit - Remove (Picked unit) from the game

Custom script: call DestroyGroup(udg_TempUnitGroup)

Game - Display to (All players) the text: ((Name of (Triggering player)) + has repicked a hero!)

Set Herochosenbyplayer[(Player number of (Owner of (Sold unit)))] = False

Selection - Clear selection for (Triggering player)

Selection - Add Tavern 0000 <gen> to selection for (Triggering player)

Camera - Pan camera for (Triggering player) to (Center of Tavern <gen>) over 1.00 seconds

-------- If you only allow 1 repick per player, keep the action below --------

Set PlayerRepicked[(Player number of (Triggering player))] = True

-------- If you allow unlimited repicks per player, remove the action above --------

Now, there is nothing as annoying as someone who repicks his hero in the middle of the game. So if we want to prohibit players from repicking your hero after e.g. 45 seconds, we add the following trigger:

Tavern Hero Repick disable
Events
Time - Elapsed game time is 45.00 seconds

Conditions

Actions
Trigger - Turn off Tavern Hero Repick <gen>

Trigger - Turn off (This trigger)

That's it, I have said everything about hero repicking in Taverns.

If you have any questions or you'd like to add something to this tutorial, feel free to post.

Circle of Power Heroes

In many RPG's you enter an area with many heroes standing on Circles of Power. Whenever a unit (usually a wisp) enters this Circle of Power, you gain your hero.

In my example I'll use the 4 Orc heroes.

The Map Data:

4 Circles of Power with 4 Orc heroes. 1 wisp in the middle of them.

1 region (HeroSpawn) where our heroes will spawn.

The Object Data:

I'll edit the wisp so it's no longer a wisp but just a unit used for selecting the hero.

The following values were changed:

 Abilities - Normal - none

 Techtree - structures build - nothing

 Text - Name : Hero Chooser

The Trigger Data:

Note: the following trigger needs to be made for EVERY selectable hero. In other words: I'll end up with 4 triggers, 1 for each hero. As they're very similar to each others, I'll only do this for 1 hero in the tutorial:

Events
Unit - A unit comes within 100.00 of Blademaster 0012 <gen>

Conditions
(Unit-type of (Triggering unit)) Equal to Hero Chooser

This trigger will run whenever the "Hero Chooser" (our edited wisp) comes close to the Blademaster.

So what do we want this trigger to do? We want to create a Blademaster for the owner of the Hero Chooser.

Actions
Unit - Create 1 Blademaster for (Owner of (Triggering unit)) at (Center of HeroSpawn <gen>) facing Default building facing degrees

This creates the Blademaster.

Unit - Remove (Triggering unit) from the game
This removes the Hero Chooser from the game. We don't want him to select another hero again.

Camera - Pan camera for (Owner of (Triggering unit)) to (Center of HeroSpawn <gen>) over 1.00 seconds
This pans the camera on your new Blademaster.

Trigger - Turn off (This trigger)
We turn off the trigger so the Blademaster cannot be chosen by another player.

So this is how my final trigger looks like:

CoP Blademaster
Events
Unit - A unit comes within 100.00 of Blademaster 0012 <gen>

Conditions
(Unit-type of (Triggering unit)) Equal to Hero Chooser

Actions
Unit - Create 1 Blademaster for (Owner of (Triggering unit)) at (Center of HeroSpawn <gen>) facing Default building facing degrees

Unit - Remove (Triggering unit) from the game

Camera - Pan camera for (Owner of (Triggering unit)) to (Center of HeroSpawn <gen>) over 1.00 seconds

Trigger - Turn off (This trigger)

This needs to be repeated for every hero in the game.

That's it.

Dialog Menu

In some RPG's, a dialog pops up at startup. In this dialog you have various options to choose your hero.

A Dialog can have unlimitted buttons, but you'll only see about max. 8 a the same time on 1 screen. Therefor, it's not recommended to use this system with many heroes. However, we can make a dialog with MULTIPLE pages. If you have many heroes, I suggest you read this tutorial and read continue on the "Extended Dialog" tutorial. However, if you're only using about 6 heroes in your map, this tutorial is what you're looking for.

Please note that you can test this dialog system in the attached map by typing "-dialog". As this is a testing map, it would've been annoying if it showed up at startup.

Recommended: Basic Triggering Knowledge. If you know anything of dialogs, this tutorial might even be unnecessary for you.

The Map Data:

I need a region named "HeroSpawn". My hero will spawn here after selecting it.

The Trigger Data:

Following Variables are used:

NameTypeInitial ValueArray

DialogDialognonenone

DialogDeathKnightDialog Buttonnonenone

DialogDreadlordDialog Buttonnonenone

DialogLichDialog Buttonnonenone

DialogCryptLordDialog Buttonnonenone

In this example I use 4 heroes again. For every hero, you'll need to create a variable such as the "DialogDeathKnight" variable.

You can open the Variable Editor by going to the trigger editor and pressing "Cntrl + B" or alternativelly going to "Edit, Variables".

We will need to create a new folder in the trigger editor. I name mine "Dialog Heroes".

The first trigger will be called "Dialog Initialization"

The event is obvious:

Events
Game - Elapsed game time is equal to 0.5 seconds

The actions:

Dialog - Change the title of Dialog to Select your hero
This will change the title of your dialog into "Select your hero".

Dialog - Create a dialog button for Dialog labelled Death Knight

Set DialogDeathKnight = (Last created dialog Button)
Through this action, you add a new dialog button to your dialog, and you give it a name. I named mine "Death Knight".

In order to recognise this button, I need to add the second action (Set DialogDeathKnight = (Last created dialog Button)).

For every hero I have I must repeat those 2 actions.

Finally, I must show this dialog to every player, this will be done in the one but last action.

As I have 4 Heroes, this is how my trigger looks like:

Dialog Initialization
Events
Player - Player 1 (Red) types a chat message containing -dialog as An exact match

Conditions

Actions
Dialog - Change the title of Dialog to Select your hero

Dialog - Create a dialog button for Dialog labelled Death Knight

Set DialogDeathKnight = (Last created dialog Button)

Dialog - Create a dialog button for Dialog labelled Dreadlord

Set DialogDreadLord = (Last created dialog Button)

Dialog - Create a dialog button for Dialog labelled Lich

Set DialogLich = (Last created dialog Button)

Dialog - Create a dialog button for Dialog labelled Crypt Lord

Set DialogCryptLord = (Last created dialog Button)

Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
Dialog - Show Dialog for (Picked player)

Trigger - Turn off (This trigger)

Right now I have made a dialog that works. However, nothing will happen when I press one of the buttons.

In order to make a button work, I need to create the following trigger:

Dialog Death Knight
Events
Dialog - A dialog button is clicked for Dialog

Conditions
(Clicked dialog button) Equal to DialogDeathKnight

Actions
Unit - Create 1 Death Knight for (Triggering player) at (Center of HeroSpawn <gen>) facing Default building facing degrees

Camera - Pan camera for (Triggering player) to (Center of HeroSpawn <gen>) over 1.00 seconds

The previous trigger will create a Death Knight when you press the "Death Knight" button on the dialog. I must copy this trigger for every hero I have added to the dialog.

In fact, this is all I have to do.

Check the attached map for the triggers in detail. As said at the start of this tutorial, you can test this dialog by typing "-dialog".

Extended Dialog Menu

This is a dialog that takes place over multiple pages, allowing you to see many options on 1 screen.

In my example, I'll allow you to choose between one of the 4 undeath heroes or 1 of the 4 NE heroes.

It's recommended that you read the previous tutorial too.

The Map Data:

I'll need a region called "HeroSpawn". In here, my chosen hero will spawn.

The Trigger Data:

Following Variables were used:

NameTypeInitial ValueArray

EDialog1Dialognonenone

EDialog2Dialognonenone

DialogDeathKnightDialog Buttonnonenone

DialogDreadlordDialog Buttonnonenone

DialogLichDialog Buttonnonenone

DialogCryptLordDialog Buttonnonenone

DialogPriestessDialog Buttonnonenone

DialogDruidDialog Buttonnonenone

DialogWardenDialog Buttonnonenone

DialogDemonHunterDialog Buttonnonenone

DialogPageNEDialog Buttonnonenone

DialogPageUDDialog Buttonnonenone

Extended Dialog Initialization
Events
Player - Player 1 (Red) types a chat message containing -extendeddialog as An exact match

Conditions

Actions
-------- Initializing Undeath Page --------

Dialog - Change the title of EDialog1 to Select Undeath Hero...

Dialog - Create a dialog button for EDialog1 labelled Death Knight

Set DialogDeathKnight = (Last created dialog Button)

Dialog - Create a dialog button for EDialog1 labelled Dreadlord

Set DialogDreadLord = (Last created dialog Button)

Dialog - Create a dialog button for EDialog1 labelled Lich

Set DialogLich = (Last created dialog Button)

Dialog - Create a dialog button for EDialog1 labelled Crypt Lord

Set DialogCryptLord = (Last created dialog Button)

Dialog - Create a dialog button for EDialog1 labelled Next Page

Set DialogPageNE = (Last created dialog Button)

-------- Initializing Night Elf Page --------

Dialog - Change the title of EDialog2 to Select Night Elf He...

Dialog - Create a dialog button for EDialog2 labelled Priestess of the Moon

Set DialogPriestess = (Last created dialog Button)

Dialog - Create a dialog button for EDialog2 labelled Druid of the Grove

Set DialogDruid = (Last created dialog Button)

Dialog - Create a dialog button for EDialog2 labelled Demon Hunter

Set DialogDemonHunter = (Last created dialog Button)

Dialog - Create a dialog button for EDialog2 labelled Warden

Set DialogWarden = (Last created dialog Button)

Dialog - Create a dialog button for EDialog2 labelled Previous Page

Set DialogPageUD = (Last created dialog Button)

-------- Showing the Dialog --------

Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
Dialog - Show EDialog1 for (Picked player)

Trigger - Turn off (This trigger)

This trigger is similar to the Dialog Initialization in the previous tutorial.

However, there are some differences: In this trigger I create TWO dialogs instead of one, each with 4 buttons. At the end of the trigger, I show the FIRST dialog of the 2 I've made.

Note that for every page you want your dialog to have, you actually need to create a new dialog variable and new actions in this trigger.

There is another change too:

Actions
Dialog - Create a dialog button for EDialog1 labelled Next Page

Set DialogPageNE = (Last created dialog Button)

Actions
Dialog - Create a dialog button for EDialog2 labelled Previous Page

Set DialogPageUD = (Last created dialog Button)

As you can see, an additional button is added to the dialog. It's named "Next Page" or "Previous Page". This will be explained later.

For Each hero in both dialogs, you need to make a new trigger (just like the ones made in the previous tutorial). In my example I use the Death Knight.

Extended Dialog Death Knight
Events
Dialog - A dialog button is clicked for EDialog1

Conditions
(Clicked dialog button) Equal to DialogDeathKnight

Actions
Unit - Create 1 Death Knight for (Triggering player) at (Center of HeroSpawn <gen>) facing Default building facing degrees

Camera - Pan camera for (Triggering player) to (Center of HeroSpawn <gen>) over 1.00 seconds

Remember: You NEED this trigger for every hero that is mentioned in the 2 dialogs. As I have 8 heroes (4 for every dialog page), I'll need 8 triggers, similar to this one.

Now, let's go back to the "Next Page" and "Previous Page" button. Right now it doesn't work yet.

What we'll need to do is make a trigger that runs whenever you click the "previous page" or "next page" button. The event & Condition is similar to the above trigger.

Extended Dialog UD
Events
Dialog - A dialog button is clicked for EDialog2

Conditions
(Clicked dialog button) Equal to DialogPageUD

Actions
Dialog - Show EDialog1 for (Triggering player)

In the actions, I'll tell the trigger to show one of the 2 dialogs. In other words: when I click "Previous Page", I'll go back to the Undeath page.

Extended Dialog NE
Events
Dialog - A dialog button is clicked for EDialog1

Conditions
(Clicked dialog button) Equal to DialogPageNE

Actions
Dialog - Show EDialog2 for (Triggering player)

Thanks to this 2nd trigger, when I click "Next Page", I'll open the NE page.

That's it. I've explained how to browse through multiple pages of a dialog system.

Dialogs are quite powerful. While I have explained how to make a hero selection system, you can also use the Dialog system for other things, such as selecting your race, selecting what weapon you wish to equip, etc. etc.

Clicking/Doubleclicking.

Although this is not a very popular method, it's quite easy to implement and use.

Basically, this is how the system works: In front of you are a few heroes. By clicking on them a description pops up, and by double clicking you select the hero.

In my example I'll use the 8 neutral heroes.

The Map Data:

I'll need to create 8 neutral heroes (close to each others) somewhere on the map. Next, I must make sure that you can select them. Therefor, I must make sure you can actually SEE the 8 heroes. To do this, one of my units needs to be near those heroes. In my example, I'll simply create a sentry ward near them.

Next, I'll need a region where my hero will spawn. I call it "HeroSpawn".

The Trigger Data:

These are the variables used:

NameTypeInitial ValueArray

Selectedunitunit-typeno unit12

SelectionHeroSelectedBooleanFalse12

We'll only need ONE trigger.

Basically, we need to check if a unit is selected. However, this trigger can only run when you have NOT chosen a hero yet and when the selected unit is one of the 8 neutral heroes I've placed on the map.

Events
Player - Player 1 (Red) Selects a unit

Player - Player 2 (Blue) Selects a unit

Conditions
And - All (Conditions) are true
Conditions
SelectionHeroselected[(Player number of (Triggering player))] Equal to False

Or - Any (Conditions) are true
Conditions
(Triggering unit) Equal to Alchemist 0019 <gen>

(Triggering unit) Equal to Tinker 0020 <gen>

(Triggering unit) Equal to Naga Sea Witch 0021 <gen>

(Triggering unit) Equal to Beastmaster 0022 <gen>

(Triggering unit) Equal to Dark Ranger 0026 <gen>

(Triggering unit) Equal to Firelord 0027 <gen>

(Triggering unit) Equal to Pandaren Brewmaster 0028 <gen>

(Triggering unit) Equal to Pit Lord 0029 <gen>

SelectionHeroSelected is a Boolean variable. Whenever this variable becomes "True", you have selected a hero. In other words: we only want this trigger to run when you have not selected a hero yet.

In our actions we'll have to check if the player has already clicked the unit. If that's true, a hero should be created (as you double-clicked the hero). Else, a description will pop up

Actions
Selection - Clear selection for (Triggering player)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to SelectedUnit[(Player number of (Triggering player))]

Then - Actions
Unit - Create 1 (Unit-type of (Triggering unit)) for (Triggering player) at (Center of HeroSpawn <gen>) facing Default building facing degrees

Set SelectionHeroselected[(Player number of (Triggering player))] = True

Else - Actions
Set SelectedUnit[(Player number of (Triggering player))] = (Triggering unit)

Wait 1.50 seconds

Set SelectedUnit[(Player number of (Triggering player))] = No unit

As you can see, we have an "IF/THEN/ELSE" action. In the "IF - conditions" we check if the selected unit is the same unit as the LAST selected unit. If this is true, "Then - actions" will be executed. else, the "Else - actions" will be executed.

In the "Then - actions" we will create a unit of the SELECTED unit type. We also set "SelectionHeroselected" equal to true so you won't be able to select a 2nd hero.

In the "Else - actions" we will set the LAST selected unit (SelectedUnit[(Player number of (Triggering player))]) to the unit you just selected. This way, the next time you select a unit, the computer will remember what unit you selected the last time.

In the Else actions we'll also put the description of the selected unit. For example:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to Pandaren Brewmaster 0028 <gen>

Then - Actions
Game - Display to (Player group((Triggering player))) the text: The Pandaren Brewmaster...

Else - Actions
Do nothing

This is how my final trigger looks like:

Selection Heroes
Events
Player - Player 1 (Red) Selects a unit

Player - Player 2 (Blue) Selects a unit

Conditions
And - All (Conditions) are true
Conditions
SelectionHeroselected[(Player number of (Triggering player))] Equal to False

Or - Any (Conditions) are true
Conditions
(Triggering unit) Equal to Alchemist 0019 <gen>

(Triggering unit) Equal to Tinker 0020 <gen>

(Triggering unit) Equal to Naga Sea Witch 0021 <gen>

(Triggering unit) Equal to Beastmaster 0022 <gen>

(Triggering unit) Equal to Dark Ranger 0026 <gen>

(Triggering unit) Equal to Firelord 0027 <gen>

(Triggering unit) Equal to Pandaren Brewmaster 0028 <gen>

(Triggering unit) Equal to Pit Lord 0029 <gen>

Actions
Selection - Clear selection for (Triggering player)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to SelectedUnit[(Player number of (Triggering player))]

Then - Actions
Unit - Create 1 (Unit-type of (Triggering unit)) for (Triggering player) at (Center of HeroSpawn <gen>) facing Default building facing degrees

Set SelectionHeroselected[(Player number of (Triggering player))] = True

Else - Actions
Set SelectedUnit[(Player number of (Triggering player))] = (Triggering unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to Alchemist 0019 <gen>

Then - Actions
Game - Display to (Player group((Triggering player))) the text: The Alchemist: War...

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to Tinker 0020 <gen>

Then - Actions
Game - Display to (Player group((Triggering player))) the text: The Tinker: Cunnin...

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to Naga Sea Witch 0021 <gen>

Then - Actions
Game - Display to (Player group((Triggering player))) the text: The Sea Witch: Mys...

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to Beastmaster 0022 <gen>

Then - Actions
Game - Display to (Player group((Triggering player))) the text: The Beastmaster: W...

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to Dark Ranger 0026 <gen>

Then - Actions
Game - Display to (Player group((Triggering player))) the text: The Dark Ranger: C...

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to Firelord 0027 <gen>

Then - Actions
Game - Display to (Player group((Triggering player))) the text: The Firelord: Myst...

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to Pandaren Brewmaster 0028 <gen>

Then - Actions
Game - Display to (Player group((Triggering player))) the text: The Pandaren Brewma...

Else - Actions
Game - Display to (Player group((Triggering player))) the text: The Pit Lord: Warr...

Wait 1.50 seconds

Set SelectedUnit[(Player number of (Triggering player))] = No unit

That's it.

#### Attachments

Tutorial.w3x

 37.9 KB

 · Views: 1,927




 Last edited: Sep 2, 2008

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

Next/Previous Hero clicking


The following tutorial has been taken directly from Tonks.


Difficulty: Moderate


Note: This is not a good idea if you have more than 8-12 heroes.


This is not a very common method of hero selection. In fact, I've only ever seen it used in the Blizzard map Azeroth Grand Prix. I used it myself in one of my maps.


Essentially what this system entails is having a unit that has several abilities; Next, Previous, and an ability with the icon of the Hero. When you click Next, it locks your camera to the next hero in line, and when you click Previous it locks your camera to the previous hero. Then when you click the ability with the Hero's icon, it selects that hero.

Variables:


- HeroSelector - Unit Array Variable - Size 12


- HeroSelectAbil - Ability Array Variable - Size 12 (Initial Value set to your first hero)


- L - Point Variable

Abilities:


- You will need an ability for every hero. The ability should be named after the hero and should have the same icon as the hero.


- Next - Should be based off an ability easily modified to do nothing.


- Previous - Should be based off an ability easily modified to do nothing, but should not be based off the same ability as "Next."

Units:


- You will need one of each of your heroes (preferably in a line) owned by Neutral Passive.


- You will need one Hero Selector unit for each player. Should have the Abilities Next, Previous, and the ability corresponding to the first hero.

Triggers:

View Previous
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Previous

Actions

Actions
Unit - Remove HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] from (Triggering unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] Equal to Dwarves

Then - Actions
Set HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] = Panda

Camera - Lock camera target for (Owner of (Triggering unit)) to Panda 0040 <gen>, offset by (0.00, 0.00) using Default rotation

Unit - Add HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] to (Triggering unit)

Skip remaining actions

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] Equal to Clubz

Then - Actions
Set HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] = Dwarves

Camera - Lock camera target for (Owner of (Triggering unit)) to Dwarves 0037 <gen>, offset by (0.00, 0.00) using Default rotation

Unit - Add HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] to (Triggering unit)

Skip remaining actions

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] Equal to Muskles

Then - Actions
Set HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] = Clubz

Camera - Lock camera target for (Owner of (Triggering unit)) to Ice T 0036 <gen>, offset by (0.00, 0.00) using Default rotation

Unit - Add HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] to (Triggering unit)

Skip remaining actions

Else - Actions

Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Previous

Runs this trigger when an ability is cast, but only if it is the "Previous" Ability.

Unit - Remove HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] from (Triggering unit)
Removes the ability that is assigned to HeroSelectAbil for that player.

If - Conditions
HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] Equal to Dwarves

Then - Actions
Set HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] = Panda

Camera - Lock camera target for (Owner of (Triggering unit)) to Panda 0040 <gen>, offset by (0.00, 0.00) using Default rotation

Unit - Add HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] to (Triggering unit)

This checks if the HeroSelectAbil variable is set to the Dwarves ability. If it is, it sets it to the Panda ability, Locks the camera to the Panda unit, and adds the Panda ability to the unit.

Skip remaining actions
This is in there to make sure that it doesn't continue the function.


The rest of the trigger is repetitive, and does not need to be explained.

View Next
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Next

Actions
Unit - Remove HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] from (Triggering unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] Equal to Panda

Then - Actions
Set HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] = Dwarves

Camera - Lock camera target for (Owner of (Triggering unit)) to Dwarves 0037 <gen>, offset by (0.00, 0.00) using Default rotation

Unit - Add HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] to (Triggering unit)

Skip remaining actions

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] Equal to Dwarves

Then - Actions
Set HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] = Clubz

Camera - Lock camera target for (Owner of (Triggering unit)) to Ice T 0036 <gen>, offset by (0.00, 0.00) using Default rotation

Unit - Add HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] to (Triggering unit)

Skip remaining actions

Else - Actions

As you can tell, the Next trigger is just the same as the Previous trigger, in reverse.

Pick Cart
Events
Unit - A unit Starts the effect of an ability

Conditions
(Unit-type of (Triggering unit)) Equal to Hero Selection

Or - Any (Conditions) are true
Conditions
(Ability being cast) Equal to Panda

(Ability being cast) Equal to Bonez

(Ability being cast) Equal to Clubz

(Ability being cast) Equal to Dwarves

(Ability being cast) Equal to Fatty

(Ability being cast) Equal to Muskles

(Ability being cast) Equal to Priest

(Ability being cast) Equal to Revenant

(Ability being cast) Equal to Spirit

(Ability being cast) Equal to Tinker

(Ability being cast) Equal to Troll

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Bonez

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Bonez for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Clubz

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Ice T for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Dwarves

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Dwarves for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Fatty

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Fatty for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Muskles

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Muskles for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Panda

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Panda for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Priest

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Priest for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Revenant

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Revenant for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Spirit

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Toad for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Tinker

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Tinker for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Troll

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Troll for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

Camera - Apply Camera 002 <gen> for (Owner of (Triggering unit)) over 0.00 seconds

Unit - Remove (Triggering unit) from the game

This trigger creates the unit corresponding to the clicked ability.

Conditions
(Unit-type of (Triggering unit)) Equal to Hero Selection

Or - Any (Conditions) are true
Conditions
(Ability being cast) Equal to Panda

(Ability being cast) Equal to Bonez

(Ability being cast) Equal to Clubz

(Ability being cast) Equal to Dwarves

(Ability being cast) Equal to Fatty

(Ability being cast) Equal to Muskles

(Ability being cast) Equal to Priest

(Ability being cast) Equal to Revenant

(Ability being cast) Equal to Spirit

(Ability being cast) Equal to Tinker

(Ability being cast) Equal to Troll

Checks if the triggering unit is the Hero Selection unit and if the cast Ability is any of the abilities corresponding to the heroes.

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Bonez

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Bonez for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Checks if the ability being cast is the ability corresponding to the Bonez hero. If it is, it creates 1 Bonez at a random point in the Team1Spawn region.

Note: "Set L = ..." and "call RemoveLocation(udg_L)" are used to avoid a memory leak in this trigger.


Last but not least you'll want to have a trigger that sets the Hero Selectors.

HeroSelectors
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Set HeroSelector[1] = Hero Selection 0012 <gen>

Set HeroSelector[2] = Hero Selection 0013 <gen>

Set HeroSelector[3] = Hero Selection 0014 <gen>

Set HeroSelector[4] = Hero Selection 0015 <gen>

Set HeroSelector[5] = Hero Selection 0011 <gen>

Set HeroSelector[6] = Hero Selection 0017 <gen>

Set HeroSelector[7] = Hero Selection 0016 <gen>

Set HeroSelector[8] = Hero Selection 0020 <gen>

Set HeroSelector[9] = Hero Selection 0019 <gen>

Set HeroSelector[10] = Hero Selection 0018 <gen>

Set HeroSelector[11] = Hero Selection 0021 <gen>

Set HeroSelector[12] = Hero Selection 0022 <gen>

This sets the HeroSelector variables according to their arrays. "HeroSelector[1]" is set to Red's Hero Selector unit, "HeroSelector[2]" is set to Blue's, etc..


P.s. Note from Poster: I might rewrite this tutorial


Thanks for reading my tutorial!


 Last edited by a moderator: Sep 6, 2007

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Wow seems like a rather detailed tutorial, this is definitelly a good job. I had a look-through though I will review it whole once.

h4x, + RUPZ111ONE

EDIT: meh i cant giev now

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Hmm, deleak please (seems like you sometimes do, sometimes don't). Also, you accidentally did

[/code] on one of your trigger tags =/.

Oh, also, get rid of the Do Nothings, please.

Pretty thourough, though. Nice.

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

yeah I wanted to deleak everything but then I decided I had enough work already and it wouldn't leak alot anyway, but I'll fix that.

the

[/code] error wasn't made by me, but then again, I'll fix it.

why exactly should I get rid of the "do nothings"? I know it works without them, but I like to play safe...

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

 why exactly should I get rid of the "do nothings"? I know it works without them, but I like to play safe...


Click to expand...

They make the code a bit slower (heck, they take more processing time than Sin, but then again almost every GUI function does) and they do... nothing. They were originally intended (So it seems) to fill the 'else' (or the 'then') of an If/Then/Else (the kind without multiple conditions) that required something in both fields.

 yeah I wanted to deleak everything but then I decided I had enough work already and it wouldn't leak alot anyway, but I'll fix that.


Click to expand...

I don't think it's mandatory to (just preferrable, unless the leaks are really bad), so take your time if you want to. Or, you can do something like wyrmlord then, and not remove any leaks but link to a tutorial explaining about them, thus not going off the focus of the topic (getting people scratching their heads over RemoveLocations and stuff), and yet still providing means to deal with the problem. However, in that case, you should probably never remove the leaks, or that method is sort of pointless.

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved!

~ Thread moved to Trigger (GUI) Editor Tutorials.

#### shadow-twin

#### shadow-twin

 Level 2

JoinedSep 18, 2007Messages21

wow...  nice tutorial and very detailed but im sure it will work great to many maps also my own

#### GhostWolf

#### GhostWolf

 Level 29

JoinedJul 29, 2007Messages5,174

Resources: 2

#### Resources
Tools 1Tutorials 1

There are better ways to make some of the stuff here (for example, you can use a arrayed string variable instead of each for each hero, a second example would be the changing directions, its easier to make when a player clicks the left/right/up/down arrows), but very nice and detailed.

#### Traxamillion

#### Traxamillion

 Level 19

JoinedSep 14, 2007Messages1,527

Resources: 10

#### Resources
Models 2Icons 7Tutorials 1

thanks this helped a bunch but what if my hero's general name is changed...for example if i changed blademasters name to wrenchmaster do i add that in..in place of blademaster?

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

Yes. I am too smart for you - need 15 characters :/

#### Phillejh

P

#### Phillejh

Hey thanks for this it have helped me, but i need help. I cant seem to find theese Triggers :


Set Tavern1Heroes[1] = Paladin (Tavern)


Set Tavern1Heroes[2] = Archmage (Tavern)


Set Tavern1Heroes[3] = Mountain King (Tavern)


Set Tavern1Heroes[4] = Blood Mage (Tavern)


In Tavern Initialization. Please help me with this. Yeah im noobish but still .

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

You can simply check the attached map and look at the trigger if you need help with it.

Anyway, the action is called "Set Variable". The variable you need to create is a unit type array called Tavern1Heroes.

#### FF7 project-G

#### FF7 project-G

 Level 11

JoinedMay 1, 2008Messages655

Resources: 6

#### Resources
Models 1Icons 2Tutorials 3

The click/double click

Else - Actions

 Set SelectionHeroSelected[(Player number of (Triggering player))] = ((Triggering unit) is selected by (Triggering player))

 Wait 1.50 seconds

 Set SelectedUnit[(Player number of (Triggering player))] = No unit

how do i get the Set SelectionHeroSelected[(Player number of (Triggering player))] = ((Triggering unit) is selected by (Triggering player)) to be just triggering unit behind the =?

send a pm/email


 Last edited: May 3, 2008

#### cHeRoL

#### cHeRoL

 Level 11

JoinedMar 30, 2008Messages650

Resources: 5

#### Resources
Models 1Maps 2Spells and Systems 2

-repick is not working on my map

Oh no its working,sorry

Thanks for this trigger,i will +rep you


 Last edited: May 31, 2008

#### timster

T

#### timster

Help plz

hi i can fix the


"Unit - Remove (Triggering unit) from the game" plz help

#### SoundHyp

S

#### SoundHyp

Rep given, I think this is exactly what I'm looking for, And if not, Gives me lots of choices. Thank you very much

#### penguinperson

#### penguinperson

 Level 2

JoinedApr 24, 2008Messages29

Very nice tutorial I did not read it word for word but i did notice some LEAKS other than that it's great. 4/5

+rep

#### PlayersFear

#### PlayersFear

 Level 3

JoinedDec 24, 2008Messages16

Resources: 1

#### Resources
Tutorials 1

Thanks SO much for the Dialog selection thingy!

It's JUST what I wanted, now if only someone would take the time to post a save/load code trigger tutorial, I haven't seen ANY.

Thank you though, this will help me move through my map process a whole lot faster, even if it is not a popular map, I hope it will be. I'm making it difficult, but easy enough to be player friendly

Support my map when it comes out please

My WC3 Name is PlayersFear, I play in U.S.West and U.S.East (mostly East)

Thanks Again (3rd time )

I'ma go test it out now

____________________________________________________________________________

Edit:

Thank you SOOOOO much, it works (well, of course it does ), you're the best!


 Last edited: Dec 24, 2008

#### PlayersFear

#### PlayersFear

 Level 3

JoinedDec 24, 2008Messages16

Resources: 1

#### Resources
Tutorials 1

Opps, almost double posted, srry.

Thanks again


 Last edited: Dec 24, 2008

#### Metralex

#### Metralex

 Level 7

JoinedApr 3, 2009Messages83

Resources: 6

#### Resources
Models 4Icons 1Skins 1

very helpfull tutorial.


I'll use any of them in my little project.


+rep

#### Reinze

R

#### Reinze

Nice tutorial......


very useful.....


But, can u tell me how to make all other unit than the selected get removed from the tavern for the owner of (sold unit).. coz it's still stuck in that tavern...


Woops.. almost forgot!!! +rep

#### thereallywhitekid

#### thereallywhitekid

 Level 6

JoinedNov 18, 2008Messages119

Resources: 2

#### Resources
Spells and Systems 1Tutorials 1

Man, you are just so awesome!


I totally needed this tutorial,


especially when I was making


my map before I gave up on it.


Thanks anyways. Just like


everyone else, I'll give you a

+rep!

#### Dr.Who_4400_GR

#### Dr.Who_4400_GR

 Level 3

JoinedAug 3, 2008Messages15

Resources: 3

#### Resources
Maps 3

I learn Double Clicking Choose System System. You rock 5/5

#### Panda-Assassin

#### Panda-Assassin

 Level 2

JoinedJun 21, 2009Messages11

is there a demo map for this?? i'm quite new to this

#### Xemnas

#### Xemnas

 Level 4

JoinedMar 30, 2008Messages52

Resources: 3

#### Resources
Icons 3

Hey thanks for this! i added the circle pick and works perfect

#### ChaosKnight

#### ChaosKnight

 Level 6

JoinedJul 18, 2009Messages199

Resources: 1

#### Resources
Spells 1

Can you post me a little tutorial how to do a custom spell?


BTW, it was VERY VERY GOOD!!!!!

#### omnislash_1992

O

#### omnislash_1992

Question

Vey good Tutorial but I search for a trigger that allow user (computer user) choose a random hero from tavern automatically after 35 seconds (without typing any chat massage)


if you know this please write this trigger for me in this page!


Thanks

#### Tharky

#### Tharky

 Level 4

JoinedMay 3, 2009Messages69

Resources: 2

#### Resources
Icons 2

Eleandor said:

Next/Previous Hero clicking


The following tutorial has been taken directly from Tonks.


Click to expand...


There is another system with no next,previous buttons. You select your hero with clicking its icon and camera locks your hero's cam.

#### Addicted07

#### Addicted07

 Level 6

JoinedNov 14, 2007Messages155

Resources: 2

#### Resources
Maps 1Spells 1

THanks for the Tavern Tutorial! but they seem to spawn at the same location whether it is yours or the enemy's hero... how can I change it?

#### Xx Rome xX

#### Xx Rome xX

 Level 3

JoinedSep 26, 2009Messages53

Excellent Tutorial

This tutorial is one of the best I have ever seen. If I were a mod, and it was up to me wether this was aproved or not, I wouldn't hesitate. +rep

#### SomeNightElf

S

#### SomeNightElf

Just to save everyones buts, you can at least put:


 Name: DialogButton


 Type: Dialog Button


 Array Size:[Number of heroes you want]


Your tutorial is perfect, but its realy complicated, just like me and spelling-47.50 grade


Edit:Frogot to give +rep

#### Hellmane357

H

#### Hellmane357

Help needed!

I Really just started World editor and tried to do the Hero Tavern and im stuck here.


I got no idea how to do this:


(Selling unit) Equal to Tavern 0000 <gen>


And for the hero part:


Set Tavern1Heroes[1] = Paladin (Tavern)


Set Tavern1Heroes[2] = Archmage (Tavern)


Set Tavern1Heroes[3] = Mountain King (Tavern)


Set Tavern1Heroes[4] = Blood Mage (Tavern)


I dont seem to have the (Tavern) word behind it. i only got the name.


Really need help, totally new to this.


Thanks alot

#### Mog_Noob

#### Mog_Noob

 Level 2

JoinedDec 23, 2009Messages15

Okay, i used dialogs for my hero picking i have a random bottom but ive tested it always picked the same unit can you plz put the trigger layout for a random hero pick dialog botton plzz ^_^

#### LunarLite

#### LunarLite

 Level 8

JoinedApr 8, 2009Messages499

Good job

 nice coverage of alot of possible hero selections, i guess i should have followed the tutorial earlier because i was doing all kind of weird stuff to make my hero selection operateble....

(i made a power circle buy a certain item and depending on item bought it would create a unit and delete buying unit.. pretty much the same as your tavern except its with items ^^ )

+rep given 5/5


 Last edited: Jun 7, 2010

#### karlk32

#### karlk32

 Level 2

JoinedMay 20, 2010Messages15

Resources: 2

#### Resources
Maps 2

Can someone tell me how to clear away that freakin start Town hall/stronghold/tree of life/ necropolis????

#### Gopa

G

#### Gopa

Extended Dialog Menu

first i have to say... realy great and nice tutorial!


i start working with the worldeditor/triggering this week and i can follow your tutorial quite easy & everything is clear  (well i read other basic tutorials first... like how to start triggers ect. but i think thats clear)

Good Job!!! & get it up


but i find a bug in the extended dialog menu, maybe someone could help me with that ?


if you test the extended dialog menu -> click on "Next Page", everything should be fine atm... but if you klick now on "Previous Page" the titel dialog of "EDialog1" will disappear. in this order now also the titel of "EDialog2" will disappear if you klick agan on "Next Page"!


i used some more stages like edialog1-8, probably all the titels disappear if i go back to the "main-stage"... i used the titels for herodiscripion & it quit sucks that i can only read them one time...


hope someone can help me with my problem


iam quit a noob... pls remember that

#### Bloodsplintzy

#### Bloodsplintzy

 Level 4

JoinedMar 18, 2010Messages66

Resources: 6

#### Resources
Icons 6

Could someone please make a Next/Previous system for me, can't seem to figure it out, allways bugg's for me ):

#### knight_godryu

#### knight_godryu

 Level 6

JoinedMay 20, 2010Messages274

Hi also want to say that i cant find the circle of power...i can only find it if im in neutral passive and not in any player (color) pls tell me why

#### knight_godryu

#### knight_godryu

 Level 6

JoinedMay 20, 2010Messages274

Hellmane357 said:

 I Really just started World editor and tried to do the Hero Tavern and im stuck here.


I got no idea how to do this:


(Selling unit) Equal to Tavern 0000 <gen>


And for the hero part:


Set Tavern1Heroes[1] = Paladin (Tavern)


Set Tavern1Heroes[2] = Archmage (Tavern)


Set Tavern1Heroes[3] = Mountain King (Tavern)


Set Tavern1Heroes[4] = Blood Mage (Tavern)


I dont seem to have the (Tavern) word behind it. i only got the name.


Really need help, totally new to this.


Thanks alot

Click to expand...


thats quite simple i think...all you need to do (well i guess so this is what i did) is change the name and add (tavern) to the names of the heroes

#### sentrywiz

S

#### sentrywiz

Great and simple to implement double click system. +rep


You need to change one thing though:


SelectUnit is not unit-type variable, but unit variable. I'm sure you meant that but still

#### Bond009

#### Bond009

 Level 13

JoinedMar 24, 2010Messages950

Resources: 3

#### Resources
Maps 3

nice, would be cool if he showed a screen shot of the menus he made though, just for show

#### Amargaard

#### Amargaard

 Level 19

JoinedJun 29, 2009Messages6,759

Resources: 16

#### Resources
Icons 16

Can you guys help me?

#### wickedcool

#### wickedcool

 Level 6

JoinedMay 9, 2011Messages228

Thanks for the Awesome Guide +rep


 Last edited: May 26, 2011

#### frostloh

#### frostloh

 Level 7

JoinedJan 9, 2010Messages339

Being the noob that I am at triggering, I can't find the

Herochosenbyplayer[(Player number of (Owner of (Sold unit)))] Equal to True
trigger. Can anyone help?

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

In doubleclick hero selection, it's wiser to store heroes in variables and then use Integer A if/then/else conditions, instead of wall of useless I.T.E. conditions.

#### criferios

C

#### criferios

can i get Herochosenbyplayer[(Player number of (Owner of (Sold unit)))] Equal to False ?

#### Skunkmunken

#### Skunkmunken

 Level 1

JoinedDec 12, 2018Messages3

Tavern Initialization

Events

Map initialization

Conditions

Actions

Set Tavern1Heroes[1] = Paladin (Tavern)

Set Tavern1Heroes[2] = Archmage (Tavern)

Set Tavern1Heroes[3] = Mountain King (Tavern)

Set Tavern1Heroes[4] = Blood Mage (Tavern)

This doesn't work, something is missing from the tutorial. I've got my tavern set up with a hero that is buyable (testet ingame), and I got the variable Tavern1Heroes right, but I can't point to a hero. When I open up the tutorial map it's there, but when I open my own map the option to pick (fx.) Paladin is just simply not there. What gives?

#### xoachimtachi

#### xoachimtachi

 Level 1

JoinedNov 15, 2019Messages6

Xin lỗi tôi đã đăng bài viết này nhưng thành thật tôi đang mong đợi các PRO chỉ có thể giúp đỡ với WE, nó không hoạt động cho người chơi 2- người chơi9, xin vui lòng cho tôi biết vấn đề này, cảm ơn mọi người đã lắng nghe! !!

#### Attachments

1.jpg

 111.9 KB

 · Views: 755



2.jpg

 133.5 KB

 · Views: 717



3.jpg

 121.1 KB

 · Views: 739



4.jpg

 138.4 KB

 · Views: 707



5.jpg

 121.5 KB

 · Views: 735



6.jpg

 140.1 KB

 · Views: 723



#### LogusMaximus

#### LogusMaximus

 Level 3

JoinedFeb 22, 2020Messages43

Not sure if this board is still active or not,but i am using the tavern and repick selections but am using about 9 different taverns. All the taverns work correctly but the repicking is what im having an issue with. When i go to repick, some taverns dont function correctly when i repick. I cant buy a champ from certain taverns. But i pick one from another then repick again and then the tavern that wasnt working starts working again but adiff one doesnt work. Also sometimes certain heroes are not selectable during repick. Any thoughts?

I feel it may have something to do with the selection triggers but not sure. Pic linked to post.

#### Attachments

Repick.PNG

 44.4 KB

 · Views: 571



 You must log in or register to reply here.


### Similar threads

 Hero Selection Systems


Tonks

Mar 23, 2007

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jun 8, 2023
dantles1992

 Altering Hero Stats For Noobs


Faith

May 1, 2013

Trigger (GUI) Editor Tutorials

Replies24Views11K

Oct 2, 2015
XxkosumaxX

 Dialog Buttons (Advanced Systems)


-Kobas-

Jan 3, 2011

Trigger (GUI) Editor Tutorials

Replies15Views24K

Jan 30, 2024
Njord

 GUI - Hero Click-Selection (Advanced)


Fulla

Oct 4, 2007

Trigger (GUI) Editor Tutorials

Replies38Views22K

Jul 23, 2008
Fulla

 Creating a Votekick System


1)ark_NiTe

Nov 8, 2007

Trigger (GUI) Editor Tutorials

Replies29Views19K

Nov 13, 2008
Tichondrius1523

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