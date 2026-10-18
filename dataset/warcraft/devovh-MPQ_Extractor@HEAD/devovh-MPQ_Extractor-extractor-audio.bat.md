---
type: vendor-doc
source_repo: "https://github.com/devovh/MPQ_Extractor"
source_path: "extractor/audio.bat"
ref: HEAD
---

@echo off
echo ******************************************************************************************
echo This uses FFMPEG to convert and rename all of the files required for AoWoW sounds and
echo music. FFMPEG will loop indefinitely if we output a file of the same extension (MP3 to 
echo MP3), so we just append an underscore there.
echo ******************************************************************************************
echo USE: Place this convert.cmd and ffmpeg.exe in the <localeCode>\Sound folder and run.
echo ******************************************************************************************
pause

REM ** converting music files
FOR /r %%f in (*.mp3) DO (
ffmpeg.exe -hide_banner -y -i "%%f" -f mp3 -acodec libmp3lame "%~d0%%~pf%%~nf.mp3_"
if not errorlevel 1 if exist "%~d0%%~pf%%~nf.mp3_" del /q /f "%~d0%%~pf%%~nf.mp3"
)

REM ** converting sound files
FOR /r %%f in (*.wav) DO (
ffmpeg.exe -hide_banner -y -i "%%f" -f ogg -acodec libvorbis "%~d0%%~pf%%~nf.wav_"
if not errorlevel 1 if exist "%~d0%%~pf%%~nf.wav_" del /q /f "%~d0%%~pf%%~nf.wav"
)