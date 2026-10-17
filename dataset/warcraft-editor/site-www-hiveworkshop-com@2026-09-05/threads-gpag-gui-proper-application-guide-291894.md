---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/gpag-gui-proper-application-guide.291894/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Gpag GUI Proper Application Guide
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/gpag-gui-proper-application-guide.291894/>
#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

GPAG - GUI Proper Application Guide

1. Introduction

GPAG defines uniform standards and conventions that we strive to apply in our GUI submissions.

The goal is to come on the same page for coding style, and to ensure clean and readable triggers.​

2. Naming Conventions

2.1. Prefix

To avoid name collision each system is asked to define a PREFIX. The spell/system PREFIX is a constant name and should be resource descriptive. For example, if it's a fireball spell, the prefix might be "FIREBALL" or "Fireball". It's up to you, if you decide to write the prefix LIKE_CONSTANTS, or like VariableNames -- choose what you prefer. The PREFIX length should be at least 3-4 characters long, or even more, to ensure some uniqueness. Also, no matter what variables your system creates, all of them must be advanced by the PREFIX and an underscore _.​

​
2.2. Variables

VariableNames start with a capital letter and also each new word is indicated by a new capitalitzed letter. The name itself must be descriptive for the variable's purpose. - What does this mean? When you work with temporary variables it's totally okay to give generic names, like: "Unit, String, Real, Point". However, when the variable usage is specified, then its name should be specified as well. So then, the name style changes from generic to specific. Here are examples:

 Generic/Temporary usage

Unit

Real

 Specific Usage

Caster/Target

Damage/Distance

The final variable name, combined with the prefix, looks like: PREFIX_VariableName:

FIREBALL_Caster

FIREBALL_Duration​

​
2.3. Constant Variables

CONSTANT_VARIABLES are exclusivly written in capital letters and each new word is seperated with an underscore _ . Constant variables are only set once, usually after map loading, and the capital letters indicates to never change the variable's value. They are usually used to define some system default datas to get the system working.

The final variable name for constants, combined with the prefix, looks like: PREFIX_CONSTANT_VARIABLE.

FIREBALL_ABILITY_ID

FIREBALL_ORDER_ID​

​
2.4 Triggers

The same principle from variables applies to your triggers. The trigger's name will result in: PREFIX TriggerName.

The TriggerName should be descriptive, similar like variable names. Example names for triggers for a fireball spell:

FIREBALL Setup -> here the user can find everything config related.

FIREBALL Cast -> this is the trigger with obivously "A Unit Casts" event.

FIREBALL Periodic -> this trigger runs periodically. Probably used for fireball movement.​

​
2.5 Object Data

For object editor data, except for items and doodads, there exists a field named "Text - Editor Suffix". This field comes handy when your system uses custom object data, so you can use it to put in your PREFIX there. The idea behind is that the user will always be able to easily distinguish which object data belongs to which system or spell.

There are cases though, where it doesn't really make sense to use a object suffix. For example, when we import a "Fireball" spell and the prefix is "Fireball".

The name result in object editor would look like "Fireball (Fireball)", so you see that it doesn't really help anyone to distinguish data.

However, when the object editor data differs from the prefix, then using "Text - Editor Suffix" is indeed often helpful and asked for.​

3. Documentation

Documentation is something good and each coder's friend. However, only when done correctly. We don't need to know uninteresting details like

"Here I set my integer variable." We can see that! Instead, it's much more important to understand why/for what you do something. We want to understand the purpose.

You usually don't need to document each single line, but instead, try to describe a whole code block in a concise manner, always when you think that some information could be helpful at that point. The bigger the code gets, the more benefits you will have from comments.​

4. Configuration

Configuration needs to be seperated from the system's core code. That's mostly done with putting all config-related parts into a seperate trigger.

Usually, things like:" Damage, Size, Durations, Radius, Effects, ..." are meant to be configured. The user doesn't care about the system's core code and he only wants to know what he should change to get the code properly working. Therefore, also the configuration always requires a proper Documentation to ensure that the user easily can follow and understand everything related.​

5. Last Word

We prefer following GPAG, but at same time we won't blindly enforce it. So in case your personal standard sligthly differs from GPAG, it may be accepted as well, as long as clean and readable code is ensured. In case of uncertainties feel free to inquire about it with the respective moderators in Staff Contact or discuss it here in this thread.​


 Last edited: May 14, 2019

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Prefixes:

Firstly, I do not think 4 characters+ is needed. After all, with just two characters (Fireball > FB) you still have 729 possible combinations. So in theory, 729 GUI spells and systems. I mean sure you might be a tad unlucky and get a duplicate, all I am saying is that it's not that big of a deal. Also, I think consistency is more important than how you actually name them. So you don't do something like:

FB_a

FB_b

FiBa_c

FiBa_d

My last point goes for variables as well. As long as all your variables are the same, all fine.

Constant variables:

This may make sense for someone who likes jass, but I do not think this is something to be enforced for everyone.

Suppose I agree with everything else.


 Last edited: Jul 30, 2019

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Chaosy said:

 but I do not think this is something to be enforced for everyone.


Click to expand...

IcemanBo said:

 We prefer following GPAG, but at same time we won't blindly enforce it. So in case your personal standard sligthly differs from GPAG, it may be accepted as well, as long as clean and readable code is ensured.


Click to expand...

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

"2 letters" -> " much combinations"

--

well, in theory if you count the systems how many you import, then it might be enough. But practicaly also often same letters are chosen, and some aren't (like "ZZ", "YY", .. etc you probably won't find often)

Yeh, this "constant" style in GUI is/was practicaly not often used, but when I tried it out, it was really a bit better to read in the GUI core code, because one could immediatly see that

this variable is not instance-specific, but just a default constant.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Not sure why this is still sitting in Submissions. Approved and stickied!

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

I find Periodic better with Loop instead.

#### JC Helas

#### JC Helas

 Level 14

JoinedOct 19, 2014Messages187

Resources: 12

#### Resources
Icons 1Maps 2Spells and Systems 9

In system part should we document it too? Or maybe only those part which need to explain because we have our own acceptable style?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Yes, it's usually useful if the system core code has some documentation. Documentation has a different meaning, depending if it's made in the config, or the core code.

 The core code

Here documentation is used to understand how the system even works.

The configuration

Here documentation is used to understad how to take usage of the system.

Readabilty and and documentation will help the reviewer, yourself, and just any reader that comes to read your code, and may safe so much time that's invested to understand it in future.

#### Map Designer

#### Map Designer

 Level 17

JoinedMay 2, 2011Messages1,351

Resources: 6

#### Resources
Maps 2Spells and Systems 4

What about spell pack conventions?

If pack has 4 spells, should they all have 1 prefix or 4 different ones?

IcemanBo said:

LIKE_CONSTANTS, or like VariableNames

Click to expand...

Is this true? it seems to me that you wanted to say like_VariableNames

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Map Designer said:

 it seems to me that you wanted to say like_VariableNames

Click to expand...

No, I didn't want to say that. Why?

Map Designer said:

 What about spell pack conventions?


Click to expand...

If all spells of a pack have exactly same prefix you need to have unique normal names, which would be not the sense. Example "ABCD" is the spell pack prefix, then there can be only one ABCD_ABILITY_ID, etc. If you would like you could have 2 prefixes, like ABCD_SPELL1_ABILITY_ID, ABCD_SPELL2_ABILITY_ID, etc - to show those spells are like in same group.

 You must log in or register to reply here.


### Similar threads

 Organizing Your GUI Trigger Works


Daffa

Jun 27, 2019

Trigger (GUI) Editor Tutorials

Replies3Views2K

Jul 5, 2019
PurgeandFire

Locked

 Conventions


PurplePoot

Sep 20, 2009

Triggers & Scripts

Replies0Views726

Sep 20, 2009
PurplePoot

Locked

 vJASS Questions


overcold_ice

Dec 26, 2011

World Editor Help Zone

Replies4Views1K

Dec 29, 2011
overcold_ice

Locked

 File changes from GUI to Jass


katya18

Jan 31, 2021

Advanced Scripting

Replies5Views2K

Feb 2, 2021
katya18

[vJASS]
 [Snippet] Array2D


Kazeon

Jan 23, 2015

Graveyard

Replies13Views3K

Aug 23, 2015
Bribe

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