---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/lan-hosting-multiple-players-single-pc.308171/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Lan Hosting Multiple Players Single Pc
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/lan-hosting-multiple-players-single-pc.308171/>
#### juvian

#### juvian

 Level 4

JoinedSep 25, 2017Messages56

Resources: 1

#### Resources
Tutorials 1

As you all know, kloader has not been working for quite some time, and no new alternative has been proposed. With 1.30 version there is no need anymore for sandboxie to be able to run 2 clients, but trying to join lan from both fails, even using sandboxie. This leaves map testing for multiplayer only possible from older w3 versions or testing directly from bnet or using 2 computers.

After many hours of trying I have found an alternative, which allows for any number of lan clients on 1.30.1 patch on w10 (should work for other windows version too).

Install ForceBindIP | r1ch.net

Create a .bat file on same folder as where the program was installed with this content:

SET W3Path=E:\Programs\w3\Warcraft III

start ForceBindIP -i 127.0.0.1 "%W3Path%\Warcraft III.exe"

start ForceBindIP -i 127.0.0.2 "%W3Path%\Warcraft III.exe"

Change E:\Programs\w3\Warcraft III to your warcraft path.

Save and run the bat file (double click). If everything went alright, 2 warcraft instances should be opened and you will be able to access lan from both.

Create a game from one and join from the other.
If you want more than 2 instances, just fill another line with a different valid ip (any in the range 127.0.0.1 to 127.255.255.254).

For 1.28-1.29 patch, you need to use sandboxie for the second instance and the .bat second line should be something like:

start E:\Programs\Sandboxie\Start.exe ForceBindIP -i 127.0.0.2 "%W3Path%\Warcraft III.exe"

For 1.30.2+ patch, forget about ForceBindIP and just open second window with sandboxie, you can enter lan with both and it will work.

Happy map testing!

#### Attachments

upload_2018-8-28_0-33-32.png

 615.1 KB

 · Views: 837



upload_2018-8-28_0-33-54.png

 564 KB

 · Views: 766




 Last edited: Jan 4, 2019

#### Tutaste

#### Tutaste

 Level 1

JoinedOct 24, 2018Messages4

Hey, i tried it with windows 8.1 and didn't work. The error message is "Warcraft III was unable to initialize. Error 0." its not opening second wc3, any idea?

#### juvian

#### juvian

 Level 4

JoinedSep 25, 2017Messages56

Resources: 1

#### Resources
Tutorials 1

Tutaste said:

 Hey, i tried it with windows 8.1 and didn't work. The error message is "Warcraft III was unable to initialize. Error 0." its not opening second wc3, any idea?


Click to expand...

I guess you are trying with 1.30.1, where they removed the "bug" that lets you open multiple w3 instances. A possible solution is to use sandboxie for the second instance and then change second line to something like

start E:\Programs\Sandboxie\Start.exe ForceBindIP -i 127.0.0.2 "%W3Path%\Warcraft III.exe"

#### Franconis

F

#### Franconis

I tested this and verified that two instances are possible in 1.30.1 using this method. I ran the first using ForceBindIP and the first two lines of your original script:

 SET W3Path=E:\Programs\w3\Warcraft III

start ForceBindIP -i 127.0.0.1 "%W3Path%\Warcraft III.exe"


Click to expand...

For the second I just dragged WarcraftIII.exe into Sandboxie and they found each other just fine.

#### CallMeWhale

C

#### CallMeWhale

hi, juvian

sandboxie + ForceBindIP works great with 1.30.1 for the last 2 months, however, it seems no longer working with 1.30.2. as video below shows, the game just stuck here with 1.30.2 after clicking "join the game" button. any new solution?

(also idk if blz see this feature as a bug and keeps an eye on this thread, we just want to test map more easily, not doing sth bad. why bother "fix this bug" (which actually not) and force us using a more laborious way like virtualbox/vmware? plz leave us a way. great thx)


 Last edited by a moderator: Dec 27, 2018

#### juvian

#### juvian

 Level 4

JoinedSep 25, 2017Messages56

Resources: 1

#### Resources
Tutorials 1

@CallMeWhale that's unfortunate. I don't use bnet anymore so don't have new patch to try, might try when I get back home from holidays in a week if I remember

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I'm going to leave this in the submissions forum til I get the chance to test & verify that this works on the latest patch on my PC. Anyone else willing to give it a go to see if it works?

#### juvian

#### juvian

 Level 4

JoinedSep 25, 2017Messages56

Resources: 1

#### Resources
Tutorials 1

@CallMeWhale yeah it doesn't seem to work anymore with this bonjour thing, so only works on 1.30.1 patch or older

#### juvian

#### juvian

 Level 4

JoinedSep 25, 2017Messages56

Resources: 1

#### Resources
Tutorials 1

@PurgeandFire@CallMeWhale updated post with what to do on older and newer patch.

#### Tutaste

#### Tutaste

 Level 1

JoinedOct 24, 2018Messages4

well, i can't open 2 programs with sandboxie. i need a key. can you help please? @juvian

#### juvian

#### juvian

 Level 4

JoinedSep 25, 2017Messages56

Resources: 1

#### Resources
Tutorials 1

@Tutaste you only need to open 1 warcraft instance with sandboxie, the other without it as usual. That way you have 2 using free version.

#### Tutaste

#### Tutaste

 Level 1

JoinedOct 24, 2018Messages4

Yeah its worked. thanks a lot

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Very short and simple.

Admittedly useful though.

However, I would argue too simple.

The tutorial basically comes down to:

1. Download X

2. copy this code / run program (depending on version)

Also noteworthy that I did not get this to work, when I ran the .bat file it said something along the lines of command ForceBindIP not found. Based on one of the replies I figure I need to specify the path which was not included in the tutorial.

But that's a side point as I don't think this can be salvaged.

#### juvian

#### juvian

 Level 4

JoinedSep 25, 2017Messages56

Resources: 1

#### Resources
Tutorials 1

Chaosy said:

 Very short and simple.

Admittedly useful though.

However, I would argue too simple.

The tutorial basically comes down to:

1. Download X

2. copy this code / run program (depending on version)

Also noteworthy that I did not get this to work, when I ran the .bat file it said something along the lines of command ForceBindIP not found. Based on one of the replies I figure I need to specify the path which was not included in the tutorial.

But that's a side point as I don't think this can be salvaged.


Click to expand...

Yeah its simple, yet it took me days to find a way to make it work after reading many posts saying it was just not possible. Given new patches I am not sure why would anyone still need this but at the time it was useful. You don't need to specify the path, ForceBindIP is a program you need to install and for the bat to work it needs to be on same folder as ForceBindIP.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Ahhh, I did install the program but did not put it in the same folder.

I 100% understand you, there should be a place somewhere to submit "useful tips"

Such as lists with IDs, color codes, terrain tileset paths etc

And possibly this.

However, I am just the messenger who reads the rules and I think this is too simple to be classified as a tutorial.

I encourage you to poke an admin or post in the site discussion section and ask for a subforum for these short tips. Perhaps it can happen.

#### Shadowmanthegod01

#### Shadowmanthegod01

 Level 2

JoinedAug 2, 2018Messages7

Is this work on 1.30.4?

#### juvian

#### juvian

 Level 4

JoinedSep 25, 2017Messages56

Resources: 1

#### Resources
Tutorials 1

Shadowmanthegod01 said:

 Is this work on 1.30.4?


Click to expand...

That's a rather specific version... which I don't have installed but shouldn't be hard to try. It did work in 1.30.2 but who knows

#### TheFireMage

#### TheFireMage

 Level 6

JoinedJul 23, 2018Messages243

Ok so, I tried to run this on 1.28.5 but I got this error instead

#### juvian

#### juvian

 Level 4

JoinedSep 25, 2017Messages56

Resources: 1

#### Resources
Tutorials 1

TheFireMage said:

 Ok so, I tried to run this on 1.28.5 but I got this error instead

View attachment 402207

Click to expand...

What did you set as w3path ?

#### TheFireMage

#### TheFireMage

 Level 6

JoinedJul 23, 2018Messages243

juvian said:

 What did you set as w3path ?


Click to expand...

My Warcraft game directory

#### juvian

#### juvian

 Level 4

JoinedSep 25, 2017Messages56

Resources: 1

#### Resources
Tutorials 1

TheFireMage said:

 My Warcraft game directory


Click to expand...

I will guess you are using program files or something with a space in which case instead of start C:\Program Files\Sandboxie\Start.exe ForceBindIP it should be start "C:\Program Files\Sandboxie\Start.exe" ForceBindIP

#### TheFireMage

#### TheFireMage

 Level 6

JoinedJul 23, 2018Messages243

juvian said:

 I will guess you are using program files or something with a space in which case instead of start C:\Program Files\Sandboxie\Start.exe ForceBindIP it should be start "C:\Program Files\Sandboxie\Start.exe" ForceBindIP


Click to expand...

So the quotation should be included?

#### juvian

#### juvian

 Level 4

JoinedSep 25, 2017Messages56

Resources: 1

#### Resources
Tutorials 1

TheFireMage said:

 So the quotation should be included?


Click to expand...

yes paths with spaces in them need to go between double quotes

#### TheFireMage

#### TheFireMage

 Level 6

JoinedJul 23, 2018Messages243

I got the typical error from using quote

#### juvian

#### juvian

 Level 4

JoinedSep 25, 2017Messages56

Resources: 1

#### Resources
Tutorials 1

TheFireMage said:

 I got the typical error from using quote

View attachment 402452View attachment 402453

Click to expand...

Not sure, file looks file. Maybe try restarting and check sandboxie did launch

#### TheFireMage

#### TheFireMage

 Level 6

JoinedJul 23, 2018Messages243

juvian said:

 Not sure, file looks file. Maybe try restarting and check sandboxie did launch


Click to expand...

File looks file, ok

Also, I tried launching separately with 2 files, but the results remain the same

Edit: I tried launching sandboxie only and it works but the second instance cannot connect to network when I tried to enter LAN game

#### w1919

#### w1919

 Level 3

JoinedDec 3, 2019Messages13

for my 1.26: no any methods are working (LAN on 2nd window is "Could not connect to the network")

 Code:

"C:\Program Files\Sandboxie\Start.exe" /box:S1 "%FBIP%\ForceBindIP" -i 127.0.0.1 "%W3Path26%\Frozen Throne.exe" -window
"C:\Program Files\Sandboxie\Start.exe" /box:S2 "%FBIP%\ForceBindIP" -i 127.0.0.2 "%W3Path26%\Frozen Throne.exe" -window

for my 1.28.5 : not_sandboxie_and_ForceBindIP + sandboxie - is not working (2nd window can't see a created game)

 Code:

start ForceBindIP -i 127.0.0.1 "%W3Path28%\Warcraft III.exe" -window
"C:\Program Files\Sandboxie\Start.exe" /box:S2 "%W3Path28%\Warcraft III.exe" -window

for my 1.28.5 : sandboxie(box1)_and_ForceBindIP + sandboxie(box2)_and_ForceBindIP - works perfectly

 Code:

"C:\Program Files\Sandboxie\Start.exe" /box:S1 "%FBIP%\ForceBindIP" -i 127.0.0.1 "%W3Path28%\Warcraft III.exe" -window
"C:\Program Files\Sandboxie\Start.exe" /box:S2 "%FBIP%\ForceBindIP" -i 127.0.0.2 "%W3Path28%\Warcraft III.exe" -window

for my 1.28.5 + JN Loader : sandboxie(box1)_and_ForceBindIP + sandboxie(box2)_and_ForceBindIP - is not working (LAN on 2nd window is "Could not connect to the network")

 Code:

"C:\Program Files\Sandboxie\Start.exe" /box:S1 "%FBIP%\ForceBindIP" -i 127.0.0.1 "%W3Path28%\JNLoader.exe" -window
"C:\Program Files\Sandboxie\Start.exe" /box:S2 "%FBIP%\ForceBindIP" -i 127.0.0.2 "%W3Path28%\JNLoader.exe" -window

so can anyone make it working for 1.28.5 + JN Loader? (JN Loader is open-sourced on GitHub)


 Last edited: Nov 25, 2025

#### Ravager16829

#### Ravager16829

 Level 40

JoinedMay 14, 2021Messages2,455

w1919 said:

 so can anyone make it working for 1.28.5 + JN Loader?


Click to expand...

I don't know it really works or not.

3rd party WC3 servers that use special launcher like M16 (Korean WC3 server), Iccup, or Eurobattle.net are pretty much hardcoded for this trick.


 Last edited: Nov 25, 2025

#### w1919

#### w1919

 Level 3

JoinedDec 3, 2019Messages13

Ravager16829 said:

 JNLoader, Iccup, or Eurobattle.net are pretty much hardcoded for this trick.


Click to expand...

JNLoader is not a server itself, server is m16

You can play by JNLoader in local LAN with friends, without m16 (and can't do the same with Iccup or Eurobattle.net)

And there are no maps which works on Iccup or Eurobattle.net only. But JNLoader-only maps are present

#### Ravager16829

#### Ravager16829

 Level 40

JoinedMay 14, 2021Messages2,455

@w1919 Maybe juvian can help?

---

Well, it seems that this thread is outdated (it's 3 years ago), but I can tell you this trick doesn't seem to work on Reforged because LAN testing is completely broken: Can't do LAN to test map.

 You must log in or register to reply here.


### Similar threads

 Can't do LAN to test map.


xorkatoss

Oct 21, 2025

Warcraft Discussion

Replies13Views2K

Dec 24, 2025
Ravager16829

Locked

[Crash]
 W3: Map size limit for LAN


RadimX

Apr 13, 2020

World Editor Help Zone

Replies1Views2K

Apr 13, 2020
Planetary

Locked

 map disconecting players on LAN


Raydrik

Dec 10, 2005

Map Development

Replies1Views1K

Dec 10, 2005
soulbad

Locked

[Solved]
 Test Map and W3 path bugged?


LovG

Oct 2, 2018

World Editor Help Zone

Replies9Views5K

Oct 3, 2018
LovG

Locked

 Can't play LAN after updating from 1.27


Wayshan

Dec 9, 2019

Warcraft Discussion

Replies3Views3K

Dec 11, 2019
deepstrasz

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