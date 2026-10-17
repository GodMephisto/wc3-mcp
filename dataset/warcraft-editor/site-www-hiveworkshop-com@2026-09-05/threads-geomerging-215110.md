---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/geomerging.215110/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Geomerging
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/geomerging.215110/>
#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

 modeling by geomerging


# Geomerging
eubz
I would like to share to everyone, specially the new modelers, this technique in editing warcraft 3 models. This is basically just merging geosets from one model to the other. Simple yet needs patience and hard work to achieve the best result specially on editing the model using MDLVis. I hope many of those who read this will gain more knowledge about simple model editing.

## Tools Needed

1. MDLVis

2. Magos Model Editor

After knowing the tools, let's get started.

## Extracting a Model, Exporting and Importing a Geoset

1. Open War3 Model Editor. Go to Windows > MPQ Browser. Click file, then click war3.mpq. In the folder named "Units", click on Creeps and look for Centaur. Open that folder and double click "Centaur.mdx". The Centaur model will appear. Right click on the "Centaur.mdx" file, then click export and save the model to your specified folder.

2. At this point we will be exporting a geoset. We will be using the portrait model of a Furbolg. Open the MPQ Browser, go to Units > Creeps > Furbolg. Double click the Furbolg_Portrait.mdx. There appears the model. Use the mouse roller to move closer to the model.

3. Click windows > Geoset Manager. The Geoset Manager Window will appear. Move it so you can see the model better. Hold Ctrl then click the geoset that correspond to the head of the Furblog. Look at the Geoset manager. The highlighted geoset there is the heads geoset; Right-clik it then click "export". You can rename the geoset to "FurbolgHead.geo". Please don't forget to type the extension .geo if you will rename the geoset (It is also recommended to rename the geoset for easy finding). Now, save the geoset to a your folder. The folder should also be easy to find.

4. Now that we have our geoset saved, we will now open the Centaur model so we can merge the Furbolg's head there. Open your exported Centaur model.

5. Click Window > Geoset Manager. Click the last geoset (Just to make sure that the import will be placed below it and you will not be selecting all the geosets just to find the newly exported one. This works fine on me.) Right-click it, then click import. Now, look for the geoset that you have just saved lately. Once found, click on it and click open. A Window will appear asking you which node you will attach the new geoset. Look for the bone named "Head" and click it, a new geoset has been imported. Now, it seems like nothing has been imported as nothing appears in the Editor. This is because the new geoset doesn't have any material. Before we assign any material to our new geoset, we will first use an image/texture. In this case we will be using the image/texture of a Furbolg since it is that unit's geoset that we have exported.

6. Use the MPQ Browser. Windows > MPQ Browser > File > War3.mpq > Units > Creeps > Furbolg > then, right-click on Furbolg.blp and click "Use as Texture". NOTE: If War3.mpq is already open, you will just need to find the Furbolg folder in Units > Creeps.

7. Now that we have used the Furbolg's texture, close the MPQ Browser. Go to Windows > Material Manager. We will now create a new material for our geoset.

8. Click on the last Material. Right-click and click "Create New". A Material window will appear. Right-click inside that window then click "Create New", a Material Layer Window will appear. On the Texture ID panel, click on the button to select the Furbolg Texture. If done click OK. A new Material Layer will appear in the Material window. Click OK. Now, we have the material to be assigned to our new geoset. We are pretty sure that the new material is located just below the highlighted material in the Material Manager. To be sure, open all those Materials by double-clicking them and see if the used texture is the Furbolg's texture.

9. For the new geoset to appear, we need to assign a material into it. Go to windows > Geoset Manager and click the last geoset therein. Its Material ID should be (None), if not, that is not the new geoset. Click all the geosets and find the one that has no Material ID. If found, Click on the Material ID and select the last Material. If that Material is not the one that has the Furbolg texture, you will have to use all the materials to find the right one. If done, click OK, then close the Geoset Manager. The Furbolg portrait should appear if done correctly. See the image below. (NOTE: Save the model as an mdl file for easy renaming in note pad.)

10. As you can see, the new geoset is not aligned with the main model, it is far. The biggest part is the geoset editing in MDLVis.

## Editing the Model in MDLVis

1. Open MDLVis, Open the model of the centaur with the new geoset.

2. Take note that our new geoset is bigger than the main model and that it is situated at the right side. The new geoset is the sixth one in the list of geosets found at the bottom-right of MDLVis. click it. Click anywhere inside the MDLVis window, hit the space bar then hit "F". You are now in the Fron view port. Box-select all the vertices of the Furbolg. Make sure that the Workplane is checked and the XY coordinates in the workplane is marked. hit "M" on your keyboard, drag/move the furbolg geoset to the Centaur model.

3. We still need to scale the new geoset to fit with the main model. Hit "Z" the re-size it.

4. Next thing to do is to delete the geosets which are not needed. Delete some by selecting vertices that you think not needed and pressing delete in your keyboard. Also, you have to delete the head of the Centaur. If you are satisfied with the result, save the model.

5. There is also a need for you to align some geosets and vertices. Always look at all angles in working with this.

6. Lastly, go to Modules > Sequence Editor > Bones. In Objects, look for "Bone_Head". Select it if found. In the Geosets panel, click check-mark geoset 6. Zoom into the head. All vertices of the head should all be black, if not select those vertices. Note that when selected, a vertex turns red. All head's vertices should be selected. In the Tools panel, click Reattach Vertices.

## Saving and Testing

1. Save the model as mdl file.

2. Open it in note pad.

3. Rename it into what ever name you like.

4. Save the file.

5. Open it in Magos model editor to check for the animation if the head is correctly attached to the Head's bone.

6. Save it and use .mdx extension in saving.

7. Always test your model in game for finalization.

#### Attachments

Import Geoset Window.jpg

 14.9 KB

 · Views: 3,771



material manager.jpg

 17.3 KB

 · Views: 3,684



Material Window.jpg

 28.8 KB

 · Views: 3,577



final model.jpg

 33.4 KB

 · Views: 3,844



material layer window.jpg

 37.1 KB

 · Views: 3,644



expoted geoset with material.jpg

 41.4 KB

 · Views: 3,790



Material Layer.jpg

 42.6 KB

 · Views: 615



geoset data.jpg

 45.3 KB

 · Views: 3,606



attached to the bone.jpg

 62 KB

 · Views: 3,608



MDLVis new geoset selected.jpg

 69.4 KB

 · Views: 5,065



Centaur_Edited.mdx

 161.6 KB

 · Views: 589




 Last edited: Apr 23, 2012

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

I can't help but post new tutorials for the beginners.

#### kent

#### kent

 Level 4

JoinedJul 15, 2009Messages72

Really nice. Makes me wonder if you could change that "axe" for a paladin maze. That would be fun

I like the kind of tutorials you're making! +rep

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Furbolg Centaur!!!

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

kent said:

 Really nice. Makes me wonder if you could change that "axe" for a paladin maze. That would be fun


I like the kind of tutorials you're making! +rep


Click to expand...


Do you know that in modeling, your imagination is the limit. So for the axe to paladin maze, you can also do that by following this tutorial.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great work, detailed and explained. A lot of people look for tutorials like this for simple yet effective model edits, and this is explained in a fool-proof way.

~Approved.

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

Thanks for the approval PurgeandFire111

#### Fire Eagle

#### Fire Eagle

 Level 6

JoinedJun 10, 2012Messages240

Good tutorial eubz. Could someone give me the link of MDLVis?


 Last edited: Jun 12, 2012

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

here

http://www.google.com.ph/search?q=m...s=org.mozilla:en-US:official&client=firefox-a

#### Fire Eagle

#### Fire Eagle

 Level 6

JoinedJun 10, 2012Messages240

Thanks

#### clockwork2

#### clockwork2

 Level 15

JoinedJan 26, 2010Messages1,629

Resources: 4

#### Resources
Models 2Maps 2

Ya know, I always wondered what a furbotaur would look like.

#### Engkanto

#### Engkanto

 Level 7

JoinedAug 13, 2011Messages142

Resources: 2

#### Resources
Models 1Maps 1

how do you add hero glows?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

XxDizzyxX said:

 how do you add hero glows?


Click to expand...

https://www.hiveworkshop.com/forums...78/how-add-hero-glow-without-modeling-146416/


That is one way.

#### Engkanto

#### Engkanto

 Level 7

JoinedAug 13, 2011Messages142

Resources: 2

#### Resources
Models 1Maps 1

thanks

#### Karkul

#### Karkul

 Level 5

JoinedJul 24, 2009Messages70

Resources: 3

#### Resources
Models 1Maps 2

I was wondering when I try to make a murloc head to a marine, the head stay in the model legs. How to split them both so I can make the edit more easily?

#### MyPad

#### MyPad

 Level 31

JoinedMay 9, 2014Messages1,878

Resources: 27

#### Resources
Models 1Icons 4Maps 6Spells and Systems 7JASS 7Lua 1Tutorials 1

I would like to say thanks to @eubz for this tutorial. This helped me greatly in making some sort of hybrid creature.

Karkul said:

 I was wondering when I try to make a murloc head to a marine, the head stay in the model legs. How to split them both so I can make the edit more easily?


Click to expand...

Are you using the MDLVis program? You could filter out which geoset you would like to manipulate if you look at the bottom right of the screen. Also, I would like to thank you for bumping this thread, making it timely! Otherwise, I would have not found it.

#### Karkul

#### Karkul

 Level 5

JoinedJul 24, 2009Messages70

Resources: 3

#### Resources
Models 1Maps 2

No problem and I use MDLVis yes. I'm just unexperienced.

#### SNIper of DARKness

#### SNIper of DARKness

 Level 13

JoinedApr 9, 2013Messages764

Resources: 1

#### Resources
Tutorials 1

How would you go about geomerging custom models or whatever is the proper term for attaching a custom Helmet Attachment to a Custom Model?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

SNIper of DARKness said:

 How would you go about geomerging custom models or whatever is the proper term for attaching a custom Helmet Attachment to a Custom Model?


Click to expand...

Depends on the model. For some attachments, you can just attach them using triggers or an item.

But if you actually need to take the entire geoset and attach it (just like this tutorial), then you'll basically follow the same process, except: you can skip Step 1, in Step 2 you'll go to File > Open and choose your custom attachment model, and then in Step 6 you'll import whatever textures are needed for your helmet instead. Apart from that, the process should be pretty similar.

If you still have trouble getting it working, you should make a post in the Modeling Forum and attach the two models you're trying to merge. I'm sure someone will be happy to help!

#### Amigoltu

#### Amigoltu

 Level 11

JoinedDec 21, 2012Messages379

SNIper of DARKness said:

 How would you go about geomerging custom models or whatever is the proper term for attaching a custom Helmet Attachment to a Custom Model?


Click to expand...

Even more simple than this. Open the helmet in Mdlvis, select the entire helmet (all of it's vertices, and make sure all the geosets of the helmet are ticked before that) After that, go to Edit - Copy. Next, open your "Custom Model", select ONE vertice on it's head, go to Edit - paste. After that, just move the helmet around a bit, so it would fit visually. If you want to be sure, go to Sequence Manager and see if the helmet looks good in animations (if it moves properly with the model's head).

 You must log in or register to reply here.


### Similar threads

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

 Fixing Faulty Death / Decay Animations


General Frank

Sep 30, 2007

3D Modeling Tutorials

Replies21Views19K

Sep 11, 2021
Liu

 Putting what you learned about Animated Textures into action


Spacebuns

Dec 15, 2015

3D Modeling Tutorials

Replies3Views4K

Jun 29, 2026
Spacebuns

 The Crystal Effect


olofmoleman

Dec 9, 2006

3D Modeling Tutorials

Replies9Views33K

Dec 10, 2006
operator

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