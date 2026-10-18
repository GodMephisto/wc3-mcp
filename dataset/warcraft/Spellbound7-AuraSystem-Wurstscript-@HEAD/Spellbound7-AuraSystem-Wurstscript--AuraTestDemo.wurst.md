---
type: vendor-doc
source_repo: "https://github.com/Spellbound7/AuraSystem-Wurstscript-"
source_path: "AuraTestDemo.wurst"
ref: HEAD
---

package AuraTestDemo

import AuraSystem
import Assets
import DamageEvent
import HashMap
import ClosureTimers
import ClosureEvents
import BeamHandler
import ObjectIdGenerator
import AbilityObjEditing

/*
    Aura example 1 - Non-stacking

    Retribution Aura
    Units under the effect of this aura will retaliate against any damage inflicted on them by any
    enemy unit with a beam of light that deals 80% of the damage taken. This effect can only occur
    once every 2 seconds.
*/

constant RETRIBUTION_ID = compiletime(ABIL_ID_GEN.next())
constant RETRIBUTION_HASH = new HashMap<int, bool>
constant COOLDOWN_HASH = new HashMap<int, bool>
constant COOLDOWN_TIME = .1
constant BEAM_STRING = "FISB"

class RetributionAura extends Aura

    effect fx

    construct(unit source, real dist, bool isStack, int lvl)
        super(source, dist, isStack, lvl)
        this.fx = addEffect(Abilities.thornsAura, source, "origin")
        source.addAbility(RETRIBUTION_ID)

    override function onRemove()
        this.fx.destr()
        source.removeAbility(RETRIBUTION_ID)

    override function onTargetFilter(unit auraTarget) returns bool
        return not auraTarget.isType(UNIT_TYPE_STRUCTURE) and auraTarget.isAllyOf(this.source) and auraTarget.isAlive()

    override function onSourceFilter() returns bool
        return source.isAlive()

    override function onMute()
        this.fx.destr()
        source.disableAbility(RETRIBUTION_ID, true, false)

    override function onUnmute()
        this.fx = addEffect(Abilities.thornsAura, source, "origin")
        source.disableAbility(RETRIBUTION_ID, false, false)

    override function onPassFilter(unit filterUnit)
        new RetributionAuraBuff(this, filterUnit)
        
    override function checkType(AuraBuff auraBuff) returns bool
        return auraBuff instanceof RetributionAuraBuff

class RetributionAuraBuff extends AuraBuff

    effect fx

    construct(RetributionAura aura, unit u)
        super(aura, u)

    override function onApply()
        let u = target
        fx = u.addEffect(Abilities.faerieDragonMissile, "head")
        let id = u.getHandleId()
        RETRIBUTION_HASH.put(id, true)
        COOLDOWN_HASH.put(id, false)
        
    override function onRemove()
        fx.destr()
        let id = target.getHandleId()
        RETRIBUTION_HASH.remove(id)
        COOLDOWN_HASH.remove(id)

init
    DamageEvent.addListener() () ->
        let source = DamageEvent.getSource()
        let target = DamageEvent.getTarget()
        let id = target.getHandleId()
        if RETRIBUTION_HASH.has(id) and target.isEnemyOf(source) and DamageEvent.getType() != DamageType.CODE
            if not COOLDOWN_HASH.get(id)
                let beam = new Beam(BEAM_STRING, (80).v3(), target, target.getWidth(), true, (80).v3(), source, source.getWidth(), true, .1, .1, DEFAULT_BEAM_COLOUR)
                addEffect(Abilities.incinerateBuff, beam.impact)..setScale(.6)..destr()
                addEffect(Abilities.incinerateBuff, beam.launch)..setScale(.6)..destr()
                DamageEvent.setNextDamageFromCode()
                target.damageTarget(source, DamageEvent.getAmount() * .8)
                DamageEvent.setNextDamageType(DamageType.UNKNOWN)
                COOLDOWN_HASH.put(id, true)
                doAfter(COOLDOWN_TIME) () ->
                    if RETRIBUTION_HASH.has(id)
                        COOLDOWN_HASH.put(id, false)
    

/*
    Aura example 2 - Stacking

    Moribund Aura
    Deals 5 damage per second to enemy units. If a target under the effect of Moribund Aura attacks
    they take 5 damage. This Aura stacks.

*/

constant MORIBUND_ID = compiletime(ABIL_ID_GEN.next())
constant MORIBUND_HASH = new HashMap<int, bool>
constant MORIBUND_DOT = 5. * AURA_UPDATE_INTERVAL
constant MORIBUND_PROC_DMG = 10.

class MoribundAura extends Aura

    effect fx

    construct(unit source, real dist, bool isStack, int lvl)
        super(source, dist, isStack, lvl)
        this.fx = addEffect(Abilities.unholyAura, source, "origin")
        source.addAbility(MORIBUND_ID)

    override function onRemove()
        this.fx.destr()
        source.removeAbility(MORIBUND_ID)

    override function onTargetFilter(unit auraTarget) returns bool
        return not auraTarget.isType(UNIT_TYPE_STRUCTURE) and auraTarget.isEnemyOf(this.source) and auraTarget.isAlive()

    override function onSourceFilter() returns bool
        return source.isAlive()
        
    override function onMute()
        this.fx.destr()
        source.disableAbility(MORIBUND_ID, true, false)

    override function onUnmute()
        this.fx = addEffect(Abilities.unholyAura, source, "origin")
        source.disableAbility(MORIBUND_ID, false, false)

    override function onPassFilter(unit filterUnit)
        new MoribundAuraBuff(this, filterUnit)

    override function checkType(AuraBuff auraBuff) returns bool
        return auraBuff instanceof MoribundAuraBuff

class MoribundAuraBuff extends AuraBuff

    effect fx

    construct(MoribundAura aura, unit u)
        super(aura, u)

    override function onApply()
        fx = target.addEffect(Abilities.bansheeMissile, "chest")
        let id = target.getHandleId()
        if not MORIBUND_HASH.has(id)
            MORIBUND_HASH.put(id, true)
        
    override function onRemove()
        fx.destr()
        let u = target
        let i = MoribundAuraBuff.getStackCount(u, this.sourceAura)
        if i == 1
            MORIBUND_HASH.remove(u.getHandleId())

    // optional
    //override function onLevel(unit u)

    // optional
    override function onPeriod()
        let u = target
        u.addEffect(Abilities.bansheeMissile, "chest").destr()
        DamageEvent.setNextDamageFromCode()
        u.damageTarget(u, MORIBUND_DOT)
        DamageEvent.setNextDamageType(DamageType.UNKNOWN)

init
    EventListener.add(EVENT_PLAYER_UNIT_ATTACKED) ->
        let u = EventData.getTriggerUnit()
        if MORIBUND_HASH.has(u.getHandleId())
            addEffect(Abilities.illidanMissile, u, "chest").destr()
            DamageEvent.setNextDamageFromCode()
            u.damageTarget(u, MORIBUND_PROC_DMG)
            DamageEvent.setNextDamageType(DamageType.UNKNOWN)

init
    let dummyA = createUnit(Player(0), 'Hpal', vec2(-1000.,0.), bj_UNIT_FACING.fromDeg())
    new RetributionAura(dummyA, 900., false, 1)

    let dummyB = createUnit(Player(0), 'Ulic', vec2(1000.,0.), bj_UNIT_FACING.fromDeg())
    new MoribundAura(dummyB, 900., true, 1)
    
    let dummyC = createUnit(Player(0), 'Ulic', vec2(1000.,0.), bj_UNIT_FACING.fromDeg())
    new MoribundAura(dummyC, 900., true, 1)

@compiletime function gen()
    new AbilityDefinition(RETRIBUTION_ID, AbilityIds.moonGlaive)
    ..setIconNormal(Icons.pASBTNReincarnation)
    ..setButtonPositionNormalX(1)
    ..setButtonPositionNormalY(1)
    ..setTooltipNormal(1, "Retribution Aura")
    ..setTooltipNormalExtended(1, "Units under the effect of this aura will retaliate against any damage inflicted on them by any " +
    "enemy unit with a beam of light that deals 80% of the damage taken. This effect can only occur" +
    "once every 0.1 seconds.")
    ..setCheckDependencies(false)

    new AbilityDefinition(MORIBUND_ID, AbilityIds.moonGlaive)
    ..setIconNormal(Icons.pASBTNRegenerationAura)
    ..setButtonPositionNormalX(1)
    ..setButtonPositionNormalY(1)
    ..setTooltipNormal(1, "Moribund Aura")
    ..setTooltipNormalExtended(1, "Deals 5 damage per second to enemy units. If a target under the effect of Moribund Aura attacks " +
    "they take 5 damage. This Aura stacks.")
    ..setCheckDependencies(false)
    
