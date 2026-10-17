---
type: vendor-doc
package: video-lake
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=sHnmKblSEPI
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 618
timestamped: yes
---

# Round Dev Log - Part 1: The beginning!  [Warcraft 3 Map Making]

Source: <https://www.youtube.com/watch?v=sHnmKblSEPI>

Uploader: Lake | Duration: 10m18s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

First video in the Round Dev Log series - a series where I take you through the development of an entire round in my Warcraft 3 map "Gladiator"! :)

For more info on the map and download visit:
https://www.hiveworkshop.com/threads/gladiator-alpha.310920/

Wurst Script:
https://wurstlang.org/

Credits:
All music and sound effects are taken from Warcraft 3 and therefore belongs to Blizzard.

## Transcript

**[0:00](https://www.youtube.com/watch?v=sHnmKblSEPI&t=0s)** [Music] hey guys and welcome to this video this will be the very first video in a series where I'm going to develop around for my gladiator map for part 3 and showcase the entire development to you guys if you don't know what my map is all about then I suggest you check out the link in the description to the height workshop there's a page where it describes pretty much everything so I don't want to do

**[0:42](https://www.youtube.com/watch?v=sHnmKblSEPI&t=42s)** any more boring talk let's just get this video rolling and start developing a really cool round ok so when I want to make a new round I usually have some sort of idea to build upon I get these idea random and when I do I write them down and when I want to create the round i build upon these ideas and this is also the case for this round I want to do something with the NAG arrays for this round and I don't know what kind of

**[1:16](https://www.youtube.com/watch?v=sHnmKblSEPI&t=76s)** Units yet I just know that it has to be something with nag arrays and perhaps some other sea creatures or something like that I also have an initial idea for mechanic which I'm quite sure I want to implement into this round this mechanic is an ability cast by these Naga sirens and the ability is an ability they will be channeling most of the time and it will summon a tornado for each gladiator in the map and then

**[1:48](https://www.youtube.com/watch?v=sHnmKblSEPI&t=108s)** each of those Tomatoes will follow the corresponding gladiator and the gladiators will have to run away from the tomato because standing arid will do damage to you but apart from that I'm not quite sure exactly which units that we are going to use or which kind of mechanics are we going to use so the very first step will be to figure that out so here I'm just trying to figure out what kind of units that I

**[2:15](https://www.youtube.com/watch?v=sHnmKblSEPI&t=135s)** want in the round usually I have some sort of idea what units I want or at least from what kind of environment that I want them in this case I want something but nyancy creatures and such but in the end I decided for these units for the grounds so we're thinking about titles and hydrous and all that but I actually ended up going primarily with maggots and a single other unit which is the water limit but this is going to be a

**[2:48](https://www.youtube.com/watch?v=sHnmKblSEPI&t=168s)** solid unit so it's not gonna be like a primary unit of the round and then of course there's the tornado but I'm not sure that's going to be an actual unit I might have some other trick up my sleeve to fix that problem I also came up with the mechanics for this round so let me just really crank explain what this round is going to be about we're going to have these three units these primary three units at the nagas and the first

**[3:15](https://www.youtube.com/watch?v=sHnmKblSEPI&t=195s)** journalist is the siren which I talked about earlier she will have that tornado ability and aside from that she will also peel some sort of lightning damage to all units around her because she won't have an auto attack and the other unit over here is the storm collar which basically just samui unit which summons these water elementals and these water elementals will have a line attack just like crushing wave line attack not as

**[3:43](https://www.youtube.com/watch?v=sHnmKblSEPI&t=223s)** big as the crushing wave but but similar to it in the last unit is the Myrmidon and the Meriden is a basic melee unit they have a higher HP than the other unit they don't have so much damage but they have an ability which is similar to the gladiators own whirling strike no sorry not whirlings right the wounding strike which slows and deals damage all the time all right so the next step is to create these units these just place

**[4:14](https://www.youtube.com/watch?v=sHnmKblSEPI&t=254s)** all the units so we have to create the actual units that we will use within the map this pretty tedious process there are a few things that I want to change for all units such as their health regeneration their attack type their armor type their movement speed want to hide them from the mini-map and so on so this I'm just doing real quick for all the units all right so these are the actual final units I've scaled them up correctly I'm

**[4:50](https://www.youtube.com/watch?v=sHnmKblSEPI&t=290s)** adjusted their selection circles just that their colors and everything like that so they are ready now to be implemented into the map so let's go and do that right now okay I'm working in worst if you're curious which is a coding language for worker 3 that compiles into the Jazz language I won't go into too much detail with how I'm doing this all I'm saying right now is that I have this code which represents the round in

**[5:22](https://www.youtube.com/watch?v=sHnmKblSEPI&t=322s)** itself and I can adjust some settings fall around such as the name and health ops bone so on and then we have a unit spawner for each type of unit that spawns sometimes I implement more units bounine for the same type of unit which allows me to progress this spawning of that unit type so I already set up the round settings and all that so I'm just going to add the unit spawner one for each unit and see if the units are

**[5:51](https://www.youtube.com/watch?v=sHnmKblSEPI&t=351s)** actually spawning into the map so I've just copied the unit spawner setup from another round and put it into this round and I'm just changing the settings for each units bonus so if it's for one of each unit this includes setting the time at which they start spawning and how often they spawn and of course the unit ID of the unit that it is spawning all right and here goes the first test of the unit's bonus in the map and

**[6:22](https://www.youtube.com/watch?v=sHnmKblSEPI&t=382s)** everything seems to be working just fine I didn't realize however that the elementals are a little bit small and then of course there is a temporary problem with the sirens and water mentals not attacking anything but that's okay because they are not meant to attack anything they have some sort of special attack instead you know the tornado and a crushing weight so I'm just going to fix their size real quick and then I decided

**[6:51](https://www.youtube.com/watch?v=sHnmKblSEPI&t=411s)** I already wanted to add some effects when they spawn I want to add some special effect and some sound effect I made it very simple to add these into the unit's poni in the code so it's not going to take too long if I could decide for a sound that is initially thought that I would use the Naga sounds but I didn't really like them so I trying to come up with something different and I actually ended up using Snapdragon and

**[7:36](https://www.youtube.com/watch?v=sHnmKblSEPI&t=456s)** turtle sounds for two of the units the male neck then I go siren I use the regulan I hear sirens sound and I also set up a special when they spawn so instead of the auricular polymorph things they make like that crushing waves flash kind of thing on the ground which yeah well that seems a little bit more fitting okay so the final thing that I realized I have to fix is the unit team colors there's a

**[8:16](https://www.youtube.com/watch?v=sHnmKblSEPI&t=496s)** park where none of my units actually they automatically get the color that I set for them even though I set it into the world editor each unit object I've come up with a solution for this by implementing a system in my code so that I can add specified or a custom color for all unit spawning into the map off a certain type so I'm just gonna adjust that into the code right now and then everything should be good and sure

**[8:46](https://www.youtube.com/watch?v=sHnmKblSEPI&t=526s)** enough all their colors are updated into the map and all spawners are actually working just fine right now so I think it's fitting to stop here for today we've created all the units we've implemented them into the round and we set up the round so we've done all the framework for this round and we're ready to implement the mechanics next time so I think next time we will actually do the most difficult I would say or more

**[9:15](https://www.youtube.com/watch?v=sHnmKblSEPI&t=555s)** interesting mechanic say the tornados from the Naga siren cool so if you liked this video give it a thumbs up and if you want to know when the next part is coming out just hit the subscribe button and anyways I will see you next time and thanks for watching
