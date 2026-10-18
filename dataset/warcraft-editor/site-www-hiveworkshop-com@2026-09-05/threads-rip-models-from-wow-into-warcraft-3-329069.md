---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/rip-models-from-wow-into-warcraft-3.329069/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Rip Models from Wow Into Warcraft 3
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/rip-models-from-wow-into-warcraft-3.329069/>
#### Maldiran

#### Maldiran

 Level 12

JoinedJul 23, 2019Messages65

Resources: 17

#### Resources
Tools 1Ported Models 15Tutorials 1

1.Introduction

 Hey guys!

Since the release of the WC3 Reforged many people were asking, if it is possible to rip for example character models and make them work in Warcraft 3. The answer is: yes! Some time ago, there was forum named Warcraft Underground, where such models were posted, but now it moved to Discord and access to the "base character models" is denied. I was trying to figure out how to remake this models or export character models without the "base models" and I realized how simple it is to do it. It took me over a month, but the results are impressing at least for me.​

2.Tools

To achive our goal we need a few programs:​

World of Warcraft - Ideally 8.3.7

Character models exported from Warlords of Draenor

Blender v.2.7x - 2.79b

M2Mod v.8.3.0 with Blender plugins v.8.2.0 - Latest

WoW Model Viewer v0.10.0.755 - The Shadowlands version.

jM2converter by Koward and Java

mdlVis

Some programs to edit model after ripping it (like Retera's model studio, Magos War3 ME and BLP Lab)

All this programs are added into one file. Here you have all needed editors apart from raw WoW of course and WoD character models.

3.Why am I using WoD character models?

 WoD character models are in better file system than later ones and they haven't changed much until Shadowlands. Converting for example Shadowlands character models would be painful and it is not worth it. Since 7.3.5 WoW character models uses .skel files, that are unreadable by jM2converter. It causes errors, that I am unable to fix.

​

4.Exporting character models

 You should check if your mappings are loaded. Open M2Mod and go to the CASC window. Click Load mappings. If you don't have listfile.csv just download it from wow.tools (go to files, select build, download listfile.csv (Blizzard Filenames Only)).

 Place WowTools from M2Mod in C:\Users\Your name\AppData\Roaming\Blender Foundation\Blender\2.79\scripts\addons

 Now check if your Blender has enabled addons, go to File -> User Preferences -> Add-ons and search for WowTools and enable it if it is disabled. Then Save User Settings and close this window.

 Open M2Mod and in Export window choose source .m2 to for example orcfemale_hd.m2 and as a target .m2i file that does not exist now. Click "Go!" and wait.

 In Blender click File -> Import File -> Import m2i and import created file.

Open WoW Model Viewer and select your character model. Gear it up.

Now you have to choose your hairstyles and beards in Blender using WoW Tools addon that came up with M2Mod. Than you have to make the same customization in WMV. Do not close WMV!

​
5.Editing WoW models in Blender 2.79b

The first thing that you have to know! The Warcraft 3 models are capped and they can not have more than around 21800 triangles. It is the reason, why you can not convert character models directly. Keep that in mind when editing WoW models.

With import-export tools for Blender comes also a special editor.

 Documentation of meshes in WoW character models. Meshes that start with:

0001 - base body parts. You have to include them.

00xx - hairstyles and additional meshes like hairbands.

from 01xx to 03xx - beards, earrings, horns, and other things modifying faces.

04xx - wrists

05xx - boots

07xx - ears

08xx - additional meshes around chest-hands

09xx - additional meshes around legs-boots

from 10xx to 13xx - legs, robe, tabard, “skirts”

15xx - capes (in blood elf female 1501 is a part of a body, there might be other models like this)

17xx - death knight’s eye glow

18xx - belts (sometimes one is as a part of a body)

20xx - feet and shoes.
- there are others, but they are used rarely and if you see them, check them.

You should just leave correct meshes an delete all the others. For example if your model in WMV uses 0403 you don’t need to include 0401. If your model uses helm, you probably should delete also all hairstyles.

When your edit is done check the number of triangles. It has to be less than 21800.

 After edition click export to .m2i and save it in the same folder as previous m2i file.

 In M2Mod in Export window as a source m2 place the basic file, in source .m2i place edited file and click Preload and later Go. Program will proceed and an error will pop up (look at point 4).

 The program will create an Export directory in your folder where the .m2 was. I advice you to immediately rename it to your model’s name.

 To “fix” that error you have to simply copy all .anim files from directory where .m2 was to the folder created by the program. You should copy only .anim files corresponding to your model (for example only hd files).

​
6.Exporting gear and textures.

 Return to WMV. If you want to export not-character model, for example druid bear form, proceed as it was some gear. Only do not forget about .anim files if there are any. Treat them as a .skin files.​

 Go to File -> Export Model -> OBJ... and save it as .obj

 Open BLP Lab Tools -> Batch converter and convert all the (.png) files you have exported to BLP1 use default settings, do not change any alpha etc. You can delete all the exported files, leave only the .blp.

 Now in WMV go to View -> Show model control. Open CASC explorer and load your WoW. Go to item\objectcomponents (if you are exporting gear, if creature, go to creature). In WMV models see what model is your character using. Do not export character models! For example, your belt is a separate model so you copy its name to CASC explorer and go to waist. You should see some files .m2 .skin and maybe .blp. Export only .m2 and .skin. They have to be in the same folder. You should think about .m2 and corresponding .skin files as a one file. You are unable to convert .m2 files without .skin files. Do not export character models!

 If your model is using some special sub-models and one of textures is named collection, it means, that you have to copy a part of its name into CASC explorer in collections folder and search after it. The file that you want is the file named like your texture and with special thing that shows the target character model. For example if you are ripping orc female model, the collection file will be name_or_f.m2. Export it with the corresponding .skin files.

7.Retro-porting

 Open every exported .m2 file with notepad (I prefer Notepad++) and delete first 8 bytes of text. What’s the meaning of that? When you open your model in notepad it starts like this: MD21xxxxMD20xxxxrestoftext. So you have to delete the first eight letters so it will be MD20xxxxrestoftext. Then you can save the model. By this edit, you have told your file, that it is now in the WoD format, not Legion or later. It might seem weird to you, that so simple edit changes so much, but the truth is, that your converter can convert every WoW model, but it is just to old and does not recognize it as a proper file. Do it with both characters, equipment, and creatures.

 Install Java if you haven't done it yet.

 Open Command Prompt (type cmd inside start menu)

 Type: java -jar (drag here your jM2converter) -in (drag here your .m2 model) -out (drag here your .m2 model if you want to replace it or just type the path for your converted model) -bc Let's explain this command. You are using jM2 converter, linking source file and output file and then the version you are converting to. You need to convert it to early bc (-bc). This part might cause some problems for you. If the converter pops up an error it is sadly probably your fault. I haven't encountered any bugs with that converter and I am using it now for about half of a year. You will have error if you haven't deleted an MD21xxxx part, tried to import .skel files, your model has more than 21800 triangles or your java is not installed properly (solution). This are the most common. If you have some problems with this part feel free to write in the comments about it.

 Click enter and wait. With bigger models, it can take even more than a minute. Do not close the command prompt until your file isn't written. The "Conversion completed" message does not mean, that conversion is completed (because the model is not saved yet).

As you can realize the new model is larger than the previous one. That's caused by importing all .skin .anim.

Now your conversion is completed! You can now open your .m2 model with mdlVis, then select all, go to Optimization -> Canonization and save it immediately as .mdx. Don't edit any sequences, because they will be bugged. Now close mdlVis and reopen your model. It is now ready for normal editing.​

8.Texturing model

 To texture your model you open Magos editor and open your model. Now go to Windows -> Texture Manager. Import the proper texture and, if it is character's hair, you probably need to right click -> Edit and now click Wrap Width and Wrap Height. Go to the Windows -> Material Manager and edit every material by selecting Unshaded and changing textures for the proper one. Than delete unused texture in Texture Manager. Save your model.

You have to change UV mapping of eye texture. Just make new material in Magos and give it to the eye geoset. Than change UV mapping.​

Sometimes the model does not open in Magos because it uses weird particle emitters. You can delete them in mdlVis and after it the model should open. You can of course try to fix them (you usually succeed).

9.Deleting broken geosets

 It only happens to character models. You should (but it is not necessarily) optimize it in mdlVis to delete the doubled ones (it sadly reduces quality) and than manually delete the broken ones. You should have around 10-20 geosets after deleting. Keep in mind, that special tool in Retera's Model Studio will probably break the animations.​

10.Animations

 Check animations in Retera's Model Studio, because in Magos they are sometimes bugged. Animation names you have to figure by yourself. For example the somethings "First" means it is for one handed weapon, "Second" is for two handed, "Third" is for polearms, "Fourth" for bows and "Fifth" for guns and crossbows. With fourth and fifth animations you have to merge Stand Ready and Attack. The new Stand Ready should be Stand Lumber or Stand Gold.​

11.End

 I hope you've enjoyed this tutorial. If you have any questions write them in the comments. If you want a special WoW model, but you still don't know how to make it, you can write to me and I will try to rip it.

​

P.S. .wmo models are easily converted by wow.export from wow.tools to special .obj because they have no anims. Check this tutorial to get an idea and basic knowledge. Just import it to Retera's Model Studio, not to Blender.


 Last edited: Jan 4, 2021

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

For starters the images need to be smaller or put in hidden tags. I also highly suggest indenting text.

Title

Body​

#### Maldiran

#### Maldiran

 Level 12

JoinedJul 23, 2019Messages65

Resources: 17

#### Resources
Tools 1Ported Models 15Tutorials 1

Chaosy said:

 For starters the images need to be smaller or put in hidden tags. I also highly suggest indenting text.

Title

Body​

Click to expand...

I hope now it's ok.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Seems clean layout wise. I'll read through it soon TM.

You'd get bonus points if you can sprinkle in some colors, but not required.

edit: [How-to] Loop In GUI! very good example of color usage.


 Last edited: Dec 8, 2020

#### WanSaweR

#### WanSaweR

 Level 5

JoinedFeb 1, 2020Messages62

Thank you so much mate. Finally we have some good tutorial. After the disappearance of Warcraft Underground I thought, we wouldn't have good tutorial for Ripping models. This tutorial is good for me, because I am new in this. Thanks again.

#### Maldiran

#### Maldiran

 Level 12

JoinedJul 23, 2019Messages65

Resources: 17

#### Resources
Tools 1Ported Models 15Tutorials 1

If you or anyone who read this tutorial ripped a model, please, post them on Hive. It will help many people in making their maps. Ripping this models is very time consuming. I will post my models around Christmas in a one big pack.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Trimmed down the tool section, fixed various grammar issues, renamed tutorial to be shorter.

Here is my issue though. I did not think of this before, this tutorial indirectly promotes piracy as you cannot get your hands on a BFA version of WoW from blizzard directly.

Then again.. we do have links and direct downloads to old wc3 patches from this site that also are no longer available from blizzard.

Soooo.. I'll poke some higher ups and confirm that it's alright.

#### Maldiran

#### Maldiran

 Level 12

JoinedJul 23, 2019Messages65

Resources: 17

#### Resources
Tools 1Ported Models 15Tutorials 1

We don’t need to use BfA, it is just simpler. The main issue with Shadowlandas is basicly the WMV. It is still in alpha and it works just worser than BfA one. I have ripped one character model while using Shadowlands and there weren’t any problems. You have to choose hairstyles and beards in blender when the .m2i is imported. Then you choose the same ones in WMV. After exporting it as .obj delete the eye texture and it is done.

If BfA game version is a problem, I can update this tutorial, but it will be a little more complicated.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I think that would be for the best, if you may.

It will be better in the long run as SL's approach will be just as stable eventually. But with more models to choose from and easier access, a fair tradeoff for a few extra rip steps, I think.

#### Maldiran

#### Maldiran

 Level 12

JoinedJul 23, 2019Messages65

Resources: 17

#### Resources
Tools 1Ported Models 15Tutorials 1

I have changed it for Shadowlands. The only new dots are 4.5 & 4.6 and point 8, but I have changed their order a little.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Point 3 can be removed then?

#### Maldiran

#### Maldiran

 Level 12

JoinedJul 23, 2019Messages65

Resources: 17

#### Resources
Tools 1Ported Models 15Tutorials 1

It can't. WoD character models have to be used, otherwise this whole tutorial is pointless. You can't rip characters after 7.3.2/7.3.5 because they uses .skel files. I have removed the part with BfA.

WoD character models are not a piracy I think, because they are only 250MB all and are 0.5% of the game size. They were exported by me and I have deleted a huge part of them (textures, unused sd files etc.). This files are the "base models".

You no longer need the old version of the game to rip models.

#### Chirox

#### Chirox

 Level 1

JoinedSep 19, 2019Messages3

Is this method sitll work ?

#### Maldiran

#### Maldiran

 Level 12

JoinedJul 23, 2019Messages65

Resources: 17

#### Resources
Tools 1Ported Models 15Tutorials 1

It is working. The only downside is, that there might be some animations that are not 100% correct (when exporting large models like the character ones). If you want to make character, the best way is to use Vulfar’s character models (for now we have sadly only Human male and Orc male). Of course, this method will work, just pay attention to animations.

#### Chirox

#### Chirox

 Level 1

JoinedSep 19, 2019Messages3

Maldiran said:

 It is working. The only downside is, that there might be some animations that are not 100% correct (when exporting large models like the character ones). If you want to make character, the best way is to use Vulfar’s character models (for now we have sadly only Human male and Orc male). Of course, this method will work, just pay attention to animations.


Click to expand...

Alright, thanks for taking time to respond to me

#### CediFly

#### CediFly

 Level 3

JoinedMay 23, 2022Messages17

After export m2mod Geoset/Mesh in blender ,How to import .anim to model in blender?

#### Maldiran

#### Maldiran

 Level 12

JoinedJul 23, 2019Messages65

Resources: 17

#### Resources
Tools 1Ported Models 15Tutorials 1

You don't import .anim into blender, they are merged with the model later.

If you really want to import WoW animations into blender than just export model from WMV as obj with anims and import that to blender.

#### CediFly

#### CediFly

 Level 3

JoinedMay 23, 2022Messages17

Thank


 Last edited: Aug 17, 2022

#### 3139245141

#### 3139245141

 Level 5

JoinedDec 17, 2021Messages15

Resources: 1

#### Resources
Ported Models 1

I was told that the system could not find the specified file, but I do not know what the problem is

#### LeCrepe

#### LeCrepe

 Level 3

JoinedAug 19, 2016Messages27

So, if I’m trying to port the models of specific npcs, would this method still work?

Also, would it be more or less difficult then ripping a base character and trying to dress them up to look like an npc?

Also, does it have to be blender 2.79b, or will a newer version suffice?

The link for WoW model viewer also doesn't work.

Looks like a few of the links here don't work anymore, this guide needs to be updated.

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


@Maldiran, can you fix this?

#### CediFly

#### CediFly

 Level 3

JoinedMay 23, 2022Messages17

@Ralle This one is outdate ,Who know how to exporting last version character base model is Vulfar but he say he not have time to share his technic ,He say It need manual fix after exporting in HIVE have lot of 3D modeler that know how to fix

THANK VULFAR for Information anyway

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


Can you ask around if anyone wants to fix this tutorial? Otherwise it will be archived.

#### CediFly

#### CediFly

 Level 3

JoinedMay 23, 2022Messages17

This tutorial is working but only version below warlord of draenor and warlord of draenor version but new version character base model in battle of azeroth ,shadowland ,dragonflight is not working with this tutorial ,

This guy you can talk to him @Ralle

###
 Vulfar


 www.hiveworkshop.com


I try to offer 100euro to him to teach me how to rip but only answer is I don't have time for that

he is the one who upload last version human and orc character base to the site

If I talk to him I will get same answer for sure I got same answer for 5month haha

#### Footman7

#### Footman7

 Level 9

JoinedDec 11, 2023Messages21

Resources: 6

#### Resources
Ported Models 6

Who knows how to fix this error?

#### deepstrasz

#### deepstrasz

Map Reviewer

 Level 84

JoinedJun 4, 2009Messages22,555

Resources: 7

#### Resources
Maps 4Spells and Systems 2Sounds 1

I think this one will essentially take every such time consuming and tedious WoW model porting tutorial's place: WoW-Converter: export WoW NPCs to Warcraft 3

 You must log in or register to reply here.


### Similar threads

 How to: Import models into World Editor


paiinkiller

May 16, 2009

Tutorial Archive

Replies7Views3K

May 18, 2009
Pyritie

[2D Art]
 How to use skin with same type of model


rysnt11

Dec 16, 2010

Tutorial Archive

Replies1Views7K

Dec 23, 2010
PurgeandFire

 Model Paths


nathanman

Nov 14, 2007

Tutorial Archive

Replies3Views5K

Nov 17, 2007
PurplePoot

Locked

 Problems with Ripping WoW Models


Zunariel

Mar 15, 2016

Modeling & Animation

Replies4Views2K

Mar 16, 2016
A Void

Locked

 Porting Heroes of the Storm (and Starcraft 2) Models to Warcraft 3


Razorclaw_X

Jul 22, 2021

Modeling & Animation

Replies4Views8K

Jun 17, 2023
Resonance Cascade

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