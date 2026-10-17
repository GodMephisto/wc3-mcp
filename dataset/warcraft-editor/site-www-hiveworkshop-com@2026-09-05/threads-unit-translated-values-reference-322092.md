---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/unit-translated-values-reference.322092/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Unit Translated Values Reference
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/unit-translated-values-reference.322092/>
#### OnGe

#### OnGe

 Level 5

JoinedFeb 3, 2020Messages25

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

I have not found anything about this topic, so I explored it myself and written it down.

Via Trigger editor you can directly modify unit values, including some with translated values. That means stuff with dropdown in object editor, bud represented as integer in unit values. This way, you may, for examplem change defense type, movement type, formation rank and more. Because it is not clear how listed values are translated to integer values, here is reference table:

Integer Values:

Defense type "udty": (armor type displayed on unit)

0 - Light

1 - Medium

2 - Heavy

3 - Fortified

4 - Normal (that strange unused type)

5 - Hero

6 - Divine

7 - Unarmoured

8+ - broken - no icon, says just "Armor" and has default "Normal" tooltip. Better avoid this.​
Armor type "uarm": (defines sound it makes when hit)

0 - no sound

1 - Flesh

2 - Metal

3 - Wood

4 - Ethereal

5 - Stone

6+ - no sound, if needed, better use 0​
Formation rank "ufor": (how will unit act in formation - bigger number means safer distance)

0 - Melee (Footman, Grunt...)

1 - Skirmish (Spellbreaker - maybe only on of that rank)

2 - Ranged (Priest, Rifleman, Headhunter...)

3 - Long range (Seige engine, Demolisher...)

4 - Non-combat (workers)

5+ havent found any unit with that values​
Weapons Integer Values:

With these values, you have to pick index of attack:

0 - attack 1

1 - attack 2​
Attack Type "ua1t": (attack type displayed on unit)

0 - Normal

1 - none - attack disabled

2 - Piercing

3 - Seige

4 - Magic

5 - Chaos

6 - Hero

7+ - undefined - attack is displayed with no icon and acts weird. Better avoid this.​
Attack Weapon Sound "ucs1": (acts weird, here is my findings)

0 - no sound

1 - attack disabled - dont ask me why

2 - Metal Medium Chop

3 - allowed targets messed up, unit can not attack properly

4 - allowed targets messed up, unit can not attack properly

5 - allowed targets messed up, unit can not attack properly

6 - Metal Heavy Slice

7 - allowed targets messed up, unit can not attack properly

8 - allowed targets messed up, unit can not attack properly

(I have stopped here, because messing with this value seems pointless)​
Attack Targets Allowed "ua1g": (what I do here is mostly wild guess based on what unit can and can not attack)

0 - any unit (friend, enemy, structure, hero...), but not item, ward, tree, debris or wall

1 - can not target anything, just attack-move on ground

2 - Ground - can attack ground units, but not structures

3 - can not target anything, just attack-move on ground

4 - Air - cat target only Air units

5 - can not target anything, just attack-move on ground

6 - Ground + Air (?) - can attack land and air units, but not structure, ward

7 - can not target anything, just attack-move on ground

8 - Structure - can target any structure, but nothing else

9 - can not target anything, just attack-move on ground

10 - Ground + Structure - can attack ground units and structures​

Here, probably any option you can check on Allowed Targets has value that is power of 2. That means 2,4,8,16,32,64... plus any possible sum of that values to give any possible combination. I do not want to explore this further but if you want to, go on and share your findings.​

There is many more translated values and I will add them as soon as I explore them. If you have your own findings, please, share. If there is already such reference somewhere, please, point me there.​

Cheers!


 Last edited by a moderator: Oct 7, 2020

#### OnGe

#### OnGe

 Level 5

JoinedFeb 3, 2020Messages25

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

I have added Formation ranks and weapon integer values (not much useful stuff here)

#### maddeem

#### maddeem

 Level 24

JoinedJan 1, 2011Messages1,506

Resources: 34

#### Resources
Models 5Icons 3Skins 1Maps 6Spells and Systems 18Lua 1

Very useful!

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I am choosing to approve this despite it being a potentially unfinished list as I think the usefulness is very high even in its current state.

I did a few structure changes for readability as I suspect the author is not currently active.

#### disruptive_

#### disruptive_

 Level 8

JoinedMar 19, 2017Messages248

This is useful, but some of this information, for those interested in why the values return the behaviour reported here, can be accessed in common.j.

For example, about "Targets Allowed" (a bitfield, so you can do things like: GetHandleId(TARGET_FLAG_GROUND) + GetHandleId(TARGET_FLAG_AIR)):

 JASS:

 // Target Flag
 constant targetflag TARGET_FLAG_NONE = ConvertTargetFlag(1)
 constant targetflag TARGET_FLAG_GROUND = ConvertTargetFlag(2)
 constant targetflag TARGET_FLAG_AIR = ConvertTargetFlag(4)
 constant targetflag TARGET_FLAG_STRUCTURE = ConvertTargetFlag(8)
 constant targetflag TARGET_FLAG_WARD = ConvertTargetFlag(16)
 constant targetflag TARGET_FLAG_ITEM = ConvertTargetFlag(32)
 constant targetflag TARGET_FLAG_TREE = ConvertTargetFlag(64)
 constant targetflag TARGET_FLAG_WALL = ConvertTargetFlag(128)
 constant targetflag TARGET_FLAG_DEBRIS = ConvertTargetFlag(256)
 constant targetflag TARGET_FLAG_DECORATION = ConvertTargetFlag(512)
 constant targetflag TARGET_FLAG_BRIDGE = ConvertTargetFlag(1024)

Passing 0 (2^0) to targets allowed is interesting though.

 You must log in or register to reply here.


### Similar threads

 Memory Leaks


IcemanBo

Mar 6, 2015

Trigger (GUI) Editor Tutorials
2

Replies95Views79K

Jul 25, 2024
corpuzmarvicjune

Locked

[Trigger]
 Refering to a unit


Darkshine

Mar 2, 2010

Triggers & Scripts

Replies6Views1K

Mar 3, 2010
Darkshine

Locked

[Trigger]
 How To Refer Last Created Lightning ?


defskull

May 31, 2011

Triggers & Scripts

Replies10Views2K

Jun 1, 2011
Bribe

Locked

 Attacking for 0 Damage


greasontim

Jan 18, 2011

World Editor Help Zone

Replies13Views2K

Jan 19, 2011
greasontim

Locked

 How to use Variables to reference a particular unit?


saberslash117

May 27, 2023

World Editor Help Zone

Replies2Views1K

May 29, 2023
saberslash117

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