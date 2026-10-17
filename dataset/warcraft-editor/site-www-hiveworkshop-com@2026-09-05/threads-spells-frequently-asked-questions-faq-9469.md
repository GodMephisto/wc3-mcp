---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/spells-frequently-asked-questions-faq.9469/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Spells Frequently Asked Questions Faq
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/spells-frequently-asked-questions-faq.9469/>
#### The_Raven

#### The_Raven

 Level 9

JoinedJun 10, 2004Messages594

Resources: 4

#### Resources
Tutorials 4

Spells – Frequently Asked Questions (FAQ):

1) How do you make spells? What program(s) do you use?


You make spells primarily by using the Warcraft III World Editor. Spells are one of the most complicated aspects of map-making because they involve combining all your knowledge of the World Editor – GUI triggers, the object editor, and special effects. Additionally, you need creativity and a sense of art and aesthetics. For examples of well-made spells, look around in the Spells Section and see what others have done. As is often the case, your first few attempts will probably not be very good. There is nothing wrong with this, but please don't submit it. The golden rule is: don't submit a spell unless you're proud of it.

2) How do I import spells into my map?


Spells are usually a combination of data in the object editor and triggers. You can import spells into your map in two ways:


 • Option #1: Export all object data and trigger data via File > Export All Object Data and File > Export Triggers. You should then import the data into your map via File > Import All Object Data and File > Import Triggers.


 • Option #2: Since the process described in option #1 will overwrite all of your current data, sometimes it is easier to Copy and Paste the individual triggers and objects into your map. This can be done using The Copy and Paste buttons on the toolbar, Edit > Copy and Edit > Paste, or Ctrl + C and Ctrl + V.


It is important to make sure that all references to buffs, spells, and units were copied correctly. In addition, you should enable Preferences > Automatically create unknown variables while pasting trigger data.


If you don't get it, try this tutorial.

3) I have multiple spells that are all based off the same spell. When I cast one, they all cast. How do I fix this?


Since they are all based off the same spell, they all have the same Order ID. Base them off different spells. This is the only way to fix this problem.

4) What is a dummy unit/caster?


A dummy unit/caster is a non-selectable, invisible unit that it only created to carry out a function of a trigger.

5) How do I make my own dummy unit/caster?


Base your dummy unit/caster off footman. Give it the locust ability. Change its Art - Model File (type it on the field down entitled Custom), Art - Shadow Image (unit) and Combat - Attacks Enabled fields to none. Set Movement - Type to Fly; Movement - Speed Base, Stats - Food Cost and Stats - Sight Radius (both Night and Day) to 0.

6) What is a dummy spell?


A dummy spell is a spell given to a dummy caster used to carry out a particular aspect of a spell. Dummy spells usually have no cooldowns, no mana cost, and an unlimited range.

7) How do I make a unit non-selectable?


Give it the “Locust” unit ability. This will make it invulnerable, non-selectable, and non-targetable. Note that this ability will also make the unit loose its pathing.

8) How do I allow the World Editor to use negative values?


Go to File > Preferences. Under the General tab, check the box titled “Allow negative real values in Object Editor.” Note that it is sometimes necessary to hold down the shift key while inputting negative values.

9) How do I make the heroes get over level 10?


Go to Advanced - Gameplay constants and check the box in the upper-left corner. Then search for Hero Maximum Level and change it to the maximum level your heroes can get to.

10) Even though I set the movement speed of my units down to 0, the unit still can move. What's wrong?


Go to Advanced - Gameplay Constants and search for the field Movement - Unit Speed - Minimum. Change the value there to 0. If you want to increase the maximum movement speed, change the Movement - Unit Speed - Maximum field. Keep in mind that these changes will affect all units!

11) How do I change what objects my spells can target?


Edit the field “Stats - Targets Allowed.”

12) How do I change the text on the expiration bar of my summoned unit?


Change the buff for the ability. The name of the buff is what will appear on the bar.

13) What is the difference between "An Unit Begins Casting an Ability" and "An Unit Starts the Effect of an Ability" events?


Faint yet existant! While the first event starts the trigger right before the unit casts the ability (and so mana cost and cooldown are not fired) while the second one starts the trigger immediately after the spell is cast.

14) Why are my units moved to the center of the map when in the trigger I move them to (Target Point of Ability Being Cast)?


That's probably because you accessed the (Target Point of Ability Being Cast) after a wait. Similar constants such as (Casting Unit), (Attacking Unit), (Target Unit of Ability Being Cast) and others are reset after the smallest wait. You can solve this by using variables to store them or in some cases, you can use (Triggering Unit). It is one of the few constants that is not affected by waits.

15) What is JASS and how can I learn it?


Try this tutorial.

16) What kinds of spells are there?


Generally speaking, there are three kinds of spells:


 • Area of Effect (AoE): This kind of spell can target multiple units at the same time. The target area for AoE spells is determined by a large circle with a dot in the center. Examples: Blizzard, Force of Nature, and Death and Decay.


 • Targetable: These spells can target a single unit at a time. They are identified by the cross hairs that appear when the spell is selected. This classification also includes auto-cast and chain spells. Examples: Chain Lightning, Heal, and Holy Light.


 • Non-Targetable: This classification applies to spells that can't be targeted. Examples: Avatar, Big Bad Voodoo, and War Stomp.

17) In what ways can spell cooldowns be manipulated?


Spell cooldowns are notoriously inflexible. Because of this, I would like to clarify a few points regarding spell cooldowns:


 • The remaining time left in a spell’s cooldown cannot be set via triggers.


 • The cooldowns of specific spells cannot be reset via triggers without resetting all of the other spell cooldowns of that unit. The only way to reset specific spell cooldowns is to remove the abilities that you want to reset and add them back in.


 • The remaining time left in a spell cooldown cannot be detected via triggers. At best, this can only be guessed at using waits and periodic events.

18) How can I detect passive abilities such as Critical Strike and Evasion?


The only way to do that is by triggering the abilities yourself.

19) What are memory leaks?


Read this Tutorial to find out. Keep in mind that memory leaks occur even when you use GUI, not only in JASS.

If you have a comment or suggestion for the FAQ, then please PM Daelin. If you have a question about the FAQ or spells in general, then please create a separate thread for it.


~The_Raven and Daelin


 Last edited by a moderator: Mar 8, 2007

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Graveyarded.

Much of the information is outdated and most of the answers can be easily found by just using the search function.

Status
 Not open for further replies.


### Similar threads

 Creating a Custom Spell


Parrothead

Nov 11, 2007

Tutorial Archive

Replies8Views4K

Mar 28, 2008
underscore

 Adding Fly Height to Any Unit


orcfan32

May 20, 2005

Tutorial Archive
2

Replies62Views32K

Nov 9, 2011
PurgeandFire

 Spells - Making a Chain spell (GUI Version)


Daelin

Oct 30, 2006

Tutorial Archive

Replies1Views3K

Mar 11, 2007
Wolverabid

 Targetable Invulnerability


DarkLordX

Dec 9, 2008

Tutorial Archive

Replies2Views2K

Dec 10, 2008
~Void~

[Mapping]
 How to give a unit splash damage via Item, Spell, or Aura.


Eric Jensen

Jan 15, 2019

Tutorial Archive

Replies6Views7K

Dec 22, 2025
Lamer

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