---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-play-warcraft-iii-without-picture-stretching-fullscreen-4-3-on-widescreens.262097/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Play Warcraft III Without Picture Stretching Fullscreen 4 3 on Widescreens
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-play-warcraft-iii-without-picture-stretching-fullscreen-4-3-on-widescreens.262097/>
#### ALEXz407

#### ALEXz407

 Level 7

JoinedAug 23, 2014Messages208

Resources: 2

#### Resources
Tutorials 2

It's not a secret that the majority of monitors nowadays are widescreen.

However, Warcraft III was created when most of the monitors used CRT and 4:3 aspect ratio. So we can say his native resolutions are 4:3, all others will just stretch your view horizontally.

1920x1080 (16:9)
 |
1728x1080 (16:10)
 |
1440x1080 (4:3)

 |

 |

If you are tired of seeing flattened faces of characters, here is a way to change the aspect ratio without any additional software.

Warning! Modern monitors often able to work with non-standard resolutions, but old monitors can just turn black.

##
 Part I. Monitor settings

First you need to check: Your monitor has the desired mode or not. How to do it?

1. Change your resolution to lower with aspect ratio 4:3 (for example 800x600).

2. Open your monitor options with buttons on the monitor.

Further steps will depend on your monitor, try to find the closest in meaning:

3. Open "Picture Advanced" menu tab.

4. Select "Display Mode" option.

Display Mode in BENQ options

5. Change "Display Mode". It will be "Full" by default (stretch video signal to all available space), change it to "Aspect" (strecth video signal keeping aspect ratio) you will notice that now your image have 4:3 aspect ratio, but blurred.

Full mode
 |
Aspect mode

 |

6. Also find "Overscan" option, turn it off if it's enabled. (this option stretches the image off the screen)

With overscan
 |
Without overscan

 |

7. Now you can go to the part II.

##
 Part II. System settings

Now you know what your monitor can handle 4:3 aspect ratio without problems. You want to set awesome resolution in Warcraft III and play without stretches. However, the system may not have the ideal resolution.

For example my system only offers 1280x960, and this leads to: 12,5% hardware strech and blur or 60px black stripes at the top and the bottom of the screen.

Well you think now: "Hey, why not set proper resolution via regedit?". Problem with this way here - Warcraft III will reduce your resolution to the closest system resolution. I think it's protection against errors.

So you need to add new resolution to the system.

I can tell you that it is very easy with Nvidia cards (don't know about Radeon cards).

Instruction

Yep, it's in Russian, so all names translated

and can be different in reality

1. Open Nvidia taskbar menu with right click on the Nvidia icon.

2. Open Nvidia control panel with corresponding button.

3. Now open tab "Monitor->Change Resolution".

4. Choose your display at the top.

5. Click "Configure" at the bottom.

6. Check "Allow non-standard modes".

7. Click "Create user resolution".

8. Input proper settings for your monitor, i.e. "Horizontal pixels: 1440, Vertical pixels: 1080, Refresh rate: 60, Color depth: 32, Scan type: Progressive, Sync: Auto".

9. Press "Test" -> "YES" -> "OK".

10. Check that your new resolution is now in resolutions list.

11. Close Nvidia control panel and run Warcraft III.

12. Select new resolution in options and enjoy

Result

##
 What are the advantages?

No blurring and stretching.

Black stripes do not distract from the game.

Cursor can not leave game window, so you can easily scroll view with mouse at the edges.

Fullscreen applications work faster.

You just run the game and everything is already set, without additional programs and other.

##
 Bonus

This solution works with all games.

Now you can set new resolution in old games if they fill resolutions list from system settings.

If game doesn't fill resolution settings from OS, you can configure it with ".ini"/".cfg" files or registry entries (depends on the game).

#### Captivate

#### Captivate

 Level 5

JoinedAug 8, 2012Messages153

Thanks! Btw which game are you playing in this shot?


 Last edited by a moderator: Mar 8, 2015

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

Gaias retaliation as far as I can tell

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

So that's why my laptop's resolution does not affect warcraft

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great job in detailing the steps. I've seen some tutorials similar to this topic, but none of them ever go into correcting the aspect ratio.

I can't test it, but your screenshots are proof enough that it does its job. Approved.

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

I added a custom 1440x1080 resolution and applied it but the game still runs at 1400x1050.

My notebook uses Optimus for Nvidia so there's no custom resolution option in the Nvidia control panel, and I added the resolution through Intel graphics options instead.

#### Onibushi

#### Onibushi

 Level 5

JoinedJan 25, 2012Messages109

Sorry to necropost this but how can I fix this in Radeon card? I just hate the widescreen stretching and for some reason the game is running in widescreen now, I think the radeon thing was updated.

#### Wlizer

#### Wlizer

 Level 9

JoinedJan 1, 2014Messages143

Resources: 4

#### Resources
Maps 4

I have issues with aspect being greyed out in my asus monitor settings.. sucks really =( i can play some games with 4:3 ratio like condition zero.. i wish wc3 could be also set the same but it will automaticially scretch

#### Ravager16829

#### Ravager16829

 Level 40

JoinedMay 14, 2021Messages2,455

Wlizer said:

 I have issues with aspect being greyed out in my asus monitor settings.. sucks really =( i can play some games with 4:3 ratio like condition zero.. i wish wc3 could be also set the same but it will automaticially scretch


Click to expand...

What version are you playing on?

This thread is criminally obsolete nowadays due to the fact the game now supports the Widescreen Resolution (without stretching), so it's meaningless to use this trick when the Reforged came out.

#### Wlizer

#### Wlizer

 Level 9

JoinedJan 1, 2014Messages143

Resources: 4

#### Resources
Maps 4

Ravager16829 said:

 What version are you playing on?

This thread is criminally obsolete nowadays due to the fact the game now supports the Widescreen Resolution (without stretching), so it's meaningless to use this trick when the Reforged came out.


Click to expand...

i play randomily 1.26a version. i use window mode and scretch it but its slower for some reason..

however i think reforged has different camera distance and view angle than 1.26a, its not exactly what a nostalgia wc3 purist would look for

 You must log in or register to reply here.


### Similar threads

Locked

Poll

 Your aspect ratio 4:3 or 16:9 ?


ALEXz407

Jan 29, 2015

Gaias Retaliation ORPG

Replies8Views9K

Jan 31, 2015
kevs926

D

Locked

 Warcraft 3 has support for 1440p


Deleted member 219079

May 11, 2014

Warcraft Discussion

Replies9Views12K

May 27, 2014
Razosh

Locked

 Adding black bars to the Sides when using a Widescreen Monitor?


qbrtux

May 28, 2009

Warcraft Discussion

Replies0Views5K

May 28, 2009
qbrtux

 Anti-aliasing in HD mode AKA making this game look ACTUALLY better


Spacebuns

Sep 24, 2021

Misc/General Mapping Tutorials

Replies1Views3K

Oct 26, 2021
Chaosy

Locked

 v1.30 stretched high res loading screen?


sur5al

May 14, 2019

World Editor Help Zone

Replies1Views1K

May 15, 2019
Dr Super Good

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