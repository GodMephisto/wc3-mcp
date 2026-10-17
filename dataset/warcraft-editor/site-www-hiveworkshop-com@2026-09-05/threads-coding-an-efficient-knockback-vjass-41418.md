---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/coding-an-efficient-knockback-vjass.41418/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Coding an Efficient Knockback vJASS
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/coding-an-efficient-knockback-vjass.41418/>
#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

INTRODUCTION + REQUIREMENTS

Knockback is probably one of the most popular things in JASS, because it can be made to be very smooth. Now, the purpose of this tutorial is how to make an efficient knockback - using vJass.

Just a good knowledge of structs is required. You can find more info on the structs in the jasshelper manual. We aren't going to use the game cache at all, and it will still be MUI.

PARAMETERS

First, let's examine our four main parameters:

u = the unit that is being knockbacked

d = the distance the unit is knockbacked to

a = the angle (direction) - in RADIANS

w = the duration of the knockback

RADIANS

Now, why am I using radians instead of degrees? Simple, so I don't have to convert degrees to radians and other way around. I have to convert degrees to radians when I'm using trigonometrical functions and I have to convert radians to degrees when I'm using Atan2() function. Example:

 JASS:

function IHateDegrees takes nothing returns nothing
 local real a = 30 // degrees
 call Sin(a * bj_DEGTORAD)
 // ...
 // ...
 set a = Atan2(y, x) * bj_RADTODEG
endfunction

It's more efficient to work with radians all the time. All natives that work with angles take radians, except these two: SetUnitFacing(), GetUnitFacing(). That could be annoying sometimes, but get used to it.Now, let's actually code something here, for starters, let's make a function that will take our four main parameters:

function Knockback takes unit u, real d, real a, real w returns nothing

TIMER

The basic thing you must have when you're making a knockback code is a timer, because a knockback is moving a unit a bit a time. That timer will periodically fire a function that will move the knockbacked unit a bit. We are going to make a global timer using the global block (vJass feature), you'll see later why:

 JASS:

globals
 timer Tim = CreateTimer()
endglobals

INTERVAL

Now, when having a timer, you must also have to have the interval of timer's periodic executions. This will tell the timer how fast he has to execute.

The best way is to make a constant interval, we have 2 ways of achieving that, we can:

Make a global block, then create a (constant) global real which will hold the interval

Make a constant function which will return the interval

There is actually no difference between those two methods afaik, so I'll choose the second one, because I want the interval to be separated from the other globals for some reason. The question you are probably (or probably not) asking is: what interval is best to put? I, personally, use 0.035, but it seems like 0.04 is the most popular one, so we are going to use that one (you can use any you like, but recommended is somewhere between 0.03 and 0.04 so we can achieve a smooth effect):

 JASS:

constant function Interval takes nothing returns real
 return 0.04
endfunction

NUMBER OF EXECUTIONS

Now when that is clear, we have one last thing we need to have: the number of executions. We have to calculate how many executions the timer has to perform, that is necessary for the timer to know when to stop. We will going to name that variable q and it will be an integer (makes sense). How to calculate q? That's simple, it's the duration divided by the interval. I hope you know math well enough to know why is it like that, if not, then learn. We will convert the result to integer, so we can put it in our integer variable:

 JASS:

function Knockback takes unit u, real d, real a, real w returns nothing
 local integer q = R2I(w / Interval())
endfunction

What now? First, we need to create a function that will be assigned to moving the unit (that function will be executed periodically by the timer). Now we must name that function somehow:

function Knockback_Execute takes nothing returns nothing

STRUCT

The function takes nothing and returns nothing because it will be executed by the timer, and we all know that when executing a function like that, we can not transfer parameters. We have to think of a way to transfer the values from Knockback to Knockback_Execute. Hmm.... I know! Lets use structs, we will create a simple struct that will hold the values we need to transfer:

 JASS:

struct Knockback_Data
 unit u
 real a
endstruct

And let's create a global array of Knockback_Datas (again, later you'll see why):

 JASS:

globals
 timer Tim = CreateTimer()
 Knockback_Data array Ar
endglobals

What about the rest of the parameters? Well, this part gets a little complicated, but don't lose hope! We're going to go through this together.

A knockback is a process in which the unit is pushed back and then it is slowing down until it reaches the speed of 0, in other words, until it stops.

So we have to have create two new variables:

d1 = the "bit" in "moving a unit a bit a time"

d2 = will decrease d1 with each execution

CALCULATIONS

The variable d2 is required because it will decrease the d1 until it reaches 0 (or slightly less than that), that will cause deceleration in unit movement and then the unit will eventually stop. Now we'll gonna do some calculations, which is a little advanced math, but don't worry, I'll try to explain everything. I suggest you take a paper and a pencil/pen, because this calculations look ugly when written like this. I'm going to use jass tags so it's more readable (I hope):

d1 = (d - d2) + (d - 2 * d2) + (d - 3 * d2) + ... + (d - q * d2)

A math rule allows us to write that like this:

d1 = q * d - q * (q + 1) * d1 / 2

We'll just make a substitute for 'q + 1' so the calculation looks a little nicer (yeah, right...), lets name it x:

d1 = q * (d - (x * d2) / 2)

Lets fix it a little bit:

d1 = q * ((2 * d - x * d2) / 2)

When d1 reaches 0:

0 = q * ((2 * d - x * d2) / 2)

Now we'll multiply the equation by 2:

0 = q * (2 * d - x * d2)

0 = 2 * d * q - x * d2 * q

We'll going to return 'q + 1' now:

0 = 2 * d * q - (q + 1) * d2 * q

We'll going to move '- (q + 1) * d2 * q' to the other side:

(q + 1) * d2 * q = 2 * d * q

On the both sides we have q so if we divide the equation with 'q', they will be gone (meaning they will turn to 1 and we all know that 'x * 1' is 'x'):

(q + 1) * d2 = 2 * d

d2 = 2 * d / (q + 1)

Now we have:

 JASS:

d1 = 2 * d / (q + 1)
d2 = d1 / q

So we'll add those two in our struct:

 JASS:

struct Knockback_Data
 unit u
 real a
 real d1
 real d2
endstruct

So, now we have to create that struct as a local in the Knockback function and set its values:

 JASS:

function Knockback takes unit u, real d, real a, real w returns nothing
 local Knockback_Data kd = Knockback_Data.create()
 local integer q = R2I(w / Interval())

 set kd.u = u
 set kd.a = a
 set kd.d1 = 2 * d / (q + 1)
 set kd.d2 = kd.d1 / q
endfunction

I think everything is clear here, we need to transfer the knockbacked unit, the angle, d1 and d2 to the Knockback_Execute function.

The important part in knockbacking is to pause the knockbacked unit, because not pausing it can cause undesirable effects (like not moving in a straight line):

 JASS:

function Knockback takes unit u, real d, real a, real w returns nothing
 local Knockback_Data kd = Knockback_Data.create()
 local integer q = R2I(w / Interval())

 set kd.u = u
 set kd.a = a
 set kd.d1 = 2 * d / (q + 1)
 set kd.d2 = kd.d1 / q

 call PauseUnit(u, true)
endfunction

STOPPING ANY ORDERS

For moving the unit, we'll going to use SetUnitX/Y, because it has (in this case) a better effect than SetUnitPosition. But SetUnitPosition can break channeling spells, moreover, stop the unit's orders (among other things), and it is important for the knockback to be able to do that. We'll going to move the unit to the same point it already is, because we just want to stop its orders:

 JASS:

function Knockback takes unit u, real d, real a, real w returns nothing
 local Knockback_Data kd = Knockback_Data.create()
 local integer q = R2I(w / Interval())

 set kd.u = u
 set kd.a = a
 set kd.d1 = 2 * d / (q + 1)
 set kd.d2 = kd.d1 / q

 call SetUnitPosition(u, GetUnitX(u), GetUnitY(u))
 call PauseUnit(u, true)
endfunction

NUMBER OF KNOCKBACKS

Now, lets get back to that mysterious global timer. The reason I'm using a global timer is because I don't want to use game cache. Instead of creating a local timer every time, attaching the struct to it and execute the movement in the Knockback_Execute function for each unit separately, I'm going to use a single timer that will perform each active knockback. I'm going to pause it when there are no active knockbacks (and unpause it when there are). I'll do this by creating a new integer global that will tell me how many units do I have to move. I'll set it's initial value to 0, because in the start, there will be 0 active knockbacks:

 JASS:

globals
 timer Tim = CreateTimer()
 Knockback_Data array Ar
 integer Total = 0
endglobals

Then I'm going to start the timer in the Knockback function if Total is equal to 0, because that will mean it is paused and it needs to be started again because the function was called (which means there is a knockback to be performed):

 JASS:

function Knockback takes unit u, real d, real a, real w returns nothing
 local Knockback_Data kd = Knockback_Data.create()
 local integer q = R2I(w / Interval())

 set kd.u = u
 set kd.a = a
 set kd.d1 = 2 * d / (q + 1)
 set kd.d2 = kd.d1 / q

 call SetUnitPosition(u, GetUnitX(u), GetUnitY(u))
 call PauseUnit(u, true)

 if Total == 0 then
 call TimerStart(Tim, Interval(), true, function Knockback_Execute)
 endif
endfunction

Now we have to tell the system that a new knockback is active, we'll going to do that by increasing Total by 1. Another thing we also have to do is add the Knockback_Data that was created (kd) in the Knockback_Data array (Ar), but the index of the first one will be 0:

 JASS:

function Knockback takes unit u, real d, real a, real w returns nothing
 local Knockback_Data kd = Knockback_Data.create()
 local integer q = R2I(w / Interval())

 set kd.u = u
 set kd.a = a
 set kd.d1 = 2 * d / (q + 1)
 set kd.d2 = kd.d1 / q

 call SetUnitPosition(u, GetUnitX(u), GetUnitY(u))
 call PauseUnit(u, true)

 if Total == 0 then
 call TimerStart(Tim, Interval(), true, function Knockback_Execute)
 endif

 set Total = Total + 1
 set Ar[Total - 1] = kd
endfunction

"KNOCKBACK_EXECUTE" FUNCTION

We have to move to the Knockback_Execute function now, what we'll do is create a local Knockback_Data variable, so we can handle each knockback (that needs to be processed) more easily:

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
endfunction

We are going to have a loop now, because we need to process each active knockback, so we'll also have to create a local integer for the loop:

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
endfunction

And because we are going to move units, we are going to add two coordinate variables (x and y):

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y
endfunction

Now we will make a loop that will stop when i (the variable, not me) reaches Total, and just in case of it skipping Total, we'll make it stop when i is equal or greater than Total:

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y

 loop
 exitwhen i >= Total
 set i = i + 1
 endloop
endfunction

In the beginning of the loop we'll set kd to Ar (so it's easier to handle):

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y

 loop
 exitwhen i >= Total
 set kd = Ar[i]
 set i = i + 1
 endloop
endfunction

MOVING THE UNIT

Remember what we said about d1? It is the "bit" in "moving a unit a bit a time", we will put its destination coordinates in variables, then we'll move the unit to those coordinates with SetUnitX/Y:

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y

 loop
 exitwhen i >= Total
 set kd = Ar[i]

 set x = GetUnitX(kd.u) + kd.d1 * Cos(kd.a)
 set y = GetUnitY(kd.u) + kd.d1 * Sin(kd.a)

 call SetUnitX(kd.u, x)
 call SetUnitY(kd.u, y)

 set i = i + 1
 endloop
endfunction

I hope we all know how to do a polar projection with coordinates, because that is what I did here.

Now, after that we need to decrease d1 by d2 (that's what I explained in the "calculations" part):

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y

 loop
 exitwhen i >= Total
 set kd = Ar[i]

 set x = GetUnitX(kd.u) + kd.d1 * Cos(kd.a)
 set y = GetUnitY(kd.u) + kd.d1 * Sin(kd.a)

 call SetUnitX(kd.u, x)
 call SetUnitY(kd.u, y)

 set kd.d1 = kd.d1 - kd.d2

 set i = i + 1
 endloop
endfunction

DESTROYING THE STRUCT

When a knockback finishes, we have to destroy its struct, right? The knockback stops when d1 reaches 0, because that means that the unit has stopped moving. We'll check if d1 reached 0 (or less) with an if/then/else:

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y

 loop
 exitwhen i >= Total
 set kd = Ar[i]

 set x = GetUnitX(kd.u) + kd.d1 * Cos(kd.a)
 set y = GetUnitY(kd.u) + kd.d1 * Sin(kd.a)

 call SetUnitX(kd.u, x)
 call SetUnitY(kd.u, y)

 set kd.d1 = kd.d1 - kd.d2

 if kd.d1 <= 0 then
 call kd.destroy()
 endif

 set i = i + 1
 endloop
endfunction

But when we are destroying that struct, it will leave an empty space, and that is definitely not good. We will have to fill it out somehow, I think the easiest way would be to move the last struct to that empty place (we'll going to do that before destroying the struct):

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y

 loop
 exitwhen i >= Total
 set kd = Ar[i]

 set x = GetUnitX(kd.u) + kd.d1 * Cos(kd.a)
 set y = GetUnitY(kd.u) + kd.d1 * Sin(kd.a)

 call SetUnitX(kd.u, x)
 call SetUnitY(kd.u, y)

 set kd.d1 = kd.d1 - kd.d2

 if kd.d1 <= 0 then
 set Ar[i] = Ar[Total - 1]
 set Total = Total - 1
 call kd.destroy()
 endif

 set i = i + 1
 endloop
endfunction

I used Ar[Total - 1] because the first index of the array is 0, not 1 (I hope that's clear). I'm setting Total to Total - 1 because when a struct is destroyed, that means there is a finished knockback that is no longer active, but done.

One thing we're forgetting (or maybe not) is unpausing the unit when the knockback is over:

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y

 loop
 exitwhen i >= Total
 set kd = Ar[i]

 set x = GetUnitX(kd.u) + kd.d1 * Cos(kd.a)
 set y = GetUnitY(kd.u) + kd.d1 * Sin(kd.a)

 call SetUnitX(kd.u, x)
 call SetUnitY(kd.u, y)

 set kd.d1 = kd.d1 - kd.d2

 if kd.d1 <= 0 then
 call PauseUnit(kd.u, false)
 set Ar[i] = Ar[Total - 1]
 set Total = Total - 1
 call kd.destroy()
 endif

 set i = i + 1
 endloop
endfunction

Also, when there are no active knockbacks (meaning Total is equal to 0), we need to pause the timer:

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y

 loop
 exitwhen i >= Total
 set kd = Ar[i]

 set x = GetUnitX(kd.u) + kd.d1 * Cos(kd.a)
 set y = GetUnitY(kd.u) + kd.d1 * Sin(kd.a)

 call SetUnitX(kd.u, x)
 call SetUnitY(kd.u, y)

 set kd.d1 = kd.d1 - kd.d2

 if kd.d1 <= 0 then
 call PauseUnit(kd.u, false)
 set Ar[i] = Ar[Total - 1]
 set Total = Total - 1
 call kd.destroy()
 endif

 set i = i + 1
 endloop

 if Total == 0 then
 call PauseTimer(Tim)
 endif
endfunction

SINE AND COSINE

Pay attention to that Cos() and Sin() in the Knockback_Execute function, we don't have to call them every 0.04 seconds, right? That's really inefficient, we can just store them in a variable. Moreover, we are going to put them in our struct:

 JASS:

struct Knockback_Data
 unit u
 real a
 real d1
 real d2

 real sin
 real cos
endstruct

Now, what we're going to do now is give those variables a value:

 JASS:

function Knockback takes unit u, real d, real a, real w returns nothing
 local Knockback_Data kd = Knockback_Data.create()
 local integer q = R2I(w / Interval())

 set kd.u = u
 set kd.a = a
 set kd.d1 = 2 * d / (q + 1)
 set kd.d2 = kd.d1 / q

 set kd.sin = Sin(a)
 set kd.cos = Cos(a)

 call SetUnitPosition(u, GetUnitX(u), GetUnitY(u))
 call PauseUnit(u, true)

 if Total == 0 then
 call TimerStart(Tim, Interval(), true, function Knockback_Execute)
 endif

 set Total = Total + 1
 set Ar[Total - 1] = kd
endfunction

But note that then we don't need the angle variable in our struct anymore, so we can remove it:

 JASS:

struct Knockback_Data
 unit u
// real a
 real d1
 real d2

 real sin
 real cos
endstruct

function Knockback takes unit u, real d, real a, real w returns nothing
 local Knockback_Data kd = Knockback_Data.create()
 local integer q = R2I(w / Interval())

 set kd.u = u
// set kd.a = a
 set kd.d1 = 2 * d / (q + 1)
 set kd.d2 = kd.d1 / q

 set kd.sin = Sin(a)
 set kd.cos = Cos(a)

 call SetUnitPosition(u, GetUnitX(u), GetUnitY(u))
 call PauseUnit(u, true)

 if Total == 0 then
 call TimerStart(Tim, Interval(), true, function Knockback_Execute)
 endif

 set Total = Total + 1
 set Ar[Total - 1] = kd
endfunction

Also, don't forget to do use them in the Knockback_Execute function:

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y

 loop
 exitwhen i >= Total
 set kd = Ar[i]

 set x = GetUnitX(kd.u) + kd.d1 * kd.cos
 set y = GetUnitY(kd.u) + kd.d1 * kd.sin

 call SetUnitX(kd.u, x)
 call SetUnitY(kd.u, y)

 set kd.d1 = kd.d1 - kd.d2

 if kd.d1 <= 0 then
 call PauseUnit(kd.u, false)
 set Ar[i] = Ar[Total - 1]
 set Total = Total - 1
 call kd.destroy()
 endif

 set i = i + 1
 endloop

 if Total == 0 then
 call PauseTimer(Tim)
 endif
endfunction

MAP BOUNDS CAUTION

I think we all know that when a unit (or anything else) gets outside the map bounds, the game crashes. What we're going to do is prevent our knockback code to push the unit outside those bounds. Instead, we'll make the unit stop if it reaches the bounds. First, we will implement Vexorian's CheckPathability function, which is very useful I must say. Then lets put the function on the top of our script (or in the custom script section, wherever you're coding this).

Now when we have that implemented, here's what we're going to do: in the Knockback_Execute function we're going to check if the point that the unit is supposed to move to is pathable, if it's not, then that means something is already there (tree, building, map bound, cliffs etc.). Here's how we're going to do that:

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y

 loop
 exitwhen i >= Total
 set kd = Ar[i]

 set x = GetUnitX(kd.u) + kd.d1 * kd.cos
 set y = GetUnitY(kd.u) + kd.d1 * kd.sin

 if CheckPathability(x, y) then
 call SetUnitX(kd.u, x)
 call SetUnitY(kd.u, y)
 endif

 set kd.d1 = kd.d1 - kd.d2

 if kd.d1 <= 0 or not CheckPathability(x, y) then
 call PauseUnit(kd.u, false)
 set Ar[i] = Ar[Total - 1]
 set Total = Total - 1
 call kd.destroy()
 endif

 set i = i + 1
 endloop

 if Total == 0 then
 call PauseTimer(Tim)
 endif
endfunction

The problem here is that we're creating an item and removing it every 0.04 seconds, that's inefficient. We can make a single global item and just move it around in each execution:

 JASS:

globals
 timer Tim = CreateTimer()
 Knockback_Data array Ar
 integer Total = 0

 item I = CreateItem('ciri', 0, 0)
endglobals

function CheckPathabilityTrickGet takes nothing returns nothing
 set bj_rescueChangeColorUnit = bj_rescueChangeColorUnit or GetEnumItem() != I
endfunction

function CheckPathabilityTrick takes real x, real y returns boolean
 local integer i = 30
 local real X
 local real Y
 local rect r
 call SetItemPosition(I, x, y)
 set X = GetItemX(I) - x
 set Y = GetItemY(I) - y
 if X * X + Y * Y <= 100 then
 return true
 endif
 set r = Rect(x - i, y - i, x + i, y + i)
 set bj_rescueChangeColorUnit = false
 call EnumItemsInRect(r, null, function CheckPathabilityTrickGet)
 call RemoveRect(r)
 set r = null
 return bj_rescueChangeColorUnit
endfunction

function CheckPathability takes real x, real y returns boolean
 local boolean b = CheckPathabilityTrick(x, y)
 call SetItemVisible(I, false)
 return b
endfunction

I also got rid of those darn Pow() functions, I don't know why Vex used them.

HITTING TREES

You have probably seen that some knockbacked units take down trees if they bump into them. So that's what we're also going to do here, otherwise, the unit would stop if it ran into a tree also (the CheckPathability part). What we'll do is add a new parameter in our Knockback function:

function Knockback takes unit u, real d, real a, real w, real r returns nothing

This parameter represents the range in which the unit must come within the tree in order to take it down. We'll make it so if you put 0 as a range parameter, no trees will be destroyed. We have to put a new variable in our struct (the one that will hold the range):

 JASS:

struct Knockback_Data
 unit u
 real d1
 real d2

 real sin
 real cos

 real r
endstruct

function Knockback takes unit u, real d, real a, real w, real r returns nothing
 local Knockback_Data kd = Knockback_Data.create()
 local integer q = R2I(w / Interval())

 set kd.u = u
 set kd.a = a
 set kd.d1 = 2 * d / (q + 1)
 set kd.d2 = kd.d1 / q

 set kd.sin = Sin(a)
 set kd.cos = Cos(a)

 set kd.r = r

 call SetUnitPosition(u, GetUnitX(u), GetUnitY(u))
 call PauseUnit(u, true)

 if Total == 0 then
 call TimerStart(Tim, Interval(), true, function Knockback_Execute)
 endif

 set Total = Total + 1
 set Ar[Total - 1] = kd
endfunction

This is a function I wrote (actually copied from Hero12341234's Tree Revival tutorial, hehe, sorry Hero....):

 JASS:

function Knockback_TreeFilter takes nothing returns boolean
 local integer d = GetDestructableTypeId(GetFilterDestructable())
 return d == 'ATtr' or d == 'BTtw' or d == 'KTtw' or d == 'YTft' or d == 'JTct' or d == 'YTst' or d == 'YTct' or d == 'YTwt' or d == 'JTwt' or d == 'JTwt' or d == 'FTtw' or d == 'CTtr' or d == 'ITtw' or d == 'NTtw' or d == 'OTtw' or d == 'ZTtw' or d == 'WTst' or d == 'LTlt' or d == 'GTsh' or d == 'Xtlt' or d == 'WTtw' or d == 'Attc' or d == 'BTtc' or d == 'CTtc' or d == 'ITtc' or d == 'NTtc' or d == 'ZTtc'
endfunction

With this function we will check if the destructible (or, in JASS, destructable, I don't know which is right anymore) in range is a tree.

Ok, now we will go to the Knockback_Execute function, because from there we're going to destroy trees. This part is kinda weird because we'll have to create a rect, because the only way we could reach "destructibles in range" is through EnumDestructablesInRect() function. We will create a rect out of range, there's a BJ that does that:

 JASS:

function GetRectFromCircleBJ takes location center, real radius returns rect
 local real centerX = GetLocationX(center)
 local real centerY = GetLocationY(center)
 return Rect(centerX - radius, centerY - radius, centerX + radius, centerY + radius)
endfunction

We will use pure natives, I just showed you that function so it's easier to understand what I'm doing. So basically, in the Knockback_Execute function:

 Code:

centerX = x
centerY = y
radius = kd.r

Now we'll create a local rect variable and we will check if a destructible is in the circle:

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y
 local rect r

 loop
 exitwhen i >= Total
 set kd = Ar[i]

 set x = GetUnitX(kd.u) + kd.d1 * kd.cos
 set y = GetUnitY(kd.u) + kd.d1 * kd.sin

 if kd.r != 0 then
 set r = Rect(x - kd.r, y - kd.r, x + kd.r, y + kd.r)
 endif

 if CheckPathability(x, y) then
 call SetUnitX(kd.u, x)
 call SetUnitY(kd.u, y)
 endif

 set kd.d1 = kd.d1 - kd.d2

 if kd.d1 <= 0 or not CheckPathability(x, y) then
 call PauseUnit(kd.u, false)
 set Ar[i] = Ar[Total - 1]
 set Total = Total - 1
 call kd.destroy()
 endif

 set i = i + 1
 endloop

 if Total == 0 then
 call PauseTimer(Tim)
 endif
endfunction

We'll create a new function that we will use for EnumDestructablesInRect() function, it will kill every picked destructible:

 JASS:

function Knockback_KillTree takes nothing returns nothing
 call KillDestructable(GetEnumDestructable())
endfunction

We will put it above Knocback_Execute function, so we can reach it.

Now we have an action and a condition, we can use EnumDestructablesInRect() now, but first we have to create a boolexpr that will check if the picked destructible is a tree:

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y
 local rect r
 local boolexpr b

 loop
 exitwhen i >= Total
 set kd = Ar[i]

 set x = GetUnitX(kd.u) + kd.d1 * kd.cos
 set y = GetUnitY(kd.u) + kd.d1 * kd.sin

 if kd.r != 0 then
 set r = Rect(x - kd.r, y - kd.r, x + kd.r, y + kd.r)
 set b = Filter(function Knockback_TreeFilter)
 call EnumDestructablesInRect(r, b, function Knockback_KillTree)
 call RemoveRect(r)
 call DestroyBoolExpr(b)
 endif

 if CheckPathability(x, y) then
 call SetUnitX(kd.u, x)
 call SetUnitY(kd.u, y)
 endif

 set kd.d1 = kd.d1 - kd.d2

 if kd.d1 <= 0 or not CheckPathability(x, y) then
 call PauseUnit(kd.u, false)
 set Ar[i] = Ar[Total - 1]
 set Total = Total - 1
 call kd.destroy()
 endif

 set i = i + 1
 endloop

 if Total == 0 then
 call PauseTimer(Tim)
 endif
endfunction

But instead of calling the Filter all the time, since the boolexpr is always the same, we can make a global boolexpr and set it in on map initialization:

 JASS:

globals
 timer Tim = CreateTimer()
 Knockback_Data array Ar
 integer Total = 0

 item I = CreateItem('ciri', 0, 0)

 boolexpr Filter
endglobals

function Trig_Map_Initialization_Actions takes nothing returns nothing
 set Filter = Filter(function Knockback_TreeFilter)
endfunction

Note that this would mean that you need to transfer the Knockback_TreeFilter function somewhere reachable by Trig_Map_Initialization_Actions, now it doesn't have to be reachable by Knockback_Execute function anymore.

SPECIAL EFFECTS

In some knockbacks there are special effects involved during the process. We're going to make two types of making a special effect:

Creating and destroying the special effect in the Knockback_Execute function

Creating a special effect attached to the knockbacked unit, and when the knockback ends, destroy it

First we are going to make a new integer parameter that will represent the type. 0 will be none (no special effects), 1 will be type 1 and 2 will be type 2:

function Knockback takes unit u, real d, real a, real w, real r, integer t returns nothing

Now, of course, we have to add the string parameter for the special effect path, and if the type is 2, a string parameter for the attachment point. We're going to make it so if you input 1 as the type, you can put "" as attachment point. Just to make something clear, in the type 1 we're going to create the special effect at the position of knockbacked unit (and destroy it) periodically:

function Knockback takes unit u, real d, real a, real w, real r, integer t, string s, string p returns nothing

If you input 0 as type, you can put "" for s and p.

We have to create two new variables in our struct (we're going to leave out type variable, you'll see why) and we're going to set their initial values to 'none':

 JASS:

struct Knockback_Data
 unit u
 real d1
 real d2

 real sin
 real cos

 real r

 string s = ""
 effect e = null
endstruct

What the hell is 'effect e'? This is used for the type 2, so it can be reachable when we want to destroy it.

Lets attach those parameters now, with few if/then/else's:

 JASS:

function Knockback takes unit u, real d, real a, real w, real r, integer t, string s, string p returns nothing
 local Knockback_Data kd = Knockback_Data.create()
 local integer q = R2I(w / Interval())

 set kd.u = u
 set kd.a = a
 set kd.d1 = 2 * d / (q + 1)
 set kd.d2 = kd.d1 / q

 set kd.sin = Sin(a)
 set kd.cos = Cos(a)

 set kd.r = r

 if s != "" and s != null then
 if t == 2 then
 if p != "" and p != null then
 set kd.e = AddSpecialEffectTarget(s, u, p)
 else
 set kd.e = AddSpecialEffectTarget(s, u, "chest")
 endif
 elseif t == 1 then
 set kd.s = s
 endif
 endif

 call SetUnitPosition(u, GetUnitX(u), GetUnitY(u))
 call PauseUnit(u, true)

 if Total == 0 then
 call TimerStart(Tim, Interval(), true, function Knockback_Execute)
 endif

 set Total = Total + 1
 set Ar[Total - 1] = kd
endfunction

I made it so if you pick type 2, but use attachment point "", the attachment point will be "chest" (that's like a default value).

Now lets go to Knockback_Execute function:

 JASS:

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y
 local rect r
 local boolexpr b

 loop
 exitwhen i >= Total
 set kd = Ar[i]

 if kd.s != null and kd.s != null then
 set x = GetUnitX(kd.u)
 set y = GetUnitY(kd.u)

 call DestroyEffect(AddSpecialEffect(kd.s, x, y)

 set x = x + kd.d1 * kd.cos
 set y = y + kd.d1 * kd.sin
 else
 set x = GetUnitX(kd.u) + kd.d1 * kd.cos
 set y = GetUnitY(kd.u) + kd.d1 * kd.sin
 endif

 if kd.r != 0 then
 set r = Rect(x - kd.r, y - kd.r, x + kd.r, y + kd.r)
 set b = Filter(function Knockback_TreeFilter)
 call EnumDestructablesInRect(r, b, function Knockback_KillTree)
 call RemoveRect(r)
 call DestroyBoolExpr(b)
 endif

 if CheckPathability(x, y) then
 call SetUnitX(kd.u, x)
 call SetUnitY(kd.u, y)
 endif

 set kd.d1 = kd.d1 - kd.d2

 if kd.d1 <= 0 or not CheckPathability(x, y) then
 if kd.e != null then
 call DestroyEffect(kd.e)
 endif
 call PauseUnit(kd.u, false)
 set Ar[i] = Ar[Total - 1]
 set Total = Total - 1
 call kd.destroy()
 endif

 set i = i + 1
 endloop

 if Total == 0 then
 call PauseTimer(Tim)
 endif
endfunction

I did that weird if/then/else with s to avoid calling GetUnitX/Y twice. You see now why I put effect e in the struct, right? I did it because it was possible to do without attaching type variable too, with a few if/then/elses everything is possible .

For the end I just want to make on more thing (I didn't know where to squeeze it):

 JASS:

function Knockback takes unit u, real d, real a, real w, real r, integer t, string s, string p returns Knockback_Data
 local Knockback_Data kd = Knockback_Data.create()
 local integer q = R2I(w / Interval())

 set kd.u = u
 set kd.a = a
 set kd.d1 = 2 * d / (q + 1)
 set kd.d2 = kd.d1 / q

 set kd.sin = Sin(a)
 set kd.cos = Cos(a)

 set kd.r = r

 if s != "" and s != null then
 if t == 2 then
 if p != "" and p != null then
 set kd.e = AddSpecialEffectTarget(s, u, p)
 else
 set kd.e = AddSpecialEffectTarget(s, u, "chest")
 endif
 elseif t == 1 then
 set kd.s = s
 endif
 endif

 call SetUnitPosition(u, GetUnitX(u), GetUnitY(u))
 call PauseUnit(u, true)

 if Total == 0 then
 call TimerStart(Tim, Interval(), true, function Knockback_Execute)
 endif

 set Total = Total + 1
 set Ar[Total - 1] = kd

 return kd
endfunction

I put that if you want to mess with the knockback during it, for example if you want to:

change the angle by changing kd.sin and kd.cos

change the unit knockbacked, though that can lead to some screwy side-effects

change the special effect by changing kd.s/kd.e

FEW TRICKS & TIPS

We can put this code in the map header by putting it in a library, so it can be reachable by anywhere:

 JASS:

library Knockback

struct Knockback_Data
 unit u
 real d1
 real d2

 real sin
 real cos

 real r

 string s = ""
 effect e = null
endstruct

globals
 timer Tim = CreateTimer()
 Knockback_Data array Ar
 integer Total = 0

 item I = CreateItem('ciri', 0, 0)

 boolexpr Filter
endglobals

function CheckPathabilityTrickGet takes nothing returns nothing
 set bj_rescueChangeColorUnit = bj_rescueChangeColorUnit or GetEnumItem() != I
endfunction

function CheckPathabilityTrick takes real x, real y returns boolean
 local integer i = 30
 local real X
 local real Y
 local rect r
 call SetItemPosition(I, x, y)
 set X = GetItemX(I) - x
 set Y = GetItemY(I) - y
 if X * X + Y * Y <= 100 then
 return true
 endif
 set r = Rect(x - i, y - i, x + i, y + i)
 set bj_rescueChangeColorUnit = false
 call EnumItemsInRect(r, null, function CheckPathabilityTrickGet)
 call RemoveRect(r)
 set r = null
 return bj_rescueChangeColorUnit
endfunction

function CheckPathability takes real x, real y returns boolean
 local boolean b = CheckPathabilityTrick(x, y)
 call SetItemVisible(I, false)
 return b
endfunction

function Knockback_TreeFilter takes nothing returns boolean
 local integer d = GetDestructableTypeId(GetFilterDestructable())
 return d == 'ATtr' or d == 'BTtw' or d == 'KTtw' or d == 'YTft' or d == 'JTct' or d == 'YTst' or d == 'YTct' or d == 'YTwt' or d == 'JTwt' or d == 'JTwt' or d == 'FTtw' or d == 'CTtr' or d == 'ITtw' or d == 'NTtw' or d == 'OTtw' or d == 'ZTtw' or d == 'WTst' or d == 'LTlt' or d == 'GTsh' or d == 'Xtlt' or d == 'WTtw' or d == 'Attc' or d == 'BTtc' or d == 'CTtc' or d == 'ITtc' or d == 'NTtc' or d == 'ZTtc'
endfunction

constant function Interval takes nothing returns real
 return 0.04
endfunction

function Knockback_Execute takes nothing returns nothing
 local Knockback_Data kd
 local integer i = 0
 local real x
 local real y
 local rect r
 local boolexpr b

 loop
 exitwhen i >= Total
 set kd = Ar[i]

 if kd.s != null and kd.s != null then
 set x = GetUnitX(kd.u)
 set y = GetUnitY(kd.u)

 call DestroyEffect(AddSpecialEffect(kd.s, x, y)

 set x = x + kd.d1 * kd.cos
 set y = y + kd.d1 * kd.sin
 else
 set x = GetUnitX(kd.u) + kd.d1 * kd.cos
 set y = GetUnitY(kd.u) + kd.d1 * kd.sin
 endif

 if kd.r != 0 then
 set r = Rect(x - kd.r, y - kd.r, x + kd.r, y + kd.r)
 set b = Filter(function Knockback_TreeFilter)
 call EnumDestructablesInRect(r, b, function Knockback_KillTree)
 call RemoveRect(r)
 call DestroyBoolExpr(b)
 endif

 if CheckPathability(x, y) then
 call SetUnitX(kd.u, x)
 call SetUnitY(kd.u, y)
 endif

 set kd.d1 = kd.d1 - kd.d2

 if kd.d1 <= 0 or not CheckPathability(x, y) then
 if kd.e != null then
 call DestroyEffect(kd.e)
 endif
 call PauseUnit(kd.u, false)
 set Ar[i] = Ar[Total - 1]
 set Total = Total - 1
 call kd.destroy()
 endif

 set i = i + 1
 endloop

 if Total == 0 then
 call PauseTimer(Tim)
 endif
endfunction

function Knockback takes unit u, real d, real a, real w, real r, integer t, string s, string p returns Knockback_Data
 local Knockback_Data kd = Knockback_Data.create()
 local integer q = R2I(w / Interval())

 set kd.u = u
 set kd.a = a
 set kd.d1 = 2 * d / (q + 1)
 set kd.d2 = kd.d1 / q

 set kd.sin = Sin(a)
 set kd.cos = Cos(a)

 set kd.r = r

 if s != "" and s != null then
 if t == 2 then
 if p != "" and p != null then
 set kd.e = AddSpecialEffectTarget(s, u, p)
 else
 set kd.e = AddSpecialEffectTarget(s, u, "chest")
 endif
 elseif t == 1 then
 set kd.s = s
 endif
 endif

 call SetUnitPosition(u, GetUnitX(u), GetUnitY(u))
 call PauseUnit(u, true)

 if Total == 0 then
 call TimerStart(Tim, Interval(), true, function Knockback_Execute)
 endif

 set Total = Total + 1
 set Ar[Total - 1] = kd

 return kd
endfunction

endlibrary

Holy macaroni, that's some code...... . You can put that code in a trigger (not in the map header).

Now when we put it in a library, we can make the functions/variables/structs that we don't want to be reachable private:

 JASS:

globals
 private timer Tim = CreateTimer()
 private Knockback_Data array Ar
 private integer Total = 0

 private item I = CreateItem('ciri', 0, 0)

 boolexpr Filter
endglobals

private function CheckPathabilityTrickGet takes nothing returns nothing

private function CheckPathabilityTrick takes real x, real y returns boolean

private function CheckPathability takes real x, real y returns boolean

private function Knockback_TreeFilter takes nothing returns boolean

private constant function Interval takes nothing returns nothing

private function Knockback_Execute takes nothing returns nothing

We are not making boolexpr Filter private because we need it for the map initialization function and we are not making the Knockback_Data struct private, because the return value of Knockback/Ex function would be impossible to use. I hope you understand why we don't make Knockback function private (because then we couldn't use it at all, well, actually, outside the library)

I noticed (I don't know if you have) that we have a lot of parameters that we don't always use, so we can make that function KnockbackEx, and make a new function called Knockback that will take fewer parameters and set the rest of them to 'none':

 JASS:

function KnockbackEx takes unit u, real d, real a, real w, real r, integer t, string s, string p returns Knockback_Data

function Knockback takes unit u, real d, real a, real w returns Knockback_Data
 return KnockbackEx(u, d, a, w, 0, 0, "", "")
endfunction

Of course, you don't have to leave out range parameter, you can put as many parameters you want to Knockback function.

Remember to put Knockback function below KnockbackEx function, you probably know why, if you don't, you should

LAST WORDS

If you find any spelling mistake somewhere, or any error, please report it!

I hope this tutorial helped someone


 Last edited: Oct 23, 2007

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Unattach your signature and it's good to go.

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved!

Silvenon said:

 If you find any spelling mistake somewhere...


Click to expand...

Document spell checked: eight minor errors corrected.

PurplePoot said:

 Unattach your signature and it's good to go.


Click to expand...

~ Tutorial approved. Thread moved to JASS/AI Scripts Tutorials.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

It still needs the signature taken out =/

[EDIT by Wolve] Done.


 Last edited by a moderator: Oct 13, 2007

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Silvenon said:

 JASS:

globals
 timer Tim = CreateTimer()
 Knockback_Data array Ar
 integer Total = 0

 item I = CreateItem('ciri', 0, 0)
endglobals

function CheckPathabilityTrickGet takes nothing returns nothing
 set bj_rescueChangeColorUnit = bj_rescueChangeColorUnit or GetEnumItem() != I
endfunction

function CheckPathabilityTrick takes real x, real y returns boolean@
 local integer i = 30
 local real X = GetItemX(I) - x
 local real Y = GetItemY(I) - y
 local rect r
 call SetItemPosition(I, x, y)
 if X * X + Y * Y <= 100 then
 return true
 endif
 set r = Rect(x - i, y - i, x + i, y + i)
 set bj_rescueChangeColorUnit = false
 call EnumItemsInRect(r, null, function CheckPathabilityTrickGet)
 call RemoveRect(r)
 set r = null
 return bj_rescueChangeColorUnit
endfunction

function CheckPathability takes real x, real y returns boolean
 local boolean b = CheckPathabilityTrick(x, y)
 call SetItemVisible(I, false)
 return b
endfunction

Click to expand...


Perhaps I'm missing something important, but shouldn't you move the item before you check it's x/y values?

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

Whooops, thanks, I fixed that

Omg, I can't believe you edited "macarone" into "macaroni".....hahaha!  I should remove that anyways, sounds stupid.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Haha, nice find there wyrm

And apparently what wyrm quoted has an @ at the end of the func declaration, but not in the original...? (Nothing of importance, just a weird site bug I guess)

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Actually, the @ is also in the original. It's not in every copy of the CheckPathability function, but it's in the first one I believe.

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

A side-effect from wc3campaigns's @@ and ## (I transfered the tutorial)

Fixed that

EDIT: Btw, Pooty, what miracle do I have to do for you to rep me?  I'm not asking you to do that, I'm just curious how it goes with you


 Last edited: Oct 15, 2007

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

I basically never rep people, I thought you realized that by now?

I might do it sometime, but (IMO) rep is stupid anyways.

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

Yeah, I realized.......But I thought you were playing hard-to-get with reps.

#### Blue_Jeans

#### Blue_Jeans

 Level 6

JoinedJun 30, 2006Messages230

Hmm... I realized this is a bit of a bump, but I'd like to see the tutorial cleaned up a little bit in formatting, if you would. Also, I'd like to see the knockback ENTIRELY automated using methods. You use the struct as sort of a mini-database and don't utilize methods. Some of this stuff in your knockback will never be changed be the user. So why not automate those parts?

Part of an 'efficient' trigger in my opinion is that it is very easily implemented. Basically, you copy a big block of code into either a library or into the trigger for the spell, and then your 'trigger' simply has three functions: an InitTrig, Conditions, and Actions. Then, all you do in Actions is call up a method which then automates the entire process. You would do all the spell customization in Actions as well.

Also, since you are using vJASS, why not use scopes? It would let you use shorter function names. Shorter names are usually easier to read and understand. I prefer looking at "Actions" instead of "Knockback_Actions". Also, if they ever need rename the trigger, with scopes it is a piece of cake.

Also, I've never played a good map where you were close enough to the edge of the map to be knocked out of bounds... I think just mentioning it is good enough, rather than doing all that extra work that should never be required.

These are just suggestions, obviously, but I'd certainly rep you if you did all this stuff. I consider it incomplete how it currently stands.

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

FINALLY someone noticed!

I think you're like the third person who actually read this tutorial. Yeah, you're right, there are a lot of inefficiencies and I'm aware of them. I was planning to fix them, but I noticed very few people read this and I had no reason to update it. But now I do.

I was a really bad vJasser then, if you go to the Knockback function in the JASS functions, you'll see that the version there is optimized doesn't look like this one.

Thanks for the improvement suggestions and for reading the tutorail, I will update the tutorial according to the function I have in JASS functions.

Heck, here's rep for you

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

I skipped it

Yeah, I didn't read it


I don't remember if you did it or not, but for some reason half the tutorials here have sh*t in them like

 JASS:

local unit BLALALBALBALABASL


for variable names, and parameters


I'm really starting to understand JASS, and I still get lost when I see that


It's f*cking confusing


Sorry bout the cursing, just re-reading some tuts, I can't believe I even understood them the first time


//===================================


I skipped your tut cuz I got stuck at the math part, just didn't make any sense to me, I love my algebra, but I'd much more prefer you write it with full words, and the after each equation write what it means and why, it gets REALLY confusing when you do like 6 variables at once without making some strong correlations between them (I also don't know a damned thing about physics, just algebra, but algebra --> physics, so I can learn)


Btw, what is that math rule that you used at the very beginning to substitute a sh*tload of variables???


Um, also, wouldn't you need to divide???

 JASS:

0 = 2 * d * q - (q + 1) * d2 * q

We'll going to move '- (q + 1) * d2 * q' to the other side:

 JASS:

(q + 1) * d2 * q = 2 * d * q

Click to expand...

Should be:









 JASS:

(q + 1)/(d2 * q) = 2 * d * q

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

No, it shouldn't be like that, because everything connected with a multiply simbol (*) is counted as one member, I'm not dividing the equation with (q + 1) * d2 * q, I'm adding that member to the equation (so the one on the right side becomes 0 and dissappears)..........learn a bit more math, what can I say


Concerning the rest of what you said, yeah, I'm working on it, I'm gonna update the whole tutorial and fix everything, explain more etc.


Btw, I'm not mad that people didn't read my tutorial and I don't think you'll have much benefit from it since you need to understand normal JASS better (and then move to vJass).

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

The math thing

I'm still real bothered by it, it would all need to be in parentheses and be multiplied by -1, like this


-1(n * q + 47) could be added to the other side


But if there are no parentheses, wouldn't work


Best way to show it would be to replace them with real numbers


Let's pretend they are these variables when plugged in, they must equal zero when multiplied


d = 0


q = 1 'The q drops to a zero, making the equation equal zero


d2 = 0


0 = 2 * d * q - (q + 1) * d2 * q


0 = 0


(q + 1) * d2 * q = 2 * d * q


Plugging in...


(1 + 1) * 0 * 1 = 2 * 0 * 1


2 * 0 * 1 = 2 * 0 * 1 It works here ><


(q + 1)/(d2 * q) = 2 * d * q


(1 + 1)/(0 * 1) = 2 * 0 * 1 Undefined here ><


Very odd, you would need a real-life example of variables that could fit here...


Otherwise, the equivalence won't come out properly


(I used just one variable being 0, and neither of them worked)


Strange, but those variables must be in parentheses to add the whole thing to the other side, otherwise, it just isn't algebraically sound


(you don't undo multiplication through addition)

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

I don't really understand what you're saying here, but x * y * z is same as (x * y * z), so - x * y * z is same as - (x * y * z), so you don't have to put in parenthesis when transfering to the other side, why would you?

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

The negative 1

Hmmm, good point, although it just doesn't make sense to me


You couldn't do this:

 JASS:

x*y = 10

To

 JASS:

x = 10 - y


Just doesn't make any sense to me


But what about

 JASS:

-1 * x * y = 10

To

 JASS:

0 = 10 + (xy)


That actually works, because technically -1* x*y = (0 - xy)


I dunno, that just really doesn't settle with me

#### Matrix

#### Matrix

 Level 5

JoinedJan 18, 2007Messages59

Resources: 6

#### Resources
Maps 4Spells 1Tutorials 1

Silvenon great job but i want to give an advise.


when u move unit plz use SafeX, SafeY to prevent war3error

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

 Hmmm, good point, although it just doesn't make sense to me


You couldn't do this:

 JASS:

x*y = 10

To

 JASS:

x = 10 - y


Just doesn't make any sense to me


Click to expand...


Of course you can't do that, what's the matter with you??

 But what about

 JASS:

-1 * x * y = 10

To

 JASS:

0 = 10 + (xy)


That actually works, because technically -1* x*y = (0 - xy)


I dunno, that just really doesn't settle with me


Click to expand...


Err yeah, that is correct, that is what I did........? Either try to state your question more clearly, or PM me.

 when u move unit plz use SafeX, SafeY to prevent war3error


Click to expand...


It would be kinda dumb for the system to require CSCache just because of those tiny functions, wouldn't it? Why are you suggesting I put those anyways? What war3error?

#### Matrix

#### Matrix

 Level 5

JoinedJan 18, 2007Messages59

Resources: 6

#### Resources
Maps 4Spells 1Tutorials 1

Silvenon Sry man i mean use it in ur jump function /there arent it/


In this its ok

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

Hey! That's a good idea, I wonder why I didn't do that already (I'll do it as soon as I feel like it). Thanks!

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

Thought about it alot more

Yeah, your completely correct with your math, now that I really thought about it


The part thats still got me, is this

 JASS:

d1 = (d - d2) + (d - 2 * d2) + (d - 3 * d2) + ... + (d - q * d2)

A math rule allows us to write that like this:

 JASS:

d1 = q * d - q * (q + 1) * d1 / 2

We'll just make a substitute for 'q + 1' so the calculation looks a little nicer (yeah, right...), lets name it x:


Click to expand...


I understand the first section

 JASS:

d1 = (d - d2) + (d - 2 * d2) + (d - 3 * d2) + ... + (d - q * d2)


but what is the math rule that can change that into this

 JASS:

d1 = q * d - q * (q + 1) * d1 / 2


???

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

 I understand the first section

 JASS:

d1 = (d - d2) + (d - 2 * d2) + (d - 3 * d2) + ... + (d - q * d2)


but what is the math rule that can change that into this

 JASS:

d1 = q * d - q * (q + 1) * d1 / 2


???


Click to expand...


Well, kinda hard to explain, it is just something known in math, just a formula that exists, there is nothing to understand (I know this explanation sucks, but it really is that way).

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

><

Heh, well I suppose I'm gonna need to figure this one out on my own


I appreciate you creating this tutorial, as I don't think I'd ever get that equation on my own (or it would take 20 hours of thought, trial and error, etc.)

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

Herman said:

 Heh, well I suppose I'm gonna need to figure this one out on my own


I appreciate you creating this tutorial, as I don't think I'd ever get that equation on my own (or it would take 20 hours of thought, trial and error, etc.)


Click to expand...


Same with me, my dad helped me, that's the only way I could come up with that piece of calculation you don't understand. It's a little advanced math.

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

Times like that when you really gotta appreciate the internet (Spark Notes! )

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

It all comes down to arithmetic sequences (that is, a series of numbers where the difference of two adjacent terms is the difference of any other two adjacent terms; for example, 1,2,3,4,5,6,7,8,9,... is an arithmetic sequence since 9-8=8-7=7-6...)

Anyways, we can identify three major variables from an arithmetic sequence: n, a, and d.

 JASS:

a = the first term of the sequence
n = the term #
d = the difference between two terms

Now, since your formula is an arithmetic sequence, these values are easy to find.

 JASS:

a = (d - d2)
d = (d-2*d2) - (d - d2) = -2*d2 + d2 = -d2
n = <varies... this is not constant>

To calculate the sum of such a sequence, you can use the formula

 JASS:

S = n(2a + (n - 1)d)/2

Where n, in this case, is the last term in the sequence. (This is for all terms 1 to n)

Judging as that is what you want to do, identify your variables

 JASS:

a = (d-d2)
d = -d2 //this isn't the same d as the one seen in a (yay conflicting formulae)
n = q
S = d1

Thus,

 JASS:

d1 = q*(2*(d - d2) - (q - 1)*d2)/2
d1 = q*(2*d - 2*d2 - q*d2 + d2)/2
d1 = q*(2*d - d2 - q*d2)/2
d1 = q*(2*d - (q*d2 + d2))/2
d1 = q*(2*d - (q + 1)*d2)/2
d1 = (2*q*d - q*(q + 1)*d2)/2
d1 = q*d - q*(q + 1)*d2/2

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

Ohhhhhh

This made ALOT of sense up until about this point

 JASS:

S = n(2a + (n - 1)d)/2


I kinda almost understand this equation, but I feel like it is condensed


Could you explain to me where the 2's come from?


Like, logically where each piece fits in


I'm thinking the 2's come from having the first term, and then adding the increment * n and the first term, and then adding those two numbers (the first term + the calculated bit) then multiplying them by the term #, a bit lost here


And then dividing by 2???


I'm gonna play around with it a little bit using regular #'s in place of the variables, but I would greatly appreciate you explaining where each piece comes from and what it means

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Herman, it comes from:

(tn is Term n)

Knowing that the sum of the first and last terms in a series = the sum of the second and second last, and so on, multiplying by n/2 (terms/2 to get the sum of all such pairs, and in the case of an odd number of terms, the value of the middle one) gives

 JASS:

S = n(a + tn)/2

as the sum of the series.

Sub in the fact that we know that tn in an arithmetic sequence is

 JASS:

tn = a + (n-1)d

To get

 JASS:

S = n(a + a + (n-1)d)/2
S = n(2a + (n-1)d)/2

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

:O


:O


Ohhh, that clears it up a bunch, cept one last question


Why is it (n - 1) rather than just n???

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Because a factors in n for n=1.

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

WOOOO I GET IT NOW!!


When you multiply the average of the first and nth number, it averages out the terms, and then adds them n number of times


If you did it normally, you will find that you can average out the terms

 JASS:

(100 - 10) + (100 - 20) + (100 - 30)

 JASS:

90 + 80 + 70

 JASS:

240


And then with the formula, where n=3:

 JASS:

3((100 - 10) + (100 - 30))/2

 JASS:

3(90 + 70)/2

 JASS:

3(160)/2

 JASS:

3(80)

 JASS:

80 + 80 + 80

 JASS:

90 + 80 + 70


I still can't articulate my thoughts on this manner, as this kind of math is fairly well above me, but it is that the average of the first and nth term, will give you an average of the terms that is equal to the average of the terms from the middle term outward

 JASS:

(90 + 70)/2 = (80 + 80)/2


So, just multiply them by n, or the total number of terms, and you will get the same number as if you added them each individually


Thank You So Much PurplePoot!!!


+rep (btw, my math teacher didn't even know how to do it)


EDIT: Also, yeah I went to go play with the numbers and realized the a factored out the -1

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

 +rep (btw, my math teacher didn't even know how to do it)


Click to expand...

That tends to happen a bit, but for me it was when I asked them a few years ago how to solve 2^i. Generally, you tend to have to find this stuff out on the net (or in a book) from like-minded people.

#### Matrix

#### Matrix

 Level 5

JoinedJan 18, 2007Messages59

Resources: 6

#### Resources
Maps 4Spells 1Tutorials 1

I found it extremly usefull to cr8 another function like Knockback but it will take x, y and not angle and distance for more customization


And it will be good to cr8 special effect not on the unit but on the ground every time the timer expired //coz some speceffects didnt seen properly.


I may make a remix of ur and put it into a map to show the function to the publicity...

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

 I may make a remix of ur and put it into a map to show the function to the publicity...


Click to expand...

You would have to have my permission to modify my function (I always wanted to say that...).

 And it will be good to cr8 special effect not on the unit but on the ground every time the timer expired //coz some speceffects didnt seen properly.


Click to expand...

Doesn't type parameter already do that if you set it to 1? Read the instructions in the JASS functions forum.

P. S. This tutorial is really outdated, I should update it, but I'm very damn lazy.

#### Matrix

#### Matrix

 Level 5

JoinedJan 18, 2007Messages59

Resources: 6

#### Resources
Maps 4Spells 1Tutorials 1

Just I say its becouse I found sm1 request: https://www.hiveworkshop.com/forums/f341/request-sample-map-efficient-knockback-67148/

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

What a retarded request

#### Shdow89

#### Shdow89

 Level 9

JoinedJun 18, 2007Messages214

Resources: 10

#### Resources
Models 1Icons 2Spells and Systems 7

Incredible... Everyone is speaking about vJASS... Is there any knockback functions WITHOUT using vJASS?

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

Yeah, it could be done, just nobody would want to do it

 You must log in or register to reply here.


### Similar threads

Locked

[JASS]
 Knockback Script Errors


Cov3r70ps

Feb 3, 2008

Triggers & Scripts

Replies2Views2K

Feb 4, 2008
Cov3r70ps

Locked

[JASS]
 Wow i feel dumb for asking this...


YO_MA_MA

Oct 28, 2008

Triggers & Scripts

Replies23Views3K

Nov 6, 2008
YO_MA_MA

Locked

[JASS]
 - [vJASS] The Dash System. First time vJASSing. Help needed.


NiddHogg-kun

Aug 19, 2009

Triggers & Scripts

Replies5Views1K

Aug 19, 2009
NiddHogg-kun

Locked

 Need to modify this JASS a bit


I-am-murloc

Mar 6, 2010

Triggers & Scripts

Replies2Views1K

Mar 6, 2010
I-am-murloc

Locked

[JASS]
 Knockback


Destroyer95

Dec 3, 2009

Triggers & Scripts

Replies3Views2K

Dec 4, 2009
Destroyer95

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top