---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/getlocalplayer.225738/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Getlocalplayer
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/getlocalplayer.225738/>
#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

#
 GetLocalPlayer

###
 By Chaosy

##
 Introduction

I made this tutorial since I think way to few people know that this exist in GUI, so I want to show you how to use "GetLocalPlayer" in GUI.​

##
 What is needed?

Decent knowlege of GUI triggering

Very basic jass knowledge

A few moments of your time

##
 What is GetLocalPlayer?

GetLocalPlayer is a method to create certain things/stuff in JASS, but I will show you how to use this method in the GUI.​

This method will allow you to create this thing/stuff [special effects, multiboards, floating text, etc] for the player of your choice only.​

For example, you can show a multiboard to player 1 only, while the other player wont see any multiboard at all!​

##
 That sounds awesome how do I use this?

As one example I will create a multiboard for player 1 only, you can use any event you want.​

I will show you 3 ways to do it, tho you should use the recommended one since you use less memory that way.​

NOTICE! We use a player variable named "GetLocalPlayer" or "Player" based on which way you want to use​

init
Events
Map initialization

Conditions

Actions
Custom script: set udg_GetLocalPlayer = GetLocalPlayer()

Multiboard for one player
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Multiboard - Create a multiboard with 1 columns and 1 rows, titled hellu

Multiboard - Hide (Last created multiboard)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
GetLocalPlayer Equal to Player 1 (Red)

Then - Actions
Multiboard - Show (Last created multiboard)

Else - Actions

Multiboard for one player jass
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Multiboard - Create a multiboard with 1 columns and 1 rows, titled hellu

Multiboard - Hide (Last created multiboard)

Custom script: if GetLocalPlayer() == Player(0) then

Multiboard - Show (Last created multiboard)

Custom script: endif

Multiboard for one player
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Multiboard - Create a multiboard with 1 columns and 1 rows, titled hellu

Multiboard - Hide (Last created multiboard)

Custom script: set udg_Player = GetLocalPlayer()

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Player Equal to Player 1 (Red)

Then - Actions
Multiboard - Show (Last created multiboard)

Else - Actions

Now I will go through the trigger

First we create a multiboard just edit it as you want

Multiboard - Create a multiboard with 1 columns and 1 rows, titled hellu
after that we instantly hide it so no player never saw that multiboard

Multiboard - Hide (Last created multiboard)
Now the the hard part, now we use a custom script since the action does not exist within the GUI. It stores the player returned from the GetLocalPlayer() function inside your player variable.

Custom script: set udg_Player = GetLocalPlayer()
And lastly we just select wich players we want it to be showed to, in this case it will ONLY be showed to player red (1) aint that awesome?

NOTICE! The jass and the GUI version is just the same you can use any of them!

NOTICE! In jass the player index is between in 0-11 insted of 1-12 in GUI

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Player Equal to Player 1 (Red)

Then - Actions
Multiboard - Show (Last created multiboard)

Else - Actions

Custom script: if GetLocalPlayer() == Player(0) then

Multiboard - Show (Last created multiboard)

Custom script: endif

##
 Great, but can I do the same to other things?

Yes you really can, lets take special effects.

init
Events
Map initialization

Conditions

Actions
Custom script: set udg_GetLocalPlayer = GetLocalPlayer()

special effect trollbrain way
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Set Effect = <Empty String>

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
GetLocalPlayer Equal to Player 1 (Red)

Then - Actions
Set Effect = Abilities\Spells\Other\TalkToMe\TalkToMe.mdl

Else - Actions

Special Effect - Create a special effect attached to the overhead of Peasant 0001 <gen> using Effect

Special effect for one player jass
Events
Player - Player 1 (Red) Presses the Right Arrow key

Player - Player 2 (Blue) Presses the Right Arrow key

Conditions

Actions
Set Effect = <Empty String>

Custom script: set udg_Player = GetLocalPlayer()

Custom script: if GetLocalPlayer() == Player(0) then

Set Effect = Abilities\Spells\Human\DispelMagic\DispelMagicTarget.mdl

Custom script: endif

Special Effect - Create a special effect attached to the overhead of Peasant 0001 <gen> using Effect

Special effect for one player
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Set Effect = <Empty String>

Custom script: set udg_Player = GetLocalPlayer()

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Player Equal to Player 1 (Red)

Then - Actions
Set Effect = Abilities\Spells\Other\TalkToMe\TalkToMe.mdl

Else - Actions

Special Effect - Create a special effect attached to the overhead of Peasant 0001 <gen> using Effect

As you see I use the same system, but I will explain the new actions for you anyway.

Here I simply store a string variable into nothing, this means if the player aint the player I want it to be there wont be any effect. (you will see later)

Set Effect = <Empty String>
Here We check if the player is player red (1) and then we set the string variable to the question mark effect path.

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Player Equal to Player 1 (Red)

Then - Actions
Set Effect = Abilities\Spells\Other\TalkToMe\TalkToMe.mdl

Else - Actions

And after that we create the effect simple as that.

Special Effect - Create a special effect attached to the overhead of Peasant 0001 <gen> using Effect

##
 Desyncronization

Desyncronization only exist in multiplayer due you need both a host and a client to make this happen, it causes clients playing the map do disconnect from the game. Thats why you must keep this in mind, if possible do only use the GetLocalPlayer in conditions.​

So if it does desync do NOT use it, if it does not desync you can just enjoy the game.​

​

NOTICE! Handles that have their own stack will never desync!​

1. Fade filters for 1 player only

Question - this cause desyncronization?

Answer - No, but you have to locally set the alpha (transparency) rather than putting the fade filter in the "if" block.

Click this link for the technique.

2. Quest for 1 player only?

Question - this cause desyncronization?

Answer - Yes but if u use Enable/Disable quest then No

3. Special effects, visibile for 1 player only?

Question - this cause desyncronization?

Answer - No

4. Special effects attachment to unit, visibile for 1 player only?

Question - this cause desyncronization?

Answer - No

6. Unit visibility, color, visibile for 1 player only?

Question - this cause desyncronization?

Answer - Yes but Vertex color dont desync

7. Fog of war and black mask, color, visibile for 1 player only?

Question - this cause desyncronization?

Answer - Yes

8. Images, visibile for 1 player only?

Question - this cause desyncronization?

Answer - No

9. Sound & music, for 1 player only?

Question - this cause desyncronization?

Answer - Mostly No

10. Show Multiboard, for 1 player only?

Question - this cause desyncronization?

Answer - No

11. Trackables, for 1-2 player only?

Question - this cause desyncronization?

Answer - No

12. Pause the game, for 1 player only?

Question - this cause desyncronization?

Answer - Yes

13. Create different unit for player?

Question - this cause desyncronization?

Answer - Yes

Note - It might not desync if (1) it is a non hero unit and (2) the two units have only differences in art fields/physical display (such as model or icon) - by PurgeandFire111

14. Weather effect off for 1 player?

Question - this cause desyncronization?

Answer - No?

15. Desctructible visibility, for 1 player only?

Question - this cause desyncronization?

Answer - ?

##
 Credits

shadowvzs - GetLocalPlayer() faq​

Hashjie - Telling me info about GetLocalPlayer and the idea to this tutorial​

Troll-Brain - awesome tip to improve the tutorial​


 Last edited: Jun 22, 2021

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

Hmm, I think you should explain more about desynchronizations in detail.

People don't really know what this is or what it does. Same counts for GetLocalPlayer().

It's not really clear when you say:

 GetLocalPlayer is a function in JASS wich allows you to make multiboards,floating texts, special effects and more.


Click to expand...

There is a bit more truth to this part:

 In other words things that are meant to be seen by every player, but with GetLocalPlayer you can make it be shown to one player only or maybe two players only.


Click to expand...

But I think it's better to explain that GetLocalPlayer() is a function in Jass which returns a local player (which can be used to save it inside a variable like you are doing or by putting it in a condition).

This can then be used to create or display (depending on wether the handle has it's own counter) handles (objects) locally to a player. So that only 1 player can see it (local).

This explenation needs to be adjusted a bit but it would be more correct so to say.

Also, you might want to fix some of your grammar mistakes. I can see how some people might not be abled to understand what you are trying to say.

 It basicly stores my player Variable into the GetLocalPlayer.


Click to expand...

This is wrong. It stores the player returned from the GetLocalPlayer() function inside your player variable.

Furthermore, some things should probably be tested with other players before submitted.

For example: GetLocalPlayer returns a player which is set inside the global variable, however I'm not sure if this could potentially cause problems since GetLocalPlayer returns a (local) player...

If it does work you need to explain that setting GetLocalPlayer() to a global GUI variable does not mean that you can use that global variable inside GUI actions and that it should solely be used for conditions to check if a player equals the variable.

It might also be nice to tell people how you can check for desynchronizations?

For example commenting out/removing lines which might cause a desynch inside local blocks, displaying debug messages of handle ID's.

Maybe give an example of what happens to the handle ID's of floating texts and explain why?

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

GetLocalPlayer is a function in JASS wich allows you to make multiboards,floating texts, special effects and more.

--> I think you better say first what GetLocalPlayer actually does, and not go directly to the possibilities of its usage... because reading this, someone might think that GetLocalPlayer() does all those by default, when the fact is that they are only methods in which you can utilize what GetLocalPlayer() does...

also,

GetLocalPlayer makes "things" meant to be seen by every player, but with GetLocalPlayer you can make it be shown to one player only or maybe two players only.

--> The first statement says that GetLocalPlayer makes things meant to be seen by every player, then the second one says that it makes it shown to only a few... Here you will see that your two statements contradict each other... The first is false, first and foremost because GetLocalPlayer() doesn't make things...

it will better be off as:

GetLocalPlayer() allows you to make things [effects,variable data, etc] be created for a specific player/s only when utilized...

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

@hashjie, I will try to add as much as I can, thanks for your comment

@adiktuz, I know that you are right here, I will change it in a moment thank you for your comment

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

other than those, this will be helpful for those who needs the power of GLP but haven't come across it yet...

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

@Hashjie : There is nothing wrong to give different values to a global (or local) variable, you can even do this.

To make it even more "GUI", you just have to use a GUI player variable (variable editor) and set it on map initialization with a custom script.

Here i name this variable " Local_player "

set udg_Local_player = GetLocalPlayer()

And then you can use and if/then in GUI, you don't need more custom scripts.

But yeah of course, since its value will be different for each player's computer, you have to care on how you use it.

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

Yeah, you're right Troll-Brain, I wasn't really thinking clearly.

Setting it to a global variable will work just as good but people just need to be carefull using it correctly. I agree with you.

Also, chaosy just tested it with me and there are indeed no problems in using global GUI variables.

But I can imagine someone trying to create a floating text locally in GUI and then still getting desync's due to CreateTextTagLocBJ.

A friend of mine tested this and told me it desync'd. (might have something to do with bj_lastCreatedTextTag?) I'll create a test later on to check if he was correct.

If so text tags should be created locally through jass. Same thing counts for, for example SmartCameraPanBJ.

I'm just trying to say that using GetLocalPlayer() in a "GUI way" doesn't nessecarely mean that it will work just as well as when using jass.

Also, lots of BJ's already have a GetLocalPlayer() check so it's not needed for those.


 Last edited: Nov 18, 2012

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Thanks for your idea troll-brain, I will post it in the tutorial as soon I get it working as I want it


 Last edited: Nov 18, 2012

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

If/then in jass or GUI are the same, excepted that conditions of the if/then in GUI are done in other functions.

Which makes annoying the use of a local variable in GUI, you have to use a temp global variable if you want to use a local variable in a GUI if/then condition.

At least it's the case for the if/then multiple, i don't remember for the single one, but i should be the same.

Now, many and many GUI actions uses and eventually creates handles, such as locations, so ofc if you create an handle in a local block, then you will got a desync (excepted few types such as texttags).

Also some GUI functions leaks inherently, such as PolledWait.

So no it's not the fault of the if/then in GUI.

Finally i don't think that "lot of" GUI functions already use GetLocalPlayer(), but i could be wrong about this last one, it was several years ago that i used GUI ...

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

@Troll-Brain I hope you was not talking to me since I do not understand much

anyway I added your tip to the tutorial and added you to credits

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

No it was for Hashjie, even if you should understand what i said in order to use GetLocalPlayer() safely.

But meh, i suppose a list of examples is enough for GUIers, such as how it's done in the "FAQ".

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

Troll-Brain said:

 If/then in jass or GUI are the same, excepted that conditions of the if/then in GUI are done in other functions.

Which makes annoying the use of a local variable in GUI, you have to use a temp global variable if you want to use a local variable in a GUI if/then condition.

At least it's the case for the if/then multiple, i don't remember for the single one, but i should be the same.


Click to expand...

Yes, this is indeed a disadvantage of using GUI.

In fact I've seen this lots of times before, ugly spagetti code created through lots of if/then's in GUI. But I don't really see your point being on this subject?

Troll-Brain said:

 Now, many and many GUI actions uses and eventually creates handles, such as locations, so ofc if you create an handle in a local block, then you will got a desync (excepted few types such as texttags).


Click to expand...

That's why I tryed to make a clear point to warn other users not to think that just by using a local block in GUI means they can make things work locally.

It's also the GUI actions eg BJ's they will have to be careful with.

Sometimes a BJ simply contains code that will not work in local blocks (code such as creating handles) when done in GUI but will work when using the non BJ version through jass.

Troll-Brain said:

 Also some GUI functions leaks inherently, such as PolledWait.


Click to expand...

I didn't know about this, thanks! Do you perhaps have any refference on this subject?

I would like to know how this was discovered. I'll search for it myself in the meanwhile, I suppose it's got somethng to do with the TSA's?

Troll-Brain said:

 So no it's not the fault of the if/then in GUI.


Click to expand...

I never said it was did I?

Troll-Brain said:

 Finally i don't think that "lot of" GUI functions already use GetLocalPlayer(), but i could be wrong about this last one, it was several years ago that i used GUI ...


Click to expand...

I thought there where quite some BJ's which use local blocks but I'm not entirely sure about that. Plus the fact that "lot of" is not really a measurement. Perhaps I find 2 functions a lot already

#### TKF

#### TKF

 Level 19

JoinedNov 29, 2006Messages1,267

Resources: 8

#### Resources
Skins 1Maps 6Spells and Systems 1

How about putting generic sounds in local to happen only to a few players? I know it doesn't cause instant desync, but I suspect it sometimes causes players to desync. I'm right? Or is it host bot related?

Otherwise I use only multiboards and texts in local

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

@TKF, thats a good question I will test this later

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

TKF said:

 How about putting generic sounds in local to happen only to a few players? I know it doesn't cause instant desync, but I suspect it sometimes causes players to desync. I'm right? Or is it host bot related?

Otherwise I use only multiboards and texts in local


Click to expand...

You mean create or play ?

I don't know about create but play should be fine.

Anyway it's already local, since a sound is not played if you have turned off the sound in the wc3 menu, and some other cases such as if wc3 is minimized or not.

@Hashjie :

Well, it's just because the way you said it, the desync was coz of the GUI if, that's why i made it clear it was not.

The local thing was just a bonus, as i like to be accurate, but yeah that's off-topic.

 I didn't know about this, thanks! Do you perhaps have any refference on this subject?


Click to expand...

blizzard.j

 I would like to know how this was discovered. I'll search for it myself in the meanwhile, I suppose it's got somethng to do with the TSA's?


Click to expand...

No it's because it destroys a timer but not null the local variable -> handle id leak

Well, i've seen it myself but anyway the leak might has been discovered just by browsing blizzard.j

And leak or not using a TSA or a timer in a local code should desync by itself.

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

Troll-Brain said:

 You mean create or play ?

I don't know about create but play should be fine.

Anyway it's already local, since a sound is not played if you have turned off the sound in the wc3 menu, and some other cases such as if wc3 is minimized or not.

@Hashjie :

Well, it just because the way you said it, the desync was coz of the GUI if, that's why i made it clear it was not.

The local thing was just a bonus, as i like to be accurate, but yeah that's off-topic.

blizzard.j

No it's because it destroys a timer but not null the local variable -> handle id leak

Well, i've seen it myself but anyway the leak might has been discovered just by browsing blizzard.j


Click to expand...

Ah, how stupid of me xD I haven't really looked that much inside blizzard.j tbh I think I'll go through the entire thing some time.

EDIT: Yet another fail of me, I totally forgot that blizzard.j is integrated in the function list of JNGP.

@TKF:

There is a BJ that does the following:

 JASS:

function StartSoundForPlayerBJ takes player whichPlayer, sound soundHandle returns nothing
 if (whichPlayer == GetLocalPlayer()) then
 call StartSound(soundHandle)
 endif
endfunction

So yeah I guess that you can start a sound locally but need to create the handle globally.

(Remember not to use a BJ if it's not nessecary, it's yet another function call for something that can be done directly instead.)


 Last edited: Nov 18, 2012

#### TKF

#### TKF

 Level 19

JoinedNov 29, 2006Messages1,267

Resources: 8

#### Resources
Skins 1Maps 6Spells and Systems 1

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Remaining time for WarningTimer[1]) Less than or equal to 0.00

Then - Actions
Set Temp_Group3 = (Units in Cruiser 2 Interior <gen>((Unit-type of (Matching unit)) Equal to Crew Member))

Unit Group - Pick every unit in Temp_Group3 and do (Actions)
Loop - Actions
Set localplayer = (Owner of (Picked unit))

Custom script: if udg_localplayer == GetLocalPlayer() then

Game - Display to (Player group(localplayer)) the text: localstring

Sound - Play alarm <gen>

Custom script: endif

Custom script: call DestroyGroup(udg_Temp_Group3)

Countdown Timer - Start WarningTimer[1] as a One-shot timer that will expire in 3.00 seconds

Else - Actions

Does this work?

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Remaining time for WarningTimer[0]) Less than or equal to 0.00

Then - Actions
Set Temp_Group2 = (Units in Cruiser 1 Interior <gen>((Unit-type of (Matching unit)) Equal to Crew Member))

Unit Group - Pick every unit in Temp_Group2 and do (Actions)
Loop - Actions
Set Temp_PlayerGroup = (Player group((Owner of (Picked unit))))

Game - Display to Temp_PlayerGroup the text: localstring

Custom script: call DestroyForce(udg_Temp_PlayerGroup)

Set localplayer = (Owner of (Picked unit))

Custom script: if udg_localplayer == GetLocalPlayer() then

Sound - Play alarm <gen>

Custom script: endif

Custom script: call DestroyGroup(udg_Temp_Group2)

Countdown Timer - Start WarningTimer[0] as a One-shot timer that will expire in 3.00 seconds

Else - Actions

Or this a safer approach?

Sorry, I'm only used to GUI. Will this way of setting up local message cause desync? I'm trying will fail and error in last 10 versions, however I cannot test it myself since it only happens with more than 1 player.

But I would prefer alarm sound for that one team only, not the other.

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

I don't know if it's safe to open a new thread in a local block, just as ForForce does. (the display text force thing)

It probably leads to a desync.

#### TKF

#### TKF

 Level 19

JoinedNov 29, 2006Messages1,267

Resources: 8

#### Resources
Skins 1Maps 6Spells and Systems 1

Troll-Brain said:

 I don't know if it's safe to open a new thread in a local block, just as ForForce does. (the display text force thing)

It probably leads to a desync.


Click to expand...

I don't completely understand. I don't understand JASS language.

Are you referring to Temp_PlayerGroup? Destroying it every pick causes desync?

Or are you referring ForForce on something in trigger 1 or 2?

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

I'm talking about that :

Game - Display to Temp_PlayerGroup the text: localstring

Since i suppose that it uses ForForce.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

ok, added a few "NOTICE!" wich will help the reader

edit;

NOTICE! We use a player variable named "GetLocalPlayer" or "Player" based on wich way you want to use

NOTICE! The jass and the GUI version is just the same you can use any of them!

NOTICE! In jass the player index is between in 0-11 insted of 1-12 in GUI

NOTICE! Handles that have their own stack will never desync!


Click to expand...


 Last edited: Nov 20, 2012

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

That will hardly help, i guess that yourself don't understand your notice.

EDIT :

Ok there are several notices, i just saw this one :

 NOTICE! Handles that have their own stack will never desync!


Click to expand...

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

NOTICE! We use a player variable named "GetLocalPlayer" or "Player" based on wich way you want to use

NOTICE! The jass and the GUI version is just the same you can use any of them!

NOTICE! In jass the player index is between in 0-11 insted of 1-12 in GUI

NOTICE! Handles that have their own stack will never desync!


Click to expand...

I know they are not very helpful but useful in some cases

edit, yay 1100 posts

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

uhm,


this:

Custom script: set udg_Player = GetLocalPlayer()

Custom script: if GetLocalPlayer() == Player(0) then

Set Effect = Abilities\Spells\Human\DispelMagic\DispelMagicTarget.mdl

Custom script: endif
should be

Custom script: if GetLocalPlayer() == Player(0) then

Set Effect = Abilities\Spells\Human\DispelMagic\DispelMagicTarget.mdl

Custom script: endif
because udg_Player was never used

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

How stupid of me, thanks

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Troll-Brain said:

 I'm talking about that :


Game - Display to Temp_PlayerGroup the text: localstring


Since i suppose that it uses ForForce.


Click to expand...


It does not.

 JASS:

function DisplayTextToForce takes force toForce, string message returns nothing
 if (IsPlayerInForce(GetLocalPlayer(), toForce)) then
 // Use only local code (no net traffic) within this block to avoid desyncs.
 call DisplayTextToPlayer(GetLocalPlayer(), 0, 0, message)
 endif
endfunction


Yes, opening new threads locally does desync. I think it takes a new id in the shared handle stack. You should be able to do something like

 JASS:

if localBoolean then
 call ForForce(f, function a)
else
 call ForForce(f, function b)
endif


though

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

You can hide items locally:

Hide Item
Events
Map initialization

Conditions

Actions
Custom script: if GetLocalPlayer() == Player(0) then

Item - Hide Claws of Attack +15 0000 <gen>

Item - Hide Claws of Attack +15 0001 <gen>

Custom script: endif

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Are you sure that won't desync when the player that can see it picks it up? Or even pathing? Generally widgets are a bit troublesome with GetLocalPlayer() due to the player interactions.

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

PurgeandFire111 said:

 Are you sure that won't desync when the player that can see it picks it up? Or even pathing? Generally widgets are a bit troublesome with GetLocalPlayer() due to the player interactions.


Click to expand...

I'm going to test other known desyncs with the kLoader program to see if I can get a desync even if both instances are on the same PC, then I'll go ahead and test this for desync. In the initial test, though, it worked fine.

EDIT: So it works - you can test desyncs on one PCs.

https://www.hiveworkshop.com/forums...multiple-warcraft-3-running-single-pc-226287/

EDIT2: Did some more tests.

Terrain deformation - temporary/permanent - desyncs

Pause day/night cycle - desyncs

Lightning effect - desyncs immediately

Animation - does not desync

Change terrain type - desyncs

EDIT3: Custom lights.

test
Events
Player - Player 1 (Red) types a chat message containing test as An exact match

Conditions

Actions
Custom script: if GetLocalPlayer() == Player(0) then

Custom script: call SetDayNightModels("","")

Custom script: endif

Doesn't desync.


 Last edited: Nov 29, 2012

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

Why are you using so many custom scripts in this GUI tutorial?

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

maybe because GetLocalPlayer() does not exist in the GUI selection...

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Just do

Events
Map Initialization

Conditions

Actions
Custom script: set udg_LocalPlayer = GetLocalPlayer()

so you can refer to the local player via the GUI variable (create player variable "LocalPlayer" in GUI), therefore do not need more jass lines or expressions.

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

Adiktuz said:

 maybe because GetLocalPlayer() does not exist in the GUI selection...


Click to expand...


But if GetLocalPlayer does not exist in the GUI section, what is this tutorial doing in the GUI section?

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

you see, I said "selection" not "section", better read carefully mate...

When I say GUI selection, I mean the drop down list that appears when you click Add Action, surely you don't see GetLocalPlayer() there...

Also, Custom script action is a part of GUI you know... So basically, this tutorial is still a GUI tutorial...

I think that is easy to understand eh?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I will try to explain, GUI is a slower version of JASS. In GUI you can't use GetLocalPlayer if you dont use JASS within the GUI, The tutorial is to meant to show how to use the GetLocalPlayer for GUI users and is clearly focused on GUI not JASS

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

Chaosy said:

 I will try to explain, GUI is a slower version of JASS. In GUI you can't use GetLocalPlayer if you dont use JASS within the GUI, The tutorial is to meant to show how to use the GetLocalPlayer for GUI users and is clearly focused on GUI not JASS


Click to expand...


If this is so, wouldn't a more useful tutorial be "How and when to use a custom script"?

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

this tutorial is a really useful one assuming you realize how many things you can do with the GetLocalPlayer() thingy...

Making a how and when to use a CS is as simple as this one:

 When to use a CS?

-> When you need to use a JASS line inside a GUI trigger

How to use a CS?

1)Click the trigger

2)Right click on the trigger's GUI space and choose add action

3)Choose Custom script from the dropdown list

4)Write your jass line


Click to expand...

Also, that is a pretty basic triggering concept... So it's not really a good idea to make a whole tutorial for that... And I believe that it is already (or should be) a part of the basic triggering tutorials here instead...

Have some experience before suggesting will be better for you...

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Its not ANY custom script anyone can add asnd it reads your mind. Its about how to use GetLocalPlaye.

Edit: how are you allways faster lol

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Only during weekends maybe... I don't have internet connection at the dorm and I don't really like going to net cafes just to browse for a while... XD...

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

Adiktuz said:

 this tutorial is a really useful one assuming you realize how many things you can do with the GetLocalPlayer() thingy...

Making a how and when to use a CS is as simple as this one:

Also, that is a pretty basic triggering concept... So it's not really a good idea to make a whole tutorial for that... And I believe that it is already (or should be) a part of the basic triggering tutorials here instead...

Have some experience before suggesting will be better for you...


Click to expand...

Doesn't this tutorial exist in the JASS section, then?

Still, how is this GUI?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Those questions seems kind of stupid to me actually...


This is a normal tigger its called GUI

trigger name

Events

Conditions

Actions
this is JASS or vJASS

 JASS:

function functionname takes nothing returns nothing
endfunction


and in the GUI there is no such thing as GetLocalPlayer and I show to how to get the JASS native GetLocalPlayer into the GUI. so tell me how can it not be suiting in the GUI tutorials?

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

Chaosy said:

 Those questions seems kind of stupid to me actually...


Click to expand...

Nah it's not stupid, I get his perspective...

Chaosy said:

 This is a normal tigger its called GUI

trigger name

Events

Conditions

Actions

Click to expand...

Damn right it is!

Chaosy said:

 this is JASS or vJASS

 JASS:

function functionname takes nothing returns nothing
endfunction

Click to expand...

Nope, just JASS ;P

Chaosy said:

 and in the GUI there is no such thing as GetLocalPlayer and I show to how to get the JASS native GetLocalPlayer into the GUI. so tell me how can it not be suiting in the GUI tutorials?


Click to expand...

Let me rephrase that for him:

If GetLocalPlayer() only exists in Jass, then how can this be a GUI tutorial?

But then again... It's in the GUI section because it effectively shows you how to put the return of a Jass function inside a GUI variable.

Which can then be used for local code without having to use custom script actions over and over again in GUI. Which can be a useful refference for GUI users.

(If you've come this far as a GUI user, I don't really see the point in not learning Jass anyways then :/)

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

Hashjie said:

 But then again... It's in the GUI section because it effectively shows you how to put the return of a Jass function inside a GUI variable.


Click to expand...

Custom script: set udg_global_variable = JASSFunctionReturn()
How hard is that?

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

frettory said:

Custom script: set udg_global_variable = JASSFunctionReturn()
How hard is that?


Click to expand...

Depends... How much do you know about GUI?

I'm just saying, this stuff is simple cake for almost anyone but you'll never know who finds it useful.

Plus the fact that not every GUI'er thinks of this solution for using local code.

It's also good to have idiot proof things...

Did you know that there was a women in the US who had fried her dog with a microwave just because the dog went outside in the rain. And afterwards sued the company who made the microwave because it killed the dog since it wasn't in the user manual that you should not put your dog in the microwave?

Imagine this world without simple idiot proof explenations 0.o

Retardation will be without limits. (no offence to anyone who is retarded b.t.w.)


 Last edited: Dec 4, 2012

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

Hashjie said:

 Depends... How much do you know about GUI?

I'm just saying, this stuff is simple cake for almost anyone but you'll never know who finds it useful.

Plus the fact that not every GUI'er thinks of this solution for using local code.

It's also good to have idiot proof things...

Did you know that there was a women in the US who had fried her dog with a microwave just because the dog went outside in the rain. And afterwards sued the company who made the microwave because it killed the dog since it wasn't in the user manual that you should not put your dog in the microwave?

Imagine this world without simple idiot proof explenations 0.o

Retardation will be without limits. (no offence to anyone who is retarded b.t.w.)


Click to expand...

My answer:

Adiktuz said:

 this tutorial is a really useful one assuming you realize how many things you can do with the GetLocalPlayer() thingy...

Making a how and when to use a CS is as simple as this one:

 When to use a CS?

-> When you need to use a JASS line inside a GUI trigger

How to use a CS?

1)Click the trigger

2)Right click on the trigger's GUI space and choose add action

3)Choose Custom script from the dropdown list

4)Write your jass line


Click to expand...

Also, that is a pretty basic triggering concept... So it's not really a good idea to make a whole tutorial for that... And I believe that it is already (or should be) a part of the basic triggering tutorials here instead...

Have some experience before suggesting will be better for you...


Click to expand...

There you go.

However, you make a good point. Maybe I will create a tutorial on how and when to use a custom script sooner or later.

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

frettory said:

 My answer:

There you go.


Click to expand...

You are correct, but that doesn't mean I don't find it useful to use this guide for reference whenever someone doesn't know how to use local code in GUI.

frettory said:

 However, you make a good point. Maybe I will create a tutorial on how and when to use a custom script sooner or later.


Click to expand...

Well that will be a brief tutorial in that case

It's good if it gave some hints towards learning Jass.

I think at this point GUI users should seriously consider doing that instead.

It's not that hard anyways...

#### CoLd Bon3

#### CoLd Bon3

 Level 14

JoinedAug 8, 2010Messages1,022

Resources: 6

#### Resources
Maps 2Spells and Systems 4

I have used GetLocalPlayer 5-6 times in my map. Only one of them causes a desync, though... i gotta find the problem.

Anyway, after doing this :

Custom script: set udg_Player = GetLocalPlayer()
what will be the current value of the "Player" variable?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

well, it's a special type of variable.. it can be any player.. you can see it as a playergroup and when you use it you pick one of them

#### CoLd Bon3

#### CoLd Bon3

 Level 14

JoinedAug 8, 2010Messages1,022

Resources: 6

#### Resources
Maps 2Spells and Systems 4

So this variable of type "Player" is actually a player group... Jass basically does magic then. Never going to understand it (i think). Doesn't matter, it solves many problems.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

No, it's still a player variable, just that it contains another value for each client. Jass code is not handled centrally on the game server but every client gets the same sheet of paper of script and executes it on its own. Functions like GetLocalPlayer() provide client-specific data the other players in the game have no access to. This way, they create asynchronous results.

 You must log in or register to reply here.


### Similar threads

Locked

 GetLocalPlayer() faq


shadowvzs

Oct 30, 2012

World Editor Help Zone
2

Replies88Views29K

Aug 17, 2016
DracoL1ch

 Multiboards


Bob27

Apr 11, 2006

Trigger (GUI) Editor Tutorials

Replies4Views43K

Aug 29, 2020
awesem

Locked

[Solved]
 multiboard problem


[K40$]-Spectre

Aug 31, 2013

World Editor Help Zone

Replies2Views1K

Sep 1, 2013
[K40$]-Spectre

Locked

[Trigger]
 How can i optimize this trigger?


vlekje5

Sep 30, 2024

Triggers & Scripts

Replies7Views1K

Oct 1, 2024
vlekje5

 Creating a Votekick System


1)ark_NiTe

Nov 8, 2007

Trigger (GUI) Editor Tutorials

Replies29Views19K

Nov 13, 2008
Tichondrius1523

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