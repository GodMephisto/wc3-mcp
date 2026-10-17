---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/town-hall-revealing-trigger-for-altered-melee-between-1-27a-and-1-24e-patch.301865/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Town Hall Revealing Trigger for Altered Melee Between 1 27a and 1 24e Patch
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/town-hall-revealing-trigger-for-altered-melee-between-1-27a-and-1-24e-patch.301865/>
#### TUCANA

#### TUCANA

 Level 3

JoinedDec 5, 2017Messages11

Resources: 2

#### Resources
Maps 1Tutorials 1

TOWN HALL REVEALING TRIGGER FOR ALTERED MELEE MAP

Tested on Frozen Throne Patch 1.27.0.52240

There is a TOWN HALL REVEALING problem when you make custom town hall, and some people suggest you to remove this script from default melee initialization:

Melee Game - Enforce victory/defeat conditions (for all players)
DON'T DO THAT, because it will remove default Warcraft III reveal program from your map, except you want to make campaign or scenario map. In this tutorial, I want sharing with you how to deal with this problem without recreate complicated trigger for enforce victory/defeat conditions, remove annoying revealing message, and enable reveal condition for your custom town hall.

The basic idea that make this tutorial work are:

Make a default town hall (first tier), as dummy town hall, for example Tree of Life without editing, in specific region (not in playable area and hidden from all players, you can place the region in border area),

Change the ownership of default town hall (first tier) in that region to Neutral Passive when all of your custom town hall dies,

And don't forget to change back the ownership to the Player when Player's unit begins custom town hall construction.
This method can be applied to:

Custom town hall with 2 (two) up to 9 (nine) tier. In this tutorial section I will give you sample for first tier until third tier and for the rest I hope you can try by yourself,

Make default race (Human, Night Elf, Undead, Orc) coexist with custom race,

Continue game after win or solo without enemy.
For bonus I will inform you how to:

Make default worker position,

Make Entangle Gold Mine when map initialization (based on entangle order),

Select custom race via handicap, so it doesn't need pause trigger in the beginning of the game (in case you create custom race selection via dialog) and able to set random select custom race (optional),

A. FIRST STEP

Don't delete enforce victory/defeat conditions (essential for win condition),

Delete create starting units, (optional, in this case I prefer not to replace default melee worker unit with custom worker unit, because it will increase score point to the Player who select the custom race),

Delete run AI scripts (optional, if you want to customize the AI scripts),

Don't forget to set free hero number and limit custom hero type to 1 (one),

In this trigger, my custom heroes are: Furbolg Ursa Warrior and Salamander Lord.

Melee Initialization
Events
Map initialization

Conditions

Actions
Melee Game - Use melee time of day (for all players)

Melee Game - Remove creeps and critters from used start locations (for all players)

Melee Game - Set starting resources (for all players)

Melee Game - Limit Heroes to 1 per Hero-type (for all players)

Melee Game - Give trained Heroes a Scroll of Town Portal (for all players)

Melee Game - Enforce victory/defeat conditions (for all players)

Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
Player - Set (Picked player) Available free Heroes to 1

Player - Limit training of Heroes to 3 for (Picked player)

Player - Limit training of Furbolg Ursa Warrior to 1 for (Picked player)

Player - Limit training of Salamander Lord to 1 for (Picked player)

B. SECOND STEP

Set variables for your map.

Set Variable
Events
Map initialization

Conditions

Actions
-------- Player Slot --------

Set Slot[0] = Player 1 (Red)

Set Slot[1] = Player 2 (Blue)

-------- Gold Mine --------

Set Mine[0] = 1 Mine <gen>

Set Mine[1] = 2 Mine <gen>

-------- Felwood Base --------

Set Tree[0] = 1 Felwood <gen>

Set Tree[1] = 2 Felwood <gen>

-------- Wisp Spawn --------

Set Wisp[0] = 1 Wisp <gen>

Set Wisp[1] = 2 Wisp <gen>

-------- Reveal Region --------

Set Reveal[0] = 1 Red <gen>

Set Reveal[1] = 2 Blue <gen>

ADDITIONAL INFO

Slot = Player Array (2), for store Player slot,

Mine = Region Array (2), for store your first Gold Mine base location,

Reveal = Region Array (2), for store default town hall (first tier) location, as dummy unit,

Tree = Region Array (2), for store your first base (custom town hall),

Wisp = Region Array (2), for store your custom worker location,

Food = Integer Array (2), initial value = 0, for store food cap (input in variable section only),

I choose 2 (two) for the array number, because I have 2 (two) player in my map,

The script number begin from 0 (zero),

You must create regions before apply this trigger,

Don't forget to insert your custom town hall (all tiers) into gameplay constant section (important).

C. THIRD STEP

Set your custom race.

Set Felwood
Events
Map initialization

Conditions

Actions
For each (Integer A) from 0 to 1, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Slot[(Integer A)] slot status) Equal to Is playing

(Race of Slot[(Integer A)]) Equal to Night Elf

(Slot[(Integer A)] handicap) Equal to 80.00

Then - Actions
Unit - Create 1 Tree of Life for Slot[(Integer A)] at (Center of Reveal[(Integer A)]) facing Default building facing degrees

Unit Group - Pick every unit in (Units in Reveal[(Integer A)]) and do (Unit - Remove Return Lumber from (Picked unit))

Unit - Create 1 Corrupted Tree of Life for Slot[(Integer A)] at (Center of Tree[(Integer A)]) facing Default building facing degrees

Player - Set Slot[(Integer A)] Food cap to 10

Unit Group - Order (Units in Tree[(Integer A)] owned by Slot[(Integer A)]) to Night Elf Tree Of Life - Entangle Nearby Gold Mine (Instant)

For each (Integer B) from 0 to 4, do (Actions)
Loop - Actions
Unit - Create 1 Wisp (Custom) for Slot[(Integer A)] at ((Center of Wisp[(Integer A)]) offset by 72.00 towards (72.00 x (Real((Integer B)))) degrees) facing Default building facing degrees

Else - Actions
Do nothing

ADDITIONAL INFO

You must remove return lumber ability and return gold and lumber ability via trigger from default town hall (tier 1), because worker unit will recognize it as return resource point and will try to return resource to this building if the building location is near,

Don't forget to order your custom town hall, in this case Corrupted Tree of Life, to entangle nearby gold mine (optional),

Don't forget to set food cap,

The offset 72,00 (seventy two) is the distance from central of the region,

I choose 72,00 (seventy two) degrees because I have 5 (five) worker units (360 degrees/5).

If your custom race don't need Entangled Gold Mine, you can remove Tree variable and replace the third line script with this script:

Unit - Create 1 (Your Custom Town Hall) for Slot[(Integer A)] at (Slot[(Integer A)] start location) facing Default building facing degrees

D. FOURTH STEP

Set up reveal condition.

Reveal Felwood
Events
Unit - A unit Dies

Conditions
Or - Any (Conditions) are true
Conditions
(Unit-type of (Dying unit)) Equal to Corrupted Tree of Life

(Unit-type of (Dying unit)) Equal to Corrupted Tree of Ages

(Unit-type of (Dying unit)) Equal to Corrupted Tree of Eternity

Actions
For each (Integer A) from 0 to 1, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Race of Slot[(Integer A)]) Equal to Night Elf

(Slot[(Integer A)] handicap) Equal to 80.00

(Owner of (Dying unit)) Equal to Slot[(Integer A)]

(Number of living Corrupted Tree of Life units owned by Slot[(Integer A)]) Equal to 0

(Number of living Corrupted Tree of Ages units owned by Slot[(Integer A)]) Equal to 0

(Number of living Corrupted Tree of Eternity units owned by Slot[(Integer A)]) Equal to 0

(Number of living Corrupted Moon Well units owned by Slot[(Integer A)]) Greater than or equal to 0

Then - Actions
Unit Group - Pick every unit in (Units in Reveal[(Integer A)]) and do (Unit - Change ownership of (Picked unit) to Neutral Passive and Change color)

Set Food[(Integer A)] = ((Number of living Corrupted Moon Well units owned by Slot[(Integer A)]) x 10)

Player - Set Slot[(Integer A)] Food cap to Food[(Integer A)]

Else - Actions
Do nothing

ADDITIONAL INFO

The trigger above this text is essential for controlling Win, Defeat, and Reveal Condition,

Don't forget to include handicap and Player's race in this trigger section (optional),

Don't forget to set food cap again,

In this trigger, my custom town halls are: Corrupted Tree of Life (first tier), Corrupted Tree of Ages (second tier), and Corrupted Tree of Eternity (third tier),

I used Corrupted Moon Well as farm,

I used Tree of Life (first tier) as dummy unit without editing,

This trigger can be extended up to 9 (ninth) tier, by using Number of Units Function.

E. FIFTH STEP

Set up unrevealing condition.

Unrevealing Felwood
Events
Unit - A unit Begins construction

Conditions
(Unit-type of (Constructing structure)) Equal to Corrupted Tree of Life

Actions
For each (Integer A) from 0 to 1, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Race of Slot[(Integer A)]) Equal to Night Elf

(Slot[(Integer A)] handicap) Equal to 80.00

(Owner of (Constructing structure)) Equal to Slot[(Integer A)]

(Number of units in (Units in Reveal[(Integer A)] owned by Slot[(Integer A)])) Equal to 0

(Number of living Corrupted Moon Well units owned by Slot[(Integer A)]) Greater than or equal to 0

Then - Actions
Unit Group - Pick every unit in (Units in Reveal[(Integer A)]) and do (Unit - Change ownership of (Picked unit) to Slot[(Integer A)] and Change color)

Set Food[(Integer A)] = ((Number of living Corrupted Moon Well units owned by Slot[(Integer A)]) x 10)

Player - Set Slot[(Integer A)] Food cap to Food[(Integer A)]

Else - Actions
Do nothing

ADDITIONAL INFO

If Player's worker unit begins Corrupted Tree of Life construction, then Tree of Life will become Player's unit,

Don't forget to reassign food cap value,

Food cap will increase start from reassign food cap value automatically, after you have finished food producing buildings,

F. SIXTH STEP (Last Step)

Set Night Elf
Events
Map initialization

Conditions

Actions
For each (Integer A) from 0 to 1, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Slot[(Integer A)] slot status) Equal to Is playing

(Race of Slot[(Integer A)]) Equal to Night Elf

Or - Any (Conditions) are true
Conditions
(Slot[(Integer A)] handicap) Equal to 100.00

(Slot[(Integer A)] handicap) Equal to 90.00

(Slot[(Integer A)] handicap) Equal to 70.00

(Slot[(Integer A)] handicap) Equal to 60.00

(Slot[(Integer A)] handicap) Equal to 50.00

Then - Actions
Melee Game - Create (Race of Slot[(Integer A)]) starting units for Slot[(Integer A)] at (Slot[(Integer A)] start location) (Include Heroes)

AI - Start melee AI script for Slot[(Integer A)]: elf.ai

Else - Actions
Do nothing

ADDITIONAL INFO

You must enable this trigger for your default race or it will be empty (in case you remove create starting units script from map initialization trigger),

Don't forget to edit the handicap (optional, if you create custom race selection via handicap),

There are several basic melee AI script in Warcraft III, the path are human.ai, undead.ai, elf.ai, and orc.ai (you can edit this section and insert your custom AI).

This is the end of tutorial section. Please give me feedback to improve this tutorial page. Thank you for your attention and I hope this tutorial useful for you. Good luck for your altered melee map.

#### Attachments

(8)GardenOfWarAMV08.w3x

 1.5 MB

 · Views: 139




 Last edited: Mar 30, 2020

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

There's an alternative proposed by @Retera a few years back. Less work even.

How to: Create a Full-working Custom Race

#### TUCANA

#### TUCANA

 Level 3

JoinedDec 5, 2017Messages11

Resources: 2

#### Resources
Maps 1Tutorials 1

Thank you for your super feedback. My tutorial is another alternative way to solve revealing problem via trigger. By the way, my tutorial can be applied for revealing and unrevealing trigger for Undead Player who used Possession ability (via Banshee) to control another peon unit type (like Peasant, Peon, Wisp, and custom worker). In this case, all of this applied tutorial trigger (derivative trigger) still work properly although all Acolyte and Undead Town Hall are dies and another town hall and peon unit type are still alive.

Actually, only second step (reveal region section), fourth step, and fifth step that are very essential for win-defeat condition, and for the rest it is optional. You can combine this tutorial trigger with another trigger (make derivative trigger), for example: make race selection via dialog.

You can also replace default town hall and worker with the custom race in map initialization section (by the way I don't like this one, because it will give you Blight terrain if you have select Undead race before). For additional info, if you replace default race with custom race in Object Editor, you don't need to create victory trigger (in case altered melee map), because the program will recognize your custom race.

I'm sorry, I don't understand Retera's JASS-script program, so I can't make any opinion about it, it's like assembly program for me. You are free to choose Retera method and other method if you find the program script is more efficient than this one. I want to upload my altered map (of course unprotected) for this tutorial example, as soon as possible.


 Last edited: Jan 14, 2018

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Where's the test map?

#### TUCANA

#### TUCANA

 Level 3

JoinedDec 5, 2017Messages11

Resources: 2

#### Resources
Maps 1Tutorials 1

This is my altered melee map for tutorial example (free to use/modify it's trigger):

(8) Garden of War AM

This map contains:

- Hero glow trigger,

- Custom terrain blight trigger (not replace any Undead blight texture),

- Custom birth animation trigger (not based on default birth animation).


 Last edited: Feb 10, 2018

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Cool tutorial, I like the layout. But I think this tutorial could be improved by making it more focused. For example, if someone is reading this tutorial, they don't really need to know parts like "set the food cap to 10" or "limit heroes to 1". The tutorial should give the minimal amount of triggers to get the system working. It is totally fine if the example map has extra triggers like that, but the tutorial itself should try to keep the triggers as small as possible.

Also, be sure to attach the test map in post #5 to the tutorial itself! That way people won't have to search through the thread to find the example.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Moving to graveyard in the interim. PM me if you'd like to submit the changes to get this approved!

#### Jhoannes19

#### Jhoannes19

 Level 2

JoinedMar 3, 2021Messages7

Hi, i'm new to modding but i am eager to know, so i want to ask you about you map and how you did things, because i am still trying to make a clue about triggers and how things work. I am trying to make a custom rac3 with some Lore, but i still have to get basics. Can you help with some explanation about #variables, #regions made, #reveal (what is it) etc etc and why you did that?

Ty very much in case you do!

 You must log in or register to reply here.


### Similar threads

B

[Mapping]
 Making a map with 4+ race


burnie

Jun 28, 2017

Tutorial Archive

Replies3Views2K

Jun 23, 2018
PurgeandFire

 Item Bonding


V2.

Mar 24, 2007

Tutorial Archive

Replies7Views3K

Jun 30, 2007
Wolverabid

 Casting Time System/Snippet


rodead

Sep 16, 2008

Tutorial Archive

Replies3Views2K

Sep 21, 2008
PurplePoot

Locked

[JASS]
 Town Hall not counting issue.


Darkholme

Mar 25, 2008

Triggers & Scripts

Replies3Views1K

Mar 25, 2008
Darkholme

 GUI Knockback System (Non-MUI, "PUI")


En_Fuego

Nov 9, 2008

Tutorial Archive

Replies6Views2K

Jan 3, 2009
Pyritie

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