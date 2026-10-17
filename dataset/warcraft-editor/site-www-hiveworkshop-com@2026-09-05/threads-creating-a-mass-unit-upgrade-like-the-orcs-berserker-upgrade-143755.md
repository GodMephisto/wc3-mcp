---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-a-mass-unit-upgrade-like-the-orcs-berserker-upgrade.143755/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating a Mass Unit Upgrade Like the Orcs Berserker Upgrade
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-a-mass-unit-upgrade-like-the-orcs-berserker-upgrade.143755/>
#### Mephestrial

#### Mephestrial

 Level 26

JoinedJun 22, 2007Messages254

Resources: 66

#### Resources
Models 44Icons 16Packs 1Skins 1Maps 2Spells and Systems 1Tutorials 1

For this tutorial I am going to refer to the unit that is going to be upgraded as the “Tier2 Unit” and the unit that it is upgraded from as the “Tier1 Unit”.

##
 Step 1: Creating the Upgrade

Locate the Orc’s “Berserker Upgrade” in the Object Editor’s upgrade tab.

Create a new upgrade based on the Berserker upgrade.

Replace the “Troll Headhunter” with your Tier1 Unit.

Replace the “Troll Berserker” with your Tier2 Unit.

##
 Step2: Creating the Upgrade ability.

Locate the Orc’s “Berserker Upgrade” ability in the Object Editor’s abilities tab and create a new ability based on it.

Change the “Troll Berserker” to your Tier2 Unit.

Change the requirements to the upgrade that you made in step 1.

-Locate your Tier1 Unit and add your newly created “Upgrade” ability to its list of abilities.

##
 Step 3: Making the Tier2 Unit trainable.

Add your Tier2 Unit to the same structure that trains your Tier1 Unit.

Add your upgrade that you made in step 1 to your structure.

##
 Step 4: Making the Tier2 Unit unavailable until researched.

Click on Scenario.

Click on Techtree.

Check "Use Custom Techtree"

Select all Players that will have access to the Tier2 Unit.

Navigate to your Tier2 Unit via the drop-down menus.

(Note: If your structure is not yet buildable by any worker it will not show in the listings)

Uncheck "available" for your Tier2 Unit.

You should now have a working upgrade that changes all units of one type into another type once researched.

##
 Step 5: Creating a second level for the Mass Unit Upgrade

Locate the Orc’s “Berserker Upgrade” in the Object Editor’s upgrade tab.

Create another new upgrade based on the Berserker upgrade.

Replace the “Troll Headhunter” with your Tier2 Unit.

Replace the “Troll Berserker” with your Tier3 Unit.

##
 Step6: Creating the Upgrade ability again.

Locate the Orc’s “Berserker Upgrade” ability in the Object Editor’s abilities tab and create a new ability based on it.

Change the “Troll Berserker” to your Tier3 Unit.

Change the requirements to the upgrade that you made in step 5.

Locate your Tier2 Unit and add your newly created “Upgrade” ability to its list of abilities.

##
 Step 7: Making the Tier3 Unit trainable.

Add your Tier3 Unit to the same structure that trains your Tier1 and Tier2 Unit.

Add your upgrade that you made in step 5 to your structure.

##
 Step 8: Making the Tier3 Unit unavailable until researched.

Click on Scenario.

Click on Techtree.

Check "Use Custom Techtree"

Select all Players that will have access to the Tier3 Unit.

Navigate to your Tier3 Unit via the drop-down menus.

(Note: If your structure is not yet buildable by any worker it will not show in the listings)

Uncheck "available" for your Tier3 Unit.

##
 Step 9: Making the upgrade made in step 5 unavailable until the upgrade made in step 1 is researched.

Click on Scenario.

Click on Upgrades.

Check "Use Custom Upgrades"

Select all Players that will have access to the upgrade made in step 5.

Navigate to your upgrade made in step 5 via the drop-down menus.

(Note: If your structure is not yet buildable by any worker it will not show in the listings)

Set to "unavailable" for your upgrade made in step 5.

 Create this trigger.

Swordsman Upgrade Finished
Events
Unit - A unit Finishes research

Conditions
(Researched tech-type) Equal to Tier2 Unit Upgrade

Actions
Player - Set the max research level of Tier3 Unit Upgrade to 1 for (Owner of (Triggering unit))

Note: If doing this optional part, making the hot keys for both levels of the upgrades and units the same helps for smooth game play.


 Last edited: May 27, 2010

#### .Ragnaros

#### .Ragnaros

 Level 6

JoinedAug 13, 2009Messages197

Resources: 5

#### Resources
Icons 3Tools 1Tutorials 1

It is kinda obvious, people only have to open up the object editor and locate Berserker...

Well anyway nice try...

Oh, and add "Hidden" tags to every image!

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

- Use

#### Mephestrial

#### Mephestrial

 Level 26

JoinedJun 22, 2007Messages254

Resources: 66

#### Resources
Models 44Icons 16Packs 1Skins 1Maps 2Spells and Systems 1Tutorials 1

Looks much better, thanks for the advice Pyrite.

.Ragnaros said:

 It is kinda obvious, people only have to open up the object editor and locate Berserker...


Well anyway nice try...


Oh, and add "Hidden" tags to every image!


Click to expand...


Yeah but everyone always forgets step 4 and then they end up using triggers to make the unit unavaible.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

.Ragnaros said:

 It is kinda obvious, people only have to open up the object editor and locate Berserker...


Well anyway nice try...


Oh, and add "Hidden" tags to every image!


Click to expand...

It's hard to say what qualifies as "easy" once you know it--if we said that about everything, we'd have no tutorials. I would say this is useful, especially the quirks of having to manually disable training of the unit, etc.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Approved because it could be useful.

#### mark4072

M

#### mark4072

How to do if i want to upgrade Footman-->Swordman-->Captain because like this way i does it then i can see both upgrades i only want it to show one of the upgrades and then after researched the next upgrade shows up. Please tell me how to.

#### Mephestrial

#### Mephestrial

 Level 26

JoinedJun 22, 2007Messages254

Resources: 66

#### Resources
Models 44Icons 16Packs 1Skins 1Maps 2Spells and Systems 1Tutorials 1

mark4072 said:

 How to do if i want to upgrade Footman-->Swordman-->Captain because like this way i does it then i can see both upgrades i only want it to show one of the upgrades and then after researched the next upgrade shows up. Please tell me how to.

Click to expand...

You will have to create a trigger that runs upon research completion that removes the old upgrade and then adds the new upgrade.

#### mark4072

M

#### mark4072

Mephestrial said:

 You will have to create a trigger that runs upon research completion that removes the old upgrade and then adds the new upgrade.


Click to expand...

could u make an totorial for it?  Please :')


 Last edited by a moderator: May 17, 2010

#### Mephestrial

#### Mephestrial

 Level 26

JoinedJun 22, 2007Messages254

Resources: 66

#### Resources
Models 44Icons 16Packs 1Skins 1Maps 2Spells and Systems 1Tutorials 1

Added a second optional part since I was asked how to do this by atleast 4 people.

#### gianni

#### gianni

 Level 5

JoinedAug 19, 2010Messages35

Resources: 12

#### Resources
Models 2Maps 9Spells and Systems 1

and how do you make Tutorials

#### karia

#### karia

 Level 3

JoinedNov 1, 2018Messages51

Resources: 1

#### Resources
Maps 1

I spent all day looking for a tutorial of this thank you.

#### Jhoannes19

#### Jhoannes19

 Level 2

JoinedMar 3, 2021Messages7

+Rep thank you!

#### Harold Grey

#### Harold Grey

 Level 7

JoinedSep 4, 2016Messages116

Resources: 1

#### Resources
Maps 1

Thanks, I didn't know that custom units show up in the techtree options (since it only says "melee" and "campaign") until finding this tutorial!

#### LordHatchet95

#### LordHatchet95

 Level 9

JoinedOct 2, 2017Messages115

Resources: 5

#### Resources
Maps 2Spells and Systems 2Templates 1

There's a problem... If I use this method on structures, the upgraded structure won't be built at all post upgrade... The building displays at the build menu, and I can give the build order, then the peasant moves to the place and it does absolutely nothing... how to fix that?

EDIT: Fixed! I just had to add the large list of buildings, including pre and post upgrades.


 Last edited: Oct 6, 2021

#### D!ma

#### D!ma

 Level 3

JoinedAug 11, 2018Messages17

Resources: 1

#### Resources
Maps 1

Thanks bro! Been struggling purely bc of that step 4

 You must log in or register to reply here.


### Similar threads

 Basic Upgrades


daxtreme

Jun 2, 2007

Misc/General Mapping Tutorials

Replies7Views16K

Sep 17, 2007
pig_ninja

Locked

[Solved]
 Berserker Upgrade!


Player

Feb 24, 2012

World Editor Help Zone

Replies4Views5K

Feb 28, 2012
Player

Locked

[Solved]
 Unit Upgrade Help


Gadgetguy

Jan 16, 2012

World Editor Help Zone

Replies11Views2K

Jan 17, 2012
Gadgetguy

Locked

 Berserk like upgrade always fail to me


HammerFist

Apr 25, 2013

World Editor Help Zone

Replies3Views1K

Apr 27, 2013
HammerFist

Locked

 Unit upgrade


WarCortez

Sep 3, 2018

World Editor Help Zone

Replies6Views3K

Sep 4, 2018
WarCortez

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