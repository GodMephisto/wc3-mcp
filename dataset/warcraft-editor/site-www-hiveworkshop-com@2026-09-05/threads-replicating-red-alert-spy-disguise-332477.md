---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/replicating-red-alert-spy-disguise.332477/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Replicating Red Alert Spy Disguise
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/replicating-red-alert-spy-disguise.332477/>
#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

GOAL

This tutorial aims to allow you to replicate the disguise ability used by Spies in Red Alert 2 and Red Alert 3. This also extends to Saboteur and Infiltrator from Mental Omega.

KNOWLEDGE REQUIRED

Basic Knowledge of Trigger Editor

Basic Knowledge of Object Editor

Some of your time
TOOLS REQUIRED

World Editor patch 1.32+

Warcraft III patch 1.32+
TABLE OF CONTENT

Introduction

Major Notes Prior to Start

Setting Up the Object Editor

Setting Up the Trigger Editor

Additional Notes
Introduction

Spies in Red Alert is a vital Allied unit that can disguise as enemy troops, and infiltrate enemy buildings to provide the spy owner additional advantage. They are unarmed and rely on their disguise to avoid combat. Today, we will replicate this effect into Warcraft III with the power of the new Skins natives and good old Mechanical Critter.​

Major Notes Prior to Start

This tutorial will not explain where is what, since you are assumed to know where to find X or Y in both Object Editor and Trigger Editor. This method also has one major limitation: it relies on abusing the Mechanical Critter's ability to attain the perfect replication, which by its own is very limiting. If you aim to have the spy unit trainable, then this will require additional workarounds (which is not part of the tutorial but hints to handle this pickle will be thrown at the last section).​

Setting Up the Object Editor

There are several pieces of stuff in the object editor we need to do before start triggering our Spy's disguise.​

Prepare your Spy and Lead Spy unit. The Lead Spy will be the one spawning the Spy unit via Mechanical Critter. I use Spell Breaker for Lead Spy and Footman as Spy.

Create an ability based on Chaos. If you use the Siege Engine variant, make sure to remove Barrage from the requirement. Refer to the image below for an example.

Create another ability based on Channel. This is the selector ability that is used by the player to select the target disguise. Refer to the image below for necessary configurations.

Add the ability from step 3 to your Spy. Step 2 will be relevant during the trigger section.

Setting Up the Trigger Editor

There's quite a number we need to do here. To start off, we will begin with the clickable Disguise ability (the one based on the channel ability). Below is the minimum trigger needed to create a visual replica of the target.​

[/INDENT]

[INDENT]Disguise[/INDENT]

[INDENT] Events[/INDENT]

[INDENT] Unit - A unit Starts the effect of an ability[/INDENT]

[INDENT] Conditions[/INDENT]

[INDENT] (Ability being cast) Equal to Disguise[/INDENT]

[INDENT] Actions[/INDENT]

[INDENT] Unit - Set Unit Skin of (Triggering unit) to (Skin ID of (Target unit of ability being cast))[/INDENT]

[INDENT] Unit - Change color of (Triggering unit) to (Color of (Owner of (Target unit of ability being cast)))[/INDENT]

[INDENT]

This trigger will change the unit skin (visual) to match the target, and of course, we also change the color for good measures (cause the only thing an opposition faction color does is marking a big red target on the spy's head). If you wanna be a bit pickier, you can also change the name, HP (though that could be problematic, but eh, whatever), MP, and so on.​

​

Now, if we just plug this into our spy and call it a day, there will be one big problem: the Enemy will auto-attack the spy, even if they look friendly. This is where the whole mechanical critter section comes in. We need to prepare another trigger.​

[/INDENT]

[INDENT]Spy Enter[/INDENT]

[INDENT] Events[/INDENT]

[INDENT] Unit - A unit Spawns a summoned unit[/INDENT]

[INDENT] Conditions[/INDENT]

[INDENT] (Unit-type of (Summoning unit)) Equal to Lead Spy[/INDENT]

[INDENT] Actions[/INDENT]

[INDENT] Unit - Add Object Editor Step 1 Ability to (Summoned unit)[/INDENT]

[INDENT]

Remember the whole chaos shenanigan we set up at the start? Since Mechanical Critter is very limiting, we can cheat this with the Chaos ability to change the critter to the unit we need. This is a problem if the unit has multiple summoning ability, so if that's the case, have a dummy unit do the critter summoning job instead.​

​

DONE! Now place your "Lead Spy" and test the map. You can scatter random owned units for example to see it in action.​

Additional Notes

If you use Footman as the base for Spy like my example, you'll notice that the unit is still attacked in combat if the Footman attacks the enemy. This can be fixed by removing the attack ability (if one wants to preserve the UI information) or disable attacking for the Spy altogether.

In case of multiple spawn ability owned by the Lead Spy, use this format: Lead Spy cast dummy ability -> spawn a dummy -> dummy cast mechanical critter -> have trigger detect dummy cast -> change critter to spy.

The Order for Mechanical Critter is "mechanicalcritter" and seems to not possess a GUI equivalent. This action can be used to order a unit to force cast it: call IssueImmediateOrderBJ( unit, "mechanicalcritter" ) with unit replaced with proper unit data.


 Last edited by a moderator: Jun 21, 2021

#### Cherrymage

#### Cherrymage

 Level 22

JoinedNov 4, 2010Messages6,233

Resources: 6

#### Resources
Models 3Maps 2Spells and Systems 1

Amazing! Always dreamt of remaking this mechanic in Warcraft III.

Shame that it only works in patches above 1.32 :c

There's a problem though, which requires more workarounds: Say you have three FFA players, all hostile to one another. If player 1 has a spy disguised as a P2 unit, P3 units will still ignore the spy because Mechanical Critter makes the spy seem neutral to everyone else. Maybe we need to trigger auto-attack in this case, albeit, it might seem too long-winded for this tutorial?

By the way, a minor issue: if I am not mistaken, the Chaos ability turns temporary stat changes (e.g. aura movement speed bonus, aura armor bonus) into permanent changes whenever added/applied. Maybe consider telling users to remove aura bonuses (and drop items), or at least a heads-up?

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Yes. The ignore is a problem but this is the closest we have for now. Elseway would be heavy trigger system with lots of forcing.

Ah yeah, forgot that pickle. Thanks for the heads up!

#### Cheshire

#### Cheshire

 Level 26

JoinedMar 29, 2020Messages1,482

Resources: 4

#### Resources
Icons 3Maps 1

this looks familiar...

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Ender Wiggins said:

 this looks familiar...

Click to expand...

Yes, it is based on your answer from our talk for the Mechanical Critter part. The other is based on the unit skins research.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Wouldn't it better to:

1. Make an ability based on channel

2. When ability is cast, create the unit. This would replace your "a unit is summoned" trigger

Since you can change the base order id of channel this solution is more universal.

You could also add the disguise ability to the spy in the object editor.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Chaosy said:

 Wouldn't it better to:

1. Make an ability based on channel

2. When ability is cast, create the unit. This would replace your "a unit is summoned" trigger

Since you can change the base order id of channel this solution is more universal.

You could also add the disguise ability to the spy in the object editor.


Click to expand...

Blizzard lacks native to prevent a unit from being auto-acquired by enemies with exception of Mechanical Critter ability. Units created via trigger will remain auto acquired.

Yes, one can use dummy and have another ability (or heck, trainable unit) for the spy lead unit, but you will in the end do the mechanical Critter trick to accomplish the not auto acquired part of the code.

Only alternative I know is, if one is insane enough, is to rewrite the whole auto-acquire system with their own.

#### Lt_Hawkeye

#### Lt_Hawkeye

 Level 12

JoinedJul 4, 2016Messages636

This could work with versions older than 1.32 by replacing unit skin with morphing.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Clanzion said:

 This could work with versions older than 1.32 by replacing unit skin with morphing.


Click to expand...

Indeed, although one has to provide all the necessary morphings. I'll study if there's a comfortable way for 1.29+ with the exposed natives

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

I tested in 1.32. The native is only existent in 1.32 and 1.33 versions. What version do you use and where does it 'don't work'?

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

This is cool! Glad this exists (shame it's only in the later versions ). I've wanted to mess around with Mechanical Critter (in the hopes of something akin to the RA2 Spy mechanic) for a while.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Kyrbi0 said:

 This is cool! Glad this exists (shame it's only in the later versions ). I've wanted to mess around with Mechanical Critter (in the hopes of something akin to the RA2 Spy mechanic) for a while.


Click to expand...

For earlier version, I think by utilizing some hacky trickery might lead to similar results, but I am not sure how it will go

 You must log in or register to reply here.


### Similar threads

Locked

 Spying system


operation3000

Dec 10, 2010

World Editor Help Zone

Replies1Views2K

Dec 10, 2010
Tleno

Locked

[Trigger]
 spell about disguise into an enemy?


leokiryu

Jul 13, 2015

Triggers & Scripts

Replies5Views2K

Jul 15, 2015
Ofel

Locked

[Spell]
 Box of Disguises


Ilusioner

Sep 27, 2016

World Editor Help Zone

Replies7Views3K

Sep 28, 2016
Ilusioner

Locked

 spying ability trigger


operation3000

May 15, 2010

Triggers & Scripts

Replies8Views2K

May 16, 2010
operation3000

Locked

 The spying ability


operation3000

May 7, 2010

World Editor Help Zone

Replies38Views4K

May 15, 2010
operation3000

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