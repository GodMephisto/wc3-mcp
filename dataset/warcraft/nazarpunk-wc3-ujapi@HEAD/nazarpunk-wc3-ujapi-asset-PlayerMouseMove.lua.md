---
type: vendor-doc
source_repo: "https://github.com/nazarpunk/wc3-ujapi"
source_path: "asset/PlayerMouseMove.lua"
ref: HEAD
---

PlayerMouseWorld = {}
PlayerMouseScreen = {}

local trig = CreateTrigger()
for i = 0, bj_MAX_PLAYER_SLOTS - 1, 1 do
    table.insert(PlayerMouseWorld, { 0, 0, 0 })
    table.insert(PlayerMouseScreen, { 0, 0 })
    TriggerRegisterPlayerEvent(trig, Player(i), ConvertPlayerEvent(307)) -- EVENT_PLAYER_MOUSE_MOVE
end

TriggerAddAction(trig, function()
    local id = GetPlayerId(GetTriggerPlayer())
    local pmw = PlayerMouseWorld[id]
    pmw[1] = GetTriggerPlayerMouseWorldX()
    pmw[2] = GetTriggerPlayerMouseWorldY()
    pmw[3] = GetTriggerPlayerMouseWorldZ()

    local pms = PlayerMouseScreen[id]
    pms[1] = GetTriggerPlayerMouseScreenX()
    pms[2] = GetTriggerPlayerMouseScreenY()
end)