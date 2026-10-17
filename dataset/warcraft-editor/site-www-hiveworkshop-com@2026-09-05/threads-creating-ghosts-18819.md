---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-ghosts.18819/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating Ghosts
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-ghosts.18819/>
#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Time for an interesting MDL tutorial, which some of you might enjoy. The subject is of course, ghosts. Didn’t you ever wonder how was Antonidas ghostly model made, or even want to do your own ghostly model? Then here’s your chance to find out how. Moreover, this tutorial doesn’t cover only this area. It also teaches those who have only ROC how to import custom models into their map.

1. Tools Needed


Well, all the tools you need can be found on this site. And they are not many at all:


a) Warcraft 3 Viewer – You will need it so that you can extract the model.


b) Warcraft3FileConverter – You will need to convert the extracted model from MDX to MDL and then vice-versa, so that you can use the model in warcraft.


c) WinMPQ – Optional for those who don’t have TFT, and so, they don’t have Import Manager.


Of course, you will need Warcraft 3, even if it is TFT or ROC. I hope that Warcraft 3 Viewer works for ROC too.

2. Getting the Model


Open Warcraft 3 Viewer. Go to File – Open MPQ and browse for your warcraft 3 directory. There, search for either war3.mpq or war3x.mpq (second one only for Frozen Throne). This is where all skins and models are stored. Right now, to make this tutorial common, I decided to take a ROC model so select war3.mpq.


Now, in the treeview you will see three radio buttons: MDX, BLP and local drives. Well, MDX are models, BLP are skins, and local drives are either of them, but outside an MPQ archive and situated on your harddisk, on a disk or on a CD/DVD. Right now, we are searching a model inside the archive so select MDX.


We are going to work on the Black Dragon model to make it ghostly. The path for this model is Units\Creeps\BlackDragon\BlackDragon.mdx. If you have followed the steps correctly, a black dragon will appear. Click on the Export MDX button (see picture below) and save it wherever you want, but remember where, since we are going to work with it.

3. Converting from MDX to MDL


Close Warcraft 3 Viewer since we are done with it. Now open Warcraft 3 File Converter. Now, this one is much simpler. Click on the Load File button, search for the Black Dragon mdx model you just exported and double-click on it. Its file type is MDX but the program since to have a bug, so you might have to switch to MDL and then back to MDX so that it recognises MDX files. Once it is loaded, click on the “Convert to MDL” model and if it says that the model was saved successfully, you’ve done it. If not, you’ll have to try again. The second file will be saved just where the first was. We now have our MDL model. Close the exporter since we will open it later, once we are done with the MDL editing.

4. Editing the MDL


This might be a hard part for most of you, since model editing is not an easy stuff. Thank god that you have to change only a couple of lines (maximum), nothing more. How to open the MDL model? With notepad of course. What’s cool about MDL models is that they are understandable text files, and not coded like executables or even MDXs. MDX is a coded type, MDL is not.


So, once you opened the file, some of you might feel really confused, since they won’t understand all the blabla inside it. No worries, I will do my best to guide you in a general way, so that you can make your ghostly model. But first of all, I have to explain the basic concepts of a model.


Models are made out of more than one part. They have their geometry (the shape of the model), animations (movements of the model, such as “Move”, “Attack – 1”, “Stand” and others), Skin (which is the thing that covers the model since without it the model would be only a white stuff, with its correct shape its true but white), and others. Well, I’m not sure if I’m correct here, but the skin is splitted into two parts: UVMapping, Textures and Material. We are interested in the material now, since that’s where we can make the model ghostly. UV just tells you where comes each part of the model on a plane, so that you can skin it. If you have seen a skin, you will notice that it looks pretty strange, and that its not actually the front and the back of the model. Only half of the face may appears, since on the UVMapping the other half was mirrored and placed exactly over the first half. Anyway, these are not stuff that should concern you. As for the Textures, this is where the modeler mentions where goes each skin. We are going to be slightly interested here, since we need to know which materials we are supposed to make ghostly.


So, let’s have a look at the Textures. Textures appear in all MDL files (converted this way atleast) after Sequences (which are after the first “paragraph” of the model). Search for something like this:

 Code:

Textures 5 {
 Bitmap {
 Image "Textures\BlackDragon.blp",
 }
 Bitmap {
 Image "ReplaceableTextures\Weather\Clouds8x8.blp",
 }
 Bitmap {
 Image "",
 ReplaceableId 1,
 }
 Bitmap {
 Image "Textures\Shockwave1b.blp",
 }
 Bitmap {
 Image "Textures\Tj_BloodSput.blp",
 }
}


The 5 after the textures tells you that there are 5 textures. Its simple to notice then where do the textures end. Now, you have to know that not all the textures make the dragon. Some are used to obtain the smoke coming from his mouth, other consists in the blood sprouted when the dragon dies. In this case we are interested only in the first texture. But how should you know this? Well, you will usually have to take all these textures in the Warcraft 3 Viewer and see which one would relatively look like the black dragon. In this case, its obvious that the first one. Other models such as Warden and Archimonde have only one texture for the model himself. But Arthas(Sword) for example has two textures: One for Arthas, and one for the sword. So be careful… Another type of textures which need to be taken in account are those with no path like the third one. You will also need to change those since they are team colors.


After you have detected the textures which make the unit itself (which will become ghostly), it will be easy to detect which materials need to be made transparent. Because all the materials corresponding to the textures which consist the model need to be made transparent, in order to obtain a correct ghost model. Ok, now to look at materials. You will find them right after the textures in the MDL. Here are the materials for the BlackDragon model:

 Code:

Materials 5 {
 Material {
 Layer {
 FilterMode None,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode Transparent,
 TwoSided,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode None,
 Unshaded,
 static TextureID 2,
 }
 Layer {
 FilterMode Blend,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode Additive,
 Unshaded,
 static TextureID 3,
 Alpha 3 {
 Linear,
 18000: 0,
 18033: 0.64,
 18367: 0,
 }
 }
 }
 Material {
 Layer {
 FilterMode Blend,
 Unshaded,
 static TextureID 4,
 Alpha 3 {
 Linear,
 18000: 0,
 18033: 1,
 18367: 0,
 }
 }
 }
}


How do you detect to which materials corresponds the first texture in this case? Well, each material has a TextureID, followed by a number. Another important thing I forgot to tell you is that for materials, textures are counted from 0. So first texture is 0, second is 1 and so on… In conclusion, we are looking for all the materials which have the TextureID followed by a 0. In this case of course! I explained above why. We will notice that the first material corresponds to the first texture, and so does the second and the third.


Now that we have detected the materials it will be piece of cake to make them ghostly. What you have to do is change what is after the syntax “FilterMode” to Additive. Let’s take an example for the first material.

 Code:

Material {
 Layer {
 FilterMode None,
 static TextureID 0,
 }
 }


Hmm, FilterMode is None. Change it Additive. It should now look like this:

 Code:

Material {
 Layer {
 FilterMode Additive,
 static TextureID 0,
 }
 }


Do the same thing with the second and the third materials. Notice though that at the third material there are two layers. Change the FilterMode to Additive only for the layer which has assigned the first material (index 0) and the third one. Since each layer is for one of these its not a problem. But there might be cases in which only one layer needs to be changed. Ok, to correctly make the black dragon ghostly, the materials should look like this:

 Code:

Materials 5 {
 Material {
 Layer {
 FilterMode Additive,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode Additive,
 TwoSided,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode Additive,
 Unshaded,
 static TextureID 2,
 }
 Layer {
 FilterMode Additive,
 static TextureID 0,
 }
 }
 Material {
 Layer {
 FilterMode Additive,
 Unshaded,
 static TextureID 3,
 Alpha 3 {
 Linear,
 18000: 0,
 18033: 0.64,
 18367: 0,
 }
 }
 }
 Material {
 Layer {
 FilterMode Blend,
 Unshaded,
 static TextureID 4,
 Alpha 3 {
 Linear,
 18000: 0,
 18033: 1,
 18367: 0,
 }
 }
 }
}


WARNING: Make sure that after each Additive there is a comma. If not, your model will not work!!!

5. Converting from MDL to MDX


This step is really important since through this, you will see if you haven’t done anything wrong with the MDL. From my knowledge, the models will be recognized as MDLs by Warcraft too, but they are bigger than MDXs and they can be bugged. That is why I suggest everyone once they are done with their MDL to convert it to MDX before importing it to warcraft. So open once again Warcraft 3 Converter, and open your MDL. Then click on “Convert to MDL” and pray that there are no errors. If there are, go back to the MDL and try the last step. If not, congratulations. You are now ready to import your ghost dragon to warcraft 3.

6. Importing the model for TFT users


Piece of cake. Open World Editor, Open the Import Manager. Click on the Import File button and import your model. Leave this path, so that you can use both dragons. Assign the model to a dragon, put one such dragon on the map and enjoy the result.

7. Importing the models for ROC users


This is going to be a little bit more difficult, but not much more. Ok, open WinMPQ. Now go to File – Open and select your map. Yes, warcraft 3 maps are also MPQ archives. Now go to Mpq-Add and add your dragon (MDX file). You will be asked to add a path. This will be very important when you will want to add skins for other models. But since this is a model, it doesn’t really matter the path. I gave it “Ghost\” . It should now appear in the list like this: Ghost\BlackDragon.mdx . Keep in mind the whole path (including the name of the model).


WARNING: Make sure that you give it a path or you won’t be able to assign it to an unit.


 Exit (you don’t need to save because it automatically saves) and open your map. Now open your Object Editor since its time to assign the new model. Select the unit you want, and click on the Art – Model File field to change its model. Now on the text bar write Ghost\BlackDragon.mdx . Place the unit on the map and voila, you are done.


Congratulations! The mighty ghost dragon should now appear on the map, flying.... Here is my result!


I really hope that this tutorial helped you. Can’t wait to see some of your work.


~Daelin


 Last edited by a moderator: Mar 6, 2007

#### LightOfPeople

#### LightOfPeople

 Level 1

JoinedJan 29, 2017Messages8

Can you make more Image pls

#### yram44

#### yram44

 Level 3

JoinedMar 26, 2011Messages44

Nice tutorial! Seems like Dota 1 Hero Tormented Soul has been made through this ghostly guide. Toremented Soul is the ghost version of Keeper of the Grove.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Funny thing actually, this tutorial is terribly outdated there are better approaches to this now.

#### Astrella

#### Astrella

 Level 8

JoinedOct 3, 2008Messages189

Resources: 1

#### Resources
Maps 1

Chaosy said:

 Funny thing actually, this tutorial is terribly outdated there are better approaches to this now.


Click to expand...

What would be a better approach now?

#### Hermit

#### Hermit

 Level 22

JoinedMay 29, 2013Messages1,573

Resources: 8

#### Resources
Models 4Icons 3Sounds 1

Astrella said:

 What would be a better approach now?


Click to expand...

Open the model in War3 Model Editor; go to Windows > Material Manager; change the Filter Mode in all material layers to Additive; save the model and you're done.

#### Astrella

#### Astrella

 Level 8

JoinedOct 3, 2008Messages189

Resources: 1

#### Resources
Maps 1

Oh thank you those turn out pretty decent; not exactly the same as Ghostly Antinodas, but I can't really put my finger on the difference.

#### Hermit

#### Hermit

 Level 22

JoinedMay 29, 2013Messages1,573

Resources: 8

#### Resources
Models 4Icons 3Sounds 1

Astrella said:

 not exactly the same as Ghostly Antinodas


Click to expand...

If you want a model to look exactly like Ghostly Antonidas, do this:

If you're editing a Hero or any other model with team glow, set the filter mode in material layers with team glow (aka Replaceable ID 2) to Additive and in all other material layers to Add Alpha.

Ghostly Antonidas doesn't have any team color because it would look unsightly under the transparent texture, so you should also delete all existing material layers that use team color (aka Replaceable ID 1).

#### Astrella

#### Astrella

 Level 8

JoinedOct 3, 2008Messages189

Resources: 1

#### Resources
Maps 1

Ah, that works great, thank you!

#### sebalarios

#### sebalarios

 Level 7

JoinedJul 15, 2015Messages35

Resources: 4

#### Resources
Maps 2Ported Icons 2

This still works on Reforged?

#### DREADMAN

#### DREADMAN

 Level 31

JoinedDec 6, 2022Messages724

Resources: 7

#### Resources
Models 6Skins 1

Hermit said:

 Open the model in War3 Model Editor; go to Windows > Material Manager; change the Filter Mode in all material layers to Additive; save the model and you're done.


Click to expand...

And it's even easier to make a certain type of units transparent in triggers by 50-60% with the help of triggers.

 You must log in or register to reply here.


### Similar threads

 Removing geosets/glow


alfredx_sotn

Apr 21, 2004

3D Modeling Tutorials

Replies4Views30K

Oct 21, 2005
Magos

 The Crystal Effect


olofmoleman

Dec 9, 2006

3D Modeling Tutorials

Replies9Views33K

Dec 10, 2006
operator

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

Locked

 Problem removing Glow


Leartes

Dec 10, 2008

Modeling & Animation

Replies3Views2K

Dec 10, 2008
Linaze

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