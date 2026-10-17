---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/polishing-your-game.168019/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Polishing Your Game
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/polishing-your-game.168019/>
#### N1ghthawk

#### N1ghthawk

 Level 10

JoinedOct 31, 2009Messages352

Resources: 3

#### Resources
Maps 1Tutorials 2

 POLISHING YOUR GAME


There are essentially three phases of the design process. The first is general concept creation. This is essentially when you get together with your team and come up with the basic ideas that will fuel your game. The second phase is implementation. This is when you gather any resources you need and put them all together to form your game. The third phase is game polish. This is simply when you go back through everything you've done and make it better. This includes fixing errors, increasing playability, maximizing player experience, improving visuals, and overall making your game look more professional.


In this tutorial I will cover many different aspects of the polishing process. Much of what I will go over applies to any game, but this tutorial will nevertheless focus on Warcraft and Starcraft mods. Many people put their games out before they are ready. It is important to remember that polishing your game is an important part of the design process. Gamers do not want to play games that have spelling errors, are un-organized, or just in general look like they were put together by an 11-year old bashing on his keyboard.

 Interface


 Icons



Every game uses some form of icons. When dealing with icons you should attempt to follow three essential rules:

1. Attempt to limit the number of imported icons used in order to reduce filesize (especially if your map is a multiplayer map that is intended to be played over a network). Instead, use icons already available in-game as much as possible.

2. Ensure that all of your icons have their necessary counter-parts. This means both including a disabled version of your icon types as well as including different icon types for certain instances (i.e. having a passive icon for a passive ability). Do NOT use normal BTN icons for passive abilities - it looks horrible.


For those who are unaware of how to import different icon types or the necessary disabled counter-parts to certain icon-types, here is an excellent tutorial that may help you:

http://world-editor-tutorials.thehelper.net/icons.php

3. Ensure that your icons go well together. While this doesn't necessarily mean that your icons have to be the same color or be of the same thing, it does mean that you should use icons with the same art style. Do NOT have realistic icons in the same toolbar as cartoonish icons (really they shouldn't even be in the same game). It is also a good idea to make sure your icons fit the unit. Don't have magical icons for machina units or vise versa.

 Tooltips



Tooltips are a vital and noticable part of any game. Having short un-informative tooltips is one of the worst mistakes a designer can make and can often ruin the experience for a player. When writing tooltips, always ask yourself, "what information would I want to know?" and then include that data. Do NOT write ridiculously short/long tooltips or write tooltips that only include descriptions without any numerical data. Aside from that, the only rule with tooltips would be to make them as easy to read and user-friendly as possible.


There are three notable tooltip types in Warcraft and Starcraft:

UNITS - This would be the only exception to the "do not write ridiculously long tooltips" rule. Players want to know as much about the unit they are purchasing or hero they are choosing as possible before they make a choice. It is a rule of thumb to include any information that changes from unit to unit and has an impact on gameplay in the tooltip (i.e. health, mana, abilities, attack speed, attack range, etc.). Most players also like to have descriptions of the units/heroes they are purchasing (after all Warcraft and Starcraft ARE partially role-playing games).

ABILITIES - Again, I am stressing the technique of asking yourself "what would I want to know in this tooltip?". For abilities, players generally want to know things such as mana cost, damage, duration, and cooldown. Not including proper information in an ability tooltip can really unnerve a player. It annoys the hell out of me when I don't know how much an ability will cost in mana until AFTER I have learned it. Try and keep ability descriptions short (if you must include them at all) and ensure that your ability tooltips contain as much numerical information about the ability as possible.

ITEMS - Item tooltips are very simple. DO include any numerical information needed (do not just say "increases attack speed", instead say "increased attack speed by 15%"). DO NOT include long descriptions or really unnecessary information in general.

HOTKEYS - On a final note regarding tooltips, you NEED to include hotkeys for abilities or anything else players will be using in-game (in a game like Warcraft this might be purchasing units or items). Regarding hotkeys in general, you should abide by these guidelines:


- Avoid using M/S/H/O/P as hotkeys in Warcraft 3 as they are used by basic Warcraft 3 unit functions.


- Make sure the hotkey you have written in the tooltip is the one you applied to the ability (you should run through your abilities to make sure all of them correspond with what you wrote in the tooltip).


- Regarding ability tooltips, try to make it so all the hotkeys of a units abilities are located near eachother on the keyboard (don't have one ability use "Q" and another use "L" - it's just difficult and frustrating for the player).


- Regarding ability tooltips, try to focus on the left side of the keyboard as most players are right handed.


- Highlight the hotkey in your tooltip (the basic yellow is fine but I would reccomend green or red as they stand out more).


- Keep the hotkey outside of the actual name of the ability. Even though this isn't how WC3 does it, it does make it easier to see for the player and makes it look much more professional.

 Unit Information



Although not necessary at all, it is always a good idea to edit the unit information section of the user interface. Not only does this set your map apart from basic WC3 maps AND make it more visually appealing to the player, but it just makes it look more proffesional in general. Simple editable aspects of the unit-information section of the UI include (along with where you can edit them in the editor):

- Strength String (Advanced -> Game Interface -> Text - General - Strength)

- Intelligence String (Advanced -> Game Interface -> Text - General - Intelligence)

- Agility String (Advanced -> Game Interface -> Text - General - Agility)

- Armor String (Advanced -> Game Interface -> Text - General - Armor)

- Damage String (Advanced -> Game Interface -> Text - General - Damage)

- Strength Icon (Advanced -> Game Interface -> Icon - Hero Strength)

- Intelligence Icon (Advanced -> Game Interface -> Icon - Hero Intelligence)

- Agility Icon (Advanced -> Game Interface -> Icon - Hero Agility)

- Armor Icon (Advanced -> Game Interface -> Icon - Defense Type X)

- Damage Icon (Advanced -> Game Interface -> Icon - Attack Type X)

- Tooltips (Advanced -> Game Interface -> Text - General - Each Point...)

 Quest Log



I'm not entirely sure of what this is called in Starcraft 2, but i'm sure it has something similar. It is just silly to leave this blank or fill it up with useless crap. You can easily use this menu to do the following things (and more):


- Write a guide that describes how to play your game


- Include a list of the different modes/waves


- Include contact information


- Keep a record of past versions


If you are going to edit this menu (and you damn well should), please make sure you do the following:


- Change the strings of the "required" and "optional" quest fields


- Change the string of the title "quests" field


- Modify the string of the "quest log" button


The data field for the quest button name, menu title, and title of each column can be found here:


Advanced -> Game Interface -> Main Quests


Advanced -> Game Interface -> Optional Quests


Advanced -> Game Interface -> Quests


Advanced -> Game Interface -> Quests (|Cfffed312F9|R)


In order to add things to each column simply create required/optional quests using triggers (required quests will be in the left column and optional quests will be in the right):

Create Quest
Events
Time - Elapsed game time is 0.50 seconds

Conditions

Actions
Quest - Create a Required quest titled Credits with the description JetFangInferno - ho..., using icon path ReplaceableTextures\CommandButtons\BTNPeasant.blp

Quest - Create a Optional quest titled Credits with the description JetFangInferno - ho..., using icon path ReplaceableTextures\CommandButtons\BTNPeasant.blp

 Data Fields



If you are not creating a simple melee map then you need to edit or change the following fields (as needed):


- Time of day


- Gold tooltip and icon


- Lumber tooltip and icon


- Food tooltip and icon


- Upkeep tooltip and icon


You must edit these. Nothing makes you look more unprofessional than when you create a highly popular AoS-style map that's played by millions of people worlwide and you forget to change the gold tooltip to something other than "gold is obtained from gold mines" (..cough..cough..). Below is an example of properly edited data fields.

 Description



Creating a short but useful description for your game IS important. When writing yours, try and follow these guidelines:


- Include the author's name(s)


- Include a site url or name of a community for updates/matches


- Include a SHORT passage about what the game is (CTF OR AOS?)


- Include things to grab the player's attention (i.e. "over 117 items available" or "choose from 96 unique heroes)


- Include any other pertinent information


- Keep it short


- Color code (keep it organized! highlight important information/links)

 Game Version



ALWAYS include the version number in your map title. Modders/Designers will generally stick to 3 sigs for their version number (i.e. V2.34). DO NOT go past 3 sigs and do not jump ahead multiple decimal places in one patch.

 Scoreboards/statistics


 Multiboard



I cannot think of a type of game that would not benefit from having a scoreboard/progress board of some sort. Players want to know how they are doing and where they are relative to other players. If you have built your game and are ready to put it out but do not have a scoreboard built in, I would HIGHLY reccomend you implement one. If you already have one, then you should read through this list to make sure players will get the best experience out of it:

DO


- Include specific player information


- Include any necessary game information


- Include anything you would want to know if you were playing


- Make your multiboard as small as possible w/o cutting out necessary data fields


- Color code fields and include player colors


- Include a game timer

DO NOT


- Make your multiboard fill most of the screen


- Leave empty player slots


- Make it difficult, confusing, or hard to read

 End-game



Including a proper ending is an important part of the game design process. Simply b/c the game is over doesn't mean you shouldn't end it professionally. A proper ending differs from game to game (story-based shooters will obviously end differently then an AoS), however most games benefit from having either a menu that allows a player to exit manually or a statistic board that shows end-player scores and game stats.

 Visuals


 Loading Screen



Loading Screens in any game really cut both ways. On the one hand they are pretty to look at and make your map look professional if done right. On the other hand they do not effect gameplay in any way and can take up a large amount of space. In Warcraft and Starcraft there is an easy solution - use one of the basic loading screens already available in the editor. Ofcourse even then your map won't look quite as professional without its own custom loading screen. In the end, it is really up to you which way you want to go - but regardless you should always make sure your loading screen looks nice, all the text included in it is correct in spelling and grammar, and it is organized and proper-looking.


If you're not sure how to import a custom loading screen this tutorial will help you:

http://world-editor-tutorials.thehelper.net/loadscreen.php

 Preview



Most games have some sort of preview from the cover of console game cases to the selection screen on X-box live to the map list in Warcraft 3. The pros and cons are really the same as above. You don't NEED a preview but it really makes your map look much more professional if you do have one (I would reccomend using one). If you do choose to include a preview be sure to follow these tips:


- Include the name of your game in large text


- Do not steal previews of other games


- Give credit to the artist who drew/painted it


- Include the version number on the preview itself


If your not sure how to import a custom map preview then this may help you:


- Download Irfanview (located here: http://www.irfanview.com/)


- Open your image with Irfanview


- Resize your image to 128/128, 256/256, or 512/512


- Save your image as .tga


- Import your image to Warcraft 3 using the import manager


- Change the path of your image to War3mappreview.tga

 Environment



Many designers overlook this and it is a shame. Environment design/terraining are vital parts of the design process. Yes your game can look like crap and still be both popular and fun to play, but there is not a single game out there that wouldn't strongly benefit from having better terrain. If you are not a terrainer I would highly reccomend getting a professional or hobbyist to do it for you (there are tons on the Hive). If you for some reason decide to tack it yourself, please abide by these rules of thumb:

DO


- Use alot of no-collision doodads (i.e. shrubs, flowers, and mushrooms)


- Manipulate destructibles and doodads to better support the environment


- Use lightning, weather, and sky-displays to compliment your environment


- Use a variety of terrain-tiles


- Consider impact on gameplay when placing objects with collision


- Use custom effects

DO NOT


- Use blizzard cliffs unless you know what your doing


- Put things together that don't go together (technology and fantasy)


- Spam basic blizzard tree models


- Use the blizzard rock model variations that have small 2-d rocks all over the place (just looks so unbelievably horrible I cannot stress this enough)

 Abilities



There are two important rules to abide by regarding abilities:


1. Try and avoid basic object-editor un-coded abilities. There is nothing wrong with these spells, its simply that players are so used to them being in EVERY freaking game that they want something new and different. Come up with ideas for unique spells that YOU would want to have available if you were playing the game and then create them. Other options would be to come up with the idea and then get a GUI-guy or programmer to write the abilities for you or simply scroll through the Hives spell-section and get ideas from there.


2. Make your spells look good. This DOES NOT mean spam effects, but rather use the effects you do have to the best advantage of the spell. There are many unique ways you can manipulate special effects to completely change their effects. Scaling up or down an effects (to do this in Warcraft or Starcraft just apply it to a dummy unit) can completely change the appearance of the effect and ergo impact player experience. There are plenty of excellent effect models available online (i.e. here at the Hive) that can make your game visually beautiful if implemented correctly.

 Playability


 File Size



File size is very pertinent to playability. People do not want to download (for example) a Warcraft 3 map that is 8 mgs large. Your primary concern, however, should be with having a filesize that is low enough to be played on B-net and other popular gaming networks that support WC3/Starcraft such as Ghost or Garena. There are several ways you can reduce filesize without impacting gameplay:

1. Protect and/or optimize your game using an optimizer (I reccomend Verxorian's):

http://www.wc3c.net/showthread.php?t=79326


This can knock a very large percentage of your file size off (my average is about 15-20%).

2. Remove all unit portraits as they largely contribute to the end-product file size and are entirely unnecessary (though nice to have). Deleting these will not impact gameplay in any way.

3. Instead of using a custom loading screen, use one of those available in the editor (pertaining to Blizzard's World and Galaxy Editors)

4. Reduce the quality of your imported 2d images using an image edited/conversion program such as the Warcraft 3 Viewer:

https://www.hiveworkshop.com/forums/tools-560/warcraft-iii-viewer-62878/

5. Reduce the number of imported models/skins used in your map

6. Comb through your imports and see which ones are the largest. Then look for alternatives to those specific imports.

 Lag



There are countless ways to reduce lag in your game. I am just going to go over two very simple coding fixes that you can implement to reduce lag.


1. Optimize periodic systems. There is really no reason to run something every .01 seconds. Even if you are attempting to create the appearance of movement, players will not notice a different if you decrease the rate to every .05 seconds.


2. Remove leaks. There are tons of excellent tutorials online about this, such as this one here regarding basic Blizzard GUI/Jass leaks:

https://www.hiveworkshop.com/forums/trigger-gui-editor-tutorials-279/basic-memory-leaks-5889/


When your done with your game, it is always a good idea to comb through your triggers/code and fix any leaks you may have missed. One leak in a really bad place can seriously slow down gameplay end-game.

 Leaking and Not Optimized


Global Move
Events
Time - Every 0.01 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in (Units in (Playable map area)) and do (Actions)
Loop - Actions
Unit - Move (Picked unit) instantly to ((Position of (Picked unit)) offset by 1.00 towards 45.00 degrees)

 Leakless and Optimized


Global Move
Events
Time - Every 0.05 seconds of game time

Conditions

Actions
Set TempGroup = (Units in (Playable map area))

Unit Group - Pick every unit in (Units in (Playable map area)) and do (Actions)
Loop - Actions
Set TempPos = (Position of (Picked unit))

Set TempPos2 = (TempPos offset by 5.00 towards 45.00 degrees)

Unit - Move (Picked unit) instantly to TempPos2

Custom script: call RemoveLocation(udg_TempPos)

Custom script: call RemoveLocation(udg_TempPos2)

Custom script: call DestroyGroup(udg_TempGroup)

 Miscellanious


 Balance



People want balanced games. Test your game multiple times before allowing people to download it. Get some friends together and test it on a LAN so you have experience with your game on multiplayer. It is also always a good idea to release a BETA so you can get feedback on balancing and perhaps get errors reports as well.


My essential message here, is continue to play and refine your game. Balance only comes through time and continual updates.

 Player Colors



You should always color code any messages or text you use in games. However, if not anything else, you should AT LEAST include player colors. Just run this trigger in your Warcraft map and whenever you reference a player name it will be colored according to the player slot:

Player Colors
Events
Time - Elapsed game time is 0.50 seconds

Conditions

Actions
Set Player_Colors[1] = |c008B7500

Set Player_Colors[2] = |c000000FF

Set Player_Colors[3] = |c0000FFFF

Set Player_Colors[4] = |c009B30FF

Set Player_Colors[5] = |c00FFFF00

Set Player_Colors[6] = |c00FFA500

Set Player_Colors[7] = |c0000868B

Set Player_Colors[8] = |c00FF1493

Set Player_Colors[9] = |c004F4F4F

Set Player_Colors[10] = |c00436EEE

Set Player_Colors[11] = |c00006400

Set Player_Colors[12] = |c008B4513

For each (Integer A) from 1 to 12, do (Actions)
Loop - Actions
Player - Set name of (Player((Integer A))) to (Player_Colors[(Integer A)] + ((Name of (Player((Integer A)))) + |r))

 Map Protection



As stated above this is a good idea as it deletes some useless code and such to decrease your filesize. Having said that you should ALWAYS have an unprotected version available to modders/designers in the gaming community. Don't be a snob about this; no one's trying to "steal" your game, people just want to have a look at how you did things. Cloudwolf's "Diablo 3 Warcraft" is one of the most popular Warcraft 3 mods to date and includes tons of innovative and brilliant ideas and he STILL gave an unprotected version out to the community. That's how we learn, and ask any moderator on this site and they will tell you that the #1 way they got to be as good as they are (aside from practice on their own part) is by looking at what other people did (and more importantly HOW they did it).

 Sounds



It is a good idea to comb through any sounds you may have used during development and make sure the sound you are using is correct, the placement of that sound is where it should be, that sound is being heard by the correct players, and the volume of the sound is where you want it.


In WC3 and Starcraft you should particularily watch out for two things:


1. The soundset of your units (alot of times people copy and pase units and change the model but forget to change the soundset along with the model).


2. The attack sound of your unit (only use this with melee units unless you've imported some custom sounds - make sure the attack sound fits the type of weapon (heavy bash with maces, light bash with staves/wooden weapons, slice with axes/swords).

 Credits



In any game you should include credits for any resources you may have used in two places:


1. On a website or community page dedicated to your game (for most people reading this that would be your Hive map page).


2. In the game itself (for WC3 maps that would be in the quest tab).

DO


- Organize your credits list according to resource and/or name


- Include ALL material resources used regardless of how minor


- Include names of people who significantly helped you on the project


- Comb through your credits list when your done and check for spelling errors


- Keep an ongoing list of resources you use during the game design

DO NOT


- Screw up or forget someone. People spend alot of time on the resources you use - the least you could do is take the time to write a clean and correct credits list.


- Wait until AFTER you've finished your game to look back on what you've used and who contributed (sure way to forget people, lose track of resources, and a general over-all time-waster)

 Community Descriptions



Although not part of the game itself, the way you present your game to a community of gamers, modders, or other designers is important. People will not pay any attention to (much less actually play) a game with a 7-word one sentence description. Here are some general guidelines to go by when writing a proper game description for an upload site:

DO


- Include the basic type of the game (AoS, CTF, Shooter, Arena, etc.)


- Include a description of what it is


- Include the story behind the game (if you have one)


- Include screenshots (better yet a gameplay video if you can)


- Use html code to hide most of your information in order to keep your game description clean


- Organize your description into sections and use html code to box it up and seperate it


- Color code your description (highlight section titles)


- Include credits


- Include a changelog (for patches)


- Check your description for spelling and grammar errors


- Indent


- If your map is protected include a link to an unprotected version or offer to message an unprotected version to anyone who would like one (this IS an open-source community)

DO NOT


- Include paragraph after paragraph of un-organized text


- Include giant over-sized concept art that has nothing to do with the game itself


- Write a three-sentence description


- Include credits or changelogs that aren't hidden (unless they are VERY short)


- Include a list of 40 screenshots in a row that make up 95% of the height of the page (even if they are hidden)]


- Write in gigantic all-capped text


HTML-based functions that you can use to organize your description can be found here:

https://www.hiveworkshop.com/forums/misc.php?do=bbcode

 Player Feedback



On a final note, I would like to say LISTEN TO FEEDBACK. Whether you like a reviewer or what he/she had to say, their feedback can help you make your game better. Take what you can from it and act on it. Be open to ideas even if you don't agree with them and don't discount a players entire review simply because you don't like one point he/she said.


BE PREPARED TO TAKE CRITICISM. This is part of game design and if you are going to make games or even just mods for Warcraft and Starcraft you need to get used to this.


 Last edited: May 26, 2010

#### The World Is Flat

#### The World Is Flat

 Level 30

JoinedMar 9, 2009Messages6,276

Resources: 5

#### Resources
Icons 3Tutorials 2

Nice tutorial. +rep.

#### HardOn

#### HardOn

 Level 4

JoinedMay 19, 2010Messages119

a very detailed tutorial. would greatly help me and others who are still fishing in the dark when it comes to map making.

#### Septimus

#### Septimus

 Level 31

JoinedMay 3, 2008Messages3,146

Resources: 50

#### Resources
Packs 1Maps 35Spells and Systems 8Tutorials 6

good tutorial, but you need to add some specific info.


For example, detail on how to edit UI at unit information...

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Well something like this should be attached as rule (or at least this one: Community Descriptions)

If you add more info just like Septimus said (I see that it is WIP so I guess you will) it will be awesome

#### N1ghthawk

#### N1ghthawk

 Level 10

JoinedOct 31, 2009Messages352

Resources: 3

#### Resources
Maps 1Tutorials 2

 good tutorial, but you need to add some specific info.


For example, detail on how to edit UI at unit information...


Click to expand...

 If you add more info just like Septimus said


Click to expand...


I'll implement some more detailed instructions later tonight. Ty for the feedback.

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

awesome so far

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Very nice. Love the images. =)


It is definitely approvable at its current state, however, feel free to update as much as you desire. =)


~Approved.

#### Falzelo

#### Falzelo

 Level 8

JoinedDec 24, 2009Messages257

Resources: 3

#### Resources
Maps 3

Oh my god awesome tutorial

thank you, your tutor helps me to be much better

+ rep

#### Pastelito

P

#### Pastelito

Hey nice Tutorial man, i woudl like to have everything on my maps as "godlike".

 I, all the time look many tutorials here at hive workshop to see if i'm coulding of do better my maps but i just work alone couse my friends are "poop" at map making and i live in Venezuela, here i speak spanish and i don't know EVRETHING about english but i can understand almost everything that i read.

Thanks for this tutorial if i were knowing how i +rep but i don't know how xD anyway thanks this is pretty helpfull (if i were knowing how to make a half of that xD)

#### The World Is Flat

#### The World Is Flat

 Level 30

JoinedMar 9, 2009Messages6,276

Resources: 5

#### Resources
Icons 3Tutorials 2

This is pretty good, in fact, its excellent.


I love it. Just read it again.

#### jsheph03

#### jsheph03

 Level 6

JoinedJun 8, 2009Messages220

In my opinion its a good tutorial just i think you are obsess with dota @.@

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

He used Dota as example because over 12.000.000 people play that map!

#### jsheph03

#### jsheph03

 Level 6

JoinedJun 8, 2009Messages220

Can someone tell me how to do the loading screen thing? i Looked at the section and i looked at the tutorial i tried DLing a file called "Warcraft III Image Extractor II" I opened it and it says "Component 'Comdlg32.ocx' or one of its dependencies not correctly registerd: a file is missing or invaild" and i tried to fix it but it wont work, does anyone know where i can get a older or working version of Warcraft III Image Extractor II.

#### The World Is Flat

#### The World Is Flat

 Level 30

JoinedMar 9, 2009Messages6,276

Resources: 5

#### Resources
Icons 3Tutorials 2

Dude, this is such a nice tut, i would ask you to join my modding team.

#### Killerspider

K

#### Killerspider

I love this  Sad about half the things described here are rated HORRIBLE on my map :x... Thanks for taking time to write all this!

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

awesome!, this is a great help for me!

#### Raging Ent

#### Raging Ent

 Level 17

JoinedJul 15, 2009Messages632

Resources: 56

#### Resources
Icons 50Packs 3Maps 3

Horrible tut.. you put your own terrain as GODLIKE :O


Lol.


Pretty cool guide! Though I doubt it'll make future maps any better

#### Shimitar

#### Shimitar

 Level 7

JoinedDec 21, 2009Messages77

Resources: 6

#### Resources
Maps 6

This tutorial is really good, I often neglected many of the things on this list.


This is my new map checklist.

#### RadiaNSh0t

#### RadiaNSh0t

 Level 3

JoinedOct 23, 2010Messages32

Resources: 1

#### Resources
Maps 1

cool tut im introducing it to my frens

#### mikeisman2

#### mikeisman2

 Level 8

JoinedMar 29, 2008Messages71

Resources: 29

#### Resources
Icons 29

Be sure to note that the log should also include the current version and the number of previous versions made and that they are dated.


Also that they should try to make as few custom units in the editor as possible and just modify existing units in the editor as much to reduce file size and game speeds. This is due to the fact that any new unit (custom unit) created in the editor is a new object instantiated in the game along with the current preexisting units in the game. When a game loads it loads the whole list of objects preexisting and custom made into the game file. When a preexisting unit is changed its just numbers in the game that are modified and in coding terms i guess numbers are very small and do not take up a lot of space which for a game that may have a lot of custom units will slow down load time and game speeds. yes this can get confusing though because of the the directory navigation for the objects but i would suggest strongly using the editor suffixes to help you identify each object created or modified as to build a level of organization for the map.


Now when it comes to modeling yes using as few high file sized models is recommended but you should also look to exporting your imported data and using conversion tools to minimize their file size so basically using a tool to take a 400kb model file and converting it down to for examples sake 100kb this yes will reduce quality of the model but with the graphical limitations of wc3 editor i do not think it would make much of a difference.


The same goes for the object data of your units in the object manager i believe there are tools for doing so but i could be wrong. trying to minimize your object data as much as possible is very important. This can be done with your icons, textures, models, and custom music and sounds imported into the game.


Now to further optimize in-game speed and reduce crash possibilities you should look to putting a recycler into the game because as far as i know i believe when an object in-game is destroyed or removed or killed its as though it is put into for examples sake a dump imagine filling a land fill eventually that land fill will be full so in terms of your game when that land fill is full the game "crashes" to try to evade this almost inevitable situation form occurring a recycler should basically take destroyed objects store them into an array most likely a limited array so say 12 spots every time an object is destroyed it will be placed in this array this storage bin so to say. now yes that storage bin will fill up but what it will do is look to the first position of that storage bin and say hey why dont i just place this newly destroyed unit were that one is what the program will do is take the data the code associated with that object and rewrite it over the existing objects data in that position. by doing this you are reducing the amount of object data building up in your landfill so to speak. imagine taking all that trash and compressing it continuously. This is the same concept that goes along with ghost code ghost code is the remains of a file when you for example delete it . when you delete a file what happens is it is told hey here is a file this is the bits that make it up let me write something like a 0000 at the end the file become unreadable to the computer and has no verification associated to it and is recognized as "deleted" when is reality there is bits of the original file remaining that is why say you have a usb and it starts with 10 gbs and after a couple years if you delete everything off of it it has for example 9.5 gb. this is the reason.


That was a lot but i hope you understand what im saying though when it comes to optimizing your game and reducing its file size!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

@N1ghthawk seems like the images in your album have been removed (or maybe got lost at some point?). Do you happen to have any backups?

I think the tutorial can stand on its own, but I did enjoy the examples the images provided.

 You must log in or register to reply here.


### Similar threads

 Game Interface - Basics


jonadrian619

Sep 1, 2007

Misc/General Mapping Tutorials

Replies10Views20K

Jun 19, 2019
winter twilight

 In-Game Font Overhaul


Hodge-Podge

Feb 22, 2010

Misc/General Mapping Tutorials
2

Replies62Views71K

Nov 24, 2017
datsatthune

 How to: Create a Full-working Custom Race


Interceptor

Mar 4, 2009

Misc/General Mapping Tutorials
2

Replies75Views140K

Dec 23, 2020
Superfrycook

 Transitions Among Campaign Maps


Ralle

Mar 20, 2006

Misc/General Mapping Tutorials

Replies34Views42K

Feb 2, 2025
Warseeker

 How to create a unit that can level up


Alok

Jul 3, 2016

Misc/General Mapping Tutorials

Replies8Views5K

Sep 22, 2016
Alok

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