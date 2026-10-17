---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/upwards-counting-timer.175541/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Upwards Counting Timer
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/upwards-counting-timer.175541/>
#### Laserdude

#### Laserdude

 Level 8

JoinedJun 30, 2010Messages259

Resources: 10

#### Resources
Maps 1Spells and Systems 7Tutorials 2

 Upwards Counting Timer


Okay, so in this tutorial, I am going to show you how to make a countdown timer that increases in time instead of decreasing. This type of timer can be used in several games such as survivals or in any map that displays game time.

 Requirements:

Knowledge about countdown timers

Knowledge about the trigger editor

Warcraft III World Editor

 The Variables

A Timer variable to be able to update the timer we are working with. Make it an array-variable if you need more then one timer. (For more than one player).

A Timer Window variable to remove leaks, if we need to. Again, make it an array-variable if you have more than one timer.

A Real variable to count seconds. This is the variable which lets us set what the timer should display. Make it an array-variable if you need more than one. (If you have more than one timer, you don't need to make this an array, if you are, for example, making a survival timer which stops when a unit dies, then you can simply turn off the trigger that makes that specific timer count. Otherwise, ff you have more than one trigger, make it an array)


Okay, I am naming my Timer variable "Time", my Timer Window variable "Time_Window", and my Real variable "Seconds".

 The Triggers


Okay, time to start triggering!


First things first, we need to make a Timer Window where the time is displayed.

Time Window
Events
Time - Elapsed game time is 1.00 seconds.

Conditions

Actions
Countdown Timer - Create a timer window for Time with title Time Elapsed:

Set Time_Window = (Last created timer window)

Now we have a timer window, but it is stuck at zero, so we need a trigger where each second updates the timer with a new number. It should look something like this:

Time Update
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Set Seconds = (Seconds + 1)

Countdown Timer - Start Time as a one-shot timer that will expire in Seconds seconds

Countdown Timer - Pause Time

That's it! Now you have an upward counting countdown timer.


If you wanted one timer for each player, make sure you set all your timers to update, and not only the first one.


Some things you might want to think about when making an upward counting countdown timer is that it CAN be used to trigger events.


For example, when one minute has passed, you want a tome to become available at the center of the map. Then, just add a new piece to the "Time Update" trigger.

Time Update
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Set Seconds = (Seconds + 1)

Countdown Timer - Start Time as a one-shot timer that will expire in Seconds seconds

Countdown Timer - Pause Time

If (All conditions are True) then do (Then actions) else do (Else actions)
If - Conditions
Seconds Equal to 60.00

Then - Actions
Set TomeLocation = Center of (Playable map area)

Item - Create Tome of Power at TomeLocation

Custom script: call RemoveLocation(udg_TomeLocation)

This can be used in almost any way. Use it wisely!


 Last edited by a moderator: Dec 11, 2010

#### Septimus

#### Septimus

 Level 31

JoinedMay 3, 2008Messages3,146

Resources: 50

#### Resources
Packs 1Maps 35Spells and Systems 8Tutorials 6

the time tome trigger ain't entirely necesary.

#### Laserdude

#### Laserdude

 Level 8

JoinedJun 30, 2010Messages259

Resources: 10

#### Resources
Maps 1Spells and Systems 7Tutorials 2

Yeah, when you mentioned it, I saw you were right. Edited the tutorial. Thanks for mentioning it, however, the other system is a better way to use when you create cinematics, for example a boss spawn.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Although there isn't too much explaining or anything, it is still a common topic that comes up and this tutorial qualifies for approval. I fixed the grammar mistakes, so ~Approved. =)

#### MacMardigan

#### MacMardigan

 Level 4

JoinedOct 28, 2014Messages37

Resources: 1

#### Resources
Models 1

Thank you for this

 You must log in or register to reply here.


### Similar threads

 Create a sucessfull timer with window (reuploaded)


Cheezeman

Sep 7, 2008

Trigger (GUI) Editor Tutorials

Replies27Views17K

Jan 8, 2021
impulse

 Create precise timing cinematic using real variable


Septimus

Apr 23, 2009

Trigger (GUI) Editor Tutorials

Replies17Views9K

Jun 27, 2009
Dem0n_Hunter

 Creating Levels in a Defense Map


eubz

Mar 15, 2014

Trigger (GUI) Editor Tutorials

Replies9Views6K

Apr 5, 2014
Daffa

Locked

 Count down timer triggers problem


Masashi

Aug 1, 2010

World Editor Help Zone

Replies4Views4K

Aug 1, 2010
Pharaoh_

Locked

 Alternating between 2 Timers


Mechcash

Sep 29, 2020

World Editor Help Zone

Replies6Views1K

Sep 30, 2020
Mechcash

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