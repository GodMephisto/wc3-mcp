---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/fixing-faulty-death-decay-animations.40794/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Fixing Faulty Death Decay Animations
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/fixing-faulty-death-decay-animations.40794/>
#### General Frank

#### General Frank

 Level 92

JoinedNov 19, 2005Messages8,035

Resources: 369

#### Resources
Models 232Icons 124Packs 6Skins 2Ported Icons 3Tutorials 2

Fixing 'Popping back' parts of models tutorial.

- Powered by Magos' Model Editor

This tutorial will explain how to fix models which will stack their part on other units after death.

Tools needed:

Magos Model Editor

MDLX Converter

 Getting Started:

1. Convert the model with the issue with MDLX Converter to mdl format.

 NEVER EVER EDIT MODEL IN MDX FORMAT. Otherwise they screw up even more afterwards.

Fixing the actual model:

 1. When you open your model (in mdl format !) with Magos Model Editor and you will see your model in the main window. Place the main window in the middle of your screen.

 2. Go to Windows-> Geoset Manager. Place the Geoset Manager window on the lower left of the main window mentioned above.

 3. Go to Windows-> Animation Controller. Place the Animation Controller window on the upper left of the main window mentioned above.

 4. Go to Windows-> Geoset Animation Manager. Place that window on the right of the main window.

View attachment 13932

 5. Now play the decay bone animation with the Animation controller. You will see that some part of the model are still present in Decay Bone Animation.

View attachment 13933

To fix the problem you'll need to assign a geoset animation to the geoset that still shows up in all animations, but you need them to disappear in Decay Bone animation.

 6. Play a stand animation with the Animation controller.

 7. Now left-click + ctrl a part of the model of that you are sure you haven't seen in the decay bone animation, mainly the chest or something similar. You'll see that the model part (geoset) has flashed with a dark shading.

That geoset will be marked in the Geoset animation manager.

View attachment 13934

 8. In Geoset Animation Manager search for the Geoset Animation that is linked with the proper, disappearing in decay bone animation, geoset. Linked geosets will be shown on the lower left of the geoset animation.

View attachment 13935

 9. In that Geoset animation window press the 'Alpha' button in the upper right corner of that small window.

Now a a window called 'Alpha' will appear. You'll see a list with one or multiple digits which end with ....:0 or .....:1xx.

 10. Copy the whole list by using ctrl + C copy function. Close the 'Alpha' and the 'Geoset Animation', not 'Geoset Animation Manager' window.

View attachment 13936

To find a geoset that still unassigned with a geoset animation, play the decay bone animation, Ctrl+rightclick a still showing part (except the bones/corpse).

 11. Now create a new Geoset Animation by right-clicking in the 'Geoset Animation Manager' window on Create New. Tick the animated check box and then press alpha and enter those values you found in 9. into the list.

Press okay in the Alpha window to save it.

Now enter the geoset that is still unassigned in the 'Geoset ID' drop-down menu in the 'Geoset Animation' window.

 12. When you now play the decay bone animation you'll see that it is now disappeared. The geoset will not the visible in all animation except decay bone.

View attachment 13937

 13. Repeat that method on all geoset that are still present in decay bones animation. All except the corpse geoset. Save it like [Model]_A_001.mdl or something like that.

 NEVER EVER OVERWRITE THE ORIGINAL MODEL.

I added a model that still needs some repairs, to this thread, so that you can test this method out.

Get it to test your skill.

Test your result.


 Last edited: Dec 25, 2021

#### Mechanical Man

#### Mechanical Man

 Level 35

JoinedJan 18, 2005Messages5,781

Resources: 42

#### Resources
Models 32Icons 3Packs 1Maps 5Tutorials 1

So there is a difference between editing in mdl or mdx in magos editor? I thought that it converts it automatically.

#### General Frank

#### General Frank

 Level 92

JoinedNov 19, 2005Messages8,035

Resources: 369

#### Resources
Models 232Icons 124Packs 6Skins 2Ported Icons 3Tutorials 2

Mechanical Man said:

 So there is a difference between editing in mdl or mdx in magos editor? I thought that it converts it automatically.


Click to expand...

if you edit and save a file in mdx format with Magos, it could occur that the attachment point and sometimes the Event Objects mess up.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Why don't you just use Magos to convert between MDL and MDX? Or does the same bug occur?

#### General Frank

#### General Frank

 Level 92

JoinedNov 19, 2005Messages8,035

Resources: 369

#### Resources
Models 232Icons 124Packs 6Skins 2Ported Icons 3Tutorials 2

PurplePoot said:

 Why don't you just use Magos to convert between MDL and MDX? Or does the same bug occur?


Click to expand...

Magos can convert mdx/mdl, but it will a pain in the ass because lots of information will be lost.

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

Heh, I could have used this when I was a beginner.

Some things I would note:

-Make sure they know not to create empty geoset animations.

-Additionally, I think duplicate geoset animations crash WE too (2 geoset animations for one geoset)

This is very nice and handy. +REP

--donut3.5--

#### HeretoDLstuff

#### HeretoDLstuff

 Level 20

JoinedFeb 27, 2007Messages2,069

Resources: 9

#### Resources
Models 4Skins 3Maps 1Tutorials 1

Goodjob, It seems like alot of people could use this! +Rep

#### General Frank

#### General Frank

 Level 92

JoinedNov 19, 2005Messages8,035

Resources: 369

#### Resources
Models 232Icons 124Packs 6Skins 2Ported Icons 3Tutorials 2

donut3.5 said:

 Heh, I could have used this when I was a beginner.

Some things I would note:

-Make sure they know not to create empty geoset animations.

-Additionally, I think duplicate geoset animations crash WE too (2 geoset animations for one geoset)

This is very nice and handy. +REP

--donut3.5--


Click to expand...

Yes indeed. Thanks for that.

#### Elenai

#### Elenai

 Level 35

JoinedMay 22, 2007Messages5,364

Resources: 36

#### Resources
Models 14Icons 22

Could that be why my Elven Knight is bugged? Because I edited some things while it was in mdx format?

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

Hmm, it could be, but I always edit in mdx unless I'm using oinker's, so that I save space on my computer. I didn't know they bug until now.

--donut3.5--

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Outstanding tutorial, Herr General!


I have made the following minor changes:

Detached the signature file.

Retitled the tutorial.

Configured the images to display in their proper locations in the document.
This week I will go over it carefully (after you make improvements based upon user input) to correct any minor grammar or spelling errors (if any are present).

#### General Frank

#### General Frank

 Level 92

JoinedNov 19, 2005Messages8,035

Resources: 369

#### Resources
Models 232Icons 124Packs 6Skins 2Ported Icons 3Tutorials 2

Wolverabid said:

 Outstanding tutorial, Herr General!

I have made the following minor changes:
Detached the signature file.

Retitled the tutorial.

Configured the images to display in their proper locations in the document.
This week I will go over it carefully (after you make improvements based upon user input) to correct any minor grammar or spelling errors (if any are present).


Click to expand...

Alright, thanks.

I was in search of a proper name and only used the previous one.

Big thanks for fixing those attachies. Somehow they were very hard to finalize for me.

#### Kaitech[SanD]

#### Kaitech[SanD]

 Level 17

JoinedApr 11, 2007Messages1,816

Resources: 4

#### Resources
Skins 2Tutorials 2

I approve! Very good General! That orcs bracers look a lot like my ring..

P.S. Can't forget the +rep!

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved!

~ Thread moved to 3D Modeling Tutorials.

#### alcadeas

A

#### alcadeas

can some one give me a tutorial wich i can make a caracter(a model)or change one(skin color,weapons...)plsss!!!

#### General Frank

#### General Frank

 Level 92

JoinedNov 19, 2005Messages8,035

Resources: 369

#### Resources
Models 232Icons 124Packs 6Skins 2Ported Icons 3Tutorials 2

alcadeas said:

 can some one give me a tutorial wich i can make a caracter(a model)or change one(skin color,weapons...)plsss!!!


Click to expand...

This not the place for requests. Go to the request forum.

#### NinjaDino

#### NinjaDino

 Level 6

JoinedFeb 4, 2008Messages191

Resources: 1

#### Resources
Maps 1

i dont get this model editing, i only build maps, i have slow internet and all model editing tools are too big, i migh mabie download 10 mb

#### OlloX3

O

#### OlloX3

someone help were can i edit models wich program i have war3 model editor .. like doing a own model

#### Deolrin

#### Deolrin

 Level 50

JoinedApr 18, 2008Messages8,455

Resources: 85

#### Resources
Models 59Icons 21Packs 2Skins 1Tutorials 2

Thanks for that tutorial, it is totally outstanding! many people will use that or already using that, and i am one of them! also sorry for bringing an really old post back to life... +rep.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Well I always do this things completely on notepad, but well GJ.

#### General Frank

#### General Frank

 Level 92

JoinedNov 19, 2005Messages8,035

Resources: 369

#### Resources
Models 232Icons 124Packs 6Skins 2Ported Icons 3Tutorials 2

BlinkBoy said:

 Well I always do this things completely on notepad, but well GJ.


Click to expand...

You know explaining on notepad is rather hard for beginners, because notepad got no Graphical interface (listed selectable geos, response of model editor).

#### Liu

#### Liu

 Level 7

JoinedJun 2, 2021Messages157

Resources: 29

#### Resources
Models 13Maps 9Ported Models 2Ported Icons 5

- MDLX Converter are lost,could you plase update?

 You must log in or register to reply here.


### Similar threads

Locked

 Fixing Decay Bone anims on Geomerges


iristle

Jun 1, 2007

3D Modeling Tutorials

Replies1Views6K

Jun 27, 2007
Wolverabid

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

 Geosetanims - fix those randomly appearing corpses!


Horn

Jul 10, 2004

3D Modeling Tutorials

Replies0Views25K

Jul 10, 2004
Horn

 Geoset Importing and Exporting Guide


-Peper-

Oct 30, 2010

3D Modeling Tutorials

Replies31Views38K

Apr 14, 2012
-Peper-

 How to make cool portraits without needing a separate file


Spacebuns

Sep 5, 2014

3D Modeling Tutorials

Replies11Views11K

Nov 27, 2017
Diegoit

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