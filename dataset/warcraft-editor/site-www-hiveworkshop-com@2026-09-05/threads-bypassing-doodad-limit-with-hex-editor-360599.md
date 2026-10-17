---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/bypassing-doodad-limit-with-hex-editor.360599/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Bypassing Doodad Limit with Hex Editor
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/bypassing-doodad-limit-with-hex-editor.360599/>
#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


Here's a short video tutorial on how to bypass the doodad limit. I'm pretty sure @sotzaii_shuen was the first person to discover it, then we both did some testing to narrow down what specific offsets corresponds with the active bytes that changes the limits.

This will increase the limits of:

Units

Destructibles

Doodads

Items

Unsure whether or not it works with third party compilers.

May be deprecated in the future, if the bytes that corresponds with the values changes.

HEX code to search for:

 Code:

40 00 00 00 D0 FB 01 00 D0 FB 01 00 00 08 00 00
00 06 00 00 E0 10 00 00 60 09 00 00 00 04 00 00
60 00 00 00 D0 FB 01 00 D0 FB 01 00 00 08 00 00
00 06 00 00 E0 10 00 00 60 09 00 00 00 04 00 00
80 00 00 00 D0 FB 01 00 D0 FB 01 00 00 08 00 00
00 06 00 00 E0 10 00 00 60 09 00 00 00 04 00 00
A0 00 00 00 D0 FB 01 00 D0 FB 01 00 00 08 00 00
00 06 00 00 E0 10 00 00 60 09 00 00 00 04 00 00
C0 00 00 00 D0 FB 01 00 D0 FB 01 00 00 08 00 00
00 06 00 00 E0 10 00 00 60 09 00 00 00 04 00 00
E0 00 00 00 D0 FB 01 00 D0 FB 01 00 00 08 00 00
00 06 00 00 E0 10 00 00 60 09 00 00 00 04 00 00
00 01 00 00 D0 FB 01 00 D0 FB 01 00 00 08 00 00
00 06 00 00 E0 10 00 00 60 09 00 00 00 04 00 00

HEX code to replace it with:

 Code:

40 00 00 00 FF FF FF 7F FF FF FF 7F FF FF FF 7F
FF FF FF 7F FF FF FF 7F FF FF FF 7F FF FF FF 7F
60 00 00 00 FF FF FF 7F FF FF FF 7F FF FF FF 7F
FF FF FF 7F FF FF FF 7F FF FF FF 7F FF FF FF 7F
80 00 00 00 FF FF FF 7F FF FF FF 7F FF FF FF 7F
FF FF FF 7F FF FF FF 7F FF FF FF 7F FF FF FF 7F
A0 00 00 00 FF FF FF 7F FF FF FF 7F FF FF FF 7F
FF FF FF 7F FF FF FF 7F FF FF FF 7F FF FF FF 7F
C0 00 00 00 FF FF FF 7F FF FF FF 7F FF FF FF 7F
FF FF FF 7F FF FF FF 7F FF FF FF 7F FF FF FF 7F
E0 00 00 00 FF FF FF 7F FF FF FF 7F FF FF FF 7F
FF FF FF 7F FF FF FF 7F FF FF FF 7F FF FF FF 7F
00 01 00 00 FF FF FF 7F FF FF FF 7F FF FF FF 7F
FF FF FF 7F FF FF FF 7F FF FF FF 7F FF FF FF 7F

Requires a HEX editor, for example HxD.


 Last edited: Jul 22, 2025

#### SHBlade

#### SHBlade

 Level 14

JoinedMay 1, 2013Messages227

Resources: 1

#### Resources
Maps 1

##
 Gaias Retaliation ORPG


This is much more promising. I'd be careful tho since if they ever change this it might backfire and lock you out of the map untill a new doodad bypass is found.

#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


SHBlade said:

 This is much more promising. I'd be careful tho since if they ever change this it might backfire and lock you out of the map untill a new doodad bypass is found.


Click to expand...

You won't technically be "locked" out of the map, you just can't save the map unless you reduce the doodad count to 30k.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

If this was a written tutorial I wish it was slimmed down, I think you could reduce the time by 33% by removing mistakes and information that is ultimately throwaway lines.

That said I don't think you care and we've had a very generous attitude towards video tutorials and I don't feel inclined to start a quality revolution.

#### RawHardstyler

#### RawHardstyler

 Level 3

JoinedJul 31, 2010Messages8

Did this on 1.29.2 - it's somewhere else (not the 7th. result) and shifted by 8 bytes but WorldEditArt, WorldEditMisc, SkyModels and war3mapImported\ in the decoded text were good orientation point.

#### Attachments

Screenshot 2025-07-14 010143.png

 20.3 KB

 · Views: 83



#### Xecutor_ShamanX

#### Xecutor_ShamanX

 Level 12

JoinedDec 14, 2016Messages125

Resources: 5

#### Resources
Models 5

Does this work in older versions, like 1.27?

#### RawHardstyler

#### RawHardstyler

 Level 3

JoinedJul 31, 2010Messages8

Xecutor_ShamanX said:

 Does this work in older versions, like 1.27?


Click to expand...

1.27 should be OK with JNGP which have doodad limit bypass included if I'm not mistaken.

 You must log in or register to reply here.


### Similar threads

Locked

[Solved]
 What can anyone tell me about this crash error?


Kakerate

Jan 5, 2016

World Editor Help Zone

Replies4Views15K

Jan 6, 2016
Kakerate

Locked

 War3 Crash (0023:6F497F70)


RatchetNClank

Nov 16, 2010

World Editor Help Zone

Replies5Views2K

Nov 16, 2010
Vunjo

Locked

 Mysterious Crash


Kam

Aug 30, 2010

World Editor Help Zone

Replies15Views3K

Sep 7, 2010
catchaos

Locked

[Trigger]
 Fatal Error Problem!!!!!!!!!!


rnjm

Dec 14, 2010

Triggers & Scripts

Replies5Views2K

Dec 15, 2010
rnjm

C

Locked

 Starcraft 2 Crash Problem


CyberDuelX16

Feb 28, 2010

StarCraft I & II

Replies9Views4K

Mar 2, 2010
CyberDuelX16

C

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