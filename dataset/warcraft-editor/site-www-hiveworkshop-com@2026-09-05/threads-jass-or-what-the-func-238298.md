---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/jass-or-what-the-func.238298/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# JASS or What the Func
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/jass-or-what-the-func.238298/>
#### GhostWolf

#### GhostWolf

 Level 29

JoinedJul 29, 2007Messages5,174

Resources: 2

#### Resources
Tools 1Tutorials 1

ALL GLORY TO THE HYPNOTOAD.

Jass, or "What the func?"

Functions, arguments, and return values

I will begin by talking about math. More specifically, the mathematical function called Sine, usually referred to as "sin".

You can use sin in any decent calculator, it looks like this: sin(x).

The "x" denotes a number which you give sin, it does something to that number, and it returns the result to you.

In other words, if we type this line in our calculator: "sin(pi/2)", then we can say that we gave sin the number pi/2, and in response it would return to us the number 1.

Equivalently: sin(pi/2) = 1.

How is this relevant to Jass you ask? "I never came here to learn math!" you exclaim. So here we begin with Jass.

Exactly like the example above, Jass (and GUI), have a Sine function. In Jass, it is called (unimaginatively) Sin, and very much like our calculator, you use it with by writing Sin(x), and it returns to you the result.

A function is a black box, very much like Sin, that takes values (note: these are usually referred to as "arguments" or "parameters"), in this case x, and returns something (the "return value").

We don’t necessarily know what it does, but we know what to give it, and what to expect it to return.

Jass code is in fact written in functions, just like Sin. This is the declaration of Sin in Jass:

 JASS:

function Sin takes real x returns real

This line tells us that there is a function called Sin. It takes a real, which is a number, called x, and it returns the result as another number.

Sin was a function made by Blizzard, but we can write our own functions!

Let’s give it a try.

 JASS:

function HelloWorld takes nothing returns nothing

We just declared a function called HelloWorld, but with a new concept. Unlike mathematical functions, in code, we sometimes have functions that don’t take any values, don’t return results, or a combination of them both.

This might seem to make a function useless. If it doesn’t take values and returns a result, what’s the use of it? That’s for later.

Every time we define a function, we need to tell Jass where it ends. You do this with the keyword endfunction.

Our HelloWorld function with the closing keyword is this then:

 JASS:

function HelloWorld takes nothing returns nothing
endfunction

This function doesn’t actually do anything yet, so let’s make it more interesting.

Every time that we use HelloWorld, we want it to display the message "Hello World" in Warcraft.

To display messages in Warcraft, yet another function made by Blizzard is given to use. It is called BJDebugMsg, and it is defined like this:

 JASS:

function BJDebugMsg takes string s returns nothing

We give BJDebugMsg a string, which is another word for text, named s, and it creates an in-game message for us with that string.

This is an example of a function, that while it doesn’t return a result to us, it is still useful, because it did something we wanted also without returning a result.

Since we wanted to display "Hello World" in our HelloWorld function, this is how it now looks:

 JASS:

function HelloWorld takes nothing returns nothing
 call BJDebugMsg("Hello World")
endfunction

There are two things to note here.

The "call" keyword was used. This is the same as telling Jass "use this function".

Our text was wrapped with quotation marks. They are used to define strings (text).

Now, since our HelloWorld function actually does something, let’s call (use) it!

 JASS:

call HelloWorld()

This would display our message in-game.

So we made a function that takes nothing, returns no result, but displays a message for us.

Let’s now create a function that does take values from us, and does return a result.

We will define a function named Add, that, unimaginatively, takes two numbers, adds them together, and returns the result.

 JASS:

function Add takes real a, real b returns real
 return a + b
endfunction

As you can see, if you want a function to take more than one value, you need to put a comma (,) between each value.

So we pass our function two numbers, named "a" and "b", and we return a number as a result.

The return keyword is used to return a result, and what follows it is what it is going to return, in this case, a + b.

We can now call this function with any arbitrary numbers, and it will return to us their addition.

call Add(5, 10) will return the result 15.

We will now create a function that calls our Add function, and creates an in-game message with the result.

Before we do that, let me introduce you to our third function created by Blizzard, called R2S.

Its definition looks like this:

 JASS:

function R2S takes real r returns string

It takes a number named "r", and returns to us a string representation of that number.

What this means, is that if we gave it the number 10, it will return the string "10" (note the quotation marks).

Since we want to make a function that creates an in-game message, we must use BJDebugMsg. Since BJDebugMsg can only create a message from a string, we must pass it a string, and since we want the message to be a result from Add, which returns a number, we must convert that number to a string, and so we must use R2S.

Let’s now define our new function. I’ll name it "MessageNumber". It will take two numbers, return no result, and create an in-game message containing the addition of those numbers.

 JASS:

function MessageNumber takes real a, real b returns nothing
 call BJDebugMsg(R2S(Add(a, b)))
endfunction

Whoah! That line of code looks a little daunting, so let’s go over it.

We first call our Add function to add a and b.

We then give the result of Add to R2S, which returns its string representation.

Finally, we give the result of R2S to BJDebugMsg, so it would create the message for us.

I bet something is bothering you about the above function...because it lacks call keywords!

What I didn’t mention until now, is that there are two ways to use a function.

Calling it, like we always did with the call keyword. Do this if the function doesn’t return any result, or if you don’t care what the result is.

Omitting the call keyword. Do this if you want to use the result of a function.

In MessageNumber, since we want to use the result of Add as the input for R2S, we omit the call keyword. In the same way, since we want to use the result of R2S in BJDebugMsg, we again omit it.

Variables

We are now going to set functions aside, and forget about them for a moment (but make sure to remember about them after the moment ends!).

Let’s talk about variables.

A variable is a data store, or, a variable is something in which you can store data.

You can use variable to store any data you want, for example: the results of functions calls.

There are two variable types in Jass. Local scope variables, and global scope variables.

Let’s begin with local scope variables, but dump the word "scope" because it makes the text lengthy.

You start defining a local variable with, you probably guessed it, the keyword local.

After local, you need to write the type of the variable. So far we went over the real and string types.

Following is the name of the variable. You can use any name you want! It’s a free language!

Let’s start then, by creating a local real and a local string:

 JASS:

local real r
local string s

Now that we have "r" and "s", and we can store any number or string we want into each respectively, by using the keyword set, following by the variable name, following by an equation mark (=), and finally following with what we want to store in it.

 JASS:

set r = 5
set s = "Hi"

And we stored the number 5 in "r", and the string "Hi" in "s".

We can, in fact, store any kind of code that results in the type of our variables.

This means that, remembering our Add function (yes, it’s time to partially remember functions again) and R2S, and how they return a number and a string, we can use them directly to store the results in "r" and "s":

 JASS:

set r = Add(5, 10)
set s = R2S(r)

"r" will now have the number 15 stored in it, and "s" will have the string "15" stored in it.

Also note that we didn’t need the call keyword, because we are using the results of Add and R2S.

Here as an example of using a variable in our Add function:

 JASS:

function Add takes real a, real b returns real
 local real c = a + b
 return c
endfunction

We create a real variable named "c", store a + b in it, and then return it as a result.

You can only define local variables inside functions, and then only at the very beginning of them.

This means that if you need a local variable, but you don’t know its value at the beginning, you still must define it at the beginning, and then set it later.

As an example, let’s edit our MessageNumber function. We will add a call to HelloWorld like so:

 JASS:

function MessageNumber takes real a, real b returns nothing
 call HelloWorld()
 call BJDebugMsg(R2S(Add(a, b)))
endfunction

This function now creates an in-game message saying "Hello World", and then another message with our number result from Add(a, b).

Let’s try to use a variable, "c", to store the result from Add, like we’ve already seen:

 JASS:

function MessageNumber takes real a, real b returns nothing
 call HelloWorld()
 local real c = Add(a, b) // Error!
 call BJDebugMsg(R2S(c))
endfunction

Note the introduction of comments. Anything that is written after "//" is ignored by Jass.

This code is not valid, because we must define locals at the very beginning of the function. This means you can’t call functions before declaring a local variable.

Here is how the above should be written:

 JASS:

function MessageNumber takes real a, real b returns nothing
 local real c = Add(a, b)
 call HelloWorld()
 call BJDebugMsg(R2S(c))
endfunction

Or, another variation that works:

 JASS:

function MessageNumber takes real a, real b returns nothing
 local real c
 call HelloWorld()
 set c = Add(a, b)
 call BJDebugMsg(R2S(c))
endfunction

So, we keep saying local this, local that, but what exactly does the "local" mean?

Local variables are variables that only exist inside the function they are declared in, and also then, they are copied for each call.

What this means, is that every time you call MessageNumber, you get a new copy of a variable named "c".

If you could call MessageNumber two times at the same time, you would have two copies of "c", they don’t interfere with each other.

Global variables, on the other hand, are the exact opposite.

A global variable exists everywhere, and it is never duplicated.

Global variables are declared in a globals block.

Much like how function and endfunction define a section (or block) of code, so do the globals and endglobals keywords:

 JASS:

globals
endglobals

In a globals block, you declare variables, like we have already seen, but without the local keyword.

 JASS:

globals
 real R = 5
endglobals

We now have a number variable called "R" with 5 stored in it, and we can access and change it in any function we define.

Conditions

Sometimes, you need to do something only if some conditions are met.

A simple example would be the mathematical absolute value function.

The absolute value of a number is defined such that if the number is 0 or anything above it, than the absolute of it is the number itself, and if the number is negative, than the absolute value of the number is its negative.

For example, the absolute value of 5, is 5, and the absolute value of -3 is -(-3) = 3.

So, if we were to create an absolute value function (called "abs" from now on), we need to somehow run different code, depending on the value of the input.

if the input is greater or equal to zero, we don’t actually run any code, but if it is below zero, we must negate the input.

The word "if" is the hint here.

To make a condition in, you start with if. Following that, you need the actual condition. The "what" part in "if what".

In the case of abs, we want to know if the input number (let’s call it "x") is smaller than 0.

This is how you would write it: x < 0.

"<" is generally used as a smaller-than condition.

After the condition, you follow with the keyword then.

Following that, comes your code, and finally the keyword endif.

We surmised that if "x" is smaller than 0, we need to negate it. If it’s greater or equal to 0, we don’t do anything, so this is how the function would look:

 JASS:

function abs takes real x returns real
 if x < 0 then
 return -x
 endif
 return x
endfunction

And a new concept has been introduced. You can have more than one return keyword in a function. But always remember this: when the code executes a return, it will exit. There is no point in having more than one return keyword if it doesn’t involve conditions, because the first one will always be the one to actually happen.

Now let’s create a new function called clamp. The definition will look like this:

 JASS:

function clamp takes real x, real minVal, real maxVal returns real

What clamp does, is take our number x, and make sure that it is in the range [minVal, maxVal].

In other words, if x is smaller than minVal, we must return minVal. If x is bigger than maxVal, we must return maxVal. If x is between (or equal to) minVal and maxVal, we simply return x.

As you read the sentence above, you can probably notice you need two conditions for this function.

One for x < minVal, and one for x > maxVal.

If you have more than one condition, you have two ways to write the code.

You can either have multiple if … endif blocks of code, or you can elseif in there.

That is, inside a condition block, you can use the keyword elseif to declare "what happens if the condition isn’t true".

And yet another keyword, else, is used to declare "what happens if no condition has been met".

With these keywords, you can chain multiple conditions in one block. So now let’s use these keywords to write clamp:

 JASS:

function clamp takes real x, real minVal, real maxVal returns real
 if x < minVal then
 return minVal
 elseif x > maxVal then
 return maxVal
 else
 return x
 endif
endfunction

If you read "<" and "> as "smaller than" and "greater than" respectively, then this function is saying: "if x is smaller than minVal, return minVal. Otherwise, if x is greater than maxVal, return maxVal. Otherwise, return x".

Loops

There are times, when you want to run the same code more than once, maybe with different inputs.

For example, let’s assume we want to use BJDebugMsg to write in the game all the numbers from 1 to 10, each in its own message.

The naive approach would be to simply copy and paste the call, with a different number:

 JASS:

function WriteNumbers takes nothing returns nothing
 call BJDebugMsg(R2S(1))
 call BJDebugMsg(R2S(2))
 call BJDebugMsg(R2S(3))
 call BJDebugMsg(R2S(4))
 call BJDebugMsg(R2S(5))
 call BJDebugMsg(R2S(6))
 call BJDebugMsg(R2S(7))
 call BJDebugMsg(R2S(8))
 call BJDebugMsg(R2S(9))
 call BJDebugMsg(R2S(10))
endfunction

While this works, it doesn’t look very nice. Imagine, if instead of a simple call to BJDebugMsg, we would want to repeat multiple lines of code? You would get a really big and messy function.

This is where loops come in. A loop is a code block that keeps executing itself, until we tell it to stop.

A loop begins with the keyword loop and ends with the keyword endloop.

 JASS:

loop
endloop

All the code between loop and endloop keeps running, again and again, until we somehow tell it to stop.

You tell it to stop by making an exit condition.

The idea of a loop, is that it works to make the exit condition true. That is, every time the loop runs, you get a step closer to making the exit condition true.

If this isn’t true, your loop will run forever, which is a bad thing.

You define a loop exit condition with the keyword exitwhen, followed by a condition, much like the if keyword.

Before showing a loop, let me introduce you to a new variable type. So far, we only talked about real numbers, and strings.

There is another type of numbers called "integer".

While real numbers can be of any value, integer numbers cannot have a decimal point.

-0.145, 45.3, and ¾ are all real numbers. -1 and 45 are integer numbers.

Now back to our loop. We want to BJDebugMsg all the numbers from 1 to 10 with a loop.

What can be done, is having an int variable set to 1 before the loop.

In the loop, we BJDebugMsg our int variable, and after that we add 1 to it.

The exit condition, in this case, would be when our int variable is bigger than 10.

An integer variable is used here, because integer variables should always be used for a code that counts something (unless that something can come in fractions):

 JASS:

function WriteNumbers takes nothing returns nothing
 local integer i = 1
 loop
 exitwhen i > 10
 call BJDebugMsg(I2S(i))
 set i = i + 1
 endloop
endfunction

A thing of note is the use of the I2S function. Similarly to R2S, it converts an integer number to its string representation.

Every time the loop above runs, it checks if "i" is bigger than 10. If it is, it exists. Otherwise, it BJDebugMsg’s it, and adds 1 to it.

Arrays

An array is a mechanism that allows you to store many variables under one name.

You define an array by using the array keyword after the variable type, when declaring a new variable:

 JASS:

local real array r

The variable "r" is now not a real number, but in fact a container of many real numbers (8192 to be exact).

Each of these real numbers in "r" is referred to as an "array element".

To access an element in an array, you must first write the name of the array, and then enclose the element index in square brackets: [x].

"x" in this context is any number between (and including) 0 and 8191, because Jass arrays are 0-indexed, which means that the first element is at index 0, and the last at index 8191.

Arrays cannot have values when you declare them, you can only use the set keyword after their creation to set elements.

Suppose we have 5 different global strings, and we want to BJDebugMsg them one after another.

The naive approach would be to do exactly what the previous sentence said: create 5 different variables, and BJDebugMsg them one by one:

 JASS:

globals
 string s1 = "I am string 1"
 string s2 = "I am string 2"
 string s3 = "I am string 3"
 string s4 = "I am string 4"
 string s5 = "I am string 5"
endglobals

function WriteStrings takes nothing returns nothing
 call BJDebugMsg(s1)
 call BJDebugMsg(s2)
 call BJDebugMsg(s3)
 call BJDebugMsg(s4)
 call BJDebugMsg(s5)
endfunction

This might look decent enough with only these 5 strings, but what if we want to run many lines of code per string? what if we have 200 strings?

A better approach would be to use a combination of arrays and loops then.

Note that this example runs the init function when the map loads. The next section explains how to do this.

 JASS:

globals
 string array strings
endglobals

// This will only run once when the map loads
function init takes nothing returns nothing
 set strings[0] = "I am string 1"
 set strings[1] = "I am string 2"
 set strings[2] = "I am string 3"
 set strings[3] = "I am string 4"
 set strings[4] = "I am string 5"
endfunction

function WriteStrings takes nothing returns nothing
 local integer i = 0
 loop
 exitwhen i > 4
 call BJDebugMsg(strings[i])
 set i = i + 1
 endloop
endfunction

Always remember that array elements start at index 0, not 1!

InitTrig, Triggers, Events, Conditions and Actions

Up until now, while we could write as many functions as we want, we couldn't actually get Warcraft to run them. This section explains how to do exactly that.

Every GUI trigger - those white pages in the World Editor - has a function that the game calls when you load the map.

This function is always named InitTrig_<White page name>.

If your white page is named "Blarg", then behind the scenes you have a function InitTrig_Blarg that is called as the map loads.

The declaration of this function is like so:

 JASS:

function InitTrig_<Name> takes nothing returns nothing

The purpose of this function is to be a point for you to put any code you want.

Triggers are objects that actually end up running your code. They allow you to register events that they respond to, conditions, and actions.

If this sounds exactly like GUI, it's because every GUI trigger is, in fact, a Jass trigger variable.

Creating triggers is a little different than what we did so far.

To create one, you need to call the function CreateTrigger:

 JASS:

local trigger t = CreateTrigger()

A trigger alone doesn't do anything, so we need to tell it what events to respond to first.

There are many different events in Jass (the same ones you can select in GUI). As an example, let's register an event that will run in 5 seconds:

 JASS:

call TriggerRegisterTimerEvent(t, 5, false)

The false keyword will be explained shortly.

After we successfully registered an event (or multiple ones) to our trigger, the next step is to attach a condition function to it.

A condition function is simply a function that takes nothing, and returns a boolean:

 JASS:

function <Name> takes nothing returns boolean

While you have already seen booleans, they weren't properly introduced yet.

A boolean is another variable type, that can be one of two values: true or false.

When we talked about conditions, and we wrote if x < minVal then, we actually checked to see if x < minVal is true or false.

That is, if x is indeed smaller than minVal, that would return true. If x isn't smaller than minVal, it would return false.

Every condition is in fact a boolean expression, or in other words, code that results, at the end, in a boolean: true, or false.

You attach a condition function to a trigger using the TriggerAddCondition function:

 JASS:

call TriggerAddCondition(t, Condition(function <Your Function>))

Note the Condition function inside, that tells Jass this is a condition function, and the usage of the function keyword before the function name.

When an event that a trigger is registered to happens, Jass runs the condition function. If it returns true, it runs the actions we attached to it, otherwise it doesn't do anything.

Actions are functions that take nothing and return nothing, and you attach them using the function TriggerAddAction:

 JASS:

call TriggerAddAction(t, function <Your Function>)

Note the use of the function keyword again.

Going back to the last array example, where the init function runs once when the map loads, this is how it is done:

 JASS:

globals
 string array strings
endglobals

// This will only run once when the map loads
function init takes nothing returns nothing
 set strings[0] = "I am string 1"
 set strings[1] = "I am string 2"
 set strings[2] = "I am string 3"
 set strings[3] = "I am string 4"
 set strings[4] = "I am string 5"
endfunction

function WriteStrings takes nothing returns nothing
 local integer i = 0
 loop
 exitwhen i > 4
 call BJDebugMsg(strings[i])
 set i = i + 1
 endloop
endfunction

// This assumes the white page is named "Strings"
function InitTrig_Strings takes nothing returns nothing
 call init()
 call WriteStrings()
endfunction

We don't actually need a trigger variable for this example, because it runs instantly when the map loads.

Here is an example that uses a trigger, and BJDebugMsg's our old "Hello World" (remember that guy?):

 JASS:

function actions takes nothing returns nothing
 call BJDebugMsg("Hello World")
endfunction

function conditions takes nothing returns boolean
 return true
endfunction

// This assumes the white page is named "HelloWorld"
function InitTrig_HelloWorld takes nothing returns nothing
 local trigger t = CreateTrigger()

 call TriggerRegisterTimerEvent(t, 5, false)
 call TriggerAddCondition(t, Condition(function conditions))
 call TriggerAddAction(t, function actions)
endfunction

Note that the conditions function above always returns true. In this case, it is not needed, you don't have to have conditions.

It is in the example just for completeness' sake.

Native functions, BJs, and what's good for you (if you guessed BJs, you guessed wrong)

There are two function types given by Blizzard - Native functions, and BJ function.

Native functions are functions that exist in the Warcraft engine itself. You can't recreate them.

BJ functions are functions written in Jass that use the native functions.
Some facts: Almost all BJ functions have "BJ" in their names. BJ stands for "Blizzard Jass". GUI uses BJ functions extensively.

Many of the BJ functions are simple wrappers above native functions:

 JASS:

function Wrapper takes ... returns ...
 return Native(...)
endfunction

In this case, there is no reason why not to directly call the native function.

Since many of the BJ functions are wrappers that do nothing but call a native (and most of the times change the order of arguments for some reason), they are considered useless, and impact the game performance, and are thus avoided.

Note that not all of the wrappers are useless, but many of them are.

In all the examples so far, you can recognize BJ functions by their red color, and natives by their purple color.

The only BJ that has been used so far was BJDebugMsg, which is actually a useful wrapper: it sends a message to all the players.

We can make a function that does this as well, but it would do the same exact thing, which shows that BJDebugMsg is useful.

In general, when you want to use a BJ function, you should check what that function does. If it doesn't do anything useful beside calling a native, then just call the native directly.

Here is an example of a useless BJ function:

 JASS:

function UnitAddItemSwapped takes item whichItem, unit whichHero returns boolean
 return UnitAddItem(whichHero, whichItem)
endfunction

It calls a native with the arguments swapped, nothing more.

Primitive variables, handles, and how to properly clean after yourself

There are two types of variables: primitives, and handles.

Primitive types include real numbers, integer numbers, strings, booleans, and more types that weren't discussed here.

Handles include every Warcraft object, such as triggers, units, unit groups, special effects, and the list goes on (there are many).

All handles are created through functions. We have previously seen the CreateTrigger function that creates a trigger.

When a handle is created, it takes memory. If the handle isn't needed anymore, you need to remove it. When primitive types are not needed, they remove themselves.

When handles are created, and never removed even though you are not using them anymore, it is referred to as a "memory leak".

The most used example is a location (Position in GUI), which is created with the Location function:

 JASS:

function Location takes real x, real y returns location

If we have this function:

 JASS:

function Leak takes nothing returns nothing
 local location l = Location(0, 0)
endfunction

Every time it is called, we create a new location, but never remove it. If we call it 100 times, we have 100 unused locations, and they use up memory.

To remove a location, you would use the RemoveLocation function:

 JASS:

function NoLeak takes nothing returns nothing
 local location l = Location(0, 0)

 call RemoveLocation(l)
endfunction

Now we can call it any number of times without using up memory for locations we are not using.

However, there is a second variation of leaking memory!

If you set a handle variable to any value, you must set it to the special value null when you don't need to use it anymore (generally at the end of the function).

The previous example should, then, be written like this:

 JASS:

function ReallyNoLeakThisTime takes nothing returns nothing
 local location l = Location(0, 0)

 call RemoveLocation(l)
 set l = null
endfunction

An important note is that triggers don't need to be removed!

Since you want the triggers to actually do things, you never need to remove them.

Setting them to null, however, is good practice.

For example:

 JASS:

function InitTrig_<Name> takes nothing returns nothing
 local trigger t = CreateTrigger()
 ...
 set t = null
endfunction

Caveats

The size of arrays in Warcraft may be 8192, but index number 8191 is discarded and we like to assume that the last index is 8190. This is because in a loaded game, the 8191th index of an array does not store anything properly.

JNGP

JNGP stands for Jass New Gen Pack. It's an improved World Editor with many additions, many of them being geared towards writing Jass.

An example would be the globals block, which you don't have direct access to unless you use JNGP.

The main advantage of it for a Jass'er is the fact that it has a full reference of all the Jass language (types, functions, BJ functions, pre-defined variables).

If you are not using it, I highly suggest you to get it from here.

Closing

I hope this has been instructive enough as to teach how to use Jass.

For a reference of all the Jass language (if you don't have JNGP), you can look here.

Once you feel comfortable with Jass, you might want to check out vJass, which adds an extended keywords set to Jass that some people like better.

Good luck!


 Last edited: Aug 1, 2013

#### sniper_zero

#### sniper_zero

 Level 3

JoinedSep 9, 2009Messages658

Resources: 3

#### Resources
Spells 2Tutorials 1

I like this tutorial. Hope it gets approved.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

a question about returns.

 JASS:

function test takes nothing returns integer
local integer i = GetRandomInt(1,5)
return i
endfunction

 JASS:

call BJDebugMsg(I2S(test))

#### Geries

#### Geries

 Level 13

JoinedSep 13, 2010Messages550

Resources: 7

#### Resources
Models 6Packs 1

Chaosy said:

 a question about returns.

 JASS:

function test takes nothing returns integer
local integer i = GetRandomInt(1,5)
return i
endfunction

 JASS:

call BJDebugMsg(I2S(test))

Click to expand...

since test is a function, you can't use it as a variable(here), you need call BJDebugMsg(I2S(test())) ( notice the parentheses ) since you need to call it

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Chaosy said:

 a question about returns.

 JASS:

function test takes nothing returns integer
 local integer i = GetRandomInt(1,5)
 return i
endfunction

Click to expand...

should be this

 JASS:

function test takes nothing returns integer
 return GetRandomInt(1,5)
endfunction

i do like this tho. i will add a link in my jass tutorial to this. not sure if mine will be approved but hopefully mine and urs will be approved.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

@dimf

Just excuse GhostWolf. GhostWolf is GhostWolf

Nice tut GW

+ 4 rep

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

thanks for telling me!

I now find returns much more useful

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great tutorial. I read through it and I really like the approach, especially relating the calculator functions to JASS functions.

Plus, it is always nice to have a recent version.

Approved.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

If only this was made some months earlier, then it would have saved me some time maundering to learn about JASS. A good tutorial though.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

I finally remembered to add the link to my jass tutorial lol.

 You must log in or register to reply here.


### Similar threads

 JASS Tutorial


Daelin

Mar 20, 2005

Scripting Tutorials

Replies24Views38K

Nov 4, 2006
Daelin

 Coding an efficient knockback (vJass)


Silvenon

Oct 13, 2007

Scripting Tutorials

Replies39Views21K

Sep 6, 2008
Herman

 Beginning JASS Tutorial Series


wyrmlord

Feb 16, 2007

Scripting Tutorials
234

Replies181Views124K

Feb 18, 2025
Eastern Programmer yahoo

 JASS: A Better Understanding


Ghan_04

May 31, 2008

Scripting Tutorials

Replies15Views10K

Jun 20, 2009
Foopad

 Advanced JASS tips


Daelin

Dec 27, 2005

Scripting Tutorials

Replies0Views12K

Dec 27, 2005
Daelin

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