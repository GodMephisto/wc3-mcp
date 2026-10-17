---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/gui-advanced-triggering-tips.190802/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# GUI Advanced Triggering Tips
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/gui-advanced-triggering-tips.190802/>
#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

[GUI]*Advanced Triggering Tips

Advanced Triggering Tips

Why I am writing this

I see lots of GUI scripts which make the same mistakes again and again, whether it be by leaks or by adding work to the list that doesn't need to be added.​

Hopeful goals

I hope GUI users will learn to pay better attention to what they are working with and save a bit of time.​

Leak removal tips

I often see people come here asking for "does this thing leak?" without realizing that leak-spotting is not a magical ability but a simple science.

- Example

Custom script: set bj_wantDestroyGroup = true
That's probably the most seemingly-magical custom script out there, which is probably why most people aren't using it. The "magic" is found in the JASS code, so the next part, if you don't mind a little JASS intimidation, will turn the magic into science:

 JASS:

//===========================================================================
function ForGroupBJ takes group whichGroup, code callback returns nothing
 // If the user wants the group destroyed, remember that fact and clear
 // the flag, in case it is used again in the callback.
 local boolean wantDestroy = bj_wantDestroyGroup
 set bj_wantDestroyGroup = false

 call ForGroup(whichGroup, callback)

 // If the user wants the group destroyed, do so now.
 if (wantDestroy) then
 call DestroyGroup(whichGroup)
 endif
endfunction

Yep, real magic. You can see the operations at the scripting level and realize that bj_wantDestroyGroup, if true, will simply tell the game to destroy the group, not "somehow destroy the group in some mysterious way".

- So what's a ForGroupBJ, and does it taste good on salad?

ForGroupBJ is, in Trigger terminology, "Unit Group - Pick all units in (Unit group) and do (Actions)". So, right before entering any "Pick all units", just set the bj_wantDestroyGroup variable and you're set and ready.

Isn't that a lot easier than setting the group to a temporary variable, doing the "pick all units", then adding a custom script variable to destroy the group? I certainly like to think so.

Just make sure to watch out for any "pick all units" in your map, because there is a custom script eager to help you destroy those memory leaks. Here are the other things you can use it for:

Uses
Events

Conditions

Actions
Set Count = (Number of units in (Unit group))

Set Unit = (Random unit from (Unit group))

Set Boolean = (All units in (Unit group) are dead) Equal to True

Set Boolean = ((Unit group) is empty) Equal to True

Unit Group - Add all units in (Unit group) to (Unit group)

-------- The first group will be destroyed in the above case --------

Unit Group - Remove all units in (Unit group) from (Unit group)

-------- The first group will be destroyed in the above case --------

Other tips

I don't know about the other "Leak Removal Experts", but here's how I decode GUI into a readable format for detecting leaks and other things:

"(" and ")"​

What's that, you ask? Those are "magic" indicators "magically" (or, to some, "annoyingly") placed throughout your GUI script.

No, really, they almost always indicate what is in JASS a "function call". If you notice, every instance of "Integer A" from a "For each (Integer A)" loop is surrounded by (parenthesis).

This is because it is a call to a function "GetForLoopIndexA()" instead of "bj_forLoopAIndex", which is what you're really working with and what the function returns.

Because of this, looping with Integer A has slower performance than looping your own custom integer. For people who value execution speed of code and other programming gibberish, (this) should be a sign.

Examples

(Position of (Triggering unit))
Look at that. Two sets of parenthesis because... two function calls are involved: GetUnitLoc(GetTriggerUnit()). Both the location and the unit should be stored in a variable (especially the location, if you know what I mean) if called more than 2 or 3 times, because this cuts down on function calls.

(Player((Integer A)))

(Owner of (Triggering unit))

Decrease your menu-scrolling time

Not only is (Triggering unit) <<faster>> than (Casting unit) or (Dying unit) in terms of execution time, it's actually faster to click because it is always the first option on the menu tree. Just use (Triggering unit) and, of course, set it to a variable if you use it more than 2-3 times.

Now that that's been mentioned, how about (Owner of (Triggering unit))? Ever notice that (Triggering player) is the first item on the menu-tree? That's because it's able to be used in more cases than you could imagine.

Every event which is selectable as "Any unit ---" is, in JASS, a "playerunitevent". This means that it has two triggering components, a unit, and a player.

Long story short, every "Any unit event" can use (Triggering player) instead of (Owner of (Triggering unit)) and get the same results.

This will cut down on your menu-scrolling time as well as game execution time, so it's faster in every sense of the word.

You can thank PurgeAndFire for showing me the (Triggering player) trick.​

Most Common Leaks - Thanks to Hashjie for this tip

Points

(Position of (Unit))

(Position of (Item))

(Center of (Region))

(Position of (Triggering unit) offset by 50.00 towards (Position of (Attacking unit)))

-------- --------

Set TempPoint = (Position of (Unit))

Custom script: call RemoveLocation(udg_TempPoint)

Unit groups

Unit Group - Pick all units in (Unit group) and do (Actions)

(Number of units in (Unit group))

-------- --------

Set TempGroup = (Unit group)

Custom script: call DestroyGroup(udg_TempGroup)
Or, more superior, as mentioned, before "units in group" or "pick all units", just place the "magic" line:

Custom script: set bj_wantDestroyGroup = true

Player groups

Game - Text Message to (Player Group)

-------- --------

Set TempForce = (Player Group)

Custom script: call DestroyForce(udg_TempForce)

Special effects

Special Effect - (Destroy (Last created Special Effect))

Lightning effects

Set LightningVar = (Last created Lightning Effect)

Custom script: call DestroyLightning(udg_LightningVar)

Dummy units

Unit - Make (Last created unit) explode on death

Unit - Add 1.00 Second Generic Expiration Timer to (Last created unit)

Floating Text

Floating Text - Change the lifespan of (Last created floating text) to 5.00 seconds

-------- Or --------

Floating Text - Destroy (Last created floating text)

Notes

Plenty of other things leak, most of which are a waste of time when working with GUI, but just keep in mind that pre-placed things in GUI usually should just be left alone (regions, sounds, triggers) as they are non-repeating leaks (aren't created+leaked over and over).​

Why are memory leaks important to watch out for?

If you haven't experienced first-hand, I suppose an explaination is in order.

You know that part of a computer called RAM? You need at least 128 of it to play WarCraft III, a recommended 512 IIRC. That's because WarCraft III idles at 100-ish MB of RAM. If you look at how much RAM WarCraft III is running, you'll notice it's higher when there are more units on the map than if there were few or none (on older computers it's often associated with slow performance as well). This is the RAM each object in the game is taking up - units, trees, doodads, items and those invisible things called locations, unit groups, player groups and more... so removing those things instead of leaving them in existence means that WarCraft III won't take up so much RAM.​

Other tips

If you use "pick all units" or "pick all players", you cannot use waits inside of the loops they give. Yeah, most people don't realize this. Any why should they? It's a weird thing that it doesn't work. But here you go:

Error
Events

Conditions

Actions
Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick all units in (Units in (Playable Map Area)) and do (Actions)
Loop - Actions
-------- Some stuff --------

Wait - 1.00 seconds

-------- More stuff --------

In that case, "more stuff" will never happen. You need:

Error
Events

Conditions

Actions
Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick all units in (Units in (Playable Map Area)) and do (Actions)
Loop - Actions
-------- Some stuff --------

Trigger - Run Error Fixed <Gen> (Ignoring conditions)

And...

Error Fixed
Events

Conditions

Actions
Wait - 1.00 seconds

-------- More stuff --------

Efficiency

Efficiency is the one of the most important thing about coding. Most people don't talk about efficiency for GUI triggers because they give it up as a "lost cause". I firmly disagree - even though GUI scripts can't be as efficient as doing everything in custom script (JASS), there are steps that can be taken to make it better - any improvements are welcome.

One of the first things people start doing when figuring out how to make JASS execute faster is store things like (Triggering unit) and (Level of (Ability being cast) for (Triggering unit)) into local variables, and reference the variables instead of calling functions. GUI users often miss this. The problem is that GUI users don't usually look at "(" and ")" as function calls - but in almost all cases, that is indeed what they are.

As a GUI user, those parenthesis should become very important for you when reviewing your code. When you see a lot of repetition in parenthesis ((Integer A) is also a function call), you should be looking out for ways to avoid using them for the same thing more than once or twice. Setting them to a variable is the best way to achieve this.

Set Caster = (Triggering unit)

Set Index = (Player number of (Triggering player))

Set Level = (Level of Blizzard for Caster)

Conclusion

I don't even read the whole GUI trigger to spot for memory leaks, I just look for the (parenthesis) and find out everything I need to know. I see this:

Example
Events
Unit - A unit starts the effect of an ability

Conditions
(Ability being cast) Equal to Blizzard

Actions
Unit - Create 1 Footman for (Owner of (Casting unit)) at (Position of (Casting unit)) facing Default building facing degrees

And I know that:

(Ability being cast) is a function call, (Owner of (Casting unit)) is two function calls, (Position of (Casting unit)) is two function calls, and I think to myself, "how can we shorten the amount of parenthesis?"

The result:

Example
Events
Unit - A unit starts the effect of an ability

Conditions
(Ability being cast) Equal to Blizzard

Actions
Set TempLoc = (Position of (Triggering unit))

Unit - Create 1 Footman for (Triggering player) at TempLoc facing Default building facing degrees

Custom script: call RemoveLocation(udg_TempLoc)

As I'm scanning through the triggers, what I'm really doing is scanning through a list of (parenthesis), and it makes it very easy to see when a location or something else is created so I can think about if it needs to be removed.​


 Last edited by a moderator: Jul 25, 2012

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

great tutorial, extremely usefull too. I already knew almost everything about this. But this is verry good to get rid of all these questions in the world editor help/triggers and scripts forums about if their trigger leaks or not.

And especially for the bj_wantDestroyGroup which I have seen a lot of people ask about. (Including me, some time ago xD)

You might however want to show the other leak removal scripts and how to use them.

Maybe a little explenation of what leaks are and why we would remove them?

Maybe tell them something about waits, MUI and stuff like that too?

This way you can completely cover it up.

Here's a little list:

Custom script: call DestroyForce( udg_Your_Variable ) //Player Group

Custom script: call RemoveRect( udg_Your_Variable ) //Region

Custom script: call DestroyLightning( udg_Your_Variable ) //Lightning Effect

Custom script: call DestroyTextTag( udg_Your_Variable ) //Floating Text

Floating Text - Change the lifespan of (Last created floating text) to 5.00 seconds //Floating Text (in GUI)

Floating Text - Destroy (Last created floating text) //Floating Text (in GUI)

Custom script: call DestroyTimer( udg_Your_Variable ) //Countdown Timer

Custom script: call DestroyTrigger( GetTriggeringTrigger() ) //Trigger

Unit - Add a 2.00 second Generic expiration timer to (Last created unit) //Unit (in GUI)

I might have forgotten a few, I hope you'll work this out.

I'm going to use this tutorial as a referral for the next one who asks the question if their trigger leaks

If I could rate this it would get a 5/5 from me

+rep for this great idea


 Last edited: Mar 1, 2011

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Updated with some new things based on the suggestions of Hashjie.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Love it. Very useful as a reference for GUI'ers. ~Approved.

#### EloTheMan

#### EloTheMan

 Level 11

JoinedMar 18, 2009Messages793

Resources: 1

#### Resources
Icons 1

This stuff is great!

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Bribe maybe you can add next few things into tutorial as well...

Explain difference:

X
Events
Map initialization

Time - Elapsed game time is 0.00 seconds

Conditions

Actions

This situation here:

Worst way

X1
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Player - Add 666 to Player 1 (Red) Current gold

+

X2
Events
Player - Player 2 (Blue) skips a cinematic sequence

Conditions

Actions
Player - Add 666 to Player 2 (Blue) Current gold

...

Better way

X
Events
Player - Player 1 (Red) skips a cinematic sequence

Player - Player 2 (Blue) skips a cinematic sequence

Player - Player 3 (Teal) skips a cinematic sequence

Player - Player 4 (Purple) skips a cinematic sequence

Player - Player 5 (Yellow) skips a cinematic sequence

Player - Player 6 (Orange) skips a cinematic sequence

...

Conditions

Actions
Player - Add 666 to (Triggering player) Current gold

The best way

X1
Events
Map initialization

Conditions

Actions
For each (Integer i) from 1 to 12, do (Actions)
Loop - Actions
Trigger - Add to X2 <gen> the event (Player - (Player(i)) skips a cinematic sequence)

+

X2
Events

Conditions

Actions
Player - Add 666 to (Triggering player) Current gold

Difference between

X1
Events
Unit - A unit Dies

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is A structure) Equal to True

Then - Actions
Player - Add 666 to (Triggering player) Current gold

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is A Hero) Equal to True

Then - Actions
Player - Add 666 to (Triggering player) Current gold

Else - Actions

and

X2

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is A structure) Equal to True

Then - Actions
Player - Add 666 to (Triggering player) Current gold

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is A Hero) Equal to True

Then - Actions
Player - Add 666 to (Triggering player) Current gold

Else - Actions

You already know all of this and how to explain, I just think it can be really useful for map/spell makers.

Also some triggers above can be good examples for your discussion about custom loop integers or calling (triggering player) instead (owner of (triggering unit))

Nice tutorial indeed

#### 88WaRCraFT3

#### 88WaRCraFT3

 Level 13

JoinedJun 9, 2009Messages1,134

Resources: 5

#### Resources
Icons 2Spells and Systems 3

wow cool stuff, just find out that my project has tons of leaks ... it would be pain in ass to fix it  but ty  +rep mannnnnnnnnnnnnnnnnnn

#### fakedrake

#### fakedrake

 Level 3

JoinedApr 5, 2010Messages31

Resources: 1

#### Resources
Maps 1

Kobas I know most what you are saying but can you explain to me the difference between the following?

-Kobas- said:

Difference between

X1
Events
Unit - A unit Dies

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is A structure) Equal to True

Then - Actions
Player - Add 666 to (Triggering player) Current gold

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is A Hero) Equal to True

Then - Actions
Player - Add 666 to (Triggering player) Current gold

Else - Actions

and

X2

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is A structure) Equal to True

Then - Actions
Player - Add 666 to (Triggering player) Current gold

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Triggering unit) is A Hero) Equal to True

Then - Actions
Player - Add 666 to (Triggering player) Current gold

Else - Actions

Click to expand...

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

The second example shows the second if-block placed inside the "Else - Actions" block of the first if-block. Instead of:

 JASS:

if condition then
 //Something
endif
if condition2 then
 //Something else
endif

It basically becomes:

 JASS:

if condition then
 //Something
elseif condition2 then
 //Something else
endif

The second being better because if "condition" is true, "condition2" will never be evaluated (as such a thing would be useless)

#### sentrywiz

S

#### sentrywiz

A great tutorial, too bad this wasn't around when I was learning the ropes.

But the floating text - isn't it removed when you disable its permanence and set it to lasting X seconds? Why do you have to manually put remove last created floating text.

+rep and a well written tut.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

I need to adjust the floating text description. There should be an "or" there, and couple the timed life with the non-permanence.

#### Skasi

#### Skasi

 Level 6

JoinedApr 18, 2011Messages47

Set TempFloat = (Last created floating text)

Floating Text - Change the lifespan of TempFloat to 0.90 seconds
What about this?

Floating Text - Destroy (Last created floating text)
Causes the text to immediately disappear.

Or doesn't that thing leak in the first place? I'm a bit confused and not even sure whether I understand what exactly is causing a leak. (I understand the "parenthesis" part though)

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

I have added the "Or" comment between the two. You are not the first to take away the wrong message from the thread.

The part about specific leaks was added in as an afterthought. And it is an incomplete list of things that leak anyway. That was just to give a rough idea.

#### Skasi

#### Skasi

 Level 6

JoinedApr 18, 2011Messages47

Aah. Well I was confused since the following worked just well:

Floating Text - Change the lifespan of (Last created floating text) to 0.90 seconds

Floating Text - Destroy (Last created floating text)
I thought the lifespan was needed so the text would not disappear after destroying it. Seems I'm really bad at those things. Thanks for the update, makes sense now!

#### Opaque

#### Opaque

 Level 2

JoinedApr 20, 2011Messages20

Wow, i never knew about some of these things, like that you could use Triggering Player instead of owner of triggering unit... that would have saved me so much time xD

One thing i don't get is, in your code to remove dummy units, you make the unit explode on death. Why? If you set the field to Can't Raise, Can't Decay in the Object Editor, wouldn't it not leave a corpse behind regardless?

#### Ber352

#### Ber352

 Level 3

JoinedJun 2, 2011Messages21

Resources: 1

#### Resources
Maps 1

I haven't read the whole tutorial, I just took a glance at it, but I didn't see the correction to another very common error, so I'll just post it here (feel free to add to the main post):

Sometimes people make triggers which can be repeated, but the person doesn't know that, let's consider this example:


Anywhere = variable for any region


[trigger=Error Trigger]Any Trigger


 Events


 Unit - a unit enters Anywhere


 Conditions


 Actions


 Do something[/trigger]


If the map is multiplayer, you see the error right away: If one player is near another and they both enter the region, the trigger will be executed twice, or the ammount of times equal to the number of units which entered the region.


Also, singleplayer maps get bugs too. If, for example, the map is too long and the person loses in the final boss. He wants to know what happens on the end, so he puts "Whosyourdaddy" and rushes to the final boss, not killing the creeps. The same error happens when his character and the creeps after him enter the region.


Now, for the fix. I found a way to deal with this ever since I began making maps, because I usually make multiplayer maps for my brother and I to play, so I had this problem a lot.


[trigger=Fixed Trigger]Any Trigger


 Events


 Unit - a unit enters Anywhere


 Conditions


 Actions


 Trigger - Turn off (This trigger)


 Do Something[/trigger]


Do this on every trigger you make, one more line won't kill you and you will avoid any future bug which you might spend hours trying to find the source. (note: this bug happens for any kind of action, but most commonly on units entering regions, that's why this is the example)


Do NOT put the "Turn off (This trigger)" at the end of the trigger, if your trigger lasts for some time, there could be enough time for another unit to get into that region, causing the same bug, so always put it on first line.


Also note that you can turn the trigger on anytime you want using the following:


[trigger=Turn Trigger On]Any Trigger 2


 Events


 Something happens


 Conditions


 Actions


 Trigger - Turn on Any Trigger[/trigger]

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

@Ber352

Or... You set up some conditions instead of turning off the trigger.

Also this tutorial is about leak removal and optimization, which I suggest you should take a good look at.

It's got nothing to do with what you where saying.

#### Ber352

#### Ber352

 Level 3

JoinedJun 2, 2011Messages21

Resources: 1

#### Resources
Maps 1

@Hashjie

Yeah, I was planning on looking at it soon, also, even if not added to the main post, one tip there won't hurt anyone, will it? Plus, it can be helpfull (and tbh I have no idea where to put it, I'm not gonna create a new thread just for one lil tip, it's kinda a waste of space)

Also, sometimes you don't want a condition on the trigger (ex: red player unit enters region), you want all the players to be able to start the trigger or something, making doing a condition way harder than simply putting a "Turn off (This trigger)"

#### NOSAKIAS

#### NOSAKIAS

 Level 3

JoinedApr 10, 2009Messages32

Resources: 1

#### Resources
Maps 1

[CAPS]Thank you so much for this![/CAPS]

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

If you turn trigger off it will disable actions for other players, but maybe you want to give +250 exp to each player hero that enter certain region.

I suggest boolean array in this case.

 Code:

Events:
 -Unit Enter THIS region
Conditions:
Actions:
 If B[PlayerNumber] == false then
 set B[PlayerNumber] = true
 ---- Actions ----
 endif

I use this as raw example it really doesn't mater will you code it with GUI or script idea is same.

#### Losam

#### Losam

 Level 11

JoinedMar 27, 2011Messages293

Resources: 22

#### Resources
Icons 7Spells and Systems 15

Dear Bribe, I have a question for you: On the way to remove that lightning is not correct (my World Edit Trigger blocks that has this script) you can explain?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Thank you for pointing that out. I have fixed it in the main post:

Set LightningVar = (Last created Lightning Effect)

Custom script: call DestroyLightning(udg_LightningVar)
It should be DestroyLightning() instead of RemoveLightning(), as shown above. (RemoveLightning() does not exist)

#### xn85d.2

#### xn85d.2

 Level 2

JoinedDec 7, 2012Messages16

PurgeandFire111 said:

 Thank you for pointing that out. I have fixed it in the main post:

Set LightningVar = (Last created Lightning Effect)

Custom script: call DestroyLightning(udg_LightningVar)
It should be DestroyLightning() instead of RemoveLightning(), as shown above. (RemoveLightning() does not exist)


Click to expand...

Lightning - Destroy (Last created lightning effect)
exists as a GUI function, why do you need to JASS it?

Also, I found that (Triggering Player) does not correctly replace (Owner of Unit(Entering Unit)) for

Unit - A unit enters Region

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Ah, I don't have the editor on this computer so I didn't know that. I will update it later.

Also, for (Triggering Player), it only works for PlayerUnitEvent/PlayerEvent (when converted to JASS) events. The region event isn't. But thanks for that, I see that the main post's statement was a bit broad so I will try to find time to update it for which GUI ones work and which ones don't.

 You must log in or register to reply here.


### Similar threads

 Convenient Unit Group Filtering in GUI


KILLCIDE

Apr 23, 2016

Trigger (GUI) Editor Tutorials

Replies27Views12K

Dec 10, 2021
Darth Atroxa

 Simple Triggered Hero Artificial Intelligence in GUI


DenZel94

May 2, 2007

Trigger (GUI) Editor Tutorials

Replies22Views26K

Jul 1, 2021
aloH

 GUI - Hero Click-Selection (Advanced)


Fulla

Oct 4, 2007

Trigger (GUI) Editor Tutorials

Replies38Views22K

Jul 23, 2008
Fulla

 Effortless and Efficient Unit Group functions for GUI Triggers


Spacebuns

Jan 6, 2022

Trigger (GUI) Editor Tutorials

Replies4Views4K

Jan 16, 2022
Spacebuns

 [Reviewed: BlinkBoy] Understanding triggers and creating them.


Zypher

Aug 20, 2008

Trigger (GUI) Editor Tutorials

Replies9Views6K

Sep 5, 2008
BlinkBoy

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