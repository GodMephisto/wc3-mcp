---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/handling-timer-imprecision.321276/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Handling Timer Imprecision
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/handling-timer-imprecision.321276/>
#### _Guhun_

#### _Guhun_

 Level 12

JoinedJun 12, 2010Messages413

Resources: 17

#### Resources
Models 1Maps 1Spells and Systems 9JASS 4Tutorials 2

This tutorial is a work in progress. I'll finish it up as I manage to put aside some time to do so.

Timers in JASS can be pretty imprecise. The reason for this is the fact thay floating point numbers are used, which are inherently imprecise. One important thing to know in order to understand this tutorial is that floating point numbers can only represent integers and fractions whose divisor is a power of 2 (1/2, 1/4, 7/4, etc.) with 100% precision. Every other number is represented through approximation.

Ilustrating Imprecision

However, normal floating point imprecision does NOT account for timer imprecision. Let's see an example:

As soon as my ticks reach 128 (2 to the power of 8), timer precision greatly decreases. The values are now being incremented by 1008, instead of 1000. However, my timer which was executing in a period that is a power of two is still fine when it comes to precision.

When we reach 256 ticks, precision dips again. This is the maximal imprecision that we will reach for 0.001. Now each timer will execute 24 more times than normal, in other words, it will execute 1024 times per second. This is the closest power of 2 to 1000, so our precision should not go lower for any practical amount of game time.

Handling Imprecision

If you don't want the precision of your timers to vary with time, then you must use a period that is perfectly represented in floating point. The best way to go about this is using fractions.

Good vs bad periods:

1/1024. ~ 0.001

1/128. ~ 0.01

1/64. ~ 1/60. (60 FPS)

 JASS:

scope aaaaa

globals
 private integer seconds = 0
 private integer milis = 0
 private integer centis = 0
 private integer milis2 = 0
 private integer tracker = 0
 private integer tracker2 = 0

 private integer lastMilis = 0
 private integer lastCentis = 0
 private integer lastMilis2 = 0
 private timer clock
 private timer clock2
endglobals

private function onMiliPow2 takes nothing returns nothing
 set milis2 = milis2 + 1
endfunction

private function onMili takes nothing returns nothing
 set milis = milis + 1
endfunction

private function onCenti takes nothing returns nothing
 set centis = centis + 1
endfunction

private function onSecond takes nothing returns nothing
 local integer miliImprecision = milis - lastMilis - 1000
 local integer centiImprecision = centis - lastCentis - 1000
 local integer mili2Imprecision = milis2 - lastMilis2 - 1024

 set lastMilis = milis
 set lastCentis = centis
 set lastMilis2 = milis2

 set seconds = seconds + 1
 call TimerStart(clock, 0.001, true, function onMili)

 call BJDebugMsg(I2S(seconds) + "Imprecision: ")
 call BJDebugMsg(I2S(centis) + "Imprecision: " + I2S(miliImprecision))
 call BJDebugMsg(I2S(milis) + "Imprecision: " + I2S(centiImprecision))
 call BJDebugMsg(I2S(milis2) + "Imprecision: " + I2S(mili2Imprecision))
endfunction

//===========================================================================
function InitTrig_Imprecie_Timers takes nothing returns nothing
 set clock = CreateTimer()
 set clock2 = CreateTimer()
 call TimerStart(clock, 0.001, true, function onMili)
 call TimerStart(clock2, 0.001, true, function onCenti)
 call TimerStart(CreateTimer(), 1/1024., true, function onMiliPow2)
 call TimerStart(CreateTimer(), 1., true, function onSecond)
endfunction

endscope

#### Attachments

WC3ScrnShot_010720_070616_02.png

 3.2 MB

 · Views: 105



WC3ScrnShot_010720_071107_03.png

 3.2 MB

 · Views: 99



WC3ScrnShot_010720_072101_04.png

 669.5 KB

 · Views: 90



#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Quick thoughts.

1. Make the image smaller or put it in a hidden tag

2. Indenting after title usage would be nice

2b. Adding a splash of color would not be bad either

3. Test map would also be w elcome

4. Code inside the scope should be indented one step further, I believe.

5. Good vs bad periods should be put in a table perhaps?

 You must log in or register to reply here.


### Similar threads

Locked

[JASS]
 Imprecision


Volvox

May 29, 2008

Triggers & Scripts

Replies16Views3K

May 31, 2008
Silvenon

 Multi-unit Instanceability With Timers (vJASS)


busterkomo

Jul 28, 2009

Tutorial Archive

Replies8Views2K

Jan 18, 2010
Kwah

Locked

 Array Number of Expired Timer?


Pinzu

Jul 24, 2014

Triggers & Scripts

Replies8Views2K

Jul 25, 2014
Dr Super Good

Locked

 Zero Second Periodic Timers


TriggerHappy

Mar 12, 2014

Advanced Scripting

Replies11Views4K

Jun 13, 2014
Troll-Brain

Locked

[JASS]
 Multiboard Timer


Dreadnought[dA]

Oct 1, 2008

Triggers & Scripts

Replies2Views1K

Oct 3, 2008
PurplePoot

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