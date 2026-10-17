---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/multiple-warcraft-3-running-on-a-single-pc.226287/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Multiple Warcraft 3 Running on a Single Pc
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/multiple-warcraft-3-running-on-a-single-pc.226287/>
#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

#
 Running Multiple Game Instances

Windows

##
 What is it?

kLoader is a program that allows you to run multiple instances of Warcraft 3 on the same computer at the same time. This is invaluable to mapmakers testing multiplayer aspects of their maps without having additional PCs or beta testers available.

Note: This does not necessarily work on the latest patch 1.27a! For patch 1.27a, take a look at this:

Universal kLoader - Raichu

##
 Required Programs

kLoader

If you'd like to find an alternative download, search for "war3 multiple loader".

kLoader is the program that allows you to run multiple instances of the game, so that is the only required software (besides the game of course).

Optional:Warcraft 3 Mouse Capture

This nifty little program keeps your mouse inside Warcraft 3 if you are using windowed mode.

##
 Installation

Extract the kLoader .zip file into your Warcraft 3 directory. By default, this is:

 Code:

C:\Program Files\Warcraft III

Now find the config.txt file. (\Warcraft III\config.txt)

Open it and change the specified path to your war3.exe's path.

You'll notice it has "-window" at the end; this launches the game in a window. You can remove it.

Extract the mouse capture .rar into your Warcraft 3 directory.

##
 Usage

To run the game in this mode, find W3MultipleLoader in your Warcraft 3 directory. You might want to make a shortcut on your desktop.

It will launch a new instance of the game with no further action required. For another instance, simply launch it again.

Keep in mind that each of these will take up a chunk of your computer's memory, so only run as many as your machine can support.

To launch Mouse Capture, find the folder you extracted it to and run it. It will stay in your taskbar until you close it or restart your PC.

(only two items are visible for one player because I was testing locally hiding items)

##
 Alternatives

Alternatively, you can use Jass NewGen Pack 2.0 Alpha to run multiple instances of Warcraft III. kLoader is built-in.

For more information, see the "Multiplayer Emulation" portion of the following thread:

https://www.hiveworkshop.com/forums...gen-pack-2-0-alpha-recruiting-testing-233145/

This will automate the process described above. Note that it is still in alpha, and thus it may not be fully bug-free. The multiplayer emulation feature, however, has been tested and works.

#### Attachments

config.png

 17.6 KB

 · Views: 19,922



same game bro.jpg

 213.8 KB

 · Views: 20,478



create and join own game.jpg

 167.5 KB

 · Views: 19,443



kLoader.zip

 88.5 KB

 · Views: 13,969




 Last edited by a moderator: May 19, 2016

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

Sometimes my world editor runs twice, but I never thought running Warcraft III more than one time. I will test this.

Edit: Ok, Windows 7 64 bits can't handle it. It seems I'll keep using my netbook. :/


 Last edited: Nov 29, 2012

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I'm curious as to how this works with things like LAN and battle.net. You don't have to test the latter, but is it possible to join your own LAN game? I'm just curious because that might be an interesting method to test maps in multiplayer. (for things like desyncs)

As for the tutorial, it seems fine to me. Good job. Anyone able to test it though? Or provide a screenshot that it works?

Sorry, I can't test myself since I am running a mac and can't run kLoader.

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

Indeed you can!

I've updated the OP with a screenshot of the program in action.

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

Anvil said:

 Sometimes my world editor runs twice, but I never thought running Warcraft III more than one time. I will test this.


Edit: Ok, Windows 7 64 bits can't handle it. It seems I'll keep using my netbook. :/


Click to expand...


I'm on Windows Ultimate 64bit.


Have you tried forcing administrator mode?


What type of error message are you getting?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

useful really

#### Luorax

#### Luorax

 Level 16

JoinedAug 7, 2009Messages1,406

Resources: 2

#### Resources
Maps 1Tutorials 1

PurgeandFire111 said:

 I'm curious as to how this works with things like LAN and battle.net.


Click to expand...

It does work on LAN, I've been using kLoader for quite a while to test things like vision over something for different players. You can host a game with one instance and join it with another one. It's awesome like that.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Awesome! Approved.

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

That was quick

#### NagaRulez

#### NagaRulez

 Level 7

JoinedNov 11, 2011Messages219

Resources: 1

#### Resources
Icons 1

When I click LAN, it quickly gone... Help?!

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

NagaRulez said:

 When I click LAN, it quickly gone... Help?!


Click to expand...


Explain?

#### bhusta

#### bhusta

 Level 13

JoinedMar 6, 2008Messages843

Resources: 4

#### Resources
Icons 1Maps 2Tutorials 1

Now the forever alone guy can have a lan party

It's realy helpful for testing systems or other stuff.

#### BUP

#### BUP

 Level 10

JoinedSep 9, 2012Messages172

Resources: 15

#### Resources
Maps 2Spells and Systems 13

Really good man!! Now I can test player settings

#### NagaRulez

#### NagaRulez

 Level 7

JoinedNov 11, 2011Messages219

Resources: 1

#### Resources
Icons 1

rulerofiron99 said:

 Explain?


Click to expand...


When I click the W3MultipleLoader.exe, a window of warcraft pop up and then when I'am ready to test my map and click Local Area Network, in 0.01 it disappears...

#### HesitationSnow

#### HesitationSnow

 Level 6

JoinedNov 24, 2012Messages218

Resources: 1

#### Resources
Spells 1

Works great here, thanks.

I can't multi-log on b.net on different realms for some reason.

Oh well, good for testing.

Guess I can't plant a feeder/spy on other team yet =)

#### BUP

#### BUP

 Level 10

JoinedSep 9, 2012Messages172

Resources: 15

#### Resources
Maps 2Spells and Systems 13

It works properly for me.


I'm on Windows 8 Pro.No fatal errors

#### CoLd Bon3

#### CoLd Bon3

 Level 14

JoinedAug 8, 2010Messages1,022

Resources: 6

#### Resources
Maps 2Spells and Systems 4

Works for Win 7 too.

@ruler, dude, i don't know how to thank you, man! I was wondering how i can test a shit doze of things in my map. Now i can with no problem at any time.

So... i will simply..

GIVE YOU +REP EVERY DAY UNTIL I FUCKING DIE!

after i spread some, though...

EDIT : Does it work exactly like another player? I want to test for desyncs and i don't know if things happen right. I mean, same PC, same IP and stuff. I ask this because i encountered a desync. But the weird thing about it is that it appears in random times. I mean, before the desync, i do some stuff related only to one player. But then i get disconnected after 4-5 seconds (where i don't do anything). Is this normal? I mean, is the problem in my map or in the fact that the PC is the same for both instances of the game?


 Last edited: Jan 2, 2013

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

In my experience, the desyncs are only ever caused by things in the map causing desyncs.

To be sure, you can test it thoroughly using a melee map and see if it ever DCs.

#### sonofjay

#### sonofjay

 Level 28

JoinedOct 28, 2011Messages4,750

Resources: 27

#### Resources
Packs 1Maps 20Templates 5Tutorials 1

Anvil said:

 Sometimes my world editor runs twice


Click to expand...


How in the world you were able to do that? If you can somehow tell us, it would be a amazing help for everyone.


Anyway, this is a nice application, might try it if all my friends die and I have no one to test my maps.

#### CoLd Bon3

#### CoLd Bon3

 Level 14

JoinedAug 8, 2010Messages1,022

Resources: 6

#### Resources
Maps 2Spells and Systems 4

sonofjay said:

 If you can somehow tell us, it would be a amazing help for everyone.


Click to expand...

With what this can help you? You can only change something in the first editor then something in the second and get mindfucked because you will not know which thing where was changed...

#### sonofjay

#### sonofjay

 Level 28

JoinedOct 28, 2011Messages4,750

Resources: 27

#### Resources
Packs 1Maps 20Templates 5Tutorials 1

Well it can be useful like this:

Import a very long system but its what you really need, but in GUI some data get fuck if you transfer them, if you can have two editors, then you can check the code from the original map and your map to see what is broken on the code you copied. Some are really just hard to find >.<

#### CoLd Bon3

#### CoLd Bon3

 Level 14

JoinedAug 8, 2010Messages1,022

Resources: 6

#### Resources
Maps 2Spells and Systems 4

sonofjay said:

 Well it can be useful like this:


Import a very long system but its what you really need, but in GUI some data get fuck if you transfer them, if you can have two editors, then you can check the code from the original map and your map to see what is broken on the code you copied. Some are really just hard to find >.<


Click to expand...

You can do this with one editor too!


When you open your map, you see (on the very top left menu, the tools, from where you can open,save,close file menu) a Window drop-menu (at the top center of the editor, or press Alt + W). At the bottom of this drop menu, you will see "1 [ProjectName]". Then, while your map is open, open the system's map. Your map will hide and the other one will appear. If you click this Window drop-menu again, you will see :


1 [ProjectName]


2 [SystemMapName]


The second one will be selected. Click one the first one (your map).


And you are ready! You can cycle between the two maps with ease.

#### Ironside

#### Ironside

 Level 22

JoinedFeb 3, 2009Messages3,291

Resources: 7

#### Resources
Maps 3Spells 3Tutorials 1

CoLd Bon3 said:

 You can do this with one editor too!

When you open your map, you see (on the very top left menu, the tools, from where you can open,save,close file menu) a Window drop-menu (at the top center of the editor, or press Alt + W). At the bottom of this drop menu, you will see "1 [ProjectName]". Then, while your map is open, open the system's map. Your map will hide and the other one will appear. If you click this Window drop-menu again, you will see :

1 [ProjectName]

2 [SystemMapName]

The second one will be selected. Click one the first one (your map).

And you are ready! You can cycle between the two maps with ease.

Click to expand...

Still takes too long between each shift on weaker computers, 2 world editors would enable extremely fast copying.

This program would be even better if it could somehow start a different installation of Warcraft 3 for the second instance, that way one could install wc3 2 times with a different set of keys and be able to play on bnet with 2 accounts.

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

Ironside said:

 Still takes too long between each shift on weaker computers, 2 world editors would enable extremely fast copying.


This program would be even better if it could somehow start a different installation of Warcraft 3 for the second instance, that way one could install wc3 2 times with a different set of keys and be able to play on bnet with 2 accounts.


Click to expand...


You could probably do that with a virtual machine.

#### Ironside

#### Ironside

 Level 22

JoinedFeb 3, 2009Messages3,291

Resources: 7

#### Resources
Maps 3Spells 3Tutorials 1

The computer needed to run a virtual machine that can run a game in it, well, mine can't.

Anyway, if the source for the kLoader was available / if the creator would see this, then it would be a very easy thing to achieve.

#### sonofjay

#### sonofjay

 Level 28

JoinedOct 28, 2011Messages4,750

Resources: 27

#### Resources
Packs 1Maps 20Templates 5Tutorials 1

 You can do this with one editor too!

When you open your map, you see (on the very top left menu, the tools, from where you can open,save,close file menu) a Window drop-menu (at the top center of the editor, or press Alt + W). At the bottom of this drop menu, you will see "1 [ProjectName]". Then, while your map is open, open the system's map. Your map will hide and the other one will appear. If you click this Window drop-menu again, you will see :

1 [ProjectName]

2 [SystemMapName]

The second one will be selected. Click one the first one (your map).

And you are ready! You can cycle between the two maps with ease.


Click to expand...

I obviously know that for a very long time >.>

I'm talking about reading the triggers at the same time since its easier to check that way. I mostly need to go here then CnP the triggers of the system to a post, then take a look on my world editor to read the other one. But sometimes I don't have internet connection to do that.

#### SA Dashie

#### SA Dashie

 Level 22

JoinedApr 3, 2010Messages1,889

Resources: 17

#### Resources
Maps 11Spells and Systems 3Templates 1User Interface 2

'kLoader v2 RC1'

This download seems to be invalid for me. :\

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I've contacted ruler. Hopefully he can upload it to this site or post another link. Thanks for reporting the link.

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

DeathChef said:

 'kLoader v2 RC1'


This download seems to be invalid for me. :\


Click to expand...


I've attached it to the first post, download should work as long as the hive works

#### neo_sluf

#### neo_sluf

 Level 18

JoinedFeb 5, 2012Messages1,685

Resources: 10

#### Resources
Icons 1Maps 9

It says


Warcraft III Unable to Initialize

#### DivineLight

#### DivineLight

 Level 13

JoinedJan 30, 2012Messages1,297

Resources: 5

#### Resources
Icons 1Maps 2Spells and Systems 2

hey, i've got some prob here..


when i open the first wc3 & create map


the second wc3 i select lan then there is no map from the first one..


help please


anyway nice app, useful really but can't play it

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It may be an issue of your connection/permissions. Are you able to use LAN regularly?

Try disabling your Windows firewall before attempting to run LAN. It may be blocking the connection.

I've updated the tutorial at the end, listing the alternative of using JNGP 2.0. kLoader is now built-in.

#### MajorKaza

#### MajorKaza

 Level 10

JoinedNov 19, 2011Messages516

Resources: 8

#### Resources
Icons 2Maps 5Spells and Systems 1

Not work for Win 7 64-bit. Same message like normal twice run WC3.

#### Ironside

#### Ironside

 Level 22

JoinedFeb 3, 2009Messages3,291

Resources: 7

#### Resources
Maps 3Spells 3Tutorials 1

MajorKaza said:

 Not work for Win 7 64-bit. Same message like normal twice run WC3.


Click to expand...

Works for me, Windows 7 Ultimate x64.

#### MajorKaza

#### MajorKaza

 Level 10

JoinedNov 19, 2011Messages516

Resources: 8

#### Resources
Icons 2Maps 5Spells and Systems 1

So may be that I have too weak PC... :'(

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

 It will launch a new instance of the game with no further action required. For another instance, simply launch it again.


Click to expand...

I had this error when I want a new instance...

#### Attachments

Clipboard01.jpg

 67 KB

 · Views: 2,355



#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

mckill2009 said:

 I had this error when I want a new instance...

Click to expand...


That's war3's usual non-multi error.


Make sure your extracted everything correctly and that you're running the multi exe for all instances. Make sure you aren't already running a normal instance (not launched with the multi exe) of the game.

#### DivineLight

#### DivineLight

 Level 13

JoinedJan 30, 2012Messages1,297

Resources: 5

#### Resources
Icons 1Maps 2Spells and Systems 2

is it JNGP 2.0. same as kLoader?

i mean the function

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

Dunno, haven't tried JNGP yet.

#### DivineLight

#### DivineLight

 Level 13

JoinedJan 30, 2012Messages1,297

Resources: 5

#### Resources
Icons 1Maps 2Spells and Systems 2

i see, why don't you tried it?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It is the same. JNGP 2.0 uses kLoader and W3MultipleLoader. Moyack implemented a nice window that gives some info on it and allows you to specify how many instances to run.

#### DivineLight

#### DivineLight

 Level 13

JoinedJan 30, 2012Messages1,297

Resources: 5

#### Resources
Icons 1Maps 2Spells and Systems 2

Warcraft mouse Capture don't work with my notebook, is it caused by windows or something?

how to fix that?

i try disabling my Windows firewall before attempting to run LAN already but still the same

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

rulerofiron99 said:

 That's war3's usual non-multi error.


Make sure your extracted everything correctly and that you're running the multi exe for all instances. Make sure you aren't already running a normal instance (not launched with the multi exe) of the game.


Click to expand...


The instruction is very simple to understand, so yes I followed, still not working...Im using win7 64bit and didnt download the Warcraft 3 Mouse Capture coz you said it's optional...


If you want a video I can show it you...

#### Combinemaster

C

#### Combinemaster

I am having the same issue as mckill2009, I can't have multiple instance running even though I used the W3MultipleLoader.exe to TRY to run multiple war3's.

For the first time it works, but then once I alt-tab and click on it again (I run it as admin) it says "Warcraft 3 was unable to initialize"

any help would be appreciated.

#### BloodDrunk

#### BloodDrunk

 Level 18

JoinedMay 11, 2012Messages2,108

Resources: 1

#### Resources
Maps 1

Combinemaster said:

 I am having the same issue as mckill2009, I can't have multiple instance running even though I used the W3MultipleLoader.exe to TRY to run multiple war3's.

For the first time it works, but then once I alt-tab and click on it again (I run it as admin) it says "Warcraft 3 was unable to initialize"

any help would be appreciated.


Click to expand...

Same, same, same...

And Also: I didn't get any NewGen War3 file in that JassNewGenPack20. Is that normal?

The path in ''Config'' Text Document in ''Blizzmod'' folder in JNGP folder changes everytime to nothing when I set the path of my warcraft 3....

scumbag wc3

#### Attachments

Multiplayer Error.jpg

 274.7 KB

 · Views: 698



#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

I don't know why this even got approved in the first place without benchmark testing...coz this truly mislead people that it's working...

#### BloodDrunk

#### BloodDrunk

 Level 18

JoinedMay 11, 2012Messages2,108

Resources: 1

#### Resources
Maps 1

maybe it's working, but it's problem in people's system or smth...

Anyway, is there any other way to run Wc3 2 times beside KLoader?

Lol, I got this to work. If the run multiple instances of wc3 in JNGP doesn't works, just run wc3 through the .exe file in blizzmod file (make sure you've set the path correctly in config file)


 Last edited: Jul 13, 2013

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

mckill it does work. Some people get errors and others don't. However, this is an external/3rd party program, so it will take some time before we find the solutions to all the issues.

@The_blood: Nice.

#### DivineLight

#### DivineLight

 Level 13

JoinedJan 30, 2012Messages1,297

Resources: 5

#### Resources
Icons 1Maps 2Spells and Systems 2

kLoader didn't work for me so i use JNGP...

 Lol, I got this to work. If the run multiple instances of wc3 in JNGP doesn't works, just run wc3 through the .exe file in blizzmod file (make sure you've set the path correctly in config file)


Click to expand...

i use the same method as you

#### dinis-madarbiev

#### dinis-madarbiev

 Level 2

JoinedDec 24, 2008Messages22

As i try to open lan game or join to smb`s game in battlnet window just dissapesrs


what to do?

 You must log in or register to reply here.


### Similar threads

Locked

 Trick for test a map with 2 user if i dont have another people for test it?


shadowvzs

Oct 29, 2012

World Editor Help Zone

Replies10Views4K

Oct 31, 2012
Hashjie

[Misc]
 Lan hosting multiple players single PC


juvian

Aug 28, 2018

Tutorial Archive

Replies29Views14K

Nov 25, 2025
Ravager16829

Locked

 Need (desperately) kloader for 1.29


puffdragon

Jun 4, 2018

Warcraft Discussion

Replies2Views3K

Jun 5, 2018
puffdragon

 Running War3 in Multi Instance


Mr.Pockets00000

Sep 24, 2015

Misc/General Mapping Tutorials

Replies7Views7K

Feb 24, 2016
Mr.Pockets00000

 Warcraft 3 in Windows 10 OS


A Void

Aug 28, 2015

Misc/General Mapping Tutorials
2

Replies69Views110K

Aug 1, 2020
Atasha JG

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