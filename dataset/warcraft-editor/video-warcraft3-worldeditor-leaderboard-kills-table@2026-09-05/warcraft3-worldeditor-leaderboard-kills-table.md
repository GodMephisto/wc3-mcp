---
type: vendor-doc
package: video-thenerdsocietyadmin
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=Q-yuEIfX6E4
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 596
timestamped: yes
---

# Warcraft3 WorldEditor, LeaderBoard/kills table

Source: <https://www.youtube.com/watch?v=Q-yuEIfX6E4>

Uploader: TheNerdSocietyAdmin | Duration: 9m56s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

A tutorial in the warcraft 3 world editor explaining how to create a leaderboard (a board/table showing different stats like kills and deaths...)

## Transcript

**[0:01](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=1s)** hello this is tobin roby from nurseside.com back with another warcraft 3 world letter tutorial um this time i'm going to show you how to create a leaderboard and work out through a letter this is it's like the uh for example if you have the kills table who has the most kills stuff like that um first i just want to say thank you to all the positive comments stuff means a lot thank you uh but let's uh move on uh we only need the uh

**[0:32](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=32s)** the trigger editor for um making uh the leaderboard sorry um we'll just do this uh we need uh three variables and two triggers um you just create two triggers like that and just call them whatever you want and just call random stuff oh you go okay uh and just open your variable editor or and create three new variables uh one of them is going to be called kills like that and that's going to be an array

**[1:13](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=73s)** and it's going to be a integer didn't create a trail about a race and that's what i'm doing right now i'm going to show you a bit how arrays work arrays are like uh it's kind of hard to explain you can like store a bunch of different values in one variable so um you can have like kills one two three four five so you can act uh kills one equals one because two equals two kills three equals three and so on so you can have a bunch of

**[1:46](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=106s)** variables in one variable yeah and click ok create one new variable call it leader board and that's going to be a leaderboard type of verbal and we need um let's call this x because this is just going to be uh just a number really click okay and okay i gotta name those okay kills here and leaderboard oh bored yeah probably spilled that wrong but okay who cares uh we need an event for this uh we're going to like

**[2:34](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=154s)** display the leaderboard uh and you can't use map installation i don't know but it doesn't work so you have to use the time time ellipse for example let's say one second after one second of game time uh the leaderboard will display so uh no condition unless you really need that and the action will be um uh leaderboard create for all players title kills or if you want like whatever the value you want i'll say

**[3:12](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=192s)** deaths if you want that instead um you have these um things called multi boards also which can display kills and deaths on the same table can actually display a bunch of values on the same table on leaderboard you can only display one value um i'm going to show you how to create the multi board in a later tutorial uh just that that's pretty advanced so i have to like prepare a bit for it um a little bit creative for all players

**[3:41](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=221s)** called till the kills and now we're going to add all the playing players in the leaderboard because now the leaderboard is completely empty so click l for the leaderboard and uh add player oh sorry castle i just go here new action called p go in front playgroup pick your player and play group and do multiple actions uh click players and uh find all players matching condition and the condition will be um

**[4:17](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=257s)** player assault status so if um if i pick player up here big player okay okay so pick play equal equals to is playing then it will add it to the uh leaderboard because if let's say you have maximum until 10 players in your remap and let's say only two are playing and will show like 10 players and the the value that we just see here and all and then all of them and yeah but totally unnecessary just takes a lot of space in yeah so now

**[4:54](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=294s)** only the playing players will be shown in the uh uh leaderboard of course we gotta add them first uh level leaderboard uh find add player add oh pick player to well yeah like label name of the player find player player name name of pic player and values here is okay we forgot one thing we have to set the variable the leaderboard variable set variable set leaderboard to value to last created leaderboard

**[5:44](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=344s)** okay so just take last created labor works but i'm just gonna do the variable easier okay okay um now we need to um show the lead board just take a new action leaderboard find show hide hi no show leaderboard okay so now we got that right now now it will display um leaderboard but uh nothing will happen you say zero like forever so we had to add the numbers when someone kills a unit so a unit new event uh unit a unit

**[6:31](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=391s)** generic unit event unit dies if you want a condition let's say you only want to display hero kills for example only when the uh killing player kills the hero for example like dota or if you want only when he kills um units owned by a specific player for example you can just add an action no no sorry condition it doesn't matter the action will work anyway so action uh go ahead and do um set variable find

**[7:06](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=426s)** x and set x to uh player number player number of owner of unit owner of killing unit okay okay okay okay so this will be the array number of our kills um variable let's take one more set variable set kills and you see it has the index so you can set like a value like one and blah blah but we want the equals to be x so the owner player number of owner of kill unit will be the index of kills so let's say this is

**[7:55](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=475s)** player one play red then we'll kill swan equals like the kills of player red if it's player blue that kills somebody it will be two there and then we kills the index equals the kills of player blue so what we got to do now is take arithmetic uh take kills index x so itself in other words if this is player blue it will also be like player two here plus one so if this is two let's say play blue it will be kills two like blue

**[8:35](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=515s)** equals kills two plus one so um you don't have to create create like one unit dice and add kills thingy for each player you can just do it on one trigger way simpler like that all right so leaderboard change yeah change player value for owner of unit owner of killing unit so the owner killing unit to the leaderboard with number kills and x there you go you gotta finish um pretty simple pretty quick leaderboard

**[9:28](https://www.youtube.com/watch?v=Q-yuEIfX6E4&t=568s)** uh it's like the easiest way ever to do it or not the easiest but the quickest i hope you understood the array thingy i think i'm going to create another story about it it's kind of hard to understand but hopefully you understood this um yeah hopefully i'm going to try to make another trail about it but yeah um and that was it so um see you next tutorial bye
