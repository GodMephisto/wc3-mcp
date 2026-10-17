---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-music-sounds-work-in-newer-patches.313010/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making Music Sounds Work in Newer Patches
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-music-sounds-work-in-newer-patches.313010/>
#### _lycan

#### _lycan

 Level 19

JoinedJan 12, 2011Messages1,537

Resources: 4

#### Resources
Maps 3Tutorials 1

Hello fellow wc3 modders, today I will bring you a tutorial for something that is very widely used and has been changed as of recently. Making music and sounds work in your map, yes, as of recently Blizzard has changed which sounds and music work in Warcraft 3, due to that, you are now required to have the files in specific format which Blizzard mp3s use.

This also serves as a tutorial to optimize the music in your map in case this got reverted, because a 320kbps MP3 can go above 10mb even if it is less than 10 minutes, however in this format it will most likely be 1/3rd of the original size, saving you tons of space.

Step 1)

Download and install audacity from here:

Download | Audacity ®

Step 2)

Install it and Run It

Alright now that we are at our main screen, you will be asked for a DLL once you try to convert music, when you get asked for it, you will have to choose a path to select the DLL in your computer, the DLL can be downloaded from:

free mp3 converter - Lame for Audacity - FFMpeg for Audacity - Free and Safe downloads

Once you downloaded the DLL, just place it on an easy to access folder and once you are prompted for the missing DLL you just select it.

Alright, let's continue with the tutorial.

First open preferences for Audacity:

Then set both the default sample rate and default sample format to the following values:

After that you will open your desired input (original) music/sound:

As you can see to the left of the track you can see the current settings of the music/sound, still aren't at our desired Output format that Warcraft 3 supports. So we will resample the music/sound:

For the new sample rate, select 22050:

Now you should be done with almost everything except the bit-rate, for this we will click on export -> export MP3:

Inside export MP3 you will select the following settings:

And lastly you just click save where you want the converted and optimized MP3 to be at so you can import it to your map.

(In short, your music/sounds must have its project rate set to 22050 Hz, Bit-Depth to 16 Bit PCM, Bit Rate mode to constant (128 Kbps) and Channel mode to Stereo).

That is basically all that you need to do, of course this can also be done with other sound editors, I did pick Audacity because it is a free and open source software.

(By the way, I prefer mp3s for music, however for sounds instead of exporting as MP3 select export as WAV)

#### Attachments

step4.JPG

 140.3 KB

 · Views: 173




 Last edited: Feb 25, 2019

#### Yeran

#### Yeran

 Level 3

JoinedSep 27, 2010Messages15

This doesn't fix the main problem with the new sound bugs.

If you stop the sound after fading it will cut off part of the sound and if you don't it will eventually bug out and not play the sound ever again.

#### _lycan

#### _lycan

 Level 19

JoinedJan 12, 2011Messages1,537

Resources: 4

#### Resources
Maps 3Tutorials 1

Yeran said:

 This doesn't fix the main problem with the new sound bugs.

If you stop the sound after fading it will cut off part of the sound and if you don't it will eventually bug out and not play the sound ever again.


Click to expand...

This isn't a fix to game bugs, this is a way to have your music actually play in-game due to it requiring a specific compression.

#### Yeran

#### Yeran

 Level 3

JoinedSep 27, 2010Messages15

I'm just saying sounds won't work properly currently no matter what you do.

This is also not the only compression format that will play your sounds (albeit bugged as stated). In fact I'm unsure which format even stopped working. Mono, and different Hz value are no problem for sure. Even .wav still plays.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I think this tutorial could benefit from a few structure improvements.

A few titles/sections, "Introduction" "Requirements" (programs) "Core/Main" and "Summery"

Just a way to easily improve readability and find the specific information you are looking for.

I would say it's a bonus to sprinkle in some color if possible too.

I'll fiddle around and see if the huge file size reduction works out for me, if so this is a must-know trick if you are making maps with third party music.

edit: I tried with a 9.9 mb .mp3 file and it got down to 8 mb, from what my ears tell me there is not a noticeable drop in quality, and it is certainly an improvement but not as big as advertised.


 Last edited: Oct 3, 2020

 You must log in or register to reply here.


### Similar threads

 How to Import sounds/music into wc3 from WoW


The Darkness

Dec 1, 2006

Tutorial Archive

Replies2Views4K

Dec 5, 2006
ArchWorm

 JASS for Music and Sounds [Awaiting Improving: Ralle]


forzamoto

Jul 9, 2008

Tutorial Archive

Replies1Views1K

Jul 18, 2008
Ralle

 How to shrink songs and other sounds


Kaitech[SanD]

Dec 26, 2007

Tutorial Archive

Replies6Views2K

Feb 13, 2008
Wolverabid

Locked

 [Download] Machine, Robot Sounds & Music


neogaidenx

Nov 1, 2008

Sound / Music Art

Replies1Views4K

Dec 20, 2008
kingbdogz

Locked

 Sound Format


Saishy

May 21, 2008

World Editor Help Zone

Replies2Views2K

May 21, 2008
Saishy

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top