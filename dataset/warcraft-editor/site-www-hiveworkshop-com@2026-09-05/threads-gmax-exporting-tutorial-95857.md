---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/gmax-exporting-tutorial.95857/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Gmax Exporting Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/gmax-exporting-tutorial.95857/>
#### AirHead

#### AirHead

 Level 9

JoinedApr 5, 2008Messages441

Resources: 8

#### Resources
Icons 1Skins 5Maps 1Tutorials 1

 GMax Exporting Tutorial


Hello, and welcome to the GMax exporting tutorial.


Table Of Contents


1.0 –You Will Need


1.1–Create your model


1.2–Precautions


1.3–DEX Exporter


1.4–Copy and Paste


1.5–Setting the Texture


1.6–What Next?

1.0 – You Will Need

GMax (of course)

DEX Exporter

MDLX Converter and/or Yobguls File Converter

WarForger

1.1 – Create your model


Obviously to export a model you must have a model to export so make your 3d model of whatever you want however you want. But make sure to put in bones and add a skin modifier to attach it to your model. The only GMax exporter currently is not able to export bones unless you do not name them. so if you do make bones make sure to not name the bones.


1.2 – Precautions


Now if you have gotten to think you understand what to do just because you have downloaded some programs you will be wondering why its not working. Well first let’s go over some things.


1.Make sure your model is a editable MESH it is fine if it was an editable poly or any other thing before hand but now it must be an editable mesh just select it right click go to Convert to > Editable Mesh.


2.Make sure that you have bones and a skin modifier.


3.Make sure you are not making the mistake of having extra un-needed polygons.


4.Make sure that you have set the unwrap and that your texture is a single file unless you know how to assign more than one file to a model outside of GMax.


5.Make sure not to animate, you will be able to later.


Ok if all of these things are done you are ready to move on.

1.3 – DEX Exporter


Alright, now we’re cooking with gas. Click the hammer icon in the top of the right side menu. Then click the "run script" button.


Then find the DEX Exporter wherever you saved it off to. It should bring up a window in the model name section type in you models name if you want and make sure every part of your model is selected then click "print to listener". It should take a second or two. If it doesn’t then you may have to go over the precautions again. also before continuing go to the bottom of the listener and make sure there is a section near the bottom that has a few blocks of code that say something like

 Code:

Bone "Bone (number)" {
 ObjectId (number),
 Parent (number),
 GeosetId (number),
 GeosetAnimId None,

1.4 – Copy and Paste


Ok now that you have it in the listener minimize GMax create a .txt file and open it bring GMax back up minimize the DEX Exporter and click the “Open Listener” button above the “New Script” button. When the window pops up make sure its long and has lots of long sets of lines of seemingly random numbers. If it does not have these things then you have done something wrong then you will have to go over the precautions again. Now start highlighting, don’t be impatient take your time as there is a glitch that makes you only be able to select roughly less than seventy lines of code and if you mess up here you have to start the copy and paste over again, so be careful.


1.5 – Setting the Texture


Ok if you have again thought you know what to do and went ahead you have probably messed up. First lets get rid of some extra lines of useless code at the top there is a block of code that looks like this

 Code:

"'DeX Version: 0.185m
"
"'DeX Version: 0.185m"
// 'DeX Version: 0.185m


And at the bottom on like this

 Code:

Export Successful!


Remove them both. Now to adding the texture. First what you want to do is think of where your texture will be relative to the model. For example if the model is in a folder called “Models” and the texture is in a folder in the “Models” folder called “Textures” than you should right “Textures/MySkin.blp” so now that we understand that lets move on. There should be a block a code near the top that looks like this.

 Code:

Textures 1 {
 Bitmap {
 Image " ",
 ReplaceableId 1,
 }
}


What you want to do is turn it into something like this.

 Code:

Textures 1 {
 Bitmap {
 Image "MySkin.blp",
 ReplaceableId 1,
 }
}

Or if you put it in a folder.

 Code:

Textures 1 {
 Bitmap {
 Image "Textures/Myskin.blp",
 ReplaceableId 1,
 }
}


NOTE: this will load the “MySkin.blp” file replace that with your skins name.


Above is what mine looked like.


Now you have applied the texture to the model and it should all work except one little detail. This is still a .txt file so save this file off as a .txt file with the name of your model as is name. now click save as again this time use the same name but instead of having it end with .txt change that to .mdl and in the “save as type” box below the name change that to “All Files”.

Yobguls File Converter


Now is where we will be using a File Converter. Open the converter and click the “load file” button find your model and click open, now click the “Convert to MDX”. If it gives an error message ignore it if it doesn’t ignore it, its fine either way, it will convert the file. once its done open your model in Warcraft 3 Viewer and have fun looking at you GMax made model.

MDLX Converter


Now is where we will be using a File Converter. Open the converter and click the convert button find your model and click open. If it gives an error message ignore it if it doesn’t ignore it, its fine either way, it will convert the file. once its done open your model in Warcraft 3 Viewer and have fun looking at you GMax made model.

1.6 What Next?


ok, now open your model in Magos' model editor and make some particles if you want, but, make sure that before you use it in Warcraft III open it in WarForger and make the animations and set the and rarity of the animations.

Animation


remember not to do this before you make particles because either Magos' Model Editor cannot load .MDX files correctly or WarForger saves them wrong, they can both be used in WCIII but Magos' model editor cannot open Warforger stuff (although Warforger can open Magos' Model Editor) ok, so now to do this open up WarForger, open you're model then open the 'Model' tab on the top then 'Sequences' now pick the stand 2 3 and more if you have them un-check looping on all of them then set the rarity to more than 0 for all of the stands except his main stand or main stand alternate. now go to death, morph, birth (all of the alternate versions of the same as well) and dissipate if you have it and un-check looping but don't set the rarity, and now you animate, this is however not a animation tutorial, you might have to look elsewhere for that. and then you are done save as a .MDX


 Last edited by a moderator: Dec 26, 2008

#### AirHead

#### AirHead

 Level 9

JoinedApr 5, 2008Messages441

Resources: 8

#### Resources
Icons 1Skins 5Maps 1Tutorials 1

EDIT: fixed the tutorial upon the discovery that you can animate and rig in GMax


EDIT 2: fixed the tutorial upon the discovery that you cannot animate in GMax but can rig the model so that it can be animated in Warforger.

#### Attachments

Gmaxeg..jpg

 165.1 KB

 · Views: 963



Gmax2.jpg

 108.9 KB

 · Views: 760




 Last edited by a moderator: Dec 26, 2008

#### Verdun

#### Verdun

 Level 16

JoinedJun 9, 2008Messages350

Resources: 11

#### Resources
Models 11

##
 Shortlegs


so basically there is NO way to export your model from gmax with animations you made there? Since well I GUESS I could figure out how to use gmax for animation but Warforge looks even more complicated...

#### AirHead

#### AirHead

 Level 9

JoinedApr 5, 2008Messages441

Resources: 8

#### Resources
Icons 1Skins 5Maps 1Tutorials 1

Trust me Warforger is less complicated than GMax, but as a result of that much less useful. It's fine as long as you can rig the model, nothing is wrong with Warforger animations. Don't bother with GMax animations, go right to Warforger.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Gmax is a lot easier for animation than Warforger, the problem comes if you don't know how to animate properly, or even making a good rigging.

#### 3rgytr54

#### 3rgytr54

 Level 11

JoinedApr 6, 2008Messages562

Resources: 12

#### Resources
Models 10Packs 1Tutorials 1

Gmax animation is easy. (i don't know how easy warforger is) But, the dex script can export animation- I do it all the time (with minor complications that are my fault but that's usually besides the point)

#### Verdun

#### Verdun

 Level 16

JoinedJun 9, 2008Messages350

Resources: 11

#### Resources
Models 11

##
 Shortlegs


Ok, now I COULD ask where can I learn to animate properly with gmax since I already tried a few tutorials without sucess, but I'm not sure that's apropriate right now, so I'd have another question...


quote:


"Make sure that you have set the unwrap and that your texture is a single file unless you know how to assign more than one file to a model outside of GMax."


so gmax can't export a model in a way that it is able to use 2 textures (for two different parts of the model), right? If anyone knows where you have to go so you can assign a second texture afterwards, please tell me (Milkshape?).

#### Hawkwing

#### Hawkwing

 Level 34

JoinedNov 28, 2006Messages5,269

Resources: 29

#### Resources
Models 16Icons 3Packs 1Skins 8Tutorials 1

Try using "
" tags and "[goto]" tags.

#### Verdun

#### Verdun

 Level 16

JoinedJun 9, 2008Messages350

Resources: 11

#### Resources
Models 11

##
 Shortlegs


and what's with references? do they have to stay unnamed, too?


 Last edited: Dec 2, 2008

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Approved.

#### Pomegranate

P

#### Pomegranate

i'm having a little trouble using this method to export my model. whenever i view it in game its just teamcolor and nothing else... here's what i mean:


should look like:


but actually looks like:


the mdl materials/textures blocks look like this:

 Code:

Textures 1 {
 Bitmap {
 Image "planet1.blp",
 ReplaceableId 1,
 }
}
Materials 1 {
 Material {
 Layer {
 FilterMode None,
 static TextureID 0,
 }
 }
}


and my texture file is a 32bit targa with everything selected as the alpha layer. it looks like this:


any help is much appreciated. i already made a thread about this but i can't seem to find it. it's not shown where it should be...

 You must log in or register to reply here.


### Similar threads

 Creating Doodads in Gmax (simple grass)


eubz

Aug 14, 2011

3D Modeling Tutorials

Replies8Views12K

Mar 7, 2012
stonneash

 Geoset Importing and Exporting Guide


-Peper-

Oct 30, 2010

3D Modeling Tutorials

Replies31Views38K

Apr 14, 2012
-Peper-

Locked

 How To Make One Model Use Many Skins


oz02

Jun 4, 2004

3D Modeling Tutorials

Replies3Views64K

Sep 19, 2007
Wolverabid

Locked

 GMax Moddeling


Blackhand

Jun 20, 2006

Modeling & Animation

Replies1Views1K

Jun 20, 2006
Kimbo

Locked

 Gmax


-=Emergenzy=-

Nov 29, 2004

Modeling & Animation

Replies7Views2K

Nov 30, 2004
-=Emergenzy=-

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