---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/adding-sounds-and-attachments.30738/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Adding Sounds and Attachments
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/adding-sounds-and-attachments.30738/>
#### Mechanical Man

#### Mechanical Man

 Level 35

JoinedJan 18, 2005Messages5,781

Resources: 42

#### Resources
Models 32Icons 3Packs 1Maps 5Tutorials 1

 Difficulty:


Easy: 3/10

 Needed Tools


Magos Model Editor

ADDING SOUNDS

Many of the custom models, mainly those from scratch, have no sound attached to it when dying or attacking. You can make the sound via trigger, but this way only one sound can be played at time, so if two buildings with that model die in a very short interval only one explosion will be heard. And triggers should be avoided when it is possible, making more mess and maybe also lag.

The best method to add them is via Magos Model Editor.

So, first open your model you wish to edit. Then select Windows->Node Manager. A new windows opens, right click on blank region (nothing selected) and click Create Event Object.

Double click a newmade event, which should be called UNNAMED. You will have a new window, where you can select the appropriate sound from Data scroll.

After doing that, before you continue, go back to model editor and select Windows->Sequence Manager. Then choose the animation when you want the sound be played. Copy the number in the interval section in From: textbox.

Return to Node Manager, the window of your new object should stay here. Right-click on Event track section and left-click Create New. Then you paste that number in textbox that appears.

The value can be a little greater (especially for attack animation), but it must not exceed the end of animation (To: textbox). Then just click OKs and save your model.

If you do not find any suitable sound, you can select any you will surely will not need and then import one of yours and override the path of sound you have chosen in in model editor (the path of sounds are in sound editor in WE).

ADDING ATTACHMENTS

Some of those models also do not have attachment points, or just not enough. Adding them is very simple. There are two types of attachments. Those which follow certain part of the body (arms, legs), and which stand still (origin).

To create an origin attachment point, go to Node Manager and right-click on blank ground. Select Create Attachment.

Then right-click it and select Edit Node (do not double-click it or select Edit, it has no real purpose here).

After that, in the name textbox name your attachment (how it should be ingame) and left those pivot points all 0 (this is because origin is at the center of the unit).

The process of creating a dynamic attachment is very similar, except you must right-click on an existing bone, you wish the attachment to follow.

Here is Killst4r's Siege Tank which originally has no attachments but I added an origin attachment so I can use my EMP spell to immobilize it


 Last edited: Mar 28, 2007

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

Good tutorial, but you can add the following to the sounds section:

If you want to use a custom sound, it usually needs the following settings:

Rate: 22050 Hz (EDIT: 44100 also seems to work)

Mono

16-bit PCM

(you can convert your sound using Audacity)

There should also be a way to add your own sound strings by editing the .slk files in UI\SoundInfo (from the MPQ)

#### Mechanical Man

#### Mechanical Man

 Level 35

JoinedJan 18, 2005Messages5,781

Resources: 42

#### Resources
Models 32Icons 3Packs 1Maps 5Tutorials 1

Hah, first comment after 7+ years!

The tutorial was aimed to use warcraft sounds. You can replace the sound in Sound editor, but the tutorial didn't cover custom sounds and is limited to use only modeling tools.

#### Sclammerz

#### Sclammerz

Patron

 Level 25

JoinedApr 12, 2014Messages1,893

Resources: 4

#### Resources
Skins 1Maps 3

##
 Legacy of the Third War


Many thanks for this tutorial and the creator.

#### gusanomental

#### gusanomental

 Level 11

JoinedOct 16, 2013Messages244

Resources: 3

#### Resources
Icons 1Maps 2

Thanks for this tutorial. It's starting to take its well-deserved flight after all these years!

#### yram44

#### yram44

 Level 3

JoinedMar 26, 2011Messages44

Haha this tutorial helps me a lot! Thanks for making it @Mechanical Man

#### Ether

#### Ether

 Level 14

JoinedNov 13, 2017Messages733

Resources: 7

#### Resources
Skins 7

Is it possible if I put a custom sound file in the war.mpq without replacing one and use that custom path to a model death. Shall it work?

#### Mechanical Man

#### Mechanical Man

 Level 35

JoinedJan 18, 2005Messages5,781

Resources: 42

#### Resources
Models 32Icons 3Packs 1Maps 5Tutorials 1

I don't think so. But there are a huge number of different sound events and I doubt you need all of them. It may be possible via things like slk files, but I have no idea about it and it's not even modeling related anymore.

#### PerPlumeShake

#### PerPlumeShake

 Level 8

JoinedOct 6, 2022Messages213

was there any sound like "LargeSwordFleshSlice" or any Combat Attack Sound from Model Editor?

#### EvictedGriffin

#### EvictedGriffin

 Level 2

JoinedMar 22, 2024Messages3

Resources: 2

#### Resources
Ported Models 1Sounds 1

As great as this tutorial is, Do you know if i can add a custom sound to this model via blender import the model here, then that sound file be carried over with the death animation?

#### JJhonJames

#### JJhonJames

 Level 6

JoinedApr 15, 2016Messages118

The link you provided doesn't redirect to the same Model Editor used.

This is Magos Model Editor.

 You must log in or register to reply here.


### Similar threads

 Geoset Importing and Exporting Guide


-Peper-

Oct 30, 2010

3D Modeling Tutorials

Replies31Views38K

Apr 14, 2012
-Peper-

 Milkshape Attachment Modeling


HappyTauren

Feb 15, 2008

3D Modeling Tutorials
2

Replies64Views37K

Nov 23, 2018
Amigoltu

 Teamcolor Particles


Anvil

Nov 28, 2012

3D Modeling Tutorials

Replies4Views7K

Dec 19, 2012
Anvil

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

 Model Editing


Fabioz

Jan 18, 2009

3D Modeling Tutorials

Replies45Views66K

Oct 10, 2021
KaiZen7th

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

3D Modeling Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top