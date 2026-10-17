---
type: vendor-doc
package: video-warseeker
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=NDlVMJNz32k
source_url: video
extractor: manual captions (en)
exported: 2026-09-05
duration_s: 1514
chapters: 5
timestamped: yes
---

# Warcraft 3: AI Script Tutorial - Part 1 - Basics

Source: <https://www.youtube.com/watch?v=NDlVMJNz32k>

Uploader: Warseeker | Duration: 25m14s | Captions: manual (en)

## Description

This tutorial shows how to make an AI script for a Warcraft 3 map or campaign.

Download JASSCRAFT:
https://www.hiveworkshop.com/attachments/jc113-zip.105003/
Download JASS SYNTAX CHECKER:
https://www.hiveworkshop.com/threads/jass-syntax-checker-0-1-7.62888/
Download Common.ai:
http://jass.sourceforge.net/doc/api/common_ai-source.shtml

Other settings that you can add below CampaignAI() include :

call SetGoldPeons(Integer) : How many workers harvest gold. By default it's 5.
call SetWoodPeons(Integer) : How many workers harvest lumber.
call SetTargetHeroes(boolean) : AI will priotize enemy heroes.
call SetHeroesFlee(boolean) : Makes heroes flee when they are seriously damaged.
call SetUnitsFlee(boolean) : Makes units flee when they are seriously damaged.
call SetGroupsFlee(boolean) : Makes entire group flee when they are seriously damaged.
call SetHeroesBuyItems(boolean) : Heroes will buy items.
call SetHeroesTakeItems(boolean) : Heroes will pick up items on the ground.
call SetDefendPlayer(boolean) : AI will defend allies.
call DoCampaignFarms(boolean) : AI will build extra farms when it runs out of food

#Warcraft3 #WorldEditor #Tutorial #AI

## Chapters

- [0:00](https://www.youtube.com/watch?v=NDlVMJNz32k&t=0s) <Untitled Chapter 1>
- [0:25](https://www.youtube.com/watch?v=NDlVMJNz32k&t=25s) Create a Global Variables
- [3:17](https://www.youtube.com/watch?v=NDlVMJNz32k&t=197s) Hero Code
- [15:28](https://www.youtube.com/watch?v=NDlVMJNz32k&t=928s) Attack Multiple Players
- [17:13](https://www.youtube.com/watch?v=NDlVMJNz32k&t=1033s) Syntax Checker

## Transcript

**[0:00](https://www.youtube.com/watch?v=NDlVMJNz32k&t=0s)** Hello guys, in this tutorial I want to show you how to make an AI Script for your map. To get started, you need the text editor such as notepad, but I strongly recommend using this tool called Jasscraft because it's helpful and gives you syntax highlighting.

**[0:18](https://www.youtube.com/watch?v=NDlVMJNz32k&t=18s)** So go ahead and create a new document in notepad or jasscraft, the first thing we need to do is to create a global variable that tells the AI which player it will attack.

**[0:35](https://www.youtube.com/watch?v=NDlVMJNz32k&t=35s)** Give this player a name, for example "user" and the player slot or the player number, in my example the AI will be player blue, I want player blue to attack player red, so the value will be 1.

**[0:58](https://www.youtube.com/watch?v=NDlVMJNz32k&t=58s)** Now we will start the AI call CompainAI initialize and configures the AI's behavior like building structures and training units. This function takes two arguments, the farm's ID and the hero code, each race in in the game uses a specific building to generate food, for example human uses farms, orcs uses borrow, undead uses ziggurats and night elves uses moonwell. So how do we find this farm ID? Well there are two options

**[1:47](https://www.youtube.com/watch?v=NDlVMJNz32k&t=107s)** : You can either go to the Object Editor Press Ctrl + D Select the farm and copy this four digit code Or you can use directly constant names Constants name are like variables that refer to the unit's raw code or ID, like instead of using the raw code you can use the constant name directly.

**[2:28](https://www.youtube.com/watch?v=NDlVMJNz32k&t=148s)** You can find more about constant names in this file called Comman.ai As you can see all units are shown in their constant names that refer to their ID or raw code But remember that not all units have constant names, for example custom units or neutral hostile units don't have constant names, so you will have to use raw code or create their constants names manually, I will explain more in the next video.

**[3:14](https://www.youtube.com/watch?v=NDlVMJNz32k&t=194s)** Next is the hero code, this hero code is used for leveling hero abilities, I will explain what it does later now just leave it null.

**[3:37](https://www.youtube.com/watch?v=NDlVMJNz32k&t=217s)** SetReplacements tells the AI how many times it will replace preplaced units when they are killed.

**[3:44](https://www.youtube.com/watch?v=NDlVMJNz32k&t=224s)** For example, if the AI has 3 pre-placed footmen, when these footmen are killed, the AI will replace each one of them depending on the value you set here which means easy normal and hard, for example on normal difficulty the AI will replace each of these footmen two times.

**[4:03](https://www.youtube.com/watch?v=NDlVMJNz32k&t=243s)** It will train them and order them to move to this exact location. SetReplacements only works on pre-placed units, which means units that are created in the game are not affected.

**[4:23](https://www.youtube.com/watch?v=NDlVMJNz32k&t=263s)** SetSlowChopping (true or false) this functions if it's enabled the AI will only have harvest one gold or lumber at the time instead of normal harvesting, this function is enabled by default.

**[4:45](https://www.youtube.com/watch?v=NDlVMJNz32k&t=285s)** SetPeonsRepair whether workers should repair damaged structures or not, this function is enabled by default when playing on hard difficulty.

**[4:57](https://www.youtube.com/watch?v=NDlVMJNz32k&t=297s)** You can add many parameters here as you want but I will not show them all here. I'll explain them outside the video.

**[5:08](https://www.youtube.com/watch?v=NDlVMJNz32k&t=308s)** Now we will set up the build order.

**[5:17](https://www.youtube.com/watch?v=NDlVMJNz32k&t=317s)** SetBuildUnitEx takes 4 arguments, the amount on easy, normal, and hard and the unit's ID For the unit's ID it's th same as the farm's ID, you can either go to the object editor and use the raw code or use constant names.

**[7:04](https://www.youtube.com/watch?v=NDlVMJNz32k&t=424s)** Also make sure that when you set up the build order that you respect the tech requirement, for example the Arcane Sanctum requires Keep, so it must be below the Keep.

**[7:44](https://www.youtube.com/watch?v=NDlVMJNz32k&t=464s)** Now we will build some defenders.

**[7:58](https://www.youtube.com/watch?v=NDlVMJNz32k&t=478s)** This function also takes 4 arguments, the amount on easy, normal, hard and the unit's ID Now we'll make the attack waves but before that we'll create this function WaitForSignal what it does is that it cuts the AI in half, which means everything that we type below this line will do nothing until we send a signal from the trigger editor like this.

**[8:54](https://www.youtube.com/watch?v=NDlVMJNz32k&t=534s)** for example 0, 0 This function is useful if you don't want the AI to attack directly for example you just want to build units and train defenders then later on it will send the attack waves.

**[9:21](https://www.youtube.com/watch?v=NDlVMJNz32k&t=561s)** Now we'll make the attack waves Now we'll order this attack wave to attack the player. SuicideOnPlayerEx this function takes 4 arguments, the duration on easy, normal, and hard and the player, the player it will attack. For the duration you can use variables or you can type the value directly.

**[10:09](https://www.youtube.com/watch?v=NDlVMJNz32k&t=609s)** For example 30.

**[10:18](https://www.youtube.com/watch?v=NDlVMJNz32k&t=618s)** M1 means 1 minute M2 means 2 minutes M3 means 3 minutes and the player, the player that we have created here We'll create another attack wave.

**[11:39](https://www.youtube.com/watch?v=NDlVMJNz32k&t=699s)** Now we will make some upgrades This function also takes 4 arguments, the level on easy, normal, and hard and the upgrade's ID Now we'll create the third attack wave You can create as many attacks waves as you want but after the last wave the AI will no longer set attack waves. If you don't want that, you can create a loop and put attack waves inside it That's it, we have successfully created the AI

**[15:27](https://www.youtube.com/watch?v=NDlVMJNz32k&t=927s)** Now if you want the AI to attack multiple players or for example if you want an attack wave to attack a specific player for example attack wave 1 will attack player A and attack wave 2 will attack player B, to do that we create another player variable name it for example "target" for example player purple here we just need to change the player name Also if you want to use one difficulty, you can remove this Ex

**[16:20](https://www.youtube.com/watch?v=NDlVMJNz32k&t=980s)** then type one value and the unit's ID Now we are ready to test this AI but before that we first need to make sure that this AI is working properly First we save this AI for example i'm gonna name it Custom ai To test if this AI is working correctly and there are no bugs in it, we use this tool called jass syntax checker select AI script and check if there are no issues in it you will see this message

**[17:40](https://www.youtube.com/watch?v=NDlVMJNz32k&t=1060s)** Now we can test this AI It is important that the AI has enough resources You can either give it a lot of gold during map initialization or periodically add it gold or lumber I'll make another trigger to tell the AI to send attack waves Let's use this cheat to speed up the progress I'll give the attack order
