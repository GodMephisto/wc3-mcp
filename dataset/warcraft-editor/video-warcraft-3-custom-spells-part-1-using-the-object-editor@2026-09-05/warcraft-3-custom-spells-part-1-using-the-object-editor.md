---
type: vendor-doc
package: video-keytorch
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=tYzj_dOdLjw
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 602
chapters: 10
timestamped: yes
---

# Warcraft 3 - Custom Spells - Part 1 - Using the Object Editor

Source: <https://www.youtube.com/watch?v=tYzj_dOdLjw>

Uploader: KeyTorch | Duration: 10m02s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

To all the people that asked for this video over 1 year ago, sorry for taking so long. I hope you're still interested in Warcraft 3 mapmaking, and thanks for you patience! 

Part 2 is already finished, i just have to render and upload it. Will do it this weekend.

Music:
- Kanisan - a place for ghosts
- Provided by Lofi Records
- Watch: https://youtu.be/V5JUsy_XRNY
- Download/Stream: https://fanlink.to/MistShadowsAndI


Timestamps:

0:00 Intro
0:10 Overview
1:44 Starting to create a (AoE) spell in the Object editor
1:51 Creating a New Custom Ability...
4:20 Testing our new custom spell
4:58 Going through how the spells are organized
5:56 Going through the most important fields of a spell
6:16 An overview over all the different lightning effects
6:47 Continuing to go through the most important fields of a spell
9:42 Outro

## Chapters

- [0:00](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=0s) Intro
- [0:10](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=10s) Overview
- [1:44](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=104s) Starting to create a (AoE) spell in the Object editor
- [1:51](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=111s) Creating a New Custom Ability...
- [4:20](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=260s) Testing our new custom spell
- [4:58](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=298s) Going through how the spells are organized
- [5:56](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=356s) Going through the most important fields of a spell
- [6:16](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=376s) An overview over all the different lightning effects
- [6:47](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=407s) Continuing to go through the most important fields of a spell
- [9:42](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=582s) Outro

## Transcript

**[0:00](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=0s)** foreign spells in Warcraft 3 can be really easy but it depends on what kind of spell you want to make there are three ways of making a spell the first and easiest way is through the object editor in here you can see all the existing spells that come with Warcraft 3 and from those spells you can create new custom abilities but the only thing you can do in here is just to change the values of the fields

**[0:28](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=28s)** from the spell you copied there is no way to change the mechanic of the spell or chain multiple spells together if you want to do that we will have to use triggers to enhance our spells which is the second way to make spells making these types of spells involves first making custom spells in the object editor and then enhancing them by making triggers that change the behavior of the spell when it's used

**[0:55](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=55s)** now the third way to make spells is by scripting with the Jazz scripting language but this is pretty Advanced and I don't know how to do it and it's also really not beginner friendly so I will not be covering it however when we make the triggers for our spells we'll use a few Jazz functions we will do this because the trigger system actually doesn't do the best job of cleaning up store values that are no

**[1:20](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=80s)** longer useful I'll explain this in more detail later there are many types of spells we can make single Target spells AOE spells passive abilities channeling spells self-cost bells and buff spells in this video I will only be showing you single Target and AOE spells let's begin with making a AOE spell only with the object editor find yourself an AOE spell you want to copy I will choose the blizzard for our

**[1:51](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=111s)** example right click it and choose new custom Mobility I'm just going to name it custom blizzard accept the ID there are a lot of fields we can change values of I will go through the most useful in a bit but first let's change the area of effect of the spell as you can see we can set the different values for different levels I will change the AOE for every level now let's rename the map and we'll create a custom hero

**[2:43](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=163s)** remove its existing abilities and give him our custom blizzard now open the unit palette and place the custom Paladin into the map now we'll create our region and we're going to use this region to spawn some XP books so we can level up the hero open the trigger editor and edit the melee initialization trigger at the set the time of day action and set it to noon so we get some more light in the map to be able to level up our character

**[3:53](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=233s)** I'll just add a trigger that will spawn XP books every two seconds in the region that we place in the map save and let's test our custom spell now you can see the area of effect of the level 1 blizzard is pretty small when we get to level two we can see it as double in size when we get to level six now we can see it has doubled in size again and it's uh pretty huge right now let's take a look at the

**[4:58](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=298s)** fields I will first show you how the Spells are organized in the folders on the left we can see all the spells that comes with Warcraft 3. they're all separated into folders for each race if we expand the folders you can see the Spells are then again separated into spells for units and spells for Heroes the difference between unit and hero spells is who can access them Heroes can access both hero spells and

**[5:25](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=325s)** the unit spells while units can only use uni spells but if you want the normal unit like a peasant for example to be able to use a hero spell create a custom unit of the peasant and turn it into a hero all you have to do is to put a capital H in the beginning of the ID the custom unit will then be registered as a hero unit as you can see it now has access to both the hero and unit abilities now let's go through the most useful

**[5:57](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=357s)** fields for the Spells I will start at the top and show you examples of what they're used for the art icon research field is used for setting the icon you will see when you're in the spell upgrade menu for the heroes the art icon normal field is used to set the icon you will see in the normal window where you can see spells that are available here's an overview over all the different lightning effects this

**[6:21](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=381s)** only work for single Target spells the next field is the art missile Arc field it gives you control over how curved the path of the spell is I also changed the prop of the object which you can do with the missile art field next up is the missile speed field it gives control over how fast the missile travels this field only works on single Target spells beware that this changes the mechanic of the spell it will now take longer before

**[7:14](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=434s)** the enemy is hit so we just looked at the missile speed field as you can see here I'm looking at the blizzard spell and the missile speed is set to zero this is because this is an AOE spell therefore this field doesn't do anything it only works for a single Target spells now the next field I want to touch on is the stats hero ability field this is used to categorize the spell since the blizzard spell is meant for

**[7:43](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=463s)** Heroes this checkbox is checked and the spell is therefore in the heroes folder if I were to uncheck it it would be put in the units folder but it's nice to keep things properly organized so we'll set it back to True again the next field is the stats levels field this determines how many levels the spouse will have and you can have up to a maximum of 100 levels now the tech tree requirements field is

**[8:13](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=493s)** used if you want the player to be required to research a technology before you can upgrade the spell now I want to quickly mention the target's allowed field which is important to know about make sure your spell works as you want it to when you edit this field you will see a bunch of check boxes you have to check the boxes of the units you want the spell to be able to hit at last we will go over the text Fields

**[8:39](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=519s)** if we set the stats levels to 1 we can see a better overview over them the first field text editor suffix is used to organize your spells it adds whatever you type in here to the end of the name of the spell we will use this later when we enhance our spells with triggers the next three are pretty self-explanatory the text Dash name is what you see in the left menu here in the object editor the next four fields are mostly used

**[9:08](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=548s)** with just scripting I believe so don't worry about these the last six is used for the text to see when you hover your mouse cursor over the Spells at different times in the game the text dash tooltip dash learn Fields show you the text when you learn the spell for the first time the dash extended version of these fields show you the text in the lower part of the window the normal Fields is for when you've

**[9:33](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=573s)** learned the spell the turnoff fields are for spells that can be deactivated such as the emulation spell now I've shown you the most useful things that you need to know when you want to make a spell in the object editor I'm going to split the video into two parts so this one won't become too long so go on to Part 2 if you want to learn how to make trigger enhanced spells which is kind of really cool

**[9:59](https://www.youtube.com/watch?v=tYzj_dOdLjw&t=599s)** actually thank you so much for watching
