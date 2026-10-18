---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/tower_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Tower Manager                                       |
    | =================================================== |
    | Manages all the different towers.                   |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.TowerManager = XTD.Systems.TowerManager or {}

XTD.Systems.TowerManager.spellTowers = {}
XTD.Systems.TowerManager.towerSellRefundRate = 0.75
XTD.Systems.TowerManager.sellTowerAbilityId = FourCC("A000")
XTD.Systems.TowerManager.sellTowerEffect = "Abilities\\Spells\\Human\\MassTeleport\\MassTeleportTarget.mdl"
XTD.Systems.TowerManager.expensiveTowerThreshold = 1000
XTD.Systems.TowerManager.confirmSellTimeout = 5.00
XTD.Systems.TowerManager.pendingSellConfirmations = XTD.Systems.TowerManager.pendingSellConfirmations or {}

--[[
    Initialize the Tower Manager.
]]
function XTD.Systems.TowerManager.Init()
    XTD.Core.Debug("TowerManager", "Initializing the Tower Manager...")

    XTD.Systems.TowerManager.RegisterTowerBuildTriggers()
    XTD.Systems.TowerManager.Start()
    XTD.Systems.TowerManager.InitSellTrigger()

    XTD.Core.Debug("TowerManager", "Tower Manager initialized.")
end

--[[
    Get the full gold value for a tower.
]]
function XTD.Systems.TowerManager.GetTowerValue(tower)
    if tower == nil then
        return 0
    end

    local unitTypeId = GetUnitTypeId(tower)

    for unitType, value in pairs(XTD.Data.Towers) do
        if unitTypeId == unitType then
            return value.gold
        end
    end

    return 0
end

--[[
    Gets the confirmation key for a player and tower.
]]
function XTD.Systems.TowerManager.GetSellConfirmationKey(player, tower)
    return GetPlayerId(player) .. ":" .. tostring(GetHandleId(tower))
end

--[[
    Checks whether a tower sell is waiting for confirmation.
]]
function XTD.Systems.TowerManager.HasSellConfirmation(player, tower)
    local key = XTD.Systems.TowerManager.GetSellConfirmationKey(player, tower)

    return XTD.Systems.TowerManager.pendingSellConfirmations[key] == true
end

--[[
    Starts the sell confirmation window for an expensive tower.
]]
function XTD.Systems.TowerManager.StartSellConfirmation(player, tower, refund)
    local key = XTD.Systems.TowerManager.GetSellConfirmationKey(player, tower)

    XTD.Systems.TowerManager.pendingSellConfirmations[key] = true

    XTD.UI.MessageManager.Player(
        player,
        XTD.Core.Color.Yellow("This is an expensive tower. Cast Sell Tower again within ") ..
        XTD.Core.Color.BrightGreen(tostring(math.floor(XTD.Systems.TowerManager.confirmSellTimeout))) ..
        XTD.Core.Color.Yellow(" seconds to confirm selling it for ") ..
        XTD.Core.Color.BrightGreen(tostring(refund)) ..
        XTD.Core.Color.Yellow(" gold.")
    )

    local timer = CreateTimer()

    TimerStart(timer, XTD.Systems.TowerManager.confirmSellTimeout, false, function ()
        XTD.Systems.TowerManager.pendingSellConfirmations[key] = nil
        DestroyTimer(timer)
    end)
end

--[[
    Clears the sell confirmation for a player and tower.
]]
function XTD.Systems.TowerManager.ClearSellConfirmation(player, tower)
    local key = XTD.Systems.TowerManager.GetSellConfirmationKey(player, tower)

    XTD.Systems.TowerManager.pendingSellConfirmations[key] = nil
end

--[[
    Shows floating gold refund text above the sold tower.
]]
function XTD.Systems.TowerManager.ShowRefundText(player, x, y, refund)
    local tag = CreateTextTag()

    SetTextTagText(tag, "+" .. tostring(refund) .. " Gold", 0.024)
    SetTextTagPos(tag, x, y, 80.00)
    SetTextTagColor(tag, 255, 220, 0, 255)
    SetTextTagVelocity(tag, 0.00, 0.035)
    SetTextTagVisibility(tag, GetLocalPlayer() == player)
    SetTextTagPermanent(tag, false)
    SetTextTagLifespan(tag, 2.00)
    SetTextTagFadepoint(tag, 1.25)
end

--[[
    Sells a tower and refunds the owner 75% of its full value.
]]
function XTD.Systems.TowerManager.SellTower(tower)
    if tower == nil then
        return
    end

    local owner = GetOwningPlayer(tower)
    local value = XTD.Systems.TowerManager.GetTowerValue(tower)

    if value <= 0 then
        XTD.UI.MessageManager.Player(
            owner,
            XTD.Core.Color.Red("This tower cannot be sold.")
        )

        return
    end

    local refund = XTD.Systems.TowerManager.GetTowerRefund(tower)

    XTD.Core.PlayerManager.AddResource(owner, "gold", refund)

    local x = GetUnitX(tower)
    local y = GetUnitY(tower)

    DestroyEffect(AddSpecialEffect(XTD.Systems.TowerManager.sellTowerEffect, x, y))

    XTD.Systems.TowerManager.ShowRefundText(owner, x, y, refund)

    XTD.UI.MessageManager.Player(
        owner,
        XTD.Core.Color.CoolBlue("Tower sold! ") ..
        XTD.Core.Color.Yellow("Refunded ") ..
        XTD.Core.Color.BrightGreen(tostring(refund)) ..
        XTD.Core.Color.Yellow(" gold.")
    )

    local playerData = XTD.Core.PlayerManager.GetPlayer(owner)

    XTD.Systems.TowerManager.ClearSellConfirmation(owner, tower)

    RemoveUnit(tower)
end

--[[
    Handles a sell tower cast.
]]
function XTD.Systems.TowerManager.HandleSellTowerCast(caster, tower)
    if caster == nil or tower == nil then
        return
    end

    local player = GetOwningPlayer(caster)
    local towerOwner = GetOwningPlayer(tower)

    if player ~= towerOwner then
        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.Red("You can only sell your own towers.")
        )

        return
    end

    local value = XTD.Systems.TowerManager.GetTowerValue(tower)

    if value <= 0 then
        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.Red("This tower cannot be sold.")
        )

        return
    end

    local refund = XTD.Systems.TowerManager.GetTowerRefund(tower)

    if value >= XTD.Systems.TowerManager.expensiveTowerThreshold and
        not XTD.Systems.TowerManager.HasSellConfirmation(player, tower) then
            XTD.Systems.TowerManager.StartSellConfirmation(player, tower, refund)
            return
        end

    XTD.Systems.TowerManager.SellTower(tower)
end

--[[
    Initialize the sell trigger.
]]
function XTD.Systems.TowerManager.InitSellTrigger()
    local trigger = CreateTrigger()

    for i = 0, 7 do
        TriggerRegisterPlayerUnitEvent(
            trigger,
            Player(i),
            EVENT_PLAYER_UNIT_SPELL_EFFECT,
            nil
        )
    end

    TriggerAddAction(trigger, function ()
        local abilityId = GetSpellAbilityId()

        if abilityId ~= XTD.Systems.TowerManager.sellTowerAbilityId then
            return
        end

        local caster = GetTriggerUnit()

        XTD.Systems.TowerManager.HandleSellTowerCast(caster, caster)
    end)
end

--[[
    Register a spell tower.
]]
function XTD.Systems.TowerManager.RegisterTower(tower)
    XTD.Systems.TowerManager.spellTowers[tower] = nil

    local unitType = GetUnitTypeId(tower)
    local towers = XTD.Constants.SPELL_TOWERS

    if unitType == towers.COLD.FROSTGUARD_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "blizzard",
            type = "point",
            channeled = true,
            cooldown = 8.00,
            nextCast = 0.00
        }
    end

    if unitType == towers.COLD.ICEWATCH_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "blizzard",
            type = "point",
            channeled = true,
            cooldown = 7.00,
            nextCast = 0.00
        }
    end

    if unitType == towers.COLD.WINTERHOLD_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "blizzard",
            type = "point",
            channeled = true,
            cooldown = 6.00,
            nextCast = 0.00
        }
    end

    if unitType == towers.COLD.BLIZZARDSPIRE_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "blizzard",
            type = "point",
            channeled = true,
            cooldown = 5.00,
            nextCast = 0.00
        }
    end

    if unitType == towers.INFERNO.EMBER_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "breathoffire",
            type = "point",
            channeled = false
        }
    end

    if unitType == towers.INFERNO.FLAMEGUARD_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "breathoffire",
            type = "point",
            channeled = false
        }
    end

    if unitType == towers.INFERNO.INFERNO_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "breathoffire",
            type = "point",
            channeled = false
        }
    end

    if unitType == towers.INFERNO.HELLSPIRE_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "breathoffire",
            type = "point",
            channeled = false
        }
    end

    if unitType == towers.ARCANE.RUNIC_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "clusterrockets",
            type = "point",
            channeled = false
        }
    end

    if unitType == towers.ARCANE.ARCANE_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "clusterrockets",
            type = "point",
            channeled = false
        }
    end

    if unitType == towers.ARCANE.SPELLWEAVER_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "clusterrockets",
            type = "point",
            channeled = false
        }
    end

    if unitType == towers.ARCANE.ASTRAL_SPIRE then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "clusterrockets",
            type = "point",
            channeled = false
        }
    end

    if unitType == towers.STORM.SPARK_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "chainlightning",
            type = "target",
            channeled = false
        }
    end

    if unitType == towers.STORM.STORMWATCH_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "chainlightning",
            type = "target",
            channeled = false
        }
    end

    if unitType == towers.STORM.TEMPEST_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "chainlightning",
            type = "target",
            channeled = false
        }
    end

    if unitType == towers.STORM.THUNDERSPIRE_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "chainlightning",
            type = "target",
            channeled = false
        }
    end

    if unitType == towers.SHADOW.SHADE_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "immolation",
            type = "immediate",
            channeled = false
        }
    end

    if unitType == towers.SHADOW.DREADWATCH_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "immolation",
            type = "immediate",
            channeled = false
        }
    end

    if unitType == towers.SHADOW.SHADOWKEEP_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "immolation",
            type = "immediate",
            channeled = false
        }
    end

    if unitType == towers.SHADOW.DOOMSPIRE_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "immolation",
            type = "immediate",
            channeled = false
        }
    end
    
    if unitType == towers.CANNON.BOMBARD_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "earthquake",
            type = "point",
            cooldown = 20.00,
            nextCast = 0.00,
            channeled = true
        }
    end

    if unitType == towers.CANNON.WAR_CANNON then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "earthquake",
            type = "point",
            cooldown = 18.00,
            nextCast = 0.00,
            channeled = true
        }
    end

    if unitType == towers.CANNON.SIEGE_CANNON then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "earthquake",
            type = "point",
            cooldown = 16.00,
            nextCast = 0.00,
            channeled = true
        }
    end

    if unitType == towers.CANNON.CATACLYSM_TOWER then
        XTD.Systems.TowerManager.spellTowers[tower] = {
            order = "earthquake",
            type = "point",
            cooldown = 14.00,
            nextCast = 0.00,
            channeled = true
        }
    end
end

--[[
    Get the spell search range for a tower.
    Uses custom range if provided, otherwise uses the unit's acquisition range.
]]
function XTD.Systems.TowerManager.GetSpellRange(tower, data)
    if data ~= nil and data.range ~= nil then
        return data.range
    end

    return BlzGetUnitRealField(
        tower,
        UNIT_RF_ACQUISITION_RANGE
    )
end

--[[
    Get the nearest enemy for the given range.
]]
function XTD.Systems.TowerManager.GetNearestEnemy(tower, range)
    local group = CreateGroup()
    local towerX = GetUnitX(tower)
    local towerY = GetUnitY(tower)
    local owner = GetOwningPlayer(tower)

    local nearest = nil
    local nearestDist = 999999

    GroupEnumUnitsInRange(group, towerX, towerY, range, nil)

    ForGroup(group, function ()
        local unit = GetEnumUnit()

        if IsUnitEnemy(unit, owner) and GetWidgetLife(unit) > 0.405 and not IsUnitType(unit, UNIT_TYPE_STRUCTURE) then
            local dx = GetUnitX(unit) - towerX
            local dy = GetUnitY(unit) - towerY
            local dist = dx * dx + dy * dy
            
            if dist < nearestDist then
                nearestDist = dist
                nearest = unit
            end
        end
    end)

    DestroyGroup(group)

    return nearest
end

--[[
    Start the periodic tower magic cast loop.
]]
function XTD.Systems.TowerManager.Start()
    local timer = CreateTimer()

    TimerStart(timer, 0.50, true, function ()
        local now = TimerGetElapsed(XTD.Core.gameTimer)

        for tower, data in pairs(XTD.Systems.TowerManager.spellTowers) do
            if GetWidgetLife(tower) > 0.405 then
                local canCast = true

                if data.channeled == true then
                    canCast = data.nextCast == nil or now >= data.nextCast
                end

                if canCast then
                    local range = XTD.Systems.TowerManager.GetSpellRange(tower, data)

                    local target = XTD.Systems.TowerManager.GetNearestEnemy(
                        tower,
                        range
                    )

                    if target ~= nil then
                        local issued = false

                        if data.type == "target" then
                            issued = IssueTargetOrder(tower, data.order, target)

                        elseif data.type == "point" then
                            issued = IssuePointOrder(
                                tower,
                                data.order,
                                GetUnitX(target),
                                GetUnitY(target)
                            )

                        elseif data.type == "immediate" then
                            issued = IssueImmediateOrder(tower, data.order)
                        end

                        if issued and data.channeled == true then
                            data.nextCast = now + data.cooldown
                        end
                    end
                end
            else
                XTD.Systems.TowerManager.spellTowers[tower] = nil
            end
        end
    end)
end

--[[
    Register the tower building triggers.
]]
function XTD.Systems.TowerManager.RegisterTowerBuildTriggers()
    local trigger = CreateTrigger()

    for i = 0, 7 do
        TriggerRegisterPlayerUnitEvent(trigger, Player(i), EVENT_PLAYER_UNIT_CONSTRUCT_FINISH, nil)
        TriggerRegisterPlayerUnitEvent(trigger, Player(i), EVENT_PLAYER_UNIT_UPGRADE_FINISH, nil)
    end

    TriggerAddAction(trigger, function ()
        local tower = GetTriggerUnit()

        if tower == nil then
            tower = GetConstructedStructure()
        end

        XTD.Systems.TowerManager.RegisterTower(tower)
    end)
end

--[[
    Get the tower data.
]]
function XTD.Systems.TowerManager.GetTowerData(tower)
    if tower == nil then
        return nil
    end

    return XTD.Data.Towers[GetUnitTypeId(tower)]
end

--[[
    Get the tower refund rate.
]]
function XTD.Systems.TowerManager.GetTowerRefundRate(tower)
    local towerData = XTD.Systems.TowerManager.GetTowerData(tower)

    if towerData == nil or towerData.tier == nil then
        return 0.75
    end

    return math.max(0.00, 1.10 - (towerData.tier * 0.10))
end

--[[
    Get the tower refund.
]]
function XTD.Systems.TowerManager.GetTowerRefund(tower)
    local value = XTD.Systems.TowerManager.GetTowerValue(tower)
    local rate = XTD.Systems.TowerManager.GetTowerRefundRate(tower)

    return math.floor(value * rate)
end