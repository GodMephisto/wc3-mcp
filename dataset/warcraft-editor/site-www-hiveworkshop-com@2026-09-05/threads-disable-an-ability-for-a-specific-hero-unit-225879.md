---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/disable-an-ability-for-a-specific-hero-unit.225879/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Disable an Ability for a Specific Hero Unit
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/disable-an-ability-for-a-specific-hero-unit.225879/>
#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

#
 Exposition


Blizzard offers the jass function SetPlayerAbilityAvailable


or in GUI

Player - Enable/Disable Ability
which allows us to hide/disable an ability from the command card of a unit. In contrast to UnitRemoveAbility


or in GUI

Unit - Remove Ability
the cooldown of the ability keeps running in the background and any momentary casts of this ability do not get interrupted. That is quite the advantage.


Unfortunately, the action covers all the units of a given player and there is no variant for a specific unit.


Fortunately, this tutorial will present some solution.


I have to differentiate between heroes and units. Well, the unit way could be used for heroes too, but it's more cumbersome and less performant. Also, the procedure for non-heroes harbors danger. I cannot say if it's practicable in the end but maybe with the proper configurations.


Anyway, read For Heroes first. The second part references it.

#
 For Heroes


There is a very useful ability for us mapmakers out there called Engineering Upgrade. Its raw code is ANeg, so I will name it that way in this tutorial from now on. The ability holds the power to convert another ability on the unit to a different one. You have to specify both the source and the target ability.


ex:

ANeg with Blizzard (source) to Bash (target)


If you add this ANeg to a unit with the Blizzard ability, it will end up with Bash instead.


Now, I have to tell that when transforming an ability this way, the target ability on the unit inherits some of the source ability's features, such as the remaining cooldown. Furthermore, if you change the source ability while the unit is currently casting it, the unit won't be bothered the least and continue normally. This can be well observed on a channeling spell like Blizzard for example. So two main properties I ascribed to SetPlayerAbilityAvailable above are fulfilled.


We only need to hide the target ability (and ANeg) now. To not get in conflict with any other stuff, we may create a new dummy ability. I will refer to it as Placeholder. It can basically be of any base, or close to. I usually choose Agyb (Gyrocopter Bombs) for an effectless non-active ability although it could make sense to use the Slow Aura of Tornado or Attribute Bonus here since those abilities do not possess an icon in the first place/can be hidden by configuration. Anyway, Placeholder and ANeg can both be shrouded by SetPlayerAbilityAvailable, which should be done at map init. Note that ANeg still works when disabled.

Simplification note: I found that the Placeholder can also be the ANeg itself. The unit will have it twice then but it works.


To reveal the ability again, remove ANeg from the unit. The conversion will be reverted with the cooldown still intact. Since ANeg does not remember the source ability it transformed but instead looks up its object data, each ability we want to be able to hide requires another Placeholder plus another ANeg (or only another ANeg if ANeg is the Placeholder).

#
 For Non-Heroes


Why can we not do the same process for non-heroes? Wc3 will crash when trying to add ANeg to a normal unit. So the plan is to disguise the unit as a heroic hero. The only way I have found to accomplish that is by morphing the unit into another (hero) unit type. Doing so is dangerous according to Dr Super Good, and I believe so myself, but maybe it's okay with the proper settings or conditions. The game won't immediately shut down but may run into problems later on when events happen. Another contra is that not each morphing method interrupts the unit's order but it will reset the animation.


Now, if we just morph the unit, add ANeg to hide the ability, then remorph the unit to its original type, that won't work because the unit would still hold ANeg without being a hero and Wc3 does not like it. If we could only remove ANeg before reverting and still keep the ability transformation, our problem would immediately vanish. There comes another trick into play:

Reversed Engineering Upgrade Trick:


As I have stated above, ANeg does not keep in mind which ability it has been used on, rather looks it up from its object editor data. What do you think will happen when we swap source and target ability? Placeholder becomes the source, the ability we want to hide becomes the target. Our unit does not possess Placeholder in the beginning. Nothing will happen when we add ANeg. When we take ANeg away again, however, it will "revert" our original ability by Placeholder. Transformation successful, without keeping ANeg.


The reason I have not suggested this above is because it needs another ANeg now to get from Placeholder to the original ability, one more object.

Additional information on morphing units: A unit will lose all abilities which the target unit type does not possess, while morphing, unless you apply the function UnitMakeAbilityPermanent before. This is necessary to keep Placeholder/original ability. I usually have it combined with UnitAddAbility in a header function. UnitMakeAbilityPermanent does not have a GUI equivalent.


Summarized:

to disable ability:


morph unit to hero


add ANeg1 (Placeholder to original)


remove ANeg1 (Placeholder to original)


morph unit back

to enable ability:


morph unit to hero


add ANeg2 (original to Placeholder)


remove ANeg2 (original to Placeholder)


morph unit back


Finally, I would like to recommend the morphing method and make some comments on the hero unit type.


Of course, we want to be able to do this without any delay. Therefore, the active ones like Metamorphosis are no good, you would also need to trigger orders etc., which would again interrupt casts. Chaos (Srtt) is not sufficient either as it suffers from some minimal delay. The choice goes to the method described here. The trick it makes use of is practically the same as in Reversed Engineering.

Comments on the hero unit type: Blizzard did not account for morphing units into heroes/backwards. Heroes benefit from hero attributes, which influence stats such as hitpoints or armor. When the unit reverts into non-hero form, those bonuses are not dissolved, so the unit ends up with false values. To counter this, just set hero attributes to zero. Also set the base armor value of heroes to 0 (gameplay constant Hero Attribute - Defense Base (before Agility Bonus)). You can equalize this by manually adding the default base armor to each hero in your map. In addition, check Values - Hero - Hide Hero Interface Icon, else the hero portrait will persist.


Test maps are attached.

#### Attachments

disableAbilityForHero.w3x

 16.6 KB

 · Views: 902



disableAbilityForUnit.w3x

 17.2 KB

 · Views: 650



#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Wow this works beautifully.  This is a very creative technique and props for testing it thoroughly/getting it to work for regular units.

Approved.

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

I have just found out that if you use this Event, it won't work.

Unit - A unit Starts the effect of an ability
Why ?

Because the Event loaded first, doesn't even give the caster a chance to successfully cast the spell, therefore, the spell will be "removed" or hidden and its cooldown won't goes off because technically, the spell isn't being cast at all.

Solution: Use Unit - A unit Finishes casting an ability

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

You should always use the end cast event unless your spell is channeling. You just need to transfer the event responses from spell effect. Well, you would build your own events anyway rather than to write native solutions everywhere.


Since ANcl abilities can be hidden on certain levels and switching the ability levels does not revoke the cooldown, is there any point to prefer the approach here?

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

Well, my point is to point out that you should not use that Event, maybe you should include that in the main post ?

Users need to know the limit/boundary of a certain solution.

 You should always use the end cast event unless your spell is channeling.


Click to expand...

Are you trying to say that if your spell is channeling, it's okay to use A unit Starts the effect of an ability ?

If this is what you're saying, then it's wrong.

You still won't be able to make it goes on cooldown if you use that Event, Channeling or non-channelling spell - or perhaps I'm mistaken for your statement.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

defskull said:

 Are you trying to say that if your spell is channeling, it's okay to use A unit Starts the effect of an ability ?


Click to expand...


My point is that you would want to detect the start of a channeling spell but end cast fires afterwards.

#### Cokemonkey11

#### Cokemonkey11

Spell Reviewer

 Level 30

JoinedMay 9, 2006Messages3,572

Resources: 25

#### Resources
Tools 4Maps 8Spells and Systems 3JASS 7Tutorials 2Wurst 1

This is a great tutorial. I've tried implementing it in my Orianna spellpack (which had this existing bug that the previous method I was using didn't passify an ability)


But just like the existing version, the passification fails to work.


Maybe you have an idea of what's wrong? Casting Q, E, and R should temporarily make all abilities passive, and when they return their cooldowns should persist.

#### Attachments

(4)Orianna102b.w3x

 168.3 KB

 · Views: 440



#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

The signature of the abilities you want to swap should be the same (use another channel ability). Else a lot of strange stuff can happen because part of the original spells properties are transfered. In your map, the passive abilities actually are given to the unit but are not displayed.


Anyway, with channel, you can simply append a level that is not visible and change to that one while it should be disabled. In my map, I did well with completely disconnecting the hero skill menu from the actual spells the unit can cast.

#### Cokemonkey11

#### Cokemonkey11

Spell Reviewer

 Level 30

JoinedMay 9, 2006Messages3,572

Resources: 25

#### Resources
Tools 4Maps 8Spells and Systems 3JASS 7Tutorials 2Wurst 1

WaterKnight said:

 The signature of the abilities you want to swap should be the same (use another channel ability). Else a lot of strange stuff can happen because part of the original spells properties are transfered. In your map, the passive abilities actually are given to the unit but are not displayed.


Click to expand...


Oh. That sounds important, maybe put it in the tutorial.

 Anyway, with channel, you can simply append a level that is not visible and change to that one while it should be disabled. In my map, I did well with completely disconnecting the hero skill menu from the actual spells the unit can cast.


Click to expand...


Yeah, but I had a problem with that, which is why I'm trying other methods. Maybe you have an idea of what's wrong there - https://www.hiveworkshop.com/forums/spells-569/orianna-spellpack-1-0-1-a-244925/

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

line 653: you check the disabled ability instead of the original


You should not mess with the ability on SPELL_EFFECT, since it is still being casted there.

#### Cokemonkey11

#### Cokemonkey11

Spell Reviewer

 Level 30

JoinedMay 9, 2006Messages3,572

Resources: 25

#### Resources
Tools 4Maps 8Spells and Systems 3JASS 7Tutorials 2Wurst 1

WaterKnight said:

 line 653: you check the disabled ability instead of the original


Click to expand...


This is a mistake in the pasted script, but doesnt exist in the uploaded map (oops)

 You should not mess with the ability on SPELL_EFFECT, since it is still being casted there.


Click to expand...


If I change it to SPELL_FINISH it fails because SPELL_FINISH doesn't include data for GetSpellTargetX(), among other things. I could put everything on a short timer to *try* this solution but it would take a lot of time. How confident are you that that's the problem?

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Cokemonkey11 said:

 Oh. That sounds important, maybe put it in the tutorial.


Click to expand...

I did not know about that or forgot it. Most abilities in my map are Channel, thus I might not have observed it.

Cokemonkey11 said:

 If I change it to SPELL_FINISH it fails because SPELL_FINISH doesn't include data for GetSpellTargetX(), among other things. I could put everything on a short timer to *try* this solution but it would take a lot of time. How confident are you that that's the problem?


Click to expand...

I tested it. I converted it to a trigger action and put a wait in the orianna.attack function. If you do it on SPELL_EFFECT, the ability's level is changed and then is cast -> since you currently do not have a proper sixth level in object editor, you gain the default properties of Channel.

It's fairly easy to store the event responses of SPELL_EFFECT on the caster and then retrieve them on SPELL_ENDCAST or SPELL_FINISH.

Ps: I believe Brilliance Aura was said to be able to fatal on IncUnitAbilityLevel. If you need passive icons for show, take abilities that really do nothing else like Gyrocopter Bombs or Moon Glaives. You do not need to cast them, so any amount of them on a single unit works.

#### Cokemonkey11

#### Cokemonkey11

Spell Reviewer

 Level 30

JoinedMay 9, 2006Messages3,572

Resources: 25

#### Resources
Tools 4Maps 8Spells and Systems 3JASS 7Tutorials 2Wurst 1

WaterKnight said:

 I did not know about that or forgot it. Most abilities in my map are Channel, thus I might not have observed it.


Click to expand...


In my map it was also channel. Is there a good passive ability icon to replace channel with that won't cause side-effects?

 I tested it. I converted it to a trigger action and put a wait in the orianna.attack function. If you do it on SPELL_EFFECT, the ability's level is changed and then is cast -> since you currently do not have a proper sixth level in object editor, you gain the default properties of Channel.


It's fairly easy to store the event responses of SPELL_EFFECT on the caster and then retrieve them on SPELL_ENDCAST or SPELL_FINISH.


Click to expand...


I didn't have to! I used a timer with a duration of 0. and it worked perfectly. Thanks so much, this bug has been around for nearly a year.

 Ps: I believe Brilliance Aura was said to be able to fatal on IncUnitAbilityLevel. If you need passive icons for show, take abilities that really do nothing else like Gyrocopter Bombs or Moon Glaives. You do not need to cast them, so any amount of them on a single unit works.


Click to expand...


Yes, you have looked at two versions so far. One increments channel to a hidden slot but was broken because incabilitylevel to a hidden slot in the same thread as the SPELL_EFFECT condition caused it to reset cooldown.


The other version used the thing described in this tutorial.


The new version (and the solution) works like the first, but adds a timer before calling Orianna.attack().


Thanks again.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Cokemonkey11 said:

 In my map it was also channel. Is there a good passive ability icon to replace channel with that won't cause side-effects?


Click to expand...

You can level Channel into invisibility, then give the unit another passive ability. Gyrocopter Bombs or Moon Glaives are good because they are just for show, their gameplay effects as you know from melee are done via researches.

#### Cokemonkey11

#### Cokemonkey11

Spell Reviewer

 Level 30

JoinedMay 9, 2006Messages3,572

Resources: 25

#### Resources
Tools 4Maps 8Spells and Systems 3JASS 7Tutorials 2Wurst 1

WaterKnight said:

 You can level Channel into invisibility, then give the unit another passive ability. Gyrocopter Bombs or Moon Glaives are good because they are just for show, their gameplay effects as you know from melee are done via researches.


Click to expand...


Right, that's what I've always been doing, but after ~a year of that aforementioned bug I tried using tinker upgrade instead.

#### Xisthename

#### Xisthename

 Level 3

JoinedMar 10, 2013Messages12

Resources: 1

#### Resources
Maps 1

This is amazing! Just what i needed for my star wars map

#### Legal_Ease

#### Legal_Ease

 Level 20

JoinedFeb 8, 2013Messages1,476

Resources: 33

#### Resources
Icons 13Packs 1Skins 3Maps 11Templates 1Tutorials 4

But what if I want to silence just one ability to show the disabled icon?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Legal_Ease said:

 But what if I want to silence just one ability to show the disabled icon?


Click to expand...

Use the steps given in the tutorial to disable the ability.

 Add a dummy ability with the disabled icon--base it off a passive so it cannot be cast. Make sure it has the same button position as the active ability.

Note: when you pause, you'll get a green icon (assuming you used a DISBTN as the "active" icon for an ability). To fix this, you'll have to import the DISBTN as a BTN and import a copy of it as its DISBTN*. It is redundant, but it works.

*Technically, you can save a bit of space using this method, but the technique hasn't been polished yet.

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

If I use this to disable a Spell Book, will the passive spells in the book still work? (Sorry, don't have Warcraft 3 near me right now)

#### Cheshire

#### Cheshire

 Level 26

JoinedMar 29, 2020Messages1,482

Resources: 4

#### Resources
Icons 3Maps 1

is there any way to use this to make the "build" ability invisible (yet usable) for only 1 unit type?

 You must log in or register to reply here.


### Similar threads

L

 Hero passive transformation


Lordshinjo

Jun 30, 2011

Misc/General Mapping Tutorials

Replies30Views48K

Mar 19, 2023
Illbean

Locked

 help with removing skills in the hero skill learning menu


sarumanthecursed

Jun 4, 2016

World Editor Help Zone

Replies3Views1K

Jun 4, 2016
sarumanthecursed

T

Locked

 Hidden Ability Icons


troglavi

Jul 15, 2010

World Editor Help Zone

Replies4Views2K

Jul 15, 2010
-SoulBurn

Locked

 Disable abilities


GIMLI_2

Jan 31, 2016

World Editor Help Zone

Replies13Views3K

Feb 2, 2016
kingkwong

 The Basics of Object Data


Ralle

Jan 11, 2006

Misc/General Mapping Tutorials

Replies0Views25K

Jan 11, 2006
Ralle

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