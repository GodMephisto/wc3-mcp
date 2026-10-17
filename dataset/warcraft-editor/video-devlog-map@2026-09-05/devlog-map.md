---
type: vendor-doc
package: video-dodoro-s-lab
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=YJ_SDKSzq5E
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 247
chapters: 10
timestamped: yes
---

# Devlog - Map

Source: <https://www.youtube.com/watch?v=YJ_SDKSzq5E>

Uploader: Dodoro's Lab | Duration: 4m07s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

In this Devlog Dodoro starts working on a new game idea. How does he make a map? Is he afraid of the ice? What about his previous projects? Make sure you stick around to find out.



0:00 Intro
0:12 Recent Projects - Combathrone
0:19 Recent Projects - (very magical) Wizards
0:36 Recent Projects - MonoGame extension
0:46 New Idea
1:22 Game Loop - Overworld Map

1:59 Game Loop - Battle
2:25 Current Progress - Menu
2:31 Current Progress - Custom Maps
3:35 Outro


#unity #devlog #indie #gamedev

## Chapters

- [0:00](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=0s) Intro
- [0:12](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=12s) Recent Projects - Combathrone
- [0:19](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=19s) Recent Projects - (very magical) Wizards
- [0:36](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=36s) Recent Projects - MonoGame extension
- [0:46](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=46s) New Idea
- [1:22](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=82s) Game Loop - Overworld Map
- [1:59](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=119s) Game Loop - Battle
- [2:25](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=145s) Current Progress - Menu
- [2:31](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=151s) Current Progress - Custom Maps
- [3:35](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=215s) Outro

## Transcript

**[0:01](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=1s)** [Music] welcome to my game development laboratory i did it yet again i started a new project why is this the case this time for my first project i got limited by my extension not being quite ready and lacking stability the wizard game i was making quite construct grew massively and constructs lack of code proper condition checking inheritance not being able to pass objects around the lack of function

**[0:30](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=30s)** parameter names and many more things started showing more clearly so the project is paused for now rebuilding my extension from scratch even while going great is exhausting it consists of only coding and not actual game development so i'm taking a break from that as well here is where the new game idea comes in this time i will be using unity because of how suitable its scriptable objects and animations

**[0:53](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=53s)** are for that case and because i have some experience with the engine and the language the idea is inspired by the mountain blade series a little facebook game i used to play a decade ago the heroes of might and magic series and the pokemon idea of encountering monsters i found a pack of assets that will go perfectly with the idea it contains six map teams the pack has a lot of weapons and many characters that are also

**[1:19](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=79s)** customizable and will probably be randomly generated in the game the game loop of the idea is the following we start on the overall map with a single character that we can customize traveling around the map triggers all kinds of random events from robbery quests battles and recruiting units to join you two completely useless ones that simply help the atmosphere of the game any event will present us with some

**[1:43](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=103s)** options on how to approach the situation for example encountering a quest will make us choose whether we want to accept it or not encountering a battle will make us choose whether we try to escape fight or give something away to our enemies to let us go if we were to choose fight this is the scene we would approach our character will be at the top giving orders to the units and below him will be the unit

**[2:07](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=127s)** themselves trying their best to follow the orders and hopefully win the fight in the process the items will be mainly weapons and armor for our units and quest items the unit stats will be simple just damage and health the rest goes as expected from an rpg and that's everything about the general idea of the game here is my progress so far i made a small menu clicking play will make us choose a map this map is a text file in our game

**[2:33](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=153s)** directory i've made a simple way of creating maps like so we have a theme a palette and the map itself this feature will be later swapped with the in-game map editor which can be seen as disabled in the menu the map editor will handle multiple teams per map and custom events those are not supported with the current approach here is how the map looks like once we start the game we have some terrain and some buildings

**[3:00](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=180s)** all of the teams contain houses that we can mostly recruit units from and some of the teams contain mules mines castles and villages from which we will mostly take quests and battle we occur almost everywhere switching the map team is as easy as just changing a word in the map file [Music] some errors pop up at the top of the file if some invalid map parameters were met the map isn't loaded in the game in this

**[3:34](https://www.youtube.com/watch?v=YJ_SDKSzq5E&t=214s)** case and that sums about everything once we are done with the combat and the events the game will be practically done not many more systems will be added after that but the game will be expanded through new weapons characters maps and events constantly i believe the scope is reasonable this time and i hope i can finish the game in a few devlogs from now if you would find this interesting make sure to stick around
