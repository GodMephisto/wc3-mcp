---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/vjass-advanced-initialization.247590/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# vJASS Advanced Initialization
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/vjass-advanced-initialization.247590/>
#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

vJass: Advanced Initialization

Table of Contents

Introduction

Main and Config
Main

Config

Manual Initialization

Notes

Introduction
This tutorial will teach you how to control the order and manner in which code is initialized.

Generally vJass initializers are ran first, then custom triggers are ran after. However you can't pick the order by default.

I will show you how this is changeable and how you can also avoid relatively slow ExecuteFunc calls.

JassHelper (or JNGP) is required.

​
Main and Config
Every map has two core functions it runs on initialization (main and config).

Inside these functions contains a bunch of generated code that initializes things in your map like custom triggers (InitTrig_Function).

Typically in vJass you initialize functions by using theinitializerkeyword or by a structs onInit method.

Insidefunction mainis where JassHelper handles custom initializers by placing anExecuteFunccall for each one. For example

 JASS:

library Lib initializer onInit
 private function onInit takes nothing returns nothing
 endfunction
endlibrary

// adds this to your scripts main function

function main takes nothing returns nothing
 //..
 call ExecuteFunc("Lib___onInit")
endfunction

Now, there's nothing wrong with this however maybe you want more control over how things are handled. Not to mention ExecuteFunc calls are significantly slower than normal function calls (*).

To do this we use the very convenient//! injectfeature which allows you to modify the config and main functions of your maps script. You can read more about the inject command in the JassHelper Manual.

Before we do anything, lets go over each function.

Main

This function is ran after the config function.

If you don't place dovjassinit inside the inject block then JassHelper will throw an error.

Responsible for calling functions that create the editors preplaced objects in your map (units, cameras, regions).

 JASS:

//! inject main

 /*
 * dovjassinit places all vJass initializers.
 * This way you can control what's initialized before or after them.
 */
 //! dovjassinit

 // These initialize preplaced objects in the editor
 call CreateRegions()
 call CreateCameras()
 call CreateAllItems()
 call CreateAllUnits()

 // call InitTrig's
 call InitCustomTriggers()
//! endinject

Config

This function is ran before the main function.

Does not allow dovjassinit (will throw error).

Requires SetPlayersandInitCustomPlayerSlotsfunction calls otherwise your map won't load. Normally the first argument of SetPlayers is the amount of players in the map, however it seems that any number works.

Responsible for defining start locations. If the function is overwritten and noDefineStartLocationcalls are made, thenGetPlayerStartLocationwill return null.

 JASS:

//! inject config
 call SetPlayers(0)

 call DefineStartLocation(0, 704.0, - 832.0)

 call InitCustomPlayerSlots()
//! endinject

Manual Initialization
We are going to modify "main", not "config" because injecting config doesn't allow dovjassinit.

For the sake of example, lets say you have a struct

 JASS:

struct Data
 static method customInit takes nothing returns nothing
 endmethod
endstruct

and you want it initialized before everything else. We can do so like this:

 JASS:

//! inject main
 call Data.customInit()
 //! dovjassinit
//! endinject

This proves useful for code optimization (faster execution time) and management.

If the function is scoped you can simply use the compilation name. You're also likely going to need to call the World Editor generated functions.

 JASS:

library Lib
 private function onInit takes nothing returns nothing
 endfunction
 public function Init takes nothing returns nothing
 endfunction
endlibrary

//! inject main
 call Lib___onInit() // ___ private
 call Lib_Init() // _ public

 //! dovjassinit

 // These initialize preplaced objects in the editor
 call CreateRegions()
 call CreateCameras()
 call CreateAllItems()
 call CreateAllUnits()

 // call (non-vjass) custom triggers initialization functions
 call InitCustomTriggers()
//! endinject

There, you have full control over initialization.

​

Notes
JassHelper likely uses ExecuteFunc to avoid the thread crashing (OP Limit) or to be able to call functions from anywhere in the script.

There can only be one inject main/config block per map, so don't include it in public resources. It's meant to allow the map maker to have more control, not give resources initialization priority.

If you'd like you can use the config block instead of main for whatever reason.

If you're hitting the OP limit with your custom initialization you could split them up across each function.


 Last edited: Jan 29, 2014

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

wow, this is nice to know...

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Nice share TriggerHappy! This can prove VERY useful to vjasser.

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

Thanks for the feedback.

I've reformatted and updated the tutorial with much more information.

#### Binsu

#### Binsu

 Level 30

JoinedNov 29, 2012Messages6,631

Resources: 5

#### Resources
Maps 4Templates 1

Not much of a triggerer or even coder but this tutorial is quite neat and clear on its points. This makes me want to learn more in triggers and possible coding.

Great job.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

there's one thing in my mind, did this trick will force the initialization to run our codes first before the actual initialization?

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Do not combine inits of different features in the same thread. Splitting them up is entirely the point to render them independent. Not only because of the operation limit but there are other termination reasons. Else you will change one thing in the future and wonder why something else stops working. Yes, you can write custom initializers and a tutorial could show how to encapsulate the function calls in order to check what has been completed/failed to do so for example.

#### DysfunctionaI

#### DysfunctionaI

 Level 7

JoinedMar 6, 2006Messages282

Resources: 1

#### Resources
Tutorials 1

Whoa, you can use this to play sounds when a player joins a custom game lobby.

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

DysfunctionaI said:

 Whoa, you can use this to play sounds when a player joins a custom game lobby.


Click to expand...


I wonder if there's any other tricks ..

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

This is pretty neat if you want good control over your initialization order (useful when you have two triggers registering the same event but you want to make sure one fires before the other). I never really looked into //! inject much, so thanks for the explanations.  Approved.

#### Pinzu

#### Pinzu

 Level 15

JoinedNov 30, 2007Messages1,202

Resources: 7

#### Resources
Spells and Systems 5JASS 1Tutorials 1

DysfunctionaI said:

 Whoa, you can use this to play sounds when a player joins a custom game lobby.


Click to expand...

I demand to know how to play Storm Earth and Fire in lobby!

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Pinzu said:

 I demand to know how to play Storm Earth and Fire in lobby!


Click to expand...

 JASS:

//! inject config
 call SetPlayers(1)

 call DefineStartLocation(0, 704.0, - 832.0)

 call InitCustomPlayerSlots()

 call PlayMusic("Sound\\Music\\mp3Music\\PH1.mp3")
//! endinject

Iirc, this can crash sometimes though.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Wow. I should use this.

+rep

#### BloodDrunk

#### BloodDrunk

 Level 18

JoinedMay 11, 2012Messages2,108

Resources: 1

#### Resources
Maps 1

I myself have no idea how to manipulate this trick(s).

But as Dysfunctional mentioned, I eould like to know how can I play music when the game is created (e.g. In lobby) and during loading time. Maybe play a sound when player joins?

Is this possible?

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

https://www.hiveworkshop.com/threads/vjass-advanced-initialization.247590/post-2682339 its not even that far

#### Svynct

#### Svynct

 Level 3

JoinedNov 18, 2013Messages21

Resources: 1

#### Resources
Maps 1

WaterKnight said:

 JASS:

//! inject config
 call SetPlayers(1)

 call DefineStartLocation(0, 704.0, - 832.0)

 call InitCustomPlayerSlots()

 call PlayMusic("Sound\\Music\\mp3Music\\PH1.mp3")
//! endinject

Iirc, this can crash sometimes though.


Click to expand...

I'm trying to play music in the lobby and it is working, but the problem is: for some reason, it is removing one force from the lobby. My map has 2 forces, and all the players from the second force are moved to the first force and thus the second force is removed. Why is this happening? How can I fix it?

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

The config is also controlling the player setup, in conjunction with the war3map.w3i that is. So just copying the above snippet would overwrite it. Take a look at the war3map.j script output that the WE generates without the custom injection, then take that version of the config (with dependencies ofc) and add the sound line.

#### Svynct

#### Svynct

 Level 3

JoinedNov 18, 2013Messages21

Resources: 1

#### Resources
Maps 1

WaterKnight said:

 The config is also controlling the player setup, in conjunction with the war3map.w3i that is. So just copying the above snippet would overwrite it. Take a look at the war3map.j script output that the WE generates without the custom injection, then take that version of the config (with dependencies ofc) and add the sound line.


Click to expand...

Worked. Thanks!

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

Apparently this doesn't work with cyrillic alphabet, with latest patch and local files enabled ofcourse.

Not that I have these issues, but I reported them from a friend.

Edit: I just realised I posted on the wrong thread... wtf. I meant to post in "Codeless Save and Load" by TriggerHappy lol


 Last edited: Apr 20, 2021

#### doblefilo

#### doblefilo

 Level 2

JoinedMay 30, 2021Messages6

hello, i am trying also to play custom music on lobby, and been experimentating for a few hours so far i found out that with:

call PlayMusic(preexisting war3 theme) it plays that theme on repeat

call PlayMusic("replaced preexisting war3 theme") plays the replaced war3 theme once

call PlayMusic("war3mapImported\\example1.mp3") plays example1.mp3 once (wich is an imported mp3)

call PlayMusic("war3mapImported\\example1.mp3;war3mapImported\\example1.mp") plays example1.mp3 once

call PlayMusic("war3mapImported\\example1.mp3;war3mapImported\\example2.mp") plays a random example once

is there a way to play an imported theme on repeat? or the only way would be to use a single and looped mp3 of what i wanna use? (wich would make the mp3 file a lot bigger and thats what i want to avoid)

edit: all of this is done / tested in patch 1.27b


 Last edited: Dec 16, 2022

#### Kakerate

#### Kakerate

 Level 14

JoinedOct 18, 2013Messages760

doblefilo said:

 hello, i am trying also to play custom music on lobby, and been experimentating for a few hours so far i found out that with:

call PlayMusic(preexisting war3 theme) it plays that theme on repeat

call PlayMusic("replaced preexisting war3 theme") plays the replaced war3 theme once

call PlayMusic("war3mapImported\\example1.mp3") plays example1.mp3 once (wich is an imported mp3)

call PlayMusic("war3mapImported\\example1.mp3;war3mapImported\\example1.mp") plays example1.mp3 once

call PlayMusic("war3mapImported\\example1.mp3;war3mapImported\\example2.mp") plays a random example once

is there a way to play an imported theme on repeat? or the only way would be to use a single and looped mp3 of what i wanna use? (wich would make the mp3 file a lot bigger and thats what i want to avoid)

edit: all of this is done / tested in patch 1.27b


Click to expand...

did you ever find a fix? I wonder if changing the filepath of the imported music to one of the filepaths for default music could work.

#### doblefilo

#### doblefilo

 Level 2

JoinedMay 30, 2021Messages6

Kakerate said:

 did you ever find a fix? I wonder if changing the filepath of the imported music to one of the filepaths for default music could work.


Click to expand...

yes i did, with the help of some other modders we found that the music file must not be over 2mb or 10mb was it? if its bigger than that limit, the music will only play once, if smaller it will repeat and nope, what you suggested didnt work, i was so happy once the solution was found! <3

#### Kakerate

#### Kakerate

 Level 14

JoinedOct 18, 2013Messages760

so strange! Ingame I don't think size matters. Thanks for the reply

#### DarknessFalls003

#### DarknessFalls003

 Level 13

JoinedMar 24, 2011Messages121

Resources: 23

#### Resources
Models 4Maps 18Ported Icons 1

CUSTOM LOBBY MUSIC TUTORIAL

>Save you map as "scenario folder"

In it, open the war3map.j file with notepad.

/find "config"

copy everything that starts with "call"

Click to expand...

in trigger editor, create a custom script

type:

Click to expand...

//! inject config

//! endinject

>between the 2 lines, paste what you copied from the notepad

in the last line, type:

Click to expand...

 call PlaySound("yourfile.mp3")

>replace "yourfile" with your music file name


 Last edited: Jun 1, 2025

 You must log in or register to reply here.


### Similar threads

Locked

 all your private base are belong to us (getting around vJass's privacy)


Aniki

Sep 3, 2016

Advanced Scripting

Replies6Views3K

Sep 7, 2016
WaterKnight

 Vanilla Jass Header Initialization


Almia

Aug 27, 2013

JASS Tutorials

Replies8Views6K

Aug 31, 2013
Rheiko

Locked

 Bridging GUI with vJass - Initialization Issues


Bribe

Nov 15, 2018

Advanced Scripting

Replies6Views3K

Nov 22, 2018
Bribe

[Lua]
 Inject main/config from WE trigger code (like JassHelper)


Luashine

Feb 15, 2022

Old Submissions

Replies7Views2K

Nov 6, 2022
Bribe

Locked

[vJASS]
 Detect auto cast orders of preplaced units with vJass


Barade

Aug 20, 2022

Triggers & Scripts

Replies0Views795

Aug 20, 2022
Barade

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

JASS Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top