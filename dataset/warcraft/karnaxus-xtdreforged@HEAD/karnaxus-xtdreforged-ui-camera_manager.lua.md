---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "ui/camera_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Camera Manager                                      |
    | =================================================== |
    | Manages the game camera.                            |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.UI.CameraManager = XTD.UI.CameraManager or {}

XTD.UI.CameraManager.DEFAULT_Z_OFFSET = 1650.00
XTD.UI.CameraManager.CINEMATIC_Z_OFFSET = 2200.00

--[[
    Initialize the Camera Manager.
]]
function XTD.UI.CameraManager.Init()
    XTD.Core.Debug("CameraManager", "Initializing the Camera Manager...")
    XTD.Core.Debug("CameraManager", "Camera Manager initialized.")
end

--[[
    Apply the camera for all players.
]]
function XTD.UI.CameraManager.ApplyForAll(x, y, zOffset, duration)
    XTD.Core.PlayerManager.ForEach(function (playerData)
        local p = playerData.player

        if GetLocalPlayer() == p then
            PanCameraToTimed(x, y, duration or 0.00)
            SetCameraField(CAMERA_FIELD_TARGET_DISTANCE, zOffset or XTD.UI.CameraManager.DEFAULT_Z_OFFSET, duration or 0.00)
            SetCameraField(CAMERA_FIELD_ANGLE_OF_ATTACK, 304.00, duration or 0.00)
            SetCameraField(CAMERA_FIELD_ROTATION, 90.00, duration or 0.00)
        end
    end)
end

--[[
    Fade out the camera.
]]
function XTD.UI.CameraManager.FadeOut(duration)
    CinematicFadeBJ(
        bj_CINEFADETYPE_FADEOUT,
        duration or 1.00,
        "ReplaceableTextures\\CameraMasks\\Black_mask.blp",
        0, 0, 0, 0
    )
end

--[[
    Fade in the camera.
]]
function XTD.UI.CameraManager.FadeIn(duration)
    CinematicFadeBJ(
        bj_CINEFADETYPE_FADEIN,
        duration or 1.00,
        "ReplaceableTextures\\CameraMasks\\Black_mask.blp",
        0, 0, 0, 0
    )
end

--[[
    Apply the player start cameras for each player.
]]
function XTD.UI.CameraManager.ApplyPlayerStartCameras(duration)
    XTD.Core.PlayerManager.ForEach(function (playerData)
        local p = playerData.player
        local start = XTD.Core.PlayerManager.GetPlayerStartLocation(p)

        if start == nil then
            XTD.Core.Debug(
                "CameraManager",
                "Start location is nil for " .. GetPlayerName(p),
                "ApplyPlayerStartCameras"
            )
            return
        end

        XTD.Core.Debug(
            "CameraManager",
            "Applying start camera for " .. GetPlayerName(p) ..
            " at x=" .. tostring(start.x) ..
            ", y=" .. tostring(start.y),
            "ApplyPlayerStartCameras"
        )

        if GetLocalPlayer() == p then
            PanCameraToTimed(start.x, start.y, duration or 0.00)
            SetCameraField(CAMERA_FIELD_TARGET_DISTANCE, XTD.UI.CameraManager.DEFAULT_Z_OFFSET, duration or 0.00)
            SetCameraField(CAMERA_FIELD_ANGLE_OF_ATTACK, 304.00, duration or 0.00)
            SetCameraField(CAMERA_FIELD_ROTATION, 90.00, duration or 0.00)
        end
    end)
end

--[[
    Hide the game UI for all players.
]]
function XTD.UI.CameraManager.HideUI()
    XTD.Core.PlayerManager.ForEach(function(playerData)
        if GetLocalPlayer() == playerData.player then
            BlzHideOriginFrames(true)
            EnableUserUI(false)
        end
    end)
end

--[[
    Show the game UI for all players.
]]
function XTD.UI.CameraManager.ShowUI()
    XTD.Core.PlayerManager.ForEach(function(playerData)
        if GetLocalPlayer() == playerData.player then
            BlzHideOriginFrames(false)
            EnableUserUI(true)
        end
    end)
end
