---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/all-about-inlining.277201/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# All About Inlining
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/all-about-inlining.277201/>
#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

All About Inlining:
Inlining is replacing the call site of a function or the lookup of a constant variable by its content making the code run faster since no function call or variable lookup actually takes place. It happens upon compiling the script (Function Inlining) or optimizing the map script (Constant Inlining).

 Contents


1. Function Inlining

2. Constant Inlining

Part 1: Function Inlining

Function inlining is one of the features of vJASS Optimization allowing readable yet efficient code. However functions that get inlined still remain in the map script (even thought it is unused) so it still takes up space in the map script (unless Wc3 optimizer is used).


 JassHelper Manual said:



 Function inlining will look for function calls that can be inlined and then just convert their calls to a direct usage of the function's contents.


Click to expand...

Here are some rules regarding function inlining (as of JassHelper 0.A.2.B included in Jass New Gen Pack 2.0):

The function must be a one-liner

 JASS:

 function Inline1 takes integer i1, integer i2 returns integer
 return i1 + i2
 endfunction

 function Inline2 takes integer i1, integer i2 returns integer
 local integer answer = 42
 return i1 + i2
 endfunction

 function I takes nothing returns nothing
 local integer i = Inline1(1, 2)
 local integer j = Inline2(1, 2)
 endfunction

JassHelper generated script:

 JASS:

 function Inline1 takes integer i1,integer i2 returns integer
 return i1 + i2
 endfunction

 function Inline2 takes integer i1,integer i2 returns integer
 local integer answer= 42
 return i1 + i2
 endfunction

 function I takes nothing returns nothing
 local integer i= ((1 ) + ( 2)) // INLINED!!
 local integer j= Inline2(1 , 2)
 endfunction

BJ Functions don't get inlined, but custom functions that do the same thing get inlined

 JASS:

 function UnitDamageTargetCustom takes unit whichUnit, unit target, real amount, attacktype whichAttack, damagetype whichDamage returns boolean
 return UnitDamageTarget(whichUnit, target, amount, true, false, whichAttack, whichDamage, WEAPON_TYPE_WHOKNOWS)
 endfunction

 function CosDeg takes real r returns real
 return Cos(r*bj_RADTODEG)
 endfunction

 function F takes nothing returns nothing
 local unit u1
 local unit u2
 local real r1 = CosBJ(42)
 local real r2 = CosDeg(42)
 call UnitDamageTargetBJ(u1, u2, 0, ATTACK_TYPE_NORMAL, DAMAGE_TYPE_NORMAL)
 call UnitDamageTargetCustom(u1, u2, 0, ATTACK_TYPE_NORMAL, DAMAGE_TYPE_NORMAL)
 endfunction

JassHelper generated script:

 JASS:

 function UnitDamageTargetCustom takes unit whichUnit,unit target,real amount,attacktype whichAttack,damagetype whichDamage returns boolean
 return UnitDamageTarget(whichUnit, target, amount, true, false, whichAttack, whichDamage, WEAPON_TYPE_WHOKNOWS)
 endfunction

 function CosDeg takes real r returns real
 return Cos(r * bj_RADTODEG)
 endfunction

 function F takes nothing returns nothing
 local unit u1
 local unit u2
 local real r1= CosBJ(42)
 local real r2= (Cos(((42)*1.0) * bj_RADTODEG)) // INLINED!!
 call UnitDamageTargetBJ(u1, u2, 0, ATTACK_TYPE_NORMAL, DAMAGE_TYPE_NORMAL)
 call UnitDamageTarget((u1 ), ( u2 ), (( 0 )*1.0), true, false, ( ATTACK_TYPE_NORMAL ), ( DAMAGE_TYPE_NORMAL), WEAPON_TYPE_WHOKNOWS) // INLINED!!
 endfunction

Wc3 Map optimizer generated script:

(BJs are still not inlined but unused functions (UnitDamageTargetCustom and CosDeg) are removed

 JASS:

function F takes nothing returns nothing
local unit u1
local unit u2
local real r1=CosBJ(42)
local real r2=(Cos(((42)*1.)*bj_RADTODEG))
call UnitDamageTargetBJ(u1,u2,0,ATTACK_TYPE_NORMAL,DAMAGE_TYPE_NORMAL)
call UnitDamageTarget((u1),(u2),((0)*1.),true,false,(ATTACK_TYPE_NORMAL),(DAMAGE_TYPE_NORMAL),WEAPON_TYPE_WHOKNOWS)
endfunction

The order of the input arguments must match how they appear in the custom function

 JASS:

 //Correct order
 function Inline1 takes integer i1, integer i2 returns integer
 return i1 + i2
 endfunction

 //Incorrect order
 function Inline2 takes integer i1, integer i2 returns integer
 return i2 + i1
 endfunction

 function I takes nothing returns nothing
 local integer i = Inline1(1, 2)
 local integer j = Inline2(1, 2)
 endfunction

JassHelper generated script:

 JASS:

 //Correct order
 function Inline1 takes integer i1,integer i2 returns integer
 return i1 + i2
 endfunction

 //Incorrect order
 function Inline2 takes integer i1,integer i2 returns integer
 return i2 + i1
 endfunction

 function I takes nothing returns nothing
 local integer i= ((1 ) + ( 2)) // INLINED!!
 local integer j= Inline2(1 , 2)
 endfunction

Each argument must only appear once in the function.

 JASS:

 //Correct way
 function Inline1 takes integer i1, integer i2 returns real
 return 3*i1 + 5.0*i2
 endfunction

 //Incorrect way
 function Inline2 takes integer i1, integer i2 returns real
 return i1 + 2*i1 + 5.0*i2
 endfunction

 function I takes nothing returns nothing
 local real r1 = Inline1(1, 2)
 local real r2 = Inline2(1, 2)
 endfunction

JassHelper generated script:

 JASS:

 //Correct way
 function Inline1 takes integer i1,integer i2 returns real
 return 3 * i1 + 5.0 * i2
 endfunction

 //Incorrect way
 function Inline2 takes integer i1,integer i2 returns real
 return i1 + 2 * i1 + 5.0 * i2
 endfunction

 function I takes nothing returns nothing
 local real r1= (3 * (1 ) + 5.0 * ( 2)) // INLINED!!
 local real r2= Inline2(1 , 2)
 endfunction

Directly setting a variable also gets inlined, but it must not take any input argument nor it must not assign one of its argument

 JASS:

 //Inlining setting a variable with unused input argument
 //Won't get inlined
 function Inline1 takes integer i returns nothing
 set global = 42 //a global variable
 endfunction

 //Inlining setting a variable with no input argument
 //Will get inlined
 function Inline2 takes nothing returns nothing
 set global = 42 //a global variable
 endfunction

 function F takes nothing returns nothing
 call Inline1(1)
 call Inline2()
 endfunction

JassHelper generated script:

 JASS:

 //Inlining setting a variable with unused input argument
 //Won't get inlined
 function Inline1 takes integer i returns nothing
 set global=42 //a global variable
 endfunction

 //Inlining setting a variable with no input argument
 //Will get inlined
 function Inline2 takes nothing returns nothing
 set global=42 //a global variable
 endfunction

 function F takes nothing returns nothing
 call Inline1(1)
 set global=42 // INLINED!!
 endfunction

Directly calling another function also gets inlined

 JASS:

 //Directly calling a function with no input argument
 //Will get inlined
 function Inline1 takes real r returns nothing
 call SetWidgetLife(globalUnit, r)
 endfunction

 //Directly calling a function with no input argument
 //Will get inlined
 function Inline2 takes nothing returns nothing
 call SetWidgetLife(globalUnit, 42)
 endfunction

 //Directly calling a function with unused input argument
 //Will not get inlined
 function Inline3 takes real r returns nothing
 call SetWidgetLife(globalUnit, 42)
 endfunction

 //Calling an inlining function
 //Will get inlined
 function Inline4 takes nothing returns nothing
 call Inline2()
 endfunction

 function F takes nothing returns nothing
 call Inline1(1)
 call Inline2()
 call Inline3(1)
 call Inline4()
 endfunction

JassHelper generated script:

 JASS:

 //Directly calling a function with no input argument
 //Will get inlined
 function Inline1 takes real r returns nothing
 call SetWidgetLife(globalUnit, r)
 endfunction

 //Directly calling a function with no input argument
 //Will get inlined
 function Inline2 takes nothing returns nothing
 call SetWidgetLife(globalUnit, 42)
 endfunction

 //Directly calling a function with unused input argument
 //Will not get inlined
 function Inline3 takes real r returns nothing
 call SetWidgetLife(globalUnit, 42)
 endfunction

 //Calling an inlining function
 //Will get inlined
 function Inline4 takes nothing returns nothing
 call SetWidgetLife(globalUnit, 42) // INLINED!!
 endfunction

 function F takes nothing returns nothing
 call SetWidgetLife(globalUnit, ((1)*1.0)) // INLINED!!
 call SetWidgetLife(globalUnit, 42) // INLINED!!
 call Inline3(1)
 call SetWidgetLife(globalUnit, 42) // INLINED!!
 endfunction

Functions which contains other function calls must have all its input argument evaluated first before the function call.

 JASS:

 //Function call after input arguments
 function Inline1 takes unit u, real x returns nothing
 call SetUnitPosition(u, x, Sin(42))
 endfunction

 //Function call before input arguments
 function Inline2 takes real x, real y returns nothing
 call SetUnitPosition(GetTriggerUnit(), x, y)
 endfunction

 function F takes nothing returns nothing
 local unit u1
 call Inline1(u1, 1)
 call Inline2(1, 2)
 endfunction

JassHelper generated script:

 JASS:

 //Function call after input arguments
 function Inline1 takes unit u,real x returns nothing
 call SetUnitPosition(u, x, Sin(42))
 endfunction

 //Function call before input arguments
 function Inline2 takes real x,real y returns nothing
 call SetUnitPosition(GetTriggerUnit(), x, y)
 endfunction

 function F takes nothing returns nothing
 local unit u1
 call SetUnitPosition((u1 ), (( 1)*1.0), Sin(42)) // INLINED!!
 call Inline2(1 , 2)
 endfunction

Part 2: Constants Variables

Constant variables are different from normal variables because they can be inlined when the script is optimized by Wc3mapoptimizer 5.0 therefore it does not take any RAM because no variable exists and it is faster than normal variables because no variable lookup takes place. The speed gain may not be noticeable, however the optimized map script will have significantly lesser file size. As the name suggest, it holds a constant value so you cannot change its value (it will throw an error in an attempt to change its value).


 Wc3 Map Optimizer Read Me said:



 Constant inlining: This method inlines constant variables and some of the constant functions in order to save size (most of the times) but the bigger impact is an improvement in performance, although this method will not have any impact unless you use some advanced Jass spells or systems.


Click to expand...

Inlining constant variables has a simple rule:

Constants that needs to be evaluated doesn't get inlined.

 JASS:

library L initializer F

 globals
 constant integer INT1 = 42
 constant integer INT2 = 42 + 73
 constant real REAL1 = 2.718281828
 constant real REAL2 = 1/32
 constant real REAL3 = 1*2*3*4
 constant real REAL4 = Sin(bj_PI/2)
 constant string STRING1 = "A constant string"
 constant string STRING2 = SubString("12345", 0, 3)
 constant boolean BOOL1 = true
 constant boolean BOOL2 = (42 == 72)
 constant boolean BOOL3 = true or false //static if will not work because of 'or' operation
 constant boolean BOOL4 = false and false
 endglobals

 function F takes nothing returns nothing
 local integer i1 = INT1
 local integer i2 = INT2
 local real r1 = REAL1
 local real r2 = REAL2
 local real r3 = REAL3
 local real r4 = REAL4
 local string s1 = STRING1
 local string s2 = STRING2
 local boolean b1 = BOOL1
 local boolean b2 = BOOL2
 local boolean b3 = BOOL3
 //However BOOL2 and BOOL4 still works with static ifs
 static if not BOOL2 then
 call BJDebugMsg("BOOL2 static if works correctly")
 endif
 static if BOOL3 then
 call BJDebugMsg("BOOL3 static if works correctly")
 endif
 static if not BOOL4 then
 call BJDebugMsg("BOOL4 static if works correctly")
 endif
 endfunction

endlibrary

Wc3 Map optimizer generated script:

 JASS:

globals
constant integer e=42+73
constant real x=1/ 32
constant real o=1*2*3*4
constant real i=Sin(bj_PI/ 2)
constant string a=SubString("12345",0,3)
constant boolean n=42==72
constant boolean V=true or false
trigger E=null
real R=.0
real I=.0
boolexpr b=null
endglobals
//...
function F takes nothing returns nothing
local integer i1=42
local integer i2=e
local real r1=2.718281828
local real r2=x
local real r3=o
local real r4=i
local string s1="A constant string"
local string s2=a
local boolean b1=true
local boolean b2=n
local boolean b3=V
call BJDebugMsg("BOOL2 static if works correctly")
call BJDebugMsg("BOOL4 static if works correctly")
endfunction

The Wc3 map optimizer generated script shows that constants containing arithmetic operation, boolean expression, comparison or functions calls doesn't get inlined. Since other variables types (e.g. widgets, players, trigger) cannot be initialized without a function call, they cannot be inlined. The code variable cannot also be used as a constant (constant variables must be initialized with a value) because it will throw an undefined function error since the globals endglobals block is placed at the top most part of the script.

To inline constant variables, the 'Optimized Script' option must be checked (enabled) and the constant inlining script optimization feature must unchecked (enabled).

Script Optimization Tweaks

#### Attachments

wc3mapoptimizer-Settings1.jpg

 89.8 KB

 · Views: 764



wc3mapoptimizer-Settings2.jpg

 52.4 KB

 · Views: 745




 Last edited by a moderator: Apr 23, 2016

#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

Well, if it's "All About Inlining" then I think it's fair to at least mention cJass and it's macrodefinitions feature, which allow the user to inline whatever he wants:

 JASS:

globals
 location loc = Location(0.0, 0.0)
endglobals

// We can't inline this in vJass unfortunately ...
//function GetTerrainZ takes real x, real y returns real
// call MoveLocation(loc, x, y)
// return GetLocationZ(loc)
//endfunction

define GetTerrainZ(x, y, result) = {
 call MoveLocation(loc, x, y)
 set result = GetLocationZ(loc)
}

globals
 real loc_z = 0.0
endglobals

define CalledGetTerrainZ(x, y) = {
 MoveLocation(loc, x, y)
 set loc_z = GetLocationZ(loc)
}

...

 local real z

 GetTerrainZ(0.0, 0.0, z)
 // =>
 // call MoveLocation(loc, x, y)
 // set z = GetLocationZ(loc)

 // The above looks strange because there is no "call", but we can work around that

 call CalledGetTerrainZ(3.14, 6.28)
 // =>
 // call MoveLocation(loc, x, y)
 // set loc_z = GetLocationZ(loc)
 set z = loc_z

 // idealy one would want:
 //
 // set z = GetTerrainZ(x, y)
 // =>
 // call MoveLocation(loc, x, y)
 // set z = GetLocationZ(x, y)
 //
 // which may or may not be possible with cJass, if there's someone more experienced with it, do tell =)
...


But of course this kind of inlining should only be used for very tight loops (code that get's called very very often), because "with great power comes" yada yada...

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Hmmm, cJass, I'm not sure. I don't really use cJass.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

The inliner isn't as good as I thought.

Nice job in making this, approved.

 You must log in or register to reply here.


### Similar threads

Locked

[Solved]
 constant variables doesn't get inlined?


Flux

Mar 24, 2016

World Editor Help Zone

Replies2Views1K

Mar 24, 2016
Flux

Locked

[General]
 Need a map optimizer that actually works


_Guhun_

Apr 2, 2019

World Editor Help Zone

Replies2Views3K

Apr 2, 2019
_Guhun_

Locked

 S2R vs JassParser.S2R (so much for code reuse...)


Aniki

Aug 29, 2016

Advanced Scripting

Replies0Views1K

Aug 29, 2016
Aniki

Locked

 execute_func


Aniki

Feb 5, 2016

Advanced Scripting

Replies13Views4K

Aug 12, 2016
IcemanBo

Locked

 t.real[X] vs t[X].real


Aniki

Oct 4, 2016

Advanced Scripting

Replies1Views1K

Oct 7, 2016
btdonald

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