---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/my-icon-is-green-in-game-why.13372/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# My Icon Is Green in Game Why
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/my-icon-is-green-in-game-why.13372/>
#### ElfWarfare

#### ElfWarfare

 Level 12

JoinedAug 29, 2004Messages618

Resources: 33

#### Resources
Models 2Icons 22Skins 5Maps 2Tutorials 2

I've seen this question asked many times and wanted to offer some possible solutions.

 Difficulty Level


Very Easy - 2/10

 Toolbox


 World Editor

Warcraft III Viewer

 The green square of death:


 Solution 1.


Icon Paths:


Icons must have their paths set correctly to be used in Warcraft III maps. Open World Editor's Import Manager and check to make sure you have set the right path for each icon.


We are going to use the path of a sample icon: BTNDragonBreath.

You can find it here.


ReplaceableTextures\CommandButtons\BTNDragonBreath.blp


ReplaceableTextures\CommandButtonsDisabled\DISBTNDragonBreath.blp


ReplaceableTextures\PassiveButtons\PASBTNDragonBreath.blp


ReplaceableTextures\CommandButtonsDisabled\DISPASBTNDragonBreath.blp

 Solution 2.


Alpha Channels: Warcraft III icons must contain a 32 bit alpha channel to display properly in the game.

Open a faulty icon using Warcraft III Viewer.

If the .blp icon file looks good, then it has an alpha channel. If not:

Convert the icon to an image file: Convert Files > BLP -> BMP, TGA or JPG.

Re-convert the image file back into an icon: Convert Files > BMP, TGA or JPG -> BLP.


Confirm that the 32 bit alpha channel is added during this step.

Save the icon and import the file into a map to test it.

 Solution 3.


Last Resort:


If you are certain that an icon is properly pathed and has an alpha channel, try re-downloading the icon from the web: be sure that your computer is correctly configured.


If the icon still does not function, contact its creator and ask him or her to test it again. If it is working, ask them to send you a new copy.


Good luck! Hope this helps.


 Last edited by a moderator: Mar 5, 2008

 You must log in or register to reply here.


### Similar threads

 Importing And Fixing Icons (RoC and TFT)


ElfWarfare

Feb 12, 2005

2D Art Tutorials

Replies0Views11K

Feb 12, 2005
ElfWarfare

Locked

Sticky

 Texturing & Surfacing - Read Before Posting!


Hive Workshop

Dec 5, 2006

Texturing & Surfacing

Replies0Views25K

Dec 5, 2006
Hive Workshop

B

 tutorial: how to fix black/green icons


bee.

Jan 23, 2008

Tutorial Archive

Replies2Views2K

Jan 23, 2008
Wolverabid

 Icons Guide


Ralle

Sep 11, 2006

2D Art Tutorials

Replies0Views37K

Sep 11, 2006
Ralle

Locked

 How to import a Icon


Archian

Apr 15, 2006

Tutorial Archive

Replies25Views7K

Sep 11, 2006
Ralle

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