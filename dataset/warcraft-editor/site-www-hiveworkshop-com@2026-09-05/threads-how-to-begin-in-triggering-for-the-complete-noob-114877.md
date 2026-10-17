---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-begin-in-triggering-for-the-complete-noob.114877/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Begin in Triggering for the Complete Noob
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-begin-in-triggering-for-the-complete-noob.114877/>
#### hawk_767

#### hawk_767

 Level 9

JoinedAug 23, 2007Messages465

Resources: 1

#### Resources
Tutorials 1

Getting started triggering in the WE


First of all, this tutorial is for people who know their way around the basic trigger editor, meaning how to create a trigger but not to set up exact events and triggers for in game.


Now let us discover what exactly triggers are and what they do.


Triggers are options you can put into place in the WE so that different events will occur in game. They can be simple to very complex, and they can create many different effects from spells and abilities to cinematic and unit, in game actions.


When you are just beginning modding with the WE the trigger editor may seem a little overwhelming at first, but I will describe each piece and give some examples of simple triggers that will help you create simple trigger maps.


Events: This is where you should start when making your trigger, however when you first start you may be drawn to look at the actions section since it is a bit more explained and easy to understand. This is however where you will make the trigger work. You will place the basic action to occur here.


Conditions: These may seem quite confusing to you beginners but they aren't as bad as you may think. Here is where you will set the conditions that must be taken for your trigger to work. In lay mans terms, here you will say what must happen for your actions to take place.


Actions: This is where you will select the exact actions to take place for your triggers. You can make units move from place to place, make cinematic videos begin and end, and many many more events. The actions are what you will most likely want to look at most since these events are shown very bluntly and easy to understand. These along with all the others will be explained further in the examples.


This is what the entire trigger list will look like.


First example: Moving units


 This is a simple trigger that will cause one unit to move from one point to another when your unit or units enter a specified region.


First start by creating a simple region. I've marked mine with patchy grass.


For the means of this tutorial you will want to place 1 unit that is owned by you and another than is owned by another color. I used peasants for my example.


Now go up to the trigger editor button and click on it. You may notice the Map initialization trigger there but you won't need it so if it gets in your way for these examples you may delete it. Once you've done this you'll want to click the new trigger button, following this you will click new event. You can now read the events and what you could possibly do. Once your ready go down to the event labeled,

Unit – Unit Enters Region,


This is in the Unit event group.


Once you've got this selected you need to click the word Region highlighted in red. Then you should select the region your unit or units will enter.


It will then look like this. Where it says Region highlighted in blue however it will be whatever you named your region instead.


This is what the trigger should read,


Now on to the condition that you will want to set. You probably will at first be confused by some of the things that are layed out in the Conditions selection.


First you'll want to create a new condition, this button is right next to the Create new event and action buttons.


You'll see


Triggering unit is a structure Equal to True


You'll want to stick with Boolean Comparison for this one as well.


Now you'll want to select the Triggering Unit is a Structure, which is highlighted in blue. You should change the function to


Unit – Unit Selected by player.


Now when you select the unit that will be used to enter the region, which will be my peasant. Mine now looks like this,


Again this is what it should look like now.


Now on to the actions. The action I will use is going to be a very simple one, when your unit, the peasant, enters the region another unit, the enemies or allies, will move to the center of that same region.


Since you now know how to get around the trigger editor I will be a little less descriptive now. This way you will be able to use what you've learned so far to find what you need.


Now in the Actions section you will want to find the action


Unit - Issue order targeting a point. Now try getting this one on your own. Get your trigger to look like this, except where it says Order Peasant 002 it will have the name of the unit you selected.


In the image you can see what group the action is in and what to change the specifics to.


If you have done this correctly you should get this,


TADA! Your done, you've just created your first trigger. Now once you have done this you should gradually make this trigger more complicated. Try adding in unit spawns when you enter the region, or gates you place could be opened. You could do many things from this base trigger.


Remember, triggering may be complicated and make you want to stop because of the difficulty, but start out small and slowly get more and more complex, that way you'll be able to get better and better.


I've included the map I made this trigger on in case you need to look at it more carefully.

#### Attachments

triggertutorial.w3m

 16.2 KB

 · Views: 496




 Last edited by a moderator: Apr 12, 2009

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

It's extremely basic but I guess it could be useful for the complete noob.

#### hawk_767

#### hawk_767

 Level 9

JoinedAug 23, 2007Messages465

Resources: 1

#### Resources
Tutorials 1

that was really the aim. For someone who's played the game and understands nothing of triggering. I wasn't trying to aim at anyone who knows slightly what their doing but for people who need somewhere simple to start.

#### medion555

#### medion555

 Level 9

JoinedJun 20, 2008Messages476

Resources: 2

#### Resources
Icons 1Maps 1

Helped me alot with beginning thanks


(maybe show how to make an opening gate trigger for all players couse i only can make it for player red of blue or teal etc open it and not for all

#### LsK_LaDyGaGa

#### LsK_LaDyGaGa

 Level 6

JoinedApr 22, 2009Messages165

Resources: 1

#### Resources
Tutorials 1

to make gate oppen for al...

if you want a gate to open for all units you can change certain conditions or delete them entirely. Remeber conditions are restrictions. If you remve a restriction it works on everbody.

#### slake

#### slake

 Level 5

JoinedMay 12, 2009Messages200

very nice tutorial but that is too simple even for a idiot > try adding more stuff

#### kenxlol2

K

#### kenxlol2

hi, hawk_767. I have try the method you taught in this thread, it is working! I am glad and nervous! But unfortuanately, the trigger is not running while I running in a map which I have imported model. What would be the issue?

#### Tonyvien

T

#### Tonyvien

I dont get what was the point of this trigger, the unit just goes into the center, is that it ?

#### Axarion

#### Axarion

 Level 11

JoinedSep 30, 2009Messages696

Resources: 2

#### Resources
Icons 1Spells and Systems 1

Even when I were a totally newbie I could make this Tutorial^^ Sorry but it's way to easy.

#### Visimirsit

#### Visimirsit

 Level 3

JoinedAug 28, 2009Messages53

this is very usefull for a begginer (do not say noob thats pretty unpolite) xP


Think abut it if he know how to make this triger then the bigginer could try to do other stuff and play with it. Like now i made an event and i have an condition so i can randomly play with the action section... so we have an entering unit event and an bollien condition ( never knew what boolien means but i use it myself xD ) so hmm il chose an action like destructible open/close gate and i chose open. OK COOL now i have a trigger that can open gates! and now he can try to make another trigger that would do the opposite thing. like a unit enters a region. The unit is trigger unit or for example peasant and chose the event to close the gaate  ok now i have a trigger that will open and close the gates hmm well i wish i could do this easier... well there is a way!!! i delete this trigger and create a better one: unit enters a region trigger unit equals to peasant , destructible open/close gate (lets chose open) then a new action that goes after the open/close one, wait (enter a n umber) seconds and then i make a new action again after the wait action, destructible close gate. so here we have an improved trigger that will open a gate when your unit will go thro and after x seconds it will close  make the same trigger form the other side of the gate and wohoo we have a perfect door...  ok now i want to make an respawn (only begginers use this to learn) event: unit dies. condition trigger unit is a blablasomeone, action wait (X) seconds then a new action: Unit- create unit facing angle  in that trigger u will chose how many units should be created what unit will be created and who will own that unit  and here u go we have a simple respawn trigger (the most simple.  ITS VERY BUGGED) but nobody was born knowing  skills comes with time.  still im am a begginer with world editor maybe a lil advanced.  anyways this is good way to learn but i prefer opening other people maps and start playing with triggers


EDIT: P.S. sorry for my english. Still learning it ;p

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

Visimirsit said:

 this is very usefull for a begginer (do not say noob thats pretty unpolite) xP


Think abut it if he know how to make this triger then the bigginer could try to do other stuff and play with it. Like now i made an event and i have an condition so i can randomly play with the action section... so we have an entering unit event and an bollien condition ( never knew what boolien means but i use it myself xD ) so hmm il chose an action like destructible open/close gate and i chose open. OK COOL now i have a trigger that can open gates! and now he can try to make another trigger that would do the opposite thing. like a unit enters a region. The unit is trigger unit or for example peasant and chose the event to close the gaate  ok now i have a trigger that will open and close the gates hmm well i wish i could do this easier... well there is a way!!! i delete this trigger and create a better one: unit enters a region trigger unit equals to peasant , destructible open/close gate (lets chose open) then a new action that goes after the open/close one, wait (enter a n umber) seconds and then i make a new action again after the wait action, destructible close gate. so here we have an improved trigger that will open a gate when your unit will go thro and after x seconds it will close  make the same trigger form the other side of the gate and wohoo we have a perfect door...  ok now i want to make an respawn (only begginers use this to learn) event: unit dies. condition trigger unit is a blablasomeone, action wait (X) seconds then a new action: Unit- create unit facing angle  in that trigger u will chose how many units should be created what unit will be created and who will own that unit  and here u go we have a simple respawn trigger (the most simple.  ITS VERY BUGGED) but nobody was born knowing  skills comes with time.  still im am a begginer with world editor maybe a lil advanced.  anyways this is good way to learn but i prefer opening other people maps and start playing with triggers


EDIT: P.S. sorry for my english. Still learning it ;p


Click to expand...


you could also use enters inbetween your text, that can work too (it also gives my eyes a lot more rest)


example:

 this is very usefull for a begginer (do not say noob thats pretty unpolite) xP


Think abut it if he know how to make this triger


then the bigginer could try to do other stuff and play with it.


Like now i made an event and i have an condition


so i can randomly play with the action section...


so we have an entering unit event and an bollien condition ( never knew what boolien means but i use it myself xD )


so hmm il chose an action like destructible open/close gate and i chose open. OK COOL now i have a trigger that can open gates!


and now he can try to make another trigger that would do the opposite thing.


like a unit enters a region.


The unit is trigger unit or for example


peasant


and chose the event to close the gaate


ok now i have a trigger that will open and close the gates hmm well i wish i could do this easier...


well there is a way!!! i delete this trigger and create a better one: unit enters a region trigger unit equals to peasant,


destructible open/close gate (lets chose open)


then a new action that goes after the open/close one, wait


(enter a n umber) seconds and then i make a new action again after the wait action, destructible close gate.


so here we have an improved trigger that will open a gate when your unit will go thro and after x seconds it will close


make the same trigger form the other side of the gate and wohoo we have a perfect door...


ok now i want to make an respawn (only begginers use this to learn) event: unit dies. condition trigger unit is a blablasomeone, action wait (X) seconds then a new action: Unit- create unit facing angle


in that trigger u will chose how many units should be created what unit will be created and who will own that unit


and here u go we have a simple respawn trigger


(the most simple.  ITS VERY BUGGED)


but nobody was born knowing  skills comes with time.


still im am a begginer with world editor maybe a lil advanced.


anyways this is good way to learn but i prefer opening other people maps and start playing with triggers


EDIT: P.S. sorry for my english. Still learning it ;p


Click to expand...


b.t.w. I think this tutorial is a bit to basic to be a tutorial... When I started trigger editing I just dive into the trigger editor play around for half an hour and got the same result  I didn't start with any tutorial... I started with some common sence of how things are ;D


well maybe it's usefull for some people who don't understand the english words: event, condition and action


because when you understand these words you probably understand a lot more of triggers.


triggers look a lot like talking to someone:


I want a unit to enter some area (region)


and I only want a certain unit type to do that


now I want to make another unit walk to the area of the first unit

doesn't this look like talking?
Events
Unit - A unit enters Area <gen>

Conditions
(Unit-type of (Triggering unit)) Equal to Peasant

Actions
Unit - Order other peasent to Move To (Center of Area <gen>)

#### hawk_767

#### hawk_767

 Level 9

JoinedAug 23, 2007Messages465

Resources: 1

#### Resources
Tutorials 1

There are a lot of people that try to start and have no basic understanding of it all. These are the type of things I made when I first started. I made this for people to learn if their beginning and to then edit and apply their own twists to however they wish it to be. Just a basic skeleton trigger for someone. Sorry for the non-existing spacing haha didn't realize I did that.

#### nutversion1

N

#### nutversion1

Thank you

#### LunarLite

#### LunarLite

 Level 8

JoinedApr 8, 2009Messages499

slake said:

 very nice tutorial but that is too simple even for a idiot > try adding more stuff


Click to expand...

true.

but this tutorial was for dummies who dont know what triggering actually is used for (or w/e). ( i think >_> )

i would recommend hawk_767 to continue on it a little and maybe go trough a whole list of basic triggerings to expand the knowledge of someone TOTALLY new to this.

anyways, gj hawk but its still missing something. ^^

( 3/5 due to incompleteness in my opinion. )

#### FluXz

F

#### FluXz

Very handy, thanks!!

#### SasuhKun

#### SasuhKun

 Level 12

JoinedJul 30, 2009Messages1,156

Resources: 1

#### Resources
Maps 1

Pyritie said:

 It's extremely basic but I guess it could be useful for the complete noob.


Click to expand...


Like me. Thank you very much! +rep

#### RoytjeScheerman3

#### RoytjeScheerman3

 Level 3

JoinedAug 11, 2009Messages11

Resources: 3

#### Resources
Maps 3

I think this is to easy. My first trigger was spawning.


Every 5 sec a footman came and walked somewhere else.


What complete noobs do is try making an escape or something. So you may want s=to say that they do not have to do that and start easy.

#### Psf_Kurt

#### Psf_Kurt

 Level 5

JoinedFeb 24, 2009Messages110

Resources: 1

#### Resources
Maps 1

Pyritie said:

 It's extremely basic but I guess it could be useful for the complete noob.


Click to expand...


Which in this case would be me. Im making my first attempt of a map after years of playing, and this was helpful

#### hawk_767

#### hawk_767

 Level 9

JoinedAug 23, 2007Messages465

Resources: 1

#### Resources
Tutorials 1

I'm glad this is at least helping some people with no idea where to start. I do realize it is extreeeemely basic, but glad to see people appreciating it. Was thinking I should add a few more basic tutorials getting a bit more complex as I continue through them. We'll see though, thanks for reading the tut guys.

#### Psf_Kurt

#### Psf_Kurt

 Level 5

JoinedFeb 24, 2009Messages110

Resources: 1

#### Resources
Maps 1

hawk_767 said:

 I'm glad this is at least helping some people with no idea where to start. I do realize it is extreeeemely basic, but glad to see people appreciating it. Was thinking I should add a few more basic tutorials getting a bit more complex as I continue through them. We'll see though, thanks for reading the tut guys.


Click to expand...


Please do so!


You have no idea how long it took me just to have the map not giving you auto-Victory as soon as it started :S. I had to put a player 2 controlled by computer and remove some of the "Melee intialization" actions (thats how you do it right?)


If you do make more tutorials please send me a PM or something so I can check them out.

#### noobieatmaps

#### noobieatmaps

 Level 3

JoinedAug 30, 2010Messages43

thanks for a little introduction to triggers. I'm sure i'll understand it all soon

#### Bokalaka4a

#### Bokalaka4a

 Level 2

JoinedSep 12, 2010Messages13

WooW the best tutorial FOR BEGINER !easy and explained !I learned how to trigger many stuff from this tutorial! ^^ °>°

#### Player

#### Player

 Level 7

JoinedFeb 9, 2010Messages298

That was really helpful to me! I'm just starting triggering and I was just wondering how to begin and here I found just what I needed - Thanks Hawk 767

#### Madcapercaillie

#### Madcapercaillie

 Level 2

JoinedDec 23, 2014Messages9

This is will really help me in order to make a map thank you!

 You must log in or register to reply here.


### Similar threads

 Basic Triggering


Ralle

Jan 3, 2006

Trigger (GUI) Editor Tutorials

Replies4Views39K

Jul 7, 2016
billel

 How to: Create Spells in GUI - The Beginning


baassee

Aug 18, 2011

Trigger (GUI) Editor Tutorials

Replies6Views32K

Oct 22, 2011
PurgeandFire

 Starting with GUI


Lord_of_sausage

Mar 21, 2008

Trigger (GUI) Editor Tutorials

Replies8Views9K

Jul 2, 2018
Ragnar9

 Reviving Heroes at a Changeable Region


dOT145

Mar 23, 2010

Trigger (GUI) Editor Tutorials

Replies12Views10K

Apr 30, 2010
PurgeandFire

 How to make slow motion special effects


nathanman

Nov 28, 2007

Trigger (GUI) Editor Tutorials

Replies0Views11K

Nov 28, 2007
nathanman

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