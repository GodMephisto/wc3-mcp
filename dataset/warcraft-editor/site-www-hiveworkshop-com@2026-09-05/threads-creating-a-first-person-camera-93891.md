---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-a-first-person-camera.93891/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating a First Person Camera
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-a-first-person-camera.93891/>
#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

Creating a First Person Camera


This Tutorial will teach you how the create a 1st Person Camera, wich is extremely useful for RPG's and a must have for a FPS (duh).

- What do you need? -


- (Any) Warcraft World Editior


- Basic Knowledge can of course come in handy

 Index


1 - The Basic Camera

2 - Looking Around

3 - Looking Up & Down

4 - Tips for Maps using FPC

5 - Second way for Looking Around

6 - Sample RPG using this FPC

Part 1 - The Basic Camera


To get started we need to get ourself a basic camera. This will form the base of the camera and will include the camera height and stuff like that. It is also the camera position you will return to after looking up or down.


Create a category called ''Camera'' with two triggers in it called ''Initialization'' and ''Camera''.


For the Initialization:


Use Map Initialization as Event


Now go to Actions - Camera - Lock Camera Target To Unit and let it use Default Rotation


Also the size of the Hero must be set to 0, otherwise you will still see the model when using the camera, so go to Animation - Change Unit Size and set the scale of your unit to zero.

Note: Because the size of the unit is set to zero, projectiles shot by the unit will NOT be visisble. I am currently trying to find a solution for this.

Note: Because the size of the unit is set to zero, footsteps will appear as one long trail. The only way to remove this is by directly editing the model and removing the footsteps. This can easily be done in the War3ModelEditor.

Your trigger should look like this:

Intilization
Events
Map initialization

Conditions

Actions
Camera - Lock camera target for Player 1 (Red) to A unit 0001 <gen>, offset by (0.00, 0.00) using Default rotation

Animation - Change A unit 0001 <gen>'s size to (0.00%, 0.00%, 0.00%) of its original size


Now on to the ''Camera'' Trigger


As Event go to Time - Periodic Event - Every 0.01 seconds of the Game


And for the Actions


Go to Camera - Set Camera Field (Timed) - Set Player 1's camera Distance to Target to -700 over 0.50 seconds


Now copy this trigger 3 times and chance the ''Distance to Target'' to "Angle of Attack", "Height Offset" and "Camera Rotation".


Make them like this:

Camera
Events
Time - Every 0.01 seconds of game time

Conditions

Actions
Camera - Set Player 1 (Red)'s camera Distance to target to -700.00 over 0.50 seconds

Camera - Set Player 1 (Red)'s camera Angle of attack to 350.00 over 0.50 seconds

Camera - Set Player 1 (Red)'s camera Height Offset to 200.00 over 0.50 seconds

Camera - Set Player 1 (Red)'s camera Rotation to (Facing of A unit 0001 <gen>) over 0.50 seconds

Now we've already got ourself a 1st Person Camera


But this might be still considered as a prototype, because the options are minimum and the way of walking is quite hard. For example, if you are facing a wall in the front you need to click and click and click at the screens edge to get out of your position.

Part 2 - Looking Around


Back to the example of the wall, if you where able to turn around, you could easily get out of your position. Well thats exactly what we're gonna do now. We're gonna make triggers using the arrow keys that make the camera turn when you press them.

NOTE: In chapter 5, an alternative way for looking around is described. Read, it first, and then decide what system you are going to use, because I highly recommend the second way!


Let's start with looking Left and Right


Make 2 categories and call them Look left and Look Right.


We start with left. Create 3 triggers named ''Start Looking left'', ''End Looking left'' and ''looking left''. The trigger ''Looking left must be disabled from the beginning. (Uncheck Initially On in the top of the screen.)

Start Looking left


Events: Player - Player 1 presses the Left Arrow Key


Actions: Trigger - Turn on Look left

Start Looking Left
Events
Player - Player 1 (Red) Presses the Left Arrow key

Conditions

Actions
Trigger - Turn on Look Left <gen>

End Looking left


Events: Player - Player 1 Releases the Left Arrow Key


Actions: Trigger - Turn off Look Left

End Looking left
Events
Player - Player 1 (Red) Releases the Left Arrow key

Conditions

Actions
Trigger - Turn off Look Left <gen>

Looking Left


Events: Time - Periodic Event - Every 0.01 seconds of the game


Actions: Unit - Make Unit 0001 face (Face of Unit 0001[Same Unit]) + 10) over 0.00 seconds


To create this action go to Units - Make Unit face Unit


On part where you need to select which unit your unit should be facing select the Function Arithmetic instead of a unit and click the first of the two numbers.


Make this the facing of your hero. The other number should be chanced into 10 or 15 or something, just how fast you want to look around.

Look Left
Events
Time - Every 0.01 seconds of game time

Conditions

Actions
Unit - Make A Unit 0001 <gen> face ((Facing of A Unit 0001 <gen>) + 10.00) over 0.00 seconds

This will allow your camera to turn left if you push the Left Arrow Key and stop turning when you release the button. Well Done! Now on to Right.


Right is almost the same thing, just copy the 3 triggers, rename them, and bring some small chances to them. They must look like this.

Start Looking Right
Events
Player - Player 1 (Red) Presses the Right Arrow key

Conditions

Actions
Trigger - Turn on LookRight <gen>

End Looking Right
Events
Player - Player 1 (Red) Releases the Right Arrow key

Conditions

Actions
Trigger - Turn off LookRight <gen>

Look Right
Events
Time - Every 0.01 seconds of game time

Conditions

Actions
Unit - Make A Unit 0001 <gen> face ((Facing of A Unit 0001 <gen>) - 9.00) over 0.00 seconds

The only thing to do is to chance the + by Look Right into a -, but remember that Look Right must be disabled from the beginning of the game.


You now have a far more advanced First person Camera! Well Done! But, where still not completely done, we still need to create a look up & down function.

Note: A lot of people would use the up and down arrow keys for a movement system, but I prefer (especially down) you to use them to look up and down. Because, if you are in first person mode, and you get, for example, ambushed by a small enemy, you couldn't look down to see what's attacking you. The same thing would be with picking up dropped items.

Part 3 - Looking Up and Down


Create a new category called ''Up & Down'' and put 4 triggers in it; ''Look Up'', ''Look Up Off'', ''Look Down'' and ''Look Down Off''.


''Look Up'' should look like this

Look Up
Events
Player - Player 1 (Red) Presses the Up Arrow key

Conditions

Actions
Camera - Set Player 1 (Red)'s camera Angle of attack to 370.00 over 0.50 seconds

Trigger - Turn off Camera <gen>

Warning: You can make your Angle of Attack as high as you want, but don't let it diviate too much from the Angle of Attack of your Basic Camera. For example, if the AOA of you basic Camera is 350, and when you look up, it will chance to 400, then objects such as trees will disappear when you look up, and reappear when you return to your basic Camera


''Look Up off'' must look like this

Look Up Off
Events
Player - Player 1 (Red) Releases the Up Arrow key

Conditions

Actions
Camera - Set Player 1 (Red)'s camera Angle of attack to 350.00 over 0.50 seconds

Trigger - Turn on Camera <gen>

''Look Down'' and ''Look Down Off'' are nearly the same, but here the AOA will decrease.

Look Down
Events
Player - Player 1 (Red) Presses the Down Arrow key

Conditions

Actions
Camera - Set Player 1 (Red)'s camera Angle of attack to 330.00 over 0.50 seconds

Trigger - Turn off Camera <gen>

Look Down Off
Events
Player - Player 1 (Red) Releases the Down Arrow key

Conditions

Actions
Camera - Set Player 1 (Red)'s camera Angle of attack to 350.00 over 0.50 seconds

Trigger - Turn on Camera <gen>

Gratz!!!! You now have created a fully controllable First Person Camera Hope your RPG will be great one

Part 4 - Tips for Maps with a FPC


- Make all objects much larger then in normal maps. For example; Humans must size 2 or even 3 if you want a realistic relation between the player's FPC and the NPC.


- If you use cinematics, be sure to turn off the camera and return the unit to it full size before starting the it.


-If you get on higher grounds, the camera won't get higher, so make regions where the Camera height will chance when walking there.

 Viva la Solutione (:\)


Now there's finally a solution for this!

All credits for this solution go to ap0calypse! Thanks!!


First of all go to the Basic Camera trigger you made earlier and delete the Camera Height action.


[trigger=Camera Basic]Camera Basic


 Events


 Time - Elapsed game time is 0.01 seconds


 Conditions


 Actions


 Camera - Set Player 1 (Red)'s camera Distance to target to -1200.00 over 0.50 seconds


 Camera - Set Player 1 (Red)'s camera Angle of attack to 350.00 over 0.50 seconds


[/trigger]


Now click the Variable button (The big yellow cross) and make two new variables.


- Variable 1


 Variable Name: Loc


 Variable Type: Point


- Variable 2


 Varialbe Name: ZHeight


 Variable Type: Real


Leave the array check and Initial Value untouched.


Now create a new trigger called Camera Basic Height


It must look like this


[trigger=Camera Basic Height]Camera Basic Height


 Events


 Time - Every 0.05 seconds of game time


 Conditions


 Actions


 Set Loc = (Position of Your Hero <gen>)


 Custom script: set udg_ZHeight = GetLocationZ(udg_Loc)


 Camera - Set Player 1 (Red)'s camera Height Offset to (ZHeight + 200.00) over 0.00 seconds


 Custom script: call RemoveLocation(udg_Loc)


[/trigger]


To get to the Custom Script action look under - General and copy the text the above trigger into the line.


If right, the height of the camera should now adapt to the height of the terrain. Thanks again Ap0calypse!

Note: Bridges are not recognized as terrain, so the camera will not adapt to the height of the bridge, but the terrain under it. (Same case with any walkable object). This can be fixed by creating regions at the location of bridges. When the hero enter that region, the adapting camera must be disabled and a camera with constant height should be enabled. Inverse when leaving.


-Use Dialogs as talking method. If you don't know how to make dialogs, take a look at the included map, or shearch for a Dialog tutorial on the hive.

Part 5 - Second way for looking Around


You may have already noticed, that you can't look left and right when your Hero is walking. This makes Mouse-Walking still kind of tricky.


We'll, I have found a way of looking wich enables to look around also when you are walking but it comes with a few issues.


-The Make Unit face Unit trigger doesen't work anymore


-When you push Left or Right the camera doesen't react immediatly.


BUT, if you use this way, walking will be far more easy when using Mouse-Walking.

What to do First


Go to the Basic Camera trigger and remove the

Camera - Set Player 1 (Red)'s camera Rotation to (Facing of Galahird 0001 <gen>) over 0.50 seconds
 Part


Now go to the trigger Look Left and delete the Action.


For the new action, go to Camera - Set Camera Field (Timed) - Set Players Rotation to (Choose Function - Arithmetic) [First Number = Camera Field of Camera Object = Rotation of Current Camera] - [Second Number = 3] over 0.05 seconds


Leave the Plus. Then it should look like this

Look Left
Events
Time - Every 0.01 seconds of game time

Conditions

Actions
Camera - Set Player 1 (Red)'s camera Rotation to ((Rotation of (Current camera)) + 3.00) over 0.05 seconds

Now Copy this Action, go to Look Right, delete that Action and paste the new Action in it. Don't forget to chance the + to a -!


Now it should work, if you done everything right. Hope I helped you more with this.

Part 6 - Sample RPG using this FPC

 HTML:

I am working on a RPG that uses my First Person camera. I wanted to finish a little part and post it here to use as sample... But the filesize was to big -.-


So now it's located here:

Tutorial - Blood, Sweat and ice - The Hive Workshop - A Warcraft III Modding Site


Plz rate the map too.


Hope this makes everything a bit more clear ^^

Tnx for using my Tutorial - Please Comment and give Rep


 Last edited: Jan 29, 2011

#### thedeathunterxx

#### thedeathunterxx

 Level 18

JoinedAug 3, 2008Messages1,935

Resources: 4

#### Resources
Maps 4

this is a nice tutorial.


see i have been in map making for 4 years.


and i still cant find a way to make the camera not go all screwy when you approach a wall, without using the arrow key movements.

#### Hawkwing

#### Hawkwing

 Level 34

JoinedNov 28, 2006Messages5,269

Resources: 29

#### Resources
Models 16Icons 3Packs 1Skins 8Tutorials 1

Remember to use the [b][/b], [h1][/h1], [h2][/h2], [h3][/h3] tags to make the different steps more obvious.


Anyways, nice tutorial. Quite descriptive, the pictures are helpful too. You need to watch the smilie spam though. Nicely made, and quite simple. Though there may be a few leaks here and there.

#### Devine

#### Devine

 Level 30

JoinedSep 2, 2007Messages3,688

Resources: 10

#### Resources
Models 3Icons 6Tutorials 1

I learned today

however i wouldnt use the up and down arrow to look up or down but to walk.

#### Hawkwing

#### Hawkwing

 Level 34

JoinedNov 28, 2006Messages5,269

Resources: 29

#### Resources
Models 16Icons 3Packs 1Skins 8Tutorials 1

Ya that's a big flaw. The Right-Click movement system sucks with First-Person.

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

Hawkwing said:

 Ya that's a big flaw. The Right-Click movement system sucks with First-Person.


Click to expand...


Yep, your're absolutely right, i used those 2 buttons for that also. But in my map you it is required to look up n' down... So I chanced it and it's in my tutorial now too =P

#### Hawkwing

#### Hawkwing

 Level 34

JoinedNov 28, 2006Messages5,269

Resources: 29

#### Resources
Models 16Icons 3Packs 1Skins 8Tutorials 1

Anyways, the tutorial itself is nice. It works. It just might not be prefered by some people.

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

Got a New Method of Looking - It's located at the end of the tut

#### Oziris

#### Oziris

 Level 27

JoinedOct 14, 2006Messages4,161

Resources: 7

#### Resources
Icons 1Skins 3StarCraft II Resources 1Tutorials 2

Nice tutorial but i hate to do a map that has 11 players becous u have to do for player 1,2,3,4....making the same triggers again and again....then i get bored to death and do something else.

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

Oziris said:

 Nice tutorial but i hate to do a map that has 11 players becous u have to do for player 1,2,3,4....making the same triggers again and again....then i get bored to death and do something else.


Click to expand...


Yep, but it is made for 1 or 2 players. And if you wanna made itfor 12 players, it's just takes some time, but it's just copying and pasting the same triggers. Isn't too hard if you question me.

#### UnholyDobry

#### UnholyDobry

 Level 11

JoinedJul 27, 2008Messages175

Resources: 9

#### Resources
Icons 8Tutorials 1

Magamdy said:

 Yep, but it is made for 1 or 2 players. And if you wanna made itfor 12 players, it's just takes some time, but it's just copying and pasting the same triggers. Isn't too hard if you question me.


Click to expand...


Yea copying wont be to hard gotha say nice tutorial whoud give u rep if i coud

#### KingWolf

K

#### KingWolf

Thank God Someone made this!

#### nesatauri

#### nesatauri

 Level 3

JoinedJun 6, 2008Messages51

Resources: 1

#### Resources
Maps 1

I was about to go off on a rant on how you handle looking up and looking down, since it looks like what you did was a really nooby mistake. Since its the camera target that is fixed in position in wc3 and not the camera source, simply changing the angle of attack would move the camera source up and down, possibly putting the camera way above the unit's head or below the unit's feet instead of keeping the camera source where it is and actually looking up and down like you want.


This is a big part of the headache of creating a good first person camera. Since wc3's cameras are geared toward third person perspectives, fine control over the camera source is non-existent. You have to manipulate the camera source via manipulations of the camera target, angle of attack, and distance to camera. It involves a lot of trigonometry.


However, you use a negative distance to target. Honestly, I haven't ever tried negative values for that, and its possible that its the solution to my angle of attack woes. However, your warning about deviating from the angle of attack too much kinda tells me its not.


Oh, my, I ranted anyway. LOL

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

nesatauri said:

 I was about to go off on a rant on how you handle looking up and looking down, since it looks like what you did was a really nooby mistake. Since its the camera target that is fixed in position in wc3 and not the camera source, simply changing the angle of attack would move the camera source up and down, possibly putting the camera way above the unit's head or below the unit's feet instead of keeping the camera source where it is and actually looking up and down like you want.


This is a big part of the headache of creating a good first person camera. Since wc3's cameras are geared toward third person perspectives, fine control over the camera source is non-existent. You have to manipulate the camera source via manipulations of the camera target, angle of attack, and distance to camera. It involves a lot of trigonometry.


However, you use a negative distance to target. Honestly, I haven't ever tried negative values for that, and its possible that its the solution to my angle of attack woes. However, your warning about deviating from the angle of attack too much kinda tells me its not.


Oh, my, I ranted anyway. LOL


Click to expand...


Guy, have you tested my camera? Becouse Angle of Attack does NOT put the camera above the head or beneath the feet of the character, that's what Height Ofsett does...

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

why does my character keep spinning?

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

Cheezeman said:

 why does my character keep spinning?


Click to expand...


Well, i know two reasons...


1= You've forgot to create the trigger that stops the camera turning when you release a button.


2= This is a bug i discovered in my map. While i was pressing left to look left, a dialog showed up. When the dialog ended the camera keeps rotating. Simple conlcusion; just press left again and it stops spinnig...


 Last edited: Sep 8, 2008

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Magamdy said:

 Well, i know two reasons...


1= You've forgot to create the trigger that stops the camera turning when you release a button.


2= This is a bug i discovered in my map. While i was pressing left to look left, a dialog showed up. When the dialog ended the camera keeps rotating. Simple conlcusion; just press left again and it stops spinnig...


Click to expand...


The second conclution is probobly right beacuse it says that I won when the game starts.


And yes, it stoped spinning when I pressed left key. What I do not understand is why he keeps headbanging all the time! (Is the facingpoint the point which his head is facing?)

#### Alkser

A

#### Alkser

Very nice tutorial and it really works!

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

Alkser said:

 Very nice tutorial and it really works!

Click to expand...


Of course it works, otherwise I would not have posted it -.-

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

Sorry for multi-post but...


I am making some kind of RPG with my FPC. I don't feel like finishing it completely but if I finished a big part i will maybe post it here as some kind of sample =P

#### Dieseldodge

#### Dieseldodge

 Level 7

JoinedAug 23, 2008Messages196

Resources: 7

#### Resources
Models 2Skins 1Maps 2Tutorials 2

Why don't you post a sample map with the triggers? It is more simple for us..

#### TheBlooddancer

#### TheBlooddancer

 Level 22

JoinedJun 24, 2008Messages3,041

Resources: 2

#### Resources
Maps 1Tutorials 1

'Neat tutorial, quite easy, but very uusefull for people who wanted a first person cam.

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

Ok, i've added the sample of the RPG. It's just a link because the filesize of the map was 6 MB and the maximum was 5 MB...


Hope the map can help you some more ^^

#### Oziris

#### Oziris

 Level 27

JoinedOct 14, 2006Messages4,161

Resources: 7

#### Resources
Icons 1Skins 3StarCraft II Resources 1Tutorials 2

Could u post a demo map?

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

Oziris said:

 Could u post a demo map?


Click to expand...


It IS a demo map...

#### anothergeek

#### anothergeek

 Level 2

JoinedSep 8, 2008Messages11

Resources: 1

#### Resources
Tutorials 1

If you use the Up/Down Arrow Keys to move the camera, you'll have to move your hero with thhe mouse, wich isn't the best in FPS games. But if you use them to move, how should you change your camera up/down? It's hard

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

anothergeek said:

 If you use the Up/Down Arrow Keys to move the camera, you'll have to move your hero with thhe mouse, wich isn't the best in FPS games. But if you use them to move, how should you change your camera up/down? It's hard


Click to expand...


Ugh, this is a tutorial toke make a CAMERA not an arrow key movement system

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

The problem with this is that when you have height variation, the camera can't change with it. Otherwise it's nice.

#### anothergeek

#### anothergeek

 Level 2

JoinedSep 8, 2008Messages11

Resources: 1

#### Resources
Tutorials 1

Magamdy said:

 Ugh, this is a tutorial toke make a CAMERA not an arrow key movement system


Click to expand...

okok, take it easy man... nice tutorial..?....


 . . . ?

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

Belgarath said:

 The problem with this is that when you have height variation, the camera can't change with it. Otherwise it's nice.


Click to expand...


Yea, i tried to fix that, i failed =( If anyone has a method to fix this problem. past it here or send me a private message and i credit you in this tutorial!

#### F4talWr4th

#### F4talWr4th

 Level 6

JoinedAug 5, 2006Messages126

Resources: 2

#### Resources
Maps 1Tutorials 1

I think I'm necroposting but I have something to tell...

I love the tutorial, especially the whole fact that the First Person Camera system you made is slightly similar to "Arx Fatalis"...but there is one problem though, if you get attacked by an enemy, could you try to make the camera (with the player of course) face the enemy??

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

F4talWr4th said:

 I think I'm necroposting but I have something to tell...


I love the tutorial, especially the whole fact that the First Person Camera system you made is slightly similar to "Arx Fatalis"...but there is one problem though, if you get attacked by an enemy, could you try to make the camera (with the player of course) face the enemy??


Click to expand...


Mmmm... Interesting question... Well the player automaticley faces the enemy to attack, but i think this camera doesn't. There should be a way to fix it, tnx for yer question^^

#### SerenityFX7

S

#### SerenityFX7

 Actions: Unit - Make Unit 0001 face (Face of Unit 0001[Same Unit]) + 10) over 0.00 seconds


To create this action go to Units - Make Unit face Unit


On part where you need to select wich unit your unit should be facing select the Function Arithmetic instead of a unit and click the first of the two numbers.


Click to expand...


Erhm, I didn't get this part!


What is the "Function Arithmetic"? I can't find it in my WE, is it a variable or is it supposed to be in "Functions"?


Reply would be appreciated!


rlly nice tutorial otherwise

#### Elsheen

#### Elsheen

 Level 5

JoinedMar 5, 2009Messages136

This is not healthy...... I got sick after watching my character walk around for awhile....


*BLARGH*

#### SP1TF1R3-0031

#### SP1TF1R3-0031

 Level 4

JoinedJul 30, 2008Messages41

Resources: 1

#### Resources
Maps 1

this is very good! i created with your "look left" and "look right" triggers as model, a move forward and a move backward trigger. with this you can look around while you are walking. it works very well. And your camera system is very easy and very good too! 5/5

#### expazatrix

#### expazatrix

 Level 8

JoinedApr 11, 2009Messages256

Resources: 6

#### Resources
Icons 4Skins 1Maps 1

On the Part:


 To create this action go to Units - Make Unit face Unit


"On part where you need to select wich unit your unit should be facing select the Function Arithmetic instead of a unit and click the first of the two numbers."


Make this the facing of your hero. The other number should chanced into 10 or 15 or something, just how fast you want to look around.


The one in Bold I don't understand, can u make it clearer?

#### graystuff111

#### graystuff111

 Level 15

JoinedDec 11, 2007Messages755

Resources: 30

#### Resources
Models 1Icons 24Skins 1Maps 3Tutorials 1

This tut fails. Period.


First person just doesnt belong in wc3. There are limits to what this game can do, and with out RtC or whatever, this won't work well. And, these long and annoying triggers only work for one player, and make no effort to conform the cam to the terrain. So, this is useless.

#### DeadZero

#### DeadZero

 Level 10

JoinedFeb 22, 2008Messages619

Resources: 2

#### Resources
Maps 1Tutorials 1

uhhh...ur wrong and obviously most people here know more about triggers than you because it doesn't make the camera go over and under the unit, it makes you face up, and down...so next time you should test the tutorial or atleast do some research about what ur gunna say before you rant or then you look like an idiot...


see i ranted but for a good cause and if you read this ur probably kinda mad at me right now because i know i would be mad at me


because im just awesome that was


so


HA!

#### Delta GBus

D

#### Delta GBus

How do you make the buttons move with W, A, S and D? Cause i just realized that in some maps, they move the characters with the W, A, S and D.

And plus, how do you make your unit so you can see the arms/shield/gun/weapon/etc. i tried other numbers for height and stuff, but all i see is inside the model. P.S. i removed the animation (0.00%, 0.00%, 0.00%) thingy.


 Last edited by a moderator: May 10, 2009

#### Delta GBus

D

#### Delta GBus

Cheezeman said:

 why does my character keep spinning?


Click to expand...


that's because you didn't turn off the turn (direction) off at map initialization. On the same trigger that you put the lock camera, put turn off left and turn off right.

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

DeadZero said:

 uhhh...ur wrong and obviously most people here know more about triggers than you because it doesn't make the camera go over and under the unit, it makes you face up, and down...so next time you should test the tutorial or atleast do some research about what ur gunna say before you rant or then you look like an idiot...


see i ranted but for a good cause and if you read this ur probably kinda mad at me right now because i know i would be mad at me


because im just awesome that was


so


HA!


Click to expand...


Who are you talking to? me or sumbuddy else =\

#### anothergeek

#### anothergeek

 Level 2

JoinedSep 8, 2008Messages11

Resources: 1

#### Resources
Tutorials 1

Magamdy said:

 Who are you talking to? me or sumbuddy else =\


Click to expand...

Me. I got a mail.

#### DeadZero

#### DeadZero

 Level 10

JoinedFeb 22, 2008Messages619

Resources: 2

#### Resources
Maps 1Tutorials 1

authergeek

#### Bwathke

#### Bwathke

 Level 9

JoinedMay 22, 2009Messages491

Resources: 1

#### Resources
Tutorials 1

The Player - Player 1 Red Presses a certain key isnt in the triggers how do I get that in? I have everything else in (and height offset isnt there) Im using RoC.

FIXED I got UMSWE so I have more triggers like in TFT.

#### Ndreaay

N

#### Ndreaay

T_T the tutorial map isnt there anymore =(

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

Cheezeman said:

 why does my character keep spinning?


Click to expand...


I reaaly don't get you guys... I can downlaod it without any problems... Try opening it immediatly and not saving it.

#### Ricardo Irving

#### Ricardo Irving

 Level 9

JoinedMay 10, 2009Messages542

A brilliant tutorial, but wtf where is the arithmetic thing in the face unit thingy.

#### Magamdy

#### Magamdy

 Level 8

JoinedApr 17, 2008Messages212

Resources: 4

#### Resources
Maps 3Tutorials 1

If...

Look Right

Events

Time - Every 0.01 seconds of game time

Conditions

Actions

Unit - Make A Unit 0001 <gen> face ((Facing of A Unit 0001 <gen>) - 9.00) over 0.00 seconds
Is what you mean... Then:


- Go to Unit -> Make Unit face Angle


- Make ''Unit'' face...


- Now select Arithmetic at ''Function''


- Click the first number, scroll down and select ''Unit - Facing Angle''


- ''Facing of'' the same unit at ''Make Unit Face''


- Set the plus to minus and set 0 to 9 over 0 seconds


Hope that helps ^^ If it doesn't', don't mind to ask again.

#### Ricardo Irving

#### Ricardo Irving

 Level 9

JoinedMay 10, 2009Messages542

?? I can't find the function arithmetic I said.

EDIT: OH - big mistake, I was doing face unit, not angle. Thanks!

#### The_Grapist

#### The_Grapist

 Level 8

JoinedMay 23, 2009Messages421

Th tutorial is awesome,but sadly i do not need it...

Still its the effort that counts...

I will give you +REP...

Good Luck on your next tutorial!

 You must log in or register to reply here.


### Similar threads

Locked

 Third person camera system


NarGalloth

Jun 26, 2012

Triggers & Scripts

Replies6Views2K

Jul 7, 2012
NarGalloth

Locked

[Trigger]
 3rd person camera problem


Jathulioh

Mar 3, 2012

Triggers & Scripts

Replies2Views1K

Mar 3, 2012
shadowvzs

 First Person Camera System


Darkzealot_128

Nov 23, 2010

Misc/General Mapping Tutorials

Replies9Views17K

Aug 23, 2015
Nickz

Locked

[Trigger]
 Help making this camera closer to unit.. It's frustrating.


Nackel

Jun 21, 2009

Triggers & Scripts

Replies4Views1K

Jun 22, 2009
Cokemonkey11

Locked

 Camera not Adjusting


aussj5link

Jan 3, 2015

World Editor Help Zone

Replies3Views2K

Jan 4, 2015
Zunariel

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