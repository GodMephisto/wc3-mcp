---
type: vendor-doc
package: video-purpleadventures666
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=ihVkbHHfCl0
source_url: video
extractor: manual captions (en)
exported: 2026-09-05
duration_s: 317
chapters: 11
timestamped: yes
---

# WC3 World Editor: How to create an AI using custom units

Source: <https://www.youtube.com/watch?v=ihVkbHHfCl0>

Uploader: PurpleAdventures666 | Duration: 5m17s | Captions: manual (en)

## Description

Wanted to make the Computer to control/train custom units and heroes...Check this tutorial to help you.
Sadly the default AI of the computer doesn't control any custom unit and we can't modify the file unless if you do by JASS. but it requires a lot of experience....That's why i'll teach you how to create an AI
00:00 Intro
00:12 Exporting Object Data
00:20 AI Editor: General Section
00:55 Building Section
01:32 Creating Expansion Town Order
02:06 Attacking Section
02:46 Saving & Importing AI
03:00 Set up AI Script for Computer Player
03:19 Make Hero learning spells by level
04:07 First Level - learn spell
04:37 Results


Music Used:
Date A Live Spirit Pledge Unknown OST
Date A Live Spirit Pledge OST - Hypedimension Battle
Date A Live Spirit Pledge OST - Lunar New Year

#worldeditor #warcraft3

## Chapters

- [0:00](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=0s) Intro
- [0:12](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=12s) Exporting Object Data
- [0:20](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=20s) AI Editor: General Section
- [0:55](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=55s) Building Section
- [1:32](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=92s) Creating Expansion Town Order
- [2:06](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=126s) Attacking Section
- [2:46](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=166s) Saving & Importing AI
- [3:00](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=180s) Set up AI Script for Computer Player
- [3:19](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=199s) Make Hero learning spells by level
- [4:07](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=247s) First Level - learn spell
- [4:37](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=277s) Results

## Transcript

**[0:00](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=0s)** Wanted a Computer player to use custom units but they don't even touch it! Check this tutorial to help you First we can't make the default AI to use custom units but we can create our own AI to any race + using custom units First go to Object Editor and go to file - Export all object data" and add any name you want then click on "save" Now go to AI Editor and click in "Import" and select the file we've created

**[0:28](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=28s)** Now we can do set anything we want for the AI. (changing the name, race, and some gameplay options) In case you're going to use custom hero units then we can just skip the part of heroes Now comes the most important part where we'll set then units which is going to use or build by the AI (which means AI must have these units/buildings in order) The 5 workers is just an example that the AI must have. Once done, the AI won't do anything more.

**[1:04](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=64s)** To add more units right click and click "add priority" and select any unit you wish the AI to produce. (Including upgrades) Remember to add the building first then the unit/upgrade else the AI will stand idle doing nothing.

**[1:19](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=79s)** Also be sure that the AI has enough food to train units. so farms are important to add else AI will stand idle doing nothing In case you want the AI to build an expansion then just add priority and in build section select "Expansion Town" also condition is required to make it working correctly right click in the expansion town priority and select "modify condition" and add an condition "Recommended condition: (Numbers of mine owned) greater than or equal to 1"

**[1:46](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=106s)** After that if you want buldings or workers to be produced in the expansion then add priority  but set the town to expasion #1 Also condition is required "(Number of mines owned) greater than 1" then we can go to Attacking Section In this part we can set waves and which units is going to fight also the delay before a wave starts and which targets the AI is going to fight First i recommend to set "Minimum forces to all units" then add a initial delay before the first wave starts to a highter number. and repeat waves to the maximum number

**[2:24](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=144s)** you can add conditions in target section (now you can use any condition you want) Now in unit type section add the custom units you wish the AI to use to fight In attack group set the waves a delay to any amount of seconds else the AI everytime a unit is trained, the unit will automaticaly ordered to fight once you set up everything then save your AI data Now import the file and you will notice you have 2 files ".ai and .wai files" delete the .wai file and go to trigger editor

**[3:00](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=180s)** Go to the default trigger and remove "Melee Game - Run Melee AI scripts for computer players"and add action "AI start melee ai script for <player> <the .ai file> In case you're still going to use the default wc3 AI then these are the codes for each race "human.ai / orc.ai / undead.ai / elf.ai" Then save your map and test it!!

**[3:19](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=199s)** In case the AI will use a custom hero and you want it to learn a ability then create a trigger and add event "Unit gains a level" Condition "Unit type of (triggering unit) is equal to <unit> Actions: If then else If conditions: "Level of (triggering unit) equal to <number>" Then - Actions: Hero learn skill for (triggering unit) <ability>. Add if then else for every level that the unit can have + same conditions(which different level number) and the skill that the unit is going to learn.

**[4:04](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=244s)** For the first level, create a new trigger with the event "A unit finishes training a unit" Conditions: "Unit type of (triggering unit) equal to <unit>" Actions: "Hero learn skill for (triggering unit) <ability>" Now save your map and test it!

**[4:38](https://www.youtube.com/watch?v=ihVkbHHfCl0&t=278s)** Here is...The AI is building different constructions and units + different construction route! Hope this video helps in your map
