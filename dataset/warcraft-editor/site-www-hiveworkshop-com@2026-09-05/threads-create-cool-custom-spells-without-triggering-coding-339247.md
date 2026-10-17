---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/create-cool-custom-spells-without-triggering-coding.339247/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Create Cool Custom Spells Without Triggering Coding
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/create-cool-custom-spells-without-triggering-coding.339247/>
#### Deleted member 247165

D

#### Deleted member 247165

--- Create cool custom spells without triggering / coding ---

As we all know, whenever we create heroes for our projects we want it to have nice abilities and unique as well. The best way to do that is to use GUI/MUI or other coding language from the WE. BUT, there are people who didn't learn how to make spells using those or other people that simply don't want to use triggered spells and instead, they stick to the ones from the Object Editor, (And based on what I've seen while playing, reviewing, testing dozens of projects, there are many people who barely trigger the spells).

 Then again, triggering is the fastest way for custom spells since you can easily maneuver them and is highly recommended if you have a DDS in your project.

 I've been experimenting stuff in the WE recently, different combinations of abilities, weird values used for heights, attack types, passive abilities and so on, and I discovered some pretty cool stuff:

In the video above, we got 4 custom abilities and the way they are made is explained below:

#1 Instant Summon

This one is made using negative cast range on an instant summon. The summon is a dummy with the burning effect that expires after 1 second. To do that, set its life to a value and the regeneration to negative, calculating the time you want. ALSO VERY IMPORTANT SET THE COMBAT TARGET AS VALUE TO "DEBRIS" so that it wont be affected by other healing sources to have the time delayed xD

#2 Exploding Summon

Is another summon, using calculated values to make it explode after a set amount of time. For the damage, use the AOE Death of the goblin mines , same for the first spell.

#3 Arcing Stormbolt

Is actually a mere Storm Bolt edited with MISSILE ARC SET TO 4 so that it makes the missile go into the sky first then fall upon the target.

#4 Frost Barrage

Is a summoned dummy with flying height of 2500, attack rate of 0.05 (set the dmg you want) and to make it hit multiple targets, make a barrage ability and add it to it. With all of these put on, enemies around will be repeatedly hit by falling arrows (or anything you want).

 NOTE THAT IT IS VERY IMPORTANT TO USE SUMMON WATER ELEMENTAL FOR INSTANT CAST ABILITIES. The reason is because the dummy will have the lightning poof effect when the spell ends if you used for example Feral Spirit, Summon Quillboar, Misha, Spirit Bear etc.

In the second video we got 3 custom spells which are more eye candy let's say:

#1 Healing Blast

Is made using instant summon (Water Elemental) using a dummy that expires the same way as the spell in the previous video. Here we got normal cast range so it summons in front of the hero (you can put the range you want). For the healing, I used negative values on the land mine AoE dmg but even though I checked and tested the radius many times, it only applies the heal to targets near or outside the dummy and not in it. Weird, the damage value worked normally for other spells.

#2 Meteor Shower

Is actually Storm, Earth and Fire, using dummies with 3000 flying height, locust put on so they are invulnerable but they are still selected (actually good since you can target the enemy you want xD ). For the visuals, just have fun with the missile arc, effect you wanna use and the radius (it can be increased or decreased as you like).

#3 Lightning Burst

Is a targeted cast point summon which again uses a dummy with flying height of 3000 and has a lightning attack. In order to change the lightning type and the blast effect, simply edit the Chimaera's lightning attack and attach it to the dummy. Put the attack range you want and voila. A spells that causes lightning bolts to hit enemies on the ground.

In the third video, we got 4 custom spells that would fit a melee hero mostly:

#1 AoE instant dmg + debuff and magic immunity

Is made using Bladestorm actually, with 1.01 duration and DPS set to 450, turning it into an instant active damage ability that can be cast while moving and also dispels your hero and grants it magic immunity which can be extremely useful especially versus multiple opponents. KEEP IN MIND THAT IT ALSO SILENCES YOUR HERO FOR THE DURATION YOU SET.

#2 Using "permanent immolation" in combination with a dummy via any Summon

Is based on Water Elemental Summon, using a dummy for the lightning effect. Two Permanent Immolations abilities are added on it, one that deals 150 damage every 0.50 seconds to enemies and another one that heals allies for the same damage every 0.50 seconds. (For the heal, put negative values and set the targets allowed to ground, air, allied, friend).

#3 Double Damage AoE Attack passive

Is actually the Cleaving Attack with the splash effect modified, radius increased a lot and the value of the damage distributed doubled. This way, your MELEE hero can deal twice the amount of its damage to enemies around the primary hit foe. (it does not affect the target you're attacking and only those in the area of effect)

#4 Swarm summon with different attack

Is Locust Swarms (pretty obvious), but with no healing, only 5 dummies and set to affect enemies 1 per target. This means that each coil will target only one enemy and multiple coils will not hit an isolated enemy at the same time. For the cool lightning blast, use the Chimaera's attack, modify the lightning as you want using the passive Lightning Attack and then attach it to the dummy. DO NOT ATTACH ORB EFFECTS ABILITIES (made using Orb of Slow or Orb of Lightning), BECAUSE THEY WILL NOT FORCE RIGHT CLICK TO TRIGGER THE PASSIVE !

I was really having fun while doing these. I will, of course, upload other stuff once I find.

This was saved using patch 1.31. (See attach in the post). I uploaded it so that you can easily understand the tricks behind these spells.

If you also found something cool in the WE, please post

#### Attachments

Custom abilities no triggers.w3x

 60.5 KB

 · Views: 173




 Last edited by a moderator: May 1, 2022

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

A bit of a wall, try formatting.

Looks at other approved tutorials for inspiration.. or a map presentations.

#### Deleted member 247165

D

#### Deleted member 247165

I will! Thanks for the feedback

Edited: I wanted to make it look like a map bundle with nice things and so on, but I thought it would be too much. I will modify it no problem.


 Last edited by a moderator: Apr 20, 2022

#### Deleted member 247165

D

#### Deleted member 247165

Hey @Chaosy I improved the description of the post and also uploaded a third video

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Seems like there is some potentially useful stuff.

Some people do not want to learn coding and would like to use the object editor in a creative way instead.

I do have some thoughts however:

Alexen said:

 Then again, triggering is the fastest way for custom spells since you can easily maneuver them and is highly recommended if you have a DDS in your project.


Click to expand...

I don't think a DDS is relevant to this statement. Coding is the fastest way to achieve almost anything if you know what you are doing, regardless if you have a DDS or not. But even if I am biased as fuck it is objective that not all spells need to use one.

Alexen said:

The first one is made using negative cast range on an instant summon. The summon is a dummy with the burning effect that expires after 1 second. To do that, set its life to a value and the regeneration to negative, calculating the time you want. ALSO VERY IMPORTANT SET THE COMBAT TARGET AS VALUE TO "DEBRIS" so that it wont be affected by other healing sources to have the time delayed

Click to expand...

Is the negative cast range important here?

I also think the tutorial should be rebranded slightly, from the title it seems like you are going to show a method of making spells in general but it seems more like a "tips and tricks" type of thing.

#### Deleted member 247165

D

#### Deleted member 247165

The negative cast range value must be mentioned imo.

I pointed out "especially for the dds" to underline the fact that triggering is the best way for custom spells. But yes, triggering is the best way no matter, yet the things posted here help those that dont want to learn how to trigger.

I thought of naming it "tricks and tips".

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Edited the title + introduction + suggested layout, note that I only changed it for the first video so you can easily compare:

### Create Custom Spells Without Triggering / Coding

As we all know, whenever we create heroes for our projects we want it to have nice abilities and unique as well. The best way to do that is to use GUI/MUI or other coding languages from the WE. BUT, there are people who didn't learn how to make spells using those or other people that simply don't want to use triggered spells and instead, they stick to the ones from the Object Editor, (And based on what I've seen while playing, reviewing, testing dozens of projects, there are many people who barely trigger the spells).

 Then again, triggering is the fastest way for custom spells since you can easily maneuver them and is highly recommended if you have a DDS in your project.

 I've been experimenting with different combinations of abilities in the WE recently, weird values used for heights, attack types, passive abilities and so on, and I discovered some pretty cool stuff:

#1 Instant Summon

Is made using a negative cast range on an instant summon. The summon is a dummy with the burning effect that expires after 1 second. To do that, set its life to a value and the regeneration to negative, calculating the time you want. ALSO VERY IMPORTANT SET THE COMBAT TARGET AS VALUE TO "DEBRIS" so that it wont be affected by other healing sources to have the time delayed​

#2 Exploding Summon

Is another summon, using calculated values to make it explode after a set amount of time. For the damage, use the AOE Death of the goblin mines , same for the first spell.​

#3 Arcing Stormbolt

Is actually a mere Storm Bolt edited with MISSILE ARC SET TO 4 so that it makes the missile go into the sky first then fall upon the target.​

#4 Frost Barrage

Is a summoned dummy with flying height of 2500, attack rate of 0.05 (set the dmg you want) and to make it hit multiple targets, make a barrage ability and add it to it. With all of these put on, enemies around will be repeatedly hit by falling arrows (or anything you want).

NOTE THAT IT IS VERY IMPORTANT TO USE SUMMON WATER ELEMENTAL FOR INSTANT CAST ABILITIES. The reason is because the dummy will have the lightning poof effect when the spell ends if you used for example Feral Spirit, Summon Quillboar, Misha, Spirit Bear etc.​

In the second video we got 3 custom spells which are more eye candy let's say:

The first oneis made using instant summon (Water Elemental) using a dummy that expires the same way as the spell in the previous video. Here we got normal cast range so it summons in front of the hero (you can put the range you want). For the healing, I used negative values on the land mine AoE dmg but even though I checked and tested the radius many times, it only applies the heal to targets near or outside the dummy and not in it. Weird, the damage value worked normally for other spells.

The second one is actually Storm, Earth and Fire, using dummies with 3000 flying height, locust put on so they are invulnerable but they are still selected (actually good since you can target the enemy you want xD ). For the visuals, just have fun with the missile arc, effect you wanna use and the radius (it can be increased or decreased as you like).

The third one is a targeted cast point summon which again uses a dummy with flying height of 3000 and has a lightning attack. In order to change the lightning type and the blast effect, simply edit the Chimaera's lightning attack and attach it to the dummy. Put the attack range you want and voila. A spells that causes lightning bolts to hit enemies on the ground.

In the third video, we got 4 custom spells that would fit a melee hero mostly:

The first one is made using Bladestorm actually, with 1.01 duration and DPS set to 450, turning it into an instant active damage ability that can be cast while moving and also dispels your hero and grants it magic immunity which can be extremely useful especially versus multiple opponents.

The second one is based on Water Elemental Summon, using a dummy for the lightning effect. Two Permanent Immolations abilities are added on it, one that deals 150 damage every 0.50 seconds to enemies and another one that heals allies for the same damage every 0.50 seconds. (For the heal, put negative values and set the targets allowed to ground, air, allied, friend).

The third one is actually the Cleaving Attack with the splash effect modified, radius increased a lot and the value of the damage distributed doubled. This way, your MELEE hero can deal twice the amount of its damage to enemies around the primary hit foe. (it does not affect the target you're attacking and only those in the area of effect)

The fourth one is Locust Swarms (pretty obvious), but with no healing, only 5 dummies and set to affect enemies 1 per target. This means that each coil will target only one enemy and multiple coils will not hit an isolated enemy at the same time. For the cool lightning blast, use the Chimaera's attack, modify the lightning as you want using the passive Lightning Attack and then attach it to the dummy.

I was really having fun while doing these. I will, of course, upload other stuff once I find.

This was saved using patch 1.31. (See attach in the post). I uploaded it so that you can easily understand the tricks behind these spells.

If you also found something cool in the WE, please post

To be clear.. this exact layout is not a must but I do think it is a bit messy to read at the moment.

#### Deleted member 247165

D

#### Deleted member 247165

Thanks for the guidance! Is it okay now ? I also added some additional stuff for several spells.

 You must log in or register to reply here.


### Similar threads

Locked

 Passive Ability Triggers


zerox144

Jul 7, 2009

Triggers & Scripts

Replies2Views1K

Jul 7, 2009
zerox144

Locked

[Altered Melee]
 Cool Warlock unit/hero ideas UNFINISHED


Sapprine

Jan 20, 2023

Idea Factory

Replies1Views1K

Jan 21, 2023
noob

 Special Effects - Manipulation and Modification


N1ghthawk

May 30, 2010

Misc/General Mapping Tutorials

Replies12Views45K

May 20, 2024
Mekanical

Locked

 WarCraft3 RPG - Need Spells Ideas


Kuritany

Sep 3, 2009

Idea Factory

Replies2Views2K

Sep 3, 2009
Kuritany

Locked

 Cool Down for dummy spell doesn't go off


AznShadowFury

Sep 6, 2007

World Editor Help Zone

Replies4Views1K

Sep 9, 2007
Just_Spectating

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