---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-custom-levelable-hero-abilities.307956/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Custom Levelable Hero Abilities
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-custom-levelable-hero-abilities.307956/>
#### 2noobig4you

#### 2noobig4you

 Level 6

JoinedOct 4, 2016Messages28

Resources: 5

#### Resources
Maps 4Tutorials 1

Alright Guys i'm trying to show you how to make: Custom levelable Hero Abilities.

Explanation:Since i searched a lot for this, i have never found something really useful about that. Most Tipps were just: "It has something to do with Engineering Upgrade but you gotta figure out on your own." and that's what i did.

So here it is:

Usage

Most common Way is to buy Items that "holds" and Ability in it. You need a little Triggers for the Setup but mostly it's (a lot of) work in the Object Editor.

Step 1

Give all of your customizeable Heroes 5 (Dummy) Abilities. In my Example i use Moonglaive because it does exactly nothing except showing an Icon. Be sure to give the Dummy Ability the exact same Amount of Levels as you want your Levelable Abilities to have.

 Little Tipp here: Change the Button Position.

Step 2

Depending on your Way of picking the Abilities (1 Ability per Slot like in Custom Hero Arena) or in my Way 4 Normal and 1 Ultimate, you gotta modify your Engineering Upgrade Ability.

For each Ability you want to create you need 1 Engineering Upgrade which replaces the Dummy Ability with your 'End' Ability. If you do use the 4:1 Ability Way you also need to remove the Ability after upgrading but lets leave that aside for now.

Be sure to remove all other Datas from the Engineering Upgrade Ability (Buff) and set everything else to 0 (Movementspeed and Damage Bonus). Also Disable it for each Player to not show an Icon. Yes it will work still.

Step 3

Use a little Amount of Triggers to add the wanted Engineering Upgrade to your Hero and set it to the specific Level of the Slot if you use the 4:1 Way.

Questions

What is the 4:1 Way?

It's the way i do it in my Map where you pick 4 Normal Abilities and pick 1 Ultimate separated.

The other Way would be to have 5 separated picking Areas where you just need 1 Level of Engineering upgrade per Ability.

What is Engineering Upgrade?

It's an Ability (of the Hero Tinker) which allows you to "remove or replace" Ability (-Levels) with another Ability (-Level). And while the Hero holds the Engineering Upgrade it replaces the Dummy Ability in the leveling Slot with the Ability you want. (Pretty useful isn't it??)

Notice:

Be sure to not mix up Things and the Order of the Abilities in the Engineering Upgrade do matter!

The first one is being removed while it adds the 2nd Ability in Order.

If you use the 4:1 Way you also need to replace the 'End' Ability with the Dummy Ability again because 'set Level of Ability' somehow skips through each Level until it reaches the Final so u gotta reverse it.

Conclusion:

Sorry for the lack of Images. I created a Testmap therefore. Also my World Editor is in German but i hope you can understand it well.

#### Attachments

Custom levelable Hero Abilities.w3x

 24.8 KB

 · Views: 128



#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

There were reports that base ability must be equal for both dummy and real ability when utilizing Engineering Upgrades

#### 2noobig4you

#### 2noobig4you

 Level 6

JoinedOct 4, 2016Messages28

Resources: 5

#### Resources
Maps 4Tutorials 1

There aren't any base Abilities. There is just a dummy placeholder Ability that you can replace with any Ability you want. The Engineering Upgrade only allows you to replace Abilities inside the 'Hero Learn Ability Section'. Thus those Fields needs to be set correctly.

Edit: To work properly you obviously shouldn't replace it with any other Ability but with another Hero Ability with the same Amount of Levels.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I have a few issues.

Firstly, I think the title is misleading and the introduction does not explain what exactly the tutorial is about. I have to run the test map to understand what the end goal was.

Secondly, if you remove the FaQ the tutorial is very short and I would consider it too simple.

Thirdly, there are quite a few grammar error

Lastly, I think there are many overall improvements to be done in terms of layout/structure but I consider this a minor thing relative to the other points.

Graveyarded for now as I think this needs a fairly big rework. I would be happy to put it back and reconsider if updates are made.

 You must log in or register to reply here.


### Similar threads

Locked

 Ability Level Cap?


Togark

Jun 24, 2018

World Editor Help Zone

Replies0Views813

Jun 24, 2018
Togark

Locked

 [Trigger] Custom Ability Level up Script


jaxfrost

May 27, 2021

Triggers & Scripts

Replies5Views1K

May 30, 2021
Uncle

A

Locked

 Transformation ability and Engine Upgrade


archimonde92

Feb 20, 2011

World Editor Help Zone

Replies7Views2K

Feb 22, 2011
defskull

Locked

[General]
 Can't increase custom hero ability levels


TimeForAChange

May 31, 2020

World Editor Help Zone

Replies5Views1K

Jun 1, 2020
Uncle

Locked

[JASS]
 hiding ability while keeping level


edo494

Jul 16, 2012

Triggers & Scripts

Replies2Views1K

Jul 16, 2012
edo494

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