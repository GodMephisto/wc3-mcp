---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/multiboards.6402/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Multiboards
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/multiboards.6402/>
#### Bob27

#### Bob27

 Level 24

JoinedJun 14, 2005Messages2,506

Resources: 12

#### Resources
Maps 6Tutorials 6

Introduction

Multiboards can be used for many things. They are used mostly to display information about the game, like for example how many kills each player has made. The difference between multiboards and leaderboards is that multiboards can have as many columns as you want and it works exactely like an Excel document.

Columns and Rows

In a multiboard there are Columns and Rows, obviously Rows going across (x) and Columns going up (y).

Multiboard Tutorial
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Multiboard - Create a multiboard with 4 columns and 2 rows, titled Kills

As in the trigger above when creating the multiboard is asks you for how many rows and columns you want.

Note: Multiboards can not be displayed or created using the event

Map initialization
so you should instead use

Time - Elapsed game time is 0.00 seconds
I have attached a picture that will show you all the columns are rows for the example multiboard I am using for this tutorial. R stands for Row and C stands for Column.

So using that picture we could figure out that C1 and R1 will be the box in the very top left corner. C4 and R2 would be the very bottom right box in the multiboard.

Colouring, Spacing, Filling In Your Spaces And Choosing If you Want to Have Icons And Text

Using the above chart we could use this trigger.

Multiboard Tutorial
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Multiboard - Set the icon for TutorialBoard item in column 2, row 2 to ReplaceableTextures\CommandButtons\BTNPillage.blp

to create an Icon in the 2nd column and the 2nd Row, which in our Demo Map is the 2 box at the very bottom of the multiboard.

Multiboard Tutorial
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Multiboard - Set the text for TutorialBoard item in column 1, row 1 to Text

The above trigger will make the word text appear in the space in the first column and first row.

Multiboard Tutorial
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Multiboard - Set the width for (Last created multiboard) item in column 1, row 1 to 3.00% of the total screen width

When you apply text to a space it might not be large enough, and will not show all of the text. Using this trigger that I showed you just above we can increase the width of a space, so it can fit in it and can be read propely.

Multiboard Tutorial
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Multiboard - Set the color for TutorialBoard item in column 1, row 1 to (100.00%, 80.00%, 20.00%) with 0.00% transparency

This above trigger can change the colour of the text, in a certain space. 100.00% for every colour field would give you a white colour, while a 0.00% for every colour field would give you black.

Showing the Multiboard

Multiboard - Show TutorialBoard
This action shows the multiboard.

Using the Multiboard

Now that we know how to set up a multiboard we can make it. This will show you how to make the multiboard show how many kills each player and players hero has.

Create
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Multiboard - Create a multiboard with 4 columns and 2 rows, titled Kills

Set TutorialBoard = (Last created multiboard)

Multiboard - Change the color of the title for TutorialBoard to (100.00%, 80.00%, 20.00%) with 0.00% transparency

Multiboard - Set the icon for TutorialBoard item in column 1, row 1 to ReplaceableTextures\CommandButtons\BTNFootman.blp

Multiboard - Set the icon for TutorialBoard item in column 1, row 2 to ReplaceableTextures\CommandButtons\BTNGrunt.blp

Multiboard - Set the icon for TutorialBoard item in column 2, row 1 to ReplaceableTextures\CommandButtons\BTNPillage.blp

Multiboard - Set the icon for TutorialBoard item in column 2, row 2 to ReplaceableTextures\CommandButtons\BTNPillage.blp

Multiboard - Set the width for TutorialBoard item in column 1, row 1 to 6.00% of the total screen width

Multiboard - Set the width for TutorialBoard item in column 1, row 2 to 6.00% of the total screen width

Multiboard - Set the text for (Last created multiboard) item in column 1, row 1 to Humans

Multiboard - Set the text for (Last created multiboard) item in column 1, row 2 to Orcs

Multiboard - Set the color for TutorialBoard item in column 1, row 1 to (30.00%, 30.00%, 20.00%) with 0.00% transparency

Multiboard - Set the color for TutorialBoard item in column 1, row 2 to (100.00%, 30.00%, 30.00%) with 0.00% transparency

Multiboard - Set the text for TutorialBoard item in column 2, row 1 to (String(HumanKills))

Multiboard - Set the text for TutorialBoard item in column 2, row 2 to (String(OrcKills))

Multiboard - Set the text for (Last created multiboard) item in column 3, row 1 to Human Hero

Multiboard - Set the text for (Last created multiboard) item in column 3, row 2 to Orc Hero

Multiboard - Set the color for TutorialBoard item in column 3, row 1 to (30.00%, 100.00%, 30.00%) with 0.00% transparency

Multiboard - Set the color for TutorialBoard item in column 3, row 2 to (100.00%, 100.00%, 30.00%) with 0.00% transparency

Multiboard - Set the width for TutorialBoard item in column 3, row 1 to 8.00% of the total screen width

Multiboard - Set the width for TutorialBoard item in column 3, row 2 to 8.00% of the total screen width

Multiboard - Set the icon for TutorialBoard item in column 3, row 1 to ReplaceableTextures\CommandButtons\BTNArthas.blp

Multiboard - Set the icon for TutorialBoard item in column 3, row 2 to ReplaceableTextures\CommandButtons\BTNThrall.blp

Multiboard - Set the icon for TutorialBoard item in column 4, row 1 to ReplaceableTextures\CommandButtons\BTNPillage.blp

Multiboard - Set the icon for TutorialBoard item in column 4, row 2 to ReplaceableTextures\CommandButtons\BTNPillage.blp

Multiboard - Set the text for (Last created multiboard) item in column 4, row 1 to (String(Human_Hero_Kills))

Multiboard - Set the text for (Last created multiboard) item in column 4, row 2 to (String(Orc_Hero_Kills))

Multiboard - Show TutorialBoard

That trigger has set up our multiboard, and we have set each "kills" space to a variable.

Kills
Events
Unit - A unit Dies

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Owner of (Triggering unit)) Equal to Player 1 (Red)

Then - Actions
Set OrcKills = (OrcKills + 1.00)

Multiboard - Set the text for TutorialBoard item in column 2, row 2 to (String(OrcKills))

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Owner of (Triggering unit)) Equal to Player 2 (Blue)

Then - Actions
Set HumanKills = (HumanKills + 1.00)

Multiboard - Set the text for TutorialBoard item in column 2, row 1 to (String(HumanKills))

Else - Actions
Do nothing

This trigger is used to add kills to the Player Kills catergory.

Hero Kills
Events
Unit - A unit Dies

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Killing unit) Equal to Human_Hero

Then - Actions
Set Human_Hero_Kills = (Human_Hero_Kills + 1.00)

Multiboard - Set the text for TutorialBoard item in column 4, row 1 to (String(Human_Hero_Kills))

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Killing unit) Equal to Orc_Hero

Then - Actions
Set Orc_Hero_Kills = (Orc_Hero_Kills + 1.00)

Multiboard - Set the text for TutorialBoard item in column 4, row 2 to (String(Orc_Hero_Kills))

Else - Actions
Do nothing

This trigger is almost exactly the same, but it now saves the hero kills, not any units kills.

Conclusion

If you have any questions you can email me at [email protected]. I have included a demo map so you can look at any of the triggers I used in this tutorial. The Demo can be downloaded Here.


 Last edited by a moderator: Oct 25, 2006

#### -JohnrheyCastor-

#### -JohnrheyCastor-

 Level 2

JoinedOct 1, 2013Messages9

the "The Demo can be downloaded Here". Doesn't work! can you give me a demo?

#### Marcus 小羊

#### Marcus 小羊

 Level 2

JoinedAug 29, 2019Messages15

I know it's super late to ask, but I'mma new dude that wanna learn and play warc3 back. So if hero was killed, the normal unit kills amount will also +1 right? Since you didn't use if (dying unit) equal to (Hero) equal to false, so if hero is dead, unit kills amount will also +1 is that right

#### Mistablockout

#### Mistablockout

 Level 4

JoinedNov 3, 2015Messages47

This tutorial could use an update with working example images.

#### awesem

#### awesem

 Level 1

JoinedJan 12, 2014Messages6

Marcus 小羊 said:

 I know it's super late to ask, but I'mma new dude that wanna learn and play warc3 back. So if hero was killed, the normal unit kills amount will also +1 right? Since you didn't use if (dying unit) equal to (Hero) equal to false, so if hero is dead, unit kills amount will also +1 is that right


Click to expand...

yes

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 Multiboard help!!


senlos

Nov 7, 2010

Triggers & Scripts

Replies12Views3K

Nov 9, 2010
senlos

Locked

 Multiboard Help


EpiK_Phail

Mar 15, 2010

World Editor Help Zone

Replies3Views2K

Mar 15, 2010
Bribe

Locked

 Multiboard help


Stelvio

Oct 31, 2009

World Editor Help Zone

Replies16Views3K

Nov 12, 2009
I34w90_238j

Locked

 MULTIBOARD Question


BagOfChips

Apr 24, 2010

World Editor Help Zone

Replies4Views2K

Apr 25, 2010
Masken

Locked

[Solved]
 Multiboard Width Issue


Kazeon

Oct 4, 2013

Triggers & Scripts

Replies7Views2K

Nov 1, 2013
Kazeon

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