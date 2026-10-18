---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "ui/cinematic_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Cinematic Manager                                   |
    | =================================================== |
    | Manages the cinematic flow.                         |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.UI.CinematicManager = XTD.UI.CinematicManager or {}

XTD.UI.CinematicManager.points = {
    logo = {
        x = 0,
        y = 10235
    },

    waveDisplayLeft = {
        x = -10260,
        y = -10994
    },

    waveDisplayRight = {
        x = 10260,
        y = -10994
    }
}

--[[
    Initialize the Cinematic Manager.
]]
function XTD.UI.CinematicManager.Init()
    XTD.Core.Debug("CinematicManager", "Initializing the Cinematic Manager...")
    XTD.Core.Debug("CinematicManager", "Cinematic Manager initialized.")
end

--[[
    Start the game intro sequence.
]]
function XTD.UI.CinematicManager.StartIntro()
    local camera = XTD.UI.CameraManager
    local points = XTD.UI.CinematicManager.points

    XTD.UI.MessageManager.Broadcast("\n\n\n\n\n\n\n")

    camera.HideUI()

    -- Start with black screen
    camera.FadeOut(0.00)

    -- Now we show the XTD terrain logo, zoomed in
    XTD.Core.Timer.After(0.50, function ()
        camera.ApplyForAll(points.logo.x, points.logo.y, 900.00, 0.00)
        camera.FadeIn(1.50)
    end)

    -- Now zoom out until the full XTD logo is visible
    XTD.Core.Timer.After(2.00, function ()
        camera.ApplyForAll(points.logo.x, points.logo.y, 2600.00, 4.00)
    end)

    -- Now we fade out
    XTD.Core.Timer.After(6.50, function ()
        camera.FadeOut(1.00)
    end)

    -- Now fade in at the left side of the creep waves display
    XTD.Core.Timer.After(8.00, function ()
        camera.ApplyForAll(points.waveDisplayLeft.x, points.waveDisplayLeft.y, 2200.00, 0.00)
        camera.FadeIn(1.00)
    end)

    -- Pan across creep display
    XTD.Core.Timer.After(9.25, function ()
        camera.ApplyForAll(points.waveDisplayRight.x, points.waveDisplayRight.y, 2200.00, 14.00)
    end)

    -- Fade out after panning
    XTD.Core.Timer.After(23.75, function ()
        camera.FadeOut(1.00)
    end)

    -- Return to player start locations
    XTD.Core.Timer.After(25.25, function ()
        camera.ApplyPlayerStartCameras(0.00)
        camera.ShowUI()
        camera.FadeIn(1.00)
    end)

    XTD.Core.Timer.After(27.00, function ()
        XTD.Core.GameManager.StartGame()
    end)
end