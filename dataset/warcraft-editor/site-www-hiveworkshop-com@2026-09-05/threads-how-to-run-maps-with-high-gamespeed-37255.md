---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-run-maps-with-high-gamespeed.37255/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Run Maps with High Gamespeed
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-run-maps-with-high-gamespeed.37255/>
#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

This was a neat trick i found out today. I've always been annoyed that blizzard gave the AI editor a "Speed" parameter, but didn't add it to the regular editor. Usually you can only test "Melee" maps with high speed, but now i found a way to do it with triggered maps too.

Open the map you want to test with high speed.

Note down the following:
Which slot you want to control when testing.

Which slot you have a computer in.

Open the AI Editor.

Find a map with 12 players slots.

Set the slot you want to control to user

Set the computer slot you noted, to a computer, and leave the AI.

Hit test AI.

Close the map right away, no need to watch it.

Open your warcraft III directory.

Open the folder "TestGame"

Open the file "WorldEditTestGame.wgc" with Notepad.

Change the file name of the map you chose earlier to the one you want to test.

Once you've done that, create a shortcut to war3.exe, and put "-loadfile TestGame\WorldEditTestGame.wgc" without the quotes, as parameter.

Run the shortcut, and the game should load the custom map, but with the speed setting you specified in the AI Editor.

Enjoy.


I've done very few tests on this, so configuring it is limited. Only way i can see to change speed, is to change it in the AI editor, doesn't seem like it's linked to the wgc file. If you do some tests of your own, please write the results, and i'll update the guide.


WIP Tutorial, i know it's a little vague, i'll see to add some pictures some time soon.


 Last edited by a moderator: Sep 19, 2007

#### Elenai

#### Elenai

 Level 35

JoinedMay 22, 2007Messages5,364

Resources: 36

#### Resources
Models 14Icons 22

interesting

#### UchihaSasuke.gr

#### UchihaSasuke.gr

 Level 20

JoinedMar 11, 2007Messages2,822

Resources: 6

#### Resources
Models 1Icons 4Tutorials 1

Hmm...That's pretty good.

#### Chaos Overlord

#### Chaos Overlord

 Level 12

JoinedFeb 1, 2007Messages1,092

Resources: 1

#### Resources
Maps 1

Lol cool... I think. So this will make the map run faster?


Off Topic: Uchiha, put an Enter after our picture so the text is centered on your sig too.

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved!

~ Thread moved to Miscellaneous Tutorials.

#### shadow-twin

#### shadow-twin

 Level 2

JoinedSep 18, 2007Messages21

nice lol

#### global

G

#### global

This is totally unuseful. Normal speed is ok, no need to speed up game.

#### UchihaSasuke.gr

#### UchihaSasuke.gr

 Level 20

JoinedMar 11, 2007Messages2,822

Resources: 6

#### Resources
Models 1Icons 4Tutorials 1

global said:

 This is totally unuseful. Normal speed is ok, no need to speed up game.


Click to expand...


This thread is like 2 months old,and you are back posting to say it's not useful?Omg what a tard.

#### AzadHuffy

A

#### AzadHuffy

It doesn't work !

#### AzadHuffy

A

#### AzadHuffy

It doesn't wrok !


I tried again, but it really doesn't work !

#### UchihaSasuke.gr

#### UchihaSasuke.gr

 Level 20

JoinedMar 11, 2007Messages2,822

Resources: 6

#### Resources
Models 1Icons 4Tutorials 1

And you have to double post in the time of 2 minutes?

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

It works for me, but I don't see why I would ever need it... :\

#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

The idea behind the highspeed testing is for AI balancing/testing, usually it's hard to make an AI start at a certain point, and just act from there, without having to do extensive triggering. I've used highspeed to test an AoS, where the forces actually had various abilities, and they were perfectly balanced. That's alot of work if you have to balance normally. The difference was big, i usually had to test for atleast 20-30 minutes after each change, that was cut down to 4-6 minutes. Same with Hero AI's, i made a couple of heroes that battled eachother as a test, and the way of testing highspeed was just awesome.

#### AutoRefresh

A

#### AutoRefresh

Gj dude

#### iSky

#### iSky

 Level 5

JoinedNov 28, 2008Messages124

Nice TuT =) Thanks!!

#### RaijinSpark

#### RaijinSpark

 Level 19

JoinedAug 31, 2008Messages1,774

Resources: 24

#### Resources
Models 1Icons 19Tools 1Maps 3

doesn't work to me

#### Cokemonkey11

#### Cokemonkey11

Spell Reviewer

 Level 30

JoinedMay 9, 2006Messages3,572

Resources: 25

#### Resources
Tools 4Maps 8Spells and Systems 3JASS 7Tutorials 2Wurst 1

Nice trick, someone should make a tool which mimics the process automatically, maybe a new JNGP extension

#### yram44

#### yram44

 Level 3

JoinedMar 26, 2011Messages44

This doesn't work on triggered maps and even though I'll tried it in vanilla world editor. And also this doesn't work in JNGP

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

This method works on all maps, but it was WorldEditor which stopped you from using it on all maps.

I've made a tool exactly for this here: WGC Utility (Test maps at high game speed)

 You must log in or register to reply here.


### Similar threads

D

Locked

 How to run maps with high gamespeed #2


dragoljub

Feb 3, 2010

Triggers & Scripts

Replies0Views2K

Feb 3, 2010
dragoljub

D

D

Locked

[AI]
 Easy AI Tester Utility


dragoljub

Feb 7, 2010

Triggers & Scripts

Replies0Views2K

Feb 7, 2010
dragoljub

D

Locked

 Gamespeed


Cobrajet127

Aug 6, 2011

StarCraft II Modding

Replies4Views2K

Aug 11, 2011
Cobrajet127

Locked

 Serious gameplay lag on a very tiny map.


ZoMbIe828

Nov 11, 2004

Map Development

Replies8Views2K

Nov 12, 2004
Lonely Wolf.

Locked

[Solved]
 How to make player slots non changeable


Saif

Jun 5, 2011

World Editor Help Zone

Replies2Views1K

Jun 5, 2011
Saif

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