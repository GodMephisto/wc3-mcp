---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-loop-in-gui.313909/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Loop in GUI
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-loop-in-gui.313909/>
#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

How-to Loop In GUI!

Introduction

What is a Loop?

Simple Loop Example

Why a Loop?

Loop Interfering - Problem

Loop Interfering - Solution

1. Introductiontop

Welcome to this tutorial where we'll have a look at the loop functionality.​
We will learn about the loop's basic usage, and how they work together with counters.

Also, with the loop, a common, but a dangerous interfering risk comes along that should be definitly known!

2. What is a Loop?top

A loop helps you to perform some actions multiple times without duplicating your code.​
To achieve this, we will have 3 numbers (integers):

start-value

end-value

counter
... and with using our 3 numbers the loop works in following steps:

Counter starts with value of start-value

Check if counter is greater than the end-value. If yes, the loop will directly exit, if not, proceed to step 3

Perform all actions inside the loop ( loop run )

Increase Counter by +1

Go back to step 2
You only have to worry for step 1 and 3. Other steps happen automatically in the background. Most important for us is what happens in the loop run, so the actions inside.

Inside the loop run we always have access to our counter, the current index of the loop, which might become handy for many actions we plan to do.

3. Simple Loop Exampletop

Let's have a look at a practical example. We want to print all names from Player_1 to Player_6 on screen. The code might look like this:​

For each (Integer A) from 1 to 6, do (Actions)
Loop - Actions
Game - Display to (All players) the text: (Name of (Player((Integer A))))

Our start-value is 1

Our end-value is 6

As counter/current index we use IntegerA
We're giving IntegerA the start-value 1, and exit the loop when IntegerA is greater than 6. In each loop run we print the name of the player of IntegerA, the counter. Try it out yourself! And also think a brief moment what importance and role IntegerA, the counter variable plays in our loop.

4. Why a Loop?top

What our loop in the example above does, is actually the very same as:​

Game - Display to (All players) the text: (Name of (Player(1)))

Game - Display to (All players) the text: (Name of (Player(2)))

Game - Display to (All players) the text: (Name of (Player(3)))

Game - Display to (All players) the text: (Name of (Player(4)))

Game - Display to (All players) the text: (Name of (Player(5)))

Game - Display to (All players) the text: (Name of (Player(6)))
Then why even bother with the loop-construct instead of just writing 6x the "Game - Display" action?

The difference becomes more clear, once we want to change logics, like for example to print now all names from Player_7 to Player_16. Without a loop we have to touch all single action we created above. We or need to modify them, create new ones, or we need to remove them. But with the loop, all what we have to do is to change two numbers in the loop definition:

For each (Integer A) from 7 to 16, do (Actions)
Loop - Actions
Game - Display to (All players) the text: (Name of (Player((Integer A))))

... and the rest of the code stays exactly the same. We have not even touched the action inside the loop.

So, with using the loop you end up having more efficient code which is easier to maintain, and to read. You reduce code duplicates, and prevent making same changes, or even same mistakes at many places!

5. Loop Interfering - Problemtop

In our loop we usually expect our counter to be increased by +1 after each loop run. Always. We expect it to start at start-value, and to go all steps through upto end-value.​
But what happens if we unwillingly change the loop counter during a loop run to an arbitary value? - Our loop would break, or at least have undefined behaviour. Let's have a look at a code example:

We run following trigger, Trigger1, at map init, loop from 1-3, and we want to print "Loop_1" in each loop run.

Inside the actions we do also run some action to execute an other trigger. So in each loop run of Loop_1 we run Trigger2.

Trigger1
Events
Map initialization

Conditions

Actions
For each (Integer A) from 1 to 3, do (Actions)
Loop - Actions
Game - Display to (All players) the text: Loop_1

Trigger - Run Trigger2 <gen> (ignoring conditions)

In Trigger2 we again want to start a loop from 1-3, and print "Loop_2" on the screen.

Trigger2
Events

Conditions

Actions
For each (Integer A) from 1 to 3, do (Actions)
Loop - Actions
Game - Display to (All players) the text: Loop_2

Now guess the result? What are your expectations? The maker would probably expect to see 3x "Loop_1" on his screen. Additionally, after each text "Loop_1" there should be 3x a text "Loop_2" be displayed, too.

But it only prints the following:

 Loop_1

Loop_2

Loop_2

Loop_2​

Click to expand...

(indention is only cosmetics)

What has happened, and why has "Loop_1" only printed once?

What is going on is that both triggers share the same counter for the loop iteration. They both use IntegerA, which is just one variable.

When Loop_2 has finished the IntegerA variable already holds a value larger than the end-value of Loop_2 "4".

Trying to continue the "Loop_1" will just fail now because IntegerA is already 4, which is higher than the end-value of Loop_1!

So the Loop_1 has ended prematurely, and we had only one single loop run. This is definitly not wanted in this case, and it is a potential problem in all loops you create.

Also, this interfering problem does not only happen with the obvious "Trigger - Run" action, but it can occur inside other, less transparent scenarios, too! Let's see...

In general, each action in the loop that will cause an other trigger to run has a risk to break the loop, if the loop counter gets changed!

Here are examples of actions that may unwillingly fire other triggers:

Killing a unit

-> fires all trigger registered with event "Unit - A unit Dies"

Creating a unit

-> fires all triggers registered with event "Unit - A unit enters Region", if the unit gets created in respective region / Entire Map

Damaging a unit

-> fires all triggers where the unit is registered with event "A unit gets damaged", like for example in all DamageDetection systems!

Ordering a unit

-> fires all triggers with registered with event "Unit - A unit is issue Point Order/Target Order/..."

Kicking a player

-> fires all triggers registered with "Player - A Player leaves Game"
... so all of these actions can cause other triggers to run. And there are many more. You might check that if you look at events, and then you can see if your trigger could be fired by a certain action, or not. And if in any of those fired triggers uses a loop with the same loop-counter, then the interfering problem occurs

6. Loop Interfering - Solutiontop

How can we face the problem, when we just need actions that will cause other triggers to run, that themselves will need a loop counter, too?​
The solution is actually pretty simple! For all triggers that may fire other triggers you need an own integer variable as loop counter.

We need to switch from IntegerA/IntegerB-loop to a loop that now uses our own variable:

Let's look at our "Problem Example" from before, and let's also create now two new integer variables:

LoopCounter_Trigger1

LoopCounter_Trigger2
We need to change our code to take usage of the new variables:

Trigger1
Events
Map initialization

Conditions

Actions
For each (Integer LoopCounter_Trigger1) from 1 to 3, do (Actions)
Loop - Actions
Game - Display to (All players) the text: Loop_1

Trigger - Run Trigger2 <gen> (ignoring conditions)

Trigger2
Events

Conditions

Actions
For each (Integer LoopCounter_Trigger2) from 1 to 3, do (Actions)
Loop - Actions
Game - Display to (All players) the text: Loop_2

Now both triggers use now their own counter variable, so there won't occur any interfering problem. The both's loop-exit condition are completly independant, and do not share any variable.

As result, Loop_1 will not end prematurely and both loops work as expected. The final result looks like:

 Loop_1

Loop_2

Loop_2

Loop_2​
Loop_1

Loop_2

Loop_2

Loop_2​
Loop_1

Loop_2

Loop_2

Loop_2​

Click to expand...

... for each run in Loop_1, Trigger2 is fired and Loop2 will correctly run!


 Last edited: Oct 9, 2020

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

I'd say this tutorial fits the bill dealing with the why not IntegerA problem and how loop works in general.

#### HeuDevil

#### HeuDevil

 Level 2

JoinedApr 15, 2019Messages15

can you teach us how to make loop with abilities ? i mean a loop action in custom ability trigger ,

to move the target unit to one direction and creating a special effect on the target unit every second when the unit move.

i hope you can understand what i said my english is bad

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Something worth teaching is how to break a loop.

As in stopping it intentionally.

I did a quick ctrl + f and you did not seem to cover it.

But if I missed it - great.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

@HeuDevil I'm afraid how to make periodic movement spells and such would exceed the limit.

But I can recommand two other tutorials, focusing more about periodic spell handling:

MUI Spells Using Artificial Waits

Visualize: Dynamic Indexing
... and for how to move units I belive the Spell Section is a good place to look at, or the Help Forums.

@Chaosy it's planned to make a second tutorial with touching this topic to end a loop prematurely.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Absolutely top tier in terms of color usage and overall layout is also neat.

I have a strong personal preference about indenting after a title, but it's subjective so not required to change.

Example

This is a test

​
I do have a few nitpicks about the way you explain some aspects too.

IcemanBo said:

 A counter starts at a start-value, will be increased by +1 until it reaches the end-value, and the the loop will exit.


Click to expand...

I feel like this comes a bit of nowhere. You explain what a counter is but don't explain how it is connected to what you're currently explaining. And it is a few lines later you actually get to understand how it fits into the puzzle.

I would probably shuffle around a few lines here.

It's still understandable, I just think I can be better.

IcemanBo said:

 In our loop we usually expect our counter to be increased by +1 after each loop run. Always. We expect it to start at start-value, and to go all steps through upto end-value.

But what happens if we unwillingly change the loop counter during a loop run to an arbitary value? - Our loop would break, or at least have undefined behaviour.


Click to expand...

I think this can be explained in a more concise way.

Something along the lines of.

"In GUI we use a pre-defined variable which is used in ALL loops, meaning that if two loops run at the same time they will conflict."

I don't suggest we speak baby language but when the intent is to teach others I would say making sure we are easily understood is important.

Neither of these are a dealbreaker in my eyes, just take it as constructive criticism and perhaps keep it in mind if you happen to agree.

Can't see if you are currently active or not, but letting the tutorial remain here for a week or so in case you feel like doing a touchup before approval.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Thanks for the review.

I tried it using the indention. Not sure I also like it more, but why not trying.

Yes, the "A counter starts at ..." phrase is not very important there actually. I removed it, and slightly rewrote the text around.

I find the suggestion phrase with "conflicting loop" less accurate, I believe I prefer the current sentence.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Fine by me. It is nitpicking after all.

Approved.

 You must log in or register to reply here.


### Similar threads

 MUI Spells Using Artificial Waits


Tank-Commander

Sep 28, 2012

Trigger (GUI) Editor Tutorials

Replies32Views20K

Oct 16, 2015
BETABABY

Locked

[Trigger]
 Does a loop continue after a run subroutine?


Bond009

Sep 12, 2010

Triggers & Scripts

Replies3Views1K

Sep 13, 2010
GhostThruster

Locked

[Trigger]
 Real to Integer in loop


Hemlock

Feb 25, 2012

Triggers & Scripts

Replies6Views1K

Feb 26, 2012
Garfield1337

 Essentials Tutorial


Deuterium

Aug 22, 2009

Trigger (GUI) Editor Tutorials
2

Replies78Views26K

Dec 8, 2014
Justin Weaver

 {DEPRECATED / OUTDATED} Dynamic Values Storage


Kingz

Mar 25, 2009

Trigger (GUI) Editor Tutorials
2

Replies71Views22K

Aug 8, 2009
NFWar

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