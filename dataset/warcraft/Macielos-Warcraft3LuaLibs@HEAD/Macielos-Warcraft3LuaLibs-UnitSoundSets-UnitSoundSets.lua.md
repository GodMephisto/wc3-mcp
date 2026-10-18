---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "UnitSoundSets/UnitSoundSets.lua"
ref: HEAD
---

if Debug then Debug.beginFile "UnitSoundSets" end
do
    --[[/**
     * Unit Sound Sets
     * Originally created in JASS by Barade
     * Ported to LUA and extended by Macielos
     *
     * Allows using custom unit sound sets without replacing existing ones or using a custom SLK file.
     *
     * Usage:
     * - Disable the sound of your unit type and then register the sounds using this system.
     * - Import custom soundfiles into your map.
     * - Call this LUA code during the map initialization:
     *   AddUnitSoundSet(FourCC('H0F2'), "war3Imported\\HeroPaladin")
     *   'H0F2' should be the raw code of your custom hero unit type.
     *   "war3Imported\\HeroPaladin" should be the prefix for all unit sound files.
     *   The extensions "mp3", "flac" and "wav" are used automatically.
     *
     *  You can have ANY number of What, Yes, Attack, Pissed etc. sounds.
     *  You can add multiple so und sets for the same unit type, they will be treated additively.
     *  For most sound types adding order doesn't matter as they are played randomly, the only exception is Pissed
     *  sounds which always play in the same order
     *
     * The following suffixes for sound files are supported:
     * Death sounds:
     * Death1
     * Death2
     * ...
     * On-click sounds:
     * What1
     * What2
     * ...
     * On-order (move, patrol etc.) sounds:
     * Yes1
     * Yes2
     * ...
     * On-attack sounds (2 alternative suffixes, you can use either of them):
     * Attack1
     * Attack2
     * ...
     * YesAttack1
     * YesAttack2
     * ...
     * Sounds played when unit's trained/hired/revived:
     * Ready1
     * Ready2
     * ...
     * Sounds played with a certain chance when attacking a hero:
     * Warcry1
     * Warcry2
     * ...
     * Secret/Hidden/Pissed sounds (again, several alternatives available because Nyctaeus and I couldn't stick to a single convention ;))
     * Pissed1
     * Pissed2
     * ...
     * Hidden1
     * Hidden1
     * ...
     * Gag1
     * Gag2
     * ...
     * Disable cinematic sub titles with ForceCinematicSubtitles or in the game settings.
     * Selecting the same or a different unit again will not play the next sound until the previous sound is playing.
     * This seems to be Warcraft's behavior.
     */]]

    --  You will hear the pissed sounds after this number of clicks on the same unit.
    local PISSED_COUNTER = 3
    --  Percentage chance to play the Warcry sound when attacking a hero.
    local WARCRY_CHANCE = 20

    --  Death : Whenver the unit dies, the Death Sound will be played.
    local SOUND_DEATH = 0
    --  What : Occurs when you click on the unit only. Replaced by Pissed Sounds if you click multiple times on the unit in a short time.
    local SOUND_WHAT = 1
    --  Yes : Occurs when you order the unit to do a "Friendly" Action.
    local SOUND_YES = 2
    --  YesAttack : Occurs when you order the unit to "Attack-Move", or an order similar to it.
    local SOUND_YES_ATTACK = 3
    --  Warcry : Occurs when you order the unit to attack a specific unit.
    local SOUND_WARCRY = 4
    --  Ready : Occurs only one time for a single unit, it is played when the unit is spawned by a building (Example : Barracks for Rifleman), or when they got revived by an Altar (Example : Paladin).
    local SOUND_READY = 5
    --  Pissed : When you click a lot of times on the unit, weird sounds will be played instead of normals, which are called Pissed Sounds.
    local SOUND_PISSED = 6

    local unitSoundSets = SoundSet:new()

    local unitAbilitySoundSets = SoundSet:new()

    --  all this exists for the local player only and is never synchronized between players
    local playerClickCounter
    local playerClickTarget
    local playerSound
    local playerSoundTimer = CreateTimer()
    local playerSpeakerUnit
    local playerPissedCounter = 0

    local selectionTrigger = CreateTrigger()
    local deselectionTrigger = CreateTrigger()
    local orderTrigger = CreateTrigger()
    local revivalTrigger = CreateTrigger()
    local trainTrigger = CreateTrigger()
    local hireTrigger = CreateTrigger()
    local deathTrigger = CreateTrigger()
    local abilityCastTrigger = CreateTrigger()

    local customConditions = {}

    --  avoid reference leaks
    local allies = CreateForce()          --force

    local EAX_SETTING = "HeroAcksEAX"
    local EAX_SETTING_DEATH = "DefaultEAXON"

    UnitSoundSets = {
        debug = false
    }

    function UnitSoundSets:addCustomCondition(condition)
        assert(type(condition) == 'function', 'Custom condition for unit sound sets must be a function that returns bool!')
        table.insert(customConditions, condition)
    end

    function UnitSoundSets:get()
        return unitSoundSets
    end

    function UnitSoundSets:getForAbilities()
        return unitAbilitySoundSets
    end

    local function printDebug(msg)
        if UnitSoundSets.debug and SimpleUtils.globalDebug then
            print("[UNIT SOUND SETS] " .. msg)
        end
    end

    function UnitSoundSets.printUnitSoundDebug(msg)
        printDebug(msg)
    end

    local function isUnitMainSelectedUnitForPlayer (whichUnit)
        local mainSelected = SelectionTracker:getMainForLocalPlayer()
        --printDebug("Main selected " .. GetUnitName(mainSelected))
        return mainSelected ~= nil and mainSelected == whichUnit
        --return true
    end

    function UnitSoundSets:removeAllUnitSoundSets()
        unitSoundSets:clear()
        unitAbilitySoundSets:clear()
    end

    function UnitSoundSets:removeUnitSoundSet(unitTypeId)
        unitSoundSets:remove(unitTypeId)
        unitAbilitySoundSets:remove(unitTypeId)
    end

    function UnitSoundSets:hasUnitSoundSet(unitTypeId)
        return unitSoundSets:exists(unitTypeId, SOUND_WHAT)
    end

    function UnitSoundSets:hasAbilitySoundSet(unitTypeId, abilityCode)
        return unitAbilitySoundSets:exists(unitTypeId, abilityCode)
    end

    local function createSoundFromFile (filePath, eaxSetting)
        local soundHandle
        local duration = GetSoundFileDuration(filePath)
        --  the duration should be 0 if the file does not exist
        if (duration > 0) then
            soundHandle = CreateSound(filePath, false, false, true, 12700, 12700, eaxSetting)
            SetSoundDuration(soundHandle, duration)
        else
            UnitSoundSets.printUnitSoundDebug("Missing sound file: " .. filePath)
        end
        return soundHandle
    end

    local function addUnitSoundSetFromFilesType (unitTypeId, filePath, soundType, prefix)
        local eaxSetting
        if soundType == SOUND_DEATH then
            eaxSetting = EAX_SETTING_DEATH
        else
            eaxSetting = EAX_SETTING
        end
        local i = 1
        while true do
            local fullName = filePath .. prefix .. I2S(i)
            if not (unitSoundSets:add(unitTypeId, soundType, i, createSoundFromFile(fullName, eaxSetting))) then
                return
            end
            UnitSoundSets.printUnitSoundDebug("Added ability sound for unit type " .. tostring(unitTypeId) .. ", ability: " .. tostring(soundType) .. " -> fullName: " .. fullName);
            i = i + 1
        end
    end

    function UnitSoundSets:addUnitSoundSet(unitTypeId, filePathPrefix)
        addUnitSoundSetFromFilesType(unitTypeId, filePathPrefix, SOUND_DEATH, "Death")
        addUnitSoundSetFromFilesType(unitTypeId, filePathPrefix, SOUND_WHAT, "What")
        addUnitSoundSetFromFilesType(unitTypeId, filePathPrefix, SOUND_YES, "Yes")
        addUnitSoundSetFromFilesType(unitTypeId, filePathPrefix, SOUND_YES_ATTACK, "Attack")
        addUnitSoundSetFromFilesType(unitTypeId, filePathPrefix, SOUND_YES_ATTACK, "YesAttack")
        addUnitSoundSetFromFilesType(unitTypeId, filePathPrefix, SOUND_WARCRY, "WarCry")
        addUnitSoundSetFromFilesType(unitTypeId, filePathPrefix, SOUND_READY, "Ready")
        addUnitSoundSetFromFilesType(unitTypeId, filePathPrefix, SOUND_PISSED, "Pissed")
        addUnitSoundSetFromFilesType(unitTypeId, filePathPrefix, SOUND_PISSED, "Hidden")
        addUnitSoundSetFromFilesType(unitTypeId, filePathPrefix, SOUND_PISSED, "Gag")
    end

    local function addUnitAbilitySoundFromFile(unitTypeId, abilityId, filePath, index)
        return unitAbilitySoundSets:add(unitTypeId, abilityId, index, createSoundFromFile(filePath, EAX_SETTING))
    end

    function UnitSoundSets:addUnitAbilitySingleSound(unitTypeId, abilityId, filePath)
        addUnitAbilitySoundFromFile(unitTypeId, abilityId, filePath, 1)
    end

    function UnitSoundSets:addUnitAbilityMultipleSounds(unitTypeId, abilityId, filePathPrefix)
        local i = 1
        while true do
            if not addUnitAbilitySoundFromFile(unitTypeId, abilityId, filePathPrefix .. tostring(i), i) then
                return
            end
            i = i + 1
        end
    end

    -- =============================================================================

    local function areUnitSoundsEnabled(unit)
        if bj_cineModeAlreadyIn == true or IsUnitPaused(unit) or UnitIsSleeping(unit) then
            return false
        end
        for i, condition in ipairs(customConditions) do
            if condition(unit) == false then
                return false
            end
        end
        return true
    end

    local function playerHasControl(whichPlayer, whichUnit)
        return GetOwningPlayer(whichUnit) == whichPlayer or GetPlayerAlliance(whichPlayer, GetOwningPlayer(whichUnit), ALLIANCE_SHARED_CONTROL)
    end

    local function getAlliesWithSharedControl(whichPlayer)
        ForceClear(allies)
        for i = 0, bj_MAX_PLAYERS - 1 do
            if (whichPlayer == Player(i) or GetPlayerAlliance(whichPlayer, Player(i), ALLIANCE_SHARED_CONTROL)) then
                ForceAddPlayer(allies, Player(i))
            end
        end
        return allies
    end

    local function isUnitSpeakingForPlayer(whichUnit)
        return playerSpeakerUnit == whichUnit and playerSound ~= nil and TimerGetRemaining(playerSoundTimer) > 0.0 --  This would desync: GetSoundIsPlaying(playerSound)
    end

    local function isAnyUnitSpeakingForPlayer()
        return playerSpeakerUnit ~= nil and isUnitSpeakingForPlayer(playerSpeakerUnit)
    end

    local function setCurrentlyPlayingPlayerSound(soundSet, whichSound, whichUnit)
        --  only update if it is a different speaker or the current unit is not speaking
        playerSound = whichSound
        playerSpeakerUnit = whichUnit
        soundSet:setLastPlayedSound(GetUnitTypeId(whichUnit), whichSound)

        StartSound(whichSound)
        printDebug(GetUnitName(whichUnit) .. ": start sound")

        TimerStart(playerSoundTimer, GetSoundDurationBJ(whichSound), false, function()
            if playerSpeakerUnit == whichUnit then
                playerSpeakerUnit = nil
                playerSound = nil
                printDebug(GetUnitName(whichUnit) .. ": end sound")
            end
        end)
    end

    local function resetCounters()
        printDebug("ResetCounters()")
        playerClickCounter = 1
        playerPissedCounter = 0
    end

    local function getNextPissedSound(whichUnit)
        if playerPissedCounter == nil then
            playerPissedCounter = 0
        end
        local currentPissedCounter = playerPissedCounter
        local unitTypeId = GetUnitTypeId(whichUnit)
        local pissedCount = unitSoundSets:getCount(unitTypeId, SOUND_PISSED)

        local whichSound
        while currentPissedCounter <= pissedCount do
            playerPissedCounter = playerPissedCounter + 1
            whichSound = unitSoundSets:get(unitTypeId, SOUND_PISSED, playerPissedCounter)
            if not (whichSound == nil) then
                printDebug("GetNextPissedSound: " .. tostring(currentPissedCounter))
                return whichSound
            end
            currentPissedCounter = currentPissedCounter + 1
        end
        resetCounters()
        return nil
    end

    local function getRandomSound(soundSet, whichUnit, soundType)
        local unitTypeId = GetUnitTypeId(whichUnit)
        return soundSet:getRandom(unitTypeId, soundType)
    end

    local function updatePlayerClickedUnit(whichUnit)
        if (playerClickTarget == whichUnit) then
            playerClickCounter = playerClickCounter + 1
        else
            resetCounters()
        end

        printDebug("playerClickCounter for unit " .. GetUnitName(whichUnit) .. ": " .. tostring(playerClickCounter))

        playerClickTarget = whichUnit
    end

    local function isPlayerSelectionPissed(whichUnit)
        return playerClickTarget == whichUnit and playerClickCounter > PISSED_COUNTER
    end

    local function getUnitPlayerColor(whichPlayer, whichUnit)
        local filterState = GetAllyColorFilterState()
        if (filterState == 2) then
            --  mode 3
            if (GetOwningPlayer(whichUnit) == whichPlayer) then
                return PLAYER_COLOR_BLUE
            elseif (IsUnitAlly(whichUnit, whichPlayer)) then
                return PLAYER_COLOR_TURQUOISE
            else
                return PLAYER_COLOR_RED
            end
        end
        return GetPlayerColor(GetOwningPlayer(whichUnit))
    end

    local function startUnitTalkPortrait(whichPlayer, whichUnit, whichSound)
        local unitTypeId = GetUnitTypeId(whichUnit)
        SetCinematicScene(unitTypeId, getUnitPlayerColor(whichPlayer, whichUnit), '', '', GetSoundDurationBJ(whichSound), GetSoundDurationBJ(whichSound))
        if FakeBars ~= nil then
            FakeBars:show(whichSound)
        end
    end

    local function playSoundInternal(soundSet, whichPlayer, whichUnit, soundType, whichSound)
        if whichSound == nil then
            printDebug("No sound provided")
            return
        end
        if isAnyUnitSpeakingForPlayer() then
            printDebug("Player already speaking")
            return
        end

        setCurrentlyPlayingPlayerSound(soundSet, whichSound, whichUnit)

        if soundType == SOUND_WHAT then
            updatePlayerClickedUnit(whichUnit)
        elseif soundType == SOUND_YES or soundType == SOUND_YES_ATTACK or soundType == SOUND_WARCRY then
            resetCounters()
        end

        if (IsUnitSelected(whichUnit, GetLocalPlayer())) then
            startUnitTalkPortrait(whichPlayer, whichUnit, whichSound)
        end
    end

    local function playRandomSound(soundSet, whichPlayer, whichUnit, soundType)
        printDebug("PlayRandomSound: " .. GetUnitName(whichUnit) .. ", " .. tostring(soundType))
        local whichSound = getRandomSound(soundSet, whichUnit, soundType)
        playSoundInternal(soundSet, whichPlayer, whichUnit, soundType, whichSound)
    end

    local function playRandomSoundForForce(whichForce, whichUnit, soundType)
        if not IsPlayerInForce(GetLocalPlayer(), whichForce) then
            return
        end
        local slotPlayer
        for i = 0, bj_MAX_PLAYERS - 1 do
            slotPlayer = Player(i)
            if (IsPlayerInForce(slotPlayer, whichForce)) then
                playRandomSound(unitSoundSets, slotPlayer, whichUnit, soundType)
            end
        end
    end

    local function playRandomSoundForAlliesWithSharedControl(whichUnit, soundType)
        if not areUnitSoundsEnabled(whichUnit) then
            return
        end
        playRandomSoundForForce(getAlliesWithSharedControl(GetOwningPlayer(whichUnit)), whichUnit, soundType)
    end

    local function stopUnitSound(whichPlayer, whichUnit)
        --  the death sound will stop any currently played sound from the unit
        if (isUnitSpeakingForPlayer(whichUnit, whichPlayer)) then
            if (GetLocalPlayer() == whichPlayer) then
                StopSound(playerSound, false, false)
            end
        end
    end

    local function playRandom3DDeathSound(whichUnit)
        local soundHandle = getRandomSound(unitSoundSets, whichUnit, SOUND_DEATH)
        if soundHandle ~= nil then
            StartSound(soundHandle)
            AttachSoundToUnit(soundHandle, whichUnit)
        end
    end

    local function playNextPissedSound(whichPlayer, whichUnit)
        local whichSound = getNextPissedSound(whichUnit)
        if (whichSound == nil) then
            playRandomSound(unitSoundSets, whichPlayer, whichUnit, SOUND_WHAT)
        else
            playSoundInternal(unitSoundSets, whichPlayer, whichUnit, SOUND_PISSED, whichSound)
        end
    end

    local function endUnitTalkPortrait(whichUnit)
        if (UnitSoundSets:hasUnitSoundSet(GetUnitTypeId(whichUnit)) and playerSpeakerUnit == whichUnit) then
            --  Do not end talk animations for native sound sets.
            --  TODO Deselecting a unit with custom sound and selecting a unit with a native sound seems to stop the talk animation because of this.
            EndCinematicScene()
        end
        printDebug('endUnitTalkPortrait' .. GetUnitName(whichUnit))
        if FakeBars ~= nil then
            FakeBars:hide()
        end
    end

    local function timerFunctionSelect(triggerUnit)
        printDebug('TimerFunctionSelect: ' .. GetUnitName(triggerUnit))
        local triggerPlayer = GetOwningPlayer(triggerUnit)
        if playerSpeakerUnit ~= nil and not isUnitSpeakingForPlayer(triggerUnit) then
            endUnitTalkPortrait(playerSpeakerUnit)
        end

        if (playerHasControl(GetLocalPlayer(), triggerUnit)
                and isUnitMainSelectedUnitForPlayer(triggerUnit)
                and areUnitSoundsEnabled(triggerUnit)) then
            if isAnyUnitSpeakingForPlayer() then
                return
            end

            if (isPlayerSelectionPissed(triggerUnit)) then
                playNextPissedSound(triggerPlayer, triggerUnit)
            else
                playRandomSound(unitSoundSets, triggerPlayer, triggerUnit, SOUND_WHAT)
            end
        end
    end

    local function triggerActionSelect()
        local unit = GetTriggerUnit()
        printDebug('TriggerActionSelect: ' .. GetUnitName(unit))
        --  some delay to determine the main selected unit
        SimpleUtils.timed(0.0, function()
            timerFunctionSelect(unit)
        end)
    end

    local function triggerConditionDeselect()
        endUnitTalkPortrait(GetTriggerUnit())
        return false
    end

    local function triggerActionOrder()
        local triggerUnit = GetTriggerUnit()
        local triggerUnitTypeId = GetUnitTypeId(triggerUnit)
        local orderId = GetIssuedOrderId()
        --printDebug("TriggerActionOrder: " .. GetUnitName(triggerUnit) .. ", " .. tostring(orderId) .. ", " .. OrderId2String(orderId))
        local slotPlayer = GetLocalPlayer()
        if (playerHasControl(slotPlayer, triggerUnit)
                and isUnitMainSelectedUnitForPlayer(triggerUnit)
                and areUnitSoundsEnabled(triggerUnit)) then
            if (orderId == ORDER_ID_ATTACK or orderId == ORDER_ID_ATTACK_ONCE or (orderId == ORDER_ID_SMART and GetOrderTargetUnit() ~= nil and IsUnitEnemy(GetOrderTargetUnit(), slotPlayer))) then
                resetCounters()
                if (unitSoundSets:exists(triggerUnitTypeId, SOUND_WARCRY)
                        and IsUnitType(GetOrderTargetUnit(), UNIT_TYPE_HERO)
                        and GetRandomInt(0, 100) <= WARCRY_CHANCE) then
                    -- chance for Warcry if the target is a hero
                    playRandomSound(unitSoundSets, slotPlayer, triggerUnit, SOUND_WARCRY)
                else
                    playRandomSound(unitSoundSets, slotPlayer, triggerUnit, SOUND_YES_ATTACK)
                end
            elseif (orderId == ORDER_ID_MOVE or orderId == ORDER_ID_PATROL or orderId == ORDER_ID_SMART or orderId == ORDER_ID_ATTACK_GROUND) then
                resetCounters()
                playRandomSound(unitSoundSets, slotPlayer, triggerUnit, SOUND_YES)
            end
        end
        slotPlayer = nil
        triggerUnit = nil
    end

    local function triggerConditionReviveFinish()
        playRandomSoundForAlliesWithSharedControl(GetRevivingUnit(), SOUND_READY)
        return false
    end

    local function triggerConditionTrainFinish()
        playRandomSoundForAlliesWithSharedControl(GetTrainedUnit(), SOUND_READY)
        return false
    end

    local function triggerConditionHire()
        playRandomSoundForAlliesWithSharedControl(GetSellingUnit(), SOUND_READY)
        return false
    end

    local function triggerConditionDeath()
        local triggerUnit = GetDyingUnit()
        local alliesWithSharedControl = getAlliesWithSharedControl(GetOwningPlayer(triggerUnit))
        local slotPlayer = nil
        for i = 0, bj_MAX_PLAYERS - 1 do
            slotPlayer = Player(i)
            if (IsPlayerInForce(slotPlayer, alliesWithSharedControl)) then
                endUnitTalkPortrait(triggerUnit)
            end
            slotPlayer = nil
        end
        stopUnitSound(GetOwningPlayer(triggerUnit), triggerUnit)
        playRandom3DDeathSound(triggerUnit)
        triggerUnit = nil
        return false
    end

    local function getAbilitySound(unitTypeId, abilityId)
        return unitAbilitySoundSets:getRandom(unitTypeId, abilityId)
    end

    local function playAbilitySound(caster, abilityId)
        local casterTypeId = GetUnitTypeId(caster)
        local sound = getAbilitySound(casterTypeId, abilityId)
        if sound == nil then
            return
        end
        playSoundInternal(unitAbilitySoundSets, GetOwningPlayer(caster), caster, abilityId, sound)
    end

    local function triggerConditionAbility()
        return playerHasControl(GetLocalPlayer(), GetTriggerUnit())
    end

    local function triggerAbilityFunction(caster, abilityId)
        printDebug("TriggerAbilityFunction " .. GetUnitName(caster) .. ": " .. tostring(abilityId))
        playAbilitySound(caster, abilityId)
    end

    local function triggerActionAbility()
        local abilityId = GetSpellAbilityId()
        local caster = GetTriggerUnit()
        if not areUnitSoundsEnabled(caster) then
            return
        end
        printDebug("TriggerActionAbility " .. GetUnitName(caster) .. ": " .. tostring(abilityId))
        SimpleUtils.timed(0.0, function()
            triggerAbilityFunction(caster, abilityId)
        end)
    end

    local function initUnitSoundSets()
        getAlliesWithSharedControl(GetLocalPlayer())

        ForForce(allies, function()
            local player = GetEnumPlayer()
            local playerId = GetPlayerId(player)

            printDebug("Registering actions for player " .. tostring(playerId) .. "...")
            if (GetPlayerController(player) == MAP_CONTROL_USER) then
                TriggerRegisterPlayerUnitEventSimple(selectionTrigger, player, EVENT_PLAYER_UNIT_SELECTED)
                TriggerRegisterPlayerUnitEventSimple(deselectionTrigger, player, EVENT_PLAYER_UNIT_DESELECTED)
                printDebug("Registered selection actions for player " .. tostring(playerId) .. "...")
            end
            TriggerRegisterPlayerUnitEventSimple(orderTrigger, player, EVENT_PLAYER_UNIT_ISSUED_ORDER)
            TriggerRegisterPlayerUnitEventSimple(orderTrigger, player, EVENT_PLAYER_UNIT_ISSUED_POINT_ORDER)
            TriggerRegisterPlayerUnitEventSimple(orderTrigger, player, EVENT_PLAYER_UNIT_ISSUED_UNIT_ORDER)
            TriggerRegisterPlayerUnitEventSimple(orderTrigger, player, EVENT_PLAYER_UNIT_ISSUED_TARGET_ORDER)
            TriggerRegisterPlayerUnitEventSimple(revivalTrigger, player, EVENT_PLAYER_HERO_REVIVE_FINISH)
            TriggerRegisterPlayerUnitEventSimple(trainTrigger, player, EVENT_PLAYER_UNIT_TRAIN_FINISH)
            TriggerRegisterPlayerUnitEventSimple(hireTrigger, player, EVENT_PLAYER_UNIT_SELL)
            TriggerRegisterPlayerUnitEventSimple(deathTrigger, player, EVENT_PLAYER_UNIT_DEATH)
            TriggerRegisterPlayerUnitEventSimple(abilityCastTrigger, player, EVENT_PLAYER_UNIT_SPELL_CAST)
            printDebug("Registered actions for player " .. tostring(playerId))
        end)

        TriggerAddAction(selectionTrigger, triggerActionSelect)
        TriggerAddCondition(deselectionTrigger, Condition(triggerConditionDeselect))

        TriggerAddAction(orderTrigger, triggerActionOrder)

        TriggerAddCondition(revivalTrigger, Condition(triggerConditionReviveFinish))

        TriggerAddCondition(trainTrigger, Condition(triggerConditionTrainFinish))

        TriggerAddCondition(hireTrigger, Condition(triggerConditionHire))

        TriggerAddCondition(deathTrigger, Condition(triggerConditionDeath))

        TriggerAddCondition(abilityCastTrigger, Condition(triggerConditionAbility))
        TriggerAddAction(abilityCastTrigger, triggerActionAbility)
        printDebug("UnitSoundSetsInit DONE")
    end

    OnInit.trig(initUnitSoundSets)

    OnInit.final(function()
        SetRandomSeed(21372137)
        --apparently this makes the timer synced so it doesn't desync in multi
        TimerStart(playerSoundTimer, 0.0, false, function() end)
    end)
end
if Debug then Debug.endFile() end
