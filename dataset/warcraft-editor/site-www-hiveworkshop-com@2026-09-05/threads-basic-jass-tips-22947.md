---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/basic-jass-tips.22947/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Basic JASS Tips
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/basic-jass-tips.22947/>
#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Basic JASS Tips


After making some research among people who had recently learnt JASS, I realized that many of them have made some common mistakes. This tutorial is meant to help all of you who do not know much about JASS, learn some new useful things.


1) Using Natives and usefulness of returned values


 Since many of you probably started with GUI, and then when they found themselves confident enough with it, gave a try to JASS. That is why, you may still use the “Convert to Custom Script” function, and it is something normal and understandable. However, many of those functions simply call (or return the value of) another function, which is supposed to do what they do. Functions which are not defined through other JASS functions, but are instead functions created by blizzard, and whose code cannot be read, are so-called Native functions. Based on these function, the programming language JASS is made.


 Now, let’s give an example of non-native function: CreateNUnitsAtLoc (the Create Units At Location GUI actions). Didn’t you find this function a little annoying since you always have to tell the number of units you want to create, even though you may want to create only a single unit? Well, that may be a reason why you want to use the native function based on which this function was created: CreateUnitAtLoc. The sole obvious difference between these two is the fact that the non-native (with NUnits) creates more than one unit at a time, while CreateUnitAtLoc creates a single unit.


 However, there are other non-native functions for which you won’t notice a difference between them and their native. An example of such function is UnitRemoveAbilityBJ. What this function do is simply return the value of the native UnitRemoveAbility. The sole difference is that the order in which the parameters are declared is switched, and nothing else. So what is the point of these non-natives?


 I tend to say “none” because at this hour, I don’t find their use. Since you are only at the beginning, it’s not that necessary to know how to replace non-natives with their natives, since in some cases it may be complicated. However, for those of you who attempt for example to replace CreateNUnitsAtLoc with CreateUnitAtLoc, you will realize that you can no longer use GetLastCreatedUnit to fetch unit created by the function.


 The fact that certain functions return a value, is extremely important. Some of the functions have only this use, to calculate something and return it, without doing anything else. But there are other function such as the creation functions (ex: CreateUnit, CreateCorpse, CreateTimerDialog etc). That’s the case of CreateUnitAtLoc as well.


 The not so obvious difference between this function and the non-native is that the non-native stores the created unit in a global variable (accessed through the function GetLastCreatedUnit()). But then, how can you fetch the created unit if not through that function?


 In fact, it’s very simple. I hope that you already know the fact that you can simply use functions which return a value, into an expression ,without using the keyword call. Example:

 JASS:

call SetUnitPositionLoc(GetLastCreatedUnit(), GetUnitLoc(GetTriggerUnit()))


 In this single line, I already used three functions which return a value, in an expression. Let’s analyze the function SetUnitPositionLoc. It takes as a parameter an unit and a location. The unit, is replaced with the value returned by the function GetLastCreatedUnit(). On the other hand, the location is replaced by the function GetUnitLoc which has its own unit parameter. For that parameter, once again, I used the function GetTriggerUnit().


 Even though it may sound confusing at first, you will get it. Try to think at the functions which return a value as of being the value itself. Let me give you a simple math example.


Example: Taking function f(x)=2x, and the function E(x)=f(x)+f(x+1)+f(x+2). If we simplify the expression we get:


E(x)=2x+2(x+1)+2(x+2)=2x+2x+2+2x+4=6x+6


And now we can replace parameter x, with values. That’s a simplified way to explain how JASS functions work, so you may base your algorithm on this principle.


 So, let’s return to the CreateUnitAtLoc problem. The function returns a unit value. But if we can’t get the unit created later, how can we directly fetch it into a variable? Simple, by assigning the function directly.


Example:









 JASS:

local unit u = CreateUnitAtLoc(GetTriggerPlayer(), ‘h000’, GetSpellTargetLoc(), 0.00)


 You could do this with CreateNUnitsAtLoc as well, but isn’t it simpler to use CreateUnitAtLoc? Hope this clearifies the problem of returned values and why the GetLastCreatedUnit(), GetLastCreatedEffectBJ() or any other such functions no longer work with the natives.


2) Simplified conditions


Antother concept, rather difficult for beginners is the concept of conditions, and inequalities (or equalities). I am sure that many of you didn’t work a lot with boolean values GUI. Boolean values are data types, which can store either a true or false value. I believe the two concepts are well-known.


Now, a comparision will return itself a boolean value.


Example: 1>2 is a false comparision.

1==1 is a true comparision.


 However, we can compare other stuff, and not only numbers. Numbers can also be compared to see if one is bigger than another, thing impossible with units or abilities for example. However, every two data of the same type (units, timers, abilities, buffs etc) can be compared to see if they are equal or not equal[/b].


Example:









 JASS:

GetTriggerUnit() != u //u being a unit variable


 So, where do we use comparisions? Well, of course that we do it in the IF structure (which executes the THEN instructions if the condition is true or the ELSE instructions, if it is false), loop (I will explain better conditional loop in the next chapter) or in trigger conditions. Now, I am extremely interested in the last case. Why? Because I have seen many useless IFS used when it came to conditions.


 For those who are not aware about the use of “AND” and “OR” keywords, I will explain them to you. You can use any of them between two conditions, and depending on the value returned by the two conditions, the final condition returns its own value. Check the chart below (elements on the table are separated by a “|”).

 Code:

 Condition A | Condition B | OR | AND |
 True | True | true | true |
 True | False | true | false |
 False | True | true | false |
 False | False | false | false |

 To simplify things, OR returns TRUE if atleast ONE condition is TRUE, and AND returns TRUE only if BOTH the conditions are TRUE.


 You can also do conditional chains. Check the code below:

 Code:

<CONDITION 1> AND <CONDITION 2> OR <CONDITION 3> AND <CONDITION 4> OR <CONDITION 5> OR <CONDITION 6>

 What the compiler does actually do is apply ANDs and ORs from left to right. First it evaluates <CONDITION 1> AND <CONDITION 2>, then <the result of this stuff (true or false of cause)> OR <CONDITION 3>; and so on, until the whole chain has been evaluated. In the end, it returns either a true or false value.


Note: You can also use paranthesis to force certain ANDs/ORs to be evaluated first. For the example above, we could use paranthesis like this:

 Code:

(<CONDITION 1> AND (<CONDITION 2> OR <CONDITION 3>)) AND (<CONDITION 4> OR <CONDITION 5> OR <CONDITION 6>)


It enters the first paranthesis, and searches for other deeper paranthesis. And it finds one! In this case, it evaluates <CONDITION 2> OR <CONDITION 3> first, and the result is evaluated with <CONDITION 1> using the AND keyword. Then the compiler goes to the second paranthesis and does the evaluations for <CONDITION 4> OR <CONDITION 5>, then the result with an OR with <CONDITION 6> and in the end, the whole stuff is evaluated with an AND with the first paranthesis.


Ok, since this is sooo complicated, I’m going to give you an example. I will replace the conditions only with true and false, to make things easier to comprehend. I will solve the problem itself, with equals between each step.

 Code:

(TRUE AND (TRUE OR FALSE)) AND (FALSE OR (TRUE AND FALSE)) =
(TRUE AND TRUE) AND (FALSE OR (TRUE AND FALSE)) =
TRUE AND (FALSE OR (TRUE AND FALSE)) =
TRUE AND (FALSE OR FALSE) =
TRUE AND FALSE =
FALSE


So in the end, the compiler returns FALSE. Now, don’t worry, you don’t have to do all this calculation by yourself. I just gave an example of how it works!


Note: Another important element is that when writing something like this:

 JASS:

if IsUnitDeadBJ(GetTriggerUnit()) then
//actions
endif


Normally, since it is a condition, I should normally compare it to something. BUT, the compiler also allows me not to compare functions that return a boolean value with something else. Why is that? I dunno. But in that case, it will automatically compare the value returned by the compiler with TRUE. In that case, you don’t have to write all the time ==TRUE.


Another function I would like to mention is the not(CONDITION) function. What it actually does is return the inverse value of the condition. So if the condition would be true, the function would return false, and vice-versa. Why is it useful? Because if in the case above, you could not compare functions or expressions that return a boolean value to TRUE, in this case, you can do it so that you don’t need to compare them with FALSE. Why is that?


Demonstration: Consider the following structure:

 JASS:

loop
 exitwhen IsUnitDeadBJ(GetTriggerUnit())
 call TriggerSleepAction(0.00)
endloop


We want to exit the loop when the unit is not dead (not something logic, but take it this way now…), without comparing it with something else. At this stage, the code makes the loop stop when the unit is dead. If the unit is dead, then the exitwhen condition is true, and the loop is over. If the unit is not dead, then the condition is false and the loop continues.


Now, we change the exitwhen condition like this:

 JASS:

exitwhen not(IsUnitDeadBJ(GetTriggerUnit()))

What happens now to the condition? The not function turns true to false, and false to true. Now, if the unit is dead, then the condition is not(TRUE) which is FALSE. The loop goes on. If the unit is not dead, then the condition becomes not(FALSE) and which is TRUE. Conclusion, the loop ends when the unit is NOT dead, so when the condition is not true!


 With this knowledge, we can easily evaluate conditions. The trick about triggers (and other functions) is that when applying the condition, you need to refer to a function which evaluates that condition. However, the compiler comes with an extremely complicated structure, if converting from GUI to JASS. A much easier method is:

 JASS:

function Condition takes nothing returns boolean
return GetSpellAbilityId()==’A000’ //replace ‘A000’ with the code of your function
endfunction


What does the compiler do? It analyzes the comparision. If the two are equal, it returns true and so, the condition passes. If not, it returns false and the trigger’s actions are not fired.


Moreover, when converting “Pick up Every Unit Matching Condition” you will get a lot of functions (a single function calling to more functions). Try to put all those functions together in a single function, chained through ANDs usually. Don’t be scared if you get a huge chain. If you find it difficult to evaluate yourself, copy-paste it in word (for example) and try to study each comparision.


3) Loops in JASS


If you have just started learning JASS (and have no experience with other programming languages), I am absolutely sure that loops are still something unknown to you. The idea of loop, is misinterpreted in GUI. There are two types of loopings (even three) in programming languages, and they should normally be all accessible. They are:


a) Loop with initial condition – When entering the loop, the compiler checks the initial condition. If that condition is true, then the compiler enters the loop. Now, the instructions inside the loop are repeated until the condition is false. If the condition will never be false, then we have an infinite cycle, which is a problem in programming… Each loop should end at a point, or else it will either lead to a freezing lag, or if a wait exists inside the loop, the compiler will not exit from the structure, maybe executing certain instructions even if they are not necessary. It is recommended to be VERY CAREFUL at the condition!


b) Loop with final condition – Just like loop with initial condition. The only difference is that the compiler does not check the condition when entering the loop. The instructions into the loop will be executed atleast once, even though the condition may not be true when entering the loop.


c) Loop with a known number of steps. This loop is executed for an exact number of steps, regardless of any other conditions. It is mostly used to avoid writing a thing ten times, or when the number of times you need to execute some instructions depends on a variable. This is the “FOR” Action from GUI.


Now, the only loop available in JASS is the first one. I perfectly understand why Blizzard chose it: Because it’s much easier to turn it into a loop with a known number of steps, and a loop with final condition is not that frequently used anyway, but if it is, you can assure before the loop that the compiler enters inside it atleast once.


 So, you will now have to learn the advantages of conditional loops, which are very numerous. First of all, these loops are very useful when you want to prevent any further actions from its thread until a certain condition is no longer true (buff spells for example). You have already met this in GUI under the name of “Wait for Condition”. However, when converting it to JASS it may look a little strange. That is why, here is a short general coding of it:

 JASS:

loop
 exitwhen CONDITION==false
 call TriggerSleepAction(0)
 endloop


Note: For those who do not know, even though TriggerSleepAction’s parameter may be extremely low (even 0.00), the wait duration will never go under 0.27, even though the value of the parameter is lower.


 You can also experiment with these loops in other situations. I will leave that up to your needs, but just remember that all the information about conditions I mentioned above, remains the same here.


 Now, how can we turn these loops into FORs? You will realize that if you want to do it with conversion from GUI, you will get a lot of instructions, and moreover, it may even use global variables, thing which will alter your MUI. So, how can we make them manually?


 Actually, the structure is really simple. We are just going to need an auxiliary numeric variable, which counts the number of loops. We start with that variable from 1, and after each loop, it increases with 1. We exit from the loop when the value of the variable is greater than the number of loops you want.


 Example:

 JASS:

local integer i = 1
loop
 exitwhen i>5
 call CreateUnitAtLoc(GetTriggerPlayer(), ‘h000’, PolarProjection(GetSpellTargetLoc(), 500.00, 90*i)
 set i = i+1
endloop


This code actually creates four points at the (Target Point of Ability Being Cast): One at north, one at east, one at south, and one at west. Since i is a variable, you can use it in expressions. And that’s what I did in this case. At the end of the loop, I just increased its value with 1. Do not forget to do that, or else you will once again get an infinite cycle.


That’s basically all I can teach you about loops!


4) Thread of execution


Ok, this is an extremely delicate subject, and may prove a little advanced, but I think that it is very important for all of you to understand it, if you haven’t already done that. Threads of execution are a very important part of the programming we are doing today, and so, of JASS.


 But what is a thread of execution? Well, it’s a little hard for me to give a definition, but I will try to explain them the best way I can, through examples. Consider more program running at the same time under your operating system (I’m more familiar with Windows): Firefox, Winamp, World Editor (believe me, I chose them randomly, I’m not suggesting you now to use Firefox or Winamp… I don’t even use Firefox). Each program runs independently, and they usually do not interract with each other, yet they all work at the same time: Winamp is playing the music, Firefox is still loading a page and you are working in World Editor. How is this possible? How can they work at the same time?


 The answer is obvious: each program runs under a separate thread of execution. A new thread, means having a series of instructions executed by the computer in paralel with other instructions (which of course, are situated into another threads). All threads lead to the operating system which controls them all. If the operating system falls, all threads fall. But if a single thread falls and the operating system remains intact, then no other thread will be affected (in the example above the problematic program is closed and you can still work into the others, without losing any information).


 Now, in Warcraft 3, threads are extremely important. Having so many units moving independently on the map, all kind of actions executed at the same time, and so on and so forth. So, why are threads so useful in JASS and how can you create a new thread?


 Well, first of all, since threads work separately, calling a TriggerSleepAction in a separate thread will not prevent the execution of any other instruction after it starts to be executed, from other threads. That is the main advantage: The fact that you can execute all kind of things at the same time.


 However, the disadvantage of threads is probably the fact that you cannot pass locals from a thread to another. You can keep instanceability unaffected by using gamecaches (but that is a more advanced subject and requires either Kattana’s system or Vexorian’s tables). So let’s see how can we use threads for now, and why are they so important.


 The very first way JASS creates a thread is when a trigger is “triggered” by the event and the conditions are true. Then, all the actions in that trigger are executed in a separate thread. That is why instanceability is possible, with the use of locals, because executing a trigger, does not interrupt other executions, because of threads.


 Another way to create threads is through timers. When a timer expires, it can be used to execute a function. That function will be executed in a separate thread. I usually use timers for spells which require constant movement of effect units or when it comes to other additional consequences of the spell (such as mana drain from the caster while the spell takes effect).


 Another way through which you can create threads, and the last one I will present you is through the function ForGroup. The function takes each unit from a group, creates a new thread, and executes the instructions from a function.


 Now, all these threads seem to have a common element: the use of Event-Response functions. Triggers almost always use the well-known GetTriggerUnit() to access the unit that fired the trigger, Timers’ will probably require GetExpiredTimer() because that’s the only event-response available in a thread created by a timer, and as for the ForGroup thread, you can access the unit for which the thread was created through the function GetEnumUnit().


Note: You cannot access Event-Responses from another thread. For example, if you have a timer in a trigger, for which you can access GetTriggerUnit(), even though the timer was created in that trigger, in the thread of the timer, you cannot access GetTriggerUnit() through the function.


5. Final Note


With this knowledge, you should be able to do more to your scripts then just converting from GUI to JASS and adding locals. You can now attempt to change conditions, add your own loops and create your own threads of execution. Understand that JASS is better than GUI for two important reasons:


- It gives you free hand when coming to editing the code, thus it allows you to use all Blizzard functions. Moreover you can organise your coding my better (which may be either a good or a bad thing).


- You have access to locals which allow you more than just MUI for spells. They can allow you to create thread of execution much easier than in GUI, with much more freedom, and allows you to manipulate objects much better.


So do not be afraid of the freedom you have in JASS. You will still be dependant to GUI, until you start learning more. However, you will realize that as you work in JASS and you use the functions, you will start to intuitively learn them. I recently discovered that working so much with JASS (over a year) has even allowed me to intuitively guess certain functions, because Blizzard had a logic when they gave names to the functions.


~Daelin

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

wow you took alot of time doing that.


im higher up than that ( now ) but i found it a good tutorial


a couple of things


-SetUnitPositionLoc in your example is leaking a location. Also, GetLastCreatedUnit() returns bj_lastCreatedUnit. It should be SetUnitPosition( bj_lastCreatedUnit, GetUnitX( GetTriggerUnit() ), GetUnitY( GetTriggerUnit() ) )


-Random Fact - if you use CreateNUnitsAtLoc( Number Greater Than 1 ) then it will create a group that, as far as i know, is impossible to access and therefore leaks


-IsUnitDeadBJ( whichUnit ) just checks if the unit life is <= 0. I suggest just comparing the integer instead, for speed reasons.


-when you mentioned the loops, loops A and B are both really part of blizzard's JASS, seeing as you are free to position the Exitwhens


-TriggerSleepAction is random even when greater than 0.27 seconds, only accurate to ~ .25 seconds.


-I hope this clarifies something. I was also wondering about BJs, and after looking at functions I saw a pattern. Single Parameter BJs are just put in because everything else is BJs.


The reason the others are just rearrangements( dont forget this is an assumption ) is because Natives must take Parameters in a certain order.


EX: Triggers, Forces, and Groups will come first respectively, while Boolexprs, booleans, and Code Funcs come last.


Integers and Reals come before single Handles


Reals come before integers


and so on...


If blizzard used the natives for, say, Unit - Issue order targeting a point would say this, because, even in GUI, Parameters are being taken in the order of the functions


Give a (string) order to (unit) to (unit) or something bizzare like that.


i hope that clarifies some things.


-Great Tutorial!

 You must log in or register to reply here.


### Similar threads

 JASS Tutorial


Daelin

Mar 20, 2005

Scripting Tutorials

Replies24Views38K

Nov 4, 2006
Daelin

 JASS: A Concise Introduction


Ghan_04

Feb 27, 2008

Scripting Tutorials

Replies19Views14K

Jun 20, 2009
Foopad

 Jass, or "What the func?"


GhostWolf

Jul 21, 2013

Scripting Tutorials

Replies9Views9K

Oct 1, 2013
deathismyfriend

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