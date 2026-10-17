---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/alpha-tile-for-dummies.227513/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Alpha Tile for Dummies
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/alpha-tile-for-dummies.227513/>
#### bhusta

#### bhusta

 Level 13

JoinedMar 6, 2008Messages843

Resources: 4

#### Resources
Icons 1Maps 2Tutorials 1

Alpha Tile for Dummies

 -by bhusta


Complexity: Low


Requirements:
Warcraft 3 Editor

An alpha tile texture

This tutorial explains what is an alpha tile, how one can create it and how one can use it.

First of all, why do people call it alpha tile?

It is a texture which only uses the alpha channel.

An alpha channel can be used for different things like team color or transparency. In this case it is the transparency.

Not all file types have an alpha channel. Look at the following example. The first file type of the picture is .png. The second one is .jpg

.png

.jpg


This tutorial is divided into three parts.

How to create an alpha tile?

How to make it useable for the editor?

How to use it?

Part 1 - How to create an alpha tile?

open a graphic modification software like gimp or photoshop.

create a new image.

The dimensions must be divisible by two like 2x2 or 256x256. The size doesn't matter.

The background color must be transparent.

After creating the image you already created the alpha tile.

Now you have to make it useable for warcraft 3.

Save it as a .bmp file because this file type has an alpha channel.

open Warcraft 3 Viewer link

Press the TGA -> BLP button

open your new created transparent image.

save it.

Now you should have something like yourFileName.blp

Part 2 - How to make it useable in the Warcraft 3 Editor?

Create a new map

this tutorial uses the Lordearon-Summer standard tile set

import your alpha tile texture with the file type .blp via Import-Manager

How can one use it?


The alpha tile is a texture which will replace an existing texture of a tile. We will replace the DirtGrass tile.

So, how do we do that?


We have to change the import path of the imported texture to the path of the tile we want to replace.


The paths of tiles have this scheme:

TerrainArt\Tileset_Name\Tile_Name.blp


As mentionened above the tileset name is Lordearon-Summer so the path is

Terrain Art\LordearonSummer\


and we want to replace the DirtGrass so the final path is

TerrainArt\LordearonSummer\Lords_DirtGrass.blp


You can find all TerrainArt paths under this link.


Now, save the map and restart the Editor.

Part 3 - How to use the alpha tile?

Basically, you can use it like any other tile. Just like this.

Pick

brush

Place units on it


Example 1 - The Bridge


Select the fire doodads and press "Ctrl + PgDn". This will lower the doodads so that the fire will be under the terrain. Now, you can place a unit on it and it lookds like a stone bridge.


Example 2 - The walkable Volcano

Place the volcano doodad somewhere.

Brush with the alpha tile and press G to see the grid.

Now raise the terrain so that you can see the grid on the volcano. You have to select the vocano first and press "Ctrl + PgDn or PgUp" so that the vocano doesn't change the height with the terrain.

A walkable volcano.


You can see the examples in the attached map.

Tile Overlapping


The last point is about overlapping of the terrain tiles. It has a significant effect on how the alpha tile looks in Warcraft 3. Overlapping simply defines which tile overlaps another tile.


This is how the overlapping is in the examples above:


Now we are going to change the overlapping order.


This is how one can change it:


Open the Edit Tileset window here:


Advanced -> Edit Tileset


This is the window you should see:


Now, move the alpha tile (DirtGrass) to the left and it is under the other tiles, move it to the right and it is on the other tiles.

At the bottom

Attention

Do not move the alpha tile to the very left like in this example!

If the alpha tile is the bottom tile, it looses its transparency

At the top

Limitations of the Alpha tile

it looses transparency in fog of war and black mask

It is white or black on the minimap

How to solve this problem can be found here.

it looses transparency if it is the bottom tile as mentioned in the overlapping part above.

#### Attachments

Bridge 1.jpg

 121.7 KB

 · Views: 4,194



Bridge 2.jpg

 117.7 KB

 · Views: 4,301



Bridge 3.jpg

 303.9 KB

 · Views: 4,335



Bridge 4.jpg

 135.2 KB

 · Views: 4,221



TileSet.jpg

 3.2 KB

 · Views: 4,092



Rename B.jpg

 17.1 KB

 · Views: 3,981



Rename A.jpg

 16 KB

 · Views: 4,051



Problem2.jpg

 8.2 KB

 · Views: 3,879



Problem.jpg

 64 KB

 · Views: 3,897



Vocano 2.jpg

 120.4 KB

 · Views: 4,199



Vocano 1.jpg

 72.5 KB

 · Views: 4,257



Using 3.jpg

 66 KB

 · Views: 4,152



Using 2.jpg

 107 KB

 · Views: 4,083



Using 1.jpg

 130.6 KB

 · Views: 4,223



Vocano 3.jpg

 189 KB

 · Views: 4,313



Vocano 4.jpg

 136.9 KB

 · Views: 3,987



Vocano 5.jpg

 74.8 KB

 · Views: 660



user178091_pic57849_1336214445.jpg

 14.5 KB

 · Views: 3,976



Alpha Tile Tutorial by bhusta.w3x

 27.6 KB

 · Views: 660



overlapping1.jpg

 80.1 KB

 · Views: 6,521



overlapping2.jpg

 35.2 KB

 · Views: 3,960



overlapping3.jpg

 98.9 KB

 · Views: 5,898



overlapping4.jpg

 110.2 KB

 · Views: 3,952



problem3.jpg

 67.8 KB

 · Views: 3,892




 Last edited: Jan 18, 2014

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

You can also apply this if you want other tiles to overlap so the full square doesn't show.

https://www.hiveworkshop.com/forums...ials-278/terrain-overgoes-small-guide-145182/

#### bhusta

#### bhusta

 Level 13

JoinedMar 6, 2008Messages843

Resources: 4

#### Resources
Icons 1Maps 2Tutorials 1

Thanks, I will also gather some other links of tutorials which use the alpha tile. Your point could be quite important. I don't know yet if it would be better I just show your link or make an example of my own.


 Last edited: Dec 24, 2012

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

Probably cover it yourself. Collect what you need from Belgarath's tutorial and optimize it.


Oh, there's also this weird phenomena that if you shift it to far left, it's no longer transparent. Don't know why.

#### bhusta

#### bhusta

 Level 13

JoinedMar 6, 2008Messages843

Resources: 4

#### Resources
Icons 1Maps 2Tutorials 1

So, I added the overlapping point. Maybe I change the layout. It looks a little bit messy now.

What do you mean by

 Oh, there's also this weird phenomena that if you shift it to far left, it's no longer transparent. Don't know why.


Click to expand...

?

The shadow in front of the map boundary acts like the fog of war, if you mean that ^^

#### Kino

#### Kino

 Level 24

JoinedOct 12, 2008Messages1,783

Resources: 22

#### Resources
Models 5Maps 17

 Oh, there's also this weird phenomena that if you shift it to far left, it's no longer transparent. Don't know why.


Click to expand...


I can confirm this.

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

bhusta said:

 The shadow in front of the map boundary acts like the fog of war, if you mean that ^^


Click to expand...

No I meant in the Edit Tileset window. If you shift the alpha tile to the far left side of the window it isn't transparent anymore when you place it.


Try it out, it converts to paper-white texture with no transparency. I acutally used it as ice once.

#### bhusta

#### bhusta

 Level 13

JoinedMar 6, 2008Messages843

Resources: 4

#### Resources
Icons 1Maps 2Tutorials 1

what the hell  that was new to me

edit: I have to edit the overlapping part ...

edit2: added the new problem

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Looks nice overall. Since you have added the part about overlapping, I think it is unique enough to be approved.

#### myPhoenixBlade123

#### myPhoenixBlade123

 Level 2

JoinedDec 8, 2013Messages22

omg GREAT its usefull clear tutorial!

1+

#### bhusta

#### bhusta

 Level 13

JoinedMar 6, 2008Messages843

Resources: 4

#### Resources
Icons 1Maps 2Tutorials 1

myPhoenixBlade123 said:

 omg GREAT its usefull clear tutorial!


1+


Click to expand...


Thank you.


I also added a point on how to solve one of the limitations.


If someone finds more limitations and/or possible solving approaches I will be happy to add them.

#### V

#### V

 Level 7

JoinedJan 13, 2019Messages284

Resources: 5

#### Resources
Spells and Systems 4Tutorials 1

The transparency issue can be solved with this configuration:

1. Alpha tile in second tile position

2. "Masked Areas Are Partially Visible" can be checked or unchecked (Map Properties)

3. In triggers add this

Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
Visibility - Create an initially Enabled visibility modifier for (Picked player) emitting Black mask across (Playable map area)

If you want to solve the minimap issue you can do this:

1. Select a dummy tile for coloring the minimap (snow in this time)

2. Travel across the map changing that tile with our alpha tile

See the attachment to make a test.

If you want to find the code of a tile:

place that tile in the center of the map (where the tree is) and use code findcode later change this value with dummy_tile_type variable value.

Commands

fogon

fogoff

blackon

blackoff

swap

findcode

I make a tutorial for that see this:

Alpha Tile Issues solving

#### Attachments

Alpha_tile.w3x

 28.1 KB

 · Views: 258




 Last edited: Nov 10, 2021

#### Meatsausage

#### Meatsausage

 Level 4

JoinedDec 21, 2019Messages51

Resources: 1

#### Resources
Maps 1

Hello! Any way how to make an invisible tile in Reforged?

Greets

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Not the answer you wanted, but if you change one of the ruined ground models to "none.mdl" and place it, it will appear invisible in game. You're limited to squares only, though.

Meatsausage said:

 Hello! Any way how to make an invisible tile in Reforged?

Greets


Click to expand...

 You must log in or register to reply here.


### Similar threads

Locked

 Alpha tile is not tranperent in fog of war


RollBoat

Jul 29, 2019

World Editor Help Zone

Replies4Views1K

Jul 31, 2019
Dr Super Good

 Using Reflections in Terraining!


fladdermasken

Jun 26, 2010

Misc/General Mapping Tutorials

Replies40Views50K

Jan 11, 2014
myPhoenixBlade123

 Alpha Tile Issues solving


V

Nov 12, 2019

Tutorial Archive

Replies2Views2K

Oct 2, 2020
FeelsGoodMan

Locked

 Mighty Alpha Tile: What exactly does it do?


Galatius

Dec 23, 2012

Terrain & Level Design

Replies2Views2K

Dec 23, 2012
Galatius

Locked

 Yet another Alpha Tile question


Hashjie

Dec 21, 2011

World Editor Help Zone

Replies13Views3K

Feb 5, 2012
FirstLegacy

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