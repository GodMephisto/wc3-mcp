---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-the-concept-of-parent-frames.320238/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI the Concept of Parent Frames
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-the-concept-of-parent-frames.320238/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

###
 Introduction

Frame - Parent is a powerful and useful concept of warcraft 3's UI-Frame System.

Every frame we can create has a parent frame. Any frame can have any amount of child frames. In the same time any child frame also can have any amount of child frames.

natives provided to get and set the current parent of a frame during the game.

native BlzFrameSetParent takes framehandle frame, framehandle parent returns nothingnative BlzFrameGetParent takes framehandle frame returns framehandle

A parent is also set when creating a new frame, as all create frame natives require an owner/parent.

In many examples custom Frames are created as child of GAME_UI which is done to simple down the example. But it is ineffective to create all custom Frames for GAME_UI, if one has Frames that are logical connected it would make more sense to collecte them as childs of one Frame.

FrameA child of Game_UI

FrameB child of FrameA

FrameC child of FrameA

FrameD child of FrameA

FrameA is the parent an logical container of FrameB, FrameC and FrameD.

For Frames "FRAME" could be used.

For SimpleFrames "SIMPLEFRAME".

Both can be created with BlzCreateFrameByType

###
 Why I should bother this concept?

The FrameParent concept simples down a lot of stuff you might want to accomplish. Want to create a new option in menu, create your Frame for ("EscMenuMainPanel", 0). Now the frame requires the visibility of ("EscMenuMainPanel", 0) and you can focus on the behaviour of the frame itself instead of handling the visiblity which can be quite complicated when injecting into the normal warcraft 3 UI. In my opinion the most powerful effect of frame-parentship.

Want to add a new UnitInfo (if you find a place for it on the screen) create it for ("SimpleInfoPanelUnitDetail", 0) then it only will be visible when the local player has only one unit selected. Through this is a bit nah, cause you need to know the selected unit to update the displayed data (in such unit cases).

If you wouldn't use this parent concept for visibility you would have to find out with triggers/code that the local player has only one unit selected, or is inside the menu for the example and also not in cinematic mode.​

###
 What are the Effects of Parent-Child

A child frame can only be visible when it's parent frame is visible. BlzFrameIsVisible also checks for the visibility of the parent frame. New created frames copy scale and alpha from their parent on default. This values can differ for the children when they are set (for the child frame) after the child frame was added to /created for the parent frame.

BlzFrameSetScale

BlzFrameSetAlpha

When changing Parents the alpha value of the new Parent is copied.​
BlzFrameSetLevel

Frames created for a parent frame also have the same Level.​

A child tends to be visible above and is prefered by mouse events over its parent. SimpleFrames can break this rule with BlzFrameSetLevel.​

###
 2 Types of children

I split childFrames in 2 Types loose children and functional children.

Functional child frames can only be defined and created inside fdf such children have a strong connection to their parent and perform some functionality for the parentFrame hence the name. Although earlier I had no good name for them, I called them strong binded/linked/connected child. An example of such functional child frame is ButtonText "ScriptDialogButtonText",

This childFrame styles the text for it's parent frame "ScriptDialogButton". Functional childFrames copy even more from the ParentFrame, their position is automatically handled and they copy the enable state.

Loose childFrames do not perform any functionality for their parent. This children can be created in fdf or with code. This code creates a new loose child frame for parentFrame. BlzCreateFrame("frameName", parentFrame, 0, 0) The position of a loose childFrame is on default independent from it's parent, it does not even require the parent frame to be placed on the screen at all.

That behaviour can be used to create Frames with a higher FrameLevel then their type supports: Create A BUTTON set its FrameLevel and create the wanted Frames using him as parent for example TEXT-Frames.

Want to show/hide a whole bunch of Frames collect them under one ParentFrame and change the ParentFrame's visibility. Now it only takes one line to toggle the visibility.​

###
 Parent Limitations?

SimpleFrames can not be children nor parent of Frames. Frames can not be Children/Parent of SimpleFrames. Some might now say: "but I created a BUTTON for SIMPLEFRAME in fdf or, I created SIMPLEFRAME for a BUTTON during the game."

Yes, you did but on a more accurate inspection, by comparing HandleIds of the parent of the new frame and the wanted/should be parent one might see they do not match, in both cases you see the handleId of another frame, I call that a substitute parent.

When that happens one might at first wonder why visibility and other parent behaviours do not apply. But the reason: the wanted parent is not the real parent and so it does not copy the visibility/alpha... .

In 1.31 BlzFrameSetParent can bug sometimes (it speeds up the animation of "SPRITE" and can empower the glowing of "HIGHLIGHT" when changing the parent for this frames directly).​

###
 Example

This is a Lua code creating a clickable Button that is only visible while the player is inside the Menu (F10), the code executes itself when insert into a map in Lua mode.

When the button is clicked the message "action in Menu" is printed onto the screen.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 local buttonFrame = BlzCreateFrameByType("BUTTON", "", BlzGetFrameByName("EscMenuMainPanel", 0), "ScoreScreenTabButtonTemplate", 0)
 local iconFrame = BlzCreateFrameByType("BACKDROP", "", buttonFrame, "", 0)
 BlzFrameSetAllPoints(iconFrame, buttonFrame)
 BlzFrameSetSize(buttonFrame, 0.03, 0.03)
 BlzFrameSetAbsPoint(buttonFrame, FRAMEPOINT_TOPLEFT, 0.1, 0.45)
 BlzFrameSetTexture(iconFrame, "ReplaceableTextures\\WorldEditUI\\Doodad-Cinematic.blp", 0, true)
 local trigger = CreateTrigger()
 TriggerAddAction(trigger, function()
 print("action in Menu")
 end)
 BlzTriggerRegisterFrameEvent(trigger, buttonFrame, FRAMEEVENT_CONTROL_CLICK)
 end
end

Although this is a bit vague cause currently it also is visible while choosing to end game and not visible while being inside options. If you plan on doing something like that you have to find out more about that frames in detail and which is the one suited for your wanted goal. This menu frames are in "ui/framedef/ui" when looking with cascview.

This was written in mind for warcraft 3 V1.31.1.

Edited: Replaced selfexecution with a less problematic approach​

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


 Last edited by a moderator: Oct 10, 2020

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Thanks for the information.

Well I thought I would have no more question, but this tutorial brings a new one :

You explain that :

Tasyen said:

 Frames of the SIMPLEFRAME world and FRAME world can not be Children/Parent of each other


Click to expand...

I understand that if we do (and yes I tried and this is correct), substitute parents will be created.

Knowing this, I have an important question :

I have created a frame by type with the type "FRAME" with the World frame as an owner/parent, and I have used it as a Parent for all my Backdrop that are background and border textures. As a result, not only are my background-textures-frames able to sit behind origin frames, but I can easily hide them all at once - for example in cases of defeats or victories by hiding this custom Parent frame for the local player.

But how can we do the same with one or more simple frames, like for example these nice Bars from your tutorial (  ). I cannot use another Simpleframe or Frame or World frame, so what kind of frame can I use, especially if I want that frame to have no visuals (in other words show nothing ingame even when set as visible) ?

Currently, I am considering simply hiding the Bars one by one, but It might become problematic when doing this late in the game, if I refer to your simple frames tutorial :

"Warning

The game does not like it, if you use this frame natives onto String/Texture, it will crash the game:

BlzFrameSetVisible

BlzFrameIsVisible

BlzFrameSetAlpha

BlzFrameSetLevel"

Any suggestion ?

Thanks (yes again, I know ^^) and take care !

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Macadamia said:

 I have created a frame by type with the type "FRAME" with the World frame as an owner/parent, and I have used it as a Parent for all my Backdrop that are background and border textures. As a result, not only are my background-textures-frames able to sit behind origin frames, but I can easily hide them all at once - for example in cases of defeats or victories by hiding this custom Parent frame for the local player.

But how can we do the same with one or more simple frames, like for example these nice Bars from your tutorial (  ). I cannot use another Simpleframe or Frame or World frame, so what kind of frame can I use, especially if I want that frame to have no visuals (in other words show nothing ingame even when set as visible) ?


Click to expand...

In code you can create a "SIMPLEFRAME" instead of "FRAME" with BlzCreateFrameByType if you need an additional logical container. BlzCreateFrameByType("SIMPLEFRAME", "", BlzGetFrameByName("ConsoleUI", 0), "", 0)

example Lua code

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 local parent = BlzCreateFrameByType("SIMPLEFRAME", "", BlzGetFrameByName("ConsoleUI", 0), "", 0)
 local frameA = BlzCreateSimpleFrame("SimpleInfoPanelIconDamage", parent, 10)
 local frameB = BlzCreateSimpleFrame("SimpleInfoPanelIconDamage", parent, 11)
 local frameC = BlzCreateSimpleFrame("SimpleInfoPanelIconDamage", parent, 12)
 BlzFrameSetAbsPoint(frameA, FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetSize(frameA, 0.1, 0.1)
 BlzFrameSetAbsPoint(frameB, FRAMEPOINT_CENTER, 0.4, 0.34)
 BlzFrameSetSize(frameB, 0.1, 0.1)
 BlzFrameSetAbsPoint(frameC, FRAMEPOINT_CENTER, 0.4, 0.38)
 BlzFrameSetSize(frameC, 0.1, 0.1)
 TimerStart(CreateTimer(), 1, true, function()
 BlzFrameSetVisible(parent, not BlzFrameIsVisible(parent))
 end)
 print("done")
 end
end

Macadamia said:

 Currently, I am considering simply hiding the Bars one by one, but It might become problematic when doing this late in the game, if I refer to your simple frames tutorial :

"Warning

The game does not like it, if you use this frame natives onto String/Texture, it will crash the game:

BlzFrameSetVisible

BlzFrameIsVisible

BlzFrameSetAlpha

BlzFrameSetLevel"

Any suggestion ?


Click to expand...

Then you need one SIMPLEFRAME parent that you can hide/show/alpha/Level to affect only the wanted String / Texture.

Edited: Replaced selfexecution with a less problematic approach


 Last edited: Sep 22, 2020

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Thanks,

The API is somewhat sometimes behaving unexpectedly.

So far what I do to get things working is this :

I create both a FRAME and a SIMPLEFRAME as global variables called Parent and BarsParent at Map Init, with the world frame as owner.

Then I use them, as you might expect :

- Parent as owner for all my custom frames, the borders-backgrounds, the Vertical Zoom Slider

- BarsParent as the owner of my HP Bars :

 Lua:

function HPBars()
 local p=nil

 Bar={}
 BlzLoadTOCFile("ui\\HPbar.toc")
 BarsParent=BlzCreateFrameByType("SIMPLEFRAME", "", BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0),"", 0)
 for i=1,4 do
 if not(PlayerAway[i]) then
 p=Player(i-1)
 Bar[i]=BlzCreateSimpleFrame("HPBarEx", BarsParent, i)
 BlzFrameSetAbsPoint(Bar[i], FRAMEPOINT_LEFT, -0.133, 0.13)
 BlzFrameSetValue(BlzGetFrameByName("HPBarEx",i), 0)
 BlzFrameSetText(BlzGetFrameByName("HPBarExText",i), " ")
 if (i~=LocalPn) then
 BlzFrameSetVisible(Bar[i], false)
 end
 end
 end
end

and :

 Lua:

function UISetup()
 local fh=nil
.................................................................... other stuff
 local miniMap=nil

 Parent=BlzCreateFrameByType("FRAME", "", BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0),"", 0)

 -- Minimap
 miniMap=BlzGetFrameByName("MiniMapFrame", 0)
 BlzFrameSetVisible(miniMap, true)
 BlzFrameClearAllPoints(miniMap)
 BlzFrameSetAbsPoint(miniMap, FRAMEPOINT_TOPLEFT, 0.0, 0.60)
 BlzFrameSetAbsPoint(miniMap, FRAMEPOINT_BOTTOMRIGHT, 0.14, 0.46)
 fh=BlzGetFrameByName("MinimapSignalButton", 0)
 BlzFrameSetVisible(BlzFrameGetParent(fh), true)
 BlzFrameSetVisible(fh, true)
 BlzFrameClearAllPoints(fh)
 BlzFrameSetPoint(fh, FRAMEPOINT_LEFT, miniMap, FRAMEPOINT_RIGHT, 0.0, 0.0)
 fh=BlzGetFrameByName("MiniMapTerrainButton", 0)
 BlzFrameSetVisible(fh, false)
 fh=BlzGetFrameByName("MiniMapAllyButton", 0)
 BlzFrameSetVisible(fh, false)
 fh=BlzGetFrameByName("MiniMapCreepButton", 0)
 BlzFrameSetVisible(fh, false)
 fh=BlzGetFrameByName("FormationButton", 0)
 BlzFrameSetVisible(fh, false)

 -- Minimap border
 fh=BlzCreateFrameByType("BACKDROP", "MinimapBorder", miniMap, "", 0)
 BlzFrameSetAbsPoint(fh, FRAMEPOINT_TOPLEFT, 0.0, 0.60)
 BlzFrameSetAbsPoint(fh, FRAMEPOINT_BOTTOMRIGHT, 0.14, 0.46)
 BlzFrameSetTexture(fh, "ui\\minimap-border.blp",0, true)

 -- Resources bar
 fh=BlzGetFrameByName("ResourceBarFrame", 0)
 BlzFrameSetVisible(fh, true)
 BlzFrameClearAllPoints(fh)
 BlzFrameSetPoint(fh, FRAMEPOINT_TOPLEFT, miniMap, FRAMEPOINT_BOTTOMRIGHT, 0.01, 0.14)

 -- Resources bar border
 fh=BlzCreateFrameByType("BACKDROP", "ResourcesBorder", Parent, "", 0)
 BlzFrameSetSize(fh, 0.33828125, 0.0218)
 BlzFrameSetAbsPoint(fh, FRAMEPOINT_TOPLEFT, 0.15, 0.60)
 BlzFrameSetTexture(fh, "ui\\resourcesbar-border.blp",0, true)

.................................................................... other stuff

 -- Command buttons border
 fh=BlzCreateFrameByType("BACKDROP", "CommandBorder", Parent, "", 0)
 BlzFrameSetSize(fh, 0.1745, 0.131)
 BlzFrameSetAbsPoint(fh, FRAMEPOINT_BOTTOMLEFT, 0.218, 0.0)
 BlzFrameSetTexture(fh, "ui\\commandbar-border.blp",0, true)

.................................................................... other stuff

 -- InfoPanel border
 fh=BlzCreateFrameByType("BACKDROP", "CommandBorder", Parent, "", 0)
 BlzFrameSetSize(fh, 0.192, 0.12)
 BlzFrameSetAbsPoint(fh, FRAMEPOINT_BOTTOMLEFT, 0.022, 0.0)
 BlzFrameSetTexture(fh, "ui\\infopanel-border.blp",0, true)

.................................................................... other stuff

And hding Parent and BarsParent actually works a treat, no substitute parent was created for the Bars surprisingly.

Now some observations :

- If I try to use BarsParent as owner for my border frames / Vertical Zoom Slider, they don't show up

- If I try to use Parent as owner of the HP Bars, they can't be hidden with Parent (substitute parents are created)

In other words we need a FRAME parent for BACKDROP, SLIDER, TEXT and other normal frame types, and a SIMPLEFRAME parent for simpleframes.

Maybe you should then specify this in your tutorial (unless I made another mistake) to explain that these frames you mentioned can not be a parent child to each other , but they can be a parent child of the same frame type (in other words, your "each other" is not inclusive, if that makes sense).

Note that this is this little detail that lead me to confusion, I thought they could not be parents/children of the same type either.

Anyways, this way no substitute parents and everything works fine.... with one exception that I don't understand :

As you can see in my code, I didn't use Parent for the minimap border. The reason is, if I do this it does not show at all !!!

Whereas using the minimap as a parent, it shows nicely.

- So does it mean the minimap conflicts / collides with the minimap border ?

- How come the border manages to show behind the minimap ? This is in complete contradiction with your tutorial :

Tasyen said:

 A child tends to be visible above and is prefered by mouse events over its parent.


Click to expand...

I hope my questions and their awnsers will help people confronted to the same issues as me.

Again, @Tasyen , thanks for your great tutorials !!!


 Last edited: Apr 19, 2020

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Macadamia said:

 As you can see in my code, I didn't use Parent for the minimap border. The reason is, if I do this it does not show at all !!!

Whereas using the minimap as a parent, it shows nicely.

- So does it mean the minimap conflicts / collides with the minimap border ?

- How come the border manages to show behind the minimap ? This is in complete contradiction with your tutorial :

Click to expand...

How you did that anyway?

When you use World as parent the BACKDROP is behind the minimap, if it also has the same color as your ground (on minimap) you won't see it even with a different color it is difficult to see through the transparent minimap. Otherwise when you use Minimap as parent the frame is above it.

You could mean somethingelse then I first thought. Actually there is also FrameLevel that has a impact on this Parent/Child Order but I have that already in a different tutorial, But somehow it would belong here.

Following only applies to Frames, not to SimpleFrames.

Each ChildFrame of a Frame is an own Layer. The Child with the highest Level is the one on top of its brothers and sisters. A child of such a child is also a new Layer which is above the parent but below the uncle with a higher Level then his parent.

Wana display that with this sketch. There are 3 Childs of GameUI Each starts an own path the children of this 3 are always in their ancestor's path. For the image that means as many offsprings B will have, they will be always below A, which is below the offspring of A. C and offspring C is below B. But all of them are above Game_UI cause they are direct children or child's child.

By using this rule it is not wierd that offsprings of World are lower than children of GameUI.

But again this only applies to non-SimpleFrames. SimpleFrames are more simple in that way.

Macadamia said:

 Maybe you should then specify this in your tutorial (unless I made another mistake) to explain that these frames you mentioned can not be a parent child to each other , but they can be a parent child of the same frame type (in other words, your "each other" is not inclusive, if that makes sense).

Note that this is this little detail that lead me to confusion, I thought they could not be parents/children of the same type either.


Click to expand...

Sry for that, updated.


 Last edited: Jul 27, 2020

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Hello again !

Tasyen said:

 When you use World as parent the BACKDROP is behind the minimap, if it also has the same color as your ground (on minimap) you won't see it even with a different color it is difficult to see through the transparent minimap. Otherwise when you use Minimap as parent the frame is above it.


Click to expand...

Well It was not hidden behind the minimap, as It is larger and then I would see the borders.

In fact the minimap border I use is different from the others, because the others have a background, whereas the minimap border is just a border with a completely transparent inside :

 Unfortunately we can not clearly see the transparency here...

The problem was that I didn't even know if it was there as it did not show at all

The whole reason I used World as parent in the first place was because my other borders (Resource bar / command buttons and InfoPanel) are full textures, with a border and a semi-transparent texture based on the tooltip background (Did that under Photoshop) :

When I used the typical Console UI as a parent when creating my borders frames, as they were BACKDROP, they were displaying on TOP of the frames, making the icons hardly visible, (but still clickable for some reason), and I was hoping to put them behind, because backgrounds are meant to be behind

Using World as owner of the frame i called "Parent" solved this, except for Minimap for reasons I still don't understand.

All borders worked fine, but the minimap border you can see above didn't.

As nothing really differs from these textures apart from the 100% transparency inside minimap-border, (others have the tooltip background transparency that is near 50% I suppose), I think there is something special with the minimap frame.

So using world as parent of Parent allowed to put all border behind frames, and for minimap border, using the minimap origin frame as parent worked.

Anyways, thanks for your sketch, it is very helpful, and if one day you have the time maybe add something like that to this tutorial, this would help a lot of people trying to learn. When you start to get used to the UI, it kind of makes sense, but we have to admit it is not intuitive at all.

Thanks for updating your tutorial, it is no more confusing, I hope it will help a lot of people.

Never give up, the work you do is amazing, I am sure many people now and in the future will be thankful for all that you do.

By the way, off topic, but I realized you have made amazing Custom Stats systems over the years (I was not back yet !), and this is impressive.

I already consider studying them to try to implement my own system with custom stats (there will only be 4 though will have to think about it) for my map.

If people like you wouldn't spend so much time trying to teach the community, we would never make such progress !!!!


 Last edited: Apr 19, 2020

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

I was wondering if it possible to toggle frame visibility based on unit selection and visibility of the command card.

In 1.31 it was possible to base frame visibility on the command card if a frame's parent was 'ORIGIN_FRAME_COMMAND_BUTTON'. In 1.32 this no longer works. Setting the frame's parent to 'CommandButton_0' or 'CommandBarFrame' doesn't work either.

Toggling frame visibility by unit selection doesn't work either. When a selected unit dies, the unit info screen and portrait will disappear entirely, but any frames with ORIGIN_FRAME_PORTRAIT as its parent will always stay visible no matter what.

I'm guessing there is no other solution than using triggers, but if there is a cleaner way I'd love to know.

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Hello there !

Thats a very interesting question, and if I could automate the hiding of the value on my custom HP Bars when the portrait disappears, I would also be really pleased.

Can't really do tests at the moment as I am still drowning into Jass2 to Lua conversion for my map,but I was thinking :

- rather than using the portrait or the command button card as a parent, have you tried using the parent of the "SimpleInfoPanelUnitDetail" origin frame ?

I ask this because the only way to toggle the visibility of that frame actively is by changing its parent's visibility.

Maybe using this parent as a parent for your frames would do the job ?

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

I'm afraid it didn't work. SimpleInfoPanelUnitDetail is never visibile, and it's parent is always visible.

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

I have done several tests myself, and indeed there seems to be no hope for automatic hiding using parents.

I have the feeling the game engine uses completely different mean to hide frames, and thus when a portrait or any other frame is not shown anymore ingame (best example was your first idea, the portrait), it doesn't seem to affect children at all for some reasons.

This leads me to believe that triggers are the only answer...

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

("SimpleInfoPanelUnitDetail", 0) is visible when the local player has selected one unit only that is not training, nor transporting nor being bulit.

("SimpleInfoPanelIconHero", 6) when that unit is also a hero

("SimpleInfoPanelIconAlly", 7) when that unit is an allied building​
("SimpleInfoPanelCargoDetail", 2) transporter (with content)

("SimpleInfoPanelBuildingDetail", 1) active trainer

("SimpleInfoPanelItemDetail", 3) selects an item on the ground

("SimpleInfoPanelDestructableDetail", 4) selects an gate/cage etc

sadly the group selection frame is unknown.

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

@Tasyen : thank you for these precisions , would you happen to know the reason of the problem we were mentioning ?

I mean, when we set the portrait origin frame as a parent for a custom frame, this frame doesn't lose its visibility when the portrait no longer is visible. Does it mean, as I suspected, that the portrait does not in fact get its visibility changed by the game engine ?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The game probably just swaps the shown model for the portrait frame instead of changing visibility. I mean it is hardly possible to have nothing selected in warcraft 3, except the current selected unit is killed/hidden. Then there are cinematic messages also needing that face. To much work to change visibility for something that has to be there for 99% of time.

Command Buttons are similiar the Parent is visible (when not in cinematic mode). Based on the need the individual buttons are visible or hidden.

For command buttons one also needs to know this things belong to SimpleFrames, if you wana use it as parent you also need to create SimpleFrames.

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Thank you for this. So yes we are constrained to use triggers. Not a big deal to be honest, but thats important information to know

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Indeed the Unit Info and Portrait frames will be displayed 99% of the time. Basically, only when the selected unit dies it will disappear, until you select a different unit. Not really worth it to put in too much effort.

For the Command Card, it would properly hide in 1.31 (and in 1.32 it might as well, if the frame visibility logic hasn't been fundamentally changed). In 1.31 the Command Card origin frame and any child frames would be hidden entirely in the following situations:

-Having no unit selected

-Selecting units of another player

-Clicking on a spell that requires a target (Command Card disappears, and only a cancel button is visible)

This will occur often. I will test a bit more, and if I happen to find a solution I will post it in this topic.

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

@Tasyen : I suppose this is the best place to ask this question:

It seems I don't yet understand how the relation between children and parent frames works.

I wanted to move the Frame that displays messages such as "Not enough gold." or "Completed : *building or upgrade*" etc..

As you know we can not access this frame directly. But we know it is a child (not accessible with the new natives, probably because it is a String frame). of Console UI.

So I moved it as I needed by moving Console UI's BOTTOM with -0.05 as y value.

Of course this also would move ORIGIN_FRAME_UNIT_MSG, and, as I wanted to avoid having to move it because how sensitive it is with being moved,

I tried to change its parent to be ORIGIN_FRAME_GAME_UI directly instead of ConsoleUI.

So I did that just before moving the Console UI slightly, but it still moved the ORIGIN_FRAME_UNIT_MSG.

Does it mean we cannot change parents of origin frames ?

It seems I am condemned to move ORIGIN_FRAME_UNIT_MSG back, or maybe I need to introduce some timer after changing the parent ?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Parentship and position on screen are 2 independent things.

Parentship controls visibility, alpha, scale... but not position, except for functional children. They get a position fitting their function.

Edit: Parents can enable/restrict positions.

For Loose children the position was manualy set somewhere. Hence a childFrame can be at the bottom Right of the screen while the parent might not take any position on the screen at all, if you can not access such a Loose childFrame you can not unlink their position.

I think GAME_UI can not have SimpleFrames as children.


 Last edited: Aug 19, 2020

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

OK, this all makes sense now.

Thanks for these precise explanations, it really helps understanding what went wrong in all my attempts.

I also take note about GameUI and SimpleFrames.

Thanks for your precious help, once again

#### watterboy

#### watterboy

 Level 3

JoinedJan 20, 2020Messages48

Video tutorials for these would be excellent

#### Atreus

#### Atreus

 Level 1

JoinedAug 24, 2020Messages15

Helped me a lot thanks

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Did the usual. Approved, long overdue.

 You must log in or register to reply here.


### Similar threads

 UI: GLUEBUTTON


Tasyen

Sep 9, 2019

Scripting Tutorials

Replies32Views15K

Nov 16, 2023
Tasyen

 UI: FrameEvents and FrameTypes


Tasyen

Aug 27, 2019

Scripting Tutorials

Replies16Views12K

Dec 7, 2024
Gaspar

 UI: Frames and Tooltips


Tasyen

Jun 27, 2019

Scripting Tutorials

Replies45Views21K

Oct 27, 2024
RPG_F

 UI - Simpleframes


Tasyen

Nov 25, 2019

Scripting Tutorials

Replies30Views16K

Sep 27, 2025
Tasyen

 UI: Positionate Frames


Tasyen

May 29, 2019

Scripting Tutorials

Replies25Views19K

Feb 11, 2026
Wareditor

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