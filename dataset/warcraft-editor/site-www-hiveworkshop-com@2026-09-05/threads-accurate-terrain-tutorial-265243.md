---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/accurate-terrain-tutorial.265243/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Accurate Terrain Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/accurate-terrain-tutorial.265243/>
#### GolluM_KoMe

#### GolluM_KoMe

 Level 16

JoinedJun 15, 2008Messages677

Resources: 10

#### Resources
Models 2Skins 1Maps 5Templates 1Tutorials 1

Accurate Terrain Tutorial

This is a tutorial to help you making an accurate terrain from an existing map (of Europe, Azeroth, Middle-Earth, whatever). It is simply explaining a technique I have been using for my own maps.

1/ Crop the picture you want to use as a pattern for your map. You should use the most detailed and clear picture you can find or you may have troubles later.

2/ Open Warcraft Editor and try to make the minimap as big as possible, and then measure its size in pixels. I was able to make it 280*280 pixels, but it may be different with your screen. Then rescale your picture IN PIXEL to make it fit exactly the minimap.

3/ Open the picture and place it exactly above the editor’s minimap. This can be annoying but is absolutely crucial. You can also check if it fits pixel wise. It is easier if your picture is a square.

4/ Now is the time to draw the sketch of your map, simply putting tiles in particular places so you can locate precisely the important "borders" of your map.

Mouse over a specific part of the map like a point on the coast, a point of a river, the limit of a mountain, of a lake … This part should be pixel-accurate, so you should use the windows zoom to help you. It will be extremely hard to point at the right pixel if the picture you used is crappy.

Once your mouse is at the right spot, press alt+tab which will switch to the editor (without moving your mouse AT ALL). Click on the minimap which will direct your camera to the right location. Then just put a tile at this spot. To be sure to apply the tile at the very exact spot, you should zoom in at the maximum in your editor.

Alt+tab again to go back to your picture.

5/ Repeat this as many times as needed to draw a usable sketch thanks to the tiles you put. Don’t feel obliged to point at every single pixel of a coast to draw it .

6/ Once it is done, a good thing can be to draw continuous borders by linking the tiles. You can then see if it fits the original and arrange it if not.

7/ Dig up the oceans, the rivers and build up the mountains!

Tips :

-First make sure that when you have water under your terrain at the beginning so you only have to dig to form the oceans/rivers. Also make sure that the height difference (z) between the ground and the water levels (the level 0) is not too high, which gives troubles when digging.

-Prepare your eyes before starting, it can be really painful to go in the pixel world .

Alternative :

I believe it is possible to turn your editor’s minimap into the picture itself (no more alt+tab required), but you couldn’t see your progress in the actual minimap (an easy way to check if your work seems appropriate).


 Last edited: May 2, 2015

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Useful but I am having trouble understanding #4. I could imagine this as a video tutorial being quite successful too. (if not better!)

#### GolluM_KoMe

#### GolluM_KoMe

 Level 16

JoinedJun 15, 2008Messages677

Resources: 10

#### Resources
Models 2Skins 1Maps 5Templates 1Tutorials 1

I rewrote 4/ Maybe is it better ?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Not really. I fail to see how you can click on a .png file to select where I want to apply water or whatever.

#### GolluM_KoMe

#### GolluM_KoMe

 Level 16

JoinedJun 15, 2008Messages677

Resources: 10

#### Resources
Models 2Skins 1Maps 5Templates 1Tutorials 1

I wrote that you only mouse over a pixel of the picture, then press alt+tab which will put the editor in the front (front window on your screen) and THEN you click (your mouse should then be over the minimap) which brings you to the right location.

#### Pinzu

#### Pinzu

 Level 15

JoinedNov 30, 2007Messages1,202

Resources: 7

#### Resources
Spells and Systems 5JASS 1Tutorials 1

You can also use regions and divide your picture into blocks. But it seems to not be as accurate!

#### Wasabee

#### Wasabee

 Level 19

JoinedOct 7, 2014Messages2,208

Resources: 2

#### Resources
Maps 2

You can use height maps right?

#### GolluM_KoMe

#### GolluM_KoMe

 Level 16

JoinedJun 15, 2008Messages677

Resources: 10

#### Resources
Models 2Skins 1Maps 5Templates 1Tutorials 1

 You can also use regions and divide your picture into blocks.


Click to expand...

Wow ^^ use regions seems exhausting, I never thought about this one

 You can use height maps right?


Click to expand...

That would be awesome ! But how would you apply it to warcraft directly ?

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


Have you taken a look at this tool: https://www.hiveworkshop.com/forums/tools-560/terrain-1-0-a-62919/?

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

I have a feeling this could be quite useful:

http://www.abstractpath.com/powermenu/

Just right click on your image and set it to "always stay on top" and you won't have to alt-tab.

And you can set transparency if you need that.

#### GolluM_KoMe

#### GolluM_KoMe

 Level 16

JoinedJun 15, 2008Messages677

Resources: 10

#### Resources
Models 2Skins 1Maps 5Templates 1Tutorials 1

Really nice idea !!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It is hard to visualize a tutorial like this without a video. I'm not quite sure what your workspace looks like. It is cool as an idea/suggestion, but I don't understand your technique specifically. If you could get screen-recording software and post a video of it on youtube, it'd be a lot more useful.

I suppose you can also try to make it clearer in the text or by using more images. At the moment it is unclear, imo.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Using a heightmap converter is faster and easier.

Just take an existing image of the landscape of the interwebs, resize it to the correct amount of pixels and import it via the tool.

#### Wasabee

#### Wasabee

 Level 19

JoinedOct 7, 2014Messages2,208

Resources: 2

#### Resources
Maps 2

I don't understand how to use heightmaps.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

apcrabnightlive said:

 I don't understand how to use heightmaps.


Click to expand...

-> Use an MPQ extractor

-> Extract heightmap from map MPQ

-> check resolution

-> make a new image in GIMP/Photoshop

-> copy your landscape into the image as a greyscale image

-> resize to resolution from heightmap

-> use the converter tool to create a heightmap file out of your image

-> import it into your map MPQ

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Gollum_Kome, anything new? Have you considered Zwiebelchen's tequnique maybe?

The images are broken now, too. We encourage to directly attach them to the post.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Submission:Accurate Terrain Tutorial

Date:28 November 2016

Status:Graveyard

Rating:none

Note:

Relate to my post above. And I honestly also agree with other opinions shared in thread, that it is hard to imagine what and how to exactly apply suggested method.

In case you're still interested to change something inthis tutorial just ping here, and share your thoughts.

 You must log in or register to reply here.


### Similar threads

 Change In-Game Minimap


WCverfrisser

Aug 15, 2007

Tutorial Archive

Replies3Views14K

Oct 7, 2007
Wolverabid

 Mini Map Image Tutorial


Daveman

May 10, 2005

Tutorial Archive

Replies26Views14K

Mar 12, 2007
Wolverabid

 How to make a Map-Preview


DarkLordX

Jun 9, 2009

Tutorial Archive

Replies8Views5K

Jun 13, 2009
Pyritie

 Basic Terraining Tutorial


Torn

Jun 9, 2005

Tutorial Archive

Replies10Views7K

Mar 27, 2007
Wolverabid

 Easy Terrain for 'Camouflaging' Units


Ivanneth

Aug 20, 2007

Tutorial Archive

Replies5Views2K

Oct 27, 2007
PurplePoot

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top