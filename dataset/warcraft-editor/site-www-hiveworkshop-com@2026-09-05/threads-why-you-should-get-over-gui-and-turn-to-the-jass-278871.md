---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/why-you-should-get-over-gui-and-turn-to-the-jass.278871/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Why You Should Get Over GUI and Turn to the JASS
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/why-you-should-get-over-gui-and-turn-to-the-jass.278871/>
#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

It's 2016 already. WC3 as a game pretty much outdated, and the only reason it still exists is custom maps.

Every little thing matters. You are creating another hero arena? Cool. Another branch of a map, which been popular once, but forgotten now? Good for you. But time and efforts aren't infinite for anyone.

Using GUI is like building a scyscraper with a team of retarded builders. They are known for their cheap prices and principles "what you see is what you get", but any of their approaches can't stand any chance in real world. Some of floors are rotated by 90 degrees, and, to compensate it, they added a small rotation over the whole building. Some of floors missing windows or doors, or even locked completely. Instead of proper foundation before building they prefer to drop a plate and start it right now. And, of course, you barely able to control any of those processes - you aren't pro to do so, right?

Pure JASS, instead, is a tool which creates only those agents you'd like to see or use. You playing Black & White here, declaring how should work this or that thing. You can go and create a custom design for one of the floors at any time. You can make sure building won't fall off using solid foundation of some pre-made big systems. As well nobody limits your ideas with a menu of possible actions. Well, except the game itself, but anyway you cannot touch the sky yet.

So, why you should move?
Let's start from obvious things which actually matters directly.

I'd like to use a list of differences, provided by another scripters, but with slightly changed order:

You can have local variables

It's faster to write (once you get used to it)

The way GUI converts into jass contains alot of useless function-chain calls and double negative conditions

You can't write custom object-oriented code in GUI

There are certain natives available in Jass that are not available in GUI (i.e. setting the z of lightning objects)

You can easily transfer Jass code from one map to another (Just CnP) but you can't do this as easily with GUI

Click to expand...

Let's face them in appropriate manner.

Local variables

GUI doesn't allow to use local variables, there are nothing like that. Maybe Blizz were too lazy to collect them on the top, or too lazy to make wrappers for them and additional rules, but fact is - you cannot store anything temporarily, only permanently. Which means you have to recycle a few globals in order to avoid collisions or create personal global for everything. Flood of variables is the least thing you normally cares about, but handling them is very tricky job.

Faster writing

Remember how a hacker in a movie types stuff in command line and do his job better & faster than a person who uses normal Window's windows? That's kinda exaggeration, but still true in some cases, and WC3 - one of them. GUI is slow to type, you cannot really speak your mind with it. Anyone with typing skills, who used to argue on the net, can type about 80 symbols per minute. Adding autocomplete and syntax highligh there and you'll see drastical changes. When I started it took a day to implement simple changes into various GUI functions. Now I can create a complex skill within 5 mins.

GUI is harsh for CPU

Well, half true. Most of GUI functions are unneccessary wrappers, which adds 1 more action. But there are also totally unneeded overkills like CreateUnit, which uses the slowest approach ever when you need to only create a single unit. On the bright side, human cannot tell the difference in normal conditions. But if you gonna deploy 50 units with this approach you'll meet lags or something even worse. Every non-opimized line increases a chance of dropping weak PC-players or even reaching an operation limit.

So, basically, GUI causes a small overheat, which is getting bigger with everyline you write. And that could be avoided with JASS.

User functions

Copy & paste is very popular method in GUI, since you can't really make a custom functions to move some repeated part aside. This increases chance of errors, floods the code and simply uneffective. You have to do so, because GUI isn't supposed to be clean and shiny, right? But you aren't supposed to have infinite time to fix variable naming, passed to the loops, either.

Limited options

Not to speak of GUI's restricts, there are also no support for some natives, and sometimes it's really big deal. You could already met it. If you didn't, you did something wrong.

Moveability

Knowing JASS, you can take any piece of a code from another map to yours. You can research, learn and re-built anything having only a simple example. You don't need a guidence anymore - in case of similarities you can freely explore how it's done and repeat without any issues. JASS code is much simpler to move within a single map than GUI, as well.

Overall

So, as I said before, JASS is a language you'd like to use to built your own castle. GUI, instead, is a group of second class builders who supposed to do your job but mostly fails and kills the whole project at some moment. Now let's speak about how to move.

Moving on
JASS isn't a real language..

As it states, Just Another Script Syntax isn't a real programming language. You won't see any kind of tricky bit operations here. It have nothing to do with Object-oriented programming neither. And, of course, you don't have to be a high school educated to understand it. If you are native speaker or even just know English well, you'll read whole thing without problems.

It's a scripty thing, which allows you to iteract just with some things, which has been allowed by Blizzards. Wanna check unit's health? Ok. Wanna check which attack type he has? Nope, you aren't supposed to. Instead, you may create a vault with data to retrieve it later with your own functions. Of course, maintainnig it would be completely on you. You can't get out of sandbox you've been provided with.

.. but any experience matters

In childhood I was enchanted by Heroes 3.5 WoG. I loved Heroes 3 very much, and addon's possibilities were like god's blessing. Later I tried myself in programming, but didn't really successed with. But anyway, that allowed me to enter JASS like open door and explore it in the wild. Anyone who ever met and understood scripting (even HTML) should be able to easily get over pure JASS. I don't call programmers here becuase I'm sure they never ever used GUI.

Use proper tools

Plain World editor has been created back to 2003 and never received any goods since then. That means it's outdated, and you have to move onto JNGP as soon as possible, to get those hightlights, hints and tips. They will not only improve your code processes, but Editor's possibilities. Here's a guide and the JNGP . You can as well search for something fresher but that's enough for a start.

How to get smoother?
JASS could be read like a book. I had very little idea about English back then, but still successfully understood things there and here. Practice makes everything better, so you can go to:

Read some basic JASS guides to understand principes of variables and
function calls.

Open your or somebody elses map using any MPQ editor, extract war3map.j file and read it.

I had no idea about JASS at all. I made few efforts with GUI before, but no luck. And one day a guy called me and asked for help debuggin a map he found before. It turned out to be Diable Defense, not that small project, with ~3k lines of pure JASS. I started as a total newbie, who had really bad aknowledge of JASS, but was very used to see it's results in DotA. 2 months later I dissected whole thing into ~100 triggers, basing on it's dependancies and effects. Since then I only learned various tricks and special issues, but JASS itself wasn't a problem anymore at all.

Apply your knowledge

No matter how good you, rust is always coming. If you don't train yourself, you will forget simplest things. Gladly here are Google who knows pretty much all default user-cases of JASS usage. I'd recommend to use it like "wc3 jass query" style, so output would be maximum relevant. "wc3 jass create unit", "wc3 jass how to toss unit flying", etc.

Create small snippets of code for your project in custom triggers. Read common.j and blizzard.j for something new. Extract .j files from any non-melee Blizzard map and dive into simple examples of things around. Once those stages are done, you can read something actually worthy reading, like various complex maps you enjoyed to play before.

About vJASS and following

All those OOP stuff, classes, etc, provided with vJASS, cJass and other derivatives aren't obligatory. They aren't needed at all if you gonna work on your map onwards right now. Don't make it's more complex than it is, do your job and make your ideas live. Rest will come with time and experience.

Sounds like hell
Well, probably. Learning isn't easiest thing, thats why people use schools - children have better learning abilities, thanks for natural selection. But there are few really good things you'll get in exchange:

Free to code anything you want.

 Able to reproduce anything what has been seen before in some other maps.

 Less questions like "why my trigger doesn't work" (not at the start though).

 Ability to use any code written by somebody else (JASSers known for awesome systems, GUI isn't that lovely, for obvious reasons).

 Possibility to improve Warcraft 3 community.

Oh hello, here's global reason to get better - to help WC3 survive. There's dozen of clones around - DotA2's LUA, various open source and free-to-non-commercial-use engines, even SC2's editor, and more to come. Players actively migrate onto different games, creators can't handle the static community and leave as well. That means every creator is valuable, and the better you project will be - the better chance of living for WC3 as well.

I know no projects which has been written on GUI and turned out to be successful since 2010. There are examples before that date, but not since. Learning JASS isn't just a blant, useless idea - it will grants you ability to understand flow of any script in future.

Few more benefits

JASS is easier to debug (since it's easier to read as well)

JASS strictly follows your order and you always know what every line does (nothing hidden behind "GUI")

JASS much cleaner than GUI


 Last edited: May 5, 2016

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I don't know about the easy to read part..

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

Chaosy said:

 I don't know about the easy to read part..


Click to expand...


few conditions turns GUI to mess, not to speak about how hard to understand included functions. like "where has been this location/unit defined before? where is this comes from?". while JASS allows you just to look above or search directly, holding every line on almost the same point of view everytime

#### Ezekiel12

#### Ezekiel12

 Level 12

JoinedMar 13, 2012Messages1,121

I like how you end the post on a motivating note.


Some minor criticism:


-calling wc3 a dead game, which can be discussed and surely is true from certain viewpoints should maybe not be part of a guide.


-using the word "retarded" (although you are not addressing the reader) or belittling people who use GUI.


I know, JASS is better, but there are people who had and have fun just using GUI and this website should welcome them and not shriek at them to instantly change their style because "you become more productive". It's mainly about fun.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

I find GUI's swapping arguments, additional unneeded lines and few other ideas "retarded" for reason - they confuse people AND their work.

ye, should have a better mood before starting, will check out for wc3

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

Chaosy said:

 I don't know about the easy to read part..


Click to expand...

It's definitely easy to read.

 JASS:

if (UnitAlive(u)) then
 call KillUnit(u)
endif

Of course it can get complicated with long code but JASS in of itself is very easy to follow.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Your example do not look that ugly in GUI in my opinion. Maybe I'm used to it, but jass looks ugly as fuck when getting into some complex stuff.

 JASS:

 if .target == null then
 set d = .boneZOff + GetUnitFlyHeight(.unit)
 call LockBone.lockAtAngle(.unit, .whichBone, .angle, d, d, d)
 else
 set d = SquareRoot((targetX - droneX)*(targetX - droneX) + (targetY - droneY)*(targetY - droneY))
 call LockBone.lockAtAngle(.unit, .whichBone, .angle, d, d, .boneZOff + GetUnitFlyHeight(.target))
 endif
 elseif .angle != a then
 set .angle = a
 /* If drone doesn't have target */
 if .target == null then
 set d = .boneZOff + GetUnitFlyHeight(.unit)
 call LockBone.lockAtAngle(.unit, .whichBone, .angle, d, d, d)
 else
 set d = SquareRoot((targetX - droneX)*(targetX - droneX) + (targetY - droneY)*(targetY - droneY))
 call LockBone.lockAtAngle(.unit, .whichBone, .angle, d, d, .boneZOff + GetUnitFlyHeight(.target))
 endif
 endif

Yeah so easy to read.

NOT. Messy af

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

Chaosy said:

 Your example do not look that ugly in GUI in my opinion. Maybe I'm used to it, but jass looks ugly as fuck when getting into some complex stuff.

 JASS:

 if .target == null then
 set d = .boneZOff + GetUnitFlyHeight(.unit)
 call LockBone.lockAtAngle(.unit, .whichBone, .angle, d, d, d)
 else
 set d = SquareRoot((targetX - droneX)*(targetX - droneX) + (targetY - droneY)*(targetY - droneY))
 call LockBone.lockAtAngle(.unit, .whichBone, .angle, d, d, .boneZOff + GetUnitFlyHeight(.target))
 endif
 elseif .angle != a then
 set .angle = a
 /* If drone doesn't have target */
 if .target == null then
 set d = .boneZOff + GetUnitFlyHeight(.unit)
 call LockBone.lockAtAngle(.unit, .whichBone, .angle, d, d, d)
 else
 set d = SquareRoot((targetX - droneX)*(targetX - droneX) + (targetY - droneY)*(targetY - droneY))
 call LockBone.lockAtAngle(.unit, .whichBone, .angle, d, d, .boneZOff + GetUnitFlyHeight(.target))
 endif
 endif

Yeah so easy to read.

NOT. Messy af


Click to expand...

That's vJass bud. and of course math makes code harder to follow.

GUI is even uglier and harder to follow / maintain with longer code.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

Chaosy said:

 Your example do not look that ugly in GUI in my opinion. Maybe I'm used to it, but jass looks ugly as fuck when getting into some complex stuff.

 JASS:

 if .target == null then
 set d = .boneZOff + GetUnitFlyHeight(.unit)
 call LockBone.lockAtAngle(.unit, .whichBone, .angle, d, d, d)
 else
 set d = SquareRoot((targetX - droneX)*(targetX - droneX) + (targetY - droneY)*(targetY - droneY))
 call LockBone.lockAtAngle(.unit, .whichBone, .angle, d, d, .boneZOff + GetUnitFlyHeight(.target))
 endif
 elseif .angle != a then
 set .angle = a
 /* If drone doesn't have target */
 if .target == null then
 set d = .boneZOff + GetUnitFlyHeight(.unit)
 call LockBone.lockAtAngle(.unit, .whichBone, .angle, d, d, d)
 else
 set d = SquareRoot((targetX - droneX)*(targetX - droneX) + (targetY - droneY)*(targetY - droneY))
 call LockBone.lockAtAngle(.unit, .whichBone, .angle, d, d, .boneZOff + GetUnitFlyHeight(.target))
 endif
 endif

Yeah so easy to read.

NOT. Messy af


Click to expand...

not to speak that's not JASS but it's deriative, created SPECIALLY for those who knows programming very well. but that's as well math-based example, pretty simple. still it's readable and, having no idea what is it, I can say what it uses without having to deal with horizontal scrolling.

 JASS:

call MBSetText(MainMB,2,1,"CD")
 call MBSetText(MainMB,3,1,"|c00838B8BL|r")
 call MBSetText(MainMB,4,1,RED+"K|r")
 call MBSetText(MainMB,5,1,BLUE+"D|r")
 call MBSetText(MainMB,6,1,GREY+"A|r")
 call MBSetText(MainMB,1,2,GetObjectName('n0DK'))//n0DK -> The Sentinel
 call MBSetText(MainMB,4,2,RED+"0|r")
 call MBSetText(MainMB,5,2,BLUE+"0|r")
 call MBSetText(MainMB,1,3+integer411,GetObjectName('n0DO'))//n0DO -> The Scourge
 call MBSetText(MainMB,4,3+integer411,RED+"0|r")
 call MBSetText(MainMB,5,3+integer411,BLUE+"0|r")
 call MBSetColor(MainMB,1,2,Func0079b(Sentinels[0],4),Func0079b(Sentinels[0],6),Func0079b(Sentinels[0],8),0)
 call MBSetColor(MainMB,1,3+integer411,Func0079b(Scourges[0],4),Func0079b(Scourges[0],6),Func0079b(Scourges[0],8),0)

all main functions are my own functions, declared before. I don't have to repeat 5 similar actions per every line, but use a single function. Thats as well much more readable than any kind of GUI

#### blancfaye7

#### blancfaye7

 Level 10

JoinedMay 21, 2014Messages580

Resources: 1

#### Resources
Maps 1

Because I'm a programmer, I would have preferred JASS.

But well, what do you know... I don't have time for it so I stick with GUI.

If there was time to learn JASS, I would just use it to learn more about Ruby.

JASS is just used for hobbies, whilst Ruby weighs more.

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

blancfaye7 said:

 Because I'm a programmer, I would have preferred JASS.


But well, what do you know... I don't have time for it so I stick with GUI.


If there was time to learn JASS, I would just use it to learn more about Ruby.


JASS is just used for hobbies, whilst Ruby weighs more.


Click to expand...


If you have any real programming experience than you can use JASS right away. It's the easiest syntax to learn by just looking at it, and all the function names are self explanatory. Not to mention it's extremely basic with only functions, variables, and one type of loop.


I can't imagine anyone with real coding knowledge to use GUI with the excuse of "I don't have time to learn JASS".

#### LimitingBounds

#### LimitingBounds

 Level 3

JoinedMay 12, 2016Messages51

Resources: 1

#### Resources
Spells 1

I think GUI is like building a skyscraper with a contractor, with some retards, that intends to extort money from you with this project, that can only speak a foreign language you don't know, and you talk to them through a translator.


 Last edited: May 20, 2016

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

The easiest way to get started with jass, is to create a simple jass spell. It may takes an hour or so for your first time. But it will help you a lot in understanding how to create things in jass. And after learning how to write efficient jass code, you will learn how fked up GUI is. And more importantly, you will learn how easy it is to make things in jass/vJass than in GUI. That's why once I learned jass I never came back to GUI, and so everybody else too.

Once I created a quite complex GUI spell. It still has a bug which probably until today I can't fix, other than to re-write the whole code. What I'm saying is that, when it comes to complex things, GUI is much harder to debug. In vJass, you can easily use external libraries to save several hours of yours. E.g. in the spell I was making a hardcoded queue which took lots of hours to make it works properly (and yes you guess, it's still bugged after all). But in vJass, I can simply use structs and Nestharus' queue, then the spell will be MUI automatically without any pain and has much lesser chances to be bugged.

And lastly, fyi I learned jass and vJass before knowing anything about other programming things. So it means you don't have to be a programmer to learn jass!

#### blancfaye7

#### blancfaye7

 Level 10

JoinedMay 21, 2014Messages580

Resources: 1

#### Resources
Maps 1

TriggerHappy said:

 If you have any real programming experience than you can use JASS right away. It's the easiest syntax to learn by just looking at it, and all the function names are self explanatory. Not to mention it's extremely basic with only functions, variables, and one type of loop.

I can't imagine anyone with real coding knowledge to use GUI with the excuse of "I don't have time to learn JASS".


Click to expand...

To be honest. When I look at JASS codes, I don't even read them. I simply neglect them. Why? I myself don't know.

I started with world editor when i was in high school. I didn't know much about codes and things like that. Maybe I just brought it up until now.

And yeah I'm still an amateur. I always believed to take steps one by one. That's just probably me.

L1ch, though, showed me what it really is. Should've known better.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

Quilnez said:

 The easiest way to get started with jass, is to create a simple jass spell. It may takes an hour or so for your first time


Click to expand...

true. it took me like more than an hour just to make a simple spell with jass for the first time. I haven't practiced more since, though.

I didn't know Jass is easier to debug. Might try to learn Jass once more when I have lots of free time.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Jass is horrible language.

Needlessly long syntax and bad tutorials. I had massive issues learning it.

Not to mention retarded rules like having to declare locals at the top of the function, no global variables etc

Loops are annoying as well.

 JASS:

//C# syntax
for(int i = 0; i < 5; i++)
{
}

//JASS syntax
local integer i = 0
loop
 exitwhen i > 5
 i = i +1
endloop

I only learned it after learning C#, otherwise I would be stuck with GUI until this day.

Even after learning jass I was negative towards it, Zinc saved me I don't get why others don't use it over vjass.


 Last edited: May 21, 2016

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2


 Chaosy said:



 Needlessly long syntax


Click to expand...

which makes it verbose and easier to read which is what I'm arguing

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Maybe this is subjective but I fail to see why "returns nothing" is readable it might as well not exist.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

the "returns nothing" actually makes sense if you are a lua programmer.

I hate reading lua libs just to know if a function returns a value or no.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

So what that it's 2016? WC3 still has the best GUI around, I prefer to mix my GUI with a hint of jass. Not everyone wants to type 24/7, plus I like buttons... Especially clicking them.

For the really slow GUI functions or missing ones are the only ones you should use custom script/jass for. Otherwise there is no speed difference and it is basically the same. People should play with what they like to play with usually.

Also using GUI discourages hackers. One look at your .j script and they'll run away. =)

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

@Draco:

You are wasting your time here. You won't convince people to switch to JASS with just good arguments.

If I learned one thing in 10 years of WC3 modding, then it is that the motivation to learn JASS must be intrinsic, not extrinsic.

@Topic:

I actually like the JASS syntax a lot, even if it is very verbose. I like that it does not have C-like shortcuts on loops, since the JASS loop format imho makes it easier to follow. I hate it when people pointlessly try to force everything into a one-liner.

The only thing I don't like is the "call" keyword; but I like having the "set" keyword. Don't really know why. Maybe because it happened more than once in my programming past that I accidently overwrote a variable I actually wanted to compare.

And yes, being forced to declare locals on top of a function is annoying, but it has it's merits when you try to fix reference leaks.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Agree, verbosity isn't all that bad. Just look at VHDL, it's so verbose yet it's one of the most used HDL language

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Thanks for the tutorial but no, I will stick with GUI.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

Zwiebelchen said:

 @Draco:

You are wasting your time here. You won't convince people to switch to JASS with just good arguments.

If I learned one thing in 10 years of WC3 modding, then it is that the motivation to learn JASS must be intrinsic, not extrinsic.


Click to expand...

Wer hören kann, der höre zu.

Many people obsessed with GUI 'cause simply afraid of programming. They don't understand it, they're afraid of it. Just like its a fire. There's barely any different. Somebody cooking stuff with his own fiery factories, while another struggle to even get next to campfire.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

DracoL1ch said:

 Wer hören kann, der höre zu.


Many people obsessed with GUI 'cause simply afraid of programming. They don't understand it, they're afraid of it. Just like its a fire. There's barely any different. Somebody cooking stuff with his own fiery factories, while another struggle to even get next to campfire.


Click to expand...


GUI is pleasant to view and work with. Much more simple and pleasing. It is not surprising why most prefer GUI.

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

You're not gonna convince anyone as long as you speak about GUI users with a condescending tone.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

dtnmang said:

 You're not gonna convince anyone as long as you speak about GUI users with a condescending tone.


Click to expand...


its so beatiful, you can see tones inside plain text! look at him, wow. just wow. /condescending

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Nice engagement for supporting jass!

GUI is intuitivly simpler for non-programmers.

I think arguments about efficiency or proper coding (behind the scences) is not really important for the average mapper as long as his code works in the end.

So they don't bother with learning a new syntax if they can reach their goal with simple clicks.

(so I would not rely too much on "conditions are unefficient etc")

It's very important, as you also did, to point out the functional advanteges from JASS to GUI.

from you some:

- locals

- custom functions

- moveability

- limited options

At "limited options" you could add some useful examples, for example:

GetHandleId (for good timer usage, or general hashtable) or SetUnitX/Y

what can be added to list:

- transparency ( for example in GUI you don't see that group enum is an other function, which can cause confusion with locals)

- code structure (in jass you can put related code, also multiple triggers, into only one "trigger sheet")

When you mention MPQ editor, you might link to it.

And maybe it also can be linked to some JASS tutorials at given point, here are listed some, for example https://www.hiveworkshop.com/forums/miscellaneous-tutorials-456/elemental-coder-guide-274346/

But such a general gui vs jass tutorial is very good.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

You might want to mention that while jass is better you will get judged harder than a rapist in court. There are so many more things to bitch about when working in jass.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

Chaosy said:

 You might want to mention that while jass is better you will get judged harder than a rapist in court. There are so many more things to bitch about when working in jass.


Click to expand...


good thing is there's much more bitching about GUI at any given forum.

IcemanBo said:

 textwall


Click to expand...

will consider that, thanks

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

JASS in the WorldEditor has some problems, it does not grant you access to the whole file. You cannot declare globals and the generated supplements are separate. That's already reason enough why normal JASS does not cut it either. Another issue is linking preplaced objects to the code. When you place a unit in the WorldEditor 3d view, albeit it gets a number, it does not obtain a JASS variable right away, only after you have referenced it in GUI. That's the case for widgets at least and still, a picker for JASS is missing.

GUI vs JASS is also a matter of use case. GUI serves defined application, clings to basic wc3 features, JASS is suited for reusability and paves its own way. GUI is very static. I did get into the quantary of using it quite a few times. It's much more roundabout. Writing spells is a prime example because here you most likely require a parallelization of data. GUI only offers you arrays and hashtables, other basic data structures you would normally require all the time or iterators are missing out. In GUI you would have to implement them everywhere, which is an uglification^3 and error-prone. Moreover, you cannot instantiate/destruct a lot of the handle types nor just allocate an index as vJass structs do.

DracoL1ch said:

 I know no projects which has been written on GUI and turned out to be successful since 2010.


Click to expand...

I guess that's mainly due to the diminished player base, lack of innovation and that the developers who dug more into Wc3/JASS proved more patience in staying with the game.

Chaosy said:

 Even after learning jass I was negative towards it, Zinc saved me I don't get why others don't use it over vjass.


Click to expand...

Lack of support, legacy code and I think it lacked some features. In the end it's all about the workflow.

Chaosy said:

 Maybe this is subjective but I fail to see why "returns nothing" is readable it might as well not exist.


Click to expand...

Almia said:

 the "returns nothing" actually makes sense if you are a lua programmer.


Click to expand...

Not in JASS though.

Almia said:

 I hate reading lua libs just to know if a function returns a value or no.


Click to expand...

Infact Lua functions do not declare a fixed return type, you can return any values (or none) from any return instruction.

Zwiebelchen said:

 The only thing I don't like is the "call" keyword; but I like having the "set" keyword.


Click to expand...

Zwiebelchen said:

 And yes, being forced to declare locals on top of a function is annoying, but it has it's merits when you try to fix reference leaks.


Click to expand...

Such redundancies and restrictions annoyed me too, hence why I had myself some precompiler then correcting it. This rendered the code more readable as well.

#### Emm-A-

#### Emm-A-

 Level 14

JoinedJul 1, 2008Messages1,314

Resources: 1

#### Resources
Spells 1

This is not meant to be non-constructive criticim, but I think, this is no tutorial.

This is a comment, not a tutorial, because it is mainly your own opinion. Its nice to point out advantages of doing something, but it is not a tutorial. There is some "from gui to jass" tutorials, and they explain very simple functions and how to convert triggers in to custom script and stuff, but here, you only provide your opinion on "why jass is better than gui".

I would also leave out stuff like "wc3 is dead", like previously mentioned, as this is not helpful at all for wc3.

Also, gui can actually be faster to write sometimes. E.g. I often use it for simple debug triggers (well only for debugging, so I somehow agree with you).

Anyway, in almost all parts of your comment, I agree with you but I think you should rather submit this in to another section.

It is still worth reading it.

#### Wietlol

#### Wietlol

 Level 24

JoinedAug 1, 2013Messages4,658

Resources: 8

#### Resources
Maps 1Spells and Systems 7

Apart from array size (when dealing with multi-dimensional arrays), no object oriented data structures, bad efficiency and no forward declaration, JASS is not really a bad language.

Yes, it is more text based than symbol based, but that is not necessarily bad.

It allows people that used other languages mostly to learn how this language works quite fast.

It is also very easy to learn how to use it when moving on from GUI.

Loops are also not bad. (Be aware you have to compare them to while loops and not for loops.)

Is there any other language that can give you a while (true) {} without having to write the condition that gets checked every iteration?

I am not really a fan of the Common.j api and create my own stuff whenever possible (not caring about the efficiency of calling 2 functions instead).

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Hahahahahaha

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

DracoL1ch said:

 will consider that


Click to expand...

Have you considered?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I am totally gonna make a guide why you should move from jass to gui because jass have some downsides, believe it or not

Such as finding the ids for lightning effects.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

IcemanBo said:

 Have you considered?


Click to expand...

nope, no time, mem hack more useful

Chaosy said:

 I am totally gonna make a guide why you should move from jass to gui because jass have some downsides, believe it or not

Such as finding the ids for lightning effects.


Click to expand...

if only there would be lightning IDs in google or ingame SLK tables. Oh wait

#### The_Silent

#### The_Silent

 Level 46

JoinedFeb 4, 2008Messages3,511

Resources: 204

#### Resources
Models 63Icons 115Packs 8Skins 16Maps 1Ported Models 1

When writing jass, one should use jass helper or another text editor, since the native te in wc3 is terrible. This should solve the issue for people who think it is hard/slow to read/write.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I don't have to do that in GUI. It's harder in jass.

#### The_Silent

#### The_Silent

 Level 46

JoinedFeb 4, 2008Messages3,511

Resources: 204

#### Resources
Models 63Icons 115Packs 8Skins 16Maps 1Ported Models 1

It comes with jnpg, which you should use anyway

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

DracoL1ch said:

 nope, no time


Click to expand...

We can wait some time, no rush. In case you really don't find time, maybe more than 1-2 months, then just post again no let us know.

But here are points to mention:

===========================

 fine

 might be improved / your attention needed

 improve it

===========================

Intro

--------

DracoL1ch said:

 Using GUI is like building a scyscraper with a team of retarded builders. They are known for their cheap prices and principles "what you see is what you get", but any of their approaches can't stand any chance in real world. Some of floors are rotated by 90 degrees, and, to compensate it, they added a small rotation over the whole building. Some of floors missing windows or doors, or even locked completely. Instead of proper foundation before building they prefer to drop a plate and start it right now. And, of course, you barely able to control any of those processes - you aren't pro to do so, right?

Pure JASS, instead, is a tool which creates only those agents you'd like to see or use. You playing Black & White here, declaring how should work this or that thing. You can go and create a custom design for one of the floors at any time. You can make sure building won't fall off using solid foundation of some pre-made big systems. As well nobody limits your ideas with a menu of possible actions. Well, except the game itself, but anyway you cannot touch the sky yet.


Click to expand...

JASS has for sure other limits than GUI has, but with almost only GUI you're also capable of doing quite solid systems.

Readability/maintainability and such is of course something difference. But you can make code some stuff (even not all) also properly with GUI.

-> Try to aovoid something like "Using GUI is like building a scyscraper with a team of retarded builders."

Local variables

--------------------

A short definition might be useful at first, what a local variable is.

And then you can do your rest with explaining such thing is provided only in JASS.

Faster writing

------------------

Used writers are usually faster, I agree.

GUI is harsh for CPU

--------------------------

I think if we compare GUI to JASS, it's valid to say JASS is CPU friendlier. It's fine.

User functions

-------------------

I might know what you meant the topic, but from the content it's not really clear imo what you wanna say exactly.

Limited options

--------------------

In the into index you named an example "z of lightings", and in the main paragraph nothing.

Make it other way around, or at least add some useful examples right there.

Moveability

----------------

Is this about moveability, or about "easier reading and understanding"?

It's indeed easier to understand internal mechanics in reading pure JASS, but please sort is out.

Moving on

----------------

It's actually fine, and I like it. Just the last part "Use proper Tools" -> "JNGP". It's actually only about the JNGP editor. (which is okay)

Read some basic JASS guides to understand principes of variables and

---------------------------------------------------------------------------------

Structure of this list seems a bit weird. And here you can add to tutorials or mpq editor.

Here you can find some links to jass relevant tutorials.

Sounds like hell

---------------------

"Possibility to improve Warcraft 3 community."

^Can be misinterpreted. Just call it "Better possibility" or something alike.

"I know no projects which has been written on GUI and turned out to be successful since 2010. There are examples before that date, but not since."

^That's not really true, and mostly your personal opinion. Just remove that, no discussion about this is asked for.

------------------------------------

You can have local variables

It's faster to write (once you get used to it)

The way GUI converts into jass contains alot of useless function-chain calls and double negative conditions

You can't write custom object-oriented code in GUI

There are certain natives available in Jass that are not available in GUI (i.e. setting the z of lightning objects)

You can easily transfer Jass code from one map to another (Just CnP) but you can't do this as easily with GUI

Click to expand...

 ^You can have them just shortly stated and you anyways explain them better in further texts.

Like "You can have local variables" -> "Local variables", etc.

I'm also not sure I understand your 4. point. In GUI you can mostly do the same "object orientated" work like with plain jass.

 You have 3 different lists over the thread which state some JASS benefits, which seems a bit random.

Probably just one complete one would be easier to follow. Like topic indices, and then you can explain in paragrahps.

There are also some typos and grammer errors.

====

I also would find it good to see these listed:

- transparency ( for example in GUI you don't see that group enum is an other function, which can cause confusion with locals)

- code structure (in jass you can put related code, also multiple triggers, into only one "trigger sheet")

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

I find there is good content that's just nice to debate about and show pros/cons, but there are some points (mentioned above) that would need your attention first before it will be approved.

It will stay open for changes and discussion, but I'm sending it to graveyard for now.

#### CampScouter

#### CampScouter

 Level 5

JoinedApr 13, 2017Messages157

Resources: 1

#### Resources
Tutorials 1

Lol. This is a funny thread to read.

 You must log in or register to reply here.


### Similar threads

 JASS in detail


thelifelessone

Aug 29, 2009

Tutorial Archive

Replies3Views2K

Sep 2, 2009
Dreadnought[dA]

 Transitioning from GUI to Lua/JASS


Ouroel

Jul 18, 2020

Tutorial Archive

Replies2Views4K

Oct 26, 2020
Chaosy

D

[JASS/AI]
 Simple JASS tutorial - for beginners


Deleted member 219079

May 5, 2014

Tutorial Archive

Replies36Views20K

Feb 11, 2017
IcemanBo

 [GvJ] GUIvJASS Basics


Overfrost

Feb 12, 2019

Tutorial Archive

Replies5Views2K

Dec 4, 2020
Chaosy

Poll

 [Tutorial] Modifying the World Editor's Trigger Editor


Sevion

Jan 2, 2008

Tutorial Archive

Replies10Views5K

Jan 29, 2008
PurplePoot

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