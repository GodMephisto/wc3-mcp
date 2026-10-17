---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/custom-movie-tutorial-patch-1-30-1.309692/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Custom Movie Tutorial Patch 1 30 1
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/custom-movie-tutorial-patch-1-30-1.309692/>
#### Tommi Gustafsson

#### Tommi Gustafsson

 Level 19

JoinedMay 26, 2017Messages448

Resources: 6

#### Resources
Tools 1Maps 2Tutorials 3

Warcraft III has capability of playing movies similar to Blizzard's intro and ending movies. However, you need to know to what format to encode your movie that it works in the game. The video format changed with the patch 1.30, and a working support for custom movies was added in patch 1.30.1.

Warcraft III Movie Resolution and Frame Rate

By default, Warcraft III supports movies with a maximum width of 800 pixels. Therefore, a 16:9 movie will be 800x450 pixels in size. Blizzard used about 24 frames per second in their movies, but in this tutorial we will encode them with 25 frames per second. We also found that using about 25 fps for the movie helps it to play correctly with low-end computers, which might have performance problems decoding higher frame rate movies.

Preparing Your Movie

You can create your movie in any video editing software and then export it to MP4 or similar common format. It can be in any resolution and frame rate as long as it is at least 800x450 @ 25 fps. For the Gnoll Campaign intro movie, we used Final Cut Pro X and exported it to 1920x1080 @ 50 fps MP4. It was also the format we uploaded it to YouTube.

Encoding Your Movie for Warcraft III 1.30 and later

Warcraft III 1.30 uses a very specific format for movies. You also need to encode video stream and audio stream separately.

Encoding Video Stream

You need to encode the video stream into VP9 AVI using the Constant Quality (CQ) mode. You can do this using the FFMPEG command line tool. This is how you do it:

 Code:

ffmpeg -i "Your Source Movie.mp4" -c:v libvpx-vp9 -crf 30 -b:v 0 -s 800x450 -r 25 -an WC3Movie.avi

-c:v libvpx-vp9 makes FFMPEG to use the VP9 codec.

-crf 30 determines the quality. We found that 30 was quite good.

-b:v 0 must be 0 to enable the CQ mode.

-s 800x450 scales the video down to 800x450.

-r 25 set the frame rate to 25.

-an removes the audio track.

The end result is WC3Movie.avi, which is a VP9 CQ AVI file. It can have any name, but the extension must be avi.

Encoding Audio Stream

The audio stream needs to be encoded to MP3. You can do it also with FFMPEG.

 Code:

ffmpeg -i "Your Source Movie.mp4" -c:a libmp3lame -b:a 128k -vn WC3Movie.mp3

-c:a libmp3lame makes FFMPEG to encode using the MP3 codec.

-b:a 128k sets the bitrate to 128 kbps.

-vn removes the video stream.

The end result is WC3Movie.mp3. It must have the same name as the video stream file, except the extension is mp3.

Importing the Encoded Files to Warcraft III

When you have the files ready, you need to import them to Warcraft III. In campaigns, you can import them using the campaign import manager (F7). After importing, you need to change their paths to

Movies\WC3Movie.avi

Movies\Audio\WC3Movie.mp3

Creating a Movie Map to Campaign

The next step is to create a separate movie map to your campaign. You need to create some triggers and import a few files. The purpose of the files is to remove all useless user interface and make the screen look totally black. The triggers are used to do the same and finally play the movie.

You can find an example one in our Gnoll Campaign. Download it (version C1R5 or later) and open it in the World Editor using the Campaign Editor (F7). Export the CinematicIntro.w3x map or open it through the Edit Map command. You will see that the triggers look like this:

You can see here that

Game music is first stopped.

Game screen is made black with the Black Mask.

Letter Box Mode is used to remove the user interface (letter box mode graphics are removed by imports).

Custom Script "call PlayCinematic" is used to play the movie at the end of the map. In the Gnoll Campaign, the movie is named GCIntro. If you used WC3Movie, then you need to use it also here.

Wait 0.00 seconds is needed or things do not work.

Next level is set to Chapter 1.

The game is finished immediately, skipping dialogs and scores.
Now the movie will play immediately on startup and the game will automatically move to the next chapter after the movie.

You can import CinematicIntro.w3x into your campaign or create a similar one. You need to change the loading screen, though.

Playing the Movie

You can now play the movie by clicking its button in the campaign main screen.

Playback Problems

Movies may not work on Windows 7. They have been tested to work on Windows 10 and macOS High Sierra.


 Last edited: Oct 30, 2018

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Really cool to know, didn't know they had changed this with 1.30.

Approved!

#### winter twilight

#### winter twilight

 Level 3

JoinedJun 9, 2019Messages27

is it possible to change move to a lot of blp images and a mp3 sound and use the images as a filter mask then play them in turn with 0.01 wait trigger ?play mp3 file fade using "first image "in 0 second wait 0.01 second fade using "2nd image "....

#### Tommi Gustafsson

#### Tommi Gustafsson

 Level 19

JoinedMay 26, 2017Messages448

Resources: 6

#### Resources
Tools 1Maps 2Tutorials 3

This tutorial is not about showing images in-game but about playing movies before, after, or in-between chapters of a campaign, using the PlayCinematic native.

#### winter twilight

#### winter twilight

 Level 3

JoinedJun 9, 2019Messages27

Tommi Gustafsson said:

 This tutorial is not about showing images in-game but about playing movies before, after, or in-between chapters of a campaign, using the PlayCinematic native.


Click to expand...

Hi.

i don't mean to show pictures i mean to show a lot pictures instead of a real movie

picture 1 >picture 2 >picture 3 >pictures 4 > ...

00:01.01>00:01.02>00:01.03>000:01.04> ...

and the mp3 file is playing

so i think it will work as a movie

does it work ?

tx for you answer

#### Tommi Gustafsson

#### Tommi Gustafsson

 Level 19

JoinedMay 26, 2017Messages448

Resources: 6

#### Resources
Tools 1Maps 2Tutorials 3

It would be highly impractical and has nothing to do with this tutorial.

#### sebalarios

#### sebalarios

 Level 7

JoinedJul 15, 2015Messages35

Resources: 4

#### Resources
Maps 2Ported Icons 2

This work in Reforged?

#### Tommi Gustafsson

#### Tommi Gustafsson

 Level 19

JoinedMay 26, 2017Messages448

Resources: 6

#### Resources
Tools 1Maps 2Tutorials 3

I do not know, haven't tested. It is for custom campaigns, so it depends on how Quenching mod supports things.

#### sebalarios

#### sebalarios

 Level 7

JoinedJul 15, 2015Messages35

Resources: 4

#### Resources
Maps 2Ported Icons 2

This work fine in Reforged for me. btw, there is a way to put this at the beginning of a map and not in the end?

#### Tommi Gustafsson

#### Tommi Gustafsson

 Level 19

JoinedMay 26, 2017Messages448

Resources: 6

#### Resources
Tools 1Maps 2Tutorials 3

sebalarios said:

 This work fine in Reforged for me. btw, there is a way to put this at the beginning of a map and not in the end?


Click to expand...

You need to make a separate map that plays the movie before the map in question (as instructed here). There's no other way to play the movie before your map begins.

#### MapMaker14

#### MapMaker14

 Level 3

JoinedFeb 11, 2025Messages18

Hi, bro, maybe a bad question but is possible reproduce a 1920x1080 movie or is not supported?

#### Tommi Gustafsson

#### Tommi Gustafsson

 Level 19

JoinedMay 26, 2017Messages448

Resources: 6

#### Resources
Tools 1Maps 2Tutorials 3

Inmortal14 said:

 Hi, bro, maybe a bad question but is possible reproduce a 1920x1080 movie or is not supported?


Click to expand...

I think it is most probably supported in Reforged (1.32.0 and later). This tutorial was written shortly after the patch 1.30.1 came out.

#### Regno

#### Regno

 Level 9

JoinedSep 16, 2016Messages248

I wonder if one wants to display a gif or movie during gameplay, maybe one can't do this approach you mentioned for that, and probably has to use loops with custom UI constantly changing texture of the frame xD

 You must log in or register to reply here.


### Similar threads

 Custom Movie Tutorial (patch 1.29)


Tommi Gustafsson

Apr 18, 2018

Misc/General Mapping Tutorials

Replies1Views4K

Jun 4, 2018
PurgeandFire

 Cinematic Movies Tutorial


A Void

Jul 16, 2014

Misc/General Mapping Tutorials

Replies18Views17K

Apr 19, 2020
pyf

 How to: Use Custom Music in Your Map


brad.dude03

Mar 4, 2007

Misc/General Mapping Tutorials
2

Replies65Views66K

Aug 9, 2020
Daffa

Locked

 In-game movies


Gausslander

May 23, 2007

World Editor Help Zone

Replies2Views1K

May 23, 2007
Gausslander

 How to add custom background to your custom campaign


CustomCreations

Feb 6, 2023

Misc/General Mapping Tutorials

Replies4Views4K

Feb 16, 2025
MapMaker14

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