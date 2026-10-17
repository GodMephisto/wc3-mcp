---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/custom-team-colors.276448/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Custom Team Colors
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/custom-team-colors.276448/>
#### Shar Dundred

#### Shar Dundred

 Level 81

JoinedMay 6, 2009Messages6,446

Resources: 15

#### Resources
Packs 1Skins 1Maps 12Tutorials 1

##
 Legends of Arkain Series


Custom Team Colors
by Shar Dundred

Introduction

I'm pretty sure there have been many attempts adding more custom player colors to be picked from for WarCraft III - both with and without success.

Personally, I've always found the limited amount of team colors quite disappointing and wanted to change that. Therefore, I searched for a solution, tinkered around, read through several threads about this matter...

To cut it short: It IS possible to replace the existing team colors quite easily, but the solution requires editing the mpq file. Importing edited team color files into a custom map will not replace the team color with a new one.

It is not possible to change the team color for one specific map only, you can only change your own game. It appears that the game takes the team color & glow directly from the war3.mpq file and ignores all imported files trying to overwrite the colors.

What is this tutorial about?

This tutorial will show you how to edit WarCraft III's mpq file to replace ingame team colors with custom ones.

I highly advise you to create a backup of your war3.mpq file before proceeding!

Tools

You're going to need a program that allows you to edit .tga files (like GIMP)

You'll also need a tool that allows you to open and edit mpq files (like MPQMaster)

Finally, you're going to need a tool that lets you convert the .blp files from the war3.mpq into .tga files and back into .blp files again (like BLP Lab)

I'll use the linked tools for the tutorial.

Got everything? Alright, let's start!

1) Create a backup of your war3.mpq file. The original file should be located in your main WarCraft III directory.

2) Start the MPQMaster and open the war3.mpq with it. When it asks you to select a listfile, select the WarCraft III.txt.

3) There are two files you have to edit: The TeamColor.blp and the TeamGlow.blp. The color file can be found in ReplaceableTextures\TeamColor, the glow file in ReplaceableTextures\TeamGlow.

You might ask yourself "Why are there 15 colors? Even with the neutrals, the game includes 13 team colors in total!".

Yes, you can only select 12 colors ingame and 13 colors when using the "Change Player Color" action in the Trigger Editor, but there's a total of 15 player slots in WarCraft III.

TeamColor00.blp is for Player One (Red) and TeamColor11.blp for Player Twelve (Brown), 12 is Neutral Hostile, 13 Neutral Passive, 14 Neutral Victim and 15 Neutral Extra (the last two could also be switched, but that doesn't matter here anyway).

4) Now we select the file we want to replace.

For this tutorial, I'll replace Pink with Dark Red. Pink is, normally, the color of Player 8. The files we'll have to change are therefore TeamColor08.blp and TeamGlow08.blp. Extract both from the war3.mpq

5) Open the BLPLab and open the .blp files. Then, use "Save as..." to save them as .tga files.

6) Open both files with GIMP (or whatever tool you use to modify .tga files) and modify them the way you want. Once you're done, save the files again.

Do not forget that you have to change the TeamGlow.blp as well, else you'll have only changed the team color, but not the hero glows.

7) Use BLPLab again to save both files as .blp again.

8) Open your war3x.mpq with mpqmaster and open the "ReplaceableTextures" directory. Then, right-click and create two new folders. Call them TeamColor and TeamGlow.

9) Put your custom TeamColor.blp in the TeamColor directory and your TeamGlow.blp in the TeamGlow directory.

10) And done! Congratulations, you have successfully replaced the team color and team glow - both in the game itself, its menu and in the world editor!

Known Issues

It has happened that the world editor would crash when replacing existing team colors with custom ones. I can't tell what's the reason for this, since this doesn't *always* happen. However, should it happen to you, simply replace the war3.mpq with your backup before starting the editor and replace it with a copy of the modified mpq once you want to play with your custom color.

In the score screen, there still appears the old team color. I'm trying to find a way to solve that issue.

I'm well aware of the fact that this solution is only "meh" due to it being for one player's game only and doesn't allow you to make it map-specific, but it's the only way I know of.

If you know of another way to edit the team colors perhaps for one map only, please contact me or write it down in this thread.

I have attached the Dark Red color I created for WarCraft III to this post, feel free to replace your Pink (or any other color of your desire) with it if you want.

#### Attachments

Screen01.JPG

 76.5 KB

 · Views: 1,767



Screen02.jpg

 152.9 KB

 · Views: 1,685



DarkRedTC.zip

 1.4 KB

 · Views: 399




 Last edited: Mar 7, 2016

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

You don't need to edit any .mpq's. Just enable local files and overwrite from Warcraft 3 without messing up. Btw, it is possible add new team colors to a custom map via "War Club" ability and replaceable textures, but it will require you to change team color material to replaceable texture id for every unit/building model. Then apply the replaceable texture (custom team color texture) using War Club ability to units owned by player via triggers.

This tutorial is useless to be honest, especially since you mentioned that IT might crash.

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

How much percent are you sure that gonna work, Shar! But it still confusing me.

 It has happened that the world editor would crash when replacing existing team colors with custom ones. I can't tell what's the reason for this, since this doesn't *always* happen.


Click to expand...

No doubt, replacing existing data such as team color crash the editor since they weren't taking properly.

 but it will require you to change team color material to replaceable texture id for every unit/building model. Then apply the replaceable texture (custom team color texture) using War Club ability to units owned by player via triggers.


Click to expand...

So in order to replace the original team color I have to replace it on every single model, but won't that change only your own game?.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Warseeker said:

 How much percent are you sure that gonna work, Shar! But it still confusing me.


No doubt, replacing existing data such as team color crash the editor since they weren't taking properly.


So in order to replace the original team color I have to replace it on every single model, but won't that change only your own game?.


Click to expand...


NO, that will affect everyone who is playing the map. Every model has team color material, it's called - Replaceable ID 1. You replace that ID with 31-35 to match with destructible tree ID's.

Click here to learn more about changing textures in-game via War Club ability.

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Well yes I know that method. But I was reffering more to replace the exact team color with new ones,

Anyway, your method might work too

#### Shar Dundred

#### Shar Dundred

 Level 81

JoinedMay 6, 2009Messages6,446

Resources: 15

#### Resources
Packs 1Skins 1Maps 12Tutorials 1

##
 Legends of Arkain Series


A Void said:

 You don't need to edit any .mpq's. Just enable local files and overwrite from Warcraft 3 without messing up. Btw, it is possible add new team colors to a custom map via "War Club" ability and replaceable textures, but it will require you to change team color material to replaceable texture id for every unit/building model. Then apply the replaceable texture (custom team color texture) using War Club ability to units owned by player via triggers.


This tutorial is useless to be honest, especially since you mentioned that IT might crash.


Click to expand...


Feel free to create a tutorial about that to share your knowledge with the rest of us. I haven't seen such a tutorial.

Warseeker said:

 How much percent are you sure that gonna work, Shar! But it still confusing me.


No doubt, replacing existing data such as team color crash the editor since they weren't taking properly.


Click to expand...


I'm 100% sure that this works, I did it myself.


The only thing I cannot be certain about is the editor. When I first did this mpq editing, everything worked without crash. At some point, I had to reinstall my game and it didn't work anymore. I also got reports from others that their editor kept crashing.


I posted this here so people could learn a simple way editing their game and so that someone might come around with a solution how to fix the crashing issue.

#### Shar Dundred

#### Shar Dundred

 Level 81

JoinedMay 6, 2009Messages6,446

Resources: 15

#### Resources
Packs 1Skins 1Maps 12Tutorials 1

##
 Legends of Arkain Series


Double-post because:

I updated the tutorial since I now found a way to solve the whole crashing issue by putting the custom blp files into the war3x.mpq instead of removing the old ones. Now everything should work without crashing.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

you a wise man

#### RED BARON

#### RED BARON

 Level 35

JoinedOct 9, 2006Messages6,392

Resources: 56

#### Resources
Models 43Icons 7Packs 1Skins 3Maps 2

A useful tutorial if one wants to design their own colors for whatever map, more so now that it doesn't crash the editor. Still always be very careful with mpq editing. (Also yea, never remove game files, always replace)

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Shar Dundred said:

 Double-post because:

I updated the tutorial since I now found a way to solve the whole crashing issue by putting the custom blp files into the war3x.mpq instead of removing the old ones. Now everything should work without crashing.

Click to expand...

Howabout creating a new folder inside Warcraft 3, and name it as the team color path,

As thisTutorial explain . Would it Work?

#### Shar Dundred

#### Shar Dundred

 Level 81

JoinedMay 6, 2009Messages6,446

Resources: 15

#### Resources
Packs 1Skins 1Maps 12Tutorials 1

##
 Legends of Arkain Series


As I said, the game takes the colors directly from the .mpq file.

If it worked like that, importing the files to overwrite the team colors would function as well - which it doesn't. I tested creating a new folder as well as importing. No success.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Anyway, I am finishing my system. I will make a new tutorial about how to change team colors using texture changing method. It works for all players and looks quite nice.

Edit: Tutorial is up.


 Last edited: Mar 6, 2016

#### MasterHaosis

#### MasterHaosis

Patron

 Level 46

JoinedSep 18, 2007Messages2,331

Resources: 37

#### Resources
Models 19Icons 15Maps 3

Shar Dundred thank you for writing this tutorial.


Yeah guys, if you are not aware of it, I have been screwing around with lot of people trying out to figure how to replace or add new god damn colors, in past.

https://www.hiveworkshop.com/forums/world-editor-help-zone-98/custom-colors-214970/index4.html


For lot of time, and only what we succeed in past is this what Shar Dundred is writing about. Game completely ignores imported team colors and glows thus this can be changed only by editing mpq from game itself or, indeed allowing locals. Yes, you can separate as well neutral passive and hostile by giving them separate colors to different which unit belongs to hostile and which to passive. You can also add those colors to player via trigger, so player in map can be black (passive or hostile color). I am not sure about neutral victim and extra, maybe those could be assigned to player as well

A Void said:

 You don't need to edit any .mpq's. Just enable local files and overwrite from Warcraft 3 without messing up.


Click to expand...

Yes, this is better, but still this tutorial works as well as another possibility.


A Void, please make that tutorial as I am also interested to see how will it turn out. I also did not know of this

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

MasterHaosis said:

 A Void, please make that tutorial as I am also interested to see how will it turn out. I also did not know of this


Click to expand...


The tutorial is already up, you can find the link in my signature or the above post.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great that you got it to no longer crash! I gave it a quick read-through and it seems good. Although, you should remove this line, assuming you've fixed the issue:

 If you know how to fix the crashing issue with the world editor or even know of another way to edit the team colors perhaps for one map only, please contact me or write it down in this thread.


Click to expand...

I'll approve it after that update.

#### Shar Dundred

#### Shar Dundred

 Level 81

JoinedMay 6, 2009Messages6,446

Resources: 15

#### Resources
Packs 1Skins 1Maps 12Tutorials 1

##
 Legends of Arkain Series


Updated.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Approved!

#### Asasimaru Lightningstrike

#### Asasimaru Lightningstrike

 Level 2

JoinedAug 26, 2016Messages21

Can you guys find were is the blademaster team color?

#### Frotty

#### Frotty

 Level 23

JoinedJan 1, 2009Messages1,622

Resources: 13

#### Resources
Models 3Tools 1Maps 7Tutorials 1Wurst 1

Add to known Issues: Might get you banned on battle.net

Since any modification of the game files (war3x.mpq in this case) can get your keys banned if you play on battle.net.

Taking this from @Dr Super Good and my own experience, since my legit bought keys got banned recently for that reason I believe.

#### Marok

#### Marok

 Level 3

JoinedNov 23, 2023Messages18

How can I do this in Reforged?

 You must log in or register to reply here.


### Similar threads

 Customizing your Warcraft


Apheraz Lucent

Nov 29, 2011

Misc/General Mapping Tutorials

Replies36Views32K

Aug 31, 2021
conde

 Custom Team Colors Tutorial


A Void

Mar 6, 2016

Misc/General Mapping Tutorials

Replies25Views12K

Nov 2, 2020
fran bel

 Exporting files from a MPQ.


Deolrin

May 14, 2009

Misc/General Mapping Tutorials

Replies10Views19K

Dec 8, 2009
Newbestyle

Locked

 Having trouble with team color textures


MantisScreamer

Dec 30, 2008

Texturing & Surfacing

Replies1Views1K

Jan 2, 2009
LiOneSS

 Cinematic Movies Tutorial


A Void

Jul 16, 2014

Misc/General Mapping Tutorials

Replies18Views17K

Apr 19, 2020
pyf

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