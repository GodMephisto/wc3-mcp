---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/high-resolution-loading-screens.322172/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# High Resolution Loading Screens
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/high-resolution-loading-screens.322172/>
#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

In this tutorial, you'll find how to get a HD Loading Screen.

There's two methods available. ".dds" & ".tga"

The one you choose is based on the Warcraft 3 version you have.

Users on the Latest Patch can choose between either, although using a ".dds" import is superior as it uses a lower file size and is more performance friendly.

Users using a Classic Warcraft Patch do not have ".dds" support enabled, and therefore will need to use ".tga"

Let's take a look at some of the tools you can use to complete this task. You'll need a decent photo editor, such as:

Option 1: GIMP

Option 2: Photoshop

Option 3: Paint.net

Note: Your version of Photoshop may not support ".dds" and you'll need to install the Nvidia Texture Tools Plug-in. Both GIMP and Paint.net support ".dds" natively and no plug-in is required.

Now that we have the tools required, we're going to need to find or create an image to use as our Loading Screen.

This is where you'll need to pick a resolution size. Realistically, a 1920×1080 image is ideal due to its high quality and low file size. You may, however, use much higher resolution images, although it is not recommended unless it's a single player game.

"But wait, what's the maximum?" - I've tested up to 7680×4320 with success.

Now that we've chosen our size and image, it's time to make it compatible for the game to read it.

This is where you'll choose between ".dds" and ".tga" depending on the version of the game you're using.

From here on, all examples will be using an image from my own map and GIMP as the tool.

Here we have a 1920×1080 image to use, with some empty space on the bottom so the loading bar doesn't intrude.

If you own Reforged, but play on SD graphics only, do not worry, ".dds" is still supported. It's what all the in-game files use, and it's what you should use too.

REFORGED (1.32+) USERS: Now we're going to use our tool to convert the image to ".dds"

Open the image in your tool of choice, double check the sizing of the image, and save the file as "Fullscreen.dds"

"Why 'Fullscreen.dds'?" - It's the path I've set for the texture on the model I created. You'll be able to download the model at the end of the tutorial.

Once you've saved the file, it will ask you what special settings you wish to use. We're going to use the settings I provide for you here:

NOTE: Mipmaps may not be required for loading screens and you can save on file size by setting it to "No mipmaps"

Now we have the image for the Loading Screen. Hang tight while I show Classic users how to do theirs.

CLASSIC (1.00-1.31.1) USERS: Now we're going to use our tool to convert the image to ".tga"

Open the image in your tool of choice, double check the sizing of the image, and save the file as "Fullscreen.tga"

"Why 'Fullscreen.tga'?" - It's the path I've set for the texture on the model I created. You'll be able to download the model at the end of the tutorial.

"But wait, what if I want to use '.blp'?" - You'll never see me support the use of ".blp" in any of my guides, but you can use ".blp" just as you can use ".tga"

Once you've saved the file, it will ask you what special settings you wish to use. We're going to use the settings I provide for you here:

Now we have the image for the Loading Screen.

Next, we're going to look at how to actually use this image in-game for your map.

Import your converted image file through Asset Manager/Import Manager under the correct path, "Fullscreen.dds" OR "Fullscreen.tga"

Download and import LoadingScreen.mdx with the path "LoadingScreen.mdx"

In World Editor, select Scenario -> Map Loading Screen

Under Loading Screen Graphic, select your imported "LoadingScreen.mdx"

Note: If you want no default map text to appear on your loading screen, simply input a "space" on each line of the three description boxes.

Note: If the image appears as completely black, double check that you have removed the "war3mapImported/" from the path file on the imports.

Now, it's time to test out our new image.

Simply test your map and check out your awesome new loading screen!

#### Attachments

LoadingScreen.mdx

 1.3 KB

 · Views: 1,357




 Last edited: Aug 7, 2023

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

One can also obtain HD loading screens using the classic loading screen models and new higher resolution textures. One just has to make sure that the textures are enlarged for all components of the loading screen equally and probably by powers of 2.

For example I made a 2,048x1,536 loading screen that way. This gets stretched to whatever aspect ratio the display is. Since it is DDS, the textures are compressed with DXT5.

#### Barorque

#### Barorque

HD Model Reviewer

 Level 51

JoinedSep 8, 2008Messages637

Resources: 89

#### Resources
Packs 3Maps 4Reforged HD Models 53Reforged HD Skins 20Reforged HD Icons 7Sounds 1Tutorials 1

Wonderful +rep

Compared to the old ¨cutting your image into a million pieces¨ for it just not to work at the end.. this is p e r f e c t !

Blessings upon thee!

#### Poduchen

#### Poduchen

 Level 2

JoinedApr 27, 2019Messages4

Hey, this surely may come in handy for me.

Thanks for posting this tutorial!

#### Saken

#### Saken

 Level 14

JoinedJan 19, 2008Messages798

Resources: 6

#### Resources
Maps 4Templates 2

Good stuff & looks great! I look forward to using this.

#### Serenity09

#### Serenity09

 Level 6

JoinedMar 7, 2011Messages124

Resources: 1

#### Resources
Maps 1

nice tutorial, this was exactly what i was looking for!

is there any reason to first export the image as "LoadingScreen.tga" before renaming to "FullScreen.tga"?

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

A slight oversight by my part, i was looking at the start of the guide to keep all names similar to each other, and focus only on the converting to .tga part. The model I use is looking for a "FullScreen.tga" however, so it is required to change the path to such for it to work.

edit: Sorry, to answer your question, no, there is not a reason to export it under a different name. You may export it as "FullScreen.tga" and it will operate the same as I have shown throughout the guide.


 Last edited: Feb 7, 2020

#### Daniel Kopp

#### Daniel Kopp

 Level 2

JoinedNov 16, 2019Messages12

This is awesome, trying it out right now!

+rep

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Use of .tga is utterly pointless. One can edit the loading screen model to use the texture file "FullScreen.blp" and then import a .dds file to FullScreen.dds and it will work.

Attached is a map showing this.

#### Attachments

DDS Loading Screen.w3m

 1.3 MB

 · Views: 419



#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Thanks to help from Dr Super Good, this tutorial has been updated to support three types of imports, all ranging in quality/file size to fit your needs for your map.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

DXT1 only supports alpha masking. If a complex loading screen model is used that relies on alpha blending then one has to use DXT5 since that supports alpha gradients. DXT1 is otherwise recommended since it achieves a compression factor of 6:1 as opposed to 4:1 from DXT5.

#### Zoson

#### Zoson

 Level 6

JoinedApr 30, 2017Messages17

Resources: 2

#### Resources
Spells and Systems 2

Worthy of being called one of the now eight wonders of the world.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Awesome stuff. Thanks for being so detailed with the steps for each file type! Approved.

#### LazZ

#### LazZ

 Level 20

JoinedNov 4, 2019Messages407

Resources: 206

#### Resources
Packs 1Tools 2Maps 2Ported Icons 199User Interface 1Sounds 1

For anyone curious, the latest patch (1.32.3) doesn't stretch/squish custom loading screens anymore depending on the aspect ratio of the client (e.g. 1920x1080 squished to a 4:3 format).

#### maddeem

#### maddeem

 Level 24

JoinedJan 1, 2011Messages1,506

Resources: 34

#### Resources
Models 5Icons 3Skins 1Maps 6Spells and Systems 18Lua 1

Excellent tutorial!

#### Arathorn

#### Arathorn

 Level 9

JoinedSep 8, 2017Messages105

Resources: 2

#### Resources
Maps 2

I was using War3ModelEditor to create my loading screen before Reforged. It worked great and was very simple but Reforged totally missed it up.

So I found this tutorial which is very clear, complete and show simple ways to do it.

I still have an advice to make the tutorial better : make the sentences bigger in the method 1/2/3 parts because it is possible to not see them between all those big images : )

Thanks again for the tutorial Mayday and for the nice request you made me.

#### Unholy0ne

#### Unholy0ne

 Level 3

JoinedJun 3, 2019Messages30

Thanks for the tutorial, Mayday.

I've implemented a loading screen as such but use loading screen text from the world edit.

It moves that text to the bottom left instead of the usual mid right, is that configurable or do I have to edit the image for proper text position?

Also, I'm using a 21:9 Monitor, is that supported for custom loading screens yet?

Here's a screenshot for reference: Screenshot

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Unholy0ne said:

 Thanks for the tutorial, Mayday.

I've implemented a loading screen as such but use loading screen text from the world edit.

It moves that text to the bottom left instead of the usual mid right, is that configurable or do I have to edit the image for proper text position?

Also, I'm using a 21:9 Monitor, is that supported for custom loading screens yet?

Here's a screenshot for reference: Screenshot

Click to expand...

Hey UnholyOne, the pic is looking great! Unfortunately, you're not able to move the text around on the loading screen, but you are able to write the text in on the image with a image editor (gimp, photoshop, paint.net)

In order to hide the text located there, you need to apply a single space bar on each line.

For 21:9 monitors, you may use an image that is scaled accordingly. For example, 2560×1080 will cover your entire screen, while retaining the same quality as a 1080p image. You can of course use higher, if that is your intent. I've tested up to 7,680 x 4,320 for loading screens. You can probably go even higher, but the file size on that image was 50 megabytes, not ideal for something that doesn't effect gameplay.

#### Unholy0ne

#### Unholy0ne

 Level 3

JoinedJun 3, 2019Messages30

Mayday said:

 Hey UnholyOne, the pic is looking great! Unfortunately, you're not able to move the text around on the loading screen, but you are able to write the text in on the image with a image editor (gimp, photoshop, paint.net)


Click to expand...

I thought as much and went ahead with it since my last post, see Screenshot

Mayday said:

 In order to hide the text located there, you need to apply a single space bar on each line.


Click to expand...

I left it completly empty, which worked as well.

Mayday said:

 For 21:9 monitors, you may use an image that is scaled accordingly. For example, 2560×1080 will cover your entire screen, while retaining the same quality as a 1080p image. You can of course use higher, if that is your intent. I've tested up to 7,680 x 4,320 for loading screens. You can probably go even higher, but the file size on that image was 50 megabytes, not ideal for something that doesn't effect gameplay.


Click to expand...

I tested this and didn't get it to work, the result looks like this for me: Screenshot.

The image is exactly 2560x1080 (stretched for test purposes) but the loading screen constrains it to 16:9.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

16:9 is kept for visual consistency between clients. This is because simply stretching a loading screen might not always be a suitable behaviour. Especially if complicated artwork is used, it would suffer from distortion which might ruin the appearance and go against the author's intentions with the piece.

What is needed is StarCraft II style loading screen scaling controls which allow the author to specify the scaling mode of the loading screen image. This gives flexibility by allowing fixed ratio scaling for pieces that require it and full screen stretch scaling for those that do not. Of course many map authors failed to use this correctly but still.

#### ZiGOR

#### ZiGOR

 Level 10

JoinedMar 17, 2012Messages582

Resources: 3

#### Resources
Maps 3

That's helpful. Thanks! +rep

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Glad you're all enjoying the guide. Massive overhaul today for a better user-friendly read.

#### Jalty

#### Jalty

 Level 2

JoinedOct 31, 2020Messages2

Any idea why its work first time and when im adding unit i got a black screen ?

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Jalty said:

 Any idea why its work first time and when im adding unit i got a black screen ?


Click to expand...

You're gonna have to elaborate, I don't know what you mean

#### Jalty

#### Jalty

 Level 2

JoinedOct 31, 2020Messages2

Well, for some obscure reason, I had oddly changed the .mdx everything works now. excellent tutorial!

#### Zankh

#### Zankh

 Level 4

JoinedJan 2, 2021Messages21

Resources: 1

#### Resources
Maps 1

Hey, at first thank you for the Tutorial, but i dont know how to get it work ^^

I'm working first time with the asset manager, and i dont know what you mean with "Import your converted image file through Asset Manager/Import Manager under the correct path."

I imported both, my image i want to use and your loadingscreen.mdx, but if i choose the mdx as loading screen, its just black.

Screenshot is under attached files.

Edit: Okay i found the solution by myself  i imported both files, then i saved the map as a Folder (Save Map as -> File Type - Folder).

After that just drag an drop both files from the war3mapimported folder to the mainfolder, and it works!

#### Attachments

asset2.jpg

 72.3 KB

 · Views: 252




 Last edited: Jan 8, 2021

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Zankh said:

 Hey, at first thank you for the Tutorial, but i dont know how to get it work ^^

I'm working first time with the asset manager, and i dont know what you mean with "Import your converted image file through Asset Manager/Import Manager under the correct path."

I imported both, my image i want to use and your loadingscreen.mdx, but if i choose the mdx as loading screen, its just black.

Screenshot is under attached files.

Edit: Okay i found the solution by myself  i imported both files, then i saved the map as a Folder (Save Map as -> File Type - Folder).

After that just drag an drop both files from the war3mapimported folder to the mainfolder, and it works!


Click to expand...

Hi Zankh, you need to remove the `war3mapimported/` from the path files in order for them to work.

edit: Looks like in my rewrite, I neglected to list the path required for both. It's fixed now

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


Looks great! Don't forget to attach the images as attachments and not use embedded Discord links

#### AbsolutePsycho

#### AbsolutePsycho

 Level 11

JoinedDec 11, 2009Messages234

Resources: 2

#### Resources
Maps 2

Mayday said:

Click to expand...

There is no need to generate mipmaps for loading screen.

Without mipmaps you get the same result but with smaller file size.

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

BizzaroFukuro said:

 There is no need to generate mipmaps for loading screen.

Without mipmaps you get the same result but with smaller file size.


Click to expand...

Since the game doesn't always accept .dds without mipmaps, I never bothered to test it; good find!

#### SenPie69

#### SenPie69

 Level 9

JoinedMar 26, 2020Messages220

Resources: 1

#### Resources
Maps 1

My good sir! you just saved me some time from cropping/splitting images of the old way

i wish i could upvote you three times lol

#### Alex0013

#### Alex0013

 Level 2

JoinedJun 30, 2017Messages9

This guide is great! I do have a question though, does anyone know how to keep the player names showing, along with the Map Name and Map Creators like the default view has? Obviously the latter 2 I can do in an image, but just curious if it's possible to use a custom loading screen, but keep the player names part?

Seems like no matter what I try, the Custom loading screens aren't working with the player names.

Note: From what it looks like in my map (I'm a 3rd generation developer, so this was set years ago by the original creator) and online, this used to work in Classic WC3 by just overwriting these files (with no mdx necessary) in Import Manager. And then using "Default" in the loadscreen settings. However, it appears the 'new default' doesn't use these, and I can't find what the new default is using to overwrite them like we used to be able too...anyone have any ideas? Or is there another new way to still keep the player's names on the screen?

UI\Glues\Loading\Multiplayer\Loading-BotLeft.blp - for your bottom left picture

UI\Glues\Loading\Multiplayer\Loading-TopLeft.blp - for your top left picture

UI\Glues\Loading\Multiplayer\Loading-Human-BotRight.blp - for your bottom right picture

UI\Glues\Loading\Multiplayer\Loading-Human-TopRight.blp - for your top right picture


 Last edited: Aug 7, 2021

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Alex0013 said:

 This guide is great! I do have a question though, does anyone know how to keep the player names showing, along with the Map Name and Map Creators like the default view has? Obviously the latter 2 I can do in an image, but just curious if it's possible to use a custom loading screen, but keep the player names part?

Seems like no matter what I try, the Custom loading screens aren't working with the player names.

Note: From what it looks like in my map (I'm a 3rd generation developer, so this was set years ago by the original creator) and online, this used to work in Classic WC3 by just overwriting these files (with no mdx necessary) in Import Manager. And then using "Default" in the loadscreen settings. However, it appears the 'new default' doesn't use these, and I can't find what the new default is using to overwrite them like we used to be able too...anyone have any ideas? Or is there another new way to still keep the player's names on the screen?

UI\Glues\Loading\Multiplayer\Loading-BotLeft.blp - for your bottom left picture

UI\Glues\Loading\Multiplayer\Loading-TopLeft.blp - for your top left picture

UI\Glues\Loading\Multiplayer\Loading-Human-BotRight.blp - for your bottom right picture

UI\Glues\Loading\Multiplayer\Loading-Human-TopRight.blp - for your top right picture


Click to expand...

I believe you can set your minimap picture to be a loading screen image (512x512, probably) and then dynamically change your minimap in game; have you tried anything like that? You can't change the background image of the map or whatever it is, though. So you'll have a tiny looking loading screen in place of the traditional minimap art

#### Alex0013

#### Alex0013

 Level 2

JoinedJun 30, 2017Messages9

I actually don't care about the minimap part of it. I could take that or leave it. Just would like it to have the player names at least. The other stuff I can insert into a background picture myself.

Take a look at my screenshots here:

Classic.png --> This is what used to display on WC3 Classic. The players names are all there, along with the map name.

Reforged Default.png --> Without my changing anything, when Reforged came out, it automatically changed to this. Notice it's got the map name, created by, and all the players names currently in game (along with computer names)

Reforged after updating.png --> I updated the background using methods described here, and you lose the player loading, the map name, (technically the minimap, but I don't really mind that.) (PS I know I need to make updates to remove the black box, but this was just testing. I also manipulated it to fit 16:9 on purpose, and cut out the bottom of the image for testing purposes.)

#### Attachments

Classic.png

 2.3 MB

 · Views: 213



Reforged after updating.png

 2.7 MB

 · Views: 247



Reforged Default.png

 4.2 MB

 · Views: 219



#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Yeah, you can't mimic the first screenshots, but you can put the zombieland image inside the final screenshot's minimap box, then dynamically change it back to your original minimap image in game.

#### Richardik

#### Richardik

 Level 2

JoinedApr 1, 2012Messages3

There's a big delay by the time loading screen appears, so much so that loading is almost even finished by that time.

Any solutions?

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Richardik said:

 There's a big delay by the time loading screen appears, so much so that loading is almost even finished by that time.

Any solutions?


Click to expand...

hi, this has been an issue since warcraft patch 1.33 released, but I believe it is only an issue when you use the "test map" feature from the editor, and doesn't really happen from bnet game lobbies.

If I am incorrect, I do not have a solution anyways.

#### Linaze

#### Linaze

 Level 24

JoinedFeb 28, 2007Messages3,478

Resources: 4

#### Resources
Models 1Tutorials 3

I can confirm this issue when testing via the WE, but working fine when launching directly from War3.

#### Nightmare2077

#### Nightmare2077

 Level 18

JoinedApr 5, 2011Messages375

Resources: 14

#### Resources
Icons 12Maps 2

Excellent stuff, all works perfectly in 1.26, 1.30 and reforged (1.36)!

Sadly, I couldn't get .blp to work (weighs ~2mb compared to 4.5 mb for .tga), but that's not that big of a deal.

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Nightmare2077 said:

 Excellent stuff, all works perfectly in 1.26, 1.30 and reforged (1.36)!

Sadly, I couldn't get .blp to work (weighs ~2mb compared to 4.5 mb for .tga), but that's not that big of a deal.


Click to expand...

1.26 has resolution limits on .blp files, which is why the method of splicing the image was used.

In 1.30+, I believe blp files no longer have the 512x512 cap, and it should work.

#### SadOnix

#### SadOnix

 Level 4

JoinedDec 14, 2023Messages47

Best Tutorial!!! I had so many Trys and all Times it resulted in an average Quality/medicore looking Picture. Now its a crisp clean, high quality Loadingscreen and iam superhappy. Mayday made my Day

#### Barbafire

#### Barbafire

 Level 14

JoinedAug 24, 2022Messages555

Resources: 2

#### Resources
Icons 1Maps 1

I can't believe I've finally found the definitive solution for my loading screen issue. It worked properly, without complication.

I did diferent with the resolution, because 1920x1080 left a black line on the top and on the bottom of the loading screen, so I just changed to 1920x1280, and fit well, both windowed and full screen modes.

Now the next issue is the map preview.

I'd like to deify you, because this topic in specific, gave me a month on headache. The result in my test map:

EDIT: I realized that the aspect ratio has been commited with 1920x1280, so I went back to 1920x1080.


 Last edited: Aug 1, 2024

#### HL_Vortex

#### HL_Vortex

 Level 14

JoinedApr 15, 2020Messages61

Resources: 10

#### Resources
Icons 1Maps 1Reforged HD Models 8

Anyone have any suggestions on how to fix this issue

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

HL_Vortex said:

 Anyone have any suggestions on how to fix this issue


Click to expand...

You have to delete all the text on the loading screen and replace it with a blank space. Then, you can add back the text with a photo editor.

#### Grünwald

#### Grünwald

 Level 2

JoinedNov 16, 2024Messages6

Thanks for your tutorial. Worked super. Did you find a way to add loading bars of players back into the custom loading screen?

example attached

#### Attachments

testbildschirm (1).jpg

 2.3 MB

 · Views: 109



#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Grünwald said:

 Thanks for your tutorial. Worked super. Did you find a way to add loading bars of players back into the custom loading screen?

example attached


Click to expand...

this is only possible when using the default blizzard loading screens, to have this + custom image, you must replace your minimap picture with loading art, and then replace it again in-game for map art

#### Grünwald

#### Grünwald

 Level 2

JoinedNov 16, 2024Messages6

Ok. Set

Mayday said:

 this is only possible when using the default blizzard loading screens, to have this + custom image, you must replace your minimap picture with loading art, and then replace it again in-game for map art


Click to expand...

So

1. Set MapOptions back to Default

2. Replace w3minimap.blp in the MPQ with my custom Loadingscreen. (yes, i converted it into .blp)

3. ????

Voila.

Question, how do i get my minimap pic fullscreen? And how do i replace it in-game for map art?

#### Attachments

image (1).jpg

 826 KB

 · Views: 117




 Last edited: Feb 8, 2025

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Grünwald said:

 So

1. Set MapOptions back to Default

2. Replace w3minimap.blp in the MPQ with my custom Loadingscreen. (yes, i converted it into .blp)

3. ????

Voila.

Question, how do i get my minimap pic fullscreen? And how do i replace it in-game for map art?


Click to expand...

You can make it fill the square by setting the images aspect ratio 1:1; such as 512x512.

To change the minimap in-game, you can use this native to change back to the original minimap art, which you can get from file -> export minimap

 native BlzChangeMinimapTerrainTex takes string texFile returns boolean

Click to expand...

#### AFreerSky

#### AFreerSky

 Level 3

JoinedJan 11, 2023Messages7

Does this not work in ultrawide resolutions? This seems to be what I get when I try to make them 21:9 or 21:10:

#### Attachments

Custom background issue.png

 6.2 MB

 · Views: 96



#### Dartz

#### Dartz

 Level 15

JoinedJan 20, 2005Messages415

Resources: 20

#### Resources
Models 2Icons 4Packs 1Ported Icons 13

For anyone who thinks 3840x2160 resolution is for fools: Yes, fortunately 2560x1440 is also supported.


 Last edited: Sep 17, 2025

 You must log in or register to reply here.


### Similar threads

 Custom Loading Screen Tutorial V1.5


Sopho

May 8, 2004

2D Art Tutorials

Replies24Views80K

Sep 2, 2022
dantles1992

 Simple Loading Screen


Aesthetics

Aug 11, 2009

Misc/General Mapping Tutorials

Replies42Views38K

Apr 27, 2021
erfaned

W

Locked

 Loading Screen Editing


WcStorm

May 12, 2004

Warcraft Editing Tools

Replies4Views2K

Jun 17, 2004
Sopho

X

 Custom loading screen for the In-experianced.


XxNiNjAsHaDoWxX

Dec 30, 2007

Tutorial Archive

Replies6Views4K

Feb 11, 2008
PurplePoot

 Photoshop Loading Screen Template


Frotty

May 21, 2014

2D Art Tutorials

Replies23Views14K

Jan 1, 2020
Anglo Warior

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