---
type: vendor-doc
package: video-tarkhisgaming
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=azjb7GHDsJ4
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 869
chapters: 5
timestamped: yes
---

# Custom Hero Ability Selection Tutorial - Warcraft 3 - Ready for Reforged

Source: <https://www.youtube.com/watch?v=azjb7GHDsJ4>

Uploader: TarkhisGaming | Duration: 14m29s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

With Reforged being released soon, many people will be creating new custom hero maps, or recreating old custom hero maps. In these games, players choose their hero, then choose which abilities they would like their hero to have. This tutorial demonstrates a way to utilize the engineering upgrade ability to allow heroes to level up the abilities they choose.

Note that while Reforged is in Beta, this may or may not work properly as the game is currently filled with bugs. This is a traditional way to create custom heroes with ability selection in Warcraft 3, and that functionality should transfer over to Reforged.

## Chapters

- [0:00](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=0s) Intro
- [0:50](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=50s) Setup
- [1:25](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=85s) Object Editor
- [7:35](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=455s) Trigger Editor
- [13:07](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=787s) Testing

## Transcript

**[0:00](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=0s)** hey guys it's Turkish from Turkish gaming and today I'm gonna show you how to make custom heroes with custom abilities that the user can choose this is similar to Maps such as custom hero line Wars and any other map along those lines where you choose a hero and then after you've chosen that hero you can go through and choose abilities for that hero so for right now I have this this hero what I'm gonna do is choose four of

**[0:22](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=22s)** these abilities and you'll see right now if I don't have any points to spend on abilities and if I go in here there's just one placeholder that's in here at the moment now once I go down to the bottom here if I walk into the circle power I have a trigger that then teleports the user the player and automatically learns those abilities that you had chosen from these stores now let me hop into the editor and I'll

**[0:49](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=49s)** show you how to do this so I've created a new map here and the first thing I'm going to do is place down a circle of power this is simply just to trigger the script it's actually not required and then I'm gonna go ahead and choose a hero to put down for myself to run this through as a test and I'll just choose the blood major right now and then I'll spit down a starting place just so that the game knows where to start us this is

**[1:13](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=73s)** all you'll need in the actual map in order to do this tutorial obviously if you're doing this in a map you've already created you'll have all your hero and everything already done but now let me hop into the object editor and I'll show you how to do this the first thing you're going to want to do is go ahead and create a couple of abilities so I'm gonna hop in under the hero abilities and for right now let's just

**[1:34](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=94s)** choose blizzard so I take blizzard here you can actually use for the initial one this act this default this default skill as default ability now what you're gonna have to do is create a placeholder after this which I'll show you in a second but the to keep in mind here the placeholder has to have the same level skip ability level skip requirement sorry and the same number of levels so this is easy as

**[2:00](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=120s)** done if all of the abilities you're using have the same level skip requirement and the same number of levels for right now I'll just leave it at 3 but depending on what you're doing you can bump that up bump that down however you want to do it so Blizzard we can actually leave this as is but if you wanted to to change any of these create your own custom abilities feel free to do so so now that we've chosen Blizzard we're

**[2:24](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=144s)** gonna create a new custom ability based off a blizzard and what we're gonna do is now that we have this open we're just gonna change this to a placeholder and then we'll just get rid of the description and the name itself will also be placeholder now since we created this directly from Blizzard we can check and the level skip requirement and the levels are obviously the same because they were created from that ability but

**[2:57](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=177s)** if you're doing multiple bilities just go ahead and check to make sure that these are equal to the base ability that you're using so now that we have those abilities set up we can go ahead and go back to the unit and we'll go to the blood mage because that's who we're using and under the hero abilities we're gonna get rid of all the ones he currently has and just go ahead and add in the placeholder that we had created

**[3:20](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=200s)** now if you want to let a hero have four abilities similar to what I showed you in that last one then you just have to give four placeholder abilities so you can just go in here again and add another placeholder right now we'll do two just to show you how to do the multiples click OK and now this hero has two placeholder abilities now the last thing we have to do before we create the actual triggers is create one more

**[3:47](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=227s)** ability so if we go back to abilities this is the key ability that that lets all this be possible if you go into neutral hostile and you go under the hero abilities there is one ability called engineering upgrade so in triggers there's a there is a trigger to learn a skill but that only you works for unit skills and so it will only allow it'll teach the unit the hero that ability but only at level one and you

**[4:16](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=256s)** can't level it up by using engineering upgrade or a ability based off of engineering upgrade you actually allow it to be added to the level up screen there so that the the hero actually acquires that skill so I'm gonna go into engineering upgrade and I'm gonna create a new one and I'm gonna call it Blizzard adder just so that I know that this is the this is the ability that's gonna add Blizzard to my to my hero for this one

**[4:45](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=285s)** the level skip requirement and levels do not matter but what does matter usually I'll bump it down to level one simply just to reduce the number of information required now what you have to do is under ability upgrade one you're going to have to go ahead and add the ability that you want first and then the placeholder ability second and I'll explain why this why this works in a minute once we get to the trigger but

**[5:15](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=315s)** it's very important that its ability you want the hero to gain and then placeholder now for the rest of these it's best to keep them populated you don't have to but sometimes the game can buggy out if you have these empty so it's best to keep them up them updated and just leave them as abilities that you know the hero's not going to have that way it won't affect them at all so now that we have this set up we can go

**[5:38](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=338s)** ahead and this is everything we need to create the triggers sorry one last thing we're gonna want to do before we create the triggers is we're going to create a an item that they can purchase from the stores this can be anything I just like to use something that's you know not usable and something that that's not going to give them any effects or anything like that so if I go under miscellaneous here we can go ahead and

**[6:04](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=364s)** just use a human flag or anything any item that you want as long as it's not usable and we're gonna go ahead and call it Blizzard because that's ability that we're working on here once that's created you'll want to change the interface icon to be the ability that you're gonna use to go to abilities and go ahead and go to Blizzard and that way once they have it in their inventory that's what it's going to look

**[6:32](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=392s)** like go ahead and make sure it can be sold by merchants because I assume you're gonna be using a store to sell it to them and it can be sold to merchants in case they want to sell it back here you'd set up your gold cost normally what I do is I set it as a price of one and then I give the the player say four gold so they can only buy four abilities you'll also want to make sure that the stock maximum is high enough that

**[6:55](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=415s)** multiple players can buy it the replenisher interval is low and the stock initial is the same as the maximum to make sure that they have everything that you need to start with so with all that done the only other thing you'll want to do is normally I change the the basic tooltip to be Blizzard as well as the description and then the the extended tooltip what I'll often do is I'll go back to abilities here we're using

**[7:23](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=443s)** Blizzard so I'll just take down this extended tooltip and add it into here so that people can see what they're buying now that the items set up we can actually go into the trigger okay so now it is time to set up the triggers first things first I'm going to go into the initialization and I'm just going to go ahead and delete all of these because I don't want it running victory conditions or anything like that that'll

**[7:48](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=468s)** stop us from testing this and what I'm actually gonna do is because I don't have a store set up to give the the heroes for these heroes to buy the items I'm just gonna give it to them right away so if I go to create item for hero I went ahead and created another one as well I just add the exact same thing but this time for about four bash so I'll go ahead and give them Blizzard and I will also go ahead and give them the bash so

**[8:20](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=500s)** now they have two items the reason why I did two items two different rule the reason I did two items is to show you that you can do multiple items the reason I did two different ones is because this does not work they cannot learn the same ability more than once so it has to be different abilities each time now that that initialization is set up I'm going to go ahead and create the actual trigger to teach them the

**[8:41](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=521s)** abilities so I'll go ahead and call it ability choice and for the event I'm gonna have it when a unit is within range when a unit comes with five and I'll choose this circle of power that I put down earlier just as a trigger point obviously you can do this any way that you want so now for the actions for the actions you're going to want to start with a for loop so go ahead and choose for each integer a do

**[9:12](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=552s)** multiple actions I'm going to go from one to six the reason we're going from one to six is because this is gonna check their inventory and there are six slots in their inventory so now in the loop you're gonna want to do in if-then-else and we'll go ahead and set the if to be a item type comparison and we're gonna say item type of item carried by hero so I am carried by hero and this will obviously be a triggering unit because

**[9:43](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=583s)** that's what you want to effect and then in slot and for the slot you're gonna want to go ahead and choose for loop integer a because that's the loop that you have running here integer a so it's going to go from one to six and check each of those slots in the triggering unit and it'll check it for whatever you put here and for this first one we'll go ahead and put Blizzard so now it's saying look for four for an item type

**[10:11](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=611s)** carried by the triggering unit in slots one to six equal to Blizzard if that's true then we're gonna want to do is go ahead and add an ability to this unit so I go to unity a unit sorry and go down to add ability the ability that we're going to want to add is the adder that we created so for right now we have the Blizzard adder and we're gonna want to add that to the triggering unit and then we'll go ahead and do the same thing for

**[10:44](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=644s)** or sorry not the same thing now we're going to go ahead and remove that ability so if I go down to unit and I instead of adding the ability we remove the ability and we're gonna want that same one to eat Blizz adder to triggering unit so now what happens is if they're carrying this item it'll add this Blizzard adder to them and then remove it now the reason this works is if we go into the object editor here and

**[11:18](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=678s)** we look at the abilities and we'll go ahead and look at the Blizzard adder what happens when this is added is this ability will try to upgrade Blizzard to the placeholder the problem is we don't have Blizzard so what happens is this essentially does nothing you add this ability and it does nothing however for the second step we're removing this and when you remove this ability it tries to do exactly what it did before but in

**[11:49](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=709s)** Reverse just now it says okay if we have placeholder we're gonna downgrade it to Blizzard and now that we actually do have placeholder this time it'll simply replace the placeholder ability with the Blizzard ability and this is what allows you to add it to a learner Bowl ability so we'll go ahead go back here and the last thing you'll want to do is to go to item and you're gonna want to remove the item that is carried by the hero hero

**[12:24](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=744s)** again will be the triggering unit in slot and we'll go again for the for loop integer a so it's saying if they do have it add the ability to remove the ability and then go ahead and get rid of that item now I'll quickly go ahead and just copy this and do the exact same thing for bash because I created that as well and that's the other one we're gonna be testing and then once I have this added we can go ahead and try and try this out

**[12:55](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=775s)** and demonstrate how it works just like a blizzard to bash adder and then reboot the same so now that the hero has these two we should be able to go ahead and try this out now that we're in here and it's loaded of course I use the flag which was a bad idea because they're on the ground there so don't use the flag fun fact I have these items in my inventory now and you'll see if I go into here abilities all I have is this placeholder

**[13:20](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=800s)** ability however if I go and step in this circle of power the two items have now disappeared from my inventory and I now have Blizzard and I have Bosch which are the two that I was trying to learn so everything worked perfectly the only thing I would recommend is if you notice before I had this the the skill point before I learn these normally what I do is I once the heroes spawn I removed that first skill point so they can't

**[13:48](https://www.youtube.com/watch?v=azjb7GHDsJ4&t=828s)** accidentally purchase the placeholder ability and then I'll go ahead and add that skill point back once they've stepped on this square so that they can learn the abilities get anyway that's all for today thank you for watching and I hope this was helpful for you don't forget to Like subscribe all that fun stuff and here's to a lot of fun times when reforge comes out thank you very much [Music]
