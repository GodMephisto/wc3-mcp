---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ever-wanted-to-change-life-without-needing-to-glitch-it-through-abilities.254190/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Ever Wanted to Change Life Without Needing to Glitch It Through Abilities
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ever-wanted-to-change-life-without-needing-to-glitch-it-through-abilities.254190/>
#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

This way can only change life by percentages however it is quite cool. I don't think it works for structures though, more precisely it doesn't work for when they upgrade.


First you learn more about player handicap then you switch the target unit to another player.. preferably a neutral slot/13-16 then change their handicap to a percentage then change ownership back to you/player then change handicap back to 100 for that neutral slot. Tuh da you now have bonus or lowered life by a %.


[trigger=Visuals]


Events


Conditions


Actions


 Unit - Change ownership of (Picked unit) to Neutral Extra and Retain color


 Player - Set Neutral Extra handicap to 150.00%


 Unit - Change ownership of (Picked unit) to Player 1 (Red) and Retain color


 Player - Set Neutral Extra handicap to 100.00%


[/trigger]


Want the method to calculate how much health something will give to a certain unit with the certain life?


Use this equation. Current Handicap * Unit's Life


Example: 420 * 1.5 = 630


1.5 would be 150% handicap.


 Last edited by a moderator: Jul 6, 2015

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

interesting

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

You should show the calculation to convert it from % to actual numbers. Just in case.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

edo494 said:

 interesting


Click to expand...

Lot's of secrets still yet to be found.


 Last edited: Jul 2, 2014

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

That's really cool. But I think this belongs in the lab more than the tutorial section. Still, that is an awesome discovery. Hopefully there aren't any downsides to it!

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Sadly there is, it gets buggy with morphing units/upgrading units as well upgrading structures. I am unaware of other flaws though.

I also dis-agree, this should be taught as how to use it instead of just showing it. My opinion though. =)

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Yah it's interesting, but I think it gets a bit buggy when playing around with it and using it more often to same unit. (I increased but also decreases the handicap multiple times)


I made a command for add/set handicap value for easy manipulating and testing it, and it had some strange behaviour. :/


I only used natives and jass code, but I think it should make no difference.


Sorry can't attach test map anymore, it's already removed. But if someone else also prepared something like this maybe he/she could post results here??

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

What was the strange behaviour besides losing selection or stopping current order?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

I think it added/substracted the wrong amount of HP, so it didn't work properly anymore. (hope my test code was valid, can't prove it anymore... and atm I have a lack of time)

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

I didn't notice this, I ran an example of positive and negative for a full 2 minutes each using a 0.01 timer and it added/subtracted fine. Maybe the equation was faulty or I could of missed something.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

I would imagine changing handicap is an O(n) operation with all units owned by that player. For this reason you need to reserve a player only for this purpose and make sure he has the minimum number of units possible.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Yeah agreed DSG. Only issue with this is you don't want to keep messing around with player slot handicaps, rather use neutral extra.

I haven't found a problem otherwise with just 1-12 slots besides killing any unit that attempts to upgrade.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I'm approving since this is a nice method. If anyone finds bugs with this, feel free to let me know!

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

I need to know IcemanBo's Test #2, man.

The suspense is killing.

OT

This could be interesting for future use, thank you.

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 Custom Value Trigger; Able to shorten?


Kenoriga

May 9, 2007

Triggers & Scripts

Replies3Views1K

May 9, 2007
Kenoriga

[Mapping]
 Town Hall Revealing Trigger for Altered Melee (between 1.27a and 1.24e patch)


TUCANA

Dec 28, 2017

Tutorial Archive

Replies7Views5K

Mar 3, 2021
Jhoannes19

Locked

[Trigger]
 Does this leak?


dontsnoozeondalose

Jul 18, 2015

Triggers & Scripts

Replies17Views3K

Jul 20, 2015
venger07

Locked

 Does "Region - Move" Work?


aussj5link

Jan 1, 2015

Triggers & Scripts

Replies12Views2K

Jan 2, 2015
aussj5link

Locked

 I need help with a bit complicated tigger


emil23

Oct 28, 2017

World Editor Help Zone

Replies5Views1K

Oct 30, 2017
Dr Super Good

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