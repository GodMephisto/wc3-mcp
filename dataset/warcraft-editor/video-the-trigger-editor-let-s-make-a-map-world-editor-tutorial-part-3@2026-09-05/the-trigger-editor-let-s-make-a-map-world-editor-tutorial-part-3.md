---
type: vendor-doc
package: video-abelhawk
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=EVZ5740YaGA
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 5290
timestamped: yes
---

# The Trigger Editor ▸Let's Make a Map! World Editor Tutorial Part 3

Source: <https://www.youtube.com/watch?v=EVZ5740YaGA>

Uploader: Abelhawk | Duration: 88m10s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

A condensed version of a Warcraft 3 World Editor tutorial livestream from 2019.

►Watch the full video here: https://youtube.com/live/je2tmwPxS5Y

_____
►Join my Discord server! https://discord.gg/RcE8GjjyJK
►Click "Join" to become a Supporter, Member, or Loyal Member!

## Transcript

**[0:09](https://www.youtube.com/watch?v=EVZ5740YaGA&t=9s)** Welcome everyone to another episode of Let's Make a Map with AbleHawk.

**[0:17](https://www.youtube.com/watch?v=EVZ5740YaGA&t=17s)** Today, I'm really excited to talk about triggers. This is one thing that I discovered in Starcraft the Starcraft map editor.

**[0:27](https://www.youtube.com/watch?v=EVZ5740YaGA&t=27s)** And it's been super super useful.

**[0:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=31s)** It's taught me a lot about logic. I have a coding job now today partly because of the things that I learned while tinkering around in this map editor and also the what's it called the Galaxy editor? Maybe it's just called world I can't remember what it's called for Starcraft. Anyway, So, today I'm going to go over a few different levels of complexity for triggers.

**[0:54](https://www.youtube.com/watch?v=EVZ5740YaGA&t=54s)** I want to be able to like be really simple for those who haven't had any experience with triggers because it can be really useful.

**[1:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=61s)** And then from there we can have some more complicated ones like a cool boss fight.

**[1:09](https://www.youtube.com/watch?v=EVZ5740YaGA&t=69s)** And I realized last week that I there's no way I'm going to get anywhere close to finished on this map. Like I keep thinking that I'm you know that we're going to keep working on this and stuff but there's it's not going to happen so I just need to accept that it's probably not going to happen. So, let's just pick like the best things to work on basically.

**[1:27](https://www.youtube.com/watch?v=EVZ5740YaGA&t=87s)** Let me introduce you first to the trigger editor.

**[1:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=90s)** So, on the trigger editor you have three different things that you need to know about.

**[1:37](https://www.youtube.com/watch?v=EVZ5740YaGA&t=97s)** There are events, conditions, and actions.

**[1:41](https://www.youtube.com/watch?v=EVZ5740YaGA&t=101s)** So, events are things that cause the trigger to fire. Like they're quite simply events. Like that's a good name for these I think. Uh something has to set off the trigger and make the actions happen and that's what the event is.

**[1:56](https://www.youtube.com/watch?v=EVZ5740YaGA&t=116s)** A condition is a condition that has to be met in order for the actions to go.

**[2:02](https://www.youtube.com/watch?v=EVZ5740YaGA&t=122s)** So, like an event might happen, but if the conditions aren't met, the trigger won't fire.

**[2:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=127s)** Um for example, if you you enter an area, like if you if you want to make an a person enter an area and then the gate opens.

**[2:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=136s)** Entering the area would be the event, and the action would be open such and such gate.

**[2:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=142s)** Uh if you wanted it to make it so that you had to have a key to open the gate first, then there you could add a condition that said unit, like the hero that goes up to the gate has to have a key in their inventory.

**[2:33](https://www.youtube.com/watch?v=EVZ5740YaGA&t=153s)** Uh and that way if you didn't have a key, the gate wouldn't open. So, that's it in a nutshell.

**[2:38](https://www.youtube.com/watch?v=EVZ5740YaGA&t=158s)** Sometimes it's a little bit difficult to wrap your head around what you want done, but like honestly practice is the best thing. So, if you guys haven't messed around with triggers or if they seem scary to you, I I invite you to like work along with me right now if you want or simply just start out with the basics and stuff because you can do a lot of stuff on here. So, let's just start with something really basic. I think it'll be

**[3:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=181s)** pretty simple if we just have it so that the gate opens up in the daytime and then yeah, let's just just do that. All right, so the event in this case would be and these are sorted by like their category. So, the time of day is just internally in the game, but there's also like if it's a unit event, player, that sort of thing. So, game if the when the time of day is 30. I don't know if it's 6.5.

**[3:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=212s)** Yeah, so it'd be 6.5. Okay, so like half an hour after sundown all of the gates close.

**[3:43](https://www.youtube.com/watch?v=EVZ5740YaGA&t=223s)** So, you go close.

**[3:46](https://www.youtube.com/watch?v=EVZ5740YaGA&t=226s)** But basically but this is super basic.

**[3:48](https://www.youtube.com/watch?v=EVZ5740YaGA&t=228s)** Um so let's just select all the gates here.

**[3:54](https://www.youtube.com/watch?v=EVZ5740YaGA&t=234s)** And make each of them open up.

**[3:56](https://www.youtube.com/watch?v=EVZ5740YaGA&t=236s)** Oh, wait. I just realized that this is night time. So, oh, yeah. Never mind.

**[3:59](https://www.youtube.com/watch?v=EVZ5740YaGA&t=239s)** Close them. Yeah, I'm going to close them first and then open them.

**[4:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=247s)** So, again, these actions will all just Did I do that one already?

**[4:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=252s)** 2520 Okay, I think that's all of them.

**[4:15](https://www.youtube.com/watch?v=EVZ5740YaGA&t=255s)** Okay, so there there doesn't need to be any conditions for this or anything. Every time that it becomes 6:30 at night, all the gates will close.

**[4:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=263s)** So, if Dante, our hero, is out questing or something, then he'll get locked out, basically, if that happens. So, actually, let's name it close.

**[4:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=274s)** And then I'm just going to copy this trigger cuz it's very similar.

**[4:37](https://www.youtube.com/watch?v=EVZ5740YaGA&t=277s)** Although, now that I think about it, I'm going to have to change every single one anyway. Anyway, so uh at dawn, uh I just realized that that is We're doing military time here. So, it's actually 18:50. So, at night, everything closes.

**[4:53](https://www.youtube.com/watch?v=EVZ5740YaGA&t=293s)** And in the morning, 6:00, everything opens. So, I'm just going to change each of these to open.

**[5:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=301s)** Super basic triggers, but that's how it'll work. Every time that it's morning, the gates will open. At night, the gates will close.

**[5:08](https://www.youtube.com/watch?v=EVZ5740YaGA&t=308s)** And they'll give everybody like a half hour extra just to get inside before they close the gates. So, once you hear that wolf howl, you're like, if you need to go back to to the town, then you can go.

**[5:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=320s)** Quests, that's a great idea.

**[5:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=323s)** Let's do quests.

**[5:25](https://www.youtube.com/watch?v=EVZ5740YaGA&t=325s)** So, let's just start by adding a quest.

**[5:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=329s)** Oh, yeah. Map initialization is just where you want to start everything like that's going to be set up before the game, like right when you start the game. So, let's start Let's create some quests here.

**[5:39](https://www.youtube.com/watch?v=EVZ5740YaGA&t=339s)** Uh let's create a required quest. We didn't really think much about the story about this, but who cares? Basically, you need to destroy all the um You need to destroy all the bosses that are causing mayhem in the world, and then they'll open the gates to the hellish area, and then you can go fight the demons. So, um let's just call this like, I don't know, the demon king or something like that.

**[6:08](https://www.youtube.com/watch?v=EVZ5740YaGA&t=368s)** So, you just have a you have a title for the quest, and then you have a description. I'm not going to spend much time on this. Obviously, this isn't not that important, but the demon king seeks to destroy all life.

**[6:21](https://www.youtube.com/watch?v=EVZ5740YaGA&t=381s)** Destroy his minions, and the path will open.

**[6:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=390s)** Uh I mentioned one of my videos that uh ambush, do you remember ambush with like the shadow meld, is has always been the default icon.

**[6:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=400s)** Uh yeah, it's right there. So, it's kind of funny that that is um that was whenever you open a new quest, especially in the Reign of Chaos editor.

**[6:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=411s)** Uh that was the quest icon, the only one that was available at the time, and at the time you couldn't just scroll through and look at all these different icons. Like, you had to know the path, and it was kind of frustrating. So, for this one, I can't remember who's who's the demon king again that we put for the boss? I think he's like Kil'jaeden or something. Yeah. So, for that, let's just make the icon be like his

**[7:17](https://www.youtube.com/watch?v=EVZ5740YaGA&t=437s)** face.

**[7:18](https://www.youtube.com/watch?v=EVZ5740YaGA&t=438s)** Okay, so there's Kil'jaeden's icon. That works perfect for the demon king.

**[7:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=443s)** Um so, you create the quest, and then you enable the quest.

**[7:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=450s)** You can always change the description, change the title.

**[7:33](https://www.youtube.com/watch?v=EVZ5740YaGA&t=453s)** Okay, so let's just let's just see what happens. Like, just with that, let's see what happens first.

**[7:39](https://www.youtube.com/watch?v=EVZ5740YaGA&t=459s)** Let's go in. I really hope that Warcraft 3 opens and stuff. Cuz we had a little bit of problems with that last time, but I tested and we'll see if it works.

**[7:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=470s)** Okay, that actually worked just fine.

**[7:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=472s)** So, there's a quest, there's the icon, there's the title, there's a description. So, what we need now is requirements or objectives or whatever they're called. Okay, so create a quest requirement for last created quest, which is the demon king, and this one is um Okay, Sandsting the scorpion is the boss that I'm going to talk about. So, we'll say defeat Sandsting the giant desert scorpion. So, that's

**[8:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=500s)** one of the requirements. Now, here's something that's really, really important for you guys to learn, variables. So, we need to create a variable so that later on in a different tick trigger, we can know which requirement that is. Like when you kill Sandsting, we need to know, okay, which requirement did that fulfill.

**[8:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=520s)** So, oh man, is it black again?

**[8:44](https://www.youtube.com/watch?v=EVZ5740YaGA&t=524s)** Okay, there it goes.

**[8:46](https://www.youtube.com/watch?v=EVZ5740YaGA&t=526s)** Uh hopefully it'll stay that way, but I don't know.

**[8:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=530s)** Okay, so you need to we need to create a variable so that later on we can tell which requirement we're talking about. So, there's tons of different kinds of variables. I'll probably Yeah, I'll probably go over quite a few of these because they're really important. But, basically what we need is So, this specific one is a quest requirement. So, this one will be called Sandsting Q wreck. I don't know.

**[9:17](https://www.youtube.com/watch?v=EVZ5740YaGA&t=557s)** Just so we remember in case there's other variables after this Sandsting.

**[9:21](https://www.youtube.com/watch?v=EVZ5740YaGA&t=561s)** All right, so we make that variable and we set that to last created quest requirement.

**[9:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=568s)** So, now Sandsting Q wreck is this requirement. So, when we kill Sandsting, we can say uh mark Sandsting Q Rec as completed as a quest objective and stuff.

**[9:42](https://www.youtube.com/watch?v=EVZ5740YaGA&t=582s)** And we can create other ones, obviously.

**[9:44](https://www.youtube.com/watch?v=EVZ5740YaGA&t=584s)** Like if this were a full map, we would.

**[9:46](https://www.youtube.com/watch?v=EVZ5740YaGA&t=586s)** Like we'd say, what are the other boss names that we came up with? Mal'Ganis, that guy's name. So, so like defeat Mal'Ganis the golem and so on and so forth.

**[10:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=601s)** And then we would make a new variable for that one.

**[10:05](https://www.youtube.com/watch?v=EVZ5740YaGA&t=605s)** All right. So, basically that's how you create quest requirements.

**[10:09](https://www.youtube.com/watch?v=EVZ5740YaGA&t=609s)** Let's make like an optional quest.

**[10:11](https://www.youtube.com/watch?v=EVZ5740YaGA&t=611s)** Because I want to test these to make sure they work and stuff. So, let's do an optional quest titled uh, artisan ship.

**[10:24](https://www.youtube.com/watch?v=EVZ5740YaGA&t=624s)** So, collecting raw ingredients will give you crafting options.

**[10:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=631s)** And the picture will be Is that a flower one?

**[10:37](https://www.youtube.com/watch?v=EVZ5740YaGA&t=637s)** Oh, I know.

**[10:38](https://www.youtube.com/watch?v=EVZ5740YaGA&t=638s)** Death and Decay has a picture of a rose on it. There we go.

**[10:42](https://www.youtube.com/watch?v=EVZ5740YaGA&t=642s)** That's actually kind of fun to do with the quests, by the way, is like be creative with the icons. Like cuz you're trying to illustrate what the quest is about, not necessarily like what specific units are in it and stuff like that. So, sometimes you might have a interesting one like that and it's kind of fun to like choose an ability or something that illustrates the quest really well.

**[11:02](https://www.youtube.com/watch?v=EVZ5740YaGA&t=662s)** So, uh, collect a We'll just start with one for now. Let's just keep it with uh, World of Warcraft major oil.

**[11:11](https://www.youtube.com/watch?v=EVZ5740YaGA&t=671s)** Collect the major oil.

**[11:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=673s)** We'll make a variable for that.

**[11:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=676s)** Uh, major oil Q Rec. So, let's make an item that grows over here in these flowers called major oil.

**[11:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=691s)** Let's just make Mageroyal a power up, meaning I don't want it to be in Dante's inventory, necessarily. I just want it to be it like disappears as soon as he picks it up and then we'll just mark it in our triggers that he has it. So, that flower death and decay, perfect.

**[11:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=710s)** And this is called Mageroyal.

**[11:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=715s)** And I don't think we'll have to worry about that cuz he won't be able to buy it, at least yet. And then a flower useful in the brewing potions.

**[12:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=727s)** Uh and then we'll take out the ability of chest and gold, so it won't give you gold coins. Well, so I don't think I talked about items last time. I should have a little bit, but description is what it says like when you click on it.

**[12:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=742s)** And then this is like when you hover over it when it's in your inventory, so anyway.

**[12:27](https://www.youtube.com/watch?v=EVZ5740YaGA&t=747s)** Okay, so flower useful in the brewing potions. Okay, so we have our Mageroyal.

**[12:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=751s)** So, now we go to items.

**[12:33](https://www.youtube.com/watch?v=EVZ5740YaGA&t=753s)** Here's our Mageroyal. Oh yeah, I forgot I forgot to change the model, so let's change the model to uh Shimmerweed. I think that's what it's called, there it is. Okay, so that's our Mageroyal right there, besides it's the same color as the roses, so whatever.

**[12:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=767s)** Let's make a new one called quests or quest progress.

**[12:53](https://www.youtube.com/watch?v=EVZ5740YaGA&t=773s)** Let's make a new trigger and this event will be hero based because it's whenever Dante picks up an item.

**[13:00](https://www.youtube.com/watch?v=EVZ5740YaGA&t=780s)** Specific unit event. So, when Dante acquires an item.

**[13:05](https://www.youtube.com/watch?v=EVZ5740YaGA&t=785s)** Okay, so that's the trigger.

**[13:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=787s)** However, we don't want it firing every time he acquires any item, we want it to just be the Mageroyal. So, you go to conditions and do item comparison. So, this is the item in question. Item type of item being manipulated equal to Mageroyal.

**[13:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=808s)** So, this trigger is saying if our swordmaster Dante acquires an item and if that item Well, okay. When he acquires an item and if it's may droil, then do the following. Otherwise, don't do anything.

**[13:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=820s)** So, all we have to do now is say what happens. And so, in this case, we want to make that quest requirement finished. So, let's see. Mark quest requirement as completed.

**[13:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=832s)** So, see this is where variables are super important because all it has and it's the ones that you can collect is last created quest requirement. So, that could be anything depending on if we create more quests later on, that might change. So, obviously, this is where the variables needed. So, we click the may droil quest requirement and we mark it as completed.

**[14:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=854s)** And then we can also make a quest message. So, quest update uh quest requirement. What's the difference? I can't remember. And now, this is the part where I'd have to look at a Warcraft 3 map and remember how it looks, but I can't remember. Anyway, we'll just kind of play it by ear. So, quest requirement complete.

**[14:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=880s)** Uh and then after that, we'll put collected a may droil.

**[14:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=887s)** All right, I think that's how it works.

**[14:49](https://www.youtube.com/watch?v=EVZ5740YaGA&t=889s)** Let's go ahead and move him just like out here because I don't want to have to keep him around to do all these quests and stuff. Okay.

**[14:58](https://www.youtube.com/watch?v=EVZ5740YaGA&t=898s)** So, let's see how that works.

**[15:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=901s)** All right. So, here we are.

**[15:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=904s)** And here are quests. The demon king, it's got those uh requirements. And here is artisanship. We got that one.

**[15:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=912s)** So, if we pick up this may droil, there we go. Collected a may droil. It is done. And if we look up here, okay, that didn't work.

**[15:24](https://www.youtube.com/watch?v=EVZ5740YaGA&t=924s)** So, we're missing something.

**[15:26](https://www.youtube.com/watch?v=EVZ5740YaGA&t=926s)** Did I forget to set that as the name of the Ah!

**[15:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=931s)** I I I saved it as Maguier. So, let's change that to major oil.

**[15:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=936s)** Okay, so that's done. Mark it as completed. That should be done.

**[15:39](https://www.youtube.com/watch?v=EVZ5740YaGA&t=939s)** We should also flash the quest dialogue button so that the player knows to check their quest that it was updated and stuff. There we go. So, now the quest is going to work. Uh the requirement's going to be saved and then here's something that I'm just going to kind of throw it together. So, check artisan ship completed.

**[16:03](https://www.youtube.com/watch?v=EVZ5740YaGA&t=963s)** So, suppose that on artisan ship we have collect a major oil, collect some mythril ore, collect, you know, like collect one of each of these crafting ingredients just to like help the player understand like what ingredients are useful for crafting. They might not do it in order and so what we want to do is make sure that if all the requirements are done, then it'll mark the quest as completed instead of just like all the

**[16:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=988s)** requirements done.

**[16:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=989s)** So, what what we're going to do is we're going to make a trigger without events, but it will have conditions. This is where I'm going to need to make more variables. So, I hope this doesn't get too complicated. I don't know how much you guys know about coo- coding, but Boolean is simply a true or false variable. Something's true, something's false.

**[16:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1007s)** And so, you can check whether something is done. So, in this case, major oil done.

**[16:53](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1013s)** So, we will set it to true when he finishes this requirement. Uh starts out false. As soon as this is done, he sets it. Um in this case, the the condition will be Boolean. So, if major oil is done, then mark quest as completed. I didn't save a variable for the actual quest.

**[17:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1040s)** Artisan quest.

**[17:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1042s)** In this case, the variable type is an actual quest, not a not a uh requirement, so we'll do that.

**[17:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1050s)** So, mark artisan quest as completed.

**[17:35](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1055s)** And quest message.

**[17:41](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1061s)** Quest completed.

**[17:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1071s)** That's more or less how they make it look on on the campaign maps.

**[17:56](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1076s)** So, in this case, we could have all of the conditions for artisan ship. We could have major royal done is true, mithril ore is true.

**[18:06](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1086s)** You we could have all the conditions there, and with each of these quest requirement things, we would just run that trigger.

**[18:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1093s)** Run checking conditions.

**[18:18](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1098s)** And then, also one thing that's important to do is uh a trigger will keep running every single time if you don't do this. Trigger turn off this trigger.

**[18:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1108s)** So, once this trigger runs once, then you want to turn it off because, you know, he only needs to get one uh major royal.

**[18:37](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1117s)** Um there's actually kind of a funny example of that in the campaign uh with David Fried, who made um the I think it's the second level of the undead campaign. It's digging up the dead.

**[18:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1130s)** If you go to the Pandaren relaxation area, it fires and says Pandaren relaxation area discovered or whatever.

**[18:59](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1139s)** Um and you can actually keep going back and forth, and it'll keep firing because David forgot to turn off the trigger, and by the time he realized it, it was too late to uh like they had to send it to QA and stuff to publish the map in the game. So, it's kind of a funny artifact of a of a bug that's still in there.

**[19:18](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1158s)** So, let's just try this out real quick.

**[19:21](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1161s)** Hopefully, my logic is sound. And if not, happens all the time. Big part of map design is just like figuring out what made it go wrong and whatnot.

**[19:33](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1173s)** So, here's a quest, Arzen ship, collect a major oil.

**[19:38](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1178s)** And then when you pick it up, look at that. It's completed.

**[19:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1187s)** So, yeah. So, obviously, that's a stupid quest if that's like one thing or whatever, but that's how it works.

**[19:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1195s)** One other thing, and I hope I don't go off on too many tangents, but one thing we will want to know is Okay, I'm not going to make this complicated, but I would make it so that whenever you pick up a major oil, if we are going to do it internally, we would have a variable called number of major oils or something, and it would increment up by one each time Dante picked one up.

**[20:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1220s)** And in this case, I'd probably want it to be this one because it's the same event and condition, and I could set You know what? Let's just do that. So, let's make a new variable called number of major oils, and this one is an integer.

**[20:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1236s)** It's just a number. Starts off at zero.

**[20:39](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1239s)** And we say number of major oils arithmetic number of major oils plus one.

**[20:49](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1249s)** So, in this case, every time he picks one up, this internal counter will go up by one. So, we don't So, he doesn't have to keep them in his inventory. It's just like a a thing on the game.

**[20:58](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1258s)** And if we have time towards the end, I might try to make like a leaderboard.

**[21:03](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1263s)** Like, you know how it has a little box up in the corner that says how many, you know, just kind of some information that they have on some of the quests. We could do that, but I haven't done that for a while, so I'm not really confident in that yet. Okay. So, for now, we want to set it to plus one major oil.

**[21:21](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1281s)** And then, we're going to do an if statement. So, an if statement is a trigger within a trigger, basically.

**[21:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1288s)** So, it gets to this point, and if major oil done is not done yet, then we do all of this.

**[21:42](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1302s)** Otherwise, just do nothing.

**[21:46](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1306s)** So, basically, what happens is when he picks up a major oil, it adds plus one to his number of major oils that he has.

**[21:53](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1313s)** Uh and then, if he hasn't done it before, if it's his first time picking one up, it'll do all that quest stuff, and then it'll be done.

**[22:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1321s)** All right. So, hopefully that makes sense to you guys, and let's I'll show you also as well what you can do on this.

**[22:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1330s)** This is really weird having nested folders. I mean, that's going to be um okay, we're going to make it some debug functions. On uh when I first found out about this, I used it to make my own cheat codes, which is kind of funny, but the more I've grown the the more I realize that cheat codes are basically for debugging, like making it easier to go in and check for bugs. So, in this case, let's check number of major oils.

**[22:35](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1355s)** So, here's how I make like debug or cheat codes or whatever. So, player one red number or orange. So, when our player types in major oil, I'll just do a dash to make it more of a debug thing.

**[22:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1371s)** Then, what what we're going to do is show a text message that says, "You have blank." And this one is going to be convert integer to string.

**[23:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1387s)** And the integer, of course, is number of major oils.

**[23:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1393s)** Major oils.

**[23:15](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1395s)** All right. So, basically, what this will do is when you type in major oil, it'll tell you how many you have.

**[23:21](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1401s)** And let's just put a bunch of major oils so that we can check to see if it doesn't fire every time.

**[23:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1408s)** Oh, is my voice that way? Ugh.

**[23:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1410s)** I'm sorry.

**[23:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1411s)** Uh I'll try something in a second to see if I can fix that. All right. So, if you pick up one major oil, you complete the quest.

**[23:41](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1421s)** And if we type in major oil here, you get one major oils.

**[23:49](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1429s)** And look, it's not doing the quest complete thing because as far as the trigger's concerned, that part is already done and the condition is complete.

**[24:00](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1440s)** So, if we type in major oil again, Okay. So, apparently, we messed up.

**[24:06](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1446s)** I messed up. Let's see what we did wrong, what I did wrong. We, we're in this. We're all in this together. Set number of major oils equal to number of major oils plus one.

**[24:18](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1458s)** Hmm.

**[24:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1460s)** Oh, I know what it was. I know what it was.

**[24:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1463s)** Turn off this trigger.

**[24:25](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1465s)** We do not want to turn off the trigger because this needs to fire every time.

**[24:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1468s)** Okay, fixed it. So, basically, now it's going to count the number of major oils.

**[24:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1472s)** And then I don't know, if you wanted some kind of like a crafting system where it's like you go to the um If you go to the I I can't remember what shops we made, but like say the tavern or something and they brew stuff, you could make it so that if you have a certain number of major oils, they'll brew it into a potion or something like that. So, that's where that variable comes in. And then that case, you could

**[24:58](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1498s)** say if they buy this potion and they have five major oils, then give them a potion and set the major oils variable to major oils minus five or whatever. Like how whatever the cost was.

**[25:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1513s)** And if not, then give them a message that says you do not have enough major oils or something like that.

**[25:19](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1519s)** So, that's the basics of quests. Um I hope that's enough for now.

**[25:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1523s)** Uh That's basically how this part is like it goes with requirements and stuff. You just have to have an event that marks it as completed and it just kind of goes through there.

**[25:33](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1533s)** I guess we should have checked to see if the gates worked as well.

**[25:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1536s)** Um but we can check that in a second. So, let's make um I'm just going to make Oh, interesting. Moonlight copy. Let's just change this to uh major oil patch.

**[25:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1555s)** Let's have one called periodic.

**[25:59](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1559s)** So, in this case, something happens over time. So, the gates open every night and let's make it so that Oh, we could actually just make it part of this trigger. Never mind. So, whenever he gets a major oil, it sets it to one and then we're going to make it so that it waits.

**[26:19](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1579s)** For now, let's just do like five seconds. Like normally we'd have them grow back every like minute or something like that.

**[26:25](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1585s)** But this here. So, let's go for the action of item create.

**[26:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1591s)** Let's create a new major oil at a random point in major oil patch. So, basically every time that he picks one up, a certain time later, it'll spawn another one so that they'll grow back over time. Obviously, you want it to be a long time so you don't just camp out and get unlimited, but that can be one that we can check as well.

**[26:56](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1616s)** Uh we could also do that with like enemies. Like if he kills an enemy, we could have it respawn a minute later or something like that if we wanted.

**[27:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1624s)** Honestly, you can do so much on triggers. Okay, so we So that's how that works. Um I'm just going to go through like Let's take a look at some of the events.

**[27:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1633s)** You can make things happen right when the map starts. You can make it so if a destructible dies, then something happens. Like if somebody breaks a gate, something could happen.

**[27:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1642s)** Uh the time of day, we've looked at that.

**[27:27](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1647s)** When you load a game, when you save a game. Oh, I don't know if you guys remember uh classic WoW, or I guess it's coming back now, but on classic World of Warcraft, work uh warlocks had items called soul stones.

**[27:41](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1661s)** And they, as well as other conjured items, like I think like conjured food and stuff that mages had, they disappeared when you logged out for a certain amount of time or something like that. So it's kind of funny on one of my maps, I uh made it so that when the the player loads a game, it deletes like it it removes all their conjured items from their inventory. So that was kind of fun.

**[28:06](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1686s)** Uh hero abilities button clicked. Some of these I don't really know why you would want to have them as an event.

**[28:11](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1691s)** Like the hero abilities button is that like plus sign that you click and pick an hero ability, but I don't know what that would what what you would want that to trigger, but I don't know, maybe in a tutorial video, but uh build structure button clicked. I don't know what tournament event is. I think Blizzard just used that. Uh chat message is for cheat codes or whatever we were talking about earlier. Cinematic skipped is one

**[28:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1714s)** I use a lot for debugging, especially when I'm making Easter egg videos. Um basically cinematic skipped is if the player presses the escape key.

**[28:43](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1723s)** And the reason they the way they have that set up is obviously if they have a cinematic going on in one of the campaigns and you press escape then it fades out and stops the cinematic so you don't have to watch the whole thing.

**[28:56](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1736s)** Selection event when you click on a on a unit, there's a keyboard event.

**[29:02](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1742s)** Unfortunately, all you can do is the arrow keys which I think is dumb. I think I hope Blizzard updates this to have more keys because then you could have like a game where you play, you know, like a not a top-down shooter or anything but you know, you could move stuff with the arrow keys and do abilities or something like that.

**[29:19](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1759s)** So, as for now, it's pretty much just the arrow keys and the escape button but mouse event when you Oh, when you click down. I think that's a that's a new one. Mouse move, interesting.

**[29:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1772s)** Player properties. Oh, this is an interesting one. If your current gold becomes a certain amount. Uh that could be useful for certain things.

**[29:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1780s)** Uh alliance change.

**[29:43](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1783s)** If they change the settings.

**[29:45](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1785s)** This is interesting. Why would you Oh, interesting. So, if you have victory.

**[29:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1791s)** This would only be useful in a campaign it seems because like if somebody wins then the game's over but maybe if you, you know, if they won then you set up what game that they have next or something. Maybe you could set some conditions and check like how many points they have and or if they destroyed this building then it's not there in the next map or something like that. I don't know. I haven't made

**[30:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1810s)** campaigns which I should sometime.

**[30:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1813s)** Defeat, that's a weird one cuz I don't think you can Maybe you can make a level after defeat. That could be interesting like you have a map where if you win you do one level and if you are defeated you go to a different level and then All right. If the player leaves game.

**[30:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1829s)** Oh, I just realized that this would be useful for multiplayer maps. That makes sense. Time elapsed. So, this this means after a certain time in the game something happens.

**[30:42](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1842s)** That's uh pretty useful. Periodic event is every certain number of seconds per game time something happens. Uh this can be useful for like checking conditions or something.

**[30:53](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1853s)** Uh we might use that in one of the boss fights.

**[30:57](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1857s)** Timer expires. Oh yeah, setting a timer.

**[30:59](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1859s)** You can have timers like when you're, you know, defending your base for however long.

**[31:03](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1863s)** These are the ones you're probably going to use most. Uh specific units, like we did with Dante. A player owned unit, so it doesn't matter which specific unit, but if a player owned by such and such player does whatever.

**[31:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1876s)** Uh or any generic unit. If a unit dies, if a unit changes owner, gains a level.

**[31:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1883s)** These are really useful because, you know, especially like look at this.

**[31:27](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1887s)** Begins casting ability. This means you can make an ability and then enhance it with triggers, you know, like if if there are no abilities that work for you, you could say if this character casts this spell, then have these things happen. You could have all kinds of stuff happen with triggers, which is pretty cool.

**[31:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1907s)** Begins reviving. Yeah, etc. etc. Uh enters region is really useful as well.

**[31:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1912s)** Uh especially for like cinematics, you're going to want, you know, if there's like a choke point that they enter, then you could start fading out the screen and have some kind of cutscene happen. Uh units life becomes less than a certain amount. Um oh, here's a fun one. Let's do this.

**[32:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1927s)** Initialization.

**[32:09](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1929s)** Uh I guess this is kind of Uh alarm.

**[32:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1934s)** This could be kind of fun. So, let's say if Dante's life becomes less than All right, so if it comes it becomes less than 50. Sure.

**[32:26](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1946s)** Then, let's change his color.

**[32:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1952s)** Change Dante's coloring to That's red, green, and blue. So, we'll make him like slightly red when he's wounded. So, when it becomes less than 50, he turns red. I mean, you could even have like a looping like on Zelda the the looping like alarm sound when you only have a few hearts left. Super annoying, but you could do it here if you wanted. You could have it just start playing over and over again until this

**[32:57](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1977s)** is until he gets healed back up.

**[33:00](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1980s)** Uh so, that can be called like alarm on.

**[33:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1984s)** And uh Oh, here's another thing that I'll look at in probably my next stream. I'm going to look at cinematics.

**[33:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=1993s)** But, you can do fade filters. So, I'm going to fade out with uh texture dream.

**[33:24](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2004s)** There's a different There's just a bunch of different textures. Like, there's the whole screen. There's horizontal wipes and stuff. The dream is just like the edges of the screen.

**[33:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2012s)** And we'll have it be red around the screen with mm 50% transparency. Let's see how that looks.

**[33:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2020s)** Uh so, you fade out with that dream thing.

**[33:44](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2024s)** And now, if his life becomes greater than or equal to 50, no, 51. No.

**[33:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2032s)** No, we already did less than 50, right?

**[33:54](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2034s)** So, Okay. So, 50 or above, it'll be fine.

**[33:58](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2038s)** Otherwise, he's like bleeding or whatever. You change the vertex coloring back to its normal coloring.

**[34:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2047s)** And you fade in, meaning you get rid of the filter.

**[34:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2052s)** And just for fun, let's throw in a sound cuz that's always useful to know as well.

**[34:18](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2058s)** Oh, here we go.

**[34:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2060s)** Actually, that's kind of high-pitched.

**[34:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2063s)** Yeah, I don't like that. Let's set it Let's pitch it down a little bit.

**[34:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2070s)** It's pretty cool. Not quite that deep, though.

**[34:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2074s)** It's kind of cool. It's like a creepy dissonance that will kind of scare the player, basically. Uh okay, so you play warning when he gets hurt or whatever.

**[34:48](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2088s)** If we I kind of wish I had imported some sounds beforehand, but it'd be kind of fun like make a like a heartbeat sound or something and I don't know, something like that. Anyway, so let's save that and there's plenty of enemies to practice on with that. So, let's test our quest. So, we have the one where this responds in a random place, the quest requirements. Oh, they have the gates. All right, let's try that.

**[35:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2113s)** So, let's see if this works.

**[35:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2116s)** Pick up the major oil.

**[35:19](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2119s)** Actually, oh look, there one grew.

**[35:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2123s)** Let's see if it runs a lot at the same time.

**[35:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2128s)** Oh look, that's so cool.

**[35:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2131s)** I've always wanted to do that in a map, but I don't think I have, so that's kind of fun. And if we check how many red major oils we have, we have eight major oils. Okay, that's working great.

**[35:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2140s)** Uh oh, dang. So, I guess we forgot to set them in the beginning as open, but let's change the time of day.

**[35:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2155s)** There we go. Okay, so here it goes.

**[35:57](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2157s)** Let's see if it works.

**[36:00](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2160s)** Hey, look at that. So, we can go inside during the daytime.

**[36:06](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2166s)** I I got I got some new abilities for Dante, by the way, so So, this one's called flash. It's basically blink.

**[36:15](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2175s)** But, it's like a little light effect instead.

**[36:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2182s)** All right, here we go.

**[36:27](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2187s)** Let's go get Dante killed right now, but for now Okay, so basically he has a half hour to get inside, which is not very much time now that I look at it, but There we go.

**[36:46](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2206s)** Perfect. Obviously, he can still break break them down, but we can make them invincible, too.

**[36:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2212s)** We can do anything. That's what triggers are. All right, let's go find some trouble in the desert, I guess.

**[37:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2224s)** So, yeah, while we're traveling there, the abilities I chose are searing blade, flame shield.

**[37:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2230s)** Uh searing blade we looked at last time.

**[37:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2232s)** It's basically acid bomb modified.

**[37:15](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2235s)** And then flame shield is basically immolation, but I changed the graphics, so I'll just kind of show you how it works.

**[37:25](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2245s)** Oh, we should have made a death uh trigger for him. Like, what happens when Dante dies? I just realized that.

**[37:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2256s)** Oops, okay.

**[37:39](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2259s)** So, here's flame shield.

**[37:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2267s)** Turn it off. All right.

**[37:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2271s)** Man, I made this quite a maze over here.

**[37:53](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2273s)** Spirals over the oasis here.

**[37:58](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2278s)** All right, cool.

**[38:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2281s)** Kill me. Get me down to 50 health.

**[38:05](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2285s)** Oh, yeah, I forgot that we gave him a fireball attack for Arenas. This guy's really cool. I would love to make a map with this. Okay, here we go.

**[38:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2292s)** Let's see if it works once he gets down to 50.

**[38:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2296s)** Oh, yeah, look at that. Everything got red. Oh, I hate how he doesn't have a death animation. Oh, well.

**[38:21](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2301s)** Anyway, uh as you can see it's stuck on that because there's no way for it to turn off, but it works.

**[38:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2309s)** And I actually like the look of that.

**[38:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2310s)** The transparency is good. Yeah.

**[38:33](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2313s)** However, 50 help is not very good because you die pretty fast when you get to that point.

**[38:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2320s)** So, I think we should change it to Let's try the percentage thing. So, what you can do is arithmetic.

**[38:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2330s)** Unit property life of Dante.

**[38:56](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2336s)** That's not working either. Max life, here we go. Okay, max life of that divided by Mhm, three?

**[39:06](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2346s)** Four? Should we do like 25% life?

**[39:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2350s)** I don't know, let's do three for now.

**[39:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2352s)** So, when he whenever he gets to a third of his health, this will turn on.

**[39:18](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2358s)** And it will turn off when it becomes greater than or equal to uh his life.

**[39:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2369s)** So, that'll work pretty well.

**[39:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2371s)** Okay, so we have that and we're going to pick some more fights again.

**[39:35](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2375s)** Uh This is all like badly organized, but I probably should have made it like Like let's make one just for Dante. And let's make Let's make a death trigger for him. What will happen when he dies?

**[39:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2391s)** Uh well, the short answer is anything you want.

**[39:57](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2397s)** This will be a specific unit event.

**[40:00](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2400s)** Okay, so if he dies then we're going to fade out to black over mhm 5 seconds, we'll say.

**[40:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2414s)** Actually, let's say decays so that it waits for him to say our hero has fallen.

**[40:19](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2419s)** Uh so we fade out over 5 seconds.

**[40:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2422s)** Oh, and also I wonder if we should lock the camera on him.

**[40:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2428s)** I'm going off on tangents, but this won't take very long.

**[40:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2432s)** I have an idea. So if a player presses escape then lock the camera to Donte.

**[40:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2447s)** And then uh Okay, so there's two ways we could do this. We could do Boolean Let's do Boolean.

**[40:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2455s)** We could also just do two uh triggers, but this is probably cleaner.

**[41:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2461s)** So we make a Boolean called camera lock.

**[41:06](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2466s)** And you set it to true when it's um locked to him.

**[41:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2476s)** So we say if if it's not locked then lock it to him.

**[41:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2482s)** And set it to true.

**[41:26](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2486s)** If it is false then lock it. If it's not false Wait, so if it's true, yeah, then turn it off.

**[41:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2494s)** Is there unlock camera? I think it might just be reset game game camera.

**[41:39](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2499s)** Yeah, so we'll just say reset. And then we set it to false.

**[41:43](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2503s)** Okay, cool. So we fade out over 5 seconds.

**[41:48](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2508s)** And then we wait 5 seconds.

**[41:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2511s)** Okay, here's another thing that we'll talk about more with cinematics, but there's a palette called the camera palette.

**[41:59](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2519s)** And we'll go to this point.

**[42:03](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2523s)** So once it's once the screen's completely black we will apply that camera.

**[42:11](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2531s)** And we'll fade in over just a couple seconds.

**[42:17](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2537s)** Wait 1 second.

**[42:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2540s)** Sorry if I'm going fast like this is just all kind of intuitive. You can obviously like tinker it with it as much as you want. Uh but then let's have revive Dante at center of Let's have him respawn at the church.

**[42:38](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2558s)** Uh It'd be kind of fun.

**[42:42](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2562s)** It's there so we might as well have a use for it. So he will revive at res.

**[42:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2570s)** So it'll move to this position with the camera.

**[42:53](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2573s)** And then it'll revive him right there for us.

**[42:57](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2577s)** Yeah, so that's how it'll work when he dies.

**[43:00](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2580s)** You could even make like penalties when he dies. You could make it so that he uh loses all his Oh, I know. This is Sorry, I'm getting way excited about this cuz I love triggers and I haven't done this for a while. I don't know why. I I don't know.

**[43:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2593s)** I get caught up with other things but this is just super fun. So I don't know if you guys have played Hollow Knight. I just started playing it recently and it's one of the best games I've ever played.

**[43:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2603s)** When you die on Hollow Knight you leave behind like a ghost.

**[43:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2608s)** And if you go back and defeat the ghost then you get all your money back. So what we could do is we could make it so when you die it waits like a second or so and then it creates an item that's like a gravestone.

**[43:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2627s)** I'll just copy major because this is just another like a useless uh item that's only used for triggers.

**[43:54](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2634s)** Uh Here lies Dante the swordsman.

**[44:05](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2645s)** As as for the icon it doesn't really matter but I don't like it's useful in the editor, so Yeah, let's use the graveyard one.

**[44:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2654s)** About the model, there's actually a really cool model that's used in the uh for the graveyard.

**[44:21](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2661s)** Create corpse. See, look at that. It's the perfect grave.

**[44:24](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2664s)** So, that will be the item. Let's check to see how it looks first.

**[44:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2669s)** Look at that. That's perfect.

**[44:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2672s)** Okay, so you wait 1 second.

**[44:38](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2678s)** It creates a tombstone at the position of Dante when he died.

**[44:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2690s)** Okay, so it creates the tombstone.

**[44:53](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2693s)** And let's make it gold tombstone.

**[44:59](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2699s)** This variable is an integer.

**[45:06](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2706s)** So, set the gold tombstone to be player's current gold. So, we store Dante's gold in that variable.

**[45:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2720s)** And we set his gold to zero.

**[45:26](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2726s)** Then when he picks up his tombstone, you can copy this for me. And your oil if he acquires an item and the item is a gravestone, then we set the player's gold to the amount in the gold tombstone.

**[45:49](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2749s)** There we go.

**[45:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2750s)** And we can like make a cool like special effect if we wanted. That's another thing that's really useful, special effects. So, you can attach to different parts.

**[46:02](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2762s)** Um when you do a special effect, uh overhead is obviously like useful for quests like that little uh exclamation point. You could do chest, head, left arm, right arm, left leg, right leg, um origin, which is just like right at its feet.

**[46:21](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2781s)** I think that's all those are all the places, but let's do Yeah, let's do overhead and let's just make it like be a Let's do that. That's pretty sweet. I mean, you know, it's like he's back to life completely sort of thing.

**[46:35](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2795s)** Um and we could do other things, too.

**[46:37](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2797s)** Like we could make him like slightly transparent until he goes back to his tombstone or something like that. That could be kind of funny. And uh And I'm already thinking of like bugs that might happen, but if you die on your way to get the tombstone, we'd have to make a way to delete the other one.

**[46:56](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2816s)** Okay, here's how we could do it.

**[46:58](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2818s)** Pick every item in Oh, wait a minute.

**[47:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2824s)** Seriously, you can't do item type? Okay, well, I can work around that. So, you pick every item in the playable map area and do the item type of the item that we picked. It's going to look at all the items on the map and if if one of them is the gravestone, then do remove picked item. There we go. So, we put that before, obviously, otherwise it would just create it and then delete it.

**[47:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2852s)** So, when he dies, it looks for any other tombstones that are created, deletes them, you lose that gold, and then it creates a new tombstone.

**[47:42](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2862s)** Perfect.

**[47:43](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2863s)** So, that's pretty fun. We can try that now. What about a way where the longer you take to get to the tombstone, the less gold you receive? Ooh. Shouldn't we be even meaner than we are now?

**[47:54](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2874s)** Let's do it. Why not?

**[47:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2875s)** This is where we're like just kind of experimenting, so let's do that. So, what you do for that, as far as I know, and there there are other ways to do this and stuff, but as far This is how I would do it. But, let's call one called gold countdown.

**[48:08](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2888s)** We can make it to You can make it trigger not initially on. So, let's start it out at the beginning because you start out and there's no tombstone, obviously. So, what we're going to do is the event is periodic event every What do you guys think? Every How fast should the gold deplete? Like one every like one at one a second? I don't want to do that, right? Let's just do like one every 10 seconds. It doesn't

**[48:35](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2915s)** have to be that bad.

**[48:37](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2917s)** But, we want to do set gold stone gold tombstone to gold tombstone minus one. And I'm I don't want like negative gold, obviously, cuz that could break the game. So, instead, let's do if gold tombstone is greater than zero, then do this. Otherwise, don't do anything.

**[49:05](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2945s)** So, that means if it's zero, then you Or Oh, here we go. If it's greater than zero, set the gold tombstone to that.

**[49:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2953s)** Otherwise, if it if it isn't greater than zero, if you run out of gold, then we want to turn this trigger off and stop, you know, decreasing the gold.

**[49:24](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2964s)** And we want to kill that item because you're done. Like you're you know, you ran out of time.

**[49:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2974s)** So, now, what we're going to do is death, it creates a tombstone, and it turns on gold countdown. So, that's how I would do that.

**[49:44](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2984s)** Uh I would like to make like a leaderboard, but I'm I don't know, they kind of scare me.

**[49:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=2992s)** They're kind of difficult, and I can't remember if I know how to do it, but it'd be nice to show up at the top like how, you know, how much gold you have left. Mhm, for the time, why don't we just make a text message up up here and it'll only be for like half a second.

**[50:06](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3006s)** Like it's not going to be that long. And it's going to say the number of gold tombstone.

**[50:11](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3011s)** Let's just do that. Like that wouldn't stay in the game, but it might be kind of cool to do that as well.

**[50:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3016s)** A countdown timer.

**[50:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3020s)** A countdown timer would be good except we're not really counting down time.

**[50:24](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3024s)** Well, on the other hand, like you could make it so that it, you know, it like divided up the time left, you know, like it's like the number of gold times 10 or something so you know how much was left or something. So that could be kind of good as well, but and then you could have the timer say, you know, time until gold is depleted or something like that.

**[50:43](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3043s)** That's a good point as well.

**[50:45](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3045s)** Okay, this is fun. Let's let's get We've We've made quite a few things for this part. Let's see how well they work. You know what? I'm going to make it easier for him to die so he can test this.

**[50:59](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3059s)** Okay, I put this campfire out here cuz I want to like bandits or something.

**[51:03](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3063s)** Cuz why not?

**[51:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3064s)** So let's just make like this unstoppable force of bandits like that you do not want to mess with.

**[51:11](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3071s)** See, is this the ranged one here?

**[51:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3074s)** So these guys you do not want to mess with this bandit gang, but we're going to cuz it'll be fun.

**[51:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3080s)** All right, let's see how this works out.

**[51:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3082s)** I hope it works well.

**[51:25](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3085s)** Okay, I will. Go ahead and go pick a fight.

**[51:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3089s)** We'll give him a fighting chance with his flying shield.

**[51:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3096s)** Oops, okay.

**[51:37](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3097s)** This is going to be hilarious. All right.

**[51:46](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3106s)** Okay, it's almost third. Oh, look at that.

**[51:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3112s)** Oh, okay, there's no visibility. So, that's I think that it didn't trigger because he didn't decay because he's a hero.

**[52:00](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3120s)** So, that's one thing we got to fix. Good to know.

**[52:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3124s)** Death.

**[52:06](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3126s)** If he becomes revivable.

**[52:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3130s)** There we go. Let's do that instead.

**[52:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3132s)** Learn a lot with this.

**[52:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3134s)** All right.

**[52:24](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3144s)** For my people.

**[52:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3151s)** Look at that.

**[52:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3152s)** Wounded.

**[52:33](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3153s)** About to die.

**[52:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3160s)** Okay, there he goes. See, we're fading out.

**[52:43](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3163s)** Did not look as good as I thought.

**[52:46](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3166s)** Oh, and we have no gold.

**[52:48](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3168s)** Okay.

**[52:49](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3169s)** The camera didn't work either, which was a little weird. All right, we'll have to look at that, but Anyway, he survived. Let's go check and see if we can find that tombstone.

**[52:57](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3177s)** Unfortunately, I forgot to add gold in the beginning. Here, let's let's add some gold. Oops.

**[53:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3184s)** Okay, we got 500 gold.

**[53:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3187s)** Oh gosh, okay, we needed to open those gates at the beginning. Oh, no, it's invulnerable.

**[53:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3196s)** There we go.

**[53:17](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3197s)** Oh, you know what? I think it deleted the tombstone cuz I had no gold. So, it was like nothing to lose. Okay, let's try this again.

**[53:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3209s)** Okay.

**[53:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3212s)** Okay, I still don't I didn't see a tombstone there. I think we might be having problems there, but Why is it not moving the camera? That's so weird. Okay. Okay, I have 500 gold left.

**[53:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3227s)** So, it's that worked. It's there's no more gold left, but Okay, it's going down. We're losing our bounty here.

**[53:54](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3234s)** Our wallet's burning up.

**[53:56](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3236s)** It shall be done.

**[53:58](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3238s)** As you wish.

**[54:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3241s)** I didn't see a tombstone though. Did it not create one? Maybe it Maybe it like went dark in the fog of war too fast.

**[54:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3247s)** For honor.

**[54:08](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3248s)** Except they think we died in the game other camps, so this ought to be interesting.

**[54:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3252s)** There it is. Okay, so let's see how much we get.

**[54:18](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3258s)** Uh Okay, we got it.

**[54:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3262s)** I love that. That is That is a cool mechanic. And yeah, it didn't it didn't happen the first time because uh For honor.

**[54:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3269s)** it just simply didn't have anything to work Okay, there we go. See, there's a bug where it's still counting because it never we never gave it a second um method by which it could be turned off.

**[54:43](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3283s)** So, that periodic one is still just going to keep running forever or at least until that variable goes down.

**[54:48](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3288s)** See, as far as it knows, we we don't have any gold. It's still just doing his job. So, okay, if he picks up the tombstone, then turn that sugar off.

**[55:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3301s)** There we go.

**[55:02](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3302s)** Cool. That's really fun. And then yeah, we don't need necessarily need that to show that text, but now we know it's working.

**[55:09](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3309s)** That is really fun. So, let me see here.

**[55:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3312s)** You could even say You could even have a variable like a boolean variable that said if it's Dante's first time dying, then tell him what to do. Like, you know, go go get your tombstone to you know, get your gold back or something like that. And then it turns off. Like, if that boolean's off, then you know, it doesn't say that. So, that could be That could work as well. I really like that though.

**[55:35](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3335s)** Um You know what we could even do? Let's make a mini map ping where the tombstone is, too, so that they know where it is, like they remember where he where they died.

**[55:46](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3346s)** So, ping mini map for player orange at a position of Oh, you know what?

**[55:57](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3357s)** I just found a better way that's cleaner and it doesn't make it look through the entire map. So, instead of picking every item, let's just create a tombstone and set it as a variable for an item.

**[56:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3374s)** So, you set tombstone as the last created item.

**[56:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3380s)** And instead of looking through everything, you just say, "Remove that variable tombstone if it exists." or whatever.

**[56:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3388s)** And then create it and set that as a new tombstone. Okay, that makes it easier because now all we have to do is once the countdown is over or whatever, just remove tombstone. Ping mini map for the orange player at position of tombstone for whatever. Yeah, 1 second, that's fine.

**[56:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3410s)** That's for 2 seconds.

**[56:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3411s)** Cool. So, it'll just keep doing that.

**[56:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3415s)** Oh, you can ping it with a color. Let's do that instead.

**[56:59](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3419s)** For 2 seconds and the color will be like red, I guess, cuz it's you know.

**[57:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3427s)** Cool. Let's see if that works, but yeah. Sure, why not? Oh, the gates. Almost forgot about the gates. Yeah, let's just at the very beginning of the game, just run gates open without even checking the conditions. It doesn't matter.

**[57:25](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3445s)** In fact, I don't think there even are conditions. Yeah, so just run at the beginning, open them all up.

**[57:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3450s)** All right.

**[57:33](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3453s)** All right, take two.

**[57:44](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3464s)** Oh, it does sound better. That's good.

**[57:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3467s)** That's really annoying. I don't like that uh and fading and thingy.

**[57:53](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3473s)** Um oh, I forgot to check for locking camera thing.

**[57:58](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3478s)** Nope, doesn't work.

**[58:00](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3480s)** I wonder if I accidentally set it to like player one red cuz that's Oh, cool.

**[58:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3484s)** Look, it's pinging on the mini map.

**[58:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3490s)** I like that. I haven't really messed with stuff like that before, but yeah, that's really cool. It's pinging on the mini map right here in the middle.

**[58:18](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3498s)** It might even be good to have like it reveal the visibility around the grave just so you can see where exactly where it is. That could That could be something that would be good to do as well, but we'll see.

**[58:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3511s)** Okay, there it is and Cool, cool. That worked.

**[58:39](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3519s)** Let's Let's have something fun. Let's do a boss battle. So, did I make a bridge here? I did. Okay, so there's a bridge there.

**[58:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3532s)** There we go. Okay, so that's the bridge.

**[58:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3535s)** All right, let's make a new folder called bosses.

**[59:00](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3540s)** All right, this might be sloppy the way I'm doing this, but it's just kind of to save time.

**[59:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3544s)** Um basically, if all of these units desert territory a unit dies owned by the enemy and if number of units in the desert territory owned by player two, if it's equal to zero, in other words, if there are no more enemy in it's in the desert, then we set the life of this bridge to 100%.

**[59:38](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3578s)** So, when the last unit dies in the desert, then that bridge will will get uh resurrected.

**[59:45](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3585s)** So, start it out at zero.

**[59:49](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3589s)** All right, there we go.

**[59:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3592s)** And I might make this like cinematic at some point, but next time, but for now, let's just do this here.

**[1:00:00](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3600s)** A sandstorm initialize.

**[1:00:05](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3605s)** Okay, unit enters sandstorm.

**[1:00:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3610s)** And if that unit, the entering unit, is Dante, then what we're going to do is we're going to pull a Zelda and block off the path.

**[1:00:25](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3625s)** So, then the battle starts.

**[1:00:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3628s)** Um what I was thinking for this boss fight though, we'll see how well this works, is he's going to keep fighting you.

**[1:00:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3636s)** And this kind this like wind and stuff kind of gave me the idea that he has like a cyclone ability. So, when he gets like two when you knock him down 25% of his health, he's going to cast cyclone on you, go over here and burrow into the sand, use his ability, and then a bunch of little ones are going to burrow out of the sand and attack you. And once those are dead, he's going to pop out again, and then you can It's like four

**[1:00:58](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3658s)** stages of of attacking and stuff.

**[1:01:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3661s)** So, once you go in there, the bridge falls, starts playing some music.

**[1:01:08](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3668s)** I got some cool boss music for this.

**[1:01:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3676s)** So, I think that'll be cool. And plays that music, and I'm going to walk through a cinematic next time because I think it'd be really cool if like as soon as you stepped over the bridge, it faded out, did like a camera thing where it showed the scorpion burrowing out of the sand and roaring or something, and then it started the music. But for now, we'll just keep it simple.

**[1:01:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3696s)** Obviously, you can polish everything later.

**[1:01:38](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3698s)** So, we'll just make the battle music start and command the unit. There's a lot more unit triggers than I remember. All right, issue order.

**[1:01:49](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3709s)** Targeting a unit. Order sand sting to attack Order sand sting to attack swordmaster.

**[1:01:57](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3717s)** Okay, so you stopped the you you crossed the bridge, the music starts, it starts attacking you.

**[1:02:03](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3723s)** Now, we want a subset of triggers for the sand sting battle.

**[1:02:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3730s)** Uh these are probably going to normally be off, I think. Um but we'll see. So, sand sting's life becomes less than I guess we could just do that same thing we did with uh We could just do that same thing with Dante to save time. I kind of like that percentage based thing. So, sand sting's life becomes less than You know what? Let's just keep it simple.

**[1:02:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3754s)** How much life does he have? 750. Okay.

**[1:02:37](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3757s)** Uh let's change his life to 1,000.

**[1:02:42](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3762s)** Just cuz that's easier to work with. And let's make it so he doesn't regenerate health.

**[1:02:48](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3768s)** Okay, so if it's less than 750 then we're going to do this whole burrow thing. Let's just make that its own trigger, actually. So, this one's not going to have any uh It's not going to have any events, cuz it's just going to be externally fired by that several times.

**[1:03:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3787s)** So, basically he's going to I think there's a new trigger that I might want to use.

**[1:03:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3793s)** Add ability. Okay, add uh we'll do the neutral hostile one just because the other one might require an upgrade.

**[1:03:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3802s)** So, give Sand Stinger cyclone ability. I don't want him to use it on his own. I want him to just use it on here to like stun you when he goes and burrows. I'm just going to make it so it doesn't cost any mana either because I don't want there for to be any issues there.

**[1:03:39](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3819s)** Mana cost zero. Okay, so give him the cyclone ability.

**[1:03:46](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3826s)** Then, issue order order Sand Stinger to to cast cyclone on Dante.

**[1:03:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3835s)** He's going to cast cyclone on Dante and we'll just wait like a second or so for that to take effect.

**[1:04:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3844s)** And then he's going to go we'll say like his main place right there.

**[1:04:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3852s)** Okay, so now we're going to order him after that second to move to that point.

**[1:04:18](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3858s)** Uh, and then we can wait for him to get there, I guess.

**[1:04:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3863s)** Okay, so we're basically going to wait until that region contains one Sand Stinger. It's kind of hacky, but I think it'll work. Uh, checking every half a second or so. So yeah, see look at all these layers of triggers. This can happen sometimes with triggers, but I don't know. I think with practice you can like anybody can get this sort of thing to work because it's just a matter of I don't know. Just kind of looking at it

**[1:04:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3890s)** from a logic perspective, I guess. Okay, I've even forgotten what I'm doing here.

**[1:04:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3895s)** Okay, let's go back. So, we need to check that he's in that region. So, number of units in units in region matching condition.

**[1:05:05](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3905s)** Okay, units in burrow, unit type of matching unit. That's what I meant to do. Matching unit equal to Sand Stinger. Where are you? There he is.

**[1:05:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3916s)** Okay, I think we did it. So, wait until burrow has basically one Sand Stinger in it, which is the only Sand Stinger, obviously.

**[1:05:27](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3927s)** Okay, so we'll try that.

**[1:05:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3929s)** So, wait until he's there, and then it waits like another half a second or so, just to make sure that he doesn't do anything weird, and then Okay, I'm going to try this. I think it might be easier for him to do a burrow ability, but I'm just going to try doing the animation.

**[1:05:45](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3945s)** So, stand alternate. Let's find his animation.

**[1:05:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3950s)** Morph. Okay.

**[1:05:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3952s)** We're going to wait for that point.

**[1:05:56](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3956s)** And then we're going to play Sand Stinger's morph animation.

**[1:06:03](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3963s)** And that morph animation is 1 second long, conveniently.

**[1:06:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3967s)** So, then we wait 1 second.

**[1:06:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3970s)** And then we hide him.

**[1:06:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3972s)** And then, we wait another half a second or so.

**[1:06:17](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3977s)** All this like waiting certain percentages of seconds is just kind of second nature to me at this point. Um then, what we're going to do is we're going to create a bunch of little baby Sand Stingers.

**[1:06:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3991s)** I'm just going to make a custom unit because Let's see here.

**[1:06:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=3996s)** Yeah, I'm going to do this burrowing one and then just make him really weak, cuz we don't want him to be We don't want to just make it an interesting fight. We want him to have like actual crazy ones. These are they're going to be called Sand Stingers young.

**[1:06:58](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4018s)** These are going to be basically normal Raknishids, except which have like 25 hit points. They're easy to kill.

**[1:07:09](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4029s)** Uh but otherwise I think they attack normally and then their scale going to make them really tiny.

**[1:07:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4036s)** Just really baby.

**[1:07:19](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4039s)** All right, so back to the triggers. So, he he burrows and then it let's create like six babies.

**[1:07:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4050s)** Burrowed babies.

**[1:07:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4052s)** There we go. Sand sting's young.

**[1:07:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4054s)** For player two at center of burrow.

**[1:07:41](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4061s)** So, it creates them.

**[1:07:43](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4063s)** And then we going to pick all those units and this will be an issue with no target. So, we're going to say pick every unit in units of type and we're going to say Yeah, why does it See look, it goes ability, item, unit, ability, item, unit. Like I wish they just grouped them all together, but with no target and then we'll order the picked unit to undead crypt thing burrow. Unburrow.

**[1:08:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4092s)** Okay.

**[1:08:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4093s)** So, we just tell them to unburrow.

**[1:08:15](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4095s)** I should have done multiple actions, but I didn't, so I'm just going to be lazy and just do this.

**[1:08:21](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4101s)** And then you order them order all the picked units to attack Dante.

**[1:08:27](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4107s)** Cool. Then Okay, so so far he cast cyclone and we're going to make this last We're going to make cyclone last the perfect amount of time, so this will work well as well, but we'll just see how that works for now.

**[1:08:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4120s)** Okay. Um and and he's going to be invulnerable when he's a cyclone, so I really should wait a second.

**[1:08:49](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4129s)** Make them unburrow and then make them attack.

**[1:08:53](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4133s)** So, this is So, he he did he cast cyclone. Oh, we can also remove that ability now that he has it.

**[1:09:03](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4143s)** Uh so that's 1 second, 2 3 4 seconds. So let's change that ability to only last for 4 seconds on a hero. Cool. Okay, so hopefully that works. This is going to be This is like the most complicated trigger we've made.

**[1:09:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4160s)** So obviously we're going to need to test it and stuff, but pick every unit in that and tell them to attack. Then here's what we're going to do.

**[1:09:27](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4167s)** Unbury.

**[1:09:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4169s)** This is going to be off and we're going to say turn on unbury.

**[1:09:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4176s)** So as soon as those little babies come out, we're going to turn on this trigger which says when a unit on by player two dies and we'll say number of units of type. So if any of those units dies and he doesn't have any babies left, then let's unhide him and play his morph alternate animation, which is him unburrowing. I have serious problems with animation sometimes, so I hope that that doesn't hurt that, but

**[1:10:11](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4211s)** I'm going to unhide him. He's going to do his alternate animation and just to be safe I'm going to reset his animation after that. Sometimes they like freeze in place after I do animation, so it's kind of annoying.

**[1:10:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4223s)** Okay, so when that happens, he appears and then we'll turn off this trigger.

**[1:10:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4231s)** Yeah, we'll just again make him like attack Dante again.

**[1:10:38](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4238s)** All right, so now we just need to make it trigger that many times. So if it becomes less than 750, then run uh bury and then turn off this trigger.

**[1:10:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4252s)** So once it becomes less than 3/4 of health, this runs and that happens. Next is when he goes to half health.

**[1:11:05](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4265s)** Then run Ember Bearing.

**[1:11:08](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4268s)** And then three is when he goes to 3/4 health, run it again.

**[1:11:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4274s)** And then that should work pretty well.

**[1:11:18](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4278s)** Okay, on paper it looks good. Not sure if it's going to work.

**[1:11:25](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4285s)** All right, his abilities. I just copied the most powerful one. Command aura, that's it, huh? Hmm. Yeah, let's definitely give him Envenomed Weapons.

**[1:11:33](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4293s)** Cool. Yeah, so he can have Envenomed Weapons.

**[1:11:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4296s)** I also wanted to have just like an active ability.

**[1:11:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4300s)** Let's give him like Shadow Strike. Like the Warden's ability, Shadow Strike. Is there anybody with Cool, there's this neutral hostile.

**[1:11:48](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4308s)** Okay, because he's got a cool like oops spell ability. Like he just shoots something out of his stinger. That's really cool.

**[1:11:56](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4316s)** So that'd be really cool. Um Oh, I'll check to see if that has mana cost.

**[1:12:02](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4322s)** Cool down is 8 seconds, that's decent.

**[1:12:05](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4325s)** But the mana cost should be nothing cuz I don't think he has mana.

**[1:12:11](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4331s)** Perfect.

**[1:12:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4333s)** Um Okay, so that works. I think that looks good so far.

**[1:12:19](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4339s)** I'm not sure if it's going to work perfectly, but we'll try it out.

**[1:12:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4343s)** In the meantime we need to make sure that Dante uh can survive this fight during the debug.

**[1:12:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4352s)** So I'm going to make a custom item called Periapt of Endless Life.

**[1:12:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4360s)** So it's going to give him an extra 5,000 life.

**[1:12:48](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4368s)** We want him to be able to survive this battle no matter how hard it is.

**[1:12:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4372s)** We're going to balance it later, obviously.

**[1:12:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4375s)** But, for now let's just worry about that. So, we'll just give it to him in his inventory.

**[1:13:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4384s)** Cool.

**[1:13:05](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4385s)** So, now he should be able to take him on.

**[1:13:09](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4389s)** Let's see. Anything else we need to prepare? One thing that I would also do at some point is like, once you start fighting him, set a boolean to let to let the computer know that you're fighting him. And then if you die and that boolean is on, then it'll return Sans to full health. It will raise the gate, that sort of thing. And then um you know, just make sure that like you have to fight him all over again, stuff.

**[1:13:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4416s)** But for now, and then maybe it would even move the grave outside of his arena.

**[1:13:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4420s)** So, that that would be fun to do. But for now, let's just kind of keep it simple. Okay. Now, let's make some debug functions real quick.

**[1:13:49](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4429s)** Uh desert clear.

**[1:13:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4431s)** Desert clear.

**[1:13:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4435s)** So, once we type that it will pick all the units in the desert territory and kill them.

**[1:14:05](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4445s)** So, that'll open up the gate.

**[1:14:08](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4448s)** And then we'll see what happens there.

**[1:14:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4450s)** Oh, I guess I do want to do one thing with his death though.

**[1:14:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4454s)** I want to make it so when he dies it turns the music off.

**[1:14:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4462s)** That way we can just set the music back to the way it was and set it to that boss music or I'll just do that. Why not?

**[1:14:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4470s)** Okay.

**[1:14:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4472s)** So, let's give that a try.

**[1:14:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4474s)** Um let's also just give Dante's scroll of speed so that he can move a little faster.

**[1:14:41](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4481s)** And let's just move him a lot closer to the desert.

**[1:14:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4490s)** Okay, cool.

**[1:14:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4491s)** Ideally also with the quests, we would have a quest for each territory like to unlock the boss and then like once you defeated so you'd know when you defeated all the creatures and it would say quest complete the you know the way to the boss is clear that sort of thing, but can work on that later. Okay.

**[1:15:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4510s)** All right.

**[1:15:11](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4511s)** I hope this works at least somewhat like we hope.

**[1:15:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4516s)** Let me face the battle.

**[1:15:20](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4520s)** For my people.

**[1:15:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4522s)** All right.

**[1:15:24](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4524s)** Let's just make sure it kills them all.

**[1:15:26](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4526s)** It shall be done.

**[1:15:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4529s)** I am not prepared.

**[1:15:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4531s)** Okay, let's kill them all and I just want to make sure that the bridge spawns.

**[1:15:41](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4541s)** Okay, that did not work. Made these two complicated without checking them first.

**[1:15:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4547s)** Oh my gosh, I killed Sand Sting. Okay.

**[1:15:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4551s)** Kill picked unit. That's what I did wrong. That's hilarious. Okay.

**[1:15:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4555s)** So pick every unit and kill everyone in the territory, not our lord Sand Sting the boss here.

**[1:16:02](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4562s)** One sec. I just want to make like a victory sound when you kill Sand Sting.

**[1:16:08](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4568s)** Um uh what's it called?

**[1:16:11](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4571s)** It's like Uther arrives or something.

**[1:16:15](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4575s)** There it is. Uther returns.

**[1:16:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4582s)** And then we'll make Sand Sting drop some cool like poison stinger or something that you can use as a like a shadow strike item that also envenoms your weapon or something. That would be awesome.

**[1:16:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4594s)** Okay, now let's try it.

**[1:16:37](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4597s)** Let me face it.

**[1:16:37](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4597s)** All right, let's try this again.

**[1:16:43](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4603s)** Oh my gosh.

**[1:16:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4607s)** I put myself there, so it killed me cuz I was on the territory. That's hilarious. You know what? That's fine because I wanted to see how that worked anyway.

**[1:16:58](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4618s)** All right, and Oh, crap.

**[1:17:03](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4623s)** Something's going on there.

**[1:17:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4627s)** Why on earth did that happen? Okay, I think it all happened.

**[1:17:12](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4632s)** So, this is where things get kind of tricky, but it's all part of the game.

**[1:17:17](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4637s)** It's kind of fun. So, why is this happening? Why is it creating those units and stuff and making them change?

**[1:17:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4648s)** Cuz it's off.

**[1:17:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4650s)** Ah.

**[1:17:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4652s)** I'm an idiot.

**[1:17:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4654s)** I changed it to sword master instead of sand storm.

**[1:17:39](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4659s)** So, I died and then all these things triggered.

**[1:17:42](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4662s)** Computers are so funny. They're like, "That's what you told me to do. You said when his life went less than whatever." That's what I did, so fair enough. It's true. He did.

**[1:17:53](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4673s)** All right, and then let's make his blink ability, I mean, flash have just unlimited range.

**[1:18:03](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4683s)** There.

**[1:18:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4684s)** How big is that desert clear thing?

**[1:18:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4690s)** Okay, but I'm going to make this a lot cleaner later, but for now, let's just not worry about that.

**[1:18:15](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4695s)** And then he can just be back where he wants to be because that's where he belongs anyway.

**[1:18:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4702s)** Okay.

**[1:18:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4703s)** I I'm pretty sure this is not going to bode well because cuz it was doing weird stuff when that thing started out, but Oh, and then also Okay.

**[1:18:43](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4723s)** Okay, let's try one more time. Desert clear.

**[1:18:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4732s)** Didn't work.

**[1:18:54](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4734s)** That's okay cuz we can just teleport where we need to be, so.

**[1:18:59](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4739s)** Yeah, I don't know why that bridge didn't respond, but I can look at that later. Okay, so let's see if this works.

**[1:19:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4750s)** This is intense.

**[1:19:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4753s)** Oh, crap. My periapt, I forgot to update it. Okay, well.

**[1:19:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4756s)** This is really cool anyway.

**[1:19:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4762s)** Oh, I'm actually doing pretty good.

**[1:19:24](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4764s)** Oh, okay, he tried to burrow. Okay, he burrowed. That was awesome. And there there come his babies.

**[1:19:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4770s)** Except why'd they re-burrow? That's weird.

**[1:19:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4776s)** Okay, this is really cool. He didn't cast cyclone though.

**[1:19:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4780s)** Where is he?

**[1:19:42](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4782s)** It worked.

**[1:19:45](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4785s)** Now, I wonder if he Oh, that's I wonder if this screwed it up.

**[1:19:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4790s)** Die.

**[1:19:57](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4797s)** Okay, did that change anything?

**[1:20:03](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4803s)** Sand sting. Hello?

**[1:20:13](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4813s)** Oh my gosh.

**[1:20:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4814s)** I think he's attacking me, but he's hidden. No fair.

**[1:20:17](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4817s)** No fair, Sand sting.

**[1:20:21](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4821s)** Yeah, he's he's invisible but attacking me, so that's pretty weird.

**[1:20:26](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4826s)** Okay, well, that actually worked better than I thought it would for our first run. Um, I don't know why he didn't cast cyclone.

**[1:20:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4834s)** I don't know why the babies kept burrowing and unburrowing like that.

**[1:20:40](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4840s)** Uh, play his morph animation. So, that worked really well, actually.

**[1:20:45](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4845s)** Okay, you know what? For now, let's disable these, um, cyclone things. Instead, let's just make Sandstone invulnerable.

**[1:20:55](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4855s)** Sand sting, rather.

**[1:20:57](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4857s)** Um, so, what you can do is you can just make units untargetable, basically. Make unit.

**[1:21:06](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4866s)** Okay, so, make Sand sting invulnerable.

**[1:21:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4870s)** And I'll make sure that when he is See, it should have unhid him, so I don't know why that didn't work, but make him vulnerable again.

**[1:21:22](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4882s)** First before I anything else I needed.

**[1:21:27](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4887s)** Make sure I fix this pairing up here.

**[1:21:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4890s)** And I'm just going to get rid of this guy because he's problematic with this reincarnation ability.

**[1:21:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4894s)** Um, so, he burrowed, which was fine, and then the babies came out, which worked way better than I thought it was. That was really cool.

**[1:21:45](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4905s)** They unburrowed.

**[1:21:47](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4907s)** And then, why didn't they just attack?

**[1:21:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4911s)** Maybe I just need to wait a little bit.

**[1:21:53](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4913s)** I just figured like when it dies and then after it dies, there's none of that, but I'll just do equal or less than one.

**[1:22:05](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4925s)** Just in case. And maybe that's why the Yeah, maybe that's why this didn't work, either. That's a good point. Less than or equal to one.

**[1:22:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4934s)** All right, let's try that. Okay, I'll give this a couple more tries if we're close, but I think it's that that time that we should finish up for now.

**[1:22:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4943s)** This was so fun, though. I really enjoyed making some of these triggers. I hope they're clear to you guys.

**[1:22:31](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4951s)** I won't be able to see you guys' chat for now.

**[1:22:34](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4954s)** Uh Jonathan Park. Okay.

**[1:22:44](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4964s)** Whatever. Let's just stick with the program here. Okay, here we go.

**[1:22:49](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4969s)** Here it comes.

**[1:22:54](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4974s)** His collision is way too small. Like I'm just like under him attacking him. He needs to be a bigger circle. So, okay, cool. So, I can't attack him till he goes he burrows.

**[1:23:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4984s)** The babies come out.

**[1:23:06](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4986s)** And then they go down.

**[1:23:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4987s)** I do not know why.

**[1:23:10](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4990s)** Stop it, you little weasels. Okay, there he comes out again.

**[1:23:16](https://www.youtube.com/watch?v=EVZ5740YaGA&t=4996s)** So, we can at least So, something's working, but Okay, there he goes again.

**[1:23:26](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5006s)** Goes to burrow.

**[1:23:33](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5013s)** I also probably need to pause him so that he can't attack while he's doing that cuz he tried to attack me instead of burrow, which isn't fun.

**[1:23:45](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5025s)** Ah!

**[1:23:46](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5026s)** Why didn't he get unhidden? That is the weirdest bug.

**[1:23:51](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5031s)** Okay.

**[1:23:54](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5034s)** Let me check the unhide bug thing real quick.

**[1:23:58](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5038s)** I wonder if there's just an easier way to do this.

**[1:24:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5041s)** Like Maybe I'll just take away their burrowing ability.

**[1:24:06](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5046s)** That's weird, yeah. Okay, so disable ability true. How do I true? Okay.

**[1:24:11](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5051s)** Let's just try that and see if that stops them from burrowing. I don't know if it will or not, but So, unbury unberry just triggered as soon as it So, number of units owned by player two blue of type sensing Zerg less than or equal to one. So, I don't know why it appeared before there was more than one left.

**[1:24:32](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5072s)** Maybe it was because they weren't burrowed.

**[1:24:36](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5076s)** That might be the problem.

**[1:24:39](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5079s)** I don't know why that would make a difference, but I guess we can try. And then, unhide. Why didn't it just unhide?

**[1:24:49](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5089s)** Whatever. Let's just try one more time.

**[1:24:50](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5090s)** I also don't know why the bridge isn't coming back to life, but whatever. Okay, last time and then we'll call it a night. Next time we'll make some cutscenes and I'll show you guys how to do use like cameras and stuff like that and we'll see how it works, but So, weird that that doesn't happen, but okay.

**[1:25:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5114s)** On guard, Sandshrew.

**[1:25:23](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5123s)** There he goes.

**[1:25:25](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5125s)** Burrows.

**[1:25:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5128s)** Okay, come out.

**[1:25:30](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5130s)** And then they attack. Okay, I'm glad that works. Oh, and then he one pops out. There's something so weird about that.

**[1:25:41](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5141s)** Okay, then he goes again. This is so cool though because I mean Okay, there he is again. Okay, cool, cool. This is working out pretty well, actually.

**[1:25:52](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5152s)** Start with one of the units so that helps.

**[1:25:56](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5156s)** It's kind of fun to fight like with them attacking me, too, though. So, I don't know.

**[1:26:01](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5161s)** This is last batch of babies and then he should die.

**[1:26:07](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5167s)** Here we go.

**[1:26:11](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5171s)** Nice.

**[1:26:14](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5174s)** Oh, now it comes up. Okay, there must be something with the triggering mechanism for that one, but that was pretty cool, guys. Oh, no.

**[1:26:24](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5184s)** Oh my gosh, I love triggers. So funny.

**[1:26:29](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5189s)** Okay, that was not bad. And uh yeah, I don't know why that's not working and I don't know why it triggers with the babies and stuff, but I can look into that. Overall, that was pretty freaking awesome.

**[1:26:42](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5202s)** That felt much more like uh There There are just not very many boss battles at all in the campaigns on Warcraft 3 that are like that, you know, with the like different stages sort of thing. It's just hack hack hack till it dies.

**[1:26:56](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5216s)** Especially like the the forgotten one on on the you know, whatever it's called, the uh the undead campaign on Frozen Throne. Worst bass but base boss battle ever. Like he's immune to magic, so you just hack at him till he dies. It's so lame. But like that was a fun battle. I Once I fix this up a little bit, I think this will be really fun. Um yeah, so basically the thing was sensing initialize. Yeah, I just turned this

**[1:27:25](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5245s)** trigger off when it happened, so.

**[1:27:28](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5248s)** Okay, I think we will call it a day um for those triggers.

**[1:27:35](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5255s)** Super fun. You guys I That was a fun stream. I uh I hope you enjoyed that. I really enjoyed it.

**[1:27:41](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5261s)** There's so many more triggers that you can do. We might Who knows? Maybe Maybe I'll make another stream like about like a more advanced triggers, like leaderboards or something like that. I don't know, but I hope that at least kind of give you guys a taste of what triggers are worth and what scope they can make, so.

**[1:28:00](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5280s)** We made some pretty cool stuff, especially that boss battle. I'm really excited for that, so.

**[1:28:04](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5284s)** Okay, have a good night and uh I'll see you guys next time.

**[1:28:09](https://www.youtube.com/watch?v=EVZ5740YaGA&t=5289s)** Job's done.
