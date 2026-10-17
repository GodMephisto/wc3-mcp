---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/faq-for-wehz.228993/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Faq for Wehz
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/faq-for-wehz.228993/>
#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

#
 FAQ @WEHZ

##
 By Chaosy

##
 Introduction

There are many threads that have been created asking for the same thing this is why I created this tutorial I want to take the most common questions and answer them here.​

​

##
 Table of contents

Why is my icon green??

How can I modify armor, attack speed, damage, sight range etc?

How can I extend my map bounds to 480?

How can I increase my hero max level?

My creeps run back to the spawn very fast how can I modify this?

I dont know a sh*t about jass so can somebody make a GUI save and load system plx?

My game suddenly freezes/lags?

I can't load some maps! HELP!

How can I create txt files via triggers?

Negative Values in the Object editor

Final Word

##
 Why is my icon green?

When you for example pause the game (f10) the game wants to use a darker version of the icons showing, but then you need two versions of the icon. example

BTNmegadamagespell.blp

DISBTNmegadamagespell.blp

now you got the normal version and the darker version of the icon imported and the game wont show the ugly green icon.​

​

##
 How can I modify armor, attack speed, damage, sightrange etc

Those are impossible to modify with normal GUI actions or JASS functions but there are workarounds.

The key to this is item abilities they are kind of hidden abilities for your hero since they got no icon, so basically if you want a 20% attack speed increase you can add the gloves of haste item ability to your unit.

list of useful base abilities:

attack speed: gloves of haste

sight range: goblin scope

armor: item armor bonus (x)

damage: item damage bonus (x)

if you are lazy you can just import bonusmod by earth-fury or bonus by Nestharus​

​

##
 How can I extend my map bounds to 480?

For this feature you need to install JNGP (Jass New Gen Pack) and run the editor there and then simply go to "scenario>map and camera bounds"

to install JNGP you can follow one of the linked tutorial bellow.

https://www.hiveworkshop.com/forums...456/how-download-install-confige-jngp-160547/​

​

##
 How can I increase my hero max level?

This is really simple and you can do it in the normal editor no need for JNGP, go to "advanced>gameplay constants>check the box who says use custom gameplay constants> then find the field Hero - maximum level"​

​

##
 My creeps run back to the spawn very fast how can I modify this?

you use the same as above, go to "advanced>gameplay constants>check the box who says use custom gameplay constants> then find the field creep return distance and creep guard return time"​

​

##
 I don't know a sh*t about jass so can somebody make a GUI save and load system plx?

I am sad to tell that there are no good GUI save loaaad right now but I can help you to the best you can get.

1. AceHarts save/load system is GUI but it's a lot of setup right there.

2. codeless save/load system this is indeed a very good system with a lot of documentation to help you out and got "GUI support" but you need JNGP to use it since the code itself is full VJASS

3. wait for Radamantus finish his GUI save/load system (if he ever do)​

​

##
 My game suddenly freezes/lags

This is most likely because of triggering leaks in GUI or not declaring locals in JASS. Leaks are data that got to be removed once used else they just remain and take unneeded ram memory, note that a single leak do no harm if it isn't in a super fast loop trigger or something.

anyway, I will tell you how to fix the most common ones.

1. point leaks: if you use a point example

Actions

set MyLocationVariable = (position of triggering unit)
 then when I'm done with whatever I need the point/location for I NEED to remove that with a custom script (jass).

Custom script: call RemoveLocation(udg_MyLocationVarable)
2. unitgroups: those are a little harder because it depends on how you use them. if you use a temporary unitgroup like "pick every unit within 400 of loc" then you simply put

Custom script: set bj_wantDestroyGroup = true
 above the unit group

Actions
Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick every unit in (Units in (Playable map area)) and do (Actions)
Loop - Actions
-------- I love leaks --------

-------- I mean actions here --------

groups who remain all the time are pointless to remove since that would just cause trouble since you need to re-add the units again after. this is a example which I use in most spell I make.

I love leaks
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in some_spell_group and do (Actions)
Loop - Actions
-------- spell code here --------

3. player groups: to make those not leak you need to put them into a player group variable and then destroy the group with the following script.

Untitled Trigger 001
Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
-------- do stuff here --------

Custom script: call DestroyForce(udg_MyPlayerGroupVariable)

4. Unit Leaks are removed easy and best of all you do not need JASS scripts to fix it, you simply use the add generic timer action as in the example bellow

Actions
Unit - Create 1 Footman for Player 1 (Red) at Loc facing Default building facing degrees

Unit - Order (Last created unit) to Human Sorceress - Polymorph (Triggering unit)

Unit - Add a 1.50 second Generic expiration timer to (Last created unit)

Custom script: call RemoveLocation(udg_loc)

here we create a dummy unit and order it to cast polymorph

5. Special effect do also leak these are also JASS free and easy to remove. We use the destroy special effect button.

Actions
Special Effect - Create a special effect attached to the overhead of (Triggering unit) using Abilities\Spells\Other\TalkToMe\TalkToMe.mdl

Special Effect - Destroy (Last created special effect)

This will allow the effect to play its animation once before it's removed.

But if you want to remove it later you need to store it into a variable and destroy it later.

For more info about leaks and how to remove them you can visit the following tutorials:

https://www.hiveworkshop.com/forums...quick-tutorial-common-triggering-tips-190802/

https://www.hiveworkshop.com/forums/triggers-scripts-269/things-leak-35124/​

​

##
 I can't load some maps! HELP

A few maps use HQ/HD texture models which forces you to have the highest grafic settings to run the map, just go to options at the main menu and then max everything now you should be able to load the map.

If that does not work it's most likely your wc3 who is corrupted try reinstalling it in that case.​

​

##
 Skin/model is not working! what am I doing wrong?

This is a major little trick that most modders ask quite fast after downloading a few models the first time. Some models use a texture files (aka .blp files) you NEED to edit the path of those else it will bug.

1. import every file needed for the model(.mdx + .blp files not readme.txt)

2. check the readme.txt (if there is any) or visit the site where you downloaded your model there you can see if the texture files uses a special path I have seen a few of those.

3. if you don't find something there you just remove the wc3imported\ from the path of the .blp files and your model or skin should work fine.

for skins there is always a special path so make sure you follow step 2.​

​

##
 How can I create .txt files via triggers?

Those of you who ask this have most likely seen it in a RPG who store their load codes like that, I got good news on this point.

I can tell that I got a script that even a blind donkey could use.

easy preload
Events
Map initialization

Conditions

Actions
Set path = C:\random\

Custom script: call PreloadGenStart()

Game - Preload -load KEUg-YaNc-rDDq-wQ54-9Zyr-7Wm2-mp5W-YaS2-77HP-4Cg3-aZbP-PxZp-SQs2-hsd-asdasdasda-sdasdasdas-dasdasdas-dasdasdasd-asdasdasd-asdasdasd-asdasdasd-asdasdasd-asdasdasdas-dasdasda-dasdasdas-dasdasda-sdasdasd

Custom script: call PreloadGenEnd(udg_path +"test.txt")

so here you just set the path where you want to store the txt file I used c:\random\ is this case. not that if the folder "random" doesn't exist it creates one automatically.

then it loads whatever you want and saves it as test.txt

if you want a better one I suggest heading over to the "file I/O @spell section"​

​

##
 Allowing Negative Values

Negative values can be used in many ways and are actually more useful than you think. A very common use for a negative value is armor removal. By using the Paladin's Devotion aura (or any other armor ability) and then setting the armor gained to a negative value you will decrease the armor of targets effected by the aura/ability.

To do this you need to click on the tab named "file" inside your world editor, then preferences, then click the general tab, and then check the box named "Allow negative real values in the Object editor". Once that's done hold down shift when you click the value that you want to edit. Then write down a negative value inside the object editor. It is that simple, yet a common question.​

​

##
 Final word

Thanks for reading my tutorial and I hope as many users as possible read this before posting a thread about it.

also if you got a question that you think would suit here please leave a comment​


 Last edited: Nov 23, 2016

#### twat

#### twat

 Level 3

JoinedJan 4, 2013Messages38

you may want to rethink this a bit psychologically. your target audience is goldfish people who cant be arsed to search the forums before they post a thread. its a blatant case of microwave mentality. so in any FAQ ease of access and smooth formatting plays a central role.


first problem is finding the thread. if this stays buried beneath piles of different tutorials in a totally different section people arent going to just stumble upon it. either you have a moderator sticky this in WEHZ or it's not going to be more than wasted cyberspace.


second problem is formatting. any way you slice it it's a big post and scrolling sucks. you need to settle this with a table of contents using [goto] and [point]

 Table of Contents


Why do we fall?

THEN WHO WAS PHONE?

What is love?

and probably separate the Q&A with something like the [table] code.

Why do we fall?

So we can learn to pick ourselves up.

THEN WHO WAS PHONE?

IT WAS FONE.

What is love?

Baby don't hurt me.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

some of these questions are not only for "gold fish" people I belive you can find which ones.

I have seen quite alot of people with atleast 100 posts asking these things and by that time im sure they have visited the tutorial section. But I get your point.

and I will see what I can do about the table thingy

edit: added


 Last edited: Jan 20, 2013

#### Jas.per

#### Jas.per

 Level 12

JoinedMar 28, 2012Messages259

Resources: 27

#### Resources
Models 7Icons 11Tools 1Maps 7Tutorials 1

cool

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

undetroyed effects leak

tnx for mentioning me about the save load.

the system will take me so much time

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Totally forgott this one

Indeed, I will add unit leak too.

thanks for your comment

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

There's something wrong in this tutorial about creating the .txt file.


- Creating a txt file by abusing the preload script does not cause any desyncs in multiplayer. Many maps use it safely without any problems.


- Creating a txt file does not need local files enabled. Only reading a .txt file does.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I think twat (lol) makes a good point. Also, the tutorial seems a bit too brief (as in, not enough topics) to be a proper FAQ. Seems a bit randomly pieced together. I might need some people to convince me otherwise. I think these are commonly asked, but I don't know if they warrant a tutorial.

Also, you have to update in accordance to what Zwieb said.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Tutorials could be useful, but hide those triggers in hidden tags.

#### Binsu

#### Binsu

 Level 30

JoinedNov 29, 2012Messages6,631

Resources: 5

#### Resources
Maps 4Templates 1

This looks like a useful tutorial. Everything was explained well and shown well. Way of presenting the tutorial is nice also. Some of the questions are quite easy for me but others are not familiar and you have just answered it.


I think for me, this deserves an Approval! +3 REP, chaosy, Nice one!


But I think this should be in WEHZ Section and stickied if you will ask me.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

very useful I think... btw, apart from set-up (which btw, I believe that all save-load systems has a lot of), AceHart's system also stops working when you save high values or lots of values...

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Seems like this needs a update.

I will edit it tomarrow I think

#### maddeem

#### maddeem

 Level 24

JoinedJan 1, 2011Messages1,506

Resources: 34

#### Resources
Models 5Icons 3Skins 1Maps 6Spells and Systems 18Lua 1

I had no idea you could create text files. Thanks for sharing

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

@maddeem

Hell yeah, I played with it and made 3 systems

#### Heinvers

#### Heinvers

 Level 53

JoinedMay 7, 2010Messages9,267

Resources: 101

#### Resources
Models 3Icons 65Packs 3Skins 27Templates 3

That's a good tutorial.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

I renamed this thread because it looked like it was called "Fag @ wehz"

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

hehe, thats. I actually didnt notice that.

@heinvers

thanks dude.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Holy.. I found like 300 spelling mistakes. I am kinda ashamed over the fact that I left it in such a state for so long.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

"armor, attack speed, ..."

Please add link to BonusMod.

nestharius -> Nestharus

"Bound to 480" -> "480x480",

also please link to new JNG

"save/load"

Always provide a link when you mentioned a certain submission.

"leaks"

Just quickly explain that memory leaks might be the reason for spikes or fps drops, and then link to respective tutorials.

No need to explain and give examples here how exactly to remove leaks.

"can't load maps"

You should make it clear you mean starting the game, and not loading in editor.

Maybe you also can make a "can't load in editor" part, which says map is probably protected/optimized/ or so.

"skin /model not working properly"

Not sure we need this anymore. There are now always explainations how and what to change exactly in the sections.

Will also think of what point could eventually be useful to add, too.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Fixed various grammar and spelling mistakes

Fixed the layout since [box] do not look like it used to on the old hive.

Linked AceHart's system

Fixed Nestharus' name.

Specified that the map loading issues is for ingame only.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

What about other changes I asked for?

You also can remove outdated things like this:

Chaosy said:

 wait for Radamantus finish his GUI save/load system (if he ever do)


Click to expand...

or the JNGP video, it doesnt work.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

It was deleted, but if it gets un-deleted it will work again.

I think the creator can do that. But I can remove it I suppose.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

So, what about other mentioned points finaly?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Submission: FAW for WEHZ

Date:31 December 2016

Status:Graveyard

Note:

Some things are not kept up to date, and there's no proper discussion going on atm.

Also, the preload chapater, as intance, does also require a whole new writeup due to last patches.

Such QA threads must be cared about more, so I'm moving this to graveyard for now. If author shows more acitvity we will re-evaluate it.

 You must log in or register to reply here.


### Similar threads

[GUI]
 Save and Load in GUI


Chaosy

Oct 24, 2014

Tutorial Archive

Replies19Views6K

Nov 28, 2016
IcemanBo

 Map FAQ


Rui

Nov 12, 2005

Tutorial Archive

Replies0Views3K

Nov 12, 2005
Rui

 How to remove Memory Leaks IN GUI


KingB00ker

Jul 6, 2009

Tutorial Archive

Replies18Views6K

Jul 12, 2009
Pyritie

Locked

 Spells - Frequently Asked Questions (FAQ)


The_Raven

Jul 25, 2004

Tutorial Archive

Replies1Views47K

Jan 27, 2021
Chaosy

 Using Custom Values in Triggers


Herio-san

May 6, 2007

Tutorial Archive

Replies16Views3K

Sep 1, 2007
Wolverabid

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top