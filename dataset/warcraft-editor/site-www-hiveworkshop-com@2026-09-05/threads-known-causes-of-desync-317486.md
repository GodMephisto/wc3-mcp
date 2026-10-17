---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/known-causes-of-desync.317486/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Known Causes of Desync
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/known-causes-of-desync.317486/>
#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

Introduction

Players should be able to differentiate a crash, a player leaving and a player being desynchronized.

Desync is the fact that one or more player gets disconnected from a online multiplayer game. It's caused by differences in the game state between the game clients.

Below I tried gathering the all the desync causes identifed in previous post, plus my own testing results.

1. Causes that will desync at 100%

a. GetLocalPlayer() used for anything else than UI-display or sound-play, or used by Blizzard internal functions (search GetLocalPlayer here to get them). Here is a FAQ on how to use it properly.​

b. GUI Function "Select unit group <group> for player <player>", alias JASS function SelectGroupForPlayerBJ. You should replace it by a for loop that adds unit 1 by 1 to selection (source: here).​

c. [Fixed in 1.31] GUI Function "Pan Camera As Necessary", alias JASS function SmartCameraPanBJ(fix here).​

d. [Since 1.32] Old widgetizer tools to protect the map (if players don't clean their cache by restarting Warcraft III between 2 different games)​

e. GetCameraTargetPosition natives (X, Y, Z) returns an asynchronous value (each player has its own camera), so like for GetLocalPlayer() it must only be used for UI-display or sound-play.​

f. Calling subfunctions in the arguments of BlzFrameSetText() in a "GetLocalPlayer() ==" condition. String concatenation is fine (source here).​

g. JASS function GameCacheSync if called more than 344 times in a single frame (source here).​

h. JASS functionSetSkyModel called with an invalid model path as parameter (source here).​

i. Iterating over Lua tables with pairs (not ipairs), because they are not sorted so there is a risk of iterating out of order (source and fix here).​

j. Editing some Game Constants related to creeps with high values (sources: here and here) will desync on game start:​

--- "Creed Guard Distance"​

--- "Creeps - Guard Return Distance"​

--- "Creeps - Maximum Creep Camp Radius"​

2. Causes of desync with low propability (so desync may happen at random times)

a. Numerous "Player slot comparison"s, for example to check if a player is connected (source: my map. I recommand storing a player group for connected players updated with a trigger on event "player leaves the game").​

b. Numerous "Player controller comparison"s (source: my map).​

c. Weapon upgrades with effect "Attack Dice Bonus" desync when "Base + (Upgrade level -1) * Increment" is superior to 14 (source here, here, and my own tests). I've no idea why 14 is the limit, I tested many values below and above.​

d. GetLocationZ calls if​

--- A terrain deformations is ongoing below the unit (because deformations are not synchronized between the clients, and not displayed for players with low quality graphics settings)​

--- Unit is on an animated walkable destructable.​

e. Models and and textures when badly made (source here).​

f. A very very huge amount of memory leaks.​

g. Creating a periodic trigger with a null period (even with Blizzard has set a safety min of 0.0001). It will likely hit the op limit at different numbers of executions/different times for each computer the game was being run on.​

h. Creating Timers with a very short delay (<= 0.01s) on map initialization has approx 25% chances of desync (source here).​

i. Destroying user-created frames (source here).​

3. Uncertain functions

LeP made a research in common.j for all annotations with "async" and found the functions below. So if you suspect one of your triggers of causing desync, you can check for the presence of any of those primitives.​

GetDestructableName​

GetSoundDuration​

GetSoundFileDuration​

GetCameraBoundMinX​

GetCameraBoundMinY​

GetCameraBoundMaxX​

GetCameraBoundMaxY​

GetCameraField​

GetCameraTargetPositionX​

GetCameraTargetPositionY​

GetCameraTargetPositionZ​

GetCameraTargetPositionLoc​

GetCameraEyePositionX​

GetCameraEyePositionY​

GetCameraEyePositionZ​

GetCameraEyePositionLoc​

GetObjectName​

GetLocalizedString​

GetLocalizedHotkey​

GetLocalPlayer​

GetLocationZ​

GetItemName​

GetUnitName​

GetPlayerSlotState​

IsMultiboardMinimized​

BlzGetLocalUnitZ​

BlzGetUnitZ​

BlzGetItemDescription​

BlzGetItemTooltip​

BlzGetItemExtendedTooltip​

BlzGetLocalSpecialEffectX​

BlzGetLocalSpecialEffectY​

BlzGetLocalSpecialEffectZ​

4. Unverified rumors, unlikely

a. Checking player slot in a trigger on Map Initialization Event. Fix would be to call it after 0.00s elasped.​

b. GetEffectZ: I don't know this primitive, but read about it somewhere, and it would be based on host's value and synced.​

c. SetSoundVolumeBJ & playing the same map many times in a row (source here).​

d. GetLocationZ: randomly causes desync for player using a different setup (linux, mac, reforged vs classic). Also walkable destructables and stiff terrains will cause desync if some players are Classic and others are Reforged (source here).​

e. Selection primitives (source here), but not sure. Indeed internally some selections use GetLocalPlayer, but normally it is synchronized by packets and primitives return the selection result properly for all clients only then.​

f. Low "Backgrounf FPS Cap" in options + players tabbing out of the game (source here).​

g. Creating frames while the game is tabbed-out by the player (because screen size getters may return 0).​

5. Fake rumors, does not cause desync

a. Wait and PolledWait do not cause desync, they just may cause more network traffic to keep all clients synchronized, and may cause lags due to lack of time accuracy.​


 Last edited: Sep 12, 2023

#### LazZ

#### LazZ

 Level 20

JoinedNov 4, 2019Messages407

Resources: 206

#### Resources
Packs 1Tools 2Maps 2Ported Icons 199User Interface 1Sounds 1

Very informative. Thx for posting

#### Nudl9

#### Nudl9

 Level 25

JoinedNov 9, 2006Messages2,561

Resources: 39

#### Resources
Icons 13Skins 2Maps 23Tutorials 1

Lots of great research has been put down here, very nice.

#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

Nudl9 said:

 Lots of great research has been put down here, very nice.


Click to expand...

Thanks.

Unfortunately I am still stuck with random desync issues in my map.

I managed to create a file logger. However I cannot log too often, because "write in file" operation is time consuming, particularly with big text files. This is way I store a log array, and only write them in file on some event.

I managed to list all the JASS trigger in my map (by executing a little script). I am only missing the custom JASS triggers whose name does not start by "gg_trg_". But if there are not too many, I can add them manually.

I made a macro able to register a callback on trigger execution start (for ex a log)

However I still miss a way to only the piece of code to write in file just after desync happens...

#### Nudl9

#### Nudl9

 Level 25

JoinedNov 9, 2006Messages2,561

Resources: 39

#### Resources
Icons 13Skins 2Maps 23Tutorials 1

Well personally i've gone through every section of my map and eliminated triggers upon triggers.

So it's not triggers.

I've probably checked everything there is and i think it has to be something basic.

Like the bigger a map is the chances increase for sure.

#### coolyoshi1

#### coolyoshi1

 Level 8

JoinedJul 25, 2009Messages194

Resources: 7

#### Resources
Maps 3Spells and Systems 4

Nudl9 said:

 Well personally i've gone through every section of my map and eliminated triggers upon triggers.

So it's not triggers.

I've probably checked everything there is and i think it has to be something basic.

Like the bigger a map is the chances increase for sure.


Click to expand...

Can totally agree with this. Also have gone through every trigger, put debugging statements in all my triggers and all custom spells and confirmed it's not just triggers.

Whatever the cause of the desync is, map size/complexity and number of players (9+ players) definitely seems to increase the chances of desync.

#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

coolyoshi1 said:

 Can totally agree with this. Also have gone through every trigger, put debugging statements in all my triggers and all custom spells and confirmed it's not just triggers.

Whatever the cause of the desync is, map size/complexity and number of players (9+ players) definitely seems to increase the chances of desync.


Click to expand...

After I asked some questions, bigbull gave me a promising possibility:

Before I was able to

- I made a logger with rolling logs & the possibility to write them in file

- With a script executed on war3map.j, I manage to list all GUI triggers (I still need something to log JASS global & local triggers)

- So I was able to write in file a log at each GUI trigger execution. I wrote only on "Player leaves" event to avoid lags. It didn't work if you were the player impacted with desync so I was stuck...

bigbull gave me the idea to not log anything on battle.net games. But to log everything in Replay (thanks to a snippet that can differentiate live game & replay). I hope that if I get the replays of many players for the same game, I can find what piece of trigger bugs.

If it is not triggers but graphics, I'm doomed^^

#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

I have discovered a new source of desync (only on patch 1.32 and newer):

- SelectGroupForPlayerBJ : systematic desync for example when I use Item Lists v1.01 system..

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Ricola3D said:

e. Editing some Game Constants related to creeps:

> "Creed Guard Distance"

> "Creeps - Guard Return Disce"

> "Creeps - Maximum Creep Camp Radius"​

Click to expand...

​

Can you provide me with a Source on this one?

#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

Zwiebelchen said:

​

Can you provide me with a Source on this one?


Click to expand...

Mhhh I think it was from Nudl9 & TriggerHappy who talked about guard & aggro ranges modification causing desync.

Both have an account on hiveworkshop

#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

Zwiebelchen said:

​

Can you provide me with a Source on this one?


Click to expand...

Here is a link : [Trigger] - Desync on start

Also Drake53 seems to have experienced the issue : Desyncs on map load

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

GetLocalPlayer() faq has a nice desync list

Regardless, currently this is not a tutorial given the introduction and whatnot. But it's just a simple edit requried.

edit: I missed the link. I removed the intro part and changed the thread title. Approved.


 Last edited: Oct 18, 2020

#### tlstkwjr

#### tlstkwjr

 Level 7

JoinedMay 29, 2013Messages152

I've been suffering from desync since patch 1.33. My map didn't change anything before and after 1.33. After the patch, there was a desync. This is an individual desync, where one specific user just exits the game. Just leaving the game seems to be the type of post-reforged desync. If anyone knows of a new desync cause since 1.33, please let me know.

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

@tlstkwjr Does your map modify any game constants? If they are buggy in the editor and due to how game files are cached, they are probably broken in more places. Try asking players if they restarted the game before joining.

#### Gnuoy

#### Gnuoy

 Level 23

JoinedMar 16, 2008Messages1,043

Resources: 3

#### Resources
Maps 3

 c. Too many weapon upgrades on a unit > each attack has a chance to cause desync


Click to expand...

how many is too many? 20? 100? anything over default (3)?

6. Random known causes

a. Player slot comparison


Click to expand...

So we shouldn't use this condition AT ALL? or just not during map init?

What conditions could we use as an alternative?


 Last edited: Aug 15, 2023

#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

Gnuoy said:

 how many is too many? 20? 100? anything over default (3)?

So we shouldn't use this condition AT ALL? or just not during map init?

What conditions could we use as an alternative?


Click to expand...

About weapon upgrade, I don't really know. Ask ThaïCat maybe, he posted he met the issue here.

When I personally used player slot comparison in frequent triggers (periodic, on player selection) I had tons of desync. It was before Reforged. So as a solution, I called it only sometimes, and saved the result as a "player group" for frequent comparisons. I wonder if it is because when the game is tabbed out of by a player, it's like during loading and some synchronizations are not made properly..

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

Condenscending as it may sound, these posts are nearly useless without test maps for replication. I understand it will take lots of time to create one (and impossible if the map maker does not have the hardware to test in LAN/on same PC).

EDIT: I see Ricola provided sourced almost everywhere, that's good. I didn't look but I hope at least some of them have ready to test maps uploaded.

I understand the likes of "custom game constants desyncing" - something very clear in the game we haven't found yet because you'd need to ask each player what their previous map was.

I don't understand the playerslot comparison example. Because desyncs happen usually due the other data you compared it with.

My life-saving protip so far is to track each change of your map using git. Reforged's map folder option is great for this, but not required (save the entire map if thats the only thing you can do).

Changed a unit's stats? Save in git. Added code - commit in git. If a desync happens you will be able to find the bad change that causes it in less than 5-8 tests by using "git bisect"

#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

Luashine said:

 Condenscending as it may sound, these posts are nearly useless without test maps for replication. I understand it will take lots of time to create one (and impossible if the map maker does not have the hardware to test in LAN/on same PC).

EDIT: I see Ricola provided sourced almost everywhere, that's good. I didn't look but I hope at least some of them have ready to test maps uploaded.

I understand the likes of "custom game constants desyncing" - something very clear in the game we haven't found yet because you'd need to ask each player what their previous map was.

I don't understand the playerslot comparison example. Because desyncs happen usually due the other data you compared it with.

My life-saving protip so far is to track each change of your map using git. Reforged's map folder option is great for this, but not required (save the entire map if thats the only thing you can do).

Changed a unit's stats? Save in git. Added code - commit in git. If a desync happens you will be able to find the bad change that causes it in less than 5-8 tests by using "git bisect"


Click to expand...

Excellent suggestion, I'll git my map from now on.

Unfortunately the current desync bugging me came with the release of Reforged, not a change in code. So even with that, it would be hard to figure out what parts of the code are guilty.

#### Gnuoy

#### Gnuoy

 Level 23

JoinedMar 16, 2008Messages1,043

Resources: 3

#### Resources
Maps 3

I'm not convinced player slot does desync, but just incase I removed over 100 of these conditions. If the player is absent then I destroy their main building then just boolean check if that building is alive as a substitute of slot comparison.

#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

Gnuoy said:

 I'm not convinced player slot does desync, but just incase I removed over 100 of these conditions. If the player is absent then I destroy their main building then just boolean check if that building is alive as a substitute of slot comparison.


Click to expand...

"GetPlayerSlotState" is among the functions whose code use "GetLocalPlayer()" if I'm not wrong, so it is suspicious.

Also I'll have to document it when it's ready, but I think I figured a way to help debugging desyncs with many things pieced together:

A file logger using Preload primitives but with good performances (thanks to rotating log files + writing logs by packet instead of 1 by 1).

Exporting the map script with the menus (Terrain Editor > File > Export script..."). Then getting a list of all triggers (for ex with Notepad++ and searching all CreateTrigger() calls.

Macros to declare the necessary functions & hooks to log when all triggers see their conditions checked & their action executed. That's the tricky part but I figured how add a log function as first condition, last condition, first action and/or last action of each trigger.

Also, in addition hooks on the natives known for using GetLocalPlayer() to log when they are directly called.

#### KitsuneTailsPrower

#### KitsuneTailsPrower

 Level 23

JoinedOct 9, 2019Messages375

Resources: 74

#### Resources
Skins 1Spells and Systems 26Ported Models 44Ported Skins 2Tutorials 1

Does anyone know if 6e applies to any edit of those gameplay constants or just setting them too high?

edit: I checked some open source maps and found that some edits do seem to work. For example this: Legion TD Mega 3.5 (B4) + 3.41 unprotect The unprotected old version has some guard distance edits


 Last edited: Aug 28, 2023

#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

KitsuneTailsPrower said:

 Does anyone know if 6e applies to any edit of those gameplay constants or just setting them too high?

edit: I checked some open source maps and found that some edits do seem to work. For example this: Legion TD Mega 3.5 (B4) + 3.41 unprotect The unprotected old version has some guard distance edits


Click to expand...

Nice to know. Have you pushed your tests with some values ?

Also for those who may be interested, I'm currently investigasting a new cause of random desyncs since Reforged: ugrades. Most upgrades don't seem to cause desync, but I'm trying to identify which ones do. For ex "Rhme (Iron Forged Swords)" level 3 (with custom dice values) causes random desyncs for me in a map with simply footmen attacking towers. I'll update you when I'm done with my investigation.

#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

I updated the list of desync causes. "2.c. Too many weapon upgrades" becomes:

Weapon upgrades with effect "Attack Dice Bonus" desync when "Base + (Upgrade level -1) * Increment" is superior to 14 (source here, here, and my own tests). I've no idea why 14 is the limit, I tested many values below and above.

#### tlstkwjr

#### tlstkwjr

 Level 7

JoinedMay 29, 2013Messages152

what is Old widgetizer tools? w3 map optimizer 5.0 is this?

#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

tlstkwjr said:

 what is Old widgetizer tools? w3 map optimizer 5.0 is this?


Click to expand...

Well I don't know the current state-of-the-art of widgetizers. But Reforged introduced a regression creating desyncs if you play a 2nd version of the map without restarting the game. So I guess if the widgetizer doesn't have a version more recent than Reforged it has this desync issue.

Since it's from 2012, does it still work ? Or maybe are you editing for older versions of w3 ?

I think https://w3protect.eu/ works on the latest patches, but haven't tested it rigorously yet

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

Ricola3D said:

 I updated the list of desync causes. "2.c. Too many weapon upgrades" becomes:


Click to expand...

We've had another thread about it recently. The person said both attack and armor upgrades. I participated in it. Ping me if I forget to link it later.

#### GrapesOfWath

#### GrapesOfWath

Sound Reviewer

 Level 20

JoinedMar 31, 2012Messages309

Resources: 29

#### Resources
Models 5Skins 1Tools 2Maps 4Sounds 17

Some other desync causes: basing any gameplay logic off of handle IDs, and StartTimerBJ, but both of these are only problems for lua maps, as handle ids aren't synced between clients in LUA, and variables get garbage collected in lua so bj_laststartedtimer gets garbage collected every time StartTimerBJ is called

#### Ricola3D

#### Ricola3D

 Level 13

JoinedFeb 27, 2019Messages404

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

Luashine said:

 We've had another thread about it recently. The person said both attack and armor upgrades. I participated in it. Ping me if I forget to link it later.


Click to expand...

You forgot to link it ^^

Is it link ?

#### tlstkwjr

#### tlstkwjr

 Level 7

JoinedMay 29, 2013Messages152

Riot's Vanguard program is causing the desync.

#### Ytrec

#### Ytrec

 Level 4

JoinedJan 1, 2021Messages7

New desync cause

###
 Desync issue: Evevators + GetlocationZ


I found a desynchronization problem in reforged, which is reproduced in 100% of cases. It can be reproduced on one computer in a local network, completely in SD graphics. The problem with the GetLocationZ() has been known for a long time, but it was previously believed that desync was caused by...

 www.hiveworkshop.com


#### SNIper of DARKness

#### SNIper of DARKness

 Level 13

JoinedApr 9, 2013Messages764

Resources: 1

#### Resources
Tutorials 1

What does this " * Increment" is superior to 14 "mean?

#### Insanity_AI

#### Insanity_AI

 Level 11

JoinedJun 30, 2017Messages99

Resources: 2

#### Resources
Spells and Systems 2

Ricola3D said:

 I have discovered a new source of desync (only on patch 1.32 and newer):

- SelectGroupForPlayerBJ


Click to expand...

To add to this:

The actual reason for this is calling ForGroup (also works with ForForce) inside a Local Player code.

It will desync regardless what values you send to those natives.

Fortunately, only that GUI function has that problem.


 Last edited: Oct 14, 2024

#### Tomotz

#### Tomotz

 Level 10

JoinedOct 29, 2024Messages72

Resources: 9

#### Resources
Maps 1Spells and Systems 8

In my Lua map, I had many desyncs.

I found that when I remove a loop of TriggerSleepAction(0.25) that was triggered inside a function called with ExecuteFunc, one of the desyncs (that was always happening at the very start of the map) was gone.

I tried removing all code from this loop except the TSA function, and the desync was still there.

Switching the TSA to a timer also resolved this issue.

I'm not sure the TSA was the direct cause for the desync, but for sure it was leading to it somehow (maybe something in another context desynced due to being called at a slightly different time on different clients).

I moved to using [Lua] - Precise Wait (GUI-friendly) (and hooking ExecuteFunc to change it to a coroutine) and this solved all my desync problems

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

Tomotz said:

 ne of the desyncs (that was always happening at the very start of the map) was gone.


Click to expand...

How did you start that other trigger? Did you use a very low "Time elapsed" in Lua?

#### Tomotz

#### Tomotz

 Level 10

JoinedOct 29, 2024Messages72

Resources: 9

#### Resources
Maps 1Spells and Systems 8

Luashine said:

 How did you start that other trigger? Did you use a very low "Time elapsed" in Lua?


Click to expand...

What other trigger?

I had a function that I stripped to something like:

 Lua:

function aaa()
 while true do
 TriggerSleepAction(0.25)
 end
end

...
 ExecuteFunc("aaa")
...

I had a lot of other code running in the map, but with this executeFunc I had desync, without it, or with switching the TSA with a timer I did not.

Timer looked something like this:

 Lua:

function aaa()
 t = CreateTimer()
 TimerStart(t, 0.25, false, function()
 DestroyTimer(t)
 aaa()
 end)
end

I tried reproducing it in an almost empty map and failed, so for sure that wasn't the sole cause for the desync.

Code was written from memory now, so not sure it's exact. I've already deleted all these changes from my map

I think I used TimerQueue lib so the timer function actually looked much simpler without the create and destroy


 Last edited: Jan 8, 2025

#### Gnuoy

#### Gnuoy

 Level 23

JoinedMar 16, 2008Messages1,043

Resources: 3

#### Resources
Maps 3

I'm wondering if we can now determine the exact causes of desyncs and crashes given this article: Blizzard Accidentally Releases Debug Symbols, Maps, and Exclusive Content in Warcraft 3 Update Version 2.0.1

#### GrapesOfWath

#### GrapesOfWath

Sound Reviewer

 Level 20

JoinedMar 31, 2012Messages309

Resources: 29

#### Resources
Models 5Skins 1Tools 2Maps 4Sounds 17

Gnuoy said:

 I'm wondering if we can now determine the exact causes of desyncs and crashes given this article: Blizzard Accidentally Releases Debug Symbols, Maps, and Exclusive Content in Warcraft 3 Update Version 2.0.1

Click to expand...

there is some debugging that can be done using their Replay Analyzer tool (diff the logs output from replays from someone who dropped from desync and someone who didn't)

outside of that, not entirely sure

#### KitsuneTailsPrower

#### KitsuneTailsPrower

 Level 23

JoinedOct 9, 2019Messages375

Resources: 74

#### Resources
Skins 1Spells and Systems 26Ported Models 44Ported Skins 2Tutorials 1

Multiple sources state that in a lua map creating Warcraft 3 Objects such as locations and hashtables outside of any scope e.g. In the declaration of a global variable will cause a desync. The solution is to initialize those variables in an init function. Can we add this finding to the above list?

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

@KitsuneTailsPrower How convenient, that I've recently posted the exact Blizzard.j code in Lua mode that the game runs before you enter the lobby:

bj_volumeGroupsTimer = CreateTimer()

bj_queuedExecTimeoutTimer = CreateTimer()

bj_suspendDecayFleshGroup = CreateGroup()

bj_suspendDecayBoneGroup = CreateGroup()

bj_delayedSuspendDecayTimer = CreateTimer()

bj_lastCreatedGroup = CreateGroup()

bj_lastStartedTimer = CreateTimer()
Citation and a test map needed.

PS: On the other hand we have this, that went unsolved for years now. Lua GUI timers always desync (minimal repro)

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

@KitsuneTailsPrower thanks for the test map and additional information in your other thread. [Lua] - [Solved] desync with transpiled lua code

I'm on it. Your workaround is indeed valid for your map. Where you re-define the DDLib__TempLocLocation created at map root (globals block) from within the DDLib__onInit function (aka map's main()). It seems to work without the redefinition. The AI must be human (or orc? but not undead and not nightelf) for the desync to be possible.

I kinda overreacted, sorry for that. Thankfully, your test map is simple enough to dissect it.

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

## UPD: Only desyncs in LAN, but not BNET?​
Now this is weird. But we should wait whether the current maintenance team will come up with something

github repo (map version as of posting attached here too)

The culprit:

 Lua:

DDLib__TempLoc = Location(0., 0.) -- this is done in INIT -- ROOT LOCATION()

-- Create garbage
-- This is equivalent to putting globals in root, something the game does
for i = 1, 100 do -- garbage directly affects WHEN the desync happens
 local key
 if i % 2 == 0 then
 key = "garbage_" .. i .. "_luatable"
 _G[key] = {}
 else
 key = "garbage_" .. i .. "_jarray"
 _G[key] = __jarray(0)
 end
end

function DDLib__onInit()
 DDLib__TempLoc = Location(0., 0.) -- THIS LINE ALONE TOGETHER WITH SAME DEFINITION IN GLOBALS AT THE START WILL DESYNC
 -- REMOVE ONE OR THE OTHER AND THE DESYNC SEEMS TO STOP.
 -- GO FIGURE.
 -- YES, PLEASE DO FIGURE IT OUT
end

 Lua:

function main()
 SetRandomSeed(0xf0f0) -- Set seed that's kinda reliable in conjunction with human AI

 -- CAMERA AND MAP SETUP OMITTED FOR HIVE POST --

 InitBlizzard()


 DDLib__onInit() -- called from here


 InitGlobals()
 InitCustomTriggers()
 RunInitializationTriggers()
end

So here it is. It desynced in my testing only IF the following conditions were met:

Location is created in map root, where globals block would have been

That Location is overwritten in init phase, part of the main() call
it's alright if created only in globals or only during init

There exists enough garbage to throw off the garbage collector and affect its iteration order (assumption). at the very least the created garbage affects the time when it finally desyncs

AI player must be playing Human (orc?)

for increased reliability: SetRandomSeed(0xf0f0) at the top of main()
The desync always seemed to coincide with AI's order to start a new building or when the worker reached the construction site and laid the foundation. +-1s

Insights:

GetHandleId numeric IDs do get out of sync. Especially if it was going to desync. However, they could go out of sync and you can keep playing without desyncing. This is no bueno.

GetHandleId eventually resyncs

If you don't overwrite location in Step 2 aka DDLib__onInit, but create another one -- no desync

Creating 10000 trash object slowed down when the desync happened by about 15-30 seconds. 100 objects seem to be the right amount (remember, Lua GC is slow?)

GetLocalPlayer, if recycled by GC, will create a new lua userdata next time. The handle ID (handle to internal object) stays the same.

I assume the only damn way for the GC to desync the game is ifit is run in incremental mode, so for player A it collects like the first 50% of the garbage and for player B it collects other 50% of the garbage, assuming that it can do the rest of the object during the next cycle.
of course, this assumes that the iteration order is not deterministic

In other words: wtf. That is, I still assume the GC works in a way that is generally not safe.

#### Attachments

desync-global-var.w3m

 12.8 KB

 · Views: 76




 Last edited: Aug 28, 2025

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Luashine said:

 So here it is. It crashed in my testing only IF the following conditions were met:


Click to expand...

Thanks for the repro and analysis.  Just to be clear, by "crashed" did you mean that it forced a disconnect? (i.e. forced a player to get booted from the game) Or do you mean it literally crashed the game and the application closed?

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

@PurgeandFire oh, always "desynced". The word "crash" was unintentional.

#### riky

#### riky

 Level 3

JoinedNov 29, 2023Messages14

Resources: 1

#### Resources
Maps 1

i dont the see it listed here, but i want add the textbook case

 JASS:

 if GetLocalPlayer() == x then
 ...
 call SetTextTagVelocityBJ(GetLastCreatedTextTag(), 75.00, GetRandomReal(10.00, 45.00))
 ...
 endif

getRandomX inside a getLocalPlayer block

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

###
 Doc - GetRandomReal


 lep.nrw


 note

 Desyncs! The random number generator is a global, shared resource. Do not change its state in local blocks asynchronously.


Click to expand...

#### Tomotz

#### Tomotz

 Level 10

JoinedOct 29, 2024Messages72

Resources: 9

#### Resources
Maps 1Spells and Systems 8

###
 Random desyncs from custom UI?


I'm just testing my new map out, and I'm getting a LOT of issues with random desync. Like 50% of the players get the boot in the first few minutes of the game. Now my map is Lua with custom UI. The desyncs all happen in the first few minutes of the game, during the creation of custom UI...

 www.hiveworkshop.com


BlzCreateFrame seems to have a chance to desync when ran in the wrong context.

I had a sporadic desync that happened right on the game start (about 1 out 8 players desynced).

I was doing

 Code:

BlzCreateFrame("EscMenuEditBoxTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0),0,0)

from

OnInit.global.

Moving the BlzCreateFrame to run from some trigger later in the game, solved the issue.

OnInit.global is supposed to run after the main already finished, so I'm not sure if that's the same issue as in the original post I shared above

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

###
 [Lua] - Total Initialization


Total Initialization Allows you to initialize your script in a safe, easy-to-debug environment. Allows your map's triggers to be arranged in any order* (similar to JassHelper). Enables functionality similar to Lua modules and vJass libraries. Provides helpful tools to hook into certain key...

 www.hiveworkshop.com


Tomotz said:

 OnInit.global is supposed to run after the main already finished, so I'm not sure if that's the same issue as in the original post I shared above


Click to expand...

That's where you are wrong.

 OnInit(function()

 print "All udg_ variables have been initialized"

 end)


Click to expand...

 -- - OnInit.global or just OnInit is called after InitGlobals and is the standard point to initialize.


Click to expand...

I really should attempt to map out the default WE code structure for jassdoc, but I think this happens while inside main(). Even if most of the data stuff is supposed to be synced at this point. I wouldn't touch UI any earlier than game start. Ideally after the player is presented with the complete world and UI view.

Thanks anyway, I'm going to include it in some form

#### Tomotz

#### Tomotz

 Level 10

JoinedOct 29, 2024Messages72

Resources: 9

#### Resources
Maps 1Spells and Systems 8

Luashine said:

 That's where you are wrong.


Click to expand...

In Total initialization, it says:

 OnInit.root(function()

 print "This is called immediately"

 end)

 OnInit.config(function()

 print "This is called during the map config process (in game lobby)"

 end)

 OnInit.main(function()

 print "This is called during the loading screen"

 end)

 OnInit(function()

 print "All udg_ variables have been initialized"

 end)

 OnInit.trig(function()

 print "All InitTrig_ functions have been called"

 end)

 OnInit.map(function()

 print "All Map Initialization events have run"

 end)

 OnInit.final(function()

 print "The game has now started"

 end)


Click to expand...

Is this not the order of the initializers?

seems like OnInit (or OnInit.global) should come after main finished

#### Insanity_AI

#### Insanity_AI

 Level 11

JoinedJun 30, 2017Messages99

Resources: 2

#### Resources
Spells and Systems 2

You're both correct, there's just miscommunication happening:

OnInit.main is executed when main gets called BUT before anything in main executes, otherwise main would execute after global/trig/map phases.

OnInit.global or OnInit is executed after InitGlobals is done (which technically runs inside map's function main's thread)

tldr: OnInit.main,global,trig,map are all executed from function main's thread

#### tlstkwjr

#### tlstkwjr

 Level 7

JoinedMay 29, 2013Messages152

today one player becomes desync, accompanied by the host's chat message stating like ‘warning: game desync detected by player [xxx]’ When this chat message appears, does it narrow down the cause of the desync? What could it be?

 You must log in or register to reply here.


### Similar threads

Locked

 Does GetLocationZ() Cause Desyncs?


Banefull

Jul 15, 2012

Triggers & Scripts

Replies13Views4K

Jul 15, 2012
Dr Super Good

Locked

 random desync problem


coolyoshi1

Aug 8, 2012

World Editor Help Zone

Replies16Views5K

Aug 16, 2012
coolyoshi1

Locked

[Crash]
 Camera Trigger causes 100% desync


Bloodheaven_

Dec 19, 2019

World Editor Help Zone

Replies2Views1K

Dec 19, 2019
Bloodheaven_

Locked

[JASS]
 Does this cause a Desync?


Sephalo

Aug 12, 2012

Triggers & Scripts

Replies7Views2K

Aug 13, 2012
Sephalo

I

Locked

 Can OwnerOfUnit cause desyncs?


Ineedajobtoo

Jul 28, 2010

World Editor Help Zone

Replies2Views1K

Jul 28, 2010
Ineedajobtoo

I

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