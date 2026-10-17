---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/create-a-sucessfull-timer-with-window-reuploaded.97651/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Create a Sucessfull Timer with Window Reuploaded
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/create-a-sucessfull-timer-with-window-reuploaded.97651/>
#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Create a sucessfull timer with window

#
 How to create a successfull timer

This tutorial will show you how to make a successfull timer with a proper window using GUI.

##
 Prologue

So the tutorial is done... not even close to what I hoped it would be since it's quite simple and short. Wish I could post a tutorial on a more usable topic since anyone engaged enough in world editing can do this by him/her -self.

If any admin see this, don't waste your time with a review. Even if it would pass the test the, as I said, topic isn't very interesting or useful.

Also, this is a reupload. I made this in August (I think at least) and by that time I didn't know double posting was forbidden.

Please, forgive my acts. I was just a child.

##
 Introduction

I've noticed when playing Battle.net games that many of the timer windows never disappear.

This may be caused by, for example, using

Timer 1
Events
Time - Timer[1] expires

Conditions

Actions
Time - Destroy (Last created timer window)

In this case the destruction of a Timer[1] refers to (Last created timer window) which can be another timer, meant for resurrecting a Hero for example.

It can get very anoying to have 5 timers that says 00:00:00 blocking the sight and disables vision of other timers...

I wish to help you with this

Timers can be used in any map for any reason. Most common cases are

- Using it to show when you get your next income

- Using it to show the amount of time there is left to finish a map.

- Using it to show when the game starts.

- Using it to show when your Hero resurrects.

- Using it to show when the next creep spawning level will begin.

##
 Dictionary

Or "words that may come in handy"

GUI Trigger

GUI Trigger
Events

Conditions

Actions

Variable

A variable is "something that can store somthing of it's kind"

(You should check out a tutorial on variables to get the idea of what they are)

If a variable was an integer, we called it "X" and it was equal to 3; X+2 would be the same as 3+2.

Or if the variable was a unit, we called it Super_Hero and it was equal to Blademaster 0001; we could use it in a map initialization:

Initialization
Events
Map initialization

Conditions

Actions
Hero - Set Super_Hero Hero-level to 10, Hide level up graphics

(If you don't get the idea follow my advice and visit a tutorial for variables)

Using [ ]

I use [ ] to represent variables and similiar stuff which is supposed to be read as one string. Very useful when reading lots of texts.

##
 Tutorial Start

###
 Part One

Creating Variables

(To create a variable, go to [Trigger Editor->Variables])

Create the following variables:

Timer

 Timer Array

 New Timer (Default)

TimerWindow

 Timer Window Array

 - None -

Explenations:

The first is the timer itself, which is not to be confused with the timer window.

The second is used to enable the destruction of the timer window.

###
 Part Two

Triggers

TheTimer Start
Events
Time - [Your event]

Conditions

Actions
Countdown Timer - Start Timer[1] as a One-shot timer that will expire in [YOUR TIME] seconds

Countdown Timer - Create a timer window for Timer[1] with the title [YOUR TITLE]

Countdown Timer - Show (Last created timer window)

Set TimerWindow[1] = (Last created timer window)

Now you're almost done. The only thing we need is to destroy the timer.

TheTimer Destroy
Events
Time - Timer[1] expires

Conditions

Actions
Time - Destroy TimerWindow[1]

As you can see, the action destroys TimerWindow[1] - which we previously set as the window for Timer[1] - and not (LastCreatedTimer) which could destroy any timer as long as it's the last created one.

Instead of creating 2 triggers for each timer you want to make which all say that "when this trigger runs out, destroy the window for it", you can combine this into the action you want to happen when it runs out.

Example of use:

Hero Revive 1
Events
Time - HeroReviveTimer[1] expires

Conditions

Actions
Time - Destroy HeroReviveTimerWindow[1]

Hero - Instantly revive Hero[1] at (Center of (Hero Revive <gen>)), Show revival graphics

##
 Credits

Thanks to Airandius for extreme review

It really helped me a lot.

Unfortunatly, since I changed the entire tutorial, you will have to do it again

If anyone catches any of his posts, please, give him +Rep.

##
 The Very End

May seem a little fair and simple, but since many map makers still don't know how to do this I felt responsible to create a tutorial.

It's mainly for the sake of destroying the right timer window and not destroying a timer that is still running...

If any of you wonder why I changed the whole tutorial from [really advanced hero siege creep leveling] to [simple timer window tutorial], it was just for that purpose: Make it a [simple timer window tutorial] and not go too much off-topic...

I think I'll make a tutorial later on [how-to create an extreme hero siege/tower defence creep-spawning system], which was my first idea of the tutorial

P.S: This was my very first tutorial.

Happy Map-making!


 Last edited: Oct 27, 2009

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

I can't belive I still havn't got any response...

#### Hawkwing

#### Hawkwing

 Level 34

JoinedNov 28, 2006Messages5,269

Resources: 29

#### Resources
Models 16Icons 3Packs 1Skins 8Tutorials 1

You must use the

#

##

###
 (header) tags, otherwise this wont get approved.

#### Airandius

#### Airandius

 Level 17

JoinedJun 12, 2007Messages1,261

Resources: 8

#### Resources
Icons 2Maps 1Ported Icons 4Tutorials 1

I'm not a mod, but in any case I will give you a full review of your tutorial.

(Better prepare yourself.)

It's good that you tell us what this tutorial is going to do, it will help us with timers.

But you made some (minor) mistakes in your introduction:

 This particular timer tutorial is mainly used for Hero Siege, possibly Tower Defence as well.

It will make the creeps spawn at the starting location of a player, if he/she/it is playing.


Click to expand...

This is disturbing me a, timers can be used in almost every map type, so give us more examples.

A few (basic ones) are:

- Using it to show when you get your next income

- Using it to show the amount of time there is left to finish a map.

- Using it to show when the game starts.

The main reason for you (rather low) score is because you told us wrong information.

 Variable

A variable origins from math. It's originally a holder for a number, like (X=2)

In this case, variable X is equal to 2, so when calculating 2+X it would be the same as 2+2


Click to expand...

This is incorrect, a variable is nothing more then something that can store something of it's type.

For example you are talking about an integer variable; it saves a number.

You also have real variables, they also save number but instead of 1, 2, 3 like integers they will store 1.00, 2.00, 3.00.

Some thins in the WE are saved in integers, while some are saved as reals.

Your second part about variables is correct however. Just say that variables can store anything you want for later use.

You can also use variables to store certain things (like points or groups) so you can destroy them later (to prevent leaks).

Also instead of using: 'Words that might come in handy:' call it: 'Basics:'.

#1: You use an integer to keep track of levels, however this tutorial was to show people how to proper create and remove timers from a game.

You should first tell them how to create a trigger, how they work, what they do and how to clean them properly.

I suggest you put the 'extra' part. So basically the example in the next section.

So you'll get two sections, a basic timer section and a section that gives you an example how to use it. In your case a defense.

#2: (Your example.) You don't have to create a new timer for each level, you can use a timer that keeps looping, and when that timer ends you create 'x' number on units of type 'y'.

For the units you can make an initialization trigger to store all units of each level to a variable array.

Then use an integer to increase the level.

Initialization
Events
Time - Elapsed game time is 0.01 seconds

Conditions

Actions
Set UnitTypeVar[1] = Peasant

Set UnitTypeVar[1] = Footman

Set UnitTypeVar[1] = Knight

Set UnitTypeVar[1] = Rifleman

Can you see what I mean? This is way better then to create 50 timers for example, instead of 50 triggers you only need 2.

One to set the levels, and one to 'increase the level' + 'spawning actions' when the timer ends.

Of course you should not destroy the timer window in this case, only at the last level.

#3: Your triggers leak, please clean them. If you do not know what I'm talking about then go here.

#1: I saw a link to a picture attachment, use image tags to show your images. It might be best if you hide it with a spoiler.

- Spoilers are used like this: [ HIDDEN="<insert title here>"]<insert hidden stuff here>[/HIDDEN ] of course remove the spaces inside the brackets.

- Images tags are used like this: [ IMG]<Image URL>[/IMG ] of course remove the space at the end.

Put the image tag inside the spoiler and you can show your images on your thread, people only have to click the show button.

#2: Give your titles a nice color to them, this way new parts of your tutorial are noticed better.

Just don't spam colors.

#3: This tutorial is very simple, so the overall usefulness of this trigger is not ensured.

Not to forget that timers and their windows are a very easy part of the editor, it's usually the 'new', 'bad', 'noob' mapmakers that leave these kind of small mistakes for what they are.

Forgive for saying noobs though, but you have to agree with me. There are a lot.

#4: Add a sample map? A map can say more then a 1000 words.

#5: Use header tags.

Everybody makes mistakes, but tutorials should not have them, or at least not a lot.

Here are a few of your simple mistakes, I could have missed some though, so read your whole trigger and check for spelling and grammar.

#1:

 This tutorial will show you how to make a succesfull timer window using GUI.


Click to expand...

- Should be: 'successful'.

#2:

 I have noticed when playing games online that many of the windows never dissapeared.

Click to expand...

- Should be: 'disappeared'.

#3:

 In this case it refers to (Last created timer window) which can be another timer, meant for ressurecting a hero for example, and it can get very troublesome...


Click to expand...

- Should be: 'resurrecting'.

#4:

 This particular timer tutorial is mainly used for Hero Siege, possibly Tower Defence as well.


Click to expand...

- Should be: 'defense'. (2x)

#5:

Theese triggers will make it easy for you to create more levels to you Tower Defence or Hero Siege.


Click to expand...

- Should be: 'these'. (2x)

#6:

 I use [ ] to represent variables and 'similiar stuff which is supposed to be read as one string. Very useful when reading lots of texts.


Click to expand...

- Should be: 'similar'.

#7:

Explenations:

The first variable is used, combined with the third, to tell the editor which timerwindow to destory later on.


Click to expand...

- Should be: 'explanations', 'timer window' (2x) and 'destroy' (2x).

#8:

 3: Change the actions wraped around comments:


Click to expand...

- Should be: 'wrapped'.

#9:

 If you examine the triggers you will see that almost everything is done automaticly

Click to expand...

- Should be: 'automatically'.

#10:

 [Timer_Window_Level[1]], beacuse it is supposed to destroy


Click to expand...

- Should be: 'because'.

#11:

 I do hope you will find the tutorial helpfull.

Click to expand...

- Should be: 'helpful'.

Final Score:

[5.5/10] + [3/10] + [3/5] + [4/10] = [15.5/35]

15.5 / 35 = 0.4428

0.4428 x 9 + 1 = 4.9852 (Out of 10.)

A tutorial will need a minimal score of 8/10, for this reason I vote against approving at this tutorial.

Fix what I told you and your score will go up for sure, your main problem is your main tutorial part followed by your spelling.

Good Luck

~ Airandius

My way of calculating for approval is not the way moderators will do it, I just made it as accurate as I thought I could, be aware that moderators only want good tutorials.

Also: I'm not a mod, I can't approve this for you even if it scored 10/10.


 Last edited: Dec 28, 2008

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

@Airandius: Tutorials have a score?

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Hawkwing said:

 You must use the

#

##

###
 (header) tags, otherwise this wont get approved.

Click to expand...

#

##

###


What the hell is that?

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Airandius said:

 I'm not a mod, but in any case I will give you a full review of your tutorial.


(Better prepare yourself.)


It's good that you tell us what this tutorial is going to do, it will help us with timers.


But you made some (minor) mistakes in your introduction:


This is disturbing me a, timers can be used in almost every map type, so give us more examples.


A few (basic ones) are:


- Using it to show when you get your next income


- Using it to show the amount of time there is left to finish a map.


- Using it to show when the game starts.


The main reason for you (rather low) score is because you told us wrong information.


This is incorrect, a variable is nothing more then something that can store something of it's type.


For example you are talking about an integer variable; it saves a number.


You also have real variables, they also save number but instead of 1, 2, 3 like integers they will store 1.00, 2.00, 3.00.


Some thins in the WE are saved in integers, while some are saved as reals.


Your second part about variables is correct however. Just say that variables can store anything you want for later use.


You can also use variables to store certain things (like points or groups) so you can destroy them later (to prevent leaks).


Also instead of using: 'Words that might come in handy:' call it: 'Basics:'.


#1: You use an integer to keep track of levels, however this tutorial was to show people how to proper create and remove timers from a game.


You should first tell them how to create a trigger, how they work, what they do and how to clean them properly.


I suggest you put the 'extra' part. So basically the example in the next section.


So you'll get two sections, a basic timer section and a section that gives you an example how to use it. In your case a defense.


#2: (Your example.) You don't have to create a new timer for each level, you can use a timer that keeps looping, and when that timer ends you create 'x' number on units of type 'y'.


For the units you can make an initialization trigger to store all units of each level to a variable array.


Then use an integer to increase the level.

Initialization
Events
Time - Elapsed game time is 0.01 seconds

Conditions

Actions
Set UnitTypeVar[1] = Peasant

Set UnitTypeVar[1] = Footman

Set UnitTypeVar[1] = Knight

Set UnitTypeVar[1] = Rifleman

Can you see what I mean? This is way better then to create 50 timers for example, instead of 50 triggers you only need 2.


One to set the levels, and one to 'increase the level' + 'spawning actions' when the timer ends.


Of course you should not destroy the timer window in this case, only at the last level.


#3: Your triggers leak, please clean them. If you do not know what I'm talking about then go here.


#1: I saw a link to a picture attachment, use image tags to show your images. It might be best if you hide it with a spoiler.


- Spoilers are used like this: [ HIDDEN="<insert title here>"]<insert hidden stuff here>[/HIDDEN ] of course remove the spaces inside the brackets.


- Images tags are used like this: [ IMG]<Image URL>[/IMG ] of course remove the space at the end.


Put the image tag inside the spoiler and you can show your images on your thread, people only have to click the show button.


#2: Give your titles a nice color to them, this way new parts of your tutorial are noticed better.


Just don't spam colors.


#3: This tutorial is very simple, so the overall usefulness of this trigger is not ensured.


Not to forget that timers and their windows are a very easy part of the editor, it's usually the 'new', 'bad', 'noob' mapmakers that leave these kind of small mistakes for what they are.


Forgive for saying noobs though, but you have to agree with me. There are a lot.


#4: Add a sample map? A map can say more then a 1000 words.


#5: Use header tags.


Everybody makes mistakes, but tutorials should not have them, or at least not a lot.


Here are a few of your simple mistakes, I could have missed some though, so read your whole trigger and check for spelling and grammar.


#1:


- Should be: 'successful'.


#2:


- Should be: 'disappeared'.


#3:


- Should be: 'resurrecting'.


#4:


- Should be: 'defense'. (2x)


#5:


- Should be: 'these'. (2x)


#6:


- Should be: 'similar'.


#7:


- Should be: 'explanations', 'timer window' (2x) and 'destroy' (2x).


#8:


- Should be: 'wrapped'.


#9:


- Should be: 'automatically'.


#10:


- Should be: 'because'.


#11:


- Should be: 'helpful'.

Final Score:


[5.5/10] + [3/10] + [3/5] + [4/10] = [15.5/35]


15.5 / 35 = 0.4428


0.4428 x 9 + 1 = 4.9852 (Out of 10.)


A tutorial will need a minimal score of 8/10, for this reason I vote against approving at this tutorial.


Fix what I told you and your score will go up for sure, your main problem is your main tutorial part followed by your spelling.


Good Luck


~ Airandius

My way of calculating for approval is not the way moderators will do it, I just made it as accurate as I thought I could, be aware that moderators only want good tutorials.


Also: I'm not a mod, I can't approve this for you even if it scored 10/10.

Click to expand...


Haha great feedback I'll work on this tomorrow if I get the time


+Rep to you


I will also put you on teh credits list if I remember

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Cheezeman said:

 What the hell is that?


Click to expand...

They're tags you should use for your headers.

Also, merge your posts. Double posting is against the rules.

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Pyritie said:

 They're tags you should use for your headers.


Also, merge your posts. Double posting is against the rules.


Click to expand...


And what are headers? xD (Seriously I don't know squat about this site)


And, yes, I will remove the first post. The first post was to get people to notice my tutorial. At that time I didn't know I could just Edit my post and post it again.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Cheezeman said:

 And what are headers? xD (Seriously I don't know squat about this site)

And, yes, I will remove the first post. The first post was to get people to notice my tutorial. At that time I didn't know I could just Edit my post and post it again.


Click to expand...

#
 h1

##
 h2

###
 h3

Use them for titles for sections and stuff.

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Pyritie said:

#
 h1

##
 h2

###
 h3


Use them for titles for sections and stuff.


Click to expand...


I see... As I said before, I'll get to work on it tomorrow if I find the time for it.

#### Airandius

#### Airandius

 Level 17

JoinedJun 12, 2007Messages1,261

Resources: 8

#### Resources
Icons 2Maps 1Ported Icons 4Tutorials 1

Pyritie said:

 @Airandius: Tutorials have a score?


Click to expand...


Nope, but it's fun to give scores so that the maker knows how good/bad his was.


Lol, do I see a new smilie?


Anyway, the tutorial should be fixed a bit, good luck with it. ^^

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Airandius said:

 Nope, but it's fun to give scores so that the maker knows how good/bad his was.


Lol, do I see a new smilie?


Anyway, the tutorial should be fixed a bit, good luck with it. ^^


Click to expand...


Haha thanks for the luck.


You're welcome any time to give this tutorial a score, it's just great feedback


Now I'll get started with fixing all my faults and 'typos'

#### Ash

#### Ash

 Level 22

JoinedAug 27, 2005Messages1,684

Resources: 1

#### Resources
Tutorials 1


 that guy said:



 You should note that this tutorial isn't finished yet. I wish to add actions which cleans up leaks and make my grammar perfect.


Click to expand...

You get kudos for that, but that's about it.

#### Staff SgT Legacy

#### Staff SgT Legacy

 Level 2

JoinedFeb 10, 2009Messages17

i mdidnt get the very idea of hero respawning


i used ur way but still it wont works


can som1 help?

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Staff SgT Legacy said:

 i mdidnt get the very idea of hero respawning


i used ur way but still it wont works


can som1 help?


Click to expand...

I'm always here to help.


But perhaps you should start a topic on it in World Editor Help Zone instead since Hero Respawning isn't really related to the destruction of timer windows, right?

#### Fisher Panda

F

#### Fisher Panda

Rep+

Rep+

#### Xx Rome xX

#### Xx Rome xX

 Level 3

JoinedSep 26, 2009Messages53

You missed a typo! At the bottom it says extream - should be extreme.

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Lol old thread.

Thanks, it was written a while back and my English - though it's not perfect yet - has been improved since then.

Topic should be closed.

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

Well im a noobie guy so, please! tell me, what is the array number i should have?


please tell me hoho

Cheezeman said:

 Lol old thread.


Thanks, it was written a while back and my English - though it's not perfect yet - has been improved since then.


Topic should be closed.


Click to expand...

please tell me the arrays, i dunno much, still 2 months of making maps, thought i created 2 maps! i dunno how to use variables to much plzz tel meeee


 Last edited by a moderator: Feb 7, 2010

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

Array is like... a variable with a number.

Instead of making 3 variables (say integers) named Int1 Int2 Int3, you make 1 integer array and then use Int[1] Int[2] Int[3].

what is the array number i should have?

Just leave this blank, it has little affect on anything.

Give me more specific questions that I might help you.

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

Cheezeman said:

 Array is like... a variable with a number.


Instead of making 3 variables (say integers) named Int1 Int2 Int3, you make 1 integer array and then use Int[1] Int[2] Int[3].

what is the array number i should have?


Just leave this blank, it has little affect on anything.


Give me more specific questions that I might help you.


Click to expand...


 i still dun get it, with numbers, player groups are in variable, and somthing in , ack, just TEll me how to use variableee!!


Thankyou very much

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

I can't just TEll you how to use the variableee, because I don't understand your problem.

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

Cheezeman said:

 I can't just TEll you how to use the variableee, because I don't understand your problem.


Click to expand...


My prob is that i cannot use such like, if there are 8 elfs, 8 of them killed by the hunter, and i want all of the lose using one trigger,( the losing one equals the elfs)


but if i didnot use any variables then if there are no player playing in the Hunter or the Elfs i dunno what to do!


Thankyou juz tell me what to do

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

I'm sorry to tell you but that's unrelated to my tutorial. Go make a thread in World Editor Help Zone instead.

#### Etzer

#### Etzer

 Level 13

JoinedFeb 18, 2009Messages1,378

THX! Guess what... rep+

#### Shaki

#### Shaki

 Level 3

JoinedJan 20, 2009Messages24

Resources: 1

#### Resources
Maps 1

I used arrays for each hero respawn timer, like one hero died it would be Timer[1], blue would have Timer[2] but for some reason the timers dont have a count down and do not disappear, only the first one.

EDIT: Stupid me, i forgot to increase the array to timer[10] from timer[1] size


 Last edited: Mar 19, 2010

#### impulse

#### impulse

 Level 3

JoinedJan 6, 2021Messages24

old post i know but what the f is Time- [your event] ?? like all you have to pick on is time elapsed, time periodic event and time expires.

should it be a variable there?

or what should i pick in event?

 You must log in or register to reply here.


### Similar threads

 Upwards Counting Timer


Laserdude

Aug 10, 2010

Trigger (GUI) Editor Tutorials

Replies4Views12K

Jul 15, 2021
MacMardigan

 How to create a sucessfull timer.


Cheezeman

Sep 4, 2008

Tutorial Archive

Replies0Views2K

Sep 4, 2008
Cheezeman

 Creating Levels in a Defense Map


eubz

Mar 15, 2014

Trigger (GUI) Editor Tutorials

Replies9Views6K

Apr 5, 2014
Daffa

Locked

[Trigger]
 Death timer can i use the same as i already have (Variable)


Shanghai

Jan 1, 2014

Triggers & Scripts

Replies19Views3K

Jan 5, 2014
Lender

Locked

 Countdown timer doesn't show the time.


Greenwhy

Mar 7, 2014

World Editor Help Zone

Replies4Views2K

Mar 8, 2014
WaterKnight

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Trigger (GUI) Editor Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top