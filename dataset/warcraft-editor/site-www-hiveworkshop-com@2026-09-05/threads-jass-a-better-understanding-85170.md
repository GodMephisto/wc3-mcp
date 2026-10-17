---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/jass-a-better-understanding.85170/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# JASS a Better Understanding
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/jass-a-better-understanding.85170/>
#### Ghan_04

#### Ghan_04

 Level 22

JoinedFeb 26, 2008Messages893

Resources: 2

#### Resources
Tutorials 2

Welcome back to what has turned into a series to tutorials concerning JASS hot off my keyboard.

If you have not journeyed with me before into the realm of JASS, I suggest that you take a look at that past venture here:

JASS: A Concise Introduction

before continuing.

This tutorial will attempt to expand your understanding of what JASS is and enhance your abilities with it. I will endeavor to keep it as simple as I did last time as we move into more advanced concepts.

And I suppose that is what we are talking about in this tutorial.

Concepts.

A scary proposition. I'm not going to be giving you blocks of code with the words, "This chunk of code will do this for you." Not going to happen. The purpose of this tutorial is to make you understand JASS. Not just be able to wield it without knowing what it is. That would be like handing you some big, complicated weapon and saying, "Here! Press the big red button when the enemy gets close!" without even giving you an explanation of what, exactly, it is.

So, let us begin this newest trek.

Introduction (Take Two)

JASS is a programming language. Or, more simply, a language. You use this language to communicate with the computer what you want done. But computers are stupid. They do exactly what you tell them, when you tell them, in the manner that you tell them. And you must tell them what to do in a very specified way.

What does this mean? This means that whenever a computer does something you didn't want it to, if the programming language itself is not the issue (such as some flaw that you run into) it's not the computer's fault. It was, most unfortunately, yours. The programmer's. But don't let you discourage you. This is why you're reading a tutorial, right?

JASS has rules. Very strict rules, too. And you must follow them when telling the computer what to do.

So, let us move onward to telling the computer what to do in a proper manner, shall we?

Functions Revisited

And Boolexprs
We've looked at functions. But we need to look at them even more to fully comprehend what they can do.

Functions allow you to do actions. They are where you put your code.

They can take and return values. Take a look:

 JASS:

function SomeFunc takes integer A returns integer
 return A + 5
endfunction

What's going on here? An integer was given, and an integer was returned. The integer that was taken got 5 added to it, and that value was returned. Simple enough? Certainly.

So, what can you do with your brand new function?

Use it.

How, you ask?

Well, let's say you wanted to use some number in a function, and you wanted to add 5 to it.

How about a loop condition? Remember loops?

 JASS:

loop
 exitwhen SomeInteger > SomeFunc(5)
 // Do Stuff!
 set SomeInteger = SomeInteger + 1
endloop

What have we here? Our function we looked at above! It takes 5. But, how can it but used in an integer comparison? Take a look. It returns an integer. So, you can use it just like one! What will this function return? 10. 5 goes in, it adds 5, then returns it. 5 + 5 = 10. Easy. What if you wanted to have an add function where you can give it the two numbers to add? Take a look at this:

 JASS:

function add takes integer A, integer B returns integer
 return A+B
endfunction

This function will take two numbers, and them together, and give you the result. Obviously, this is not a very useful function for practical purposes, but it gives a good illustration of the basic operation of functions.

Functions also have other purposes.

You know when you create some condition in your GUI trigger? That's a function.

Try this! Create a new trigger. Add some condition, and convert it to JASS. What do you get? Here's mine:

 JASS:

function Trig_Untitled_Trigger_002_Conditions takes nothing returns boolean
 if ( not ( IsUnitType(GetTriggerUnit(), UNIT_TYPE_STRUCTURE) == true ) ) then
 return false
 endif
 return true
endfunction

function Trig_Untitled_Trigger_002_Actions takes nothing returns nothing
endfunction

//===========================================================================
function InitTrig_Untitled_Trigger_002 takes nothing returns nothing
 set gg_trg_Untitled_Trigger_002 = CreateTrigger( )
 call TriggerAddCondition( gg_trg_Untitled_Trigger_002, Condition( function Trig_Untitled_Trigger_002_Conditions ) )
 call TriggerAddAction( gg_trg_Untitled_Trigger_002, function Trig_Untitled_Trigger_002_Actions )
endfunction

Start at the bottom. There's the function that creates the trigger. Above it is the actions function. And above that is some new function. How does that work? Well, take a look down at the bottom again. See the call TriggerAddCondition line? That adds a condition to the function. The name of the trigger is there, and some thing that says Condition( function Trig_Untitled...). Well, that's the condition. Now look at that function up top.

Just read the first line. See how it returns boolean? That's how it gives you a yes or no for the trigger. It gives the trigger true or false to that condition that you entered. The syntax of that actual condition is there, but see how it returns true or returns false depending on the comparison. That's the key. It gives true or false back to the AddCondition call there at the bottom.

The Condition( function ...) part is a bit tricky. This is what is called a boolexpr (boolean expression). It's an entire condition that is in itself a function. You can store these in boolexpr variables if you wish. Boolexpr functions will always return a boolean. Unfortunately, they cannot take any parameters when used in this manner. JASS will yell at you for trying.

Pick Every Unit Functions and Matching Functions

Ah, yes. Everyone's favorite. Pick Every Unit and Do Actions. Hopefully you know how to eliminate the leaks that come with that. We'll be looking at it in JASS, of course.

So, create some such function and convert it. Here's what I have:

 JASS:

function Trig_Untitled_Trigger_002_Func001A takes nothing returns nothing
endfunction

function Trig_Untitled_Trigger_002_Actions takes nothing returns nothing
 call ForGroupBJ( udg_SomeGroup, function Trig_Untitled_Trigger_002_Func001A )
endfunction

//===========================================================================
function InitTrig_Untitled_Trigger_002 takes nothing returns nothing
 set gg_trg_Untitled_Trigger_002 = CreateTrigger( )
 call TriggerAddAction( gg_trg_Untitled_Trigger_002, function Trig_Untitled_Trigger_002_Actions )
endfunction

Again, start at the bottom. There's our trigger starter function. Now look up. There an action in the action function, but it doesn't look like something we want. It says call ForGroupBJ( udg_SomeGroup, function Trig_Untitled...).

What's this? Well, whatever actions you want to do under the Pick Every function go in their own function that is called through the ForGroupBJ function.

This means that actions you want to have happen to each unit goes in the function that is named in the ForGroupBJ call. The way you get the Picked Unit in that function is GetEnumUnit().

The same sort of thing happens with functions that set some group of units or use the Matching keyword are used. Take a look:

 JASS:

function Trig_Untitled_Trigger_002_Func001002003 takes nothing returns boolean
 return ( IsUnitType(GetTriggerUnit(), UNIT_TYPE_STRUCTURE) == true )
endfunction

function Trig_Untitled_Trigger_002_Actions takes nothing returns nothing
 set udg_SomeGroup = GetUnitsInRangeOfLocMatching(500.00, GetRectCenter(GetPlayableMapRect()), Condition(function Trig_Untitled_Trigger_002_Func001002003))
endfunction

//===========================================================================
function InitTrig_Untitled_Trigger_002 takes nothing returns nothing
 set gg_trg_Untitled_Trigger_002 = CreateTrigger( )
 call TriggerAddAction( gg_trg_Untitled_Trigger_002, function Trig_Untitled_Trigger_002_Actions )
endfunction

See the Action function where it's setting the variable? It uses the GetUnitsInRange function (this is a BJ function) and includes a condition just like we saw earlier. Yes, it's a boolexpr. And this function is using it to get the correct units to put into the unit group variable. It tests units, and puts them in the group if they meet the conditions. That's the equivalent of the Matching part of the function in GUI. Pretty simple. Here's the GUI that I used:

 Code:

Set SomeGroup = (Units within 500.00 of (Center of (Playable map area)) matching (((Triggering unit) is A structure) Equal to True))

Local and Global Variables - The Shakedown

So, what are they? Well, global variables are the ones in the Variable Editor. You create them there and can use them in any trigger anywhere. They're Global to the entire world that is your map. So, how are local variables different? Well, let's explore that....

Local variables are created in a function and can only be used in that function. A quick example:

 JASS:

function SomeFunc takes nothing returns nothing
 local unit SomeUnit = GetTriggerUnit()
endfunction

That creates a local unit variable called SomeUnit, and, for good measure, we set it to the triggering unit, assuming there is one.

You can use this variable in that function, and there's no need for a udg_ prefix! You can use it as-is.

The best thing about local variables is that they can't be overwritten. They only exist the one time the function is called. They act sort of like Triggering Unit in GUI. The same function can be called multiple times, but it won't lose what was stored the first time. That will be key when making spells MUI.

Leaks

With local variables come more leak problems. Any local handle - WHAT!? Handle! Huh!?!

Handles. A handle is basically some object in Warcraft. If you take a look at the basic variable types in JASSCraft, you may see that some extend handle. These things will most likely leak if you use local versions of them. This is because the thing you stored can't ever be accessed again, but it's still in that variable. This also can occur with objects that extend the widget type. You don't really have to know what all of that means, just know these things can leak.

Local units can leak.

So can timers.

Unit Groups leak a bit differently when they are locals.

Locations are the same as Unit Groups.

But these are fairly simple to resolve.

If the object has a destroy function, such as timers, use it first.

call DestroyTimer(SomeTimer)

call RemoveLocation(SomeLocation)

Then, set the variable to null. Like so:

set SomeUnit = null

set SomeGroup = null

One other thing. If the object in question will never be destroyed, there is no need to clean the leak as there will not be one. For example, you don't need to clean player leaks. Even though they can technically leak, players never get destroyed, so there is no need to do anything.

Pretty easy. That should do it for your leaks.

Integers, reals, booleans, strings, and codes do not need to be destroyed or nulled, so no need to worry about them! Hold on. Only one section left to go!

BJs

Remember that ForGroupBJ that we looked at earlier? That is a BJ function, hence the two letters there at the end. This stands for Blizzard JASS. Unfortunately for us, GUI uses a lot of these, and they can be inefficient.

Take a look at this:

 JASS:

SetUnitAbilityLevelSwapped(integer abilcode, unit whichUnit, integer level)

This is a BJ function. It doesn't say BJ, but you can find out by looking in JASSCraft to see if that function calls another function. If it says native, though, it's not a BJ.

All this does is change the order of the parameters of the native function.

Here's the native:

 JASS:

SetUnitAbilityLevel(unit whichUnit, integer abilcode, integer level)

See that? All the one above does is call this one with the things in the right place. Isn't that inefficient? It calls the first one, then the second one. When you're writing JASS, try to avoid these kinds of silly functions that make unnecessary calls.

Conclusion

So! How do you feel? Beat? Perhaps. But if you go over this tutorial a few times, I'm sure you can learn a few things. Remember this is the second tutorial in this series. I do not think it will be the last. I hope you enjoyed it, at least a little, and I'll see you back here for the next one!

The next tutorial will cover optimization as one of its topics. Just to give you something to look forward to!


 Last edited: Jul 18, 2008

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

While on the subject of inefficiencies, why not cover why GUI condition structure (when converted, that is) sucks and how to avoid/improve it?

Also, what you said about bugs is not strictly true. Jass has a lot of weird flaws that aren't your fault, but its own.

Potentially cover multiple parameters just so readers can cover how to add an indefinite number, not just a single one?

The entire list of non-handles is: integer real boolean string code

Nulling is not universal for handles. Leaks only occur if the object will ever be destroyed; that is, certain things, such as references to units that will never die, players, etc, do not need to be nulled.

boolexpr stands for boolean expression, you may want to bring that up so that it's not just a meaningless acronym to readers.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

I think this is too basic, most jass introduction tutorials cover this. You should add things like Recurtion, bitwise operations or lists.


some thing that you could add when talking of groups, it's how to loop units correctly. Examples:

 JASS:

function Trig_LoopUnits_A takes nothing returns nothing
local group g = CreateGroup()
local unit u
call GroupEnumUnitsOfPlayer(g, Player(1), null)
loop
 set u = FirstOfGroup(g)
 exitwhen u == null
 //Make actions with u
 //Example: call RemoveUnit(u)
 call GroupRemoveUnit(g,u)
endloop
call DestroyGroup(g)
set g = null
endfunction


I hate it when people always uses ForGroup, which happens to be slower and requires you to use globals for traspassing values.

#### Ghan_04

#### Ghan_04

 Level 22

JoinedFeb 26, 2008Messages893

Resources: 2

#### Resources
Tutorials 2

> While on the subject of inefficiencies, why not cover why GUI condition structure (when converted, that is) sucks and how to avoid/improve it?

Optimization comes next tutorial.

> Also, what you said about bugs is not strictly true.

Hmm.... I'm really not aware of any such bugs with the language. But I'm not surprised they exist.

> Potentially cover multiple parameters just so readers can cover how to add an indefinite number, not just a single one?

Added a bit more there....

> The entire list of non-handles is: integer real boolean string code

Cool. Thanks for the list!

> Nulling is not universal for handles.

Yes, I am aware of that. I made that more clear.

> you may want to bring that up so that it's not just a meaningless acronym to readers.

Added.

> some thing that you could add when talking of groups, it's how to loop units correctly.

Yes, but isn't that rather like teaching someone to fly a plane by putting them in the cockpit at 20000 ft. in the air alone?

This is a series. I'm stating slow and moving a little bit at a time.

And I don't know everything there is to know. From what I see here, you are far better at JASS than I do, so I may not even be able to write more advanced tutorials.


 Last edited: May 31, 2008

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

While I don't think this would fit as a standalone tutorial, as a series this would be very useful!

It was pretty easy to read, and very basic (so good for beginners, but not much else).

The code is very inefficient at places, but I see you plan to make tutorials on optimization in another time.

I like your step by step teaching method.

--donut3.5--

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

BlinkBoy said:

 I think this is too basic, most jass introduction tutorials cover this. You should add things like Recurtion, bitwise operations or lists.

some thing that you could add when talking of groups, it's how to loop units correctly. Examples:

 JASS:

function Trig_LoopUnits_A takes nothing returns nothing
local group g = CreateGroup()
local unit u
call GroupEnumUnitsOfPlayer(g, Player(1), null)
loop
 set u = FirstOfGroup(g)
 exitwhen u == null
 //Make actions with u
 //Example: call RemoveUnit(u)
 call GroupRemoveUnit(g,u)
endloop
call DestroyGroup(g)
set g = null
endfunction

I hate it when people always uses ForGroup, which happens to be slower and requires you to use globals for traspassing values.


Click to expand...

By the way, nulling boolexpr values leak.

 This is what is called a boolexpr (boolean expression). It's an entire condition that is in itself a function. You can store these in boolexpr variables if you wish. Boolexpr functions will always return a boolean. Unfortunately, they cannot take any parameters when used in this manner. JASS will yell at you for trying.


Click to expand...

Boolexprs aren't really the function itself, (right?) aren't they just pointers to the function? I don't know, what ever.  I think simply for knowledge, you should add that it might be hazardous to destroy non-runonce conditions, and that it leaks to input "null" as a value for boolexprs. So you should instead use a safe filter:

 JASS:

function SafeFilt takes nothing returns boolean
 return true
endfunction

But whatever, I am sleepy right now so some stuff might be incorrect about what I am saying.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

I never do those things, because noone has ever proved me that they leak, just by inserting a null value. Even blizzard functions have nulls in them. a null value just means that there's no pointer to an object, thus the native should use the standard value.


As far as I know about boolexpr is that they are stored in a condition catche, rather than functions themselves.


Sry if I'm late with this post, it's been 20 days, I'm not sure if I'm necroposting or not.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Eh, no problem, it's good to have this revived anyways.

And Blink, here's proof of 'null leaks':

 JASS:

function NullTest takes nothing returns nothing
 local unit u = CreateUnit(Player(0),'hfoo',0,0,0)
 call BJDebugMsg(I2S(H2I(u)))
 call RemoveUnit(u)
endfunction

function InitTrig_NullTest takes nothing returns nothing
 call TimerStart(CreateTimer(),.005,true,function NullTest)
endfunction

Now, run that and watch the handle IDs that are displayed.

Now run this one.

 JASS:

function NullTest takes nothing returns nothing
 local unit u = CreateUnit(Player(0),'hfoo',0,0,0)
 call BJDebugMsg(I2S(H2I(u)))
 call RemoveUnit(u)
 set u = null
endfunction

function InitTrig_NullTest takes nothing returns nothing
 call TimerStart(CreateTimer(),.005,true,function NullTest)
endfunction

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Okey, that indeed is true, yet how does that answers my question.


how does this leaks:

 JASS:

call GroupEnumUnitsOfPlayer(g, Player(1), null) //the null object


I'm inserting a null value, I not even created any new pointers or anything, so for starters I haven't taken a new space in memory.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Oh, never mind, I guess I misinterpreted your post.

That apparently applies only (at least mostly) to global groups which you reuse. There's a thread somewhere about it on wc3campaigns, started by grim001 I think.

#### Ghan_04

#### Ghan_04

 Level 22

JoinedFeb 26, 2008Messages893

Resources: 2

#### Resources
Tutorials 2

Any chance at approval here?


What can I do to improve this?

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

The only real problem I have with this is that using GetUnitsInRangeOfLocMatching is misleading without clarifying that it is a BJ, since it doesn't look like one.

#### Ghan_04

#### Ghan_04

 Level 22

JoinedFeb 26, 2008Messages893

Resources: 2

#### Resources
Tutorials 2

This is fixed.

#### Ghan_04

#### Ghan_04

 Level 22

JoinedFeb 26, 2008Messages893

Resources: 2

#### Resources
Tutorials 2

Massive bump time.


It's still here. I am well aware that there are few people reviewing tutorials at the moment. I'm hoping for something more in the area of just more user feedback if possible.

#### Voltrillian

#### Voltrillian

 Level 2

JoinedJan 25, 2009Messages14

i dont really understand how to make events and conditions. im just learning and nothings told me how to do that. please help

#### Foopad

#### Foopad

 Level 6

JoinedJun 15, 2008Messages175

Resources: 3

#### Resources
Maps 1Spells 2

Voltrillian, you should probably read his first tutorial, before proceeding to this one.


I for one had massive use of this tutorial, I am looking forward to seeing the next one


There's a few spelling mistakes such as: "an/a"


"This function will take two numbers, and them together," You probably mean:" This function will take two numbers and add them together,"


Otherwise great tutorial for noobs .


Blinkboy, what you were saying just confused me, and this is a tutorial for beginners it seems, so you should make your own tutorial instead of adding to others .

 You must log in or register to reply here.


### Similar threads

 JASS Tutorial


Daelin

Mar 20, 2005

Scripting Tutorials

Replies24Views38K

Nov 4, 2006
Daelin

 Jass, or "What the func?"


GhostWolf

Jul 21, 2013

Scripting Tutorials

Replies9Views9K

Oct 1, 2013
deathismyfriend

 Beginning JASS Tutorial Series


wyrmlord

Feb 16, 2007

Scripting Tutorials
234

Replies181Views124K

Feb 18, 2025
Eastern Programmer yahoo

 JASS: A Concise Introduction


Ghan_04

Feb 27, 2008

Scripting Tutorials

Replies19Views14K

Jun 20, 2009
Foopad

Locked

 How to Pick Units in JASS


Spellbound

Sep 7, 2015

Triggers & Scripts

Replies16Views5K

Sep 11, 2015
Fingolfin

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