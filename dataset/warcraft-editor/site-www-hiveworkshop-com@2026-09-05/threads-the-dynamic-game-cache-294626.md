---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/the-dynamic-game-cache.294626/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# The Dynamic Game Cache
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/the-dynamic-game-cache.294626/>
#### Elf_Lord

#### Elf_Lord

 Level 13

JoinedFeb 5, 2008Messages228

Resources: 3

#### Resources
Maps 2Tutorials 1

Hey, everyone! I wasn't quite sure where to post this, but courtesy of some awesome mods, this thread is now where it belongs! Anyway, I just wanted to share some of what I've learned about game caches in my time with Chasing the Dawn and the ANA system, both of which are highly dependent on using game caches creatively. I will note that some of what I discuss here is different from what I use in my campaign, as I've tried to adapt my system to work with a traditional campaign setup where you can replay missions at any time (which you cannot do in mine). All of what I'm doing can be accomplished with GUI alone.

Please note that I recommend not changing your campaign's name once you've built a dynamic game cache. More likely than not it will make no difference, but I'm superstitious about it. I will test whether the game caches persist between campaigns of different names once I'm finished with this and update it accordingly.

1) Building a Basic Dynamic Game Cache

I'm assuming that, if you're here, you know what a game cache is and why it's useful to a campaign. If not, there is an actual tutorial on this by Bob27 (Game Caches) which can give you a rundown of the basics of building a traditional mission-by-mission game cache. My system is intended to complement the traditional game cache system. There is an alternate build for dynamic game caches that replaces traditional game caching entirely, but right now there is a potential exploit that, in my opinion, renders it less viable. I will explain complementary systems here, as I believe they will make more sense with a traditional campaign setup.

First, either way we're doing it, we need a main game cache file. I typically recommend building a trigger just for game caching and have your victory trigger call it rather than building it directly in. It makes it easier to update later, which trust me, if you are anything like me, you will be doing.

Events
(No need to put anything here - will be called by another trigger)

Conditions

Actions
Game Cache - Create a game cache from AdventuresOfBuddyThePaladin.w3v

Set MainGameCache = (Last created game cache)

In a complementary system, you will then create a second game cache for this mission in particular, while in a full dynamic system you will need only the main cache.

Actions
Game Cache - Create a game cache from AdventuresOfBuddyThePaladin.w3v

Set MainGameCache = (Last created game cache)

Game Cache - Create a game cache from AoBtPMission1.w3v

Set MissionGameCache = (Last created game cache)

All mission specific data will be saved to the Mission game cache. We just need a few special items for the main cache for now, namely a boolean (Mission1Complete) and an integer (LastCompletedMission). And yes, for now I recommend both, although it seems redundant.

Actions
Game Cache - Store BuddyThePaladin as BuddyThePaladin of Heroes in MissionGameCache

Game Cache - Store (Other Stuff) as (Other Stuff) of (Reasonably Named Category) in MissionGameCache

Set LastCompletedMission = 1

Set Mission1Complete = True

Game Cache - Store LastCompletedMission as LastCompletedMission of General in MainGameCache

Game Cache - Store Mission1Complete as Mission1Complete of General in MainGameCache

Game Cache - Save MissionGameCache

Game Cache - Save MainGameCache

So far, nothing weird, nothing special. At the beginning of the next mission, we'll just call up our game caches and restore everything*. Basic game cache stuff.

*There is no actual need to restore any of the data from the main game cache in any map but Mission 1 at the moment.

So far all we've done is do a basic campaign game cache, but we've begun setting up for the dynamic cache. Now we're going to get into some functions of the dynamic cache.

2) Game Restoration

The first use of the dynamic game cache is restoring games. If you're like me, it has always bothered you that, if you download an updated version of the campaign you're playing, you have to start all over (that, or cheat). However, the dynamic game cache presents a simple, elegant solution to this problem. All we have to do is go into the first map that is available to the player and set up a quick trigger or two.

First, although the mission has nothing cached of its own yet, we need to restore the main game cache and all of the values related to mission completion. So if we have a four-mission campaign...

Events
Map Initialization

Conditions

Actions
Game Cache - Create a game cache from AdventuresOfBuddyThePaladin.w3v

Set MainGameCache = (Last created game cache)

Set Mission1Complete = (Load Mission1Complete of General from MainGameCache)

Set Mission2Complete = (Load Mission2Complete of General from MainGameCache)

Set Mission3Complete = (Load Mission3Complete of General from MainGameCache)

Set Mission4Complete = (Load Mission4Complete of General from MainGameCache)

Set LastCompletedMission = (Load LastCompletedMission of General from MainGameCache)

This reload needs to be done before the map proper begins, so set this to run in Map Initialization or somewhere similar and put the actual functions that start the mission/cinematic/whatever in a separate trigger with no event trigger (for now). Now we need the map to check if we have previous progress in this game. For this, a simple If/Then/Else function will do (I'm using a multi-function here because it looks more legible).

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
LastCompletedMission Greater than 0

Then - Actions
Trigger - Run RestoreSetup <gen> (ignoring conditions)

Else - Actions
Trigger - Run (whatever trigger you put main map functions in) (ignoring conditions)

If there is no LastCompletedMission data, then this trigger will just start the map as normal, no harm, no foul. If there is data, it'll call a trigger we will now write to interpret this data. So let's make our new trigger, RestoreSetup. It needs no events or conditions, as it is being called by another trigger.

Now, we have a choice here. We can a) load the last played map in the campaign, b) load the farthest the player has gotten in the campaign, or c) let the player choose. I'm going to walk through the setup for the third option, as I like giving the player more control if possible, but it should be fairly simple to extrapolate from option (c) how to do the other two. Let's start by building a dialog. I will have it built in one of two ways, depending on whether the player's last played mission matches their farthest point in the campaign. You will need an "and" condition check for each mission in the game but one, as so.

Events

Conditions

Actions
Dialog - Change the title of RestoreDialog to Do you wish to restore your previous progress?

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Mission2Complete Equal to True) and (LastCompletedMission Equal to 1)

(Mission3Complete Equal to True) and (LastCompletedMission Equal to 2)

(Mission4Complete Equal to True) and (LastCompletedMission Equal to 3)

Then - Actions
Dialog - Create a dialog button for RestoreDialog labelled (Restore Last Played (Mission + (String(LastCompletedMission)) + )))

Set RestoreDialogButton[1] = (Last created dialog Button)

If (Mission1Complete Equal to True) then do (Set ButtonBuilder = 1) else do (Do nothing)

If (Mission2Complete Equal to True) then do (Set ButtonBuilder = 2) else do (Do nothing)

If (Mission3Complete Equal to True) then do (Set ButtonBuilder = 3) else do (Do nothing)

If (Mission4Complete Equal to True) then do (Set ButtonBuilder = 4) else do (Do nothing)

Dialog - Create a dialog button for RestoreDialog labelled (Restore Farthest Point (Mission + (ButtonBuilder + )))

Set RestoreDialogButton[2] = (Last created dialog Button)

Dialog - Create a dialog button for RestoreDialog labelled Start New Campaign

Set RestoreDialogButton[3] = (Last created dialog Button)

Else - Actions
Dialog - Create a dialog button for RestoreDialog labelled (Restore Last Played (Mission + (String(LastCompletedMission)) + )))

Set RestoreDialogButton[1] = (Last created dialog Button)

Dialog - Create a dialog button for RestoreDialog labelled Start New Campaign

Set RestoreDialogButton[3] = (Last created dialog Button)

Dialog - Show RestoreDialog for Player 1 (Red)

What this does is check if the player has recently gone back and replayed missions and, if so, prompt the player to choose between his most recently played game and the farthest he has gotten in the campaign.

The first button is built using Concatenate Strings, converting the variable LastCompletedMission into a mission number, while the second button is built using Concatenated Strings and a string variable called ButtonBuilder. You can of course create whatever buttons you want. Now we build a response trigger to a button press, again using If/Then/Else functions to dictate responses. Clicking buttons 1 and 2 will restore those respective points in the campaign, while clicking button 3 will effectively wipe the main game cache and force a full restart.

Events
Dialog - A dialog button is clicked for RestoreDialog

Conditions

Actions
Dialog - Hide RestoreDialog for Player 1 (Red)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to RestoreDialogButton[1]

Then - Actions
If (LastCompletedMission Greater than or Equal to 1) then do (Show custom campaign button (Mission 2)) else do (Do nothing)

If (LastCompletedMission Greater than or Equal to 2) then do (Show custom campaign button (Mission 3)) else do (Do nothing)

If (LastCompletedMission Greater than or Equal to 3) then do (Show custom campaign button (Mission 4)) else do (Do nothing)

Game - Victory Player 1 (Red) (Skip dialogs, Skip scores)

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to RestoreDialogButton[2]

Then - Actions
If (Mission1Complete Equal to True) then do (Show custom campaign button (Mission 2)) else do (Do nothing)

If (Mission2Complete Equal to True) then do (Show custom campaign button (Mission 3)) else do (Do nothing)

If (Mission3Complete Equal to True) then do (Show custom campaign button (Mission 4)) else do (Do nothing)

Game - Victory Player 1 (Red) (Skip dialogs, Skip scores)

Else - Actions
Do nothing

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Clicked dialog button) Equal to RestoreDialogButton[3]

Then - Actions
Set Mission1Complete = False

Set Mission2Complete = False

Set Mission3Complete = False

Set Mission4Complete = False

Set LastCompletedMission = 0

Game Cache - Save Mission1Complete as Mission1Complete of General in MainGameCache

Game Cache - Save Mission2Complete as Mission2Complete of General in MainGameCache

Game Cache - Save Mission3Complete as Mission3Complete of General in MainGameCache

Game Cache - Save Mission4Complete as Mission4Complete of General in MainGameCache

Game Cache - Save LastCompletedMission as LastCompletedMission of General in MainGameCache

Game Cache - Save MainGameCache

Trigger - Run (whatever trigger you put main map functions in) (ignoring conditions)

Else - Actions
Do nothing

And there you have it. Your campaign will now restore previous progress with each update. You could also add in more if/then/else functions that would load the latest unfinished map using a similar system. I have opted against doing so right now out of laziness.

3) Choices, Choices

One of my favorite features in my campaign is the ability to change your army composition. This is a feature that allows the player to make decisions that carry over from mission to mission but can be changed at any time. While I will not be giving a full tutorial in how to recreate the ANA system here, I will, however, do a brief overview of how to create a system that allows you to catalog decisions made by the player and carry them over in a way that is easily changeable from map to map.

For our example, I have decided that, in the Adventures of Buddy the Paladin, you can pick whether you can build Footmen or High Elven Swordsmen from the Barracks, which is decided in some manner in some place that is not a dialog that pops up at the beginning of each mission (because then you would have no need of this system!) So first, in whatever map this decision is made, we need to find a way catalog our decision, so we're going to create an integer called FootmanChoice. Then, in our choosing trigger, we add...

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
You picked Footman

Then - Actions
Blah blah blah in-game actions

Set FootmanChoice = 1

Else- Actions
Blah blah blah other in-game actions

Set FootmanChoice = 2

And we note this decision in our main game cache.

Actions
Game Cache - Store FootmanChoice as FootmanChoice of Units in MainGameCache

Then, in all our maps, we restore this choice and set unit availability based on it. So...

Events
Map Initialization

Conditions

Actions
Game Cache - Create a game cache from AdventuresOfBuddyThePaladin.w3v

Set MainGameCache = (Last created game cache)

If (FootmanChoice Equal to 1) then do (Make Footman Available for training/construction by Player 1 (Red)) else do (Make Swordsman Available for training/construction by Player 1 (Red))

If you're doing unit selection like this, I highly recommend making all units unavailable at startup then allowing the game cache to activate the chosen ones; making things unavailable by trigger also works but will take more scripting if you have more than 2 options.

Obviously this part of the system can be used for far more than just unit selection. It can be used to effectively catalog any recurring choice and replicate it across multiple maps. I use it for unit selection, talent selection, and game difficulty in Part I of my campaign.

Of course, you could also use this to catalog mission-specific stuff in the Mission Game Cache, like if you wanted to make a sort of Brood War-style choice where you could destroy one type of tech for the enemy or another and it would restrict their tech in the specified way in the next mission.

4) Conclusion

Well, that's it. That's the basics of complementary dynamic game caching. If you wanted to fully dispose of Mission Game Caches you could use just a Main Game Cache (as I do in my campaign), but it would effectively prevent missions from being replayed unless you build a system of detecting whether the mission is being replayed rather than being played for the first time, which could be done using the MissionXComplete boolean sets we generated for Section 2. However, I will leave that development to you.

I will likely be updating this thread as I think of other uses for dynamic game caching. I'm sure I've already used it in more ways than I have listed and I have simply forgotten because I have a pounding headache right now and can barely focus. This is pretty simple stuff, so I'm honestly not sure if this will help anyone or not, but it felt useful to me to put my knowledge somewhere where people could find it. If nothing else, I would love to see more campaigns use this to make campaigns that can restore previous progress.

Please feel free to comment, ask questions, point out issues, et cetera, et cetera. I will do my best to watch this thread and respond to comments.

Also, make sure you actually fire the game cache trigger in the victory trigger (or alternatively directly integrate it, which I don't recommend).


 Last edited: Jun 18, 2017

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

Question! Shouldn't this be in tutorial section instead?

I just read at a glance but I don't find any issue so far. Looking nice, sir.

#### Elf_Lord

#### Elf_Lord

 Level 13

JoinedFeb 5, 2008Messages228

Resources: 3

#### Resources
Maps 2Tutorials 1

Rheiko said:

 Question! Shouldn't this be in tutorial section instead?

I just read at a glance but I don't find any issue so far. Looking nice, sir.


Click to expand...

Thank you! And yes, it probably should, but I've never posted a tutorial before and the place I thought it was supposed to go was locked. I've probably missed something obvious and a moderator will come point it out to me at some point.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

Well, you should post it as a submission first before it gets approved and finally moved to the tutorial section.

Tutorial Submission

#### Elf_Lord

#### Elf_Lord

 Level 13

JoinedFeb 5, 2008Messages228

Resources: 3

#### Resources
Maps 2Tutorials 1

Ah, there we are. The something obvious.  Thank you. Should I just repost this there, then, and delete this thread?

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

I think a mod can simply move this.

#### Elf_Lord

#### Elf_Lord

 Level 13

JoinedFeb 5, 2008Messages228

Resources: 3

#### Resources
Maps 2Tutorials 1

Weeeellllllll, after uncovering a rather major potential game cache exploit in my campaign (thankfully found by accident and not by someone actually exploiting it), I now no longer support SOLELY using a dynamic/campaign-wide game cache. ALWAYS use it in conjunction with regular mission-to-mission game caches if possible. Thankfully that is what this tutorial explores, so no need to change anything. Just wanted to point this out for anyone else who might follow in my footsteps and make my same mistake (which I'm going to explain below for those who are interested). This by no means that dynamic game caches are in themselves bad.

My campaign, as those who have played it know, does not allow maps to be replayed until you have finished that section of the campaign, at which point a boolean is thrown noting that Replay mode is now on for that section which won't allow progress from replayed maps to carry over to the next mission. This was intended to prevent players from repeatedly playing an easier map to farm levels and/or items. Unfortunately, there is a way around this: if a player saves during a mission, completes the mission successfully, reloads the mission, then restarts from the reload, this safeguard can be bypassed and the player can effectively level/item farm. As of yet, I have not found a way using a dynamic game cache alone to prevent this exploit, so I am now retroactively building a complementary system into Chasing the Dawn. In short, complementary systems are the way to go.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Sorry for reviewing this so late. This is a really awesome topic! Definitely great for campaigns where things are more open-world (e.g. Rexxar's campaign) rather than linear: Mission 1 -> Mission 2 -> Mission 3, etc. (and also for situations where players want to replay a map, like you mentioned). I like that you also included that restore option--it is really nice to have, especially if your game ends up in a bad state some how where their data can't be recovered from the gamecache.

Approved!

 You must log in or register to reply here.


### Similar threads

 Game Caches


Bob27

Mar 18, 2006

Trigger (GUI) Editor Tutorials

Replies34Views52K

Jul 21, 2018
PurgeandFire

Locked

 Game cache-restoring custom hero


Veliotv

Nov 13, 2012

Triggers & Scripts

Replies3Views2K

Nov 15, 2012
Veliotv

Locked

[Trigger]
 Game Caches:


robot-dude

Feb 8, 2008

Triggers & Scripts

Replies3Views2K

Jul 17, 2008
redmarine

Locked

[Solved]
 Game Cache Issue


Morseldancer

Jan 4, 2020

Triggers & Scripts

Replies5Views1K

Jan 4, 2020
Morseldancer

Locked

[Trigger]
 Game Cache


shinji

Feb 3, 2012

Triggers & Scripts

Replies6Views2K

Feb 3, 2012
shinji

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Trigger (GUI) Editor Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top