---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/tower-defense-for-beginners.215596/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Tower Defense for Beginners
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/tower-defense-for-beginners.215596/>
#### Maker1209

M

#### Maker1209

Tower Defense for Beginners.

This is the simplest way of how to create a TD (Tower defense) in Warcraft III world editor.

Requirements: Basic understanding of the world editor.

Terrain

Difficulty:

First start up Warcraft world editor.

Then, select Your preferable map size.

96 x 96 recommended.

How to create a new map?

Click on this icon:

Pathing

Difficulty:

Now create a path for the creeps to walk with the terrain editor.

Something like this: ( Be sure the path is unbuildable )

Now go to the region Palette and put two regions on the start and end of your path. Rename them as Start andEnd.

Triggers

Difficulty:

Now time for the triggers!

First, the bounty.

Create a new trigger and name it Bounty.

Now create a new Event.

Select Time > Periodic event

and set the time to every 1.00 seconds. This means that something would happen every 1 second of game time.

Then, go to Actions and select Player > Turn player flag on/off

and set it to turn player's bounty on for player brown.

Why brown? Because brown would be the monsters or creeps that would be killed by the towers. Bounty means the "reward" earned via killing the creep.

Thus, we must on the bounty for brown so that we get gold by killing creeps.

Next we want to have the creeps to move from the starting point to the ending point which would take away the player's life.

Create another trigger and name it Unit move or move (anything you want)

Create a new event: Unit > Unit enters region

And set region to the region that you have named "start"

Then, create a condition: Player comparison

and make Player as Player brown. This means that you want only player brown's units to move. If you do not put the condition, any player's units that enters that region would automatically move.

Lastly, make an Action: Unit > Issue unit to target a point

and set point to region "End" and put unit to "move to" region "End". This would allow the creeps to move to the ending point.

Now we need creeps! Make a new trigger call "units".

Create an action: Time > Time elapsed (passed)

and set time to whichever time you want the creeps to spawn at.

Then, create an action: Unit > Create unit facing

 angle.

Set unit quantity to whatever number of creeps you want to spawn in and region to region "start" and set for player brown. Ignore building facing degrees. This means after a certain amount of time, a number of creeps would spawn and move towards the ending point. Be sure that your unit has no attack or damage.

Now for the hardest trigger: Lives.

Create a unit first, set its hp to 50,name to lives , and place it near your path. This unit would train your builder and also shows how much life you have left.

Do not make it invulnerable. Now, set an Event:Unit > Unit enters region

set region to region "end".

Then, set a condition: Player comparison

and make Player as Player brown.

Now add an action: Unit > Damage target

and set damage to 1, ignore the attack type and damage type and set "Cause triggering unit to damage triggering unit" into "Cause Entering unit to damage lives" ("lives" is the building which i told you to put near the path.)

and add a final action: Unit > Remove

and set unit to entering unit.

Now for the "selling" of buildings.

Create a unit, perhaps a footman, name it "Sell Building".

Remove its requirements, set building time to 1 , sound set to none and gold and lumber cost to 0.

Now, create a trigger named "sell"

and do this:

then, make your towers able to train your "sell building" unit! Remember to change your tower's point value to how much it costs!

Now for the starting gold for the td.

create a new trigger called "gold start"

Create an event: Time > Elapsed

and set time to 1.00.

Then create an action: Player > Add property

ans set to "Add 100 to player red's current gold."

you could put any number in there.

Now for defeat condition.

Create a new trigger called "lose"

Create an event: Unit > Unit dies

and set Unit to "lives" (your building again) by selecting it in your map.

Then add an condition: Player comparison

and set to player red.

Create an action: Game > defeat!

Set player to player red and with the message defeat or lose.

Allies and forces

Now, just go and add player brown as your enemy in player and forces

 properties.

Finally, name your map and test it out!

You should have:

-1 wave of creeps

-a map with a path

-starting gold for towers

-defeat condition

-creep bounty

To finish off your map, make towers and more waves of creeps!

I am sure my method for Td is not the official one, but this version should be understandable for beginners!


 Last edited by a moderator: May 1, 2012

#### sonofjay

#### sonofjay

 Level 28

JoinedOct 28, 2011Messages4,750

Resources: 27

#### Resources
Packs 1Maps 20Templates 5Tutorials 1

Use text colors and BBcodes to make your tutorial more attractive.

And add your trigger like this

 JASS:

[trigger]Melee Initialization
 Events
 Map initialization
 Conditions
 Actions
 Melee Game - Use melee time of day (for all players)
 Melee Game - Limit Heroes to 1 per Hero-type (for all players)
 Melee Game - Give trained Heroes a Scroll of Town Portal (for all players)
 Melee Game - Set starting resources (for all players)
 Melee Game - Remove creeps and critters from used start locations (for all players)
 Melee Game - Create starting units (for all players)
 Melee Game - Run melee AI scripts (for computer players)
 Melee Game - Enforce victory/defeat conditions (for all players)
[/trigger]

this thing will show up like this

Melee Initialization
Events
Map initialization

Conditions

Actions
Melee Game - Use melee time of day (for all players)

Melee Game - Limit Heroes to 1 per Hero-type (for all players)

Melee Game - Give trained Heroes a Scroll of Town Portal (for all players)

Melee Game - Set starting resources (for all players)

Melee Game - Remove creeps and critters from used start locations (for all players)

Melee Game - Create starting units (for all players)

Melee Game - Run melee AI scripts (for computer players)

Melee Game - Enforce victory/defeat conditions (for all players)

I'll read it later.

EDIT:

I just remember that we already have a good TD tutorial from the Chieftain Ralle so I think this will not be approved.


 Last edited: May 2, 2012

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

We have two good guides on TD's already:

https://www.hiveworkshop.com/forums/general-mapping-tutorials-278/basics-tower-defense-7004/

https://www.hiveworkshop.com/forums/general-mapping-tutorials-278/ralles-tower-defence-guide-34870/

If you want this approved, it would have to include some more interesting things beyond the basics. But otherwise, I think the basics have been well covered by those two tutorials so we don't need another. Thanks for the contribution though.

EDIT: Graveyarded for now. Let me know if you are willing to update.


 Last edited: Jul 10, 2012

#### XperiencePro

X

#### XperiencePro

The Creeps wont move

 You must log in or register to reply here.


### Similar threads

[Mapping]
 Making an efficient TD


Crazed_seal2

Dec 25, 2009

Tutorial Archive

Replies13Views3K

Dec 11, 2010
PurgeandFire

 Maze Making Tutorial For Beginners


DestroAnt

Apr 26, 2006

Tutorial Archive

Replies4Views4K

Mar 12, 2007
Wolverabid

N

 Lives for Tower defenses


NeJiIjEn

Dec 1, 2006

Tutorial Archive

Replies2Views2K

Dec 1, 2006
NeJiIjEn

N

Poll

 How to make a "TD"(Tower Defense)


jacobemslayer

Nov 29, 2009

Tutorial Archive

Replies8Views3K

Apr 29, 2010
PurgeandFire

 Basics of a TD


ragingspeedhorn

Mar 2, 2006

Tutorial Archive

Replies22Views6K

Mar 6, 2007
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