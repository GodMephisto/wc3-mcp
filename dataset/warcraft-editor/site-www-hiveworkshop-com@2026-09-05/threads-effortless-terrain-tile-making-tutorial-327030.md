---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/effortless-terrain-tile-making-tutorial.327030/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Effortless Terrain Tile Making Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/effortless-terrain-tile-making-tutorial.327030/>
#### Strydhaizer

#### Strydhaizer

 Level 26

JoinedFeb 28, 2013Messages1,904

Resources: 8

#### Resources
Models 2Maps 1Templates 3Ported Skins 1Tutorials 1

Effortless Terrain Tile Making Tutorial

Note: This is for Classic/SD only, Reforged/HD tiles are kinda more complicated since it have three layers therefore it is not included in this tutorial.

Description

 Have you ever wanted to make a tile, but you don't know how to? Worry no more, I will be teaching you how to effortlessly make a tile by yourself. I do personally use this method in my tiles so this has been tested multiple times. This same transparency (that I will provide in the tutorial if you follow) have been used by multiple tile makers back in the day (tobyfat, Nan0, WerbackIII) because of its cool "blend/blurring transition" effect.

Tools

 What tools do we need? Well, GIMP. It's a free tool that you can download here - GIMP - Downloads. You will also need blplab for conversion.

Steps/Procedures:

1. Open GIMP

 After downloading the GIMP you have to fire it up.

2. Finding a tile-able/seamless texture

 So we'll need to find a tile-able seamless texture, you can search for tile-able/seamless textures online although be careful as some images online might have copyright. If you're a professional artist, you can probably make a seamless texture yourself.

 The texture I will be using in this tutorial is from textures.com, you can get free real and tile-able textures from that website.

3. Importing the image

 Now that you have a seamless texture, you will now have to import it to GIMP. You can either go to your desktop and right-click the file, click "Open With" and choose GIMP, or alternatively in GIMP you can just go to File (at the Toolbar) > Open and find the image you downloaded.

The desktop method.

Or alternatively opening it in the GIMP.

This is what it looks like in the canvas.

4. Resizing the image

 In order to be able to use the tile transparency that I provided in the attachments, you will have to decide what texture resolution you want.

If you want a 1024 x 1024 tile texture, you will have to resize the image to 1024 ÷ 4 = 256 (256 x 256)

If you want a 512 x 512 tile texture, you will have to resize the image to 512 ÷ 4 = 128 (128 x 128)

If you want a 256 x 256 tile texture, you will have to resize the image to 256 ÷ 4 = 64 (64 x 64)
 It is divided by 4 because Warcraft 3 tiles are tiled in a 4 by 4 pattern (basic pattern). Warcraft 3 also supports 8 x 4 pattern (for tiles with variation) but I am only teaching you to make basic (no variation) tiles in this tutorial.

 In this tutorial, I want a 512 x 512 texture, so I will be resizing it to 128 x 128. To resize the image in GIMP, go to Image (at the Toolbar) > Scale and input 128 x 128.

This is what it looks like when resized, obviously lots of quality have been lost, we can't do anything about that; unless you decided to make a higher resolution tile.

5. Tiling the image

 After Step 4 your texture is now ready to be tiled in a 4 by 4 pattern. In order to tile this image simply go to Filters (at the Toolbar) > Map > Tile.

 Remember what resolution you have picked in Step 4. In this tutorial, I mentioned that I will be making a 512 x 512 tile texture so the tiling size should be 512 x 512 image. If you want to make a 1024 x 1024 tile then input 1024 (width) x 1024 (length), if 256 x 256 then input 256 x 256.

6. Importing transparency to GIMP

 Now that you have a texture in a 4 by 4 pattern, it's now ready to have transparency, so you can use it in the game.

Download the image with transparency (provided below) and load it in the GIMP.

If you decided to make a 256 x 256 tile, download this. https://www.hiveworkshop.com/attachments/standard_tiles_alpha_256-tga.363358/

If you decided to make a 512 x 512 tile, download this. https://www.hiveworkshop.com/attachments/standard_tiles_alpha_512-tga.363359/

If you decided to make a 1024 x 1024 tile, download this. https://www.hiveworkshop.com/attachments/standard_tiles_alpha_1024-tga.363363/
Like I mentioned in previous steps I am making a 512 x 512 tile so I downloaded and imported the 512 x 512 transparent image.

 Now you will have to copy the transparency of the image (provided by this tutorial) to your tile. In order to do that, go to Layer (at the Toolbar) > Mask > Add Layer Mask then pick "Layer's Alpha Channel"

Now do the same in the tile you created.

 Now that both have layer masks, go back to the image with transparency that you downloaded, and copy the transparency from it, then paste it to your tile. To copy the transparency, be sure to select the mask of the image. I marked it red in the screenshot below so you can see it.

 After selecting the mask, hit CTRL + C to copy it.

 Now go to your tile, then select the mask aswell. I marked it red just in case. Then paste the transparency mask by hitting CTRL + V. It should look like this.

Before pasting

After pasting

 Now anchor the mask you copied to your tile.

Now it's done!

7. Exporting

 Now that you successfully made a tile, you can now export it (I recommend exporting it to targa format/.tga format). Just name your exported file to "<filename>.tga" or alternatively look for the targa extension under "Select File Type (By Extension).

8. Converting the file and testing the tile ingame/in the world editor

 You can use blplabto convert targa/.tga image to .blp or .dds to be able to use it ingame. Here's what it looks like ingame (along with another tile I converted). You can download the test map here.


 Last edited by a moderator: Oct 29, 2020

#### Strydhaizer

#### Strydhaizer

 Level 26

JoinedFeb 28, 2013Messages1,904

Resources: 8

#### Resources
Models 2Maps 1Templates 3Ported Skins 1Tutorials 1

Please let me know if I need to revise it. English is not my mother-tongue so there might be some broken grammar at some parts of this tutorial.


 Last edited: Sep 3, 2020

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Excellent guide for tile-set beginners!

Your grammar was good enough to read though with no confusion, though I did not extensively look for errors.

The only thing you could revise really is at the top you have listed "Also you will need blplab for conversion."

While later you have listed "You can use blplabto convert targa/.tga image to .blp or .dds to be able to use it ingame."

Blplab is not required to convert to .tga or .dds, which I would suggest both over .blp

Gimp, Photoshop and Paint.net can all handle .dds / .tga formatting, and blplab is not a required tool.

It's optional however, if you're looking to use this method for maps patched pre-1.32 (which do not support .dds, but they do support .tga)

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I fixed a few grammar errors.

Otherwise solid.

My one nitpick would be that a tool list looks better.

Tools

Gimp - link here

blp lab - link here

Approved.

#### Symphoneum

#### Symphoneum

 Level 29

JoinedAug 14, 2020Messages263

Resources: 37

#### Resources
Maps 1Reforged HD Models 20Ported Models 15Reforged HD Skins 1

Thanks for your tutorial, it was really useful! I have made some templates in the same style as yours for use in creating grass and dirt cliff textures. Below is a pastebin link for the files:

###
 Natural Cliff Alpha Template 1024


 www.hiveworkshop.com


#### Dark-Zalor

#### Dark-Zalor

 Level 19

JoinedOct 10, 2011Messages580

Resources: 28

#### Resources
Maps 25Spells and Systems 2Sounds 1

A big thanks man, You saved my life

#### P3in

#### P3in

 Level 11

JoinedDec 16, 2017Messages437

Resources: 7

#### Resources
Packs 7

Just a quick one, can you make more than 16 tilesets? I came across this..that i cannot add more..can somehow the textures of the tilesets be added to a doodad with mushroom pathfile or even none and it would appear as a tileset, even if it is not treated as an tileset by WE?

#### RatzRatzzz

#### RatzRatzzz

 Level 15

JoinedJan 3, 2022Messages144

Resources: 11

#### Resources
Templates 1Ported Models 9Ported Skins 1

Thank you so much! That's really great article for begginers. Thanks to you, I finally managed to master creating tiles!  and thank you for tha alpha-channels templates!

 You must log in or register to reply here.


### Similar threads

Locked

 Terrain Tile size limit


oger-lord

Mar 3, 2014

World Editor Help Zone

Replies3Views2K

Mar 7, 2015
oger-lord

Locked

 Create ALL trees using triggers/JASS Code per line limit


D4RK_G4ND4LF

Sep 21, 2009

World Editor Help Zone

Replies5Views2K

Sep 25, 2009
D4RK_G4ND4LF

Locked

[Template / Terrain]
 256 x 256 Terrain Tile


A Void

Jun 29, 2014

Map Development

Replies11Views3K

Jul 8, 2014
A Void

Locked

 About Loading Screens


DaDDyWitch

Feb 19, 2014

World Editor Help Zone

Replies4Views2K

Feb 23, 2014
DaDDyWitch

Locked

 UI Tile 4 not displaying...


AL0NE

Mar 17, 2011

Texturing & Surfacing

Replies15Views3K

Mar 25, 2011
AL0NE

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