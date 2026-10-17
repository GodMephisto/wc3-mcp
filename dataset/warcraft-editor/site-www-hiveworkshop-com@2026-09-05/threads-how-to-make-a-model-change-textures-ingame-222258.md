---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-make-a-model-change-textures-ingame.222258/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Make a Model Change Textures Ingame
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-make-a-model-change-textures-ingame.222258/>
#### Ender

#### Ender

 Level 7

JoinedJan 28, 2012Messages266

Resources: 3

#### Resources
Spells 2Tutorials 1

1.0 Modeling

1.1 Overview

1.2 Importing Texture

1.3 Editing Materials​
2.0 Using this in your map

2.1 Making the ability

2.2 Making the Destructables

2.3 triggering​
3.0 Conclusion

3.1 TestMap​

1.0 Modeling

to do this I used Magos's Model Editor​

1.1 Overview

all you are going to do is add an alpha tile texture to your model, set its replaceable id to 31 then add as a material layer on top of materials that you want to change their texture, and making sure to set filter type to transparent

if this is enough information for you, edit a model and head onto 2.0 if not keep reading

Go To Index​

1.2 Walkthrough

First off you will want to go to the texture manager window>Texture Manager

next import a alpha tile into the texture manager: RightClick>Import

i am using the alpha tile from De.Factos Tutorial

then edit the new texture: Right Click it>Edit

Then set its replacable id to 31

Go To Index​

1.3 Editing Materials

First Go to the Material Manager: Window>Material Manager

then right click the first material edit it and then look at its material layers,if it is the main texture of the model

Create a new material: Right click> Create New

set its texture to your texture

set its filter mode to transparent, if you look at the model, when your done, and it is completely black you forgot to set its filter mode to transparent

Go To Index​

2.0 Using This in your map

Have any of you evr noticed the mountain giants warclub ability? and how depending on what tree He picks up, he has a different looking club? This will require one ability + however many textures you intend on using.​
2.1 Making the Ability

first off, in the object editor create a new ability based on warclub, name it Texture Changer

then select it and set Data- maximum attacks to 0

then select disable attacks, press shift enter and set it to 3

do the same thing for Data - Enable Attack and then set the other purple data to the values i have set them(this is to make it almost instant)​
2.2 Making the Destructables

First off you are going to create a new destructible, base it off of a tree

Next set its Fly height to 0.00 this is to prevent it from modifying flying units heights for a brief instant.

set its replaceable texture to your textures path,

set its pathing to none this is to prevent it from pushing units around

Go To Index​

 2.3 Triggering it

Look at at which ever explanation you want, what you'll be doing is creating a destructible, adding Texture Changer(the ability you made early) to the unit, ordering it to cast, removing Texture Changer, and then waiting and removing the destructible(to prevent a build up of destructible's.

 JASS:

function change texture takes unit u, integer dtype returns nothing
 local real a = GetUnitFacing(u)*bj_DEGTORAD//What i am doing here is converting the units facing angle into radians, though why blizzard doesn't have them use a radians is beyond me
 local destructable d = CreateDestructible(dtype,GetUnitX(u) + 12*Cos(a),GetUnitY(u) + 12*Sin(a),0,.1,1)
 //this just creates a destructable in front of the unit, to make so
 // that the unit can insta cast
 // IMPORTANT remembber to make sure all destructables that you are
 // using have no pathing, otherwise they will move the unit backwards.

 call UnitAddAbility(u,TEXTURE_CHANGER)
 // adding the texture changing ability to the unit, WARNING
 // this could conflict with a unit that has war club, if this could be a
 // concern add an if here that checks if the unit has the ability warclub
 call IssueTargetOrder(u,"warclub",d)
 //a better way to do this would be using IssueTargetOrderById,
 // but i am to lazy to look up the order id:)
 call UnitRemoveAbility(u,TEXTURE_CHANGER)
 //what this does is cause warcraft 3 to instantly cast this spell
 // i wish i had a long complicated reason to give you, but i don't.
 call RemoveDestructable(d)
 // originally I put a wait between this and the remove ability but it isn't needed
endfunction

Actions
Set TempPoint = (Position of (Triggering unit))

-------- ---------- ------------ --------- --------

-------- Setting the position of the unit to a --------

-------- point in order to prevent leaks --------

-------- using xy coords is better but that --------

-------- requires some jass knowledge --------

-------- ---------- ------------ --------- --------

Set TempPoint2 = (TempPoint offset by 12.00 towards (Facing of (Triggering unit)) degrees)

-------- ---------- ------------ --------- --------

-------- same as above, but this time we are --------

-------- putting the point in front of the unit, --------

-------- so it doesn't have to turn to cast --------

-------- ---------- ------------ --------- --------

Destructible - Create a Alpha at TempPoint facing 0.00 with scale 0.01 and variation 0

-------- ---------- ------------ --------- --------

-------- Creating a destructable, remember --------

-------- make sure the destructable you are --------

-------- using does not have any pathing --------

-------- ---------- ------------ --------- --------

Unit - Add TextureChanger to (Triggering unit)

-------- ---------- ------------ --------- --------

-------- Add the ability to the unit, so it can cast --------

-------- it --------

-------- ---------- ------------ --------- --------

Unit - Order (Triggering unit) to Night Elf Mountain Giant - War Club (Last created destructible)

-------- ---------- ------------ --------- --------

-------- pretty basic, just ordering the unit to --------

-------- cast the ability WARNING this can --------

-------- conflict with the warclub ability --------

-------- so i recomend that you don't use --------

-------- units that have the warclub ability --------

-------- ---------- ------------ --------- --------

Unit - Remove TextureChanger from (Triggering unit)

-------- ---------- ------------ --------- --------

-------- removing an ability after you order a unit --------

-------- to cast it causes the unit to instant --------

-------- cast that ability, provided that the --------

-------- unit doesn't have to turn to face the --------

-------- target --------

-------- ---------- ------------ --------- --------

Destructible - Remove (Last created destructible)

-------- ---------- ------------ --------- --------

-------- Removing the destructible to prevent --------

-------- a build up of dead destructables --------

-------- ---------- ------------ --------- --------

Custom script: call RemoveLocation(udg_TempPoint)

Custom script: call RemoveLocation(udg_TempPoint2)

Custom script: set udg_TempPoint = null

Custom script: set udg_TempPoint2 = null

-------- ---------- ------------ --------- --------

-------- Removing location leaks --------

-------- ---------- ------------ --------- --------

Custom script: set bj_lastCreatedDestructable = null

-------- ---------- ------------ --------- --------

-------- this is to prevent the destructable from --------

-------- creating a minute leak which all --------

-------- Handles will do if you don't null --------

-------- variables that you set to them --------

-------- ---------- ------------ --------- --------

you will also need to use a method to make units who enter the map change their texture to the alpha tile(I use bribes unit indexer), because for some reason the units appear white(anyone who figures out a solution for this problem please tell me)

Go To Index​

3.0 Conclusion

I hoped you learned from this tutorial, please comment and give suggestions for improvment

Go To Index​

 3.1 View attachment Complicated Texture Switcher.w3x

Go To Index​

#### Attachments

Texture Manager.jpg

 16.4 KB

 · Views: 5,046



Material Manager.jpg

 16.3 KB

 · Views: 4,884



EditTexture2.jpg

 26.6 KB

 · Views: 4,922



EditTexture.jpg

 5 KB

 · Views: 4,853



CreateNew Material Layer.jpg

 4 KB

 · Views: 4,807



MaterialEditLoksLike.jpg

 24.8 KB

 · Views: 4,913



Selecting Filter Mode.jpg

 5.2 KB

 · Views: 4,831



Selecting texture.jpg

 39 KB

 · Views: 4,926



Object Editor WarClub.jpg

 42.8 KB

 · Views: 4,953



ObjectEditor DeisableAttack.jpg

 65.6 KB

 · Views: 4,849



ObjectEditor Max Attacks.jpg

 64.6 KB

 · Views: 4,836



Destruct Make Your Destructable.jpg

 38.7 KB

 · Views: 4,849



Destruct FlyHeight.jpg

 83.1 KB

 · Views: 4,914



Destruct Pathing.jpg

 128.1 KB

 · Views: 4,846



Destruct Replace Texture.jpg

 136.9 KB

 · Views: 5,029




 Last edited by a moderator: Jul 4, 2016

#### Ender

#### Ender

 Level 7

JoinedJan 28, 2012Messages266

Resources: 3

#### Resources
Spells 2Tutorials 1

Please review and post suggestions for improvements, thank you.

if you find this useful and make use of it in your map please credit me.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Very nice guide! This is wonderful for anyone interested in some cool hero systems or a DGUI like system.

Approved.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

no comment?

Is this a safe way to change texture in-game? bcs I want to implement this to FSI so user could change inventory theme anytime and to evade uselessly recreating destructables for icons. By using unit as UI and button I can also initialy preload them and add SetInventoryPosition functionality maybe..

a very useful tutorial.. thanks..

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Dalvengyr said:

 no comment?


Is this a safe way to change texture in-game? bcs I want to implement this to FSI so user could change inventory theme anytime and to evade uselessly recreating destructables for icons. By using unit as UI and button I can also initialy preload them and add SetInventoryPosition functionality maybe..


a very useful tutorial.. thanks..


Click to expand...


It is indeed a safe way, but you still have to make object data for the different textures.


This is useful for dynamically changing unit textures, though.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

 It is indeed a safe way, but you still have to make object data for the different textures.


Click to expand...

yup, that's right.. but we got many advantages by only changing the texture, especially for dragging feature  and also we can preload the button units first..

so is this still working if the unit is building? I want to set the button unit to a building so facing angle is not changed when casting the spell..

I got a basic question, is that building/structure unit has collision after we add locust to it?

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

Here is a correction to the jass code:

 JASS:

function change_texture takes unit u, integer dtype returns nothing
 local real a = GetUnitFacing(u)*bj_DEGTORAD//What i am doing here is converting the units facing angle into radians, though why blizzard doesn't have them use a radians is beyond me
 local destructable d = CreateDestructible(dtype,GetUnitX(u) + 12*Cos(a),GetUnitY(u) + 12*Sin(a),0,.1,1)
 //this just creates a destructable in front of the unit, to make so
 // that the unit can insta cast
 // IMPORTANT remembber to make sure all destructables that you are
 // using have no pathing, otherwise they will move the unit backwards.

 call UnitAddAbility(u,TEXTURE_CHANGER)
 // adding the texture changing ability to the unit, WARNING
 // this could conflict with a unit that has war club, if this could be a
 // concern add an if here that checks if the unit has the ability warclub
 call IssueTargetOrderById(whichUnit, 852511, d)
 //a better way to do this would be using IssueTargetOrderById,
 // but i am to lazy to look up the order id:)
 call UnitRemoveAbility(u,TEXTURE_CHANGER)
 //what this does is cause warcraft 3 to instantly cast this spell
 // i wish i had a long complicated reason to give you, but i don't.
 call RemoveDestructable(d)
 // originally I put a wait between this and the remove ability but it isn't needed
 set d = null
endfunction

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

nvm I am stupid


 Last edited: Dec 18, 2015

#### maddeem

#### maddeem

 Level 24

JoinedJan 1, 2011Messages1,506

Resources: 34

#### Resources
Models 5Icons 3Skins 1Maps 6Spells and Systems 18Lua 1

Bit of a necro but it is important. You can create the texture changing destructables on init and move units directly on top of them and cast the ability. Then you revive the destructable and you effectively have infinite uses from the texture changing destructables. This is (probably) faster than creating a destructable with polar projection (which also has issues for units out of bounds). I also read somewhere that creating and removing destructables causes leaks over time so this also helps with that.

 JASS:

 function SetUnitTexture takes unit u, destructable d returns nothing
 local real x = GetUnitX(u)
 local real y = GetUnitY(u)
 call SetUnitX(u,0)
 call SetUnitY(u,0)
 call DestructableRestoreLife(d,5,false)
 call UnitAddAbility(u,Texture_Abil)
 call IssueTargetOrder(u,"grabtree",d)
 call UnitRemoveAbility(u,Texture_Abil)
 call SetUnitX(u,x)
 call SetUnitY(u,y)
 endfunction

 You must log in or register to reply here.


### Similar threads

 How To Change Textures on a Tree or other Doodad (Without importing several models)


InfernalTater

Sep 3, 2012

Misc/General Mapping Tutorials

Replies6Views19K

Mar 11, 2024
fostaa

Locked

 Texture swapping via warclub - any ideas how to make 2 swappable textures?


Zwiebelchen

Dec 12, 2014

Advanced Scripting

Replies0Views1K

Dec 12, 2014
Zwiebelchen

Locked

[Trigger]
 How to make it MUI?


iamdl

Aug 11, 2014

Triggers & Scripts

Replies5Views2K

Aug 13, 2014
Ceday

Locked

[Trigger]
 How to make this MUI...


Exavier00

Aug 3, 2011

Triggers & Scripts

Replies2Views2K

Aug 3, 2011
Jay the Editor

 How to create buildable and viable walls, gates and switchable lantern posts


Alok

Jul 19, 2016

Misc/General Mapping Tutorials

Replies6Views6K

Nov 1, 2016
Chaosy

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