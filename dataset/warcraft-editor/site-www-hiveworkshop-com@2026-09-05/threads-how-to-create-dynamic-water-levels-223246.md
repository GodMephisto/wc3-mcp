---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-create-dynamic-water-levels.223246/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Create Dynamic Water Levels
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-create-dynamic-water-levels.223246/>
#### Pinzu

#### Pinzu

 Level 15

JoinedNov 30, 2007Messages1,202

Resources: 7

#### Resources
Spells and Systems 5JASS 1Tutorials 1

How To Create Multiple Water Levels

Hi. I haven’t seen any tutorials on this technique so I have the privilege to provide you with it. It’s really simple and by the end of the day you will be creating lakes and rivers running down every mountain.

NOTICE:  Before we start you must disable 'Enforce Water Height Limits' located in the advanced menu.

Creating a base terrain with shallow water and a cliff hight of 2.

What it looks like.

Step 2: Copying the water - Create a new map with the size of 64x64, as before use the shallow water base. The tileset doesn’t matter, what does is the cliff height. Previously we used [2], this time we want to use a higher water level so set it on [3]. Once the map is created you select some of the terrain and copy it.

Creating a new map with a different cliff height to copy the water from.

Selecting a area to copy (Ctrl-C).

Step 3: Pasting the water - When you have copied the water it’s time to return to the original map. Paste the water where you want it or in a corner to be used as a palette for later. The cliff that appears under the water can easily be removed by just using the same level tool.

This is how it looks when you paste (Ctrl-V) )it to your map.

To remove the cliff below the water simply use the Same Level tool circled in red.

Step 4: Creating the water palette –Repeat Step 2 and 3 changing the cliff height every time. Once you have pasted over enough water levels you can start terraining. When your completed it will look something like this:

This is how the palette looks when completed. The red numbers is the cliff height of the water. I do not recomend the area of the water being as small as seen in this picture.

Step 5: Terraining - You can now use your previously created palette to paste the water height where ever you want on your map. If you regret any of the water heights simply paste a new one over.

Another trick you can use once you have decided the water height is to expand the area by using the Deep Water or Shallow Water tool on the water and just draging it to where you want to expand the water.

I have selected the Deep Water tool and am holding over as to expand the area.

Once the water has been expanded a hill will be created below it.

To remove the hill simply use the Plateau tool circled in red.

Here is a sample of the process. I decided not to fiddle around with it too much so forgive me for its apearance.

This is how it looks after I have pasted terrain from the water palette.

This is how it looks after the hills have been raised around it.

This is how it looks after I tried to make it resemble a mountain.

[...]

#### Attachments

TutorialStart.jpg

 42.6 KB

 · Views: 4,410



Ashenvale.jpg

 59.2 KB

 · Views: 4,491



TutorialCoppyWater1.jpg

 47.5 KB

 · Views: 4,468



TutorialCoppyWater2.jpg

 32.4 KB

 · Views: 4,411



TutorialPasteWater1.jpg

 63.2 KB

 · Views: 4,465



TutorialCliff2.jpg

 229.6 KB

 · Views: 4,566



TutorialBasicCliff.jpg

 268.8 KB

 · Views: 4,550



TutorialWaterPlaced.jpg

 134.5 KB

 · Views: 4,530



WaterPalette.jpg

 108.6 KB

 · Views: 4,505



TutorialRemoveCliff.jpg

 165 KB

 · Views: 4,653



TutorialDONE.jpg

 317.9 KB

 · Views: 4,819



ExpandingWater1.jpg

 174.2 KB

 · Views: 4,972



ExpandingWater2.jpg

 97.9 KB

 · Views: 4,679



ExpandingWater3.jpg

 147.1 KB

 · Views: 4,638



Notice.jpg

 360.8 KB

 · Views: 3,372




 Last edited: Jul 7, 2014

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Good work. I think this explains it really well and I like the sample screenshots. I can't see anything wrong with it; I think this might actually be a bit more appropriate than the water walls tutorial.

Approved.

#### darwin

#### darwin

 Level 14

JoinedAug 30, 2004Messages909

Resources: 3

#### Resources
Icons 1Maps 2

I have been making maps for 10 years, and I never knew you could cut and paste terrain!

That aside, this is a very nice tutorial. I've been doing a water map and working with this sort of water, and this would have been very helpful. I've been monkeying around raising cliffs, adding water, then lowered the cliffs and raising the terrain. If you had done this a week earlier!

Anyway, good job, and +rep.

EDIT:

This reminds me. On this same map, I need to detect when a boat goes down a level of water. Terrain height appears to give me the height of the ground beneath. GetLocationZ does the same. Is there anyway to detect water height?


 Last edited: Sep 27, 2012

#### Pinzu

#### Pinzu

 Level 15

JoinedNov 30, 2007Messages1,202

Resources: 7

#### Resources
Spells and Systems 5JASS 1Tutorials 1

Thank you for your kind words.

darwin said:

 This reminds me. On this same map, I need to detect when a boat goes down a level of water. Terrain height appears to give me the height of the ground beneath. GetLocationZ does the same. Is there anyway to detect water height?


Click to expand...

Well I'm no scripter but I'm sure there are workarounds to your problem.

1. Check if the unit is on water.

2a. Place invisible platforms under the water or change the ground beneth acordingly?

2b. Create enter and exit regions?

... I don't know if any of this may work for you but atleast I tried.

#### darwin

#### darwin

 Level 14

JoinedAug 30, 2004Messages909

Resources: 3

#### Resources
Icons 1Maps 2

Pinzu said:

 Thank you for your kind words.


Well I'm no scripter but I'm sure there are workarounds to your problem.


1. Check if the unit is on water.


2a. Place invisible platforms under the water or change the ground beneth acordingly?


2b. Create enter and exit regions?


... I don't know if any of this may work for you but atleast I tried.


Click to expand...


The problem is that it needs to be exactly precise, so regions won't work. Changing the terrain below would have worked if I hadn't messed it all up already. Anyway, if anyone knows please post it. If not, I'll work around...maybe I'll shake the camera so the player doesn't see how bad the drop looks. Thanks again for the tutorial.

#### Pinzu

#### Pinzu

 Level 15

JoinedNov 30, 2007Messages1,202

Resources: 7

#### Resources
Spells and Systems 5JASS 1Tutorials 1

Made a small update in the terraining section (Part 5); Added a trick/technique.

#### The_wand_mirror

#### The_wand_mirror

 Level 28

JoinedSep 24, 2006Messages4,986

Resources: 1

#### Resources
Maps 1

Euhm this is what happens when i do it the terrain around it deforms or something there seems to be nothing i can do about it. And i am using a fresh installation of warcraft 3.

#### Attachments

wtfff.png

 1.1 MB

 · Views: 7,357



#### FRENGERS

#### FRENGERS

 Level 6

JoinedAug 4, 2012Messages1,021

Resources: 13

#### Resources
Models 1Icons 10Skins 1Maps 1

@.@ Now i know this @.@ Tq so much

I don't know terrain can be copyed

#### Pinzu

#### Pinzu

 Level 15

JoinedNov 30, 2007Messages1,202

Resources: 7

#### Resources
Spells and Systems 5JASS 1Tutorials 1

The_wand_mirror said:

 Euhm this is what happens when i do it the terrain around it deforms or something there seems to be nothing i can do about it. And i am using a fresh installation of warcraft 3.


Click to expand...

Try removing the cliff under it. (See step 3)

#### Code_Name_XXX

#### Code_Name_XXX

 Level 5

JoinedAug 21, 2012Messages107

Resources: 3

#### Resources
Maps 3

I don't mean to spoil in those whomed successfully do this but this is a, W-O-R-S-T Tutorial. Dood many other users will get stuck at step 3 and a video is pleasing for you to show us step by step /w instructions. Dood if your on to creating a video, I'am waiting.


 Last edited: Dec 27, 2012

#### Pinzu

#### Pinzu

 Level 15

JoinedNov 30, 2007Messages1,202

Resources: 7

#### Resources
Spells and Systems 5JASS 1Tutorials 1

The_wand_mirror said:

 Euhm this is what happens when i do it the terrain around it deforms or something there seems to be nothing i can do about it. And i am using a fresh installation of warcraft 3.


Click to expand...

Hmm, I tried this again when i had freshly installed wc3 roc, it worked perfectly. Then I used my brothers computer and the same thing as seen in this post happend. I honestly don't know how to work around it yet. Or why it happens, maybe someone with more knowledge could answer this.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

You can also modify the .w3e file directly.

#### Pinzu

#### Pinzu

 Level 15

JoinedNov 30, 2007Messages1,202

Resources: 7

#### Resources
Spells and Systems 5JASS 1Tutorials 1

I know now why it doesn't work for some people, its because they have the setting "Enforce Water Height Limits" enabled. To dissable it, you can press ctrl+shift+w or simply advanced menu and click on Enforce Water Height Limits.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

This is nothing...


People who use the editor to do water do not know what they are missing.


There are 16,384 different levels of water. This is a result of it being stored in 14 bits.

#### NyxScream

#### NyxScream

 Level 4

JoinedMar 30, 2013Messages69

Hi, thanks for your teaching on the water.

Not if I could help the 5th part I do not get the sand under water ...

#### Pinzu

#### Pinzu

 Level 15

JoinedNov 30, 2007Messages1,202

Resources: 7

#### Resources
Spells and Systems 5JASS 1Tutorials 1

NyxScream said:

 Hi, thanks for your teaching on the water.


Not if I could help the 5th part I do not get the sand under water ...

Click to expand...


Have you disabled enforced water heighsts? (look at the first hidden tag.)

#### mikeisman2

#### mikeisman2

 Level 8

JoinedMar 29, 2008Messages71

Resources: 29

#### Resources
Icons 29

awesome

#### Thedraes

#### Thedraes

 Level 5

JoinedApr 27, 2014Messages87

Dr Super Good said:

 This is nothing...


People who use the editor to do water do not know what they are missing.


There are 16,384 different levels of water. This is a result of it being stored in 14 bits.


Click to expand...

well, what did YOU use??

#### Paillan

#### Paillan

 Level 20

JoinedJan 22, 2011Messages3,996

Resources: 7

#### Resources
Models 3Maps 3Sounds 1

The Lch King said:

 well, what did YOU use??


Click to expand...


He used this system with copy paste aparently. It's not that big deal.

#### Thedraes

#### Thedraes

 Level 5

JoinedApr 27, 2014Messages87

i see... thanks for the info!

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

 well, what did YOU use??


Click to expand...

I used a custom written Java program. Although I do possess a Java MPQ reader, I do not have a writer. As such I used MPQEdit to manually export the w3e file of a blank map and manually imported the modified one back.


First image of random height water used the standard library RNG of Java on a per tile basis.


Second image took advantage of the Java image framework built into Java SE as part of the Java standard libraries. The framework supports most common file formats and has simple modification capabilities. After scaling the bitmap to the appropriate size I iteratively applied a mapping function to every pixel to translate it into one of the w3e fields.

 He used this system with copy paste aparently. It's not that big deal.


Click to expand...

No I used a custom written program as a "proof of concept". The reality is the editor really should have a tool to modify the water mesh like it has with the ground mesh. The reason why it does not eludes me.


Without a tool, doing what I did on the scale of what I did with the same precision using the "copy" approach would take you years.

#### king arthas123

#### king arthas123

 Level 3

JoinedJun 18, 2014Messages54

Resources: 2

#### Resources
Maps 2

Hey this was a great tutorial thanks for posting it

#### Leubast

#### Leubast

 Level 6

JoinedAug 2, 2009Messages130

Resources: 2

#### Resources
Models 1Tutorials 1

Fast forward to today, this method no longer works in WE version 1.31b (6072) as copy and pasting water crashes the WE.

#### ThomasEddington

#### ThomasEddington

 Level 7

JoinedAug 28, 2019Messages84

Resources: 4

#### Resources
Icons 2Skins 2

An idea: If you want flexibility in your map's overall water level, never create the map with any baseline water level. Using any base water level permanently makes your map with that base water level, and the only way to override water levels to higher cliffs is with pasting. Even if you uncheck the "Enforce Water Level Restrictions" in the menu, you will always have to cut and paste. Instead, create your new map without any water level. Then you can play with the water levels by simply (for example):

1) on a flat plane, use the "raise 1 level" to put a single square of terrain

2) change to shallow water, then overwrite that single square

3) repeat 1 and 2, except with 2, use deep water, and compare.

4) repeat 1 and 2, except use "raise 2 levels", and then repeat 3

5) with this method, you never need to cut and paste, and you can fully modulate without using another map. It's more tedious but you at least aren't Alt-W-1 and 2ing.

#### forelmashi

#### forelmashi

 Level 7

JoinedJan 7, 2010Messages27

Resources: 9

#### Resources
Maps 9

amazingly natural looking waterfall..

 You must log in or register to reply here.


### Similar threads

 Creating Rivers with Rolling Shores


1337D00D

Dec 31, 2006

Misc/General Mapping Tutorials

Replies25Views35K

Jul 17, 2016
Silva

 Terrain: Forest Cliffs


Jattenalle

May 6, 2007

Misc/General Mapping Tutorials

Replies15Views21K

Nov 1, 2008
Dimadelic

 Submerged Units


kellym0

Nov 3, 2012

Misc/General Mapping Tutorials

Replies13Views10K

Jun 4, 2013
kellym0

 [Terrain] Advanced Water Effects


-Kobas-

Jan 18, 2012

Misc/General Mapping Tutorials

Replies17Views23K

May 3, 2023
Mekket

 Playable Terrain: Islands


Belgarath

Sep 6, 2008

Misc/General Mapping Tutorials

Replies14Views14K

May 14, 2009
Belgarath

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top