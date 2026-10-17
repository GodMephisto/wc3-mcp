---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/complete-icon-tutorial-all-about-icons.37832/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Complete Icon Tutorial All About Icons
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/complete-icon-tutorial-all-about-icons.37832/>
#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

"Complete Icon Tutorial with ALL about Icons" Flame_Phoenix

[broke="Remember to POST comments; Give reputation POINTS and to RATE this tutorial."]IMPORTANT !![/broke]

After the unexpected success my first tutorial had Creating a Tavern by Flame_Phoenix i was astonished. I never expected my tutorial to have an excellent qualification by the users. In deed i remember trying to go to every detail and that seemed to work for me. Now, to thank that opportunity i decided to write a new tutorial. All about icons. I guaranty that when you finish reading my tutorial you will be a professional making icons - if you are new at icon making, don't worry, this tutorial it's just for you. It was made by purpose to people who never managed themselves into the world of icon making, but non less this tutorial will also give to advanced icon makers all the experience they need, as it also contains advanced icon knowledge. This super tutorial DOES HAVE EVERYTHING ABOUT ICONS. YOU WON'T FIND A BETTER TUTORIAL. To this tutorial i searched for all icon websites, and this is a work with all that information. As a work it is, it also has a Bibliography section you can visit and a few sites i recommend myself to all icon makers. Now to maybe end this note I am not going to say that I don't want this to go to the grave, because I know this work is to much good for that. To nearly end i just want you to comment my 3,5 months work and give suggestions. If you don't understand something, post it on the forum so than I can help you and make a better tutorial.

To finally end i want you to know that this is also my first and probably my last super tutorial, as the chances of you guys seeing me doing more tutorials will be small. Doing this tutorial gave the chance to be with the best people among icon making like BlackDoom and gave me the chance to learn tons of new stuff while making it. Still I must confess myself very disappointed with the moderators of the "Submit a Tutorial" section , as I only got the feedback I needed in the very final stage of my work (Special thanks to Purplepoot, Wolverabid and low_kwaliti). Making this tutorial gave me the most difficult times I have ever had in the Hive Workshop, but also the hope that it can give me the best moments I will ever have as well. So to finish I just want to say to ALL people who read super tutorials, to COMMENT them, even if you are the newest guys in the world to the subject, keep in mind that your WORDS, REPUTATION POINTS and RATING are the best help a tutorial creator can ever have. Even if you don't understand the subject in cause, I guarantee you that the simple word of Thanks is a very important word for all those who make tutorials. Psychologically it does have a GREAT IMPACT, so use it to do good.

And now before you start reading my tutorial I would like you to close this revealed window, as it does take needed space from the tutorial, thus damaging it's contend.

“General Introduction”

Hi, this tutorial will teach you people everything you will ever need to know about icons. This is a massive tutorial that gets together all icon information for Warcraft 3 TFT. It also applies to ROC users, but you will need an extra program to import the icon.

The programs I will use are:

Irfanview

W3IR

Warcraft 3 Viewer

Word Editor

I will use Irfanview as an image program. However you may choose to use other programs such as Photoshop, Paint Shop Pro, PhotoImpact, GIMP or others. In the end of the tutorial you will have the links to download this programs if you want.

In this tutorial you will learn how to:

• Create an Icon:

o Introduction

o Start with Irfanview

o Add Borders with W3IR and Irfanview for active game play icons

o Add Borders with W3IR for passive game play icons

o Converting to BLP with Warcraft 3 Viewer​
• Edit an Icon:

o Introduction

o Incomplete packages of images

o Transforming a 64x64 pixels image into a 48x48 pixels image

o Modifying an icon​

> Modifying with IrfanView

> Modifying with W3IR​

• Import an Icon

o Introduction

o Importing and using 64x64 icons

o Importing and using 48x48 icons​
• Fix an Icon

o Introduction

o Fixing green icons

o Fixing black icons​
• List of important known bugs in the programs

• Bibliography

• Websites and downloads

• Credits

• Special Thanks

This tutorial also has a list of other Internet sites that can help you if you need.

 “Creating an Icon”

 “Introduction”

Before I start I want you to know that I will not teach you how to create an image for an icon; I will just teach how to create the icon with the image. If you want to create a good image for an icon you can search in goggle, or, the most common solution, you can use a professional image program like those I mentioned in the start. In the end of this tutorial you will have the websites of the programs I mentioned, it is up to you to choose the best.

This means that to start we must know what an icon is. So, what is an icon?? Well, this is not a simple answer. There are two types of icons: icons which we use in active game play (BTN, DISBTN, PAS and DISPAS, I will explain what this is later) and icons that we use in passive game play (Type of attack, armor, level of upgrade, etc).

So we have:

1.Active game play icons – these icons are 64x64 pixels icons with 24 bit colors. We use these icons when we cast a spell or give an order.

​

2.Passive game play icons – these icons are 48x48 pixels and 24 bit, used by the game to determinate the level of an upgrade, they are referenced by the game as “special icons”.​

In this tutorial you will learn how to create both of these icons.

Both icons can be in TGA format or BLP format. Icons in TGA format do not save alpha channels which cause some icons to appear green or even black in the game, BLP format does save alpha channels and it is the format I recommend and use.

Green icon … sometimes there are also black icons. This happens when you do not do things correctly. It can be caused due the TGA format.

Each icon graphic for a unit / building consists of two basic icons, the main icon you see when normally playing the game (BTN - Button), and the darkened icon that you see when you hit F10 to display the menu (DISBTN – Disabled Button).

Other icons, such as the passive abilities (like endurance aura, Unholy aura, etc) use a different type of icon, an icon with no borders (PASBTN – Passive button) and the darkened version (DISPASBTN – Disabled Passive Button). PASBTN and DISPASBTN icons are not necessary for an icon to work properly, they are optional. Passive game play icons, are an exception as they can not have Passive Icons and Disabled Icons.

In this tutorial I will uses the programs:

• IrfanView ( You can use other programs that can save in BMP format and that can work with images, like GIMP, Photoshop, Paint Shop Pro, PhotoImpact, etc). IrfanView is available in http://www.irfanview.com/, or you can download the version I include on this tutorial.

• W3IR (available in this website, or you can download the version I include with this tutorial)

• Warcraft 3 viewer (available in this website, or you can download the version I include with this tutorial)

• Warcraft World Editor (to import the icons)​

Now I will explain what I will do during this section:

1. I will start by using IrfanView to "create an image". After that I will resize the image set it to 24 bit and save it in BMP format.

2. After that I will use W3IR to create the borders for my icon.

3. In the end I will use Warcraft 3 viewer to convert my image into BLP format.

4. It is done!, now I just need to import it.

​

 “Start with Irfanview”

To start install Irfanview. Now because Irfanview is a very limited program (and that is why my advice is to use better programs, like the others I mentioned), it can not create a new image. To solve this problem, I used a very simple and easy solution, Print Screen your Desktop! Or search for a BIG image in Google, I decided to search for a big image in Google. Now that the most difficult part is done (lol) save the image in a paste. Now use Irfanview and open the image with it. If you want to learn how to create and design your own icon, then visit the Websites and Downloads section and have a look on the websites I recommend.

MMM Shrek and Fiona are so nice together! Now it is time to resize our image which is very easy to do. Go to Image > and then use Resize/Resample, or if you are lazy you can just press CTRL + R.

Now uncheck the “Preserve ratio aspect” box, and choose size 64x64 for an active game play icon, or 48x48 for a passive game play icon. I will make an active game play icon, so I will use 64x64 pixels size.

OMG, MM it seems that Merlin did a great job, Shrek and Fiona are much smaller now =).

With our image resized it is time to set it to 24 bit colors. But, how do I know the color depth my image has? Well, to know the current Bit colors of your image just go to Image > Information or press “I”.

IrfanView tells me that the current Color depth of my image (number of color bits it has) it 24 BitsPerPixel. This is good news because now I don’t need to change anything.

But what if my image has too many color bits?

Well IrfanView does have a solution for that. To decrease the color depth of your image you just need to go to Image > decrease Color depth ( in this case, but if your image needs more colors, go to Increase Color depth) and set the color depth of the image to 24BPP.

Now to end this part of the tutorial, just save you image in BMP format. This is very easy to do. Go to File > Save as and there give a name to the image, and save it as a BMP image in a place you know where it is. I am saving the image in BMP format because the program I will use next works better with BMP. Also Warcraft 3 Viewer and Warcraft World Editor do have a few errors and bugs when they handle TGA images. This means that I will save the images in BMP and in the end I will convert them into BLP.

I saved my image in my desktop as you can see.

Now close IrfanView, we will not need it for now. Time to move to the next section.

“Add Borders with W3IR for active game play icons”

W3IR is a very complex program and I warn you that this will be the most difficult part of the tutorial. This program allows you to do many things, but for now, I will just add the BTN, DISBTN, PASBTN, DISPASBTN and the AUTOCAST borders to my shrek icon.

If you remember active game play icons are 64x64 icons, and so we will work out that format.

To start create 2 folders. One folder is where you have to place the image you just created now (in this case name it “In”), and the other folder is the folder where the program will place the Bordered images it will create (in this case name it “Out”).

Now place your recently created image in the “In” folder. After doing this open the W3IR program.

Good, now press the button Add Borders.

If you done this correctly, this new window will pop up:

Here is where the challenge begins.

If you look to the image you can see that there are 2 windows. In the left one, you must choose the “input” folder; this is the folder where you have your created image. In the right window, select the “output” folder, the output folder is the folder where the program will place the “Bordered icons”.

This means that in the “Input window” you have to select the “In” folder you created, and that in the “Output window” you have to select the “out” folder you also created before.

Now click in the 64x64 images button.

After this I will check all other boxes, the BTN, DISBTN, PASBTN, DISPASBTN, and the Autocast border boxes, just like in the image below.

Now it is time to name my icons.

Naming an icon is not easy as well, it has several and important rules:

• When naming your icons, make sure you put the letters "BTN" at the start of the BTN icon filename.

• Put "DISBTN" at the start of the DISBTN icon filename.

• Put "PASBTN" at the start of the PASBTN icon filename.

• Put "DISPASBTN" at the start of the DISPASBTN icon filename.

• Autocast icons are an exception. These icons are regular BTN icons, but they have a different border type. This means that when naming an Autocast icon, it needs to have “BTN” at the start of the file name.​

If you didn't understand what I wrote before, I will just give an example:

• BTNshrek

• DISBTNshrek

• PASBTNshrek

• DISPASBTNshrek

• BTNshrek_autocast​

So, resuming, the name of an icon is:

IconType + IconCustomName

Be careful, because, a BTN icon to correspond to a DISBTN icon, they both need to have the same IconCustomName. If this doesn’t happen, Warcraft will display a normal good looking icon during the game, but a green box when you hit F10.

Example:

BTNshrek + DISBTNshrek = works fine, good work

BTNshrek + DISBTNfiona = does not work, bad work

The PASBTN and the DISPASBTN icons work the same way, however the prefix is different:

PASBTNshrek + DIPASSBTNshrek = works fine, good work

PASBTNshrek + DISPASBTNfiona = does not work, bad work

The Autocast Icon is an exception, the program can not create a DISBTN version for the Autocast Icon, but don't worry, I will teach you later how to create it with only a few steps in IrfanView, it is very easy.

So, now that we know how to name our icons, let’s do it!

Good, now we just tell the program which type of file the folder has. Click "Please select the target image type:" and there choose BMP.

Now it is time to Create some Borders !!! Click the Border images button and the program will create the bordered images in the “output” folder according to the extension ( this means that if the image in the “input” folder is in BMP format, the bordered images created in the “output” folder will be in BMP format. However if the image in the “input” folder is in TGA format, the bordered images created in the “output” folder will be in TGA format, and so on).

After you press the button a box will pop up. Just press the “Yes” button as you have no other choice.

Great job until now. Now let’s open the “Output” folder where our new icons are.

Wow, great !! I have the icons! BUT WAIT A SECOND .. NOOOOOO the work is not done yet .. why !!.

Well that happens for 1 main reason:

• W3IR is a great program but it has 2 bugs in it. The first one that you can easily see is that the file name is not correct, if you look to the name of the icons, you can read that we have NormalIconName + IconType + IconCustomName which is not correct. If we place this in the Warcraft 3, it will cause it to crash. This is easy to solve, just change the name and delete the NormalIconName part; delete everything before the IconType part.​

Good, now we have the correct formula: icon type + icon custom name.

The second bug it has is that this program fails to create a DISBTN image for the BTN Autocast icon. This means that we will have o create our own DISBTN Autocast icon.

This is very simple to do and we will use Infraview.

To start creating your DISBTN Autocast icon, open IrfanView. Once the program is opened, use it to open the BTN Autocast icon you’ve created.

Now that you have the BTN Autocast file opened, it is time to remember what a DISBTN is. DISBTN icons are darkened BTN icons, so this means that in order to make our icon darker; we have to decrease its Brightness. To change the brightness of an image go to Image > Enhance colors or press Shift + G. A new window will pop up.

Now just set Brightness to “-100”. I’ve chosen “-100” because I believe it is a good value, if you wish you can try other values. Keep in mind that changing the other values such as “Contrast” or “Color Balance” per example, will change the quality of your image. Besides, we only need a darker image, so we only need to change Brightness.

Now just press “Ok” button. ”DO NOT CLICK IN “APPLY TO ORIGINAL” as you will loose the original image if you do so.

Now that we have our darkened icon, we will use the command “Save AS” and we will change its name to DISBTN_shrek_autocast (or whatever the name you choose).

When saving the icon keep in mind the formula and the fact that it needs to have the same name of its BTN correspondent.

BTN_shrek_autocast >> DISBTN_shrek_autocast

Good job now look at your folder. If you did all steps correctly and saved the images in BMP format, this is what you will see.

If you saved your images in TGA format, use IrfanView to see them.

Good job. Now that everything it is ok, let’s convert all this images to BLP format.

Close IrfanView, we won’t need it for now. Now create another folder and name it “BLP shrek”.

Now we have 3 folders, “In”, “Out”, and “BLP shrek” just like in the image. We will use no more the “In” folder now, we will work with the “Out” and the BLP shrek folders.

Next is the ”Add Borders with W3IR for passive game play icons” section of the tutorial. If you don’t wish to learn it and if you want to move to the final step please go to the ”Converting to BLP with Warcraft 3 Viewer” section of the tutorial.

“Add Borders with W3IR for passive game play icons”

Please before doing passive game play icons read the create active game play icons section of this tutorial, because it has all the basics you need to know. Passive game play icons are special BTN icons that DO NOT need a DISBTN correspondent. This makes our job a lot easier. Here I will teach you (I hope) how to create an Upgrade type Border and an Attack / defense type border. W3IR also has some limitations and these are the only types of borders it can create. You can eventually use these icons to do more things and to add to new sets of borders, new attack types, etc. Your imagination is the boss once you learned how to create active and passive game play icons.

I will assume that you already read the Introduction and the Start with IrfanView sections of this tutorial, if you have not read them yet, do it.

To start create 2 folders. One folder is where you have to place the image you just created now (in this case name it “In”), and the other folder is the folder where the program will place the Bordered images it will create (in this case name it “Out”).

Now place your recently created image in the “In” folder. After doing this open the W3IR program. Remember that the images in the “In” folder must be 48x48 pixels size and 24 bits colored.

Good, now press the button Add Borders.

If you done this correctly, this new window will pop up:

Here is where the challenge begins.

If you look to the image you can see that there are 2 windows. In the left one, you must choose the “input” folder, this is the folder where you have your created image. In the right window, select the “output” folder, the output folder is the folder where the program will place the “Bordered icons”.

This means that in the “Input window” you have to select the “In” folder you created, and that in the “Output window” you have to select the “out” folder you also created before.

Now click in the 48x48 images button.

After this I will check all other boxes, the Upgrade Border and the Attack Type border boxes, just like in the image below. I do not advice you to check the bordered images string box as it will just complicate things, and it doesn’t create any type of border.

Now it is time to name my icons. Let’s remember our rules. These 48x48 sized icons are regular BTN icons, and they do not need a DISBTN correspondent. This way what we need to know is:

• When naming your icons, make sure you put the letters "BTN" at the start of the BTN icon filename.​

If you didn’t understand what I wrote before, I will just give an example:

• BTN_attack

• BTN_upgrade​

To remember, the name of an icon is:

IconType + IconCustomName

So, now that we know how to name our icons, let’s do it!

Good, now we just tell the program which type of file is in the folder. Click Please select the target image type: and there choose BMP, if you saved the icon in BMP format, or TGA, if you saved the icon in TGA format.

Now it is time to Create some Borders !!! Click the Border images button and the program will create the bordered images in the “output” folder according to the extension ( this means that if the image in the “input” folder is in BMP format, the bordered images created in the “output” folder will be in BMP format. However if the image in the “input” folder is in TGA format, the bordered images created in the “output” folder will be in TGA format, and so on).

After you press the button a box will pop up. Just press the “Yes” button as you have no other choice.

Great job until now. Now let’s open the “Output” folder where our new icons are.

Wow, great !! I have the icons! BUT WAIT A SECOND .. NOOOOOO (lol, again) the work is not done yet .. why !!.

Well that happens for 1 main reason:

• Well, as I told you before, W3IR is a great program but it has 2 bugs. In this case we are lucky, because we are not affected by the second bug type, but only by the first bug type. If you remember well, the first bug type that you can easily see is that the file name is not correct, if you look to the name of the icons, you can read that we have NormalIconName + IconType + IconCustomName which is not correct. If we place this in the Warcraft 3, it will cause it to crash. This is easy to solve, just change the name and delete the NormalIconName part.

​

Good, now we have the correct formula: IconType + IconCustomName.

Do this to all icons.

Good job. Now that everything it is ok, let’s convert all this images to BLP format.

You can create a 3rd folder to save the BLP format images if you want. I decided that it will be more organized, so I created a 3rd folder named BLP shrek.

Now we have 3 folders, “In”, “Out” and “BLP shrek just like in the image. We will use no more the “In” folder, now we will work with the “Out” and the BLP shrek folders.

The next section will instruct you how about to convert BMP files into BLP files.

“Converting to BLP with Warcraft 3 Viewer”

Now it is time to head to the final step of icon creation !! This chapter applies to both active game play icons and passive game play icons. The following examples will be with the active game play icons but it applies to the others as well.

To start open Warcraft 3 viewer.

Now just follow the steps on the image:

If you did this correctly, a new window will appear.

Now press ok. A new window will pop up. This window is EXTREMELY IMPORTANT, and you must not ignore it. This window will make you a question about alpha channels. If you remember the introduction, an icon with no alpha channels, is probably a black or a green icon. So, to answer to the question say “YES, add alpha channel to all”.

After this important step, if you did everything right, a box saying that it was converted with success will appear. If it doesn’t it is because the program has a bug, or because the installation has some problems. In last case, you can read the hole tutorial again to see if you did something wrong. Post in the forum if you have any questions.

Now close the program and import the icon !! congratulations you now have advanced icon making knowledge !!! enjoy it.

You have created a professional blizzard icon ! =P.

Now use your imagination as it is your limit from now on ! create hundreds of icons and don’t forget to place them in this website!

If you don’t know how to import an icon into Warcraft, read my “Import Icon” section !

 “Edit an Icon”

 “Introduction”

Editing an icon is very similar to creating one, the steps are the same but the order they follow is different. Many people compare editing to stealing, this is not truth; when you edit an icon, you change the work of someone, you don’t steal it. However icons can be stolen, so when you edit someone else’s work, remember to give credit to the creator. Sometimes the creators of icons don’t make a complete package of icons with all icons they should have, other times, you just don’t like the color or some other detail per example. Other times in the map making job you want to create a different attack type, or different armor and you just can’t because you can’t find 48x48 icons.

This section of the tutorial will help you with all those problems, and I will do my best to make sure you will be an iconing master when you finish reading it. In this section you will also learn how to work better with Irfanview and hopefully you shall acquire medium/advance knowledge about the program itself. However remember that it is you who are editing the icon and not me, which means that it must be you who is going to do the creator’s part of the editing work. I will not teach you how to make major changes or completely new images; I will just give you the tools and the knowledge for you to do it.

 “Incomplete packages of images”

This is the most common problem. Most of the time, you find a perfect icon for something, but when you open the package it only has the BTN and DISBTN file or sometimes only the BTN. Sometimes you wan that icon for a passive ability, or for an autocast spell and you can’t because you don’t know what to do. This section of the tutorial will explain all this stuff to you, but remember that it is a lot similar to creating an icon. My advice is READ THE CREATE AN ICON SECTION of the tutorial.

So imagine that you are in the internet site of hiveworkshop and suddenly you see this icon:

OOHHHH SO BEAUTIFUL !!!! I love it !! my map must have it !!!

However when you download it you see a problem:

HEEYYY!! There is only a BTN icon … Where are the others !? OMG there is no others … crap I will have to create them myself ..

Yes it is truth, sometimes when you want a job well done; you have to do it yourself.

So to start let’s remember our steps. When we created an icon we first used IrfanView to create the image, after that W3IR and than Warcraft 3 Viewer. This means that now we will start backward. We will use Warcraft 3 viewer to convert our BLP file into a BMP file. Run Warcraft 3 viewer, and once there press the BLP>TGA icon like the image shows, but choose the BMP format when converting.

Ha Ha! There he is. Now press open and, after pressing open, choose the BMP format in the format section and press save (no need to change the name). You can also choose a different directory if you want. But I think there is no need to do that (it is just one image).

If you done this correctly the program will ask you the quality of compression. If you remember my long introduction icons must be 24 bits, so it is obvious that you must chose 24 bit compression. After that press ok.

The program can also have a bug (mine has) which will show a pop up box telling you that your icon was converted into BLP format successfully. If that happens just ignore it and don’t worry as your icon was saved into TGA format with no problems.

Now if you saved the BMP file in the original folder where the BLP file was (like did) you probably have something like this by now:

This means you are doing a good job.

Now that you converted your icon into BMP format we should use W3IR, however there is a problem. If the icon you downloaded already has a BTN border (probably has, known sites as Hive, per example, only accept icons with borders) it means that if we try to create an Autocast icon (per example) the result will be an icon with two borders – the autocast border and the BTN original border of the icon.

An example of an icon with 2 borders – the BTN border and the Autocast border.

To solve this problem we could use the DISBTN icon than but, if we do so our images will be so dark in game that it will be nearly impossible to see them. Also, the DISBTN icons are also border. Than there is only one way out – we remove the BTN border from the BTN icon with IrfanView. If the icon you downloaded has no borders (unusual) you are lucky and you can jump this part of the section and move to the part where I use W3IR. Other way, if you don’t need an Autocast icon, you can also jump this section of the tutorial.

To start run IrfanView, than go to File > Open and search/open for your recently converted BMP file so you can work it.

Once the icon is opened in the program, zoom it to a zoom between 300 and 500. It is important that you choose a value which is a multiple to number 100 (like 100, 200, 300 ...). To understand why, please read the "List of important known bugs in the programs".

Now with you mouse (the arrow) select the inner part of the image (don’t select the border, this requires some ability, if you did this well done, when the image is small again, you will not see any gray areas from the border).

Now go to Edit > Crop selection or just press CTRL + Y.

If you done this hard step correctly this is what you will see when if you amplify the cropped image.

No gray areas form the borders as you can see.

Images with no borders are 56x56 pixels sized. This value is very difficult to obtain when cropping so don’t worry if you don’t get it, just try to get as closer as possible. You can see the pixels size by pressing CTRL + R.

Now if you did this well, you have created another small problem (lol), your image should be 64x64 pixels size and now it is 56x56 pixels. Fortunately this problem has a quick and simple answer. Just press CTRL + R (Resize/Resample), uncheck the box that says Preserve aspect ratio and resize your image to 64x64 pixels size.

Good job, however remember that an icon to be well accepted must have 24 bits (24BPP). To see if you icon has this value just press “i”.

My icon has the correct value so it means that I can move on. If yours does not have it you should go to Image > Decrease Color Depth or Increase Color Depth and choose the 24 BPP option. For additional help on this section you can read the Create an Icon section.

Now that our image is ok, we will save it, overwrite the old file with this one if you may prefer. I will just press “s” and overwrite the file I no longer need.

Now that we had all this work with removing borders, it is time to add them with W3IR - Yes, life is ironic I know …

If you remember, last time we used W3IR, we created an “In” folder and an “Out” folder. This time my “In” folder will be where these two icons (if you remember the BMP file we just changed is in the same folder the BLP icon you downloaded is) are ( this means that our “In” folder is “Shrek_cat”). So now we just need to create an “Out” folder. Create a folder somewhere you know where to find it and name it “shrek_cat_BMP_Borders”; this folder is important as by the means of logic it will work as our “Out” folder (“out” folder is the folder where our bordered icons will be).

Now open the W3IR program and click in Add Borders.

If you done this correctly, this window will pop-up.

Good, now it is just like creating an icon. Select the “shrek_cat”/the folder where your two icons are) in the Input window (left side) and the “shrek_cat_BMP_Borders” in the Output window (right side). Now choose the pixels size of the image (normally is 64x64 because the Hive Workshop and all other sites rarely have 48x48 pixels sized images), check the BTN, DSBTN, PASBTN, DISPASBTN and the Autocast boxes (just check the type of border that you want) and name your images (read the create an active game play icon section to better understand this). Finally select the type of image you want to convert (in this case BMP).

To finish just press the Border images button and say yes to the window that will appear.

Now if we make a quick visit to our “Out” folder (shrek_cat_BMP_Borders) we will see a “bug” from the program.

The program saved all our images with the previous name. This “Bug” doesn’t mean that the images don’t work, because they still work, however if we import them into Warcraft this way, we won’t know which images are passive, autocast or even disabled. This will cause green icons and in the worst scenario game crashes. However this fearsome bug is very easy to delete. Just change the name of all icons to their correct formulas: IconType + IconCustomName.

This means that (in this particular case) we must remove the “BTN_shrek_cat” part of the name that is repeated in the start.

These are the names you should have after you change the name of the files to the correct formula.

Now to end I will create a folder named “BLP cat”. This is the folder where you will place the BLP icons we will create now. Yes, if you read the Create an Icon section you will understand why it is so important to use BLP icons instead of TGA.

After creating the folder open Warcraft 3 Viewer and once there convert all your BMP files into BLP files with the method described above.

Now just follow the steps on the image:

If you did this correctly, a new window will appear.

Now press ok. A new window will pop up. This window is EXTREMELY IMPORTANT, and you must not ignore it. This window will make you a question about alpha channels. If you remember the introduction, an icon with no alpha channels, is probably a black or a green icon. So, to answer to the question say “YES, add alpha channel to all”.

After this important step, if you did everything right, a box saying that it was converted with success will appear. If it doesn’t it is because the program has a bug, or because the installation has some problems. In last case, you can read the hole tutorial again to see if you did something wrong. Post in the forum if you have any questions.

Done, you edited my incomplete icon!!! Now you just need to import it into Warcraft 3! If you don’t know how to import an icon go to the Import icon section of my tutorial. Congratulations as you already know how to complete those incomplete packages of images =)

 “Transforming a 64x64 pixels image into a 48x48 pixels image”

MM, imagine that when you download an icon you want that icon to represent an armor, or even an attack type. The first problem you will have is that it is very hard to find and download 48x48 from any website. People just prefer to do 64x64 pixels icons because they are more used. So, the only option that is left for you is to learn how to convert a 64x64 icon into a 48x48 icon.

Well I created a 64x64 icon with another shrek image and it has the name of BTN_shrek2.BLP. I created a folder to this new icon and named it Shrek_64x64. You should also create this folder, unless if you already have it.

To start we must convert our BLP icon to BMP format with Warcraft 3 Viewer. So, let's do it =) .

Run Warcraft 3 viewer, and once there go to the BLP > TGA icon, just like the image shows.

Now just follow the image, and save your icon in BMP format:

If you did this correctly the program will ask you the quality of compression. If you remember my long introduction icons must be 24 bits, so it is obvious that you must chose 24 bit compression. After choosing your option press ok.

The program can also have a bug (mine has) which will show a pop up box telling you that your icon was converted into BLP format successfully. If that happens just ignore it and don’t worry as your icon was saved into BMP format with no problems.

Now that you converted you shrek2 icon into BMP you have something like this in the Shrek_64x64 folder.

Well because 64x64 BTN icons have borders and 48x48 BTN icons don’t we will have to remove the borders from our recent BMP icon. No problem, if you read the last section you will see that we already did this step before.

To start run Irfanview, than go to File > Open an search for your recently converted BMP file so you can work it.

Once the icon is opened in the program, zoom it to a zoom between 300 and 400 keeping in mind that the value you choose must be a multiple of 100, to avoid a bug from the program. To know why visit the List of important known bugs in the programs section of the tutorial.

Now with you mouse (the arrow) select the inner part of the image (don’t select the border, this requires some ability, if you did this well done, when the image is small again, you will not see any gray areas from the border).

Now go to Edit > Crop selection or just press CTRL + Y.

If you done this hard step correctly this is what you will see when if you amplify the cropped image.

No gray areas form the borders as you can see.

Images with no borders are 56x56 pixels sized. This value is very difficult to obtain when cropping so don’t worry if you don’t get it, just try to get as closer as possible. You can see the pixels size by pressing CTRL + R.

Now it is different, if you did this well, you have created another small problem (lol). However you can take advantage of it. As you know the image we want to create is 48x48. Following the steps, we started with a 64x64 pixels image and now we have a 56x56 pixels image (image with no borders are 56x56 pixels). Now follow the logic, the time for us to resize the image to 48x48 pixels has come. Just press CTRL + R (Resize/Resample), uncheck the box that says Preserve aspect ratio and resize your image to 48x48 pixels size.

Good job, however remember that an icon to be well accepted must have 24 bits (24BPP). To see if you icon has this value just press “i”.

My icon has the correct value so it means that I can move on. If yours does not have it you should go to Image > Decrease Color Depth or Increase Color Depth and choose the 24 BPP option. For additional help on this section you can read the Create an Icon section.

Now that our image is ok, we will save it, overwrite the old file with this one if you may prefer. I will just press “s” and overwrite the file I no longer need.

Now that we had all this work with removing borders, it is time to finally transform our icon into what we want – a passive game play icon.

To start close Irfanview. After that create a folder and name it “Shrek_48x48_BMP”. This folder will be our “Out” folder. To know what an “out” folder is read the Create an Icon section of the tutorial. Now finally open W3IR.

If you done this correctly, this window will pop-up.

Good, now it is just like creating an icon. Select the “shrek_64x64” in the Input window (left side) and the “Shrek_48x48_BMP” in the Output window (right side). Now choose the pixels size of the image (in this case is 48x48), check the “Upgrade Border” and the “Attack Type border” boxes and name your images (read the create an active game play icon section to better understand this). Finally select the type of image you want to convert (in this case BMP).

To finish just press the Border images button and say yes to the window that will appear.

Good, now we have our shrek icon turned into what we want. If you did this correctly this is what you will see when you open your “Out” folder.

Now if you look closely to the images you can see that there is something wrong - the icon Formula.

The program saved all our images with the previous name. This “Bug” doesn’t mean that the images don’t work because they still work, however if we import them into Warcraft this way, we won’t know which images are passive, autocast or even disabled. This will cause green icons and in the worst scenario game crashes. To solve it, just change the name of all icons to their correct formulas: IconType + IconCustomName.

This means that (in this particular case) we must remove the “BTN_shrek2” part of the name that is repeated in the start.

 The correct name for the icon.

These are the correct names you should have.

After this step, just close W3IR as we won’t need it anymore.

Good, now let’s finish our job. Create another folder named “Shrek_48x48_BLP”.

Now we have 3 folders:

Shrek_64x64

Shrek_48x48_BMP

Shrek_48x48_BLP

After creating the folder open Warcraft 3 Viewer and once there convert all your BMP files into BLP files with the method described above.

Now just follow the steps on the image:

If you did this correctly, a new window will appear.

Now press ok. A new window will pop up. This window is EXTREMELY IMPORTANT, and you must not ignore it. This window will make you a question about alpha channels. If you remember the introduction, an icon with no alpha channels, is probably a black or a green icon. So, to answer to the question say “YES, add alpha channel to all”.

After this important step, if you did everything right, a box saying that it was converted with success will appear. If it doesn’t it is because the program has a bug, or because the installation has some problems. In last case, you can read the hole tutorial again to see if you did something wrong. Post in the forum if you have any questions.

Done, you transformed a 64x64 complete icon into a 48x48 icon!! Now remember, that attack type icons can be used as armor icons and others. As you can see you will soon enough be ale to do anything with icons. Just stay around to see my next chapter. Now you ca import your icon to Warcraft 3 !! Have fun !

To create a 64x64 pixels image from a 48x48 image just follow this steps as well, however don’t remove the borders and use Irfanview to resize you image to 64x64. After that Select the 64x64 option in W3IR.

If you have trouble converting a 48x48 into a 64x64 icon, just read the Create an icon section. If that doesn’t help you just post something in this forum and I will create a section for it (If there are many post requesting the same).

If you don’t know how to import an icon go to the Import icon section of my tutorial.

 “Modifying an Icon”

There are hundreds of ways to modify an already existing icon. In this section of the tutorial I will use Irfanview and W3IR to make a few changes in the icons. Remember that Irfanview and W3IR are not professional image programs so they are very limited. What I am going to put here all programs have. My advice is for you to work with a professional image program because you will have more freedom to do whatever you want.

I will start by using Irfanview and after that W3IR.

 “Modifying with Irfanview”

So to make a quick introduction, Irfanview is a program wit limitations but still a program all can use. This is the main reason why I am dedication to much time to it. I decided not to use trials because they have several bugs, take hours to download and mainly because they only last 30 days … What you are going to learn here also applies to all other programs I mentioned making of Irfanview the perfect program for minor/medium changes in icons. If you wish to make radical changes I suggest professional programs like those I mentioned in the start.

So we will learn a few interesting things here with Irfanview. The icon I will use is this one:

I named my icon BTN_shrek3.bmp ; yes I will work with BMP files because Irfanview does not support BLP files and because they offer more vantages when being edited, unlike TGA files. You can use TGA files as well, they will not be a bad choice but I recommend BMP. Non less, I also made a BLP version of this icon, so if you wish you can download it.

Anyway, after entering IrfanView and after you opened your image, use the zoom and zoom your image to a value between 200 and 400, remember that the value must be a multiple of 100. To know why visit the List of important known bugs in the programs section.

Now, all image editing function of this program are available in the Image menu. I will use some so you can see their effects.

If we have a look on the menu we will see the functions are in groups. First we have the image information’s, after the creating an image functions, rotating functions, resize and Canvas functions, the Color Depth functions and finally the coloring/editing/effect functions.

I will not loose time with the information, creation and rotation functions by some reasons:

 Information function only gives you information about your image. As we saw earlier it is very useful to know the color bits of the image and other things but it can not be used to edit anything.

 Irfanview can not create an image all by himself; it needs a base image to do it. Even if you create an image with Irfanview, it will never be as good as other images created with professional programs. To resume, this section is not important for icon modeling

 Rotation functions are extremely easy and intuitive of using. They just rotate the image or flip it. They are useful to icon modeling but to resume they are so easy to use, I think everyone can understand what they do. This means that you don’t need my help in these functions. Just experiment them.

Next are the Resize/resample and Canvas functions. The resize functions are very often used in this tutorial so I think that you already know what they do – they resize. If you have troubles with the resize functions, just read the Edit an icon or theCreate an icon sections of the tutorial.

So we have the Canvas function left. Canvas function creates an area around the image. It is just like a picture in a wall. Canvas adds a border to an image. So you are wondering, why can’t we use Irfanview to make a BTN icon? Well we could use Irfanview to do that but the quality of the icon would be very poor. Irfanview is not detailed to make 3D buttons (although he does have that function), and when it does them they loose quality when compared to border programs like W3IR. Anyway I will just create a nice blue border.

To create a Canvas border go to Image > Canvas size.

And now here is the result:

Nice don’t you think?

Now next to the Canvas function we have the Color Depth functions. These functions are used to change the colored bits of an image. Its job in icon modeling is to change the color depth of an image to 24 BPP. So all this means that the Color depth functions are very useful but not here. I want to show effects and changes and besides, you already are familiar with this function, we used it a lot in the previous sections of this tutorial. So, if you have any questions, just read the Edit an icon or theCreate an icon chapters of the tutorial.

Next to this function we have the coloring/editing/effect functions. These are the best functions to anyone use if you want to change something in an image.

If you look to the menu first is the Convert to Greyscale function. It is not hard to know what this does … it makes your image grey.

To access this function go to Image > Convert to Greyscale.

Not difficult at all.

After this, we have the Negative function. It is also very easy to use and to know what it does – converts your image to negative form.

To access this function go to Image > Negative.

It seems you are watching some of those weird blocked channels that you have to pay to be able to see them… like SportTV (what were you thinking!! you are sick! =P).

Now we have the Enhance colors function. If you remember we already used this function to create a DISBTN_Autocast icon. This function is extremely useful when editing.

 To access it go to Image > Enhance colors.

As you can see you can change the Brightness, Contrast, Saturation, Color Balance and Gamma correction values of the image. This allows you to make moderate changes in it. With the right values you can improve your image or just have some fun seeing what happens. This extremely useful tool can not be thought. We must use it and experiment it in order to understand what it does. Per example, if you want to create a DIS icon, you just need to set the Brightness values to negative. If you want your image to be redder, just change the color balance, if you want it to be more transparent change the Gamma correction values and so on. You choose what you need and what you do.

Next to this important tool we have another decisive function – the Auto adjust colors function. It is not difficult to know what this wool does; it balances the colors of your image. You should use this tool to improve you icon as harmony between colors is very important. My icon is already balanced so I don’t need to use this function (DreamWorks did a great job when they created this wallpaper =P), but I strongly recommend that you do so.

To access this function go to Image > Auto adjust colors.

After the Auto adjust colors function we have the Sharpen function. This function allows you to see your image better by increasing the Contras levels it has. This means that you can use it to improve small details, but if you use it too much it will eventually ruin your image. My advice is using this function if you need, but always keep the original file for backup.

You can access this function by using Image > Sharpen

This image is ruined because I used the function Sharpe too much. Fortunately I have my original file as I backup. Phew …

Next to he Sharpen function we have the Red eye reduction (selection) function. This function is used to remove red eyes when you take a picture, so we won’t need to waste time with this function unless you want to convert one of your photos into a Warcraft icon. I won’t explain this function because I would waste my time if I did so and because there is another extremely important function next to this one.

I am talking about the Effects function. This function allows you to create several effects for you icons. When using this function I recommend that you use it by going to Image > Effects > Effect Browser. The Effect browser allows you to see the result of your experiments in a instant way.

Here I used the Shift Colors effect. There are lots of them for you to use.

Another great thing about effect, is that you can combine them !!

In this image, after saving the first effect I am using a second one – the Swirl effect.

This is what my icon looks like with the 2 effects together.

Now in the last place, my favorite function – the Swap Colors function. This function swaps the colors of your icon using the RGB (Red, Green and Blue) definitions. It is very easy and beautiful to use, you can use this to create a new ambient for your icon and, just like the effects, you can double use it!

To access this function go to Image > Swap colors > chose anything!.

This is my new purple ogre ! (lol). To do this I used the Image > Swap colors > RGB -> RBG function.

Now say hello to my new weird ogre! After using the first function (RGB -> RBG) I decided to use RGB -> BRG, and this was the result.

Quick and simple! There is nothing better =P.

Now you learned the most important, and the best editing tools Irfanview has. You now have advanced knowledge about this program (together with the information within the other sections). However as I told you before this program is limited and I recommend you to explore new image programs. Now it is time to use W3IR.

 “Modifying with W3IR”

If you don’t have time to experiment changing an icon with Irfanview, or you just don’t have the patience to do it, than W3IR is your solution. This program allows you to make minor changes for your icons while bordering them.

Do you think that your icon is really cool?? Than try to see how it is with some weird effects and make it better! Why have only 1 icon when you can have a whole bunch of them!!??

This section will show you why it is so important to save your files in BMP format! This section only works for those who saved their icons in BMP.

So here it goes. I will use another function of the program. Before using this section of the tutorial I strongly recommend that you read the Create an Icon section because you will need an icon to use this function of the program. This section applies to both 64x64 and 48x48 icons as well as the others.

Now a quick summary, after saving your images in BMP format, remember that you must have two folders – an “Input” folder and an “Output” folder.

Now to start Open W3IR and click in the add borders button, like the image shows.

After all this click in the RGB scramble button.

Now a window will pop up with white and black boxes don’t worry, we will give a good use to them soon.

Now click in the Load Icon button, and search for your image so you can open it.

Now that the program loaded the image, just click in the Process button.

MMM I wonder what will happen.

WooooW look at that!! 15 new weird icons!!! Crazy Merlin did his crazy stuff again =P.

What happened is that when you pressed the “Process” button, the program just changed all Red, Green, and Blue channels. It also created a few more effects, as you can see.

MMM I am particularly interested in the GBR image. It will probably be nice in my map. I want it!!

To select an image you like just check the box bellow it. In this case, I will check the box that says GBR.

Now all I need to do is tell to the program the format I want for this image. I will choose BMP.

To save your image press Save imgs button and say “Yes” to the box that will appear. BE CAREFUL, the box says that if you place 2 icons in the same folder, 1 original, and the other an RGB modification, with the same format the program will replace the first icon (the original one) with the RGB modification. You may want to be careful when selecting your “OUTtput” folder in the “OUTput window”.

Congratulations, Now you now how to do small/medium changes on icons. Imagine that you are looking for an ice arrow image, but instead you find a perfect fire image ... Just use the change the RGB channels and make it blue !! Or imagine you want an icon to represent stun or dizziness, just use IrfanView to do it!

You now have advanced knowledge about these 2 programs (together with the information of the other chapters) so feel free to use it =).

Now it is time to import the icons =)

 “Importing an Icon”

 “Introduction”

Importing an icon is the basic knowledge that I assume the most of you already have. However I still advice you to read it. This importing section is not only to teach how to import icons… there are already hundreds of cheap tutorials teaching that … I will try to go further, I will try to teach about how to use your created icons. Sometime you create a perfect icon to be shown in your map; you want that icon to represent something in your map beyond ladder values like gold and lumber … imagine that you want your icon to represent a source of plasma energy that will be used instead of gold or lumber but you don’t know where to put that icon. In this section I will teach how to import correctly and how to use imported icons as well within the world editor.

 “Importing and using 64x64 icons”

Great, now that you already have you icons created (or selected) it is time to import them into the game. But how do we do that? Well if you have TFT version this will be easy, if only have ROC there will be a few problems. This tutorial will explain how to import icons in TFT because with ROC you will need a special program to do that.

Importing to TFT is relatively easy.

To start open Warcraft. After that go to the Import Manager (F12).

Good, now a new window will appear. In that window choose the Import File option or just press CTRL + I.

Good, now once you click in it you just need to browse for your files (icons). This is the easy part.

Once you have done the importation of your icons you will have something like this.

If you read the image you will see it is right. The names of the icons may be correct, however if you play the game with that path your icons will be green. This means that the path is incorrect and that you must change it. But how to know the correct path for your icon? Well there is another formula for that, depending on the type of your icon.

<>ReplaceableTextures\CommandButtons\BTN + IconCustomName( _cat in this case) + .IconFormat( BLP in this case) = BTN_cat.blp

<>ReplaceableTextures\CommandButtonsDisabled\DISBTN + IconCustomName + .IconFormat = DISBTN_cat.blp

<>ReplaceableTextures\PassiveButtons\PASBTN + IconCustomName + . IconFormat = PASBTN_cat.blp

<>ReplaceableTextures\CommandButtonsDisabled\DISPASBTN + IconCustomName + . IconFormat = DISPASBTN_cat.blp​

These are the paths you need to know. So basically we have ReplaceableTextures\CommandButtons\ + IconType + IconCustomName +. IconFormat

This is not as hard as it may look. However keep in mind that the BTN path is used to the Autocast Icon as well and is used for the all 48x48 icon too.

Now we just need to change the path of the files to the correct formula. To change a file path, double click on the file. After that a new window will appear. In that window check the box saying Use custom path and than just use the formula.

When you finish importing the icons, you should something like this image:

Now it is time to use our imported icons. BTN and DISBTN icons are used to both units and abilities, however, PASBTN, DISBTN and Autocast icons are used only for spells and abilities. I will now teach you how to use your icons. This is extremely easy I think everyone can do it, however I will loose some of my time teaching you.

Now to use your icons, close the Import Manager and open the Object editor.

Very easy until now. Now I will first use my icons for some abilities and than for units.

To change the icon of an ability select the abilities section of the object editor.

Now I will change the icons of the archmage and the priest abilities. I will start by the archmage.

Well this is how it is done for heroes. You need to change those two values in point 2 because there are two images – the game icon and the learn/research icon.

For the passive abilities such as the Brilliance aura use the PASBTN icon. Here you don’t need to worry about any DIS file because, if the path is correct and if the IconCustomNames of the BTN and the DISBTN are the same, the DIS version will be shown automatically by the game when you press F10.

Now for the priest I will edit the Autocast icon. Again you don’t need to worry about it’s DISBTN version as it will be shown when you press F10.

Autocast need two fields because they have two types – when they are on and when they are off. Normal icons don’t need the off version, so you only need to change the first value because the other one is None[/I ] by default.

Now I will change the icon of peasant into a beautiful cat. Unit’s icons are very similar to normal abilities icons because you only need to change one field.

To start move select Units in the object editor.

After that select the peasant and follow the image.

These are the most important things about importing 64x64 images. I will not teach you more details because I think it is very basic to do what I just did. Now the 48x48 are a bigger challenge.

You will probably be surprised with what you can do with them and I strongly advice you to read the next section as it has very important things that can be applied to 64x64 icons as well.

This part of the tutorial has a map. You can and you should download it to better understand the lesson.

 “Importing and using 48x48 icons”

Now that you know now to import 64x64 icons, importing 48x48 icons will be the same. It will be even easier because 48x48 are only BTN icons who don’t need a DISBTN correspondent.

To start open Warcraft. After that go to the Import Manager (F12).

Good, now a new window will appear. In that window choose the Import File option or just press CTRL + I.

Good, now once you click in it you just need to browse for your files (icons). This is the easy part.

Now, once you have done the importation of your icons you will have something like this.

Now it is time to change the paths. If you remember our last chapter the BTN paths will be:

<>ReplaceableTextures\CommandButtons\BTN + IconCustomName + .IconFormat( BLP in this case) = BTN_attack(or _upgrade).blp[/INDENT

So it is time to correct this. To change the paths, the process is equal to the one I explained before.

When you finish importing you will have something like this.

Now it is time to use our icons. The newest map makers will defiantly be surprised about what they can do with these icons.

To start close the Import Manager.Then, in the main menu, go to Advanced > Game Interface. After that check the box saying Use custom interface.

Good. Now it is time to make some damage!

I will change the icon of the normal attack into my new shrek icon.

As you can see we must change those two values. The first one is the icon with the upgrade border, and the second one is the icon with no upgrade border.

This means that in the first filed we will place our BTN_upgrade.blp icon and in the second field the BTN_attack.blp icon.

I will start by changing the first field, and than the second.

Do the same to the second field but choose the BTN_attck.blp icon.

When you end this part, you will have something like this:

Continues on the next post​


 Last edited: Oct 5, 2007

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Continuation

As you can see, if you continue on, you will be able to change most of the values, like gold, lumber and food. However be careful as the Attack, Move and other icons are 64x64 pixels images. In this case you must know the game to know which icons you can change.

You can also use the BTN_attack.blp to change armors. In my map I changed the piercing attack and the large armor.

If you find a 64x64 icon (like move icon) and if you want to change it, you will have to use a 64x64 image, but the process will be the same. However if you use a 64x64 icon remember that you will need its DISBTN correspondent (yes, you will want to use BTn icons because PAS icons will be wasting your time as you won’t even be able to see the properly).

Now I will change the adept/master training icon of the priest. To do this enter the object editor and choose units

Now select the priest unit and choose the field caster upgrade art so you can change it.

It is done. Now you know a little bit more. I decided not too loose much time in this section as well. Because of what I said before, I consider importing and using very easy to do.

If you have any questions or if you want to learn more just post something in the forum and I will see what I can do. Importing and using the icons it’s the easiest part of icon making.

This section of the tutorial has a map as well as the other. You can and you should download it to better understand what is I the tutorial.

Now it is time to end my tutorial with my last section – Fixing an Icon

 “Fixing an Icon”

 “Introduction”

In contradiction to the other sections of this tutorial, I will not tell you exactly what to do in this section. To understand the meanings and the concepts I am going to write here you have to read my tutorial. However if you already have some knowledge about icon making you will probably understand what I mean. Next to this section there is also has a list of bugs that the programs you use can have, so I advice you to read it.

 “Fixing Green Icons”

There are millions of ways to make an icon to appear green in Warcraft, however I will write here the most common.

I will start from the beginning. A common problem is the size and color depth of an icon. If an icon does not have 64x64 or 48x48 pixels and does not have 24BPP (24 bits color) it will be green. To solve this problem convert your icons into BMP and set the icons to 64x64 (or 48x48) pixels and 24 BPP

After that we have the name of an icon. To be well accepted by the game the icon name must follow a formula. The formula is: IconType + IconCustomName + .FileFormat = BTN_myicon.blp , per example.

If the icon name is correct check for its correspondent, remember that 64x64 icons MUST always have a DISBTN correspondent, with the same name. In this case the formula is: DISIconType + IconCustomName(equal to the correspondent icon name) + .FileFormat = DISBTN_myicon.blp . PASBTN and DISPASBTN are NOT an exception to this formula. Only 48x48 icons don’t need a DIS correspondent

Finally and the most common cause the Import path is usually wrong. To correct an import path use the formula: ReplaceableTextures\CommandButtons\IconType + IconCustomName + .FileFomat = ReplaceableTextures\CommandButtons\BTN_myicon.blp .Keep in mind that he previous rules also applies here.

These are the main reasons why your icons don’t work. If you have more questions or any trouble please post them in this forum so that I can help you solve your problems.

 “Fixing Black Icons”

The main reason (and the only I know) for icons to appear black in game it is because they don’t have alpha channels. This problem is most frequent in TGA format icons. To solve this problem convert your icon into BLP format with WarcCraft 3 Viewer. This program will convert your icon and it will add the so needed alpha channels automatically.

 “List of Important Bugs in the Programs”

Irfanview:

1. - Sometimes you can not undo your actions. I recommend that you save you files with regularity.

 2. - Also sometimes when you zoom to a value which is not a multiple of 100, your zoomed image can get "cut" in some areas, thus i am forced to recommend you people to zoom to values that are multiples of 100 (like 200, 300 and so on).​

W3IR:

1. - This program cannot create a DISBTN file for the Autocast icon. This means that you have to create it. Read my tutorial as it teaches how overrun this bug.

2. - Although this program can name icons, when you finish adding borders to them, the program doesn’t work properly. In the case the bordered new icons have a double name which needs to be deleted manually.​

WarCraft III Viewer:

1. - Sometimes the program crashed due an unknown issue. My advice is for you to save your files regularly.

2. - When you finish converting a BLP file into another type of file, the program says that the issued file was converted into BLP successfully. Ignore this bug as it causes no harm to you, your file was converted correctly in the desired model.

3. - When you convert a TGA image into a BLP image, the program flips vertically your image. This means that what should be up will be down and what should be down will be up. There are two way of correcting this bug:

a. Save your files in BMP format and convert them into BLP

b. If you have already saved them into BLP format you will have to convert the BLP file into a BMP file. After that use Irfanview to flip vertically your recently BMP created image. Once it is in the correct position convert the BMP file into a BLP file again.​

WarCraft III World Editor:

1. - Sometimes when using imported icons in the Game Interface menu the program just does not recognize the file. To solve this either create another map or close the current map and open it later. My advice here is to always save regularly your job and to always use a test map file where you do your experiments such as changing important game values like this one.

2. - Happens that after importing an icon the program does not recognize it immediately in the editor. To solve this save the map, close the program and reopen it a few minutes later. It also can happen that when making your own map that the amount of data information is so big that the program does not recognize the icons in order to have extra virtual memory. This bug cannot be solved but don’t worry the icons will always appear when playing regular Warcraft.​

These are the bugs I experienced while I was creating my icon. I hope they help you avoid some of the troubles I had. If you had any extra bug problems please report them as I would like to complete my tutorial as much as possible.

Done! Now you are a professional with icon making !!! I hope you enjoyed. Down here it is my Bibliography section and the links of some professional image making programs you can experiment. You can also download the installation files of the programs I used together with my maps.

 “Bibliography”

To create this super tutorial I used information from:

• Creating Custom Icons by Darg

• 6 years of WarCraft professional game play and 4 years as a lonely map maker (yes it took me a lot of time to know what I know now)

• W3IR program tutorial (here available for download)

• THW rules about icons (also available for download)​

 “Websites and Downloads”

 “Websites”

Here you will found a list of other icon making websites and tutorials that will help you:

Icons Guide by Ralle

Importing and Fixing Icons(Roc and Tft) by ElfWarfare

My icon is green in game ... why? by ElfWarfare

Game Interface - Basics by jonadrian619

How to make an Icon by Tiki

 “Downloads”

Here you will find a list of programs you can use. This includes professional image programs and the programs I used:

Paint Shop Pro

Adobe PhotoShop

Ulead PhotoImpact

GIMP

IrfanView (you can download the version i include on my tutorial)

W3IR (you can download the version i include on my tutorial)

Warcraft III Viewer (you can download the version i include on my tutorial)

 Credits


me

myself

I

Flame_Phoenix (me, myself, I)

 Special Thanks


 Thx to BlackDoom for giving his opinion and making constructive critics on my work. Thx to him I improved my tutorial.

 Thx to Wolverabid for telling me about BlackDoom and for his comments

 Thx to PurplePoot for being the first and nearly only moderator that motivated me to finish the tutorial

 Thx to low_kwaliti for defending me against some angry moderators and for commenting my work as well.

VERY VERY THANKS to all these people and to all those i forgot to mention that somehow contributed with their comments and opinions to help me make a better work. Also thx for the reputation points they all gave =D .

Now here are the maps, icons and programs that you can and you should download. Hope you have fun making icons now =)

Also the Import maps with icons are here. You should download them to better understand my tutorial if you want to.

#### Attachments

warcraft 3 viewer.rar

 1 MB

 · Views: 966



W3IR.rar

 743.9 KB

 · Views: 1,251



Importing and using Icons.rar

 76.8 KB

 · Views: 737



IrfanView_instal400.rar

 1.1 MB

 · Views: 919



W3IR Tutorial and Icon Rules in THW.rar

 34.7 KB

 · Views: 621




 Last edited: Oct 5, 2007

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Allright, I won't bother, but I'll ask wolverabid or someone to (since they can Hard Delete, removing the 'this message was deleted by...' thing)

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


Hello Flame_Phoenix,

Please re-read our rules, especially this one:


 Double Posting / Double Threading said:



 Please do not double post in threads or make two of the same thread. We have an edit button for a reason, if you accidently double posted/threaded, please delete it before we do.


Click to expand...

Next time you double post, you will receive an infraction.

Ralle

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Overall, the tutorial is outstanding Flame_Phoenix! I have been watching your progress all along, although this is my first post.


Much of the information is covered elsewhere on the site in other tutorials, but yours ties it all together in one complete (if somewhat unwieldy) package. I need to go over it again and (with your permission of course) correct some minor errors of context, grammar and spelling. We also need to hear from BlackDoom about those sections that apply to W3IR.

The tutorial is certain to be approved after some further development.

 Another Hive Workshop tutorial you could mention in your “Websites” section is:

How to Make an Icon

The title has to be shortened, that's for sure: I will do so when I find time to go over it again (problably this week).


No problem Flame, before I do any editing I will back up the post(s) into a text file and attach the backups to one of mine.


Please give me at least a few days as I have a lot of admin projects I'm working on. I think it would also be helpful for you to send a PM to BlackDoom and ask his advice/opinion on the tutorial: he is one of the "powers that be/ultimate authorities" when it comes to icon creating.


 Last edited: Sep 10, 2007

#### BlackDoom

#### BlackDoom

 Level 9

JoinedSep 2, 2006Messages264

Resources: 17

#### Resources
Icons 16Tools 1

Hello, Flame_Phoenix!

Thanks for the message and let me say that I hope I will be somewhat helpful.

1. "i am making the ultimate tutorial about icons" - suggestion: try not to use extreme words, as when anything changes in the software you have used in the tutorial, what you wrote may not apply completely.

2. "ultimate master icon making" - well, let me say I feel flattered, but I consider myself as just another icon maker, with some skill in design. Nothing else. There are other artists out there - THW, WC3C - that deserve that rank more than I do.  Thanks, nevertheless!

3. "W3IR: 1. - This program cannot create a DISBTN file for the Autocast icon. This means that you have to create it. Read my tutorial as it teaches how overrun this bug." - I dare to say the contrary. If you'd analyze better the way Blizzard named the icons in the original MPQs, you'd notice that they use BTN for the autocast icons and DISBTN for the disabled autocast icons. For instance - "BTNWebOff" from RoC or "BTNSearingArrowsOn" have their corresponding "DISBTNWebOff" and "DISBTNSearingArrows" files. So, no, this is not a bug. THW chose to implement the "btn, disbtn, atc, disatc, pas, dispas, upg, att, ssc" convention. I am not totaly against it, nor totally for it. But I personally choose the way Blizzard implemented things - you can use the DISBTN icon for both the BTN icon and the Autocast icon.

 If you analyze the DISBTN and DISPASBTN icons from the original MPQs, you will notice that they are not identical. This is why I do not think the DISBTN should be used for the DISPASBTN icon as well.

 A way easier method of obtaining your DISBTNIcon_autocast icon is to compy the DISBTN icon and rename it. Simple, huh?

4. About the seconds supposed bug, what I can say is that I did not implement a completely safe set of output routines. Sometimes, it may not work as one would like. I only considered the scenarios I was able to think of. You can't blame a programmer for doing so.

5. I recommend that you to remove the attachments containing the kits of various software. First, because W3V will not be updated any time soon, so the "official" page is good to go. Seconds, because IrfanView kept a very nice compatibility with the previous version Irfan published. And third, because I have already presented an URL for direct download, not dependent to THW.

However, I do agre that this way, you provide the same version of software to the users as you have used in the tutorial itself. Nice job!

6. The next version of W3IR to come will include some nice features (as 32bit border images you can customize, easier bordering and naming module, the same engine for renaming icons but also an engine based on wild-cards, IrfanView-independent resampling from 64x64 to 48x48 etc.).

 Thing is, the way I want the next version of W3IR to look will rend the section you granted it in your tutorial unusable due to different GUI and features included. For the time being, it's okay as it is. But in a few months, W3IR will upgrade. Radically.

7. And I suggest enforcing the use of BMP images as they usually are not compressed. No compression means full quality. 100% what you meant to design. Compare this to a JPEG.

8. About the "Add Borders with W3IR for active game play icons" section... Mate, you really messed that one up, didn't you? From what I saw, you completely skipped the tutorial I wrote for icon suffixes. I will not get into it, as I have did so enough already - in the ReadMe.htm file issued with W3IR, in the posts both on THW and on WC3C. However, next version of W3IR will compose the names of the output files based on this convention: IconFormatPrefix + IconProjectName + OptionalSuffix (e.g.: BTN + Shrek + _autocast). You won't have to specify the suffix strings anymore - you'll only have to do so in the old-fashioned renaming module.

9. Furthermore, I strongly suggest you to use some identifiers for the steps to take. Why? You could use "Go through the 1-5 steps in the "Add Borders with W3IR for active game play icons", then proceed with these steps, in "Add Borders with W3IR for passive game play icons"". Saves some space and is clearer to read.

10. As I wrote in the ReadMe document of the latest W3IR version, you can practically create all the required formats USING THE PASBTN ICON. Try it. You'll like it. So, you take a BTN icon, get a PASBTN icon from it, then run W3IR on it as if it were a normal design. Easier, don't you think?

11. Specify a zoom level that is multiple of 100. E.g. 200, 400, 500... This way, no flawed images will appear - trust me, they do.

12. IrfanView is "limited" as it is first of all an image viewer. Better say it has basic image editing features. It is a really nice program to have - I use it all the time and I find it excellent. If you don't believe me, ask Irfan Skiljan how many times he had e-mails from me, from feedback to bug reports.

13. "You may want to be careful when selecting your “OUTtput” folder in the “OUTput window”." - OUTtput?

14. The Blizzard naming convention is this: IconType + IconCustomName not  IconType + IconCustomName + .FileFormat. The .blp part of the filename is required, otherwise W3 won't see your icons. At least, this is what I know about icons. I never tested to see if a .tga icon is accepted by W3 - my guess, it's not.

15. 64x64 is the standard icon size. Including the Upgrade icons. I used the 48x48 name to specify that this is the size of the region an user may customize in such an icon. That's all. I could have as well called the 64x64 icons 56x56 - due to the same reason. Please, don't lead people into thinking that they need to have a final BLP file of 48x48 pixels, as it will not work correctly. From what I know.

16. ".Keep in mind that he previous rules also applies here." - " . Keep" and "rules also apply".

17. You should add the ReadMe file of W3IR to the sources you used in the research process. And, in the "Bibliography" list as well...

18. The most notable issue, however, is that you said you use IrfanView as an image program. No problem, but without any of the other graphical design software you specified, one cannot design an icon. Copy&Paste icons are NOT allowed on THW as valid icons. Nor are the recolored icons (as those obtained using the RGB Scramble of W3IR or the Swap Colors of IrfanView). Better add the Icon Rules section of THW to your Bibliography as well.

19. One thing I can tell - most of the icons, although 64x64 resources, are resampled in-game. To 48x48 pixels. So, all icons are 64x64 by standard. Okay?

20. I do not suggest recommending people to obtain 64x64 images from 48x48 images. That, because the 64x64 images will have a slight blur. When you have no other option, in order to obtain the 64x64 image, yes, one may have to come to this. Otherwise, do not recommend it. The Icon Rules also state this issue.

21. "BTN_attack(or _upgrade).blp[/INDENT" - add a "]".

22. "There is millions of ways to make an icon to appear green in warctaft, " - there "are" millions... and the game is called Warcraft.

Overall, your tutorial is really big! And I honestly wish I would have had the chance of finding such a material prior to my first icon, as it would have spared me from some hellish headaches obtained while trying to understand and put together the signification of several icon-related aspects in W3. Furthermore, it is really through and easy to follow.

I hope you do not find my above-specified points as offending, as I did not write them in such purpose. They are just the things I thought you should consider before publishing it as final.

I did not read the tutorial 100% top-bottom. I was mostly interested by the sections you wrote about using W3IR. I did look over the rest of it, but not as closely as on the W3IR-related sections.

Good luck with your work!

Also - rep added.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

HOLY MACAROLI !!! jesus, now that is a big reply. It is ok as i don't find your information offensive in any way.

I just need some more time to re-read all your post again and ask you some questions as well.

I hope i didn't messed up with the W3IR to much because it was the first time i was using, and yes i DID read the help-me file (the tutorial you guys include with the program, that is what you are talking about rit ?).

But as i said, i will re-read your thread again to find more information.

Btw, thx for the rep points =)

Also please, if you find any more errors in my tutorial, please indicate in which sections they are, because it is a little difficult to find 1 line errors in a work with more than 70 pages and way more than 100 images.

PS: Can anyone please remove Ralle's post from my thread ???

Btw, thx for removing your posts PurplePoot, low kwaliti and Wolverabid.

UPDATE


 blackdoom said:



 "i am making the ultimate tutorial about icons" - suggestion: try not to use extreme words, as when anything changes in the software you have used in the tutorial, what you wrote may not apply completely.


Click to expand...

Well, wermmm you do have a point... still i never saw a tutorial like mine and i think it is fair for it to have an extreme name ... besides if i keep updating the tutorial it extreme title will always apply to it ...

the problem is that i will soon run out of time and so i just know i can't keep track of the upgrades of all programs i use ... Therefore i must accept your critic and change the title .... again....

I really don't know about a really good title to describe my tutorial so ... i am opened to new ideas .. if anyone has a suggestion, please give it ...


 blackdoom said:



 "ultimate master icon making" - well, let me say I feel flattered


Click to expand...

Well, Wolverabid recomended you and when i saw you curriculum vitae on THW i was very surprised ... you actually made part of a program i use ... Well, if you say there are other people better than you out there then name them because i don't know anyone better than you and i am sure that THW would really make good use of people with such talents =)


 blackdoom said:



 3. "W3IR: 1. - This program cannot create a DISBTN file for the Autocast icon. This means that you have to create it. Read my tutorial as it teaches how overrun this bug." - I dare to say the contrary. If you'd analyze better the way Blizzard named the icons in the original MPQs, you'd notice that they use BTN for the autocast icons and DISBTN for the disabled autocast icons. For instance - "BTNWebOff" from RoC or "BTNSearingArrowsOn" have their corresponding "DISBTNWebOff" and "DISBTNSearingArrows" files. So, no, this is not a bug. THW chose to implement the "btn, disbtn, atc, disatc, pas, dispas, upg, att, ssc" convention. I am not totaly against it, nor totally for it. But I personally choose the way Blizzard implemented things - you can use the DISBTN icon for both the BTN icon and the Autocast icon.

If you analyze the DISBTN and DISPASBTN icons from the original MPQs, you will notice that they are not identical. This is why I do not think the DISBTN should be used for the DISPASBTN icon as well.

A way easier method of obtaining your DISBTNIcon_autocast icon is to compy the DISBTN icon and rename it. Simple, huh?


Click to expand...

well, i already know all that stuuf and i explain it on my tut. Wermm how shall i start ...

Well, first i know that the Autocast Icon is a regular BTN icon but with a different border. However there is a problem, please attempt on the following images:

As you can see the first image shows the autocast ability "heal" when the game normally runs. The second image is when it is paused.

In the second image you can see that the "bright" is still running in the border, but where the hell is the border ??? Yes there is no border, still the bright is there ... Personally i think this is a bug and i don't find this system correct by two reasons:

1 - If all the other icons do have a Single DIS correspondent, why can't this one have, if it has a different border as well ??

2 - Think on the Innerfire example. Before upgrading to reach it how do you know that ability is an autocast ability ??? That's rit, you don't know if that ability is or is not an autocast ability, thus can and will miss guide you during the game.

I already was a great warcraft player and i try to apply as much game knowledge as i have to my tutorial. I perfectly remember that when i started playing warcraft i didn't know which icons and which abilities could be autocasted ... just found that later with lot's of trouble ... In my opinion this is one of those bugs that blizzard never had the chance to correct, like the incomplete races of the naga and draenai (which were never completed due the lack of time).

On the other hand i refuse to use the DISBTN icon for auras as the aura icon is far more brilliant than the DISBTN correspondent icon.

In this tutorial i teach what i think it is correct for people to learn, still i give them new paths to try and i encourage them to do so by giving them alternative websites they can visit and by incentive them to use other programs.


 blackdoom said:



 You can't blame a programmer for doing so.


Click to expand...

Well i cheers, the program you did was really impressive. Besides no1 is blaming you and no1 is perfect ... If not by your program i would have never done this tutorial =)

Congratulations m8 =)

EDIT:


 BlackDoom said:



 I recommend that you to remove the attachments containing the kits of various software. First, because W3V will not be updated any time soon, so the "official" page is good to go. Seconds, because IrfanView kept a very nice compatibility with the previous version Irfan published. And third, because I have already presented an URL for direct download, not dependent to THW.

However, I do agre that this way, you provide the same version of software to the users as you have used in the tutorial itself. Nice job!


Click to expand...

Well, i will not remove the software packages.

I also though about the upgrade issues, and so i decided to add the official links of the programs so people can actually download them. This way i give to the readers to options: they can download the packages i use from this tutorial, or if them prefer they can use the websites i provided them and make the download from the official page. =)


 blackdoom said:



 Thing is, the way I want the next version of W3IR to look will rend the section you granted it in your tutorial unusable due to different GUI and features included. For the time being, it's okay as it is. But in a few months, W3IR will upgrade. Radically.


Click to expand...

Well, that will never happen because i provide the readers with the version i use =). They can just download that version from here. However an update to my tutorial is not a question i ignore completely, it depends on the changes on the program, if it is way better, faster and more easy/simple to use, than i will update my tutorial for sure.

You should also check the Program issues section i include in my tutorial, as it may give you valuable information about what you should fix in W3IR.


 blackdoom said:



 And I suggest enforcing the use of BMP images as they usually are not compressed. No compression means full quality. 100% what you meant to design. Compare this to a JPEG.


Click to expand...

Agree 100% with you. I my tutorial i tell people about the vantages of BMP and i strongly recommend the use of BMP files instead of TGA files and JPEG.

However if you are talking about the more than 100 images i attached to my tutorial, well, i can't save them in BMP because i use a very old version of Paint Shop Pro (8) which only allows me to save them in JPEG format. Still because PSP 8 is way more advanced than IrvanView i use it ignoring it's obvious limitations ... until i get a better program or a new version ofc =)


 blackdoom said:



 About the "Add Borders with W3IR for active game play icons" section... Mate, you really messed that one up, didn't you? From what I saw, you completely skipped the tutorial I wrote for icon suffixes. I will not get into it, as I have did so enough already - in the ReadMe.htm file issued with W3IR, in the posts both on THW and on WC3C. However, next version of W3IR will compose the names of the output files based on this convention: IconFormatPrefix + IconProjectName + OptionalSuffix (e.g.: BTN + Shrek + _autocast). You won't have to specify the suffix strings anymore - you'll only have to do so in the old-fashioned renaming module.


Click to expand...

Well i did NOT messed it up. As i already wrote before, before i decided to write this tutorial did read the tutorial about W3IR, and if i did something worng it is because i didn't understand the tutorial. I must confess in a NON-offensive way that the tutorial was confusing and hard to read... at least for me ... The experience i gained was by experimenting the program.

About the Active game play icons and the passive gameplay icons, please read my tutorial to understand this division. this division was made using my Warcraft game play experience. As i already told you guys, in ages past i was one of the best warcraft players in all europe (25).So every tutorial i make i want it to apply to the game as much as possible. In my past glorious days i used this system to classify the information. Passive gameplay icons are in way important to a players real strategy, but the one a players uses are the active game play ones. I did not messed up this part of the tutorial and i think it is a very effective way to distribute them in two classes, thus making my job a lot easier.

Anyway, if you think that the renaming section is completely wrong, please let me know, as long as you find an easier alternative.


 Blackdoom said:



 Furthermore, I strongly suggest you to use some identifiers for the steps to take. Why? You could use "Go through the 1-5 steps in the "Add Borders with W3IR for active game play icons", then proceed with these steps, in "Add Borders with W3IR for passive game play icons"". Saves some space and is clearer to read.


Click to expand...

MM that would require me to do extra small indexes in each section ... a lot of work.. still i think that is a great idea ... but werrmmm... the tutorial is already divided ... and ....wermmm... i am not sure about what to do and how to do it =S

Please make this constructive critic a little more clear by using an example or something like that as i am confused =S


 blackdoom said:



 10. As I wrote in the ReadMe document of the latest W3IR version, you can practically create all the required formats USING THE PASBTN ICON. Try it. You'll like it. So, you take a BTN icon, get a PASBTN icon from it, then run W3IR on it as if it were a normal design. Easier, don't you think?


Click to expand...

Well, i know about that .... but thing is that most of the icons THW and W3C have are not PASBTN. So i decided to use the BTN icon as a main reference because it is the most common format. Besides if some1 posts a PASBTN icon they normally post the BTN, DISBTN and PASBTN icon among with it.


 blackdoom said:



 Specify a zoom level that is multiple of 100. E.g. 200, 400, 500... This way, no flawed images will appear - trust me, they do.


Click to expand...

Yes i did experienced that bug, but i didn't know it was caused due to the zoom. Well, i will change the tutorial, but the images =S ... Well i really don't know ... One thing is for sure, the text will be changed and i will add that bug to the known program issues.


 blackdoom said:



 IrfanView is "limited" as it is first of all an image viewer. Better say it has basic image editing features. It is a really nice program to have - I use it all the time and I find it excellent. If you don't believe me, ask Irfan Skiljan how many times he had e-mails from me, from feedback to bug reports.


Click to expand...

Completely agree with you. =)


 blackdoom said:



 13. "You may want to be careful when selecting your “OUTtput” folder in the “OUTput window”." - OUTtput?


Click to expand...

Please tell the section where this error is, because it is difficult for me to find an error in a single word or line in a work with more than 70 pages and more than 100 images.

Still, it only demonstrates how closely you were reading my tutorial... Thx =)


 blackdoom said:



 The Blizzard naming convention is this: IconType + IconCustomName not IconType + IconCustomName + .FileFormat. The .blp part of the filename is required, otherwise W3 won't see your icons. At least, this is what I know about icons. I never tested to see if a .tga icon is accepted by W3 - my guess, it's not.


Click to expand...

Well, the Blizzard naming icon convention actually IS IconType + IconCustomName + .FileFormat , by example =BTN_MyIcon.blp (BTN = IconType; _MyIcon = IconName;blp = FileFormat) And yes the blp part of the icon is required by warcraft 3. But warcraft 3 also accepts icons i tga format, still the main difference is that tga icon do not save alpha channels and so they are more likely to cause crashes and errors, like green and black icons. With blp icons, you DON'T have those problems, thus making me recommend blp icons to all =).


 blackdoom said:



 15. 64x64 is the standard icon size. Including the Upgrade icons. I used the 48x48 name to specify that this is the size of the region an user may customize in such an icon. That's all. I could have as well called the 64x64 icons 56x56 - due to the same reason. Please, don't lead people into thinking that they need to have a final BLP file of 48x48 pixels, as it will not work correctly. From what I know.


Click to expand...

Well, passive game play icon are 48x48 pixels size. However with the border they become 64x64. I understand what you mean i think think that if people read my tutorial from top to bottom they will not be miss guided and their icons will have 100% chances to work, as i reviewed the tutorial step by step before posting it, and all my icons worked fined. However i may add some explication if you want, to make sure no1 is miss guided.... Still no1 will be as long as they obey to the tutorial step by step and as long as people do everyting exactly as i say =)

EDIT EDIT


 blackdoom said:



 Keep in mind that he previous rules also applies here." - " . Keep" and "rules also apply".

Click to expand...


 flame_phoenix said:



 Please tell the section where this error is, because it is difficult for me to find an error in a single word or line in a work with more than 70 pages and more than 100 images.


Click to expand...

jesus, you really read my word =) thx =)


 blackdoom said:



 You should add the ReadMe file of W3IR to the sources you used in the research process. And, in the "Bibliography" list as well...


Click to expand...

Actually it is a great idea .. i don't know why i didn't think about it earlier ... Consider it done =)


 blackdoom said:



 The most notable issue, however, is that you said you use IrfanView as an image program. No problem, but without any of the other graphical design software you specified, one cannot design an icon. Copy&Paste icons are NOT allowed on THW as valid icons. Nor are the recolored icons (as those obtained using the RGB Scramble of W3IR or the Swap Colors of IrfanView). Better add the Icon Rules section of THW to your Bibliography as well.

Click to expand...

Well, in the first section of my job i do explain why i used an Wallpaper as an icon. I admit i am not good at making pictures with a mouse (or something else) and so i explained my problem and my solution.

Still to correct that fact i decided to add a link to a tutorial that teaches players how to create the design of an icon, so i believe that is no longer a problem.

Anyway i think that the creation of the picture depends on each person and so i believe that teaching my own methods would be wrong, i think everyone should be able to find their own path.

However i must admit that issue ... If you find a free program which can be used to make images (Well, GIMP may do the job, but i also see it as a very limited program and i don't know how it works), will you make a section explaining to people how to create a icon with that program ???

You are not forced to accept this task, and neither will i be sad if you refuse it, it is just your choice and it will be fine for me if you say no.

Also i think it would be a good idea to add the iconing rules of THW to my tutorial and i will do it as soon as i find them .


 blackdoom said:



 One thing I can tell - most of the icons, although 64x64 resources, are resampled in-game to 48x48 pixels. So, all icons are 64x64 by standard. Okay?


Click to expand...

Werrmmmm, hã !? i don't understand what this means =S


 blackdoom said:



 I do not suggest recommending people to obtain 64x64 images from 48x48 images. That, because the 64x64 images will have a slight blur. When you have no other option, in order to obtain the 64x64 image, yes, one may have to come to this. Otherwise, do not recommend it. The Icon Rules also state this issue.


Click to expand...

I agree, but to give that information to people is always good =). We may never know if that knowledge will be needed by us some day in the future =).


 blackdoom said:



 "BTN_attack(or _upgrade).blp[/INDENT" - add a "]".

and

"There is millions of ways to make an icon to appear green in warctaft, " - there "are" millions... and the game is called Warcraft.

Click to expand...


 flame_phoenix said:



 Please tell the section where this error is, because it is difficult for me to find an error in a single word or line in a work with more than 70 pages and more than 100 images.


Click to expand...

What the **** you have the eyes of an hwak .. i re-read my work at least 2 times and i din't find a single mistake ... now this is the 3rd you find ... how do you do it !?


 blackdoom said:



 Overall, your tutorial is really big! And I honestly wish I would have had the chance of finding such a material prior to my first icon, as it would have spared me from some hellish headaches obtained while trying to understand and put together the signification of several icon-related aspects in W3. Furthermore, it is really through and easy to follow.

I hope you do not find my above-specified points as offending, as I did not write them in such purpose. They are just the things I thought you should consider before publishing it as final.

I did not read the tutorial 100% top-bottom. I was mostly interested by the sections you wrote about using W3IR. I did look over the rest of it, but not as closely as on the W3IR-related sections.


Click to expand...

well thx by you friendly reply. I will add a special thanks section and your name will certainly be included there, you helped me a lot with your comments and suggestions.

I also hope i didn't offend you in any of my comments, because that is not i want, please report me if you find any of my comment unpleasant, so i can apologise and remake it into a civilized form.

Now about the modifications i will do them as soon as i can, still it may take a while to find all that data you suggested. Anyway, good works do take a lot of trouble =P

Thx for you reply (again) and i hope i see more of your posts soon =)

Now to end i want to dedicate a smillie to you:

Say hello to the happy pirate Lord =) arrghh thx for your help lad argghhh, may the seas of future be kind on you argghhh =)

PS READ: cooooommmmmmmmmeeeennnttss plz !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


 Last edited: Sep 11, 2007

#### BlackDoom

#### BlackDoom

 Level 9

JoinedSep 2, 2006Messages264

Resources: 17

#### Resources
Icons 16Tools 1

As far as I know, the animation (the sparks over the normal border) are done by W3 in-game. I did not see any icon from the original MPQ archives having that border. If you know any - and I mean, any of the original icons - please let me know. That would be news for me!

From what I remember from the original campaigns, you did not know if the ability is or not autocast-able. That's what I know. This is why I think the DISBTN icon applies perfectly to 'em (BTN, Autocast...).

About the Naga and Draenai races. Are you sure they had not the time to complete them or these races are not part of the Warcraft lore? I do not posses to much info about this subject - on WC3C there is a thread on W3 Lore, perhaps there one could learn more / ask about this issue.

I do not consider it a bug. Icons - as we call them - are simply buttons. That's it. And a button - as far as a programmer is concerned - has 2 main states: active (normal, enabled) and inactive (disabled). Off course, there are the pressed, focused and hovered states, but as far as the W3 in-game GUI goes, active and disabled are enough.

W3IR is developed only by me. Except the feedback - which I did acknowledge in the ReadMe file - everything is written by me. The errors are also mine  - some invalid URLs, as there was the change from the old site structure to the new one implemented by Ralle and between those 2 moments I was not completely able to keep track of all the URLs I've inserted in my document.

As about the location of the errors I've highlighted - you can search then in the first post you put here. That's where I got them from, no special text formatting, so searching them in your web-browser should do.

This is my last post here - to limit the amount of damage I bring your thread.

I'll delete both my posts when you decide - for now, I'd like this answer to be available for those who have your thread subscribed. A couple of days, then I'll delete them, okay?

Cheers, mate

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3


 blackdoom said:



 As far as I know, the animation (the sparks over the normal border) are done by W3 in-game. I did not see any icon from the original MPQ archives having that border.


Click to expand...

Yes it is an in-game animation designed to let the player know that the ability is activated.


 blackdoom said:



 From what I remember from the original campaigns, you did not know if the ability is or not autocast-able.


Click to expand...

Exactly, that is why i think the DISBTN icon should not be applied to an Autocast icon. That way you will never know if the ability is autocasteble and therefore you will not be able to make a strategy for it before you reserach unit you get it. That is wahy i decided to create an autocast DISBTN icon.

About the races:


 MCV said:



 The Naga are not a full race. If you compared them to my tutorial they are quite different from the layout used by all other races.

The Naga have too few units, too few buildings, and too few heroes. I suspect they at one point were planned as a fifth race, but Blizzard cancelled it either because:

a) It would shift the release date backwards

-OR-

b) It became too much work to ballance

This would not supprise me, it has happened before (C&C Tiberian Sun: Firestorm supposedly would have a third side, CABAL, for example, and you were supposed to be able to play as Corrino in Dune 2000.)


Click to expand...

and i agree 100% with this tutorial creator.


 blackdoom said:



 W3IR is developed only by me. Except the feedback - which I did acknowledge in the ReadMe file - everything is written by me. The errors are also mine

Click to expand...

And it is a wonderful program you have created, but no1 is perfect. If you need any kind of help, in the future to make your program, please contact me as i want to help as best as i can.


 blackdoom said:



 This is my last post here - to limit the amount of damage I bring your thread.

I'll delete both my posts when you decide - for now, I'd like this answer to be available for those who have your thread subscribed. A couple of days, then I'll delete them, okay?


Click to expand...

Your posts and comments do NOT damage thread in any way. Your posts are very welcome because it is always good to have a decent constructive critic to my work by some1 who FINALLY understands what is an icon ... lol =)

Therefore for those who do have some knowledge about icons, the fact that you actually posted in my thread is just one more reason for them to see it, because they know you as an iconing reference.

I don't want you to remove your threads... at least not for now, because i am using them to try to improve my tutorial as best as i can.

Thx by all the help =) If you need my help in your program just let me know. I personally think that the actual interface of the program could be improved. But that it's just my personal opinion, as i already told you, without your program, i would have never started creating this tutorial. Your program is as well an icon reference to all who make icons, as it enables people to the several types of icons by using only 1 image. And i actually searched for icon programs ... to let you know, the reason why i used yours, it is because it was the best bordering program i found =D

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

List of changes:

1 - To start i will add the W3IR tutorial link to my work as well as THW icon rules

2 - After that i will re-read the tutorial again and try to correct any errors i may find.

3 - Then i will change some images about IrvanView and recommend people to zoom them between multiple numbers of 100. I will also add that bug to the program issues.

I may also add some steps to the W3IR sections but i am not sure yet, so in this part i won't promise a thing because doing so would require me to evaluate and probably change the entire structure of the tutorial itself.

These are the changes i will son make on my tutorial, i also also keep updating this post so people can see the updates.

EXTRA: I also changed the title. I believe the "super" word is not as strong as the "ultimate" word according to the dictionary ( i think)

EDIT: All done, all changes made. However i was unable to find some typing errors in point two because i don't know where they are .... (they must be hiding from me because i am evil =p)

I think the tutorial may be ready now, i will add a special thx box and i guess it is everything.

I would still like to receive some suggestions if you guys have some. Also keep posting


 Last edited: Sep 12, 2007

#### 3ICE

#### 3ICE

 Level 5

JoinedJul 4, 2007Messages84

Typofix

+rep (I didn't read it sorry but im sure it deserves a rep)


Get Google ToolBar and you will never misspell anything!


Introducrion - Introduction


colours - colors


viwer - viewer


colour - color


shrek - Shrek


fiona - Fiona


everyting - everything


Infraview - IrfanView


APLLY - APPLY


defence - defense


borded - bordered


iconing master - icon making master (?)


hiveworkshop - HiveWorkshop (or THW)


BEATIFULL - BEAUTIFUL


Increae - Increase


armours - armors


armour - armor


BTn - BTN


warctaft - WarCraft


WarcCraft - WarCraft


warcrraft - WarCraft


Tft - TFT


PhtoShop - PhotoShop


goggle - Google


(Yes i know the British English uses armour and colour, but this site is American afaik) [as far as I know]


edit:

 im so kind I rewrote your whole tut


If you trust me simply replace your post's code with mines.


(and you won't have anymore typos) [btw i corrected OVER 200]


edit:


Well after seeing how not nice the thing looks like I have attached a TXT document that contains the exact code you need to replace your post with.

"The SUPER Icons Tutorial with ALL about icons" Flame_Phoenix


[broke="Remember to POST comments; Give reputation POINTS and to RATE this tutorial."]IMPORTANT !![/broke]


After the unexpected success my first tutorial had Creating a Tavern by Flame_Phoenix i was astonished. I never expected my tutorial to have an excellent qualification by the users. In deed i remember trying to go to every detail and that seemed to work for me. Now, to thank that opportunity i decided to write a new tutorial. All about icons. I guaranty that when you finish reading my tutorial you will be a professional making icons - if you are new at icon making, don't worry, this tutorial it's just for you. It was made by purpose to people who never managed themselves into the world of icon making, but non less this tutorial will also give to advanced icon makers all the experience they need, as it also contains advanced icon knowledge. This super tutorial DOES HAVE EVERYTHING ABOUT ICONS. YOU WON'T FIND A BETTER TUTORIAL. To this tutorial i searched for all icon websites, and this is a work with all that information. As a work it is, it also has a Bibliography section you can visit and a few sites i recommend myself to all icon makers. Now to maybe end this note I am not going to say that I don't want this to go to the grave, because I know this work is to much good for that. To nearly end i just want you to comment my 3,5 months work and give suggestions. If you don't understand something, post it on the forum so than I can help you and make a better tutorial.


To finally end i want you to know that this is also my first and probably my last super tutorial, as the chances of you guys seeing me doing more tutorials will be small. Doing this tutorial gave the chance to be with the best people among icon making like BlackDoom and gave me the chance to learn tons of new stuff while making it. Still I must confess myself very disappointed with the moderators of the "Submit a Tutorial" section , as I only got the feedback I needed in the very final stage of my work (Special thanks to Purplepoot, Wolverabid and low_kwaliti). Making this tutorial gave me the most difficult times I have ever had in the HiveWorkshop, but also the hope that it can give me the best moments I will ever have as well. So to finish I just want to say to ALL people who read super tutorials, to COMMENT them, even if you are the newest guys in the world to the subject, keep in mind that your WORDS, REPUTATION POINTS and RATING are the best help a tutorial creator can ever have. Even if you don't understand the subject in cause, I guarantee you that the simple word of Thanks is a very important word for all those who make tutorials. Psychologically it does have a GREAT IMPACT, so use it to do good.


And now before you start reading my tutorial I would like you to close this revealed window, as it does take needed space from the tutorial, thus damaging it's contend.

“General Introduction”


Hi, this tutorial will teach you people everything you will ever need to know about icons. This is a massive tutorial that gets together all icon information for WarCraft 3 TFT. It also applies to ROC users, but you will need an extra program to import the icon.


The programs I will use are:

IrfanView

W3IR

WarCraft 3 Viewer

Word Editor


I will use IrfanView as an image program. However you may choose to use other programs such as Photoshop, Paint Shop Pro, Paint Shop Pro, PhotoImpact, GIMP or others. In the end of the tutorial you will have the links to download this programs if you want.


In this tutorial you will learn how to:


• Create an Icon:

o Introduction


o Start with IrfanView


o Add Borders with W3IR and IrfanView for active game play icons


o Add Borders with W3IR for passive game play icons


o Converting to BLP with WarCraft 3 Viewer​
• Edit an Icon:

o Introduction


o Incomplete packages of images


o Transforming a 64x64 pixels image into a 48x48 pixels image


o Modifying an icon​

> Modifying with IrfanView


> Modifying with W3IR​

• Import an Icon

o Introduction


o Importing and using 64x64 icons


o Importing and using 48x48 icons​
• Fix an Icon

o Introduction


o Fixing green icons


o Fixing black icons​
• List of important known bugs in the programs


• Bibliography


• Websites and downloads


• Credits


• Special Thanks


This tutorial also has a list of other Internet sites that can help you if you need.

 “Creating an Icon”

 “Introduction”


Before I start I want you to know that I will not teach you how to create an image for an icon; I will just teach how to create the icon with the image. If you want to create a good image for an icon you can search in Google, or, the most common solution, you can use a professional image program like those I mentioned in the start. In the end of this tutorial you will have the websites of the programs I mentioned, it is up to you to choose the best.


This means that to start we must know what an icon is. So, what is an icon?? Well, this is not a simple answer. There are two types of icons: icons which we use in active game play (BTN, DISBTN, PAS and DISPAS, I will explain what this is later) and icons that we use in passive game play (Type of attack, armor, level of upgrade, etc).


So we have:

1.Active game play icons – these icons are 64x64 pixels icons with 24 bit colors. We use these icons when we cast a spell or give an order.

​
View attachment 10293

2.Passive game play icons – these icons are 48x48 pixels and 24 bit, used by the game to determinate the level of an upgrade, they are referenced by the game as “special icons”.​

View attachment 10294


In this tutorial you will learn how to create both of these icons.


Both icons can be in TGA format or BLP format. Icons in TGA format do not save alpha channels which cause some icons to appear green or even black in the game, BLP format does save alpha channels and it is the format I recommend and use.

View attachment 10295


Green icon … sometimes there are also black icons. This happens when you do not do things correctly. It can be caused due the TGA format.


Each icon graphic for a unit / building consists of two basic icons, the main icon you see when normally playing the game (BTN - Button), and the darkened icon that you see when you hit F10 to display the menu (DISBTN – Disabled Button).


Other icons, such as the passive abilities (like endurance aura, Unholy aura, etc) use a different type of icon, an icon with no borders (PASBTN – Passive button) and the darkened version (DISPASBTN – Disabled Passive Button). PASBTN and DISPASBTN icons are not necessary for an icon to work properly, they are optional. Passive game play icons, are an exception as they can not have Passive Icons and Disabled Icons.


In this tutorial I will uses the programs:

• IrfanView ( You can use other programs that can save in BMP format and that can work with images, like GIMP, Photoshop, Paint Shop Pro, PhotoImpact, etc). IrfanView is available in http://www.IrfanView.com/, or you can download the version I include on this tutorial.


• W3IR (available in this website, or you can download the version I include with this tutorial)


• WarCraft 3 viewer (available in this website, or you can download the version I include with this tutorial)


• WarCraft World Editor (to import the icons)​


Now I will explain what I will do during this section:

1. I will start by using IrfanView to "create an image". After that I will resize the image set it to 24 bit and save it in BMP format.


2. After that I will use W3IR to create the borders for my icon.


3. In the end I will use WarCraft 3 viewer to convert my image into BLP format.


4. It is done!, now I just need to import it.

​

 “Start with IrfanView”


To start install IrfanView. Now because IrfanView is a very limited program (and that is why my advice is to use better programs, like the others I mentioned), it can not create a new image. To solve this problem, I used a very simple and easy solution, Print Screen your Desktop! Or search for a BIG image in Google, I decided to search for a big image in Google. Now that the most difficult part is done (lol) save the image in a paste. Now use IrfanView and open the image with it. If you want to learn how to create and design your own icon, then visit the Websites and Downloads section and have a look on the websites I recommend.

View attachment 10296


MMM Shrek and Fiona are so nice together! Now it is time to resize our image which is very easy to do. Go to Image > and then use Resize/Resample, or if you are lazy you can just press CTRL + R.

View attachment 10297

View attachment 10298


Now Uncheck the “Preserve ratio aspect” box, and choose size 64x64 for an active game play icon, or 48x48 for a passive game play icon. I will make an active game play icon, so I will use 64x64 pixels size.

View attachment 10299


OMG, MM it seems that Merlin did a great job, Shrek and Fiona are much smaller now =).


With our image resized it is time to set it to 24 bit colors. But, how do I know the color depth my image has? Well, to know the current Bit colors of your image just go to Image > Information or press “I”.

View attachment 10300


IrfanView tells me that the current Color depth of my image (number of color bits it has) it 24 BitsPerPixel. This is good news because now I don’t need to change anything.


But what if my image has too many color bits?


Well IrfanView does have a solution for that. To decrease the color depth of your image you just need to go to Image > decrease Color depth ( in this case, but if your image needs more colors, go to Increase Color depth) and set the color depth of the image to 24BPP.


Now to end this part of the tutorial, just save you image in BMP format. This is very easy to do. Go to File > Save as and there give a name to the image, and save it as a BMP image in a place you know where it is. I am saving the image in BMP format because the program I will use next works better with BMP. Also WarCraft 3 Viewer and WarCraft World Editor do have a few errors and bugs when they handle TGA images. This means that I will save the images in BMP and in the end I will convert them into BLP.

View attachment 10375

View attachment 10302


I saved my image in my desktop as you can see.


Now close IrfanView, we will not need it for now. Time to move to the next section.


“Add Borders with W3IR for active game play icons”


W3IR is a very complex program and I warn you that this will be the most difficult part of the tutorial. This program allows you to do many things, but for now, I will just add the BTN, DISBTN, PASBTN, DISPASBTN and the AUTOCAST borders to my Shrek icon.


If you remember active game play icons are 64x64 icons, and so we will work out that format.


To start create 2 folders. One folder is where you have to place the image you just created now (in this case name it “In”), and the other folder is the folder where the program will place the Bordered images it will create (in this case name it “Out”).

View attachment 10360


Now place your recently created image in the “In” folder. After doing this open the W3IR program.

View attachment 10303

View attachment 10304


Good, now press the button Add Borders.

View attachment 10305


If you done this correctly, this new window will pop up:

View attachment 10306


Here is where the challenge begins.


If you look to the image you can see that there are 2 windows. In the left one, you must choose the “input” folder; this is the folder where you have your created image. In the right window, select the “output” folder, the output folder is the folder where the program will place the “Bordered icons”.


This means that in the “Input window” you have to select the “In” folder you created, and that in the “Output window” you have to select the “out” folder you also created before.

View attachment 10307


Now click in the 64x64 images button.

View attachment 10308


After this I will check all other boxes, the BTN, DISBTN, PASBTN, DISPASBTN, and the Autocast border boxes, just like in the image below.

View attachment 10309


Now it is time to name my icons.


Naming an icon is not easy as well, it has several and important rules:

• When naming your icons, make sure you put the letters "BTN" at the start of the BTN icon filename.


• Put "DISBTN" at the start of the DISBTN icon filename.


• Put "PASBTN" at the start of the PASBTN icon filename.


• Put "DISPASBTN" at the start of the DISPASBTN icon filename.


• Autocast icons are an exception. These icons are regular BTN icons, but they have a different border type. This means that when naming an Autocast icon, it needs to have “BTN” at the start of the file name.​


If you didn’t understand what I wrote before, I will just give an example:

• BTNShrek


• DISBTNShrek


• PASBTNShrek


• DISPASBTNShrek


• BTNShrek_autocast​


So, resuming, the name of an icon is:


IconType + IconCustomName


Be careful, because, a BTN icon to correspond to a DISBTN icon, they both need to have the same IconCustomName. If this doesn’t happen, WarCraft will display a normal good looking icon during the game, but a green box when you hit F10.


Example:


BTNShrek + DISBTNShrek = works fine, good work


BTNShrek + DISBTNFiona = does not work, bad work


The PASBTN and the DISPASBTN icons work the same way, however the prefix is different:


PASBTNShrek + DIPASSBTNShrek = works fine, good work


PASBTNShrek + DISPASBTNFiona = does not work, bad work


The Autocast Icon is an exception, the program can not create a DISBTN version for the Autocast Icon, but don’t worry, I will teach you later how to create it with only a few steps in IrfanView, it is very easy.


So, now that we know how to name our icons, let’s do it!

View attachment 10310


Good, now we just tell the program which type of file the folder has. Click "Please select the target image type:" and there choose BMP.

View attachment 10365


Now it is time to Create some Borders !!! Click the Border images button and the program will create the bordered images in the “output” folder according to the extension ( this means that if the image in the “input” folder is in BMP format, the bordered images created in the “output” folder will be in BMP format. However if the image in the “input” folder is in TGA format, the bordered images created in the “output” folder will be in TGA format, and so on).


After you press the button a box will pop up. Just press the “Yes” button as you have no other choice.

View attachment 10312


Great job until now. Now let’s open the “Output” folder where our new icons are.

View attachment 10350


Wow, great !! I have the icons! BUT WAIT A SECOND .. NOOOOOO the work is not done yet .. why !!.


Well that happens for 1 main reason:

• W3IR is a great program but it has 2 bugs in it. The first one that you can easily see is that the file name is not correct, if you look to the name of the icons, you can read that we have NormalIconName + IconType + IconCustomName which is not correct. If we place this in the WarCraft 3, it will cause it to crash. This is easy to solve, just change the name and delete the NormalIconName part; delete everything before the IconType part.​

View attachment 10351


Good, now we have the correct formula: IconType + IconCustomName.


The second bug it has is that this program fails to create a DISBTN image for the BTN Autocast icon. This means that we will have o create our own DISBTN Autocast icon.


This is very simple to do and we will use IrfanView.


To start creating your DISBTN Autocast icon, open IrfanView. Once the program is opened, use it to open the BTN Autocast icon you’ve created.

View attachment 10352

View attachment 10353


Now that you have the BTN Autocast file opened, it is time to remember what a DISBTN is. DISBTN icons are darkened BTN icons, so this means that in order to make our icon darker; we have to decrease its Brightness. To change the Brightness of an image go to Image > Enhance colors or press Shift + G. A new window will pop up.

View attachment 10354


Now just set Brightness to “-100”. I’ve chosen “-100” because I believe it is a good value, if you wish you can try other values. Keep in mind that changing the other values such as “Contrast” or “Color Balance” per example, will change the quality of your image. Besides, we only need a darker image, so we only need to change Brightness.

View attachment 10355


Now just press “Ok” button. ”DO NOT CLICK IN “APPLY TO ORIGINAL” as you will loose the original image if you do so.


Now that we have our darkened icon, we will use the command “Save AS” and we will change its name to DISBTN_Shrek_autocast (or whatever the name you choose).

View attachment 10356

View attachment 10357


When saving the icon keep in mind the formula and the fact that it needs to have the same name of its BTN correspondent.


BTN_Shrek_autocast >> DISBTN_Shrek_autocast


Good job now look at your folder. If you did all steps correctly and saved the images in BMP format, this is what you will see.


If you saved your images in TGA format, use IrfanView to see them.

View attachment 10358


Good job. Now that everything it is ok, let’s convert all this images to BLP format.


Close IrfanView, we won’t need it for now. Now create another folder and name it “BLP Shrek”.

View attachment 10359View attachment 10360


Now we have 3 folders, “In”, “Out”, and “BLP Shrek” just like in the image. We will use no more the “In” folder now, we will work with the “Out” and the BLP Shrek folders.


Next is the ”Add Borders with W3IR for passive game play icons” section of the tutorial. If you don’t wish to learn it and if you want to move to the final step please go to the ”Converting to BLP with WarCraft 3 viewer” section of the tutorial.

“Add Borders with W3IR for passive game play icons”


Please before doing passive game play icons read the create active game play icons section of this tutorial, because it has all the basics you need to know. Passive game play icons are special BTN icons that DO NOT need a DISBTN correspondent. This makes our job a lot easier. Here I will teach you (I hope) how to create an Upgrade type Border and an Attack / defense type border. W3IR also has some limitations and these are the only types of borders it can create. You can eventually use these icons to do more things and to add to new sets of armors, new attack types, etc. Your imagination is the boss once you learned how to create active and passive game play icons.


I will assume that you already read the Introduction and the Start with IrfanView sections of this tutorial, if you have not read them yet, do it.


To start create 2 folders. One folder is where you have to place the image you just created now (in this case name it “In”), and the other folder is the folder where the program will place the Bordered images it will create (in this case name it “Out”).


Now place your recently created image in the “In” folder. After doing this open the W3IR program. Remember that the images in the “In” folder must be 48x48 pixels size and 24 bits colored.

View attachment 10360

View attachment 10303

View attachment 10304


Good, now press the button Add Borders.

View attachment 10305


If you done this correctly, this new window will pop up:

View attachment 10361


Here is where the challenge begins.


If you look to the image you can see that there are 2 windows. In the left one, you must choose the “input” folder, this is the folder where you have your created image. In the right window, select the “output” folder, the output folder is the folder where the program will place the “Bordered icons”.


This means that in the “Input window” you have to select the “In” folder you created, and that in the “Output window” you have to select the “out” folder you also created before.

View attachment 10307


Now click in the 48x48 images button.

View attachment 10362


After this I will check all other boxes, the Upgrade Border and the Attack Type border boxes, just like in the image below. I do not advice you to check the bordered images string box as it will just complicate things, and it doesn’t create any type of border.

View attachment 10363


Now it is time to name my icons. Let’s remember our rules. These 48x48 sized icons are regular BTN icons, and they do not need a DISBTN correspondent. This way what we need to know is:

• When naming your icons, make sure you put the letters "BTN" at the start of the BTN icon filename.​


If you didn’t understand what I wrote before, I will just give an example:

• BTN_attack


• BTN_upgrade​


To remember, the name of an icon is:


IconType + IconCustomName


So, now that we know how to name our icons, let’s do it!

View attachment 10364


Good, now we just tell the program which type of file is in the folder. Click Please select the target image type: and there choose BMP, if you saved the icon in BMP format, or TGA, if you saved the icon in TGA format.

View attachment 10366


Now it is time to Create some Borders !!! Click the Border images button and the program will create the bordered images in the “output” folder according to the extension ( this means that if the image in the “input” folder is in BMP format, the bordered images created in the “output” folder will be in BMP format. However if the image in the “input” folder is in TGA format, the bordered images created in the “output” folder will be in TGA format, and so on).


After you press the button a box will pop up. Just press the “Yes” button as you have no other choice.

View attachment 10367


Great job until now. Now let’s open the “Output” folder where our new icons are.

View attachment 10376


Wow, great !! I have the icons! BUT WAIT A SECOND .. NOOOOOO (lol, again) the work is not done yet .. why !!.


Well that happens for 1 main reason:

• Well, as I told you before, W3IR is a great program but it has 2 bugs. In this case we are lucky, because we are not affected by the second bug type, but only by the first bug type. If you remember well, the first bug type that you can easily see is that the file name is not correct, if you look to the name of the icons, you can read that we have NormalIconName + IconType + IconCustomName which is not correct. If we place this in the WarCraft 3, it will cause it to crash. This is easy to solve, just change the name and delete the NormalIconName part.

​

View attachment 10368


Good, now we have the correct formula: IconType + IconCustomName.


Do this to all icons.


Good job. Now that everything it is ok, let’s convert all this images to BLP format.


You can create a 3rd folder to save the BLP format images if you want. I decided that it will be more organized, so I created a 3rd folder named BLP Shrek.

View attachment 10359View attachment 10360


Now we have 3 folders, “In”, “Out” and “BLP Shrek just like in the image. We will use no more the “In” folder, now we will work with the “Out” and the BLP Shrek folders.


The next section will instruct you how about to convert BMP files into BLP files.

“Converting to BLP with WarCraft 3 Viewer”


Now it is time to head to the final step of icon creation !! This chapter applies to both active game play icons and passive game play icons. The following examples will be with the active game play icons but it applies to the others as well.


To start open WarCraft 3 viewer.

View attachment 10369


Now just follow the steps on the image:

View attachment 10370

View attachment 10371


If you did this correctly, a new window will appear.

View attachment 10372


Now press ok. A new window will pop up. This window is EXTREMELY IMPORTANT, and you must not ignore it. This window will make you a question about alpha channels. If you remember the introduction, an icon with no alpha channels, is probably a black or a green icon. So, to answer to the question say “YES, add alpha channel to all”.

View attachment 10373


After this important step, if you did everything right, a box saying that it was converted with success will appear. If it doesn’t it is because the program has a bug, or because the installation has some problems. In last case, you can read the hole tutorial again to see if you did something wrong. Post in the forum if you have any questions.


Now close the program and import the icon !! congratulations you now have advanced icon making knowledge !!! enjoy it.


You have created a professional blizzard icon ! =P.


Now use your imagination as it is your limit from now on ! create hundreds of icons and don’t forget to place them in this website!


If you don’t know how to import an icon into WarCraft, read my “Import Icon” section !

 “Edit an Icon”

 “Introduction”


Editing an icon is very similar to creating one, the steps are the same but the order they follow is different. Many people compare editing to stealing, this is not truth; when you edit an icon, you change the work of someone, you don’t steal it. However icons can be stolen, so when you edit someone else’s work, remember to give credit to the creator. Sometimes the creators of icons don’t make a complete package of icons with all icons they should have, other times, you just don’t like the color or some other detail per example. Other times in the map making job you want to create a different attack type, or different armor and you just can’t because you can’t find 48x48 icons.


This section of the tutorial will help you with all those problems, and I will do my best to make sure you will be an iconing master when you finish reading it. In this section you will also learn how to work better with IrfanView and hopefully you shall acquire medium/advance knowledge about the program itself. However remember that it is you who are editing the icon and not me, which means that it must be you who is going to do the creator’s part of the editing work. I will not teach you how to make major changes or completely new images; I will just give you the tools and the knowledge for you to do it.

 “Incomplete packages of images”


This is the most common problem. Most of the time, you find a perfect icon for something, but when you open the package it only has the BTN and DISBTN file or sometimes only the BTN. Sometimes you wan that icon for a passive ability, or for an autocast spell and you can’t because you don’t know what to do. This section of the tutorial will explain all this stuff to you, but remember that it is a lot similar to creating an icon. My advice is READ THE CREATE AN ICON SECTION of the tutorial.


So imagine that you are in the internet site of HiveWorkshop and suddenly you see this icon:

View attachment 10997


OOHHHH SO BEAUTIFUL !!!! I love it !! my map must have it !!!


However when you download it you see a problem:

View attachment 10998


HEEYYY!! There is only a BTN icon … Where are the others !? OMG there is no others … crap I will have to create them myself ..


Yes it is truth, sometimes when you want a job well done; you have to do it yourself.


So to start let’s remember our steps. When we created an icon we first used IrfanView to create the image, after that W3IR and than WarCraft 3 Viewer. This means that now we will start backwards. We will use WarCraft 3 viewer to convert our BLP file into a BMP file. Run WarCraft 3 viewer, and once there press the BLP>TGA icon like the image shows, but choose the BMP format when converting.

View attachment 10999


Ha Ha! There he is. Now press open and, after pressing open, choose the BMP format in the format section and press save (no need to change the name). You can also choose a different directory if you want. But I think there is no need to do that (it is just one image).

View attachment 11000


If you done this correctly the program will ask you the quality of compression. If you remember my long introduction icons must be 24 bits, so it is obvious that you must chose 24 bit compression. After that press ok.


The program can also have a bug (mine has) which will show a pop up box telling you that your icon was converted into BLP format successfully. If that happens just ignore it and don’t worry as your icon was saved into TGA format with no problems.


Now if you saved the BMP file in the original folder where the BLP file was (like did) you probably have something like this by now:

View attachment 11001


This means you are doing a good job.


Now that you converted your icon into BMP format we should use W3IR, however there is a problem. If the icon you downloaded already has a BTN border (probably has, known sites as Hive, per example, only accept icons with borders) it means that if we try to create an Autocast icon (per example) the result will be an icon with two borders – the autocast border and the BTN original border of the icon.

View attachment 11002


An example of an icon with 2 borders – the BTN border and the Autocast border.


To solve this problem we could use the DISBTN icon than but, if we do so our images will be so dark in game that it will be nearly impossible to see them. Also, the DISBTN icons are also border. Than there is only one way out – we remove the BTN border from the BTN icon with IrfanView. If the icon you downloaded has no borders (unusual) you are lucky and you can jump this part of the section and move to the part where I use W3IR. Other way, if you don’t need an Autocast icon, you can also jump this section of the tutorial.


To start run IrfanView, than go to File > Open and search/open for your recently converted BMP file so you can work it.


Once the icon is opened in the program, zoom it to a zoom between 300 and 500. It is important that you choose a value which is a multiple to number 100 (like 100, 200, 300 ...). To understand why, please read the "List of important known bugs in the programs".

View attachment 12842


Now with you mouse (the arrow) select the inner part of the image (don’t select the border, this requires some ability, if you did this well done, when the image is small again, you will not see any grey areas from the border).

View attachment 11004


Now go to Edit > Crop selection or just press CTRL + Y.


If you done this hard step correctly this is what you will see when if you amplify the cropped image.

View attachment 11005


No grey areas form the borders as you can see.


Images with no borders are 56x56 pixels sized. This value is very difficult to obtain when cropping so don’t worry if you don’t get it, just try to get as closer as possible. You can see the pixels size by pressing CTRL + R.


Now if you did this well, you have created another small problem (lol), your image should be 64x64 pixels size and now it is 56x56 pixels. Fortunately this problem has a quick and simple answer. Just press CTRL + R (Resize/Resample), uncheck the box that says Preserve aspect ratio and resize your image to 64x64 pixels size.

View attachment 11006


Good job, however remember that an icon to be well accepted must have 24 bits (24BPP). To see if you icon has this value just press “i”.

View attachment 11007


My icon has the correct value so it means that I can move on. If yours does not have it you should go to Image > Decrease Color Depth or Increase Color Depth and choose the 24 BPP option. For additional help on this section you can read the Create an Icon section.


Now that our image is ok, we will save it, overwrite the old file with this one if you may prefer. I will just press “s” and overwrite the file I no longer need.


Now that we had all this work with removing borders, it is time to add them with W3IR - Yes, life is ironic I know …


If you remember, last time we used W3IR, we created an “In” folder and an “Out” folder. This time my “In” folder will be where these two icons (if you remember the BMP file we just changed is in the same folder the BLP icon you downloaded is) are ( this means that our “In” folder is “Shrek_cat”). So now we just need to create an “Out” folder. Create a folder somewhere you know where to find it and name it “Shrek_cat_BMP_Borders”; this folder is important as by the means of logic it will work as our “Out” folder (“out” folder is the folder where our bordered icons will be).


Now open the W3IR program and click in Add Borders.

View attachment 10305


If you done this correctly, this window will pop-up.

View attachment 10306


Good, now it is just like creating an icon. Select the “Shrek_cat”/the folder where your two icons are) in the Input window (left side) and the “Shrek_cat_BMP_Borders” in the Output window (right side). Now choose the pixels size of the image (normally is 64x64 because the HiveWorkshop and all other sites rarely have 48x48 pixels sized images), check the BTN, DSBTN, PASBTN, DISPASBTN and the Autocast boxes (just check the type of border that you want) and name your images (read the create an active game play icon section to better understand this). Finally select the type of image you want to convert (in this case BMP).


To finish just press the Border images button and say yes to the window that will appear.

View attachment 11008


Now if we make a quick visit to our “Out” folder (Shrek_cat_BMP_Borders) we will see a “bug” from the program.

View attachment 11009


The program saved all our images with the previous name. This “Bug” doesn’t mean that the images don’t work, because they still work, however if we import them into WarCraft this way, we won’t know which images are passive, autocast or even disabled. This will cause green icons and in the worst scenario game crashes. However this fearsome bug is very easy to delete. Just change the name of all icons to their correct formulas: IconType + IconCustomName.


This means that (in this particular case) we must remove the “BTN_Shrek_cat” part of the name that is repeated in the start.

View attachment 11010


These are the names you should have after you change the name of the files to the correct formula.


Now to end I will create a folder named “BLP cat”. This is the folder where you will place the BLP icons we will create now. Yes, if you read the Create an Icon section you will understand why it is so important to use BLP icons instead of TGA.


After creating the folder open WarCraft 3 Viewer and once there convert all your BMP files into BLP files with the method described above.

View attachment 10369


Now just follow the steps on the image:

View attachment 10370

View attachment 10371


If you did this correctly, a new window will appear.

View attachment 10372


Now press ok. A new window will pop up. This window is EXTREMELY IMPORTANT, and you must not ignore it. This window will make you a question about alpha channels. If you remember the introduction, an icon with no alpha channels, is probably a black or a green icon. So, to answer to the question say “YES, add alpha channel to all”.

View attachment 10373


After this important step, if you did everything right, a box saying that it was converted with success will appear. If it doesn’t it is because the program has a bug, or because the installation has some problems. In last case, you can read the hole tutorial again to see if you did something wrong. Post in the forum if you have any questions.


Done, you edited my incomplete icon!!! Now you just need to import it into WarCraft 3! If you don’t know how to import an icon go to the Import icon section of my tutorial. Congratulations as you already know how to complete those incomplete packages of images =)

 “Transforming a 64x64 pixels image into a 48x48 pixels image”


MM, imagine that when you download an icon you want that icon to represent an armor, or even an attack type. The first problem you will have is that it is very hard to find and download 48x48 from any website. People just prefer to do 64x64 pixels icons because they are more used. So, the only option that is left for you is to learn how to convert a 64x64 icon into a 48x48 icon.


Well I created a 64x64 icon with another Shrek image and it has the name of BTN_Shrek2.BLP. I created a folder to this new icon and named it Shrek_64x64. You should also create this folder, unless if you already have it.


To start we must convert our BLP icon to BMP format with WarCraft 3 Viewer. So, let's do it =) .


Run WarCraft 3 viewer, and once there go to the BLP > TGA icon, just like the image shows.

View attachment 12147


Now just follow the image, and save your icon in BMP format:

View attachment 12148


If you did this correctly the program will ask you the quality of compression. If you remember my long introduction icons must be 24 bits, so it is obvious that you must chose 24 bit compression. After choosing your option press ok.

View attachment 12149


The program can also have a bug (mine has) which will show a pop up box telling you that your icon was converted into BLP format successfully. If that happens just ignore it and don’t worry as your icon was saved into BMP format with no problems.


Now that you converted you Shrek2 icon into BMP you have something like this in the Shrek_64x64 folder.

View attachment 12150


Well because 64x64 BTN icons have borders and 48x48 BTN icons don’t we will have to remove the borders from our recent BMP icon. No problem, if you read the last section you will see that we already did this step before.


To start run IrfanView, than go to File > Open an search for your recently converted BMP file so you can work it.


Once the icon is opened in the program, zoom it to a zoom between 300 and 400 keeping in mind that the value you choose must be a multiple of 100, to avoid a bug from the program. To know why visit the List of important known bugs in the programs section of the tutorial.

View attachment 12843


Now with you mouse (the arrow) select the inner part of the image (don’t select the border, this requires some ability, if you did this well done, when the image is small again, you will not see any grey areas from the border).

View attachment 12152


Now go to Edit > Crop selection or just press CTRL + Y.


If you done this hard step correctly this is what you will see when if you amplify the cropped image.

View attachment 12153


No grey areas form the borders as you can see.


Images with no borders are 56x56 pixels sized. This value is very difficult to obtain when cropping so don’t worry if you don’t get it, just try to get as closer as possible. You can see the pixels size by pressing CTRL + R.


Now it is different, if you did this well, you have created another small problem (lol). However you can take advantage of it. As you know the image we want to create is 48x48. Following the steps, we started with a 64x64 pixels image and now we have a 56x56 pixels image (image with no borders are 56x56 pixels). Now follow the logic, the time for us to resize the image to 48x48 pixels has come. Just press CTRL + R (Resize/Resample), uncheck the box that says Preserve aspect ratio and resize your image to 48x48 pixels size.

View attachment 12154


Good job, however remember that an icon to be well accepted must have 24 bits (24BPP). To see if you icon has this value just press “i”.

View attachment 11007


My icon has the correct value so it means that I can move on. If yours does not have it you should go to Image > Decrease Color Depth or Increase Color Depth and choose the 24 BPP option. For additional help on this section you can read the Create an Icon section.


Now that our image is ok, we will save it, overwrite the old file with this one if you may prefer. I will just press “s” and overwrite the file I no longer need.


Now that we had all this work with removing borders, it is time to finally transform our icon into what we want – a passive game play icon.


To start close IrfanView. After that create a folder and name it “Shrek_48x48_BMP”. This folder will be our “Out” folder. To know what an “out” folder is read the Create an Icon section of the tutorial. Now finally open W3IR.

View attachment 10305


If you done this correctly, this window will pop-up.

View attachment 10306


Good, now it is just like creating an icon. Select the “Shrek_64x64” in the Input window (left side) and the “Shrek_48x48_BMP” in the Output window (right side). Now choose the pixels size of the image (in this case is 48x48), check the “Upgrade Border” and the “Attack Type border” boxes and name your images (read the create an active game play icon section to better understand this). Finally select the type of image you want to convert (in this case BMP).


To finish just press the Border images button and say yes to the window that will appear.


Good, now we have our Shrek icon turned into what we want. If you did this correctly this is what you will see when you open your “Out” folder.

View attachment 12155


Now if you look closely to the images you can see that there is something wrong - the icon Formula.


The program saved all our images with the previous name. This “Bug” doesn’t mean that the images don’t work because they still work, however if we import them into WarCraft this way, we won’t know which images are passive, autocast or even disabled. This will cause green icons and in the worst scenario game crashes. To solve it, just change the name of all icons to their correct formulas: IconType + IconCustomName.


This means that (in this particular case) we must remove the “BTN_Shrek2” part of the name that is repeated in the start.

View attachment 12156 The correct name for the icon.


These are the correct names you should have.


After this step, just close W3IR as we won’t need it anymore.


Good, now let’s finish our job. Create another folder named “Shrek_48x48_BLP”.


Now we have 3 folders:

Shrek_64x64

Shrek_48x48_BMP

Shrek_48x48_BLP


After creating the folder open WarCraft 3 Viewer and once there convert all your BMP files into BLP files with the method described above.

View attachment 10369


Now just follow the steps on the image:

View attachment 10370

View attachment 12157


If you did this correctly, a new window will appear.

View attachment 10372


Now press ok. A new window will pop up. This window is EXTREMELY IMPORTANT, and you must not ignore it. This window will make you a question about alpha channels. If you remember the introduction, an icon with no alpha channels, is probably a black or a green icon. So, to answer to the question say “YES, add alpha channel to all”.

View attachment 10373


After this important step, if you did everything right, a box saying that it was converted with success will appear. If it doesn’t it is because the program has a bug, or because the installation has some problems. In last case, you can read the hole tutorial again to see if you did something wrong. Post in the forum if you have any questions.


Done, you transformed a 64x64 complete icon into a 48x48 icon!! Now remember, that attack type icons can be used as armor icons and others. As you can see you will soon enough be ale to do anything with icons. Just stay around to see my next chapter. Now you ca import your icon to warcrat 3 !! Have fun !


To create a 64x64 pixels image from a 48x48 image just follow this steps as well, however don’t remove the borders and use IrfanView to resize you image to 64x64. After that Select the 64x64 option in W3IR.


If you have trouble converting a 48x48 into a 64x64 icon, just read the Create an icon section. If that doesn’t help you just post something in this forum and I will create a section for it (If there are many post requesting the same).


If you don’t know how to import an icon go to the Import icon section of my tutorial.

 “Modifying an Icon”


There are hundreds of ways to modify an already existing icon. In this section of the tutorial I will use IrfanView and W3IR to make a few changes in the icons. Remember that IrfanView and W3IR are not professional image programs so they are very limited. What I am going to put here all programs have. My advice is for you to work with a professional image program because you will have more freedom to do whatever you want.


I will start by using IrfanView and after that W3IR.

 “Modifying with IrfanView”


So to make a quick introduction, IrfanView is a program wit limitations but still a program all can use. This is the main reason why I am dedication to much time to it. I decided not to use trials because they have several bugs, take hours to download and mainly because they only last 30 days … What you are going to learn here also applies to all other programs I mentioned making of IrfanView the perfect program for minor/medium changes in icons. If you wish to make radical changes I suggest professional programs like those I mentioned in the start.


So we will learn a few interesting things here with IrfanView. The icon I will use is this one:

View attachment 12209


I named my icon BTN_Shrek3.bmp ; yes I will work with BMP files because IrfanView does not support BLP files and because they offer more vantages when being edited, unlike TGA files. You can use TGA files as well, they will not be a bad choice but I recommend BMP. Non less, I also made a BLP version of this icon, so if you wish you can download it.


Anyway, after entering IrfanView and after you opened your image, use the zoom and zoom your image to a value between 200 and 400, remember that the value must be a multiple of 100. To know why visit the List of important known bugs in the programs section.

View attachment 12844


Now, all image editing function of this program are available in the Image menu. I will use some so you can see their effects.


If we have a look on the menu we will see the functions are in groups. First we have the image information’s, after the creating an image functions, rotating functions, resize and Canvas functions, the Color Depth functions and finally the coloring/editing/effect functions.


I will not loose time with the information, creation and rotation functions by some reasons:

 Information function only gives you information about your image. As we saw earlier it is very useful to know the color bits of the image and other things but it can not be used to edit anything.

 IrfanView can not create an image all by himself; it needs a base image to do it. Even if you create an image with IrfanView, it will never be as good as other images created with professional programs. To resume, this section is not important for icon modelling

 Rotation functions are extremely easy and intuitive of using. They just rotate the image or flip it. They are useful to icon modelling but to resume they are so easy to use, I think everyone can understand what they do. This means that you don’t need my help in these functions. Just experiment them.


Next are the Resize/resample and Canvas functions. The resize functions are very often used in this tutorial so I think that you already know what they do – they resize. If you have troubles with the resize functions, just read the Edit an icon or theCreate an icon sections of the tutorial.


So we have the Canvas function left. Canvas function creates an area around the image. It is just like a picture in a wall. Canvas adds a border to an image. So you are wondering, why can’t we use IrfanView to make a BTN icon? Well we could use IrfanView to do that but the quality of the icon would be very poor. IrfanView is not detailed to make 3D buttons (although he does have that function), and when it does them they loose quality when compared to border programs like W3IR. Anyway I will just create a nice blue border.


To create a Canvas border go to Image > Canvas size.

View attachment 12211


And now here is the result:

View attachment 12212


Nice don’t you think?


Now next to the Canvas function we have the Color Depth functions. These functions are used to change the colored bits of an image. Its job in icon modelling is to change the color depth of an image to 24 BPP. So all this means that the Color depth functions are very useful but not here. I want to show effects and changes and besides, you already are familiar with this function, we used it a lot in the previous sections of this tutorial. So, if you have any questions, just read the Edit an icon or theCreate an icon chapters of the tutorial.


Next to this function we have the coloring/editing/effect functions. These are the best functions to anyone use if you want to change something in an image.


If you look to the menu first is the Convert to Greyscale function. It is not hard to know what this does … it makes your image grey.


To access this function go to Image > Convert to Greyscale.

View attachment 12213


Not difficult at all.


After this, we have the Negative function. It is also very easy to use and to know what it does – converts your image to negative form.


To access this function go to Image > Negative.

View attachment 12214


It seems you are watching some of those weird blocked channels that you have to pay to be able to see them… like SportTV (what were you thinking!! you are sick! =P).


Now we have the Enhance colors function. If you remember we already used this function to create a DISBTN_Autocast icon. This function is extremely useful when editing.


 To access it go to Image > Enhance colors.

View attachment 12215


As you can see you can change the Brightness, Contrast, Saturation, Color Balance and Gamma correction values of the image. This allows you to make moderate changes in it. With the right values you can improve your image or just have some fun seeing what happens. This extremely useful tool can not be thought. We must use it and experiment it in order to understand what it does. Per example, if you want to create a DIS icon, you just need to set the Brightness values to negative. If you want your image to be redder, just change the color balance, if you want it to be more transparent change the Gamma correction values and so on. You choose what you need and what you do.


Next to this important tool we have another decisive function – the Auto adjust colors function. It is not difficult to know what this wool does; it balances the colors of your image. You should use this tool to improve you icon as harmony between colors is very important. My icon is already balanced so I don’t need to use this function (DreamWorks did a great job when they created this wallpaper =P), but I strongly recommend that you do so.


To access this function go to Image > Auto adjust colors.


After the Auto adjust colors function we have the Sharpen function. This function allows you to see your image better by increasing the Contras levels it has. This means that you can use it to improve small details, but if you use it too much it will eventually ruin your image. My advice is using this function if you need, but always keep the original file for backup.


You can access this function by using Image > Sharpen

View attachment 12216


This image is ruined because I used the function Sharpe too much. Fortunately I have my original file as I backup. Phew …


Next to he Sharpen function we have the Red eye reduction (selection) function. This function is used to remove red eyes when you take a picture, so we won’t need to waste time with this function unless you want to convert one of your photos into a WarCraft3 icon. I won’t explain this function because I would waste my time if I did so and because there is another extremely important function next to this one.


I am talking about the Effects function. This function allows you to create several effects for you icons. When using this function I recommend that you use it by going to Image > Effects > Effect Browser. The Effect browser allows you to see the result of your experiments in a instant way.

View attachment 12217


Here I used the Shift Colors effect. There are lots of them for you to use.


Another great thing about effect, is that you can combine them !!

View attachment 12218


In this image, after saving the first effect I am using a second one – the Swirl effect.

View attachment 12219


This is what my icon looks like with the 2 effects together.


Now in the last place, my favourite function – the Swap Colors function. This function swaps the colors of your icon using the RGB (Red, Green and Blue) definitions. It is very easy and beautiful to use, you can use this to create a new ambient for your icon and, just like the effects, you can double use it!


To access this function go to Image > Swap colors > chose anything!.

View attachment 12220


This is my new purple ogre ! (lol). To do this I used the Image > Swap colors > RGB -> RBG function.

View attachment 12221


Now say hello to my new weird ogre! After using the first function (RGB -> RBG) I decided to use RGB -> BRG, and this was the result.


Quick and simple! There is nothing better =P.


Now you learned the most important, and the best editing tools IrfanView has. You now have advanced knowledge about this program (together with the information within the other sections). However as I told you before this program is limited and I recommend you to explore new image programs. Now it is time to use W3IR.

 “Modifying with W3IR”


If you don’t have time to experiment changing an icon with IrfanView, or you just don’t have the patience to do it, than W3IR is your solution. This program allows you to make minor changes for your icons while bordering them.


Do you think that your icon is really cool?? Than try to see how it is with some weird effects and make it better! Why have only 1 icon when you can have a whole bunch of them!!??


This section will show you why it is so important to save your files in BMP format! This section only works for those who saved their icons in BMP.


So here it goes. I will use another function of the program. Before using this section of the tutorial I strongly recommend that you read the Create an Icon section because you will need an icon to use this function of the program. This section applies to both 64x64 and 48x48 icons as well as the others.


Now a quick summary, after saving your images in BMP format, remember that you must have two folders – an “Input” folder and an “Output” folder.


Now to start Open W3IR and click in the add borders button, like the image shows.

View attachment 10305


After all this click in the RGB scramble button.

View attachment 12241


Now a window will pop up with white and black boxes don’t worry, we will give a good use to them soon.

View attachment 12242


Now click in the Load Icon button, and search for your image so you can open it.

View attachment 12243


Now that the program loaded the image, just click in the Process button.

View attachment 12244


MMM I wonder what will happen.

View attachment 12245


WooooW look at that!! 15 new weird icons!!! Crazy Merlin did his crazy stuff again =P.


What happened is that when you pressed the “Process” button, the program just changed all Red, Green, and Blue channels. It also created a few more effects, as you can see.


MMM I am particularly interested in the GBR image. It will probably be nice in my map. I want it!!


To select an image you like just check the box bellow it. In this case, I will check the box that says GBR.

View attachment 12246


Now all I need to do is tell to the program the format I want for this image. I will choose BMP.


To save your image press Save imgs button and say “Yes” to the box that will appear. BE CAEFUL, the box says that if you place 2 icons in the same folder, 1 original, and the other an RGB modification, with the same format the program will replace the first icon (the original one) with the RGB modification. You may want to be careful when selecting your “OUTtput” folder in the “OUTput window”.


Congratulations, Now you now how to do small/medium changes on icons. Imagine that you are looking for an ice arrow image, but instead you find a perfect fire image ... Just use the change the RGB channels and make it blue !! Or imagine you want an icon to represent stun or dizziness, just use IrfanView to do it!


You now have advanced knowledge about these 2 programs (together with the information of the other chapters) so feel free to use it =).


Now it is time to import the icons =)

 “Importing an Icon”

 “Introduction”


Importing an icon is the basic knowledge that I assume the most of you already have. However I still advice you to read it. This importing section is not only to teach how to import icons… there are already hundreds of cheap tutorials teaching that … I will try to go further, I will try to teach about how to use your created icons. Sometime you create a perfect icon to be shown in your map; you want that icon to represent something in your map beyond ladder values like gold and lumber … imagine that you want your icon to represent a source of plasma energy that will be used instead of gold or lumber but you don’t know where to put that icon. In this section I will teach how to import correctly and how to use imported icons as well within the world editor.

 “Importing and using 64x64 icons”


Great, now that you already have you icons created (or selected) it is time to import them into the game. But how do we do that? Well if you have TFT version this will be easy, if only have ROC there will be a few problems. This tutorial will explain how to import icons in TFT because with ROC you will need a special program to do that.


Importing to TFT is relatively easy.


To start open WarCraft. After that go to the Import Manager (F12).

View attachment 12247


Good, now a new window will appear. In that window choose the Import File option or just press CTRL + I.

View attachment 12248


Good, now once you click in it you just need to browse for your files (icons). This is the easy part.


Once you have done the importation of your icons you will have something like this.

View attachment 12249


If you read the image you will see it is right. The names of the icons may be correct, however if you play the game with that path your icons will be green. This means that the path is incorrect and that you must change it. But how to know the correct path for your icon? Well there is another formula for that, depending on the type of your icon.

<>ReplaceableTextures\CommandButtons\BTN + IconCustomName( _cat in this case) + .IconFormat( BLP in this case) = BTN_cat.blp


<>ReplaceableTextures\CommandButtonsDisabled\DISBTN + IconCustomName + .IconFormat = DISBTN_cat.blp


<>ReplaceableTextures\PassiveButtons\PASBTN + IconCustomName + . IconFormat = PASBTN_cat.blp


<>ReplaceableTextures\CommandButtonsDisabled\DISPASBTN + IconCustomName + . IconFormat = DISPASBTN_cat.blp​


These are the paths you need to know. So basically we have ReplaceableTextures\CommandButtons\ + IconType + IconCustomName +. IconFormat


This is not as hard as it may look. However keep in mind that the BTN path is used to the Autocast Icon as well and is used for the all 48x48 icon too.


Now we just need to change the path of the files to the correct formula. To change a file path, double click on the file. After that a new window will appear. In that window check the box saying Use custom path and than just use the formula.

View attachment 12250


When you finish importing the icons, you should something like this image:

View attachment 12251


Now it is time to use our imported icons. BTN and DISBTN icons are used to both units and abilities, however, PASBTN, DISBTN and Autocast icons are used only for spells and abilities. I will now teach you how to use your icons. This is extremely easy I think everyone can do it, however I will loose some of my time teaching you.


Now to use your icons, close the Import Manager and open the Object editor.

View attachment 12266


Very easy until now. Now I will first use my icons for some abilities and than for units.


To change the icon of an ability select the abilities section of the object editor.

View attachment 12253


Now I will change the icons of the archimage and the priest abilities. I will start by the archimage.

View attachment 12254


Well this is how it is done for heroes. You need to change those two values in point 2 because there are two images – the game icon and the learn/research icon.


For the passive abilities such as the Brilliance aura use the PASBTN icon. Here you don’t need to worry about any DIS file because, if the path is correct and if the IconCustomNames of the BTN and the DISBTN are the same, the DIS version will be shown automatically by the game when you press F10.


Now for the priest I will edit the Autocast icon. Again you don’t need to worry about it’s DISBTN version as it will be shown when you press F10.

View attachment 12255


Autocast need two fields because they have two types – when they are on and when they are off. Normal icons don’t need the off version, so you only need to change the first value because the other one is None[/I ] by default.


Now I will change the icon of peasant into a beautiful cat. Unit’s icons are very similar to normal abilities icons because you only need to change one field.


To start move select Units in the object editor.


After that select the peasant and follow the image.

View attachment 12256

View attachment 12257


These are the most important things about importing 64x64 images. I will not teach you more details because I think it is very basic to do what I just did. Now the 48x48 are a bigger challenge.


You will probably be surprised with what you can do with them and I strongly advice you to read the next section as it has very important things that can be applied to 64x64 icons as well.


This part of the tutorial has a map. You can and you should download it to better understand the lesson.

 “Importing and using 48x48 icons”


Now that you know now to import 64x64 icons, importing 48x48 icons will be the same. It will be even easier because 48x48 are only BTN icons who don’t need a DISBTN correspondent.


To start open WarCraft. After that go to the Import Manager (F12).

View attachment 12247


Good, now a new window will appear. In that window choose the Import File option or just press CTRL + I.

View attachment 12248


Good, now once you click in it you just need to browse for your files (icons). This is the easy part.


Now, once you have done the importation of your icons you will have something like this.

View attachment 12258


Now it is time to change the paths. If you remember our last chapter the BTN paths will be:

<>ReplaceableTextures\CommandButtons\BTN + IconCustomName + .IconFormat( BLP in this case) = BTN_attack(or _upgrade).blp[/INDENT


So it is time to correct this. To change the paths, the process is equal to the one I explained before.

View attachment 12259


When you finish importing you will have something like this.

View attachment 12260


Now it is time to use our icons. The newest map makers will defiantly be surprised about what they can do with these icons.


To start close the Import Manager.Then, in the main menu, go to Advanced > Game Interface. After that check the box saying Use custom interface.

View attachment 12261

View attachment 12262


Good. Now it is time to make some damage!


I will change the icon of the normal attack into my new Shrek icon.

View attachment 12263


As you can see we must change those two values. The first one is the icon with the upgrade border, and the second one is the icon with no upgrade border.


This means that in the first filed we will place our BTN_upgrade.blp icon and in the second field the BTN_attack.blp icon.


I will start by changing the first field, and than the second.

View attachment 12264


Do the same to the second field but choose the BTN_attck.blp icon.


You end this part, you will have something like this:

View attachment 12265Continues on the next post

Continuation


As you can see, if you continue on, you will be able to change most of the values, like gold, lumber and food. However be careful as the Attack, Move and other icons are 64x64 pixels images. In this case you must know the game to know which icons you can change.


You can also use the BTN_attack.blp to change armors. In my map I changed the piercing attack and the large armor.


If you find a 64x64 icon (like move icon) and if you want to change it, you will have to use a 64x64 image, but the process will be the same. However if you use a 64x64 icon remember that you will need its DISBTN correspondent (yes, you will want to use BTN icons because PAS icons will be wasting your time as you won’t even be able to see the properly).


Now I will change the adept/master training icon of the priest. To do this enter the object editor and choose units

View attachment 12272


Now select the priest unit and choose the field caster upgrade art so you can change it.

View attachment 12273


It is done. Now you know a little bit more. I decided not too loose much time in this section as well. Because of what I said before, I consider importing and using very easy to do.


If you have any questions or if you want to learn more just post something in the forum and I will see what I can do. Importing and using the icons it’s the easiest part of icon making.


This section of the tutorial has a map as well as the other. You can and you should download it to better understand what is I the tutorial.


Now it is time to end my tutorial with my last section – Fixing an Icon

 “Fixing an Icon”

 “Introduction”


In contradiction to the other sections of this tutorial, I will not tell you exactly what to do in this section. To understand the meanings and the concepts I am going to write here you have to read my tutorial. However if you already have some knowledge about icon making you will probably understand what I mean. Next to this section there is also has a list of bugs that the programs you use can have, so I advice you to read it.

 “Fixing Green Icons”


There are millions of ways to make an icon to appear green in WarCraft, however I will write here the most common.

I will start from the beginning. A common problem is the size and color depth of an icon. If an icon does not have 64x64 or 48x48 pixels and does not have 24BPP (24 bits color) it will be green. To solve this problem convert your icons into BMP and set the icons to 64x64 (or 48x48) pixels and 24 BPP

After that we have the name of an icon. To be well accepted by the game the icon name must follow a formula. The formula is: IconType + IconCustomName + .FileFormat = BTN_myicon.blp , per example.

If the icon name is correct check for its correspondent, remember that 64x64 icons MUST always have a DISBTN correspondent, with the same name. In this case the formula is: DISIconType + IconCustomName(equal to the correspondent icon name) + .FileFormat = DISBTN_myicon.blp . PASBTN and DISPASBTN are NOT an exception to this formula. Only 48x48 icons don’t need a DIS correspondent

Finally and the most common cause the Import path is usually wrong. To correct an import path use the formula: ReplaceableTextures\CommandButtons\IconType + IconCustomName + .FileFomat = ReplaceableTextures\CommandButtons\BTN_myicon.blp .Keep in mind that he previous rules also applies here.


These are the main reasons why your icons don’t work. If you have more questions or any trouble please post them in this forum so that I can help you solve your problems.

 “Fixing Black Icons”


The main reason (and the only I know) for icons to appear black in game it is because they don’t have alpha channels. This problem is most frequent in TGA format icons. To solve this problem convert your icon into BLP format with WarCraft 3 Viewer. This program will convert your icon and it will add the so needed alpha channels automatically.

 “List of Important Bugs in the Programs”

IrfanView:

1. - Sometimes you can not undo your actions. I recommend that you save you files with regularity.


 2. - Also sometimes when you zoom to a value which is not a multiple of 100, your zoomed image can get "cut" in some areas, thus i am forced to recommend you people to zoom to values that are multiples of 100 (like 200, 300 and so on).​

W3IR:


1. - This program cannot create a DISBTN file for the Autocast icon. This means that you have to create it. Read my tutorial as it teaches how overrun this bug.


2. - Although this program can name icons, when you finish adding borders to them, the program doesn’t work properly. In the case the bordered new icons have a double name which needs to be deleted manually.​

WarCraft III Viewer:


1. - Sometimes the program crashed due an unknown issue. My advice is for you to save your files regularly.


2. - When you finish converting a BLP file into another type of file, the program says that the issued file was converted into BLP successfully. Ignore this bug as it causes no harm to you, your file was converted correctly in the desired model.


3. - When you convert a TGA image into a BLP image, the program flips vertically your image. This means that what should be up will be down and what should be down will be up. There are two way of correcting this bug:

a. Save your files in BMP format and convert them into BLP

b. If you have already saved them into BLP format you will have to convert the BLP file into a BMP file. After that use IrfanView to flip vertically your recently BMP created image. Once it is in the correct position convert the BMP file into a BLP file again.​

WarCraft III World Editor:


1. - Sometimes when using imported icons in the Game Interface menu the program just does not recognize the file. To solve this either create another map or close the current map and open it later. My advice here is to always save regularly your job and to always use a test map file where you do your experiments such as changing important game values like this one.


2. - Happens that after importing an icon the program does not recognize it immediately in the editor. To solve this save the map, close the program and reopen it a few minutes later. It also can happen that when making your own map that the amount of data information is so big that the program does not recognize the icons in order to have extra virtual memory. This bug cannot be solved but don’t worry the icons will always appear when playing regular WarCraft.​


These are the bugs I experienced while I was creating my icon. I hope they help you avoid some of the troubles I had. If you had any extra bug problems please report them as I would like to complete my tutorial as much as possible.


Done! Now you are a professional with icon making !!! I hope you enjoyed. Down here it is my Bibliography section and the links of some professional image making programs you can experiment. You can also download the installation files of the programs I used together with my maps.

 “Bibliography”


To create this super tutorial I used information from:

• Creating Custom Icons by Darg


• 6 years of WarCraft professional game play and 4 years as a lonely map maker (yes it took me a lot of time to know what I know now)


• W3IR program tutorial (here available for download)


• THW rules about icons (also available for download)​

 “Websites and Downloads”

 “Websites”


Here you will found a list of other icon making websites and tutorials that will help you:

Icons Guide by Ralle

Importing and Fixing Icons(Roc and TFT) by ElfWarfare

My icon is green in game ... why? by ElfWarfare

Game Interface - Basics by jonadrian619

How to make an Icon by Tiki

 “Downloads”


Here you will find a list of programs you can use. This includes professional image programs and the programs I used:

Paint Shop Pro

Adobe PhotoShop

Ulead PhotoImpact

GIMP

IrfanView (you can download the version i include on my tutorial)

W3IR (you can download the version i include on my tutorial)

WarCraft III Viewer (you can download the version i include on my tutorial)

 Credits


me

myself

I

Flame_Phoenix (me, myself, I)

 Special Thanks


 Thx to BlackDoom for giving his opinion and making constructive critics on my work. Thx to him I improved my tutorial.

 Thx to Wolverabid for telling me about Blackdoom and for his comments

 Thx to PurplePoot for being the first and nearly only moderator that motivated me to finish the tutorial

 Thx to low_kwaliti for defending me against some angry moderators and for commenting my work as well.

VERY VERY THANKS to all these people and to all those i forgot to mention that somehow contributed with their comments and opinions to help me make a better work. Also thx for the reputation points they all gave =D .


Now here are the maps, icons and programs that you can and you should download. Hope you have fun making icons now =)


Also the Import maps with icons are here. You should download them to better understand my tutorial if you want to.​

​

#### Attachments

icontut.txt

 77 KB

 · Views: 721




 Last edited: Sep 17, 2007

#### 3ICE

#### 3ICE

 Level 5

JoinedJul 4, 2007Messages84

Since I hit the post length limit (80000) I must doublepost...


i thank you mainly for this part:

 “Fixing Black Icons”


The main reason (and the only I know) for icons to appear black in game it is because they don’t have alpha channels. This problem is most frequent in TGA format icons. To solve this problem convert your icon into BLP format with WarcCraft 3 Viewer. This program will convert your icon and it will add the so needed alpha channels automatically.


Click to expand...

I made 50 icons (of the ABC + numbers + special chars) (how original eh?) They look cool with warcraft style handwriting letters but in game they were black. So i came here and read your tut and now the icons work. THX! (im gona submit it to the icons resources once im done with the DISBTN part too)


edit:


found another typo: creatin - Creating (its in the title)


edit:


nvm not gona submit... they suck


preview:

View attachment BTNQ.tga

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

3ICE thx for your posts, rep points and for your effort to correct my spelling mistakes =).

I will correct the errors as soon as i can. Also thx by the huge post, still i want to find the errors myself because i have them in a Word document as well, which i may post soon for all people to download (still the file is 13mb, so i don't know if THW will actually accept it).

As i said b4,if you find errors, please just tell the sections where they are in as i prefer to correct the errors by myself (still thx for the enthusiasm, my tutorial has over 70 pages and 110 images, i believe it is a challenge to read it, now think about correcting it =P, + rep for you too  )

Also i feel good to know that i actually helped you with your icons, that is mainly why i decided to write this tutorial alone, because i want to help people like you, and receiving posts like the one you did really helps psychologically, it's just god feedback you know =).

Also i don't think your icons suck, i think that many people may find them useful once in game, your icons may create their own kind of game by using the letters. Just imagine people playing warcraft scramble with your icons, or pictionary, any game with words and/or numbers will use your icons, and will motivate people to create and play new types of games.

 Thx for the posts =) Good Luck with Icon making =) Flame_Phoenix


Click to expand...

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Want a different title?

How about...

"A Complete Guide To Icons"?

In other news, 3ICE, did you make them 24bit? They seem to not work in other formats. (or whatever the heck you call those bit-sizes)

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

"A Complete guide to icons" wouldn't be a bad idea for a title... Wouldn't be at all, however my tutorial is not just a "guide" it is much more than that, thus making me refuse such a title. However the word "complete" does offer a lot of new suggestions. Thanks for the tip Purplepoot.

Also yes, to work properly the icons must be 24 bits. Some people say they can be 32 bits, but that format is way less efficient and takes more memory, thus making choose 24 bits. Good icons are always 24 bits.


 PurplePoot said:



 or whatever the heck you call those bit-sizes


Click to expand...

Well, i call them 24 bits ... lol ...

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Well, the thread title "Creatin editing importing fixing ALL ABOUT ICONS by Flame_Phoenix" needs an update... it's really long.

So... can you come up with a shorter name? And why isn't your tutorial a guide? That's what it seems to be to me.

Maybe... "The icon maker's handbook" or something?

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

1st: Not a guide. Guides only give indication about how things should be done, does not teach or makes the things themselves. My tutorial is much more than a simple guide because it has the theory and the practice components. it does not ONLY gives you indication, but it does teach you about how to do everything, thus the "guide" thing would be a bad choice. In my tutorial i teach you guys and i make the same icons you do. guides don't do that, they ONLY give an overall introduction. My tutorial has very specific and detailed information for all sections, thus not being a simple guide.

2nd: I already changed the dam title !!! "The SUPER Icons Tutorial with ALL about icons". This is the title on the top of the tutorial.... I just don't know how to rename the thread ... help please ??

3rd: "Handbook", again, you an excellent idea ruined by only 1 word. According to the dictionary handbooks are small books that give an overall information about the subject in cause. As i said before, my tutorial doesn't offer an overall view, but a very detailed component.

Still thx by the tips =) + rep for you =)

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Is your work complete here Flame_Phoenix?


~ Thread renamed.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

OMG, you renamed my thread ..

1 - Please next time you do so, let me know

2 - I would like to change the thread according to the new icons tutorial title ... Can some1 teach me how to rename threads !?

And finally to end, Wolverabid, please read my dam posts about the W3 hidden stuff. I need some replies to move on, and my time will soon be over !!!

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Only Moderators may rename threads.

This tutorial is long and detailed, I still need to go over it to correct some minor spelling and grammar issues. I will try to get it done by tomorrow. Afterward, I'll approve it, clean the thread and move it.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Lol, i am also trying to correct the grammar and spelling mistakes ...

Take all the time you need.

Also please change the thread's title to the tutorial actual name ...

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Thread renamed to

Super Icons Tutorial - All About Icons.

Until further notice. (Wolverabid has higher authority, of course, so it's his final decision as well as partly yours, flame)

I shortened the title a bit as you can see, it was a little long before, but I think it gives the same idea.

I still think "A complete guide to icons" gets across the same idea, plus sounds better, etc., though.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Lol, thx PurplePoot. however i still think the word "guide" is too weak .

Just read my whole tut purp and i promise you will even be able to make an icon with your screensaver =)

#### UchihaSasuke.gr

#### UchihaSasuke.gr

 Level 20

JoinedMar 11, 2007Messages2,822

Resources: 6

#### Resources
Models 1Icons 4Tutorials 1

That tutorial was really outstanding!!!This will help a lot newcomers to icons!

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Thx for your comment sasuke.gr. I have already seen some of your posts and i am happy to know that my job is good to the point of making you make such a comment. Thx by the feedback =)

However i am confident and i also know that many professional icon makers will also learn lot's of stuff with my tutorial, as it has information for all audiences =)


 Last edited: Sep 20, 2007

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Wee Guys, i think i checked all spell mistakes in my tutorial, my grammar corrector program didn't find any.


What else do i need to do to see this tutorial approved ?? It is been here for a while.

#### Kwaliti

#### Kwaliti

 Level 37

JoinedJun 7, 2007Messages2,394

Resources: 31

#### Resources
Models 13Icons 5Skins 7User Interface 3Tutorials 3

We just have to wait for Wolverabid to move it.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

mmmm Guess so ... Btw, what happened to your Goblin Bee ? lol

#### Kwaliti

#### Kwaliti

 Level 37

JoinedJun 7, 2007Messages2,394

Resources: 31

#### Resources
Models 13Icons 5Skins 7User Interface 3Tutorials 3

Meh, Psycho-Stalker-Pig is better ^^

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

lol ... he lok like a ... werrr ... like a pig lol ... Is ugly =S

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved!

It's really good work, Flame_Phoenix! Even though some of the information exists in other tutorials, this one wraps things up nicely.


The only necessary change is the title: Super Icons Tutorial - All About Icons. "Super" is not very descriptive.

PurplePoot said:

 ...Until further notice. (Wolverabid has higher authority, of course, so it's his final decision...


Click to expand...

~ Thread renamed, tutorial approved!

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

YEEEEE thx Wolverabid, it has been a while you know ...

=) i really can't wait unitl people start seeing my tutorial =) I feel so happy about it =) So proud =) lol.

ALso, please have a look, n the Hidden stuff tutorial, as i need to know if my screenshots are ok.

MM, don't we usually get + rep points when a tutorial is approved ? lol

#### kharma

#### kharma

 Level 6

JoinedAug 17, 2007Messages22

Nice tutorial, Flame Pheonix.


I noticed you mention that 24-bit icons are the only format supported. This is, in fact, not entirely true. WC3 also has support for 8-bit paletted blp images and Pitzermike has created a tool to facilitate their creation called BLPaletter. It might be worth a look to mention this somewhere in your tutorial if you wish to improve it further.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

mmm


 kharma said:



 I noticed you mention that 24-bit icons are the only format supported. This is, in fact, not entirely true. WC3 also has support for 8-bit paletted blp images


Click to expand...

Well, warcarft accepts 24-bit and 32-bit color images. I have chosen 24 bit because you have the same quality with less space, and i recommend everyone to use 24 and not 32 bit. As a map maker i am, i know how the size of a map can be important.

About 8bit, there is no point ... Why only have 8 bit images when you can have 24 ??? You can use the double of the colours and the image quality is way better for smooth effects and others.

I will probably not change the tutorial, i don't think i need to do so. But i will give you a try, and i will eventually check your program.

My tutorial has only the programs you need to use to create, the program you indicated me is "an extra" as there is no need to use it when you have W3IR.

Non less i will check it.

EDIT Sry kharma but i will not use the program. To start the program has countless bugs, it doesn't even open properly on my system. Also, it is NOT user friendly and it requires C++ or another programing language, which is beyond the knowledge of most people who work with icons. The program is only usable inside maps also, which does complicate our lives and to end it does not create a safe copy from the icon (it should use save as, and it doesn't). I concluded that this program is to risky to be mentioned on my tutorial and there for, i won't recommend or mention it.

Anyway, thx for your post =) Hope you get a good time becoming an iconing master =)


 Last edited: Oct 7, 2007

#### BlackDoom

#### BlackDoom

 Level 9

JoinedSep 2, 2006Messages264

Resources: 17

#### Resources
Icons 16Tools 1

W3IR does not feature the functionality of BLPalleter. I did only implement support for 24bit images and partial 32bit support as I found it enough for what I needed. 8bit, well... PS usually saves in 24bit - as I set it to - so I did not bother to go through the 8bit space.


Flame Pheonix... 2^24 is not double of 2^8. It's more than its square. You go from 3 color codes injected in 8 bits (thus 2-3 bits per color, if that) up to 8 bits per each color.


I suppose 8bit would bring some size decrease for an icon. I only suggest it if the image has no more than 256 colors. Otherwise, the next step is 24bit, as I am not really sure about the 16bit format - is it supported, or not?


32bit images for icons are a waste of space, as W3V will force add an opaque mask to them before converting them to BLP.


About the BLP format - what standards it has and what color depths it supports - I do not know. I only know that 24bit images work ok on it.


Cheers!

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Hi mate =) good to see you again.

Well, the game only accepst 24-bit and 32-bit colors. Thx to BLPallete it can accept 8-bit colors, but still, it would be a waste of time to save images with 8-bits when you can save them with 24-bits.

After all your comments, you just proved my point - BLPallete is a waste of time lol. Please read my decision... the program didn't pass my user rules.

So, how the new W3IR going ???

#### BlackDoom

#### BlackDoom

 Level 9

JoinedSep 2, 2006Messages264

Resources: 17

#### Resources
Icons 16Tools 1

No, BLPalleter is NO WASTE OF TIME.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

well, i believe oit is ... maybe not dor you (you are an icon expert) but in my point of view, i have no use for it ... It does not apply to my tutorials contest and objective = teach an easy way to make all types of icons.

The program didn't pass my rules, i will not use it.

PS: waste of time can be a very strong term (to much strong maybe) i just say that is has no use for me. lol.

Also, please don't make comments with only 1 line ...

W3IR is 100x better the BLPallete and i decide that is the program i recomend for all users.

#### BlackDoom

#### BlackDoom

 Level 9

JoinedSep 2, 2006Messages264

Resources: 17

#### Resources
Icons 16Tools 1

Mate, W3IR and BLPalleter have nearly nothing in common. This is why I'd say not to compare them.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

lol ..... After using W3IR and BLPallte i probably agree with you .... Still, in my opinion, i have to say that W3IR is was better ... It's like comparing a little mouse to a big elephant ... Ofc the Elephant wins lol

#### 1)ark_NiTe

#### 1)ark_NiTe

Patron

 Level 18

JoinedNov 1, 2006Messages1,612

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

Flame_Phoenix... when do you have time for real life? lol. Not meaning to offend you at all, but you make such large posts that seem like they consume so much time. It's a great post, no kidding. It's wonderful actually. But how do you find this time?

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Well ... If you wanna talk about real life ... Once upon a time i was a great Warcraft Player ... I managed to be the 25 best European player ever ... top 25, i was invited to Italy to make tourneys and stuff like that .... But The problem is that so much time here nearly ruined my real life ... Everything has a price and the price for being the best at warcraft turned to be too much high ... this is i didn't had all that time i was needing.... And today i still don't have it.

Therefore i was forced to leave professional game play and turn my attention to other areas... THW and map making were one of those areas.

Now I have more tme for me and for school, friends and to study (ofc ... now that i am in university i really have no other choice but to study hard lol).

However people in my country say: "When you do something you like, you never get tired". And that is what happens to me.

I like programming and in warcraft i can apply all programming i want and i can learn more then just that.

The key to study and make long posts is actually simple: Time planning.

Each week, when i wake up, i see what i need to so, i go to the calendar and i plan what i have to do, when i have to, until the end of the week.

Time is precious and therefore i try to take as much advantage as i can from it.

This way i can save 30mins to 1hour every day, so i can come here and comment and play ... just to relax, as i am not a machine =).

Also, this super tutorial was made during vacation ... =P I took 3 months to do it with all images and all that stuff =).

Still, thx fr you comment, and don't forget to give rep points and rate my tutorial lol =P.

If you read the tutorial creator's note, you will also see i had some troubles with mods, but nothing that hard work can't solve.

Remember, the key is "time planning".

Besides I like helping people, that is why i create tutorials and help them with maps/camp. It feels it is the right thing to do ... thus knowing that you are helping someone, thus making a better world, well, that has no price for me.


 Last edited: Nov 2, 2007

#### PitzerMike

#### PitzerMike

 Level 8

JoinedNov 25, 2003Messages208

Resources: 5

#### Resources
Icons 2Tools 3

Oh god, where to start?

Flame_Phoenix said:

 Well, warcarft accepts 24-bit and 32-bit color images. I have chosen 24 bit because you have the same quality with less space, and i recommend everyone to use 24 and not 32 bit. As a map maker i am, i know how the size of a map can be important.

About 8bit, there is no point ... Why only have 8 bit images when you can have 24 ???


Click to expand...

You contradict yourself. First you say 24 is better than 32 because it saves space, then you say 8bit has no point, although it saves much more space.

 You can use the double of the colours and the image quality is way better for smooth effects and others.


Click to expand...

No, for a 64x64 image there's no way you'll need more than 256 colors.

 To start the program has countless bugs


Click to expand...

No, you just don't know how to use command line tools.

 ... it doesn't even open properly on my system.


Click to expand...

No, you just don't know how to use command line tools.

 Also, it is NOT user friendly


Click to expand...

No, it's as user friendly as dragging and dropping an image file onto the program is.

 and it requires C++ or another programing language


Click to expand...

Just no.

 The program is only usable inside maps also, which does complicate our lives


Click to expand...

If you mean that the size decrease is only noticeable once the image is imported into an mpq archive, I can only say that BLPs are not used anywhere else.

 and to end it does not create a safe copy from the icon (it should use save as, and it doesn't).


Click to expand...

No, by default it uses the name of the image and replaces the extension with .blp (so it doesn't overwrite the original file). And you're free to specify a custom output file name (once you've learned how to use command line)

 W3IR is 100x better the BLPallete


Click to expand...

No, the two cannot be compared because they do different things.

Ok, i hope i was at least able to show the obvious flaws in your reasoning.

I coud go on with the list if you will.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

PitzerMike said:

 You contradict yourself. First you say 24 is better than 32 because it saves space, then you say 8bit has no point, although it saves much more space.


Click to expand...

I don't contradict myself. You see, if you use 8 bits images you will have less colours to use. It's from 2^24 to 2^8. Memory is important, but so are the colours for your icon. Therefore 24 images is the perfect measure because it allows you to have a nice set of colours to use, and it doesn't take that much space.

 No, for a 64x64 image there's no way you'll need more than 256 colors.


Click to expand...

You don't use it, but there may be people outside there that may do. You should remember that.

 No, you just don't know how to use command line tools.


Click to expand...

and

 No, it's as user friendly as dragging and dropping an image file onto the program is.


Click to expand...

LOL, now you contradict yourself.

This program requires specific knowledge about a specific area that many people don't have. Therefore it is NOT user friendly for all of it's users. It is a restricted program for restrict people with a level of knowledge.

 If you mean that the size decrease is only noticeable once the image is imported into an mpq archive, I can only say that BLPs are not used anywhere else.


Click to expand...

Well, when i import my icons, i don't place them in the mpq file. just import them. To do that (in the mpw) i would probably just replace old warcraft icons, which is bad. Besides there is no need for doing so.

 No, by default it uses the name of the image and replaces the extension with .blp (so it doesn't overwrite the original file). And you're free to specify a custom output file name (once you've learned how to use command line)


Click to expand...

See above comments. Also, the program should bring some kind of tutorial, like W3IR has.

Sry, there is no way i will ever approve that Program, unless a better version is released.

 No, the two cannot be compared because they do different things.


Click to expand...

They both are icon and image programs .... they are not that different. However the quality is different.

 Ok, i hope i was at least able to show the obvious flaws in your reasoning.

I coud go on with the list if you will.


Click to expand...

Thx for reading my posts. However, the real work here is the tutorial. I will not change my decision on not using that program, it doesn't pass my requirements.

And, no, you don't have to write anything else, unless you really want to.

Still, if you find more flaws, find them in the 72 pages long tutorial i've created.

Finally, to end, remember to rate the tutorial and give rep+ points !! yee lol

Thx for your post.


 Last edited: Nov 4, 2007

#### PitzerMike

#### PitzerMike

 Level 8

JoinedNov 25, 2003Messages208

Resources: 5

#### Resources
Icons 2Tools 3

I can accept most of the points you bring up, but:

Flame_Phoenix said:

 Well, when i import my icons, i don't place them in the mpq file. just import them. To do that (in the mpw) i would probably just replace old warcraft icons, which is bad. Besides there is no need for doing so.


Click to expand...

You see, maps ARE mpq archives. Just that they don't have the file extension .mpq but .w3x/.w3m, but nonetheless they are mpq archives. That's why the compression works when you simply import an icon using the import manager like everyone does it.

You'll uderstand that I'm finding it offensive when you're talking about "countless bugs" when you haven't even used the tool.

I mean, how coud you tell that "it does not create a safe copy from the icon" when "it doesn't even open properly on your system"?

That's like if I rated your tutorial, although I havn't read it.

If you had left it at "The documentation isn't good enough, I don't know how to use it, and this is why for me it is not useful", that would have been the proper way to put it, especially in a tutorial which should normally be based on "scientific" reasoning, not guesses.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

 You see, maps ARE mpq archives. Just that they don't have the file extension .mpq but .w3x/.w3m, but nonetheless they are mpq archives. That's why the compression works when you simply import an icon using the import manager like everyone does it.


Click to expand...

MMmm my mistake then, i thought that the units/images etc in the CD-ROM were the mpq files ... And that what we create is not ... anyway, that is not important for now.

 You'll understand that I'm finding it offensive when you're talking about "countless bugs" when you haven't even used the tool.

I mean, how could you tell that "it does not create a safe copy from the icon" when "it doesn't even open properly on your system"?


Click to expand...

Arrghhh ... When i tested the program in my computer i had a critical error for God sake !!!! and every time i test it, i have that same error !!! Isn't that a bug !?

Maybe it is i who is doing the things in the wrong way ... dunno... but that seems like a bug to me...

 If you had left it at "The documentation isn't good enough, I don't know how to use it, and this is why for me it is not useful", that would have been the proper way to put it, especially in a tutorial which should normally be based on "scientific" reasoning, not guesses.


Click to expand...

If i guess things i won't be able to explain those things to people ... and if you actually read my tutorial you will see that i explain everything in a very detailed more to every1, thus meaning that i understand what i say ... thus meaning that i don't guess things... it means that i actually test things and that i make an evaluation about them ... You just can't explain something which you just don't understand and that you just guessed...

Also, all my comments do not mean to diminish your program. My comments are not meant to say that it's crap. I know how creating a tutorial is hard, i can't imagine how creating a functional program is...

Therefore i apologise if i somehow insulted you or the program you refer to.

If my comments are way to offensive, please say it, so that i can change them.

#### PitzerMike

#### PitzerMike

 Level 8

JoinedNov 25, 2003Messages208

Resources: 5

#### Resources
Icons 2Tools 3

Flame_Phoenix said:

 When i tested the program in my computer i had a critical error for God sake !!!! and every time i test it, i have that same error !!! Isn't that a bug !?


Maybe it is i who is doing the things in the wrong way ... dunno... but that seems like a bug to me...


Click to expand...


I see, bug reports are always welcome.


I mean, write down what the error message said, your system specifications and what steps you followed to get the error (maybe also include the image that you used).


That way the program can be improved and possible bugs can be resolved, which will help us all.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Mmmm, i deleted the program ... i will have to download it again ... besides i am not sure if that error was due my own computer ... as i said "dunno"... But is weird that happening all the time. Still i will see what i can do and i may post soon the bug report.

#### BlackDoom

#### BlackDoom

 Level 9

JoinedSep 2, 2006Messages264

Resources: 17

#### Resources
Icons 16Tools 1

Mate, and I hereby mean Flame, you should stop for a moment and check for yourself where this chat is going to.


On the one hand, Pitzer here has at least a clue concerning what he's talking about.


On the other hand, you have a clue or two about the tutorial you are trying to make.


Forgive me, but simply being an outstanding user of a software product does not immediately license you to the degree of a great developer. As cold and as cruel as it may sound, the simple fact we play W3 does not make us good custom resource authors. Period.


If you say that BLPalleter launches an error on your system, have you tried to make it go away??? Have you tried to send some feedback to Pitzer? Well... NO.


So please, as much as it would tickle my ego about W3IR, give us all a break and stop claiming W3IR is so damn better than BLPalleter. Ok? Please. Pretty one, too.


Then, try to diminish the amount of inflexibility in your discussion. Again, please. For you ain't gonna to too far this way. You'll only end up loosing Pitzer's support. I am not suggesting you to become some kind of *** licker, but consider the options.


I personally do not completely agree to your tutorial. I prefer to let people decide how to decide on their own, that's why I only got as far as borders go in icon creation, for others. I do my own work.


Well... good luck. And try fixing that error, would you? Then, at least, you'll be able to say that you did not like it as it was too... I don't know... Not Flame...

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

This is my tutorial and therefore i have the obligation to defend it ... I could give a good reason to prove most of your comments are not truth Blckdoom, but i won't bother.

I am not just a player, i was once that, but i am much more now.

Anyway back to the point, i know how creating a program is difficult, and so i admire all those who do it freely for the well of other people.

Still I confess my ignorance as i didn't know Pitzer was the creator of that program(well, i knew, but i was NOT sure... hadn't time to make a better research about him; my fault) Still, i have already apologised him and i am sorry if some how my comments contributed to take away his idea of making such a project.

projects are difficult to make and many times we don't have the feedback we need to continue.

If we are strong in admitting we did a mistake, then we are even stronger if we continue on a project with no feedback.

I know how difficult it was for me to do this tutorial alone, and some how i feel ashamed that i did not contributed to help Pitzer, after all, we was just trying to be nice to me.

That's why i asked my apologies to him.

Still back to the other point, I actually don't have time to it. I will do it after i format my computer. As i said, the error could be in my system, but i am not sure ... It never happened before.

My comments also don't have the objective of offending and/or pushing back support from people.


 Last edited: Nov 5, 2007

 You must log in or register to reply here.


### Similar threads

S

 Button Manager Tutorial (Custom Icons)


sentrywiz

Sep 27, 2014

2D Art Tutorials

Replies11Views11K

Jun 22, 2015
PurgeandFire

 How to Make an Icon


Tiki

Jul 24, 2004

2D Art Tutorials

Replies1Views29K

Sep 2, 2019
Null

 How to Customize Icons Borders


Cpt.ZnacK

May 12, 2010

2D Art Tutorials

Replies6Views12K

Feb 14, 2022
Kakerate

 Important things about icons, my style - an icon tutorial


Palaslayer

Aug 9, 2009

2D Art Tutorials

Replies6Views7K

Aug 10, 2009
Rmx

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