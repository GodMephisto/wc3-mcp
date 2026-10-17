---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/cinematic-movies-tutorial.254860/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Cinematic Movies Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/cinematic-movies-tutorial.254860/>
#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Cinematic Movies Tutorial

Tools: World Editor, Trigger Editor, Windows Registry Editor, Any Media Player.

Difficulty:Advanced.

This tutorial will be about cinematic movies, those beautiful cinematics that you see often at the beginning, end and the middle of single player campaigns. They are located in your Warcraft III installation directory in a folder named - Movies.

If you open this folder you will see various .mpq files, those .mpq files are actually cinematics that are played during campaign. To view and watch them you have to rename .mpq to .avi. If you haven't finished playing both games I advise you not to watch those cinematics, it will contain huge spoiler of the story-line.

Blizzard had put a sort of protection in renaming .avi to .mpq, they have done this to prevent new players from accessing cinematics and spoilers early on in the game. Once you renamed files to .avi you can watch them in any Media Player that supports .avi format.

Notice: Cinematics only work if they are in .mpq format, the game specifically is coded to search for this format and then in-game unwrap it into .avi.

Now let's create our first custom cinematic:

I already have my video (cinematic) in .avi format and ready.

Move your custom cinematic to Movies folder located in your Warcraft III installation, rename the file ending to .mpq.

Now you will have to open up Trigger Editor and find an action in Cinematic category named Cinematic - Play Movie. If you can't find the trigger then use a custom script: Custom Script: call PlayCinematic("<Cinematic>").

<Cinematic> - is your movie file name, but without the ending.

Ex: MyCinematic.mpq = Bad, MyCinematic = Good

Here is how the rest of trigger should look like to run a cinematic instantly after the loading screen:

You can now test your cinematic map my pressing Test Map button.

As you noticed that your cinematic was running in 800 x 600 resolution that was preset by Warcraft III, you can change your cinematics color depth, refresh rate, and resolution in the registry. To do that, you will have to press Start and type in: regedit - then press enter and it will open Registry Editor.

Now you will find these main options for configuring your cinematic display in Warcraft III:

Cinematic Override - enables custom registry changes.

Cinematic Width - resolution width of the cinematic.

Cinematic Height - resolution height of the cinematic.

Ex: 1024 x 768.

Cinematic BPP - color depth of the cinematic.

Ex: 8 - Bit, 15/16 - Bit, 18 - Bit, 24 - Bit, 30-48 - Bit. More information about color depth.

Cinematic Refresh - monitor refresh rate of the cinematic.

Important - to edit the values and numbers of the selected setting you must always click on decimal and write your value (number).

Ex: Click "CinematicWidth" option in registry and click decimal, then enter the width number (value) of the cinematic: 800. 800 (width) x 600 (height) = 800 x 600 resolution. Then click OK to save.

Notice: You must enable Cinematic Override to 1 for custom registry changes to take effect.


 Last edited: Jun 15, 2016

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Extremely nice, I was thinking about doing something like this.. sometime..

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Cool tutorial. It is nice that someone made a tutorial for this. I always had to search for my old post on it if someone asked about it. But I like this tutorial's organization much better.

But the tutorial should be in regular text. You can use that color for bold things, e.g. "Notice", or for any headers, but you should use default text for the rest of it (and non-italicized).

I didn't know about the registry settings. It is cool that you found that out.  If you have time, it could be cool if you could fiddle with trying to display captions. See the bottom of this thread:

https://www.hiveworkshop.com/forums/2390249-post11.html

I never tried it out, but I assume that if you can put a captions file in the correct location under the same name. e.g. HumanEd corresponds to UI\Captions\HumanEd.txt. It would be awesome if you had a part about that (if it works).

Also, the name could use some work. We usually associate cinematics with triggered cinematics. There are a few different options: AVI, video, movie, cinematic movie. e.g. "Play Custom AVI's". Or "Play External Movie". It'll be easy for someone to locate, and they'll understand what the tutorial is about just from the title (titles are really important!).

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

PurgeandFire said:

 Cool tutorial. It is nice that someone made a tutorial for this. I always had to search for my old post on it if someone asked about it. But I like this tutorial's organization much better.

But the tutorial should be in regular text. You can use that color for bold things, e.g. "Notice", or for any headers, but you should use default text for the rest of it (and non-italicized).

I didn't know about the registry settings. It is cool that you found that out.  If you have time, it could be cool if you could fiddle with trying to display captions. See the bottom of this thread:

https://www.hiveworkshop.com/forums/2390249-post11.html

I never tried it out, but I assume that if you can put a captions file in the correct location under the same name. e.g. HumanEd corresponds to UI\Captions\HumanEd.txt. It would be awesome if you had a part about that (if it works).

Also, the name could use some work. We usually associate cinematics with triggered cinematics. There are a few different options: AVI, video, movie, cinematic movie. e.g. "Play Custom AVI's". Or "Play External Movie". It'll be easy for someone to locate, and they'll understand what the tutorial is about just from the title (titles are really important!).


Click to expand...

Aye! I will update it.

Edit: How can I change the name to - "Cinematic Movies Tutorial" ?


 Last edited: Jul 17, 2014

#### Heinvers

#### Heinvers

 Level 53

JoinedMay 7, 2010Messages9,267

Resources: 101

#### Resources
Models 3Icons 65Packs 3Skins 27Templates 3

Make a thread in staff contact and ask there for the name to be changed.

#### Binsu

#### Binsu

 Level 30

JoinedNov 29, 2012Messages6,631

Resources: 5

#### Resources
Maps 4Templates 1

Well, I changed it for you.

Have a good day and a very helpful tutorial. To be honest here, I've got no idea or even a little that you could do this and now, I've learned something from this!

Good job.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Hell_Master said:

 Well, I changed it for you.


Have a good day and a very helpful tutorial. To be honest here, I've got no idea or even a little that you could do this and now, I've learned something from this!


Good job.


Click to expand...


Thank You

#### machadomarrano

M

#### machadomarrano

a problem

Forgive my intrusion... but I don't see that option anywhere in the trigger editor from the WORLD EDITOR... did blizzard removed that option? I just can't find it. Thank you for your answer!

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

machadomarrano said:

 Forgive my intrusion... but I don't see that option anywhere in the trigger editor from the WORLD EDITOR... did blizzard removed that option? I just can't find it. Thank you for your answer!


Click to expand...


Do you have Jass New Gen World Editor? Otherwise if you do not you can use:


Custom Script: call PlayCinematic("Cinematic Name")

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Wtf, sorry for letting this sit so long. Great job with the tutorial--I like the topic--and very few people know that this is even possible.

#### darwin

#### darwin

 Level 14

JoinedAug 30, 2004Messages909

Resources: 3

#### Resources
Icons 1Maps 2

Is this option there for custom campaigns that are uploaded on the site here? Would you have to upload the .avi files and give instructions for the downloaders to put them in the right folder?

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

darwin said:

 Is this option there for custom campaigns that are uploaded on the site here? Would you have to upload the .avi files and give instructions for the downloaders to put them in the right folder?


Click to expand...


If you are making a campaign with custom cinematic, then yes. Those cinematics will have to be uploaded somewhere (pastebin)on this site and then put in correct folder to view them. folder - Movies.

#### ladiszlai

#### ladiszlai

 Level 1

JoinedMar 3, 2016Messages2

Hello,


Somebody can help me where can I put this command ? :


"call PlayCinematic("<Cinematic>")"

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

ladiszlai said:

 Hello,


Somebody can help me where can I put this command ? :


"call PlayCinematic("<Cinematic>")"


Click to expand...

#### ladiszlai

#### ladiszlai

 Level 1

JoinedMar 3, 2016Messages2

Thank you

Edit

Ok I tried this is working but I needed to add this script too:

call EndGame(True or False)

If I not add this the cinematic won't play.


 Last edited: Mar 4, 2016

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

Thanks for the tutorial.

I understand you refer to FMVs " at the beginning, end and the middle of single player campaigns ".

Are there really FMVs in the middle of WC3's campaigns ? Maybe you refer to WCII here.

The files in WC3's Movies folder, are standard avi containers with the mpq file extension. One may use MediaInfo Lite to know everything about them (program description at MajorGeeks).

As a "protection" Blizzard used, for the video part, the fourcc BLZ0, which is exclusive to them afaik. Back then, media players could not split/decode the files properly.

Nowadays, FFDShow supports BLZ0. FFDShow is both standalone, and part of codec packs such as K-Lite. VLC can also play such files (no need for a codec pack in that case).

To watch these FMVs in a media player, you can directly drag-and-drop the "mpq" files in any decent media player (no need to rename them to avi beforehand). Works with VLC or MPC-HC. It will *not* work with Windows Media Player 11 (you have to rename the file to avi first).

The FMVs may play on some HDTVs, which may have unofficial support for them. You will very certainly have to rename the files to avi first. For some reason, the same manufacturer may unofficially support BLZ0 in a specific product, then ditch/break BLZ0/audio support the year after in another of its products (no thanks, Samsung !). Therefore, your mileage may vary. Experiment, guys.


 Last edited: May 16, 2016

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

I find this tutorial very useful ! Thanks for sharing

#### anzelayne

#### anzelayne

 Level 5

JoinedJun 13, 2011Messages57

How to access custom cinematics in patch 1.30+? .mpq no longer seems to work.

#### pyf

#### pyf

 Level 32

JoinedMar 21, 2016Messages2,985

Resources: 4

#### Resources
Tutorials 4

With the right tool?

Casc Storages - Main page

 You must log in or register to reply here.


### Similar threads

 Custom Movie Tutorial (patch 1.29)


Tommi Gustafsson

Apr 18, 2018

Misc/General Mapping Tutorials

Replies1Views4K

Jun 4, 2018
PurgeandFire

 Custom Movie Tutorial (patch 1.30.1+)


Tommi Gustafsson

Oct 30, 2018

Misc/General Mapping Tutorials

Replies12Views8K

Sep 21, 2025
Regno

 Various tips for a cinematic maker


bounty hunter2

Jan 4, 2009

Misc/General Mapping Tutorials

Replies11Views8K

Mar 19, 2009
Aeroblyctos

 How to make Cinematics in Warcraft III - Complete Series


Turnro

Apr 8, 2022

Misc/General Mapping Tutorials

Replies0Views5K

Apr 8, 2022
Turnro

Locked

 Chaper Cinematic Button on a Custom Campaign


Azsure

May 14, 2016

World Editor Help Zone

Replies7Views3K

May 17, 2016
pyf

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