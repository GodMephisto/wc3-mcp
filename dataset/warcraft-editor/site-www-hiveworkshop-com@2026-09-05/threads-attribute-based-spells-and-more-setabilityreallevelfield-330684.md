---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/attribute-based-spells-and-more-setabilityreallevelfield.330684/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Attribute Based Spells and More Setabilityreallevelfield
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/attribute-based-spells-and-more-setabilityreallevelfield.330684/>
#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

 ------SetAbilityRealLevelField Tutorial------

Hi Everyone, this is my "First Tutorial" about SetAbilityRealLevelFields in GUI, and evrything we can Modify on our Spells.

I started this Tutorial because, i cant find anything about it on Hive.. and its very usefull for Beginners.

Content:

1. This Tutorial Shows how to Modify Attribute based "Spell Damage" on Default based Abilitys.

2. Give your Abilitys some "Extra" effects.

Lets start with "Lightning Chain" as Spell for our Tutorial, as example.

Lightning Chain:

We set everything to 0 for now because, we will handle this things with Triggers based on our Heros stats.

1. Attribute based "Spell Damage"

Lets start with some Easy one the Damage of our "Lightning Chain", first we need the Real Field we wanna change.

We open the ObjectEditor>Abilities and click on our Lightning Chain, then we Press "Ctrl + D" and can see the Short Cut for our Real Field

We wanna Modify the Damage of our Spell, so the Field we need for Lightning Chain is Ocl1.

Now we open our Trigger Editor.

Lightning Chain
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Lightning Chain

Actions
Ability - Set Ability: (Unit: (Casting unit)'s Ability with Ability Code: Lightning Chain )'s Real Level Field: Damage per Target ('Ocl1') of Level: ((Level of Lightning Chain for (Casting unit)) - 1) to (Real((Intelligence of (Casting unit) (Include bonuses))))

The "Event" and "Conditon" are self explained, lets talk about our "Action".

Ability - Set Ability Real Level Field, Example

Set Ability = Units: Which Unit? Casting Unit.

Ability Code = Which Ability Code? Lightning Chain (Because thats our Spell).

Real Level Field = ? Ok lets go, we wanted to change the Damage and Find out the Short Cut for the "Real Field" of "Damage" for Lightning Chain is ('Ocl1') thats the Field for Damage per Target.

Level of Ability = ? Now here we Need to Understand that the Index of the Level is needed.. and the Index start at 0, not at 1 that means.

Ability Index 0 = Ability Level 1

Ability Index 1 = Ability Level 2

Ability Index 2 = Ability Level 3

Ability Index 3 = Ability Level 4

So i used Level of Ability of Unit and then -1, no matter which level now my Spell has its evrytime set to the Index next to it.

Modified Real = ? Because we wanna have our Damage based for each Target our Lightning Chain hits, to be based on our Attributes i set it to Intelligence of (Casting Unit)(Include bonuses).

2. Extra Effects with SetAbilityIntegerLevelField

We wanna give our Lightning Chain some extra "Effect" for this Example, lets use

Set Ability Integer Level Field.

Because the Field we Wanna Change now its an Integer Field

Lightning Chain
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Lightning Chain

Actions
Ability - Set Ability: (Unit: (Casting unit)'s Ability with Ability Code: Lightning Chain )'s Real Level Field: Damage per Target ('Ocl1') of Level: ((Level of Lightning Chain for (Casting unit)) - 1) to (Real((Intelligence of (Casting unit) (Include bonuses))))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Random integer number between 1 and 100) Less than or equal to 25

Then - Actions
Ability - Set Ability: (Unit: (Casting unit)'s Ability with Ability Code: Lightning Chain )'s Integer Level Field: Number of Targets Hit ('Ocl2') of Level: ((Level of Lightning Chain for (Casting unit)) - 1) to 4

Else - Actions

Now our Spell has a 25% Chance to Hit one More Target because we now Modified:

Integer Level Field: Number of Targets Hit ('Ocl2')

to 4.

Final Word:

We can Change and Modifie with this Fields all on Default Based Spells..

set them to Attributes, or to 1% of our Max Hp...

we can even Modify Passivs... with an Event like Every 5 Seconds to Refresh the Modified Amount of the Passiv.

We can Create Items for Specific Heros with Real[Arrays] like:

Shaman[PlayerNumber(OwnerofUnit(HeromanipulatingItem)]

or even Create Talentrees

Example 1: we Buy/Press/What Ever your Talentree is Based on, and the set a Real[Array] and if the Real[Array] is equal to 1.00 the Paladin has a 25% that his Next Holy Light cost no Mana

Example 2: or Set the Cooldown to 0 / Set the Cast Time to 0 or what ever you want you can change Multiple Reals like i did for the Spell Avatar and Changed the Armor/Hp/Damage based on Strenght.

For Dummy Units you can use it Aswell... Thats it it Might be a Short Tutorial just Try it out it has Potentiual if you use it Good

Cheers Pwnica

#### Attachments

Screenshot (34).png

 124.8 KB

 · Views: 478



#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Given the potential of this native, I am surprised we have no tutorial of it yet.

Anyway, how well it works with tooltips?

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

I did it Check it out it will change the tooltip evrytime the hero level up or aqquires a item...

i never used it before so this is the first time

you can also set it before in a variable makes it less text and so you can change the color of the amount he can heal or the amount of damage in the tooltip

these real/integer/string fields are realy nice imagine how much perfection you can set into a map

#### Attachments

Tooltip Test.w3m

 18.5 KB

 · Views: 148



#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

I changed it a bit so it will just change the number of the healing amount when he learn the spell so it makes more sense...

and looks much better with some Color

#### Attachments

Tooltip Test2.w3m

 18.7 KB

 · Views: 148



#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


The first two images are attached as thumbnails, so hiding them is unnecessary as the images won't hog space.

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

GhostHunter123 said:

 The first two images are attached as thumbnails, so hiding them is unnecessary as the images won't hog space.


Click to expand...

Ye maybe I did a mistake

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

A lot of text for something relatively simple?

Also, what's the point of setting the damage to 0 in the OE?

If you are gonna change it on-cast anyway the initial value is irrelevant.

Still, simple does not mean bad. This might be an eye-opener for someone getting started.

That being said I think this needs to be simplified. It should not take a full paper to show how to use a single GUI action and edit a couple of fields in the object editor.

And while you are at it, some structure upgrades would be a plus. A couple of more titles and colors for a start.

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

Chaosy said:

 A lot of text for something relatively simple?

Also, what's the point of setting the damage to 0 in the OE?

If you are gonna change it on-cast anyway the initial value is irrelevant.

Still, simple does not mean bad. This might be an eye-opener for someone getting started.

That being said I think this needs to be simplified. It should not take a full paper to show how to use a single GUI action and edit a couple of fields in the object editor.

And while you are at it, some structure upgrades would be a plus. A couple of more titles and colors for a start.


Click to expand...

Hey Chaosy, this is a Simple Tutorial for Beginners... behause when i Start myself Mapping when Reforged came out i was missing a lot of informations and Knowhow...

And i wanted to gibt People a easier way in

And also it was my First ever Thread so don’t judge me for my Noob Forum Skills

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

What I mean by simple is not that a tutorial should be complex, no, that sounds counterproductive.

But what I mean is that what you are trying to teach is on the easier side of the spectrum.

To make that feasible I think this is too verbose.

#### Masky

#### Masky

 Level 5

JoinedMar 16, 2013Messages108

thanks man +rep <3

#### gaginw100

#### gaginw100

 Level 2

JoinedFeb 12, 2024Messages16

Chaosy said:

 What I mean by simple is not that a tutorial should be complex, no, that sounds counterproductive.

But what I mean is that what you are trying to teach is on the easier side of the spectrum.

To make that feasible I think this is too verbose.


Click to expand...

I want to know more about the advanced because I am new to this new native and don't know what it can fully do.

Do you have any link about this for me to learn?

For The original Poster: Thanks so much. This is what I want to do since the 1.26 Version of World Editor.

 You must log in or register to reply here.


### Similar threads

 Spells - Making a Chain spell (GUI Version)


Daelin

Oct 30, 2006

Tutorial Archive

Replies1Views3K

Mar 11, 2007
Wolverabid

Locked

 Why doesn't my trigger work.


AznShadowFury

Sep 6, 2007

World Editor Help Zone

Replies2Views1K

Sep 9, 2007
Anachron

K

Locked

 increase Spell Damage With Item


Karam

Dec 13, 2021

World Editor Help Zone

Replies3Views2K

Dec 13, 2021
Uncle

Locked

 Scaling Ability Damage on Chain Lightning?


Lysho

Jul 2, 2015

Triggers & Scripts

Replies7Views3K

Jul 7, 2015
SAUS

Locked

[Spell]
 Cooldown disabler buff


Chronauer

Aug 27, 2019

World Editor Help Zone

Replies2Views1K

Aug 31, 2019
Uncle

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