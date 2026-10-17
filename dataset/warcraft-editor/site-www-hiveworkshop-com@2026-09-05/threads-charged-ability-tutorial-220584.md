---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/charged-ability-tutorial.220584/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Charged Ability Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/charged-ability-tutorial.220584/>
#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

#
 Charged Ability Tutorial


By:


Radamantus

What is this?

Answer:


This is a tutorial of how to make an ability with a charge.This allows you to cast the ability many times even without cooldown but has limits and allows you to not duplicate a spell many times and use the action "Add blahblah to blahblah"


and"Set Level of blahblah to blahblah".

How can we make it?

Answer:


The ability triggers include of 4 parts:Initialization,Learn,Cast,Loop.


We will start first in Initialization


We create first the hastable,either of the two you can use:


[trigger=]


 Hashtable - Create a hashtable


 Set Hash = (Last Created Hashtable)[/trigger]


or


[trigger=] Custom script : set udg_Hash = InitHashtable()[/trigger]


There we have it.


Now,we will use some configuration for our spell.


We will start first with the charges color.


I will choose 4 charges as a max charge so that we could use the colors red,orange,yellow,green and if empty black.If you want other colors go here:

https://www.hiveworkshop.com/forums...278/warcraft-iii-color-tags-linebreaks-31386/


[trigger=]


 -------- GENERAL --------


 -------- Determines the max charges --------


 Set MaxCharges = 4


 -------- Determines the color of charges --------


 -------- if 0(Black) --------


 Set Color[0] = |c00000000


 -------- if 1(Red) --------


 Set Color[1] = |c00FF0000


 -------- if 2(Orange) --------


 Set Color[2] = |c00FF7F00


 -------- if 3(Yellow) --------


 Set Color[3] = |c00FFFF00


 -------- if 4(Green) --------


 Set Color[4] = |c0000FF00[/trigger]


There you have it.Next we will use the Floating Text.I will use letter "I" so that we could easily see the Charges.


[trigger=]


 -------- Determines the text for the charges --------


 -------- if 0 --------


 Set ChargeText[0] = (Color[0] + I I I I|r)


 -------- if 1 --------


 Set ChargeText[1] = (Color[1] + (I|r + (Color[0] + I I I|r)))


 -------- if 2 --------


 Set ChargeText[2] = (Color[2] + (I I|r + (Color[0] + I I|r)))


 -------- if 3 --------


 Set ChargeText[3] = (Color[3] + (I I I|r + (Color[0] + I|r)))


 -------- if 4 --------


 Set ChargeText[4] = (Color[4] + I I I I|r)[/trigger]


There,so the charges will look like this:

I I I I

I I I I

I I I I

I I I I

I I I I


After that,we will use the time to make a new charge.Lets use 5 as a cooldown.


[trigger=] Set ChargeCooldown = 5.00[/trigger]


Now,Let's Put the Starting Charge when learned and the charge number when the learner dies.


[trigger=] Set StartingCharge = 1[/trigger]


We are now finish with the Initialization,The trigger will look like this


[trigger=]Initialization


 Events


 Map Initialization


 Conditions


 Actions


 Custom script : set udg_Hash = InitHashtable()


 -------- GENERAL --------


 -------- Determines the max charges --------


 Set MaxCharges = 4


 -------- Determines the color of charges --------


 -------- if 0(Black) --------


 Set Color[0] = |c00000000


 -------- if 1(Red) --------


 Set Color[1] = |c00FF0000


 -------- if 2(Orange) --------


 Set Color[2] = |c00FF7F00


 -------- if 3(Yellow) --------


 Set Color[3] = |c00FFFF00


 -------- if 4(Green) --------


 Set Color[4] = |c0000FF00


 -------- Determines the text for the charges --------


 -------- if 0 --------


 Set ChargeText[0] = (Color[0] + I I I I|r)


 -------- if 1 --------


 Set ChargeText[1] = (Color[1] + (I|r + (Color[0] + I I I|r)))


 -------- if 2 --------


 Set ChargeText[2] = (Color[2] + (I I|r + (Color[0] + I I|r)))


 -------- if 3 --------


 Set ChargeText[3] = (Color[3] + (I I I|r + (Color[0] + I|r)))


 -------- if 4 --------


 Set ChargeText[4] = (Color[4] + I I I I|r)


 -------- Determines the charge cooldown --------


 Set ChargeCooldown = 5.00


 -------- Determines the starting charge --------


 Set StartingCharge = 1[/trigger]


Now let's conclude with the Learn Trigger.This is the shortest trigger here.


Let's make the events and conditions.


[trigger=]Learn


 Events


 Unit - A unit Learns a skill


 Conditions


 (Learned Hero Skill) Equal to (Your Ability)[/trigger]


After that,we will put some important variables.Let's Start with the key(hashtable)


[trigger=]


 Set Learner = (Learning Hero)


 Custom script: set udg_LearnerKey = GetHandleId(udg_Learner)[/trigger]


Now,let's save the variables to the key.


[trigger=]


 -------- Saving --------


 Hashtable - Save (CurrentCharges + StartingCharge) as 0 of LearnerKey in Hash


 Hashtable - Save Timer as 2 of LearnerKey in Hash[/trigger]


The timer will be used for the looping action of the cooldown.


Next,let's start the loop.


[trigger=]


 -------- Starting Loops --------


 Unit Group - Add Learner to LoopGroup


 Trigger - Turn on Loop <gen>[/trigger]


Now we are finish with the Learn Trigger,2 more triggers to go!


Here is what the trigger will look like:


[trigger=]Learn


 Events


 Unit - A unit Learns a skill


 Conditions


 (Learned Hero Skill) Equal to (Your Ability)


 Actions


 Set Learner = (Learning Hero)


 Custom script: set udg_LearnerKey = GetHandleId(udg_Learner)


 -------- Saving --------


 Hashtable - Save (CurrentCharges + StartingCharge) as 0 of LearnerKey in Hash


 Hashtable - Save Timer as 2 of LearnerKey in Hash


 -------- Starting Loops --------


 Unit Group - Add Learner to LoopGroup


 Trigger - Turn on Loop <gen>[/trigger]


Now let's start with the Loop.


So first let's start the loop event.


You could use either 0.03 or 0.04,but ill prefer 0.04.


[trigger=]Loop


 Events


 Time - Every 0.04 seconds of game-time[/trigger]


Now, for the actions.Let's first pick the group


[trigger=] Unit Group - Pick every unit in LoopGroup and do (Actions)


 Loop - Actions[/trigger]


Now,let's load the variables,but first we will create the key.


[trigger=]


 Set Picked = (Picked Unit)


 Custom script : set udg_PickedKey = GetHandleId(udg_Picked)[/trigger]


Now,Loading Variables


[trigger=]


 -------- Load --------


 Set CurrentCharges = (Load 0 of PickedKey from Hash)


 Set Timer = (Load 2 of PickedKey from Hash)[/trigger]


Let's Check if the Picked Unit is Alive,if alive then:


[trigger=]


 -------- Checking --------


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 (Picked is alive) Equal To True


 Then - Actions[/trigger]


Now,let's put the actions for the Then Actions.We will start first with the floating text


[trigger=]


 Then - Actions


 -------- Destroying the currently created floating text --------


 -------- (That means in the last loop) --------


 Floating Text - Destroy (Load (Key FTXT) of PickedKey in HashIf the label is not found, this function returns NULL.)


 -------- Creating Floating Text --------


 Floating Text - Create floating text that reads ChargeText[CurrentCharges] above Picked with Z offset 0.00, using font size 15.00, color (100.00%, 100.00%, 100.00%), and 0.00% transparency


 -------- Saving the Floating Text so that we could destroy it on the next Loop. --------


 Hashtable - Save Handle Of(Last created floating text) as (Key FTXT) of PickedKey in Hash[/trigger]


There we have it,next we will hide the Floating Text from the Enemies so they wont know that you have any charges left.


[trigger=]


 Set Enemies = (All enemies of (Owner of Picked))


 -------- Hiding Floating Text --------


 Floating Text - Hide (Last created floating text) for Enemies


 -------- Clean Leak --------


 Custom script : call DestroyForce(udg_Enemies)[/trigger]


Now,lets conclude with the charge cooldown.


[trigger=]


 -------- Adding Charge --------


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 Timer Greater than or equal to ChargeCooldown


 Then - Actions


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 CurrentCharges Less than MaxCharges


 Then - Actions


 -------- Because it is less than the max,we will add one --------


 Hashtable - Save (CurrentCharges + 1) as 0 of PickedKey in Hash


 Else - Actions


 -------- Reset Timer --------


 Hashtable - Save (Timer x 0.00) as 2 of PickedKey in Hash


 Else - Actions


 -------- Saving Timer --------


 Hashtable - Save (Timer + 0.04) as 2 of PickedKey in Hash[/trigger]


Now,lets make the Else Actions of the Main If/Then/Else


[trigger=]


 Else - Actions


 -------- Destroys Floating Text --------


 Floating Text - Destroy (Load (Key FTXT) of PickedKey in HashIf the label is not found, this function returns NULL.)


 -------- Turns Charges to 1 --------


 Hashtable - Save StartingCharge as 0 of PickedKey in Hash[/trigger]


Noteon't Clean the child hastables or else it will bug!


End of Loop Trigger:


[trigger=]Loop


 Events


 Time - Every 0.04 seconds of game-time


 Conditions


 Actions


 Unit Group - Pick every unit in LoopGroup and do (Actions)


 Loop - Actions


 Set Picked = (Picked Unit)


 Custom script : set udg_PickedKey = GetHandleId(udg_Picked)


 -------- Load --------


 Set CurrentCharges = (Load 0 of PickedKey from Hash)


 Set Timer = (Load 2 of PickedKey from Hash)


 -------- Checking --------


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 (Picked is alive) Equal To True


 Then - Actions


 -------- Destroying the currently created floating text --------


 -------- (That means in the last loop) --------


 Floating Text - Destroy (Load (Key FTXT) of PickedKey in HashIf the label is not found, this function returns NULL.)


 -------- Creating Floating Text --------


 Floating Text - Create floating text that reads ChargeText[CurrentCharges] above Picked with Z offset 0.00, using font size 15.00, color (100.00%, 100.00%, 100.00%), and 0.00% transparency


 -------- Saving the Floating Text so that we could destroy it on the next Loop. --------


 Hashtable - Save Handle Of(Last created floating text) as (Key FTXT) of PickedKey in Hash


 Set Enemies = (All enemies of (Owner of Picked))


 -------- Hiding Floating Text --------


 Floating Text - Hide (Last created floating text) for Enemies


 -------- Clean Leak --------


 Custom script : call DestroyForce(udg_Enemies)


 -------- Adding Charge --------


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 Timer Greater than or equal to ChargeCooldown


 Then - Actions


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 CurrentCharges Less than MaxCharges


 Then - Actions


 -------- Because it is less than the max,we will add one --------


 Hashtable - Save (CurrentCharges + 1) as 0 of PickedKey in Hash


 Else - Actions


 -------- Reset Timer --------


 Hashtable - Save (Timer x 0.00) as 2 of PickedKey in Hash


 Else - Actions


 -------- Saving Timer --------


 Hashtable - Save (Timer + 0.04) as 2 of PickedKey in Hash


 Else - Actions


 -------- Destroys Floating Text --------


 Floating Text - Destroy (Load (Key FTXT) of PickedKey in HashIf the label is not found, this function returns NULL.)


 -------- Turns Charges to 1 --------


 Hashtable - Save StartingCharge as 0 of PickedKey in Hash[/trigger]


Now let's Start with the Cast Trigger


First we will put the events and conditions.


[trigger=]Cast


 Events


 Unit - A unit Starts the effect of an ability


 Conditions


 (Ability being cast) Equal to (Your Ability)[/trigger]


Then The Actions of the Trigger


We will start first with loading and setting important variables.


[trigger=]


 -------- Variables --------


 Set Caster = (Triggering unit)


 Custom script: set udg_CasterKey = GetHandleId(udg_Caster)


 Set CurrentCharges = (Load 0 of CasterKey from Hash)[/trigger]


Now,let's check if the charges is not 0 or empty.


[trigger=]


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 CurrentCharges Greater than 0[/trigger]


Now for the Then Actions


[trigger=]


 Then - Actions


 -------- Decrease a charge --------


 -------- But first we will check if the charges is greater than 0 so it wont bug --------


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 Current Charges Greater than 0


 Then - Actions


 Hashtable - Save (CurrentCharges - 1 ) as 0 of CasterKey in Hash


 Else - Actions[/trigger]


Now for the Else Actions for the Main If/Then/Else


[trigger=]


 Else - Actions


 -------- Disables casting --------


 Set Levels = (Level of (Your Ability) for Caster)


 Unit - (Your Ability) from Caster


 Unit - Add (Your Ability) to Caster


 Unit - Set level of (Your Ability) for Caster to Levels


 -------- Text that Indicates you have no more charges left. -------


 Set Self = (Player group(Triggering Player))


 Game - Display to Self the text: |cffffcc00No More Charges Left.


 Custom script: call DestroyForce(udg_Self)[/trigger]


Now we are done with the Cast Trigger.


Here is the overall trigger:


[trigger=]Cast


 Events


 Unit - A unit Starts the effect of an ability


 Conditions


 (Ability being cast) Equal to (Your Ability)


 Actions


 -------- Variables --------


 Set Caster = (Triggering unit)


 Custom script: set udg_CasterKey = GetHandleId(udg_Caster)


 Set CurrentCharges = (Load 0 of CasterKey from Hash)


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 CurrentCharges Greater than 0


 Then - Actions


 -------- Decrease a charge --------


 -------- But first we will check if the charges is greater than 0 so it wont bug --------


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 Current Charges Greater than 0


 Then - Actions


 Hashtable - Save (CurrentCharges - 1 ) as 0 of CasterKey in Hash


 Else - Actions


 Else - Actions


 -------- Disables casting --------


 Set Levels = (Level of (Your Ability) for Caster)


 Unit - (Your Ability) from Caster


 Unit - Add (Your Ability) to Caster


 Unit - Set level of (Your Ability) for Caster to Levels


 -------- Text that Indicates you have no more charges left. -------


 Set Self = (Player group(Triggering Player))


 Game - Display to Self the text: |cffffcc00No More Charges Left.


 Custom script: call DestroyForce(udg_Self)[/trigger]


For the Demo Map:

https://www.hiveworkshop.com/forums/spells-569/shadow-shroud-v7-219496/


Ask any questions if you wanted to.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Notes:

Ability icons for the charges are very complex and uses MANY gui triggers and Jass Codes.

I Prefer to use this one for a charged spell due to that this is easier than icon replacement.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

^No, it's just an array of abilities that you have to add and remove.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

My bad just checked the name, but then its good

There is allready a tutorial about this actually, it shows how to use a knockback spell with hashtables and thats basicly the same thing as charge

https://www.hiveworkshop.com/forums/trigger-gui-editor-tutorials-279/hashtables-mui-133407/


 Last edited: Aug 10, 2012

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Uhmm,about that sir,its not about the spell with "charging" or some sort of knockback.It is about an ability just like items with limits

Please read how the spell works first.

About that Mag,it will take me 10 triggers and repeating them due to the icons.It will be a pain in the object editor

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

sorry my bad

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice job. Everything seems in order to me. Sorry for the late response.

~Approved.

#### neku99

#### neku99

 Level 10

JoinedSep 3, 2009Messages458

Resources: 4

#### Resources
Spells 3Tutorials 1

whoa I've been figuring out how to do this, finally ahahahha thanks for this :3 rep+!!!!!

#### ExecutorWar

#### ExecutorWar

 Level 4

JoinedNov 11, 2012Messages132

Haha! this one is too complex for me. But later on i guess this will be useful. I got so many ideas about charged spells

#### Legal_Ease

#### Legal_Ease

 Level 20

JoinedFeb 8, 2013Messages1,476

Resources: 33

#### Resources
Icons 13Packs 1Skins 3Maps 11Templates 1Tutorials 4

Charged Spell System v.01.w3x

So here is another version. I've gone about as far as I can with this. It's not MUI and it is hard coded, but it works and it's pretty neat. If anyone wants it or has any use for it, please feel free to make it your own. Also, if anyone is serious about using it, I would be happy to make additional numbered icons. It can most likely be used to be integrated with Almia's hastable system and then it will be MUI. The only thing my system really adds is the icons and icon swapping. I just thought this might be more helpful here than in the Help Zone post.

Cast
Events
Unit - A unit Is issued an order with no target

Conditions
Or - Any (Conditions) are true
Conditions
(Issued order) Equal to (Order(manashieldon))

(Issued order) Equal to (Order(manashieldoff))

(Issued order) Equal to (Order(unimmolation))

Actions
Set Loc = (Position of Caster)

Set Angle = (Facing of Caster)

Animation - Play Caster's spell animation

Unit - Create 1 Dummy_type for Player 1 (Red) at Loc facing Angle degrees

Unit - Add Shockwave (Neutral Hostile) to (Last created unit)

Unit - Turn collision for (Last created unit) Off

Unit - Order (Last created unit) to Orc Tauren Chieftain - Shockwave (Loc offset by 300.00 towards Angle degrees)

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Unit - Hide (Last created unit)

Custom script: call RemoveLocation(udg_Loc)

Countdown Timer - Start Clock as a Repeating timer that will expire in 4.00 seconds

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Issued order) Equal to (Order(manashieldoff))

(Issued order) Equal to (Order(unimmolation))

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
int Less than 3

Then - Actions
Trigger - Turn off Cast <gen>

Unit - Remove Ability[int] from Caster

Set int = (int + 1)

Unit - Add Ability[int] to Caster

Unit - Order Caster to Neutral Naga Sea Witch - Activate Mana Shield

Unit - Order Caster to Night Elf Demon Hunter - Activate Immolation

Trigger - Turn on Cast <gen>

Else - Actions
Trigger - Turn off Cast <gen>

Unit - Remove Ability[int] from Caster

Unit - Add Ability[int] to Caster

Set int = (int + 1)

Unit - Order Caster to Neutral Naga Sea Witch - Activate Mana Shield

Trigger - Turn on Cast <gen>

Else - Actions

Reset Charges
Events
Time - Clock expires

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
int Equal to 1

Then - Actions
Countdown Timer - Start Clock as a Repeating timer that will expire in 4.00 seconds

Countdown Timer - Pause (Last started timer)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
int Equal to 4

Then - Actions
Trigger - Turn off Cast <gen>

Set int = (int - 1)

Unit - Remove Ability[int] from Caster

Unit - Add Ability[int] to Caster

Unit - Order Caster to Night Elf Demon Hunter - Activate Immolation

Unit - Order Caster to Neutral Naga Sea Witch - Activate Mana Shield

Trigger - Turn on Cast <gen>

Else - Actions
Trigger - Turn off Cast <gen>

Unit - Remove Ability[int] from Caster

Set int = (int - 1)

Unit - Add Ability[int] to Caster

Unit - Order Caster to Night Elf Demon Hunter - Activate Immolation

Unit - Order Caster to Neutral Naga Sea Witch - Activate Mana Shield

Trigger - Turn on Cast <gen>

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

I made the same thing, because of you ringing a bell to my old intention that i lost.

you can check that in the jass submission section(sorry can't link it, currently using a mobile phone)

#### Legal_Ease

#### Legal_Ease

 Level 20

JoinedFeb 8, 2013Messages1,476

Resources: 33

#### Resources
Icons 13Packs 1Skins 3Maps 11Templates 1Tutorials 4

Damn mobile phones! I want the link  I think your system is worthy of a spell forum submission.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

ChargedAbility

 You must log in or register to reply here.


### Similar threads

 State-based Passive Ability Tutorial


Blightsower

Sep 22, 2024

Trigger (GUI) Editor Tutorials

Replies8Views3K

Sep 23, 2024
Blightsower

 How to make Professions


Airandius

Sep 14, 2008

Trigger (GUI) Editor Tutorials

Replies43Views18K

Feb 14, 2010
Dark hamster

Locked

[Trigger]
 Hashtable spell


Jay the Editor

Jul 31, 2011

Triggers & Scripts

Replies4Views2K

Jul 31, 2011
Jay the Editor

Locked

[Trigger]
 Item Charges Problem.


Arisato

Nov 2, 2012

Triggers & Scripts

Replies1Views1K

Nov 3, 2012
Arisato

Locked

 2 very big problems!


CoLd Bon3

Dec 5, 2010

World Editor Help Zone

Replies9Views2K

Dec 5, 2010
mckill2009

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