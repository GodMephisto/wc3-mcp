---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-import-a-skin.6530/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Import a Skin
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-import-a-skin.6530/>
#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

How to Import a Skin

by Archian

Description


In Warcraft it is possible to replace an original skin in the scenarios you make. In this tutorial we are going to replace the Mountain Giant with a green one. When importing a skin it is very important to put the skin in the right destination so the game can find it. Just leaving the skin in the war3imported folder will let your unit stay invisible.

Download Skins


You can download skins from many places on the net. Making a quick Google query might be very successful. For this tutorial I'm going to use the Mecha-Giant skin by timfarman. You can yourself download it here.


You can download more skins from these websites:

[self="http://hiveworkshop.com/resources_new/skins/"]The Hive Workshop[/self]

Warcraft3 Campaigns

wc3.sear.ch

Clan CBS

Samods


Download it, and extract the files into a folder.


Make sure that the skin(s) are in .blp format.

Importing


Open up the Import Manager (F12) and import the skins you need.


Locate the skin you want to replace, in our case we are looking for the Mountain Giant skin. We now have to replace the Mountain Giant skin by ticking Use Custom Path on the imported skin and set it to the same path as the Mountain Giant skin.


The path should not look like this: "war3mapImported\???.blp".


This is incorrect and you need to edit it.


The path for the Mountain Giant skin should look like this Textures\MountainGiant.blp


Usually creators write the paths for the models/skins in red so people know what path they should use.


After you've entered the correct path, click on OK.


After a skin has been imported it will be invisible. To check if World Editor can display the skin you can either close the map (and W.E.) then open it again, or you can


click File -> Preferences -> Video -> Change detail of models and textures. Changing the detail will force Warcraft to reload all the models and textures.

Unknown Skin Path


Two methods exist for finding unknown skin paths.

Use Warcraft III Viewer. Open Warcreft III Viewer and then a model with an unknown skin. Select Current File > Show Used Texture Names.

Use an MPQ extractor. Click Open and locate your Warcraft MPQ files (in the warcraft directory). Search through them all to find the model that should use your skin. Right-Click the model and press Extract.


Then open up the model in notepad and see if you can find something like Textures/Somename.blp, there might be more than one of them but see if you can find a clue there.

 Toolbox


MPQ master

Warcraft III Viewer


This is how our Mountain Giant is gonna look like when we're done.


 Last edited by a moderator: Jul 28, 2007

#### Carlzerx99

C

#### Carlzerx99

How 'bout using the .blp file to another skin???Like when I want to use the model for something else and not only for the m. giant??

#### r.ace613

#### r.ace613

 Level 31

JoinedAug 7, 2015Messages629

Resources: 56

#### Resources
Models 1Icons 53Skins 2

It doesn't work

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Give details if you want help.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

Raider Neoh said:

 It doesn't work

Click to expand...

You probably input the wrong custom path

#### r.ace613

#### r.ace613

 Level 31

JoinedAug 7, 2015Messages629

Resources: 56

#### Resources
Models 1Icons 53Skins 2

Rheiko said:

 You probably input the wrong custom path


Click to expand...


Yeah I putted it in the wrong path and now I know what to do

#### CrazyTeddiez

C

#### CrazyTeddiez

Sorry I couldnt pass the Import Manager bit and where to find it, im trying to put a skin on the huntress its bpl but im just not sure how to

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Try this https://www.hiveworkshop.com/forums/miscellaneous-tutorials-456/importing-models-skins-icons-160555/

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

CrazyTeddiez said:

 Sorry I couldnt pass the Import Manager bit and where to find it, im trying to put a skin on the huntress its bpl but im just not sure how to

Click to expand...


You need The Frozen Throne expansion to use the import manager.

#### Alfee :)

#### Alfee :)

 Level 3

JoinedSep 23, 2019Messages29

hey can someone help me with reforged world editor (i have made it classic only becuse i like it better) i cant make a orc warlock into a human shaman (its a skin so i tought i could find help here)

#### Superfrycook

#### Superfrycook

 Level 47

JoinedApr 12, 2019Messages832

Resources: 291

#### Resources
Packs 1Skins 1Maps 29Reforged HD Models 231Reforged HD Skins 18Reforged HD Icons 8Sounds 3

##
 Republic Anthology


Is this still working on Reforged?

#### RanchBurner

#### RanchBurner

 Level 11

JoinedJul 3, 2010Messages242

Resources: 12

#### Resources
Skins 4Maps 1StarCraft II Resources 4Ported Skins 2Tutorials 1

Yeah, this tutorial needs to be updated. I could never get this right, and now with the Reforged makeover, I'd be eager to try again.

#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

Importing textures (even for skins) has not been changed in Reforged, the only thing you may want to consider looking into when using reforged are all the new toggles available in the asset manager, most prominently the Asset mode that pertains to whether the asset will be used for SD or only for HD.

Changing the imported files' paths no longer opens a new window but instead happens in the already open window (THE asset manager). Other notable things that were not present at the time of making of this ancient tutorial are the folder mode which would allow you to place the imports using a file explorer (what you move all other files on your pc with) and for the part mentioning unknown file paths, you would no longer be able to open mdx files with any of the old software and should use an up-to-date tool like Retera's Model Studio.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I think that the basic concept is roughly the same so I wouldn't say this needs to be updated.

The following could still be considered:

HD/SD

Certain paths are different in HD and SD mode, tilesets come to mind.

You can also specify a texture for one unit only (at least for pre-placed units)

The above points are situational, but probably helpful knowledge.

#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

The first two points, while very valid, fall into general reforged importing, with the second also falling under "unknown skin path" which is itself a part of "unknown asset path", with both too useful not to mention which is what my previous message did.

I am not aware of what the 3rd point is about so if that is indeed possible (specifying a texture for only one preplaced unit), that should probably get explained since it would be very useful.

 You must log in or register to reply here.


### Similar threads

 How to: Find a skin's path and capture screenshots in WE


Afronight_76

Nov 13, 2004

2D Art Tutorials

Replies0Views10K

Nov 13, 2004
Afronight_76

 Skinning on a Mac


Archean

Aug 7, 2007

2D Art Tutorials

Replies16Views8K

Dec 14, 2007
1800-screw you

1

Locked

[Import]
 How to use skins


Manuval

Feb 25, 2013

World Editor Help Zone

Replies15Views4K

Mar 2, 2013
bhusta

 Creating Custom Skins


Archian

Jul 21, 2007

2D Art Tutorials

Replies1Views57K

Jul 21, 2007
Wolverabid

O

 Alpha Skinning


Orc_Tamer

May 5, 2006

2D Art Tutorials

Replies2Views35K

Mar 16, 2015
PurgeandFire

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

2D Art Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top