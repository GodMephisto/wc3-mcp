---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/src/Screenplays/ScreenplayUtils.lua"
ref: HEAD
---

if Debug then Debug.beginFile "ScreenplayUtils" end
do
    -- utility class with functions you can use in your screenplays
    ScreenplayUtils = {
    }

    local function printDebug(msg)
        if ScreenplaySystem.debug and SimpleUtils.globalDebug then
            print("[SCREENPLAY UTILS] " .. msg)
        end
    end
    
    local customCamera = CreateCameraSetup()

    local function copyCameraField(from, to, field)
        CameraSetupSetField(to, field, CameraSetupGetField(from, field), 0.0)
    end
    
    local function copyCameraSetup(from, to)
        copyCameraField(from, to, CAMERA_FIELD_ZOFFSET)
        copyCameraField(from, to, CAMERA_FIELD_ROTATION)
        copyCameraField(from, to, CAMERA_FIELD_ANGLE_OF_ATTACK)
        copyCameraField(from, to, CAMERA_FIELD_TARGET_DISTANCE)
        copyCameraField(from, to, CAMERA_FIELD_ROLL)
        copyCameraField(from, to, CAMERA_FIELD_FIELD_OF_VIEW)
        copyCameraField(from, to, CAMERA_FIELD_FARZ)
        copyCameraField(from, to, CAMERA_FIELD_NEARZ)
        copyCameraField(from, to, CAMERA_FIELD_LOCAL_PITCH)
        copyCameraField(from, to, CAMERA_FIELD_LOCAL_YAW)
        copyCameraField(from, to, CAMERA_FIELD_LOCAL_ROLL)
        CameraSetupSetDestPosition(to, CameraSetupGetDestPositionX(from), CameraSetupGetDestPositionY(from), 0.0)
    end

    local function fixCameraIfNeeded(cameraFromX, cameraToX, cameraFromY, cameraToY, cameraFromDistance, cameraToDistance, cameraTo, durationInt, durationLeft)
        local fraction = (durationInt - durationLeft) / durationInt
        local interpolatedX = ScreenplayUtils.interpolate(cameraFromX, cameraToX, fraction)
        local interpolatedY = ScreenplayUtils.interpolate(cameraFromY, cameraToY, fraction)
        local interpolatedDistance = ScreenplayUtils.interpolate(cameraFromDistance, cameraToDistance, fraction)

        local sceneCameraX = GetCameraTargetPositionX()
        local sceneCameraY = GetCameraTargetPositionY()
        local sceneCameraDistance = GetCameraField(CAMERA_FIELD_TARGET_DISTANCE)
        local ERROR_MARGIN = 5.0
        local fixed = false
        if math.abs(sceneCameraX - interpolatedX) > ERROR_MARGIN or math.abs(sceneCameraY - interpolatedY) > ERROR_MARGIN then
            printDebug("interpolateCamera: " .. tostring(interpolatedX) .. ", " .. tostring(interpolatedY))
            printDebug("sceneCamera: " .. tostring(sceneCameraX) .. ", " .. tostring(sceneCameraY))
            printDebug("fixing camera pos")
            SetCameraPosition(interpolatedX, interpolatedY)
            fixed = true
        end
        if math.abs(sceneCameraDistance - interpolatedDistance) > ERROR_MARGIN then
            printDebug("fixing camera distance" .. tostring(sceneCameraDistance) .. " -> " .. interpolatedDistance)
            SetCameraField(CAMERA_FIELD_TARGET_DISTANCE, interpolatedDistance, 0.0)
            fixed = true
        end
        if fixed == true then
            CameraSetupApplyForceDuration(cameraTo, true, durationLeft)
        end
    end

    local function interpolateCamera(cameraFrom, cameraTo, duration)
        ScreenplayUtils.clearInterpolation()
        CameraSetupApply(cameraFrom, true)
        local cameraFromX = CameraSetupGetDestPositionX(cameraFrom)
        local cameraFromY = CameraSetupGetDestPositionY(cameraFrom)
        local cameraFromDistance = CameraSetupGetField(cameraFrom, CAMERA_FIELD_TARGET_DISTANCE)

        local cameraToX = CameraSetupGetDestPositionX(cameraTo)
        local cameraToY = CameraSetupGetDestPositionY(cameraTo)
        local cameraToDistance = CameraSetupGetField(cameraTo, CAMERA_FIELD_TARGET_DISTANCE)
        if(cameraToX == cameraFromX and cameraFromY == cameraToY) then
            --ugly workaround for blizz camera getting unlocked when positions from == to
            cameraFromY = cameraFromY - 5.0;
        end

        SetCameraPosition(cameraFromX, cameraFromY)
        SetCameraField(CAMERA_FIELD_TARGET_DISTANCE, cameraFromDistance, 0.0)

        CameraSetupApplyForceDuration(cameraTo, true, duration)
        local timer = CreateTimer()
        ScreenplaySystem.cameraInterpolationTimer = timer
        local durationInt = math.floor(duration)
        local durationLeft = durationInt
        TimerStart(timer, 1.0, true, function()
            --print("durationLeft: " .. tostring(durationLeft) .. " / " .. tostring(durationInt))
            durationLeft = durationLeft - 1
            if durationLeft <= 0 then
                local backupCameraMovementDuration = 9999
                CameraSetupApplyForceDuration(cameraFrom, true, backupCameraMovementDuration)
                fixCameraIfNeeded(cameraToX, cameraFromX, cameraToY, cameraFromY, cameraToDistance, cameraFromDistance, cameraFrom, backupCameraMovementDuration, backupCameraMovementDuration + durationLeft)
            else
                fixCameraIfNeeded(cameraFromX, cameraToX, cameraFromY, cameraToY, cameraFromDistance, cameraToDistance, cameraTo, durationInt, durationLeft)
            end
        end)
        return timer
    end

    function ScreenplayUtils.interpolateCameraFromCurrentTillEndOfCurrentItem(cameraTo)
        return ScreenplayUtils.interpolateCameraFromCurrent(cameraTo, ScreenplayUtils.getCurrentItemDuration())
    end

    function ScreenplayUtils.interpolateCameraFromCurrent(cameraTo, duration)
        copyCameraSetup(GetCurrentCameraSetup(), customCamera)
        return interpolateCamera(customCamera, cameraTo, duration)
    end

    function ScreenplayUtils.interpolateCameraTillEndOfCurrentItem(cameraFrom, cameraTo)
        ScreenplayUtils.interpolateCamera(cameraFrom, cameraTo, ScreenplayUtils.getCurrentItemDuration())
    end

    function ScreenplayUtils.interpolateCamera(cameraFrom, cameraTo, duration)
        SkippableTimers:skip()
        interpolateCamera(cameraFrom, cameraTo, duration)
    end

    function ScreenplayUtils.interpolate(from, to, fraction)
        return from + (to - from) * fraction
    end

    function ScreenplayUtils.clearInterpolation()
        if ScreenplaySystem.cameraInterpolationTimer then
            SimpleUtils.releaseTimer(ScreenplaySystem.cameraInterpolationTimer)
        end
        StopCamera()
    end

    function ScreenplayUtils.getCurrentItemDuration()
        local currentItem = ScreenplaySystem:currentItem()
        if currentItem == nil then
            return 0.0
        end
        return currentItem:getDuration()
    end
end
if Debug then Debug.endFile() end