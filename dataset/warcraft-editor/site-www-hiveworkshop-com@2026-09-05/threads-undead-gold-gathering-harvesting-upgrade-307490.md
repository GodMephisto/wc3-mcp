---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/undead-gold-gathering-harvesting-upgrade.307490/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Undead Gold Gathering Harvesting Upgrade
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/undead-gold-gathering-harvesting-upgrade.307490/>
#### Raen7

#### Raen7

 Level 4

JoinedMay 14, 2018Messages27

Resources: 2

#### Resources
Tutorials 2

Intro

I was recently trying to build a map with "improved resource gathering" upgrades. This is very simple for Human and Orc (based off the Human "Improved Lumber Harvesting" upgrade), and semi-simple for Night Elf (the upgrade has to use a separate effect called "Gold harvesting bonus (Entangled)", or something close to that). However, it is ridiculously hard to implement for Undead. Hence, this tutorial will help explain why, and guide you through correct implementation of an effective Undead "Gold gathering upgrade".

Credits

To start with, I had found this sample map posted by Jampion: post. It worked fairly well; however, the trigger structure was a bit convoluted and confusing (especially the part where he sets a Unit-Group variable to "all units in playable map area" and then subsequently empties it). So I spent some time reverse-engineering it and refactoring it to make it A) more understandable, and B) much cleaner. The main problem with it, as I discovered, was the use of a nested "Pick every unit" loop, i.e. one inside another -- the references to "Picked Unit" inside the inner loop would not always refer to the correct unit.

Another helpful tip came from this thread (@ thehelper.net, not here, fair warning), where they posed the idea of using begins-construction & finishes-construction events to toggle a switch-value for a given building so that it could be checked by another trigger, e.g. so I could detect if it was still under construction or not.

Now, let's get into it.

What You Need

World Editor (duh)

Familiarity with Object Editor and Trigger Editor.

TL;DR

If you truly want to "just get going" and start playing around with the sample map and triggers, download them! You can choose between the full WCX, or just the triggers.zip file, which contains the WCT & WTG dump of the triggers, which you can import into any map using the Trigger Editor's "file \ import triggers" menu option.

A Quick Recap

If you're keeping things balanced, your other races will also have "resource gathering upgrades"; and as I said, it's quite simple for Human, Orc, and Night Elf. Use the Human "Improved Lumber Harvesting" (image 'R1') upgrade as a base. Copy and paste. Change the "Race" property, the requirements, names and tooltips. A few examples are shown at the end (not to detract from the main point of this guide). 'R2' shows the Human Gold Mining upgrade; as you can see, I just changed the "Data- Effect 1" to "Gold Harvest Bonus", and edited all the other stuff appropriately. 'R3' shows the Night Elf gold upgrade, in which I've circled the different Effect that you need to pick - "Gold Harvest Bonus (Entangle)".

Finally, the key thing you need to do to ensure this works, is to add the Upgrade to the Unit which will use it! In Human & Orcs, that's the Peasant or Peon. For Undead lumber, that's Ghoul (for Undead Gold, it's complicated, and that's hopefully why you're reading this tutorial!). For Night Elf, the Wisp gets the "improved lumber" upgrade, BUTthe "improved gold harvesting (entangle)" upgrade goes on the Entangled Gold Minebuilding! See? Little tricksy, not bad. With that out of the way, we can move on to the main point of this tutorial!

Starting with Objects

First, we'll need a new Custom Ability, based on the Undead "Blighted Gold Mine Ability". This is an ability that the Haunted Gold Mine (hereafter abbreviated as 'HGM') has, which has no UI (no icon or displayed text or tooltip), but which contains the data that governs the Acolytes' gold-gathering mechanics. See "Step 1" image. Note that you can (in theory) change other properties like "Max # of miners" and "Interval duration", but for simplicity, we're just going to double the "Gold per Interval", i.e. the 'gather rate', to 20.

Second, we need to create a new Custom Unit, based on the Undead "Haunted Gold Mine". Moar copy-pasta! Let's name the new unit "Haunted Gold Mine 2", to keep it simple. We edit its "Abilities - Normal" field to remove the default "Blighted Gold Mine Ability", and add our new one, "Blighted Gold Mine Ability 2". See "Step 2" image. Make sure to set its "Techtree - Requirements" to require the

Third, we'll add this new building to the "Structures Built" field for our Acolyte. See "Step 3". While this technically exceeds the available build-slots for a builder unit (you can only have up to 11 visible on the command-card), it doesn't matter because A) it's in the same spot as the default HGM, which overrides it, and B) it won't be available to build until the upgrade is researched (which we'll get to in Step 5).

Fourth, we finally get to create our new Custom Upgrade! Here, we copy-paste from the Orc "Berserker Upgrade" upgrade. See "Step 4" image. We use this one because it's a built-in "unit swapper" upgrade, i.e. it replaces all existing units of one type with another type, while also making the first type "UNavailable" (to build/train) and the second "Available".

Fifth, just to be thorough, we go back to the HGM2 building and make it "require" this upgrade in its "Techtree - Requirements" field. See "Step 5" image. In practice, this is not 100% required, but it makes things easier for the game to understand and helps keep our build-tree normal.

Step '5b', we have to actually assign the upgrade-research as an "Available Research" to a building where we want to research it from! The sample map puts it under the Necropolis, but this screenshot shows adding it to the Graveyard, since that's probably more appropriate. You can decide!

And Now the Hard Part: Triggers!

They're not really hard, just a little convoluted. The reasons behind this are complex, partially having to do with how Blizzard implemented the UD gold mine mechanics (and gold mines in general), and partially because we want to make the upgrade as seamless as possible to the user -- i.e. the player shouldn't need to know or be aware of how the unit-swapping and gold-gathering-rates are being done behind-the-scenes, nor should they have to worry about re-assigning their currently harvesting Acolytes to re-start harvesting on the new HGM2's! It should all "just work", like magic. And that's why you're here, because I spent over 12 hours futzing with and play-testing these triggers to make sure I covered as many use-cases and scenarios as I could!

In Step 6a, I set up my new trigger folder, some notes at the top of an empty trigger to provide some background and "issues". And in Step 6b, I create my variables -- probably the most important step.

Briefly, the purpose of each is as follows:

HGM_Counter, integer: keep track of the # of HGM's found for a player during a Research-finished or Construction-finished event run (the Research trigger).

Replace_Loop, integer: keep track of the HGM's replaced by HGM2's during the Replace trigger.

HGM_Owner, Player array: keep track of player ownership of HGM's.

TempPosition, point: current position of the HGM/GoldMine being replaced inside the Replace loop.

HGM_Position, point array: all HGM positions found during the Research loop.

HGM_Life, real array: hit-points for each HGM found during the Research loop.

TimerZero, timer: used to signal the end of the Research trigger and fire the Replace trigger.

TempGoldMine, unit: the currently-being-replaced Gold Mine in the Replace loop.

HauntedMines, unit array: all HGM2's created during the Replace loop.

Grp_ActiveMiners, unit group array: for each HGM, a group of its surrounding actively-mining Acolytes.

Grp_NearbyUnits, unit group array: for each HGM, a group of all nearby units (not structures).

Now we get to real gory details of this operation. Step 7 is the "Research" trigger, which fires after the new "Gold Mine Upgrade" upgrade is done.. (OR after a new HGM is constructed, if it was not finished by the time the upgrade was done -- but we'll get to that later). I have provided copious notes about what it does and why, including some explanation of why things seem a little strange but are necessary for it to work out. If you aren't sure of how to find any particular Trigger Action in the menus, use the "search" feature, or just ask.

There are two main loops here, the "For each Unit of type HGM", aka the "HGM finder loop", and then the "For each Integer A" loop, aka the "Unit finder loop". As stated in the notes, we first have to find all existing HGM's for the player, and set some variables to store their positions and properties. Once that's all done, we proceed to set up the Unit-groups. We have "Nearby Units" (all non-structure units within 300 points of the HGM), which we have to HIDE so that they're not "in the way" when we go to create the new HGM2's later (i.e. they can't block placement of the new building). And of course we have "Active Miners", which we want to hide too, but also, we want to re-issue their "Harvest" orders once the new HGM2 is in-place.

Step 8 shows the "Replace" trigger, which is fired after the "Research" trigger finishes and sets off 'TimerZero'. Here, we are looping through the previously determined set of HGM's (which, having been removed, now just have a standard Gold Mine in their place on the map), and replacing them with HGM2's with the appropriate life/gold-amount. Then, we UN-hide all the units that were hidden before. And finally, we re-issue the "Harvest" order to all previously detected harvesting Acolytes for the HGM (now HGM2!).

With me so far? Don't worry, it's almost over!

Step 9 is more of a convenience than anything else. Because when you right-click to order a Unit to do something (its default action against whatever target that is), it's called a "Smart" order. But remember back in Step 7 where we had that disabled "Or" condition-block in the middle of our "unit-hiding loop"? This trigger makes it so we don't have to worry about that. Alternatively, we could remove this trigger and go back and enable that "Or" block instead, while also disabling/removing the single condition just above it ("Current order of picked unit equal to order(harvest)"). But I digress.

Step 10 a/b are a pair of triggers that fixe the "bug" in the original wherein, if an HGM is under construction and the Gold Mine Upgrade is researched before said HGM is finished, it gets replaced with a full-on HGM2 instantly, instead of having to wait for construction to finish. Here, we use the unit's "Custom Value" (which is an Integer) -- we set it to '1' if it's under construction, and only when it finishes do we set it back to '0'. (It's actually un-initialized, by default, so it's neither 0 nor 1 nor any other number, on a unit that it's never touched for, but I digress. Again.)

The whole reason we do this is so that very first "If" condition in our big "HGM finder loop" can be checked -- we only want to find (and thus replace) HGM's ifthey're fully constructed.

--------------------------------------------------

Phew! You did it, congratulations. You can now go forth and make your very own map with an Undead gold-harvesting-bonus upgrade, just like all the other races. Speaking of, here are the "recap" pics of sample upgrades for Human & Night Elf. As stated before, Orc would be same as Human for both kinds.

Thanks again to all the folks here at HiveWorkshop and elsewhere whose ideas and posts from the past helped me fully and completely understand how to get this working and how to explain it in plain English. GL HF!

#### Attachments

ResUpg Tut all ScreenShots.zip

 1.7 MB

 · Views: 126



Acolyte Gold Mining Upg triggers.zip

 5.2 KB

 · Views: 114



Acolyte Gold Mining Upg sample.w3x

 33.1 KB

 · Views: 119




 Last edited: Jul 30, 2018

#### Raen7

#### Raen7

 Level 4

JoinedMay 14, 2018Messages27

Resources: 2

#### Resources
Tutorials 2

Update: fixed image links, using embedding properly. Yay!


 Last edited: Jul 29, 2018

#### Raen7

#### Raen7

 Level 4

JoinedMay 14, 2018Messages27

Resources: 2

#### Resources
Tutorials 2

Just realized I forgot a step— need to show adding the upgrade as an available research to a building. Sample map has it on the Necropolis. More appropriate place is probably Graveyard. TBD soon!

#### Selaya

#### Selaya

 Level 7

JoinedMay 30, 2013Messages210

what happens if you just give the haunted gold mine ability multiple levels and level it up (through research or triggers), will it bug

#### Raen7

#### Raen7

 Level 4

JoinedMay 14, 2018Messages27

Resources: 2

#### Resources
Tutorials 2

Correct, multi-leveling/leveling-up the "Blighted Gold Mine Ability" ability was the first thing I tried that did NOT work. Hence the crazy work-around.

Demo:

You can see from the messages here that, after the upgrade is finished, before the trigger actions run for each HGM, the BGM ability level is STILL 1! So the Upgrade itself doesn't even work as-advertised.

So yes, unfortunately for us, whatever Blizzard did with the HGM/BGM mechanics has made it impossible for us to SIMPLY implement a gold harvesting upgrade for Undead. Thus, we had to get creative. =)

#### Attachments

BGM Ability Upg sample NOT WORKING.w3x

 20.2 KB

 · Views: 103




 Last edited: Jul 29, 2018

#### Raen7

#### Raen7

 Level 4

JoinedMay 14, 2018Messages27

Resources: 2

#### Resources
Tutorials 2

Known bug 1, with sample map & tutorial: HGM constructed BEFORE upgrade is researched, will still get replaced with upgraded HGM2. Fixing soon!

Known bug 2: Acolytes trained from a Hall that's been ordered to "Rally" to a HGM, will not re-Harvest on the replaced HGM2. Fixing soon!

Known bug 3: Acolytes "on their way to" a HGM (i.e. moving towards it to harvest) when it's upgraded/replaced, will NOT retain their order, and will stop (having lost the target of their original order). Possibly complicated; fix unknown.


 Last edited: Jul 29, 2018

#### Pyrogasm

#### Pyrogasm

 Level 45

JoinedFeb 27, 2007Messages5,594

Resources: 1

#### Resources
Spells and Systems 1

Raen7 said:

 Known bug 3: Acolytes "on their way to" a HGM (i.e. moving towards it to harvest) when it's upgraded/replaced, will NOT retain their order, and will stop (having lost the target of their original order). Possibly complicated; fix unknown.


Click to expand...

These bugs also apply to allied units traveling to a HGM via right-click and hostile units that may be trying to attack the HGM before it becomes an HGM2, not just player owned acolytes. Hashtables make this easier; store 2 groups in the hashtable per HGM to keep track of ally and enemy units currently moving toward the HGM. 1 additional global group holds all units currently targeting an HGM.

A) When a unit is ordered to smart, move, or attack a haunted gold mine, you add it to the main HGM targeting group, add it to either the ally group or enemy group, and also store the HGM it's interacting with in the hashtable (under the ordered unit's handle ID, not the HGM's) so you can properly remove it from groups later. (if its issued the order for stun, which I think only has a # equivalent not an orderstring, just ignore it)

B) When a unit is ordered any other order (except stun) and it's in the HGM target group, remove it from the HGM target group.

C) When a unit dies, if its in the HGM target group remove it from all groups it is in and properly flush its hashtable data.

D) When an HGM is replaced with an HGM 2, loop through both its ally and enemy groups and re-order them to "smart" the new HGM2. (probably need to disable the A trigger while you do this)

#### Raen7

#### Raen7

 Level 4

JoinedMay 14, 2018Messages27

Resources: 2

#### Resources
Tutorials 2

Pyrogasm said:

 These bugs also apply to allied units traveling to a HGM via right-click and hostile units that may be trying to attack the HGM before it becomes an HGM2, not just player owned acolytes. Hashtables make this easier; store 2 groups in the hashtable per HGM to keep track of ally and enemy units currently moving toward the HGM. 1 additional global group holds all units currently targeting an HGM.


Click to expand...

Awesome feedback, and I love your ideas to fix it. I'm going to upload the "bug 1 fix" soon. If you want to take the sample map and try your hand at implementing your solution, that would be amazing. I'm not a hashtable expert at this point, but I'd like to see them in action as you describe. =)

"Bug 2" is proving much more difficult than I thought. The new sample map will contain my "attempt to fix it" but the RallyStore trigger is not detecting every Necropolis's order-issued event, and thus the "RallyTrain" trigger is only working occasionally (to make rallied Acolytes re-Harvest against the replaced HGM2). Very frustrating.

#### Pyrogasm

#### Pyrogasm

 Level 45

JoinedFeb 27, 2007Messages5,594

Resources: 1

#### Resources
Spells and Systems 1

You just need to re-order them to "smart" the HGM2 after it gets replaced. Fixing bug 2 also involves storing orders and re-issuing them once the HGMs are replaced with HGM2s. Unless I misunderstand the problem. If you need to re-set the rally point after HGM->HGM2 conversion you can do that, and even get the unit's current rally point. If you get it before removing the old HGMs it will properly return a unit, but if you do it after removing the HGM it will return "no unit". For example:

Unit - Set Rally-Point for (Triggering unit) to (Rally-Point of (Picked unit) as a unit)[trigger]

#### Raen7

#### Raen7

 Level 4

JoinedMay 14, 2018Messages27

Resources: 2

#### Resources
Tutorials 2

Pyrogasm said:

 You just need to re-order them to "smart" the HGM2 after it gets replaced. Fixing bug 2 also involves storing orders and re-issuing them once the HGMs are replaced with HGM2s. Unless I misunderstand the problem. If you need to re-set the rally point after HGM->HGM2 conversion you can do that, and even get the unit's current rally point. If you get it before removing the old HGMs it will properly return a unit, but if you do it after removing the HGM it will return "no unit".


Click to expand...

Thanks bud, I'll fiddle with it some more. Do you mean that I should store the individual Acolytes' orders to re-issue? Not (just) the Necropolis's? Because.. I thought that's what my "ReOrder" trigger was doing -- it was detecting Acolyte 'smart' orders against an HGM, and re-issuing them as 'harvest'. But that doesn't seem to affect the rallied Acolytes.

Bug fix #1 is up; full update to original post with new screenshots, new map, and new trigger-dump.

#### Raen7

#### Raen7

 Level 4

JoinedMay 14, 2018Messages27

Resources: 2

#### Resources
Tutorials 2

At this point, it's easier to just assume ALL nearby Acolytes to a given HGM should be re-issued the Harvest order on the new HGM2. If there are more than 5, the remainder just do nothing. Tough beans.

New "step 7" trigger with revised "unit-finder loop" logic:

Here's the thing that's really bothering me. I made a trigger like so:

It correctly detects AND replaces the trained Acolyte's order from empty(none) into 'harvest'. The debug-messages prove it. AND YET, when the Research trigger runs thru its "unit-finder loop", and detects the current order-string on the picked Acolyte, IT'S STILL EMPTY!! WTF. I give up for today. As I said, it's a helluva lot easier to just re-issue Harvest to all nearby Acolytes on each HGM.

As for the more general issue of "resuming ANY in-progress orders having targeted a replaced unit", that's a big topic and probably deserves its own tutorial (or at least its own thread somewhere). I'm not going to tackle that here. @Pyrogasm has a good framework/outline in his head, so maybe someone can give that a try at some point.

Final note on this: As I said above, I don't feel it's justified to spend hours upon hours on the task of making 'swapped-in' HGM's retain pending unit orders (in which they are the target), other than Acolyte harvesting. My reasoning stands thus: It's literally a one-or-two-time event in the entire course of play. You upgrade your existing mines, you MAYBE have one in-build-progress that gets swapped when it's done. That's it. Frankly, if you've got enemy units after your gold mines, or you're ordering your own units in a queue that involves a mine, you're probably a bit loony. =P So, GLHFDD!


 Last edited: Aug 7, 2018

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Huh!

#### Jampion

#### Jampion

 Level 16

JoinedMar 25, 2016Messages1,327

Resources: 1

#### Resources
Spells and Systems 1

You can achieve the same much easier by using chaos to morph the building. This way the unit stays the same and variables or orders referencing the building do not have to be updated.

All you need to do is to reset gold amount and reorder currently mining acolytes to stop and then mine again.

It seems you need to wait a short time before you can reset the gold and reorder to mine. 0.03 seconds worked for me and won't be noticeable.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Woah, test map?

#### Jampion

#### Jampion

 Level 16

JoinedMar 25, 2016Messages1,327

Resources: 1

#### Resources
Spells and Systems 1

I'll provide a simple one, because I am lazy. I think Raen7 already has solutions for the points my simplified map ignores.

Only works when you manually order to harvest and not use right click. It's not MUI, leaks and picking the currently mining acolytes is not 100% accurate. It's a showcase of the idea rather than a template.

Regarding gold mines in construction, this system can be applied independent from upgrades, so you could just replace gold mines once they are finished constructing.

#### Attachments

UndeadGoldMine.w3x

 19.2 KB

 · Views: 90



#### Pyrogasm

#### Pyrogasm

 Level 45

JoinedFeb 27, 2007Messages5,594

Resources: 1

#### Resources
Spells and Systems 1

Jampion said:

 Only works when you manually order to harvest and not use right click.


Click to expand...

Is that some limitation of the chaos morph on buildings process, or just a limitation of the test map because you only listened for the harvest order and not also the smart order? Maybe I misunderstand what you meant but I'm out of town without a computer so I cannot look at the map code.

#### Jampion

#### Jampion

 Level 16

JoinedMar 25, 2016Messages1,327

Resources: 1

#### Resources
Spells and Systems 1

Just my test map. I pick nearby acolytes with order harvest and order them to harvest again. With smart it would be more difficult, cause I would have to filter them to only pick the ones that were ordered smart on a gold mine. Smart is used for moving, repairing and other things as well, so you cannot simply assume all acolytes on smart are supposed to mine.

Even with harvest it's not accurate, because an acolyte could have been ordered to harvest another gold mine, but still be close enough to the first gold mine to be ordered to harvest it. Ideally one would have to use a system that keeps track of orders and targets to only pick acolytes that have been ordered to harvest/smart the relevant gold mine.

This however goes far beyond what I wanted to show, so it would only confuse people, trying to figure out how it works. I might release a fully working version later.

Edit: here is a better working map. Works with buildings in construction and right click. Acolytes are only reordered to mine if they have been mining the exact mine before.

Known Issue: if the gold mine is frozen (Freezing Breath) during the time it is upgraded, the unit morph is delayed. Since the gold mine loses its gold at the morph, the trigger to restore the gold runs before the morph resulting in 0 gold for the mine.

Possible solution: When a gold mine is upgraded it is added to a group and periodically checked wether it already morphed. Once it is morphed, the gold is restored and the unit is removed from the group.

#### Attachments

UndeadGoldMine2.w3x

 25.9 KB

 · Views: 103




 Last edited: Aug 25, 2018

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Very cool, thanks!

#### Raen7

#### Raen7

 Level 4

JoinedMay 14, 2018Messages27

Resources: 2

#### Resources
Tutorials 2

@Jampion nicely done buddy. I thought that the troll Berserker form was just another flavor of Chaos. If your method is easier to implement I'd sure love to see it done as a tutorial.

EDIT: I see yours is heavy on the JASS. Mine is aimed at pure GUI users, but I can definitely see the value and efficiency that the custom scripting adds.


 Last edited: Aug 26, 2018

#### Jampion

#### Jampion

 Level 16

JoinedMar 25, 2016Messages1,327

Resources: 1

#### Resources
Spells and Systems 1

Raen7 said:

@Jampion nicely done buddy. I thought that the troll Berserker form was just another flavor of Chaos.


Click to expand...

Yeah, you are right, I think it does the same. But I thought this tutorial (step 8) and my map I initially posted back then use the GUI replace function to replace the gold mines, which is problematical and is the main cause why so many workaround triggers (that can never be perfect) had to be created.

Replacing the unit removes it, so everything that references the gold mine no longer works (orders, variables, handle id). You can probably create triggers in a way, that this does not affect your triggers. However orders cannot fully be restored due to the order queue: If you ordered a unit to attack the gold mine and then do something else with shift, the something else part would be lost, because a trigger orders the unit to attack the gold mine after replacing it.

Berserker form or chaos has certainly issues as well, but they aren't as fundamental.

Raen7 said:

 If your method is easier to implement I'd sure love to see it done as a tutorial.


Click to expand...

Not sure if it is easier, but it should be working better. I also think that the total trigger length is shorter, just because you don't have to create so many workarounds.

The unit indexer I used in the map is from Bribe's Damage Engine, so it's not something one would have to do yourself.

I split the triggers into two parts: core and upgarde

Core handles upgrading a single gold mine and upgrade is a possible implementation of how to upgrade all gold mines, when you research an upgrade.

I would certainly try to use unit morph instead of replacing the gold mine. The replace function simply breaks to much stuff, including all buffs currently on the gold mine.

You can use as much from my map in your tutorial as you want.

Raen7 said:

 EDIT: I see yours is heavy on the JASS. Mine is aimed at pure GUI users, but I can definitely see the value and efficiency that the custom scripting adds.

Click to expand...

The UpgradeMine trigger is mostly JASS, apart from that it is all GUI. It's not like it cannot be created in GUI, it's just a lot easier and faster for me to do it in JASS.

The only difficulty for GUI I see is how to implement the 0.03 seconds delay. But even that could be done with dynamic indexing and a periodic timer, though it would be less accurate. What is the common technique to get accurate delays in GUI, while being MUI?

#### Pyrogasm

#### Pyrogasm

 Level 45

JoinedFeb 27, 2007Messages5,594

Resources: 1

#### Resources
Spells and Systems 1

Jampion said:

 What is the common technique to get accurate delays in GUI, while being MUI?


Click to expand...

Jampion said:

 dynamic indexing and a periodic timer, though it would be less accurate


Click to expand...

Or using TimerStart() on a function that ExecuteFunc()s a trigger's Action function. Trying to call a trigger's action function directly from the timer callback function usually results in an error because you can't force one trigger's functions to be before another's in the compiled war3map.j

#### Jampion

#### Jampion

 Level 16

JoinedMar 25, 2016Messages1,327

Resources: 1

#### Resources
Spells and Systems 1

I actually used a similar approach in this map, but I was more looking for a method that would be used by GUI users, since Raen7 said the tutorial was aimed at pure GUI users.

But I guess you have to use local timers, if you want MUI and accuracy.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Based on the feedback by @Jampion I choose to graveyard this for the following reasons:

This is a very niche tutorial, I would say. Not a dealbreaker but worth considering

There are other solutions to this that is arguably easier to work with

Images should either be smaller or put in hidden tags

There are a few other minor tweaks I would prefer when reading through long tutorials, makes structure/layout a lot more important

I would also argue that it would be more user friendly to simply make it a system and upload it to the spell section rather than following a step by step guide. Although there is an argument to be made that learning is better.
I think this CAN be salvaged, but as the author is inactive (for 2 years) I graveyard it until updated.

I want to applaud the effort however, salute.

 You must log in or register to reply here.


### Similar threads

 Gold Mine Distance Remover


bisnar13

Sep 7, 2012

Tutorial Archive

Replies11Views3K

May 1, 2013
PurgeandFire

Locked

 Upgrading Haunted Gold Mines


theoligarch

Dec 1, 2015

World Editor Help Zone

Replies10Views4K

Dec 3, 2015
theoligarch

P

Locked

 Replace Gold Mine with Haunted Gold Mine.


PRO3

May 5, 2017

World Editor Help Zone

Replies2Views2K

May 6, 2017
PRO3

P

Locked

 Custom Gold Mine(Rich Gold Mine)


Rattlehead

Aug 6, 2018

World Editor Help Zone

Replies5Views2K

Aug 16, 2018
Rattlehead

Locked

[Solved]
 Setting Gold of a Custom Haunted Gold Mine via trigger? (1.28)


PiesOfNorth

May 11, 2021

World Editor Help Zone

Replies0Views1K

May 11, 2021
PiesOfNorth

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