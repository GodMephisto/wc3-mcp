---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "Libs/src/FakeBars.lua"
ref: HEAD
---

if Debug then Debug.beginFile "FakeBars" end
do
    -- Timer interval in which the HP and mana of the fake text frames is updated.
    local UPDATE_INTERVAL = 0.05
    -- Hides the HP and mana text if it is longer than this.
    local FAKE_BAR_CHARACTERS_LIMIT = 11
    local HIDE_HP_INVULNERABLE_STRUCTURE = true
    -- Default positions of fake bars, they are hardcoded as there's no native to get them from the original frames,
    -- if you use customized, UI may need to adjust these
    local FAKE_BAR_X = 0.2145
    local FAKE_HP_BAR_Y = 0.027
    local FAKE_MANA_BAR_Y = 0.0135
    -- Default sizes of fake bars, those are placeholders and are replaced by the ones from original frames on first use
    local FAKE_BAR_WIDTH = 0.078125
    local FAKE_BAR_HEIGHT = 0.011875

    local FAKE_BAR_TEXT_SCALE = 1.16

    local playerHPFrame = nil --framehandle
    local playerManaFrame = nil --framehandle

    local playerAnimationTimer = {} --timer
    local playerUpdateCinematicSceneBarsTimer = {} --timer

    local gameLoadedTrigger = CreateTrigger()

    FakeBars = {
        debug = false
    }

    local function printDebug(msg)
        if FakeBars.debug and SimpleUtils.globalDebug then
            print("[FAKE BARS] " .. msg)
        end
    end

    function FakeBars:hide()
        printDebug('FakeBars:hide')
        BlzFrameSetVisible(playerHPFrame, false)
        BlzFrameSetVisible(playerManaFrame, false)
        PauseTimer(playerUpdateCinematicSceneBarsTimer)
    end

    local function timerFunctionEndCinematicSceneBars()
        FakeBars:hide()
        EndCinematicScene()
    end

    -- Does not divide by 100.
    local function getPercent (value, maxValue)
        -- Return 0 for nil units.
        if (maxValue == 0) then
            return 0.0
        end

        return value / maxValue
    end

    local function I2X (int)
        local hexas = "0123456789abcdef"
        local hex = ""
        local dev

        while true do
            if int > 15 then
                dev = int - math.floor(int / 16) * 16
                int = math.floor(int / 16)
                hex = SubString(hexas, dev, dev + 1) .. hex
            else
                hex = SubString(hexas, int, int + 1) .. hex
                return hex
            end
        end
        return hex
    end

    local function I2XW (int, width)
        local hex = I2X(int)
        local i = StringLength(hex)
        while i < width do
            hex = "0" .. hex
            i = i + 1
        end
        return hex
    end

    local function getHPTextEx (life, maxLife)
        local lifePercent = getPercent(life, maxLife)
        local red = 255
        local green = 255
        local lifeStr = I2S(R2I(life))
        local maxLifeStr = I2S(R2I(maxLife))
        local lifeTextLength = StringLength(lifeStr)
        local fullTextLength = lifeTextLength + StringLength(maxLifeStr) + 3 -- / and the two spaces

        if (lifePercent > 0.6) then
            --[[/*
             BYTE2(v53) = (signed int)(255.0 - ((lifePercent - 0.5) * 255.0 + (lifePercent - 0.5) * 255.0));
             y = 255 - ((x - 0.5) * 255 + (x - 0.5) * 255)
             y = 255 - 255 * ((x - 0.5) + (x - 0.5))
             y = 255 - 255 * (x - 0.5 + x - 0.5)
             y = 255 - 255 * (2x - 1)
             y = 255 - (510x - 255)
             y = 255 - 510x + 255
             y = 510 - 510x
             */]]
            red = R2I(510 - 510 * lifePercent)
        elseif (lifePercent > 0.3) then
            green = R2I(lifePercent / 0.6 * 255.0)
        else
            green = R2I(lifePercent / 0.8 * 255.0)
        end

        -- show
        if (lifeTextLength > FAKE_BAR_CHARACTERS_LIMIT) then
            return ""
            -- only show life without maximum life
        elseif (fullTextLength > FAKE_BAR_CHARACTERS_LIMIT) then
            return "|cff" .. I2XW(red, 2) .. I2XW(green, 2) .. "00" .. lifeStr .. "|r"
        end

        return "|cff" .. I2XW(red, 2) .. I2XW(green, 2) .. "00" .. lifeStr .. " / " .. maxLifeStr .. "|r"
    end

    local function getHPText (whichUnit)
        if (HIDE_HP_INVULNERABLE_STRUCTURE and IsUnitType(whichUnit, UNIT_TYPE_STRUCTURE) and GetUnitAbilityLevel(whichUnit, FourCC('Avul')) > 0) then
            return ""
        end
        return getHPTextEx(GetUnitState(whichUnit, UNIT_STATE_LIFE), GetUnitState(whichUnit, UNIT_STATE_MAX_LIFE))
    end

    local function getManaTextEx (mana, maxMana)
        local manaStr = I2S(R2I(mana))
        local maxManaStr = I2S(R2I(maxMana))
        local manaTextLength = StringLength(manaStr)
        local fullTextLength = manaTextLength + StringLength(maxManaStr) + 3 -- / and the two spaces

        -- show
        if (manaTextLength > FAKE_BAR_CHARACTERS_LIMIT) then
            return ""
            -- only show mana without maximum mana
        elseif (fullTextLength > FAKE_BAR_CHARACTERS_LIMIT) then
            return "|cffc3dbff" .. manaStr .. "|r"
        end

        return "|cffc3dbff" .. manaStr .. " / " .. maxManaStr .. "|r"
    end

    local function getManaText (whichUnit)
        return getManaTextEx(GetUnitState(whichUnit, UNIT_STATE_MANA), GetUnitState(whichUnit, UNIT_STATE_MAX_MANA))
    end

    -- Use with GetLocalPlayer() only.
    local function updateCinematicScene()
        local selected = SelectionTracker:getMainForLocalPlayer()
        if (selected == nil) then
            BlzFrameSetVisible(playerHPFrame, false)
            BlzFrameSetVisible(playerManaFrame, false)
        else
            if (GetUnitState(selected, UNIT_STATE_MAX_LIFE) > 0.0) then
                BlzFrameSetVisible(playerHPFrame, true)
                BlzFrameSetText(playerHPFrame, getHPText(selected))
            else
                BlzFrameSetVisible(playerHPFrame, false)
            end
            if (GetUnitState(selected, UNIT_STATE_MAX_MANA) > 0.0) then
                BlzFrameSetVisible(playerManaFrame, true)
                BlzFrameSetText(playerManaFrame, getManaText(selected))
            else
                BlzFrameSetVisible(playerManaFrame, false)
            end
        end
    end

    local function timerFunctionUpdateCinematicSceneBars()
        updateCinematicScene()
    end

    local originalFrameHpWidth
    local originalFrameHpHeight

    local originalFrameManaWidth
    local originalFrameManaHeight

    local function readOriginalBarSizes()
        local originalFrameHp = BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT_HP_TEXT, 0)
        local originalFrameMana = BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT_MANA_TEXT, 0)

        originalFrameHpWidth = BlzFrameGetWidth(originalFrameHp)
        originalFrameHpHeight = BlzFrameGetHeight(originalFrameHp)

        originalFrameManaWidth = BlzFrameGetWidth(originalFrameMana)
        originalFrameManaHeight = BlzFrameGetHeight(originalFrameMana)

    end

    local function scaleBars()
        if not originalFrameHpWidth then
            readOriginalBarSizes()
        end

        BlzFrameSetAbsPoint(playerHPFrame, FRAMEPOINT_BOTTOMRIGHT, FAKE_BAR_X + originalFrameHpWidth, FAKE_HP_BAR_Y - originalFrameHpHeight)
        BlzFrameSetAbsPoint(playerManaFrame, FRAMEPOINT_BOTTOMRIGHT, FAKE_BAR_X + originalFrameManaWidth, FAKE_MANA_BAR_Y - originalFrameManaHeight)

        printDebug('scaleBars X: ' .. tostring(FAKE_BAR_WIDTH) .. ' -> ' .. tostring(originalFrameHpWidth))
        printDebug('scaleBars Y: ' .. tostring(FAKE_BAR_HEIGHT) .. ' -> ' .. tostring(originalFrameHpHeight))
    end

    function FakeBars:show(whichSound)
        printDebug('FakeBars:show')
        scaleBars()

        BlzFrameSetVisible(playerHPFrame, true)
        BlzFrameSetVisible(playerManaFrame, true)
        updateCinematicScene()

        PauseTimer(playerAnimationTimer)
        TimerStart(playerAnimationTimer, GetSoundDurationBJ(whichSound), false, timerFunctionEndCinematicSceneBars)
        TimerStart(playerUpdateCinematicSceneBarsTimer, UPDATE_INTERVAL, true, updateCinematicScene)
    end

    local function timerFunctionCreatePlayerPortraits()
        playerHPFrame = BlzCreateFrameByType("Text", "HP", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
        BlzFrameSetAbsPoint(playerHPFrame, FRAMEPOINT_TOPLEFT, FAKE_BAR_X, FAKE_HP_BAR_Y)
        BlzFrameSetAbsPoint(playerHPFrame, FRAMEPOINT_BOTTOMRIGHT, FAKE_BAR_X + FAKE_BAR_WIDTH, FAKE_HP_BAR_Y - FAKE_BAR_HEIGHT)
        BlzFrameSetScale(playerHPFrame, FAKE_BAR_TEXT_SCALE)
        BlzFrameSetTextAlignment(playerHPFrame, TEXT_JUSTIFY_TOP, TEXT_JUSTIFY_CENTER)
        BlzFrameSetVisible(playerHPFrame, false)

        playerManaFrame = BlzCreateFrameByType("Text", "Mana", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
        BlzFrameSetAbsPoint(playerManaFrame, FRAMEPOINT_TOPLEFT, FAKE_BAR_X, FAKE_MANA_BAR_Y)
        BlzFrameSetAbsPoint(playerManaFrame, FRAMEPOINT_BOTTOMRIGHT, FAKE_BAR_X + FAKE_BAR_WIDTH, FAKE_MANA_BAR_Y - FAKE_BAR_HEIGHT)
        BlzFrameSetScale(playerManaFrame, FAKE_BAR_TEXT_SCALE)
        BlzFrameSetTextAlignment(playerManaFrame, TEXT_JUSTIFY_TOP, TEXT_JUSTIFY_CENTER)
        BlzFrameSetVisible(playerManaFrame, false)
    end

    local function triggerConditionGameLoaded()
        timerFunctionCreatePlayerPortraits()
        return false
    end

    local function initFakeBars()
        playerAnimationTimer = CreateTimer()
        playerUpdateCinematicSceneBarsTimer = CreateTimer()

        TriggerRegisterGameEvent(gameLoadedTrigger, EVENT_GAME_LOADED)
        TriggerAddCondition(gameLoadedTrigger, Condition(triggerConditionGameLoaded))

        SimpleUtils.timed(0.0, timerFunctionCreatePlayerPortraits)
    end

    OnInit.final(initFakeBars)
end
if Debug then Debug.endFile() end
