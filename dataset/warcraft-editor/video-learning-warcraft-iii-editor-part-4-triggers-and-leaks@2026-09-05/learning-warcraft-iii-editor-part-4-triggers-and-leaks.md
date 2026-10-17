---
type: vendor-doc
package: video-masterja
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=58Hf6B6hgc4
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 699
timestamped: yes
---

# Learning Warcraft III Editor - Part 4 (Triggers and Leaks)

Source: <https://www.youtube.com/watch?v=58Hf6B6hgc4>

Uploader: Masterja | Duration: 11m39s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Here i show how to make basic triggers and remove leaks. This is very important lesson, it will affect map performance, especially in RPG maps or other ones that are played for hours.

## Transcript

**[0:00](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=0s)** hello guys it's mastera here and I'll be doing an world at tutorial right now I'm going to show you a basics of triggers the types of triggers and stuff like that okay so let's begin to enter the triggers you press the F4 or the trigger Ator button and you're up with a panel when you first start this panel you're only going to have one category here it's going to be called initilization if you don't want melee map just delete

**[0:32](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=32s)** this wall thingy uh well right now I have leak removal but that's the prescripted thing I I wrote earlier to show you how to remove leaks we're going to use that later on okay first thing you're going to do is create new category and name it whatever you want let's just name it um start and make a new trigger what will this trigger do let's just do some basic triggers it's going to be called start kill okay what does

**[1:04](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=64s)** this thing do uh you have first off let me tell you you got two languages uh first language is GUI and that's the one uh we're going to be using and a second language uh is JZ JZ is an advanced language that requires lots of skill patience and stuff like that to learn and you can convert your text to Jazz by going to edit and then convert to C text as you can see J has things you can write I know very little of Jaz so I can

**[1:40](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=100s)** just typ type this that's all I know and it means nothing so let's just leave J out of this and we're going to make map in guy gy okay so what's this trigger going to do uh every trigger has event condition and action event that means what will trigger this trigger so let's just say uh there's a lot of things here let's just take time lapsed if time lapsed is Seconds the conditions they usually you

**[2:16](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=136s)** know check if something is true or not if it's true then they proceed to actions if not then whatever so let's just check uh what condition should we have let's just say owner off then you get to choose when you um choose some of these blue thingies another window pops up and then you either choose one of the pr players or you get to choose players Bound by variables or you get to choose a function let's just say function owner

**[2:48](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=168s)** of unit of which unit let's select the unit and we're going to make this thing here this guy is going to be the unit the Knight okay now if or of Knight of Knight is equal to player one okay then what are we going to do action you get lots of actions here uh let's just skip the units kill kill the the Knight yes well this is some of the some most basic trigger there are far far more advanced triggers and sometimes you're

**[3:26](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=206s)** going to be using locations let's just say uh like this let's find unit group be care unit and playable map area and unit let's just create special effected their point special effected unit position of picked unit and the effect is going to be random Thunderclap yeah that's fine I think it's okay it doesn't really matter I just yeah Thunder that's it okay and what we going to do we're going to just

**[4:01](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=241s)** um uh floating text we're going to create floating text that says this add the unit position of picked unit okay well this is a harmless trigger no wait it isn't why not because every time you use a trigger and you use a new location for example uh Pi every unit in units in playable map area this is a new location and Warcraft saves this location but doesn't remove it after you use this and that's the

**[4:33](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=273s)** problem when you save location and don't remove it it just causes extra space in a map and that's called a leak there are lots of leaks in this thing and I'm going to teach you how to remove them okay first of all uh we got a leak removal I mean uh trigger I just made that so I can show you guys what does this look like um to make one one of these leak removers uh you go new action find the cuum script and enter this okay now what

**[5:07](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=307s)** you're going to type here is always go call first space and then depending on what your thing is it either goes remove something or destroy something if it's unit group for example you go destroy group space u uh wait udg slash and the name of your group let's just call it group okay well I already have this Trier and a bunch more uh destroy group is used for unit groups remove location is used for points the ones I mentioned

**[5:46](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=346s)** earlier uh destroy force is used for player groups remove wrecked is used for Regions which I'm going to show talk about later destroy lightning this is uh used for all the lightning effects that you have on a map destroy text tag is used for floating text and Destroy timer is used for timers now all these require the first word to be call and the thing between this how it's called first word is udg SL something and then the the

**[6:22](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=382s)** name of your variable now what is variable you got to be wondering you see this x up here click on it and variable uh will open this variable window when you create new variable let's just name it um I don't know group or what whatever this thing is called and we're going to find a unit group okay now we have unit group named group and we failed a bit because this thing removes unit group not just group

**[6:53](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=413s)** so let's just name this unit group okay so right now this trigger is going to remove unit group we don't have a event this is just showing you how it's done and for also another two things that leak are units and special effects however unlike other thingies that leak uh units can be removed by simple unit at expiration timer where the game will automatically remove after certain number of seconds and special effect

**[7:26](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=446s)** destroy special effect this is also quite nice one so what are we going to do right now okay first uh unit kill unit this since this thingy uh kills this unit he won't cause any more space well it would be optimal to remove him using the the the leak remover expiration timer however most of the triggers require a corpse so we will take our time besid it takes uh around 10 million leaks uh for map to start lagging yeah

**[8:03](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=483s)** I've counted it okay well what we're going to do right now is um special effect we're going to remove special effect like I said before uh wait where is this special special special special special destroy last created special effect okay this will play special effect then destroy it uh okay unit PE every unit in units in playable map area we're just going to set unit group the one I think use earlier equal to uh wait a second we're

**[8:38](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=518s)** just going to make point point is going to be a location okay so let's just pick every unit in units in no let's just say pick every unit wait oh yeah units in Point where's the point oh oh my bad my bad my bad this is any direct region okay this is more like it and let's just set before that we're going to set set this region to playable map area playable map area right we got units in playable map area and and right

**[9:29](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=569s)** now we're going to remove this thing into unit group pick every unit in unit group set this thing create special effect create floating text add position of picked unit now this is a problem we're going to make a new thingy no wait it's here Set set set set set set set edit variables new variable this is going to be point point point point okay set Point Unit position of picked unit now create special effect at

**[10:07](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=607s)** point and create floating text at Point okay now you just remove these leaks by copying this thingies here right now we used the Let's see we used region unit group uh point and floating text so we're going to use four of these to remove just copy these things create scripts at the end of a trigger and it's going to be removed and that's about it for triggers uh if you want to learn jaas you should check out Hive Workshop they

**[10:48](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=648s)** have a bunch of tutorials there however I do recommend if you want just casual map making you should stick with GUI it covers most of the things most of the basics the only thing that's uh that can be done in Jaz and not GUI is leak removal however instead of uh transferring wall trigger into uh how it's called into into this Jaz you can just do it in GUI however if you want to do it in jazz this is how it looks like call destroy

**[11:20](https://www.youtube.com/watch?v=58Hf6B6hgc4&t=680s)** group udg unit group and stuff like that okay well I hope you liked my guide and I'll see you next time in abilities Ator since right now we have uh we know how to use trigger Ator we can also use ability Ator later on okay ciao guys have fun
