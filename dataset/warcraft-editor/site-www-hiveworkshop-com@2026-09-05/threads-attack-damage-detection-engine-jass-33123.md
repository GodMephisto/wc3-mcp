---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/attack-damage-detection-engine-jass.33123/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Attack Damage Detection Engine JASS
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/attack-damage-detection-engine-jass.33123/>
#### paskovich

#### paskovich

 Level 11

JoinedJul 12, 2005Messages764

Resources: 3

#### Resources
Maps 1Tutorials 2

Attack-damage detection engine


This system uses Kattana's Handle Vars!


This system is supposed to detect when an attack hits the target unit. It is important, that this system detects only damage coming from attack, so it’s not the same as the WEU trigger ‘Any unit damaged’! (I just heared about it, I don’t use WEU on purpose)


Advantages of this system (over ‘Any unit is attacked’ triggers):


-Detects when the attack HITS the target. Noticable on ranged attacks.


-Does care about “missed” attacks.


-We can get the exact value of the damage!


How it works:


1. First we create a trigger that detects when a unit is attacked (let’s call this the “AttackTrigger” - AT)

 JASS:

 function AttackTrigger_Conditions takes nothing returns boolean
 //Conditions will be put here later.
endfunction

function AttackTrigger_Actions takes nothing returns nothing
 local unit AttackingUnit = GetAttacker() //We store the attacking unit
 local unit AttackedUnit = GetTriggerUnit() //We store the attacked unit - this one will take the damage
endfunction

function InitTrig_AttackDamageDetection takes nothing returns nothing
 local trigger AttackTrigger = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(AttackTrigger, EVENT_PLAYER_UNIT_ATTACKED)
 call TriggerAddCondition(AttackTrigger, Condition( function AttackTrigger_Conditions))
 call TriggerAddAction(AttackTrigger, function AttackTrigger_Actions)
 set AttackTrigger = null
endfunction

From now on, I’ll illuminate the function(s) only that need to be refreshed/completed!


2. The AT creates a new (local) trigger that fires when the attacked unit takes damage. Let’s call this the “DamageTrigger” – DT.

 JASS:

 function DamageTrigger_Conditions takes nothing returns boolean
endfunction

function DamageTrigger_Actions takes nothing returns nothing
endfunction

function AttackTrigger_Actions takes nothing returns nothing
 local unit AttackingUnit = GetAttacker() //We store the attacking unit
 local unit AttackedUnit = GetTriggerUnit() //We store the attacked unit - this one will take the damage
 local trigger DamageTrigger = CreateTrigger() //We create the damage trigger
 call TriggerRegisterUnitEvent(DamageTrigger, AttackedUnit, EVENT_UNIT_DAMAGED)
 call TriggerAddCondition(DamageTrigger, Condition(function DamageTrigger_Conditions))
 call TriggerAddAction(DamageTrigger, function DamageTrigger_Actions)
endfunction


3. Note that there is duration between the attack and the hit (damage)! Think of ranged attacks as an example. In this period, the attacked unit can take damage from other units, so we must filter the one that’s caused by the attacking unit. For this, we must attach/store the attacker onto the DT, and make a check in the condition.

 JASS:

 function DamageTrigger_Conditions takes nothing returns boolean
 return GetEventDamageSource() == GetHandleUnit(GetTriggeringTrigger(), "Attacker")
endfunction

function DamageTrigger_Actions takes nothing returns nothing
endfunction

function AttackTrigger_Actions takes nothing returns nothing
 local unit AttackingUnit = GetAttacker() //We store the attacking unit
 local unit AttackedUnit = GetTriggerUnit() //We store the attacked unit - this one will take the damage
 local trigger DamageTrigger = CreateTrigger() //We create the damage trigger

 call SetHandleHandle(DamageTrigger, "Attacker", AttackingUnit)

 call TriggerRegisterUnitEvent(DamageTrigger, AttackedUnit, EVENT_UNIT_DAMAGED)
 call TriggerAddCondition(DamageTrigger, Condition(function DamageTrigger_Conditions))
 call TriggerAddAction(DamageTrigger, function DamageTrigger_Actions)
endfunction

 I hope it’s clearer now…


4. Now that we filtered the attacker, we can go on to the DT. We have to identify which unit is which. Here it goes:

 JASS:

 function DamageTrigger_Actions takes nothing returns nothing
 local unit AttackedUnit = GetTriggerUnit()
 local unit AttackingUnit = GetEventDamageSource()
 local trigger DamageTrigger = GetTriggeringTrigger()
 call FlushHandleLocals(DamageTrigger)
 call DestroyTrigger(DamageTrigger)
 //Here come the actions you want originally with the attacked/attacking unit.
 set DamageTrigger = null
 set AttackingUnit = null
 set AttackedUnit = null
endfunction


5. We are nearly(!) ready. We still have to deals with bugs that could occur:


The unit attacks, the DT is created, but the attacker misses. The DT remains forever. To eliminate this bug, we have to destroy the DT in the AT after a time period. After that, it’s time to null the locals.

 JASS:

 function AttackTrigger_Actions takes nothing returns nothing
 local unit AttackingUnit = GetAttacker() //We store the attacking unit
 local unit AttackedUnit = GetTriggerUnit() //We store the attacked unit - this one will take the damage
 local trigger DamageTrigger = CreateTrigger() //We create the damage trigger

 call SetHandleHandle(DamageTrigger, "Attacker", AttackingUnit)

 call TriggerRegisterUnitEvent(DamageTrigger, AttackedUnit, EVENT_UNIT_DAMAGED)
 call TriggerAddCondition(DamageTrigger, Condition(function DamageTrigger_Conditions))
 call TriggerAddAction(DamageTrigger, function DamageTrigger_Actions)

 call TriggerSleepAction(1.5)
 call FlushHandleLocals(DamageTrigger)
 call DestroyTrigger(DamageTrigger)

 set DamageTrigger = null
 set AttackingUnit = null
 set AttackingUnit = null
endfunction

 “BUG”


 And here, I have to mention something, because this is the only point where this system is not (it can’t be) 100% accurate. Because of that wait time. Let’s see some situations. Ideally Hunter attacks the Wild with an axe (melee). The DT is created, the damage is taken, the DT is destroyed, everything is just fine. Bug situations:


1) Hunter attacks the Wild with a bow (ranged). Same happens, but let’s say that the arrow does not reach Wild in 1.5 seconds! The DT is already destroyed, so no effect will go off…


2) Hunter attacks, but misses! The DT remains for 1.5 seconds, but what if Hunter’s attack-rate is less than 1.5 (0.8 for instance). He attacks again, creates a new DT, but the other one is still active, so double effect will go off…


For the first bug there’s no solution.


6. Let’s solve bug #2. For this, we must store the DT on the attacker, and make a check in the AT’s condition if there’s an active DT already. I’ll highlight newly needed lines only:

 JASS:

 function AttackTrigger_Conditions takes nothing returns boolean

 return GetHandleTrigger(GetAttacker(), "DamageTrig") == null

endfunction

function DamageTrigger_Conditions takes nothing returns boolean
// return GetEventDamageSource() == GetHandleUnit(GetTriggeringTrigger(), "Attacker")
endfunction

function DamageTrigger_Actions takes nothing returns nothing
// local unit AttackedUnit = GetTriggerUnit()
// local unit AttackingUnit = GetEventDamageSource()
// local trigger DamageTrigger = GetTriggeringTrigger()
// call FlushHandleLocals(DamageTrigger)
// call DestroyTrigger(DamageTrigger)

 call SetHandleHandle(AttackingUnit, "DamageTrig", null)

// set DamageTrigger = null
// set AttackingUnit = null
// set AttackedUnit = null
endfunction
//
//function AttackTrigger_Actions takes nothing returns nothing
// local unit AttackingUnit = GetAttacker() //We store the attacking unit
// local unit AttackedUnit = GetTriggerUnit() //We store the attacked unit - this one will take the damage
// local trigger DamageTrigger = CreateTrigger() //We create the damage trigger

 call SetHandleHandle(AttackingUnit, "DamageTrig", DamageTrigger)

// call SetHandleHandle(DamageTrigger, "Attacker", AttackingUnit)
// call TriggerRegisterUnitEvent(DamageTrigger, AttackedUnit, EVENT_UNIT_DAMAGED)
// call TriggerAddCondition(DamageTrigger, Condition(function DamageTrigger_Conditions))
// call TriggerAddAction(DamageTrigger, function DamageTrigger_Actions)
// call TriggerSleepAction(2)
// call FlushHandleLocals(DamageTrigger)
// call DestroyTrigger(DamageTrigger)

 call SetHandleHandle(AttackingUnit, "DamageTrig", null)

// set DamageTrigger = null
// set AttackingUnit = null
// set AttackingUnit = null
endfunction

function InitTrig_AttackDamageDetection takes nothing returns nothing
// local trigger AttackTrigger = CreateTrigger()
// call TriggerRegisterAnyUnitEventBJ(AttackTrigger, EVENT_PLAYER_UNIT_ATTACKED)
// call TriggerAddCondition(AttackTrigger, Condition( function AttackTrigger_Conditions))
// call TriggerAddAction(AttackTrigger, function AttackTrigger_Actions)
// set AttackTrigger = null
endfunction


7. The final code should look like this:


This is an “artificial” critical strike (100% chance, 2x damage, without damage display just to keep the code clean).

 JASS:

 function AttackTrigger_Conditions takes nothing returns boolean
 return GetHandleTrigger(GetAttacker(), "DamageTrig") == null and GetUnitAbilityLevel(GetAttacker(), 'A000') > 0
endfunction

function DamageTrigger_Conditions takes nothing returns boolean
 return GetEventDamageSource() == GetHandleUnit(GetTriggeringTrigger(), "Attacker")
endfunction

function DamageTrigger_Actions takes nothing returns nothing
 local unit AttackedUnit = GetTriggerUnit()
 local unit AttackingUnit = GetEventDamageSource()
 local trigger DamageTrigger = GetTriggeringTrigger()
 call FlushHandleLocals(DamageTrigger)
 call DestroyTrigger(DamageTrigger)
 call SetHandleHandle(AttackingUnit, "DamageTrig", null)
 //Here come the actions you want originally with the attacked/attacking unit.
 call UnitDamageTarget(AttackingUnit, AttackedUnit, GetEventDamage(), true, false, ATTACK_TYPE_CHAOS, DAMAGE_TYPE_UNIVERSAL, null)
 set DamageTrigger = null
 set AttackingUnit = null
 set AttackedUnit = null
endfunction

function AttackTrigger_Actions takes nothing returns nothing
 local unit AttackingUnit = GetAttacker() //We store the attacking unit
 local unit AttackedUnit = GetTriggerUnit() //We store the attacked unit - this one will take the damage
 local trigger DamageTrigger = CreateTrigger() //We create the damage trigger
 call SetHandleHandle(AttackingUnit, "DamageTrig", DamageTrigger)
 call SetHandleHandle(DamageTrigger, "Attacker", AttackingUnit)
 call TriggerRegisterUnitEvent(DamageTrigger, AttackedUnit, EVENT_UNIT_DAMAGED)
 call TriggerAddCondition(DamageTrigger, Condition(function DamageTrigger_Conditions))
 call TriggerAddAction(DamageTrigger, function DamageTrigger_Actions)
 call TriggerSleepAction(2)
 call FlushHandleLocals(DamageTrigger)
 call DestroyTrigger(DamageTrigger)
 call SetHandleHandle(AttackingUnit, "DamageTrig", null)
 set DamageTrigger = null
 set AttackingUnit = null
 set AttackingUnit = null
endfunction

function InitTrig_AttackDamageDetection takes nothing returns nothing
 local trigger AttackTrigger = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(AttackTrigger, EVENT_PLAYER_UNIT_ATTACKED)
 call TriggerAddCondition(AttackTrigger, Condition( function AttackTrigger_Conditions))
 call TriggerAddAction(AttackTrigger, function AttackTrigger_Actions)
 set AttackTrigger = null
endfunction


 Last edited by a moderator: Sep 1, 2007

#### Diablo-dk

#### Diablo-dk

 Level 10

JoinedNov 10, 2004Messages351

Resources: 3

#### Resources
Maps 1Spells and Systems 1Tutorials 1

Hmm, it seems like you are leaking a triggeraction. use TriggerRemoveAction for that.

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Also, you need to remove the condition too. In addition to that, the Condition function returns a boolexpr which needs to be destroyed too (either that or just make it a global variable and use the global every time).

#### paskovich

#### paskovich

 Level 11

JoinedJul 12, 2005Messages764

Resources: 3

#### Resources
Maps 1Tutorials 2

Umm... WHAT?


Could you give me an example on how to make a local trigger leakless?


-what to store


-what to destroy/remove


-what actions to use

#### Diablo-dk

#### Diablo-dk

 Level 10

JoinedNov 10, 2004Messages351

Resources: 3

#### Resources
Maps 1Spells and Systems 1Tutorials 1

acording to Vexorian boolexpr's doesent leak when you use Condtition() I think it actually is risky destroying them.

Easy to make a local trigger leakless.

 JASS:

call SetHandleHandle(DamageTrigger,TriggerAddAction)(DamageTrigger, function DamageTrigger_Actions,"Action")
call SetHandleHandle(DamageTrigger,TriggerAddCondition(DamageTrigger, Condition(function DamageTrigger_Conditions)),"Cond")
// to destroy:
call TriggerRemoveAction(GetHandleTriggerAction(DamageTrigger,"Action"))
call TriggerRemoveCondition(GetHandleCondition(DamageTrigger,"Cond")) //not 100% sure on if this one should be removed. i guess it should.
call DestroyTrigger(DamageTrigger)

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

I tend to remove triggerconditions for safety, but I can't say I know if they leak.

Anyways, I'm pretty sure you leak a boolexpr with Condition, never heard anything against it as you said, so I would do it to be safe.

Meh, I guess it's something that's worth testing, though.

#### Diablo-dk

#### Diablo-dk

 Level 10

JoinedNov 10, 2004Messages351

Resources: 3

#### Resources
Maps 1Spells and Systems 1Tutorials 1

http://www.wc3campaigns.net/showthread.php?t=93168&highlight=boolexprhas been discussed alot at wc3c, tho boolexpr's made with functions like And() Or() or Not() does have to be destroyed.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Insane. The most useful thing to me there was the GroupEnum... thing, but anyways, insane.

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

hm... in that case, only one person can either approve or disapprove this

VEXORIAN

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Vexorian doesn't visit here, as far as I know.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

All handles leak, and I believe that includeds boolexpr since why should it be an exception, but I believe in the common use for it it does not leak and thus it does not leak but if you make triggers constantly as part of a spell it probably will leak.

 JASS:

native TriggerAddAction takes trigger whichTrigger,code actionFunc returns triggeraction
native TriggerAddCondition takes trigger whichTrigger,boolexpr condition returns triggercondition
//BLANK LINE

This is the specification of the functions we are talking about.

 JASS:

native TriggerRemoveAction takes trigger whichTrigger,triggeraction whichAction returns nothing
native TriggerRemoveCondition takes trigger whichTrigger,triggercondition whichCondition returns nothing
//BLANK LINE

These are the functions that diablo recomended to remove the leak, notice that they require the storing of the handles of triggeraction and triggercondition when you make them to use them and thus it would require more handle opperations making it inconveinent to use and could result overall lower map preformance on slow computers. These seem too inconvinent to really try and use alot.

 JASS:

native TriggerClearActions takes trigger whichTrigger returns nothing
native TriggerClearConditions takes trigger whichTrigger returns nothing
//BLANK LINE

Now correct me if I am wrong but do these not do exactly the same as the ones diablo recommended but better since it requires nothing but the trigger and it removes all the conditions/actions from the trigger? If so you couldeasily fix it via adding these onto the trigger destruction section to help flush it and much more efficently than Diablo's idea.


So I say this should be approved once this minor action/condition leak is fixed and who really cares if the removing of the boolexpr is not necescary but it is better to be safe than sorry (ruin others maps with leaks).


You may also concider submiting a version of this to the JASS functions section if you have not already.


You may also like to add how to make it single unit specfic (or include the code for that) since some people may not want it to trigger when every unit attacks especially if it is for just 1 heroes ability.


I would like to remind members that vexorian is not part of this site and thus do not yell for him when some of our trigger moderators have no idea about something.


 Last edited: Jul 8, 2007

#### Bobo_The_Kodo

#### Bobo_The_Kodo

 Level 5

JoinedJan 15, 2007Messages199

TriggerClearActions does leak after several tests, you do need to do TriggerRemoveAction()

#### gorillabull

#### gorillabull

 Level 17

JoinedJul 17, 2011Messages1,863

Resources: 5

#### Resources
Skins 1Spells 4

i like the approach but this is insufficient and extremely basic. example there is a damage over time on the unit.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

gorillabull said:

 i like the approach but this is insufficient and extremely basic. example there is a damage over time on the unit.


Click to expand...


it was 7 years ago man

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Outdated.

Do not re-invent the wheel. This has fewer features than modern DDS systems which I suggest you study instead of using this as a baseline to make your own. Of course, you do pay a size-tax for that, but that's hardly an issue for 99.9% of people.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Chaosy said:

 Do not re-invent the wheel. This has fewer features than modern DDS systems which I suggest you study instead of using this as a baseline to make your own. Of course, you do pay a size-tax for that, but that's hardly an issue for 99.9% of people.


Click to expand...

Of course it is out dated. This system was made over 14 years ago. Some WC3 players were not even born when the author was making it...

 You must log in or register to reply here.


### Similar threads

Locked

[JASS]
 Help Customizing a Damage Detection


MightyBobBarker

Jun 26, 2008

Triggers & Scripts

Replies3Views2K

Jun 27, 2008
MightyBobBarker

Locked

[JASS]
 Damage Help


Beast(TrOg)

Apr 12, 2009

Triggers & Scripts

Replies2Views752

Apr 12, 2009
maskedpoptart

 [Snippet] DamageEvent


Nestharus

Jan 8, 2011

Graveyard
2

Replies69Views24K

Mar 9, 2013
Nestharus

Locked

 text wrapping on code?


Nestharus

Jan 10, 2013

Site Discussion

Replies2Views1K

Jan 11, 2013
Almia

Locked

 Advice please. I'm going to learn JASS...


Greenwhy

Mar 8, 2012

World Editor Help Zone

Replies12Views3K

Mar 10, 2012
Greenwhy

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