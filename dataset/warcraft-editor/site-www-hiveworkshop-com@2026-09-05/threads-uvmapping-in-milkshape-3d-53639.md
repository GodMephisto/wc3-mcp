---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/uvmapping-in-milkshape-3d.53639/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Uvmapping in Milkshape 3D
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/uvmapping-in-milkshape-3d.53639/>
#### ElectricSaiyan

#### ElectricSaiyan

 Level 9

JoinedJul 20, 2005Messages414

Resources: 4

#### Resources
Models 2Tutorials 2

@Admin or Moderator reading this: Thank you for the time you're spending reading my submission. I'm not extremely certain about the rules for tutorial submissions because this is my first one, so if I have any changes that need to be made, please notify me so I can do so and possibly get this approved. Thanks very much.


Also, this tutorial was from a thread where a member needed help, so most is copy/pasted from that thread (which is where my images are attached).


Once again thanks for your time


This tutorial is for using only one skin, so if necessary, you can PM me for information on multiple skins.


Now the easiest way to start this is to create a texture for you to UVMap to.


What I would do is imagine how you want your pad laid out


Do you want to have detail on a majority of the front/back side, or the top of it?


1. If front/back, change the perspective of the view in Milkshape to front, make sure it's on Wireframe, and then take a screenshot (PRT SCRN).


2. Paste that view into paint or photoshop, on a document that's size is 128x128 or 256x256. I would probably choose 128 unless you want lots of details for just the shoulder pads, or if you have plenty of space.


The view should look something like this:


And your new texture like this:


BTW: Now if you wanted to do it from the Top, do the same process as 1. but instead take the screenshot from the top view.


Moving on, now that you have your base UVMap, save it (I would avoid saving it as a jpeg, as it will lose quality).


Now move into Milkshape once again. Go to the Materials tab and create a new material by selecting the button the says New at the bottom (1). Now that your new material has been created, you need to assign your texture that you just created to it. Do this by selecting None in the middle of the Materials tab panel. There are two, one on the bottom, one on the top. Just select the top one for the sake of not messing something up  . If you're lost, look at 2 on the image at the bottom of this paragraph. Now that the window has opened after pressing None, select your new texture, which is your uvmap...map!


You can see your texture on the sphere that is shown in the panel (3).


Now we need to assign your mesh to that material. You do this by going to the groups tab, and selecting all the groups that contain your model. Select it by clicking on the Group Name at the top, and then hitting the Select button right below it.


Now if you have all the groups you want to be assigned to the texture, go back to the materials tab and click Assign. Assign is 2 buttons below the New button, so I'm hoping you won't need another screenshot (trying to save some space here )


Now your texture has been assigned to the model! whoop whoop!


You are now ready to begin uvmapping the model! IMPORTANT: SELECT THE FACES OF YOUR MODEL BEFORE YOU BEGIN THIS STEP! IT WILL CAUSE YOU LESS CONFUSION THAN NECESSARY! Go to Window > Texture Coordinator (Ctrl + T). A spiffy window will pop up. The view will be a bit weird, so it helps to zoom in (Scale tool AT THE BOTTOM) as well as to move the camera around (hold ctrl and left click and drag to move it).


If you wanted to map the top view, then during this next step, use top instead of front.


Now you'll see two drop down bars, and a button below it.


The top one represents the geoset selected, and the second one represents which view you want to remap to, hence the remap button.


We want to remap to the Front view, so select front, and then press remap.


Now you can simply resize your selection and move it in place over the Uvmap/texture's white lines and match them up, and then you're done!


It should look like this:


(those should be red dots, the jpg lost quality).


Now that that's done, your model is uvmapped and ready to be textured.


Save your progress so far.


To add your real texture, in paint or Photoshop just create your texture over your uvmap lines.


Whatever you color is what it will show up like exactly on the front and back sides of your model.


When you've finished texturing your skin, save it and then convert it to a .BLP using either Warcraft 3 Viewer or (from what i've heard, I don't actually use this though) MDLX Converter (?).


For exporting the Milkshape model, I would use Guesst's MDL/MDX Exporter.


In your exported MDL, after you add in your sequences and all that other jazz, use







 Code:

Textures 1 {
 Bitmap {
 Image "Texture.blp",
 }
}

 as your textures area. You can change "Texture.blp" to "whateveryouwant.blp"....meaning you can change it to whatever you feel like and that will be your path.


If you have any questions, feel free to PM me.


Good luck!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Thank you, this seems approvegable as it is factual and useful. Many of our users use Milkshape so I think they will benefit from it.

#### ElectricSaiyan

#### ElectricSaiyan

 Level 9

JoinedJul 20, 2005Messages414

Resources: 4

#### Resources
Models 2Tutorials 2

So is there any hope on this getting approved?

#### blazing-inferno

#### blazing-inferno

 Level 7

JoinedDec 29, 2006Messages365

Resources: 1

#### Resources
Tutorials 1

its pretty good, but i think it would be more useful if you were unwrapping a more complicated model. a box is simple, i think if you added a small section on how to unwrap a unit it would be much better.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I agree with Blazing. I think that more complex (it doesn't necessarily have to be a unit) UV Wrapping should be taught as well. It can just be a more distorted shape, not a whole model or anything just so we know how to properly wrap non standard primitives.

#### ElectricSaiyan

#### ElectricSaiyan

 Level 9

JoinedJul 20, 2005Messages414

Resources: 4

#### Resources
Models 2Tutorials 2

I'll get on that as soon as I have time

Any suggestions as to which model I should do it on?

I'd prefer to just import a War3 model and undo all it's uvmappings.

#### Kwaliti

#### Kwaliti

 Level 37

JoinedJun 7, 2007Messages2,394

Resources: 31

#### Resources
Models 13Icons 5Skins 7User Interface 3Tutorials 3

Looks good. But the unwrapper is poor in MS3D, I prefer LithUnwrap.

+rep

#### GoD.RoK

#### GoD.RoK

 Level 2

JoinedDec 12, 2007Messages14

omg its monkey!

#### General Frank

#### General Frank

 Level 92

JoinedNov 19, 2005Messages8,035

Resources: 369

#### Resources
Models 232Icons 124Packs 6Skins 2Ported Icons 3Tutorials 2

That's what people who use MilkShape (like me^^) could find very handy. Just make 'step and paragraphs' and I think I could approve this for it's usefiullness.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Why using milkshape? Max's better, .


Anyways good tutorial, but as with other max tutorial, you should go a lot forward than the basics of the basics. Anyone can unwrap a box, but making a good unwrap for a character or a building is something different.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I agree with Blinkboy, try adding how to UVMap more complicated textures.

Then it would probably be a bit more useful. Because not everyone will want to just make a box.

Nice job other than that.

 Why using milkshape? Max's better, .


Click to expand...

Milkshape = Cheap

Max = $$$ (unless you get it illegally)

But max is kind of worth it too.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

PurgeandFire111 said:

 I agree with Blinkboy, try adding how to UVMap more complicated textures.


Then it would probably be a bit more useful. Because not everyone will want to just make a box.


Nice job other than that.


Milkshape = Cheap


Max = $$$ (unless you get it illegally)


But max is kind of worth it too.


Click to expand...


Max 2008 costed me 80€, I'm smart and ordered an Educational edition, .


Yet I need to admit that I onteined max 5 through non-legal ways; mainly because discreet no longer selled it.

 You must log in or register to reply here.


### Similar threads

 Milkshape Attachment Modeling


HappyTauren

Feb 15, 2008

3D Modeling Tutorials
2

Replies64Views37K

Nov 23, 2018
Amigoltu

 Milkshape tutorial - A simple way to unwrap your models.


Mulgrim

Jan 23, 2010

3D Modeling Tutorials

Replies4Views14K

Jun 16, 2010
makai

 Create Models only with free tools


D a w n

Nov 16, 2008

3D Modeling Tutorials
234

Replies174Views87K

Jun 10, 2014
clockwork2

D

 Team Color in 3DsMax 5


Deleted member 126647

Mar 13, 2007

3D Modeling Tutorials

Replies0Views10K

Mar 13, 2007
Deleted member 126647

D

Locked

 Adding Texture to Mesh in Milkshape


Kahiera

Oct 5, 2009

Requests

Replies7Views3K

Oct 5, 2009
dansaDisco

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