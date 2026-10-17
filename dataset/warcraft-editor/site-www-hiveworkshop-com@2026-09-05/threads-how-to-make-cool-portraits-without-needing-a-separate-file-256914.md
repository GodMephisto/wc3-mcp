---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-make-cool-portraits-without-needing-a-separate-file.256914/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Make Cool Portraits Without Needing a Separate File
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-make-cool-portraits-without-needing-a-separate-file.256914/>
#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

I believe many people already know this, but I haven't seen any Tutorial that addresses this. So here it goes, this is how to make a portrait with background glow without having to make a separate file of the model.

How to make cool portraits without needing a separate file

Tutorial Difficulty: Easy


Tools needed: Magos’s Model Editor, MDLVIS


This tutorial assumes you have basic knowledge of geosets, materials, and animation sequences.

Step 1: Choose a background screen

There are multiple ways to make a background screen for your model’s portrait, however I will go for the simplest method, that is extracting it from a pre-existing model.

1.1. Open Model Editor

1.2. Go to Windows -> MPQ Browser

1.3. Go to any portrait model file, for example “Units\Human\Arthas\Arthas_Portrait.mdx” in the MPQ Browser

1.4. CTRL+Click on the glowing part behind Arthas’s portrait model

1.5. Go to Windows -> Geoset Manager, a geoset has been selected for you. It is the geoset that you’ve just chosen by CTRL-clicking.

1.6. Right click the geoset and choose Export. Save it as a file with .geo extension, for example “Background.geo”

Step 2: Make the background screen

For this tutorial we will use the https://www.hiveworkshop.com/forums/models-530/outpost-53313/ model by Kofi_Banan.

2.1. Open “outpost.mdx”

2.2. Go to Windows -> Node Manager

2.3. Create a new bone by right clicking the white space and choose New Bone. You may rename it to anything (for example “background”) for easier reference later.

2.4. Go to Windows -> Texture Manager, create a new texture using the “Create Empty” option. The newly created texture is named “Replaceable ID 0”, so right click on it, choose Edit and change Replaceable ID to 2.

2.5. Go to Windows -> Material Manager, create a new material using “Create New” option.

2.6. Open the material you’ve just created and create a new layer in it.

2.7. In the Texture ID field, choose Replaceable ID 2. In the Filter Mode field, choose Additive. You may also tick the “Unshaded” flag so that the color in the portrait will look brighter later on. After you finish don’t close the Material Manager window yet, we’ll come back to it later.

The Filter Mode must be set to Transparent/Additive/Add Alpha, or the material will show up in animations other than Portrait, which is undesirable.

2.8. Go to Windows -> Geoset Manager, import the background geoset you’ve previously extracted from Step 1 with the “Import” option.

2.9. Attach the imported geoset to the “background” bone.

2.10. Double click the newly imported geoset’s name. If you can’t remember its unique geoset ID, open the Node Manager again and double click the “background” bone to find the ID we’re looking for. In the Misc field of the Geoset window, choose the material you’ve created in step 2.5-2.7. Now the glow finally shows up, but we’re not done yet.

2.11. Save the model for now, and open it in MDLVIS. Go to Module -> Sequence editor. Find the Stand animation in the "Current sequence" box, select and copy all frames using Ctrl+C. Next, find a space in the model's entire sequence (choose "All lines" in the Current sequence box) that hasn't been used by any other animation, and has enough length for the animation we're going to clone. Paste it using Ctrl+V.


In this case, I used the frames between 1400-1900 for the portrait sequence, as the sequences of Stand, Attack and Death are 0-500, 1000-1300 and 2000-4000 respectively. Lastly, click Create to make a new animation, change its name to Portrait, and set "Frame interval" to 1400-1900. Save the model when you're done. Thanks Anvil for the tip.

If the model you’re working with already had Portrait animation sequences, skip this step and go to the next one.

2.12. Go back to the Material Manager and open the material created in step 2.5-2.7. Open the layer and tick Animated box under Alpha field. Type in the blank field lines in X: Y format, where X is the beginning frame of each animation sequence, and Y=0 (hide) or 1 (show) depending on whether or not you want the glow to show during the sequence that begins with frame X. In this tutorial we want the glow to only show during the portrait sequence, hence Y=1 only if X=beginning frame of the Portrait sequence.

2.13. Go to Windows -> Camera Manager and create a new camera angle if there isn't already one. The “Create New from Current View” is really handy here.

2.14. Go to Windows -> Animation Controller and watch through all animations to check if step 2.12 has been done correctly. If it has, then congratulations, you have finished the main part of this tutorial.

Step 3: Tweak the background screen

If you are not satisfied with the way the portrait looks in-game, here is a list of things you could do to improve it:

3.1. Go to Windows -> Model Editor. Select the red vertices that make up the square that is your background screen. Use Transformations -> Translation and Scaling options to tweak the background. Translation moves it in X,Y,Z coordinates relative to its current position, and Scaling changes its X,Y,Z dimensions (for Scaling you only need to change Y and Z). When you scale also remember to check the "Center of Mass" box so the geoset doesn't change position when you rescale. MDLVIS can also do this task more efficiently.

In Model Editor


In MDLVIS

3.2. Go to Windows -> Camera Manager. Delete the old camera angle and make a new one you like, using "Create New from Current View". Or if you are good at coordinates, you can just straight up edit the fields of old camera angle.

3.3. Bored of the same old glow? Import a new texture for your background screen. You do this by going to the background screen’s material in the Material Manager and change the Texture ID field to the newly imported texture. Generally anything can work, but sky textures are the best in my opinion.


Yes, I know WoW rips are frowned upon here, but used it for the sake of this tutorial.


 Last edited: Sep 6, 2014

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Some screenshots can help since I get a lil confused at some points. However this is a good tut.

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

dtnmang said:

IMPORTANT: The Filter Mode must be either Transparent/Additive/Add Alpha, or else it will not work properly!


Click to expand...


That's not true.


Also, this tutorial is not finished... if you create an empty sequence, the model will not be animated in the portrait's sequence, at least copy the stand animation keyframes with MdlVis, and create a new animation based on that.

#### Deleted member 238589

D

#### Deleted member 238589

Like Cakey said, some screenies would be good.

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

Anvil said:

 That's not true.


Also, this tutorial is not finished... if you create an empty sequence, the model will not be animated in the portrait's sequence, at least copy the stand animation keyframes with MdlVis, and create a new animation based on that.


Click to expand...

First point, from my experiences in the Editor, when I set the Filter Mode to None, the material will show up in all sequences. It only shows up in the sequences I want it to when I'm using Transparent/Additive/Add Alpha modes, and I've never bothered to check Blend and Modulate.


About the 2nd point, you're probably right. I'll look into it.


I'll also try to grab some screens tomorrow.

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

dtnmang said:

 First point, from my experiences in the Editor, when I set the Filter Mode to None, the material will show up in all sequences. It only shows up in the sequences I want it to when I'm using Transparent/Additive/Add Alpha modes, and I've never bothered to check Blend and Modulate.


Click to expand...

That's because you're hidding the plane with the material, use a Geoset Animation to hide the background plane or any geoset.

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

Anvil said:

 That's because you're hidding the plane with the material, use a Geoset Animation to hide the background plane or any geoset.


Click to expand...

Yup. But the point is this tutorial does not involve using Geoset Animations.

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

Edited and added screenies for the tutorial.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

You've put several images, good job.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Great tutorial, very useful! +rep

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Very useful. I personally didn't know the whole technique myself.

Approved.

#### Diegoit

#### Diegoit

 Level 23

JoinedApr 8, 2017Messages1,624

Resources: 11

#### Resources
Models 5Icons 5Maps 1

Outpost model does not work. Link is wrong.

 You must log in or register to reply here.


### Similar threads

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

 Fixing Faulty Death / Decay Animations


General Frank

Sep 30, 2007

3D Modeling Tutorials

Replies21Views19K

Sep 11, 2021
Liu

 Geoset Importing and Exporting Guide


-Peper-

Oct 30, 2010

3D Modeling Tutorials

Replies31Views38K

Apr 14, 2012
-Peper-

 Making a dissipate animation using Magos' War3ModelEditor


Callahan

Apr 15, 2007

3D Modeling Tutorials

Replies19Views22K

Apr 23, 2019
strongwill

 Model Editing


Fabioz

Jan 18, 2009

3D Modeling Tutorials

Replies45Views66K

Oct 10, 2021
KaiZen7th

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