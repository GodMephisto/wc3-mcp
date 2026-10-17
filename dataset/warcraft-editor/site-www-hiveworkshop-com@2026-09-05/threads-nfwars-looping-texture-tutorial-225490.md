---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/nfwars-looping-texture-tutorial.225490/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Nfwars Looping Texture Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/nfwars-looping-texture-tutorial.225490/>
#### NFWar

#### NFWar

 Level 48

JoinedJul 27, 2008Messages1,756

Resources: 268

#### Resources
Models 1Icons 255Packs 1Skins 2Spells and Systems 7Tutorials 2

This tutorial explains how to create a texture, that will merge to itself when placed at four directions (top, bottom, left, right). You may find it useful in drawing tiles or background textures. Lets get started with example of coblestone.

Preperation


 Image must be 3 times biger than texture's original size. Texture bounds in center and rest of space for 8 more textures around for checking on smooth merging. I made 300x300 as original texture size and added 300 more pixels to each size (total 900x900 pixels image). Keep outside area white.

1.Corners


Because this is coblestone texture, it is made of random pieces/areas, that represent each rock. Draw the first one in a corner, so it will be on all 4 planes of surface. I marked surfaces. You will be moving pieces of outline in corners inside texture area like on image (hold shift in Photoshop for horizontal/vertical/diagonal movement). As you progress, notice that you broke rock outline in four lines, each touches 2 texture borders.

2.Sides


 Draw a few outlines for rocks, that will intersect a side on a new level. As you see, rocks that should be near corner rock, are missing outline from outside. If you will make whole shape for those, mostly those will intersect corner rock outline and this is a problem. Finish those rocks after moving inside shapes in white area. Be careful when moving things around. Pixel or two of misplacement may cause noticeable difference and texture will be not so nice. Repeat the same for rock outlines on top/bottom in a new layer. Whe you are done, copy the texture and place it all around 8 times to fill all background. You will see if your texture is merging from sides.

3.Insides and Texturing


 Now when you have done with sides and corners, fill the area inside with rocks. Create a new layer for that. Try to avoid acute angles or weird shapes. At the end it will be something like a net. You can make lines thicker or thiner in some places for variation.


 What I call here a texture actually is a bunch of transparent white and black brushes one top to an other. It is messy, but you can blur it for smoothness, or smudge color around, or even sharpen it more. When you are done with texture, move whatever is out of borders inside to oposide border. Check for smooth looping as well.

4.Shadows and Lights


Every artist see things different and mostly like your image by now is very different from mine. I drew sharp light from top on a new overlay layer. While shading, texture on rocks may inspire you to shade in many ways. Randomness in texturing is good. Add details like scratches, holes. I was in haste, so I may made not enough. And yes, you will need to move shading just like you did before.


Thank you for your time.


 Last edited by a moderator: Nov 16, 2012

#### I3lackDeath

#### I3lackDeath

 Level 29

JoinedApr 11, 2009Messages1,453

Resources: 18

#### Resources
Models 1Icons 10User Interface 7

Oh wow, this is incredibly neat.

Crisp style, as usual, and a detailed step by step process, starting with a very interesting technique of actually starting looping textures.

We really never stop learning.

I feel like the same technique of looping can be used for creating borders for custom UIs (cinematic and option menu borders), but I can not wrap my head around that.

Do you have any clues or experience with that application?

#### NFWar

#### NFWar

 Level 48

JoinedJul 27, 2008Messages1,756

Resources: 268

#### Resources
Models 1Icons 255Packs 1Skins 2Spells and Systems 7Tutorials 2

I have very little idea about how UI in WC3 works.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great tutorial. Useful technique.

Approved.

#### Golden-Drake

#### Golden-Drake

 Level 24

JoinedAug 1, 2009Messages620

Resources: 173

#### Resources
Icons 165Packs 2Maps 2Spells and Systems 4

can i report NFWar for making posts too awsome??? 0.0

#### NFWar

#### NFWar

 Level 48

JoinedJul 27, 2008Messages1,756

Resources: 268

#### Resources
Models 1Icons 255Packs 1Skins 2Spells and Systems 7Tutorials 2

I am planing to release a tutorial which will explain how to create Tiled Texture, which is made from many objects. Useful for creation of complicated textures like floor full of garbage or in my case autumn fallen leafs.

I made 4 variations so far using few variations of leafs (15) and 4 variations of sticks/twigs. Just from beginning I realized, that placing leaf by leaf is butthurt process. I don't think that Photoshop have a function, which places random variation of objects around.

#### Blood Raven

#### Blood Raven

 Level 47

JoinedAug 1, 2010Messages1,740

Resources: 235

#### Resources
Models 144Icons 53Packs 7Skins 13Maps 16Templates 1Tutorials 1

You would make a perfect minecraft texture pack artist  well, you're a perfect artist anyway.

Really neat and useful tutorial, man, plums for you

 You must log in or register to reply here.


### Similar threads

 NFWar's Tutorials


NFWar

Sep 15, 2010

2D Art Tutorials

Replies41Views37K

May 12, 2017
plazma019

 Polished Iron Texture


~Void~

Jan 23, 2009

2D Art Tutorials

Replies12Views9K

Jan 28, 2009
~Void~

Locked

 Basic Photoshop Tutorial


Skudgemuffin

Jul 18, 2007

2D Art Tutorials

Replies10Views15K

Sep 6, 2007
Wolverabid

 Anti-Noobism Texturing


Hawkwing

Jul 25, 2008

2D Art Tutorials

Replies14Views17K

Nov 12, 2008
Hawkwing

 Direct Size Icon Making Tutorial (Part 1)


Darkfang

May 11, 2014

2D Art Tutorials

Replies4Views5K

Jun 4, 2018
Darkfang

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