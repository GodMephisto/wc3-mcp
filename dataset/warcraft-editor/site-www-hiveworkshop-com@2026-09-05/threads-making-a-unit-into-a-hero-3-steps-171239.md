---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-a-unit-into-a-hero-3-steps.171239/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making a Unit Into a Hero 3 Steps
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-a-unit-into-a-hero-3-steps.171239/>
#### Leubast

#### Leubast

 Level 6

JoinedAug 2, 2009Messages130

Resources: 2

#### Resources
Models 1Tutorials 1

#
 Making a Unit into a Hero

Converting a non-hero unit to a hero


 Rui said:



 It is not possible to actually convert a unit to a hero directly. But you can always copy all the important stats, models, abilities and paste them on to the other character.


Click to expand...

This tutorial will show you how to make a unit a hero. (This way is much easier)

Required:Jass Newgen Pack

Optional:Hero Glow Model (For adding hero glow to the unit.)

###
 Step 1: Select the Hero-to-be

In the object editor under the unit tap, highlight the unit you want to be a hero. Ctrl + C (Copy) then Ctrl + V (Paste) I will use the Peasant as an example.

###
 Step 2: The Unit ID

After pasting, this will pop up:

Just change the lower-case 'h' into a upper-case 'H' (you may change the other three [3] characters if you wish) and click 'Ok'

###
 Step 3: Setup the New Hero

Under Custom Units, he is now a hero.

There are a few things you need to setup:

You may keep these values, but...

NotRecommended!

Also doesn't add hero glow, here is a tutorial to add the hero glow:How Add Hero Glow Without Modeling

I hope everyone at the Hive will find this tutorial helpful for making heroes faster.


 Last edited: Jun 29, 2010

#### Etzer

#### Etzer

 Level 13

JoinedFeb 18, 2009Messages1,378

Remember to change the attack style and armor.

By the way, it does NOT get hero glow.

#### Leubast

#### Leubast

 Level 6

JoinedAug 2, 2009Messages130

Resources: 2

#### Resources
Models 1Tutorials 1

I will add them to the tutorial, thanks Etzer.

Added what you said, thanks again.


 Last edited: Jun 27, 2010

#### Etzer

#### Etzer

 Level 13

JoinedFeb 18, 2009Messages1,378

You can download the hero glow here on hive. How to attach it I'm not aware.

#### Leubast

#### Leubast

 Level 6

JoinedAug 2, 2009Messages130

Resources: 2

#### Resources
Models 1Tutorials 1

I believe you make a buff with no icon, change the target art to the Hero Glow model, make sure the target attachment is origin and give it to the unit.


 Pyritie said:



 I should also mention that you should use an ability like the healing fountain's AoE heal as a dummy ability. That one changes the attachment's team color when the unit has that ability.


Click to expand...


 Last edited: Jun 29, 2010

#### Linaze

#### Linaze

 Level 24

JoinedFeb 28, 2007Messages3,478

Resources: 4

#### Resources
Models 1Tutorials 3

Everyone I know of makes heroes by basing their custom unit of a existing hero and changing the values of each field, and with that said, I really don't find this useful, seeing how mappers are already used to making heroes another way than this tutorial teaches. I'm also wondering how much time you actually save by doing it this way, it's probably something like a minute, at most.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Nothing special really!

Hero can be created very easy just like any other thing in Object Editor with simple:

Ctrl + C and then Ctrl + V

I don't see any point in this tutorial!

Until you show me some new and cool stuff my vote will be:

REJECTION

#### Leubast

#### Leubast

 Level 6

JoinedAug 2, 2009Messages130

Resources: 2

#### Resources
Models 1Tutorials 1

If you're making a hero arena, this may save you quite a bit of time.

@-Kobas-: This is Ctrl + C, Ctrl + V. You're just copying a unit and making him a hero without copying another hero and changing everything on him to make him like another unit.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Sorry, I think this tutorial is a tad short/simple. D; It is nice to know that method sometimes, but it is mostly a tip. (if only there was a "tips" section) I'm afraid I'll probably graveyard this soon. =(


The tutorial is very nice itself, just the topic is a bit simple and hard to expand on.

#### Septimus

#### Septimus

 Level 31

JoinedMay 3, 2008Messages3,146

Resources: 50

#### Resources
Packs 1Maps 35Spells and Systems 8Tutorials 6

Everlive said:

 If you're making a hero arena, this may save you quite a bit of time.


@-Kobas-: This is Ctrl + C, Ctrl + V. You're just copying a unit and making him a hero without copying another hero and changing everything on him to make him like another unit.


Click to expand...


this doesn't make any sense.


cause if you copy and paste a unit as a hero just like your tutorial shown, you still got to setup the attributes of the hero just like what people normally do to normal heroes.


the only thing differences between your tutorial and this method is that one need to change the icon and model of the hero to make it look as different unit while the other change the unit id from "h" to "H"


if people are planning to use custom model/icon, then this tutorial have no purpose at all.


in other word, it only work for non-hero unit who you are planning to make it into hero unit without using custom model/icon that barely saves 3 seconds of times in map editing.

#### Leubast

#### Leubast

 Level 6

JoinedAug 2, 2009Messages130

Resources: 2

#### Resources
Models 1Tutorials 1

PurgeandFire111 said:

 I'm afraid I'll probably graveyard this soon. =(


Click to expand...

You can graveyard it, I just thought I found out something cool and useful and decided to tell others, but if you think it's useless it's your choice.

EDIT: There should be a tips section! For all those tutorials that were rejected for not being "tutorial-ish" enough but are great things to know, like: How add/give Random Abilities to a unit and Invisible Model - Visible Attachment


 Last edited: Jul 12, 2010

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Everlive said:

 You can graveyard it, I just thought I found out something cool and useful and decided to tell others, but if you think it's useless it's your choice.


Click to expand...


It is useful! I just think it doesn't warrant a tutorial, mostly for its length. Many people are not aware of this, and it can certainly save time, and is just a neat technique to know in general. If there was a "tips" thread, this would certainly make a great addition. =)


Anyway, I'll go ahead and graveyard it then.

#### HelloFlighty

#### HelloFlighty

 Level 8

JoinedJul 29, 2010Messages319

Resources: 2

#### Resources
Maps 2

i don't get that id popup :/

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

Do you have Jass Newgen Pack?

#### doom_sheep

#### doom_sheep

 Level 27

JoinedOct 18, 2008Messages955

Resources: 44

#### Resources
Models 43Spells and Systems 1

Everlive said:

###
 Step 2: The Unit ID

After pasting, this will pop up:

Click to expand...

no it won't. I dunno what you are doing but it doesn't work that way for me.

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

doom_sheep said:

 no it won't. I dunno what you are doing but it doesn't work that way for me.


Click to expand...

LordDz said:

 Do you have Jass Newgen Pack?


Click to expand...

#### Hive_Parasite

#### Hive_Parasite

 Level 8

JoinedAug 19, 2010Messages172

Resources: 2

#### Resources
Maps 2

Can this be done to an altered melee map not done in Jass?

Edit:I also want to ask if this works with patch 1.26?


 Last edited: Feb 7, 2017

#### BLazeKraze

#### BLazeKraze

 Level 34

JoinedFeb 17, 2013Messages1,336

Resources: 167

#### Resources
Icons 125Packs 4Maps 33Templates 3Sounds 2

I don't know if it's too late to reply. But I think this is useful to me. Because I'm tired editing animation, blending time etc. when creating new hero. But I think it's useful to others but this is unnecessary for most people here.

 You must log in or register to reply here.


### Similar threads

 Converting Normal Units to Hero Units


Ralle

Jan 9, 2006

Tutorial Archive

Replies1Views3K

Dec 5, 2006
Daelin

 Tutorial-Making a hero


Global_idiot

Feb 29, 2008

Tutorial Archive

Replies13Views4K

Mar 30, 2008
Matrix

 Creating Double-Click Hero Selections (GUI)


ghost ROM

Apr 22, 2010

Tutorial Archive

Replies3Views2K

Apr 22, 2010
Septimus

 [UNIT => Hero] Add glow system


Geoffroy77

Jan 28, 2009

Tutorial Archive

Replies8Views3K

Apr 12, 2009
Pyritie

 Up the level of Unit Spells on a Hero


PlayersFear

Dec 25, 2008

Tutorial Archive

Replies0Views1K

Dec 25, 2008
PlayersFear

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