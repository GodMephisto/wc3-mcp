---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/adding-more-than-11-buildable-structures-to-a-unit.351342/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Adding More Than 11 Buildable Structures to a Unit
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/adding-more-than-11-buildable-structures-to-a-unit.351342/>
#### KitsuneTailsPrower

#### KitsuneTailsPrower

 Level 23

JoinedOct 9, 2019Messages375

Resources: 74

#### Resources
Skins 1Spells and Systems 26Ported Models 44Ported Skins 2Tutorials 1

This isn't really a tutorial. Its more of a collection of my findings on this subject.

1: You can have about 200 buildable structures on a unit. However players can only see the first 11 on the menu. You can order the construction of the others via issue order triggers. If you wish to mass add entries, shift click in the object editor and enter them in the following format without the "s "h000,h001,h002" and so on.

2. Contrary to the information found here: Transformed units cannot construct buildings You CAN have a morphed builder build a unique set of units. You just need to remove the racial build ability before morphing and add it after. e.g.

call UnitRemoveAbility(u,'AUbu')

//bear form trick morph code here

call UnitAddAbility(u,'AUbu')

I have a attached a sample map demonstrating these findings that allows you to build any unit in the base game with one builder.

edit: To have the omni builder build anything: first click the desired unit type from the list, then hold left control and click the point on the map where you want to build.

#### Attachments

omni builder test map.w3m

 195.5 KB

 · Views: 80




 Last edited: Oct 25, 2023

#### Tronuen

#### Tronuen

 Level 3

JoinedMay 10, 2026Messages6

Resources: 1

#### Resources
Maps 1

I can't open this map. Is it corrupted?

#### KitsuneTailsPrower

#### KitsuneTailsPrower

 Level 23

JoinedOct 9, 2019Messages375

Resources: 74

#### Resources
Skins 1Spells and Systems 26Ported Models 44Ported Skins 2Tutorials 1

Tronuen said:

 I can't open this map. Is it corrupted?


Click to expand...

I checked again and it works for me

#### Tronuen

#### Tronuen

 Level 3

JoinedMay 10, 2026Messages6

Resources: 1

#### Resources
Maps 1

While I trying open this map, I get error:"-Level info data missing or invalid."

I use version 1.28.5 of W3. Maybe I can't open because this.

I want create a builder that it has randomly 10 structures from pool. Like at Castle Fight random mode or Legion TD random mode. How can I this?

#### Ravager16829

#### Ravager16829

 Level 40

JoinedMay 14, 2021Messages2,455

Tronuen said:

 While I trying open this map, I get error:"-Level info data missing or invalid."

I use version 1.28.5 of W3. Maybe I can't open because this.

I want create a builder that it has randomly 10 structures from pool. Like at Castle Fight random mode or Legion TD random mode. How can I this?


Click to expand...

I figured the map doesn't run due to the incorrect patch version. The map requires the higher patch (actually, his OP was way back 3 years ago, so that was even before Reforged 2.0). Unless you're using the 3rd party trick like Drake53's Map Adapter, the map won't work on older patches like 1.28.5.

 You must log in or register to reply here.


### Similar threads

S

Locked

 how can Builder/Worker Build more than 11


snipe

Oct 23, 2018

World Editor Help Zone

Replies4Views1K

Oct 27, 2018
00110000

Locked

 Builder Question


tyeeeee1

Jun 3, 2008

World Editor Help Zone

Replies6Views1K

Jun 4, 2008
Borax

Locked

[Trigger]
 Constructing a Building via a blueprint


ColdRocker

Feb 12, 2009

Triggers & Scripts

Replies6Views1K

Feb 12, 2009
ColdRocker

 Issue order to Human worker to assist building (fast-build) a structure


nettles

Feb 12, 2026

World Editor Help Zone

Replies1Views322

Feb 12, 2026
Chaosium

Locked

 Adding Buildings to a builder via Trigger!


JanSze

Aug 1, 2008

World Editor Help Zone

Replies15Views2K

Aug 2, 2008
JanSze

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