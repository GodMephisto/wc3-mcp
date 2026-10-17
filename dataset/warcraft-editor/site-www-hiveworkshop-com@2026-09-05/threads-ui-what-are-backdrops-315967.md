---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-what-are-backdrops.315967/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI What Are Backdrops
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-what-are-backdrops.315967/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

##
 Introduction

Backdrops mange the visual textures of frames, they are borders, backgrounds or images (for non simple frames). Most frames beeing more than simple text have backdrops that manage the textures beeing shown.

BACKDROP can use all the ImageTypes Warcraft 3 supports: BLP, TGA, DDS. DDS can only be used in Warcraft 3 V1.32+. For DDS the compression type matters, in my Test only DXT1/DXT5 worked (generated with GIMP).​

Jump to without fdf

##
 BACKDROP FDF

These backdrop childs/subFrame get a stronger connection by declaring them as Control. Such control backdrops mimics the mainFrame in size and position, (if not said otherwise). The 2 most basic controlBackdrops are.

ControlBackdrop <name>,
<name> is the name of the frame beeing used, it also is most time declared right below the control line. This backdrop is used when the parentFrame is enabled and basicly every Frame has such a thing.

ControlDisabledBackdrop <name>,
backdrop when the parentFrame is disabled.

Some FrameTypes have more additional controlbackdrops.​

##
 What are the Backdrop stats?

Backdrops can not have frameevents.

Backdrops are above simpleframes, hiding them.

nameargumentsinfo

DecorateFileNamesLook-Up FileNames in some String table (for example gameinterface)

BackdropTileBackgroundFills the Frame with tiles of the texture. Without the background file is stretched

BackdropBackgroundSize0 to 1.0in Tile mode, the size of each tile

BackdropBackgroundFilePath only one "\"main image

BackdropBackgroundInsets real real real realcuts in the background from the frames outside, granting space between the edge and the background. Right Top Bottom Left. The numbers can be Negative to extend this side over the edge into the outside.

BackdropCornerFlags"UL|UR|BL|BR|T|L|B|R"FRAMEPOINTS having borders (edgefile). Order does not matter

BackdropCornerSize0.0 to 1.0size of the border/edge.

BackdropEdgeFileFilePaththe border File, this texture having the border as fragments next to each other (gameinterface). You don't need the single ones if you have this one.

BackdropLeftFileFilePath

BackdropCornerFileFilePath

BackdropTopFileFilePath

BackdropRightFileFilePath

BackdropBottomFileFilePath

BackdropBlendAlluse transparency by alpha channels. An image with alpha channel transparency but without this will look wierd and wrong. It also allows to see the things behind the frame.

BackdropHalfSides

BackdropMirroredvertical, a face looking to the left will look with this option enabled to the right.

When changing textures with code most of that stuff becomes irrelevant and the backdrop can be empty except for the BackdropBackgroundSize, if one wants tile filling. One changes texture by code with this native.

 JASS:

native BlzFrameSetTexture takes framehandle backdrop, string texFile, integer flag, boolean blend returns nothing

texFile the file one wants to use.

flag, 0 for stretched mode; 1 for tile mode.

blend has something to do with transparenz. (false) makes it look wierd, if the texture uses transparenz
Using BlzFrameSetTexture will drop the BackdropEdgeFile.​

##
 Creating BACKDROPS without FDF

Creating just an Icons on the screen doesn't even need a fdf, one can create a BACKDROP using BlzCreateFrameByType, afterwards set texture, position and size of that BACKDROP and you have a visible image on the screen. Such a BACKDROP doesn't have a tile mod nor an additional edgefile/border.

The Example creating a backdrop with BlzCreateFrameByType is from xorkatoss.

 JASS:

local framehandle blademaster = BlzCreateFrameByType("BACKDROP", "Blademaster", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
call BlzFrameSetSize(blademaster, 0.04, 0.04)
call BlzFrameSetAbsPoint(blademaster, FRAMEPOINT_CENTER, 0.4, 0.3)
call BlzFrameSetTexture(blademaster, "ReplaceableTextures\\CommandButtons\\BTNHeroBlademaster",0, true)

##
 Example with FDF

This is the text of a custom BACKDROP. In side the fdf there are 2 Frames. "MyBackgroundTile" and "MyBackground".

 Code:

Frame "BACKDROP" "MyBackdropTile" {
 //DecorateFileNames,
 BackdropTileBackground, //Fills the space with the background texture
 BackdropBackground "ReplaceableTextures\CommandButtons\BTNHeroPaladin.blp",

 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.0125,
 BackdropBackgroundSize 0.02, //in tile mode, size of each tile
 BackdropBackgroundInsets 0.005 0.005 0.005 0.005,
 BackdropEdgeFile "UI\Widgets\ToolTips\Human\human-tooltip-border.blp",
}

Frame "BACKDROP" "MyBackdrop" {
 BackdropBackground "ReplaceableTextures\CommandButtons\BTNHeroPaladin.blp",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.0125,
 BackdropBackgroundInsets 0.005 0.005 0.005 0.005,
 BackdropEdgeFile "UI\Widgets\ToolTips\Human\human-tooltip-border.blp",
}

This jasscode loads the custom toc loading the BACKDROPs in and creates 1 of both after 1 second passed. The tile version is at the left side of the screen. The Big image is at the center of the screen.

 JASS:

function CreateBackdrops takes nothing returns nothing
 //Create the big face
 local framehandle fh = BlzCreateFrame("MyBackdrop", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0),0, 0)
 call BlzFrameSetSize(fh, 0.1, 0.1)
 call BlzFrameSetAbsPoint(fh, FRAMEPOINT_CENTER, 0.4, 0.3)

 //Create the tile faces
 set fh = BlzCreateFrame("MyBackdropTile", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0),0, 0)
 call BlzFrameSetSize(fh, 0.1, 0.1)
 call BlzFrameSetAbsPoint(fh, FRAMEPOINT_CENTER, 0.2, 0.3)
endfunction

//===========================================================================
function InitTrig_Backdrop takes nothing returns nothing
 set gg_trg_Backdrop = CreateTrigger( )
 call TriggerRegisterTimerEventSingle( gg_trg_Backdrop, 1.00 )
 call TriggerAddAction( gg_trg_Backdrop, function CreateBackdrops )
 call LoadToc("war3mapimported\\mybackdrop.toc") //Loads the custom toc file
endfunction

The code will not create exactly the following images cause it is recreation, but the will look quite similar.

Changelog:

added missing statements to the table.

 added a GoTo no Fdf, colored the Headlines and some formating.​

###
 Other UI-Frame Tutorials

The following links might provide more insight into this subject.

UI: Change Lumber Text

[JASS/AI] - UI: Create a TextButton

[JASS/AI] - UI: Positionate Frames (important)

UI: toc-Files

UI: Reading a FDF

UI - The concept of Parent-Frames

[JASS/AI] - UI: FrameEvents and FrameTypes

UI: Frames and Tooltips

[JASS/AI] - UI: Creating a Bar

UI - Simpleframes

UI: What are BACKDROPs?

UI: GLUEBUTTON

UI: TEXTAREA the scrolling Text Frame

UI: EditBox - Text Input

[JASS/AI] - UI: Creating a Cam control

UI: Showing 3 Multiboards

UI: OriginFrames

Default Names for BlzGetFrameByName (Access to Existing game-Frames)

[JASS/AI] - UI: List - Default MainFrames (built in CreateAble)​

#### Attachments

MyBackdrop.w3x

 17.1 KB

 · Views: 663




 Last edited by a moderator: Oct 10, 2020

#### The_Silent

#### The_Silent

 Level 46

JoinedFeb 4, 2008Messages3,511

Resources: 204

#### Resources
Models 63Icons 115Packs 8Skins 16Maps 1Ported Models 1

This is all very abstract. Could you attack a working example?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Do you mean an example for using "BlzFrameSetTexture"? Or showing that in frame definition of a existing BACKDROP, or both?

#### The_Silent

#### The_Silent

 Level 46

JoinedFeb 4, 2008Messages3,511

Resources: 204

#### Resources
Models 63Icons 115Packs 8Skins 16Maps 1Ported Models 1

Just the example of how you created the example in the picture ingame.

I have trouble initiating a framehandels with a backdrops. So a working example would be great.


 Last edited: Jun 4, 2019

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The example is only an Backdrop/image with no interactivity, But okay will add custom fdf, code and map.

#### The_Silent

#### The_Silent

 Level 46

JoinedFeb 4, 2008Messages3,511

Resources: 204

#### Resources
Models 63Icons 115Packs 8Skins 16Maps 1Ported Models 1

Thank you very much, that is fine.

It has taken some time for me to figure UI out, so even simple examples helps a ton.

My first working backdrop was literally a modification of the map you just uploaded. Took some time to realized everything was so fdf dependent, so had to dig through all the fdf files to find the type of ui elements that worked for me.

Also, incredibly annoying 'BlzFrameSetTexture' removes the edge file. Have to go with classic icon buttons for now. But I suppose a layered gluebutton definition where the edge is on another backdrop than the icon will eventually solve it. Unless blizzard fixes it first.

#### rolandc85

#### rolandc85

 Level 1

JoinedJan 2, 2019Messages3

In certain cases, we would want checkbox to be disabled after user first checked it. However, we don't want it to be grayed out. Can this be done?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Yes, it can be done, although you might have to consider that the greying is a help for the user. It might confuse him to not be able to click a yellowed box.

A gluecheckbox has 5 backdrop childs each shows one part of the shown checkbox. There are multiple options. But what you want to do is make ControlDisabledBackdrop and ControlBackdrop be kinda equal. This are not the names of the frames you need to change. For the real names one would need to know which checkbox you are using.

You define a custom checkbox which uses the same texture in disabled (ControlDisabledBackdrop) as in enabled (ControlBackdrop).

After you disabled the checkbox change the texture of the backdrop showing the graybox to the same as the enabled one.

swap visiblity for disabled/enabled backdrops.

#### Donach

#### Donach

 Level 6

JoinedJan 12, 2011Messages110

Resources: 2

#### Resources
Maps 1Tutorials 1

Hey Tasyen,

Is it possible to create a circular backdrop/frame? Have you tried?

Cannot find any relevan information if anyone tried...

Thanks.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

No, i did not try that. Maybe one would have to make the rect outside of the circle transparent. I mean blp support transparent behaviour from what I know.

Edit: Triggerhappy has shown an custom ui with circled Textures.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

Backdrop seems to refuse to go below point zero. For instance if I set the position-x to something around -0.05 then the texture gets deformed. Why is that?

(Inventory texture at bottom left corner)

The texture is supposed to look like this

Do you know a good replacement for backdrop, just for displaying simple texture, that doesnt have this problem?

#### The_Silent

#### The_Silent

 Level 46

JoinedFeb 4, 2008Messages3,511

Resources: 204

#### Resources
Models 63Icons 115Packs 8Skins 16Maps 1Ported Models 1

You cannot make ui beyond the non-hd ui borders it seem.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

Well, simple status bar can. Maybe it's just for simple frames? Haven't checked that...

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

SIMPLEFRAME have texture childs to display blp. Texture can not be a mainFrame.

 Code:

Frame "SIMPLEFRAME" "SimpleBackground" {
 Texture "SimpleBackgroundTexture" {
 File "UI\Widgets\Console\Human\human-inventory-slotfiller.blp",
 }

}

One can repos the menu buttons out of the 4:3 Screen.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

Can SIMPLEFRAME have events?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

only SIMPLEBUTTON & SIMPLECHECKBOX but for both only the needed click events.

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

"Backdrops can not have frameevents" so they can't be made click-able?

Question 1:

How can one remove the border aka "edgefile"?? I tried this: BackdropEdgeFile "",

but it gives an error, also completely removing the line gives an error

I guess the only way is to make a SimpleFrame instead??

Question 2:

I'm sure a lot of confused people will ask this question regarding UI Inventory Systems.

"Backdrops are above simpleframes, hiding them"

So if I understand this right, it means for example, if you make an inventory system you want to use Backdrops for the Inventory Background and the Item Icons.

Then you have to make SimpleFrames with substitute frames to show tooltips?

but how do you manage which backdrops are above which?

EDIT:

I checked your tutorials and I didn't find a way to make backdrops/simpleframes click-able?

can you cover that as well please?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

xorkatoss said:

 Question 1:

How can one remove the border aka "edgefile"?? I tried this: BackdropEdgeFile "",

but it gives an error, also completely removing the line gives an error

I guess the only way is to make a SimpleFrame instead??


Click to expand...

You also have to remove/disable the other lines setting values for the border/Edge, in the fdf.

BackdropCornerFlags

BackdropCornerSize

xorkatoss said:

 So if I understand this right, it means for example, if you make an inventory system you want to use Backdrops for the Inventory Background and the Item Icons.

Then you have to make SimpleFrames with substitute frames to show tooltips?


Click to expand...

No, Frames have good frametypes beeing able to show text: TEXT and TEXTAREA, it is not needed to use Simpleframes for that. My current available tutorials miss parts of the frame usage.

xorkatoss said:

EDIT:

I checked your tutorials and I didn't find a way to make backdrops/simpleframes click-able?

can you cover that as well please?


Click to expand...

I have such a tutorial, it was approved and moved into the jass section, Although I'd recommend using a (GLUE)BUTTON when one wants something clickable.

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

hmm so after experimenting a bit I have come to the conculsion that you only need the .fdf files if you want "BackdropEdgeFile" or any other stuff.

You can actually create backdrops using only code:

 JASS:

local framehandle blademaster = BlzCreateFrameByType("BACKDROP", "Blademaster", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
call BlzFrameSetSize(blademaster, 0.04, 0.04)
call BlzFrameSetAbsPoint(blademaster, FRAMEPOINT_CENTER, 0.4, 0.3)
call BlzFrameSetTexture(blademaster, "ReplaceableTextures\\CommandButtons\\BTNHeroBlademaster",0, true)

@Tasyen

I think this information might be useful to include in your tutorial since at first I thought that if you wanted just to make a basic backdrop you had to include .fdf and .toc files with this text to set the texture:

 Code:

Frame "BACKDROP" "MyBackdrop" {
 BackdropBackground "ReplaceableTextures\CommandButtons\BTNHeroPaladin.blp",
}

#### Attachments

Backdrop Test.w3x

 7.5 KB

 · Views: 399



#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

xorkatoss said:

 hmm so after experimenting a bit I have come to the conculsion that you only need the .fdf files if you want "BackdropEdgeFile" or any other stuff.

You can actually create backdrops using only code:

 JASS:

local framehandle blademaster = BlzCreateFrameByType("BACKDROP", "Blademaster", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
call BlzFrameSetSize(blademaster, 0.04, 0.04)
call BlzFrameSetAbsPoint(blademaster, FRAMEPOINT_CENTER, 0.4, 0.3)
call BlzFrameSetTexture(blademaster, "ReplaceableTextures\\CommandButtons\\BTNHeroBlademaster",0, true)

@Tasyen

I think this information might be useful to include in your tutorial since at first I thought that if you wanted just to make a basic backdrop you had to include .fdf and .toc files with this text to set the texture:

 Code:

Frame "BACKDROP" "MyBackdrop" {
 BackdropBackground "ReplaceableTextures\CommandButtons\BTNHeroPaladin.blp",
}

Click to expand...

yes, if one wants an normal image on the screen, creating the BACKDROP with BlzCreateFrameByType is the easiest way.

Edit: Added your Code into the tutorial mentioning you.

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

@Tasyenwoah thanks man

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

For .DDS textures, you can use Paint.Net, you do not need a plugin, as it already has its own support for .DDS. You may not have all the tools and professional quality of Ps, but it is easily found, free, fast and light. Besides having tools very close to the main ones used in Ps.

Saving in these settings as I put below, will work perfectly in your Reforged:

Ps. MipMaps always checked, this is important!


 Last edited: Sep 20, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Approved. Useful much like the others.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

BackdropBackgroundInsets can be used to rotate an image, when Top&Bottom are as big as Height and Left&Right are as big as Width it rotates top to bottom

 Code:

Frame "BACKDROP" "Test" {
 Width 0.1,
 Height 0.1,
 BackdropBackgroundInsets 0.1 0.1 0.1 0.1,
 SetPoint CENTER, "ConsoleUI", CENTER, 0, 0,
 BackdropBackground "ReplaceableTextures\CommandButtons\BTNFootman",
}

#### Attachments

Mirrored twice.jpg

 8.3 KB

 · Views: 188



#### q531620267

#### q531620267

 Level 1

JoinedOct 6, 2021Messages3

Thank you for your tutorial. It helps me learn about UI. Do you have a list of other types of frame stats besides backdrop stats?

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


@Tasyen Is it possible to change the border texture of a BACKDROP without a custom fdf file?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

q531620267 said:

 Thank you for your tutorial. It helps me learn about UI. Do you have a list of other types of frame stats besides backdrop stats?


Click to expand...

Yes, on hive Tutorial [JASS/AI] - The Big UI-Frame Tutorial

Pastebin: The Big UI-Frame Tutorial

scroll down to Section FrameTypes that has some fdf stuff, (if it is done).

Or on github i have a not to frametypes connected list of fdf actions. FDF/Keywords at master · Tasyen/FDF

HeavenSmitingDevilEmperor said:

@Tasyen Is it possible to change the border texture of a BACKDROP without a custom fdf file?


Click to expand...

I don't think so.

good luck.

#### q531620267

#### q531620267

 Level 1

JoinedOct 6, 2021Messages3

Tasyen said:

 Yes, on hive Tutorial [JASS/AI] - The Big UI-Frame Tutorial

Pastebin: The Big UI-Frame Tutorial

scroll down to Section FrameTypes that has some fdf stuff, (if it is done).

Or on github i have a not to frametypes connected list of fdf actions. FDF/Keywords at master · Tasyen/FDF

I don't think so.

good luck.


Click to expand...

thank you very much.

 You must log in or register to reply here.


### Similar threads

 UI: Frames and Tooltips


Tasyen

Jun 27, 2019

Scripting Tutorials

Replies45Views21K

Oct 27, 2024
RPG_F

 UI: Reading a FDF


Tasyen

May 29, 2019

Scripting Tutorials

Replies15Views16K

Dec 1, 2020
Tasyen

 UI: Creating a Cam control


Tasyen

Jun 1, 2019

Scripting Tutorials

Replies24Views10K

Mar 4, 2021
ESTO

 UI: GLUEBUTTON


Tasyen

Sep 9, 2019

Scripting Tutorials

Replies32Views15K

Nov 16, 2023
Tasyen

 UI: Create a TextButton


Tasyen

May 30, 2019

Scripting Tutorials

Replies12Views12K

Jan 11, 2021
Tasyen

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top