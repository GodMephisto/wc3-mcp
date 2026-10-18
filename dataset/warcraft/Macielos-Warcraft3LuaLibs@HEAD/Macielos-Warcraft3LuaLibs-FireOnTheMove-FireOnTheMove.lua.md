---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "FireOnTheMove/FireOnTheMove.lua"
ref: HEAD
---

if Debug then Debug.beginFile "FireOnTheMove" end
--MUST BE PUT AFTER UNIT TRACKER
do
    TrackedUnitState = {
        rangeTrigger = nil,
        fireTimer = nil,
        targetQueue = {},
        targetGroup = {},
        currentTarget = nil,
        lastAttackTarget = nil,
    }

    UnitTypeFireOnTheMoveInfo = {
        bone = 'Turret_Base',
        fireEffectPath = nil,
        projectileLaunchOffset = {
            x = 0,
            y = 0,
            z = 0
        },
        targetGround = true,
        targetAir = false,
        attackIndex = 0, --0 or 1
        range = nil,
        freeFlightTime = 0.0
    }

    FireOnTheMove = {
        unitTypes = {},
        abilitiesAllowingFireOnTheMove = {},
        trackedUnitStates = {},
        debug = false,
        queueDebug = false
    }

    local function printDebugTargetQueue(msg)
        if FireOnTheMove.queueDebug and SimpleUtils.globalDebug then
            print("[FIRE ON THE MOVE QUEUE] " .. msg)
        end
    end

    local function printDebug(msg)
        if FireOnTheMove.debug and SimpleUtils.globalDebug then
            print("[FIRE ON THE MOVE] " .. msg)
        end
    end

    local function isValidTarget(unitToTrack, target)
        if unitToTrack == target then
            return false
        end
        local owner = GetOwningPlayer(unitToTrack)
        if not IsUnitEnemy(target, owner) then
            return false
        end
        local unitTypeInfo = FireOnTheMove.unitTypes[GetUnitTypeId(unitToTrack)]
        if UnitIsSleeping(target) then
            return false
        end
        if IsUnitInvisible(target, owner) then
            return false
        end
        if unitTypeInfo.targetGround == true and (IsUnitType(target, UNIT_TYPE_GROUND) or IsUnitStructure(target)) then
            return true
        end
        if unitTypeInfo.targetAir == true and IsUnitType(target, UNIT_TYPE_FLYING) then
            return true
        end
        return false
    end

    local function isAliveAndInRangeAndValid(unit, target, range)
        return unit ~= nil
                and UnitAlive(target)
                and SimpleUtils.distanceBetweenCoordinates(GetUnitX(unit), GetUnitY(unit), GetUnitX(target), GetUnitY(target)) <= range + 50.0
                and isValidTarget(unit, target)
    end

    local function acquireTarget(trackedUnitState, unit, target)
        local typeId = GetUnitTypeId(unit)
        local bone = FireOnTheMove.unitTypes[typeId].bone
        if bone ~= nil then
            SetUnitLookAt(unit, bone, target, 0, 0, 0)
        end
        trackedUnitState.currentTarget = target
        ResumeTimer(trackedUnitState.fireTimer)
        printDebugTargetQueue("acquired target: " .. GetUnitName(target))
    end

    local function getAttackIndex(unit)
        local typeId = GetUnitTypeId(unit)
        return FireOnTheMove.unitTypes[typeId].attackIndex or 0
    end

    local function getRange(unit)
        local typeId = GetUnitTypeId(unit)
        local overridenRange = FireOnTheMove.unitTypes[typeId].range
        if overridenRange ~= nil then
            return overridenRange
        end
        local attackIndex = getAttackIndex(unit)
        return BlzGetUnitWeaponRealField(unit, UNIT_WEAPON_RF_ATTACK_RANGE, attackIndex)
    end

    local function findAndAcquireNextTarget(trackedUnitState, unit)
        printDebugTargetQueue("acquireOrFilterOutTarget: " .. GetUnitName(unit))
        local range = getRange(unit)
        if isAliveAndInRangeAndValid(unit, trackedUnitState.lastAttackTarget, range) then
            acquireTarget(trackedUnitState, unit, trackedUnitState.lastAttackTarget)
            return
        end

        local targetQueue = trackedUnitState.targetQueue

        local i=1
        while i <= #targetQueue do
            local target = targetQueue[i]
            printDebugTargetQueue("checking queue pos: " .. tostring(i) .. " of " .. tostring(#targetQueue))
            if isAliveAndInRangeAndValid(unit, target, range) then
                acquireTarget(trackedUnitState, unit, target)
                return
            else
                GroupRemoveUnit(trackedUnitState.targetGroup, target)
                table.remove(targetQueue, i)
                printDebugTargetQueue("filtered out target: " .. GetUnitName(target))
            end
        end
        printDebugTargetQueue("acquireOrFilterOutTarget - queue empty " .. GetUnitName(unit))
        GroupClear(trackedUnitState.targetGroup)
        trackedUnitState.targetQueue = {}
        trackedUnitState.currentTarget = nil
        ResetUnitLookAt(unit)
    end

    local function fire(sourceUnit, targetUnit, unitTypeInfo)
        --TODO Read this on track start and store per unit? (but it could change, e.g. via upgrades) Could I do this per unit type at game start?
        local attackIndex = getAttackIndex(sourceUnit)
        local missileModel = BlzGetUnitWeaponStringField(sourceUnit, UNIT_WEAPON_SF_ATTACK_PROJECTILE_ART, attackIndex)
        local missileSpeed = BlzGetUnitWeaponRealField(sourceUnit, UNIT_WEAPON_RF_ATTACK_PROJECTILE_SPEED, attackIndex)
        local damage = BlzGetUnitWeaponIntegerField(sourceUnit, UNIT_WEAPON_IF_ATTACK_DAMAGE_BASE, attackIndex)
        local attackType = ConvertAttackType(BlzGetUnitWeaponIntegerField(sourceUnit, UNIT_WEAPON_IF_ATTACK_ATTACK_TYPE, attackIndex))
        local fireEffectPath = unitTypeInfo.fireEffectPath
        local projectileLaunchOffset = unitTypeInfo.projectileLaunchOffset
        local onHit = unitTypeInfo.onHit

        local sourceX = GetUnitX(sourceUnit)
        local sourceY = GetUnitY(sourceUnit)
        local sourceZ = GetUnitFlyHeight(sourceUnit)

        local hasFreeFlightTime = unitTypeInfo.freeFlightTime ~= nil and unitTypeInfo.freeFlightTime > 0

        local angle
        local targetX
        local targetY
        local targetZ = 60

        if hasFreeFlightTime then
            angle = GetUnitFacing(sourceUnit)
            targetX = sourceX + unitTypeInfo.freeFlightTime * missileSpeed * 100 * Cos(angle)
            targetY = sourceY + unitTypeInfo.freeFlightTime * missileSpeed * 100 * Sin(angle)
        else
            targetX = GetUnitX(targetUnit)
            targetY = GetUnitY(targetUnit)
            angle = SimpleUtils.angleBetweenCoordinates(sourceX, sourceY, targetX, targetY)
        end

        if projectileLaunchOffset ~= nil then
            sourceX = sourceX + (projectileLaunchOffset.x or 0) * Cos(angle)
            sourceY = sourceY + (projectileLaunchOffset.y or 0) * Sin(angle)
            sourceZ = sourceZ + (projectileLaunchOffset.z or 0)
        end

        --printDebug("FireOnTheMove height: " .. tostring(sourceZ) .. " -> " .. tostring(targetZ))

        printDebug(GetUnitName(sourceUnit) .. " firing at " .. GetUnitName(targetUnit))

        local missile = Missiles:create(sourceX, sourceY, sourceZ, targetX, targetY, targetZ)
        missile:model(missileModel)
        missile:speed(missileSpeed)
        missile.theta = angle
        missile.owner = GetOwningPlayer(sourceUnit)
        missile.source = sourceUnit
        if not hasFreeFlightTime then
            missile.target = targetUnit
        end

        missile.onRemove = function()
            UnitDamageTarget(sourceUnit, targetUnit, damage, true, true, attackType, DAMAGE_TYPE_NORMAL, nil)
            printDebug("fire on the move dmg: " .. tostring(damage))
            if onHit then
                onHit(sourceUnit, targetUnit)
            end
            return true
        end

        if hasFreeFlightTime then
            SimpleUtils.timed(unitTypeInfo.freeFlightTime, function()
                missile.target = targetUnit
                missile.turn = 0.1
            end)
        end

        missile:launch()

        if fireEffectPath ~= nil then
            local unitState = FireOnTheMove.trackedUnitStates[sourceUnit]
            if unitState.fireEffect ~= nil then
                DestroyEffect(unitState.fireEffect)
            end
            local fireEffect = AddSpecialEffectTarget(fireEffectPath, sourceUnit, "weapon")
            unitState.fireEffect = fireEffect
            BlzSetSpecialEffectPosition(fireEffect, 0, 90, 90)
            printDebug("Created fire effect")
        end
    end

    local function isValidOrderToFireOnTheMove(order)
        return order == ORDER_ID_MOVE
                or order == ORDER_ID_SMART
                or order == ORDER_ID_PATROL
    end

    local function fireRoutine(unitToTrack)
        local unitState = FireOnTheMove.trackedUnitStates[unitToTrack]
        local currentOrder = GetUnitCurrentOrder(unitToTrack)
        if not isValidOrderToFireOnTheMove(currentOrder) then
            printDebug("order no longer valid to fire on the move: " .. tostring(currentOrder) .. "[" .. OrderId2String(currentOrder) .. "]")
            ResetUnitLookAt(unitToTrack)
            PauseTimer(unitState.fireTimer)
            return
        end
        local unitTypeInfo = FireOnTheMove.unitTypes[GetUnitTypeId(unitToTrack)]
        if BlzGetUnitWeaponBooleanField(unitToTrack, UNIT_WEAPON_BF_ATTACKS_ENABLED, unitTypeInfo.attackIndex) ~= true then
            ResetUnitLookAt(unitToTrack)
            PauseTimer(unitState.fireTimer)
            return
        end

        local typeId = GetUnitTypeId(unitToTrack)
        local target = unitState.currentTarget
        local range = getRange(unitToTrack)
        if not isAliveAndInRangeAndValid(unitToTrack, target, range) then
            findAndAcquireNextTarget(unitState, unitToTrack)
        end
        target = unitState.currentTarget
        if isAliveAndInRangeAndValid(unitToTrack, target, range) then
            fire(unitToTrack, target, FireOnTheMove.unitTypes[typeId])
        else
            printDebug("no target in range to continue fire on the move")
            ResetUnitLookAt(unitToTrack)
            PauseTimer(unitState.fireTimer)
        end
    end

    local function getAttackInterval(unit)
        local attackIndex = getAttackIndex(unit)
        return BlzGetUnitWeaponRealField(unit, UNIT_WEAPON_RF_ATTACK_BASE_COOLDOWN, attackIndex)
    end

    local function startFireTimer(unitToTrack, fireTimer)
        local attackInterval = getAttackInterval(unitToTrack)
        printDebug("startFireTimer: " .. GetUnitName(unitToTrack) .. ", " .. tostring(attackInterval))
        TimerStart(fireTimer, attackInterval, true, function()
           fireRoutine(unitToTrack)
        end)
    end

    local function addTarget(unitFiringOnTheMove, target)
        table.insert(unitFiringOnTheMove.targetQueue, target)
        GroupAddUnit(unitFiringOnTheMove.targetGroup, target)
    end

    local function registerTarget(unit, target)
        local trackedUnitState = FireOnTheMove.trackedUnitStates[unit]
        --printDebug("registerTarget: " .. GetUnitName(unit) .. ", " .. GetUnitName(target) .. ", targets: " .. CountUnitsInGroup(trackedUnitState.targetGroup))
        if IsUnitGroupEmptyBJ(trackedUnitState.targetGroup) or IsUnitGroupDeadBJ(trackedUnitState.targetGroup) then
            GroupClear(trackedUnitState.targetGroup)
            addTarget(trackedUnitState, target)
            findAndAcquireNextTarget(trackedUnitState, unit)
        else
            addTarget(trackedUnitState, target)
            if target == trackedUnitState.lastAttackTarget and isAliveAndInRangeAndValid(unit, target, getRange(unit)) then
                acquireTarget(trackedUnitState, unit, target)
            end
        end
    end

    local function isInTargetGroup(unitToTrack, target)
        return IsUnitInGroup(target, FireOnTheMove.trackedUnitStates[unitToTrack].targetGroup)
    end

    local function createRangeTrigger(unitToTrack)
        local rangeTrigger = CreateTrigger()
        TriggerRegisterUnitInRangeSimple(rangeTrigger, getRange(unitToTrack), unitToTrack)
        TriggerAddCondition(rangeTrigger, Condition(function()
            if isInTargetGroup(unitToTrack, GetTriggerUnit()) then
                --printDebugTargetQueue("shouldRegisterTarget(" .. GetUnitName(GetTriggerUnit()) .. "): [already in queue] " .. tostring(false))
                return false
            end
            local result = isValidTarget(unitToTrack, GetTriggerUnit())
            --printDebugTargetQueue("shouldRegisterTarget(" .. GetUnitName(GetTriggerUnit()) .. "): " .. tostring(result))
            return result
        end))
        TriggerAddAction(rangeTrigger, function()
            registerTarget(unitToTrack, GetTriggerUnit())
        end)
        return rangeTrigger
    end

    local function trackUnit(unitToTrack)
        printDebug("trackUnit: " .. GetUnitName(unitToTrack))

        local trackedUnitState = TrackedUnitState:new()
        trackedUnitState.fireTimer = CreateTimer()
        startFireTimer(unitToTrack, trackedUnitState.fireTimer)
        PauseTimer(trackedUnitState.fireTimer)
        trackedUnitState.targetQueue = {}
        trackedUnitState.targetGroup = CreateGroup()
        trackedUnitState.rangeTrigger = createRangeTrigger(unitToTrack)

        FireOnTheMove.trackedUnitStates[unitToTrack] = trackedUnitState
    end

    local function canFireOnTheMove(unit)
        local typeId = GetUnitTypeId(unit)
        if FireOnTheMove.unitTypes[typeId] == nil then
            return false
        end
        if FireOnTheMove.abilitiesAllowingFireOnTheMove == nil then
            return true
        end
        for i, ability in ipairs(FireOnTheMove.abilitiesAllowingFireOnTheMove) do
            if UnitHasBuffBJ(unit, ability) then
                return true
            end
        end
        return false
    end

    local function rememberLastAttackOrderTarget(unit, target)
        local unitState = FireOnTheMove.trackedUnitStates[unit]
        if unitState == nil then
            SimpleUtils.printWarn('No tracked state for unit firing on the move ' .. GetUnitName(unit))
            return
        end
        unitState.lastAttackTarget = target

        printDebug("rememberLastAttackOrderTarget: " .. GetUnitName(target))
    end

    local function validateUnitType(key, unitTypeInfo)
        assert(unitTypeInfo.targetGround == true or unitTypeInfo.targetAir == true, 'Fire on the move: unit ' .. tostring(key) .. ' must attack ground, air or both')
    end

    local function validateUnitTypes(unitTypes)
        for key, unitTypeInfo in pairs(unitTypes) do
            validateUnitType(key, unitTypeInfo)
        end
    end

    UnitTracker:register('fireOnTheMove', canFireOnTheMove)

    function FireOnTheMove:init(unitTypes, abilitiesAllowingFireOnTheMove)
        validateUnitTypes(unitTypes)
        self.unitTypes = unitTypes
        self.abilitiesAllowingFireOnTheMove = abilitiesAllowingFireOnTheMove
    end

    OnInit.trig(function()
        SimpleUtils.newClass(TrackedUnitState)
        SimpleUtils.newClass(UnitTypeFireOnTheMoveInfo)

        UnitUtils:forEachUnit(UnitUtils:getInitialUnitsFiringOnTheMove(), trackUnit)

        local trainTrigger = CreateTrigger()
        TriggerRegisterAnyUnitEventBJ(trainTrigger, EVENT_PLAYER_UNIT_TRAIN_FINISH)
        TriggerAddCondition(trainTrigger, Condition(function()
            return canFireOnTheMove(GetTrainedUnit())
        end))
        TriggerAddAction(trainTrigger, function()
            trackUnit(GetTrainedUnit())
        end)

        UnitUtils:registerUnitDeathAction(function(dyingUnit)
            local trackedUnitState = FireOnTheMove.trackedUnitStates[dyingUnit]
            if trackedUnitState == nil then
                return
            end
            DestroyTrigger(trackedUnitState.rangeTrigger)
            SimpleUtils.releaseTimer(trackedUnitState.fireTimer)
            DestroyGroup(trackedUnitState.targetGroup)
            trackedUnitState.targetQueue = nil
            FireOnTheMove.trackedUnitStates[dyingUnit] = nil
        end)

        local attackOrderTrigger = CreateTrigger()
        TriggerRegisterAnyUnitEventBJ(attackOrderTrigger, EVENT_PLAYER_UNIT_ISSUED_UNIT_ORDER)
        TriggerAddCondition(attackOrderTrigger, Condition(function()
            return (GetIssuedOrderId() == ORDER_ID_ATTACK or GetIssuedOrderId() == ORDER_ID_SMART)
                    and canFireOnTheMove(GetTriggerUnit())
                    and isValidTarget(GetTriggerUnit(), GetOrderTargetUnit())
        end))
        TriggerAddAction(attackOrderTrigger, function()
            rememberLastAttackOrderTarget(GetTriggerUnit(), GetOrderTargetUnit())
        end)

        local moveOrderTrigger = CreateTrigger()
        TriggerRegisterAnyUnitEventBJ(moveOrderTrigger, EVENT_PLAYER_UNIT_ISSUED_POINT_ORDER)
        TriggerAddCondition(moveOrderTrigger, Condition(function()
            return isValidOrderToFireOnTheMove(GetIssuedOrderId()) and canFireOnTheMove(GetTriggerUnit())
        end))
        TriggerAddAction(moveOrderTrigger, function()
            local trackedUnitState = FireOnTheMove.trackedUnitStates[GetTriggerUnit()]
            if trackedUnitState == nil then
                return
            end
            findAndAcquireNextTarget(trackedUnitState, GetTriggerUnit())
        end)

        printDebug("FireOnTheMove OnInit.final DONE")
    end)
end
if Debug then Debug.endFile() end