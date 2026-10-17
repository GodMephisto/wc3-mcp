---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/a-beginners-guide-to-map-making.8204/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# A Beginners Guide to Map Making
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/a-beginners-guide-to-map-making.8204/>
#### oz02

#### oz02

 Level 13

JoinedJan 9, 2004Messages1,037

Resources: 6

#### Resources
Tools 1Tutorials 5

A Beginner's Guide to Map Making


For Windows: images captured in Windows XP with Paint Shop Pro 8.

Tools Needed


Warcraft III - The Frozen Throne

Introduction


This tutorial will teach a beginning map maker how to create a simple melee map from scratch. You will learn how to form terrain, place neutral units and doodads, and make a description of your map to make a high quality melee map.

The Process

You begin by opening the Warcraft III World Editor, located in the Warcraft III directory on your computer (C:\Program Files\Warcraft III\). If this is your first time opening the World Editor, or you have never changed the preferences of the editor, a new map will be created on startup. Close this map by clicking File->Close Map or by pressing Ctrl+W.

The next step is to create a new map. Do this by clicking File->New Map..., pressing Ctrl+N, or by clicking the  button on the toolbar.

There are a few options you must understand to make a new map. First look under Map Size:. The width and height are defined by the amount of grid boxes horizontally and vertically. So, if it's 64x64 there are 64 grid boxes across and 64 grid boxes down. Leave the size to 64x64 for this map. The next thing you will see is Playable Area:. This tells you how much area of the map the player can 'play with' and how much area you have to edit. It's the same thing for Size Description:. This just tells you relatively what size you or the player can expect. Next, look at the Tileset: part. This gives you a list of available tilesets to choose from. What a tileset is, is a type of area with it's own unique ground textures, lighting, and sounds. So, for example, the Lordaeron Summer tileset has the dirt, rough dirt, grassy dirt, rock, grass, and dark grass ground textures, a brighter lighting to it, and a summer sound set. Leave the tileset as Lordaeron Summer for this map. If you look under the tileset box, you can see Initial Tile: setting. Choose grass for this part. Now look to the right. You should see the Initial Cliff Level: box. This chooses what level of elevation your map will be at. Leave it at 2 for this map. Next you will see Initial Water Level:. This is asking you if you want your map to have no water at all, be filled with shallow water, or be completely covered with water. Leave it at no water. Finally, you will see the Random height field check box. This is asking you if your map should be bumpy or not. Leave it unchecked for this map.


Now your settings should be like this:


Click OK.

Congratulations! You have just created your first map, and are on your way to becoming a great map maker!


Now you'll want to begin by molding that terrain to create mountains and valleys. Open the Terrain Palette by clicking Window->New Palette->Terrain or by pressing T.


Now we'll create a dirt path. Click the Apply Texture: check box and select the dirt tile. Move your mouse over the main window and click to make a path going from the bottom left corner of the screen to the top right corner of the screen.

TIP: You can close the Terrain Palette if it's blocking your view, and just reopen it after.


TIP: You can change your brush size and shape on the bottom of the Terrain Palette.


Once you are finished experimenting by changing the tiles, we'll make a cliff in the middle of the road. Open the Terrain Palette if it's not already open, and click the check box Apply Cliff: and click the Increase One option, and select the Dirt Cliff option. Now, move your mouse over the road and click until you feel that the cliff is the right size.


Now we'll make a small river run along the road. Select the Deep Water option in the Apply Cliff: box. Once again, run your mouse on the side of the road and get clicking.


Finally, let's make a hill on the other side of the road. Select Raise in the Apply Height: box and click and hold on the side of the road until you like the height.


Experiment with all these tools to get the feeling for forming terrain.

Now that you are used to the Terrain Editor, it's time to begin applying doodads to your map. A doodad is an object used for decoration on your map to make it look more interesting. You can open the Doodad Palette by clicking Window->New Palette->Doodads or by pressing D.

TIP: You can always switch palettes by clicking the box on the top of your current palette and selecting the palette you want from the pull-down menu.

If you look at the top of the Doodad Palette, you will see that you have access to a number of different categories in a number of different tilesets. So if you can't find the doodad you want, chances are that it is listed under a different category or an entirely different tileset.


For now, let's try placing trees next to the river. Make sure you are under the Trees/Destructibles category and select Summer Tree Wall. Move your mouse over the map and start clicking.

TIP: You can click and hold while moving your mouse to continue on in a continuous line.


If you want, you can change the properties of the doodad by clicking the  button on the toolbar and double-clicking the doodad, or clicking the doodad and pressing Enter.

TIP: You can select more than one doodad by clicking anywhere and dragging the box to surround the doodads you want selected.


Now try placing a barren in the middle of the road by yourself.


Let's say you want to make it that when the player breaks the barrel, he gets an item. Open the Barrel's properties and click the Items Dropped tab.


Under Items Dropped On Death Select Use Custom Item Table. Then click New Set and click New Item.


Select the Specific radio button and choose the item you want from the list. Click OK and then OK. Your barrel should now have a white thing floating over it. That shows that the barrel drops an item upon 'death'.

Now we'll learn how to set a player's starting point and how to place neutral units.


Open the Unit palette by clicking Window->New Palette->Units or by pressing U.


Select Start Location and place it on the side of the road.


You'll also want to make a gold mine, so click Neutral Passive (which means units that are on no one's team), select Gold Mine, and place it on the map. Make sure that it's not too close to the start location or the town hall wont be built.


Now to place Neutral Hostile units. Select Neutral Hostile at the top and select Red Dragon Whelp and place it on the other side of the river. Make sure you don't place it too close or the player will be killed before he has a chance to defend himself.

Now for the final step. Click Scenario->Map Description. You can fill in these fields however you like.

You may want to test your map. Do this by clicking File->Test Map, by clicking the  button on the toolbar or by clicking Ctrl+F9.

End Note


Congratulations! You now have the ability to create a high quality melee map. Don't be afraid to experiment with all the different controls until you are used to them!


 Last edited by a moderator: Mar 15, 2007

#### ent_kill_98

#### ent_kill_98

 Level 4

JoinedJun 12, 2005Messages14

Resources: 3

#### Resources
Maps 3

ent_kill`s Beginner Map-Making Tutorial v1.0

Ent_ Kill`s Beginner Map Making Tutorial!


---------------------------------


Ok, If it`s your first time making a map you shouldnt start with an AOS. But an RPG would be a good choice.

Terraining:


First we need to discuss terraining =)


Terrain is simple, cliffs are easy to produce. Go to the main window then click "New Pallete" Go to "Terrain Pallete". In the middle will be the "apply cliffs" section.


there will be a number of choices of cliffs to choose from. First you must choose the base cliff at the bottom left.


There are 2 base cliffs to choose from,this depends on the tileset you pick.


after you`ve placed your cliffs then we can go to the next step. =)

Water Terraining:


This also depends on what tileset you choose. you also need to set the base cliff to depend on what the "surroundings" of the water will be.


you can select from a number of sizes,Just like in cliffs.


Water is the most simplest step,next to cliffs.


The Terrain for your map should be A-OK now. =)


Now we`ll discuss doodads.

Doodads:


Doodads will make any map look nicer.


There are different types of doodads such as cliffs,enviromental,and others.


Well there`s not really anything difficult about doodads..making doodads is fun, but this is a tutorial for begginers.


Units are next!

Units:


Units are easy... if your not creating them, but let`s go to the basics.


go to window,and go to Unit pallete.


Here you can now select from a number of different races


such as Human, Orc, Undead, Night Elf, and Neutral (TFT players)


Here also you can choose from melee units, campaign units or your own custom ones.


Just select them and place them in the desired places.


Now on to regions!

Regions:


Regions are simple, they are bases for most triggers. You can place regions by clicking the box at the top right and highlighting the desired area.


Cameras are next >=)

Cameras:


Cameras aren`t arent needed, unless your game is a cinematic or you


teleport a unit.

Destructibles:


Destructibles are very useful in many aspects of a good map.


Trees make a map look nicer, but too many will make it look horrocious.


Gates are also destructibles, they are useful in many areas also.You can also make trigger enhanced destructibles. This is like...If you destroy a unit, a gate opens. That`s one of the best ways to impress a person playing one of your maps. Heres How:

Events: (Specific Unit Event) A (Unit) Dies

Conditions: None, unless wanted

Actions (Destructibles-Open/Close/Destroy Gates) Open(Gate)
Destructables is an easy aspect and this is all you need to know about begginer map-Destructables.

Custom Spells:


For a beginner, custom spells aren`t that important. If you`re a beginner, stick to changing names, effects, and damage of already made spells. If you are not a beginner, trigger-enhanced spells are the way to go =).

Triggers:


Triggers are a beginning map makers nightmare. If you are a beginner, you`d probably say "OMFG WTF IS THAT?!" looking at a trigger from current RPGs hosted on B.net. Triggers are still hard even if you`re not a beginner, but they become easier every time you make one. Here are some recently used triggers featured in alot of maps:

Region Triggers:


This is when you step on or enter a certain region and something happens. Teleporting is commonly used by doing this. Heres an example:

Events: (Unit enter region) a unit enters (region)

Conditions: ( Mostly this is a Unit Type Conditon, we`ll talk about those later.)

Actions: (Unit-Move Unit Instantly) move (triggering unit) instantly to (region).
Done with that.

Hero Selections:


This is probably the most commonly used trigger releated thing in RPG maps/arenas/AOS.


There are more than one way to do this, but we`ll use the simplest way.(and longest, unfortunatley.)


First, make a circle of power. Then, put the unit you want to able to be selected in front of the circle of power. Now, make a region inside the circle of power. Then make this trigger:

Events:

(Unit enters region) A unit enters ( The region inside the circle of power)

Conditions:(This should be the unit type of your hero selector)

Actions: (Units- Create units facing angle) Create (1) (Type of unit want to be selected) at ( a region you want the hero to start).
Now, keep making these for as many heroes as you want. Yes, it takes time, but if you want a good Hero Selector, do it.

Basic Variable Use:


Variables are stored values that anything can be saved into. For example, you can specify certain units for future use.


EX: You want to make a hero for a player and save him for later.


Trigger creates a hero unit


Next function is set variable (in this case, your unit variable) = last created unit.


If you do this, you can make triggers for example:


A unit dies.


Dying unit equal to [variable]


[Actions here]


This way it would be easier to find your hero again, because he is saved.


Different types of variables can be used for many things, and some variables are an absolute must for some triggers, such as dialog buttons.

Unit Properties:


Editing unit properties is fun (sometimes) and easy. Now, the main thing everybody does when editing a unit is changing its  Model File. First, to do this, go to the object editor. Then, select any unit. Scroll the information and look for Model File. Then, you can select from tons of models! You should choose a model to match what its name is. Next, is its Icon. Go up to Icon, there should be (2) sections for Icons if you pick a hero to be changed. One is its model file in the game and the other one is the model file in the End Game Screen. Make them both the same according to the model file(or not, lol.) What you should do, when you create a Custom Unit is modify its attacks. The Damage Base of an attack is what the unit`s damage should be based around. The Dice are the things that modify the difference between low and high damage. Projectile Speed is how fast the projectile (if the unit has one, if it doesn`t, leave this at 0) reaches the enemy. Projectile is its model file of the projectile. You can pick anything you want. Now, go to Name(which if you aren`t using raw data, should be at the bottom) If you selected a hero, there are Proper Names. If you don`t, there`s just a generic name. In a hero, the Normal Name is what class it is and proper names are there real name.

Making Custom Items:


Items are nice. Especiialy ones you make. Making an item is one of the easist things to do in the WE. First go to Object Editor then select 'Items' from the tabs. Choose from any item. I`ll use Claws of Attack +15. Now, you can change the name and description of the item. I`ll change it to ' ATTACK BONUS + 5000!'. Okay, now to edit the items effect. Go to ' Abilities' in tabs, and make a custom one, any name out of Claws Of Attack +15. Now, change the attack bonus to 5000, lol. Now go back to items and change the item abilities to ' Whatever you named the +5000 attack bonus ability' Now your done! Explore different items and have fun!

Custom Doodads/Destructibles:


This is another very simple feature of the WE. Go to Object Editor, click doodads or destructibles. Make a new one and change its model file. It`s that easy!

Gameplay Constants:


This is fairly easy, but not for beginners. This section can change many things. This is mostly for changing the max level of Heroes and units.To do this, go to Advanced then click Gameplay Constants and then click the liitle box that says ' Edit Data ' and go to Hero-Max Level and Unit-Max Level and change it.

Gameplay Interface:


This is one of the hardest and confusing features of WE. It can change the icons of many things in the game like Gold, Lumber, and Food icons. Beginners shouldn`t play around in this area.

Importing Models:


Importing models is easy if you know how to do it. First, d/l the model you like, then open your map and go to the Import Manager, which is under Module. Now Import everything in the folder. Now, copy the file locatian of the textures that were on the web page and change the ones on The Import Manager to match those. Leave the .mdx file alone. .blp=texture file, change those to match the ones on the web page.Then save your map, close WE, and open it again: your custom model should now be there.


Ent_Kill`s Beginner Map Making Tutorial v.1.3


Key:

Blue Text = Hard

Red Text = Easy

Hope this helps lots of beginner map makers!!


Credits:

Thanks to OrderOfPhoenix for variables


 Last edited by a moderator: Mar 15, 2007

#### foxx.assassin

#### foxx.assassin

 Level 3

JoinedNov 20, 2013Messages23

Resources: 2

#### Resources
Maps 2

how to add others starting location ????like making more mines n adding others locations ?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

foxx.assassin said:

 how to add others starting location ????like making more mines n adding others locations ?


Click to expand...

Guide

First, you have to add the players you want as "Users" or "Computers" so that they will be playable. Go to Scenario -> Player Properties in the editor's toolbar.

Mark the players you want to as "User" or "Computer".

Go to Layer -> Units to display the unit palette.

Where it says Player 1 (Red), click. Choose the player you want to add a starting location for. Next, click the "Starting Location" button, and place it on the map wherever you want. If you want to move it after placing it, just hit the spacebar to go into selection mode and then move it around with your mouse.

To place a gold mine, click the area where it says Player (1) Red again (or whichever player it shows at the moment), and then choose Neutral Passive. Select the Gold Mine under the buildings category, and place it on your map.


If you need more help, I recommend that you visit the World Editor Help Zone and make a new thread. If you need help with triggers, then make a new thread in Triggers & Scripts.

#### Attachments

Basics0.jpg

 38.6 KB

 · Views: 18,063



Basics1.jpg

 110 KB

 · Views: 18,510



Basics2.jpg

 55.5 KB

 · Views: 17,741



Basics3.jpg

 145.2 KB

 · Views: 17,928



Basics4.jpg

 140.8 KB

 · Views: 18,590



#### momok123

#### momok123

 Level 2

JoinedJul 28, 2014Messages20

How a useful page...

#### BABA

#### BABA

 Level 6

JoinedMay 21, 2013Messages156

How to make on my map when i start only with units without buildings ,don`t show me message "you are defeated" ???

#### LordOfThePings

#### LordOfThePings

 Level 3

JoinedDec 26, 2013Messages29

Resources: 1

#### Resources
Tutorials 1

BABA said:

 How to make on my map when i start only with units without buildings ,don`t show me message "you are defeated" ???


Click to expand...


Should be the action "Enforce Victory/Defeat Conditions" in the "initialization" trigger.

#### RaptorXI

#### RaptorXI

 Level 1

JoinedJun 21, 2014Messages7

Thanks for these tutorials, helped me alot.

Question: How to look up the size of map that already exists? And can it be post-edited?


 Last edited: Sep 29, 2014

#### LordOfThePings

#### LordOfThePings

 Level 3

JoinedDec 26, 2013Messages29

Resources: 1

#### Resources
Tutorials 1

RaptorXI said:

 Question: How to look up the size of map that already exists? And can it be post-edited?


Click to expand...


I have a German client, so the following might not be fully accurate, also it's standard World Editor (maybe you use WE Unlimited or something like that?).


Open the map -> Scenario (in the standard menu) -> Map Size and Camera Borders


There you can see the sizes of the map and the area the camera can move in. Using the toggle boxes in the bottom left corner and the "arrow buttons" on each side you can modify these areas.


PS: I want to inform you that it's against the rules to double-post, use the edit button instead. Nobody is going to cut your head off for that, but this rule makes life just that much easier for everyone.

#### RaptorXI

#### RaptorXI

 Level 1

JoinedJun 21, 2014Messages7

Ok, thanks for the help (German Editor version here too) and sorry for the doublepost. I found out there is an edit button, so won't happen anymore.

#### Justin Weaver

#### Justin Weaver

 Level 4

JoinedMar 1, 2014Messages68

Resources: 1

#### Resources
Spells 1

BABA said:

 How to make on my map when i start only with units without buildings ,don`t show me message "you are defeated" ???


Click to expand...


Open the trigger editor and delete all triggers on map intialization if you are not making a melee map )

#### whitestorm7890

#### whitestorm7890

 Level 1

JoinedJun 1, 2015Messages9

nice guide

#### WhiteFang

#### WhiteFang

 Level 21

JoinedJul 6, 2014Messages6,786

This tutorial and the comments by Ent Kill 98 and PurgeandFire are super useful.I come here often to refresh my knowledge

#### soulshot

#### soulshot

 Level 2

JoinedApr 22, 2013Messages10

GREAT, but how do i change the position of my units like if i want them to face on the left side ? how ? or right side ?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

You should head over to the World Editor Help Zone for such questions in the future.

You can change the facing of a preplaced unit by doubleclicking on them and then writing in which angle to face. Put a value between 0 and 360, 0 is right 180 is left.

#### DaLittle

#### DaLittle

 Level 2

JoinedSep 20, 2016Messages10

wow this totally help me a lot, thanks! well its not different like other rts game editor (same concept like age of empire) still thanks

#### oio

#### oio

 Level 1

JoinedSep 22, 2016Messages3

ent_kill_98 said:

ent_kill`s Beginner Map-Making Tutorial v1.0

Ent_ Kill`s Beginner Map Making Tutorial!

---------------------------------

Ok, If it`s your first time making a map you shouldnt start with an AOS. But an RPG would be a good choice.

Terraining:

First we need to discuss terraining =)

Terrain is simple, cliffs are easy to produce. Go to the main window then click "New Pallete" Go to "Terrain Pallete". In the middle will be the "apply cliffs" section.

there will be a number of choices of cliffs to choose from. First you must choose the base cliff at the bottom left.

There are 2 base cliffs to choose from,this depends on the tileset you pick.

after you`ve placed your cliffs then we can go to the next step. =)

Water Terraining:

This also depends on what tileset you choose. you also need to set the base cliff to depend on what the "surroundings" of the water will be.

you can select from a number of sizes,Just like in cliffs.

Water is the most simplest step,next to cliffs.

The Terrain for your map should be A-OK now. =)

Now we`ll discuss doodads.

Doodads:

Doodads will make any map look nicer.

There are different types of doodads such as cliffs,enviromental,and others.

Well there`s not really anything difficult about doodads..making doodads is fun, but this is a tutorial for begginers.

Units are next!

Units:

Units are easy... if your not creating them, but let`s go to the basics.

go to window,and go to Unit pallete.

Here you can now select from a number of different races

such as Human, Orc, Undead, Night Elf, and Neutral (TFT players)

Here also you can choose from melee units, campaign units or your own custom ones.

Just select them and place them in the desired places.

Now on to regions!

Regions:

Regions are simple, they are bases for most triggers. You can place regions by clicking the box at the top right and highlighting the desired area.

Cameras are next >=)

Cameras:

Cameras aren`t arent needed, unless your game is a cinematic or you

teleport a unit.

Destructibles:

Destructibles are very useful in many aspects of a good map.

Trees make a map look nicer, but too many will make it look horrocious.

Gates are also destructibles, they are useful in many areas also.You can also make trigger enhanced destructibles. This is like...If you destroy a unit, a gate opens. That`s one of the best ways to impress a person playing one of your maps. Heres How:

Events: (Specific Unit Event) A (Unit) Dies

Conditions: None, unless wanted

Actions (Destructibles-Open/Close/Destroy Gates) Open(Gate)
Destructables is an easy aspect and this is all you need to know about begginer map-Destructables.

Custom Spells:

For a beginner, custom spells aren`t that important. If you`re a beginner, stick to changing names, effects, and damage of already made spells. If you are not a beginner, trigger-enhanced spells are the way to go =).

Triggers:

Triggers are a beginning map makers nightmare. If you are a beginner, you`d probably say "OMFG WTF IS THAT?!" looking at a trigger from current RPGs hosted on B.net. Triggers are still hard even if you`re not a beginner, but they become easier every time you make one. Here are some recently used triggers featured in alot of maps:

Region Triggers:

This is when you step on or enter a certain region and something happens. Teleporting is commonly used by doing this. Heres an example:

Events: (Unit enter region) a unit enters (region)

Conditions: ( Mostly this is a Unit Type Conditon, we`ll talk about those later.)

Actions: (Unit-Move Unit Instantly) move (triggering unit) instantly to (region).
Done with that.

Hero Selections:

This is probably the most commonly used trigger releated thing in RPG maps/arenas/AOS.

There are more than one way to do this, but we`ll use the simplest way.(and longest, unfortunatley.)

First, make a circle of power. Then, put the unit you want to able to be selected in front of the circle of power. Now, make a region inside the circle of power. Then make this trigger:

Events:

(Unit enters region) A unit enters ( The region inside the circle of power)

Conditions:(This should be the unit type of your hero selector)

Actions: (Units- Create units facing angle) Create (1) (Type of unit want to be selected) at ( a region you want the hero to start).
Now, keep making these for as many heroes as you want. Yes, it takes time, but if you want a good Hero Selector, do it.

Basic Variable Use:

Variables are stored values that anything can be saved into. For example, you can specify certain units for future use.

EX: You want to make a hero for a player and save him for later.

Trigger creates a hero unit

Next function is set variable (in this case, your unit variable) = last created unit.

If you do this, you can make triggers for example:

A unit dies.

Dying unit equal to [variable]

[Actions here]

This way it would be easier to find your hero again, because he is saved.

Different types of variables can be used for many things, and some variables are an absolute must for some triggers, such as dialog buttons.

Unit Properties:

Editing unit properties is fun (sometimes) and easy. Now, the main thing everybody does when editing a unit is changing its  Model File. First, to do this, go to the object editor. Then, select any unit. Scroll the information and look for Model File. Then, you can select from tons of models! You should choose a model to match what its name is. Next, is its Icon. Go up to Icon, there should be (2) sections for Icons if you pick a hero to be changed. One is its model file in the game and the other one is the model file in the End Game Screen. Make them both the same according to the model file(or not, lol.) What you should do, when you create a Custom Unit is modify its attacks. The Damage Base of an attack is what the unit`s damage should be based around. The Dice are the things that modify the difference between low and high damage. Projectile Speed is how fast the projectile (if the unit has one, if it doesn`t, leave this at 0) reaches the enemy. Projectile is its model file of the projectile. You can pick anything you want. Now, go to Name(which if you aren`t using raw data, should be at the bottom) If you selected a hero, there are Proper Names. If you don`t, there`s just a generic name. In a hero, the Normal Name is what class it is and proper names are there real name.

Making Custom Items:

Items are nice. Especiialy ones you make. Making an item is one of the easist things to do in the WE. First go to Object Editor then select 'Items' from the tabs. Choose from any item. I`ll use Claws of Attack +15. Now, you can change the name and description of the item. I`ll change it to ' ATTACK BONUS + 5000!'. Okay, now to edit the items effect. Go to ' Abilities' in tabs, and make a custom one, any name out of Claws Of Attack +15. Now, change the attack bonus to 5000, lol. Now go back to items and change the item abilities to ' Whatever you named the +5000 attack bonus ability' Now your done! Explore different items and have fun!

Custom Doodads/Destructibles:

This is another very simple feature of the WE. Go to Object Editor, click doodads or destructibles. Make a new one and change its model file. It`s that easy!

Gameplay Constants:

This is fairly easy, but not for beginners. This section can change many things. This is mostly for changing the max level of Heroes and units.To do this, go to Advanced then click Gameplay Constants and then click the liitle box that says ' Edit Data ' and go to Hero-Max Level and Unit-Max Level and change it.

Gameplay Interface:

This is one of the hardest and confusing features of WE. It can change the icons of many things in the game like Gold, Lumber, and Food icons. Beginners shouldn`t play around in this area.

Importing Models:

Importing models is easy if you know how to do it. First, d/l the model you like, then open your map and go to the Import Manager, which is under Module. Now Import everything in the folder. Now, copy the file locatian of the textures that were on the web page and change the ones on The Import Manager to match those. Leave the .mdx file alone. .blp=texture file, change those to match the ones on the web page.Then save your map, close WE, and open it again: your custom model should now be there.

Ent_Kill`s Beginner Map Making Tutorial v.1.3

Key:

Blue Text = Hard

Red Text = Easy

Hope this helps lots of beginner map makers!!

Credits:

Thanks to OrderOfPhoenix for variables

Click to expand...

 hi there  i finished to subscribe just now , i am going to edit my personal version of the night elfs , but , i need some informations to do this : i noticed that set all the three basic colors to 0 make the modded unit black , and that's part of what i need to do , but i don't know if with the existent tools to edit textures , i can give to a different unit , the ghostly\spectral\transparency effect that belong to the special night elfs unit ( vengance spirit and vengance ) , because i want to give that transparency effect to the assassin ( if i well remember it can be found in the mission were tayrande chose to free illidan , can someone help me with this ?

#### mfdsrax2

#### mfdsrax2

 Level 3

JoinedMay 19, 2009Messages38

What should I do to add an upgrade to the Necropolis so that it produces corpses (like the Graveyard) after the upgrade?

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

mfdsrax2 said:

 What should I do to add an upgrade to the Necropolis so that it produces corpses (like the Graveyard) after the upgrade?


Click to expand...

add the ability "Create Corpse" to the upgraded version of Necropolis.

#### mfdsrax2

#### mfdsrax2

 Level 3

JoinedMay 19, 2009Messages38

Rheiko said:

 add the ability "Create Corpse" to the upgraded version of Necropolis.


Click to expand...

Thanks for the help. Yes I tried to add the ability "Create Corpse" to the Necropolis and it worked.

But I found a problem. I want the Necropolis to be able to produce corpses only after an upgrade. So I set a prerequisite to it linking to a custom upgrade. But when I test the map, it produces corpses at the very beginning without that technology.

Could you help me with that? Many thanks!

#### Cellesteel

#### Cellesteel

 Level 2

JoinedMar 26, 2018Messages10

was wondering how i would even get to where to even do a custom spell as that is not explained

#### Aggiba

#### Aggiba

 Level 5

JoinedDec 30, 2018Messages19

Resources: 5

#### Resources
Maps 5

Just made a map last week. Hope youll like it

#### Attachments

(12) Turtle Island.w3x

 206 KB

 · Views: 294



#### Lytair

#### Lytair

 Level 1

JoinedMay 11, 2019Messages1

Does anyone knows where can i enable that only race human's can be available.. ?

#### xoachimtachi

#### xoachimtachi

 Level 1

JoinedNov 15, 2019Messages6

sorry I posted this article but honestly I'm looking forward to PROs who can only help with WE, it doesn't work for player 2- player9, please show me this problem, thank you everyone for listening! !!

#### Attachments

1.jpg

 111.9 KB

 · Views: 373



2.jpg

 133.5 KB

 · Views: 363



3.jpg

 121.1 KB

 · Views: 397



4.jpg

 138.4 KB

 · Views: 313



5.jpg

 121.5 KB

 · Views: 305



6.jpg

 140.1 KB

 · Views: 343



7.jpg

 149.9 KB

 · Views: 362



#### Fabián Montero

F

#### Fabián Montero

Hi! how can I fix the "too many" trees problem? I have been doing a couple of maps changes and I cant save my progress because of thta. Thx

 You must log in or register to reply here.


### Similar threads

 Beginners F.A.Q To Terraining


iPeez

May 22, 2009

Misc/General Mapping Tutorials

Replies19Views16K

Nov 2, 2010
_StarScreaM_

S

 Terrain: Forest Cliffs


Jattenalle

May 6, 2007

Misc/General Mapping Tutorials

Replies15Views21K

Nov 1, 2008
Dimadelic

 The Ultimate Beginners Terrain Tutorial


HermanTheGibbon

Jun 12, 2007

Misc/General Mapping Tutorials

Replies48Views68K

Nov 21, 2021
Verdun

 Basic Terraining Tutorial


Torn

Jun 9, 2005

Tutorial Archive

Replies10Views7K

Mar 27, 2007
Wolverabid

 iPeez's Complete Terraing Tutorial v1.0


iPeez

Jan 18, 2009

Misc/General Mapping Tutorials

Replies19Views20K

Mar 20, 2025
Filson

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