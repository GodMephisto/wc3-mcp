---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "Libs/src/SelectionTracker.lua"
ref: HEAD
---

if Debug then Debug.beginFile "SelectionTracker" end
do
    local loadBugTrigger = CreateTrigger()
    local containerFrame = nil
    local groupFrame = nil
    local group = CreateGroup()
    local units = {} -- unit array
    local unitsCount = 0
    local selectedUnitsOrderedFilter -- filterfunc

    SelectionTracker = {
        debug = false
    }

    local function printDebug(msg)
        if SelectionTracker.debug and SimpleUtils.globalDebug then
            print("[SELECTION TRACKER] " .. msg)
        end
    end

    local function GetUnitOrderValue (u)
        --heroes use the handleId
        if IsUnitType(u, UNIT_TYPE_HERO) then
            return GetHandleId(u)
        else
            --units use unitCode
            return GetUnitTypeId(u)
        end
    end

    local function selectionTrackerFilterFunction()
        local u = GetFilterUnit()
        local prio = BlzGetUnitRealField(u, UNIT_RF_PRIORITY)
        local found = false
        printDebug("FilterFunction: unit count: " .. unitsCount)
        -- compare the current u with already found, to place it in the right slot
        for loopA = 1, unitsCount do
            local priority = BlzGetUnitRealField(units[loopA], UNIT_RF_PRIORITY)
            printDebug("FilterFunction: unit " .. GetUnitName(u) .. ": priority: " .. priority .. ", unit order value " .. GetUnitOrderValue(units[loopA]))
            if priority < prio or (priority == prio and GetUnitOrderValue(units[loopA]) > GetUnitOrderValue(u)) then
                unitsCount = unitsCount + 1
                for loopB = unitsCount, loopA + 1, -1 do
                    units[loopB] = units[loopB - 1]
                end
                units[loopA] = u
                printDebug("FilterFunction: found - unit count: " .. tostring(unitsCount))
                found = true
                break
            end
        end

        -- not found add it at the end
        if not found then
            unitsCount = unitsCount + 1
            units[unitsCount] = u
        end

        printDebug("FilterFunction: NOT found - unit count: " .. tostring(unitsCount))

        u = nil
        return false
    end

    local function getSelectedUnitIndex()
        -- local player is in group selection?
        if not BlzFrameIsVisible(containerFrame) then
            return -1
        end

        local groupSubFrame = FrameUtils.safeFrameGetChild(groupFrame, 0)
        local selectedUnitFrameCount = BlzFrameGetChildrenCount(groupSubFrame)
        local selectedUnitFrame
        local selectedUnitHighlightFrame
        for i = 0, selectedUnitFrameCount - 1 do
            selectedUnitFrame = FrameUtils.safeFrameGetChild(groupSubFrame, i)
            selectedUnitHighlightFrame = FrameUtils.safeFrameGetChild(selectedUnitFrame, 0)
            if BlzFrameIsVisible(selectedUnitHighlightFrame) then
                printDebug("GetSelectedUnitIndex: " .. tostring(i))
                return i
            end
        end

        return -1
    end

    local function getSelectedUnitByIndex(whichPlayer, index)
        printDebug("GetMainSelectedUnit: " .. tostring(index))
        GroupClear(group)
        if index >= 0 then
            GroupEnumUnitsSelected(group, whichPlayer, selectedUnitsOrderedFilter)
            local unit = units[index + 1]
            units = {}
            unitsCount = 0
            return unit
        else
            GroupEnumUnitsSelected(group, whichPlayer, nil)
            return FirstOfGroup(group)
        end
    end

    --the local current main selected unit, using it in a sync gamestate relevant manner breaks the game.
    function SelectionTracker:getMainForLocalPlayer()
        return getSelectedUnitByIndex(GetLocalPlayer(), getSelectedUnitIndex())
    end

    local function initFrames()
        console = BlzGetFrameByName("ConsoleUI", 0)
        bottomUI = FrameUtils.safeFrameGetChild(console, 1)
        containerFrame = FrameUtils.safeFrameGetChild(bottomUI, 2)
        groupFrame = FrameUtils.safeFrameGetChild(containerFrame, 5)
    end

    local function initSelectionTracker()
        selectedUnitsOrderedFilter = Filter(selectionTrackerFilterFunction)
        SimpleUtils.timed(0, initFrames)
        TriggerRegisterGameEvent(loadBugTrigger, EVENT_GAME_LOADED)
        TriggerAddAction(loadBugTrigger, initFrames)
    end

    OnInit.final(initSelectionTracker)
end
if Debug then Debug.endFile() end
