---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-change-lumber-text.315855/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Change Lumber Text
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-change-lumber-text.315855/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

This is an example showing how to change the shown text in the lumber box at the top of the screen, using jass.

First we need to know how the frame showing the lumber text is called. It is called "ResourceBarLumberText".

After one knows the frames name, one gets access to it by using BlzGetFrameByName.

BlzGetFrameByName loads a frame using a string (name) and an integer (creationcontext).

After one got the frame one uses BlzFrameSetText on it, with the text wanted to be shown.

 JASS:

function ChangeLumberTextBox takes nothing returns nothing
 local framehandle fh = BlzGetFrameByName("ResourceBarLumberText" , 0) //Load "ResourceBarLumberText" with creationcontext 0
 call BlzFrameSetText(fh , "Lumber") //Change the text to Lumber
 set fh = null
endfunction

After using/calling ChangeLumberTextBox() your screen should look kinda like that.

One should use ChangeLumberTextBox() after map init finished.

As soon the local player needs to update the lumber text, your custom text will be lost.

One could also change the text shown in the other resourceboxes, By using a different name inside BlzGetFrameByName.

For Gold use: "ResourceBarGoldText"

Lumber is named "ResourceBarLumberText"

Food is "ResourceBarSupplyText"

For the box showing the Upkeep level (no, low, high) use "ResourceBarUpkeepText"
Using BlzGetFrameByName for only one player can desync the game, hence it is recommended to set it to variable before the GetLocalPlayer Block and using that variable inside the Local Block.

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

[JASS/AI] - UI: List - Default MainFrames (built in CreateAble)

#### Attachments

SetLumberText.w3x

 16.3 KB

 · Views: 445




 Last edited: Oct 3, 2020

#### purparisien

#### purparisien

 Level 43

JoinedJul 17, 2011Messages977

Resources: 157

#### Resources
Models 119Icons 13Skins 2Maps 19Ported Models 4

I like your tutorial. Is it possible to change the Upkeep text this way ? I dont want to use it with the food upkeep.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Yes, one has to use a different name. "ResourceBarUpkeepText", 0

This 4 String-Frames are defined in "UI/FrameDef/UI/ResourceBar.fdf"

#### purparisien

#### purparisien

 Level 43

JoinedJul 17, 2011Messages977

Resources: 157

#### Resources
Models 119Icons 13Skins 2Maps 19Ported Models 4

Thanks you. I nearly finish it. I would like to know how we can switch to another name. Indeed I did this and it doesnt work. What do I miss ?

#### Attachments

1.JPG

 38.8 KB

 · Views: 604



2.JPG

 36.8 KB

 · Views: 554



#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

in image 2 there is no local framehandle fh in the gui trigger.

#### purparisien

#### purparisien

 Level 43

JoinedJul 17, 2011Messages977

Resources: 157

#### Resources
Models 119Icons 13Skins 2Maps 19Ported Models 4

Indeed it works. Thanks. Furthemore, it can be displayed for only one player without crash

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Useful. I'm going to go ahead and approve it, but some feedback for the future if you want to make some updates:

I think it'd be useful to go ahead and post the 4 other strings from ResourceBar.fdf in the main post (for upkeep/gold/whatever). I imagine a lot of people who would find this useful would also want to edit the other resource bars.

You don't have to null framehandle variables--only variables that have extends agent in common.j need to be nulled for the reference count bug

If you're writing it as a function, I would make it take the string as a parameter. It'll make it more dynamic.

 JASS:

function ChangeLumberTextBox takes string text returns nothing
 local framehandle frame = BlzGetFrameByName("ResourceBarLumberText", 0)
 call BlzFrameSetText(frame, text)
endfunction

Do you know if we need to use BlzDestroyFrame(...) on these frames? I don't know if they create a new object each time we call BlzGetFrameByName or not.

A lot of GUI users have been checking out your tutorials. In the past, a lot of tutorials did it your way where they wrote a function and told users to put it in the map header, but GUI users tend to run into all sorts of issues trying to get it to fit their needs. For small functions like this, I'd recommend giving an example inline like this:

Example
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Set LumberText = "Custom Text Here"

Custom script: call BlzFrameSetText(BlzGetFrameByName("ResourceBarLumberText", 0), udg_LumberText)

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

PurgeandFire said:

 Do you know if we need to use

BlzDestroyFrame(...)

on these frames? I don't know if they create a new object each time we call BlzGetFrameByName or not.


Click to expand...

They do not create an new object each time, through they can somehow desync when used in a GetLocalPlayer Block. Also its not recommented to Destroy String Texture frames, cause it crashes the game (hopefully that changes). If one uses BlzDestroyFrame the real frame is destroyed not only the reference (variable).

PurgeandFire said:

 I think it'd be useful to go ahead and post the 4 other strings from ResourceBar.fdf in the main post (for upkeep/gold/whatever). I imagine a lot of people who would find this useful would also want to edit the other resource bars.


Click to expand...

Added a list with their names.

PurgeandFire said:

 A lot of GUI users have been checking out your tutorials. In the past, a lot of tutorials did it your way where they wrote a function and told users to put it in the map header, but GUI users tend to run into all sorts of issues trying to get it to fit their needs. For small functions like this, I'd recommend giving an example inline like this:


Click to expand...

I uploaded a map that has such a GUI-trigger, if you don't mind.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Awesome, that is perfect! Thanks for following-up!

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Is there a way to access individual resource icons, like lumber icon?

I know that by changing position of ResourceFrameBar, you can move all three resource icons at ones. Rather I want to take out the Lumber icon and shift Gold further to the right, to clean up the UI for players that only use Gold and Food.

Editing out the icon via Game Interface is not an option either, because I only want to make a change for a subset of players.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Normaly not, the icon-Frames have no names. Hence no direct access.

In Reforged one can replace the resourcebarframe with a custom one by loading a custom one in before the normal is loaded using BlzLoadTOCFile. But the loading has to be done inside the function config or in the Lua root code.

Step for step:

1: get the default resourcebar.fdf

2: add names for the icons

3: import the custom one into your map

4: write a TOC-File loading the custom resourcebar

5: import that TOC-File

6: inside function config or in the Lua root code use BlzLoadTOCFile onto the custom toc

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Thanks for the response. I wanted to use this, but in the end I figured for my purpose, a bit simpler solution was to move the ResourceBar frame out of view and create new Texture frames containing the Resource Icons.

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Although I simply use the Advanced - Game interface Text fields to achieve this, I have to admit this offers some flexibility.

But I was wondering about the fields to change this text in other parts of the UI, like in an ally unit info panel

These InfoPanels have really caused me a lot of issues, for example I didn't manage to hide the hero attributes on heroes with 0 stats, nor have I managed to hide these resources informations on ally units.

Is this an issue with the context ?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Maybe.

The unit info panel uses CreateContext which are not 0.

Unit Info Panel

The Unit Info Panel the one at the bottom center of the UI when only one unit is selected can be found in the fdf "ui\framedef\ui\simpleinfopanel.fdf". They are all SimpleFrames.

BlzGetFrameByName("SimpleInfoPanelIconDamage",0)

"SimpleInfoPanelIconDamage",1

"SimpleInfoPanelIconArmor",2

"SimpleInfoPanelIconRank",3

"SimpleInfoPanelIconFood",4

"SimpleInfoPanelIconGold",5

This frames are shown/hidden based on the current's units features.

They have Child-Frames with the same names, only the createContext differs.

"InfoPanelIconBackdrop" the image

"InfoPanelIconLevel" the shown techLevel (text)

"InfoPanelIconLabel" Damage/Armor..

"InfoPanelIconValue" the amount

BlzGetFrameByName("InfoPanelIconBackdrop", 0)

would access the iconFrame of Attack 1 (object Editor).

Moving this UnitInfo frames is best done by moving the IconFrames: "InfoPanelIconBackdrop", Because The Texts pos themself relative to the IconFrame and the Container Frames like "SimpleInfoPanelIconDamage" are frequently reposed based on the current Unit's need. With breaking the bound from the Icon to the container one still has the whole functionality but total control over the position.

One can read and write onto this Frames but except for "InfoPanelIconLabel" the game update the displayed content of this frames very often. One could get rid of some of this displayed Texts by using BlzFrameSetFont(frame, "",0,0) onto it, or by moving it away. When one reads this Text one has to remember that the displayed value is a local only thing for anyone having a different unit selected the value will not match, so be careful what you do with that.

All of the UnitInfo-Hero Frames use 6 as CreateContext.

"SimpleInfoPanelIconHero" is the container.

"InfoPanelIconHeroIcon" is the Big Icon showing the primary Attribute.

"SimpleInfoPanelIconHeroText" is an additional logical container Frame for the Attribute String-Frames.

The visibility of the container Frames are a way to know that the local player has selected one hero only. Or a good Parent for custom SimpleFrames displaying something for heroes only.

"InfoPanelIconHeroStrengthLabel"

"InfoPanelIconHeroStrengthValue"

"InfoPanelIconHeroAgilityLabel"

"InfoPanelIconHeroAgilityValue"

"InfoPanelIconHeroIntellectLabel"

"InfoPanelIconHeroIntellectValue"

The alliance info when having one allied building selected, all of them use CreateContext 7.

"SimpleInfoPanelIconAlly", 7

"InfoPanelIconAllyTitle"

"InfoPanelIconAllyGoldIcon"

"InfoPanelIconAllyGoldValue"

"InfoPanelIconAllyWoodIcon"

"InfoPanelIconAllyWoodValue"

"InfoPanelIconAllyFoodIcon"

"InfoPanelIconAllyFoodValue"

"InfoPanelIconAllyUpkeep"

The Textures are anchored to the previous Textures and the Strings next to the Textures. Except for "InfoPanelIconAllyTitle" & "InfoPanelIconAllyGoldIcon" this are anchored to "SimpleInfoPanelIconAlly".

This are other SimpleFrames in that unit info frame.

"SimpleInfoPanelUnitDetail", 0

"SimpleNameValue", 0 The name at the top

"SimpleHeroLevelBar", 0 Exp bar has values from 0 to 1.0

"SimpleProgressIndicator", 0 Remaing Time bar from 0 to 1.0

"SimpleBuildTimeIndicator", 0

"SimpleBuildingActionLabel", 0

"SimpleUnitStatsPanel", 0 parent of SimpleClassValue

"SimpleClassValue", 0 The displayed Text below the name (in the bar).

There are also other Frames shown at the bottom center UI, instead of "SimpleInfoPanelUnitDetail", they and "SimpleInfoPanelUnitDetail" share one Parent.

"SimpleInfoPanelBuildingDetail", 1

This Frame is shown when one unit is selected and it trains something.

"SimpleBuildingNameValue", 1

"SimpleBuildingDescriptionValue", 1

"SimpleBuildTimeIndicator", 1

"SimpleBuildingActionLabel", 1

"SimpleBuildQueueBackdropFrame", 1

"SimpleBuildQueueBackdrop", 1

"SimpleInfoPanelCargoDetail", 2

This Frame is shown when one unit is selected and it transports units.

"SimpleHoldNameValue", 2

"SimpleHoldDescriptionValue", 2

"SimpleInfoPanelItemDetail", 3

This Frame is shown when an item on the ground is selected.

"SimpleItemNameValue", 3

"SimpleItemDescriptionValue", 3

"SimpleInfoPanelDestructableDetail", 4

This Frame is shown when a destructable is selected (like a Gate, cage etc).

"SimpleDestructableNameValue", 4

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

OK. Dear UI Master @Tasyen, this worked wonders.

That information allowed me to get exactly what I wanted : Hiding the Display of Hero Attributes on units I had to set as Hero for some other reasons, but with 0 attributes points, and hide the upkeep Icon from allied Towers :

Of course, I avoided to set the visibility of these frames, because it bugs or crashes the game as you explained in UI - Simpleframes

So I simply moved them off screen, and the result is perfect !!!

Here is the part of my code that uses some of this information you provided :

 Lua:

------
 local fh=nil
 if IsUnitType(u, TOWER) then
 if (pn==on) then -- We only show the Range Indicator for the tower owner
 local range,scale=RangeIndicator[on],GetUnitAcquireRange(u)/400
 SetUnitFlyHeight(range, 258.00-BlzGetUnitZ(u), 0)
 SetUnitPosition(range, GetUnitX(u), GetUnitY(u))
 SetUnitScale(range, scale, scale, scale)
 ShowUnit(range, true)
 else -- Hide "Macadamia 2020" (Upkeep icon) from allied towers
 fh=BlzGetFrameByName("InfoPanelIconAllyUpkeep",7)
 BlzFrameClearAllPoints(fh)
 BlzFrameSetAbsPoint(fh, BOTTOM, 0.0, -0.24)
 end
 elseif IsUnitType(u, BALL) or IsUnitType(u, HEALER) then -- Hiding Hero attributes on Balls and Healer
 fh=BlzGetFrameByName("SimpleInfoPanelIconHero", 6)
 BlzFrameClearAllPoints(fh)
 BlzFrameSetAbsPoint(fh, BOTTOM, 0.0, -0.24)
 end
------

Thank you so much again for your precious help !!!

EDIT :

Ok what I did was not actually 100% right :

As I wanted to always hide the upkeep icon, I made it in my UISetup function.

I also decided, for optimization sake, not store the HeroAttribute frame in a global variable and clear all points at map init rather than accessing the frame and clearing its points every time :

 Lua:

function UISetup()
 -- Global Frames
 AttribFrame=BlzGetFrameByName("SimpleInfoPanelIconHero", 6)
 BlzFrameClearAllPoints(AttribFrame)
 -- Upkeep Icon Frame
 local fh=BlzGetFrameByName("InfoPanelIconAllyUpkeep",7)
 BlzFrameClearAllPoints(fh)
 BlzFrameSetAbsPoint(fh, BOTTOM, 0.0, -0.24)
 -- System buttons
------

Next, I forgot the hero attribute frame was not recreated everytime a hero is selected, and that it should only be moved offscreen for the selecting player, but also put back in place when a hero with non 0 attributes is selected (BUILDER unit type in my case).

So here's the updated part in my trigger managing selections :

 Lua:

------
 if IsUnitType(u, TOWER) then
 if (pn==on) then -- We only show the Range Indicator for the tower owner
 local range,scale=RangeIndicator[on],GetUnitAcquireRange(u)/400
 SetUnitFlyHeight(range, 258.00-BlzGetUnitZ(u), 0)
 SetUnitPosition(range, GetUnitX(u), GetUnitY(u))
 SetUnitScale(range, scale, scale, scale)
 ShowUnit(range, true)
 end
 elseif IsUnitType(u, BALL) or IsUnitType(u, HEALER) and (pn==LocalPn) then -- Hiding Hero attributes frame on Balls and Tower Healer
 BlzFrameSetAbsPoint(AttribFrame, BOTTOM, 0.0, -0.24)
 elseif IsUnitType(u, BUILDER) and (pn==LocalPn) then -- Restoring Hero attributes frame
 BlzFrameSetAbsPoint(AttribFrame, BOTTOM, 0.16, 0.02)
 end
------

It now works seamlessly !!!

Side Note :for some reason, operations on frame do not seem to work instantly, on every visibility / moving of frames, there seems to be a delay as I can clearly see the attribute frames disappear before my very eyes (same thing applies to custom HP Bars)

EDIT 2 :

New natives solved the fact that I needed to make the second builder a hero, so I now only need to hide the Hero Attributes on Balls (they need to be heroes as I revive them for the sake of recycling)


 Last edited: Jun 3, 2020

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Hello Tasyen,

Any idea how to change the tooltip that belongs to "ResourceBarUpkeepText"?

I couldn't find any lead in CascView.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Frame-wise I only know how to access the Frame that shows the tooltip as soon the mouse hovers the Upkeep. That mouse Listener can be hidden to prevent the players to ever see that tooltip. Which is get over the Child Native added in 1.32.x.

 Lua:

BlzFrameSetVisible(BlzFrameGetChild(BlzGetFrameByName("ResourceBarFrame", 0), 2), false)

I recomment to use that line at 0s.

I wrote a list of such childFrames in a hidden section in UI: OriginFrames | HIVE (hiveworkshop.com)

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Works great, thanks!

 You must log in or register to reply here.


### Similar threads

 UI: Creating a Bar


Tasyen

Jun 12, 2019

Scripting Tutorials

Replies38Views18K

Oct 2, 2025
KenArok

 UI: Creating a Cam control


Tasyen

Jun 1, 2019

Scripting Tutorials

Replies24Views10K

Mar 4, 2021
ESTO

 UI: EditBox - Text Input


Tasyen

Jun 12, 2019

Scripting Tutorials

Replies15Views10K

Apr 6, 2023
Tasyen

 UI: Positionate Frames


Tasyen

May 29, 2019

Scripting Tutorials

Replies25Views19K

Feb 11, 2026
Wareditor

 UI: TEXTAREA the scrolling Text Frame


Tasyen

Sep 20, 2019

Scripting Tutorials

Replies20Views9K

Dec 6, 2021
Seago

S

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