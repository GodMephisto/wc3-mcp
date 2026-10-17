---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/general-spell-creation.236141/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# General Spell Creation
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/general-spell-creation.236141/>
#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

#
 General Spell Creation - by Chaosy

#
 Introduction

Hello, I thought it would be nice of me to upload a little guide/tutorial on how to create spells in general with triggering.

In almost every spell you use a config trigger, one cast trigger and one looping trigger so this tutorial will help you creating almost any spell.

Please notice I don't say that my way to trigger is the best one but it's the way I like the most.​

##
 How skilled do I need to be to understand and use this tutorial?

You need to understand the triggering in general, you need to know how to use actions events and so on.​

##
 Ready? Let's go then

So to start with we need a spell idea, I will simply create a spell that damages a unit by X every Y seconds for Z seconds.

So now that you got a spell you want to create we can start triggering, first off we need to create the setup trigger.

setup trigger
Events
Map initialization

Conditions

Actions
Hashtable - Create a hashtable

Set SS_hash = (Last created hashtable)

Set SS_damage[1] = 5.00

Set SS_damage[2] = 10.00

Set SS_damage[3] = 15.00

Set SS_time = 2.00

Set SS_duration = 10.00

Set SS_ability = Storm Bolt

You may notice that the name is SS_X. The SS stands for some spell, and we use that kind of variable name to make the name unique. So if your spell is named Fire Dagger, you should use FD_X.

So to explain the variables, we have 3 real variables: SS_time is how often we will damage the unit, SS_damage is how much damage the spell will deal per tick, and SS_duration is the duration of the spell obviously.

Note: The array stands for the level of the ability so the spell will deal 5 damage every 3 seconds at level 1 and 10 damage at level 2.

Next up is the cast trigger.

cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to SS_ability

Actions
Set SS_handle = (Target unit of ability being cast)

Hashtable - Save 0.00 as (Key SS_handle) of 1 in SS_hash

Hashtable - Save SS_duration as (Key SS_handle) of 2 in SS_hash

Hashtable - Save Handle Of(Triggering unit) as (Key SS_handle) of 3 in SS_hash

Hashtable - Save (Level of SS_ability for (Triggering unit)) as (Key SS_handle) of 4 in SS_hash

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(SS_group is empty) Equal to True

Then - Actions
Trigger - Turn on loop <gen>

Else - Actions

Unit Group - Add (Target unit of ability being cast) to SS_group

and then the loop trigger.

loop
Events
Time - Every 0.10 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in SS_group and do (Actions)
Loop - Actions
Set SS_handle = (Picked unit)

Set SS_counter = ((Load (Key SS_handle) of 1 from SS_hash) + 0.10)

Hashtable - Save SS_counter as (Key SS_handle) of 1 in SS_hash

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
SS_counter Greater than or equal to SS_time

Then - Actions
Hashtable - Save 0.00 as (Key SS_handle) of 1 in SS_hash

Unit - Cause (Load (Key SS_handle) of 3 in SS_hash) to damage (Picked unit), dealing SS_damage[(Load (Key SS_handle) of 4 from SS_hash)] damage of attack type Spells and damage type Normal

Hashtable - Save ((Load (Key SS_handle) of 2 from SS_hash) - SS_time) as (Key SS_handle) of 2 in SS_hash

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Load (Key SS_handle) of 2 from SS_hash) Less than or equal to 0.00

Then - Actions
Unit Group - Remove (Picked unit) from SS_group

Hashtable - Clear all child hashtables of child (Key SS_handle) in SS_hash

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(SS_group is empty) Equal to True

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

Else - Actions

Else - Actions

If you don't know how hashtables work this will indeed be very confusing to you, so I will try to explain how it works.

Hashtables are an action we can use to make the spell MUI. They save something and locks it with 2 keys. I will try explain in a odd way, but I think it is easier.

So let's say we have a bank, firstly we need to create the bank right? We should put this in the setup trigger:

Actions
Hashtable - Create a hashtable

Set SS_hash = (Last created hashtable)

Now, if we want to put something into the bank we need an account.

So for example, I can store 0.00 in the bank, but then I need to tell which account I want to put it in.

Actions
Hashtable - Save 0.00 as (Key (Target unit of ability being cast)) of 1 in SS_hash

this saves 0.00 in the account called (Key (Target unit of ability being cast)) + 1

now the 0.00 is successfully stored. And then we can unload it.

Set SS_counter = ((Load (Key (Picked unit)) of 1 from SS_hash) + 0.10)

If you have coded before this is basically a 2d array. For example.

Actions
Set array_one[1] = 64

Set array_one[2] = 86

Set array_one[3] = 33

Set array_two[1][1] = 34

Set array_two[1][2] = 55

Set array_two[1][3] = 75

That is basically what a hashtable is. If you study my code example I am sure you will understand.

If you don't understand me you should check out a tutorial:

A Complete Beginners Guide to Hashtables

Hashtables and MUI

​

#### Attachments

Hashtable Spell Example.w3x

 19 KB

 · Views: 309




 Last edited: Jul 21, 2021

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Don't destroy SS_Group

Store the damage when the ability is cast, load it in the looping trigger.

Currently the caster can learn the ability while the damage over time is running, and benefit from

higher ability level than what is was when it was cast

Turn the looping trigger off when the group is empty

The initialization trigger has the actions twice

You should swap the usage of parent and child keys, currently when you try to flush the data, it doesn't do that

Currently this looks like a short hashtable demonstration. You need to improve this guide a lot.

Pro tip: Wrap the trigger tags in [stable][/stable] tags.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

1. my bad I added that the last second because I saw a leak. Did not think.

2. indeed, will do

3. ok

4. no? I just didnt add the hasshtable in the first trigger to avoid confuseon.

5. should I flush the parent insted?

I guess it is, not my intention tho

will do

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

u should store picked unit to a temp variable for speed and efficiency.

in ur cast trigger u use target unit of ability being cast a lot. u should use the variable that it is stored into. better yet use a temp variable for key (Target unit of ability being cast))

for the last one u should also store key( picked unit) into a variable and use that.

on a side note: i dont like the way u config since it needs so many variables in order to work. But thats just an opinion. it is still a viable way.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

You should also teach them how to use an Indexed Array. If they keep making spells with hashtables,you are going to have a bad time.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

you should include in your tutorials how configurable works, not only damage and time but dummys and effects, also preloadings...


using an array in a non-dynamic way, such as [1][2][3] is no good practice unless you pick it randomly using GetRandomInt...


indexing is the best way for spells except when your storing something outside the casted spell, then use HT...

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

@variables

I could indeed add more variables but I want a demo code short and simple, variables ruin the readability too. Just personal opinion I guess.

@handle

I didnt want to confuse someone even more that is why I didnt add it. but I guess its needed.

@index

I dont use it because it completely destroys the reabability. and bad in which way? because you hit the hashtable limit? you got to think smart use 5 spells per hashtable or even more if you want to.

@setup

yes, I will include effect setup and explain. ty

and in which way is that bad? thats the only way I have seen.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

variables if named correctly are easier to read than tons of picked unit and other things.

indexing is a lot better IMO, but i dont believe there is a hashtable spell tutorial. So this might be a good idea. Although u should still encourage everyone to go for efficiency. So indexing is a much better solution in that respect.

Also teaching new ppl how to make spells more efficient by storing things into variables should always be done. y teach someone the wrong way just because its easier to read ? IMO its not easier to read.

This should stay a hashtable spell tutorial but u should note the pros and cons of hashtables vs indexing.

u could also link tutorials like mine to this and if this gets approved i will gladly link this in mine as an alternative to indexing for spells.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

as said just a personal opinion.will fix it.

there is actually a spell example in the hashtable and MUI tutorial but alternatives is allways good. People should go for whatever they like the most, however I should mention pros and cons.

could link your tut insted of explaining indexing since I cant work with it myself.

will do a big update on this as soon as possible but I got prom stuff today so will most likely be tomarrow.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Chaosy said:

 @variables


I could indeed add more variables but I want a demo code short and simple, variables ruin the readability too. Just personal opinion I guess.


@handle


I didnt want to confuse someone even more that is why I didnt add it. but I guess its needed.


@index


I dont use it because it completely destroys the reabability. and bad in which way? because you hit the hashtable limit? you got to think smart use 5 spells per hashtable or even more if you want to.


@setup


yes, I will include effect setup and explain. ty


and in which way is that bad? thats the only way I have seen.


Click to expand...


Im sure you know that you can only have 255 hashtables.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

thats why you use many spells with the same hashtable.

also 255 spells is much anyway, will not be a issue in most maps.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Chaosy said:

 thats why you use many spells with the same hashtable.


also 255 spells is much anyway, will not be a issue in most maps.


Click to expand...


definitely true.


one other thing u should tell them is how to properly store multiple spells into one hashtable. and tell them that as the hashtable gets bigger it gets much much slower than indexing with arrays.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

1 hashtable can be enough for spells of a whole map if there is no super duper hyper mega complex spell...

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Adiktuz said:

 1 hashtable can be enough for spells of a whole map if there is no super duper hyper mega complex spell...


Click to expand...


ya that is true but the decrease in speed is not good at all. especially when a lot of spells are running at the same time trying to read and write from that hashtable

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Please use the [stable][/stable] tags outside the trigger tags. It is stretching my PAGE!

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

ok, made a small update:

I now use stable tags

I added handle variables to the triggers

I fixed 2 buggs within the triggers

I now turn of the trigger when group is empty

Setup no longer got the actions twice

Will fix the indexing pros and cons thingy later

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

what about the inefficiency thing with using picked unit so many times, and key ( picked unit)

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

1. there is no longer a key picked unit I replaced them with handles

2. picked unit is used twice

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

u already store it here Set SS_handle = (Picked unit)

so u should use that variable instead of picked unit.

also key SS_handle is used quite a bit so that should also be stored into a temp variable.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I lost you somewhere mate.

1. yes it is stored once

2. I do use that..

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

It is stored yes but look in ur then actions. U have picked unit u should use the variable instead.

Also u should store the key of the picked unit into a variable as that will also make it faster.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

1. I dont find that

2. like set x = picked unit > set y = key handle (x)? In that case it wont work without a custom script. GUI does not support variables as keys

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

this works fine u just cant do it in JNGP.

Set tempInt = (Key (Picked unit))
also i meant this.

Unit - Cause (Load (Key SS_handle) of 3 in SS_hash) to damage (Picked unit), dealing SS_damage[(Level of YOUR_ABILITY for (Load (Key SS_handle) of 3 in SS_hash))] damage of attack type Spells and damage type Normal

Hashtable - Save ((Load (Key SS_handle) of 2 from SS_hash) - SS_time) as (Key (Picked unit)) of 2 in SS_hash
these 2 u still use picked unit and u use the variable u have already used.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

the damage action only got pickeded unit not handle picked unit, I dont use a variable for that since it's only used twice.

and about the key thingy I wont add it since most people use JNGP and they wont find that.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Picked unit is used twice but u already have picked unit stored. That's my point use the variable that already has picked unit in it.

Also with JNGP u can't use key of anything so u would have to use custom scripts for the saving and loading anyways.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

1. oh LOL that works as a handle and a unit variable?

2. yes, but it's a GUI tut so I cant use that. I normaly use jass hashtables.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Same here lol and yes that was the point I was trying to make lol. U should make a note for the ppl that don't use JNGP saying if u don't use it then u should store the key of picked unit into a variable.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I've been indecisive about what to do with this. It is fine the way it is, but I feel that it may not be as useful to show just one method. There are usually three common ways of making GUI spells:

(1) Hashtables

(2) Unit groups (add a unit to the group, iterate through group periodically)

(3) Indexing

I think for this to be a tutorial about "General Spell Creation", it should show those. It would make for a really nice tutorial. There are tutorials about using hashtables and indexing, but there isn't anything that shows all the methods and how to apply it to a general spell. It would be really useful for anyone looking to make submissions to the spells section.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I dont use indexing that's a little problem. I cant read the code/triggers if I use fugly indexing. Maybe if I simply show how to work with brides indexer and just link them to the everything a guier should know tut

also the (2) is kinda the method I use. I use a unitgroup and a hahstable

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I'd like to approve this. There is one issue though:

SS_Group is empty

That check is sort of expensive as far as checking whether a group is empty. It iterates through every unit just to see if it is empty. A better method would be to keep an integer variable to keep track of the number of instances. When a caster is added to the group, increment by 1 (instances = instances + 1), and when you remove the unit from the group (e.g. spell is finished) you just decrement by 1 (instances = instances - 1). If instances == 0, then turn off the trigger.

EDIT: P.S. I just noticed that the first trigger won't work. You add the unit before doing the "if" check to see if it is empty, so it'll never end up turning on the trigger.

EDIT2: You should also mention for JNGP users that they must use the custom script "GetHandleId()" for the key. Such as:

Custom script: set udg_SS_CasterID = GetHandleId(udg_SS_Caster)

Hashtable - Save SS_Caster as 3 of SS_CasterID ...
or whatever the functions are. Even if this is a GUI tutorial, you should at least mention it. Most JNGP users will get easily lost trying to use the "Key" function.

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Store the ability level when the spell is cast.

And I think tese types of spells could theoretically benefit from generating a unitque instance id for each cast. Now you are saving everything using the target unit as the key. A second cast on the unit while the first one is still on will override the first one. A lower level spell can override a higher level one. The spell can't stack. This is something to keep in mind.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I will see if I can update this soon.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Updated. Decided to clean stuff like this up.

I fixed what you said purge with the turn on trigger.

The ability level is stored into the hashtable.

I added another explaination about hashtables.

Uploaded test map

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

You could store the damage value directly.

The example is not the best since you can not make spells stack and a weaker level of the spell can override a stronger one.

For example one casts level 3 spell that does 100 damage over 10 seconds. One second later someone casts level 1 spell that does 60 damage over 6 seconds. The first hero just wasted his spell.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

hm, true indeed. However that isnt a issue in any map genre. Unless it's a very specific case. I mean even in ORPG's you arent able to pick the same hero/class. (not the ones I have tried atleast)

this can be fixed with a if-block in the cast trigger quite simply too

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I feel like the trigger could get a little bit more explanation. Maybe explain the "Key" function briefly (just to show that you are saving it to a unique slot, which is why it is MUI). Also, you should explain the unit-group loop a bit so that people understand what is going on. e.g. you can state that all the casters are added to a group, and then the loop goes through each caster and does whatever spell business it has to do for that caster's spell.

But I think this is still useful as a reference, especially for users who are getting into making spells. Approved.

P.S. I fixed the grammar/spelling a bit.

 You must log in or register to reply here.


### Similar threads

 How to create an artificial level-up system


frettory

Nov 22, 2012

Trigger (GUI) Editor Tutorials

Replies14Views9K

Mar 13, 2013
frettory

 A Complete Beginners Guide to Hashtables


Jazztastic

Jun 15, 2011

Trigger (GUI) Editor Tutorials
2

Replies57Views45K

Nov 12, 2024
PurgeandFire

 Charged Ability Tutorial


Almia

Aug 5, 2012

Trigger (GUI) Editor Tutorials

Replies12Views13K

Mar 4, 2016
Almia

 Hashtables and MUI


wyrmlord

Jun 26, 2009

Trigger (GUI) Editor Tutorials
345

Replies245Views126K

Jul 29, 2022
Chaosy

 Demystifying hashtables


Antares

May 25, 2024

Trigger (GUI) Editor Tutorials

Replies7Views3K

Oct 11, 2025
Antares

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