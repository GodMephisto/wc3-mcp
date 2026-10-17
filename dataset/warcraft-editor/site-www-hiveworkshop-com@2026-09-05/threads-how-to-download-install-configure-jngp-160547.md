---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-download-install-configure-jngp.160547/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Download Install Configure Jngp
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-download-install-configure-jngp.160547/>
#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Edit by Purge:

Note:You can avoid a lot of stress just by downloading from this link instead:

https://www.hiveworkshop.com/forums/tools-560/jassnewgenpack-5d-227445/

Please check out the troubleshooting section before downloading. Make sure your antivirus is configured so that you will have a proper download and patch your Warcraft 3 to 1.26.

Actual tutorial:

For the 100th time this year I have to tell a guy how to probably download, install and configure JNGP In order to make it work. So I wrote this short tutorial to not have to answer it every time again and again.

Requirements

Download

Install

Update JASSHelper

Configure

Using

Settings

Misc

Requirements​

Windows or windows emulator

Latest Version of Warcraft III.

Download​
Open this url, download the JNGP file.

Alternative: JNGP 5d with JassHelper 0.A.2.B.

Install​
Extract it anywhere you want.

Update JASSHelper​
Open this url, download the JassHelper file.

Extract the content.

CnP this three files:

clijasshelper.exe

jasshelper.conf

jasshelper.exe
, go to the JassNewGenPack folder, open it, and copy it into the subfolder "jasshelper" and click yes to overwrite.

Configure​
Open JNGP. (For that, you need to open "NewGen WE.exe"

If your antivir program does block something, set it to the exception list! This is not a virus, even if it may seem like it for antivirus!

Check the menu. Click on Reinventing the Craft, check the box "Disable RtC".

Close the JNGP instantly.

Open JNGP again, now you can use it with vJASS support.

Using​
To open the map, simply open it in the menu or with CTRL + O.

To save the map, please use Menu - Save (CTRL + S).

To save the map under a new name, press save as and then press save (CTRL + S) again!

If you edit your map (anything, while this can be scrolling in the trigger editor aswell) you can NOT play the map!

You have to save it again with CTRL + S in order to testplay it!

Settings​
Please make sure the settings of your JNGP match the following:

When saving for real version please uncheck "Debug Mode" in JassHelper menu.

Misc​
Grimoire

[X] Start War3 with -window

 If enabled opens Warcraft III in a window when testplaying maps.

 This does NOT change the normal Wc3 startup.

[ ] Start War3 with -opengl

 If you enable this you will use OpenGL (GraphicFormat).

 This is only useful when you know what you do.

[X] Enable TESH

 Like a theme manager.

[X] Enable Colorizer

 If you want to choose your own colors for coding, you can

 enable it here.

[ ] Enable No limits

 Removes the World Editor limits.

 For example the 8000 doodad limit will be removed and you

 will be able to place more doodads!

[X] Enable object editor hack

 Allows you to put in field values that are normally not valid

 for the WE.

[X] Disable default description nag

 If you go save a map in normal WE and you haven't changed name from "Just anther warcraf 3 map" Then a dialog pops up, letting you know that you haven't.

This disables/enables that dialog.

Thanks to Cweener for this answer.

[X] Don't let WE disable triggers.

 When your trigger has one or more invalid actions WorldEditor normally

 disables it. You can prevent the WE from doing so here.

[X] Always allow trigger enable.

 Same for above, but this time you are able to enable it.

[X] Mute Editor Sounds

 You can mute the sounds here. I find it more annoying to

 have sounds in the world editor. Listen to music instead.

[X] Disable First Save Warning

 The WE pops up a little annoying message that you are saving the

 map for the first time. You can disable it here.


 Last edited by a moderator: Dec 22, 2012

#### karland90

#### karland90

 Level 14

JoinedMar 30, 2009Messages970

Resources: 3

#### Resources
Models 3

btw for users of AVG: avg has 2 different lists of exceptions. one for pup (potentially unwanted program) and one for resident shield. i had trouble with it, so i added the files to both exception lists, just to be sure

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Updated.

#### Dynasti

#### Dynasti

 Level 18

JoinedOct 18, 2007Messages929

Resources: 5

#### Resources
Maps 2Spells 1Tutorials 2

Done everything this post tells me to do. Doesn't work

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

What exactly doesn't work? Tell me what you do and what it does.

#### karland90

#### karland90

 Level 14

JoinedMar 30, 2009Messages970

Resources: 3

#### Resources
Models 3

same here. updated and tested :S

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Can you please elaborate everything you did? ^^

#### karland90

#### karland90

 Level 14

JoinedMar 30, 2009Messages970

Resources: 3

#### Resources
Models 3

i made a copy of your map to my desktop.

opened it up in the NewGenWE.exe.

moved one of the trees in map.

pressed CTRL+S

got this error:

EDIT: how can i have map version 0.1.Y.3?? you have uploaded y.2

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

I mean the installation of JNGP t the point you get the errors. Everything.

#### karland90

#### karland90

 Level 14

JoinedMar 30, 2009Messages970

Resources: 3

#### Resources
Models 3

well up to the points i mentioned in my previous post i did exactly as you told in this tutorial:

extract.

replace the 3 files.

run.

disable rtc.

rerun.

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Make sure you have vJass enabled in JassHelper menu.

#### karland90

#### karland90

 Level 14

JoinedMar 30, 2009Messages970

Resources: 3

#### Resources
Models 3

like this?

#### Attachments

screen.JPG

 25.1 KB

 · Views: 1,811



#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

No!

Please uncheck 'Disable vJass Syntax'.

#### karland90

#### karland90

 Level 14

JoinedMar 30, 2009Messages970

Resources: 3

#### Resources
Models 3

it lives!

-You must spread some Reputation around before giving it to Anachron again.-

-.-

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Sure thing, as I told.

But why is everything enabled in your JNGP? Are you sure you are using the newest version and not an old version you already used?

A new JNGP version automatically disables most stuff and enables vJass syntax.

#### karland90

#### karland90

 Level 14

JoinedMar 30, 2009Messages970

Resources: 3

#### Resources
Models 3

well i deleted the jasshelper and JNGP folders last night. went on wc3c to get me latest versions, but found out that the .zips i kept on my computer just in case were the most up to date. then i extracted them again. and yes i think these are all the initial settings (well it doesnt keep any saved info elsewhere, does it?)

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

I think it does (in Registry).

Edit: Updated first post with JNGP menu settings.

#### syltman

#### syltman

 Level 14

JoinedJun 13, 2007Messages1,430

 to this path "\$JNGPFOLDER$\jasshelper" and click yes to overwrite.


Click to expand...


don't type \$JNGPFOLDER$, it's confusing. Insteed tell them to go to the JNGP folder and then to the jasshelper folder.

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Thanks, I fixed it.

#### real138

#### real138

 Level 15

JoinedSep 3, 2009Messages1,520

Resources: 14

#### Resources
Icons 10Packs 1Spells and Systems 3

hmm good tutorial but i just noticed that i didnt have reinventing the craft in my WE i tried reinstalling everything but yet again it hasn't shown up. do you know any reason that might be? don't think i need it anyway since im disabling it just would like to know.

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

That means your antivir is blocking the ReinventingTheCraft hack.

#### real138

#### real138

 Level 15

JoinedSep 3, 2009Messages1,520

Resources: 14

#### Resources
Icons 10Packs 1Spells and Systems 3

ah ok the rtcexehack right? but i dont need it right? or should i fix it anyways?

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Antiviruses normally block every RTC executable file.

And no need to fix if it does compile correctly.

(For me it gives errors, so I had to put it in the exception list)

#### real138

#### real138

 Level 15

JoinedSep 3, 2009Messages1,520

Resources: 14

#### Resources
Icons 10Packs 1Spells and Systems 3

ah ok well it compiles fine but just in case i'm gonna add it to the exceptions 2 mins won't kill me . great tutorial for people new to JNGP. +rep about time someone did it.

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Originally I wanted to make it longer and more described, but I said dude, no one wants to read a wall of text just to get his answer!

So I made it short but informative. I hope you like it.

#### real138

#### real138

 Level 15

JoinedSep 3, 2009Messages1,520

Resources: 14

#### Resources
Icons 10Packs 1Spells and Systems 3

ya thats all that's really needed. but maybe put a mini description of the menu items like a sentence. i know at first i didn't know what half that stuff was for.

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Good idea, will add it later.

#### Etzer

#### Etzer

 Level 13

JoinedFeb 18, 2009Messages1,378

I don't have the "reinventing the craft" when i open the NewGen WE.exe

EDIT : Nomatter, i just found out that the guy above me had the same problem, i'll try to fix it.

2nd EDIT : You know where the exception list for AVG 2009 is?

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Maybe make a google search?

#### Etzer

#### Etzer

 Level 13

JoinedFeb 18, 2009Messages1,378

Anachron said:

 Maybe make a google search?


Click to expand...


Oh yeah google 4 life <3

#### karland90

#### karland90

 Level 14

JoinedMar 30, 2009Messages970

Resources: 3

#### Resources
Models 3

as i stated in the first post of this thread (after Anachron ofc) avg has two different exception lists.

first one: open interface. open resident shield. on the lower right of the main part of the window is manage exceptions

second: form toolbar open tools -> advanced settings. somewhere in the middle of the list of the newly opened window is PUP exceptions

hope this helped

#### Etzer

#### Etzer

 Level 13

JoinedFeb 18, 2009Messages1,378

Yea. No, i just found out that i have PAV  Somehow, it have switched...

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Bumpy.

#### MortAr

#### MortAr

 Level 19

JoinedFeb 25, 2009Messages2,004

Resources: 11

#### Resources
Icons 6Packs 1Spells 2Tutorials 2

Hmm, what about UMSWE, you didn't mentioned any special options for it, and should it be enabled/disabled?

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

First post includes now a guide what the settings of Grimoire do.


I never worked with UMSWE :/ Will check it though.

#### phoenixfire

#### phoenixfire

 Level 12

JoinedJun 10, 2008Messages1,043

Hey, "Reinventing the Craft" isn't one of my options. Is there something wrong with my JNPG?


EDIT: Also none of the things from the "Extensions" section work.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Do you have the latest NewGen installed? If you don't have RtC, it is fine since it is currently disabled by almost all NewGen users anyway. (At least for the moment, until MindWorX and SFilip release the next version)

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

nuuh, as I already said, make sure you antivirus DOES not block anything of JNGP.

It seems like it blocked RtC.

#### karland90

#### karland90

 Level 14

JoinedMar 30, 2009Messages970

Resources: 3

#### Resources
Models 3

lol i added some of the detected files to 2 different safelists of avg and the only thing that avg found during the last scan were those same files again -.-

and i had to restore them from quarantine again, cause i have no options.

#### phoenixfire

#### phoenixfire

 Level 12

JoinedJun 10, 2008Messages1,043

Anyways, everytime I click on something in the "Extensions" menu, nothing happens. I also installed this while my Antivirus system was down.

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

You do not only have to have it disabled when its on install, but also after. You better put JassNewGenPack folder in your antivirus exception list.

Once a file of JNGP has been deleted due to AntiVir or Firewall, you should reinstall it.

#### machris13

#### machris13

 Level 7

JoinedMar 22, 2010Messages228

i did everything you said,..


but when i look in trigger editor it is till in GUI?


im not running the old WE, im running the NEW PACK...


what will i do??

#### real138

#### real138

 Level 15

JoinedSep 3, 2009Messages1,520

Resources: 14

#### Resources
Icons 10Packs 1Spells and Systems 3

.... its still gonna be in GUI. just when you convert a trigger to custom scripts will you have the options for JNGP

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Triggers don't magically change to vJass.

You have to convert them to CustomScripts. (Edit -> Convert to custom script)

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice tutorial overall. Well explained, useful to those who can't setup New Gen properly, and well-formatted.


Good job, ~Approved.

#### Oziris

#### Oziris

 Level 27

JoinedOct 14, 2006Messages4,161

Resources: 7

#### Resources
Icons 1Skins 3StarCraft II Resources 1Tutorials 2

I still cant setup the New Gen properly.

We.Lua

Couldn't start World Editor. An instance of World Editor may already be running.

If running a patch prior to 1.21b make sure the Warcraft III CD is in your drive and try again

If your using kaspersky antivirus, turn it off before using NewGen.

I have Kaspersky and i turn it off, but it doesn't help. It still gives me the error. I updated the jasshelper. But nothing helped. :'(

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Make sure you followed every single step. Just don't think "Oh alright, I've already done this, lets skip this".

Turn off all antivirus, delete JNGP fully, use my tutorials, add JNGP folder to the virus exception list and start JNGP again.

#### Oziris

#### Oziris

 Level 27

JoinedOct 14, 2006Messages4,161

Resources: 7

#### Resources
Icons 1Skins 3StarCraft II Resources 1Tutorials 2

I turned off the protection of the Kaspersky, downloaded the JNPG into a folder, made the folder a trusted zone. I copied the jasshelper, pasted it and started the editor but the same error came out again. I really don't know what to do anymore.

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Maybe ask at wc3c.net

#### indomitable1319

#### indomitable1319

 Level 19

JoinedJan 22, 2010Messages2,583

Resources: 5

#### Resources
Maps 3Spells and Systems 1Tutorials 1

Anarchon, I really need to use JNGP so I can continue my map, without the fear of too many doodads. I download it, but when I try to unizip it, it says that it's corrupted or something. Please help.

[EDIT]


 WinZip


Cannot open file; it does not appear to be a valid archive.


If you downloaded this file, try downloading it again.


That's what WinZip says.

 You must log in or register to reply here.


### Similar threads

 Integrating Tools Into JNGP


PurgeandFire

May 7, 2013

Misc/General Mapping Tutorials

Replies27Views10K

Sep 26, 2013
PurgeandFire

Locked

 How do you install JNGP?


MushiKeewi

Feb 16, 2015

Warcraft Editing Tools

Replies8Views3K

Feb 18, 2015
Daffa

 How To: Run vJASS on a Mac Computer


tooltiperror

Jun 3, 2010

Misc/General Mapping Tutorials

Replies2Views4K

Jun 7, 2010
tooltiperror

Locked

 JNGP Help. Not working.


Shdow89

Oct 11, 2009

Warcraft Editing Tools

Replies10Views3K

Feb 6, 2010
DevilDuke1995

 Setting up NewGen WE and Intro to Trigger Editor


Nestharus

Jan 28, 2011

Misc/General Mapping Tutorials

Replies17Views23K

Aug 11, 2011
Nestharus

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