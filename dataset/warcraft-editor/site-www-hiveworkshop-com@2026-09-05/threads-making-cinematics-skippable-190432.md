---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-cinematics-skippable.190432/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making Cinematics Skippable
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-cinematics-skippable.190432/>
#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

#
 Making Cinematics Skippable

Introduction:

In this tutorial, you will learn how to enable the player to skip a cinematic that has already begun by pressing the "Esc" key using only GUI triggers.


This tutorial assumes that you know how to create a cinematic already.​

Why do I need this?:

Well, think about it this way. If you just lost a game, you damn sure don't want to have to go trough the 8 minute beginning cinematic all over again, do you? I know I wouldn't. In fact, I'd rather do anything else but that.​

So what can we find in an ordinary cinematic?:

Well a cinematic is usually a trigger that, for the most part, sticks to using actions of these types: Camera, Cinematic, and Unit.​

Important:

Make sure your cinematic is finished before you make it skippable, because it will look a bit more diluted when we're done.

This is the cinematic I prepared for this tutorial.

Example Cinematic
Events
Map Initialization

Conditions

Actions
Cinematic - Turn on cinematic mode for [All players]

Cinematic - Disable user control for [All players]

Cinematic - Fade in over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

Camera - Apply Camera 1 for [Player 1] over 0.00 seconds

Unit - Order [Unit 1] to Move To [Region 1]

Wait 2.00 Seconds

Cinematic - Send transmission to [All players] from [Unit 1] named Superman: Play no sound and display It's a bird. it's a plane. No! Its superman. Modify duration: add 4.00 seconds and Do not wait

Wait 4.00 Seconds

Cinematic - Send transmission to [All players] from [Unit 2] named Lex Luthor: Play no sound and display Your going down superman. Modify duration: add 3.00 seconds and Do not wait

Wait 3.00 Seconds

Unit - Order [Unit 2] to Laser beam [Unit 1]

Wait 1.00 Seconds

Unit - Order [Unit 2] to attack [Civilian]

Wait 1.00 Seconds

Unit - Kill Civilian

Wait 2.00 Seconds

Cinematic - Fade out over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

Wait 2.00 Seconds

Camera - Reset camera for [Player 1] over 0.00 seconds

Cinematic - Turn off cinematic mode for [All players]

Cinematic - Enable user control for [All players]

Cinematic - Fade in over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

Quest - Create a Requirement quest titled Kill Luthor with description Kill the villain using icon path ReplaceableTextures\Units\Lex luthor

Pretty simple huh?​

Getting Started:

It is very important that after the player skips the cinematic, everything in the map acts as it would have, regardless of whether or not the cinematic was fully viewed.


How do we do that? Well, we create a trigger that does it for us.

(I am going to name this trigger "The Kill Switch")


Look at the end result of the cinematic. What do you see?

This is what you should see:

[Unit 1] is now in Region 1.

[Unit 1] lost life because it was hit by Luthor's laser beam.

[Civilian] is now dead.

[Unit 2] has lower mana because it used the laser beam.

[Quest] "Kill Luthor" has been discovered.

[Cinematic] The cinematic mode gets turned off, the camera is reset, and user control has been re-enabled.


Now we need to create a trigger that does this instantly.

And here it is:

Example Cinematic Kill Switch
Events

Conditions

Actions
Cinematic - Fade out over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

Wait 2.00 Seconds

Unit - Move [Unit 1] (instantly) to [Region 1]

Unit - Set Life of [Unit 1] to [[Maximum Life of [Unit 1]] - 75.00]

Unit - Add a 0.00 second Generic expiration timer on [Civilian] (This kills the unit without leaving leaks behind)

Unit - Set Mana of [Unit 2] to [[Maximum Mana of [Unit 2]] - 100.00]

Quest - Create a Requirement quest titled Kill Luthor with description Kill the villain using icon path ReplacebleTextures\Units\Lex luthor

Camera - Reset camera for [Player 1] over 0.00 seconds

Cinematic - Turn off cinematic mode for [All players]

Cinematic - Enable user control for [All players]

Cinematic - Fade in over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

Different actions in the original cinematic, make it so that you need to add different things to the Killswich. (Go figure)


Just look at what happens in the original cinematic, and make the killswich act accordingly.​

Laying The Ground Work:

Now you need a Boolean Variable with the name of the trigger_Is_On


So: Example_Cinematic_Is_On


If this variable is true, then the cinematic is running right now.


If this variable is false, then the cinematic is not running right now.


That is the outline, so now let's put it to practice by adding it to the 2 triggers we have.

Example Cinematic
Events
Map Initialization

Conditions

Actions
Cinematic - Turn on cinematic mode for [All players]

Cinematic - Disable user control for [All players]

Cinematic - Fade in over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

Set Example_Cinematic_Is_On = True

Camera - Apply Camera 1 for [Player 1] over 0.00 seconds

Unit - Order [Unit 1] to Move To [Region 1]

Wait 2.00 Seconds

Cinematic - Send transmission to [All players] from [Unit 1] named Superman: Play no sound and display It's a bird. it's a plane. No! Its superman. Modify duration: add 4.00 seconds and Do not wait

Wait 4.00 Seconds

Cinematic - Send transmission to [All players] from [Unit 2] named Lex Luthor: Play no sound and display Your going down superman. Modify duration: add 3.00 seconds and Do not wait

Wait 3.00 Seconds

Unit - Order [Unit 2] to Laser beam [Unit 1]

Wait 1.00 Seconds

Unit - Order [Unit 2] to attack [Civilian]

Wait 1.00 Seconds

Unit - Kill Civilian

Wait 2.00 Seconds

Cinematic - Fade out over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

Wait 2.00 Seconds

Set Example_Cinematic_Is_On = False

Camera - Reset camera for [Player 1] over 0.00 seconds

Cinematic - Turn off cinematic mode for [All players]

Cinematic - Enable user control for [All players]

Cinematic - Fade in over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

Quest - Create a Requirement quest titled Kill Luthor with description Kill the villain using icon path ReplaceableTextures\Units\Lex luthor

If the Cinematic Kill Switch is used, then the cinematic will be off, so we need to set the variable to false in here as well.

Example Cinematic Kill Switch
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions
[Example_Cinematic_Is_On] Equal to True

Actions
Cinematic - Fade out over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

Set Example_Cinematic_Is_on = False

Wait 2.00 Seconds

Unit - Move [Unit 1] (instantly) to [Region 1]

Unit - Set Life of [Unit 1] to [[Maximum Life of [Unit 1]] - 75.00]

Unit - Add a 0.00 second Generic expiration timer on [Civilian] (This kills the unit without leaving leaks behind)

Unit - Set Mana of [Unit 2] to [[Maximum Mana of [Unit 2]] - 100.00]

Quest - Create a Requirement quest titled Kill Luthor with description Kill the villain using icon path ReplacebleTextures\Units\Lex luthor

Camera - Reset camera for [Player 1] over 0.00 seconds

Cinematic - Turn off cinematic mode for [All players]

Cinematic - Enable user control for [All players]

Cinematic - Fade in over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

The last two things we need to do before we're done laying the ground work are:


Add an Event that starts the Example Cinematic Kill Switch when the player presses the "Esc" Key, and a Condition that only allows the Kill Switch to run while Example Cinematic is running.

If you will look up at the trigger for a second time, you can see that I have added these elements already.​

Finishing the Product:

There's one more problem that we need to solve before the Example Cinematic is by all means "Skippable".


The problem is that if the player presses the "Esc" Key at the beginning, or in the middle of the cinematic, The Kill Switch trigger will do it's thing, but the cinematic will not know that it's supposed to stop, and it will keep on running as if nothing happened, which could possibly lead to many bugs in your map.

Now you can get around this problem, and I'm going to show you how.

(Credits to Maker for helping me here)

The outline goes like this:


You are going to have to add an If / Then / Else function, after each Wait action, of the cinematic trigger. This will result in the cinematic to realize whether it is going to continue or skip the cinematic.

The If / Then / Else Function:


This trigger action is designed to allow the map maker to add a condition in the middle of the trigger, and tell the trigger that

If the condition is met, Then it should do something, and if it is not, then it should do something else.

If:Example_Cinematic_Is_On equal to False,

Then: Skip remaining actions,

Else [Nothing]

So like I said, you add an If / Then / Else function, after each Wait action.

Example Cinematic
Events
Map Initialization

Conditions

Actions
Cinematic - Turn on cinematic mode for [All players]

Cinematic - Disable user control for [All players]

Cinematic - Fade in over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

Set Example_Cinematic_Is_On = True

Camera - Apply Camera 1 for [Player 1] over 0.00 seconds

Unit - Order [Unit 1] to Move To [Region 1]

Wait 2.00 Seconds

If [Example_Cinematic_Is_On] equal to [False] then [Skip Remaining Actions] Else [Do nothing]

Cinematic - Send transmission to [All players] from [Unit 1] named Superman: Play no sound and display It's a bird. it's a plane. No! Its superman. Modify duration: add 4.00 seconds and Do not wait

Wait 4.00 Seconds

If [Example_Cinematic_Is_On] equal to [False] then [Skip Remaining Actions] Else [Do nothing]

Cinematic - Send transmission to [All players] from [Unit 2] named Lex Luthor: Play no sound and display Your going down superman. Modify duration: add 3.00 seconds and Do not wait

Wait 3.00 Seconds

If [Example_Cinematic_Is_On] equal to [False] then [Skip Remaining Actions] Else [Do nothing]

Unit - Order [Unit 2] to Laser beam [Unit 1]

Wait 1.00 Seconds

If [Example_Cinematic_Is_On] equal to [False] then [Skip Remaining Actions] Else [Do nothing]

Unit - Order [Unit 2] to attack [Civilian]

Wait 1.00 Seconds

If [Example_Cinematic_Is_On] equal to [False] then [Skip Remaining Actions] Else [Do nothing]

Unit - Kill Civilian

Wait 2.00 Seconds

If [Example_Cinematic_Is_On] equal to [False] then [Skip Remaining Actions] Else [Do nothing]

Cinematic - Fade out over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

Wait 2.00 Seconds

If [Example_Cinematic_Is_On] equal to [False] then [Skip Remaining Actions] Else [Do nothing]

Set Example_Cinematic_Is_On = False

Camera - Reset camera for [Player 1] over 0.00 seconds

Cinematic - Turn off cinematic mode for [All players]

Cinematic - Enable user control for [All players]

Cinematic - Fade in over 2.00 seconds using texture [White Mask] and color [0.00%, 0.00%, 0.00%] with 0.00% transparency

Quest - Create a Requirement quest titled Kill Luthor with description Kill the villain using icon path ReplaceableTextures\Units\Lex luthor

Important:

Careful not to use the If / then / else multiple function, because even though it might work just as well as this one, it can become a large mess.


Credits: Maker, for helping me finish the product properly.


And I wish you good luck.​


 Last edited: Jun 1, 2011

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

The trigger formatting doesn't look right. What's with all the elses at the end of the last trigger?


You can use a single line if/then/else.


You shoud turn on/off the cinematic skipping trigger.

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

I admit, there is a problem with the if then else multiple function.


I tryed to make it look as it should have, but I bearly started using the trigger tag, and I don't know how.


I think I'm going to Make a Screedshot And Make it look right in Bitmap. . .


EDIT:


I'm done, and now it looks exactly the way it should.


And you can see that All those Else functions are still there, they have a purpuse.


It looks much less confuzing then It used to.


What do you mean "I can use a single line Iff then else"?


Allso, about turning off or on the Killswich.


Instead of adding 2 actions in the Cinematic trigger, I added 1 condition in the Killswich it's self


Makes no diference if you ask me


It's not like the kill swich will turn on any other time then when the specific cinematic is running.


P.S If this somehow counteracts a leak, then let me know, Because I have no Ideea how to get rid of them,


And I do not want to teach people to use sistems that have leaks.


 Last edited: Feb 26, 2011

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Use trigger tags, not images.


That kind of if/then/else snake can be horrible to manage if the cinematic is long. This is how to do it in a better way.

Opening Cinematic
Events

Conditions

Actions
// Set skip state to false so it won't automatically skip things

Set Cinematic_Skip = False

// Turn on the trigger that skips this cinematic

Trigger - Turn on Opening Skip <gen>

Cinematic - Fade out over 3.00 seconds ...

Wait 4.00 seconds

// This line makes the actions to be skipped. Add this after every wait.

If (Cinematic_Skip Equal to True) then do (Skip remaining actions) else do (Do nothing)

Visibility - Disable black mask

Camera - Apply 1c1 <gen> for Player 1 (Red) over 0.00 seconds

Wait 3.00 seconds

If (Cinematic_Skip Equal to True) then do (Skip remaining actions) else do (Do nothing)

Cinematic - Send transmission to Player Group - Player 1 (Red) from Scourge_Lord named Unknown: Play No sound and display Come my child, into.... Modify duration: Add 3.00 seconds and Don't wait

Wait 3.00 seconds

If (Cinematic_Skip Equal to True) then do (Skip remaining actions) else do (Do nothing)

Cinematic - Send transmission to Player Group - Player 1 (Red) from Scourge_Lord named Unknown: Play No sound and display Come my child, into.... Modify duration: Add 3.00 seconds and Don't wait

=======================

Wait 1.00 seconds

If (Cinematic_Skip Equal to True) then do (Skip remaining actions) else do (Do nothing)

Set p1 = (Center of Region 006 <gen>)

Special Effect - Create a special effect at p1 using Abilities\Spells\Human\MassTeleport\MassTeleportCaster.mdl

Special Effect - Destroy (Last created special effect)

Unit - Hide Scourge_Lord

Custom script: call RemoveLocation(udg_p1)

Wait 4.00 seconds

-------- ==================================== --------

If (Cinematic_Skip Equal to True) then do (Skip remaining actions) else do (Do nothing)

Cinematic - Fade in over 2.00 seconds using texture White Mask and color (0.00%, 0.00%, 0.00%) with 0.00% transparency

Cinematic - Turn cinematic mode Off for (All players) over 2.00 seconds

// Turn off the skipping trigger

Trigger - Turn off Opening Skip <gen>

Opening Skip
Events
// Registers when ESC is pressed

Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Cinematic - Fade out over 0.50 seconds using texture Black Mask and color (0.00%, 0.00%, 0.00%) with 0.00% transparency

// Sets the boolen to true, so actions will be skipped

Set Cinematic_Skip = True

// Prevent this trigger from running again

Trigger - Turn off (This trigger)

=====================

// You may need to do some actions here

=====================

Cinematic - Turn cinematic mode Off for Player Group - Player 1 (Red) over 2.50 seconds

Cinematic - Fade in over 2.50 seconds using texture White Mask and color (0.00%, 0.00%, 0.00%) with 0.00% transparency

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

There is no action in GUI that will "Skip remaining actions".


Also, You cant have a single line in "THEN", and Multiple lines in "ELSE"


The only way that would work, would be if you had new a trigger for each step of the cinematic


That wouldent be just messy, it would be Chaos.


And If you somehow find a way to use this to remove the tendency of the text to move right,


 it will cause lag, because it's going to act like this:


A 4 minute cinematic begins, and the player stops it in the first second.


Every 3 seconds or so, the trigger will ask its self "Should I do this?" (Creating a bit of lag every time)


and this will last 4 minutes of gameplay,


Unnecesary lag has been Achived.


It may be a bit (Not a lot) less messy, but you will compromise gaming comfort,


just to add mapping comfort.

EDIT: Never mind what I said up there I WAS SO WRONG.


Down below is the original post


I am glad your helping me inprove on this trigger, and if you could teach me how to add the doted line on the left, using


the trigger tag instead of images, I would be grateful.


Allso don't use custom scrip when trying to prove a point in GUY.


I also want to ask you what you think of the Tutorial its self,


besides the fact that the If then else function is a bit messy.


And I would like to know, how is it that tutorials like this one gett aproved.


Cause I have no Ideea


 Last edited: Mar 1, 2011

#### GIANT_CRAB

#### GIANT_CRAB

 Level 12

JoinedApr 4, 2010Messages862

Resources: 5

#### Resources
Maps 2Spells 3

This thing looks really weird and not really eye-candy.

Moreover, screenshots should be ATTACHED.

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

GIANT_CRAB said:

 This thing looks really weird and not really eye-candy.


Moreover, screenshots should be ATTACHED.


Click to expand...


Are you talking about the If / Then / Else function making the cinematic messy?


Or About how I formated the tutorial it's self?


If you will be so kind, the tutorial is may 23th post here,


I tryed to attach them, but I couldent make them show any way other then this


This is not what I want to show. . .


And still, the only reason I posted this tutorial, is because I looked fore a tutorial just like this,


ever since I started making maps in Warcraft III


The very first Trigger I made, was a cinematic.


It took me an entire year to figure out how to make Cinematics skipable,


And I say this is better then having to wait for enlignenment to strike.


P.S. This is the sistem I use in all of my cinematics, and it worck like a charm ingame.

#### GIANT_CRAB

#### GIANT_CRAB

 Level 12

JoinedApr 4, 2010Messages862

Resources: 5

#### Resources
Maps 2Spells 3

TDA said:

 Are you talking about the If / Then / Else function making the cinematic messy?


Or About how I formated the tutorial it's self?


If you will be so kind, the tutorial is may 23th post here,


I tryed to attach them, but I couldent make them show any way other then this

View attachment 98557


This is not what I want to show. . .


And still, the only reason I posted this tutorial, is because I looked fore a tutorial just like this,


ever since I started making maps in Warcraft III


The very first Trigger I made, was a cinematic.


It took me an entire year to figure out how to make Cinematics skipable,


And I say this is better then having to wait for enlignenment to strike.


P.S. This is the sistem I use in all of my cinematics, and it worck like a charm ingame.


Click to expand...


You should also fix your spelling mistakes. And you took a year to learn how cinematics are skip-able? Damned, you should read up on tutorials instead! Time and tide waits for no man, wasted 1 year, man...

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

Gosh those ware many spelling mistakes I had to correct!


But all of them (accept for the ones in the last 3 Images, are corrected)


And I can't find the right way to spell Skippable anywhere.


Well, when I say 1 year, I'm talking from the distance between the time I made my first cinematic, to the time I knew how to make it skippable,


Not about how much time I spent during that year, making maps.


I guess that if I put it this way, I only did about 4 months worth, of work and map related research during that year. And I did more then half in the Rain of Chaos Editor


You should know, that When I started making maps, I allredy had some triggering skills form making a few Starcraft I Maps.


I did read up on some tutorials, but I had no Idea how to use any of the following


Conditions, Variables, Imported material, Still don't know Any Custom scrip.


And you will now belive, what I actualy did create, using none of those.


My very first GUI Spell, was done using none of the above


I bet you that you can't create a Gui Spell without Variables an conditions, using the Rain of Chaos editor.


It is next to imposibile.


Do now Underestimate my work as a noob.


 Last edited: Feb 27, 2011

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Triggers show by Maker was used by blizzard itself

 Are you talking about the If / Then / Else function making the cinematic messy?


Click to expand...

Yes, very...

 Conditions, Variables, Imported material, Still don't know Any Custom scrip.

And you will now belive, what I actualy did create, using none of those.


Click to expand...

Then learn scripting language (jass, vjass...)

You won't improve your skills if you ignore things you listed above

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

I have been enlightened...

The entire Tutorial has been edited to use If / Then / Else Simple Funciton


Maker has been credited. +REP


You see, I always looked for the skip remaining action in the Trigger Section


After all, thats where it should be...


I looked for it in search, but then I didn't know what to look for...


So I have never seen that action before in my life. (To bad for me)


This is the reason why I did this statement this just a couple posts ago.

 There is no action in GUI that will "Skip remaining actions"


Click to expand...


Now I did a little more research, and turns out, it's in no section what so ever.


(What was blizzard thinking when they did that?)


Kobas? how can you know what blizzard did???


And sereously, what was blizzard thinking, when they put "Skip remaining actions, in no category what so ever?"

Hope you approve of the tutorial now.


 Last edited: Feb 28, 2011

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

 In this tutorial I am going to teach you, how to enable the player to jump over a cinematic

that has already begun, by pressing the "Esc" Key and I'm gonna do it all in GUY.


Click to expand...

see something wrong here ?

 Well we create a trigger that dose it for us.


Click to expand...

 Now we need to create a trigger that dose this instantly.


Click to expand...

and here ?

(if I'd take an overdose I'd probably be dead xD)

Also: add a expiration timer to the units instead of killing them. (Units leaks, but it's not such a big problem)

EDIT: correction: units don't leak they get removed after 60 seconds which means it doesn't stay in memory, but to make sure that you won't get to much units in your map it is advised to add an expiration timer or removing killed units.


 Last edited: Dec 22, 2011

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

And adissional spelling check has been performed, and Expiration timer has been added.


Well ofcourse that the Spelling checker wouldent tell me dose is the wrong way to spell that word


It's still an english word.


Dose is one of my: You think you know how to spell it, but really. . .


Your making a fool of your self on national television Spelling mistakes xD


P.S. I have a lot of spelling mistakes in my own language, so don't be surprised to find that the word english is spelled wrong in this post.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I fixed some things. However, for the escape trigger, what if they skip it after the laser beam is cast? Then the unit will lose twice the mana and the other unit will lose twice the health. To fix this, just change it to (Maximum Mana - 100) and (Maximum Life - 75). (or store the previous life/mana values in the cinematic trigger and change it accordingly)

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

Who on earth Has been playing with my tutorial!?


It no longer looks or feals anything like the tutorial I submited.

What? Is my english not scientific enough for you? you trashed every single trace of personality I put into it.


now it's just an encyclopedia... Nothing else!...


Seriously admin. If your going to do someting like this, you could have given me a heads up.


Atleast have the decency to post when you Drasticly change an important post.


Was my tutorial realy that bad? or do you have a compulsive urge to change the "good" to the "OMG what have you done?"


I understand you might want to make it a bit more scientific...


But you don't have to chance every single sentence into Grey matter and Technobabble.


not every single one.


And if you do this to all tutorials, then freaking tell us in the thread called READ BEFORE POSTING


you have the thread to tell us what to do and what not to do when we submit a tutorial, right???


Well... You should also tell us what your gonna do

Please, the admin who changed my tutorial, I'm not that mad, I have a saved copy of the text, so I havent lost the information.


Just... Post a reply, I would like to know why.


P.S. Admin... I don't like the new page arangement. (puting the subtitles more to the left then the text then the rest of the writhingl)


Makes it look... well... what's the opuset of good?


I would realy like to get the old alighnment back where it belongs.

PurgeandFire111 said:

 I fixed some things. However, for the escape trigger, what if they skip it after the laser beam is cast? Then the unit will lose twice the mana and the other unit will lose twice the health. To fix this, just change it to (Maximum Mana - 100) and (Maximum Life - 75). (or store the previous life/mana values in the cinematic trigger and change it accordingly)


Click to expand...


This tutorial's pourpos isn't to tell you how to make the loose life actions or kill unit action.


Cause if that were so, the tutorial would be twice as long.


That bug is easely fixable by even the dummest of WE users.


I just put it there, and when I noteced it was a glich, I didn't take it out


Because it is not in any way important to the tutorial.


Changed it anyway.


P.S. I'm glad we'r done with the huge things, and down to the minute glihces tough.


 Last edited: Mar 2, 2011

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Tutorial wasn't edited by admin at first place, tutorial moderator did that, and I support that action!

2nd important thing is that tutorial should look like encyclopedia, after all it's created for all people out there, yes even for the dummest WE users. So it need to be error-less.

And for the end we showed you what's wrong. Single lined if then else functions even with Do Nothing trigger actions are far more easier and less complex for use. Your idea was good and ok, but it will fail after 10 different parts of cinematic (You will lost yourself in if then else function, also it will ruin code, shown on your screen).

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I agree with Kobas, but anyway, you can have it your way if you want. I thought I was doing you a favor.

TDA said:

Who on earth Has been playing with my tutorial!?


It no longer looks or feals anything like the tutorial I submited.

What? Is my english not scientific enough for you? you trashed every single trace of personality I put into it.


now it's just an encyclopedia... Nothing else!...


Seriously admin. If your going to do someting like this, you could have given me a heads up.


Atleast have the decency to post when you Drasticly change an important post.


Was my tutorial realy that bad? or do you have a compulsive urge to change the "good" to the "OMG what have you done?"


I understand you might want to make it a bit more scientific...


But you don't have to chance every single sentence into Grey matter and Technobabble.


not every single one.


And if you do this to all tutorials, then freaking tell us in the thread called READ BEFORE POSTING


you have the thread to tell us what to do and what not to do when we submit a tutorial, right???


Well... You should also tell us what your gonna do

Please, the admin who changed my tutorial, I'm not that mad, I have a saved copy of the text, so I havent lost the information.


Just... Post a reply, I would like to know why.


P.S. Admin... I don't like the new page arangement. (puting the subtitles more to the left then the text then the rest of the writhingl)


Makes it look... well... what's the opuset of good?


I would realy like to get the old alighnment back where it belongs.


Click to expand...


I understand that you may have liked the tutorial in its previous format, but some of what you are saying does not make sense. I fixed grammatical issues and clarification. Organization as well. I'm not sure who wants to read a tutorial with broken English. The rules are clearly posted that they must have grammar and all that, and so far no one has complained about me revamping their tutorials to fit the rules. If anything, you should be fortunate that I bother to spend my time to fix every single tutorial before it gets approved.


If you don't like the alignment, you can remove the indents if you want. I added them because that's what I normally do for separation, but you can change it back.

 This tutorial's pourpos isn't to tell you how to make the loose life actions or kill unit action.


Cause if that were so, the tutorial would be twice as long.


That bug is easely fixable by even the dummest of WE users.


I just put it there, and when I noteced it was a glich, I didn't take it out


Because it is not in any way important to the tutorial.


Changed it anyway.


P.S. I'm glad we'r done with the huge things, and down to the minute glihces tough.


Click to expand...


That isn't the point, it is meant to make the reader take into consideration every aspect. You can at least mention it so that people will know what to do, otherwise they will assume to do what you did. If the triggers don't work properly then the tutorial will not be approved.


-------


If you want to change it back, that is fine by me. Just note that tutorials will not be approved without proper indenting for triggers and proper trigger functions. Also, grammar must be in order, at least to a decent extent.

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

Thankyou for your reply, I hope you didn't take my slightly angry post to personaly,


I admit there were places where you did make my words more understandable,


but i think my gramar is alright.


I thank you for your help and asistance, I will try to keep what's best of your work, but I shall change some of the tutorial back to it's original state.


Proper indenting? I tought that the way I aranged it was understanadable, and I find it more estetic then using indent at all. (Do I realy need it to be indented the way you did it?)


I hope there's no harm done, and nothing personal. I didn't mean to do that, and sureley you didn't eather.


I just realy think you should pm the writers of the tutorial b4 you do whatever it is your going to do, because not everybody keeps a backup


and you know...


EDIT:


Ahh what the hec... I'l just let it be the way it is.


 Last edited: Mar 5, 2011

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Changes were made. Nice job on the tutorial. ~Approved.

Also, for future cases, I'll make sure to PM the user before making drastic changes.

EDIT: Not sure where to put this... Would it be better in GUI tutorials or general mapping tutorials? I put it in GUI for now.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Yeah GUI will do

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

This tutorial was made for the GUI Section


So good call.

#### Rui

#### Rui

 Level 41

JoinedJan 7, 2005Messages7,551

Resources: 12

#### Resources
Icons 1Maps 8Tutorials 3

Reopening on request.

#### Restor

#### Restor

 Level 12

JoinedMar 26, 2005Messages790

Resources: 6

#### Resources
Maps 6

usefull tutorial,


it was created for the regular using of waits in qui, but you can still mention the "exact timing using trigger queue system" and its easier escaping system

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

This is not MUI at all. If you reactivate the cinematic before a previous instance has been completed (before the Waits have been run through), it will execute the remaining actions there. That's why you usually take timers instead that you can globally address and stop. But the interesting problem here is actually how to meet the end conditions (like the ones stated in the Kill-Trigger) without stating them another time.

#### The Wizard

#### The Wizard

 Level 6

JoinedAug 27, 2011Messages305

I'm using the system similar to the Legend of Magnador's one.

maybe it's too heavy ('cos there are too many actions), but it requires only 1 variable and that's pretty good for me)

and i should thank BlueEagle)

if noone knows, it uses integer variable "CinemaCount" - it can be hard if you have too much cinematics.

 You must log in or register to reply here.


### Similar threads

 Basics of a Cinematic


Archian

Aug 28, 2006

Trigger (GUI) Editor Tutorials

Replies2Views39K

Oct 4, 2023
raypack

Locked

 Why is this cinematic not working?


Qrom Bellascream

Mar 10, 2016

World Editor Help Zone

Replies2Views1K

Mar 10, 2016
Zunariel

 The cinematic keeps running after it is skipped


Pekisa7

Mar 28, 2026

World Editor Help Zone

Replies4Views268

Mar 29, 2026
Pekisa7

 Cinematic/Tutorial issues on multiplayer


MrFrichyBoy

Nov 4, 2025

Triggers & Scripts

Replies6Views1K

Nov 7, 2025
MrFrichyBoy

Locked

 Help with the Game Camera during and after Cinematics


MrFrichyBoy

Jul 3, 2023

World Editor Help Zone

Replies11Views2K

Jul 19, 2023
Uncle

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