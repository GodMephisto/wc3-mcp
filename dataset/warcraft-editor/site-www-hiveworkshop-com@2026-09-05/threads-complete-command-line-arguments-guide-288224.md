---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/complete-command-line-arguments-guide.288224/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Complete Command Line Arguments Guide
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/complete-command-line-arguments-guide.288224/>
#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

These are the commands that are currently supported by Warcraft III, based on known information and some clever reverse engineering of the executables.

Commands

Video Mode

Changing the video mode can sometimes be used to increase performance or make Warcraft III run better in other environments like Wine.

d3d (Starts the game using Direct3D. This is the default.). DEPRECATED AS OF 1.29 SEE BELOW

opengl (Starts the game using Open Graphics Library.) DEPRECATED AS OF 1.29 SEE BELOW

swtnl (Starts the game using Software Transform and Lighting). DEPRECATED AS OF 1.29 SEE BELOW

graphicsapi <option> (This replaced d3d and opengl, and should be used with one of the options below)
OpenGL2 (Works as the old opengl)

OpenGL4 (CRASHES THE GAME)

Direct3D9 (Works as the old d3d, still the default)

Direct3D11 (CRASHES THE GAME)

Direct3D12 (CRASHES THE GAME)

Metal2 (CRASHES THE GAME)

Null (CRASHES THE GAME)

Window State

This controls if the game is in fullscreen or windowed mode.

fullscreen (Starts the game in fake fullscreen mode. - default)

window (Starts the game in windowed mode.)

nativefullscr (Starts the game in normal fullscreen mode.)

nowfpause (Stops the game from pausing when the window loses focus, useful for testing)
Reign of Chaos

This simply controls whether the game runs as Reign of Chaos (classic) or The Frozen Throne.

classic
Quick Start

This allows you to instantly start a replay or a map without going through the menu.

loadfile <path> (Specifies which file to load, the file must be in a subdirectory of your Warcraft III installation. This also works with the editor.)
Modding

This is a single command that allows you to load all resources from a different directory. Useful for things like total conversions.[/SIZE]

datadir <path>
Undocumented

gametype <string> (It is unknown what this command does, but it works with loadfile and must be a number.)

crash (Instantly crashes the game, maybe used to test the error reporter)

assert

locale <locale> (Appears to change the game from English to another language)

rockey <key> (Possibly sets the Reign of Chaos CD-Key)

tftkey <key> (Possible sets the The Frozen Throne CD-Key)

plugindir <path?> (Are we getting plugins)

routerconfig <string>

routerapp

Examples

To use this, simply create a shortcut to War3.exe and in Properties->Shortcut, modify the Target.

"C:\Games\Warcraft III\Warcraft III.exe" -window (Starts the game in windowed mode.)

"C:\Games\Warcraft III\Warcraft III.exe" -window -graphicsapi OpenGL2 (Starts the game in windowed mode with OpenGL rendering.)

"C:\Games\Warcraft III\Warcraft III.exe" -loadfile "C:\Games\Warcraft III\Maps\(2)BootyBay.w3m"


 Last edited: Mar 3, 2018

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Mind to elaborate for me what it means?

MindWorX said:

 Useful for things like total conversions.


Click to expand...

Nevertheless this shares somthing useful and is straight forward.

Submission:Complete Command-Line Arguments Guide

Date:21 November 2016

Status:Approved

Note:

Useful information. Approved.

#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

This isn't up to date anymore and needs to be updated with the changes from the latest patches.

#### Hotwer

#### Hotwer

 Level 7

JoinedMar 10, 2013Messages366

MindWorX said:

 This isn't up to date anymore and needs to be updated with the changes from the latest patches.


Click to expand...

Now we have only -window and -nativefullscrn, not too much to add.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Can someone explain the "quick start" to me a little more? I'm not sure I get it.

#### Hotwer

#### Hotwer

 Level 7

JoinedMar 10, 2013Messages366

Kyrbi0 said:

 Can someone explain the "quick start" to me a little more? I'm not sure I get it.


Click to expand...

You can call it as a command-line (or as he suggests, creating a shortcut) as, for example:

"C:\Games\Warcraft III\War3.exe" -loadfile "C:\Games\Warcraft III\Replays\LastReplay.w3g"


 Last edited: Apr 12, 2017

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Ah, so (ignoring replays because I don't care), I can take any arbitrary map & either make a shortcut to run it from, say, the desktop... Or run it from the command line?

#### Hotwer

#### Hotwer

 Level 7

JoinedMar 10, 2013Messages366

Kyrbi0 said:

 Ah, so (ignoring replays because I don't care), I can take any arbitrary map & either make a shortcut to run it from, say, the desktop... Or run it from the command line?


Click to expand...

Not map, the path must point to a replay file, but yes, it is.

#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

It doesn't have to be a replay, works with maps as well. Worldedit uses -loadfile to start testmaps. The only caveat is that maps have to be in the same directory or a subdirectory of war3.exe. I'm guessing some testing will be needed now that everything has changed with the recent patches.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Which command forces starting Frozen throne?

Edit 17.05.2018: That value was moved into window's reg. HKEY_CURRENT_USER\Software\Blizzard Entertainment\Warcraft III - "Preferred Game Version" = 0 or 1. [1 beeing frozen throne]


 Last edited: May 17, 2018

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

MindWorX said:

 This isn't up to date anymore and needs to be updated with the changes from the latest patches.


Click to expand...

@MindWorX: an update could prove to be useful indeed...

#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

pyf said:

@MindWorX: an update could prove to be useful indeed...


Click to expand...

Done and done, updated for 1.29.

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

Thanks a lot. Much appreciated.

MindWorX said:

Examples

To use this, simply create a shortcut to War3.exe and in Properties->Shortcut, modify the Target.

"C:\Games\Warcraft III\War3.exe" -window (Starts the game in windowed mode.)

"C:\Games\Warcraft III\War3.exe" -window -opengl (Starts the game in windowed mode with OpenGL rendering.)

Click to expand...

Maybe this paragraph could also be updated / expanded for v1.29+?

#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

Done!

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

You are faster than lightning.

Thanks again.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Holy cow, I'm just now reading all of this, and this caught my eye:

 Modding

This is a single command that allows you to load all resources from a different directory. Useful for things like total conversions.[/SIZE]

datadir <path>

Click to expand...

*What?* Am I reading this right? "Resources" as in "custom imported resources"??

#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

Kyrbi0 said:

 *What?* Am I reading this right? "Resources" as in "custom imported resources"??


Click to expand...

More like, you supply your own war3.mpq, war3x.mpq, etc and the game uses that to load instead of the files in the installation dir. It's a very big task to actually use it. I'm guessing it has been used internally for testing.

The new plugindir stuff might be interesting though. It appeared in 1.29, but is undocumented still. Might mean we get some way to add plugins in the future.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

MindWorX said:

 More like, you supply your own war3.mpq, war3x.mpq, etc and the game uses that to load instead of the files in the installation dir. It's a very big task to actually use it. I'm guessing it has been used internally for testing.

The new plugindir stuff might be interesting though. It appeared in 1.29, but is undocumented still. Might mean we get some way to add plugins in the future.


Click to expand...

I don't know what plugins are, really, or what they could be used for. But I'm *very* interested in that bit about custom MPQs, since that's a big part of what I'm using for my projects (which, if I'm not mistaken (??) was deprecated with a recent Patch)... Is *that* a method to avoid getting in the way of the Battle.net verifier? Or to replace SEMPQs?

And even if not, I suppose it could still be really great for testing my custom MPQs myself, dynamically, without needing to physically replace my standard MPQs.

Am I reading that right? (Can't test now, at work)

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

Any additions / changes with v1.30?

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

Bump!

@MindWorX

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

Command Line and In-Game Options for Classic Games

Now I am soooooo confused:

#### loktar

#### loktar

 Level 47

JoinedNov 2, 2004Messages2,348

Resources: 90

#### Resources
Models 48Icons 30Tools 2Maps 4Spells and Systems 5Tutorials 1

##
 Melee Classics


Are there any known arguments for World Editor?

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

Now that Reforged is live, does the OP need any update for v1.32.x?

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

pyf said:

 Any additions / changes with v1.30?


Click to expand...

pyf said:

 Bump!

@MindWorX

Click to expand...

pyf said:

Command Line and In-Game Options for Classic Games

Now I am soooooo confused:

View attachment 306532

Click to expand...

loktar said:

 Are there any known arguments for World Editor?


Click to expand...

pyf said:

 Now that Reforged is live, does the OP need any update for v1.32.x?


Click to expand...

hyperbump!

@MindWorX

#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

There are other command line arguments, and there have been other ones left undocumented from the 2016 era too, like all the commands used when running a map from the command line, and the very important -launch to launch the game through the exe directly in 1.32.x. Other than this there are some miscellaneous arguments that are unlikely to be very useful to us. A probably outdated but more complete list I have is as follows (I also don't guarantee my values for the arguments nor the arguments themselves are fully correct, but the map testing ones should be and you can instead see the HiveWE code for it for a guaranteed to be correct reference):

2018-08-13:

d3d

swtnl

assert

datadir

buildinfo

loadfile

gametype

locale

rockey

tftkey

nowfpause

plugin

worldeditplugindir

graphicsapi

routerconfig

routerapp

enablenagle

automation

errordir

2020-05-05

launch

crash

sso

loadfile

gametype (none/versus/custom/replays/campaign)

region (us/eu/kr/tw/cn/sg/xx)

locale

audiolocale

teen (0/1)

hd (0/1)

windowmode (windowed/windowedfullscreen/fullscreen)

width

height

vsync

nowfpause

plugin

graphicsapi

soundapi

csdk-aurora-dnsname

csdk-gameutil-filter

enablenagle

audioquality

routerconfig

routerapp

automation

errordir

mapdiff (difficulty number)

testmapprofile (profile name)

fixedseed

scriptingenginevm

#### tillinghast

#### tillinghast

 Level 55

JoinedJun 2, 2008Messages946

Resources: 99

#### Resources
Models 97Icons 2

i seem to be unable to use more than one command at once, and some don't work at all

is this because i'm using 1.26?

#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

When using arguments, be sure to watch out for those that require additional values for them, like loadfile, mapdiff, and testmapprofile.

"E:\Warcraft III 1.26.0 backup\War3.exe" -window -loadfile "Maps\Test\WorldEditTestMap.w3m" would be what 1.26 would use to start up a map located under "E:\Warcraft III 1.26.0 backup\Maps\Test\WorldEditTestMap.w3m". As 1.26 is an ancient version it doesn't allow for testmapprofile, mapdiff and many other arguments to be passed on the command line and the value passed to -loadfile must be under the game's install directory and has to be a relative path.

#### tillinghast

#### tillinghast

 Level 55

JoinedJun 2, 2008Messages946

Resources: 99

#### Resources
Models 97Icons 2

@Bogdan3 i tried using -window -nowfpause and the first command worked but the second one didn't

i swapped them around and they both didn't work

then i tried using the -graphicsapi Null just to see if it crashes and it didn't

now i tried -window -loadfile with a relative path and both of those lines worked

i'm only interested in -nowfpause and it looks like it doesn't work on 1.26, oh well

#### BogdanW3

#### BogdanW3

 Level 20

JoinedFeb 25, 2013Messages359

Resources: 5

#### Resources
Tools 2Tutorials 3

Nowfpause is one of the commands that do not work in 1.26, that is correct. The Window focus pause toggle probably wasn't even a thing back then - I'd not suppose it existed before 1.29.

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

Bogdan3 said:

 [...] and the very important -launch to launch the game through the exe directly in 1.32.x [...]


Click to expand...

@Bogdan3: Yeah, I am myself using that one with StarCraft. It is very useful and fit for that game, because its free content never expires, and also because I do not play online. This is why I am still at v1.23.6

tillinghast said:

 [...] i'm only interested in -nowfpause and it looks like it doesn't work on 1.26, oh well


Click to expand...

Bogdan3 said:

 Nowfpause is one of the commands that do not work in 1.26, that is correct. The Window focus pause toggle probably wasn't even a thing back then - I'd not suppose it existed before 1.29.


Click to expand...

@tillinghast: please see if it has been implemented in Warcraft Feature Extender. If not, then maybe you can post something about -nowfpause in its support thread, in the hopes the author might be interested in it?

#### tillinghast

#### tillinghast

 Level 55

JoinedJun 2, 2008Messages946

Resources: 99

#### Resources
Models 97Icons 2

@pyf i'm good

#### KittyMaster

#### KittyMaster

 Level 1

JoinedJul 26, 2021Messages2

Are there something like Complete Registry Guide for Warcraft 3?

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

See Bogdan's post above for Reforged args.

1.33.0 added -legacykerning

 Updated the GPU renderer to improve performance, with a slight change to the appearance of the font. To enable the old font rendering, run the commandline paramter “-legacykerning”. This can be done in the setting options in the battle net launcher.


Click to expand...

EDIT:

The new renderer's text is wider, each character is more spaced out. Most annoying to me is the chat input. Before it looked like "helloI" now it looks like "hello I" (the gap to the cursor is so big it looks like a space)

If your text doesn't fit on leaderboards and tables, that's why. The text is much wider.

 You must log in or register to reply here.


### Similar threads

 Play Warcraft in a Window


Ralle

Jan 3, 2006

Misc/General Mapping Tutorials

Replies3Views35K

Mar 14, 2014
Chaosy

 List of official patches for Warcraft 3


Firstrun70

Feb 29, 2020

Misc/General Mapping Tutorials

Replies8Views78K

Aug 3, 2025
Luashine

 Running War3 in Multi Instance


Mr.Pockets00000

Sep 24, 2015

Misc/General Mapping Tutorials

Replies7Views7K

Feb 24, 2016
Mr.Pockets00000

Locked

 Volunteers Needed - Notepad++ vJASS


Nestharus

Jul 20, 2013

Warcraft Editing Tools

Replies19Views7K

Jul 30, 2013
Nestharus

Locked

 How to fix -loadfile not loading the map, main menu instead (Warcraft 3 Classic)


Luashine

Nov 14, 2023

Warcraft Discussion

Replies1Views2K

Nov 14, 2023
LeP

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