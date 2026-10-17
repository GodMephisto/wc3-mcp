---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/button-manager-tutorial-custom-icons.257777/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Button Manager Tutorial Custom Icons
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/button-manager-tutorial-custom-icons.257777/>
#### sentrywiz

S

#### sentrywiz

Button Manager Tutorial

This tutorial is meant to ease and improve making icons to use in your WC3 maps.

Instead of manually shrinking images, making them into .blp through other programs,

this program does all that for you. Let's get started then:

Download https://www.hiveworkshop.com/forums/tools-560/button-manager-v1-8-2-a-116280/

Let's take some WoW icons from Wowhead. You can pick whatever, I just picked these three:

So what do we do with them? First download them to a folder, name the folder whatever, I named mine icons_bla:

Next create 4 other folders for our active, passive, passive disabled and active disabled icons.

Note that you can create more, for toggle, toggle disabled and the other smaller icons.

This program has you covered!

Now that we have our folders (note, you can name them whatever

I just like to name them like that for clarity) let's open our program:

Under the program's icon wizard, you have two empty text boxes

that need the 2 path files: one for where the images are and the

other for where should the output icons go to. You can put all

in the same folder, but it will get messy real fast. This way you

can do it without falling into the mud.

So first, get your path file directory. You can do this by clicking at

the top of the folder's location:

And then copy your path file:

Now that we have our path file, insert it into the two empty text boxes.

In the second one, which is our output path, also add "FolderName".

Since our folder name is "Act" we add "Act".

Or if this is too complex for you, click the button next to the textbox

and locate the folder manually. Then from the list of options, select

"Button" for our active icons and click proceed.

And there. Our 3 icons have been converted into .blp and made

into active icons. Now we do the same thing 3 more times, only

we change the folder name and the type of icon we want. For

passive we click Passive, for disabled we click Disabled and for

passive disabled we click Disabled Passive and click the button.

And now we have our icons. That's it. Let's import them now.

Open warcraft editor (I hope I don't have to show you how to do

that through an image) and click on the Import button.

Here is the most boring process of them all: adding all the icons

through the import button. You can hotkey this by clicking CTRL + I

and then navigating to each folder and importing the icons. Let's

assume you're done, your screen should look like this:

Now I am gonna give you the gold formula for icon naming. No, seriously

you will need this everytime you want to add icons to your map. So

either paste these into notepad and save it where you can easily get it.

ReplaceableTextures\CommandButtons\

This is for your active icons.

ReplaceableTextures\PassiveButtons\

This is for your passive icons.

ReplaceableTextures\CommandButtonsDisabled\

This is for both active disabled and passive disabled icons.

So we need these to replace the "war3mapImported" of each icon

with the name representing the icon type. So just change all the icon

names by looking at the names above and replacing the standard

warcraft import name with it. Like this:

Copy the "ReplaceableTextures\CommandButtons\" and then highlight

the "war3mapImported" and hit CTRL + V or paste. And done. Now do

it for every icon.

Whew! When you're done save your map to finalize your new icons.

Now you can add them to abilities, like this:

And that concludes our tutorial. Of course, there are other features in the settings menu of the button manager.

You might find them useful, so tinker with it on your own.

Thanks for reading! Hope you learned how easy it is to make WC3 custom icons.


 Last edited by a moderator: Jun 21, 2015

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

You should structure this tutorial better. Put it in sections. All the huge green text is horrible for reading.

#### sentrywiz

S

#### sentrywiz

deathismyfriend said:

 You should structure this tutorial better. Put it in sections. All the huge green text is horrible for reading.


Click to expand...

I disagree, the green color is great for reading because the background is dark.

EDIT: Changed light green for gold, and this one I like even more!

Plus, I don't have just a huge chunk of text, there are images for nearly every step.

If you follow the tutorial from top to bottom, there's no way you can get lost or confused.

I could add sections, but how do I do that?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice tutorial (I just realized none of the other icon tutorials use ButtonManager). But I would rename it to "Batch Icon Conversion" or something along those lines ("Button Manager Tutorial"? "Converting Images to Icons"?). Also, I would remove the italics since italics is usually used for emphasis. If it is applied to the whole tutorial, then it is useless.

#### sentrywiz

S

#### sentrywiz

PurgeandFire said:

 Nice tutorial (I just realized none of the other icon tutorials use ButtonManager). But I would rename it to "Batch Icon Conversion" or something along those lines ("Button Manager Tutorial"? "Converting Images to Icons"?). Also, I would remove the italics since italics is usually used for emphasis. If it is applied to the whole tutorial, then it is useless.


Click to expand...

I agree on all things.

Italics removed. Renamed. I re-wrote some of it to make it clearer. Change colors as well.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Looks better, and is almost on its way to approval. Why the larger font though? Keep it at the standard font size--if people need bigger text then they'll have "cmd/ctrl +" in their browser.

#### sentrywiz

S

#### sentrywiz

PurgeandFire said:

 Looks better, and is almost on its way to approval. Why the larger font though? Keep it at the standard font size--if people need bigger text then they'll have "cmd/ctrl +" in their browser.


Click to expand...


Just so that it highlights those three lines.


Whenever I come back to wc3 mapping, and I forget the lines I google my own tutorial, scroll down and copy-paste those lines. So it worked for me in that regard. That's why I think they should be a little bigger and in different color.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It's fine to have those paths as a bigger font and color since that is for emphasis. I was referring to the text in the rest of the tutorial. It is all size 4.

#### sentrywiz

S

#### sentrywiz

PurgeandFire said:

 It's fine to have those paths as a bigger font and color since that is for emphasis. I was referring to the text in the rest of the tutorial. It is all size 4.


Click to expand...


Oh lol. Okay, is size 3 what you're wanting?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Just use no sizing, a.k.a size 2.

Size 1

Size 2

Regular text

Size 3

Size 4

But you can feel free to leave headers and emphasized text as larger sizes. Sorry, I know these things are sort of trivial.

#### sentrywiz

S

#### sentrywiz

PurgeandFire said:

 Just use no sizing, a.k.a size 2.

Size 1

Size 2


Regular text

Size 3

Size 4


But you can feel free to leave headers and emphasized text as larger sizes. Sorry, I know these things are sort of trivial.


Click to expand...


Its ok. It looks better with the changes (to my preference)


Edited!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great! Approved.

 You must log in or register to reply here.


### Similar threads

 Complete Icon Tutorial - All About Icons


Flame_Phoenix

Aug 5, 2007

2D Art Tutorials
23

Replies129Views115K

Aug 25, 2020
TheFireMage

 Icons Guide


Ralle

Sep 11, 2006

2D Art Tutorials

Replies0Views37K

Sep 11, 2006
Ralle

 Full Icon Tutorial (Reforged) + ¨tool¨


Barorque

Feb 21, 2020

2D Art Tutorials

Replies39Views26K

Jan 22, 2026
Michael Peppers

 Importing And Fixing Icons (RoC and TFT)


ElfWarfare

Feb 12, 2005

2D Art Tutorials

Replies0Views11K

Feb 12, 2005
ElfWarfare

 Basic - Creating Icons from Screenshots


ILH

May 9, 2017

2D Art Tutorials

Replies4Views6K

Oct 8, 2017
IcemanBo

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