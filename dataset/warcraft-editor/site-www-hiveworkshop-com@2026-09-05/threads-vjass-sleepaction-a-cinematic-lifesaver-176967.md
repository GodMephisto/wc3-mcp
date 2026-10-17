---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/vjass-sleepaction-a-cinematic-lifesaver.176967/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# vJASS Sleepaction a Cinematic Lifesaver
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/vjass-sleepaction-a-cinematic-lifesaver.176967/>
#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

When I was making cinematic scripts a few months ago, I was disappointed by the inaccuracies of TriggerSleepAction(), so I moved to fix that problem with Waldbaer's Exact-Timing Cinematic tutorial. Unfortunately, that looked like a mess and was getting tiresome to create the trigger queues for every wait period that I wanted to be exact.


I moved to JASS.


What happened then was the precedence-behavior of programming messed everything up in a whole new way. The TimerStart() call only compiles functions above it, which made my cinematic script look out-of-order and I was finding it hard to expand on it and to locate a specific section (because everything was in reverse-chronological order).


I tried learning Anitarf's Cinematic System but I found myself drifting further and further away from the approach I wanted to take with creating the cinematic. I wanted to have a script that looked like it was mine and would be easy for someone to pick up on.


The SleepAction library is the realization of those dreams I had. Now, thanks to a textmacro, it is an easy thing to implement a wait period. The only parts that require thinking are in the parameters passed to the macro.


Example 1:

 JASS:

scope foo initializer bar // requires SleepAction

 private function bar takes nothing returns nothing
 //! runtextmacro SleepAction("5.5", "1")
 call BJDebugMsg("Hello,") // Executed exactly 5 and a half seconds into the game.
 //! runtextmacro SleepAction("0.1", "A")
 call BJDebugMsg("World!") // Executed exactly 1/10th of a second after the last message.
 endfunction

endscope


The first of the parameters is the period you want to wait, which must be a real value or variable. Everything below the line of the textmacro will not execute until the *exact moment* the wait period has ended. Not slightly before or slightly after which you get from TriggerSleepAction() and without disorganizing and/or complicating your script to accomplish something which should not have ever been a complicated process.


The textmacro must be within a scope or library (pretty much every vJass user does this anyway) and within a vJass function (not a method or a Zinc function [Zinc users have anonymous functions to make up for all the features Zinc didn't get]). Note that local variables are not remembered after the textmacro line, though it is possible to declare new local variables after the line. How it works:


Invisible to you while scripting, the textmacro breaks the function in two. A timer gets started with the duration you specified and then, using a vJass function interface combined with TimerData from Vexorian's TimerUtils, the "second part" of the function is executed when the timer expires. For this reason, the second parameter must be a scope-unique sequence of letters and/or numbers and/or underscores as that parameter more or less gives the hidden function a name.


I find it user-friendly to just name each part a sequencial number (1,2,3...) as you scroll down. The hidden function is a private function which is prefixed with SleepAction_ and, being private, prefixed with the scope's name as well, with the second parameter's name tacked on at the end of it all.


Hopefully, this script allows you to create cinematics in an environment which is suitable for you!


Example 2:

 JASS:

 scope Scene1 initializer Init // requires SleepAction

 private function Message takes real duration, string msg returns nothing
 call DisplayTimedTextToPlayer(GetLocalPlayer(), 0.0, 0.0, duration, "|cff888888" + msg + "|r")
 endfunction

 private function Init takes nothing returns nothing

 //! runtextmacro SleepAction("0.0", "1")
 // Actions below this line happen exactly 0 seconds into the game.
 call Message(5.0, "This is an easy method to create cinematics.")

 //! runtextmacro SleepAction("5.0", "2")
 // Actions below this line happen exactly 5 seconds after the last message.
 call Message(7.0, "Using this tool, you can create perfect wait-periods without " + /*
 */"distorting the look and feel of your cinematic scripts.")

 //! runtextmacro SleepAction("7.0", "3")
 // Actions below this line happen exactly 7 seconds after the last message.
 call Message(10.0, "All you have to do is fill in the blanks:\n" + /*
 */"//! runtextmacro SleepAction(\"<Real Duration>\", \"<Scope-Unique Name>\")")

 call TriggerSleepAction(10.0)
 // Yes, you can safely mix regular TriggerSleepActions in, if you want.
 endfunction

 endscope

## Cons


I can't just butter this whole thing up. There are some real things to consider and to review from what I mentioned already.


A) The textmacro may only be enclosed in a function which takes nothing returns nothing.


B) The function may not be a method and may not be in the Zinc language.


C) The function must be contained in a scope or a library.


D) Local variables are forgotten after the textmacro line.


E) The textmacro line may not be embedded within a block such as if/then/else/endif or loop/endloop.


F) You must input a scope-unique name as a second textmacro parameter.


SleepAction script:

 JASS:

library SleepAction requires TimerUtils

 function interface SleepActionFunc takes nothing returns nothing

 globals
 private timer t = null
 endglobals

 private function Sleep takes nothing returns nothing
 set t = GetExpiredTimer()
 call SleepActionFunc(GetTimerData(t)).execute()
 call ReleaseTimer(t)
 endfunction

 function DoSleepAction takes SleepActionFunc func, real duration returns nothing
 set t = NewTimer()
 call SetTimerData(t, integer(func))
 call TimerStart(t, duration, false, function Sleep)
 endfunction

 //! textmacro SleepAction takes RealSleepDuration, FuncName
 globals
 private keyword SleepAction_$FuncName$X
 endglobals
 call DoSleepAction(SleepAction_$FuncName$X, $RealSleepDuration$)
 endfunction

 private function SleepAction_$FuncName$X takes nothing returns nothing
 //! endtextmacro

endlibrary


 Last edited: Feb 7, 2011

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Cool technique. Although a lot may prefer to do this themselves, I'm sure this will be useful to people who want to get cinematics done fast. =D


Good job, I'll detach your signature from the post.


~Approved.

#### SanKakU

#### SanKakU

 Level 13

JoinedMay 11, 2008Messages1,198

Resources: 6

#### Resources
Icons 2Maps 3Spells 1

in the middle of reviewing your code. from your explanation of the system, it looks good so far. still reviewing but it looks very promising.


by the way i noticed an error in example 2. check the first line.


also, do you know if this system is good for things besides cinematics?


i remembered when first learning this coding stuff being frustrated with timers incredibly and even now i still don't understand them entirely, even though i use them frequently. this system kind of looks like i wonder why it wasn't created sooner, if indeed it works with things besides cinematics, that is.


by the way, i think it's cool that you're using textmacro to make function name.


edit:


how are you making cinematics? can you give a bigger example? so far every time i try to use this i get errors. maybe there are some bugs, or i just don't know what i'm doing.


the problem i keep running into is variables keep becoming undeclared.


 Last edited: Feb 8, 2011

#### ZEEP

#### ZEEP

 Level 4

JoinedFeb 27, 2011Messages22

Resources: 4

#### Resources
Icons 1Maps 1Spells 2

A) The textmacro may only be enclosed in a function which takes nothing returns nothing .


D) Local variables are forgotten after the textmacro line.


Even worse than TimerStart but as you said for your cinematics may be usefull.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

I'm planning to update the script to this:

 JASS:

library SleepAction requires TimerUtils, Table

 globals
 private timer t = null
 private key theKey
 private Table s = theKey
 endglobals

 private function Sleep takes nothing returns nothing
 local integer id
 set t = GetExpiredTimer()
 set id = GetHandleId(t)
 call ExecuteFunc(s.string[id])
 call s.string.remove(id)
 call ReleaseTimer(t)
 endfunction

 function DoSleepAction takes string func, real duration returns nothing
 set t = NewTimer()
 set s.string[GetHandleId(t)] = func
 call TimerStart(t, duration, false, function Sleep)
 endfunction

 //! textmacro SleepAction takes RealSleepDuration, FuncName
 call DoSleepAction(SCOPE_PRIVATE + "SleepAction_$FuncName$X", $RealSleepDuration$)
 endfunction

 private function SleepAction_$FuncName$X takes nothing returns nothing
 //! endtextmacro

endlibrary

For straightforward cinematics in GUIJASS (I intend to use this trick to easily patch up my original WarChasers map):

Events

Conditions

Actions
Custom script: endfunction

Custom script: function DoAction takes nothing returns nothing

Custom script: call ExecuteFunc(udg_globalString)

Custom script: endfunction

Custom script: function ActionTime takes nothing returns nothing

Custom script: call TimerStart(udg_globalTimer, udg_globalReal, false, function DoAction)

Custom script: endfunction

-------- The above functions could also go into a map header

Custom script: function Phase1 takes nothing returns nothing

Set globalString = Phase2

Set globalReal = 3.00

Custom script: call ActionTime()

Custom script: endfunction

Custom script: function Phase2 takes nothing returns nothing

Set globalString = Phase3

Set globalReal = 6.00

Custom script: call ActionTime()

Custom script: endfunction

Custom script: function Phase3 takes nothing returns nothing


 Last edited: Jul 28, 2015

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

For cinematic needs, why not just create a single periodic timer and increment a count variable by one for each iteration, then use the count as the actual time-frame to refer to with a simple switch-case structure?

--> everything is ordered top-down

--> simple easily understandable code

--> almost no custom script needed, even for GUIers

--> 100% accurate

Obviously, you can't use locals here either, but in cinematics, why even use locals anyway?

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

I think I know what you mean. Like expiring once every.25 seconds calling a single function which increments a timestamp variable, and in that same function have an if/else structure to identify which phase to execute. I like that approach - thanks for sharing!

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Bribe said:

 I think I know what you mean. Like expiring once every.25 seconds calling a single function which increments a timestamp variable, and in that same function have an if/else structure to identify which phase to execute. I like that approach - thanks for sharing!


Click to expand...

It's basicly my go-to solution for creating script events of any kind where efficiency doesn't really matter.

The only downside is that it's hard to adjust timings afterwards, as you need to adjust all following timings aswell.

You can solve that easily by not having a periodic timer. Instead, make the timer function recursive:

 JASS:

globals
 private integer count = 0
 private timer t = CreateTimer()
endglobals

private function callback takes nothing returns nothing
 local real wait = 0
 set count = count + 1

 if count == 1 then
 //here be code
 set wait = 5
 elseif count == 2 then
 //here be code
 set wait = 10
 elseif count == 3 then
 //here be code
 set wait = 3
 endif

 call TimerStart(t, wait, false, function callback)
endfunction

function StartCinematic takes nothing returns nothing
 call callback()
endfunction

... when you think about it: you could actually create textmacros for that to mimic the TriggerSleepAction syntax...

And here is why this approach is so cool:

 JASS:

globals
 private constant integer LAST_PHASE = 4

 private integer count = 0
 private timer t = CreateTimer()

 private boolean skipped = false
endglobals

private function callback takes nothing returns nothing
 local real wait = 0
 set count = count + 1

 if skipped then
 set count = LAST_PHASE
 endif
 if count == 1 then
 //here be code
 set wait = 5
 elseif count == 2 then
 //here be code
 set wait = 10
 elseif count == 3 then
 //here be code
 set wait = 3

 //...

 elseif count == LAST_PHASE then
 //the final state of your cinematic... remove cinematic mode, remove units etc.
 endif

 if not skipped then
 call TimerStart(t, wait, false, function callback)
 endif
endfunction

function StartCinematic takes nothing returns nothing
 call callback()
endfunction

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Perfect. I'm gonna do that for my map. That's a lot more sane.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

and if you want extra performance juice, do binary search on the numbers

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Dropping by as I am moving this into the new sub-sections.

This tutorial inspired my "cinematic system at home" which I used in my Timewalking cinematic, it has some flaws but that's not the system's fault. Selecting cameras and pre-placed units are a pain because we do not have auto-complete for those variables.

So yeah, very useful if you want to make a cinematic but does not want to use GUI (which you really should for the smoothest experience, the drawbacks of waits do not affect cinematics that much)

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Chaosy said:

 Dropping by as I am moving this into the new sub-sections.

This tutorial inspired my "cinematic system at home" which I used in my Timewalking cinematic, it has some flaws but that's not the system's fault. Selecting cameras and pre-placed units are a pain because we do not have auto-complete for those variables.

So yeah, very useful if you want to make a cinematic but does not want to use GUI (which you really should for the smoothest experience, the drawbacks of waits do not affect cinematics that much)


Click to expand...

Honestly, for strictly-GUI systems, Perfect Polled Wait in Lua is a far-better alternative.

Additionally, this resource itself does not need to be limited to a scope or library. I can easily port it to work with GUI.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I have not used Lua - at all so I cannot really judge in either direction on that one.

I'd argue however that most cinematics do not need millisecond precision timing so normal waits are more than fine for GUI purists.

I recall that @APproject makes his cinematics in GUI and those are arguably the best we have in terms of raw quality.

Maybe I am delusional but I genuinely think the only reason to use a system like this is because you simply hate GUI with a passion and refuse to use it. If I was to make a cinematic right now I would personally use a mix of wurst and GUI (which kinda-sorta mix, wurst can talk to GUI but not the other way around) because the convenience factor is really high.

It does not matter what I think though, everyone can judge for themselves.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Chaosy said:

 I have not used Lua - at all so I cannot really judge in either direction on that one.

I'd argue however that most cinematics do not need millisecond precision timing so normal waits are more than fine for GUI purists.

I recall that @APproject makes his cinematics in GUI and those are arguably the best we have in terms of raw quality.

Maybe I am delusional but I genuinely think the only reason to use a system like this is because you simply hate GUI with a passion and refuse to use it. If I was to make a cinematic right now I would personally use a mix of wurst and GUI (which kinda-sorta mix, wurst can talk to GUI but not the other way around) because the convenience factor is really high.

It does not matter what I think though, everyone can judge for themselves.

Click to expand...

The main reason for me for exact waiting is to time cuts correctly. Also the minimum wait period of 0.25 seconds (even if one specifies 0.00) makes it impossible to rely on rapid events.

Take my WarChasers II map in the Maps section as an example. Exact timing would have really helped.

Here's an untested script showcasing how this would work with GUI:

 JASS:

library SleepAction initializer Init requires TimerUtils, Table

 globals
 private timer t = null
 private Table tb
 private integer i
 endglobals

 private function Sleep takes nothing returns nothing
 set t = GetExpiredTimer()
 set i = GetHandleId(t)
 call ExecuteFunc(tb.string[i])
 call tb.string.remove(i)
 call ReleaseTimer(t)
 endfunction

 function DoSleepAction takes string funcName, real duration returns nothing
 set t = NewTimer()
 set tb.string[GetHandleId(t)] = funcName
 call TimerStart(t, duration, false, function Sleep)
 endfunction

 //! textmacro SleepAction takes RealSleepDuration, FuncName
 call DoSleepAction("SleepAction_$FuncName$X", $RealSleepDuration$)
 endfunction
 function SleepAction_$FuncName$X takes nothing returns nothing
 //! endtextmacro

 private function Init takes nothing returns nothing
 set tb = Table.create()
 endfunction

endlibrary


 Last edited: Nov 27, 2021

 You must log in or register to reply here.


### Similar threads

 [vJass] SleepAction - The new "Wait"


Bribe

Sep 7, 2010

Graveyard

Replies14Views5K

Sep 23, 2010
azlier

Locked

[JASS]
 [PROVED] Parameters are faster than declaring/nulling locals


Bribe

Apr 21, 2010

Triggers & Scripts

Replies22Views5K

Apr 23, 2010
D4RK_G4ND4LF

Locked

[vJASS]
 //! textmacro_once


NEL

Dec 10, 2018

Triggers & Scripts

Replies2Views2K

Dec 11, 2018
NEL

Locked

[Spell]
 vJass beginning


kawas11

Dec 25, 2014

World Editor Help Zone

Replies6Views2K

Dec 25, 2014
IcemanBo

Locked

[vJASS]
 How to use vJass without vJass Class Core System?


xorkatoss

Jan 6, 2013

Triggers & Scripts

Replies8Views2K

Jan 7, 2013
Nestharus

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