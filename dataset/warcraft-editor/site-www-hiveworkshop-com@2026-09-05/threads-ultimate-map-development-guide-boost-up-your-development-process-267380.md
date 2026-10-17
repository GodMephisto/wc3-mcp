---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ultimate-map-development-guide-boost-up-your-development-process.267380/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Ultimate Map Development Guide Boost Up Your Development Process
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ultimate-map-development-guide-boost-up-your-development-process.267380/>
#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

Map Development Suite

- for better, faster, and convenient map development

Click to expand...

I. Intro

Hi and welcome! Just a brief resumé, I (in the modding scene) am a big enthusiast of total-conversion maps. I'm also experienced in developing some big total-conversion map projects such as Garden's Tale, Glideon, Coconut Party, and EmberCraft.​

​

This thread is a one-stop solution for you to significantly improve your map development process. It is perfect for ambitious mapper who seek to work on big map projects. It contains a lot of mapping-related advice, tips and tricks, code snippet & custom system recommendations, and more. It will guide you to develop high quality and highly attractive map, back from ground up to be a fully developed map, that at very least will receive instant approval here in Hive.​

​
A. Mapping Tips & Advices

Use interesting custom map preview

First impression is key! And in most cases, the very first thing people see about your map is the preview. Create and use the most interesting preview image you can make. But preferably without faking anything. Try to keep the preview image to resemble the actual map as possible. Using fake image, if your map quality doesn't keep up, will only provide minor & temporary download boost and it might stain your reputation as a map maker. Alternatively, you can make a custom logo for your map, but it will require some art/design skills.

Invest on good map presentation and resource thumbnail!

Your map thread design and appearance (presentation) will hugely impact your audience's expectation of your map. Invest on good amount of time designing and building your thread to be as interesting or unique as you can possibly make. To make advanced thread layout you might to learn more about BB codes. Remember, appearance is half of the whole performance!

These map threads might give you some inspirations:
IID Beta v0.88b

Iceborn v1.9

EmberCraft v1.2a

WoW: Heroes Return [ORPG] [1.2.b]

Enthashara´s Tales ORPG 1.3.4 fix

Mercenaries r1.2f

BoW ORPG v1.32e

Use custom loading screen

Keep track of credit list along the way

Make a to-do list & try deadlining

Build and grow your community

Get rid of corrupted (green) icons

Don't be too greedy & perfectionist, start small!

Warcraft shell extension

Stop over-sharing

Learn color tags

Elaborate tooltips, don't use sharp & high-contrast colour

Learn vJass and use syntax highlighter programs

Index

1. Use hashtable easily​

2. Handle keyboard events easily​

3. Handle trackable easily​

4. Keep your map clean from tome items​

5. Create multiple missile-based abilities easily and effectively​

6. Assign unique id to any unit​

7. Assign certain data into a timer​

​

​
Content

 >>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<

Click to expand...

1. Use hashtable easily: [Snippet] NewTable

​

​

• Recommendation level: High

• Usage difficulty: Easy

• Implementation difficulty: Easy

• Advantages:

• Hashtable limit is removed​

NewTable (by Bribe) is by far one of our best code resource so far. It's very widely used among both spell makers and map makers. Let's say it's a must have resource in every map. Why? Almost every map needs hashtable. Where as hashtable is very limited (only 255 hashtable per map). Using NewTable the limit does no longer exist. Other than that, NewTable provides nice interface:

call SaveInteger(ht, parent, child, 123)

=>

set parent.integer[child] = 123

It's very easy to use even for who don't know anything about vJass. It also let you do some 2D variable stuffs (you can read the further details in the link provided above).​

 >>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<

Click to expand...

2. Handle keyboard events easily: [System] ArrowKeyEvent

​

​

• Recommendation level: Low

• Usage difficulty: Medium

• Implementation difficulty: Easy

• Advantages:

• Arrow state checking (IsArrowKeyPressed)​

If only your map needs arrow key events for something (control, camera adjustments, or anything else), this system will ease your life as well as providing useful extra feature like what's mentioned above.​

 >>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<

Click to expand...

3. Handle trackable easily: [System] Track

​

​

• Recommendation level: Low

• Usage difficulty: Hard

• Implementation difficulty: Easy

• Advantages:

• Getting the trackable clicker​

What's trackable? It's not going to be discussed here but to get basic idea what it is, trackables are widely used in Full Screen Inventory systems (like what TKoK has) cz it provides quite instant click event and useful hover event.

The recommendation level is low simply because I rarely see maps use trackables. Usage difficulty is hard because you need basic knowledge about trackable, and since I rarely saw people are actually experienced about trackable. But once you understand what trackable is and how to use it, this system will be absolutely helpful. It also provides a great feature to get the triggering player of the click/hover event, which normal events do not provide.​

 >>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<

Click to expand...

4. Keep your map clean from tome items: Item Cleanup

​

​

• Recommendation level: Medium

• Usage difficulty: Easy

• Implementation difficulty: Easy

• Advantages:

• -​

It's very rarely known by map makers that tome items never get removed from the map after being used (acquired) and eventually may causes lags. Thus this system was made by Bribe to settle this issue. If your map has tomes, this system is absolutely must have.​

 >>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<

Click to expand...

5. Create multiple missile-based abilities easily and effectively: [System] Missile

​

​

• Recommendation level: High

• Usage difficulty: Hard

• Implementation difficulty: Easy

• Advantages:

• Tons of missile customization​

Sometimes we want to create missile-based spells with different but similar effects, thus we simply copy paste the other ability and modify it, which is not very practice and easy to do. Now there comes this system that allows you to toggle on/off which event do you actually need (on hit/on periodic/ etc.) to ease you in customizing your spell effects.

It's hard to tell exactly how hard is it to use this, because basically it's very easy to use for anyone who understand vJass. But yeah, it requires vJass knowledge.​

 >>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<

Click to expand...

6. Assign unique id to any unit: (vJass) UnitIndexer | (GUI) GUI UnitIndexer

​

​

• Recommendation level: High

• Usage difficulty: Easy

• Implementation difficulty: Easy

• Advantages:

• Provides on index/deindex events (brief explanation below)​

Almost every map use this. UnitIndexer is used to assign unique id to every unit in your map automatically. Sometimes we want to have variables that contain specific information about a unit, as example I want to store a unit's damage into a variable named "UnitDamage":

(vJass)

 JASS:

set UnitDamage[GetUnitUserData(TheUnit)] = 100

(GUI)

Set UnitDamage[(Custom value of (TheUnit))] = 100[/FONT]

[FONT=Tahoma]

You simply need to call "GetUnitUserData" (for jass) or "Custom value of unit" (for GUI, this is available under Unit function group). That's just one example usage of UnitIndexer, you may discover other usages as you experience more about spell making.

Another great deal is UnitIndexer provides events called on index event and on deindex event. Index event is fired when a unit is created (added to the world) and deindex event is fired when a unit is removed from the world.

However, you may need to be careful to pick which UnitIndexer for your map. Having multiple indexer systems would likely mess things up. If you use Nestharus' UnitIndexer, you mostly will not be able to use any other systems that requires Bribe's GUI UnitIndexer. And vice versa.​

 >>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<

Click to expand...

7. Assign certain data into a timer: TimerUtils

​

​

• Recommendation level: High

• Usage difficulty: Easy

• Implementation difficulty: Easy

• Advantages:

• Preserves & recycles timers​

This is also one of the best code resource in wc3 so far, widely used among spell makers. It has simple yet very useful feature to allow us to assign any integer data into a timer. And as been mentioned above, TimeUtils (in certain condition) does not destroy timer once you release it but TimeUtils will recycle (store) the timer for later use.​

 >>>>>>>>>>>>>>>>><<<<<<<<<<<<<<<<<

Click to expand...


 Last edited: Nov 5, 2021

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Wow, very useful.. +rep

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

If you extend this a little bit further, this could become a nice go-to for mappers on what is a great coding framework to start a new project with.

Here's no honorable mentions that imho you should include:

Knockback3D from Cokemonkey.

-> Simple yet effective knockback system; doesn't come with a lot of extra features like custom callbacks, but it's exactly the simplicity that I like about it.

AIDS or UnitIndexer.

-> Nowadays, the unit indexing feature is actually the least significant reason to use an indexer - it's the onIndex and onDeindex events that are so god damn useful. You never need to worry about all the ways a unit can be added or removed from the battlefield anymore ... the onIndex and onDeindex events catch them all.

SimError.

-> Custom error messages are always nice.

DDS:

-> Everyone needs a damage detection system.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice idea.

Perhaps a section on timer systems? Usually it'll be TimerUtils for general purpose and CTL for periodic.

If expanded, I wonder if this could be a sticky in the JASS section? Or maybe a sticky in the JASS tutorials section? I'm not sure which.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

Update. Some suggestions have been added.

Sure I will expand this but rather slowly.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Feels kinda lacking.

Out of those 7 mentioned I have never felt a need for 6 of them.

I am surprised that you haven't included a DDS in the list btw.

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Great idea. I like the structure and rating system.

--> SpellEffectEvent?

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

^That is a must and the player event by Mag. Why not throw Alloc in the mix also.

#### Ardenian

A

#### Ardenian

What about the CSS by Doomlord or a collision system ?

#### btdonald

#### btdonald

 Level 11

JoinedDec 3, 2011Messages366

Resources: 9

#### Resources
Maps 1Spells and Systems 8

Ardenian said:

 What about the CSS by Doomlord or a collision system ?


Click to expand...


I'm thinking about (Bonus Nestharus or Bonus Mod Earth Fury) instead of CSS ^^!

#### Ardenian

A

#### Ardenian

Well, I suggested it as it is GUI-user friendly which is especially good for map creators being new to Jass and vJass.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

I'm actually planning a GUI SpellEffectEvent but I have to test a few things first.

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Keep on working on this. I really like it.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

I will work on this later..

- Going to remove TimerUtils from the list. Because, mostly it's rather for minor improvements, definedly not a must have in a map.

- Adding DDS and CSS to the list.

The list is supposed for systems that seem useful for common map makers, not just for coders, or I will ended up adding every system and snippets in the jass section.

Chaosy said:

 Feels kinda lacking.

Out of those 7 mentioned I have never felt a need for 6 of them.

I am surprised that you haven't included a DDS in the list btw.


Click to expand...

Bcs this thread is not made specifically for you. And yes, it needs times to improve.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Indeed it is not. But if someone who knows a bit of vjass does not find the systems needed.. I doubt the average GUI user will. Effectively ignoring most of hive. In your intro you state that it is for everything though.

I might be wrong, wouldn't be the first time. Just my thoughts though, take it as you will.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

- A generic list/vector/stack, something of this category can be also useful often.

- WorldBounds maybe

- Missle maybe

- TileDefinion maybe

.. it's nice. Will you update it?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

It's approveable, but you would need to keep it up to date, will you?

Like we can discuss about mentioned DDS, CSS, a generic data structure, [vJASS] - [System] Polygon, [System] SoundTools.

Is ItemCleanup important enough for you?

It only makes sense if we keep it up to date from time to time I believe.

Quilnez said:

Index

(Index is used to ease you in memorizing which entries are useful for your personal needs)


Click to expand...

Could you put the library name there instead the index numbers?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

@Quilnez

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

Updated with improved formatting, nothing new's added yet tho.

I will add suggested resources later.

Sorry, @IcemanBo. This one was completely out of my radar.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

i still like this and would like to approve it, @Quilnez , but some of the BB tags are messed up.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Add damage detection systems and I'll consider this tutorial as a good guide

SpellEffectEvent or Spell Event is also a good recommendation (been using it for all my newer spell resources)

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

@Kazeon I am taking the liberty to update the broken tags in order to approve it. Purge asked you to do this a long while ago so I figure you don't have the intention to do it yourself.

Would still like to see this updated with new additions however.

 You must log in or register to reply here.


### Similar threads

 Full Map Descriptions Creation Process


Apheraz Lucent

Nov 27, 2011

Misc/General Mapping Tutorials

Replies3Views5K

Nov 29, 2011
PurgeandFire

 Description Guide - Maps


Rufus

Aug 28, 2016

Misc/General Mapping Tutorials

Replies12Views16K

May 24, 2017
Rufus

 Basic guide - RPG gameplay


LordOfThePings

Apr 13, 2014

Misc/General Mapping Tutorials

Replies12Views8K

Mar 15, 2015
daverave1212

 Description Guide - Melee Maps


Rufus

Aug 23, 2016

Misc/General Mapping Tutorials

Replies10Views12K

Nov 21, 2016
Chaosy

Locked

 Map Development


Rheiko

Sep 28, 2013

Off-Topic

Replies3Views996

Jun 27, 2014
Brygor009

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