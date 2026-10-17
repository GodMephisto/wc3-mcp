---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-custom-error-messages.232494/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating Custom Error Messages
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-custom-error-messages.232494/>
#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

CREATINGCUSTOMERRORMESSAGES

~Created by Archange1

REQUIREMENTS:

A medium-level understanding of the World Editor

Understanding variables and their use

The ability to read and process information

CREDITS:

Vexorian

PurgeandFire111

METHOD ONE

A few things to take note of:

Not recommended - use method two if possible

Harder to use; more annoying than method two
Step One:

To start off, you're going to want to make three variables.

These three variables will be used in method two as well, so you might as well make them now.

NameTypeArray

ErrorSoundSoundNo

ErrorMessageStringNo

ErrorPlayerPlayerNo

- The sound variable is the sound played when the error message pops up.

- The string variable is the message displayed when you tell it to appear.

- The player variable is the player that the message is shown to.

Step Two:

Next you're going to find where you want your custom error message - what trigger you're going to use it for.

Then copy this:

YourTrigger
Events

Conditions

Actions
Custom script: if udg_ErrorSound == null then

Custom script: set udg_ErrorSound = CreateSoundFromLabel( "InterfaceError",false,false,false,10,10)

Custom script: endif

Set ErrorMessage = YourMessage

Set ErrorPlayer = YourPlayer

Custom script: if GetLocalPlayer() == udg_ErrorPlayer then

Custom script: call ClearTextMessages()

Custom script: call DisplayTimedTextToPlayer( udg_ErrorPlayer, 0.52, 0.96, 2.00, "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n|cffffcc00"+udg_ErrorMessage+"|r" )

Custom script: call StartSound(udg_ErrorSound)

Custom script: endif

Step Three:

Now you're going to fill in those variables if you haven't already.

Once again;

- ErrorMessage is the message displayed when you tell it to appear.

- ErrorPlayer is the player that the message is shown to.

Once you have all that completed, you're done!

METHOD TWO

A few things to take note of:

Recommended method

Quick and simple to do

Doesn't tie up a lot of space in the trigger's actions
Step One:

If you haven't already, make the variables listed in method one.

Step Two:

In the trigger editor, go to your map header and enter the following code:

 JASS:

function ErrorMessage takes string error, player whichPlayer returns nothing
 if udg_ErrorSound == null then
 set udg_ErrorSound = CreateSoundFromLabel("InterfaceError",false,false,false,10,10)
 endif
 set error = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n|cffffcc00"+error+"|r"
 if GetLocalPlayer()==whichPlayer then
 call ClearTextMessages()
 call DisplayTimedTextToPlayer(whichPlayer,0.52,0.96,2,error)
 call StartSound(udg_ErrorSound)
 endif
endfunction

Step Three:

Next you're going to find where you want your custom error message - what trigger you're going to use it for.

Then copy this:

YourTrigger
Events

Conditions

Actions
Set ErrorMessage = YourMessage

Set ErrorPlayer = YourPlayer

Custom script: call ErrorMessage(udg_ErrorMessage,udg_ErrorPlayer)

Step Four:

Now you're going to fill in those variables if you haven't already.

Once again;

- ErrorMessage is the message displayed when you tell it to appear.

- ErrorPlayer is the player that the message is shown to.

Once you have all that completed, you're done!

ADDITIONALLY

You can find the full version of Vexorian's SimError here : Vexorian's SimError

If you need to keep track of any messages for debugging try Almia's DebugLog here : https://www.hiveworkshop.com/forums...-2-a-228919/?prev=search=debuglog&d=list&r=20
Keep in mind that the SimError is written in vJass, and Almia's tool has a complete different purpose.


 Last edited: Apr 3, 2013

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

https://www.hiveworkshop.com/forums/spells-569/debuglog-v1-2-a-228919/?prev=status=a&u=Almia

You can try this, this is very advanced.

Warcraft 3 messages as limits, so there's no way you can retrieve older messages than cacheing it.Mine can do a lot of lines.

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

Almia said:

https://www.hiveworkshop.com/forums/spells-569/debuglog-v1-2-a-228919/?prev=status%3Da%26u%3DAlmia

You can try this, this is very advanced.

Warcraft 3 messages as limits, so there's no way you can retrieve older messages than cacheing it.Mine can do a lot of lines.


Click to expand...

Then make a tutorial.

This is the basic side of it. It's only used to mimic the standard error messages, not keep track of them.

For instance, if you cast frost armor on something that already has frost armor, you could make show an error message (like the "not enough lumber/gold" ones),

showing that the unit already has frost armor, and then cancelling the ability or refunding mana or something.

It looks like your system is used for debugging errors found within the map, while this is used only during play. It's not a REAL error, just the message.

I think I over-explained that, but I wanted to be sure you saw the difference. ^^

Correct me if I'm wrong.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

No no no.

Im just linking it to you .

Just say as an alternative but advanced way

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2


 DebugLog v1.2 said:



 This records the debugs messages you want,but it wont appear in-game.The debug messages then will be created inside a text document found in a folder path w/c can be configured inside.


Click to expand...

This is different tho. T_T

Having the messages outside the game doesn't do much good.

This is what the system mimics.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Okay ;_;

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

I'd give you rep if I could, because I see what you were doing. ^^

Unfortunately --> dat 24 hour thang. :c

Thanks anyway.

EDIT: I just realized while re-reading the tutorial that I put created by Archange1.

The tutorial is, of course, but I DON'T claim credit for the system!

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

How about linking SimError? Because that's where you got the idea

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

I think that there is no need for all that |n|n|n , correct me if I'm wrong, but you can set text X,Y on screen right?

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

-Kobas- said:

 I think that there is no need for all that |n|n|n , correct me if I'm wrong, but you can set text X,Y on screen right?


Click to expand...

I wasn't aware of anything like this. x)

Also this is a repost of PurgeandFire111's ideas, not mine.

He just gave me permission to put it into a tutorial for others to see.

Almia said:

 How about linking SimError? Because that's where you got the idea

Click to expand...

I actually got the idea from PurgeandFire111. I've never seen SimError before in my life. ^^

EDIT: Linked both at the bottom of the tutorial.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

@Kobas: The X-Y parts have limits on how far you can set it. (I don't remember if you can do negative values or not, correct me if I'm wrong) Also, it will move all previous text messages to that spot.

This tutorial looks solid.  Although, the system isn't really by me, so you can remove the credit about "~System by PurgeandFire".

Approved.

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

PurgeandFire111 said:

 @Kobas: The X-Y parts have limits on how far you can set it. (I don't remember if you can do negative values or not, correct me if I'm wrong) Also, it will move all previous text messages to that spot.

This tutorial looks solid.  Although, the system isn't really by me, so you can remove the credit about "~System by PurgeandFire".

Approved.


Click to expand...

Alright will do I suppose. :s

And thanks.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Okay nice tutorial

+ rep

Useful for debugging purposes and/or some notifications

#### Wintaer

#### Wintaer

 Level 3

JoinedApr 10, 2013Messages8

Very nice tutorial lizzeh

#### NewMapMaker

#### NewMapMaker

 Level 2

JoinedApr 12, 2013Messages11

I like it. However I wouldn't really call it an "error message". Maybe I'd call it a "notification message", since error seems to bring to mind something like the program or map crashing

Good job to all involved.

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

NewMapMaker said:

 I like it. However I wouldn't really call it an "error message". Maybe I'd call it a "notification message", since error seems to bring to mind something like the program or map crashing

Good job to all involved.


Click to expand...

 Well it's titled error message, because the messages that are shown when: for example, you try to building something without enough gold; are called error messages.

#### NewMapMaker

#### NewMapMaker

 Level 2

JoinedApr 12, 2013Messages11

fair enough  that makes sense... I guess "In-game Error Message" then, as opposed to "windows error message"...

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

why dont use BJDebugMsg()?

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

BjDebugMsg() has a different purpose entirely. It displays the text to all players for what, 60 seconds?

It's useful for finding problems with triggering, but that's not what this does.

This system is used for creating similar messages like the one below.

Unlike your average error messages, these are SUPPOSED to be in-game, as they tell the player some sort of information.

#### Wintaer

#### Wintaer

 Level 3

JoinedApr 10, 2013Messages8

BjDebugMsg will also give this

 JASS:

function BJDebugMsg takes string msg returns nothing
 local integer i = 0
 loop
 call DisplayTimedTextToPlayer(Player(i),0,0,60,msg)
 set i = i + 1
 exitwhen i == bj_MAX_PLAYERS
 endloop
endfunction

While DisplayTextToPlayer will only give this small code

 JASS:

native DisplayTextToPlayer takes player toPlayer, real x, real y, string message returns nothing

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

Wintaer said:

 BjDebugMsg will also give this

 JASS:

function BJDebugMsg takes string msg returns nothing
 local integer i = 0
 loop
 call DisplayTimedTextToPlayer(Player(i),0,0,60,msg)
 set i = i + 1
 exitwhen i == bj_MAX_PLAYERS
 endloop
endfunction

While DisplayTextToPlayer will only give this small code

 JASS:

native DisplayTextToPlayer takes player toPlayer, real x, real y, string message returns nothing

Click to expand...

And that.

Ty. <33

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Indeed, however I did not check that properly so this is what I actually meant

Untitled Trigger 002
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Custom script: call DisplayTimedTextToForce( bj_FORCE_PLAYER[0], 30, "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n|cffffcc00"+"error"+"|r" )

this is converted from GUI btw so I know it's not the fastest way but you get the point miss

edit: or this to shorten the code

Untitled Trigger 002
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Custom script: call DisplayTimedTextToForce( bj_FORCE_PLAYER[0], 30, "|cffffcc00"+"error"+"|r" )

however it's indeed useful even if I don't like the way this works

#### Tauriel

#### Tauriel

 Level 2

JoinedDec 23, 2013Messages6

Look at you Beth. Moving on up in the world. ;p

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

Tauriel said:

 Look at you Beth. Moving on up in the world. ;p


Click to expand...


Thank's asshole.

#### LeoWee

#### LeoWee

 Level 2

JoinedFeb 27, 2021Messages10

Unlike real in-game error messages, it would clear all text messages on the player's screen though.

#### maxodors

#### maxodors

 Level 7

JoinedFeb 9, 2021Messages301

Faith said:

CREATINGCUSTOMERRORMESSAGES

~Created by Archange1

REQUIREMENTS:

A medium-level understanding of the World Editor

Understanding variables and their use

The ability to read and process information

CREDITS:

Vexorian

PurgeandFire111

METHOD ONE

A few things to take note of:

Not recommended - use method two if possible

Harder to use; more annoying than method two
Step One:

To start off, you're going to want to make three variables.

These three variables will be used in method two as well, so you might as well make them now.

NameTypeArray

ErrorSoundSoundNo

ErrorMessageStringNo

ErrorPlayerPlayerNo

The sound variable is the sound played when the error message pops up.

The string variable is the message displayed when you tell it to appear.

The player variable is the player that the message is shown to.

Step Two:

Next you're going to find where you want your custom error message - what trigger you're going to use it for.

Then copy this:

YourTrigger
Events

Conditions

Actions
Custom script: if udg_ErrorSound == null then

Custom script: set udg_ErrorSound = CreateSoundFromLabel( "InterfaceError",false,false,false,10,10)

Custom script: endif

Set ErrorMessage = YourMessage

Set ErrorPlayer = YourPlayer

Custom script: if GetLocalPlayer() == udg_ErrorPlayer then

Custom script: call ClearTextMessages()

Custom script: call DisplayTimedTextToPlayer( udg_ErrorPlayer, 0.52, 0.96, 2.00, "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n|cffffcc00"+udg_ErrorMessage+"|r" )

Custom script: call StartSound(udg_ErrorSound)

Custom script: endif

Step Three:

Now you're going to fill in those variables if you haven't already.

Once again;

ErrorMessage is the message displayed when you tell it to appear.

ErrorPlayer is the player that the message is shown to.

Once you have all that completed, you're done!

METHOD TWO

A few things to take note of:

Recommended method

Quick and simple to do

Doesn't tie up a lot of space in the trigger's actions
Step One:

If you haven't already, make the variables listed in method one.

Step Two:

In the trigger editor, go to your map header and enter the following code:

 JASS:

function ErrorMessage takes string error, player whichPlayer returns nothing
 if udg_ErrorSound == null then
 set udg_ErrorSound = CreateSoundFromLabel("InterfaceError",false,false,false,10,10)
 endif
 set error = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n|cffffcc00"+error+"|r"
 if GetLocalPlayer()==whichPlayer then
 call ClearTextMessages()
 call DisplayTimedTextToPlayer(whichPlayer,0.52,0.96,2,error)
 call StartSound(udg_ErrorSound)
 endif
endfunction

Step Three:

Next you're going to find where you want your custom error message - what trigger you're going to use it for.

Then copy this:

YourTrigger
Events

Conditions

Actions
Set ErrorMessage = YourMessage

Set ErrorPlayer = YourPlayer

Custom script: call ErrorMessage(udg_ErrorMessage,udg_ErrorPlayer)

Step Four:

Now you're going to fill in those variables if you haven't already.

Once again;

ErrorMessage is the message displayed when you tell it to appear.

ErrorPlayer is the player that the message is shown to.

Once you have all that completed, you're done!

ADDITIONALLY

You can find the full version of Vexorian's SimError here : Vexorian's SimError

If you need to keep track of any messages for debugging try Almia's DebugLog here : https://www.hiveworkshop.com/forums/spells-569/debuglog-v1-2-a-228919/?prev=search=debuglog&d=list&r=20
Keep in mind that the SimError is written in vJass, and Almia's tool has a complete different purpose.

Click to expand...

I just wish it was possible to find SimError on Hive

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 Custom Scrifice Ability


sarumanthecursed

Jun 17, 2015

Triggers & Scripts

Replies11Views2K

Jun 18, 2015
Nichilus

Locked

[Need Info]
 Custom Scrifice Ability


sarumanthecursed

Jun 16, 2015

StarCraft II Modding

Replies2Views1K

Jun 17, 2015
sarumanthecursed

Locked

[Solved]
 Limited Summon trigger


Lordul Dracula

Oct 8, 2022

World Editor Help Zone

Replies7Views2K

Oct 8, 2022
Lordul Dracula

Locked

 Unit to be replaced gets duplicated instead


Eagle XI

Nov 21, 2015

World Editor Help Zone

Replies5Views1K

Nov 22, 2015
Eagle XI

Locked

[General]
 Message Log mess


Natu

Oct 19, 2016

World Editor Help Zone

Replies8Views2K

Nov 1, 2016
DracoL1ch

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