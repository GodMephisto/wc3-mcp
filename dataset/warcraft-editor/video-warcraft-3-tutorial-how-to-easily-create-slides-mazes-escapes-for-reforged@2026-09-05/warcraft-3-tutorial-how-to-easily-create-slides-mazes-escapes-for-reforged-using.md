---
type: vendor-doc
package: video-fieryfox
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=ePhY5laYC-E
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 2060
chapters: 10
timestamped: yes
---

# Warcraft 3 Tutorial: How To Easily Create Slides/Mazes/Escapes For Reforged Using MEC

Source: <https://www.youtube.com/watch?v=ePhY5laYC-E>

Uploader: Fieryfox | Duration: 34m20s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

To use Max Escape Creation by Maximaxou to create a slide / escape / maze you will need some tools:

Warcraft 3 Patch 1.31: https://www.hiveworkshop.com/threads/wc3-download-archive-1-00-1-31-1-beta-demo.326836/ or https://mega.nz/file/S51xQZSZ#HpoZTTyOy2GKDa0zepxeDpcuvGWVNPdmVt7GTKItvG4 

MEC site: http://max.slid.free.fr/maxEscapeCreation/ 

MEC map MaxEscapeCreation v1.3b edit: https://www.epicwar.com/maps/320371/

MPQ Master: https://www.hiveworkshop.com/threads/mpq-master-v1-2-0.62935/ 

Cache Converter: https://www.hiveworkshop.com/threads/war3-cache-converter-1-0.66432/

The line you have to add in JassHelper conf file is: [doreturnfixer]
If you struggle with opening the patch 1.31 World Editor, then follow these steps:
- Open the Registry Editor - press Windows Key + R. Type in "regedit" (without quotes) in the window that opens and press Enter.
- In the Registry Editor, navigate to: HKEY_CURRENT_USER\Software\Blizzard Entertainment\WorldEdit.
- Delete the “Tool Windows” key

Hope the video inspired you to make slides :)

## Chapters

- [0:00](https://www.youtube.com/watch?v=ePhY5laYC-E&t=0s) <Untitled Chapter 1>
- [3:06](https://www.youtube.com/watch?v=ePhY5laYC-E&t=186s) Required Tools
- [5:20](https://www.youtube.com/watch?v=ePhY5laYC-E&t=320s) Prepare a Map for a New Escape
- [5:44](https://www.youtube.com/watch?v=ePhY5laYC-E&t=344s) World Editor
- [6:13](https://www.youtube.com/watch?v=ePhY5laYC-E&t=373s) The Registry Editor
- [7:52](https://www.youtube.com/watch?v=ePhY5laYC-E&t=472s) Modify the Map Bands
- [8:35](https://www.youtube.com/watch?v=ePhY5laYC-E&t=515s) Change the Map Name
- [18:50](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1130s) Create Monsters
- [23:44](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1424s) Create a Level
- [29:51](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1791s) Trigger Editor

## Transcript

**[0:00](https://www.youtube.com/watch?v=ePhY5laYC-E&t=0s)** hello there this is a tutorial on how to use mac uh to create slides in warcraft 3 itself without having to use the world editor much at all max stands for max escape creation is two made by maximixo and in principle the idea is that you don't have to use the world editor much you can create the terrain and the units inside the map itself and once you're done with creating stuff you can save your progress by exporting

**[0:32](https://www.youtube.com/watch?v=ePhY5laYC-E&t=32s)** all you created to some text files outside the game then putting the content of that into world editor and then into your map the world editor is then actually only used to apply some finishing touches so how to start well since you are unable to create slides with mac in reforge you will have to use an older warper 3 patch there are multiple versions of mac but the ones you should choose from are versus 1.2 d

**[1:08](https://www.youtube.com/watch?v=ePhY5laYC-E&t=68s)** and 1.3 b 1.2 d can be used for walker 3 patch 1.26 and higher well 1.3 b can be only be used in patch 1.31 and higher since 1.3b is newer this tutorial is about how to use that one but the ideas are actually the same for one put for the 1.2 d version so first to start is you will need to use the patch 1.31 or worker 3. you can down and you can download it from this website you can go to game files and then you

**[1:45](https://www.youtube.com/watch?v=ePhY5laYC-E&t=105s)** see a download link for 1.31.1 the link for this website is also in the description for them um then the next thing you do is you open this the walker 3 1.3 31 in your folder and you go to jazz helper the folder js helper and for me i use the x86 uh underscore 64 since i have a 64-bit system but it doesn't really matter much so once you're in just helper you open it and you get into here and you see three lines

**[2:26](https://www.youtube.com/watch?v=ePhY5laYC-E&t=146s)** and you basically have to add one more line of code to this and that's uh what you have to add is uh open brackets do return fixer just without spaces and then close brackets and then you save this right you need to do this if you want to save your uh macmap inside world editor next you go to the uh max escape creation website makes by max pixel this is the website you will not use for making uh your slide in game with mac

**[2:58](https://www.youtube.com/watch?v=ePhY5laYC-E&t=178s)** uh because there's a whole tutorial on how to you know to use all the commands and what commands are uh first you go to like the required tools and let me just guide you through here because you don't need everything uh first you just new generation pack uh you don't need it all because this 1.31 patch of worker 3 doesn't it's not compatible with this so instead of that you use the work worker 3 patches

**[3:30](https://www.youtube.com/watch?v=ePhY5laYC-E&t=210s)** 1.31 world editor which can be found in your folder then you need the base map max escape creation however this version is as few bucks in it the 1.3 b version that we use in this tutorial so instead i released a fixon box with the help of max maxwell and released a new version an edited version that you can use the link is in the description as well then you need to mpq master well this link also doesn't work anymore

**[4:06](https://www.youtube.com/watch?v=ePhY5laYC-E&t=246s)** so there's also a link in the description of this video that you can use where you can download it you need the walker three cache converter and it's the same story here you doesn't work so you need to download it uh from this side also in the link in the description uh then you will need the just generator but this one does work and you basically get the 1.3b version which is the one then the map converter doesn't work for

**[4:41](https://www.youtube.com/watch?v=ePhY5laYC-E&t=281s)** patch 1.31 so you don't need it because there's uh it's combined every map you make and you uh played in the frozen throne and since in a past 1.31 you get both then the worker 3 map optimizer doesn't work either and instead if you really want to protect your map you need you need to download the newer version which can be found on the internet but i don't think your map will ever be stolen so i don't

**[5:10](https://www.youtube.com/watch?v=ePhY5laYC-E&t=310s)** really think it's necessary but it's up to you next you go to the tab prepare a map for a new escape as you can see uh the first thing you gotta do is you get the you downloaded the base map and you need to make a copy of this so here you go copy and you rename it to your slides which is for me it's the slide let's just then you open the world editor and this might actually give some problems to you uh because if

**[5:57](https://www.youtube.com/watch?v=ePhY5laYC-E&t=357s)** you uh worked in a world editor in the reforged app then this one will not open unless you change the registry in the description of this video there's a there's some a guide on how to to do this it's really quick you basically open the registry editor through the windows key plus r and when you where you type in the red and red edit then you navigate to h key of current user backslash software backslash blizzard

**[6:28](https://www.youtube.com/watch?v=ePhY5laYC-E&t=388s)** entertainment backslash world edit and you find the key to windows and you just delete that key and then if you've done that you can open the world editor then you open the map your map and test light and then you wait a bit and you can see uh there's not really damage in it it's actually almost empty but if you uh the first thing you gotta do is go to the trigger editor which you do not have to really use much

**[7:04](https://www.youtube.com/watch?v=ePhY5laYC-E&t=424s)** but for there's just one thing you've got to change on the top you see in the first live we see the trigger constants you open this and you see the constant real low period for work you change this value from 0.0001 to 0.005 which is like a 50 times increase and then you save them by the way if you forget i forgot to step about jess helper then you will not be able to save the map but i did save implemented this step so it's safe

**[7:43](https://www.youtube.com/watch?v=ePhY5laYC-E&t=463s)** then you go to the first and you want to do a few things and you can see this on the website is to increase the size of the train you can go and modify the map bands let me just guide you through that i think it's a scenario map size and camera balance it has to be um this this number has to be a multiple of 32.

**[8:04](https://www.youtube.com/watch?v=ePhY5laYC-E&t=484s)** so you click click on this and then you just change well you can get it through to 64 times 64 for example i recommend to use a 160 by 160 if you want to use about eight levels of decent size and you press ok and next step is you have to change the map name it's important then you've got scenario and map description and you call it the slide for me okay then you can also change uh the red color that you see in the upkeep

**[8:53](https://www.youtube.com/watch?v=ePhY5laYC-E&t=533s)** it's on the top right when you enter the game uh where normally there's the value by maximum soul but you can change it to your own author name however i'm not going to show that basically everything is in the on a website if you want to change it so let me save and before i forget we need to do one more thing and that is in the terrain pilot we need to use make a starting area first we want we don't want this desert

**[9:30](https://www.youtube.com/watch?v=ePhY5laYC-E&t=570s)** terrain so we want a different terrain then we go to advanced modified tileset we want to use a custom teleset for example and i want to use this leaf style set but you can also use everything else it doesn't really matter that much uh we want to add this down and i press okay this is a little bit laggy but then we create a starting area where your main character's your sliders will start in this is going to be our walk area so it

**[10:06](https://www.youtube.com/watch?v=ePhY5laYC-E&t=606s)** is not going to be a slight area then we go to the region palette and this is starting um the five point and if you you need to put it over your starting area so that your slide is refined from this area that we've done that we've done everything and we can go into the game and make some changes make your make the first level of the slide i can close this then we go to worker 3 the patch 1.31 version

**[10:46](https://www.youtube.com/watch?v=ePhY5laYC-E&t=646s)** uh don't worry about it's black it's just a weird look now we find here my test light started we use solo don't think it really matters but just be sure the first thing you gotta do is dash va it's a vision visibility of forever so we can see the whole uh the whole map and as you can see i spawned here on my star so to create stuff in mac we use the website again and we go to step one choose and set

**[11:32](https://www.youtube.com/watch?v=ePhY5laYC-E&t=692s)** trains and the first thing you want to do is make a walkthrough a death turin and slide drain you can have multiple slide trains different some walk drains but that's up to you you need three of them at least one of each at least i mean um well then the first thing you got to do is i want to make this my walkthrough at last and i can either find the the terrain code in the trains tab of this of the website

**[12:06](https://www.youtube.com/watch?v=ePhY5laYC-E&t=726s)** where you can find for example eshenville and you can see leaves is number 34 and then this is the code alfidi and you need this in order to specify the terrain in the map but i want a different time terrain we want to do the nice terrain the northern ice train and for the slide and the north and snow train for the tester so um we first do try to find for example if we've already made this entertrain in the world editor

**[12:47](https://www.youtube.com/watch?v=ePhY5laYC-E&t=767s)** and if i don't want to use the the website to see what kind of terrain code this is i just type gti this is get terrain info and then right click and then you can see here action fill and the french name 34 alvd now then we go back to the tutorial and we see now we need to use the the command new walk and then the label we call it so this is new walk it's the to create uh the wall train then we give it the label walk and then

**[13:24](https://www.youtube.com/watch?v=ePhY5laYC-E&t=804s)** we type the code alvd for the action field now this is in our walkthrough and we need to specify this like we can already walk here but this is like the official walkthrough now now we need to also make it for the the the slides and the thread for the slide train is similar you just do dash news slide and nice and then for that we do the same don't forget the apostrophe quotation marks then we need to create the slide

**[14:02](https://www.youtube.com/watch?v=ePhY5laYC-E&t=842s)** and this is very easy uh as you can see here is dash create walk or press three death guess through slide uh it matters what uh the uh oh we already have this slot it matters what the um the label is for example if you didn't call your walkthrough and walk but something like uh walk two then you need to type create walk tool instead of clickball now with right clicks you can create rectangles you can create your slideshow

**[14:41](https://www.youtube.com/watch?v=ePhY5laYC-E&t=881s)** okay beautiful this is my first level let me just add a little bit more okay perfect now i continuously or often type dash s and it's to stop your uh current action you can also essentially use it that often you can also use shortcut and you can assign for example assign a and s s2 and if you tap a you can tap dash ash all the time it's useful for example if you want to create death or create slides

**[15:19](https://www.youtube.com/watch?v=ePhY5laYC-E&t=919s)** very often create some death round beautiful this is much quicker than the world editor foreign so uh one of the adventures of using mac okay and then we end this level with a new walkthrough great great death uh and you know what this is my whole map there's only one level um then the next thing you want to do is add units to this to the map uh by the way there's also more parameters you can use to change um

**[16:13](https://www.youtube.com/watch?v=ePhY5laYC-E&t=973s)** on the uh to change for the death train for the slide train for the walkthrough for example the slide speed you can change and there's uh this comments doc has a lot of comments in which you can use example here is their stuff and then for the monsters you want to use get we first need to get the unit types for the monsters and you can find these unit apps in the world editor let me just open it again

**[16:44](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1004s)** i open it closely prematurely [Music] then if you go to module and then object editor and then uh let me just show you how it shows for you and this is how it shows for you first uh you want to go to view and then click on display failures is raw data so you can see like a four letter code in front of every unit this is the unit type id that you want that you need to make units in the game so what are the most traditional units

**[17:16](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1036s)** to use in a slide are trolls and those are also my favorite units so let me just find the i stroll which fits the setting it's called knitter it said the united then you go to the website and you find this is the construction of your to create a monster this new monster label unit type id which is the knitter and then emulation radius the emulation radius is uh how big the area is around the unit in which it will kill you like if it's

**[17:56](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1076s)** very small emulation then you can almost just go over the unit without dying and it's very large you die even in an area around it your monster leave this yep label labels troll knitter and then emulation i use 50 is common but you it's up to you what you what you do you just have to play around with it now i have my first monster i can check my monsters that i've created in dm and for the slides that i created dt

**[18:35](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1115s)** i want to create a create a monster troll and this is uh for the next step like you as you can see here in the parameters you have uh more parameters you can change but that's uh for you to to look at if you want to create monsters on your map in your slide you have to type you have several things you can do is clear monster with uh dash create monster crm and then troll and it creates this is probably your most used one

**[19:08](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1148s)** it can create monsters that walk where you click you put two clicks and you can make them diagonal you can make them straight you can do whatever you want then you tap dash ash to stop the action and the next thing next type of monster creation you can use this cream a crma troll and it does the same but you don't have to use two clicks you have to watch one click and it will walk in a straight line from one

**[19:35](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1175s)** side to the other and it's also one of the most used style creation types commands you will use it's very simple now there's also the non-moving monsters you can use i would use a different monster type for this but let's just use troll for this one here we go and if you don't want to put it here you can use dash as that and we want to put it here for example but all the commands are also on the website you can check that and there's

**[20:06](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1206s)** one more thing in the case you want to use a monster walking a very long distance for example from this side to the other as you can see it doesn't walk in a straight line it it slightly walks off and you uh to a different point in the middle or somewhere else so to change that or to solve that you can use a different commands and this is just a trick i'm going to tell you in principle you can use this

**[20:34](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1234s)** command for different stuff as well and that is with the create monsters with multiple petrols and then we try this again you go here we want to put it here we want to stop it here and then here now as you can see this monster will walk first to this point then to this point and then to this point all in a seemingly straight line and it's pretty much more straighter than in the other way now that now that you've created your

**[21:11](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1271s)** monsters uh you have two more things to do well there's a step forwards to create meteors but we're not going to do this in this step this uh up to you it doesn't have to be in your map then what you do want is the visibility source because when you enter your map it's gonna be dark but you want to make it your first level feasible so what you see now is uh get making that phone it's a command to see your uh your level

**[21:40](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1300s)** that you're going at is this is the first level so we want to make a feasibility zone for this first level is the command crv as you can see in the step five crv [Music] then you want to make rectangles to create your facebook so so i want my to make visibility zone from here to here actually from here too to here is nice and then from here to see here and from here to here now i made multiple rectangles because

**[22:30](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1350s)** uh my my slide is not cannot be covered in one rectangle or it could be but it's kind of ugly because you have a whole area that you will see if it's building that's not part of your slide it's kind of waste so now if you hide the train you can see the visibility zones it's multiplayer it's three rectangles as you can see this part is kind of ugly so we could remove that and to uh just a note on this

**[23:00](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1380s)** if you measure your visibility zones then you can change them at the moment you're making them by using z to remove them and they try to re to go back to your the step that you removed basically the same as same word but if you uh stop using visibility zones and you for example create monsters and then you want to go back to uh changing your visibility zones you cannot do that so then you have to remove all your visibilities once you

**[23:34](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1414s)** start again so make sure that when you you make visibility so much you're certain that this is what you want now the next step is for the levels we're going to create a level uh so for the this is going to be the first level and we want to first make a new level so we create new level then for this level that we are right now is we're currently at number with zero the first level uh we want to create an the start of the

**[24:04](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1444s)** next level so this is create start next and then let me just see more this is going to be the start it's going to be rectangle this is the spawning area of the next level then we want to create the end of the current level and this is the rectangle in which the slider has to pass in order to finish the level and we're gonna make it just not at the start of the walk area but just inside so this is how the first level is made

**[24:38](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1478s)** and we can just see if it works hide it we're gonna slide through very easy i almost died i died okay uh hold up we're gonna try this again yeah these petrols are not made that well anyway right now we're on the walker but it's not the next level yet if you go a little bit further this level appears we end up alive and we are at the next level which is over here like if you die now and we respawn then we respawn here all

**[25:15](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1515s)** right this is how you make a level we've got long level we've finished all the steps now we want to save our in-game uh modif uh modifications if this is also on the website it's clearly written steps but let me just guide you through that the first thing you type is s make to save everything you have into the cache converter or like just save it outside okay you wait until the saving map finished because if you have a lot of

**[25:45](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1545s)** stuff you made then saving my finish will be later uh next uh you can leave the map you can leave rocker 3 you can do that make sure that if you only leave the the map and you're still inside worker 3 that you might not be able to change to modify the things in your map so and this is with regards to the uh to the to the terrain that you added so the best thing you can do is just exit worker three so you make sure that

**[26:18](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1578s)** uh there's no problem next you go to the walker three folder and it's safe you find up your profile profile 1 and let me just go there this is in save games it's worker 3 profile 1 and here we have my cash converter that i put in here that i downloaded before i also put in my mac jazz generator so the first step you want to do is uh you make a short shortcut of this uh of the cache converter then you go to

**[26:50](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1610s)** properties and then you add space campaigns dot worker 3 w3v apply ok and let me see if you could correctly walk through and this is how it should look like that's correct i'm going to press ok uh okay and what you have to make sure is that the campaigns point w3v is at the latest date which is from for me very close to the latest date it's just when i uh use the uh dash asmic command then then this will

**[27:32](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1652s)** be updated now we use the cache converter and something quick happens uh and once we've done that we can use the jazz generator okay now we've done that it worked we got a new folder fishy this sortie um oh yeah and by the way it might be that it says that it's a not very trusty program but you can just ignore it there's nothing wrong with it here there's your your in-game stuff that you your in-game

**[28:11](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1691s)** modifications that you made and what you first need to do is get your terrain inside your map because to do that we go to mpq master which you also downloaded this is a slightly different version and what you you probably gonna download then we go to [Music] you first open your slide then you the list faster you need is work three for me it's error rate of chaos and tft but for you it might be just mark three

**[28:45](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1725s)** then we see the walker three uh let me go to workers war three map dots w3e and we need this we go to add file then we navigate to officiate the sortie again here and we see a similar file or similarly named and we open this so we replace this file and here it says zero files added and one file is replaced then we close this and we go to the world editor which i happen to happen to have already opened

**[29:24](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1764s)** but i did not open the map yet and then we go to our test slides the slide that we created let me open it all right and now we see hey our terrain is in the map that we didn't previously have so that's good all right then the next thing you do is you go to your trigger editor and then you go scroll all the way down and you see chess generator you see three triggers that you have and you need to add

**[30:02](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1802s)** some information in there then you go back to your officiated sortie file you see terrain types you open it you can open it with visual studio but you can also open it with notepad plus plus it doesn't really matter that much then you see here uh specs this is the stuff you create at your in-game terrain you go to [Music] accurate trigger you remove this and you copy paste what you as found in the file

**[30:32](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1832s)** then you do the same for the other two steps you copy this mostly drops creation up and for levels this one is uh the bigger file three types nope levels so and sometimes um it might not completely copy so you do you need if for example something went wrong when you enter your game it's not completely uh as you actually made it it might be because it didn't copy correctly you know in the world editor and some of

**[31:13](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1873s)** that has problems or sometimes it even shows some weird symbols but then you just have to try again and if it doesn't copy completely you have to add the things that haven't been copied so be wary of that but for me this is probably right all right now that we've done this we've put all the progress we made in game into my map so we first changed the map description to my new map it's the slide version one

**[31:43](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1903s)** and then we save this map as the slide version one should all work well and this is how we entered every other progress inside the map now we can check this again just to make sure we open work three again let's sing a player custom game test slide version one is under you see the train in the menu apparently what i chose will promise and you can select solo mode and you see hourly units that you made

**[32:32](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1952s)** your hero will spawn and we can try it out let's try it out let's make let's complete the first level this time without dying beautiful there we go and we have in the second level but as you can see it's the same as when we left it but this time uh it's already saved in your map so every time you open it it's the same uh the cheat commands and the making commands are still in the game so if you want to continue you just do the same

**[33:03](https://www.youtube.com/watch?v=ePhY5laYC-E&t=1983s)** you go create walk or create slides you create slides create a new level create that string create the units and then save it again according to what the note is on the website you can use it all this stuff and then well in the end you will have your final slide and you have some more steps to do once you're completely done but it's all on the website you can see that um so yeah these maps can also be it can be

**[33:32](https://www.youtube.com/watch?v=ePhY5laYC-E&t=2012s)** played on reforge and on the patch 1.31 where we created it but the idea is that we want to use it on reforged uh the whole thing and i'm just gonna say this again why we use dispatch is because on refresh it doesn't work this whole creating this slide with mac and this is for me this is much easier than to create a slide from scratch with all the triggers you have to make on world editor and i think well if you want to make a

**[33:59](https://www.youtube.com/watch?v=ePhY5laYC-E&t=2039s)** slide you can use this it's very useful and if you don't want to use it then you don't so yeah that was it if you have any questions just post a comment and i will try to answer them if i see them yeah see you later
