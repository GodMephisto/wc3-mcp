---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/wc3s-damage-system.279673/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Wc3s Damage System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/wc3s-damage-system.279673/>
#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

Source: This Wc3c thread.

Table of Content

I. Introduction

1. Attack type (AT)

2. Damage type (DT)

3. Process for damage calculation​
II. Detailed information about Attack types and Damage types

1. Attack types

2. Damage types

3. Some known examples

4. Notes​

I. Introduction

Damage in Warcraft 3 is made up of 2 components: Attack type and Damage type. This is the case for every damage instance, be it attack or spell, even though the game rarely ever makes it explicit to us.

1. Attack type (AT): This is colloquially known as the "Normal, Siege, Piercing, Chaos, Magic etc." that you see when hovering over a unit's basic attack. What Attack type does is comparing itself against the target's Defense Type to work out whether it will be reduced or increased, before armor is taken into account. The damage factors VS. Defense types for all Attack types can be modified in Gameplay Constants.

For example: Siege Attack type has a 1.5x multiplier against Fortified Defense type before taking into account the target's Defense Value (armor). Thus, the target's armor will have to deal w/ 1.5x the attack value of the Siege attacker.

2. Damage type (DT): This is the information that you rarely ever see, which is only available to choose in the "Unit - Damage Area/Target" triggers. Ever wondered why Slow Poison and Disease Cloud's damage always leave the target at 1 HP without killing, but Shadow Strike, which is also a (poison) Damage over time spell, can kill the target? It's because they have different damage types.

What damage type does is determining whether the damage instance will be reduced by armor value.

For example: Slow Poison (Aspo) deals AT: Spells, DT: Slow Poison damage; in-game, it is only reduced by Hero Defense type since AT: Spells deals 0.75x damage VS. Heroes, and DT: Slow Poison ignores how much armor the target is packing.

3. Process for damage calculation

Step 1. Attacker's attack value gets multiplied with its AT bonus/penalty against the target's Defense Type.

Step 2. Attacker's DT is checked. If the DT is one that ignores armor, the multiplied damage from Step 1 is applied. Otherwise, the multiplied damage is further reduced by the target's Defense Value before applying.

II. Detailed information about Attack types and Damage types

That being said, the following are classifications of ATs and DTs, so you have a good idea of what to use in Triggers:

1. Attack Types

There are a total of 7 attack types but since some of them have similar behaviors, they belong to 3 broad categories:

- Normal (Normal, Piercing, Siege, Hero, Chaos): It's essentially a PHYSICAL attack type. Ethereal units ignore all damage with this AT.

ATTACK_TYPE_MELEE (this is AT:Normal),

ATTACK_TYPE_PIERCE,

ATTACK_TYPE_SIEGE,

ATTACK_TYPE_HERO,

ATTACK_TYPE_CHAOS

- Spells (Spells): It's an attack type for SPELLS. Understandably, it's used by all default spells in this game. AT: SPELLS is the most versatile AT since it can be ignored by Ethereal/Spell Immunity based on damage type. The Runed Bracers ability reduces damage from this AT only.

ATTACK_TYPE_NORMAL

- Magic (Magic): It's a MAGICAL attack type. It's used by spellcasters such as Priests, Witch Doctors, Necromancers etc. Spell Immune units ignore all damage with this AT, but Ethereal units don't. That's why spellcasters can hurt Ethereal units.

ATTACK_TYPE_MAGIC

2. Damage Types

There's a lot more of damage types but similar to ATs, they can be grouped into 4 categories:

- Normal Physical (Normal): Physical damage type which takes Defense Value into account. Basic attacks of almost all units in the game (except spellcasters) are AT Normal and DT Normal Physical.

DAMAGE_TYPE_NORMAL

- Enhanced Physical (Enhanced, Poison, Disease, Acid, Demolition, Slow Poison): Physical damage type which ignores Defense Value.

DAMAGE_TYPE_ENHANCED,

DAMAGE_TYPE_POISON,

DAMAGE_TYPE_DISEASE,

DAMAGE_TYPE_ACID,

DAMAGE_TYPE_DEMOLITION,

DAMAGE_TYPE_SLOWPOISON

- Universal (Unknown, Universal): Universal damage type which ignores Defense Value.

DAMAGE_TYPE_UNKNOWN,

DAMAGE_TYPE_UNIVERSAL

- Magical (The rest, including Fire, Cold, Lightning etc.): Magical damage type which ignores Defense Value. Will never affect Spell Immune units.

DAMAGE_TYPE_COLD,

DAMAGE_TYPE_DEATH,

DAMAGE_TYPE_DEFENSIVE,

DAMAGE_TYPE_DIVINE,

DAMAGE_TYPE_FIRE,

DAMAGE_TYPE_FORCE,

DAMAGE_TYPE_LIGHTNING,

DAMAGE_TYPE_MAGIC,

DAMAGE_TYPE_MIND,

DAMAGE_TYPE_PLANT,

DAMAGE_TYPE_SHADOWSTRIKE,

DAMAGE_TYPE_SONIC,

DAMAGE_TYPE_SPIRITLINK

Here's an excel table.

3. Some known examples

- Cleaving Attack (ANca, ACce): This spell takes AT from the basic attack of whichever unit it is present on, and has DT:Enhanced. On the Pit Lord, Cleaving Attack deals armor-ignoring damage to secondary targets in its AoE, including Spell immune targets, but gets reduced if the secondary target has Fortified Defense type.

- Death and Decay (AUdd): This spell has AT:Spells and DT:Universal. It can damage both Spell Immune units and Ethereal units, and gets reduced by Hero Defense type.

- If you played Dota in Warcraft 3, there's a hero called Beastmaster whose "Wild Axes" spell deals a damage that checks for both Defense type and Defense value on the target. It has AT:Spells and DT:Normal.

- Most of the "Pure" damage in Dota is actually AT:Hero (category Normal) and DT:Magic. That's why it affects neither Spell Immune nor Ethereal units, but always deals full damage otherwise.

4. Additional notes

- There should be no issue with dealing AT:Spells damage via Triggers; however, Blizzard abilities which deal spell immunity-ignoring AT:Spells damage will not affect on Spell immune units as long as the abilities' required level isn't greater than 1. It's hardcoded this way. For example, you can make a Death and Decay spell with 1 as minimum level requirement, and it will not damage Spell immune targets.

#### Attachments

3f38dc70a4[1].png

 18.1 KB

 · Views: 2,124




 Last edited: Mar 25, 2017

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

This is what I've always been looking for. But since the title says it's for beginners, it means I'm still a beginner after all these years. ;_;

Anyway, Nice tutorial. I finally know what "Pure" damage in DotA actually is.

#### Wareditor

#### Wareditor

 Level 15

JoinedJan 16, 2009Messages764

Resources: 4

#### Resources
Maps 4

This is exactly what I needed. Clear and precise tutorial!

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

pretty good and clean

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

That's a very useful tutorial.

Please attach the image directly to the post. We enforce this since it happened more than once already that other sites removed the images, and then the tutorial will be broken.

You're encouraged to use hive's bb code for lists and such, which will increase readability. It's all very useful info, but a bit boring to read.

You also can add an extra step into the structure.

Maybe:

Intro

AttackType (terminology)

DamageType (terminiology)

Damage Calculcation (new seperated point)

Detailed infos about all ATs

Detailed infos about all DTs

Your examples

Notes
Why you say "for beginners"? I think it's normal and good tutorial about the damage system.

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

IcemanBo said:

 That's a very useful tutorial.

Please attach the image directly to the post. We enforce this since it happened more than once already that other sites removed the images, and then the tutorial will be broken.

You're encouraged to use hive's bb code for lists and such, which will increase readability. It's all very useful info, but a bit boring to read.

You also can add an extra step into the structure.

Maybe:

Intro

AttackType (terminology)

DamageType (terminiology)

Damage Calculcation (new seperated point)

Detailed infos about all ATs

Detailed infos about all DTs

Your examples

Notes
Why you say "for beginners"? I think it's normal and good tutorial about the damage system.

Click to expand...

Changes have been made.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

dtnmang said:

 - There should be no issue with dealing AT:Spells damage via Triggers; however, Blizzard abilities which deal spell immunity-ignoring AT:Spells damage will not affect on Spell immune units as long as the abilities' required level isn't 6 at least. It's hardcoded this way. For example, you can make a Death and Decay spell with 1 as minimum level requirement, and it will not damage Spell immune targets.


Click to expand...

Is there a way to take note of what type a spell is? Like universal/physical. For example here, he based an ability on WarStomp, made it level 6, but had no luck that it deals out damage to magic immune units.

Also, I think it doesn't matter if it's required level is "6" or just ">1", like "2" would be fine, too.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Submission:Wc3's Damage System

Date:28 Nobember 2016

Status:Approved

Note:

I still would want us to talk about my post above, but it was actually only an additional info,

and the main tutorial looks good, so we might sort it out after approval slowly. Though I personaly find

the chosen green a bit aggresive for my eyes. I removed the "for beginners" from the title, it makes no real sense. Approved.

#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

Does this look correct to you?

 JASS:

library DamageFlags

struct Reduced_By_Defense_Value extends array
 static constant integer NO = 0
 static constant integer YES = 1
endstruct

struct Affects_Spell_Immune extends array
 static constant integer NO = 0
 static constant integer YES = 1
endstruct

struct Affects_Ethereal extends array
 static constant integer NO = 0
 static constant integer YES = 1
endstruct

struct Damage_Flags extends array
 static attacktype attack_type = null
 static damagetype damage_type = null

 private static attacktype array attack_types
 private static damagetype array damage_types

static if DEBUG_MODE then
 // we need this because enum types like "ATTACK_TYPE_NORMAL= ConvertAttackType(0)"
 // compare equal to null
 private static boolean array is_valid_combination
endif

 private static method onInit takes nothing returns nothing
 local integer i

 // NOTE: by default ATTACK_TYPE_NORMAL (Spells), is reduced by 25% by heroes
 // Gameplay Constants/Damage Bonus Table Spells/Hero: 0.75

 // NOTE: ethereal units take 66%? extra damage from spells and magic damage

 set i = (((Reduced_By_Defense_Value.NO) * 2 + Affects_Spell_Immune.NO) * 2 + Affects_Ethereal.NO)
 set attack_types[i] = ATTACK_TYPE_NORMAL
 set damage_types[i] = DAMAGE_TYPE_MAGIC
 set is_valid_combination[i] = true

 set i = (((Reduced_By_Defense_Value.NO) * 2 + Affects_Spell_Immune.NO) * 2 + Affects_Ethereal.YES)
 set attack_types[i] = ATTACK_TYPE_NORMAL
 set damage_types[i] = DAMAGE_TYPE_MAGIC
 set is_valid_combination[i] = true

 set i = (((Reduced_By_Defense_Value.NO) * 2 + Affects_Spell_Immune.YES) * 2 + Affects_Ethereal.NO)
 set attack_types[i] = ATTACK_TYPE_NORMAL
 set damage_types[i] = DAMAGE_TYPE_ENHANCED
 set is_valid_combination[i] = true

 set i = (((Reduced_By_Defense_Value.NO) * 2 + Affects_Spell_Immune.YES) * 2 + Affects_Ethereal.YES)
 set attack_types[i] = ATTACK_TYPE_NORMAL
 set damage_types[i] = DAMAGE_TYPE_UNIVERSAL
 set is_valid_combination[i] = true

 set i = (((Reduced_By_Defense_Value.YES) * 2 + Affects_Spell_Immune.NO) * 2 + Affects_Ethereal.NO)
 set attack_types[i] = null
 set damage_types[i] = null
 set is_valid_combination[i] = false

 set i = (((Reduced_By_Defense_Value.YES) * 2 + Affects_Spell_Immune.NO) * 2 + Affects_Ethereal.YES)
 set attack_types[i] = ATTACK_TYPE_MAGIC
 set damage_types[i] = DAMAGE_TYPE_NORMAL
 set is_valid_combination[i] = true

 set i = (((Reduced_By_Defense_Value.YES) * 2 + Affects_Spell_Immune.YES) * 2 + Affects_Ethereal.NO)
 set attack_types[i] = ATTACK_TYPE_NORMAL
 set damage_types[i] = DAMAGE_TYPE_NORMAL
 set is_valid_combination[i] = true

 set i = (((Reduced_By_Defense_Value.YES) * 2 + Affects_Spell_Immune.YES) * 2 + Affects_Ethereal.YES)
 set attack_types[i] = null
 set damage_types[i] = null
 set is_valid_combination[i] = false
 endmethod

 static method get takes Reduced_By_Defense_Value d, Affects_Spell_Immune s, Affects_Ethereal e returns nothing
 local integer i = (((d) * 2 + s) * 2 + e)

 set attack_type = attack_types[i]
 set damage_type = damage_types[i]

static if DEBUG_MODE then
 if not is_valid_combination[i] then
 call BJDebugMsg("|cffFF0000Damage_Flags.get: there is no combination of attack-type/damage-type that matches the request; SORRY!|r")
 if 1 / 0 == 1 then // stop execution
 endif
 endif
endif
 endmethod
endstruct

endlibrary

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

IcemanBo said:

 Is there a way to take note of what type a spell is? Like universal/physical. For example here, he based an ability on WarStomp, made it level 6, but had no luck that it deals out damage to magic immune units.

Also, I think it doesn't matter if it's required level is "6" or just ">1", like "2" would be fine, too.


Click to expand...

You are right, I'll fix that. Perhaps the only way to take note of what type a spell is through testing, but the only "Physical" spell that comes to my mind is Ensnare. War Stomp doesn't deal damage to magic immune units because its damage is (hardcoded) ATTACK_TYPE_NORMAL (AT:Spells), putting 6 as the required level only changes whether the stun effect should affect magic immune targets. He's better off using Triggers to set off the damage portion.

Aniki said:

 Does this look correct to you?

 JASS:

library DamageFlags

struct Reduced_By_Defense_Value extends array
 static constant integer NO = 0
 static constant integer YES = 1
endstruct

struct Affects_Spell_Immune extends array
 static constant integer NO = 0
 static constant integer YES = 1
endstruct

struct Affects_Ethereal extends array
 static constant integer NO = 0
 static constant integer YES = 1
endstruct

struct Damage_Flags extends array
 static attacktype attack_type = null
 static damagetype damage_type = null

 private static attacktype array attack_types
 private static damagetype array damage_types

static if DEBUG_MODE then
 // we need this because enum types like "ATTACK_TYPE_NORMAL= ConvertAttackType(0)"
 // compare equal to null
 private static boolean array is_valid_combination
endif

 private static method onInit takes nothing returns nothing
 local integer i

 // NOTE: by default ATTACK_TYPE_NORMAL (Spells), is reduced by 25% by heroes
 // Gameplay Constants/Damage Bonus Table Spells/Hero: 0.75

 // NOTE: ethereal units take 66%? extra damage from spells and magic damage

 set i = (((Reduced_By_Defense_Value.NO) * 2 + Affects_Spell_Immune.NO) * 2 + Affects_Ethereal.NO)
 set attack_types[i] = ATTACK_TYPE_NORMAL
 set damage_types[i] = DAMAGE_TYPE_MAGIC
 set is_valid_combination[i] = true

 set i = (((Reduced_By_Defense_Value.NO) * 2 + Affects_Spell_Immune.NO) * 2 + Affects_Ethereal.YES)
 set attack_types[i] = ATTACK_TYPE_NORMAL
 set damage_types[i] = DAMAGE_TYPE_MAGIC
 set is_valid_combination[i] = true

 set i = (((Reduced_By_Defense_Value.NO) * 2 + Affects_Spell_Immune.YES) * 2 + Affects_Ethereal.NO)
 set attack_types[i] = ATTACK_TYPE_NORMAL
 set damage_types[i] = DAMAGE_TYPE_ENHANCED
 set is_valid_combination[i] = true

 set i = (((Reduced_By_Defense_Value.NO) * 2 + Affects_Spell_Immune.YES) * 2 + Affects_Ethereal.YES)
 set attack_types[i] = ATTACK_TYPE_NORMAL
 set damage_types[i] = DAMAGE_TYPE_UNIVERSAL
 set is_valid_combination[i] = true

 set i = (((Reduced_By_Defense_Value.YES) * 2 + Affects_Spell_Immune.NO) * 2 + Affects_Ethereal.NO)
 set attack_types[i] = null
 set damage_types[i] = null
 set is_valid_combination[i] = false

 set i = (((Reduced_By_Defense_Value.YES) * 2 + Affects_Spell_Immune.NO) * 2 + Affects_Ethereal.YES)
 set attack_types[i] = ATTACK_TYPE_MAGIC
 set damage_types[i] = DAMAGE_TYPE_NORMAL
 set is_valid_combination[i] = true

 set i = (((Reduced_By_Defense_Value.YES) * 2 + Affects_Spell_Immune.YES) * 2 + Affects_Ethereal.NO)
 set attack_types[i] = ATTACK_TYPE_NORMAL
 set damage_types[i] = DAMAGE_TYPE_NORMAL
 set is_valid_combination[i] = true

 set i = (((Reduced_By_Defense_Value.YES) * 2 + Affects_Spell_Immune.YES) * 2 + Affects_Ethereal.YES)
 set attack_types[i] = null
 set damage_types[i] = null
 set is_valid_combination[i] = false
 endmethod

 static method get takes Reduced_By_Defense_Value d, Affects_Spell_Immune s, Affects_Ethereal e returns nothing
 local integer i = (((d) * 2 + s) * 2 + e)

 set attack_type = attack_types[i]
 set damage_type = damage_types[i]

static if DEBUG_MODE then
 if not is_valid_combination[i] then
 call BJDebugMsg("|cffFF0000Damage_Flags.get: there is no combination of attack-type/damage-type that matches the request; SORRY!|r")
 if 1 / 0 == 1 then // stop execution
 endif
 endif
endif
 endmethod
endstruct

endlibrary

Click to expand...

the jass values look correct to me, but I can't read jass anyway so I wouldn't be able to further tell if your code is correct

#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

 the jass values look correct to me, but I can't read jass anyway so I wouldn't be able to further tell if your code is correct

Click to expand...

Thanks for figuring WC3's attack/damage system in the first place =).

Anyway... here's a ridiculous function that calculates (at least it should) the damage, attack type and damage type that one needs in order to damage a unit (spell immune, ethereal) with exactly X damage (except heroes ;P):

 JASS:

static method get_damage takes unit u, real damage returns real
 local real result = damage
 local boolean is_spell_immune = IsUnitType(u, UNIT_TYPE_MAGIC_IMMUNE)
 local boolean is_ethereal = IsUnitType(u, UNIT_TYPE_ETHEREAL)

 // We assume that spell damage will do 100% to heroes, i.e
 // the value of Gameplay Constants/Damage Bonus Table Spells/Hero: 1.0
 // has been set to 1.0, and heroes don't have spell damage reducing items.

 if is_spell_immune and is_ethereal then
 call get(Reduced_By_Defense_Value.NO, Affects_Spell_Immune.YES, Affects_Ethereal.YES)

 // because of ethereal we have to scale down: 1 / 1.66 = 0.6024096
 // blizzard will scale the damage back up with 1.66 and we'll get our
 // original damage
 set result = result * 0.6024096

 elseif is_spell_immune then
 call get(Reduced_By_Defense_Value.NO, Affects_Spell_Immune.YES, Affects_Ethereal.NO)

 elseif is_ethereal then
 call get(Reduced_By_Defense_Value.NO, Affects_Spell_Immune.NO, Affects_Ethereal.YES)
 set result = result * 0.6024096

 else
 call get(Reduced_By_Defense_Value.NO, Affects_Spell_Immune.NO, Affects_Ethereal.NO)
 endif

 return result
endmethod

Example usage:

 JASS:

local unit u = ...
local unit target = ...
local real damage = 100.0

set damage = Damage_Flags.get_damage(target, damage)
call UnitDamageTarget(u, target, damage, false, false, Damage_Flags.attack_type, Damage_Flags.damage_type, WEAPON_TYPE_WHOKNOWS)

PS: WC3's attack/damage system is bananas =)...

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

nice work! just what I need!

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

This is not only a excellent topic to explain, but an excellently-written tutorial to do it.

Thanks!

#### BladeMaster3 (TVWK)

#### BladeMaster3 (TVWK)

 Level 5

JoinedApr 13, 2019Messages56

Resources: 4

#### Resources
Maps 4

hey friend i am a begginer and i cant find a tutorial on how to edit unit damage you know per exemple i know how to change the model of a footman but how can i change his attack like 14-19 to 17-24 you know

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Change the base value, dice and side per die. If I'm not mistaken...

Min: Base + Dice

Max: Base + (Dice*Die)

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Some things I'd like to add to this:

The mapping of ALL damage-dealing abilities to their respective damage types:

REPO in progress - mapping damage types to their abilities

The below table used by WarCraft 3 damage processing:



 1

Unit attacks or casts a spell

 → →



 2

The projectile or weapon hits the target unit

 → →



 3

EVENT_UNIT_DAMAGING is fired before any modifications to the damage are made.

 → ↓





 ↓ ←



 6

WarCraft 3 applies the value of Anti-Magic Shell, then Mana Shield, then Hardened Skin.

 ← ←



 5

WarCraft 3 runs EVENT_UNIT_DAMAGING and EVENT_UNIT_DAMAGED for potential defensive damage like Thorns Aura/spikes, then runs those events for the “Spirit Link” chain.

 ← ←



 4

WarCraft 3 processes any user damage made via BlzSetEventDamage.



 7

Attack type, defense type and armor value calculations are made, taking into consideration user changes made via things such as BlzSetEventDamage/Attack/WeaponType.

 → →



 8

WarCraft 3 runs EVENT_UNIT_DAMAGED based on the triggering instance of EVENT_UNIT_DAMAGING and any variables which were modified at the time.

 → →



 9

The audio of the weapontype and armortype of the source and target unit is played. If the weapontype and armortype are both 0, no sound is played. This can be manipulated via script to emulate a "missed" attack.

 → ↓





 √



 12

If there were no reincarnations found, process the sequence of events for death of a unit.

 ← ←



 11

The unit’s life is adjusted by the value of the damage event amount. If the damage was lethal, run any reincarnation abilities on the damaged unit.

 ← ←



 10

WarCraft 3 processes any final user damage made via BlzSetEventDamage.


 Last edited: Jul 4, 2019

#### Spellbound

#### Spellbound

 Level 33

JoinedJan 9, 2005Messages2,142

Resources: 25

#### Resources
Models 2Icons 1Skins 1Maps 4Spells and Systems 10User Interface 4JASS 2Wurst 1

Can you modify attack type on step 4 or is that a function that only works for EVENT_UNIT_DAMAGED?

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Spellbound said:

 Can you modify attack type on step 4 or is that a function that only works for EVENT_UNIT_DAMAGED?


Click to expand...

You can only use BlzSetEventAttack/Damage/WeaponType from a DAMAGING event (which fires before the DAMAGED event). That's why I moved DamageModifierEvent 1.00 to a DAMAGING event.

#### bruunk

#### bruunk

 Level 9

JoinedMar 17, 2016Messages156

Resources: 7

#### Resources
Icons 5Maps 2

thank you

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Based on my observation in BOEG REIMAGINED I am working on, Spiked Carapace and similar abilities seems to react to DT Normal, regardless of AT. Costs my Magician hero lots of self-killing to get even an idea of it happening, but I think I'll share for everyone to know.

#### Orphea

#### Orphea

 Level 6

JoinedApr 12, 2026Messages18

Resources: 2

#### Resources
Reforged HD Models 2

Bug catch: damage type shadow strike should be DAMAGE_TYPE_SHADOW_STRIKE

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Orphea said:

 Bug catch: damage type shadow strike should be DAMAGE_TYPE_SHADOW_STRIKE


Click to expand...

This guide is outdated. You'd want to use this:

### Thread 'Spell/Ability Damage Types and what they mean'

Jun 13, 2019

For information on cross-referencing the terminology used below, please review: Wc3C.net - View Single Post - ATTACK_TYPE and DAMAGE TYPE testing. Big shout out to @Tasyen for helping me to fill out this list and also providing good feedback.

Damage TypeUsed by AbilitiesBehavior

MagicAerial Shackles

 Mana Burn

 Finger of Death/Pain

 Life Drain

 Dispel/Devour Magic/Purge/Abolish Magic (Damage Summon)Cannot damage spell immune; bypasses Armor.

FireRain of Fire

 Flame Strike...

Bribe

Replies: 40

Forum: Advanced Scripting

 You must log in or register to reply here.


### Similar threads

Locked

 Attack Type & Damage Type


IsaacNewbton

Mar 9, 2012

World Editor Help Zone

Replies1Views7K

Mar 9, 2012
Maker

Locked

 need quick advice on Spell damage


Bond009

Nov 2, 2011

World Editor Help Zone

Replies11Views8K

Nov 3, 2011
Adiktuz

Locked

 Damage types are so confusing


best_player_88

Aug 20, 2020

World Editor Help Zone

Replies5Views2K

Aug 23, 2020
best_player_88

Locked

 Damage Type Attack type in triggers, where you use "UNit- Cause this unit to damage..


hdm

May 27, 2014

World Editor Help Zone

Replies4Views3K

May 27, 2014
Nichilus

Locked

[General]
 Question about damage type


Kusanagi Kuro

Jul 17, 2013

World Editor Help Zone

Replies12Views2K

Jul 17, 2013
defskull

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