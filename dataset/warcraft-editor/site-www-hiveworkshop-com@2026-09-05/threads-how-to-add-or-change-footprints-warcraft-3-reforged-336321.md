---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-add-or-change-footprints-warcraft-3-reforged.336321/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Add or Change Footprints Warcraft 3 Reforged
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-add-or-change-footprints-warcraft-3-reforged.336321/>
#### Mr Ogre man

#### Mr Ogre man

HD Model Moderator

 Level 53

JoinedSep 30, 2020Messages894

Resources: 86

#### Resources
Models 2Packs 3Reforged HD Models 79Ported Models 1Tutorials 1

Tutorial level:Beginner

Prerequisites: Basic understanding of Retera Model Studio (RMS). I'm using version 0.04 for this.

So you have created an awesome model out of, say, the batrider's bat rider. Or you have imported a walk animation from another model. Or you had to delete your footprints for reasons unknown.

Now, how would be give this nice fella some foot prints?

1. First of all, make sure that your model has footprints (usually named FPTxFPL0 [leftfoot] and FPTxFPR0 [right foot]). They're found in the nodes subsection. If you don't have them, copy them from a model that has similar feet.

When you have pasted them in the model, go to the walk 1 animation, remember WHERE the feet touch the ground (usually it's on the middle, but not always) and move both nodes to that position in the T-pose (not in the animation tab).

2. When you have set the nodes, save the model as a .mdl

3. Open the .mdl model in RMS, save it again (sometimes it's buggy if you don't) and open the .mdl with notepad. Don't close RMS, you're still going to need it.

You'll see a bunch of letters and numbers. You don't mind them, you just neet to click search (ctrl+F in English computers) and search for FPT.

4. Open the tab of RMS with the model. Go again to walk 1 and find WHEN the model treads on the "floor". In my case, it's in the frame 133100 (FPT Left) and in the frame 133500 (FPT right).

5. Remember your numbers, or write them somewhwere. Remember which node is which.

6. Go to the .mdl. Change the frame numbers of the event track for the numbers that you've picked right before.

7. That's it, save the .mdl. Open it in Retera. Save it as .mdx. Make yourself a nice cup of coffee while you enjoy your creation. Test it ingame. Rage because it doesn't work. Try it again. This time you notice that you deleted a semicolon and now it works. Sip your coffee.


 Last edited: Oct 24, 2021

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I feel like the structure would hurt if the tutorial was longer.

But, since it's short and simple you get away with it, yay!

A tad niche but, that's true for many tutorials.

#### Mr Ogre man

#### Mr Ogre man

HD Model Moderator

 Level 53

JoinedSep 30, 2020Messages894

Resources: 86

#### Resources
Models 2Packs 3Reforged HD Models 79Ported Models 1Tutorials 1

Chaosy said:

 I feel like the structure would hurt if the tutorial was longer.

But, since it's short and simple you get away with it, yay!

A tad niche but, that's true for many tutorials.


Click to expand...

Yeah, I found that I'd have to explain this several times, so I made a foolproof and easily followable tutorial for those interested. It is incredibly niche, but since event nodes usually bug when importing new animations, it's quite a common issue in reforged.

#### Ardenaso

#### Ardenaso

HD Model Reviewer

 Level 35

JoinedJun 22, 2013Messages2,047

Resources: 23

#### Resources
Tools 1Reforged HD Models 19Reforged HD Skins 3

Could you make a Twilac RMS version?

#### Mr Ogre man

#### Mr Ogre man

HD Model Moderator

 Level 53

JoinedSep 30, 2020Messages894

Resources: 86

#### Resources
Models 2Packs 3Reforged HD Models 79Ported Models 1Tutorials 1

Ardenaso said:

 Could you make a Twilac RMS version?

Click to expand...

I guess it's the same approach, I really haven’t tested Twilac's that much

 You must log in or register to reply here.


### Similar threads

 [Guide] Adding Illidan Footprints to HD Models


Firexjkxfire

Apr 16, 2025

Modeling & Animation

Replies0Views425

Apr 16, 2025
Firexjkxfire

Locked

 How do I fix floating footprints?


Urkdrengi

Jul 24, 2004

Modeling & Animation

Replies3Views2K

Jul 25, 2004
Carrington2k

Locked

 Edit Skins for Footprints for Models?


SanKakU

Mar 27, 2009

Texturing & Surfacing

Replies2Views1K

Apr 1, 2009
SanKakU

Locked

 how to save mdl.


FAILCOPTER

Nov 27, 2010

Modeling & Animation

Replies2Views1K

Nov 30, 2010
Deolrin

Locked

 Adding Sound, Footprints, Splats, too custom models.in 3dMAX


Sansui

Jun 27, 2004

Modeling & Animation

Replies0Views1K

Jun 27, 2004
Sansui

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