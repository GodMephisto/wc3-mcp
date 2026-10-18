---
type: vendor-doc
source_repo: "https://github.com/Spellbound7/AuraSystem-Wurstscript-"
source_path: "AuraSystem_v1.01.wurst"
ref: HEAD
---

package AuraSystem

/*
    v1.01

    AuraSystem will provide you with a framework for creating your own Auras, with the ability to retrieve the source of the aura
    as well as make them stack, level, etc.
*/

import UnitIndexer
import TimerUtils
import LinkedList
import HashMap

public constant AURA_UPDATE_INTERVAL = .33

constant GLOBAL_AURA_LIST = new LinkedList<Aura>
constant GLOBAL_AURA_BUFF_LIST = new LinkedList<AuraBuff>
constant UNIT_AURA_LIST_HASH = new HashMap<int, LinkedList<Aura>>
constant UNIT_AURA_BUFF_LIST_HASH = new HashMap<int, LinkedList<AuraBuff>>
constant FILTER_GROUP = CreateGroup()
timer UPDATE_CLOCK

function onAuraPeriodGlobal()
    for aura in GLOBAL_AURA_LIST
        if not aura.onSourceFilter()
            if not aura.isMute
                aura.onMute()
                aura.isMute = true
                for auraBuff in aura.childList
                    auraBuff.removeSource(aura)
        else
            if aura.isMute
                aura.onUnmute()
                aura.isMute = false
            let sourcePos = aura.source.getPos()
            for auraBuff in aura.childList
                let u = auraBuff.target
                if u == null // if u was removed from the game
                    auraBuff.removeSource(aura)
                else
                    FILTER_GROUP.addUnit(u)
                    if not aura.onTargetFilter(u) or not u.isInRange(sourcePos, aura.range)
                        auraBuff.removeSource(aura)
                    else
                        if auraBuff.sourceAura == aura
                            if auraBuff.level != aura.level
                                auraBuff.onLevel()
                            auraBuff.onPeriod()
                
            ENUM_GROUP.enumUnitsInRange(sourcePos, aura.range + MAX_COLLISION_SIZE)
            ENUM_GROUP.removeGroup(FILTER_GROUP)
            FILTER_GROUP.clear()
            for u from ENUM_GROUP
                if u.isInRange(sourcePos, aura.range)
                    if aura.onTargetFilter(u)
                        if aura.isStacking
                            //new aura.typeCheck.typeId(aura, u) maybe?
                            aura.onPassFilter(u)
                        else
                            let auraBuff = AuraBuff.get(u, aura)
                            if auraBuff == null
                                aura.onPassFilter(u)
                            else
                                auraBuff.addSource(aura)

            aura.onPeriod()


/** Aura class. The Aura is the carrier for the AuraBuff. */
public abstract class Aura
    
    protected unit source
    protected real range
    protected bool isStacking
    protected int level
    protected bool isMute
    protected LinkedList<AuraBuff> childList
    protected AuraBuff typeCheck
    
    construct(unit u, real dist, bool stack, int lvl)//, AuraBuff ref)
        let id = u.getHandleId()
        bool duplicateAura

        var auraList = UNIT_AURA_LIST_HASH.get(id)
        if auraList == null
            duplicateAura = false
            auraList = new LinkedList<Aura>
            UNIT_AURA_LIST_HASH.put(id, auraList)
        else
            duplicateAura = Aura.has(auraList, this)

        if not duplicateAura
            auraList.push(this)
            source = u
            range = dist
            isStacking = stack
            childList = new LinkedList<AuraBuff>
            level = lvl
            //typeCheck = ref

            GLOBAL_AURA_LIST.push(this)
            if GLOBAL_AURA_LIST.size() == 1
                UPDATE_CLOCK = getTimer()
                UPDATE_CLOCK.startPeriodic(AURA_UPDATE_INTERVAL, function onAuraPeriodGlobal)
        else
            destroy this

    ondestroy

    /** Use this instead of destroy to remove an aura from a unit. This automatically deals with all AuraBuffs associated with this Aura instance. */
    function remove()
        onRemove()
        for auraBuff in childList
            auraBuff.removeSource(this)
        destroy childList
        childList = null

        let id = source.getHandleId()
        let auraList = UNIT_AURA_LIST_HASH.get(id)
        auraList.remove(this)
        if auraList.isEmpty()
            destroy auraList
            UNIT_AURA_LIST_HASH.remove(id)

        source = null
        range = 0.
        level  = 0
        isMute = false

        GLOBAL_AURA_LIST.remove(this)
        if GLOBAL_AURA_LIST.isEmpty()
            UPDATE_CLOCK.release()

        destroy this

    function updateBuffLevels()
        for auraBuff in childList
            auraBuff.rearrangeLevels()

    /** destroys all Aura instances stored to a unit. This calls removeSource() on any AuraBuffs maintained by these Auras. */
    static function clearAll(unit u)
        let id = u.getHandleId()
        let auraList = UNIT_AURA_LIST_HASH.get(id)
        if auraList != null
            for aura in auraList
                aura.remove()

    /*static function get(unit u, Aura aura) returns thistype
        let id = u.getHandleId()
        let auraList = UNIT_AURA_LIST_HASH.get(id)
        if auraList != null
            for _aura in auraList
                if _aura == aura
                    return aura
        return null*/

    protected static function has(LinkedList<Aura> auraList, Aura aura) returns bool
        for _aura in auraList
            if _aura == aura
                return true
        return false
        
    // Obligatory functions
    abstract function onTargetFilter(unit auraTarget) returns boolean
    abstract function onSourceFilter() returns boolean
    abstract function onPassFilter(unit filterUnit)
    abstract function checkType(AuraBuff auraBuff) returns boolean

    // Optional
    function onRemove()
    function onMute()
    function onUnmute()
    function onPeriod()
    function onLevel()


public abstract class AuraBuff

    protected unit target
    protected int level
    protected Aura sourceAura // for non-stacking auras, this also serves as the primary aura source.
    private LinkedList<Aura> sourceAuraList

    construct(Aura aura, unit u)
        let id = u.getHandleId()

        var auraBuffList = UNIT_AURA_BUFF_LIST_HASH.get(id)
        if auraBuffList == null    
            auraBuffList = new LinkedList<AuraBuff>
            UNIT_AURA_BUFF_LIST_HASH.put(id, auraBuffList)
        auraBuffList.push(this)

        if not aura.isStacking
            sourceAuraList = new LinkedList<Aura>
            sourceAuraList.push(aura)
        sourceAura = aura
        aura.childList.push(this)
        target = u
        level = aura.level
        GLOBAL_AURA_BUFF_LIST.push(this)

        this.onApply()

    //construct()

    ondestroy
        this.onRemove()
        let id = target.getHandleId()
        let auraBuffList = UNIT_AURA_BUFF_LIST_HASH.get(id)
        auraBuffList.remove(this)
        if auraBuffList.isEmpty()
            destroy auraBuffList
            UNIT_AURA_BUFF_LIST_HASH.remove(id)
        if sourceAuraList != null
            destroy sourceAuraList
            sourceAuraList = null
        sourceAura.childList.remove(this)
        sourceAura = null
        target = null
        GLOBAL_AURA_BUFF_LIST.remove(this)

    protected function rearrangeLevels()
        if sourceAuraList != null
            var lvl = 0
            for aura in this.sourceAuraList
                if aura.level > lvl
                    lvl = aura.level
                    this.sourceAura = aura
            if this.level != lvl
                this.level = lvl
                this.onLevel()

    protected function addSource(Aura aura)
        if sourceAuraList != null // is sourceAuraList exists, then the aura is non-stacking
            if not sourceAuraList.has(aura)
                sourceAuraList.push(aura)
                aura.childList.push(this)
                if sourceAuraList.size() > 1
                    rearrangeLevels()

    protected function removeSource(Aura aura)
        if sourceAuraList == null
            if sourceAura == aura
                destroy this
        else
            if sourceAuraList.has(aura)
                sourceAuraList.remove(aura)
                aura.childList.remove(this)
            if sourceAuraList.isEmpty()
                destroy this
            else
                rearrangeLevels()

    protected static function clearAll(unit u)
        let id = u.getHandleId()
        let auraBuffList = UNIT_AURA_BUFF_LIST_HASH.get(id)
        if auraBuffList != null
            for auraBuff in auraBuffList
                if auraBuff.sourceAuraList != null
                    for aura in auraBuff.sourceAuraList
                        auraBuff.removeSource(aura)
                else
                    destroy auraBuff

    protected static function has(unit u, Aura aura) returns bool
        let id = u.getHandleId()
        let auraBuffList = UNIT_AURA_BUFF_LIST_HASH.get(id)
        if auraBuffList != null
            for auraBuff in auraBuffList
                if auraBuff.sourceAura == aura
                    return true
        return false

    protected static function get(unit u, Aura aura) returns thistype
        let id = u.getHandleId()
        let auraBuffList = UNIT_AURA_BUFF_LIST_HASH.get(id)
        if auraBuffList != null
            for auraBuff in auraBuffList
                if aura.checkType(auraBuff)
                //if auraBuff.typeId == aura.typeCheck.typeId
                    return auraBuff
        return null

    protected static function getStackCount(unit u, Aura aura) returns int
        let id = u.getHandleId()
        var i = 0
        let auraBuffList = UNIT_AURA_BUFF_LIST_HASH.get(id)
        if auraBuffList != null
            for auraBuff in auraBuffList
                if aura.checkType(auraBuff)
                //if auraBuff.typeId == aura.typeCheck.typeId
                    i++
        return i

    protected static function getSourceCount(unit u, Aura aura) returns int
        let id = u.getHandleId()
        let auraBuffList = UNIT_AURA_BUFF_LIST_HASH.get(id)
        if auraBuffList != null
            for auraBuff in auraBuffList
                if aura.checkType(auraBuff)
                //if auraBuff.typeId == aura.typeCheck.typeId
                    return auraBuff.sourceAuraList.size()
        return 0

    // public getters
    static function getBuffList(int id) returns LinkedList<AuraBuff>
        return UNIT_AURA_BUFF_LIST_HASH.get(id)

    function getSourceAura() returns Aura
        return this.sourceAura

    function getSourceAuraSource() returns unit
        return this.sourceAura.source

    // Obligatory functions
    abstract function onApply()
    abstract function onRemove()

    // Optional functions
    function onLevel()
    function onPeriod()

init
    onUnitDeindex() ->
        let u = getIndexingUnit()
        AuraBuff.clearAll(u)
        Aura.clearAll(u)
