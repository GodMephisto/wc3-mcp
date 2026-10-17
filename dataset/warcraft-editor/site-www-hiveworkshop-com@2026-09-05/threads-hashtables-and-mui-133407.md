---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/hashtables-and-mui.133407/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Hashtables and Mui
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/hashtables-and-mui.133407/>
#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Before starting this tutorial, you should be comfortable with using GUI. This tutorial was not made to target those just starting out with GUI and may be confusing for those that are.

##
 What is a hashtable?

For starters, it's a data structure capable of holding almost any kind of data in Wc3. It can hold integers, unit, special effects, and anything else you would ever need to store.

##
 How do we use it?

First thing is first, we need to create a hashtable in order to use it. While we're at it, we'll use a variable to keep track of it.

Init
Events
Map initialization

Conditions

Actions
Hashtable - Create a hashtable

Set hashTable = (Last created hashtable)

Now that we have a hashtable, we can use it in a way that's very similar to using an array. The difference? It requires 2 integers instead of 1 and can hold any type we desire. There's also no limit as to how big or small these integers can be. Here's an example:

Example
Events

Conditions

Actions
-------- Store the value --------

Hashtable - Save 5.00 as 0 of 0 in hashTable

-------- Load the value --------

Set LoadedValue = (Load 0 of 0 from hashTable)

-------- Will show the text: 5.00 --------

Game - Display to (All players) the text: (String(LoadedValue))

As you can see, a hashtable is a structure that lets you store things in it. You give it a value to store and 2 integer "keys" that will later be used to retrieve the object you stored. In this example, we could just as easily have used an integer, string, unit, floating text, etc. in place of the real value and they could have all be stored in the hashtable.

###
 Why are Hashtables Useful?

I know what you're thinking. "If hashtables are just giant arrays, why do we need them?" Hashtables by themselves are not terribly useful, but when we use them with Handle IDs they can actually be incredibly useful. They allow us to "attach" data to handles that can be retrieved later. We're going to cover this now.

###
 What is a Handle?

For those needing it, here's a simple overview of what a handle is. There are many types in Warcraft 3: integers, reals, strings, units, players, items, special effects, etc. A handle is any type that is not either an integer, real, boolean, or string. An example of some commonly used handles are: units, destructables, items, players, special effects, floating text, and locations (referred to as "points" in GUI). There are many, many more handles than the list I gave, but these are just some of the more commonly used ones.

###
 Handle IDs and why they're useful

This, although you might not realize it now, is incredibly useful when combined with Hashtables. With the new patch, you are now able to get the Handle ID (it's an integer) of things such as units. As far as you should be concerned, these IDs are unique to each handle. Now, as previously mentioned, hashtables require two integers when storing data inside of it. One of these integers could easily be the Handle ID of a unit that's part of a MUI spell.

Since you might still not understand its usefulness, here's an example of a spell that heals 10 life per second for 10 seconds:

While I'm at it, please use "Unit - A unit Starts the effect of an ability" instead of "Unit - A unit Begins casting an ability".

Start
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Heal

Actions
-------- Store the duration in the Hashtable under the unit's Handle ID --------

Hashtable - Save 10.00 as 0 of (Key (Target unit of ability being cast)) in hashTable

-------- Add the unit to the group of units that are being healed over time --------

Unit Group - Add (Target unit of ability being cast) to HealOverTimeUnits

Every second, this trigger goes through all the units being healed and heals them.

Healing
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in HealOverTimeUnits and do (Actions)
Loop - Actions
-------- Get the remaining time in seconds --------

Set RemainingTime = (Load 0 of (Key (Picked unit)) from hashTable)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
RemainingTime Greater than 0.00

Then - Actions
-------- Heal the unit and update the time remaining --------

Unit - Set life of (Picked unit) to ((Life of (Picked unit)) + 10.00)

Hashtable - Save (RemainingTime - 1.00) as 0 of (Key (Picked unit)) in hashTable

Else - Actions
-------- The healing is done, clean up --------

Unit Group - Remove (Picked unit) from HealOverTimeUnits

Hashtable - Clear all child hashtables of child (Key (Picked unit)) in hashTable

I hope you're able to understand all of this. The only thing you might be wondering about is the "Hashtable - Clear all child hashtables of child (Key (Picked unit)) in hashTable". When you're done using the hashTable for a specific unit, you need to clean up just like you would clean up memory leaks. This function cleans up all data stored with the Handle ID of the unit that was being healed.

This might be more work than using a bunch of waits inside a single trigger, but this way of doing things is completely MUI.

Now, another popular example. We're going to make a single target knockback spell that uses Hashtables. It will work just like the HealOverTime trigger because it will also use a group to hold all the units being knocked back. The difference is that more values will be stored in the hashTable and of course the unit will be knocked back instead of healed.

Here's another thing to keep in mind. Unless you handle your values carefully, spells that use the same hashtable can easily overwrite values in other spells. So, I recommend creating a hashtable global for each spell that needs to use it unless you know what you are doing and can prevent conflicts. Anyways, here are the knockback functions:

KnockbackTarget
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Knockback

Actions
-------- Just for configuration/readability --------

Set KnockbackAngle = (Angle from (Position of (Triggering unit)) to (Position of (Target unit of ability being cast)))

-------- Distance unit is knocked back per second --------

-------- .04 is how often the units are moved, so we multiply the distance by .04 --------

Set KnockbackDistance = (400.00 x 0.04)

-------- How long the unit is knocked back for --------

Set RemainingTime = 2.00

-------- Store the values --------

Hashtable - Save KnockbackDistance as 0 of (Key (Target unit of ability being cast)) in knockbackTable

Hashtable - Save KnockbackAngle as 1 of (Key (Target unit of ability being cast)) in knockbackTable

Hashtable - Save RemainingTime as 2 of (Key (Target unit of ability being cast)) in knockbackTable

Unit Group - Add (Target unit of ability being cast) to KnockbackUnits

This trigger moves all the units currently being knocked back.

KnockbackUnits
Events
Time - Every 0.04 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in KnockbackUnits and do (Actions)
Loop - Actions
-------- Load all necessary variables stored in the Hashtable --------

Set KnockbackDistance = (Load 0 of (Key (Picked unit)) from knockbackTable)

Set KnockbackAngle = (Load 1 of (Key (Picked unit)) from knockbackTable)

Set RemainingTime = (Load 2 of (Key (Picked unit)) from knockbackTable)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
RemainingTime Greater than 0.00

Then - Actions
-------- Move the unit and update time remaining --------

Unit - Move (Picked unit) instantly to ((Position of (Picked unit)) offset by KnockbackDistance towards KnockbackAngle degrees)

Hashtable - Save (RemainingTime - 0.04) as 2 of (Key (Picked unit)) in knockbackTable

Else - Actions
-------- Knockback is done, clean up --------

Hashtable - Clear all child hashtables of child (Key (Picked unit)) in knockbackTable

Unit Group - Remove (Picked unit) from KnockbackUnits

As you noticed, the usage of a unit group is very similar to the Heal example. What you should also notice is that a different key is used for each value be stored (0 for distance, 1 for angle, 2 for time remaining). It's important that each of these keys are different because otherwise the data would be overwritten.

While remembering each key in an example like this isn't tricky, sometimes with larger systems they can become mixed up. There's a handy function that will get a String ID of a string just like you can get the Handle ID of a unit. So instead of using 0 for distance, we could have gotten the String ID of "distance". Here's what the first trigger might look like if we used String IDs instead:

KnockbackTarget
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Knockback

Actions
-------- Just for configuration/readability --------

Set KnockbackAngle = (Angle from (Position of (Triggering unit)) to (Position of (Target unit of ability being cast)))

-------- Distance unit is knocked back per second --------

-------- .04 is how often the units are moved, so we multiply the distance by .04 --------

Set KnockbackDistance = (400.00 x 0.04)

-------- How long the unit is knocked back for --------

Set RemainingTime = 2.00

-------- Store the values --------

Hashtable - Save KnockbackDistance as (Key distance) of (Key (Target unit of ability being cast)) in knockbackTable

Hashtable - Save KnockbackAngle as (Key angle) of (Key (Target unit of ability being cast)) in knockbackTable

Hashtable - Save RemainingTime as (Key time) of (Key (Target unit of ability being cast)) in knockbackTable

Unit Group - Add (Target unit of ability being cast) to KnockbackUnits

The KnockbackUnits trigger would need to be updated to use the String IDs as well, but that should be straightforward enough that I shouldn't have to show it here. The example in the demo map uses String IDs, so you can look there for more information.

As a final note, these triggers are merely examples. As you may or may have not noticed, the knockback trigger would require memory leak fixing since it leaks 25 locations each second for every unit that is being knocked backward. I intentionally left the triggers how they are so they would be easier to understand. Those who wish to use these kinds of triggers in their maps should make sure to fix any memory leaks that were not cleaned up in the examples.

#### Attachments

Hashtable Tutorial.w3x

 27.3 KB

 · Views: 3,349




 Last edited: Aug 31, 2009

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Seems fairly straightforward, although I'd like a few GUI users to have a look at this and see if they get it.

#### dansaDisco

#### dansaDisco

 Level 16

JoinedJun 25, 2008Messages1,039

Resources: 22

#### Resources
Models 7Icons 13Skins 2

I understand it perfectly, thank you very much for this tutorial.

I reckon hashtables and handles will be extremely useful when the new patch is finally stable

And, in contrast to many other tutorials, I only had to read this once to get how it works

Awesome job wyrmlord!

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

This ready to be approved?

#### dansaDisco

#### dansaDisco

 Level 16

JoinedJun 25, 2008Messages1,039

Resources: 22

#### Resources
Models 7Icons 13Skins 2

Pyritie said:

 This ready to be approved?


Click to expand...


I would say yes. I don't see anything wrong with it and it explains everything in a easy manor.

#### Boreal

B

#### Boreal

While I would have preferred more information on how hash tables work (which would probably ruin the tutorial for everyone else), I still think this is a excellent tutorial.

#### Kwah

#### Kwah

 Level 24

JoinedMay 9, 2007Messages3,561

Resources: 12

#### Resources
Icons 7Maps 3Tutorials 2

Py it's wyrm. Just approve it.

#### redmarine

#### redmarine

 Level 19

JoinedSep 4, 2007Messages2,826

Resources: 1

#### Resources
Maps 1

I somewhat understand the concept. It's kinda unclear but once I've actually try it out I believe I should fully understand it.


 Last edited: Jul 5, 2009

#### Reaper2008

#### Reaper2008

 Level 12

JoinedJul 27, 2008Messages1,181

Resources: 1

#### Resources
Spells 1

Nice tut!

#### Rmx

#### Rmx

 Level 20

JoinedAug 27, 2007Messages1,164

Resources: 37

#### Resources
Icons 16Maps 1Spells and Systems 20

Easy to understand but in big triggers and complicated ones...


Hashtable will be extremely complicated, but some easy triggering and jumps slides ext..


Hashtable is F T W ( for the win ) ^^

#### Lightstalker

#### Lightstalker

 Level 7

JoinedJul 8, 2008Messages150

Resources: 2

#### Resources
Maps 1Spells and Systems 1

I think it's a great tutorial but lacking a bit in information...


For example, if you save a value of 1, then save another with the same value, the previous will be overwritten.


Pretend a Paladin with "Heal Over Time" is fighting a Death Knight with "Damage Over Time" (the exact opposite of your spell). Now, the Paladin will cast "Heal Over Time" on himself and the duration will be saved of 0. Now, the Death Knight casts "Damage over Time" on the Paladin and the duration will be saved of "0". Won't the Paladin's spell stop healing him?


Also, do points and other things that leak need to be assigned to a variable and then destroyed, or is clearing the child hashtable enough?

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Good job, quite like this, I guess it was nice explained and well written.

#### Linaze

#### Linaze

 Level 24

JoinedFeb 28, 2007Messages3,478

Resources: 4

#### Resources
Models 1Tutorials 3

I must admit, I actually learned something new. As soon as the patch comes out, I'm gonna try this for sure.

However, you need to add a title ([h1][/h1]) and your headings must use the [h2], [h3] etc. tags.

#### Element of Water

#### Element of Water

 Level 18

JoinedAug 3, 2008Messages2,345

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Lightstalker said:

 I think it's a great tutorial but lacking a bit in information...

For example, if you save a value of 1, then save another with the same value, the previous will be overwritten.

Pretend a Paladin with "Heal Over Time" is fighting a Death Knight with "Damage Over Time" (the exact opposite of your spell). Now, the Paladin will cast "Heal Over Time" on himself and the duration will be saved of 0. Now, the Death Knight casts "Damage over Time" on the Paladin and the duration will be saved of "0". Won't the Paladin's spell stop healing him?

Also, do points and other things that leak need to be assigned to a variable and then destroyed, or is clearing the child hashtable enough?


Click to expand...

If you use the same hashtable for both spells then yes, it will bug, but if, as you should, you use different global hashtables for each spell, then there will be no problem.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Linaze said:

 I must admit, I actually learned something new. As soon as the patch comes out, I'm gonna try this for sure.


However, you need to add a title ([h1][/h1]) and your headings must use the [h2], [h3] etc. tags.


Click to expand...


Changed and approved

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Element of Water said:

 If you use the same hashtable for both spells then yes, it will bug, but if, as you should, you use different global hashtables for each spell, then there will be no problem.


Click to expand...

I will make sure this point is emphasized in the tutorial, thanks for pointing it out.

Lightstalker said:

 Also, do points and other things that leak need to be assigned to a variable and then destroyed, or is clearing the child hashtable enough?


Click to expand...

I would still assign the values to variables and destroy them manually just to be sure.

#### Lightstalker

#### Lightstalker

 Level 7

JoinedJul 8, 2008Messages150

Resources: 2

#### Resources
Maps 1Spells and Systems 1

And then what after I've created a global hashtables? If I use a spell that has a periodic event, how will I find that hashtable again, since there is no "Save hashtable" function? Do I have to create a hashtable array variable, and at the beginning of my action have an integer variable and add + 1 to it, and set it as the last created hashttable?


Example:


Create a Hashtable


If


MUICount Less than 1000


Then


Set MUICount = MUICount + 1


Else


Set MUICount = 0


Set HashT[MUICount] = Last Created Hashtable


Is that what I should do?

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

I'm extremely confused as to what you're trying to do. You should only need one hashtable per spell. The primary use of a hashtable is to "attach" data to handles. In my examples, I attached the information to units to keep track of how much more healing they needed and knockback information.


When using hashtables, you should not need to use global array variables as well, only the hashtable. If you can explain what you're trying to accomplish, I can try to point you in the right direction.

#### redmarine

#### redmarine

 Level 19

JoinedSep 4, 2007Messages2,826

Resources: 1

#### Resources
Maps 1

wyrmlord said:

 I'm extremely confused as to what you're trying to do. You should only need one hashtable per spell. The primary use of a hashtable is to "attach" data to handles. In my examples, I attached the information to units to keep track of how much more healing they needed and knockback information.

When using hashtables, you should not need to use global array variables as well, only the hashtable. If you can explain what you're trying to accomplish, I can try to point you in the right direction.


Click to expand...

Are hashtables also useful for storing multiple data for individual units in a group? By that I mean can they also contain references for those units?

#### SlayerII

#### SlayerII

 Level 9

JoinedAug 21, 2008Messages533

Resources: 3

#### Resources
Skins 1Maps 1Tutorials 1

hmm


could this mean i can use hashtables like 2d arrays? the one from vjass are bugged-.-

#### Element of Water

#### Element of Water

 Level 18

JoinedAug 3, 2008Messages2,345

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

The 2D arrays in vJass

a) are not bugged

b) are faster than hashtables

Hashtables can have an infinite number of indices, however, which is more than you can say for vJass 2D arrays.

#### SlayerII

#### SlayerII

 Level 9

JoinedAug 21, 2008Messages533

Resources: 3

#### Resources
Skins 1Maps 1Tutorials 1

So if i have a item table should i use a 2d array or a hastable(the array should save lvl and type of item exemple a 13(lvl) 3(type) gives a helm wich i gave a lvl 13 armor bonus

#### Element of Water

#### Element of Water

 Level 18

JoinedAug 3, 2008Messages2,345

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

hashtables would be better I think, because then you could store values to the huge numbers of item rawcodes.

#### Yixx

#### Yixx

 Level 16

JoinedOct 12, 2008Messages1,569

Resources: 3

#### Resources
Spells and Systems 3

The tutorial is very nice. Not extremely usefull to me because i already kinda figured it out, but very usefull to new people and people who are just new to hashtables.

This way gives a much easier MUI way for GUI, and not to forget, spells stack much easier this way! You can just set the duration back to 10, or add 10 to the duration, whatever is your preference.

Good job!

BTW: When will THW support GUI hashtable actions? (Now it gives a question mark, as you can all see)

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

I will never use them, where should I need them for, if I can do my vJASS stuff with structs...

#### SlayerII

#### SlayerII

 Level 9

JoinedAug 21, 2008Messages533

Resources: 3

#### Resources
Skins 1Maps 1Tutorials 1

Cause you also use strings for the keys of the hashtables, making it easier to use em(if you need 30 keys its easier if you name instaed of having the numbers 1-30)

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

But, the same thing is what I can do with structs, but structs are older and work without any bugs.

#### Michael Jackson

#### Michael Jackson

 Level 2

JoinedJul 8, 2009Messages20

I still don't get this...as to why this is useful.

#### Element of Water

#### Element of Water

 Level 18

JoinedAug 3, 2008Messages2,345

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

There are some things which are quite hard to do with just structs, for example if you need a separate timer for each instance of the spell. These things are much easier to do with hashtables.

#### Lightstalker

#### Lightstalker

 Level 7

JoinedJul 8, 2008Messages150

Resources: 2

#### Resources
Maps 1Spells and Systems 1

wyrmlord said:

 I'm extremely confused as to what you're trying to do. You should only need one hashtable per spell. The primary use of a hashtable is to "attach" data to handles. In my examples, I attached the information to units to keep track of how much more healing they needed and knockback information.


When using hashtables, you should not need to use global array variables as well, only the hashtable. If you can explain what you're trying to accomplish, I can try to point you in the right direction.


Click to expand...


Well I was trying to create a hashtable per unit every time it casted the spell so a spell is MUI and SUI (Same Unit Instanceable).


How would you create only 1 hashtable per spell??? Explain everything!

#### SlayerII

#### SlayerII

 Level 9

JoinedAug 21, 2008Messages533

Resources: 3

#### Resources
Skins 1Maps 1Tutorials 1

lol are you tottally retarded


hashtables in the examplas are using the ID of units so the only way to make a bug is that a unit cast the same spell more than one time. But this should be also easy to solve.


So:read the tutorial esatly before asking this

#### Element of Water

#### Element of Water

 Level 18

JoinedAug 3, 2008Messages2,345

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

You could try storing new hashtables in one main hashtable, but I guess you'd need an indexing system to organise it so... it's a very hard thing to achieve.

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Lightstalker said:

 Well I was trying to create a hashtable per unit every time it casted the spell so a spell is MUI and SUI (Same Unit Instanceable).


How would you create only 1 hashtable per spell??? Explain everything!

Click to expand...


This would be a rather simple problem in JASS since timers can be created dynamically while timers in GUI need to be created at map init. In GUI, I would probably prefer to use an approach with arrays that used some careful indexing. I recommend asking for help in the Triggers and Scripts forum since it isn't entirely about hashtables.

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Ok since this is gonna be dealing hashtables (and I must say very easy-to-understand tutorial, GOOD JOB!) there's one thing on efficiency I think should have been mentioned since all users that want to make MUI spells using hashtables are gonna be reading this tutorial from now on.


You know how using indexing GUI'ers turn the periodic event on and off, yupp something like that.


Here's a very fast draft I did (it's just a draft so don't tell me there's something wrong ):

Main
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Animate Dead

Actions
-------- bla bla bla --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Number of units in DT) Equal to 0

Then - Actions
Trigger - Turn on Loop <gen>

Else - Actions

Unit Group - Add (Triggering unit) to DT

Loop
Events
Time - Every 2.00 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in DT and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
-------- bla bla bla --------

Then - Actions
-------- bla bla bla --------

Else - Actions
-------- Clearing hashtables and all --------

Unit Group - Remove (Triggering unit) from DT

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Number of units in DT) Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

Makes sense eh?


Umm anyways, about hashTables, gotta make things a piece of cake, specially since now GUI users can basically use their arrays to loop through variables instead of using them to index.


I guess that could be useful for those Jassers who don't use vJass, although I doubt anyone would prefer hashtables over structs, 'cause that's just absurd...


Good job on this tutorial

#### Element of Water

#### Element of Water

 Level 18

JoinedAug 3, 2008Messages2,345

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

There are things which require attachment systems, and it seems to me that hashtables would make an ideal way of attaching things in vJass.

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Lightstalker said:

 Well I was trying to create a hashtable per unit every time it casted the spell so a spell is MUI and SUI (Same Unit Instanceable).


How would you create only 1 hashtable per spell??? Explain everything!

Click to expand...


Huh? You can increase the index by X and save it in the SpellHashtable, and after the instance shut down, you decrease it? Whats so hard about it?


Anyway, I would prefer structs, as already told.

#### Rmx

#### Rmx

 Level 20

JoinedAug 27, 2007Messages1,164

Resources: 37

#### Resources
Icons 16Maps 1Spells and Systems 20

When you do indexing system in GUI ... the spell will be MUI SUI ext..


But hashtables if you want it SUI ( Same Unit Insta... ) you have to index that unit bla bla .


Anyway Hashtable is easy to use but still same as indexing if Indexing is recycled right ^^

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Ok we are here to discuss the tutorial I guess rather than commenting on what's better and what's not, so I think closing this thread would be appropriate by now as the tutorial got enough comments (in my opinion).

#### Lightstalker

#### Lightstalker

 Level 7

JoinedJul 8, 2008Messages150

Resources: 2

#### Resources
Maps 1Spells and Systems 1

Yeah what I said was pretty dumb lol. Must have been tired when I wrote it.


Anyway, what I meant to say was this: how could you make hashtables SUI? Seeing as how when you cast the spell again, the previous values for that unit will be overwritten.


It seems everyone has suggested indexing system. How would that work? Could it be done in GUI?

#### Element of Water

#### Element of Water

 Level 18

JoinedAug 3, 2008Messages2,345

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Use Paladon's indexing system. Simplez.

#### Rmx

#### Rmx

 Level 20

JoinedAug 27, 2007Messages1,164

Resources: 37

#### Resources
Icons 16Maps 1Spells and Systems 20

 Use Paladon's indexing system. Simplez.


Click to expand...


Agreed ! Truth have been spoken !

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Paladon's "indexing system" is rather dangerous at best--indexes are never recycled if even one is somehow lost.

#### Rmx

#### Rmx

 Level 20

JoinedAug 27, 2007Messages1,164

Resources: 37

#### Resources
Icons 16Maps 1Spells and Systems 20

 Paladon's "indexing system" is rather dangerous at best--indexes are never recycled if even one is somehow lost.


Click to expand...


What do you mean is rather Dangerous :S, it will 100% recycle everything when all spells are finished :O, and how could it get LOST !, You will check by a game message .. Debug message if the trigger turn off and make everything until you are 100% sure, it is a perfect system, also if recycled correctly it will be better, but Paladon until everything finishes it will recycle

#### Element of Water

#### Element of Water

 Level 18

JoinedAug 3, 2008Messages2,345

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

Ok then, if Paladon's indexing system won't work, use the one in PurplePoot's tutorial.

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Definitely PurplePoot's is better, but Paladon's isn't bad imo, I mean, I don't think there would even be 8191 units in the map  so no worries about filling arrays.

#### Roflcoptor

#### Roflcoptor

 Level 9

JoinedNov 25, 2008Messages194

Resources: 6

#### Resources
Icons 1Spells and Systems 5

The problem about paladons would just be that it only resets if there is NO active instance running, so if you have an often used spell using this and it lasts quite long, and the counter never drops to 0 you might reach 8190 (or what it was), so I prefer a simple recycling, by setting all variables of the highest index into the expired index and decrease the max index. Well i didn't look at PP's yet but I'm sure it does something similar

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Ok, supposing each second one unit triggers the ability, knowing that each hour is 3600 seconds, you need over two hours before you fill the array. And since this is usually an impossible situation, well, no worries on using Paladon's system.


Yet, I prefer PP's, but I just happen to use Paladon's since it's no big deal and since it's neater (imo).

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Just because I wrote a tutorial about it doesn't mean I invented it. And yes, it's far better.


Also, why would a spell use only one index?

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1


 PurplePoot said:



 Just because I wrote a tutorial about it doesn't mean I invented it.


Click to expand...

Well yes true, it's actually the algorithm used in vJass =P

And I don't think anyone was talking about using one index as much as they were talking about the possibility of filling up arrays up to more than 8191.

#### Roflcoptor

#### Roflcoptor

 Level 9

JoinedNov 25, 2008Messages194

Resources: 6

#### Resources
Icons 1Spells and Systems 5

Ok now I got a question about hashtables: How efficient are they compared to indexing?

 You must log in or register to reply here.


### Similar threads

Locked

 Knockback Pathing Not Detected???


JeffreyQ

Dec 8, 2009

Triggers & Scripts

Replies3Views1K

Dec 12, 2009
JeffreyQ

 A Complete Beginners Guide to Hashtables


Jazztastic

Jun 15, 2011

Trigger (GUI) Editor Tutorials
2

Replies57Views45K

Nov 12, 2024
PurgeandFire

A

Locked

[Trigger]
 Hashtable - Spell Not Working


Acid9

May 9, 2010

Triggers & Scripts

Replies7Views2K

May 11, 2010
Acid9

A

 Demystifying hashtables


Antares

May 25, 2024

Trigger (GUI) Editor Tutorials

Replies7Views3K

Oct 11, 2025
Antares

Locked

 Hashtable D.O.T. A.O.E. bug


M_Vegeta

Nov 2, 2009

Triggers & Scripts

Replies12Views3K

Nov 3, 2009
M_Vegeta

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