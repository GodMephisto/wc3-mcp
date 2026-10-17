---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/editing-warcraft-3-trees.205624/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Editing Warcraft 3 Trees
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/editing-warcraft-3-trees.205624/>
#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

 TreeEditing


## Editing Warcraft Trees


Trees are important in every map as destructibles or as decorations. This is the reason why I created this tutorial.

Tools Needed:


1. WC3 Viewer


2. MDLVis

Contents


1. Extracting


2. Editing

### Extracting
1. Open WC3 Viewer. Look for the tree you wanted to edit. Right click it then click extract MDX. This tutorial will use AshenCanopyTree0.mdx found at Warcraft III\War3x.mpq in my computer. The path is Doodads\Ashenvale\Plants\AshenCanopyTree\AshenCanopyTree0.mdx.


2. Do not forget which folder you saved the model.

### Editing
1. Open MDLVis. Open the model which you have saved/extracted. Right Click Perspective, Click Front so that we may take a look at the front view of the model. Press F To view the model in textured view. Take note that the model has two geosets: the one for the tree and the second one for the dead tree version. We will not edit the dead tree version. Note: The number of geoset can be found at the bottom right corner.


2. We are going to create a branch for this tree and some leaves for it. Select all the vertices bounding the trunk. Be careful in selecting vertices, you might select the wrong one. Click Edit then click Copy.We now have the copy of the trunk.Always view the model in textured view.

The edit command.

This image shows the selected vertices of the trunk before you copy it.

This image shows the selected vertices of the trunk after you copy it.


3. In Work Plane click YZ. Click Rotate. Position the cursor at the trunk then move left or right to rotate it. This new trunk will become our branch. Note: All the bounding vertices of the trunk must be selected first before rotating it. The result must be like the one shown below.


4. You will now decide the location of your branch with reference to the trunk of the tree. You can experiment in many ways here. You can rotate, move or resize your branch at your will. If you are done experimenting and is satisfied with the result, It is now time to place the leaves for that branch.

You may want to position the brach like the one shown in the image.


5. Now, we will put leaves into our new branch. We will just select the vertices bounding the leaves. Always be careful in selecting vertices. If you are done selecting the vertices, click Edit then click Copy to create new leaves for our trunk. Always press F after copying any geosets from a model.


6. With the leaves' vertices still selected, carefully position it into the new branch using the move command. Take note that you are always free to experiment in MDLVis.


7. If you are satisfied with the position of the leaves, save it. See the finished new AshenvaleCanopyTree below.

Note From the Author


What is good in editing Warcraft Tree is that we do not need to create animations. Once you are done editing it, you can immidiately save it for use because it already has stand, stand hit, birth, and death animations. If you think that your edit is in good quality, you can upload it in the model section.


#### Attachments

selected trunk vertices copied textured.jpg

 33.6 KB

 · Views: 3,675



selected trunk vertices textured.jpg

 42 KB

 · Views: 3,739



saved model.jpg

 58.5 KB

 · Views: 4,572



rotated new trunk.jpg

 37.3 KB

 · Views: 3,694



positioned branch.jpg

 34.6 KB

 · Views: 3,632



move rotate commands.jpg

 32 KB

 · Views: 470



extracting mdx file.jpg

 50.7 KB

 · Views: 4,002



edit command.jpg

 12.7 KB

 · Views: 3,626



selected new leaves vertices textured.jpg

 41.4 KB

 · Views: 6,579



the new variation of AshenvaleCanopyTreeEdited.jpg

 46.2 KB

 · Views: 3,617




 Last edited: Oct 23, 2011

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Image of step 5 is not working.


Otherwise, great job. I'll approve it once you fix that image quirk.

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

Image fixed.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

~Approved.

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

New modelers can start with trees in modeling. Afterward they can already edit models using those which are already in the Warcraft 3 Archive. I hope many will read this tutorial and many are those which will be helped with this tutorial. Thanks PurgeandFire111 for the approval.

#### Fiend_68

#### Fiend_68

 Level 4

JoinedApr 14, 2012Messages72

I have a problem.Please anyone help me. When I open Warcraft 3 Viewer.

I click Doodads --> Ashenvale --> Plants.

And there are only AshenBush0, AshenCatTail and AshenShrooms.

No trees. What to do ? Please help.

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

Fiend_68 said:

 I have a problem.Please anyone help me. When I open Warcraft 3 Viewer.


I click Doodads --> Ashenvale --> Plants.


And there are only AshenBush0, AshenCatTail and AshenShrooms.


No trees. What to do ? Please help.


Click to expand...


Look for it in Warcraft III\War3x.mpq

#### Fiend_68

#### Fiend_68

 Level 4

JoinedApr 14, 2012Messages72

eubz said:

 Look for it in Warcraft III\War3x.mpq


Click to expand...


War3x.mpq or War3.mpq.


Becouse I don't have War3x.mpq and at MDX I have D:\Games\Warcraft |||\War3.mpq.


And there are no trees in all telisets. Ashenvale, Barrens etc.


Help please.

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

You don't have TFT? You should have one.

#### Fiend_68

#### Fiend_68

 Level 4

JoinedApr 14, 2012Messages72

eubz said:

 You don't have TFT? You should have one.


Click to expand...


I am using Warcraft 3 TFT of course, I play in garena ffs. Just my folder is Warcraft ||| named.

#### snowman990

#### snowman990

 Level 11

JoinedAug 23, 2010Messages663

Resources: 7

#### Resources
Models 1Icons 3Skins 1Maps 2

Maybe adding "x" between "3" and "." Will take you to the war3x.mpq?

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

Fiend_68 said:

 I am using Warcraft 3 TFT of course, I play in garena ffs. Just my folder is Warcraft ||| named.

Click to expand...


I don't know, but I think if you have TFT then, possibly, you also have waqr3x.mpq because war3x.mpq is the mpq for the expansion.

#### JesusHipster

#### JesusHipster

 Level 35

JoinedSep 19, 2011Messages2,119

Resources: 70

#### Resources
Models 62Icons 7Skins 1

Viewer sux, replace it with model editor. It's also needed, when it comes to materials and shit.. And it has a better mpq browser

 You must log in or register to reply here.


### Similar threads

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

 Putting what you learned about Animated Textures into action


Spacebuns

Dec 15, 2015

3D Modeling Tutorials

Replies3Views4K

Jun 29, 2026
Spacebuns

 How to edit models- a complete guide


Horn

Oct 4, 2004

3D Modeling Tutorials

Replies5Views49K

Dec 1, 2013
animation31

[3D Art]
 Creating procedural trees using Blender


Fingolfin

Jan 13, 2019

Tutorial Submission

Replies1Views5K

Apr 29, 2020
MrModify

Locked

 Modeling and Animation 101


donut3.5

Oct 21, 2007

3D Modeling Tutorials
2

Replies70Views72K

May 14, 2015
wcity

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