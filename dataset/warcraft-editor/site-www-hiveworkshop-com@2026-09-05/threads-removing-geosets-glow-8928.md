---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/removing-geosets-glow.8928/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Removing Geosets Glow
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/removing-geosets-glow.8928/>
#### alfredx_sotn

#### alfredx_sotn

 Level 18

JoinedOct 1, 2003Messages464

Resources: 47

#### Resources
Models 26Icons 15Skins 2Tutorials 4

How it works


Removing of them is pretty easy, it doesn't requires much work. It can be done solely in Notepad/Wordpad.


Let's get started.

Tools needed:


Notepad/Wordpad


YobGuls' Fileconverter Get it here


Any programs that can extract models

How it can be done


First, get the model you would like to remove its Team Color or Glow. I will explain on how to remove Team Coloe first. Open YobGuls' Fileconverter, then click on Load file, load up the .MDX of the model you extracted.


Then, click on convert to .MDL.


Open the .MDL file with Notepad/Wordpad.


Ctrl+F Textures.

 Code:

Textures 3 {
 Bitmap {
 Image "Textures\Peasant.blp",
 }
 Bitmap {
 Image "",
 ReplaceableId 1,
 }
 Bitmap {
 Image "Textures\gutz.blp",
 }
}

You will see this part. This is taken from the peasant model. See the

 Code:

 Bitmap {
 Image "",
 ReplaceableId 1,
 }

?


ReplaceableID 1, means Team Color, 2 means Team Glow.


Count the number of textures, until you reach this one, starting from 0. In this model, it's 1. (So if it's the 10th texture in the Textures section, it would be 9)


Then Ctrl+F TextureID x


Where x = the number you got just now.

 Code:

Materials 3 {
 Material {
 Layer {
 FilterMode Transparent,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode Transparent,
 Unshaded,
 TwoSided,
 static TextureID 1,
 }
 Layer {
 FilterMode Blend,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode Transparent,
 static TextureID 2,
 Alpha 6 {
 Linear,
 46533: 0.748443,
 52900: 1,
 98133: 1,
 99800: 1,
 155733: 1,
 159800: 0.75,
 }
 }
 }
}

You will see this portion,

 Code:

 Material {
 Layer {
 FilterMode Transparent,
 Unshaded,
 TwoSided,
 static TextureID 1,
 }
 Layer {
 FilterMode Blend,
 static TextureID 0,
 }
 }

Like I said, for this model, the number is 1.


Copy the whole line, including the spaces in front, then make a new line below it, then delete everything except for the 'static ' and the spaces in front, then type 'Alpha 0'.


You will then see this:

 Code:

 Material {
 Layer {
 FilterMode Transparent,
 Unshaded,
 TwoSided,
 static TextureID 1,
 static Alpha 0,
 }
 Layer {
 FilterMode Blend,
 static TextureID 0,
 }
 }

When that's done, save your .MDL file.


Then open your YobGuls' Fileconverter(You need to close the previous one and open again for some reasons.)


Then again, click on Load File, make sure the extension for the file is .MDL, then load it up.


Convert it back to .MDX file, then all is done, Team Color removed!!! YAY!!!


For Team Glows, do the same. Except that it is ReplaceableID 2, not ReplaceableID 1.


If you have any further questions regarding this, post it here and I will answer them.


 Last edited by a moderator: Mar 12, 2007

#### Horn

#### Horn

 Level 15

JoinedMar 31, 2004Messages860

Resources: 20

#### Resources
Models 14Icons 1Skins 3Tutorials 2

Firstly, load up your model in the Warcraft Image Viewer II. Keep it at animation 'none' so that you can see the team coloured glow.


Next, you need to find which meshes are glows, by loading the tree view in viewer. Deselect the meshes (the tick boxes) one at a time until you find which meshes are glows. If a glow disappears when you uncheck a box, that mesh is obviously a glow. Make a note of the mesh number, or numbers (where heroes have glowing weapons too).


Now convert your mdx into mdl. You will need to find the geosets (meshes) that you hve noted down. Remember to count from 0, so mesh 2, would be the second mesh, 1. (from 0, 1).


The easy way to do this is search (Control+F) for Geoset, and press next until you get to the required Geoset.


Once you locate the right Geoset, select the whole Geoset, from before 'Geoset' until the end of the Geoset; the last two lines are:

 Code:

 MaterialID 0,
 SelectionGroup 0,
}


^select after the bracket


Then replace this with a blank geoset (attached).

optional


You can then, if you dare, delete the glow texture and material, but this is not neccesary as you have to ensure to renumber the textures and materials after the deleted, and renumber all the geosets and emitters that may use textures/materials after the deleted ID.


This should achieve the same results, but does 'blank' the glow geoset, and reduces file size a bit.


This replacing of geosets can be used on any geoset you wish to 'remove', such as the morph form of a Demon Hunter, to considerably reduce filesize. You cannot simply delete a geoset, as you model will not work.


Lastly, convert back to mdx, and cross your fingers as you test.


Empty geoset code:

http://www.wc3campaigns.net/attachment.php?attachmentid=34579


 Last edited by a moderator: Mar 12, 2007

#### Horn

#### Horn

 Level 15

JoinedMar 31, 2004Messages860

Resources: 20

#### Resources
Models 14Icons 1Skins 3Tutorials 2

EventObjects - Birth, footprint, blood etc.

This tutorial assumes you can convert an MDX to MDL and have read my Geosetanim tutorial.


Open an MDL, and search for EventObject. They look a bit like this:

 Code:

EventObject "SPNxUBAC" { [b]<Code represents a model[/b]
 ObjectId 58, [b]<--ordered Object ID, must be maintained[/b]
 EventTrack 1 { [b]<--number of sequences to appear on[/b]
 22667, [b]<--start time of a sequence that model appears[/b]
 }
}


You can add EventObjects by copying this example, and editing the Modelcode, the ID and the eventTrack info (see my Geosetanim tutorial for info on sequence times).


The model code and eventtrack times can easily be changed.


Attached is a list (taken from the Blizzard Art tools) of all ObjectEvent codes.

http://www.wc3campaigns.net/attachment.php?attachmentid=33983


 Last edited by a moderator: Mar 12, 2007

#### Mecheon

#### Mecheon

 Level 25

JoinedMar 31, 2004Messages4,465

Resources: 13

#### Resources
Models 12Ported Models 1

How To Properly Remove Geosets

Hello there people. How is your existance going? Today I share with you one of the greatest secrets of Warcraft modding*


How to "remove" geosets. This doesn't remove them, but it does replace them with a single face so the game won't crash on you


Here it is. Copy and paste into a text document, and find a geoset you want rid of and paste it in. I guarantee this works when done right

 Code:

Geoset {
 Vertices 3 {
 { -30.735449, -1.698751, 0.000000 },
 { -30.215448, -1.998751, 0.000000 },
 { -30.175447, -1.318751, 0.000000 },
 }
 Normals 3 {
 { 0.000000, 0.000000, 1.000000 },
 { 0.000000, 0.000000, 1.000000 },
 { 0.000000, 0.000000, 1.000000 },
 }
 TVertices 3 {
 { 0.000000, 0.000000 },
 { 0.000000, 0.000000 },
 { 0.000000, 0.000000 },
 }
 VertexGroup {
 0,
 0,
 0,
 }
 Faces 1 3 {
 Triangles {
 { 0, 1, 2 },
 }
 }

 Groups 1 1 {
 Matrices { 0 },
 }
 MinimumExtent { -200, -200, -200 },
 MaximumExtent { 200, 200, 200 },
 BoundsRadius 400,
 Anim {
 MinimumExtent { -200, -200, -200 },
 MaximumExtent { 200, 200, 200 },
 BoundsRadius 400,
 }
 MaterialID 0,
 SelectionGroup 0,
}


* This is not actually the greatest secret of Warcraft modding

#### Magos

#### Magos

 Level 13

JoinedMar 25, 2004Messages28

Resources: 1

#### Resources
Tools 1

You can easily remove geosets in my model editor (see sig). CTRL-click on a part and it will be selected, then select Remove. All ID's will be automatically rearranged.


NOTE: If there are geoset animations refering to the geoset you remove remember to remove these as well. Warcraft 3 doesn't like geoset animations that refers to "none".

 You must log in or register to reply here.


### Similar threads

Locked

 Problem removing Glow


Leartes

Dec 10, 2008

Modeling & Animation

Replies3Views2K

Dec 10, 2008
Linaze

Locked

 Adding Animated Textures with NOTEPAD ALONE


alfredx_sotn

Apr 30, 2007

3D Modeling Tutorials

Replies0Views16K

Apr 30, 2007
alfredx_sotn

P

Locked

 Skin with Team Color Problem


Panguino

Aug 6, 2006

Texturing & Surfacing

Replies0Views1K

Aug 6, 2006
Panguino

P

 The Crystal Effect


olofmoleman

Dec 9, 2006

3D Modeling Tutorials

Replies9Views33K

Dec 10, 2006
operator

 Creating Ghosts


Daelin

Jul 14, 2005

3D Modeling Tutorials

Replies10Views27K

Aug 14, 2023
DREADMAN

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