---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/vjass-optimization-using-a-first-of-group-loop-for-enumeration.223140/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# vJASS Optimization Using a First of Group Loop for Enumeration
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/vjass-optimization-using-a-first-of-group-loop-for-enumeration.223140/>
#### Cokemonkey11

#### Cokemonkey11

Spell Reviewer

 Level 30

JoinedMay 9, 2006Messages3,572

Resources: 25

#### Resources
Tools 4Maps 8Spells and Systems 3JASS 7Tutorials 2Wurst 1

vJass Optimization: Using a First of Group Loop for Enumeration

Preface:


This guide is intended for experienced jass users who have a goal of optimized triggers in their maps. My aim is to create a short, concise, and targeted tutorial that is easy to follow and navigate.


My primary audience is users who know how to use the best method of enumeration prior to the 1.24 warcraft 3 update - a filterfunc that always return false'd. For these users, this tuturial will explain the "new" best way to do the same thing, and why you'll see this practice in code.


The body of this article will be in example form, with only some minor explanation throughout.

Limitations:


The inherent idea of a First of Group loop is that the group is emptied each time it is used. This can be detrimental in the case that you want to save a group of units in a group for use at different times and/or asynchronously.

Pre-requisites:


You should be familiar with using a filterfunc to run actions on groups of units, but this is not necessary to learn how a First of Group loop is done.


This article is written in vJass for use of globals blocks, which increases readabiliy. The same concepts are possible in vanilla JASS, but I recommend using JassHelper or just having JNGP to compile vJass.

The Tutorial:


To begin, I'd like to open with an example script of an enumeration using a filterfunc:

Example A: Enumeration using a filter function

 JASS:

scope filterfuncExample initializer i
 globals
 private group grp=CreateGroup() //We create the group on initialization, but never destroy it. The group never actually retains units, too, which means that you could potentially use one "tempGroup" for your whole map instead of one for your scope, as I do in this case.
 endglobals

 private function f takes nothing returns boolean
 local unit fU=GetFilterUnit()
 if GetUnitTypeId(fU)=='hfoo' then //this block is equivalent to imposing a condition and set of actions to enumerated units, but consolidated to one function.
 call KillUnit(fU)
 endif
 set fU=null
 return false //we return false so that our filter doesn't "pass" any unit. The group remains empty, but we apply our effects to a group of units anyway.
 endfunction

 private function i takes nothing returns nothing
 call GroupEnumUnitsInRange(grp,0.,0.,256.,Filter(function f))
 endfunction
endscope


If you're newer to jass and you've never seen this before, take note that it was the standard practice for enumeration prior to the 1.24 wc3 update. With this method you could apply any number of conditions or actions to a group of units without creating, keeping track of, or destroying groups.


However, there is another common method. We have a native called FirstOfGroup() which fetches the "first" unit of a group. It also happens to be a very fast native.


Here is the equivalent function to the one above, but using a "first of group loop":

Example B: Enumeration using a first of group loop

 JASS:

scope firstOfGroupLoopExample initializer i
 globals
 private group grp=CreateGroup()
 endglobals

 private function i takes nothing returns nothing
 local unit FoG=null //We initially declare an empty unit handler
 call GroupEnumUnitsInRange(grp,0.,0.,256.,null) //Note that we're using 'null' for our filterfunc. This means that *all* units in range will be added to the group.
 loop
 set FoG=FirstOfGroup(grp) //If you're new to this kind of loop, this part might look strange to you.
 exitwhen FoG==null //combined with the above line, this is effectively checking if the group is empty
 if GetUnitTypeId(FoG)=='hfoo' then //here we have a make-shift condition statement.. followed by an action below
 call KillUnit(FoG)
 endif
 call GroupRemoveUnit(grp,FoG) //here's how we make the loop eventually terminate. Remove first of group, and when the loop restarts we get the new "first of group".
 endloop
 endfunction
endscope


That's all there is to it. You can now literally convert any filter function based enumeration to a first of group based one. Here's a few additional notes to keep in mind:

Additional notes

This method was also useable before 1.24, however the null in call GroupEnumUnitsInRange(grp,0.,0.,256.,null) caused an unavoidable leak. As a result you could still use first of group loops, but since a filterfunc was necessary to avoid the leak, first of group loops were actually slower.

This method also keeps the whole loop in-line, meaning that local variables don't need to unnecessarily be stored as globals as in the case with the filterfunc method.

The reason this is faster (if you were wondering) is because FirstOfGroup() is significantly faster than calling a filter function - even if you have to call FirstOfGroup() exponentially more often.

If you're asking yourself if all of this is actually necessary, the answer is "no, it's not", because the performance you'll save doing this, although large in relation, is actually so small that you shouldn't see a difference in any real application. The point of this is to encourage script structure that is uniform. As such, if you perform a filterfunc enumeration in a system or spell which you submit to the hive, you will likely receive warning from more experienced scripters saying you should be using the first of group method.

Advanced: Retaining group contents with a swap variable


What if you want to keep a group of units, and perform some actions on all the units without changing the group itself? We can cheat of course, by using a swap group and filling it in the scope of an FoG loop.


Is this the only method? No. Indeed, for a long time, the standard practice was to:

 Enumerate units with a GroupEnumerate... native, potentially filtering them with a filterfunc

 Use the ForGroup() native to perform actions on all the units and keep the group contents the same


What's wrong with this method? Nothing is strictly wrong with it - ForGroup() is even still a useful method for cases when a new virtual thread of execution needs to be started.


Consider the following code which uses a swap group:

 JASS:

scope test
 globals
 private group iterator=CreateGroup()
 private group swap=CreateGroup()
 private group temp
 endglobals

 private function fgSwap takes nothing returns nothing
 local unit FoG
 loop
 set FoG=FirstOfGroup(iterator)
 exitwhen FoG==null
 //
 call GroupAddUnit(swap,FoG)
 call GroupRemoveUnit(iterator,FoG)
 endloop
 set temp=iterator
 set iterator=swap
 set swap=temp
 endfunction
endscope


It would appear that this code is significantly slower than ForGroup() due to the overhead of adding units at each stage and swapping the handles - you'd be wrong. This method is faster because it doesn't start a new virtual thread of execution for each unit. In fact, the swapping method is 30% faster for 20 units, with non-diminishing returns at higher values.


I suspect that in due time, group utility libraries will be updated to accommodate this improvement, but for now you can script it yourself in an ad-hoc manner.


 Last edited: Jan 18, 2014

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice explanations. This will be useful for linkbacks to guide the misinformed.

Approved.

#### Cokemonkey11

#### Cokemonkey11

Spell Reviewer

 Level 30

JoinedMay 9, 2006Messages3,572

Resources: 25

#### Resources
Tools 4Maps 8Spells and Systems 3JASS 7Tutorials 2Wurst 1

Woot! Thanks purge, I hope someone finds this useful.

I've written this tutorial in response to a PM I've received, basically asking questions about a system. I'd like to write a tutorial like this again in the future if people will use them.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

I once said that I would make love to anyone who would make this tutorial for me.

Pucker up.

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

You should include info about group swapping.

#### Cokemonkey11

#### Cokemonkey11

Spell Reviewer

 Level 30

JoinedMay 9, 2006Messages3,572

Resources: 25

#### Resources
Tools 4Maps 8Spells and Systems 3JASS 7Tutorials 2Wurst 1

Updated. Please check for succinctness and correctness.

#### Matt1965

#### Matt1965

 Level 3

JoinedJul 12, 2006Messages23

Pssh.. advanced users..

#### Ceday

#### Ceday

 Level 12

JoinedFeb 22, 2010Messages1,115

So are you guys SURE now this is the best method?History is weird.

1)Condition + ForGroup

2)FirstOfGroup loop

3)Condition only without ForGroup

4)Now FirstOfGroup loop again

#### Cokemonkey11

#### Cokemonkey11

Spell Reviewer

 Level 30

JoinedMay 9, 2006Messages3,572

Resources: 25

#### Resources
Tools 4Maps 8Spells and Systems 3JASS 7Tutorials 2Wurst 1

Ceday said:

 So are you guys SURE now this is the best method?History is weird.


1)Condition + ForGroup


2)FirstOfGroup loop


3)Condition only without ForGroup


4)Now FirstOfGroup loop again


Click to expand...


Yes, #3 came in because it was discovered that null Condition leaked. However, that was fixed by blizzard in a patch (1.24?)


See "Additional notes" section. Also see https://www.hiveworkshop.com/forums/lab-715/jass-benchmarking-results-245362/

 You must log in or register to reply here.


### Similar threads

Locked

[Solved]
 Group swap


ZiBitheWand3r3r

Jul 21, 2018

Triggers & Scripts

Replies8Views3K

Jul 31, 2018
IcemanBo

Locked

[vJASS]
 Group Enumeration


Mr_Bean

Jun 22, 2012

Triggers & Scripts

Replies4Views2K

Jun 22, 2012
WaterKnight

 SwappedGroup


Luorax

Dec 26, 2013

Graveyard

Replies35Views4K

Mar 14, 2014
TriggerHappy

Locked

[JASS]
 Locust Enumeration


Ofel

Mar 23, 2016

Triggers & Scripts

Replies3Views2K

Mar 25, 2016
Ofel

Locked

 Basics - Unit Groups


th3ygen

Jul 12, 2018

Advanced Scripting

Replies2Views1K

Jul 15, 2018
th3ygen

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

JASS Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top