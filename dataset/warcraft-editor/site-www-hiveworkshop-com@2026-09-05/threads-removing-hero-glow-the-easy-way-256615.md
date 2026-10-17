---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/removing-hero-glow-the-easy-way.256615/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Removing Hero Glow the Easy Way
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/removing-hero-glow-the-easy-way.256615/>
#### UlithiumDragon

#### UlithiumDragon

 Level 3

JoinedMay 5, 2013Messages20

Resources: 1

#### Resources
Tutorials 1

#

 Removing Hero Glow - The Easy Way

By Ulithium_Dragon

## Overview

###
 Difficulty

2/10 Easy difficulty

###
 Tools Needed

GIMP (Get the Latest Version)

Or Photoshop, but this tutorial uses GIMP because it's free

Warcraft 3 Viewer

War3 Model Editor

World Editor

###
 What You're Expected To Already Know

How to navigate folders and open files

How to navigate program tabs and sub menus (for World Editor and War3 Model Editor)

How to follow some simple directions without a picture (all but the most basic steps have pictures)

###
 Contents

Section 1: Introduction

Section 2: Creating and Exporting the Blank Texture Using GIMP
Section 2.1: Creating the Texture

Section 2.2: Exporting the Texture

Section 3: Converting the .tga File to .blp Using Warcraft 3 Viewer

Section 4: Replacing the Hero Glow Texture With Your New Transparent One
Section 4.1: Setting up War3 Model Editor

Section 4.2: Importing the Transparent Texture and Replacing the Hero Glow

Section 4.3: Saving a New Portrait Model (Optional)

Section 5: Importing the New Model Into World Edit and Setting It Up
Section 5.1: Importing the Models and New Texture

Section 5.2: Editing the File Path for the Imported Files

Section 6: Changing to the No Hero Glow Model

##
 Section 1: Introduction

I decided to do this after reading this post which gives a link to this tutorial.

However, the method used is both complicated, and requires coding-esk editing of the .mdl file.

In this tutorial, I will show you how to create a simple "blank" texture with nothing but an

alpha (transparent) channel which we will use to replace the hero glow texture.

For purposes of this tutorial, I will be using the Priestess of the Moon model to demonstrate how to do this.

If you wish to use a base game model, you will have to extract it yourself.*

*There are plenty of tutorials available on how to do this, so I won't be going over it in this tutorial!

Here is the model with the Hero Glow:

At the end of this tutorial, the model will look like this:

*If you want to just use the transparency texture file I made, you can download it at the bottom of this post and skip to Section 3.

##
 Section 2: Creating and Exporting the Blank Texture Using GIMP

By this point you should already have GIMP installed. If you don't then you can download it here.

###
 Section 2.1: Creating the Texture

Open up GIMP and click File -> New.

 In the new window, expand Advanced Options, and change the drop down menu for Fill with to Transparency. A new window will pop up with the settings we used.*

*If you already had a file open in GIMP (like what's seen in the following video clip). If you didn't have any files open in GIMP, it will use the main GIMP window that opens when GIMP does. Both are the same.

Also, any size texture should work fine for this, but to avoid any issues I went with the standard 64x64.

###
 Section 2.2: Exporting the Texture

Now it's time to export the new texture. Click File -> Export.*

*Make sure you have the correct GIMP window Selected!

A new window will pop up. Save the file in a format recognized by Warcraft 3 Viewer (I just used .tga). The default Export setting should be fine.

###
 Section 3: Converting the .tga File to .blp Using Warcraft 3 Viewer

Now we are going to convert the texture we just exported from a .tga file to a .blp file. While World Editor supposedly does support .tga files, we're going to be on the safe side here and convert our texture to a .blp file (a format created for and used exclusively by Warcraft 3).

Open up Warcraft 3 viewer (download link here), and in the Convert Files dropdown menu, select BMP, TGA, or JPG -> BLP (the second one from the top). Or just press CTRL+V (Windows).

A new window will pop up. Open up the file location of where you exported your .tga texture, select it and hit Open

In the next window, make sure your file now has a .blp file extension (this should be done automatically), then hit Save.

A new window will pop up, just keep the default settings the same and hit Ok.

When it's finished being converted, one final window will pop up saying it was successful. Just hit Ok to close it.

*Note that your model DOES NOT need to be open to convert files this way, mine was just still loaded when I recorded this!

##
 Section 4: Replacing the Hero Glow Texture With Your New Transparent One

At last it's time to get rid of that annoying hero glow! Open up your desired model using War3 Model Editor (download link here).

###
 Section 4.1: Setting up War3 Model Editor

You now need to open up two extra windows from within War3 Model Editor. Select the Windows drop down menu, and then select both Texture Manager and Material Manager.

###
 Section 4.2: Importing the Transparent Texture and Replacing the Hero Glow

Right click anywhere on the Texture Manager window and select Import. Navigate to where you saved your transparency .blp texture (it will have been saved to the same folder as your .tga texture), select it and hit Open.

Now double click on the material you need to change in the Material Manager window (you're looking for any material layer using Replacable ID 2. In most cases a model won't have too many materials or materials layers, so just checking them all is easiest).

A new window will pop up. Double click on the material sub node that uses Replacable ID 2 and another new window will pop up called Material Layer.

In the Material Layer window, select the drop down menu under the Texture ID section, and select your transparent texture. Now hit Ok to close the Material Layer window, then Ok again to close the Material window. If you did this correctly, the hero glow should now be gone!

Now save the model with File -> Save if you wish to overwrite the old model, or File -> Save As if you don't want to overwrite the model (make sure to give it a different name).

###
 Section 4.3: Saving a New Portrait Model (Optional)

*This part is only necessary if you saved your model under a new name, and if your model has a portrait model included with it!

Open up the original portrait model using War3 Model Editor, then select File -> Save As and give it the same name as your normal model file but with a _portrait suffix (this is NOT OPTIONAL - it's just how the game searches for portrait models for the unit models).

Note that in some rare cases (such as the Priestess of the Moon model), the hero's weapons have a hero glow effect, AND are within the camera view of the portrait model. If this is the case and you want the hero glow removed from the weapons in the portrait, too, then simply re-do Section 4.2 again with the hero portrait model.

##
 Section 5: Importing the New Model Into World Edit and Setting It Up

Now that we have a working model with no hero glow, let's get it to work in World Edit! So open up World Edit and select your desired map.

###
 Section 5.1: Importing the Models and New Texture

Open up the Import Manager. This can be done by using the icon on the top bar (the second one in from the right that as a little file icon), selecting Module drop down menu, then selecting Import Manager, or by simply hitting F12 (Windows).

Once open, right click anywhere on the blank space and select Import File....

A new window will pop up. Navigate to where you saved your model file and select Open. You will need to do this for the transparency texture file, and the portrait model file.

###
 Section 5.2: Editing the File Path for the Imported Files

By default, all imported files are given the file path war3mapImported\. Depending on the what the model/texture you're importing is used for, you may need to give it a different file path, but if I'm not sure, I just use a basic file path and remove the war3mapImported\ part.

Right click on each of your imported files and select Modify File Properties..., then check the checkbox use custom path, and delete the war3mapImported\ part. When you're done, hit Ok. Do this for each file you imported!

*WARNING: If you chose to simply overwrite the original model, then you will need to input the proper file path that the original model used! For example, the Priestess of the Moon model uses the file path Units\NightElf\HeroMoonPriestess\.

##
 Section 6: Changing to the No Hero Glow Model

*Before you start this part, you should save your map, close World Edit, restart it, and re-open your map. This is because World Edit seems to need to be restarted in order for it to recognize the new models and textures inside the editor (although they seem to appear just fine when using the "Test Map" button in World Edit).

I would guess it has something to do with World Edit finalizing or indexing the MPQ file when it's closed, but I'm not sure. Note that simply reloading the map doesn't work, you need to fully exit World Edit!

Now to tell the game to use the model without hero glow! For this tutorial, I will be replacing the normal Priestess of the Moon model with our new no hero glow version.

Open up the Object Editro. This can be done by hitting the icon in the top bar that looks like a little helmet, selecting Module drop down menu, then selecting Object Manager, or by simply hitting F6 (Windows).

Select the hero you wish to edit in the left pane of the Object Manager window, and in the right pane, double click on the line that says Art - Model File.

A new window will pop up named Edit Unit Value - Model. Check the selection circle next to Import:, then find your model in the drop down menu and select it. When you're done, hit Ok.

----

If you followed this tutorial properly, your new model should have been successfully imported with the hero glow removed:

I hope this tutorial was both helpful and informative!

If you have further questions or run into any problems, post a comment bellow, or send me a PM!

#### Attachments

Transparency.7z

 544 bytes

 · Views: 329



[00]_MoonPriestess_WithHeroGlow.gif

 224.4 KB

 · Views: 1,418



[01]_GIMP_MakeTransparentTexture.gif

 369.8 KB

 · Views: 1,380



[02]_GIMP_Export_P1.gif

 64.8 KB

 · Views: 1,414



[03]_GIMP_Export_P2.gif

 341.4 KB

 · Views: 1,571



[04]_Warcraft3Viewer_Convert_TGA_To_BLP.gif

 1.1 MB

 · Views: 2,214



[05]_War3ModelEditor_Import_Texture.gif

 782.4 KB

 · Views: 1,606



[06]_War3ModelEditor_Change_Texture_To_Transparency.gif

 718.4 KB

 · Views: 1,449



[07]_War3ModelEditor_Save_New_Portrait.gif

 1.1 MB

 · Views: 2,011



[08]_WorldEditor_Import_New_Files.gif

 1 MB

 · Views: 1,352



[09]_WorldEditor_Change_File_Path.gif

 463.1 KB

 · Views: 1,524



[10]_WorldEditor_Change_Model.gif

 867.5 KB

 · Views: 1,429



[11]_MoonPriestess_WithoutHeroGlow.gif

 194.5 KB

 · Views: 1,346




 Last edited: Sep 3, 2014

#### UlithiumDragon

#### UlithiumDragon

 Level 3

JoinedMay 5, 2013Messages20

Resources: 1

#### Resources
Tutorials 1

Let me know of any changes you think I should make before this gets approved!

EDIT: In case anyone was interested, I made the .gif's using GifCam. It's a very nice little freeware app, and I'd happily recommend it to anyone who wants to record .gif screen captures!

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

All of the pics are not working.

#### UlithiumDragon

#### UlithiumDragon

 Level 3

JoinedMay 5, 2013Messages20

Resources: 1

#### Resources
Tutorials 1

deathismyfriend said:

 All of the pics are not working.


Click to expand...

That's odd... One sec.

EDIT: Do they show up now? As I was adding them in, they started disappearing from my imported content as I hit the "Preview Post" button, so I just assumed that they were being removed from the section because they were being used in the message content. Weird that I could still see them even after you said they weren't showing up and I refreshed the page... I re-uploaded the files and refreshed the links, so they should be showing up now!

Anyways, I'm going to bed, so I hope it's working now! :S

#### UlithiumDragon

#### UlithiumDragon

 Level 3

JoinedMay 5, 2013Messages20

Resources: 1

#### Resources
Tutorials 1

I don't usually like to artificially "bump" my threads, but since the two submissions after mine have received between 2-3x as many replies, and it's been over 2 days, and even the one guy who did respond initially hasn't gotten back to tell me if my fixes worked or not, I am...

*BUMP*

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

The images are working now.

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

You can change filter mode to transparent and the alpha value to 0 and you're done. You don't need a custom texture, also, I don't think this is the easy way.

#### UlithiumDragon

#### UlithiumDragon

 Level 3

JoinedMay 5, 2013Messages20

Resources: 1

#### Resources
Tutorials 1

Anvil said:

 You can change filter mode to transparent and the alpha value to 0 and you're done. You don't need a custom texture, also, I don't think this is the easy way.


Click to expand...


Well I couldn't find anywhere that said this, and since the only tutorial I could find involved removing sections of code from the .mdl file (for which it was a lot harder to find the required section to remove on some non-vanilla assets), I made this.

#### Nichilus

#### Nichilus

 Level 34

JoinedSep 26, 2009Messages2,893

Resources: 2

#### Resources
Spells and Systems 2

Your tutorial ain't bad, but the tutorial at thehelper is better. The reason for that is because they remove the unneeded. In this case, they remove the hero glow which is unwanted. Your tutorial suggest that we leave it but make it invisible. But the thing is - if it can't be seen, why leave it at all when it serves no purpose, right? You also need to import another texture (albeit it will be of very small size).

So you need to import new model + texture, while the other tutorial lets you upload the same model, without any extra textures and the model will definitely have smaller size, since some geosets have been removed.

(Although I don't understand why they had to go out of their way in that tutorial and remove texture paths and other things via notepad, when you can do it in War3Model editor as well...)

Also, asking others to give you rep is generally frowned upon, if they like it, they'll give you rep, else they won't.

Apart from that technical stuff I wrote above, I think this is a very nice tutorial and it definitely did take some time to make all those gifs. I would use more colors, but that's just personal opinion.

Good job

#### UlithiumDragon

#### UlithiumDragon

 Level 3

JoinedMay 5, 2013Messages20

Resources: 1

#### Resources
Tutorials 1

Nichilus said:

 Your tutorial ain't bad, but the tutorial at thehelper is better. The reason for that is because they remove the unneeded. In this case, they remove the hero glow which is unwanted. Your tutorial suggest that we leave it but make it invisible. But the thing is - if it can't be seen, why leave it at all when it serves no purpose, right? You also need to import another texture (albeit it will be of very small size).


So you need to import new model + texture, while the other tutorial lets you upload the same model, without any extra textures and the model will definitely have smaller size, since some geosets have been removed.


(Although I don't understand why they had to go out of their way in that tutorial and remove texture paths and other things via notepad, when you can do it in War3Model editor as well...)

Also, asking others to give you rep is generally frowned upon, if they like it, they'll give you rep, else they won't.


Apart from that technical stuff I wrote above, I think this is a very nice tutorial and it definitely did take some time to make all those gifs. I would use more colors, but that's just personal opinion.


Good job

Click to expand...


I made this in the first place because a lot of the custom models I was dealing with had .mdl coding very different from what that tutorial was using as an example, and as a result I couldn't get some of them to work (and also I was CRAZY bored lol).

As for the rep thing, I recalled seeing it on several other tutorials, so I thought it was some standard practice or something (I tried very hard to follow the exact format and quality of several much-loved approved tutorials). ANyways, thanks for the tip, I removed it!

#### GaLaxY_256

#### GaLaxY_256

 Level 13

JoinedMar 21, 2014Messages893

Resources: 8

#### Resources
Models 2Icons 3Tutorials 3

Geosets are easy to modify to remove/add Hero Glows. =_="


And you don't need to import custom textures for that.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

 Before you start this part, you should save your map, close World Edit, restart it, and re-open your map. This is because World Edit seems to need to be restarted in order for it to recognize the new models and textures inside the editor (although they seem to appear just fine when using the "Test Map" button in World Edit).

I would guess it has something to do with World Edit finalizing or indexing the MPQ file when it's closed, but I'm not sure. Note that simply reloading the map doesn't work, you need to fully exit World Edit


Click to expand...

Just save the map after importing and before using, the model should appear fine. To me, saving is easier than re-opening WE and loading the map.

 Open up the Object Editro.


Click to expand...

Typo.

Also as aforementioned just removing the Hero Glow geoset (with the help of Ctrl + Clicking it of course) would do. No custom textures or material changes included.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I love the layout of this tutorial. But as others have mentioned, it is a bit of a longer route than deleting the geoset. Any reason why using a blp would be better?

Still, +rep for taking the time to make the tutorial. I can definitely see that you put effort into it.

#### UlithiumDragon

#### UlithiumDragon

 Level 3

JoinedMay 5, 2013Messages20

Resources: 1

#### Resources
Tutorials 1

PurgeandFire said:

 I love the layout of this tutorial. But as others have mentioned, it is a bit of a longer route than deleting the geoset. Any reason why using a blp would be better?


Still, +rep for taking the time to make the tutorial. I can definitely see that you put effort into it.


Click to expand...


No, there really isn't a reason to do it this way - the other way is easier and requires less file space. I didn't know that when I made this, nor was there another tutorial showing me how to do it that way.


I simply tried to share the method I found out on my own because the other way way undocumented...

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Ah okay. Sad to hear. I suppose I'll send this to the graveyard. Still, great job on the writeup and the demonstrative gifs.

#### sunshinex3

#### sunshinex3

 Level 12

JoinedMar 1, 2009Messages372

Resources: 5

#### Resources
Maps 5

Thank you lithium this helped me at least, thanx alot!!

 You must log in or register to reply here.


### Similar threads

 Model Paths


nathanman

Nov 14, 2007

Tutorial Archive

Replies3Views5K

Nov 17, 2007
PurplePoot

 Import Custom Textures and Create/Edit custom models for Newbies!


Statharas

Aug 6, 2008

Tutorial Archive

Replies6Views21K

Oct 21, 2008
Hawkwing

 Removing Team Glow VERY QUICKLY


Ham Ham

Feb 16, 2007

Tutorial Archive

Replies8Views2K

Mar 4, 2007
Wolverabid

 Invisible Model - Visible Attachment


I34w90_238j

Jun 3, 2009

Tutorial Archive

Replies4Views4K

Jun 22, 2009
BlinkBoy

 Easy Loading Screen


Aksu

Oct 12, 2007

Tutorial Archive

Replies1Views2K

Oct 12, 2007
Wolverabid

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