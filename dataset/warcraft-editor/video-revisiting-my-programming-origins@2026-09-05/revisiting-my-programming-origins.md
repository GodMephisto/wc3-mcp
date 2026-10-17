---
type: vendor-doc
package: video-aussie-bim-guru
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=9K9vFdd0NPw
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 784
chapters: 8
timestamped: yes
---

# Revisiting my programming origins...

Source: <https://www.youtube.com/watch?v=9K9vFdd0NPw>

Uploader: Aussie BIM Guru | Duration: 13m04s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Going way off track today... time to nerd out a bit, well maybe a lot!

TO ARMS!!!

I hope you enjoy the session and learn something new! Feel free to comment any feedback/questions below, or follow my channel if you enjoyed this.

0:00 Introduction
1:15 World editor
2:19 Trigger editor
5:05 Testing the trigger
5:54 A finished project
6:41 Playing some WC3
10:16 Behind the scenes
11:58 Outro

Software used;
WC3 Reforged (and world editor): https://playwarcraft3.com/en-us/
DaVinci Resolve: https://www.blackmagicdesign.com/au/products/davinciresolve/
OBS: https://obsproject.com/

Hardware used;
Webcam: https://support.logitech.com/en_us/product/hd-webcam-c615
Laptop: https://www.msi.com/Workstation/WT75-8SM/
Microphone: https://www.jbhifi.com.au/products/rode-nt1a-condenser-microphone
Audio interface: https://focusrite.com/en/usb-audio-interface/scarlett/scarlett-2i2

#revit #dynamo #bim

## Chapters

- [0:00](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=0s) Introduction
- [1:15](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=75s) World editor
- [2:19](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=139s) Trigger editor
- [5:05](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=305s) Testing the trigger
- [5:54](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=354s) A finished project
- [6:41](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=401s) Playing some WC3
- [10:16](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=616s) Behind the scenes
- [11:58](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=718s) Outro

## Transcript

**[0:00](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=0s)** [Music] g'day there you're watching the aussie bim guru now as you can see on the side here today's not really a bim dynamo revit style tutorial it's pretty off the beaten track today um have i gone crazy no so i was actually getting ready to participate in a web panel the other day and we had a pre-meeting and someone asked the question where did you all learn to program and immediately i was like oh sure

**[0:34](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=34s)** dynamo my first programming program in adc but i thought well i was sort of programming before i worked in aec just a bit of fun programming on the side and i remember my days of working in the game warcraft 3.

**[0:48](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=48s)** in a world editor that let us build custom games and i realized that i actually learned a lot of programming concepts before i really looked at programming properly in aec so i thought it'd be really fun today just to show you how i used to program in in here and also some of the things that i built back in the day so i know it's a little bit different to the normal video format so you don't have to watch it it's okay

**[1:10](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=70s)** it's up to you but um hopefully some of you find this interesting anyway let's jump in so this is pretty much where i learned to program when i was younger um at least the concepts of programming it might seem a little bit silly but i'm not kidding i used to build maps in this program um now warcraft 3 reforged unfortunately but back in the day warcraft 3 classic and we had this really awesome program called the world editor

**[1:35](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=95s)** that let us build custom maps in warcraft and the best thing about this thing is not only does it let us build maps like you know we can go on you know add cliffs and do all sorts of weird stuff but you know i can i don't know i'm trying to remember exactly how how this even worked you can you know muck around with the terrain but you can also add cliffs i think as well somewhere in here is cliffs um there we go

**[1:56](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=116s)** you can go on build a map essentially um you can shut down doodads and all sorts of silly objects but more importantly it let you build what was called triggers which let you essentially program an entire map so let's just throw down a starting point and i'm going to put down a bunch of peasants and a footman so this was what was what we call the trigger editor now the moment there's an initialization trigger that starts off

**[2:21](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=141s)** the game i'm getting to get rid of that and i'm just going to build a trigger called um peasant is oppressed and i'm going to make an event and in this case i'm going to say that a unit a generic unit of n a unit is attacked because they're being oppressed in this case i'm going to check the unit type i'm going to say the unit type of that that unit being attacked in this case attacked unit is equal to a peasant

**[2:53](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=173s)** i'm going to make sure that the element doing it um doing the oppressing isn't a peasant so i'm going to say what's the type of the attacking unit and obviously this probably seems nice and zany and silly but it's sort of showing you principles that i use in things like dynamo like not equal to and booleans and conditions and like if then else almost this is really like if then else in some respects and then

**[3:16](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=196s)** i'm just going to say as the action um i'm going to play a noise so in this case i believe there's a sound under somewhere in here under environment i think it is [Music] wow i haven't i haven't played this for such a long time but it was it was super fun back in the day maybe it's under sound the thing is too they've changed it so like everything's out of order now as well um nothing's in alphabetical order there

**[3:44](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=224s)** we go here we want to play a sound and i'm just going to find this sound play sound on unit and in this case i'm going to have to pick a sound to play so i'm going to have to go and load a sound first so i think under the sound editor we had all the sounds of the units in the game um so in this case i believe there's a human and there's a peasant and somewhere in here they're saying they're being oppressed

**[4:08](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=248s)** so i'm not sure which one it is i think it might be [Music] i mean obviously this was a fun game we mucked around a lot in this game but i'm just gonna make i'm just gonna add this as a sound um you know this game was so much fun back in the day just being back here is um bringing back so many memories um but yeah learning just learning to to code here was very fun and it sort of takes you back to the point i always

**[4:44](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=284s)** make that you know if coding is fun you learn more easily but i'll say that the attack unit is going to say that being oppressed and yeah i'll do that so in this case we're just going to make a map where all the peasants say they're being oppressed when you attack them um so warcraft 3 was obviously a bit of a violent game it wasn't necessarily one where everyone goes and hugs and bees friendly it's not as friendly as dynamo

**[5:08](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=308s)** but you know for someone that was in their late teens obviously this was a more fun way to learn to program what do you do and fun to watch as well um so that was sort of like a little taste of how i used to program in warcraft but i mean i used to go pretty deep on this so if anyone's ever played diablo 2 by blizzard i actually rebuilt this entire game at least the first act inside the world editor

**[5:48](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=348s)** and i learned a lot about programming here as well i sort of learned about object manipulation and managing lists of elements or groups of units as you might say in warcraft because everything had like equivalency in the world editor to how you'd program typically a self-self builder a full map it's huge and i'm just going to go and place down some items to level me up really quickly so that i can show you

**[6:10](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=370s)** more um in less time and then i'll show you a couple of the more advanced things that i used to program because obviously i showed you a very basic trigger um the triggers that we built for real maps um these were like the real thing so that was like a very simple trigger that might be like just a global trigger that manages something if it ever happens because if the unit ever gets attacked it says the noise that's pretty much it

**[6:33](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=393s)** but in this case this is a real map um it used to take like 10 minutes to load and now on a good computer it's going to take like 10 seconds so it's crazy how much computer power has increased by then as well i'm probably just going to pick um i'll pick the druid i used to like playing as the druid and let's just go and score some levels so the reason why i rebuilt this game is that diablo 2 was a isometric game and it wasn't truly

**[7:02](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=422s)** 3d so i really wanted to see this game in 3d and as a result i just i just built it i'll go pick up a quest um but already you can just see everything looks pretty different it's very customized there's messages being played which is a trigger i'm just going to grab some abilities so each class essentially had like three trees of abilities that you learn in this case i'll use a couple of fun little abilities like a firestorm that

**[7:30](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=450s)** shoots out through things so i essentially programmed those elements to shoot out from positions and tell them where to move to i have a system where you can transform him into a werewolf unfortunately some of the things don't work anymore because um they've changed the game in the reforged edition and you could summon allies when you were just in human form so in this case i am going to try and i'll have to wait until i'm outside my

**[7:54](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=474s)** web air form unfortunately but you can do things like stomp the ground so it was pretty fun um and eventually we should see some items get dropped as well um which essentially will allow me to to use some different abilities where i can summon someone some allies so let's summon some ravens so these actually created other units um for me so there was a lot of things that happened in the game i even programmed custom

**[8:18](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=498s)** systems that went outside the rules of the game so for example um i programmed the potions belt that was separate from the unit so rather than holding all your items because you were limited to six i created extra units that passed items and effects to the main character so off the map there's an extra unit and there's triggers that explain when i run when that unit uses a potion here on my character so it was pretty um

**[8:40](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=520s)** pretty wacky the things i used to get up to here um there was things like summoning different things so i've got like some some wolves i can summon as well um in this case they look more like dogs in the remake um so it was really fun um those items i could pick up for example these greaves so i had a system that stopped you from picking up more than one pair of shoes because in the original warcraft you

**[9:02](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=542s)** could pick up 20 pairs of shoes and you could you'd be up six and they could all just be shoes um which is you know silly ass like someone in a grizzly bear here and then eventually i'll just um i'll wait i'll try and find a way to heal just a little bit of mana i think she might hear my manner if i remember right no she doesn't bummer i just need to get a mana potion and then i'll just show you a really fun

**[9:29](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=569s)** ability i added to this guy um that creates like a hurricane on top of him i'll go in this cave so you could program things to be moved as well so that doesn't actually take you into a cave that just moves you off the map to another location where you can see seymour so it was all about creating the illusion of the game um by building little scripts and events and behind the scenes there's like you know hundreds of things always

**[9:54](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=594s)** running here we go okay yeah so and then anything i go near essentially gets damaged by the hurricane so this is so much fun um but yeah i'll probably rather than play this for another hour or two i'll probably just um just close it for now but i might put this game on my github just for fun so i'll show you some of the triggers behind the scenes of what was actually happening there um so i mean obviously there are a lot of

**[10:20](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=620s)** things happening just to make the game run in the first place um for example just starting off the game establishing variables these are just establishing variables all of that just a process because we used to store things inside variables um so it's pretty intense um an entrance for example so when i went into that that cave um i was essentially saying if if a unit comes within 200 units of an entrance unit so there's

**[10:47](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=647s)** actually a little character sitting here that's invisible um this trigger will essentially pick that up and assuming that the triggering unit is a hero which is basically a good guy in the game then it will essentially move the unit somewhere else and move the camera with you some of the more complex things i was doing for example like that firestorm uh that i cast which shot three pieces of fire out

**[11:12](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=672s)** um i'll just try and find that so in this case i start an ability the ability is firestorm i created three invisible units that would shoot out fire um in three directions and essentially you can't see them but it's three units that are shooting it so you're not actually moving all that fire yourself but what you're doing is creating events that react to things and essentially create units to do it for you

**[11:34](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=694s)** um but really what was happening here is just the event condition action is really what it taught me um but yeah i thought that would be a little funny video just to share uh just for a little bit of a change of pace and if any of you used to play warcraft 3 or you know build custom maps let me know too um or or if you used other types of gaming engines that taught you maybe like cryengine that taught people about crisis um but

**[11:56](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=716s)** there we go just a bit of fun today so there we go um if anyone made it this far i guess you must probably have enjoyed that um i'm not gonna become a pro game streamer on this channel anytime soon don't worry i'll be back with rev dynamo the good stuff next week i just wanted to make that for a bit of fun and also for me too because i was sort of reminiscing there on something that i used to enjoy when i

**[12:18](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=738s)** was a lot younger and probably still would if i had the time to and if the warcraft 3 reforged remake wasn't such a hot mess anyway if you're not already following and subscribing feel free to do so there's probably not going to be too many warcraft 3 focus videos in future um but at the same time you know if there's enough people that really want to see that i guess i'll make a channel for it but i'll see you in future for more bim

**[12:41](https://www.youtube.com/watch?v=9K9vFdd0NPw&t=761s)** tutorials anyway thanks take care bye you
