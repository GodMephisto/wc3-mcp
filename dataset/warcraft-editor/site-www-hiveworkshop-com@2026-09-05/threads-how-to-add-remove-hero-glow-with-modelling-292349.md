---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-add-remove-hero-glow-with-modelling.292349/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Add Remove Hero Glow with Modelling
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-add-remove-hero-glow-with-modelling.292349/>
#### Alok

#### Alok

 Level 15

JoinedSep 6, 2015Messages576

Resources: 26

#### Resources
Models 12Icons 6Skins 1Maps 3Tutorials 4

This is a detailed tutorial on how to add hero glow to a unit and how to remove hero glow from a hero with modelling. The length of the tutorial doesn't reflect the difficulty of the process, as the tutorial is written in full detail so that it's comprehensible even to the people who have no modelling experience whatsoever.

Table of contents:

- The programs you need

- How to add hero glow to a unit

- How to remove hero glow from a hero

- Hero glow models (needed for adding hero glow)

First of all, these are the programs you need:

WinMPQ v1.64 (only if the model is from the base game)

MDLX Converter

Modeling Resource: Customized Vertex Editor (also known as MatrixEater)

How to add hero glow to a unit:

Part 1 - WinMPQ - extracting the model (only for models from the base game):

If the model of the unit you want to add hero glow to is from the base game, you need to extract it from one of the MPQs located in your Warcraft 3 installation folder using the WinMPQ program linked above. To find the location and the name of the models within the MPQ, open the "Art - Model File" of the unit using that model within the Object Editor in your World Editor and look what's written under "Custom:". If you can't find the desired model in one MPQ, look in another MPQ (they are all located in your Warcraft 3 folder). To open an MPQ go File -> Open. To extract the models, right click on them in WinMPQ after you've opened an MPQ and click "Extract". If your model is a custom model not found in the base game, ignore this step and move to Part 2.

Part 2 - MdlxConv - converting the model file:

Now that you extracted the model/have your model, it's time to convert it to the .mdl format using MdlxConv. Open the program, click Convert and convert your model's file.

Part 3 - MatrixEater - importing the hero glow model:

Now that you have your model file in the .mdx format, it's time to add hero glow to it. To do this you need to download one of the files attached to the end of this tutorial (here), either normal or large hero glow, depending on what you need (normal = Jaina hero glow, large = Pit Lord hero glow). Open the model you converted in the previous step in MatrixEater. Click File -> Import and import the hero glow model you downloaded now from this tutorial. A big window should be opened now. Go to the "Animation" tab of that window. Click on the "Stand -1" animation in the list. Un-tick the "Import this Sequence" because you don't want to import that animation. Click "Finish" at the bottom of the window. Now all you need to do is to save the model. Go File -> Save or Save as and you are done. To use the model in-game you have to convert it back to the .mdx format following the procedure described before, in the Part 2 of this tutorial (see above). Enjoy!

The end product should look something like this:

How to remove hero glow from a hero:

Part 1 - WinMPQ - extracting the hero (only for the models from the base game):

If the model of a hero you want to remove hero glow from is from the base game, you need to extract it following the procedure described in the Part 1 of the previous part of the tutorial (see above). If your model is a custom model not found in the base game, ignore this step and move to Part 2.

Part 2 - MdlxConv - converting the model file:

See the Part 2 of the previous part of the tutorial on how to do this (see above).

Part 3 - MatrixEater - removing the hero glow geoset:

Open the extracted/custom model of the hero you want to remove hero glow from in MatrixEater. In the window to the left titled "View Controller", set the Editability ("Editable") off for every geoset except for the hero glow geoset. The geosets should turn green in the model preview window as you mouse them over in the View Controller window. That way you can identify the hero glow geoset. Now select the whole hero glow geoset by dragging a box with your mouse around all the vertices (black dots on one of the three white screens) until they turn red. Once the whole geoset is selected delete it by clicking Delete either on your keyboard or in the menu. Save your model and you are done. The only thing left is to convert the model back to the .mdx format following the procedure described in the Part 2 of the previous part of the tutorial (see above). Enjoy!

Here are the hero glow models you need for adding hero glow in the attachment (choose either normal or large, depending on what you need (normal = Jaina hero glow, large = Pit Lord hero glow)):

#### Attachments

TeamGlow.mdl

 5.3 KB

 · Views: 651



TeamGlowLarge.mdl

 5.7 KB

 · Views: 487




 Last edited: Mar 11, 2017

#### Eagle XI

#### Eagle XI

 Level 30

JoinedMar 9, 2012Messages1,557

Resources: 149

#### Resources
Models 11Icons 129Packs 5Skins 4

Thanks, perhaps you can also describe the process for adding teamglow particles to the weapon of an model.

#### Alok

#### Alok

 Level 15

JoinedSep 6, 2015Messages576

Resources: 26

#### Resources
Models 12Icons 6Skins 1Maps 3Tutorials 4

Eagle XI said:

 Thanks, perhaps you can also describe the process for adding teamglow particles to the weapon of an model.


Click to expand...

I still haven't done that ever. If I do and find out, I will.

#### Alok

#### Alok

 Level 15

JoinedSep 6, 2015Messages576

Resources: 26

#### Resources
Models 12Icons 6Skins 1Maps 3Tutorials 4

Update: Re-did the whole tutorial using MatrixEater for a better result, because it can transfer bones too.

Eagle XI said:

 perhaps you can also describe the process for adding teamglow particles to the weapon of an model.


Click to expand...

I found a possible way how to do it. However, it is very complicated and heavily depends on the model from which you are transferring hero weapon glow and the model to which you are transferring it, and writing the tutorial would take a lot of effort. Better watch the MatrixEater video tutorials, they will give you a better picture of an idea how to do it:

And then, when you become adept at it, you can create your own tutorial on how to do it, if you want.

#### Alok

#### Alok

 Level 15

JoinedSep 6, 2015Messages576

Resources: 26

#### Resources
Models 12Icons 6Skins 1Maps 3Tutorials 4

Update: Re-did the whole tutorial again. Now it's using the hero glow models I made (normal and large hero glow) and the procedure is much simpler. The hero glow now looks and performs like an actual proper hero glow with all the animations and everything.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Short, sweet, and to the point. Definitely a popular question.

Approved.

#### Diegoit

#### Diegoit

 Level 23

JoinedApr 8, 2017Messages1,624

Resources: 11

#### Resources
Models 5Icons 5Maps 1

This a really great tuto, this will help me a LOT.

THANKS

#### Gumberbunbder

#### Gumberbunbder

 Level 13

JoinedSep 25, 2013Messages793

Resources: 4

#### Resources
Icons 1Maps 3

Thanks for this info, very easy to follow, appreciate it much!

 You must log in or register to reply here.


### Similar threads

 Removing geosets/glow


alfredx_sotn

Apr 21, 2004

3D Modeling Tutorials

Replies4Views30K

Oct 21, 2005
Magos

Locked

 Removing Hero Glow


Watcher

Nov 20, 2017

Modeling & Animation

Replies4Views2K

Nov 21, 2017
Watcher

Locked

 Removing Hero Glow Stuck


warcraftmodding123

Mar 21, 2016

World Editor Help Zone

Replies8Views2K

Mar 25, 2016
warcraftmodding123

Locked

 Basic Weapon Removal


Kimbo

Mar 14, 2007

3D Modeling Tutorials

Replies5Views28K

Sep 6, 2007
Kimbo

Locked

 Remove Hero Glow


Deminonon

Feb 18, 2020

World Editor Help Zone

Replies11Views4K

Feb 18, 2020
Deminonon

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