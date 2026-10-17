---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/vjass-meet-vjass-extending-structs.267719/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# vJASS Meet vJASS Extending Structs
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/vjass-meet-vjass-extending-structs.267719/>
#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

 Today's topic: Extending structs.

Basics​

Extending structs​

Miscellaneous​

Introduction

Constructor & Destructor

Stub Methods & super

Generated Code

Extends array

typeid & getType()

Conclusion

Demo Code

Introduction


These days it appears to be a dying art for vJass resources, hence I want to raise interest on this easily readable way of coding.


So before going into details, let's make sure you have a faint idea what extending structs are.


You're probably aware of struct A or struct A extends array.


The topic for today is struct A extends B, how it works and how to use it.


I will also talk about finally compiled code when using this feature of vJass.

 Quote from JassHelper manual


You base a struct on a previously declared struct, by doing so your new type aquires methods and variable members of the base struct.


Furthermore it is able to use instances of this type with functions and variables of the base type.

Constructor & Destructor


Let's quickly repeat allocation and deallocation of structs extending other structs.


For demonstration I will make a realistic spell name: Hydra ( the Diablo II & Diablo III spell )

 JASS:

struct Hydra
endstruct

 JASS:

struct FireHydra extends Hydra
endstruct

allocate()​
When using FireHydra.allocate() you will end up calling Hydra.allocate() instead.

create()​
You can use FireHydra.create() instead of FireHydra.allocate(). It will also use the parent structs constructor instead.


If you override Hydra.create() with your own custom create method the create method for child structs ( FireHydra ) will require the same arguments.

destroy()​
When destroying a FireHydra instance, Hydra's destructor is called instead and FireHydra's onDestroy method is evaluated via trigger.


Note: The onDestroy[structid] trigger is always evaluated, independent of method onDestroy beeing declared or not.

deallocate()​
In case you override method destroy with a custom destroy method, use method deallocate to call the normal destroy method.

### Code

### After JassHelper

 JASS:

// In the example Hydra is our parent struct.
struct Hydra
 player owner// Child structs can use members of the parent struct
 // aslong as they are "not" private!

 // Child structs can use methods of parent structs.
 method fire takes unit target returns nothing
 endmethod

endstruct

// In the example FireHydra is a child struct extending Hydra.
struct FireHydra extends Hydra

endstruct

 JASS:

globals

//JASSHelper struct globals:
constant integer si__Hydra=1// Hydra struct id
integer si__Hydra_F=0 // Free indexes
integer si__Hydra_I=0 // Index count
integer array si__Hydra_V // Reprents existing instances.
player array s__Hydra_owner

constant integer si__FireHydra=2 // FireHydra struct id
integer array si__Hydra_type // Stores which type an instance is ( i.e. is it FireHydra? --> 2 )
trigger array st__Hydra_onDestroy// Evaluate proper onDestroy method.
integer f__arg_this // Global to transfer data via TriggerEvaluate

endglobals


//Generated allocator of Hydra
function s__Hydra__allocate takes nothing returns integer
 local integer this=si__Hydra_F
 if (this!=0) then
 set si__Hydra_F=si__Hydra_V[this]
 else
 set si__Hydra_I=si__Hydra_I+1
 set this=si__Hydra_I
 endif
 if (this>8190) then
 call DisplayTimedTextToPlayer(GetLocalPlayer(),0,0,1000.,"Unable to allocate id for an object of type: Hydra")
 return 0
 endif

 set si__Hydra_type[this]=1
 set si__Hydra_V[this]=-1
 return this
endfunction

//Generated destructor of Hydra
function sc__Hydra_deallocate takes integer this returns nothing
 if this==null then
 call DisplayTimedTextToPlayer(GetLocalPlayer(),0,0,1000.,"Attempt to destroy a null struct of type: Hydra")
 return
 elseif (si__Hydra_V[this]!=-1) then
 call DisplayTimedTextToPlayer(GetLocalPlayer(),0,0,1000.,"Double free of type: Hydra")
 return
 endif
 set f__arg_this=this
 call TriggerEvaluate(st__Hydra_onDestroy[si__Hydra_type[this]])
 set si__Hydra_V[this]=si__Hydra_F
 set si__Hydra_F=this
endfunction

//Generated allocator of FireHydra
function s__FireHydra__allocate takes nothing returns integer
 local integer this=s__Hydra__allocate()// Calls Hydra's allocator instead.
 local integer kthis
 if(this==0) then
 return 0
 endif
 set si__Hydra_type[this]=2// Sets the type
 set kthis=this

 return this
endfunction

stub methods & super


A method declared in your parent struct can't be re-declared in your child struct. The JassHelper will throw an error:

 Error


Member name already in use by a parent type

Now a stub method behaves like a normal method, but can be overridden in your child struct.


Make use of this feature, if you want different method behaviours in your child struct than in your parent struct. ( i.e. a filter )

super forces the parent method to be called, you might need this when extending an interface or avoiding an overrriden stub method.


I guess the usage of super is very seldom to non-existent.


An example is better than 1000 words:

 JASS:

// In the example Hydra is our parent struct.
struct Hydra

 stub method onCreate takes unit new returns nothing
 endmethod

 stub method onFilter takes unit filterUnit returns boolean
 return true
 endmethod

 // Child structs can use methods of parent structs.
 method fire takes unit target returns nothing
 endmethod

endstruct

// In the example FireHydra is a child struct extending Hydra.
struct FireHydra extends Hydra

 // overrrides stub method onCreate.
 method onCreate takes unit new returns nothing
 call BJDebugMsg("parent onCreate overridden")
 call super.onCreate(new)// Will now also call the parent struct onCreate method.
 endmethod

 // overrides stub method onFilter. Returned boolean may now also be false.
 method onFilter takes unit filterUnit returns boolean
 return UnitAlive(filterUnit)
 endmethod

 // Will throw an error, because method fire is already declared in Hydra!
 method fire takes unit target returns nothing
 endmethod
endstruct

 Limitation


It is impossible to call a stub method within the parent creator

 JASS:

library A

 struct A

 // Eval via call TriggerEvaluate(st__A_onCreate[si__A_type[this]])
 public stub method onCreate takes nothing returns nothing
 // Will always run
 endmethod

 static method create takes nothing returns thistype
 local thistype this = thistype.allocate()// set si__A_type[this]=1

 call this.onCreate()// Evaluates st__A_onCreate[si__A_type[this]]!

 return this// Returned to a function, which set si__A_type[this]=2
 endmethod

 endstruct

 struct B extends A

 private method onCreate takes nothing returns nothing
 // Will never run
 endmethod

 static method doit takes nothing returns nothing
 local B this = B.create()// here si__A_type[this]=2
 call Print(I2S(this.getType()))// Prints 2
 endmethod

 endstruct

endlibrary

Generated Code


One could think: Quite awesome this vJass!


But it is very important to analyse the code generated behind stub method and extending structs.


Constructor and destructors stay as described above, but how is it possible to "override" stub methods?

stub method​
Stub methods create a trigger array a static integer, a static type for the returner and for each function argument a respective type variable.


Variables of the same type are shared across structs in your map to reduce overall used globals to a minimum.


These variables are required to pass over agruments when using trigger evaluations.

overridden stub methods​
Once re-declared in your child struct, stub methods are accessed via TriggerEvaluate(trigger[structid]).


So we have an additional triggeraction and triggercondition registered to the above mentioned trigger array. The array index is the child struct id.

super​
super doesn't create any extra code. It compiles to a simple function call to the parent struct method.

### code

### After JassHelper

 JASS:

// In the example Hydra is our parent struct.
struct Hydra

 stub method onCreate takes unit new returns nothing
 call BJDebugMsg("- parent -")
 endmethod

endstruct

// In the example FireHydra is a child struct extending Hydra.
struct FireHydra extends Hydra

 // overrides stub method onCreate.
 method onCreate takes unit new returns nothing
 call BJDebugMsg("parent onCreate overridden")
 endmethod

endstruct

 JASS:

//JASSHelper struct globals:
constant integer si__Hydra=1
integer si__Hydra_F=0
integer si__Hydra_I=0
integer array si__Hydra_V
constant integer si__FireHydra=2
trigger array st__Hydra_onCreate// trigger array for stub method onCreate
integer array si__Hydra_type
trigger array st__Hydra_onDestroy
unit f__arg_unit1
integer f__arg_this

endglobals

// Generated method caller for Hydra.onCreate
// This method is always called onCreate, from here the proper ( stub ) method is evaluated via trigger.
// Arguments are passed via globals as you can see.
function sc__Hydra_onCreate takes integer this,unit new returns nothing
 set f__arg_this=this
 set f__arg_unit1=new
 call TriggerEvaluate(st__Hydra_onCreate[si__Hydra_type[this]])
endfunction

//Generated method caller for FireHydra.onCreate
function sc__FireHydra_onCreate takes integer this,unit new returns nothing
 call BJDebugMsg("parent onCreate overridden")
endfunction

function s__Hydra_onCreate takes integer this,unit new returns nothing
 call BJDebugMsg("- parent -")
endfunction
function s__FireHydra_onCreate takes integer this,unit new returns nothing
 call BJDebugMsg("parent onCreate overridden")
endfunction

// Struct method generated initializers/callers:
// First for Hydra the second for FireHydra.
// f__arg_this, and f__arg_unit1 are set in the trigger conditions.
function sa__Hydra_onCreate takes nothing returns boolean
 call s__Hydra_onCreate(f__arg_this,f__arg_unit1)
 return true
endfunction
function sa__FireHydra_onCreate takes nothing returns boolean
 call s__FireHydra_onCreate(f__arg_this,f__arg_unit1)
 return true
endfunction

// Array index is the constant struct id.
function jasshelper__initstructs325686859 takes nothing returns nothing
 // Hydra onCreate ( Parent struct )
 set st__Hydra_onCreate[1]=CreateTrigger()
 call TriggerAddCondition(st__Hydra_onCreate[1],Condition( function sa__Hydra_onCreate))
 call TriggerAddAction(st__Hydra_onCreate[1], function sa__Hydra_onCreate)

 // FireHydra onCreate ( Child struct )
 set st__Hydra_onCreate[2]=CreateTrigger()
 call TriggerAddCondition(st__Hydra_onCreate[2],Condition( function sa__FireHydra_onCreate))
 call TriggerAddAction(st__Hydra_onCreate[2], function sa__FireHydra_onCreate)

 // Hydra and FireHydra onDestroy ( not declared, therefore null )
 // Even though they do no exists, the trigger is still evaluated.
 set st__Hydra_onDestroy[1]=null
 set st__Hydra_onDestroy[2]=null
endfunction

function main takes nothing returns nothing
 // ... Blizzard code, init stuff

 // Struct initializer are executed via ExecuteFunc native.
 call ExecuteFunc("jasshelper__initstructs325686859")
endfunction

Array Structs


Structs extending array ( i.e. struct Hydra extends array ) are not discussed in detail in this tutorial.


What you should know is that array structs do not generate a struct creator and destructor. You have to do all of this by yourself.


You want to use array structs for the . syntax and for struct fields. ( JassHelper link )


Side note: struct extends array is also recommended for large scale systems,


as high frequency trigger evaluations ( onDestroy, stub methods, ... ) can have a negative impact on the overall computation time.

typeid & getType()


Let's imagine you want to code more different Hydra type spells in your map ( i.e. Ice, Lightning, Venom-Hydra ... ).


How is it possible to distinguish which type of Hydra an instance is? Remember they are all allocated via our parent struct: Hydra.

thistype.typeid​
Each struct has an id represented as constant integer ( i.e. constant integer si__FireHydra=2 )


You can read the struct id by using struct.typeid ( i.e. FireHydra.typeid has here the value 2 )

getType()​
Method instance.getType() returns the struct id ( see above ) of an allocated instance.


In FireHydra.allocate() ( see code analysis ) set si__Hydra_type[this]=2 defines the type this instance is.


These fields allow the following comparison: hydra.getType() == FireHydra.typeid.

Conclusion


Many people postulate that extending structs should be avoided due to extra code generation and trigger evaluations.


We can't deny that stub methods are not top performers in vJass coding ( due to trigger evaluations ),


however the syntax is very readable and easy to use. Performance-wise you will not realise a difference ingame in 99% of all cases.


As an example extending structs are beneficial when it comes to coding spell resources for you map. ( reconsider my Hydra example )


Alternative ways of coding ( eventually faster ) with modules combined with static ifs and Events are often really hard to understand at a glance.

I totally recommend to try out extending structs for clean, readable OOP coding in vJass.

Demo Code


Some links to resources on THW using the features discussed above:

BuffHandler by Muzzel

Custom Projectile by Berb

Hydra by BPower


Thank you for your attention.


 Last edited by a moderator: Oct 23, 2015

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Not sure if we need such kind of tutorial.

If you think it's useful, I willl polish it up a little bit more and work on the content.

Especially point out Demo Code, Conclusion and extends array.

So please feedback

Edit: I should also talk about getType() and thistype.typeid


 Last edited: Jul 16, 2015

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

I think the tutorial is pretty well done. I read it all and understood everything at first glance (though, I kinda knew how it worked already, so this view might be different from someone who doesn't know about extends).

Yes, getType() and thistype.typeid are crucial for this; Looking forward to that chapter.

I think the conclusion should mention that one of the main applications for struct extensions is actually coding spells.

In most maps, the runtime efficiency of spells is determined by the taxing handle operations (creating units, moving units, dealing damage) or enumerations, while the overall speed impact of a trigger evaluation more or less does seldom matter in the grand scheme of things. After all, if thousands of instances of your spell are active at the same time, this is not about efficiency anymore, but just bad game design.

Extending structs is definitely not the preferred option for fundamental systems where speed efficiency matters.

But it is a very elegant solution for coding spells.

I also like your Hydra example; very descriptive as lots of people will instantly remember what this spell does in Diablo and it's a great showcase for when extending structs is useful, as fire/ice/lightning hydra work somewhat similar, but not quite the same.

Fire has a DoT which requires a periodic method.

Ice applies a timed frost nova effect.

Lightning has a chance to stun the enemy and doesn't shoot moving missiles, but deals instant damage.

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

 Yes, getType() and thistype.typeid are crucial for this; Looking forward to that chapter.


Click to expand...

Definitly will do this.

 Extending structs is definitely not the preferred option for fundamental systems where speed efficiency matters.

But it is a very elegant solution for coding spells.


Click to expand...

Yes

Nice that you like it.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

I really like it.

 JASS:

// overwrites stub method onFilter. Returned boolean may now also false.
 method onFilter takes unit filterUnit returns boolean
 return UnitAlive(filterUnit)
 endmethod

^Here


"Returned boolean may now also false."


->


"Returned boolean may now also be false."


Maybe it could quickly be noted that extends array won't extend a real upper type.

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Thanks. I'll try to edit it in the evening.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

you dont overwrite, you override methods(just a wording)

"We can't denie" -> We can't deny.

Useful? Probably, needed? very much.

Nowadays, people actually fear using such features because of getting banished by performance elitists saying that "YOUR SYSTEM RUNS 1 MICROSECOND SLOWER DIE IN HELLFIRE!"

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Thank you. I wrote the tutorial in one piece and sure made some grammar or spelling mistakes.

I will correct them soon and polish everything a bit up.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

It's not 1 microsecond slower ; ). Extending structs spams trigger evaluations. For example, destroy turns into a trigger eval. This is to support polymorphism. It's ok for most code, but for large scale systems it's a big no-no.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

I know of exactly 2 "large scales systems", compared to the amounts of hatred towards these features, thats quite low number

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Updated the tutorial a bit. I added information about struct field typeid and getType().

Demo code is still lacking. I will see if I find good examples.

Furthermore I will update my Hydra spell in the spell section.

( the old version stays, but I will add a second one demonstrating in this tutorial content )

I didn't use the word inheritance in my text. Maybe I should.

Is there anything really important missing, which I should add to the tutorial?

It was fun to write, dependant on the feedback I will make another one about delegate.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

I like that the tutorial is short and to the point.

But yes, a few more examples would be nice. The little sub-chapter about struct extends array sticks out a bit, though. I'm not sure if this should be part of this tutorial, as it's basicly a hack we use to override the default vJass implementation of structs.

It should imho just be mentioned at the bottom of the tutorial and that it should not be used in combination with struct A extends B (unless you really know what you're doing), then just link to a dedicated tutorial to struct extends array.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Everything I learned from struct extension I learned from Berbanog's projectile. www.hiveworkshop.com/forums/jass-resources-412/system-custom-projectiles-162121/

He uses all of these fancy methods and it was very exciting to see how he used these things.

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Added to demo.

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Added additional information about a limitation when using stub methods:

- An onCreate stub method cannot be properly evaluated, ergo is not working.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

There is no reason that this.onCreate() inside A should call As child's onCreate(such as B).

That is because when you do this.onCreate, the this variable will be A and not B, since you are inside A not B.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

edo494 said:

 There is no reason that this.onCreate() inside A should call As child's onCreate(such as B).

That is because when you do this.onCreate, the this variable will be A and not B, since you are inside A not B.


Click to expand...

That is not how inheritance works. The parent struct is always supposed to call the child methods if they exist, no matter if you look at the child or parent type.

If B extends A (and you are looking at A), then all members of A must still call B's methods (if struct A is actually a type B).

An example of why that is required can be found directly in the core game mechanics: unit extends widget extends handle

If I kill a widget that is actually a unit, it will still fire all unit related mechanics aswell (like a unit death event), because it is, in fact, not a widget, but a unit.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

ah well ok, sometimes when you dont see * and & you forget that they are still meant to be pointers/references.

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

I added my Hydra spell to the demo code example.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Sweet, thanks for making this.

Approved!

#### Crystalknoct

#### Crystalknoct

 Level 7

JoinedAug 15, 2012Messages318

Resources: 2

#### Resources
Maps 1Spells 1

thanks will make learning easier.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Why does stub methods generate triggeractions too if there's already a triggercondition? Seems redundant if you ask me.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Flux said:

 Why does stub methods generate triggeractions too if there's already a triggercondition? Seems redundant if you ask me.


Click to expand...

Yeah, it is redundant. I think a lot of the jasshelper features were implemented with the plan that the optimizer could come in and remove that redundancy. I don't think it does, though.

#### Deleted member 219079

D

#### Deleted member 219079

When I learned vJASS, I was told to stay away from extends. So I did. I can only give the same suggestion here.

#### LeP

#### LeP

 Level 13

JoinedFeb 13, 2008Messages548

jondrean said:

 When I learned vJASS, I was told to stay away from extends. So I did. I can only give the same suggestion here.


Click to expand...

that's just some cargo cult bs.

#### Deleted member 219079

D

#### Deleted member 219079

Well I remember inspecting myself what JassHelper spews out when using extends, it wasn't anything nice. JASS2 is already slow af why make it slower?

Besides I don't want to be a rebel, I'm doing what I've been taught. WC3 modding has been thing for over a decade so we know what's fast and what is not.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

jondrean said:

 Well I remember inspecting myself what JassHelper spews out when using extends, it wasn't anything nice. JASS2 is already slow af why make it slower?

Besides I don't want to be a rebel, I'm doing what I've been taught. WC3 modding has been thing for over a decade so we know what's fast and what is not.


Click to expand...

In most of the use-cases for struct extension, you don't care about minor speed differences.

Extends is for readability and modular code. It's not for speed-freaks.

I personally adore extends and use it whereever it makes sense. It creates neat, clean and - most importantly - highly maintainable code that you can easily modify or add features to later.

Would I use it for speed-critical systems like damage detection? Hell no.

But for stuff where speed optimization isn't top priority, like spells, buffs, etc.? Fuck yes!

A great example for the clever use of extends is having basic spell components that you can use for rapid-prototyping new abilities.

I basicly built base structs for "cast time spell", "basic direct missile spell", "basic cone-type spell", "basic line-type spell", "basic buff placer", etc. that I can extend upon whenever I need a spell to do something out of the norm. It saves so much time to be able to use pre-build structures and just add whatever the specific spell you code adds to the table instead of recreating everything from scratch every time. Plus, if I want to add a feature retroactively to all the spells of one group (let's say, I decided to add a mechanic that deflects missiles), I can simply edit the base struct and have it apply to all derivates automaticly.

#### Deleted member 219079

D

#### Deleted member 219079

Yes the concept is nice, we just need a good implementation. My own will have a pool around variable, pool of what checks to include.

 JASS:

abstract struct Parent
 void printMsg()

struct ChildA : Parent
 void printMsg()
 BJDebugMsg("A")

struct ChildB : Parent
 void printMsg()
 BJDebugMsg("B")

struct ChildC : Parent
 void printMsg()
 BJDebugMsg("C")

INIT
 Parent myInstance = ChildB.create()
 myInstance.printMsg()
 Parent myInstance2 = ChildA.create()
 myInstance2 = ChildC.create()
 myInstance2.printMsg()

Pool for myInstance has ChildB only, translates into call ChildB_printMsg(myInstance).

Pool for myInstance2 has ChildA and ChildC, translates into:

 JASS:

if(myInstance2_id>1)then // binary search
 call ChildC_printMsg(myInstance2)
else // omit B (1) check, not in the pool
 call ChildA_printMsg(myInstance2)
endif

Ruke's vrJASS has better implementation as well. Also I think Nestharus is working on his own JassHelper alternative.

#### Prometheus3375

#### Prometheus3375

 Level 9

JoinedJul 20, 2018Messages177

Flux said:

 Why does stub methods generate triggeractions too if there's already a triggercondition? Seems redundant if you ask me.


Click to expand...

PurgeandFire said:

 Yeah, it is redundant. I think a lot of the jasshelper features were implemented with the plan that the optimizer could come in and remove that redundancy. I don't think it does, though.


Click to expand...

All methods generate triggeractions and triggerconditions. triggeractions are used when you call a method using .execute().


 Last edited: Sep 1, 2018

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Prometheus3375 said:

 All methods generate triggeractions and triggerconditions. triggeractions are used when you call a method using .execute().


Click to expand...

Yes, the idea of what they're saying is that it's often redundant, and triples the code length.

 You must log in or register to reply here.


### Similar threads

 Structs for Dummies


Magtheridon96

Jun 15, 2011

JASS Tutorials

Replies19Views26K

May 3, 2016
Zwiebelchen

Locked

[JASS]
 (vJass) Inheritance problem


Anachron

Mar 31, 2010

Triggers & Scripts

Replies3Views2K

Apr 1, 2010
Anachron

 Introduction to Struct


iAyanami

Nov 20, 2011

JASS Tutorials

Replies19Views12K

Mar 10, 2014
muzzel

Locked

[JASS]
 Stub - Parent - Child - Thistype - Struct Problem


Anachron

Aug 28, 2009

Triggers & Scripts

Replies26Views4K

Aug 30, 2009
Element of Water

Locked

 Issues with structs in vJass


Prometheus3375

Sep 2, 2018

Advanced Scripting

Replies12Views3K

Feb 11, 2019
Overfrost

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