---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-make-an-ai-for-your-map.245613/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Make an AI for Your Map
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-make-an-ai-for-your-map.245613/>
#### sentrywiz

S

#### sentrywiz

# How to make an AI for your map

### NOTE
This tutorial assumes that you have an adept knowledge in the GUI part of the World Editor.


You do not have to know JASS or any other code to read through this tutorial.


Everything here will be triggers, with very little script structure.


This tutorial will NOT be addressing common concepts and mechanics,


such as triggering loops, removing leaks or explaining in minor detail


where you can find the conditions and actions of the triggers.


This tutorial assumes you have this knowledge. If you do not, then this


tutorial may seem confusing to you. I suggest you obtain some knowledge


 before reading, either via other tutorials or practice. Thank you

### My Thoughts On AI
AI or Artificial Intelligence is something that in my opinion (IMO) should be present in every map.


However each map is unique to itself and no AI would work the same for all maps.


I can understand that map devs usually just want to get their project alive,


get done with triggers or code and have a working map.


Unfortunately, to many devs the idea of an AI is great,


but many think of AI making as maybe a waste of effort, time and resources.


I think people believe AI is hard to make and requires complex coding or lots of huge triggers.


While some AI really has complex code structures behind it,


a basic functional AI is nothing complex and adds BIG BONUS to your map.


For one it will ensure that no matter what, your map can be played solo.


The experience will definately not be the same like you are playing with a real player.


I for one, am not a battle.net or a garena player and any map


that allows me to play alone with bots and doesn't take away from the


fun because its not a real player, is a lot of points in my book.


The best AI I have ever seen is the one in DOTA. No wonder I still play


dota with bots. They make the experience so fulfilling. But there are


many other maps like that. Castle Fight, Pudge Wars... all have a great


and exceptionally well made AI. And you can have that kind of AI, it will


take some resources and effort but it is really easy once your read


through this tutorial and understand how fun and easy can it be


for you to design your own AI for your maps.

### AI Structure and Game States


This is the MEAT of the tutorial. It will


teach you the basic method and the template


that you use to shape the triggers.

Game States is a method of making your triggers.


With game states the AI making is simplified and you can easily adopt old and new


concepts into a slick and easy triggers or JASS code. What game states is about:


- Creating a 'state' for every action you want your AI to do


- Managing all 'states' via a control state, called the MAIN STATE


- Planning to avoid leaks and conflicts in your main state


- Realization of your main state conditions


All this means that when creating an AI for your map, you want to start your


template of the AI by first defining what your AI is supposed to be doing.


You can brainstorm this process with the actions you want your AI to do.


Basic actions can involve:


- Move, Attack, Patrol, Jungle, Gank, Buy Items, Cast Spells, Level Up, Assist Allies etc...


When you have your basic actions defined, you want to scour and see what you don't


want your AI to do. This is the opposite of the first brainstorming, and it can give you


a basic preparation for the problems your AI might face. It involves actions like:


- Not Waste Mana, Not Jungle, Not Buy Random Items, Not Initiate Fights First etc...


This adds to your process of creation. The basic actions give you a primary blueprint


to work with. The actions you don't want your AI to be doing add to the details of


your blueprint. It causes you to focus on great details that usually leads to a great AI.


Don't skip your don't actions process.


Now when you have your do's and dont's about your AI, its time to figure out your


control state or MAIN STATE. This is a state which will constantly monitor your AI and


give him commands regarding all decisions the AI is supposed to be doing. This will


be your bread and butter, because without it, your AI is as good as not working.


Usually this will be your main trigger or triggers which will compare all inputs for the


AI and cause them to react and decide in a great manner.


Your MAIN STATE should be composed of:


- Constant monitoring and comparrison (Every X Seconds)


- An array of conditions and actions (Multiple IF/ELSE)


The actions you brainstormed will be filling every nook and cranny of the IF/ELSE


conditions. This will lead the AI to have a valid response to most situations you


can think of. It will be able to do most things players do with great accuracy.


The MAIN STATE conditions have to be planned out carefully. Because if you


add many confusing conditions, the MAIN STATE will have no option but to


enforce the AI to make dumb decisions. So the more UNIQUE the condition is


the better your AI will be. If you make similar conditions between multiple


IF/ELSE's your AI will be confused and will often be stuck in infinite loops,


attempting to do both actions at the same time.


You will spend most of your time testing your MAIN STATE conditions.


As I said, this is your bread and butter and a key to an amazing AI.


Its up to you how much tweaking you will do and how UNIQUE and


clear your conditions will be.


The rest of this tutorial will be focused on examples and triggers.


I will attempt to do AI for different maps over time, so check


back on this tutorial to see if I have made it or leave questions.


I am more than happy to help you with ideas about your AI.

### Example #1: Single Lane AoS (AI)


The map will be a short lane in which the AI will have to


go through lines of defences, level his skills, use them,


buy items when enough gold is acquired and will have


the option to go back to a base to heal when wounded.


This is how the map looks like:


The AI will take control over the Mountain King hero


which has the default spells: Storm Bolt, Thunder Clap,


Bash and Avatar.


The AI will have to retreat to this base when on low


health in order to replenish health, mana and possibly


buy items if it has enough gold.


The AI will move the Hero along the map and have a


progression scale, allowing the Hero to get level 6


before engaging into the final battle:


The AI will move up to this point at which the final


battle it will use all of its skills, use an item and win


the game by killing all neutral hostile units.

### The AI Itself
This is the model the AI will be based upon:


- MAIN STATE (Controls the other states and the survival of the AI)


- Move Attack (When the AI is at full health, it will move and attack)


- Back to Heal (When the AI is wounded, it will return to base)


- Use Spells Items (When the AI engages in combat, use spells and items)


- Level Up (When the AI levels up, decide which skills it will level)


- Buy Items (When the AI retreats to base, buy items)


With this in mind, we now have our model for triggering.


First is the main state, the heart of the AI operations.

MAIN STATE
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Set AI_Group = (Units owned by Player 1 (Red) matching ((((Matching unit) is alive) Equal to True) and ((Unit-type of (Matching unit)) Equal to Mountain King)))

Unit Group - Pick every unit in AI_Group and do (Actions)
Loop - Actions
-------- If Hero has less than 25% Health and is not Healing --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of (Picked unit)) Less than or equal to 25.00

Healing Not equal to True

Then - Actions
Trigger - Run Back to Heal <gen> (ignoring conditions)

Else - Actions

-------- If Hero has 100% Health and is Healing --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of (Picked unit)) Equal to 100.00

Healing Equal to True

Then - Actions
Trigger - Run Move Attack <gen> (ignoring conditions)

Else - Actions

-------- If Hero has more than 25% health and isn't healing --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of (Picked unit)) Greater than 25.00

Healing Not equal to True

Then - Actions
Trigger - Run Move Attack <gen> (ignoring conditions)

Else - Actions

Custom script: call DestroyGroup ( udg_AI_Group )

We will now look at this bit by bit. At first it looks like a chunk


of triggers, but this is what our control state looks like.

Time - Every 1.00 seconds of game time
Every second it will check and manage the AI.

Set AI_Group = (Units owned by Player 1 (Red) matching ((((Matching unit) is alive) Equal to True) and ((Unit-type of (Matching unit)) Equal to Mountain King)))
Our main unit group that picks the Hero used by the AI.

Unit Group - Pick every unit in AI_Group and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of (Picked unit)) Less than or equal to 25.00

Healing Not equal to True

Then - Actions
Trigger - Run Back to Heal <gen> (ignoring conditions)

Else - Actions

Our first IF/ELSE. We declare a boolean var Healing which will


depict a condition that the AI is healing in base. We then make our


IF/ELSE by saying that if the Hero isn't healing, which means he is


not in base AND his total health is less than 25%, initiate state


BACK TO HEAL.

Back to Heal
Events

Conditions

Actions
Set Healing = True

Set AI_Loc = (Position of Fountain of Health 0000 <gen>)

Unit - Order (Picked unit) to Move To AI_Loc

Custom script: call RemoveLocation ( udg_AI_Loc )

This is our BACK TO HEAL state in which we set the Healing


var to true, set the location of the fountain, move the Hero


back to base and remove the location to avoid leaks. As you


can see, there are no events and conditions, so this state can


only be called from the main state. It will not run on its own

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of (Picked unit)) Equal to 100.00

Healing Equal to True

Then - Actions
Trigger - Run Move Attack <gen> (ignoring conditions)

Else - Actions

Now we go back to our MAIN STATE, and look at the second IF/ELSE.


If the Hero's total health is at 100% and the Healing var is true,


then we run the MOVE ATTACK state.

Move Attack
Events

Conditions

Actions
Set Healing = False

Set AI_Loc = (Center of Attack Region <gen>)

Unit - Order (Picked unit) to Attack-Move To AI_Loc

Custom script: call RemoveLocation ( udg_AI_Loc )

Our MOVE ATTACK state sets the Healing var to false,


sets the location of the region where our Hero needs to go,


issues a move-attack order and removes the location.

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of (Picked unit)) Greater than 25.00

Healing Not equal to True

Then - Actions
Trigger - Run Move Attack <gen> (ignoring conditions)

Else - Actions

We look at the final IF/ELSE of the MAIN STATE. It is issued


when the health of the Hero is more than 25% and the


Healing var is set to false. If both are true, then we call our


MOVE ATTACK state again.

Custom script: call DestroyGroup ( udg_AI_Group )
Last but not least, we clean our group variable to avoid leaks.


Now let's explain all we have read so far. The MAIN STATE


has 3 IF/ELSE conditions. One checks if the Hero is at


25% or less health and isn't healing. If yes, it sends it back


to base to heal. The second IF/ELSE checks if the Hero is


at base healing and if its health is 100% and sends it back


fighting if true.


The third IF/ELSE is ment if the AI has finished


killing a wave of defences but it still has more than 25% health.


In which case the AI will be stuck there, doing nothing until it can


regenerate back to full health and the second IF/ELSE can be run


again. The third IF/ELSE pushes the AI to fight until it has 25%


or less health, in which case it sends it back to base to heal.

Level Up
Events
Unit - A unit Gains a level

Conditions
(Unit-type of (Leveling Hero)) Equal to Mountain King

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Level of (Leveling Hero)) Equal to 3

(Level of (Leveling Hero)) Equal to 5

Then - Actions
Hero - Learn skill for (Leveling Hero): Storm Bolt (Hero)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Level of (Leveling Hero)) Equal to 2

(Level of (Leveling Hero)) Equal to 8

(Level of (Leveling Hero)) Equal to 10

Then - Actions
Hero - Learn skill for (Leveling Hero): Thunder Clap

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Level of (Leveling Hero)) Equal to 4

(Level of (Leveling Hero)) Equal to 7

(Level of (Leveling Hero)) Equal to 9

Then - Actions
Hero - Learn skill for (Leveling Hero): Bash

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of (Leveling Hero)) Equal to 6

Then - Actions
Hero - Learn skill for (Leveling Hero): Avatar

Else - Actions

Our LEVEL UP state, which manages the skills the AI takes.


Because we have the freedom and we know what skills


the Hero has, we can therefor set at which level does the AI


take a specific skill.

Use Spells Items
Events
Unit - A unit Is attacked

Conditions
(Unit-type of (Attacking unit)) Equal to Mountain King

(Percentage life of (Attacking unit)) Greater than 25.00

Actions
Set Hero_Loc = (Position of (Attacking unit))

Set Enemies_Around = (Units within 250.00 of Hero_Loc matching ((((Matching unit) belongs to an enemy of (Owner of (Attacking unit))) Equal to True) and (((Matching unit) is alive) Equal to True)))

-------- If Hero has Scroll, use it at the start of the fight --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Attacking unit) has an item of type Scroll of Protection) Equal to True

Then - Actions
Hero - Order (Attacking unit) to use (Item carried by (Attacking unit) of type Scroll of Protection)

Else - Actions

-------- We use Storm Bolt as a finisher, when the enemy has 100 * Level health remaining --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Storm Bolt (Hero) for (Attacking unit)) Greater than 0

(Mana of (Attacking unit)) Greater than or equal to 75.00

(Life of (Attacked unit)) Less than or equal to (Real(((Level of Storm Bolt (Hero) for (Attacking unit)) x 100)))

Then - Actions
Unit - Order (Attacking unit) to Human Mountain King - Storm Bolt (Attacked unit)

Else - Actions

-------- We use Thunder Clap only when there are more than 1 enemy at 250 range around the Hero --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Thunder Clap for (Attacking unit)) Greater than 0

(Percentage mana of (Attacking unit)) Greater than or equal to 50.00

(Number of units in Enemies_Around) Greater than 1

Then - Actions
Unit - Order (Attacking unit) to Human Mountain King - Thunder Clap

Else - Actions

-------- We use Avatar when the hero's health is less than 50% --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Avatar for (Attacking unit)) Greater than 0

(Mana of (Attacking unit)) Greater than or equal to 150.00

(Percentage life of (Attacking unit)) Less than or equal to 50.00

Then - Actions
Unit - Order (Attacking unit) to Human Mountain King - Activate Avatar

Else - Actions

Custom script: call DestroyGroup ( udg_Enemies_Around )

Custom script: call RemoveLocation ( udg_Hero_Loc )

Our CAST SPELLS ITEMS state. This is where the AI will shine


and use spells with very specific conditions. We will look at this


bit by bit. Let's start with the basics:

Use Spells Items
Events
Unit - A unit Is attacked

Conditions
(Unit-type of (Attacking unit)) Equal to Mountain King

(Percentage life of (Attacking unit)) Greater than 25.00

We use the CAST SPELLS ITEMS state when the Hero is attacked.


Since the Hero will encounter enemies that will attack it, this


is when it will decide on the use of spells. Also since this state


isn't controlled via the MAIN STATE, it will run on its own so we


have to define that if the Hero's health is lower than 25%, the


entire state is skipped even if the Hero is attacked.

Use Spells Items
Actions
Set Hero_Loc = (Position of (Attacking unit))

Set Enemies_Around = (Units within 250.00 of Hero_Loc matching ((((Matching unit) belongs to an enemy of (Owner of (Attacking unit))) Equal to True) and (((Matching unit) is alive) Equal to True)))

We set a Hero_Loc variable to hold the position of the Hero.


And a Enemies_Around unit group that will see if there are


units at 250 range around the position of the Hero. This is necessary


for our Thunder Clap spell and will be explained shortly.

Use Spells Items
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Attacking unit) has an item of type Scroll of Protection) Equal to True

Then - Actions
Hero - Order (Attacking unit) to use (Item carried by (Attacking unit) of type Scroll of Protection)

Our first IF/ELSE is whether the hero has an item


Scroll of Protection. If it does, it uses it.

Use Spells Items
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Storm Bolt (Hero) for (Attacking unit)) Greater than 0

(Mana of (Attacking unit)) Greater than or equal to 75.00

(Life of (Attacked unit)) Less than or equal to (Real(((Level of Storm Bolt (Hero) for (Attacking unit)) x 100)))

Then - Actions
Unit - Order (Attacking unit) to Human Mountain King - Storm Bolt (Attacked unit)

Else - Actions

The Hero's Storm Bolt spell costs 75 mana and deals 100/200/300


damage. Let's say that we want the AI to preserve mana and use


the Storm Bolt as a finisher, to kill off the wounded enemy. This means


it has to know when to cast it.


The IF/ELSE checks if the Hero's mana is 75 or greater, sees if the level


of Storm Bolt is at least one and checks if the unit attacking


our Hero has health less or equal to 100 * Level of Storm Bolt. If yes, a Storm Bolt is cast

Use Spells Items
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Thunder Clap for (Attacking unit)) Greater than 0

(Percentage mana of (Attacking unit)) Greater than or equal to 50.00

(Number of units in Enemies_Around) Greater than 1

Then - Actions
Unit - Order (Attacking unit) to Human Mountain King - Thunder Clap

Else - Actions

Remember our top two variables, Hero_Loc and Enemies_Around?


Let's say we want our AI to only cast Thunder Clap when it has


at least 2 enemies at 250 range around it. This will ensure our


spell gets a good value from being used. And let's also say that


since we want our AI to cast as many Storm Bolts as possible


without spending too much mana on Thunder Clap.


Since our Thunder Clap deals less damage and costs 90 mana


this means we need the AI to make a good decision of when


to use it and when not to. This is where our variables come into


play.


Our Enemies_Around group checks for all enemy units at 250


range around our Hero_Loc position. So we check if the number


of units is 2 or more and we check if the mana of the Hero is


atleast 50% or more. This saves mana for our Storm Bolts and


only uses Thunder Clap in good situations and when the Hero


has excess mana.

Use Spells Items
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Avatar for (Attacking unit)) Greater than 0

(Mana of (Attacking unit)) Greater than or equal to 150.00

(Percentage life of (Attacking unit)) Less than or equal to 50.00

Then - Actions
Unit - Order (Attacking unit) to Human Mountain King - Activate Avatar

Else - Actions

Last but not least, we have our Hero's ultimate Avatar.


Since Avatar gives bonus hitpoints, armor and damage and


spell immunity, we can use it as a weird heal our hero can have.


For this example, I set Avatar to be cast when the Hero has


150 mana or more and when it is at 50% or less health. This


makes Avatar be used more as a heal and less as a buff.

Use Spells Items
Custom script: call DestroyGroup ( udg_Enemies_Around )

Custom script: call RemoveLocation ( udg_Hero_Loc )

We clean our used variables for our Thunder Clap


checking, avoiding leaks.

Init
Events
Map initialization

Conditions

Actions
Trigger - Turn off (This trigger)

Hero - Learn skill for Mountain King 0017 <gen>: Storm Bolt (Hero)

Set Healing = False

Set AI_Loc = (Center of Attack Region <gen>)

Unit - Order Mountain King 0017 <gen> to Attack-Move To AI_Loc

Custom script: call RemoveLocation ( udg_AI_Loc )

Lastly, we have an INIT trigger in which we set the first pick


skill for our Hero to be Storm Bolt. And since the states become


active once the Hero starts fighting and going back to base, the


INIT trigger launches the Hero into battle. This trigger only runs


once, at the start of the game. Its just for testing purposes


Also, if you are interested in a MUI Solution to these triggers


because there were made for a single unit in mind, here is an


actual solution. Basically its just changing the Healing var to


an Array and changing the main AI_Group so it can pick


multiple heroes instead of one. Do bare in mind that the


only hero in the triggers is Mountain King, if you want more


heroes you will have to do the spells and level up triggers for


them as well.

MAIN STATE MUI
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Set AI_Group = (Units in (Playable map area) matching ((((Matching unit) is A Hero) Equal to True) and ((((Matching player) controller) Equal to Computer) and ((Owner of (Matching unit)) Not equal to Neutral Hostile))))

Unit Group - Pick every unit in AI_Group and do (Actions)
Loop - Actions
-------- If Hero has less than 25% Health and is not Healing --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of (Picked unit)) Less than or equal to 25.00

Healing_MUI[(Player number of (Owner of (Picked unit)))] Not equal to True

Then - Actions
Trigger - Run Back to Heal <gen> (ignoring conditions)

Else - Actions

-------- If Hero has 100% Health and is Healing --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of (Picked unit)) Equal to 100.00

Healing_MUI[(Player number of (Owner of (Picked unit)))] Equal to True

Then - Actions
Trigger - Run Move Attack <gen> (ignoring conditions)

Else - Actions

-------- If Hero has more than 25% health and isn't healing --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of (Picked unit)) Greater than 25.00

Healing_MUI[(Player number of (Owner of (Picked unit)))] Not equal to True

Then - Actions
Trigger - Run Move Attack <gen> (ignoring conditions)

Else - Actions

Custom script: call DestroyGroup ( udg_AI_Group )

Move Attack MUI
Events

Conditions

Actions
Set Healing_MUI[(Player number of (Owner of (Picked unit)))] = False

Set AI_Loc = (Center of Attack Region <gen>)

Unit - Order (Picked unit) to Attack-Move To AI_Loc

Custom script: call RemoveLocation ( udg_AI_Loc )

Back to Heal MUI
Events

Conditions

Actions
Set Healing_MUI[(Player number of (Owner of (Picked unit)))] = True

Set AI_Loc = (Position of Fountain of Health 0000 <gen>)

Unit - Order (Picked unit) to Move To AI_Loc

Custom script: call RemoveLocation ( udg_AI_Loc )

### Summary -- Last Words
I conclude this tutorial with the hope that you saw


that making an AI can be a fun process.


Its a matter of you thinking about the states you


want your AI to do. The above provides you with a


template that you can base your own ideas on.


Its important to understand that every action


that you think the AI can do can be upgraded as


a state and used by a control state. You don't need


lots of huge triggers, just smart IF/ELSE and all the


game states you want.

View attachment AI Tutorial sentry.w3x


 Last edited by a moderator: Aug 6, 2014

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

yey, AI tutorial =)

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

"You can download this map below" No download available  I want to see the bot in action

and yes its nice to see bot tutorial, even tho its GUI one(its quite simple too)

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

sadly this seems very specific. This only cover a few aspect that an AI can use.

however you can indeed pick up some tricks here if youre new.

#### sentrywiz

S

#### sentrywiz

Chaosy said:

 sadly this seems very specific. This only cover a few aspect that an AI can use.

however you can indeed pick up some tricks here if youre new.


Click to expand...

Unfortunately, you cannot make a non-specific AI tutorial.

Every map is unique and needs an unique AI.

That's why game states is the topic.

The AI is merely an example.

I think you mistook the tutorial. Its not to make an AI for

a lane specific map. Its about game states and showing

people how they can make their own AI.

 "You can download this map below" No download available  I want to see the bot in action

and yes its nice to see bot tutorial, even tho its GUI one(its quite simple too)


Click to expand...

Many thanks, I forgot to attach the map. Here it is now

 yey, AI tutorial =)


Click to expand...

Yeah finally lol

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

well even so the tutorial does only cover few types of maps. I mean there is only one attack region. That would never work in an bigger map, that's the hard part that you dont show.

#### sentrywiz

S

#### sentrywiz

Chaosy said:

 well even so the tutorial does only cover few types of maps. I mean there is only one attack region. That would never work in an bigger map, that's the hard part that you dont show.


Click to expand...

EDIT:

This tutorial is about game states as a method of making your own AI.

Its a lot of work if I were to make an AI for every possible map.

But the logic behind the triggers will work for every map.

Whether is 100 regions or 1, its just more IF/ELSE and more states.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

yeah for example. each hero got different points to move towards. They cant just move there either because then they will run into towers or running through the jungle. (if you think of the dota map).

the tutorial is not bad but I suggest making it more advanced

also I would call this a template then

#### sentrywiz

S

#### sentrywiz

Chaosy said:

 yeah for example. each hero got different points to move towards. They cant just move there either because then they will run into towers or running through the jungle. (if you think of the dota map).

the tutorial is not bad but I suggest making it more advanced


Click to expand...

I can't make an AI for every possible map just so I can look at

the various game states that would interact one with another.

What you wrote is a specific AI, and we can discuss this.

But it has nothing to do with the tutorial. I only showcase game states

as a method to make your own AI.

Maybe what you're asking is how I would make a Dota AI?

EDIT:

 also I would call this a template then


Click to expand...

I did write this at the end of the tutorial.

This is exactly what you said: a template.

Its a matter of you thinking about the states you

want your AI to do. The above provides you with a

template that you can base your own ideas on.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

ofcourse you cant make an AI for every map with a tutorial. However showing how to make an AI with defferent points for different units is kinda key to any AI.

also I didnt take dota as an example, you took it as an example yourself I just followed.

my bad I actually missed that you wrote that at the end.

edit: oh you edited that part. but before you used a 3 lane map as an example. I just followed

#### sentrywiz

S

#### sentrywiz

Chaosy said:

 ofcourse you cant make an AI for every map with a tutorial. However showing how to make an AI with defferent points for different units is kinda key to any AI.


also I didnt take dota as an example, you took it as an example yourself I just followed.


my bad I actually missed that you wrote that at the end.


edit: oh you edited that part. but before you used a 3 lane map as an example. I just followed


Click to expand...


I see.


Well Dota is a good example of what I consider an end-game AI.


Of course, the AI in Pudge Wars is end-game. The Castle Fight AI is also


end-game. Those are all different AI but well made. They all follow a state based system. So that's kind of the key here, not the AI structure itself.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Hmm... the only part I don't like is that some of it aren't MUI... like the healing check which uses a single variable...

oh wait, judging from other parts, the set-up seems to be for 1 unit only... so why use a unit-group picking for the AI???

it would have been good if it was made MUI...

#### sentrywiz

S

#### sentrywiz

Adiktuz said:

 Hmm... the only part I don't like is that some of it aren't MUI... like the healing check which uses a single variable...


oh wait, judging from other parts, the set-up seems to be for 1 unit only... so why use a unit-group picking for the AI???


it would have been good if it was made MUI...


Click to expand...


I don't know, I just used unit group instead of just picking up the unit itself.


The Healing var can be easily changed to an Array var to affect multiple AI units. Its just more clicking, because then each array has to be with:


Player Number of Owner of Picked Unit.


EDIT:

MAIN STATE MUI
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Set AI_Group = (Units in (Playable map area) matching ((((Matching unit) is A Hero) Equal to True) and ((((Matching player) controller) Equal to Computer) and ((Owner of (Matching unit)) Not equal to Neutral Hostile))))

Unit Group - Pick every unit in AI_Group and do (Actions)
Loop - Actions
-------- If Hero has less than 25% Health and is not Healing --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of (Picked unit)) Less than or equal to 25.00

Healing_MUI[(Player number of (Owner of (Picked unit)))] Not equal to True

Then - Actions
Trigger - Run Back to Heal <gen> (ignoring conditions)

Else - Actions

-------- If Hero has 100% Health and is Healing --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of (Picked unit)) Equal to 100.00

Healing_MUI[(Player number of (Owner of (Picked unit)))] Equal to True

Then - Actions
Trigger - Run Move Attack <gen> (ignoring conditions)

Else - Actions

-------- If Hero has more than 25% health and isn't healing --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Percentage life of (Picked unit)) Greater than 25.00

Healing_MUI[(Player number of (Owner of (Picked unit)))] Not equal to True

Then - Actions
Trigger - Run Move Attack <gen> (ignoring conditions)

Else - Actions

Custom script: call DestroyGroup ( udg_AI_Group )

Move Attack MUI
Events

Conditions

Actions
Set Healing_MUI[(Player number of (Owner of (Picked unit)))] = False

Set AI_Loc = (Center of Attack Region <gen>)

Unit - Order (Picked unit) to Attack-Move To AI_Loc

Custom script: call RemoveLocation ( udg_AI_Loc )

Back to Heal MUI
Events

Conditions

Actions
Set Healing_MUI[(Player number of (Owner of (Picked unit)))] = True

Set AI_Loc = (Position of Fountain of Health 0000 <gen>)

Unit - Order (Picked unit) to Move To AI_Loc

Custom script: call RemoveLocation ( udg_AI_Loc )


This is a quick solution for the triggers to be MUI. Then you can make new heroes, add them and they will all behave the same way. However since we only did Spells and Level Up for the Mountain King, they would also need triggers for their spells and levels or they wouldn't cast any or level their own.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

which is why it's always nice to come with at least two examples for these things...

#### sentrywiz

S

#### sentrywiz

Adiktuz said:

 which is why it's always nice to come with at least two examples for these things...

Click to expand...


I think I will follow on Chaosy's advice and make multiple AI triggers for multiple kinds of maps.


But yeah, thanks for letting me know about the MUI. I will copy these triggers into the main post, just because other people might be interested in a MUI solution.

#### sentrywiz

S

#### sentrywiz

UPDATED!

Re-wrote the entire Game States.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

sentrywiz said:

 AI or Artificial Intelligence is something that in my opinion (IMO) should be present in every map.


Click to expand...


Singleplayer maps like point'n'click adventures?


Your idea as stated here is to lock entities like units with tasks but further below you say one has to pay attention to write exclusive conditions to not create infinite loops:

sentrywiz said:

 The MAIN STATE conditions have to be planned out carefully. Because if you


add many confusing conditions, the MAIN STATE will have no option but to


enforce the AI to make dumb decisions. So the more UNIQUE the condition is


the better your AI will be. If you make similar conditions between multiple


IF/ELSE's your AI will be confused and will often be stuck in infinite loops,


attempting to do both actions at the same time.


Click to expand...


If you really want the locking, then your "main state" trigger can simply block incoming instructions after one has been set. However, that is not very flexible. There may be more important ones than what is currently in action. See process scheduling, interrupts. A better concept is to weight the actions in priorities, maybe even dynamic priorities.


On the other hand, if you let the instructors freely go battle royal, that will become one major class of problems and I see no other paradigms here readers could pick up to deal with that. The more your map grows, the more conditions and actions bloat up. When everything references and needs to consider other scopes, it gets very detailed but also convoluted, hence why paradigms and inter-layers shoud lead.


And well, there are other slices of this topic you could have at least marginally addressed. You call that AI but you only covered very simple reactive, deterministic mechanics. A good intelligence should not only see the current and local values but have a global understanding and forecast prospects, maybe come with a personality. From a planning being, you expect that it learns in the course. The map background itself is static, that can be precompiled but during the game, an intelligence may also analyze the behavior of the other players and acquire previously fogged information.


It's not only about units. The AI players must be capable to do almost any form of input like a human. Maybe forbid rage quits though  As the case may be, the mapmaker has to write extra routines. For example, a gameplay-relevant dialog must be selected or if you give the players control via chat commands, the cpu has to have some access to it too. That of course may have nothing to do with your task schedule since it can be handled in parallel.


You dropped one more interesting statement:

sentrywiz said:

 Since Avatar gives bonus hitpoints, armor and damage and


spell immunity, we can use it as a weird heal our hero can have.


[...]This


makes Avatar be used more as a heal and less as a buff.


Click to expand...


You categorize the ability 'Avatar'. Categorizing can shape inter-layers. Imagine you have two shield abilities that render a unit magic immune. Therefore you can flag them as "grants magic immunity" and when the AI finds need to make a unit immune at some point, it can regard its options and settle for the better one. In addition, the priority to cast such a spell may fall if the target possesses a similar effect already.

#### sentrywiz

S

#### sentrywiz

 Your idea as stated here is to lock entities like units with tasks but further below you say one has to pay attention to write exclusive conditions to not create infinite loops:


Click to expand...

What? Where did I say two opposite things? I said the actions should be planned out as states and later on I said that the conditions must be unique in order to prevent infinite loops.

I really don't understand your ranting. With what point did you write this? To call the AI simplistic? This is a tutorial, I'm not trying to impress people with my huge list of stuff the AI can do. This was done to illustrate the game states topic, not to bloat my ego.

 It's not only about units. The AI players must be capable to do almost any form of input like a human. Maybe forbid rage quits though  As the case may be, the mapmaker has to write extra routines. For example, a gameplay-relevant dialog must be selected or if you give the players control via chat commands, the cpu has to have some access to it too. That of course may have nothing to do with your task schedule since it can be handled in parallel.


Click to expand...

You talk a lot "Must do this, must do that". No. The mapmaker doesn't have to do all that. The AI don't have to forbid rage quit. Those are all POTENTIAL IDEAS, not musts.

This also has nothing to do with the tutorial, it has to do with what you think the AI should do.

 A good intelligence should not only see the current and local values but have a global understanding and forecast prospects, maybe come with a personality. From a planning being, you expect that it learns in the course. The map background itself is static, that can be precompiled but during the game, an intelligence may also analyze the behavior of the other players and acquire previously fogged information.


Click to expand...

How does an AI has a global understanding and forecast prospects? What does that even mean?

Also, you can add a game state "Analyze Other Players" and make something that analyzes the behavior of others.

BUT ITS STILL AN IDEA, NOT A MUST. What is it with you and all of these musts?

I really don't understand half your rant. Are you getting at that the AI should be something much much more? Okay, it might get to that point. The map maker might change the method of triggering later on, although I believe that game states is a great method for making an AI.

#### Idaero Ace

#### Idaero Ace

 Level 15

JoinedDec 21, 2013Messages911

Nice, actually AI is play with your brain, I have created one long time ago.


Spending 3 hours thinking and creating, the results is stupid


AI is make me stressed.....

#### DNA.h

#### DNA.h

 Level 2

JoinedJan 23, 2014Messages14

Hi all


first of all i want to say a BIG thanks to sentrywiz for his guide, and yeah for a beginner like me this tutorials are pure gold and thanks to lack of unprotected maps these guides are my one and only resources.


anyway i read the whole guide, downloaded the map, watched how it works and tried to improve this AI. my first idea was to use Storm Bolt on cooldown so i added a boolean variable and made this few changes (the Game - Display .... is only for debugging purpose)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Storm Bolt (Hero) for (Attacking unit)) Greater than 0

(Mana of (Attacking unit)) Greater than or equal to 75.00

BoltCooldown Equal to True

Then - Actions
Unit - Order (Attacking unit) to Human Mountain King - Storm Bolt (Attacked unit)

Set BoltCooldown = False

Game - Display to (All players) the text: Bolt is false now

Trigger - Run MyCooldown <gen> (ignoring conditions)

Else - Actions

MyCooldown
Events

Conditions

Actions
Wait 11.00 seconds

Set BoltCooldown = True

Game - Display to (All players) the text: Bolt is true now

Trigger - Turn off (This trigger)

but unfortunately the results in the action was awful. the hero used his ability only few times. the result must be like this


Bolt is true now


Bolt is false now


Bolt is true now


Bolt is false now


Bolt is true now


Bolt is false now


Bolt is true now


Bolt is false now


but in reality this is what happened


Bolt is true now


Bolt is false now


Bolt is true now


Bolt is false now


Bolt is false now


Bolt is true now


Bolt is true now


Bolt is false now


Bolt is true now


Bolt is false now


Bolt is true now


Bolt is false now


Bolt is false now


2 false or 2 true in a row is not good.fortunately i found the cause of the problem and let me describe it to you. in a 1vs1 situation where the hero was attacking just 1 unit it is working good but when Mountain King is fighting 2 or more units some time the trigger is running multiple times in a few millisecond interval. and somehow both Triggers try to order the Mountain King and set the BoltCooldown to false and therefore nothing happens at all.


I made a little change and added a WAIT like this

Then - Actions
Unit - Order (Attacking unit) to Human Mountain King - Storm Bolt (Attacked unit)

Wait 1.00 seconds

Set BoltCooldown = False

Game - Display to (All players) the text: Bolt is false now

and the result was MUCH better and hero used the storm bolt regularly BUT and the end of road where there are many units attacking the hero the problem rises again and the hero does no storm bolt. I'm a professional programmer but with few experiencing in world editor, if you are familiar with java or ... you can use synchronization on a function so that only one instance of it run at same time , how can i do the same thing here?


I found the "use ability on cooldown" a real pain in the ass cause i can't find any event for it and it's strongly needed for a good AI.

#### sentrywiz

S

#### sentrywiz

DNA.h said:

 Hi all

first of all i want to say a BIG thanks to sentrywiz for his guide, and yeah for a beginner like me this tutorials are pure gold and thanks to lack of unprotected maps these guides are my one and only resources.

anyway i read the whole guide, downloaded the map, watched how it works and tried to improve this AI. my first idea was to use Storm Bolt on cooldown so i added a boolean variable and made this few changes (the Game - Display .... is only for debugging purpose)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Storm Bolt (Hero) for (Attacking unit)) Greater than 0

(Mana of (Attacking unit)) Greater than or equal to 75.00

BoltCooldown Equal to True

Then - Actions
Unit - Order (Attacking unit) to Human Mountain King - Storm Bolt (Attacked unit)

Set BoltCooldown = False

Game - Display to (All players) the text: Bolt is false now

Trigger - Run MyCooldown <gen> (ignoring conditions)

Else - Actions

MyCooldown
Events

Conditions

Actions
Wait 11.00 seconds

Set BoltCooldown = True

Game - Display to (All players) the text: Bolt is true now

Trigger - Turn off (This trigger)

but unfortunately the results in the action was awful. the hero used his ability only few times. the result must be like this

Bolt is true now

Bolt is false now

Bolt is true now

Bolt is false now

Bolt is true now

Bolt is false now

Bolt is true now

Bolt is false now

but in reality this is what happened

Bolt is true now

Bolt is false now

Bolt is true now

Bolt is false now

Bolt is false now

Bolt is true now

Bolt is true now

Bolt is false now

Bolt is true now

Bolt is false now

Bolt is true now

Bolt is false now

Bolt is false now

2 false or 2 true in a row is not good.fortunately i found the cause of the problem and let me describe it to you. in a 1vs1 situation where the hero was attacking just 1 unit it is working good but when Mountain King is fighting 2 or more units some time the trigger is running multiple times in a few millisecond interval. and somehow both Triggers try to order the Mountain King and set the BoltCooldown to false and therefore nothing happens at all.

I made a little change and added a WAIT like this

Then - Actions
Unit - Order (Attacking unit) to Human Mountain King - Storm Bolt (Attacked unit)

Wait 1.00 seconds

Set BoltCooldown = False

Game - Display to (All players) the text: Bolt is false now

and the result was MUCH better and hero used the storm bolt regularly BUT and the end of road where there are many units attacking the hero the problem rises again and the hero does no storm bolt. I'm a professional programmer but with few experiencing in world editor, if you are familiar with java or ... you can use synchronization on a function so that only one instance of it run at same time , how can i do the same thing here?

I found the "use ability on cooldown" a real pain in the ass cause i can't find any event for it and it's strongly needed for a good AI.


Click to expand...

I'm glad you found it helpful.

First, this is just a tutorial. All the triggers involved are to help, not to confuse people who are new to the AI making process. Its ment to inspire you to improve the code on your own and to learn how to make improvements.

Its good practice and learning that you decided to change how the bolt functions but what you describe as a problem might be a problem for HOW YOU WANT the AI to react. In my case or in the tutorial -- I was ok with the idea of the bot being attacked by multiple units and casting the bolt on the first that was on low health.

First as an advice, you should avoid using Wait X Seconds, because while it might work for one player it is not MUI friendly (MUI being Multi-Unit Instanceability) so of course its gonna trigger multiple waits that all force the unit to wait and will cause either crashes or infinite loops of stuck-ness.

Secondly, if you want the trigger to count the cooldown of Storm Bolt and also stop the bot from casting it, here is an example of how I would do it:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Storm Bolt (Hero) for (Attacking unit)) Greater than 0

(Mana of (Attacking unit)) Greater than or equal to 75.00

Bolt_ON_Cooldown Equal to False

Then - Actions
Set Bolt_ON_Cooldown = True

Set Bolt_CD_Timer = 11

Unit - Order (Attacking unit) to Human Mountain King - Storm Bolt (Attacked unit)

Game - Display to (All players) the text: Storm Bolt is on CD.

Trigger - Turn on Bolt CD Trigger <gen>

Else - Actions

Bolt CD Trigger
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Set Bolt_CD_Timer = (Bolt_CD_Timer - 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Bolt_CD_Timer Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Set Bolt_ON_Cooldown = False

Else - Actions

The Bolt CD Trigger will count down the time of the Storm Bolt cooldown and turn itself off when its done. Then when the bot casts the Storm Bolt again,

the Bolt CD Trigger will be started and will count down from 11 again.,

#### Twilight

#### Twilight

 Level 13

JoinedSep 14, 2012Messages437

Resources: 30

#### Resources
Maps 30

This is a grate tutorial and all, rely good, tnx for tutorial, making AI on my map, but just one thing: HOW CAN I MAKE THE AI BYE ITEMS?

I try every single possible way that I can make the AI bye items, but they endup full with bugs, so please tell me how.

#### sentrywiz

S

#### sentrywiz

Twilight said:

 This is a grate tutorial and all, rely good, tnx for tutorial, making AI on my map, but just one thing: HOW CAN I MAKE THE AI BYE ITEMS?


I try every single possible way that I can make the AI bye items, but they endup full with bugs, so please tell me how.


Click to expand...


There is a section of this tutorial which focuses on buying items.


Please read it and post any questions regarding that.

#### iamdl

#### iamdl

 Level 8

JoinedMar 6, 2014Messages203

Resources: 8

#### Resources
Icons 1Spells and Systems 7

thanks man this tutorial help me on my map ! +rep for you! Btw can this trigger can cast channel spells?

#### grayback222

#### grayback222

 Level 4

JoinedJul 17, 2012Messages51

Resources: 5

#### Resources
Maps 3Spells 2

BUG!!!!!! Theres are bug in itema every time unit comes with in the store it buys 1 item in the loop in his trigger have 6 loops when the unit finish the 6 th loop it started coz a bug it start to buy again the 1sr item in then 1loop until it gets in6 th loop and buy and buy and buy until his base his base full of items


 Last edited by a moderator: Aug 6, 2014

#### Twilight

#### Twilight

 Level 13

JoinedSep 14, 2012Messages437

Resources: 30

#### Resources
Maps 30

grayback222 said:

 BUG!!!!!! Theres are bug in itema every time unit comes with in the store it buys 1 item in the loop in his trigger have 6 loops when the unit finish the 6 th loop it started coz a bug it start to buy again the 1sr item in then 1loop until it gets in6 th loop and buy and buy and buy until his base his base full of items


Click to expand...

Try If/Then/Else actions lol,

[trigger=My Trigger]Buy Items

 Events

 Unit - A unit enters shop000 <gen>

 Conditions

 Actions

 If(All conditions are True)then do(Then Actions)else do(Else Actions)

 If - Conditions

 ((Triggering Unit) hase (Item carried by (Triggering unit) in slot 1)) Equal to False

 Then - Actions

 Item - Create item for (Triggering unit)

 Else -Actions

 Do nothing[/trigger]


 Last edited by a moderator: Aug 6, 2014

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Sorry for the immense delay. I finally got around to reading the tutorial, and it is fine in its current state.

AI is multi-faceted: there are tons of ways of going about making one. As such, it is nice to have a guide to cover the basics, and your tutorial is well-organized. Hopefully it'll give rise to people making quality, competitive AI's specific to their own maps.

Approved.

#### sentrywiz

S

#### sentrywiz

PurgeandFire said:

 Sorry for the immense delay. I finally got around to reading the tutorial, and it is fine in its current state.


AI is multi-faceted: there are tons of ways of going about making one. As such, it is nice to have a guide to cover the basics, and your tutorial is well-organized. Hopefully it'll give rise to people making quality, competitive AI's specific to their own maps.


Approved.


Click to expand...


So much time has passed from me writing that I totally forgot.


Glad that you didn't though.


May it help those that need it.


Thanks for your approval

#### Yrk

#### Yrk

 Level 6

JoinedApr 1, 2014Messages239

Hi sentrywiz ,

Can you show me how to make the AI attack / chase the User / Computer ?


 Last edited: Sep 26, 2014

#### sentrywiz

S

#### sentrywiz

Yrk said:

 Hi sentrywiz ,


Can you show me how to make the AI attack / chase the User / Computer ?

Click to expand...


You mean chase after one specific hero?


There are many ways to make what you want.


An example: add a new "Chase Hero" state to the triggers that will be run if the AI is attacked by an enemy hero, it will only attack and cast spells on that hero instead of any other units that attack it.


And when the AI is engaged by multiple heroes, you can attack only the hero with the lowest health. If both heroes have equal amounts of health, then the AI can select a random of them.

#### Yrk

#### Yrk

 Level 6

JoinedApr 1, 2014Messages239


 sentrywiz said:



 You mean chase after one specific hero?

There are many ways to make what you want.

An example: add a new "Chase Hero" state to the triggers that will be run if the AI is attacked by an enemy hero, it will only attack and cast spells on that hero instead of any other units that attack it.

And when the AI is engaged by multiple heroes, you can attack only the hero with the lowest health. If both heroes have equal amounts of health, then the AI can select a random of them.


Click to expand...

Yes ( Like in DotA , If you played it  )

Can you show me in Trigger ?

Thanks in advance

#### sentrywiz

S

#### sentrywiz

Yrk said:

 Yes ( Like in DotA , If you played it  )

Can you show me in Trigger ?

Thanks in advance

Click to expand...

DotA's AI is the best AI I have seen. This tutorial is just the base point on which you could make such an amazing AI, but you would need to know plenty of other things.

Dota is a map that grew for many years and many people worked on it, so it's good to say that plenty of brilliant ideas influenced and made the AI the way it is today. You cannot accomplish such a feat overnight, nor would you want to hence the progression is where the fun is, not the end result.

No sorry, I won't do it for you. I can help you if you show me some triggers you did yourself and have a problem with, but the point of this tutorial is to teach you how to do it yourself. Otherwise I will just be doing it for you, which isn't in your best interest IF YOU WISH TO LEARN.

I did tell you how to do it though, in my previous post. Try experimenting with the triggers, and do something yourself.

However, if you just want a working AI for your map, you can go to the https://www.hiveworkshop.com/forums/requests-341/ page and ask people for it.

#### Yrk

#### Yrk

 Level 6

JoinedApr 1, 2014Messages239

Well , Thanks anyway

#### ukn0wnD3str0y3r

#### ukn0wnD3str0y3r

 Level 9

JoinedFeb 15, 2013Messages369

Resources: 8

#### Resources
Maps 2Spells and Systems 6

I Would give you credits and +rep if I used on my map =)


How if i have multiple lane?

#### sentrywiz

S

#### sentrywiz

ukn0wnD3str0y3r said:

 I Would give you credits and +rep if I used on my map =)

How if i have multiple lane?


Click to expand...

There are infinite ways to solve multiple lanes.

I will suggest 3 ways that come to mind right now:

1. Add all bot heroes to a group and make a region in each lane. Make a boolean variable "Botinlane[x]". Then pick a random unit from the group and if the botinlane[player_number] is equal to false, send it to a random of the X lanes and set its boolean variable to true. When the bot goes back to base to heal or dies, you can set this variable to false

2. You can make a region array variable that simply says "Whichlane[x]" that you can change throughout the game. That way if a bot hero has revived after death or when it kills an enemy, it will always return to the lane under its player number.

1. You could do complex checking per X seconds to check how many enemies are in each lane (both enemy and ally) and if there are enemy heroes there. You can use "Units in region" or "Units in group" under conditions and if the number exceeds a certain parameter like "More than 10 enemies" or "More than 1 enemy hero" send multiple bot heroes to that lane.

#### ukn0wnD3str0y3r

#### ukn0wnD3str0y3r

 Level 9

JoinedFeb 15, 2013Messages369

Resources: 8

#### Resources
Maps 2Spells and Systems 6

Example... i'm sucks in Boolean here Boolean There Sorry xD

#### sentrywiz

S

#### sentrywiz

ukn0wnD3str0y3r said:

 Example... i'm sucks in Boolean here Boolean There Sorry xD


Click to expand...


Boolean is a simple true/false variable. Its either true or false.


Here is an example of the first solution.

Untitled Trigger 001
Events

Conditions

Actions
Set botinlane[1] = False

Set botinlane[2] = False

Set botinlane[3] = False

Set botgroup = (Units in (Playable map area) matching ((((Matching unit) is alive) Equal to True) and (((Owner of (Matching unit)) controller) Equal to Computer)))

Set botgroup2 = (Random 1 units from botgroup)

Unit Group - Pick every unit in botgroup2 and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
botinlane[(Player number of (Owner of (Picked unit)))] Equal to False

Then - Actions
-------- send bot to lane --------

Set botinlane[(Player number of (Owner of (Picked unit)))] = True

Else - Actions
-------- nothing --------

#### ukn0wnD3str0y3r

#### ukn0wnD3str0y3r

 Level 9

JoinedFeb 15, 2013Messages369

Resources: 8

#### Resources
Maps 2Spells and Systems 6

So you checked it if the unit is computer and set the boolean to true and if true the other trigger will on?

#### sentrywiz

S

#### sentrywiz

ukn0wnD3str0y3r said:

 So you checked it if the unit is computer and set the boolean to true and if true the other trigger will on?


Click to expand...


Yeah. You would send a bot to the lane and set its boolean to true. Then the next time you do this trigger, if the unit is in lane, it will be skipped. Only bots that aren't in a lane and are alive will be affected

#### ukn0wnD3str0y3r

#### ukn0wnD3str0y3r

 Level 9

JoinedFeb 15, 2013Messages369

Resources: 8

#### Resources
Maps 2Spells and Systems 6

Owh okay

#### Mirage1

#### Mirage1

 Level 1

JoinedOct 31, 2015Messages98

Very good tutorial. There could be more of them on hive. I would like some help with a problem in my AI. When a hero who is in the AI group is controlled by a Computer player (Set by filling the player's slot with a Computer) some of the "default AI" commands are activated conflicting with the triggered commands.

I tried to create a null AI in AI Editor without orders, builds, attack groups, hero skill selection or anything and run it in these players. Made it entirely blank to not send any order as flee or use items. But it did not work. The AI still used items and skills.

Anyone know how to solve this? In Dota Allstars the AI is placed by computer slots and I doubt they interfere in the commands. What I need is a way to nullify these "default" commands of Warcraft so that they do not conflict with my triggered ones.

I searched for hours in the Hive and found nothing related. Any help is welcome.

#### sentrywiz

S

#### sentrywiz

Mirage1 said:

 Very good tutorial. There could be more of them on hive. I would like some help with a problem in my AI. When a hero who is in the AI group is controlled by a Computer player (Set by filling the player's slot with a Computer) some of the "default AI" commands are activated conflicting with the triggered commands.

I tried to create a null AI in AI Editor without orders, builds, attack groups, hero skill selection or anything and run it in these players. Made it entirely blank to not send any order as flee or use items. But it did not work. The AI still used items and skills.

Anyone know how to solve this? In Dota Allstars the AI is placed by computer slots and I doubt they interfere in the commands. What I need is a way to nullify these "default" commands of Warcraft so that they do not conflict with my triggered ones.

I searched for hours in the Hive and found nothing related. Any help is welcome.


Click to expand...

I hope you like the term "better late than never" because this is a 2 year reply

Sorry.

Yes the default AI commands cannot be controlled without triggers.

But you'd have to share your map or something in order to see whats wrong.

#### Mirage1

#### Mirage1

 Level 1

JoinedOct 31, 2015Messages98

Thx dude. But i don't even remember which map I was talking about haha

#### sentrywiz

S

#### sentrywiz

Mirage1 said:

 Thx dude. But i don't even remember which map I was talking about haha


Click to expand...

Yeah  you might have solved the problem so far.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Reminds me of my Game Design and Digital Circuit courses. An approach with Finite State Machine logic for Game AI.

#### sentrywiz

S

#### sentrywiz

Tbh, I left hive for a while, and now that I'm reading my own material after years ...

I'm learning a new game engine now to create my own games from scratch and this tutorial

reminded me of how easy it can be to make an AI (since everything's new for me in the engine)

Well finite is relative. I'm guessing you can also add a dynamic state that makes its own patterns

but the logic has to be defined somewhere... or maybe you'd like a chaotic unpredictable system.

Great reply thanks.

#### Guillekon

G

#### Guillekon

someone helps me to find this detonator line in the mapeditor that I can't find for boolean

sentrywiz said:

Healing_MUI[(Player number of (Owner of (Picked unit)))] Not equal to True


Click to expand...

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Boolean comparison. @Guillekon

#### GAcrewkent

#### GAcrewkent

 Level 3

JoinedSep 16, 2016Messages36

thanks for tutorial mate, but I have a better idea to let the AI heroes learn different skills when they gained a level.

1. Creating & storing variables data:

- AIhero[arrayed] (unit)

- SkillLearningChance[arrayed] (integer)

2. Here's the trigger

Events - A unit <gains a level>

Conditions - <Leveling hero> is <a hero> equal to true /OR/ <Leveling hero> equal to <AIhero[Player number of <Owner of <Leveling hero>>]>

Actions -

IF,THEN,ELSE multiple actions

->IF Hero level of <Leveling hero> not equal to 6

->THEN Set SkillLearningChance[Player number of <Owner of <Leveling hero>>] = Random number between 1 and 3.

->IF,THEN,ELSE multiple actions

->->IF SkillLearningChance[Player number of <Owner of <Leveling hero>>] equal to 1

->->THEN Learn <Skill1> for <Leveling Hero>

->->ELSE Do Nothing

->->IF SkillLearningChance[Player number of <Owner of <Leveling hero>>] equal to 2

->->THEN Learn <Skill2> for <Leveling Hero>

->->ELSE Do Nothing

->->IF SkillLearningChance[Player number of <Owner of <Leveling hero>>] equal to 3

->->THEN Learn <Skill3> for <Leveling Hero>

->->ELSE Do Nothing

->ELSE IF,THEN,ELSE multiple actions

->->IF Hero level of <Leveling hero> equal to 6

->->THEN Learn <Skill4> for <Leveling Hero>

->->ELSE Do Nothing

That's pretty much it , but where's the buying items to AI trigger? :'(


 Last edited: Nov 19, 2019

 You must log in or register to reply here.


### Similar threads

 How to create an artificial level-up system


frettory

Nov 22, 2012

Trigger (GUI) Editor Tutorials

Replies14Views9K

Mar 13, 2013
frettory

 State-based Passive Ability Tutorial


Blightsower

Sep 22, 2024

Trigger (GUI) Editor Tutorials

Replies8Views3K

Sep 23, 2024
Blightsower

 Simple Triggered Hero Artificial Intelligence in GUI


DenZel94

May 2, 2007

Trigger (GUI) Editor Tutorials

Replies22Views26K

Jul 1, 2021
aloH

 Triggered Item Drops


Zinox

Jan 8, 2011

Trigger (GUI) Editor Tutorials

Replies24Views31K

Feb 10, 2021
PaulieD

 Hero Selection Systems


Tonks

Mar 23, 2007

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jun 8, 2023
dantles1992

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