---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/anti-noobism-texturing.92402/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Anti Noobism Texturing
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/anti-noobism-texturing.92402/>
#### Hawkwing

#### Hawkwing

 Level 34

JoinedNov 28, 2006Messages5,269

Resources: 29

#### Resources
Models 16Icons 3Packs 1Skins 8Tutorials 1

#
 ANTI-NOOBISM TEXTURING

Don't be offended by the name. It just means the basics and following the rules.

 Requirements


Programs:

Adobe Photoshop -Suggested

GIMP -Does work but tutorial does not use this program.

Other Requirements:

Understanding of Texturing -Minimal knowledge of texturing.

 Difficulty


3/10 -Easy - May vary.

People are laughed at, mocked, put down, and hurt when their texture gets rejected. This usually happens because of several common reasons; bad colour schemes, messy design, lack of shadows and highlights, breaking the rules, and of course the infamous evil model wrap. Follow these tips to keep yourself out of, what and I and many others like to call, noobism.

Why did I make this guide? When I first began texturing I didn't know what to do. There was no explanation on what CnP was or how to properly add shadows and highlights. So I decided to make a simple, easy to understand, tutorial for the new members of Wc3 Modding. Hopefully the result will end in more freehand work in the Hive Database.

##
 I - The Rules

A quick review of the submission rules.

###
 A) Skin Rules

75% Modification

A skin must be modified at least 75% from its original form in order to to be approved.

Original Work

Textures created by other authors are not permitted unless permission to submit it, under their name, is given. If users design a texture inspired by another author's texture full credit must be given to the original author for the idea.

Shading

Proper shadows and highlights must be positioned in the appropriate position to correspond with the in game perspective.

Copy and Paste

Copy and Paste from other author's work is not permitted. A maximum of 25% Copy and Paste from default Blizzard textures may be acceptable, providing it is executed very well.

Recolors

A maximum of 25% recolour on a texture is appropriate, as long as the rest of the changes are freehanded. For example, you may not have 25% Copy and Paste and 25% recolor, resulting in a total of 50% non-freehanded changes.

##
 I - Basics of Shadows and Highlights

Shadows and highlights are a mandatory part of any work of art. Whether it is a model, a texture, an icon, or even your ordinary piece of artwork, it needs shadows and highlights. In case you don’t know what shadows and highlights are here are the definitions.

Shadows - The representation of the absence of light

Highlights - An area that is strongly illuminated.

###
 A) How to Apply Shadows and Highlights

Cover the area you are texturing in your base colour. In this tutorial we will use red.

Find a spot where the light would normally hit the object. Light usually comes from above, unless of course there is some sort of light below them. Take a lighter shade of the base colour, red, and apply it where that light source hits. On the opposite side, where the light does not hit, apply a darker shade of the base colour. Sometimes, using soft brushes helps too. Scroll down through the different types of brushes and find the soft brush tool.

At this point it should still look fairly irregular. Use the eyedropper tool and click on the area where your highlights are. Now get a brighter shade of that colour and apply it to the point closest to the light source. Do the same with your shadows. It should look a bit more realistic now.

Now we have an object with proper shadows and highlights. Unfortunately, it’s still quite rough and the shadows and highlights are too sudden. Take a shade of your base colour that’s darker than the highlights but brighter than the base colour and apply it in between the two. Once again, do the same with the shadows. If you can’t get the different shades to blend well you can try smudging it a bit, but too much smudge can cause problems. I did not use any in this example.

Applying Shadows and Highlights to Pure White/Back Surfaces[/h3]

In some situations you may want to work with pure white colours or pure black colours. That will disable your ability to apply shadows and highlights, as shown above. This guide will show you how you can.

Instead of using the darkest black possible, to get a good pure colour, work with a dark gray. Cover the selected area with that colour.

Follow the guides above, and apply shadows and highlights.

You can do the same with whites as well.

###
 C) Positioning of Shadows and Highlights

The light emits from above so you'll want to apply highlights on the open areas facing up, like the shoulders. You'll also want to apply highlights to areas facing away from the center of the body, like the center of the chest, the outer side of the arms, the center of the legs, etc. The reason we do need apply highlights to the side of the legs, for example, is because the arms block out the sun causing it to shadow that area.

Areas like under the arms, under the chin, the bottom of the feet, and areas facing toward the center of the body will have shadows.

Some areas like the nose and feet should have highlights because they stick out farther than the rest of the body, causing the light to hit them.

###
 Applying Shadows to Flat Surfaces

Open up your texture and draw the outline of the flat surface.

Apply the same shadows and highlights used on the area you covered, so it looks nicely placed onto that surface.

Apply highlights where the light should hit, usually the front side of the area you are modifying.

Take the burn tool and set it to 10% opacity. Make a small outline around the area you're modifying. I overdid mine, so try not to do the same.

If the colour is off use the Hue and Saturation tool to change it slightly.

##
 III - Shadows and Definition

Definition is how definite, distinct, and clear the area around an object is. Sometimes when an object is too defined it can look ugly. But that can be fixed through shadowing.

###
 A) Removing that Nasty Definition

Open your ‘overly defined’ texture. In this tutorial we will use the Grunt skin. Notice how the lines around the breastplate are thick and very visible.

The outline is basically a very sudden shadow/highlight. You need to make it more gradual.

Now take the eyedropper tool

 and click on the shadows. The current colour you are using will change to the one you have selected. You should get an image similar to this n your toolbar, now.

Double-click on the black square, the colour you just got from the eyedropper tool.

Select a slightly brighter shade of black. Now paint that overtop the closest point of the shadows towards the middle.

Repeat step 3, 4, and 5 with the highlights.

Your result should be a much less definition and more smooth shadows.

##
 IV - Wrap and Unwrap

Textures have two different ‘forms’, a wrap and an unwrap. The unwrap is when you have your texture on a flat 2D Image. The wrap is where the texture is applied to the model, like wrapping paper the image is wrapped around a 3D Mesh to give it colours and better shadows and highlights.

###
 A) Dealing with Messy Wraps

Blizzard is fairly known for creating poor model wraps causing our textures to become stretched and distorted. There is a way to fix this though. I’ve aligned the chest muscles in the breastplate on the unwrap but in the wrap it’s not aligned.

Lower the left chest muscle on the breastplate in the unwrap and it should align it on the wrap. I made this quickly so it’s still quite messy but hopefully the point gets through.

Take a long hard look at the right chest muscle and notice its shape. Try to repeat that on the left side as much as possible without ruining the wrap.

##
 V - Keeping Below 26% Non-Freehand

The Hive Workshop, and other modding sites, have rules against recoloring, filters, and CnP. Here’s some ways to use them and continue to follow the rules.

###
 A) Filters for the Better

Open up your texture. In this guide we will be using the Knight texture. Apply a base colour the selected area.

Now add some detail, like shadows, highlights, and such.

Now use the polygonal lasso the select an area along the area you wish to use filters on. Try to use the least amount of room for the filtering; we have to stay under 25% filters. Now hit Ctrl+C followed by Ctrl+V. This will duplicate the selection onto a new layer. Now go to Filters > Texture > Craquelure, or whatever filter you wish to use.

Toy around with the different fields until you’ve got what you want. Now click ‘Ok’.

If the filtered area dosen't blend well with the non-filtered area grab the eraser tool and put the opacity to 40%. Use of the eraser tool over top Layer 2 once, this will make the filter blend with the background better. Keep going over the upper side of the filtered area until you’ve got it blended nice and good.

Filters are still quite messy so you’ll need to play around with the shadows a bit. Take the smudge tool and smudge the edge of the filtered area to remove the ugly outline the filter gives you. Afterwards, use burn to increase the darkness at the point where the light should hit it the least.

This is just one of many ways that you can use filters for the better. As long as you avoid using them too much they can be very useful.

###
 B) No Need for Freehand - Recolours For the Better

Open up a texture, we will be using ZombieA.blp. Now use the polygonal Lasso to select the area of the skin you wish to recolour. Once again make it small as possible because we have to follow the 75% freehand rule.

Press Ctrl+U or go to Image > Adjustments > Hue/Saturation. Adjust the hue until you get the colour you want.

Now add in freehand details around that recoloured area.

Keep in mind that recolours are frowned upon and should not be used regularly. Do not make a habit of this. If you can make the area you are recolouring yourself, without using recolours.

###
 C) Adjusting the Hue - Recolours For the Better

Sometimes after freehanding a skin you get the incorrect shade or colour. With a quick recolour you can fix that.

Use the polygonal lasso to select the area you are going to recolour.

Press Ctrl+U and mess around with the different fields until you have the desired shade/colour.

###
 D) Copy and Paste, For a Better Effect

Sometimes copying photographs of different materials at a low opacity overtop your texture can help and give it a more realistic look. Much like in this tutorial by Zambo.

Check out Zambos' full tutorial at Wc3Campaigns - Skinning Techniques.

##
 VI - Detailing

Follow these tutorials to add extra details to your texture.

###
 A) The Noise Effect

In order to make a noise effect we must first use the information we learned earlier and design the basic layout of the area we're editing. For example, apply a base colour, shadows, and then highlights.

Now take the brush tool and choose a brighter or darker shade of your base colour. Set the opacity of your brush to 40% and the size to 1 pixel. (May change depending on image size.) Paint on dots all over the area you're editing. Sometimes it looks nice if you mess up the shadowed and highlighted areas to give it a rough and rippled texture. Afterwards do the same with the opposite shade. (i.e. dark shade <-> light shade.)

The result may still be quite ugly, but keep going and it wont for much longer.

Take some miscellaneous colours and paint them all over the same area. Try not to do as many dots as you did with the previous colours or it will look odd.

Now take the smudge tool at a low opacity (10%-30%) and blend in the dots of paint. If you lose a lot of the misc colours you added in you can just reapply them and resmudge them. But be warned, if you smudge the image too much it will end in a very blurry appearance.

##
 VII - Additional Texturing How-To's

Use these extra tips to help you before and after making your texture.

###
 A) Taking Screenshots

Open the Prophet's Wc3 Viewer.

Click on the Search button.

Enter the name of the model your texture uses.

Now press Current File > Revamp Textures.

Find the texture you modified and select it (I have modified foresttroll.blp, so I will select that). Now press change and find your texture file.

Once you found it double-click and press "Okay". Now return to the Viewer and use the Treeview window to set an animation, rotate the model so you have a good view of it and press Snapshot on the toolbar at the top.

Save the file and you're done!

##
 Texturing Index

Some phrases apply to Wc3 texturing only.

###
 Phrase

###
 Meaning

RecolourThe use of the Hue/Saturation Tool or a low opacity brush to change the colour of a texture.

CnP Copying parts from other textures and pasting them onto yours.

FiltersA pattern overlay used for detailing.

FreehandDesigning part of a texture using only the brush tool.

Alpha ChannelsAlpha channels allow you to literally removing areas or applying Team Colour through the eraser tool.

Team ColourAn area where the colour is dependant on the player's team colour.


 Last edited: Jan 29, 2009

#### Linaze

#### Linaze

 Level 24

JoinedFeb 28, 2007Messages3,478

Resources: 4

#### Resources
Models 1Tutorials 3

Way to go, Hawkwing. I thought this tutorial was very good, and any "noob" at texturing should read it. Though it isn't much of a tutorial and more of a "what not to do while texturing" still will prove useful to lots and lots of people. Though I am afraid these kind of people don't really read the tutorials, I don't know. Still, this is awesome and should definately be approved in my opinion, good work!

#### Hawkwing

#### Hawkwing

 Level 34

JoinedNov 28, 2006Messages5,269

Resources: 29

#### Resources
Models 16Icons 3Packs 1Skins 8Tutorials 1

Linaze said:

 Way to go, Hawkwing. I thought this tutorial was very good, and any "noob" at texturing should read it. Though it isn't much of a tutorial and more of a "what not to do while texturing" still will prove useful to lots and lots of people. Though I am afraid these kind of people don't really read the tutorials, I don't know. Still, this is awesome and should definately be approved in my opinion, good work!

Click to expand...


No they don't usually read them but you can link them to it in the comments for the resources and then they can improve. The bottom is what not to do. That's all. I'm adding more "what to do" things too. I've added a Messy Wrap section and I'll be doing some more stuff later.

#### TDR

#### TDR

 Level 18

JoinedSep 26, 2004Messages1,543

Resources: 2

#### Resources
Skins 1Tutorials 1

Hawkwing said:

1. Filters are a wonderful and beautiful tool that can be found in programs like Adobe Photoshop and GIMP. Why not use it? Because they suck and they have no use except to make your work look goofy and expose your lack of experience.

Click to expand...

fixed. now the tutorial is complete.

And always remember this basic rule of thumb: shadows -> color is more saturated, light -> color is less saturated.

#### Hawkwing

#### Hawkwing

 Level 34

JoinedNov 28, 2006Messages5,269

Resources: 29

#### Resources
Models 16Icons 3Packs 1Skins 8Tutorials 1

Thanks for that.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

well its good, but there's something you need to clear up. First: Recoloring, Filters and CnP aren't really against the rules, its just that if you only use those techniks with your texture, it won't be approved because anyone can simply do that, it's like geomerges and adding simple edits in modelling.


You should add when those techniks should be used , like: Lets say Recoloring, we use recoloring when after finishing an areas shading, we want to color it, normally what I do is to apple some hue and a low saturation to have a base color, then I do some brushing with similar colors and I saturate and desaturate some areas.


Filters are also useful for extra effects, like when you are doing metal, but you want it to look a bit more realistic, you add a new layer apply a non-monochromatic noise, then blur, then I set the layers opacity to 5-10, then to overlay and I merge it with the metal layer.

#### Hawkwing

#### Hawkwing

 Level 34

JoinedNov 28, 2006Messages5,269

Resources: 29

#### Resources
Models 16Icons 3Packs 1Skins 8Tutorials 1

Okay. I'll fix that up as soon as I can.


Edit: Done. I've heavily modified the recolours and filters section.


Edit2: Okay, I've added a Definition section.


 Last edited: Jul 26, 2008

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

you know CnP has a lot of uses, but there's a rule: you can only CnP pictures, no skins(or textures made by other authors), only pictures taken by camera.


Here is an example, not the best example, because he used recoloring in a bad way, but meh... Advanced Skinning in Photoshop


and here's another: Creating a realtexture plant [example: syringa] - Wc3campaigns it's a way better example.


and the example you gaved us, its good to produce cinimatic/movement in pictures such as icons.

#### Hawkwing

#### Hawkwing

 Level 34

JoinedNov 28, 2006Messages5,269

Resources: 29

#### Resources
Models 16Icons 3Packs 1Skins 8Tutorials 1

Oh really? I was wondering why all those pictures of Marble and stuff were used in that Skinning Techniques tutorial. Still, I think you should avoid it. That's just my opinion. I was more or less refering to Textures to Texture CnP in the Tut.

Edit: Revised Definition section and created a 'Adjustments through Recolours'.


 Last edited: Jul 27, 2008

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

BlinkBoy said:

 you know CnP has a lot of uses, but there's a rule: you can only CnP pictures, no skins(or textures made by other authors), only pictures taken by camera.


Here is an example, not the best example, because he used recoloring in a bad way, but meh... Advanced Skinning in Photoshop


and here's another: Creating a realtexture plant [example: syringa] - Wc3campaigns it's a way better example.


and the example you gaved us, its good to produce cinimatic/movement in pictures such as icons.


Click to expand...


What if the author, or site that hosts the textures, gives them out for the sake of C&Ping them into your pictures? Like renders?

#### General Frank

#### General Frank

 Level 92

JoinedNov 19, 2005Messages8,035

Resources: 369

#### Resources
Models 232Icons 124Packs 6Skins 2Ported Icons 3Tutorials 2

Awesome tutorial. Finally got it approval.

~> Approved.

#### Hawkwing

#### Hawkwing

 Level 34

JoinedNov 28, 2006Messages5,269

Resources: 29

#### Resources
Models 16Icons 3Packs 1Skins 8Tutorials 1

Thanks.

#### Jack_Sparrow93

#### Jack_Sparrow93

 Level 16

JoinedMay 9, 2008Messages1,435

Resources: 9

#### Resources
Icons 5Maps 2Spells and Systems 2

Useful tutorial! Thanks!

#### darvarda93

#### darvarda93

 Level 2

JoinedNov 4, 2008Messages12

Your tutorial is very easy to understand, I think I'll give texturing a go now .


Btw, the pictures aren't showing anymore :O. Photobucket nailed them XO

#### Hawkwing

#### Hawkwing

 Level 34

JoinedNov 28, 2006Messages5,269

Resources: 29

#### Resources
Models 16Icons 3Packs 1Skins 8Tutorials 1

I moved them all. I'm redoing the tutorial.

 You must log in or register to reply here.


### Similar threads

D

 Basic 'Soft Shading' Technique


Deleted member 157129

Sep 29, 2010

2D Art Tutorials

Replies5Views23K

May 4, 2020
-Grendel

 Polished Iron Texture


~Void~

Jan 23, 2009

2D Art Tutorials

Replies12Views9K

Jan 28, 2009
~Void~

 NFWar's Tutorials


NFWar

Sep 15, 2010

2D Art Tutorials

Replies41Views37K

May 12, 2017
plazma019

 Bandage Tutorial


Kimbo

May 12, 2010

2D Art Tutorials

Replies14Views28K

May 11, 2015
Grey Knight

 Texturing Armour


Kimbo

Dec 27, 2007

2D Art Tutorials

Replies16Views17K

Aug 20, 2008
Needles

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

2D Art Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top