---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/mui-triggers-with-waits.218354/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Mui Triggers with Waits
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/mui-triggers-with-waits.218354/>
#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

MUI Triggers with Waits

Disclaimer: This is not an acceptable form for Spell/System resources.

It is only meant for map-making.

Go here instead.

Table of Contents

Introduction

What is 'MUI'?

The Problem

The Solution

Pros and Cons

Delayed MUI Spells Without Waits

Wrap-Up

Introduction

Good morning reader, or if it's not morning at the moment

while you're reading this, good evening! Then again, it might

be just around noon, so in that case, Good day! What I'm

trying to say is... Bah, how about a simple "Hello"? That

applies to all readers in all timezones and places at all times.

Hello.

Since you might be new here, I'm going to explain to you exactly

why I made this tutorial. If you've never made a spell or a system

before (Yes! You can actually make custom spells with Warcraft III

and custom systems. Cool, right?), then you're probably psyched

right about now after reading the previous text in parenthesis.

Before I move on, if your understanding of the French language is

far more developed than your understanding of English, I'd recommend

this made by our very own Troll-Brain.

What is 'MUI'?

MUI is an acronym for "Melons United International". It is a secret

organization bent on the annihilation of the human race so that

melons may take over and CLAIM POWER THEN VOYAGE TO THE STA-

Oh, sorry, forget everything I said, I didn't know we were talking

about JASS. Anyway, MUI means "Multi-unit Instanceability". It's

an attribute we give to a spell or a system indicating that it can

actually work for more than one unit. In the spell case, it means

that two units can cast the spell at the same time and still have

it working. In the systems case, it means that it can handle two

units or more.

MPI means the same thing, except it refers to player-handling.

(And certainly not Mango world domination or anything like that.

What a silly thought, why are you even asking? LET'S MOVE ON

SHALL WE?)

An example of a system that needs to be MPI is a Custom Fullscreen

Inventory system. (If you're new here and you're reading this, you

probably need new pants right about now. It's okay, I'll reserve the

rest of the information for the next section. Go on, you may discard

your current pair, I'M CERTAINLY NOT WATCHING YOU.)

... Well? Move along now.

The Problem

As a 14-year old child with a dream to become the best spell-writer

ever, I came to Hive so I can submit a resource of mine. That day,

I learned of a new dark side to something I've loved for a very long

time.

I started Warcraft III modding when I was an 11-year old child. One

of my favorite GUI actions was the "Wait" action. I used it excessively,

but on that day, I learned that it's one of the worst things you could

use in a trigger.

If you're new, this is probably news for you:

Using waits can make a trigger Non-MUI in a lot of cases

Terrible, right? Crying won't help, and isn't very necessary, after all,

I didn't make this tutorial to speak of the problem, I made it to address

and fix the problem.

Yes! There's hope!

The Solution

I'm going to teach you how to make YOUR triggers MUI in 3 easy steps!

Sorry, I didn't mean to make it sound like a Penal Enlarging Ad, this is a GUI

tutorial, so let's get back on topic, shall we?

Okay, for new users, this is going to seem difficult, but it's actually

very simple. I'm going to introduce you to the concept of the solution

first.

Do you know what a buffer is?

A buffer is basically an array of data. I can write data to a buffer,

then read it in the same order that I wrote it.

What we're going to do in order to address the Wait-MUI problem is

use a buffer for all the data. The main reason this problem arises is

because of data modifications. There are certain things in GUI that

remain unchanged after waits, and others that change depending on

what's going on in the map.

An example of something that remains constant: (Triggering unit)

An example of something that changes: (Last created unit)

The triggering unit is different for each trigger execution, so it remains

constant, but the last created unit is pretty much a variable that gets

set every time you create a unit with the BJ function (meaning the GUI

create unit action).

Using (Triggering unit) as is will work after a "Wait", but since you're

often going to store that into a unit variable, it's not going to work well.

Let's begin.

Bad Trigger
Events
Unit - A unit starts the effects of a spell

Conditions
(Ability being cast) Equal to MyDerpyAbility

Actions
Set caster = (Triggering unit)

Wait 5.00 seconds

Unit - Remove caster from the game

This trigger will not work correctly if our ability is cast twice in a row with an interval less than 5 seconds.

Let's investigate why, shall we?

What's going on?

Imagine we have two units on the map, just hanging out, smoking weed,

and appreciating the beauty of nature high-poly models.

The first unit decides to cast a spell (MyDerpyAbility)

Thus, currently, in the trigger, caster is equal to the first unit because

he casted the spell.

3 seconds later, the trigger is still waiting because of the 5-second wait.

Suddenly, the second unit decides to cast the spell in an attempt to troll

the first unit.

Guess what happens! The caster variable in the trigger is going to be equal

to the second unit!

After 2 more seconds, the 5-second wait for the first cast finishes, so the

trigger is going to remove the unit stored in the caster variable from the game!

This is why spells are not MUI when you use waits in them and include variables.

The second unit's trolling scheme fails as he is removed from the game.

After 3 seconds, the second 5-second wait will finish and the unit stored in the

caster variable will be removed from the game. (And it happens to be the second

unit too). So nothing actually happens because he was already removed.

See how that doesn't work?

This is pretty much what happens in almost every other case.

The solution is to make the data read on each execution a different source.

But how do we do that? The answer, my friend, is an array, which we will use as a buffer.

Pro Trigger
Events
Unit - A unit starts the effects of an ability

Conditions
(Ability being cast) Equal to MyDerpyAbility

Actions
-------- WriteIndex and ReadIndex are integer variables --------

Set WriteIndex = (WriteIndex + 1)

Set caster[WriteIndex] = (Triggering unit)

Wait 5.00 seconds

Set ReadIndex = (ReadIndex + 1)

Unit - Remove caster[ReadIndex] from the game

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ReadIndex Equal to WriteIndex

Then - Actions
Set ReadIndex = 0

Set WriteIndex = 0

Else - Actions

This trigger will execute perfectly.

What's going on?

Let's imagine that the two units are in the same state.

Just hanging around, smoking weed, etc...

The first unit decides to cast the spell.

At this moment, the WriteIndex variable will be equal to 1.

Hence, caster[1] will be equal to the first unit.

After 3 seconds, the second unit decides to cast the spell.

At this moment, the WriteIndex variable will be equal to 2.

Hence, caster[2] will be equal to the second unit.

After 2 seconds, the 5-second wait for the first unit's spellcast

ends. The ReadIndex variable is increased by 1, so it becomes 1.

Thus, caster[1] will be removed from the game.

The trigger continues to check if ReadIndex is equal to WriteIndex.

Currently, WriteIndex is 2 and ReadIndex is 1. When this happens,

it means that one of the units is still waiting.

After 3 more seconds, the 5-second wait for the second unit's

spellcast ends. The ReadIndex will be increased by 1 and become 2.

Thus, caster[2] will be removed from the game.

The trigger continues to check if ReadIndex is equal to WriteIndex.

Both of them equal 2, so they are both reset to 0.

We don't really need to check if they're equal so we can set them

to 0, but we're doing this because, well, you may or may not know this,

but arrays have a limit. The highest index you could use for an array is

8191. And due to a bug with Game-saving, that limit is cranked down

to 8190. So for all of you who've been trying to use 9001 as an index,

I'M SORRY, BUT THAT'S NEVER GOING TO WORK ;_;.

How-to

 For every piece of data you want to store and retrieve after the wait, you need to use an array.

 You need two integers, one to increase and use as an index before the wait and before setting the variables,

and one to increase and use after the wait and before reading the variables. (WriteIndex and ReadIndex were used in the examples)

 Don't forgot to check if these two integers are equal at the end of the trigger so you can reset them to 0.

Pros and Cons

Waits have both pros and cons:

Pros

 They are easy-to-use and clear.

Cons

 They are inaccurate (The actual wait time depends on the latency meaning that waits on Garena take much longer than waits on LAN

:3

)

 Wait times vary with an offset range of [+0.098 ... +0.2] seconds in the best case.

 Waits don't 'wait' when the game is paused. (Disputable)

 In JASS, when called inside Conditions, they crash the thread.

 The wait time with this method must be a constant. (Thank you IcemanBo for pointing this out)

 You either store all state and every last bit of event-response data that you want to use after the wait code, or you store absolutely nothing and depend on the current execution context of the trigger.

(This means that using something like (Triggering Unit) after the wait action will give you incorrect results if you used a buffer to store data for the instances. You either use the buffers to store absolutely everything, or you use no buffers at all.) (Thank you again, IcemanBo for helping me make this realization)

As you can see, the cons outweigh the pros.

This is why I only recommend using the method outlined in this

tutorial for map-making only. Public resources using this may be

rejected for using this method.

(Melon United International has absolutely nothing to do with this, I can assure you.

THEY DON'T EXIST I SWEAR.)

Delayed MUI Spells Without Waits

Tutorial Moved Here

Wrap-Up

I hope this tutorial has helped you with any difficulties you're facing. If you have any

questions, feel free to ask. Also, I wouldn't recommend using this method very often in

public resources. This method is only for map-making. The cons that waits have outweigh

the pros, hence it's up to the map-maker to decide what he wants to do. Thank you for

reading this tutorial. I hope you learned a thing or two. (Actually, I hope you learned

nothing, and that's a compliment because it implies that I believe you know a lot :3).

~Magtheridon96


 Last edited: Sep 20, 2013

#### Geries

#### Geries

 Level 13

JoinedSep 13, 2010Messages550

Resources: 7

#### Resources
Models 6Packs 1

 As a 14-year old child


Click to expand...

You are still 15 :> BTW this is a nice tutorial. It will be nicer if you explain what is the actual danger with waits :>

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Well, I'm going to be 16 in 5 days now D:

I did indicate the pros and cons of waits, but if you want me to mention how they affect JASS too, then sure.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

A pretty entertaining tutorial.  And it is a useful technique.

I don't see anything wrong with it, so I'll approve.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Thank you PurgeandFire

Updated to fix a typo and I lengthened the Wrap-Up in order to make it even more entertaining.

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

You are actually really bored, aren't you ?

Here is for you a simple "tutorial" about GUI MUI stuff in french.

I link it because i've tried all GUI response events to know if they are MUI or not.

Yes, all of them ! And it was back to the days where i was really accurate, ofc i'm just an human after a troll, so some nasty errors/lacks could still be hidden somewhere.

If you don't understand something i will traduct it for you.

And about TSA, iirc it depends the delay, a TSA in a solo game should be faster than the same TSA in a multiplayer game (even lan game).

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Yes, all event responses are local to the scope.

However, that doesn't mean this technique is useless. In his examples, it probably is because you can just use triggering unit instead.  However, for things that don't deal with event responses (timed effects are a good example of this), you could benefit form this technique.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

 we were talking about JASS


Click to expand...

no, we're talking about MUI Triggers with Waits...XD...

 pretty much a global variable that gets set every time you create a unit with the BJ function


Click to expand...


=====


The What's going on is really confusing for the new user, just put it in a trigger example like;


If 2 units casting;

Set caster = (Triggering unit)

Wait 5.00 seconds

Comment "The second unit will be removed, the first will probably live forever (good for him)"

Unit - Remove caster from the game

 Waits don't 'wait' when the game is paused. (Disputable)


Click to expand...

well someone claim in https://www.hiveworkshop.com/forums/spells-569/simple-timed-sfx-system-v1-2-gui-218338/ that TriggerSleepAction does obey the Game Pause.

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

PurgeandFire111 said:

 Yes, all event responses are local to the scope.


Click to expand...

I've never said the opposite, but it's not true, if you call a function from within the local scope, the event response function should still return the right value.

 However, that doesn't mean this technique is useless. In his examples, it probably is because you can just use triggering unit instead.  However, for things that don't deal with event responses (timed effects are a good example of this), you could benefit form this technique.

Click to expand...

Sure, but if the native response event is MUI by itself, no need to overcomplicate it just because it's GUI

That's why i've tested all responses event available in GUI.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

 no, we're talking about MUI Triggers with Waits...XD.


Click to expand...

Oh, right.

Sorry, when I made this, it was 1:50 AM and I was pretty tired that day.

 And about TSA, iirc it depends the delay, a TSA in a solo game should be faster than the same TSA in a multiplayer game (even lan game).


Click to expand...

True. TSA is a Sync-class native (You know, like all those other silly willy Sync natives Nestharus and geries are yammerin' about).

 well someone claim in Simple Timed SFX System v1.2 [GUI] that TriggerSleepAction does obey the Game Pause.


Click to expand...

Yeah, I saw that post the other day. Hence, I wrote (Disputable) next to that con, and I'm going to test it today when I get home.

 The What's going on is really confusing for the new user, just put it in a trigger example like;


Click to expand...

I guess I /can/ make it a bit easier for new users.

 You are actually really bored, aren't you ?


Click to expand...

Why yes.

edit

Okay, updated.

It's now 3% cooler \o/

That number is totally made up :bitchplease:


 Last edited: Jun 28, 2012

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1


 Magtheridon96 said:



 True. TSA is a Sync-class native (You know, like all those other silly willy Sync natives Nestharus and geries are yammerin' about).


Click to expand...

What i meant is that the 0.27 minimum value seems a magic value from nowhere, how it was determined ?

More, in a solo game at least iirc sometimes a TriggerSleepAction is randomly quite ligthning fast, like a Timer(0) or near, but again tests are needed.

 Yeah, I saw that post the other day. Hence, I wrote (Disputable) next to that con, and I'm going to test it today when I get home.


Click to expand...

If i remember correctly the actual wait (already started) still end when the game is paused but not new ones (which would start during the pause), ofc a test is definetely needed.

Also in some case you need a TSA, like if you want to pause the game for X seconds, you can't use a timer.

 I guess I /can/ make it a bit easier for new users.


Click to expand...

Only few gui reponse events (Get...) are not MUI, despite everybody say the opposite, i've listed all of them in the link i've given.

So basically in GUI most of time you even don't need to use a variable for these responses, it's not like efficiency really does matter in GUI, or it wouldn't be GUI, simplicity is the GUI leitmotiv.

So you should really mention them.

 Why yes.


Click to expand...

Because you're making a GUI tutorial

edit

Okay, updated.

It's now 3% cooler \o/

That number is totally made up :bitchplease:

Click to expand...

Add the list of the not MUI response events available in GUI and it will be 100 % cooler

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

This will be useful to starters for sure ^_^

Damn I lol'd when I finished with first 2 paragraphs.

#### Geries

#### Geries

 Level 13

JoinedSep 13, 2010Messages550

Resources: 7

#### Resources
Models 6Packs 1

The functions which are non-MUI in GUI:

Everything... I repeat: EVERYTHING whose name do not start with "Event Response - " are surely not safe to use after a Sleep Action or an Execute Trigger( Yes those can cause troubles too ). I think everything else( including "Conversion - " stuff ) can be used safely.

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

And when my statement was debated somewhere else, I didn't get noted

I don't know, for me (that thread situation) the Wait does really obey the game pause, I don't really need to explain it all the procedure of how I do it, you can read it on that thread.

Also, I don't wanna say that the statement is a fact, because perhaps I did some mistakes in that test ?

You guys should test it, and should vote for majority whether that is a fact or not.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Okay, tonight, I will test it.

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

Why not now ?

Just a few seconds

Oh wait, you're writing a softwareeee haha jokin`

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Yes, I am. I'm at work at the moment.

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

You already got a job ?

15-years old, seriously ?

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

It's a summer-job

Plus, I'm making a loooot of money  (My grandfather owns the company)

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

This tutorial helps the new user a lot about waits and they will probably say, "to hell with indexing and hashtables,


I'm too lazy to learn to it, waits are simple and provides less code"...

 My grandfather owns the company


Click to expand...

wow!...I hope I see a donation icon below your name Mags, joke ...

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

 This tutorial helps the new user a lot about waits and they will probably say, "to hell with indexing and hashtables,

I'm too lazy to learn to it, waits are simple and provides less code"...


Click to expand...

And that's exactly why I'm telling them not to use it in public resources

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

geries said:

 The functions which are non-MUI in GUI:

Everything... I repeat: EVERYTHING whose name do not start with "Event Response - " are surely not safe to use after a Sleep Action or an Execute Trigger( Yes those can cause troubles too ). I think everything else( including "Conversion - " stuff ) can be used safely.


Click to expand...

This is an other false wc3 myth, i repeat myself i've tested all event responses available in GUI, only few are not MUI, and i've listed them.

The issue with ExecuteTrigger is probably about GetTriggeringTrigger and such natives, but i've never really used TriggerExecute so it could have more issues with that, but it's quite off-topic with this subject anyway.

EDIT :

Ok i've read it too quickly, i don't know why you're talking about anything else than event response, but yes it seems obvious, or not maybe since it's a tutorial for beginners after all.

But as said there are few response events which are not MUI, i've listed them and it's easy to test (but boring)

#### maddeem

#### maddeem

 Level 24

JoinedJan 1, 2011Messages1,506

Resources: 34

#### Resources
Models 5Icons 3Skins 1Maps 6Spells and Systems 18Lua 1

Ahh this would have helped me so much about a year ago.

The way you made that trigger MUI with waits was really intriguing.

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

So mag, how's the test ?

Wait obey Game Pause

Click to expand...

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

I'm quite sure it acts like i said, current waits are not halted but no more TSA are started while the game is paused.

It makes sense, else you coudn't unpause the game after X seconds of pausing it.

But yeah a test is needed.

And btw deskfull nothing forbids you to test it yourself

Or like me you don't have wc3 installed anymore, and you're still here for the nostalgy ?

EDIT :

Meh i wanted to do several tests about wc3 so i've reinstalled it and yes TSA are not halted during pause, at least with the patch 1.26a :

 JASS:

function Act takes nothing returns nothing
 local integer i = 0
 call TriggerSleepAction(10)
 call BJDebugMsg("let's start the pause")
 call PauseGame(true)
 call TriggerSleepAction(5)
 call PauseGame(false)
 call BJDebugMsg("end")
endfunction

//===========================================================================
function InitTrig_test takes nothing returns nothing
 local trigger trig = CreateTrigger( )
 call TriggerAddAction( trig, function Act )
 call TriggerExecute(trig)
endfunction

 JASS:

function Act2 takes nothing returns nothing
 local integer i = 0
 loop
 set i = i+1
 call TriggerSleepAction(1)
 call BJDebugMsg(I2S(i))
 exitwhen false
 endloop

endfunction

//===========================================================================
function InitTrig_test2 takes nothing returns nothing
 local trigger trig = CreateTrigger( )
 call TriggerAddAction( trig, function Act2 )
 call TriggerExecute(trig)
endfunction

During the pause it keeps to spam an integer each second, so no TSA don't obey to the pause at all, and not "sometimes" like i said.

I've tested this code on lan and deskfull now i've read you better (damn i really read to quickly these times ...), your test is probably flawed in the sense that the spell was probably not effective (instant) when the game was paused, or share your code.

Or it's also totally possible that effects are not played during the pause, for example death animations are played for the player X only when he has his view on it, you can test it easily, create/destroy an effect where you don't actually have your view, wait for a while and then go where the effect was created/detroyed, you will see the dead animation.


 Last edited: Jul 5, 2012

#### fodorgyuri666

#### fodorgyuri666

 Level 2

JoinedMar 19, 2012Messages7

Good, but for units I usually use player number as variable index, that will never rise up to 8191 .

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

But sometimes, you might have 2 units owned by the same player casting the same spell

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

fodorgyuri666 said:

 Good, but for units I usually use player number as variable index, that will never rise up to 8191 .


Click to expand...

What you are talking is called mpi : Multi player Instanceability

This inferior thing is only is GUI because in jass it's incredibly easy to make a trigger mui, just with one local variable.

Well, not that easy in fact if you want it to obey game pause and be accurate, but that's an other story.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Updated.

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

^

Is that supposed to be "bitch please" reaction artist version ? ;p

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Actually, it's because I put a tutorial in a tutorial xD

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

I tried to follow what Tank-Commander did on "Using a Counter" method but it seems when I cast 3 abilities at different time (each interval is 1 second per cast) and it's working great handling movement per unit and duration per unit until the first cast unit has run out of duration and the spell would end, and the first cast unit has stopped moving, this is intended for that ability BUT, when first cast unit stops moving (run out of duration), it affects second cast and third cast unit whereas they still have remaining duration like 2 or 3 seconds left for their movement, it's like they share the same counter and the counter being a reference is the first cast unit's counter.

I've attached a test map, try look at it and see what's the problem ?

For those who don't want to download, you can inspect from these triggers;

Cast
Events
Unit - A unit Starts the effect of an ability

Conditions

Actions
Set MaxIndex = (MaxIndex + 1)

Game - Display to (All players) the text: (String(MaxIndex))

Set TempUnit[MaxIndex] = (Triggering unit)

Set Counter[MaxIndex] = 5.00

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
MaxIndex Equal to 1

Then - Actions
Trigger - Turn on Loop <gen>

Else - Actions

Loop
Events
Time - Every 0.03 seconds of game time

Conditions

Actions
For each (Integer CurrentIndex) from 1 to MaxIndex, do (Actions)
Loop - Actions
Set Counter[CurrentIndex] = (Counter[CurrentIndex] - 0.03)

Floating Text - Create floating text that reads (String(Counter[CurrentIndex])) above TempUnit[CurrentIndex] with Z offset 0.00, using font size 10.00, color (100.00%, 100.00%, 100.00%), and 0.00% transparency

Floating Text - Change (Last created floating text): Disable permanence

Floating Text - Change the lifespan of (Last created floating text) to 0.03 seconds

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Counter[CurrentIndex] Greater than 0.00

Then - Actions
Set TempLoc = (Position of TempUnit[CurrentIndex])

Set TempLoc2 = (TempLoc offset by 5.00 towards (Facing of TempUnit[CurrentIndex]) degrees)

Unit - Move TempUnit[CurrentIndex] instantly to TempLoc2

Custom script: call RemoveLocation(udg_TempLoc)

Custom script: call RemoveLocation(udg_TempLoc2)

Else - Actions
Set TempUnit[CurrentIndex] = TempUnit[MaxIndex]

Set MaxIndex = (MaxIndex - 1)

Game - Display to (All players) the text: (String(MaxIndex))

Set CurrentIndex = (CurrentIndex - 1)

Game - Display to (All players) the text: (String(CurrentIndex))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
MaxIndex Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Game - Display to (All players) the text: turn off

Else - Actions

The main issue is: It's like all units in the index shares the same counter (duration) by making first cast unit's duration as their reference. So when first cast unit has stopped moving, all other units will stop too.

I just noticed counter needs to be de-index too (is it correct ?), so I did this in addition to the code;

Set Counter[CurrentIndex] = Counter[MaxIndex]
Now it has unique timer per unit, yipee !

But Mag, at your first post, under Tank-Commander section about using Counter, you did not mention this, about de-index the counter for each instance ?

Are you forgetting this code or what ?

Here's the code I was mentioning/following;

Cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Animate Dead

Actions
Set MaxIndex = (MaxIndex + 1)

-------- Here's the bit we had in the first place before the wait --------

Set TempUnit[MaxIndex] = (Triggering unit)

Set TempPoint[MaxIndex] = (Position of TempUnit[MaxIndex])

Set Counter[MaxIndex] = 0

-------- ----------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
MaxIndex Equal to 1

Then - Actions
Trigger - Turn on Loop <gen>

Else - Actions

Loop
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
For each (Integer CurrentIndex) from 1 to MaxIndex, do (Actions)
Loop - Actions
Set Counter[CurrentIndex] = (Counter[CurrentIndex] + 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Counter[CurrentIndex] Equal to 2

Then - Actions
-------- Here's the bit we had in the first place after the wait --------

Set TempGroup = (Units within 200.00 of TempPoint[CurrentIndex])

Unit Group - Pick every unit in TempGroup and do (Actions)
Loop - Actions
Unit - Cause TempUnit[CurrentIndex] to damage (Picked unit), dealing 50.00 damage of attack type Spells and damage type Normal

Custom script: call DestroyGroup(udg_TempGroup)

-------- Recycling --------

Custom script: call RemoveLocation(udg_TempPoint[udg_CurrentIndex])

Set TempPoint[CurrentIndex] = TempPoint[MaxIndex]

Set TempUnit[CurrentIndex] = TempUnit[MaxIndex]

Set MaxIndex = (MaxIndex - 1)

Set CurrentIndex = (CurrentIndex - 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
MaxIndex Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

Else - Actions

#### Attachments

Indexing 1.w3x

 14 KB

 · Views: 372



#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

My mistake.


Fixed.

#### iceteaftw

#### iceteaftw

 Level 4

JoinedAug 2, 2012Messages20

Resources: 3

#### Resources
Icons 1Spells and Systems 2

Thanks Magtheridon!! It helped me so much

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

The lowest possible wait time isn't 0.27. You can do this test with a timer, TSA, and timer get elapsed. The lowest I've received was 0.09875, but it will really vary. The number 0.27 is just a myth from the past.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Yeah, I learned that a while ago.


This photo has been in my album for quite a while

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

With wait 0.00 you can delay something for 0.10 - 0.15 seconds.

That's enough to allow something like this:

Untitled Trigger 001
Events
Unit - A unit Starts the effect of an ability

Conditions

Actions
Wait 0.00 seconds

Unit - Move (Triggering unit) instantly to (Center of (Playable map area))

If we remove wait, trigger action will break spell cast, mana, cooldown won't be effected.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

I'm only guessing here, but it's probably different if you use SetUnitX/SetUnitY.


SetUnitPosition has a lot of extra operations done internally.


First, it sets x and y, then it does some collision operations, and the rest is stuff I do not know of.


Possibly order canceling because of what you showed us here Kobe <:

#### ELECTROLYTE

#### ELECTROLYTE

 Level 6

JoinedApr 1, 2009Messages201

Loved this tutorial, its the first mui one I could understand and use. Works perfectly

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

Magtheridon96 said:

 Yeah, I learned that a while ago.


This photo has been in my album for quite a while

Click to expand...


The "0.09998321" is imba

#### Kaoskid

#### Kaoskid

 Level 1

JoinedSep 8, 2012Messages5

I tried to implement this on my spell but i keep failing :X what am i doin wrong?

Set WriteIndex = (WriteIndex +1)

Set CurseCaster[WriteIndex] = (Triggering uit)

Set CurseTarget[WriteIndex] = (Target unit of ability being cast)

Set ReadIndex = (ReadIndex +1)

Wait until ((CurseTarget[ReadIndex] is dead) Equal to True. checkingg every 0.10seconds

Unit - Create 2 (Unit-Type- of CurseTarget[ReadIndex]) For player 1 (Red) at (Position of CurseTarget[ReadIndex]) facing Default building facing degrees

Set CurseSpawnedUnit[ReadIndex] = last created unit

Wait until ((CurseSpawnedUnit[ReadIndex] is dead) Equal to True. checkingg every 0.10 seconds

Unit - Create 4 (Unit-Type- of CurseTarget[ReadIndex]) For player 1 (Red) at (Position of CurseTarget[ReadIndex]) facing Default building facing degrees
The spell works fine, but when another person starts the same spell, it messes up the first spell, all units spawn in the position of the last target Unit of ability being cast..how can i fix this?


(PS- any one know how i can create the 4 units after (Both) previous units are dead and not just the last created unit)

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

The ReadIndex = ReadIndex + 1 needs to be after the wait.

#### Kaoskid

#### Kaoskid

 Level 1

JoinedSep 8, 2012Messages5

I just tried that it didknt work.


Wait until ((CurseTarget[ReadIndex] is dead) Equal to True. checkingg every 0.10seconds


Set ReadIndex = (ReadIndex +1)


Unit - Create 2 (Unit-Type- of CurseTarget[ReadIndex])


If i put it after wait then, trigger is gonna wait until ((CurseTarget[ReadIndex] is dead and since Set ReadIndex = (ReadIndex +1) is right after it, Its gonna wait until CurseTarget0 Is dead and there is no such unit so it will just skip it and spawn the 2 units

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

I'm sorry, but this can't be used with the "Wait until" action :/


The whole thing depends on the order of the execution of the actions.


This is why the last part of the tutorial that shows you how to make MUI Spells without waits is what I want readers to concentrate on.

edit


Also, you have more than 1 unit being created, so the spell will wait for one unit to die before it continues to do the actions after each wait.

#### Kaoskid

#### Kaoskid

 Level 1

JoinedSep 8, 2012Messages5

ohh damn

#### privatenumber

#### privatenumber

 Level 2

JoinedMar 10, 2012Messages10

I see..as a newcomer this guide is very handy and useful  thx!

#### Siryan

#### Siryan

 Level 4

JoinedAug 9, 2012Messages84

What would be really useful for extraordinaire noobs like me in coding is to tell viewers in your GUI tutorials from here on out what you're setting each variable to. If it's a more extensive tutorial with like 10+ triggers I wouldn't recommend that but, considering the short length of the GUI you're showing off, saying what variables your using would be useful to me.


I have 2 custom skills in my map that rely on a wait so as to cause a delayed action to happen after the wait. They are waits of 1.07.


Aside from all the above, Thank You for making this.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

I have made some minor modifications and improvements to the tutorial.


This thing is so lulzy

#### Binsu

#### Binsu

 Level 30

JoinedNov 29, 2012Messages6,631

Resources: 5

#### Resources
Maps 4Templates 1

Very useful tutorial. Quite entertaining to read it and also learned a lot on this. I didnt know Waits can cause a lot of trouble like that....


Thanks and +3 REP!

#### Siryan

#### Siryan

 Level 4

JoinedAug 9, 2012Messages84

Magtheridon96 said:

 I have made some minor modifications and improvements to the tutorial.


This thing is so lulzy


Click to expand...


What's so lulzy? Your tutorial or my remark, lol.

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 A MUI concern


JaysProjex

May 4, 2014

Triggers & Scripts

Replies13Views2K

May 6, 2014
deathismyfriend

Locked

[Solved]
 Trying to make a trigger MUI


HelloFlighty

Aug 13, 2017

Triggers & Scripts

Replies3Views2K

Aug 13, 2017
HelloFlighty

Locked

[Spell]
 Just wanted to ask if my spell is MUI


junius619

Dec 21, 2012

World Editor Help Zone

Replies16Views2K

Dec 21, 2012
junius619

 MUI Spells Using Artificial Waits


Tank-Commander

Sep 28, 2012

Trigger (GUI) Editor Tutorials

Replies32Views20K

Oct 16, 2015
BETABABY

Locked

 Any way to make this trigger work?


gamerandyeah

May 11, 2018

Triggers & Scripts

Replies3Views1K

May 11, 2018
gamerandyeah

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Trigger (GUI) Editor Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top