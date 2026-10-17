---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-make-a-chain-ability-the-easy-way.278300/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Make a Chain Ability the Easy Way
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-make-a-chain-ability-the-easy-way.278300/>
#### ShadowBladeX

#### ShadowBladeX

 Level 6

JoinedApr 20, 2016Messages215

Resources: 1

#### Resources
Tutorials 1

#

 Chain ability tutorial

##

 (the easy way)

 This means there is no variables needed

## What is a chain ability?
A chain ability is a continuous casting of a desired ability

## How to make it
Well all you need are

abilities with base either target or teleport

knowledge about triggers

## how this works?
Well it just an ability casted after an another ability is casted


This tutorial has 2 types

teleport based

target based


When you are using teleport based ability that means you are using an ability (ex.blink) to activate this trigger


[trigger=My trigger]Doom Spikes


 Events


 Unit - A unit Begins casting an ability


 Conditions


 Actions


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 (Ability being cast) Equal to Blink


 Then - Actions


 Unit - Create 1 (any flying unit) for Player 1 (Red) at (Position of (Casting unit)) facing (Facing of Casting unit) degrees


 Unit - Add (any ability you want) to (Last created unit)


 Wait 0.90 game-time seconds


 Unit - Order (Last created unit) to (matching your ability) (Position of Casting unit)


 Else - Actions


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 (Ability being cast) Equal to (matched ability)


 Then - Actions


 Unit - Pause Casting unit[/trigger]


So to clarify your hero will cast an ability equal to blink then it will create a flying unit you picked then it will add the ability you want to the created unit (the ability must be a unit or could just set it to hero ability false and unit ability true) let's just use impale as an example so impale will be added to last created unit then order last created unit to undead crypt lord - impale casting unit.


Now we will make a target based we will use storm bolt


[trigger=My trigger]Doom Spikes


 Events


 Unit - A unit Begins casting an ability


 Conditions


 Actions


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 (Ability being cast) Equal to Storm bolt


 Then - Actions


 Unit - Create 1 (any flying unit) for Player 1 (Red) at (Position of (Casting unit)) facing (Facing of Casting unit) degrees


 Unit - Add (any ability you want) to (Last created unit)


 Wait 0.90 game-time seconds


 Unit - Order (Last created unit) to (matching your ability) (Position of Targeted unit of ability being cast)


 Else - Actions


 If (All Conditions are True) then do (Then Actions) else do (Else Actions)


 If - Conditions


 (Ability being cast) Equal to (matched ability)


 Then - Actions


 Unit - Pause Casting unit[/trigger]


Because we are using target based that means the storm bolt will be the marker of the next ability being cast


Now I know that this is a bit complicated because of my bad English that's why I'm going to tell you how this trigger works first your hero will cast the ability then followed by the condition ability being cast is equal to. That's the part of creating and ability casting next the last created unit cast Impale so your next condition is impale and so on. At the then parts just add if then else multiple do this as many as you want But you can change the event depending on your timing. Then if you want to end your trigger then just add inside casting unit that's all.


If you think that this is helpful a "rep+" would really be appreciated....


I promise when I get my new laptop I will edit this to a more understandable tutorial I was making this on my phone and it is a bit laggy that's why it's hard to edit


 Last edited: May 10, 2016

#### Meatmuffin

#### Meatmuffin

 Level 11

JoinedJul 25, 2014Messages490

Resources: 10

#### Resources
Maps 2Spells and Systems 7Tutorials 1

First post, and it's a shuddy tutorial. There are many things wrong with this:

The structure of your tutorial. Use headlines to signify the main topics/points of your tutorial. The grammar is also poor, this looks like this was made in 5 minutes by not giving a damn.

The tutorial itself is short. There have been many tutorials or rather, submissions that have been regarding chain-type abilities. For example, Chain by Almia. This is not worthy of being uploaded.

The triggers are not embedded into TRIGGER tags, use them whenever you want to post GUI trigger text. Also this was not even copied using "Copy as Text", seems like manually typed.

Triggers are far beyond inefficient. Waits are fraud upon in hive. Look at some tutorials how to make your stuff efficient and leakless/bugless.

#### ShadowBladeX

#### ShadowBladeX

 Level 6

JoinedApr 20, 2016Messages215

Resources: 1

#### Resources
Tutorials 1

Im using my phone

1.Dont worry ill post later my original work on my laptop


2.Its a chain but i made it simple


3.Thank you for notifying me


4.If i copy it as text will it copy the icon (ex.event will it copy the flag) im new here remember.....

#### Meatmuffin

#### Meatmuffin

 Level 11

JoinedJul 25, 2014Messages490

Resources: 10

#### Resources
Maps 2Spells and Systems 7Tutorials 1

Put the text inside trigger tags, like this:

[TRIGGER]<text here>[/TRIGGER]


 Last edited: Apr 23, 2016

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

I've had a look at the Code and I don't think this tutorial really stands up to scrutiny:

The ability:


 - The spell isn't MUI


 - Contains leaks


 - Utilises wait functions


 - Uses Begins casting not starts the effect of


These things aren't inherently disallowed in tutorials - but when it's literally the only content I don't think it stands up

The tutorial:


 - Contains no explanation


 - Does not deliver the proposed content (this isn't the "easy way", this is the "wrong way"

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

This is more of a spell submission rather than a tutorial.

I wont comment on the code as others already did that.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

^more of a proposal I would say.

Votes for rejection

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I agree with the above. It isn't really a tutorial--it is more of a sample code.

If you still want to update it to include explanations, I'll leave it here. Otherwise, I'll graveyard it!

#### ShadowBladeX

#### ShadowBladeX

 Level 6

JoinedApr 20, 2016Messages215

Resources: 1

#### Resources
Tutorials 1

dudes can u tell me all the tags in this site so that i can clarify my trigger

#### Wasabee

#### Wasabee

 Level 19

JoinedOct 7, 2014Messages2,208

Resources: 2

#### Resources
Maps 2

BB Code

#### ShadowBladeX

#### ShadowBladeX

 Level 6

JoinedApr 20, 2016Messages215

Resources: 1

#### Resources
Tutorials 1

Now this is my finished tutorial hope you'll like it


 Last edited: May 10, 2016

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

It is still missing most of the details like:

- What is a Chain Ability

- How does it work?

- how to make one?

About telling how the code works, you are just telling literally what the code is, not how it works.

It's like you are a teacher that teaches people by telling what you read, but not what is the meaning of what you are teaching.

On the code, it uses inappropriate event (begins casting instead of start effect)

- It leaks points

- uses waits

- pauses units

- (Again)documentation on how the code works.

#### ShadowBladeX

#### ShadowBladeX

 Level 6

JoinedApr 20, 2016Messages215

Resources: 1

#### Resources
Tutorials 1

How bout this

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

You didn't read my review.

You only read, like, 30% of what is necessary.

You followed those suggestions, yet they still are still lacking and ambiguous.

#### ShadowBladeX

#### ShadowBladeX

 Level 6

JoinedApr 20, 2016Messages215

Resources: 1

#### Resources
Tutorials 1

Almia said:

 You didn't read my review.


You only read, like, 30% of what is necessary.


You followed those suggestions, yet they still are still lacking and ambiguous.


Click to expand...


I'm sorry but I'm still a newbie and I'm still using waits but it's okay of you don't need to pause your units depends on what you are used to but I doesn't leak when I tried it

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

leak != massive frame drop

you just can't create tutorials that teaches others wrong.

just read my review

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

I suggest that you should learn more about triggers. What should and what shouldn't.

I'll just point out the mistakes in your trigger even though the others have mentioned it previously and I'll try to explain the reason so you can understand why you should avoid it.

Unit - A unit Begins casting an ability

This event means the trigger will fire when your unit begins casting the ability. Before the ability itself actually works, your trigger will fire first. This can become a problem, you can just cancel the ability quickly after you cast it. Because the ability hasn't worked yet, there will be no cooldown but the trigger will run anyway. Allowing the user to spam this ability. The workaround is to use starts the effect of an ability.

As Almia and Tank mentioned, your trigger contains leaks. Don't know what leaks are? Check it out here.

You are using waits which isn't a very good thing because waits aren't very accurate and can cause a problem.

Useful links about waits - https://www.hiveworkshop.com/forums/trigger-gui-editor-tutorials-279/mui-triggers-waits-218354/ - https://www.hiveworkshop.com/forums...279/mui-spells-using-artificial-waits-223315/

Pausing unit isn't good either, the duration of the buff that unit has will be paused as well when you pause a unit. Causing it to have longer effects.

I'm not good at explaining things, so please do help me to give this guy an enlightenment and also please correct any mistake I made if there is any.

#### ShadowBladeX

#### ShadowBladeX

 Level 6

JoinedApr 20, 2016Messages215

Resources: 1

#### Resources
Tutorials 1

Ok i will re-edit this soon

#### blancfaye7

#### blancfaye7

 Level 10

JoinedMay 21, 2014Messages580

Resources: 1

#### Resources
Maps 1

Well...


Newbie Tip


Be clear.


Don't make excuses.


Listen heartily to those who give advice.


Search for the unknown terms before asking continuously.


There are buttloads of tutorials here. Look them up first.

 but I doesn't leak when I tried it


Click to expand...

...


You would've made Spinnaker proud.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

ShadowBladeX said:

 I promise when I get my new laptop I will edit this to a more understandable tutorial I was making this on my phone and it is a bit laggy that's why it's hard to edit


Click to expand...

What about this?

There are still many valid critique points from posts above that are not answered or fixed.

#### ShadowBladeX

#### ShadowBladeX

 Level 6

JoinedApr 20, 2016Messages215

Resources: 1

#### Resources
Tutorials 1

You could just put this in the graveyard coz, I reviewed it and everyones right it isn't the easy way it's the wrong way and i don't need it anymore

 You must log in or register to reply here.


### Similar threads

 This is a good way to make a orpg


jordon

Jul 9, 2009

Tutorial Archive

Replies4Views1K

Jul 16, 2009
jordon

 Unit Replacement Tutor (Very Easy)


Slaydon

Jun 3, 2008

Tutorial Archive

Replies5Views2K

Jun 4, 2008
Dagguh

 Casting Time System/Snippet


rodead

Sep 16, 2008

Tutorial Archive

Replies3Views2K

Sep 21, 2008
PurplePoot

 How to: Create Professions easily


duderock101

Sep 18, 2009

Tutorial Archive

Replies6Views2K

Sep 22, 2009
Septimus

 Hero Choosing - Choose a Hero with an Ability


.Ragnaros

Aug 26, 2009

Tutorial Archive

Replies13Views2K

Sep 7, 2009
M4stah

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top