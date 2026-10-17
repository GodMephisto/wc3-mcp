---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/warcraft-3-in-windows-10-os.269225/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Warcraft 3 in Windows 10 Os
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/warcraft-3-in-windows-10-os.269225/>
#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Warcraft III in Windows 10

As some of you know that Windows 10 uses DirectX 12, which is incompatible with some of the older software, games and graphic drivers. Warcraft 3 was designed to work best with DirectX 8, as such it is more prone to weird graphical glitches, performance drops and some settings being no longer relevant. This tutorial is for those who are using Windows 10 OS (or DirectX 12).

What is DirectX?

Microsoft DirectX is a collection of application programming interfaces (APIs) for handling tasks related to multimedia, especially game programming and video, on Microsoft platforms. Originally, the names of these APIs all began with Direct, such as Direct3D, DirectDraw, DirectMusic, DirectPlay, DirectSound, and so forth. - Wiki

The problems of playing Warcraft 3 in Direct X 12 include some setting being irrelevant such as gamma setting.

The solution is very simple:

Download this .dll file and put it into your Warcraft 3 installation folder. You can now change in-game gamma.

d3d8.dll

This is a dynamic linked library for older version of Direct X. Warcraft 3 seems to be working better with DirectX 8.

Your Warcraft 3 should now work overall better and you can change gamma again.

Report your Win 10 related problems in the comments, we'll help you find a solution.

This solution is obsolete as of patch 1.27a. Patch 1.27a fixed numerous compatibility issues with Windows 10, Warcraft III no longer uses DirectX 8.

#### Attachments

D3d.zip

 116.9 KB

 · Views: 15,823




 Last edited: Aug 31, 2016

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Do note there is a small chance this will flag up in Warden and could get you banned. Even though they are genuine signed Microsoft DLLs they are in the wrong location. Additionally there might be other side effects to other running applications due to possible resource conflicts.

d3d9.dll should not be used as WC3 is only D3D8? I doubt it even knows to link to it.

Also I do not see how this should improve performance since those are nothing but wrappers for driver framework. Chances are only specific features have been disabled (eg gamma) due to them conflicting with newer features (calibration).

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Damn I am having another (minor) issue with my WE at the moment, sadly not what you described. Got my hopes up for nothing. Useful tutorial though.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Dr Super Good said:

 Do note there is a small chance this will flag up in Warden and could get you banned. Even though they are genuine signed Microsoft DLLs they are in the wrong location. Additionally there might be other side effects to other running applications.

d3d9.dll should not be used as WC3 is only D3D8?

Also I do not see how this should improve performance since those are nothing but wrappers for driver framework.


Click to expand...

You can't get banned by this, not possible. Since these .dll are also used in Bloom and HDR which I used many times in genuine Battle.Net.

There may be more issues with how Direct X handles Warcraft 3, we may not be aware of the others. Gamma was noticeable while performance related issues could not be so easily noticeable.

Chaosy said:

 Damn I am having another (minor) issue with my WE at the moment, sadly not what you described. Got my hopes up for nothing. Useful tutorial though.


Click to expand...

can you please describe your problem? I will be adding all solutions to this thread. Usually running a program as administrator or in compatibility mode (Win7, Vista) fixes the problem. Windows 10 is notorious for having screwed up administrator privileges to TrustedInstaller.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

DirectX does not directly deal with anything performance related. It acts as a wrapper for drivers. Direct3D calls translate into graphic driver calls. These are usually handled by the NVidia, AMD or Intel graphic drivers which interact with the hardware.

The performance gains by using Direct3D 12 come from a more efficient interface to the graphic hardware rather than Direct3D 12 doing anything magical.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice idea to have this thread. If I find any solutions to weird Win10 glitches with wc3, I'll let you know.

Approved for now. If anyone has any problems/solutions to share, feel free to contribute!

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

Can you guys test a thing for me?

In TFT, load up the night elf campaign and play the first cinematic.

Does it play at all or does it move you back to the campaign menu again?

It did play in Windows 7, but as of Windows 10 it seems to have stopped working.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

LordDz said:

 Can you guys test a thing for me?


In TFT, load up the night elf campaign and play the first cinematic.


Does it play at all or does it move you back to the campaign menu again?


It did play in Windows 7, but as of Windows 10 it seems to have stopped working.


Click to expand...


Weird. For me it's just a black screen. I'll try to find a solution, stay tuned.

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

Black screen is solved by installing codecs it seems.

http://us.battle.net/en/forum/topic/12203110379

But mine doesn't even play, it just returns me to the campaign screen.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

LordDz said:

 Black screen is solved by installing codecs it seems.

http://us.battle.net/en/forum/topic/12203110379

But mine doesn't even play, it just returns me to the campaign screen.


Click to expand...

Do you have a file called "Blizzard.ax" in Warcraft 3 installation folder? Mine was missing for some reason. Now it's working for me.

Edit: You should always run War3 as administrator.

#### Attachments

Blizzard.Ax.zip

 193 KB

 · Views: 1,466



#### datsatthune

#### datsatthune

 Level 6

JoinedSep 9, 2015Messages138

Not working for me, after copy there dll file, i can't open the game:

But working in my friend laptop. I don't know why

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

 can you please describe your problem? I will be adding all solutions to this thread. Usually running a program as administrator or in compatibility mode (Win7, Vista) fixes the problem. Windows 10 is notorious for having screwed up administrator privileges to TrustedInstaller.


Click to expand...

The editor generally loading very slowly. For example when you want to specify which effect you want to spawn at a location with triggers. It takes forever before the list loads and you can start browsing. Same goes when changing a unit's model it takes a lot of time before the list shows up.

Also sometimes I can't scroll. If I got, let's say 5 variable and try to scroll down to the one at the bottom, it simply wont work.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Chaosy said:

 The editor generally loading very slowly. For example when you want to specify which effect you want to spawn at a location with triggers. It takes forever before the list loads and you can start browsing. Same goes when changing a unit's model it takes a lot of time before the list shows up.


Also sometimes I can't scroll. If I got, let's say 5 variable and try to scroll down to the one at the bottom, it simply wont work.


Click to expand...

Here is a nice tutorial.

 Not working for me, after copy there dll file, i can't open the game:


Click to expand...


It may be that you are already running Warcraft 3, try going to task manager and checking if there is a process called War3.exe and close it. The next solution would be two install this. Finally if it still isn't working, try reinstalling and patching the game using the latest patch.


It is also recommended to contact Blizzard support.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Already done that a long time ago.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

 It takes forever before the list loads and you can start browsing. Same goes when changing a unit's model it takes a lot of time before the list shows up.


Click to expand...

Check your disk I/O load. That hints of something happening in the background using up disk I/O bandwidth (eg peer to peer update/upgrade sharing).

#### datsatthune

#### datsatthune

 Level 6

JoinedSep 9, 2015Messages138

A Void said:

Here is a nice tutorial.


It may be that you are already running Warcraft 3, try going to task manager and checking if there is a process called War3.exe and close it. The next solution would be two install this. Finally if it still isn't working, try reinstalling and patching the game using the latest patch.


It is also recommended to contact Blizzard support.


Click to expand...


install directx9 and it working for me, thank you very much

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Dr Super Good said:

 Check your disk I/O load. That hints of something happening in the background using up disk I/O bandwidth (eg peer to peer update/upgrade sharing).


Click to expand...

Funny thing, wc3 including JNGP is on my SSD so speed should actually be.. well..

I do not know how to track such though. I did some quick googling which lead me to something called "Prestandaövervakaren" in Swedish. Which I'd roughly translate into "Performance Monitor" which I started by pressing Start + R and type "%SystemRoot%\system32\perfmon.msc /s"

edit: Okay I made the program track "Drive - Read Byte/s", "Drive - Written Byte/s" and "Drive Byte/s"

As you see there are some odd spikes but it did not seem to spike when creating said scenarios in the editor. It spikes either way.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

 install directx9 and it working for me, thank you very much


Click to expand...

Bad idea. Windows 10 needs DirectX 12 which is installed automatically, anything else might corrupt the kernel.

DirectX 12 is backwards compatible with Direct3D 9 and earlier, although how well drivers implement it is another question as the old APIs are now 2 generations deprecated.

 Funny thing, wc3 including JNGP is on my SSD so speed should actually be.. well..


Click to expand...

In which case disc I/O is clearly not a problem. Unless of course the OS is installed on a mechanical drive in which case it could be dlls loading slowly.

Another possible cause is the new windows manager Windows 10 uses. This might cause some older applications to become more unresponsive or run slower because of how they were implemented. For example alt+tab in Windows 10 is much faster from full screen to windowed mode but it introduces compatibility bugs such as colour calibration not swapping properly in older applications.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Dr Super Good said:

 Bad idea. Windows 10 needs DirectX 12 which is installed automatically, anything else might corrupt the kernel.


Click to expand...


It does not remove DirectX 12, I checked it myself using DirectX Diagnostic Tool.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

 It does not remove DirectX 12, I checked it myself using DirectX Diagnostic Tool.


Click to expand...

What does this have to do with this...

 install directx9 and it working for me, thank you very much


Click to expand...

The installer should not be able to run as it was never designed for Windows 10.

EDIT:

I am also getting bad performance in game with WC3 in Windows 10 as other people have reported. It would be interesting to know why this is happening.

Of interest is the reported frame rate. It often spikes to "62 FPS" instead of anchoring at 60-59.9 FPS. I am guessing the game is being run in some sort of pseudo Windowed Full Screen mode for resource sharing as part of the DX12 API and the new windowing frame work.

EDIT 2:

To clarify, only d3d8.dll is used. WC3 does not use d3d9.dll so you should not bundle it with. Further more WC3 behaves as intended when running d3d8.dll you provided but breaks with the d3d8.dll Windows 10 comes with. I will look into this problem.

EDIT 3:

I am not sure your d3d8.dll is safe? It is from some Russian site and claims it is a wrapper for d3d9 and it created a batch file in the WC3 installation folder. In any case that is a hacky solution.

The d3d8.dll used comes from the SysWOW64 folder (System 32 only if using 32bit Windows 10). I tried substituting back in the one used by Windows 7 (from the same folder) but the dll is incompatible with Windows 10 (WC3 throws DirectX not installed error).

As such d3d8.dll in Windows 10 is different from Windows 7. The big question is what is different with it to be causing the problems.


 Last edited: Sep 10, 2015

#### Alprazolam

#### Alprazolam

 Level 1

JoinedSep 25, 2015Messages4

I'm having the same issue in Wc3..

Espacially when alot of units are coming together and if alot of players have more items, talking about dota..

this pretty much sucks, since my old laptop did its job at 60 fps..

@DrSuperGood..

Did you look into te problem yet?

I have been searching about this issue, with no avail sadly...

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

I could not find any documented Windows 10 changes to legacy Direct3D support. I am guessing it has something to do with DirectX 8 being old and DirectX 12 being very different from DirectX 11.

The only real solution currently is to use the "-opengl" command line option as OpenGL does not appear to be affected by this.

#### Alprazolam

#### Alprazolam

 Level 1

JoinedSep 25, 2015Messages4

Dr Super Good said:

 I could not find any documented Windows 10 changes to legacy Direct3D support. I am guessing it has something to do with DirectX 8 being old and DirectX 12 being very different from DirectX 11.

The only real solution currently is to use the "-opengl" command line option as OpenGL does not appear to be affected by this.


Click to expand...

So i have to start the game up with this thicked on?

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

 So i have to start the game up with this thicked on?


Click to expand...

No?! You simply change...

 "C:\Program Files (x86)\Warcraft III\Frozen Throne.exe"


Click to expand...

to...

 "C:\Program Files (x86)\Warcraft III\Frozen Throne.exe" -opengl


Click to expand...

With the WC3 launcher shortcut.


For example on desktop...

Right click "Frozen Throne" shortcut.

Select (press) "Properties".

Select the field "Target:".

Change it from "..." to "..." -opengl where "..." is a path to the installed version of Warcraft III. An example of such a path was provided earlier in this post (for English UK 64bit Windows).

#### Alprazolam

#### Alprazolam

 Level 1

JoinedSep 25, 2015Messages4

Dr Super Good said:

 No?! You simply change...


to...


With the WC3 launcher shortcut.


For example on desktop...

Right click "Frozen Throne" shortcut.

Select (press) "Properties".

Select the field "Target:".

Change it from "..." to "..." -opengl where "..." is a path to the installed version of Warcraft III. An example of such a path was provided earlier in this post (for English UK 64bit Windows).

Click to expand...


Tried this, but the same is happening.


Ohwell.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

 Tried this, but the same is happening.


Click to expand...

Works fine for me... Maybe you are not doing it right as you appear to be using a hacked version of WC3 started by a third party launcher.

#### Alprazolam

#### Alprazolam

 Level 1

JoinedSep 25, 2015Messages4

Dr Super Good said:

 Works fine for me... Maybe you are not doing it right as you appear to be using a hacked version of WC3 started by a third party launcher.


Click to expand...


Its not a hacked version.


I bought the golden edition on G2A.


Why do i use a third party program you might think...


Well, for dota stats and GPROXY in order to reconnect to the game..

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

As long as you start the game with that shortcut it should work, I just re-tested and Gamma as well as frame rate works normally on Windows 10 when starting WC3 with that shortcut.

#### joeng64

#### joeng64

 Level 1

JoinedOct 4, 2015Messages3

Dr Super Good said:

 As long as you start the game with that shortcut it should work, I just re-tested and Gamma as well as frame rate works normally on Windows 10 when starting WC3 with that shortcut.


Click to expand...


Good day sir, I have tried the openlgl and it works but my problem is I play WC3 using GARENA or RGC and I tried to run the game but it seems the opengl was lost or not working, the gamma was set back again to default when using these apps but when I just run the game from the shortcut itself it works with no problem, any idea to this issue?

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

 Good day sir, I have tried the openlgl and it works but my problem is I play WC3 using GARENA or RGC and I tried to run the game but it seems the opengl was lost or not working, the gamma was set back again to default when using these apps but when I just run the game from the shortcut itself it works with no problem, any idea to this issue?


Click to expand...

I recommend contacting the maintainers of those third party programs. I only use BattleNet.

What must be happening is when they start the game they are not using the -opengl flag. I would imagine their program allows you to toggle that flag on or off before starting the game through their program or at least allows you to specify command line flags to send to the game when starting.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Dr Super Good said:

 I recommend contacting the maintainers of those third party programs. I only use BattleNet.


What must be happening is when they start the game they are not using the -opengl flag. I would imagine their program allows you to toggle that flag on or off before starting the game through their program or at least allows you to specify command line flags to send to the game when starting.


Click to expand...


Thank you for maintaining this thread, lately my activity has diminished.

#### joeng64

#### joeng64

 Level 1

JoinedOct 4, 2015Messages3

I see..thanks for the advice, I kinda thought that may have been the case.. too bad the dll also didnt work for me because it seems RGC doesnt allow any modification to the wc3 folder, it automatically deletes it when I hit the start WC3 button. guess I'm stuck with this for now

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

joeng64 said:

 I see..thanks for the advice, I kinda thought that may have been the case.. too bad the dll also didnt work for me because it seems RGC doesnt allow any modification to the wc3 folder, it automatically deletes it when I hit the start WC3 button. guess I'm stuck with this for now


Click to expand...


Move the .dll and in the properties select "Read Only". It will prevent it from being deleted by other programs.

#### joeng64

#### joeng64

 Level 1

JoinedOct 4, 2015Messages3

 Move the .dll and in the properties select "Read Only". It will prevent it from being deleted by other programs.


Click to expand...

just tried this out and I get this error msg when starting wc3, also tried to set it at hidden but same thing..

[ERROR] Failed to clean up: C:\Program Files (x86)\Warcraft III\ :: d3d8.dll

[ERROR] Your game directory contains modifications and RGC was unable to remove them. Your game cannot be started.

damn no luck at all  thanks for the tip though

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Thank you for this tutorial, Void! When I upgraded to Windows 10, I got a noticable FPS drop when playing WC3. However, after installing the .dll files you provided, it has returned to normal

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

I still would recommend OpenGL mode instead of some third party work around.

#### WXZ

#### WXZ

 Level 1

JoinedDec 8, 2015Messages6

the dll methods worked for me !! I just copy the 2 dll into w3 folders, restarted with the installer, 1 min and it worked ! thanks so much ^

#### Darkshooter1986

#### Darkshooter1986

 Level 5

JoinedNov 10, 2009Messages85

Re: Running WC3 on Windows 10

I have a new computer with windows 10 and have installed and played Warcraft 3 with no problem at all. Only thing I do is run as admin and no gamma or graphics problems nor any lag related so might be an issue with your graphics on your computer or something else. I believe I am even using Direct X 12 currently and have experienced zero problems...

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

 I have a new computer with windows 10 and have installed and played Warcraft 3 with no problem at all. Only thing I do is run as admin and no gamma or graphics problems nor any lag related so might be an issue with your graphics on your computer or something else. I believe I am even using Direct X 12 currently and have experienced zero problems...


Click to expand...

The only problem is that Gamma cannot be set in Direct3D mode, moving the slider does nothing. There are no other problems and gamma may never need to be set if you are happy with its current setting. There is even no need to run the game in compatibility mode or as an administrator and I would advise against doing that for security reasons.

#### Zhall

#### Zhall

 Level 9

JoinedMar 27, 2009Messages311

Resources: 4

#### Resources
Maps 3Tutorials 1

I'm getting this error.


I have 20 gb of hard drive space left, and if I uninstall and re-install it works fine again.

#### Attachments

No space.png

 330.5 KB

 · Views: 863



#### StoPCampinGn00b

#### StoPCampinGn00b

 Level 32

JoinedApr 2, 2013Messages3,954

Resources: 1

#### Resources
Maps 1

Zhall said:

 I'm getting this error.


I have 20 gb of hard drive space left, and if I uninstall and re-install it works fine again.


Click to expand...


See if it works if you run it as admin.

#### Zhall

#### Zhall

 Level 9

JoinedMar 27, 2009Messages311

Resources: 4

#### Resources
Maps 3Tutorials 1

StoPCampinGn00b said:

 See if it works if you run it as admin.


Click to expand...


Thanks, seems to have worked.


I swear i tried that.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Running in administrator in the long run is asking for problems. One malicious map with an undisclosed exploit and your computer could be toast.

Zhall I am guessing your install folder does not have write permission with your user account. You should be able to fix this like all permission problems by finding the folder, right clicking, going to properties and then modifying permissions in there. You need to make sure your user account has full control over the Warcraft III folder and all sub folders and files.

#### Darkshooter1986

#### Darkshooter1986

 Level 5

JoinedNov 10, 2009Messages85

Well I believe the only reason I run in admin or have to is that it is in my progam files (x86) folder. Im sure that placing it somewhere else will work better.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

 Well I believe the only reason I run in admin or have to is that it is in my progam files (x86) folder. Im sure that placing it somewhere else will work better.


Click to expand...

Mine is also in that folder. So that is not a problem. Once again, you just need to make sure that single folder (Warcraft III) has full control permission for your user account.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

This is now obsolete with the release of todays patch. Warcraft III no longer needs to be run in OpenGL mode or use some hacky DLL.

I would advise people remove the DLL. There is a good chance it could trigger anti-hack detection in Warcraft III as it causes a dependency to not match a signed system dll like it expects.

A reasonably correct gamma setting is 2.2 (setting 44?). Gamma is not brightness/darkness as the option hints but rather color correction since darkest and brightest colors are not affected by gamma.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Dr Super Good said:

 This is now obsolete with the release of todays patch. Warcraft III no longer needs to be run in OpenGL mode or use some hacky DLL.


I would advise people remove the DLL. There is a good chance it could trigger anti-hack detection in Warcraft III as it causes a dependency to not match a signed system dll like it expects.


A reasonably correct gamma setting is 2.2 (setting 44?). Gamma is not brightness/darkness as the option hints but rather color correction since darkest and brightest colors are not affected by gamma.


Click to expand...


You failed to mention that gamma is not fixed in windows mode. Besides, this thread is about giving support for Windows 10 not about gamma alone.


I will still be using this solution, because Blizzard's one is still crap compared to this.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

 You failed to mention that gamma is not fixed in windows mode.


Click to expand...

It ever worked in Windowed mode? Not even in OpenGL mode does gamma work in Windowed mode.

The reason for this is that the gamma is applied by the GPU to display output in a separate color profile stage completely unrelated to how the display image was composed. If running in windowed mode the game is forced to use the desktop color profile, which by default assumes the pixel values are sRGB so does not perform any correction (stuck at gamma 1.0). If the option did work it would raise the gamma of the entire desktop and break any loaded color profiles which is something worse than just not working.

The fix requires a programmable pixel shader be used to add a gamma correction stage to the composed images independent of the display color profile.

EDIT:

Warcraft III no longer uses D3D8 hence the D3D8 related stuff now does nothing. That said it now uses D3D9 so the previously pointless D3D9 stuff now applies. As far as I am aware the D3D9 implementation of Windows 10 is not buggy in the way the D3D8 one is so I am not sure if there is a reason to do this anymore.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Dr Super Good said:

 It ever worked in Windowed mode? Not even in OpenGL mode does gamma work in Windowed mode.

The reason for this is that the gamma is applied by the GPU to display output in a separate color profile stage completely unrelated to how the display image was composed. If running in windowed mode the game is forced to use the desktop color profile, which by default assumes the pixel values are sRGB so does not perform any correction (stuck at gamma 1.0). If the option did work it would raise the gamma of the entire desktop and break any loaded color profiles which is something worse than just not working.

The fix requires a programmable pixel shader be used to add a gamma correction stage to the composed images independent of the display color profile.

EDIT:

Warcraft III no longer uses D3D8 hence the D3D8 related stuff now does nothing. That said it now uses D3D9 so the previously pointless D3D9 stuff now applies. As far as I am aware the D3D9 implementation of Windows 10 is not buggy in the way the D3D8 one is so I am not sure if there is a reason to do this anymore.


Click to expand...

To forgot that this has two .dll included. One is 9 and the other 8.

Edit: Regardless, this solution is no longer supported or needed. Windows 10 compatibility was greatly improved in Patch 1.27a.


 Last edited: Mar 27, 2016

#### Aravarth

A

#### Aravarth

Even after installing either the K-Lite Codec Pack (now version 12.1.8) or the LAV Filters codec pack, I still get blown out cinematic colours. Is there any way to square this away so they appear as they are supposed to?

EDIT: I'm also running dual monitors, and when I go to cinematics, it looks to put the cinematics in the middle of both of my monitors. Is this contributing to the problem?


 Last edited by a moderator: Jun 11, 2016

 You must log in or register to reply here.


### Similar threads

Locked

 DirectX Problems


Berb

Jan 14, 2010

Computer Tech

Replies4Views4K

Jan 19, 2010
Berb

Locked

[Crash]
 Warcraft and NEWGEN crash (DirectX error)


Wayshan

Aug 10, 2016

World Editor Help Zone

Replies10Views11K

Sep 2, 2016
Wayshan

Locked

 W3 Reforged System Requirements (Speculation)


Archian

Oct 18, 2019

Warcraft Discussion

Replies3Views4K

Nov 6, 2019
Dr Super Good

Locked

 Warcraft 3 Engine Upgrade


chopinski

May 12, 2018

Warcraft Discussion

Replies25Views10K

Jun 1, 2018
pyf

Locked

 Can't run World Editor now


cryoreeze

Oct 28, 2015

World Editor Help Zone

Replies8Views3K

Oct 28, 2015
cryoreeze

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top