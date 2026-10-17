---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/putting-what-you-learned-about-animated-textures-into-action.273627/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Putting What You Learned About Animated Textures Into Action
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/putting-what-you-learned-about-animated-textures-into-action.273627/>
#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

Tutorial difficulty:ADVANCED

Tools required:

- Magos's Model Editor

- MDLvis

- Any image editor that supports transparency

- MDX/L converter and text editor

For the purpose of this tutorial, I will assume that you have understood how to make animated textures from the Animated Textures Tutorial, and is also able to navigate through Magos's Model Editor, as well as MDLvis's functions.

The example I will make today is a Faceless One who has harnessed the power of water, and as a result his limbs are made of flowing water...

At this point you've probably thought this could be easily done by putting an animated water texture layer below the Faceless One's body texture layer. Well, not so fast. Although this will work flawlessly in the Model Editor, WC3's engine limitations make the FO's body texture also animate in-game which is not what we want. That's why we will have to do this the ghetto way.

In WC3, for a texture to animate it needs to be the first rendered layer of a material.

However, animating the first rendered layer will also move all layers above it in the material.

Step 1:

- Extract the Faceless One model & texture using Model Editor's MPQ browser.

Units\Creeps\FacelessOne\

- Edit the extracted texture, make the areas of the FO's limbs transparent. Since we need his limbs animate after all.

Convert it back to .blp afterwards

Step 2:

- Open the Faceless One's model in MDLvis. Select the vertices for the Faceless One's limbs.

- Copy and paste the vertices. Move the duplicated vertices back to the correct position of the original vertices.

You can move the duplicated vertices to the EXACT correct position by raising their Z-axis coordinate by 5 units. This is because MDLvis always creates duplication of a vertex 5 units (on Z axis) below it.

- With the duplicated vertices still selected, click "Reverse normals" (-1 icon). Then click "Detach as new geoset". NOTE: You need to do in this exact order.

- Save the model.

Step 3:NOTE: This can be done with MDX/L converter and text editor optionally.

- Open the edited FO model in Magos's Model Editor. Import (or create) a water texture with the Texture Manager.

I use Textures\Lords0000.blp because it's the texture for Water Elemental. You will also need to open this water texture's properties and tick on "Wrap Width" and "Wrap Height".

- Create a new material with the water texture. Use the Material Manager and Geoset manager to find the limbs geoset and align it to the water material.

Step 4:

- It's time to animate the material. Follow the Animated Textures Tutorial to make the water flow.

Animate a texture on a completely new material with what is basically the geoset with reversed normals.

#### Attachments

WateryOne.zip

 233.9 KB

 · Views: 345



#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Thanks a ton for making this. I've been inactive, so sorry for letting this sit for so so long. I'm going to add this as a link in the animated textures tutorial, and approve it.

#### CopyDog

#### CopyDog

 Level 2

JoinedJul 28, 2025Messages8

Some models do not require the animated texture be the first rendering layer of a material.

Like Scarab(back shell) and CryptLord(wing).

But I can't find the differences between these materials.

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

CopyDog said:

 Some models do not require the animated texture be the first rendering layer of a material.

Like Scarab(back shell) and CryptLord(wing).

But I can't find the differences between these materials.


Click to expand...

It's been found by @OVOgenez that the order of the layers don't really matter, but by giving layers different alpha values, you can allow the engine to identify which material layer to animate.

 I managed to figure out what the issue was.

The problem is that the Warcraft engine doesn't handle material layer identification well, especially when the layers have identical parameters.

The issue was resolved by giving the layers slightly different alpha values - for example, 1.00 and 0.99 for the two layers respectively.


Click to expand...

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

 Animated Textures Tutorial


Amigurumi

Feb 9, 2010

3D Modeling Tutorials

Replies39Views49K

Dec 24, 2024
Greedy Martyrdom

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

 The Crystal Effect


olofmoleman

Dec 9, 2006

3D Modeling Tutorials

Replies9Views33K

Dec 10, 2006
operator

 How to make cool portraits without needing a separate file


Spacebuns

Sep 5, 2014

3D Modeling Tutorials

Replies11Views11K

Nov 27, 2017
Diegoit

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