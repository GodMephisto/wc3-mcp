---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/transitions-among-campaign-maps.6283/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Transitions Among Campaign Maps
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/transitions-among-campaign-maps.6283/>
#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


From www.battle.net

by Mtlhead

This tutorial describes a specialized use of the 'Save Game and Load Game' and 'Save Game and Change Level' triggers that were originally demonstrated by Blizzard in the Orc Expansion Campaign that came with the Frozen Throne WCIII expansion, and later were released for public mapping in WCIII Patch 1.13. This tutorial assumes that you already have experience and a good understanding of the WCIII World Editor, and have some experience with advanced triggering. If you are new to WE (i.e. a "newbie"), I recommend you spend some time learning about the World Editor by making maps and reading other tutorials before trying this one. This tutorial particularly assumes that you know how to use the Campaign editor and are familiar with techniques for Game Caching. If you don't know what those are, please don't even try this yet.

I must give credit where due and explain why this tutorial is unique. I want to thank Blizzard for releasing the save/load triggers to the community, as this adds a level of creativity above and beyond what you could do with WE before. I must give credit to Adins Bejoty, as his save/load tutorial, hosted on a similar site to this one, was the first tutorial released on the subject. However, this tutorial is different in two ways.

First, his tutorial draws directly from the Blizzard triggers in the Orc Ex. Campaign Part 1. I learned the basics of the system and created what I believe is a simpler method of doing transitions. Second, and most importantly, this tutorial shows how you can create transitions between any maps in a campaign, not just between an 'Overmap' and secondary maps. In the Orc campaign, you had a hub-and-spoke configuration that looked something like this:

Though this was possible in the Orc Campaign, the Blizzard designers wanted to keep the "Overmap" structure. In my opinion, this put restrictions on travel from region to region (map to map) that don't reflect reality.

On the other hand, this tutorial demonstrates a full-mesh system, where there is no need for acentral Overmap, like this:

In other words, you can set up the system to allow travel to any map from any map. It makes things much more interesting. Once you understand the system, it is easy enough to create any combination of fully and/or partially connected maps for your campaign creations. I highly recommend you create your first campaign by reading this tutorial and manually typing in triggers as you go along, instead of just copying the triggers out of the campaign files. That way you will have a better understanding of what is going on and then you will be able to use the system for your own creations. You can make 4-map, 5-map, or even larger campaigns with unlimited transition possibilities.

...On with the show...

The first task is to open the campaign editor and create a new campaign and name it something like 'Transitions Tutorial". Then create three blank maps and name them something like 'Map1', 'Map2', and 'Map3'. Save these maps and then import them in to your campaign on the 'General' tab of the campaign editor. Don't forget to add a button for each map on the ''Loading Screen' tab also, so that the maps will all show up as 'used' in the 'General' tab. Once you have done this and saved your campaign file, close the maps in WE. From now on, you should only edit the maps by right-clicking them in the Campaign Manager 'General' tab window and selecting 'Edit Map'. This way, when you save the changes to your maps, they are saved in to the campaign file directly and you don't have to worry about re-importing maps in to the campaign file.

At this point I must pause to give warning of a particularly frustrating bug in the save/load trigger system before continuing with the tutorial.

The 43-Character Limit

In creating this tutorial, I intended to be as clear and comprehensive as I could in providing descriptions and documentation of every aspect of the system. Note the heavy use of commenting in 'TransTutorialMap1'. One of the things I thought might help was to rename the campaign, map files and Saved Game folder with accurate descriptions. This was when I encountered one of the strangest and most perplexing bugs I have ever seen in the World Editor. I discovered that the Load Saved Game action has a limit as to how many characters it can read in the path to a saved map file. I'll try to explain this, as it was an extremely aggravating problem that took me weeks to solve.

One of the keys to the Save/Load trigger system that Blizzard released in Patch 1.13 is where the game saves maps when you run the save-game trigger and then load another saved game. The basic path to these saves is:

 Code:

C:\Program Files\WarcraftIII\save\Profile1\CustomSaves\(name of your campaign)\(custom save folder, optional)\(map name).w3z.

Your path may vary depending on the drive letter your game is installed on, and whether you installed the game in the default directory or a custom directory, but these are not really relevant to the problem. For this tutorial, you are only concerned with the part of the path that comes after the …\CustomSaves\… folder. To use the Save/Load system, your maps must be copied in to a campaign and you must run the game from the Custom Campaign screen for your maps to work. You can not test your maps by clicking the 'Test Map' button in WE. The transitions will not work if you try to test this way. In the tutorial campaign, I used a custom save folder for the maps, but this is not really necessary. The bottom line is that the combined length of

 Code:

(name of your campaign)\(custom save folder, optional)\(map name)

or:

 Code:

(name of your campaign)\(map name)

cannot exceed 43 characters, including the back-slashes (but not including the file extension). If it does, the game will not find the saved map and will crash to the campaign loading screen. Let me illustrate:

 Code:

Transition Tutorial\Saves\TransTutorialMap1.w3z

is the path to the maps in the tutorial campaign (see Download at the end of this tutorial). The length of this path is 43 characters including spaces and backslashes, but not including the .w3z extension. This works, but is as long as it can be without causing problems. If I were to have used:

 Code:

Transition Tutorial\Saves\Trans Tutorial Map1.w3z

This would be 2 characters too long because of the added spaces. I hope this explains the problem clearly enough for most people to understand. But the gist of it all is just remember to keep your campaign name and map names relatively short when using the save/load trigger system.

...Back to the show......

When creating your three maps, I recommend you use different tile sets so that you can tell them apart as your hero unit transitions (travels) among them. You can use a standard hero for simplicity, or create a custom one like I did in the tutorial campaign. If you use a custom hero, remember to copy the custom unit to your other maps or to the campaign custom data table so that your hero will be available throughout the campaign. Now, to create a full-mesh, you need a transition location that coincides with every other map in the campaign.

Since you are using three maps in this tutorial, you need two transition locations on each map. If you were to use four maps, you would need three transition locations on each map, and five maps would require four locations per map, and so on. These locations can be indicated with a cave entrance, a forest clearing, a building, or whatever suits your purposes. The idea is to indicate to the player that this is a location where something different is going to happen. I used cave entrances in the tutorial maps.

I recommend you place a hero, either standard or custom, on map 1 to use for your character. Later you can devise your own method of placing a unit triggers if you like, but pre-placing a unit is easier for now. Go ahead and decorate your maps as you see fit with terrain and doodads. It is a good idea to place some creeps to fight and some items to pick up, so that you can clearly see how the changes to you're hero are carried from map to map.

Once you are done decorating the maps and placing your hero, the last task in the terrain editor is to place two special regions near each of your transition locations. You need a 'To Map X' region and a 'From Map X' region at each location. So, on Map 1, you will have a location to transition to Map 2 that will need a 'To Map 2' region and a 'From Map 2' region nearby. You will also have a transition location for Map 3 and accompanying 'To' and 'From' regions. Make sure these regions DO NOT overlap or you will have problems. Place your 'To' regions near the target transition location (cave entrance, pub door, etc.), and your From locations a little farther away. This is so your unit can walk away from the arrival point without having to walk through the 'To' region. That would cause unlimited frustrations!

Now, on to the meat of the system!

Now for the actual triggers: leave the Initialization trigger in place. You can leave or remove the Melee Game actions as you see fit, but make sure you take out 'Melee Game - Enforce victory/defeat conditions (for all players)' in all maps. In addition to Initialization, you need to create four other triggers in all maps. Just create the triggers for now. You will add Events and Actions in a bit. In Map 1, you will need the following:

Initialization

Subsequent Starts

Go to Map 2

Go to Map 3

Load
Map 2 and Map 3 will have the same triggers, except in Map 2 use 'Go to Map 1' and 'Go to Map 3', and in Map 3 use 'Go to Map 1' and 'Go to Map 2'.

Variables

Name: CommonCache

Type: Game Cache

Value: None

- Self-explanatory; you can use multiple caches to track different data, but it's simpler to use one cache with different Headings for different data.

Name: LeftFromMap

Type: Integer

Value: 0

- A simple integer that holds the map number you just left. This way the game knows which 'From' region to place your unit in when the next map loads.

Name: LoadingFrom

Type: Region

Array: 2

Value: None

- used to convert the LeftFromMap variable from an integer to a region

Name: Map2NewGame

Type: Boolean

Value: True

- This is a special requirement of this transition system. The game needs to know always when it must change to a new level (map that hasn't been visited yet) or load a previously saved map. You need one of these for every map in your campaign except Map 1. Since you start the campaign on Map 1, you will always be loading a saved map when you come back to Map 1.

Name: Map3NewGame

Type: Boolean

Value: True

Triggers

Map 1 Triggers

Initialization
Events
Map initialization

Conditions

Actions
Game Cache - Create a game cache from CommonCache.w3v

Set CommonCache = (Last created game cache)

Set LoadingFrom[2] = From Map 2

Set LoadingFrom[3] = From Map 3

Trigger - Turn off (This trigger)

The triggers in this text have been reduced to the absolute minimum to make the maps work. You may notice a few that I added to the tutorial campaign, which are commented as to their use. Also, in case there is confusion, I commented the triggers in the attached campaign before the applicable actions, but in this text, the comments are after the actions.

To save some space, it's easier to create the game cache right up front.

Setting the game cache variable allows referencing the cache without using 'Last Created Cache', which could cause problems when you have multiple triggers accessing the cache.

You must set region variables so that the cache knows where to place your Hero upon returning to this map.

I'm not actually sure if turning off this trigger is necessary, but it seemed like a good safety check.

Subsequent Starts
Events
Game - A saved game is loaded

Conditions

Actions
Game Cache - Create a game cache from CommonCache.w3v

Set CommonCache = (Last created game cache)

Trigger - Run Load (ignoring conditions)

Recreating the game cache here is absolutely necessary for the system to work. It took me days to figure this out before it would work.

On map 1, the cache will be loaded only after loading a saved game. I suppose you could do all the loading actions here, but I created a separate trigger to maintain commonality with the other maps, and to allow for debugging if there is a problem.

Go to Map 2
Events
Unit - A unit enters To Map 2

Conditions
((Entering unit) is A Hero) Equal to True

Actions
Set LeftFromMap = 1

Game Cache - Clear all labels of Units in CommonCache

Game Cache - Store (Entering unit) as Hero of Units in CommonCache

Game Cache - Store (Life of (Entering unit)) as HeroLife of Units in CommonCache

Game Cache - Store (Mana of (Entering unit)) as HeroMana of Units in CommonCache

Game Cache - Store LeftFromMap as LeftFromMap of Variables in CommonCache

Game Cache - Store Map2NewGame as Map2NewGame of Variables in CommonCache

Game Cache - Store Map3NewGame as Map3NewGame of Variables in CommonCache

Game Cache - Save CommonCache

Unit - Remove (Entering unit) from the game

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Map2NewGame Equal to True

Then - Actions
Game - Save game as Saves\TransTutorialMap1.w3z and change level to TransTutorialMap2.w3x (Skip scores)

Else - Actions
Game - Save game as Saves\TransTutorialMap1.w3z and load Saves\TransTutorialMap2.w3z (Skip scores)

You can of course set the conditions to trigger the transition based on any criterion you choose (specific Hero, unit belongs to Player 1, etc.) but you should have something here to so that the transition won't be triggered by a wandering creep or something.

This tells the Cache where you are leaving from so the game will know where to place your Hero in the next map.

You have to clean out the Units category each time you transition, or you won't get the latest stats, items, etc. for your Hero in the next map. Note that we have created a variable for the game cache, but not for the category or labels. In the next steps, you should type these in by hand in the value box on the input screen. Using variables here is known to cause strange behavior. However, you can use a variable for the cache name.

Now you have stored all relevant information in the Cache.

The preceding variables are the key to the game knowing whether to load a new map or a saved game. These variables were given a default setting of 'True' in the variable editor. You need to use variables in the first field, but hand type values in the second field. Note you don't have to store the status of Map 1. You started here, so will always come back to a saved game.

This last part is critical. If you haven't visited the next map, the game calls the default Map 2. If you have been to Map 2 before, the game loads the saved map from the campaign custom saves file. Note that Map 1 is being saved to a custom saved-game folder. This is not absolutely necessary, but if you experience a problem with the game crashing to the loading screen, then try adding an action in the Init trigger to delete this saved-game folder. You could also do the condition by testing whether a saved game exists, but I found the Boolean variable test to be simpler.

Go to Map 3
Events
Unit - A unit enters To Map 3

Conditions
((Entering unit) is A Hero) Equal to True

Actions
Set LeftFromMap = 1

Game Cache - Clear all labels of Units in CommonCache

Game Cache - Store (Entering unit) as Hero of Units in CommonCache

Game Cache - Store (Life of (Entering unit)) as HeroLife of Units in CommonCache

Game Cache - Store (Mana of (Entering unit)) as HeroMana of Units in CommonCache

Game Cache - Store LeftFromMap as LeftFromMap of Variables in CommonCache

Game Cache - Store Map2NewGame as Map2NewGame of Variables in CommonCache

Game Cache - Store Map3NewGame as Map3NewGame of Variables in CommonCache

Game Cache - Save CommonCache

Unit - Remove (Entering unit) from the game

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Map3NewGame Equal to True

Then - Actions
Game - Save game as Saves\TransTutorialMap1.w3z and change level to TransTutorialMap3.w3x (Skip scores)

Else - Actions
Game - Save game as Saves\TransTutorialMap1.w3z and load Saves\TransTutorialMap3.w3z (Skip scores)

This one is the same as the previous trigger, except its for Map 3. Please see the comments in 'Go to Map 2' above, but note the differences that apply to map 3 below.

Load
Events

Conditions

Actions
Game Cache - Reload all game cache data from disk

Set LeftFromMap = (Load LeftFromMap of Variables from CommonCache)

Set Map2NewGame = (Load Map2NewGame of Variables from CommonCache)

Set Map3NewGame = (Load Map3NewGame of Variables from CommonCache)

Game Cache - Restore Hero of Units from CommonCache for Player 1 (Red) at (Center of LoadingFrom[LeftFromMap]) facing (Center of (Playable map area))

Unit - Set life of (Last restored unit) to (Load HeroLife of Units from CommonCache)

Unit - Set mana of (Last restored unit) to (Load HeroMana of Units from CommonCache)

Here the game gets the latest data.

This variable tells the game where to place the entering unit(s). They should appear at the transition location associated with the map you just left.

You must keep track of the status of every map except the one you are entering. Therefore, in Map 1 you need to keep track of every other map in your campaign. This is again so that the game will know which maps have been visited so far.

Your hero appears at the cave exit (or whatever) all ready for the next quest.

Map 2 Triggers

Initialization
Events
Map initialization

Conditions

Actions
Game Cache - Create a game cache from CommonCache.w3v

Set CommonCache = (Last created game cache)

Set LoadingFrom[1] = From Map 1

Set LoadingFrom[3] = From Map 3

Set Map2NewGame = False

Trigger - Turn off (This trigger)

Trigger - Run Load (ignoring conditions)

You must set region variables so that the cache knows where to place your Hero upon returning to this map. Note that you are setting region variables relevant only to the other maps in the campaign. There is of course no need to set a variable for LoadingFromMap2 when your are already on Map 2.

Here is one key factor. Now that you have entered Map 2 once, you set Map2NewGame to False. This way, the game will load from the soon-to-be saved Map when transitioning back here from any other map from now on.

Since you will always be entering this map from another map, you have to load the cache.

Subsequent Starts
Events
Game - A saved game is loaded

Conditions

Actions
Game Cache - Create a game cache from CommonCache.w3v

Set CommonCache = (Last created game cache)

Trigger - Run Load (ignoring conditions)

Go to Map 1
Events
Unit - A unit enters To Map 1

Conditions
((Entering unit) is A Hero) Equal to True

Actions
Set LeftFromMap = 2

Game Cache - Clear all labels of Units in CommonCache

Game Cache - Store (Entering unit) as Hero of Units in CommonCache

Game Cache - Store (Life of (Entering unit)) as HeroLife of Units in CommonCache

Game Cache - Store (Mana of (Entering unit)) as HeroMana of Units in CommonCache

Game Cache - Store LeftFromMap as LeftFromMap of Variables in CommonCache

Game Cache - Store Map2NewGame as Map2NewGame of Variables in CommonCache

Game Cache - Save CommonCache

Unit - Remove (Entering unit) from the game

Game - Save game as Saves\TransTutorialMap2.w3z and load Saves\TransTutorialMap1.w3z (Skip scores)

One difference from Map 1. Map 1 will always be loaded as a saved game when going back there. Therefore, there is no reason to test for Map 1 status. You just go there using the action above.

Go to Map 3
Events
Unit - A unit enters To Map 3

Conditions
((Entering unit) is A Hero) Equal to True

Actions
Set LeftFromMap = 2

Game Cache - Clear all labels of Units in CommonCache

Game Cache - Store (Entering unit) as Hero of Units in CommonCache

Game Cache - Store (Life of (Entering unit)) as HeroLife of Units in CommonCache

Game Cache - Store (Mana of (Entering unit)) as HeroMana of Units in CommonCache

Game Cache - Store LeftFromMap as LeftFromMap of Variables in CommonCache

Game Cache - Store Map2NewGame as Map2NewGame of Variables in CommonCache

Game Cache - Save CommonCache

Unit - Remove (Entering unit) from the game

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Map3NewGame Equal to True

Then - Actions

Game - Save game as Saves\TransTutorialMap2.w3z and change level to TransTutorialMap3.w3x (Skip scores)

Else - Actions

Game - Save game as Saves\TransTutorialMap2.w3z and load Saves\TransTutorialMap3.w3z (Skip scores)

Load
Events

Conditions

Actions
Game Cache - Reload all game cache data from disk

Set LeftFromMap = (Load LeftFromMap of Variables from CommonCache)

Set Map3NewGame = (Load Map3NewGame of Variables from CommonCache)

Game Cache - Restore Hero of Units from CommonCache for Player 1 (Red) at (Center of LoadingFrom[LeftFromMap]) facing (Center of (Playable map area))

Unit - Set life of (Last restored unit) to (Load HeroLife of Units from CommonCache)

Unit - Set mana of (Last restored unit) to (Load HeroMana of Units from CommonCache)

Map 3 Triggers

Initialization
Events
Map initialization

Conditions

Actions
Game Cache - Create a game cache from CommonCache.w3v

Set CommonCache = (Last created game cache)

Set LoadingFrom[1] = From Map 1

Set LoadingFrom[2] = From Map 2

Set Map3NewGame = False

Trigger - Turn off (This trigger)

Trigger - Run Load (ignoring conditions)

Map 3 triggers are virtually identical to Map 2. I left them in here so that you can compare them with Map 2 to note the minor differences.

Subsequent Starts
Events
Game - A saved game is loaded

Conditions

Actions
Game Cache - Create a game cache from CommonCache.w3v

Set CommonCache = (Last created game cache)

Trigger - Run Load (ignoring conditions)

Go to Map 1
Events
Unit - A unit enters To Map 1

Conditions
((Entering unit) is A Hero) Equal to True

Actions
Set LeftFromMap = 3

Game Cache - Clear all labels of Units in CommonCache

Game Cache - Store (Entering unit) as Hero of Units in CommonCache

Game Cache - Store (Life of (Entering unit)) as HeroLife of Units in CommonCache

Game Cache - Store (Mana of (Entering unit)) as HeroMana of Units in CommonCache

Game Cache - Store LeftFromMap as LeftFromMap of Variables in CommonCache

Game Cache - Store Map3NewGame as Map3NewGame of Variables in CommonCache

Game Cache - Save CommonCache

Unit - Remove (Entering unit) from the game

Game - Save game as Saves\TransTutorialMap3.w3z and load Saves\TransTutorialMap1.w3z (Skip scores)

Go to Map 2
Events
Unit - A unit enters To Map 2

Conditions
((Entering unit) is A Hero) Equal to True

Actions
Set LeftFromMap = 3

Game Cache - Clear all labels of Units in CommonCache

Game Cache - Store (Entering unit) as Hero of Units in CommonCache

Game Cache - Store (Life of (Entering unit)) as HeroLife of Units in CommonCache

Game Cache - Store (Mana of (Entering unit)) as HeroMana of Units in CommonCache

Game Cache - Store LeftFromMap as LeftFromMap of Variables in CommonCache

Game Cache - Store Map3NewGame as Map3NewGame of Variables in CommonCache

Game Cache - Save CommonCache

Unit - Remove (Entering unit) from the game

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Map2NewGame Equal to True

Then - Actions
Game - Save game as Saves\TransTutorialMap3.w3z and change level to TransTutorialMap2.w3x (Skip scores)

Else - Actions
Game - Save game as Saves\TransTutorialMap3.w3z and load Saves\TransTutorialMap2.w3z (Skip scores)

Load
Events

Conditions

Actions
Game Cache - Reload all game cache data from disk

Set LeftFromMap = (Load LeftFromMap of Variables from CommonCache)

Set Map2NewGame = (Load Map2NewGame of Variables from CommonCache)

Game Cache - Restore Hero of Units from CommonCache for Player 1 (Red) at (Center of LoadingFrom[LeftFromMap]) facing (Center of (Playable map area))

Unit - Set life of (Last restored unit) to (Load HeroLife of Units from CommonCache)

Unit - Set mana of (Last restored unit) to (Load HeroMana of Units from CommonCache)

That's it. This completes the triggers to make a 3-map campaign in which you have total freedom to travel to any map from any map. Good Luck and Good Mapping!


 Last edited: Aug 12, 2014

#### Luis Antony

#### Luis Antony

 Level 1

JoinedOct 24, 2013Messages8

It seems that this very well done tutorial but seeing is believing and I'll put on my map.

#### Veritas 117

#### Veritas 117

 Level 14

JoinedNov 17, 2010Messages1,266

This is the tutorial I used to put this in my campaign and it works great! I can travel between maps and back again flawlessly. I see a lot of people asking how this is done so I would definitely recommend this tutorial. Thanks to Ralle for posting it here and especially thanks to Mtlhead for making it.

#### Luis Antony

#### Luis Antony

 Level 1

JoinedOct 24, 2013Messages8

If so, I'll put in my campaign (under construction)

#### MantisHair

#### MantisHair

 Level 4

JoinedJun 30, 2014Messages72

Resources: 1

#### Resources
Tutorials 1

I think it's just my lack of experience (and patience when troubleshooting and debugging), but for some reason I have this problem: when I change maps, the map changes just fine, but the Hero doesn't seem to restore (as in, empty restore location when entering new map). I'm running out of patience with this (every transition system I've tried does this...)


Think I've found it.


Can't believe I made that mistake...


EDIT: Maybe it's not a mistake? There's nothing wrong with this in theory, but... In the triggers here for "Set LoadingFrom...", the regions don't have <gen> after them. This implies not using pre-placed regions. My regions are pre-placed and have "<gen>" after them. Is this cosmetic, or a real (and problematic) difference?


I wasn't even close on my previous guess. I had a filepath issue (not "43 characters", lucky me.) BTW, I thought a record of fixes I've made to my own pitfalls might be useful to other people who have problems with this.


For people who are having problems, one thing to make sure of is not having "Maps\" still in the filepath when filling in the "Maps\MapName.w3x" field, in a change map trigger.


Still having issues with a "crash to loading screen" (not "43 characters bug", even though it does this), will edit this when I fix it.


Like this guy's BOOM, still not working quite right.


 Last edited by a moderator: Aug 12, 2014

#### tichun

#### tichun

 Level 1

JoinedMar 19, 2012Messages2

Is there somebody who can prove it works?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I use it in my campaign, I can confirm that this works.

It's one of blizzards tutorials, I don't think they know how to use their own game.

http://classic.battle.net/mod/tutorials/TransitionsTutorial.xml

#### Screamernail

S

#### Screamernail

can someone create a test map with this? for the slow ones. i dont know if they would understand tough. people like me that is.

#### tichun

#### tichun

 Level 1

JoinedMar 19, 2012Messages2

Map with this to download would be great!!!

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

... copy > paste..

#### Screamernail

S

#### Screamernail

copy > paste what? im not a genius.

p.s where lasy to replie.

#### flyedrye

#### flyedrye

 Level 1

JoinedJan 19, 2019Messages4

Hello guys, i just want to learn one thing and i cant seem to be able to make the connection.

This one line in few triggers starts from Subsequent Starts - Trigger - Run Load (ignoring conditions).

In this case what is 'Load' supposed to be?

Thank you!

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

flyedrye said:

 In this case what is 'Load' supposed to be?


Click to expand...

It reloads all the data you saved in the previous map like heroes, items, quests, etc...

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Load is just the name of a trigger.

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Chaosy said:

 Load is just the name of a trigger.


Click to expand...

I thought he was asking what the trigger does and not what is it since this is very obvious ^^

#### flyedrye

#### flyedrye

 Level 1

JoinedJan 19, 2019Messages4

Thank you for the response. But it seems i don't have this trigger in my wc3 editor or idk, at first i thought it is variable that i didn't make, or i am just missing something. Is it possible to have an older version of world editor that is missing this?

#### Veritas 117

#### Veritas 117

 Level 14

JoinedNov 17, 2010Messages1,266

I think you are looking for something in the trigger editor that says run - load, but in fact you need to use run - trigger and select the triggered named "load" you are basically telling it to run a certain trigger (in this case, the load trigger).

If I have completely misunderstood your problem then disregard this.

#### flyedrye

#### flyedrye

 Level 1

JoinedJan 19, 2019Messages4

There is no such thing as a load trigger... Imgur

If someone could send a pic of where it is supposed to be it would be nice.

#### Veritas 117

#### Veritas 117

 Level 14

JoinedNov 17, 2010Messages1,266

If you look at all of the triggers as they are shown in the tutorial the last one is named "Load" it is a custom name for the trigger. If you took that trigger and renamed it to BlahBlah then instead of Trigger - Run Load it would be Trigger - Run BlahBlah.

This is about as clear as I can make it.

#### flyedrye

#### flyedrye

 Level 1

JoinedJan 19, 2019Messages4

I think i got it now, i just realized this 'Load' is not from the drop down menu but it's a whole custom made trigger,... Thanks

P.S.

Well as i continued down the road, it seems to me that this is either incomplete, something not explained, not added variable or whatever. Few confusing lines, so i cant replicate the actual trigger...

Unit - Set life of (Last restored unit) to (Load HeroLife of Units from CommonCache)

Assuming HeroLife is variable not mentioned above since i cant find anything of that sort in drop down menu, also it was gonna be way easier to upload a file so we can examine and learn. Anyway i stumble on a rock once again.


 Last edited: Jan 24, 2019

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

It will be in the dropdown menu once you add it to your map

#### BladeMaster3 (TVWK)

#### BladeMaster3 (TVWK)

 Level 5

JoinedApr 13, 2019Messages56

Resources: 4

#### Resources
Maps 4

how can i copy and paste????

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

I made a test campaign for this tutorial if anyone is interested.

Important Note : Your campaign filename must be less than 14 characters or the transition among the campaign will not work.

#### Attachments

Transition.w3n

 60 KB

 · Views: 285



#### BladeMaster3 (TVWK)

#### BladeMaster3 (TVWK)

 Level 5

JoinedApr 13, 2019Messages56

Resources: 4

#### Resources
Maps 4

Warseeker said:

 I made a test campaign for this tutorial if anyone is interested.

Important Note : Your campaign filename must be less than 14 characters or the transition among the campaign will not work.


Click to expand...

men i love you thanks so much

#### SanyaBelotsky

#### SanyaBelotsky

 Level 12

JoinedJul 7, 2023Messages62

Resources: 7

#### Resources
Maps 7

Warseeker said:

 I made a test campaign for this tutorial if anyone is interested.

Important Note : Your campaign filename must be less than 14 characters or the transition among the campaign will not work.


Click to expand...

There's a bug there, if you save the game and load it, there will be two heroes, or if you save and transition to the sub-map and then load the game, there will also be two heroes.

#### WarCortez

#### WarCortez

 Level 12

JoinedJul 5, 2014Messages556

Resources: 1

#### Resources
Spells and Systems 1

SanyaBelotsky said:

 There's a bug there, if you save the game and load it, there will be two heroes, or if you save and transition to the sub-map and then load the game, there will also be two heroes.


Click to expand...

Agreed. I think it can be solved by putting a condition into the "load saved game" trigger that checks if you have your campaign hero already and only loads if you don't.

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

SanyaBelotsky said:

 There's a bug there, if you save the game and load it, there will be two heroes, or if you save and transition to the sub-map and then load the game, there will also be two heroes.


Click to expand...

WarCortez said:

 Agreed. I think it can be solved by putting a condition into the "load saved game" trigger that checks if you have your campaign hero already and only loads if you don't.


Click to expand...

I think I may have forgotten to remove the hero when transitioning to à sub-map. This can be solved by adding this action to the trigger Go To Map X:

Unit - Remove (Entering Unit) from the game

#### WarCortez

#### WarCortez

 Level 12

JoinedJul 5, 2014Messages556

Resources: 1

#### Resources
Spells and Systems 1

Warseeker said:

 I think I may have forgotten to remove the hero when transitioning to à sub-map. This can be solved by adding this action to the trigger Go To Map X:

Unit - Remove (Entering Unit) from the game


Click to expand...

No, the hero is removed. But the "load saved game" option triggers when you manually load a save, not just during a cache.

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

WarCortez said:

 No, the hero is removed. But the "load saved game" option triggers when you manually load a save, not just during a cache.


Click to expand...

Add this condition to the "load saved game" trigger.

(Map transitioned is set) Equal to True

#### WarCortez

#### WarCortez

 Level 12

JoinedJul 5, 2014Messages556

Resources: 1

#### Resources
Spells and Systems 1

Warseeker said:

 Add this condition to the "load saved game" trigger.

(Map transitioned is set) Equal to True

Click to expand...

That seemed to do the trick. However, I've noticed something else in your triggers. Shouldn't that be 3 instead of 2?

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

WarCortez said:

 That seemed to do the trick. However, I've noticed something else in your triggers. Shouldn't that be 3 instead of 2?View attachment 482093

Click to expand...

Yes.

#### SanyaBelotsky

#### SanyaBelotsky

 Level 12

JoinedJul 7, 2023Messages62

Resources: 7

#### Resources
Maps 7

When I decided to add the same thing to my campaign based on this article, I noticed this bug, but I added something and it works fine. The heroes are not duplicated. I decided to add a condition for loading the game (if it's a transition between maps). You can use any, for example, whether this variable hero is hidden, and hide it in the triggers instead of deleting it (when transitioning between maps). Then, when the transition to the map occurs, I first delete the hidden variable unit and then restore it through the buffer

#### AGTwaves

#### AGTwaves

 Level 1

JoinedFeb 2, 2025Messages1

Thank you for this tutorial, but i have one problem, if i save game from game menu while my character is in map 2 or 3, and then i load it, game also load data from last transition and there will be two character, what must i do?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Always post triggers with trigger tags otherwise it's too vague to help

###
 How To Post Your Trigger


TopHow To Post Your Triggers I've seen many members across the site—especially newcomers—who need help with their triggers but don't know how to properly post them in a thread. Posting your triggers clearly is very important when you're asking others for help in fixing or identifying issues...

 www.hiveworkshop.com


#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

AGTwaves said:

 Thank you for this tutorial, but i have one problem, if i save game from game menu while my character is in map 2 or 3, and then i load it, game also load data from last transition and there will be two character, what must i do?


Click to expand...

Try adding this condition to "Subsequent Starts" trigger:

Map transitioned is set Equal to True

It's under Boolean Comparison.

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 transition among maps


alexanderlow00

Mar 22, 2015

Triggers & Scripts

Replies0Views2K

Mar 22, 2015
alexanderlow00

Locked

 regions in map issue


Momowarlord

Jan 6, 2023

Triggers & Scripts

Replies7Views1K

Jan 8, 2023
Momowarlord

Locked

[Trigger]
 Game Cache problem


Chaosy

Jan 12, 2014

Triggers & Scripts

Replies6Views2K

Jan 13, 2014
Chaosy

Locked

[Trigger]
 Submap Question P.S My Rep is Magic


Atticus

Feb 22, 2009

Triggers & Scripts

Replies2Views1K

Feb 22, 2009
Atticus

Locked

[Trigger]
 Save system - Multiple save files


FeelsGoodMan

Nov 1, 2024

Triggers & Scripts

Replies0Views751

Nov 1, 2024
FeelsGoodMan

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