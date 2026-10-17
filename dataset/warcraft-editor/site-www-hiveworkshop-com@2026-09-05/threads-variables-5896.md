---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/variables.5896/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Variables
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/variables.5896/>
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


Variables are places to store small amounts of data so you can use them later. They can for example be used for storing a players kill-counts, a players name, a type of unit, text-strings and dialogs.

Creating a variable


Most variables are made in the Variable Editor. To specify exactely how you want your variable you will have to set a type, a name, and a value.

Type


The type of the variable forms which data is stored in it. An integer is only numbers, a string is only text. Putting them together can mess up the game if you don't do it right.

Name


The name of the variable doesn't have to be anything specific. It's just a short string that helps you remember which variable stores what.

Value


The value of the variable is obvious, it's the value. If the variable is a Unit-Type then you will be able to set it to a unit-type. If it's an Integer, then set it to an integer.


The value can be set in the Variable Editor to a value but you can also just keep it as the default and then set the variable by triggers.

Boolean variables


Boolean variables can only store 2 different values: true or false. Boolean variables are mostly used in conditions like this:

Player1AutoBankingOn Equal to True

Arraying variables


Arrays are very useful to store many things in one variable. You can set the amount of arrays in the variable. I'd recommend just setting it to 9999 in the Variable Editor cause then yuo will have less problems trying to reach a high array.


To access the different values in the variable we use the variable[#] tag. So in our following example, Fruits[1] is Apples.


Let's use most unoriginal example. The one with fruits.

Untitled Trigger 001
Events
Map initialization

Conditions

Actions
Set Fruits[1] = Apples

Set FruitsAmount[1] = 30

Set Fruits[2] = Pears

Set FruitsAmount[2] = 43

Set Fruits[3] = Bananas

Set FruitsAmount[3] = 6

Set Fruits[4] = Mangos

Set FruitsAmount[4] = 8

Then we can make a trigger that tells you these data:

Untitled Trigger 002
Events
Time - Elapsed game time is 5.00 seconds

Conditions

Actions
Game - Display to (All players) the text: (Fruits[1] + (: + (String(FruitsAmount[1]))))

This trigger will output:

Apples: 30


We could also make our trigger by using the For Each Integer function:

Untitled Trigger 002
Events
Time - Elapsed game time is 5.00 seconds

Conditions

Actions
For each (Integer A) from 1 to 4, do (Game - Display to (All players) the text: (Fruits[(Integer A)] + (: + (String(FruitsAmount[(Integer A)])))))

What it does is that it counts from 1 to 4 and each time it replaces the A with the number it has counted. So if it has counted to 1, A will be 1, if it has counted to 2, A will be 2.


The following function does the same as the previous function, it's just faster and better.

Untitled Trigger 002
Events
Time - Elapsed game time is 5.00 seconds

Conditions

Actions
Game - Display to (All players) the text: (Fruits[1] + (: + (String(FruitsAmount[1]))))

Game - Display to (All players) the text: (Fruits[2] + (: + (String(FruitsAmount[2]))))

Game - Display to (All players) the text: (Fruits[3] + (: + (String(FruitsAmount[3]))))

Game - Display to (All players) the text: (Fruits[4] + (: + (String(FruitsAmount[4]))))

Using variables


Now.. What should I do with them?


Here is a few examples of what practical stuff you could use them for:

Untitled Trigger 001
Events
Unit - A unit Dies

Conditions

Actions
Set Kills[(Player number of (Owner of (Killing unit)))] = (Kills[(Player number of (Owner of (Killing unit)))] + 1)

A kill counter that each time a unit dies, it sets Kills[playernumber of owner of killing unit] and adds 1 to its value.


Then we can make a leaderboard with that:

Untitled Trigger 002
Events
Time - Elapsed game time is 0.01 seconds

Conditions

Actions
Leaderboard - Create a leaderboard for (All players) titled Kills

Player Group - Pick every player in (All players) and do (Leaderboard - Add (Picked player) to (Last created leaderboard) with label (Name of (Picked player)) and value Kills[(Player number of (Picked player))])

This one sets up our leaderboard for all players.

Untitled Trigger 001
Events
Unit - A unit Dies

Conditions

Actions
Set Kills[(Player number of (Owner of (Killing unit)))] = (Kills[(Player number of (Owner of (Killing unit)))] + 1)

Leaderboard - Change the value for (Owner of (Killing unit)) in (Last created leaderboard) to Kills[(Player number of (Owner of (Killing unit)))]

This one is our Unit Dies thing again. This time I added the Leaderboard thing which updates the persons kill-count.

Common variable types

Integer - Stores a whole number, and does not accept decimal points.

Real - Stores a floating point number, or a number with decimal points.

String - Stores a group of characters, or line of text.

Ability - Stores a type of ability, like Windwalk or Bloodlust.

Boolean - Stores a true or false statement.

Buff - Stores an type of buff, like slow or poision.

Camera - Object Stores a pre-placed camera object on your map.

Destructable - Stores a particular destructable, either pre-placed or created.

Destructable Type - Stores a type of destructable.

Defeat Condition - Can store the last defeat condition that you created.

Dialog - Can store the last dialog box that you created.

Dialog Button - Can store the last dialog button that you created.

Floating Text - Can store the last floating text that you created.

Game Cache - Can store the last came cache that you created.

Game Speed - Stores a a game speed value, either the current speed or another value.

Item - Stores a particular item that already exists in the map.

Item Class - Stores the class of an item.

Item Type - Stores the type of an item.

Leaderboard - Can store the last leaderboard you created. Handy for showing & hiding a leaderboard, or changing its values.

Multiboard - Can store the last multiboard you created.

Order Stores a unit order, like Attack or Hide.

Player - References a particular player.

Player Color - Stores a player color value, like red.

Player Group - Stores a group of players to be used again.

Point - Stores a X,Y coordinate on the map.

Quest - Stores the last quest you created.

Quest Requirement - Stores the last quest requirement you created.

Race - Stores a race, like Human or Night Elf.

Region - References a particular region on the map.

Sound - References a sound that you have used or imported, in the sound manager.

Special Effect - Stores the last special effect you created. Most commonly used for removing them.

Tech Type - Stores a type of research or upgrade.

Terrain Deformation - Stores the last terrain deformation you created.

Timer - Used for creating timers. Every timer has a timer variable. This stores the timers value.

Timer Window - Stores the last timer window you created. Good for using multiple timer windows.

Trigger - References a particular trigger in your map.

Unit - Stores a particular, single unit on your map.

Unit Group - Stores a group of units. Later then you can use it to refere to that whole group.

Unit Type - Stores a particular type of unit, like Footman or Sorceress.

Visibility Modifier - Stores the last visibiliy modifier you created.

Weather Effect - Stores the last weather effect you created.


 Last edited by a moderator: Mar 28, 2007

#### OverClocked

#### OverClocked

 Level 5

JoinedJul 16, 2017Messages65

Resources: 3

#### Resources
Maps 3

Guess I'll be the first here to comment

#### Juntacadaveres

#### Juntacadaveres

 Level 2

JoinedFeb 28, 2019Messages10

Then i Will be the second one.

#### Aegeuse

A

#### Aegeuse

Then I will be the third one then.

#### winter twilight

#### winter twilight

 Level 3

JoinedJun 9, 2019Messages27

which trigger increase the number of a integer variables ?i i think i saw it some where but i can't find it now

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Math - Arithmetic @winter twilight

#### KaiZen7th

#### KaiZen7th

 Level 11

JoinedMay 25, 2021Messages407

Resources: 3

#### Resources
Icons 1Spells and Systems 1Reforged HD Icons 1

What if I want to make a unit type (in my case: Sapphiron(living), able to be built from the Boneyard) to exist only one at a time (like the Mothership in SCII)?

P/s: Pardon my English if there is any mistake.

#### SenPie69

#### SenPie69

 Level 9

JoinedMar 26, 2020Messages220

Resources: 1

#### Resources
Maps 1

KaiZen7th said:

 What if I want to make a unit type (in my case: Sapphiron(living), able to be built from the Boneyard) to exist only one at a time (like the Mothership in SCII)?

P/s: Pardon my English if there is any mistake.


Click to expand...

^ this, i want to know too~

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

@KaiZen7th@im pinned you can use the following:

Player - Limit Training of Unit for Player

#### KaiZen7th

#### KaiZen7th

 Level 11

JoinedMay 25, 2021Messages407

Resources: 3

#### Resources
Icons 1Spells and Systems 1Reforged HD Icons 1

Thank you very much @Daffa

#### warpten090

#### warpten090

 Level 6

JoinedApr 4, 2010Messages67

Resources: 4

#### Resources
Skins 1Maps 2Spells and Systems 1

This tutorial is not working

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

warpten090 said:

 This tutorial is not working


Click to expand...

Define not working. The only goal of this tutorial is to teach people "what is a variable" and "how to make a variable". Provide your triggers else there's no way others can help.

 You must log in or register to reply here.


### Similar threads

Locked

 Leaderboards


Archian

Aug 10, 2006

Trigger (GUI) Editor Tutorials

Replies0Views40K

Aug 10, 2006
Archian

 Game Caches


Bob27

Mar 18, 2006

Trigger (GUI) Editor Tutorials

Replies34Views52K

Jul 21, 2018
PurgeandFire

 Chaosy's GUI tutorial for beginners


Chaosy

Apr 6, 2012

Trigger (GUI) Editor Tutorials

Replies9Views8K

Dec 25, 2016
Chaosy

 Creating Easy Multi-User Quests


The_Big_S

Aug 15, 2007

Trigger (GUI) Editor Tutorials

Replies33Views21K

Nov 17, 2007
PurplePoot

 Coloured Names


sPy

Dec 27, 2009

Trigger (GUI) Editor Tutorials

Replies19Views21K

Jan 26, 2011
jokerman10000

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