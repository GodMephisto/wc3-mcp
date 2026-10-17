---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/custom-doodad-variations.207150/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Custom Doodad Variations
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/custom-doodad-variations.207150/>
#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

### Introduction
This is a short tutorial that should allow to apply own variation sets to doodads. The here displayed method requires the nowadays widely spread Jass NewGen Pack.

### Exposition
Variation sets in object editor are defined by having the field Art - Variations (dvar) set to >1. Additional coloration fields will spawn but you can only state one model. If you take a look at other doodads with variations, you will recognize that a click on Art - Model (dfil) does not display anything in the preview window. That's because there really is no model placed on this path but when searching for single variations, it attachs number suffixes. Assumed that the specified path is Folder\FileName, then the first variation is on Folder\FileName0, second on Folder\FileName1, third one on Folder\FileName2 and so forth.

Now, what is the problem? If you imported your models to Plant0.mdx, Plant1.mdx and Plant2.mdx, according to above logic, you would enter Plant as model in doodad editor. The editor disapproves of this though! Everytime you want to edit that field, it adds .mdl to the end since the editor likes to have this format. As a consequence, it won't find the variations anymore, it probably searches for Plant.mdl0 or similar, which does not work either.

### Solution
Since the editor does not permit it, the following method will adjust the attribute of the doodad otherwise. JNGP possesses an external tool named ObjectMerger. It has the ability to unite different object editor data, if you want to transfer stuff from one map to another for example without overwriting the existing objects there. Additionally, you can create objects directly via input lines or modify them. The used language is lua. Do not fear. I will show the needed mini script here and explain what to do. The in JNGP integrated vJass language (trigger editor) is able to call external tools like the ObjectMerger automatically. So we go to the trigger editor (F4), create a new page (CTRL+T) and convert it to jass format (>>Edit >Convert to custom text). You may delete the already present content of the lower window. Instead, we write:

 JASS:

//! textmacro SetDoodadModel takes id, path
 //! externalblock extension=lua ObjectMerger $FILENAME$
 //! i setobjecttype("doodads")

 //! i modifyobject("$id$")

 //! i makechange(current, "dfil", "$path$")
 //! endexternalblock
//! endtextmacro

//! runtextmacro SetDoodadModel("D000", "Plant")

What to do now? I should first mention that the script does not need to be run more than once actually. But maybe you want to change it later on or you would like to add more variation sets, so I would leave it. That is why I have wrapped it in a textmacro. The script gets run when the map is being saved and you won't see the changes in editor until you reload the map. Afterwards, you can comment out the runtextmacro line (simply add one more / in front) to avoid lengthened saving times.

As you should see, the textmacro parameters are between quotation marks. At the place where Plant is currently written, the model path needs to be - like I said above without .mdl/.mdx and without variation number. Folders are specified by double backslash (\\).

The first parameter D000 represents the id of the doodad. You can inspect it by changing to raw data display in object editor (CTRL+D), search for your desired doodad in the left list then and select the first four letters (pay attention to case sensitivity!). For custom objects it's something like D000:OOal; the colon is only a delimiter and the four letters after that indicate the original object yours was based on, not important. So only take the first four letters, D000 in that case.

### Addition for destructables
Destructables may also have variations. The script for them is a bit altered though:

 JASS:

//! textmacro SetDestructableModel takes id, path
 //! externalblock extension=lua ObjectMerger $FILENAME$
 //! i setobjecttype("destructables")

 //! i modifyobject("$id$")

 //! i makechange(current, "bfil", "$path$")
 //! endexternalblock
//! endtextmacro

//! runtextmacro SetDestructableModel("B000", "Plant")

### Concluding word
That's all. I divagated a bit to ensure that every novice gets it right since the variation stuff does not have anything more in common with the remains and you do not need more in that direction. Those are basics however one will stumple upon sooner or later.

An example map is attached. Open the map in JNGP, activate the runtextmacro line inside the single page in trigger editor (remove one of the slashes), save, reload the map and comment the line out again in this order.

#### Attachments

DoodadVariationExample.w3x

 46 KB

 · Views: 493



#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Tested. Works perfectly.

Great job, very useful tutorial. ~Approved.

EDIT: I'm moving this to General Mapping Tutorials, let me know if you think it belongs elsewhere.

#### Mechanical Man

#### Mechanical Man

 Level 35

JoinedJan 18, 2005Messages5,781

Resources: 42

#### Resources
Models 32Icons 3Packs 1Maps 5Tutorials 1

It's confusing as most programming languages including JASS use // a comments, but here it seems to actually activate the commands

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

I think the reason for the //! is so that if JNGP is not present, the compiler will simply ignore it (since it thinks it's a comment).

#### Doomcore

#### Doomcore

 Level 6

JoinedSep 7, 2017Messages207

I did this but it corrupted my map and all it gives is just a crash. How could it be?


 Last edited: Oct 14, 2017

#### SA Dashie

#### SA Dashie

 Level 22

JoinedApr 3, 2010Messages1,889

Resources: 17

#### Resources
Maps 11Spells and Systems 3Templates 1User Interface 2

Thank you for the tutorial. I love these little things that editing more enjoyable.

#### DarkIllidan

#### DarkIllidan

 Level 12

JoinedApr 27, 2009Messages237

Resources: 4

#### Resources
Maps 2Reforged HD Models 2

Gtron said:

 While this is appreciated it is not necessary. By naming each doodad variation .mdl0.mdx, .mdl1.mdx, .mdl2.mdx, and so on we can trick the editor into using doodad variations. To avoid confusion it is recommended to use a descriptive subfolder name for each Doodad.

For example a working path in the Art - Model File field would look like the following. However if you type this in the trick will not work.

When typing the path in use the following format. The editor will automatically add .mdl to the path but it will understand to use the models you previously renamed to .mdl0.mdx, .mdl1.mdx, .mdl2.mdx, and so on.


Click to expand...

Could not find an answer for variation problem for some time. Thanks man!


 Last edited: Jan 12, 2024

#### Footman16

#### Footman16

Community Manager

 Level 58

JoinedJul 14, 2012Messages4,279

Resources: 37

#### Resources
Models 6Packs 1Skins 11Maps 12Reforged HD Models 3Reforged HD Skins 2Reforged HD Icons 2

Gtron said:

 While this is appreciated it is not necessary. By naming each doodad variation .mdl0.mdx, .mdl1.mdx, .mdl2.mdx, and so on we can trick the editor into using doodad variations. To avoid confusion it is recommended to use a descriptive subfolder name for each Doodad.

For example a working path in the Art - Model File field would look like the following. However if you type this in the trick will not work.

When typing the path in use the following format. The editor will automatically add .mdl to the path but it will understand to use the models you previously renamed to .mdl0.mdx, .mdl1.mdx, .mdl2.mdx, and so on.


Click to expand...

So if it was a group of models called "tree1" "tree2" etc in a folder called "trees", you would import those models and then change their path to:

War3mapimported\trees\tree1

War3mapimported\trees\tree2

War3mapimported\trees\tree3

etc without the .mdx?

#### DarkIllidan

#### DarkIllidan

 Level 12

JoinedApr 27, 2009Messages237

Resources: 4

#### Resources
Maps 2Reforged HD Models 2

Hey Footman16 the above you wrote did not work for me. But this below did.

war3mapImported\FolderwithDoodadVariationModels\.mdl0.mdx (Yes . is first in the name of the file in import manager, weird right, but it works)

war3mapImported\FolderwithDoodadVariationModels\.mdl1.mdx

war3mapImported\FolderwithDoodadVariationModels\.mdl2.mdx

Then put the path to the model exactly like this war3mapImported\FolderWithDoodadVariationModels\

Then hit enter then Object Editor will do it's magic and give you variations.


 Last edited: Jan 12, 2024

#### DarkIllidan

#### DarkIllidan

 Level 12

JoinedApr 27, 2009Messages237

Resources: 4

#### Resources
Maps 2Reforged HD Models 2

Gtron said:

 Thank you for providing the illustrative screenshots to better communicate this process to people attempting it. To provide more clarification the folder names containing the models can be named anything. It's just important to have different folders for each Doodad you want variations of due to the inevitable duplicate file names using this trick.

No the files are literally named .mdl0.mdx, .mdl1.mdx, .mdl2.mdx hence the need for descriptive folders for each Doodad you're variating. I discovered this trick when seeing if I didn't enter anything in the folder path for a model the Warcraft 3 Editor automatically added .mdl to the path. Using the knowledge of existing destructible variation file path behavior I could logically conclude it's going to add 0, 1, 2, etc. to the path and I named the files accordingly.


Click to expand...

People should know this. I barely found the answer. Someone should write a small tutorial, use the pictures above because people need this info. Write something simple so people find it on google and name it 'Using custom Doodad variations tutorial' or something like that.

Thank you again for it. Very useful, this is the only place I found this answer.

#### Verdun

#### Verdun

 Level 16

JoinedJun 9, 2008Messages350

Resources: 11

#### Resources
Models 11

##
 Shortlegs


DarkIllidan said:

 Hey Footman16 the above you wrote did not work for me. But this below did.

war3mapImported\FolderwithDoodadVariationModels\.mdl0.mdx (Yes . is first in the name of the file in import manager, weird right, but it works)

war3mapImported\FolderwithDoodadVariationModels\.mdl1.mdx

war3mapImported\FolderwithDoodadVariationModels\.mdl2.mdx

Then put the path to the model exactly like this war3mapImported\FolderWithDoodadVariationModels\

Then hit enter then Object Editor will do it's magic and give you variations.

View attachment 458742View attachment 458743

Click to expand...

It's not working for me. I even changed the files to exactly those paths. Does it only work in Reforged?

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Verdun said:

 It's not working for me. I even changed the files to exactly those paths. Does it only work in Reforged?


Click to expand...

I think so. I've ran a test on an older patch and the trick didn't work. However, in Reforged it works. Strange.

 You must log in or register to reply here.


### Similar threads

Locked

 Problem (more or less) with variations


Axarion

Aug 3, 2011

World Editor Help Zone

Replies8Views8K

Aug 4, 2011
WaterKnight

Locked

 Doodad variation


Brombie

Nov 8, 2010

World Editor Help Zone

Replies3Views2K

Nov 9, 2010
Brombie

Locked

[Import]
 Setting up Doodad Variation


Jupiterstorm

Jun 20, 2015

World Editor Help Zone

Replies6Views3K

Jun 21, 2015
WaterKnight

Locked

 Doodad random variations


Serpent

Jan 21, 2009

World Editor Help Zone

Replies12Views3K

Jan 29, 2009
Serpent

Locked

 How do I create custom doodads with variations?


Marsmallos

Jun 22, 2009

World Editor Help Zone

Replies3Views5K

Jun 23, 2009
Lmao.Me

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