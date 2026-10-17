---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/customizing-your-warcraft.207843/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Customizing Your Warcraft
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/customizing-your-warcraft.207843/>
#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

# Warcraft III

How to customize it?

With the help of this tutorial, you will learn how to effectively and simply replace existing files in Warcraft databases, without having to mess up the MPQ packs. This will enable you to have your favorite resources in any map you play!

## Content of tutorial:
I - Allowing Custom Files

II - Finding Resources

III - Replacing Files

IIIa - Skins

IIIb - Icons

IIIc - Models

IIId - Sounds

IV - Main Menu Background

### I - Allowing Custom Files

First of all, to get this started, you will need to edit Warcraft's registry, in order to allow your files to be applied. To do this, simply follow these steps:

1. Hit the "Start" button.

2. Open the "Run" sub-program.

3. Type in "regedit" (Without punctuation) and click on "OK"

4. When the Registry Editor opens, find the Main Folder called "HKEY_CURRENT_USER", and open it.

5. Open the "Software" sub-folder.

6. Search for "Blizzard Entertainment" sub-folder, and open it.

7. Click only once on "Warcraft III" sub-folder.

8. Now, you will see a list on the right side of the screen. Find the registry that says "Allow Local Files", and open it (Double click).

9. In the field of "Value Data", replace the zero (0) with number one (1). Make sure that the "Hexadecimal" flag is checked.

10. Hit okay. The registry you edited should display this in Data category: 0.00000001 (1).

You're finished with this part of the job. If your files are not being replaced, check again if you have edited the registry correctly.

 Hint: You can't replace files that are already in maps you play, and this mainly concentrates on Imported resources. Map's MPQ has higher priority than your files. Also, files you replace are only visible to you.

### II - Finding Resources

I won't discuss this much, since on this site, The Hiveworkshop, you will find plenty of Models, Skins and Icons to use.

But there is one thing you should keep an eye for: some models use custom skins, and some skins are made in two or more parts. Icons do not suffer from any problems, with an exception that you can find units in most maps that use same icons for their skills, which can look weird if the themes are not same.

### III - Replacing Files

This is the most important part, yet the easiest. All you will need to do is to find the resources you like, and seek the path. The easiest way to find the correct path for the resource you would like to display is to use the https://www.hiveworkshop.com/forums/tools-560/war3-model-editor-62876/, or the harder way with https://www.hiveworkshop.com/forums/tools-560/warcraft-iii-viewer-62878/. The process is simple now.

With Magos' Warcraft III Model Editor, just open the "Windows" and then "MPQ Browser", "File" afterwards, and select the MPQ you would like to open. "war3.mpq" contains files before the Frozen Throne, and vice verse for the "war3x.mpq".

With Warcraft III Viewer, just hit "Ctrl + F" and type first few letters of what you seek. Note that you will might have to open the right MPQ before you seek what you want. I still suggest Magos' Editor, since it's simpler, easier and a lot more functional.

Please see the example in skins so that you can understand all this better.

IIIa - Skins

I will use this skin of Jaina, by 67chrome. I opened Magos' Editor, and found the Jaina files in War3.mpq database, under "Units -> Human -> Jaina".

To successively replace Jaina's skin, enter your Warcraft III folder, which is usually found at "C:\Program Files\Warcraft III". Now create a new folder in it, called "Units". In newly created folder, create another one called "Human". Then, another one in it, called "Jaina". You see what I did there, and why I told you to use Magos' Editor? With it, the files you want to replace have the exact same path as their parent folders. If you look at your Editor, you will notice that the Jaina Resources are found under the path made of parent folders "Units\Human\Jaina". Each dash, or new folder, is one part of the path. Easy, huh?

To replace skins, just place the skin file in your main folder, which would be "Jaina" this time. You will have to rename the file to the same name which it uses in war3.mpq/war3x.mpq. I renamed my downloaded skin, the "BloodElfEnchantress" to "Jaina". The skin should be replaced now.

IIIb - Icons

Icons are the simplest one to replace. Find the icons you would like to use, and just make one new folder called "ReplaceableTextures". In it, go ahead and create two new folders, called "CommandButtons" and "CommandButtonsDisabled". Place the BTNYourIcon in the first folder, and DISBTNYourIcon in the second one. Remember to change names of the icons to what you want to replace, like "BTNJaina" and "DISBTNJaina".

IIIc - Models

Replace the models by paths. If the model uses custom textures, you should place the textures in exact folders as the models request, with same names, like "Textures\Princess_v2.blp"

IIId - Sounds

Just find the paths, and replace the sounds. Note that the sounds have to be the same format which Warcraft III uses, and that would be .wav type.

### IV - Main Menu Background
So you want that awesome Night Elf Archer to watch you in the main menus, rather than old and boring Frozen Throne? It is simple as this: all you need to do is to find what you want to replace. The usual path of campaign screens, in both Regular and Expansion MPQs is: UI\Glues\SinglePlayer

Find the one you want, extract it, and replace the parent folders in your Warcraft III folder in the way I explained up there. The only thing you will need is the .mdx file. The textures can be replaced if you wish to customize the Menu even more. The path (with parent folders) should be:

UI\Glues\MainMenu\MainMenu3d_exp\MainMenu3d_exp.mdx

We're done! I hope that, with the help this tutorial provided you, you will have some fun while creating your own Warcraft!

Back to the Top


 Last edited: Dec 23, 2011

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice tutorial! A useful link for those who want to mod their own warcraft mpq's.

~Approved.

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Update: added fourth chapter which explains how to customize your Warcraft III Main Menu background.

I am really close with models, by the way. I should be able to update this soon.

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Update: added model replacements.

#### gorillabull

#### gorillabull

 Level 17

JoinedJul 17, 2011Messages1,863

Resources: 5

#### Resources
Skins 1Spells 4

interesting.. for some reason when i enable local files wc3 can not load certain maps

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

that's weird... I never had problems with local files...

#### sigmaceti

#### sigmaceti

 Level 2

JoinedJun 21, 2014Messages11

what I need to know is can I add file whit out replacing them, in MPQ.

#### noob

#### noob

 Level 14

JoinedSep 28, 2011Messages968

Resources: 5

#### Resources
Maps 4Tutorials 1

Yes you can add new files without replacing old ones but it will be useful only for playing maps using those new files and did not imported them.

#### Zed

#### Zed

 Level 7

JoinedFeb 22, 2014Messages280

Nice

#### Strange1884

#### Strange1884

 Level 4

JoinedOct 20, 2010Messages59

Resources: 1

#### Resources
Maps 1

BattleNet music

Hey. It's a really nice tutorial and certainly using it!


Although I'm having trouble finding BattleNet music file. It's annoying when you sit in the lobby


waiting for online game to start and there's the same old boring music in the background.


Where can I find the file or is there a file for it?

#### Zed

#### Zed

 Level 7

JoinedFeb 22, 2014Messages280

Strange1884 said:

 Hey. It's a really nice tutorial and certainly using it!


Although I'm having trouble finding BattleNet music file. It's annoying when you sit in the lobby


waiting for online game to start and there's the same old boring music in the background.


Where can I find the file or is there a file for it?


Click to expand...

Browse through mpq files

#### Strange1884

#### Strange1884

 Level 4

JoinedOct 20, 2010Messages59

Resources: 1

#### Resources
Maps 1

Zed said:

 Browse through mpq files

Click to expand...


Okay, I realized that BattleNet actually plays all the music that you have in Warcraft 3 including race themes and such.


 I wonder though if you're able to make it so that it only plays one theme...

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

Delete all other themes except 1? Or replace all themes with 1.

#### vasyapro2005

#### vasyapro2005

 Level 1

JoinedSep 17, 2014Messages2

Nice tutorial! Thanks for it! I changed backgroud pic, main menu buttons, deleted lots of frames, disabled wind whistling which I hate and menu pop-up\pop-down sounds.

The last thing I need to do: disable delay between menu pop-up and pop-down. Like when you click "Local Area Network" and main menu hiding down for 1 second and LAN menu showing up for 1 second. I want to make it immediately. Any ideas how to do this?

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

That'd involve programming, unfortunately, out of my branch.

#### vasyapro2005

#### vasyapro2005

 Level 1

JoinedSep 17, 2014Messages2

Maybe you know someone who know something about that?

#### kamyflex

#### kamyflex

 Level 6

JoinedJan 4, 2014Messages227

Apheraz Lucent said:

IIIc - Models

Replace the models by paths. If the model uses custom textures, you should place the textures in exact folders as the models request, with same names, like "Textures\Princess_v2.blp"


Click to expand...

here i have a wow model with textures that has no path, which i figured out in magos model editor, shall i just put them in the same folder as the model ??

EDIT : Problem solved here : https://www.hiveworkshop.com/forums...98/custom-local-model-not-showing-why-259931/


 Last edited: Dec 3, 2014

#### Deleted member 242196

D

#### Deleted member 242196

Hello! Thanks for this awesome guide! Can i make a custom 3D main menu background ? If yes, can anyone write a tutorial ?


Regards

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

Dawnbringer said:

 Hello! Thanks for this awesome guide! Can i make a custom 3D main menu background ? If yes, can anyone write a tutorial ?


Regards


Click to expand...

Yes you can

#### Deleted member 242196

D

#### Deleted member 242196

LordDz said:

Yes you can

Click to expand...


Thanks!

#### Slaver

#### Slaver

 Level 4

JoinedApr 4, 2015Messages43

Hey, I've re-installed the game because my game has gone totally mute and I still don't know why, but since I've done so, the "Allow Local Files" option has gone missing as a registry; they end at War3XCD. I can see all my new models and textures perfectly in World Editor, but when I go in-game, they're the default textures. All textures are accounted for, etc, so I'm assuming that the missing registry must have something to do with it. Any info?

P.S. sorry for this being out of topic thing, but I've sent reports to the webmasters and no one has answered, so if any mods or someone with control can verify my account, that would be sweet; I've put in my verification code and resent alterations about 40+ times and I always get the "this URL is invalid" after I successfully enter it, and it never goes through for some reason.

#### kamyflex

#### kamyflex

 Level 6

JoinedJan 4, 2014Messages227

Solution 1 : Put this is notepad

 Windows Registry Editor Version 5.00


[HKEY_CURRENT_USER\Software\Blizzard Entertainment\Warcraft III]


"Allow Local Files"=dword:00000001


Click to expand...

save the file as EnableLocalFiles.Reg and run it


Solution 2 : Create a DWORD value at HKEY_CURRENT_USER\Software\Blizzard Entertainment\Warcraft III and name it "Allow Local Files" then set its value to 1.

#### Slaver

#### Slaver

 Level 4

JoinedApr 4, 2015Messages43

Wow, I didn't think it would be that easy. Thank you so much

One more thing though, while that put my models back into the game, etc, my Warcraft ENB lighting mod is only coming up in World Editor and not in game. I'm not sure that's related at all, but I was wondering if you might know. (sorry for asking for so much, I'd make a thread with these issues correlated but I'm having site issues)

Edit: I made it work by altering the affinity level. Now if Blizzard would answer my inquiries as to why my game is mute, I can actually finish this damn mod. Thank you for all the help kamyflex, you're my new bedtime hero <333


 Last edited: Apr 7, 2015

#### kamyflex

#### kamyflex

 Level 6

JoinedJan 4, 2014Messages227

Slaver said:

 Wow, I didn't think it would be that easy. Thank you so much


One more thing though, while that put my models back into the game, etc, my Warcraft ENB lighting mod is only coming up in World Editor and not in game. I'm not sure that's related at all, but I was wondering if you might know. (sorry for asking for so much, I'd make a thread with these issues correlated but I'm having site issues)


Edit: I made it work by altering the affinity level. Now if Blizzard would answer my inquiries as to why my game is mute, I can actually finish this damn mod. Thank you for all the help famyflex, you're my new bedtime hero <333


Click to expand...


lol ! i gave two defficult solution because i wasnt able to upload the reg file, you are welcome, feel free to ask, thats what the hive is for


What do you mean by "Warcraft ENB lightning" ??

#### Slaver

#### Slaver

 Level 4

JoinedApr 4, 2015Messages43

Warcraft ENB, the shadow/lighting mod. It works great.

https://www.hiveworkshop.com/forums/warcraft-editing-tools-277/enb-warcraft-3-a-237739/

Pictures from my personal mod

EDIT: HOLY SHIT that was bigger than I wanted it to be sorry, let me keep it as an attachment

#### Attachments

beforeandafter2.png

 3.2 MB

 · Views: 398



#### kamyflex

#### kamyflex

 Level 6

JoinedJan 4, 2014Messages227

Did not know about that, thanks for the info ! will be usefull in my Mod, but i'm not gonna be able to help you until i try it.

#### That_Guy_There

#### That_Guy_There

 Level 3

JoinedNov 10, 2015Messages50

Welp, I'm having problems with the files..


As if it won't replace them.


Anyway to fix this?


I did put the same name and path as the "Parent" Folder

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Did you edit the registry to allow custom files?

#### That_Guy_There

#### That_Guy_There

 Level 3

JoinedNov 10, 2015Messages50

Yeah...

I did everything on the post...

I made some skins work mostly Human skins... But, I can't make other races skins work..

#### That_Guy_There

#### That_Guy_There

 Level 3

JoinedNov 10, 2015Messages50

Yeah...


I did everything on the post...


I made some skins work mostly Human skins... But, I can't make other races skins work..

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

You're doing something wrong then. Which program you're using to locate where you should put the skins in and how to name then?

#### That_Guy_There

#### That_Guy_There

 Level 3

JoinedNov 10, 2015Messages50

Mago's Model Editor thingymadgigers

Some skins with two or more names (ie: HeroTaurenCheiftain,ChaosHellscream) Are not working for some reasons while other names (ie: Captain,Jaina,Proudmoore) Are working...

#### Syegfryed

#### Syegfryed

 Level 1

JoinedJan 28, 2016Messages2

im not have success with the two names too, want switch the "HeroBladeMaster" with the grom HellScream, but i cant

i create the "units" and "textures" folder with the hellscream mdx and blp, and rename with "HeroBladeMaster" doesn't work

its is something with the model? what can i do?

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Path with folders should be Units/Orc/HeroBladeMaster/HeroBladeMaster.mdx

Are you sure you did the folders right?

#### Syegfryed

#### Syegfryed

 Level 1

JoinedJan 28, 2016Messages2

never mid, i did it the suggestion with create the "allow local files" and work, sorry

the annoying thing, is i change the grom responses too, but the blademaster have too much responses, then he say botch voices, anyway to change this?

eddit, i just repeat the same responses again with different names, and work it


 Last edited: Jan 29, 2016

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Glad you got it figured out

#### conde

#### conde

 Level 5

JoinedNov 21, 2018Messages50

Someone knows how can I change location maps folder?

 You must log in or register to reply here.


### Similar threads

 Custom MPQ


Chaosy

Jan 9, 2014

Misc/General Mapping Tutorials
2

Replies76Views26K

Oct 2, 2023
raypack

 How to: Fix the World Editor on Mac (Patch 1.24)


Death_X

Aug 10, 2009

Misc/General Mapping Tutorials

Replies3Views8K

Apr 30, 2010
PurgeandFire

 Custom Team Colors


Shar Dundred

Mar 5, 2016

Misc/General Mapping Tutorials

Replies19Views14K

Jun 25, 2025
Marok

 Exporting files from a MPQ.


Deolrin

May 14, 2009

Misc/General Mapping Tutorials

Replies10Views19K

Dec 8, 2009
Newbestyle

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