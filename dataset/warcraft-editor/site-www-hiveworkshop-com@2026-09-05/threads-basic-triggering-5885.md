---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/basic-triggering.5885/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Basic Triggering
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/basic-triggering.5885/>
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


Introduction


This tutorial describes the difference between Events, Conditions and Actions.


I will show some examples along the way and help you to understand the effects and power of triggers.


We have many things to do, so let's get started.


To begin we must create a new blank trigger using WE’s Trigger Editor. Press F4 on your keyboard or select the lowercase “c” button on the toolbar. Now select New>Trigger from the trigger menu and…presto! A new trigger is born.

Untitled Trigger 001
Events

Conditions

Actions

Let’s give our triggers names to make them easy to identify. When new triggers are created, you always have the option to give them unique names. If you forget, in the left hand pane, double click on the trigger you want to rename and enter a name for it.

Events


Events are the functions that “trigger” your triggers.


For example, when you make a timed bomb, the event will activate the trigger when the timer reaches 0.


We can make simple events for triggers by right clicking on the event and selecting “New Event.” The Configure Event window then opens and displays Map Initialization as the default event. We’ll chose a different event for our first trigger so scroll down the list until you find Unit - Generic Unit Event and select it. The default event in this category is A Unit Dies and this is the event that we want. Notice how the word “Dies” is colored and underlined? This means that you can change the value for that field to a variety of other generic unit events. For now, let’s leave it alone and proceed.

Unit Dies
Events
Unit - A unit Dies

Conditions

Actions

This trigger will start every time a unit dies.


To create a timed trigger select the event: Time - Periodic Event.


The default value for this event is two seconds. Let’s edit this value so that our trigger fires every minute. Click on the underlined and colored 2.00 and a window opens that allow us to change this value to any real number. Use the tiny arrow button or type in 60 to make the change.

Timed Trigger 001
Events
Time - Every 60.00 seconds of game time

Conditions

Actions

This trigger will start every 60 seconds of the game.


For a region specific event use: Unit - Unit Enters Region.

Unit Enters Region
Events
Unit - A unit enters Region 000 <gen>

Conditions

Actions

This trigger will start when a unit enters a region (regions are made in Terrain Editor, Region Palette)

Conditions


Conditions are present to make your trigger run only under certain conditions. In the above example, the trigger would fire when any unit owned by any player enters Region 000. Say you wanted only certain units or players to trip the trigger?


Let's just add a simple condition to our trigger to make only certain unit types operate it. Again, right click on the condition to create a new one. Since the default condition is Boolean Comparison, scroll down and select Unit Type Comparison. This applies the condition:

Unit Enters Region
Events
Unit - A unit enters Region 000 <gen>

Conditions
(Unit-type of (Triggering unit)) Equal to Footman

Actions

This is the trigger from above, the one where a unit enters a region. This time, the trigger will only be executed if the entering unit is a Footman.


Another simple condition would allow only a certain player (or players) to set off the trigger:

Unit Enters Region
Events
Unit - A unit enters Region 000 <gen>

Conditions
(Owner of (Triggering unit)) Equal to Player 1 (Red)

Actions

Under this condition the trigger would affect only units owned by Player 1.


This condition would qualify our timed trigger:


We could also add a condition like this:

Unit Dies
Events
Unit - A unit Dies

Conditions
(Unit-type of (Dying Unit)) Equal to Archer

Actions

This is the example we used before, the one where a unit dies. But the difference is that this time the trigger will only be executed if the Dying unit is an Archer.


Conditions are necessary for a trigger to work properly but that doesn't mean that you will be forced to use them in order to make the trigger work.

Actions


Actions are the main part of a trigger. If we take the example from before with the bomb, this will be the explosion of your bomb.


So when something triggers your event, it checks if it matches the conditions (if they exist). Then it runs your actions.


So here we have our trigger again. The one with the dying archer:

Unit Enters Region
Events
Unit - A unit Dies

Conditions
(Unit-type of (Entering unit)) Equal to Archer

Actions
Unit - Create 2 Archer for (Owner of (Dying unit)) at (Center of (Playable map area)) facing Default building facing (270.0) degrees

This trigger will endlessly create 2 archers when one dies and could cause the game to crash because it will create archers all the time.

Unit Enters Region
Events
Unit - A unit enters Region 000 <gen>

Conditions
(Unit-type of (Entering unit)) Equal to Footman

Actions
Unit - Kill (Entering unit)

This is another example, the one where a footman enters a region. This time it also has an action. When the footman enters the region, the trigger action kills it.


All my examples have only one event, one action and one condition. They could have an unlimited of each, like this more advanced trigger:

Unit Enters Region
Events
Unit - A unit enters Region 000 <gen>

Conditions
(Unit-type of (Entering unit)) Equal to Footman

(Owner of (Entering unit)) Equal to Player 3 (Teal)

Actions
Unit - Kill (Entering unit)

Again the same trigger with the footman but this time the owner of the footman has to be player 3.


You could also make a trigger like this:

Unit Dies
Events
Unit - A unit Dies

Conditions
(Unit-type of (Dying unit)) Equal to Archer

Actions
Unit - Create 2 Archer for (Owner of (Dying unit)) at (Center of (Playable map area)) facing Default building facing (270.0) degrees

Unit - Change ownership of (Last created unit) to (Owner of (Killing unit)) and Change color

This trigger is the same example with the dying archer again: it creates 2 archers. But now it also takes the last created unit, which is an archer, and changes the owner of it to the owner of the unit that killed your archer in the first place.


I hope you learned something about this subject and that it wasn't a waste of our time. I’m sure that you now realize that the question: "is it an event, a condition, or an action???" is unnecessary and you will always know just how to make a trigger. Beginning trigger editors should start simple, learn some of the many options available, and practice, practice, practice!


 Last edited by a moderator: Mar 26, 2007

#### xXMephistoXx

#### xXMephistoXx

 Level 18

JoinedSep 3, 2006Messages940

Resources: 23

#### Resources
Models 10Icons 7Packs 1Skins 1Maps 2User Interface 1Tutorials 1

Basic Trigger Functions

OK, the first function we're going to look at is creating a unit:


First of we create ourselves an Event. It creates many possibilities for different things we can do. We do not NEED an event, we could also use another trigger to activate this Trigger, but then that Trigger would require an Event...


We also sometimes have 1 or more conditions which makes the Event more specific.


In the End, there are Actions, this is what happens when the Event is activated and (sometimes) the Condition is right.

Note: Words underlined like this Word is words you can replace with your own. Like No unit = 013 Paladin


For example we can let one or more units spawn periodically:

Events

Time - Every 2.00 seconds of game time

Conditions

Actions

Unit - Create 1 Footman for Player 1 (Red) at (Center of [U](Playable map area)[/U]) facing Default building facing degrees
Or... We can spawn one or more units when a unit enters a region, read Archian's Tutorial on Regions

Events

Unit - A unit enters [U]No region[/U]

Conditions

Actions

Unit - Create 1 Footman for Player 1 (Red) at (Center of [U](Playable map area)[/U]) facing Default building facing degrees
We can also only use this Trigger if a specific unit enters the Region:

Events

Unit - A unit enters [U]No region[/U]

Conditions

((Triggering unit) is [U]A Hero[/U]) Equal to True

Actions

Unit - Create 1 Footman for Player 1 (Red) at (Center of [U](Playable map area)[/U]) facing Default building facing degrees
Now we are going to take a look at Special Effects. Jazz is much more "unlaggy" at this, but that's not what this tutorial is for...


A Trigger which makes abilities look cooler than before with Triggers. Is also possible simply with Buffs:

Events

Unit - A unit Starts the effect of an ability

Conditions

(Ability being cast) Equal to [U]Animate Dead[/U]

Actions

Special Effect - Create a special effect attached to the [U]origin/overhead/hand right/ hand left/foot right/foot left/head/chest[/U] of [U](Triggering unit) using [U]Abilities\Spells\Human\Avatar\AvatarCaster.mdl[/U]
Making a unit go. Can also use AI, much better...

Events

Unit - A unit enters [U]No region[/U]

Conditions

((Triggering unit) is [U]A Hero[/U]) Equal to True

Actions

Unit - Set Rally Point for [U](Triggering unit)[/U] to (Center of [U]No region[/U])
Teleporting a unit:

Events

Unit - A unit enters [U]No region[/U]

Conditions

((Triggering unit) is [U]A Hero[/U]) Equal to True

Actions

Unit - Move [U](Triggering unit)[/U] to (Center of [U]No region[/U])
These two last Triggers we can also use for many units:


Making units go. Can also use AI, much better...

Events

Unit - A unit enters [U]No region[/U]

Conditions

Actions

Unit Group - Pick every unit in (Units in [U]No region[/U]) and do (Unit Set Rally-Point for (Picked Unit) to (Center of [U]No region[/U]))
Teleporting units:

Events

Unit - A unit enters [U]No region[/U]

Conditions

Actions

Unit Group - Pick every unit in (Units in [U]No region[/U]) and do (Unit Move (Picked Unit) instantly to (Center of [U]No region[/U]))
Cinematics (have to be done before, but I'll look at them anyway):

Events

Map initialization

Conditions

Actions

Cinematic - Turn cinematic mode On for [U](All players)[/U]

Cinematic - Clear the screen of text messages for [U](All players)[/U]

Cinematic - Turn on letterbox mode (hide interface) for [U](All players)[/U]: fade out over 0.00 seconds

Cinematic - Disable user control for [U](All players)[/U]
When we are in Cinematic, we often have messages from units in game:

Events

Unit - A unit enters [U]No region[/U]

Conditions

(Unit-type of [U](Triggering unit)[/U]) Equal to [U]Paladin[/U]

Actions

Wait [U]5.00[/U] seconds

Cinematic - Send transmission to [U](All players)[/U] from a [U]Player 1 (Red)[/U] [U]Footman[/U] named [U]Name[/U] at [U](Random point in (Playable map area))[/U]: Play [U]No sound[/U] and display [U]Message[/U]. Modify duration: Add [U]5.00[/U] seconds and [U]Wait[/U]
Now we have covered some of the most basic Triggering functions.


 Last edited: Jul 9, 2007

#### billel

#### billel

 Level 2

JoinedJun 12, 2016Messages7

Resources: 1

#### Resources
Maps 1

thanks a lot, and can you make a tutorial to how to make animation of birth skeleton or any unit?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

billel said:

 thanks a lot, and can you make a tutorial to how to make animation of birth skeleton or any unit?


Click to expand...

Hey billel, welcome to the hive!

Making animations is a bit tricky. A lot of them require modeling software. For basic animation transfers, check out this tutorial:

https://www.hiveworkshop.com/threads/basic-animation-transfers.31703/

But it won't work on all models/animations. Experiment with it and see if it suits your needs!

#### billel

#### billel

 Level 2

JoinedJun 12, 2016Messages7

Resources: 1

#### Resources
Maps 1

PurgeandFire

thank you very much.they helped me a lot!!!!!

 You must log in or register to reply here.


### Similar threads

 Starting with GUI


Lord_of_sausage

Mar 21, 2008

Trigger (GUI) Editor Tutorials

Replies8Views9K

Jul 2, 2018
Ragnar9

 Reviving Heroes at a Changeable Region


dOT145

Mar 23, 2010

Trigger (GUI) Editor Tutorials

Replies12Views10K

Apr 30, 2010
PurgeandFire

 Simple Triggered Hero Artificial Intelligence in GUI


DenZel94

May 2, 2007

Trigger (GUI) Editor Tutorials

Replies22Views26K

Jul 1, 2021
aloH

 Chaosy's GUI tutorial for beginners


Chaosy

Apr 6, 2012

Trigger (GUI) Editor Tutorials

Replies9Views8K

Dec 25, 2016
Chaosy

 Making a Maze


DarkLordX

Feb 11, 2009

Trigger (GUI) Editor Tutorials

Replies28Views16K

Aug 5, 2010
jacobemslayer

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