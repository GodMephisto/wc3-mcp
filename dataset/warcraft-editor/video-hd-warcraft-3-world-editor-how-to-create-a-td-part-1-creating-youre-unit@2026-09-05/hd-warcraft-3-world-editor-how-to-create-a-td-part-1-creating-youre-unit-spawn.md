---
type: vendor-doc
package: video-thenerdsocietyadmin
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=yu3o-yq-jL4
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 521
timestamped: yes
---

# [HD]Warcraft 3 world editor, how to create a TD part 1 (creating youre unit spawn)

Source: <https://www.youtube.com/watch?v=yu3o-yq-jL4>

Uploader: TheNerdSocietyAdmin | Duration: 8m41s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

[NEW ACCOUNT: tobitherobitv] This is part 1 of the tutorial: how to create a basic TD. In part 1 i am going to show you how to create a basic unit spawn! Watch part 2 here: http://www.youtube.com/watch?v=wg-CV4x_seU
If you have any questions, tell me and I will do my best to answer them, thank you!
thenerdsociety.com

## Transcript

**[0:00](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=0s)** hello let's turn the Robie from their insider.com here to give you a basic trail in the Warcraft 3 World Editor in this tutorial I'm going to show you how to create arter defense a really basic tower defense and the trail will be in two parts since we're in youtube I can only create videos with ten minutes length or less so it's gonna be in two parts the first part I'm gonna show you how to create the units bond where a unit

**[0:28](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=28s)** spawns at a particular point each 10 second and moves from wrap point to the ending point and dies and in part two I'm gonna show you how to make you know when Leon enters the ending point you're going to lose your life but this is part 1 so I'm gonna show you how to create the user spawn all right click on are to open the region plot this is where you create regions and they're useful to like point out particular places you

**[1:01](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=61s)** want to use in v-trigger it i've only show you I'm gonna show you now click on that guy which is create a region and you just drag it over like that there you go you have a region and you go down here you create one more region and now you have like starting a region and ending region all right now we need to go do of course if you have like a specific unit that you want to create like if you have a custom unit you can

**[1:34](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=94s)** go ahead and use that for your TD but I'm not gonna create a custom unit that now I'm just gonna use a footman or something ok go to the trigger editor delete the melee initiation or whatever and create a new trigger call it unit units then go see what we're gonna create events at the end event will be you click on the flag button and go down click T you see time and leaps that's not the one we want what the periodic

**[2:13](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=133s)** event each or every your amount of seconds time of game time I'm gonna take ten so every ten seconds game time the action we don't need any conditions this just in action and the action will be equal to you the first one that pops up which is the crit unit facing angle and we're gonna create one or you can create like five you don't I'm just gonna agree to foot - okay you can of course you can choose your custom unit here you just go

**[2:48](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=168s)** ahead and click custom big your custom unit and for the player now for play red because I'm plan red and I want to fight these units so I'm just gonna take player brown and not central player will map rear center off region center of south region that region reason zero zero zero so now our d4 please that doesn't really matter it's only facing you've reached up the unit alright great equipment and familiar Brown at the

**[3:24](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=204s)** region 0 0 0 so now we have created the unit and now we're gonna make the unit move all right unit all right so now the event is going to be when the unit enters that region and that's good you go to unit unit enters region and here's region select a region region and this region 0 0 0 yes we need a condition because we only want our enemy units to move or at least if you don't want that you don't need you

**[4:04](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=244s)** created but most people do the condition player Edition comparison and so when only when player Brown enters the region so on or off entering in it sorry I'm going a bit fast oops owner of entering equal to player Brown because like if you enter the region you automatically start moving down to that region we don't want that we only want the enemy player to move all right actions cook you and go ahead

**[4:43](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=283s)** and find unit issue order charging a point at order which unit are entering unit all right move - yes if you have a look at hero series you can take attack move - then it will attack everything and then Ray no one gonna create I'll make move - because we don't want them soo attacker towers move to center of south region that region so Ord you're entering you need to move to center of region 0 0 1 alright so now basically we have like a

**[5:24](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=324s)** basic move system you spawn move system and I'm gonna just gonna create two more triggers now for part one the one is killing the unit yeah yeah now it's the same events as this one actually we could just copy it that would be easier deal with that click control C control V you have a copy just call it unit kill because you have these same events and the condition is stuff just choose that region that region is

**[6:11](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=371s)** dead of that region and owner if entering an equal to play Brown we want that and find unit oh my good wave here the kill remove explode and that's c3 killing removing actions I'm just gonna go for a kill since I'm feeling a bit murderous today okay okay entering unit and there we go we have we kill spawn move kill alright I'm just gonna do one more trigger and this is just a tip for most people who create maps because when

**[6:55](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=415s)** you create map you don't want want the spawning units to be there you don't want it to be neutral hostile not a good idea because if the neutral hostile for the first thing they will move back they'll walk towards the point and they will start moving back again and this happens because the neutral hostile has like a basic you know they have like artificial intelligence which tells them that true far away from their original

**[7:24](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=444s)** spawning point so they have to do back and that's why we're picking clear Brown instead but then you have a player enemy and but the negative thing about that is that plenty Brown doesn't give any bounty they don't you know when you kill a player blonde Browns unit and you don't get any money for it and that's what we're going to fix just right now take event map installation alright that's when the map starts up

**[7:54](https://www.youtube.com/watch?v=yu3o-yq-jL4&t=474s)** actions create a new action click P turn player flag on/off gives bounty on for player route and that's it and I get money for killing player Brown soon units shouldn't call it just call out whatever you want your bounty but we're anyway and that's how you do it part one of how to create basic TD thanks for watching and see you in the next trail by that
