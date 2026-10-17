---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/simple-loading-screen.139768/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Simple Loading Screen
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/simple-loading-screen.139768/>
#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

#
 Simple Loading Screen

##
 By Aesthetics

 Contents


I - Required Tools and Files

II - Implementation

III - Troubleshooting

IV - Conclusion

##
 I - Required Tools and Files

Irfanview (Click "Download" on the left sidebar, click on a mirror, download Infranview, and set it up. Next, next, agree, next, next...)

Warcraft III Viewer

(Just open it with a program like winrar and run the executable later on when directed)

##
 II - Implementation

LoadingScreen.mdx

Please download the file above.

Step 1) Select your image, 800x600 or 1024x768 (You can use Infranview to resize it if it's too big). Make sure your image is not below 512 in both dimensions.

Step 2) Open the image in Infranview and click Image -> Resize/Resample

Step 3) Uncheck "Preserve Aspect Ratio" and select "Resize (Faster)". Now set the width and height of the resize to 512.

Step 4) File -> Save As... then save your image as a TGA file.

Step 5) Open Warcraft 3 Viewer and Press Ctrl V. Browse for your TGA file and save it as BLP.

Step 6) Wc3 Viewer will ask you about the Quality of Compression. Set it to an integer between 50 or 75 depending on the quality you prefer then confirm. Wc3 Viewer will probably ask you if you want to add an Alpha Channel to your image, just click Yes and close Wc3 Viewer.

Step 7) Open your map, press F12 and press CTRL I. Import LoadingScreen.mdx.

Step 8) Import your BLP file, check "Custom Path" and change the file's path to "FullScreen.blp" without the quotes.

Step 9) Finally, go to Scenario -> Map Loading Screen, check "Use Imported File" then select war3Imported\LoadingScreen.mdx from the drop-down list.

Congratulations on your loading screen! If your loading screen didn't show, please try the troubleshoot steps below.

##
 III - Troubleshooting

If you have problems with your loading screen, try these steps in order;

1. Check if you changed the path of your BLP file correctly. It should read FullScreen.blp

2. Check if you resized your image to 512x512.

3. Did you save your file as TGAthen convert it to BLP?

4. If you tried all the steps above, it's best to repeat the whole process again. If all else fails, PM me describing your issue and I'll try my best to help you.

##
 IV - Conclusion

I hoped you learned how to create a simple loading screen from this tutorial.

Credits go to SFilip for his awesome and original tutorial at:

How to: Make a Loading Screen Tutorial - Also features Hi-Definition loading screen and loading screens with bars!

Simplified by Aesthetics.

#### Attachments

LoadingScreen.mdx

 1.3 KB

 · Views: 4,961




 Last edited: Apr 12, 2010

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

1) Attach the loading screen file to the post, not the pastebin. Pastebins expire.

2) Why the heck is every step in its own [box]? Remove those.

3) Use

#
 ,

##
 ,

###
 tags

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

Pyritie said:

 1) Attach the loading screen file to the post, not the pastebin. Pastebins expire.


2) Why the heck is every step in its own [box]? Remove those.


3) Use

#
 ,

##
 ,

###
 tags

Click to expand...

#

##

###


Thanks for your time and review. I'm working on the issues you mentioned above.


EDIT: Fixed all of it. (From what I know...) If I've done anything wrong again please tell me.

#### penguinperson

#### penguinperson

 Level 2

JoinedApr 24, 2008Messages29

I like it, it's easy to read and seems to work.

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

Bump.

#### .Ragnaros

#### .Ragnaros

 Level 6

JoinedAug 13, 2009Messages197

Resources: 5

#### Resources
Icons 3Tools 1Tutorials 1

There is already a tutorial about Loading Screens

and... There's a faster way to make a loading screen with War3 Model Editor

But, after all, the tutorial is good  7/10 for his simplicity but not 10/10 because there are no images which may help us to understand...

#### Dr. Boom

#### Dr. Boom

 Level 16

JoinedMay 1, 2008Messages1,605

Resources: 4

#### Resources
Maps 1Spells 2Tutorials 1

Or just use the WEU Unlimited (only for loading screen)

Open WEU Enhancer -> Map edit -> import the picture u want have (1024x768) > and picture automatic splited and mdx created.

First try: 15 sec used

Second and more trys: 5 sec used

The easiest way I bet - but your tutorial is cool and useful for everyone 10/10

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

.Ragnaros said:

 There is already a tutorial about Loading Screens

and... There's a faster way to make a loading screen with War3 Model Editor

But, after all, the tutorial is good  7/10 for his simplicity but not 10/10 because there are no images which may help us to understand...


Click to expand...

I tried uploading images but they came out so small that you couldn't see anything. Didn't resize the images yet so I guess no screenshots for now. The tutorial you linked to makes use of the Hi-Definition loading screen which is different from my lower quality yet smaller loading screen. And finally with the War3 Model Editor, some people can't open it apparently from the posts, so I guess people could use a backup method like this tutorial.

#### .Ragnaros

#### .Ragnaros

 Level 6

JoinedAug 13, 2009Messages197

Resources: 5

#### Resources
Icons 3Tools 1Tutorials 1

So.... what you're saying is that this tutorial shows how to make a HD Loading Screen?

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

.Ragnaros said:

 So.... what you're saying is that this tutorial shows how to make a HD Loading Screen?


Click to expand...

No, this is lower quality and smaller.

But I will add the HD Loading Screen section if I have time.


 Last edited: Sep 1, 2009

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Dr. Boom said:

 Or just use the WEU Unlimited (only for loading screen)


Open WEU Enhancer -> Map edit -> import the picture u want have (1024x768) > and picture automatic splited and mdx created.


First try: 15 sec used


Second and more trys: 5 sec used


The easiest way I bet - but your tutorial is cool and useful for everyone 10/10


Click to expand...


WEU is a load of crap and shouldn't be used at all.

#### .Ragnaros

#### .Ragnaros

 Level 6

JoinedAug 13, 2009Messages197

Resources: 5

#### Resources
Icons 3Tools 1Tutorials 1

Pyritie said:

 WEU is a load of crap and shouldn't be used at all.

Click to expand...

Oh cmon, its not true, there are some useful things people can use on it but the rest is a bunch of crap ^^

Aesthetics said:

 No, this is lower quality and smaller.

But I will add thee HD Loading Screen section if I have time.

Click to expand...

Ah ok, at the moment the tutorial is repeated because there is another tutorial showing how to make a standard quality Loading Screen, but ye, after you add the explanation for the HD loading screens it'll be sightly different and better because it'll be more complex.

Edit: Forgot to tell you something about the images...

You're trying to upload your images to Imageshack for sure, well, imageshack sucks, try Photobucket, with it you can resize, write cute textes on your image, etc and... most important... it'll be shown in the original size.

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

Added screen shots.

#### .Ragnaros

#### .Ragnaros

 Level 6

JoinedAug 13, 2009Messages197

Resources: 5

#### Resources
Icons 3Tools 1Tutorials 1

Nice

New Rating: 10/10 ))

+rep

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

Thanks. +Rep to you too.

By the way, along with the HD loading screen part, I plan on adding a Video to help some people who just don't understand how to read and look at pictures.

EDIT: The tutorial you were referring to in post #12 is actually an HD loading screen, so I'm actually not repeating that tutorial.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Use

#### .Ragnaros

#### .Ragnaros

 Level 6

JoinedAug 13, 2009Messages197

Resources: 5

#### Resources
Icons 3Tools 1Tutorials 1

Aesthetics said:

 Thanks. +Rep to you too.

By the way, along with the HD loading screen part, I plan on adding a Video to help some people who just don't understand how to read and look at pictures.

EDIT: The tutorial you were referring to in post #12 is actually an HD loading screen, so I'm actually not repeating that tutorial.


Click to expand...

 You're right there's people who are too lazy to read, or they don't really understand shiz what you've wrote

Yep, indeed, video will make the tutorial better, good job.

Lol pyritie, always talking about the forum tags, yayz, you're a tag addicted!

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

Pyritie said:

 Use


Click to expand...

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

First, Edit > Go advanced. At the bottom you should see your attachments. Look at the url, there should be a number in the url near the end (but not the number right at the end). Then you just put that number between attach tags and you're done.

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

Thanks it works! Sadly, no rep because I have to spread some around first.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

alrighty then, approved

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

Small edit: Changed the "Contents" simple-table to a box.

#### Teaspoon

#### Teaspoon

 Level 14

JoinedOct 1, 2009Messages596

Resources: 14

#### Resources
Models 14

I followed your instructions but the file is not mdx so i can't put it on Loading Screen. Rep+ if u help me fix it


EDIT: Never mind I solved it.


 Last edited: Oct 6, 2009

#### T3rm1n4tor

#### T3rm1n4tor

 Level 3

JoinedAug 17, 2009Messages33

Oh thank you very much I've ever wanted to make own load screenz

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

T3rm1n4tor said:

 Oh thank you very much I've ever wanted to make own load screenz

Click to expand...

Making is a different thing from implementation. This tutorial tells you how to implement loading screens.

#### BwForka

#### BwForka

 Level 4

JoinedAug 15, 2009Messages27

Resources: 8

#### Resources
Maps 8

The type of my picture its not model its image/texture how to make it model

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

BwForka said:

 The type of my picture its not model its image/texture how to make it model

Click to expand...

Maybe the tool(s) in my tutorial will work...

#### BwForka

#### BwForka

 Level 4

JoinedAug 15, 2009Messages27

Resources: 8

#### Resources
Maps 8

Aesthetics said:

 Maybe the tool(s) in my tutorial will work...

Click to expand...


no no they dont

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

So what file extension does your picture have?

EDIT:

 Some Small Changes


 - Added anchors to table of contents

 - Added a PM link in the troubleshooting section



 Last edited: Nov 18, 2009

#### Aggrius

#### Aggrius

 Level 5

JoinedAug 11, 2009Messages95

Resources: 2

#### Resources
Maps 2

.Ragnaros said:

 There is already a tutorial about Loading Screens

and... There's a faster way to make a loading screen with War3 Model Editor

Click to expand...

I looked at that tutorial, and I like this one better! Here he tells you excactly what you should do, not just some short comments and pictures.

#### Crius

#### Crius

 Level 5

JoinedFeb 27, 2010Messages95

Resources: 1

#### Resources
Maps 1

TYVM I've been trying to get this to work without buying some high end photoshop thing for months. +rep

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

GIANT_CRAB said:

 ERRR . ps. i did everything and the custom loading screen u said did not appear out for me to click..i tried 4 times lerhs... wasted about 15 mins... i checked for 5 mins over and over again.. dammit.... pls reply to me asap


Click to expand...

Please describe your issue more clearly and with better spelling, grammar, and less of ..... or I cannot help you with your problem.

#### prplprince

#### prplprince

 Level 3

JoinedOct 31, 2010Messages32

Resources: 2

#### Resources
Maps 1Spells 1

I have to say tyvm. I probly would have never gotten a custom loadscreen

#### FriendlyHost

#### FriendlyHost

 Level 7

JoinedAug 11, 2009Messages211

Resources: 4

#### Resources
Maps 4

WOOOOOOOOOOOOOT IT WORKS!! Thanks a LOT.

#### Ogr3Blood

O

#### Ogr3Blood

Excellent tutorial THANKS ALOT!

#### Arcisal

#### Arcisal

 Level 12

JoinedJul 7, 2009Messages1,030

Resources: 2

#### Resources
Maps 1Tutorials 1

Worked well for me!


Great Tutorial!

+rep

#### Cozythrias

#### Cozythrias

 Level 3

JoinedDec 7, 2009Messages58

this has always proven to help me, thanks!

#### Aesthetics

#### Aesthetics

 Level 13

JoinedMay 30, 2009Messages828

Resources: 2

#### Resources
Tutorials 2

Cozythrias said:

 this has always proven to help me, thanks!


Click to expand...

Glad to hear it!

#### MagiC_AntO.

#### MagiC_AntO.

 Level 4

JoinedOct 11, 2010Messages97

It s Ok for me good and simple tutorial , thank you ....+Rep

#### Kunfury

#### Kunfury

 Level 6

JoinedMay 26, 2010Messages210

Resources: 1

#### Resources
Maps 1

Worked for me, tyvm

#### aitz-

#### aitz-

 Level 6

JoinedSep 24, 2015Messages174

hmm, i tried many tutos and even on this on you don't mention about not modifiying the mdx path...i was in struggle like 2 hours cause of that, anyway thanks for the tuto it helped me alot !!

#### evening_wear

E

#### evening_wear

Can someone help me with this on discord?, I'm new. I can share my screen got all the files needed and 800x600 jpg photo I resized in MS paint. Tried doing this alone didn't work.

000000000000001#9960

#### erfaned

#### erfaned

 Level 4

JoinedApr 27, 2021Messages52

Thanks for maknig this epic tutorial, I used to find it very hard to make loading screens.

 You must log in or register to reply here.


### Similar threads

 How to create a High/Low quality loading Screen


Arisato

Feb 17, 2010

Tutorial Archive

Replies16Views6K

Apr 30, 2010
PurgeandFire

X

 Custom loading screen for the In-experianced.


XxNiNjAsHaDoWxX

Dec 30, 2007

Tutorial Archive

Replies6Views4K

Feb 11, 2008
PurplePoot

T

Locked

 Loading Screen


TwAizZ

Feb 14, 2009

World Editor Help Zone

Replies3Views2K

Mar 2, 2009
Metov

Locked

 Custom Loading Screen


erdelf

Jan 17, 2012

World Editor Help Zone

Replies3Views3K

Jan 17, 2012
Maker

 Animated Loading Screens


N00byStance

Dec 27, 2005

Misc/General Mapping Tutorials

Replies1Views27K

Oct 23, 2013
Gamegear

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