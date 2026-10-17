---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/save-and-load-in-gui.258693/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Save and Load in GUI
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/save-and-load-in-gui.258693/>
#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

#
 Easy Save and Load in GUI

-Only works in Single Player until updated!-

##
 Introduction

Greetings. I decided to create a tutorial for saving/loading in GUI. I know that there are existing GUI systems out there. Code Gen and Ace Harts are the most famous ones. However this tutorial is directed for those that want to create their own, or the existing systems lack something that they need in terms of saving, whatever that might be. I also quickly want to mention that I wont explain any GUI actions in this tutorial since it's not for starters, unless you don't understand simple GUI actions yet, you should go and learn some GUI before heading back to this tutorial.

##
 Difficulty

5/10

##
 What is needed?

*above basic knowledge of GUI in general

*New Gen Editor (JNGP) - Download

*FILE I/O - Download

*Local Files enabled (Extensions > Enable Local Files)

##
 What is file I/O and why do we use it?

File I/O allows the user to create and read files which contain information of our choice.

Pros:

*Encrypts/Decrypts the information we decide to store into our files

*Saves us a good chunck of work and effort

*We don't need to care about a save code, which means using this method the user can type "-load" instead of "-load xxx xxxx xxx xx xxxxx"

Cons:

*file I/O is made in vJASS, which forces us to use custom scripts more than we should.

##
 Preperations

In order to understand this tutorial fully you will need to know about GetLocalPlayer().

Once you've done that. We will create our variable for the GetLocalPlayer().

Init
Events
Map initialization

Conditions

Actions
Custom script: set udg_local_player = GetLocalPlayer()

Before we start, you also need to decide on what you want to store. In my example I decided to save the following values.

*Player Name

*Main Hero

*Hero Level

*Player Gold

*Player Lumber

For learning purposes I strongly suggest that you don't start with loading a unit, that's the most annoying part. Try doing it with gold or something like that at first.

Note: You can only save string values, and not other variable types.

This is my setup so I got some values I can save in my map.

Just for testing
Events
Map initialization

Conditions

Actions
Player - Add 666 to Player 1 (Red) Current lumber

Player - Add 1337 to Player 1 (Red) Current gold

Set hero_types[1] = Paladin

Set hero_types[2] = Archmage

Set hero_types[3] = Mountain King

Set hero_types[4] = Blood Mage

Set hero_names[1] = Paladin

Set hero_names[2] = Archmage

Set hero_names[3] = Mountain King

Set hero_names[4] = Blood Mage

Set hero_count = 4

Set spawn_loc = (Center of Spawn Region <gen>)

Unit - Create 1 hero_types[(Random integer number between 1 and hero_count)] for Player 1 (Red) at spawn_loc facing Default building facing degrees

Set player_hero = (Last created unit)

Hero - Set player_hero Hero-level to (Random integer number between 2 and 10), Hide level-up graphics

##
 Saving

Now to the real triggering part of the tutorial.

We'll use the following script to declare (create) a file variable. Note that the variable is of the type "File" and is named "file".

Custom script: local File file
That line need to be on the top of the trigger else JNGP will throw errors in your face.

Now that we got our file variable, we need to make some settings before we can actually start to do stuff.

Custom script: local File file

Set mapName = testMap

Set file = exp
*mapName = the folder that the file will be created in.

*file = file name

The result will be: Warcraft III/GameData/mapName/file

Now we're ready to finally start saving values. We need to set our file variable before we can start with the actual saving though.

Custom script: set file = File.open(udg_mapName, udg_file, File.Flag.WRITE)
Once that's placed out, we can start to use the file.write() basically it writes something on the first line of a empty text document.

Set what_to_save = (Name of (Triggering player))

Custom script: call file.write(udg_what_to_save)
In my case the player name will be saved on the first row of the file.

After that you simply repeat the process over and over until everything is saved.

GUI Save
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Custom script: local File file

Set mapName = testMap

Set file = exp

Set player = (Triggering player)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
local_player Equal to Player 1 (Red)

Then - Actions
Custom script: set file = File.open(udg_mapName, udg_file, File.Flag.WRITE)

Set what_to_save = (Name of (Triggering player))

Custom script: call file.write(udg_what_to_save)

Set what_to_save = (Name of player_hero)

Custom script: call file.write(udg_what_to_save)

Set what_to_save = (String((Level of player_hero)))

Custom script: call file.write(udg_what_to_save)

Set what_to_save = (String((player Current gold)))

Custom script: call file.write(udg_what_to_save)

Set what_to_save = (String((player Current lumber)))

Custom script: call file.write(udg_what_to_save)

Game - Display to (All players) the text: saved!

Custom script: call file.close()

Else - Actions

##
 Load

Loading the information we've stored use the same method as before with a few changes. We start off just like before.

Custom script: local File file

Set mapName = testMap

Set file = exp

Set player = (Triggering player)
However when setting our file variable to a value there is a slight difference. File.Flag.WRITE > File.Flag.READ

Custom script: set file = File.open(udg_mapName, udg_file, File.Flag.READ)
To start the actual loading, I use a string variable named "what_to_load" and makes it read the upper row inside our file.

Custom script: set udg_what_to_load = file.read()
After that we just need to use that variable for our purposes in. In my case it's the player name.

Custom script: set udg_what_to_load = file.read()

Player - Set name of player to what_to_load
After that, it's just copy > paste pretty much. re-store the variable, and reuse it.

GUI Load
Events
Player - Player 1 (Red) types a chat message containing load as An exact match

Conditions

Actions
Custom script: local File file

Set mapName = testMap

Set file = exp

Set player = (Triggering player)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
local_player Equal to Player 1 (Red)

Then - Actions
Custom script: set file = File.open(udg_mapName, udg_file, File.Flag.READ)

Custom script: set udg_what_to_load = file.read()

Player - Set name of player to what_to_load

Custom script: set udg_what_to_load = file.read()

For each (Integer A) from 1 to hero_count, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
what_to_load Equal to hero_names[(Integer A)]

Then - Actions
Unit - Create 1 hero_types[(Integer A)] for Player 1 (Red) at spawn_loc facing Default building facing degrees

Else - Actions

Custom script: set udg_what_to_load = file.read()

Hero - Set (Last created unit) Hero-level to (Integer(what_to_load)), Hide level-up graphics

Custom script: set udg_what_to_load = file.read()

Player - Add (Integer(what_to_load)) to Player 1 (Red) Current gold

Custom script: set udg_what_to_load = file.read()

Player - Add (Integer(what_to_load)) to Player 1 (Red) Current lumber

Custom script: call file.close()

Else - Actions

##
 Credits

Nestharus - File I/O


 Last edited: Jun 22, 2015

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Why not just use gamecache? : )

Without synchronization, File I/O is a glorified gamecache = P

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I'll make sure to include it later on. I am just a little tired atm. Thanks!

#### Deleted member 219079

D

#### Deleted member 219079

 Why not just use gamecache? : )

Without synchronization, File I/O is a glorified gamecache = P


Click to expand...

What do you mean by without synchronization?

Also good tutorial, colors look pleasant.

 -Only works in Single Player until updated! (I am need to sleep first!)-


Click to expand...

Well at the moment this is just as good as game cache.

Also this:

Custom script: local File file

Set mapName = testMap

Set file = exp
You have udg_file and file here, it might be misleading for some GUI users.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

jondrean said:

 What do you mean by without synchronization?


Click to expand...


File only on one machine. What happens when only 1 player makes a unit or something?

#### Deleted member 219079

D

#### Deleted member 219079

I see, well I'm intrigued to see how OP will turn this to mp

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

They have to usr my Network lib, which is too slow and needs an update. If they use it, this tut can't be used unless u wanna freeze the game for 5 mminutes while sync takes place.

We already know how to fix Network, all of us that code core resources. The prob is that nobody has time to update it.

If it's updated, you can just use my pure GUI codeless save/load stuff. Will be easier to use and faster.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Nestharus said:

 File only on one machine. What happens when only 1 player makes a unit or something?

Click to expand...


Ouch, I didn't take that into consideration. I will test if it works without a huge load time with network. If it's too slow I will think of something else.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Chaosy said:

 Ouch, I didn't take that into consideration. I will test if it works without a huge load time with network. If it's too slow I will think of something else.


Click to expand...


I already told you what happens... lol

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

 -Only works in Single Player until updated!-


Click to expand...

Ever got around to testing it in multiplayer? As for Nes' statement, I assume that the lag only happens when the file is large? Or does the freeze happen in any other situation?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

No I did not, I kinda trust Nes to know about Save/Load.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

This tutorial completely fails to mention that this only works with "enable local files" enabled in the Windows registry.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Zwiebelchen said:

 This tutorial completely fails to mention that this only works with "enable local files" enabled in the Windows registry.


Click to expand...


Thanks for pointing that out. I honestly never thought of that, I always have it enabled myself since I don't see a reason not too. I will mention it somewhere.


rep+

#### sentrywiz

S

#### sentrywiz

I really liked this. Until I read it was singleplayer only. In that case, what is the point of this? Save game works much easier and much better and saves the hassle of looking for files, copying a large amount of characters etc... Also like Zwie said.

Its great as a tutorial on how to make your own.

But it sucks if its only for singleplayer.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

From what I understand it work in multiplayer but it's very slow apparently. Though I have never tested to check exactly how slow.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Well, the reason it's slow is because the thread locking that Network uses needs to be optimized a bit and needs to use asynchronous things to achieve its synchronous behavior. Right now locking floods the network.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

sentrywiz said:

 I really liked this. Until I read it was singleplayer only. In that case, what is the point of this?


Click to expand...

It has a purpose in singleplayer, as the normal method of saving/loading via the game menu will break periodic timers and waits.


Of course, the mapper *can* technically account for these things with workarounds, but more than often it is easier (and also desirable) to implement a manual save/load into a singleplayer map over the menu based save/load.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Zwiebelchen, can just use gamecache directly in single player.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

It's basicily a tutorial for GUI users how to use File IO with a few of custom scripts.

Save/Load tequniques usually use some sort of encryption, and also some synchronisation library.

I suggest to screw the idea of making full Save/Load tutorial. They should go with existing systems and learn them to use.

But a simple tutorial for GUI users how to properly use File IO as easy as possible. Why not. ¯\_(ツ)_/¯

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Submission:Save Load in GUI

Date:28 November 2016

Status:Graveyard

Note:

Some things needs to be changed and clarified. Related to my post above.

In case you will be working on it just ping here with your thoughts. For now, Graveyard.

 You must log in or register to reply here.


### Similar threads

M

[Misc]
 Save/Load by AceHart


meXy92

Oct 20, 2010

Tutorial Archive

Replies7Views3K

Oct 26, 2010
PurgeandFire

 How to remove Memory Leaks IN GUI


KingB00ker

Jul 6, 2009

Tutorial Archive

Replies18Views6K

Jul 12, 2009
Pyritie

 Trying to Configure Games Played Counter (Save/Load System)


Gnuoy

Mar 14, 2025

World Editor Help Zone

Replies0Views324

Mar 14, 2025
Gnuoy

Locked

[Trigger]
 Save/Load Problem


RiotZ

Aug 27, 2011

Triggers & Scripts

Replies13Views2K

Aug 30, 2011
RiotZ

Locked

 Save/Load Code


somedood

Aug 19, 2014

World Editor Help Zone

Replies3Views2K

Aug 21, 2014
Gismo359

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