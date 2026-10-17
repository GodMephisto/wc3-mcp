---
type: vendor-doc
package: video-implave
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=TP8YpgiGOPs
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 1390
chapters: 13
timestamped: yes
---

# WoW Modding: Modern Map Making Guide

Source: <https://www.youtube.com/watch?v=TP8YpgiGOPs>

Uploader: Implave | Duration: 23m10s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Modern Map Making discord: https://discord.gg/C85673kkWd
Written Guide: https://marlamin.github.io/modern-map-making/category/basic

Chapters:
0:00 Intro
1:09 Setup
4:50 Noggit Basics
10:17 Level Design
13:16 Advanced: Listfile
13:47 Advanced: Area Names
15:11 Advanced: Continent Editing
16:01 Advanced: Map Downporting
18:03 Advanced: Noggit vs Red
19:02 Advanced: Moving Maps
20:27 Advanced: Generated Terrain
20:53 Advanced: Custom WMOs
22:11 Credits and Outro

Songs Used:
Shaman Village - Elden Ring Shadow Of The Erdtree OST
Quest For A Cure - Baldur's Gate 3 OST
Dream Walk (Instrumental) - Baldur's Gate 3 OST
I Want to Live (Instrumental) - Baldur's Gate 3 OST
Wash My Pain Away - Baldur's Gate 3 OST
Last Light - Baldur's Gate 3 OST

## Chapters

- [0:00](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=0s) Intro
- [1:09](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=69s) Setup
- [4:50](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=290s) Noggit Basics
- [10:17](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=617s) Level Design
- [13:16](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=796s) Advanced: Listfile
- [13:47](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=827s) Advanced: Area Names
- [15:11](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=911s) Advanced: Continent Editing
- [16:01](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=961s) Advanced: Map Downporting
- [18:03](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1083s) Advanced: Noggit vs Red
- [19:02](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1142s) Advanced: Moving Maps
- [20:27](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1227s) Advanced: Generated Terrain
- [20:53](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1253s) Advanced: Custom WMOs
- [22:11](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1331s) Credits and Outro

## Transcript

**[0:01](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1s)** [Music] have you ever thought to yourself man world Warcraft is a cool Universe I wish I could build a Zone in it well this is going to be the video for you whether you are on Epsilon Noble Garden dragonflight BFA Shadowland The New Classic builds using custom lunchers or other tools that would let you add files to the game this step-by-step guide should get you started I should also preface this by saying this is for local

**[0:45](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=45s)** sandboxes and private servers only the goal of this video is to set up a rapid and seamless workflow where instead of fiddling with various tools and converters you can just focus on making Maps there are chapters available below for some of the distinct topics that we will be discussing but for now let's start with the setup even though we will be making maps for new clients nugget the community map

**[1:15](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=75s)** editor sadly Works only with the old wrath client currently so the first thing we need is a 335a US client has to be that exact build as long as the client is stock it shouldn't matter where you down downloaded but generally wow dl.net chromcraft War main Etc all should be fine next we're going to need modern assets they imported to wrath links will be in our Discord after that we need to get a particular version of nugget one

**[1:47](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=107s)** that supports modern features navigate to releases and grab the latest build another thing we need here is the converter note that you might need to install that net to run C programs and the latest V C++ redistributable for nugget after you have everything downloaded place the down ported assets into the data folder of your wrath client together with the other mpq files after this let's start nugget hit create

**[2:14](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=134s)** new project and fill in the directories the path to your wrath folder path to where your files will be saved and the name double click the project that you have just made and next to setting up a template map this this is supposed to be the simplest most pain-free way to start editing I highly recommend starting with this as you learn because it will solve a lot of issues hit create new map and in map directory put custom map as one

**[2:44](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=164s)** word and here you can put pretty much any cosmetic name that you want and save let's close nugget for now and get the files for the template map template Maps will likely be expanded upon and will be hosted in our Discord go into the mega download the two archives in the list file and extract them custom map 335 goes into your project folder it should be your project folder world maps custom map and replace empty files that

**[3:16](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=196s)** we have created custom map modern goes into your modern client with whatever mapping file it is using to load files this particular one is an Epsilon patch and that needs to be abled in the launcher custom list file goes into your upconverter meta folder if it's not there yet just create it go back and now let's do the converter setup when you first open the GUI you will need to download the necessary config files hit

**[3:46](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=226s)** those four buttons and let's fill in the pass here input directory is your project folder where the map that you will be editing is in our case it is your project folder world maps custom map that we have created output directory is from where your game will be reading modern map files from since I'm on Epsilon it's my patch for this particular map I should also note that Arc mapping support is also planned

**[4:12](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=252s)** as well as more generic folder patches the last thing to do is to grab the texture config for nugget it also can be found in releases navigate to your project folder and make a new folder there call it lowercase extra uppercase data as one word and paste it there now we're finally done with the setup and can start editing of course you can edit other Maps but that has more variables and will be covered in

**[4:40](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=280s)** the advanced section with template maps on the other hand the only potential failure Point might be from extreme user error when you first open a map and nugget it will prompt you to get a uid check hit get Max uid I would also suggest going help keep binds for general information on how the tools work also in settings double check that the modern features is enabled the movement in nugget is pretty standard

**[5:13](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=313s)** it's w ASD o can decrease your movement speed and P can increase it let's go over the left panel first if you mous over any icon it should display what tool you're looking at the first one is rais and lower on the right you can control the type speed radius object behavior and there's also a custom brush to load height map based [Music] brushes you use shift left click to raise and controll left click to

**[5:51](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=351s)** lower the second tool is the smoothing tool used to smooth out Heights and get the angles you need out of the terrain [Music] the third one is the texturing tool a thing of Interest here is the high texturing and texture scaling functionality where with high texturing you can basically paint a texture in the low points of another one when doing texturing try to pick tile sets that fit together well in

**[6:37](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=397s)** terms of their color palette also a good idea here is to always start with a base of usually dirt and paint other textures on top of it though do mind that each chunk has a four texture limit it might be overwhelming at first but with just a bit of practice and looking at how blizzard do zones it should get a lot easier there are also a few essentials like wireframe here verx coloring and much more we will touch on some of that

**[7:13](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=433s)** stuff a bit later while looking at blizzard Maps the next tool is a hole cutter holding control and left click will cut a hole in terrain letting you place a cave object there or doing floating islands shift left click returns them to normal the next tool is the area designator with the way it is now we will have to talk about it in the advanc section after this is the impass tool it basically closes off chunks from

**[7:38](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=458s)** player Movement Like An Invisible wall the water tool is pretty self-explanatory the only thing to note here is the lock you almost always want to lock water levels for smooth surfaces up next is a big one vertx color honestly I'm not good enough to try and explain this one but basically you have the ability to paint color onto textures you want to think where would light hit certain rocks and lighten that

**[8:13](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=493s)** darken texture transitions add greens and yellows to patches of grass have a campfire the area around it probably has soot in it and you would want to paint a bit of black a l torch emits light paint some orange there it's [Music] Etc one major thing I forgot to mention while talking about the texturing tool now what's called ground effects or detail D ads or ground clutter these tiny grasses that get assigned to

**[8:43](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=523s)** textures based on texture weights and a texture map sometimes those weight differences are inconclusive and you might want to go into the ground effect tools and paint exclusion so they don't get generated there up next is the object tool breaks down into two parts Parts the asset browser and object control when adding objects to a map if they are post battle for Azeroth only add them if they are not in a placeholder folder so World WM

**[9:12](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=552s)** moo expansion 09 is fine World wo autogen names is not a very important point is don't Place objects that are not in your client or if you don't know how to add those objects terrain files are basically just Heights texture data vertex color data and object position data so if your client doesn't have the objects it won't load [Music] them the mini map tool after selecting the lighting you want you can create a

**[9:45](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=585s)** mini map for your own though you might need to delete and regenerate the modern wdt for that to work the only tool of note that is left is a stamp tool it's a bit Advanced but if you can set it up correctly it can give great results basically letting you stamp fully textured mountains the panel up top controls rendering of certain things like m2s wmos chunk borders wireframe vertex color shadows and so on and so

**[10:16](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=616s)** forth now let's talk about level design proper planning and starting small are key here let's take a bit of time to look at how blizzard make zones this is a BFA level design map showcasing their technique and processes as you can see this is a rough layout blockouts of zones and towns but you can already understand the feel of the zone and how it will flow next they do a rough texture pass and start detailing with

**[10:51](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=651s)** objects after this everything is refined vertx colors are finalized detailing is done Etc other points that we should briefly note are texture brains and object clusters object clusters are pretty self-explanatory instead of placing one object at a time it's easier to do several groups of objects that fit together well and position those around your map texture brain breaks down the monotony of single textures by introducing

**[11:33](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=693s)** patterns of cracks or low points for the terrain if you want to learn more about Blizzard's level design I would recommend looking at biscon panels and also at artists art stations probably the hardest skill to master for a beginner is making mountains and rocks for that I would recommend looking at two zones the river Zone in culas and the stormsong valley if you want to know how to do that please stick around until

**[12:15](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=735s)** the advanced section when we will be talking about editing continents going back to our map whenever you will want to see it in game just turn on the converter and save the map map at first I would recommend always saying the full map and then only the change tiles whenever the map is saved and converted you would want to teleport there use whatever command your server is using the map ID for this template

**[12:48](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=768s)** zone is 1 1925 assign Sky boxes if you have the tools that can do that or just use auras in general a good piece of advice for nugget is to save often and another one would be to check your Zone in game to see if the things are framed correctly and feel right now let's start the advanced section first let's talk about the list file it's probably the most important thing when it comes to Modern versus old

**[13:25](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=805s)** map making old clients are running on file name ref references and new ones use file data ID list file is basically a ledger of file names and their file data IDs if an object you are placing is not in the list file the converter won't know what to place and that works the other way around if you're downpouring things so keep that in mind area names in an Ideal World we would be able to choose an ID that is created with nugget

**[13:54](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=834s)** but currently that's not a feature first thing you need to do is go to Wago do tools or all. wow. tools and open the area table find the idea of an area that you want to paint and save it somewhere open the area tool in nogget and create a new entry the name here doesn't matter since it's basically a placeholder but save it and close nugget after this navigate to your project folder DB files client and open the DBC

**[14:37](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=877s)** file there to do that you will need wdbx editor you can find it in the resources on the mega like all the other tools for the advanced section find the entry below and change the ID manually to whatever you saved and after this you you should be able to payent normally in [Music] nugget now let's talk about continent editing if you want to edit continents and that includes Eastern kingdoms and calor you will need to get the patches

**[15:18](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=918s)** for them from resources on Mega there are names of continents and their directories just like with the template map you have to add a new one with the proper intern map name the directory so for example for pandium it's Hawaii Mainland and yes that's real after that clear the empty files that net made and paste the mpq into the data folder of your wrath client three things here only do it if necessary convert only the small things

**[15:48](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=948s)** you edit with wdt and wdl generation off and be ready to fix a lot of data loss the process of down porting Maps removes a lot of tech and we did try to mitigate with we could but there are limits to it but what if you want a different map for example the new arath Basin or the crestfall island well you will have to downport it yourself here's how download the tools in the mega the ones that are called Ure file names in multicon

**[16:16](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=976s)** converter and you will also need wow.

**[16:19](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=979s)** export in wow. export find the map that you need and Export it fully after the export is done take the main wdt file and the adts and place them in some folder f after this you want to extract the tools that you downloaded from Mega there should be a text file called how to use double click that and double check the instructions you want to open CMD and paste the command the command should basically consist of a few paths

**[17:13](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1033s)** the first one is the path to the tool the second one is the path to the list file for this tool up next is the output folder and after this is the input folder it's important that you add asterisk ADT at the end that will tell the tool to convert all the files ending in ADT and that's what we need this step will translate the file data IDs that we have talked about in the list file section into file names now we can open

**[17:39](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1059s)** multicon converter and place the main wdt and the converted ADT files there and hit convert then comes the already familiar part of adding a new map put in the proper directory the proper cosmetic name save close nugget Place files in your project folder and voila [Music] nugget versus nugget red why are there two different nuggets you ask well it's a sad story look at it as different tools for different jobs and one day

**[18:13](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1093s)** they will be United I believe nugget has two features that are very powerful the chunk mover and scripts scripts have a lot of functionality like height noise clearing certain objects replacing other objects objects or placing them through an algorithm Etc the chunk manipulation tool does what it says have brain patterns that you want to fill your map with sure thing modular mountains yeah done copy

**[18:44](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1124s)** rotate raise lower mountains yes blizzard use it all the time if you know you [Music] know also there is an eraser tool for when you're really screwed up you can use the two nuggets interchangeably with the same project folder moving Maps let's say you want to move some objects from one map to another create a tile on the source map and paste whatever you want there like this kit bashed Bowser model

**[19:24](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1164s)** blizzard go into your project folder and find the map rename the the ADT chunk that you created and paste it into your main map [Music] folder after this open the main wdt for that map with gr me go second page scan there save wdt files are like lists that tell the game which tiles are there and how to load them [Music] whenever you are done rename the tile and adjust the WT back if you need more tiles you might want to download bulr

**[20:20](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1220s)** name utility the last two points I will only only touch on very briefly because I don't really understand them myself generated terrain and custom wo up boarding net red has a feature to import height Maps Alpha maps and verx maps from tools like Gaia you will need to find the guides for that in the moding Discord but if you want it there is a way the Wrath Clan being old is a blessing and a curse everything is stuck

**[20:57](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1257s)** in 2009 but hey there are at least a lot of tools available one such tool is wow blender Studio through it you can make World of Warcraft wmos that you can then uport with the attached tool remember our discussion about list files though every custom blp texture and wmo part needs to have proper file data ID references bounding boxes have to be added in the blob file in The converter's Meta folder but at other

**[21:28](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1288s)** than that it is as simple as opening blender closing your eyes focusing on the cube and manifesting a beautiful object oh it's an elen gazebo in this case nice when you can export a proper wmo with the Adon and have all the list file entries for the converter there place it into input and convert add them both to the Wrath files and the modern files and you're good to go the only limit with W blender studium

**[21:58](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1318s)** is your own knowledge with blender but if that is developed you can make pretty much anything remaster dungeons make custom assets or uport old assets and that should conclude our rather Bare Bones guide there will be a written guide attached to this and that you can find in our Discord a massive thank you to marman for writing the converter and combining the work of so many awesome nugget developers thank you to V for releasing

**[22:29](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1349s)** his modern texturing implementation Lasix for his work on the warcraft net Li thinking to Binary space and other corar Developers for making their ad and wmo converter and inspiring this project aaras for the Epsilon integration thank you to Grim from K of time for their RP map El near intra and Ki for letting me upo their awesome custom wos 5050 for the guy map and thank you for watching learn improve share the knowledge and

**[23:00](https://www.youtube.com/watch?v=TP8YpgiGOPs&t=1380s)** show off the cool stuff that you will be building and I'll see you later
