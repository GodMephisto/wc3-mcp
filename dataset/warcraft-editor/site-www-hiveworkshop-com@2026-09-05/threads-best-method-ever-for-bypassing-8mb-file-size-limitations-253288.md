---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/best-method-ever-for-bypassing-8mb-file-size-limitations.253288/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Best Method Ever for Bypassing 8mb File Size Limitations
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/best-method-ever-for-bypassing-8mb-file-size-limitations.253288/>
#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

 Local File useage of file paths


Getting tired of that 8mb limit on official bnet servers and still want to play on blizzard's official bnet servers instead of using an edited game.dll for private or un-official servers or custom MPQ's which takes hours to get to everyone? Well with this you only have to teach people two things... One would be how to enable local files while the other thing is where to put it.

Remeber you must put all files inside your main warcraft III directory where the game is and all the MPQ's as well remind everyone who plays your map or with you to do the same.

An example of using it;

Not a mapmaker and want to skip right to the instructions? Click Here

Object Editor;

Model path of object

Foldername\model.mdx

This is the same for everything else such as icons and textures too.

Unless the textures are already inside the model then you just need to make sure the model path works/is correct.

If you need further help or something you do not understand then feel free to ask away and post here or send me a VM/PM.

Don't try to do exact path locations, it doesn't work. Stuff like C;/bla/bla/Warcraft... Just put it in your default wc3 folder..

Extra

Foldername\texture.blp

Foldername\soundfile.mp3

It's pretty simple, just need to name your stuff and let people know how to enable local files and then download the required files to see and hear everything that is suppose to occur.

You can either do it by going to the registry which is where usually programs write too and store their data there. BE VERY CAREFUL AND FOLLOW THE INSTRUCTIONS VERY CAREFULLY IF YOU DO NOT KNOW WHERE YOU ARE AND WHAT YOU ARE DOING!

You can also do this by playing certain maps that preload a download program that alters the registry for you. You still have to run the program/file they preload for you.

INSTRUCTIONS!

First you need to know how to run stuff and no I don't mean double click or right click then left clicking run.

You can usually find it in your windows menu or if on windows 8+ then you find it in app's and it should be around your command prompt.

Once you managed to click "Run" then a box and a bar inside it appear, it looks similar to the search function somewhat.

Type regedit

Press enter or click ok/run/continue/whatever it is for you

A big box will open, don't freak out. That is your registry, one of the most important things on your computer. Now you don't want to mess with it even if you know what your doing so we will proceed with extreme caution.

We will now find the folder named HKEY_CURRENT_USER

Once found open it

Proceed to software then open it

You should find a folder named EXACTLY Blizzard Entertainment

Next you'll see a folder named Warcraft III, don't double click or anything to open it. Just click on it once so we can view the contents of it.

Next we will right click "Allow Local Files" Then left click "Modify"

Then a box will appear with its name and Base which is followed by its two bases WHICH YOU SHOULD NOT TOUCH! UNLESS! Hexadecimal is unchecked, MAKE SURE IT IS CHECKED. By checked it'll have a black dot/circle within the white circle/bubble.

There is one value there I didn't mention yet, "Value data:"

It has a small box which allows numbers in it, by default it should be 0 or blank. Type 1 into it then click ok.

You can now close the registry and congratulate yourself on enabling local files for your current computer.

REMEBER ONCE YOU ENABLE THIS YOU MUST BE VERY CAREFUL IN WHAT YOU DO ON WARCRAFT III

Because well some people just find it funny to be purely evil which is very depressing so keep an eye open at all times as long as you have it enabled.

To answer some questions;

Yes you can mess up big time if you refuse to follow my instructions.

No Blizzard can not ban you for this.

They added this themselves so it is perfectly fine and normal to use this.

Yes people can "hack" you once you enable this HOWEVER only if you really fall for dumb crap like that.

No I will not reveal/tell you what to look out for and watch for because that itself is basically telling how to do it.

Just check daily before turning off your PC as well if your really concerned about this stuff then after playing the map you really wanted to try that required this method you just set it back to 0 which means off.

1 = on

0 = off

That is the values Blizzard used for this.

Quick Edit: I would appreciate it if anyone could provide pictures for the registry and at the part of entering 1 to allowing local files as well the run menu. I don't have the tools required to do this myself so I hope someone will be able to help.

Click Me if you want to go back to the top

Important Infomation

You still need to let people download all the files required for your map.

You have to restart Warcraft III in order for this to start working if it is running when you did this.

A helpful off-topic tip that might prove to be useful to anyone.

Restarting Warcraft III is a good thing once in a while to free up memory and might help reduce fatals/crashes and lag.


 Last edited: Aug 11, 2014

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

this is exactly like using a custom MPQ... the only difference is that those folders you use are inside the MPQ. And this don't fix the problem, you can't just enable local files and you've bypassed the 8mb limit, the download of everything still remains and it'll be just as big as using the MPQ method.

Either that or I have misunderstood the tutorial completely.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

It's easier then using a MPQ as well less to type.

Though yes you are right, it is very similar to custom MPQ's. I will go even as far as MPQ's are better due to them being less messy HOWEVER they're harder to use and people are more likely to use this over that just due to well think GUI vs JASS.

Since people have resorted to using edited game.dll's instead of the method you made a tutorial about is proof of what I am trying to say here. People are willing however if it is complex then some turn away.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

while I can see how it seems easier to use.. actually, nevermind I wont argue about it.

I'd suggest new structure for the tutorial anyway, because it seems rather messy and ugly in the current state. Different sizes everywhere, "random" underlines etc.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

I had a few friends read it and they were able to understand it better this way.

I might consider it however I actually like it and consider that this structure is better despite its messiness.

Thanks again Chaosy, it might be near-identical to your method however you can separate downloads into 1,2,3 steps instead of one giant file.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

This is the same as other methods. Your oversized font and red color is hurting my eyes. There also was a tutorial about using this method. I guess this thread can go to the graveyard now?..

8 MB Map Size Limit Remover is much faster than this method and requires the same- both players having it.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

There are some advantages for keeping files outside the map.


1. Shorter save times


2. Smaller downloads, once you have the initial pack (new map versions usually differ in only some kilobytes)


I prefer local files, especially since there are never problems with file paths, as long as the locals have been organized neatly (which they should be anyway).


What would really make it easy would be a single executable to turn local files on in the registry. I know this can be done, because JNGPE does this among other things.

#### GreeN!X

#### GreeN!X

 Level 23

JoinedOct 20, 2012Messages3,068

Resources: 4

#### Resources
Icons 1Maps 1Templates 1Tutorials 1

What's with the oversized texts and colors? It looks like you're shouting every paragraph or so. I suggest you format the tutorial properly.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

I'll fix the structure, I did use too much red now that I look at it again.

genin32 this isn't the same, it is the best of the others. Can you link me to that tutorial though? It's not nice to guess a thread to graveyard you know.

Is it really? Sure it requires the same, but can it be played on Battle.Net?

How long are the wait times in those lobbies as well? Are you really sure about that?

Thanks for the comments guys.

Awesome Xonok and it is better that they know how to modify this in the registry then have it done for them. Yes there is a executable for that however would they know how to turn it off? Nope unless someone made another for that when they could spend 2 minutes learning this way instead.

Edit:

GreeN!X said:

 What's with the oversized texts and colors? It looks like you're shouting every paragraph or so. I suggest you format the tutorial properly.


Click to expand...

Working on it as you posted.

Edit2: Is this any better?

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Dat-C3 said:

 I'll fix the structure, I did use too much red now that I look at it again.


genin32 this isn't the same, it is the best of the others. Can you link me to that tutorial though? It's not nice to guess a thread to graveyard you know.


Is it really? Sure it requires the same, but can it be played on Battle.Net?


How long are the wait times in those lobbies as well? Are you really sure about that?


Thanks for the comments guys.


Awesome Xonok and it is better that they know how to modify this in the registry then have it done for them. Yes there is a executable for that however would they know how to turn it off? Nope unless someone made another for that when they could spend 2 minutes learning this way instead.


Edit:


Working on it as you posted.


Click to expand...


There's very few cases where local files would cause any problems, so I don't think it ever needs to be turned off.


Although in that case what could be done is making the executable just have a toggle.


Something like "Click to turn locals on/off" depending on whether they are turned on.


The reason for such luxury is that most people won't bother putting so much work into playing a map.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Local File method will require everyone to add files and data of that particular map in the local folder of War3. That kind of thing will take up the disk space of your computer either way. And you will still eventually have to download the "map" and "map files". Both players will have to have Local Files turned on in the registry. That means you cant simply host it and play it with those who don't have Local Files on. It is too much trouble and the method is almost the same. The .dll however (8 MB Map Size Limit Remover), is fast to download, you only have to move it to your main War3 Folder and you can play with others that have it too.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Sounds like your method is identical to mine in terms of obtaining it. Your edited file will force maps to take up more disk space anyways. Downloading the map will take 1-5 seconds while the map files take maybe at most three minutes depending on the player. That's fine, people can learn how to turn it on. It's more trouble having to grab a edited file and then remembering not to replace your original one not to mention you can't play 9mb maps with those who don't have your .dll... We're at a stalemate you know.

No use in discussing it further, yours is useful if people want to play 9mb+ maps on un-official servers while mine is useful if you want actual efficiently and playing on official servers or un-official. If someone can't follow these instructions then I surely don't want them playing my maps. That's my honest opinion. The only difference being one is easier while one is slower and harder to use.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

One advantage for local files is that it scales better the more people use them(Only need to download each model once). For this reason I have tried to advocate a community pack of locals, but it was shot down due to people not being able to remove their models from it (or older maps would break).

For instance, I have 6 versions of gaias retaliation. That is 48MB for essentially one game. With local files it would reduce to perhaps 10 MB and this is just one map.

#### GreeN!X

#### GreeN!X

 Level 23

JoinedOct 20, 2012Messages3,068

Resources: 4

#### Resources
Icons 1Maps 1Templates 1Tutorials 1

Dat-C3 said:

 Edit2: Is this any better?


Click to expand...


Well, that's better and a bit more readable. But I suggest using bold tags only on important text and avoid using text size for the contents that are larger than the title itself.


I won't join in the discussion on the tutorial's contents though since genin and Xonok seems to be doing that already.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

give me one reason why any of the text should be of such huge size?

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

edo494 said:

 give me one reason why any of the text should be of such huge size?


Click to expand...


Well I tried to make it a better tutorial then usual since people said my last one was too.... bland I think would best describe it.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

bigger != better

also if it was at least consistant, but some of the text is of size 20 lets say, some is 26 some is 13

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Xonok said:

 What would really make it easy would be a single executable to turn local files on in the registry. I know this can be done, because JNGPE does this among other things.


Click to expand...

blub

Ps: https://www.hiveworkshop.com/forums...t-tutorial-please-read-before-posting-119320/

#### Attachments

AllowLocalFiles.zip

 354 bytes

 · Views: 250



#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Ooh, nice. Exactly what I wanted.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Note, JNGP can do the Local File enable with one click from the menu bar.

I've been using this method since Nov last year. I've had a problem with custom texture models, yet MPQ Editor and Magos' Model Editor solved that for me.

a. Create an MPQ in Magos' Folder.

b. Add custom textured models/textures to that MPQ. It must be the exact path you're using in your map with local files. Be sure that the files exist in your local folder version too.

c. Open model with Magos' Editor.

d. Magos' Editor MPQ Browser, select your MPQ.

e. 'Use' the corresponding textures (Right Click -> Use).

f. Change the textures in Material Manager.

g. Done.

Also note, Magos' cannot do the same with icons. Though it can open BLP files, it only saves .mdl/.mdx files.

One more note. The MPQ here is only a tool, and will not be included with your local files. It only helps Magos' Editor set path strings.

Vote for approval. At least the content. Not the text size.


 Last edited: Jun 22, 2014

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Not everyone knows how to disable it as well not everyone uses JNGP. Exact paths are bad since they won't work for everyone. You should just do Foldername\filename and I have never seen it fail once yet.

Make sure all files are in the folder and the folder is in the Warcraft 3 folder. That way it will work for more then just English people and it'll work for other languages too as well people who uses different disc's.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

I was talking about models with custom textures...

The texture will be searched for in the Warcraft 3 directory, where as everything should be in a folder inside the directory, hence the texture will be nullified and units will appear as shadows.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Then you might be doing something wrong. . . because well that is what I meant.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

For example the custom texture has a path of Texture\, you mean it will work if placed in the same folder as the model? For me it does not.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Paths can either be absolute (C:Stuff\lolz...) or relative (...\Warcraft III\Path)


All paths first check in the map, then locals(if enabled), then the MPQ.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

@Xonok I see.

@Dat C3 I use 'relative paths', how does that not work for everyone?

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

CakeMaster said:

 For example the custom texture has a path of Texture\, you mean it will work if placed in the same folder as the model? For me it does not.


Click to expand...


Try making a folder inside the folder with the model named Textures then place the texture in that folder.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Any solutions that require the use of local file support will never be successful in the long run, simply because there is no way to force local file support through the map itself for the random guy on Bnet.

Nestharus used an approach that auto-downloaded an executable that enables local file support in his codeless save/load, but even that required the user to manually run the executable and restart WC3 before it could work.

You guys need to accept that custom MPQs or anything like that will only be feasable in multiplayer applications for local content that can be overwritten without risking desyncs (for example: overwriting the standard WC3 soundtrack with custom music tracks ... this would mean that even those people without the edited MPQ will be able to play this game online; they just don't hear the same tracks).

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

just a little correction, it wasnt executable and wasnt downloaded, it was merely batch file that you could run and it would rewrite the registery key inside registers of Windows

#### JC Helas

#### JC Helas

 Level 14

JoinedOct 19, 2014Messages187

Resources: 12

#### Resources
Icons 1Maps 2Spells and Systems 9

wow this cool

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

A lot of people look for these methods. While it isn't as practical for general play/distribution, it can still achieve success for large mods and among small circles of friends. And it can be neat for optional data (e.g. higher quality mp3's optionally loaded, higher res textures), but I won't go into that.

The tutorial content itself is approvable. But the formatting is still weird, lol. Just make everything the same text size, and make the headers bold/colored or whatever. Maybe link to WaterKnight's blub post for the automagic exe that enables local files. Optionally, an image or two will also prevent the tutorial from appearing as a wall of text.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Okay, will see what I can do. Though I might need a reminder if I take too long... =P

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Reminder.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

So, is there anything you will change?

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

IcemanBo said:

 So, is there anything you will change?


Click to expand...

Thanks for the reminder, my apologies for not getting to this sooner. I'll see what I can change and fix/edit this week.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Sure, take the time you need.

Just wanted to get up to date.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Submission:Bypassing 8mb file size

Date:31 December 2016

Status:Graveyard

Rating:none

Note:

Issue was outpatched.

Thanks for contribution, though. :}

edit: When you plan on updating the tutorial, with also brining up new patch changes you can ping us anytime.


 Last edited: Dec 31, 2016

 You must log in or register to reply here.


### Similar threads

 Create a >8MB Map (MUI)


Grey Nightmare

Mar 23, 2010

Tutorial Archive

Replies15Views7K

May 2, 2010
SasuhKun

 Creating your own Warcraft 3 Background


AJalex

Jan 10, 2008

Tutorial Archive

Replies6Views25K

Jan 30, 2008
AJalex

 WC III Icons For Windows and Linux


DesKaladA

Sep 6, 2007

Tutorial Archive

Replies10Views10K

Aug 9, 2008
hive01

 How to Import sounds/music into wc3 from WoW


The Darkness

Dec 1, 2006

Tutorial Archive

Replies2Views4K

Dec 5, 2006
ArchWorm

 Bypassing doodad limit (Reforged+)


FeelsGoodMan

Jun 17, 2025

Tutorial Archive

Replies11Views2K

Jul 1, 2025
FeelsGoodMan

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