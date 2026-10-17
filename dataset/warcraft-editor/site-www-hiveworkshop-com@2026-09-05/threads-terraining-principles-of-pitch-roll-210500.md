---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/terraining-principles-of-pitch-roll.210500/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Terraining Principles of Pitch Roll
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/terraining-principles-of-pitch-roll.210500/>
#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

Introduction

Pitch and Roll are two extremely useful object data fields usable for many object data fields. This tutorial will focus entirely on the usage of doodads and destructibles. They can seem relatively complex when starting out to use them, but actually, they are quite simple. The two fields allow you to tilt an object, forwards and sideways, so as to put different angles to use, tilt normal things to show gravity shifts; there are hundreds of uses for this application.


I've also found that a lot of people either have no idea how they work or only have a vague idea - using random numbers of any kind of silly thing to limit their usage of these tools. Therefore, this should be good for anyone who wants to understand the Pitch and Roll Angle fields better.


In this tutorial, I'll be showing you the basics of the usage of positive numbers, which allow you to tilt objects to the angle of the terrain, and then moving onto a technical, mathematical explanation of the ideas behind the use of negative numbers, which will allow you to tilt objects at will on flat ground.


All you need to know for this is a basic understanding of the conversion of radians and degrees - this will be covered in the negative number section. Note that understanding of some mathematical concepts would help.

Note that the test doodad in this tutorial is a unit model, but not actually a unit. It was simply for the convenience of showing the turning well.

I: Positive Numbers

These are the fields we are talking about.


Positive numbers are quite a simple tool. Basically, they allow the user to tilt doodads/destructibles according to the gradient of the terrain. The almost only usage of this is to set both fields to 3.14 (as you might notice - this is pi). What this does is allow the maximum angle of pitch and roll of the doodad in relation to the normal, flat floor on a gradient to be a very high amount (180 degrees to be exact).


Because, practically speaking, no wall in WC3 modding is actually at a 90 degree angle to the ground, it is technically more than you need and therefore all that you need. If all that went over your head in technicality -- the unit will simply pitch and roll to the angle and gradient of the terrain, as in this picture:


Note how the Proudmoore model on the right hand side is at an exact perpendicular angle to the ground. This is allowed by the high maximum angle of pitch/roll in relation to a flat floor.


The differences between the Pitch and Roll angles are not particularly important in positive values as the process and everything is quite simple. Once the mathematical ideas behind this stuff are elaborated on, you can do your own (though limited) positive number pitching and rolling as well.

II: Negative Numbers


Negative numbers are the fun stuff of pitching and rolling things. Firstly, a guide to the actual impacts of pitch/roll:


These two lines show the path that your doodad's position will take as you increase/decrease the negative value of Max Pitch/Roll Angle. In the picture, the black line represents the Pitch Angle whereas the red represents the Roll Angle. Note that the position of the origin (of your unit) does not change. only the actual model. Simply imagine that the Proudmoore model's head will move along those lines as you change just one value and between them as you change both values.


Now, before we can start on the actual work, you need to understand the interchangeable concept of radians and degrees.


Radians and degrees are simply different ways of saying the same thing. While degrees go from 0-360 in a circle, radians go from 0-2 π. By the same token, in both instances, 0 = 360; 0 = 2 π.


You can change from degrees to radians by the formula of: "x * (π/180)" where x = your degrees and π = 3.14


This works backwards too: "x * (180/

π)" converts x radians into degrees.


So, with this idea in your mind, some important degrees/radians are:


2 π = 6.28 = 360


1.5 π = 4.71 = 270


1 π = 3.14 = 180


0.5 π = 1.57 = 90


0 π = 0 (==) 2 pi = 360


Therefore we move onto the next concept, which might seem obvious to you already by now. Pitch and Roll Angles have directions. These are determined in a simple way.


In this picture, firstly, there is a green line. This green line is running from the ORIGIN, the exact centre of the two circles of Pitch (Black) and Roll (Red), to the point decided by the Roll and Pitch of your doodad. They are, in a way, values of X and Y, with a constant value of Z that is simply the height of your model. Your model simply wraps around the green line. It is the green line for all intents and purposes.


As well as this, two arrows have been added. This is to show the direction that Pitching and Rolling occurs in - 'higher' negative values (lower values overall) will move it further along that direction.


Now we can get to the actual pitching and rolling. Firstly, you need to have your world editor allow you to use negative values. You can configure this in File>Preferences>General. To actually enter a negative value, you have to hold SHIFT while clicking the field, then enter the value with a - before it to make it negative. You HAVE to hold SHIFT while going into the value.


All that negative values do is move the tip of your green line, the top of your model to the given values. They will do this normally on flat ground, if the ground is angled, it will become confusing because the original picture is tilted and all sorts of weird things happen to the result. In my experience, it usually turns them opposite.


First, we'll change the Max Pitch Angle to -0.786 - 0.25 π - 45 degrees. This will change our base model's green line to a 45 degree inclination towards the floor.


As shown, Proudmoore moves as expected. Everything is going good. You can increase this value up to 6.29 before it begins to start again. However, if you want to move him 45 degrees in the other direction, you'd have to subtract the 45 degrees / 0.25 π (0.786) from 6.29 - so therefore, this would have to be 5.50. In this way, the direction of the rotation is extremely important to know.


Next, we'll add the Roll in as well. This time, we'll move Proudmoore 90 degrees, therefore he should move partially into the floor - but not quite, because of the only 45 degree angle tilt on the Pitch. 90 degrees are 0.5 π, which makes for -1.57 as a value to put into the Max Roll Angle.


Again, he moves as desired. Since both pitch and roll have now been used, you can see how each field affects the doodad in question and how you can manipulate it to your best purposes. The best way to get familiar with all this stuff is to simply experiment. Know that it is extremely useful to know all this, both in playable terrain and art terrain.


For example - in the first picture, the bridge and vines are tilted. In the second, the walls, bridge and vines are. These are just some of the possible uses you could put this to, the only limit is your imagination.


Further exampels:


 Last edited: Jan 20, 2012

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

Beautifuck! Now start typing proper π's you inexpert pig.

You could add the numbers in decimal as well for convenience.

2 π = 360° = 6.28

1.5 π = 270° = 4.71

1 π = 180° = 3.14

0.5 π = 90° = 1.57

Otherwise great and really elaborate.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

This was explained few times already, if I remember right, long time ago, but I guess it can do.

Anyway I presented to kael some errors in his map and small part of my review was related to terrain.

Small note:

 Pitch and Roll are two extremely useful object data fields usable for all doodads and destructibles.


Click to expand...

I would like to say that it is actually useful in whole WE, but commonly used in doodads and destructibles.

You can use this to rotate dummy units and once when you attach special effect to such dummy units you will rotate effects as well, awesome really for creating some KA ME HA ME HA waves ^_^ or such like spells.

#### 2-P

#### 2-P

 Level 8

JoinedAug 31, 2005Messages254

Resources: 8

#### Resources
Maps 8

Units use degrees, not radians.


That makes your positive numbers part very confusing. You talk about units and then you make a doodad and give it the model of a unit (Proudmoore).

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

the pis look all funny and stuff, i don't think half the people would recognize them because they're just three lines. if it was the symbol from word it'd work great : (

i'll add the decimals though, good point

Crap I hadn't thought of units ahaha. I think I'll just leave it to doodads and destructibles for this terrain tutorial though, I have no idea how they work

Goddamn I was sure it was radeans (spelling wise) :<. I'll fix that up then, thanks. And yeah you're right about the units/doodads/etc parts as well, it was meant to be a doodad continuously but I thought it'd be easier to see the change in the model if it's a unit.

Thanks for the feedbacks

Edit: changes made


 Last edited: Jan 19, 2012

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

I'd like to see more examples, just to get people thinking.

For instance, in my latest terrain (the snowy one) I used various angles of Glacier and Icy Rock doodads to build cliffs and caves, and upside-down Ice Claws for stalactites (stalagmites? Fuck it).

I'd also mention the thing with that Light from Above doodad tilted on its side to make the sun, or upside-down to make light coming from the bottom of a crevasse. That's always useful.

Belgarath said:

 Goddamn I was sure it was radeans :<. I'll fix that up then, thanks. And yeah you're right about the units/doodads/etc parts as well, it was meant to be a doodad continuously but I thought it'd be easier to see the change in the model if it's a unit.e


Click to expand...

It's definitely radians for doodads. Might want to include a handy reference chart for more common angles like 30, 45, 60, 90, 120, 135, 180 etc.

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

whoops i did have in mind to put more examples up but i'd done that late last night so was a bit barren as to the imagination.

the radeans/radians thing was just the spelling by the way. i guess my calc teacher spelled it wrong all year

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

 if it was the symbol from word it'd work great : (


Click to expand...

Try some different font


Palatino Linotype; π


Comic Sans MS; π


Fixedays; π


Garamond; π


System; π

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

alright i added 2 examples i kind of have no imagination left for anything else, it'd be really bad. plus there's kobas's so it's not all bad. and i fixed the pis.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Useful. I know this has been explained in other tutorials but since this is dedicated to the particular topic and goes into a good amount of detail, I'll approve. =)

#### WaremDarkslayer

#### WaremDarkslayer

 Level 9

JoinedDec 25, 2008Messages110

Resources: 19

#### Resources
Models 10Maps 9

Nice tutorial. One thing though:


The part where you explain the angles

 2 π = 6.28 = 360


1.5 π = 4.71 = 270


1 π = 3.14 = 180


0.5 π = 1.57 = 90


0 π = 0 (==) 2 pi = 360


Click to expand...


It should be :

 2 π = 6.28 ~ 360


1.5 π = 4.71 ~ 270


1 π = 3.14 ~ 180


0.5 π = 1.57 ~ 90


0 π = 0 (==) 2 pi ~ 360


Click to expand...


With ~ meaning that the two values have the exact same result , since 3.14 doesn't


equal 180 , pi/2 doesn't equal 90,etc.


Or you could just put the values of rads and °.

#### byrnjolf

#### byrnjolf

 Level 3

JoinedAug 21, 2014Messages23

Okay but i have a question, how do you place negative numbers on max pitch angle? i try it on my editor and it just doesn't let me D:

#### Veritas 117

#### Veritas 117

 Level 14

JoinedNov 17, 2010Messages1,266

You need to hold shift when you double click the field you want change. This allows negative values to be set.

#### White987

#### White987

 Level 2

JoinedNov 8, 2020Messages8

I'm sorry for my bad English. No matter how hard I try, I can't increase its negative value.

#### AbsolutePsycho

#### AbsolutePsycho

 Level 11

JoinedDec 11, 2009Messages234

Resources: 2

#### Resources
Maps 2

@White987

Belgarath said:

 Firstly, you need to have your world editor allow you to use negative values. You can configure this in File>Preferences>General. To actually enter a negative value, you have to hold SHIFT while clicking the field, then enter the value with a - before it to make it negative. You HAVE to hold SHIFT while going into the value.


Click to expand...

 You must log in or register to reply here.


### Similar threads

 Using Reflections in Terraining!


fladdermasken

Jun 26, 2010

Misc/General Mapping Tutorials

Replies40Views50K

Jan 11, 2014
myPhoenixBlade123

Locked

 roll angle and pitch angle


jonbon29

Sep 3, 2014

World Editor Help Zone

Replies3Views1K

Sep 3, 2014
jonbon29

Locked

 cant change pitch and roll angles


PrisonLove

Oct 3, 2008

World Editor Help Zone

Replies8Views2K

Oct 3, 2008
BlinkBoy

Locked

 max pitch angle


raid1000

Jun 30, 2008

World Editor Help Zone

Replies4Views2K

Jun 30, 2008
Huurka

Locked

[General]
 Calculation of Yaw, Roll, Pitch and Space Rotation for Effects


maxodors

May 16, 2021

World Editor Help Zone

Replies4Views3K

May 23, 2021
maxodors

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