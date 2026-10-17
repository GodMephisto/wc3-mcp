---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-a-map-with-4-race.294889/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making a Map with 4 Race
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-a-map-with-4-race.294889/>
#### burnie

B

#### burnie

How to make extra races work

Hi guys, at first im going to say thad this, what is often here called is impossible, isnt impossible.

It took me a lot of work to get extra races work with out replacing any normal race, but there are still some little things to fix like a the win/defeat condition like a normal match, and that the replaced units allways move some pixel, but we can handle this last things as community i think.

And so far i am focusing in first line at the triger, and editor work because im pretty bad at creating balanced units. Also this tutorial is based on a map for up to 8 players.

Usefull custom race creation tutorial overview:

Custom Race Creation: Advanced Mechanics

You should be also aware that every Custom Hero and every Custom Altar must exist 2 times.

One free version (0 gold;0 wood), and one regular. In fact i choosed that at the begging the Altars who be build before u picked ure first hero are changed to the free ones (there is an variable and a trigger wich i show u at a later time wich check this). After the training of ure first Hero the free Altar changes back to the regular one and the trained free hero is replaced with the one with normal costs, so there is no difference in playing.

Initzialisation:

At first we are going to delete in the Melee Initialization this two points:

Melee Game - Create starting units (for all players)

Melee Game - Enforce victory/defeat conditions (for all players)
For the reason that there are no starting units (because u can choose an race in dialog menu), and thad u dont get viewed to every one because the custom townhalls dont work for that trigger even if u listed them up.

Pick a race dialog menu:

Now im gonna show how it could look like, for those of you wich don't know how to create a dialog i put a link right below:

Basics of a Dialog

At first we need to create 1 Dialog (choise) variable and 7 Dialog Buttons (a-f).

In fact 1 Dialog Button each race.

the buttons:

Buttons
Events
Map initialization

Conditions

Actions
Wait 0.05 seconds

Dialog - Change the title of wahl to Races

Dialog - Create a dialog button for wahl labelled Human

Set a = (Last created dialog Button)

Dialog - Create a dialog button for wahl labelled Orc

Set b = (Last created dialog Button)

Dialog - Create a dialog button for wahl labelled Undead

Set c = (Last created dialog Button)

Dialog - Create a dialog button for wahl labelled Nightelf

Set d = (Last created dialog Button)

Dialog - Create a dialog button for wahl labelled Highelf

Set e = (Last created dialog Button)

Dialog - Create a dialog button for wahl labelled Feel Orks

Set f = (Last created dialog Button)

Dialog - Create a dialog button for wahl labelled United Trolls

Set g = (Last created dialog Button)

the menu:

Menu
Events
Map initialization

Conditions

Actions
Wait 0.10 seconds

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Player 1 (Red) controller) Equal to User

Then - Actions
Dialog - Show choise for Player 1 (Red)

Else - Actions
Do nothing

Melee Game - Create (Race of Player 1 (Red)) starting units for Player 1 (Red) at (Player 1 (Red) start location) (Include Heroes)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Player 2 (Blue) controller) Equal to User

Then - Actions
Dialog - Show choise for Player 2 (Blue)

Else - Actions
Do nothing

Melee Game - Create (Race of Player 2 (Blue)) starting units for Player 2 (Blue) at (Player 2 (Blue) start location) (Include Heroes)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Player 3 (Teal) controller) Equal to User

Then - Actions
Dialog - Show choise for Player 3 (Teal)

Else - Actions
Do nothing

Melee Game - Create (Race of Player 3 (Teal)) starting units for Player 3 (Teal) at (Player 3 (Teal) start location) (Include Heroes)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Player 4 (Purple) controller) Equal to User

Then - Actions
Dialog - Show choise for Player 4 (Purple)

Else - Actions
Do nothing

Melee Game - Create (Race of Player 4 (Purple)) starting units for Player 4 (Purple) at (Player 4 (Purple) start location) (Include Heroes)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Player 5 (Yellow) controller) Equal to User

Then - Actions
Dialog - Show choise for Player 5 (Yellow)

Else - Actions
Do nothing

Melee Game - Create (Race of Player 5 (Yellow)) starting units for Player 5 (Yellow) at (Player 5 (Yellow) start location) (Include Heroes)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Player 6 (Orange) controller) Equal to User

Then - Actions
Dialog - Show choise for Player 6 (Orange)

Else - Actions
Do nothing

Melee Game - Create (Race of Player 6 (Orange)) starting units for Player 6 (Orange) at (Player 6 (Orange) start location) (Include Heroes)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Player 7 (Green) controller) Equal to User

Then - Actions
Dialog - Show wahl choise Player 7 (Green)

Else - Actions
Do nothing

Melee Game - Create (Race of Player 7 (Green)) starting units for Player 7 (Green) at (Player 7 (Green) start location) (Include Heroes)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Player 8 (Pink) controller) Equal to User

Then - Actions
Dialog - Show choise for Player 8 (Pink)

Else - Actions
Do nothing

Melee Game - Create (Race of Player 8 (Pink)) starting units for Player 8 (Pink) at (Player 8 (Pink) start location) (Include Heroes)

Futhermore this trigger secure thad if there is no human player, the KI get the race wich was choosen in the game menu.

the race placement:

Choose Race
Events
Dialog - A dialog button is clicked for choise

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to a

Then - Actions
Melee Game - Create Human starting units for (Triggering player) at ((Triggering player) start location) (Include Heroes)

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to b

Then - Actions
Melee Game - Create Orc starting units for (Triggering player) at ((Triggering player) start location) (Include Heroes)

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to c

Then - Actions
Melee Game - Create Undead starting units for (Triggering player) at ((Triggering player) start location) (Include Heroes)

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to d

Then - Actions
Melee Game - Create Night Elf starting units for (Triggering player) at ((Triggering player) start location) (Include Heroes)

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to e

Then - Actions
Unit - Create 1 Ratsturm for (Triggering player) at ((Triggering player) start location) facing Default building facing degrees

Unit - Create 5 Worker (Blood Elf) for (Triggering player) at ((Triggering player) start location) facing Default building facing degrees

Player - Limit training of Archmage Hochelfen to 1 for (Triggering player)

Player - Limit training of Blood Mage (Hochelfen) to 1 for (Triggering player)

Player - Limit training of Ranger to 1 for (Triggering player)

Player - Limit training of Paladin to 1 for (Triggering player)

Player - Limit training of Archmage free to 1 for (Triggering player)

Player - Limit training of Blood Mage free to 1 for (Triggering player)

Player - Limit training of Ranger free to 1 for (Triggering player)

Player - Limit training of Paladin free to 1 for (Triggering player)

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to f

Then - Actions
Unit - Create 1 Great Hall to for (Triggering player) at ((Triggering player) start location) facing Default building facing degrees

Unit - Create 5 Fel Orc Peon for (Triggering player) at ((Triggering player) start location) facing Default building facing degrees

Player - Limit training of Blademaster to 1 for (Triggering player)

Player - Limit training of Blademaster free to 1 for (Triggering player)

Player - Limit training of Warlord to 1 for (Triggering player)

Player - Limit training of Warlord free to 1 for (Triggering player)

Player - Limit training of Warlock to 1 for (Triggering player)

Player - Limit training of Warlock free to 1 for (Triggering player)

Player - Limit training of Pit Lord to 1 for (Triggering player)

Player - Limit training of Pit Lord free to 1 for (Triggering player)

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to g

Then - Actions
Unit - Create 1 Great Hall for (Triggering player) at ((Triggering player) start location) facing Default building facing degrees

Unit - Create 5 Peon for (Triggering player) at ((Triggering player) start location) facing Default building facing degrees

Player - Limit training of Avatar to 1 for (Triggering player)

Player - Limit training of Avatar free to 1 for (Triggering player)

Player - Limit training of Beastmaster to 1 for (Triggering player)

Player - Limit training of Beastmaster free to 1 for (Triggering player)

Player - Limit training of Shadow Hunter to 1 for (Triggering player)

Player - Limit training of Shadow Hunter free to 1 for (Triggering player)

Player - Limit training of Pit Lord to 1 for (Triggering player)

Player - Limit training of Pit Lord free to 1 for (Triggering player)

Else - Actions
Do nothing

Altar and Hero training:

As told above we are now going to the abuse of the no free Hero problem.

For thad i've even got a solution.

First of all we need (in this scenario) 8 Integer Variants (p1-p8).

These are set to 0 at will only used to count to 1.

Here is one example of the Altar script (you have to make this for every race):

altar
Events
Unit - A unit Finishes construction

Conditions
(Unit-type of (Constructed structure)) Equal to Chaos Altar

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
And - All (Conditions) are true
Conditions
(Owner of (Constructed structure)) Equal to Player 1 (Red)

p1 Equal to 0

Then - Actions
Unit - Replace (Constructed structure) with a Chaos Altar free using The old unit's relative life and mana

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
And - All (Conditions) are true
Conditions
(Owner of (Constructed structure)) Equal to Player 2 (Blue)

p2 Equal to 0

Then - Actions
Unit - Replace (Constructed structure) with a Chaos Altar free using The old unit's relative life and mana

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
And - All (Conditions) are true
Conditions
(Owner of (Constructed structure)) Equal to Player 3 (Teal)

p3 Equal to 0

Then - Actions
Unit - Replace (Constructed structure) with a Chaos Altar free using The old unit's relative life and mana

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
And - All (Conditions) are true
Conditions
(Owner of (Constructed structure)) Equal to Player 4 (Purple)

p4 Equal to 0

Then - Actions
Unit - Replace (Constructed structure) with a Chaos Altar free using The old unit's relative life and mana

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
And - All (Conditions) are true
Conditions
(Owner of (Constructed structure)) Equal to Player 5 (Yellow)

p5 Equal to 0

Then - Actions
Unit - Replace (Constructed structure) with a Chaos Altar free using The old unit's relative life and mana

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
And - All (Conditions) are true
Conditions
(Owner of (Constructed structure)) Equal to Player 6 (Orange)

p6 Equal to 0

Then - Actions
Unit - Replace (Constructed structure) with a Chaos Altar free using The old unit's relative life and mana

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
And - All (Conditions) are true
Conditions
(Owner of (Constructed structure)) Equal to Player 7 (Green)

p7 Equal to 0

Then - Actions
Unit - Replace (Constructed structure) with a Chaos Altar free using The old unit's relative life and mana

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
And - All (Conditions) are true
Conditions
(Owner of (Constructed structure)) Equal to Player 8 (Pink)

p8 Equal to 0

Then - Actions
Unit - Replace (Constructed structure) with a Chaos Altar free using The old unit's relative life and mana

Else - Actions
Do nothing

This one check if the Integer is 0, and if is the altar changes to the one with the free heroes.

The first hero placement:

Furthemore here are we changing the free hero to the normal, it is there fore that you get the first hero for free, but have later to take the repair costs like regular.

The annoying thing about this is, thad u have to redo the Action if/then/else Condition for every single custom hero.

And this is the part were the above called Integer gets count to 1 and the replacing of the altars stops.

Events
Unit - A unit Finishes training a unit

Conditions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Unit-type of (Trained unit)) Equal to Blademaster free

Then - Actions
Unit - Replace (Trained unit) with a Blademaster using The old unit's relative life and mana

Unit - Replace (Triggering unit) with a Chaos Altar using The old unit's relative life and mana

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Owner of (Trained unit)) Equal to Player 1 (Red)

Then - Actions
Set p1 = 1

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Owner of (Trained unit)) Equal to Player 2 (Blue)

Then - Actions
Set p2 = 1

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Owner of (Trained unit)) Equal to Player 3 (Teal)

Then - Actions
Set p3 = 1

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Owner of (Trained unit)) Equal to Player 4 (Purple)

Then - Actions
Set p4 = 1

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Owner of (Trained unit)) Equal to Player 5 (Yellow)

Then - Actions
Set p5 = 1

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Owner of (Trained unit)) Equal to Player 6 (Orange)

Then - Actions
Set p6 = 1

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Owner of (Trained unit)) Equal to Player 7 (Green)

Then - Actions
Set p7 = 1

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Owner of (Trained unit)) Equal to Player 8 (Pink)

Then - Actions
Set p8 = 1

Else - Actions
Do nothing

Else - Actions
Do nothing

Now we are going to the last point wich is are the Gameplay Constants:

Gameplay Constants:

- first go to Advanced -> Gameplay Constants

- then click the box "Use Custom Gameplay Constants"

- now move to the line: "Techtree - Depemdemcy Equivalents"

- to open the tabs you need to double click them

Editing these:

Techtree - Depemdemcy Equivalents - Altar (add here you're Custom Altar's)

Techtree - Depemdemcy Equivalents - Hero (add here you're Custom Hero's (just the regular ones!))

Techtree - Depemdemcy Equivalents - Tier 1 Hall ( add here you're basic townhall's)

Techtree - Depemdemcy Equivalents - Tier 2 Hall ( add here you're advanced townhall's)

Techtree - Depemdemcy Equivalents - Tier 3 Hall ( add here you're maximum townhall's)

You can add up to Tier 9 Hall if you like to advance the game more and more, but 3 Hall's are the basic.

Last but not least the missing features were i require you're help:

The first thing is, as i told at the beginning, that the replaced Altar's allways move some pixel's wich doesnt look nice ( because there get replaced 2 times until you have the first hero).

The second thing is the hardest i think: the win/defeat conditions.

In fact i can't get one to work wich is like the regular one.

The only thing i got for now is this pretty poor trigger:

win/defeat
Events
Unit - A unit Dies

Conditions
And - All (Conditions) are true
Conditions
(Count structures controlled by (Owner of (Dying unit)) (Exclude incomplete structures)) Equal to 0

(Count non-structure units controlled by (Owner of (Dying unit)) (Exclude incomplete units)) Equal to 0

Actions
Game - Display to (All players) the text: ((Name of (Owner of (Dying unit))) + was defeaten!)

Game - Defeat (Owner of (Dying unit)) with the message: defeat!

Postscript

I hope thad you enjoyed my tutorial and that it will help you to make 4+ Race maps on mass.

At least I am waiting for some feedback and i hope you guys can help me out with the last problems.

For the totally interessted ones i am uploading my done work so that every on can take a look at it.

But be warned: i made this map (and i don't know why) half in my native language (german) and half in english.

#### Attachments

HurricaneIsle WCIV Editon.w3x

 1 MB

 · Views: 178




 Last edited by a moderator: Jun 28, 2017

#### Sarungard

#### Sarungard

 Level 3

JoinedJun 25, 2011Messages39

I will check this out and if you are interested, I can tell some hints, I am currently working on an altered melee with (currently planned) 8 custom races (4 redesigned original and 4 totally new and unique), expanded techtrees, new features.

If you are interested, we can even work together

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Thanks for the submission! You have a lot of cool ideas, but I think there are a lot of ways to improve this tutorial:

Formatting: some headers, only use colors for emphasizing small things (e.g. headers), put the lists in list tags, fix spelling/grammar

A lot of these triggers could be condensed, especially if you use player indices to reduce all the if/then/else statements.

I think you'll want to consider adding things to separate yourself from the existing guides on this topic:
How to: Create a Full-working Custom Race

Custom Race Creation: Advanced Mechanics

I like that you had triggers for defeat and such. But they could be improved. And handling all the weird cases like allied forces, revealing players, etc. Some of it is covered in this post if you need help:

How to: Create a Full-working Custom Race

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Graveyarding since it has been a while since the last update. Feel free to PM me if you'd like me to move it back!

 You must log in or register to reply here.


### Similar threads

 Redoing your maps Terrain(Advanced)


dab

Jun 6, 2009

Tutorial Archive

Replies8Views2K

Jun 9, 2009
Pyritie

 This is a good way to make a orpg


jordon

Jul 9, 2009

Tutorial Archive

Replies4Views1K

Jul 16, 2009
jordon

 How to make a TD Wars


jordon

Sep 12, 2009

Tutorial Archive

Replies4Views2K

Sep 15, 2009
.Ragnaros

[Solved]
 Trigger - Starting units not created with triggers (Dialog button custom race pick)


Mekanical

Mar 21, 2026

World Editor Help Zone

Replies2Views205

Mar 21, 2026
Mekanical

Locked

 Open slot for players get custom race starting units


tdrsmynm

Feb 6, 2021

World Editor Help Zone

Replies1Views1K

Feb 8, 2021
hexhound

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