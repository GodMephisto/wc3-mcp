---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/basics-of-a-dialog.31304/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Basics of a Dialog
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/basics-of-a-dialog.31304/>
#### Orc_Tamer

#### Orc_Tamer

 Level 6

JoinedJan 5, 2007Messages40

Resources: 3

#### Resources
Models 1Tutorials 2

Basics of a Dialog


by ragingspeedhorn & Orc_Tamer


Note: In this tutorial we will show you how you create a simple multiply choice dialog for 1 player. Keep in mind that this is directed towards the beginners within the Trigger Editor and is in no way meant to teach the experienced modders anything that they probably didn't knew already.

1) What is a dialog?


A "Dialog" is a litle window that pops up which contains some buttons that you can click. An example of what a Dialog can be used for is a simple thing like if you would like to have a 100 gold. Instead of having the choice based on chat commands where you write -yes or -no it is (and looks) better to have a Dialog window apearing asking you if you would like to have a 100 gold and then having 2 buttons which says "Yes" or "No". We will now show you how you make a Dialog like I just told you about in the simpliest way (that we know of).

2) Getting Started.


To get started you should (ofcourse) open up the World Editor and enter the Trigger Editor (Hotkey for it is F4), the icon for it is located inbetween the Terrain Editor & the Sound Editor.

3) Creating The Variables.


Once you entered the Trigger Editor the first thing to do is finding the Variable Editor (Hotkey for it is Ctrl+B), the icon for it is located to the left of the New Category icon.


When you are inside the Variable Editor you click on the green x with a + infront of it  to create a new variable, then you make the two variables needed for the Dialog as shown below.

4) Creating The Dialog Window.


Once you made your two variables you can start with the actual triggers for getting the Dialog Window to apear.


First you create a new trigger and do as shown below.

The Dialog Window
Events
Time - Elapsed game time is 1.00 seconds

Conditions
None

Actions
Dialog - Clear MyDialog

Dialog - Change the title of MyDialog to Do you want a 100 gold?

Dialog - Create a dialog button for MyDialog labelled Yes

Set DialogButton[1] = (Last created dialog Button)

Dialog - Create a dialog button for MyDialog labelled No

Set DialogButton[2] = (Last created dialog Button)

Dialog - Show MyDialog for Player 1 (Red)

Once this is done the Dialog will apear perfectly but there will be no effect when clicking the buttons.

5) Creating The Buttons.


To make the buttons actually do something you create two triggers as shown below.

Yes Button
Events
Dialog - A dialog button is clicked for MyDialog

Conditions
(Clicked dialog button) Equal to DialogButton[1]

Actions
Player - Add 100 to Player 1 (Red) Current gold

Game - Display to Player Group - Player 1 (Red) the text: You have recieved 100 gold

No Button
Events
Dialog - A dialog button is clicked for MyDialog

Conditions
(Clicked dialog button) Equal to DialogButton[2]

Actions
Game - Display to Player Group - Player 1 (Red) the text: You didn't want any gold

Once these two triggers have been made your Dialog should work and look like shown below.


 Last edited by a moderator: Jun 26, 2007

#### BiroN

#### BiroN

 Level 8

JoinedSep 10, 2013Messages372

Simple and very useful. Thank you!

#### DarkRanger2

#### DarkRanger2

 Level 9

JoinedMar 2, 2017Messages232

Resources: 2

#### Resources
Maps 1Spells 1

Finally a simple Tuto that didn't pyt fire to my brain, thank you.

#### SSJ7107

#### SSJ7107

 Level 5

JoinedOct 16, 2015Messages132

Resources: 2

#### Resources
Maps 2

How can I remove a Dialogue via triggers

#### Jampion

#### Jampion

 Level 16

JoinedMar 25, 2016Messages1,327

Resources: 1

#### Resources
Spells and Systems 1

Removing title and all buttons:

Dialog - Clear MyDialog
If you want to destroy it, use:

 JASS:

call DialogDestroy(udg_MyDialog)

#### SSJ7107

#### SSJ7107

 Level 5

JoinedOct 16, 2015Messages132

Resources: 2

#### Resources
Maps 2

Only the JASS code works but still it's helpful THanks

#### tough_zai

#### tough_zai

 Level 7

JoinedNov 10, 2012Messages229

Resources: 2

#### Resources
Maps 2

Jampion said:

 Removing title and all buttons:

Dialog - Clear MyDialog
Should i add this to action below the game display??


Click to expand...

 You must log in or register to reply here.


### Similar threads

O

 Basics of a Dialog


Orc_Tamer

Jul 1, 2006

Tutorial Archive

Replies7Views4K

Mar 7, 2007
Wolverabid

 Basics of a Dialog


ragingspeedhorn

Jun 24, 2006

Tutorial Archive

Replies1Views2K

Mar 8, 2007
Wolverabid

Locked

 Problem with a trigger, Dialog.


Dave21

Mar 3, 2008

World Editor Help Zone

Replies5Views1K

Mar 4, 2008
Ardnived

Locked

[Trigger]
 Dialog buttons problem


Duke

Apr 14, 2008

Triggers & Scripts

Replies6Views2K

Apr 14, 2008
Duke

 Dialog Buttons (Advanced Systems)


-Kobas-

Jan 3, 2011

Trigger (GUI) Editor Tutorials

Replies15Views24K

Jan 30, 2024
Njord

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