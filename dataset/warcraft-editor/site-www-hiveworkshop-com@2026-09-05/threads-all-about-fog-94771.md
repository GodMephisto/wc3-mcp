---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/all-about-fog.94771/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# All About Fog
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/all-about-fog.94771/>
#### Volvox

#### Volvox

 Level 15

JoinedJan 16, 2008Messages1,244

Resources: 24

#### Resources
Icons 20Skins 1Maps 2Tutorials 1

All about fog


You are having troubles getting that fog to blend in with your terrain? No matter what you do, the colors just don't fit in? Your fog looks unnatural? This brief tutorial may help.

1. Fog options


In order to use the fog, you need to enable it first. Fog options are accesed by pressing scenario -> Map options.


This is the map options window:


As you see, the fog has been disabled in this case. In order to enable it, just thick the 'Use terrain fog'. Once enabled, you will gain access to the fog options.


First option is the fog style. The three styles available are Linear, exponential 1 and exponential 2. Exponential fog styles are very agressive and tend to give unwanted results so i would advise using the linear style.


As second comes the Z start. It is a real value determining how far from the ground the fog will start. The lesser this value is, the more fog you will get in the end.


Next is the Z end. Just like Z start, it is a real number determining how far from the ground the fog ends. The amount of fog is inversely proportional with this number.


The relation between the two numbers above is very important as the difference between them directly affects fog density in the overal outcome. The fog density is inversely proportional with the difference(as difference increases, density lowers). If Z end is lower then Z start, the result is a firm wall of fog. This wall can be useful for hiding the real sky.


Sky hidden by a dark teal wall of fog:


The same result can be acchieved by making the Z difference positive but very small, however, in that case, the wall will be softer.


Density number, as stated by it's name, affects density. As useful as it may seem, i find it way easier to adapt the fog density via the Z difference(this value has greater effect on the exponential styles).

2. Coloring


This is probably the most awkward thing to set as it has the greatest visual effect. Let's take a look at an example and try to correct it:


Bad:


The blending problem is obvious; Red background conflicts with the teal-fogged hill. The first step we can take is to set the fog color to approximately match the background. In this case, we need dark red. The dark red color is in the presets so we will not have to adjust it additionally.


Let's see the results:


Now that's better. The hill color now does well but the density seems problematic. Now that we have chosen a more intensive fog color, we get to see that the fog starts suddenly and gets intensive right away. That points us to the fact that our Z difference is too small. Let us try to spread the fog a bit. In this particular case, i will reduce the Z start from 1000 to 0(this increases the amount of fog a little) and the Z end from 3000 to 4000(this decreases the amount of fog). Now i have brought the fog closer to the camera but i have also increased the Z difference, reducing the fog density. The result is a softer field of fog for better blending:


Dough these simple steps can make your fog look better, each terrain is specific and it's fog options may require different approaches. As with everything, the key is excercise and experimenting. Good luck with your fog!

#### Uchiha Sasuke.

#### Uchiha Sasuke.

 Level 3

JoinedJun 11, 2014Messages13

Resources: 2

#### Resources
Maps 2

+Rep. Thanks for the tutorial. It helped me create a decent fog in my dungeon map.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

you are master necromancer my friend, posting to 6 years old thread, wow

#### Uchiha Sasuke.

#### Uchiha Sasuke.

 Level 3

JoinedJun 11, 2014Messages13

Resources: 2

#### Resources
Maps 2

edo494 said:

 you are master necromancer my friend, posting to 6 years old thread, wow


Click to expand...

Hey, I just felt like appreciating the person who made this tutorial; because I'm not very good at terraining so I was scratching my head on how to create a fog. And I was able to make one thanks to this tutorial.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

I dont think author cares, considering he was last online on July 2012

#### urachi

#### urachi

 Level 4

JoinedAug 7, 2010Messages74

Was hoping to see Area/Dungeon Fogs as well but somehow everyone only explains the easy map property fog... So this post is missing it.

 You must log in or register to reply here.


### Similar threads

Locked

[General]
 Setting Terain Fog to Specific Region


Abyssion

Mar 2, 2018

World Editor Help Zone

Replies11Views4K

Mar 2, 2018
Abyssion

Locked

 Exponential Fog


Wolfhelm

Mar 22, 2009

Terrain & Level Design

Replies6Views2K

Mar 23, 2009
~Void~

Locked

 How to make fog become denser smoothly?


deama

Jul 25, 2013

World Editor Help Zone

Replies4Views2K

Jul 25, 2013
deama

Locked

 Fog color set by trigger, what is the correct precentage?


Wraithwynd

Feb 17, 2009

World Editor Help Zone

Replies0Views2K

Feb 17, 2009
Wraithwynd

Locked

 Removing environment fog effect


warbucket

Feb 8, 2023

Triggers & Scripts

Replies0Views1K

Feb 8, 2023
warbucket

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