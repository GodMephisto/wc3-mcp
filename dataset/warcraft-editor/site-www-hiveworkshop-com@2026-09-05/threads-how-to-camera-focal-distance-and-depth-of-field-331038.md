---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-camera-focal-distance-and-depth-of-field.331038/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Camera Focal Distance and Depth of Field
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-camera-focal-distance-and-depth-of-field.331038/>
#### Kam

#### Kam

 Level 45

JoinedAug 3, 2004Messages2,912

Resources: 44

#### Resources
Models 10Icons 2Maps 29Reforged HD Models 1Ported Models 1Tutorials 1

Have you wondered what Camera Focal Distance and Depth of Field do? Have you tried to use them without success?

Stay a while, and listen.

What is Depth of Field (DoF)?

DoF is the total area that is in focus, or the distance between the closest object/subject in focus and the furthest object/subject in focus.

What is Focal Distance (FD)?

This is the distance to the target point (focal point). You can think of this as the origin point of where you are aiming.
How do these two relate?

You may read "narrow" or "wide" DoF and wonder what this means. These are terms referring to the scale or total area that will be in focus around the FD point.

Narrow DoF

Click to expand...

Wide DoF

Click to expand...

When I try this the values don't seem to behave as they should. What is wrong?

In many occasions the camera focal point, the blue arrow tip in the World Editor, will not be on the target object/subject. This means you must compensate when defining FD and DoF.

Example focal point on the target

Click to expand...

Example focal point past the target

Click to expand...

In this case you will need to account for the ~448 distance the focal point exceeds the target by.

Now that you have a basic grasp let's look at an example for DoF.

Follow along with this test map
Typing "Reset" will reset all values

Here I am using the example above where the focal point is on the target, 800 units apart

You will notice I start at -300 DoF, and you can see the effect in the lower corners
You can use + or - integers, the effect is identical.

You will see as I continue decreasing DoF the focus narrows

Click to expand...

What about FD?

Imagine a horizontal line across the screen. As you increase FD this line will move.

You can see this as the closest Footman leaves focus and eventually the Farm is almost in focus.

Click to expand...

What if you have 0 for either field?

0 in either field means there will be no effect

Click to expand...

Test map and tutorial request by Tamplier777

#### Attachments

CameraPosition01.png

 2.4 MB

 · Views: 935



CameraPosition02.png

 2.4 MB

 · Views: 905



DoFNarrow.png

 3.7 MB

 · Views: 929



DoFNarrow01.png

 3.2 MB

 · Views: 902



DoFNarrow02.png

 3 MB

 · Views: 889



DoFNarrow03.png

 2.9 MB

 · Views: 861



DoFNarrow05.png

 2.9 MB

 · Views: 845



DoFNarrow06.png

 2.8 MB

 · Views: 688



DoFWide.png

 3.7 MB

 · Views: 916



FD01.png

 2.9 MB

 · Views: 888



FD02.png

 2.9 MB

 · Views: 853



FD03.png

 2.9 MB

 · Views: 824



FD04.png

 2.9 MB

 · Views: 830



NoEffect01.png

 3.5 MB

 · Views: 845



NoEffect02.png

 3.5 MB

 · Views: 869



CameraLensTest.w3x

 98.2 KB

 · Views: 219




 Last edited: Apr 14, 2021

#### Tamplier

#### Tamplier

 Level 48

JoinedSep 7, 2018Messages356

Resources: 94

#### Resources
Models 13Packs 3Skins 2Tools 2Maps 3Reforged HD Models 69Ported Models 1Reforged HD Skins 1

##
 Chronicles of the Second War


THank you so much for the research)

#### Kakerate

#### Kakerate

 Level 14

JoinedOct 18, 2013Messages760

I just noticed these (new?) camera functions the other day, and tried figuring em out with @Saken. Awesome guide!

#### Kam

#### Kam

 Level 45

JoinedAug 3, 2004Messages2,912

Resources: 44

#### Resources
Models 10Icons 2Maps 29Reforged HD Models 1Ported Models 1Tutorials 1

They've been there since Reforged launched. =)

#### Hermit

#### Hermit

 Level 22

JoinedMay 29, 2013Messages1,573

Resources: 8

#### Resources
Models 4Icons 3Sounds 1

Kam said:

 They've been there since Reforged launched. =)


Click to expand...

Cool; are they exclusive to Reforged or do they also work in SD mode?

#### Kam

#### Kam

 Level 45

JoinedAug 3, 2004Messages2,912

Resources: 44

#### Resources
Models 10Icons 2Maps 29Reforged HD Models 1Ported Models 1Tutorials 1

They work in both.


 Last edited: May 3, 2021

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Something needs to be done about the images. Takes way too long to scroll through.

Hidden tags work fine, I think there is also a way to make images smaller until you click on them.

Otherwise good, can see it being used for niche cinematic situations.


 Last edited: Apr 14, 2021

#### Kam

#### Kam

 Level 45

JoinedAug 3, 2004Messages2,912

Resources: 44

#### Resources
Models 10Icons 2Maps 29Reforged HD Models 1Ported Models 1Tutorials 1

Updated!

#### Knecht

#### Knecht

 Level 22

JoinedSep 24, 2017Messages150

Resources: 16

#### Resources
Maps 16

Kam said:

 They work in both.


Click to expand...

Tried the test map in SD and it didn't work - the triggers do change the numbers but it doesn't affect the SD camera.

#### Kakerate

#### Kakerate

 Level 14

JoinedOct 18, 2013Messages760

Is there even a reason for it to not work in SD or is it just a "buy reforged KEK" meme?

#### Kam

#### Kam

 Level 45

JoinedAug 3, 2004Messages2,912

Resources: 44

#### Resources
Models 10Icons 2Maps 29Reforged HD Models 1Ported Models 1Tutorials 1

I stand corrected.

We did not intentionally gate triggers to HD. This is probably tied to the HD renderer out of necessity, or SD integration is broken. Apologies for that.

 You must log in or register to reply here.


### Similar threads

 Cameras


PurgeandFire

Mar 5, 2010

Misc/General Mapping Tutorials

Replies21Views47K

Oct 30, 2018
GrandMaitre

Locked

 Max Camera distance


Drazhar

Jul 1, 2010

World Editor Help Zone

Replies2Views4K

Jul 2, 2010
Drazhar

Locked

 Camera Z Offset Problem


AJalex

May 5, 2010

StarCraft II Modding

Replies0Views2K

May 5, 2010
AJalex

Locked

[General]
 My map ignores Default Camera Distance since 2.0


Ragnarogue

Nov 16, 2024

World Editor Help Zone

Replies5Views2K

Dec 18, 2024
Ragnarogue

Locked

 Need terrainer


Disasterpiece512

Oct 22, 2009

Requests

Replies8Views849

Oct 24, 2009
trolman

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