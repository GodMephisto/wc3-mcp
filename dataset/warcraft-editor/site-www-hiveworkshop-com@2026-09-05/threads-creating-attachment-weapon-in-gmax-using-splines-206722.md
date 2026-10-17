---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-attachment-weapon-in-gmax-using-splines.206722/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating Attachment Weapon in Gmax Using Splines
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-attachment-weapon-in-gmax-using-splines.206722/>
#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

 WeaponsCreation


# Creating Attachment Weapon in Gmax using SPLines

### Introduction

Some modelers rely on creating objects from the standard primitives which made the smoothing not as good as expected. Creating a complex geoset can be created using splines, but I am not really sure if many of us, the modelers, know about this. Here's some kind of starting point in creating geoset using splines. From this, you will be able to create a very superb unit with complex features. Yes, complex thing really starts from simple ones. Here, we will just create a simple attachment that if done nicely will be a very useful model.

### Contents

1. What you need

2. Note

3. The Tutorial

4. Note From the Author

### What You Need

1. GMax

### Note

I will not be covering exporting geosets and texturing in this tutorial. there are tutorials of these kind here.

### The Tutorial

So, let us begin working.

1. On the Create panel > Shapes > Object Type rollout, turn on Line.

2. Using the Illustration below, draw to create it for our blade. Finish creating the drawing to where you have started. A window should pop up telling you to close spline. Click OK.

3. Now, click the perspective view port and maximize it. In the modify panel>Selection turn on spline. Lines should turn red just like the illustration below when you click the drawing/object.

4. Hold shift. Select-hold Y-axis and pull it to the right to copy the selected object/drawing. the result should be like the illustration below.

5.

In the modify panel>Selection turn on vertex

. In the geometry roll out, turn on Create Line.

Turn on 3d snap toggle, right click it, check mark vertex. Connect all vertices with a line. End the command by right clicking the mouse. The result should be like the illustration below.

6.

In the modify panel>Modifier>Patch SPline Editing select surface

. the effect should be like the illustration below; If it isn't, there might be some vertices that isn't connected. In the Path Topology>Steps: type 0. Convert the geoset to editable poly for easy editing.

7. Our weapon doesn't have an edge to cut an enemy so we need to create it. In the Modify Panel>Selection Roll Out turn on vertex

. In the same panel>edit geometry roll out>weld turn on target. Select some verteces to weld so that the resulting figure/object should be like the image shown below.

Note: The resulting geoset might be awkward, so what you will do is to align some vertices which are miss aligned due to the weld command.

8. Now, we will create the handle to finish our work. Draw another object using the illustration shown.

9. Follow step 2 to 6 to create a editable poly from the object drawn. The resulting object with the blade and the handle should be like the illustration below. Sometimes there is a need for you to flip normals if needed. Also, you need to adjust the location of the weapon so that the middle of its handle is exactly positioned at Y=0, x=0, and z=0.

### Note from the Author

You now have your attachment weapon created from splines. If you like this to be converted into the .mdx format and be used in WC3, you will need to read another tutorials. There are many tutorials regarding unwrapping UVs here so that the weapon you have created will have textures. There is also one tutorial on how to export models from gmax. NeoDex is the best exporter for gmax I guess. The model shown below is the textured model of our project. It has no been converted to editable mesh.

#### Attachments

vertex.jpg

 1.3 KB

 · Views: 3,439



3dsnaptoggle.jpg

 1,019 bytes

 · Views: 3,445



Step1.jpg

 14.7 KB

 · Views: 2,347



Step2.jpg

 14.1 KB

 · Views: 2,406



Step3.jpg

 20.3 KB

 · Views: 2,431



Step4.jpg

 25.5 KB

 · Views: 2,349



Step5.jpg

 23.5 KB

 · Views: 2,360



Step6.jpg

 24.6 KB

 · Views: 2,361



Step7.jpg

 26 KB

 · Views: 2,313



Step8.jpg

 21.3 KB

 · Views: 2,319



Step9.jpg

 26.3 KB

 · Views: 2,315



modify panel.jpg

 1.1 KB

 · Views: 3,586



patchSPLine Editing.jpg

 7.3 KB

 · Views: 2,296



Final Model.jpg

 15.8 KB

 · Views: 2,428



spline.jpg

 1.2 KB

 · Views: 1,481




 Last edited: Nov 12, 2011

#### -Grendel

#### -Grendel

 Level 49

JoinedJan 20, 2010Messages2,990

Resources: 2

#### Resources
Skins 2

your images aren't working :<

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

Editing.

#### Xian

#### Xian

 Level 14

JoinedMar 23, 2011Messages1,431

Dude still not working... though useful

i give you rep

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

waiting for the pics to work... ^.^

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

Tutorial updated. Images fixed.

#### Drunken_Jackal

#### Drunken_Jackal

 Level 19

JoinedApr 10, 2010Messages2,789

Resources: 3

#### Resources
Maps 1Spells and Systems 2

Very nice and helpful.

+rep to you.

#### Talavaj

#### Talavaj

 Level 17

JoinedJan 18, 2010Messages1,122

Resources: 6

#### Resources
Models 6

I'm pretty sure anybody working in 3ds max uses lines frequently. But yeah for those who are unfamiliar with them they are pretty useful. Oh and I'm not sure about Gmax, but in 3ds you can drag instead of click and it creates a bezier curve instead of a point.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice tutorial, it is organized well and has nice examples.

~Approved.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Loved this tutorial, using it for 3DS... though I kinda still get bad results in some parts... need to practice more...

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

Thank you, i have memorized it and it has successfully been written into my modeling skills, +rep to you sir

#### Lembidi

#### Lembidi

 Level 8

JoinedJul 30, 2011Messages295

Resources: 23

#### Resources
Icons 21Maps 2

Where i download the free Gmax??

#### Xian

#### Xian

 Level 14

JoinedMar 23, 2011Messages1,431

Lembidi said:

 Where i download the free Gmax??


Click to expand...


Step 1. Go to any search engine (Google,Etc.)


Step 2. Search "Free Gmax"


Step 3. Click the first link


Step 4. ?????


Step 5. PROFIT

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

Lembidi said:

 Where i download the free Gmax??


Click to expand...

here and its totally free of any charge.

#### Lembidi

#### Lembidi

 Level 8

JoinedJul 30, 2011Messages295

Resources: 23

#### Resources
Icons 21Maps 2

THANKS U VERYYYYYY MUCH!!!!!!!!!!!!!!!! eubz_ the greates lord of modeler more than rysnt11!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

#### Xian

#### Xian

 Level 14

JoinedMar 23, 2011Messages1,431

Lembidi said:

 THANKS U VERYYYYYY MUCH!!!!!!!!!!!!!!!! eubz_ the greates lord of modeler more than rysnt11!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


Click to expand...


That is offensive to Rysnt11,please delete the post now

#### Lembidi

#### Lembidi

 Level 8

JoinedJul 30, 2011Messages295

Resources: 23

#### Resources
Icons 21Maps 2

Rysnt11 is my friend Xian Duh!!

#### Tompoolk

#### Tompoolk

 Level 2

JoinedApr 14, 2012Messages17

when i do step 6 and find patch/spline editing i only have 2 options "edit patch" and "delete patch" have i done something wrong?

 You must log in or register to reply here.


### Similar threads

 Creating Doodads in Gmax (simple grass)


eubz

Aug 14, 2011

3D Modeling Tutorials

Replies8Views12K

Mar 7, 2012
stonneash

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

 GMax Exporting Tutorial


AirHead

Aug 22, 2008

3D Modeling Tutorials

Replies10Views14K

Jan 18, 2009
Pomegranate

P

[Tower Defense]
 Spline TD: Multi-level Tower Defense on 3D paths


wiselen

Oct 17, 2025

Idea Factory

Replies6Views971

Nov 21, 2025
Wareditor

 Scratch-Made Weapon Attachments with Particle Emitters


Fabioz

Apr 8, 2008

3D Modeling Tutorials
2

Replies52Views42K

Oct 13, 2010
Ikithe

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