---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/game-caches.6273/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Game Caches
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/game-caches.6273/>
#### Bob27

#### Bob27

 Level 24

JoinedJun 14, 2005Messages2,506

Resources: 12

#### Resources
Maps 6Tutorials 6

Introduction

This tutorial will tell you how to move units and other things from map to map. Remember that Game Caches can not be used on Battle Net, and are almost only ever used for campaigns.

Creating the Game Cache

The first thing you want to do is to create a variable for the Game Cache, go into the Variale editor and create a new Game Cache, like in the pictures bellow.

Click the "X" sign to open the variable editor, and create a new one like so:

Then when the Game Caches variable has been created use this trigger to create the Game Cache.

You can name your gamecache anything you'd like (it is "TutorialCache.w3v" in this tutorial), but do not put

any spaces in it.

Create Cache
Events
Map initialization

Conditions

Actions
Game Cache - Create a game cache from TutorialCache.w3v

Set TutorialCache = (Last created game cache)

Game Cache - Save TutorialCache

Storing Heros and Other Things

Now i'll show you how to store things using a Game Cache. The following trigger can be used to store a hero.

Store Hero
Events

Conditions

Actions
Game Cache - Store Paladin 0032 <gen> as Paladin of Heros in TutorialCache

Game Cache - Save TutorialCache

Trigger - Run Store Gold <gen> (ignoring conditions)

Remember that you will have to create all the nesecary variables in order for the triggers to work. This next trigger will show you how to store a players gold or lumber. This will require a variable, unlike storing a hero.

Store Gold
Events

Conditions

Actions
Set Gold = (Player 1 (Red) Current gold)

Game Cache - Store Gold as Gold of Recources in TutorialCache

Game Cache - Save TutorialCache

Trigger - Run Store Hero Name <gen> (ignoring conditions)

This next trigger will show you how to store a heros proper name, and one of the ways it can be used in a different map.

Store Hero Name
Events

Conditions

Actions
Set HeroName = (Proper name of Paladin 0032 <gen>)

Game Cache - Store HeroName as HeroName of Heros in TutorialCache

Game Cache - Save (Last created game cache)

Trigger - Run End Game <gen> (ignoring conditions)

This will also require a variable.

Restoring things from the Game Cache

I am now going to show you how to restore all these things in a different map. The first thing we need to do is the re-create all the variables. It makes it alot easier if the new variables in the new map are the same as in the old map. After all the required variables have been made make this trigger.

Load Game Cache
Events
Map initialization

Conditions

Actions
Game Cache - Create a game cache from TutorialCache.w3v

Set TutorialCache = (Last created game cache)

Now that the Game Cache has been re-created we can restore our heros, gold and hero names. This next trigger is used to restore the hero.

Restore Gold
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Set Gold = (Load Gold of Recources from TutorialCache)

Player - Set Player 1 (Red) Current gold to Gold

Now that our gold has been restored we can make a text message appear on the screen saying Your heros proper name is the last game was: and have the heros proper name appear. You can do this with almost anything. Heres the trigger for it.

Restore Hero Name
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Set HeroName = (Load HeroName of Heros from TutorialCache)

Game - Display to (All players) for 20.00 seconds the text: (Your heros proper name is the last game was: + (Load HeroName of Heros from TutorialCache))

Conclusion

I hope you now find Game Caches alot easier, if you would like to ask me anything just private message me or email me at [email protected]. If you need it you maybe download the demo Here.

#### Attachments

variable2tu.gif

 5 KB

 · Views: 3,101




 Last edited by a moderator: Feb 26, 2016

#### FireWave.

F

#### FireWave.

Does it work in multiplayer?

#### Hemlock

#### Hemlock

 Level 13

JoinedJun 22, 2004Messages783

Resources: 6

#### Resources
Maps 6

Fixed the trigger tags, so this tutorial looks good.

Game cache is made for single player only, multiplayer wise, you will need make a load/save code.

#### Bob27

#### Bob27

 Level 24

JoinedJun 14, 2005Messages2,506

Resources: 12

#### Resources
Maps 6Tutorials 6

Nah Game Caches can be used in b.net, its just when u exit the game or b.net (either one) the Cache is completly useless and will not work.

#### operator

#### operator

 Level 15

JoinedFeb 9, 2006Messages1,598

Resources: 2

#### Resources
Maps 2

Nice tutorial!

#### Hemlock

#### Hemlock

 Level 13

JoinedJun 22, 2004Messages783

Resources: 6

#### Resources
Maps 6


 Bob27 said:



 Nah Game Caches can be used in b.net, its just when u exit the game or b.net (either one) the Cache is completly useless and will not work.


Click to expand...

Thats why its not handy to really use in multiplayer made, making a load/save key is way better ^_^.

but like said above, nice tutorial.

#### EmilWolf4

E

#### EmilWolf4

Below the sentence :Creating the Game Cache

The first thing you want to do is to create a variable for the Game Cache, go into the Variale editor and create a new Game Cache, like in the pictures bellow. : two photos only show this

What do?

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

You did the best necro post Ive ever seen.

"03-18-2006, 03:18 PM " you cant expect the image to be still on their database 8 years after upload

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It is ok to necro in cases like these, but it is better to PM me for concerns like that.

I recovered one image from a web archive, but the other was gone long before that. I added a description that should serve the same purpose.

#### Oraien

#### Oraien

 Level 7

JoinedFeb 10, 2011Messages133

Resources: 10

#### Resources
Maps 10

I have a problem, it doesn't work. I did all exactly how it was stated here but my unit just won't load.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Post in Triggers&Scripts with your triggers. People may be able to help you more quickly in that section.

Make sure that you have something that actually runs "Store Hero"/"Store Gold"/"Store Hero Name". In the tutorial, those triggers don't have any events so they should be ran from an external trigger. Otherwise nothing ever gets saved.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

It's important to know that game cache's name shouldn't contain a space else it won't work. InitGameCache("game cache.w3v") doesn't work, InitGameCache("gamecache.w3v") works.

I wonder tho where (in which directory) are cache files got saved? Is it even visible?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

The original wc3 directory as far as I know. I am 99% certain I have seen the files there.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Quilnez said:

 It's important to know that game cache's name shouldn't contain a space else it won't work. InitGameCache("game cache.w3v") doesn't work, InitGameCache("gamecache.w3v") works.

I wonder tho where (in which directory) are cache files got saved? Is it even visible?


Click to expand...

Updated.

I assume it is in "/Save/Profile#/gamecache.w3v". I haven't checked though.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

PurgeandFire said:

 Updated.

I assume it is in "/Save/Profile#/gamecache.w3v". I haven't checked though.


Click to expand...

No, they aren't there.

I have auto save system in my map using game caches, named it with "dc_#.w3v", # is version number from one and it has reached 20 by now. Means the older versions contain flaws so they are no longer used. I want to remove them so they won't become trashes somewhere in my disk. But I can't find them even after using the search feature.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Quilnez said:

 No, they aren't there.


I have auto save system in my map using game caches, named it with "dc_#.w3v", # is version number from one and it has reached 20 by now. Means the older versions contain flaws so they are no longer used. I want to remove them so they won't become trashes somewhere in my disk. But I can't find them even after using the search feature.


Click to expand...


Sorry, it is actually within "/Save/Profile#/Campaigns.w3v" as an aggregate.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

PurgeandFire said:

 Sorry, it is actually within "/Save/Profile#/Campaigns.w3v" as an aggregate.


Click to expand...


So we can't delete a specific game cache then? Thanks for the info anyway : )

#### Hayate_ayasaki22

#### Hayate_ayasaki22

 Level 3

JoinedMay 10, 2017Messages32

Can i know where to find

Set Gold = (Player 1 (Red) Current gold)

I'm new and confused


 Last edited: Nov 8, 2017

#### Hayate_ayasaki22

#### Hayate_ayasaki22

 Level 3

JoinedMay 10, 2017Messages32

Can someone explain how to do this game cache things...Please..

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Hayate_ayasaki22 said:

 Can i know where to find

Set Gold = (Player 1 (Red) Current gold)

I'm new and confused


Click to expand...

"Gold" is also a variable (Integer)

Hayate_ayasaki22 said:

 Can someone explain how to do this game cache things...Please..


Click to expand...

Which part you don't understand?

#### Hayate_ayasaki22

#### Hayate_ayasaki22

 Level 3

JoinedMay 10, 2017Messages32

Warseeker......everything man

Warseeker said:

 "Gold" is also a variable (Integer)


Click to expand...

Integer?like just like that?and what is the action for that?

Thanks by the way

#### Hayate_ayasaki22

#### Hayate_ayasaki22

 Level 3

JoinedMay 10, 2017Messages32

Bob27 said:

 Set HeroName = (Proper name of Paladin 0032 <gen>)


Click to expand...

what is the action for this?

i cant find the proper name option

#### Hazop / Sarven

#### Hazop / Sarven

 Level 15

JoinedJul 2, 2015Messages880

Resources: 6

#### Resources
Maps 6

Whats "HeroName" Variable

#### TheLordOfChaos201

#### TheLordOfChaos201

 Level 19

JoinedJul 2, 2011Messages2,161

I don't understand the purpose of this code?

you can do the same thing within the world editor by exporting all object data.

why is this extraordinarily long version better?

ps, exporting via the world editor doesn't limit your maps to campaigns

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Err what.

You obviously do not understand what this does then.

#### TheLordOfChaos201

#### TheLordOfChaos201

 Level 19

JoinedJul 2, 2011Messages2,161

Chaosy said:

 Err what.

You obviously do not understand what this does then.


Click to expand...

you could at least explain it instead of criticising

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

When there is a tutorial that is supposed to explain?

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


TheLordOfChaos201 said:

 I don't understand the purpose of this code?

you can do the same thing within the world editor by exporting all object data.

why is this extraordinarily long version better?

ps, exporting via the world editor doesn't limit your maps to campaigns


Click to expand...

Game Caches do not transfer object data, but game data, such as hero levels, lumber amount, health of hero, etc. At the very least, game caches were used for that purpose.

Chaosy said:

 When there is a tutorial that is supposed to explain?


Click to expand...

What?

#### TheLordOfChaos201

#### TheLordOfChaos201

 Level 19

JoinedJul 2, 2011Messages2,161

Chaosy said:

 When there is a tutorial that is supposed to explain?


Click to expand...

the tutorial?

admit it, there is a reason why this out dated piece of junk is in the grave yard

it is perfectly useless and a hassle to implement

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

TheLordOfChaos201 said:

 the tutorial?

admit it, there is a reason why this out dated piece of junk is in the grave yard

it is perfectly useless and a hassle to implement


Click to expand...

No.

You just fail to understand what it does.

#### TheLordOfChaos201

#### TheLordOfChaos201

 Level 19

JoinedJul 2, 2011Messages2,161

@GhostHunter123

sorry didn't see your message...

game data like hero levels, now that explains it. much better. now I just wonder why this tool is in the grave yard.

ps... is there a tool to perfectly transfer object data from one map to another without erasing old data? (I'm taking about units and items...etc not levels and resources)

#### Trill

#### Trill

 Level 12

JoinedFeb 24, 2018Messages405

Resources: 2

#### Resources
Icons 2

So let me get this straight: Cache creates a file and stores information in it?

Can I, for instance, take a cache file from a friend's computer and load data from it on mine?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Trill said:

 So let me get this straight: Cache creates a file and stores information in it?

Can I, for instance, take a cache file from a friend's computer and load data from it on mine?


Click to expand...

yeah. I've done this with the wc3 campaign cache, at least. I haven't tried it with regular games though.

#### Trill

#### Trill

 Level 12

JoinedFeb 24, 2018Messages405

Resources: 2

#### Resources
Icons 2

PurgeandFire said:

 yeah. I've done this with the wc3 campaign cache, at least. I haven't tried it with regular games though.


Click to expand...

So what's preventing sending/receiving cache files from third-party server? That could facilitate real large-scale MMOs.

Although I guess latency would be brutal.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Trill said:

 So what's preventing sending/receiving cache files from third-party server? That could facilitate real large-scale MMOs.

Although I guess latency would be brutal.


Click to expand...

there are a couple of issues kinda preventing that: (1) gamecaches don't get stored to disk in multiplayer iirc (2) the files can be large (3) it is hard to design your map around that, especially if the cache can change at any time

Usually with MMOs you'll want to only stream the info you need. In that case, you can use a custom host-bot and send metadata that'll control sending/receiving data from your server, as shown here:

Web Requests (HTTP) from your map (and more)

And TriggerHappy did make a little mmo demo (or as I like to call it, a demmo) here:

[Role Playing Game] - Warcraft 3 MMO - Persistent Gameplay

 You must log in or register to reply here.


### Similar threads

Locked

 game cache restore hero


Danyyy

Jul 31, 2012

World Editor Help Zone

Replies6Views6K

Nov 25, 2012
ap0calypse

 Making a Basic Campaign


Bob27

Mar 21, 2006

Misc/General Mapping Tutorials
2

Replies50Views63K

Jan 29, 2017
BooRadly

Locked

[General]
 How to bind maps in campaign?


vlekje5

Aug 20, 2015

World Editor Help Zone

Replies8Views3K

Aug 21, 2015
vlekje5

Locked

[Trigger]
 game cache


greasontim

Nov 21, 2008

Triggers & Scripts

Replies3Views1K

Nov 23, 2008
greasontim

 The Dynamic Game Cache


Elf_Lord

Jun 17, 2017

Trigger (GUI) Editor Tutorials

Replies7Views4K

Jun 17, 2018
PurgeandFire

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Trigger (GUI) Editor Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top