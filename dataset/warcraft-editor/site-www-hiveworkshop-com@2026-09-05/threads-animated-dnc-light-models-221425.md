---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/animated-dnc-light-models.221425/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Animated Dnc Light Models
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/animated-dnc-light-models.221425/>
#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Animated DNC Light Models

DNC (DayNightCycle) is a model made to simulate day/night cycle behavior. It occured to me, one day, that I could make one with much more contrast than the original, Blizzard ones, have. I started editing values in Magos' model editor, just to realize that the editor doesn't save those values, but discards them. Then I thought, no biggie, I will just write everything I have in MDL, and then convert it to MDX, using Guesst's MDLX converter. It did save animated light and intensity values, but now I've had to deal with a different problem: all lights were set to Ambient, and in order for this to work, they must be directional.

I've realized that this is a common problem people don't actually know about, but just think that their generated lights simply "don't work". Truth be told, light nodes are immensely simple in Warcraft III, but those two great tools do not cope too well with them.

So, here's what we need to make our DNC model:

Guesst's MDLX converter

Magos' model editor

 ANY text processor (I used notepad here)

 ANY hex editor (I used this free one)

 Possibly Yobgul's converter

 Luck (to deal with the hex editor and don't mess up anything)

 .MDL base for DNC (that I have created myself for the purpose of this

tutorial) View attachment DNCUnit.mdl

 Helper Model (that I have also created so that you can set rotation and position values more easily) View attachment HelperModel.mdx

Step 1: Choosing rotation and position

When you want to create a custom DNC model, you must also think about where you want your light to come from. This is pretty hard to do in .MDL alone, especially if you are not familiar with the way coordinates/rotation works in model files.

First go to "Windows > Animation Controller" and set it to play the "Stand" animation.

Now, open the "HelperModel.mdx" that I have provided, go to "Windows > Node Manager", right-click "LightSource" and then "Edit Node". Here you can open "Translation" or "Rotation". First you should set "Rotation" values.

When you open the "rotation" window you will see some zeros and ones.

0:{0,0,0,1} is what you get. Changing the first value changes the roll of your light while changing the second one changes its pitch. If you want a pure left-to-right light, for example, you will only change the first value. If you want to change the rotation further, change the second one. If you want the light to come from the camera eye, you will have to edit both. Basically, you can leave the third and fourth values intact. When satisfied with the angle, copy values (like 0:{0.3, 0.1, 0, 1} for example) to a blank text file, you will need this later.

Now you need to change the light object's position.

Experiment with 0:{0,0,0} values until you're satisfied with the position of light source. Keep ind mind that Z (the last value) should be around 450, as set by Blizzard, but it is important to mention that the first two values do matter a lot. Since you've set rotation in the helper model, now you should also change the X and Y coordinates so that the light's beam (the spike on your HelperModel.mdx) hits the center. Once you've set the values properly, copy them (in my case 0:{-80,-80,450}) to the text file you opened to paste rotation data.

Step 2: Setting up the actual model file

Open the file DNCUnit.mdl in your text processor and follow the instructions provided there. You will have to create two files, one for units (actually, objects), and the other one for the terrain.

Step 3: MDX Conversion

This step can go two ways, either you'll use Yobgul's converter, or Guesst's. With Yobgul's converter, all you need to do is to convert it (and hope it doesn't give you errors without converting the file) OR you will have to use Guesst's tool, which leads to the hex editing stage. What's the catch, why would anybody use Guesst's if it requires more work? Because Yobgul's converter makes perfect Blizzard conversions, by using Blizzard's native .dll files. However, it requires the retail ones, before any patching has occurred, and they can be found in the war3.mpq archive. Not to mention Yobgul's converter won't convert the file at all if you, for example, put the order of some parameters or tags wrong (which all other converters tolerate) and there's few people I know who have been able to properly run it in 2012, on 64bit windows 7. The knowledge about Yobgul's capabilities largely comes from our resident .mdl master, Rao Dao Zao, so a lot of credits to as far as that is concerned. So there you go, if you can get your hands on Yobgul's converter, make it work, make it actually convert the model, you won't need the next, Hex editing stage. In case you are not very lucky with Yobgul's...

Convert your model to MDX using the Guesst's MDLX converter. Just open the tool, click convert, and find your model file, the .mdx version will be saved in the same folder. Aside from being handier and fixing a lot of stuff for you, Guesst's converter also 'fixes' something it shouldn't...

Step 4: Hex editing

Since Guesst's converter doesn't process light node's type properly, you will have to do so yourself. Not a biggie, though, unless you get lost in the model data. Open up the hex editor, and pay attention to any block that may reassemble this:

When you find it, click on the first character after "yyyy" (or "FF FF FF FF" in hex) on the right. Then, when you see what you've selected on the left, double click it on the left window, this lets you change the value via hex. You have two fields that you must edit, and you need to edit the first one to "01" and the second one to "02".

Once done, you've done half the job. Now, if the "@?" (or "40 3F" in hex) block isn't directly below the "FF FF FF FF" one, do not mind, search for it, keep in mind that it can't be more than two rows farther. Once found, use the same technique to edit values just after the said block to "01" and "00", respectively.

Do this for both models you're supposed to have, the DNCUnit.mdx and the DNCTerrain.mdx one.

Step 5: Import into the map and use

Once everything is set, you can start testing your models.

Just import those two models you made into the map, and use the custom script









 JASS:

call SetDayNightModels("DNC\\DNCTerrain.mdx", "DNC\\DNCUnit.mdx")

 ...assuming you change the strings to your file's locations (and don't forget to replace "\" with "\\"!).

Now you can load the map, play around and see if you need to change something, after reading the tutorial, you will have the knowledge to know what is wrong, so fixing will be easy. Here's the result, as provided by Anvil, since the video has compromised youtube quality, you can instead check the attached map out:

#### Attachments

DNCTest.w3x

 43.2 KB

 · Views: 969




 Last edited: Sep 7, 2012

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

I am sure at least half of the readers will not get anything written here -- it is not your fault. I simply suck at making sense, so if there's something isn't explained well enough, do not hesitate to say it!

#### Pharaoh_

#### Pharaoh_

 Level 40

JoinedNov 6, 2008Messages8,316

Resources: 13

#### Resources
Icons 4Spells and Systems 7User Interface 1Tutorials 1

I have deleted our posts -pretty much a thread hijacking  - and moved the thread to the Warcraft III Tutorials section.

Also, nice tutorial, though I'd appreciate some screenshots that display the difference in the effect, compared to the original DNC lighting.

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Well, the thing is, you can pretty much make the same effect this way. However, you can also make it much different, and since I am working on my project now, screens will have to wait, but I will put screens from my project (because I really utilize the difference between blizzard's lighting and this one).

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Good job on the tutorial overall. It seems fine to me. However, a screenshot of the results would be really nice.

Otherwise, I'll try to test it myself and see what results I get.

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

I'll try it now, light animated are bugged in magos.. and for this, all my DNC are in ambient light and looks ugly


Edit: This works, but if you use another model, all parameters change.


I tried to follow the steps but couldn't find what you have in the pictures, so looking at some values ​​I could change from, ambient to directional, ambient to omnidirectional, etc. +rep


 Last edited: Aug 28, 2012

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Anvil said:

 I'll try it now, light animated are bugged in magos.. and for this, all my DNC are in ambient light and looks ugly


Edit: This works, but if you use another model, all parameters change.


I tried to follow the steps but couldn't find what you have in the pictures, so looking at some values ​​I could change from, ambient to directional, ambient to omnidirectional, etc. +rep


Click to expand...


Yeah, it is mostly that 0 = ambient, 1 = directional, 2 = omnidirectional. The most important thing to change is that one zero to 1, all others, I think just make it work somehow.

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

In my model, change "02" to "00" to go from ambiental to directional.


Take a look, I changed this before "B HC":

#### Attachments

01.jpg

 182.7 KB

 · Views: 2,379



#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Yes, however, sometimes it is not so obvious (it is not always in the same place in hex code), so you have to edit more values (which sucks).

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

I've made a video for those who want a screenshot, this is a custom DNC model made from scratch some time ago. With this tutorial, I optimize my old model converting it from ambient light to directional light. Game speed is 800%, with normal speed the transition is alot better.


Youtube destroyed the quality of the video, maybe at night up the video again with better quality.

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Nice, mind if I include that video in the first post, just to describe what this does?

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

Go ahead, in the night i'll reupload the video again with better quality.

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Well then, in that case, I will update it later

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

Sh*t, I deleted the original video when it was converted to mp4, well, the program did it without my confirmation...


So, i don't have nothing to reupload, attach the map (With de DNC model) that i used in the video for those who want more definition...

#### Attachments

DNCTest.w3x

 43.2 KB

 · Views: 540



#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice contribution. HT, if you link that video in the first post, it'll definitely do justice as a good example. In fact, it is even more appropriate than a screenshot.

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Updated the tutorial, added one more section after Rao Dao Zao's insight on Yobgul's converter.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Finally, someone who explains how to correctly animate the DNC models... now I don't need to make one for each lighting scheme...

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great job, very useful tutorial.

Approved.

#### venger07

#### venger07

 Level 16

JoinedDec 21, 2010Messages592

Resources: 11

#### Resources
Models 2Icons 5Maps 3Spells and Systems 1

##
 Zombie Island Survival


It still lack with step by step instructions.. I dd'nt get the last part on importing the two.. models do you have DNCUnit.mdx and DNCterrain.mdx that is ready to import?.. should I change the path "\" to "\\"? can you please put a specific path so I can just copy paste it... I really want to use this in my map....

EDIT: nvm, I used Anvil's map test and it works! I'll put both of you in credits ^_^


 Last edited: May 13, 2014

#### Cake is a Lie

#### Cake is a Lie

 Level 4

JoinedOct 1, 2017Messages96

How about changing the colors of the DNC?

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Cake is a Lie said:

 How about changing the colors of the DNC?


Click to expand...

Download the attached model file. Half the tutorial is in its comments.

#### Cake is a Lie

#### Cake is a Lie

 Level 4

JoinedOct 1, 2017Messages96

HappyTauren said:

 Download the attached model file. Half the tutorial is in its comments.


Click to expand...

What do you mean?

What attached model file?

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Cake is a Lie said:

 What do you mean?

What attached model file?


Click to expand...

.MDL base for DNC (that I have created myself for the purpose of this

tutorial) View attachment DNCUnit.mdl

#### Cake is a Lie

#### Cake is a Lie

 Level 4

JoinedOct 1, 2017Messages96

Oh ok,thanks by the way...

 You must log in or register to reply here.


### Similar threads

Locked

 mdlx converter buggy


edge[d1]

Aug 15, 2007

Warcraft Editing Tools

Replies4Views3K

Aug 19, 2007
edge[d1]

D

Locked

 New and improved file converter!


Deleted member 126647

Jan 28, 2007

Modeling & Animation

Replies5Views3K

Nov 5, 2007
Wolverabid

Locked

 Fixing Decay Bone anims on Geomerges


iristle

Jun 1, 2007

3D Modeling Tutorials

Replies1Views6K

Jun 27, 2007
Wolverabid

 Basic Animation Transfers


Xarwin

Mar 20, 2007

3D Modeling Tutorials
2

Replies70Views87K

Jan 31, 2024
Vertigo2604

 GMax Exporting Tutorial


AirHead

Aug 22, 2008

3D Modeling Tutorials

Replies10Views14K

Jan 18, 2009
Pomegranate

P

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