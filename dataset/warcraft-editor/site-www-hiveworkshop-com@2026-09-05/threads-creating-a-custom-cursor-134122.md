---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-a-custom-cursor.134122/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating a Custom Cursor
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-a-custom-cursor.134122/>
#### dab

#### dab

 Level 10

JoinedOct 30, 2008Messages413

Resources: 7

#### Resources
Skins 1Maps 1User Interface 2Tutorials 3

#
 Creating a custom cursor

Extracting the files.

Changing the models path.

Skinning the cursor.

Importing it properly, changing the model file of the cursor.

Testing.

Before we get started with any of this, i need you to have 5 things.

WC3 Image Extractor

https://www.hiveworkshop.com/forums/tools-560/war3-model-editor-62876/

https://www.hiveworkshop.com/forums/tools-560/mdlx-converter-62991/

https://www.hiveworkshop.com/forums/tools-560/blp-tga-converter-62972/

Photoshop (Trial version found at: http://www.adobe.com/downloads/?PID=2294914) or something similar.

## 1. Extracting the files

We open Warcraft 3 Image Extractor

###
 ONLY EXPERIENCED USERS:

 You can also jump over this step, and just extract it later in the process via War3 Model Editor)
 make sure the archive(MPQ) file you have is the NORMAL(war3.mpq), and NOT the war3patch.mpq file(or any other for that sake),(else the following things will not be there) and then UI-Cursor-HumanCursor.blp.

Now, save it as a .tga file for later use.

We now need a model to extract(or, this is depending if you only want to replace the skin,

or want to use a custom mdx file too. Option number 2 is preferable, so thats what ill do.),

so we open War3 Model editor, and in there, choose Windows - MPQ Browser.

Now that we have our MPQ Browser, we then press File - War3.mpq.

You now have a Treeview infront of you, press UI - Cursor and then right click on HumanCursor.mdx and export it where ever you want to.

(

###
 JUMP-OVER:

 for those of you who jumped over the step with Warcraft 3 image extractor, you now also extract the file named Humancursor.blp)

##  2. Changing the models path

###
 (Not nessesary, but im guessing you want you .blp file to be named something else than HumanCursor.blp?)

Open Mdlx Converter and convert the HumanCursor.mdx to HumanCursor.mdl.

Now, edit it with any other Text program(i prefer using WordPad) and find the line:

 Code:

Textures 2 {
 Bitmap {
 Image "HumanCursor.blp"

(heres where it is for those who cant find it):

 Code:

// Converted Mon Jul 13 10:19:17 2009
// MdlxConv Version Mar 3 2007.
Version {
 FormatVersion 800,
}
Model "HumanCursor" {
 BlendTime 150,
 MinimumExtent { -0.028030, -0.028166, -0.045400 },
 MaximumExtent { 0.027501, 0.027105, 0.000000 },
}
Sequences 14 {
 Anim "Target" {
 Interval { 0, 33 },
 MinimumExtent { -0.013600, -0.013600, 0.000000 },
 MaximumExtent { 0.013200, 0.013300, 0.000000 },
 BoundsRadius 0.018986,
 }
 Anim "Normal" {
 Interval { 333, 533 },
 MinimumExtent { -0.004000, -0.024100, 0.000000 },
 MaximumExtent { 0.019700, 0.000000, 0.000000 },
 BoundsRadius 0.016900,
 }
 Anim "Select" {
 Interval { 1000, 1500 },
 MinimumExtent { -0.004000, -0.024100, 0.000000 },
 MaximumExtent { 0.019700, 0.000000, 0.000000 },
 BoundsRadius 0.016900,
 }
 Anim "TargetSelect" {
 Interval { 2600, 3133 },
 MinimumExtent { -0.013600, -0.013600, 0.000000 },
 MaximumExtent { 0.013200, 0.013300, 0.000000 },
 BoundsRadius 0.018986,
 }
 Anim "InvalidTarget" {
 Interval { 3800, 3967 },
 MinimumExtent { -0.013600, -0.013600, 0.000000 },
 MaximumExtent { 0.013200, 0.013300, 0.000000 },
 BoundsRadius 0.018986,
 }
 Anim "HoldItem" {
 Interval { 4233, 4400 },
 MinimumExtent { -0.020000, -0.024900, -0.045400 },
 MaximumExtent { 0.020400, 0.008000, 0.000000 },
 BoundsRadius 0.034553,
 }
 Anim "Scroll Left" {
 Interval { 4500, 4700 },
 MinimumExtent { -0.001998, -0.013600, 0.000000 },
 MaximumExtent { 0.024802, 0.013300, 0.000000 },
 BoundsRadius 0.018986,
 }
 Anim "Scroll Right" {
 Interval { 4833, 5033 },
 MinimumExtent { -0.024825, -0.013508, 0.000000 },
 MaximumExtent { 0.001975, 0.013392, 0.000000 },
 BoundsRadius 0.018986,
 }
 Anim "Scroll Up" {
 Interval { 5167, 5367 },
 MinimumExtent { -0.013372, -0.025030, 0.000000 },
 MaximumExtent { 0.013528, 0.001770, 0.000000 },
 BoundsRadius 0.018986,
 }
 Anim "Scroll Down" {
 Interval { 5500, 5700 },
 MinimumExtent { -0.013281, -0.001944, 0.000000 },
 MaximumExtent { 0.013619, 0.024856, 0.000000 },
 BoundsRadius 0.018986,
 }
 Anim "Scroll Up Left" {
 Interval { 5767, 5967 },
 MinimumExtent { -0.010470, -0.026762, 0.000000 },
 MaximumExtent { 0.027501, 0.011209, 0.000000 },
 BoundsRadius 0.018986,
 }
 Anim "Scroll Up Right" {
 Interval { 6100, 6300 },
 MinimumExtent { -0.028030, -0.028166, 0.000000 },
 MaximumExtent { 0.009941, 0.009806, 0.000000 },
 BoundsRadius 0.018986,
 }
 Anim "Scroll Down Left" {
 Interval { 6433, 6633 },
 MinimumExtent { -0.011621, -0.011653, 0.000000 },
 MaximumExtent { 0.026350, 0.026318, 0.000000 },
 BoundsRadius 0.018986,
 }
 Anim "Scroll Down Right" {
 Interval { 6767, 6967 },
 MinimumExtent { -0.026751, -0.010867, 0.000000 },
 MaximumExtent { 0.011220, 0.027105, 0.000000 },
 BoundsRadius 0.018986,
 }
}

[B]Textures 2 {
 Bitmap {
 Image "HumanCursor.blp",
 }[/B]
...

and change it to

 Code:

Textures 2 {
 Bitmap {
 Image "Textures\[b][u](whatever you want)[/u][/b].blp"

Then, use the Mdlx converter again, and convert it to .mdx

##  3. Redoing the cursor skin

Now, we convert the HumanCursor.blp to HumanCursor.tga using the TGA<->BLP converter. Once you've done that, open the HumanCursor.tga in Photoshop. Now, you should have something looking like this:

Next we do is to find our custom cursor. Ill pick this one:

I then place my cursor in photoshop, and cut away everything else around it so it now looks like this:

But! What about the other cursors already there? No problem. We just use the polygon lasso tool to wrap around them, and do a Fill - Color and then just pick the color behind them, and just do that with all the hands(cursors) you wanna get rid of.

###
 Warning! DONT do it on the aimers and the larger arrows!(unless of cause this is your intention)

Then, put your custom cursor down at the place where the other cursors once were(You can also do this BEFORE the other step, to ensure maximum precision

Now, the hand should be replaced by the hand you choose

BUT!(You just thought we was finished with this point dident ya'? )

We still need the transparency to fit to our cursor, else it will look something like this:

Now, to do that, you go into Channels. Here we have RGB, Red, Green, Blue and Alpha.

This time we'll only be doing alpha.

Now, you see those white spots? When you see the shape of them you almost instinctually knows what it is.

Correct! It's the transparency! Now, we need to get the shape of them, so we go into RGB and use the lasso tool to wrap around one of our cursors. Then, back to the alpha channel. Then we drag the selection over a white spot, so WHOLE the selection gets filled with white (If your selection is too big for that, just Copy/Paste until you have a large enough area of white to do the following), and then CTRL+C (Copy) the selected area and CTRL+V (Paste) somewhere random. Now, we overcolor ALL of the white CURSORS! Once again, DO NOT OVERCOLOR THE AIMERS/ARROWS, UNLESS THIS IS YOUR INTENTION! We should now have something like this

Do that with every one of the cursors, until you have something like this

## 4. Importing it correct, changing the path of the <yourname>.blp

Now, you save the HumanCursor.tga to the same as you wrote in the .mdl file(See step 2). That would say, if you wrote

 Code:

Textures 2 {
 Bitmap {
 Image "Textures\MyStupidExampleCursor.blp"

in the .mdl file(which we converted to .mdx), the name of the HumanCursor.tga should be MyStupidExampleCursor.tga, and should be imported to a custom path named "Textures\MyStupidExampleCursor.blp"

Example:

Hey! Wait a second! How did it go from .tga to .blp suddenly? Simple! We used the TGA<->BLP converter(found at this site) to once again redo it to .blp

## 5. Testing

Just run the map and test it! Should look something like this:


 Last edited by a moderator: Jul 28, 2009

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

- Remove the colored text or use #D0F680 instead.


- FFS do you have to attach the whole mdl? Just attach the part where it is and then "..." the rest.


- Link to the tools you're using instead of just saying "found on this site". Also turn that section into a list.


- Use [goto] tags for the table of contents


- Use
 tags for the step-by-step bits. Don't cram everything into one paragraph -- it's hard to follow.


- Attach all of your images to this post. Use


 Last edited: Jul 13, 2009

#### dab

#### dab

 Level 10

JoinedOct 30, 2008Messages413

Resources: 7

#### Resources
Skins 1Maps 1User Interface 2Tutorials 3

Pyritie said:

 - Remove the colored text or use #D0F680 instead.


- FFS do you have to attach the whole mdl? Just attach the part where it is and then "..." the rest.


- Link to the tools you're using instead of just saying "found on this site". Also turn that section into a list.


- Use [goto] tags for the table of contents


- Use
 tags for the step-by-step bits. Don't cram everything into one paragraph -- it's hard to follow.


- Attach all of your images to this post. Use

Click to expand...


 Last edited: Jul 13, 2009

#### Squidle

#### Squidle

 Level 13

JoinedApr 15, 2008Messages852

CTRL + V is Paste.


CTRL + P is to print


Isn't it?

#### Zeron

#### Zeron

 Level 8

JoinedNov 2, 2008Messages206

Resources: 6

#### Resources
Maps 5Tutorials 1

nice TUT dab(born 2 years after me apparently) i didnt know this.... i thot u were stuck with the ugly as @$$ bloody orc hand and such lol.

GJ!

p.s squidel..... yes,well about past. Idk print i got a print button on keyboard. but thats not for here...

 JASS:

The default settings are:
Copy = C
Paste = V
Cut = X
Print = P
Save = S

Etc.


 Last edited: Jul 14, 2009

#### dab

#### dab

 Level 10

JoinedOct 30, 2008Messages413

Resources: 7

#### Resources
Skins 1Maps 1User Interface 2Tutorials 3

Well, my username? just some default, i use for sites where i got no information that needs secureness. And yeah, i thought you were stuck with those too, untill i thought "Hm.. wonder if you can skin cursors?" and began researching, which eventually leaded to this.

Feel free to PM me if you dont think your own cursors is pretty enough

EDIT: Nop, my CTRL+P is paste(or... thats how i fixed it once, and dont remember how to undo it)

#### Squidle

#### Squidle

 Level 13

JoinedApr 15, 2008Messages852

The regular settings are:


Copy = C


Paste = V


Cut = X


Print = P


Save = S


Etc.

#### Zeron

#### Zeron

 Level 8

JoinedNov 2, 2008Messages206

Resources: 6

#### Resources
Maps 5Tutorials 1

Oh thought u were born in 96' i was in 94' any ways dabb you kno your avatar? ya ima spruce it up for u to look cooler. ill paste here when im done.... you'll like it.

#### Squidle

#### Squidle

 Level 13

JoinedApr 15, 2008Messages852

Zeron said:

 Oh thought u were born in 96' i was in 94' any ways dabb you no your avatar? ya ima spruce it up for u to look cooler. ill paste here when im done.... you'll like it.


Click to expand...


Not the right place to do it.


And doncha mean "Know" instead of "No"?

#### Zeron

#### Zeron

 Level 8

JoinedNov 2, 2008Messages206

Resources: 6

#### Resources
Maps 5Tutorials 1

dude its a post not a THREAD STARTER so it doesnt realy matter but i changed it for u

#### dab

#### dab

 Level 10

JoinedOct 30, 2008Messages413

Resources: 7

#### Resources
Skins 1Maps 1User Interface 2Tutorials 3

Nah, i know how to change my avatar if i'd like ^^. But thnx for trying

#### Zeron

#### Zeron

 Level 8

JoinedNov 2, 2008Messages206

Resources: 6

#### Resources
Maps 5Tutorials 1

oh well i just fixed it so im just gona post it...dont wanna waste...

#### dab

#### dab

 Level 10

JoinedOct 30, 2008Messages413

Resources: 7

#### Resources
Skins 1Maps 1User Interface 2Tutorials 3

BUMB* Re-review it plz


BUMP* Pyritie...


 Last edited by a moderator: Jul 25, 2009

#### bboy-tiger-

#### bboy-tiger-

 Level 12

JoinedJul 28, 2007Messages920

Resources: 2

#### Resources
Maps 2

Dab, can you change some functions by editing model ? Like disable cursor to change color on enemy hover ?

#### dab

#### dab

 Level 10

JoinedOct 30, 2008Messages413

Resources: 7

#### Resources
Skins 1Maps 1User Interface 2Tutorials 3

Why shouldnt i could?


Bump*


Bump*


 Last edited by a moderator: Jul 25, 2009

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

quit bumping this, I was in cambodia ffs

#### bboy-tiger-

#### bboy-tiger-

 Level 12

JoinedJul 28, 2007Messages920

Resources: 2

#### Resources
Maps 2

Comn I think this is a good tutorial.

#### dab

#### dab

 Level 10

JoinedOct 30, 2008Messages413

Resources: 7

#### Resources
Skins 1Maps 1User Interface 2Tutorials 3

Erh? Your back, but still doesn't look at it?


Pyritie?? Would you mind re-modding it?


 Last edited by a moderator: Jul 28, 2009

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

yeah yeah whatever

#### Dreadnought[dA]

#### Dreadnought[dA]

 Level 12

JoinedFeb 23, 2007Messages1,030

Resources: 1

#### Resources
Tutorials 1

DaBs was victorious.

#### tdo_le_s4to

#### tdo_le_s4to

 Level 3

JoinedOct 9, 2023Messages19

i Didn't manage to understand it

 You must log in or register to reply here.


### Similar threads

M

Locked

 Attachment problems


Magee93

Jan 3, 2010

Requests

Replies2Views2K

Jan 3, 2010
Magee93

M

Locked

 mdl editing add animations and tag


Troll-Brain

Sep 13, 2012

Modeling & Animation

Replies12Views6K

Sep 15, 2012
Troll-Brain

V

Locked

 Arthas Stand Ready


VoidNeo

Jan 7, 2007

Modeling & Animation

Replies6Views2K

Jan 7, 2007
Kimbo

Locked

 Anims not working properly, ergh


tee.dubs

Jul 16, 2009

Modeling & Animation

Replies3Views2K

Jul 17, 2009
tee.dubs

D

Locked

 Multiple texture problem.


Deleted member 177737

Jan 9, 2010

Modeling & Animation

Replies2Views1K

Jan 9, 2010
Deleted member 177737

D

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