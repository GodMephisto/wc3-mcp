---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-a-full-td-reviewed-ralle.88620/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making a Full Td Reviewed Ralle
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-a-full-td-reviewed-ralle.88620/>
#### Ostaph13

#### Ostaph13

 Level 4

JoinedJun 27, 2008Messages15

Resources: 2

#### Resources
Tutorials 2

First Thoughts


This is my recommendation if you want to make a new innovative map that everyone wants to play......


-Don't make something that has been done before!


(Example) A Maul such as Wintermaul.


(Why Not?) It has been overused so many times before, and these days you rarely see a maul anymore because everyone has played it and gotten bored of it.


-Make something new that features nice qualities, and effects.


(Example) Element TD.


(Why?) It features a new system to make towers and fuse elements to upgrade towers. Its hard to get bored of because there are so many possibilities!


-Make something that you created on your own and didn't steal then edit.


(Why??) I know its a lot faster just taking a already made map with levels, and such, but then you don't get the experience of working with triggers, and finding out ways to make a great TD!


-Always protect your map so people can't steal it!


When you protect your map people won't be able to take it apart and recreate a ripped off version of it!


-Always go to www.hiveworkshop.com if your stuck on something that you don't know how to do.


(Why?) Hiveworkshop includes many great tutorials on helping you with specific details on triggering, terraining, and editing!


_________________________________________________________________

Map Start

Step 1:Go to trigger editor. Click on the Initialization folder, and delete it. Make a new folder called Game Start, and make a new trigger called Game Start. Make the Trigger like this

Game Start

Events:

Map Initialization

Conditions:

None

Actions:

Melee Game - Use melee time of day (for all players)
You can also make sounds, and text for the start of your map to do that make a trigger like this.

Game Welcome

Events:

Elapsed Time is 0.10 seconds

Conditions:

None

Actions:

Game - Display to (All Players) the text: Whatever you want to say
Then go to the sound editor and pick out a cool sound that you want to play when you do make a trigger like this.

Game sound

Events:

Elapsed Time is 0.10 seconds

Conditions:

None

Actions:

Sound - Stop Music immediately

Sound - Play Whatever Sound you like
And you have created a good start to your TD.


_________________________________________________________________

Pathing


Alright kids lets start your TD! Its extremely simple. I'll guide you through simple steps!

Step 1:Make a basic, or complex area where creeps pass through and towers are placed! You shouldn't need help on this step just use your imagination!

Step 2:Now lets work on pathing for the creeps to go.


Go Layer then Regions. You will see a region palette on your top right side of the world editor. Click add region. Then make a region for where the creeps will spawn. Call it whatever you like I prefer creepspawn.

Step 3:Now make a region for every turn you have and just name them things like Path 2, Path 3, Path 4 and so on. This will be the route that the creeps will take to get to the end.

Step 4:Now make an end region where you will lose a life when a creep enters there. Call it whatever you like. Then Make a region surrounding the whole area where Player 1(Red) Plays, or where every player plays and name it Whole Red, or Whole Players.

Step 5(Triggering):Get ready for some really easy triggering I'll guide you through every step just incase. First lets just say my Td has 4 turns in it so it will have 6 regions. Start, T1, T2, T3, T4, End. This will be the triggering. Yours will probably be simpler but you will understand the concept easily!

Trigger1:StarttoT1

Events:

Unit - Unit Enters Region(Start)

Conditions:

NONE

Actions:

Unit - Order (Entering unit) to Move To (Center of T1)

T1toT2

Events:

Unit - Unit Enters Region(T1)

Conditions:

None

Actions:

Unit - Order (Entering unit) to Move To (Center of T2)

T2toT3

Events:

Unit - Unit Enters Region(T2)

Conditions:

None

Actions:

Unit - Order (Entering unit) to Move To (Center of T3)

T3toT4

Events:

Unit - Unit Enters Region(T3)

Conditions:

None

Actions:

Unit - Order (Entering unit) To Move To (Center of T4)

T4toEnd

Events:

Unit - Unit Enters Region(T4)

Conditions:

None

Actions:

Unit - Order (Entering unit) To Move To (Center of End)

Step 6:All you have to do is that concept to connect all of your regions and you got the pathing completely correct! Well now that you've made pathing you probably want to test how it works right? O.K.!


Make a new trigger titled Test Level. And make it exactly like this!

Trigger2:Test Level

Events:

Time - Elapsed Time is 1.00 seconds

Conditions:

NONE

Actions:

Unit - Create 40 Footman for Player 12 (Brown) at (Center of Start Region)(Facing 0.00 degrees)
Step 7:Now test the map. If it doesn't work then recheck your work for any mistakes, and try again. If it did work than you got pathing easily down Good Job!


_________________________________________________________________

Lives

Step 1: Go to trigger editor, then go to variables. Its the yellow X. You will see a little box that should contain 3 categories. Variable Name, Variable Type, and Initial Value. Click on new variable its the green X. Set the Variable name to RedLives, and set the variable type to Integer. Make the initial value the amount of lives you want Red to start with, and then press Ok.

Step 2:Now lets setup a leaderboard containing the value of RedLives. Do this trigger Exactly!

Trigger3:Lives Leaderboard

Events:

Time - Elapsed game time is 0.30 seconds

Conditions:

Absolutely None!

Actions:

Leaderboard - Create a leaderboard for (All players) titled Player Lives

Leaderboard - Add Player 1 (Red) to (Last created leaderboard) with label Lives and value RedLives

Leaderboard - Show (Last created leaderboard)

Step 3: Now that you have created a leaderboard you probably want to see the lives value change. Well then make this trigger!

Trigger4:Lives Lost Red

Events:

Unit - A unit enters End Region

Conditions:

(Owner of (Entering unit)) Equal to Player 12 (Brown)

Actions:

Set RedLives = (RedLives - 1)

Unit - Remove (Entering unit) from the game

Leaderboard - Change the value for Player 1 (Red) in (Last created leaderboard) to RedLives

Game - Display to Player Group-Player 1 (Red) the text: 1 Life Lost!

Trigger - Run RedDefeat (checking conditions)
The Red Defeat Trigger....

Trigger5:RedDefeat

Events:

None

Conditions:

Lives less than or equal to 0

Actions:

Unit Group - Pick every unit in (Units in Whole Red Region) and do (Unit - Remove (Picked unit) from the game)

Game - Defeat Player 1 (Red) with the message: You Lose!
 Yay! Horray! You made a lives system, and a pathing system for Player 1(Red)!!!!


_________________________________________________________________

A time to Think and Discuss


Alright, now that you have finished making the terrain, pathing, and the lives feature its time to take a break and think about how you are going to continue your Tower Defense. Ask your self these following questions.


Question #1:


How successful do I want my Tower Defense to be?


Answer #1:


Well I'd recommend that you aim for the most successful possible because when you aim high you have a less chance of hitting low. As you now think that your TD can be really successful you need to know that you need really smooth, long, and nice triggering features. I'll provide you with that, but you need to work hard to earn a good Tower Defense.


Question #2:


How long am I willing to work on this Tower Defense?


Answer #2:


The answer is simple.... as long as you can. Don't work on it all day though you need to take breaks, go outside, hang out with friends. Thats where you can really get your ideas from... looking outside. There's plenty that nature provides us, and creativity is definitely one of them.


Final Question:


This isn't really a question but an answer. Don't think that you can't get help on your map. Ask your friends on Warcraft 3 or go to hiveworkshop if your stuck at any time. Of course this tutorial should provide you with all your TD needs there may be more, and that's a good thing. Now its time to resume work on what we have so far. Lets begin....


_________________________________________________________________

Levels!

Step 1:Finally you get to make your own levels right? Well before that we there's one thing that you have to make. Its a trigger that allows players to see the whole map. Its very important for a TD. Make a new folder, name it Visibility. Make a new Trigger and name it Visibility. Now do this.

Trigger6:Visibility

Events:

Time - Elapsed game time is 0.10 seconds

Conditions:

None

Actions:

Visibility - Disable Fog of War

Visibility - Disable Black Mask
Step 2:Alright now that you disabled the Fog of War, and the Black Mask its time to make a starting time for the beginning of the TD.


In the beginning you need to give people more time than in-between levels. So lets make a separate beginning timer. Make a new folder named TD Timers, and then make a new Trigger named beginning timer. Now go to variables, and make a new variable. Make the Variable name TDtimer, make the variable type a timer. Then press O.K.Now make a new variable. Name it Level. Make it an integer. And Make it equal to 0. Now make a trigger like this.

First timer

Events:

Time - Elapsed game time is 0.10 seconds

Conditions:

None

Actions:

Countdown Timer - Start TDTimer as a repeating timer that will expire in 1.00 seconds

Set BeginningTimer = (Last started timer)

Countdown Timer - Create a timer window for (Last started timer) with title First timer in...
Step 3:Now that you have your TDtimer you have to make the timer repeat for the rest of the game. Do this trigger.

Continuous Timer

Events:

Timer - TDTimer expires

Conditions:

None

Actions:

Countdown Timer - Destroy (Last created Timer Window)

Wait until ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True), checking every 1.00 seconds

Countdown Timer - Start TDTimer as a Repeating timer that will expire in 30.00 seconds

Set Level = (Level + 1)

Set TDTimer = (Last started timer)

Countdown Timer - Create a timer window for (Last started timer) with title Enemy Wave in...
Step 3: Alright now that you have the timer done, I will inform to you what it does. You see when you made a variable named level it equaled 0. But now every time that timer expires it will add 1 to the level variable so you will get a continuous flow of levels, but levels won't start until one levels creeps are all killed. Isn't that great? Of course it is.

Step 4:Delete your test level you don't need it anymore. Now lets make a couple of level triggers so you can understand how it works. Make a new folder named Levels. Then make a new trigger named level 1. Now make it like this.

Level 1

Events:

Time - TDTimer Expires

Conditions:

Level is equal to 1

Actions:

Unit - Create 15 Level1Units for Player 12 (Brown) at (Center of Start Region)
Of course you have to make a unit for level 1. Most likely you want to make it weak, and then the units will get stronger and stronger after each level. Lets now make level 2.

Level 2

Events:

Time - TDTimer Expires

Conditions:

Level is equal to 2

Actions:

Unit - Create 15 Level2Units for Player 12 (Brown) at (Center of Start Region)
Lets now make level 3 and after making level 3 you will probably be able to make all the rest of the levels on your own.

Level 3

Events:

Time - TDTimer Expires

Conditions:

Level is equal to 3

Actions:

Unit - Create 15 Level3Units for Player 12 (Brown) at (Center of Start Region)
There I showed you now how to make your first 3 levels remember to make different units for each one, and continue this level thing for as long as you would like. You can work on it whenever you want, but for now lets not make anymore levels and lets work on the next part of your TD.


_________________________________________________________________

Making Towers

Step 1:Alright now we are going to make some towers I'll show you how. Open up your object editor. Click on the new custom unit box. Make the Base units Race Human, and Melee. then choose the Guard Tower and press O.K.

Step 2:Alright on the top there's a thing called Abilities - Normal, remove all those abilities from it. Now change the icon and model file to whatever you would like. Then if you want change the tinting colors. Change the damage, and damage type, and change who it attacks to either ground enemy air, ground enemy, or air enemy so your tower can't attack other players towers. Change the Build time to 1 for Fast build, almost everyone in TD likes fastbuild on towers. Change the gold cost, and lumber cost. Remove Lumber Mill from the techtree requirements. Change the name and both basic and extended tooltips. And that's how to make a tower!

Step 3:Now lets make a guy that builds the towers. Go to object editor, then make a new object again. Keep it at human and melee. And pick peasant for the builder. Now for the Abilities remove Harvest(Gold and Lumber), and Call to Arms(Peasant), but DON'T REMOVE REPAIR. Change the icon and model if you would like. Make the Attack type None, and make the damage 0. So he can't attack. Go back to abilities and add invulnerability to him so he can't be destroyed. Then make his move speed 500, and move type fly. Change his fly height somewhere between 300-500. Remove all the upgrades used and structures built, and add your towers. Change his name and both tooltips.

Step 4:Now lets place the builder that you made on the map. Go to Layer and then Units. Go to human then custom. Then place the builder on the map. After you did you can continue making towers levels, and other features that you like.


_________________________________________________________________

Importing Models

Step 1:Importing models is an important thing to do in a TD, it can make your towers or creeps look better than before. To get a model first go to www.hiveworkshop.com and go the the Models page.

Step 2:Find a good model that suites what you are trying to make.

Step 3:Download the model and save it to a folder you will remember.

Step 4:Go on World editor and go to import manager. Click the Import File button and find the .mdx file of your downloaded model, and import it.

Step 5:Add the model to your selected unit, it will appear blank at first, but when you close and open world editor your model will fully load to your map.

Final Step:Don't get to many models because the more you have the longer it will take for your map to load!


_________________________________________________________________

Final Tips


-Don't Make certain Races/Towers overpowered


-Never make cheats to your game


-Protect your game


-Get help when needed


-Test your game all the time


-Focus and try not to make mistakes


-Make different types of levels like: Armored, Hero, Air, Ground, Multiplying ETC.


-Have fun!


 Last edited: Jun 27, 2008

#### Ostaph13

#### Ostaph13

 Level 4

JoinedJun 27, 2008Messages15

Resources: 2

#### Resources
Tutorials 2

This is my TD making Tutorial I will update it most of the time, and I hope you admins like it I'm a new User to this website, and Its great. PLEASE APPROVE!!!!

#### forzamoto

#### forzamoto

 Level 9

JoinedApr 14, 2008Messages191

Resources: 13

#### Resources
Maps 8Tutorials 5

Ostaph13 said:

 This is my TD making Tutorial I will update it most of the time, and I hope you admins like it I'm a new User to this website, and Its great. PLEASE APPROVE!!!!


Click to expand...


I guess its ok but it is to long and you will need to make it shorter. And alot


of the triggers will need fixing sorry.

2/5

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


Your triggers don't have the spaces from when you copied it from the Trigger Editor, this is required

Please use the new [h1][h2][h3] tags, read the The Hive Workshop - A Warcraft III Modding Site - Announcements in Forum : Tutorial Submission.

#### forzamoto

#### forzamoto

 Level 9

JoinedApr 14, 2008Messages191

Resources: 13

#### Resources
Maps 8Tutorials 5

I agree with you 100% there Ralle

#### Ostaph13

#### Ostaph13

 Level 4

JoinedJun 27, 2008Messages15

Resources: 2

#### Resources
Tutorials 2

Yea I'm going to update this tutorial as well as creating another one.

#### diabloste

#### diabloste

 Level 4

JoinedAug 25, 2007Messages43

This actually helped me alot.. Thanks! Even tho. I do exactly what you write, and theres still bugs in my TD. Either you could help me, or else it must go on being bugged >:l

#### Plokoon

P

#### Plokoon

Wait until ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True), checking every 1.00 seconds
How i do that?

#### Oziris

#### Oziris

 Level 27

JoinedOct 14, 2006Messages4,161

Resources: 7

#### Resources
Icons 1Skins 3StarCraft II Resources 1Tutorials 2

Yes, it's really hard to make this trigger:

Wait until ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True), checking every 1.00 seconds

If you cant tell me then at least post the map. :/

Edit: I found a bug when i was testing, or mybe i diden't do it right. When the creep comes to the END of the region the game exits to the desktop and Frozen Throne closes.

Edit: I fixed the problem, the game doesn't crash anymore.


 Last edited: Jan 3, 2009

#### SomeRandomDeleted Dr. Cabbage0

#### SomeRandomDeleted Dr. Cabbage0

 Level 9

JoinedJan 11, 2008Messages448

Resources: 3

#### Resources
Maps 3

Leaking trigger omfg.. srsly, you aren't joking with this tutorial? You are serious?

#### Kercyn

#### Kercyn

 Level 11

JoinedFeb 14, 2009Messages883

Plokoon said:

Wait until ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True), checking every 1.00 seconds
How i do that?


Click to expand...


 Oziris said:



 Yes, it's really hard to make this trigger


Click to expand...


This is very simple. Just go to the Object Editor and set every creep's "Stats - Food Cost" to 1. Then create a trigger that will look like this:

Events
Player - <Creep Controller>'s Food used becomes Equal to 0.00

Actions
<Wave cleared, spawn the next>

#### Catch_ya

#### Catch_ya

 Level 12

JoinedMay 21, 2009Messages984

Resources: 1

#### Resources
Spells and Systems 1

Leaks

 Trigger1:StarttoT1


 Events:


 Unit - Unit Enters Region(Start)


 Conditions:


 NONE


 Actions:


 Unit - Order (Entering unit) to Move To (Center of T1)


 T1toT2


 Events:


 Unit - Unit Enters Region(T1)


 Conditions:


 None


 Actions:


 Unit - Order (Entering unit) to Move To (Center of T2)


 T2toT3


 Events:


 Unit - Unit Enters Region(T2)


 Conditions:


 None


 Actions:


 Unit - Order (Entering unit) to Move To (Center of T3)


 T3toT4


 Events:


 Unit - Unit Enters Region(T3)


 Conditions:


 None


 Actions:


 Unit - Order (Entering unit) To Move To (Center of T4)


 T4toEnd


 Events:


 Unit - Unit Enters Region(T4)


 Conditions:


 None


 Actions:


 Unit - Order (Entering unit) To Move To (Center of End)


Click to expand...

Unit - Order (Entering unit) To Move To (Center of End) = Leak set end = (Center of End)


unit - Order (Entering unit) To Move To (Center of End)


call RemoveLocation(udg_end)

#### Slaydon

#### Slaydon

 Level 20

JoinedJan 6, 2008Messages2,622

Resources: 10

#### Resources
Icons 4Tutorials 6

Kercyn said:

 This is very simple. Just go to the Object Editor and set every creep's "Stats - Food Cost" to 1. Then create a trigger that will look like this:

Events
Player - <Creep Controller>'s Food used becomes Equal to 0.00

Actions
<Wave cleared, spawn the next>

Click to expand...


I dont think thats smart, isnt it better to just do this:

Events
A unit owned by player 12 dies.

Conditions

Actions
(Is there a variable for each level that you can insert in Conditions?)

If then else multi thing
If (it was lvl 1)

And
Number of Living Footmen? owned by player 12 equal to 0

Then
Show text

Start Level 2 Trigger

Else

For removing leaks? or?


EDIT: Catch_Ya posted while writing this

#### Kercyn

#### Kercyn

 Level 11

JoinedFeb 14, 2009Messages883

Slaydon said:

Events
A unit owned by player 12 dies.

Conditions

Actions
(Is there a variable for each level that you can insert in Conditions?)

If then else multi thing
If (it was lvl 1)

And
Number of Living Footmen? owned by player 12 equal to 0

Then
Show text

Start Level 2 Trigger

Else

Click to expand...

Huh? My trigger only has only one event, not multiple if/then/else statements. Why on earth should you confuse yourself with variables and multiple if/thens when you can replace that with just one event?

#### Slaydon

#### Slaydon

 Level 20

JoinedJan 6, 2008Messages2,622

Resources: 10

#### Resources
Icons 4Tutorials 6

Your trigger will spawn the same units every time then.

#### Kercyn

#### Kercyn

 Level 11

JoinedFeb 14, 2009Messages883

No, it won't. Not if the creator uses arrays.

#### Catch_ya

#### Catch_ya

 Level 12

JoinedMay 21, 2009Messages984

Resources: 1

#### Resources
Spells and Systems 1

you got these variables:


Level = integer (the current level of the waves)


Unit_Type = Unit-Type (The Unit type of being spawned)


Number = integer (The number of units spawned)


so


number of living (Unit_Type) owned by player 12 brown equal to 0

#### demonspy

#### demonspy

 Level 3

JoinedJun 21, 2008Messages38

Mm 3/5 becouse i dont understand very much .i thought it will be easier.

#### demonspy

#### demonspy

 Level 3

JoinedJun 21, 2008Messages38

Trigger4:Lives Lost Red


 Leaderboard - Change the value for Player 1 (Red) in (Last created leaderboard) to RedLives


HOW TO MAKE THIS ? OMG I DONT SEE THIS IN THE TRIGER ?

#### KEALOR

K

#### KEALOR

im makin a mini TD and my 2nd wave (giant Kobold name:that thing) doesnt work it goes bout halfway thru my maze then turns around & goes bak 2 start


sum1 plz help its pissin me off

#### Cr1mS0nWolF

C

#### Cr1mS0nWolF

Ostaph13 said:

 In the beginning you need to give people more time than in-between levels. So lets make a separate beginning timer. Make a new folder named TD Timers, and then make a new Trigger named beginning timer. Now go to variables, and make a new variable. Make the Variable name TDtimer, make the variable type a timer. Then press O.K.Now make a new variable. Name it Level. Make it an integer. And Make it equal to 0. Now make a trigger like this.

First timer

Events:

Time - Elapsed game time is 0.10 seconds

Conditions:

None

Actions:

Countdown Timer - Start TDTimer as a repeating timer that will expire in 1.00 seconds

Set BeginningTimer = (Last started timer) <<<<----- HERE!

Countdown Timer - Create a timer window for (Last started timer) with title First timer in...

Click to expand...


Just joined this website.  Btw when did you made that variable "BeginningTimer" ? If you look at it beginning timer is a trigger instead of a variable as you said there. So how come it is changed? Just an opinion, in tutorial, naming something by using double quotes will make it easier to see.

#### KEALOR

K

#### KEALOR

NVM I didnt figure out what it was but I fixed up by changin the object dunno Y it didnt work tho

#### gourgeon

G

#### gourgeon

nice tutorial, but what about if a unit owned by you go in, the unit go to the other location, its the only prob, you should another trigger to fix this !

#### Dr4c0

#### Dr4c0

 Level 4

JoinedJul 1, 2009Messages65

Just add the condition "entering unit owned by player 12"

#### sheep_monster2

S

#### sheep_monster2

omg

i got stucked at the Wait until ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True), checking every 1.00 seconds trigger and it looks like other do to can you plzz explain it a little more proper?

#### sendrak

S

#### sendrak

sheep_monster2 said:

 i got stucked at the Wait until ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True), checking every 1.00 seconds trigger and it looks like other do to can you plzz explain it a little more proper?


Click to expand...


I am stuck on the exact same trigger. It seems that the creator of this guide no longer visits this site though, as the last date that he logged onto this site was back in 2008.


Anywho, if there is anyone out there that can suggest a better way to set up level progression triggers, please say somthing about it

#### NoAmmoSrry

#### NoAmmoSrry

 Level 11

JoinedMay 2, 2009Messages650

#### NoAmmoSrry

#### NoAmmoSrry

 Level 11

JoinedMay 2, 2009Messages650

#### Editor_Noob

#### Editor_Noob

 Level 3

JoinedSep 22, 2009Messages43

Good day


I know the post is a bit dated, but perhaps still one reads it.


How should indicate so, I understand


 not.


"" Lives less than or equal to 0 ""


 Last edited: Oct 13, 2009

#### Mr.Chaos

#### Mr.Chaos

 Level 4

JoinedSep 2, 2009Messages60

Resources: 1

#### Resources
Maps 1

Upload an map because it many triggers I dont find. It would be easier if u upload an map!!

#### ZentineZ

Z

#### ZentineZ

How do I make so that the timer starts over again when all the creeps are dead? (Player 12 brown food 0 thing)

#### ZentineZ

Z

#### ZentineZ

The Level and timer system doesnt work for me :S

#### kimberker

#### kimberker

 Level 3

JoinedNov 8, 2009Messages46

Help i really really need ur help

Hi,

 PLS HELP ME!!!! I GOT PROBLEMS...  i cant find this...

Actions:

Set RedLives = (RedLives - 1) <----About this, the (RedLives - 1) how do u get there?

Conditions:

Lives less than or equal to 0

Actions:

Wait until ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True), checking every 1.00 seconds

Conditions:

Level is equal to 1 <--- from 1 to alot

Actions:

Unit - Create 15 Level1Units for Player 12 (Brown) at (Center of Start Region)

For that how to make Level1Units ???

And under levels... You wrote Make a new folder named TD Timers, and then make a new Trigger named beginning timer.

Making that trigger is for??

Trigger4:Lives Lost Red

Events:

Unit - A unit enters End Region

Conditions:

(Owner of (Entering unit)) Equal to Player 12 (Brown)

Actions:

Set RedLives = (RedLives - 1)

Unit - Remove (Entering unit) from the game

Leaderboard - Change the value for Player 1 (Red) in (Last created leaderboard) to RedLives

Game - Display to Player Group-Player 1 (Red) the text: 1 Life Lost!

Trigger - Run RedDefeat (checking conditions)

About that i dont understand... That is when Red lives is 0 or -1 of Red's lives ????

When 1 of my creep when to the end point, i still got 30 lives , it should -1 of my lives but my live in the leaderboard says -1 and i lost the game. Werid

How to add cold effect to a tower and when the tower attack the unit, how to make the unit have been slow. And for fire is burn the unit for a few second. And like when the unit takes more damage to nature and my nature tower attack that unit... it will take more dmg. how to do that???

And i my map got alot of problems...

I can send u the map to help me..But u must teach me too...


 Last edited: Dec 18, 2009

#### Slaydon

#### Slaydon

 Level 20

JoinedJan 6, 2008Messages2,622

Resources: 10

#### Resources
Icons 4Tutorials 6

kimberker said:

 Hi,


 PLS HELP ME!!!! I GOT PROBLEMS...  i cant find this...


And i my map got alot of problems...


I can send u the map to help me..But u must teach me too...


Click to expand...

 Make an Integer Variable, and then you use Set RedLives = (Value - 1) (ARITHMETIC)

Make another integer, using the "Condition" Integer

Use Wait for Condition

For each (Integer A) from 1 to 100, do (Actions)


 Loop - Actions


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 Levels Equal to (Integer A)


 Then - Actions


 Actions, this will only run once, since Levels is only set to 1 of the 100.


 Else - Actions

I would advice to use a Unit_Type Variable with Array, then you set it to 100, and create Unit - Create 15 Unit_Type_Variable[IntegerA] for Player 12 at Center of Start Position

Its so you know what the time is for that the new wave will come.

It will tell you what lives you got left, so it will set your life to lets say 99 if it was 100


Do you remember to reset the leaderboard/multiboard?

I would leave the red to someone else, not my strong side...

#### kimberker

#### kimberker

 Level 3

JoinedNov 8, 2009Messages46

I still dont get it

I dont get it... if u dont mind take a screen shot of it or make it a video... T_T its really hard to understand... i dont know where to get that... best is to help me like this... Make steps...

For ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True)

How to get that??? i in the wait for condition. den click condition den do what???

For each (Integer A) from 1 to 100, do (Actions)

Loop - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)

If - Conditions

Levels Equal to (Integer A)

Then - Actions

Actions, this will only run once, since Levels is only set to 1 of the 100.

Else - Actions

How to get that???

When i put Set Redlives -1 it means that red life is -1(negative 1)

And now when 1 of the creeps when to the end point. The leaderboard says -1(negative 1) and the game never ends...

Pls write in steps


 Last edited: Dec 19, 2009

#### Slaydon

#### Slaydon

 Level 20

JoinedJan 6, 2008Messages2,622

Resources: 10

#### Resources
Icons 4Tutorials 6

its in the boolean..., and press Search text... its Integer aswell..


the -1 is cuz you dont set a number of the lives, so they only got one, and then they will go negative...

#### Duggler

#### Duggler

 Level 2

JoinedDec 10, 2009Messages13

Where do i find this Condition?

I cant seem to find the condition " Lives less than or equal to 0." Also this action i cant seem to find Wait until ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True), checking every 1.00 seconds.

#### Slaydon

#### Slaydon

 Level 20

JoinedJan 6, 2008Messages2,622

Resources: 10

#### Resources
Icons 4Tutorials 6

search for text: Condition and its Integer, the Lives less is integer

#### 13rOkEn_IcE

#### 13rOkEn_IcE

 Level 2

JoinedNov 4, 2009Messages18

Plokoon said:

Wait until ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True), checking every 1.00 seconds
How i do that?


Click to expand...


Hello, i found how you can do it. First make the action and then: "Wait for condition" then when you clicked on "Condition" use "Boolean Comparison" press on "Triggering unit is a structure" and change "Unit - Unit Classificaton Check" to "Unit Group - Units of Unit Group are dead" press on "Unit Group" and change it to "Units owned by (Creep controller)" then just click "Ok" and you're done!!!!


(If you don't understand something just ask and i will answear you).

#### nonsense

N

#### nonsense

 Trigger5:RedDefeat


Events:


None


Conditions:


Lives less than or equal to 0


Actions:


Unit Group - Pick every unit in (Units in Whole Red Region) and do (Unit - Remove (Picked unit) from the game)


Game - Defeat Player 1 (Red) with the message: You Lose!


Click to expand...

How to do this plss reply asap.

#### Slaydon

#### Slaydon

 Level 20

JoinedJan 6, 2008Messages2,622

Resources: 10

#### Resources
Icons 4Tutorials 6

sssssuuggeessttioooaaan,


If you have trouble of the units being too seperated, simply set the Collizion size of the unit to 0.30 or something, it will look more proffesional. ^^


^^Also, You above

Events
Time - Every 1.00 seconds of game time

Conditions
Integer - Lives less than or equal to 0

Actions
Unit Group - Pick every unit in REGION, and do Unit - Remove (Picked Unit),

Game - Defeat. ^^

#### Elizmo

E

#### Elizmo

Plzz help me..

I have a little problem, i want to make towers that the creeps can walk throug and i want to build the towers "on" the road.

but i have 2 problems..

1:

 I can't make the towers walk throug able.

2:

 I don't know how to make towers that can be build on terrain that towers

 normaly not can be build on.

will someone plzz help me with this?

And ahow can you make that "trigger window" ?=

And BTW cool tut it willy helped me


 Last edited by a moderator: Feb 11, 2010

#### Tatanoc

#### Tatanoc

 Level 2

JoinedFeb 21, 2010Messages12

I'm having trouble with the Set RedLives = (RedLives -1) action in the lives section. I have the first Set RedLives = part but not the rest, could somebody please explain to me how to got the -1 to go there?


Thank you

#### Tatanoc

#### Tatanoc

 Level 2

JoinedFeb 21, 2010Messages12

I'm having trouble with the RedLives = RedLives - 1 action. Could someone please tell me where i can get that action? I have the first part but can't manage to fit in the second RedLives -1 action. Thanls

#### cpjok

#### cpjok

 Level 6

JoinedFeb 18, 2010Messages153

Level1

Events:

Time - TDTimer expires

Conditions:

Level Equal to 1

Actions:

Unit - Create 25 Footman for Player 12 (Brown) at (Center of Start4 <gen>) facing Default building facing degrees
Ok i put that but it creates 25 Footman every like 10 seconds!

How i fix it?

#### Slaydon

#### Slaydon

 Level 20

JoinedJan 6, 2008Messages2,622

Resources: 10

#### Resources
Icons 4Tutorials 6

set level Level+1 ?

#### cpjok

#### cpjok

 Level 6

JoinedFeb 18, 2010Messages153

Slaydon said:

 set level Level+1 ?


Click to expand...


Nah the Set Level + 1 is in differant trigger


this trigger is so it knows when level your on so its if level = 1 and so on

#### turk32

T

#### turk32

Set BeginningTimer = (Last started timer)


i cant find the variable beginningTimer?


or isnt it a variable?


plz help me

#### turk32

T

#### turk32

Help me out plz

Set BeginningTimer = (Last started timer)

#### D0W3R

#### D0W3R

 Level 7

JoinedJan 29, 2010Messages213

Resources: 1

#### Resources
Maps 1

Levels conditions has to be like this "unit type of (entering unit) not equal to ("builder")" !!!

 You must log in or register to reply here.


### Similar threads

R

Locked

 Making a TD


Rebuke

Dec 27, 2009

World Editor Help Zone

Replies0Views1K

Dec 27, 2009
Rebuke

R

 Making a Maze


DarkLordX

Feb 11, 2009

Trigger (GUI) Editor Tutorials

Replies28Views16K

Aug 5, 2010
jacobemslayer

 How to make a TD


iown_azz

Apr 15, 2010

Tutorial Archive

Replies4Views2K

Apr 15, 2010
ap0calypse

 Basic Triggering


Ralle

Jan 3, 2006

Trigger (GUI) Editor Tutorials

Replies4Views39K

Jul 7, 2016
billel

 Making Elevators


cool_is_i

Nov 25, 2008

Trigger (GUI) Editor Tutorials

Replies32Views30K

Jan 20, 2011
Orcnet

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