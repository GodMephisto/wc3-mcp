---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/animated-3d-portraits-in-16-9.277063/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Animated 3D Portraits in 16 9
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/animated-3d-portraits-in-16-9.277063/>
#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

(continued from my https://www.hiveworkshop.com/forums/tutorial-submission-283/3d-opening-screens-campaign-screens-16-9-a-277047/ tutorial)

Caveat : like the previous one, this tutorial is a temporary workaround for proper 16/9, until Blizzard offers us true native widescreen support for the entire game.

Goal : to make Blizzard's animated 3D portraits fit for 16/9 resolutions (tested in 1280x768). See attachments below for comparisons.

This works with any WC3 version. It is not perfect, but I hope it will encourage people to discover perfect values for 16/9 and 16/10 aspect ratios. I also hope someone wil develop someday a program to automate the many steps described below.

Tools used :

- Ladik's MPQ Editor

http://www.zezula.net/en/mpq/download.html

- War3 Model Editor 1.07, by Magos

http://home.magosx.com/index.php?topic=6.0

1) Extract the portrait files using Ladik's MPQ Editor. This part is easy, because all portrait files are named ****_portrait.mdx

There are portrait files in war3.mpq, war3x.mpq and war3patch.mpq

Extract the portraits in war3patch.mpq last, since they are improvements/bugfixes.

So, load each MPQ in Ladik's MPQ Editor

Go to Tools/Search Files...

Search for *_portrait.mdx

Select all the files found

Do (rightclick)/Extract...

Save the selected found files in your installed WC3 directory, preserving directory structure

2) Using War3 Model Editor 1.07, load a portrait file

Go to Windows/Model Editor

Do Selection/Select All (=> red points will go green)

Do Transformations/Scaling

Select 'Use Centre of Mass' (this is very important !)

Change the Scaling value on the Y axis to 0.8

Save

3) Repeat step 2 with every portrait. Unfortunately, War3 Model Editor 1.07 does not allow batch processing.

4) When this is done, take a break. You earned that, after all that tedious, repetitive work.

5) Your modified portrait files go into the proper folder structure in your installed WC3 directory, no name must be changed.

6) Finally, if not done already, edit the registry to allow local files

https://www.hiveworkshop.com/forums/attachments/lab-715/144045d1426519868-warcraft-3-hq-allowlocalfiles.zip

7) Launch the Game, and select a 16/9 resolution

(I personnaly use 1280x768, which is pretty close to 1280x720)

8) Enjoy

Maybe a few portraits do not use the standard portrait template (and therefore would need special tweaking) ? Feedback welcome.

Since ratio for 16:9 is between 1.77 and 1.78, the right value for Y scaling should be between 0.75 and 0.8

If you feel like it, you can also modify the four mdx game cursors.

Their files are located in the UI/Cursor folder.

Have fun and experiment with your own values.

#### Attachments

WC3 E2M6 800x600 - Blizzard portrait.png

 75.2 KB

 · Views: 643



WC3 E2M6 1280x768 - Blizzard portrait.png

 127 KB

 · Views: 632



WC3 E2M6 1280x768 - Portrait and cursor tweaked.png

 129.5 KB

 · Views: 555




 Last edited: Mar 22, 2016

#### RED BARON

#### RED BARON

 Level 35

JoinedOct 9, 2006Messages6,392

Resources: 56

#### Resources
Models 43Icons 7Packs 1Skins 3Maps 2

I am fairly impressed by the effort you put into making it appear to be proper widescreen.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Changing Y scaling likely does not produce correct projection unless there is no point of view (the geometry is projected against the screen with the screen representing some physical distance). It looks more correct but is still wrong due to perspectives. You need to do some skewing of the entire mesh so that it is perspectivly correct. This is similar to how optical illusions are some times done.

For a normal projection you will need some sort of square pyramid distortion from the camera, with the further back vertices being moved more than front ones.

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

RED BARON said:

 I am fairly impressed by the effort you put into making it appear to be proper widescreen.


Click to expand...

Thanks for the praise, but it is nothing, really. Think about the Egg of Columbus, I would say. I am also saying (and demonstrating) anybody can do it. And do it better.

All I did was take bits of already existing ideas, knowledge and tools here and there. I then connected them together in a certain way. This is how new inventions happen IRL, btw.

Pre-compensating widescreen stretch is already used in at least one video game, as an unofficial fan-made workaround to avoid 3D model deformation. Also, have you never tweaked you HDTV settings, for proper display of some 4:3 airings ? same basic concept here.

Being a newcomer in WC3 modding, I see things from a fresh, optimistic angle. Therefore, I believe in the impossible. Plus I am a bit of a tweaker. Meaning I bend things until they work the way I want them to, using Superior Techniques and Reasoning.

taking my (bb)cue from this guy

(The Simpsons, S10E19 / Episode no. 222 / Copyright FOX TV Network, and the respective owners)

A few notes for the daring :

The first two 16:9 tutorials are an incentive to *experiment* with the great free tools created by the WC3 modding community. I was unable to find such a tutorial about mdx tweaking using such tools, for such goals. I hope I have shown the way, nothing more.

These tutorials also allow to spot bugs and shortcomings in said great tools, and suggest improvements. For example :

- War3 Model Editor, MDLXconv 1.04 and Geoset Translation 1.04 lack command line support, which would be great for batch conversion.

- Geoset Translation 1.04 has a bug, triggered by some regional settings

- War3 Model Editor has a horrible bug, which makes it unusable for 3D opening screens / campaign screens modifications (weird deformations for some animated objects)

- Geoset Translation 1.04 might be unable to deal properly with complex mdx (like ArthasIllidanFight)

- MDLXconv 1.03 has an issue with omnilights (thankfully fixed in the unofficial v1.04 I use).

The Animated 3D portraits were easy to figure out. Thanks go to Magos, and the "Use Centre of Mass" option, which really saves the day here. Also, it seems Blizzard only uses one template for (maybe) all the portraits. Portraits which can be very easily found and extracted from the MPQs.

Perspective is not an issue here (or it is very hardly noticed). The 0.8 value is more or less arbitrary, and I perfectly understand it is not "pixel perfect". Simply, it was my first try, and I simply kept the results because they fit my own personal needs. Automating the conversion process would allow for more experimentation, because it is a bit of a long and repetitive work (roughly 2 hours).

That said, having proper widescreen ratio 3D portraits in a stretched UI, gives us the benefit of having more of said 3D portrait displayed on screen. So it is worth it imho.

They are currently not perfect, because I know nothing about proper perspective in video games.

Therefore, I simply took a screenshot in 800x600 as a reference, and then tweaked values to death until the resulting ingame view fit my reference image, as much as I could make it do so at the time. Please see "Superior Techniques and Reasoning" shown above.

Since it was my first foray into it, it took a bit of time (weeks !). One has to start somewhere. But it was worth it, too.

You only need to have the right state of mind for these kinds of things, That is, learn from your mistakes and never give up.

So yes, they are not perfect. But I know they can be *perfected*. Since it is basically maths, it is very well possible to deduct proper values from those already existing, and then feed Geoset Translation 1.04 with better ones.

General problems can already be spotted, like the Large Feet syndrome. I believe this can be fixed by changing the view angle. Of course, this is not a problem for Sam Kieth's fans

There are basically three templates for the 3D opening screens / campaign screens :

- the easy one (sometimes with an tweak)

- the difficult one (sometimes with a tweak)

- the total <censored>

Easy ones only require an x value (MainMenu3d, NightElf_Exp)

Easy ones might require an additional tweak (NightElfCampaign3D, Undead3D_Exp)

Adding a movement value allows to move some things around slightly, therefore improving global scenery and creating interesting variations sometimes) - see NightElfCampaign3D

Difficult ones require an Y value and more tweaks(HumanCampaign3D, Orc_Exp). See also UndeadCampaign3D for much more tweaking (because of the tunnel)

Total <censored> are Orc_Exp (because of that damn scroll), and ArthasIllidanFight

Surprisingly, some easy templates might require unexpected, slightly different values for a better aesthetic look and feel. Please see Alliance_Exp for a perfect example. Here, 0.75 gives better visual results than 0.8.

Therefore, while slight variations can be done, it all comes down in the end to aesthetics, balance and beauty. Not only pure maths, though there is a correlation between maths and beauty. Now that *is* interesting for sure.

All credit for the awesomeness of the *widescreen* 3D opening screens / campaign screens goes to *Blizzard*.

And I put emphasis here, because it is my belief Blizzard envisioned and created said 3D screens as widescreen from the very start, during game development. I think they decided to *dumb them down* using 4:3 compatible values for them, for the final retail release of the game (which is all 4:3 in the end, too).

I believe they changed their plans during game development, probably because widescreen monitors were very scarce at the time.

My tweaking may simply *reveal* Blizzard's original widescreen vision of the intro/campaign files, which has always been there in the mdx files from the very start. See how the global scenery fits widescreen. This is *not* my work.

Acknowledging the horrible registry hack floating around with patch 1.25b, was a mistake imho. It only hurt Blizzard's brand. They should and *could* have done much better back then.

Of course, it is never too late to fix that, isn't it Mr Blizzard Sir, mmmh ?

For the curious, I have uploaded pastebins of my screenshot archives for 3D Campaign Screens, both in 7z ans sitx format.

7z compression (2.30 Mo)

sitx compression (1.79 Mo)

sitx is great for compressing jpg in an archive. These files can be uncompressed with the free sitx expander.

http://my.smithmicro.com/stuffit-file-compression-software.html

(any fake email can do)

More on jpg collections compression in the Computer Tech section someday.

Dr Super Good said:

 Changing Y scaling likely does not produce correct projection unless there is no point of view (the geometry is projected against the screen with the screen representing some physical distance). It looks more correct but is still wrong due to perspectives. You need to do some skewing of the entire mesh so that it is perspectively correct. This is similar to how optical illusions are some times done.

For a normal projection you will need some sort of square pyramid distortion from the camera, with the further back vertices being moved more than front ones.


Click to expand...

Thanks for the judicious comments (as always) and clues.

Undead3D_Exp was the most interesting challenge, due to :

- perspective problems (Arthas' arm)

- polygon issues (floating cape)

- global scenery symmetry for aesthetics.

- text readability

I am still not happy with the block of ice on the middle-right (white on white is not that great for readability). But movement tweaks applied here also allow to hide deficiencies in some 3D elements (the spikes in the background).

UndeadCampaign3D was interesting, too, because of the tunnel at the beginning.

Some deficiencies can not be hidden imho (see MainMenu3d_exp as a blatant example, sadly).

For the torchlight in TutorialCampaign3D, there might be hope. or not.

I believe the scroll in Orc_Exp is simply not fixable at all. At least not without editing the MDX itself, which goes far beyond my poor magician's tricks.


 Last edited: Apr 27, 2016

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Perhaps we should merge this tutorial with the 3D campaign screen one, since their process is so similar. The main difference is the values and the files involved. But it might save some people the trouble of having to search through multiple threads.

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

I would suggest you keep them separate.


- First, it would be a mammoth of a post.


- Second, the tools, reasonings and goals are different imho (even though both tutorials indeed deal with basic, blind tweaking)


- Third, a separate 16:9 portraits tutorial might better interlock with a tentative, future 16:9 interface tutorial (made by someone else, do not get excited folks !). I understand the UI is one of the great challenges in WC3 modding.


- Fourth, others may propose 16:10 (or whatever ratio) tutorials. Therefore, it may be better to keep things separate, thus modular. Might fit into a specific category someday, why not ?


- Fifth, I hope someone will soon post a great tutorial with proper values, so my first two tutorials can be buried in the graveyard. Their goal is simply to show the way, like Zen. I hope they demonstrate anything can be done with a simple idea, free tools, time and efforts (and some knowledge about perspective applied to video games)


- Sixth, I hope Blizzard is reading this


That said, if you believe it is better to merge the threads, you may do so, no problem.

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

People are kindly invited to have a look here:

[RenderEdge] Widescreen Fix

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Going to graveyard this for now since 16:9 is supported in the latest patches.

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

Is this part of the tutorial still relevant with v1.29.2?

 If you feel like it, you can also modify the four mdx game cursors.

Their files are located in the UI/Cursor folder.


Click to expand...

(can also be done for the RoC/TFT logo of the opening 3D animated screen btw, if v1.29.2 does not scale them properly yet)

...but whatever, I understand that THW does not support previous versions of the game.

 You must log in or register to reply here.


### Similar threads

 Ingame / cinematics screen in 16/9 for WC3 *1.26a* (thanks to ZUKMAN's dll)


pyf

Mar 22, 2016

Tutorial Archive

Replies5Views11K

Jun 26, 2021
dam3w

 3D Opening Screens / Campaign Screens in 16:9


pyf

Mar 21, 2016

Misc/General Mapping Tutorials

Replies10Views7K

May 11, 2017
pyf

 War3 Patch Extracting


orcfan32

Sep 18, 2004

Tutorial Archive

Replies0Views9K

Sep 18, 2004
orcfan32

Locked

 How to play Beyond the Dark Portal campaign in Warcraft 3 Classic (pre 1.30)


DC1.26a

Apr 21, 2024

Warcraft Discussion

Replies3Views2K

Jun 1, 2024
EvilJin27

 WarCraft Files (w3m, w3x, mpq..)


Ralle

Jan 3, 2006

Tutorial Archive

Replies1Views27K

Mar 26, 2007
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