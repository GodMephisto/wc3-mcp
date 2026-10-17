---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/jass-moving-from-gui-to-jass-the-start.40617/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# JASS Moving from GUI to JASS the Start
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/jass-moving-from-gui-to-jass-the-start.40617/>
#### Rheias

#### Rheias

 Level 5

JoinedSep 13, 2007Messages33

Resources: 1

#### Resources
Tutorials 1

 Moving From GUI to Jass, the Start

Original Post

 0. Table of Content

0.1 Content

0.1.1 Introduction  - An introduction to the tutorial and to the basics of Jass. I will explain here about the tutorial, what is it's goal and how I'm attempting to achieve it. Also a brief explanation of what Jass is and why it is better then GUI.

1.1 Introduction to the Tutorial.

1.1.1 Basic Review

1.1.2 The Tutorial’s Organization

1.1.3 Tutorial’s Explanation Process

1.1.4 Tutorial’s Requirements

1.1.5 How to read this Tutorial

1.1.6 Tags

1.1.6.1 Why use tags?

1.1.6.2 When will I use each Tag?

1.1.7 Updates

1.1.8 Final Notes

1.2 Introduction to Jass

1.2.1 What is Jass?

1.2.2 Why use Jass?

1.2.3 Writing Jass

1.2.4 Jass’ System

1.3 Conclusions

 1.3.1 The Tutorial

1.3.2 Jass

1.3.3 Final words

0.1.2 Functions  - Introduction to functions. What are they? How are they used and created? What are user-designed functions and native functions? What is the difference? Parameters? And more.

2.1 User-Defined function

 2.1.1 Functions and GUI

2.1.2 Functions’ Goals

2.1.3 Creating a Function

2.1.3.1 Parameters’ Syntax

2.1.4 Functions as Blocks

2.1.5 Returning

2.2 Blizzard’s Functions

2.2.1 Blizzard Functions and GUI

2.2 Blizzard Functions and User Functions

2.2.3 Calling Functions

2.3 Conclusions

2.3.1 Conclude User-Defined Functions

2.3.2 Conclude Blizzard Functions

2.3.3 What next

2.3.4 Again, Calling

2.3.5 Functions in Math

0.1.3 Variables - A whole part dedicated to variables, how to use them, when, why. Globals, locals, comparisons, and setting them to new values.

 3.1 Introduction to Variables

3.1.1 Variables in GUI

3.1.2 Variables in Jass

3.1.3 Warning

 3.2 Global Variables

3.2.1 Using Global Variables

3.2.2 Globals in Jass

.2.3 Locals and Globals

 3.3 Local Variable

 3.3.1 Introduction to locals

3.3.2 Creating Locals

3.3.3 Setting Values

3.3.4 Comparisons

 3.4 Conclude Variables

3.4.1 Globals and Locals

3.4.2 Variables in Jass

0.1.4 Special Actions - This chapter will talk about the special actions which are If / Then / Else and Loop orders. Those are two important actions, read about them in this chapter!

4.1 Introduction to Special Actions

 4.1.1 What are Special Actions

4.1.2 The Dangers

4.1.3 Multiple Use

 4.2 If / Then / Else

 4.2.1 Introduction to If / Then / Else

4.2.2 If / Then / Else’s Syntax

4.2.3 Examples

4.2.4 Elseif

4.2.5 Uses of If / Then / Else

4.3 Loops

4.3.1 Introductions to Loops

4.3.2 Loops’ Syntax

4.3.3 Normal Loop

4.3.4 Other Loops

 4.4 Special Actions Conclusions

 4.4.1 Special Actions

4.4.2 Conclude If / Then / Else

4.4.3 Conclude Loops

0.1.5 What's Next? - Suggesting on how to keep learning Jass after this tutorial. Short part but does what it needs.

1. Introduction

Here you will find an introduction to the tutorial itself as a written script and to Jass. This is the least important part however I suggest reading it to make sure we are all on the same level before going into the tutorial itself. Skipping over this chapter could cause you to be confused later on. I HIGHLY suggest that you read the other tutorial I wrote, as it can ease your reading through this tutorial greatly, reading it first would make the learning path much shorter and easier to cross. Before Jass

 1.1 Introduction to the Tutorial

 1.1.1 Basic Review

This tutorial came to attempt to teach the basics of Jass. Indeed there are already tutorials on this, however in this one I’ll try to explain what other tutorials seemed to be a bit unclear. I’ll take other points of view in explaining and do my best to get you started with the basics of Jass.

 1.1.2 The Tutorial’s Organization

The tutorial is divided into parts, chapters, sub-chapters, notes / lists & additions. The left integer presents the part’s number, then a dot and then the chapter’s number, after that the sub-chapter's number. Notes / lists & additions do not appear as a number.

Long additions may be expanded into sub-sub-chapters. If so, their number will be included.

Parts would be bolded and underlined (B and U) chapters bolded (B), sub-chapters underlined (U) and additions Italic (I).

 1.1.3 Tutorial’s Explanation Process

Well, basically the way I will attempt to teach Jass in this tutorial is by constant reference to GUI. Each subject will be learned deeply and each subject will be reviewed and compared to a similar function in GUI. This way, the GUIers of you could read this tutorial more easily. This is what Daelin partly attempted to do in his tutorial, but I’ll take it one step forward and make the whole tutorial (or most of it) something sort of like Jass vs GUI.

 Note: GUI is Graphical User Interface. Usually when working with triggers you are dealing with either Jass or GUI. Jass is the script language, while GUI uses charts to ease the working process with Jass (more of this later).

 1.1.4 Tutorial’s Requirements

Well, there are none really I guess, you should have at the very least basic knowledge of GUI, no need for more than that. The tutorial is in English, so obviously you need to know how to speak English well. Oh, and a common sense wouldn’t hurt, I would hate answering questions such as, “Why is the tutorial so darn long?”

 1.1.5 How to read this Tutorial

There is not doubt this is a long, tiring tutorial (believe me writing it was hard as well ;-) ) this is why I organized it into so many parts, chapters and sub-chapters, you can easily stop somewhere, take a rest, and then continue from the same place. All you’ll need to do is to look at the chapter’s headers to remember what was discussed already and you are good to go.

I do not recommend reading this whole tutorial at once, it is long, hard to read and generally tough; take few rests between the chapters, that way you could let the information “sink into your brain” better.

Also I suggest reading everything, skipping on something might make it harder for you to understand the rest of the tutorial. If you really don’t want to read something, because you don’t think it is important enough or something like that, at least read the conclusion at the end of every part.

 1.1.6 Tags

While reading this tutorial you will meet 2 type of tags, code tags and Jass tags, both will be used in order to represent codes and functions.

 JASS:

Hello, I’m a Jass tag, how are you?

 Code:

Look, I’m a talking code tag!

 1.1.6.1 Why use tags?

Tags allow me to represent the tutorial in a clearer way for you to read. First it distinguishes between a code and the tutorial which will usually include explanation. Secondly tags allow me to write long lines without expanding the page’s width, so you will not have to keep scrolling to the sides. Example:

 Code:

 This is a really long line. Am I talking about anything here? No this is just an example, isn’t it? Well let’s just see what happens when this gets really long… Ok this is long enough!

Finally, when using tags I can put spaces before the line, this will make the presentation clearer.

 JASS:

 So many spaces. Wow!
 And few more.
 You got the idea I hope?

 1.1.6.2 When will I use each Tag?

I will use Jass tags to represent most of the functions and Jass scripts in the tutorial. Code tags will be used in order to represent GUI codes and Jass codes where certain lines will need to be highlighted.

 1.1.7 Updates

Each update will be recorded here, also I will bump this thread when any update occurs. Note that I will record only major updates, fixing something like “jsut” to “just” will not be recorded. If I will fix many spelling / grammar record then I will state something like: “update: major grammar and spelling fixes”

 Current Version: 1.0

Version 1.0

 - First release of the tutorial


Click to expand...

 1.1.8 Final Notes

Here is list that includes a few final notes about the tutorial.

 English is not my native language; I should note that at first. I’m using a dictionary and a spell checker, so most errors should be fixed however if you spot any problems please know that it was made out of ignorance and not out of laziness to re-read the tutorial (actually I read every part about 3 times). If you find a critical error I would appreciate it if you would point it out.

 I would love to hear comments about the tutorial, negative, positive, whatever, any comments would be great. However, I worked very hard on this tutorial, so if your feedback is negative, try to put it politely please. If you have any questions please do ask them.

 If during the tutorial you don't understand something, don't be frustrated, Jass is hard to learn and for some people it is harder to than for others. Re-read whatever you don't understand, and if you still don't understand skip it and return to it after finishing with the tutorial. I also strongly suggest to read this tutorial at least twice to make sure you understood everything completely.

 I worked on this tutorial about a month, so, take that into account when commenting… I don’t mean to say you must say this is a wonderful tutorial, but try not mocking me by saying “This tutorial sucks!” etc. If you learned something from it, appreciation post could only make me feel better.

 The tutorial is broken into several parts because it is too long for thehelper.net to store it in only one post, so I had to divide it into few posts, sorry about that!

 1.2 Introduction to Jass

Here is a brief explanation of what Jass is. Most of the things I will talk about are technical; I will not talk yet on how Jass really works. Also, since I did this in another tutorial of mine, please excuse me if I quote myself, I simply have already put it in a clear way, no need to fix what works fine.

 1.2.1 What is Jass?

Quoted from Before Jass.

 Like any other software, World Editor needs to read script to know what to do, everything else but the script is simply to ease the script writing. So, for example, when you are making “regular” trigger you use Graphical User Interface (known as GUI). What World Editor does is convert your triggers (and everything else, but that’s something else) into script and then reads it. Do you want a proof? Fine, create a new trigger and do something with it. Say create a trigger that will create a footman in the middle of the map for player red every second. Now select the trigger, go to Edit > Convert to text and see what you get, that’s Jass. This is the true scripting language of World Editor, whenever you work with GUI it is converted to the text you see here, and then when playing your map, Warcraft’s engine can read it.


Click to expand...

In other words we can say the World Editor must take script in order to execute orders. When we are working with GUI (normal triggers) World Editor takes the data given, converts it into Jass, and then works from there. Let’s create a simple trigger together, something like this:

Untitled Trigger 001
Events
Time - Every 2.00 seconds of game time

Conditions

Actions
Special Effect - Create a special effect at (Center of (Playable map area)) using Abilities\Spells\Human\ThunderClap\ThunderClapCaster.mdl

This trigger will basically create a special effect in the middle of the map every 2 seconds. Now select the trigger you created and go up to edit. In edit go and click on Convert to Custom Text.

A warning message will appear on the screen saying that the trigger could not be converted back into GUI. Actually you can convert it back to GUI by pressing on Ctrl + Z but that doesn’t matter at the moment. Now you should be able to see this text:

 JASS:

function Trig_Untitled_Trigger_001_Actions takes nothing returns nothing
 call AddSpecialEffectLocBJ( GetRectCenter(GetPlayableMapRect()), "Abilities\\Spells\\Human\\ThunderClap\\ThunderClapCaster.mdl" )
endfunction

//===========================================================================
function InitTrig_Untitled_Trigger_001 takes nothing returns nothing
 set gg_trg_Untitled_Trigger_001 = CreateTrigger( )
 call TriggerRegisterTimerEventPeriodic(gg_trg_Untitled_Trigger_001, 2)
 call TriggerAddAction(gg_trg_Untitled_Trigger_001_ function Trig_Untitled_Trigger_001_Actions)
endfunction

This is Jass, looking like gibberish? It should at the moment, don’t worry. By the end of the tutorial you should be able to understand it.

So, as you can see, GUI is converted to Jass, and we can say that Jass is World Editor’s true scripting language, and GUI is nothing but a tool to edit it.

 1.2.2 Why use Jass?

Again, allow me to quote myself from my tutorial Before Jass.

Typing is faster then clicking - It was tested and proven that it is faster to write in the computer then to use your mouse and clicking it. So technically Jass is faster to use, because you write scripts. In GUI however, you use charts and click on your picked values.

Jass can do more things - In Jass there are functions (events / conditions / actions) which you can’t find in GUI. Blizzard didn’t convert all of its functions into GUI from Jass because a few were unsafe, or simply Blizzard thought they weren’t needed.

Jass’ functions are faster - Not only that Jass has more functions, but usually when working with Jass you use natives functions (certain types of functions) which are fast, while in GUI usually you work with BJs (other type of functions) which are slower then natives.

Jass does things faster - Not only are Jass’ functions faster, but Jass can also do things faster. Because of the clumsy way GUI is organized, everything is limited, however in Jass because of the flexible system you can do things with many fewer lines and functions calls. Also because of that it is faster to write Jass (well at least after you are familiar with it).

Jass takes less memory - Well this is hardly noticeable, but yes, Jass takes less memory space, especially because it uses locals and not globals. Also with Jass you can remove all leaks, while in GUI you can remove none (well you can remove some by using Custom Script, but not all even with that).

Jass is MUI - Because Jass uses local variables (a special kind of variable) you can easily create MUI spells and triggers with it.

Better Loops - Jass’ loops are much more flexible like most things in Jass and allow us to have complete control over them and use them in the best way possible.

Systems with Jass - Most of the systems now days (or at least the more advanced ones) use Jass, not only because it is easier to work with Jass but because systems can be used the best with Jass only (we can call new user defined functions with Jass ).

Jass is easier to read - Once you know Jass well enough it is easier to read, and to edit than GUI, it makes sharing of triggers, spells and systems easier and better.

People use Jass - People move to Jass. Lately many tutorials were written that ease the learning by much, so many people decide they can spend the time on learning Jass and work with it, you wouldn’t want to stay with GUI while the rest of the mappers make products for Jass.

Jass is powerful - Finally, most of the interesting finds about World Editor are about Jass, GUI is too simple to work on, however some amazing stuff was discovered about Jass that completely changes the programming experience. For example the return bug (well now few problems were found with it but oh well), using conditions as actions and many more things that relate to Jass but not at all to GUI.

Click to expand...

So we can assume that Jass is better than GUI from those finds I guess. The real question that you should be asking yourself is if it’s worth putting all this effort to learn Jass. Well, as shown, Jass is by far better than GUI, if you have the time to learn Jass, you should learn it, no doubts. Few people feel like they want to master GUI completely before moving to Jass. That’s fine, GUI can be learned in less than a month, and after that in order to advance you’ll need to learn Jass.

 1.2.3 Writing Jass

There are few ways to write Jass, each one has its own upsides and downsides.

 World Editor - As you can see you can convert triggers into custom text in World Editor by selecting them, and then going to edit and convert to custom text (see the image above). Well you can convert an empty trigger to Jass and start working from there.

Upsides: World Editor gives you the basic functions; without them you need to keep rewriting them. Also you can test your trigger much more easily.

Downsides: There are 3 problems with World Editor’s syntax checker. A) It is stupid. B) It returns false errors, and even the true errors it can't put clearly. C) Little mistakes may cause it to crash.

 Notepad - The simplest answer, you don’t need to down anything, just open notepad and start working. Don’t use “Word” though, because it’s pages’ length is limited, notepad’s pages can get very wide.

Upsides: You don't have to deal with annoying World Editor, and nothing needs to be download, just get into notepad and start working!

Downsides: No way to know easily if you have errors in your script, plus it might look unclear and plain.

 Software - There are few programs out there that can aid you in scripting in Jass.

TESH - A nice add-on to World Editor, makes it easier to work with it, however I found it a bit buggy. Warmly suggested if you don’t want to keep switching between two windows.

JassCraft - My favorite tool, everything you need comes with it: colored text, accurate errors finder (more or less), function list and detailed information about functions. There are few others programs, but they similar to those two. Jass Shop Pro or Jass Editor are 2 other possible programs for you to use.

Upsides: It really depends on what program you are using, but they usually come with a lot to help you with, such as colored text, better errors finder, list of functions and so on...

Downsides: None really, the only thing that might irritate you a bit is the fact that you’ll need to keep switching between your program’s window and World Editor’s window.

 1.2.4 Jass’ System

Now we start talking seriously, all of this was merely an introduction. Let’s examine a second trigger in GUI. GUI’s triggers are made of events, conditions and actions. Once any of the events is met the trigger will run, if all the conditions are met the trigger will precede to the actions part, which is the main part of the trigger and will do something. For example this trigger:

 Code:

Untitled Trigger 001
 Events
 Unit - A unit Is attacked
 Conditions
 (Unit-type of (Triggering unit)) Equal to Peasant
 Actions
 Unit - Kill (Triggering unit)

This is pretty clear, the event is that a unit is attacked, the condition is that it’s a peasant so the trigger will only run if it is a peasant and the action is to kill the attacked unit. Jass is not made of actions, conditions and events but from functions. Each function can do anything really. It can be an action or part of it, Boolean or Boolean expression, something that returns an integer, a function that will do something to a given group… Anything really. This makes Jass much more flexible, because we don’t have the usual order of event, condition and action but function, function and function, each one of those can do about anything I tell it. In other words:

 GUI is made out of events, conditions and actions, and those three are stored inside a whole trigger which stores them all. That’s why GUI is so easy to edit and work with, but it makes it very limited. Jass is made out of functions, each function can serve as event, condition, action, part of one of those three, or something else completely. This makes the working process faster, easier and more flexible.


Click to expand...

 1.3 Conclusions

A quick conclusion of this part.

 1.3.1 The Tutorial

The tutorial is made to help you learn the basics of Jass while continuing to compare it to GUI. The tutorial is divided into many parts, chapters and sub-chapters to make the reading process easier.

 1.3.2 Jass

Jass is the source language of World Editor and GUI is nothing but a tool to help us edit Jass. GUI will be converted to Jass so the game can read it; however by working directly with Jass we benefit a lot more. The way Jass works is from many functions that can be formed into many triggers, systems, spells etc.

 1.3.3 Final words

Final words before the (long) introduction ends:

I hope you will enjoy this tutorial I worked on this very hard to try and teach you this stuff so please take that into consideration. All questions and comments are more than welcomed.

I hope you enjoy your reading, or at least learn something from it!

Tutorial’s author: Rheias.

The tutorial itself is in the second post so keep reading! It was divided because the tutorial is too long to fit in one post.

Credits: Ghan_04 Fixing some grammars.

2. Functions

In this chapter I will introduce you to functions. I will talk about two types of functions, blizzard made functions and user-defined functions. Both types are similar but we usually use them to achieve different goals. Here is a tutorial I did about functions, however it is a bit advanced so I suggest reading it only after you are done with this tutorial.

 2.1 User-Defined function

 2.1.1 Functions and GUI

Before we start learning about functions let's talk about them quickly. As said in the introduction, differently from GUI, Jass is made out of functions where each function can do whatever I tell it to do. It can be an action, condition, event, part of those or something else completely. In GUI however we have only events, conditions and actions, and that’s it. Do you remember the trigger we converted? Let’s take another look at it.

 JASS:

function Trig_Untitled_Trigger_001_Actions takes nothing returns nothing
 call AddSpecialEffectLocBJ( GetRectCenter(GetPlayableMapRect()), "Abilities\\Spells\\Human\\ThunderClap\\ThunderClapCaster.mdl" )
endfunction

//===========================================================================
function InitTrig_Untitled_Trigger_001 takes nothing returns nothing
 set gg_trg_Untitled_Trigger_001 = CreateTrigger( )
 call TriggerRegisterTimerEventPeriodic( gg_trg_Untitled_Trigger_001, 2 )
 call TriggerAddAction( gg_trg_Untitled_Trigger_001, function Trig_Untitled_Trigger_001_Actions )
endfunction

Here we have two functions, function call Trig_Untitled_Trigger_001_Actions and the other is call InitTrig_Untitled_Trigger_001

 Code:

[B]function Trig_Untitled_Trigger_001_Actions takes nothing returns nothing[/B]
 call AddSpecialEffectLocBJ( GetRectCenter(GetPlayableMapRect()), "Abilities\\Spells\\Human\\ThunderClap\\ThunderClapCaster.mdl" )
endfunction

//===========================================================================
[B]function InitTrig_Untitled_Trigger_001 takes nothing returns nothing[/B]
 set gg_trg_Untitled_Trigger_001 = CreateTrigger( )
 call TriggerRegisterTimerEventPeriodic( gg_trg_Untitled_Trigger_001, 2 )
 call TriggerAddAction( gg_trg_Untitled_Trigger_001, function Trig_Untitled_Trigger_001_Actions )
endfunction

You see those now? Let’s check what each function does.

The first function (Trig_Untitled_Trigger_001_Actions) is the action part of the trigger; here we put the actions that are made to the trigger. The second function is more unique. That function creates the trigger, and attaches the events & actions to it.

So we can see here an example of 2 functions, one that does certain actions and the other that creates the trigger and attaches the actions to it. It might be a bit hard to understand so I will try to put it in other words.

Unlike GUI where the event and the conditions call the actions if they are met, in Jass we have many different functions that each do something different. Those functions call each other and form in this way a trigger.

This sub-chapter was just to give you the feeling of function in Jass, soon it will become clearer.

 2.1.2 Functions’ Goals

Functions, as said, make Jass, everything in Jass is made out of functions and from them, every command you wish to transfer to the game will happen in a function. When creating a function we always need to keep in mind that it does something. In other words each function is its own block of code, and this block of code together needs to do something, else it is useless. As said functions can do many, many things if we build them correctly, but no function can work alone, in order to create a working World Editor product we need to connect functions, not just build them; this will prove very important later on.

 2.1.3 Creating a Function

So in short functions are blocks of code that do something, everything in Jass needs to be in a function so it can be read, now let’s see how we can create one of those functions.

Functions’ Syntax:

 Code:

[b]function[/b] name [b]takes[/b] parameters [b]returns[/b] value

The bolded words must be put down as written, the others vary.

Function - We put this to begin our function by creating a line, if you forget to state “function” at the start World Editor will have no idea what to do. It will not understand this as a function defining line unless we put the word "function" in the start.

Name - You need to state the name of the function; to refer to a function you name its name. The name mustn’t start with integers and can’t have signs in them (such as ? ! : “ etc…). Replace spaces with _. The name is case sensitive, thus calling a function BLAH is different from name it blah or Blah or bLaH etc. You can’t name 2 functions with the same name.

Takes - From now on (until we state “returns”) we give the function parameters to work with (see later the sub-chapter “Calling Functions”).

 2.1.3.1 Parameters’ Syntax

The parameters’ syntax is as follows:

 Code:

ParameterType ParameterName

ParameterType is the type of the taken parameter. Types can be integers, strings, reals, Boolean, trigger, timer, unit, player, item and so on. ParameterName is the name of the taken parameter, in order to refer to the parameter inside the function you will use ParameterName. Note that you can’t name two parameters with the same name inside a function but you may in different functions. Parameters’ naming rules are like the function names’ ones. If a function takes more than one parameter we separate the different parameters with a comma. Example:

 JASS:

integer i, unit a, player blue, real number33998, item Wowish_thing

In case you want your function to take no parameters state “nothing”.

Returns - After we write returns we need to decide if the function will return something.

Value - The value is what returned by the function. Value can be anything that is ParameterType (look at sub-sub-chapter above - 2.1.3.1). Again, if you wish your function to take nothing simply state “nothing”.

Here are few examples of functions:

 JASS:

function This_Is_An_Example takes integer test, unit tirggerer, player owner returns real

 JASS:

 function eXaMpLe_2 takes nothing returns unit

 JASS:

 function Tutorial takes real x, real y, returns location

 JASS:

 function One_More takes location l returns nothing

 JASS:

 function last_one takes nothing returns nothing

So, you know how to basically create a function, now let’s take a quick look at what is inside the function (it will be described in much more detail later on).

 2.1.4 Functions as Blocks

This will be a short sub-chapter to discuss how functions look; later on I’ll go much more deeply into this.

So after you created a function (as described in the sub-chapter above - 2.1.3) you need to define it so it would do something. You’ll need to write lines of Jass (which will be explained later) that will order the function to do something; after you are done with that you need to close the function. This could be done by creating a new line and writing in it “endfunction”.

 JASS:

function close takes nothing returns nothing
endfunction

This function for example will be created and immediately be closed so it would do nothing, a proper function would look like this:

 JASS:

function close takes nothing returns nothing
// A jass line
// Another one
// So on until what you want is achieved
endfunction

Note: "//" starts a comment, comments are ignored by the syntax checker so we can add notes about the code.

 2.1.5 Returning

As said functions may return something, it can be anything that is parameter type. Now lets see how we return it really.

 JASS:

 function example takes nothing returns integer
 return 5
endfunction

You see? This function will return 5.

 JASS:

 function another_ExAmPlE takes unit u returns unit
 return u
endfunction

This function doesn’t really do anything because it takes a unit and returns the same exact unit, so this is useless just an example.

So you should understand how returns works. However you need to remember something very important, once Jass reads the word return it stops executing the function which means:

 JASS:

 function blah takes nothing returns integer
 return 4
 return 16
 call CreateUnit(…)
 return 579079
endfunction

The function above will only return 4 and nothing else.

Tip: If you want to skip remaining actions in a function (like GUI) all you need to do is create a new line and type "return" this way the function will stop executing itself because, as said, once a function meets return it will stop running.

 2.2 Blizzard’s Functions

 2.2.1 Blizzard Functions and GUI

Blizzard functions (or natives (there are BJs as well but let’s ignore them for the time being)) are what allows us to deal with Jass. Everything that you do in GUI is actually converted into a native (or BJ, but as I said I’m ignoring those now). Once a native function is called it does something. For example when you want to create a unit you call native that creates a unit. Do you want to show all players the message “blah, blah”? You are again using a native.

In other words we can say that natives execute orders given, those natives let the game know that we want to do a certain action and so the game reads it. Continue reading to see how you tell a native what to do!

 2.2.2 Blizzard Functions and User Functions

So we understand that Blizzard functions need to do something, although User Functions are created so we could do something inside them. In other words we create User Functions, and inside those function we put Blizzard Functions that execute the wished orders. However User Functions sometimes supply information to the blizzard functions, so don’t think those user functions are useless, they are very important in organizing the code in the map and executing everything in the game time and properly.

Now, much like user functions take something and return something. Differently from user functions though, they must do one of those things, you will never see a Blizzard function that takes nothing and returns nothing.

Let’s go on to the next sub-chapter then!

 2.2.3 Calling Functions

We call Blizzard function the same way we call user-designed functions, you should keep that in mind. Now when calling a function we need to follow this syntax:

 Code:

[b]call[/b] FuncName[b]([/b]Parameters[b])[/b]

As you can see we call a function when stating the word call. Doesn’t it remind you something? Oh yeah, when we created user-designed functions we had to start the line by stating “function”, well here we switch “function” with “call” (as in calling a function). After that we put the function’s name, which is how we refer to functions. After that is done we open parentheses, put down the parameters and close them.

Now let’s talk about the parameters. When we put a parameter down the only thing we must do is refer to it, thus giving its name, but not its type. If we put several parameters we separate them with commas.

Here is an example.

 JASS:

function DisplayTextToForce takes force toForce, string message returns nothing

We can see that the function called DisplayTextToForce takes a force (player group) and a string and returns nothing. What this means is the syntax to this function would be.

 Code:

 [b] call DisplayTextToForce ([/b]force,string[b])[/b]

Let’s give an example.

 JASS:

 call DisplayTextToForce(GetPlayersAll(),”Hi, this will display this message to all players")

You got the big picture? Not sure? Let’s put it in other words, when calling a function (user-defined or blizzard one) we put down “call TheNameOfTheFunction(The thing the function takes and does with them something)”. If we would compare this to GUI thing when adding a new function, say Game > Display Text you need to tell what player-group you want to show the text to, and what string they should see. This is the same thing only that here we separate parameters with commas and it’s in Jass.

Note: If a function takes nothing leave the brackets empty.

 2.3 Conclusions

 2.3.1 Conclude User-Defined Functions

So we can say that we create a user-defined function by putting “function” then the function’s name, what it takes and what it returns. Once a function created we can call it like any other Blizzard Function. In a user-defined function we put lines of Jass that tells it what to do.

Everything when working with Jass must be in those function, each function must have different name.

 2.3.2 Conclude Blizzard Functions

Blizzard function take something and return something, much like user-defined functions, however differently from those, blizzard functions must do one of those two. While user-defined functions store in themselves blizzard functions, blizzard functions are the ones that do the real work and execute orders given.

By know you should understand the taking part of function, a function takes certain parameters in a certain order, and when it takes those it can do with them something. The returning part will be discussed in more little in the part regarding local variables.

 2.3.3 What’s next

Just a brief review on what I will talk about next. Since I talked already in general about the biggest difference between Jass and GUI there is not much left. I will discuss variables, triggers, special actions (loops & if/then/else) and after that I’ll do a short chapter on how to continue by yourself, the tough part is behind you!

 2.3.4 Again, calling

For all of you that may have problem with understanding how to call a function, this short sub-chapter will attempt to conclude what I said about calling functions and putting it in other words.

When you want to do something in GUI what are you doing? You are going to the events / conditions / actions tab and from the many charts there pick an option. For example to create units. Now, once you picked the action to create units, you need to let the computer know few things right? For example how many units, of what type, to who player, where and so on. In Jass basically it works the same way, you put “call” and then type the picked command (funcname) open brackets and put parameters down so the computer, again, will know what to do. Also, like GUI, in Jass there is a certain order where you need to put it. The only difference is that you type everything and you put commas between parameters.

 2.3.5 Functions in Math

One thing that helped in understand function after looking back is comparing them to math. Lets take Pi. Pi returns 3.14(and infinite number after the 4), so when thinking about it in Jass it’s a function that takes nothing and returns real, right?

 JASS:

 function Pi takes nothing returns real
 return 3.14
endfunction

now lets thing about multiplying, of course we can multiply normally (integer1 * integer2) however if we would want to make a special function for it then:

 JASS:

function multiply takes integer a, integer b returns integer
 local integer counter = 1
 local integer ConstantA = a
 loop
 exitwhen counter > b
 set a = a + constantA
 set counter = counter + 1
 endloop
 return a
endfunction

I don’t expect you to understand what was inside the function, however you need to understand that this function will take 2 integers multiply them and return the result.

3 Variables

In this chapter I will discuss fully variables in Jass. Global (normal) variables, locals, comparisons, and finally setting new value and, all of those would be discussed in this chapter.

 3.1 Introduction to Variables

 3.1.1 Variables in GUI

In GUI there is only one type of variables which is called “global”. You can open the variable’s tab by clicking on ctrl + b. Once opened you can create a new variable by defining its type and name (the name follows the same rules as the functions’ name follow). For example you can crate a variable on type integer, that has the name VarInt. Variable’s type is just like parameter’s type.

Now what those variables actually do, is storing value of their type in them. For example a variable of type integer could store integers (1,5,39398,number of units in the map, the player’s id… Anything that is an integer) unit variable could store units (triggering unit, picked unit, last created unit… Any unit) and so on. Once we store information inside a variable, we could later on refer to the variable and get the stored information in it. For example after 30 seconds started in the game you want to check the number of units in the map, and after another 30 seconds see if there are more or less units now. This could not have been done without a variable, however by storing the number of units after 30 seconds in a variable and then referring to the variable after a minute could make it possible.

One last thing about variables, variables can have arrays. Arrays are indexes, which are presented inside []. So for example Some_Variable[0] can store different value then Some_Variable[1], however they must store the same type of value. You can think of an index as an addition to the name which is represented by an integer so we could get a better use of it (for example loops).

 3.1.2 Variables in Jass

In Jass we have two types of variables. One type is the global type as mentioned above and will be discussed later in greater detail, and the local type which is basically the same thing but more limited. This limitation of the local variables allow us to achieve with them many things, yes the limitation is in some way a benefit.

 3.1.3 Warning

If you are very new to GUI and have no idea what variables are, I don’t suggest reading this without practicing variables in GUI. It takes certain way of thinking to understand variables, which can be easily understood with some practicing so I suggest you work on that before going on and reading. Just create triggers and check out variables’ powers in them.

 3.2 Global Variables

 3.2.1 Using Global Variables

As said, there are variables, one type is locals and one type is called globals. Let’s talk about globals. Global variables store inside themselves information, and they will keep that information until the end of the game, unless you change it. For example you can store in a variable called IntVar the integer five, and then change it to seven. From the time you stored the five in the variable until you stored seven, referring to the variable will return five. Afterwards it will result seven because we changed the value to something else.

Now, we said that you can create globals in the variables tab, where you give a variable a name and a type. If you want to refer to a global variable in Jass you need to follow this syntax:

 Code:

 udg_[b]VariableName[/b]

So say we talk about the IntVar again, in order to refer to it in Jass we will put:

 JASS:

 udg_IntVar

“udg” stands for “User Defined Global”, and the “_” is just a space.

 3.2.2 Globals in Jass

One thing you must understand is that once you stored an information inside a global variable, no matter where, when or how whenever this variable will be referred to it will return the information stored in it, kind of like a function that takes nothing and returns something (and that something is stored in the variable). You can refer to a global variable in any function. This means another thing, if you changed the value of the global variable in one function it will affect all other functions that will use the variable later on, if you want to avoid that you’ll need to create few variables, or possibly adding an array to the variable.

Now, a question that you might ask yourself is how do I use global variable? Well as I said in order to refer to it you put udg_VarName, but one thing I didn’t make clear yet is that variables are used only as parameters. Variables can’t execute and order or something like that, because the only thing that they can do is store a certain value and then when needed return it, thus the only time you would use variables is to use them as parameters.

As for arrays (indexes) they are represented inside a []. So:

 JASS:

IntVar[0]

It a variable called IntVar and its 0 index. You must remember that you can't use array in a global that wasn’t originally created with an array (inside the variables’ tab).

Reminder: To those that don’t remember parameters are passed to functions (blizzard ones, or ones made by users) so those functions could use the information that the parameters pass to execute certain orders.

 3.2.3 Locals and Globals

Before I start talking about local variables I’ll give a brief introduction to it here while comparing it to global variables. Locals, unlike globals are created especially for the function. They are like parameters taken by the function if you will; they can be used only in a function and can’t be referred to from other functions. While this seems like a limitation we can benefit from it. Say we use a function with wait inside it, how can we know for sure that the variables we are using won’t change their values during this wait and then ruin the whole function? The most common example is spells, say you store few parameters in variables once the spell is being cast and then you use wait. Think if another unit will cast the same spell during the waiting time, the previous variables will be overwritten and the spell will be ruined. This is why locals can come in handy, because locals change their values only in the functions were they where created in, we can avoid data being lost.

 Note: Spells that can be cast without fearing this data overwriting are call MUI. In case you are using arrays to represented players, but still it is not MUI you call the spell MPI.

 3.3 Local Variables

 3.3.1 Introduction to Locals

As said, local variables are different from global variables because globals can change their value in any function and will affect all other functions while local variables change their value only in one function. You can think of locals as parameters that you give to a function, it something that the function may refer to. You may use the same name for two locals if they are in different functions, else they must be named differently. Also locals should be named the same as parameters.

Now, let’s explain in more detail how locals work. You can create local variables inside functions, once you do that you can store in those information. But the fact that you need to create locals in every function separately and that they change their value only within the function they were created in, locals are like globals in anyway. Local variables store information that can later on be restored by referring to them. Of course we can use arrays in locals as well.

 3.3.2 Creating Locals

The syntax to create locals is as follows:

 Code:

 [b]local[/b] localType local_name

Local: The local is like “function” and “call” you simply tell Jass what this line will do, in this case, create a local variable.

localType: The local variable’s type, just like parameter type, locals can store only values that are like their type.

local_name: the name of the local variable, you type the local’s name to refer to it. Follow functions’ names rules. Examples:

 JASS:

 local unit triggerer

 JASS:

 local integer eight

 JASS:

 local trigger exe

 JASS:

 local real x

 JASS:

 local player picked

And so on, those are nothing but examples. Of course you may have any number of locals you want in a function, you can have none, you can have dozens, it is very flexible.

Something very important to remember is that locals must be declared in the start of the function, you may not do anything before all locals are defined.

Legal:

 JASS:

function ok takes nothing returns real
 local unit something
 local real other_thing
 return real
endfunction

 JASS:

function legal takes unit u returns nothing
 local unit u_copy = u
endfunction

Illegal:

 JASS:

function wrong takes play p returns nothing
 local unit p // You have parameter and local with the same name!
 local integer s
 local real s // 2 variables with the same name!
endfunction

 JASS:

function X takes nothing returns nothing
 local unit blah
 call DisplayTextToPlayer(…)
 local item t // An action is executed before all locals are defined
endfunction

Now say we want to add indexes to a variable, how do we do that? As always, simple syntax to follow.

 Code:

[b]local[/b] LocalType [b]array[/b] local_name

Just squeeze in the word “array” between the local’s type and it’s name.

Important: Once adding an index to a local variable you must ALWAYS attach an index to it (local_name[integer]). Same goes to global variables by the way.

 3.3.3 Setting Values

Now, locals aren’t any good if we don’t set a value to them, right? Again, follow the following simple syntax to store a value inside a local.

 Code:

[b]set[/b] local_name [b]=[/b] value

Simple? Yes.

Set: Like “function”, ”call” and “local”, we define the line’s type.

Local_name: The local’s name which we want to store the value into.

=: After that we put the value which needs to be stored.

Value: Value is a parameter, it is stored inside the local. The value’s type must match LocalType of the local, else errors will pop.

Examples:

 JASS:

local unit x
set x = GetTriggerUnit()

 JASS:

local integer id
set id = GetPlayerId(GetTriggerPlayer())

 JASS:

local real p
set p = 0.373

Etc.

Now, do you see the examples above? What do we do? We create a variable in one line, and the in the other line store a value inside it. Wouldn’t it be nice if we could do it in just one line? Well guess what, Blizzard thought about it, you can do it.

 Code:

[b]local[/b] LocalType local_name [b]=[/b] value

Pretty simple, aye? What we are doing is merging the creation line and the setting line into one that does the exact same thing. Fun, huh? So the three example above can look like:

 JASS:

local unit x = GetTriggerUnit()

 JASS:

local integer id = GetPlayerId(GetTriggerPlayer())

 JASS:

local real p = 0.373

Note: We can set values to globals easily the same way

 Code:

[b]udg_[/b]global_name = value

 3.3.4 Comparisons

Now let’s talk about comparisons. We can compare variables easily to each other, usually this will be used to check certain conditions (is X > Y etc…). When comparing two variables they must be of the same value (is a player bigger then a unit?.. What?!) unless we are talking about integers and reals.

 JASS:

X > Y // Checks if X is greater then Y. Only integers and reals.
X < Y // Checks if Y is greater then X. Only integers and reals.
X >= Y // Checks if X is bigger or equal to Y. Only integers and reals.
X <= Y // Checks if Y is bigger or equal to X. Only integers and reals.
X == Y // Checks if X is equal to Y. All variable types.
X != Y // Checks if X is not equal to Y. All variable types.

 3.5 Conclude Variables

 3.4.1 Globals and Locals

We have two types of variables. Globals, which are created in the variables’ tab (ctrl + b). Globals can change their values everywhere, and when they do change their value they affect all functions that use them. Locals are created inside a function and can change it’s value only within this function. This limitation (allegedly) can help us keep a function working, doesn’t matter what happens in game, so changes won’t effect it.

 3.4.2 Variables in Jass

Variables in Jass look pretty similar to ones in GUI. Of course you need to create locals (at the START of the function), but other then that we got the same things, setting, using as parameters and comparing. Of course the syntax is different but you get the idea.

4 Special Actions

In this chapter I will talk about two very important actions, if / then / else and loops. Both can do great things if used wisely (as you probably know from your GUI experience) but you need to learn them!

 4.1 Introduction to Special Actions

 4.1.1 What are Special Actions

Special actions allow us to do things that usually we couldn’t have done, each one of those is very important for the coding process, without the special actions, Jassers were doomed to make boring, simple codes.

There is the If / Then / Else action, that allows us to check if a condition is true. If it is then we do one things, if it isn’t then we do other thing. The loop action allows us to repeat one action several times until we order it to stop.

 4.1.2 The Dangers

Special actions have few problems in them. They can cause crashes in-games when called or even crash World Editor upon saving if they are handled incorrectly. Make sure you follow the instructions posted to stop those events.

 4.1.3 Multiple Use

Before I’ll go into explaining in more detail what each one of the special actions do, you need to note that you can use special actions inside special actions as much as you want, there is no limitation to that.

 4.2 If / Then / Else

 4.2.1 Introduction to If / Then / Else

If / Then / Else (I/T/E) action allows us to put a condition and two set of actions. The I/T/E action will check if the condition is true. If it is then the first set of actions will be executed else the other set will be.

 4.2.2 If / Then / Else’s Syntax

As always in Jass we got a syntax to follow.

 Code:

[b]if[/b] condition [b]then[/b]
 then actions
[b]else[/b]
 else actions
[b]endif[/b]

If: Yup, like the others (“function”, ”call” etc.), defined the line.

Condition: The action will check if the condition is true, if it is then the first set will be executed, else the other one will.

Then actions: If the condition is true this set of actions will be executed by the game.

Else: Separates between the first and the second sets of actions

Else actions: The second set of actions which will be executed only if the condition was false.

Endif: Ends the I/T/E actions.

IMPORTANT(!!!): Forgetting to put the “Endif” will result a World Editor crash when saving the map. Triple check that every I/T/E actions is closed by the “endif”.

IMPORTANT(!!!): Something that many people don’t know is that a function must return something out of an I/T/E actions. What that means is that if you have a function that returns something, it can return a value inside the I/T/E block, but at least one returning line must be outside this block

Last thing to say is that if you want the I/T/E action to do nothing if the condition is false you can drop the “else” line. So this works:

 JASS:

if condition then
 actions
endif

 4.2.3 Examples

Well, that was short because If / Then / Else is powerful, yet easy. Here is an example

 JASS:

function ITE_Example takes player user returns nothing
 if user==Player(0) then // Checks if the player taken by the function is player red
 call SetPlayerState(user,PLAYER_STATE_RESOURCE_GOLD,500) // Sets’ the player’s gold to 500
 else
 call SetPlayerState(user,PLAYER_STATE_RESOURCE_GOLD,100) // Sets’ the player’s gold to 100
 endif
endfunction

Easy, huh? This function takes a player, if the player taken is player 1 (red) then the game sets the player’s gold to 500. If the function takes another players then it sets the player’s gold to 100.

 4.2.4 Elseif

The is something called Elseif. Say you want to do the following:

 JASS:

if 1 == 1 then
 actions
else
 if 1==2 then
 actions
 endif
endif

You can short this block to:

 JASS:

if 1 == 1 then
 actions
elseif 1==2 then
 actions
endif

Simple as that.

 4.2.5 Uses of If / Then / Else

So, you know perfectly well how to use If / Then / Else, the question is, when and why to use. Well, say you want to check in the middle of the trigger if a certain condition true to affect you actions. Well without If / Then / Else you had no simple way of doing it, however now you can, by simply typing “if condition then actions else actions” (of course you can drop the else actions as said). Otherwise, us Jassers would be in a bit of problem.

 4.3 Loops

 4.3.1 Introductions to Loops

Loops are also great actions. Loops allow us to repeat certain set of action again and again until a certain condition is met. There are classic loops that run X amount of times (from 1 to 10 times) and other loops, which are more flexible that can follow other type of conditions.

 4.3.2 Loops’ Syntax

The loops’ syntax is as follows

 Code:

[b]loop
 exitwhen[/b] condition
 action set
[b]endloop[/b]

Loop: Starts the loop, all action between loop and endloop will be executed several times.

Exitwhen: After exitwhen we put a condition that will order the loop to stop looping when it’s met.

Condition: The condition that will stop the loop once it is true.

Action set: The actions that will be executed several times

Endloop: Ends the loop (much like endif)

IMPORTANT(!!!): Forgetting to put Endloop at the end, will cause World Editor to crush, like forgetting to put Endif.

 4.3.3 Normal Loop

If you think about GUI, usually the loop run certain amount of times, from integer X to integer Y, right? This can be done easily in Jass, following this general syntax:

 JASS:

local integer start = // The starting integer.
local integer end = // The ending integer.

loop
 exitwhen start > end // This will stop running the loop once start is bigger then end
 Actions here
 set start = start + 1
endloop

Do you understand what’s going on? We store two integers in two locals, one is the starting integer of the loop, and one is ending. After that we add a condition that says “Stop the loop and start is bigger then end”. Then, what we do is after each loop run, we increase the starting integer by one.

 4.3.4 Other Loops

And then we have other loops, in the exitwhen we can put any condition that will be checked every loop run, what means you can put whatever condition you wish and the loop will run until that condition is met. You can tell the loop to run until some player have some amount of some resource, or make the loop run until a unit dies… Whatever, you can put it all there.

 4.4 Special Actions Conclusions

 4.4.1 Special Actions

So special actions can either check if a condition is true, and knowing that affect our actions (If / Then / Else) or run a set of actions several times until a condition is met (loops). You will find yourself using both a lot, because they can make the coding experience, faster and more efficient.

 4.4.2 Conclude If / Then / Else

If / Then / Else action, like its name, allows us to check IF a condition is true. If it is THEN we do one set of actions, ELSE other set. That helps us change our actions if needed by simply checking if certain conditions return true or false, quite effective.

 4.4.3 Conclude Loops

Loops allow us to repeat action several times until an action is met. If used wisely we can make loops run until we need them to stop, by modifying the exitwhen condition. Also, if we use regular loops (start > end) we get an integer that keeps increasing it’s value, what can make the actions a little different each time.

5 What’s Next?

Well you are through with the most basic things, there is yet much for you to learn, but you know the basics now. First of all, check my tutorial Before Jass, it explains in great detail on how to learn Jass.

However I will explain also here in short how to learn as well.

Now that you know the basics of Jass you need to do three things.

Learn new commands - You need to learn commands (Blizzard functions) to do things in Jass, as said they execute orders, but you need to learn what they do, their names, what parameters they take and in what order. There are two ways to do that. One it to convert GUI triggers to Jass, or a better way is to download one of Jass’ programs and search in the list there for actions. More on this in the link above! Also you can find in my tutorial links to other tutorial that teach Jass, highly suggested that you read them too, to understand things I didn’t talk about them here.

Practice, practice, practice - Yes! Practicing is the key to learn Jass, you must get better with it, do things faster, get better understanding of things and so on. With practicing you can understand things alone, be able to write and understand Jass better, learn new tricks and so on. Don’t be lazy!

Moderate Knowledge - After you did the previous two steps, go to my tutorial and check the many links there for more advance tips that can improve your coding, slowly you’ll learn many new things that will allow you to do many complicated things and master Jass better.

Thanks for reading my tutorial, please comment on it.

Rheias 16.7.07(DD.MM.YY)

Back to Top


 Last edited: Dec 25, 2007

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

This looks very nice, however, we do have [trigger] tags.

I like this, and therefore you get +REP!

--donut3.5--

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Try out [goto] and [point] tags.

Example use:

[goto=Pnt]To Top![/goto]

(at the top, have [point]Pnt[/point])

To Top!

This is going to be a nightmare to review -.-

So far;

-All images must be attached to the post, not linked offsite. This is enforced so that the failure of other sites will not cause the loss of pieces of tutorials on this one.

-You forgot to list Functions in "Why Use Jass?"

-Suggest JassNewGenPack. Even if the people aren't ready for vJass, they can still benefit from debugging and a competent syntax checker

-Please don't use Word, it's making your quotes "Smart" which really screws up strings.

I'll review some more later.

#### Rheias

#### Rheias

 Level 5

JoinedSep 13, 2007Messages33

Resources: 1

#### Resources
Tutorials 1

> Try out [goto] and


 Last edited: Dec 24, 2007

#### Mini-Me

#### Mini-Me

 Level 17

JoinedOct 30, 2004Messages1,277

Resources: 10

#### Resources
Maps 10

+Rep for lenght . You seem to have put a lot of work in this.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

 > You forgot to list Functions in "Why Use Jass?"

WhT do you mean?


Click to expand...

You forgot to list them as an advantage of Jass

 > Suggest JassNewGenPack

This is a very basic Jass review, why jump into more advanced stuff? No, after you learn a bit, you'll find out about NewGen pack.


Click to expand...

As I said, not for the vJass, but for the better compiler (among other things) that doesn't crash at every turn.

 > Please don't use Word, it's making your quotes "Smart" which really screws up strings.

Give an example please.


Click to expand...

 JASS:

call DisplayTextToForce(GetPlayersAll(),”Hi, this will display this message to all players")

Click to expand...

#### Rheias

#### Rheias

 Level 5

JoinedSep 13, 2007Messages33

Resources: 1

#### Resources
Tutorials 1

> You forgot to list them as an advantage of Jass


Functions, by themselves are not at advange, and, anyway it is too techinical.


> As I said, not for the vJass, but for the better compiler (among other things) that doesn't crash at every turn.


I support JassCraft.


> Last thing


How do I fix it?

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Type the quotes in something like Notepad or in the browser, not in MS Word.

Also, functions are technical, but loop effectiveness and memory usage aren't?

Your choice, of course, though.

And I personally prefer the normal WE over JC (other than the native checker) just because it's easier to compile an entire map, and it auto-supports all the UD and GG globals.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Hi, you obviously took a lot of effort to create such a tutorial.

However in my JASS experience and in my tut experience i must say that your work is VERY messed up. Iy is a real head hake to read all the information.

As purplePoot said, Use goto tags for God sake.

And images ???? (as he also said)

Also if you don't like JassnewGenPack (i don't like it either because i don't know how to use it lol) Please indicate JassCraft as a program to do JASS.

Programs like those i mentioned are necessary do good jassing as they correct our errors and make our job a lot easier.

You should mention indenting as well.

The best way to pass from GUI to JASS is to convert GUI spells into JASS spells. You should either talk about that or give some examples (my map has some great GUI spells i ma converting, if you want, i can give you an example from a converted spell to your tutorial, as long as you give me credits, after all, i did the spell).

Laso an advantage of JASS:

-"Jass runs faster". YYa but why ???? Well here is why: GUI has access to the internal memory of the computer, however to do so, he uses BJ's and other functions which take alternative long ways to reach the command. By using JASS you can order the computer to reach those values immediatly thru the fastets and easiest ways. Also when GUI uses a variable it does not "cleans" it thus, contributing to increase lag. Although you may clear variables with GUI, it is impossible to do a clean script with it. Jass runs faster because with it you can reach commands in new ways, and because with it you can make clean scripts, thus greatly reducing lag.

-"JAss is MUI" ya, but why ??? Well: To start GUI only uses global variables which can be replaced. By using locals JASS has an advantage. Although you explain this part quite well, i think you should give it a look. When GUI uses a variable it stores the variable's value inside the computer's memory. Because that value is never removed it will leak, and most probably be replaced by the next unit.

- And finally one last advantage of JASS: JASS is very similar to other programing languages such as C. Language C is the base of JAVA and C++, so by learning JASS you have your way oppened to learn new languages (however language C is more difficult then JASS, i good jasser will be good at language C).

Also if you already know language C, then JASS will be like stealing a kandy from a baby (evil conception of programing i know =P)

All for now, i need to read more. As i said you took a lot of effort to do such tutorial, but you still have a long way ahead you.

You should also talk about loops and ifs.

Also you should add links and recomend people other pro JASS tutorials so they can learn JASS.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

C/Java/C++ and JASS are basically nothing alike -.-

I also think he mentions JC

[jass=Some Jass]function bleh takes integer i, string params returns real//we couldn't have a string array =(

 local integer i = 5

 call BJDebugMsg(params)

 return I2R(i)/5.

endfunction[/code]

Some Java:

 Code:

class myClass {
 float myMethod(int myInt, String[] params) {
 int i = 5;
 System.out.println(params[3]);
 return i/5;
 }
}

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

mmm, Java uses program C as a basis. If JASS is like C then it can be used in JAVA.

I will start learning prg C and JAVA very soon, but according to what i already know, i can find many similarities between those two languages.

I could write some if you like from the book i am reading.

I strongly believe that if you are a good Jasser, then you will be good at C or others.

Also C++ is type of evolution from C, so they have a common basis as well.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

JASS would help with learning other languages, yes, but it's not really similar -.-

And C++ is a precompiler for C, afaik of course but I'm pretty darned sure.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

What you said just proves my point ... personally i thik it helps because i finf those two (JASS and C) similar. I only started to learn C a few days ago, but i find very exciting the fact that i can actually apply JASS concepts to C. They do work =)

I also know i can program warcraft 3 to run with program C , (maybe C++ as well ?) so i really wanna learn it =)

Still, the points is, that the fact that it helps, is one more afvantage to JASS in relation with GUI.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

No, you can't write scripts for maps in C.

Anyways, in that sense, FP, any language helps with any other.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

LOL, yes i can. I know i can because i already saw tutorials teaching that. I saw one of them here in THW a few months ago, but i couldn't find it now ...

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

No, you can make very basic custom natives that either call other natives or do operations not involving handles.


 Last edited: Sep 29, 2007

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

hã !? please chech the grammar as i don't understand the meaning of your setence ..... =s

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Missed a letter. (wrote "all" instead of "call")

Fixed it.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

AHHH, lol, well, that is what i know of JASS for now 2. I also don't master the handles stuff, so C is my solution. I know once i master C, not using handles will not be a problem for me.

Also, please can you give me some more tips about that icy aura i made in the jass section !? i am getting a litlle desperate here =s

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Rheias, how's it coming?

Maybe tomorrow, FP, but let's continue that discussion there.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

lol, k thx m8. Still , i would like to know if Rheias wants some of my JASS spells.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

PurplePoot said:

 Rheias, how's it coming?


Click to expand...


Are there any new developments?

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

I don't think he intens to finish his tutorial =S

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved!

Goto and Point tags added, two posts merged.


~ Thread moved to JASS/AI Scripts Tutorials.

#### GoSu.SpeeD

#### GoSu.SpeeD

 Level 6

JoinedNov 1, 2007Messages42

Well being completely new to WE altogether i decided to give this a read. It was a good read considering i know almost next to nothing about JASS.


Also i've been learning C for coming up to a year soon and i can say the syntax is nothing like C, i'll agree that learning C for a while then taking up JASS as well makes JASS look easy, C teachs good programming fundamentals in my opinion.


C++ is actually crazily different from C but everyone seems to think just because they both start with the same letter, they are practically the same. Even though C is Procedural Programming and C++ is object-orientated (such as the ability to use and create classes).


Ps. the equals sign (=) is known as the assignment operator, as to assign a value to the target variable, i remember reading you using a different name for it in the tutorial so i thought i'd point that out.

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

Suggestion

I just recently finished reading a few of the tutorials written in these forums, and I think I got a pretty clear definition on all the topics included in this tutorial


The thing about, is that when you "see" it as opposed to "trying" it, the situation changes completely


The days that I had devoted solely to JASS learning (creating,testing, + then fixing) I found alot of little things about the code that could cause it to bug


These tutorials should include lists of such things, and some common error messages from the WE compiler(what they could possibly/probably mean)


Or any suggested compilers for that matter


One little error that really got me for a while (30mins, with the conclusion of keyboard bashing ><) was that strings need " " quotations no matter where they are


Ex)


"Any string you like"


and the only time they don't need that, is if they are set to string variables


Another thing was this 'A000' I'm not 100% sure of the meaning, but I know it refers to a spell, and also needs ' ' type quotation, or else it will bug


Those are little errors and things that come from practicing continually, and for brand-new (I mean like, doesn't even understand the word parameter type of new) these mistakes could cause them to just up-and quit because they are so damned frustrated


I like Phoenix_Fires idea of using GUI spells, and then converting them to JASS


Somebody should make a whole thread where such things are posted by any user, and then people can try challenges (and admins should make damnedsure the challenges have answers somewhere, otherwise the purpose would be somewhatself-defeating)


Posted up by more advanced JASSers


P.s, what the heck is a "struct" is that just for NewGen??? (I don't like using newer programs until I've learned the original basics)

#### bReathl3sS

#### bReathl3sS

 Level 5

JoinedDec 18, 2007Messages205

Hey, I think I found 2 writing mistakes:


-1.2.4 Jass’ System


 "...but function, function and & function, each..."


 > If u read it its 'function, function and and function'


-3.2.1 Using Global Variables


 "..From the time you stored the fine in the variable until you stored seven, referring to the variable will return five..."


 > isnt it 'five' instead of 'fine'?


and: ur tutorial is great, i'm just learning jass and i understand everything. great work


greetz

#### Zynk

#### Zynk

 Level 3

JoinedDec 16, 2007Messages38

Awesome Tutorial man!!


I read everything in a few hours without breaks, and I really liked, you explain very well..


Keep the GJ!!

#### Rheias

#### Rheias

 Level 5

JoinedSep 13, 2007Messages33

Resources: 1

#### Resources
Tutorials 1

I'm dearly sorry, I wasn't active here for a while. I'll get back on fixing the tutorial, thanks for the patience and thanks for apporoving the tutorial.


Edit: I seem to be having problems with fixing the code and jass tags problem. Even though I'm rewriting them, they still remain as they are.


 Last edited: Dec 24, 2007

#### bReathl3sS

#### bReathl3sS

 Level 5

JoinedDec 18, 2007Messages205

Hey, I think I found another writing mistake. I don't want to bother u, i just want that the tutorial is perfect =) . so here it is:


-4.2.3 Examples


"...Well, that’s was short because If / Then / Else is powerful, yet easy..."


>If u read it, it is: "Well, that is was short..."


greetz

#### monchopim

#### monchopim

 Level 8

JoinedSep 25, 2007Messages382

when will the second part come? i dun want to be annoying, but just asking xD, i want to know it all about JASS. also, im patient, waiting.

#### bReathl3sS

#### bReathl3sS

 Level 5

JoinedDec 18, 2007Messages205

yeah good question, just the same i wanted to ask xD

#### Rheias

#### Rheias

 Level 5

JoinedSep 13, 2007Messages33

Resources: 1

#### Resources
Tutorials 1

Second part? When did I ever say I was to release a second part?

#### bReathl3sS

#### bReathl3sS

 Level 5

JoinedDec 18, 2007Messages205

wait anyhow did i post this comment to a wrong address, sorry!


but if u want you can make a 2nd part


and i still make sth wrong with the 'loop's and 'if's -.-


well his is not the theme to talk about it


greetz bReathl3sS

#### Aeline

A

#### Aeline

Well, I just read over the tutorial, and I'm not totally sure that I understand functions and how they take and return things, or how to get the trigger to do things with what its taking. I'm fairly sure that this will clear up once I start turning my GUI stuff into Jass and experiment with it.


Another thing I'm not sure about is why there has to be four spaces before each line thats an indent. Is it required for the code to work or just there to simplify things?


I'm also wondering about is local variables. If a trigger detects it's events, starts performing it's actions, and has a wait action in it, then the same trigger detects its events again before it finishes the first time around, will the local variables stay the same for the trigger's first run, and be different for the trigger's second run while they are both running at the same time, though a few seconds apart?


Lastly, I'm not sure if I just dont understand Jass at all yet, or if this tutorial doesnt cover the Gui equal of events for Jass.


Everything taken into consideration, I enjoyed reading this tutorial and am pretty sure its given me the knowledge I need to start experimenting effectively with Jass, many thanks and +rep .

#### 01239

#### 01239

 Level 1

JoinedSep 25, 2007Messages3

Great thanks for making this tutorial!


Im a old GUI guy and know quite everything there, so this was easy understanding for me.


thx again, i will go to read on now : )

#### bReathl3sS

#### bReathl3sS

 Level 5

JoinedDec 18, 2007Messages205

Aeline said:

 I'm also wondering about is local variables. If a trigger detects it's events, starts performing it's actions, and has a wait action in it, then the same trigger detects its events again before it finishes the first time around, will the local variables stay the same for the trigger's first run, and be different for the trigger's second run while they are both running at the same time, though a few seconds apart?


Click to expand...


Well as i understood the tutorial, that's the advantage of locals:


They can run multiple times without changing values.

#### FR Jeka

#### FR Jeka

 Level 2

JoinedAug 14, 2005Messages14

+REP for easy (i think) explanation for a GUI'er like myself it took two hours to understand the basics...thanks man, the only thing is a bit more examples, maybe a simple function that do something real...

#### Rheias

#### Rheias

 Level 5

JoinedSep 13, 2007Messages33

Resources: 1

#### Resources
Tutorials 1

Thanks all for kind comments about the tutorial.


Now on to answering questions.


> but if u want you can make a 2nd part


I covered up all the basics, I'm not sure there is room for a second part.


Maybe, we'll see.


> and i still make sth wrong with the 'loop's and 'if's


What do you have problems with exactly?


> Well, I just read over the tutorial, and I'm not totally sure that I understand functions and how they take and return things


In order to work perfectly functions needs to take values they can refer to. When a function returns something after it did a certain action it returns something that you (or other functions) could refer to.


> will the local variables stay the same for the trigger's first run, and be different for the trigger's second run while they are both running at the same time


Precisely.

#### Blackveiled

#### Blackveiled

 Level 8

JoinedMay 26, 2007Messages214

Resources: 4

#### Resources
Models 1Maps 1Tutorials 2

Thank you Rheias. I always saw this and never took the time to read it, but tonight I was thinking to myself, heh I am bored. I also wanted to learn Jass, I went through the whole thing in a good 2 hours and now I know the basics of Jass and how it works


GJ

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

2nd Tutorial

Perhaps you could go into detail about the JASS upgrades, NewGen and such


How structs work, and other things that I have seen in advanced code that makes my eyes bleed

#### bReathl3sS

#### bReathl3sS

 Level 5

JoinedDec 18, 2007Messages205

Rheias said:

 > and i still make sth wrong with the 'loop's and 'if's


What do you have problems with exactly?


Precisely.


Click to expand...


Eeeeer, well now, i dont know which problem it was, but i solved it


i think it was just another error message of world edit which was wrong (showing that endif / endloop is missing)


well thanks to you i can now write my own trigger etc. in JASS


thank you man! you are the best


greetz

#### azalgath

#### azalgath

 Level 4

JoinedApr 8, 2005Messages70

Resources: 6

#### Resources
Maps 6

Very useful, this isn't my first programing language so I picked it up pretty quick. Thanks for the basics .

Found a couple of spelling mistakes:

 By know you should understand the taking part of function


Click to expand...

Should be now

 will cause World Editor to crush, like forgetting to put Endif


Click to expand...

Should be crash

#### KaiserG

#### KaiserG

 Level 6

JoinedFeb 12, 2008Messages207

going again to the C and JASS discussion... i can really say that JASS is like PHP, using functions, variables, and arrays.


Yes, i know, PHP is FAR more advanced and has a bigger flexibility. But it seems to me (now that i wanted to learn JASS) that JASS is really easy to learn if you actually have PHP knowledges (and i think viceversa).


I know that most of you will say that theyre not like each other, i know that web pages and war3 maps are different ¬¬ but the point is on the syntaxes and usage of the code.


As far as i know, C language is some kind of base for PHP, so thats why i think theres a close connection between C and JASS. Anyways, im conscious that they have differences for sure (PHP can use multidimensional Arrays for example)

#### ZeRO.ToLErAnCE

Z

#### ZeRO.ToLErAnCE

I think that even though a few things are wrong with it (nobody writes a perfect tutorial the first time), it still gives people transitioning from GUI to JASS an excellent path to follow.


+Rep.

#### forzamoto

#### forzamoto

 Level 9

JoinedApr 14, 2008Messages191

Resources: 13

#### Resources
Maps 8Tutorials 5

WoW i love you ur so GOOD

#### Vercas

#### Vercas

 Level 21

JoinedDec 9, 2007Messages3,096

Resources: 12

#### Resources
Icons 6Tools 1Spells and Systems 5

For who doesn't want to learn JASS, better read this about GUI!

What?!? Why JASS, and not GUI?!? I just created a spell like Omnislash from DotA just from GUI functions, and i'm working on a spell like Charge right now!!! I am shure I have no bugs on my spells, because i tested them 100% ! Almost anything is possible with GUI, and it's easyer to copy and transform than JASS, I tryed 3 tutorials for jass, but I learned nothing


Better, for who prefers GUI, use your mind and you can do anything with it, just by being patient and reading carefuly the functions and the instructions writen with gray down of the function window ... Please remember this, I will soon put my spells on a map, then I will upload it! Wait 5 days since I post this message, because I want to make a couple more spells, then I upload them!


One more thing, have you ever seen what happends when you select your map in Wc3 if it has any JASS trigger?It doesn't show the players, and you can't start it, just by a little mistake like a wrong character ...

#### Dreadnought[dA]

#### Dreadnought[dA]

 Level 12

JoinedFeb 23, 2007Messages1,030

Resources: 1

#### Resources
Tutorials 1

Vercas I used to be just like you. Why move to JASS? I now consider myself an idiot for ever doubting that JASS is better. How many units can you have charge at once? How many can Omnislash at once?

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

vercas said:

 What?!? Why JASS, and not GUI?!? I just created a spell like Omnislash from DotA just from GUI functions, and i'm working on a spell like Charge right now!!! I am shure I have no bugs on my spells, because i tested them 100% ! Almost anything is possible with GUI, and it's easyer to copy and transform than JASS, I tryed 3 tutorials for jass, but I learned nothing

Better, for who prefers GUI, use your mind and you can do anything with it, just by being patient and reading carefuly the functions and the instructions writen with gray down of the function window ... Please remember this, I will soon put my spells on a map, then I will upload it! Wait 5 days since I post this message, because I want to make a couple more spells, then I upload them!

One more thing, have you ever seen what happends when you select your map in Wc3 if it has any JASS trigger?It doesn't show the players, and you can't start it, just by a little mistake like a wrong character ...


Click to expand...

Poor noob

Yeah, thats only vJASS, which is the most common form of JASS spells nowadays

You cannot open the trigger editor of a vJASS-ed map, unless you want it to bug on you

You probably won't believe me until you learn more about JASS, but GUI is f*cking retarded in comparison, many many many things can be done far far far more efficiently with JASS functions and not GUI fluff

I'm going to post a systems spell-map in a few months. If you happen to see it, it will probably blow your GUI-restricted mind

EDIT - Sorry if that was mildly insulting, I can help you out with learning if you are willing to take the time

#### Dreadnought[dA]

#### Dreadnought[dA]

 Level 12

JoinedFeb 23, 2007Messages1,030

Resources: 1

#### Resources
Tutorials 1

JASS is so superior in every way it's not even funny. It's 5x easier to make spells. They don't lag. Have access to things not available in GUI.

call DestroyGroup(noobs)

 You must log in or register to reply here.


### Similar threads

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

 JASS: A Better Understanding


Ghan_04

May 31, 2008

Scripting Tutorials

Replies15Views10K

Jun 20, 2009
Foopad

 Beginning JASS Tutorial Series


wyrmlord

Feb 16, 2007

Scripting Tutorials
234

Replies181Views124K

Feb 18, 2025
Eastern Programmer yahoo

 JASS: A Concise Introduction


Ghan_04

Feb 27, 2008

Scripting Tutorials

Replies19Views14K

Jun 20, 2009
Foopad

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