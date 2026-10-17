---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-change-the-hero-xp-bar.7328/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Change the Hero XP Bar
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-change-the-hero-xp-bar.7328/>
#### Orc_Tamer

#### Orc_Tamer

 Level 6

JoinedJan 5, 2007Messages40

Resources: 3

#### Resources
Models 1Tutorials 2

How to Change the Hero XP Bar


by Orc_Tamer


Basic Information: This tutorial will show you and explain how to change your interface of your game by changing the XP bar for you hero. Making you map unique and very nice.

Tools Required:


-Photohop or Gimp (or any program that can save/load .tga)


-Magos' Model Editor Link


Step 1:


Open up Magos' Model Editor and go to Windows >> MPQ Browser. Then go on File >>War3.mpq. After that go on UI>Feedback>XpBar>human-bigbar-fill.blp (As shown below)


(This is what it should look like)


Step 2: Once you open that file up go on Windows > Texture Manager. Find the texture and right click it and export it to your desktop with the .tga extension. (As shown below)


Step 3: Open it up on Photoshop or Gimp (Either one) for this tutorial I'm going to use Photoshop. Select all of it and delete all the space so its all white now change it to any color you want. Now go on the layer's options and add these settings.


(This is what I ended up with)


Step 3: Save it as .tga and reopen it on Magos' Model Editor and do the same thing as Step 2 but instead of saving it as .tga save it as .blp. Now open up your Warcraft III Map so we can import it.


Step 4: Go on Import Manage (F12). Import your new xp bar and now your file. The Full Path is "war3mapImported\human-bigbar-fill.blp" and change it to "UI\Feedback\XpBar\human-bigbar-fill.blp" (Important)


(As shown below)


Step 5: Close World Editor (Save your map) and reopen it, now test your map. Your hero's xp bar should end up having a different color.


(As shown below)


That's It. Have fun making your own XP bar.


-Orc_Tamer


 Last edited by a moderator: Jun 7, 2007

#### cal823

#### cal823

 Level 5

JoinedMar 6, 2007Messages109

i know this is old, but does this just change the colour, our could you have like a pattern or an image on it?

#### brad.dude03

#### brad.dude03

 Level 36

JoinedMar 15, 2006Messages7,941

Resources: 2

#### Resources
Maps 1Tutorials 1

It's a tutorial, so I feel that he has the right to answer, because it's not incredibly old, and this is the only source of information on the subject here.


Really, I should neg rep you for trying to urge us on, when the situation doesn't require it.

#### Fulla

#### Fulla

 Level 12

JoinedAug 3, 2005Messages744

Resources: 2

#### Resources
Tutorials 2

Seemed like a perfectly fine question ?


(Im also curious of the answer).

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved

~ Thread moved to 2D Art Tutorials.

#### Palin_Twinblade

#### Palin_Twinblade

 Level 5

JoinedJun 3, 2007Messages167

I wish there would be someone who would do this as a request, I used to be fairly skilled with photoshop cs2 i used to go to forums all the time and make sigs for competition but now i don't even have the program to do something as simple as this! lol i'm not paying another $600.


Does anyone know if the purple that fills it can be changed? or is that what the blue is? then can you change the black?

#### Kalbintion

#### Kalbintion

 Level 7

JoinedFeb 19, 2005Messages136

Resources: 22

#### Resources
Icons 22

i looked at the tutorial and i noticed in the first image "human-statbar-color" & "human-statbar-hightlight" perhaps that does something with the fill? just a thought im chking into this shortly and will edit if its true or not

#### Palin_Twinblade

#### Palin_Twinblade

 Level 5

JoinedJun 3, 2007Messages167

hmm maybe you kal could try and make this for me =D.


I thought having just having not filled with exp would look black but top half would be red with jagged edges coming down like this


__________________


|vvvvvvvvvvvvvvvvv|


|_________________|


except the "V"'s wouldnt be letter just jagged lines and not all the same length and size. and when exp fills the black thats still there be turned to red. now that would be sweet. or opposite red background.

#### Kalbintion

#### Kalbintion

 Level 7

JoinedFeb 19, 2005Messages136

Resources: 22

#### Resources
Icons 22

well im testing out my rpg i have this testing in right now, gotta make a exp trigger so i can add some lol, havent finished my game mechs if i figure it out all of it ill see what i can do palin!


edit: i did something with some image that changed it to red like i wanted lol, now to figure out what image it was


edit 2: its the hightlight picture that does the filling -- and palin ill make that image asap -- bleach is on so im workn on it during commercials lol, should be a fast edit with the image i got already tho


edit 3: DONE! now just to upload it, but where? hmm attachment?


UI\Feedback\XpBar\human-statbar-highlight.blp is path btw to set it


edit 4: added another attachment, a preview

#### Attachments

human-statbar-highlight.blp

 6.5 KB

 · Views: 715



human-statbar-highlight.jpg

 5.9 KB

 · Views: 3,006




 Last edited: Jun 27, 2007

#### Unscrewed_Magic

#### Unscrewed_Magic

 Level 8

JoinedOct 8, 2005Messages409

Resources: 3

#### Resources
Maps 3

is it possible to make one that is light blue and light green ?


with the same zig zag pattern ^

#### Kalbintion

#### Kalbintion

 Level 7

JoinedFeb 19, 2005Messages136

Resources: 22

#### Resources
Icons 22

yes, it is, when i wake up ill make it for u

#### Palin_Twinblade

#### Palin_Twinblade

 Level 5

JoinedJun 3, 2007Messages167

thanks kalb! im actually working on 2 maps right now. and do you think you could make another one for me? you don't have 2 just wondering. I'd like the top to be brown and bottom black. but like it to when it fills the black turns green and not the brown. if thats possible. maybe the second image would just be same as brown one but filled with green instead of black so brown is double layered but will look the same.

#### Kalbintion

#### Kalbintion

 Level 7

JoinedFeb 19, 2005Messages136

Resources: 22

#### Resources
Icons 22

:its kal, plz, i asked u before in the sig thread too if u didnt notice --- and ye ill make it sometime soon -- the changing color on fill i dont kno if its possible or not -- ill make both of those tho, BG: Black, Spikes: Brown & BG: Green, Spikes: Brown

edit: Heres the two you asked for palin: Rename the files if u usem. or import and rename. Previews of them attached, see Attachment Thumbnails 2 and 3

human-statbar-highlightB_BlackS_Brown.blp -> human-statbar-highlight.blp

human-statbar-highlightB_BlackS_Green.blp -> human-statbar-highlight.blp

#### Attachments

human-statbar-highlight2.jpg

 8.8 KB

 · Views: 2,552



human-statbar-highlight.blp

 14 KB

 · Views: 533



human-statbar-highlightB_BlackS_Brown.jpg

 6.6 KB

 · Views: 2,479



human-statbar-highlightB_BlackS_Green.jpg

 8.7 KB

 · Views: 2,481



human-statbar-highlightB_BlackS_Brown.blp

 8.4 KB

 · Views: 703



human-statbar-highlightB_BlackS_Green.blp

 10 KB

 · Views: 713




 Last edited: Jun 29, 2007

#### Palin_Twinblade

#### Palin_Twinblade

 Level 5

JoinedJun 3, 2007Messages167

sorry kal bad memory lol and thanks love u so much in a non gay way.


but will the highlite fillw up with blue? it didn't mean a second brown for the normal pic i meant that to be the bottom that fills not blue. but otherwise its great. red looks like blood but when you change it too green it looks like ooz =D

#### Kalbintion

#### Kalbintion

 Level 7

JoinedFeb 19, 2005Messages136

Resources: 22

#### Resources
Icons 22

i dont kno if the highlight will fill up with blue, i havent figured out 100% of all of this yet, its a bit confusing on how blizz set it up. Btw that first attachment, green/blue was for the other person who had requested one. I agree with your "ooz" thing and the "blood" thing, i can see it, but what about the brown spikes? cavish? heh.

btw palin, if u wanna speak of this more go to my "Kalbintion's Request Thread" sincei ll pay attention to that more than anything else really

#### Unscrewed_Magic

#### Unscrewed_Magic

 Level 8

JoinedOct 8, 2005Messages409

Resources: 3

#### Resources
Maps 3

thanks, but why is mine dark blue and dark red in the game ?

#### Kalbintion

#### Kalbintion

 Level 7

JoinedFeb 19, 2005Messages136

Resources: 22

#### Resources
Icons 22

may be due to gamma, or its just the way the game is generating the color instead of what its ment to be

#### Unscrewed_Magic

#### Unscrewed_Magic

 Level 8

JoinedOct 8, 2005Messages409

Resources: 3

#### Resources
Maps 3

It turns the right colors on experation timers !

#### kwoxer

K

#### kwoxer

hi guys i have a little prob and kofi couldn't help me too xD


the problem:


i want to change the loadbar which means(i think) the statusbar when a game loads : UI\Glues\Loading\LoadBar\Loading-BarBorder.blp


well this one must be the right, I looked up all but only this one can be the right


how do i have to change it that it works?


mby changing the mdx too?


and plz dont describe how to make a path or something else cuz i change nearly all blp's on my way and it worked, but this one don't


help me thx

#### Kaitech[SanD]

#### Kaitech[SanD]

 Level 17

JoinedApr 11, 2007Messages1,816

Resources: 4

#### Resources
Skins 2Tutorials 2

Is the skin actually the load bar? If it isn't the loadbar, and you're trying to use a hero EXP or something like that, then that may be why.

#### kwoxer

K

#### kwoxer

mhh it must be the right, can't some1 look after and look if it is the right, would be cool if it get it to work,


if you could help me u'll get Credit in my Map is promise


to the problem: all you know the loading bar when game is loading ...


isnt the path : UI\Glues\Loading\LoadBar\Loading-BarBorder.blp ?


just today i looked again but couldn't find any other blp matching the loading bar i wanna change


and again the dont ask just try by yourself and tell me when u got it to work, u guys would rly help me, thx man

#### madcat2

#### madcat2

 Level 14

JoinedAug 25, 2005Messages1,105

Resources: 14

#### Resources
Icons 6Skins 6User Interface 1Tutorials 1

I don't think you can change the loading screen

#### Kaitech[SanD]

#### Kaitech[SanD]

 Level 17

JoinedApr 11, 2007Messages1,816

Resources: 4

#### Resources
Skins 2Tutorials 2

Oh, yeah, you can change the loading screen http://world-editor-tutorials.thehelper.net/loadscreen.php

But it's the loading bar that you can't, that's a B.Net thing.

#### kwoxer

K

#### kwoxer

ok, than i have to do other stuff


all in all thx

#### busterkomo

#### busterkomo

 Level 17

JoinedJun 17, 2007Messages1,433

Resources: 5

#### Resources
JASS 1Tutorials 4

You should also mention that this will change the look of unit expiration timers(the things on summoned units with a duration).

#### Kaitech[SanD]

#### Kaitech[SanD]

 Level 17

JoinedApr 11, 2007Messages1,816

Resources: 4

#### Resources
Skins 2Tutorials 2

That shouldn't happen... One of them is purple while the other is blue, one goes forward one goes back... But, they are in the same place...

#### kwoxer

K

#### kwoxer

y its crazy^^


i changed bar too looks nice BUT:


also die bar i changed of casted units, i mean time bar thats going down to zero


why i changed too?

#### pyropk95

#### pyropk95

 Level 9

JoinedFeb 5, 2008Messages383

Resources: 5

#### Resources
Skins 1Maps 4

it would be splendid to tell us how to do it on gimp?~?!?!

#### t3h b055

#### t3h b055

 Level 5

JoinedMar 16, 2008Messages104

Resources: 1

#### Resources
Maps 1

thanks for the help! with the exp bars!

#### BloodStorm2262

#### BloodStorm2262

 Level 8

JoinedMar 18, 2005Messages168

Resources: 3

#### Resources
Skins 1Maps 2

I wanted to mention too that you may simply change the file with World Edit without importing a new file.


 By going to:


Advanced >


Game Interface >


Image - Console - Simple Experience Bar


 And then changing the art file to whatever image you want.


Like:


ReplaceableTextures\CommandButtons\BTNCriticalStrike.blp


You can actually do this with any and all bars in the game, like...


Image - Console - Simple Experience Bar


Image - Console - Simple Hit Point Bar


Image - Console - Simple Hit Point Bar (Small)


Image - Console - Simple Mana Bar


Image - Console - Simple Mana Bar (Small)


Image - Console - Simple Progress Bar


-Footnote-


 If someone can find the art file of the loading screen bar, I'm quite sure I changed that before as well. Hope this helps save space for people.

#### Attachments

Hero XP Bar [Critical Strike Image].jpg

 164.7 KB

 · Views: 1,357




 Last edited: Oct 25, 2008

 You must log in or register to reply here.


### Similar threads

A

Locked

 Skinning a Hero's XP Bar


aKeio

Aug 23, 2006

Texturing & Surfacing

Replies4Views2K

Jan 29, 2007
Dacav man

Locked

 Need help with xp bar


Googlieeyes

Dec 24, 2009

World Editor Help Zone

Replies2Views1K

Dec 24, 2009
Googlieeyes

Locked

 XpBar Coloring


WaterKnight

Sep 28, 2013

World Editor Help Zone

Replies5Views2K

Sep 28, 2013
WaterKnight

[Mapping]
 How to change loading bar text


light bolt30

Dec 20, 2010

Tutorial Archive

Replies3Views2K

Dec 22, 2010
darkzxx

 Mini Guide on How to: Alpha Texture


Ribenamania

Jun 27, 2009

2D Art Tutorials

Replies10Views7K

Dec 19, 2009
real138

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

2D Art Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top