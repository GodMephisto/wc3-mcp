---
type: vendor-doc
source_repo: "https://github.com/nazarpunk/wc3-ujapi"
source_path: "asset/HandleCounter.lua"
ref: HEAD
---

TimerStart(CreateTimer(), 0, false, function()
    DestroyTimer(GetExpiredTimer())

    local gameUi = GetOriginFrame(ORIGIN_FRAME_GAME_UI, 0)
    local menu = CreateFrame('LoadingPlayerSlot', gameUi, 0, 0)
    SetFrameSize(menu, .16, .03)

    local FRAMEPOINT_BOTTOM = ConvertFramePointType(7);
    SetFrameRelativePoint(menu, FRAMEPOINT_BOTTOM, gameUi, FRAMEPOINT_BOTTOM, 0, 0.16)

    local left = GetFrameByName('LoadingPlayerSlotName', 0)
    local right = GetFrameByName('LoadingPlayerSlotRace', 0)

    local seconds = 0
    local period = .25

    TimerStart(CreateTimer(), period, true, function()
        seconds = seconds + period;
        SetFrameText(left, tostring(GetHandleCount()))
        SetFrameText(right, tostring(math.floor(seconds)))
    end)
end)