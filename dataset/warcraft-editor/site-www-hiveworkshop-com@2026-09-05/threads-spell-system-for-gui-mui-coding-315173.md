---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/spell-system-for-gui-mui-coding.315173/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Spell System for GUI Mui Coding
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/spell-system-for-gui-mui-coding.315173/>
#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

#
 Spell System for GUI MUI Coding

By Daffa the Mage

##
 Required Materials

World Editor

GUI Spell System v1.8.0.0 by Bribe

Basic knowledge of how to import Spell System into your map

Knows how to open Trigger Editor and making a new trigger

Have basic knowledge of trigger editor workings

##
 Introduction

Greetings,

This tutorial is aimed at newbie GUI triggerer to be able to make simple spells that fit the MUI criteria by utilizing spell event. This is part of my aim to make Spell Section a more interesting place with more MUI resources to boot and the first tutorial of a series of tutorial related to this.​

##
 Making Your First Spell

So, for the startup, we will make a simple 'Heal' spell using the Spell System. I'm talking about Priest 'Heal', but instead of just healing, we want it to increase our max health per second for a few seconds.

So, buckle up and get yourself ready. First, follow instruction to install the Spell System. Then, open the trigger editor and copy the Spell System Sample Config. Let's call the new trigger Heal Config. Enable Heal Config and set your ability to your healing ability. Create 2 empty triggers called Heal Cast and Heal Loop.

Then, configure the config to be somewhat like the following. This is configuration is based on Bribe's documentation on example config trigger.​

Heal Config
Events
Map initialization

Conditions

Actions
-------- You must assign an ability ID to let the system know which spell you're using: --------

Set Spell__Ability = Heal

-------- Next, you must pick at least one of the following types of triggers to run at a phase of the spell: --------

Set Spell__Trigger_OnCast = Heal Cast <gen>

-------- Next, you can specify a trigger to run periodically, if you want that type of behavior. --------

Set Spell__Trigger_OnLoop = Heal Loop <gen>

-------- Added 28 Feb 2016: You can now specify Spell__Time, Spell__Duration and Spell__DurationPerLevel from the Config trigger. --------

-------- NOTE: With these settings, the OnLoop trigger will run every 1 second for 4/5/6 seconds --------

Set Spell__Time = 1.00

Set Spell__Duration = 3.00

Set Spell__DurationPerLevel = 1.00

-------- The next ones are for the InRange filter, if you use it. You don't have to include any of these for many typical spells. --------

-------- TIPS: --------

-------- > For healing spells, you'll want to set AllowAllies to True and AllowEnemies to False. --------

-------- > If you are creating a "kill unit" spell, you'll want to set AllowHero to False. --------

-------- > If you are making a custom "Resurrect", you'll want to set AllowDead to True and AllowLiving to False. --------

-------- > AllowMagicImmune as False also prevents invulnerable units from being picked, which is a plus in my opinion. --------

Set Spell__Filter_AllowEnemy = True

Set Spell__Filter_AllowLiving = True

Set Spell__Filter_AllowHero = True

Set Spell__Filter_AllowNonHero = True

Set Spell__Filter_AllowAlly = False

Set Spell__Filter_AllowDead = False

Set Spell__Filter_AllowFlying = False

Set Spell__Filter_AllowMechanical = False

Set Spell__Filter_AllowStructure = False

Set Spell__Filter_AllowMagicImmune = False

-------- NEW: Added 17 May 2016 - you can specify your own trigger conditions to add to the InRange check --------

-------- The trigger conditions reference the unit "Spell__InRangeUnit" --------

Set Spell__Trigger_InRangeFilter = Spell System Sample Filter <gen>

-------- If you want to keep track of which units have already been hit by the spell, set UseTargetGroup to True --------

-------- TIPS: --------

-------- > The group is called Spell__TargetGroup. It starts empty and will destroy itself when the spell is finished --------

-------- > After using a Spell__InRange check, you can use "Add all units in Spell__InRangeGroup to Spell__TargetGroup" --------

Set Spell__UseTargetGroup = True

-------- When everything is set the way you want it, run Spell System <gen>: --------

Trigger - Run Spell System <gen> (ignoring conditions)

We will not use the filters, so I just skim it. You can alter the duration as needed.​

Heal Cast
Events

Conditions

Actions
Set Spell__StartDuration = True

Heal Cast is only for duration triggering. Without it, the spell loop effect will not run.​

Heal Loop
Events

Conditions

Actions
Unit - Set Max HP of Spell__Target to ((Max HP of Spell__Target) + 10)

Heal Loop will add 10 Max HP per second. Something odd I found with my test map running the spell 5 times despite I set it to 4 seconds. It can easily be fixed by checking the remaining duration as shown below:​

Heal Loop
Events

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Spell__Duration Greater than 0.00

Then - Actions
Unit - Set Max HP of Spell__Target to ((Max HP of Spell__Target) + 10)

Else - Actions

Now, it works as intended. I provided a test map for you to look at the final result. If you don't get the same result, feel free to compare your code with the provided attachment.​

~Daffa the Mage

#### Attachments

SpellTutorial1.w3x

 52.9 KB

 · Views: 190




 Last edited: Oct 3, 2020

#### Lt_Hawkeye

#### Lt_Hawkeye

 Level 12

JoinedJul 4, 2016Messages636

Not sure if this question is specifically related to this tutorial, but I notice that in some of your spells, you sometimes use the GUI spell system's arrays instead of the non-arrayed variables? Why is that? When is it necessary to use the arrays over the standard variables to keep it MUI.

I never could understood the purpose behind it while working with the system.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Clanzion said:

 Not sure if this question is specifically related to this tutorial, but I notice that in some of your spells, you sometimes use the GUI spell system's arrays instead of the non-arrayed variables? Why is that? When is it necessary to use the arrays over the standard variables to keep it MUI.

I never could understood the purpose behind it while working with the system.


Click to expand...

I'm experimenting on it and trying to see the difference. So far, I found it's mostly related to how the readibility concerning the spell. I still need to investigate further.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

GUI Spell System has internal data structure and system variables, they are array.

Before the user spell "InternalSpell[SpellIndex]" runs, the usage of system variables gets simplified...

Duration[SpellIndex] becomes only Duration

Caster[SpellIndex] becomes only Caster

....
so it's easier to work with for the user. Technically, you also could use his internal system variables Duration[SpellIndex], for example, instead of only Duration, it will work the same.

The very same SpellIndex is also provided for the user, so he might use it, too. It might be useful for example, if the user wants to store additional information to the spell, like creating a temporary extra effect for the spell. So he stores it in his NewSpellEffect[SpellIndex] array variable, and can refer to it later.

In Chidori Lightning Storms v1.0, one can see some internal system variables are used, like ChidoriLS_BlastRange[Spell_i_Level[Spell__Index]], in the "Chidori LS Effect trigger". But instead ChidoriLS_BlastRange[Spell__Level] could be used, too.


 Last edited: May 15, 2019

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

IcemanBo said:

 GUI Spell System has internal data structure and system variables, they are array.

Before the user spell "InternalSpell[SpellIndex]" runs, the usage of system variables gets simplified...

Duration[SpellIndex] becomes only Duration

Caster[SpellIndex] becomes only Caster

....
so it's easier to work with for the user. Technically, you also could use his internal system variables Duration[SpellIndex], for example, instead of only Duration, it will work the same.

The very same SpellIndex is also provided for the user, so he might use it, too. It might be useful for example, if the user wants to store additional information to the spell, like creating a temporary extra effect for the spell. So he stores is in his NewSpellEffect[SpellIndex] array variable, and can refer to it later.

In Chidori Lightning Storms v1.0, one can see some internal system variables are used, like ChidoriLS_BlastRange[Spell_i_Level[Spell__Index]], in the "Chidori LS Effect trigger". But instead ChidoriLS_BlastRange[Spell__Level] could be used, too.


Click to expand...

Thanks for pointing that out. I personally like it when I can see the index just to be certain of the whole spell, especially when there are external variables at play. I can ascertain all goes with index that way. Never know it had it's own internal data structure.

@Clanzion hope @IcemanBo clears your questions.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Solid overall.

There are a few minor things I would suggest however.

1. Use a different color for titles to make it easier to read

2. Indent the text underneath a title

How to become awesome

Just do it

​
3. I think the config trigger has waaaaaaaaaaaay too many comments. Its more comments than code. I suggest maybe putting the comments in a hidden bb tag to make them more optional. I think they can be useful, but I also think good variable naming should make it so no comments (or very few) are required. It also feels a bit weird to have parts of the tutorial inside the trigger code.

I would not say either of these are a must-change but giving you a few days to update if you feel like it before I approve it.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Chaosy said:

 Solid overall.

There are a few minor things I would suggest however.

1. Use a different color for titles to make it easier to read

2. Indent the text underneath a title

How to become awesome

Just do it

​
3. I think the config trigger has waaaaaaaaaaaay too many comments. Its more comments than code. I suggest maybe putting the comments in a hidden bb tag to make them more optional. I think they can be useful, but I also think good variable naming should make it so no comments (or very few) are required. It also feels a bit weird to have parts of the tutorial inside the trigger code.

I would not say either of these are a must-change but giving you a few days to update if you feel like it before I approve it.


Click to expand...

Something to surprise me after AGES

Anyway, thanks for the review. I'll do them ASAP.

EDIT:

1. Changed color for all texts. Cyan apparently is very irritating when I look at it again.

2. Hopefully all set now.

3. Config trigger is based on Bribe's own, but I made some adjustment now.


 Last edited: Oct 3, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Looks cleaner, nice.

Approved.

 You must log in or register to reply here.


### Similar threads

 MUI Spells Using Artificial Waits


Tank-Commander

Sep 28, 2012

Trigger (GUI) Editor Tutorials

Replies32Views20K

Oct 16, 2015
BETABABY

S

 How to make an AI for your map


sentrywiz

Dec 18, 2013

Trigger (GUI) Editor Tutorials

Replies48Views38K

Nov 19, 2019
GAcrewkent

D

Locked

[Trigger]
 Need MUI help :(


d.o.g

Jun 15, 2009

Triggers & Scripts

Replies6Views1K

Jun 15, 2009
d.o.g

D

 General Spell Creation


Chaosy

Jun 11, 2013

Trigger (GUI) Editor Tutorials

Replies35Views10K

Feb 8, 2014
PurgeandFire

Locked

[Solved]
 Simple MUI spell


Naze

Mar 20, 2017

Triggers & Scripts

Replies7Views2K

Mar 22, 2017
Naze

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