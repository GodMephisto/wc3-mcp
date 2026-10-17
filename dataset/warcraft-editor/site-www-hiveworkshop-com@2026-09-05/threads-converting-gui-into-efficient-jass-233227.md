---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/converting-gui-into-efficient-jass.233227/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Converting GUI Into Efficient JASS
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/converting-gui-into-efficient-jass.233227/>
#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

 How to easily convert GUI into efficient JASS

 Table of Contents

Step 1 - Creating a GUI trigger

Step 2 - Converting the GUI trigger

Step 3 - How to understand the Syntax

Step 4 - Understanding what happened to the Events

Step 5 - Understanding what happened to the Conditions

Step 6 - Understanding what happened to the Actions

Step 7 - Starting on making the Events efficient

Step 8 - Starting on making the Conditions efficient

Step 9 - Starting on making the Actions efficient

Finished Product

There are things out there to help you with functions. I use TESH, it is in JNGP.

What TESH does is it has a list of all of the function names, their arguments and what they return.

This is incredible valuable when writing JASS. It helps with the syntax of functions.

Step 1 - Creating a GUI trigger

Ok I have taken the liberty to create a GUI trigger as I believe you already know how to make GUI triggers.

Let me note that I have made this trigger not based on efficiency.

I have made this trigger to help show you the most things I can in one trigger to help you in converting later.

I made a pre-placed unit and a pre-placed Region( regions in GUI are called rects in JASS).

I added the pre-placed unit and region to help you understand how they work in JASS.

Step 1
Events
Unit - A unit Dies

Conditions
(Triggering unit) Equal to Footman 0000 <gen>

Actions
Set tempPoint = (Random point in myRegion <gen>)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Life of (Triggering unit)) Greater than or equal to 10.00

Then - Actions
Unit - Order (Triggering unit) to Move To tempPoint

Else - Actions
Unit - Create 1 Footman for Player 1 (Red) at tempPoint facing 270.00 degrees

Custom script: call RemoveLocation( udg_tempPoint)

Step 2 - Converting the GUI trigger

Now lets convert the Trigger.

This is very easy just click on the Edit button in the Trigger Editor.

Next click the Convert to Custom Text button and you're trigger should now look like this.

This May look like a garbled mess right now but don’t worry it's a lot easier than it looks like.

Take note everything that has call before it in here is a function call. Function calls are how you do everything in JASS.

GUI actions are converted into function calls when the map is compiled(saved).

 JASS:

function Trig_Step_1_Conditions takes nothing returns boolean
 if ( not ( GetTriggerUnit() == gg_unit_hfoo_0000 ) ) then
 return false
 endif
 return true
endfunction

function Trig_Step_1_Func002C takes nothing returns boolean
 if ( not ( GetUnitStateSwap(UNIT_STATE_LIFE, GetTriggerUnit()) >= 10 ) ) then
 return false
 endif
 return true
endfunction

function Trig_Step_1_Actions takes nothing returns nothing
 set udg_tempPoint = GetRandomLocInRect(gg_rct_myRegion)
 if ( Trig_Step_1_Func002C() ) then
 call IssuePointOrderLocBJ( GetTriggerUnit(), "move", udg_tempPoint )
 else
 call CreateNUnitsAtLoc( 1, 'hfoo', Player(0), udg_tempPoint, 270.00 ) // notice player(0). in JASS player 1 red is actually Player(0)
 endif
 call RemoveLocation( udg_tempPoint)
endfunction

//===========================================================================
function InitTrig_Step_1 takes nothing returns nothing
 set gg_trg_Step_1 = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( gg_trg_Step_1, EVENT_PLAYER_UNIT_DEATH )
 call TriggerAddCondition( gg_trg_Step_1, Condition( function Trig_Step_1_Conditions ) )
 call TriggerAddAction( gg_trg_Step_1, function Trig_Step_1_Actions )
endfunction

Step 3 - How to understand the Syntax

First let me explain how to declare and read a function.

 JASS:

function Name takes Arguments returns argument

First all functions need to be started with the function keyword. This declares the start of a function.

Were I have "Name" that is were you declare what the function is called.

Then you put the takes keyword this is to declare the start of the arguments that function will take.

Were I have "Arguments" you need to replace that with the arguments.

Arguments are What the function takes.

To simplify this the arguments in

Unit - Order (Triggering unit) to Move To tempPoint
 are triggering unit and tempPoint. This is a function.

For example if you want your function to take an integer you right it like this.

 JASS:

function Name takes integer I returns argument

Note the integer keyword you need to first declare what type of variable you want the function to take.

The I is the integer name. This is important. I will show you examples of why it is important soon.

If you want your function to take multiple arguments you added a comma after the I. Then you need to put the type of variable and then the name of that variable.

Next you need the return keyword. This declares what you are returning.

You return a variable or nothing. Lets take the creation of a group. This returns a group.

Finally you need to declare what to return. This is how you do that.

 JASS:

function Name takes nothing returns real

This returns an integer. To return the real in the function you have to use the return keyword.

This is how you use that.

 JASS:

function Name takes unit u returns real
 return GetUnitState( u, UNIT_STATE_LIFE)
endfunction

If you take a look in the TESH document GetUnitState returns a real.

In this example I am taking a unit argument and returning its life.

Functions are very useful for when you have to repeat actions cause you can store those repeated actions in the function and call the function when you need to. It will then store the data you want and/or return a value.

As a last note. When you create a function you need to end it. To do this you need the endfunction keyword at the end of your function.

Step 4 - Understanding what happened to the Events

 JASS:

function InitTrig_Step_1 takes nothing returns nothing
 set gg_trg_Step_1 = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( gg_trg_Step_1, EVENT_PLAYER_UNIT_DEATH )
 call TriggerAddCondition( gg_trg_Step_1, Condition( function Trig_Step_1_Conditions ) )
 call TriggerAddAction( gg_trg_Step_1, function Trig_Step_1_Actions )
endfunction

Now easy way to look at it is look at the line that says call TriggerRegisterAnyUnitEventBJ( gg_trg_Step_1, EVENT_PLAYER_UNIT_DEATH )

This is the Event. It is changed into this when converted. There are many different type of events so they don’t always look like this.

If you look closely all Events have the words TriggerRegister and Event in them. That’s how you can easily tell which calls are Events.

TriggerRegister is how you register events to triggers.

TriggerAddAction is how you add Action functions to the trigger.

TriggerAddCondition is how you add Condition functions to the trigger.

Step 5 - Understanding what happened to the Conditions

There are 2 types of conditions that I have placed in here.

First one is the Condition in the condition block(the one right under the Events in GUI).

Second is the Condition in the ITE(if then else) block.

These are slightly different in JASS but only because of how you deal with them.

Conditions always have function name takes nothing returns boolean

The boolean is a true or false for the condition.

 JASS:

//this is the first condition. The one in the condition block under the events block.
call TriggerAddCondition( gg_trg_Step_1, Condition( function Trig_Step_1_Conditions ) )
//You may be asking what happened to the condition I made. It looks confusing.
//Look at the inside of the function call. Specifically this ( Condition( function Trig_Step_1_Conditions ))
//This is your condition converted in JASS. It translates it into another function call because it needs to return a boolean.
//So to find the condition look for function Trig_Step_1_Conditions
//Here it is and note the returns boolean at the end of the function line.
function Trig_Step_1_Conditions takes nothing returns boolean
 if ( not ( GetTriggerUnit() == gg_unit_hfoo_0000 ) ) then
 return false
 endif
 return true
endfunction
//I will be explaining how to improve these later. I think its easier to show what is what, then to show how to improve it step by step.

//Here is the second condition. The one in the ITE block.
//First you have to locate the ITE. Here it is.
 if ( Trig_Step_1_Func002C() ) then
 call IssuePointOrderLocBJ( GetTriggerUnit(), "move", udg_tempPoint )
 else
 call CreateNUnitsAtLoc( 1, 'hfoo', Player(0), udg_tempPoint, 270.00 )
 endif
//Notice the if portion. It says
 if ( Trig_Step_1_Func002C() ) then
//Just like before it translates into a function call. The reason it doesn't have call Trig_Step_1_Func002C() is because the if is basically a call.
//So you do not need to call twice.
//now lets locate function Trig_Step_1_Func002C()
//Here it is
function Trig_Step_1_Func002C takes nothing returns boolean
 if ( not ( GetUnitStateSwap(UNIT_STATE_LIFE, GetTriggerUnit()) >= 10 ) ) then
 return false
 endif
 return true
endfunction
//Now you should know how to tell the conditions from the functions.

Step 6 - Understanding what happened to the Actions

As I have said above function calls are actions.

 JASS:

 if ( Trig_Step_1_Func002C() ) then
 call IssuePointOrderLocBJ( GetTriggerUnit(), "move", udg_tempPoint )
 else
 call CreateNUnitsAtLoc( 1, 'hfoo', Player(0), udg_tempPoint, 270.00 )
 endif
//Notice the IssuePointOrderLocBJ and CreateNUnitsAtLoc.
//These are the actions

Step 7 - Starting on making the Events efficient

I’m going to explain how to fix this by starting with the event function and making it efficient.

Lets take a look at the events function again.

 JASS:

function InitTrig_Step_1 takes nothing returns nothing
 set gg_trg_Step_1 = CreateTrigger( ) // The gg_trg_Step_1 is the trigger.
 call TriggerRegisterAnyUnitEventBJ( gg_trg_Step_1, EVENT_PLAYER_UNIT_DEATH ) // This is the event as said before.
 call TriggerAddCondition( gg_trg_Step_1, Condition( function Trig_Step_1_Conditions)) // this is how the conditions are added to the trigger.
 call TriggerAddAction( gg_trg_Step_1, function Trig_Step_1_Actions) // this is how the actions are added to the trigger.
endfunction

First take a look at the notes I placed in the trigger function.

I showed you what each line means.

Next ill show you how to make these efficient.

Ill start off with the first line. set gg_trg_Step_1 = CreateTrigger()

To make the trigger more efficient we will use a local variable. A local variable is a variable that is only used in that function.

It's better to keep local variable names shorter. You can use one letter for these most of the time.

Also whenever you change the trigger with a local or global variable you need to change it in the other function calls it is used in.

In this case it is used in the call TriggerRegisterAnyUnitEventBJ, call TriggerAddCondition, call TriggerAddAction

The reason I know it's used in these is the arguments it takes.

You can find out what arguments it takes if you hit CTRL and click on TriggerRegisterAnyUnitEventBJ.

This will show you this.

 JASS:

function TriggerRegisterAnyUnitEventBJ takes trigger trig, playerunitevent whichEvent returns nothing
 local integer index

 set index = 0
 loop
 call TriggerRegisterPlayerUnitEvent(trig, Player(index), whichEvent, null)

 set index = index + 1
 exitwhen index == bj_MAX_PLAYER_SLOTS
 endloop
endfunction

Note the takes trigger. This is an argument. So you need to plug in a trigger there. The current trigger is gg_trg_Step_1.

So when we change the current trigger we need to change the trigger arguments.

Here is the new function with the trigger changed to a local variable.

 JASS:

function InitTrig_Step_1 takes nothing returns nothing
 local trigger t = CreateTrigger( ) // for a local variable you have to type local .type of variable. and the name
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_DEATH ) // note the trigger variable is changed
 call TriggerAddCondition( t, Condition( function Trig_Step_1_Conditions)) // note the trigger variable is changed
 call TriggerAddAction( t, function Trig_Step_1_Actions) // note the trigger variable is changed
 set t = null // note this. this is like the custom scripts you use to null variables in GUI. You have to null them here also.
endfunction

Next step - lets check out the event. TriggerRegisterAnyUnitEventBJ

This is one of the few BJs that is ok to use. The others you will want to look up the native function and use that native function instead.

I will show you more on native functions in the next couple of steps.

 JASS:

function Trig_Step_1_Conditions takes nothing returns boolean
 if ( not ( GetTriggerUnit() == gg_unit_hfoo_0000 ) ) then
 return false
 endif
 return true
endfunction

function Trig_Step_1_Func002C takes nothing returns boolean
 if ( not ( GetUnitStateSwap(UNIT_STATE_LIFE, GetTriggerUnit()) >= 10 ) ) then
 return false
 endif
 return true
endfunction

function Trig_Step_1_Actions takes nothing returns nothing
 set udg_tempPoint = GetRandomLocInRect(gg_rct_myRegion)
 if ( Trig_Step_1_Func002C() ) then
 call IssuePointOrderLocBJ( GetTriggerUnit(), "move", udg_tempPoint )
 else
 call CreateNUnitsAtLoc( 1, 'hfoo', Player(0), udg_tempPoint, 270.00 )
 endif
 call RemoveLocation( udg_tempPoint)
endfunction

//===========================================================================
function InitTrig_Step_1 takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_DEATH )
 call TriggerAddCondition( t, Condition( function Trig_Step_1_Conditions))
 call TriggerAddAction( t, function Trig_Step_1_Actions)
 set t = null
endfunction

Step 8 - Starting on making the Conditions efficient

Now this one I will show you how to make Conditions efficient since GUI does a very noticeably bad job at this.

First take a look at the condition function for the trigger.

 JASS:

function Trig_Step_1_Conditions takes nothing returns boolean
 if ( not ( GetTriggerUnit() == gg_unit_hfoo_0000 ) ) then // note the gg_unit_hfoo_0000. This is a pre-placed unit.
 return false
 endif
 return true
endfunction

Notice the return false and return true. These are the booleans that get returned.

Also they use an ITE in there. The ITE is why this is inefficient. This can be simplified to return only what you want it to.

What I mean by that is we want to check if the triggering unit is the pre-placed unit.

To do this we can simplify it to return the trigger unit

Here is how we do this.

 JASS:

function Trig_Step_1_Conditions takes nothing returns boolean
 return GetTriggerUnit() == gg_unit_hfoo_0000 // Now you can see we return the unit.
endfunction

If you want to check if the triggering unit is not the pre-placed unit you do it like this.

 JASS:

function Trig_Step_1_Conditions takes nothing returns boolean
 return not GetTriggerUnit() == gg_unit_hfoo_0000 // notice the not in there. This checks to see if the triggering unit is not the pre-placed unit.
endfunction

Here is how this way works.

 This GetTriggerUnit() == gg_unit_hfoo_0000 returns a true or false.

So if the triggering unit is the pre-placed unit it returns true. If the triggering unit isn't the pre-placed unit it returns false.

Next lets take care of the condition in the ITE. This will be done slightly different. We do not need the function call for the ITE.

 JASS:

function Trig_Step_1_Func002C takes nothing returns boolean
 if ( not ( GetUnitStateSwap(UNIT_STATE_LIFE, GetTriggerUnit()) >= 10 ) ) then
 return false
 endif
 return true
endfunction

function Trig_Step_1_Actions takes nothing returns nothing
 set udg_tempPoint = GetRandomLocInRect(gg_rct_myRegion)
 if ( Trig_Step_1_Func002C() ) then
 call IssuePointOrderLocBJ( GetTriggerUnit(), "move", udg_tempPoint )
 else
 call CreateNUnitsAtLoc( 1, 'hfoo', Player(0), udg_tempPoint, 270.00 )
 endif
 call RemoveLocation( udg_tempPoint)
endfunction

Here is how we should change this. since the ITE can check the units life we will put that in there.

Notice the if has the condition you wanted to check right in it.

The function is now not needed.

 JASS:

function Trig_Step_1_Actions takes nothing returns nothing
 set udg_tempPoint = GetRandomLocInRect(gg_rct_myRegion)
 if GetUnitStateSwap(UNIT_STATE_LIFE, GetTriggerUnit()) >= 10 ) then
 call IssuePointOrderLocBJ( GetTriggerUnit(), "move", udg_tempPoint )
 else
 call CreateNUnitsAtLoc( 1, 'hfoo', Player(0), udg_tempPoint, 270.00 )
 endif
 call RemoveLocation( udg_tempPoint)
endfunction

This is still not as efficient as it can be. So lets change that.

First hold CTRL and click on GetUnitStateSwap you should see this.

 JASS:

function GetUnitStateSwap takes unitstate whichState, unit whichUnit returns real
 return GetUnitState(whichUnit, whichState) // note the names of the arguments and match them w the names of the variables for the arguments that the function call takes.
endfunction

It returns the native GetUnitState. So lets change this.

 JASS:

function Trig_Step_1_Actions takes nothing returns nothing
 set udg_tempPoint = GetRandomLocInRect(gg_rct_myRegion)
 if GetUnitState( GetTriggerUnit(), UNIT_STATE_LIFE) >= 10 ) then // now it checks if the units life is greater than or equal to 10 using the native function call
 call IssuePointOrderLocBJ( GetTriggerUnit(), "move", udg_tempPoint )
 else
 call CreateNUnitsAtLoc( 1, 'hfoo', Player(0), udg_tempPoint, 270.00 )
 endif
 call RemoveLocation( udg_tempPoint)
endfunction

Now the conditions are improved for efficiency.

 JASS:

function Trig_Step_1_Conditions takes nothing returns boolean
 return GetTriggerUnit() == gg_unit_hfoo_0000
endfunction

function Trig_Step_1_Actions takes nothing returns nothing
 set udg_tempPoint = GetRandomLocInRect(gg_rct_myRegion)
 if GetUnitState( GetTriggerUnit(), UNIT_STATE_LIFE) >= 10 ) then
 call IssuePointOrderLocBJ( GetTriggerUnit(), "move", udg_tempPoint )
 else
 call CreateNUnitsAtLoc( 1, 'hfoo', Player(0), udg_tempPoint, 270.00 )
 endif
 call RemoveLocation( udg_tempPoint)
endfunction

//===========================================================================
function InitTrig_Step_1 takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_DEATH )
 call TriggerAddCondition( t, Condition( function Trig_Step_1_Conditions))
 call TriggerAddAction( t, function Trig_Step_1_Actions)
 set t = null
endfunction

Step 9 - Starting on making the Actions efficient

Now we can work on the actions.

 JASS:

function Trig_Step_1_Actions takes nothing returns nothing
 set udg_tempPoint = GetRandomLocInRect(gg_rct_myRegion)
 if GetUnitState( GetTriggerUnit(), UNIT_STATE_LIFE) >= 10 ) then
 call IssuePointOrderLocBJ( GetTriggerUnit(), "move", udg_tempPoint )
 else
 call CreateNUnitsAtLoc( 1, 'hfoo', Player(0), udg_tempPoint, 270.00 )
 endif
 call RemoveLocation( udg_tempPoint)
endfunction

First lets start by changing the udg_tempPoint variable into local real variables and a global region variable.( regions in GUI are rects in JASS)

Not you should make a global region variable. I called mine region. in JASS this is written udg_region.

Now in a map init trigger set your variable to the pre-placed rect.

The reason to do this is efficiency. It is faster to get a variable than to get a pre-placed region.

You may ask when should I put something into a variable. Whenever you use it twice or more is what i go by.

Set region = myRegion <gen>

Then you create local variables a local real x and a local real y and then you get the random reals.

To find out how to assign the random reals.

Hold CTRL and click on the GetRandomLocInRect and this will show you what to do.

 JASS:

local real x = GetRandomReal(GetRectMinX( udg_region), GetRectMaxX( udg_region))
local real y = GetRandomReal(GetRectMinY( udg_region), GetRectMaxY( udg_region))

Second lets change the IssuePointOrderLocBJ

If you hold CTRL and click on this one you still have the location.

But if you look in the functions list you can find this action.

 JASS:

native IssuePointOrder takes unit whichUnit, string order, real x, real y returns boolean

This is a safer point order to use since it uses reals which don't leak.

To use this you put in the unit, the order string, and then the local reals x and y

 JASS:

call IssuePointOrder( GetTriggerUnit(), "move", x, y)

Finally lets do the CreateNUnitsAtLoc

This one also has a safer function call. Here it is.

 JASS:

native CreateUnit takes player id, integer unitid, real x, real y, real face returns unit

To use this you put in the player id, unitID, and then the local reals x and y, and the face.

First like i said before player 1 red in GUI is Player(0) in JASS.

Second the unit Id can be found by going to the object editor and click View -> Display Values as Raw Data.

Then look for the units data you want. ex 'hpea' is a peasant. 'hfoo' is a footman

The Raw values are integers. More specifically they are ASCII integers.

The face value is the direction they face in.

 JASS:

call CreateUnit( Player(0), 'hfoo', x, y, 270)

Also lets add in a local unit u = GetTriggerUnit()

As per the rule I stated before whenever something is called two or more times put it in a variable.

We also need to null this at the end of the function.

Note: when you have a return make sure to null all values before the return.

The reason for this is return is the same as skip remaining actions in GUI.

It will end the function so everything after that doesn’t happen.

If you don't null the variables before it, you will leak.

Finished Product

Finally you can get rid of the useless point variables.

Now for the last thing you should never use TriggerAddAction(). Here is why.

Almia said:

 Ask them to merge the conditions and actions and use conditions rather than action for triggers.

Actions creates new thread, conditions don't.


Click to expand...

So we need to get rid of the trigger add actions and transfer everything to use trigger add condition.

First we will change this line.

 JASS:

function InitTrig_Step_1 takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_DEATH )
 call TriggerAddCondition( t, Condition( function Trig_Step_1_Conditions))
 call TriggerAddAction( t, function Trig_Step_1_Actions)
 set t = null
endfunction
//change it to this. I only got rid of the TriggerAddAction()
function InitTrig_Step_1 takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_DEATH )
 call TriggerAddCondition( t, Condition( function Trig_Step_1_Actions)) // also changed this
 set t = null
endfunction
//next change this
function Trig_Step_1_Conditions takes nothing returns boolean
 return GetTriggerUnit() == gg_unit_hfoo_0000
endfunction

function Trig_Step_1_Actions takes nothing returns boolean
 local real x = GetRandomReal(GetRectMinX( udg_region), GetRectMaxX( udg_region))
 local real y = GetRandomReal(GetRectMinY( udg_region), GetRectMaxY( udg_region))
 if GetUnitState( GetTriggerUnit(), UNIT_STATE_LIFE) >= 10 ) then
 call IssuePointOrder( GetTriggerUnit(), "move", x, y)
 else
 call CreateUnit( Player(0), 'hfoo', x, y, 270)
 endif
endfunction
//we need to change this into an ITE and get rid of the old condition function.
//Also move change the local variables and set them in the ITE.
//The reason for this is to only set the local variables when the condition is true.
//This keeps it efficient.
//Change it to this.
function Trig_Step_1_Actions takes nothing returns boolean
 local real x
 local real y
 if GetTriggerUnit() == gg_unit_hfoo_0000 then
 set x = GetRandomReal(GetRectMinX( udg_region), GetRectMaxX( udg_region))
 set y = GetRandomReal(GetRectMinY( udg_region), GetRectMaxY( udg_region))
 if GetUnitState( GetTriggerUnit(), UNIT_STATE_LIFE) >= 10 ) then
 call IssuePointOrder( GetTriggerUnit(), "move", x, y)
 else
 call CreateUnit( Player(0), 'hfoo', x, y, 270)
 endif
 endif
 return false
endfunction

 JASS:

function InitTrig_Step_1 takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_DEATH )
 call TriggerAddCondition( t, Condition( function Trig_Step_1_Conditions))
 call TriggerAddAction( t, function Trig_Step_1_Actions)
 set t = null
endfunction
//change it to this. I only got rid of the TriggerAddAction()
function InitTrig_Step_1 takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_DEATH )
 call TriggerAddCondition( t, Condition( function Trig_Step_1_Conditions))
 set t = null
endfunction
//next change this
function Trig_Step_1_Conditions takes nothing returns boolean
 return GetTriggerUnit() == gg_unit_hfoo_0000
endfunction
//we need to change this back to an ITE.
//The reason I changed it before was to show you how you can return values if you need to.
//Change it to this.
function Trig_Step_1_Conditions takes nothing returns boolean
 if GetTriggerUnit() == gg_unit_hfoo_0000 then
 call Trig_Step_1_Actions() // this is how you call a function you have made.
 endif
 return false
endfunction

 JASS:

function Trig_Step_1_Actions takes nothing returns boolean
 local real x
 local real y
 local unit u = GetTriggerUnit()
 if u == gg_unit_hfoo_0000 then
 set x = GetRandomReal(GetRectMinX( udg_region), GetRectMaxX( udg_region))
 set y = GetRandomReal(GetRectMinY( udg_region), GetRectMaxY( udg_region))
 if GetUnitState( u, UNIT_STATE_LIFE) >= 10 then
 call IssuePointOrder( u, "move", x, y)
 else
 call CreateUnit( Player(0), 'hfoo', x, y, 270)
 endif
 endif
 set u = null
 return false
endfunction

//===========================================================================
function InitTrig_Step_1 takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_DEATH )
 call TriggerAddCondition( t, Condition( function Trig_Step_1_Actions))
 set t = null
endfunction

Here is another way you could structure this. You can decide which one you like more.

In this one you have to switch the condition function under the actions function.

Two reasons I do this.

1) You technically have to. JASS reads things from bottom up.

In other words if you put conditions function on top it cannot call the actions function.

2) If you always keep your condition functions at the bottom its always easy to find them.

 JASS:

function Trig_Step_1_Actions takes nothing returns nothing
 local real x = GetRandomReal(GetRectMinX( udg_region), GetRectMaxX( udg_region))
 local real y = GetRandomReal(GetRectMinY( udg_region), GetRectMaxY( udg_region))
 local unit u = GetTriggerUnit()
 if GetUnitState( u, UNIT_STATE_LIFE) >= 10 then
 call IssuePointOrder( u, "move", x, y)
 else
 call CreateUnit( Player(0), 'hfoo', x, y, 270)
 endif
 set u = null
endfunction

function Trig_Step_1_Conditions takes nothing returns boolean
 if GetTriggerUnit() == gg_unit_hfoo_0000 then
 call Trig_Step_1_Actions() // this is how you call a function you have made.
 endif
 return false
endfunction

//===========================================================================
function InitTrig_Step_1 takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_DEATH )
 call TriggerAddCondition( t, Condition( function Trig_Step_1_Conditions))
 set t = null
endfunction

For better understanding on how functions are read / used / implemented you should look at this tutorial. https://www.hiveworkshop.com/forums/jass-ai-scripts-tutorials-280/jass-what-func-238298/

If you have any more questions please feel free to ask on here or pm me.


 Last edited: Oct 1, 2013

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Use trigger and jass tags at the start :\.

Also, people converting a GUI trigger to JASS won't know wth a function is, and will def not understand TriggerRegister and TriggerAdd stuff.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Nestharus said:

 Use trigger and jass tags at the start :\.

Also, people converting a GUI trigger to JASS won't know wth a function is, and will def not understand TriggerRegister and TriggerAdd stuff.


Click to expand...

oops lol i must've missed the tags. i explain the triggeradd and triggerregister later i believe but ill elaborate more on those points.

updated: fixed the missing TAGS.

updated: added a short explanation for trigger register, trigger add action, and trigger add condition

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Trigger for the first one, not jass

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Nestharus said:

 Trigger for the first one, not jass


Click to expand...


wow. sry my mistake im worn out today lol will change it now


updated: u gotta love stupid mistakes lol. not used to trigger tags haha

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Fix your bbcodes.

Also,Ask them to merge the conditions and actions and use conditions rather than action for triggers.Actions creates new thread,conditions don't.So your tutorial is not fully completed.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Almia said:

 Fix your bbcodes.

Also,Ask them to merge the conditions and actions and use conditions rather than action for triggers.Actions creates new thread,conditions don't.So your tutorial is not fully completed.


Click to expand...

like i said above im losing my mind today. so many stupid mistakes. kinda why im making tutorials lol.

i will fix this asap.

updated for the incomplete bbcodes lol finally got them right. now ill do the condition thing like u said thx.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Btw,warn them that because they are in JASS,they should not use location and instead use reals for coordinates.

edit

So blue.........................my eyes............

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Almia said:

 Btw,warn them that because they are in JASS,they should not use location and instead use reals for coordinates.

edit

So blue.........................my eyes............


Click to expand...

already told them about that. what color should i use this blue is a little hard to read. idk what the best colors to use are.

changed it to orange lol

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Teal. This fits every description that has green in them(view Meg's tut) but im ok with orange.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

i kinda like the orange i think ill stick with that for now.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Another thing.

Please cache those functions repeated,to increase speed.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

i see what ya mean will update it now.

updated

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

This tutorial is practically useless, as it requires the reader to understand the JASS syntax in order to follow it.


However, people that do understand the JASS syntax won't need it.


The thing is; this tutorial jumps right into the action. Like in your other tutorial, you explained almost nothing.


You should start with a general idea how the JASS syntax works, how JASS functions work, how to pass arguments to functions, how to read returns, etc..


In other words: the infamous "Hello world" function. There is a reason any good programming tutorial starts with "hello world".


You should also explain what BJs are, natives, you should go into more detail with keywords (scope, privates and publics, globals declaration, etc.).


You should also mention helpful tools to write JASS and how they work, like JASShelper and TESH and the function library.


That being said, converting GUI to efficient JASS is a mistake in its own. Efficient JASS is JASS that has been written from scratch. The only reason why someone should convert GUI to JASS is because they are too lazy to use the function browser and want to add some quick and dirty locals. And I think we shouldn't support this lazyness with tutorials.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

This is how I learned. I converted things from GUI since I made a full map in GUI. Then I went and learned how to make it efficient.

#### Orthane

#### Orthane

 Level 7

JoinedJan 22, 2013Messages293

deathismyfriend said:

 This is how I learned. I converted things from GUI since I made a full map in GUI. Then I went and learned how to make it efficient.


Click to expand...


That's how I learned GUI, I tested every single thing, no tuts until a year ago lol. I finally joined the forums recently.


However Death, you have just helped me convert to jass lol.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Glad to help I still have a little to improve

#### Xerxes

#### Xerxes

 Level 20

JoinedApr 14, 2012Messages2,906

Resources: 8

#### Resources
Icons 4Maps 1Spells and Systems 3

Oh hey deathismyfriend; I never saw this other tutorial of yours =D


Nice job on making it. Might really help a lot of people!

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

MasterTrainer said:

 Oh hey deathismyfriend; I never saw this other tutorial of yours =D


Nice job on making it. Might really help a lot of people!


Click to expand...


still needs some work i have to ad near the top on how functions are made and how to read them.

#### Xerxes

#### Xerxes

 Level 20

JoinedApr 14, 2012Messages2,906

Resources: 8

#### Resources
Icons 4Maps 1Spells and Systems 3

^ Actually in it's current state it was really helpful for me... but I agree it does need some work.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

MasterTrainer said:

 ^ Actually in it's current state it was really helpful for me... but I agree it does need some work.


Click to expand...


ya haha i was pretty drunk when i wrote and submitted this one lol. never really checked it out lol. will hopefully get to it soon

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

updated this finally lol

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

This tutorial makes sense to an extent, but I think it is somewhat overdone. I don't think GUI-to-JASS conversion has been covered explicitly, but it is almost always a feature of the JASS intro tutorials:

https://www.hiveworkshop.com/forums/jass-ai-scripts-tutorials-280/jass-tutorial-16456/

https://www.hiveworkshop.com/forums/jass-ai-scripts-tutorials-280/jass-moving-gui-jass-start-40617/

https://www.hiveworkshop.com/forums/jass-ai-scripts-tutorials-280/jass-concise-introduction-62279/

https://www.hiveworkshop.com/forums...als-280/beginning-jass-tutorial-series-30765/


^They all cover events, conditions, and actions (from GUI-converted code) to a decent extent. I think this tutorial could still be useful, but let me cite one of the guidelines for the tutorial forum:


 Purgeandfire111 said:



 2. A tutorial may be graveyarded if there is a superior tutorial that covers the same concept.


There are countless tutorials that are repeats of others. While it is not the worst thing in the world, usually if there is a better one you'll want users to read that one instead, no? It is essentially directing someone to a helpful consultant over a less-helpful consultant. I may use a lack of originality as a critique when reviewing tutorials, but remember that it does not mean your tutorial is useless. You can still make updates to improve the tutorial, and I may replace the other version with yours or I may allow them to coexist. However, if there is already a really good tutorial explaining a topic, then try to find a different topic to explain or make sure that you are using a different or better technique.


Click to expand...


You can still argue against this though and let me know your thoughts. I don't enjoy graveyarding tutorials, but I also don't want to be incredibly lenient. At the moment, this tutorial has a fundamental flaw that it requires a bit of prior knowledge/understanding. Some may get it, but many might not:


 Zwiebelchen said:



 You should start with a general idea how the JASS syntax works, how JASS functions work, how to pass arguments to functions, how to read returns, etc..


In other words: the infamous "Hello world" function. There is a reason any good programming tutorial starts with "hello world".


You should also explain what BJs are, natives, you should go into more detail with keywords (scope, privates and publics, globals declaration, etc.).


Click to expand...

As Zwieb said, it could use more of a comprehensive intro. However, I don't really advise doing that since that would basically make it a JASS intro tutorial (and we already have tons of those [as listed above], including a video version by Nestharus).


Let me know your thoughts, and I'll take them into consideration before I go on to the next step of reviewing.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

PurgeandFire111 said:

 This tutorial makes sense to an extent, but I think it is somewhat overdone. I don't think GUI-to-JASS conversion has been covered explicitly, but it is almost always a feature of the JASS intro tutorials:

https://www.hiveworkshop.com/forums/jass-ai-scripts-tutorials-280/jass-tutorial-16456/

https://www.hiveworkshop.com/forums/jass-ai-scripts-tutorials-280/jass-moving-gui-jass-start-40617/

https://www.hiveworkshop.com/forums/jass-ai-scripts-tutorials-280/jass-concise-introduction-62279/

https://www.hiveworkshop.com/forums...als-280/beginning-jass-tutorial-series-30765/

^They all cover events, conditions, and actions (from GUI-converted code) to a decent extent. I think this tutorial could still be useful, but let me cite one of the guidelines for the tutorial forum:

You can still argue against this though and let me know your thoughts. I don't enjoy graveyarding tutorials, but I also don't want to be incredibly lenient. At the moment, this tutorial has a fundamental flaw that it requires a bit of prior knowledge/understanding. Some may get it, but many might not:

As Zwieb said, it could use more of a comprehensive intro. However, I don't really advise doing that since that would basically make it a JASS intro tutorial (and we already have tons of those [as listed above], including a video version by Nestharus).

Let me know your thoughts, and I'll take them into consideration before I go on to the next step of reviewing.


Click to expand...

i understand ur ideas on graveyarding it. I would rather not make it graveyarded. I learned jass through converting GUI to JASS and i thought this reflected good on how to do that piece by piece.

I also explained the syntax of JASS to were i thought it was very easy to understand.

If i need to explain more or something to get this approved plz tell me what i should work on exactly. What is lacking and what isnt.

#### Orthane

#### Orthane

 Level 7

JoinedJan 22, 2013Messages293

an understanding structure for GUI users to see and relate the structure towards

Trig Step 1 Actions
Actions
-------- Jass Conditions --------

Trig Step 1 Conditions
Conditions
-------- Jass Actions --------

Trig Step 1
Events
Trigger - Run Trig Step 1 Conditions <gen>

Trigger - Run Trig Step 1 Actions <gen>

-------- The Difference is they wont be seperate triggers in jass--------

That is the structure of this:

Example
Events

Conditions

Actions

You will get to modify the basic's with jass.

Jass takes these locations and seperates them when you convert into Jass:

(Would be part of Step 1 in your tutorial)

Step 1
Events
-------- This This becomes one with The events and the triggers name --------

Conditions
-------- This ends up alone but is added to the trigger in the events location --------

Actions
-------- All but one part of this is together --------

-------- The Conditions for the (If, Then, Else) will be seperated -------

When Converted You see this:

 JASS:

function Trig_Step_1_Conditions takes nothing returns boolean // This is your basic Condition
 if ( not ( GetTriggerUnit() == gg_unit_hfoo_0000 ) ) then
 return false
 endif
 return true
endfunction

function Trig_Step_1_Func002C takes nothing returns boolean // this is the condition for the (If, Then, Else) that was in Actions
 if ( not ( GetUnitStateSwap(UNIT_STATE_LIFE, GetTriggerUnit()) >= 10 ) ) then
 return false
 endif
 return true
endfunction

function Trig_Step_1_Actions takes nothing returns nothing // your actions part have come here
 set udg_tempPoint = GetRandomLocInRect(gg_rct_myRegion)
 if ( Trig_Step_1_Func002C() ) then
 call IssuePointOrderLocBJ( GetTriggerUnit(), "move", udg_tempPoint )
 else
 call CreateNUnitsAtLoc( 1, 'hfoo', Player(0), udg_tempPoint, 270.00 ) // notice player(0). in JASS player 1 red is actually Player(0)
 endif
 call RemoveLocation( udg_tempPoint)
endfunction

//===========================================================================
function InitTrig_Step_1 takes nothing returns nothing // Your event part comes here
 set gg_trg_Step_1 = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( gg_trg_Step_1, EVENT_PLAYER_UNIT_DEATH )
 call TriggerAddCondition( gg_trg_Step_1, Condition( function Trig_Step_1_Conditions ) )
 call TriggerAddAction( gg_trg_Step_1, function Trig_Step_1_Actions )
endfunction

^^

DeathismyFriend, The Above helps people those who use different learning styles.

Also, your finished product doesn't even work.

The only thing I can notice is one of the conditions.

 JASS:

if GetUnitState( u, UNIT_STATE_LIFE) >= 10 ) then

(I am rather new to Jass so I don't even know if your missing one for sure but let me point out what you didn't show and might have forgot)

Problem 1: When I look at single line of jass above this I only see this ( ( ) I think your missing one lol.

Problem 2: You never showed them you swapped u, over to the spot UNIT_STATE_LIFE was orginally at.

Shown here:

 JASS:

if ( not ( GetUnitStateSwap(UNIT_STATE_LIFE, GetTriggerUnit()) >= 10 ) ) then

As a new user of Jass, tought by you (DeathisMyFriend) I am very possitive people would like a working final product, I use JNGP so there isn't an excuse why I can't see it in action lol.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

i updated to fix the mistake. and did u read everything. I thought i mentioned in there to note the differences in the arguments that the native functions take.

Also it does work. When using jass ur trigger name has to be the same as the InitTrig function. Just take out the InitTrig_ and replace the _ with a space in the name

#### Orthane

#### Orthane

 Level 7

JoinedJan 22, 2013Messages293

deathismyfriend said:

 i updated to fix the mistake. and did u read everything. I thought i mentioned in there to note the differences in the arguments that the native functions take.

Also it does work. When using jass ur trigger name has to be the same as the InitTrig function. Just take out the InitTrig_ and replace the _ with a space in the name


Click to expand...

I made a 100% copy, and it don't work lol.

EDIT: Perhaps my editor NJGP doesn't like me

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

like u said there was a small error with the extra parenthesis. also what did u name the trigger ?

#### Orthane

#### Orthane

 Level 7

JoinedJan 22, 2013Messages293

deathismyfriend said:

 like u said there was a small error with the extra parenthesis. also what did u name the trigger ?


Click to expand...

everything is fixed, I got your tutorial to work, I produced this.

 JASS:

function CheckPlayerEnemy takes unit U, real x1, real x2, real y1, real y2 returns nothing
 if IsPlayerEnemy( GetOwningPlayer(U), Player(13)) == true and GetUnitPointValue(U) == 0 then
 call IssuePointOrder( U, "attack", x1, y1 )
 endif
 if IsPlayerEnemy( GetOwningPlayer(U), Player(14)) == true and GetUnitPointValue(U) == 0 then
 call IssuePointOrder( U, "attack", x2, y2 )
 endif
endfunction

function CheckPoint1 takes nothing returns boolean
 local unit U = GetTriggerUnit()
 local real x1 = GetRectCenterX( gg_rct_AllianceBase)
 local real y1 = GetRectCenterY( gg_rct_AllianceBase)
 local real x2 = GetRectCenterX( gg_rct_TL2)
 local real y2 = GetRectCenterY( gg_rct_TL2)
 call CheckPlayerEnemy( U, x1, x2, y1, y2 )
 set U = null
 return false
endfunction

function CheckPoint2 takes nothing returns boolean
 local unit U = GetTriggerUnit()
 local real x1 = GetRectCenterX( gg_rct_TL1)
 local real y1 = GetRectCenterY( gg_rct_TL1)
 local real x2 = GetRectCenterX( gg_rct_TL3)
 local real y2 = GetRectCenterY( gg_rct_TL3)
 call CheckPlayerEnemy( U, x1, x2, y1, y2 )
 set U = null
 return false
endfunction

function CheckPoint3 takes nothing returns boolean
 local unit U = GetTriggerUnit()
 local real x1 = GetRectCenterX( gg_rct_TL2)
 local real y1 = GetRectCenterY( gg_rct_TL2)
 local real x2 = GetRectCenterX( gg_rct_TL4)
 local real y2 = GetRectCenterY( gg_rct_TL4)
 call CheckPlayerEnemy( U, x1, x2, y1, y2 )
 set U = null
 return false
endfunction

function CheckPoint4 takes nothing returns boolean
 local unit U = GetTriggerUnit()
 local real x1 = GetRectCenterX( gg_rct_TL3)
 local real y1 = GetRectCenterY( gg_rct_TL3)
 local real x2 = GetRectCenterX( gg_rct_TL5)
 local real y2 = GetRectCenterY( gg_rct_TL5)
 call CheckPlayerEnemy( U, x1, x2, y1, y2 )
 set U = null
 return false
endfunction

function CheckPoint5 takes nothing returns boolean
 local unit U = GetTriggerUnit()
 local real x1 = GetRectCenterX( gg_rct_TL4)
 local real y1 = GetRectCenterY( gg_rct_TL4)
 local real x2 = GetRectCenterX( gg_rct_TL6)
 local real y2 = GetRectCenterY( gg_rct_TL6)
 call CheckPlayerEnemy( U, x1, x2, y1, y2 )
 set U = null
 return false
endfunction

function CheckPoint6 takes nothing returns boolean
 local unit U = GetTriggerUnit()
 local real x1 = GetRectCenterX( gg_rct_TL5)
 local real y1 = GetRectCenterY( gg_rct_TL5)
 local real x2 = GetRectCenterX( gg_rct_HordeBase)
 local real y2 = GetRectCenterY( gg_rct_HordeBase)
 call CheckPlayerEnemy( U, x1, x2, y1, y2 )
 set U = null
 return false
endfunction

function InitTrig_A001 takes nothing returns nothing
 local region R1 = CreateRegion()
 local region R2 = CreateRegion()
 local region R3 = CreateRegion()
 local region R4 = CreateRegion()
 local region R5 = CreateRegion()
 local region R6 = CreateRegion()
 local trigger T1 = CreateTrigger()
 local trigger T2 = CreateTrigger()
 local trigger T3 = CreateTrigger()
 local trigger T4 = CreateTrigger()
 local trigger T5 = CreateTrigger()
 local trigger T6 = CreateTrigger()
 call RegionAddRect(R1, gg_rct_TL1)
 call RegionAddRect(R2, gg_rct_TL2)
 call RegionAddRect(R3, gg_rct_TL3)
 call RegionAddRect(R4, gg_rct_TL4)
 call RegionAddRect(R5, gg_rct_TL5)
 call RegionAddRect(R6, gg_rct_TL6)
 call TriggerRegisterEnterRegion(T1, R1, null)
 call TriggerRegisterEnterRegion(T2, R2, null)
 call TriggerRegisterEnterRegion(T3, R3, null)
 call TriggerRegisterEnterRegion(T4, R4, null)
 call TriggerRegisterEnterRegion(T5, R5, null)
 call TriggerRegisterEnterRegion(T6, R6, null)
 call TriggerAddCondition( T1, Condition(function CheckPoint1))
 call TriggerAddCondition( T2, Condition(function CheckPoint2))
 call TriggerAddCondition( T3, Condition(function CheckPoint3))
 call TriggerAddCondition( T4, Condition(function CheckPoint4))
 call TriggerAddCondition( T5, Condition(function CheckPoint5))
 call TriggerAddCondition( T6, Condition(function CheckPoint6))
 set R1 = null
 set R2 = null
 set R3 = null
 set R4 = null
 set R5 = null
 set R6 = null
 set T1 = null
 set T2 = null
 set T3 = null
 set T4 = null
 set T5 = null
 set T6 = null
endfunction

Right to the point is my style

+Rep

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Orthane said:

 everything is fixed, I got your tutorial to work, I produced this.

 JASS:

function CheckPlayerEnemy takes unit U, real x1, real x2, real y1, real y2 returns nothing
 if IsPlayerEnemy( GetOwningPlayer(U), Player(13)) == true and GetUnitPointValue(U) == 0 then
 call IssuePointOrder( U, "attack", x1, y1 )
 endif
 if IsPlayerEnemy( GetOwningPlayer(U), Player(14)) == true and GetUnitPointValue(U) == 0 then
 call IssuePointOrder( U, "attack", x2, y2 )
 endif
endfunction

function CheckPoint1 takes nothing returns boolean
 local unit U = GetTriggerUnit()
 local real x1 = GetRectCenterX( gg_rct_AllianceBase)
 local real y1 = GetRectCenterY( gg_rct_AllianceBase)
 local real x2 = GetRectCenterX( gg_rct_TL2)
 local real y2 = GetRectCenterY( gg_rct_TL2)
 call CheckPlayerEnemy( U, x1, x2, y1, y2 )
 set U = null
 return false
endfunction

function CheckPoint2 takes nothing returns boolean
 local unit U = GetTriggerUnit()
 local real x1 = GetRectCenterX( gg_rct_TL1)
 local real y1 = GetRectCenterY( gg_rct_TL1)
 local real x2 = GetRectCenterX( gg_rct_TL3)
 local real y2 = GetRectCenterY( gg_rct_TL3)
 call CheckPlayerEnemy( U, x1, x2, y1, y2 )
 set U = null
 return false
endfunction

function CheckPoint3 takes nothing returns boolean
 local unit U = GetTriggerUnit()
 local real x1 = GetRectCenterX( gg_rct_TL2)
 local real y1 = GetRectCenterY( gg_rct_TL2)
 local real x2 = GetRectCenterX( gg_rct_TL4)
 local real y2 = GetRectCenterY( gg_rct_TL4)
 call CheckPlayerEnemy( U, x1, x2, y1, y2 )
 set U = null
 return false
endfunction

function CheckPoint4 takes nothing returns boolean
 local unit U = GetTriggerUnit()
 local real x1 = GetRectCenterX( gg_rct_TL3)
 local real y1 = GetRectCenterY( gg_rct_TL3)
 local real x2 = GetRectCenterX( gg_rct_TL5)
 local real y2 = GetRectCenterY( gg_rct_TL5)
 call CheckPlayerEnemy( U, x1, x2, y1, y2 )
 set U = null
 return false
endfunction

function CheckPoint5 takes nothing returns boolean
 local unit U = GetTriggerUnit()
 local real x1 = GetRectCenterX( gg_rct_TL4)
 local real y1 = GetRectCenterY( gg_rct_TL4)
 local real x2 = GetRectCenterX( gg_rct_TL6)
 local real y2 = GetRectCenterY( gg_rct_TL6)
 call CheckPlayerEnemy( U, x1, x2, y1, y2 )
 set U = null
 return false
endfunction

function CheckPoint6 takes nothing returns boolean
 local unit U = GetTriggerUnit()
 local real x1 = GetRectCenterX( gg_rct_TL5)
 local real y1 = GetRectCenterY( gg_rct_TL5)
 local real x2 = GetRectCenterX( gg_rct_HordeBase)
 local real y2 = GetRectCenterY( gg_rct_HordeBase)
 call CheckPlayerEnemy( U, x1, x2, y1, y2 )
 set U = null
 return false
endfunction

function InitTrig_A001 takes nothing returns nothing
 local region R1 = CreateRegion()
 local region R2 = CreateRegion()
 local region R3 = CreateRegion()
 local region R4 = CreateRegion()
 local region R5 = CreateRegion()
 local region R6 = CreateRegion()
 local trigger T1 = CreateTrigger()
 local trigger T2 = CreateTrigger()
 local trigger T3 = CreateTrigger()
 local trigger T4 = CreateTrigger()
 local trigger T5 = CreateTrigger()
 local trigger T6 = CreateTrigger()
 call RegionAddRect(R1, gg_rct_TL1)
 call RegionAddRect(R2, gg_rct_TL2)
 call RegionAddRect(R3, gg_rct_TL3)
 call RegionAddRect(R4, gg_rct_TL4)
 call RegionAddRect(R5, gg_rct_TL5)
 call RegionAddRect(R6, gg_rct_TL6)
 call TriggerRegisterEnterRegion(T1, R1, null)
 call TriggerRegisterEnterRegion(T2, R2, null)
 call TriggerRegisterEnterRegion(T3, R3, null)
 call TriggerRegisterEnterRegion(T4, R4, null)
 call TriggerRegisterEnterRegion(T5, R5, null)
 call TriggerRegisterEnterRegion(T6, R6, null)
 call TriggerAddCondition( T1, Condition(function CheckPoint1))
 call TriggerAddCondition( T2, Condition(function CheckPoint2))
 call TriggerAddCondition( T3, Condition(function CheckPoint3))
 call TriggerAddCondition( T4, Condition(function CheckPoint4))
 call TriggerAddCondition( T5, Condition(function CheckPoint5))
 call TriggerAddCondition( T6, Condition(function CheckPoint6))
 set R1 = null
 set R2 = null
 set R3 = null
 set R4 = null
 set R5 = null
 set R6 = null
 set T1 = null
 set T2 = null
 set T3 = null
 set T4 = null
 set T5 = null
 set T6 = null
endfunction


Right to the point is my style


+Rep


Click to expand...


thx for the rep.


it can still be shortened if u use an array and store the x and y values into them. it can be shortened to 3 mybe 4 functions.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

so basicly


1. convert the trigger to GUI-ish JASS


2. replace BJ functions with natives


done?

#### Xerxes

#### Xerxes

 Level 20

JoinedApr 14, 2012Messages2,906

Resources: 8

#### Resources
Icons 4Maps 1Spells and Systems 3

Chaosy said:

 so basicly


1. convert the trigger to GUI-ish JASS


2. replace BJ functions with natives


done?


Click to expand...


I don't think so... there are some natives and functions that GUI does not have.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

if the native dont exist in GUI, I cant convert it from GUI

 right?

#### Xerxes

#### Xerxes

 Level 20

JoinedApr 14, 2012Messages2,906

Resources: 8

#### Resources
Icons 4Maps 1Spells and Systems 3

Chaosy said:

 if the native dont exist in GUI, I cant convert it from GUI

 right?


Click to expand...

Well if you used the Custom Script in GUI I guess- uh, right.

Nice systems btw

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

That is true that some natives GUI doesnt have. Converting and changing the BJs arent all u have to do tho. I show how to reduce the returns from conditions and that u shouldnt use TriggerAddAction instead use TriggerAddCondition.

#### Orthane

#### Orthane

 Level 7

JoinedJan 22, 2013Messages293

deathismyfriend said:

 That is true that some natives GUI doesnt have. Converting and changing the BJs arent all u have to do tho. I show how to reduce the returns from conditions and that u shouldnt use TriggerAddAction instead use TriggerAddCondition.


Click to expand...


You should show the difference between A Loop in GUI and one in JASS


Also there is a rather large difference between the names of GUI triggers and JASS triggers.


Such as:


> The Difference between a Region and a Rect.


> Force/PlayerGroup


> UnitGroup/UnitPool


also not many people who convert understand the new flex-abilities they are able to control now.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Orthane said:

 You should show the difference between A Loop in GUI and one in JASS


Also there is a rather large difference between the names of GUI triggers and JASS triggers.


Such as:


> The Difference between a Region and a Rect.


> Force/PlayerGroup


> UnitGroup/UnitPool


also not many people who convert understand the new flex-abilities they are able to control now.


Click to expand...


Unit group is not the same as a unit pool


a unit group in GUI is a group in JASS


not sure what ur last sentence is supposed to mean either lol.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

All in all, we should encourage people to learn to write and learn JASS from scratch, not converting GUI to JASS.


Bad things happen to people who try to mess around with JASS that don't actually know what they are doing. I think learning JASS by converting GUI to JASS makes them learn bad habbits.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Zwiebelchen said:

 All in all, we should encourage people to learn to write and learn JASS from scratch, not converting GUI to JASS.


Bad things happen to people who try to mess around with JASS that don't actually know what they are doing. I think learning JASS by converting GUI to JASS makes them learn bad habbits.


Click to expand...


and again this is how i learned. i dont think it was a bad way to learn. The main thing that ppl have to know when learning this way is how to inline. Which is very easy with TESH. ( i know there are others, never used them b4 tho)

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Zwiebelchen said:

 All in all, we should encourage people to learn to write and learn JASS from scratch, not converting GUI to JASS.


Bad things happen to people who try to mess around with JASS that don't actually know what they are doing. I think learning JASS by converting GUI to JASS makes them learn bad habbits.


Click to expand...


If I didn't learn JASS by converting GUI triggers, then I wouldn't have learned it at all.


All tutorials that tried to teach JASS simply failed miserably. (Even the JASS class, as it fits more for improving your skills rather than learning something)

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Xonok said:

 If I didn't learn JASS by converting GUI triggers, then I wouldn't have learned it at all.


All tutorials that tried to teach JASS simply failed miserably. (Even the JASS class, as it fits more for improving your skills rather than learning something)


Click to expand...


thats the problem i had lol. most made no sense to me. Then i started to convert GUI to JASS to look for the syntax and to see what line of code meant what. Thats y i made this to make it easier for the ppl that want to learn this way.

#### Orthane

#### Orthane

 Level 7

JoinedJan 22, 2013Messages293

The reason why people fail to learn in the jass Classes and such is because they have to learn things strait to the point, and learn the critical needs as they progress farther and farther.

The main issue is that no one really explains anything enuf or so much that it gives you nothing.

Triggering is a Monkey See Monkey Do process when first learning, it is a Creative Copy-Cat Learning Style to Learn Triggering. Where people use their own ideas to use replications of others successful triggering.

The best way to learn is to give the basic's on structures, the difference in how things are being used. How to make things work correctly.

When a person is first starting Jass they are still exploring what they are able to do, when they see a line of trigger that shows them a new things they can do they will copy cat that general piece or it would be given to them as a tip when they asked for help.

No matter how they learn there are three different learning styles. Nearly everyone who is already experienced tries to teach people in the Learning style that involves read and understand with nearly no examples.

The second learning styles requires experimenting step by step but has to have room to use things on their own terms otherwise they know nothing.

The Third Learning style which no one helps towards at all is the learning style I have, When you see something done you learn it, a Visual Copy-Cat learning (I forget the name of it lol) Because no one likes copy-cats they don't support this class of learning.

Well, if I can see it done I can memorize it and use it for any creative feature I can think of because it sticks with me when I see it open to a new area or know it can be opened to that area I will use that new feature, so it keeps me in a field of limitation until one can show me how to reach farther then what I currently make. DeathismyFriend has allowed me to reach that far.

Personally Triggering is one of the hardest things to do with my learning style because of that fact.

#### Spartipilo

#### Spartipilo

 Level 20

JoinedJul 14, 2011Messages3,213

I think jass (and any programming) learning is better learned from the base: logic.

Logic behind JASS interpretation.

Logic behind Wc3 script parse.

Logic behind GUI limitations.

Logic behind integers and arrays.

Logic behind 32-bit based game plataform

Logic behind a 1D array, and a 2D array.

Logic behind Bj's

Logic behind leaks

and so on...

As some others have said, this tutorial introduces people to a ONE WAY to learn. It's absurd to think there's a not-found-yet perfect-universal way for everyone to learn the same thing. No one would read a tutorial that covers everything. If someone wants to learn, it will read, will try something, will not succeed, and will ask for help.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

i never said this is the only way to learn. its just another alternative to what is already out there. this is how i learned. i just converted some of my triggers and looked for the similarities. So as a tutorial i think this is a good way. it introduces ppl to another way to learn jass from what they already know ( GUI )

#### Spartipilo

#### Spartipilo

 Level 20

JoinedJul 14, 2011Messages3,213

I never meant to say you were thinking this is the only way; I was rather telling the rest to admit this as a valuable resource since it provides a new approach to jass learning some may found more usefull than the other out there. Good job.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

oooo lol thanks i mustve misread or misunderstood something lol

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Okay, after rereading it I've decided that it qualifies to be approved. Even though it isn't necessarily the best "practice", this is usually the route people take (including me) to learn how to write JASS code. After a while, they'll get the hang of it and be able to write it from scratch themselves.

The other tutorials I linked covered similar topics, but not explicitly converting GUI into JASS, so I think that this may be useful.

There is certainly room for improvement, but I think that overall people will benefit from this. Before I can approve it, you need to fix Step 9. The part in the hidden tags should be in JASS tags.

I won't be a grammar nazi this time. I just require the tutorials to be readable. However, you may want to look into spell check or a similar functionality for your web browser to make the post more professional. Even though it isn't necessary, it will help readability and will reflect well maturity-wise.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

ill fix step 9.

will reflect maturity-wise ? just because i dont care about grammar doesnt mean im not mature. at 19 i started my own business lol. i think id have to be pretty mature to have it still working great.

edit:

updated and fixed step 9s bb codes.

#### Spartipilo

#### Spartipilo

 Level 20

JoinedJul 14, 2011Messages3,213

deathismyfriend said:

 ill fix step 9.

will reflect maturity-wise ? just because i dont care about grammar doesnt mean im not mature. at 19 i started my own business lol. i think id have to be pretty mature to have it still working great.

edit:

updated and fixed step 9s bb codes.


Click to expand...

Congratulations about your own business. This was, however, not a mature answer.

Bad grammar requires the reader to put some effort to understand what you write and makes the text "ugly", "not provocative". People just don't want to read something with bad grammar, it's exhausting. This reduces your resource impact; people may pass by it, but not read it, or jump to another one with better grammar SPECIALLY when you're talking about triggers and scripts that require perfect grammar to work.

Be easy, PurgeAndFire is giving some tips to help you improve your resource, not to discredit it nor offend you.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Spartipilo said:

 Congratulations about your own business. This was, however, not a mature answer.

Bad grammar requires the reader to put some effort to understand what you write and makes the text "ugly", "not provocative". People just don't want to read something with bad grammar, it's exhausting. This reduces your resource impact; people may pass by it, but not read it, or jump to another one with better grammar SPECIALLY when you're talking about triggers and scripts that require perfect grammar to work.

Be easy, PurgeAndFire is giving some tips to help you improve your resource, not to discredit it nor offend you.


Click to expand...

i did forget to say that i would spend some time on fixing the grammar as yes this is a tutorial and it should be easily readable by everyone.

I didnt mean to bash purge lol. But yes i do apologize for forgetting to say i would fix it and that purge is right. I just didnt like that maturity comment lol.

I dont believe grammar to be deciding on maturity lol

Edit: i fixed the spelling and most if not all the grammar


 Last edited: Aug 2, 2013

 You must log in or register to reply here.


### Similar threads

 All About Inlining


Flux

Mar 25, 2016

JASS Tutorials

Replies3Views5K

Apr 23, 2016
PurgeandFire

 Vanilla Jass Header Initialization


Almia

Aug 27, 2013

JASS Tutorials

Replies8Views6K

Aug 31, 2013
Rheiko

Locked

 Troubleshooting help with JASS


Metov

Jul 6, 2009

World Editor Help Zone

Replies0Views1K

Jul 6, 2009
Metov

Locked

[Spell]
 GUI to JASS help


sniper_zero

Jul 9, 2013

World Editor Help Zone

Replies26Views3K

Jul 12, 2013
deathismyfriend

Locked

 What is more efficient way about add actions/conditions to a trigger?


HerlySQR

May 21, 2021

World Editor Help Zone

Replies5Views2K

May 22, 2021
HerlySQR

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