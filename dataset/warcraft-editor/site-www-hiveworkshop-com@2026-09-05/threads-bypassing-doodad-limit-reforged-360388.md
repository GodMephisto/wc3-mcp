---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/bypassing-doodad-limit-reforged.360388/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Bypassing Doodad Limit Reforged
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/bypassing-doodad-limit-reforged.360388/>
#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


DISCLAIMER: This guide is superceded by this: [Misc] - Bypassing doodad limit with HEX editor

Here's a short video tutorial on how to bypass the doodad limit. I'm pretty sure @Achille was the first person to discover it. This video is just meant to relay the message to people who may be interested in bypassing the limit, I take no credits.

Also, it should be noted that this utilizes cheat engine. I don't endorse cheating, this is purely for modders, to assist them in their map development.

Possible challenges to keep in mind:

This adds a level of hassle when saving your map. To save the map normally after this, you'd have to reduce the doodad count back to 30k, so this should be done when most stuff is complete, unless you want to do this every time you save your map.

Doesn't work with third party compilers, only works with the native way of saving.


 Last edited: Jul 1, 2025

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Creative and clunky, very Wc3 in spirit that way.

Me personally thinks this is too annoying to be worth it.

If you do the terrain before your code where you'd have to use this trick every time you want to save code progress.

If you rely on this trick and they fix it your map is now locked.

Does not work for other languages such as Typescript and Wurst? they need to recompile the map and inject their code, I have no idea if this is blocked by editor limit or if it bypasses it naturally by not using the editor.

If you're blocked by this while coding you can at least look into hot reloading to reduce the amount of pain.

That said, it's not up to me if this is useful to an individual but I would probably like more 'warnings' on this for newer modders who maybe don't see the full scope of potential downsides and workarounds to reduce these downsides, such as doing the terrain last rather than first.

edit:

Also a more wild thought is that you could script or macro the save process. More food for thought for future readers rather than something reasonable to require in the tutorial.

#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


Chaosy said:

 Me personally thinks this is too annoying to be worth it.

If you do the terrain before your code where you'd have to use this trick every time you want to save code progress.


Click to expand...

Added some warnings. As for if this is worth it or not, at least to me, this is a huge discovery. I have dreaded the doodad limit since forever, so that there's actually now a way to bypass it on the latest patch is groundbreaking. This is definitely something I will be using in my maps, especially at the end, for the finishing touches.

Chaosy said:

 If you rely on this trick and they fix it your map is now locked.


Click to expand...

Your map is not locked, you just have to reduce doodads until you're at 30k if you want to save the map. You can still open it, play it, and change it.

Chaosy said:

 Does not work for other languages such as Typescript and Wurst? they need to recompile the map and inject their code, I have no idea if this is blocked by editor limit or if it bypasses it naturally by not using the editor.


Click to expand...

Doubt it, unless the compilers are made to bypass the save prompts. You could try it, and add your findings to this thread.

Chaosy said:

 If you're blocked by this while coding you can at least look into hot reloading to reduce the amount of pain.


Click to expand...

You won't be blocked by this, you can still open the map.

Chaosy said:

 Also a more wild thought is that you could script or macro the save process. More food for thought for future readers rather than something reasonable to require in the tutorial.


Click to expand...

That would be nice, and I imagine, not too hard. After all, it's just a value that's modified so you can bypass the save prompt, nothing more. So should be ez.

#### SHBlade

#### SHBlade

 Level 14

JoinedMay 1, 2013Messages227

Resources: 1

#### Resources
Maps 1

##
 Gaias Retaliation ORPG


I've tried this a long time ago but it won't work. The editor will only save the specified amount of doodads.

So if you placed 28 doodads but changed the value to 20 via cheat engine, the editor will only save 20 doodads.

#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


SHBlade said:

 I've tried this a long time ago but it won't work. The editor will only save the specified amount of doodads.

So if you placed 28 doodads but changed the value to 20 via cheat engine, the editor will only save 20 doodads.


Click to expand...

It works if you follow the instructions in the video.

Don't take my word for it though, open this map, and check the amount of doodads.

#### Attachments

map.w3m

 524.3 KB

 · Views: 85



#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I'll admit that I use 'blocked' very loosely in the sense that you cannot develop the map as is without changes. Obviously you can still scale it back which might be possible if you're using 35000 let's say, and practically a lot less so with 60000.

As for Wurst / Typescript / Other compiled languages, the reason I am curious is that the limit is obviously not enforced in the game since the map is playable so by manipulating the map directly I suspect it should work.. I might give it a try if I remember it later.

FeelsGoodMan said:

 That would be nice, and I imagine, not too hard. After all, it's just a value that's modified so you can bypass the save prompt, nothing more. So should be ez.


Click to expand...

There are keyboards that have built in macro support that even record mouse clicks which means you could have a dedicated save button that does all of this automatically with the major downside that it's bound by pixel locations so you can't resize windows etc but if you happen to have one of these keyboards that might be a small sacrifice for convenience and I am sure there are more fancy solutions too.

Anyway, cool until fixed - approved.

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


I tried this method with old maps that were already developed in older patches. I could not set the doodad count to 30000 because the count would bounce right back to the actual doodad count.


 Last edited: Jun 18, 2025

#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


Elprede said:

 I tried this method with old maps that were already developed in older patches. I could not set the doodad count to 30000 because the count would bounce right back to the actual doodad count.


Click to expand...

Can you provide me with one of these maps so I can test it?

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


FeelsGoodMan said:

 Can you provide me with one of these maps so I can test it?


Click to expand...

Here you go. Rise of a Realm 1.30 - Warcraft 3 Maps - Epic War.com

#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


Elprede said:

 Here you go. Rise of a Realm 1.30 - Warcraft 3 Maps - Epic War.com

Click to expand...

Hmm I'm having the same results. There's normally 3 addresses that shows up, two of reverts back, one of which can be changed (which also changes the doodad count). It seems for the map you linked (and some other maps I tried), only 2 addresses shows up, where both reverts back if changed. Not sure what exactly causes this behavior, but I'll do some more testing.

#### deepstrasz

#### deepstrasz

Map Reviewer

 Level 84

JoinedJun 4, 2009Messages22,555

Resources: 7

#### Resources
Maps 4Spells and Systems 2Sounds 1

Elprede said:

 I tried this method with old maps that were already developed in older patches. I could not set the doodad count to 30000 because the count would bounce right back to the actual doodad count.


Click to expand...

Can someone check if this map has been ran through optimization software?

Also, to test, various version maps (newly made ones if possible to be sure they haven't been edited with third party programs like optimizers or custom editors) and compile of list of which patch version maps are affected by the trick and which not.

Is the cheat engine update based on some Warcraft III files?

#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


@Chaosy This guide can be graveyarded, and replaced with: [Misc] - Bypassing doodad limit with HEX editor

 You must log in or register to reply here.


### Similar threads

Locked

 Bypassing the doodad limit?


D.I.Y Death

Nov 27, 2008

World Editor Help Zone

Replies9Views2K

Nov 29, 2008
Saishy

Locked

 commision for world editor 1.33 bypass limits


TheKaldorei

Nov 11, 2022

Requests

Replies0Views1K

Nov 11, 2022
TheKaldorei

Locked

[General]
 World Editor 1.31 Doodad limit


FeelsGoodMan

May 25, 2019

World Editor Help Zone

Replies4Views2K

May 25, 2019
FeelsGoodMan

Locked

 MPQ editing or Bypass map size limit?


deafir

Dec 11, 2016

Warcraft Editing Tools

Replies9Views4K

Jan 10, 2017
Barade

Locked

 Need some way to bypass the editor terrain limits


Marsmallos

Nov 6, 2009

World Editor Help Zone

Replies2Views1K

Nov 7, 2009
HELL_YA

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