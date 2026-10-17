---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/visualize-dynamic-indexing.241896/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Visualize Dynamic Indexing
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/visualize-dynamic-indexing.241896/>
#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Contents

Introduction

MUI

Dynamic Indexing

Comparison

Credits

Introduction

Dynamic indexing is a technique used to ensure that your spell or code can be ran multiple times

without any MUI issues. At first, it is incredibly daunting to look at. This tutorial aims to show

it in a better light: de-complicating the complicated.

Before we go into dynamic indexing, you should understand its purpose. The main reason is to achieve

MUI, the goal that all non-failing spell-makers should hope to achieve.

​

MUI

MUI stands for Multiple Unit Instanceability. That is its only purpose: make a spell able to be

cast by multiple units (without undesired behavior).

What are these so-called MUI-issues that I speak of? Well, let's consider a simple timed spell that

transfers health every 0.1 seconds.​

Seems simple enough. You would start a timer when the spell starts to loop every 0.1 seconds.

In the loop, you would damage the target for 2 hit points, and give 2 hit points to the caster:

Siphon Life Cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Siphon Life

Actions
Set SL_Caster = (Triggering unit)

Set SL_Target = (Target unit of ability being cast)

Set SL_Counter = 0

Trigger - Turn on Siphon Life Loop <gen>

Siphon Life Loop
Events
Time - Every 0.10 seconds of game time

Conditions

Actions
Unit - Cause SL_Caster to damage SL_Target, dealing 2 damage of attack type Spells and damage type Normal

Unit - Set life of SL_Caster to ((Life of SL_Caster) + 2.00)

Set SL_Counter = (SL_Counter + 0.10)

If (All Conditions are True) then do (Then Actions) else do (Else actions)
If - Conditions
SL_Counter Greater than or equal to 3.00

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

This would work (ignoring the fact that it doesn't check if the unit is dead and junk).​

(The circle in the bottom right shows how much time has elapsed)

But what do you think would happen when another caster comes in?​

Now the original caster doesn't get any HP, and hogger only takes 2 damage per tick instead of 4

(2 + 2). Why? Look at the trigger above. When the spell is cast, SL_Caster is set to the casting

unit. Since variables can only point to one thing at a time, the original caster (green) is overwritten.

Not only that, but the target is overwritten, and the counter is reset to 0.

This illustrates why we need MUI. Without it, hogger doesn't experience as much pain as he should.​

Dynamic Indexing

There are many ways to achieve MUI. Dynamic indexing is just one way. It involves arrays and

one index per spell cast.

The first thing to do is to figure out what variables you need to store. In our case, we should store:

Caster: unit

Target: unit

Counter: real
Why? Because those are the variables we use in the periodic loop. Only save what you need to access

during the periodic. If you only need the data when the spell is first cast, you don't need to save it.

Instead of making single variables, we will convert them into arrays:​

Now, the goal of dynamic indexing is to have one index per spell cast. In the periodic trigger,

you will loop through every active spell cast (known as an instance of a spell). An instance

usually refers to one spell cast, and it will be associated with the index that is used. The first

instance of a spell will have an index of 1, the second will have an index of 2. When we talk about

multi-instanceability, we are basically saying that the spell should work with multiple-spell casts at

the same time.

For example, if you have 3 spell instances, this is how it will look:​

As you loop through the instances, you'll have a different caster and target to deal with. Here is some pseudo code:

For each (SL_Loop_Integer) from 1 to 3, do (Actions)
Loop - Actions
Unit - Cause SL_Caster[SL_Loop_Integer] to damage SL_Target[SL_Loop_Integer], dealing 2 damage of attack type Spells and damage type Normal

Unit - Set life of SL_Caster[SL_Loop_Integer] to ((Life of SL_Caster[SL_Loop_Integer]) + 2.00)

Set SL_Counter[SL_Loop_Integer] = (SL_Counter[SL_Loop_Integer] + 0.10)

Notice that each time the loop iterates, it refers to a new instance. When SL_Loop_Integer is 1,

it will refer to the first instance (see picture above). When SL_Loop_Integer is 2, it will refer to the

second instance. Same for the third.

Now, that code is specific. It loops through only 3 instances. But what if we want 5? Or 10? Or just 1?

The point of dynamic indexing is to make a general form that will work for all cases. How do we do that?

Simple, we must make an integer variable to keep track of the instances:

SL_Index (Name) - Integer (Type) - 0 (Initial Value)

So how do we go about setting this up? Well, first we have to worry about allocation. Allocation is

the process of getting a new index and assigning the variables we need to. Since we have an order of

1, 2, 3, etc.. we just need to increment the index once each time a spell is cast.

Consider the following scenario. The spell is cast by a dark ranger onto a hawk, a blood mage onto a pit lord,

a firelord onto a skeleton, and a sea witch onto a clockwerk goblin:

​

The above picture will show what it should look like on allocation. When the spell is cast by the

dark ranger, the index should be 1. SL_Caster[1] should be the dark ranger. SL_Target[1] should be

the hawk. SL_Counter[1] should be 0.0. So forth with the other indexes. Here is the trigger:

Siphon Life Cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Siphon Life

Actions
Set SL_Index = SL_Index + 1

Set SL_Caster[SL_Index] = (Triggering unit)

Set SL_Target[SL_Index] = (Target unit of ability being cast)

Set SL_Counter[SL_Index] = 0

If (All Conditions are True) then do (Then Actions) else do (Else actions)
If - Conditions
SL_Index Equal to 1

Then - Actions
Trigger - Turn on Siphon Life Loop <gen>

Else - Actions

This should work for it. SL_Index will increase by 1 for each instance, and the arrays will be set

for that particular instance. For index 1, the caster will be the dark ranger, the target will be the

hawk, and the counter will be 0. Same for the other instances, as shown in the picture above.

How about the actual looping? It is the same as we did far above, but instead of looping up to 3,

we just loop up to SL_Index. Why? If you think about it, SL_Index actually keeps track of the

number of spell instances. When the first spell is cast, the index is set to 1. When the second is cast,

the index is 2. When the fourth is cast, the index is 4. etc.

As for the last lines, it turns on the periodic loop when the first instance is made. You may wonder why

I don't turn it on initially–don't worry about it. That will be important later. Now we will move on to the looping.

(This trigger should be initially off)

Siphon Life Loop
Events
Time - Every 0.10 seconds of game time

Conditions

Actions
For each (SL_Loop_Integer) from 1 to SL_Index, do (Actions)
Loop - Actions
Unit - Cause SL_Caster[SL_Loop_Integer] to damage SL_Target[SL_Loop_Integer], dealing 2 damage of attack type Spells and damage type Normal

Unit - Set life of SL_Caster[SL_Loop_Integer] to ((Life of SL_Caster[SL_Loop_Integer]) + 2.00)

Set SL_Counter[SL_Loop_Integer] = (SL_Counter[SL_Loop_Integer] + 0.10)

If (All Conditions are True) then do (Then Actions) else do (Else actions)
If - Conditions
SL_Counter[SL_Loop_Integer] Greater than or equal to 3.00

Then - Actions

Else - Actions

Let's analyze the trigger. SL_Loop_Integer is just an integer variable used for looping, instead of

(Integer A). It loops up to SL_Index, basically going through each instance up until the last one.

SL_Caster[SL_Loop_Integer] will be the caster for that instance. Same for the other variables.

That trigger will loop through the instance and perform the actions for every spell instance.

You may be wondering why I left the "Then" actions empty. What do we do when the spell is over?

We must deallocate the instance. Otherwise, the spell will keep running on the finished instance

pointlessly. It will also ensure that SL_Index won't keep going up and up (max array index is 8191).

So how do we do that? We overwrite the instance-to-delete with the last instance, and then we set

the loop and SL_Index back 1 (so that it will run that instance). This is what it will look like:​

As you can see, the blood mage finished his life-sucking (for now). So now the sea witch should take his

spot. Our goal: get the sea witch to have index 2 (overwrite blood mage). Then reduce the index

and iterator (SL_Loop_Index) by 1. This is how it will look:

Siphon Life Loop
Events
Time - Every 0.10 seconds of game time

Conditions

Actions
For each (SL_Loop_Integer) from 1 to SL_Index, do (Actions)
Loop - Actions
Unit - Cause SL_Caster[SL_Loop_Integer] to damage SL_Target[SL_Loop_Integer], dealing 2 damage of attack type Spells and damage type Normal

Unit - Set life of SL_Caster[SL_Loop_Integer] to ((Life of SL_Caster[SL_Loop_Integer]) + 2.00)

Set SL_Counter[SL_Loop_Integer] = (SL_Counter[SL_Loop_Integer] + 0.10)

If (All Conditions are True) then do (Then Actions) else do (Else actions)
If - Conditions
SL_Counter[SL_Loop_Integer] Greater than or equal to 3.00

Then - Actions
Set SL_Caster[SL_Loop_Integer] = SL_Caster[SL_Index]

Set SL_Target[SL_Loop_Integer] = SL_Target[SL_Index]

Set SL_Counter[SL_Loop_Integer] = SL_Counter[SL_Index]

Set SL_Index = (SL_Index - 1)

Set SL_Loop_Integer = (SL_Loop_Integer - 1)

Else - Actions

Visually, this is how it would look overwriting the blood mage instance:​

That takes care of that. But what about reducing the index and the loop integer? Why do we do that?

Well, this is how it would look like right after we overwrite instance 2:

​

Now that we've overwritten slot 2, we must reduce SL_Index because we don't need instance 4.

SL_Index = SL_Index - 1, which is: 4 - 1 = 3. Here is what it will look like:​

Now the problem is that instance 2 is over! The sea witch never got her turn! Thus, we must set

SL_Loop_Integer back so that the sea witch's instance will run:​

​
Finally, we've finished! The spell should work correctly. If you cast another spell, the index will take

slot 4, which we have no issue with. Now the spell works, but there is one thing we can do for efficiency.

If there are no active instances, we should turn off the trigger. Otherwise it will loop needlessly. Here

are the final triggers:

Siphon Life Cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Siphon Life

Actions
Set SL_Index = SL_Index + 1

Set SL_Caster[SL_Index] = (Triggering unit)

Set SL_Target[SL_Index] = (Target unit of ability being cast)

Set SL_Counter[SL_Index] = 0

If (All Conditions are True) then do (Then Actions) else do (Else actions)
If - Conditions
SL_Index Equal to 1

Then - Actions
Trigger - Turn on Siphon Life Loop <gen>

Else - Actions

Siphon Life Loop
Events
Time - Every 0.10 seconds of game time

Conditions

Actions
For each (SL_Loop_Integer) from 1 to SL_Index, do (Actions)
Loop - Actions
Unit - Cause SL_Caster[SL_Loop_Integer] to damage SL_Target[SL_Loop_Integer], dealing 2 damage of attack type Spells and damage type Normal

Unit - Set life of SL_Caster[SL_Loop_Integer] to ((Life of SL_Caster[SL_Loop_Integer]) + 2.00)

Set SL_Counter[SL_Loop_Integer] = (SL_Counter[SL_Loop_Integer] + 0.10)

If (All Conditions are True) then do (Then Actions) else do (Else actions)
If - Conditions
SL_Counter[SL_Loop_Integer] Greater than or equal to 3.00

Then - Actions
Set SL_Caster[SL_Loop_Integer] = SL_Caster[SL_Index]

Set SL_Target[SL_Loop_Integer] = SL_Target[SL_Index]

Set SL_Counter[SL_Loop_Integer] = SL_Counter[SL_Index]

Set SL_Index = (SL_Index - 1)

Set SL_Loop_Integer = (SL_Loop_Integer - 1)

If (All Conditions are True) then do (Then Actions) else do (Else actions)
If - Conditions
SL_Index Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

Else - Actions

Done!

Comparison

A lot of people ask how dynamic indexing compares to other methods. The important thing to

remember that this is a matter of nanoseconds and can vary from situation to situation, but in general:

Allocation is often faster than saving all the items in a hashtable. (array set vs. hash set)

Iteration is faster than a hashtable/unit-group loop (array load vs. hash load)

Deallocation varies. Hashtables can sometimes be faster in deallocation since

dynamic indexing deallocation is O(n).

Versus a struct linked list, allocation is faster. Iteration is relatively the same.

Deallocation is almost always slower.
Note that this is just a comparison of the methods, a lot of other factors can alter the speed.

​

Credits

Hanky - popularized this method with this.

Sellenisko - Banshee Queen model.

Ghostwolf - Model viewer.

Classic BattleNet - A few images.

WoW - Hogger model.

#### Attachments

Dy.png

 6 KB

 · Views: 6,645



SiphonTooltip.png

 50.1 KB

 · Views: 4,509



SiphonHogger.png

 79.8 KB

 · Views: 4,363



SiphonHogger2.png

 129 KB

 · Views: 4,487



Variables.png

 9.6 KB

 · Views: 4,276



Instances.png

 28.6 KB

 · Views: 4,341



InstanceList.png

 114.8 KB

 · Views: 4,402



InstanceList2.png

 110.3 KB

 · Views: 4,476



InstanceList3.png

 113.1 KB

 · Views: 4,380



InstanceList4.png

 89.7 KB

 · Views: 4,410



InstanceList5.png

 86.4 KB

 · Views: 4,295



Deallocate.png

 59.9 KB

 · Views: 4,173




 Last edited: Sep 4, 2017

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

Nice tutorial Purge, I was looking for one. Thank you very much.


 Last edited: Sep 27, 2013

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Yep, I just left that out because I didn't want to explain it at that point in the tutorial.  No worries though, I have that check in the trigger later on.

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

Oh sorry, my fault, I haven't finished reading the tutorial yet I commented. I apologize.

EDIT:

on general details:

- So this indexing method also allows multiple spell instances for every unit, say for example, a footman can cast 5 indexed spells and not bug out.

- I know this is not a tutorial about instancing, but since it is closely related, and that is what this method accomplishes, why not add a small section to explain instancing. I think it's relevant to the tutorial since people who start making spells stumble on the problem of instancing their spells.

on the comparison:

- considering the method proposed, is the de-allocation O(n) because of the data swaps involved with the operation? If that's the case then the said time complexity could also be said of the vjass implementation of linked lists, since it internally uses arrays. I think it should be O(1) since the iteration operation is essential for the trigger actions, and the thing to consider on de-allocation would only be the swapping.


 Last edited: Sep 27, 2013

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

No problem. Anything that causes momentary confusion in a tutorial is worth considering, in my opinion. All comments are welcome.

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

I've added comments on my previous post, I don't want to clutter the tutorial with my replies.

#### pOke

#### pOke

 Level 13

JoinedMar 24, 2013Messages1,105

Resources: 1

#### Resources
Maps 1

Great tutorial, wish it had been around when I first had tried to learn this method .

2 quick questions--

Any advantage to using a real counter instead of an integer?

Is there a difference in decreasing the max index before the current index?

Set SL_Index = (SL_Index - 1)

Set SL_Loop_Integer = (SL_Loop_Integer - 1)

Set SL_Loop_Integer = (SL_Loop_Integer - 1)

Set SL_Index = (SL_Index - 1)

Edit: Thanks for the response purge, and wow those pictures make this tut look spectacular.


 Last edited: Sep 28, 2013

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

chobibo said:

 Oh sorry, my fault, I haven't finished reading the tutorial yet I commented. I apologize.

EDIT:

on general details:

- So this indexing method also allows multiple spell instances for every unit, say for example, a footman can cast 5 indexed spells and not bug out.

- I know this is not a tutorial about instancing, but since it is closely related, and that is what this method accomplishes, why not add a small section to explain instancing. I think it's relevant to the tutorial since people who start making spells stumble on the problem of instancing their spells.

on the comparison:

- considering the method proposed, is the de-allocation O(n) because of the data swaps involved with the operation? If that's the case then the said time complexity could also be said of the vjass implementation of linked lists, since it internally uses arrays. I think it should be O(1) since the iteration operation is essential for the trigger actions, and the thing to consider on de-allocation would only be the swapping.


Click to expand...

- Yes, 5 footman can cast the spell without bugging out. If you visualize the stack, it will always resolve itself, regardless of when you allocate/what instance you deallocate. Even if you deallocate the last instance (in which case, it is just setting itself = to itself), it will work.

- Alright. I'll try to add some more info on instancing. I don't know how long it'll be though. The way I refer to "instances" of a spell is just one spell cast, and the index represents that particular instance.

- Yes, it is O(n) because of the data swaps. If you have n pieces of data, then you must swap n-pieces of data in the deallocation. ex: you have 5 data vars: Caster, Target, Counter, Effect, Effect2

You would need to do 5 swaps:

Caster = Caster[max_index]

Target = Target[max_index]

... etc.

If you have 7, you have to do 7 swaps. So forth. This is all done so that you have a nice list of indexes:

1 - 2 - 3 - 4 - 5..

You'll never have an unordered list:

1 - 4 - 7 - 8 - 2..

However, with vJASS linked lists, you can have an unordered list, so we don't care about neatness. Linked lists just have pointers to the next and previous instance, so rather than looping through 1, 2, 3, 4, etc. we just go: next, next, next, next, next, 0 -> stop. Deallocation is just:

 JASS:

set this.next.prev = this.prev
set this.prev.next = this.next
// unlinks the instance
call this.deallocate() // free it to be used in allocation

Its speed doesn't rely on how much data it has, so it is O(1).

pOke said:

 Great tutorial, wish it had been around when I first had tried to learn this method .

2 quick questions--

Any advantage to using a real counter instead of an integer?

Is there a difference in decreasing the max index before the current index?

Set SL_Index = (SL_Index - 1)

Set SL_Loop_Integer = (SL_Loop_Integer - 1)

Set SL_Loop_Integer = (SL_Loop_Integer - 1)

Set SL_Index = (SL_Index - 1)

Click to expand...

- Nah, you can use a real or an integer. It is up to you. I like reals because it is easier to compare. If you increment it by the timer's period each time, you would just check if it is over 3.00 (3 seconds). If you used an integer, you would have to check if the integer is over 30 (30 ticks * 0.1 = 3).

- No difference. You can do it in whichever order you want because you are already done with the swapping.  Just don't place that code before the swapping, otherwise it'll swap the wrong indexes.

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

Oh yeah, I forgot that detail on linked lists. Thanks for the correction.


 tutorial; said:



 Now, the goal of dynamic indexing is to have one index per spell cast. In the periodic trigger,

you will loop through every active spell cast (known as an instance of a spell). So for example, if

you have 3 spell instances, this is how it will look:


Click to expand...

Thanks for considering my suggestion, this would clear up a lot of questions for beginners.

I've got no further questions, thanks for the tutorial, it's very useful.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Updated to fix the images. Sorry for those who had to read the huge wall of text without any images.  (sorry chobibo, pOke)

@chobibo: Yeah that is a perfect spot to put more info on instances. I'll make that update soon.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

make a tutorial on lists now

regular and linked

;D

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Yeah lol, I was thinking of doing that. I'll keep it in mind for the future.

edit: Added some follow-up info on "instances", in that area chobibo pointed out.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

shouldnt in this picture:

Target[2] = Target[4] instead of caster?

You dont want the poor Naga to drain life from herself do you

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

sheet you're right.

Thank you, I'll fix that. I was half watching a game when I made it so I must've not been paying attention.

edit: Fixed! Thank you!

#### FUJI

#### FUJI

 Level 21

JoinedAug 13, 2013Messages1,709

Resources: 16

#### Resources
Spells and Systems 16

Hmmmmm... I thought this method is Indexed Arrays xD because dynamic indexing is from Hanky Template . I'm too confused about them

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

jakeZinc said:

 Hmmmmm... I thought this method is Indexed Arrays xD because dynamic indexing is from Hanky Template . I'm too confused about them


Click to expand...

It very well might be named that. I have no clue. I know the method, but I don't follow much of the GUI-business so I don't know its name.

For all intents and purposes, you just need to know the technique. This is certainly different from hanky's method, but it performs a very similar task with a pretty similar method.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

amazing looking tutorial!


sadly this will increase the number of spells that use this method. Indexing is is just so fugly

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

With a little bit more complication it is possible to do less swapping when allocating.

Basically the way that structs are done behind the scenes.

You have a data structure(some amount of arrays)

The same indexes on arrays are always the same instance.

However, your indexed array will actually only point to an integer(the object ID in data structure, aka instance)

That way you only need to swap integers and not all the data behind them.

This is all from the top of my head, so it might not be explained in the best way. However, I see no reason why it wouldn't work.

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

You must be referring to a list and an instance allocator, Xonok.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

chobibo said:

 You must be referring to a list and an instance allocator, Xonok.


Click to expand...


I'm not sure how it's called, but in any case the main advantage is that instance IDs are kept separate from data(less swapping)

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Xonok said:

 With a little bit more complication it is possible to do less swapping when allocating.

Basically the way that structs are done behind the scenes.

You have a data structure(some amount of arrays)

The same indexes on arrays are always the same instance.

However, your indexed array will actually only point to an integer(the object ID in data structure, aka instance)

That way you only need to swap integers and not all the data behind them.

This is all from the top of my head, so it might not be explained in the best way. However, I see no reason why it wouldn't work.


Click to expand...

It is certainly possible. That is essentially emulating structs. The method in the tutorial is mostly designed for simplicity. I outlined the differences between it and a couple of other methods.

If we do have an external array to point to the instances, then you need a different allocator, because instances aren't in a fixated order--so doing the instance + 1 thing won't work. You'll need a list of freed objects (which is what structs do). That method is usually known as a struct-stack loop. I agree, it is often better, and perhaps I'll write a separate tutorial on different methods in the future.  I just started off with this since it is generally the easiest (IMO) to understand when you're new to MUI.

Thanks for the input!

@Chaosy: Thanks! And yeah, maybe so. It does end up looking pretty ugly. But once you understand the code behind it, it isn't too bad. It usually looks fugly because of all the underscores and junk.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

PurgeandFire said:

 It is certainly possible. That is essentially emulating structs. The method in the tutorial is mostly designed for simplicity. I outlined the differences between it and a couple of other methods.


If we do have an external array to point to the instances, then you need a different allocator, because instances aren't in a fixated order--so doing the instance + 1 thing won't work. You'll need a list of freed objects (which is what structs do). That method is usually known as a struct-stack loop. I agree, it is often better, and perhaps I'll write a separate tutorial on different methods in the future.  I just started off with this since it is generally the easiest (IMO) to understand when you're new to MUI.


Thanks for the input!


@Chaosy: Thanks! And yeah, maybe so. It does end up looking pretty ugly. But once you understand the code behind it, it isn't too bad. It usually looks fugly because of all the underscores and junk.


Click to expand...


Well, for struct recycling I'd just loop from 0-8190 and start at 0 again.


Every time a new struct is created I would loop through the indexes until I find an empty slot.


How do I determine what slot is empty? Depends on situation. If for instance, the struct is about the properties of a unit, then I can just check if the unit actually exists anymore.


This would probably be cheap.

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

Using that method would be bad for index iteration, you'll be doing a lot of checking because if, for example, index at 0 is still used and you've managed to reach 8190 then you would need to check that slot, which means that you need to check every slot if it is being used.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

chobibo said:

 Using that method would be bad for index iteration, you'll be doing a lot of checking because if, for example, index at 0 is still used and you've managed to reach 8190 then you would need to check that slot, which means that you need to check every slot if it is being used.


Click to expand...

That's a very unlikely case.

Unless you actually have 8190 units in the map, then you'd usually only move the index checker by 1-5 every time you index something new.

I see how this problem could arise, but it's easy to fix by just not reseting the index checker to 0 every time.

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

Just go experiment on it, I've tried doing that 4 years ago. No point debating it here without actual tests.

EDIT: I forgot to mention that iterating through that kind of array is ugly, a lot of gaps, and uses more memory than the subject of this tutorial.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

chobibo said:

 Just go experiment on it, I've tried doing that 4 years ago. No point debating it here without actual tests.

EDIT: I forgot to mention that iterating through that kind of array is ugly, a lot of gaps, and uses more memory than the subject of this tutorial.


Click to expand...

Well, my original suggestion was simply keeping stuff separate. Much less overwriting.

Like, it's quite known in the greater programming world, that one shouldn't toss around all the data for every single thing that happens. Keep the data stored in a (central) database and only manipulate references.

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

Yeah, that's why I told you to experiment on it, I found it too troublesome when I tested it. If you allowed indices to be empty then you'll be facing an iteration that grows every time a new index is introduced, unlike this method that keeps the iteration precisely to how many active instances are in the game.

The issue in speed of array writes is defeated by the taxing iteration you have to perform on fragmented data.

EDIT: Again, I encourage you to test it.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

If we have "Linked List For Dummies", that will be great.


+ 4 rep


As always, you amazed me Purgey-poo

#### Sunchips

#### Sunchips

 Level 30

JoinedJul 23, 2009Messages1,028

Resources: 125

#### Resources
Models 110Icons 8Packs 7

I can't sweat it how helpful this was. I had never been able to arse myself to read up on indexing and multi instancing but this guide was so compact yet visual and clear. You rock at explaining. +Rep for you!


Edit: I already rep'd you recently so have a handful of imagination rep instead

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Sunchips said:

 I can't sweat it how helpful this was. I had never been able to arse myself to read up on indexing and multi instancing but this guide was so compact yet visual and clear. You rock at explaining. +Rep for you!


Edit: I already rep'd you recently so have a handful of imagination rep instead

Click to expand...


Thanks, I'm glad it helped.  And yay for imagination rep, it is the best!

#### Deleted member 219079

D

#### Deleted member 219079

Wow, this tutorial really helped me to understand the mechanic of this particular indexing method  Really good work! And the pictures are awesome as well

#### NightStalker

#### NightStalker

 Level 19

JoinedApr 21, 2013Messages1,194

Resources: 5

#### Resources
Icons 4Spells and Systems 1

If i get it right the for loop is used once every time am i right because loopInteger from 1 to SL_Index is a one shot loop. Then it is triggered via the every .1 seconds of game event?

EDIT: Oh wow i get it now thats why it makes this spell mui okay

Thanks for the tutorial


 Last edited: Jan 26, 2014

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

NightStalker06698 said:

 If i get it right the for loop is used once every time am i right because loopInteger from 1 to SL_Index is a one shot loop. Then it is triggered via the every .1 seconds of game event?


EDIT: Oh wow i get it now thats why it makes this spell mui okay


Thanks for the tutorial

Click to expand...


Yes. That should be correct. And I'm glad you liked it.

#### NeosDany

#### NeosDany

 Level 1

JoinedJun 13, 2008Messages132

Resources: 2

#### Resources
Maps 2

And what if the channeling unit stops channeling, I should create another trigger but what is next?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Mapas said:

 And what if the channeling unit stops channeling, I should create another trigger but what is next?


Click to expand...


Well, for a spell that requires the unit to be channeling, it may be easier to add an if/then/else to check if the unit still has the spell order. If it doesn't have that order anymore, then you can end that instance (just as you would normally end it).

#### cboy123

#### cboy123

 Level 11

JoinedAug 1, 2009Messages714

Resources: 11

#### Resources
Icons 7Maps 3Spells 1

https://www.hiveworkshop.com/forums...ne-98/help-dynamic-indexing-unitgroup-248431/

Do you mind helping me with this problem...

Just.. one... tiny... detail to my code....

#### Geshishouhu

#### Geshishouhu

 Level 11

JoinedOct 11, 2012Messages711

Resources: 2

#### Resources
Spells 2

Thanks for this tutorial, it has helped me a lot. But I have a questions with your dynamic indexing method:

Which one is more efficient, your method or using a hashtable to make a spell MUI? Such as:

 JASS:

call SaveUnitHandle(hash,GetHandleId(t),0,u)

 Because for your method, I need to create global variables for each spell, right? Is that problematic for a map which has lots of spells? I also know that the efficiency of a hashtable decreases as it stores more data. So which one is better? Thanks.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

@Geshishouhu: The difference is minimal, but using dynamic indexing and one timer is generally better. In JASS, globals are so easy to create that this method is usually a lot cleaner than using hashtables. Or you can even combine the two: you would still have the allocation, but instead of running on a "global" timer, you would create your own and save the index under the ID of the timer. (at that point, you are pretty much simulating structs, so it would be better to just use structs directly)

Arrays are faster than hashtables overall. But that doesn't mean you should ignore hashtables. Arrays are usually cleaner, which is why I prefer them (especially in JASS). But there are still ways to make use of a hashtable. Perhaps I'll write a guide at some point on the different methods for making a spell in vJASS, since most of the present tutorials don't cover the modern methods. But idk.

#### Ceday

#### Ceday

 Level 12

JoinedFeb 22, 2010Messages1,115

Thank you so much, you can't guess how much I needed something like that.

#### Geshishouhu

#### Geshishouhu

 Level 11

JoinedOct 11, 2012Messages711

Resources: 2

#### Resources
Spells 2

PurgeandFire said:

 @Geshishouhu: The difference is minimal, but using dynamic indexing and one timer is generally better. In JASS, globals are so easy to create that this method is usually a lot cleaner than using hashtables. Or you can even combine the two: you would still have the allocation, but instead of running on a "global" timer, you would create your own and save the index under the ID of the timer. (at that point, you are pretty much simulating structs, so it would be better to just use structs directly)

Arrays are faster than hashtables overall. But that doesn't mean you should ignore hashtables. Arrays are usually cleaner, which is why I prefer them (especially in JASS). But there are still ways to make use of a hashtable. Perhaps I'll write a guide at some point on the different methods for making a spell in vJASS, since most of the present tutorials don't cover the modern methods. But idk.


Click to expand...

Thanks a lot, PnF. I am also looking forward to such a tutorial about vJass, it is really needed by noobs like me.

#### jonhysone

#### jonhysone

 Level 15

JoinedOct 29, 2012Messages1,474

Resources: 7

#### Resources
Maps 3Spells 3Tutorials 1

Purge you rock , thanks

I got a question bro, I am having a trouble with controlling a type of dummies, and make them stop when they reach a selected distance... After two units cast the same spell, this happens: First Unit casts fireball spray, and all balls reach such distance and explode ( Normal ), second unit casts fireball spray, they explode normally... when both cast eventually ( first one casts then 1 second , the 2nd unit casts), when fireballs explode, the second fireballs don't explode, they be stuck... ?? I am using Distance counter, what would be the problem?

( If I focus a bit I would know the problem but surely I will face problems with multi-projectiles and eye-candy spells  in the future, making them MUI by indexing... I usually use Unit Groups and Players Group to make them MPI MUI, I also use hashtables, but indexing makes it screwed)

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

jonhysone said:

 Purge you rock , thanks


I got a question bro, I am having a trouble with controlling a type of dummies, and make them stop when they reach a selected distance... After two units cast the same spell, this happens: First Unit casts fireball spray, and all balls reach such distance and explode ( Normal ), second unit casts fireball spray, they explode normally... when both cast eventually ( first one casts then 1 second , the 2nd unit casts), when fireballs explode, the second fireballs don't explode, they be stuck... ?? I am using Distance counter, what would be the problem?


( If I focus a bit I would know the problem but surely I will face problems with multi-projectiles and eye-candy spells  in the future, making them MUI by indexing... I usually use Unit Groups and Players Group to make them MPI MUI, I also use hashtables, but indexing makes it screwed)


Click to expand...


I would have to see the code to fix it. It sounds like a normal MUI problem.

#### jonhysone

#### jonhysone

 Level 15

JoinedOct 29, 2012Messages1,474

Resources: 7

#### Resources
Maps 3Spells 3Tutorials 1

It's a normal projectile system that launches 'X' fireballs for each player, after some distance they explode.

I've fixed the problem that some stuck and some explode... The problem now, that both can't cast at once, the first caster's fireballs launch, after 0.5 sec, the other one casts but fireballs never move... Everyone should cast in different time :'( I wish it's obvious. (I think the fireballs aren't being added to the fireballs group, I did game-text detection)

Fire Spray Start
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Fire Spraying

Actions
-------- ==== Storing ==== --------

Set FS_Index = (FS_Index + 1)

Set FS_Caster[FS_Index] = (Triggering unit)

Unit - Set the custom value of FS_Caster[FS_Index] to FS_Index

Set FS_CasterLocation = (Position of FS_Caster[FS_Index])

Set FS_PointTarget = (Target point of ability being cast)

Set FS_Level[FS_Index] = (Level of Fire Spraying for FS_Caster[FS_Index])

-------- ==== Configurables ==== --------

Set FS_Range[FS_Index] = ((75.00 x (Real(FS_Level[FS_Index]))) + 400.00)

Set FS_Damage[FS_Index] = ((Real(FS_Level[FS_Index])) x 100.00)

Set FS_Number[FS_Index] = (3 + FS_Level[FS_Index])

Set FS_Speed[FS_Index] = (((Real(FS_Level[FS_Index])) x 3.00) + 20.00)

Set FS_MinMax_Angle[FS_Index] = ((Real(FS_Level[FS_Index])) + 15.00)

-------- ==== Creating Dummies ==== --------

Set FS_DividedAngle[FS_Index] = ((FS_MinMax_Angle[FS_Index] x 2.00) / (Real(FS_Number[FS_Index])))

Set FS_StartingAngle[FS_Index] = ((Facing of FS_Caster[FS_Index]) - FS_MinMax_Angle[FS_Index])

Set FS_DistancePassed[FS_Index] = 0.00

For each (Integer A) from 1 to FS_Number[FS_Index], do (Actions)
Loop - Actions
Set FS_BallSpawnPoint = (FS_CasterLocation offset by 50.00 towards FS_StartingAngle[FS_Index] degrees)

Unit - Create 1 Fireball for (Owner of FS_Caster[FS_Index]) at FS_BallSpawnPoint facing FS_StartingAngle[FS_Index] degrees

Unit Group - Add (Last created unit) to FS_FireGroup[FS_Index]

Custom script: call RemoveLocation(udg_FS_BallSpawnPoint)

Set FS_StartingAngle[FS_Index] = (FS_StartingAngle[FS_Index] + FS_DividedAngle[FS_Index])

Game - Display to (All players) the text: (String((Number of units in FS_FireGroup[FS_Index])))

Set FS_DividedAngle[FS_Index] = 0.00

Set FS_StartingAngle[FS_Index] = 0.00

-------- ==== Starting Spell ==== --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
FS_Index Equal to 1

Then - Actions
Trigger - Turn on Fire Spray Loop <gen>

Else - Actions

=============================

=============================

=============================

Fire Spray Loop
Events
Time - Every 0.05 seconds of game time

Conditions

Actions
For each (Integer FS_MUI) from 1 to FS_Index, do (Actions)
Loop - Actions
Unit Group - Pick every unit in FS_FireGroup[FS_MUI] and do (Actions)
Loop - Actions
Set FS_PickedFB = (Picked unit)

Set FS_CurrentPoint = (Position of FS_PickedFB)

Set FS_NextPoint = (FS_CurrentPoint offset by FS_Speed[FS_MUI] towards (Facing of FS_PickedFB) degrees)

Unit - Move FS_PickedFB instantly to FS_NextPoint

Custom script: call RemoveLocation(udg_FS_CurrentPoint)

Set FS_DistancePassed[FS_MUI] = (FS_DistancePassed[FS_MUI] + (FS_Speed[FS_MUI] / (Real(FS_Number[FS_MUI]))))

Set FS_EnemiesAround = (Units within 100.00 of FS_NextPoint matching (((Matching unit) belongs to an enemy of (Owner of FS_PickedFB)) Equal to True))

Unit Group - Pick every unit in FS_EnemiesAround and do (Actions)
Loop - Actions
Unit - Cause FS_Caster[FS_MUI] to damage (Picked unit), dealing FS_Damage[FS_MUI] damage of attack type Spells and damage type Fire

Custom script: call DestroyGroup(udg_FS_EnemiesAround)

Custom script: call RemoveLocation(udg_FS_NextPoint)

Set FS_PickedFB = No unit

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
FS_DistancePassed[FS_MUI] Greater than or equal to FS_Range[FS_MUI]

Then - Actions
Set FS_Index = (FS_Index - 1)

Unit Group - Pick every unit in FS_FireGroup[FS_MUI] and do (Actions)
Loop - Actions
Special Effect - Create a special effect at (Position of (Picked unit)) using Abilities\Spells\Human\MarkOfChaos\MarkOfChaosTarget.mdl

Unit Group - Remove (Picked unit) from FS_FireGroup[FS_MUI]

Unit - Remove (Picked unit) from the game

Set FS_DistancePassed[FS_MUI] = 0.00

Set FS_Range[FS_MUI] = 0.00

Set FS_Damage[FS_MUI] = 0.00

Set FS_Number[FS_MUI] = 0

Set FS_Speed[FS_MUI] = 0.00

Set FS_MinMax_Angle[FS_MUI] = 0.00

Game - Display to (All players) the text: (String((Number of units in FS_FireGroup[FS_MUI])))

Set FS_Caster[FS_MUI] = No unit

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
FS_Index Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

Else - Actions

Thanks in advance purgy

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Two things:

(1) Make sure the size of FS_FireGroup (in the variables editor) has a sufficient size so that they'll be properly created on initialization and ready for use.

(2) Your deallocation should be different. You're decrementing FS_Index, but you aren't moving the other data down. So let's say you have 3 instances going on:

1 -> 2 -> 3; FS_Index = 3

When you deallocate, you end up zeroing out all the data for "2", but you don't reassign the data:

1 -> 0 -> 3; FS_Index = 2

So you end up with a gap in the data. The next time you loop, you'll loop through the instance 1 and an instance that just has 0 data in it (since FS_Index is 2), but you won't loop through "3". That is why you have to reassign the data on deallocation. It should look like this instead:

1 -> 3; FS_Index = 2

So to do that (move the FS_Index decrementation below this, as shown below):

Set FS_DistancePassed[FS_MUI] = FS_DistancePassed[FS_Index]

Set FS_Range[FS_MUI] = FS_Range[FS_Index]

Set FS_Damage[FS_MUI] = FS_Damage[FS_Index]

Set FS_Number[FS_MUI] = FS_Number[FS_Index]

Set FS_Speed[FS_MUI] = FS_Speed[FS_Index]

Set FS_MinMax_Angle[FS_MUI] = FS_MinMax_Angle[FS_Index]

Set FS_Caster[FS_MUI] = FS_Caster[FS_Index]

Set FS_Index = FS_Index - 1

Set FS_MUI = FS_MUI - 1
Hopefully that should fix the issues.

#### jonhysone

#### jonhysone

 Level 15

JoinedOct 29, 2012Messages1,474

Resources: 7

#### Resources
Maps 3Spells 3Tutorials 1

Thanks for the replay

Should I decrease FS_MUI ?! Because I never increased it or decreased :'O

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Yeah you should decrease it when you deallocate an instance. When you deallocate an instance (e.g. the blood elf in this picture), you are copying the last object into the blood elf's position:

And then you decrease "SL_Index" so that you won't have two copies of the same instance (think of it as removing the old instance):

But the issue is that we already iterated through the blood elf's index number, but we haven't done things for the naga yet. So we have to set the loop back (FS_MUI - 1) so that we will run it properly for the naga:

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

You could also store the indicies to be recyxled and after the loop you recycle things. Just sayin

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

edo494 said:

 You could also store the indicies to be recyxled and after the loop you recycle things. Just sayin


Click to expand...

Yep, that is possible. The reason I chose to do it the way in the tutorial (during the loop) was because I felt it was more intuitive to have distinct areas for handling the MUI aspect of the spell: one section will be the setup (in the setup trigger) and one section will be for deallocation (in the periodic trigger).

#### jonhysone

#### jonhysone

 Level 15

JoinedOct 29, 2012Messages1,474

Resources: 7

#### Resources
Maps 3Spells 3Tutorials 1

Thanks purge! I am gonna try everything and feedback

#### jonhysone

#### jonhysone

 Level 15

JoinedOct 29, 2012Messages1,474

Resources: 7

#### Resources
Maps 3Spells 3Tutorials 1

Oh yeah the problem was Unit Group's size, still when the first instance's fireballs explode, second instance's fireballs stop (neither explode nor complete their way), until another instance runs, it seems it doesn't go through instance "2" .. what's the problem in here?

EDIT: Nope it runs through all instances including 2, but it seems they all go to 0 when an instance ends... :/, it's like 2nd instance was stuck, it seems FS_Index becomes 0 after the first instance ends although 2nd instance is still running :O

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 Spell problems.


TheBigMetalHandInTheSky

Sep 4, 2017

Triggers & Scripts

Replies17Views3K

Sep 7, 2017
TheBigMetalHandInTheSky

 {DEPRECATED / OUTDATED} Dynamic Values Storage


Kingz

Mar 25, 2009

Trigger (GUI) Editor Tutorials
2

Replies71Views22K

Aug 8, 2009
NFWar

 MUI Spells Using Artificial Waits


Tank-Commander

Sep 28, 2012

Trigger (GUI) Editor Tutorials

Replies32Views20K

Oct 16, 2015
BETABABY

Locked

[Solved]
 Simple MUI spell


Naze

Mar 20, 2017

Triggers & Scripts

Replies7Views2K

Mar 22, 2017
Naze

 [GUI] Efficient Arrays - Multiinstanceabilty for Timers, Custom Values, and more


PurplePoot

Aug 27, 2008

Trigger (GUI) Editor Tutorials

Replies8Views10K

Aug 30, 2008
PurplePoot

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