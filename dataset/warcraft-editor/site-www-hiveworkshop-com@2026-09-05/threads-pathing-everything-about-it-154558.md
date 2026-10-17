---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/pathing-everything-about-it.154558/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Pathing Everything About It
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/pathing-everything-about-it.154558/>
#### Dr. Boom

#### Dr. Boom

 Level 16

JoinedMay 1, 2008Messages1,605

Resources: 4

#### Resources
Maps 1Spells 2Tutorials 1

Pathing - Everything about it [2.0]

Seas =)

 The Directory


1.0) The first words

2.0) Where can you see the pathing?

3.0) Different Pathing Types

 - 3.1) Structures

 - 3.2) Trees and Doodads

4.0) Naval Pathing ( Water )

 - 4.1) Structures

 - 4.2) Units

5.0) The Pathing Blocker

6.0) Create your own Pathing Texture

7.0) Invisible Platforms

8.0) Pathing and Triggers

9.0) Last words

1.0) The first words


First of all I want say why I want write this tutorial:


- Pathing Textures and Pathing Blocker are an important part of mapping. With Pathing you can do a lot of things and improve your own map. Also in the last time I got some questions about it, so I want write this tutorial.

2.0) Where can you see the pathing?


If you open your World Editor go to the "Terrain Editor". This is the basic window of the world editor. ( Else you can get it when you press "F3" ) Now press "P" and you can see some different colors. <-- These colors are the pathing. You can see pathing at structures that has pathing, at un-build-able terrain and at the pathing blockers.

3.0) Different pathing types

--> 3.1) Structures


You can see different pathing with different colors. Each color has an own function. Now we want look to a normal structure pathing

Here you can see two colors:

Purple: No ground unit is able to walk over this area. Also you can't build on this area.

Blue: Units can walk over this area but you are unable to build at this area.


If you want to change this pathing do the following:


- Open the Object Editor and search for your structure, where you want change the pathing.


- Search for the line: "Pathing - Pathing Map"


For example I will change the pathing of a tower. I change normal tower pathing to the lumber mill pathing:

Left side: You see the tower with the pathing of the lumber mill


Right side: You see the tower with the normal tower pathing.


Why I did this?


- If I would increase the size of the left tower but it still has the normal pathing, you can walk through the tower until the unit reach the pathing.

--> 3.2) Trees and Doodads


After we looked to the structures, lets get another useful example for pathing: The Trees and other doodads!


If you want change the size of the trees, to get a better feeling that trees must be higher then structures, then it's recommend to change the pathing as well. Because if you don't, again units are able to walk through the tree until they reach the pathing.


The same thing is with ( for example ) rocks. If you see the following picture, you will understand, what I'm talking about:

The left pathing: Pathing from a lumber mill


The right pathing: Pathing from the normal rock


( Both rocks have increased size )


And here you see where you can change the pathing texture of doodads:

4.0) Naval Pathing ( water )


First let us activate the Naval Pathing view. If you are at the Terrain Editor ( press F3 ) press: "N" ( "N" is for: View > Pathing - Naval )


Now we see an orange pathing at all places, where you don't place any terrain water areas.

This orange pathing means that you are not able to put "water structures and water units" on it.


Don't worry, I will explain it =)

--> 4.1) Structures


To get a "Water Building" you need only to change one option: "Pathing - Placement Requires".

Now, after you change the pathing to "Sea-Pathable" you are only able to put or build this structure on non-orange areas so only in the water.

--> 4.2) Units


Almost the same thing you can to with units, if you want them only place or walk in the water. Here you need change: "Movement - Type "

For this "non land walking" you need to change the movement into "Float". Now the unit can only be put or walk in water areas.

5.0) The Pathing Blocker


Now, after we got the pathing of structures and doodads, lets get a look to Pathing Blocker. Pathing Blocker can be useful if you don't find the correct pathing texture or if you want create an area that should be un-walk-able.


The type of Pathing Blocker:


Now we got three different colors but don't worry!

The White Pathing: No ground and no air unit can leave the circle here. It's the > Pathing Blocker (Both) <

The Purple Pathing: No ground unit is able to leave the circle but the air unit can escape. It's the > Pathing Blocker (Ground) <

The Teal Pathing: The ground unit can leave the circle but the air unit not. It's the > Pathing Blocker (Air) <


-> You can't create buildings on all pathing types.


-> You can use "normal" or "large" pathing. For everything the correct pathing =)

6.0) Create your own Pathing

- You need a photo editing program that is able to save images as tga files.

The first step: Think of the size you want create the pathing with. The smallest size here is 1x1.


( Reminder: 2x2 is the normal Pathing Blocker - 4x4 is the "Pathing Blocker -Large" )


Also you shouldn't create the pathing to big ( look at the two screens I give for an example):

( For the example I used the map size: 96x96 )


Also an important part of the creating are the colors:

0000FF: Unbuildable

FF00FF: Unbuildable / Unwalkable

FFFFFF: Unbuildable / Unwalkable / Unflyable

00FFFF: Unbuildable / Unflyable


You can create your own pathing as you want. You don't need to use the same two values, you are able to create ( for example ) 2x4 - 4x6 - 1x3 pathing. So you can create pathing that supports everything you need.

The second step: After you create you pathing you need to save it as a 32bit tga file.

The last Step: Now you can go to the editor, import your created file and use it as your own pathing.

7.0) Invisible platforms


Another important part of pathing are invisible platforms platform.

( the blue parts one the cliff )


When you put those platforms on a cliff, you are able to walk over this cliffs.


Note: You can't put them on buildings, so you can walk over them ( remind: The blue pathing = unbuildable )

8.0) Pathing and Triggers

--> Create and remove pathing blocker


You can create pathing blockers with the trigger actions:

Destructible - Create a Pathing Blocker (Both) (Large) at (Center of (Playable map area)) facing (Random angle) with scale 1.00 and variation 0
Of course you can delete pathing blockers as well:

Destructible - Remove Pathing Blocker (Ground) (Large) 0057 <gen>
--> Trees with pathing


Also you are able to create trees with pathing on a pathing area. For this example...:

Here you see the pathing area ( Pathing Blocker - Large are used here)

Now I use this trigger. This trigger create trees in a circle:

Create Trees at Pathing Area
Events
Time - Elapsed game time is 2.00 seconds

Conditions

Actions
Set TempPoint = (Center of Region 000 <gen>)

For each (Integer A) from 1 to 10, do (Actions)
Loop - Actions
Set TempArray[(Integer A)] = (TempPoint offset by 250.00 towards (36.00 x (Real((Integer A)))) degrees)

Destructible - Create a Summer Tree Wall at TempArray[(Integer A)] facing 0.00 with scale 1.00 and variation 1

Custom script: call RemoveLocation(udg_TempArray[GetForLoopIndexA()])

Custom script: call RemoveLocation(udg_TempPoint)

As you see at the following screen: you are able to create trees with pathing on the pathing area

--> Locust units on Pathing area


If you got a unit with Locust, you can let them walk over a pathing area with a trigger: ( I used that simple trigger )

Move Locust Unit into the Pathing Area
Events
Time - Elapsed game time is 2.00 seconds

Conditions

Actions
Unit - Create 1 Test Unit for Player 1 (Red) at (Center of Region 001 <gen>) facing Default building facing degrees

Unit - Order (Last created unit) to Move To (Center of Region 000 <gen>)

( After testing the created unit ( with locust ) walks into the pathing area ( the screen in "Trees with pathing" ))

9.0) Now the last words:


- Maybe you see there are > Pathing Blocker (Dead) < They don't have any color and you can walk and build on them.


- To the pictures I give you where you can see, where to change the pathing: I got the English World Editor. For the German World Editors I don't know, if this Pathing option are at the same position <-- sorry for that.


- I hope I got everything about pathing. It would be cool if someone read this and found some mistakes, errors or improvements please and would post it.


- Here again "thanks" to all who give a feedback for my tutorial. As you see I edit many things while [1.0] to give the best help for pathing for you.

== 1.0a ==

- Add: The Changelog


- Change: The Pathing Colors


- Change: Some typos

== 1.0b ==

- Add: "Create your own Pathing!"


- Add: "Invisible Platforms"


- Add: The Directory


- Add: Links from the Directory to the Tutorial


- Add: "Special thanks to" tag


- Change: Some typos and colors

== 1.0c ==

- Add: "Pathing and Triggers" ( Create / Remove Pathing Blocker + Trees with pathing + Locust units)


- Change: The Directory

== 2.0 ==

- Add: "Naval Pathing ( water )


- Change: The Directory


- Crazed_seal2 <-- thanks for helping with the colors / idea with the invisible platforms / help with fixing some typos


- ap0calypse <-- thanks for helping with "Create your own Pathing" / helping with "


 Last edited: Jan 20, 2010

#### Crazed_seal2

#### Crazed_seal2

 Level 14

JoinedJan 5, 2009Messages1,126

Resources: 10

#### Resources
Icons 1Skins 1Tutorials 8

I suggest adding a thing about invisible platforms, Either then that its an overall good tutorial prolly some typo's in there( i haven't bothered to check) and its purple not red, teal not blue-green.

#### ap0calypse

#### ap0calypse

 Level 28

JoinedJan 26, 2007Messages4,787

Resources: 5

#### Resources
Maps 3Spells and Systems 1Tutorials 1

Verry good, people should watch out with pathing, they're a huge part of nearly every game.

TD's use them so the creeps don't get stuck all the time (note that when TD's don't have that, you also say the TD has a bad "Pathing", if that clears it up a little ^^).

All other maps... well, look at the screenies above, especially the paladin-screeny should be obvious


 Dr. Boom said:



 This colors are the pathing.


Click to expand...

These (plural).

This gets my vote of approval

#### Crazed_seal2

#### Crazed_seal2

 Level 14

JoinedJan 5, 2009Messages1,126

Resources: 10

#### Resources
Icons 1Skins 1Tutorials 8

This has my vote for approval, it is now very useful for newbie mapmakers.

#### Spoontoobig

#### Spoontoobig

 Level 10

JoinedNov 3, 2009Messages687

Resources: 1

#### Resources
Maps 1

Damn this is awesome :> I about spelling/grammar  *puff* Joking...  Really awesome tutorial 5/5 +REP +Approval :>

#### Slaydon

#### Slaydon

 Level 20

JoinedJan 6, 2008Messages2,622

Resources: 10

#### Resources
Icons 4Tutorials 6

nice tutorial, but you could say if placing destructables with cordinates will remove the pathing, also Locust "Removes the soul" and pathing of stuff

#### Dr. Boom

#### Dr. Boom

 Level 16

JoinedMay 1, 2008Messages1,605

Resources: 4

#### Resources
Maps 1Spells 2Tutorials 1

Seas =)


First of all I want say "THANKS DUDES" =) Really cool that you help me with the tutorial =)


@Slaydon: I only add the thing with the destructible, because I try that with the locus units but this doesn't work ( or I missed the way how you put locus units on a pathing area )


Edit: Ahh now I understood what you was talking about: Add this to

#### Crazed_seal2

#### Crazed_seal2

 Level 14

JoinedJan 5, 2009Messages1,126

Resources: 10

#### Resources
Icons 1Skins 1Tutorials 8

I think its locust? not locus

#### Dr. Boom

#### Dr. Boom

 Level 16

JoinedMay 1, 2008Messages1,605

Resources: 4

#### Resources
Maps 1Spells 2Tutorials 1

Seas =)


Ahh ok thanks for that =)

#### Kwah

#### Kwah

 Level 24

JoinedMay 9, 2007Messages3,561

Resources: 12

#### Resources
Icons 7Maps 3Tutorials 2

This tutorial is now approved, and moved to General Mapping.

#### supertoinkz

#### supertoinkz

 Level 28

JoinedDec 29, 2008Messages3,351

Resources: 46

#### Resources
Models 23Icons 17Skins 2Maps 2Tutorials 2

would you add Naval Pathing? :>

#### ubran

#### ubran

 Level 5

JoinedSep 29, 2008Messages170

My only dilemma with pathing now is making small blockers for those diagonals that you don't wanna chunk off or cover up with some trees off to the side. Hmmm..


On another note involving pathable doodads : is it possible to make part of a doodad walk-under-able while keeping other parts walkable? I got something like that to work by making hills under platforms on something so you walk on it and under the archway, but that takes too long to be worth it for each little building.

#### woiffal

#### woiffal

 Level 3

JoinedApr 25, 2010Messages16

Resources: 1

#### Resources
Maps 1

Hawidere!


Great Tutorial, was interesting to see how some things work, especially i forgot its possible to LOOK at pathing...


I was reading the tutorial because i am searching for a way to use the visionblockers. They appear as little checkered boxes and i cant figure out how to use them properly... So i thought maybe you know more about them?

#### kaizar

#### kaizar

 Level 7

JoinedJul 20, 2009Messages295

Resources: 1

#### Resources
Maps 1

Good tutorial, I've learned some new things after reading it. ^^


+Rep

#### matzu

M

#### matzu

i have one question.. if i make a mountain with height tool i saw ground units can climb it..


to make it unpathable i need to use pathing blockers or are there any triggers i can make/use?

#### ap0calypse

#### ap0calypse

 Level 28

JoinedJan 26, 2007Messages4,787

Resources: 5

#### Resources
Maps 3Spells and Systems 1Tutorials 1

Wouldn't triggers overcomplicate things?

You can't make terrain unpathable with triggers, but you can make it so units cannot walk over certain regions.

Isn't that two times the same thing, you ask? No it isn't: terrain pathability can't be changed in-game, so there is no easy way of doing it.

You can, however, create a system that moves units away from the region they're in (if you take your time to make it smooth enough, it'll almost feel like pathing blockers).

I still highly suggest you use pathing blockers though, I can't see anything wrong with them.

If you're afraid you might've missed a spot, just turn on terrain pathability and make all doodads/units/shadows invisible so you can have a clear view on the pathing.

#### nickel510

#### nickel510

 Level 8

JoinedOct 1, 2010Messages408

Oh great Pathing One, I have a question for you. I want to make it so that units cannot attack through things like walls that are actually doodads, when fog of war is disabled, but still have the agrod unit find a way around the wall or doodad like it regularly would. Is this possible? Please help me Path Master!

Edit: Also, any great insights on bridges and ramps to your pathing tutorial would be nice . Not sure if there are any, but it would be fitting and very helpful.

Edit: also, appocolypse, isn't JNPG better to use for unpathable tiles? I ask because I may switch to JNPG if so.


 Last edited: Nov 20, 2010

#### ubran

#### ubran

 Level 5

JoinedSep 29, 2008Messages170

Also, for chunky rocks, you can give them no pathing and place in blockers yourself.. Most people reading your tutorial for the first-time knowledge probably don't know this, so it may be handy to mention in your tutorial.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

There is this one trick that I use all the time.

Try to hold SHIFT on your keyboard and then place objects on the map.

#### kellym0

#### kellym0

 Level 25

JoinedNov 28, 2009Messages933

Resources: 57

#### Resources
Models 51Icons 1Skins 1StarCraft II Resources 2User Interface 1Tutorials 1

you gave the hex codes for most pathing types, but you didn't give the hex code for the orange pathing for naval units, why not? that color is something I desperately needed a while back, back when I was trying to create naval only pathing blockers.

Does any body know?

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Isn't naval on a separate layer?

#### Doomcore

#### Doomcore

 Level 6

JoinedSep 7, 2017Messages207

Sooooooo helpful.... sooooo informative... and sooo useful.

^^ How about pathing gates and bridges?

#### fostaa

#### fostaa

 Level 6

JoinedMar 18, 2011Messages109

Any idea why are pathing blockers counted as Destructibles, therefore I can't place more than 6144 INCLUDING TREES. This is so silly. Any ideas how to efficiently fix it / go around this?

#### Crazed_seal2

#### Crazed_seal2

 Level 14

JoinedJan 5, 2009Messages1,126

Resources: 10

#### Resources
Icons 1Skins 1Tutorials 8

fostaa said:

 Any idea why are pathing blockers counted as Destructibles, therefore I can't place more than 6144 INCLUDING TREES. This is so silly. Any ideas how to efficiently fix it / go around this?


Click to expand...

I havent touched the World Editor in years but maybe you can use triggers/scripts to remove pathing blockers since they are destructibles which is something you cant do with normal props.

#### Barade

#### Barade

 Level 29

JoinedFeb 2, 2006Messages1,692

Resources: 5

#### Resources
Tools 1Maps 2Spells and Systems 2

Hi, invisible platforms have the same pathing as shallow water (look at the color when showing ground pathing). You can change the movement type of a floating unit to foot and place it on an invisible platform. Then change the movement type back to floating. You can move the unit ingame on the invisible platforms. Maybe you should add this info to the tutorial. It might be useful when you want to move ships on non-water areas etc.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Unless you've tested this, I think you're wrong. Here's why:

The normal pathing map doesn't actually show whether something is water-pathable. Water is a separate pathing map - blue just means unbuildable, which water typically is.

You can easily see that this is true by placing any rocky tile - they are typically unbuildable, hence blue, but you can't place water units on them in editor.

#### Barade

#### Barade

 Level 29

JoinedFeb 2, 2006Messages1,692

Resources: 5

#### Resources
Tools 1Maps 2Spells and Systems 2

It's not blue. Invisible platforms have some purple pathing (not the same purple as shown in the first thread) and it is the same for shallow water. You can use a floating unit like a ship change its movement type to foot place it on invisible platforms and change it back to floating. Then you can move the ships ingame over the invisible platforms but not outside of them. I tried it in the editor. You can just try it.

I am not sure if it has something to do with the invisible platform itself (some destructible setting).

You might be right about the flag if something is water or not but apparently the purple color means that it is pathable for floating units?

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Invisible platforms in editor don't seem to work like that for me. Also, for me they have blue pathing.

#### Freddyk

#### Freddyk

 Level 23

JoinedSep 7, 2013Messages240

Resources: 12

#### Resources
Models 11Icons 1

I have an issue with naval pathing and invisible platforms :

Apparently the blue pathing of destructibles removes the orange naval pathing.

I wanted to use the trigger condition "Terrain pathing at point of type Float is off" (which clearly detects the orange naval pathing) to perform some action when my unit (ground unit) is on shallow water, but unfortunately it detects invisible platforms too...

 You must log in or register to reply here.


### Similar threads

 Ground Pathing Blockers Upsize


Vinz

Jun 21, 2014

Misc/General Mapping Tutorials

Replies11Views6K

Jul 24, 2025
Chaosium

 [Terraining] Alternative Pathing Technique


sonofjay

May 22, 2012

Misc/General Mapping Tutorials

Replies34Views19K

May 15, 2013
EmoBrother

 Walkable Walls/Doodads


Archian

Aug 10, 2006

Misc/General Mapping Tutorials

Replies0Views38K

Aug 10, 2006
Archian

 Terrain: Playable Forests/Villages


Bugbuster96

Jul 16, 2010

Misc/General Mapping Tutorials

Replies6Views13K

Feb 14, 2011
Orcnet

Locked

 Failure to generate appropriate Pathing on Water


WherewolfTherewolf

Nov 25, 2010

World Editor Help Zone

Replies8Views2K

Dec 4, 2010
WherewolfTherewolf

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top