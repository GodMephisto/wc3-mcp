---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-use-custom-music-in-your-map.31249/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Use Custom Music in Your Map
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-use-custom-music-in-your-map.31249/>
#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

 How To: Use Custom Music in Your Map

By Brad.Dude03


This is a tutorial that will allow you to use a song of your choice as music in your game. This allows for greater personalization of maps, and it also helps to set the theme, for example scary music adds greatly to a map that is intended to be scary. In this tutorial I am using a song from the Fable Soundtrack, but if your following along, then obviously substitute it with the song of your choice.

 Helpful Hints For Choosing Music:

 Hints


Never, ever, choose a song that is more then 2 MB's, unless of course you are doing a campaign or single player map, in which case you need not worry about your map size. The absolute maximum size map that you can host on Battle.Net is 4 MB. That doesn't leave you with a lot of space, and music isn't that important to your game that you have to make everything else subpar so that it can fit.

Choose a song that fits with the theme of your map. If you are doing a scary map, then choose some scary music, perhaps something ominous, for example the Graveyard theme off of the Fable soundtrack, in my case. If you have an arena style map or something, when it comes time to battle, then maybe some techno or other fast-paced upbeat music that will get people into the game.

If you are making a themed map, then using music from that theme is always a good option. For example, if you were making a Lord of the Rings map and wanted to add custom music, then using a soundfile from the movies would be great idea, and it would enhance your map.

PREPARATION:


1. Firstly you have to choose your audio file, following the tips above. Not that hard. In this example I use a theme from Fable called Witchwood.


2. Once you have done such and chosen your audio file, unless it is terribly small (far below 1 MB) then you would want to compress it to make it smaller. How would you do this? By using an audio encoder.


3. Doing a simple google search, I came across Imtoo Audio Encoder. It's a good program, and a trial version can be found here: ImTOO Audio Encoder v2.0


4. Open the Rar with WinRAR, double click on the Audio Encoder.exe and install.


5. After you have installed, start the program up. Click on Add and then choose your song. You will notice it has been added to the list.


6. Next, select your encoder. Click on Lame MP3 Encoder. Next click the checkbox that says Same as input folder. This will make it easier for you to find the new version of the song after it has been encoded.


7. Press settings. At the box where it says 'Version' choose Mpeg II.5 put the bitrate down to 56kbps, this will save you a lot of space. You can go lower, but I don't recommend it, since it will REALLY warp the quality of the audio file.


8. Finally go to Quality and choose Low (q=9). Hit okay and you will be brought back to the main program.


9. Now simply hit the Encode button, hit cancel when it prompts you to buy the software, and let the program go to work.


You should notice that your new file wont be too much different in sound quality, and that it will be much smaller in size. Witchwood.wma was 2.86 MB to begin with, but after encoding it I brought it down to 1.23 MB


Now that you have your audio file ready to go, it's time to put it into the game.

 Here's how to get it into the game:

1. Pop open your World Editor. Damn, that was hard.


2. Click on Sound Editor. Once it pops open, hit Ctrl + I


3. Browse to the file that you are importing. Then hit open.


4. WE will import the sound file into the game. Scroll down and you will see it under War3Imported.


5. Now, simply, right click the file and select Use as Music.


Good, you have your file in the game, WE recognizes it as music, now all that's left is to make it play.


The last part is quite easy. Make a trigger that looks like this:

[LEFT][/LEFT]Events
Time - Elapsed game time is 0.01 seconds

[LEFT][/LEFT]Actions
Sound - Play Witchwood <gen>

If you don't know how I did this then look here:


1. Right click actions. Select new action.


2. Select the top arrow next to where it says 'All' and scroll down to you see 'Sound'. Click on that.


3. Now click where it says 'Play Sound' and scroll until you see where it says 'Play Music'. Click on that, as well.


4. You will notice that 'Music' is in red. This means that you haven't selected the music to play. Click on the red 'Music' text, and then click on the bullet above where it says Variable. Next to that you should see the name of your music file with <gen> at the end. Select all of this and hit okay. When your game starts, your music will now play in the background as a continuous soundtrack.


And that's all there is to it. Enjoy your new music!


 Last edited: Mar 4, 2007

#### operator

#### operator

 Level 15

JoinedFeb 9, 2006Messages1,598

Resources: 2

#### Resources
Maps 2

A nice tutorial


But don't you have to stop the current playing theme before playing the music ?

#### MightyBobBarker

#### MightyBobBarker

 Level 4

JoinedAug 17, 2007Messages36

Resources: 2

#### Resources
Maps 2

Compressing Audio Files

Keeping maps a small size is one of the biggest challenges you'll face when trying to add music; and filesize is something that you'll want to keep to a minimum so that downloads don't take forever and a day. This process will make files about 1/10 of the original song's filesize. Also, if it's a full song, it will still take up a large amount of map space, so try to put only one full song per map.

 To compress the music (or sound), you need 5 (FIVE) things:

 1 (ONE)-The sound file that you would like to compress

2 (TWO) - The map to which you would like to add the sound

3 (THREE)- Warcraft III World Editor (Of Course)

4 (FOUR) -Audacity Audio Editor 1.2.6+(http://audacity.sourceforge.net/)

5 (FIVE) -Lame_enc.dll(http://www.dll-files.com/dllindex/dll-files.shtml?lame_enc)

STEP ONE

 Download and install Audacity onto your computer.

STEP TWO

 Download Lame_enc.dll and put it in the same folder as Audacity.

STEP THREE

 Open up your sound file inside of Audacity by clicking on File, then Open. For this example, I'll be using the song Smoothie King by Bowling for Soup.

(Click on image to enlarge.)

STEP FOUR

 Audacity will import the file.

(Click on image to enlarge.)

 When the file is done importing, click on the settings button as shown in the picture below. Scroll over "Set Rate" and click on 96000 hz.

(Click on image to enlarge.)

STEP FIVE

 Make sure that the data rate under the button is now set to 96000 hz:

(Click on image to enlarge.)

STEP SIX

 Now, go to File and select Export as MP3...

(Click on image to enlarge.)

STEP SEVEN

 Save the file anywhere with the same name, but with a (2) at the end.

(Click on image to enlarge.)

STEP EIGHT

 Now it will ask you to locate Lame_enc.dll, click on yes.

(Click on image to enlarge.)

STEP NINE

 Select the Lame_enc.dll that you downloaded earlier and then click on open.

(Click on image to enlarge.)

STEP TEN

 As soon as you click open, it will export your modified sound file in .MP3 format.

(Click on image to enlarge.)

STEP ELEVEN

 Now, open up the file you just saved (the one with (2)) inside of Audacity. Again, set the rate to 96000 hz.

(Click on image to enlarge.)

STEP TWELVE

 Click on Export as MP3 again and this time, save it with FAST at the end, rather than (2).

(Click on image to enlarge.)

 This time it'll save the audio file automatically, you won't have to locate Lame_enc.dll again.

STEP THIRTEEN

 Exit out of Audacity and, using Warcraft III World Editor, open up the map to which you are adding the audio file.

STEP FOURTEEN

 Now, go to the sound editor (the megaphone icon) and click on Import Sound.

(Click on image to enlarge.)

STEP FIFTEEN

 Select your file that has FAST at the end of it. After it's imported, right click on it inside of the sound editor and click on "Use As Sound".

(Click on image to enlarge.)

STEP SIXTEEN

 It'll appear in the pane at the right. Right click on the sound in the right pane and click on "Edit Sound Variable".

(Click on image to enlarge.)

STEP SEVENTEEN

 In here, set the pitch to 0.20 and the volume to 100.

(Click on image to enlarge.)

STEP EIGHTEEN

 Click on OK and then make sure your sound is still highlighted. Then click on Play Sound to make sure that the sound is what you want. It will sound very compressed, but it's worth it to get the quicker download speeds.

(Click on image to enlarge.)

 If the sound is too fast, then set the pitch to a lower number, such as .15. If it's too slow, then set it to a higher number, such as .25. You can mess around with all the options until the sound is what you want.

 In my case, the final outcome was this:

 Original Size =8.02 MB

 Compressed Size =800 KB

[NOTE1: If it's a smaller sound clip, you can try speeding it up only one time to help keep it closer to the original quality. If you do this, make sure that with the World Editor's sound editor you set the pitch somewhere between .4 and .6]

 [A compressed version of Smoothie King is available for download in case you would like to put it inside of Warcraft III World Editor and practice using the sound editor before compressing a file yourself.]

View attachment 12_-_Smoothie_King FAST.mp3


 Last edited by a moderator: May 10, 2016

#### Syth

#### Syth

 Level 7

JoinedMar 6, 2007Messages219

i did what u said but when i added it 2 my map it didn't work (map size 2 big) 2005KB how can i make it smaller or sumthing and is there a way to cut out pieces of music in warcraft 3 sound editor or do i need sumthing else, like cutting out a verse of a song etc


 Last edited: Aug 31, 2007

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

2 MB's is not too big. There shouldn't be any problems hosting a map of that size.


And yes, multiple triggers are required in order to play multiple songs.


5002 mb. That's five gigabytes fool, bigger than my whole warcraft directory


5 Megabytes is too big, but 2 megabytes is not. If you're trying to use a 5 mb song, compress it or choose something else because it's obviously too big to be hosted. Trying to fit whole commercial songs from commercial artists into your map is NOT RECOMMENDED because they don't usually go well with the theme and they get downright annoying after a while. Most good maps effectively use sounds as short clips or loops.


 Last edited by a moderator: Sep 19, 2007

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Compressing Audio Files tutorial approved.


~ Threads merged.

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

Excellent, they really do both belong in the same thread


Thanks wolve!

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

So do you combine the two steps or what? Like do one first and then the other? Or...?

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

If you were to use Audacity instead of the Imtoo encoder (recommended) you would just follow the other tutorial, and then skip to mine to see how you actually get the music in game.

#### Sorccerer

#### Sorccerer

 Level 5

JoinedMay 24, 2006Messages38

Resources: 5

#### Resources
Icons 1Maps 3Tutorials 1

I have a problem, whenever i try to use my music, it gives me a message saying "not valid music file", what do i do?

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

As it stands, I do not have a fix for this problem. World Editor is very selective about which files it will take and which it rejects.


The only thing I can suggest is finding a different copy of the file.

#### Supermj

#### Supermj

 Level 12

JoinedJul 11, 2007Messages642

Resources: 7

#### Resources
Skins 2Maps 4Tutorials 1

I think you should include how to make music change as you get to different areas. if you don't, expect a lot a PMs from noobs wanting to know how to do that.

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

Well, this tutorial was published 6 months ago, and I haven't had a single PM to do with it, though I might make another tutorial detailing the use of some of the other sound triggers.

#### Supermj

#### Supermj

 Level 12

JoinedJul 11, 2007Messages642

Resources: 7

#### Resources
Skins 2Maps 4Tutorials 1

oh. Got cha. You should defiantly do that.

#### Sorccerer

#### Sorccerer

 Level 5

JoinedMay 24, 2006Messages38

Resources: 5

#### Resources
Icons 1Maps 3Tutorials 1

ok i think i found a problem. Looking at the music file, i found that it has format = unknown. While the other sounds have a format, could that be the problem? If so, how can i fix it?

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

Wait wait wait.... so it doesn't have a format? Let's take this one step at a time:


1) Does it play on your computer via Windows Media Player etc?


2) Is the format of the file .mp3 or .wav?


That's about all I can offer. If the file is not an .mp3 or not a .wav, it wont work. Any .wma's or a.ac3's, or .mp4's, any of the other formats are not compatible with the WE.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

If it's just a "file", with the default icon, then make a copy of it (backup), and then add .wav or .mp3 to it. See if that works.

Sometimes when I download a picture or something it saves it as a "file", but when I add .jpg to the end it recognizes it as a picture and changes the icon accordingly.

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

With music though it's usually hella more screwy then that, but yes, try what Pyritie suggests and see what happens.

#### Sorccerer

#### Sorccerer

 Level 5

JoinedMay 24, 2006Messages38

Resources: 5

#### Resources
Icons 1Maps 3Tutorials 1

well it does play in media player and it mp3

#### NoNZealot

#### NoNZealot

 Level 10

JoinedSep 6, 2007Messages440

Resources: 1

#### Resources
Maps 1

I got a problem. When I use brad's method, WE makes every triggger with "," instead of "." and makes my map unplayable. I mean when I click on test map it does nothing. And when I browse it on maps in WC, it shows to no player thing. :,((( Help

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

Hold on a second. When you use my method of what? Exactly which step are you directly referring to so that I can try to isolate the problem.


Where is it adding these commas? Post the triggers please to show exactly what's up.

#### NoNZealot

#### NoNZealot

 Level 10

JoinedSep 6, 2007Messages440

Resources: 1

#### Resources
Maps 1

1) Your last step. Importing to the map.


2) It does it on WAITS.


3) Makes my map unplayable and I'm unable to see forces. However, if I delete the music I imported, and save the map and reopen it, it becomes playable without music.


4) It doesn't have to do with triggers. Generally, WAIT action screws up.

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

WAITS? Sorry dude I still can't figure out what you're talking about.


I got the part that after you import the file into the map, the map becomes unplayable.


But I need more details, details that I can understand lol.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Maybe he's saying that the music file is corrupted or something.

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

I can't really tell at this point.

#### NoNZealot

#### NoNZealot

 Level 10

JoinedSep 6, 2007Messages440

Resources: 1

#### Resources
Maps 1

 Brad.dude03


details that I can understand lol.


Click to expand...

Example:


0.27 > turns into 1,0. I can't use specific waits and they get screwed up and no file is not corrupted! More detail? When I try to select my map it shows mini map but nothing else. Need more??

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

I really have no idea what you're talking about still. What the hell are waits?


Either way, try using an alternate copy of the file to see if it makes any difference.

#### NoNZealot

#### NoNZealot

 Level 10

JoinedSep 6, 2007Messages440

Resources: 1

#### Resources
Maps 1

do you know the wait ACTION in triggers?

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

Ah, but you'te telling me this wasn't a trigger problem.


Post the trigger then so I can have a look.

#### NoNZealot

#### NoNZealot

 Level 10

JoinedSep 6, 2007Messages440

Resources: 1

#### Resources
Maps 1

Heh I was trying to say it generally doesn't make anything to triggers . I'll post how it affects my triggers soon. By that sentence it means my spells but waits actually make my spells unusable. And somehow it makes my map unplayable due to no player selection screen shows up.

Actions

Wait 0.27 seconds.
 turns to

Actions

Wait 1,0 seconds.
 You get it now? It's something very strange

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

Okay, so every trigger in the game or just the one concerning your audio?

#### NoNZealot

#### NoNZealot

 Level 10

JoinedSep 6, 2007Messages440

Resources: 1

#### Resources
Maps 1

Every trigger! Thats why I am sad

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

But this doesn't happen when you follow the other tutorial?


This is out of my league i'm afraid I can't help as I have no idea why that would happen.

#### NoNZealot

#### NoNZealot

 Level 10

JoinedSep 6, 2007Messages440

Resources: 1

#### Resources
Maps 1

Well I didn't try the other tutorial because it was long, anyway I'll try and tell you the result.

#### MightyBobBarker

#### MightyBobBarker

 Level 4

JoinedAug 17, 2007Messages36

Resources: 2

#### Resources
Maps 2

@NoNZealot


I don't know if my tutorial will be much help if you're having problems with the gui. I recommend reinstalling Warcraft and hope that it fixes the problem.

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

which extension was "Smoothie King" before beginning the compression?

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

You'll have to let MightyBobBarker field that one. Though he was active here several days ago, he obviously doesn't post regularly (ten posts since August 2007). I'm assuming that it was an MP3.

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

if it was an mp3 BEFORE the compression it's f***ing OK!!

#### MightyBobBarker

#### MightyBobBarker

 Level 4

JoinedAug 17, 2007Messages36

Resources: 2

#### Resources
Maps 2

Yeah, it was an .mp3 before compression--after even too.


Also, I don't think I understand your statement. Are you saying that my guide is worthless?


If so, I'll respond as politely as I can:


The purpose of the guide was to compress (make smaller), not to convert (make a different format). An 8 mb file, such as "Smoothie King", will bring a Warcraft III map up to a size that is unhostable on battle.net. An 800kb file, such as the compressed version, will allow the map to be hosted. As well, if someone has a lot of smaller audio clips, my guide will allow them to cut the size down so that on-line download speeds can be quicker.


If that's not what you meant by the comment, than I apologize for having you read it.

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

thanks for the answer. Obviously i meant your guide is worthy!

There is a problem however Brad!! When i import the mp3 compressed files in the editor, it tells me "not valid audio file"! What's this???

EDIT: the sound works with media player, it's an mp3 but WE does not recognize it. What now?


 Last edited: Feb 2, 2008

#### iHigh

I

#### iHigh

how do i make my music play on the loading screen?

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

You make a trigger for the music, and the event is Map Initialization. This will play the music for the player as soon as they finish loading.

#### I3arf

I

#### I3arf

I get it!

Just wanted to say thx. most of my sound imports are working now!

#### jee

J

#### jee

LordPerenoldeII said:

 thanks for the answer. Obviously i meant your guide is worthy!


There is a problem however Brad!! When i import the mp3 compressed files in the editor, it tells me "not valid audio file"! What's this???


EDIT: the sound works with media player, it's an mp3 but WE does not recognize it. What now?


Click to expand...


Im having the same problem. I can import the sound file but i can't play it. It's an mp3 file (137 KB). Layer 3 56 kbps 32000 Hz.

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

As i've explained a million times, there is no solution for this. WE just rejects your audio file, so you need to find a different copy or a different file altogether.

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

i understood why WE can't import these files even if they're compressed. Map files, when grouped in a campaign, crash when opened via game when they go over 4 MB. That's why. Single maps can exceed i think.

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

No, the entire point is for the song to be compressed to something small, like 1 MB, which clearly shouldn't drive you over the 4 MB cap unless you have a hundred imports, in which case you should be making a campaign not a multiplayer map.

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

brad.dude03 said:

 No, the entire point is for the song to be compressed to something small, like 1 MB, which clearly shouldn't drive you over the 4 MB cap unless you have a hundred imports, in which case you should be making a campaign not a multiplayer map.


Click to expand...

And that's really what i meant

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

And there you go then


I guess that is indeed the only advice I can offer: Make sure that the size of your map, after importing music, does NOT exceed 4 MB's, or the map wont be playable on B.net, etc.

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

the map won't be playable from me neither if it trepass that size! xD

I already solved the problem anyway, from long time by now.

 You must log in or register to reply here.


### Similar threads

 Custom Music Lists


Tasyen

Jun 30, 2017

Misc/General Mapping Tutorials

Replies9Views12K

Apr 3, 2024
PotatoMeister

D

 Using MIDI music in Wc3


Deleted member 177737

Jun 10, 2012

Misc/General Mapping Tutorials

Replies2Views7K

Jan 7, 2013
ck5524209

 Custom Movie Tutorial (patch 1.30.1+)


Tommi Gustafsson

Oct 30, 2018

Misc/General Mapping Tutorials

Replies12Views8K

Sep 21, 2025
Regno

 Custom Movie Tutorial (patch 1.29)


Tommi Gustafsson

Apr 18, 2018

Misc/General Mapping Tutorials

Replies1Views4K

Jun 4, 2018
PurgeandFire

Locked

[General]
 How to make music playlist?


Gumberbunbder

Jan 7, 2017

World Editor Help Zone

Replies1Views2K

Jan 10, 2017
Gumberbunbder

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