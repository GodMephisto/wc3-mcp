---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-a-simple-hint-system-reviewed-ralle.83275/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making a Simple Hint System Reviewed Ralle
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-a-simple-hint-system-reviewed-ralle.83275/>
#### HamzahA

#### HamzahA

 Level 9

JoinedJul 24, 2007Messages308

Resources: 7

#### Resources
Maps 2Tutorials 5

Making a simple hint System


To do this simple system, you will need two triggers:


One in initialization, which must contain these actions:

Actions
Set RandomHint[1] = |cff32cd32Hint:|r Hint#1 text

Set RandomHint[2] = |cff32cd32Hint:|r Hint#2 text

Set RandomHint[3] = |cff32cd32Hint:|r Hint#3 text

Set RandomHint[4] = |cff32cd32Hint:|r Hint#4 text

Set RandomHint[5] = |cff32cd32Hint:|r Hint#5 text

Set RandomHint[6] = |cff32cd32Hint:|r Hint#6 text

Set RandomHint[7] = |cff32cd32Hint:|r Hint#7 text

Set RandomHint[8] = |cff32cd32Hint:|r Hint#8 text

Set RandomHint[9] = |cff32cd32Hint:|r Hint#9 text

Explain what the trigger does, and what you can edit:


This trigger basicly sets the hints which will be used, but it doesnt show the, because we didnt tell him to show them, we just told the trigger to make the texts ready before everything.


What you can edit:


"|cff32cd32Hint:|r" : This will be displayed like this Hint: you can edit the code to change the colour.


"Hint#X" : this is the content of each hint, X refers to the number of the hint.


note : the RandomHint[X] variable's array must be diffrent on each use.


You can continue to the number as much as you want, know that the variable "RandomHint" is an array string variable.


Now on another trigger, make this :

Hints
Events
Time - Every 10 seconds of game time

Conditions

Actions
Game - Display to (All players) the text: RandomHint[(Random integer number between 1 and 9)]

This trigger is the one which will display the hints, here i will explain you how it works:


First the function, "Game - Display to (All players) the text:" , this is the action that is used to show text for all players.


Second : the "RandomHint[(Random integer number between 1 and 9)]" : this picks a random hint number from the variable's array, and it is to be the chosen to be displayed.


Now test the game, then you will see that every 10 seconds a random hint will display.


Congratulations, you just made a hint system


Note: if you want a sound with the hint, just change the "Game - Display to (All players) the text:" to "Quest - Display the hint message" and use same text.


If you want a custom sound, you should use the "Sound - Play Sound" and refer to the sound that you want.


 Last edited: May 12, 2008

#### SkriK

#### SkriK

 Level 16

JoinedMar 26, 2004Messages569

Resources: 35

#### Resources
Icons 34Tutorials 1

You should add the Hint sound aswell, imo.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

This is definitely too short at the moment.

Suggestions for lengthening/improvement -

Show the reader how to enable/disable hints ingame (players can choose whether or not to see hints)

Show the reader how to make hints not be displayed twice

Explain what you are doing a bit more (especially with thes enew things)

Also, on an unrelated note, arrays start at [0], not [1].

#### HamzahA

#### HamzahA

 Level 9

JoinedJul 24, 2007Messages308

Resources: 7

#### Resources
Maps 2Tutorials 5

Done.


I have updated the tut


as the name explains, its a simple hint system )

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


Please use the new [h1][h2][h3] tags, read the The Hive Workshop - A Warcraft III Modding Site - Announcements in Forum : Tutorial Submission.

Also, I think the tutorial is a bit too easy. I dunno where the border goes.

#### BenZa

#### BenZa

 Level 3

JoinedJun 30, 2005Messages46

Simple tuts for the win!!

#### Eat_Bacon_Daily

#### Eat_Bacon_Daily

 Level 13

JoinedJan 4, 2014Messages558

Resources: 1

#### Resources
Maps 1

This is exactly what I was looking for!

 You must log in or register to reply here.


### Similar threads

Locked

[Solved]
 Enable/Disable some tips with cmd


AlwaysAndForever

Apr 2, 2019

Triggers & Scripts

Replies9Views1K

Apr 4, 2019
AlwaysAndForever

Locked

 GUI Votekick system crash


raft_pl

Nov 18, 2007

Triggers & Scripts

Replies6Views2K

Nov 20, 2007
raft_pl

Locked

 Problem with string commands


Rykon-V73

Apr 19, 2020

World Editor Help Zone

Replies6Views1K

Apr 19, 2020
Rykon-V73

Locked

[Trigger]
 Tiered Construction Mulitple Triggers Help


Lysho

Jul 3, 2015

Triggers & Scripts

Replies3Views1K

Jul 7, 2015
Lysho

 A simple non-repeating random number for GUI users.


Jamesbones

Sep 27, 2022

Tutorial Archive

Replies4Views2K

Sep 28, 2022
Elprede

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