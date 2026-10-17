---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-adding-sprite.321423/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Adding Sprite
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-adding-sprite.321423/>
#### Rigborn

#### Rigborn

 Level 8

JoinedApr 17, 2017Messages316

Resources: 1

#### Resources
Tutorials 1

This tutorial will cover the basics of adding a model to your UI and fix model related issues like black and white models.

Tools you need:

Retera's Model Studio:Retera's Model Studio

OinkerWinkle's Geoset Translator(optional):Oinkerwinkle's MDL Toolkit - Wc3C.net

Notepad++:Downloads | Notepad++
Let's start with creating a simple 3d model:

 JASS:

library uitest initializer init

 function init takes nothing returns nothing
 local framehandle gameUI = BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0) //This is our default Origin Frame, we will use it to create our model frame on top of it.
 local framehandle model = BlzCreateFrameByType("SPRITE", "model", gameUI, "", 0) //This is how you define a frame as a sprite. If you don't have TOC files loaded, you have to use this to create SPRITE.
 call BlzFrameSetAbsPoint(model, FRAMEPOINT_CENTER, 0.4, 0.3) //Positioning our SPRITE frame
 call BlzFrameSetSize(model, 0.001, 0.001) //For in-game models, size is mostly dependant on the size of the model, since in-game models are too big for our frames, we need to reduce its size to fit it into screen otherwise you will get wierd screen blackouts.
 call BlzFrameSetScale(model, 0.001) //We need to reduce our model size.
 call BlzFrameSetModel(model, "units\\human\\HeroPaladin\\HeroPaladin.mdx", 0) //This is pretty self explanatory. 0 here is the animation index that you want to use.For most models, 0 is for stand, 1 is for death animations.
 endfunction
endlibrary

This how our SPRITE looks like in-game:

This issue happens because of two things:

It happens due to shading

It happens because your models uses things like HeroGlow and TeamColor in Textures Layer.
Luckily we can fix that issue easily. Open your model using Retera's Model Studio then save it as mdl.

Then open your mdl file on Notepad++. Then edit these:

To this:

You can change Teamcolor00 and TeamGlow00 to 01(up to 23 I guess). These will change your model's color and glow. Now we need to change the shading options. To do that you need to find Materials Layer and change these:

Note= without setting the layer as unshaded, our model will have black and white parts attached to that layer. So we change it like this:

Now import the file to your map and change









 JASS:

 call BlzFrameSetModel(model, "units\\human\\HeroPaladin\\HeroPaladin.mdx", 0)

 to your imported model path.

Now our model should look like this:

For those that want the model to be rotated. You can use Oinkerwinkle's Geoset Translation Tool to rotate the model.


 Last edited: Jan 13, 2020

#### Drake53

#### Drake53

 Level 20

JoinedJan 1, 2018Messages777

Aeryn said:

 JASS:

local framehandle model = BlzCreateFrameByType("SPRITE", "model", gameUI, 0)

Click to expand...

Isn't BlzCreateFrameByType supposed to have 5 arguments?

 JASS:

native BlzCreateFrameByType takes string typeName, string name, framehandle owner, string inherits, integer createContext returns framehandle

#### Rigborn

#### Rigborn

 Level 8

JoinedApr 17, 2017Messages316

Resources: 1

#### Resources
Tutorials 1

Drake53 said:

 Isn't BlzCreateFrameByType supposed to have 5 arguments?

 JASS:

native BlzCreateFrameByType takes string typeName, string name, framehandle owner, string inherits, integer createContext returns framehandle

Click to expand...

 Yeah a small typo . Should be fixed now.

#### Drake53

#### Drake53

 Level 20

JoinedJan 1, 2018Messages777

Aeryn said:

 Yeah a small typo . Should be fixed now.


Click to expand...

Cool, I got it working now. Thanks for the tutorial.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Seems a bit niche.

It's also quite short/simple when you take away the introduction elements, you could probably slim down the tutorial to 4 quick steps.

I am still uncertain on my stance on this and might have to go and re-read the rules and whatnot.

However, regardless I have a few suggestions.

To simplify you can combine pictures into a single one, specifically the ones detailing what to edit in notepad++

OR

Can probably just show it in text form which would be more space-efficient.

Might also be easier to divide the tutorial into steps/sections somehow

#### BaobaoWan

#### BaobaoWan

 Level 2

JoinedJun 18, 2021Messages5

hi, aeryn, I used your method, but I failed. Can you tell me why? Look forward to your reply.

#### Attachments

model.rar

 926.7 KB

 · Views: 110



#### Rigborn

#### Rigborn

 Level 8

JoinedApr 17, 2017Messages316

Resources: 1

#### Resources
Tutorials 1

If your model uses a custom camera, you should use that cameraIndex. So your code









 JASS:

call BlzFrameSetModel(model, "war3mapImported\\YX16lemh.mdx", 0)

 becomes









 JASS:

call BlzFrameSetModel(model, "war3mapImported\\YX16lemh.mdx", 1)

I still have no idea how cameras are indexed internally so you have to try which one works.

#### BaobaoWan

#### BaobaoWan

 Level 2

JoinedJun 18, 2021Messages5

Aeryn said:

 If your model uses a custom camera, you should use that cameraIndex. So your code









 JASS:

call BlzFrameSetModel(model, "war3mapImported\\YX16lemh.mdx", 0)

 becomes









 JASS:

call BlzFrameSetModel(model, "war3mapImported\\YX16lemh.mdx", 1)

I still have no idea how cameras are indexed internally so you have to try which one works.


Click to expand...

Thank you very much for your help,Aeryn, I'll try to use your advice

#### Dark-Zalor

#### Dark-Zalor

 Level 19

JoinedOct 10, 2011Messages580

Resources: 28

#### Resources
Maps 25Spells and Systems 2Sounds 1

Hello

I need to rotate the model to display it in a sprite frame. With the camera change I still see my model from the top.

The problem is I have never edit models so if you can show me how to do it or a tutorial lin. That should be very nice.

I've already downoad Retera's editor V0.04

Thnaks for your help

#### Rigborn

#### Rigborn

 Level 8

JoinedApr 17, 2017Messages316

Resources: 1

#### Resources
Tutorials 1

I don't know if there is a specific tutorial for rotating models but there are tools out there. The one you should be looking for is oinkerwinkle's geoset translation tool or you can talk to Retera. He's made a tool that let's you rotate models easily but I can't share it without his permission. If it's just 1 model I can do it for you.

#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


Dark-Zalor said:

 Hello

I need to rotate the model to display it in a sprite frame. With the camera change I still see my model from the top.

The problem is I have never edit models so if you can show me how to do it or a tutorial lin. That should be very nice.

I've already downoad Retera's editor V0.04

Thnaks for your help


Click to expand...

You need Mdlvis 1.41 and War3 Model Editor, not Retera's model studio.

Make a helper bone in Magos and set it as a parent of the root bone. Then you rotate the helper bone with the sequence manager in Mdlvis.

#### Dark-Zalor

#### Dark-Zalor

 Level 19

JoinedOct 10, 2011Messages580

Resources: 28

#### Resources
Maps 25Spells and Systems 2Sounds 1

ok I've created a new bone, I've opened the editor to select it but wich coords should I gave to it? I think I have to rotate the model around ZX axis to make it face the top. But in this editor I don't see anything to do that.

And second problem in my sprite there is a very flashy glow that cover the full screen when th sprite show the model, d you know how to remove that?


 Last edited: Apr 5, 2022

#### Rigborn

#### Rigborn

 Level 8

JoinedApr 17, 2017Messages316

Resources: 1

#### Resources
Tutorials 1

Sorry for the late reply. If your model has particle emitters, you need to remove them.

#### C.B.King

#### C.B.King

 Level 8

JoinedNov 6, 2019Messages230

So i thought this would be to have sprites only 5, as a class selection screen, i got the model to show on the CustomUI, however it form top down facing right.

So i know to edit and rotate the model, however the editor will not open the mdl i extracted from the Retera's Model Studio, as suggested in this Thread

i already dont know how to edit models, and add bone, like FeelsGoodMan said, but cant get editor to open it to start with to even try and figure it out.

Gets: Unable To Load ""File Location"", unknown token "NumParticleEmittersPopcorn" in "Model"!

Now i did see you say particle have to be removed, but when extracting originally they particles was unticked, and if that not enough dunno how to remove particles from Retera.

EDIT: so lol i downloaded a model from Hive and edited to do some testing, after rotating and such, and now it is messy i guess you could say

Unsure how i broke it like this, but its pretty funny.


 Last edited: Jun 24, 2022

#### Rigborn

#### Rigborn

 Level 8

JoinedApr 17, 2017Messages316

Resources: 1

#### Resources
Tutorials 1

C.B.King said:

 So i thought this would be to have sprites only 5, as a class selection screen, i got the model to show on the CustomUI, however it form top down facing right.

So i know to edit and rotate the model, however the editor will not open the mdl i extracted from the Retera's Model Studio, as suggested in this Thread

i already dont know how to edit models, and add bone, like FeelsGoodMan said, but cant get editor to open it to start with to even try and figure it out.

Gets: Unable To Load ""File Location"", unknown token "NumParticleEmittersPopcorn" in "Model"!

Now i did see you say particle have to be removed, but when extracting originally they particles was unticked, and if that not enough dunno how to remove particles from Retera.

EDIT: so lol i downloaded a model from Hive and edited to do some testing, after rotating and such, and now it is messy i guess you could say

View attachment 401833

Unsure how i broke it like this, but its pretty funny.


Click to expand...

Could you provide link to model?

If you managed to rotate the model correctly, i guess it might have something to do with new patch.

#### C.B.King

#### C.B.King

 Level 8

JoinedNov 6, 2019Messages230

100% i did it wrong, lol wasnt sure what i was doing anywho.

i gotta figure it out if i wana add 5 in total and maybe more in future.

One thing am unsure about, is if you to export rotate and import, wont you need like 2 dif models per one you adding, so one for the sprite other for playable hero.

so doubles the map size.

#### Attachments

BEFarstriderSelection.rar

 2.3 MB

 · Views: 78



#### Dark-Zalor

#### Dark-Zalor

 Level 19

JoinedOct 10, 2011Messages580

Resources: 28

#### Resources
Maps 25Spells and Systems 2Sounds 1

I have the same problem, I don't know how to add bones, rotates, ...

to display sprites in frames.

My models are bugged I try to display them on a sprite but they are displayed really far away from the target location in the bottom of the screen...

Do you know a way to put them in the correct frame?

My map is already very heavy so if it will double the size I'm not sure that it is the good way in my case.

#### C.B.King

#### C.B.King

 Level 8

JoinedNov 6, 2019Messages230

I added a bone and rotated it, it did nothing.

#### C.B.King

#### C.B.King

 Level 8

JoinedNov 6, 2019Messages230

@Rigborn - ok so how do you rotate a Model to face forward on a sprite Properly?

Now your post says to rotate, but doesn’t say how to rotate properly.

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


To expand on the rotation part of this short tutorial via Retera's Model Studio:

Create a new bone.

Position the new bone at the origin (x: 0, y: 0, z: 0) of the model.

Place all bones, expect for the ones that are already under one, under your new bone (That is set your new bone as their parent).

Create a new Global Sequence and have it last for only one frame.

Within the Global Sequence, rotate only the new bone and every other bone will follow.
You would want the model lying down and its feet facing the left in the model view under the Editing Tab of Model Studio.

The Global Sequence will apply to all your model animations.

That's it. You can see how it looks in game after following Rigborn's tutorial and the short how-to from above in the image below.

#### Attachments

Sprite.jpg

 99.4 KB

 · Views: 127




 Last edited: Oct 6, 2022

#### Villagerino

#### Villagerino

 Level 69

JoinedJul 28, 2021Messages1,495

Resources: 64

#### Resources
Models 26Icons 36Maps 1Reforged HD Models 1

##
 Warcraft 3 Classic HD


FeelsGoodMan said:

 You need Mdlvis 1.41 and War3 Model Editor, not Retera's model studio.

Make a helper bone in Magos and set it as a parent of the root bone. Then you rotate the helper bone with the sequence manager in Mdlvis.


Click to expand...

I wonder if we can do anything with the particle emitters. They should be deleted BUT we need them as hell, because Infernals are not Infernals without fire particles. Maybe u have some Sprites working with emitters somehow in the Acolyte project? @FeelsGoodMan

#### Attachments

Warcraft_III_2024-02-27_17-15-20.mp4

 14.2 MB




 Last edited: Feb 27, 2024

 You must log in or register to reply here.


### Similar threads

Locked

[General]
 Sprite Facing Forward


UnknownCallerID

Feb 9, 2023

World Editor Help Zone

Replies0Views657

Feb 9, 2023
UnknownCallerID

 UI Frames, starting guide


Anchorist

Sep 8, 2019

Tutorial Archive

Replies2Views9K

Oct 3, 2020
Chaosy

Locked

[JASS]
 3D Model on UI


xorkatoss

Nov 27, 2019

Triggers & Scripts

Replies2Views2K

Dec 4, 2019
Tasyen

Locked

 Warcraft 2.0 Ui Bug


Nikita17

Nov 16, 2024

Triggers & Scripts

Replies12Views2K

Nov 17, 2024
Nikita17

 How to rotate a sprite frame without editing the model?


wiselen

Dec 6, 2025

World Editor Help Zone

Replies0Views375

Dec 6, 2025
wiselen

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