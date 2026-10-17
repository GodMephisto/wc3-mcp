---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/lumber-food-clock.147990/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Lumber Food Clock
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/lumber-food-clock.147990/>
#### Arowanna

#### Arowanna

 Level 26

JoinedDec 30, 2007Messages1,559

Resources: 18

#### Resources
Models 2Icons 5Skins 10Tutorials 1

Instructions

 This tutorial shows you how to create two different clocks.


The first one is based on Lumber and Food and the second one is based on Food.

 Requirements


 • Warcraft 3 The Frozen Throne - World Editor


• A small knowledge of how to "Trigger" in the Trigger Editor


Lumber/Food Clock Function

[TD]

[TD][/TD]


This Clock works like the clock in DotA.


Every second the player will gain 1 Food, representing 1 second and


each 60 second, they will be removed and replaced with 1 Lumber, representing 1 minute.

Disadvantages:


- Can’t use Lumber or Food in your map


- All units must have 0 Lumber- and Food cost.

 Difficulty


Easy

[td][/td]

Food Clock Function


Every second the player will gain 1 Max Capability, representing 1 second and each


60 second, they will be removed and replaced with 1 Used Food, representing 1 minute.

Advantages:


- You can use Lumber without a problem

Disadvantages:


- Can’t use Food in your map


- All units must have 0 Food cost.

 Difficulty


Easy

[TD][/TD]
[td][/td]

[/td]

Lumber/Food Clock

Now when we have an idea, let’s get started.


Create a new Category and a new Trigger, name it “Clock”.

Lumber/Food Clock Triggers

Step I


First we need an Event, which is repeated every second of the game.


So let’s create a “Time – Periodic Event” that is repeated every second.

Time - Every 1.00 seconds of game time

Press the button “New Event (CTRL+E)”.


Find the Trigger “Time – Time Elapsed”.


This text will be shown:


”Elapsed game time is 5:00 seconds”, change 5:00 to 1:00.


Trigger Explanation:


This Event, is triggered every second that passes by.

Step II


Now when we have an Event, we must choose who’s going to be affected by the Event.


And we want everyone to “gain” 1 second in their Food Section. So create a


”Player Group - Pick every player in Player Group and do Multiple Actions” action.

Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions

Press the button “New Action (CTRL+R)”.


Find the action “Player Group - Pick every player in Player Group and do Multiple Actions”


This text will be shown:


”Pick every player in (All Players) and do Multiple Actions”


Change nothing.


Trigger Explanation:


This action picks All Players and performs the following actions.

Step III


Now we have a loop that will be repeated each second of the game.


We need an If/Then/Else action to check if we got more or less than


59 Seconds in our stock, so create a If/Else/Then action.

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions

Then - Actions

Else – Actions

Press the button “Action (CTRL+R)”.


Find the action “If / Then / Else, Multiple Actions”.


This text will be shown:


”If (All Conditions are True) then do (Then Actions) else do (Else Actions)


Trigger Explanation:


This action speaks for it self.


If the Conditions are true, then the Then Actions will run, and


if the Conditions are false, it will run the Else Actions.

Step IV


Now we need a Condition that checks if we got more


or less than 59 seconds in stock. So create a Integer Comparison.

((Picked player) Food used) Equal to 59

 Press the button “New Condition (CTRL+D)”.


Find the Condition called: Interger Comparison.


This text will be shown:


(Number of units in (Units in (Playable map area))) Equal to 0


Change “Number of Units in (Units in (Playable Map Area)))” into “Player – Player Property”.


From there, change Player 1 (Red) into the Function “Picked Player”.


Change “Current Gold” into “Food Used”.


Let Equal to stay.


Change the value “0” into”59”.


Trigger Explanation:


This trigger checks if Picker Player (All Players) uses 59 Food or not.


If it’s true, then it will activate the “Then Actions” in the If / Then / Else actions,


but if it’s false, it will activate the “Else Actions”.

Step V


Now we have a Condition that checks if we have 59 Seconds,


but we need some Actions so something really happens.


We want the Trigger to either add 1 Seconds or to remove the current


seconds and replace it with 1 Minute. So let’s create some Then – Actions.

Side Note


Then = when the Condition is true

Then - Actions
Player - Set (Picked player) Food used to 0

Player - Add 1 to (Picked player) Current lumber

Now the Trigger will remove the current Food and add one


Lumber to the Lumber Stock, but only if the Condition is true.

First Trigger:


Press the button “New Action (CTRL+R)”.


Find the action ”Player – Set Property”


This text will be shown:


”Player - Set Player 1 (Red) Current gold to 750”


Change “Player 1 (Red)” into “Picked Player”


Change “Current Gold” into “Food used”


Change the value “750” into “0”


Now it should look like this:


Player - Set (Picked player) Food used to 0


Second Trigger:


Press the button “New Action (CTRL+R)”.


Find “Player – Add Property”


This text will be shown:


”Player - Add 1000 to Player 1 (Red) Current gold”


Change the value “1000” into “1”


Change Player 1 (Red) into “Picked Player”


Change “Current Gold” into “Current Lumber”


Now it should look like this:


Player - Add 1 to (Picked player) Current lumber


Trigger Explanations:


First:


This trigger simply removes all Food in stock for all players.


Second:


This trigger adds one Lumber to the Lumber stock.

Step Final


Right now, nothing will happen if the Condition is False, so we need


to create a Else - Action that adds one Food each time the Condition is False.

Else - Actions
Player - Add 1 to (Picked player) Food used

Press the button “New Action (CTRL+R)”.


Find “Player – Add Property”


This text will be shown:


”Player - Add 1000 to Player 1 (Red) Current gold”


Change the value “1000” into “1”


Change Player 1 (Red) into “Picked Player”


Change “Current Gold” into “Food Used”


Now it should look like this:


”Player - Add 1 to (Picked player) Food Used”


Now the Trigger will add one Food for each time the Condition is False,


and now the Trigger is complete and should look like this:

Clock
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked player) Food used) Equal to 59

Then - Actions
Player - Set (Picked player) Food used to 0

Player - Add 1 to (Picked player) Current lumber

Else - Actions
Player - Add 1 to (Picked player) Food used

 Optional: Game Interface – For Lumber/Food Clock


Right now there’s a problem with our clock.


If you scroll over Food or Lumber, it will still say:


 “Food – The amount of food you are using over the total amount you can currently sustain.”


And that’s not really any “eye candy” now is it?


So why not change the name of Lumber and Food in to something that fits?

Step I


Go to the Advanced and go to “Game Interface”.


From here you go to “Text - General – Food” and change it to Seconds.


Do the same with “Text - General – Lumber” and “Text General – Lumber:”


but change them into Minutes and not seconds!

Step II


Now the “name” of Food and Lumber is renamed but it still shows the old


information about Food and Lumber, so go to:

“Text – General – Lumber is harvested from trees” and change it to something you like.


I used: “The amount of minutes played.” After that, go to:

“Text – General - The amount of food you are using over the total amount you can currently sustain.”


and change it in to something you like as well.


I used:

“The amount of seconds played.”


Now we have new information about Minutes and Seconds


and the map looks much more serious than it did before.

Step III


But there’s still a few very annoying things left.


Each time the clock reaches 51 seconds it will say:

LOW UPKEEP


And that’s not really something we want, so let’s remove it.


Go to Advanced and go to “Game Constants”.


Find “Upkeep – Food Levels” and change Low and Medium to 100.


Now we have removed the annoying Upkeep message and our clock is


nearly complete, but there’s still on thing that should be removed.

Step Final


We can still see the old icons for Wood and Food, and they are now


minutes and seconds, so it still looks a bit weird. Let’s change that.


Go to Advanced and go to “Game Interface”.


Find “Icon – Food” and change it into:

UI\Widgets\Console\Human\human-console-button-back-active.blp


This will almost hide the Food button, but you will see a “blue” filter


if you concentrate. To get a black space, download/make a plain black filter instead


Find and Change “Icon – Lumber”, change it in to something you think fit.


I used:

ReplaceableTextures\Selection\SpellAreaOfEffect.blp


because it almost looks like a “Blue Clock”.


Now we have removed the ugly Lumber and Food icons and


replaced them with two that fits better with our Clock System


and should now look something like this:


For more info about the Game Interface and Constants, go here:

https://www.hiveworkshop.com/forums/general-mapping-tutorials-278/game-interface-basics-39401/

[TD][/TD]
[td][/td]

Food Clock

To do this Clock, do the Lumber/Food Clock first.


It’s almost the exact same clock, so you won’t waste any time doing it,


but do NOT fix the Game Interface just yet!


When you’re done with the Lumber/Food Clock,


you just need to change three triggers.

Clock
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked player) Food used) Equal to 59

Then - Actions
Player - Set (Picked player) Food used to 0

Player - Add 1 to (Picked player) Current lumber

Else - Actions
Player - Add 1 to (Picked player) Food used

Step I


Begin with the Then Actions in the If / Then / Else action.


The first one currently removes the current Food and adds 1 Lumber.


We need to change that into Used and Capability Food.


So, change the


“Player - Set (Picked player) Food used to 0”


into


”Player - Set (Picked player) Food cap to 0” .


and then change the


” Player - Add 1 to (Picked player) Current lumber”


into


”Player - Add 1 to (Picked player) Food used”


Now the Then system is done and should look like this:

Then - Actions
Player - Set (Picked player) Food cap to 0

Player - Add 1 to (Picked player) Food used

Step Final


Now we just need to change the Else part, so change


”Player - Add 1 to (Picked player) Food used”


into


“Player - Add 1 to (Picked player) Food cap”

Else - Actions
Player - Add 1 to (Picked player) Food cap

Now the Clock is complete, but you can move on to the Eye Candy part


to make it look better, and it should look like this:

Clock
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked player) Food cap) Equal to 59

Then - Actions
Player - Set (Picked player) Food cap to 0

Player - Add 1 to (Picked player) Food used

Else - Actions
Player - Add 1 to (Picked player) Food cap

Optional: Game Interface – For Food Clock


Right now there’s a problem with our clock.


If you scroll over Food Section, it will still say:


 “Food – The amount of food you are using over the total amount you can currently sustain.”


And that’s not really any “eye candy” now is it?


So why not change the name of Food in to something that fits?

Step I


Go to the Advanced and go to “Game Interface”.


From here you go to “Text - General – Food” and change it to Time.

Step II


Now the “name” of Food is renamed but it still shows the old


information about Food, so go to:

“Text – General - The amount of food you are using over the total amount you can currently sustain.”


and change it in to something you like as well.


I used:

“ The time passed; Minutes and Seconds.”


Now we have new information about the time


and the map looks much more serious than it did before.

Step III


But there’s still a few very annoying things left.


Each time the clock reaches 51 “minutes” it will say:

LOW UPKEEP


And that’s not really something we want, so let’s remove it.


Go to Advanced and go to “Game Constants”.


Find “Upkeep – Food Levels” and change Low and Medium to 100.


Now we have removed the annoying Upkeep message and our clock is


nearly complete, but there’s still on thing that should be removed.

Step Final


We can still see the old icon for Food, and it’s suppose to look like minutes and seconds,


so it still looks a bit weird. Let’s change that.


Go to Advanced and go to “Game Interface”.


Find “Icon – Food” and change it into:

 ReplaceableTextures\Selection\SpellAreaOfEffect.blp


This icon almost looks like a “Blue Clock” and fits well, but


you can download a icon if you think that fits better!


Now we have removed the ugly Food icon and


replaced it with one that fits better with our Clock System


and should now look something like this:


For more info about the Game Interface and Constants, go here:

https://www.hiveworkshop.com/forums/general-mapping-tutorials-278/game-interface-basics-39401/

[TD][/TD]
[td][/td]

Updates and Extras

Updates

[TD]

[TD][/TD]

3 November – 21:00


Made some major changes in the Tutorial.


In general, the Tutorial should be easiler to understand.

 - New Layout


- Added a new Clock


- A new "helper"; How-To boxes


- Minor fixes; such as gramma.

[td][/td]

Test Maps

Food/Lumber Clock - View attachment Food-Lumber Clock System.w3x

Food Clock - View attachment Food Clock System.w3x

[TD][/TD]
[td][/td]

Editor Notes

I've done this from scratch and everything


is done by me, but I had some help from Dr.Boom and


I used DOTAs' clock as a reference to make it (Lumber/Food).


This tutorial was written in a way so everyone should


be able understand it, that's why it's so long.


Regards


Arowanna

[TD][/TD]
[td][/td]

[/td]


 Last edited: Nov 4, 2009

#### .Ragnaros

#### .Ragnaros

 Level 6

JoinedAug 13, 2009Messages197

Resources: 5

#### Resources
Icons 3Tools 1Tutorials 1

Pretty impressive tutorial... Very good quality and with a cute organization. I would rate it 10/10. Its rare to find quality tutorials like yours  +Rep

#### Arowanna

#### Arowanna

 Level 26

JoinedDec 30, 2007Messages1,559

Resources: 18

#### Resources
Models 2Icons 5Skins 10Tutorials 1

.Ragnaros said:

 Pretty impressive tutorial... Very good quality and with a cute organization. I would rate it 10/10. Its rare to find quality tutorials like yours  +Rep


Click to expand...

Thanks, and I'm glad that you like it.


I just hope that it will get approved.

#### .Ragnaros

#### .Ragnaros

 Level 6

JoinedAug 13, 2009Messages197

Resources: 5

#### Resources
Icons 3Tools 1Tutorials 1

It'll took a while, or not... Everything depends on the moderators...

#### Arowanna

#### Arowanna

 Level 26

JoinedDec 30, 2007Messages1,559

Resources: 18

#### Resources
Models 2Icons 5Skins 10Tutorials 1

Would be glad if someone could review it >.<

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

Instead of showing the actions you are doing I think you should show the whole ITE function all the time, step by step so it will be easier to see what the fuck you are doing without reading badly

Also mentioning about that units or what you have in your map shouldn't use food nor lumber in-game as it will ruin this.

Also what will happen if you do the "greedisgood" cheat. Then you will have limitless "minutes", you get my point, save the minut value into an integer instead to not interfeear this "cheat bug" ^^

else it is great

#### Gamecrafter_DK

#### Gamecrafter_DK

 Level 14

JoinedSep 25, 2007Messages1,257

Resources: 14

#### Resources
Icons 5Maps 6Spells and Systems 1Tutorials 2

Very simple, and baassee did some good points there. What about cranking it up a noch? Go away from the premade DotA system, and fidget with some other stuff. What about making the entire system run in food, so you don't have to use lumber also?

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

Or make it in the multiboard as I do  HAHA but as many want the "Dota" system I think it is worth it to post this here.

#### Arowanna

#### Arowanna

 Level 26

JoinedDec 30, 2007Messages1,559

Resources: 18

#### Resources
Models 2Icons 5Skins 10Tutorials 1

baassee said:

Instead of showing the actions you are doing I think you should show the whole ITE function all the time, step by step so it will be easier to see what the fuck you are doing without reading badly


Also mentioning about that units or what you have in your map shouldn't use food nor lumber in-game as it will ruin this.


Also what will happen if you do the "greedisgood" cheat. Then you will have limitless "minutes", you get my point, save the minut value into an integer instead to not interfeear this "cheat bug" ^^


else it is great


Click to expand...

Finally!


And thanks baassee!


I will add the Food Cost and Lumber as side notes in the second update.


And I might change the whole tutorial, just need some time to do it.


Right now I can't do anything, school is killing me agian >.<


The BOLD marked:


I explain what each Trigger does (not perfect, but I said: Some basic GUI skill is req.)


Do you want me to take a Printscreen for each little thing I do?


(I will do it, if you think that it will improve the Tutorial)


Edit:


Which part is "confusing"?


The Triggers or the "Eye-Candy"?

Gamecrafter_DK said:

 Very simple, and baassee did some good points there. What about cranking it up a noch? Go away from the premade DotA system, and fidget with some other stuff. What about making the entire system run in food, so you don't have to use lumber also?


Click to expand...


Just give me a minute

Clock
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked player) Food cap) Equal to 59

Then - Actions
Player - Set (Picked player) Food cap to 0

Player - Add 1 to (Picked player) Food used

Else - Actions
Player - Add 1 to (Picked player) Food cap

Done.


Just as simple, but you can use Lumber with it


I guess it can come in handy for someone


Updating the Tutorial soon, just gotta' do some homework...


 Last edited: Nov 3, 2009

#### Gamecrafter_DK

#### Gamecrafter_DK

 Level 14

JoinedSep 25, 2007Messages1,257

Resources: 14

#### Resources
Icons 5Maps 6Spells and Systems 1Tutorials 2

Yeah, uhh, i don't really needed the system. I can do way better, it was just something for you to possibly put in your tutorial ^^

#### Arowanna

#### Arowanna

 Level 26

JoinedDec 30, 2007Messages1,559

Resources: 18

#### Resources
Models 2Icons 5Skins 10Tutorials 1

Didn't mean it like that


I just made it, going to add it soon, just need to fix some stuff.


Edit:


Currently updating the Tutorial, it will be up soon!


Updated:


I will update it again soon, fix screenshots and such, but schools calling :S


 Last edited: Nov 3, 2009

#### Gamecrafter_DK

#### Gamecrafter_DK

 Level 14

JoinedSep 25, 2007Messages1,257

Resources: 14

#### Resources
Icons 5Maps 6Spells and Systems 1Tutorials 2

Sounds great. ^^

#### Darkness-4ever

#### Darkness-4ever

 Level 9

JoinedNov 4, 2007Messages932

Just bringing up more 3 month old tut. submissions that have been neither graveyarded nor approved, and wondering if this should even be bothered with anymore (the tut. submission sub-forum, not my pestering whoever mods it.)

#### Arowanna

#### Arowanna

 Level 26

JoinedDec 30, 2007Messages1,559

Resources: 18

#### Resources
Models 2Icons 5Skins 10Tutorials 1

Hmm, yes, was a while ago.


Maybe I should update it?

#### Darkness-4ever

#### Darkness-4ever

 Level 9

JoinedNov 4, 2007Messages932

You should probably wait, the person that mods tutorials appears to have fallen into a black hole.

#### Arowanna

#### Arowanna

 Level 26

JoinedDec 30, 2007Messages1,559

Resources: 18

#### Resources
Models 2Icons 5Skins 10Tutorials 1

Did the tutorial moderator die?

#### Darkness-4ever

#### Darkness-4ever

 Level 9

JoinedNov 4, 2007Messages932

Yes...


I mean no...


I mean yes!

#### dOT145

#### dOT145

 Level 3

JoinedJun 22, 2009Messages18

Resources: 1

#### Resources
Tutorials 1

This is a really good tutorial, comprehensive and helpful to both beginners and middle-level trigger-ers (experts just...do their own thing). I like how it shows how to get to each action/condition/etc., even though I don't need it. It looks like you spent a lot of time and effort making this.

#### Arowanna

#### Arowanna

 Level 26

JoinedDec 30, 2007Messages1,559

Resources: 18

#### Resources
Models 2Icons 5Skins 10Tutorials 1

Five months or something.


Not that I care anymore, but someone should clean the Tutorial Section.


And by the way, thanks dOT145.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Today is your lucky day. Anyway, amazing tutorial. It is very well formatted and neat as possible. I don't see any visible problems with it (except for the colors) but they are fine since they are similar to the theme anyway [and are at least appropriate].


Definitely, ~Approved. Nice job.

#### sadvisor

#### sadvisor

 Level 9

JoinedApr 28, 2009Messages538

Nice tutorial, but I don't think it has any use.

If I've ever played any map with minute / second counter as lumber / food, I didn't even noticed.

I, for one, find this whole thing confusing and strange. The multiboard still remains the proper palce to place a timer in. It's practical, and better for a map from any point of view.

But again...this is just my opinion.

#### Gekigengar

#### Gekigengar

 Level 16

JoinedAug 20, 2009Messages1,550

Resources: 3

#### Resources
Maps 1Spells 2

Using multiboard will block some player's click

and even that show timer windows made me annoyed...

using time in that windows make me want to kill the creator

well, it really disable a portion of your clicks, i find that annoying,

so, this tutorial is awesome !!

it can really show your skill

like in dota, in 10 minutes, you get 112 creep kills, isn't that a record for you?

i personally like getting some top record and beat my own record to increase my gaming skills

so, don't say it isn't a usefull thing,

maybe we could learn a bit of triggering from it, even though its pretty simple

but for the food only clock

i find that at some points where the minutes is greater than the seconds

the number turns red...

and instead of setting low upkeep and stuff to 100

set it to 0

it disables any upkeep level

even low upkeep

so you could add your map's version number and stuff like in dota.

or any description, instead of showing "no upkeep"

*maybe if you could remove the red icon,when the minutes is greater than seconds, i would give you a +rep  and i will use it on my map. with your credits

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Why don't you use

UI\Glues\SinglePlayer\HumanCampaign3D\Black32.blp

instead of

UI\Widgets\Console\Human\human-console-button-back-active.blp

?

It's black, not dark blue

#### pOke

#### pOke

 Level 13

JoinedMar 24, 2013Messages1,105

Resources: 1

#### Resources
Maps 1

If anyone is ever looking for the built in clock icon:

UI\Widgets\BattleNet\bnet-tournament-clock.blp

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 Lumber/Food based Clock


Arowanna

Oct 18, 2009

Triggers & Scripts

Replies4Views2K

Oct 18, 2009
Arowanna

 Arrow Key Movement / Camera System


OneEight7even

Nov 15, 2007

Trigger (GUI) Editor Tutorials

Replies32Views28K

Apr 10, 2014
Andser

Locked

 Confused about leaks... insanely confused... -.-


Karzama

Apr 26, 2012

World Editor Help Zone

Replies6Views2K

Apr 29, 2012
Spartipilo

D

Locked

[Trigger]
 Add Passive Ability to Random units of Player...


Deleted member 231919

May 28, 2017

Triggers & Scripts

Replies2Views997

Jun 2, 2017
Dr Super Good

Locked

 Still i am amateur and i know it. How to make this trigger less painful?


JFAMAP

Jan 26, 2023

World Editor Help Zone

Replies2Views628

Jan 27, 2023
JFAMAP

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