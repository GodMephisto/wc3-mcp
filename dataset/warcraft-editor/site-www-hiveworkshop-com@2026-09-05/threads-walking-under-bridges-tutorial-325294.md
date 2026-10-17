---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/walking-under-bridges-tutorial.325294/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Walking Under Bridges Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/walking-under-bridges-tutorial.325294/>
#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


Walking under and over bridges!

 Have you ever wanted to make units able to walk under bridges? You can, with some limitations. I will show you how you can, with the use of pathing blockers and invisible platforms walk under and over bridges to get an extra level of depth in your maps.

Some things to keep in mind before I start the tutorial;

You have to use bridges that aren't pathable. If they are pathable it will not work.

It will only work in very specific scenarios as two units won't be able to stand under and over at the same time.

It can turn into a convoluted mess if your bridges are large, and you need to keep your triggers organized.

You can't order a unit to walk to an inaccessible place, so to make movement orders work properly you have to save the order of the unit, and then make it walk into the right regions with triggers. I won't show how to do that, but it's fairly simple.

The map with the example images can be found attached.

So to begin;

Step 1:

Choose where the bridge should be, and create the desired elevation.

Step 2:

Make a bridge of invisible platforms and elevate them to the desired height by using "CTRL + PgUp". To make them all be the same level simply press "Y".

Step 3:

Create two rows of pathing blockers on the sides of the bridge.

Step 4:

Create two destructible variables, and call them whatever. I called mine PBR_1/2 (for Pathing Blocker Row 1 and 2). Make sure your destructible variables are arrays to keep things organized.

Step 5:

Make a region that contains all the invislbe platforms and pathing blockers.

Step 6:

Make a trigger that runs on map initialization where you kill all the invisible platforms inside the region, and then you also kill the pathing blockers that blocks the path (the ones marked in blue in the screenshot). You do this to "disable" the bridge at initialization, and so you can create the correct variables for later use.

My trigger looks like this:

Melee Initialization
Events
Map initialization

Conditions

Actions
Destructible - Pick every destructible in Region 000 <gen> and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Destructible-type of (Picked destructible)) Equal to Invisible Platform (small)

Then - Actions
Destructible - Kill (Picked destructible)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Picked destructible) Equal to Pathing Blocker (Ground) 0073 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0074 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0075 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0076 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0077 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0078 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0092 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0091 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0090 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0089 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0088 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0087 <gen>

Then - Actions
Destructible - Kill (Picked destructible)

Else - Actions

Step 7:

In the same initialization trigger you have to create new pathing blockers on the dead invisible platforms locations (only the ones marked in blue in the picture below) to make sure you're properly blocking when the bridge is "dead" (if you don't do this you would be able to walk inside the "base" of the bridge when it's dead). Then you have to save the newly created pathing blockers to variables. Two destructibles can't be "on top" of each other unless one of them is dead.

The trigger would look like this (I haven't bothered with leak cleaning simply for readability):

Melee Initialization
Events
Map initialization

Conditions

Actions
Destructible - Pick every destructible in Region 000 <gen> and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Destructible-type of (Picked destructible)) Equal to Invisible Platform (small)

Then - Actions
Destructible - Kill (Picked destructible)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Picked destructible) Equal to Pathing Blocker (Ground) 0073 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0074 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0075 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0076 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0077 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0078 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0092 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0091 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0090 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0089 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0088 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0087 <gen>

Then - Actions
Destructible - Kill (Picked destructible)

Else - Actions

-------- Row 1 --------

Destructible - Create a Pathing Blocker (Ground) at (Position of Invisible Platform (small) 0065 <gen>) facing (Random angle) with scale 1.00 and variation 0

Set PBR_1[1] = (Last created destructible)

Destructible - Create a Pathing Blocker (Ground) at (Position of Invisible Platform (small) 0051 <gen>) facing (Random angle) with scale 1.00 and variation 0

Set PBR_1[2] = (Last created destructible)

Destructible - Create a Pathing Blocker (Ground) at (Position of Invisible Platform (small) 0037 <gen>) facing (Random angle) with scale 1.00 and variation 0

Set PBR_1[3] = (Last created destructible)

Destructible - Create a Pathing Blocker (Ground) at (Position of Invisible Platform (small) 0023 <gen>) facing (Random angle) with scale 1.00 and variation 0

Set PBR_1[4] = (Last created destructible)

Destructible - Create a Pathing Blocker (Ground) at (Position of Invisible Platform (small) 0009 <gen>) facing (Random angle) with scale 1.00 and variation 0

Set PBR_1[5] = (Last created destructible)

-------- Row 2 --------

Destructible - Create a Pathing Blocker (Ground) at (Position of Invisible Platform (small) 0058 <gen>) facing (Random angle) with scale 1.00 and variation 0

Set PBR_2[1] = (Last created destructible)

Destructible - Create a Pathing Blocker (Ground) at (Position of Invisible Platform (small) 0044 <gen>) facing (Random angle) with scale 1.00 and variation 0

Set PBR_2[2] = (Last created destructible)

Destructible - Create a Pathing Blocker (Ground) at (Position of Invisible Platform (small) 0030 <gen>) facing (Random angle) with scale 1.00 and variation 0

Set PBR_2[3] = (Last created destructible)

Destructible - Create a Pathing Blocker (Ground) at (Position of Invisible Platform (small) 0016 <gen>) facing (Random angle) with scale 1.00 and variation 0

Set PBR_2[4] = (Last created destructible)

Destructible - Create a Pathing Blocker (Ground) at (Position of Invisible Platform (small) 0002 <gen>) facing (Random angle) with scale 1.00 and variation 0

Set PBR_2[5] = (Last created destructible)

Step 8:

Now the bridge is dead, and all the initial settings are made. Now we have to make the regions that "activates" and "deactivates" the bridge.

Step 9:

The only remaining thing is to make the triggers to activate and deactivate the bridge when walking into the regions.

Make two triggers. One to activate the bridge and one to deactivate the bridge. The one to deactivate the bridge should be turned off initially.

Let's make the one to activate the bridge first. Basically you kill the pathing blocker variables so you can pass over the bridge, and then you resurrect all the invisible platforms so you can also walk over it. Then you resurrect the pathing blockers on the sides (marked in blue) so you can't walk "off" the bridge.

Also remember to turn off the trigger, and turn on the trigger that deactivates the bridge.

Activate Bridge
Events
Unit - A unit enters Region 001 <gen>

Conditions

Actions
Trigger - Turn off (This trigger)

Trigger - Turn on Deactivate Bridge <gen>

Destructible - Kill PBR_1[1]

Destructible - Kill PBR_1[2]

Destructible - Kill PBR_1[3]

Destructible - Kill PBR_1[4]

Destructible - Kill PBR_1[5]

Destructible - Kill PBR_2[1]

Destructible - Kill PBR_2[2]

Destructible - Kill PBR_2[3]

Destructible - Kill PBR_2[4]

Destructible - Kill PBR_2[5]

Destructible - Pick every destructible in Region 000 <gen> and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Destructible-type of (Picked destructible)) Equal to Invisible Platform (small)

Then - Actions
Destructible - Resurrect (Picked destructible) with (Max life of (Picked destructible)) life and Hide birth animation

Else - Actions

Destructible - Resurrect Pathing Blocker (Ground) 0092 <gen> with (Max life of Pathing Blocker (Ground) 0092 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0091 <gen> with (Max life of Pathing Blocker (Ground) 0091 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0090 <gen> with (Max life of Pathing Blocker (Ground) 0090 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0089 <gen> with (Max life of Pathing Blocker (Ground) 0089 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0088 <gen> with (Max life of Pathing Blocker (Ground) 0088 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0087 <gen> with (Max life of Pathing Blocker (Ground) 0087 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0078 <gen> with (Max life of Pathing Blocker (Ground) 0078 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0077 <gen> with (Max life of Pathing Blocker (Ground) 0077 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0076 <gen> with (Max life of Pathing Blocker (Ground) 0076 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0075 <gen> with (Max life of Pathing Blocker (Ground) 0075 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0074 <gen> with (Max life of Pathing Blocker (Ground) 0074 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0073 <gen> with (Max life of Pathing Blocker (Ground) 0073 <gen>) life and Hide birth animation

Step 10:

Now you have successfully activated the bridge, so now you have to deactivate it again when walking down from it. You basically follow the same steps as in the initialization trigger, but instead of creating new pathing blockers you now simply resurrect the ones already made (remember to resurrect them AFTER you kill all the invisible platforms).

Deactivate Bridge
Events
Unit - A unit enters Region 002 <gen>

Conditions

Actions
Trigger - Turn off (This trigger)

Trigger - Turn on Activate Bridge <gen>

Destructible - Pick every destructible in Region 000 <gen> and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Destructible-type of (Picked destructible)) Equal to Invisible Platform (small)

Then - Actions
Destructible - Kill (Picked destructible)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Picked destructible) Equal to Pathing Blocker (Ground) 0073 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0074 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0075 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0076 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0077 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0078 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0092 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0091 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0090 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0089 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0088 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0087 <gen>

Then - Actions
Destructible - Kill (Picked destructible)

Else - Actions

-------- Row 1 --------

Destructible - Resurrect PBR_1[1] with (Max life of PBR_1[1]) life and Hide birth animation

Destructible - Resurrect PBR_1[2] with (Max life of PBR_1[2]) life and Hide birth animation

Destructible - Resurrect PBR_1[3] with (Max life of PBR_1[3]) life and Hide birth animation

Destructible - Resurrect PBR_1[4] with (Max life of PBR_1[4]) life and Hide birth animation

Destructible - Resurrect PBR_1[5] with (Max life of PBR_1[5]) life and Hide birth animation

-------- Row 2 --------

Destructible - Resurrect PBR_2[1] with (Max life of PBR_2[1]) life and Hide birth animation

Destructible - Resurrect PBR_2[2] with (Max life of PBR_2[2]) life and Hide birth animation

Destructible - Resurrect PBR_2[3] with (Max life of PBR_2[3]) life and Hide birth animation

Destructible - Resurrect PBR_2[4] with (Max life of PBR_2[4]) life and Hide birth animation

Destructible - Resurrect PBR_2[5] with (Max life of PBR_2[5]) life and Hide birth animation

Here's the final result and an example of how I have used it in a map I am working on:

#### Attachments

Pathing.w3x

 21.4 KB

 · Views: 204




 Last edited: Oct 5, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I was going through the tutorial steps and everything was nice and logical until step 6-7.

Why kill all of the invisible platforms? as I understand it, you needed to kill some of them in order to stack pathing blockers on top. But why all the others?

As stated, this has a lot of flaws which is why am hesitant to approve it.

There is one thing I could think of that makes it slightly less flawed.

Using getlocalplayer would possibly make it so you can ignore pathing blockers for one player only, aka players not on top of the bridge. But from memory I am not certain if that causes desyncs or not.

#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


Chaosy said:

 I was going through the tutorial steps and everything was nice and logical until step 6-7.

Why kill all of the invisible platforms? as I understand it, you needed to kill some of them in order to stack pathing blockers on top. But why all the others?

As stated, this has a lot of flaws which is why am hesitant to approve it.

There is one thing I could think of that makes it slightly less flawed.

Using getlocalplayer would possibly make it so you can ignore pathing blockers for one player only, aka players not on top of the bridge. But from memory I am not certain if that causes desyncs or not.


Click to expand...

I have updated it with some more information to try to explain why you need to kill the invisible platforms at initialization.

Also I'm not really sure how to use GetLocalPlayer in this context.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Right.

I made some changes to the triggers however.

I would have added an array for the pathing blockers but I was too lazy

We don't need 2 arrays for PBR, it just makes it more difficult to work with.

Life is a constant value set at map init so you dont have to get the max hp each time you ressurect

i is used for looping

init
Events
Map initialization

Conditions

Actions
Destructible - Pick every destructible in Region 000 <gen> and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Destructible-type of (Picked destructible)) Equal to Invisible Platform (small)

Then - Actions
Set VariableSet i = (i + 1)

Destructible - Create a Pathing Blocker (Ground) at (Position of (Picked destructible)) facing (Random angle) with scale 1.00 and variation 0

Set VariableSet life = (Max life of (Last created destructible))

Set VariableSet PBR[i] = (Last created destructible)

Destructible - Kill (Last created destructible)

Destructible - Kill (Picked destructible)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Picked destructible) Equal to Pathing Blocker (Ground) 0073 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0074 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0075 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0076 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0077 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0078 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0092 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0091 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0090 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0089 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0088 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0087 <gen>

Then - Actions
Destructible - Kill (Picked destructible)

Else - Actions

Activate Bridge
Events
Unit - A unit enters Region 001 <gen>

Conditions

Actions
Trigger - Turn off (This trigger)

Trigger - Turn on Deactivate Bridge <gen>

For each (Integer A) from 1 to i, do (Actions)
Loop - Actions
Destructible - Kill PBR[(Integer A)]

Destructible - Pick every destructible in Region 000 <gen> and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Destructible-type of (Picked destructible)) Equal to Invisible Platform (small)

Then - Actions
Destructible - Resurrect (Picked destructible) with (Max life of (Picked destructible)) life and Hide birth animation

Else - Actions

Destructible - Resurrect Pathing Blocker (Ground) 0092 <gen> with (Max life of Pathing Blocker (Ground) 0092 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0091 <gen> with (Max life of Pathing Blocker (Ground) 0091 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0090 <gen> with (Max life of Pathing Blocker (Ground) 0090 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0089 <gen> with (Max life of Pathing Blocker (Ground) 0089 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0088 <gen> with (Max life of Pathing Blocker (Ground) 0088 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0087 <gen> with (Max life of Pathing Blocker (Ground) 0087 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0078 <gen> with (Max life of Pathing Blocker (Ground) 0078 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0077 <gen> with (Max life of Pathing Blocker (Ground) 0077 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0076 <gen> with (Max life of Pathing Blocker (Ground) 0076 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0075 <gen> with (Max life of Pathing Blocker (Ground) 0075 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0074 <gen> with (Max life of Pathing Blocker (Ground) 0074 <gen>) life and Hide birth animation

Destructible - Resurrect Pathing Blocker (Ground) 0073 <gen> with (Max life of Pathing Blocker (Ground) 0073 <gen>) life and Hide birth animation

Deactivate Bridge
Events
Unit - A unit enters Region 002 <gen>

Conditions

Actions
Trigger - Turn off (This trigger)

Trigger - Turn on Activate Bridge <gen>

Destructible - Pick every destructible in Region 000 <gen> and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Destructible-type of (Picked destructible)) Equal to Invisible Platform (small)

(Picked destructible) Equal to Pathing Blocker (Ground) 0073 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0074 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0075 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0076 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0077 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0078 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0092 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0091 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0090 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0089 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0088 <gen>

(Picked destructible) Equal to Pathing Blocker (Ground) 0087 <gen>

Then - Actions
Destructible - Kill (Picked destructible)

Else - Actions

For each (Integer A) from 1 to 10, do (Actions)
Loop - Actions
Destructible - Resurrect PBR[(Integer A)] with life life and Hide birth animation

#### Attachments

Pathing_edited.w3x

 20.4 KB

 · Views: 156




 Last edited: Oct 5, 2020

 You must log in or register to reply here.


### Similar threads

 How to get flashy 90 degree cliffs using height modifier


AJalex

May 28, 2009

Tutorial Archive

Replies1Views2K

May 29, 2009
Pyritie

Locked

 Question about invisible platforms


Hashjie

Feb 23, 2012

World Editor Help Zone

Replies16Views11K

Feb 29, 2012
Zwiebelchen

Locked

 Bridges


Sephalo

Jun 19, 2008

Terrain & Level Design

Replies5Views3K

Jun 20, 2008
wet_water

Locked

 Trigger with destructibles


xgeneralthorx

Nov 6, 2012

World Editor Help Zone

Replies5Views4K

Nov 7, 2012
Hashjie

Locked

 Change move type through trigger


Death Adder

Nov 3, 2013

Triggers & Scripts

Replies1Views2K

Nov 3, 2013
Lender

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