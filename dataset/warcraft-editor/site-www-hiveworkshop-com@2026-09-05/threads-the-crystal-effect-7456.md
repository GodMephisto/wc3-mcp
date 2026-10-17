---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/the-crystal-effect.7456/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# The Crystal Effect
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/the-crystal-effect.7456/>
#### olofmoleman

#### olofmoleman

 Level 41

JoinedDec 5, 2004Messages1,940

Resources: 239

#### Resources
Models 114Icons 106Packs 1Skins 14Maps 1Tutorials 3

In this tutorial I will tell you how to make the crystal effect used in Mc!'s Crystal Golem and my Crystal Dragon

Tools Needed


Warcraft 3 TFT (ROC also possible but I use a TFT ingame skin for this tutorial)


Notepad or Wordpad

War3Fileconverter

Warcraft3 Viewer or/and Magos' model editor

Vertex modifier (or another modeling program)

Knowledge Needed


basic model exporting knowledge


basic mdl editing knowledge


basic modeling knowledge

Getting started


First you need to find a model you want to make in to a crystal one


in this tutorial we'll use the frog model, because it has only one geoset and is a very simple model, but the technique works the same with every other model, it's just more work with more complex models


Note: remember to save your files regularly


First, extract the frog model from the Mpq file.


Then you use the file converter to convert the .mdx frog model in to an .mdl file.

Mdl Editing


Now, open the .mdl file, the first thing we do, is change the path of the skin so it uses the crystal texture.


This is what it looks like

 Code:

Textures 1 {
 Bitmap {
 Image "units\Critters\Frog\FrogSkin.blp",
 }
}


Change it to

 Code:

Textures 1 {
 Bitmap {
 Image "Textures\CrystalOpaque.blp",
 }
}


Now for the model to really look like crystal the textures, materials and geosets have to be copied and make them work propperly, since this model only has one texture, one material and one geoset, thats not going to be that difficult, with more textures, materials and geosets it's just the same, only more copieng work to do.

Copying the textures


Copy this and put it right below it

 Code:

 Bitmap {
 Image "Textures\CrystalOpaque.blp",
 }

Then the texture part should look like this:

 Code:

Textures 1 {
 Bitmap {
 Image "Textures\CrystalOpaque.blp",
 }
 Bitmap {
 Image "Textures\CrystalOpaque.blp",
 }
}

Copying the Materials


it's almost the same with the materials


copy this:

 Code:

 Material {
 Layer {
 FilterMode None,
 static TextureID 0,
 }
 }

and put it below it like this:

 Code:

Materials 1 {
 Material {
 Layer {
 FilterMode None,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode None,
 static TextureID 0,
 }
 }
}

once you've copied it, you need to change a few things in the code, change it to:

 Code:

Materials 1 {
 Material {
 Layer {
 FilterMode None,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode Additive,
 static TextureID 1,
 }
 }
}

as you can see, I only changed the "FilterMode" and the "TextureID" on one material, I left the first material unchanged.

Copying the Geosets


By copying the geosets and changing what kind of material ID it should use you can create the overlaying transparent look that makes it look like crystal.


Since the geoset code is really long I'll only copy a small part in to this tutorial


the geoset begins with this:

 Code:

Geoset {
 Vertices 157 {
 { -12.3964, 1.52395, 0.31411 },
 { -0.52003, 2.88477, 0.33941 },
 { -5.1976, 4.42178, 3.53995 },
 { -13.4577, 4.11941, 0.31442 },
 { -8.1316, 10.2445, 0.33869 },
 { -3.29927, -0.21647, 3.66749 },
 { -5.1976, 4.42178, 3.53995 },
 { -0.57432, 7.25025, 10.4476 },
 { 6.04609, 6.50425, 11.3752 },

and it ends with this:

 Code:

 Matrices { 10, 11 },
 Matrices { 11 },
 Matrices { 12, 13 },
 Matrices { 12, 13, 14 },
 Matrices { 13, 14 },
 Matrices { 14 },
 }
 MinimumExtent { -28.2479, -14.3093, -9.09061 },
 MaximumExtent { 19.7491, 27.7945, 46.2399 },
 BoundsRadius 28.6633,
 Anim {
 MinimumExtent { -28.2479, -14.3093, -3.77587 },
 MaximumExtent { 19.7491, 15.0309, 46.2399 },
 BoundsRadius 26.3345,
 }
 Anim {
 MinimumExtent { -17.6841, -10.6745, -0.0313365 },
 MaximumExtent { 11.7315, 10.2365, 20.4019 },
 BoundsRadius 18.3231,
 }
 Anim {
 MinimumExtent { -19.204, -10.68, -9.09061 },
 MaximumExtent { 12.7842, 27.7945, 32.1506 },
 BoundsRadius 24.6313,
 }
 MaterialID 0,
 SelectionGroup 0,
}

It's a lot of numbers and stuff, but it's all needed, make sure you copy the whole geoset, then paste it directly below it so the first part of the second geoset and last part of the first geoset look like this:

 Code:

 }
 Anim {
 MinimumExtent { -17.6841, -10.6745, -0.0313365 },
 MaximumExtent { 11.7315, 10.2365, 20.4019 },
 BoundsRadius 18.3231,
 }
 Anim {
 MinimumExtent { -19.204, -10.68, -9.09061 },
 MaximumExtent { 12.7842, 27.7945, 32.1506 },
 BoundsRadius 24.6313,
 }
 MaterialID 0,
 SelectionGroup 0,
}
Geoset {
 Vertices 157 {
 { -12.3964, 1.52395, 0.31411 },
 { -0.52003, 2.88477, 0.33941 },
 { -5.1976, 4.42178, 3.53995 },
 { -13.4577, 4.11941, 0.31442 },
 { -8.1316, 10.2445, 0.33869 },
 { -3.29927, -0.21647, 3.66749 },
 { -5.1976, 4.42178, 3.53995 },
 { -0.57432, 7.25025, 10.4476 },
 { 6.04609, 6

Now you go to the very bottom of the second geoset and change the "MaterialID" from 0 to 1. Like this:

 Code:

 }
 MaterialID 1,
 SelectionGroup 0,
}

Now the mdl editing part is done, so if you convert the .mdl file back to .mdx, this is what it should look like:


(this is a screenie take with magos model editor, in the wc3 viewer it looks different)

Model editing in vertex modifier


One of the geosets has to be resized to get that overlapping transparent look.


Since this is a really simple model, this part is about the easiest, but with a complex model like the Crystal Dragon it's much harder.


Open the vertex modifer, and open the .mdl frog model. You'll get a popup window that asks you wich geoset you want to open, because only the second geoset has been modified (the materialID) we are going to open that one


there are two ways to make that crystal effect, you simply make one geoset bigger then the other (like Mc! did with his Golem), or you could make it bigger piece by piece (like I did with my Dragon), this takes much more time, but the result can be more detailed, and less messy, but the frog model it a really simple one, so it won't really matter if we just resize one geoset, once you have resized it a bit, save it and convert it back to .mdx


it should now look like this:


If it does, then you're finished!

Finishing up


Some times you have to tweak the model a little bit to make it look just right


there are also a few things you can do to change the look of the crystal effect a bit


You can brighten the transparent part by making it twosided


this is how you do that in the mdl file


Change this:

 Code:

Materials 1 {
 Material {
 Layer {
 FilterMode None,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode Additive,
 static TextureID 1,
 }
 }
}

To this:

 Code:

Materials 1 {
 Material {
 Layer {
 FilterMode None,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode Additive,
 TwoSided,
 static TextureID 1,
 }
 }
}

In the code I added "TwoSided,", that makes it twosided (duh), but the "T" and the "S" should be spelled with capitals, if you don't do this, it will not work. when you saved the .mdl file and converted it to .mdx you can see the differance if you compare them


Normal


Twosided


the differance isn't that noticable, but with models where the layers overlap each other more you can see a great differance, my Crystal Dragon uses a twosided outside, and Mc!'s Crystal Golem doesn't


Here below you can see the examples


My Crystal Dragon


Mc!'s Crystal Golem


Have fun with making your own crystal creatures!


And special thanks for Mc! for teaching me how to make this effect.


 Last edited: Dec 10, 2006

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Again, nice effect and well written tutorial

However, as much as I would like to apporve this, we have stated some rules which should be observed. I'm afraid you will have to attach the image files to your post, in order for this to be approved.

#### camel's_hump

#### camel's_hump

 Level 12

JoinedApr 28, 2005Messages766

Resources: 4

#### Resources
Skins 4

I approve since there is nothing cooler than a crystal frog Just do as Archian says...

#### olofmoleman

#### olofmoleman

 Level 41

JoinedDec 5, 2004Messages1,940

Resources: 239

#### Resources
Models 114Icons 106Packs 1Skins 14Maps 1Tutorials 3

Done.


Though I can't seem to get the pics to show up as pics instead of links, wich ruins comparison pics

#### camel's_hump

#### camel's_hump

 Level 12

JoinedApr 28, 2005Messages766

Resources: 4

#### Resources
Skins 4

You can keep the pictures where they were in your post, you just have to attach some pictures also. Also I believe there is a way to link stuff in your post to a different part of your post. I don't remember though, and sadly I cannot move this thread, I have to wait for an admin

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

olofmoleman said:

 Done.

Though I can't seem to get the pics to show up as pics instead of links, wich ruins comparison pics


Click to expand...

Thanks

However, you could use the Attachments (post editor) and use the for the images.

Anyway we know of the issue you speak of, and it will be fixed

#### olofmoleman

#### olofmoleman

 Level 41

JoinedDec 5, 2004Messages1,940

Resources: 239

#### Resources
Models 114Icons 106Packs 1Skins 14Maps 1Tutorials 3

There, I fixed all the images now.

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Good


Approved

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

Excellent tutorial, I look forward to making many crystsl creatures  Yay for crystal frogs! Nothing cooler

#### operator

#### operator

 Level 15

JoinedFeb 9, 2006Messages1,598

Resources: 2

#### Resources
Maps 2

nicey nicey

 You must log in or register to reply here.


### Similar threads

 Crystal Effect for models


olofmoleman

Dec 24, 2005

Tutorial Archive

Replies3Views3K

Dec 29, 2006
Proll_Troll

P

 Removing geosets/glow


alfredx_sotn

Apr 21, 2004

3D Modeling Tutorials

Replies4Views30K

Oct 21, 2005
Magos

Locked

 Adding Animated Textures with NOTEPAD ALONE


alfredx_sotn

Apr 30, 2007

3D Modeling Tutorials

Replies0Views16K

Apr 30, 2007
alfredx_sotn

Locked

 Problem removing Glow


Leartes

Dec 10, 2008

Modeling & Animation

Replies3Views2K

Dec 10, 2008
Linaze

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