---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/vjass-dynamic-indexing-tutorial.245956/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# vJASS Dynamic Indexing Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/vjass-dynamic-indexing-tutorial.245956/>
#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Hi everybody !

Today, I’ll explain you how to use properly dynamic indexing with structs (so in vJASS )!

I’ll not explain how struct works (it is not the goal of this tutorial) but how to properly index them to render spells MUI.

I – Introduction

Okay I hope you know how works struct because as I just said before I’ll not explain how they work.

I’ll show two manners to do this indexing :

- The first way is C-like which means that we will use structs just as data.

- The second way is Java-like which means that we will use methods and static member to do so.

I hope you know a bit how work indexing otherwise I’ll explain this just now (and again later in the code commentaries).

Dynamic indexing’s goal is to do MUI spells!

If you ever did so in GUI you remembered that it needs one array for each thing you want to store about the spell. Now with structs we will just have to store the struct inside a struct array and there we go, we will be able to access every member!

To do dynamic indexing we also need an index that will increase when an instance will fire and that will decrease when an instance will be finished and recycled.

II – What spell will we do?

We will do a timed damage spell.

It will deal 75 damage per second over 3 seconds.

So we will have to store:

- The caster of the spell (to know which unit will deal damages) => caster

- The target of the spell (to know which unit will get damaged) => target

- The amount of damage => damage

- The time there is before the damaging => temp

- How many times will we have to hit the target before the end of the spell => steps

So we got our struct members just above!

Now, how the spell will work: the caster cast the spell, then 1s later the target got the damage. So we will need a periodic function that decrease the temp value and when the temp value is below or equal to 0 we will damage the target and recycle the index.

III – The spell

So we will start with global variables we need:

 JASS:

 globals
 private integer dindex = -1
 private constant real FPS = 0.0312500
 private timer period = CreateTimer()
endglobals

So there is our index that will start to -1 and each time an instance fire we will increase it by one so we will start to the index 0. We got our FPS of 0.0312500 which is surely one of the best period interval you can have.

Why it is a good period ? Because 1/0.0312500=32 so we have an integer number for the number of period per second

And at last we create a timer.

Now we will create our struct:

 JASS:

private struct tempDat
 unit caster
 unit target
 real temp
 real damage
 integer steps
endstruct

Now we need a struct array!

So we will just add it to our global block:

 JASS:

 globals
 private integer dindex = -1
 private constant real FPS = 0.0312500
 private tempDat array data
 private timer period = CreateTimer()
endglobals

Okay now we will use a scope to implement our spell so the code looks like this :

 JASS:

scope MySuperCoolSpell initializer init
 globals
 private integer dindex = -1
 private constant real FPS = 0.0312500
 private tempDat array data
 private timer period = CreateTimer()
 endglobals

 private struct tempDat
 unit caster
 unit target
 real temp
 real damage
 integer steps
 endstruct

 private function cond takes nothing returns boolean
 if GetSpellAbilityId() == OurSpellId then

 endif
 return false
 endfunction

 private function init takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(t, Condition(function cond))
 set t = null
 endfunction
endscope

Okay I implemented the init function and the condition function (with nothing inside ^-^)!

Now we will implement the beginning of the condition function!

 JASS:

private function cond takes nothing returns boolean
 local tempDat this
 if GetSpellAbilityId() == OurSpellId then
 //We create an instance of tempDat
 set this = tempDat.create()
 //We store the caster of the spell
 set this.caster = GetTriggerUnit()
 //We store the target of the spell
 set this.target = GetSpellTargetUnit()
 //We store the damage of the spell
 set this.damage = 75 * GetUnitAbilityLevel(this.caster, OurSpellId)
 //We store in how many times the spell will deal damage.
 set this.temp = 1
 //We will do the damage three times.
 set this.steps = 3
 endif
 return false
endfunction

Ok so I chose 75*level and 1s for fun you can freely change them or better store them into globals block for the second) or into constant function before the core spell but I’ll just interest myself to the indexing.

So I’ll implement the indexing :

 JASS:

 private function cond takes nothing returns boolean
 local tempDat this
 if GetSpellAbilityId() == OurSpellId then
 //We create an instance of tempDat
 set this = tempDat.create()
 //We store the caster of the spell
 set this.caster = GetTriggerUnit()
 //We store the target of the spell
 set this.target = GetSpellTargetUnit()
 //We store the damage of the spell
 set this.damage = 75 * GetUnitAbilityLevel(this.caster, OurSpellId)
 //We store in how many times the spell will deal damage.
 set this.temp = 1
 // We will do the damage three times.
 set this.steps = 3
 //We increase our index by one.
 set dindex = dindex + 1
 //We store our struct instance into the array.
 set data[dindex] = this
 //if the array was empty before
 if dindex == 0 then
 //We start the timer (I didn’t create the periodic function for the moment wait !)
 call TimerStart(period, FPS, true, function periodic)
 endif
 endif
 return false
endfunction

So to rephrase this we increase our index, we store the struct instance and if the array was empty so we start our timer.

Sweet clear isn’t it  ?

Now let’s do our periodic function:

 JASS:

private function periodic takes nothing returns nothing
 local tempDat this
 local integer i = 0
 loop
 exitwhen i>dindex
 set this = data[i]
 …
 set i = i + 1
 endloop
endfunction

With this we’ll loop through every struct instance we stored before.

Now we will do the core of the spell (nothing complicated xD) : we have to decrease the temp value of FPS each time we run the periodic function and if the temp value is expired we have to damage thetargeted unit :

 JASS:

private function periodic takes nothing returns nothing
 local integer i = 0
 local tempDat this
 loop
 exitwhen i>dindex
 set this = data[i]
 set this.temp = this.temp-FPS
 if this.temp <= 0 then
 call UnitDamageTarget(this.caster, this.target, this.damage, true, false, ATTACK_TYPE_MAGIC, DAMAGE_TYPE_MAGIC,null)
 //We decrement the number of times we damaged the unit.
 set this.steps = this.steps - 1
 //If it the third time the unit is damaged so we will stop the spell
 if this.steps == 0 then
 //Wash leaks :)
 set this.caster = null
 //Wash leaks :)
 set this.target = null
 //The spell is finish we don’t need to have this struct anymore.
 //We will see the recycle after
 call this.destroy()
 //Otherwise we continue the spell by setting the temp value to 1 again.
 else
 set this.temp = 1
 endif
 endif
 set i = i + 1
 endloop
endfunction

Now we have to recycle our index:

 JASS:

set data[i] = data[dindex]
set i = i – 1
set dindex = dindex – 1

To explain this piece of code we put the last stored instance to the position where an instance has been destroyed and we decrement of one both index.

Now we implement it:

 JASS:

private function periodic takes nothing returns nothing
 local integer i = 0
 local tempDat this
 loop
 exitwhen i>dindex
 set this = data[i]
 set this.temp = this.temp-FPS
 if this.temp <= 0 then
 call UnitDamageTarget(this.caster, this.target, this.damage, true, false, ATTACK_TYPE_MAGIC, DAMAGE_TYPE_MAGIC,null)
 //We decrement the number of times we damaged the unit.
 set this.steps = this.steps - 1
 //If it the third time the unit is damaged so we will stop the spell
 if this.steps == 0 then
 //Wash leaks :)
 set this.caster = null
 //Wash leaks :)
 set this.target = null
 set data[i] = data[dindex]
 set i = i – 1
 set dindex = dindex – 1
 //The spell is finish we don’t need to have this struct anymore.
 call this.destroy()
 //Otherwise we continue the spell by setting the temp value to 1 again.
 else
 set this.temp = 1
 endif
 endif
 set i = i + 1
 endloop
endfunction

Okay now there is still one problem !

The timer will run forever even if we got no instance :/

So we’ll have to check if your index is equal to -1 (meaning the array is empty):

 JASS:

if dindex == -1 then
 call PauseTimer(period)
endif

So here is the result :

 JASS:

scope MySuperCoolSpell initializer init
 globals
 private integer dindex = -1
 private constant real FPS = 0.0312500
 private timer period = CreateTimer()
 endglobals

 private struct tempDat
 unit caster
 unit target
 real temp
 real damage
 integer steps

 method destroy takes nothing returns nothing
 //Wash leaks
 this.caster = null
 this.target = null
 if dindex == -1 then
 call PauseTimer(period)
 endif
 call this.deallocate()
 endmethod
 endstruct

 //Sometimes it bugs if you declare this before the struct so in the final result I put it after for safety.
 globals
 private tempDat array data
 endglobals

 private function periodic takes nothing returns nothing
 local integer i = 0
 local tempDat this
 loop
 exitwhen i>dindex
 set this = data[i]
 set this.temp = this.temp-FPS
 if this.temp <= 0 then
 call UnitDamageTarget(this.caster, this.target, this.damage, true, false, ATTACK_TYPE_MAGIC, DAMAGE_TYPE_MAGIC,null)
 set this.steps = this.steps - 1
 if this.steps == 0 then
 set data[i] = data[dindex]
 set i = i - 1
 set dindex = dindex - 1
 call this.destroy()
 else
 set this.temp = 1
 endif
 endif
 set i = i + 1
 endloop
 endfunction

 private function cond takes nothing returns boolean
 local tempDat this
 if GetSpellAbilityId() == 'A000' then
 set this = tempDat.create()
 set this.caster = GetTriggerUnit()
 set this.target = GetSpellTargetUnit()
 set this.temp = 1
 set this.damage = 75*GetUnitAbilityLevel(this.caster, 'A000')
 set this.steps = 3
 set dindex = dindex + 1
 set data[dindex] = this
 if dindex == 0 then
 call TimerStart(period, FPS, true, function periodic)
 endif
 endif
 return false
 endfunction

 private function init takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(t, Condition(function cond))
 set t = null
 endfunction
endscope

IV - Now within the struct

I'll put the code with commentary to show you how to do it only in the struct :

 JASS:

scope MySuperCoolSpell
 globals
 private constant real FPS = 0.0312500
 endglobals

 private struct tempDat
 unit caster
 unit target
 real temp
 real damage
 integer steps
 //Static members work like globals :)
 static integer dindex
 static timer period
 static thistype array data

 method destroy takes nothing returns nothing
 //Wash leaks
 this.caster = null
 this.target = null
 if dindex == -1 then
 call PauseTimer(period)
 endif
 call this.deallocate()
 endmethod

 static method periodic takes nothing returns nothing
 local integer i = 0
 local tempDat this
 loop
 exitwhen i>dindex
 set this = data[i]
 set this.temp = this.temp-FPS
 if this.temp <= 0 then
 call UnitDamageTarget(this.caster, this.target, this.damage, true, false, ATTACK_TYPE_MAGIC, DAMAGE_TYPE_MAGIC,null)
 set this.steps = this.steps - 1
 if this.steps == 0 then
 set data[i] = data[dindex]
 set i = i - 1
 set dindex = dindex - 1
 call this.destroy()
 else
 set this.temp = 1
 endif
 endif
 set i = i + 1
 endloop
 endmethod

 static method cond takes nothing returns boolean
 //Thistype is replaced by the name of the struct :)
 local thistype this
 if GetSpellAbilityId() == 'A000' then
 set this = thistype.allocate() //Exactly the same as create
 set this.caster = GetTriggerUnit()
 set this.target = GetSpellTargetUnit()
 set this.temp = 1
 set this.damage = 75*GetUnitAbilityLevel(this.caster, 'A000')
 set this.steps = 3
 set dindex = dindex + 1
 set data[dindex] = this
 if dindex == 0 then
 call TimerStart(period, FPS, true, function thistype.periodic)
 endif
 endif
 return false
 endmethod

 //This method is called at the begginning of the map.
 //The keyword onInit does it.
 static method onInit takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(t, Condition(function thistype.cond))
 //I personally prefer to instanciate static members there just a point of view
 //You can do it in their declaration too it is the same.
 set dindex = -1
 set period = CreateTimer()
 set t = null
 endmethod
 endstruct
endscope

Conclusion

Here you made a MUI spell in vJASS.

Feel free to add more configurable things like this after the globals to make this more user-friendly :

 JASS:

private constant function Damage takes integer level returns real
 return 75.*level
endfunction

Thanks for reading and feel free to tell me what do you think of it !

Malhorne


 Last edited: Mar 8, 2017

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

~Reserved

EDIT : This was Christmas gift (only if it is good otherwise it is Christmas random thingy !)

#### Geshishouhu

#### Geshishouhu

 Level 11

JoinedOct 11, 2012Messages711

Resources: 2

#### Resources
Spells 2

This is a nice, easy-understandable and very useful tutorial. It is especially helpful for newbies like me.

Thanks, Malhorne.

+Rep if I can.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice tutorial overall. The content is fine. However, I think overall it would be a bit better to do a periodic spell as an example, such as a DoT spell, just because you don't normally associate a looping timer with a one-shot sort of spell. The spell in your example deals damage after "1 second", which would probably make more sense with a TimerUtils approach (just wait 1 second and then deal damage vs. 32 loops of 0.03125 seconds before doing damage).

If you change it to a DoT (damage over time) type of spell (or anything like that), it'll be a perfect example of using dynamic indexing + a timer.

Otherwise, it is pretty well done. nj. I might need to fix some grammar though, are you okay with that?

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Oh yes go on to fix my grammar I'm french xD !

I might to a second approach with DoT spells but I'd like to keep this as first approach

Anywya thanks for the reply !

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

PurgeandFire said:

 Nice tutorial overall. The content is fine. However, I think overall it would be a bit better to do a periodic spell as an example, such as a DoT spell, just because you don't normally associate a looping timer with a one-shot sort of spell. The spell in your example deals damage after "1 second", which would probably make more sense with a TimerUtils approach (just wait 1 second and then deal damage vs. 32 loops of 0.03125 seconds before doing damage).


If you change it to a DoT (damage over time) type of spell (or anything like that), it'll be a perfect example of using dynamic indexing + a timer.


Otherwise, it is pretty well done. nj. I might need to fix some grammar though, are you okay with that?


Click to expand...


Yes, it is not too efficient as is, but I prefer having one timer personally


Imagine you have 150 instances running, you would have to have 150 timers

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Yes it is better to use one timer normally if you have large amount of instances it will be better.

Thanks for the reply edo

#### GhostWolf

#### GhostWolf

 Level 29

JoinedJul 29, 2007Messages5,174

Resources: 2

#### Resources
Tools 1Tutorials 1

Or just use locals and a sleep, and you end up with about 10 lines of code, because not everything has to be overcomplicated.

So yes, this example isn't the best.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

if you want to be half precise about timing you have to use timer

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Okay I'll change the example to a three times periodic damage.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Okay updated with a spell that deal damage thrice in a row !

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Not bad, still it lacks read-ability (not the code, but the whole tutorial).

While the content is good, I wasn't really caught by the presentation.

For instance one of the best tuts we have is the dynamic indexing from Purge,

it is short, well highlighted and easy to understand.

What we do need are very user(beginner)-friendly tutorials for correct vJass usage. People must get into the subject matter as fast as possible. The key is to let them know that what they do in GUI is as easy in vJass.

If I come up with a proper layout or constructive ideas I'll let you know.

- For now don't use the whole page from left to right for one sentence.

- Make the content short, but distinct -->

 So after 1s it will deal, damage, then after another second, it will deal damage, and again after the last second it will deal damage.


Click to expand...

It will deal 5 damage per second over 3 seconds.

 We got our FPS of 0.0312500 which is surely one of the best period interval you can have.


Click to expand...

I see this quite often. People say something is awesome or bad, but don't eplain why.

Something like 0.03125 is handy, because it is 1/32 of a second, speaking about such don't be over-correct (if this word even exists ^^).

Finally:

 JASS:

/*
* I like these comments more ....
* because with spacing you can seperate
* your code into blocks
*
* Use spacing so eyes and head don't
* have to move too much.
*/
if this != 0 then
 /*
 * Check whatsoever
 */
 if IsThisAwesome(evidence) then

 endif
 /*
 * Here we do ...
 */
endif

Good luck with this.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Thanks for your review.

I posted only this tut among the three I did because it is the only one which is at least somewhat read-able ^^'

I've a huge JASS tutorial but it is not that good to read (even if people manage to learn JASS through it xD) ....

I'm pretty bad to make something read-able because I don't see what it needs to be so :/

So thanks for pointing some idea !

EDIT : Corrected some mistakes and changed some things you pointed.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I like it. I agree with BPower's statements about compaction, etc., but it seems that you have improved it for the most part. There may be a few grammar/spelling things to fix here and there, but it is still perfectly readable, so it is approved.

#### Jet Kong

#### Jet Kong

 Level 1

JoinedAug 12, 2012Messages7

Hello, you're tutorial is very short but it is something, I really want to learn vJass. I did configure something in your spell code ( ehm or trigger i guess. ) I just want something to get clarify

 JASS:

scope MySuperCoolSpell
 globals
 private constant real FPS = 0.0312500
 endglobals

 private struct tempDat
 unit caster
 unit target
 real x
 real y
 real temp
 real damage

 integer duration
 //Static members work like globals :)
 static integer dindex
 static timer period
 static thistype array data

 method destroy takes nothing returns nothing
 if dindex == -1 then
 call PauseTimer(period)
 endif
 call this.deallocate()
 endmethod

 static method periodic takes nothing returns nothing
 local integer i = 0
 local tempDat this
 loop
 exitwhen i>dindex
 set this = data[i]
 set this.temp = this.temp-FPS
 set this.x = GetWidgetX(this.caster)
 set this.y = GetWidgetY(this.caster)
 call SetUnitPosition(this.target, this.x, this.y)
 if this.temp <= 0 then
 call UnitDamageTarget(this.caster, this.target, this.damage, true, false, ATTACK_TYPE_MAGIC, DAMAGE_TYPE_MAGIC,null)
 call AddSpecialEffectTarget( "Objects\\Spawnmodels\\Critters\\Albatross\\CritterBloodAlbatross.mdl", this.caster, "head" )
 call DestroyEffect( GetLastCreatedEffectBJ() )
 set this.duration = this.duration - 1
 if this.duration == 0 then
 call PauseUnit ( this.target, false )
 call ShowUnitShow ( this.target )
 call SetUnitPathing ( this.target, true )
 set this.caster = null
 set this.target = null
 set data[i] = data[dindex]
 set i = i - 1
 set dindex = dindex - 1
 call this.destroy()
 else
 set this.temp = 1
 endif
 endif
 set i = i + 1
 endloop
 endmethod

 static method cond takes nothing returns boolean
 //Thistype is replaced by the name of the struct :)
 local thistype this
 if GetSpellAbilityId() == 'AHtb' then
 set this = thistype.allocate() //Exactly the same as create
 set this.caster = GetTriggerUnit()
 set this.target = GetSpellTargetUnit()
 call PauseUnit ( this.target, true )
 call ShowUnitHide ( this.target )
 call SetUnitPathing ( this.target, false )
 set this.temp = 1
 set this.damage = 15*GetUnitAbilityLevel(this.caster, 'AHtb')
 set this.duration = 3
 set dindex = dindex + 1
 set data[dindex] = this
 if dindex == 0 then
 call TimerStart(period, FPS, true, function thistype.periodic)
 endif
 endif
 return false
 endmethod

 //This method is called at the begginning of the map.
 //The keyword onInit does it.
 static method onInit takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(t, Condition(function thistype.cond))
 set dindex = -1
 set period = CreateTimer()
 set t = null
 endmethod
 endstruct
endscope

PLEASE DO CHECK IF I DID SOMETHING WRONG.

and 1 more question, if i'm gonna make another spell which part of the code should or needed to be changed, i mean in gui if use the variable caster in the next spell you need to use caster2 something like that ... I hope i'm getting somewhere with my question

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

ShowUnitHide(this.target) -> ShowUnit(this.target, false)

 JASS:

call AddSpecialEffectTarget( "Objects\\Spawnmodels\\Critters\\Albatross\\CritterBloodAlbatross.mdl", this.caster, "head" )
 call DestroyEffect( GetLastCreatedEffectBJ() )

->

call DestroyEffect(AddSpecialEffectTarget( "Objects\\Spawnmodels\\Critters\\Albatross\\CritterBloodAlbatross.mdl", this.caster, "head" ))

call PauseUnit ( this.target, true )

PauseUnit is a bad function to use -> I would rather use the Stun System or use a custom stun.

set this.x = GetWidgetX(this.caster)

Better use GetUnitX/Y they are faster.

call SetUnitPosition(this.target, this.x, this.y)

Use SetUnitX/Y it is faster.

Then don't forget to use BoundSentinel by Vexorian.

 JASS:

set this.x = GetWidgetX(this.caster)
 set this.y = GetWidgetY(this.caster)
 call SetUnitPosition(this.target, this.x, this.y)

This piece of code does pretty nothing since you're moving the unit to its position.

call ShowUnitShow ( this.target )

->

call ShowUnit(this.target, true)

Then in your code you never deal the damage.

#### Jet Kong

#### Jet Kong

 Level 1

JoinedAug 12, 2012Messages7

Malhorne said:


This piece of code does pretty nothing since you're moving the unit to its position.

call ShowUnitShow ( this.target )


->

call ShowUnit(this.target, true)


Then in your code you never deal the damage.


Click to expand...


I don't get this one. The spell was based on devour so I tried to hide the unit, it still get the damage.


Thanks. I think I will get use to this.


+rep

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

When I was saying it does nothing it was on the SetUnitPosition ^^

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Fixed readibility.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

This tutorial teaches bad OOP styles.

The cleaning of leaks should always be done in the onDestroy method, not manually from within the calling method (in your case, "periodic").

This is so that when destroying the struct from the outside, your spell will not crash or leave any uncleaned leaks.

.destroy() should always be coded in a way to correctly terminate the struct and all of it's members in a single call, no matter what the execution state of the spell is. It should immediately stop and destroy all timers and free all handle variables.

If the spell creates a dummy unit or special effect, it should also remove the dummy unit and effect instead of just nulling the variables.

This not only avoids potential bugs, but it also allows for extra functionality, like having spells that immediately terminate all ongoing spell effects.

Also, replacing the static method create by something that takes the initial struct members as parameters is a good practice to reduce the number of lines required for each struct allocation.

Also, for indexing spells (to be honest, I always prefer having an individual timer per spell instance, as it gets rid of all the indexing overhead and shortens the code), I recommend having a base indexed spell struct and extend your spells upon it.

This avoids repetition of code and allows easier access from the outside.

Basicly:

struct MySpell extends Spell

This way, each new index-based spell reuses the base structure from Spell and only adds custom callbacks on top of it, which eliminates repeating the index structure again and again.

If you want to get extra fancy, you can add another layer of abstraction to it, with each layer adding new members to your base struct:

struct Flamethrower extends ChanneledDot extends Spell

PS: Reminds me ... someone really has to write styleguides for vJass.


 Last edited: Jul 9, 2015

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

onDestroy kills performance.

Yeah I forgot the wash leaks in destroy method my bad.

I'll fix it soon.

Individual timer per instance is by far less efficient.

The extends also kill performance in vJASS.

EDIT : Fix the tutorial.

Here is a link you should read : https://www.hiveworkshop.com/forums/jass-ai-scripts-tutorials-280/structs-dummies-197435/

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

I dont really like that tutorial, it doesnt go indepth into the features, he just says, like in onDestroy, to send you there if someone say to use it, thats not really how good tutorial should work.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

I'm not here to prove the known fact that onDestroy and extends aren't good pratice in vJASS

I can freely put the output code of onDestroy (creating a trigger and evaluating it), or extends (which generates onDestroy methods).

EDIT : Obviously the pattern described by Zwiebelchen with the extends sounds good but isn't efficient in vJASS.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Malhorne said:

 I'm not here to prove the known fact that onDestroy and extends aren't good pratice in vJASS

I can freely put the output code of onDestroy (creating a trigger and evaluating it), or extends (which generates onDestroy methods).

EDIT : Obviously the pattern described by Zwiebelchen with the extends sounds good but isn't efficient in vJASS.


Click to expand...

A tutorial that aims to teach people about vJass should never be about efficiency. It should be about readability and clear, logical structure.

There is no point in teaching beginners bad practices just because they are slightly faster.

There is a reason vJass was created. And that reason wasn't speed. It was to bring order into the chaos. Creating more chaos just because it's slightly faster than order is just plain wrong.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

So I'll teach them inefficient ways in order them to write inefficient spells in order them to be corrected in spell sections when they could learn could pratice in tutorials ?

What I am explaining is not at all a bad practice.

We're not in Java with UML and design pattern everywhere.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Malhorne said:

 So I'll teach them inefficient ways in order them to write inefficient spells in order them to be corrected in spell sections when they could learn could pratice in tutorials ?


What I am explaining is not at all a bad practice.


We're not in Java with UML and design pattern everywhere.


Click to expand...

Nobody has ever rejected a clean and well-structured spell resource just for the lack of minor speed improvements.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Yes nobody rejects but "needs fix" is sometimes used in this case.

I mean go to Jass section and you will see.

And people will always tell you to fix some efficiency tricks.

For instance, why using onDestroy methods when you can do everything inside the destroy methods ?

Why ?

For nothing. Except the fact of calling a trigger. Which adds memory taken by the map and execution time.

EDIT :


 JassHelper Manual said:



 It is best to avoid implementing features that look cool in paper but do not plain work when compiled.


Click to expand...

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

this is not problem of the efficiency of the language, but the way mods see the not so usual vJass constructs, like interfaces, polymorphism, inheritance or even hooks.

Are you really going to say its so inefficient? If we wanted efficient stuff, we wouldnt be coding for wc3 to begin with.

 Firing trigger adds to memory and execution time


Click to expand...

1. While it will take longer, if it is like 1 microsecond longer, its literally unnoticable.

2. memory? not like we have 256 MB ram chips nowadays.

If size was concern, Iceborn wouldn't be approved, because it itself is 100 MB map, which means that 100 MB of stuff needs to be loaded into RAM even before you heat up VM.

onDestroy is indeed kind of funny yes.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

There is a good way to do extending with delegate still. Why people tend to use this extends :s

I like coding for wc3, I don't see why you say that I should quit if I want my code to be efficient regarding vJASS.

You say that everyone shouldn't care about efficiency and calling BJs and every features of vJASS involving trigger firing ?

I played maps that heavily lag because of triggers mostly.

War3 is still limited to 2gb ram.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

I dont say you should quick, but you know, read my signature.

+ I didnt really mention programming per se, but tutorials as Zwieb said.

You think 1 trigger evaluation will bring you to 2 gigs of ram?

Also people usually do loops for AnyUnitEvent, but there is completly fine native, and in onInit 1 more function call is barely any overhead to notice it on loading.

You can be as efficient as you want, but shoving rejects or need fixes into other people's necks(I dont say you do it, but generally it happens) just because of polymorphism or inheritance is plain retardment. The code does what it is meant to, and unless it is really critical system(Timer stuff is all I can think of that is really crucially critical) its not really problem

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

No but assume you have 1000 structs in a good map. It is not impossible at all.

With extending.

It could easily come up with 2000 thousands more trigger and evaluating :s

Ahah yes the AnyUnitEvent xD

Init stuff isn't really a problem, people should understand this. Better to loose time on init that in game.

Yeah the timer sounds important to me too. 100 spells, 10 issues per spell -> 1000 timer running only for spell, that's bad.

After I agree on the fact that some optimizations are not that important and I'm far too much concerned about it and that's one of my default.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

It's still bad style of coding. Especially all the "magic hacks" that basicly break the original intention of vJass ("struct extends array"...). There is a reason why onDestroy() was implemented. There is a reason why inheritance is there. It's because vJass aims to replicate a "real" OOP language.

And I want to add: it does this job quite fine. Most people that learn OOP styles will immediately feel familiar with the syntax and interfaces. vJass is user comfort. OOP is about avoiding bugs through clear code structure and never having to repeat yourself.

Not using extends just because it creates code copies is just stupid. I don't care how vJass works behind the scenes. What I care about is having readable, easily maintainable code. Everything that deviates from typical OOP style and every single one of those hacks applied undermine the whole idea of vJass.

Think about it: vJass compiles to plain Jass. So if you care about speed, why even code in vJass? Just use the original unmodified Jass language and be done with it. Not only will it create less junk in your map MPQ and save filesize, but it will also execute faster.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Vexorian himself wrote the sentence I quoted.

There is a reason why struct extends array exists.

There is a reason why delegate exists.

vJASS compiles to JASS involving triggers and evaluate which is super slow.

Proper and efficient vJASS will just add some globals. No speed loss.

The only bad side is the long global variables which can obviously be reduced with Vexorian optimizer. So no more problem.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

The inheritance suggestion is a good idea, and I'd definitely support that decision when designing a map. With public spells, it becomes a bit difficult to judge. Should there be a standard spell parent delegate? One for channeling, one for DoT's, etc.? Or would we end up with spells with each their own parent (sort of defeating the purpose)?


I agree that deciding not to include inheritance for efficiency reasons is not a good reason. A better reason would be that it has a big learning curve, or that it isn't the focus of the tutorial. This is just one way to implement a spell, and as with most tutorials, they are just there to get you started and build your intuition. I think this tutorial does a decent job on honing in on the indexing part, which is the main lesson. If this was a "spells in vJASS" tutorial, I'd say that it could use expanding.


I agree with your notes on inheritance and that the general view of it is unnecessarily negative, but at least a lot of that 'old' stigma is washing away. Fewer people use "extends array", and the only reason interfaces/extends isn't being used is because people probably don't know how to use it. OO is one of those things that really don't get used properly by beginners unless it is shoved down your throat. ;D But if we shoved it down people's throat, then people would probably be too afraid to even try vJASS (I mean, that is pretty much how it was when the standards were far more strict). That's the drawback of enforcing style-guides besides something basic like JPAG. Perhaps we have become too lax?


 edo said:



 I dont really like that tutorial, it doesnt go indepth into the features, he just says, like in onDestroy, to send you there if someone say to use it, thats not really how good tutorial should work.


Click to expand...


He adds a tiny bit of reasoning. But I agree, it is not the full length of discussion that it could've been. Honestly, the manual is the best resource for the most of the vJASS features (although even that could be expanded). But his tutorial is concise and to the point (as are most of the "for dummies" books). That isn't a bad thing. I'm a fan of bloated college textbooks as much as the next guy, but sometimes it helps to have an overview of syntax and features before you learn about the inner workings of a language.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

PurgeandFire said:

 I agree with your notes on inheritance and that the general view of it is unnecessarily negative, but at least a lot of that 'old' stigma is washing away. Fewer people use "extends array", and the only reason interfaces/extends isn't being used is because people probably don't know how to use it.


Click to expand...

And this is exactly my problem. Who are those people going to learn from? There are basicly 2 crowds in these forums; the vJass elitists and the GUIers.

But sometimes, the sky opens and a GUIer breaks out from his prison and actually learns Jass.

I still remember seeing horrible GUI mess in the Trigger Help section from some of the guys that now browse the JASS section on a regular basis and submit resources.

The problem is, that every once in a while when a GUIer wants to learn JASS, they are mostly learning it from the elitists. And elitists aren't good teachers per definition, as they know all the bugs and tricks and workarounds that you just shouldn't teach a beginner.

Struct extends array was just the tip of the iceberg. I can't even say how glad I am that this abomination has almost vanished, now that the "big guys" like Nestharus and Magtheridon rarely submit new resources anymore.

What I'm saying is: we - as the guys that browse the JASS section on a regular basis - have a responsibility. With every thread we answer in the Trigger Help section, we should keep that responsibility in mind: we are basicly the "teacher" for those seeking for help.

And when you study programming in college or university, the first thing you learn is that maintainable, readable code trumphs efficiency almost any time. If you can't measure a speed improvement in miliseconds, it's not about efficiency anymore; it's about your ego.

Efficiency matters in databases or root systems. Efficiency matters for automated systems. Efficiency matters for embedded software, sorting or archiving algorithms. Efficiency matters in graphics and pathfinding.

Also, it's always funny to see elitists complain about the lack of speed quirks in spell submissions. Because it's a spell. Most likely, this spell will have a dummy unit and even move it around. Most likely, this spell will interact with multiple other units or even enumerate units when it is an AoE spell. Most likely, this spell will deal damage to another unit.

Do you know what UnitDamageTarget, SetUnitX/Y, GroupEnumUnitsInRange have in common?

that they create overhead the size of Russia.

A trigger evaluation more or less is peanuts compared to that.

Efficiency in spells is never about script runtime. It's about avoiding bugs and low maintenance.

 OO is one of those things that really don't get used properly by beginners unless it is shoved down your throat. ;D But if we shoved it down people's throat, then people would probably be too afraid to even try vJASS (I mean, that is pretty much how it was when the standards were far more strict). That's the drawback of enforcing style-guides besides something basic like JPAG. Perhaps we have become too lax?


Click to expand...

I don't think that OOP is hard to learn or that stuff like inheritance are too advanced for any beginner to grasp. They are not; in fact, inheritance is a pretty logical and intuitive step for every programming beginner that understood the basic concept of structs.

Struct extends array is not.

We should teach beginners how to use OOP in the way it was intended to be used. Vexorian might have added some quirks here and there that not many people like; but that shouldn't keep us from using them as 'positive rolemodels'.

After all, people will eventually move away from JASS somewhere in the future. They might land in a Unity project or a university project. And they will eventually draw from the experience gathered in JASS, especially when they learned OOP programming via JASS.

Bad habits are hard to unlearn. That's why we should shove all the JASS specific things out of the window (as long as they can be avoided) and teach "good practice" styles.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

 A trigger evaluation more or less is peanuts compared to that.


Click to expand...

I just want to disagree about this.


The rest is your point of view there is nothing to debate we keep saying the same thing over and over.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Malhorne said:

 I just want to disagree about this.


Click to expand...

Disagree or not; it doesn't matter, because you can actually try that out:


Evaluate 10000 triggers per second or use one of the other functions I mentioned 10000 times per second. Then tell me what will make your FPS drop harder.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Dude do you think before saying this?

The fact that you evaluate trigger for nothing is far more slower that just plainly call the functions.

Seriously this discussion is so pointless..

https://www.hiveworkshop.com/forums/lab-715/jass-benchmarking-results-245362/

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Malhorne said:

 Dude do you think before saying this?

The fact that you evaluate trigger for nothing is far more slower that just plainly call the functions.

Seriously this discussion is so pointless..

https://www.hiveworkshop.com/forums/lab-715/jass-benchmarking-results-245362/

Click to expand...

Of course trigger evaluation is slower than a simple function call (about 10 times slower, we know that).

That wasn't the point.

But a Trigger evaluation is faster than moving units, dealing damage or whatever gameplay effect you want to have for your spell.

Therefore, it doesn't really matter if you use a trigger evaluation more or less, unless your spell is used a thousand times and doesn't have any impact on gameplay. In comparison to any gameplay affecting function, a trigger evaluation is nothing.

I usually compare it like this:

When you pull a caravan with your car, one person more sitting on the backseat won't make a difference.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

 JASS:

scope Stopwatch initializer onInit

 native UnitAlive takes unit id returns boolean

 globals
 trigger tr
 integer arg
 unit u
 endglobals

 private function Actions takes nothing returns boolean
 local integer sw
 local integer i = 0
 local integer array ticks
 local unit u = CreateUnit(Player(0), 'hfoo', 0, 0, 0)

 set sw = StopWatchCreate()
 loop
 exitwhen i == 1000
 //TEST 1
 call TriggerEvaluate(tr)
 set i = i + 1
 endloop
 set ticks[0] = StopWatchTicks(sw)

 call BJDebugMsg("1000 iterations of TriggerEvaluate took " + I2S(StopWatchMark(sw)) + " milliseconds to finish.")
 call StopWatchDestroy(sw)

 set sw = StopWatchCreate()
 set i = 0

 loop
 exitwhen i == 1000
 // TEST 2
 call SetUnitX(u, 0)
 call SetUnitY(u, 0)
 set i = i + 1
 endloop
 set ticks[1] = StopWatchTicks(sw)

 call BJDebugMsg("1000 iterations of SetUnitX/Y took " + I2S(StopWatchMark(sw)) + " milliseconds to finish.")
 call StopWatchDestroy(sw)

 if (ticks[0] > ticks[1]) then
 call BJDebugMsg("SetUnitX/Y is faster than TriggerEvaluate")
 else
 call BJDebugMsg("TriggerEvaluate is faster than SetUnitX/Y")
 endif
 return false
 endfunction

 private function A takes nothing returns boolean
 local integer this = arg
 return true
 endfunction
 //===========================================================================
 private function onInit takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterPlayerEvent(t, Player(0), EVENT_PLAYER_END_CINEMATIC)
 call TriggerAddCondition(t, function Actions)
 set tr = CreateTrigger()
 call TriggerAddCondition(tr, Condition(function A))
 set u = CreateUnit(Player(0), 'hpea', 0, 0, 0)
 set t = null
 endfunction

endscope


Attached the result.


SetUnitX AND SetUnitY takes less time than trigger evaluate.


The content of the trigger condition is the same as an empty onDestroy methods (directly taken from the war3map.j).

#### Attachments

Proof.png

 59.5 KB

 · Views: 225



#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Malhorne said:

 JASS:

scope Stopwatch initializer onInit

 native UnitAlive takes unit id returns boolean

 globals
 trigger tr
 integer arg
 unit u
 endglobals

 private function Actions takes nothing returns boolean
 local integer sw
 local integer i = 0
 local integer array ticks
 local unit u = CreateUnit(Player(0), 'hfoo', 0, 0, 0)

 set sw = StopWatchCreate()
 loop
 exitwhen i == 1000
 //TEST 1
 call TriggerEvaluate(tr)
 set i = i + 1
 endloop
 set ticks[0] = StopWatchTicks(sw)

 call BJDebugMsg("1000 iterations of TriggerEvaluate took " + I2S(StopWatchMark(sw)) + " milliseconds to finish.")
 call StopWatchDestroy(sw)

 set sw = StopWatchCreate()
 set i = 0

 loop
 exitwhen i == 1000
 // TEST 2
 call SetUnitX(u, 0)
 call SetUnitY(u, 0)
 set i = i + 1
 endloop
 set ticks[1] = StopWatchTicks(sw)

 call BJDebugMsg("1000 iterations of SetUnitX/Y took " + I2S(StopWatchMark(sw)) + " milliseconds to finish.")
 call StopWatchDestroy(sw)

 if (ticks[0] > ticks[1]) then
 call BJDebugMsg("SetUnitX/Y is faster than TriggerEvaluate")
 else
 call BJDebugMsg("TriggerEvaluate is faster than SetUnitX/Y")
 endif
 return false
 endfunction

 private function A takes nothing returns boolean
 local integer this = arg
 return true
 endfunction
 //===========================================================================
 private function onInit takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterPlayerEvent(t, Player(0), EVENT_PLAYER_END_CINEMATIC)
 call TriggerAddCondition(t, function Actions)
 set tr = CreateTrigger()
 call TriggerAddCondition(tr, Condition(function A))
 set u = CreateUnit(Player(0), 'hpea', 0, 0, 0)
 set t = null
 endfunction

endscope

Attached the result.

SetUnitX AND SetUnitY takes less time than trigger evaluate.

The content of the trigger condition is the same as an empty onDestroy methods (directly taken from the war3map.j).


Click to expand...

Now try the same with some "Enter Region" events in place and several other units on the map for the sake of context.

Also, SetUnitX/Y isn't even that much faster from your test. Use SetUnitX() 5 times and the TriggerEvaluation breaks even. Now try the other functions for comparison, like UnitDamageTarget(). But make sure to have a damage detection system in place for realism.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

You're kind of destroying the purpose of this tutorial with debating about efficiency so hard.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

IcemanBo said:

 You're kind of destroying the purpose of this tutorial with debating about efficiency so hard.


Click to expand...

Yeah, sorry, blowing this way out of proportion.


I agree that it's a matter of taste. I brought my criticism; he doesn't want to accept it - which is fine, I guess. Nothing more to say here, really.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1


 IcemanBo said:



 You're kind of destroying the purpose of this tutorial with debating about efficiency so hard.


Click to expand...


 Malhorne said:



 Seriously this discussion is so pointless..


Click to expand...


 Zwiebelchen said:



 Now try the same with some "Enter Region" events in place and several other units on the map for the sake of context.


Click to expand...

What's the matter with Enter Region? There is no event in this map..


 Zwiebelchen said:



 Do you know what UnitDamageTarget, SetUnitX/Y, GroupEnumUnitsInRange have in common?


that they create overhead the size of Russia.


A trigger evaluation more or less is peanuts compared to that.


Click to expand...


 Zwiebelchen said:



 Disagree or not; it doesn't matter, because you can actually try that out:


Evaluate 10000 triggers per second or use one of the other functions I mentioned 10000 times per second. Then tell me what will make your FPS drop harder.


Click to expand...


 Zwiebelchen said:



 Also, SetUnitX/Y isn't even that much faster from your test. Use SetUnitX() 5 times and the TriggerEvaluation breaks even. Now try the other functions for comparison, like UnitDamageTarget(). But make sure to have a damage detection system in place for realism.


Click to expand...

They are equal with 5 times. And pay attention about the fact that the trigger that I evaluate is nearly empty !


I think this quote collection is enough.


Now thanks for the interlude and could we stop.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Malhorne said:

 What's the matter with Enter Region? There is no event in this map..


Click to expand...

I'll just answer this, just because I still think you didn't get my point:

I'm saying you are putting the comparison out of context. Which is important here, because the overhead of these functions depends on the number of enter-events and units on the map.

SetUnitX/Y get more taxing the more events you have registered. So the benchmark result is not representative for the lack of a test environment that reflects an actual use-case scenario.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Yes and the onDestroy method will never be empty...

Could we just stop ? One page and a half for this narrow minded discussion starts to be huge.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Malhorne said:

 Yes and the onDestroy method will never be empty...


Could we just stop ? One page and a half for this narrow minded discussion starts to be huge.


Click to expand...

I think this discussion was and still is actually quite valid.


But, yes call it "narrow-minded" ... -.-

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

So let's continue the boring off topic stuff in my VM where you have your opinion, I have one. And since the beginning I answered to every argument with proof you showed nothing.

So sorry to sound a little aggressive or bored.

Everywhere there should be someone that want to jump at your throat and bite you for nothing..

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Malhorne said:

 So let's continue the boring off topic stuff in my VM where you have your opinion, I have one. And since the beginning I answered to every argument with proof you showed nothing.

So sorry to sound a little aggressive or bored.

Everywhere there should be someone that want to jump at your throat and bite you for nothing..


Click to expand...

Seriously, excuse me for expressing my oppinion on a tutorial made for public use in a public forum.

I don't know when exactly this discussion turned into aggression, because I found it to be pretty interesting and non-offensive. But that's just me. Maybe it's because I don't find discussions about technical things offensive in general.

If you feel offended; yeah, I agree, then there is no point in going further.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Discussion about things that have nothing to do with the tutorial.

I really understand why Nestharus left now.

 You must log in or register to reply here.


### Similar threads

Locked

 Group Leaking


WyrmSlayer

Oct 26, 2019

World Editor Help Zone

Replies8Views3K

Oct 27, 2019
Uncle

Locked

[vJASS]
 Still new, am I doing it right, and some questions


Idaero Ace

Jul 12, 2018

Triggers & Scripts

Replies5Views1K

Jul 14, 2018
Pyrogasm

Locked

 Static if bug


Malhorne

Mar 13, 2014

Triggers & Scripts

Replies7Views2K

Mar 13, 2014
Malhorne

Locked

[vJASS]
 Logic Challenge or Impossible?


userid907

Nov 20, 2016

Triggers & Scripts

Replies10Views3K

Nov 24, 2016
Wietlol

[vJASS]
 Lightning Bender


userid907

Aug 7, 2016

Graveyard

Replies8Views3K

Nov 3, 2016
IcemanBo

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