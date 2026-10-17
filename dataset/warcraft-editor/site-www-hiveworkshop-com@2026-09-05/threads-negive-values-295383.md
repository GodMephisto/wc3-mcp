---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/negive-values.295383/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Negive Values
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/negive-values.295383/>
#### fireblasts

#### fireblasts

 Level 6

JoinedAug 31, 2014Messages137

Resources: 2

#### Resources
Spells and Systems 1Tutorials 1

Lets learn about Negive Values shall we? now some may work some might not work but theres a lot you can do to make a more unqiue gameplay. Some may help so you don't have to use gui,jass,vjass, just using your creativdy will get you some place better

What could go wrong? As bizzard warns some negive values can cause crashes and weird bugs.

What it can do for you? As stated in the first sentence it could make for less trigger work.

Heres some abilitys that you can use negive values and can change the way you see them and how they work

Rejuvenation. You can give this a negive value in healing field and mana field to make a dot instead of a heal like for a warlock spell corruption and much more so you don't have to use soul fire or something that silances a target. Yes you could just use triggers but this may be a more effect way to change things up and if you want you can also trigger addional damage based on what you want or feel.

Phoxie Fire. This can only be used on intial damage damage over time will not work. Can be used to make a passive heal. For a healing totem a hero of some sort change then effects and buffs and give it more of healing look or even a imp for a burning heal that heals once then deals damage such as the one in wow must set targetable to friend/ally(then whatever you need else)

Inner Fire. Can use this to make a negive degrading Heath regen armor even damage.

Endurance aura, can be used to lessen the speed around units and attack speed.

Unholy aura. Can make a percanty or point value loss of heath reg.

soul burn. Can be used to grant attack damage, but also be used to deal damage to the unit kinda like unholy frezy but with out attack speed increase.

more to come

This is still in the testing phase and more will be added to the list give a comment below and test around to see what we can do to better the gameplay with fewer triggers.


 Last edited: Jul 16, 2017

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

First of all, the word is "Negative".

It's a pretty good idea; on one hand, it's kinda the first thing people messing around with the WE & OE learn to do. One the other hand, that doesn't mean there shouldn't be a tutorial for it. Also, there are definitely some little protips that could be said.

Something that would really make this useful would be to exhaustively work your way through the Object Editor & catalog/test/notate the results of putting negative values on every ability. However, not only is this a lot of work... @therealattacka already did it, on this. Perhaps you can hook into that.

Good luck!

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

fireblasts said:

 Negive Values


Click to expand...

What is a "negive" value? I hope you are not being racist...

fireblasts said:

 Rejuvenation. You can give this a negive value in healing field and mana field to make a dot instead of a heal like for a warlock spell corruption and much more so you don't have to use soul fire or something that silances a target. Yes you could just use triggers but this may be a more effect way to change things up and if you want you can also trigger addional damage based on what you want or feel.


Click to expand...

Have you tested that it still fires on damage events? Healing usually does not, and so negative healing kind of "unheals" a unit instead of dealing damage. This is important for kill credits as if the fatal damage is caused by negative healing then it is known to have died by suicide rather than correctly crediting the negative healing source as the killer. This is why you must use the damage unit actions instead of set unit life actions to deal damage to a unit.

fireblasts said:

 Phoxie Fire. This can only be used on intial damage damage over time will not work. Can be used to make a passive heal. For a healing totem a hero of some sort change then effects and buffs and give it more of healing look or even a imp for a burning heal that heals once then deals damage such as the one in wow must set targetable to friend/ally(then whatever you need else)


Click to expand...

This might have the opposite problem unless you tested it. It might deal negative damage rather than healing so be subjected to armor type and ability reductions.

fireblasts said:

 Inner Fire. Can use this to make a negive degrading Heath regen armor even damage.


Click to expand...

Negative health regeneration is problematic from a kill credit point of view as the units might appear to suicide rather than be killed during combat.

fireblasts said:

 Unholy aura. Can make a percanty or point value loss of heath reg.


Click to expand...

Same problem as above. It does not do damage so will not correctly credit unit deaths that it causes.

#### fireblasts

#### fireblasts

 Level 6

JoinedAug 31, 2014Messages137

Resources: 2

#### Resources
Spells and Systems 1Tutorials 1

Kyrbi0 said:

 First of all, the word is "Negative".

It's a pretty good idea; on one hand, it's kinda the first thing people messing around with the WE & OE learn to do. One the other hand, that doesn't mean there shouldn't be a tutorial for it. Also, there are definitely some little protips that could be said.

Something that would really make this useful would be to exhaustively work your way through the Object Editor & catalog/test/notate the results of putting negative values on every ability. However, not only is this a lot of work... @therealattacka already did it, on this. Perhaps you can hook into that.

Good luck!


Click to expand...

ty for that I will try to get in touch with this user maybe work together with it.

Dr Super Good said:

 What is a "negive" value? I hope you are not being racist...

#Fireblast# no I'm not sure how you got that from negive values?

I will be more carful of how I word things from here out

Have you tested that it still fires on damage events? Healing usually does not, and so negative healing kind of "unheals" a unit instead of dealing damage. This is important for kill credits as if the fatal damage is caused by negative healing then it is known to have died by suicide rather than correctly crediting the negative healing source as the killer. This is why you must use the damage unit actions instead of set unit life actions to deal damage to a unit.

#fireblasts# requires further testing.

This might have the opposite problem unless you tested it. It might deal negative damage rather than healing so be subjected to armor type and ability reductions.

Negative health regeneration is problematic from a kill credit point of view as the units might appear to suicide rather than be killed during combat.

#fireblasts# Can be triggered to set a unit target? so when it dies could still give a kill.

Same problem as above. It does not do damage so will not correctly credit unit deaths that it causes.


Click to expand...

I will hopefully get in touch with the person who started it and perhaps tie in with him using his own data

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

No fixes happaned. Sending this to GY for now. Grammer and formatting is not good, too. Were you in contact with the author of this meanwhile? Warcraft 3 WE Ability Insight

 You must log in or register to reply here.


### Similar threads

Locked

 Playing with Negative Values Help!!!


Kratoz-X

Nov 26, 2011

Triggers & Scripts

Replies5Views1K

Nov 26, 2011
Talavaj

 Using Custom Values in Triggers


Herio-san

May 6, 2007

Tutorial Archive

Replies16Views3K

Sep 1, 2007
Wolverabid

 A little of Deckboy's spell knowledge


Deckboy

Aug 1, 2006

Tutorial Archive

Replies1Views2K

Aug 12, 2006
Shados

[Mapping]
 Negative WorldEdit Values in Object Editor


Megamax

Jan 30, 2014

Tutorial Archive

Replies10Views4K

Mar 7, 2014
PurgeandFire

Locked

 Healing Spray negative values


PrisonLove

Dec 31, 2008

World Editor Help Zone

Replies10Views2K

Jan 1, 2009
f0rsAk3n

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