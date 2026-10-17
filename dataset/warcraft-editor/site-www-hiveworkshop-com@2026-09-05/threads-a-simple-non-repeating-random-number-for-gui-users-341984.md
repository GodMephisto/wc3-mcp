---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/a-simple-non-repeating-random-number-for-gui-users.341984/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# A Simple Non Repeating Random Number for GUI Users
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/a-simple-non-repeating-random-number-for-gui-users.341984/>
#### Jamesbones

#### Jamesbones

 Level 6

JoinedAug 8, 2022Messages12

Resources: 4

#### Resources
Models 1Icons 1Skins 1Tutorials 1

Random Number
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Set Random_Number = (Random integer number between 1 and 10)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Removed_Number Equal to 9

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Check_Number[Random_Number] Not equal to 1

Then - Actions
Set Removed_Number = (Removed_Number + 1)

Set Check_Number[Random_Number] = 1

Game - Display to (All players) the text: (String(Random_Number))

Else - Actions
Trigger - Run (This trigger) (checking conditions)

This trigger randomize numbers between 1 and 10 (or more) without repeating.

You will need to create 3 integer variables = Random_Number, Removed_Number and Check_Number[with array].

*Note that in the condition that turn off the trigger, the 'Random Number' must be minus 1 of the total numbers you will need.

Hope it helps someone.

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

This is more of a "here's the trigger" solution type and not a tutorial at all. You need to explain the thought process and why it works that way.

#### Jamesbones

#### Jamesbones

 Level 6

JoinedAug 8, 2022Messages12

Resources: 4

#### Resources
Models 1Icons 1Skins 1Tutorials 1

Wrda said:

 This is more of a "here's the trigger" solution type and not a tutorial at all. You need to explain the thought process and why it works that way.


Click to expand...

You're right, this isn't meant to be here. Is there a section where this post can be moved? I'm really bad at explaining (and my mother tongue isn't English, which makes it even worse) and it will take some time to come up with a decent post. Maybe another time I can do that. I'll wait for your answer, if there's nothing you can do I'll delete it, no problem.

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

Well, this has been discussed many times on Hive already, but it could be useful to have a tutorial for it I suppose.

###
 [Solved] - Random - No Repeat


Is there a way to make a random number system that avoids certain numbers? So if it picks three integers, it won't pick the same integer number twice.

 www.hiveworkshop.com


###
 How to pick random integers without repetitions?


How to do it?

 www.hiveworkshop.com


#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


@Jamesbones There is the Small Code Snippets thread for vJASS and JASS. Maybe you could start a similar thread for GUI.

 You must log in or register to reply here.


### Similar threads

 Random Region Spawning Trigger


Ham Ham

Mar 18, 2007

Tutorial Archive

Replies3Views2K

Jul 6, 2007
Wolverabid

K

 Pick a Random Integer


kuyanutay

Nov 13, 2011

Tutorial Archive

Replies9Views3K

Dec 18, 2011
PurgeandFire

 Basic drop sistems


alhadrian

Aug 30, 2010

Tutorial Archive

Replies7Views3K

Dec 11, 2010
PurgeandFire

Locked

 Random Integer Generator


Starquizer

Jul 24, 2009

Triggers & Scripts

Replies3Views4K

Jul 24, 2009
Starquizer

Locked

[Solved]
 Move units to random region without repeats


Ninjaninus

Oct 23, 2024

World Editor Help Zone

Replies3Views783

Oct 23, 2024
Ninjaninus

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