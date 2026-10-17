---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/animated-textures-tutorial.158421/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Animated Textures Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/animated-textures-tutorial.158421/>
#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

#
 Animated Textures Tutorial

###
 by Amigurumi

 Index


Introduction

Tools needed

Textures

Animated Texture

Moving Texture

Examples

##
 Introduction

Hello, and welcome to my short tutorial, about how to add animated textures easily by yourself!

There is already a tutorial, but only with notepad, and I found it a bit complicated.

So I thought that I make another one, where you only need some tools, and a bit work.

This is not a very hard tutorial, but you should have some basic knowledge of textures/models.

(It’s also not needed, but recommended!)

You only need free tools for this tutorial.

I hope you learn something from it, and have fun while learning  .

##
 Tools needed

These are the tools, which are needed to make animated textures:

(Tools marked with an *, are not needed, but you can make the custom models/textures with them.)

Notepad

War3 Model Editor

MDLX Converter

Milkshape 3D* (not free)

Gimp*

##
 Textures

So, we will practice with a water texture. And which model do we know, has an animated

water texture? Yes, the ‘Water Elemental’ .

Now open Magos Model Editor, and open the attached model file.

The model already has the texture ‘Textures\Lords0000.blp’, but it needs more, to create an animated texture.

So click the toolbar button ‘Windows -> MPQ Browser’.

Now a new window pops up. There you click on ‘File -> War3.mpq’, to open the wc3 mpq.

There are all files for warcraft 3 saved.

Now search the folder ‘Textures’ and open it.

In that folder are many *.blp files. But we are searching the water texture. So press ‘L’, and the

Marker should jump to the ‘LavaLump.blp’. Now scroll a bit down, until you can see

‘Lords0000.blp’ and the other ones.

It should look now like that:

The model already uses the ‘Lords0000.blp’ texture, so we don’t have to add it.

Now add the other 7 textures (‘Lords0001.blp’ – ‘Lords0007.blp’).

You can easily do that with right click on the texture, and then ‘Use as texture’.

Our model uses now all 8 water textures!

##
 Animated Texture

Now we will make the animated texture.

Open Magos Model Editor, if you closed it, or minimized it, and click at the toolbar

on ‘Windows -> Material Manager’. A new window pops up:

For this model, there is only one Material. It’s something called like ‘Material 04474770’.

(The numbers after the material are always random, so it might be, that it's not exact this number)

Double click this material to open a new window. There double click again on the MaterialLayer.

Another window should open:

You see, that it uses one texture at the moment: ‘Textures\Lords0000’.

To ‘animate’ it, check the box ‘Animated’ and click on the button ‘Texture ID’.

Now this window should open:

This looks a bit confusing at the beginning, but it’s easier than it looks like.

The ‘0’ before the ‘:’ shows the keyframe, where something happens.

The ‘0’ after the ‘:’ shows the texture ID.

To understand the keyframes better, go back to the main window of magos, and click

on ‘Windows -> Sequence Manager’. There should be an animation called ‘Stand’.

This is the normal stand animation.

Double click on it, and a new window opens. We need only the ‘Interval’ for this tutorial.

You see, the interval starts at 0, and goes to 1050. That means that the animation finishes,

when the interval reaches 1050.

Back to the texture animations.

You have 8 textures, and it should look fluently, so we make 8 parts. We have 1050 keyframes, and

we have to add 8 textures, so one texture every 150 keyframes.

It should look like this:

 Code:

0: 0
150: 1
300: 2
450: 3
600: 4
750: 5
900: 6
1050: 7

Now, you are done!

Now go to the Animation Controller (‘Windows -> Animation Controller’), and play the stand animation.

It should look like this:

Voilà!

##
 Moving Textures

After we learned how to make an animated texture with more than one texture, you will learn now,

how to make a moving texture, with only 1 texture, which saves space.

At first, download the attached ‘Stone.mdx’ model, and open it with

Magos Model Editor. Now click the Window button at the toolbar, and then on MPQ Browser, to

search our texture. In this example, we will use the ‘Textures\grad2b.blp’.

Open the ‘War3.mpq’, open the ‘Textures’ folder,

and search the ‘grad2b.blp’. When you found it, right click on it, and click on

‘Use as texture’.

Now we have to attach the texture to the model.

Click the window button in the toolbar, and click on Material ‘Manager’, and

double click on the Material. Now a new window should pop up, and there you right click on the

white background, and click ‘Create New’.

Now another window appears, and there, you check the ‘Unshaded’,

make the Filter Mode to ‘Additive’, and give it your texture.

It should look like that:

If you look at your model preview screen, you can see now, that the stone has a white thing

at its top. Well, let’s continue the work.

Again, click the window button in the toolbar, and select the ‘Texture Animation Manager’

and right click at the white background, and select ‘Create New’.

A new window pops up, with 3 different options:

Translation, Rotation and Scaling.

We need the Translation one, so check the ‘Animated’ box, under the

Translation category. Now we can click the Translation button.

A new window pops up.

At first, you can see at the bottom, a small field with the name ‘Interpolation Type’.

Set this to ‘Linear’, so the animation looks flowing.

Now to the second part of the translation window.

 Code:

0: { 0, 0, 0 }

Looks strange, and I’m also not 100% sure what these numbers in the brackets do.

But I can tell you: the first number is for the keyframe.

So our model’s animation goes from 100 to 500.

Change the first 0 to 100.

 Code:

100: { 0, 0, 0 }

Now copy this whole thing 3 times, so it looks like this:

 Code:

100: { 0, 0, 0 }
100: { 0, 0, 0 }
100: { 0, 0, 0 }
100: { 0, 0, 0 }

After this change the second 100 to 250, and the third 100 to 251, so the animation looks nice.

It should look like that:

 Code:

100: { 0, 0, 0 }
250: { 0, 0, 0 }
251: { 0, 0, 0 }
500: { 0, 0, 0 }

But we’re not finished. Now we change the second 0 in the brackets of the second keyframe to

0.8, and the second 0 of the third keyframe to -0.8, so it looks nice, later on.

That’s how it should look like:

 Code:

100: { 0, 0, 0 }
250: { 0, 0.8, 0 }
251: { 0, -0.8, 0 }
500: { 0, 0, 0 }

You can try a bit with the 0.8, to find the perfect animation.

Now to the next step:

Open the Material Manager again, and double click on the material.

Now open the second material, with the animated texture.

There should be a small box, with the header: ‘Animated Texture ID’.

There, select the animated texture, we made.

Now click okay, and return to the preview screen.

Open the ‘Animation Manager’, and play the stand animation:

(Sorry about the quality, but it's a fast animation, and hard to record.

Now there is a problem with magos: It can't save the animated texture

(That's an odd magos bug, but there is a way to fix it).

Open now MdlxConverter, and convert the rock.mdx to rock.mdl.

Now open the rock.mdl file with notepad. Scroll down, until you reach the

the Materials.

Here a screenshot, how it should be changed:

If you haven't understand this, just look at this:

 Code:

Materials 1 {
 Material {
 Layer {
 FilterMode None,
 static TextureID 0,
 }
 Layer {
 FilterMode Additive,
 Unshaded,
 }
 }
}

That's how it looks like. Our animated texture is the second one, so we

have to change the second layer.

 Code:

 Layer {
 FilterMode Additive,
 Unshaded,
 }

Only add this:

 Code:

 static TextureID 1,
 TVertexAnimId 0,

so it looks like this:

 Code:

 Layer {
 FilterMode Additive,
 Unshaded,
 static TextureID 1,
 TVertexAnimId 0,
 }

and you finished the notepad work! Just save it, use the MdlxConverter

to convert it back to rock.mdx, and watch it in Magos!

(Thanks to DonDustin, for the explanation how to fix the magos bug!)

Voilà!

##
 Examples

Now I want to list some example models, which use animated/moving

textures. If you search in the model section under DC (Directors Cut),

you will find those well-done models.

### Lich King by l0w_kwaliti

### Prime Matron by dickxunder

### Lightning Elemental by jigrael

##
 Addendum

If the model shows up correctly in the model editor, but not in game (e.g. the rest of the texture is animated when yo don't want it to be), try following this guide to get the animated portion onto a separate material:

Putting what you learned about Animated Textures into action

#### Attachments

TutorialWaterSphere.mdx

 4.8 KB

 · Views: 796



TutorialWaterSphereFINISHED.mdx

 6.7 KB

 · Views: 685



Stone.mdx

 4.9 KB

 · Views: 652



LEExample.gif

 191.3 KB

 · Views: 8,162



PMExample.gif

 546.4 KB

 · Views: 10,969



LKExample.gif

 826.2 KB

 · Views: 13,541



StoneFINISHED.mdx

 5.2 KB

 · Views: 692



HowToMakeAnimatedTexturesWork.jpg

 65.4 KB

 · Views: 9,646




 Last edited by a moderator: Jun 17, 2018

#### Vengeancekael

#### Vengeancekael

 Level 37

JoinedAug 11, 2009Messages6,521

Resources: 16

#### Resources
Tools 1Maps 13Spells 1StarCraft II Resources 1

 welcome at my short tutorial


Click to expand...

at = to


-----------------------------------------------------

 You only need free tools for this tutorial.


Click to expand...

Then why are you saying that Milkshape isn't free xD


-----------------------------------------------------

 So click at the toolbar on Windows.....


Click to expand...

Should be:

 So click the toolbar button Windows....


Click to expand...

 click at the toolbar


on ‘Windows


Click to expand...

Same thing.


-----------------------------------------------------

 Double click on it


Click to expand...

Should be :

 Double click it


Click to expand...

-----------------------------------------------------

 So, you are done!


Click to expand...

Should be :

 Now, you are done!


Click to expand...

-----------------------------------------------------

 Voila!


Click to expand...

Should be :

 Voilà!


Click to expand...

-----------------------------------------------------


Overall a very nice tutorial, good job

#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

will correct everything now.

 Then why are you saying that Milkshape isn't free xD


Click to expand...

well, it's an optional tool, only to show with which tools you can model, to create models

with animated textures.

+rep

#### Vengeancekael

#### Vengeancekael

 Level 37

JoinedAug 11, 2009Messages6,521

Resources: 16

#### Resources
Tools 1Maps 13Spells 1StarCraft II Resources 1

Amigurumi said:

 will correct everything now.


well, it's an optional tool, only to show with which tools you can model, to create models


with animated textures.


+rep

Click to expand...

Ah, now i get it xD


Well thx for the rep

#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

~bump~

still waiting for approval/comments/suggestions/critisize/and so on.

#### cookies4you

#### cookies4you

 Level 9

JoinedMay 9, 2009Messages535

Resources: 1

#### Resources
Spells 1

very nice tutorial indeed. usfullnow, 0%, since i can't get full. usefulness, pretty high. good tutorial though

#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

BIG UPDATE!!!!!!

added MOVING TEXTURES!

//edit:

ignore the black thing in the gif animation...

#### Vengeancekael

#### Vengeancekael

 Level 37

JoinedAug 11, 2009Messages6,521

Resources: 16

#### Resources
Tools 1Maps 13Spells 1StarCraft II Resources 1

 Moving Textures


Click to expand...

Wow, i didn't find any typos Good job. +rep

#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

thanks again, for looking for typos!

+rep

#### tee.dubs

#### tee.dubs

 Level 15

JoinedJan 25, 2008Messages449

Resources: 17

#### Resources
Models 11Icons 6

This is fantastic, i've never understood moving textures before, this is pretty simple and easy to follow, thanks so much for a great tutorial. Maybe i will take part in the tank competition now!

#### cookies4you

#### cookies4you

 Level 9

JoinedMay 9, 2009Messages535

Resources: 1

#### Resources
Spells 1

good job with the moving texture's tutorial. Disco Ball is Disco :3.

i vote for approval

Edit. OMFG ITS MY 444 POST!!!

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

nice tutorial. it is good to know that we have another one now which is up-to-date. Animated textures/animated materials are pretty confusing at the beginning, but once you understand how they work and how to create them, they get actually easy and fast to make and always give your model some catchy effect.

The only small suggestion I could give you is that you provide some examples how awesome they could be


 Last edited: Feb 2, 2011

#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

DonDustin said:

 nice tutorial. it is good to know that we have another one know which is up-to-date. Animated textures/animated materials are pretty confusing at the beginning, but once you understand how they work and how to create them, they get actually easy and fast to make and always give your model some catchy effect.

The only small suggestion I could give you is that you provide some examples how awesome they could be

Click to expand...

thanks, I'm glad to hear, that also someone, who models a long time, says

that this is useful

Well, you mean, I should make a better example, than the rock?

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

maybe some links like l0w_kwaliti's LichKing model (the sword) or GF's Fire Dragon - Fire Blade(gems and sword) so people actually see how to use it in practice

#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

DonDustin said:

 maybe some links like l0w_kwaliti's LichKing model (the sword) or GF's Fire Dragon - Fire Blade(gems and sword) so people actually see how to use it in practice


Click to expand...

okay, I will search some good examples, and post them here.

thanks, neat idea

[EDIT]

updated! added now 3 examples of models, which uses animated/moving textures!

thanks to DonDustin for the great idea!


 Last edited: Feb 17, 2010

#### M0rbid

#### M0rbid

 Level 22

JoinedDec 20, 2009Messages1,894

Resources: 47

#### Resources
Models 3Icons 29Skins 15

I tested it with myself ;-)


(I have no experience with mago):


Definitely useful, very good to follow and well documented with underlining images!


I vote for approval!

#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

I thought about adding some easy understandable drawings, for the beginning.


what do you think?

[EDIT]


fixed something in the middle part about a magos bug.


(thanks to DonDustin for helping.)


 Last edited: Mar 5, 2010

#### M0rbid

#### M0rbid

 Level 22

JoinedDec 20, 2009Messages1,894

Resources: 47

#### Resources
Models 3Icons 29Skins 15

Ok, I promised you a little review and so I go now

Overview:  The Tutorial is designed for not so experienced or totally unexperienced persons and this is really what it aims for. It's well documented with images that show what the text describes. The chapters are easy to follow and even total newbies will get the result they are looking for.

Type of Writing: The writing style is full of life and that's what makes it interesting, also it's very detailed, which helps the reader to follow the explanations! Some smaller descriptions aren't on the level of the overall text but this doesn't really matter:


"Now, you are done!


Now go to the Animation Controller (‘Windows -> Animation Controller’), and play the stand animation."


This "now,...now" for example is a little annoying.


Another example is: "You have 8 textures, and it should look fluently, so we make 8 parts.". Maybe use "divide" instead of "make"!


Overall it's well written, and nice to read: 9/10

Visual Makeup:


This is a big plus point of the Tutorial: The colours that show the paths in Magos are very good underlined by colour. (Tip: It would be great if you would give every button in your tutorial a colour). Also the chapters are well done and the text passages are not to short and not to long. The hidden images help making the tutorial easy to handle!


Overall it's easy and compact organized, which helps the tutorial a lot: 9.5/10

Topic: Another big adventage of this tutorial, it's really EASY to follow, even a total beginner with Magos and Modelling as able to follow it, maybe even if he uses a computer for the first time. I did everything like it's said in the tutorial and it worked fine for me, all small mistakes are already fixed and everything is mentioned.


One or two things I've to mention though: 1. It's very easy to follow the tutorial but at some points it isn't clear what you're really doing there if you're a beginner. Sentences like: "This is for changing ....." or "This changes the value of...." would give a better imagination of what the reader does in that moment and how he can individualize his work!


2. I already mentioned that and you seem to get first ideas of how to change it, is that the reader doesn't really know what he's aming for if he doesn't zoom through the whole tutorium which is a little annoying! Maybe add a pic of a model that uses those effects or something like that.


By the way, this is another great idea of the tutorial: To show pictures of Models which include the explained technique!


Overall the content of the tutorial is very well done, but some minor changes would be nice: 8.5/10

Overall: 27/30

#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

a BIG thanks to you for taking your time to review my tutorial

 This "now,...now" for example is a little annoying.


Click to expand...

I know, english isn't my first language, so I'm not very good in reshaping some words.

 Tip: It would be great if you would give every button in your tutorial a colour


Click to expand...

hmm, I don't get this part. do you mean for every button in magos, that I mentioned?

 By the way, this is another great idea of the tutorial: To show pictures of Models which include the explained technique!


Click to expand...

yes, DonDustin gave me the idea, and I found it great.

#### M0rbid

#### M0rbid

 Level 22

JoinedDec 20, 2009Messages1,894

Resources: 47

#### Resources
Models 3Icons 29Skins 15

Amigurumi said:

 hmm, I don't get this part. do you mean for every button in magos, that I mentioned?


Click to expand...


Yes, that's my point, it would really help to get along the tutorial!


EDIT: sry, for forgetting: This +rep is very deserved!

#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

M0rbid said:

 Yes, that's my point, it would really help to get along the tutorial!


Click to expand...


okay, I will think about that.

#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

~bump~

#### Vengeancekael

#### Vengeancekael

 Level 37

JoinedAug 11, 2009Messages6,521

Resources: 16

#### Resources
Tools 1Maps 13Spells 1StarCraft II Resources 1

Why bump? It's a tutorial, not a map development thread.

#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

Vengeancekael said:

 Why bump? It's a tutorial, not a map development thread.


Click to expand...


Well, I wait for approval.

#### Misha

#### Misha

 Level 49

JoinedJun 9, 2008Messages16,523

Resources: 73

#### Resources
Models 64Icons 1Packs 2Skins 4StarCraft II Resources 2

A truly good and easy to follow tutorial :]

#### Jaret

#### Jaret

 Level 7

JoinedDec 23, 2009Messages1,434

Resources: 10

#### Resources
Icons 8Packs 1Tutorials 1

OMG! I know S*** about animating and modeling. And I thought that this was hard. Real HARD.

Only now I am convinced that This Animated texture part about the model isn't really that hard.

REP+ For...This.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

hmm I think this is aproveable. Even though it shows just how to do it in magos alone, I think is far enough for anyone to learn their basics.

+12 rep.

#### Kwah

#### Kwah

 Level 24

JoinedMay 9, 2007Messages3,561

Resources: 12

#### Resources
Icons 7Maps 3Tutorials 2

BlinkBoy sez it's approveable, I approve.

#### ZiDI

Z

#### ZiDI

Nice Tutorial, I will use

#### HappyCockroach

#### HappyCockroach

 Level 40

JoinedFeb 4, 2009Messages3,170

Resources: 76

#### Resources
Models 34Icons 40Packs 1Tutorials 1

After you did this tutorial, animated texture effects are getting much more accessible to the modelling crowd, so the quality of the models in the resource section is certainly raising! good job

#### Amigurumi

#### Amigurumi

 Level 23

JoinedApr 7, 2008Messages1,621

Resources: 50

#### Resources
Models 27Icons 6Packs 3Maps 7Spells and Systems 6Tutorials 1

haha, thank you )

#### TinkerLord

#### TinkerLord

 Level 44

JoinedOct 20, 2010Messages2,934

Resources: 104

#### Resources
Models 83Icons 17Spells and Systems 3Tutorials 1

thank you, this tutorial let me make lava elemental and poison elemental.. i wan upload to hiveworkshop but i dont know how to import......(the texture is lava crack that i edit and cut it into 1 and 1)(sorry for my broken english)

and it is work in wc3 model editor but i dont know how to import to warcraft 3 world editor(model i made)

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

Useful tutorial, but I think Animated Textures in WC3 is kinda broken. If I have a material with 2 layers and the first layer uses animated texture but the 2nd layer is static, in-game both layers will animate. I can't do something like a Dota 2 Chaos Knight where his armor suit is static but his body is animated flowing lava.

#### Veronnis

#### Veronnis

 Level 14

JoinedJul 19, 2010Messages309

Resources: 18

#### Resources
Models 3Icons 1Skins 5Maps 4Reforged HD Skins 4Sounds 1

Great info, thanks!

#### TastyBamboo

#### TastyBamboo

 Level 7

JoinedFeb 23, 2010Messages227

Bumping in the hopes for an updated tutorial/pictures because reforged hype and I want to get into custom maps hard.

#### Veronnis

#### Veronnis

 Level 14

JoinedJul 19, 2010Messages309

Resources: 18

#### Resources
Models 3Icons 1Skins 5Maps 4Reforged HD Skins 4Sounds 1

Dang, think I understood this 4 years ago. But now im clueless again lol and the pictures are broken D:

#### OVOgenez

#### OVOgenez

 Level 38

JoinedJul 7, 2019Messages382

Resources: 27

#### Resources
Models 24Packs 2Spells and Systems 1

What about Rotation and Scaling? Are they work?

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

I too wonder about the other values usually ignored, they don't seem to work when I tried them.

Does more then one animated texture in the same material present any problems?

Spacebuns said:

 Useful tutorial, but I think Animated Textures in WC3 is kinda broken. If I have a material with 2 layers and the first layer uses animated texture but the 2nd layer is static, in-game both layers will animate. I can't do something like a Dota 2 Chaos Knight where his armor suit is static but his body is animated flowing lava.


Click to expand...

You require a specific method in textures to achieve this or separate geosets instead however can be done.

#### OVOgenez

#### OVOgenez

 Level 38

JoinedJul 7, 2019Messages382

Resources: 27

#### Resources
Models 24Packs 2Spells and Systems 1

Tal'Rarity said:

 I too wonder about the other values usually ignored, they don't seem to work when I tried them.

Does more then one animated texture in the same material present any problems?

You require a specific method in textures to achieve this or separate geosets instead however can be done.


Click to expand...

After tests, rotation and scalation work fine, it's just rarely used anywhere. I also had no problems with the material where there was 1 static layer and 2 with texture animation. Here's an example

#### Attachments

test3.mdl

 6.2 KB

 · Views: 72



test3.mdx

 3.8 KB

 · Views: 95



#### Greedy Martyrdom

#### Greedy Martyrdom

 Level 49

JoinedMar 16, 2018Messages991

Resources: 60

#### Resources
Models 45Skins 1Sounds 14

There are no images... D:

 You must log in or register to reply here.


### Similar threads

Locked

 Adding Animated Textures with NOTEPAD ALONE


alfredx_sotn

Apr 30, 2007

3D Modeling Tutorials

Replies0Views16K

Apr 30, 2007
alfredx_sotn

 Creating Ghosts


Daelin

Jul 14, 2005

3D Modeling Tutorials

Replies10Views27K

Aug 14, 2023
DREADMAN

 Removing geosets/glow


alfredx_sotn

Apr 21, 2004

3D Modeling Tutorials

Replies4Views30K

Oct 21, 2005
Magos

 The Crystal Effect


olofmoleman

Dec 9, 2006

3D Modeling Tutorials

Replies9Views33K

Dec 10, 2006
operator

 Making Effects in Magos - Advanced Techniques


WILL THE ALMIGHTY

Feb 23, 2008

3D Modeling Tutorials

Replies14Views10K

Nov 2, 2008
3rgytr54

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

3D Modeling Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top