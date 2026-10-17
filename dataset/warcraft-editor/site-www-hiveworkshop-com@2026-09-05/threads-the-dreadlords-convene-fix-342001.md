---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/the-dreadlords-convene-fix.342001/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# The Dreadlords Convene Fix
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/the-dreadlords-convene-fix.342001/>
#### raypack

#### raypack

 Level 31

JoinedDec 3, 2020Messages1,261

Resources: 10

#### Resources
Models 1Maps 8Tutorials 1

I noticed that on certain patches the Dreardlords Convene interulude is bugged, Anetheron and Mephistroth do not appear. I found out why.

Tichondrius is given the command to use "Dark Summoning" ability on them to teleport them to him. But it doesn't work since "Dark Summoning" is an item ability and not a hero ability (it's been given to Tichondrius as a hero ability, since I assume it was a hero ability in patch 1.00 or 1.01 but got changed to an item/ item ability).

All you have to do is go to the object editor of the map and go to "abilities" section and go to items and find "Dark Summoning". Then change it to a hero ability (from an item ability).

This will fix the issue but there is still 1 more thing left to fix. When Tichondrius teleports them they will be "clustered" and not go in the correct places during the interlude. Just disable clustering in the object editor the same way you changed it to a hero ability.

Voila, all fixed.

This is useful if you're editing the campaign for personal use (or not).

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Could be useful, but not really a tutorial.

I wish there was a section for smaller/just useful stuff but there is none for now.

#### raypack

#### raypack

 Level 31

JoinedDec 3, 2020Messages1,261

Resources: 10

#### Resources
Models 1Maps 8Tutorials 1

Okay, no problem. Hopefully this helped someone

#### heyoarthurishere

#### heyoarthurishere

 Level 3

JoinedDec 23, 2024Messages17

raypack said:

 Okay, no problem. Hopefully this helped someone

Click to expand...

Where exactly do I need to put the modified Interlude file? I tried putting in maps/campaign/ but it doesn't seem to be detected. This is on 1.31 with local files already enabled


 Last edited: Jan 6, 2025

 You must log in or register to reply here.


### Similar threads

Locked

 Overhaul Concept - The Dreadlords Convene


Khazmo

Aug 15, 2021

Warcraft 3 Re-Reforged

Replies10Views5K

Jan 23, 2022
Ethenil

 Item makeing REAL


forzamoto

Jul 4, 2008

Tutorial Archive

Replies2Views1K

Jul 4, 2008
Ghan_04

 GUI Equipment System


V2.

Apr 8, 2007

Tutorial Archive

Replies1Views2K

Jul 10, 2007
Mini-Me

 Changeable Inventory


Lancer1111

Jun 5, 2009

Tutorial Archive

Replies27Views5K

Jun 23, 2009
Eleandor

 Editor Help


cool_is_i

Mar 6, 2008

Tutorial Archive

Replies7Views2K

Mar 22, 2008
cool_is_i

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