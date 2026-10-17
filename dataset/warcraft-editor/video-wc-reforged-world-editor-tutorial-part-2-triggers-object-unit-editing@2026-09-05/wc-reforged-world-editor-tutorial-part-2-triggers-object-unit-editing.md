---
type: vendor-doc
package: video-sunfire-spawn
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=n_vNJ1v6ZIg
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 1478
chapters: 6
timestamped: yes
---

# WC: Reforged World Editor Tutorial (Part 2: Triggers, Object & Unit editing)

Source: <https://www.youtube.com/watch?v=n_vNJ1v6ZIg>

Uploader: Sunfire Spawn | Duration: 24m38s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

In this second part we cover the slightly more advanced Trigger and Object Editors!




#Warcraft #Worldeditor
Music from Freesound . org: https://freesound.org/people/Migfus20/

## Chapters

- [0:00](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=0s) Intro
- [1:20](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=80s) Isometric RPG
- [3:54](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=234s) Object Editing
- [7:20](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=440s) Trigger Editing
- [15:19](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=919s) Region Editing
- [22:45](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1365s) Outro

## Transcript

**[0:01](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1s)** hello and welcome to part two of my warcraft world editor tutorial series in the last part we covered these tool palettes for doodads units and terrain that should be your basic building blocks for being able to sort of set up morph and you know move stuff around and whatever in this we're going to go over some more complicated things which is going to be the trigger editor which is f4 and you can open up the trigger editor

**[0:33](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=33s)** and we're also going to talk about the object editor which is f6 so between these two editors or these two tools that the engine offers you you're going to be able to tweak and modify everything in the game and you're also going to be able to set up quests cinematics encounters uh effects spawning spawning in units despawning units all these everything this this is your this is your whole network of everything that

**[1:04](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=64s)** you're going to want to be able to change so this part is going to get a little a little bit deeper and it's gonna be probably longer than the first part um so i'll try to be as speedy about this process as i can so if you remember at the end of the last video i said i want to make a single player isometric rpg so isometric rpg means that i'm going to need the camera to be locked to a specific unit so let's go ahead and

**[1:38](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=98s)** let's go over to the unit palette and let's get a super cool guy you know just somebody who's who's dope uh here we go we've got mr dawn seeker here right so this is where he's he's gonna start he's gonna start here let's uh let's go into the terrain editor let's create a road that he can follow so that there's some incentive right out the gate let's create a little town over here [Music] create some hills

**[2:19](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=139s)** [Music] so [Music] so let's also put maybe some trees and let's go to structures cityscape in order to find some buildings and we're going to place one of these buildings here maybe one out here in here and there [Music] okay so a sort of uh peeve that i have with uh this with this game is that the buildings are not really accurately sized to units i mean if you can see here i mean yeah it sort of looks like you're at like a

**[3:49](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=229s)** mini golf resort or something like that you know so uh turns me off so how can we make these buildings look a little bit more realistic and scaled so there's two ways we can go about this we could just make the buildings massive or we could compensate a little bit in both ways by making this unit slightly smaller and making the buildings uh significantly bigger and with this combined effect you you get basically a much better look

**[4:22](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=262s)** to it so let's go ahead and let's go into the object editor and we're going to go into doodads and we're going to want to go into structures and these are the randomized building but have no fear we don't have to change every variation right over here down here we have the editor maximum scale and minimum scale so if we set the minimum scale to 1.8 instead of 0.8 and set this to 3.0 now if i go over here and select one of

**[5:00](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=300s)** these and hit page up to modify the the size you'll see it snap to the new settings and if that feels too wonky too ridiculous you can come back in here and you can set that minimum scale back to normal or you could just leave it at that if you wanted if you wanted to and then you can come back out here and now you have a lot more leeway and by the way you do home and end to make it fatter and skinnier

**[5:32](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=332s)** and then you do page up and page down to make it taller or shorter so now we've got far more variation that we can work with we can make these buildings actually look pretty realistic [Music] at least in their size [Music] this one's i mean it's three stories so i guess it should be kind of massive huh all right now let's go into props because the town ought to have some lights [Music] scroll down to torches

**[6:26](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=386s)** [Music] get some torches placed okay so we've got kind of our first little little town to go to so now let's go down here let's zoom in on our guy and uh let's go ahead and let's set the camera to where we want it to be set to so let's first reset the camera orientation you do that by clicking the middle mouse button or the scroll button and you'll reset the camera orientation and you can scroll in or out however far

**[6:59](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=419s)** you want it to be i kind of want it to be a little more zoomed out than it normally is if you press m you'll go to the camera palette let's create camera and let's call this camera let's go right click edit camera properties let's call this camera player cam now let's go into the trigger editor so you can see that we've got this melee initialization trigger here so this is basically what makes it so

**[7:29](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=449s)** that the game boots up like well like it's supposed to where it's gonna spawn everybody and it's gonna spawn their bases and it's gonna spawn their their people and you know set the uh victory and defeat conditions and it's going to do this based on the event that and it's going to do this in the event of map initialization so i'm going to save you a whole lot of time a whole lot of effort delete this

**[7:58](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=478s)** just delete it so that now when you go to test and by the way when it says this just hit yes don't worry these won't even show up they just have to be there in order for it to boot up properly so it says that there's the spawn location out here but you can see that it didn't put a base there because i deleted that trigger so it doesn't start up the same way anymore but you can see since this guy was placed under player

**[8:23](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=503s)** one red he's my player so i've got some abilities i can move him around that's cool but the camera's not locked to him yet so let's come back out here so let's go back in here and let's create a new trigger let's call that player cam and i want this camera to lock to him right as the game starts so let's go ahead and let's make that event map initialization [Music] action we're going to press c a couple times

**[9:00](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=540s)** to jump down to cameras and i'm going to want to go to camera apply camera object so this is how you're going to set it to whatever camera you've created so let's go back in here apply camera object so this is where we're going to select the variable player cam so it'll have the full list of cameras you've created here or you can go to select a camera and you can click it it's up to you and it's going to set that camera for

**[9:29](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=569s)** player one that's us over zero seconds don't worry about the timing that just means it's going to do it instantly if you wanted it to do it over slower amounts of time well that's getting more into cinematic territory and we'll get to that for now just keep that at zero so that it snaps right to our desired camera so here's the thing though it'll set the camera to this camera and it'll be more zoomed out but it won't follow him

**[9:59](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=599s)** unless i create another action so let's go to new action let's go back to cameras and let's hit lock camera target to unit so it's going to lock the camera target for player one that's us two select a unit and now it's gonna it's gonna lock that camera to this character and you don't need you don't need to edit any of this you just leave that at that because we've already got the camera set to the orientation and stuff

**[10:30](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=630s)** that we want already by doing this so now let's go ahead and let's boot this up so you can see the camera is far more spaced out than the standard warcraft camera and when i move my character he is locked or rather the camera is locked to him so i can't move the camera away this plays just like any kind of isometric rpg the only difference is that i think you do have to target him but so then you come over here and you

**[11:04](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=664s)** can learn your abilities and we've got this empty town there's nothing going on here all right so let's come out here and let's do some more so like i was saying earlier there's this kind of combined effect that you can get right from making the buildings bigger and also making the units smaller so hero units are generally taller than standard units yeah significantly so uh if this bothers you the same way that it

**[11:34](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=694s)** bothers me here's how you can change that so you can come into the object editor you're going to want to go to units you're going to want to go find your specific unit so i'm working with the campaign hero mr dawn seeker so in here you're going to be able to find scaling value underneath the art section so this is essentially his height so you're going to want to go over here and you're going to want to change this

**[12:04](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=724s)** so i'll let's let's first give you some reference and see him while i do this okay so now let's go ahead and let's take this and let's scale it down by one you can see that that made them a little shorter for each one of these scale downs you're basically going like about a foot in their height so i still want him to be you know maybe pretty tall maybe taller than your average joe he's the main character so

**[12:34](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=754s)** you know but now he's not you know he's not [ __ ] master chief and uh for some comparison here's how he's gonna look next to buildings so now that he's been shortened and the building has been made larger this actually is a pretty reasonable size especially if he's a tall you know like six foot elvish dude so look at that he can yeah he can walk through that doorway hundred percent cool so that's kind of the way that i do that

**[13:09](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=789s)** i like to scale the buildings up but also scale the player models down uh so that you get this sort of combined effect where it just looks a little more even without without having to adjust either one too much and trees you don't really gotta change i mean they they kind of they're the trees are pretty good if you want them to be bigger you can you can change them the exact same way i showed you how to change the buildings that's

**[13:34](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=814s)** how you can change everything you just come into do you just go into do dance oh jesus you just go into doodads and you come into here you select anything and it will have this editor minimum and maximum scale and also if you're confused about what you're looking at just right click select in tool palette there you go that's the object you're editing you also can go into art team color and you can make it so that they don't match

**[14:06](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=846s)** the owning player so you can change the color of the unit so say you know i i want this guy to be owned by player red but i don't want him to be red i want them to be i want to be gray um if i go ahead and a website didn't i set a different character uh so if i do that for this guy for mr dawn sneaker let's say snow i'm sorry i'm very tired i'm getting loopy now okay so that's how you can keep the uh the colors uh neutral on

**[14:35](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=875s)** these characters so he'll still be owned by player one red so if i go over here and i go to player two blue and then i place another one of these units you see he's still gray but this one is now owned by player two so so yeah so there's a lot of what you can change and stuff just in the object edit and also you can right click on any item destructible doodad anything you can right click on it and you can hit new

**[15:05](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=905s)** custom unit so when you make this new custom unit you can title it whatever you want so let's call it mr dawn seeker and it's going to make a version of the guy we were just working on the only downside to this is simply that when you copy the new unit you do have to go back through these values again so in order to make him you know the the the same height we have to come back in here and we have to set those things back so do

**[15:35](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=935s)** keep that in mind when you're going about copying and making new units you do have to reset these values again so if i go up here and i go to this guy and i go to snow set him to that then now this custom unit now he's the same so that's how custom units work it's the same thing with um [Music] it's the same thing with doodads you just come in here and you just go and you hit new custom doodad you call it

**[16:06](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=966s)** whatever you want just just whatever right and it's like word there it is and you can change it however in the doodad menu you're not actually going to find a custom section so it's actually it's just gonna be it's just gonna be at the bottom of whatever section you you put it in so so here's my whatever so so yeah that's kind of you know the basic gist of you know uh basically the trigger and object editors however let's let's go

**[16:37](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=997s)** over one more thing with with triggers while we're in this basic tutorial which is going to be how we can utilize regions so let's press r to go to the region palette let's go ahead and let's make a little region here so let's say the player is incentivized to come down this road and when they walk into this region i want them to be ambushed by a few bandits so let's hit spacebar to go selection mode so that we can click

**[17:05](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1025s)** on this we can kind of move it around and let's go into edit region properties and let's rename it let's call it bandit encounter and uh if you want to you can also change the color of these so you you can categorize and color so you know it's a it's a bandit encounter it could potentially lead to a quest so let's make it yellow so [Music] okay so now we've got our region and we've got our purpose for it but we need

**[17:41](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1061s)** to make it work so that when this unit comes over and walks into here it actually spawns the bandits so let's go ahead and let's go into the trigger editor let's make a new trigger and we're going to call it bandit encounter one the reason we're going to call it that is because odds are you'll probably want to have a lot of encounters like this in in your games so this is just the first of many so let's say new events

**[18:10](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1090s)** and you're going to press you you and that'll get you down into the unit so unit enters region a unit enters region select region this region okay okay new action we're going to go into unit and it's going to create units facing angle don't worry about the facing angle that doesn't really matter right now however if you do want to change this you can come in here and you can go down to value from 0 to 360 degrees with 0

**[18:40](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1120s)** being looking straight right and that would be well i guess 90 degrees looking straight down and then 180 looking off to the left and 360 looking up to the top so um honestly they can just they can be facing wherever they're gonna turn to attack the player at the moment that they spawn so um let's say three bandits and let's go in here let's change up to bandits so they're gonna need to be neutral we're gonna go to cityscape

**[19:12](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1152s)** and here's bandits so we'll click on that bandit hit okay so we'll create three bandit four player one so that's not good that'll just create three bandits that we can control so let's set this to neutral hostile so it creates three bandits four neutral hostile at oh we don't want them in the middle of the map do we so let's go ahead and let's go in here center of select a region let's click on the region that we made

**[19:42](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1182s)** and hit okay okay so now it will create three bandits for neutral hostile at the center of the region we created facing yeah that doesn't really matter but again if you if you want to 360 degrees you can set that value so that they are facing a specific direction that can come in handy in some situations particularly cinematics but in this case we we just want them to spawn there and just start attacking you so

**[20:07](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1207s)** here you go so that'll create the units the moment that a unit enters that region however there's sort of an open end here if i were to enter the region leave the region and re-enter the region or rather if i were to enter the region the units spawn there and they technically enter the region this creates a chain effect that will just continuously spawn bandits over and over and over so you're going to want to bake

**[20:32](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1232s)** this into every single encounter or trigger that you make of this kind where you want to come in here and you want to go to t and that will take you straight to trigger turn off turn off this trigger so it's automatically set to this trigger so now this isn't open-ended the moment that you walk in it'll create these three bandits and then it won't do it again after the first time so now we've got our player cam and we've

**[20:58](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1258s)** got our bandit encounter however there's some other things i don't really like the abilities that this unit has so i'm going to change some of his abilities now too so let's go into the object editor let's go to units heroes here he is let's go up here to the top where you've got abilities hero so let's go ahead and change these so let's get rid of these let's give them something cool let's give them uh let's

**[21:23](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1283s)** give them some chain lightning chain lightning and [Music] i'll give him a heal holy light and i'll give him a reign of fire ability so now he's got some new abilities and with that let's go ahead and let's test this out so we've started up camera's in the right place it's locked to him he's got the abilities that i gave him so for this encounter [Music] i think i'm gonna i think i'm gonna go with the chain

**[22:09](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1329s)** lightning for this encounter and let's just go ahead and let's go down there [Music] and the moment that i entered this region the bandits spawn so now i've got my first encounter [Music] i can fight these bandits and they give gold and experience later on and some videos down the line i can show you how you can create vendors how you can customize itemization and even set up progression paths for your characters so

**[22:46](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1366s)** i think that'll just about do it for this video so here we've covered a lot of the basics as you can see with the trigger editor it's actually pretty simple it it does get convoluted when you want to do some tricky things but at the base level it's actually very simple and if you have an idea i guarantee you in some way through the triggers you can work it out you can think of almost anything it's it's it's

**[23:12](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1392s)** pretty limitless what you can do with this so in part three we're going to be going over some more interesting things we're going to be going over how to make cinematics which is grueling it's a long process to make cinematics we're also going to be going over quests and we're going to be going over more advanced terrain and level design we'll also be going into itemization progression stat balances

**[23:39](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1419s)** all kinds of good stuff that you're going to need in order to have your own rpg that really feels like your own so thank you so much for watching this i hope that this was helpful beneficial and again if you have any specific questions that you want to ask go ahead and leave those in the comments i promise you i will answer that as as specifically and as quickly as possible thank you so much and i'll see you in the next one

**[24:07](https://www.youtube.com/watch?v=n_vNJ1v6ZIg&t=1447s)** [Music] you
