---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/vjass-spell-templates.216166/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# vJASS Spell Templates
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/vjass-spell-templates.216166/>
#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

vJASS Spell Templates

Table of Contents

Introduction

Instant Spells

Timed Spells

Public Resources

Wrap-Up

Introduction

This is more of a repository or reference rather than a tutorial.

I'm going to show you some of the cleanest and most ideal spell

models you could follow. These models have no requirements at all.

Instant Spells

Instant spells have one and only one model in my eyes.

This is the ideal model for an instant spell:

 JASS:

struct Spell extends array

 private static constant integer ABIL_CODE = 'A000'
 private static constant integer BUFF_CODE = 'B000'
 private static constant integer UNIT_CODE = 'u000'

 private static method run takes nothing returns nothing
 local unit caster = GetTriggerUnit()
 local unit target = GetSpellTargetUnit()

 // Code.

 set caster = null
 set target = null
 endmethod

 private static method check takes nothing returns boolean
 if GetSpellAbilityId() == thistype.ABIL_CODE then
 call thistype.run()
 endif
 return false
 endmethod

 private static method onInit takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(t, Condition(function thistype.check))
 set t = null
 endmethod
endstruct

Timed Spells

No matter what kind of timed spell you need, this is the model that I see fit.

Even if you didn't need a periodic spell, this model would assure that you will

only be needing one and only one timer at all times.

If you wanted a spell that would wait for a small amount of time, execute

some code and then end, you would use an integer that is decreased after

each iteration. When the integer is 0, you would execute some code and

destroy the instance.

This is the spell model that I use:

 JASS:

struct Spell extends array

 private static constant integer ABIL_CODE = 'A000'
 private static constant integer BUFF_CODE = 'B000'
 private static constant integer UNIT_CODE = 'u000'

 private static constant real TIMEOUT = 0.03125

 private static integer array rn
 private static integer ic = 0

 private static integer array next
 private static integer array prev

 private static timer iterator = CreateTimer()
 private static integer count = 0

 private static unit array caster
 private static unit array target
 private static real array damage

 method destroy takes nothing returns nothing
 /*
 * Deallocate this instance.
 */
 set rn[this] = rn[0]
 set rn[0] = this

 /*
 * We remove the instance from the linked list.
 */
 set prev[next[this]] = prev[this]
 set next[prev[this]] = next[this]

 /*
 * We decrease the count by 1.
 * If the count is 0, we pause the timer.
 */
 set count = count - 1
 if count == 0 then
 call PauseTimer(iterator)
 endif

 /*
 * We null the data in the struct.
 * This is completely optional. It doesn't really make a difference
 * at all. (Unless you're casting the spell some hundreds of times.)
 * If you have some real memory intense systems in your map,
 * you might want to do this, especially if your struct has a lot of data.
 *
 * These are global variables, so they will be recycled eventually.
 * It's all up to you, my friend.
 */
 set caster[this] = null
 set target[this] = null

 // Code.

 endmethod

 private static method periodic takes nothing returns nothing
 /*
 * Starting from the first instance, we loop
 * over all the instance in the list until we hit
 * a dead-end.
 */
 local thistype this = next[0]
 loop
 exitwhen this == 0

 // Code.

 set this = next[this]
 endloop
 endmethod

 private static method run takes nothing returns nothing
 /*
 * We allocate an instance.
 */
 local thistype this = rn[0]
 if this == 0 then
 set ic = ic + 1
 set this = ic
 else
 set rn[0] = rn[this]
 endif

 /*
 * We add the instance to the linked list.
 */
 set next[this] = 0
 set prev[this] = prev[0]
 set next[prev[0]] = this
 set prev[0] = this

 /*
 * We increase the count by 1.
 * If the count is 1, we start the timer to loop through
 * the instances. This is because recasting the spell while
 * an instance is already running shouldn't restart the timer.
 */
 set count = count + 1
 if count == 1 then
 call TimerStart(iterator, TIMEOUT, true, function thistype.periodic)
 endif

 /*
 * We set our struct data.
 */
 set caster[this] = GetTriggerUnit()
 set target[this] = GetSpellTargetUnit()

 // Code.
 endmethod

 private static method check takes nothing returns boolean
 if GetSpellAbilityId() == thistype.ABIL_CODE then
 call thistype.run()
 endif
 return false
 endmethod

 private static method onInit takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(t, Condition(function thistype.check))
 set t = null
 endmethod
endstruct

What I'm doing inside the above model is using my own allocation/deallocation technique.

This tutorial by Nestharus shows you how to make your own custom allocator. In fact, if

you don't like writing your own and don't want to learn how to, you can use resources

like Alloc by Sevion.

You would also notice that I made the struct arrays explicit.

Most people hate this as well.

So, the below model does the exact same thing that the above one does, except it

embraces the dot operator and it uses JassHelper's generated allocator which is totally fine.

 JASS:

struct Spell

 private static constant integer ABIL_CODE = 'A000'
 private static constant integer BUFF_CODE = 'B000'
 private static constant integer UNIT_CODE = 'u000'

 private static constant real TIMEOUT = 0.03125

 private thistype next
 private thistype prev

 private static timer iterator = CreateTimer()
 private static integer count = 0

 private unit caster
 private unit target
 private real damage

 private method destroy takes nothing returns nothing
 /*
 * Deallocate this instance.
 */
 call this.deallocate()

 /*
 * We remove the instance from the linked list.
 */
 set this.next.prev = this.prev
 set this.prev.next = this.next

 /*
 * We decrease the count by 1.
 * If the count is 0, we pause the timer.
 */
 set count = count - 1
 if count == 0 then
 call PauseTimer(iterator)
 endif

 /*
 * We null the data in the struct.
 * This is completely optional. It doesn't really make a difference
 * at all. (Unless you're casting the spell some hundreds of times.)
 * If you have some real memory intense systems in your map,
 * you might want to do this, especially if your struct has a lot of data.
 *
 * These are global variables, so they will be recycled eventually.
 * It's all up to you, my friend.
 */
 set this.caster = null
 set this.target = null

 // Code.

 endmethod

 private static method periodic takes nothing returns nothing
 /*
 * Starting from the first instance, we loop
 * over all the instance in the list until we hit
 * a dead-end.
 */
 local thistype this = thistype(0).next
 loop
 exitwhen this == 0

 // Code.

 set this = this.next
 endloop
 endmethod

 private static method run takes nothing returns nothing
 /*
 * We allocate an instance.
 */
 local thistype this = thistype.allocate()

 /*
 * We add the instance to the linked list.
 */
 set this.next = 0
 set this.prev = thistype(0).prev
 set thistype(0).prev.next = this
 set thistype(0).prev = this

 /*
 * We increase the count by 1.
 * If the count is 1, we start the timer to loop through
 * the instances. This is because recasting the spell while
 * an instance is already running shouldn't restart the timer.
 */
 set count = count + 1
 if count == 1 then
 call TimerStart(iterator, TIMEOUT, true, function thistype.periodic)
 endif

 /*
 * We set our struct data.
 */
 set this.caster = GetTriggerUnit()
 set this.target = GetSpellTargetUnit()

 // Code.
 endmethod

 private static method check takes nothing returns boolean
 if GetSpellAbilityId() == thistype.ABIL_CODE then
 call thistype.run()
 endif
 return false
 endmethod

 private static method onInit takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(t, Condition(function thistype.check))
 set t = null
 endmethod
endstruct

Public Resources

Please note that the above models shouldn't be used as is for public resources.

In public resources, I would recommend wrapping all the above code in

a library block so that you could make the requirements explicit and clear.

The models I depicted above should only be used for Map-making.

They were made to be easy to understand, efficient and clean.

Wrap-Up

This concludes my very short and 'straight to the point' tutorial.

Please note, I'm not saying that you should be using these spell models,

I'm merely offering you some pretty good ones in case you don't know

any. If you have any questions, feel free to ask.


 Last edited by a moderator: Mar 24, 2016

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

 I would recommend wrapping all the above code in

a library block so that you could make the requirements explicit and clear.


Click to expand...

Can you show me what a library block looks like. This is a nice reference. As you know, I am switching to vjass from GUI.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

A library block:

 JASS:

library MyLibrary requires MyOtherLibrary, MyAwesomeLibrary, MyBoringLibrary

// The templates I showed go here

endlibrary

They're used to put your systems in order.

If I have a system that's using TimerUtils and it's code ends up above TimerUtils, it won't compile right.

If you put your system in a library and make it require TimerUtils, it will compile right because JassHelper will move TimerUtils above it.

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

How about if the spell has no requirements, do I still need to use library block?

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

So I don't have to type the same thing again, https://www.hiveworkshop.com/forums/members/eubz/#vmessage290591

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

Thanks Mag, I guess I have to right my next spells in vjass. It is really better to write than to roll and click.

EDIT: So I will just write

 JASS:

library MyLibrary
//codes
endlibrary

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Any thoughts? Suggestions? Tips?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Seems good. However, you should provide a version that isn't using arrays. A lot of people aren't used to using arrays within their structs, and with the timer sample they are unable to use the normal struct syntax. Try having this as an alternative:

 JASS:

struct Spell extends array

 private static constant integer ABIL_CODE = 'A000'
 private static constant integer BUFF_CODE = 'B000'
 private static constant integer UNIT_CODE = 'u000'

 private static constant real TIMEOUT = 0.03125

 private thistype rn
 private static integer ic = 0

 private thistype next
 private thistype prev

 private static timer iterator = CreateTimer()
 private static integer count = 0

 private unit caster
 private unit target
 private real damage

 method destroy takes nothing returns nothing
 /*
 * Deallocate this instance.
 */
 set this.rn = thistype(0).rn
 set thistype(0).rn = this

 /*
 * We remove the instance from the linked list.
 */
 set this.next.prev = this.prev
 set this.prev.next = this.next

 /*
 * We decrease the count by 1.
 * If the count is 0, we pause the timer.
 */
 set count = count - 1
 if count == 0 then
 call PauseTimer(iterator)
 endif

 /*
 * We null the data in the struct.
 */
 set this.caster = null
 set this.target = null

 // Code.

 endmethod

 private static method periodic takes nothing returns nothing
 /*
 * Starting from the first instance, we loop
 * over all the instance in the list until we hit
 * a dead-end.
 */
 local thistype this = thistype(0).next
 loop
 exitwhen this == 0

 // Code.

 set this = this.next
 endloop
 endmethod

 private static method run takes nothing returns boolean
 /*
 * We allocate an instance.
 */
 local thistype this = thistype(0).rn
 if this == 0 then
 set ic = ic + 1
 set this = ic
 else
 set thistype(0).rn = this.rn
 endif

 /*
 * We add the instance to the linked list.
 */
 set this.next = thistype(0) // or just 0
 set this.prev = thistype(0).prev
 set thistype(0).prev.next = this
 set thistype(0).prev = this

 /*
 * We increase the count by 1.
 * If the count is 1, we start the timer to loop through
 * the instances. This is because recasting the spell while
 * an instance is already running shouldn't restart the timer.
 */
 set count = count + 1
 if count == 1 then
 call TimerStart(iterator, TIMEOUT, true, function thistype.periodic)
 endif

 /*
 * We set our struct data.
 */
 set this.caster = GetTriggerUnit()
 set this.target = GetSpellTargetUnit()

 // Code.

 return false
 endmethod

 private static method onInit takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(t, Condition(function thistype.run))
 set t = null
 endmethod
endstruct

A few small things.

 JASS:

 set caster[this] = null
 set target[this] = null

Nulling is not necessary since they are globals. The handle ids of objects will be recycled even if a global variable is pointing to it, but that is not the case with local variables. I suppose one could argue that it saves memory to null them, but remember that with spells they are usually cast pretty often throughout the map, meaning that the globals will end up being reassigned anyway. In the end, the nulling will probably not make any significant difference in memory usage. It is just not as necessary as nulling local variables.

The next problem:

 JASS:

 set next[this] = 0
 set prev[this] = prev[0]
 set next[prev[0]] = this
 set prev[0] = this

In here, you set this.next = head, then this.prev = head.prev, and then head.prev.next = this, and finally head.prev = this (head = thistype(0))

Therefore this is what it would look like in the process:

Before:

D <-> C <-> B <-> head

this.next = head

D <-> C <-> B <-> head, A -> head

this.prev = head.prev

D <-> C <-> B -> head, B <- A -> head

head.prev.next = this

D <-> C <-> B <-> A -> head

head.prev = this

D <-> C <-> B <-> A <-> head

Of course, you know this. However, this is a backwards list. Therefore, you would iterate using head.prev rather than head.next. Since global integer arrays default 0 iirc, the loop here:

 JASS:

 local thistype this = next[0]
 loop
 exitwhen this == 0

 // Code.

 set this = next[this]
 endloop

Will immediately quit since next[0] (head.next) is equal to 0. To fix this, just change it to prev[0] and prev[this] or you can change the list allocation to make it a forward list where head.next = first member.

One final note to add is that you should link Nestharus' tutorial for users to know why the spell model has a custom allocation/deallocation:

https://www.hiveworkshop.com/forums...ls-280/coding-efficient-vjass-structs-187477/

While yours is edited slightly to be improved a bit, they will still understand the motives. I know a lot of people get confused when they first see that, and there are a lot of vJASS spell makers who don't know of the reasons behind it.

Fix those things and it can be approved.  Nice job mag, your tutorials/references/samples are always detailed and well thought-out.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

 However, you should provide a version that isn't using arrays. A lot of people aren't used to using arrays within their structs, and with the timer sample they are unable to use the normal struct syntax.


Click to expand...

I agree. Will do.

I guess I can give several models and put them in order based on complexity.

 Nulling is not necessary since they are globals. The handle ids of objects will be recycled even if a global variable is pointing to it, but that is not the case with local variables. I suppose one could argue that it saves memory to null them, but remember that with spells they are usually cast pretty often throughout the map, meaning that the globals will end up being reassigned anyway. In the end, the nulling will probably not make any significant difference in memory usage. It is just not as necessary as nulling local variables.

Click to expand...

Then I'll change this:

 JASS:

/*
 * We null the data in the struct.
 */
 set caster[this] = null
 set target[this] = null

to this

 JASS:

/*
 * We null the data in the struct.
 * This is completely optional. It doesn't really make a difference
 * at all. (Unless you're casting the spell some hundreds of times.)
 * If you have some real memory intense systems in your map,
 * you might want to do this, especially if your struct has a lot of data.
 *
 * These are global variables, so they will be recycled eventually.
 * It's all up to you, my friend.
 */
 set caster[this] = null
 set target[this] = null

 The next problem:

 JASS:

set next[this] = 0
set prev[this] = prev[0]
set next[prev[0]] = this
set prev[0] = this

In here, you set this.next = head, then this.prev = head.prev, and then head.prev.next = this, and finally head.prev = this (head = thistype(0))

Therefore this is what it would look like in the process:

Before:

D <-> C <-> B <-> head

this.next = head

D <-> C <-> B <-> head, A -> head

this.prev = head.prev

D <-> C <-> B -> head, B <- A -> head

head.prev.next = this

D <-> C <-> B <-> A -> head

head.prev = this

D <-> C <-> B <-> A <-> head

Of course, you know this. However, this is a backwards list. Therefore, you would iterate using head.prev rather than head.next. Since global integer arrays default 0 iirc, the loop here:

 JASS:

local thistype this = next[0]
loop
 exitwhen this == 0

 // Code.

 set this = next[this]
endloop

Will immediately quit since next[0] (head.next) is equal to 0. To fix this, just change it to prev[0] and prev[this] or you can change the list allocation to make it a forward list where head.next = first member.


Click to expand...

I get that sometimes, but here's what's going on:

next[0] is never being set unless we're creating the first node.

 JASS:

set next[this] = 0
set prev[this] = prev[0]
set next[prev[0]] = this // This is where it gets set. prev[0] will initially be 0.
set prev[0] = this

Now if we clear the list and re-add a node:

 JASS:

set next[prev[this]] = next[this]
set prev[next[this]] = prev[this]

Let's assume there's one node.

Currently, we have:

next[this] = 0

prev[this] = 0

For only one node in the list, so:

 JASS:

set next[prev[this]] = next[this]
set prev[next[this]] = prev[this]

->

[code=jass]set next[0] = 0
set prev[0] = 0

Meaning we would return to the initial conditions of the list before we added any nodes to begin with

And if we add a node, next[0] will be set to 'this' node via set next[prev[0]] = this

I hope this explains it. C:

 One final note to add is that you should link Nestharus' tutorial for users to know why the spell model has a custom allocation/deallocation:

Coding efficient vJASS structs

While yours is edited slightly to be improved a bit, they will still understand the motives. I know a lot of people get confused when they first see that, and there are a lot of vJASS spell makers who don't know of the reasons behind it.


Click to expand...

Funny, I always somehow find a reason to link to that tutorial in most of my tutorials. xD

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Ohhh haha I must've misread/misunderstood it. Thanks for clearing that up. I just realized it is kind of circular. (that is, if you count 0 as the head node)

Yeah if you make those changes I'll approve it. Although, it is still probably approvable in its current state.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

There, updated.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

In the dot syntax version, you have this:

 JASS:

private integer next
private integer prev

When it should be:

 JASS:

private thistype next
private thistype prev

iirc. Otherwise they won't let you do the dot syntax. It'll throw an error, no?

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

I'm not sure. The static integer array causes no errors, but I think you're right about the dot operator syntax version.

I'll update it.

edit

Done, about 5 days ago


 Last edited: Jun 15, 2012

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Oops. Timed out last time I checked this thread.. thought i had approved.

Anyway, approved.

#### IsaacNewbton

#### IsaacNewbton

 Level 4

JoinedFeb 22, 2012Messages74

Resources: 1

#### Resources
Spells and Systems 1

these make a lot of sense after I just watched the vjass tutorials on youtube. I am wondering if because structs are instances, can you have 8191 simultaneous instances of each spell going? does it recycle the index when it is finished?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Yes, you can have 8191 instances of a spell going on at the same time. And yes, you will normally recycle the index when the spell is finished, so the instance cap doesn't really matter. Usually you won't hit 8191 instances unless you are doing something wrong.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

I saw a typo : call TriggerAddCondition(function thistype.run)

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Fixed.

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

For instant Spells, I can just use a method like "private static method run" and there is no need for "create/allocate" or "deallocate/destroy" ?

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Since they're instant, yes.

Of course, if it's possible that you're going to order a unit to cast the casted spell within your run static method, you'll have to use a recursion safe function. I'll write a tutorial on Recursion safety <:

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Bump:

Here you go, another tutorial by yours truly~

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Thank you.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

In your 2nd example of Timed Spell Model, why are thistype next and thistype prev not static?


Also, you could prevent using a count variable by checking the status of the linked list.


In static method create, if thistype.prev == 0 then start timer


In method destroy, if thistype.next == 0 then pause timer.

#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

There doesn't seem to be any GetSpellAbilityId() "guards" in any of the "spell models", which could mean that those "spell models" might not have been tested by the author?


Also it's kind of ironic that the author of the tutorial uses TriggerRegisterAnyUnitEventBJ instead of SpellEffectEvent which is based on one of his own resources and is more efficient/clean when that's was the whole point of the tutorial.

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

You are both right and that issues

should eventually be fixed by PurgeandFire, me or any other moderator in charge.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

1 timer per spell instance (instead of 1 global timer for all spell instances) combined with a single global hashtable write/read beats this big time in terms of simplicity, because all the linked list stuff is gone. You need only 1/3rd the amount of code you need with the examples in this tutorial.

When it comes to coding spells, simple code is preferred over efficient code imho.

So, in other words, the title of this tutorial is a bit misleading. It should be "Efficient vJass spells" and not "clean and efficient vJass spells".

Also, here's more food for thought:

Clean spells make use of missile and buff systems that allow custom callbacks, which render most cases in which you would need a periodic callback manually implemented nonsensical.

Your spell deals damage over time? --> Buff system periodic callback

Your spell places a debuff that triggers on attacks? --> Buff system onAttack callback

Your spell fires a missile that damages units? --> Missile system periodic callback or onHit callback

Your spell knocks a unit in the air and damages it on impact? --> Knockback system callback

Unless you are really doing fancy stuff like a time stop or something, you rarely even need manual implementations anymore. A robust missile, buff and knockback system will solve 99% of spell designs.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Flux said:

 In your 2nd example of Timed Spell Model, why are thistype next and thistype prev not static?


Click to expand...


They are static:

 JASS:

 private static integer array next
 private static integer array prev


 Flux said:



 Also, you could prevent using a count variable by checking the status of the linked list.


In static method create, if thistype.prev == 0 then start timer


In method destroy, if thistype.next == 0 then pause timer.


Click to expand...


Good catch. I'll update it later.

Aniki said:

 There doesn't seem to be any GetSpellAbilityId() "guards" in any of the "spell models", which could mean that those "spell models" might not have been tested by the author?


Also it's kind of ironic that the author of the tutorial uses TriggerRegisterAnyUnitEventBJ instead of SpellEffectEvent which is based on one of his own resources and is more efficient/clean when that's was the whole point of the tutorial.


Click to expand...


Fixed. I opted not to use SpellEffectEvent at the moment, but maybe I'll change it.

Zwiebelchen said:

 ...


Click to expand...


That is all very true. This tutorial could probably use a revamp. I'll just rename the thread for now to "vJASS Spell Models".

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

2nd Timed Spell = 3rd code overall. My bad for not being clear enough.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

I've further renamed this to "vJass Spell Templates" as "model" is a reserved keyword for artistic models.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Flux said:

 2nd Timed Spell = 3rd code overall. My bad for not being clear enough.


Click to expand...


Why should they be static? Each instance of the struct has its own next/prev instance.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

PurgeandFire said:

 Why should they be static? Each instance of the struct has its own next/prev instance.


Click to expand...


I was mistaken. Yeah it shouldn't be static because he is using OOP syntax.

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

I quickly wrote this. Still requires some pollishing and a final conclusion.

Spell Crafting for Dummies​

Table of Content

Jass Newgen Pack

Proper encapsulation

SpellEffectEvent

Clean design - globals

Instant spell template

Timed spell template

Jass Newgen Pack

top

Before getting started make sure you have JNGP installed. ( download here )

This tool enables a more professional WorldEditor surface including the JassHelper, which is the vJass compiler.

Without Jass Newgen Pack you'll not be able to write and compile vJass at all.

Proper encapsulation

top

A big advantage of vJass over JASS is encapsulation ( JassHelper link ), basically restrict access to your code.

Available are scope endscope, library endlibrary, struct endstruct and the private keyword.

Use libaries if other code relates on the content of your spell or system.

Otherwise a scope should be your first choice.

Structs will be discussed later.

 JASS:

scope MySpell
// Content.
endscope

SpellEffectEvent

top

It's a common practise in map-making to outsource specific parts of your code,

in other words handing on the work steps to external libraries.

In this guide I want you to focus on SpellEffectEvent written by Bribe.

Library SpellEffectEvent structures the EVENT_PLAYER_UNIT_SPELL_EFFECT in the most performant way possible.

Furthermore we get rid of the quite annoying GetSpellAbilityId() == MY_ABILITY_ID comparison.

Hint: Take a closer look into our JASS section.

Globals

top

Don't make the mistake and hardcode your entire spell. At least the ability raw should be stored

into a constant integer. You'll end up with much better read-ability and a good surface for later changes in code.

Instant spells

top

The template for an instant spell is rather simple.

Note that I'm using the above mentioned private keyword.

 JASS:

scope MySpell initializer Init

 globals // Always use descriptive variable names.
 private constant integer ABILITY_ID = 'A000'
 endglobals
 // Runs when an ability of type id "ABILITY_ID" was casted.
 private function OnEffect takes nothing returns nothing
 local unit source = GetTriggerUnit()
 local unit target = GetSpellTargetUnit()
 local integer level = GetUnitAbilityLevel(source, ABILITY_ID)

 // Your spell code.

 set source = null
 set target = null
 endfunction
 // Runs when the map is initialized.
 private function Init takes nothing returns nothing
 call RegisterSpellEffectEvent(ABILITY_ID, function OnEffect)
 endfunction

endscope

Timer based spell template

top

Honestly there are many good code structures for timed spells.

What fits best relates on many factors such as

The timer timeout.

The approximate amount of simultaneously running instances.

The spell concept itself.

And other factors

The way I'll show you is by far the simplest. One timer per spell instance.

I would like to introduce Vexorian's TimerUtils to you.

TimerUtils allows us to pop ( get ) a timer from a preloaded timer collection and connect it to an integer.

The API is as easy as pie:

function NewTimerEx takes integer index returns timer​

function GetTimerData takes timer t returns integer​

function ReleaseTimer takes timer t returns nothing​
What we need next is a unique index for our spell instance. For example 1, 2, 3, 4, ...

At this point structs ( JassHelper link ) come into play.

Next to a more advanced encapsulation ( see above ), they come along with an invisible instance allocator.

It's a bit complex to explain, but I think the example will do it.

 JASS:

scope MySpell

 globals // Always use descriptive variable names.
 private constant integer ABILITY_ID = 'A000'
 endglobals

 private struct MySpell

 // These are called "struct members" ( array variable )
 private unit source// You could also see them like for example source[index].
 private unit target
 private real damage
 private integer level
 private real duration

 private static method onPeriodic takes nothing returns nothing
 local timer tmr = GetExpiredTimer()

 // TimerUtils API - function GetTimerData takes timer t returns integer.
 //
 // Returns the in onEffect saved thisytpe "this" value.
 local thistype this = GetTimerData(tmr)

 // Your spell code. For example.
 call UnitDamageTarget(this.source, this.target, this.damage, false, false, null, null, null)

 set this.duration = this.duration - 0.03125
 if this.duration <= 0. then

 // When the spell is over you have to recycle the instance,
 // so it can be re-used when the spell is casted again.
 //
 call this.destroy()// Recycles thistype "this".

 // TimerUtils API - function ReleaseTimer takes timer t returns nothing
 //
 // Pushes timer tmr back to the timer collection of the TimerUtils library,
 // so it can be re-used for the next NewTimerEx(index) function call.
 call ReleaseTimer(tmr)

 // Cleanup
 set this.source = null
 set this.target = null
 endif

 set tmr = null
 endmethod

 private static method onEffect takes nothing returns nothing
 //
 // First of all we want to create a unique index for our spell.
 // For example index 1.
 // In structs we are using "thistype this"
 // which is compiled nothing else then "MySpell__integer" this.

 local thistype this = thistype.create()// Creates a unique index "this".

 // Assign the desired values the struct members.
 // The syntax is very simple. this.variable =
 //
 set this.source = GetTriggerUnit()
 set this.target = GetSpellTargetUnit()
 set this.level = GetUnitAbilityLevel(source, ABILITY_ID)
 set this.duration = 5.
 set this.damage = 150.

 // More of your spell code.

 // Save "this" to a timer and start it.
 call TimerStart(NewTimerEx(this), 0.03125, true, function thistype.onPeriodic)
 endmethod
 // Runs when the map is initialized.
 private static method onInit takes nothing returns nothing
 call RegisterSpellEffectEvent(ABILITY_ID, function MyOnEffect)
 endmethod

 endstruct

endscope

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

That is really good. I would prefer if you submitted it as a separate tutorial, and then I'll GY mag's.

 You must log in or register to reply here.


### Similar threads

Locked

[vJASS]
 Why this timer does not stop?


Geshishouhu

Dec 13, 2013

Triggers & Scripts

Replies4Views2K

Dec 13, 2013
Geshishouhu

 [vJASS] Dynamic Indexing Tutorial


Malhorne

Dec 25, 2013

JASS Tutorials
2

Replies58Views16K

Jul 13, 2015
Zwiebelchen

 Efficient Spell Cloning


AGD

Mar 20, 2017

JASS Tutorials

Replies3Views6K

Jun 19, 2018
PurgeandFire

Locked

[vJASS]
 Spell ending a lot of times


kawas11

Dec 25, 2015

Triggers & Scripts

Replies1Views1K

Dec 25, 2015
Bribe

 Structs for Dummies


Magtheridon96

Jun 15, 2011

JASS Tutorials

Replies19Views26K

May 3, 2016
Zwiebelchen

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