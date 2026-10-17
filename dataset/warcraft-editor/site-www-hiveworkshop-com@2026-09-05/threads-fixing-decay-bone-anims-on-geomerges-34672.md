---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/fixing-decay-bone-anims-on-geomerges.34672/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Fixing Decay Bone Anims on Geomerges
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/fixing-decay-bone-anims-on-geomerges.34672/>
#### iristle

#### iristle

 Level 9

JoinedMar 26, 2006Messages202

Resources: 7

#### Resources
Models 3Icons 1Skins 1Tools 1Tutorials 1

Fixing Decay Bone anims on Geomerges


Say you have done a geomerge, but the custom parts keep popping up in decay bone and you don't know how to fix it, I'm here to tell you how to fix this annoying problem and have the decay bone animation only display the geosets you want.


For example, lets take my Dragon Warrior, he was originally bugged with this problem, if your model is in .mdl form, open it in notepad and go to step one.


If it is in .mdx form(though I highly doubt it will) convert the model to an .mdl using Yobguls File Converter.(Dl it at this link http://hiveworkshop.com/resources_new/tools/344/)

1)So you have opened it in notepad, press ctrl+f and type in the find box 'geosetanim' without the quotation marks. Do not use the geosetanim at the beginning of the file, you need to go further down to find the one we are working with.

2)Check all the geoset IDs and keep them in your mind for later.


Select one of the geosetanims that has a geosetID,


Copy it and paste it down below the bottom-most geosetanim.

3)Remember the geosetIDs I said you to remember? Change the geosetID to a new number one above the previous numbers, like 5, 6, or 7. Depending on which geoset you put in first and how many geosets you first had on the unedited model, like the head i put in first so that geosets ID is 5 and that the footman had 4 geosets before I edited it.


You can see the 1s and 0s after the numbers on the geosetanim, tweak with them and check in a model viewer until you find the row that makes the geoset show on the Decay bone anim, set that to 0 so that it doesn't show when the animation plays.


The Decay bone anim on my model was the bottom most one.

4)Redo step 3 until you have covered all the geosets you need to.


Hopefully now your problem has been fixed and your decay bone animation is bug free, if not, pm me.


 Last edited by a moderator: Nov 3, 2012

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved!

Thread moved to 3D Modeling Tutorials.

Status
 Not open for further replies.


### Similar threads

 Geosetanims - fix those randomly appearing corpses!


Horn

Jul 10, 2004

3D Modeling Tutorials

Replies0Views25K

Jul 10, 2004
Horn

 Fixing Faulty Death / Decay Animations


General Frank

Sep 30, 2007

3D Modeling Tutorials

Replies21Views19K

Sep 11, 2021
Liu

Locked

 Notepad: Adding Geosets


Sage Chow

Dec 1, 2010

Modeling & Animation

Replies4Views2K

Dec 3, 2010
NhazUl

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

Locked

 Adding Animated Textures with NOTEPAD ALONE


alfredx_sotn

Apr 30, 2007

3D Modeling Tutorials

Replies0Views16K

Apr 30, 2007
alfredx_sotn

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