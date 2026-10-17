---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/2d-do-it-yourself-recolor-it-yourself.277374/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# 2D Do It Yourself Recolor It Yourself
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/2d-do-it-yourself-recolor-it-yourself.277374/>
#### Ardenian

A

#### Ardenian

#  [2D] Do it yourself: Recolor it yourself

Tired and ashamed of having to bother other people doing stuff for you?


Want to become a successful modder and creator?


Read through this tutorial and you never have recolor requests again!


You don't have to draw anything, I promise!

##
 Requirements:

GIMP, a free tool and great alternative to pay-to-use 2D tools

Button Manager ( for only icon recolors)

BLP Lab ( for skins and icon recolors)

https://www.hiveworkshop.com/forums/tools-560/war3-model-editor-62876/ ( for resource extraction of Warcraft III files)

### Index

Step 1: Converting your BLP resource to a usable file type

Step 2a: Full image recolor

Step 2b: Recoloring parts of an image

Extra: Easiest and fastest way to preview changes of a skin

##
 Step 1: Converting your BLP resource to a usable file type


GIMP cannot open BLP files, we need to convert the BLP files to PNG, TGA, JPG or BMP.


Open the BLP file in the BLP Lab if it is a skin or icon, or in the Button Manager


if it is an icon. Choose 'Save as...' and select your file type and a name.

Attention, both tools don't have their conversion quality


initially set to 100%, you might want to change that before converting,


done in the settings or an option field for quality, dependent on the tool.


Open the converted file in GIMP.

##
 Step 2a: Full image recolor


In some cases, we want an icon match another one.


Here is an example:

 <->


Credit for the left icon to https://www.hiveworkshop.com/forums/icons-541/btnxrandomgreenthing-147743/


Credit for the right icon to Chen.


We want the right one match the left one.


But how do we do this ?


We have the option to fully image recolor an image:


Go to the tab 'Color' and then you see these two options in the list:

View attachment 153424 - Changes the color dependent on the current one

View attachment 153425 - Re-colors the image


What is the difference ? The top one takes your current color and allows


you to change the color, brightness and saturation.


For this tutorial, we are going to use that method.


The second, bottom one is basically the same, but applies a new color on the image.


Now let's get started. Press the View attachment 153424 field.


( the other one recolors the icon border, too, which we do not want)


Now a pop-up appears.


Focus on the three bars at the bottom, color tone, brightness, and saturation.

Move them a bit around and see how the icon changes.


But back to our icon project. Let's make the spheres similar to the reference icon.


It is a bit blue, isn't it? So we change the color tone towards blue.


Hm, our reference is bright, let's increase brightness here, too.


Now we see the reference icon has a weaker green tone, so we reduce saturation.

Result:

 <--> View attachment 153427 <-


Maybe a bit too blue, maybe not saturated enough, but I assume you understand the direction.

Pro Tip:


Always have the reference in visible range when recoloring,


saves time and nerves.


You can open multiple images at once in GIMP and drag the reference around


so it is visible even if you work on the other one.

##
 Step 2b: Recoloring parts of an image


Now things get a lot more complicated.


You have to learn about image layers in this step. Basically, an image is one layer.


However, GIMP is able to manipulate multiple layers at the same time, so


we have different images within the same image, overlapping.


We come back to this later.


We want a skin of a unit texture change its color ( don't confuse the resource type 'skin' with the skin of a creature)


Let's take an orc texture. Open the MPQ browser in the https://www.hiveworkshop.com/forums/tools-560/war3-model-editor-62876/.


For our example, I choose the Witch Doctor.


The texture is located under Units\Orc\WitchDoctor\WitchDoctor.blp in the "war3x.mpq" (Frozen Throne assets).


We want it to share the same texture as the green orcs.


Follow Step 1, use the BLP Lab.


Open the converted file in GIMP.


Now we try our recolor option

 from Step 2a:


Oh no, everything is green.


How can we prevent this and only recolor actual orc skin?


Now I introduce you to the selection tools:


You can see these fields in the tool board, located to the left.


From left to right:

Box Selection: Selects a rectangle or square

Circle Selection: Selects a circle

Freehand Selection: Create lines and connect beginning and end for a free shape

Magic Wand: Select matching image parts.


But what is that 'selection' ?


It allows you to pick out a manual chosen part of the image and manipulate it,


cut it, recolor it, move it and so on. Endless options.

Play a bit around with the different selection tools.


Find one you like and keep using it in this tutorial.


Note that for a perfect recolor, a use of all is to consider.


You might notice now every time you use a new selection tool or


create a new selection with the same tool, the selection gets refreshed.


We can use these to change that:


You find them under the tool board, where the settings for the currently selected tool


are displayed.


Change the current mode to the one to the right of the currently selected.


Now new selections are merged with the old one.


Now select all parts of the visible orc skin.

Pro Tip: Zoom in to increase accuracy. The current zoom is displayed


and can be changed at the bottom of the middle window, where the image is


displayed.


Now you should have something like this ( left one, right one for reference):


Now we cut these off and create a new layer.


To do so, hit CTRL+X, then CTRL+V.


Now, move your eye to the right board.


Now right click on the top one and select

 'Create as new layer'.


Ignore the border that appeared now on the image.


Now select the top layer, the one you just created, and hit one of the two recolor ways.


Choose your color to your liking.


When you now save your image, it will be saved as .xcf, the GIMP file type.


You need to export it to one of the previous file types ( PNG, TGA, BMP, JPG).


Use 'Export as...' in the main menu.


If it asks you to merge layers, do so.


Then re-convert it to BLP and enjoy.

##
 Extra: Easiest and fastest way to preview changes of a skin


Isn't it tiresome to import an edited texture to the world editor, save the map, close the editor and open it again ?


Luckily, there is a way to preview skins within seconds.


You can do so in both tools, the Model Editor or the https://www.hiveworkshop.com/forums/tools-560/warcraft-iii-viewer-62878/.


I am going to explain how to preview a skin in the W3 Viewer because there


the preview can be refreshed faster than in the Model Editor and it is easier for


people who haven't touched modeling yet.


To preview a skin, open the corresponding model in the W3 Viewer.


If it is a default unit, you can search the MPQs, too, but you have to manually


search the MPQ files when swapping between them.


In difference to the Model Editor, the W3 Viewer can search the currently opened MPQ ( CTRL+f).


Once you opened the model, hit CTRL+M.


Now you see a bunch of paths. One of these paths is the texture path of the


skin you edited.


Click on that path and hit 'Change...' in the middle under the list.


Search your edited skin and open it ( does not accept PNG, but TGA, BMP, JPG and BLP).


Now the old skin is replaced with your new one ( that's how the final picture in Step 2b was created).


But it comes better! You can refresh the skin with hitting CTRL+T.


This reloads the textures, including the edited one.


Work flow example:


Edit a texture in GIMP.


Export it as TGA, BMP or JPG.


Preview in the W3 Viewer, leave both GIMP and the Viewer open.


Continue working on the texture.


Export it once again to the same file as before ( over-writing it).


Hit CTRL+T to preview the changes immediately.


 Last edited by a moderator: May 24, 2016

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

Nice one, I like this tutorial. It's simple and clear. Good work.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

 Does someone know by chance how goto works in the BBCode ?


Click to expand...

I know how, It's just hard to explain it. I believe there's the explanation in the BB Code list.

#### Chen

#### Chen

 Level 25

JoinedJul 30, 2013Messages2,674

Resources: 25

#### Resources
Models 6Icons 14Packs 1Skins 4

Nice tutorial, Ard! Pretty simple, yet this could be very useful for many people. Good job!

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

A very nice and useful tutorial, Ardenian! I've always wanted to learn that

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Approved! Great job!

#### KeepitMovinBaby

#### KeepitMovinBaby

 Level 6

JoinedJun 5, 2015Messages185

Ooh! Ardy! Thanks! Nice tutorial

#### OnyxTheDark

#### OnyxTheDark

 Level 8

JoinedJun 15, 2013Messages122

Resources: 5

#### Resources
Maps 5

This tuto seems to be really nice, sadly all the pictures are missing so I can't really understand much about what to do

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Thanks for the heads up Onyx. I'm going to move this to the graveyard.

#### OnyxTheDark

#### OnyxTheDark

 Level 8

JoinedJun 15, 2013Messages122

Resources: 5

#### Resources
Maps 5

Okay ..., and can you link another tuto on how to recolor icons?

Would appreciate a lot

 You must log in or register to reply here.


### Similar threads

 Making An Icon


Healingpoison

Sep 1, 2006

Tutorial Archive

Replies14Views6K

Dec 23, 2006
Ezikielth

 CnP through Paint


Gausslander

Mar 12, 2007

Tutorial Archive

Replies7Views3K

Mar 14, 2007
Wolverabid

 Creating an icon using simple programs.


Hanza-Ru

Jun 18, 2009

Tutorial Archive

Replies13Views6K

Jun 28, 2009
Pyritie

[2D Art]
 Making Hide buttons


GaLaxY_256

May 16, 2014

Tutorial Archive

Replies9Views4K

Jun 4, 2018
PurgeandFire

 WC III Icons For Windows and Linux


DesKaladA

Sep 6, 2007

Tutorial Archive

Replies10Views10K

Aug 9, 2008
hive01

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