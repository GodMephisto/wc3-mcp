---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/a-simple-way-to-greatly-reduce-object-editor-lag.169536/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# A Simple Way to Greatly Reduce Object Editor Lag
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/a-simple-way-to-greatly-reduce-object-editor-lag.169536/>
#### Caedrus

#### Caedrus

 Level 11

JoinedFeb 11, 2010Messages199

Resources: 3

#### Resources
Maps 1Spells and Systems 1Tutorials 1

#
 How to greatly reduce Object Editor lag!

###
 The Problem

So I've heard from a number of people that they've been having a problem with the World Editor: Quite simply, when you try to do things like "delete a custom unit from the object editor," it takes an unreasonably long amount of time, even for people with stellar computers. Well, the good news is, it's not your computer's fault! With one simple action, you can remove this lag.

###
 The Cause

The issue is, almost certainly, the Brush List. What is the Brush List, you ask? Well it's that little thing under the model previewer that shows all your object editor data. Most people don't even touch this thing, but it causes a significant amount of lag when editing units in the object editor, because the World Editor has to update the brush list.

###
 The Solution

The solution is simple, and takes merely seconds. Simply go into your world editor, select the  Window dropdown, and see where Brush List is checked. Uncheck it. You're done! Easy, huh? Now you can create or delete 60 new units in the object editor without delay!


^---This is what you're looking for.

###
 More Speed

If the world editor still runs slow, you might want to make sure your computer meets the recommended system requirements. Still, for those of you who use the editor on a slow laptop or something, you can increase speed further (at the expense of multitasking effectively with other programs) if you're using a newer version of Windows (XP, Vista, etc) by setting a higher priority in the task manager. To do this, first make sure your World Editor is on and running. Next, go to the task manager (Just hit CTRL+ALT+DEL). Now, either click on the Processes tab and scroll down until you see "worldedit.exe" OR you go to the applications tab, right click on World Editor, and select "Go to process" from the dropdown menu. Now, right click on the process, and select a higher priority from the dropdown menu.


^---this is what you're looking for.


Be careful when you're doing this, however, because changing the priority can potentially unbalance your system. At the very least, increasing the priority of a program will hog resources for that program, and slow down any program that isn't that program until you return priorities to normal. I recommend increasing the priority gradually (one step at a time) until you find the point that works best for you.


Anyways, that's it. If you like this, please comment, etc.


All the best,


-Caedrus

#### Attachments

WorldEditorLagCrop.jpg

 58.8 KB

 · Views: 17,563



Priority.jpg

 73.2 KB

 · Views: 17,621




 Last edited by a moderator: Jan 29, 2011

#### Numerion

#### Numerion

 Level 13

JoinedJul 3, 2008Messages1,098

Resources: 4

#### Resources
Icons 1Maps 3

^ old news are old

#### Caedrus

#### Caedrus

 Level 11

JoinedFeb 11, 2010Messages199

Resources: 3

#### Resources
Maps 1Spells and Systems 1Tutorials 1

Numerion said:

 ^ old news are old


Click to expand...

Maybe so, but I didn't see a tutorial about it here already, and a lot of people here don't know it, in my experience. And for those who don't know it, "new news are new." It's also a really important thing to know, seeing as creating a bunch of units takes forever if you have the brush list selected.

#### Joe-black-5

#### Joe-black-5

 Level 25

JoinedApr 27, 2008Messages2,512

Resources: 7

#### Resources
Icons 6Maps 1

First time I know about these things

it will really help me in my work

it might be old news for some but it's the first time I hear about something like this.

Thanks for the tutorial

+rep

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice tutorial. There are still a lot of people who ask how to make the object editor work faster. This tutorial pretty much sums it up. I'll approve it once I attach the images to THW.

EDIT: Done. ~Approved.

#### Mr.Pockets00000

#### Mr.Pockets00000

 Level 9

JoinedJan 1, 2010Messages197

Resources: 15

#### Resources
Skins 2Maps 10Spells and Systems 1Tutorials 2

dude freaking Sweet. It used to take forever to create custom units and change thier names. Thanks dude


Also just increasing it to above normal seems to be enough... and hopefully wont cause instability

#### Caedrus

#### Caedrus

 Level 11

JoinedFeb 11, 2010Messages199

Resources: 3

#### Resources
Maps 1Spells and Systems 1Tutorials 1


 Joe-Black-5 said:



 First time I know about these things

it will really help me in my work

it might be old news for some but it's the first time I hear about something like this.

Thanks for the tutorial

+rep


Click to expand...

Mr.Pockets00000 said:

 dude freaking Sweet. It used to take forever to create custom units and change thier names. Thanks dude

Click to expand...

 You are welcome

 Also just increasing it to above normal seems to be enough... and hopefully wont cause instability


Click to expand...

 Of course, this is entirely dependent on your own personal rig, so I can't really say much here. You have to figure out which settings are best for your system.

#### Alfagar

#### Alfagar

 Level 16

JoinedMay 23, 2009Messages1,351

Resources: 11

#### Resources
Models 5Icons 5StarCraft II Resources 1

:O Great tutorial

#### adonisis

#### adonisis

 Level 3

JoinedMay 31, 2010Messages35

Helpfull, thanx!  but can someone tell me how to use imported models that requires image/textures? :| i know im still newb ^^

#### Bugbuster96

#### Bugbuster96

 Level 7

JoinedAug 11, 2009Messages163

Resources: 5

#### Resources
Maps 2Spells 1Templates 1Tutorials 1

yea, this rox, it makes sense, and you're right, i didn't even touch that ugly-lagly thing

+rep

@adonisis:

I'll PM you.

#### XAlexX

#### XAlexX

 Level 2

JoinedJun 16, 2010Messages22

Really Great Tut. and very usefull.


I'm working on a map now with tons of custom units and that Object edit. lag pissed me off many times.


Thx for help...now my Object editor runs like Speedy Gonzales.....seriously


rep+

#### sanityslayer42

#### sanityslayer42

 Level 2

JoinedNov 5, 2009Messages15

great tip.

#### Superz_Ze_Man

#### Superz_Ze_Man

 Level 8

JoinedAug 21, 2009Messages408

Resources: 1

#### Resources
Maps 1

omg man, i cant thank you enough.. I never new about this... I have a great computer and i couldn't figure out why it lagged.. Thanks to you... my work is 50 times faster

#### tommynator

#### tommynator

 Level 5

JoinedMar 1, 2010Messages86

Resources: 1

#### Resources
Maps 1

thanks. i have a new computer and it was better than my last one and the old one did not lagg, but this one did.


this helped a lot!!!

#### Allen Walker 22

#### Allen Walker 22

 Level 4

JoinedSep 23, 2010Messages48

Helps me tnx!

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

wooooooooooow !!


thnx man that helped a lot !!


i copyed and pasted 50 units and it only took a second xD

#### iBear

#### iBear

 Level 2

JoinedAug 29, 2010Messages23

Awesome! Maybe old news to lots of modders, but newbies like me are thrilled to learn that we can disable that infuriating lag time so easily!

#### Eldubs

#### Eldubs

 Level 15

JoinedSep 17, 2010Messages1,449

Resources: 13

#### Resources
Packs 1Maps 12

Great help, +rep!

#### lht001300000

L

#### lht001300000

Although it seems an old news, but for me, it could be helpful as I've been taken forever to figure out "What is the editor doing in those 3 seconds of laging?". Old news are new for me

#### SetaSouji

#### SetaSouji

 Level 3

JoinedMar 10, 2010Messages23

Resources: 3

#### Resources
Models 2Maps 1

thanks

#### cxjiek

#### cxjiek

 Level 2

JoinedDec 15, 2008Messages18

thank you, this is such important new news for me. What other important things are there? I gotta know.

#### solomon409

#### solomon409

 Level 10

JoinedApr 22, 2010Messages421

Resources: 12

#### Resources
Icons 4Skins 2Maps 6

i got a question, wat happens when it is unstabalized?, would my comp crash?

#### Zulabar

Z

#### Zulabar

BAM! Now I finally can change Unit's icons and names quickly.


Thanks a lot.

#### Darkwing

#### Darkwing

 Level 6

JoinedMay 15, 2008Messages147

Resources: 2

#### Resources
Maps 1Tutorials 1

Nice one!!!

#### SexYFrostWyrM

#### SexYFrostWyrM

 Level 8

JoinedJan 18, 2010Messages131

Resources: 15

#### Resources
Icons 15

OMFG ! Why... why didn't have read this tutorial many years ago ? I was pissed off by this horrible slow editor, and i just had to remove this useless bar to make the things go much faster !


Thx a lot for this, you changed my life !

#### Razor_Edge

#### Razor_Edge

 Level 2

JoinedDec 27, 2010Messages15

cheerz man, really useful. didnt no about that  +rep

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Your second tip of raised task piority only applies really for single core systems or systems which are running demanding processes on top of WE. In multi core systems it will prety much give it full use of an entire processor when it demands something.

#### RaijinSpark

#### RaijinSpark

 Level 19

JoinedAug 31, 2008Messages1,774

Resources: 24

#### Resources
Models 1Icons 19Tools 1Maps 3

Even if this is a really old tutorial, still very helpful to me  +REP :3

#### Hashjie

#### Hashjie

 Level 14

JoinedApr 20, 2009Messages1,543

Dr Super Good said:

 Your second tip of raised task piority only applies really for single core systems or systems which are running demanding processes on top of WE. In multi core systems it will prety much give it full use of an entire processor when it demands something.


Click to expand...

so your saying using a quad core and putting the priority of world editor higher wouldn't make it go faster ?

lol thats why I see no difference in speed, thnx dude !

(not that it's quite noticable with 8gb ddr3 memory xD)

even after changing the priority and removing the bar I still find the world editor too slow. (Especially when changing your camera with control + right mouse button) :S

even though my specs are quiet high for a program like world editor >.<

but I guess I should be more pacience. I always want everything to go really fast...

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

That thing!!! It has Consumed Entire hours of my life!!!! (In total)


I only wish I knew this b4 moveing all the Object editor data from an entire map to The Campaign Editor.


That took like 1 hour and a half, when it would take me much less now.


Thanks. +Rep

#### Valkyrie Cain

#### Valkyrie Cain

 Level 2

JoinedDec 3, 2010Messages25

Cool thanks I hated how the editor took an age just to change a hero's name>.>

#### Renschi

#### Renschi

 Level 4

JoinedApr 9, 2011Messages84

Thank you man! Great tutorial.

#### uYkam

#### uYkam

 Level 3

JoinedMay 24, 2011Messages57

Resources: 1

#### Resources
Maps 1

Such a useful tip.... thnx

#### GenoSabre

G

#### GenoSabre

Caedrus said:

 Maybe so, but I didn't see a tutorial about it here already, and a lot of people here don't know it, in my experience. And for those who don't know it, "new news are new." It's also a really important thing to know, seeing as creating a bunch of units takes forever if you have the brush list selected.


Click to expand...


I second.


It's new news for me too. I was awed.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I'm going to sticky this thread. Hopefully it will make more users see it, because this is pretty much need-to-know information for any editor.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Just post a sticky in the WEHZ. Name it something like "How to make WorldEdit run much faster on every system".


Hints like this need to get passive reads. Yes people who are complaining the editor is slow will find it but its different getting people who do not realise the editor could run faster to find it.

#### tmntbbq

T

#### tmntbbq

Thank you for the advice, it really works

#### Bugz

#### Bugz

 Level 16

JoinedApr 18, 2011Messages1,844

Resources: 7

#### Resources
Maps 4StarCraft II Resources 1Tutorials 2

Indeed usefull

#### Crucify_

#### Crucify_

 Level 2

JoinedJan 10, 2010Messages17

Thanks a lot! I've been making maps for a couple of years but could never figure out why it ran so slow there, but this explains a lot so again, thanks.

#### woodknight

#### woodknight

 Level 3

JoinedAug 16, 2011Messages26

Resources: 1

#### Resources
Spells and Systems 1

Very helpfull and informative. Saved me a lot of time and wasted nerves. Thx for a great tutorial.

#### TerraNexx

#### TerraNexx

 Level 6

JoinedDec 9, 2008Messages233

damn! really saved my life with this


thank you man

#### Dhaniels

D

#### Dhaniels

this helped more then alot, it took like 2 mins to process a thing now it's way faster, thanks for the guide

#### Donach

#### Donach

 Level 6

JoinedJan 12, 2011Messages110

Resources: 2

#### Resources
Maps 1Tutorials 1

Oh DAMN!... Thanks a lot. Too bad I've already created about 100 units without knowing this :-D

#### SYLAR18

#### SYLAR18

 Level 4

JoinedMay 6, 2012Messages69

Resources: 1

#### Resources
Maps 1

thanks i didnt knew it and i was waiting like 2-3 min every time i was changing anything

#### Daeku346

#### Daeku346

 Level 3

JoinedMay 4, 2011Messages35

Very great tip. I'm always pissed off why Object Editor is sloppy always

#### Tigrick

#### Tigrick

 Level 10

JoinedApr 27, 2012Messages240

Resources: 4

#### Resources
Maps 4

soooo helpfull! Without the cruel waiting time object editor lag causes it is actually possible to have a real life besides mapping

#### MrThomas369

#### MrThomas369

 Level 2

JoinedMay 25, 2012Messages13

It works! Thanks!


+rep

#### Unhit

#### Unhit

 Level 6

JoinedNov 4, 2004Messages125

Resources: 2

#### Resources
Maps 2

Wow, so easy. If only I had known earlier. Thanks very much

#### Codric

#### Codric

 Level 11

JoinedApr 7, 2012Messages742

Resources: 8

#### Resources
Models 3Icons 2Skins 1Maps 2

Thanks you helped allot!

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

The process priority will not help at all. Worse is if you set it very high and do something intensive in the editor it could lock single core computers until the action complets.

 You must log in or register to reply here.


### Similar threads

Locked

 Reducing World Editor Lag (Trigger Editor GUI)


CAAentertainment

May 26, 2020

World Editor Help Zone

Replies6Views4K

Jun 17, 2020
Chaosy

 How to reduce loading times of Object Editor data


Xonok

May 15, 2013

Misc/General Mapping Tutorials

Replies33Views14K

Oct 1, 2023
Luashine

 Importing/Exporting Object Data from WE


RedBlade

May 24, 2007

Misc/General Mapping Tutorials

Replies17Views21K

Aug 7, 2021
ThePerfectIdiot

 World Editor Tips


Ralle

Jan 3, 2006

Misc/General Mapping Tutorials

Replies28Views57K

Sep 20, 2025
your568473

Locked

 Reducing Loading Object Editor


Vunjo

Mar 31, 2013

World Editor Help Zone

Replies4Views1K

Apr 2, 2013
Ragon.Slear

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