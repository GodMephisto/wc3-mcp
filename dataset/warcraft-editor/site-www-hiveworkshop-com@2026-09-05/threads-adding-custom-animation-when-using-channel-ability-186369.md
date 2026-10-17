---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/adding-custom-animation-when-using-channel-ability.186369/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Adding Custom Animation When Using Channel Ability
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/adding-custom-animation-when-using-channel-ability.186369/>
#### neku99

#### neku99

 Level 10

JoinedSep 3, 2009Messages458

Resources: 4

#### Resources
Spells 3Tutorials 1

#
 Adding Triggered Animation When Using Channel Ability

So I have always wanted to this kind of thing. I did once accidentally but failed to replicate it. But because of a good friend, Adiktuz, I finally made it work.

###
 Firstly . . .

As all of us map makers know, the channel ability is so useful, because of all it's given features. Of practically being any ability you'd ever want it to be. But then I had this problem as I ventured into this ability.

###
 The Problem . . .

Basically as channel is too good to be true(nah!) I found out that you can't put a custom tiggered animation with channel. For example when you cast channel, then trigger an animation it will have a small transition with the basic anim and triggered anim.

Also even if you put follow through time zero there is still the small transition in which is very ugly.

###
 The Solution

Well I don't know if people know about this already but for the sake of those who don't, I shall show you how to "Add Custom Animation When Using Channel Ability"

###
 1. Setting Up Channel

First, you need to set these values on the channel ability:

1.Data - Art Duration - 0.00 (Dunno what it does but meh better safe than sorry)

2.Data - Disable Other Abilities - false(You won't want to do this )

3.Data - Follow Through Time - 0.00(Well basically but it doesn't matter really just better set it to 0)

4.Data - Options - Visible(Makes the ability show up in the command card)

5.Data - Target Type - [insert what you want here] (It doesn't matter what you put, the triggered animation will run perfectly whether target unit, target point, no target)

Guide Image

###
 2. Setting Up The Units

Second you need to set this to any unit that might use channel:

1. Art - Animation - Cast Point - 0.00

Remember to do this to any and all units that might use channel. This is basically the thing that let's the triggered animation run smoothly.

Guide Image

NOTE: By doing this other abilities that this unit uses, besides channel, will be affected.

###
 3. Triggering The Animation

Now all the last part to be done is to trigger the animation. Well before you go rushing to the close button on your tab, there's a little trick so that the animation will work.

When you animate, you need to do these actions in sequence:

When Spell Is Cast >> PAUSE UNIT >> PLAY ANIM >> UNPAUSE UNIT

GUI EXAMPLE

Animation
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Channel

Actions
Unit - Pause (Triggering unit)

Animation - Play (Triggering unit)'s attack animation

Unit - Unpause (Triggering unit)

NOTE: You need to do this in order for the animation to work.

###
 3. Enjoyment and Other Notes

Once that is done enjoy all the possibilities. Although remember that since the Cast Point set to 0, all abilities that your unit will be using will be affected. I don't know what'll happen though, I will test later. Also Channeling won't work as you have seen in the trigger, it will pause the unit, basically cancelling the ability. But none the less , use it in your discretion.

Special thanks to Adiktuz for giving me an answer that lead me to the answer.

##
 Thanks For Reading


 Last edited by a moderator: Aug 4, 2017

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

neku99 said:

 Basically as it is too good to be true I found out that you can't trigger a custom animation with channel.


Click to expand...

By custom animation, you mean any default animation by original WC3 unit, not an all custom, created by some forum member for example, animation?

Although remember that since the Cast Point set to 0, all abilities that your unit will be using will be affected. I don't know what'll happen though, I will test later.
Cast point defines the time between

Unit - A unit Begins casting an ability
and

Unit - A unit Starts the effect of an ability
Let's say you've triggered a spell that uses "Starts the effect..".

Usually when cast point it's at a default value (about 0.3-0.5 seconds), the caster has time to face the unit it targeted even if it's facing 180° away from it when it begins the casting. But if the cast point is 0, the caster is can be facing whatever angle, not the target. This is especially harmful if your triggered spell uses the facing of the caster.

#### neku99

#### neku99

 Level 10

JoinedSep 3, 2009Messages458

Resources: 4

#### Resources
Spells 3Tutorials 1

Yes I should have put Triggered Animation. I just remembered this morning. I need to check on it more.


Also do you mean to say that Cast Point is actually a time for a unit to face the target? I didn't know that again thanks on the info. I'll try and see what I can discover.


EDIT: The ability and animation works fine. But I see no problem with spells using the facing of the unit. Please elaborate.


EDIT: I think I get what you mean. What if the channel is a point target spell for example dash. You need to cast it at a direction but uses the unit facing as an angle.


Well these cases are uncommon(?) What I have to say is use it a your discretion. This method prevents for example channeling spells though channel ability (the follow through time) as it cancels the ability, but there are other ways to simulate a channel. And on that note there are also ways that we can simulate a cast point.


It depends on how much someone wants to utilize the channel, animation wise. BUt I have to say I like the power to add triggered anims.


 Last edited: Jan 3, 2011

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

neku99 said:

 Also do you mean to say that Cast Point is actually a time for a unit to face the target?


Click to expand...

No. But with lower cast point time, the unit has less time to face the target before "Starts the effect..." triggers.

 But I see no problem with spells using the facing of the unit. Please elaborate.


Click to expand...


Cast a point targeted ability, like Shockwave, behind the hero while having 0.00 cast point. Use this trigger.

Untitled Trigger 007
Events
Unit - A unit Starts the effect of an ability

Conditions

Actions
Unit - Create 1 Mortar Team for Player 1 (Red) at ((Position of (Triggering unit)) offset by 300.00 towards (Facing of (Triggering unit)) degrees) facing Default building facing degrees

#### neku99

#### neku99

 Level 10

JoinedSep 3, 2009Messages458

Resources: 4

#### Resources
Spells 3Tutorials 1

Maker said:

 No. But with lower cast point time, the unit has less time to face the target before "Starts the effect..." triggers.


Cast a point targeted ability, like Shockwave, behind the hero while having 0.00 cast point. Use this trigger.

Untitled Trigger 007
Events
Unit - A unit Starts the effect of an ability

Conditions

Actions
Unit - Create 1 Mortar Team for Player 1 (Red) at ((Position of (Triggering unit)) offset by 300.00 towards (Facing of (Triggering unit)) degrees) facing Default building facing degrees

Click to expand...


So technically the the cast point is like a time given to the unit before it can cast. So that it has time to turn ok got it.


Oh lookie my previous post.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

anyway, using facing of unit is pretty bothersome since it does not return all possible angles from 0.00 to 359.99... its better to use AngleBetweenPoints for targeted abilities...

though it would look kinda wierd to have a shockwave going to the right when you're facing the left...

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Is there any way to set the animation properly without setting the cast point to 0.00? I'll look into it a bit, and if I can't find anything, it'll be ready for approval.

#### neku99

#### neku99

 Level 10

JoinedSep 3, 2009Messages458

Resources: 4

#### Resources
Spells 3Tutorials 1

As of now that's the only method I can think of. I've done it once with a different method but I forgot how to do it. But I do remember not editing the cast point back then.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Well, it'll do for now. I checked some common stuff, including setting the animation index instead and a few other things, but none seemed to work.

This method works properly for its intent, so ~approved.  Nice tutorial.

Moved to general mapping because it has to be a bit more miscellaneous/external to fit in the misc category.

#### neku99

#### neku99

 Level 10

JoinedSep 3, 2009Messages458

Resources: 4

#### Resources
Spells 3Tutorials 1

Ehehe thanks!

I'll remember the "more miscellaneous/external" next time

#### daverave1212

#### daverave1212

 Level 11

JoinedApr 9, 2010Messages425

Resources: 8

#### Resources
Skins 2Tools 2Maps 3Tutorials 1

The pause command seems to bug the animation and make it loop infinitely

#### daverave1212

#### daverave1212

 Level 11

JoinedApr 9, 2010Messages425

Resources: 8

#### Resources
Skins 2Tools 2Maps 3Tutorials 1

The pause command makes the unit animation loop infinitely

There still is delay, it's just smaller

#### daverave1212

#### daverave1212

 Level 11

JoinedApr 9, 2010Messages425

Resources: 8

#### Resources
Skins 2Tools 2Maps 3Tutorials 1

I found the pause and unpause thing buggy

Also, it works a lot better with breath of fire  try it

Great one mate

#### daverave1212

#### daverave1212

 Level 11

JoinedApr 9, 2010Messages425

Resources: 8

#### Resources
Skins 2Tools 2Maps 3Tutorials 1

I found the pause and unpause thing buggy


Also, it works a lot better with breath of fire  try it

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

The idea of using Channel is that you can have multiple abilities based on Channel on a single unit without bugging it as long as the Raw ID on the data fields of Channel is different. You would lose that edge when using any other ability. Like using two BoF based abilities on the same unit would bug.

#### alucy0210

#### alucy0210

 Level 1

JoinedApr 10, 2015Messages5

Thanks.

คาสิโน

#### Barade

#### Barade

 Level 29

JoinedFeb 2, 2006Messages1,692

Resources: 5

#### Resources
Tools 1Maps 2Spells and Systems 2

I've found this thread and I get the same issue as the others. When pausing/unpausing the unit the trigger seems to be triggered again and again. Even when I disable and enable it.

Is there any better solution?

The only solution which works for me is disabling the trigger waiting 0.0 and enabling again but I don't want any delay.

#### neku99

#### neku99

 Level 10

JoinedSep 3, 2009Messages458

Resources: 4

#### Resources
Spells 3Tutorials 1

Barade said:

 I've found this thread and I get the same issue as the others. When pausing/unpausing the unit the trigger seems to be triggered again and again. Even when I disable and enable it.

Is there any better solution?

The only solution which works for me is disabling the trigger waiting 0.0 and enabling again but I don't want any delay.


Click to expand...

Hey there Barade! This is a pretty old tutorial of mine and after going back to modding I have noticed that the Pause - Stop - Unpause doesn't seem to work for me as well. (Since I'm still using the technique but I dont use the pause - stop - unpause anymore)

The alternative I'm using right now is - instead of pausing the unit, I created code to [Stun] the unit using a Modified Firebolt. This seems to do the trick for me. Also I suggest playing with the Events for spell casting (Starts the effect, Ends the effect, Casting the effect etc.), there seems to be a combination that works.

 Code:

//----------------------------------------------------------------------------
// Judgement v0.1
//----------------------------------------------------------------------------
//
// Judgement is a small library with the goal to create a delay when actions
// are activated and also play a specific animation after it.
//
//----------------------------------------------------------------------------

library Judgement
 //------------------------------------------------------------------------
 //Setup
 //------------------------------------------------------------------------
 globals
 endglobals

 //------------------------------------------------------------------------
 //Main Functions
 //------------------------------------------------------------------------
 function Delay takes unit u returns nothing
 local real x = GetUnitX(u)
 local real y = GetUnitY(u)
 local unit t = CreateUnit(Player(PLAYER_NEUTRAL_PASSIVE),'u001',x,y,bj_UNIT_FACING)

 call UnitApplyTimedLifeBJ( 0.50, 'BTLF', t)
 call IssueTargetOrderBJ( t, "firebolt", u )
 call SetUnitAnimation( u, "attack" )

 call BJDebugMsg("System: Delay")
 endfunction

endlibrary

It's a quick a dirty code for testing. Although I haven't tested it in a while (Been coding some other stuff currently). I really should update this thread.

#### Barade

#### Barade

 Level 29

JoinedFeb 2, 2006Messages1,692

Resources: 5

#### Resources
Tools 1Maps 2Spells and Systems 2

Okay thx. Well I guess my approach with a wait is okay as long as my ability has a cooldown higher than 0.0.

 You must log in or register to reply here.


### Similar threads

 Basic Animations


Kwah

Sep 6, 2008

Misc/General Mapping Tutorials

Replies13Views27K

Sep 13, 2008
Kwah

Locked

[General]
 Why triggered animations fail when using "Channel" as base ability ?


streetpunk

Mar 25, 2016

World Editor Help Zone

Replies5Views1K

Mar 25, 2016
streetpunk

Locked

 Model - Animation problem


Ceday

Jan 28, 2013

World Editor Help Zone

Replies8Views2K

Jan 29, 2013
Ceday

Locked

[Trigger]
 Drinking animation not working anymore after basing ability on Channel.


Fakuspakus

Mar 5, 2019

Triggers & Scripts

Replies4Views1K

Mar 5, 2019
Fakuspakus

Locked

 Substitutes to Channel ability, for Triggering purposes.


JJhonJames

Dec 6, 2024

World Editor Help Zone

Replies9Views1K

Dec 8, 2024
Pyrogasm

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top