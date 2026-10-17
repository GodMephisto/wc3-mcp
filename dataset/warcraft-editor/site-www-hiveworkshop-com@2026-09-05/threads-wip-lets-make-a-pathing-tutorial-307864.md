---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/wip-lets-make-a-pathing-tutorial.307864/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Wip Lets Make a Pathing Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/wip-lets-make-a-pathing-tutorial.307864/>
#### Champara Bros

#### Champara Bros

 Level 21

JoinedAug 29, 2008Messages325

Resources: 51

#### Resources
Models 44Icons 2Packs 1Ported Icons 1Tutorials 3

First of all, a message to the moderators. I request that this tutorial be not graveyarded or removed as I intend it to be a community project that will shed some light on the mistery of pathing.

Introduction:

Unit pathing has been an enigma for many novice WarCraft III enthusiasts from their early modding beginnings. With structures it is relatively simple. There is an amazing and detailed tutorial on building and terrain pathing written by

Dr. Boom Pathing - Everything about it. However, even this detailed and excellent tutorial does not cover a question asked by many. What about non structure units? If you are any bit like me, you had an experience where you doubled a scale of some model and then other units started walking all over it. You tried changing the Unit collision size but it had no effect. This tutorial is meant for all bees of the Hive. To contribute with their own ideas knowledge and experiences to make this tutorial complete.

Goals:

1. Analyzing properties and components of Pathing - Collision Size.

2. Determining how does Art - Model File and Art - Scaling Value impact the pathing.

3. Explaining Gameplay Constants - Collision Maximum Radius.

So lets begin:

Say you want to build a barrier wall. The wall is set at a distance from a caster and faces the enemies. So you could use a building with pathing, like in tutorial by Doctor Boom. Or you could even use destructible pathing blockers that Doctor Boom covered as well. However this method does not allow for rotation of pathing blocking. So you would want to use a unit. But how? I noticed in the Blood Elf campaign, during wagon escort that units obey pathing of wagons. So after a LOT of experimenting I determined that there are three things that impact Unit Collision. Pathing - Collision Size, Art - Model File and Art - Scaling Value model. So as far as I gather, Pathing - Collision Size is like a circular field of potential collision, that is not in effect. The way it gets in effect is when it encounters a unit geoset (then the collision shape/pathing gets the same shape as the geoset), NOT Collision Object as many assume. Collision Object is only there to make unit selectable. So if you scale the model and not Pathing - Collision Size, unit will be bigger, but other units and doodads WILL phase through its periphery. So the solution is to change both Unit Scale and Pathing. Also if you are not using same model you need to make sure that units model in game is not bigger than Collision Size (which is an area) The best way to check it is by using an area ability with preset area to your collision size. When you do all this, unit should be tangible. But there is still one issue. If you open Gameplay Constants - Collision Maximum Radius, you will see that it is preset to 200. Also this is the maximum collision size for Warcraft III. I noticed that changing the value in Gameplay Constants does not affect this pathing at all. So there you have it. You can choose unit pathing as long as it is less than 200 and as long as you have unit geoset(s) that cover the area.

Conclusions:

1. Pathing - Collision Size: Creates a field of potential collision that only materializes in the shape and presence of present unit geosets, NOT collision shapes, which unlike their name only affect selection. However if the collision shape is bigger than the area that model covers the pathing will be square shaped.

2. Art - Model File and Art - Scaling Value both can change the size of geosets that create the unit pathing.

3. Gameplay Constants - Collision Maximum Radius can only tell you what is Warcraft III maximum Collision Size, and changing it does not do a thing.

Thank you in advance for your contribution and comments. Also I would like to thank Dr. Boom for making such an amazing pathing tutorial. If this gets approved of I am ready to add pictures and everything else.


 Last edited: Aug 13, 2018

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Add pictures and everything else first I suppose. Can't approve a tutorial with missing pieces now, can we?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Although I would not be against a community-driven collaboration of sorts, not enough interest was shown and the author is currently inactive so it cannot realistically be continued even if more attention is received in the future.

Graveyarded for now, might be open for resurrection if the author returns and brings some life into this.

 You must log in or register to reply here.


### Similar threads

Locked

 Pathing to Collision Size


lelyanra

Apr 25, 2010

World Editor Help Zone

Replies5Views2K

Apr 26, 2010
Levadam

Locked

[General]
 Unit Collision Size Problems


Skadrel

Nov 26, 2016

World Editor Help Zone

Replies6Views5K

Nov 28, 2016
Skadrel

Locked

 Collision size


nerdcafe

Oct 27, 2018

World Editor Help Zone

Replies5Views4K

Oct 29, 2018
Dr Super Good

Locked

 Map is ignoring Pathing - Collision Size


WorldMapper

Aug 7, 2016

World Editor Help Zone

Replies9Views3K

Aug 9, 2016
Gumberbunbder

Locked

 Manipulating building's pathing map {pathing map and collision}


toofless

Mar 27, 2009

World Editor Help Zone

Replies4Views2K

Mar 27, 2009
toofless

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top