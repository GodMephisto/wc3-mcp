---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/music_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Music Manager                                       |
    | =================================================== |
    | Manages the background music in XTD.                |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.MusicManager = XTD.Systems.MusicManager or {}

XTD.Systems.MusicManager.currentTrack = 0
XTD.Systems.MusicManager.playlist = {}
XTD.Systems.MusicManager.playlistIndex = 1

XTD.Systems.MusicManager.trackTimer = nil
XTD.Systems.MusicManager.playTimer = nil

XTD.Systems.MusicManager.trackLength = 0.00
XTD.Systems.MusicManager.trackRemaining = 0.00

XTD.Systems.MusicManager.isPaused = false
XTD.Systems.MusicManager.isTemporaryMusicPlaying = false

XTD.Systems.MusicManager.TRACK_END_PADDING = 2.00
XTD.Systems.MusicManager.TRACK_SWITCH_DELAY = 0.10

--[[
    Initialize the Music Manager.
]]
function XTD.Systems.MusicManager.Init()
    local self = XTD.Systems.MusicManager

    XTD.Core.Debug("MusicManager", "Initializing the Music Manager...", "Init")

    self.currentTrack = 0
    self.playlist = {}
    self.playlistIndex = 1
    self.trackLength = 0.00
    self.trackRemaining = 0.00
    self.isPaused = false
    self.isTemporaryMusicPlaying = false

    self.DestroyTrackTimer()
    self.DestroyPlayTimer()
    self.ShufflePlaylist()
    self.PlayNextTrack()

    XTD.Core.Debug("MusicManager", "Music Manager initialized.", "Init")
end

--[[
    Gets the music data table.
]]
function XTD.Systems.MusicManager.GetMusic()
    return XTD.Data.Music
end

--[[
    Validates that music data exists.
]]
function XTD.Systems.MusicManager.HasMusic()
    local music = XTD.Systems.MusicManager.GetMusic()

    if music == nil then
        XTD.Core.Debug("MusicManager", "music is nil.", "HasMusic")
        return false
    end

    if #music == 0 then
        XTD.Core.Debug("MusicManager", "No music tracks found.", "HasMusic")
        return false
    end

    return true
end

--[[
    Destroys the active track timer.
]]
function XTD.Systems.MusicManager.DestroyTrackTimer()
    local self = XTD.Systems.MusicManager

    if self.trackTimer ~= nil then
        PauseTimer(self.trackTimer)
        DestroyTimer(self.trackTimer)
        self.trackTimer = nil
    end
end

--[[
    Destroys the delayed play timer.
]]
function XTD.Systems.MusicManager.DestroyPlayTimer()
    local self = XTD.Systems.MusicManager

    if self.playTimer ~= nil then
        PauseTimer(self.playTimer)
        DestroyTimer(self.playTimer)
        self.playTimer = nil
    end
end

--[[
    Shuffles the gameplay playlist.
]]
function XTD.Systems.MusicManager.ShufflePlaylist()
    local self = XTD.Systems.MusicManager

    if not self.HasMusic() then
        return
    end

    local music = self.GetMusic()

    self.playlist = {}

    for index = 1, #music do
        table.insert(self.playlist, index)
    end

    for index = #self.playlist, 2, -1 do
        local randomIndex = math.random(1, index)

        self.playlist[index], self.playlist[randomIndex] =
            self.playlist[randomIndex], self.playlist[index]
    end

    if #self.playlist > 1 and self.currentTrack ~= 0 and self.playlist[1] == self.currentTrack then
        self.playlist[1], self.playlist[2] = self.playlist[2], self.playlist[1]
    end

    self.playlistIndex = 1
end

--[[
    Gets the next track index from the shuffled playlist.
]]
function XTD.Systems.MusicManager.GetNextTrackIndex()
    local self = XTD.Systems.MusicManager

    if not self.HasMusic() then
        return nil
    end

    if self.playlist == nil or #self.playlist == 0 then
        self.ShufflePlaylist()
    end

    if self.playlistIndex > #self.playlist then
        self.ShufflePlaylist()
    end

    local trackIndex = self.playlist[self.playlistIndex]
    self.playlistIndex = self.playlistIndex + 1

    return trackIndex
end

--[[
    Starts a timer for the current track.
]]
function XTD.Systems.MusicManager.StartTrackTimer(duration)
    local self = XTD.Systems.MusicManager

    self.DestroyTrackTimer()

    if duration == nil or duration <= 0 then
        return
    end

    self.trackTimer = CreateTimer()

    TimerStart(self.trackTimer, duration, false, function()
        self.DestroyTrackTimer()

        if self.isPaused then
            return
        end

        if self.isTemporaryMusicPlaying then
            return
        end

        self.PlayNextTrack()
    end)
end

--[[
    Calculates the timer duration for the current track.
]]
function XTD.Systems.MusicManager.GetTrackTimerDuration(length)
    local self = XTD.Systems.MusicManager

    if length == nil or length <= 0 then
        return 0.00
    end

    local duration = length - self.TRACK_END_PADDING

    if duration < 0.10 then
        duration = length
    end

    return duration
end

--[[
    Actually starts playing a music file after a short delay.
]]
function XTD.Systems.MusicManager.DelayedPlay(file)
    local self = XTD.Systems.MusicManager

    self.DestroyPlayTimer()

    self.playTimer = CreateTimer()

    TimerStart(self.playTimer, self.TRACK_SWITCH_DELAY, false, function()
        PlayMusic(file)

        self.DestroyPlayTimer()
    end)
end

--[[
    Plays a specific gameplay track by index.
]]
function XTD.Systems.MusicManager.PlayTrack(trackIndex)
    local self = XTD.Systems.MusicManager

    if not self.HasMusic() then
        return
    end

    local music = self.GetMusic()
    local track = music[trackIndex]

    if track == nil then
        XTD.Core.Debug("MusicManager", "track is nil.", "PlayTrack")
        return
    end

    if track.file == nil then
        XTD.Core.Debug("MusicManager", "track.file is nil.", "PlayTrack")
        return
    end

    self.currentTrack = trackIndex
    self.trackLength = track.length or 0.00
    self.trackRemaining = self.trackLength
    self.isPaused = false
    self.isTemporaryMusicPlaying = false

    ClearMapMusic()
    StopMusic(true)
    self.DelayedPlay(track.file)

    self.StartTrackTimer(self.GetTrackTimerDuration(self.trackLength))

    XTD.UI.MessageManager.Broadcast(
        XTD.Core.Color.AncientGold("♪ Now Playing: ") ..
        XTD.Core.Color.CoolBlue(track.name or "Unknown Track") ..
        XTD.Core.Color.AncientGold(" - ") ..
        XTD.Core.Color.Ivory(track.artist or "Unknown Artist")
    )

    XTD.Core.Debug("MusicManager", "Playing track: " .. track.file, "PlayTrack")
end

--[[
    Plays the next shuffled gameplay track.
]]
function XTD.Systems.MusicManager.PlayNextTrack()
    local self = XTD.Systems.MusicManager
    local nextTrack = self.GetNextTrackIndex()

    if nextTrack == nil then
        return
    end

    self.PlayTrack(nextTrack)
end

--[[
    Starts the background music.
]]
function XTD.Systems.MusicManager.StartMusic()
    local self = XTD.Systems.MusicManager

    self.ShufflePlaylist()
    self.PlayNextTrack()
end

--[[
    Skips the current gameplay track.
]]
function XTD.Systems.MusicManager.SkipTrack()
    local self = XTD.Systems.MusicManager

    self.DestroyTrackTimer()
    self.DestroyPlayTimer()
    self.PlayNextTrack()
end

--[[
    Pauses the current gameplay music.
]]
function XTD.Systems.MusicManager.PauseMusic()
    local self = XTD.Systems.MusicManager

    if self.isPaused then
        return
    end

    local elapsed = 0.00

    if self.trackTimer ~= nil then
        elapsed = TimerGetElapsed(self.trackTimer)
    end

    self.trackRemaining = self.trackLength - elapsed

    if self.trackRemaining < 0.00 then
        self.trackRemaining = 0.00
    end

    self.DestroyTrackTimer()
    self.DestroyPlayTimer()

    StopMusic(true)

    self.isPaused = true
end

--[[
    Resumes the paused gameplay music.
]]
function XTD.Systems.MusicManager.ResumeMusic()
    local self = XTD.Systems.MusicManager

    if not self.isPaused then
        return
    end

    ResumeMusic()

    self.isPaused = false

    self.StartTrackTimer(self.GetTrackTimerDuration(self.trackRemaining))
end

--[[
    Plays temporary music, such as boss, victory, or defeat music.
]]
function XTD.Systems.MusicManager.PlayTemporaryMusic(file)
    local self = XTD.Systems.MusicManager

    if file == nil then
        XTD.Core.Debug("MusicManager", "file is nil.", "PlayTemporaryMusic")
        return
    end

    self.PauseMusic()
    self.isTemporaryMusicPlaying = true

    ClearMapMusic()
    StopMusic(true)
    self.DelayedPlay(file)
end

--[[
    Stops temporary music and resumes the gameplay playlist.
]]
function XTD.Systems.MusicManager.StopTemporaryMusic()
    local self = XTD.Systems.MusicManager

    if not self.isTemporaryMusicPlaying then
        return
    end

    ClearMapMusic()
    StopMusic(true)

    self.isTemporaryMusicPlaying = false

    self.ResumeMusic()
end

--[[
    Stops all music and clears timers.
]]
function XTD.Systems.MusicManager.StopAllMusic()
    local self = XTD.Systems.MusicManager

    self.DestroyTrackTimer()
    self.DestroyPlayTimer()

    self.isPaused = false
    self.isTemporaryMusicPlaying = false
    self.trackRemaining = 0.00

    ClearMapMusic()
    StopMusic(true)
end