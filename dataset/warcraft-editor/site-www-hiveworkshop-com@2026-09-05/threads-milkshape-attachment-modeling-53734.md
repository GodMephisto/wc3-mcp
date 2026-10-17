---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/milkshape-attachment-modeling.53734/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Milkshape Attachment Modeling
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/milkshape-attachment-modeling.53734/>
#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

#
 Milkshape Attachment Modeling

By HappyTauren/TotallyAwesome

##
 Introduction

Many have seen attachments models made by me and kept asking me how I do it. That is why I decided to write this tutorial. Keep in mind that the texturing part is not included in this tutorial, though I will show you how to unwrap your model here.

##
 Part one: Preparation

1. What do you need?

-Milkshape 3D, a low polygon modeler mainly made for Half Life, but has a proper exporter for mdx/mdl;

-Guesst's ms3d exporter, which you unzip and put into ms3d directory;

-Warcraft3 Viewer;

-Some paint program , you can use either MSPaint, or download GIMP;

-Very basic or no knowledge of milkshape 3D (in which case you have to figure some things out by yourself).

2. Now that you are all set, lets start!

##
 Part two: Reference image

1. Lets say you want to make a sword. First, you will have to draw it, like this:

Start up paint, make a new bmp file, and modify attributes so that they match power of 2, meaning image resolution could be 128X512, 64X128 and so on, warcraft 3 resolutions. Click okay.

2.Then, fill the image with dark blue colour (so you can see vertices). Then you will get something like this:

3.After that has been made, pick up red colour and start drawing your sword!

First you should draw an outline, which looks like this:

4.Then, as we know that swords are not really flat, you should make indents somewhere! Okay, the image is complete!

##
 Part three: Modeling the mesh in Milkshape3D

1.Open the milkshape. The only tools you will need for this are marked in RED, you only need "Vertex" and "Face" buttons. This is milkshape interface:

2.Now you have to use your reference picture. To do this, just right click on the first field and select "Choose background image". Then click the ">>" Button and find the image on your hard drive, in this case, "Sword.bmp" placed on desktop.

3.You are all set! Now you an start creating your mesh. Use the "Vertex" tool (on the right panel) and start placing vertices on the "hot spots" What is a hot spot? It is a place where few edges collide, for example like in the picture, or when edge is curved too much (those green points are "hot spots". Make sure to use "wireframe overlay" view (right click > wireframe overlay).

4.Now, when vertices are placed, you need to make faces on them. You do this by selecting three vertices, in counter-clockwise rotation. You will get something like this:

5.Now you have finished adding anything. Change background image to blank. Now select ALL indented vertices (the ones you placed on the inner red lines. Then look at the window on the right. Use move feature, locking all axis BUT "Y" (to lock an axis, just click its letter on the right panel, e.g "X","Y","Z")

6.You have got sword finished, but it is one sided. To make it have the other side as well, you must duplicate it (CTRL+D) and then go to Vertex > Mirror front-back. Now weld it (CTRL+W). After that, go to groups, click both groups and click regroup. Now rightclick and select "Smooth shaded". You will see your model actually has a fill. So the geometry of the model is actually complete!

7. Now you have to change material and texture coordinates. This will be easy, just go to "materials" tab on right panel and click new. Then click small blank button a little up it (the first one!) and find your Sword.bmp texture. Then select all (CTRL+A) and doubleclick this material. Material is created. Now just go to Window>Texture Coordinate Editor and select "front" in small box. Then click remap.You will notice that your model is a bit bigger than the wrap but use scaling to get it to the right place (untill red lines on texture and on the selection are even!). This is what should it look like:

Now exit texture coordinate editor and returns to ms3d. This is what it should look like:

8.Now the major thing: Make it usable in wc3! First set up rotation. To do this, click rotation tab on the right panel and you will see 3 values near the botton, x,y and z. Set X=-90,Y=90 and Z=0. Now click rotate two times (the button right those three). Now the model has correct facing.

##
 Part four: Exporting

1.It is time to export your new model to wc3. Go to "File>Export>Wc3MDLX beta 10" and then choose a location where you want to save it (remember to put MODELNAME.mdl when you are choosing name!!).

2. Now open this mdl in notepad, and search for "white.blp". Replace it with "Modelname.blp". Now you have to convert texture from ".bmp" to ".blp". To do this, use Warcraft3 Model Viewer and find button on the upper panel, right, that says something like "TGA>BLP", upside down. Click it. You will be prompted for quality settings, select 50, and it will say that you are missing alpha channel, asking you to add it, you say yes.

3. Now your texture is exported. Rename it (if it is not already) MODELNAME.blp and put in same folder as model is. Then import both of them ingame and delete "war3mapimported\" off the texture's filename.

4. Now, the only thing that the model lacks is a suitable texture. To read some tutorials, follow those links:

-(THW) 2D Art Tutorials

-(WC3C) 2D Art Tutorials (Skinning)


 Last edited: Jul 28, 2008

#### operator

#### operator

 Level 15

JoinedFeb 9, 2006Messages1,598

Resources: 2

#### Resources
Maps 2

Nice : ) *

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

Operator lives. Sweet tutorial HT

#### Dumbdumb36

#### Dumbdumb36

 Level 7

JoinedJan 13, 2008Messages201

Resources: 1

#### Resources
Maps 1

Nice ^^ I'm still a newb with milkshape

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Good tutorial.

[off topic]

The sword hilt looks "obscene"

Maybe it's just my twisted mind.

[/off topic]

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

O NOES VOID FIGURED IT OUT1111

yeah, pron out there

#### blazing-inferno

#### blazing-inferno

 Level 7

JoinedDec 29, 2006Messages365

Resources: 1

#### Resources
Tutorials 1

it works in game but for some reason the viewer doesn't detect the texture even when its changed in the mdl. eh... ill stick to 3d max. good tutorial though.

#### Firelord213

#### Firelord213

 Level 14

JoinedNov 4, 2006Messages576

Resources: 5

#### Resources
Models 2Icons 3

Pl0x explain the 6th part much more?Im stuck and cant do anything...I fail following it as it is at the moment.


 Last edited: Mar 1, 2008

#### underscore

#### underscore

 Level 6

JoinedMar 17, 2008Messages150

Resources: 6

#### Resources
Skins 1Tutorials 5

Well made tutorial, I always look at your models you know. HappyTauren!

#### RedFuser

#### RedFuser

 Level 6

JoinedJun 29, 2008Messages16

Resources: 4

#### Resources
Models 3Tutorials 1

Perfect tut!!! +rep

#### DragonElite

#### DragonElite

 Level 2

JoinedJun 28, 2008Messages21

Hehe, Redfuser just made his second model thanks to this tutorial, I guess.


But someone knows how to get 'adobe photoshop' free? because I don't have one.


I can only make models but not texturing them... =/

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

HappyTauren said:

 -Some paint program , you can use either MSPaint, or download GIMP.


Click to expand...

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


Please use the new [h1][h2][h3] tags, read the The Hive Workshop - A Warcraft III Modding Site - Announcements in Forum : Tutorial Submission.

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

updated

#### General Frank

#### General Frank

 Level 92

JoinedNov 19, 2005Messages8,035

Resources: 369

#### Resources
Models 232Icons 124Packs 6Skins 2Ported Icons 3Tutorials 2

Nice tutorial, very detailed and informative. I already practised the techniques described in it. Good work.

[RAINBOW]~ Approved ~[/RAINBOW]

#### deathchiken

D

#### deathchiken

there was no Wc3MDLX beta 10 under file>export, do u have to have the full milkshape, as in regestration? please help me.

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

You download guesst's exporter and put it into Milkshape 3D folder.

#### skullzilla

#### skullzilla

 Level 5

JoinedJul 1, 2008Messages46

Resources: 3

#### Resources
Models 2Icons 1

How do you use more than one texture on a model with milkshape? I tried using 2 different materials but when i exported it only had textures 1 white.blp, i didn't see a place to put the second one.


[nvm I figured it out]


You just add a second texture in the mdl file and change the material id to reference the second texture.

 Code:

Textures 1 {
 Bitmap {
 Image "Textures\white.blp",
 }
}


becomes

 Code:

Textures 2 {
 Bitmap {
 Image "Textures\whatever.blp",
 }
 Bitmap {
 Image "Textures\yaddayadda.blp",
 }


but then you have to change the materials part to match the texture..


static TextureID 0,


...


static TextureID 1,

#### T3MPL4R

#### T3MPL4R

 Level 4

JoinedSep 27, 2007Messages85

very well done tutorial and it looks very helpful although, how do you add COLOUR to the sword!?!?!?


P.S why does the bottom of the handle look disturbing?! in the first picture


P.P.S why do does the bottom of the handle look even more disturbing when you put dots in them XD

#### cool_is_i

#### cool_is_i

 Level 12

JoinedOct 23, 2007Messages565

Resources: 19

#### Resources
Icons 9Maps 8Tutorials 2

Firelord213 said:

 Pl0x explain the 6th part much more?Im stuck and cant do anything...I fail following it as it is at the moment.


Click to expand...


same here, can some 1 please send me a PM explaining how to do this?

#### valkemiere

#### valkemiere

 Level 13

JoinedNov 20, 2006Messages553

Resources: 9

#### Resources
Models 6Maps 3

can someone help me..i want to ask about using milkshape 3d. i made nagoya castle.


why in milkshape look good. when in magos look not good >.<

#### Attachments

Nagomilkshape.JPG

 20.4 KB

 · Views: 542



Nagomilkshape2.JPG

 17.5 KB

 · Views: 509



#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

My guess is you didn't use proper face culling when making this. well, the easiest way to do fix this (although not the best and most effective way) would be to go into your material manager in magos, then double click on your material, then on the material layer. Inside, you should see the checkbox flag for "Two-Sided". Check that, and you should be good.

--donut3.5--

#### valkemiere

#### valkemiere

 Level 13

JoinedNov 20, 2006Messages553

Resources: 9

#### Resources
Models 6Maps 3

thank you so much donuts. then how to use proper face culling when making model?

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

When you make a model and use the face tool, they must be created in a counter-clockwise order (meaning click on the vertices in a counter clockwise order)

To correct this, you can select every improperly culled face and press Ctrl+Shift+F, or go up to the Face menu and select Reverse Vertex Order (I think that's the command)

--donut3.5--

#### valkemiere

#### valkemiere

 Level 13

JoinedNov 20, 2006Messages553

Resources: 9

#### Resources
Models 6Maps 3

okay.. thanks again.

#### NoobinJass

#### NoobinJass

 Level 5

JoinedAug 24, 2008Messages230

Resources: 2

#### Resources
Icons 2

(Off topic) Where can i get a full registered Milkshape? Can anybody upload, attach or whatever for me? (Off topic)

I only can make a vase on milkshape...

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

That would be illegal so no. Pay the 40 dollars or whatever it is that it costs to buy it if you're serious.

#### Rubber

#### Rubber

 Level 6

JoinedSep 26, 2008Messages139

Resources: 6

#### Resources
Models 1Icons 2Skins 1Maps 1StarCraft II Resources 1

is possible save a image saved by link and edit in milkshape: ex one piece image ecc?

#### Manoo

#### Manoo

 Level 10

JoinedSep 14, 2007Messages227

Resources: 7

#### Resources
Models 5Maps 2

Hey happy tauren where i can get guesst's exporter ??????

#### Manoo

#### Manoo

 Level 10

JoinedSep 14, 2007Messages227

Resources: 7

#### Resources
Models 5Maps 2

oh sorry i founde  tnx very tnx

#### Total_Warrior

#### Total_Warrior

 Level 12

JoinedApr 8, 2007Messages692

Resources: 8

#### Resources
Models 4Maps 3Tutorials 1

I still got lots to learn -.- i messed up on part 3 step 3-end

#### ProfessorZmurge

#### ProfessorZmurge

 Level 7

JoinedJul 7, 2008Messages332

Step 7 is useless?

After I exported all the textures got messed up.


 Last edited: Jan 19, 2009

#### valkemiere

#### valkemiere

 Level 13

JoinedNov 20, 2006Messages553

Resources: 9

#### Resources
Models 6Maps 3

can someone help me, the yellow_star model lookslike box,


how to change it like really yellowstar ? tQ

#### Siloth

S

#### Siloth

I hope that, this tutorial will help me with models. GJ!

#### HaZe

H

#### HaZe

 4.Now, when vertices are placed, you need to make faces on them. You do this by selecting three vertices, in counter-clockwise rotation. You will get something like this:


Click to expand...


I'm a noob and I really got lost here, anyone think they can explain a bit more, or show me? would appreciate it a lot =) otherwise nice tutorial


+rep if I can get past the 4th part and succeed in actually making the sword ^^

#### M3dalz

#### M3dalz

 Level 2

JoinedJan 14, 2009Messages17

~Void~ said:

 Good tutorial.


[off topic]


The sword hilt looks "obscene"


Maybe it's just my twisted mind.


[/off topic]


Click to expand...


Lol I do agree with you there =) but this guide is still really really Good Thanks!

#### Zurukin

Z

#### Zurukin

Help!!!

Ok, I went to download Milk Shape, but i got a newer version that doest work the same way. I cant "choose background image"!!!! What do i do?

#### Bluchaos

B

#### Bluchaos

How are we suppost to save what we've created? We have to buy the program?

#### Zurukin

Z

#### Zurukin

Ohhhhhh......

OK, got it now. apparently, I was using the wrong field. I tried using the one in the tutorial, but it wouldn't allow me to make the background image. so, im using the one on top of it.

>.<... OK, new problem.... The MS3D Exporter/Importer thing doesn't work. u cant dl the correct file. theyre all updates...

ohhh.... nvm


 Last edited by a moderator: Jun 15, 2009

#### madman9800

#### madman9800

 Level 10

JoinedFeb 19, 2006Messages237

Resources: 9

#### Resources
Skins 8Maps 1

I followed this tutorial to make my first model and it worked out pretty well..but i have a question...


first of all..when i imported my model in mdl format it didnt show up in warcraft, so i converted it to mdx, then it showed...but when i attached it a villagers hand...the sword was facing backwards, and it was attached in like the middle...


is there something i need to do to tell this model should be attached at the HANDLE not in the middle and that it shouldnt face backwards...it looks weird...please help someone...thank you

#### Jacky The turtle

#### Jacky The turtle

 Level 13

JoinedAug 30, 2009Messages618

Resources: 32

#### Resources
Models 3Icons 22Packs 1Skins 5Tutorials 1

please help me


 in the new milkshape> export i cant find the good one

#### Dumbdumb36

#### Dumbdumb36

 Level 7

JoinedJan 13, 2008Messages201

Resources: 1

#### Resources
Maps 1

icecreamofdeath said:

 please help me


 in the new milkshape> export i cant find the good one


Click to expand...


? there is a warcraft III exporter built into milkshape, or you can use war3 model editor to convert it to a MDX file.

#### Sunchips

#### Sunchips

 Level 30

JoinedJul 23, 2009Messages1,028

Resources: 125

#### Resources
Models 110Icons 8Packs 7

I replaces the white.blp with my models name and then .blp and so I converted it to blp with the viewer program but it shows up as 0kb and when I import it to wc3 the model has no texture. Also I tried reversing the blp back to bmp and then it turned up to be an empty picture so something is wrong when i convert to blp pls help  +rep for help

EDIT: I tried converting the image to TGA first and then Convert to BLP and now I can see the image in the viewer at least and it now take more space than 0kb but still the weapon is untextured

EDIT again: I fixed it, nevermind. Although now I wan't to upload my model because I'm pretty happy I was able to do such a model and I want to show it but I can't upload models made in milkshape? check my thread https://www.hiveworkshop.com/forums/modeling-and-animation-276/cant-upload-milkshape-model-145671/

#### Attachments

imageshack.jpg

 15 KB

 · Views: 414




 Last edited: Oct 3, 2009

#### Zurukin

Z

#### Zurukin

Duplication?

The duplication, mirroring, and welding parts arent working. plz help me. It will be my first model!

Nvm, my dad helped.

Well... crud..... i finished it but the export wasnt there AND it said i had to pay....


 Last edited by a moderator: Oct 17, 2009

#### Teaspoon

#### Teaspoon

 Level 14

JoinedOct 1, 2009Messages596

Resources: 14

#### Resources
Models 14

Stuck at part 6... Nothing happens even tho I do as you said.


Nvm I solved it


But I don't really understand the exporter thing, where can you download it? This doesn't require you to buy MS3D right?


 Last edited: Oct 29, 2009

#### kikicamkd

#### kikicamkd

 Level 5

JoinedJun 12, 2009Messages27

Resources: 8

#### Resources
Icons 5Skins 3

where can i find counter-clockwise rotation?


i dont have it.Pls tell me where!

#### akatsuki2700

#### akatsuki2700

 Level 3

JoinedDec 30, 2008Messages47

 6.You have got sword finished, but it is one sided. To make it have the other side as well, you must duplicate it (CTRL+D) and then go to Vertex > Mirror front-back. Now weld it (CTRL+W). After that, go to groups, click both groups and click regroup. Now rightclick and select "Smooth shaded". You will see your model actually has a fill. So the geometry of the model is actually complete


Click to expand...


Everything is going fine until I reach the 6th part I can't seem to weld them somehow it won't fuse together please tell me what's the problem~?


 Last edited: Dec 16, 2009

#### Gamecrafter_DK

#### Gamecrafter_DK

 Level 14

JoinedSep 25, 2007Messages1,257

Resources: 14

#### Resources
Icons 5Maps 6Spells and Systems 1Tutorials 2

Hmm... i can't seem to find a WCIII export, and i have the newest version, trial edition.

#### BishopOfDoom

#### BishopOfDoom

 Level 6

JoinedJun 16, 2009Messages82

Resources: 6

#### Resources
Icons 2Skins 4

But how do I make it work as attachment model?

The one you import in game, and attaches to a certain bone there.

#### Mulgrim

#### Mulgrim

 Level 15

JoinedMar 31, 2009Messages1,556

Resources: 4

#### Resources
Models 3Tutorials 1

nice tutorial +rep

@Gamecrafter_DK, just get it and Zero Joints anywhere in the ms3d folder, then it will be on Export next time on milkshape

@BishopOfDoom, you don't have to add attachment points, just place it right, as in where the axises meet, that will be where your unit holds it.

 You must log in or register to reply here.


### Similar threads

 UVMapping in Milkshape 3D


ElectricSaiyan

Feb 14, 2008

3D Modeling Tutorials

Replies11Views18K

Jun 20, 2008
BlinkBoy

 Milkshape tutorial - A simple way to unwrap your models.


Mulgrim

Jan 23, 2010

3D Modeling Tutorials

Replies4Views14K

Jun 16, 2010
makai

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

 How to edit models- a complete guide


Horn

Oct 4, 2004

3D Modeling Tutorials

Replies5Views49K

Dec 1, 2013
animation31

 Creating Doodads in Gmax (simple grass)


eubz

Aug 14, 2011

3D Modeling Tutorials

Replies8Views12K

Mar 7, 2012
stonneash

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