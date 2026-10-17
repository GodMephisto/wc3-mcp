---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/wc3-networking-crucial-component-of-codeless-save-load.304287/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# WC3 Networking Crucial Component of Codeless Save Load
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/wc3-networking-crucial-component-of-codeless-save-load.304287/>
#### Lizreu

#### Lizreu

 Level 19

JoinedJun 13, 2016Messages588

Resources: 2

#### Resources
Tools 1Tutorials 1

Preface

A while ago, I worked on a Network / FileIO library for Wurst, a duo of systems to facilitate easy save/load in WC3.

The only decent resources on the subject matter, which I used as a reference for my systems, are two systems by TriggerHappy - Sync, and SyncInteger. I've (somewhat partially) reimplemented them in Wurst, with various additions and adjustments.

During the implementation period, I learned a lot about networking in WC3, as well as some useful tricks. They aren't really documented anywhere, except in libraries, so I would like to share my newfound insight into WC3's internals on this subject with the community.

What is this tutorial about?

This tutorial is going to cover various advanced topics related to networking in WC3.

For those who are unfamiliar with the idea, in a very abstract sense, networking refers to the act of sending something that only one client knows about, to the other clients.

Most mappers don't ever need to think once about networking in WC3, because it is handled behind the scenes automatically by the engine, without the mapper having to intervene. This makes it very easy to just write your game logic - but not very easy when you actually have to send something from one client to another - such as the contents of a file that only one client has, which is a necessity if you want to use a codeless (i.e. file-based) save/load system.

If you would like some code examples to read along with this tutorial, you can head over to Wurst's standard library (wurstscript/WurstStdlib2) and look in the following files, and their dependencies:

network/SyncSimple.wurst

network/Network.wurst
Everything you see in these files is based on the theory and tricks that I am about to explain.

Without further ado, let's get into it.

How does WC3's networking model work?

Unlike virtually all FPS (and even some RTS) games, WC3 is built using something called deterministic lockstep.

In deterministic lockstep, every player in a game has all the data about the game world, even the data the player maybe shouldn't know about. This includes units in the fog of war, other players' selections, the orders they are sending, and sometimes, even, the messages they send (even if it is ally or observer only).

The only data that gets sent over the network are user inputs, such as unit selections, unit orders, chat messages, some button presses, and so on.

Each player then simulates the game world in parallel to other users, on his machine, using the current state of the world and the user inputs he received from other players. This is where the deterministic part becomes really important - given the same starting state, and the same inputs, every player is going to arrive at the same conclusion about the simulation.

At least, usually. Sometimes, due to a bug in WC3 or a logic error on the mapmaker's part, a desync will occur, where some players arrive at one result, and others at another. This makes the simulations diverge, and players will start to see different results on their screens. At this point, they are no longer playing the same game. Usually, WC3 catches that and disconnects the player who desynced, but in extremely rare and bizzare cases, the game may continue to go on in multiple desynced states, creating a lot of confusion for unaware players.

If this sounds too abstract or confusing, consider the following real-world analogy as a simple explanation:

Imagine you're playing a game of chess with a friend who lives far away, by mail. Each of you starts with the same board configuration, and after that, you mail your moves to your friend, and vice-versa. When he receives your mail, he applies your move, makes his own move, and then mails that move to you. When you receive his mail, you do the same. And so on, and so on. Since chess is a deterministic game, meaning there is no random element, this is all you need to have a synchronized board (i.e. game state), provided that you execute each others' moves correctly.

But, suppose you make one move, and accidentally make a mistake, and write it down as a different move when mailing your friend. He now thinks your board is a different configuration than the one you really have. This is analagous to the dreaded desync, and it is going to get worse with time, and may be hard to catch.

WC3's networking slightly in-depth

Obviously, WC3 is much more complicated than a chess board. There are potentially hundreds of units, each with a dozen of different parameters, all at once, doing different things. As mentioned previously, only the user inputs get networked, and everything else is simulated in parallel.

In WC3, the host (in past times, a player, now, usually, a bot hosted by someone else) serves as a relay between the players. Each player communicates only with the host, sending their actions to them, and receiving other players' actions from them.

There's one important takeaway here: the clients do not simulate their own actions until they receive a confirmation from the host. This is necessary in order to maintain synchronization, so that each client simulates every event at the same game tick. In addition, the host also waits for some time before relaying your order to the other players, in order to allow for other players with higher ping to catch up. This is usually called latency, and it is introduced in order to have a smoother game for everyone, even if at the expense of some extra delay.

One other important detail is that WC3 is built on top of the TCP protocol, which is sequential. This means that whatever you send through TCP is going to arrive in the same order that you sent it in. This is likewise very important, because it means that all your inputs are going to be processed by other players in the same order. This is also important, because this allows us to reliably send data from one player to the rest. More on that later.

Using WC3 networking to our advantage

That was a lot of theory, but it isn't going to be terribly useful to most mappers, since all of this is automagically handled by the engine behind the scenes. The mapper doesn't have to think much about what goes on the wire, and what doesn't. Unless...

Unless we need to send some data that isn't automatically networked. There aren't many things in WC3 like that. It pretty much boils down to:

 Obscure local data that isn't networked, e.g. camera position / parameters, terrain height, and some other things.

 File contents.

Fortunately for us, there are natives in JASS that allow us to send arbitrary data from one player to the rest. These are the GameCache Sync natives, namely SyncStoredInteger, SyncStoredReal and SyncStoredBoolean. There is also SyncStoredString, but unfortunately, it is broken and doesn't work.

When called locally, i.e. from within a Local Player clause (e.g. inside an if client == GetLocalPlayer() statement), these synchronize the current stored value at the specified index to the other players. Using these three natives, we can send integers, reals, and booleans to other players.

Cool! We can send stuff. That means that, theoretically, we can load a file from one of the players, break it down into integers, reals, and booleans (which is outside the scope of this tutorial), and send it to other players.

There is just one problem, however. There is no event that fires when a GameCache sync has finished, and when the other clients have received the data. If only we could somehow generate our own event...

Wait, remember when I said that every user input in WC3 is sequential? Turns out, that GameCache syncs are no different! They go through the same system that all other user inputs do - unit orders, key presses, chat messages, and so on. Consider the following code snippet:

 JASS:

if sender == GetLocalPlayer() then
 call SyncStoredInteger(...)
 call SelectUnit(...)
endif

What's going to happen here, is that the sender player is going to first send the stored integer, and then select the unit. By the time the unit selection event is fired, all previous network events will have been acknowledged by other players, including the synced integer. This allows us to "catch" the moment when a sync has finished, with an event, and without risking desyncs!

In fact, using this SelectUnit trick we can build a simple wrapper that allows us to synchronize some code between players, making an event that only fires after all previous pending network actions have been processed! In fact, this is exactly what I did in my Wurst library. You can look in network/SyncSimple.wurst to see exactly how it can be done.

At that point, all you need is some scaffolding to wrap it all up into a system of some kind. Previously, only TriggerHappy had dared try and do so, in his Sync and SyncInteger libraries. I personally found the event-driven vJASS API rather unwieldy to use, so I never bothered, even though his libraries have proven to be an immense aid in writing my own. However, since Wurst provides lambdas and many other cool language features, it was possible to make a much more user-friendly API in Wurst. The actual library is rather boring, and mostly consists of various tricks and scaffolding to support arbitrary amount of data and string (de-)serialization. If you're curious, you can skim through the source code, it's pretty heavily documented. Here's a few exerpts, going slightly more in-depth:

SyncSimple.wurst:

This library can be used to send a 'synchronize' network event that will fire

for all players after all previous network events have also been received

by all players.

Examples of such events include:

 - Unit selection

 - Unit orders

 - Chat messages

 - Gamecache synchronization

 - Keyboard events

For example, this can be used in conjunction with gamecache synchronization

to send 'local' values from one player to the rest, such as camera position,

data from files, and so on. For a package implementing this functionality,

see Network.

It depends on the fact that all network events in WC3 are fired sequentially,

meaning that they arrrive for other players in the order that they were

sent in.

It also depends on the fact that the EVENT_PLAYER_UNIT_SELECTED fires

synchronously for all players at the same time, allowing us to know for

certain when other players have acknowledged our unit selection, as well

as all network events that have been fired before it.

By calling the .sync() method, we queue a network action (specifically,

a unit selection event) that will only be delivered after all previously

queued network actions have also been delivered.

The primary use of this library is in conjunction with gamecache's Sync

natives, because they also fire sequential network events. When we call

.sync() after a series of Sync natives, we ensure that the .onSynced()

callback will only be called after all players have received the data.

This way, we can send local data from one player to the rest, such as

camera position, data from files, and so on.

There may be other usages related to async network events as well.

Network.wurst:

Overview:

In multiplayer games, this package is for synchronizing data between game clients.

It's useful for when one player is the source of game data, such as from gamecache or file IO.

Like SyncSimple, it depends on the fact that all network actions in WC3 are

sequential, and are received by players in the same order that are sent in.

It uses the SyncStored* natives of the gamecache to send data to other players

from the sender, and then uses SyncSimple to send a 'finish' event, that will

only be received by other players after they have also received all sync data.

The Network class provides 4 independent buffers of data:

 integers,

 reals,

 booleans,

 strings

each of which can be written/read to using the relevant

methods of the HashBuffer class.

Before sending the data, the sender populates the HashBuffer with the data that

they want to send, then Network.start() is called, and data is received from

the same buffer inside the callback when it has all been transferred.

Read SyncSimple docs for a slightly more in-depth overview of this particular

peculiarity of WC3.

Implementation overview:

Reference for various claims about performance and operation can be found here:

Sync Doc | HIVE

Original research by TriggerHappy.

There are several core classes here:

 HashBuffer

 GamecacheBuffer

 StringEncoder

 GamecacheKeys

 Network

HashBuffer is a hashtable-based container with buffer semantics for writing

integers, reals, booleans, strings and serializables.

Longer gamecache keys take a longer time to synchronize (for each value synced,

we also send it's keys), so we use keys of fixed length and send data in multiple

rounds. Because of this, we can't store all data immediately in the gamecache.

We also need to know the size of data being sent prior to starting the transmission,

so we have to store all of it in an intermediate buffer, which is HashBuffer.

Prior to sending, all strings in the HashBuffer are encoded into a buffer

of integers, because SyncStoredString doesn't work. The responsible class is StringEncoder.

After sending, they are decoded back into strings and written to the HashBuffer.

GamecacheKeys provides int-string conversion for keys for usage in gamecaches.

GamecacheBuffer is a gamecache-based container for writing

integers, reals and booleans. There is a GamecacheBuffer for each primitive type,

int, bool, real and asciiInts.

Network is the main class that coordinates HashBuffer and GamecacheBuffer and does

all the heavy lifting.

Before starting the transmission, the HashBuffer is locked into an immutable state, in

which incorrect mutation attempts will print warnings, as long as safety checks are not disabled.

The maximum amount of data across all primitive buffers is calculated, and the amount of

required 'sync rounds' is calculated - that is, the amount of times we need to flush/sync

data out of the gamecaches to keep key sizes short.

Since only the local player has any knowledge about the amount of data needed to be sent,

and consequently, the amount of sync rounds required, we first send a pre-fetch "metadata"

payload with the amount of data in each buffer and the amount of sync rounds, using fixed

keys. At the same time, we also send the first payload.

When a round is received, we write data to the HashBuffer, using the metadata to know when

to stop, and start another sync round if necessary. If it is not necessary,

we open the HashBuffer for reading and call the finish callback, and destroy the instance.

Epilogue

Obviously, I haven't covered every single topic relating to WC3 networking. This tutorial was meant to provide an important theoretical base for writing your own networking systems, for whichever purpose they may be used, and to spread the knowledge in the community. I haven't even scratched the surface of desyncs, as it can get really complicated with those.

With that said, there are already libraries implementing this functionality both in vJASS and Wurst, but I felt like it was worthwhile taking a behind-the-scenes look at this very obscure topic.

In the future, I might write a tutorial about File IO in WC3 and fully-fledged codeless Save/Load systems (which Wurst also supports, in the form of Persistable, combining File IO with Networking and various tips. Please tell me if you'd like a tutorial for that!

And, of course, if you have any suggestion, questions, or corrections for this tutorial - please leave a comment as well.


 Last edited: Mar 19, 2018

#### MyPad

#### MyPad

 Level 31

JoinedMay 9, 2014Messages1,878

Resources: 27

#### Resources
Models 1Icons 4Maps 6Spells and Systems 7JASS 7Lua 1Tutorials 1

Reading the tutorial, I could see from the way it was written that it was driven with the passion for informing others. Although I already understood some parts beforehand, the syncing mechanism is what caused me to falter in my understanding of the synchronization process. I puzzled over the nature of gamecaches and syncing, wondering if the data held by one player is carried onto multiplayer. This puzzling notion is what stopped me from creating my Clash-Royale inspired map, since I would have to read and write files for the data of the player.

If I was to go about with the overall semantics and presentation style, I would say that it is quite alright, just a lukewarm rating, and I think the presentation can be improved, but the thing about it being written with passion is mostly about content. By explaining to the community what one thing does, and giving a real-world example that most closely aligns with the topic, you open up the thought process of whomsoever reads this article/tutorial. I would suggest adding a Table of Contents for the tutorial.

With that being said, I appreciate it if you are to write a tutorial about File IO, and enlighten the community of hivers, which I would presume to be growing once again.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Awesome tutorial. Definitely informative, and I really like the chess analogy.

And thanks for explaining the SelectUnit(...) trick; I had always seen it mentioned throughout the years but never knew what it was actually doing. Your explanation makes perfect sense.

I think the tutorial is perfect as it is. It would definitely be cool to see some of the stuff MyPad mentioned, but I would suggest putting them in a separate tutorial. I made a fix to one typo (conents -> contents) and added a list tag + a JASS tag.

A few parting questions/comments:

 In WC3, the host (in past times, a player, now, usually, a bot hosted by someone else) serves as a relay between the players. Each player communicates only with the host, sending their actions to them, and receiving other players' actions from them.


Click to expand...

Is there any advantage to the host being the one who does the initial broadcast? I was reading this thread a while back, but wasn't sure if I was interpreting his words correctly:

[code=jass] - A really really ridiculously easy way to sync data / new GetHost command

i.e. i know that if player 5 wants to sync an action, he has to go through the host and then it gets broadcast, but if the host himself is starting the broadcast, can it skip one packet roundtrip?

File IO tutorial would be great!

It is pretty amazing how much wc3 does behind the scenes. It is honestly kinda scary when you think about how we're all just simulating each other's actions--we aren't really in a world with other people.

Does this cause the selection to jitter whenever a sync happens? i.e. if I have a group of units selected, will I be able to see the UI switch to selecting one unit and then back?

#### Lizreu

#### Lizreu

 Level 19

JoinedJun 13, 2016Messages588

Resources: 2

#### Resources
Tools 1Tutorials 1

MyPad said:

 Reading the tutorial, I could see from the way it was written that it was driven with the passion for informing others. Although I already understood some parts beforehand, the syncing mechanism is what caused me to falter in my understanding of the synchronization process. I puzzled over the nature of gamecaches and syncing, wondering if the data held by one player is carried onto multiplayer. This puzzling notion is what stopped me from creating my Clash-Royale inspired map, since I would have to read and write files for the data of the player.

If I was to go about with the overall semantics and presentation style, I would say that it is quite alright, just a lukewarm rating, and I think the presentation can be improved, but the thing about it being written with passion is mostly about content. By explaining to the community what one thing does, and giving a real-world example that most closely aligns with the topic, you open up the thought process of whomsoever reads this article/tutorial. I would suggest adding a Table of Contents for the tutorial.

With that being said, I appreciate it if you are to write a tutorial about File IO, and enlighten the community of hivers, which I would presume to be growing once again.


Click to expand...

Yes, passion is exactly what I was feeling when writing this.

This is one of the few times that I've ever written a tutorial, and unfortunately it turned a bit more technical than maybe it should have, but I think that's partly due to the technical nature of the topic itself. It definitely isn't geared towards newcomers to JASS, but rather mappers who already have some experience on their shoulders and want to get into the guts of the engine.

I'll add a table of contents as well! (whenever I figure out how to)

PurgeandFire said:

 Is there any advantage to the host being the one who does the initial broadcast? I was reading this thread a while back, but wasn't sure if I was interpreting his words correctly: [code=jass] - A really really ridiculously easy way to sync data / new GetHost command i.e. i know that if player 5 wants to sync an action, he has to go through the host and then it gets broadcast, but if the host himself is starting the broadcast, can it skip one packet roundtrip?


Click to expand...

AFAIK, the host still waits a single latency cycle before broadcasting any of his actions to others. This is why, for example, even if you are the host in a game, you usually still have some small delay to your actions. In past, there were even tools to reduce/increase this latency, and you could definitely feel the difference when you set it down to e.g. 50ms as host.

At the end of the day, however, in the case of syncing I don't think it'll make a huge difference. The data transfer starts at about 4 kb/s for about a second or two, and then dips to 1 kb/s, even in LAN. If you aren't transferring a lot of data, you can expect it to be synced within one or two latency cycles as well.

This method of syncing isn't really well suited to syncing data in realtime either, although there are different approaches to the SyncStored natives that can offer different performance, geared for different workloads.

PurgeandFire said:

 Does this cause the selection to jitter whenever a sync happens? i.e. if I have a group of units selected, will I be able to see the UI switch to selecting one unit and then back?


Click to expand...

AFAIK, it shouldn't. In the system that I use as an example, the deselection/reselection occurs in a single frame, and because SelectUnit doesn't have any delay, by the time the engine draws the next frame, the selection is back to what it used to be. In theory, it should be completely transparent.

EDIT: However, all your actions are going to be 'stalled' if you are syncing a large payload. Any orders/actions that you issue -after- the SyncStored* calles are going to be delayed, so the player won't be able to move his units, chat, or do anything, really, until the Sync has finished.

Thank you for your responses, guys. This definitely gives me motivation to do a writeup on FileIO and Save/Load, and it's probably going to be two separate tutorials as well..

#### Deleted member 219079

D

#### Deleted member 219079

This was a good read, the contents are well organized.

Sir Moriarty said:

 In the future, I might write a tutorial about File IO in WC3 and fully-fledged codeless Save/Load systems (which Wurst also supports, in the form of Persistable, combining File IO with Networking and various tips. Please tell me if you'd like a tutorial for that!


Click to expand...

That would be nice.

#### Ceday

#### Ceday

 Level 12

JoinedFeb 22, 2010Messages1,115

This is indeed a really useful tutorial. I was trying to make a codeless save load system for my friend but I could not understand existing ones (especially SelectUnit calls here and there), now things start to make sense. I have some questions:

1- When you say SyncString does not work, you mean it does nothing at all or it is a buggy function which sometimes work and sometimes does not, or it has unpredictable delay? If it is the first case this explains why I was failing for days.

2- What are the alternatives of SelectUnit trick, I am just asking this for curiosity.

#### Lizreu

#### Lizreu

 Level 19

JoinedJun 13, 2016Messages588

Resources: 2

#### Resources
Tools 1Tutorials 1

Ceday said:

 This is indeed a really useful tutorial. I was trying to make a codeless save load system for my friend but I could not understand existing ones (especially SelectUnit calls here and there), now things start to make sense. I have some questions:

1- When you say SyncString does not work, you mean it does nothing at all or it is a buggy function which sometimes work and sometimes does not, or it has unpredictable delay? If it is the first case this explains why I was failing for days.

2- What are the alternatives of SelectUnit trick, I am just asking this for curiosity.


Click to expand...

I don't think SyncString works at all. In my tests, I haven't observed it ever syncing the string correctly.

One alternative to SelectUnit could be key presses (simulated), but it's less robust since you can't attach data to a key press, but you can to a unit. Unit orders would work too, but the same problem applies. It's easiest with a unit because you can create a unit for each synchronization and attach data to it.

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

Ceday said:

 This is indeed a really useful tutorial. I was trying to make a codeless save load system for my friend but I could not understand existing ones (especially SelectUnit calls here and there), now things start to make sense. I have some questions:

1- When you say SyncString does not work, you mean it does nothing at all or it is a buggy function which sometimes work and sometimes does not, or it has unpredictable delay? If it is the first case this explains why I was failing for days.

2- What are the alternatives of SelectUnit trick, I am just asking this for curiosity.


Click to expand...

1. The native doesn't work at all. Internally, it has code, but it flat out doesn't work. The client doesn't accept the packet from the host.

2. The escape key, unit abilities, unit orders, and unit selections. The escape key is actually only a 1 byte action packet whereas selections are a lot more. The problem is users can spam the ESC key themselves and the syncing/attached value is O(n). If you want to send the number 10, you have to call ForceUICancel 10 times.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Hmm.. the big trick behind File IO is the Preloader native I guess. You can insert new lines to close the Preloader calls and override it with your own JASS script. I believe that I personally used SetPlayerName. SetPlayerName gave me a way to read strings out of the file : ). You only have so many players in a game though, so 1 file can only have so many lines. Thus, if you want many lines, you gotta span it across many files. Atleast that's what I did. Maybe somebody came up with something better.

JASS, by default, lets you write files like this. You can abuse Preloader as much as you want. However, it doesn't let you read files. This is because Warcraft 3 doesn't let you execute JASS code in a willy nilly file somewhere =P. There is a setting in the Windows Registry that lets Warcraft 3 execute a JASS file. I believe I generated a little registry script or an OS shell script in the C drive using the same Preloader trick for JASS to set that registry setting I mentioned before to true. I also checked to see if a JASS file could be run outside of warcraft 3 by trying to execute it. I think it was Preload that executed? Anyways, nothing happening when trying to run the little JASS file meant that the registry setting for Warcraft 3 wasn't set. In that case, I made the ol' script to set the registry. I had a flag in the File IO System that people could read so that they could let players know to run that file.

Not too difficult : ).

Now... synchronization is a whole other can of worms o-o. Actually, the keys you use in a gamecache adds to the overhead of synchronization ;o. Any sync you call also adds to the overhead. That's why TriggerHappy and I moved towards timers (well, I just devised an algorithm but have yet to update my stuff to it >.>). You can actually get pretty crazy and have a bunch of gamecaches created so that minimally sized keys can be used. From there,, you can do a sort of stream to pack the data up 32 bits at a time (SyncStoredInteger). It's just about squeezing every last little performance you can squeeze outta wc3's synchronization natives. I don't think anyone's used my little model for gamecaches yet since just using bigger strings is ok enough ; P. I think I was the only one that took it that far, lol. I used to have all the time in the world : /.

#### Lizreu

#### Lizreu

 Level 19

JoinedJun 13, 2016Messages588

Resources: 2

#### Resources
Tools 1Tutorials 1

Nestharus said:

 Hmm.. the big trick behind File IO is the Preloader native I guess. You can insert new lines to close the Preloader calls and override it with your own JASS script. I believe that I personally used SetPlayerName. SetPlayerName gave me a way to read strings out of the file : ). You only have so many players in a game though, so 1 file can only have so many lines. Thus, if you want many lines, you gotta span it across many files. Atleast that's what I did. Maybe somebody came up with something better.

JASS, by default, lets you write files like this. You can abuse Preloader as much as you want. However, it doesn't let you read files. This is because Warcraft 3 doesn't let you execute JASS code in a willy nilly file somewhere =P. There is a setting in the Windows Registry that lets Warcraft 3 execute a JASS file. I believe I generated a little registry script or an OS shell script in the C drive using the same Preloader trick for JASS to set that registry setting I mentioned before to true. I also checked to see if a JASS file could be run outside of warcraft 3 by trying to execute it. I think it was Preload that executed? Anyways, nothing happening when trying to run the little JASS file meant that the registry setting for Warcraft 3 wasn't set. In that case, I made the ol' script to set the registry. I had a flag in the File IO System that people could read so that they could let players know to run that file.

Not too difficult : ).

Now... synchronization is a whole other can of worms o-o. Actually, the keys you use in a gamecache adds to the overhead of synchronization ;o. Any sync you call also adds to the overhead. That's why TriggerHappy and I moved towards timers (well, I just devised an algorithm but have yet to update my stuff to it >.>). You can actually get pretty crazy and have a bunch of gamecaches created so that minimally sized keys can be used. From there,, you can do a sort of stream to pack the data up 32 bits at a time (SyncStoredInteger). It's just about squeezing every last little performance you can squeeze outta wc3's synchronization natives. I don't think anyone's used my little model for gamecaches yet since just using bigger strings is ok enough ; P. I think I was the only one that took it that far, lol. I used to have all the time in the world : /.


Click to expand...

That's actually pretty much what I did in my Wurst port of the sync libraries. I used yours and TriggerHappy's libraries as references, which I've mentioned right at the top.

I don't really bother with keeping a lot of separate gamecaches, but I do keep key sizes down to 2 characters at most. I decided to omit implementation details from this tutorial, mostly because they are all already documented within various libraries, and because the purpose of this tutorial is to illustrate the core idea behind sync in wc3.

Thanks for you input, though!

#### piale-chini

P

#### piale-chini

Hi all. Unfortunately, I don't know where to start writing a client Application to play DOTA via network.

These days I'm trying to gathering required information to write this app but still I can't do that. Because there is no useful answer as I see.

So How can I get game data (player locations & status) from game and also write location to player over the network??

 You must log in or register to reply here.


### Similar threads

Locked

[Miscellanous / Other]
 Evolving Artificial Neural Networks


DeadZero

Jan 11, 2016

Idea Factory

Replies2Views1K

Jan 11, 2016
DeadZero

Locked

 Am I doing this codeless loading wrong?


LazZ

Apr 9, 2020

Triggers & Scripts

Replies5Views1K

Apr 12, 2020
LazZ

Locked

 SyncInteger/SyncString (Codeless Save/Load System)


TriggerHappy

Apr 30, 2016

Advanced Scripting

Replies14Views3K

Apr 30, 2016
TriggerHappy

W

Locked

 THW's social network game platform


Winimasker

Jul 6, 2015

Site Discussion

Replies15Views2K

Jul 27, 2015
TinkerLord

Locked

 [Ubuntu/Linux] Samba/Network issues


Anachron

Oct 3, 2010

Computer Tech

Replies7Views2K

Dec 21, 2010
Ironside

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top