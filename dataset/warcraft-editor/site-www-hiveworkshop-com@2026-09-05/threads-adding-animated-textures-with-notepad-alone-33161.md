---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/adding-animated-textures-with-notepad-alone.33161/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Adding Animated Textures with Notepad Alone
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/adding-animated-textures-with-notepad-alone.33161/>
#### alfredx_sotn

#### alfredx_sotn

 Level 18

JoinedOct 1, 2003Messages464

Resources: 47

#### Resources
Models 26Icons 15Skins 2Tutorials 4

How it works:


You take or 'rip' a tvertexanim from a model (for instance: Water Elemental).


Then you apply it to a mesh, in which it will be the 'layer' of animated texture.

Tools needed:


Notepad/Wordpad


YobGuls' Fileconverter Get it here


One model with animated texture in which you want to 'rip' from


... and some patience if you want a layer of animated texture which is above the original geoset.

How it can be done:


Convert the 2 models(one with animated texture, one you want to add on) to MDLs. In this tut, we will rip


the tvertexanim from water elemental(Of course, there are different models with different movement of


the texture.) We will call the model you want to add animated texture on as model A, and the other one


as model B, which will be a Water Elemental.


Now open model B.

 Code:

TextureAnims 2 {
 TVertexAnim {
 Translation 2 {
 Bezier,
 GlobalSeqId 1,
 0: { 0.0997751, 1.72412, -0.0260519 },
 InTan { 0, 0, 0 },
 OutTan { 0.0665167, 1.14942, -0.0173681 },
 2767: { 0, 0, 0 },
 InTan { 0.0332584, 0.574708, -0.00868383 },
 OutTan { 0, 0, 0 },
 }
 }
 TVertexAnim {
 Translation 2 {
 Bezier,
 GlobalSeqId 12,
 0: { 0, 0, 0 },
 InTan { 0, 0, 0 },
 OutTan { -0.107482, -0.210476, 1.13849 },
 3333: { -0.322445, -0.631428, 3.41546 },
 InTan { -0.214963, -0.420952, 2.27698 },
 OutTan { 0, 0, 0 },
 }
 }
}

Now look for this, then copy the entire section.


Open model A, then paste this below the materials section

 Code:

 }
 }
 }
 Material {
 Layer {
 FilterMode Blend,
 static TextureID 4,
 }
 }
}
TextureAnims 2 {
 TVertexAnim {
 Translation 2 {
 Bezier,
 GlobalSeqId 1,
 0: { 0.0997751, 1.72412, -0.0260519 },
 InTan { 0, 0, 0 },
 OutTan { 0.0665167, 1.14942, -0.0173681 },
 2767: { 0, 0, 0 },
 InTan { 0.0332584, 0.574708, -0.00868383 },
 OutTan { 0, 0, 0 },
 }
 }
 TVertexAnim {
 Translation 2 {
 Bezier,
 GlobalSeqId 12,
 0: { 0, 0, 0 },
 InTan { 0, 0, 0 },
 OutTan { -0.107482, -0.210476, 1.13849 },
 3333: { -0.322445, -0.631428, 3.41546 },
 InTan { -0.214963, -0.420952, 2.27698 },
 OutTan { 0, 0, 0 },
 }
 }
}

So it looks somewhat like that. Now you will remove one tvertexanim away, so making only


one tvertexanim in the section.

 Code:

TextureAnims 2 {
 TVertexAnim {
 Translation 2 {
 Bezier,
 GlobalSeqId 1,
 0: { 0.0997751, 1.72412, -0.0260519 },
 InTan { 0, 0, 0 },
 OutTan { 0.0665167, 1.14942, -0.0173681 },
 2767: { 0, 0, 0 },
 InTan { 0.0332584, 0.574708, -0.00868383 },
 OutTan { 0, 0, 0 },
 }
 }
}

Now, make a duplicate of the mesh in which you want animated texture on. As for the crypt fiend,


it will be the main body, aka mesh 1 and 2. So CnP the ENTIRE of the 2 geosets and put them as


the last 2 geosets(Just above the set of geosetanims)

 Code:

 MaterialID 3,
 SelectionGroup 0,
} < Press enter here to create a new line and paste omg.
GeosetAnim {
 Alpha 8 {
 DontInterp,
 40267: 1,
 100100: 0,
 113600: 0,
 176667: 1,
 177667: 0,
 178000: 0,
 180000: 0,
 180033: 1,
 }
 GeosetId 0,

Now, create 2 new materials(One for the duplicate of 1st mesh, one for the other.) below the last


material.

 Code:

Materials 4 {
 Material {
 Layer {
 FilterMode None,
 TwoSided,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode None,
 Unshaded,
 static TextureID 1,
 }
 Layer {
 FilterMode Blend,
 TwoSided,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode Transparent,
 static TextureID 2,
 Alpha 7 {
 Linear,
 40267: 0,
 47100: 0,
 48933: 1,
 100267: 1,
 113600: 1,
 171933: 1,
 173600: 0,
 }
 }
 }
 Material {
 Layer {
 FilterMode Blend,
 static TextureID 4,
 }
 }
 Material {
 Layer {
 FilterMode None,
 TwoSided,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode None,
 TwoSided,
 static TextureID 0,
 }
 }
}

I CnP the first material in this case, so I have a template to start with. Now, change the filtermode


to AddAlpha, or Additive. (Since I'm guessing the effect you want should be 'magical' kind.) Then add


the TvertexAnim ID x by CnPing from the Water Elemental's materials, and change the value to 0.

 Code:

 Material {
 Layer {
 FilterMode Additive,
 TwoSided,
 static TextureID 0,
 TVertexAnimId 0,
 }
 }
 Material {
 Layer {
 FilterMode Additive,
 TwoSided,
 static TextureID 0,
 TVertexAnimId 0,
 }
 }

Once this is accomplish, now choose a texture you want. In this example, we will make a fire effect.


The path of such texture is Textures\EQ_Rock2.blp. You can also make new textures yourself, but to save


space, I usually use textures in war3. Give it the wrapwidth and height values, like the water elemental's.

 Code:

Textures 5 {
 Bitmap {
 Image "units\Undead\CryptFiend\CryptFiend.blp",
 }
 Bitmap {
 Image "",
 ReplaceableId 1,
 }
 Bitmap {
 Image "Textures\gutz.blp",
 }
 Bitmap {
 Image "Textures\Dust5A.blp",
 }
 Bitmap {
 Image "ReplaceableTextures\Splats\BurrowSplat.blp",
 }
 Bitmap {
 Image "Textures\EQ_Rock2.blp",
 WrapWidth,
 WrapHeight,
 }
}

Now detect what nth position is the new texture added. It is the 6th texture in this case, so we will


minus one to it, since counting starts from 0 in MDL. Enter 5 to the 2 new materials you made to their textureIDs.

 Code:

 Material {
 Layer {
 FilterMode Additive,
 TwoSided,
 static TextureID 5,
 TVertexAnimId 0,
 }
 }
 Material {
 Layer {
 FilterMode Additive,
 TwoSided,
 static TextureID 5,
 TVertexAnimId 0,
 }
 }

Now create some GlobalSequence by adding a new section. Right after the animation names, and before the textures,


CnP from the water elemental its GlobalSequence section. Then remove all except one. Then replace its value as 3333.


This affects the speed because the longer time you take to travel the same distance = slow speed, shorter time in


same distance = fast speed. I choose 3333 as it is moderate.

 Code:

GlobalSequences 20 {
 Duration 3333,
}
Textures 5 {
 Bitmap {
 Image "units\Undead\CryptFiend\CryptFiend.blp",
 }
 Bitmap {
 Image "",
 ReplaceableId 1,

Now go to the TvertexAnim you had, and update its information to

 Code:

TextureAnims 2 {
 TVertexAnim {
 Translation 2 {
 Bezier,
 GlobalSeqId 0,
 0: { 0.0997751, 1.72412, -0.0260519 },
 InTan { 0, 0, 0 },
 OutTan { 0.0665167, 1.14942, -0.0173681 },
 3333: { 0, 0, 0 },
 InTan { 0.0332584, 0.574708, -0.00868383 },
 OutTan { 0, 0, 0 },
 }
 }
}

Time to update your 2 geosets, change their material IDs to 4 and 5 respectively. (I made 2 materials so you can add


different stuff to each materials next time).


Apply new geosetanims to the 2 new geosets. So they:


1) will not appear on decay


2) will be orange by applying vertex coloring(that would look like fire)


3) are a must for a REAL MDL edited model.


CnP the first 2 geosetanims(Since they refer to the first 2 geosets, and we have 2 copies of those geosets.) and put


them as the last geosetanims. Change their IDs to 4 and 5 respectively. Now CnP

 Code:

 GeosetId 5,

so you can get the 'spaces' which are created by tab right. Then delete that value, leaving the 'spaces' then enter

 Code:

 static Color { 0, 0.5, 0.9 },

This is the vertex coloring, in BGR form. Do that to both of the geosetanims.


So you get

 Code:

GeosetAnim {
 Alpha 8 {
 DontInterp,
 40267: 1,
 100100: 0,
 113600: 0,
 176667: 1,
 177667: 0,
 178000: 0,
 180000: 0,
 180033: 1,
 }
 static Color { 0, 0.5, 0.9 },
 GeosetId 4,
}
GeosetAnim {
 Alpha 8 {
 DontInterp,
 40267: 1,
 100100: 0,
 113600: 0,
 176667: 1,
 177667: 0,
 178000: 0,
 180000: 0,
 180033: 1,
 }
 static Color { 0, 0.5, 0.9 },
 GeosetId 5,
}


You are done! Now for the little addition, you can make the geoset bigger so it appears as a seperate layer above


the original mesh(like Cookie's Fire Infernal). Go to both of the geosets, Add 3 to EVERY coordinates of every vertices.


If the value is negative, make it -3, so they will go further from the origin by 3 units in xyz axis. That will 'enlarge'


the model. It is tedious and time consuming, I know. That is why patience is needed!  I've done this before so many times


lol. So anyways here's an example


From

 Code:

 Vertices 455 {
 { 96.2245, -29.1532, 69.6575 },
 { 89.1451, -22.9421, 74.3438 },
 { 95.8924, -29.1638, 61.5637 },
 { 91.8675, -22.9966, 61.723 },
 { 24.1103, 0.37503, 84.9669 },
 { 24.7768, 0.37503, 94.0652 },
 { 31.6054, 9.09599, 79.764 },
 { 42.4421, 17.8187, 95.3328 },
 { 21.3428, 0.37503, 105.891 },

To

 Code:

 Vertices 455 {
 { 99.2245, -22.1532, 72.6575 },
 { 92.1451, -25.9421, 77.3438 },
 { 98.8924, -32.1638, 64.5637 },
 { 94.8675, -25.9966, 64.723 },
 { 27.1103, 3.37503, 87.9669 },
 { 27.7768, 3.37503, 97.0652 },
 { 34.6054, 12.096, 82.764 },
 { 45.4421, 20.8187, 98.3328 },
 { 24.3428, 3.37503, 108.891 },

...and apply it to every vertices. Now you are truly done!


 Last edited by a moderator: May 2, 2007

Status
 Not open for further replies.


### Similar threads

 Removing geosets/glow


alfredx_sotn

Apr 21, 2004

3D Modeling Tutorials

Replies4Views30K

Oct 21, 2005
Magos

 Adding of Emitters to your models with Notepad/Wordpad


alfredx_sotn

Jul 8, 2004

3D Modeling Tutorials

Replies0Views25K

Jul 8, 2004
alfredx_sotn

 The Crystal Effect


olofmoleman

Dec 9, 2006

3D Modeling Tutorials

Replies9Views33K

Dec 10, 2006
operator

Locked

 Problem removing Glow


Leartes

Dec 10, 2008

Modeling & Animation

Replies3Views2K

Dec 10, 2008
Linaze

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