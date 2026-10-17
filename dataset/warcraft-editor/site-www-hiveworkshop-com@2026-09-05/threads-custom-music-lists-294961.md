---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/custom-music-lists.294961/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Custom Music Lists
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/custom-music-lists.294961/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Using Custom Assets is a great thing since the filesize limit increase, and music is no exception, but the question is: how do you play that music?

#
 1. A Way To Replace Default Music

The simplest way if you don't need the default one is by replacing the default music files with custom ones.

How to do it?

Open Sound Editor:

Let's close the "Sounds", if they are open. We only want "Music".

A short explanation: the music played by default is Race1.mp3,Race2.mp3,Race3.mp3,RaceX1.mp3.

A player playing Humans will hear rotational Human1, Human2, Human3 and HumanX1.

A player playing Orcs will listen to Orc1, Orc2, Orc3 and OrcX1.

And so on.

Now let's replace Human1.mp3. Right-click it with your mouse and choose "Replace Internal Sound" then a file dialog will open and you can can choose your music. Best is to pick an mp3 file. I do not know if other formats work, but mp3 works (after-all the default music is mp3).

After you have replaced Human1.mp3, Human2.mp3, Human3.mp3 and HumanX1.mp3; players playing as Human will have a full custom music list.

Replacing is easy but has its limits.

 You can only make 4 playlists (Human,Orc,Undead,NightElf) and each is linked to one race.

 Each play list must have 4 Songs.

 The original music can't be played in the map.

 Replaced sounds keep until warcraft 3 is closed, (Last checked before 1.28, but wasn't mentioned in patch notes so it's probably still the same.)
To have less limits we need to stick to triggers.

#
 Triggers

Most skilled coders will try to mimic playlists with waits or timers (both have some problems). I have tried this too, until I recently stepped across an old post from Vexorian on wc3c.net (Own music list - Wc3C.net).

Mimicking a playlist is actually unneeded; Warcraft 3 already has the ability to set music playlists or loop multiple music files.

Before I continue, the Warcraft 3 playlist has timeouts of ca. 20 to 40 seconds between songs.

And there are some problems with some specific MP3 files. I did not discover the specifics of this yet.

The important information from the post from Vexorian is:


 Vexorian said:



 Well the thing is that string musicName contains the path of the music file or "music" for default or plenty of paths separated by ; ("path1;path2;path3")


Click to expand...

This -> ("path1;path2;path3")

Conclusion: playing a music list is as simple as using a string which separates songs with ";".

Example:

The following playlist would contain these 4 Songs: War2IntroMusic.mp3 , Doom.mp3, TragicConfrontation.mp3, LichKingTheme.mp3

Set PlayList = Sound\Music\mp3Music\War2IntroMusic.mp3;Sound\Music\mp3Music\Doom.mp3;Sound\Music\mp3Music\TragicConfrontation.mp3;Sound\Music\mp3Music\LichKingTheme.mp3
There are now two trigger-based ways to use the playlists.

#
 2. Way: Native SetMapMusic

Let's take a closer look at SetMapMusic:

 JASS:

native SetMapMusic string musicName, boolean random, integer index

its a native having 3 arguments: String musicName, boolean random, and integer index.

musicName is: The file(s) which you want to play.

random: is order randomly changed?

index: not so sure what it does, but from the BJ-Action I would assume its the starting Index-song.

The BJ-Version can not be used - you can try but the two only things to choose is last played music and default music. Not want we want now.

The Action in Trigger Editor has the description: "Plays the music after the current one ends".

When should we use SetMapMusic?

"Map Init" is a good place as it will instantly set the playlist to the wanted one and play it.

In other situations, the playlist will always be delayed after the current song.

Stoping Music will stop this playList anyway and playing a music before results in some weird problems.

For instant music change there is an easier option, but we'll go into that later.
How it looks as a trigger:

This Trigger sets the Playlist to War2IntroMusic.mp3, Doom.mp3, TragicConfrontation.mp3, and LichKingTheme.mp3 for all players.

Again between 2 song files is a ";".

SetPlayList On Map Init
Events
Map initialization

Conditions

Actions
Set PlayList = Sound\Music\mp3Music\War2IntroMusic.mp3;Sound\Music\mp3Music\Doom.mp3;Sound\Music\mp3Music\TragicConfrontation.mp3;Sound\Music\mp3Music\LichKingTheme.mp3

Custom script: call SetMapMusic(udg_PlayList, true, 0)

To make playlists for specific players you'll have to use GetLocalPlayer().

This time we'll use the 4 Race Songs added with Frozen Throne: HumanX1.mp3, OrcX1.mp3, UndeadX1.mp3, NightElfX1.mp3.

We replace the music of players who chose "Random" as their race.

SetPlayList On Map Init
Events
Map initialization

Conditions

Actions
Set PlayLists = Sound\Music\mp3Music\HumanX1.mp3;Sound\Music\mp3Music\OrcX1.mp3;Sound\Music\mp3Music\UndeadX1.mp3;Sound\Music\mp3Music\NightElfX1.mp3

Custom script: set udg_Boolean = IsPlayerRacePrefSet(GetLocalPlayer(), RACE_PREF_RANDOM)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Boolean Equal True

Then - Actions
Custom script: call SetMapMusic(udg_PlayList, true, 0)

Else - Actions

#
 3. Way: PlayMusic

PlayMusic is an easy, powerful native which can, similar to SetMapMusic, take a string with all the music as an argument and can play it .

 JASS:

 native PlayMusic string musicName

If you clear the current Music List and use PlayMusic with multiple music files it will play all of them and loop them.

Sound - Clear the music list

Sound - Stop music Instantly

Custom script: call PlayMusic( udg_PlayList)
You can combine that with local player to let only specific players have a custom playlist.

The following lines will start the playlist PlayListBloodMage for any player who's hero is a Bloodmage

Blutmage Pick
Events
Unit - A unit is Trained

Conditions
(Unit-type of (Trained unit)) Equal Bloodmage

Actions
Set PlayList = Sound\Music\mp3Music\BloodElfTheme.mp3;Sound\Music\mp3Music\NagaTheme.mp3;Sound\Music\mp3Music\IllidansTheme.mp3

Custom script: set udg_LocalPlayer = GetLocalPlayer()

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
'IF'-Bedingungen
(Owner of (Triggering unit)) Equal LocalPlayer

Then - Actins
Sound - Clear the music list

Sound - Stop music Sofort

Custom script: call PlayMusic( udg_PlayList )

Else - Actins

Some might want to return to the default playlist during the game.

To reset to the default playlist:

-------- Music is a preset --------

Sound - Stop music Sofort

Sound - Play Music

Sound - Set the music list to Music, starting with a random song
Change playlist by using chat 0 to 5.

start
Events
Map initialization

Conditions

Actions
Set PlayLists[0] = Sound\Music\mp3Music\Human1.mp3;Sound\Music\mp3Music\Human2.mp3;Sound\Music\mp3Music\Human3.mp3;Sound\Music\mp3Music\HumanX1.mp3

Set PlayLists[1] = Sound\Music\mp3Music\Orc1.mp3;Sound\Music\mp3Music\Orc2.mp3;Sound\Music\mp3Music\Orc3.mp3;Sound\Music\mp3Music\OrcX1.mp3

Set PlayLists[2] = Sound\Music\mp3Music\NightElf1.mp3;Sound\Music\mp3Music\NightElf2.mp3;Sound\Music\mp3Music\NightElf3.mp3;Sound\Music\mp3Music\NightElfX1.mp3

Set PlayLists[3] = Sound\Music\mp3Music\Undead1.mp3;Sound\Music\mp3Music\Undead2.mp3;Sound\Music\mp3Music\Undead3.mp3;Sound\Music\mp3Music\UndeadX1.mp3

Set PlayLists[4] = Sound\Music\mp3Music\HumanX1.mp3;Sound\Music\mp3Music\OrcX1.mp3;Sound\Music\mp3Music\UndeadX1.mp3;Sound\Music\mp3Music\NightElfX1.mp3

Set PlayLists[5] = Sound\Music\mp3Music\War2IntroMusic.mp3;Sound\Music\mp3Music\Doom.mp3;Sound\Music\mp3Music\TragicConfrontation.mp3;Sound\Music\mp3Music\LichKingTheme.mp3

Set PlayListsSize = 5

For each (Integer A) from 0 to PlayListsSize, do (Actions)
Loop - Actions
Trigger - Add to Change Music <gen> the event (Player - Player 1 (Rot) types a chat message containing (String((Integer A))) as Exact Match)

Select - Select Paladin 0000 <gen>

Trigger - Run Random PlayList <gen> (ignoring conditions)

Change Music
Events

Conditions

Actions
Set PlayListsSelected = (Integer((Entered chat string)))

Trigger - Run SetPlayList <gen> (checking conditions)

SetPlayList
Events

Conditions
PlayListsSelected greater than or equal to 0

PlayListsSelected less than or equal to PlayListsSize

Actions
-------- Find the Songs --------

Set Counter = 0

Set Songs[0] = <Leerer String>

-------- Check Each Single Letter, if it is a ";" start new Song, else add it to the current Song --------

For each (Integer A) from 1 to (Length of PlayLists[PlayListsSelected]), do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Substring(PlayLists[PlayListsSelected], (Integer A), (Integer A))) Unequal ;

Then - Actions
Set Songs[Counter] = (Songs[Counter] + (Substring(PlayLists[PlayListsSelected], (Integer A), (Integer A))))

Else - Actions
Set Counter = (Counter + 1)

Set Songs[Counter] = <Leerer String>

-------- Print them --------

For each (Integer A) from 0 to Counter, do (Actions)
Loop - Actions
Game - Display to (All players) for 30.00 seconds the text: Songs[(Integer A)]

Custom script: set udg_LocalPlayer = GetLocalPlayer()

-------- Play them for Local Player --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering player) Equal LocalPlayer

Then - Actions
Sound - Clear the music list

Sound - Stop music Sofort

Custom script: call PlayMusic( udg_PlayLists[udg_PlayListsSelected] )

Else - Actions

#
 How to Use Imported Music

Well Warcraft 3 does not accept any MP3 Files for being part of the playlist or as multiple Music Argument.

But it can play any Mp3 once, if used as single argument.

If you put together playlist of songs that can be put in a playlist and ones that cannot be put in a playlist, only one song is played.

Looks to me like these MP3's contain something that warcraft 3 can't work with and it skips setting the playlist, because of invalid argument.

We use the Win7 default MP3 Musics.

Sorry, but this does not work with the default win7 Music-Mp3s, but the concept is right.

It's not so much different from the stuff before. But you can now define the path of the music file which results to easier strings in general.

First let's import 3 music files with Import Manager.

Switch into Import Manager:

In the now opened window, start importing.

I import the Win7 basic included music.

Currently the paths in my opinion are bad. Everything is war3mapImported, and "Maid with the Flaxen Hair" to long.

Lets change it to Music\

We do that with all of them.

Now it looks like that:

As you can see at the bottom: music is quite heavy. These 3 Songs take ~17 MB.

Now we switch to Trigger Editor and create an String Variable name PlayList.

And create this trigger:

PlayMusic Starting at 5 seconds
Events
Time - Elapsed game time is 5.00 seconds

Conditions

Actions
Sound - Clear the music list

Sound - Stop music Instantly

Set PlayList = Music\Kalimba.mp3;Music\Maid Flaxen Hair.mp3;Music\Sleep Away.mp3

Custom script: call PlayMusic(udg_PlayList)

Setting the PlayList.

Make sure that every song is separated by ";".

The default GUI shown Action - "PlayMusic" can not take strings. That's why we have to use custom script.

After some testing with current WEX: I still do not know why Warcraft 3 does not like these 3 Songs.

3 Songs from another game, the ProtectorsMod, are working. All of those 6 Files are Mp3.

Warcraft 3 can play each of the win7 music files as single argument, but not as pack. .

I even reduced Win7 Music Names to One Word-Files without Spaces and not beeing in SubFolders.

The Music from Protectors Mod can be play as pack, loops and can be set to be the playlist. Each is in an subfolder. :O

I realized now that warcraft 3 does not even loop this Win7-music files even if set as MapMusic being an Single Argument.

Current conclusion it has to do something with the files itself, but the problem is not the filesize.

Credits:

Vexorian - multiple Musicfiles as 1 String

RG00 - The Member why I discovered this stuff

Chaosy - GetLocalPlayer

The uploaded map contains the examples in Way 2 and Way 3.

#### Attachments

MusicList2.w3x

 20.3 KB

 · Views: 517




 Last edited by a moderator: Jun 17, 2018

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Awesome trick! And great examples. I didn't know that you could input more than one sound file into PlayMusic and SetMapMusic. Thanks a ton for making this!

I've fixed up some of the grammar and spelling in the tutorial (hope you don't mind), and converted some of the German in the triggers to english. Approved!

P.S. Regarding the invalid mp3, I know that wc3 has very specific settings in its mp3 files, such as bit-rate, channel, etc. You might want to look at the sound info in the sound editor and try to mirror that in your custom mp3s.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Did check the infos in war 3 Sound Editor also compared them with VLC's MediaInfo. For some reason Warcraft 3 does dump down the imported Music to 22.5 khz and 56 kbps. While in VLC they have 44.1 khz and 192 kbps. the Protector music also has 44.1 khz and 192 kbps (VLC) but is a valid music but also is dumped down, wtf.

Whatever I converted the invalid (for warcraft 3) mp3 - Win 7 Example-Music - using VLC Player to match Orc1.mp3 data beeing 44.1 khz and 112 kbps MPEG1, Layer 3.

Also tested 128 kbps which is used by other Warcraft 3 Musics like Arthas Theme, Bloodelf Theme or HumanX1.

Both ways it works.

Thanks for that hint.

#### Marcraft22

#### Marcraft22

 Level 4

JoinedAug 9, 2012Messages27

@Tasyen I just want to point out that this tutorial no longer works as of version 1.32.3

Hope it can be fixed since it was really useful


 Last edited: Apr 4, 2020

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

I too would like to play a music list, and unfortunately since reforged, this doesn't work anymore.

Is there any known way of starting a playlist?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Multiple songs in Playmusic & SetMapMusic should be be fixed with the next patch. It is mentioned in the current patchnotes V1.32.9 PTR and I tested playing multiple musics in one PlayMusic on that PTR version which worked. But PlayMusic didn't loop after it played all songs. it returned to default music. I remember that the music was looped. Well fine for me, one play isn't bad.

SetMapMusicRandomBJ still loops the given songs.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

There is another option to change the playList of a map, By using the game interface file war3mapSkin.txt. In that File one sets the keys Music_V0, Music_V1 & Or Music_V1Beta.

I would assume V1 is for tft maps and V0 for roc maps.

 Code:

[CustomSkin]
Music_V0=Sound\Music\mp3Music\TragicConfrontation.mp3;Sound\Music\mp3Music\Comradeship.mp3;Sound\Music\mp3Music\War2IntroMusic.mp3
Music_V1=Sound\Music\mp3Music\TragicConfrontation.mp3;Sound\Music\mp3Music\Comradeship.mp3;Sound\Music\mp3Music\War2IntroMusic.mp3
Music_V1Beta=Sound\Music\mp3Music\TragicConfrontation.mp3;Sound\Music\mp3Music\Comradeship.mp3;Sound\Music\mp3Music\War2IntroMusic.mp3

#### PotatoMeister

#### PotatoMeister

 Level 4

JoinedFeb 5, 2020Messages51

I tried this setup and it works just fine when testing the map in the editor but for some reason when hosting the game in multiplayer it only plays the playlist once. Any idea on how to fix that?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

What setup you used?

PlayMusic?

SetMapMusic?

file war3mapSkin.txt?

Regardless of above, the music files need to have some specific music settings otherwise they wont loop.

###
 Custom Music Lists


Using Custom Assets is a great thing since the filesize limit increase, and music is no exception, but the question is: how do you play that music? 1. A Way To Replace Default Music The simplest way if you don't need the default one is by replacing the default music files with custom ones. How...

 www.hiveworkshop.com


#### PotatoMeister

#### PotatoMeister

 Level 4

JoinedFeb 5, 2020Messages51

Tasyen said:

 What setup you used?

PlayMusic?

SetMapMusic?

file war3mapSkin.txt?

Regardless of above, the music files need to have some specific music settings otherwise they wont loop.

###
 Custom Music Lists


Using Custom Assets is a great thing since the filesize limit increase, and music is no exception, but the question is: how do you play that music? 1. A Way To Replace Default Music The simplest way if you don't need the default one is by replacing the default music files with custom ones. How...

 www.hiveworkshop.com


Click to expand...

I don't have the triggers anymore as I use a different method now but it kinda looked like this;

PlayList
Events
Map initialization

Conditions

Actions
Set VariableSet PlayList = Sound\Music\mp3Music\mymusic1.mp3;Sound\Music\mp3Music\mymusic2.mp3;Sound\Music\mp3Music\mymusic3.mp3;

Custom script: call SetMapMusic(udg_PlayList, true, 0)

Sound - Clear the music list

Sound - Stop music Immediately

Custom script: call PlayMusic( udg_PlayList )

The trigger works fine in singleplayer but in multiplayer it breaks. It won't repeat the playlist nor will it jump back to the standard music list unless the "playmusic" trigger is used. It's a shame as it seems to be the best and easiest method of creating multiple playlists. The real issue is that for some reason blizzard never added in a music variable option.


 Last edited: Apr 3, 2024

 You must log in or register to reply here.


### Similar threads

Locked

[Solved]
 Playlist with start song


FeelsGoodMan

Feb 13, 2020

Triggers & Scripts

Replies4Views2K

Feb 14, 2020
FeelsGoodMan

X

Locked

[JASS]
 How do I set PERSONAL music with JASS?


Xifos

Aug 1, 2004

Triggers & Scripts

Replies3Views2K

Aug 12, 2004
Tommi

Locked

 Music PlayList


Tasyen

Jul 3, 2017

Advanced Scripting

Replies0Views2K

Jul 3, 2017
Tasyen

Locked

[Trigger]
 making Playlist with path


_lycan

Jan 16, 2012

Triggers & Scripts

Replies1Views1K

Jan 16, 2012
Arhowk

Locked

 Region Sounds


FeelsGoodMan

Jun 9, 2019

World Editor Help Zone

Replies3Views5K

Oct 1, 2021
Kakerate

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