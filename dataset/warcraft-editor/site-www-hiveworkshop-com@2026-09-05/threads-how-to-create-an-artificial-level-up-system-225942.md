---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-create-an-artificial-level-up-system.225942/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Create an Artificial Level Up System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-create-an-artificial-level-up-system.225942/>
#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

How to create a level-up system

Why create a level-up system?

A level-up system would let non-heroes level up. It is very useful to make units stronger after every kill. It allows you to let the player create "veterans", who are units who deal higher damage due to the unit's experience in combat. Popular maps who use this system include YouTD (towers) and Werewolf: Transylvania (non-hero veteran bonus).

So how do I make it?

First, you need to create a hashtable.

Hashtable Creation
Events
Map initialization

Conditions

Actions
Hashtable - Create a hashtable

Set Level = (Last created hashtable)

Then, you need to create a trigger which increases the level of the unit every time it kills something.

Level Up System
Events
Unit - A unit Dies

Conditions

Actions
Hashtable - Save ((Load (Key (Killing unit)) of 1 from Level) + 1) as (Key (Killing unit)) of 1 in Level

Replace 1 with the name of a spell if you are leveling up a spell.

If you feel like it, you can turn the system into a "kill X for rank" system, by adding these few lines to the end of the trigger.

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Load (Key (Killing unit)) of 1 from Level) Equal to 50

Then - Actions
Hashtable - Save 5 as (Key (Killing unit)) of 1 in Rank

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Load (Key (Killing unit)) of 1 from Level) Equal to 29

Then - Actions
Hashtable - Save 4 as (Key (Killing unit)) of 1 in Rank

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Load (Key (Killing unit)) of 1 from Level) Equal to 28

Then - Actions
Hashtable - Save 3 as (Key (Killing unit)) of 1 in Rank

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Load (Key (Killing unit)) of 1 from Level) Equal to 3

Then - Actions
Hashtable - Save 2 as (Key (Killing unit)) of 1 in Rank

Else - Actions
-------- etc... --------

NOTE: The if/then/elses for the higher ranks be executed first, or it will bug!

Also, add these lines to the end of Hashtable Creation.

Hashtable - Create a hashtable

Set Rank = (Last created hashtable)
The final Level Up System trigger will look like this.

Level Up System
Events
Unit - A unit Dies

Conditions

Actions
Hashtable - Save ((Load (Key (Killing unit)) of 1 from Level) + 1) as (Key (Killing unit)) of 1 in Level

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Load (Key (Killing unit)) of 1 from Level) Equal to 50

Then - Actions
Hashtable - Save 5 as (Key (Killing unit)) of 1 in Rank

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Load (Key (Killing unit)) of 1 from Level) Equal to 29

Then - Actions
Hashtable - Save 4 as (Key (Killing unit)) of 1 in Rank

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Load (Key (Killing unit)) of 1 from Level) Equal to 28

Then - Actions
Hashtable - Save 3 as (Key (Killing unit)) of 1 in Rank

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Load (Key (Killing unit)) of 1 from Level) Equal to 3

Then - Actions
Hashtable - Save 2 as (Key (Killing unit)) of 1 in Rank

Else - Actions
-------- etc... --------

Now, how do I actually use this system?

You can use it for a variety of things, like increasing the damage of a unit for every level or rank he has, or increasing the spell potency, or simply adding more abilities. Just use this segment of a line to load levels,

((Load (key([INSERT UNIT HERE])))) of 1 from Level)
and this to load ranks.

((Load (key([INSERT UNIT HERE])))) of 1 from Rank)

#### Attachments

Levelupsystemtutorial.w3x

 21.7 KB

 · Views: 369




 Last edited: Mar 13, 2013

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

Hope it hadn't been done before! If it had been done before, I will feel so stupid for going all the way to the Hashtables page in hope that hashtables can do this properly instead of just going to the tutorial page.

Also, this post is not part of the tutorial.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

since you only use one childkey (which is 1), this can be done using an array instead of hashtable... they work much faster and are also easier to deal with...

also those If-then-elses are better off when nested [put the next if-then on the else of the higher one]... because right now, if a unit is greater than rank 50, it will also trigger the actions for >12,>10,>5 so in the end, the rank 50 unit will have a value of 2 instead of 5...

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

#1: Can arrays use handles as keys? That is why I used a hashtable.

#2: Fixed!

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Not sure, but you can always use a unit indexer to obtain a key specific to each unit

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

Arrays cap at 8192. The cap of units are 1080x12=12960, which is bigger than 8192.

I don't believe that the number of keys in a hashtable caps.

Furthermore, I want to go to extremely great lengths to make the system both GUI and MUI, which means no 8192-capping arrays.


 Last edited: Nov 23, 2012

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

it's just that you can only have a limited hashtable amount (256), a pretty big number...

8192 is a really huge amount already, remember that this is a level up system which uses 1 instance per unit. I'm sure your map will most probably lag so bad or get a fatal error before you reach 8192 units to use this system on the map... of course you should also be using recycling if you do that to free up spaces occupied by dead units...

anyway, lots of systems/spells here cap at 8192 but they are still considered as MUI...

you can use game caches too if you really want to avoid caps I think...

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

Fixed!

#### sonofjay

#### sonofjay

 Level 28

JoinedOct 28, 2011Messages4,750

Resources: 27

#### Resources
Packs 1Maps 20Templates 5Tutorials 1

Hey bro, can you provide as some test map? I really want to test this since it looks a very nice idea.

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

Map uploaded. Also, I added a new section to the tutorial on how to use the system, just to make the tutorial feel complete.


 Last edited: Nov 24, 2012

#### Arhowk

#### Arhowk

 Level 19

JoinedAug 8, 2007Messages2,763

why game caches? if i remember correctly they wont function properly on multiplayer. either use hashtables or arrays.

arrays wont cap out because the graphical limit for units is closer to 6 grand even if the solid is near 12 grand

the max for physical hashtables is 256, not hashtable keys. keys are infinite

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Game Caches work in MP, it's just that they aren't saved AFTER the game ends...

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

Since it can be done on a hashtable without key caps, I have decided that this tutorial will use hashtables once more!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

The tutorial is very brief but covers everything needed. You offered an adequate description of the triggers and attached a good test map. Plus, it is a pretty unique idea (at least in my opinion).

Approved.

#### frettory

#### frettory

 Level 4

JoinedJan 19, 2012Messages52

Resources: 2

#### Resources
Tutorials 2

I am going to update the tutorial so that whizzy animations fit in more nicely.

 You must log in or register to reply here.


### Similar threads

 General Spell Creation


Chaosy

Jun 11, 2013

Trigger (GUI) Editor Tutorials

Replies35Views10K

Feb 8, 2014
PurgeandFire

 A Complete Beginners Guide to Hashtables


Jazztastic

Jun 15, 2011

Trigger (GUI) Editor Tutorials
2

Replies57Views45K

Nov 12, 2024
PurgeandFire

Locked

 Problem with Item Ability Spell


idodik

Sep 17, 2009

Triggers & Scripts

Replies6Views2K

Sep 20, 2009
idodik

 Charged Ability Tutorial


Almia

Aug 5, 2012

Trigger (GUI) Editor Tutorials

Replies12Views13K

Mar 4, 2016
Almia

Locked

 Custom stat system!


AlwaysAndForever

Aug 8, 2024

Triggers & Scripts

Replies2Views679

Aug 10, 2024
AlwaysAndForever

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