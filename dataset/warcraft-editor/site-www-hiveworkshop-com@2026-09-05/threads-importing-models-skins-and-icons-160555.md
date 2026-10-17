---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/importing-models-skins-and-icons.160555/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Importing Models Skins and Icons
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/importing-models-skins-and-icons.160555/>
#### Traxamillion

#### Traxamillion

 Level 19

JoinedSep 14, 2007Messages1,527

Resources: 10

#### Resources
Models 2Icons 7Tutorials 1

Importing Models, Skins and Icons

A Picture book for the new and weary.

 I made this tutorial for a few reasons, I want to create an extremely basic way to do import these resources with detailed information along with visual aides. Numerous times people enter the chat wondering how to import these files, after saying read the tutorials hundreds of times I always get the same reply "It makes no sense" or "I did"

 This tutorial will NOT discuss importing tilesets, UI, or any other custom resources that do not fall under the category of the title.

Basic Terminology:

.zip - The most common means of a compressed folder

.rar - A compressed folder, accessed by “7zip” and “Winrar”

.7z - A compressed folder, accessed by the program “7zip”

.mdx - The standard path used by Blizzard for all models within the WC3 engine.

.mdl - A model when it is still in its beta phase, for editing and reworking, prior to a .mdx

.tga - Texture path that can be opened in various art tools, a texture will be this before converting to a .blp

.blp - Blizzard’s texture path used in the WC3 engine.

Texture/skin - a recreated “skin” for a currently existing model, standard or custom.

Helpful Tools:

Magos Model Editor (Modeling)

Warcraft 3 Viewer (Modeling/Skinning)

Button Manager (Icon)

Models

Here is the resource I’m going to use; Cavman and Hawkwing’s Dire Troll

Step by Step Importing

Step 1: Open the Import Manager of your map within World Editor

Step 2: Make sure you unzip, unrar your compressed file before importing.

Step 3: use the Import button to locate your custom resource file and click open or double click on the containing files

It should look like this;

Step 4: Now you want to set the correct paths to the model files (.mdx) and the texture files (.blp)

PLEASE NOTE: Always erase the “wc3imported” part of an imported resource, Also if you are unaware of a model’s texture paths, either view the readme.txt file in the model folder or use Magos’ Texture Manager to view its path.

Step 5: Applying the model, Within the units tab of the object editor either make a custom unit, or use an existing one, click on the unit. Find “Art-Model File” and double click. Click the import button and scroll down and apply the .mdx file, not the portrait. (Portraits do not need to be applied only imported)

Step 6: IMPORTANT, in order for the model to show in game, without looking like a big green and black box, you must SAVE your map and close World Editor, then reopen the map in World Editor.

Step 7: Find your unit in the unit palette and place it wherever.

Skins/Textures

Here is the resource i'm going to use; Red Shift's Timberwolf (Replacing Dire Wolf)

Step by Step Importing

Step 1:Open the Import Manager of your map within World Editor

Step 2: Make sure you unzip, unrar your compressed file before importing.

Step 3: use the Import button to locate your custom resource file and click open or double click on the containing files

It should look like this;

Step 4: Now you want to set the correct paths to the texture files (.blp)

NOTE: Skins/Textures have SET paths and only use ONE certain path when it comes to editing a pre-existing model skin. You can go to the skin section here at the hive and find a skin that is used for the same model to find the path in case you don't know it. Also use Magos Model Editor and open up texture manager within Magos and it will show the paths for all skins used in the resource opened.

Heres an example of a skin path: Units\Creeps\DireWolf\DireWolf.blp

Step 5: Applying the skin/texture, Within the units tab of the object editor either make a custom unit, or use the existing dire wolf model, click on the unit. (When the texture is imported, saved and correctly path'd you do NOT need to change anything regarding the model as long as the model is set to the standard dire wolf.)

Step 6: IMPORTANT, in order for the skin/texture to show in game, you must SAVE your map and close World Editor, then reopen the map in World Editor.

Step 7: Find your unit in the unit palette and place it wherever.

Icons

Here is the resource i'm going to use; Mr.Goblin's Cape Icon.

Step by Step Importing

Step 1: Open the Import Manager of your map within World Editor

Step 2: Make sure you unzip, unrar your compressed file before importing.

Step 3: use the Import button to locate your custom resource file and click open or double click on the containing files

It should look like this;

Step 4: Now you want to set the correct paths to the Icon files (.blp)

Icons have set paths, here are the set path variations;

ATCAutocasting - ReplaceableTextures\CommandButtons\

DISATCDisabled Autocasting - ReplaceableTextures\CommandButtonsDisabled\

BTNRegular Icon Button - ReplaceableTextures\CommandButtons\

DISBTNDisabled Regular Icon Button - ReplaceableTextures\CommandButtonsDisabled\

PASPassive Button - ReplaceableTextures\CommandButtons\

DISPASDisabled Passive Button - ReplaceableTextures\CommandButtonsDisabled\

Step 5: Applying the Icon, Within the abilities/buffs/Upgrades tab of the object editor either make a custom Ability,Buff or Upgrade, or use an existing one, click on the Ability,Buff,Upgrade. Find “Art-Icon” and double click. Click the import button and scroll down and apply the .blp, this can be for any variation.

Step 6: IMPORTANT, in order for the Icon to show in game, you must SAVE your map and close World Editor, then reopen the map in World Editor.

Step 7: Find your Icon in the correct palette or within the object editor.

 Hopefully this helps, its pretty straight-forward and thorough, if you need additional assistance feel free to PM me.


 Last edited: Mar 16, 2010

#### Misha

#### Misha

 Level 49

JoinedJun 9, 2008Messages16,523

Resources: 73

#### Resources
Models 64Icons 1Packs 2Skins 4StarCraft II Resources 2

very well thought out that even a dummy can understand it.

Good job Trax

#### Pyramidhe@d

#### Pyramidhe@d

 Level 44

JoinedNov 8, 2007Messages2,252

Resources: 35

#### Resources
Models 4Icons 28Skins 2Tutorials 1

looks pretty straight forward. hopefully people will read through it properly

#### zt2211

#### zt2211

 Level 7

JoinedAug 13, 2006Messages199

Resources: 2

#### Resources
Maps 2

So easy! Your grandmother who has arthritis could import warcraft III files if she read this tutorial!

#### Kwah

#### Kwah

 Level 24

JoinedMay 9, 2007Messages3,561

Resources: 12

#### Resources
Icons 7Maps 3Tutorials 2

Explain how to find the paths first, and mention that they can be found on the site, and how to find it in Magos (link tool).

Explain skin paths in more depth.

Provide links to the resources used.

And I believe you don't actually have to re-open the WE to have all models/skins be available in-game.

Overall, it's clear.

#### Darkness-4ever

#### Darkness-4ever

 Level 9

JoinedNov 4, 2007Messages932

I have to at the very least close and re open my map on WE for the imported models and skins to show, maybe your computer's just different.

#### Traxamillion

#### Traxamillion

 Level 19

JoinedSep 14, 2007Messages1,527

Resources: 10

#### Resources
Models 2Icons 7Tutorials 1

Ok I touched on everything you mentioned hawk. Tell me what you think.

#### Kwah

#### Kwah

 Level 24

JoinedMay 9, 2007Messages3,561

Resources: 12

#### Resources
Icons 7Maps 3Tutorials 2

Immediatly, still link to the first model.

Will re-read, give more suggestions and approve tomorrow.

#### Traxamillion

#### Traxamillion

 Level 19

JoinedSep 14, 2007Messages1,527

Resources: 10

#### Resources
Models 2Icons 7Tutorials 1

The model I used is not released, therefore no link. Doesnt matter anyway its a model with standard .mdx and a skin.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Overall, it is a fine tutorial. There are existing tutorials for this, but I guess people can look at this one if they just want a quick reference or if they just don't remember. Thus, ~approved.

#### Kwah

#### Kwah

 Level 24

JoinedMay 9, 2007Messages3,561

Resources: 12

#### Resources
Icons 7Maps 3Tutorials 2

There is actually plans to integrate something like we have now more noticeably into the resource system, so eventually these will be obsolete.

Still, at the moment it was approve-able quality.

#### clockwork2

#### clockwork2

 Level 15

JoinedJan 26, 2010Messages1,629

Resources: 4

#### Resources
Models 2Maps 2

good tutorial but there is something like this in model links

#### Dyslexter

D

#### Dyslexter

real useful tutorial,

But ive been trying to get the EpicRPGtileset by 67 chrome to work on one of my maps for a long while now but ll that i seem to get, at teh very most is a tile being black around teh edges of teh man-made walls. Can anyone lend me a hand i would really appreciate it

#### nichawkins

#### nichawkins

 Level 3

JoinedJan 9, 2010Messages53

Could used this like a year ago haha. This is a Great tutorial. But The object Editor changes my selected Inport to the unselected custom. The custom is .mdl , and as you know i need the mdx anyway you can help me?

#### Traxamillion

#### Traxamillion

 Level 19

JoinedSep 14, 2007Messages1,527

Resources: 10

#### Resources
Models 2Icons 7Tutorials 1

When you import the .mdx the object editor automatically displays it as a .mdl in reality it is still a .mdx so you should be ok. If you have the file strictly as a .mdl then you need to convert it to a .mdx before importing. I believe you can do so in Magos.


And thanks for the kind words everyone.


 Last edited: Jun 30, 2010

#### nichawkins

#### nichawkins

 Level 3

JoinedJan 9, 2010Messages53

Wait re Models skins do they need to be Texture/(insertmodelnamehere).mdx?

#### Traxamillion

#### Traxamillion

 Level 19

JoinedSep 14, 2007Messages1,527

Resources: 10

#### Resources
Models 2Icons 7Tutorials 1

No, Skins made for models are "blahblahblah.blp" textures will always be in .blp format. If you are having trouble finding the paths for the skins connected to the model, open the model in magos model editor, and go to "windows" at the top and open "Texture Manager" it will list all models and skins involved with what model you have opened with the correct paths.

#### kaiserimpact

#### kaiserimpact

 Level 4

JoinedAug 7, 2010Messages77

is it a must to use blp? do i need to convert tga into blp before importing? sorry newbie here.


Edited* problem solved, thanks for the guide! thumb up!


 Last edited: Aug 9, 2010

#### rafaelzimrp

R

#### rafaelzimrp

Pretty good tutorial! Helped me a lot! Thanks!

#### DarkGreenDude

D

#### DarkGreenDude

I cant find my "Import Manager" I don`t have the Frozen Throne. Help??

#### struna

S

#### struna

cool

cool, but what with models contains upgrades itself?


like one .mdl file which include 3 - 9 upgrades? how to use them in game?

#### Traxamillion

#### Traxamillion

 Level 19

JoinedSep 14, 2007Messages1,527

Resources: 10

#### Resources
Models 2Icons 7Tutorials 1

struna said:

 cool, but what with models contains upgrades itself?

like one .mdl file which include 3 - 9 upgrades? how to use them in game?


Click to expand...

im not sure i understand what you mean by upgrades.

#### DMCfan

#### DMCfan

 Level 2

JoinedOct 13, 2010Messages34

really thanks man i was trying import skins and i havent no idea how to do this before this tutorial

#### Mr.Pockets00000

#### Mr.Pockets00000

 Level 9

JoinedJan 1, 2010Messages197

Resources: 15

#### Resources
Skins 2Maps 10Spells and Systems 1Tutorials 2

I am having a major issue with importing skins, for some reason even when i put in the path that they tell you to use it does not work, even after i save world edit and close it, and sometimes when it does replace the model's skin the model is just transparent.

#### Traxamillion

#### Traxamillion

 Level 19

JoinedSep 14, 2007Messages1,527

Resources: 10

#### Resources
Models 2Icons 7Tutorials 1

Mr.Pockets00000 said:

 I am having a major issue with importing skins, for some reason even when i put in the path that they tell you to use it does not work, even after i save world edit and close it, and sometimes when it does replace the model's skin the model is just transparent.


Click to expand...

Sometimes the paths that are listed are not always correct so to be sure i would check the path of the same model but different texture. For instance check two different abomination paths are right in case one is wrong. Also sometimes textures are broken or possibly named wrong via editing and is worth asking a knowledgeable skinner why something may not work after troubleshooting yourself.

#### BananaEater

B

#### BananaEater

Just a question what if you have multiple .blp files and a single .mdx/.mdl file?

#### Robovision

#### Robovision

 Level 2

JoinedNov 8, 2018Messages11

Hi. I know how to import models for units and doodads. The same textures for units. But how to put texture to the ground texture? Because in object editor there is no option for ground textures and I can not find it anywhere. I as well used Google and searched in HIVE but all is connected to the same stuff I already know but none connected to ground textures.

So where is actually field for changing model for ground texture?

Thank you for answer

#### Hermit

#### Hermit

 Level 22

JoinedMay 29, 2013Messages1,573

Resources: 8

#### Resources
Models 4Icons 3Sounds 1

Robovision said:

 But how to put texture to the ground texture? Because in object editor there is no option for ground textures and I can not find it anywhere. I as well used Google and searched in HIVE but all is connected to the same stuff I already know but none connected to ground textures.

So where is actually field for changing model for ground texture?


Click to expand...

In Object Editor the 'Art - Ground Texture' field determines which ground texture (aka Ubersplat) appears under the building when it is placed on the ground.

Only ubersplats that are used as building foundations (by standard buildings) are listed in the drop-down list when this field is opened.

If the shift key is pressed while this field is being opened, any ubersplat that is specified in the 'UberSplatData.slk' can be used by entering its four letter code. This allows all ubersplats to be used, even those that are unused (e.g. LSDL, CLTS, NMED) or intended for abilities (e.g. HMTP, UDSU, NVOL). Custom ubersplats can also be used by entering their codes if a custom 'Splats\UberSplatData.slk' file is imported into the map.

#### Shadowsky

#### Shadowsky

 Level 2

JoinedJan 13, 2022Messages3

Hi there,

Is there a way to turn .dds into .mdx? All files I've downloaded from here for Warcraft 3 Reforged only come as a .dds file and I am not given the option to save as another format. Please advise if you have a solution

 You must log in or register to reply here.


### Similar threads

 Importing/Exporting Object Data from WE


RedBlade

May 24, 2007

Misc/General Mapping Tutorials

Replies17Views21K

Aug 7, 2021
ThePerfectIdiot

 How to: Import an Icon


Archian

Nov 7, 2006

Misc/General Mapping Tutorials

Replies4Views59K

Jan 22, 2023
Shawwy

 Simple Loading Screen


Aesthetics

Aug 11, 2009

Misc/General Mapping Tutorials

Replies42Views38K

Apr 27, 2021
erfaned

 Importing Skins


oz02

Jun 6, 2004

Tutorial Archive
234

Replies170Views38K

Mar 6, 2007
Wolverabid

 How To Change Textures on a Tree or other Doodad (Without importing several models)


InfernalTater

Sep 3, 2012

Misc/General Mapping Tutorials

Replies6Views19K

Mar 11, 2024
fostaa

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