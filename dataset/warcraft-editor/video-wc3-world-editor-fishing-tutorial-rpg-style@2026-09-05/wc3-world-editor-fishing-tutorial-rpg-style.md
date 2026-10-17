---
type: vendor-doc
package: video-chuck-norris
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=6rJnIiVGd9g
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 792
timestamped: yes
---

# WC3 World Editor | Fishing Tutorial | RPG Style

Source: <https://www.youtube.com/watch?v=6rJnIiVGd9g>

Uploader: Chuck Norris | Duration: 13m12s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Hey guys Tyler here, with a guide on how to make a fishing ability in WC3 World Editor.
Please if you have any questions or other video suggestions you would like me to do, leave a comment below.

## Transcript

**[0:00](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=0s)** what's going on Guys Tyler here um today I'm going to do a quick little guide on how to make a fishing skill on your Warcraft 3 map here I have just a little bit of terrain that I made nothing too fancy um I'm going to show you how it works and then show you the tring and then show you how to do it yourself all right so you're in game you have a fishing pole in your inventory you click you catch a fish but it's not

**[0:28](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=28s)** 100% says you do not manage to catch any fish catch another one do not manage to catch any fish and if you're not in the water this is a cool feature says you need to be near water to use this item okay so here's what the trigger looks like so if you know how triggers work then you can go ahead and pause this video and just copy everything to look just like this but if you don't know how this works and I'm going to

**[0:58](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=58s)** explain it step by step first things first you're going to want to have your terrain and you're going to want to set up a region for here it's not really the a good shape so I put up two regions then you're going to want to go up to the top right here and open the object editor from here you're going to want to make a custom item I like to use Tom of experience level two power up right down here and

**[1:29](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=89s)** name it fishing pole and since I already have one I'm just going to name this one too now you're going to want to delete its ability go down to the icon and change that to whatever you want I like to use Ironwood branch cuz that looks fishing poleish a model use that doesn't really matter um cool down group you can keep all that fine classification permanent um um you can change all the description a fishing pole used for

**[2:07](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=127s)** fishing fish and you know all that none of that really matters unless you want it to sell stock maximums how many they have in the shop you can put 100 how many like replenishment integers so like every second one ex set up to one so every second it'll replenish so if you buy one it'll be right back um perishable make that false level zero hit points um I don't like my items to be able to be destroyable so I just do like 50 500 I

**[2:43](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=163s)** don't know something like that gold cost then again like this only matters if you want it to be sold in a shop if you don't then you can just make this whatever you want I'm going to put zero dropped one carried false can be sold to Merchant sure um can be sold by Merchant this is what you want to do if you do want it to be able to be sold um and that looks good for here sure I didn't uh forget anything

**[3:12](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=192s)** number of charges put that at zero and actively used make sure that's set to true now we're going to want to go to abilities you're going to want to create a new custom ability for fishing and I like to use battle Roar just name that fishing and since I already have it I'm going to name mine fishing too all right for art Caster I don't think you need a model but if you really want to um you can see over here on the side it

**[3:44](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=224s)** shows you what it looks like and you can just add a model that'll be on your Caster like what it looks like I think on mine I use this one um blighted gold it goes right below your feet um and you you can change that all to the icon you can make that Ironwood Branch items and copy and paste that all right now you're going want to do um levels zero um Buffs going to delete this buff that's important area of effect none of

**[4:28](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=268s)** this is too too important you're just going to I mean for me I'll just set it to one um race I like to make it human it's easy to find required level one check dependent um I think that doesn't really matter targets allowed I'm just going to go ahead and remove all these so it's anything um here you need item ability checked hero ability you can uncheck or check it doesn't matter for hero all right

**[5:00](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=300s)** duration hero for mine I put 3 seconds cool down I wanted it to be 3 seconds as well and that looks pretty good now we go over to buff effects create new custom buff you're going to scroll down to the RS and find Roar name that fishing and for me I'm going to name it fishing 2 again since I already have fishing is an effect true race um see this doesn't really matter but I'm going to put human cuz for me

**[5:41](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=341s)** it's easier to find attachment I don't like it to be overhead Target you can just delete that or you can add another like I said it's a model it's what it looks like below your feet abilities and you can see this is what it's going to look like on your character on the side over here all these to go through but for me I'm just going to leave that blank um icon you want to change that to the iron wood Branch or

**[6:10](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=370s)** whatever um I'm looking in the wrong place items here we go heavy CD FG I know pretty bad all right and that looks good so we're going to go abilities fishing two that we just created go down to where it says where is it hey you want to go to Buffs and you're going to want to add buff and find the one that we just created um I'm having some sort of glitch I don't see fishing that I just created but it should be there so I'm

**[6:50](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=410s)** just going to copy and paste from the one that I had right here um now we're going to go back to items to the fishing pole we just created go up to abilities add ability custom item and then fishing is the one that we just created now that that's all set up we're going to go to the trigger part so open the trigger editor and we're going to do oops undo that we're going to do a new trigger name it fishing or cast fishing

**[7:27](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=447s)** or you know whatever that works for for you new event unit generic unit event a unit and then you're going to want to use our um unit uses an item so you're going to go to conditions item type comparison item type of item being manipulated equal to and then scroll down to the item you just created the fishing pole new action you want to do if then else multiple functions new condition Bull in comparison go to unit and region

**[8:22](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=502s)** playable map area select region contains triggering unit equal to true then action and you're going to do if and else multiple functions all right here's where we're going to have to go to this x which is variables and create a new variable and we're going to name it fishing variable type is going to be um it's going to be integer right here AR and hit okay I'm just going to hit cancel because I already have

**[9:05](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=545s)** one under then actions before the if all conditions are true you're going to want to do set variable variable we just created one to um math random number between one and for me I did 100 you're going to want to go to new condition integer comparison up to the one we just created fishing under variable one equal to I put greater than or equal to for mine and I did 50 so you have a 50/50 chance of catching a

**[9:53](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=593s)** fish then action we're going to want to go to hero create item for hero it'll say create blank item and give to triggering unit you're going to change triggering unit to Hero manipulating item and make the item um a fish which you're going to have to make your your own custom fish I forgot to go over that I'll do that at the end of the video else [Music] action you're going to want to do game text message Auto timed display

**[10:36](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=636s)** 2 convert player to player group owner of unit and then change triggering unit to Hero manipulating item so this displays a message to the owner of the hero that's using the fishing pole you did not manage to catch any fish all right and you want to copy this paste it down here which if this is below this if condition which means if region o gen contains triggering unit equal to True which means you're not in

**[11:17](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=677s)** that region you're going to want it to say you need to be near water to use your fishing Pole or you know whatever you want it to say it's really all up to you you're also going to want it to say do nothing which is just okay and there you have it and if you want to know how to make an item for uh fish to actually give um just new custom item uh anything works if you want your fish to be edible use uh it item base

**[12:00](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=720s)** that's consumable like a potion if you don't want it to then use anything I like to use shimmering weed I don't want my fish to be edible just sellable or cookable and cooking will be in my next tutorial so you're going to want to name it fish interface icon find one that fits you I'm just going to keep it at shimmerweed for now um and you're going to want to make sure actively used false perishable

**[12:27](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=747s)** false and then like for me I want it to be able to sell for 500 gold to the shop so I'm going to set it at 1,000 gold which it sells for half of and there you go you're just going to want to go back to your trigger editor and then um create fish this is where you're going to add the item that you just created right here and that's how you do it I hope this helped you if you enjoyed this video please leave a like share the

**[13:03](https://www.youtube.com/watch?v=6rJnIiVGd9g&t=783s)** video subscribe and don't miss out on any of our future guides I'm Tyler and peace out
