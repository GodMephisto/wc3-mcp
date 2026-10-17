---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/hero-selection-systems.31799/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Hero Selection Systems
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/hero-selection-systems.31799/>
#### Tonks

#### Tonks

 Level 8

JoinedFeb 4, 2007Messages389

Resources: 4

#### Resources
Tutorials 4

Various Hero Selection Systems and How They Work


In this tutorial I'm going explain many of the methods for hero selection.

Index-​
1 · Next / Previous Hero Viewing

2 · Dialog Menu Selection

3 · Using a Unit and Circles of Power

4 · Hero Taverns

5 · Clicking and Doubleclicking

6 · Random Hero

 1 - Next / Previous Hero Viewing


Difficulty: Moderate


Note: This is not a good idea if you have more than 8-12 heroes.


This is not a very common method of hero selection. In fact, I've only ever seen it used in the Blizzard map Azeroth Grand Prix. I used it myself in one of my maps.


Essentially what this system entails is having a unit that has several abilities; Next, Previous, and an ability with the icon of the Hero. When you click Next, it locks your camera to the next hero in line, and when you click Previous it locks your camera to the previous hero. Then when you click the ability with the Hero's icon, it selects that hero.

Variables:


- HeroSelector - Unit Array Variable - Size 12


- HeroSelectAbil - Ability Array Variable - Size 12 (Initial Value set to your first hero)


- L - Point Variable

Abilities:


- You will need an ability for every hero. The ability should be named after the hero and should have the same icon as the hero.


- Next - Should be based off an ability easily modified to do nothing.


- Previous - Should be based off an ability easily modified to do nothing, but should not be based off the same ability as "Next."

Units:


- You will need one of each of your heroes (preferably in a line) owned by Neutral Passive.


- You will need one Hero Selector unit for each player. Should have the Abilities Next, Previous, and the ability corresponding to the first hero.

Triggers:

View Previous
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Previous

Actions

Actions
Unit - Remove HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] from (Triggering unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] Equal to Dwarves

Then - Actions
Set HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] = Panda

Camera - Lock camera target for (Owner of (Triggering unit)) to Panda 0040 <gen>, offset by (0.00, 0.00) using Default rotation

Unit - Add HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] to (Triggering unit)

Skip remaining actions

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] Equal to Clubz

Then - Actions
Set HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] = Dwarves

Camera - Lock camera target for (Owner of (Triggering unit)) to Dwarves 0037 <gen>, offset by (0.00, 0.00) using Default rotation

Unit - Add HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] to (Triggering unit)

Skip remaining actions

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] Equal to Muskles

Then - Actions
Set HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] = Clubz

Camera - Lock camera target for (Owner of (Triggering unit)) to Ice T 0036 <gen>, offset by (0.00, 0.00) using Default rotation

Unit - Add HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] to (Triggering unit)

Skip remaining actions

Else - Actions

Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Previous [/CODE]

Runs this trigger when an ability is cast, but only if it is the "Previous" Ability.

[TRIGGER]Unit - Remove HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] from (Triggering unit)
Removes the ability that is assigned to HeroSelectAbil for that player.

If - Conditions
HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] Equal to Dwarves

Then - Actions
Set HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] = Panda

Camera - Lock camera target for (Owner of (Triggering unit)) to Panda 0040 <gen>, offset by (0.00, 0.00) using Default rotation

Unit - Add HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] to (Triggering unit)

This checks if the HeroSelectAbil variable is set to the Dwarves ability. If it is, it sets it to the Panda ability, Locks the camera to the Panda unit, and adds the Panda ability to the unit.

Skip remaining actions
This is in there to make sure that it doesn't continue the function.


The rest of the trigger is repetitive, and does not need to be explained.

View Next
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Next

Actions
Unit - Remove HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] from (Triggering unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] Equal to Panda

Then - Actions
Set HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] = Dwarves

Camera - Lock camera target for (Owner of (Triggering unit)) to Dwarves 0037 <gen>, offset by (0.00, 0.00) using Default rotation

Unit - Add HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] to (Triggering unit)

Skip remaining actions

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] Equal to Dwarves

Then - Actions
Set HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] = Clubz

Camera - Lock camera target for (Owner of (Triggering unit)) to Ice T 0036 <gen>, offset by (0.00, 0.00) using Default rotation

Unit - Add HeroSelectAbil[(Player number of (Owner of (Triggering unit)))] to (Triggering unit)

Skip remaining actions

Else - Actions

As you can tell, the Next trigger is just the same as the Previous trigger, in reverse.

Pick Cart
Events
Unit - A unit Starts the effect of an ability

Conditions
(Unit-type of (Triggering unit)) Equal to Hero Selection

Or - Any (Conditions) are true
Conditions
(Ability being cast) Equal to Panda

(Ability being cast) Equal to Bonez

(Ability being cast) Equal to Clubz

(Ability being cast) Equal to Dwarves

(Ability being cast) Equal to Fatty

(Ability being cast) Equal to Muskles

(Ability being cast) Equal to Priest

(Ability being cast) Equal to Revenant

(Ability being cast) Equal to Spirit

(Ability being cast) Equal to Tinker

(Ability being cast) Equal to Troll

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Bonez

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Bonez for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Clubz

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Ice T for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Dwarves

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Dwarves for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Fatty

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Fatty for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Muskles

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Muskles for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Panda

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Panda for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Priest

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Priest for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Revenant

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Revenant for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Spirit

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Toad for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Tinker

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Tinker for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Troll

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Troll for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Else - Actions

Camera - Apply Camera 002 <gen> for (Owner of (Triggering unit)) over 0.00 seconds

Unit - Remove (Triggering unit) from the game

This trigger creates the unit corresponding to the clicked ability.

Conditions
(Unit-type of (Triggering unit)) Equal to Hero Selection

Or - Any (Conditions) are true
Conditions
(Ability being cast) Equal to Panda

(Ability being cast) Equal to Bonez

(Ability being cast) Equal to Clubz

(Ability being cast) Equal to Dwarves

(Ability being cast) Equal to Fatty

(Ability being cast) Equal to Muskles

(Ability being cast) Equal to Priest

(Ability being cast) Equal to Revenant

(Ability being cast) Equal to Spirit

(Ability being cast) Equal to Tinker

(Ability being cast) Equal to Troll

Checks if the triggering unit is the Hero Selection unit and if the cast Ability is any of the abilities corresponding to the heroes.

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Bonez

Then - Actions
Set L = (Random point in Team1Spawn <gen>)

Unit - Create 1 Bonez for (Owner of (Triggering unit)) at L facing Default building facing degrees

Custom script: call RemoveLocation(udg_L)

Checks if the ability being cast is the ability corresponding to the Bonez hero. If it is, it creates 1 Bonez at a random point in the Team1Spawn region.

Note: "Set L = ..." and "call RemoveLocation(udg_L)" are used to avoid a memory leak in this trigger.


Last but not least you'll want to have a trigger that sets the Hero Selectors.

HeroSelectors
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Set HeroSelector[1] = Hero Selection 0012 <gen>

Set HeroSelector[2] = Hero Selection 0013 <gen>

Set HeroSelector[3] = Hero Selection 0014 <gen>

Set HeroSelector[4] = Hero Selection 0015 <gen>

Set HeroSelector[5] = Hero Selection 0011 <gen>

Set HeroSelector[6] = Hero Selection 0017 <gen>

Set HeroSelector[7] = Hero Selection 0016 <gen>

Set HeroSelector[8] = Hero Selection 0020 <gen>

Set HeroSelector[9] = Hero Selection 0019 <gen>

Set HeroSelector[10] = Hero Selection 0018 <gen>

Set HeroSelector[11] = Hero Selection 0021 <gen>

Set HeroSelector[12] = Hero Selection 0022 <gen>

This sets the HeroSelector variables according to their arrays. "HeroSelector[1]" is set to Red's Hero Selector unit, "HeroSelector[2]" is set to Blue's, etc..

 2 - Dialog Button Selection


Difficulty: Easy


This is a fairly easy to create and very easy to comprehend method. It only requires two triggers, and only a few variables.

Variables:


- Dialog_Var - Dialog Variable


- Hero1_Var - Dialog Button Variable


- Hero2_Var - Dialog Button Variable


- You must create a Dialog Button Variable for each of the Dialog Buttons.

Triggers:

Dialog
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Dialog - Change the title of Dialog_Var to Pick your Hero

Dialog - Create a dialog button for Dialog_Var labelled Hero 1

Set Hero1_Var = (Last created dialog Button)

Dialog - Create a dialog button for Dialog_Var labelled Hero 2

Set Hero2_Var = (Last created dialog Button)

For each (Integer A) from 1 to 12, do (Dialog - Show Dialog_Var for (Player((Integer A))))

Dialog - Change the title of Dialog_Var to Pick your Hero
Changes the title of the Dialog_Var variable to "Pick your Hero." This is what will be displayed at the top of the menu.

Dialog - Create a dialog button for Dialog_Var labelled Hero 1

Set Hero1_Var = (Last created dialog Button)
Puts a dialog button on the menu labeled "Hero 1" This is what will be displayed on the button itself. Then it sets the Hero1_Var variable to the button that was just created. You need to set the variable so that you can reference it later.


Now you need this trigger to register when a button is clicked, and what button it was:

ClickedButton
Events
Dialog - A dialog button is clicked for Dialog_Var

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to Hero1_Var

Then - Actions
Unit - Create 1 Footman for (Triggering player) at (Center of (Playable map area)) facing Default building facing degrees

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to Hero2_Var

Then - Actions
Unit - Create 1 Footman for (Triggering player) at (Center of (Playable map area)) facing Default building facing degrees

Else - Actions

If - Conditions
(Clicked dialog button) Equal to Hero1_Var

Then - Actions
Unit - Create 1 Mountain King for (Triggering player) at (Center of (Playable map area)) facing Default building facing degrees

Creates a Mountain King if the clicked button was the Hero1_Var button.

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

roxx0rz

At first glance this tutorial seems helpful, powerful and useful.


As its further development continues, I'm sure that this will become an even more informative document.

#### Tonks

#### Tonks

 Level 8

JoinedFeb 4, 2007Messages389

Resources: 4

#### Resources
Tutorials 4

Wolverabid said:

 At first glance this tutorial seems helpful, powerful and useful.


As its further development continues, I'm sure that this will become an even more informative document.


Click to expand...


Thank you.


I'm also welcoming any suggestions for forms of Hero / Unit Selection that I didn't list in the Index.

#### Kixer

#### Kixer

 Level 9

JoinedJul 27, 2006Messages652

Nice

Looks really good... You could add arrow key scrolling for the "Next / Previous Hero Viewing" system for those who may want it...


A suggestion for a system is a selection preview system like Hero's Hero Selection System . Im not saying copy it but learn from it...

#### paskovich

#### paskovich

 Level 11

JoinedJul 12, 2005Messages764

Resources: 3

#### Resources
Maps 1Tutorials 2

Good one!


But, i saw separate toturials for these (exept the first and the last one) systems. Anyway it's a good idea to collect them! And, you wrote it really well, so good job!


Btw, i have already made the last (click-on-the-hero-to-see-details and click-again-to-select  ) type. However, it's in jass , and i don't think it will fit this tut.

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Looks good so far, but you can easily make some optimizations to the code in some spots. The only major one I see is the trigger to pick the heroes using the first method. You go through each type of unit, and then create the location, the unit, and then remove the location. This makes the code much larger than it has to be. Simply create the location at the beginning of the trigger and destroy it at the end. All you'll have to do after each if/then action is just create the hero. This will also improve the readability of the code.

Also, something tells me that the first method could be improved more if you only used 1 ability to pick a hero. First off, you'd keep track of the current hero being looked at by a player. Then when the "pick hero" ability is cast, you would create a unit with type of the currently being looked-at hero. This would eliminate much more coding and will make it easier to add more heroes.

Now for your method of moving between heroes. You're doing that if/then thing again which makes the code look very ugly. I would suggest using an array of either rects or just the available units to be selected as I mentioned above. The player would start off looking at rect[0] or unit[0], when the next button was hit, the player would then be looking at rect/unit[current+1]. You would of course have to add more code to the initializing trigger, but the rest of the code wouldn't need to be updated after that. Additionally, a variable would be needed to keep track of the maximum number of heroes as well.

Now for your next method of hero selecting. Once more, you're doing the ugly if/then thing. Variable arrays would come in handy once again. You would create each dialog and set it to an index in the array, and then use another array for the heroes. You would also, again, have another variable that would keep track of the maximum amount of heroes. When the dialog is clicked, you'll start a loop that will loop through all the indexes of the dialog, and if that's the correct dialog, it will create a unit of type heroes[index].

You still have a long way to go on this system, good luck.

#### Tonks

#### Tonks

 Level 8

JoinedFeb 4, 2007Messages389

Resources: 4

#### Resources
Tutorials 4

 A suggestion for a system is a selection preview system like Hero's Hero Selection System . Im not saying copy it but learn from it..


Click to expand...


Thanks for the suggestion, I'll try to add that in.

wyrmlord said:

 Looks good so far, but you can easily make some optimizations to the code in some spots. The only major one I see is the trigger to pick the heroes using the first method. You go through each type of unit, and then create the location, the unit, and then remove the location. This makes the code much larger than it has to be. Simply create the location at the beginning of the trigger and destroy it at the end. All you'll have to do after each if/then action is just create the hero. This will also improve the readability of the code.


Also, something tells me that the first method could be improved more if you only used 1 ability to pick a hero. First off, you'd keep track of the current hero being looked at by a player. Then when the "pick hero" ability is cast, you would create a unit with type of the currently being looked-at hero. This would eliminate much more coding and will make it easier to add more heroes.


Now for your method of moving between heroes. You're doing that if/then thing again which makes the code look very ugly. I would suggest using an array of either rects or just the available units to be selected as I mentioned above. The player would start off looking at rect[0] or unit[0], when the next button was hit, the player would then be looking at rect/unit[current+1]. You would of course have to add more code to the initializing trigger, but the rest of the code wouldn't need to be updated after that. Additionally, a variable would be needed to keep track of the maximum number of heroes as well.


Now for your next method of hero selecting. Once more, you're doing the ugly if/then thing. Variable arrays would come in handy once again. You would create each dialog and set it to an index in the array, and then use another array for the heroes. You would also, again, have another variable that would keep track of the maximum amount of heroes. When the dialog is clicked, you'll start a loop that will loop through all the indexes of the dialog, and if that's the correct dialog, it will create a unit of type heroes[index].


You still have a long way to go on this system, good luck.


Click to expand...


I'm aware of all these methods, and I have attempted each and every one of them. They do not work. If you would like, I could go through, list all the problems for each one, but I'd rather not.


The Next / Previous Selection was very difficult because all the methods you suggested, I thought of, and didn't work. This was the fifth remake of this system, and the first one to work.


I'm not going to worry about small things like where I placed the location setting, or whether I used 'too many' If / Then / Else functions. In my opinion, they're possibly the most useful Gui functions available, along with Integer A. In all honesty, why would I sacrifice workability for readability?

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved

~ Thread moved to Trigger (GUI) Editor Tutorials.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Incomplete tutorial .... if you are going to finish please add some images, you should also add a map with that stuff ... The only thing you are doing is copy/paste all codes you know ..... You only explain few of them and you don't even tell people how to reach them .... You have lot's of work to do.

1.5/5 in my opinion... However when you finish this huge tutorial (if you finish it) you will have a better rank (3.5/5 maximun because you have no images and no map.)

You should also create a bibliography section because this is not only your work .... It is the work of other people as well. Blizzard included.

It is poor for now, but you can make it better, keep doing the good work, this tutorial has potential.

I also strongly advice you to see my "How to Create a Tavern by Flame_Phoenix" Tutorial at general mapping section, it will help you, trust me.

#### Matrix

#### Matrix

 Level 5

JoinedJan 18, 2007Messages59

Resources: 6

#### Resources
Maps 4Spells 1Tutorials 1

why u don't send attach example?

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Flame_Phoenix & Matrix:


Guys if you are trying to talk to Tonks, you are talking to a brick wall: Gone.


If our Spell moderators feel that this tutorial is incomplete or unhelpful then it will be moved to the Graveyard, otherwise it stands as-is for the information of users who are interested in this subject.

#### paskovich

#### paskovich

 Level 11

JoinedJul 12, 2005Messages764

Resources: 3

#### Resources
Maps 1Tutorials 2

At my first comment, I wrote this is good, but I didn't think this will be all of it... The guys are right, it still needs a testmap, and it should be finished (cover all points), but anyway, if only one person finds this useful in the future, then Tonks did not work on this redundantly.

IMO, this shouldn't be graveyarded, as you said Wolve, it should stand as-is.

#### dantles1992

#### dantles1992

 Level 6

JoinedAug 15, 2022Messages116

I am sorry if it is wrong to post but is there tutorial on how to make race selection stuff and how t rename races if I later want to change the name of the race? pleas help

 You must log in or register to reply here.


### Similar threads

 Hero Selection Systems


Eleandor

Aug 20, 2007

Trigger (GUI) Editor Tutorials
2

Replies51Views81K

Feb 28, 2021
Caalanuaa

 Altering Hero Stats For Noobs


Faith

May 1, 2013

Trigger (GUI) Editor Tutorials

Replies24Views11K

Oct 2, 2015
XxkosumaxX

 Simple Triggered Hero Artificial Intelligence in GUI


DenZel94

May 2, 2007

Trigger (GUI) Editor Tutorials

Replies22Views26K

Jul 1, 2021
aloH

 Making an Open Rpg


Ralle

Jan 12, 2006

Trigger (GUI) Editor Tutorials

Replies0Views47K

Jan 12, 2006
Ralle

 State-based Passive Ability Tutorial


Blightsower

Sep 22, 2024

Trigger (GUI) Editor Tutorials

Replies8Views3K

Sep 23, 2024
Blightsower

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