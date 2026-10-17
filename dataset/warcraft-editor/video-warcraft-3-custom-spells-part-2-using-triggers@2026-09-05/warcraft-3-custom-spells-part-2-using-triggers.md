---
type: vendor-doc
package: video-keytorch
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=qXrSDu7r6yA
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 1289
chapters: 10
timestamped: yes
---

# Warcraft 3 - Custom Spells - Part 2 - Using Triggers

Source: <https://www.youtube.com/watch?v=qXrSDu7r6yA>

Uploader: KeyTorch | Duration: 21m29s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Lets create some fancy spells with triggers!

Program to change the sounds of effects: https://www.hiveworkshop.com/threads/war3-model-editor.62876/page-24#post-3549672

Other nice resources:
Guide to trigger enhancing spells: https://www.hiveworkshop.com/threads/basics-of-trigger-enhancing-spells.7334/
How to manipulate and modify Special Effects: https://www.hiveworkshop.com/threads/special-effects-manipulation-and-modification.168560/

Timestamps:

0:00 Intro
0:21 Starting our trigger enhanced spell
0:54 Setting up a trigger for our spell
2:42 Overview of what our trigger will contain
3:02 Special Effects
4:52 Leaks (Custom scripts)
6:29 Sound
8:24 Dummy Units
9:12 Mechanics and Logic (Creating our spell)
20:39 Outro

## Chapters

- [0:00](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=0s) Intro
- [0:21](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=21s) Starting our trigger enhanced spell
- [0:54](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=54s) Setting up a trigger for our spell
- [2:42](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=162s) Overview of what our trigger will contain
- [3:02](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=182s) Special Effects
- [4:52](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=292s) Leaks (Custom scripts)
- [6:29](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=389s) Sound
- [8:24](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=504s) Dummy Units
- [9:12](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=552s) Mechanics and Logic (Creating our spell)
- [20:39](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=1239s) Outro

## Transcript

**[0:02](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=2s)** now that we have some more knowledge of the Spells we can make in the object editor let's take a look at making a spell by using triggers to begin making a trigger enhanced spell we first need a spell to enhance so let's go to the object editor and choose a spell when we choose our spell we'll need to think about what type of spell we want to make for example if I want an AOE spell I will have to pick a spell that is an AOE

**[0:30](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=30s)** spell from before like blizzard if I want a single Target spell I will need to pick a spell that is a single Target spell like stormbolt let's make a single Target spell and base it of stormbolt now we have an identical copy of the stormbolt spell so how do we go about enhancing it with triggers let's go into the trigger Editor to make it easier to test our spell I have adjusted that initialization trigger to this

**[1:00](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=60s)** and add a trigger that spawns XP books to enhance our spell create a new trigger I will call mine superbolt to start off the trigger we'll need an event and we want an event that will trigger exactly when the spell is casted in the list of events we can find an event called Unit a generic unit event this event will trigger after any unit in the map has casted a spell after you've picked this event you

**[1:37](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=97s)** will again get a long list of events to choose from notice these two events a unit starts the effect of an ability and a unit begins costing an ability they look like they will trigger at the same time but there's a difference between them that a unit starts the effect of an ability will trigger after the effects of the Spells have been applied and the amount has been drained but a unit begins costing an

**[2:04](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=124s)** ability will trigger after the effect has been applied but before the man has been drained this makes it possible for people to abuse the spell to have the effects applied but no mon drained therefore we choose a unit starts the effect of an ability to continue our trigger will have to make sure it only runs when superbolt is costed we will create a new condition and use the Ability comparison we can then compare the

**[2:34](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=154s)** ability that is casted and check if it's equal to the custom stormbolt now what actions should we choose to enhance our spell I will be using five types of actions special effects sound effects mechanics and logic custom scripts I will add some of them to fix some leaks and dumo units which I will explain later I've set up a new map to show you the different spell actions first up is spell effects we add a new action and we

**[3:10](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=190s)** sort by spell effects now the first two actions are create special effect at point and create special effect on unit create a special effect at point is as simple as it sounds you specify a point and the special effect will be spawn there create a special effect on unit gives you the ability to spawn an effect that is attached to a unit here you get to choose where on the unit you want to spawn the effect but there is no drop

**[3:42](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=222s)** down list here and you have to know the exact names for the attachment points here's an overview over all the points you can attach an effect to and here are some examples of these kind of effects what do you need I my Lord what do you need I me Lord what do you need I a lord before we look at destroy special effect let's just quickly go through the rest of the list so in the top part here we can change the visual

**[4:24](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=264s)** of the special effect then we can change its position and then we can change its orientation ation and at last we can also change how fast spell animation should be now destroy special effect it's a very simple action it does what it's called it destroys a special effect in the game the reason I wanted to explain it as The Last Action is because this action is very important to use to make sure our spell doesn't leak you might be

**[4:53](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=293s)** wondering what a leak is and it's simply just something that is used in the game that never gets destroyed for example if we create an effect that just has a short animation and we call it again and again and again it looks like it's removed from the game when it vanishes because the effect isn't there anymore but it's actually still there it's just invisible and your computer stores this effect in its memory this can eventually

**[5:19](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=319s)** fill up your memory and cause huge lags so you might understand now why it's so important to destroy the effects after you've created them now I've explained to you you what special effects are and you should also know a little bit about leaks now let's look at an example of creating special effects and how you can avoid leaks here is an example of creating a special effect before we used to put the position of unit or Center of

**[5:45](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=345s)** region right into the function that create the effect this is not a good way to do it because it will leak a position meaning the computer will remember this position for as long as you're in the game we can avoid this by by using a temporary variable and using that variable in our special effect function instead now to make sure our effect also doesn't leak we destroy it now the only way to destroy the

**[6:12](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=372s)** position is with a custom script you can find it under actions and custom script here we can write one line of code and the code you see here is the Jaz code to remove a variable this removes it from the memory next up is sounds some of the spell effects come with built-in sounds this can be beneficial if the default sound is the sound that you want but if you want to replace the sound then you have to use a third party program

**[6:42](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=402s)** unfortunately I will not show you how to use it but I will leave a link in the description to a program that can do this if you have a spell effect that don't contain any sound you can play extra sounds with triggers and here are all the different sound actions you can use if we choose the play sound action and click on the sound word we see that we have no sounds to choose from the reason for this is that we first have to choose

**[7:09](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=429s)** the sounds we want our map to load we do this in the sound editor here you can see all the 7,495 sounds that are available in the game I'm going to go down to the peasant you can play all the sounds to see which one you like right to use it in a trigger we have to right click it and choose use as sound the sound will then be saved as a variable in our map now there is one important thing you have to

**[7:36](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=456s)** know the sound variables can either be a normal sound or a 3D sound if we go into the properties of the sound variable you can see whether your sound variable is a 3D sound or not we can turn this on and off as we like the reason you need to care about this is because there are two main ways to play sounds either as a sound that sounds like it's right in front of you or as a sound that comes from a specific Direction normal sounds

**[8:05](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=485s)** are used in these two actions and 3D sounds are used in these two while we're here if you go to file you can import your own sounds if we now try to add sounds you can choose between them from the variable list domain units is also a useful concept to learn they're used to give you a way to create points which you can cast spells from and spawn effects on or slash to there's not a built-in way to do this so we use Dum units as a

**[8:38](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=518s)** workaround essentially what you do is that you copy a unit make it invisible and impossible to interact with then you use triggers to spawn this unit at the position of where your herrow wants to send a spell and then you use the dummy unit to sound spells from that unit or position spawn effects Etc to make a copy of a unit into a dummy unit you need to change these fields the fields that are blank here just means

**[9:07](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=547s)** that you have to remove anything that are on those fields and finally we get to mechanics and logic so far we've gone through adding effects sounds fixing leaks and creating dummy units but our spell is missing its mechanics like how much damage will it do which objects will it be able to Target and how it scales when you level it up some of this we will do with triggers but some of it we can do in the object

**[9:34](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=574s)** editor too first off let's see how our spell is working right now I'm going to rename the spell to superbolt so it's easier to recognize then I'm going to create a custom hero remove his existing spells and add our spell when we test the spell we can see that it's working like a normal stormbolt we don't want that we want to add our own mechanics to the spell so we need to change some fields in the object

**[10:01](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=601s)** editor first we want to do the damage with triggers this way we have more control over how the damage is done so we're going to set it to zero for all of its levels now I want to make this spell be able to Target both units and buildings I want it to have quite a small impact on units but a huge damage to buildings we can't do this in the object editor but with triggers this is possible let's make our action for the

**[10:29](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=629s)** damage image in our Super Bowl trigger I'm going to add a new action it's going to be a if then else action if you're familiar with programming you might know this as an if else statement what it basically does is to check if something is true or not if it is true it does an action and if it is not true then it does a different action the condition for our damage will be to check if our Target is a unit we're

**[10:54](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=654s)** going to open the first part here we don't want to check the triggering unit we want to check the target unit of ability being cast and instead of a structure we want to check if it's a ground unit now our condition is correct let's create the action that will happen if this is true first we sort for unit as you can see there are a lot of things we can do but we want damage Target which is at the bottom

**[11:31](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=691s)** we'll change the damage to be done to Target unit of ability being cast the same as before now for the damage we don't want it to be the same all the time we want it to change with each level of the super bolt spell we can do this by creating a base damage and multiplying it for each level for example to do this let's add the arithmetic function let's set the base damage to 25 and we change the plus sign to

**[12:04](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=724s)** multiply now there is one important thing to think about you see the numbers are numbers with decimals this is because you can't do any math on different type of numbers they have to be the same type in the Warcraft 3 editor these types of numbers are called real numbers if you know some programming from before you might also know them as float numbers the reason I'm mentioning this is because the level of our herrow is an

**[12:31](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=751s)** integer which is a number without decimals so we need to change it to a real number to do math with it there is a function that can do this called conversion convert integer to real and we then get to choose which integer we want to change to real we choose unit level of ability for unit and set the ability to superbolt we can also change our attack type and damage type I'm going to set mine to Magic and

**[13:13](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=793s)** Magic lastly we need to set our else action to do nothing we can now just copy this action and change our Target to a structure and the damage to something bigger I'm also going to change our attack type to Siege because that will give our spell more damage towards buildings but to be able to hit a building we have to make sure that the spell we're enhancing has buildings allowed as Target go into the Super Bowl spell and

**[13:55](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=835s)** add mechanical and structure to the targets this will allow it to also Target buildings then copy it to all the levels right now our damage from the triggers are taken instantly when the spell is casted but our spell also has a missile that will stun the target when it reaches it to make the damage and the spell missile synchronized we can change the missile speed to something really high so it will almost be instant

**[14:23](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=863s)** also now let's test the spell I am not afraid As You Wish as you can see it does a lot of damage to buildings for and not so much to units now maybe this makes the spell a little bit overpowered but wouldn't it be cool if the spell also used blizzard when it's targeting a unit however there is no trigger we can use to just spawn a spell we could use the trigger to order our hero to cast blizzard but then it would need to have

**[14:58](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=898s)** blizzard as a spell and he also wouldn't be able to cast it at the same time that he's casting other spells this is where we can use dummy units to cast the spell for us so I'm going to create a dummy unit from the instructions I gave you earlier now we're going to spawn our D unit right under the target but to not leak any position we will create a new variable that will store the position of the target of the spell and to not

**[15:46](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=946s)** create any leaks we will add the small custom script that will remove our variable after we're done using it in the parenthesis after remove location you have to write UD Gore and then the name of the variable that you want to remove drag it to the bottom so it is executed last then we're going to add an action to spawn our dummy unit we'll use the create unit facing angle and change the unit to dummy then we need to change the

**[16:43](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=1003s)** unit owner to the owner of the unit casting the spell which is the triggering unit and instead of using a region we will now use our position variable then we need to create an expiration timer so that our Dum unit eventually will disappear I'm just going to set it to 5 Seconds sure to drag it below the create unit action drag your set variable action to the Top If not the Dum unit won't work add a if then else function with

**[17:40](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=1060s)** the possibility for multiple functions then we add our condition which will check if the unit is a ground unit it will be identical to our condition in the first if statement before we tell our dummy unit to cast blizzard we need to create a custom blizzard ability this is because the normal blizzard ability needs Mana to be drained and our d unit doesn't have any Mana so set all the Mana requirements to

**[18:16](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=1096s)** zero and just to have things organized I'm going to set the editor suffix to superbolt in parenthesis now we can make our demo unit cast blizzard add a new action to the then actions go into the unit action types and find add ability set the ability to custom blizzard and set the unit to last created unit create another Den action and go into the unit types this time you have to def find set level of ability for

**[19:02](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=1142s)** unit then set the ability to custom blizzard and the unit to last created unit the level of the spell we set to the level that the hero has in superbolt to get this value we use the unit level of ability for unit select superbolt as the ability and make sure to drag it under our first action now we're going to add the Last Action this will order our D unit to cast the spell sort for unit actions and find the issue order

**[19:41](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=1181s)** targeting a point in this list you can see all the different actions and spells that has a point as a Target you can see that there is no spell that has a name custom blizzard this can be a bit confusing in the beginning but our spell is based on the normal blizzard so if we order our dumy unit to cast the normal blizzard it will actually cast whatever spell it has access to that is either the normal

**[20:08](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=1208s)** Blizzard or any spell that is a copy of the normal blizzard Now set the unit to the last created unit and the region to our position variable and at last move our action to the Bottom now finally we get to test our spell and look at that it's raining ice screams now you have lots of tools to be creative and create all kinds of different spells there is one problem with our trigger enhanced spell though

**[20:48](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=1248s)** and that is the case where two Heroes cast our super bow spell at exactly the same time right now our spell does not have the ability to have multiple instances of itself and it's possible to fix this by making it multi-unit instantiable as it's called or muui for short it's not too hard to fix but I will create a separate video for this anyway if you're still watching thank you so much for watching this far I hope

**[21:17](https://www.youtube.com/watch?v=qXrSDu7r6yA&t=1277s)** you enjoyed the tutorial I would appreciate it if you like the video and subscribe if you want more content and yeah thanks again for watching and have a great day
