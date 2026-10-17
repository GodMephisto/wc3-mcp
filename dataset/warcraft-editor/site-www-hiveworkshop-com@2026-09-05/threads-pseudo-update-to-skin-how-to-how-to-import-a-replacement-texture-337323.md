---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/pseudo-update-to-skin-how-to-how-to-import-a-replacement-texture.337323/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Pseudo Update to Skin How to How to Import a Replacement Texture
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/pseudo-update-to-skin-how-to-how-to-import-a-replacement-texture.337323/>
#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

Description

In Warcraft 3, it is possible to replace a model's texture in a map you make. For the purpose of this tutorial, we are going to replace the Orc Watchtower's original texture with a custom one. When importing a replacement texture, it is paramount to put the asset in the correct path so the game knows what it's for. Just leaving the texture in the war3MapImported folder will not create any visible changes to the map.

Finding Replacement Textures

You can download replacement textures from many places on the internet. A quick Google query might be very successful. For this tutorial I'm going to use the Watchtower.blp by chr2. You can download it yourself from here.

You can download more replacement textures from the section right here on the Hive Workshop.

There used to be many other sites that hosted replacement textures, but most have since gone out of order.

If not creating content for the latest version of the game, make sure the replacement textures are not in the dds format as it is only supported by 1.32+.

Importing

Open up the Asset Manager (F12) and import the replacement textures you wish to use.

What is important here is the path, so you'll need to find out the correct one for each texture you replace.

For this particular Watchtower texture, it's Textures\Watchtower.blp, and all replacement textures from Hive will have the import path specified on their pages (the "contents" part of the main post). For those new to the asset manager, double-click the filename field to change the path.

If you've not placed the object before, as soon as you place it you'll be able to see how it looks with the replaced texture. If it still looks the same, try saving the map and re-opening World Editor as assets get cached.

Finding Unknown Paths

When getting replacement textures from other sources, there will probably either be a readme file to consult, or you may have to find the path out on your own.

To find the path of a texture on your own, you will need a game data browser and a model editor. (Retera's Model Studio is recommendable for this as it provides both capabilities AND has a unit browser, so you don't have to search for the model files either, for older patches you can also use W3ME in conjunction with CascView)

Whatever model editor you prefer, look at the "textures" the model has, all the paths you it uses are there, and the filename is often a good indicator of which texture is most likely the one your custom one goes over.

End result

If you imported the texture correctly, the Watchtower will look like this in-game


 Last edited: Jan 6, 2022

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I think that if the old/current tutorial should be updated we should do a better job than this.

For starters, I think it's way too long.

The following parts can be removed entirely, they contain very obvious information.

BogdanW3 said:

 You can download replacement textures from many places on the internet. A quick Google query might be very successful.


Click to expand...

BogdanW3 said:

 You can download more replacement textures from the section right here on the Hive Workshop.


Click to expand...

BogdanW3 said:

 There used to be many other sites that hosted replacement textures, but most have since gone out of order.


Click to expand...

I think the following can be shortened in at least half. I have only done it with Magos model viewer but it's literary:

1. launch the program

2. Select a model

3. Open the texture list

4. Read

BogdanW3 said:

 When getting replacement textures from other sources, there will probably either be a readme file to consult, or you may have to find the path out on your own.

To find the path of a texture on your own, you will need a game data browser and a model editor. (Retera's Model Studio is recommendable for this as it provides both capabilities AND has a unit browser, so you don't have to search for the model files either, for older patches you can also use W3ME in conjunction with CascView)

Whatever model editor you prefer, look at the "textures" the model has, all the paths you it uses are there, and the filename is often a good indicator of which texture is most likely the one your custom one goes over.


Click to expand...

The structure is also very boring, it looks sloppy and is not nice to read. Try with more linebreaks and h3 tags for titles. Colors could also be a nice addition.

 You must log in or register to reply here.


### Similar threads

 HOW TO IMPORT MODELS - by Mecheon


DeadDogNoGoWoof

Oct 13, 2005

Tutorial Archive

Replies30Views9K

Nov 5, 2006
Mecheon

 Import Custom Textures and Create/Edit custom models for Newbies!


Statharas

Aug 6, 2008

Tutorial Archive

Replies6Views21K

Oct 21, 2008
Hawkwing

Locked

 How to replace a specific cliff texture?


Deon

Feb 21, 2009

World Editor Help Zone

Replies1Views2K

Feb 21, 2009
Vegavak

 How to: Import a Skin


Archian

May 5, 2006

2D Art Tutorials

Replies14Views71K

Dec 28, 2020
BogdanW3

[2D Art]
 How to use skin with same type of model


rysnt11

Dec 16, 2010

Tutorial Archive

Replies1Views7K

Dec 23, 2010
PurgeandFire

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