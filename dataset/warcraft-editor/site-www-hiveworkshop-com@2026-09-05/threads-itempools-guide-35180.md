---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/itempools-guide.35180/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Itempools Guide
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/itempools-guide.35180/>
#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

This is a tutorial I decided to make, because from what i've experienced, very few people actually know about the itempool natives, and even fewer knows how to use them.


Itempools are an internal system, just like a normal units droptable, you know, the ones where you doubleclick a unit, and sets it's droptable.


Short explanation of the natives:

 JASS:

//Creates an itempool.
native CreateItemPool takes nothing returns itempool

//Destroys an itempool.
native DestroyItemPool takes itempool whichItemPool returns nothing

//Adds an item to an itempool.
native ItemPoolAddItemType takes itempool whichItemPool, integer itemId, real weight returns nothing

//Removes an item from an itempool.
native ItemPoolRemoveItemType takes itempool whichItemPool, integer itemId returns nothing

//Creates a random item from an itempool.
native PlaceRandomItem takes itempool whichItemPool, real x, real y returns item

That's the natives from Common.j.


Here's some examples:


This creates an item pool, with 1/2(50%) chance of dropping item 'I001' and 1/2(50%) chance of dropping item 'I002'.

 JASS:

 local itempool ip = CreateItemPool()
 call ItemPoolAddItemType(ip, 'I001', 1)
 call ItemPoolAddItemType(ip, 'I002', 1)


This creates an item pool, with 1/3(33%) chance of dropping item 'I001' and 2/3(66%) chance of dropping item 'I002'.

 JASS:

 local itempool ip = CreateItemPool()
 call ItemPoolAddItemType(ip, 'I001', 1)
 call ItemPoolAddItemType(ip, 'I002', 2)


This creates an item pool, with 1/5(20%) chance of dropping item 'I001', 2/5(40%) chance of dropping item 'I002' and 2/5(40%) chance of dropping item 'I003'.

 JASS:

 local itempool ip = CreateItemPool()
 call ItemPoolAddItemType(ip, 'I001', 1)
 call ItemPoolAddItemType(ip, 'I002', 2)
 call ItemPoolAddItemType(ip, 'I003', 2)


As you can see it's pretty simple actually, the "weight" determines the chance of how often an item will drop. The easiest way to get the drop chance of an itempool, is to add the weight's together and divide the individual weight. Like the last example there's 1+2+2 which is 5, then 'I001' has 1 as weight, then you just do 1/5 which is 0.20, then you multiply by 100, and you have 20%.


To create an item using an itempool, you just use the PlaceRandomItem native, like this:

 JASS:

 local itempool ip = CreateItemPool()
 call ItemPoolAddItemType(ip, 'I001', 1)
 call ItemPoolAddItemType(ip, 'I002', 2)
 call ItemPoolAddItemType(ip, 'I003', 2)
 call PlaceRandomItem(ip,x,y)

This will place a random item from the itempool created, and return the item created, incase you want to do something with it.


An example of using the returned item would be the following function i use in hero rewards:

 JASS:

function GiveUnitRandomItem takes unit u, itempool ip returns nothing
 local item i = PlaceRandomItem(ip,0,0)
 call UnitAddItem(u,ip)
 set i = null
endfunction


Leaks: You should always remember to destroy itempools that you don't use. And also, remember to null any itempool variabels after you're done using them.


Hope this tutorial helps someone understanding itempools.


 - MindWorX

#### Attachments

itempool_example.w3x

 12.7 KB

 · Views: 669




 Last edited: Jun 13, 2007

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Short but good tutorial. It is descriptive as well. This tutorial actually gave me an understanding of JASS itempools. Gj and +rep!

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Only reason I see this not being approved is that it has no demo map showing how to use it to make a basic system, but next to that this should definatly be approved. Also you should add a warning that they can leak unless you destroy them if you use them only once as a local.

#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

Dr Super Good said:

 Only reason I see this not being approved is that it has no demo map showing how to use it to make a basic system, but next to that this should definatly be approved. Also you should add a warning that they can leak unless you destroy them if you use them only once as a local.


Click to expand...


Done and Done.

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved!

~ Thread moved to JASS/AI Scripts Tutorials.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

One can add a nothing roll to an itempool by using 0 as itemCode in ItemPoolAddItemType(ip, 0, 1).

#### tulee

#### tulee

 Level 31

JoinedJul 26, 2008Messages1,575

Resources: 26

#### Resources
Maps 26

This really helped me with understand what itempools were. I am very surprised they haven't been added to GUI trigger-form yet. Not sure why I only just found out about this native 2 decades late.

 You must log in or register to reply here.


### Similar threads

Locked

[JASS]
 leak check


Josemite

Jan 21, 2008

Triggers & Scripts

Replies8Views2K

Jan 22, 2008
HINDYhat

T

Locked

[JASS]
 [Question] Item pool ?


Taudier

Mar 25, 2005

Triggers & Scripts

Replies0Views1K

Mar 25, 2005
Taudier

T

Locked

 Jass Code item Table


Jan12345

Feb 24, 2020

Triggers & Scripts

Replies3Views910

Feb 24, 2020
FUJI

Locked

[JASS]
 Itempool help


thedragiin

Mar 8, 2009

Triggers & Scripts

Replies3Views1K

Mar 14, 2009
Ciebron

Locked

[General]
 Creating a spawn table and drop table system


DrFuzzyGloves

Jun 22, 2024

World Editor Help Zone

Replies7Views2K

Jun 29, 2024
Uncle

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top