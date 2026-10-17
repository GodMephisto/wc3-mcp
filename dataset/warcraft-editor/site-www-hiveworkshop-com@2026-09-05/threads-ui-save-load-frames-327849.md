---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-save-load-frames.327849/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Save Load Frames
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-save-load-frames.327849/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

###
 Introduction

Currently none of the versions of warcraft 3 that have access to the UI-Frame natives do probably Save&Load the usage of Frames/TOC (this was written when 1.32.8 /1.32.9 PTR were the current Versions).

This broken Save&Load stops Frames from being recreaterd/modified when the Game is Loaded (but they still hold their handleIds). It is even worser as it sounds, when one uses a variable reference of such a now broken Frame then the game can crash.

As mapper one could fix it without Blizzard fixing the Save&Load for Frames.​

###
 How to fix it

The idea is simple. We just let the old broken Frames leak (can't do much about it anyway) and repeat every frame creation/modification after a 0s timer run in a trigger with a Game was Loaded Event. With that all references should be updated and the custom UI does continue to work. Also be aware that you also have to restore any counters being used in the Frame creation and that you have to repeat the TOC-Loading. This works cleaner and better when your Frame creation/modification function doesn't do anything else then TOC and Frames.

One could write a ReLoader with that Game is Loaded Event that runs all the UI-Frame Init Functions.

In Lua one would have array of functions. But in vjass a trigger to which you add all the functions that have to be repeated after loading the game.​

###
 Code Example

The crash code Example, it just creates a button at the center of the screen at 0s and makes it visible once a second. After the game was Saved & Loaded. The game will crash as soon the game tries to make the frame visible.

 Lua:

do
 local frame
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 frame = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.4, 0.3)
 TimerStart(CreateTimer(), 1, true, function()
 BlzFrameSetVisible(frame, true)
 end)
 end
end

This are a Lua and vjass example of the above described "reloading".

### Lua

### vjass

First the reloader:

 Lua:

-- in 1.31 and upto 1.32.9 PTR (when I wrote this). Frames are not correctly saved and loaded, breaking the game.
-- This runs all functions added to it with a 0s delay after the game was loaded.
do
 local data = {}
 local real = MarkGameStarted
 local timer

 function FrameLoaderAdd(func)
 table.insert(data, func)
 end
 function MarkGameStarted()
 real()
 local trigger = CreateTrigger()
 timer = CreateTimer()
 TriggerRegisterGameEvent(trigger, EVENT_GAME_LOADED)
 TriggerAddAction(trigger, function()
 TimerStart(timer, 0, false, function()
 for _,v in ipairs(data) do v() end
 end)

 end)
 end
end

Second the example: This takes the first Hero Button and moves it to the center of the screen.

 Lua:

do
 local real = MarkGameStarted
 local function Init()
 BlzEnableUIAutoPosition(false)
 local frame = BlzGetOriginFrame(ORIGIN_FRAME_HERO_BUTTON, 0)
 BlzFrameClearAllPoints(frame)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.3)
 end
 function MarkGameStarted()
 real()
 Init()
 if FrameLoaderAdd then FrameLoaderAdd(Init) end
 end
end

 JASS:

library FrameLoader initializer init_function
// in 1.31 and upto 1.32.9 PTR (when I wrote this). Frames are not correctly saved and loaded, breaking the game.
// This library runs all functions added to it with a 0s delay after the game was loaded.
// function FrameLoaderAdd takes code func returns nothing
 // func runs when the game is loaded.
 globals
 private trigger eventTrigger = CreateTrigger()
 private trigger actionTrigger = CreateTrigger()
 private timer t = CreateTimer()
 endglobals
 function FrameLoaderAdd takes code func returns nothing
 call TriggerAddAction(actionTrigger, func)
 endfunction
 private function timerAction takes nothing returns nothing
 call TriggerExecute(actionTrigger)
 endfunction
 private function eventAction takes nothing returns nothing
 call TimerStart(t, 0, false, function timerAction)
 endfunction
 private function init_function takes nothing returns nothing
 call TriggerRegisterGameEvent(eventTrigger, EVENT_GAME_LOADED)
 call TriggerAddAction(eventTrigger, function eventAction)
 endfunction
endlibrary

Second the example: This takes the first Hero Button and moves it to the center of the screen.

 JASS:

library Example initializer init_function requires FrameLoader
 private function At0s takes nothing returns nothing
 local framehandle frame = BlzGetOriginFrame(ORIGIN_FRAME_HERO_BUTTON, 0)
 call BlzEnableUIAutoPosition(false)
 call BlzFrameClearAllPoints(frame)
 call BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.3)
 endfunction
 private function init_function takes nothing returns nothing
 call FrameLoaderAdd(function At0s)
 call TimerStart(CreateTimer(), 0, false, function At0s)
 endfunction
endlibrary

#### Attachments

Crash.w3x

 12 KB

 · Views: 214



Lua.w3x

 12.7 KB

 · Views: 210



vjass.w3x

 13.5 KB

 · Views: 181



#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Couldn't let me approve your tutorials before adding a new one could you?

No edits needed in terms of structure which is nice.

Will read through this later today.

edit: so I suspect this might be too simple.

 Wurst:

import ClosureTimers
import ClosureEvents

init
 EventListener.add(EVENT_GAME_LOADED) ->
 doAfter(0) ->
 myUiInitFunction()

This is basically it right? admittedly wurst's standard library makes this insanely simple but I consider the following:

A user will fall into one of two groups

The user has knowledge of some text based modding language and is using that to build their own UI from scratcch

The user has downloaded a pre-built UI
The first group does not need a tutorial to achieve this as they already know basic coding, they just need the information "create frames after a 0 second delay on load"

The second group should rely on the prebuild supporting this to begin with.


 Last edited: Nov 3, 2020

#### MyPad

#### MyPad

 Level 31

JoinedMay 9, 2014Messages1,878

Resources: 27

#### Resources
Models 1Icons 4Maps 6Spells and Systems 7JASS 7Lua 1Tutorials 1

Can existing custom frames (bugged through save and load) be safely destroyed, or would it still crash the game? This is in the context of recreating the frames afterwards.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

MyPad said:

 Can existing custom frames (bugged through save and load) be safely destroyed, or would it still crash the game? This is in the context of recreating the frames afterwards.


Click to expand...

Even BlzDestroyFrame crahses the game when used onto a variable that points to such a broken frame after loading (happens in 1.31 and 1.32.8).

Tring to reach that frame over BlzGetFrameByName won't give you the broken Frame, hence no crash but also no Destroyed Frame.

Chaosy said:

 This is basically it right? admittedly wurst's standard library makes this insanely simple but I consider the following:

A user will fall into one of two groups

The user has knowledge of some text based modding language and is using that to build their own UI from scratcch

The user has downloaded a pre-built UI
The first group does not need a tutorial to achieve this as they already know basic coding, they just need the information "create frames after a 0 second delay on load"

The second group should rely on the prebuild supporting this to begin with.


Click to expand...

You got this, Total true. The solution is quite simple.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Right.

So while I technically think this is too simple for a tutorial, given that the target audience probably already knows jass; (more detailed reasoning in my previous post)

I think this is mandatory information on the subject which is currently not common knowledge and needs to be spread somehow.

So for pure usefulness, I'll approve it, although I will try to keep this one at the back of my head in case blizzard fixes this in the future.


 Last edited: Oct 13, 2020

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Warcraft 3 crashes when an EditBox contained 5k bytes and the game is saved.

Edit: or maybe not and it is something else like line seperator \n.


 Last edited: Nov 9, 2023

#### Izzetin

#### Izzetin

 Level 9

JoinedMay 19, 2016Messages162

Is this still up to date (2.0.2) ? i need to load custom frames in Jass

thanks

#### Dark-Zalor

#### Dark-Zalor

 Level 19

JoinedOct 10, 2011Messages580

Resources: 28

#### Resources
Maps 25Spells and Systems 2Sounds 1

The TOC file seems to not need to be necessary to reload, I have that error:

 5/11 09:35:16.098 Error, frame "MoveToFirstBtn-2-1" already encountered!


Click to expand...

or it is because I try to create an item with the same name? And he thinks the older one still exists?

Does it causes crashes?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

As far I know the TOC & fdf are stored until the warcraft 3 game itself is closed. At least it is like that in Warcraft 3 V1.31.1. This reencounter is not a problem, as long it is from the same fdf.

#### Dark-Zalor

#### Dark-Zalor

 Level 19

JoinedOct 10, 2011Messages580

Resources: 28

#### Resources
Maps 25Spells and Systems 2Sounds 1

On loading I still have random crashes, I comment different part of the code, but some random bugs still happens. I test it by closing the game between save/reload.

Test is launched by world etitor Test option.

Here is the code of the loading game:

 vJASS:

scope LoadGame

private function launchLoader takes nothing returns nothing
 local timer T = GetExpiredTimer()
 call PauseTimer(T)
 call DestroyTimer(T)

 // TO DESTROY STRUCTS AND NULLIFY THE FRAMES
 call clearInterfaces()

 // TO RE-CREATE STRUCTS WITH FRAMES
 call reloadInterfaces()

 set LOADING = false

 set T = null
endfunction

private function loadActions takes nothing returns nothing
 local timer T

 set LOADING = true

 // TO DISABLE FRAMES USED BY PERIODIC EVENTS THAT COULD CAUSE INSTANT CRASHES
 // AND NULLIFY FRAMES
 call toDoFirst()

 call BlzLoadTOCFile(TOC_FILE_PATH)

 call TimerStart(T, 1, false, function launchLoader)

 set T = null
endfunction

//===========================================================================
function InitTrig_LoadGame takes nothing returns nothing
 local trigger T = CreateTrigger()
 call TriggerRegisterGameEvent(T, EVENT_GAME_LOADED)
 call TriggerAddAction(T, function loadActions)
 set T = null
endfunction

endscope

Is it well formatted and used the good way.

My Structs and Triggers are created with getOrCreate functions (it allow only 1 instance, never null, and all of theme can be created at any moment in the game)

Even with all of this I have random crashes on loading the map.

I saw the message before, it seems that it crashes more often when I've filled a TEXT frame (with only max 8 chars and no '\n') before the save. (I clear the TEXT frame value after submiting the input). The trigger that get the input values is a

VALIDATE_ENTER_TRIGGER on the input.

So if you have any ideas....


 Last edited: May 11, 2025

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The order looks good.

First clear all old frames from before the loading, no frame api onto them anymore.

Then remake it.

I dont see a timer that runs function launchLoader, T is empty when call TimerStart is reached.

Not sure if 1s delay is good some of your Timers might touch frames in that time. Otherwise you clear them, should be okay.

 You must log in or register to reply here.


### Similar threads

 UI: Positionate Frames


Tasyen

May 29, 2019

Scripting Tutorials

Replies25Views19K

Feb 11, 2026
Wareditor

 UI: Creating a Cam control


Tasyen

Jun 1, 2019

Scripting Tutorials

Replies24Views10K

Mar 4, 2021
ESTO

 UI: Frames and Tooltips


Tasyen

Jun 27, 2019

Scripting Tutorials

Replies45Views21K

Oct 27, 2024
RPG_F

 UI: Creating a Bar


Tasyen

Jun 12, 2019

Scripting Tutorials

Replies38Views18K

Oct 2, 2025
KenArok

 UI - The concept of Parent-Frames


Tasyen

Nov 19, 2019

Scripting Tutorials

Replies20Views11K

Oct 10, 2020
Chaosy

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