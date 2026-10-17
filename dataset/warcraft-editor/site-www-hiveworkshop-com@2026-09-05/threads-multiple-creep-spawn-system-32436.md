---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/multiple-creep-spawn-system.32436/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Multiple Creep Spawn System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/multiple-creep-spawn-system.32436/>
#### DoOs_101

#### DoOs_101

 Level 12

JoinedAug 7, 2004Messages875

Resources: 12

#### Resources
Maps 11Tutorials 1

Foreword:


In this tutorial you will learn how to make a system based on GUI for multiple creep spawning. Creep spawning is commonly used in map types such as Tower Defense, Castle Defense, Survival, and Hero Defense.


The system you are about to learn is user friendly, easy to understand once you know the functions. It is also feasible and is easy to be adjusted to any frequency of difficulty and style you want.


Why use this method? It is efficient and it only requires three simple trigger packages. This system method will have an initVar, which sets the unitType and amount for each lvlNum, it will also have the callOperate, which creates the creeps and orders it to move to a specific location, and it will also have a roundTimer, which is a countdown timer for every round.

The Code:


Firstly create these variables in your map


Array unit_type = variable unit type


Array unit_type_var_two = variable unit type


Array unit_type_var_three = variable unit type


int lvlNum = variable integer value


Array unit_amount = variable integer value


Array unit_amount_var_two = variable integer value


Array unit_amount_var_three = variable integer value

roundwindow = countdown timer


The variable unit_type and unit_amount depends on how many unit types you want in each level. For this code example, there will be three unit types so I'd create three unit_type variables and three unit_amount variable.


To start with I'm going to create a countdown timer first that will end in 60 seconds but is repeating.

Actions
Game - Display to (All players) the text: Countdown started...

Countdown Timer - Start RoundWindow as a Repeating timer that will expire in 60.00 seconds

Set RoundWindow = (Last started timer)

Countdown Timer - Create a timer window for (Last started timer) with title Enemy Wave in...

Right now you have completed the simplest package which is the countDown. This will be used to call the callOperate method. You can add event - conditions for this package such as periodic event - game time elapsed. The countdown RoundWindow will not stop unless you call a method to stop the RoundWindow. This package can be twisted to any style you like such as if you don't want this countdown timer to automatically repeat, you can add a new event timer expires, timer = roundWindow and have:

Event
timer expires

Actions
bj_wantDestroyGroup = true

Wait until ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True), checking every 1.00 seconds

Game - Display to (All players) the text: Countdown started...

Countdown Timer - Start RoundWindow as a Repeating timer that will expire in 60.00 seconds

Set RoundWindow = (Last started timer)

Countdown Timer - Create a timer window for (Last started timer) with title Enemy Wave in...

Now lets start making the second package which is initVar. This package is the constructor of the whole system class. It will initialize variables unit_type and unit_amount for each lvlNum (lvlNum can be considered the index number of the creep level).

Event
Game Initialization

Actions
Set Lvl_num = 0

-------- Level 1 --------

Set unit_type[1] = Ghoul

Set unit_type_var_two[1] = Troll

Set unit_type_var_three[1] = Zombies

Set unit_amount[1] = 3

Set unit_amount_var_two[1] = 1

Set unit_amount_var_three[1] = 1

-------- Level 2 --------

Set unit_type[2] = Troll

Set unit_type_var_two[2] = Ghoul

Set unit_type_var_three[2] = Zombies

Set unit_amount[2] = 2

Set unit_amount_var_two[2] = 2

Set unit_amount_var_three[2] = 1

-------- Level 3 --------

Set unit_type[3] = Zombies

Set unit_type_var_two[3] = Troll

Set unit_type_var_three[3] = Light Trolls

Set unit_amount[3] = 2

Set unit_amount_var_two[3] = 2

Set unit_amount_var_three[3] = 1

...


You can go on forever to any maximum level you want. For this code example, lets stick to 3 levels only.


Right now you have completed the second package which is initVar. All it does is initializing the variables of unit_type and unit_amount. The unit_type is to declare a unit type that will be created during each level. The unit_amount is to declare how many units of a unit type will be created during each level. The number you put in the array index; set unit_type[1] = Ghoul, is the number of the lvlNum or the index number of the level.


In this package you can create as many level as you want, many unit types for each level, and different amounts. It is easily adjusted for you to change the frequency of the difficulty and style in the map.


Now lets start making the final package which is the callOperate. This method will run a loop that will create the initialized creep units. Note that in this tutorial example, the loop will create the initialized creep at 8 locations in the corners of the map. It will then issue an order for the created creep unit to move to the center of the map. In your own version you can set different creep spawn locations if you want, you can have 6 locations instead but before modifying the method you must understand the concept first.


Before moving on please create this variables in your map,


Array loc = variable location (a variable for the eight spawning locations)


int incrementx = variable integer (a variable used for the loop)


int incrementy = variable integer (a variable used for the loop)

Events
Map Initialization

Actions
Set loc[1] = (Center of Corner BR <gen>)

Set loc[2] = (Center of Corner BL <gen>)

Set loc[3] = (Center of Corner BL2 <gen>)

Set loc[4] = (Center of Corner TL2 <gen>)

Set loc[5] = (Center of Corner TL <gen>)

Set loc[6] = (Center of Corner TR <gen>)

Set loc[7] = (Center of Corner TR2 <gen>)

Set loc[8] = (Center of Corner BR2 <gen>)

Spawn
Events
Time - RoundWindow expires

Conditions

Actions
Set Lvl_num = (Lvl_num + 1)

For each (Integer A) from 1 to unit_amount[Lvl_num], do (Actions)
Loop - Actions
Set incrementx = 1

Set incrementy = 1

For each (Integer B) from 1 to 4, do (Actions)
Loop - Actions
Unit - Create 1 unit_type[Lvl_num] for Player 12 (Brown) at loc[incrementy] facing Default building facing degrees

Unit - Order (Last created unit) to Attack-Move To center of map

Unit - Create 1 unit_type[Lvl_num] for Player 12 (Brown) at loc[(incrementy + 1)] facing Default building facing degrees

Unit - Order (Last created unit) to Attack-Move To center of map Set incrementx = (incrementx + 1)

Set incrementy = (incrementy + 2)

Wait 0.75 seconds

For each (Integer A) from 1 to unit_amount_var_three[Lvl_num], do (Actions)
Loop - Actions
Set incrementx = 1

Set incrementy = 1

For each (Integer B) from 1 to 4, do (Actions)
Loop - Actions
Unit - Create 1 unit_type_var_three[Lvl_num] for Player 12 (Brown) at loc[incrementy] facing Default building facing degrees

Unit - Order (Last created unit) to Attack-Move To center of map

Unit - Create 1 unit_type_var_three[Lvl_num] for Player 12 (Brown) at loc[(incrementy + 1)] facing Default building facing degrees

Unit - Order (Last created unit) to Attack-Move To center of map

Set incrementx = (incrementx + 1)

Set incrementy = (incrementy + 2)

Wait 0.75 seconds

For each (Integer A) from 1 to unit_amount_var_two[Lvl_num], do (Actions)
Loop - Actions
Set incrementx = 1

Set incrementy = 1

For each (Integer B) from 1 to 4, do (Actions)
Loop - Actions
Unit - Create 1 unit_type_var_two[Lvl_num] for Player 12 (Brown) at loc[incrementy] facing Default building facing degrees

Unit - Order (Last created unit) to Attack-Move To center of map

Unit - Create 1 unit_type_var_two[Lvl_num] for Player 12 (Brown) at loc[(incrementy + 1)] facing Default building facing degrees

Unit - Order (Last created unit) to Attack-Move To center of map

Set incrementx = (incrementx + 1)

Set incrementy = (incrementy + 2)

Wait 0.75 seconds

This method is not very complicated once you understand it. All it does is it initializes the variable loc for the 8 spawning locations. It then calls the first loop which will create 1 unit for every unit_type in each spawning locations for every loop increment. Then it calls for the next loop which will do the same thing except it will create 1 unit for every unit_type_var_two. Then it calls for the third loop which will do the same thing except it will create 1 unit for every unit_type_var_three. Note that if you have 5 multiple creeps for your game, you need 5 loops. The code will then remove the 8 spawning locations from the memory in order to remove leakage in the game.


You have completed the Multiple Creep Spawn system. Test it now and see if it works. If you found bugs and errors please contact me at [email protected] or else submit your report at this thread.

Things to note if you want to modify the code:


• If you want to change the number of multiple units for every level in your map, the corresponding variables are: unit_type and unit_amount, don't forget the loop.


• If you want to change the time the countdown timer will last, modify the countDown code.


• If you want to make it so that the countdown timer does not repeat itself and waits for condition that the enemy attackers have zero attacking units, read the countDown package section.


• This system can be done more efficiently in Jass. Next tutorial will be about implementing a Smart Pathing system and AI for spawned creeps with Jass.


Thank you and have fun coding

#### Attachments

MultipleCreepSpawn.w3x

 27.5 KB

 · Views: 2,829




 Last edited by a moderator: Nov 26, 2007

#### Jedi §layer

#### Jedi §layer

 Level 3

JoinedMay 3, 2007Messages30

Resources: 1

#### Resources
Maps 1

I have read trough your tutorial and all i can say is WTF? This is so complicated that I faint only by looking at it! You have to find a simpler way to wxplain it. You can start by explaining where to do everything, I knew it was in the trigger editor but I fell of pretty fast. For someone who has English as their mother-tongue and not Norwegian may find this hlpful, but for me many things are pretty confusing. I don't expect you to make one in Norwegian or something but you will have to know when something is to advanced for others!


Expect from those points you tutorial is amazing DoOs_101, keep on writing more tutorials, I'll bet that almost everyone understands what you mean ( expect from me on some points).

#### Sworddragon

#### Sworddragon

 Level 5

JoinedJul 21, 2005Messages120

Resources: 2

#### Resources
Maps 2

I understood it quite well. Though, I didn't read ANY of it. The pictures are very nice, helpful too.

#### GST_Nemisis

#### GST_Nemisis

 Level 21

JoinedJan 5, 2005Messages3,515

Resources: 1

#### Resources
Tutorials 1

yeh its just a basic run down of spawn creeps using variables. there are of course other ways of doing it but this does expain it ok. the thing to remember about this is that you can use the variables how ever you want, this is just a nice example.

#### beast_boy_bets

#### beast_boy_bets

 Level 6

JoinedMay 7, 2007Messages83

Resources: 6

#### Resources
Maps 4Tutorials 2

an easier way to get it to your map is to download the map then copy and paste the folder for the triggers (to your map).. Worked for me

#### DoOs_101

#### DoOs_101

 Level 12

JoinedAug 7, 2004Messages875

Resources: 12

#### Resources
Maps 11Tutorials 1

I hope you guys like the demo where one guy gets rumbled by 100 creep . Yeah its easier that way but better if you can understand the concept.

#### Shadow_01

#### Shadow_01

 Level 2

JoinedJan 19, 2007Messages16

Pretty easy to create... I suggest to not copy paste - because when you make your own trigger you learn something from it...


Conclusion - Copy/Paste: Easy but you dont learn a thing from it


 Own Making:You learn something and maybee make the trigger better

#### Sojobo

#### Sojobo

 Level 2

JoinedJun 30, 2007Messages19

Basically, it is quite easy except for the last part which a bit of time to digest.

#### Raaka Arska

R

#### Raaka Arska

i used these multiple creep spawnings in my map R_A Hero Siege too. it wasnt hard to make those at begin,but later i found few bugs in spawning (got those fixed long time ago) like one bug was that only red got multiple creeps,others got first waves only! but no need to worry in my map these is hardly no bugs found recently,if you play it and find a bug,then pls tell me! (i didnt use this your way i used integers) good work anyway!

#### Clone Commander

#### Clone Commander

 Level 2

JoinedAug 21, 2007Messages18

Resources: 1

#### Resources
Tutorials 1

a good tut but unless your bill gates...this is jiberish

#### masteromag70

#### masteromag70

 Level 4

JoinedJun 16, 2007Messages59

Resources: 3

#### Resources
Maps 3

I, personally, didn't get it. Seeing as to how this is a tutorial I would think that this would be simpler to understand. I really don't get variables to begin with though, so that might've helped. But even so a tutorial should be in a format that anyone who knows the basics should be able to understand. And yes I understand the basics.


Note: That's just my opinion.

#### Xexorian

X

#### Xexorian

Mcss


Here's a revised version of your work.


First:


Create a Unit-Type array - but before you do, you need to keep in mind you need multiple arrays for multiple types of monsters per wave.


So, say you want 3 monster types per wave (or more).


Unit_Type_Var_1


Unit_Type_Var_2


Unit_Type_Var_3


Check ''Array'' box and each one won't have an initial value when you make them.


Make sure the variable is set to 'Unit-Type'


Second:


Create an Integer Array variable. Simply put, this is a Integer variable with the ''Array'' checked.


Keep in mind, you need 1 Integer array for each type of monster.


Unit_Integer_Var_1


Unit_Integer_Var_2


Unit_Integer_Var_3


Third:


You need to create a ''point'' array variable from the down down list.


Make sure it's checked as an ''Array''.


Location_Var


Fourth:


You need to create a ''Increment'' integer and a ''Level'' integer to determine which level you're on. (these are NOT an array).


Increment_Var


Level_Var


You also need to create a ''Timer'' Variable to Countdown which level you're on. (this one is NOT an array).


RoundWindow_Var


Lets start off by setting Unit-Type variables and Unit-Amount variables for each wave, and set starting variables for any easy-to-do TD map.

Events
Map Initialization

Conditions

Actions
----- (first enable visibility for all picked players) -----

Player Group - Pick every player in (All players) and do (Visibility - Create an initially Enabled visibility modifier for (Picked player) emitting Visibility across (Playable map area))

----- (then you might want to add some gold to build towers) -----

Player Group - Pick every player in (All players) and do (Player - Add 350 to (Picked player) Current gold)

----- (as well as turn on bounty rewards for the enemy) -----

Player - Turn Gives bounty On for Player 12 (Brown)

----- (You want to make sure you start on level 1 =).)

Set Level_Var = 1

----- LEVEL 1 -----

----- Unit-Types -----

Set Unit_Type_Var_1[1] = Zombie

Set Unit_Type_Var_2[1] = Skeleton Warrior

Set Unit_Type_Var_3[1] = Ghoul

----- Unit-Amounts -----

Set Unit_Integer_Var_1[1] = 3

Set Unit_Integer_Var_2[1] = 2

Set Unit_Integer_Var_3[1] = 1

----- LEVEL 2 -----

----- Unit-Types -----

Set Unit_Type_Var_1[2] = Ghoul

Set Unit_Type_Var_2[2] = Skeletal Archer

Set Unit_Type_Var_3[2] = Skeleton Warrior

----- Unit-Amounts -----

Set Unit_Integer_Var_1[2] = 3

Set Unit_Integer_Var_2[2] = 1

Set Unit_Integer_Var_3[2] = 2

etc..
Okay, let me explain this trigger. Basically, we just set it up so that the unit types for level 1 are a Zombie, Skeleton Warrior, and Ghoul, on the second level the unit-types change (because it's harder).


Since we're going to setup the level's to use multiple units of each type, you can increase difficulty without changing the base unit by simply adding MORE units per wave.


Also keep in mind everytime you set the next level up, you need to change the [#] inside the [#] box next to each array, this relates to the level integer variable we're about to setup for you.


In the example above I put in 1, or 2, or 3 amounts for the corresponding amount of units spawn for each type. So, each Unit-type, matches it's Unit-amount integer.


Next.. Setup the Time elapsed trigger, and then the Countdown timer window trigger.

Events
Time - Elapsed game time is 25.00 seconds

Conditions

Actions
Game - Display to (All players) the text: Countdown started...

Countdown Timer - Start roundwindow as a One-shot timer that will expire in 30.00 seconds

Set RoundWindow_Var = (Last started timer)

Countdown Timer - Create a timer window for (Last started timer) with title (Enemy Wave + ((String(lvlNum)) + in...))

----- (Then we pick all players and show the window to them) -----

Player Group - Pick every player in (All players) and do (Countdown Timer - Show (Last created timer window) for (Picked player))

This trigger uses multiple concatenate strings and uses the String - Integer conversion to put in the Wave number you're on.

Events
Time - roundwindow expires

Conditions

Actions
----- (First we hide the last created timer for all players) -----

Player Group - Pick every player in (All players) and do (Countdown Timer - Hide (Last created timer window) for (Picked player))

----- (Then we destroy it..) -----

Countdown Timer - Destroy (Last created timer window)

Wait until ((All units of (Units owned by Player 12 (Brown)) are dead) Equal to True), checking every 1.00 seconds

----- (we wait till player 12's units are dead, then start the next timer) -----

Game - Display to (All players) the text: Countdown started...

Countdown Timer - Start roundwindow as a One-shot timer that will expire in 30.00 seconds

----- (set it to the last started timer again) -----

Set RoundWindow_Var = (Last started timer)

----- (then use the concatenate strings from the previous one) -----

Countdown Timer - Create a timer window for (Last started timer) with title (Enemy Wave + ((String(lvlNum)) + in...))

----- (..as well as the show timer for all picked players action) -----

Player Group - Pick every player in (All players) and do (Countdown Timer - Show (Last created timer window) for (Picked player))

This sets it up so when it expires it hides the window till all of player 12's units are dead, once they're dead it starts the timer back up for the next wave to be created. This is similar to how we also spawn units in the aspect that it revolves around an expiring one-shot timer (further down).


Next... We'll deal with setting Regions = to the ''point'' (aka location) variable integer.


NOTE: Make sure you set the array values = to the amount of types, for unit-types, to the amount of levels for unit-amounts, and to the amount of regions you wish to spawn from, for your Location_Var.

Events
Map Initialization

Conditions

Actions
Set Location_Var[1] = Region 0001 <gen>

Set Location_Var[2] = Region 0002 <gen>

Set Location_Var[3] = Region 0003 <gen>

Set Location_Var[4] = Region 0004 <gen>

In the example above I set the Location's Variable = to 1 through 4 for regions 1-4.


Okay this next trigger is where it gets complicated.

Events
Timer Expires

Conditions

Actions
Set Increment_Var = 1

For each (Integer A) from 1 to 4, do (Actions)
Loop - Actions
----- We do this for each of the unit types (for example 3 times.) -----

Unit - Create Unit_Integer_Var_1[Level_Var] Unit_Type_Var_1[Level_Var] for Player 12 (Brown) at Location_Var[Increment_Var] facing Default building facing degrees

Unit - Create Unit_Integer_Var_2[Level_Var] Unit_Type_Var_2[Level_Var] for Player 12 (Brown) at Loc[Increment_Y] facing Default building facing degrees

Unit - Create Unit_Integer_Var_3[Level_Var] Unit_Type_Var_3[Level_Var] for Player 12 (Brown) at Loc[Increment_Y] facing Default building facing degrees

Set Increment_Var = (Increment_Var + 1)

Set Level_Var = (Level_Var + 1)

This create's (#) of units you specified for (Unit-type) you specified, at the 1st location (Increment = 1, remember?) you also set previously, facing default degrees. It then increases the Increment Variable per loop (4 times for 4 regions).


Finally, after the loop is over it increases the Level_Variable so that next time it pulls from the array variables it pulls from the [2] group you set in the first trigger above.


NOTE1: that at the start of the Trigger we set Increment = 1 so that it reset's the variable to spawn at each of the 4 regions we had setup all over again.


NOTE2: when you setup the 'time expires' event, you can tell it to add gold per level there (by pick every player function). etc. You can also base it on an amount you set just like monster variables.


I hope this elaborates (and works out) issues for some of you. I just tested these triggers exactly as I wrote them (unless I have misswritten them), and everything seems to work perfectly. Good Luck!


~Xexorian


 Last edited by a moderator: Nov 25, 2007

#### DoOs_101

#### DoOs_101

 Level 12

JoinedAug 7, 2004Messages875

Resources: 12

#### Resources
Maps 11Tutorials 1

Lol either way its 'bad' english or 'good' english you all should be thankful that this resource exists freely for you. Any being can depict knowledge from the most unlikely sources such as rocks to make fire. Come on if stone age people can do that why you wine about such things .

#### Kieve

K

#### Kieve

...technically, you only need the one UnitType array.


As I understand it, you could initialize one UnitType[#] for whatever units you choose, and just use an additional "helper" Integer variable for # of creep-types per wave.

Set VariantsPerWave = 4

----- Level 1 Wave -----

Set CreepTypes[1] = Zombie

Set CreepTypes[2] = Ghoul

Set CreepTypes[3] = Nerubian Warrior

Set CreepTypes[4] = Skeletal Mage

----- Level 2 Wave -----

Set CreepTypes[5] = Zombie (Wave 2)

Set CreepTypes[6] = Furbolg Warrior

Set CreepTypes[7] = Nerubian Overlord

Set CreepTypes[8] = Giant Skeleton

----- Level 3 Wave -----

Set CreepTypes[9] = Doomguard

Set CreepTypes[10] = Fel hound

Set CreepTypes[11] = Revenant of the Depths

Set CreepTypes[12] = Fel Guard

...etc...
Then, for actualy calling the spawns, just use a For loop and base each wave on Integer A + (VariantsPerWave)

For each (Integer A) from 1 to 4, do (Actions)
Loop - Actions

Unit - Create (Units) CreepTypes[Integer A] for Player 12 (Brown) at Location X

----- For Wave 2 types -----

Unit - Create (Units) CreepTypes[(Integer A + VariantsPerWave)] for Player 12 (Brown) at Location X

----- For Wave 3 types -----

Unit - Create (Units) CreepTypes[(Integer A + (VariantsPerWave * 2))] for Player 12 (Brown) at Location X

...And so forth. Some might find it simpler, some might not, but personally I consider it a bit more efficient to reuse an array and divide it into theoretical "ranks," than to assign a new array for each creep-type in a wave.

#### rApp

R

#### rApp

I need help, what does "bj_wantDestroyGroup = true" means, whats bj_wantDestroyGroup?


Thx

#### ringmaster

R

#### ringmaster

hey any one know how to make a quest?

 -ringmaster

#### masteromag70

#### masteromag70

 Level 4

JoinedJun 16, 2007Messages59

Resources: 3

#### Resources
Maps 3

ringmaster said:

 hey any one know how to make a quest?


 -ringmaster


Click to expand...


Yes actually, it's quite simple. All you have to do is make the event whatever you want for example, map initialization. That will make the quest be there from the start. Then you go to the action "Quest". Then go to "Create quest". Put in all the information (Name, description, etc.) in the info slots. Then in order to complete it make whatever you want to happen the event, for example make the event footman001 dies. Then as an action mark the quest completed and make it completed or w/e. Play around with it, it's actually quite simple. One of the very few things I figured out by myself.

#### Mormegil

M

#### Mormegil

This trigger almost worked when I attempted to create it.


Trigger currently looks like this, some variables' names may differ and so on:


EDIT: Okay I figured out the monster types, now I get the right ones but the trigger still spawns too many monsters, dozens of them in the middle of the map and I can't understand why?

#### Attachments

Picture.jpg

 60.6 KB

 · Views: 658



Spawning.jpg

 37.2 KB

 · Views: 677




 Last edited by a moderator: Feb 29, 2008

#### NGLY

#### NGLY

 Level 3

JoinedMar 22, 2008Messages13

Resources: 6

#### Resources
Maps 6

i want to ask what does the X= mean?

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

It means set a variable "X" to be a certain value.


It could also mean another variable is used but X is there to represent it to tell you what to do.


X could also represent the X cordinates in a variable form of an object.


NOTE - I have not read where you got that "X" from so I gave a list of posibilities.

#### cHeRoL

#### cHeRoL

 Level 11

JoinedMar 30, 2008Messages650

Resources: 5

#### Resources
Models 1Maps 2Spells and Systems 2

Complicated man,make an easy trigger,or i will post my creep spawning trigger.

#### Yamateh

Y

#### Yamateh

I started by creating regions where I want the creeps to go to and spawn from. You can place regions just like units in the map editor, and pick on the region and hit the 'enter' key to rename it. I recommend naming regions by what they are for, which makes them easier to pick in the lists from the trigger window.

first you will need some variables -

First create one and check the 'array' box. name the variable Creep_list or something similar and in the variable type select 'Unit-Type' the set the array size to how many kinds of units you want to spawn.

Second we will need a variable type 'region' called temp_location

Third we need a variable type 'Unit-Group' called temp_group

now, closing the variable editor window, go back into the trigger editor.

your map should have a trigger called initialization (or similar) create new actions in that trigger like this -

Events
Map initialization

Conditions

Actions
Set CreepList[1] = Footman

Set CreepList[2] = Knight

Now we have set which units will spawn, we will set them to spawn every 20 seconds, and order them to attack-move to wherever they need to go.

Red 1 Spawns
Events
Time - Every 20.00 seconds of game time

Conditions

Actions
Set temp_location = red 1 <gen>

Unit - Create 3 CreepList[1] for Player 1 (Red) at (Center of temp_location) facing Default building facing degrees

Set temp_group = (Last created unit group)

Unit Group - Order temp_group to Attack-Move To (Center of blue base <gen>)

this will create a spawn of 3 footmen at a region called 'red 1' to attack-move toward a region called 'blue base' although you could send them from anywhere to anywhere you wanted.

I dont really feel like writing out triggers for every spawn point, but you should be able to see the basic structure each spawn should follow. Now, when it comes time to spawn something besides footmen from red 1, we add a trigger which turns off Red 1 Spawns, and turns on the replacement spawn, which might look like this

the replacement spawn

Red 1 NewSpawns
Events
Time - Every 20.00 seconds of game time

Conditions

Actions
Set temp_location = red 2 <gen>

Unit - Create 3 CreepList[2] for Player 1 (Red) at (Center of temp_location) facing Default building facing degrees

Set temp_group = (Last created unit group)

Unit Group - Order temp_group to Attack-Move To (Center of blue base <gen>)

by changing the index of the variable to creeplist[2] we have changed the spawning unit to what we set in the initialization, knights.

and the switching trigger, which switches the first spawn off and the 2nd spawn on, when the tower in that region takes damage (you can pick anything for the condition, but unit damage/death is typical)

Spawn switch
Events
Unit - Scout Tower 0007 <gen> Takes damage

Conditions

Actions
Trigger - Turn off Red 1 Spawns <gen>

Trigger - Turn on Red 1 NewSpawns <gen>

I did not include any memory leak fixes in this explanation because that is covered in  here

#### EricDaRed

#### EricDaRed

 Level 4

JoinedJul 23, 2007Messages129

As typical, Yamateh displays a complete lack of understanding... just kidding!

On the subject of leaks, a very nice explanation can be found at http://www.thehelper.net/forums/showthread.php?t=27219.

#### blacksniper

B

#### blacksniper

meh this might be a stupid question but i can't find


" Wait until ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True), checking every 1.00 seconds "

#### p8intballer

#### p8intballer

 Level 2

JoinedMay 20, 2008Messages14

rApp said:

 I need help, what does "bj_wantDestroyGroup = true" means, whats bj_wantDestroyGroup?


Thx


Click to expand...


thats a gui function in jass i belive its used when u cant do something in gui

#### p8intballer

#### p8intballer

 Level 2

JoinedMay 20, 2008Messages14

blacksniper said:

 meh this might be a stupid question but i can't find


" Wait until ((All units of (Units owned by Player 12 (Brown)) are dead)Equal to True), checking every 1.00 seconds "


Click to expand...


alright its a boolaien


unit group


units of unit group are dead


units owned by player


player 12 brown

#### turtlelink2

#### turtlelink2

 Level 4

JoinedJun 14, 2008Messages72

iv compleated this and it doesnt do anything,...... is there any way i could just import thing trigger or somthing?

#### p8intballer

#### p8intballer

 Level 2

JoinedMay 20, 2008Messages14

turtlelink2 said:

 iv compleated this and it doesnt do anything,...... is there any way i could just import thing trigger or somthing?


Click to expand...


the only way to copy a trigger is if someone makes it for you in jass or puts it in a map


other wise you will have to do it yourself

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

This should be updated as it is still very inefficient.

#### pred1980

#### pred1980

 Level 13

JoinedMar 19, 2010Messages870

Resources: 1

#### Resources
Maps 1

LOL, do you really think that someone work on this system or will change anything  - It's a 6 years old thread my friend

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

pred1980 said:

 LOL, do you really think that someone work on this system or will change anything  - It's a 6 years old thread my friend

Click to expand...


It doesn't matter how old a tutorial is. It should be kept up to date (with lenience) or removed.

#### pred1980

#### pred1980

 Level 13

JoinedMar 19, 2010Messages870

Resources: 1

#### Resources
Maps 1

DoOs_101 -->

Last Activity: 03-04-2012 07:22 AM

I don't think he'll keep this tutorial up to date. Maybe someone else.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Only mods can do that, sadly.

 You must log in or register to reply here.


### Similar threads

Locked

 Creep Spawn Help. Start timer after all creeps die.


networkmancer

Jan 17, 2011

Triggers & Scripts

Replies4Views1K

Jan 18, 2011
ironman47

Locked

[Trigger]
 I Need Help to Solve with this bug


DoOs_101

Jan 17, 2007

Triggers & Scripts

Replies4Views1K

Jan 29, 2007
Dr Super Good

Locked

 How can I add this trigger?


networkmancer

Jan 30, 2011

World Editor Help Zone

Replies4Views1K

Feb 1, 2011
ironman47

Locked

[Trigger]
 resetting timer bugs out spawn


CodeBlack

Nov 3, 2018

Triggers & Scripts

Replies6Views2K

Nov 6, 2018
CodeBlack

Locked

 Trigger help! Why the timer runs twice. HELP


networkmancer

Feb 2, 2011

World Editor Help Zone

Replies4Views2K

Feb 2, 2011
networkmancer

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