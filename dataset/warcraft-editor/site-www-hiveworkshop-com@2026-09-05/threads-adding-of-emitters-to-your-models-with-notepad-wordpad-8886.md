---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/adding-of-emitters-to-your-models-with-notepad-wordpad.8886/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Adding of Emitters to Your Models with Notepad Wordpad
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/adding-of-emitters-to-your-models-with-notepad-wordpad.8886/>
#### alfredx_sotn

#### alfredx_sotn

 Level 18

JoinedOct 1, 2003Messages464

Resources: 47

#### Resources
Models 26Icons 15Skins 2Tutorials 4

How it works


You will add emitter to a model all by hand in Wordpad/Notepad. You either take it from a model, or further edit the taken emitter.

Tools needed:


Notepad/Wordpad


Guesst's MDLX Converter Get it here


Any programs that can extract models


We will add emitter to the Crypt Fiend's (Reign of Chaos') left hand taken from the Infernal model.

How it can be done


Now... Extract the Crypt Fiend's model and Infernal model from war3.mpq. Next, open YobGuls' Fileconverter.


Click on Load File. Load up CryptFiend.mdx, then click on Convert to MDL. Do the same to Infernal.mdx.


Now open both of them with Wordpad/Notepad. Now, on the Infernal.mdx, press Ctrl+F and type pivotpoints.


Now scroll down... Until you see this:

 Code:

ParticleEmitter2 "BlizParticleShoulderLeft" {
 ObjectId 103,
 Parent 25, // "Sphere12"
 Unshaded,
 ModelSpace,
 static Speed 60,
 static Variation 0,
 static Latitude 22,
 static Gravity -530,
 Visibility 6 {
 DontInterp,
 33: 1,
 23767: 0,
 24133: 1,
 29033: 1,
 30567: 0,
 34000: 0,
 }
 LifeSpan 0.4,
 static EmissionRate 40,
 static Width 17.4,
 static Length 54.292,
 Additive,
 Rows 8,
 Columns 8,
 Head,
 TailLength 1,
 Time 0.5,
 SegmentColor {
 Color { 0.658824, 1, 0.996078 },
 Color { 0, 1, 0.8 },
 Color { 0, 1, 0.329412 },
 },
 Alpha {255, 255, 0},
 ParticleScaling {24.7, 20, 8},
 LifeSpanUVAnim {0, 40, 1},
 DecayUVAnim {41, 63, 1},
 TailUVAnim {0, 40, 1},
 TailDecayUVAnim {41, 63, 1},
 TextureID 2,
 Rotation 4 {
 Hermite,
 33: { -0.413683, 0.0289275, 0, 0.909961 },
 InTan { -0.593511, 0.0415023, 0, 0.803755 },
 OutTan { -0.593511, 0.0415023, 0, 0.803755 },
 3100: { -0.413683, 0.0289275, 0, 0.909961 },
 InTan { -0.413683, 0.0289275, 0, 0.909961 },
 OutTan { -0.413683, 0.0289275, 0, 0.909961 },
 5467: { -0.413683, 0.0289275, 0, 0.909961 },
 InTan { -0.413683, 0.0289275, 0, 0.909961 },
 OutTan { -0.413683, 0.0289275, 0, 0.909961 },
 30400: { -0.413683, 0.0289275, 0, 0.909961 },
 InTan { -0.413683, 0.0289275, 0, 0.909961 },
 OutTan { -0.413683, 0.0289275, 0, 0.909961 },
 }
}

Then, copy the whole of this portion. Switch over to your CryptFiend.mdx, then Ctrl+F pivotpoints again. Scroll down, then paste it


where the emitter was in the Infernal model. You will get something like

 Code:

 { 0, -22.5742, 155.104 },
 { 0, 0, 0 },
 { 51.7403, 0.293222, 69.0057 },
 { -20.2287, 0.293219, 57.2672 },
}
ParticleEmitter2 "BlizParticleShoulderLeft" {
 ObjectId 103,
 Parent 25, // "Sphere12"
 Unshaded,
 ModelSpace,
 static Speed 60,
 static Variation 0,
 static Latitude 22,
......
 InTan { -0.593511, 0.0415023, 0, 0.803755 },
 OutTan { -0.593511, 0.0415023, 0, 0.803755 },
 3100: { -0.413683, 0.0289275, 0, 0.909961 },
 InTan { -0.413683, 0.0289275, 0, 0.909961 },
 OutTan { -0.413683, 0.0289275, 0, 0.909961 },
 5467: { -0.413683, 0.0289275, 0, 0.909961 },
 InTan { -0.413683, 0.0289275, 0, 0.909961 },
 OutTan { -0.413683, 0.0289275, 0, 0.909961 },
 30400: { -0.413683, 0.0289275, 0, 0.909961 },
 InTan { -0.413683, 0.0289275, 0, 0.909961 },
 OutTan { -0.413683, 0.0289275, 0, 0.909961 },
 }
}

Once that is done, look for a part in the portion you pasted which says

 Code:

......
 TailUVAnim {0, 40, 1},
 TailDecayUVAnim {41, 63, 1},
 TextureID 2, <---- Take note of this part.
 Rotation 4 {
......

Take note of the TextureID. Then on Infernal.mdx, scroll all the way to the top, then Ctrl+F Textures.


You will see this part

 Code:

Textures 5 {
 Bitmap {
 Image "units\Demon\Infernal\Infernal.blp",
 WrapWidth,
 WrapHeight,
 }
 Bitmap {
 Image "Textures\clouds_anim1_bw.blp",
 WrapWidth,
 WrapHeight,
 }
 Bitmap {
 Image "ReplaceableTextures\Weather\Clouds8x8.blp",
 }
 Bitmap {
 Image "Textures\Yellow_Star.blp",
 }
 Bitmap {
 Image "Textures\Clouds8x8Grey.blp",
 }
}

What was the TextureID? 2 right? Alright, now count the textures, starting from 0, until you reach 2. You will get

 Code:

 Bitmap {
 Image "ReplaceableTextures\Weather\Clouds8x8.blp",
 }

Copy it, then go to your CryptFiend.mdx, go to the Textures section, then paste it so it is the last texture.

 Code:

Textures 3 {
 Bitmap {
 Image "Textures\CryptFiend.blp",
 }
 Bitmap {
 Image "",
 ReplaceableId 1,
 }
 Bitmap {
 Image "Textures\gutz.blp",
 }
 Bitmap {
 Image "ReplaceableTextures\Weather\Clouds8x8.blp",
 }
}

Then go back to your emitter you pasted earlier on your CryptFiend.mdx. Now, change the TextureID to the number of texture


you pasted, counting starting from 0. It will be 3 in this case. So change it to 3.

 Code:

......
 DecayUVAnim {41, 63, 1},
 TailUVAnim {0, 40, 1},
 TailDecayUVAnim {41, 63, 1},
 TextureID 3,
 Rotation 4 {
......

Now, remove this portion

 Code:

 Rotation 4 {
 Hermite,
 33: { -0.413683, 0.0289275, 0, 0.909961 },
 InTan { -0.593511, 0.0415023, 0, 0.803755 },
 OutTan { -0.593511, 0.0415023, 0, 0.803755 },
 3100: { -0.413683, 0.0289275, 0, 0.909961 },
 InTan { -0.413683, 0.0289275, 0, 0.909961 },
 OutTan { -0.413683, 0.0289275, 0, 0.909961 },
 5467: { -0.413683, 0.0289275, 0, 0.909961 },
 InTan { -0.413683, 0.0289275, 0, 0.909961 },
 OutTan { -0.413683, 0.0289275, 0, 0.909961 },
 30400: { -0.413683, 0.0289275, 0, 0.909961 },
 InTan { -0.413683, 0.0289275, 0, 0.909961 },
 OutTan { -0.413683, 0.0289275, 0, 0.909961 },
 }

Next, go to the next object after the emitter, which happens to be an event track in this case.

 Code:

EventObject "SPLTDBL0" {
 ObjectId 96, <---- Take note of this number.
 EventTrack 1 {
 31633,
 }
}

Go back to the emitter, look for the part which says ObjectId x(any number), it will be 103 in this case. Change it to 96,


since originally before the event track, it was an attachment point, and it's ObjectId is 95. Every ObjectIds must go in an


order. Then after changing that, change the rest of the ObjectIds for the following objects, all by +1.

 Code:

ParticleEmitter2 "BlizParticleShoulderLeft" {
 ObjectId 96,
 Parent 25, // "Sphere12"
 Unshaded,
 ModelSpace,
 static Speed 60,
 static Variation 0,
 static Latitude 22,
 static Gravity -530,
 Visibility 6 {
 DontInterp,
 33: 1,
 23767: 0,
 24133: 1,
 29033: 1,
 30567: 0,
 34000: 0,
 }
 LifeSpan 0.4,
 static EmissionRate 40,
 static Width 17.4,
 static Length 54.292,
 Additive,
 Rows 8,
 Columns 8,
 Head,
 TailLength 1,
 Time 0.5,
 SegmentColor {
 Color { 0.658824, 1, 0.996078 },
 Color { 0, 1, 0.8 },
 Color { 0, 1, 0.329412 },
 },
 Alpha {255, 255, 0},
 ParticleScaling {24.7, 20, 8},
 LifeSpanUVAnim {0, 40, 1},
 DecayUVAnim {41, 63, 1},
 TailUVAnim {0, 40, 1},
 TailDecayUVAnim {41, 63, 1},
 TextureID 3,
}
EventObject "SPLTDBL0" {
 ObjectId 97,
 EventTrack 1 {
 31633,
 }
}
EventObject "SPLTDBL1" {
 ObjectId 98,
 EventTrack 1 {
 31733,
 }
}
EventObject "SPLTDBL2" {
 ObjectId 99,
 EventTrack 1 {
 31900,
 }
}
EventObject "SPLTDBL3" {
 ObjectId 100,
 EventTrack 1 {
 31967,
 }
}
EventObject "FPTyFKR0" {
 ObjectId 101,
 EventTrack 1 {
 27076,
 }
}
EventObject "FPTxFKR0" {
 ObjectId 102,
 EventTrack 1 {
 27167,
 }
}
EventObject "FPTxFKL0" {
 ObjectId 103,
 EventTrack 1 {
 27030,
 }
}
EventObject "FPTyFKL0" {
 ObjectId 104,
 EventTrack 1 {
 27667,
 }
}
EventObject "SNDxDPIT" {
 ObjectId 105,
 EventTrack 1 {
 30433,
 }
}
EventObject "SNDXFDSL" {
 ObjectId 106,
 EventTrack 1 {
 27533,
 }
}
EventObject "SNDYFDSL" {
 ObjectId 107,
 EventTrack 1 {
 27033,
 }
}
EventObject "SNDXFDSR" {
 ObjectId 108,
 EventTrack 1 {
 27067,
 }
}
EventObject "SNDYFDSR" {
 ObjectId 109,
 EventTrack 1 {
 27467,
 }
}
EventObject "SPNxDBCR" {
 ObjectId 110,
 EventTrack 1 {
 30500,
 }
}
CollisionShape "Collision 2" {
 ObjectId 111,
 Sphere,
 Vertices 1 {
 { 51.7403, 0.293222, 69.0057 },
 }
 BoundsRadius 58.481,
}
CollisionShape "Collision 1" {
 ObjectId 112,
 Sphere,
 Vertices 1 {
 { -20.2287, 0.293219, 57.2672 },
 }
 BoundsRadius 68.4228,
}


We are going to add this to the left hand right? We need to give the emitter a pivot, we have 2 choices, we can either take


the pivot of the left hand attachment point, or the pivot of the bone of the left hand. We will take use the attachment point's


in this tutorial. Now Ctrl+F attachment, then look around there, until you find the attachment for the left hand. Which is

 Code:

Attachment "Hand Left Ref " {
 ObjectId 92, <---- Take note of this.
 Parent 79, // "Bone_Box29" <---- And this. Just the number, ignore the stuff behind the //.
 Visibility 3 {
 DontInterp,
 40267: 1,
 100100: 0,
 113600: 0,
 }
}

Go to the PivotPoints section, then count to the 93rd pivot, since Ids starts from 0, it is actually Id 92. Copy that pivot,


then continue counting until you reach the 96th. Press enter and you will create a new line, then paste there by pressing Ctrl+v,


so it will be on the 97th pivot(our emitter is Id 96, remember?).

 Code:

......
 { -54.7252, 47.7444, 0.151143 },
 { 57.6861, 38.1531, 0.0964531 },
 { -54.4284, -45.1488, 0.115693 },
 { 55.811, -38.8694, -1.23524 },
 { 100.393, -21.7079, 69.1697 },
 { 82.1753, -0.292041, 105.869 },
 { 85.1562, -0.292045, 99.0712 },
 { 103.352, 25.1936, 70.7488 }, <---- Copy this, the 93rd pivot.
 { 47.1684, -0.00997656, 102.803 },
 { -0.896102, 0.600088, 0.0480423 },
 { -0.896099, 0.600088, 146.933 },
 { 103.352, 25.1936, 70.7488 }, <---- Create a new line, paste, and it will be here. 97th pivot
 { 12.9809, -53.5058, 12.9314 },
 { 67.9901, -8.25223, 12.9314 },
 { -1.88774, 0.604691, 12.9314 },
 { 56.9887, -26.7666, 12.9314 },
 { -74.5087, -67.3454, 9.17015 },
......

Once that is done, go back to your emitter, change the parent so that it is the same as the attachment's. It is 79, so change it


to that.

 Code:

ParticleEmitter2 "BlizParticleShoulderLeft" {
 ObjectId 96,
 Parent 79, // "Sphere12"
 Unshaded,
 ModelSpace,
 static Speed 60,
 static Variation 0,
 static Latitude 22,
 static Gravity -530,
......

Finally, change all visibility to 1, since it isn't important now, I will explain it when I have the time to update this tutorial.

 Code:

......
 Visibility 6 {
 DontInterp,
 33: 1,
 23767: 1,
 24133: 1,
 29033: 1,
 30567: 1,
 34000: 1,
 }
......

Save your MDL, then load up YobGuls' Fileconverter, click on Load File, load up your MDL file, then click Convert to MDX.


Import the model into your map and test. There you go, emitter added.

<Attachment Broken>


Finished product.


And this is the Chaos Spider from RoTD, skin by abriko, model by me. It's done by this method, and adding of another 'larger' version of the original geoset(I did them all by hand by adding all coordinates of the vertices by 3), then assign it to a material with animated texture(or tvertexanim).


 Last edited by a moderator: Aug 3, 2014

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

A

Locked

 Adding particle emitters to portraits


Abattoir

May 28, 2007

Modeling & Animation

Replies1Views2K

May 28, 2007
General Frank

M

Locked

 Attachment problems


Magee93

Jan 3, 2010

Requests

Replies2Views2K

Jan 3, 2010
Magee93

M

 Simple Particle Emitter is played four times instead of once


Oriri Ire

Aug 6, 2025

Modeling & Animation

Replies2Views554

Aug 7, 2025
Oriri Ire

[3D Art]
 creating paticle emitiers with notepad only


stonneash

Mar 8, 2012

Tutorial Archive

Replies5Views3K

May 6, 2012
PurgeandFire

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