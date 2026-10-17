---
type: vendor-doc
package: video-riozanrz
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=P6aOYSXRpeQ
source_url: video
extractor: manual captions (en)
exported: 2026-09-05
duration_s: 1339
timestamped: yes
---

# Warcraft 3 World Editor Tutorial  Creating AI Part 1

Source: <https://www.youtube.com/watch?v=P6aOYSXRpeQ>

Uploader: RiOZanRZ | Duration: 22m19s | Captions: manual (en)

## Transcript

**[0:00](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=0s)** hey guys it's Tomo here and today I'll be covering how to make your own AI scripts for Warcraft 3 now regardless of whether you want to make AI scripts for a custom game a Mele map or a campaign there are two separate ways you can go about doing this sort of thing and as a result I'll be splitting this video into two and this first video will just focus on one of these methods which is probably referred to as the most common

**[0:32](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=32s)** method people use when making AI scripts for the first time which is where you make use of the AI editor within the Warcraft 3 world ended up so once you click on this green button it will bring up a window that will allow you to begin making your AI so the very first thing you want to do is give it a name which we'll call it computer AI you select what race you want the AI to be for whether it be for human or under night

**[1:02](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=62s)** of or a completely new race altogether next you have a series of options that you can select which will determine how the AI will behave in certain situations some of these options are straightforward like for example repairing structures having heroes take items buy items etc my personal recommendation would be for a few of these options to have set player name on in that way when the AI is used throughout the game it will make use of

**[1:39](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=99s)** the name you specify here you have Mellie turned on regardless of whether or not you are using this AI for emily map or not simply because if you have it turned off you have to rely on conditions in order to have the AI build structures and train units otherwise the AI won't do either just so that you're aware of that have no mercy you have that I personally have it turned off because it makes the AI

**[2:19](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=139s)** deliberately attack the player when the player is at a disadvantage like for example they have hardly any units at their best so you're sort of punishing the player if they're not playing the game good enough and smart artillery is a similar option to have no mercy and where the AI will deliberately send siege units at the player who is at a disadvantage so you have that also selected some done slow harvesting now

**[2:52](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=172s)** depending on what sort of maps you want to make if you're making a custom campaign or in some cases a custom game you don't want the enemy AI to run out of resources in their gold mine or in your lumber so you have this option turned on in that way the player doesn't revert to strategies such as waiting for the AI to run out of resources and then attack otherwise if you're making a normal Mellie map then you have this

**[3:22](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=202s)** option turned off just to make it fair that all players have similar amounts of resources so it depends on what sort of map you want to make so once you've specified what options you'd like the next thing to do would be to select what units and buildings are trained or constructed by the AI which you can do so in this building tab here where by default it will have the Town Hall followed by the worker units as

**[3:50](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=230s)** well as how many workers are assigned to collect in gold in lumber and if it's at the central main base that the player starts off at the beginning of the map or if it's at a potential expansion if they expand at all there is also a heroes tab where you can select and choose what heroes the AI will train throughout the game and what order they are potentially trained in like for example you want there's a 17% chance that the

**[4:25](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=265s)** paladin will be the first hero followed by the arc mage than the Mountain King you can change these numbers to whatever you want you can also select that if a hero is trans like example the Paladin you can select which spells they learn first like for example you want the power and to learn devotion aura at level 2 instead of level 3 so we move this up you want the hero to learn Holy Light level 2 when the Paladin reaches

**[4:56](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=296s)** level 3 and therefore we move it up etc now depending on what sort of AI you want to build in most cases you generally won't use this because chances are the AI player in your map already has heroes actually in the map with their levels and default spells and while you may desire to have the hero gain levels throughout the mission and learn additional spells this sort of system only works if the AI is training

**[5:36](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=336s)** the actual heroes in the map now you will notice that the units in this building tab and heroes tab are all human units and that is because back in the general tab we have selected that our race will be based off the humans if we want to say for example have the walks as a race for the AI if we go back to the building tab you'll notice that the Town Hall and workers have been replaced with their orc equivalents as

**[6:09](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=369s)** well as the heroes forming select on the heroes tab furthermore you will also notice that you're only restricted to the heroes and units and buildings available for that specific race so if you decide to make an AI script for a completely new race you've made yourself you need to select as your race and what this will do is we go back to the heroes tab you are free to select whatever hero you want in the

**[6:44](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=404s)** game as well as being able to select whatever buildings and units etc you want to train or build now we could take this one step further and say we have created our own custom units within our map and we want to be able to specify the AI to build those units can we do it if you desire to have the AI build the units you've specifically created for your map or campaign then you can do so by making use of the custom data import

**[7:26](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=446s)** option the very first thing you would do is you would go into your actual map and click on object editor and as you can see we have a custom units that we want to use you go to file export all project data and we simply give it a name and click on save now we simply go back to the AI editor we click on import and we import the data that we previously exported from our map and as you can see it says that we have imported the data

**[8:09](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=489s)** successfully and if we go to the building tab and we click on add priority if we select custom we are able to select and use the custom units we have just previously we have just inserted so when it comes to building your own units and training they are done in priority so the Great Hall will be built first followed by the peons just as any standard AI you will generally want to have them construct an

**[8:45](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=525s)** altar next we may want them to construct a barracks etc now there are a few things you need to cater for as you're specifying what water the AI is building there unison constructing their structures because it does take into account the tech trees for example if he border the AI to train a grunt and there's no barracks then the AI won't build the grunt likewise if you ordered the AI to train a radar he won't train

**[9:32](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=572s)** the radar because first of all it requires not only Abby's dairy but it also in order to build the base dairy the Great Hall must be upgraded into a stronghold so just little things like that to keep in mind it also requires you know how the racers work in addition you also need to take into account the current food supply taken up by the AI player if you order the AI to construct or train units that

**[10:10](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=610s)** cost food and it is over the current food limit then it will simply highlight the text in red meaning that you need to have the AI build more structures that provide food which we can easily do so by ordering the AI to build a butter now there's a couple of things to help you with organizing this simply you can add priority by clicking on this button which will simply allow you to specify a unit or building that can be constructed

**[10:50](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=650s)** likewise you can also do the same thing by right-clicking on the white space and going add priority you can change a unit or structure by simply double-clicking on it or you can simply just left-click once and go edit priority now in regards to conditions in some cases you may want the AI only to train units or build structures depending on a set of circumstances like for example we want to say that the AI

**[11:29](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=689s)** will only upgrade their Great Hall into a stronghold if their current gold amount is plentiful so we go back into the general tab and we right-click on the white space and go add condition we would call this excess gold click on configure condition we want an integer comparison simply because we're comparing numbers to each other and we simply select if the current gold amount of the player is greater than equal to

**[12:13](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=733s)** one thousand then we go back to the building tab we right click on the stronghold we go modify condition and then we select the condition of if the current gold amount is greater than or equal to a thousand likewise you can also specify how many workers you want to train to harvest gold or lumber you can depending on if you choose custom as your race you can change the Town Hall they use you can specify if they use a

**[12:55](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=775s)** haunted goldmine or even the gold or lumber workers that are available to them one thing I forgot to mention earlier is the fact that you can specify which town that the AI does this sort of stuff where for example if we double-click on the Great Hall we can specify if it's for any town it's for their main base or expansion which may or may not be useful depending on what the AI does or what you want it to do generally this is

**[13:33](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=813s)** useful for Melling maps where the AI does expand to continue gaining resources otherwise for custom campaigns where the AI only has one base and they're never expand it doesn't really matter that much also it is possible to tell the AI to research upgrades through here which we can do so by clicking on build and we can select specific upgrade and we are able to select what upgrades the AI will research depending on if the

**[14:07](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=847s)** buildings and the tech tree requirements are met now you will notice that there is a hero first hero second and hero third option and they are different compared to when you go specific unit and train a specific hero because these three are referring to the heroes you want to train in this heroes tab right here where hero 1 is simply the hero specified in the first drop-down and then the second drop-down is Hero 2 and

**[14:44](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=884s)** third hero etc one other aspect of AI scripts we haven't covered yet is what is known as attack waves where you want the AI to send attacks at the plate which you can do so in those tacking tab first of all you would want to create the different attack groups you want which consists of what unit types are in the attack wave and in how many do so by hang on ad group or right-clicking on the white space and going ad group here

**[15:21](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=921s)** we can give it a name we call it brunt attack next we click on ad unit and we can select the unit type we want for example we want grunts we can select the quantity of grunts that will be part of the attack wave whether we want all the grunts probably not likely we want it to be a fixed value of say for example five grunts and we can continue adding units from men next we want to be able to tell the AI to send this attack group to

**[16:08](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=968s)** attack a player which is over in this attack waves box right here where we simply say the very first attack wave that will be sent will be of grunt attack and we can also specify how long the our weight in seconds before it actually sends the attack wave at the player there are also some additional options you can just specify with the attack waves like for example initial delay this is where you can order the AI

**[16:43](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=1003s)** from the beginning of the map to how long in seconds it will wait before it sends any attack waves you can also specify how many times the AI will repeat the last wave once they reach the end so for example we create a couple of waves and this one will wear down to 20 seconds and this one will waves 30 seconds so once it sends this last attack you can essentially tell the AI right we want the last wave attack to be

**[17:22](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=1042s)** repeated and you can or you can specify the sec attack wave to be repeated etc just so you don't have to continuously create attack waves if the player plays them up long enough in order for the AI to send that many waves now target priorities in most cases you would generally want the AI to target just the player or enemy players which are these three options here otherwise if you are making the AI for a

**[18:01](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=1081s)** melee map you do generally want to make use of them attacking creep camps or deliberately going out to a new goldmine to establish a an expansion or attack with their allies but otherwise if you're just making a basic AI where they just attack the player and nothing else then you don't need these additional options and that pretty much covers how to order the AI to build structures build units stat heroes upgrades and

**[18:39](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=1119s)** attack players now there is a test configuration tab which allows you to test your AI in a very specific scenario but generally and my personal preference is that if you want to test your AI the best and most easiest way of doing that is by actually testing the AI within the map you're going to use so first of all we're going to say this AI we're gonna call it script click on save and in addition to saving this AI we are also

**[19:24](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=1164s)** going to go to file export script and when it could call it the exact same name now what I just did I will briefly go over if we just open the file again now you'll notice that there are two files one of W AI and one of a I the difference between them is that the AI is the actual AI script that is what was created when we went in to file export script and is what we'll be using when we actually test the air within our map

**[20:05](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=1205s)** the dot W AI file on the other hand is the file that allows us to edit the AI through the AI editor so without this file we come we can no longer edit the AI through the AI editor but if you lose the AI script you can just simply recreate it from the W AI file by going into file export script now that we have our AI we can go into our map we go to import manager we right-click on the white space and go import file we would

**[20:51](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=1251s)** select the AI we wish to import we can reduce the file path just to make it more easily readable next we would go into the triggers and ignoring this we click on actions go to AI and we can either select start Meli AI script or start campaign AI script now in regards to which action you use you can use either they both essentially do the same thing so you would select what player slot the AI is for in this case we want it to be player

**[21:41](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=1301s)** blue and then we can select the AI script we wish to use and then you're free to save the map and test the map within Warcraft 3 and that essentially does it for this tutorial I hope you guys find this useful if you did like this video feel free to like it and subscribe to my youtube channel in the next video I will be providing a very different way how we can go about making our own AI scripts which involves coding but until then

**[22:15](https://www.youtube.com/watch?v=P6aOYSXRpeQ&t=1335s)** I'll catch you guys next time bye now
