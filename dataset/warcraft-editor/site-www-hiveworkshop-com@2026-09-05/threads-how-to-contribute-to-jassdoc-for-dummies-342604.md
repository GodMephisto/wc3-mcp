---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-contribute-to-jassdoc-for-dummies.342604/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Contribute to Jassdoc for Dummies
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-contribute-to-jassdoc-for-dummies.342604/>
#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

### What is jassdoc?​
Jassdoc (lep's explanation, source & viewer; moyack's) is the community's collaborative attempt to do something a multi-billion (or smol million?) corporation couldn't afford a budget for: documentation for the part of the game, the part that made it live on for almost a decade on its own. Thanks to the custom games. And more specifically, it's about the game API, the code, that's hidden behind WorldEdit's GUI triggers. Haha and yes, I will continue to sound cynical until the doc is complete and will help to create bigger and better map projects - faster.


 Unryze said:



 Multi-dollar company...


Click to expand...

### What is it about​
When you create triggers in WE GUI, they're ultimately converted to JASS code for the game to execute. This code uses game's functions (aka natives) to modify the game world when you play. Although WorldEdit provides very basic descriptions, they're not enough. And people who write code without WE have nothing to rely on at all. A better documentation will result in more maps done, with less frustration and (hopefully) fewer questions asked.

My hope is to get more people on board, to help, and that's why I'm writing this tutorial.

### It's all about reverse engineering​
While the natives names are known and you can kinda guess what they do, it's not good enough.

For example, what does call Player(1) do? It calls the function Player and then? It may sound obvious, if you've dealt with JASS before. It returns a handle for the specified player. Using this handle you can manipulate the player with other functions.

Another important question: which player? If you guessed "1" is "red" then you were wrong. The player IDs start with zero; the above returns a handle for player blue.

That's what it comes down to. Look at undescribed functions, test them, play with them and describe what they do. Write it down in a way that's easily understood without taking second guesses. And sometimes you'll find and describe bugs too. In a way, you'll be a tester that Blizzard never had lol

You'll be writing tiny pieces of code to test the natives. I bet you'll find programming is not as intimidating (hard) as you thought it would be.

### Simple example: BlzDisplayChatMessage​

 JASS:

native BlzDisplayChatMessage takes player whichPlayer, integer recipient, string message returns nothing

This function must be sending a text message. It takes player, a number (integer) and text (a string). We don't know what the number "recipient" does, so let's test it for player red, "boring text" and number 0.

 JASS:

call BlzDisplayChatMessage(Player(0), 0, "boring text")

As you will see, this created a chat message as if it came from player red. If you were in the first slot, it will show you as the message author. Your goal is to create a good and complete descriptions. Let's ask these questions and test them:

What happens if I change the recipient number?

Does this chat message appear in the Journal (F12)?

What happens if I give it a player that's not in the game? Like Player(25)

Do chat triggers work on these chat messages?

Does this cause a desync when executed locally?
Test and write down your findings...

This number changes the chat "channel". When you play, you can send messages to "All", "Allies", "Observers" and "Private" (to a specific player). This is what the number allows you to change about the appearance. If you join the game with two game windows, you'll notice that it has no effect on message visibility, even if set to "Private".

The messages don't appear in the Journal

The game still sends the message and the player's name is a default placeholder, literally "Player 26" (this is not a typo).

I will be honest, I didn't test this, but based on experience I can tell that it shouldn't trigger anything. I mean it doesn't appear in the Journal either.

It's an exercise for you maybe I think it shouldn't because it doesn't modify the game state/world

After testing your hypotheses you need to add your notes to the jassdoc. Maybe you'll need to rewrite them to be understandable. And to follow the jassdoc syntax too, with the @note, @param etc.

This is my resulting documentation:

/**

Displays the message in chat as if it were sent by the specified player. The message does not appear in log (F12).

@paramwhichPlayer The target player will be shown as sender of the message.

@paramrecipient Changes the type of chat channel prefix shown. It has no effect on the message's visibility.

0: "All" chat prefix

1: "Allies"

2: "Observers"

3: "Private"

4+: Defaults to private too.

@parammessage Text to show

*/

nativeBlzDisplayChatMessagetakesplayerwhichPlayer, integerrecipient, stringmessagereturnsnothing

First comes a free-form function description and what it does. Then you can use the special @tags to clarify the description. For example, @param <name> <description> clarifies the input arguments.

### A harder example: PauseUnit​
I took it from the jassdoc:

/**

Pauses a unit. A paused unit has the following properties:

 * Buffs/effects are suspended

 * Orders are stored when paused and fired on unpause

 * The paused unit does not accept powerups. UnitAddItem returns true but

 the item is not picked up

*/

nativePauseUnittakesunitwhichUnit, booleanflagreturnsnothing

At first glance it's a simple function to pause and unpause a unit, only two arguments, right? In reality you need a deep understanding of the game to find out all of these through testing:

Do buffs/effects work (auras on nearby units)?

What happens to queued orders? (Shift-clicking)

What you give items to the unit?

Actually, what else might have been missed here? I'm certain the list is not complete.

Oh and the flag is not described. Probablytrue will stand for "pause" and false for "unpause" - and this needs to be tested and added.

### A hard example: UnitUseItemPoint​

 JASS:

native UnitUseItemPoint takes unit whichUnit, item whichItem, real x, real y returns boolean

As you will read below, I had to test the following questions:

Does it abort the currently queued actions (like running) -> Yes, hence it's an "immediate" order

How does it handle different types of items? Potions: drink immediately. Dagger: jump immediately, on the spot. Infernal cast: run to the specified location and only cast it there.

What does the return value stand for? Turns out, it's broken.

And of course, don't forget to spice add an understandable description.

/**

Issues an immediate order for the unit to use item pointed at position (x,y).

This is the same as left-clicking and using an item in inventory.

Units that cannot use items will not do anything.

Examples:

- Potion of Healing 'phea':

Restores HP

- Dagger of Escape 'desc':

Casts immediately towards (x,y), even if too far, item on cooldown.

Does not cast if position is already reached (no cooldown).

- Inferno Stone 'infs':

runs towards (x,y) and once in range, casts to spawn an Infernal.

If already in range, casts immediately.

@paramwhichUnit Target unit

@paramwhichItem Handle to item instance

@paramx Point at X map coordinate to use the item

@paramy Point at Y map coordinate to use the item

@Bug Seems to always return false (tested v1.32.10).

@note See: UnitUseItem, UnitUseItemTarget

*/

nativeUnitUseItemPointtakesunitwhichUnit, itemwhichItem, realx, realyreturnsboolean

### How do I test things?​
There're two methods:

#### 1. Prepare triggers in GUI and test map to solve a question you have.​
This is slow and tiresome. The maps take a long time to load, sometimes you'll make a mistake and need to restart. In the end, you'll still need to be writing Custom Script code, because you don't know what exactly WE uses and sometimes you want to break a function deliberately by giving it bad input.

#### 2. Use Eikonium's Debug Console for Lua.​
This works great, the only downside: it's Lua. Lua isn't bad, but it's not always behaving the same as JASS. In a perfect world, we would test both Lua and JASS to understand their differences better, but we don't have a perfect world nor all the time in the world. I test using his console and you should too. Where needed, one can always create a Jass map.

#### 3. Use lep's JHCR (Jass Hot Code Reload)​
Any interest? ...

### ​

### Getting started with Debug Console (Lua)​
Download his map. It's pretty much all you need. When in-game, press Alt+A to put the game in windowed mode. Open Notepad to take notes. You are ready now.

Write -console in chat to open the code console. When it's open, every chat message will be executed as code and you'll see its output (return) there.

The map is completely empty. Let's create two units to fight each other (paste one line at a time):

 Lua:

uRed = CreateUnit(Player(0), FourCC("hfoo"), -100, 0, 90)
uBlue = CreateUnit(Player(1), FourCC("hfoo"), 100, 0, 90)

This code spawned units for red and blue (they will fight) and saved their handles to variables "uRed" and "uBlue". You can use these variables now. The following code will print the unit's localized name:

 Lua:

print("hey I can show myself text now!")
print("Unit's name is: ", GetUnitName(uRed))

Also let's find out the reason they're fighting:

 Lua:

print("Is uRed an enemy of blue?", IsUnitEnemy(uRed, Player(1)) )

Btw, that function is not described. Seems obvious when applied to fighting players, but what about the relation of any player to bj_PLAYER_NEUTRAL_EXTRA or bj_PLAYER_NEUTRAL_VICTIM? (note: constants are currently not showing up in lep's jassdoc)

Here's a little spoiler (I found while writing this): As of now, line 325 in jassdoc's "Blizzard.j" file describes possible player relations. One of them is "UNALLIED", and "ALLIED" and finally "NEUTRAL". So a neutral is indeed neither an enemy nor an ally. And this is exactly what a jassdoc for these functions should be, to make them easily understood. In this case you actually need to look further: functions SetPlayerAlliance, GetPlayerAlliance and the alliancetype type along with ConvertAllianceType and their named constants... You can look those up as an exercise

### The end for now​
I hope this is enough to get you interested and started. Continue here or here.

Topics not explained here yet:

Jassdoc's formatting is explained on its page

How to edit & create pull requests on Github. This should be easy with enough tutorials online

Basic Lua to be enough for tests

(Advanced) local editing and pushing with git, Notepad++ Jass syntax highlighting

If you have questions: ask here (preferred), ask over at jassdoc thread/repository, ask in Hive's Discord (@Warsmash...)


 Last edited: Oct 28, 2022

#### LeP

#### LeP

 Level 13

JoinedFeb 13, 2008Messages548

Big thanks from me. And if you have questions and want a real time chat I'm available on the wurst matrix channel which is linked to the wurst discord. You can find both links on the wurst homepage.

 You must log in or register to reply here.


### Similar threads

 [lua] [VScode] wc3 jassdoc access


Tomotz

Nov 21, 2025

Warcraft Editing Tools

Replies2Views619

Nov 25, 2025
Tomotz

Locked

 jassdoc


LeP

Feb 9, 2016

Warcraft Editing Tools

Replies23Views8K

Jun 24, 2023
Luashine

 Converting GUI into Efficient JASS


deathismyfriend

Apr 18, 2013

JASS Tutorials
2

Replies58Views22K

Feb 22, 2015
ZiBitheWand3r3r

Locked

[Lua]
 How to convert GetUnitGoldCost to LUA?


Bloodheaven_

Oct 19, 2023

Triggers & Scripts

Replies10Views2K

Oct 20, 2023
HerlySQR

Locked

 [Lua] Dialog Queue


Insanity_AI

Dec 3, 2022

Advanced Scripting

Replies0Views1K

Dec 3, 2022
Insanity_AI

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