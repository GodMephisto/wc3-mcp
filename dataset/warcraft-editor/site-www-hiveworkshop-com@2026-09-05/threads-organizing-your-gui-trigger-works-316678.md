---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/organizing-your-gui-trigger-works.316678/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Organizing Your GUI Trigger Works
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/organizing-your-gui-trigger-works.316678/>
#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Organizing Your GUI Trigger Works

Introduction

As usual, this is Daffa the Mage bringing another tutorial in relation to the trigger editor. This time, we will talk about managing lots of triggers in trigger editor and making your project as controllable as possible from triggering aspect.

Content

The content of this tutorial covers how to structure your trigger and working with them. This may in part have a relation with GUI Proper Application Guide that can be found here. This tutorial aims to make your long term project more feasible. However, it will add an initial burden to the development in the triggering aspect. This is aimed for GUI (being a GUI user myself), though this is applicable in Jass, vJass, cJass, Zinc, Wurst, and our newcomer Lua.

Naming stuff

Handling values in triggers

Handling duplicate triggers

Structuring your trigger folder
I will be focusing on these four aspects for this tutorial. The aim is to have a well-defined structure in your work that will give some advantages for the long run. Here are the advantages:

Minimizing confusion

Less clutter

More clarity

Better trigger structure
Naming Stuff

When you are naming stuff that you wrote, it's important to make sure that IT IS EASY TO UNDERSTAND. Not only for you but also for others. This is VERY IMPORTANT IN TEAM PROJECTS. Say, when one of you got hit by a bus, it's important the other trigger maker can read and work with your code so the project continues while the bus struck person recovers.

Some convention from GPAG that can be helpful for you:

THIS_IS_CONSTANT

MyVariableName

MyTriggerName

Also from GPAG, ideally one will add a PREFIX to any variable used. This means your final variable would look something like one of these:

PREFIX_THIS_IS_CONSTANT

PREFIX_MyVariableName

PREFIX_MyTriggerName

What these PREFIX would represent? It represents a system, for example, My Awesome System, or a spell, like My Awesome Spell. This helps you identify better where these variables are meant to be used.

Some examples in action:

AWESOME_SYSTEM_Damage

AWESOME_SYSTEM_Health

AWESOME_SYSTEM_Counter

AWESOME_SPELL_DamagePerSecond

Handling Values in Triggers

When you are making a trigger, say, a spell, you will have some arbitrary number that pops out of nowhere in the code, in which can be easily called magic number. If it's only one or two lines, you should be able to tell which do what, but as the trigger extends, it gets more annoying to navigate. These magic numbers also not completely understandable by others as well. Thankful that GUI has that descriptive style... but wait!

Now I have spell A that modifies the value of variable x to x+600 then add another x+600, spell B that modifies x to x+600 and spell C that takes x as one of its parameters. Then a balance patch needs to change both spells A and B from x+600 to x+750, well... good luck me to not miss any of those. How about if it has like 4 different triggers have a relation with just one change? Welp. It's easy to miss them. And since they use the same value, if I only need to change A, I might accidentally change B in confusion.

So, what we will do? First thing, those magic numbers (or strings or whatever) need to have someplace to store them and use them on their respective triggers. The storage needs to be initialized on Map Initialization. They need to be constant as well (at least for GUI case, others can play a bit with function and return). This ensures that the value comes from one source and if the need arises to change this value, we just need to change that one source.

What we can save in these constant variables? Anything that is constant in their use. This allows you to rely on one trigger source to handle all the values when you need to change them for balance reasons. Keeping them in one place will help a lot when you need to change something, especially when you have like 80 triggers or something that can cause you to get lost easily.

If you hate the fact it gets too messy with many lines of Set ..., you can always separate them into several triggers. One for spells, one for systems, and so on.

Handling Duplicate Triggers

You create something once for spell A. You then later create this again for spell B. And... yes, one more time for spell C. At that rate, it's better to just have the three of them have their similar actions combined into one trigger and call that trigger via Run Trigger. Before running the trigger, create some global variables that act as parameters to the target trigger. This is something that some systems in spell section meant to do for you, such as the Shockwave System and Spell System, so you might consider them if they fit your need.

It's important to minimize the number of actions in each trigger to a number of like 10 to 20, but its not always the case. Though by delegating some actions to other triggers which help with how the trigger flows and make them better to read overall, do not delegate everything though, as that will cause Middle Man problem where the trigger only act as a bridge of sort to other triggers. Just find the balance between having too much trigger and a big momma trigger that you cannot read what's happening at all.

Structuring Your Trigger Folder

The simplest would be to just arrange each portion to one folder. Spells in one folder, cinematics in another one folder, and so on. Sometimes it might even be a good idea to split some into two or three folders, depending on the complexity.

The key is to have an elegant and easy to navigate structure. Say, the team has two spell makers who both works in GUI. These guys can actually work only in the Spell folders and tell them the systems are in System folders. Much better than navigating through one folder with LOTS... I mean LOTS of trigger with different purposes, confusing the team as a whole.

Final Words

I hope this tutorial can be of some use for the trigger makers or maybe project managers out there. This is something new to me and believes me, I'm also still experimenting with it. Mentally taxing, yes, but I felt that it help me navigate through my project better, as it gives me a heads up of where my constants for balancing be, where the trigger for A or B is located and so on and so on.

Of course, it won't be honorable if I don't mention where I get this concept from. For the most part, I took it from refactoring.guru as well as reading some summaries about clean code. Though those are more aimed toward OOP, I can see some of its contents might benefit in development for Warcraft 3 maps, especially big maps. See ya!


 Last edited: Nov 9, 2019

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Looks great! My one comment would be that you should mention the PREFIX_ part from GPAG in your "Naming Stuff" section (i.e. MyVariableName would really be PREFIX_MyVariableName, like FIREBALL_TargetPoint). It is pretty important to make sure your names don't collide and are grouped/easy to edit in the variable editor, especially when your project gets massive!

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

@PurgeandFire Added the prefix issue, also mentioned that having magic values can turn confusion when patching (mistaking one for the other). I hope it's all clear now, if there's anything, do tell

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Wonderful! Approved!

 You must log in or register to reply here.


### Similar threads

Sticky

 How to get your GUI spell approved


Maker

Nov 24, 2011

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jan 10, 2019
Daffa

 How to: Create Spells in GUI - The Beginning


baassee

Aug 18, 2011

Trigger (GUI) Editor Tutorials

Replies6Views32K

Oct 22, 2011
PurgeandFire

 [GUI] Advanced Triggering Tips


Bribe

Mar 1, 2011

Trigger (GUI) Editor Tutorials

Replies23Views27K

Dec 20, 2012
PurgeandFire

 Chaosy's GUI tutorial for beginners


Chaosy

Apr 6, 2012

Trigger (GUI) Editor Tutorials

Replies9Views8K

Dec 25, 2016
Chaosy

 Spell System for GUI MUI Coding


Daffa

May 5, 2019

Trigger (GUI) Editor Tutorials

Replies7Views4K

Oct 3, 2020
Chaosy

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