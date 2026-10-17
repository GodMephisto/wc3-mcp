---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/warcraft-iii-on-wineskin-mac.257050/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Warcraft III on Wineskin Mac
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/warcraft-iii-on-wineskin-mac.257050/>
#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Introduction

Throughout the years, Warcraft III has slowly become less and less compatible with OS X.

 OS X 10.7+ cannot install the game since the installer is in PPC format (deprecated).

 OS X 10.9+ cannot play on Battle.net/LAN due to the deprecation of open transport.
Most players would recommend installing a Windows partition, but that isn't that convenient.

Have no fear, though. Through Wineskin, you can run Warcraft III without issues.

​

Prerequisites

The first step is to download Wineskin here.

Unzip the file, and move the application to your applications folder.

Note: You may receive a warning that prevents you from running a program that wasn't downloaded

from the app store. To circumvent this, open System Preferences, go to Security & Privacy,

and allow apps downloaded from "Anywhere".

Next, install XQuartz here.

Select the .dmg file. Open it. Double click on XQuartz.pkg. Go through the installation

​

Setting up Wineskin

 Open Wineskin Winery.

 In the main window, click the + symbol, left of "New Engine(s) Available".

Select Download and Install on the latest version.

 Select Create New Blank Wrapper, and name it "WarcraftIII.app".

 Press OK. If you don't have certain packages installed on your computer, you will be prompted

to download them (just click "Install"). These packages include Mono, Gecko, etc. It isn't necessary

to install them, but I can't guarantee whether this will work without those packages.

 Once the wrapper is created (it may take a while), click "Show in Finder".

 You should see a "WarcraftIII" app. Right now, it doesn't do anything. It just has WINE and a windows-like

file setup. Right-click the app, and select Show Package Contents. Look for "Wineskin". This is

the heart of the app: this will be used to configure Warcraft III and set it up. Open it up.

 Now we are ready to install Warcraft 3!

Warcraft III Installation

 Log in to battle.net:

https://us.battle.net/account/management/

 Register your CD key if you haven't already, and download the Windows Reign of Chaos game client.

(Click on the game, and click the windows icon. Or go to "Game Clients", and download there). Or use a CD.

 Go back to your Wineskin configuration window, and click "Install Software".

Click "Choose Setup Executable".

 Select the RoC installer executable. When prompted to choose an installation location,

just choose C:\Program Files\. Go through the installation as usual, and close when done.

 Do the same exact thing for TFT. Get the game client (online or CD), and select "Install Software".

Go through the installation, and close the installer when done.

 The installers will create some stuff on your desktop. You can feel free to delete it.

 Sadly, you must download one last thing: the patch. Get it here.

Download patch 1.26a. Select "Install Software", "Choose Setup Executable", and select the patch.

 Once it is finished, it will open Warcraft III. Quit it.

 Go to the Wineskin configuration, and click "Advanced". Change the settings to match this:

 Go to "Screen options", and change the settings to match this:

Note: -opengl is optional. It may actually lead to some graphical differences. If you want

wc3 without -opengl, then see the (Optional) note to rename the movies folder--or else

the cinematics will cause the game's display to mess up.

 Select "Test Run", and it should work! Feel free to adjust the resolution and settings

by going to Options -> Video.

(Optional) Sadly, the blizzard movies will not load properly. You can avoid issues with

 it by navigating to the Warcraft III folder and renaming the "Movies" folder to something

 else. This is optional if you run with the -opengl flag. If you run it without the -opengl flag,

 then you should rename the movies folder to ensure the program won't get stuck

loading the movies.

Final Q&A (Please Read)

Now what do I do?

Now you can close the Wineskin configuration, and play Wc3. Simply double click the "WarcraftIII.app", and

it should automatically load.

What if I want to run Reign of Chaos?

Go to the wineskin configuration, select "Advanced", and change the Windows EXE to war3.exe

Can I run the world editor?

Yes, but it is rather buggy. The toolbars will not show up and certain things will freeze the editor.

I haven't tested it that much though. Perhaps I'll try to see if a particular wineskin configuration works.

Any way to minimize? Or quit?

To pull out of full-screen, press Cmd+Option+A (press the same to go back into full screen).

To quit the game, press Cmd+Q.

Also, as a general tip, go to System Preferences -> Keyboard, and checkmark

"Use all F1, F2, etc. keys as standard function keys", in case you want to use F1/F2/F3. Else use fn.

Any bugs?

There might be some graphical glitches: the CGI cinematics do not work, the loading screen has some

lines going through it, etc. but those are all minor.

The option/alt will not display health bars. Use the command button, or press square brackets [] instead.

How do I install maps?

Navigate to your WarcraftIII.app in finder. Right click it -> Show Package Contents. Go to

drive_c/Program Files/Warcraft III/Maps and paste the map as you normally would.

Can I delete the installer folders?

Yes, and I recommend doing so. Go to drive_c/Program Files/ and delete the installers. They'll

take up about ~1 gb of space.

Everything appears to be blurry after I close Warcraft III.

This issue is common with retina display computers. Go to System Preferences -> Display,

and switch to one of the other resolutions, and then back to your standard one. It should

fix the issue.

Congratulations, you can play Warcraft III at your leisure! Let me know if you have any issues.

Original credits belong to prouve for his guide.

 Statistics


 File size: ~1.6 gb

 CPU: Averages around 25-30% CPU use. Without -opengl, it is a little more CPU-intense.

#### Attachments

Screen Shot 2014-09-08 at 7.52.04 PM.png

 77.2 KB

 · Views: 13,377



Screen Shot 2014-09-08 at 8.06.27 PM.png

 38.1 KB

 · Views: 13,509



Screen Shot 2014-09-08 at 8.08.28 PM.png

 37.3 KB

 · Views: 13,123



Screen Shot 2014-09-08 at 8.11.14 PM.png

 31.3 KB

 · Views: 13,149



Screen Shot 2014-09-08 at 8.19.14 PM.png

 113.9 KB

 · Views: 13,640



Screen Shot 2014-09-08 at 10.32.43 PM.png

 58.8 KB

 · Views: 13,030



Screen Shot 2014-09-08 at 10.23.11 PM.png

 57 KB

 · Views: 13,155



Screen Shot 2014-09-08 at 9.15.45 PM.png

 482.1 KB

 · Views: 15,439



Screen Shot 2014-09-08 at 8.30.02 PM.png

 55.9 KB

 · Views: 13,343



Screen Shot 2014-09-08 at 8.22.46 PM.png

 44.1 KB

 · Views: 13,020




 Last edited: May 21, 2015

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


I just Googled for this and found my own website with your guide on display. That's pretty cool .


Purge, I am getting weird graphics using -opengl, are you getting this too?

#### Attachments

Screenshot 2015-01-02 19.08.55.png

 573.4 KB

 · Views: 983



#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Weird. I did not get that issue. Which model are you using, and which OS?

Did you try taking out -opengl? And is your wineskin under the default settings?

#### Kakerate

#### Kakerate

 Level 14

JoinedOct 18, 2013Messages760

That's interesting. So when you are running War3 through Wineskin, how robust is it? Will War3 still crash with .blps having too few mipmaps? I'd test myself but I've never owned a Mac XD

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Kakerate said:

 That's interesting. So when you are running War3 through Wineskin, how robust is it? Will War3 still crash with .blps having too few mipmaps? I'd test myself but I've never owned a Mac XD


Click to expand...

EDIT: I apologize for the misinformation. It should not crash.


 Last edited: Jan 30, 2016

#### Kakerate

#### Kakerate

 Level 14

JoinedOct 18, 2013Messages760

& Good tutorial, its nice for Mac users with newer OS to be able to play War3 again, along with other older games. ^^

That's unfortunate that crashes from mip maps and things of that nature have no work around to the end-user of maps :L Ah well.


 Last edited by a moderator: Jan 30, 2016

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


PurgeandFire said:

 Weird. I did not get that issue. Which model are you using, and which OS?


Did you try taking out -opengl? And is your wineskin under the default settings?


Click to expand...


MacBook Air 2011, Yosemite. Actually I had the same error with native Warcraft 3 for Mac a few years ago.


If I remove -opengl it looks fine but the framerate averages about ~25, if I use -opengl it averages around 55.


The black graphics are only for the menus, in-game it looks fine.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Ralle said:

 MacBook Air 2011, Yosemite. Actually I had the same error with native Warcraft 3 for Mac a few years ago.


If I remove -opengl it looks fine but the framerate averages about ~25, if I use -opengl it averages around 55.


The black graphics are only for the menus, in-game it looks fine.


Click to expand...


You're having minimap issues as well, right? I wish I knew a solution to it. I've seen some people encounter the same issue over on reddit. Sadly, I can't reproduce the issue.

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


I think it has to do with Intel graphics and OpenGL.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I'm running Intel Iris 1536. You have intel hd 3000 graphics card, right?

#### Gnilwod

G

#### Gnilwod

Finished everything, but having trouble starting it up.

As I said in the title, I went through the steps and downloaded everything without issue. But now that I want to run the game when I select the WarcraftIII.app the same options box pops up from the beginning. ('Install software' 'Screen Options' etc.)


Idk what this could mean other than perhaps I saved it all in the wrong place? I'm gonna keep trying but if anyone has had a similar issue please let me know how you fixed it.


Thanks for your time everyone.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Gnilwod said:

 As I said in the title, I went through the steps and downloaded everything without issue. But now that I want to run the game when I select the WarcraftIII.app the same options box pops up from the beginning. ('Install software' 'Screen Options' etc.)


Idk what this could mean other than perhaps I saved it all in the wrong place? I'm gonna keep trying but if anyone has had a similar issue please let me know how you fixed it.


Thanks for your time everyone.


Click to expand...


I apologize for the late response! Feel free to send me a PM if you want to discuss this back-and-forth.


1. In the Wineskin window that pops up to you, go to "Advanced -> Test Run". See if that runs it for you (assuming that you have the "Windows EXE" part set to a valid executable). Let me know if this works or not.


2. Make sure that you're running the outermost app and not the "Wineskin" app that is inside it.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

I tried this with Notepad++ before, works well, thanks. Though I switched to CotEditor since Notepad++ doesn't look that good with Mac.

#### macfan_

M

#### macfan_

Thanks!

Thanks a lot for the diligent and detailed work!

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


I hope this stuff is not necessary anymore since the latest patch. What is your experience Purge?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

@macfan: No problem, I'm happy it could help!

@Ralle: Hopefully! I'll revise this tutorial a bit. It is still useful for certain versions of OS X. PPC Mac users are locked into using Wineskin because they cannot upgrade past OS X 10.7.5, so now this is their only option to play on the latest patch. (since the installer/game requires OS X 10.9 and above)

I'll probably rewrite the introduction to include details/suggestions/etc. and point them towards the installer.

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


BTW. I doubt they updated the Mac World Editor.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Ralle said:

 BTW. I doubt they updated the Mac World Editor.


Click to expand...


Nope, they didn't. It isn't even included in the installation. But at least we can play the game.

#### k0gama

#### k0gama

 Level 2

JoinedJul 8, 2016Messages5

Hey, I just joined today after following some of these instructions. I have a mac and thought I wouldn't be able to play my childhood game ever again.

I did everything seemingly properly but when I install software it just opens the program like a standalone so it will open up Frozen Throne or RoC and I can play everything but Bnet. It didn't go through any installing screen for me to put a CD key in to get to Bnet

I think the issue may be with the C drive thing, I used to have windows and know thats a windows things. As far as I know (which apparently might not be much) OS X doesn't have a C drive?

any help would be appreciated! Thank you!

edit:

I found the C drive and program files in package contents but when moving warcraft inside program files I can't open it and if I don't put it in there and open RoC it just opens up

edit 2:

registered my almost 11 year old RoC key, my old TFT key must have been taken by a friend

when I tried installing RoC through the downloaded installer it opens up frozen throne somehow...no place to input cd keys but says its installed


 Last edited: Jul 8, 2016

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

k0gama said:

 I did everything seemingly properly but when I install software it just opens the program like a standalone so it will open up Frozen Throne or RoC and I can play everything but Bnet. It didn't go through any installing screen for me to put a CD key in to get to Bnet

I think the issue may be with the C drive thing, I used to have windows and know thats a windows things. As far as I know (which apparently might not be much) OS X doesn't have a C drive?

any help would be appreciated! Thank you!


Click to expand...

Weird. Where did you download the installers from? Was it from us.battle.net? Could you take a screenshot of what it looks like when you try to use "Install Software"?

About the C drive, yeah that is a Windows only thing. Wineskin tries to emulate Windows by using your hard drive as a "virtual" drive, and it'll probably say "Z:/" instead of "C:/". You don't need to worry about that though.

Just as a note, you only need to use this tutorial if you are experiencing problems with the new mac installer that Blizzard made (or if you want to run w3arena or something).

k0gama said:

 edit:

I found the C drive and program files in package contents but when moving warcraft inside program files I can't open it and if I don't put it in there and open RoC it just opens up


Click to expand...

Make sure you set the path to your file as shown in this image:

Otherwise it'll just open the winery app.

 edit 2:

registered my almost 11 year old RoC key, my old TFT key must have been taken by a friend

when I tried installing RoC through the downloaded installer it opens up frozen throne somehow...no place to input cd keys but says its installed


Click to expand...

Nice.

So you've tried the mac installer, right? (on us.battle.net) If that is what you're talking about, then yeah. The downloader will download the installer for you, and put it in the same folder as the downloader app. You should check your "Downloads" folder for a Warcraft III Reign of Chaos installer. I can give you more info a bit later, I gotta take off. But definitely try seeing if it put that file anywhere!

#### k0gama

#### k0gama

 Level 2

JoinedJul 8, 2016Messages5

PurgeandFire said:

 Weird. Where did you download the installers from? Was it from us.battle.net? Could you take a screenshot of what it looks like when you try to use "Install Software"?

About the C drive, yeah that is a Windows only thing. Wineskin tries to emulate Windows by using your hard drive as a "virtual" drive, and it'll probably say "Z:/" instead of "C:/". You don't need to worry about that though.

Just as a note, you only need to use this tutorial if you are experiencing problems with the new mac installer that Blizzard made (or if you want to run w3arena or something).

Make sure you set the path to your file as shown in this image:

Otherwise it'll just open the winery app.

Nice.

So you've tried the mac installer, right? (on us.battle.net) If that is what you're talking about, then yeah. The downloader will download the installer for you, and put it in the same folder as the downloader app. You should check your "Downloads" folder for a Warcraft III Reign of Chaos installer. I can give you more info a bit later, I gotta take off. But definitely try seeing if it put that file anywhere!


Click to expand...

Thank you for the reply! I contacted Blizzard about getting a replacement CD key because I have an old picture with my email displayed by the CD keys and an old customer service conversation mentioned as long as I have proof they wouldn't make me buy any new keys. Anyways I downloaded RoC and have been working with that.

So first it downloads the 'Downloader" from battlement.

I set the path to the Downloader in my program files and it downloads the installer.

In the advanced settings I set the path to the installer and try to run it but it just opens up a screen saying "play warcraft III"

If I go to "install software" it just tells me to choose the setup executable which if I choose the installer it brings me to the play warcraft III screen.

Now the thing is that I haven't run the installer and only the downloader. In my program files there are no exe files for the actual game. When I open the downloader or installer it just brings me to play Warcraft 3, no place to put in a cd key and it also opens up the Frozen Throne but its not even in program files as I didn't download it.

I know you said this is for people who had trouble with the Mac installer. Well I wasn't aware one existed until I went to Bnet so logically I tried that and was excited because this was a native installer by Blizzard.

The installation for mac went smooth, I put my cd keys in but when I got into RoC battlenet wasn't working. I live in the US but the only server working was Asia or Europe and people playing online said those servers were up.

I exited out disappointed and opened up my mac RoC again to play on that sever anyways but now when I tried a second and third time no servers worked at all

I apologize if this is too long or a little confusing. I'm doing my best to understand and relay the problem.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

k0gama said:

 Thank you for the reply! I contacted Blizzard about getting a replacement CD key because I have an old picture with my email displayed by the CD keys and an old customer service conversation mentioned as long as I have proof they wouldn't make me buy any new keys. Anyways I downloaded RoC and have been working with that.

So first it downloads the 'Downloader" from battlement.

I set the path to the Downloader in my program files and it downloads the installer.

In the advanced settings I set the path to the installer and try to run it but it just opens up a screen saying "play warcraft III"

If I go to "install software" it just tells me to choose the setup executable which if I choose the installer it brings me to the play warcraft III screen.

Now the thing is that I haven't run the installer and only the downloader. In my program files there are no exe files for the actual game. When I open the downloader or installer it just brings me to play Warcraft 3, no place to put in a cd key and it also opens up the Frozen Throne but its not even in program files as I didn't download it.

I know you said this is for people who had trouble with the Mac installer. Well I wasn't aware one existed until I went to Bnet so logically I tried that and was excited because this was a native installer by Blizzard.

The installation for mac went smooth, I put my cd keys in but when I got into RoC battlenet wasn't working. I live in the US but the only server working was Asia or Europe and people playing online said those servers were up.

I exited out disappointed and opened up my mac RoC again to play on that sever anyways but now when I tried a second and third time no servers worked at all

I apologize if this is too long or a little confusing. I'm doing my best to understand and relay the problem.


Click to expand...

Ah, oops! I just realized that I haven't updated this tutorial for patch 1.27. They probably changed the installer for Windows, so the tutorial is a bit inconsistent. I'll see if I can make a proper guide in a few days.

Thanks for pointing it out, and hopefully you'll be able to play on BNet soon enough!

As for your mac RoC not being able to log in to US servers, that is pretty strange. Is there any error? Have you tried disabling your firewall? (Open system preferences -> security & privacy -> Firewall tab -> Click the lock sign at the bottom left corner -> type your password -> uncheck the firewall. I don't recommend keeping it this way, but it is fine if you just want to play wc3.

#### k0gama

#### k0gama

 Level 2

JoinedJul 8, 2016Messages5

PurgeandFire said:

 Ah, oops! I just realized that I haven't updated this tutorial for patch 1.27. They probably changed the installer for Windows, so the tutorial is a bit inconsistent. I'll see if I can make a proper guide in a few days.

Thanks for pointing it out, and hopefully you'll be able to play on BNet soon enough!

As for your mac RoC not being able to log in to US servers, that is pretty strange. Is there any error? Have you tried disabling your firewall? (Open system preferences -> security & privacy -> Firewall tab -> Click the lock sign at the bottom left corner -> type your password -> uncheck the firewall. I don't recommend keeping it this way, but it is fine if you just want to play wc3.


Click to expand...

Again, thank you for the reply

I have an update on the installation process for RoC

I deleted my wrapper from wineskin and made another one with a newer version of the engine. Now when I execute the downloader I am brought to the installer and it installs as it should within seconds.

Now it opens up RoC but I have the same problem as I have on the mac one. I can't connect to battlenet :/

Also I tried disabling my firewall but it doesn't seem to help. I have no errors except the "you might be connecting to an invalid server, if you're using a modem..." etc

My internet connection is of course working, I'm going to try turning off my antivirus and see if that helps at all (I just keep it to stop a spread of windows viruses if I share files)

Another thing I noticed on the Mac RoC is that every now and again it will severely freeze and Cmd+Q won't work, I have no option but to hold down the power button and restart

edit:

Disabling firewall and anti virus did not help on Mac version of RoC and Wineskin installation

also I found out that the mac one crashes overtime I try to access Asia (I try all of the servers because the one time it worked it was on either Asia or Europe, most likely Europe)


 Last edited: Jul 9, 2016

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

k0gama said:

 Again, thank you for the reply

I have an update on the installation process for RoC

I deleted my wrapper from wineskin and made another one with a newer version of the engine. Now when I execute the downloader I am brought to the installer and it installs as it should within seconds.

Now it opens up RoC but I have the same problem as I have on the mac one. I can't connect to battlenet :/

Also I tried disabling my firewall but it doesn't seem to help. I have no errors except the "you might be connecting to an invalid server, if you're using a modem..." etc

My internet connection is of course working, I'm going to try turning off my antivirus and see if that helps at all (I just keep it to stop a spread of windows viruses if I share files)

Another thing I noticed on the Mac RoC is that every now and again it will severely freeze and Cmd+Q won't work, I have no option but to hold down the power button and restart

edit:

Disabling firewall and anti virus did not help on Mac version of RoC and Wineskin installation

also I found out that the mac one crashes overtime I try to access Asia (I try all of the servers because the one time it worked it was on either Asia or Europe, most likely Europe)


Click to expand...

Unfortunately, you may need to contact customer support.  It might be an issue on their end.

But I have heard of that error occurring when you have an invalid CD key. You might want to see if you've been banned without knowing it--so be sure to ask their customer support about it. You can even try their support forums:

http://us.battle.net/forums/en/bnet/12790218/

#### k0gama

#### k0gama

 Level 2

JoinedJul 8, 2016Messages5

PurgeandFire said:

 Unfortunately, you may need to contact customer support.  It might be an issue on their end.

But I have heard of that error occurring when you have an invalid CD key. You might want to see if you've been banned without knowing it--so be sure to ask their customer support about it. You can even try their support forums:

http://us.battle.net/forums/en/bnet/12790218/

Click to expand...

yea, so I contacted customer support and they said they have no idea why but I was auto banned for 13 days. They said it happens on their legacy games and no one knows why. I literally got banned within minutes of downloading the game apparently...

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

@k0gama: Yeah, it is a buttload of time too.

Sorry to hear man. Hopefully they'll have that sorted out at some point, now that they have a team for classic games.

#### k0gama

#### k0gama

 Level 2

JoinedJul 8, 2016Messages5

PurgeandFire said:

@k0gama: Yeah, it is a buttload of time too.

Sorry to hear man. Hopefully they'll have that sorted out at some point, now that they have a team for classic games.


Click to expand...

At least they were able to give me a replacement TFT key though because I had an old picture of proof with email on it before I threw out box. Regular representative refused to do it so I asked for a supervisor and they were more than happy to replace it. I sat there wondering how someone would be stingy about replacing a CD key for a 13 year old game, especially when I had proof I owned it.

on a side note I'm IP banned according to the supervisor for another 9 days due to unknown reasons.

edit: I think my absence from using a computer for anything other than work/essays/internet browsing made me forget how easy it was to bypass an IP ban

using my VPN I made it on lol

IM BACK!!!!!!!!!!


 Last edited: Jul 14, 2016

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

k0gama said:

 At least they were able to give me a replacement TFT key though because I had an old picture of proof with email on it before I threw out box. Regular representative refused to do it so I asked for a supervisor and they were more than happy to replace it. I sat there wondering how someone would be stingy about replacing a CD key for a 13 year old game, especially when I had proof I owned it.

on a side note I'm IP banned according to the supervisor for another 9 days due to unknown reasons.

edit: I think my absence from using a computer for anything other than work/essays/internet browsing made me forget how easy it was to bypass an IP ban

using my VPN I made it on lol

IM BACK!!!!!!!!!!


Click to expand...

Haha, awesome! I'll definitely give that tip out to folks who need it.

#### Leebbzz

#### Leebbzz

 Level 1

JoinedOct 29, 2016Messages2

Hey thanks for the help but i have a problem i used to be able to play wc3 on wine and stuff but suddenly i cant open it forsome reason idid update to seirra but it was working fine but now its not plz help

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Leebbzz said:

 Hey thanks for the help but i have a problem i used to be able to play wc3 on wine and stuff but suddenly i cant open it forsome reason idid update to seirra but it was working fine but now its not plz help

Click to expand...

I haven't tried it on macOS Sierra. Maybe it is an issue with Wineskin?

You can try opening Wineskin Winery and seeing if any new engines are available--install them if so. Then you can go to your Warcraft 3 Wineskin wrapper, right click it -> show package contents, open up the wineskin, and rebuild the wrapper from there.

But I'm not sure if that will work. If I update to sierra, I'll try to test it and let you know!

P.S. Did you get any error messages? Does anything happen when you open wc3? Or does it do nothing?

#### Leebbzz

#### Leebbzz

 Level 1

JoinedOct 29, 2016Messages2

PurgeandFire said:

 I haven't tried it on macOS Sierra. Maybe it is an issue with Wineskin?

You can try opening Wineskin Winery and seeing if any new engines are available--install them if so. Then you can go to your Warcraft 3 Wineskin wrapper, right click it -> show package contents, open up the wineskin, and rebuild the wrapper from there.

But I'm not sure if that will work. If I update to sierra, I'll try to test it and let you know!

P.S. Did you get any error messages? Does anything happen when you open wc3? Or does it do nothing?


Click to expand...

it just doesnt pop up any more ill try what u said thanks

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

@PurgeandFire you still want to make some changes? (was reading posts)

I can't technicaly test what you explain, but it sounds good and useful. What you say? Must some things be updated due to new patch?

#### Dcoil

D

#### Dcoil

Do I check on "force use of system installed XQuartz"? since we had to download XQuartz package at the very first step... It is unchecked by default and you don't say to check it in the tutorial itself. Need to clarify this.

BTW I can't play neither LTA nor KTA nor Foremath because of the desync/fatal error in the Mac version of WC3 that is downloadable from the Battlenet. The only thing that works smoothly in the Mac version of WC3 is the regular maps. Even DOTA has desync issues when someone picks Tiny or Void (chronosphere) on the Mac version. So I had to roll back to the "good old Wineskin" to play some LTA. Zo_om ispired me to do so.)))

#### Attachments

Screen Shot 2017-08-02 at 10.41.00 PM.png

 195.1 KB

 · Views: 241



#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Dcoil said:

 Do I check on "force use of system installed XQuartz"? since we had to download XQuartz package at the very first step... It is unchecked by default and you don't say to check it in the tutorial itself. Need to clarify this.

BTW I can't play neither LTA nor KTA nor Foremath because of the desync/fatal error in the Mac version of WC3 that is downloadable from the Battlenet. The only thing that works smoothly in the Mac version of WC3 is the regular maps. Even DOTA has desync issues when someone picks Tiny or Void (chronosphere) on the Mac version. So I had to roll back to the "good old Wineskin" to play some LTA. Zo_om ispired me to do so.)))


Click to expand...

When I first wrote the tutorial, I had XQuartz enabled, but it isn't necessary. Wineskin has its own internal x11 server (similar to XQuartz), so I wouldn't check "force use of system installed XQuartz" unless your windows aren't showing up--or if you get the error "Wineskin x11 could not be opened".

Also, I should get around to updating this tutorial. You don't need to install the 1.26a installer since it isn't the latest patch. And running battle.net launcher should do most of the work automatically, but I haven't tested it with Wine yet. You can feel free to experiment and see if you get it working, but sorry that the info in this tutorial is a bit outdated. :X

P.S. If you can, make a thread about the desyncs in our patch discussion forum, with any crash logs (if you have them) or steps to reproduce the desync/fatal error. It'll help devs enormously + we'll be able to send that issue directly to the devs.

#### Tamama Nitouhei

T

#### Tamama Nitouhei

This is pretty late but if I already have the warcraft III copy (patched and reign of chaos) should I still install the mono and gecko package and continue like normal or it would be finished already??

#### Kakerate

#### Kakerate

 Level 14

JoinedOct 18, 2013Messages760

software gremlins have got you in their clutches

#### Zerius

#### Zerius

 Level 1

JoinedMar 1, 2018Messages2

Hello everyone my first post on this forum and topic.

I'm running the latest Mac OS on my macbook pro and play on Battlenet mainly but I'm always disconnected.

I have followed all the steps of this tutorial until the part when I have to install the game; I cannot install the .exe because I have no access to the folder if I place it in the program files and if I try to install it outside the folder Wineskine don't allow me to install it since it is not in the Crive.

Do you have any ideas how to figure out ?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Zerius said:

 Hello everyone my first post on this forum and topic.

I'm running the latest Mac OS on my macbook pro and play on Battlenet mainly but I'm always disconnected.

I have followed all the steps of this tutorial until the part when I have to install the game; I cannot install the .exe because I have no access to the folder if I place it in the program files and if I try to install it outside the folder Wineskine don't allow me to install it since it is not in the Crive.

Do you have any ideas how to figure out ?


Click to expand...

Hi Zerius, have you tried downloading the macOS version online?

This tutorial is a bit outdated since Blizzard released a new mac client. It works really well, I highly recommend it! To install it, just go to battle.net account management (make an account if you don't have one):

https://us.battle.net/account/management/

Click on "Games & Codes" -> "Download Clients", and in "Classic Games", click on the mac installer for Frozen Throne. Run it and the rest will follow suit! Make sure you have your CD keys ready since you'll need to put them in at the end! I also suggest registering them online by going to "Games & Codes" -> "Add a game key".

Let me know if you still get disconnects. Blizz has been working pretty hard to fix those types of bugs recently.

#### Zerius

#### Zerius

 Level 1

JoinedMar 1, 2018Messages2

PurgeandFire said:

 Let me know if you still get disconnects. Blizz has been working pretty hard to fix those types of bugs recently.


Click to expand...

Hi PurgeanFire,

Thanks for your reply but I have already played on wc3 with the new client but I still have lots of disconnections that is why I was looking for an alternative.

Apparently this problem appears for all mac users player so I hope they will fix it soon...

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Graveyarding this for now since they added support for the mac client. Hopefully we won't have to use this in the future!

#### Gurka

G

#### Gurka

Hi, the mac version does work for me but i really wanna use the world editor.

I'm currently using the WS9Wine2.20 engine and wrapper version Wineskin-2.6.2. I've downloaded the RoC and TFT .exe files from battle-net with the windows version.

I've followed all your steps but when i try to install RoC I get this:

Then I immediately get the installer option this again. (as you should know by now I'm a newbie at this)

Then I tired to drag the RoC file into the drive_c Manually but then i had to force quit my wrapper.

I don't understand, I thought I would get a blizzard launcher for w3 and then be able to save it to the C_drive? Am I supposed to change the install location before trying to ''Choose Setup Executable''? And then how do I do that?

Or could it be something wrong with either my wineskin or my .exe files?

Sorry for being the worst at this and thanks for the guide!

#### Attachments

image.png

 33.6 KB

 · Views: 204



#### Oridgen

#### Oridgen

 Level 4

JoinedOct 20, 2020Messages18

I'm having an issue when starting wineskin. I don't see any engines or wrapper versions available, and clicking the plus symbol does nothing. Any way around this? I tried the manual option but the site is blank.

 You must log in or register to reply here.


### Similar threads

Poll

 Configuring Warcraft III for Vista


ace992

Oct 2, 2007

Tutorial Archive
23

Replies100Views32K

Apr 29, 2010
andreasaspenberg

 Warcraft III Auto Refresh installation for battle.net


Drag0nMast3r

May 9, 2009

Tutorial Archive

Replies5Views3K

May 11, 2009
cookies4you

 WC III Icons For Windows and Linux


DesKaladA

Sep 6, 2007

Tutorial Archive

Replies10Views10K

Aug 9, 2008
hive01

Locked

 How to İnstall and connect "Eurobattle.net"


Juvia_Lockser

Jul 12, 2017

Gaias Retaliation ORPG

Replies0Views9K

Jul 12, 2017
Juvia_Lockser

 Solving "Fatal Error" Problems in WC3


Shadow_Fox

Jan 8, 2008

Tutorial Archive

Replies7Views53K

Apr 1, 2008
Kaitech[SanD]

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