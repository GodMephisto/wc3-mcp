---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/mui-spells-using-artificial-waits.223315/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Mui Spells Using Artificial Waits
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/mui-spells-using-artificial-waits.223315/>
#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

Introduction


Creating delayed spell effects that are MUI without the use of waits is fairly


simple. It just requires two variables and one periodic trigger. The first variable


is an integer that represents the current spell instance, whereas the second variable


is an integer that represents the total number of spell instances.

Avoiding Waits


This is an example of a spell that uses a Wait action in order to create a delayed effect:

Example
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Example

Actions
Set TempUnit = (Triggering unit)

Set TempPoint = (Target point of ability being cast)

Wait 2.00 seconds

Set TempGroup = (Units within 300.00 of TempPoint)

Unit Group - Pick every unit in TempGroup and do (Actions)
Loop - Actions
Unit - Cause TempUnit to damage (Picked unit), dealing 50.00 damage of attack type Spells and damage type Normal

Custom script: call DestroyGroup(udg_TempGroup)

Custom script: call RemoveLocation(udg_TempPoint)

This spell is not MUI and will give an inaccurate delay. In order to fix this, we


need to split this spell into two triggers. The first trigger will execute when the


spell is cast, and it will store spell data such as the caster, the level, the owner


of the caster, etc... The second trigger is going to have a periodic event. Let's make


it run every 1 second for simplicity as opposed to the 0.03 seconds used by most spell


makers. If you're going to make a timed spell, use 0.03. What we're going to do is add


one spell instance when the spell is cast, and iterate over all the spell instances in


the periodic trigger. It's basic indexing, and it looks like this:

Cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Animate Dead

Actions
Set MaxIndex = (MaxIndex + 1)

-------- Here's the bit we had in the first place before the wait --------

Set TempUnit[MaxIndex] = (Triggering unit)

Set TempPoint[MaxIndex] = (Position of TempUnit[MaxIndex])

-------- ----------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
MaxIndex Equal to 1

Then - Actions
Trigger - Turn on Loop <gen>

Else - Actions

Loop
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
For each (Integer CurrentIndex) from 1 to MaxIndex, do (Actions)
Loop - Actions
-------- Here's the bit we had in the first place after the wait --------

Set TempGroup = (Units within 200.00 of TempPoint[CurrentIndex])

Unit Group - Pick every unit in TempGroup and do (Actions)
Loop - Actions
Unit - Cause TempUnit[CurrentIndex] to damage (Picked unit), dealing 50.00 damage of attack type Spells and damage type Normal

Custom script: call DestroyGroup(udg_TempGroup)

-------- This is recycling. It's unimportant in terms of spell effects, but it is important in terms of MU-Instancability --------

Custom script: call RemoveLocation(udg_TempPoint[udg_CurrentIndex])

Set TempPoint[CurrentIndex] = TempPoint[MaxIndex]

Set TempUnit[CurrentIndex] = TempUnit[MaxIndex]

Set MaxIndex = (MaxIndex - 1)

Set CurrentIndex = (CurrentIndex - 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
MaxIndex Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

There we go, this is our spell. 100% MUI and functional.

Using a Counter


This spell will wait 1 second, not 2. This is a problem that can


be addressed by using a counter.


In the caster trigger, we're going to set our counter to 0, and


in the periodic trigger, we're going to increment our counter by 1.


If the counter reaches our desired value, we execute the remaining


code and end the spell instance.


This is what our final product would look like:

Cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Animate Dead

Actions
Set MaxIndex = (MaxIndex + 1)

-------- Here's the bit we had in the first place before the wait --------

Set TempUnit[MaxIndex] = (Triggering unit)

Set TempPoint[MaxIndex] = (Position of TempUnit[MaxIndex])

Set Counter[MaxIndex] = 0

-------- ----------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
MaxIndex Equal to 1

Then - Actions
Trigger - Turn on Loop <gen>

Else - Actions

Loop
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
For each (Integer CurrentIndex) from 1 to MaxIndex, do (Actions)
Loop - Actions
Set Counter[CurrentIndex] = (Counter[CurrentIndex] + 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Counter[CurrentIndex] Equal to 2

Then - Actions
-------- Here's the bit we had in the first place after the wait --------

Set TempGroup = (Units within 200.00 of TempPoint[CurrentIndex])

Unit Group - Pick every unit in TempGroup and do (Actions)
Loop - Actions
Unit - Cause TempUnit[CurrentIndex] to damage (Picked unit), dealing 50.00 damage of attack type Spells and damage type Normal

Custom script: call DestroyGroup(udg_TempGroup)

-------- Recycling --------

Custom script: call RemoveLocation(udg_TempPoint[udg_CurrentIndex])

Set TempPoint[CurrentIndex] = TempPoint[MaxIndex]

Set TempUnit[CurrentIndex] = TempUnit[MaxIndex]

Set Counter[CurrentIndex] = Counter[MaxIndex]

Set MaxIndex = (MaxIndex - 1)

Set CurrentIndex = (CurrentIndex - 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
MaxIndex Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

Else - Actions

Overview


This is a small explanation concerning what will actually happen during the


execution of the spell to clear things up for you if you didn't get it yet.

Let MaxCounter = 2.


1) The spell has been cast.


2) After 1 second:

 Counter is initially 0.

 Counter = Counter + 1

 Counter is now 1.

 Counter < MaxCounter

 Nothing happens.


3) After 2 seconds:

 Counter is initially 1.

 Counter = Counter + 1

 Counter is now 2.

 Counter == MaxCounter

 Actions execute.

 Spell instance recycled.


This spell is now MUI and works for our desired delay duration.

Wrap-up


If you still find it hard to understand, feel free to PM me any time.


Thank you for reading.


~Tank-Commander

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Thank you for submitting this.

I replaced the content in my tutorial in the "MUI without Waits" section with a link to this.

Since my tutorial was approved, I guess this one is supposed to be approved by default as well, but as a form of respect to Purgeandfire111, I'll leave this in the Submissions section for him approve himself

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice job. It all looks fine to me and is explained well. Approved.

#### T. D. W.

#### T. D. W.

 Level 16

JoinedJul 31, 2012Messages2,217

Resources: 9

#### Resources
Maps 1Spells and Systems 8

I am not good at MUI but there is something i know good:

Loop with 1 second loop is not accurate he does not start looping when you turn it on if he was off

i knew that by accident when i had a loop trigger every 30 s if i turned it on the 20 s of game time he will execute after 10 seconds

So the only "accurate loop" is 0.01 s [spells]

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

That applies to loop triggers because they are started and turned off I guess, so internally, the code just wouldn't execute while they tick away (If this is even true)

For timers, it's different because you start them up and pause them, then restart when you want to continue.

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

DotCa said:

 I am not good at MUI but there is something i know good:


Loop with 1 second loop is not accurate he does not start looping when you turn it on if he was off


i knew that by accident when i had a loop trigger every 30 s if i turned it on the 20 s of game time he will execute after 10 seconds


So the only "accurate loop" is 0.01 s [spells]


Click to expand...


 Tank-Commander said:



 Let's make


it run every 1 second for simplicity as opposed to the 0.03 seconds used by most spell


makers. If you're going to make a timed spell, use 0.03.


Click to expand...


Read context first, then comment

#### Xerxes

#### Xerxes

 Level 20

JoinedApr 14, 2012Messages2,906

Resources: 8

#### Resources
Icons 4Maps 1Spells and Systems 3

This really helped me; thanks Tank-Commander!

#### BlackShadows

#### BlackShadows

 Level 2

JoinedJun 11, 2013Messages4

do we need to add this?

Custom script: call RemoveLocation(udg_TempPoint[Current_Index])

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

That's a line to remove the leak before recycling, that's only necessary if you have a location that needs to be removed (see leak-based tutorials). It's not necessary to specifically artificial waits no.

#### DivineLight

#### DivineLight

 Level 13

JoinedJan 30, 2012Messages1,297

Resources: 5

#### Resources
Icons 1Maps 2Spells and Systems 2

i tried this and when i use the

Custom script: call RemoveLocation(udg_TempPoint[Current_Index])
and save it, WE rejected it..


i mean it displaying an error message


why could that happen?

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

If you're following the tutorial spell, you should have a variable called udg_TempPoint of type point which is an array, if you have that line -without- a variable to relate it to, WE will reject it

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

It's not Current_Index, it's udg_CurrentIndex (or udg_Current_Index if you named the variable Current_Index in the variable editor)

All variables created with the Variable Editor are referenced in JASS with an udg_ prefix.

#### DivineLight

#### DivineLight

 Level 13

JoinedJan 30, 2012Messages1,297

Resources: 5

#### Resources
Icons 1Maps 2Spells and Systems 2

so something like this?

Custom script: call RemoveLocation(udg_TempPoint[udg_Current_Index])
hey mag, don't talk about jass please..

i don't understand any of it

super confusing............ at least to me

back to topic so everything related to variable use the udg_prefix i suppose?


 Last edited: Jun 21, 2013

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

if it's done in a custom script line and you're using GUI, then yes.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

DivineLight said:

 so something like this?

Custom script: call RemoveLocation(udg_TempPoint[udg_Current_Index])
hey mag, don't talk about jass please..


i don't understand any of it


super confusing............ at least to me


back to topic so everything related to variable use the udg_prefix i suppose?


Click to expand...


that custom script line is jass thats y he said referenced in jass

#### Xerxes

#### Xerxes

 Level 20

JoinedApr 14, 2012Messages2,906

Resources: 8

#### Resources
Icons 4Maps 1Spells and Systems 3

Hello Tank_Commander

I know you've told me the answer to this question, but I would really appreciate it again if you or someone else with experience reiterated the answer to me again:

Your tutorial only tells me how to efficiently use 1 counter in an MUI spell; what if I wanted to use multiple counters to differentiate and separate different stages of my spell?

Also, I've pondered on this question for a while now:

In the case that I have to use more than a Cast and a Loop trigger, and I add a Loop 2 trigger, where do I de-index or recycle my variables? Or do most cases in spell making avoid having a Loop 2 trigger because of multiple counters?

I would love some feedback here, I would really appreciate it.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

u can use one counter for the whole of ur multistage spell.

use a counter to count up to 50 then it finishes stage one so u do the actions. then it continues to count to 100 then it finishes stage 2 and u do actions. and so on.

just remember to structure ur ITE the right way or it wont work properly

#### Xerxes

#### Xerxes

 Level 20

JoinedApr 14, 2012Messages2,906

Resources: 8

#### Resources
Icons 4Maps 1Spells and Systems 3

Yes I remember one part of TC's messages to me were about the structures of the ITEs; something about nesting and stuff; could you please elaborate more deathismyfriend?

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

im on phone so ill do best i can. basically u just have a few ITEs were ur smallest value is last. this way it will only run once.

example:

Set counter = counter + 1

if
counter equal to 200

then

else
if
counter equal to 150

then

else
if
counter equal to 100

then

else
if
counter equal to 50

then

else

this way the counter counts up and hits the first option selected. i use equal to which means that it will only run when its equal to that number. if u want actions in there then u would need to use greater than or equal to. dont use less than or equal to in this case.

u can use less than ( notequal to) but u would have to switch the code around so that the smaller numbers go first. then u can use less than

#### Xerxes

#### Xerxes

 Level 20

JoinedApr 14, 2012Messages2,906

Resources: 8

#### Resources
Icons 4Maps 1Spells and Systems 3

^ Thanks a lot; I'm going to try it out

+REP

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

MasterTrainer said:

 ^ Thanks a lot; I'm going to try it out


+REP


Click to expand...


np and thx

#### flacidTurtle

#### flacidTurtle

 Level 4

JoinedAug 18, 2013Messages71

Blink Variation Not Working

Hey Guys, Long time Wc3 Map editor.

 Quick Query, can you proof-read my triggers, the "wait" isn't working. I believe its not even catching the ability.

Blink Catch
Events
Unit - A unit Finishes casting an ability

Conditions
(Ability being cast) Equal to Blink

Actions
Set Blink_Max_Index = (Blink_Max_Index + 1)

Set Blink_Temp_Unit[Blink_Max_Index] = (Triggering unit)

Set Blink_Counter[Blink_Max_Index] = 0

Unit - Add Blink Immunity (Spell Immunity) to (Triggering unit)

Unit - Set level of Blink Immunity (Spell Immunity) for Blink_Temp_Unit[Blink_Current_Index] to 1

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Max_Index Equal to 1

Then - Actions
Trigger - Turn on Blink Loop <gen>

Else - Actions

Blink Loop
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
For each (Integer Blink_Current_Index) from 1 to Blink_Max_Index, do (Actions)
Loop - Actions
Set Blink_Counter[Blink_Current_Index] = (Blink_Counter[Blink_Current_Index] + 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Counter[Blink_Current_Index] Equal to 3

Then - Actions
Unit - Remove Blink Immunity (Spell Immunity) from Blink_Temp_Unit[Blink_Current_Index]

Set Blink_Temp_Unit[Blink_Current_Index] = Blink_Temp_Unit[Blink_Max_Index]

Set Blink_Counter[Blink_Current_Index] = Blink_Counter[Blink_Max_Index]

Set Blink_Max_Index = (Blink_Max_Index - 1)

Set Blink_Current_Index = (Blink_Current_Index - 1)

Else - Actions
Unit - Set level of Blink Immunity (Spell Immunity) for Blink_Temp_Unit[Blink_Current_Index] to Blink_Counter[Blink_Current_Index]

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Max_Index Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

Spell Immunity isn't even being applied to the caster during debug test. The ability is supposed to give the caster "Blink Immunity" for 3 seconds after cast (Levels are used as a countdown via Tooltip display)

I've also Attempted This to no avail.

Blink Loop
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
For each (Integer Blink_Current_Index) from 1 to Blink_Max_Index, do (Actions)
Loop - Actions
Set Blink_Counter[Blink_Current_Index] = (Blink_Counter[Blink_Current_Index] + 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Counter[Blink_Current_Index] Equal to 1

Then - Actions
Unit - Remove Blink Immunity - 3 (Spell Immunity) from Blink_Temp_Unit[Blink_Current_Index]

Unit - Add Blink Immunity - 2 (Spell Immunity) to (Triggering unit)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Counter[Blink_Current_Index] Equal to 2

Then - Actions
Unit - Remove Blink Immunity - 2 (Spell Immunity) from Blink_Temp_Unit[Blink_Current_Index]

Unit - Add Blink Immunity - 1 (Spell Immunity) to (Triggering unit)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Counter[Blink_Current_Index] Equal to 3

Then - Actions
Unit - Remove Blink Immunity - 1 (Spell Immunity) from Blink_Temp_Unit[Blink_Current_Index]

Set Blink_Temp_Unit[Blink_Current_Index] = Blink_Temp_Unit[Blink_Max_Index]

Set Blink_Counter[Blink_Current_Index] = Blink_Counter[Blink_Max_Index]

Set Blink_Max_Index = (Blink_Max_Index - 1)

Set Blink_Current_Index = (Blink_Current_Index - 1)

Else - Actions
Unit - Set level of Blink Immunity - 3 (Spell Immunity) for Blink_Temp_Unit[Blink_Current_Index] to Blink_Counter[Blink_Current_Index]

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Max_Index Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

This variation consists of 3 separate "Blink Immunity" spells being interchanged during the for loop.

I know the solution is probably simple, but I don't see it.

Found it!

 (Ability being cast) Equal to Blink


Click to expand...

Was using the wrong Blink

Also Changed

 Unit - A unit Finishes casting an ability


Click to expand...

to

 Unit - A unit Begins casting an ability


Click to expand...

Further Discover also brought me to notice in my second attempt without a level system,

 Blink Loop

 Events

 Time - Every 1.00 seconds of game time

 Conditions

 Actions

 ...

 Unit - Add Blink Immunity - 2 (Spell Immunity) to (Triggering unit)


Click to expand...

Changed that and everything works fine. Free MUI Code for a Blink + Spell Immunity Here

Blink Catch
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to Blink

Actions
Set Blink_Max_Index = (Blink_Max_Index + 1)

Set Blink_Temp_Unit[Blink_Max_Index] = (Casting unit)

Set Blink_Counter[Blink_Max_Index] = 0

-------- --------

-------- Spell Immunity 1/3 - Name Change to "Spell Immunity - 3" to signify a countdown --------

Unit - Add Blink Immunity - 3 (Spell Immunity) to (Casting unit)

-------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Max_Index Equal to 1

Then - Actions
Trigger - Turn on Blink Loop <gen>

Else - Actions

This requires 3 Spell Immunity spells in the data editor

Blink Loop
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
For each (Integer Blink_Current_Index) from 1 to Blink_Max_Index, do (Actions)
Loop - Actions
Set Blink_Counter[Blink_Current_Index] = (Blink_Counter[Blink_Current_Index] + 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Counter[Blink_Current_Index] Equal to 1

Then - Actions
-------- --------

-------- Removes Previous Immunity (1/3) --------

Unit - Remove Blink Immunity - 3 (Spell Immunity) from Blink_Temp_Unit[Blink_Current_Index]

-------- Spell Immunity 2/3 - Name Change to "Spell Immunity - 2" to signify a countdown --------

Unit - Add Blink Immunity - 2 (Spell Immunity) to Blink_Temp_Unit[Blink_Current_Index]

-------- --------

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Counter[Blink_Current_Index] Equal to 2

Then - Actions
-------- --------

-------- Removes Previous Immunity (2/3) --------

Unit - Remove Blink Immunity - 2 (Spell Immunity) from Blink_Temp_Unit[Blink_Current_Index]

-------- Spell Immunity 3/3 - Name Change to "Spell Immunity - 1" to signify a countdown --------

Unit - Add Blink Immunity - 1 (Spell Immunity) to Blink_Temp_Unit[Blink_Current_Index]

-------- --------

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Counter[Blink_Current_Index] Equal to 3

Then - Actions
-------- --------

-------- Removes Previous Immunity (3/3) --------

Unit - Remove Blink Immunity - 1 (Spell Immunity) from Blink_Temp_Unit[Blink_Current_Index]

-------- --------

-------- Cleanup below --------

Set Blink_Temp_Unit[Blink_Current_Index] = Blink_Temp_Unit[Blink_Max_Index]

Set Blink_Counter[Blink_Current_Index] = Blink_Counter[Blink_Max_Index]

Set Blink_Max_Index = (Blink_Max_Index - 1)

Set Blink_Current_Index = (Blink_Current_Index - 1)

Else - Actions
Unit - Set level of Blink Immunity - 3 (Spell Immunity) for Blink_Temp_Unit[Blink_Current_Index] to Blink_Counter[Blink_Current_Index]

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Max_Index Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions


 Last edited: Aug 18, 2013

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Your code has one problem. The trigger-turn-off block should be executed immediately after the instance deallocation code. The trigger never turns off in its current state. (I can tell by reading it, I haven't tested anything)

Move this:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Blink_Max_Index Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

to:

Set Blink_Temp_Unit[Blink_Current_Index] = Blink_Temp_Unit[Blink_Max_Index]

Set Blink_Counter[Blink_Current_Index] = Blink_Counter[Blink_Max_Index]

Set Blink_Max_Index = (Blink_Max_Index - 1)

Set Blink_Current_Index = (Blink_Current_Index - 1)

// Here.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

I normally put it after the loop. That way it only runs once each time rather than multiple times if you de-index multiple ones.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

But that way, it runs 32x a second regardless of whether the instance count changed or not. With mine, it only does the check when required /o/

So, for a 3-second spell, your way runs it 96 times, while mine runs it 1 time.

For a 3-second spell with 10 instances, yours runs 96 times, mine runs 10 times.

In general, mine runs much, much less /o/

#### Burning Legion

#### Burning Legion

 Level 1

JoinedJun 19, 2013Messages9

Tank- Commander, if the event of the " loop" trigger is - Every 2 seconds of game time , will the actions of trigger be executed only after 2 seconds after the trigger is turned on ?


Why cant we simply do it like this ?

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

every 2 seconds means that it will run every 2 seconds after being turned on.

not sure what your questions is exactly. It's really vague. Please elaborate more.

#### Burning Legion

#### Burning Legion

 Level 1

JoinedJun 19, 2013Messages9

deathismyfriend , what i meant was if any trigger with the event that i have mentioned is turned on , will the action be executed only on 2 seconds after the trigger is turned on ? Or will they be executed immediately ? Anyway , thanks for trying to help me.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

its every 2 seconds after it has been turned on. i said that in the above post.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Nice tut TC! This will surely be of help to those aspiring coders out there.

 Changed that and everything works fine. Free MUI Code for a Blink + Spell Immunity Here


Click to expand...

No offense, but it's GUI, it's not like we could copy paste it to our maps, right?

EDIT: And also instead of lots of ITEs, you could array those immunity abilities.

#### BETABABY

#### BETABABY

 Level 12

JoinedMay 28, 2015Messages382

Resources: 21

#### Resources
Icons 20Skins 1

the only thing confusing to me are the variables.. If someone just make a tutorial about MUI's with a list of variables you need to use like variable types and stuff, i would be freaking happy.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

BETABABY said:

 the only thing confusing to me are the variables.. If someone just make a tutorial about MUI's with a list of variables you need to use like variable types and stuff, i would be freaking happy.


Click to expand...


I made a tutorial that discusses the same topic as applied to a specific spell:

https://www.hiveworkshop.com/forums...orials-279/visualize-dynamic-indexing-241896/


It is quite long, but it might be worth giving a shot! Otherwise, the best way to learn it is honestly to: (1) come up with a spell idea (2) try making it. If you get stuck, ask for help on the forums and we'll gladly help you get it going.


MUI is one of those issues that is really hard to understand until you experience it. Sometimes it can be helpful to make the spell not-MUI, then try casting it with multiple units to see what goes wrong. Just reading a tutorial won't be enough to fully understand it. But if you apply it to a spell, you'll begin to understand it much better.

#### BETABABY

#### BETABABY

 Level 12

JoinedMay 28, 2015Messages382

Resources: 21

#### Resources
Icons 20Skins 1

PurgeandFire said:

 I made a tutorial that discusses the same topic as applied to a specific spell:

https://www.hiveworkshop.com/forums...orials-279/visualize-dynamic-indexing-241896/

It is quite long, but it might be worth giving a shot! Otherwise, the best way to learn it is honestly to: (1) come up with a spell idea (2) try making it. If you get stuck, ask for help on the forums and we'll gladly help you get it going.

MUI is one of those issues that is really hard to understand until you experience it. Sometimes it can be helpful to make the spell not-MUI, then try casting it with multiple units to see what goes wrong. Just reading a tutorial won't be enough to fully understand it. But if you apply it to a spell, you'll begin to understand it much better.

Click to expand...

Thanks dude, and btw.. I have a question, it is not really related to this tutorial but is it okay for a spell to be not MUI as long as its only for one unit?? Is it bad to have it in multiplayer maps?

Edit: Is it also bad if I use the same variables with arrays?? Like for example.. If I use Caster_Loc[1] on my first spell and also use it on my other spell. Sorry bout the unrelated questions, I'm too lazy to search the forums and I just want quick answers.


 Last edited: Oct 16, 2015

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 Mui


Wolf_Wing

Dec 26, 2012

Triggers & Scripts

Replies16Views2K

Jan 2, 2013
Ofel

Locked

 Help Indexing


HolyBear

Dec 28, 2013

World Editor Help Zone

Replies2Views2K

Dec 29, 2013
IcemanBo

 Building large spells using StageIDs


Tank-Commander

Apr 30, 2016

Trigger (GUI) Editor Tutorials

Replies11Views5K

Jun 7, 2016
Tank-Commander

 [GUI] Efficient Arrays - Multiinstanceabilty for Timers, Custom Values, and more


PurplePoot

Aug 27, 2008

Trigger (GUI) Editor Tutorials

Replies8Views10K

Aug 30, 2008
PurplePoot

Locked

 List Unit Abilities


IcemanBo

Feb 26, 2019

Advanced Scripting

Replies27Views7K

May 20, 2022
Bribe

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