---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/complete-and-concise-gui-tutorial.111863/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Complete and Concise GUI Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/complete-and-concise-gui-tutorial.111863/>
#### Zypher

#### Zypher

 Level 7

JoinedMay 21, 2008Messages217

Resources: 7

#### Resources
Icons 2Skins 1Maps 1Tutorials 3

GUI Complete Trigger Tutorial by Zypher-

###
 By Zypher

What will this tutorial teach you?

-------------------

Beginner

What is a trigger?

What is an event?

What is a condition?

What is an action?

How can I make these work to my advantage?

--------------------

Intermediate

What is a variable?

What is an array?

What is a loop?

What is a memory leak?

Side Notes to remember.

--------------------

Expert

There is no such thing as GUI expert , now it's time you start learning jass.

#
 Beginner

##

What is a trigger?

A trigger is something that happens when something happens in a warcraft game.

Ex: A unit dies Then do this: Create unit for Player 1 Red

A trigger consists of 3 parts:

Event-Something that happens that makes the trigger activate/go.

Condition-If this isn't met then the trigger won't run.

Action--What happens when your trigger runs and the condition(s) are met.

An analogy for a trigger would be:

Imagine there is a marble a track and a hole it has to go into to come out the other side.An event is like putting the marble down on the track, a condition is the marble will only go out the other side if the hole is big enough, the action is going out the hole and whatever happens after that.

Example:

A trigger
Events
Unit - A unit Dies

Conditions
(Unit-type of (Dying unit)) Equal to Footman

Actions
Unit - Create 1 Footman for Player 1 (Red) at (Center of (Playable map area)) facing Default building facing degrees

I will explain how this works.

Event-This trigger runs everytime a unit dies

Condition-If this is met then the actions will run (In this case if the dying unit is a footman then the action will run.)

Action-In this trigger when executed it creates a footman for player 1 red at the center of the playable map area(the map) facing 0 degrees (left)

##

What is an event?

An event is something that happens that makes a trigger run. These include the following. Bolded text is changeable to equel, not equel, greater than, less than, greater than or equel to, less than or equel to. The underlined text is changed to your variable, a preset, or a raw id(selecting something). Astricks (*) are the exception for bold text.

General Event

-Map initialization (start of map)

Destructible Event

-Destructible dies  (a killable doodad dies)

-Destructible within reigon dies  (unit dies in a preset reigion (reigion pallete))

Dialog Event

-A dialog button is clicked for dialog(player clicks a dialog button for a created dialog)

Game Event

-The in-game time of day becomes Equal toTime (Time = time of virtual day in wc3)

-Variable becomes equal toNumber (a precreated real variable becomes equal to a number)

-A saved game is loaded  (campains, just what it says)

-The game is about to be saved  (player trys to save the game)

-The hero abilities button is clicked  (the red plus on a learning hero is clicked)

-The build structure button is clicked (the hammer icon on a builder (exeon) is clicked)

-The tournament game must end now(0.o no idea what this is)

Player Event

-Player types a chat message containing text as an Exact Match*  (just what it says)

-Player skips a cinematic sequence (player presses escape key (useable outside of cinematics))

-Playerselects* a unit  (a player selects any unit)

-Playerpresses* the left* arrow key  (player presses an arrow key)

-Player's current gold * becomes equal to Number (does what it says)

-Player changes Alliance settings  (player changes alliance via the alliance menu)

-Player changes  Alliance (non-aggression) settings  (same as above?)

-Player leaves the game with a victory  (player wins)

-Player leaves the game with a defeat  (player loses)

-Player leaves the game  (player leaves)

Time Event

-Elapsed game time is number seconds  (time thats gone by is equal to (number) seconds)

-Every number seconds of game time  (periodic event that fires every x number of seconds)

-timer expires  ( a created timer expires)

Unit event

-UnitDies*(specific) (a specific unit dies)

-A unit owned by PlayerDies*(generic)  (a unit owned by a player dies)

-A unit Dies*(generic) (does what it says)

-A unit enters Reigon(generic)  (a unit enters a premade reigion (reigion pallete))

-A unit leaves Reigon(generic)  (a unit leaves a premade reigion (reigion pallete))

-A unit comes within Number of Unit(generic) ( a unit comes within range of a specific unit)

-Unit's life becomes Less thanNumber(specific)  ( a units life becomes less than x)

-Unit's mana becomes Less thanNumber ( a units mana becomes less than x)

##
 What is a condition

A condition is something that a trigger checks before it runs it's actions. If the condition is met then the actions run and the trigger does something. These include the following. Bolded text is changeable to equel, not equel, greater than, less than, greater than or equel to, less than or equel to. Titles of conditions are bolded so you can see them better. The underlined text is changed to your variable, a preset, or a raw id(selecting something). Astricks (*) are the exception for bold text.

Boolean comparison

-checks for something and can come back true or false

Ability comparison

-checks what ability is being cast Ex: Acid Bomb

Buff comparison

-checks to see if a buff variable is a certain buff Ex:Bloodlust

Destructible comparison

-checks to see if a destrucible is equal to a variable or other destructible. Ex: Summer Tree wall

Destructible type comparison

-checks to see if the destructible is a certain type Ex: Summer Tree Wall

Dialog button comparison

-checks to see if the clicked dialog button is a certain dialog button

Game difficulty comparison

-checks game difficulty to see if it is a certain difficulty Ex: easy

Game speed comparison

-checks the game speed to see if it's a certain game speed Ex:fast

Hero Skill Comparison

-checks to see if the learned hero skill is a certain skill Ex:Holy Light

Integer Comparison

-checks to see if an integer is equal to a Number

Item Comparison

-checks to see if an item being dropped, manipulated, or aquired ect. is a certain item

Item Class Comparison

-checks to see if an item class is a certain class Ex: Power-Up

Item Type Comparison

-checks to see if an item type is a certain item type Ex: Talismen of Evasion

Melee AI Comparison

-checks to see if a computer player's level is a certain level Ex: easy,normal,insane

Order Comparison

-checks to see if an order is a certain order

Player Comparison

-Many different variations on this one, I'll list a few. Owner of unit, Owner of Item, Picked player, Triggering player is a certain player Exlayer 1 Red

Player Color Comparison

-checks if the color of Player is a certain color

Player Controller Comparison

-checks to see the controller of a player Ex: computer, neutral, player

Player Slot Status Comparison

-checks to see if a player's slot status is a certain slot status Ex: Playing, Unused, Left

Race comparison

-checks to see if a players race is a certain race Ex: Human, Undead

Real comparison

-Checks to see if a Number is equal to a Number

String Comparison

-checks a string(text) to see if it is a certain string Ex:-save

Tech-Type Comparison

-checks to see if the researched tech-type is a certain tech type

Terrain Type Comparison

-checks to see if the terrain type at a Point is a certain terrain type.

Trigger Comparison

-checks to see if a trigger is a certain trigger(variable)

Unit Comparison

-checks to see if a unit is a certain unit Ex: Picked unit equal to casting unit

Unit Type Comparison

-checks to see if a unit is a certain type Ex: Footman

And(multiple conditions)

-Only used when using the or state ment so you could do condition or condition and condition

Or(multiple conditions)

-Used so If any of the conditions are true the actions run

##
 What is an action?

An action is the part of a trigger that does something, an action only runs if the conditions are met. For the sake of time I am only going to post catagories of actions as it is not completely necessary for me to spend an hour writing down all the actions. On request I will write them down in this tutorial. The following are action catagories.

AI

Animation

Camera

Cinematic

Countdown Timer

Destructible

Dialog

Enviroment

Floating Text

Game

Game cashe

Hero

Item

Image

Leaderboard

Lightning

Melee Game

Multiboard

Neutral Building

Player

Player Group

Quest

Reigon

Selection

Sound

Special Effect

Trigger

Unit

Unit Group

Ubersplat

Visibility

### How can I make these work to my advantage?

If you combine these events, conditions,and actions you can do just about anything in warcraft 3. Heres an example trigger just for you to look at and guess at what it does.

Player Leaves Game Gold Division
Events
Player 1 Red leaves the game

Player 2 Blue leaves the game

Player 3 Teal leaves the game

Player 4 Purple leaves the game

Player 5 Yellow leaves the game

Player 6 Orange leaves the game

Player 7 Green leaves the game

Player 8 Pink leaves the game

Player 9 Grey leaves the game

Player 10 LightBlue leaves the game

Player 11 Brown leaves the game

Player 12 DarkGreen leaves the game

Conditions

Actions
Set LeavingPlayer = (Triggering player)

Set LeavingPlayersGold = (LeavingPlayer Current gold)

Set LeavingPlayersLumber = (LeavingPlayer Current lumber)

Set AllPlayers = (All players)

Player Group - Remove LeavingPlayer from AllPlayers

Player Group - Pick every player in AllPlayers and do (Actions)
Loop - Actions
Player - Add (LeavingPlayersGold / (Number of players in AllPlayers)) to (Picked player) Current gold

Player - Add (LeavingPlayersLumber / (Number of players in AllPlayers)) to (Picked player) Current lumber

Set LeavingPlayersGold = 0

Set LeavingPlayersLumber = 0

Custom script: Call DestroyForce (udg_AllPlayers)

If you already guessed what this was then good job, your definatly learning . If not then keep trying , keep experimenting with triggers and such, You'll get it. What this does is it divides a leaving players gold between all the remaining players. The actions that have an [X=] before them are actions that set variables.The custom script at the bottom fixes this thing called a memory leak you will learn about that later.

#

Intermediate

##

What is a variable?

A variable is a reference to data stored in the computer's memory, variables are used to store values and get them back (in the case you want to use them again). A variable can be a number, unit, destructable, terrain type, ect...

You can create a variable by using the variable editor in the trigger section. Just click the green x to adda variable. Pick a type and a name. When using variable it should look a bit like this.

It can be set to anything using the set variable = Something action.

Setting a variable

Events
Your Event

Conditions
Your Condition

Actions
Set My_Unit_Type = Footman

Makemehaunit
Events
Time - Elapsed game time is 0.01 seconds

Conditions

Actions
Set temp_player = Player 1 (Red)

Set temp_point = (Center of (Playable map area))

Set temp_unittype = Peasant

Unit - Create 1 temp_unittype for temp_player at temp_point facing 270.00 degrees

Set temp_unit = (Last created unit)

VARIABLES ARE YOUR FRIENDS, USE THEM, I CAN'T STRESS THIS ENOUGH!

## What is an array?
An array is a group of more that one variable. You have to test this out yourself to understand it, but heres an explanation anyway. An array stores things just like a variable does but when making an array you must set the size. In order to find out which part of the array you are refering to the variable array needs an index number to refer to the variable in the variable array that you are trying to access. Heres an example trigger. Setting a variable array
Events
Map initialization

Conditions

Actions
Set PlayingPlayers[1] = Player 1 (Red)

Set PlayingPlayers[2] = Player 2 (Blue)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(PlayingPlayers[1] controller) Equal to User

(PlayingPlayers[1] slot status) Equal to Is playing

Then - Actions
Do nothing

Else - Actions
Game - Defeat PlayingPlayers[1] with the message: Defeat!

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(PlayingPlayers[2] controller) Equal to User

(PlayingPlayers[2] slot status) Equal to Is playing

Then - Actions
Do nothing

Else - Actions
Game - Defeat PlayingPlayers[2] with the message: Defeat!

This checks if Player 1 Red and Player 2 Blue are playing and user controled. This is the inefficent way to do it and will teach you a better way in the next part: What is a loop?

##
 What is a loop?

A loop is something that keeps going until through or cleared. Loops are another tricky thing to understand but in my opinion are by far the most useful, time-saving action/function in warcraft 3. LOOPS ARE ALSO YOUR FRIENDS (I know this personally because I used to make long ass triggers instead of using loops). Heres an example trigger for a loop.

Defeating a computer empty slot status player

Events
Map initialization

Conditions

Actions
Set PlayingPlayers[1] = Player 1 (Red)

Set PlayingPlayers[2] = Player 2 (Blue)

Set PlayingPlayers[3] = Player 3 (Teal)

Set PlayingPlayers[4] = Player 4 (Purple)

Set PlayingPlayers[5] = Player 5 (Yellow)

Set PlayingPlayers[6] = Player 6 (Orange)

Set PlayingPlayers[7] = Player 7 (Green)

Set PlayingPlayers[8] = Player 8 (Pink)

Set PlayingPlayers[9] = Player 9 (Gray)

Set PlayingPlayers[10] = Player 10 (Light Blue)

Set PlayingPlayers[11] = Player 11 (Dark Green)

Set PlayingPlayers[12] = Player 12 (Brown)

For each (Integer A) from 1 to 12, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(PlayingPlayers[(Integer A)] slot status) Equal to Is playing

(PlayingPlayers[(Integer A)] controller) Equal to User

Then - Actions
Do nothing

Else - Actions
Game - Defeat PlayingPlayers[(Integer A)] with the message: Defeat!

This defeats all the Computer/Unused player's in the game at map initilization using a loop.

##
 What is a memory leak?

A memory leak happens when the game uses something and forgets about it. This creates lagg over time if enough of them pile up. I will post a link to what I consider the most helpful memory leak tutorial.

The best GUI tool ever. Leak check.

##
 ~~emjlr3's Memory Leak / Custom Script Guide~~

-----------------------------------------------------------------------------------

This is what I used and now use in all my triggers ^

##
 Side Notes

Do not use Do Nothing as it basically compiles to a blank function call that does its name and wastes processor time.

A global variable stores a value so that it can be used at a later point in time, even if that point is near immediatly.

Also for the array, the size parameter that the globals editor provides mostly does nothing and so can be ignored but the true maximum size on an array is 8191.

###
 Expert

There is no such thing as GUI expert , now it's time you start learning jass.

The rest is up to you, have fun creating maps with triggers. If you still don't get it and need help creating a trigger then just ask and I'll be happy to help. Some other talents of mine include: loading screens, loading minimap, any graphic design Ex:skinning. If you need any of those I will probably be willing to help. In conclusion I hope everyone who read this learned a little something about triggering. I also hope the 4 hours I put into this tutorial don't go to waste.My warcraft 3 account is named Zypher and I am in no way aphiliated with the other Zypher that lurks THW, just a coincidence. Happy triggering.

-Zypher


 Last edited by a moderator: Jan 20, 2014

#### redmarine

#### redmarine

 Level 19

JoinedSep 4, 2007Messages2,826

Resources: 1

#### Resources
Maps 1

Nice how the tutorial turned out.

On the top of the page, add a small summery about the content and what you may learn from it:

*What will this tutorial teach you?*

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Please use

#
 ,

##
 and

###
 tags in accordance with the tutorial submission rules.

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


Please also have consistant headings.

#### Zypher

#### Zypher

 Level 7

JoinedMay 21, 2008Messages217

Resources: 7

#### Resources
Icons 2Skins 1Maps 1Tutorials 3

will do


Edit: fixed to your liking

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

It's good, I'll give you that. You might want to explain the events more instead of just listing them.

Also it's best to link to the other leak tutorial instead of just copying and pasting the whole thing.

#### Zypher

#### Zypher

 Level 7

JoinedMay 21, 2008Messages217

Resources: 7

#### Resources
Icons 2Skins 1Maps 1Tutorials 3

Done

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Alrighty. Approved.

#### redmarine

#### redmarine

 Level 19

JoinedSep 4, 2007Messages2,826

Resources: 1

#### Resources
Maps 1

Could this GUI tutorial become a static one?

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

What do you mean by static?

#### Foopad

#### Foopad

 Level 6

JoinedJun 15, 2008Messages175

Resources: 3

#### Resources
Maps 1Spells 2

Nice tutorial!

I only need to find out how to use a loop, and I think I've got an idea now, but I am still not certain . Your example is with players currently playing, I understand your idea, I usally have to make a trigger for each player playing. If I use a loop I do not have to, create a new trigger each time?

To sum up: A loop is a function that creates other functions according to the amount of integers called? A just creates the triggers for you?

#### redmarine

#### redmarine

 Level 19

JoinedSep 4, 2007Messages2,826

Resources: 1

#### Resources
Maps 1

Foopad said:

 Nice tutorial!

I only need to find out how to use a loop, and I think I've got an idea now, but I am still not certain . Your example is with players currently playing, I understand your idea, I usally have to make a trigger for each player playing. If I use a loop I do not have to, create a new trigger each time?

To sum up: A loop is a function that creates other functions according to the amount of integers called? A just creates the triggers for you?


Click to expand...

Loop basically means repeating.

Loops that repeat integers e.g. x-x.

Unit - Create 1 Footman for Player 1 (Red) at (Center of (Playable map area)) facing Default building facing degrees

Unit - Create 1 Footman for Player 2 (Blue) at (Center of (Playable map area)) facing Default building facing degrees

Unit - Create 1 Footman for Player 3 (Teal) at (Center of (Playable map area)) facing Default building facing degrees

Unit - Create 1 Footman for Player 4 (Purple) at (Center of (Playable map area)) facing Default building facing degrees

Unit - Create 1 Footman for Player 5 (Yellow) at (Center of (Playable map area)) facing Default building facing degrees

Unit - Create 1 Footman for Player 6 (Orange) at (Center of (Playable map area)) facing Default building facing degrees

Unit - Create 1 Footman for Player 7 (Green) at (Center of (Playable map area)) facing Default building facing degrees

Unit - Create 1 Footman for Player 8 (Pink) at (Center of (Playable map area)) facing Default building facing degrees

Unit - Create 1 Footman for Player 9 (Gray) at (Center of (Playable map area)) facing Default building facing degrees

Unit - Create 1 Footman for Player 10 (Light Blue) at (Center of (Playable map area)) facing Default building facing degrees
Instead of doing all that shit you could just do this:

For each (Integer A) from 1 to 10, do (Actions)
Loop - Actions
Unit - Create 1 Footman for (Player((Integer A))) at (Center of (Playable map area)) facing Default building facing degrees

#### Foopad

#### Foopad

 Level 6

JoinedJun 15, 2008Messages175

Resources: 3

#### Resources
Maps 1Spells 2

Oh okay . Thanks for clearing that up.

I will be very usefull for alot of my triggers!

#### leet.firefox

#### leet.firefox

 Level 17

JoinedNov 18, 2008Messages1,537

Events
Map initialization

Conditions

Actions
Set PlayingPlayers[1] = Player 1 (Red)

Set PlayingPlayers[2] = Player 2 (Blue)

Set PlayingPlayers[3] = Player 3 (Teal)

Set PlayingPlayers[4] = Player 4 (Purple)

Set PlayingPlayers[5] = Player 5 (Yellow)

Set PlayingPlayers[6] = Player 6 (Orange)

Set PlayingPlayers[7] = Player 7 (Green)

Set PlayingPlayers[8] = Player 8 (Pink)

Set PlayingPlayers[9] = Player 9 (Gray)

Set PlayingPlayers[10] = Player 10 (Light Blue)

Set PlayingPlayers[11] = Player 11 (Dark Green)

Set PlayingPlayers[12] = Player 12 (Brown)

For each (Integer A) from 1 to 12, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(PlayingPlayers[(Integer A)] slot status) Equal to Is playing

(PlayingPlayers[(Integer A)] controller) Equal to User

Then - Actions
Do nothing

Else - Actions
Game - Defeat PlayingPlayers[(Integer A)] with the message: Defeat!

Woudn't this be eaiser than writing it out.

Events
Map initialization

Conditions

Actions
For each (Integer A) from 1 to 12, do (Actions)
Loop - Actions
Set PlayersPlaying(Integer A) = Player(Integer A)

For each (Integer A) from 1 to 12, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(PlayingPlayers[(Integer A)] slot status) Equal to Is playing

(PlayingPlayers[(Integer A)] controller) Equal to User

Then - Actions
Do nothing

Else - Actions
Game - Defeat PlayingPlayers[(Integer A)] with the message: Defeat!

#### redmarine

#### redmarine

 Level 19

JoinedSep 4, 2007Messages2,826

Resources: 1

#### Resources
Maps 1

Yes but then again it would be easier to write:

Events
Map initialization

Conditions

Actions
For each (Integer A) from 1 to 12, do (Actions)
Loop - Actions
Set PlayersPlaying(Integer A) = Player(Integer A)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(PlayingPlayers[(Integer A)] slot status) Equal to Is playing

(PlayingPlayers[(Integer A)] controller) Equal to User

Then - Actions
Do nothing

Else - Actions
Game - Defeat PlayingPlayers[(Integer A)] with the message: Defeat!

#### Zypher

#### Zypher

 Level 7

JoinedMay 21, 2008Messages217

Resources: 7

#### Resources
Icons 2Skins 1Maps 1Tutorials 3

Bump! I wish the people that needed to read this actually wanted to read it.

#### Vizel

#### Vizel

 Level 12

JoinedJan 30, 2009Messages1,067

Zypher said:

 Bump! I wish the people that needed to read this actually wanted to read it.


Click to expand...

I need to read this, and I want to...it's just a little thing called ADHD... I'm gonna start twitching but I will read this. And then I'll give you my thoughts.

Edit 1: First off, I got sidetracked almost immediately. I curse my curse! >< Anyway, might want to state that this tutorial will make a LOT more sense if you are looking at the trigger editor while reading. otherwise all the "Bold text this, Underline text this" is confusing and basically a turn-off to other nubs like me. I'll continue to read with the editor open and hopefully make heads or tails of this ^^

Edit 2: After reading most of this I got to the part with Arrays and got sidetracked last night. I created a Multiboard using the Multiboard tut, and I learned a lot from that tut. I just re-read this whole tut and the Array section is for sure very hard to understand unless you mess with it yourself (Though even after doing so, I still don't get it methinks). This was very helpful, and the part where you list the events conditions and actions was very clear while reading each entry in the Trigger Editor itself (Where the brief explanation is).

You're getting a major +Rep from me, mate. ^^


 Last edited: Feb 9, 2009

#### Monkez

#### Monkez

 Level 2

JoinedJan 26, 2008Messages16

'Set AllPlayers = (All Players)'. How to set that ( All Players ) ? I can't figure out where to seek for it :/

#### leet.firefox

#### leet.firefox

 Level 17

JoinedNov 18, 2008Messages1,537

AllPlayers is a Player Group Variable.

#### rover2341

#### rover2341

 Level 12

JoinedFeb 23, 2008Messages587

Resources: 5

#### Resources
Maps 2Tutorials 3

A Very good guide. Its very useful to see explanations for what the triggers do as some times i can get confused

 There is no such thing as GUI expert , now it's time you start learning jass.


Click to expand...


I don't agree with this statement.

#### tomdeterre

T

#### tomdeterre

do you really can call DestroyPlayer (udg_LeavingPlayer)? so leaving player is a player group ? because i never seen DestroyPlayer function !!

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

uhm. No you cant destory a player...

you can add the leaving player to a player group tho.

but why revive a 4-5 year old thread anyway

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

tomdeterre said:

 do you really can call DestroyPlayer (udg_LeavingPlayer)? so leaving player is a player group ? because i never seen DestroyPlayer function !!


Click to expand...

DestroyPlayer() does not exist. You don't have to destroy players--there are just 16 on the map, and you can't create new ones.

Thanks for catching that, I've updated the main post to remove that line.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

You should remove integer A/B as it is slower and less efficient. It may give new users the wrong impression since you are using them.

 You must log in or register to reply here.


### Similar threads

 [Reviewed: BlinkBoy] Understanding triggers and creating them.


Zypher

Aug 20, 2008

Trigger (GUI) Editor Tutorials

Replies9Views6K

Sep 5, 2008
BlinkBoy

 Chaosy's GUI tutorial for beginners


Chaosy

Apr 6, 2012

Trigger (GUI) Editor Tutorials

Replies9Views8K

Dec 25, 2016
Chaosy

 Opening/Closing Gates


RazorHedgeFan

Jan 4, 2011

Trigger (GUI) Editor Tutorials

Replies29Views29K

Mar 23, 2016
Drankore

 Arrow Key Movement / Camera System


OneEight7even

Nov 15, 2007

Trigger (GUI) Editor Tutorials

Replies32Views28K

Apr 10, 2014
Andser

 GUI Equipment Manager - up to 11 items at once


Donach

Apr 16, 2012

Trigger (GUI) Editor Tutorials

Replies25Views15K

Jun 29, 2014
Tal'Rarity

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