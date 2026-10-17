---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ingame-cinematics-screen-in-16-9-for-wc3-1-26a-thanks-to-zukmans-dll.277080/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Ingame Cinematics Screen in 16 9 for WC3 1 26a Thanks to Zukmans Dll
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ingame-cinematics-screen-in-16-9-for-wc3-1-26a-thanks-to-zukmans-dll.277080/>
#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

(continued from my Animated 3D portraits in 16/9 tutorial)

First and foremost : *all* the credit for the file below belongs to @ZUKMAN, from D3Scene.

http://www.d3scene.com/forum/members/zukman.html

I only advertise his work here, since it seems not to be widely known (that file exists since february 2014)

Caveat : like the 2 previous ones, this tutorial is a temporary workaround for (somewhat) proper 16/9, until Blizzard offers us true native widescreen support for the entire game.

Goal : to make Blizzard's ingame (and cinematics using the game engine) screen fit for 16/9 resolutions (tested in 1280x768 with XP SP3 and Intel i915 graphics). See attachments below for comparisons.

This works with WC3 1.26a. It does *not* work with WC3 1.27a, because 1.27a now uses D3D9 instead of D3D8.

Also this dll is designed for 16:9 resolutions.

It is not perfect, but I hope it will encourage ZUKMAN, or someone else, to work some more on it.

Hopefully someone might want to work on a new dll, suitable for WC3 1.27a

File used :

- ZUKMAN's D3D8.dll (version 3), available here

d3d8_v3.zip — RGhost — файлообменник

GitHub - ZUKMAN/WC3WIDESCREENFIX: Fixing stretch in warcraft 3 on widescreen resolutions (source)

1) Extract the file D3D8.dll into your installed WC3 *1.26a* directory.

Do *not*, I repeat, do *NOT* try to put this file in your system32 directory

2) Launch the game, and select a 16/9 resolution

(I personally use 1280x768, which is pretty close to 1280x720)

3) (hopefully) Enjoy

A Void already posted pics

Patch 1.27

... but for some reason, he gets strange results compared to mine. Not sure which version of the dll he is using. I personally use version 3, with XP SP3 and an Intel i915 graphics, and for me all is great (see attachments below). Feedback welcome, though I can do nothing to fix bugs.

This dll v3 still has a few flaws :

- if you click on an object that is far away from the center of the screen, you will miss it. A workaround would be to have the object as much as possible in the center of the screen, before clicking on it. Another workaround would be to learn to not click exactly on the object if it far away from the center of the screen. One can get used to it. I played the entire TFT solo campaign that way.

- in order to maintain aspect ratio, the dll makes you lose a bit of image on top and bottom. This is good because it can not be considered a hack giving the player unfair advantage. But there is a drawback : when playing the solo campaign cinematics, sometimes heads get chopped. This is because Blizzard designed its scenery to have full vertical view.

A few links :

http://www.d3scene.com/forum/warcraft-3-hacks/93156-project-warcraft-3-widescreen-fix.html

Warcraft 3 Widescreen fix

(2 ill-fated threads - now, do people really care about true widescreen support in WC3 ?)

#### Attachments

WC3 E2M6 800x600 - Blizzard.png

 234.7 KB

 · Views: 991



WC3 E2M6 1280x768 - Blizzard.png

 388.6 KB

 · Views: 1,020



WC3 E2M6 1280x768 - Cursor Portrait Ingame D3D8_v3.png

 397.1 KB

 · Views: 1,443



WC3 E2M6 1280x768 - Portrait Cinematics D3D8_v3.png

 258.4 KB

 · Views: 1,116




 Last edited: May 11, 2017

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Looks solid.

(1) You should attach the d3d8.dll on the hive (in case rghost's mirror goes down at some point)

(2) You should explain the images a little bit so people know which is which (without having to read the file name).  Also, if you want to put it in the post, you can put it in [IMG][/IMG] tags.

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

People are kindly invited to have a look here:

[RenderEdge] Widescreen Fix

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

pyf said:

 People are kindly invited to have a look here:

[RenderEdge] Widescreen Fix

Click to expand...

This has a widescreen fix for the latest patch.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Going to graveyard this for now since 16:9 is supported in the latest patches.

#### dam3w

#### dam3w

 Level 2

JoinedNov 4, 2020Messages8

Hello, maybe someone have that d3d8.dll (ZUKMAN's D3D8.dll (version 3)? can u share plz.

 You must log in or register to reply here.


### Similar threads

 Animated 3D portraits in 16/9


pyf

Mar 22, 2016

Tutorial Archive

Replies8Views6K

Jun 4, 2018
pyf

S

Locked

 Dll's


Sev

Jul 12, 2006

Off-Topic

Replies1Views2K

Jul 12, 2006
Ramza

Locked

[Trigger]
 Screen Center Text display


Bond009

Jan 11, 2013

Triggers & Scripts

Replies6Views3K

Jan 19, 2013
Cokemonkey11

 3D Opening Screens / Campaign Screens in 16:9


pyf

Mar 21, 2016

Misc/General Mapping Tutorials

Replies10Views7K

May 11, 2017
pyf

Locked

Poll

 Your aspect ratio 4:3 or 16:9 ?


ALEXz407

Jan 29, 2015

Gaias Retaliation ORPG

Replies8Views9K

Jan 31, 2015
kevs926

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