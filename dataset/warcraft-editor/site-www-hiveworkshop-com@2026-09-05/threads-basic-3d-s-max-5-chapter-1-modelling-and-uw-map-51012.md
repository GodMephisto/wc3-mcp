---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/basic-3d-s-max-5-chapter-1-modelling-and-uw-map.51012/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Basic 3D S Max 5 Chapter 1 Modelling and Uw Map
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/basic-3d-s-max-5-chapter-1-modelling-and-uw-map.51012/>
#### xXMephistoXx

#### xXMephistoXx

 Level 18

JoinedSep 3, 2006Messages940

Resources: 23

#### Resources
Models 10Icons 7Packs 1Skins 1Maps 2User Interface 1Tutorials 1

Basic 3D S.Max 5 - Chapter 1: Modelling and UVW-Map

Ok, in this chapter of my tutorials we're going to learn how to model, skin(only UW) and export a 3D Model for Warcraft III from 3D Studio Max 5.

1.:3D Studio Max 5 with Wc3 Art-Tools

2.:MDX Pather

1.:Magos' Wc3 Model Editor

2.:Wc3 Viewer

1.:Modelling it:

2.:Unwrapping it:

3.:Adding a Material:

4.:Exporting it:

First of, we're going to make a Box. Press the "BOX" button marked in the picture and draw a box(You first draw the length and width, then when you release your mouse-button, you choose the height). Then press the "Modify" button, also marked in the picture. Here you can choose your model's specifics(Length, Width, Hight and Planes pr. side). Set this as you want it, but it may be smart to set it like me if this is your first time modelling.

Now, right click on the "Box" text marked in the picture. Select "Convert to:>Editable Mesh"

If you've done it right you'll now see that the "Box" text changes to "Editable Mesh".

Now select the "Modifer List" and choose "Unwrap UVW".


Now select "Select Face" adn click the "Edit" button. The big window on the left side of the picture should now appear. Now select one of the sides and press "Planar Map" as shown in the picture. Later, if you decide to make more models, you could play around with this a little, and see how it all works and how to make good unwraps. Now repeat that stage for all the sides.

Now you'll have a square in the "Edit UVWs" window. Select all the vertexes(the corners). Then select the "Scale" button as shown in the picture and scale the entire square down a'bit, so that the model will not go beyond the skin.

Now you need to convert it back to "Editable Mesh", but this time just right-click in the window showing the model. Convert to>Editable Mesh, and you've got yourself a box with Un-Wrap!

Now we're going to add a skin to the model. This we'll do by entering the "Material Editor" as shown in the picture. When you've opened the material-window.


Now press the button showed in the picture labeled "Standard". Then a new window will pop up. Select Warcraft III from the bottom as shown in the picture.

Now the menu to the left will change drasticly, but the only button you need to worry about is the "Diffuse" button. Press it and a new window will appear. Double-click on "Bitmap" on the top and select the skin you would like to use for this model(Remember! Have to be a ".TGA" with an "Alpha Channel"!).


I would suggest you turn off "Show End Result", since it allows you to see the skin more clearly.

You see?


Now (while having your model selected) press the Assing Material to Object(The marked button to the left in the picture.). The model now has that Material assigned to it. If you've done it right, it should turn gray. To see the Skin on the model, we have to press the other marked button in the picture, "Show Map in Viewport". That'll make the ksin visible. If you can't see the ksin on the model, you've done something wrong.

Tada! We now have a finished model with a skin!

To export the model we have to have Wc3 Art-Tools.


Go to File>Export as shown in the picture. Select a destination and set the file-type to "Warcraft III Model (*.MDX,*.MDL)" and press save.

When a warning appears, just click OK, it has to do with animations.


In the folder you exported your model there should be two files: "'Your_model_name'.mdx" and "'Your_model_name'.blp"(In addition to some other files you might have saved there, like the "'Your_model_name'.MAX" and such.


First: Create a new folder called "Textures". Place the "'Your_model_name'.blp" in it.

Then open up the MDX Pather and open your model(*.MDX). Add "Textures\" to the path of the skin. Save it adn it should all work fine! To test it, open up in Wc3 Viewer or Magos' Model Editor.

And you have a finished model!


Follow This tutorial to learn how to import it to your map.

If you learn something from this tutorial, it was a good one!


 Last edited: Jan 9, 2008

#### madcat2

#### madcat2

 Level 14

JoinedAug 25, 2005Messages1,105

Resources: 14

#### Resources
Icons 6Skins 6User Interface 1Tutorials 1

What?(15 limit)

#### xXMephistoXx

#### xXMephistoXx

 Level 18

JoinedSep 3, 2006Messages940

Resources: 23

#### Resources
Models 10Icons 7Packs 1Skins 1Maps 2User Interface 1Tutorials 1

Sry, I have a shortcut to play music on my keybord, but when I pressed it, I submitted the thread... :S

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

the tutorial is too basic, tthere r advanced tutorials that do the same as yours but go further, maybe you should add something to make your tutorial more unique?

#### madcat2

#### madcat2

 Level 14

JoinedAug 25, 2005Messages1,105

Resources: 14

#### Resources
Icons 6Skins 6User Interface 1Tutorials 1

xXMephistoXx said:

 Sry, I have a shortcut to play music on my keybord, but when I pressed it, I submitted the thread... :S


Click to expand...


I wondered if that was the case xP.

#### xXMephistoXx

#### xXMephistoXx

 Level 18

JoinedSep 3, 2006Messages940

Resources: 23

#### Resources
Models 10Icons 7Packs 1Skins 1Maps 2User Interface 1Tutorials 1

@BlinkBoy: This is supposed to be simple, so that the ines that haven't got the knowledge will have an easy time beginning with modelling.


@madcat2:

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

the problem mephisto is that it is way too simple, you should try atleast teaching how to use the basic modelling tools like extrude, collapse, etc. The rest of the tutorial looks fine, so keep working on it.

#### xXMephistoXx

#### xXMephistoXx

 Level 18

JoinedSep 3, 2006Messages940

Resources: 23

#### Resources
Models 10Icons 7Packs 1Skins 1Maps 2User Interface 1Tutorials 1

It's a kinda "Work-Flow" Tutorial, so that's not what I'm trying to teach here, that'll be in a later tutorial(if this gets accepted) called "How to enhance your models".

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

oh ok, sorry I misunderstood ur tutorial purpose's; how about you add some basic animation like up and down and adding note keys, maybe edit the curve and set controllers. All that is inside the work flow.

#### xXMephistoXx

#### xXMephistoXx

 Level 18

JoinedSep 3, 2006Messages940

Resources: 23

#### Resources
Models 10Icons 7Packs 1Skins 1Maps 2User Interface 1Tutorials 1

It's coming in another tutorial  Maybe the mods'll decide to fuse them all into one, but for now I'll take one step at a time...

#### Sephiroth_VII

#### Sephiroth_VII

 Level 15

JoinedJun 10, 2007Messages389

Resources: 12

#### Resources
Models 8Icons 2Maps 2

You should Make the tutorial have you make a complex model with particles, animations, and using several different textures with teamcolor, transparent ect, that covers everything you would ever need to know to make a cool model. But keep it at this simple level its at now.

#### xXMephistoXx

#### xXMephistoXx

 Level 18

JoinedSep 3, 2006Messages940

Resources: 23

#### Resources
Models 10Icons 7Packs 1Skins 1Maps 2User Interface 1Tutorials 1

I don't even know all those things myself, but I'll put some of it in a later tutorial, "Enhancing your models".

#### Elfsilver Lord

#### Elfsilver Lord

 Level 16

JoinedFeb 3, 2007Messages1,081

Resources: 27

#### Resources
Icons 20Packs 1Tutorials 6

Or you can make an icon and assign it to the SammyCube.mdl...

#### xXMephistoXx

#### xXMephistoXx

 Level 18

JoinedSep 3, 2006Messages940

Resources: 23

#### Resources
Models 10Icons 7Packs 1Skins 1Maps 2User Interface 1Tutorials 1

Ofcourse you can, but that wont teach people how to model, would it?


BTW: How's that skin for my town hall coming along?

#### Kofi_Banan

#### Kofi_Banan

 Level 20

JoinedMay 26, 2006Messages653

Resources: 78

#### Resources
Models 70Maps 8

I agree with Blink its a bit too simple. Going with this newbe can create a skinned primitive. What about anims and creating more complicated shapes

Quality of tutorial itself is good.

#### xXMephistoXx

#### xXMephistoXx

 Level 18

JoinedSep 3, 2006Messages940

Resources: 23

#### Resources
Models 10Icons 7Packs 1Skins 1Maps 2User Interface 1Tutorials 1

It's coming more tutorials, maybe I'll get one done within a week...

#### JediMindTrix

#### JediMindTrix

 Level 6

JoinedJun 20, 2016Messages46

Resources: 4

#### Resources
Maps 2Templates 2

thanks for the intro!

#### xXMephistoXx

#### xXMephistoXx

 Level 18

JoinedSep 3, 2006Messages940

Resources: 23

#### Resources
Models 10Icons 7Packs 1Skins 1Maps 2User Interface 1Tutorials 1

Nihilism_Is_Death said:

 thanks for the intro!

Click to expand...

No problem; it's great that people are still using this!

 You must log in or register to reply here.


### Similar threads

Locked

 Basic Weapon Removal


Kimbo

Mar 14, 2007

3D Modeling Tutorials

Replies5Views28K

Sep 6, 2007
Kimbo

 UVMapping in Milkshape 3D


ElectricSaiyan

Feb 14, 2008

3D Modeling Tutorials

Replies11Views18K

Jun 20, 2008
BlinkBoy

 Creating Doodads in Gmax (simple grass)


eubz

Aug 14, 2011

3D Modeling Tutorials

Replies8Views12K

Mar 7, 2012
stonneash

 Basic Animation Transfers


Xarwin

Mar 20, 2007

3D Modeling Tutorials
2

Replies70Views87K

Jan 31, 2024
Vertigo2604

 Exporting models from Blender using the MDL exporter plugin


Fingolfin

Aug 20, 2018

3D Modeling Tutorials
23

Replies141Views68K

Sep 3, 2023
Fingolfin

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