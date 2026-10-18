---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "UnitSoundSets/SoundSet.lua"
ref: HEAD
---

if Debug then Debug.beginFile "SoundSet" end
do
    local KEY_LAST_PLAYED_SOUND = 2

    SoundSet = {
        ids = {},
        soundsByIds = {},
        counts = {},
        lastPlayedSounds = {},
        sequence = 0
    }

    SimpleUtils.newClass(SoundSet, function(self)
        self.ids = InitHashtable()
        self.soundsByIds = InitHashtable()
        self.counts = InitHashtable()
        self.lastPlayedSounds = InitHashtable()
    end)

    function SoundSet:add(unitTypeId, soundType, index, whichSound)
        if whichSound == nil then
            return false
        end
        local soundId = LoadInteger(self.ids, unitTypeId, soundType)
        if soundId == 0 then
            self.sequence = self.sequence + 1
            soundId = self.sequence
            SaveInteger(self.ids, unitTypeId, soundType, soundId)
        end

        SaveSoundHandle(self.soundsByIds, soundId, index, whichSound)
        local count = LoadInteger(self.counts, unitTypeId, soundType)
        count = count + 1
        SaveInteger(self.counts, unitTypeId, soundType, count)
        UnitSoundSets.printUnitSoundDebug("SAVED ID: unit type: " .. tostring(unitTypeId) .. " -> sound type: " .. tostring(soundType) .. " -> soundId: " .. tostring(soundId) .. ", count: " .. tostring(count))
        UnitSoundSets.printUnitSoundDebug("SAVED SOUND: soundId: " ..  tostring(soundId) .. ", index: " .. tostring(index))
        return true
    end

    function SoundSet:get(unitTypeId, soundType, index)
        local id = LoadInteger(self.ids, unitTypeId, soundType)
        return LoadSoundHandle(self.soundsByIds, id, index)
    end

    function SoundSet:getCount(unitTypeId, soundType)
        return LoadInteger(self.counts, unitTypeId, soundType)
    end

    function SoundSet:remove(unitTypeId)
        FlushChildHashtable(self.ids, unitTypeId)
        FlushChildHashtable(self.counts, unitTypeId)
    end

    function SoundSet:clear()
        FlushParentHashtable(self.ids)
        FlushParentHashtable(self.soundsByIds)
        FlushParentHashtable(self.counts)
        self.sequence = 0
    end

    function SoundSet:exists(unitTypeId, soundType)
        return LoadInteger(self.ids, unitTypeId, soundType) > 0
    end

    function SoundSet:getRandom(unitTypeId, soundType)
        local lastPlayedSound = self:getLastPlayedSound(unitTypeId)
        local whichSound
        local soundId = LoadInteger(self.ids, unitTypeId, soundType)
        local soundCount = LoadInteger(self.counts, unitTypeId, soundType)
        UnitSoundSets.printUnitSoundDebug("SoundSet:GetRandom: unitTypeId: " .. tostring(unitTypeId) .. ", soundType: " .. soundType .. ", soundId: " .. tostring(soundId) .. ", soundCount: " .. tostring(soundCount))

        local soundsCounter = 0
        local soundIndex = 1
        local sounds = {}
        while soundIndex <= soundCount do
            whichSound = LoadSoundHandle(self.soundsByIds, soundId, soundIndex)
            --  never play the same sound twice if there are multiple sounds
            if (not (whichSound == nil) and not (whichSound == lastPlayedSound)) then
                sounds[soundsCounter + 1] = whichSound
                soundsCounter = soundsCounter + 1
            end
            soundIndex = soundIndex + 1
        end
        if (soundsCounter > 0) then
            local pickedSoundIndex = GetRandomInt(1, soundsCounter)
            UnitSoundSets.printUnitSoundDebug("SoundSet:GetRandom: picked random sound " .. tostring(pickedSoundIndex) .. " / " .. tostring(soundsCounter))
            return sounds[pickedSoundIndex]
        end

        return nil
    end

    function SoundSet:getLastPlayedSound(unitTypeId)
        return LoadSoundHandle(self.lastPlayedSounds, unitTypeId, KEY_LAST_PLAYED_SOUND)
    end

    function SoundSet:setLastPlayedSound(unitTypeId, whichSound)
        SaveSoundHandle(self.lastPlayedSounds, unitTypeId, KEY_LAST_PLAYED_SOUND, whichSound)
    end
end
if Debug then Debug.endFile() end
