---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-effects-in-magos-advanced-techniques-2.67542/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making Effects in Magos Advanced Techniques 2
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-effects-in-magos-advanced-techniques-2.67542/>
#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

IMPORTANT

Before reading these tutorials you MUST FULLY UNDERSTAND the basics of SFX in Magos and have entirely finished following the previous tutorial on Advanced techniques. I warned you, this is not easy stuff to master and I won't repeat all the basics.

Making SFX in Magos:


Advanced Techniques #2

Nuke base

PART 2: Circular translations (in other words: pretty circles)


I'll assume you've done all the steps to making the nuke in the previous tutorial, so you'd have the smoke/fire column and the smoke/fire cloud (which only moves up in the WE, due to a bug with the "model space" tag in Magos).


We're going to learn how to use the "bezier"-type of translation. Bezier is useful for making arcs of particles, and not just straight lines, like "linear" makes. Hermite also works well, but I prefer using Bezier.


This type of translation will be useful for making a big circle of fire as the base of our nuke. We could just make a large square emmitter, but this is a hell of a lot better... and a hell of a lot more complicated.


First step, make a bone. Right-click on it and select "Edit Node". Right after that, edit the name to something that fits, like "Stream1". We'll be making TWO circling emmitters, because one would move too slowly (looks ugly) and is even MORE complicated.


Click the "translation" button and a new window opens.


It should read:

 Code:

0: { 0, 0, 0 }


Now go to the small list at the bottom that reads "Interpolation Type" (whatever the heck that means). Change the "none" to "Bezier". Instantly the texte will change to:

 Code:

0: { 0, 0, 0 }
 InTan: { 0, 0, 0 }
 OutTan: { 0, 0, 0 }


Copy and paste it so the text looks like this (it's easier to explain when you have a second set of coordinates):

 Code:

0: { 0, 0, 0 }
 InTan: { 0, 0, 0 }
 OutTan: { 0, 0, 0 }
0: { 0, 0, 0 }
 InTan: { 0, 0, 0 }
 OutTan: { 0, 0, 0 }


You're probably wondering what all these coordinates do. Please be patient, and I will try to explain as best as I can (I'm not all that good with this part of SFX, but I can explain).

0: { 0, 0, 0 } -----This is the first set of coordinates, where the bone will be at the start.

InTan: { 0, 0, 0 } ----- This says where the previous summit was. For us, it must have the same values as the previous "OutTan".


 OutTan: { 0, 0, 0 } ----- This says where the arc's summit will be. It will go to that point in between.

0: { 0, 0, 0 } ----- This is the next set of coordinates. Where our bone will arrive at the specified frame.


 InTan: { 0, 0, 0 } ----- This says where the previous summit was. For us, it must have the same values as the previous "OutTan".

OutTan: { 0, 0, 0 }


Here's a diagram.


 Last edited: Aug 1, 2008

#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

i've given enough to give you a good start, and maybe someday I'll get around to finishing it completely.

#### Devine

#### Devine

 Level 30

JoinedSep 2, 2007Messages3,688

Resources: 10

#### Resources
Models 3Icons 6Tutorials 1

The diagram is more effective then the tutorial. Im going to try it .

Next tutorial is about rotating somethin (not make it translate into circles but rotate)

*ahum request ahum*

Now i finally understand this. +rep

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Is this all the tutorial? or is it still in progress? I doubt it can be approved since you stayed in the theory, a tutorial needs both theory and practice, without practical examples, the tutorial has no life and thus fails.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Approved.

#### ChaoticHunter

#### ChaoticHunter

 Level 8

JoinedMar 23, 2013Messages66

Resources: 8

#### Resources
Models 8

I think i love you

 You must log in or register to reply here.


### Similar threads

Locked

 Adding Animated Textures with NOTEPAD ALONE


alfredx_sotn

Apr 30, 2007

3D Modeling Tutorials

Replies0Views16K

Apr 30, 2007
alfredx_sotn

 Making Effects in Magos - Advanced Techniques


WILL THE ALMIGHTY

Feb 23, 2008

3D Modeling Tutorials

Replies14Views10K

Nov 2, 2008
3rgytr54

 Adding of Emitters to your models with Notepad/Wordpad


alfredx_sotn

Jul 8, 2004

3D Modeling Tutorials

Replies0Views25K

Jul 8, 2004
alfredx_sotn

Locked

 Interpreting animation data from an mdl file


gorillabull

Jan 7, 2014

Modeling & Animation

Replies2Views2K

Jan 7, 2014
gorillabull

A

Locked

 Adding particle emitters to portraits


Abattoir

May 28, 2007

Modeling & Animation

Replies1Views2K

May 28, 2007
General Frank

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