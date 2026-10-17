---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/geoset-importing-and-exporting-guide.181697/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Geoset Importing and Exporting Guide
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/geoset-importing-and-exporting-guide.181697/>
#### -Peper-

#### -Peper-

 Level 8

JoinedJun 28, 2010Messages3,335

Resources: 16

#### Resources
Models 12Icons 2Packs 1Tutorials 1

Geoset Importing and Exporting Guide


By -Peper-
Requirements:

Vertex Modifier (Optional)

War3 Model Editor

Introduction:

What will we do in this tutorial?

We will learn the basics of geoset exporting and importing using Magos' Model Editor.​
Do we need the vertex modifier tool?

Nah, it's optional. It helps keep the polies low.​

Note:

You should have at least an average knowledge of Magos' Model Editor.

In this tutorial, we will learn the basics of geoset exporting and importing. We will be making a Rifleman with a Footman's head.​

Part 1: Exporting

 First, open the Footman's model using the MPQ Browser.

 Second, rip off his body. This can be simply done by scaling the body part to 0. (NOTE: If you have experience with the Vertex Modifier, use that tool for this part instead.)

 See? Now he is decapitated.

 Next, find the appropriate geoset that contains the head.


To do this easily:

 Open the Geoset Manager.

 Randomly select one Geoset and set the Material ID to [none].

 See if the head disappears, and if it does, then export that geoset. If not, then try another geoset until you find the right one.

Part 2: Importing

 Open the Rifleman model using the MPQ browser.

 Next, scale his head to 0.

 From your MPQ browser, search for Textures/Footman.blp. Right-click it and select "Use As Texture".

 Open the material manager, right-click anywhere and select "Create New".

 A new window will open, and right click inside it again. Select Create new.

 Another window will open again, and set the Texture ID to Textures/Footman.blp. Click OK on everything. Make sure you remember what the name of the newly created material is.

 This is the IMPORTANT PART, bone attaching! Why?! Because if you don't attach it to a bone, it will be invisible in-game.

 Go to the node manager, and search Head-Ref.

 You will see a bone-logo thing (must be a bone), and beside it is CYLINDER03, CYLINDER04, and Object4.

 Remember the most suitable, Object4.

 Go back to the geoset manager, right-click, and select Import.

 Import the Footman head, and when prompted to, select Object4.

 A new geoset has now been created, so double-click it and set the Material ID to that of your created material.

 The Footman's head has now appeared! Move it around to connect it to the body.

 And FINISH! You made yourself a new unit, and now save it!

Important Note:


The Decay Bone animation doesn't display his head, so please view the following tutorial for the solution:

https://www.hiveworkshop.com/forums...2/fixing-faulty-death-decay-animations-40794/


 Last edited by a moderator: Dec 2, 2010

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

When the head's scale is reduced to 0, are the vertices removed or still there? If they are still there, then that might not be the best idea as that will result in unnecessary polygons in the model. So, is there any other way to do it in Magos?


If there is no other way, then that method should be fine. =)

#### -Peper-

#### -Peper-

 Level 8

JoinedJun 28, 2010Messages3,335

Resources: 16

#### Resources
Models 12Icons 2Packs 1Tutorials 1

Sadly, the vertex cannot deleted.


So the size will be pumped.

#### snowman990

#### snowman990

 Level 11

JoinedAug 23, 2010Messages663

Resources: 7

#### Resources
Models 1Icons 3Skins 1Maps 2

hmmm i think i've seen this in youtube...but THANKS ALOT  this is very usefull

#### Devine

#### Devine

 Level 30

JoinedSep 2, 2007Messages3,688

Resources: 10

#### Resources
Models 3Icons 6Tutorials 1

http://www.oinkerwinkle.com/tools/tutorialheadswitch1.html

http://www.oinkerwinkle.com/tools/tutorialheadswitch2.html

Gosh, don't you feel silly now?

#### -Peper-

#### -Peper-

 Level 8

JoinedJun 28, 2010Messages3,335

Resources: 16

#### Resources
Models 12Icons 2Packs 1Tutorials 1

OOooops, i feel silly now...


Btw, the title is "Geoset Export and Import with Magos"

#### -Peper-

#### -Peper-

 Level 8

JoinedJun 28, 2010Messages3,335

Resources: 16

#### Resources
Models 12Icons 2Packs 1Tutorials 1

BUMP~

Anything to add? Complete?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I fixed it up a bit, the grammar anyway. Nice job, ~Approved. Sorry for the long delay.

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

How to add the wood to a peasant-type unit that had his original wood removed? I tried to follow your steps for the shoulder bone but the wood does not appear!  how to?

#### -Peper-

#### -Peper-

 Level 8

JoinedJun 28, 2010Messages3,335

Resources: 16

#### Resources
Models 12Icons 2Packs 1Tutorials 1

Is the bone you are attaching to is really a bone?


I mean in node manager the sign is a blue bone.

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

Well, let's just say this model is a ghoul modified and it had his "wood on shoulder" deleted to delete his worker task(animations are deleted but i fixed that).

Now, i checked the ghoul's bone where supposely the wood was on to and i found on node manager the text "lumber" next to a blue bone icon, right under another blue bone icon named "shoulder2" which was under a helper called "Bone_Arm1_L".

What i did so far:

I created new bones exactly like these ones i just told but.....heck, i'm seeing unequal things now here. :/ see attached images.

I surely thought wrong when i saw both "Bone_Arm1_L". The first is a helper(the original ghoul's) but the other is a bone itself. I feel a bit stuck now.

EDIT: Ok i managed to make it show up but i still can't put it on the shoulder, it only goes in the same space of the left arm. It was invisible because the material ID was missing so i temporarly set it to one of the model ones to see where it is. I guess i have to import peasant.blp to gain a common lumber look, but how? I tried to import peasant.blp in texture manager but there's not a new voice in material ID.

#### Attachments

modeleditorghoulnodes.jpg

 52.7 KB

 · Views: 7,484



modeleditoraliennodes.jpg

 58.1 KB

 · Views: 7,473




 Last edited: Jan 11, 2011

#### -Peper-

#### -Peper-

 Level 8

JoinedJun 28, 2010Messages3,335

Resources: 16

#### Resources
Models 12Icons 2Packs 1Tutorials 1

When you import peasant.blp, make a new material and select the peasant.blp. And reference the geoset to the new material.

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

-Peper- said:

 When you import peasant.blp, make a new material and select the peasant.blp. And reference the geoset to the new material.


Click to expand...

Hahhhhh!! It works!!!!! ^^ +rep! Now the next and last thing. How do i change a model(for expample a spider egg) turning it to a geoset and repeat the same process?

EDIT: i did it, imported geoset, placed material id etc. etc. but the egg bends toward a different point when i play its animation instead of how i place it in the model editor.

EDIT 2: i managed to make it appear properly. Now i only have to see if it shows in game.


 Last edited: Jan 12, 2011

#### alphajethro10

#### alphajethro10

 Level 2

JoinedJan 16, 2011Messages11

hello, i tried this with weapon but the weapon's skin/texture is different. i did the mpq browser to "used as texture"

it is okay if i use other unit model? (not from war3mpq)

or i should use war3mpq unit so the weapon's skin will appear correctly?

#### -Peper-

#### -Peper-

 Level 8

JoinedJun 28, 2010Messages3,335

Resources: 16

#### Resources
Models 12Icons 2Packs 1Tutorials 1

You can use other skins and models (like gun skin if you want to attach one) but you must import the skin too if you will import the model with custom texture.

#### alphajethro10

#### alphajethro10

 Level 2

JoinedJan 16, 2011Messages11

-Peper- said:

 You can use other skins and models (like gun skin if you want to attach one) but you must import the skin too if you will import the model with custom texture.


Click to expand...


when im changing texture the edited model , my portrait can't display or bugged

 but imported with the portrait model. sry for bothering again

#### -Peper-

#### -Peper-

 Level 8

JoinedJun 28, 2010Messages3,335

Resources: 16

#### Resources
Models 12Icons 2Packs 1Tutorials 1

Because that model didn't have an external portrait or camera.

#### alphajethro10

#### alphajethro10

 Level 2

JoinedJan 16, 2011Messages11

-Peper- said:

 Because that model didn't have an external portrait or camera.


Click to expand...


oh i see, i need camera manager. thnx very much. im learning with the magos war model editor

#### Arisaitou

#### Arisaitou

 Level 7

JoinedFeb 24, 2011Messages197

Resources: 6

#### Resources
Models 4Maps 1Spells and Systems 1

this is actually very easy , and for those who doesn't know, read this -peper- tutorial!

It will help you~

and thanks peper, it helped me a bit coz i know how to do it but i didn't know how the heck it works, because of you i know how it works now >.< + REP!!

#### -Peper-

#### -Peper-

 Level 8

JoinedJun 28, 2010Messages3,335

Resources: 16

#### Resources
Models 12Icons 2Packs 1Tutorials 1

And just a suggestion for everyone, use mdlvis instead of vertex modifier. Because vertex modifier often cause models to crash

#### Arisaitou

#### Arisaitou

 Level 7

JoinedFeb 24, 2011Messages197

Resources: 6

#### Resources
Models 4Maps 1Spells and Systems 1

oh yeah, i forgot the rep >.< heres your rep -Peper- ..>.<!!!

#### Arisaitou

#### Arisaitou

 Level 7

JoinedFeb 24, 2011Messages197

Resources: 6

#### Resources
Models 4Maps 1Spells and Systems 1

use mdlvis to delete the unused vertexes


it will be easier to do it in mdlvis


i hope you, peper, make a tut using mdlvis

#### Orcnet

#### Orcnet

 Level 30

JoinedJul 31, 2010Messages5,248

Resources: 5

#### Resources
Maps 4Tutorials 1

just made my day, still viewing it

#### asdfADRiAN

#### asdfADRiAN

 Level 7

JoinedJun 8, 2010Messages283

Resources: 2

#### Resources
Icons 2

I cant believe I didnt see this on the oinker tools website before.

Edit: Any alternative way to physically remove the Vertex rather than scale?

#### -Peper-

#### -Peper-

 Level 8

JoinedJun 28, 2010Messages3,335

Resources: 16

#### Resources
Models 12Icons 2Packs 1Tutorials 1

Using Mdlvis or Oinkerwinkle's vertex modifier

#### mischief123

#### mischief123

 Level 8

JoinedApr 14, 2011Messages191

is it possible to edit the skin of a new model you've made?

#### -Peper-

#### -Peper-

 Level 8

JoinedJun 28, 2010Messages3,335

Resources: 16

#### Resources
Models 12Icons 2Packs 1Tutorials 1

No connection. But you can always edit it, that's how custom skin made.


And skin of every model can be always ever, ever, edited. Note that.

#### -Grendel

#### -Grendel

 Level 49

JoinedJan 20, 2010Messages2,990

Resources: 2

#### Resources
Skins 2

really wonderful tutorial. i owe all of my modeling knowledge on this tutorial

#### SuperFalconButcher

#### SuperFalconButcher

 Level 2

JoinedDec 9, 2011Messages5

Resources: 1

#### Resources
Models 1

Article is good but for me it wasn't helpful^^

#### GrapesOfWath

#### GrapesOfWath

Sound Reviewer

 Level 20

JoinedMar 31, 2012Messages309

Resources: 29

#### Resources
Models 5Skins 1Tools 2Maps 4Sounds 17

Uhhhhhh, could you maybe add a tutorial for weapon changing?

#### T.ReXs

#### T.ReXs

 Level 2

JoinedDec 4, 2011Messages20

-Peper- said:

 The Footman's head has now appeared! Move it around to connect it to the body.

Click to expand...


how to make that? iam newbie

#### -Peper-

#### -Peper-

 Level 8

JoinedJun 28, 2010Messages3,335

Resources: 16

#### Resources
Models 12Icons 2Packs 1Tutorials 1

Select all the footman's head vertices (they will turn green) then select Transformation>Translation in the upper window in Model Editor. Then manipulate the X,Y,Z


X=Front&Back


Y=Left&Right


Z=Up&Down


So you leave X and Y to 0, and set the Z to -5 (or minus whatever) until it connects.

 You must log in or register to reply here.


### Similar threads

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

 Create Models only with free tools


D a w n

Nov 16, 2008

3D Modeling Tutorials
234

Replies174Views87K

Jun 10, 2014
clockwork2

 How to make cool portraits without needing a separate file


Spacebuns

Sep 5, 2014

3D Modeling Tutorials

Replies11Views11K

Nov 27, 2017
Diegoit

 Fixing Faulty Death / Decay Animations


General Frank

Sep 30, 2007

3D Modeling Tutorials

Replies21Views19K

Sep 11, 2021
Liu

S

Locked

 Geoset cutting and importing


soulreaver6

Mar 29, 2009

Modeling & Animation

Replies1Views1K

Mar 30, 2009
Cyb3r

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