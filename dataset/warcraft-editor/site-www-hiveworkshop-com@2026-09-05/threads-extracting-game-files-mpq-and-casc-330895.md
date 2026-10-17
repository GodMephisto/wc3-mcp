---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/extracting-game-files-mpq-and-casc.330895/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Extracting Game Files MPQ and Casc
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/extracting-game-files-mpq-and-casc.330895/>
#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

How to extract game files

Introduction

Extracting game files is not something everyone will need, but those who would can need them for many things: you may need to get some data from the game SLKs, get a function's definition from the blizzard's code files, get a file to modify and use in your map or with local files.

This tutorial will have instructions for both MPQ (Warcraft 3 patches before 1.30 and from maps) and CASC (Warcraft 3 patches 1.30+), and will outline which MPQs and CASC roots contain which files.​

What are MPQs?

MPQ is an archive format made by Blizzard, similar in some aspects to a ZIP file. They are used both to store map files (w3m and w3x files are MPQs, sometimes with an extra header), custom campaign files (w3n files are also MPQs), and game files until 1.29.2. More technical information can be found here.

​
MPQ files' contents

There were 4 main game MPQs: War3.mpq, War3Local.mpq, War3x.mpq, War3xLocal.mpq; note that these are not the only files the game would look in when loading a file.

War3.mpq contained all the game files that were not TFT-only and did not change based on the game's locallization (translation, for example sounds and text).

War3Local.mpq contained the localized files that were not TFT-only.

War3x.mpq contained all the game files that were TFT-only but did not change based on the game's locallization.

War3xLocal.mpq contained the localized files that were TFT-only.

There was also War3Patch.mpq that used to stored all files that were updated with patches, but it has been removed after 1.28.

Inside of the main MPQs, there were also MPQs for every tileset (called like A.mpq, with A being the tileset character specified in the w3i) and for low and medium graphics modes (war3_med.mpq and war3_low.mpq).

When loading a file from a system with MPQs, the data sources were considered as such (only specifying the main ones in this tutorial):

Local filesif local files are enabled

Map archive

Tileset MPQ

War3xLocalif the map was loaded in TFT mode

War3xif the map was loaded in TFT mode

War3Local

War3

How to extract from MPQs

There are many MPQ editors, but I personally recommend the one by Ladislav Zezula. It has the option to open one or multiple MPQs at once, automatically recognizes extensions of files not covered with a listfile (most files in Warcraft 3 patch MPQs), and can even make an MPQ from scratch or from a folder.

It will also give you the option to set it as the default handler for MPQs, so opening the game files becomes as simple as a double-click on the MPQ.​

What is CASC?

CASC is a system of storing game files, serving as a replacement for game MPQs. More in-depth information can be found here.​

CASC contents

CASC is a simpler system, being unified as opposed to the many MPQs of the old one.

​
war3.w3mod: the main root

_balance/*.w3mod: balance-related files, for any particular map the usage of these is dictated by "Game Data Set" and "Game Data Version"

_deprecated.w3mod: files that should no longer be used by anything but kept for backwards compatibility

_hd.w3mod: the root for HD files

_locales/*.w3mod: language-dependent file overrides for HD

_teen.w3mod: teen asset mode files for HD

_tilesets/*.w3mod: tileset-dependent files for HD​
_locales/*.w3mod: language-dependent files

_teen.w3mod: teen asset mode files

_tilesets/*.w3mod: tileset-dependent files

Maps can also have most of the subroots that war3.w3mod can have.

The hierarchy that will be outlined below is adjusted for latest version, but will be similar to the hierarchies of the older patches (not including locale and balance roots nor the teen [low violence mode] root, and the * in tileset names only means the active tileset, not every one of them):

Local filesif local files are enabled

Map archive:_hd.w3mod:_tilesets/*.w3modif HD mode is enabled

Map archive:_hd.w3modif HD mode is enabled

Map archive:_tilesets/*.w3mod

Map archive

war3.w3mod:_hd.w3mod:_tilesets/*.w3modif HD mode is enabled

war3.w3mod:_hd.w3modif HD mode is enabled

war3.w3mod:_tilesets/*.w3mod

war3.w3mod

​

How to extract from CASC

​
The only tool that can extract files from CASC is Ladislav Zezula's CascView. You will have to select either a game folder or game from a list. Getting the files from CASC is very similar to getting the files from MPQs from this point on.

​
Closing words

In both MPQ can CASC, files have similar paths accounting for roots:

abilitiesability and weapon models

buildingsbuilding models

campaigncampaign maps

doodadsdoodads models and data

environmentmiscellaneous models and foliage data

fontsfonts

mapsall melee and custom maps included with the game, they were

in the main Warcraft III folder(not in an mpq) in the MPQ system

moviesall pre-rendered cutscenes

objectsmore miscellaneous models

pathtexturesobject pathing maps

replaceabletexturestoo many things to list in a table

scriptscode files: ai, jass and preload

shaders3d graphics' shaders

sharedmodelsmore miscellaneous models

soundaudio files

splatssplat data

terrainartterrain models and data

texturesmost of the game textures

uiui data, models, and textrues

unitsunit models, and data for most objects

webuiwebui (pre-game UI for 1.32+) files


 Last edited: Feb 25, 2021

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Sorry about being late to check this one out.

Wouldn't you say this is too simple?

I have not tried this with Warcraft 3, but I have extracted wow models from casc with the suggested program. I think the process is identical.

The tutorial is more or less:

1. Download the program

2. open the mpq/casc

3. find your file and right click>extract

Or, realistically:

1. use the program

It's pretty obvious how to use the program if you are remotely tech-savvy.

You offer potentially interesting and useful information outside of the main steps but I don't think it belongs in a tutorial.

#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

I would agree it's very simple, but people too often don't want to look into any changes to the files, trying to use the MPQ editor on 1.30 and having no idea what to open with it; I have also added the folder structure to make the tutorial more comprehensive just so people who aren't too scared to try the tools themselves still have something to gain from reading the one post without having to search for more. The file loading hierarchies are also probably niche enough not to need a tutorial since people who need them can probably research them themselves or ask in the discord (link here in case some random person reads this and doesn't know we have it).

The root cause for me even writing this tutorial is the amount of people needing help in various Warcraft 3 discords with the most basic extraction, and I believe I still kept it less oversimplified than some of the older approved tutorials for it.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Bogdan3 said:

 I would agree it's very simple, but people too often don't want to look into any changes to the files, trying to use the MPQ editor on 1.30 and having no idea what to open with it


Click to expand...

I disagree.

If I have a tech problem in general, what is the first thing I do?

Google.

The thread: Where the heck is the .mpq?

This gives you enough info.

The same would probably happen if you used the site search functionality.

I agree there probably should exist a section with "useful tips" or something like that for things like this where the information is useful but is not worth taking up a tutorial slot (it's annoying to go through many pages if it starts to get bloated)

Bogdan3 said:

 I have also added the folder structure to make the tutorial more comprehensive just so people who aren't too scared to try the tools themselves still have something to gain from reading the one post without having to search for more. The file loading hierarchies are also probably niche enough not to need a tutorial since people who need them can probably research them themselves or ask in the discord


Click to expand...

I think you're having the opposite effect.

If I was intending to learn something and I go to the tutorial and see an a4 page worth of fairly compact information I would probably be discouraged and think it's something advanced.

I cannot prove this one way or the other so I'll leave that be.

But as I said in the first post, the extra stuff you added might be useful.

But not to the extent where I think it compensates for what I think is an unnecessary tutorial.

If a returning modder becomes confused, he can google/hive search it or ask in discord he gets the reply "You need another program in later patches" he downloads the program and he is basically good to go.

I think that a tutorial should give answers that are too complex or lengthy to be reasonably answered on a common basis by those willing to help.

If you disagree with this you are welcome to poke an admin or someone else on the staff team for a second opinion as the rule "too simple" is ultimately subjective.

#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

The only problem with that whole reply I see is that people wouldn't know "casc" is the word to search for, other than that I fully agree - people who won't google these kinds of issues have infinitesimal chances of finding this kind of a tutorial so it's useless to that group, and those that would google stuff can just do that instead.

All the extra data I put in is probably just feature-creep and you're correct that it makes the tutorial seem so much larger and more complicated than it is.

Also no need to poke anyone since I trust your opinions more on this, especially with me not having a good idea of the vision for the tutorial section. I guess one of the causes of this tutorial was also the amount of outdated and/or duplicate tutorials about extracting from MPQ and similar but making just one more pointless tutorial will by itself not change that.

As a last paragraph, sorry if any of these replies seem in any way attacking. English is not my native language so I might not conceive the context of some phrases or words. Thank you for your time

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

Chaosy said:

 If I have a tech problem in general, what is the first thing I do?

Google. [...]


Click to expand...

When people have a problem nowadays, many of them imho prefer to post "I can't do [whatever stuff they want to do] plz halp me!" in a forum, and then they wait for an answer.

 You must log in or register to reply here.


### Similar threads

 WarCraft Files (w3m, w3x, mpq..)


Ralle

Jan 3, 2006

Tutorial Archive

Replies1Views27K

Mar 26, 2007
Wolverabid

 Parsing metadata from w3m w3x w3n


Firstrun70

Feb 2, 2020

Tutorial Archive

Replies4Views8K

Aug 5, 2022
Firstrun70

 Local files


BogdanW3

Feb 23, 2021

Misc/General Mapping Tutorials

Replies12Views30K

Dec 7, 2025
Moozore

 Overview and Setup


Retera

Apr 5, 2021

Warsmash Mod Engine

Replies0Views7K

Apr 5, 2021
Retera

Locked

 Reforged Textures Problem


xorkatoss

Jan 1, 2021

Texturing & Surfacing

Replies1Views3K

Apr 4, 2021
Ardenaso

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