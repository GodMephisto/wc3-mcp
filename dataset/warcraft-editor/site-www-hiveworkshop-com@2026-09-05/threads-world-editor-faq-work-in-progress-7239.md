---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/world-editor-faq-work-in-progress.7239/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# World Editor Faq Work in Progress
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/world-editor-faq-work-in-progress.7239/>
#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


World Editor Basics

The Object Editor

The Object Manager

The Import Manager

The Terrain Editor

The Trigger Editor

The Sound Editor

Introduction

This FAQ is a collection of several frequently asked questions and short tutorials about performing different tasks in Blizzard's Warcraft III World Editor. The FAQ serves as a basic reference point: it should (hopefully) assist mapmakers of all skill levels. This FAQ is by no means definitive: it is still under development- please post any suggestions or recommendations in this thread.

World Editor Basics

 Things To Keep In Mind by Healingpoison


 These are a couple things I think everyone should know when making a map.

Test your map regularly.

Most important: test your map often, as you put in new triggers test them to make sure they work. The last thing you want is to make your whole map (triggers and all) and then find out one trigger doesn't work, then you have to go through and check each trigger which can be very difficult.

Save your map often.

Save your map often (under a new file name) before new triggers are added or new resources are imported. If a map should somehow become corrupt, having a recent backup is critical: it could make the difference between loosing five minutes of work versus five months.

Stay on topic.

Keep being focused around one topic (This may not apply to some of you more senior people), especially if your new to world editor its important to stay on one topic. I know it is easy to drift off into other ideas and then attempt to mush them all together, but you should try to focus your map, for example: if you have a Tower Defense it wouldn't fit to have the players building a hero along the side lines, I mean think about people play TDs because they like the fast paced action and the idea of building defenses.

Start small.

Do not try to go past your abilities. Map making is like skiing, when your learning to ski you just don't start out on the slope with the most black diamonds, you start easy on a green or blue, in map making you shouldn't try to start putting in advanced triggers that you don't understand, stick with the easy stuff first, If your making an AoS just start with the basic spawing and hero selection triggers, try to refrain from advanced triggering until your at the point where your map requires it.

Imported Resources:

Maps do not need custom models, skins, and icons to be great. One great example is Warhammer 40k TD! Any map can be great without imported items. Also, its alright if you don't understand how to import such items. Don't get all upset if you cant get customs into your map, just keep working on it.

Don't give up!

Keep at it. Giving up on a map you have worked hard to create is one of the worst things you can do in world editor. I know i have do it before, once I was working on a Warhammer 40k battle and I had nearly finished, I had all the terrain, units, and triggers, but then I gave up on it and started making a new map, and now I'm making a map just like that one and i wish i had never gotten rid of that first version.

Be open.

Sharing your ideas with other people to get their opinion is always a good idea. If your stuck when your writing a storyline for your map and you don't know whether the heroes should get squashed by trolls or shot by dwarfs, ask someone. Also, you can have people help you make your map, its not like its a shame to have someone else work on your map with you, in fact its probably going to make a better map.

(Thanks Ralle)

Increase the speed of the World Editor

Many people complain about World Editor lag, especially when working in the Object Editor. This is because of the Brush List, a list of all the objects included in the editor. For example, when you rename a unit the editor will completely refresh the Brush List: this could require up to 20 seconds. If you disable the Brush List, you will find that you can instantly rename a unit and not have to wait at all.

It's easy to disable the Brush List:

From the main WE menu choose: Window

De-select the item: Brush List

Easy backup

If your map has been corrupt, go look in the Maps/Test folder, there is always a backup map (WorldEditTestMap.w3x) from the last time you tested.

Disable auto-save

File/Preferences/General and uncheck Autosave.

NOTE: Disabling autosave is a good idea, it can often be annoying and can also ruin your map if you do something wrong and it suddenly autosaves.

Taking screenshots

Hit the Print Screen key (just over the Insert button) to take a screenshot.

NOTE: For best results, select View>Lock to Game View Settings.

NOTE: Screenshots will be saved in the Warcraft 3/Screenshots Directory.

Controlling Cameras

Zooming: Use the scrolling wheel to zoom in and out.

Changing angle: Hold down CTRL and hold down rightclick while you rotate camera in any direction with the mouse.

When zooming far out, black mask will appear, hold down CTRL and use the scrolling wheel to make it disappear.

Different views

x change to letterbox view.

v change to game view.

c change to camera view.

Working with colorizing and line breaks

Warcraft 3 have it's own text formatting system. It allows you to put color on text strings and make linebreaks. Every tag starts with a '|' (without the quotes) followed by a letter.

So the color tag is '|c' followed by a 6 digit hexadecimal (RR GG BB) color code and a 2 digit hexadecimal transparency code. But the transparency code is not used so just leave it as 'ff'. The tag ends with a '|r'.

Example: 'Hello my name is Rasmus'. So now I want to color the part 'name' in red and it will look like this: 'Hello my |cff0000ffname|r is Rasmus'.

Some places when you want to enter a string, it is not possible to make a linebreak by pressing ENTER and then you have to write '|n' at the place you want to make a linebreak.

[self="http://hiveworkshop.com/forums/showthread.php?t=31386"]Warcraft III Color Tags and Linebreaks[/self] covers this topic in much greater detail.

Custom Minimap Preview

Custom minimap previews for custom scenarios is a big fashion. Some people might actually leave the game you've hosted because they think that the map sucks when it doesn't have a minimap preview. So here's a guide on how to make your own.

Open your favorite image editor and make yourself a good picture. You have to keep in mind that the image should have the dimensions of either 256x256 or 128x128.

Save it as .jpg or .tga if you can. If not, go get the tool Warcraft 3 Image Extractor II, open up your image and save it as .blp or .tga.

Open up World Editor and then open the Import Manager and import the image.

Double-Click on the image and tick "Use Custom Path" and set the path to:

war3mapPreview.tga or war3mapPreview.blp to have the image being the minimap preview.

You can also save it as war3mapMap.tga or war3mapMap.blp to make it being the in-game minimap.

Importing a Minimap Image

Importing Minimap images is easy. You will just have to make an image in .tga or .blp format and import it into your map and change its name. This method is used in many maps and makes the map more interesting for people who might join when hosting a game.

You can choose between two sizes of images: 256x256 and 128x128, I prefer using 256x256 because there is a world's difference between the quality of these images. But also a world's difference in size :S

Creating the Image (with advanced image editing program)

Let's say I want to make an image, so I open up Photoshop, Photoimpact, Gimp... You will need an advanced image editing program and not just paint. You can get Photo Impact Here.

Remember to make the image 256x256 or 128x128 in size and save it as .tga!

Creating the Image (using any image editing program)

Create a new image, set the size to 256x256 and save it as .jpg.

Then open up Wc3ImageExtractor (get it here)

Click Open -> Image and open your picture, then click Save -> Image and save it as a .tga or .blp image.

Importing the Image

Open up World Editor, then Import Manager and press Import File.

After that, double click on the imported file in the Import Manager, tick Use Custom Path and set the custom path to one of these:

war3mapPreview.tga - this is the preview people can see when joining the game.

war3mapMap.tga - this is the in-game minimap which you can see when present in the game.

Custom Loading Screen Images

Magos' War3 Model Editor Can easily create custom loading screens.

Custom Loading Screen Tutorial V1.5 shows an alternate method.

The Object Editor

Copy objects (doodads, units etc) from map to map

Right-click the object you want to move, press Copy, then open the new map, right-click and press paste.

NOTE: Some units' custom abilities may become corrupt during this process, just reset their abilities (this wont delete any of your objects).

Heroes

Creating new heroes

Two methods exist for creating new hero's.

From the Object Editor file menu, select New Custom Unit; the Create New Custom Unit dialog appears. Select a unit from the interface and press OK.

Choose a hero (paladin, warden etc.), right-click and select Copy Unit, then right-click again and press Paste Unit.
Edit the new heroes' stats to suit your requirements.

Heroes past level 5 by creeping

Normally heroes stop gaining experience when they have reached level 5, but it's actually very easy to change those limits.

Go to "Advanced" -> "Gameplay Constants"

Tick "Use Custom Gameplay Constants"

Find the field called "Hero XP Gained - Creep Reduction Table"

This is the table where you can see how much experience you get on each level. As default it's set to "80,70,60,50,0" which we can put in a table like this:

1 - 80%

2 - 70%

3 - 60%

4 - 50%

5 - 0%

So by changing the 5th limit to 40% instead, you will be enabled to reach level 6. Then at level 6,7,8,9,10 you will still receive 40% experience so you might want to add more values to it.

Heroes past level 10

In many WarCraft scenarios you can now raise a hero's level to above level 10. This is done by changing one single value.

Go to "Advanced" -> "Gameplay Constants"

Tick "Use Custom Gameplay Constants"

Find the field called "Hero Maximum Level"

Set this to whatever you want the maximum level to be.

NOTE: Maximum is 10,000.

Converting a non-hero unit to a hero

It is not possible to actually convert a unit to a hero directly. But you can always copy all the important stats, models, abilities and paste them on to the other character.

Using morphed models

It's possible (but a little tricky) to change a unit's standard model to a morphed Demon Hunter, Robo-Goblin etc. The problem is that a models like the Demon Hunter have both morphed and standard models in one single mdx file. So you need to send a morph tag to it so the game knows how to display units using such models.

In the Object Editor

Set the model of the unit to the standard model of the morphed model e.g:

 Code:

Art - Model File: [U]Tinker, Tinker[/U]

Here comes what way we want the unit to be displayed, set this value:

 Code:

Art - Required Animation Names: [U]Alternate[/U]

The value "Alternate" seems to be used for all morphs on units that have the morphed value in the same model file.

This trick also works with:

Druid of the Claw (Night Elf, morphed to bear)

Druid of the Talon (Night Elf, morphed to crow)

Gargoyle (Undead, morphed to stoneform)

Obsidian Statue (Undead, morphed to destroyer-form)

Troll Headhunter (Night Elf, morphed to berserker)

Viewing raw object name

If you need an objects raw name, hit CTRL + D to see it.

Autofill information

When making skills with many levels it's good to right-click on the first level and click Auto Fill Levels and set how you want it to be.

Setting units damage

When it's your first time visiting the Object Editor, it might be quite overwhelming seeing that changing the damage of a unit requires 3 values, Damage Base, Damage Number of Dice and Damage Sides per Dice.

The minimum damage is:

 Code:

Base Damage + Damage Number of Dice

The maximum damage is:

 Code:

Base Damage + Damage Number of Dice * Damage Sides per Dice

So if we now set the Base Damage to 3, the Damage Number of Dice to 5 and the Damage Sides per die to 4 you will get the values:

Minimum Damage = 3 + 5 = 8

Maximum Damage = 3 + 5 * 4 = 23

So the unit will have the damage of 8 - 23

Turning on Bounty

Bounty is the gold you get from killing a unit. The bounty can as the damage vary, you can have a minimum bounty and a maximum bounty.

The minimum bounty is:

 Code:

Gold Bounty Awarded - Base + Gold Bounty Awarded - Number of Dice

The maximum bounty is:

 Code:

Gold Bounty Awarded - Base + Gold Bounty Awarded - Number of Dice * Gold Bounty Awarded - Sides per Die

But if the unit you set the bounty on is not a normal creep, you will have to add a trigger too. Go to your Trigger Editor and create a new trigger:

Bounty
Events
Map Initialization

Conditions

Actions
Player - Turn Gives Bounty On for (Player 1 (red))

(You might have a hard time finding the 'Turn On Bounty' but it is called: 'Player Turn Player Flag On/Off')

How to rename Quests to Info and Bounty to Author

When browsing around the hosted scenarios on battle.net you will find that some maps have renamed the button for 'Quests' to 'Info' or maybe changed the Food Limit popup in the top right corner to the name of the author. This is what I will be explaining.

Click Advanced -> Gameplay interface

Tick "Use Custom Gameplay Interface"

Then find the value you want to change. Here's a list of some of the often changed values:

 Text - General - 'Optional Quests'

Text - General - 'Quests'

Text - General - 'No Upkeep'

Text - General - 'Low Upkeep'

Text - General - 'High Upkeep'


Click to expand...

The Object Manager

The Import Manager

WarCraft Files (w3m, w3x, mpq..)

MPQ (MoPaQ) files are similar to zip-files. They are used for many of the popular Blizzard Games (Warcraft, Diablo, Starcraft). They contain many models, icons and other elements for these games. It's easy to open these files and extract their contents for use in the World Editor or in other software.

Many programs exist to extract files from MPQ archives, among my favorites is

Warcraft III Image Extractor it can not only access mpq files but also w3m files and w3x files which are the extensions of Warcraft maps, this allows you to extract your minimap image and various other images.

List Files

A MPQ-file has a listfile to show all the contents of the MPQ-file. Here is the list of the content inside each of the different Warcraft MPQ-Files:

 war3.mpq - Standard Warcraft 3 (RoC)

 war3patch.mpq - Patches -

 war3xlocal.mpq - Frozen Throne Local Files -

 war3x.mpq - Frozen Throne Files
For more detailed information about MPQ's see the following articles about this subject.

BlizzPubs Article

Inside MoPaQ

W3M and W3X Files Format

A mirror of the above image {PRESENTLY DOWN}

The Terrain Editor

Select ALL objects

CTRL + A selects all objects in Terrain Editor According to which palette you are using.

Selecting / deselecting objects

Hit SPACE to select the last selected object and again to deselect it.

Quick move objects or regions

Hold down ALT and click wherever you want to move the selected thing.

Changing doodads' height

CTRL + PgUp for moving the doodad Up.

CTRL + PgDn for moving the doodad Down.

NOTE: this is very useful for placing bridges correctly.

Placing Objects without Collision-size

Hold down the SHIFT key while placing doodads on the map. This method allows doodads to be placed very close together (or even on top of one another).

Basic Terraining Tutorial by Torn source

Introduction

So you've made a map, but its just not complete, it needs one more thing, terrain, terrain makes up most of the map, sure it can have great triggers, great skins and what not, but no one likes to play on a plain bare surface. So lets get started.

Step 1: The basic tileset.

When you make terrain you should always be aware of what type of terrain you want the players to focus on mainly, rather it be grass, snow or dirt, it doesn't matter. But lets make ours a type of forest area. I made a 32x32 map with Lordaeron tileset. Go to the terrain palette and select the 'Grass' tile. Place the grass randomly around the basic area of your piece of terrain. It should now look something like this.

Step 2: Blending Tilesets

Sure, it looks horrible for the moment, but now we will have to blend tilesets. Select another tile from the terrain palette, lets use grassy dirt, fill in SOME of the gaps were there is dirt, don't fill in all the gaps. Now that we have blended those two tiles together its time to add one more tile, lets use dark grass. Fill in the remaining dirt spots with dark grass and it should turn out to look something like this...

Step 3: Water and Cliffs

Now, you have a flat plain of land that looks decent, but you don't see that type of land when you go out walking do you? Let's fix it up some...Back to the terrain palette select the 'cliff' labeled shallow water and drag it diagonally downward to make a stream. You should now have a stream, but it is bare and it needs some doodads, so, go to the doodad palette and look under water. Lets put some fish in, one destroyed ship and some floating lily pads. You should now have a beautiful stream. Back to work, back to the doodad palette, go under the category labeled Bridges/Ramps, search for short natural bridge (diagonal 2) and lay it across your newly created stream. If the bridge doesn't seem to look quite right, left click on it and click Ctrl + Page up or Page down to raise and lower the bridge.

Now your map should look like this...

Step 4: Trees and surroundings

Your map is looking great already, but not great enough, go back to the Terrain palette and at the bottom of it apply height 'Raise'. Click on area of your map, around the bridge and such, do create small bumps in the terrain. Select the 'smooth' tool if they get too pointy. Now, back to the doodad palette. In the doodad palette go under Trees/Destructibles and select the Summer Tree Wall. Place trees around your desired area until it looks just as you would like it too, ours is supposed to be a variation of a forest. Your done, if you wish you can add custom doodads or tilesets which can be downloaded from this site, this is just a brief explanation of terraining, more doodads should be added such as: Barrels, Homes, Crates, Tents, Banners. It all depends on the setting.

Lets look at the final product:

Good luck with your terraining!

The Trigger Editor

Remove starting units

Remove the Action "Meeée Game - Create starting units (for all players)" from the trigger "Melee Initialization".

Remove starting resources

Remove the Action "Melee Game - Set starting resources (for all players)" from the trigger "Melee Initialization".

The Sound Editor

Soundsets

A soundset is the "vocabulary" of a unit. Units have a varying amount of specific voices in their "vocabularies" that you cannot change. However you can replace the voices in a unit's vocabulary.

This also means that you can't create a new soundset or extend the chosen soundset. So you have to find a soundset that you are not using and which also fits your amount of voices. As an example I will use the Footman's soundset.

The Footman's soundset is located in Sounds/Internal/Units/Human/Footman/ and contains the following sounds/voices:

FootmanDeath.wav

FootmanPissed1.wav

FootmanPissed2.wav

FootmanPissed3.wav

FootmanPissed4.wav

FootmanReady1.wav

FootmanWarcry1.wav

FootmanWhat1.wav

FootmanWhat2.wav

FootmanWhat3.wav

FootmanWhat4.wav

FootmanYes1.wav

FootmanYes2.wav

FootmanYes3.wav

FootmanYes4.wav

FootmanYesAttack1.wav

FootmanYesAttack2.wav

FootmanYesAttack3.wav
It's easy to replace all the existing sounds with new ones.

Importing & Replacing

For each file of the footman's soundset you right-click and choose: Replace Internal Sound Then locate your own recorded sound.

Done, You should now have made.. I mean replaced an entire soundset.

Contents

http://www.forums.hiveworkshop.com/showthread.php?t=5899


 Last edited by a moderator: Nov 2, 2009

Status
 Not open for further replies.


### Similar threads

 World Editor Tips


Ralle

Jan 3, 2006

Misc/General Mapping Tutorials

Replies28Views57K

Sep 20, 2025
your568473

 Importing/Exporting Object Data from WE


RedBlade

May 24, 2007

Misc/General Mapping Tutorials

Replies17Views21K

Aug 7, 2021
ThePerfectIdiot

Locked

 World Editor Basics


Ralle

Oct 26, 2006

World Editor Help Zone

Replies0Views10K

Oct 26, 2006
Ralle

 New Trees in World Editor


Tails96

Nov 18, 2009

Misc/General Mapping Tutorials

Replies24Views34K

Oct 4, 2018
Raka Uthada

 How to use the campaign editor


Septimus

Apr 18, 2009

Misc/General Mapping Tutorials

Replies21Views36K

Jul 4, 2014
PurgeandFire

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