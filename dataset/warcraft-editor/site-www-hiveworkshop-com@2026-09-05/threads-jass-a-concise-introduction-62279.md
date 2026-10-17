---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/jass-a-concise-introduction.62279/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# JASS a Concise Introduction
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/jass-a-concise-introduction.62279/>
#### Ghan_04

#### Ghan_04

 Level 22

JoinedFeb 26, 2008Messages893

Resources: 2

#### Resources
Tutorials 2

I don't know if you already have enough JASS tutorials, but I have one that is geared more toward the people who haven't gotten anything up to this point and are looking for a different take on the subject. Anyway, here it is:

A Concise Introduction to JASS

By: Ghan_04

Introduction


You all have heard of it; it’s JASS, the trigger language of WC3. And, if you think you don’t know it, you’re wrong. If you can use GUI, you can just as easily use JASS. Now, in this tutorial, I’m not going to use 3 pages of advanced JASS code script. I’m going to focus on HOW you should write JASS. What are some of the syntax rules? What do some actions look like in JASS? So, let’s get started….


First, create a trigger (or just look at mine), don’t add anything to it, and convert it to JASS by clicking on it and going to Edit -> Convert to Custom Text. Mine is named Blank JASS Trigger. It should look like this:

 JASS:

function Trig_Blank_JASS_Trigger_Actions takes nothing returns nothing
endfunction

//===========================================================================
function InitTrig_Blank_JASS_Trigger takes nothing returns nothing
 set gg_trg_Blank_JASS_Trigger = CreateTrigger( )
 call TriggerAddAction( gg_trg_Blank_JASS_Trigger, function Trig_Blank_JASS_Trigger_Actions )
endfunction


This is the basic structure of everything you will do when creating JASS triggers. Now, I’m going to give a brief explanation of what each thing is doing, but first, we need to talk about functions.

Functions and Triggers


Functions are, in the simplest of terms, an action or group of actions. That’s it. No frills involved here. Now, there are 4 types of functions, depending on the arguments they give or return. The function can neither give nor return arguments, it can return but not take arguments, it can take but not return arguments, or it can both take and return arguments.


But, you’re asking, “What does that mean!” Functions that do neither just do the actions inside the function. Pretty simple. Whatever actions that are in the function are executed. If the function takes something, that means you have to give it something to make the function work. Maybe you have to give it an integer so it can do a calculation. If the function returns a value, that means that when the function is done, it gives you something. You may or may not want to use the thing it gives you. But, to have this make more sense, let’s talk about functions that both give and return a value.


Think of a machine where you put something in, the machine does something to it, and something new is spit out the other side. So it is in JASS. You give the function something, it runs it through the actions, then gives something back to you that could be totally different.


Now we can go back to the trigger above. Look at this line:

 JASS:

function Trig_Blank_JASS_Trigger_Actions takes nothing returns nothing
endfunction

All this does is yell at the game, “Hey! We got some actions here that don’t need anything and don’t give anything. Deal with it!” Then you can put actions below the function Trig_Blank_JASS… blah blah, and above the endfunction. If the function is ending, you don’t want to be putting things after it, right? Easy enough so far? Definitely.


Look at this now:

 JASS:

//===========================================================================
function InitTrig_Blank_JASS_Trigger takes nothing returns nothing
 set gg_trg_Blank_JASS_Trigger = CreateTrigger( )
 call TriggerAddAction( gg_trg_Blank_JASS_Trigger, function Trig_Blank_JASS_Trigger_Actions )
endfunction

I know it’s a big chunk, but we’ll get through it. First, this whole section is what sets up the trigger. It basically says, “We’ve got a trigger here, and here’s what sets it off, what we need to check and what it does.” As you can see, this is a function in itself. That will pretty much stay the same. It’s what is inside the function that counts. Now, take a look at this:

 JASS:

set gg_trg_Blank_JASS_Trigger = CreateTrigger( )


This is what creates the trigger when the map loads up. It tells the game to store another trigger for use. See the name of the trigger here: Blank_JASS_Trigger?

Actions


Now look here:

 JASS:

call TriggerAddAction( gg_trg_Blank_JASS_Trigger, function Trig_Blank_JASS_Trigger_Actions )


This is what will be run first when the event of the trigger is met. Notice how it says the name of the trigger with this line:

 JASS:

gg_trg_Blank_JASS_Trigger


Now check this out:

 JASS:

function Trig_Blank_JASS_Trigger_Actions


It’s the name of the function we looked at above! Basically, this is saying, “Run the function named Trig_Blank_JASS_Trigger_Actions.” Want more? Ok, here we go:


Any action must have the word call in front of it. Let's look at a simple action. How about killing a unit? This is what it looks like:

 JASS:

call KillUnit(GetTriggerUnit())


What is this? Well, like I said, it has to have a call. See it? Good. So, what's the rest? Well, KillUnit is the name (in JASS, obviously) of the action. In this case, killing a unit. All you have to do is put which unit is being killed in the parentheses. In this case, we're killing the Triggering Unit. See the GetTriggerUnit()? Simple, right? It's not that hard, right? Of course not. You worried for nothing.

Events


Now, I’m not going to bore you by trying to explain all of the different events and what they look like in JASS. Say I wanted to use the event A unit Dies. If you did that in JASS, it would look like this:

 JASS:

call TriggerRegisterAnyUnitEventBJ( gg_trg_Blank_JASS_Trigger, EVENT_PLAYER_UNIT_DEATH )


But where does this go? It goes below the line that creates the trigger like this:

 JASS:

set gg_trg_Blank_JASS_Trigger = CreateTrigger( )
call TriggerRegisterAnyUnitEventBJ( gg_trg_Blank_JASS_Trigger, EVENT_PLAYER_UNIT_DEATH )


Look here:

 JASS:

TriggerRegisterAnyUnitEventBJ


This is the part in GUI where you would see Generic Unit Event. Get it?


And this:

 JASS:

gg_trg_Blank_JASS_Trigger


This tells the game which trigger gets this event. See the trigger name in there?


Lastly, this:

 JASS:

EVENT_PLAYER_UNIT_DEATH


This is the actual event, which in this case is A unit Dies.


See, it really is too easy.


To find out what events look like, experiment with changing triggers into JASS. Also, JASSCraft is a wonderful source.

Conditions


Conditions look similar. Let’s look at what the condition Player 1 (Red) slot status equal to Is Playing would look like in JASS:

 JASS:

call TriggerAddCondition( gg_trg_Blank_JASS_Trigger, Condition( function Trig_Blank_JASS_Trigger_Conditions ) )


Now, conditions are a little weird. If you create a condition in GUI and convert it to JASS, you will find that it creates its own function just for the condition. This is what the separate function looks like:

 JASS:

function Trig_Blank_JASS_Trigger_Conditions takes nothing returns boolean
 if ( not ( GetPlayerSlotState(Player(0)) == PLAYER_SLOT_STATE_PLAYING ) ) then
 return false
 endif
 return true
endfunction


Notice it returns a boolean. The trigger wants to know yes or no in answer to the slot status comparison, so it returns either true or false. So, a condition operates almost the same way as an action. It runs a function. But in this case, the function either approves or denys the rest of the trigger. It’s like the guard of the gate. In other words, this function either stops the trigger or lets it continue. Now, this part goes at the bottom with the event:

 JASS:

call TriggerAddCondition( gg_trg_Blank_JASS_Trigger, Condition( function Trig_Blank_JASS_Trigger_Conditions ) )


It goes in between the Events and Actions, so in all, it should look like this:

 JASS:

set gg_trg_Blank_JASS_Trigger = CreateTrigger( )
call TriggerRegisterAnyUnitEventBJ( gg_trg_Blank_JASS_Trigger, EVENT_PLAYER_UNIT_DEATH )
call TriggerAddCondition( gg_trg_Blank_JASS_Trigger, Condition( function Trig_Blank_JASS_Trigger_Conditions ) )
call TriggerAddAction( gg_trg_Blank_JASS_Trigger, function Trig_Blank_JASS_Trigger_Actions )


Just as in GUI, it goes Event, Condition, Action. See them all? Good.


You can have functions that don’t show up at the bottom. Functions can call other functions. Call? What does call mean? Call means that the action or function will be executed. If I said:

 JASS:

call Trig_Blank_JASS_Trigger_Actions()


I just told it to do that function just like it does at the bottom. When you use call in JASS to call something, it must be in all lower-case.


See the parentheses? That’s where you would put any arguments that the function would take. Since that function takes no arguments, it’s blank. Logical? Certainly, Mr. Spock.


So, how do you write actions in JASS? What if I wanted to kill a unit? Look it up. Just know that every action will have a call in front of it. Create the action in GUI and convert the trigger to JASS. It will show you.


Are you ready to move on to a little bit more advanced material? Come on, the first part was so simple, you won’t have to use but maybe 5 more brain cells to understand this. Ready? Ok, first, we’ll start off by talking about Ifs....

Ifs, Loops, and Returns

Ifs


Pretty simple, right? If something is true, do something. Otherwise, do something else. How do you do it in JASS? Look at this:

 JASS:

if udg_BooleanVariable == true then
 call DisplayTextToForce(GetPlayersAll(), “Hi!”)
else
 call DisplayTextToForce(GetPlayersAll(), “Go Away!”)
endif


That’s it. Don’t worry about the calls right now, they are actions converted from GUI, but you can look them up if you want. See the if? That’s where you put your condition. Seriously. See the “then” at the end of the line? That tells the game “Hey, we’re done with conditions.” Next line is the action. What do you think the “else” does? How about the endif? Tells the game that the if is done. That was unexpected. You can also use an elseif in place of the else. Ever used ifs inside of other ifs? There you go. All an elseif does is say, "If the condition isn't true, go down here and do another condition comparison." Easy? Good. Now, let’s take a look at loops.

Loops


So, loops. Wonderful tools, loops are. They repeat a set of actions for a pre-determined number of ‘loops’. Complicated? No. How about in JASS? Still no. Look:

 JASS:

set udg_IntegerVariable = 1
loop
 exitwhen udg_IntegerVariable > 5
 call DisplayTextToForce(GetPlayersAll(), “Loop Number: “+I2S(udg_IntegerVariable) )
 set udg_IntegerVariable = udg_IntegerVariable + 1
endloop


So. Let’s look at this. See the udg_ part? That means that IntegerVariable was a variable created in the Variable Editor. For the curious, it stands for User Defined Global. That set action sets the IntegerVariable to 1 so it can be used to set the number of loops that will occur. Exitwhen tells the loop when to stop. It stops when the condition is met. What’s the condtion? The variable is greater than 5. We set it at 1 before the loop. We increase it by 1 in the loop itself. How many times will the loop repeat? 5. Greater than 5 would be 6, but it starts at 1, so 6 – 1 = 5. Endloop? Tells the game there are no more actions in that loop. So, with loops, you better have an exitwhen condition, or you’ll be in trouble. Oh, and for reference, the maximum number of loops before the trigger collapses is approximately 6250. Just so you know. Now, onward and upward to returns….

 Returns


Ever used the Skip Remaining Actions option in GUI? Of course you have. That’s what return does. I’m serious! Wherever you put a return, that’s where the trigger will stop execution. Now, that’s not all, though (boos). If the function is supposed to return something (remember functions above?), that’s where you put it. Take a look:

 JASS:

function notreallyafunction takes nothing returns nothing
 return
endfunction


This does absolutely nothing. Take a look when it returns something, though:

 JASS:

function notreallyafunction takes nothing returns integer
 return udg_IntegerVariable
endfunction


Ooh, look, now when you call that function, it gives you the variable IntegerVariable. Difficult? Don’t make me laugh at you.  One more thing to look at, and we’re done. What? Already? Yes.

 The Golden Rule of JASS


Now, JASS has one big rule. There are others, (really?) but this one is one of the most important. Listen carefully. JASS IS CASE-SENSITIVE. So, Call is not the same as call. When you look it up in JASSCraft, get it right! Now, this is only the surface of JASS. But, armed with this knowledge, you can now sally forth to the wild annals of harder, more in-depth tutorials. (What? (Look it up.)) Remember, JASS is just a different way of displaying GUI. Well, that’s about it. Did you have fun? I certainly did. Now, go read a more difficult tutorial and finally understand what is being said!


Feedback welcome. I want to hear all your comments, including, "This tutorial is stupid; you should never have written it." Was this tutorial too easy for you? Go read a harder one. Was it too hard? Well, I'm open to questions!

#### Ghan_04

#### Ghan_04

 Level 22

JoinedFeb 26, 2008Messages893

Resources: 2

#### Resources
Tutorials 2

Bump


I can't decide if having no replies is good or bad....

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I don't think wyrmlord goes on much so I guess Poot is the main man for this. There is Earth Fury and some others as well though...

Anywho, it looks nice to me as I probably said before on TH.

#### Kamikazzee

#### Kamikazzee

 Level 6

JoinedJan 15, 2005Messages188

Resources: 1

#### Resources
Maps 1

I'll read it tonight and see what I can learn from this. I'll tell you what I think tomorrow. Thus far, it looks good in my eyes, however.

Next Day:

Okay, here's my scores:

(1 = Horrible ; 10 = Perfect)

Organization: 9

Usefulness: 10

Understandability: 10

Overall: 9.5/10

I'm sure this is the easiest, possibly most well-written intro to JASS tutorial I've found. It's actually taught me how to do JASS explaining how it relates to GUI instead of using other JASS which I can't yet understand, then explaining it to me in ways attempting to get to make you understand what they're trying to say, and not the subject itself.

Overall, possibly the best JASS tutorial I've read.


 Last edited: Mar 4, 2008

#### Ghan_04

#### Ghan_04

 Level 22

JoinedFeb 26, 2008Messages893

Resources: 2

#### Resources
Tutorials 2

Thanks for the comments and ratings.

That is exactly what I was trying to do with this tutorial.

I don't go very far into the subject, but I spend extra time on the extreme basics, so I hope can help those who are completely confused and probably have never used a programming language before.

#### Dil999

#### Dil999

 Level 7

JoinedJun 10, 2007Messages225

Resources: 2

#### Resources
Maps 2

Nice, but how about including a section about filters. (Maybe put it inside the Conditions section)

#### Ghan_04

#### Ghan_04

 Level 22

JoinedFeb 26, 2008Messages893

Resources: 2

#### Resources
Tutorials 2

> how about including a section about filters.


Hmm... well, I have to stop at some point.  I was thinking about making a sequel to this, and putting things like that in there.


I think it might be a bit overwhelming to put it here.


Notice I didn't really go in depth about having multiple functions in a trigger, and filters do exactly that.

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

 Remember, JASS is just a different way of displaying GUI.


Click to expand...


No. It's not. GUI is another way of displaying JASS. And it doesn't cover all of it.


Anyways, good tutorial.

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

GUI is nothing like JASS.

Anyway, I approve.

You could expand it a little more. But if you're planning on writing a sequal I'll just wait for that one to come.

#### SmartGood

S

#### SmartGood

I have no idea after all this reading ...

#### meercat

#### meercat

 Level 5

JoinedJan 27, 2007Messages154

SmartGood said:

 I have no idea after all this reading ...


Click to expand...


This may not be the best of tutorial for JASS beginners but this one is good...


especially the part "Difficult? Don’t make me laugh at you.  One more thing to look at, and we’re done. What? Already? Yes."


It took me about 10 times to read it before my mind fully understood what it really means LoL... or if blakabam, I just thought i really do understand it.... hahahahahahahahahaahhaahhaahah!!!


niweiz.. I envy those who make tutorials, wait for me and i'll contribute in the near future heheheehhe...

#### Sergeant Ray

#### Sergeant Ray

 Level 8

JoinedAug 12, 2008Messages417

It might be worth mentioning, Ghan, that:

=- Equals

==- Equal to

>=- Greater then or equal to

<=- Less then or equal to

!=- Not equal to


Other then that, great starter tutorial. 10/10

#### DawnOfMe

D

#### DawnOfMe

good tutorial thx lol

#### saphiree

#### saphiree

 Level 13

JoinedMar 23, 2008Messages1,213

Resources: 1

#### Resources
Maps 1

This is really good. Ive read many JASS tutorials and never understood anything, but this... this is SO good. Ofcause i will need to read it again, but this time i actually understood alot. I have only read till "ifs, loops and returns". But its GREAT so far

I think im finnaly on my way out of GUI and on my way into JASS

Also. How can it be your text has colors many places? When i convert into JASS everything is just written with black text.


 Last edited: Apr 13, 2009

#### Sergeant Ray

#### Sergeant Ray

 Level 8

JoinedAug 12, 2008Messages417

It's a special tag we have. The world editor doesn't use it unless you upgrade to WE helper. You can also find that effect in JASS text editors.

#### saphiree

#### saphiree

 Level 13

JoinedMar 23, 2008Messages1,213

Resources: 1

#### Resources
Maps 1

yeah ive seen it in JASS craft


Another question: you wrote this

 JASS:

call KillUnit(GetTriggerUnit())

But in WE there are spaces so it looks like this

 JASS:

call KillUnit( GetTriggerUnit() )

Does this affect the writing on anyway? i mean, making the trigger unfunctional?


And where can i get this WE helper?

#### Sergeant Ray

#### Sergeant Ray

 Level 8

JoinedAug 12, 2008Messages417

There is no difference.

I'm to lazy to get you a link. It's on wc3 campaigns, I'll tell you that much.

#### saphiree

#### saphiree

 Level 13

JoinedMar 23, 2008Messages1,213

Resources: 1

#### Resources
Maps 1

plz make a link soon someone. As i actually have more questions that pops up all the time. And i guess you would get annoyed by keeping answering all the time ^^

#### Sergeant Ray

#### Sergeant Ray

 Level 8

JoinedAug 12, 2008Messages417

Vexorians Jass Helper - Warcraft III Campaigns


PM me if you have any questions.

#### Foopad

#### Foopad

 Level 6

JoinedJun 15, 2008Messages175

Resources: 3

#### Resources
Maps 1Spells 2

I'd agree with Saphiree, this tutorial really lays it down, instead of explaining jass functions and names, with other words of advanced english or jass-speech, it's all explained in simple words of GUI

I too think that I might be on my way to jass (finally) after reading this tutorial.

It is great! Thanks Ghan_04! +rep

10/10

 You must log in or register to reply here.


### Similar threads

 JASS: A Better Understanding


Ghan_04

May 31, 2008

Scripting Tutorials

Replies15Views10K

Jun 20, 2009
Foopad

 JASS Tutorial


Daelin

Mar 20, 2005

Scripting Tutorials

Replies24Views38K

Nov 4, 2006
Daelin

 Jass, or "What the func?"


GhostWolf

Jul 21, 2013

Scripting Tutorials

Replies9Views9K

Oct 1, 2013
deathismyfriend

 JASS: Moving From GUI to Jass, the Start


Rheias

Sep 26, 2007

Scripting Tutorials
2

Replies61Views32K

Jul 22, 2008
Vercas

 Beginning JASS Tutorial Series


wyrmlord

Feb 16, 2007

Scripting Tutorials
234

Replies181Views124K

Feb 18, 2025
Eastern Programmer yahoo

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top