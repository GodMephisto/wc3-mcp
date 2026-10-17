---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/custom-loading-screen-tutorial-v1-5.7927/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Custom Loading Screen Tutorial V1 5
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/custom-loading-screen-tutorial-v1-5.7927/>
#### Sopho

#### Sopho

 Level 21

JoinedFeb 14, 2004Messages3,311

Resources: 26

#### Resources
Models 6Icons 13Skins 5Tutorials 2

Hello!


Its possible, yes a custom loading screen can be added into your maps to improve its appearences.


In this tutorial, I will explain to you all, how to do it. If you don't beleive me, I have seen other maps with custom loading screens and even once my map had it too. As I work along in this tutorial, I guarantee you will be satsifed with the results.

 - GENERAL -


Difficulty: Medium+

Estimated Work Time


Beginner: 15-30mins


Skilled person: 10-15mins


Confident Pro: 5-10mins

Recommended/Needed Tools

Paint Shop Pro

www.jasc.com

Photoshop CS

www.adobe.com

Serif Photoplus

www.serif.com


-----------------------------

The GIMP

www.gimp.org


Irfanview

www.irfanview.com

TIP: Try to always stay updated on these image programs!

REQUIRED


Warcraft III: The Frozen Throne

Some skills with the import manager.

Some good skills with image programs

- WORKING -

1. You need a 1024 x 768 pixles image or a reasonable size as your custom loading screen. (must be quite large)


If you have problems doing this, try resizing with a easier paint program. (I highly suggest Irfanview as I will be using Irfanview for the whole of this Tutorial )


I got myself an interesting image


Now to resize...


Heh, done

2. Slice the 1024 x 768 image for your custom loading scren into 4 parts. (like dividing a piece of cake into 4) NOW FOLLOW CAREFULLY THIS PART - Bottom left and bottom right sliced parts of the image MUST be 512 x 256 pixles and Top left and Top Right parts MUST be 512 x 512 pixles. TIP: Crop the images slowly by hand with a good and easy to use paint program. Also look at the screenshots.


Bottom left (freehand crop)


Top left (freehand crop)

3. Once you've done that, you should have 4 parts of the loading screen image. You can do it after or before imporing them into your map, but I suggest renaming them now. Now rename the parts:

Bottom Left part > LoadingScreenBL

Bottom Right part > LoadingScreenBR

Top Left part > LoadingScreenTL

Top Right part > LoadingScreenTR


Renaming all 4 parts

4. Convert ALL of the parts (make sure they are renamed correctly) to .tga or .blp TIP: I recommend .blp as its much smaller but there are more problems with it. 28/5/04 < I have discovered the problem with why some loading screen parts will not work even though they are converted to .blp. This is because: the .tga bits are not high enough, 32 bits is required, it hasn't got alpha channels. If you just want to insert a opaque alpha channel then use Wc3 Viewer to do it for you easily.

5. Ok, put them in a folder for easy acess for the next step. This is optional.

6. Start up World Editor (or your map) and go to Import Manager.

7. Import the 4 parts of your custom Loading Screen (make sure they are renamed, resized correctly, properly converted) with Import File or Import Files...


The folder created earlier makes things easy

8. Once they are imported, remove their paths from w3mapImportered to just the loading screen part proper name. Eg: War3MapImported\LoadingScreenBL.blp to LoadingScreenBL.blp remove all 4 of em. The " use custom path " should be ticked.


You have to do this

9. Now, import the LoadingScreen.mdx file. TIP: There are specific LoadingScreens.mdx. There are 2 types. One is designed by Blizzard for .tga parts and another is modified from Blizzard's by someone from campaigncreations.org. Try to mix these together and get them right.

Below will tell you which LoadingScreen.mdx is for whether its for .tga or .blp. This is very important!


> If you are using .tga...


Export LoadingScreen.mdx from the import manager of Blizzard's " Monolith Map " and put it somewhere which you will use later, this is important or else the custom loading screen won't work! .blp won't work if you have this one but only .tga


TIP: At the end of this Tutorial there will be some needed resources for help


if you can't do this.


Get it from Monolith! (Exporting it from its Import Manager)

If you are using .blp... (recommended)


Download the specific LoadingScreen.mdx from campaigncreations.org as this specific one allows .blp parts to be the custom loading screen. A .blp custom loading screen saves tons of space so I recommend this. .tga won't work if you have this one but only .blp TIP: At the end of this Tutorial the zip file will include this specific LoadingScreen.mdx as well!

10. Minimize/exit Import manager for the " Monolith Map ". Start back your map. Go to " Scenario " on the toolbar, then go to " Map Loading Screen... " underneath it.

11. Tick " Use Imported File " and select the LoadingScreen.mdx as it. Then press ok. Note that the path of it is also displayed.


Yes! We're done!!


12. Restart (close it and open it) your map and apply the changes again to make the World Editor used to it. Then test the map, and WALLA!! congrats!


" Well done Sopho and you " - Sopho

- QUICK FAQs -

Having trouble? You may find your question below before asking

Q: I use .tga and the loading screen is all black! Why is this?


A: Make sure the .tga is 32 bits or 24 bits (I think). Irfanview will convert it sucessfully for you. Make sure the paths are also correct!

Q: Arghh!! The loading screen is all messed up! Its not correct the parts are displayed wrongly!! Help! I can still see it though.


A: Make sure the LoadingScreen parts are correct. For example the Bottom Left (BL) is named as TR (top right) and the TL (top left) is named as BR (bottom right) and so on... Just correct the names. Also make sure the sizes of each part is precisely accurate. (Remember what I did earlier on in this Tutorial)

Q: I use .blp and the loading screen is all black, how come?


A: When you converted the .tga, .jpg or .bmp, to .blp was it 32bits? No? Then a opaque alpha channel will be needed but I don't really understand those things so download WC3 Viewer to add them for you easily.

Q: I can't find " Monolith " , I'm confused with the 2 LoadingScreen models help!


A: Below is a zip which includes the 2 loading screen models. I have also made things easier by labelling the Loading Screen models. Remember when you import them using the ones in the zip, get rid of the TGA or BLP at the end!

Q: I can't find the loadingscreen.mdx for .blp!! Help


A: Here http://www.campaigncreations.org/

Q: My custom loading screen shows up black


A: Make sure you used the correct loadingscreen.mdx for your .tga/.blp

Q: Can I use .blp and .tga in my custom loading screen?


A: Unfortunately, you cannot


===================================================


Hope that help, if anyone else is looking at this, I hope this helped you too!

WARNING - Custom Loading Screens take up lots of space if you are using .tga. Try .blp [RECOMMENDED]

If you still have difficulties, PM me as I am usually free to help (But I don't think you will need it because I make things so easy)


Tutorial Version 1.5 (updated at 2/8/04)


Remember PM me if you still have questions, comments or thanks. NOTE - I cannot upload attachments on wc3sear tutorial version so you will have to try working it by yourself...


 Last edited by a moderator: Mar 12, 2007

#### Magos

#### Magos

 Level 13

JoinedMar 25, 2004Messages28

Resources: 1

#### Resources
Tools 1

Check the Tools section for War 3 Model Editor. It can create a loading screen automatically. It does all the dirty work itself (resizing image etc...).

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

thnks for the thread!

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

not even bumped.

#### hypsandar

#### hypsandar

 Level 7

JoinedMar 9, 2016Messages226

You forgot to mention to add the LoadingScreen.mdx Into the import manager of the user's map and than select it. Tutorial only said to export it and put it aside for later use.

Anyways, awesome tutorial. Helped a lot.

Don't ban me for replying to a 12 years old thread, please. Lol


 Last edited: Jun 30, 2016

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

hypsandar said:

 You forgot to mention to add the LoadingScreen.mdx Into the import manager of the user's map and than select it. Tutorial only said to export it and put it aside for later use.

Anyways, awesome tutorial. Helped a lot.

Don't ban me for replying to a 12 years old thread, please. Lol


Click to expand...

Replying for tutorials is not considered a necro-post. Don't worry.

#### Veronnis

#### Veronnis

 Level 14

JoinedJul 19, 2010Messages309

Resources: 18

#### Resources
Models 3Icons 1Skins 5Maps 4Reforged HD Skins 4Sounds 1

Thank you! Out of the 5 tutorials I have read on this matter this is the only one that resulted in me actually getting an HD load screen.

#### Mistablockout

#### Mistablockout

 Level 4

JoinedNov 3, 2015Messages47

Hahaha! I felt so happy when I saw this worked on my first go. Thank you!

#### KenArok

#### KenArok

 Level 12

JoinedSep 27, 2016Messages175

Resources: 4

#### Resources
Models 2Maps 1Reforged HD Models 1

how if i want to use 1920 x 1040 px image?

#### Veronnis

#### Veronnis

 Level 14

JoinedJul 19, 2010Messages309

Resources: 18

#### Resources
Models 3Icons 1Skins 5Maps 4Reforged HD Skins 4Sounds 1

I don't quite remember the process off the top of my head but you need to make a model comprised of 4 squares with separate texture references. Feel free to download and check out my Dark Planet map, I used it there for its load screen. You can also extract the model from my map.

#### Anglo Warior

#### Anglo Warior

 Level 7

JoinedDec 28, 2011Messages145

Resources: 4

#### Resources
Icons 4

The link to the loadingscreen mdx no longer works, I think.

#### Veronnis

#### Veronnis

 Level 14

JoinedJul 19, 2010Messages309

Resources: 18

#### Resources
Models 3Icons 1Skins 5Maps 4Reforged HD Skins 4Sounds 1

Anglo Warior said:

 The link to the loadingscreen mdx no longer works, I think.


Click to expand...

I believe this loading screen will no longer be functional in reforged anyway. I think they have made a better load screen system that doesn't require a 4-part model work around.

#### Kapharna

#### Kapharna

 Level 3

JoinedFeb 12, 2020Messages35

Oe boi, I need this loading screen.mdx file. Will it still work on reforged?

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

For Reforged and other versions supporting 16/9 resolution, use the attached loading screen model with a simple picture converted to BLP with no mipmaps and the name "Fullscreen.blp" (no path) in the asset manager.

#### Attachments

LoadingScreen.mdx

 1.3 KB

 · Views: 596



#### Cheshire

#### Cheshire

 Level 26

JoinedMar 29, 2020Messages1,482

Resources: 4

#### Resources
Icons 3Maps 1

wow. I have literally spent hours on these loading screen tutorials... only this one finally came through... hallelujah.

I attached the loading screen mdx from monolith.

and here is a great free tool for resizing: Online Image Resizer - Crop, Resize & Compress Images, Photos and Pictures for FREE

#### Attachments

LoadingScreen.mdx

 3.1 KB

 · Views: 491



#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Not that I currently need a new model,

But I am curious.

This is definitely a different model, what does it do, and where does it come from ?

Thanks.

#### Cheshire

#### Cheshire

 Level 26

JoinedMar 29, 2020Messages1,482

Resources: 4

#### Resources
Icons 3Maps 1

It's the one he suggested using in the tutorial. He says to export it from the map monolith, so I did just that and put it here to save the next guy 5 minutes. I know not why or how it is special.

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Oh ok, can it handle a single fullHD image or does it require you to split the image in 4 parts like in the tutorial ?

#### Cheshire

#### Cheshire

 Level 26

JoinedMar 29, 2020Messages1,482

Resources: 4

#### Resources
Icons 3Maps 1

I don't know about other options. I just followed the tutorial.

#### gadina_prokleta

#### gadina_prokleta

 Level 11

JoinedApr 11, 2006Messages338

Resources: 2

#### Resources
Maps 2

Ender Wiggins said:

 wow. I have literally spent hours on these loading screen tutorials... only this one finally came through... hallelujah.

I attached the loading screen mdx from monolith.

...


Click to expand...

My OS is Win10 and I was trying to create a loading screen for Reforged.

I confirm this was the only working solution for me as well.

I spent hours and hours across several days to try all outdated tools for exporting .blps with bad results (black screen, crashing game, etc.). The best result I got was from some tool that left me with a picture crumpled into 512x512 pixels and not widening when loaded by the game.

So, the only working solution was dividing the file in 4 parts as described above, exporting them as .tga and complementing them with the file provided by Ender Wiggins.

Since I don't have the patience for "freehand" cutting, I have created two Adobe InDesign files that do it for me.

If you want, use this archive.

https://drive.google.com/file/d/1WGQ8FgyMWV7OI12JUUqp5BksByR4Ur-W/view?usp=sharing

1. Replace the picture of LoadingScreen.jpg with whatever your picture is.

2. Load both .indd files and update the links.

3. Export them (Ctrl+E) as jpg-s with the correct names (LoadingScreenBL.jpg etc.).

4. Convert those .jpg-s to .tga using Photoshop or whatever program you like.

5. Import them using Asset Manager along with the LoadingScreen.mdx file.

6. Remove the unneeded war3mapImported\ from the paths.

7. Go to Scenario >> Map Loading Screen and set it to point to LoadingScreen.mdx

__________________

However, I still can't make a custom war3mapPreview.tga appear in the lobby - unless it works, but only for those joining it?


 Last edited: Jan 29, 2021

#### Barade

#### Barade

 Level 29

JoinedFeb 2, 2006Messages1,692

Resources: 5

#### Resources
Tools 1Maps 2Spells and Systems 2

Macadamia said:

 For Reforged and other versions supporting 16/9 resolution, use the attached loading screen model with a simple picture converted to BLP with no mipmaps and the name "Fullscreen.blp" (no path) in the asset manager.


Click to expand...

Thx but does Reforged support loading screen textures of any size or will it just be scaled do? This tutorial should be updated for Reforged. I would like to use bigger textures and I don't like to cust my images into 4 parts.

Weirdly Reforged places the text for custom loading screens at the bottom left?!

#### P3in

#### P3in

 Level 11

JoinedDec 16, 2017Messages437

Resources: 7

#### Resources
Packs 7

Hello, i am having this issue, does anyone knows how to solve it? I use patch 1.28.5

Where the 4 images bind, it appears a linear line between them..

I imported them as BLP

#### dizzilnate

#### dizzilnate

 Level 1

JoinedApr 14, 2022Messages2

Macadamia said:

 For Reforged and other versions supporting 16/9 resolution, use the attached loading screen model with a simple picture converted to BLP with no mipmaps and the name "Fullscreen.blp" (no path) in the asset manager.


Click to expand...

I tried making a 16/9 picture, however warcraft 3 viewer won't find the file (it's jpg) so I can't turn it into a blp...

#### dizzilnate

#### dizzilnate

 Level 1

JoinedApr 14, 2022Messages2

gadina_prokleta said:

 My OS is Win10 and I was trying to create a loading screen for Reforged.

I confirm this was the only working solution for me as well.

I spent hours and hours across several days to try all outdated tools for exporting .blps with bad results (black screen, crashing game, etc.). The best result I got was from some tool that left me with a picture crumpled into 512x512 pixels and not widening when loaded by the game.

So, the only working solution was dividing the file in 4 parts as described above, exporting them as .tga and complementing them with the file provided by Ender Wiggins.

Since I don't have the patience for "freehand" cutting, I have created two Adobe InDesign files that do it for me.

If you want, use this archive.

https://drive.google.com/file/d/1WGQ8FgyMWV7OI12JUUqp5BksByR4Ur-W/view?usp=sharing

1. Replace the picture of LoadingScreen.jpg with whatever your picture is.

2. Load both .indd files and update the links.

3. Export them (Ctrl+E) as jpg-s with the correct names (LoadingScreenBL.jpg etc.).

4. Convert those .jpg-s to .tga using Photoshop or whatever program you like.

5. Import them using Asset Manager along with the LoadingScreen.mdx file.

6. Remove the unneeded war3mapImported\ from the paths.

7. Go to Scenario >> Map Loading Screen and set it to point to LoadingScreen.mdx

__________________

However, I still can't make a custom war3mapPreview.tga appear in the lobby - unless it works, but only for those joining it?


Click to expand...

Is there no way to make it into blp for reforged? file size is already an issue with the map I'm working on.

#### dantles1992

#### dantles1992

 Level 6

JoinedAug 15, 2022Messages116

just wanna add that Paint.NET can convert jpg or dds or what ever to .tga and just use "Warcraft 3 Viewer" to turn tga to blp file, it works

 You must log in or register to reply here.


### Similar threads

 High Resolution Loading Screens


Mayday

Feb 7, 2020

2D Art Tutorials
2

Replies54Views27K

Jan 7, 2026
Elprede

Locked

 Loading screen.mdx


maplover

Aug 4, 2008

Requests

Replies1Views8K

Aug 4, 2008
lvfkotan

Locked

 Loading screen trouble


Death_bringer_2

Oct 30, 2010

World Editor Help Zone

Replies5Views2K

Oct 30, 2010
Death_bringer_2

Locked

 I need help with a loading screen


Cleaver

Feb 12, 2006

Map Development

Replies2Views1K

Feb 13, 2006
Tr!KzZ

T

Locked

 Loading Screen


TwAizZ

Feb 14, 2009

World Editor Help Zone

Replies3Views2K

Mar 2, 2009
Metov

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