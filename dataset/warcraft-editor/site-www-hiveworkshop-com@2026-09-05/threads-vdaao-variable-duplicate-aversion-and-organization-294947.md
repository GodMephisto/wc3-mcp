---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/vdaao-variable-duplicate-aversion-and-organization.294947/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Vdaao Variable Duplicate Aversion and Organization
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/vdaao-variable-duplicate-aversion-and-organization.294947/>
#### Xeryxoz

#### Xeryxoz

 Level 8

JoinedJun 27, 2014Messages227

Resources: 3

#### Resources
Models 1Spells and Systems 1Tutorials 1

Introduction

I am sure everyone has experienced a duplicated variable once or twice in a map they were making. There are always many ways to regulate it, but nobody really does it. So in this tutorial I will be showing a few neat ways of how to avert

Our options...

The best one is using the first prefix(es) to account for a select system.

People don't use two same systems for the same goal, so the chance of overlapping is minimal, therefor if we put the first prefix to represent our system or it's name, the chances of duplication are very small. There is also the possibility of using your author's name or an unique part of it. That's something people can't overlap (Only if you type in a simple word in it like Chaos or Orc - this is where unique names are handy).

Examples :

EQsys_Uequip_Cast - Makes chances of the same named variable much lower.

Henn_Uequip_Cast - Nobody can copy an unique part of your name.

Slightly longer but it should be worth it. Because you labeled all your variables like this, it's easier to find them in the variable's tab. Not only that, but if you are not sure what variable corresponds to which system - this would be a much faster and safer way to understand.

You could also try combining...

EQsys_H_Uequip_Cast - Very low chance of duplication

And if that fails - the use of numbers in a thread is possible (now that would remove it completely).

294947 - This thread's number. I'll use it for the example bellow

T294947_Uequip_Cast - First letter represents Thread, as using the numbers first would cause some issues with the variables...

Anyways - using this method, duplicate variables would never happen, but organization would not be as good. You would also have to put the map on hive to get the thread as well...).

The only way someone could duplicate this last one is if they failed in copying and pasting their thread number (for some odd reason). When we also add into account that those two codes would have to be in the same map, I can proudly say that the chances of this happening are as low as science proving the existence of God....

Do note - changing variable and array names may cause triggers to stop working correctly, so make a backup copy just in case!

Final words

There are many more ways to avert duplication, but I think these should be the most efficient.

If you want to know more possibilities GPAG - GUI Proper Application Guide should be a reference.


 Last edited: Jun 30, 2017

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

In GUI, this would have some use. Anyway, you might want a header or something to make this less messy.

There are several case where duplicates are needed. Yes, I know it's ridiculous as it sounds.

Take infamous Temp_Blabla with the Blabla changed into Point, Int and list goes on.

It's a good practice however, to have different variables whenever possible.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Don't use that brownish something color. Use the red one instead.

#### Xeryxoz

#### Xeryxoz

 Level 8

JoinedJun 27, 2014Messages227

Resources: 3

#### Resources
Models 1Spells and Systems 1Tutorials 1

Daffa the Mage said:

 In GUI, this would have some use. Anyway, you might want a header or something to make this less messy.

There are several case where duplicates are needed. Yes, I know it's ridiculous as it sounds.

Take infamous Temp_Blabla with the Blabla changed into Point, Int and list goes on.

It's a good practice however, to have different variables whenever possible.


Click to expand...

Indeed, sometimes it may be good, but in most cases it can mess something up. Having a sure way of preventing duplication for most variables is good practice, just like you commented.

Daffa the Mage said:

 Don't use that brownish something color. Use the red one instead.


Click to expand...

Kind of think the brownish one fits with the regular text color, but will do. Also, I've updated the Terrain Add-on, Would you care to look at the code and tell me what you think of the current version?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

I don't know if I find it good someone uses arbitary prefixes. It should be system related, and for example usage of thread numbers doesn't make any sense.

The variable should be associated with a spell, or system, so a useful prefix name is asked for. And if a good prefix name is chosen, then it's most likely enougb safety for most cases. Sorry, not sure this this is a good tequnique to use.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I have to agree with IcemanBo, I feel like this is covered well under GPAG. In general: (1) be consistent (use the same prefix system throughout your map), and (2) make sure it makes sense for the trigger. Choosing a random number or prefix is just going to be confusing.

Thanks for the contribution. Sadly, I think I'm going to graveyard this, sorry.

#### Xeryxoz

#### Xeryxoz

 Level 8

JoinedJun 27, 2014Messages227

Resources: 3

#### Resources
Models 1Spells and Systems 1Tutorials 1

PurgeandFire said:

 I have to agree with IcemanBo, I feel like this is covered well under GPAG. In general: (1) be consistent (use the same prefix system throughout your map), and (2) make sure it makes sense for the trigger. Choosing a random number or prefix is just going to be confusing.

Thanks for the contribution. Sadly, I think I'm going to the graveyard this, sorry.

Click to expand...

No problem ^^ I mean, this was a one-shot idea which was aimed to cause better sorting in the variables and the ability to quickly identify which system what variable is connected to ^^ (To think that I made this a year ago..)

 You must log in or register to reply here.


### Similar threads

 Local Variables


jonadrian619

May 19, 2007

Tutorial Archive

Replies7Views28K

Jun 27, 2007
Wolverabid

 Avoiding waits with local variables


Kingz

May 11, 2009

Tutorial Archive

Replies6Views3K

May 20, 2009
Pyritie

 All About Variables


Slaydon

Aug 12, 2009

Tutorial Archive

Replies9Views2K

Aug 26, 2009
Eleandor

Locked

 Duplication oddities and inconsistencies? (Duplicate Settings?)


Newuser

Dec 23, 2010

StarCraft II Modding

Replies10Views3K

Jan 15, 2011
Newuser

Locked

 Noob To Variables - Need Help


Pikachu

May 1, 2020

Triggers & Scripts

Replies7Views1K

May 3, 2020
Pikachu

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