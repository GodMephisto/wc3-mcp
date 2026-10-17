---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/efficient-spell-cloning.292751/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Efficient Spell Cloning
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/efficient-spell-cloning.292751/>
#### AGD

#### AGD

 Level 17

JoinedMar 29, 2016Messages688

Resources: 30

#### Resources
Spells and Systems 13JASS 12Lua 4Tutorials 1

# Efficient Spell Cloning

Tutorial Outline:

Rationale

Global Configuration

Local Configuration

Local Configuration API

Core Code

Example

### Rationale
Many of you might be familiar with some custom games like Nevermore Wars (aka Shadowraze Wars), Mirana Wars, Pudge Wars, Axe Wars and many similar maps. The concept of these maps, for those who didn't know, is that all players posses the same hero, with the same stats and abilities. The goal of the game is to have the highest number of kills (individually or by team depending on the game mode). But the thing that make such games fun is the many different spell variants which can be acquired through buying ability-upgrading items. If you're not familiar with the above mentioned maps, I suggest you to check a sample from them first (I recommend Shadowraze Wars) so that you will have a much clearer understanding of the tutorial.

In this tutorial, I will show you an easy and efficient way of doing just this - making many spell variations out of a single spell mechanics. So if you're making a map with a similar concept to the above mentioned maps, this might be a fitting tutorial for you.

When you want to create another variant of the spell, what would you do? Copy-paste the whole code and then change some values in the configuration, right? This is the easiest and fastest way to do it. And of course you're right. But this method is not so efficient as it would multiply your spell's script size n times (where n is the number of spell variants).

So this is what we're going to do instead - a single script for the main spell mechanics, and a separate local configuration set of each spell variant. For this, I would like to indorse to you the SpellEffectEvent library made by Bribe. It would make our example code later on much shorter and less convoluted.

The trick behind this spell cloning technique is that we need to divide the configuration into two parts namely, the global configuration and the local configuration. We will discuss about the global configuration first.

### Global Configuration
Global configuration is a set of configurable values that apply to all spell instances. It doesn't matter it the spell instances are of different variety, as long as they belong to the same spell mechanic. To give you can example, consider a Blackhole spell. It can have many variety, with each variety having different model, special effects, as well as duration, damage, etc. but they all follow a single spell mechanic. Therefore, the global configuration should apply to all these varieties.

The usual things to include in the global configuration are: The periodic timeout, dummy rawcodes, and the dummy unit owner.

### Local Configuration
The next type of configuration is the local configuration. Here, as the term suggests, it is a set of configurable values that only apply for a specific spell variety. Therefore, this is where you ought to put all the other configurable values that do not belong to the global configuration. These values are stored in an array variable or a struct member so that we can retrieve the correct local configuration later on through an index based on the casted spell.

### Local Configuration API
Another thing we need is an API for the users to declare a new spell variety. Actually, to do so, they only need to declare a new local configuration so we must provide an API for them. We should remember that each spell variety corresponds to a different activation spell, thus, we require a user to input the rawcode of the spell when declaring a new local configuration. There are many ways to do this but I'll provide an example.

 JASS:

struct Spell
 private static thistype varietyCount = 0
 // This is your spell
 //...

 static method newVariety takes integer spellId returns thistype
 set varietyCount = varietyCount + 1
 call SaveInteger(hashtable, 0, spellId, varietyCount)
 call RegisterSpellEffectEvent(spellId, function thistype.onCast) //Make sure to register the input spellId as an activation spell
 return varietyCount
 endmethod
endstruct

### Core Code
The code for the main spell mechanics is not so different compared to one where you wouldn't apply this kind of technique. We only have to make sure we'll be able to do these things:

1. Determine the 'variety index' of the casted spell

2. Attach the 'variety index' to the spell instance

### Example
Now let's proceed with an actual example. For simplicity, I chose the Shadowraze spell from the map Shadowraze Wars as an example.

Normally when you write a spell, you do something like this and you put the configuration to constant variables or sometimes constant functions for a better dynamic scaling of the values.

Shadowraze

 JASS:

library Shadowraze /*

 */uses /*

 */SpellEffectEvent

 private module ShadowrazeConfiguration
 /*
 Rawcode of the activation spell (Preferably a 'No order target'
 spell) */
 static constant integer SPELL_ID = 'Asdr'
 /*
 Rawcode of the dummy unit */
 static constant integer DUMMY_ID = 'dumi'
 /*
 Owner of the dummy unit */
 static constant player DUMMY_OWNER = Player(14)
 /*
 Duration of the dummy unit (Make sure to accommodate for the
 special effect's death animation within this value) */
 static constant real DUMMY_DURATION = 2.00
 /*
 Impact damage */
 static constant real DAMAGE_BASE = 0.00
 static constant real DAMAGE_PER_LEVEL = 75.00
 /*
 Radius of the affected area */
 static constant real RADIUS_BASE = 200.00
 static constant real RADIUS_PER_LEVEL = 0.00
 /*
 Distance of the shadowraze from the caster */
 static constant real DISTANCE_BASE = 200.00
 static constant real DISTANCE_PER_LEVEL = 0.00
 /*
 Impact delay */
 static constant real IMPACT_DELAY_BASE = 0.00
 static constant real IMPACT_DELAY_PER_LEVEL = 0.00
 /*
 Spell's attacktype and damagetype */
 static constant attacktype ATTACK_TYPE = ATTACK_TYPE_HERO
 static constant damagetype DAMAGE_TYPE = DAMAGE_TYPE_NORMAL
 /*
 Shadowraze impact special effect model */
 static constant string IMPACT_SFX_MODEL = "Abilities\\Spells\\Undead\\AnimateDead\\AnimateDeadTarget.mdl"

 endmodule

 /*=========================== Spell Core ===========================*/

 private struct Shadowraze

 implement ShadowrazeConfiguration

 private static hashtable table = InitHashtable()
 private static group enumGroup = CreateGroup()
 private static unit tempUnit
 private integer level
 private unit caster
 private real targetX
 private real targetY

 method destroy takes nothing returns nothing
 call .deallocate()
 set .caster = null
 // Not necessary though
 set .level = 0
 set .targetX = 0.00
 set .targetY = 0.00
 endmethod

 private static method onImpact takes nothing returns nothing
 local timer expired = GetExpiredTimer()
 local thistype this = LoadInteger(table, 0, GetHandleId(expired))
 local integer level = .level
 local unit caster = .caster
 local real damageAmount = DAMAGE_BASE + DAMAGE_PER_LEVEL*level
 call GroupEnumUnitsInRange(enumGroup, .targetX, .targetY, .radius, null)
 loop
 set tempUnit = FirstOfGroup(enumGroup)
 exitwhen tempUnit == null
 call GroupRemoveUnit(enumGroup, tempUnit)
 call UnitDamageTarget(caster, tempUnit, damageAmount, true, false, ATTACK_TYPE, DAMAGE_TYPE, null)
 endloop
 call DestroyTimer(expired)
 call .destroy()
 set expired = null
 endmethod

 private static method onCast takes nothing returns nothing
 local thistype this = allocate()
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, SPELL_ID)
 local real distance = DISTANCE_BASE + DISTANCE_PER_LEVEL*level
 local real radius = RADIUS_BASE + RADIUS_PER_LEVEL*level
 local real angle = GetUnitFacing(caster)*bj_DEGTORAD
 local real casterX = GetUnitX(caster)
 local real casterY = GetUnitY(caster)
 local real targetX = casterX + distance*Cos(angle)
 local real targetY = casterY + distance*Sin(angle)
 local timer delayTimer = CreateTimer()
 local unit dummy = CreateUnit(DUMMY_OWNER, DUMMY_ID, targetX, targetY, angle)
 call SetUnitScale(dummy, radius*0.01, 0, 0)
 call DestroyEffect(AddSpecialEffectTarget(IMPACT_SFX_MODEL, dummy, "origin"))
 call UnitApplyTimedLife(dummy, 'BTLF', DUMMY_DURATION)
 set .caster = caster
 set .targetX = targetX
 set .targetY = targetY
 set .radius = radius
 set .level = level
 call SaveInteger(table, 0, GetHandleId(delayTimer), this)
 call TimerStart(delayTimer, IMPACT_DELAY_BASE + IMPACT_DELAY_PER_LEVEL*level, false, function thistype.onImpact)
 set caster = null
 set dummy = null
 set delayTimer = null
 endmethod

 private static method onInit takes nothing returns nothing
 call RegisterSpellEffectEvent(SPELL_ID, function thistype.onCast)
 endmethod

 endstruct

endlibrary

The above code is the basic format of the spell. Now, let's convert it using the said technique.

First, you need to provide global configurations. I’ll just put it in a module for better organization, but it depends on you.

 JASS:

 module ShadowrazeConfiguration
 /*
 Rawcode of the dummy unit */
 static constant integer DUMMY_ID = 'dumi'
 /*
 Owner of the dummy unit */
 static constant player DUMMY_OWNER = Player(14)
 /*
 Duration of the dummy unit (Make sure to accommodate for the
 special effect's death animation within this value) */
 static constant real DUMMY_DURATION = 2.00
 endmodule

 /*=========================== Spell Core ===========================*/

 struct Shadowraze

 implement ShadowrazeConfiguration

 // Other stuffs…

 endstruct

Next, you need an interface for you to declare local configurations. You also need to prepare the variables for storing the configuration values but unlike usual, the variables must be array or instance struct members and they must be fully public so you can set their values from outside the main code.

 JASS:

struct Shadowraze

 private static hashtable table = InitHashtable()
 private static thistype configurationCount

 // Configuration vars
 real DAMAGE_BASE
 real DAMAGE_PER_LEVEL
 real RADIUS_BASE
 real RADIUS_PER_LEVEL
 real DISTANCE_BASE
 real DISTANCE_PER_LEVEL
 real IMPACT_DELAY_BASE
 real IMPACT_DELAY_PER_LEVEL
 attacktype ATTACK_TYPE
 damagetype DAMAGE_TYPE
 string IMPACT_SFX_MODEL

 // Spell codes…

 // This method counts the number of spell variants defined by the user. Then
 // it saves the configuration id of the spell’s rawcode and registers it to the SpellEffectEvent
 // library.
 static method new takes integer spellId returns thistype
 set configurationCount = configurationCount + 1
 call SaveInteger(table, 0, spellId, configurationCount)
 call RegisterSpellEffectEvent(rawcode, function thistype.onCast)
 return configurationCount
 endmethod

endstruct

Now, let’s proceed to the spell’s core code. The next thing to do is to catch the configuration id of the spell being cast.

 JASS:

private static method onCast takes nothing returns nothing
 local integer spellId = GetSpellAbilityId()
 local thistype id = LoadInteger(table, 0, spellId)
 // Do on spell cast actions here
 // We can then use ‘id’ to refer to the index of our configuration variables
 local real var = id.MY_VAR
 local real otherVar = id.OTHER_VAR
 // etc.
endmethod

In case you have a periodic method/function, you can just attach the configuration id to the current spell instance then attach that spell instance to the timer, or you could just directly attach the configuration id to the timer. But I suggest the former method because it allows you to have a static timer that loops through all the current spell instances and just retrieve the configuration id attached to the spell instance.

So our Shadowraze spell following the new format would look like this:

 JASS:

library Shadowraze /*

 */uses /*

 */SpellEffectEvent

 private module ShadowrazeConfiguration
 /*
 Rawcode of the dummy unit */
 static constant integer DUMMY_ID = 'dumi'
 /*
 Owner of the dummy unit */
 static constant player DUMMY_OWNER = Player(14)
 /*
 Duration of the dummy unit (Make sure to accommodate for the
 special effect's death animation within this value) */
 static constant real DUMMY_DURATION = 2.00

 endmodule

 /*=========================== Spell Core ===========================*/

 struct Shadowraze

 implement ShadowrazeConfiguration

 private static hashtable table = InitHashtable()
 private static group enumGroup = CreateGroup()
 private static unit tempUnit
 private static thistype configurationCount = 0
 private thistype configurationId
 private integer level
 private unit caster
 private real targetX
 private real targetY
 private real radius

 real DAMAGE_BASE
 real DAMAGE_PER_LEVEL
 real RADIUS_BASE
 real RADIUS_PER_LEVEL
 real DISTANCE_BASE
 real DISTANCE_PER_LEVEL
 real IMPACT_DELAY_BASE
 real IMPACT_DELAY_PER_LEVEL
 attacktype ATTACK_TYPE
 damagetype DAMAGE_TYPE
 string IMPACT_SFX_MODEL

 private method destroy takes nothing returns nothing
 call .deallocate()
 set .caster = null
 // Not necessary though
 set .level = 0
 set .targetX = 0.00
 set .targetY = 0.00
 set .radius = 0.00
 endmethod

 private static method onImpact takes nothing returns nothing
 local timer expired = GetExpiredTimer()
 local thistype this = LoadInteger(table, 0, GetHandleId(expired))
 local thistype id = .configurationId
 local integer level = .level
 local unit caster = .caster
 local real damageAmount = id.DAMAGE_BASE + id.DAMAGE_PER_LEVEL*level
 call GroupEnumUnitsInRange(enumGroup, .targetX, .targetY, .radius, null)
 loop
 set tempUnit = FirstOfGroup(enumGroup)
 exitwhen tempUnit == null
 call GroupRemoveUnit(enumGroup, tempUnit)
 if tempUnit != caster then
 call UnitDamageTarget(caster, tempUnit, damageAmount, true, false, id.ATTACK_TYPE, id.DAMAGE_TYPE, null)
 endif
 endloop
 call DestroyTimer(expired)
 call .destroy()
 set expired = null
 endmethod

 private static method onCast takes nothing returns nothing
 local thistype this = allocate()
 local integer spellId = GetSpellAbilityId()
 local thistype id = LoadInteger(table, 0, spellId)
 local unit caster = GetTriggerUnit()
 local integer level = GetUnitAbilityLevel(caster, spellId)
 local real distance = id.DISTANCE_BASE + id.DISTANCE_PER_LEVEL*level
 local real radius = id.RADIUS_BASE + id.RADIUS_PER_LEVEL*level
 local real angle = GetUnitFacing(caster)*bj_DEGTORAD
 local real casterX = GetUnitX(caster)
 local real casterY = GetUnitY(caster)
 local real targetX = casterX + distance*Cos(angle)
 local real targetY = casterY + distance*Sin(angle)
 local timer delayTimer = CreateTimer()
 local unit dummy = CreateUnit(DUMMY_OWNER, DUMMY_ID, targetX, targetY, angle)
 call SetUnitScale(dummy, radius*0.01, 0, 0)
 call DestroyEffect(AddSpecialEffectTarget(id.IMPACT_SFX_MODEL, dummy, "origin"))
 call UnitApplyTimedLife(dummy, 'BTLF', DUMMY_DURATION)
 set .configurationId = id
 set .caster = caster
 set .targetX = targetX
 set .targetY = targetY
 set .radius = radius
 set .level = level
 call SaveInteger(table, 0, GetHandleId(delayTimer), this)
 call TimerStart(delayTimer, id.IMPACT_DELAY_BASE + id.IMPACT_DELAY_PER_LEVEL*level, false, function thistype.onImpact)
 set caster = null
 set dummy = null
 set delayTimer = null
 endmethod

 static method new takes integer spellId returns thistype
 set configurationCount = configurationCount + 1
 call SaveInteger(table, 0, spellId, configurationCount)
 call RegisterSpellEffectEvent(spellId, function thistype.onCast)
 return configurationCount
 endmethod

 endstruct

endlibrary

You're done with the spell core and the last thing you need to do is to setup the local configuration.

By using the interface we provided above, we create another library in where we setup our local configuration:

 JASS:

library ShadowrazeDefaultConfiguration initiallizer Init uses Shadowraze

 private function Init takes nothing returns nothing
 local Shadowraze this = Shadowraze.new('Asdr')
 /*
 Impact damage */
 set this.DAMAGE_BASE = 0.00
 set this.DAMAGE_PER_LEVEL = 75.00
 /*
 Radius of the affected area */
 set this.RADIUS_BASE = 125.00
 set this.RADIUS_PER_LEVEL = 0.00
 /*
 Distance of the shadowraze from the caster */
 set this.DISTANCE_BASE = 500.00
 set this.DISTANCE_PER_LEVEL = 0.00
 /*
 Impact delay */
 set this.IMPACT_DELAY_BASE = 0.00
 set this.IMPACT_DELAY_PER_LEVEL = 0.00
 /*
 Spell's attacktype and damagetype */
 set this.ATTACK_TYPE = ATTACK_TYPE_MAGIC
 set this.DAMAGE_TYPE = DAMAGE_TYPE_MAGIC
 /*
 Shadowraze impact special effect model */
 set this.IMPACT_SFX_MODEL = "Abilities\\Spells\\Undead\\AnimateDead\\AnimateDeadTarget.mdl"
 endfunction

endlibrary

Or we can even an additional interface for us. At the bottom of the main spell library, we provide a textmacro. Something like this:

 JASS:

library Shadowraze

 //...

 //! textmacro SHADOWRAZE_CONFIGURATION takes SPELL_ID
 struct Shadowraze_Configuration_Set_$SPELL_ID$ extends array
 private static method onInit takes nothing returns nothing
 local Shadowraze this = Shadowraze.new('$SPELL_ID$')
 //! endtextmacro

 //! textmacro END_SHADOWRAZE_CONFIGURATION
 endmethod
 endstruct
 //! endtextmacro

endlibrary

With that above, our configuration would finally be like this:

 JASS:

library ShadowrazeDefaultConfig requires Shadowraze

 //! runtextmacro SHADOWRAZE_CONFIGURATION("Asdr")
 /*
 Impact damage */
 set this.DAMAGE_BASE = 0.00
 set this.DAMAGE_PER_LEVEL = 75.00
 /*
 Radius of the affected area */
 set this.RADIUS_BASE = 125.00
 set this.RADIUS_PER_LEVEL = 0.00
 /*
 Distance of the shadowraze from the caster */
 set this.DISTANCE_BASE = 500.00
 set this.DISTANCE_PER_LEVEL = 0.00
 /*
 Impact delay */
 set this.IMPACT_DELAY_BASE = 0.00
 set this.IMPACT_DELAY_PER_LEVEL = 0.00
 /*
 Spell's attacktype and damagetype */
 set this.ATTACK_TYPE = ATTACK_TYPE_MAGIC
 set this.DAMAGE_TYPE = DAMAGE_TYPE_MAGIC
 /*
 Shadowraze impact special effect model */
 set this.IMPACT_SFX_MODEL = "Abilities\\Spells\\Undead\\AnimateDead\\AnimateDeadTarget.mdl"
 //! runtextmacro END_SHADOWRAZE_CONFIGURATION()

endlibrary

A little bit more concise and also comes with an extra advantage, i.e., when you run the textmacro and input the same spell rawcode more than once, it will throw an error, thus, saving us from collisions.

Below, is an attached map with the example we made here and a template for spell cloning.

That's all. Have fun spell cloning!

Btw, if you want a full-length finished product using this technique, check this resource -> TorrentArray.

#### Attachments

Spell Cloning [Tutorial Sample].w3x

 35.6 KB

 · Views: 292




 Last edited: Mar 20, 2017

#### AGD

#### AGD

 Level 17

JoinedMar 29, 2016Messages688

Resources: 30

#### Resources
Spells and Systems 13JASS 12Lua 4Tutorials 1

This currently tackles cloning active spells. Maybe I'll include passive spells soon.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Useful, but the requirement of Jass knowledge can be an overhead.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Awesome. Really good insight on making your codes modular and configurable dynamically. I would love to try this out.

Approved!

 You must log in or register to reply here.


### Similar threads

 vJASS Spell Templates


Magtheridon96

May 13, 2012

JASS Tutorials

Replies32Views19K

Mar 29, 2016
PurgeandFire

Locked

[vJASS]
 multiple structs in one trigger usage


kawas11

Jun 9, 2015

Triggers & Scripts

Replies1Views1K

Jun 10, 2015
PurgeandFire

[Spell]
 Problem with dummy caster in a JASS-spell.


Dinodin

Oct 11, 2025

World Editor Help Zone

Replies3Views553

Oct 13, 2025
Uncle

Locked

[vJASS]
 Spell Efficiency


pks9067

Jun 5, 2011

Triggers & Scripts

Replies5Views2K

Jun 8, 2011
pks9067

Locked

[Solved]
 Transferring variable type to another function


maxodors

Jan 29, 2022

World Editor Help Zone

Replies6Views2K

Feb 9, 2022
maxodors

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

JASS Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top