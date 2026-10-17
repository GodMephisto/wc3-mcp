---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-a-tier-system.179342/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating a Tier System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-a-tier-system.179342/>
#### born_by_killers

B

#### born_by_killers

Creating a Tier System (GUI)


 Way One = Difficulty: 2/10


 Way Two = Difficulty: 7/10


-------------------------

Introduction


First of all, a tiering system is something that allows the player to advance to the next character in a story, or 'tier up'. It removes the current character, and creates a new one for the player. This is most useful in RPG's, where the character advances through levels, but it can be used for many different game types.


In this tutorial I will show you how to create a tiering system that can be used in many ways (i.e. ORPG's or RPG's), using only two triggers and four variables.


I will go over two different ways of doing this. Note that the first option does not require the variables, and the second option only requires them if you are making the quest.

The Tutorial


-------------------------

Setup


There are a few things you'll need to set up. We'll begin with the variables.

Variables


First things first, if you are doing Option 1, then you will not need the variables. Only if you are making a Quest for Option 2 will you need the variables.


- Open your Trigger Editor and press CTRL+B to open the variable manager.


- While there press CTRL+N to create a new variable.


- Set Variable Type to Integer. Name it T2Integer_1. Press OK.


- Make another Variable just like this but name it T2Integer_2.


- Create another Variable. Set this one's variable type to String. Name it T2Quest_1. In this one, you'll also check the [ ] Array field for the variable. Then press OK.


- Make another Variable just like this but name it T2Quest_2.

Option 1

The Map


Now you will need a few things:


- Three regions, one over a Circle of Power, like so, which will be used to teleport your unit to the upgrade area once they step into it.


- Another in the upgrade area, (which would be impossible to get to unless the player teleported in) as a target for your unit to teleport to. ( You can make yours larger, smaller, whatever )


- And the final one over another circle of power in the same upgrade area as the second region, for the current player to walk on in order to upgrade.

Triggers


The first trigger is fairly straightforward.


When a unit enters the upgrade place, we check to make sure it is the type of unit we want going in here. Once it passes that check, We teleport it to the upgrade place. We then move the camera to the unit and put the unit in the player's selection.


The second trigger is also fairly straightforward.


When the unit enters a region (FireBat), then we remove that unit from the game, and replace it with a brand new unit. We then teleport the unit out of the area, move the camera to the unit and put the unit in the player's selection just like before.


This is the end of way one.

Option 2

The Map


You will need to create a unit which you will talk to in order to upgrade, and if you wish, choose some sort of quest, or objective in order to upgrade through this unit.

Triggers


Make a trigger titled T2 BeastMaster. This is a slightly difficult trigger IF you wish to include the Quest portion. All that happens here is when a unit comes within range of the Trainer unit, the Actions will start. First it makes sure that you have the triggering unit close to the Trainer, not just some other unit, and then click on the Trainer. It then checks the unit type, and if all conditions are in order, it replaces the unit, transports it and selects the unit for the player. If you choose to make include the quest portion, then under the ELSE action set, make a Quest. If you don't want the quest portion, then just don't include the things with relevance to the quest.


The second trigger is the hardest one, for it makes a quest, and applies it to the Beastmaster upgrade. This will use the four variables we made in the beginning.


Make a trigger titled Quest Kill Unit 1. This trigger will activate when a unit dies, and the unit dying is the type of unit you want to die for the quest to complete. The most important part of these triggers if the quest you are making requires killing more than one unit is the

Skip Remaining Actions
 action. Without this action following the variable set bit, when one target unit dies, it will go through the whole trigger and complete the quest.


For this quest you will also need to add a Set Variable action to your Map Initialization trigger, or the upgrade quest will not start.


If you need any further instruction on this topic, send me a private message and I'll get back to you as soon as I see it.

Test Map = View attachment Tutorial Map.w3x


 Last edited by a moderator: Oct 30, 2010

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

You don't need to use Do Nothing...

It will work fine even without it.

Also attach test map.

#### Ikithe

#### Ikithe

 Level 5

JoinedSep 15, 2010Messages156

Option 1 was a bit simple, but 2 was kinda helpful, thanks for posting this.

#### born_by_killers

B

#### born_by_killers

is there anything else i need to attach or fix?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Add a brief description of what a tier system is in the intro, and then I'll approve it. Otherwise it is a pretty cool, unique tutorial. Good job. =)

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Pretty nice, ~approved.

#### Kharyus

K

#### Kharyus

-Kobas- said:

 You don't need to use Do Nothing...


It will work fine even without it.


Also attach test map.


Click to expand...


I think i read somewhere in the forums that putting (Do Nothing) in the else if it does nothing increases efficiency.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

You think wrong buddy.

Do Nothing call function to do nothing -.-

And if you ask why blizzard added useless function, answer is because of 1 lined if then else functions!

#### Warnem55

#### Warnem55

 Level 3

JoinedFeb 27, 2011Messages39

WHat if the unit that is removed from the game has an item? do you need a special trigger to regive it the items?

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

If you remove unit from the game it will remove items abilities or anything else related to it (attachments, skill points, experience...)

But if you want to save anything mentioned above you need to create variable (for more units use array variable) and save those things before you remove unit, then just edit values of new created unit

 You must log in or register to reply here.


### Similar threads

 Creating Easy Multi-User Quests


The_Big_S

Aug 15, 2007

Trigger (GUI) Editor Tutorials

Replies33Views21K

Nov 17, 2007
PurplePoot

 How to make a Find that Person Quest.


Dark Hunter1357

Apr 11, 2008

Trigger (GUI) Editor Tutorials

Replies4Views11K

Jun 25, 2008
Ghan_04

 How to Make a Simple Quest in WE to Show Info (Noob Tutorial)


Slayer14

Jan 23, 2011

Trigger (GUI) Editor Tutorials

Replies20Views29K

Nov 4, 2015
DaneTheBeast

 Hero Selection Systems


Tonks

Mar 23, 2007

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jun 8, 2023
dantles1992

 Hero Selection Systems


Eleandor

Aug 20, 2007

Trigger (GUI) Editor Tutorials
2

Replies51Views81K

Feb 28, 2021
Caalanuaa

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