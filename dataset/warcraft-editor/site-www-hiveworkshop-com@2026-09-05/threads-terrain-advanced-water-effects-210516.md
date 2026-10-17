---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/terrain-advanced-water-effects.210516/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Terrain Advanced Water Effects
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/terrain-advanced-water-effects.210516/>
#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

 Terrain Tutorial by -Kobas-


Advanced Water Effects

by -Kobas-

Requirements

* Basic knowledge of the World Editor

* Basic terrraining knowledge

* A moment of your time  (Around 10-15 min).

Introduction

How many times you wanted to improve blizzard water with so many great custom water doodads? But honestly how many times you failed.

Huge number of guys just like you asked same question so many times, that's why I want to present this tutorial to you.

Idea will be to use both Object editor and World editor tricks to avoid 2 main problems:

- Z offset

- Water overlapping

Also I will go step forward and show you unique terrain technique that will (I hope so) impress you, and force you to forget blizzard water from this point.

I hope that this sound interesting so far, so let's continue.

Let's start

Okay, let's start with terrain in general first!

You must know some basic things, how to move doodads around map, resize, rescale, import doodads, change Z offset in WE and so on (I request basic terrain skill above in text).

If you already know all this import this resource into your map:

http://www.wc3c.net/showthread.php?t=82133

This path will do: war3mapImported\WaterPlane.MDX

This model is essential for this tutorial, just like some spells won't work without dummy units, we can't work without it.

In game it look like this:

I like to use destructible instead of doodads, you can change destructibles with triggers and do all kind of crazy things with it so I suggest destructibles.

Here I based our model on "Foot Switch":

As you can see I changed a lot of things, removed pathing ground, renamed it, removed shadow, set fixed rotation to -1 so we can rotate it in any direction, you can edit selection size, model size and so on.

When you finish this basic setup let's create some basic terrain shape:

Yes we will need blizzard water, I will explain why later

Now check pathing ground, this is really important thing in playable terrain and I would like to see a lot of them after this (Press P or go to view and enable Pathing Ground):

Not bad.

As you can see in my WE water has no pathing ground generated manually (Jass New Gen), this is both good and bad, depend on terraining you need/want.

Won't bother you with details, there are other tutorials about this, let's keep going.

Now check water riverbed (Press W or go to view and disable Water):

Look ok, can do, but let's add some rocks and grass etc:

Ok we are ready now to do magic.

First of all let me show you some basic errors that forced many to quit (I mentioned them above):

### Z offset

See what happen when you place 2 water object next to each other and try to balance their height.

It's impossible.

Or maybe not:

It is, trust me, just change angle:

Really annoying, but do not worry we will fix this.

But take a closer look at this picture you will see that 2 models overlapping each other and because of model features that part or river become darker.

This is our 2nd most common problem.

### Overlapping

Again, no mater how many times you try you won't be able to fix this just like that:

So time has come for me to show you tricks and hints how to fix this, improve your skills, and go even beyond in pwning and creating awesome water effects in your projects.

### Fixing Problems

Go to object editor and change object path to some tower, lumber mil or whatever:

Can you see how nice it look like right now

I was in hurry so I went 1 step further and placed them all on same ground level (Z offset).

Lines are maybe a little "jagged" because of camera angle but terrain must be same here.

Place few additional water objects to make sure it will cover our terrain water.

Now be sure that you enabled option in WE to move objects with keeping their Z offset (Disable: Advanced -> Reset fixed object height).

Increase water just a little to force WE to remember their Z offset.

Go to Object Editor and remove pathing ground.

It should look like giant model, awesome right.

Select everything and move it over our terrain.

DO NOT COPY AND PASTE THEM YOU WILL FORCE WE TO RESET Z OFFSET!

It should look like this:

Now rise it or lower it to make it same height like blizzard water.

And that's it, you successfully fixed 2 main problems, good job.

But question remains why we used blizzard water at first place?

Now really magic, go to trigger editor and do this both in guy or jass.

Initialization
Events
Map initialization

Conditions

Actions
Environment - Change water tinting color to (100.00%, 100.00%, 100.00%) with 100.00% transparency

 JASS:

library Water initializer onInit

 private function onInit takes nothing returns nothing
 call SetWaterBaseColor(255, 255, 255, 0)
 endfunction

endlibrary

We will make water 100% transparent, we will disable it.

But waves will remain as well as all water effect such as footprint generation when unit step on it (check next picture).

###
 Finally just few questions left unanswered

Why we use 100 doodads instead of single 1?

Well try and see what happen, water waves, color and everything else will look horrible.

Is there any way to do this, what if we don't have space to place large number of this water doodads and then move it over our terrain.

Well go to map options and try to resize map, shrink it again after you finish.

Honestly any other way?

At map start when you decide on what height will you use blizzard water, place deep water over all map, then place doodad over all map, don't forget to increase it's height (Z offset) just a little, so terrain don't reset it later.

Then just use rise or lower tool without fear.

Oh 1 cool trick that is worth mentioning.

Changing model Z scale, you will be able to make it look more denser/transparent.

And that's will be everything folks.

Author's notes:

I hope that you enjoyed as much as I did making this.

You are free to post suggestions and questions in comment below or sending me message.

I own no credits to anyone for this, I figured this on my own, so please don't attack me or something.

Oh 1 last thing, I wish you happy terraining.

#### Attachments

Image 01.JPG

 99.5 KB

 · Views: 4,515



Image 02.JPG

 108.5 KB

 · Views: 4,527



Image 03.JPG

 380.3 KB

 · Views: 4,589



Image 04.JPG

 317 KB

 · Views: 4,496



Image 05.JPG

 369.7 KB

 · Views: 4,526



Image 10.JPG

 69.6 KB

 · Views: 4,243



Image 09.JPG

 88.2 KB

 · Views: 4,336



Image 08.JPG

 90.7 KB

 · Views: 4,279



Image 07.JPG

 95.3 KB

 · Views: 4,345



Image 06.JPG

 373.5 KB

 · Views: 4,456



Image 15.JPG

 142.4 KB

 · Views: 4,204



Image 14.JPG

 57.5 KB

 · Views: 4,179



Image 13.JPG

 95.1 KB

 · Views: 4,192



Image 12.JPG

 233.5 KB

 · Views: 4,277



Image 11.JPG

 136.6 KB

 · Views: 4,293



Image 16.JPG

 250.9 KB

 · Views: 4,566



Image 17.JPG

 287.5 KB

 · Views: 4,379



Image 18.jpg

 490.6 KB

 · Views: 4,854




 Last edited: Jan 18, 2012

#### Misha

#### Misha

 Level 49

JoinedJun 9, 2008Messages16,523

Resources: 73

#### Resources
Models 64Icons 1Packs 2Skins 4StarCraft II Resources 2

why.. that can be useful to make something like a muddy river flowing into sea

#### iAye

#### iAye

 Level 4

JoinedMay 22, 2011Messages68

Resources: 1

#### Resources
Maps 1

Nice guide

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

I have new tricks and hints related to this model... I will see can I update this tutorial or should I create new one.

Because only same thing is model...

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Cool trick. It is nice for those who don't want to spam a crapload of pathing blockers on the edges just to simulate water pathing.

I'm going to approve this. If you want to add to it, go ahead and I'll review it again.  However, I think it is approvable in the state that it is currently in.

#### unk

#### unk

 Level 5

JoinedMar 19, 2009Messages31

Resources: 2

#### Resources
Spells 2

well ... all i can say ... it is no good or i might made smth wrong .... when i'm in game in wc3 ... and if the water is in fog ... i pass in it and then ... i see square's of water

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

unk said:

 well ... all i can say ... it is no good or i might made smth wrong .... when i'm in game in wc3 ... and if the water is in fog ... i pass in it and then ... i see square's of water

Click to expand...


Because model fall into fog, it become dark and animations are stopped.


re-size water into small squares, it should fix problem (more or less)

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

I think there's a circular waterplane model. Should be burried somewhere beneath thick layers of forum matter over at WC3C.

#### Hell_Raider

#### Hell_Raider

 Level 7

JoinedJul 25, 2008Messages292

Resources: 1

#### Resources
Maps 1

Possibly a stupid question, but before I try making this in my map, is the water actually usable as water (for instance, can ships sail it?)

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Yes, because you will use regular wc3 water as well.

#### Animalpower

#### Animalpower

 Level 12

JoinedJun 1, 2010Messages745

Resources: 3

#### Resources
Maps 3

Hell_Raider said:

 Possibly a stupid question, but before I try making this in my map, is the water actually usable as water (for instance, can ships sail it?)


Click to expand...


Of course not, lol. A model can not make a ship sail.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Animalpower said:

 Of course not, lol. A model can not make a ship sail.


Click to expand...

It can omg, we just make water transparent ^_^.

#### Animalpower

#### Animalpower

 Level 12

JoinedJun 1, 2010Messages745

Resources: 3

#### Resources
Maps 3

-Kobas- said:

 It can omg, we just make water transparent ^_^.


Click to expand...


Yes, but a model can not do it. Though I like the idea of transparent water, I do not think it is realistic. If you see a water surface, only the shore is transparent.

#### blink

#### blink

 Level 10

JoinedMar 6, 2012Messages64

Resources: 3

#### Resources
Models 3

nice because of you Ive learned about the fog radius

#### Pinzu

#### Pinzu

 Level 15

JoinedNov 30, 2007Messages1,202

Resources: 7

#### Resources
Spells and Systems 5JASS 1Tutorials 1

Nice tutorial. Maybe you should mention in the tutorial that you can have multiple colors on the lakes and normal water only allows for one? (Haven't tried it though.)

#### lord_Turin

#### lord_Turin

 Level 11

JoinedOct 30, 2004Messages338

Resources: 1

#### Resources
Maps 1

I"ve got an addition to this guide, a simple thing.

You CAN actualy copy and past the water together, if you press Y all the selected doodads will take an avarage of the Z offset. So everything will be on the same height, and you can now lower it in the the correct height.

This is a very usefull tool for maps with alot of height diffrence where you dont have a large field to prepare the water. Can also be used for all the other doodads / destructables!

#### Lt_Hawkeye

#### Lt_Hawkeye

 Level 12

JoinedJul 4, 2016Messages636

The water plane link no longer exists, is there another location where one can acquire it?

#### Mekket

#### Mekket

 Level 4

JoinedSep 9, 2010Messages12

Lt_Hawkeye said:

 The water plane link no longer exists, is there another location where one can acquire it?


Click to expand...

I know I'm too late but here is the model for future reference since the link in the tutorial no longer exists.

#### Attachments

WaterPlane.MDX

 11 KB

 · Views: 129



 You must log in or register to reply here.


### Similar threads

 How To Create Dynamic Water Levels


Pinzu

Sep 26, 2012

Misc/General Mapping Tutorials

Replies24Views24K

Nov 26, 2023
forelmashi

 Advanced Terraining Tutorial


Blood Raven

Sep 9, 2011

Misc/General Mapping Tutorials
2

Replies55Views83K

Oct 1, 2020
Blood Raven

 The Ultimate Beginners Terrain Tutorial


HermanTheGibbon

Jun 12, 2007

Misc/General Mapping Tutorials

Replies48Views68K

Nov 21, 2021
Verdun

 Underwater effect for maps


olofmoleman

Dec 9, 2006

Misc/General Mapping Tutorials

Replies5Views39K

Sep 28, 2015
BETABABY

 How to Make Easy Waterfalls


RoboHippo

Nov 10, 2009

Misc/General Mapping Tutorials

Replies13Views20K

Jul 18, 2015
The Nightraven

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