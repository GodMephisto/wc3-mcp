---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/custom-mpq.246641/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Custom MPQ
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/custom-mpq.246641/>
#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

#
 Custom MPQ tutorial step by step

##
 by Chaosy

##
 Introduction

I have seen this question many times recently so I decided to make a short tutorial on how to do it.

If you don't know anything about using a custom MPQ it basicly allows a multiplayer map to extend the 8 mb limit on battle.net.

We put models and skins in one of the MPQs that Warcraft 3 uses to run the game and save them there instead of in the map.

However everyone who plays your map will need this modified MPQ so it would be smart to tell those

who download the map to backup the MPQ before they download yours.​

##
 What is needed?

An MPQ editor I would suggest this one - Ladik's MPQ editor

Warcraft 3 (no shit Sherlock)​

##
 Let us start then

First of you need to locate your WC3 directory. Normally found at "C\Program Files\Warcraft III"

In your WC3 folder you will hopefully find some MPQ files, I have choosen to edit the "War3x.mpq".

So open it up with your MPQ editor.

Sorry for the Swedish language but I still think the pic is helpful. In English it says "open with"

Once the editor is open it should look like this.

Now, you should create a new folder to put for stuff in. Right click "mpqname.mpq" in the left folder section and then "new folder"

Now that we got our folder we can start adding files. Make sure you select your folder.

I will add this model

Anyway, once the model is imported it should look like this.

Now open the WE and get into the object editor. Once in the object editor you choose a unit and then go to the model field. Then you put the path for your model. "FolderName\modelname.mdl"

Once you have done that youre done your model should now appear ingame.

##
 Hey! You have not showed how to use skins and textures!

I waited with this because I wanted the basic method to be fully understood.

Icons - Icons are imported exactly the same way as models you just put them into a folder and then use the path. However it's smart to organize your files by having one folder for models and one for icons. For example you can create a new folder inside your first folder. So the paths could be "AwesomeMap\Models\ModelName.mdl" and "AwesomeMap\Icons\IconName.blp"

Skins - A downloaded skin needs to be imported at the path the uploader tells you too.

so in this case we click on the unit folder and then the creep folder and so on and then we replace the current texture with the downloaded one. Not harder than importing a model

Textures - Those needs to be placed at the base of the MPQ, meaning not inside a folder. Notice when you import a .blp file via the import manager you remove the "war3mapimported\" from the path right? That tells us that the textures we import wont be placed in the "war3mapimported" folder. It should be placed within the red markings

Example:

I got a WoW model (not ripped by me) it has the following files:

and then we place the model into our folder

then we place the textures at the base of the mpq

set the path ingame

profit

However this is just for most textures. Some textures uses different paths but then you just import them as they were a skin and put them at the path provided by the model creator.

If you don't like having your texture files at the base of the MPQ you can actually fix that by editing the models the texture(s) are attached too. However I am no model expert so I can't show how to do that part.

However I found this thread that should explain it to you.

Sound and Music - These are rather simple to import but it seems that wc3 doesn't load every music file found in the music folder. You need to replace some original sound / music in order to make it display in the sound editor. In war3x.mpq uo go to Sound\music\mp3\and replace one of the files there​

##
 Credits

Sindorei300 - Featured model​


 Last edited: Feb 24, 2021

#### IamFootman061

#### IamFootman061

 Level 7

JoinedDec 26, 2013Messages230

Resources: 3

#### Resources
Maps 3

Quite useful but those screenshots had other language.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

there was only one of those and I wrote the translation anyway. I wont re-download my windows 7 to update one picture

#### neo_sluf

#### neo_sluf

 Level 18

JoinedFeb 5, 2012Messages1,685

Resources: 10

#### Resources
Icons 1Maps 9

Nice tutorial... though some players and most in our place... Internet Cafe' computers are locked and you can't open wc3 folder. So there will be still a problem.

Actually if this method is really good and convenient Icefrog would use this in Dota since dota is now on 8.3 mb and 0.4 mb more it wont reach the limit.

But again this is good... nice tutorial +rep..

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

thanks

yes this would not work on lans unless you got acces to the files. But if you dont got acces to the files you wont be able to download maps either so.

edit: added a example with a model which use 5 textures.

edit: added a short explaination on how to import sound and music into the MPQ


 Last edited: Jan 11, 2014

#### Sunreaver

#### Sunreaver

 Level 18

JoinedAug 22, 2013Messages1,290

Resources: 16

#### Resources
Models 2Packs 1Maps 13

Thanks this helps a lot that I change the original unit skins to the one's 67Chrome made

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

yeah it can be quite useful!

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

What happens if I replace the default music files of WC3 with custom ones and only some players in the (bnet) game have the altered MPQ?

Will there be a desync or crash or will the other players just play the default files instead?

This could basicly be a perfect method to allow unlimited custom mp3 imports at high quality.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

I guess it won't desync, but the risk is mainly the data send will be different (some got no music while others have)


 Last edited by a moderator: Jun 5, 2015

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I think everything would be invisible..

since the game will look for a model/texture at a certain location, if it cant find something there the unit or whatever will be without model (invis)

just set model path for a footman to yoloswag1337.mdx to test.

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

This is a neat trick, and a potentially useful tutorial.

However I think SharpCraft is a better option now.

Maybe you could state that somewhere in the tutorial?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I have not tested SharpCraft out. Or well I have but I have not checked the features. Once it's released I will take a deeper look.

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

Chaosy said:

 I have not tested SharpCraft out. Or well I have but I have not checked the features. Once it's released I will take a deeper look.


Click to expand...

It's been released for quite a while now.

Not to mention you won't have to download a +1GB MPQ just to play a map.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I thought it was in beta stage, my bad then I guess.

Anyway, you can make really smart use of this, you only add textures in the MPQ this way the map will be playable without the MPQ but it will enchant the experiance.

For a optinal solution, (not TM) you could merge 2 existing MPQ's into one leaving one empty. That would reduce the download size by a whole lot.

You could even extract the files directly into the WC3 directory. (this would mess up the folder completely, but it would be a solution to the size problem.)

#### The Algerien

#### The Algerien

 Level 2

JoinedJan 7, 2014Messages22

need engish screenshots . no one understands swidish!!!!!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

The Algerien said:

 need engish screenshots . no one understands swidish!!!!!


Click to expand...


The only part that is Swedish is the first screenshot. It is just right-click -> Open With -> Ladik's MPQ Editor.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

+I wrote the translation.

#### Paillan

#### Paillan

 Level 20

JoinedJan 22, 2011Messages3,996

Resources: 7

#### Resources
Models 3Maps 3Sounds 1

I just had a doubt... can you work with spells of war3?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

hm, what do you mean? Spell effects?

#### Paillan

#### Paillan

 Level 20

JoinedJan 22, 2011Messages3,996

Resources: 7

#### Resources
Models 3Maps 3Sounds 1

no, spells itself, open them like a notepad++ or something...

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

well.. if you find a file with spell data or whatever you can replace the original one for sure. However I doubt blizzard use a flamestrike.xxx

#### Paillan

#### Paillan

 Level 20

JoinedJan 22, 2011Messages3,996

Resources: 7

#### Resources
Models 3Maps 3Sounds 1

well... right, I would try if I could but I can't.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

I was hoping for custom MPQs and Local Files. :<


Nonetheless a nice tutorial.

#### sigmaceti

#### sigmaceti

 Level 2

JoinedJun 21, 2014Messages11

is it possible to add files in MPQ. but so that WE can show it like other files in that MPQ.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

It is.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

sigmaceti said:

 is it possible to add files in MPQ. but so that WE can show it like other files in that MPQ.


Click to expand...

I think a model needs to be in certain folder in order to be displayed with the others normally. Like the Unit folder for example.

#### noob

#### noob

 Level 14

JoinedSep 28, 2011Messages968

Resources: 5

#### Resources
Maps 4Tutorials 1

You just need to change the WE string data and stuff of this kind.

The list of the 3D models that the editor will show is in the WC3 MPQ somewhere.

So if you use custom MPQ you can add models to the world editor list for example the hidden ones.

#### sigmaceti

#### sigmaceti

 Level 2

JoinedJun 21, 2014Messages11

problem is I dont know how to do it, any tutorial around?

#### noob

#### noob

 Level 14

JoinedSep 28, 2011Messages968

Resources: 5

#### Resources
Maps 4Tutorials 1

Ask the creator of UNSWE who surely know many about this.

#### Zed

#### Zed

 Level 7

JoinedFeb 22, 2014Messages280

Chaosy, You can change a model's texture path by using MDX Pather

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Zed said:

 Chaosy, You can change a model's texture path by using MDX Pather

Click to expand...


Right, that's pretty useful! I might mention that in the tutorial then.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

Well, using local files is always become my best solution for this kind of matter. It basically allows you to do the same thing, without modifying the default MPQ, and without requiring any tool.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

You'd have to have a big chuck of ugly folders in your warcraft directory though.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Well, you can create a "Local" folder and put all the ugly folders in there. xD

Path editing would be a thing though if the map already has local material.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Impossible. That will only work with models. For textures/icons that's impossible. Unless you want to change every single blp path.

edit: I found out that you can hide the folders, but then you are unable to modify them afterwards.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

I don't use Locals for icons personally, they are little that ~600 of them are okay.

This thread is about MPQs anyways so okay.

#### Paillan

#### Paillan

 Level 20

JoinedJan 22, 2011Messages3,996

Resources: 7

#### Resources
Models 3Maps 3Sounds 1

About spell effects, where do you find them anyway? Searched scripts and such stuff.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Just go here:

"Abilities\Spells\Human\Thunderclap\ThunderClapCaster.mdl"

Note that spells from Reign of Chaos is in the War3.mpq and Frozen Throne stuff is in War3x.mpq

#### Paillan

#### Paillan

 Level 20

JoinedJan 22, 2011Messages3,996

Resources: 7

#### Resources
Models 3Maps 3Sounds 1

those are the models I was talking about the code of spells actually.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Oh right. I don't know then ^^

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Could be useful. It would've been nice to go into the different techniques and weighing them (e.g. local files), but I suppose this is good as well. This method could actually work really well with an exe to automatically inject your files (so it is a very good option for mods), approved.

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

Well this would be really sweet so you didnt have to download the same models that you've already downloaded for every map you play.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Aye. There are actually a few Warcraft HD mods on MODDB which more or less are MPQs with custom models.

#### ToastAddict1057

#### ToastAddict1057

 Level 4

JoinedOct 2, 2015Messages76

When i finish and open WE


it says


"Unable to open War3.mpq"

#### ToastAddict1057

#### ToastAddict1057

 Level 4

JoinedOct 2, 2015Messages76

Also do i need to do this proccess in every map i go? (setting the path in game)

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

1. Because you still have the MPQ editor open I suppose. Make sure to close it before starting the WE.

2. No. But if you want it to be applied to every map you need to replace the original model.

#### WerBackIII

#### WerBackIII

 Level 14

JoinedJul 27, 2007Messages789

Resources: 11

#### Resources
Skins 5Maps 6

I'm sure there is a way to create a truly custom MPQ which is accessed trough a custom EXE file (DoC mod does this, as well as the non-functional Divine Right, and Nirvana). Is this hard to implement?

#### Kaien Shiba

#### Kaien Shiba

 Level 2

JoinedSep 9, 2015Messages19

How will I organize the models and put them in Units and in their corresponding folder?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Enter the Units folder and create a new folder there.

You can put them in existing race folders as well.

#### Kaien Shiba

#### Kaien Shiba

 Level 2

JoinedSep 9, 2015Messages19

Can I make a folder inside another folder? Just for organizing.. When I made one, I put a model file then it won't appear after that when I reopen MPQ Editor the files are named Files00000000.xxx.. Please help

 You must log in or register to reply here.


### Similar threads

 Exporting files from a MPQ.


Deolrin

May 14, 2009

Misc/General Mapping Tutorials

Replies10Views19K

Dec 8, 2009
Newbestyle

 Customizing your Warcraft


Apheraz Lucent

Nov 29, 2011

Misc/General Mapping Tutorials

Replies36Views32K

Aug 31, 2021
conde

 How to: Fix the World Editor on Mac (Patch 1.24)


Death_X

Aug 10, 2009

Misc/General Mapping Tutorials

Replies3Views8K

Apr 30, 2010
PurgeandFire

Locked

 Please aid me


kyklos

Feb 27, 2005

Map Development

Replies3Views1K

Feb 27, 2005
kyklos

 Local files


BogdanW3

Feb 23, 2021

Misc/General Mapping Tutorials

Replies12Views30K

Dec 7, 2025
Moozore

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