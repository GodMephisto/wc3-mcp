---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-frames-starting-guide.318603/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Frames Starting Guide
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-frames-starting-guide.318603/>
#### Anchorist

#### Anchorist

 Level 5

JoinedOct 10, 2010Messages71

Resources: 2

#### Resources
Tutorials 2

Check out Tasyen's guides, tutorials and introductions at this link ui-fdf | HIVE for more informations about UI Frames

Overview

With patch 1.31 Blizzard added more than a few new API functions that allow us, mappers to manipulate with game UI in general (such as visibility of certain parts of default UI) and also allowed to use custom created frames, so if you ever wanted for your map to provide totally custom UI now it's easier than ever!

What functions?

Functions needs types, so we're gonna start with them

Frame related types

 Code:

type framehandle extends handle
type originframetype extends handle
type framepointtype extends handle
type textaligntype extends handle
type frameeventtype extends handle

Frame related constants

 Code:

 constant originframetype ORIGIN_FRAME_GAME_UI
 constant originframetype ORIGIN_FRAME_COMMAND_BUTTON
 constant originframetype ORIGIN_FRAME_HERO_BAR
 constant originframetype ORIGIN_FRAME_HERO_BUTTON
 constant originframetype ORIGIN_FRAME_HERO_HP_BAR
 constant originframetype ORIGIN_FRAME_HERO_MANA_BAR
 constant originframetype ORIGIN_FRAME_HERO_BUTTON_INDICATOR
 constant originframetype ORIGIN_FRAME_ITEM_BUTTON
 constant originframetype ORIGIN_FRAME_MINIMAP
 constant originframetype ORIGIN_FRAME_MINIMAP_BUTTON
 constant originframetype ORIGIN_FRAME_SYSTEM_BUTTON
 constant originframetype ORIGIN_FRAME_TOOLTIP
 constant originframetype ORIGIN_FRAME_UBERTOOLTIP
 constant originframetype ORIGIN_FRAME_CHAT_MSG
 constant originframetype ORIGIN_FRAME_UNIT_MSG
 constant originframetype ORIGIN_FRAME_TOP_MSG
 constant originframetype ORIGIN_FRAME_PORTRAIT
 constant originframetype ORIGIN_FRAME_WORLD_FRAME

Frame related hashtable addition

 Code:

native SaveFrameHandle
native LoadFrameHandle

Frame related methods themselves

 Code:

native BlzGetOriginFrame takes originframetype frameType, integer index returns framehandle
native BlzEnableUIAutoPosition takes boolean enable returns nothing
native BlzHideOriginFrames takes boolean enable returns nothing
native BlzConvertColor takes integer a, integer r, integer g, integer b returns integer
native BlzLoadTOCFile takes string TOCFile returns boolean
native BlzCreateFrame takes string name, framehandle owner, integer priority, integer createContext returns framehandle
native BlzCreateSimpleFrame takes string name, framehandle owner, integer createContext returns framehandle
native BlzCreateFrameByType takes string typeName, string name, framehandle owner, string inherits, integer createContext returns framehandle
native BlzDestroyFrame takes framehandle frame returns nothing
native BlzFrameSetPoint takes framehandle frame, framepointtype point, framehandle relative, framepointtype relativePoint, real x, real y returns nothing
native BlzFrameSetAbsPoint takes framehandle frame, framepointtype point, real x, real y returns nothing
native BlzFrameClearAllPoints takes framehandle frame returns nothing
native BlzFrameSetAllPoints takes framehandle frame, framehandle relative returns nothing
native BlzFrameSetVisible takes framehandle frame, boolean visible returns nothing
native BlzFrameIsVisible takes framehandle frame returns boolean
native BlzGetFrameByName takes string name, integer createContext returns framehandle
native BlzFrameGetName takes framehandle frame returns string
native BlzFrameClick takes framehandle frame returns nothing
native BlzFrameSetText takes framehandle frame, string text returns nothing
native BlzFrameGetText takes framehandle frame returns string
native BlzFrameSetTextSizeLimit takes framehandle frame, integer size returns nothing
native BlzFrameGetTextSizeLimit takes framehandle frame returns integer
native BlzFrameSetTextColor takes framehandle frame, integer color returns nothing
native BlzFrameSetFocus takes framehandle frame, boolean flag returns nothing
native BlzFrameSetModel takes framehandle frame, string modelFile, integer cameraIndex returns nothing
native BlzFrameSetEnable takes framehandle frame, boolean enabled returns nothing
native BlzFrameGetEnable takes framehandle frame returns boolean
native BlzFrameSetAlpha takes framehandle frame, integer alpha returns nothing
native BlzFrameGetAlpha takes framehandle frame returns integer
native BlzFrameSetSpriteAnimate takes framehandle frame, integer primaryProp, integer flags returns nothing
native BlzFrameSetTexture takes framehandle frame, string texFile, integer flag, boolean blend returns nothing
native BlzFrameSetScale takes framehandle frame, real scale returns nothing
native BlzFrameSetTooltip takes framehandle frame, framehandle tooltip returns nothing
native BlzFrameCageMouse takes framehandle frame, boolean enable returns nothing
native BlzFrameSetValue takes framehandle frame, real value returns nothing
native BlzFrameGetValue takes framehandle frame returns real
native BlzFrameSetMinMaxValue takes framehandle frame, real minValue, real maxValue returns nothing
native BlzFrameSetStepSize takes framehandle frame, real stepSize returns nothing
native BlzFrameSetSize takes framehandle frame, real width, real height returns nothing
native BlzFrameSetVertexColor takes framehandle frame, integer color returns nothing
native BlzFrameSetLevel takes framehandle frame, integer level returns nothing
native BlzFrameSetParent takes framehandle frame, framehandle parent returns nothing
native BlzFrameGetParent takes framehandle frame returns framehandle
native BlzFrameGetHeight takes framehandle frame returns real
native BlzFrameGetWidth takes framehandle frame returns real
native BlzFrameSetFont takes framehandle frame, string fileName, real height, integer flags returns nothing
native BlzFrameSetTextAlignment takes framehandle frame, textaligntype vert, textaligntype horz returns nothing

So how do I do things?

As with anything in warcraft 3 world editor there are multiple ways to achieve same result, we will try to achieve our results writing everything from scratch (you'll note that later), so in order to start this way we need this function:

 Code:

native BlzLoadTOCFile takes string TOCFile returns boolean

Note that it is native and is present in common.j so it is available for GUI (via call), JASS, vJASS and Wurst.

As function name states, it is for loading TOC file, TOC file is nothing else as Table Of Contents being a file with .toc extension which looks like this:

 Code:

ourAwesomeFile.fdf

that's it! TOC file stores file paths one at a line, so multiple files would look like this:

 Code:

ourAwesomeFile.fdf
yourAwesomeFile.fdf
myAwesomeFile.fdf

Note that there is no empty line at beginning but there is one at the end of it.

Once you import TOC file into your map we need to tell the game to load it via BlzLoadTOCFile which takes a string, that string is an absolute path to our TOC file (you can see path of file you imported in WorldEditor and for Wurst files are imported accordingly to directory structure within imports directory at root of your project).

Returning to TOC file, entries there represent relative paths to files that are to be loaded so in our example those files should be at same directory level as our TOC file (near them, next to them, with them).

Note that BlzLoadTOCFile returns boolean which is set to true when all files are loaded successfully but if something goes wrong - function will return false which comes in handy during debug.

So where we are now?

Quick answers is nowhere, but we are close to start things!

Create files

Create 2 files, name them MyFiles.toc and MyFrames.fdf, put name of second one into first so it looks like this:

MyFiles.toc

 Code:

myFrames.fdf

MyFrames.fdf is empty for now

Import them into map

For Wurst place them into imports directory within your project

For GUI/JASS/vJASS import them via World Editor and remember paths to them (you may also set paths into something shorter)

Instruct your map to load TOC file

During map initialization we need to run:

 Code:

BlzLoadTOCFile("MyFiles.toc")

BUT we are savvy programmers/mappers so we want what happens with our loading function so:

 Code:

init
 boolean loadingResult = BlzLoadTOCFile("myfiles.toc")
 if loadingResult == false
 BJDebugMsg("SOMETHING WENT WRONG WITH myFiles.toc")
 else
 BJDebugMsg("MyFiles.toc is loaded (files that it points to)")

MyFrames.fdf can be empty, function still returns true

If your function returns false check if you got empty line at the end of MyFrames.toc file and/or path to TOC file.

Still we didn't do anything?

Well, yes but we're about to do some amazing stuff!

This .fdf file, what is it?

FDF files are files that contain Frame Definitions and other stuff related to them, all default things related to GUI (not trigger thingy) in Warcraft 3 is defined in those files, you can view them via Ladik's Casc Viewer under war3.w3mod->ui->framedef, some of those files are loaded by default, some of them are not.

Learn through replication

To not create some weird idea for you to follow let's replicate some of default behaviour but in our way!

We're gonna tackle hero icon in upper left corner because that will touch numerous problems you should be aware of.

HEY, I want to do stuffs and see them right now!

Okay, put this into your MyFrames.fdf:

 Code:

Frame "SPRITE" "WarCraftIIILogo" {
 BackgroundArt "UI\Glues\MainMenu\WarCraftIIILogo\WarCraftIIILogo.mdl",
 }

And update your map code to this:

 Code:

init
 boolean loadingResult = BlzLoadTOCFile("myfiles.toc")
 if loadingResult == false
 BJDebugMsg("SOMETHING WENT WRONG WITH myFiles.toc")
 else
 BJDebugMsg("MyFiles.toc is loaded (files that it points to)")


 framehandle logo = BlzCreateFrame("WarCraftIIILogo", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameClearAllPoints(logo)
 BlzFrameSetPoint(logo, FRAMEPOINT_TOPLEFT, BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), FRAMEPOINT_TOPLEFT, 0, 0)

And voila we got something like this:

#### TUTOR01

Anchorist

Sep 8, 2019

Quick breakdown:

In FDF file we've created Frame Definition which consists of:

 Code:

Frame(object type) "SPRITE"(frameType) "WarcraftIIILogo"(objectName) {
 BackgroundArt(Property Name) "UI\Glues\MainMenu\WarCraftIIILogo\WarCraftIIILogo.mdl",(Property Value)
}

In our map code:

 Code:

framehandle logo = BlzCreateFrame("WarCraftIIILogo"(objectName), BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
BlzFrameClearAllPoints(logo) // Clears all point settings it may have
BlzFrameSetPoint(logo, FRAMEPOINT_TOPLEFT, BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), FRAMEPOINT_TOPLEFT, 0, 0) // Sets one point (top-left) relative to top-left point of other frame (that exists)


 Last edited: Sep 9, 2019

#### yxq1122

#### yxq1122

 Level 8

JoinedJun 5, 2018Messages188

Resources: 3

#### Resources
Tools 3

Seems to be not completed.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Suspect this might be too simple but I can see it being argued otherwise.

Some changes I would consider mandatory however.

1. Use jass tags over code tags for syntax highlighting

2. The lists of natives/constants should be put in hidden tags to save space

3. I am not sure if using wurst in your example is a good choice. To you and me, the syntax difference is not big but for the sake of a more universal tutorial I think vjass, jass or lua as they are the natively supported languages.

4. Indention would be nice but not required

Graveyarded as you are not around to update it. Poke if you come back and decide to do so and I'll put it right back.

 You must log in or register to reply here.


### Similar threads

Locked

[JASS]
 UI - Frame


Sakata Gintoki

Feb 28, 2020

Triggers & Scripts

Replies2Views2K

Feb 29, 2020
Sakata Gintoki

 UI: Positionate Frames


Tasyen

May 29, 2019

Scripting Tutorials

Replies25Views19K

Feb 11, 2026
Wareditor

Locked

 UI Utils (one-stop solution for UI business)


Kazeon

Jun 14, 2019

Advanced Scripting

Replies48Views15K

Feb 17, 2020
Kazeon

Locked

 Local Frame


Nikita17

Sep 10, 2023

Triggers & Scripts

Replies8Views2K

Sep 24, 2023
emperor_d3st

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

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top