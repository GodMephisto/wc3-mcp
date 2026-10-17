---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/passive-ability-with-cooldown-best-method.275068/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Passive Ability with Cooldown Best Method
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/passive-ability-with-cooldown-best-method.275068/>
#### leandrotp

#### leandrotp

 Level 9

JoinedJul 30, 2012Messages156

Resources: 1

#### Resources
Tutorials 1

## Passive ability with cooldown - the easy way!
The hardest part of this tutorial is formatting it properly

Introduction

​

First of all, this tutorial is not a copy of [thread=211589]this[/thread]. This tutorial presents a new way to make passives with cooldown. This is not attack-based, instead it gives you a passive ability with a cooldown that you can trigger any time you want.

I've known this trick for a very long time, and I guess it's more than time to make it public, as I couldn't find it anywhere else.

​

Exhume Corpses - the magic passive

​

The Exhume Corpses ability is the only passive ability with cooldown in the entire game!

I believe this has not been found before, because this ability does not have a cooldown by default. But in fact, it's just like any other spell: it can have a cooldown, a manacost, and it fires spell casting events whenever it's cast. The only difference is that it displays a button that cannot be clicked.

So how can we cast it? We don't need to do that, the Exhume Corpses ability automatically casts itself whenever possible!

​

Configuring Exhume Corpses

​

You can just make your custom ability based on Exhume Corpses and modify it as you like: you can change icons, tooltips, set a cooldown, and have multiple levels if you want.

Here I modify the icon and tooltips of Exhume Corpses to look like the Pocket Factory ability. I removed the techtree requirement and set a cooldown of 5 seconds.

Now the important thing is to set theDuration - Normal field to 0.01. This field determines how often the Exhume Corpses ability will check if it can be cast. Setting it to 0 would also work, but it causes the game to freeze some times (like if the unit is stunned), so the minimum safe value is 0.01.

The "Unit Type" and "Maximum Corpses" data fields can be ignored, we're not going to use them.

​

Triggering the cooldown

​

Exhume Corpses requires the Cargo Hold (Meat Wagon) ability ('Sch2') to be present on the unit. If this ability is not present, Exhume Corpses cannot be cast. Therefore, all you need to do is:

Unit - Add Cargo Hold (Meat Wagon) to <Your Unit>

After that, the Exhume Corpses ability will magically cast itself at every cooldown. But as you may know, Exhume Corpses creates a corpse with every cast. We need to remove this side effect, as we don't want the game to create a unit for no reason.

Fortunately that's very easy to achieve. To prevent the creation of the corpse we just need to disable the Cargo Hold ability. The easiest way to do that is with a requirement, just go to the Cargo Hold ability, select theTechtree - Requirements field, press Shift+Enter and type an invalid id:

Doing that, Cargo Hold will be always disabled for all players. Remeber that if your map uses Meat Wagons, this will interfere with them. You can fix that by using a custom Cargo Hold ability instead of the original (make sure that you use the Meat Wagon's Cargo Hold as the base)

Alternatively you can disable it directly with triggers:

Player - Disable Cargo Hold (Meat Wagon) for Player 1

But you would need to do that for all players. Using a requirement is much easier.

​

Triggering the effects

​

With everything ready, you can use your passive ability just like any other spell:

Pocket Factory
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Exhume Corpses

Actions
Unit - Create 1 Clockwerk Goblin for (Owner of (Triggering unit)) at (Position of (Triggering unit))

You can easily control when your passive will be cast simply by adding and removing Cargo Hold from the unit. And it does not interrupt the unit's orders when it goes on effect. In fact, this ability is really a spell, you just can't click on it.

Notice that when you add Cargo Hold to the unit, the ability will not be cast immediately, but only after a 0-second delay. So you should wait for the spell to finish if you want to remove Cargo Hold after that.

​

Tips and Tricks

​

### Having multiple passives on the same unit

In normal ocasions, a unit can't have more than one custom ability with the same ability as base. But since Exhume Corpses is the only passive with cooldown, this is not an option if you want a unit to have multiple of it.

The only solution is, whenever you want to make one of those abilities go on cooldown, you temporarily disable all the others:

Cast Passive 1
Actions
Player - Disable Passive 2 for (Owner of (Triggering unit))

Unit - Add Cargo Hold (Meat Wagon) from (Triggering unit)

Cast Passive 2
Actions
Player - Disable Passive 1 for (Owner of (Triggering unit))

Unit - Add Cargo Hold (Meat Wagon) from (Triggering unit)

​

And then you make a trigger to re-enable all abilities after casting any of them:

Multiple passives
Events
Unit - A unit Finishes casting an ability

Conditions
Or - Any (Conditions) are true
Conditions
(Ability being cast) Equal to Passive 1

(Ability being cast) Equal to Passive 2

Actions
Unit - Remove Cargo Hold (Meat Wagon) from (Triggering unit)

Player - Enable Passive 1 for (Owner of (Triggering unit))

Player - Enable Passive 2 for (Owner of (Triggering unit))

### Using Exhume Corpses with items

The Exhume Corpses ability can also be used with items. With this you can create a passive item that displays a cooldown, similar to the "Amulet of Spell Shield".

However, Exhume Corpses doesn't work if you put it directly on the item. So instead, you must put it in a Spellbook, and then put this spellbook on the item:

Make sure that Shared Spell Cooldown is set to true in the Spellbook. After that, you can put the Spellbook on an item that cannot be actively used:

Doing that, Exhume Corpses will work perfectly inside an item. When it gets cast, it will cause the item to go on cooldown, and it will even fire a "Unit uses item" event. And if the unit drops and re-acquires the item, the cooldown will still be there and work as expected.

### Forcing the ability to cast immediately

When you add the Cargo Hold ability to a unit, your passive ability will not be cast immediately, but only after a small delay. This is the delay that you put in theDuration - Normal field, you can't set it to 0, and even if you did, it still wouldn't be instant.

But if for some reason you really need to bypass this delay and cast your ability instantly, you can simply give the order directly:

Custom script: call IssueImmediateOrderById(<Your Unit>, 852537)

The order id for Exhume Corpses is 852537. When you issue this order, if Cargo Hold present on the unit, Exhume Corpses will be cast immediately, ignoring the delay.

However this has the downside that it interruputs the current orders of the unit, so I do not recommend using this method unless you really know what you're doing.

​

Conclusion

​

I have attached a Test Map demostrating the passive with cooldown. In this example it creates a Clockwerk Goblin every 5 seconds, while displaying a cooldown indicator to the user.

If you have any questions or suggestions, please let me know. And I'm sorry for being too lazy to write this guide before, I have really found this trick a long time ago.

​

Changelog

​

02-04-2016

-Added information about using Exhume Corpses with items

-Added information about having multiple passives on a unit

-Added information about making the ability cast immediately

-Changed minimum delay of the ability to 0.01 to prevent game freeze

01-27-2016

-Initial release

​

#### Attachments

Exhume Corpses.png

 97.6 KB

 · Views: 2,248



Cargo Hold.png

 86.8 KB

 · Views: 2,168



TestMap.w3x

 16.4 KB

 · Views: 678



Spellbook.png

 100.1 KB

 · Views: 2,057



Item.png

 103.3 KB

 · Views: 2,047




 Last edited: Feb 4, 2016

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Excellent tutorial! Thanks for sharing; I've seen this requested many times before--and I personally wasn't aware that this ability had that field.

Approved.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Wasn't aware of this either. Will it still work if the unit is stunned?

#### leandrotp

#### leandrotp

 Level 9

JoinedJul 30, 2012Messages156

Resources: 1

#### Resources
Tutorials 1

Flux said:

 Wasn't aware of this either. Will it still work if the unit is stunned?


Click to expand...


Not only it doesn't work, but it caused the game to freeze when the unit was stunned! Thanks to your post, I could find that bug, now I set the duration to 0.01 instead of 0, and it doesn't freeze the game anymore (but unfortunately, the ability doesn't go on cooldown while the unit is stunned)


I have modified some things in the guide, fixed this bug, and added some new bits of useful information. If you have something more to say, I'd like to hear it.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

how the hell i've missed this?

you didn't wrote about "cooldown doesnt starts if unit cannot act (disabled)" in guide tho, only note about 0.01

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

I noticed how exhume corpses works back when i was playing melee maps but it never came to my mind to use it for this kind of trickery! well done.

#### Lign

#### Lign

 Level 6

JoinedAug 19, 2014Messages72

Resources: 4

#### Resources
Maps 4

Thanks for this awesome tutorial =)

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

I thought Spell Shield(or Spell block, I forgot the term) also has a passive cooldown?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I wonder if this is the best best method though..

Make two versions of the passive ability, make one do nothing. (+0 armor for example)

Use some local variables and a TriggerSleepAction to swap them, easy.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

Chaosy said:

 I wonder if this is the best best method though..


Make two versions of the passive ability, make one do nothing. (+0 armor for example)


Use some local variables and a TriggerSleepAction to swap them, easy.


Click to expand...

why?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

What do you mean "why", I just don't think the tutorial explains the easiest way to make a passive have a cooldown.

edit: on second though, my original approach was slightly invalid.

But regardless, using this method is not flexible. Making a passive with a cooldown is not the same as automatically procing every x seconds.

Example: "One attack every x seconds got 100% crit chance"

In which case this method does not work.


 Last edited: May 12, 2016

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Chaosy said:

 What do you mean "why", I just don't think the tutorial explains the easiest way to make a passive have a cooldown.

edit: on second though, my original approach was slightly invalid.

But regardless, using this method is not flexible. Making a passive with a cooldown is not the same as automatically procing every x seconds.

Example: "One attack every x seconds got 100% crit chance"

In which case this method does not work.


Click to expand...

This tutorial just deals with getting a passive ability to show a cooldown (visually). So it should work with any example (including that one)--just trigger the passive and then run the cooldown so it can only proc every X seconds.

You can do it with active abilities as well (as you described)--and that is how a lot of maps have done it--but this method does it with an actual passive so it doesn't have the downside of being clickable, plus it can be triggered on demand without interrupting the unit's order.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

what if I want 2 or 3 or 5 passives with cooldown, will this stack, or will it go bonkers?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

edo494 said:

 what if I want 2 or 3 or 5 passives with cooldown, will this stack, or will it go bonkers?


Click to expand...


There is a section in the tutorial about "Having multiple passives on the same unit".

#### Zordrak

#### Zordrak

 Level 8

JoinedDec 19, 2014Messages300

Resources: 1

#### Resources
Maps 1

Good ideas, i will use this for a new Artifact in my map.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Leandrotp, as Leader of the Secret Council of Object Editor Delving, Sounding, and Otherwise Discovering (SCoOEDSOD for short), I must warn you: you stand at a precipice both great and terrible.

: D

Seriously, though, creating a whole Tutorial to broadcast such an impressive find? Great honor or great shame await you, for flouting our primary tenants; "Hold That Which Thou Findest Sacred, Even Unto Death or Hard-Drive Crash"

I am impressed; primarily because I did not know this. I have one big question, but first two smaller ones:

leandrotp said:

 the only passive ability with cooldown in the entire game!


Click to expand...

Come now. Perhaps you could say "the only passive ability w/ cooldown I know of in the entire game", but such a statement puts you right in the crosshairs of the Council of Mathematic Literacy and Correctness In Diverse Forums (CoMLaCIDF for short).

Alright I made that last group up.

Alright I made them all up. Don't have to be so rude about it. : P (Except for the first one, which totally exists in my head)

(To be fair, I don't know of a single solitary other one as well, and the testing would be insaaaaaane. But still)


 leandrotp said:



 Alternatively you can disable it directly with triggers:

...

But you would need to do that for all players. Using a requirement is much easier.


Click to expand...

!!!

HOLD ON.

So wait. Does this work... For anything? Specifically Spellbooks?

Because if it does... You just totally made my day. : )

We're always talking of using a Disabled Spellbook to hide spells in a unit's command card... Well, I'm always trying to pare down my coding to the absolute essentials (& using the OE entirely whenever possible), so this might be just the trick. : )

~

The big question: "What can one do with this?"

Though I bring shame upon my family by asking, I must admit that though this sounds interesting & powerful, I'm at a loss for what exactly can be done with it. Normally I discover these little OE quirks & am all like "AW YEAH! NOW I CAN DO (insert thing here)!!", but this one... Ach, stumps me. Some examples, por favor?

Pssst.... Hey, buddy. Wanna join my club? The SCoOEDSOD could use a Vice Prez... xD

#### Emm-A-

#### Emm-A-

 Level 14

JoinedJul 1, 2008Messages1,314

Resources: 1

#### Resources
Spells 1

thank you for this tutorial!

Edit: Maybe add a note, that you can enter an invalid id by shift+left click in oe.


 Last edited: Sep 17, 2016

#### K.V.

#### K.V.

 Level 7

JoinedJun 19, 2017Messages147

Resources: 3

#### Resources
Icons 2Maps 1

Very usefull tutorial and a "trick revial". This interesting and hidden propertise of the ability Exhume Corpse can be used along with triggers to achieve cool ability concepts.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

I learned recently that Reincarnation is another passive ability which has a cooldown when used (though apparently Exhume Corpses is better somehow). Just thought I'd throw that in there.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Exhume Corpse is actually laggy I found and why I stopped using it, triggered cooldown or forced reincarnation should be way better if one knows how.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Really? Dang. Do you have any idea why?

What's "forced Reincarnation"?

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

Dat-C3 said:

 Exhume Corpse is actually laggy I found and why I stopped using it, triggered cooldown or forced reincarnation should be way better if one knows how.


Click to expand...

because it scans for corpses every freaking time it moves, every step it takes, making up to thousands calls every second when moving. And that's just for a single unit.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Why is "Exhume Corpse" scanning for corpses in the environment around the triggering/casting unit? I get how the Meat Wagon is, but Exhume just automatically fills the Corpse Cargo Hold with corpses whenever it procs, right?

(maybe someone who can look at the underlying JASS could help elucidate)

#### Kakerate

#### Kakerate

 Level 14

JoinedOct 18, 2013Messages760

Kyrbi0 said:

 Why is "Exhume Corpse" scanning for corpses in the environment around the triggering/casting unit? I get how the Meat Wagon is, but Exhume just automatically fills the Corpse Cargo Hold with corpses whenever it procs, right?

(maybe someone who can look at the underlying JASS could help elucidate)


Click to expand...

The underlying code is C+ not JASS lol. I didnt notice lag issues using this, but I think it might just crash on newer patches. I'll look into it further.

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

I've been using Exhume Corpses with the Start Cooldown function on the latest patch:

Unit - For Unit (Triggering unit), start cooldown of ability Exhume Corpses " over "5.00 seconds.
I've never noticed any performance issues but I could've easily been blind to it. I was only using it on a handful of heroes.

I wonder if changing the abilities fields could prevent the issue. Like if you set the maximum number of corpses to 0 or adjusted the Duration.

Also, Reincarnation seems like an out of the question alternative. I guess you could use a Damage Engine to detect lethal damage and remove the Reincarnation ability in response, but it's not a very pretty solution and it will come with some consequences.

Anyway, maybe there's another ability out there that can have it's cooldown started. $5 to the man that finds it


 Last edited: Mar 18, 2021

#### Kakerate

#### Kakerate

 Level 14

JoinedOct 18, 2013Messages760

Well, the orb effects display Cooldown, right? Thats been heavily documented though lol.

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

The Orb effect abilities have their own problems as well since they requires 2 abilities and mess with orb stacking.

I guess at the end of the day if Exhume Corpses is really problematic you could make the ability based on Channel and give it a 0 second cooldown. This would be clickable but nothing would happen if you did so. It'd only ever go on cooldown when you triggered it to.


 Last edited: Mar 18, 2021

#### Kakerate

#### Kakerate

 Level 14

JoinedOct 18, 2013Messages760

A funny dummy passive would be Spell Shield Amulet with a dummy caster proccing it immediately, though I'm not 100% that it displays Cooldown.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Kakerate said:

 A funny dummy passive would be Spell Shield Amulet with a dummy caster proccing it immediately, though I'm not 100% that it displays Cooldown.


Click to expand...

(5) Passive Cooldown System [GUI Friendly] v1.04 | HIVE (hiveworkshop.com)

#### Wind_w

#### Wind_w

 Level 8

JoinedJun 5, 2021Messages25

Resources: 5

#### Resources
Models 5

you did me a great favor,My friend.

#### ck5524209

#### ck5524209

 Level 3

JoinedNov 30, 2012Messages30

I tested your map and a new map but 'manacost' didn't show in ability tooltip,although it costs mana.

#### ck5524209

#### ck5524209

 Level 3

JoinedNov 30, 2012Messages30

I set manacost to 9999 then set ability level 2 manacost=0

setunitabilitylevel 2 makes the ability coolcown

when spelleffect,set real=unitmana then setunitabilitylevel 1 then after 0. second set unitmana=real

thisway can make one ability coolcown without unavailable other ability

#### Damage

#### Damage

 Level 15

JoinedAug 31, 2009Messages779

Resources: 9

#### Resources
Maps 7Spells and Systems 2

Can you provide an example of the Item implementation?

I followed the tutorial's example of adding an Item (Actively Used = False) with a Spellbook containing an ability based on Exhume Corpses, but it won't cast / display a cooldown.

Does the spellbook (or the hero carrying the item) not also need to contain a disabled Cargo Hold ability to function?

EDIT: Found the issue. As I suspected, the unit (or the spellbook) must also contain a Disabled Cargo Hold ability. Putting it in the spellbook itself is a little neater, but harder to disable at the right time.


 Last edited: Feb 3, 2022

#### leandrotp

#### leandrotp

 Level 9

JoinedJul 30, 2012Messages156

Resources: 1

#### Resources
Tutorials 1

Uncle said:

 I've been using Exhume Corpses with the Start Cooldown function on the latest patch:

Unit - For Unit (Triggering unit), start cooldown of ability Exhume Corpses " over "5.00 seconds.
I've never noticed any performance issues but I could've easily been blind to it. I was only using it on a handful of heroes.

I wonder if changing the abilities fields could prevent the issue. Like if you set the maximum number of corpses to 0 or adjusted the Duration.


Click to expand...

The Duration - Normal field controls how often the game will check for castability, if you set it to a really high value, the internal loop that checks castability will never run, eliminating any possible performance issue.

Of course the ability will also never be casted, but now that we have a function to put an ability into cooldown, this is no longer an issue.

#### Bender

#### Bender

 Level 6

JoinedOct 11, 2017Messages66

I had a small discovery after some time tinkering with this amazing tutorial that I believe should be useful.

having Exhume Corpses to cast inside a spell book will forcibly close any other spell books, and the hero skill leveling up menu as well, so I's suggest caution when using a short cooldown, lest it might bother players.

 You must log in or register to reply here.


### Similar threads

Locked

[Spell]
 Detecting when unit is order to drop one corpse


CollectorofStoriesUntold

Aug 3, 2023

World Editor Help Zone

Replies2Views1K

Aug 4, 2023
CollectorofStoriesUntold

 Passive ability with a cooldown (attack-based)


Arisato

Feb 9, 2012

Misc/General Mapping Tutorials

Replies42Views42K

Mar 25, 2025
Alyssa-Quinxzel

Locked

[Solved]
 How to Make This Ability MUI?


Hermit

Jul 17, 2020

World Editor Help Zone

Replies2Views2K

Jul 18, 2020
Hermit

Locked

 Why is this "simple" trigger not working?


Proxym8

Oct 8, 2023

Triggers & Scripts

Replies9Views2K

Oct 8, 2023
Proxym8

Locked

 Meat wagon cargo


Veritas 117

Jan 27, 2021

World Editor Help Zone

Replies10Views3K

Feb 7, 2021
Map Designer

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