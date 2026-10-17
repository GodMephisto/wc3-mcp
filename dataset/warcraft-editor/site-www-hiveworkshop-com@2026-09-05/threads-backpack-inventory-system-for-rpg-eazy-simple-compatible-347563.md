---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/backpack-inventory-system-for-rpg-eazy-simple-compatible.347563/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Backpack Inventory System for RPG Eazy Simple Compatible
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/backpack-inventory-system-for-rpg-eazy-simple-compatible.347563/>
#### Master Zul'khan

#### Master Zul'khan

 Level 8

JoinedApr 26, 2020Messages66

Resources: 1

#### Resources
Tutorials 1

Came up wit a neat Backpack system while ago. Since I saw it is a very asked system so I'm postin' it here.

My handy simple Backpack Inventory! Just in one pocket-size trigger using de backpack unit and its custom value.

Introduction

Most rpgs need extra storage space for them' player to collect items. Backpack system provides additional carry-along item storage for da Adventurer. On your journey u would open Backpack to grab/drop some items and put it back to move on. Our's is safe and light and shouldn't mess wit your smooth gameplay. Easily compatible.

- Backpack Unit

You'll need an invulnerable Backpack Unit created wit Inventory and Perma Invisibility abilities. Set the classifications of unit to "Worker, Ward"

Recommended: Hide minimap-display, Remove shadow image, Set all movement values to (0), Set maximum pitch and roll angle to (0.00) and change the unit model and icon as desired. (e.g. Item Chest model)

Now place the Backpack Units under your Player- Start Location somewhere in the map. Cause we can not use variables for selection event we gonna use preset units. Also remember to change the map's "Advanced> Game Interface> Icon- Idel Worker" to desired icon.

- Backpack Trigger

Events
Unit - Backpack 0001 <gen> Is selected

Unit - Backpack 0002 <gen> Is selected

Unit - Backpack 0003 <gen> Is selected

Unit - Backpack 0004 <gen> Is selected

Set the Unit is selected event for each Backpack preseted in the map, usin' tha "Specific Unit Event"

Note: Multiple events doesn't mean they should be cast all at once. It triggers whether any of the events fires.

Conditions
(Custom value of (Triggering unit)) Equal to 0

Settin' the Backpack's Custom Value to (1) means the backpack is open. So if (Condition) the already open backpack gets selected again, the trigger shall not run.

Custom Value (0) marks the backpack as currently out and closed.

Actions
Unit - Order Hero to Hold Position

Unit Group - Pick every unit in Backpacks and do (Unit - Move (Picked unit) instantly to (Player 3 (Teal) start location))

Unit Group - Pick every unit in Backpacks and do (Unit - Set the custom value of (Picked unit) to 0)

Stops the Hero from movement. Then moves any open Backpack back to it's location and sets their values to (0).

- ya should have set your backpacks to a Unit Group variable in Map Initialization:

Set Backpacks = (Units owned by Player 3 (Teal) of type Backpack Unit)
De action above does it.

Unit - Move (Triggering unit) instantly to ((Position of Hero) offset by 100.00 towards (Facing of Hero) degrees), facing ((Facing of Hero) - 180.00) degrees

Unit - Set the custom value of (Triggering unit) to 1
This actions continue the previous actions. It transfers the wanted backpack infront of Hero and sets its value to (1).

If ((Triggering unit) Equal to Backpack 0001 <gen>) then do (Game - Display to Player Group - Player 3 (Teal) for 0.50 seconds the text: |cffffcc00Backpack (1)|r is loaded.) else do (Do nothing)

If ((Triggering unit) Equal to Backpack 0002 <gen>) then do (Game - Display to Player Group - Player 3 (Teal) for 0.50 seconds the text: |cffffcc00Backpack (2)|r is loaded.) else do (Do nothing)

If ((Triggering unit) Equal to Backpack 0003 <gen>) then do (Game - Display to Player Group - Player 3 (Teal) for 0.50 seconds the text: |cffffcc00Backpack (3)|r is loaded.) else do (Do nothing)

If ((Triggering unit) Equal to Backpack 0004 <gen>) then do (Game - Display to Player Group - Player 3 (Teal) for 0.50 seconds the text: |cffffcc00Backpack (4)|r is loaded.) else do (Do nothing)
The above If Actions are all text messages to inform the Player whenever a Backpack is opened: (Optional / Can be used for debugging)

Wait until ((Current order of Hero) Not equal to (Order(hold position))), checking every 0.75 seconds

Unit - Move (Triggering unit) instantly to (Player 3 (Teal) start location)

Unit - Set the custom value of (Triggering unit) to 0

Selection - Remove (Triggering unit) from selection
Wait until the Hero moves away or begins a new order then return the Open Backpack to Player's Start Location and mark it as closed. (Custom Value = 0)

Also deselects tha Backpack automatically.

- How it works

By clickin' on the Backpack Inventory (aka Idle Worker) icon, de Backpack will be transfered to the location of your hero, infront of his feet. Then you can exchange items and when finished just run away and backpack will be moved back to your Player- Start Location.

If ya mon wanted to for example open Backpack (3) you should click the icon 3-times repeatly. Waitin' long between repeats will open up the Backpack (1) again.

Instead of clickin' on icon you can use the Idel Worker hotkey which I believe it is (~) or (F8) too.

The Full Trigger:

Backpack Inventory
Events
Unit - Backpack 0001 <gen> Is selected

Unit - Backpack 0002 <gen> Is selected

Unit - Backpack 0003 <gen> Is selected

Unit - Backpack 0004 <gen> Is selected

Conditions
(Custom value of (Triggering unit)) Equal to 0

Actions
Unit - Order Hero to Hold Position

Unit Group - Pick every unit in Backpacks and do (Unit - Move (Picked unit) instantly to (Player 3 (Teal) start location))

Unit Group - Pick every unit in Backpacks and do (Unit - Set the custom value of (Picked unit) to 0)

Unit - Move (Triggering unit) instantly to ((Position of Hero) offset by 100.00 towards (Facing of Hero) degrees), facing ((Facing of Hero) - 180.00) degrees

Unit - Set the custom value of (Triggering unit) to 1

If ((Triggering unit) Equal to Backpack 0001 <gen>) then do (Game - Display to Player Group - Player 3 (Teal) for 0.50 seconds the text: |cffffcc00Backpack (1)|r is loaded.) else do (Do nothing)

If ((Triggering unit) Equal to Backpack 0002 <gen>) then do (Game - Display to Player Group - Player 3 (Teal) for 0.50 seconds the text: |cffffcc00Backpack (2)|r is loaded.) else do (Do nothing)

If ((Triggering unit) Equal to Backpack 0003 <gen>) then do (Game - Display to Player Group - Player 3 (Teal) for 0.50 seconds the text: |cffffcc00Backpack (3)|r is loaded.) else do (Do nothing)

If ((Triggering unit) Equal to Backpack 0004 <gen>) then do (Game - Display to Player Group - Player 3 (Teal) for 0.50 seconds the text: |cffffcc00Backpack (4)|r is loaded.) else do (Do nothing)

Wait until ((Current order of Hero) Not equal to (Order(hold position))), checking every 0.75 seconds

Unit - Move (Triggering unit) instantly to (Player 3 (Teal) start location)

Unit - Set the custom value of (Triggering unit) to 0

Selection - Remove (Triggering unit) from selection

- Loading Backpacks To Another Map

For saving/loading form map to map just store all de Backpacks in tha Gama Cache one by one. Then load dem like this:

Actions
-------- Restore Backpacks --------

Game Cache - Restore Backpack[1] of Character from GameCache[0] for Neutral Passive at (Player 3 (Teal) start location) facing 0.00

For each (Integer A) from 1 to 6, do (Actions)
Loop - Actions
Hero - Give (Item carried by (Last restored unit) in slot (Integer A)) to Backpack 0001 <gen>

If ((Integer A) Equal to 6) then do (Unit - Remove (Last restored unit) from the game) else do (Do nothing)

Game Cache - Restore Backpack[2] of Character from GameCache[0] for Neutral Passive at (Player 3 (Teal) start location) facing 0.00

For each (Integer A) from 1 to 6, do (Actions)
Loop - Actions
Hero - Give (Item carried by (Last restored unit) in slot (Integer A)) to Backpack 0002 <gen>

If ((Integer A) Equal to 6) then do (Unit - Remove (Last restored unit) from the game) else do (Do nothing)

Game Cache - Restore Backpack[3] of Character from GameCache[0] for Neutral Passive at (Player 3 (Teal) start location) facing 0.00

For each (Integer A) from 1 to 6, do (Actions)
Loop - Actions
Hero - Give (Item carried by (Last restored unit) in slot (Integer A)) to Backpack 0003 <gen>

If ((Integer A) Equal to 6) then do (Unit - Remove (Last restored unit) from the game) else do (Do nothing)

Game Cache - Restore Backpack[4] of Character from GameCache[0] for Neutral Passive at (Player 3 (Teal) start location) facing 0.00

For each (Integer A) from 1 to 6, do (Actions)
Loop - Actions
Hero - Give (Item carried by (Last restored unit) in slot (Integer A)) to Backpack 0004 <gen>

If ((Integer A) Equal to 6) then do (Unit - Remove (Last restored unit) from the game) else do (Do nothing)

-------- * if a custom backpack upgrade is reaserched for player-3 then set owner of backpack0002/0003/0004 to player-3 --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions

Then - Actions

Else - Actions

You most have preset Backpack Units again in your new map. The restorin' trigger will restore cached backpacks and give their items to the new map's Backpacks.

* Set any of preseted backpacks' (not to be confused wit restored backpacks) owner to either Player's ownership for havin' access to them from de beginnin' or to "Neutral Passive" and then while loading, check if a upgrade is done by player, then set their owner back to Player.

-- Hope this works properly! If ya know better systems link dem below so we all can use'em.

Note: If u know 'bout Unit Indexer systems, they would interfere wit dis, if used any.

Edit: will be updated soon using new features and unit indexer - Today 30 June

#### Attachments

Backpack Inventory System.w3x

 16 KB

 · Views: 94




 Last edited: Jun 30, 2025

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


Seems like this is more fitting for the spells section. You can upload a map and copy-paste this full code into it.

#### Master Zul'khan

#### Master Zul'khan

 Level 8

JoinedApr 26, 2020Messages66

Resources: 1

#### Resources
Tutorials 1

Tha owner mon of Hive! How humble of you.

Will do upload a map tonight.

I don'no if dis be a spell but if ya say so.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Some thoughts:

1. It should be described what a backpack system is.

2 Picture(s) of the final product is probably a good idea

3. Using waits is not considered best practice

4. A system should be able to support an (practically) unlimited amount of backpacks, too much of your things are hard coded. For example you can use unit groups of unit type instead of using backpack 1,2,3,4

#### Master Zul'khan

#### Master Zul'khan

 Level 8

JoinedApr 26, 2020Messages66

Resources: 1

#### Resources
Tutorials 1

Chaosy said:

 Some thoughts:

1. It should be described what a backpack system is.

2 Picture(s) of the final product is probably a good idea

3. Using waits is not considered best practice

4. A system should be able to support an (practically) unlimited amount of backpacks, too much of your things are hard coded. For example you can use unit groups of unit type instead of using backpack 1,2,3,4


Click to expand...

Added an Introdution and pic.

We all don' like 'wait until' too mon, but ya know there I didn't found anything more quality.

I used Triggering Unit references and Group events beside de preset unit anywhere possible. It is just dat there isn't any "Is Selected" for Variables or Group Units. Oh, no it is not hardcoded. You can just add a backpack by placin' another in tha map and addin' another selection event for it wit an specific text message to showup. Eazy. (However I recommend 24 item slots (+6 hero inventory) to be enough for an one man rpg.)
I tink it is Complete now!

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

You're leaking Unit Groups:

Unit Group - Pick every unit in (Units owned by Player 3 (Teal) of type Backpack) and do (Unit - Move (Picked unit) instantly to (Player 3 (Teal) start location))

Unit Group - Pick every unit in (Units owned by Player 3 (Teal) of type Backpack) and do (Unit - Set the custom value of (Picked unit) to 0)
Also, taking advantage of the Custom Value action will interfere with Unit Indexer systems that use it as well, and those are pretty common.

I suppose the system's simplicity is what you were aiming for and I can respect that, but it could be greatly improved with some complexity.

#### Master Zul'khan

#### Master Zul'khan

 Level 8

JoinedApr 26, 2020Messages66

Resources: 1

#### Resources
Tutorials 1

Uncle said:

 You're leaking Unit Groups:


Click to expand...

Wow, did not expect dat.

Dis means every time I picked every unit in a not-variable group, i was actually creatin' new unit groups. But does dat means too that I can refer to dem by last created unit group? I'll check it. No worry cause I'll fix it by storin' backpacks in a unit group and then referin' to dat variable. - Sure i'll do remove de (no variables) from title.

I just read about Bribe's Unit Indexer so I noted it to this thread.

Thanx for tha hints big mon.


 Last edited: Feb 14, 2023

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

So... excluding transfering items between maps, this is basically a backpack system where the backpack is teleported to the main character's position whenever it is selected by the Idle Worker key?

#### Master Zul'khan

#### Master Zul'khan

 Level 8

JoinedApr 26, 2020Messages66

Resources: 1

#### Resources
Tutorials 1

Dis be a basic inventory sys' indeed yet handy n' useful. Much like opennin' a real backpack and puttin' it back.

Spacebuns said:

 So... excluding transfering items between maps


Click to expand...

Just store the Backpack Units simply, using "Gamecache - store unit" action. Restore like I did in tha last section.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Returning to this,

Chaosy said:

 Some thoughts:

1. It should be described what a backpack system is.

2 Picture(s) of the final product is probably a good idea

3. Using waits is not considered best practice

4. A system should be able to support an (practically) unlimited amount of backpacks, too much of your things are hard coded. For example you can use unit groups of unit type instead of using backpack 1,2,3,4


Click to expand...

Point 3 and 4 are still not addressed. The code contains too many things are are not considered anywhere near best practice while coming with the HUGE downside of not working with unit indexers.

A creative user could certainly take this as a base and expand upon but on its own I think a more robust creation can be made.

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 Simple Backpack Ability


ToolorDie

Jul 21, 2016

Triggers & Scripts

Replies7Views2K

Jul 22, 2016
Wietlol

Locked

[Trigger]
 Backpack hashtable not switching items.


Flitty

Nov 3, 2015

Triggers & Scripts

Replies5Views2K

Nov 3, 2015
Wietlol

Locked

 Backpack


Faith

Jul 4, 2013

World Editor Help Zone

Replies4Views2K

Jul 4, 2013
Faith

Locked

[Trigger]
 Backpack Unit


Yiux

May 6, 2014

Triggers & Scripts

Replies10Views2K

May 14, 2014
deathismyfriend

Locked

 Does "Region - Move" Work?


aussj5link

Jan 1, 2015

Triggers & Scripts

Replies12Views2K

Jan 2, 2015
aussj5link

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