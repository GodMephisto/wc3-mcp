---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/recursion-safety.238740/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Recursion Safety
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/recursion-safety.238740/>
#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Recursion Safety

Table of Contents

The Problem

The Solution

Wrap-Up

The Problem


Recursion-safety is often needed in cases where recursive execution of a function is possible


and harmful to an array of system data. Consider this example:

 JASS:

function OnDamage takes nothing returns nothing
 set source = GetEventDamageSource()
 set target = GetTriggerUnit()
 set amount = GetEventDamage()

 call damageEvent.fire()

 set source = null
 set target = null
 set amount = 0
endfunction


This is a function inside a damage detection system that runs whenever damage is dealt.


Take note of the 5th line of code that fires the event. This line of code will execute


all functions registered to the system to run upon a damage event. What if one of those


functions dealt damage to a unit? You guessed it, this OnDamage function would be called


again and it would refire all the functions registered to the system.


Sometimes, if there are no conditions for dealing damage in the registered functions,


this would lead to an infinite loop. When there are conditions, this leads to a recursion


that would repeat a finite number of times.


Do note that upon the first recursion, the source, target and amount variables will be


changed, so all the functions that still haven't been executed by the damageEvent.fire()


call would be using different source/target/amount information. This also affects the


function that dealt the damage. By dealing damage, you are changing the source, the


target and the amount.

The Solution


The solution to keep the data valid is to preserve the previous values before setting new ones


and restoring them after the functions are executed. We have to wind and unwind a stack. There are


two approaches:


1) The array approach: Instead of using regular variables to store our data, we can use


arrays. We'd have to have an index to represent the stack depth that we increment each time we


execute the function. We'd use that index when writing data to the arrays and subsequently,


reading data from the arrays:

 JASS:

function OnDamage takes nothing returns nothing
 set stackDepth = stackDepth + 1
 set source[stackDepth] = GetEventDamageSource()
 set target[stackDepth] = GetTriggerUnit()
 set amount[stackDepth] = GetEventDamage()

 call damageEvent.fire()

 set source[stackDepth] = null
 set target[stackDepth] = null
 set amount[stackDepth] = 0
 set stackDepth = stackDepth - 1
endfunction


This works quite well, but unfortunately, it's not the best solution. In fact, it's a bad one.


2) The locals approach: We can avoid arrays and stack depths altogether by taking advantage


of the automatic storage facilities provided by JASS. Yes, that's a fancy phrase for local variables.

 JASS:

function OnDamage takes nothing returns nothing
 local unit previousSource = source
 local unit previousTarget = target
 local real previousAmount = amount

 set source = GetEventDamageSource()
 set target = GetTriggerUnit()
 set amount = GetEventDamage()

 call damageEvent.fire()

 set source = previousSource
 set target = previousTarget
 set amount = previousAmount

 set previousSource = null
 set previousTarget = null
endfunction


This is quite an effective solution because the winding and unwinding is abstracted. The handle


pointers are managed by the JASS VirtualMachine rather than our JASS code.

Wrap-Up


This concludes my tutorial on Recursion-safety in JASS. Stay in school.


~Magtheridon96

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

YAY! A recursion safety tutorial

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Approved. Sorry for the delay, I was thinking about a different approach but eh, I kind of forgot what I had in mind.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Don't worry Purge, no need to rush, this tutorial wasn't going anywhere anyway <:

Also, I completely forgot about it

 You must log in or register to reply here.


### Similar threads

 [vJASS] Dynamic Indexing Tutorial


Malhorne

Dec 25, 2013

JASS Tutorials
2

Replies58Views16K

Jul 13, 2015
Zwiebelchen

Locked

 Function Call and Recursion Problem


chopinski

Feb 27, 2019

Triggers & Scripts

Replies22Views4K

Mar 2, 2019
MyPad

Locked

 How to calculate a unit's armor with a skill.


John_Syin

Jul 13, 2012

Triggers & Scripts

Replies2Views1K

Jul 13, 2012
defskull

Locked

 HealEvent


Bribe

Sep 30, 2015

Advanced Scripting

Replies15Views4K

Oct 26, 2015
pick-a-chew

 [System] Instance DD


Kingz

Jun 20, 2012

Graveyard

Replies9Views2K

Jun 23, 2012
Bribe

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