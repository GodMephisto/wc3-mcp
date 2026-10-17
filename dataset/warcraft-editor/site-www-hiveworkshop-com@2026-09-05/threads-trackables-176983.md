---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/trackables.176983/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Trackables
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/trackables.176983/>
#### D4RK_G4ND4LF

#### D4RK_G4ND4LF

 Level 19

JoinedFeb 4, 2009Messages1,313

Resources: 20

#### Resources
Models 3Spells and Systems 15Tutorials 2

Trackables

I've seen many people who belive that one can't use trackables in GUI so here is a tutorial which shows how it is done.

What are trackables?

Trackables are models which can be placed on the map and are used to detect click- or mouse-hover-events. They can't be moved or destroyed. The function to create them is the following:

native CreateTrackable takes string trackableModelPath, real x, real y, real facing returns trackable

How to handle them

We start with creating a local variable for our trackable. This is necessary because we can't create global variables of type trackable with the variable editor.

Custom script: local trackable tr
Other variables are:

s: the path of the model (string)

x: the x-position (real)

y: the y-position (real)

z: the z-position (real)

a: the facing angle (real)

d: a destructable

p: a player

i/i2/i3: integers

Track_Table: a hashtable

To use global variables for custom script code you will have to add an "udg_" to them because warcraft renames them internally. So "x" becomes "udg_x" and "Set x = x + 1.00" becomes "set udg_x = udg_x + 1.00".

Next we create the hashtable to hold all the data about our trackables.

Hashtable - Create a hashtable

Set Track_Table = (Last created hashtable)
Then we setup the position in 3d-space.

Set x = 100.00

Set y = 200.00

Set z = 300.00
To apply a height to a trackable we create an invisible platform to put the trackable on it.

Custom script: set udg_d = CreateDestructableZ( 'OTip', udg_x, udg_y, udg_z, 0.00, 1, 0 )
After that we create the trackable.

Custom script: set tr = CreateTrackable(udg_s, udg_x, udg_y, udg_a)
There is no way to get any data of a trackable like GetTrackableX/Y/Z. A condition to check which player clicked the trackable is not given as well so we will have to find a way around it.

The first problem is easy to solve. We save all data of the trackable at creation and since it can't be destroyed or moved the data won't change. We will save the data in a hashtable and use the handle id of the trackable to find it again later.

Custom script: set udg_i = GetHandleId(tr)

Hashtable - Save x as 0 of i in Track_Table

Hashtable - Save y as 1 of i in Track_Table

Hashtable - Save z as 2 of i in Track_Table

Hashtable - Save s as 3 of i in Track_Table

Hashtable - Save Handle Ofp as 4 of i in Track_Table
The second problem is a little tricky. But since we can change the model of a trackable we can solve it as well. A trackable with no model can't be clicked so we will use GetLocalPlayer to set the model path for one player only. We will have to do it 12 times for every player and trackable in order to make it MPI.

Set s = units\human\Footman\Footman.mdl

Custom script: if udg_p != GetLocalPlayer() then

Set s = <Empty String>

Custom script: endif
Last we register our events. There are two of them. The first fires if a trackable is clicked:

Custom script: call TriggerRegisterTrackableHitEvent(gg_trg_Hit, tr)
The second fires if the mouse cursor is above it.

Custom script: call TriggerRegisterTrackableTrackEvent(gg_trg_Track, tr)
To register the event to a specific trigger you will have to add "gg_trg_" in front of the name of the trigger because warcraft (just like the "udg_" of global variables).

Ini
Events
Map initialization

Conditions

Actions
-------- Create a local variable for our trackable --------

Custom script: local trackable tr

-------- And one for a destructable which will be used to raise our trackable in the air --------

-------- Create a hashtable to save our trackables' data --------

Hashtable - Create a hashtable

Set Track_Table = (Last created hashtable)

-------- Facing angle of the trackables --------

Set a = 270.00

For each (Integer i2) from 1 to 50, do (Actions)
Loop - Actions
-------- X/Y/Z-position of our next trackable --------

-------- We will create a spiral of them just for fun --------

Set x = ((Cos((20.00 x (Real(i2))))) x 150.00)

Set y = ((Sin((20.00 x (Real(i2))))) x 150.00)

Set z = ((Real(i2)) x 15.00)

-------- Create 12 of them --------

-------- 1 for every player so it is MPI --------

-------- Usually you don't know which player clicked on it so we use this workaround --------

For each (Integer i3) from 1 to 12, do (Actions)
Loop - Actions
Set p = (Player(i3))

Set s = units\human\Footman\Footman.mdl

-------- Make model visible for 1 player only --------

Custom script: if udg_p != GetLocalPlayer() then

Set s = <Empty String>

Custom script: endif

-------- Create an invisible platform with z-height to put our trackable on --------

Custom script: set udg_d = CreateDestructableZ( 'OTip', udg_x, udg_y, udg_z, 0.00, 1, 0 )

-------- Create trackable --------

Custom script: set tr = CreateTrackable(udg_s, udg_x, udg_y, udg_a)

-------- Remove invisible platform --------

Custom script: call RemoveDestructable(udg_d)

-------- Register Click event --------

Custom script: call TriggerRegisterTrackableHitEvent(gg_trg_Hit, tr)

-------- Register Move event --------

Custom script: call TriggerRegisterTrackableTrackEvent(gg_trg_Track, tr)

-------- Get handle id of our trackable --------

Custom script: set udg_i = GetHandleId(tr)

-------- Save all data related to our trackable --------

Hashtable - Save x as 0 of i in Track_Table

Hashtable - Save y as 1 of i in Track_Table

Hashtable - Save z as 2 of i in Track_Table

Hashtable - Save s as 3 of i in Track_Table

Hashtable - Save Handle Ofp as 4 of i in Track_Table

-------- Null handle leaks --------

Custom script: set tr = null

Now our trackables are created. Now we only need to know how to load the data if a trackable is clicked.

Again we start with a local variable and save the triggering trackable in it.

Custom script: local trackable tr = GetTriggeringTrackable()
To retrive its data we get the handle id of the trackable and load it.

Custom script: set udg_i = GetHandleId(tr)

Set x = (Load 0 of i from Track_Table)

Set y = (Load 1 of i from Track_Table)

Set z = (Load 2 of i from Track_Table)

Set s = (Load 3 of i from Track_Table)

Set p = (Load 4 of i in Track_Table)

Hit
Events

Conditions

Actions
-------- Get trackable --------

Custom script: local trackable tr = GetTriggeringTrackable()

-------- Get its handle it --------

Custom script: set udg_i = GetHandleId(tr)

-------- Load data associated with it --------

Set x = (Load 0 of i from Track_Table)

Set y = (Load 1 of i from Track_Table)

Set z = (Load 2 of i from Track_Table)

Set s = (Load 3 of i from Track_Table)

Set p = (Load 4 of i in Track_Table)

-------- Clear text messages --------

Cinematic - Clear the screen of text messages for (All players)

-------- Display data --------

Game - Display to (All players) the text: Event type: Hit

Game - Display to (All players) the text: (X: + (String(x)))

Game - Display to (All players) the text: (Y: + (String(y)))

Game - Display to (All players) the text: (Z: + (String(z)))

Game - Display to (All players) the text: (Model path: + s)

Game - Display to (All players) the text: (Triggering player: + (Name of p))

Custom script: set tr = null

That's it! Now you can use trackables in GUI! Have fun with them

#### Attachments

Trackables.w3x

 15.1 KB

 · Views: 901




 Last edited by a moderator: Oct 4, 2012

#### Cihparg

#### Cihparg

 Level 16

JoinedJul 4, 2008Messages1,106

Resources: 9

#### Resources
Models 1Icons 5Maps 2Spells and Systems 1

This sure will be useful!

#### GhostThruster

#### GhostThruster

 Level 42

JoinedJul 24, 2009Messages5,688

Resources: 77

#### Resources
Models 14Icons 50Packs 1Skins 3Maps 6Spells and Systems 3

Wowwwwwwww... Awesome, +rep. I think it could come in handy for my inventory.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice tutorial. Great for GUI users who want to incorporate some of JASS's great features into their own map.

Although:

Set s = <Empty String>

Custom script: if udg_p == GetLocalPlayer() then

Set s = units\human\Footman\Footman.mdl

Custom script: endif
You might want to change that to this:

Set s = units\human\Footman\Footman.mdl

Custom script: if GetLocalPlayer() != udg_p then

Set s = ""

Custom script: endif
This way, the string table stays synched with all players in case the "s" local string wasn't already entered into the table.

Basically, once you declare it, it will enter it into the table. Then it will set the s locally to "" (null string) for everyone else, which is already in the string table. The first method would have set it to the null string, which is fine, but then it would set it to some string locally that might not be in the string table for everyone else.

~Approved, you might want to update for that though.

EDIT: Updated tutorial. There was a little mistake in the code (a missing exclamation point in the condition). It should work properly now.


 Last edited: Oct 4, 2012

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

This needs an update, the current method from the uploaded map doesn't work because someone edited it I think and didn't know to change the player grabber.

Here is how to fix it if anyone is interested.

[trigger=]

Ini

 Events

 Map initialization

 Conditions

 Actions

 -------- Create a local variable for our trackable --------

 Custom script: local trackable tr

 -------- And one for a destructable which will be used to raise our trackable in the air --------

 -------- Create a hashtable to save our trackables' data --------

 Hashtable - Create a hashtable

 Set Track_Table = (Last created hashtable)

 Set s = units\human\Footman\Footman.mdl

 -------- Facing angle of the trackables --------

 Set a = 270.00

 For each (Integer i2) from 1 to 50, do (Actions)

 Loop - Actions

 -------- X/Y/Z-position of our next trackable --------

 -------- We will create a spiral of them just for fun --------

 Set x = ((Cos((20.00 x (Real(i2))))) x 150.00)

 Set y = ((Sin((20.00 x (Real(i2))))) x 150.00)

 Set z = ((Real(i2)) x 15.00)

 -------- Create 12 of them --------

 -------- 1 for every player so it is MPI --------

 -------- Usually you don't know which player clicked on it so we use this workaround --------

 For each (Integer i3) from 1 to 12, do (Actions)

 Loop - Actions

 Set p = (Player(i3))

 Set s = <Empty String>

 -------- Make model visible for 1 player only --------

 Custom script: if udg_p == GetLocalPlayer() then

 Set s = units\human\Footman\Footman.mdl

 Custom script: endif

 -------- Create an invisible platform with z-height to put our trackable on --------

 Custom script: set udg_d = CreateDestructableZ( 'OTip', udg_x, udg_y, udg_z, 0.00, 1, 0 )

 -------- Create trackable --------

 Custom script: set tr = CreateTrackable(udg_s, udg_x, udg_y, udg_a)

 -------- Remove invisible platform --------

 Custom script: call RemoveDestructable(udg_d)

 -------- Register Click event --------

 Custom script: call TriggerRegisterTrackableHitEvent(gg_trg_Hit, tr)

 -------- Register Move event --------

 Custom script: call TriggerRegisterTrackableTrackEvent(gg_trg_Track, tr)

 -------- Get handle id of our trackable --------

 Custom script: set udg_i = GetHandleId(tr)

 -------- Save all data related to our trackable --------

 Hashtable - Save x as 0 of i in Track_Table

 Hashtable - Save y as 1 of i in Track_Table

 Hashtable - Save z as 2 of i in Track_Table

 Hashtable - Save s as 3 of i in Track_Table

 Hashtable - Save Handle Ofp as 4 of i in Track_Table

 -------- Null handle leaks --------

 Custom script: set tr = null

[/trigger]

If you don't do it that way then you have 12 trackables in every created spot instead of only one.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I actually used this method a month or two ago, worked fine for me. I only played around with it in single player though.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Chaosy said:

 I actually used this method a month or two ago, worked fine for me. I only played around with it in single player though.


Click to expand...

It allows you to select another player's trackables. . . You should see it in single player too.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Well I only created trackables for myself  (player 1)


that'd explain why I didn't notice.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Chaosy said:

 Well I only created trackables for myself  (player 1)


that'd explain why I didn't notice.


Click to expand...

 That'd be why.

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 Trackable grid not being created


Tal'Rarity

Sep 4, 2016

Triggers & Scripts

Replies3Views2K

Sep 4, 2016
Tal'Rarity

Locked

[Solved]
 Trackables not being created


Blazing_Legion

Jul 20, 2013

Triggers & Scripts

Replies2Views1K

Jul 22, 2013
Blazing_Legion

Locked

[vJASS]
 Mpi trackables?


dardas

May 17, 2011

Triggers & Scripts

Replies2Views1K

May 18, 2011
dardas

Locked

[Solved]
 Another local variable problem


Blazing_Legion

Aug 2, 2012

Triggers & Scripts

Replies5Views2K

Aug 3, 2012
gorillabull

T

Locked

[Trigger]
 TriggerRegisterTrackableHit/TrackEvent?


TwilightSoul

Jan 30, 2015

Triggers & Scripts

Replies2Views1K

Jan 30, 2015
TwilightSoul

T

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