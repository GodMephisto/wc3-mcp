---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "Libs/src/FrameUtils.lua"
ref: HEAD
---

if Debug then Debug.beginFile "FrameUtils" end
FrameUtils = {
    FRAME_POINTS = { -- shorthand 'FRAMEPOINT' for terser code.
        tl = FRAMEPOINT_TOPLEFT,
        t = FRAMEPOINT_TOP,
        tr = FRAMEPOINT_TOPRIGHT,
        r = FRAMEPOINT_RIGHT,
        br = FRAMEPOINT_BOTTOMRIGHT,
        b = FRAMEPOINT_BOTTOM,
        bl = FRAMEPOINT_BOTTOMLEFT,
        l = FRAMEPOINT_LEFT,
        c = FRAMEPOINT_CENTER,
    }
}

-- @bool = true to fade out (hide); false to fade in (show).
function FrameUtils.fadeFrame(fade, frame, duration)
    BlzFrameSetVisible(frame, true)
    local bool = fade
    local alpha = 255
    local int = math.floor(255 / math.floor(duration / 0.03))
    -- show:
    if bool then
        BlzFrameSetVisible(frame, true)
        BlzFrameSetAlpha(frame, 255)
        SimpleUtils.timedRepeat(0.03, nil, function(timer)
            if BlzFrameGetAlpha(frame) > 0 and BlzFrameGetAlpha(frame) > int then
                alpha = alpha - int
                BlzFrameSetAlpha(frame, alpha)
            else
                BlzFrameSetAlpha(frame, 0)
                BlzFrameSetVisible(frame, false)
                SimpleUtils.releaseTimer(timer)
            end
        end)
        -- hide:
    else
        BlzFrameSetVisible(frame, true)
        BlzFrameSetAlpha(frame, 0)
        SimpleUtils.timedRepeat(0.03, nil, function(timer)
            if BlzFrameGetAlpha(frame) ~= 255 and BlzFrameGetAlpha(frame) < 255 - int then
                alpha = alpha + int
                BlzFrameSetAlpha(frame, alpha)
            else
                BlzFrameSetAlpha(frame, 255)
                BlzFrameSetVisible(frame, true)
                SimpleUtils.releaseTimer(timer)
            end
        end)
    end
end

local function getOffset(offset)
    if offset == 0 then
        return ""
    end
    return string.rep("-", 2 * offset)
end

function FrameUtils.printFrameStructure(frame, maxDepth, offset)
    if offset == nil then
        offset = 0
    end
    local childrenCount = BlzFrameGetChildrenCount(frame)
    local visible = BlzFrameIsVisible(frame)
    if visible then
        print(getOffset(offset) .. "FRAME " .. BlzFrameGetName(frame) .. ": [" .. tostring(childrenCount) .. "]")
    else
        print(getOffset(offset) .. "[INVISIBLE] FRAME " .. BlzFrameGetName(frame) .. ": [" .. tostring(childrenCount) .. "]")
    end
    if childrenCount > 0 then
        if offset < maxDepth then
            for i = 0, childrenCount - 1 do
                FrameUtils.printFrameStructure(BlzFrameGetChild(frame, i), maxDepth, offset + 1)
            end
        else
            print(getOffset(offset + 1) .. "[...]")
        end
    end
end

function FrameUtils.getChildByName(parent, expectedChildName)
    local childrenCount = BlzFrameGetChildrenCount(parent)
    for i = 0, childrenCount - 1 do
        local child = BlzFrameGetChild(parent, i)
        local childName = BlzFrameGetName(child)
        if childName == expectedChildName then
            return child
        end
    end
    SimpleUtils.printWarn('No child ' .. expectedChildName .. ' found for parent')
end

--BlzFrameGetChild(index) for index outside of the parent frame's child array size crashes the game.
--Added this function following hours-long debugging of a crash introduced by a patch in #$*$ Reforged
function FrameUtils.safeFrameGetChild(parent, childIndex)
    local childrenCount = BlzFrameGetChildrenCount(parent)
    if childIndex < 0 or childIndex >= childrenCount then
        local name = BlzFrameGetName(parent)
        SimpleUtils.printWarn("Attempt to get " .. tostring(childIndex) .. ". child, but frame " .. name .. " only has " .. tostring(childrenCount))
        return nil
    end
    return BlzFrameGetChild(parent, childIndex)
end

function FrameUtils.fixFocus(fh)
    BlzFrameSetEnable(fh, false)
    BlzFrameSetEnable(fh, true)
end

function FrameUtils.displayImage(image, centerX, centerY, width, height)
    -- 1. Create a container frame that handles the positioning
    local container = BlzCreateFrameByType("FRAME", "TitleContainer", BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), "", 0)
    -- 2. Center the container on the screen
    BlzFrameSetAbsPoint(container, FRAMEPOINT_CENTER, centerX, centerY)

    -- 3. Set a strict square size (0.2 width by 0.2 height)
    -- Increase or decrease both numbers equally to change the size (e.g., 0.3, 0.3)
    BlzFrameSetSize(container, width, height)

    -- 4. Create the actual image texture frame inside the container
    local titleImage = BlzCreateFrameByType("BACKDROP", "TitleImage", container, "", 0)

    -- 5. Pin the texture perfectly to all four edges of our square container
    BlzFrameSetAllPoints(titleImage, container)

    -- 6. Apply your imported BLP texture
    BlzFrameSetTexture(titleImage, image, 0, true)

    FrameUtils.fadeFrame(false, container, 2.0)

    SimpleUtils.timed(8.0, function()
        FrameUtils.fadeFrame(true, container, 2.0)
    end)
end

function FrameUtils.displayImageCentered(image, width, height)
    FrameUtils.displayImage(image, 0.4, 0.3, width, height)
end

function FrameUtils.displayImageCutsceneCentered(image, width, height)
    FrameUtils.displayImage(image, 0.4, 0.35, width, height)
end

if Debug then Debug.endFile() end
