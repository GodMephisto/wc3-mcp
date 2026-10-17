---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/local-files.330849/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Local Files
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/local-files.330849/>
#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

Local Files

What are Local files?

Local files are a way to override game files without changing the original instances, so it is the only safe way to do it across all versions of Warcraft 3.​

The first question one could ask if "Why would I want to override the game files?", so I will also outline some interesting and useful things you can do using this mechanism.​

How to enable local files?

On Windows, you will have to make a registry key at location HKEY_CURRENT_USER\SOFTWARE\Blizzard Entertainment\Warcraft III, with type DWORD and name Allow Local Files, and set its value to 1.​

An easier way to do this is to open the .reg file attached in a zip archive below this tutorial and accept all popups that get opened because of it.​

​

Alternatively, run this command: reg add "HKCU\Software\Blizzard Entertainment\Warcraft III" /v "Allow Local Files" /t REG_DWORD /d 1​

​

On Mac, the terminal command to enable local files is defaults write "com.blizzard.Warcraft III" "Allow Local Files" -int 1

(Replace Warcraft III with Warcraft III Public Test if you're trying to do this for PTR)​

How to use local files?

To tell the game to override an internal file with the one you want, you will have to place it in the correct path inside the game root.​

​

Up to patch 1.32.6, the game root was the folder Warcraft III was installed into, also called install directory, for most people the game root would've been C:/Program Files/Warcraft III​

​

Since 1.32.7, the game root is a subfolder located inside the install directory, with the name of _retail_ for the retail flavour of the game and _ptr_ for the ptr flavour. As you are more likely to be playing on the retail flavour of the game, the example of a game root for 1.32.7+ is C:/Program Files/Warcraft III/_retail_​

What can you do with local files?

After having wasted 2 minutes on setting local files up and learning where the game root is, you will want to know how you can use this knowledge.​

​

Local files can be used to override any file in the game: models, textures, images, videos, sounds, UI definitions, fonts, object data, and more.​

​

I will give a couple simple examples which will illustrate the power of this very easy to use system:​

​

1) Overriding a model​

​

Let's say I wanted to overwrite the peasant's model with my cool handmade worker model even in melee maps.​

I would go into the game root, create the folder structure units/human/peasant, place my model in there, and rename it to peasant.mdx or peasant.mdl depending on what format I had my model saved in (rule of thumb, don't change the text after the last dot in the filename unless you know what it means).​

​

2) Overriding a texture​

​

Suppose you've just found a very cool skin for a hero you love to play (e.g. Illidan), cool enough that you want them to look like that in any map they appear in.​

You would go into the game root, create the folder textures, place the skin file in there, in this specific case you wouldn't have to rename it as, since it is a skin, it would already have the correct name.​

​

You may be noticing a pattern here, but let's do some more​

​

3) Overriding a sound​

​

Imagine having a friend that is complaining about how one sound in the game sounds too quiet (e.g the creep sleeping sound); you might even agree with them, but even if you didn't you could suggest then a simple fix.​

They would be told to go into the game root, create the folder structure sound/ambient, find or make a louder version of the sound file in there (creepsleeploop1.flac).​

​

For all of these 3 very different cases, there is a lot in common:​

You only need the file path, the file to override with, and the ability to place that file in the correct file path.​

​

4) Overriding a resource indicator icon​

​

This time without any flavour text, you know the drill by now ​

We will be overriding the gold icon this time.​

​

Go into the game root and make the folder hierarchy ui/feedback/resources (this just means you make the folder called "ui", then inside it make the folder "feedback" and inside it make a folder called "resources")​

​

Then I edited the gold resource icon to be a different colour, which can be done in GIMP or any other good image editing tool, you can do anything you want to the image, only saving the final texture in the correct format matters (this is an example DDS configuration in GIMP's export window, note DXT5 and "Generate mipmaps"; you can use DXT1 and DXT3 instead, with DXT1 being preferable if you're not using the alpha channel and DXT3 not being specifically useful)​

The file was saved in that last folder I created in the steps above with the name resourcegold.dds.​

And that's it! Run the game to test if you've done everything correctly

P.S: Although this is not necessarily in the scope of this tutorial, to utilize local files to their fullest capabilities, you will want to know how to get files from the installation of the game on your disk.

Using an MPQ editor on version up to 1.29.2, or CASC viewer for versions 1.30+, you can get both files and their filepaths from the game files. In CASC, the paths will have one or more prefixes, which are denoted by a colon after the prefixes' names, which do not play part in the path in the game root. To provide an example for this, in 1.32+ the peasant's model's path would look like war3.w3mod:units/human/peasant/peasant.mdx which can be compared to the path given in the first override example.

All the files the game uses can be found in the CASC, so feel free to think big.

If you want a small exercise in how to use this mechanism, try overriding the menu's background video (only applicable to 1.32+) or the games' font files.

#### Attachments

AllowLocalFiles.zip

 304 bytes

 · Views: 2,895




 Last edited: Dec 3, 2022

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Didn't they make it so you didn't need to do anything to enable local files with one of the newer patches?

edit: could not find anything backing me up while more than one source mention reg edits so not sure where I have read otherwise.

Anyway:

You might want to bump up the headers one size and then add a smaller header for the sub-header (overriding model etc)

Programs used/showcased should be linked for easy access

I think the folder image is redundant, path in text form should be more than enough to get the point across and takes up less space.

big fan of color usage to highlight words.

Example:

_______________

What can you do with local files?

Some text

Models

Model instrctions​
Textures

Texture instrctions​


 Last edited: Feb 24, 2021

#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

There was an era where you didn't have to modify the registry, but that got fixed back to the intended functionality some time around 1.32.0 (always having local files allowed is not exclusively positive) so what you heard was correct for a long time but also a bug.

I will try to adjust the content as suggested and will add a link to GIMP as that's the only program shown in the tutorial and Ladislav's MPQ editor for an example MPQ editing program.

#### azgarol

#### azgarol

 Level 5

JoinedJun 19, 2018Messages43

I wonder if this can be used for world editor as well. May be it is possible to inject UMSWE trigger data? I've never delved into how it worked for JNGP yet.

#### azgarol

#### azgarol

 Level 5

JoinedJun 19, 2018Messages43

Ok, it totally does work for the Editor, I did added all the trigger data from UMSWE and the map that utilises it loaded in 1.32. However it crashes if you try to edit custom actions, they are not properly displayed etc. That's because I need extra files from UMSWE and not just trigger data, but i haven't figured out the right pathes and files yet.

#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

Local files do also work in the World Editor, which is how the old way to change terrain pathing by modifying terrain.slk works, but interacting with 3rd party hacks like JNGP is not in the scope of this tutorial.

#### keepitmovin

#### keepitmovin

 Level 18

JoinedNov 18, 2013Messages86

Resources: 63

#### Resources
User Interface 63

Hi, I would like to use custom HUD. What foldername do I have to create and what filename and file type should be the HUD image?

By HUD i mean this.

p.s Is it possible to set reforged version of HUD instead of classic version for classic version graphics?

p.s.s. dont suggest quenching mod

#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

keepitmovin said:

 Hi, I would like to use custom HUD. What foldername do I have to create and what filename and file type should be the HUD image?

By HUD i mean this.

p.s Is it possible to set reforged version of HUD instead of classic version for classic version graphics?

p.s.s. dont suggest quenching mod


Click to expand...

For 1.32+ textures can be dds, blp, tga, or tif. For the filenames, check in war3.w3mod:ui\console\human`, per what's in the PS of the tutorial, all the human ui tiles (01-06) are all there. And you can use the Reforged HUD files for classic, sure, but I am not sure if Blizzard realigns some UI elements differently between the two.

This tutorial is about how to use the local files in general, specific ways to use them is probably something you're best off finding out yourself but what I wrote above should point you in the right direction on the example of human ui.

#### Bitterman

#### Bitterman

 Level 13

JoinedFeb 20, 2020Messages272

just a reminder that you need to extract all of the asserts under retail or ptr folders

THAT ALSO includes the sounds folder, because blizzard is too stupid and used buggy flac codec.

So replace all sounds with the 1.0(RoC) and 1.26(TFT) releases so your campaign sounds are uncensored and not buggy.

#### tlstkwjr

#### tlstkwjr

 Level 7

JoinedMay 29, 2013Messages152

thank you!

#### Moozore

#### Moozore

 Level 3

JoinedFeb 8, 2025Messages10

Hi! This seems to be an awesome resource. Thank you so much!

I'm trying to get this implemented in my map but I have 2 open questions:

I'm getting the "Local Files" error, (I'm on the latest patch) on my map, although when testing your map, save/load works perfectly. So, is there something I need to enable in my map options as well?

How do you know which hero you need to save? In my map, players pick heros through a tavern (Only 1 hero, it's a Roguelike/RPG), which then this is stored in a variable "udg_PlayerHero[1-6]", in my case, do I need to do something special? Cause I'm noticing "-save" doesn't do anything, but "-load" just throws me the error above, so I'm just curious.

Appreciate any help given. Thanks again!

#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

Moozore said:

 Hi! This seems to be an awesome resource. Thank you so much!

I'm trying to get this implemented in my map but I have 2 open questions:

I'm getting the "Local Files" error, (I'm on the latest patch) on my map, although when testing your map, save/load works perfectly. So, is there something I need to enable in my map options as well?

How do you know which hero you need to save? In my map, players pick heros through a tavern (Only 1 hero, it's a Roguelike/RPG), which then this is stored in a variable "udg_PlayerHero[1-6]", in my case, do I need to do something special? Cause I'm noticing "-save" doesn't do anything, but "-load" just throws me the error above, so I'm just curious.

Appreciate any help given. Thanks again!

Click to expand...

Hi there! This thread isn't about save-load systems, their "local files" aren't the same thing, the ambiguity stems from the old versions guarding that whole system behind the same registry key. You'll probably get better help if you ask in the thread of the save-load system you're using, good luck!

#### Moozore

#### Moozore

 Level 3

JoinedFeb 8, 2025Messages10

Thanks!

BogdanW3 said:

 Hi there! This thread isn't about save-load systems, their "local files" aren't the same thing, the ambiguity stems from the old versions guarding that whole system behind the same registry key. You'll probably get better help if you ask in the thread of the save-load system you're using, good luck!


Click to expand...

Thanks, that was my intention! But somehow I got confused between tabs

 You must log in or register to reply here.


### Similar threads

 Extracting game files (MPQ and CASC)


BogdanW3

Feb 25, 2021

Tutorial Archive

Replies5Views23K

Apr 15, 2021
pyf

 Local files help


Rvmzii

Aug 28, 2026

Warcraft Discussion

Replies0Views49

Aug 28, 2026
Rvmzii

 Exporting files from a MPQ.


Deolrin

May 14, 2009

Misc/General Mapping Tutorials

Replies10Views19K

Dec 8, 2009
Newbestyle

Locked

 Use Local Files/Allow Local Files


Musketmarine

Nov 9, 2019

Warcraft Discussion

Replies0Views3K

Nov 9, 2019
Musketmarine

Locked

[General]
 Local files vs MPQ


AL0NE

Dec 9, 2011

World Editor Help Zone

Replies11Views4K

Dec 12, 2011
Apheraz Lucent

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