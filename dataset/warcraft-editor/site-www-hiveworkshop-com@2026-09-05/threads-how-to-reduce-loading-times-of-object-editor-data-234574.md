---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-reduce-loading-times-of-object-editor-data.234574/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Reduce Loading Times of Object Editor Data
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-reduce-loading-times-of-object-editor-data.234574/>
#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

One of the main things that makes big maps lag is inefficient use of the object editor. This tutorial is meant to bring out some things that can be used to prevent that. This tutorial is not limited to object editor stuff, but it somewhat focuses on it.

Whenever you change something in the object editor it is written down. The base values are all unchangeable. What the editor really does is create a new unit and write down how it is different from the base unit.

Purple color marks the fields that have been changed and thus, are written into the map file.

Whatever you do, one thing is for certain. You can only avoid purple fields if you are making a melee map.

What you can do, however, is use the purple fields wisely.

Common mistake 1:

Making an ability with 100 levels can be all fine and dandy, but lets imagine that each ability level takes 0.05 seconds to load. Quite little time, isn't it? But now suddenly your map loads 5 seconds longer from just 1 ability.

This is not a real life example, but it can be used to show the difference.

This is all because in any practical way if you make an ability with lots of levels, then it will have many purple fields.

Note that number values are very cheap, as seen in Benchmarks.

Now imagine what would happen if you made an ability with 10 000 levels? You can try it out, but be warned that it will lag the editor(and increase loading times).

Does each level really make a considerable difference ingame?

What is the lowest amount of levels that this ability must have.

I used to use 100-level abilities for making units gain combat experience. So that a unit with critical strike ability would have a greater chance to crit when it actually has been in a fight.

The solution to this is coded criticals.

Note that this is not an easy solution(aww, triggers...), but it saves huge amounts of power.

Get a damage detection system

Get a unit indexer

Create a variable array for the critical chance of units(preferrably integer, because integers are less expensive than reals. Not that it matters that much. )

Every time a unit deals damage check if it is allowed to have critical strikes(if it has ability). If it is, then add some amount to critical hit chance

This can be applied to any stat. I just used critical hits as an example. Due to numbers being cheap you do not need such system for criticals, you only need to avoid having lots of text fields(only create text for the first field).

Changing stats such as health can be tricky, that's why you should combine the previous approach with a Bonusmod system.

If you use a custom editor such as JNGPE, then you can even make hero units out of non-heroes and the other way around, without using purple fields. On highly customized heroes this might even be worth it if you know a non-hero unit similar to your hero idea.

*When one of the text fields is empty, then the editor uses the text from the previous field, if the previous is also empty, then it goes one more step back, and so on.

*Always keep brush list disabled. It hasn't got anything to do with loading times, but it makes mapping much faster.

Widgetizing is not a mistake, but a way of optimizing the map. Essentially it makes the map use different base files(slk/txt), which simply load a lot faster due to being in a different format.

One of the limitations of widgetizing is that abilities are limited to 3 levels(no room in the tables).

In a sense the widgetizer fixes one of the mistakes that blizzard made.

Widgetizer

Widgetizing has been told to decrease loading times by up to 80%, but increases map size.

However, note that widgetizing can cause crashes when you play a non-widgetized map right after playing a widgetized map.

A lot of the loading time can be taken up by various things that happen at initialization. Some of those things can be done at other times though.

How it's typically done:

Trigger
Events
Map initialization

Conditions

Actions
-------- Doing stuff here --------

How it should be done:

Trigger
Events
Time - Elapsed game time is 0.01 seconds

Conditions

Actions
-------- Doing stuff here --------

Doing so pushes the actions of that trigger to happen after the map has loaded, resulting in a lag spike right when players have finished loading, but reduces loading times. By my experience this lag spike feels shorter than the loading time that is avoided with this method, but that's mostly an illusion.

Typically the game loads things only when it needs to. Things can be loaded before they are really needed by placing them on the map or though a special JASS function.

This example preloads the wisp model:

 JASS:

call Preload( "units\\nightelf\\Wisp\\Wisp.mdx" )

The Preload function is typically used in melee games to prevent lag when a new unit type appears or some spell is cast for the first time.

Anything that happens during map initialization, including creation of preplaced units, makes the loading time longer, so it's better to create things after the map has loaded.

Models are loaded when they first appear.

Abilities are loaded the first time they are given to a unit.

This means that if you have an ability with 9999999999999999999999 levels and there's a preplaced unit that has it, it will make the loading time very long.

However, if you create the unit just 0.01 seconds after loading, it instead creates a huge lag spike after the game starts.

When you have a map with lots of abilities it's reasonable to preload everything over a long time so players won't notice any lag.

Preloading is useful because it lets you prevent random lag spikes by making them happen at a specific time.

Testmap with 1 custom critical strike(neutral)

Test1: No purple fields - 4 seconds

Test2: 101 purple fields - 4 seconds

Test3: 1001 purple fields - 4 seconds

Test4: 3001 purple fields - 4 seconds

On each of those tests there was 1 integer field and the rest were reals. There might have been up to a 0.5 second loading time difference, but I consider it insignificant, considering the amount of fields.

Test5: 6001 purple fields - 35 seconds.

On this test I filled 3000 text fields. Elseway it's like test 4.

Test6: 101 purple fields - 8 seconds.

This time it was the levels field and 100 tooltip fields.

Quite apparently strings take way more time to load than reals.

Testmap with 1 custom Raise Dead

Test1: 1001 purple fields - 8 seconds

1000 of the fields are unittype fields.

Apparently unittype fields are somewhat costly. It seems that a unittype field is a string with 4 letters.

The reason to strings/unittypes being expensive seems to be that while every number variable is 32 bits a string is 32 for every letter.

It also seems that it has something to do with strings beings processed in an inefficient manner.

#### Attachments

Purple Field Testmap1.w3x

 6.5 KB

 · Views: 281



Purple Field Testmap2.w3x

 7 KB

 · Views: 264



Purple Field Testmap3.w3x

 12.1 KB

 · Views: 274



Purple Field Testmap4.w3x

 23.3 KB

 · Views: 269



Purple Field Testmap5.w3x

 82.3 KB

 · Views: 267



Purple Field Testmap6.w3x

 81.5 KB

 · Views: 259



Purple Field Testmap7.w3x

 9 KB

 · Views: 285




 Last edited: Apr 28, 2016

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

u forgot to meantion about unselecting brush list in the window tab.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

deathismyfriend said:

 u forgot to meantion about unselecting brush list in the window tab.


Click to expand...


I thought about adding it, but it has nothing to do with reducing loading times. I guess I'll still add it.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

thats true but it helps a ton with editing time. it may not be a perfect fit with the tutorial but its still in the right area. IMO

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Xonok said:

 Whenever you change something in the object editor it is written down. The base values are all unchangeable.


Click to expand...

They are unchangeable within the editor. You can however overwrite the slk/txt files that harbor them by overshadowing their paths in the map. This format is loaded much, much faster and it is initialized altogether on map loading. There are two, maybe three cons. You cannot have data for levels/variations > 4, the fields for them do not exist. Those would again have to be realized via magenta modifications. Second, when replacing the txt files, those are normally language-dependent, so you lose the localization there. And lastly, in case you want to additionally use the standard data, you would have to reimport the whole files, which boosts map size. In reality, you should discard stuff you do not need.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

WaterKnight said:

 They are unchangeable within the editor. You can however overwrite the slk/txt files that harbor them by overshadowing their paths in the map. This format is loaded much, much faster and it is initialized altogether on map loading. There are two, maybe three cons. You cannot have data for levels/variations > 4, the fields for them do not exist. Those would again have to be realized via magenta modifications. Second, when replacing the txt files, those are normally language-dependent, so you lose the localization there. And lastly, in case you want to additionally use the standard data, you would have to reimport the whole files, which boosts map size. In reality, you should discard stuff you do not need.


Click to expand...


Does this have a tutorial somewhere? If yes, then I'd link it. If no, then I'd add it to this tutorial.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

This stuff is essentially done through the tool, Widgetizer:

http://www.wc3c.net/showthread.php?t=79231 [download link]

http://www.wc3c.net/showthread.php?t=78640 [advanced features tutorial]

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

You could use the hack of JNGP and add a tool that transforms your object editor data upon saving. The problem is that the editor cannot work with custom slks effectively. It does not load imported slks from the map, so when you reopen your map, you will not have access to your data. And if someone would succeed in modifying the editor this way, it would have to be 100% accurate in order not to kill your file or at least store a backup of the modification data.

I am doing it the way that I separate editor map and playable file. When saving in editor, it copies the map to elsewhere and alters the duplicate (and also issues to playtest it, so I do not have to traverse the os everytime).

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

WaterKnight said:

 You could use the hack of JNGP and add a tool that transforms your object editor data upon saving. The problem is that the editor cannot work with custom slks effectively. It does not load imported slks from the map, so when you reopen your map, you will not have access to your data. And if someone would succeed in modifying the editor this way, it would have to be 100% accurate in order not to kill your file or at least store a backup of the modification data.


I am doing it the way that I separate editor map and playable file. When saving in editor, it copies the map to elsewhere and alters the duplicate (and also issues to playtest it, so I do not have to traverse the os everytime).


Click to expand...

Would be nice if someone made it. I don't know how though.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

It's not only about oe data. In general, maps can be optimized and there are approaches to go away from the standard editor. To join hands, the separation into development data and processed map would be recommendable.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

WaterKnight said:

 It's not only about oe data. In general, maps can be optimized and there are approaches to go away from the standard editor. To join hands, the separation into development data and processed map would be recommendable.


Click to expand...


Wish someone actually made a specialized editor though, not just a hack.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Sorry about not giving a review.

I think it had the "This is under construction" text on it so I hadn't done a thorough review yet. I really like this info though. It is great to consider when making a map. I don't really like having the entire tutorial in spoilers (it is a bit hard on the eyes) but to each his own.

Nice job. Approved and useful!

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

PurgeandFire said:

 Sorry about not giving a review.

I think it had the "This is under construction" text on it so I hadn't done a thorough review yet. I really like this info though. It is great to consider when making a map. I don't really like having the entire tutorial in spoilers (it is a bit hard on the eyes) but to each his own.

Nice job. Approved and useful!


Click to expand...

Yes, I had a large text that said it's still in writing.

After a while I realized I'm out of ideas, so I just decided it's good enough for now.

EDIT: I believe the most important part of this are the benchmarks, as they even changed my original hypothesis.

I now know that number values don't have a noticeable impact, while strings can kill your map.

It makes sense though, since each number is 32 bits while each letter in a string is 32.


 Last edited: Jun 15, 2013

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Added a new section called "Initialization".

I'm considering reworking this tutorial to be more thorough.

#### Ceday

#### Ceday

 Level 12

JoinedFeb 22, 2010Messages1,115

I don't agree with initialization part.If you are going to do something that early which doesn't require game to start(multiboard, timer window etc) you should do all your stuff in initialization/loading.That is what loading means anyway.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Well, the point is to reduce the time spent in the loading screen. As far as I've seen, having a lag spike at the start of the game isn't even noticeable even with lots of things loaded that way.

#### laserdemon

#### laserdemon

 Level 6

JoinedAug 4, 2012Messages193

Maybe you should discuss on how to reduce purple field, I think it's really hard to avoid changing them if you want to make your map different from Warcraft 3

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Usually it's nothing tragic when you have hundreds of custom abilities. This tutorial is just meant to give people some idea where exactly the long loading times might come from.

Note that purple fields only matter on abilities that are actually on some unit. If you don't have any units with that ability, it doesn't get loaded.

I think the Preloading part might even be more important than purple fields, not everything necessarily happens during the loading.

If you have lots of things in the map you can significantly increase performance by just loading them when they're needed, instead of right away.

In an ORPG this might mean organizing the map as areas where you only spawn creeps when the players first enter that area.

I've seen many ORPG maps where the creeps are preplaced. In most cases that's just not necessary.

#### laserdemon

#### laserdemon

 Level 6

JoinedAug 4, 2012Messages193

I think this becomes a problem when you have heroes with hundreds of levels and custom abilities with 100 levels, the purple field for abilities data and tooltip make the abilities data exceed 2mb which causes the loading time increase dramatically. Preloading make the game lag and probably disconnect when player's computer is too old

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

2MB of ability data is something I have never seen. An ability with 8000 levels of floats(the ability is spell shield) freezes the game for over 10 seconds. Yet, the map only takes 98 KB with that.

If you have 2MB of ability data, you're probably doing things so wrong that it doesn't even matter if you preload things or not.

In any case, you shouldn't preload lots of things at the same time. If you don't load many things at a time the lag won't be noticeable.

#### laserdemon

#### laserdemon

 Level 6

JoinedAug 4, 2012Messages193

Since the abilities have many levels, and each level requires tooltips, this will make the ability data very big especially the tooltips is long, and for every level, how can i overcome this?

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

By designing your map so that it's unnecessary. What are those levels for?


I have a feeling that this question might work better in a separate thread.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Even if you have so many levels, you normally won't have completely individual tooltips, therefore it's a job for the mpq compression.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

MPQ compression reduces map size, but it doesn't appear to have any useful effect on loading times. The best you could do is just having the object data be loaded as gradually as possible OR creating a huge lag spike at the start of the game to prevent it later on.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Xonok said:

 MPQ compression reduces map size, but it doesn't appear to have any useful effect on loading times.


Click to expand...


Yes, replied to the question.

Xonok said:

 The best you could do is just having the object data be loaded as gradually as possible OR creating a huge lag spike at the start of the game to prevent it later on.


Click to expand...


Gradualism in that regard in wc3 is hardly possible, though. The atoms are too large, when loading an object with 100 levels, you cannot split it up and concurrency, the separation of gameplay and interface for us to handle, is almost not-existing. Those are advanced concepts to begin with, even if you had the freedom. But most of the load in my maps stemmed from object modifications. What is still causing it on your end?

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

I was talking in a more general sense. You can't reduce the lag much after a certain point, all you can do is reallocate it to some specific time that doesn't matter as much.

#### Wark

#### Wark

 Level 13

JoinedOct 12, 2016Messages769

Resources: 3

#### Resources
Maps 3

This is a lot of information I did not know when I started designing my map 7 months ago.

I have a tendency to organise my object editor for my own convenience, and my map has many lvl 80 abilities

for easier ability management. This creates a ton of purple fields and modified text fields.

Now, I do have a question about this.

IF:

I copy all of the modified units and abilities and make them custom data in their respective organised fields and reset the base data to default,

THEN:

Would the copied custom units, abilities, upgrades, doodads and buffs with base data reset to defaults cause less loading time than if I had purple fields everywhere from reorganising everything?

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

A custom ability is a custom ability, it doesn't care what the values of the ability it was based upon are.

So no, "reseting base data" will not reduce the loading time. Reducing the levels of the abilities so they're at max 3, will reduce loading time.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

All that really matters here is the amount of purple fields. Each object and each purple field takes some space. They also have to be loaded into the map at some point, although not necessarily during loading.

One thing you could do is stagger out the loading. You can preload objects by placing them in the map after the game has started. If you place them in editor, they add to the loading time, but if placed ingame it only causes a lag spike. Spread the lag spikes out enough and make them happen when players can't do anything useful anyway. Voila, you have tricked the players into not noticing your loading times.

#### Wark

#### Wark

 Level 13

JoinedOct 12, 2016Messages769

Resources: 3

#### Resources
Maps 3

I read that part with the script for preloading. The primary thing is the lvl 80 abilities, which are mandatory and part of the core design.

Let's say I have a unit ability called "Fireball."

How would I go about preloading that in the map to reduce initial load times?

The units that use these abilities are created at map initialization, so should I preload those units in particular then create them at Time Elapsed = .01 seconds?

 A custom ability is a custom ability, it doesn't care what the values of the ability it was based upon are.


Click to expand...

The main thing I am concerned with is units, since I make a lot of purple fields simply moving (for example) unused units to the Neutral Passive - Campaign section. In other words, for organization purposes, is it better to just leave the default data alone and create custom units?


 Last edited: May 5, 2017

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Yes, abilities are preloaded if any unit that has them appears, so preplacing them works to reduce lag spikes. It will increase loading time a bit too though.

I think organization isn't really all that big of a problem. It's just a few fields here and there. If you get problems it's much more likely from the ability with 80 levels having a separate tooltip for each other.

Consider that every letter in a string costs as much as an entire number.

#### Wark

#### Wark

 Level 13

JoinedOct 12, 2016Messages769

Resources: 3

#### Resources
Maps 3

I see. Thank you Xonok for the clarification. I simply want to optimize my map and code to be as lag free as possible with the lowest possible loading time.

Edit:

I found out something rather important to add to this section.

Hidden purple text fields not only cause longer load times, but also cause lag on the Quest menu (F9)!

I found this out the hard way.

Case example: let's say you set an ability to lvl 100. Then, you modify all the text fields, and set it back to lvl 3 or something. All those modified text fields will still register even though they are hidden, and cause massive lag on the F9 menu and load screen.


 Last edited: May 24, 2017

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

I was aware of the hidden fields still existing. Forgot to mention it I guess. You can use this to make stuns go through spell immunity by making their level required higher than 1. Normally this is only possible for hero abilities, but you can just turn the ability back once you've changed level required.

Lag on the quest menu is quite surprising though. Why would that cause abilities to load? Strange.

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

This tutorial is old, last responses too, but I cannot not comment on it.

First of call, cool topic and tests. However I disagree with the conclusions. The loading stage of the map is meant to load everything that's needed for gameplay. Are long loading times bad? Sure. However it's only worth optimizing for loading times if it comes at very little cost. 10 years later, we no longer have machines from 2006 running Warcraft. Reforged bumped the system requirements so high even infamously long loading times are bearable.

If the improvements would cost you a lot of time, you shouldn't do them. This includes heavy restructuring of current objects. You are a single map author and I'd rather have a playable map with +5s loading times than none at all.

Preloads are a different topic. I get mildly annoyed when I load a custom unit for the first time on a map and there's a short stutter. Yes, it's the game loading the model. Why not in the loading screen... it would eliminate this stutter!

Finally, initializing and preloading stuff during the loading screen should be safer than doing so in-game. You wouldn't want a stutter to turn into a lag, which would become a disconnect of the laggy player. The loading screen has special treatment by the game in this regard, everybody's waiting. Though I've played an old map that waited a couple seconds on purpose after loading. This indicates it was once needed for really slow PCs to transition from the loading screen into the game.

Use Preloads if you can (basically hand-picking what assets to load and to create a file)

Do most stuff during loading. Except for really rare stuff that might not be used during the game session

Reduce custom text fields as suggested

Reduce ability levels as suggested (nobody's gonna want to click through 50+ levels anyway)
Please note:

Map init trigger is special, delayed timers are different

Lua-only: 0.01 or 0.001s (I don't remember) delay after start to do anything is currently a cause for desyncs

People alt-tab or use their phones. An audio signal that the map has finished loading works greatly. Like the cuckoo of the morning cock.

Don't optimize something that's not a problem. You can only optimize ahead of time if you have the experience to anticipate problems in the future.

 You must log in or register to reply here.


### Similar threads

Locked

 Object Editor showing too much data?


Soul Reaver

Jun 16, 2012

World Editor Help Zone

Replies13Views11K

Aug 4, 2016
Soul Reaver

Sticky

 A simple way to greatly reduce Object Editor lag!


Caedrus

Jun 9, 2010

Misc/General Mapping Tutorials
2

Replies80Views95K

Aug 27, 2020
Atreus

F

Locked

 Object Editor Issues {reducing time to change fields}


FireSpore

Jun 21, 2009

World Editor Help Zone

Replies2Views1K

Jun 21, 2009
FireSpore

F

 Raw Data Tutorial


Oninuva

Apr 9, 2007

Misc/General Mapping Tutorials

Replies2Views15K

Apr 25, 2007
Wolverabid

 The Basics of Object Data


Ralle

Jan 11, 2006

Misc/General Mapping Tutorials

Replies0Views25K

Jan 11, 2006
Ralle

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