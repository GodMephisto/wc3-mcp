---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/object-leak-vs-reference-leak.290363/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Object Leak vs Reference Leak
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/object-leak-vs-reference-leak.290363/>
#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

That's a guide for mid-level of JASSing, yet even profis don't really understand nulling.

Pretty much everyone knows about jass' thing "a leak". Basically, it's a link to an object you don't need to use anymore. Let's say you had to set location for creeps to go into. You gonna do it like that:

 JASS:

local location l = Location(x,y)
if IsLocationVisibleToPlayer(l,GetTriggerPlayer()) then
call IssuePointOrderByIdLoc(GetTriggerUnit(),851297,l)
call RemoveLocation(l)
set l=null

Why do you remove location here? 'Cause you don't need it anymore, you used it once and won't anymore. So you call it's destroyer object RemoveLocation and null the variable.

Let's take different example where you want to register every unit entering specific region

 JASS:

function TriggerRegisterEnterRectSimple takes trigger trig, rect r returns event
 local region rectRegion = CreateRegion()
 call RegionAddRect(rectRegion, r)
 return TriggerRegisterEnterRegion(trig, rectRegion, null)
endfunction

That's default Blizzard's function, and even tho it looks leaky, it's not dangerous at all. Created region will be used constantly, it's the thing which has trigger attached to. If you destroy region, trigger will be gone too. You don't need to null it either, since no matter what, region will stay there whole game, and you don't need to utilize it anyhow.

See the difference?

Leak's definition

Common description of leak is either not nulling, either not destroying of an object you won't use anymore. Thing is, nulling isn't big deal at all.

Whenever you assign an object (handle) to variable, it's inner counter increased by 1. Everytime you null object counter reduced by 1. This counter is used only for inner game's functions. It doesn't prevent game from destroying object if user said so - it rather blocks specific handle ID from recycling. Not a big deal, huh?

That means everytime you not null object which is destroyed, you waste 4 bytes of memory. Game won't re-use those, so will take following 4 bytes. So, in order to waste just 1 MB of RAM you have to make 1048576 leaks (1mln+).

Meanwhile object ARE heavy. They have very spreaded stats, like unit could take few KB with all his structures and inner links, while effect takes only few tens of bytes plus very few sub-calls. So, you actually need to remove objects, not the links. Don't starve for nulling everything, better watch out for things you leftover non-destroyed.

You have, let's say, an ability on the hero. On usage you use triggers to handle, and of course you put GetTriggerUnit() into variable for easier access. If you're sure hero won't be removed from the game, you don't need to null him at all. Same goes for destructibles you're not gonna delete, triggers you're not gonna remove, etc etc.

But can it lead to handle overload?

Well, definitely can. In theory. If you're using save games, probably. Because handles are counted with a 4 bytes unsigned counter, which means, at worst case, you may have up to 2^31 entities max, or 2 147 483 647. Seems enough for a few months of gameplay, isnt it?

Also I find HandleID lock very useful for some.. situatoins. This way you don't need to save 0's or nulls in the hashtables, using HandleID, to make sure there's no invalid data. You don't need to clean those either, in fact, but that's definitely not recommended technique.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Resource can leak object and it's reference, yes. We usually call it "reference leak" if variables is not nulled, and "object leak" when an object is not destroyed.

I changed my tutorial slightly to match some wordings with handle as reference, but I think my tutorial already covers some important stuff: Memory Leaks

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

IcemanBo said:

 Resource can leak object and it's reference, yes. We usually call it "reference leak" if variables is not nulled, and "object leak" when an object is not destroyed.

I changed my tutorial slightly to match some wordings with handle as reference, but I think my tutorial already covers some important stuff: Memory Leaks

Click to expand...

if you actually explain difference between these leaks it's fine

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

DracoL1ch said:

 Everytime you null object counter reduced by 1


Click to expand...

DracoL1ch said:

 . Game won't re-use those, so will take following 4 bytes. So, in order to waste just 1 MB of RAM you have to make 1048576 leaks (1mln+)


Click to expand...

that might be good to note, which I don't atm. Will look for it in some days, where to do best.

Also iirc the id is not even recycled instantly, or? So technicaly it might also work perfectly fine if you null all variables after 10 seconds or so after removal? I'm not very sure how the process works, if it's a periodical lookup or what.

But maybe it's anyways just conufusing in the end and not important.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

u = gettriggerunit()

unit count ++

u=null

unit count --

if function ends without nulling you will never ever reduce counter to zero therefore this ID will be unusable in the future

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

You can null a locals after a triggersleepaction(), or null globals after a timer timeout.

And if a periodic check handles the id recycling each 30 seconds for example, then you might be lucky if you null it only after 10 seconds, I meant.

I just said, I'm not fully aware of how the recylcing process works, but iirc it was not instant (awlays?).

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

units normally dies for 14 seconds (by default), except illsuoins and summons. during this time they're still keep the handle

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Are you sure that the handle is instantly recylced after complete removal? I'm really curious. Have you maybe tested in past? Because I somehow thought it's recylcled in peridic recycle batches.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

I didnt test it, no reason to know this anyway

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Submission:Object Leaks vs Reference Leak

Date:24 Jan 17

Status:Graveyard

Note:

It's an interesting topic to discuss.

Though Memory Leaks covers important points as I can see.

 You must log in or register to reply here.


### Similar threads

 Memory Leaks


IcemanBo

Mar 6, 2015

Trigger (GUI) Editor Tutorials
2

Replies95Views79K

Jul 25, 2024
corpuzmarvicjune

 How to remove Memory Leaks IN GUI


KingB00ker

Jul 6, 2009

Tutorial Archive

Replies18Views6K

Jul 12, 2009
Pyritie

Locked

[JASS]
 Nulling leaks...


HINDYhat

Dec 17, 2007

Triggers & Scripts

Replies22Views3K

Dec 26, 2007
Earth-Fury

Locked

 Where is the handle leak in this function ???


Macadamia

Mar 17, 2020

World Editor Help Zone

Replies0Views1K

Mar 17, 2020
Macadamia

Locked

[Spell]
 Does this ability leak?


Doomlord

Aug 8, 2012

World Editor Help Zone

Replies22Views3K

Aug 11, 2012
Dr. Boom

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