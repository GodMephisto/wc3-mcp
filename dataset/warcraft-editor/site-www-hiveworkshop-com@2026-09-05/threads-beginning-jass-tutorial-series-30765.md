---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/beginning-jass-tutorial-series.30765/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Beginning JASS Tutorial Series
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/beginning-jass-tutorial-series.30765/>
#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Contents

Overview

Introduction to JASS

Lesson 1: A Simple Function

Lesson 2: Variables

Lesson 3: Functions

Lesson 4: If/Then/Else and loop structures

Lesson 5: Triggers in JASS

Lesson 6: Memory Leaks

Lesson 7: Starting a Spell in JASS

Lesson 8: Continuing the Spell


This tutorial series is being made by:


-darkwulfv


-moyack


-wyrmlord

Overview


Have you wanted to learn JASS, but haven't really had enough tutorials to walk you through the basics and beyond? The aim of this series of tutorials is to provide many tutorials to help a new user start from scratch and after going through the tutorials (and practicing a bit after) to have a fairly good understanding of JASS. "Lessons" are meant to be fairly small, and have only a few topics explained at once in an attempt to not overwhelm people trying to learn.


If anything in any of the tutorials isn't explained clearly enough, please say so. We want the tutorials to be as informative as possible, and an unclear point could ruin an entire lesson.


This tutorial series was originally posted at wc3campaigns. We decided to submit the tutorial series here on The Hive because we just recently found out that Daelin was unable to continue his JASS Classroom, so we thought it would be a good idea to have somewhere else for people to look to learn more than what Daelin had taught.


If you are interested, the attachment at the bottom contains all the tutorials in either .doc or .html format.


Good luck, and happy JASSing!

Introduction to JASS


Tutorial 1: Welcome to the world of JASS.

What is JASS?


-JASS is the programming language used by the Wc3 engine.

What is GUI?


-GUI stands for Graphic User Interface. It is the default crap used by the trigger editor, made to make triggering easier. Unfortunatly, Blizzard made it harder. GUI is bad. It uses BJ's (which will be covered later on), inneficient code, and is overall messy and limiting.

Why use JASS? Can't I just use GUI?


-Sure, you can use GUI. It's not recomended anymore, but it's not dead or nothing.

Here are the Pros of JASS:


-Easier to write out


-Faster, more effecient, and no leaks (when done properly that is)


-Breaks many restraints created by GUI


-Doesn't use BJ's (which again is possible when done correctly)


-Neater, easier on the eyes for longer triggers


-Easier to copy/paste, and can be edited in a text editor or 3rd-party editors (such as JASScraft)


-Generally faster to do when you get the hang of it. Much less clicking around.


However, you must be careful with JASS. As great as it is, it must be used correctly. One single mis-type can screw up an entire trigger.

Will it be hard to use?


-If you've programmed anything before, JASS will be much easier. Even if you haven't, its still pretty easy. It'll take some getting used to, and can be frustrating. You may even feel like reverting back to GUI, but don't. This is worth it. Very complex things will obviously be harder, but we're gonna start you off easy.

How do I get JASS in my editor?


-Make a trigger and name it. Then go to Edit -> Convert to Custom Text. This cannot be reversed. Now edit away!

I'm having trouble typing JASS in the WE!


-Use a 3rd party program for some help. JASScraft and Silly JASS Utility are 2 I would suggest for anyone. Both can be found on the site. JASScraft is perfect for editing your new JASS, since it gives function names, native lists, and Take/Return values, plus a good syntax checker, and more. Definatly a must-have.

When I try to save or test, I get a ton of errors!!


-You probably have a typo or two somewhere, or an unexistant function. Check all your code and JASS is VERY case sensitive. If you type an "A" where an "a" should be, you will get an error. Also, check to make sure there aren't other errors. JASScraft can assist you here. That's why we recommend editing your codes in JASScraft. Much much much easier.

So what's the first step to learning JASS?


-Forget everything you know about GUI, except for globals. JASS is far different from GUI, and trying to compare them is a bad idea. However, a good first step is to make some triggers in GUI, then make them JASS. Throw them into JASScraft and have a ball converting BJ's to natives and cleaning leaks. You'll learn fast. It definatly helps the crossover from GUI to JASS.

Ok, I know the basics. Now what?


-If you want to learn more, read ahead to tutorial #2, by Wyrmlord. He'll teach you the basics of functions. Or, you can go ahead and take it alone from here. The choice is yours. Either way, I wish you good luck!


~Darkwulfv

Lesson 1: A Simple Function

Overview


In this lesson, I will go over with you a very simple function example. I will explain everything you will need to know about it and then at the end I will give you a 'challenge'. This challenge contains some content from the next section, so you will be able


to find out your answer in Lesson 2.


Now that you've looked at an introduction to the advantages of JASS, you're probably eager to get started with some code, right? Now, forget GUI (regular 'triggers'), JASS is different from GUI, so I'll be treating


it that way. Now, open up a text editor of some sort such as Notepad (windows) or some other program


depending on your operating system.


Now, I'm going to show some code. It will be a bit confusing at first, but I'll explain it throughout this lesson.

 JASS:

function HelloWorld takes nothing returns nothing
 call BJDebugMsg("Hello World")
endfunction


Now, don't put this into the editor yet as it's not complete. First off, I'll start with the first line.


JASS, like many programming languages, is made up of a series of functions. I won't go into detail


on that yet, but for now just know that you are declaring/creating a function.


Now, the next line:

 JASS:

call BJDebugMsg("Hello World")


This particular line 'calls' a function. Functions are like actions if you've used GUI before, they execute a command. This particular function puts the text "Hello World" (without the quotes) onto the screen.

 JASS:

endfunction


Now, the final line is pretty simple, when you declare/create a function it needs to end somewhere, right? This line just tells you that the function ends here.


Now, I've gone over with you a very basic example of JASS, if you wish to test this function in the World Editor, simply:


-Create a new map


-Go to the trigger editor


-Click on the map icon at the top of the list of triggers


-Paste the code in the custom script section.


Now, create a trigger (sorry for the GUI reference), you can make the event whatever you want, I'd suggest that you make it go off whenever you press an arrow key. Now, for the action, just find custom script, and in the box


that it gives you, type:









 JASS:

call HelloWorld()


And there you have it, you can now test the function in the game.


Notice how you call a function in that line. I'm now going to go over the function declarations a bit.


When you call a function, you type 'call <function_name>( <parameter1>, <parameter2>, <parameterN> )'


What this basically says is that you type call, while is followed by the name of the function and parenthesis '()'. If the function needs certain info (known as parameters) you put that specific info inside the parenthesis. Each set of 'info' is separated by a comma ','


This particular function only needed a string (message) to display on the screen. Here's some more info:

 JASS:

function HelloWorld takes nothing returns nothing


When a function has the text 'takes nothing', that means you don't need anything in the parenthesis when calling it.


The return value is a value that the function returns. Like the CreateUnit function returns a unit. If you're confused, don't worry as this will be covered in a later lesson.


Now, feel free to mess around with the HelloWorld function, add more calls to the BJDebugMsg, show whatever text you want. Let me say that for now, the text should be inside quotation marks "like this"

Challenge #1: Modify the HelloWorld function to print out other info:


Here's an example:

 JASS:

function HelloWorld takes nothing returns nothing
 call BJDebugMsg("Hi, my name is Bob.")
 call BJDebugMsg("I am 2 years old!")
 //And so on, by the way you can add comments to your code like this
 //Basically if you have '//' somewhere on a line, the rest of the line (after the '//') counts as a comment
endfunction


This may seem like a waste of time, but the more you use JASS the more familiar it will become.


Good Luck -wyrmlord

Lesson 2: Variables


By: Darkwulfv

What is a variable?


-A variable is sort of like a place holder for a handle or other object. There are 2 types of variables. Local and Global.

What is the difference between Local and Global variables?


-Local variables are variables only usable by the function they’re declared in. These are used for cleaning Memory Leaks (which will be explained later), neatening code, and making script Multi-instanceable.


-Globals are variables which can be accessed by any function, but can also be changed by any function. This can be bad for multi-instanceability (which will now be referred to as MUI), depending. MUI will be discussed later on.

How do I create a local variable?


-There are two ways to creating/setting a variable.

 JASS:

 function variable_test takes nothing returns nothing
 local unit u = CreateUnit()
endfunction

This method creates and sets it in one line, at the beginning of the function.

 JASS:

 function variable_test takes nothing returns nothing
 local unit u
 set u = CreateUnit(…..) //Creates a unit and sets the variable to it
endfunction

And this method creates a blank variable, to be set later on in the function.

Hey wait! I want the value to change, does that mean I need a new variable?


-No, not necessarily. All you need to do is put “set your_variable = newvalue


Here’s an example.

 JASS:

function variable takes nothing returns nothing
local unit u = CreateUnit(…)
//Other actions
set u = other_unit //other_unit would be some other unit, don't use this example literally!
endfunction


Remember though, that old value isn’t set to that variable, so if you want 2 different units, use 2 different variables.

Ok, so I have my local variable. Now what?


-Now, use it! Here’s an example.

 JASS:

function KillUnit takes unit u returns nothing

-If you notice, KillUnit needs a unit. Now, you can fill this with many things, like GetTriggerUnit(), GetSpellAbilityUnit(), or any function which returns a unit. However, a variable can be a unit, so you can put that there instead. Here’s a side-by-side comparison.

 JASS:

function variable_testA takes nothing returns nothing
 local unit u = GetTriggerUnit()
 call KillUnit(u)
endfunction

function variable_testB takes nothing returns nothing
 call KillUnit(GetTriggerUnit())
endfunction

They both work, it's up to you to choose which way you prefer.

 Naming variables


 When naming variables, you can't just use any name. First off, the only characters you can use are any letter in the alphabet, any number, and the underscore character (_). The first letter in a variable name must be a letter, after that you can use numbers, letters, and underscores all you want. Let me make it clear that you can not use spaces in a variable name.


What about global variables?


-Global variables are created through the Variable Editor, inside the Trigger Editor. These variables can be used in any function, unlike locals. Example:

 JASS:

 function A takes nothing returns nothing
 call KillUnit(udg_u)
endfunction

function B takes nothing returns nothing
 call KillUnit(udg_u)
endfunction

This is basically what it means. A variable starting with “udg_” usually means it is global. You can do this sort of thing with locals, but it isn't advised. If you need to refer to the same unit in a different function, there are methods of doing so which will be discussed later. The names of the variables you create are udg_variable_name. Each space in the variable name you enter in should be referred to with an underscore '_'

You mentioned Memory Leaks, what are those?


-Although these will be discussed in full later, it is good that you know now.

 Edit by wyrmlord


 Whenever a variable, handle, or string is created, the game must set aside space for it to use. To get rid of that space, there are various methods you can use depending on the object. For example, if you had a special effect, you could destroy it and free up any space it was using. Memory leaks occur when an object isn't able to be destroyed, and it will stay in memory until the end of the game. Another example: suppose I create another special effect, and no variable is set to it. The special effect will use up memory (even if it can't be seen), and since it's unable to be destroyed, it will continue to use memory until the end of the game. Local variables that aren't integers, reals, or boolean are like this too. The fix to them is simply setting them equal to null at the end of a function. Strings use up memory as well, but you can't do anything to stop them from doing so.


So how do I fix Memory Leaks?

 Edit by wyrmlord


 Memory leaks will be gone over in a later tutorial. If you're eager to learn about them, by all means skip to the lesson on memory leaks. However, I wouldn't suggest doing any major amounts of coding until you know more about them


I saw an option to make the variable an array. What’s that mean?


-An array is a variable with more than one option, per say. It’s like an index of that variable type, sort of like a library. To access the variables, you must set them to an index number, then retrieve them as such.


Example:

 JASS:

 function variable_test takes nothing returns nothing
 local unit array u
set u[1] = GetTriggerUnit()
set u[2] = GetSpellTargetUnit()
 call KillUnit(u[1])
 call KillUnit(u[2])
set u[1] = null
set u[2] = null
endfunction

There is an easier way to assign index numbers and null them. It involves integers and loops, and will be shown during the discussion of loops.


Functions can also be fit into those brackets [] to set it to a number; any function that returns an integer or real should cover it.

 Edit by wyrmlord


 This is just to have multiple explanations of the same thing, for possibly confused readers. Arrays are basically a lot of variables squeezed into one. To be exact, 8192 variables. To declare an array in JASS, you would do something like this:

 JASS:

local integer array int_array

The difference is really just adding the word "array" before the variable name. You are unable to set the array to a value on the same line that you create it, keep in mind. Now, you'll need to know how to access each of the 8192 parts of the array. You do that like a normal variable, only you have brackets after the variable with an integer between them, which would be a number from 0 to 8191. If that's a bit confusing, here's an example:

 JASS:

local integer array myIntegers //declaring the integer array
set myIntegers[0] = 5 //Setting the 0th slot in the array to 5
set myIntegers[1] = myIntegers[0] + 2 //Setting the 1st slot in the array to the value in the 0th slot in the array + 2
set myIntegers[myIntegers[1]] = 2 //This is just showing that you can use a variable inside the brackets as well, it doesn't have to be an array though

I would not recommend using arrays for a purpose as small as this. An array contains 8192 values, no more and no less. For something like this it's like killing a fly with a bazooka. (In other words, don't use arrays unless they are actually needed)


There you have it! Variables in a nutshell. If you want to see a list of all the variables, open up either common.j in the Patch.mpq file. Or if you're not sure how to do that, get a tool like JassCraft, and on the right side of the screen when you open it up is a search box and a long list. On the bottom of that list is "show options". Click that, and then uncheck everything in the "search for" box except types. You can now click "hide options" and search through the types of variables.

 Edit by wyrmlord



There are a few main variable types: integer, real, string, boolean, and handle. I'm sure you know what an integer is, it's a whole number (including negatives) such as: 1, 145, -37. A real is a number with decimal places such as: 3.14, -15.78, 11.1134. A string, as you might already know can be thought of as a message or text: "Hello", "This is a string". A boolean has two values: true or false. A handle, you could say, is a reference to an object such as a unit. You don't direct use the handle type usually, but you use types that extend the handle type. This basically means that they are a specific type of handle. A handle is pretty much anything that isn't an integer, real, string, boolean. Handles include: units, special effects, timers, groups, players, and so on.


CHALLENGE:


Can you fill in this code with the correct variables? (this does not involve arrays)


The “_”’s represent where something needs to be filled in.

 JASS:

 function variable_challenge takes nothing returns nothing
 local ____ _ = GetTriggerUnit()
 call KillUnit(_)
set _ = ____
endfunction


Good luck, and happy JASSing!


~Darkwulfv

Lesson 3: Functions

Overview


In this lesson, I will be going over how to:


1.Create functions


2.Call functions


3.Anything else I can think of

Part 1: Creating a function


Now, you're probably wanting to know how to create functions, right? Well, to create functions, you first need to tell the program that you're creating a function, so you do that by delcaring a function. This is basically telling the program the function's name, what values it needs to get when called, and what it returns. Take a look at this function:

 JASS:

function DisplayAMessage takes string message returns nothing
 call BJDebugMsg(message)
endfunction


Now, the word 'function' tells the program that you will be declaring a function. The next part of the function is the name of the function. You need to make sure your function has a unique name that no other function has, otherwise you'll get errors when saving a map. Next, you tell the program what the function 'takes' or needs pretty much. This function 'takes' a string, which is basically a message of some sort. After you put the type the function needs, you then have to put a name for it (so you can refer to it inside of your function). Lastly, you tell the program the type the function will return. You could make this a string, integer, real, and so on. However for this example, no value will be returned so we put 'nothing'.


After declaring a function, inside the function you need to put what the function does, otherwise it would be pretty useless. This function simply calls another function using the string value that is needed to call it. Lastly, you need to tell the program when the function ends, you do this by putting 'endfunction' at the end of the function.

Part 2: Calling functions


Now, that may not all make sense, so I'll now explain how you call functions, which should hopefully clear things up a bit. First off, you can call a function in a few different ways, but I'll start by the easiest way, which is using the word 'call'.

 JASS:

call DisplayAMessage("hello")


This line would call the function I just showed you. When calling a function this way, you need to have the word 'call' followed by the function name which is then followed by parenthesis with the values the function takes inside of them. If a function takes multiple values, they are each separated by a comma ','. It works that same way when declaring functions too (whoops forgot to mention that earlier =P)


Now, I will go over what the return values in functions are. They're pretty simple really, though I don't want anyone to be confused about anything. When a function has a return value, you can set variables that are the same type as the return value to that function. Here's an example:

 JASS:

local string player_name = GetPlayerName(Player(0))


Now, see that when you set a variable to the return value of a function, you don't need the word 'call'. Also, if you want to pass the return value of a function into another function call, you don't need to put the word 'call' either, as seen with the Player function that was the needed parameter in the GetPlayerName function.


If when and when not to use call are confusing you, let me just say you can only have 1 call in a line, and call can't be used if you have another word before that such as 'set' or you're declaring a local variable.


Also, and this is important when declaring a function, if the function declaration states that the function returns a value, somewhere in the function if must have a line that 'returns' the value, like so:

 JASS:

function TestReturn takes nothing returns integer
 return 5
endfunction


When a function reaches the line with return on it, it returns the specified value and ends (no code below that line is looked at by Wc3)


There is one last thing you need to know when calling functions, you can only call a function that is above another, since that may not make complete sense, here's an example:

 JASS:

function SomeFunction takes nothing returns nothing
endfunction

function SomeOtherFunction takes nothing returns nothing
endfunction


Now, this is only used as a visual example so I can more easily describe what I was saying. You can only call a function that is above the function you are currently in. So, 'SomeOtherFunction' would be able to call 'SomeFunction'. However, 'SomeFunction' wouldn't be able to call 'SomeOtherFunction'.

Challenge Time!


Now, I figure you're wondering why the challenges so far aren't very hard (I'm just assuming that), and that's because we haven't taught you enough for us to really stump you on something. I hope this challenge will make you think more than normal. The following code has a few errors in it, find each error in it. There are 5 errors in the following code, can you find them all?

 JASS:

function Function2 takes nothing returns string
 local string message = SomeFunction("Hello World")
endfunction

function SomeFunction takes string returns nothing
 local string a message
 a message = 'Something is wrong'
 call DisplayTextToPlayer(Player(0), 0, 0, a message)
endfunction


Good Luck -wyrmlord

Lesson 4: if/then/else and loop structures


Tutorial No 4. if/then/else and loop structures. By Moyack.


These kind of structures are the most important in any programming language. In JASS, these structures are quite versatile and useful.


Let's see the first structure: IF/THEN/ELSE


To work with it, we can write it in this way.


if/then/else structure

 JASS:

if <condition 1 function or variable> then
 <Condition 1 script>
elseif <condition 2 function or variable> then
 <Condition 2 script>
...
else
 <Else condition script>
endif


Now you understand why I said versatile


As you can see, the conditions must be a function which has to return a boolean variable type (true/false) or a boolean variable. Other interesting feature with the if/then/else structure in JASS, is that it can be used as a multiselection structure too, something very useful in many cases.


In order to ensure the understanding of this structure, This is a small example showing the IF/THEN/ELSE in action


IF/THEN/ELSE example

 JASS:

function Set_Damage_To_Unit takes integer level returns real
 // this example will set the damage amount in a spell, according to a level
 // (unit level, ability level, etc)
 local real dam
 if level < 2 then
 set dam = 0
 elseif level == 1 then
 set dam = 50.
 // probably you noticed the point after the number. this is used to indicate the number is a real one.
 // Is a good programming practice to do that in order to avoid annoying debugging mistakes.
 elseif level == 2 then
 set dam = 63.
 elseif level == 3 then
 set dam = 125.
 else
 set dam = 125. + 50. * (level - 1)
 return dam
endfunction


Here we are using a local variable which will have a value according to the input argument in the function, and then that variable will be returned by the function.

 Important note


 If any IF or ELSEIF clauses apply, then the ELSE clause will be avoided. This example function has a bug by purpose, in order to show an IF/THEN/ELSE characteristic. if level = 1, then the first clause will be executed (set dam = 0). Then the second condition will be executed and will be true too (elseif level == 1), then it will execute the second clause (set dam = 50). With that we can see that the IFTHENELSE structure can be used to make data filtering, for instance.


 Another Important Note


 The '=' character is used when setting a value, the '==' operator is used when comparing values. In other words, don't do: if x = 5 then... because Wc3 can't read your mind as to what you want it to really do. This line is saying to the computer "if (set x = 5) then..." and I don't think that makes much sense, how about you?


Now let's see the LOOP structure.


Loops in programming are used to make a set of instructions to be executed several times, loops can be classified by finite and infinite. The finite loops are which have a defined a fixed number of repetitive executions, controlled by an exit control. The Infinite loops in the other hand, will execute the script contained in it indefinitely.


The loop structure is as following:

 JASS:

loop
 //Here you can put code too :)
 exitwhen <condition function or variable>
 //your iterative script
endloop


This structure is quite flexible as you can see, because you can put the exitwhen condition in ANY place inside the loop. Let's see an example.


Loop Example

 JASS:

function Heal_Unit_Group takes group g returns nothing
 // this example will heal all units in a unit group.
 local unit u // a local unit variable
 loop
 set u = FirstOfGroup(g) //Selects the first unit in the group.
 exitwhen u == null // the loop will finish when the group will be empty,
 // in tother words, when the FirstOfGroup() function returns null
 call SetUnitState(u, UNIT_STATE_LIFE, GetUnitState(u, UNIT_STATE_MAX_LIFE))
 //sets the life of the selected unit at its maximum.
 call GroupRemoveUnit(g, u) // removes the unit from the group
 endloop
 call DestroyGroup(g) // to avoid group leaks, we must destroy the group variable.
endfunction


The usage of FirstOfGroup and group functions inside a loop is very common in AOE spells, with them we can do several things to a unit group.

 Some tips with loops



Sometimes, we want to use the loops as a way to do a mass effect spell. If you want to add special effects inside a loop, there is a big chance the code will lag, in order to avoid that, you should use a TriggerSleepAction() function, which will give to the WC3 engine a small break to process other tasks.


You can create an infinite and unstoppable loop using only the loop and endloop instructions without an exitwhen command.


 Exercise.



Create a function which displays all the numbers which can divide perfectly a given number. The function should be in that way:

 JASS:

function DivideNumbers takes integer n returns nothing
 // your code here
endfunction


For exaple, if we make









 JASS:

DivideNumbers(12)

 we should see on the game screen something like this:


1, 2, 3, 4, 6, 12


Good luck :

Lesson 5: Triggers in JASS

Overview


Now, if all you've ever known is GUI, you're used to everything being a trigger. GUI is just a system of triggers, but you're probably wondering what exactly is a trigger. This lesson will cover triggers in JASS and how to use them. First, I'd like to tell you exactly what a trigger is.

What is a Trigger?


I'm sure that you know what a trigger is if you've used GUI before, but GUI hides some important things from you. A trigger is really just another object, like a unit or a special effect. What makes triggers useful is that they are able to detect events. Without triggers, you couldn't do too much in the editor without a lot of trouble. Now, here's an example of making a trigger in JASS:\

 JASS:

function InitTrig_JASSTrigger takes nothing returns nothing
 local trigger myTrigger = CreateTrigger()
 call TriggerRegisterPlayerEvent(myTrigger, Player(0), EVENT_PLAYER_END_CINEMATIC)
 call TriggerAddAction(myTrigger, function MyFunction)
 set myTrigger = null
endfunction


Now, this may be a bit overwhelming at first, but I'll explain everything well enough so that you should understand. First off, you're going to create a local variable for the trigger. This is fine if you don't need other triggers to run TriggerExecute on this trigger (TriggerExecute is a function that runs a trigger). Now, whenever you create a new trigger, you must use the function CreateTrigger(). It's like using the CreateUnit function to create a unit. Now that you have created a trigger, you'll need to add some things to it so it would be useful. The next line adds an event to the trigger. TriggerRegisterPlayerEvent takes 3 values, which are the trigger you want to add the event to, the player this event applies to, and the specific event. There are many other TriggerRegister... functions which you can experiment with later, but for now this will be fine. The next line adds an action which is basically telling the trigger to call that function when the event happens. Now, I didn't show you the function MyFunction, but you need to know that it must take nothing and return nothing. Now, the final line sets the local variable to null, which is necessary for local variables which are handles so that memory leaks can be prevented. If you don't know what a memory leak is, don't worry about it, they will be covered in a later tutorial.


Okay, now you're probably still using that GUI trigger to run your test functions in the custom script. This JASS trigger does pretty much the exact same thing as the GUI trigger, so you can replace it. Just create a new trigger, and then go to Edit -> Convert to Custom Text. What you will see is something similar to the function shown above, only it's setting a global variable automatically generated by the editor to a trigger and just adding an action. Delete all the code and make a new function that has the name: InitTrig_[insert trigger name here]. So if the name of your trigger was JASSTrigger, you would have your function be: function InitTrig_JASSTrigger takes nothing returns nothing It's important that you name it this way so that it gets called while the map is loading.


Now, if you've noticed, I've left out how to add a condition, oh my! Well, I'll show you how adding conditions work in triggers. Don't convert a GUI trigger to JASS to see how to do conditions, that way is inefficient. There's more code than you really need to use. Let me show you an example:

 JASS:

function Condition_Function takes nothing returns boolean
 return GetTriggerPlayer() == Player(0)
endfunction

function InitTrig_JASSTrigger takes nothing returns nothing
 local trigger myTrigger = CreateTrigger()
 call TriggerRegisterPlayerEvent(myTrigger, Player(0), EVENT_PLAYER_END_CINEMATIC)
 call TriggerAddCondition(myTrigger, Condition(function Condition_Function))
 call TriggerAddAction(myTrigger, function MyFunction)
 set myTrigger = null
endfunction


This might look confusing, it might not. Either way I'm going to review what everything does. First off, I'll review how to set up a function so it can be used as a condition in a trigger. First off, you can name the function whatever you want, but it has to take nothing and return boolean. A boolean value, as you should know, is true/false. This function returns GetTriggerPlayer() == Player(0). You might look at that and be clueless, others might understand it already. The first part gets the player who triggered the event (the player pressing escape). And finally, the == Player(0) part checks if the player is Player(0) (Player 1 (Red)). Since this is a boolean statement, meaning it is either true or false, it can be used as a return value.


Finally, we have the added line in the InitTrig_JASSTrigger function. The function TriggerAddCondition takes a trigger and a boolexpr (boolean expression) . You don't need to know what a boolexpr is yet, all you need to know is that you put Condition(function [insert trigger name here]) and it returns a boolexpr which is used for conditions in triggers.

Challenge


This challenge will require a bit of researching JASS functions, so I recommend you get a tool such as JassCraft. Well, I'll give you three possible challenges. The first challenge will to be to create a trigger in JASS that will run a function that you create (any). After you've messed with that a bit, open up JassCraft or a similar program where you can search JASS functions. You're next challenge will be to create a trigger that will go off whenever you type in "-run" or whatever kind of text message in-game. I'll give you some help in telling you that the function you're looking for to add the event is TriggerRegisterPlayerChatEvent, however it's up to you to figure out how to use it. If you've solved that challenge, then make a trigger that goes off when the message a player types contains the character "-" or whatever letter you choose. In the actions, you will then display the value the message the player typed in. Hints for this challenge are keep using the same function to add the event, only you need to change a value or two. And I suppose it would also be helpful to tell you that the GetEventPlayerChatString can be used to get the string that the player types in.


Good Luck -wyrmlord

Lesson 6: Memory Leaks

Lesson 6: Memory Leaks


By: Darkwulfv, edited by wyrmlord

What is a memory leak?


-A memory leak is caused by bad coding, where a handle that was stored in memory is lost from refrence. Basically, it’s something that got placed in wc3’s memory usage and never got removed, and is unable to be removed.

What does a memory leak do?


-A memory leak adds a little tiny bit of memory usage to Wc3 when it’s running. If enough memory leaks, your map is going to cause some serious lag.

What causes memory leaks?


-When a handle is created, it's stored it in Wc3’s memory. If it is not destroyed or removed when you are done with it, then it stays there, taking up memory.

Well how am I supposed to fix them?


-Well, there are a few ways to do so, depending.


Method #1:


=============================

 JASS:

function MemoryLeak takes nothing returns nothing
 call AddSpecialEffect(…) //Creates a special effect
endfunction

=============================


The native AddSpecialEffect(…) creates an effect. If you do not destroy this effect with the proper function, it will stay embedded in the memory, and thus take up memory. Now, one leak is terribly insignificant. But if your map made a lot of leaks very often, you would notice a big change.


Here’s a way to fix that leak:

 JASS:

 function MemoryLeak takes nothing returns nothing
 local effect my_effect = AddSpecialEffect(...) //Create a special effect
 call DestroyEffect(my_effect) //Destroys the effect, removing it from memory
 set my_effect = null //You have to null local handles at the end of a function, or they leak too
endfunction

//Alternatively, this can be used

function MemoryLeak2 takes nothing
 call DestroyEffect(AddSpecialEffect(...))
 //Adds a special effect and destroys it all at once. This will still show the animation once for most (not necessarily all) effects, and doesn't require the use of a variable
 //However, if you want to see the entire animation, you should wait for the duration of the effect's 'stand' animation
endfunction


This method goes for anytime you are using a handle(any variable that isn’t a real, integer, boolean, boolexpr, code, or string)


However, the destroying/removing function changes for each type of handle. A list of variable types and how to remove them will be given later for you to refer to.

NOTE: When destroying/removing an object, you are removing/destroying a specific object and not the variable itself. The variable just points to the location in memory where the object is stored.


NOTE: A leak can be defined as an object that is created, not removed, unable to be removed during the game (assuming not removing it is undesired). If it’s only used once (like dummy units, rects, groups, etc.) then remove it. If not, you should leave it. (Like for spells, don’t remove the caster!)


NOTE: All non-handle variables except strings do not leak. String leaks cannot be fixed, but you usually don't need to worry about them.


So memory leaks only apply when you create things?


-WRONG!


A memory leak not only happens when you create things. It also happens when you make a handle-variable, set it to a value, and don’t null it.


Example:

 JASS:

function MemoryLeak takes nothing returns nothing
 local unit u
 //actions
endfunction

This example wouldn't leak, as the handle variable wasn't set to a value. However, if it had been set to a value, like:

 JASS:

local unit u = GetTriggerUnit()

It would leak. To fix this, set your variable to null. (Reals, integers, and booleans cannot be nulled, and don't need to be as the memory they use is automatically removed)


To null a variable, simply put


set yourvariable = null


You don't need to set a variable to null after removing each object. You only need to null it once; this is usually done at the end of a function.

 JASS:

 function MemoryLeak takes nothing returns nothing
 local group g = CreateGroup()

//Some code

 set g = null
endfunction

NOTE: To satisfy those wanting to know why handle variables leak (not referring to the object, I'm referring to the variable). When you set a handle equal to a value, the handle points to where that value is stored in memory. It will remain pointing to where that value is stored unless it is set to null (meaning 0, not pointing to a specific place in memory.)

What about globals?


-Since these are always in the memory to begin with, and are always available, they don't count as leaks (and don't need to be nulled).

So uh… What do I do about non-handle variables?


-They don’t leak though, so worry not.

Conclusion:


-Always clean leaks. If you don’t, and there’s enough leaks, you will notice the effects.

Challenge!


-Clean this function of all memory leaks:

 JASS:

function ChallengeTrigger takes nothing returns nothing
 local unit u = GetTriggerUnit() //This unit is in the game, it needs to be kept (hint)
 local effect SFX = AddSpecialEffect(…)
 local group g = CreateGroup()
 local unit u2 //this is going to be a dummy unit and should be removed (hint)
 local real r = 4
 //actions
 //(hint) only one of these variables isn’t nulled.
endfunction


Good luck!

#### Attachments

JASS Tutorials.zip

 31.8 KB

 · Views: 1,637




 Last edited: Oct 5, 2007

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Update

Lesson 7: Starting a Spell in JASS

Overview


Welcome to the next lesson. The basics have pretty much been covered, and now you'll probably want to use JASS to create spells or something. In this lesson, we'll go through the process of making a spell similar to the Omnislash spell found in DotA. (I've never actually seen the spell, but I know somewhat of how it works) This will show you some basic things used in JASS and room will be left for you to be able to modify the spell later on.

Starting Off


Before making a spell, you'll have to consider how it will work, what is happening step by step. First off, we need the trigger to go off when a unit casts a certain spell, right? And then we can begin thinking of the actions. In the spell I'm thinking off, the hero moves around in an area hitting a certain amount of units, but never the same unit twice. If you're looking at the spell being cast in-game, it might seem confusing, it might not. You have to break things down little by little. Here's some code for now that will start us off and I'll add to it as the lesson progresses:

 JASS:

function Slash_Condition takes nothing returns boolean

 return GetSpellAbilityId() == 'A000' //Compares the ability id of the ability being cast to the ability id of our Slash spell.

endfunction

function Slash_Actions takes nothing returns nothing
endfunction

function InitTrig_Slash takes nothing returns nothing
 set gg_trg_Slash = CreateTrigger() //Set the global variable that is created for our trigger to a new trigger

 call TriggerRegisterAnyUnitEventBJ(gg_trg_Slash, EVENT_PLAYER_UNIT_SPELL_EFFECT) //This BJ function registers an event for whenever a unit starts the effect
 //of an ability for our trigger

 call TriggerAddCondition(gg_trg_Slash, Condition(function Slash_Condition))
 call TriggerAddAction(gg_trg_Slash, function Slash_Actions)
endfunction


So far all we have is the basic trigger set up that will go off whenever the right ability is cast. You need to make sure you've created an ability (preferably based off channel with the values set correctly, if you need to know more about channel, please ask). Now we will create the actual effects of the spell. Now you'll need to think "what happens in the spell". Here's what happens: the hero is moved around within a certain area, he will only be moved to enemies, for each unit he is moved to, he will deal damage, and at the end he will return to the spot that he started at. The first thing to do now is to create a local variable to everything we'll need to keep track of. Here's what I thought of: a unit variable for the hero, a location for where the hero starts at, a group to hold all the enemies the hero will hit, and another unit variable to temporarily hold picked units in. You might not get the point of the last variable, but I'll show you how it's used soon. Here's what we should have now:

 JASS:

function Slash_Actions takes nothing returns nothing

 local unit caster //Will hold our caster
 local location start_position //Holds the starting position of the hero
 local group enemies //Will hold the unit group of enemies our hero will hit
 local unit temp //Used for when going through the group (explained later)

endfunction


Now, if you've used GUI before, you'll probably be wanting to do something like "Pick Every Unit in (group) and do actions", but there's a problem with that. In order to use that function (it's know as ForGroup in JASS), you'll need a separate function that you'll be unable to pass any values to. Now you could use global variables to transfer information, but then the spell wouldn't be able to be used by more than 1 unit at a time. You could possibly think of other ways around this, but they'd take more time than it would be worth. I'll give you a hint on how I do it, I use a loop. "Loop?" you might be thinking, "how can I use a loop?" because if you've used GUI before, the loops would run for a fixed length (for integer # through #, do actions), unlike in JASS where a loop will only end when a certain condition is met. And that condition will be that all enemies in the enemy group have been hit. Here's some more code to hopefully explain a little bit more of what I mean:

 JASS:

function Slash_Actions takes nothing returns nothing

 local unit caster = GetSpellAbilityUnit() //Create a local variable and set it to the unit that's casting the spell
 local location start_position = GetUnitLoc(caster) //Create the local and set it to the caster's position
 local group enemies = CreateGroup() //When you create a group in JASS, it's like a trigger. First you need it empty, and then you add stuff to it
 local unit temp //Used when looping through the group
 local integer count = 5 //This will be the maximum amount of enemies that can be hit

 call GroupEnumUnitsInRangeOfLoc(enemies, start_position, 500.0, null)

// GroupEnumUnitsInRangeOfLoc takes a group, a location, a radius, and a boolexpr (a condition, kind of) and then gets units within that radius and adds them to our group

 loop
 set temp = FirstOfGroup(enemies) //FirstOfGroup returns a unit from a group in no particular order that I know of
 exitwhen temp == null or count == 0

 //The loop will stop when temp is null (meaning that there are no more units left in the group) or when we've hit 5 enemies

 call GroupRemoveUnit(enemies, temp) //Removes the unit from the group so that it can't be picked again
 endloop

 //Cleaning up memory leaks will be put here

endfunction


In this section of code, we've set up some more basic parts to our code. We've initialized the variables and then filled the group with units within 500 range of the caster. Next, we start a loop and for each cycle in the loop, the temporary unit variable will be set to a unit from the group. Then we'll tell the loop to exit when the group has no units left in it or the caster has hit 5 enemies (count would be decreased for each enemy hit). At the end of each loop, the unit being used will be removed from the group so that it can't be picked again (and so prevents an infinite loop as well). Then we end the code executed by the loop and end the function.


The next step will now be to add in what the function will do for each unit picked. The first thing that you will have to notice is that the GroupEnumUnitsInRangeOfLoc grouped all units in 500 range of the hero's location, including allies and the hero. That means we'll have to add a simple condition to check whether or not the unit is an ally before performing additional actions. After that's done, what next? The unit will have to move to each enemy hit and damage them, right? Don't forget setting the count variable to count – 1 as well, or more enemies could be hit than desired. Before going on to the next bit of code, declare another location variable to hold the picked enemy's position, I'll go by the name of temp_loc. Here's what we'll have next for the loop:

 JASS:

loop
 set temp = FirstOfGroup(enemies)
 exitwhen temp == null or count == 0

 if IsUnitEnemy(temp, GetOwningPlayer(caster)) then //If the enemy unit is an enemy of the owner of the caster

 set temp_loc = GetUnitLoc(temp) //Set a temporary location variable to the enemy position

 call SetUnitPositionLoc(caster, temp_loc) //Move our unit instantly to the enemy's location

 call UnitDamageTarget(caster, temp, 50, true, false, ATTACK_TYPE_CHAOS, DAMAGE_TYPE_NORMAL, null)

 //The caster will instantly damage the enemy for 50 damage with an attack type of chaos
 //You can ignore the damage type, and the last parameter (weapontype) is just for sound, but we will
 //have none for right now

 set count = count - 1 //A unit has been hit, so now the units left to hit is reduced by 1

 endif

 call GroupRemoveUnit(enemies, temp)
endloop


Okay, this loop will now allow our unit to move to every unit within range and damage them. You might think we're done, right? Our hero will move to each unit in range and damage them, but you now have to ask yourself if we're missing anything. Are we?

Challenge


I bet you've already guessed the challenge, finish the spell! Don't worry about special effects (unless you want to), you just need to finish up on the basics that will happen and clean memory leaks. In the next lesson, the spell will be finished up and enhanced with special effects and more! Don't hesitate to post code on the forums if you're having trouble.


Good Luck! -wyrmlord

Lesson 8: Continuing the Spell

Overview


First off, I'd like to say sorry for taking so long, but here's finally the next tutorial in the series. In this tutorial, we'll continue off the spell from last time. Just to refresh your minds (in case you haven't looked at these tutorials in awhile like me) we were working on a slash spell where the hero would blink throughout an area and damage enemy units. He would hit up to a specified number of units, or until he had hit every nearby unit. Now to add the finishing touches, first we will rid the spell of memory leaks and add some special effects to make the spell more appealing. After that, we will work on ways to improve the spell further. To improve the spell further, we will look into timers and gamecache usage. However, this lesson will only cover the effects portion as I think you've waited long enough with no response from me and I also want to prevent the lesson from being too large.

Starting Off


Make sure you have the spell being worked on from last time. I'll post the code so you can take a look at it if you've misplaced the code from last time:

 JASS:

function Slash_Condition takes nothing returns boolean
 return GetSpellAbilityId() == 'A000'
endfunction

function Slash_Actions takes nothing returns nothing
 local unit caster = GetSpellAbilityUnit()
 local location start_position = GetUnitLoc(caster)
 local group enemies = CreateGroup()
 local unit temp
 local integer count = 5
 call GroupEnumUnitsInRangeOfLoc(enemies, start_position, 500.0, null)
 loop
 set temp = FirstOfGroup(enemies)
 exitwhen temp == null or count == 0
 if IsUnitEnemy(temp, GetOwningPlayer(caster)) then
 set temp_loc = GetUnitLoc(temp)
 call SetUnitPositionLoc(caster, temp_loc)
 call UnitDamageTarget(caster, temp, 50, true, false, ATTACK_TYPE_CHAOS, DAMAGE_TYPE_NORMAL, null)
 set count = count - 1
 endif
 call GroupRemoveUnit(enemies, temp)
 endloop
endfunction
function InitTrig_Slash takes nothing returns nothing
 set gg_trg_Slash = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(gg_trg_Slash, EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(gg_trg_Slash, Condition(function Slash_Condition))
 call TriggerAddAction(gg_trg_Slash, function Slash_Actions)
endfunction


I haven't commented the code because there's nothing new, and you should already know what it all does from last time. When making a spell, I always like to first get it working and then fine-tune it afterwards. It should already be working, but it still has errors in it because of memory leaks. I expected you to be able to fix these up yourself, but if you need to double-check your answers, here's what I would have done:

 JASS:

 set count = count – 1
 call RemoveLocation(temp_loc) // This line has been added
 endif
 call GroupRemoveUnit(enemies, temp)
 endloop

 // Everything below has been added

 call RemoveLocation(start_position)
 call DestroyGroup(enemies)
set caster = null
set start_position = null
set enemies = null
set temp = null


 Now, if you thought the only errors were memory leaks, you've overlooked something else that needs to be changed with the code. The start_position variable is never used! We needed to return the hero to the location where the spell started being cast. I won't post the code for this, because I'm assuming you should know 100% how to do this. Just remember that you can't move the hero to a location that's been removed, and that the function you're looking for is already used for something else in the code.

Effects


 This now brings us to the next point, making the spell more interesting. A spell where the hero just moves around an area looks rather boring don't you think? Also, you need to take into consideration how the hero moves from point to point and portray that with special effects of some sort. Since the hero is "blinking" we will use the blink model for a special effect. The model path for the blink model happens to be: Abilities\Spells\NightElf\Blink\BlinkTarget.mdl. I got this path from using the object editor. First, I found a random ability (doesn't matter) and then went to the Art – Special Field. After that, I added a new model that was the blink effect. After adding the model, I went to Edit Model and looked at the bottom text area next to the word "Custom:". The path for the model I had just picked was there.


 Now, we will go over adding an effect. The function we will be using to create an effect is the "AddSpecialEffectLoc" function. Here's the function declaration for it:

 JASS:

native AddSpecialEffectLoc takes string modelName, location where returns effect


 This function is pretty strait-forward. You simply give it the path to the model you want to use and the location the effect is to be created at. However, there is one detail I need to go over about the model path. The above path I showed you needs a few minor modifications in order to be used properly. Here's how we would go about calling this function:

 JASS:

set blinkEffect = AddSpecialEffectLoc("Abilities\\Spells\\NightElf\Blink\\BlinkTarget.mdl", effectLoc)


 Notice how I replaced each "\" in the model path to "\\". This is required in JASS to properly represent the "\" character because the "\" character has other uses that I will not go into. Also, noticed how I set the effect to a variable. A very common mistake is creating the effect, not setting it to a variable, and then trying to use the GetLastCreatedEffectBJ function to refer to the effect. This function only works with the function AddSpecialEffectLocBJ which I do not ever recommend using as it has no point other than for allowing a "Get Last Created Effect" action in GUI. Since we are using JASS, we can ignore this function and use the native.

Challenge


Okay, so now we know how to add a special effect, but now we need to take advantage of this newly learned function and add it into our spell. First, we'll need to decide when and where we want the effect to be placed. I think now will be an appropriate time for a challenge. Your challenge will be to add special effects to the spell as you find appropriate. Feel free to experiment with various models and where you place them in the code. Now, I thought I'd give you one last tip before sending you on your way. Effects, like all other handles, leak. Even though the model of the effect may disappear after a few seconds, you'll still need to remove the effect. There are various ways to do so and I'll go over them now.


First, and most obvious, you create the effect and set it to a variable. Later in the function, you call the DestroyEffect function on the effect to destroy the model and get rid of the memory the model was using. Here's an example of that:

 JASS:

local effect specialEffect = AddSpecialEffectLoc("model\\path", someLocation)
// Clean up
call DestroyEffect(specialEffect)
set specialEffect = null


 This creates an effect and sets it to the local variable specialEffect. Later in the function, it is destroyed with the DestroyEffect function and set to null. Another thing that should be noted about special effects is that until it's destroyed, an effect will play its "stand" animation if it has one. When you destroy an effect, it plays its "death" animation if it has one. Here's where the second method comes into play. Some functions only have a single animation, or perhaps you only want to play an effect's death animation. You can both create and destroy an effect on the same line. Most effects will play either their death animation or sometimes their regular animation the whole way through before being removed. However, this may not work on all effects so be sure to test it out. Here's basically how the code would look to demonstrate this:

 JASS:

call DestroyEffect( AddSpecialEffectLoc("model\\path", someLocation) )


 Once again, some models may respond differently than expected with this function so make sure to look things over when using this method. This method is nice due to squeezing many things onto a single line. No other variable for the effect is needed and no memory leak is created from the effect. However, remember that you need to remove the location used in this function as it is not cleaned like the effect is.


 Now, I think this is sufficient information for you to add special effects to the spell. Go ahead and give it a shot, the next tutorial will contain how I added the effects. If you have trouble with adding the effects, make sure to ask as questions help you learn.


Good Luck - wyrmlord


This tutorial is still in progress and I would greatly appreciate suggestions.


 Last edited: Oct 5, 2007

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

The tutorial has been "unquoted" and some syntax errors in lesson 7 have been fixed.

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

IMPORTANT:

Posts in this thread should relate only to the tutorials themselves.

If you have other questions, please ask them in the [self="https://www.hiveworkshop.com/forums/forumdisplay.php?f=271"]JASS Scripting Forum.[/self]


 Last edited: Apr 30, 2007

#### paskovich

#### paskovich

 Level 11

JoinedJul 12, 2005Messages764

Resources: 3

#### Resources
Maps 1Tutorials 2

Wolverabid asked me to to look up the script. Here is what i've got so far:


1.

 When a function has 'takes nothing'


Click to expand...

A function does not "have" takes nothing, it just takes nothing.


2.

 A variable is sort of like a place holder for a handle or other object. There are 2 types of variables. Local and Global.


Click to expand...

 Handles ARE THE objects themselves!


And there are also event-related variables (TriggerUnit, SpellTargetUnit, etc.), and generated variables (gg_trg_<>, gg_rct<>, ...).


3.

 JASS:

 function variable_test takes nothing returns nothing
local unit u = …
endfunction

Click to expand...

To fit the example list, the correct line would be:


local unit u = CreateUnit(...)


4.

 JASS:

function variable_test takes nothing returns nothing
local unit u = GetTriggerUnit()
call KillUnit(u)
call KillUnit(GetTriggerUnit())
endfunction

Click to expand...

I don't like that they're put together in one functions. I'd make separate functions for the two variations.


5.

 Non-handles (strings, integers, reals, Booleans, code) do not, so don’t try to null them.


Click to expand...

You CAN null them, you CAN try it, but you have to use the appropriate value instead of null:


set string = ""


set integer = 0


set real = 0


set boolean = false


Btw, that's true that you don't really need to null them!


6.

 You can create an infinite and unstoppable loop using only the loop and endloop instructions without an exitwhen command.


Click to expand...

Is this possible? I haven't tried it yet. But i would use:


loop


exitwhen false


endloop


7.

 Excersise: Create a function which displays all the numbers which can divide perfectly a given number.


Click to expand...

Here the ModuloInt function has got to be mentioned!! For a beginner, it is impossible without that.


8.

 What is a memory leak?


-A memory leak is sort of self-explanatory. It is when something in Wc3’s memory is left there to rot. Memory leaks have been hailed as one of the most potent map-destroyers.


Click to expand...

This is the worst description i ever heared...


9.

 function MemoryLeak takes nothing returns nothing


call CreateUnit(…)


endfunction


Click to expand...

Units are the worst examples of memory leaks... Special effect are not, instead.


Also where removing/destroying objects is mentioned, i would list which functions are supposed to destroy the most common leaks (groups, locations, etc...).


All together, not bad tutorial.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Like the poster above, I was asked to check this for mistakes. . .

 Method #1:


=============================

 JASS:

function MemoryLeak takes nothing returns nothing
call CreateUnit(…)
endfunction

=============================


The native CreateUnit(…) makes unit. If you do not destroy this group with the proper function, it will stay embedded in the memory, and thus take up memory. Now, one leak is terribly insignificant. But if this were run twice every second in a 60 minute game…


2 leaks per second x 60 seconds = 120.


120 leaks per minute times 60 minutes = 7200 leaks per hour.


7200 x 4 kilobytes of memory per leak = 28800kb = 28.8 megabytes of memory drained.


Still not TOO much. But in maps with a lot of spells being used over and over again, your gonna have problems.


To fix this, simply add this to the end of your function:


call RemoveUnit(GetLastCreatedUnit())


However, this method is somewhat innefecient, and messy for that matter. Here’s a much better way, and cleaner.


Click to expand...


This makes absoultly NO SENSE!


CreateUnit() returns a UNIT not a groupe and thus does not leak,


You are getting confused with CreateNUnitsAtLoc() which still does not leak since just look at the structure of it in jass, it recycles the groupe and so does not leak if im mistaken unless the return makes an copy of the groupe.


Also if it returns a unit that you do not attach to a local it should not leak.


Also RemoveUnit() does not stop leaks!


It removes a unit instantly but since it is a unit. . .


UNITS NEVER LEAK!


Leaks are when unused data is writen to RAM where YOU the map maker have lost hold of it and thus wastes RAM and processing power.


Once data is leaked, you can not unleak it since you do not know what has leaked.


Units never leak since you can always pick them and thus remove them and so they might lag like hell or the entire map is side to side units but still they have not leaked since you can still fetch a reference and remove them.


Also units when they die are eventualy automaticly removed and so do not leak that way.


Only way a unit can leak is with locoust where you seem not to be able to pick it after you make it so watch out but other people say this is false.


Your math also is funny, 4 KB = 4096 characters worth.


People used to fear databases with 1000s of entries would be a few KB big but come on, an integer is atmost 16-32 BYTES big with a 32-64 BYTE header. Do the maths and see that unless WC3 is more inifecent than a plumber your 4KB is not accurate.

 What is a memory leak?


-A memory leak is sort of self-explanatory. It is when something in Wc3’s memory is left there to rot. Memory leaks have been hailed as one of the most potent map-destroyers.


Click to expand...


Like the previous poster, this is what my 7 year old sister would write.


What I recomend it should be. . .

 A "Memory Leak" is when a data object (eg. location, group, trigger. . .) is created and through bad coding the handle reference (the infomation that tells a function where the data object is in your memory) is lost, this means the data object is still stored on your computer's RAM (memory) but it can not be removed since you lost all references to it.


Thus the name Memory Leak


And so you beginners remember you can think of it like


Memory (data objects) Leak (lost with no way to find)


Click to expand...


Note the QUOTES are to tell where it starts and ends.


Also you left out one of jass's most major features.


HANDLE VARS and GAME CACHES for MUI.


Even beginners have to be informed of their power.

 How do I create a local variable?


-There are a few ways, but here are 3 very common ones.


 function variable_test takes nothing returns nothing local unit u = …endfunction


This one declares it and gives it a value at the beginning of the function.


 function variable_test takes nothing returns nothing local unit u call CreateUnit(…..)set u = GetLastCreatedUnit()endfunction


This one declares it but gives it a value later in the function.


 function variable_test takes nothing returns nothing local unit uset u = CreateUnit(…)endfunction


And this one declares it then sets it by calling a function.


Click to expand...

QUOTE IS MESSED UP


Well there is actually only 2 ways to make a local

 1. You set it while you make it

 JASS:

local real r = 123456.789


2. You make a blank local and then set it

 JASS:

local real r
set r = 123456.789


It is important to note that you may define a local with a value and then set it's value and infinate number of times and thus the 2 ways above have no real affect on how a local functions.


Click to expand...

Again this was used to show what it should be.


You also missed out timers, wich are often used for effects that are carried out over a duration like a jump or black hole spell.


Also you need to describe in more detail how to use arrays since the "[]" might confuse beginners into thinking that you can not put functions in them and also some people might not know how to use them properly.


Next to that seems ok, ill recheck once you fix thoes major flaws but otherwise good work.

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

I'll get on that.

paskovich said:

 2. In the excercise, where the mistakes have to be found, you could include a solution in the end of the tutorial or hide or i don't know, but it makes no sence to ask people to correct it, but if they are wrong, they won't know it...

But this could be done in all excercises.


Click to expand...

I should also probably put who did what tutorial too, so things don't get mixed up. The if/then/else and loop structure tutorial was moyack's, I'll bring that message to his attention.

#### paskovich

#### paskovich

 Level 11

JoinedJul 12, 2005Messages764

Resources: 3

#### Resources
Maps 1Tutorials 2

 JASS:

function MemoryLeak takes nothing returns nothing
local unit u
//actions
endfunction

Click to expand...

Oh man, no, this does NOT leak, as you didn't store any unit under the 'u' variable. The variable is created, but empty ones do not eat memory...


Btw, i saw that you changed the unit-leak issue to special effects. Mention that, effects that have a 'stand' animation need more care, as they are supposed to last longer. Like this:

 JASS:

function ASE takes string path, real x, real y, real duration returns nothing
local effect e = AddSpecialEffect(path, x, y)
call TriggerSleepAction(duration)
call DestroyEffect(e)
set e = null
endfunction

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

 A memory leak not only happens when you create things. It also happens when you make a handle-variable and don’t null it.


Click to expand...


That is a lie since when you cerate something it does not leak, it only leaks when you lose track of a created object.


So if you make a location, it has not leaked, but if you do not attach it to a veriable then it has leaked since you can no longer reference that location you just made and so can not be removed.


Leaks by the lack of nulling local handles is also a leak only because the function has ended and you can no longer reference that veriable (since it is local) and it still is storing infomation.


Watch the definition of leak carefully. . .

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Another thing I saw that needs to be added is trackables since thoes are jass only and can be usefull.


Next to that and the handle vars there is no reason not to aprove this.

#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Okay, here are some comments (I add them as I read the tutorial so I don't forget).


DisplayTextToPlayer - replace it in the Hello World function with BJDebugMsg as it is much faster and easier to comprehend. I know it is a BJ but it is easier than all those funky parameters

 JASS:

call CreateUnit(…..)
set u = GetLastCreatedUnit()

FALSE! CreateUnit is not a BJ and therefore does not use bj_lastCreatedUnit to create the unit. Only CreateUnitAtLocSaveLast does this (and any BJs using it of course ). For this example I'd rather you used GetTriggerUnit() and explained later what it does.


For Kill Unit example I suggest you write two separate functions so that you don't confuse people. One with the variable, and the other without it. And the "They both work, but which one looks neater?" question is not concludent, neither is it relevant. It depends from case to case. In this example, I'd personally go with GetTriggerUnit().

 A variable starting with “udg_” means it is global. You cannot do this sort of thing with locals, so do not try it.


Click to expand...

But of course you can. How do you think locals are declared in GUI? With a custom script you create a local with the same name of the global. When a local and global with the same name exist in a function the local prioritary (the scooping of variables). Be careful though as JASS does not allow like C++ to use the "::" operator to refer to the global instead of the local.


About memory leaks, they do not include only variables. They are mainly information stored into the memory that is no longer removed, even though it is not necessary anymore, therefore uselessly consuming the memory of the computer.


You don't destroy the variable. Please don't use such silly concepts even though they sound more comprehensible as people will be confused. You destroy the object to which the handle is pointing (you can't destroy a handle... it's just an adress, a number, not an object).


Actually I don't really like how you refered to the array as "options". False! They are just many variables with the same name and so, differenciated by an index (number). This can be useful because it's easier to work with different consecutive numbers rather than names (and the index can be refered through a variable).


Warning! I really suggest that you advise people about the danger of useless arrays. Explain them that each array is equivalent with 8191 variables, and that this number cannot be modified. Therefore, they shouldn't declare arrays because they look shorter than writing 3 variables. Use them only in extreme case!


Note: It's best to teach people the five variable types (integer, real, string, boolean and handle). Remember that they don't know what these are just now. They are beginners.


!!!Teach people how to name variables and that they can't leave spaces into the name.!!!


"so you do that by delcaring a function" Please change this to "declaring" as people could get confused. You're just introducing the term.


IF/THEN/ELSE - at the important note change "if level=1" to "if level == 1" so familiarize people with the ==. Teach people operators!!!

 "You can create an infinite and unstoppable loop using only the loop and endloop instructions without an exitwhen command."


Click to expand...

I wouldn't recommend that!!!

 "The native CreateUnit(…) makes unit. If you do not destroy this group with the proper function, it will stay embedded in the memory, and thus take up memory."


Click to expand...

HUH? Unit... Group... I'm confused .

 To fix this, simply add this to the end of your function:


call RemoveUnit(GetLastCreatedUnit())


Click to expand...

OMG... GetLastcreatedUnit() after CreateUnit()? AGAIN? It doesn't work!!!

 "A memory leak not only happens when you create things. It also happens when you make a handle-variable and don’t null it."


Click to expand...

That's a bit vague. Usually the bug occurs if in the function you make the handle point to something (or something else). Simple examples:

 JASS:

function E1 takes unit u returns nothing
call DestroyUnit(u)
endfunction

function E2 takes unit u returns nothing
call DestroyUnit(u)
set u = CreateUnit(...)
return u
endfunction

First function does not leak because no attribution has been made inside the function. Parameter handles (which are also locals) do not "leak" unless they are attributed a value like in the second example.


Second function leaks because even though the value is returned, u is not nullified. Quite complex to solve, but not impossible. You can make a global variable point to the local, nullify the local and then return the global.


Well, that's abut it. I may have missed a lot of problems, but this is really long and I don't have the energy to triple check. So change these stuff for now and it's ready for approval.


-Daelin


 Last edited: Mar 12, 2007

#### Deep Sea Kraken

#### Deep Sea Kraken

 Level 15

JoinedDec 29, 2006Messages985

Resources: 10

#### Resources
Models 9Icons 1

Hmm, I have a suggestion: I completely SUCK at reading tutorials, so maybe you could put the tutorial into a downloadable map and create comments on what each function does; for me this would help alot, and im probably not the only person out there (looking at everything like they do in tutorials rly confuses me) i got JASS craft, but i have to have something (or someone) explaining what it does and how it work, and these tutorials just dont do that for me. Sorry if I'm bugging you or being over pretentious (whatever that means ^.^), but you did ask for suggestions, so this is mine.


Although i do probably have a slight edge over other people; i write codes in a program called: Ready to Program, which uses Java language, and its pretty close to this, but far less complex

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

For some reason, I am unable to upload files to this topic. The tutorials should pretty much be self-explanatory, but not everyone learns everything the same way. You'll probably have to stick with copying and pasting the code into a map (there's instructions for running JASS code) and later in the tutorial it goes into triggers in JASS so you'll lose the need for GUI. You should easily be able to put code into a map and just run it. As for commenting, I pretty heavily commented the source code for anything above self-explanatory things (and even some simple things as well).

EDIT: It seems the site allows me to upload files now, but creating a map showing this stuff off wouldn't really take much, so you should be able to do it as I said before.

#### Deep Sea Kraken

#### Deep Sea Kraken

 Level 15

JoinedDec 29, 2006Messages985

Resources: 10

#### Resources
Models 9Icons 1

I still cant figure out how to make it create a specific unit.


How do you find out what the JASS term for each unit is (footman is: 'hf00') how would you find that for custom units? or would you just have to create it in GuI then convert the trigger?

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

While the object editor is open, press ctrl+d. That will show the rawcodes for all objects.

#### Deep Sea Kraken

#### Deep Sea Kraken

 Level 15

JoinedDec 29, 2006Messages985

Resources: 10

#### Resources
Models 9Icons 1

Ah, thank you, also you used this code: local location start_position = GetUnitLoc(caster) to get the postiton of the casting unit whould you do:


local location start_position = GetUnitLoc(attacker) if you want it to be the position of an attacking unit?

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Find the function that returns the attacking unit. Get a program such as JassCraft or open up common.j and search for it. I'm not going to do all the work for you. By the way, I'd like this thread to only be about the tutorials; if you have trouble go ask in the Jass Forum.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

This is a great and descriptive tutorial. I hope more is to be added, or is it finished? Either way, it is an outstanding tutorial (IMO), +rep... Thanx :emote_grin:

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

PurgeandFire111 said:

 I hope more is to be added


Click to expand...

More will be added, as I'm currently working on the next tutorial. I would probably have it finished by now, but I've had a lot of homework this week.

#### Armentia

#### Armentia

 Level 2

JoinedMay 21, 2007Messages14

One suggestion, which may or may not be avoidable. Fix the scrolling requirement. -.- When 'powerreading' tutorials as I do, I lose concentration when having to horizontally scroll for every line of text. I'm only guessing that this is do to the long comments in your code boxes. Maybe edit these and turn some of the longer ones into multiple lines. Thanks -Arm

#### HermanTheGibbon

#### HermanTheGibbon

 Level 8

JoinedMay 27, 2007Messages170

Resources: 1

#### Resources
Tutorials 1

This a "most outstounding" tutorial Rufus! It's awesome for people like me who know precisely nil about technical computery witchcraft, it's thorough and in layman's terms, which is what a tutorial should be. All the other JASS tutorials I tried to read were obviously made by computer boffins/nerds (lol) who usually just outline a bit of code and expect all the 'begginers' who the tutorial is aimed at to know how to fill it in and what everything means. This is not the case here however, all the jazzy jassy terms explained nicely, most excellent tutorial dudes! +rep WHOOT!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

>wyrmlord:

 JASS:

local unit caster = GetSpellAbilityUnit()

It is good to say that GetTriggerUnit() proves to be more efficient:

 JASS:

local unit caster = GetSpellTargetUnit()

> When a function has the text 'takes nothing', that means you don't need anything in the parenthesis when calling it.

I think you could put something like this:

--=======--------------------------=======--

When a function has the text 'takes nothing', that means that there is no variables to substitute parameters of a call.

The format is like so:

 JASS:

function JASS takes var Vname returns nothing

To have multiple parameters to what the function takes, you seperate them with commas:

 JASS:

function JASS takes var1 Vname1, var 2 Vname2 returns nothing

An example of a function with parameters would be this:

 JASS:

function Killunit takes unit whichUnit returns nothing
 call KillUnit(whichUnit)
endfunction

Parameters from functions can also be used to create your own functions:

 JASS:

function Killunit takes unit whichUnit returns nothing
 call KillUnit(whichUnit)
endfunction

function test takes nothing returns nothing
 call Killunit(GetTriggerUnit)
endfunction

--=======--------------------------=======--

I may reply more, but that's it for now.

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

PurgeandFire111 said:

 >wyrmlord:

 JASS:

local unit caster = GetSpellAbilityUnit()


It is good to say that GetTriggerUnit() proves to be more efficient:

 JASS:

local unit caster = GetSpellTargetUnit()


> When a function has the text 'takes nothing', that means you don't need anything in the parenthesis when calling it.


I think you could put something like this:


--=======--------------------------=======--


When a function has the text 'takes nothing', that means that there is no variables to substitute parameters of a call.


The format is like so:

 JASS:

function JASS takes var Vname returns nothing


To have multiple parameters to what the function takes, you seperate them with commas:

 JASS:

function JASS takes var1 Vname1, var 2 Vname2 returns nothing


An example of a function with parameters would be this:

 JASS:

function Killunit takes unit whichUnit returns nothing
 call KillUnit(whichUnit)
endfunction


Parameters from functions can also be used to create your own functions:

 JASS:

function Killunit takes unit whichUnit returns nothing
 call KillUnit(whichUnit)
endfunction

function test takes nothing returns nothing
 call Killunit(GetTriggerUnit)
endfunction


--=======--------------------------=======--


I may reply more, but that's it for now.

Click to expand...


The use of GetSpellAbilityUnit() is just for clarity. Thanks for your second suggestion, though unless I'm mistaken the tutorial already covers that.

#### hot lips

#### hot lips

 Level 3

JoinedApr 28, 2007Messages24

Resources: 1

#### Resources
Maps 1

WOW to hard way to hard i like the trigger editer better.

#### cosmicat

#### cosmicat

 Level 4

JoinedNov 26, 2006Messages61

Well you did say we could ask... What's so special about Channel?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

wyrmlord said:

 The use of GetSpellAbilityUnit() is just for clarity. Thanks for your second suggestion, though unless I'm mistaken the tutorial already covers that.


Click to expand...

Yea, it does, but I was just saying that it may be better to explain it a bit better, and I provided an example.

hot lips said:

 WOW to hard way to hard i like the trigger editer better.


Click to expand...

Well, it takes a while to learn JASS and it will benefit you in map making. Though many people do prefer GUI...

cosmicat said:

 Well you did say we could ask... What's so special about Channel?


Click to expand...

Channel is a "special" ability because channel has more fields than what a regular spell has.

Channel is one of the only few abilities that allow you to change the Base Order ID which is extremely useful if you want to quickly change an ability's ID. Channel also allows you to set whether or not it is visible, it's target types, and more. It is like the perfect ability to create a spell on due to it's features. Just simply remove the fields and add some, then trigger the rest.

#### Mr.Jamaka2

#### Mr.Jamaka2

 Level 4

JoinedMar 21, 2007Messages88

now what about attaching stuff to models how do i do that in JASS?

#### Fulla

#### Fulla

 Level 12

JoinedAug 3, 2005Messages744

Resources: 2

#### Resources
Tutorials 2

 JASS:

call AddSpecialEffectTarget()


example:

 JASS:

call AddSpecialEffectTarget(effect,GetTriggerUnit(),"chest")

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Guys, great job, but i really don't understand the lesson number 4 - If/Then/Else. I just think you are moving to fast for unexperienced people to learn it. Also the challenge is too difficult ... I really think you should post the solutions for this exercises ... Not everyone is as professional as you are. By the way, the JASS program you mentioned really helps. Thanks.


Please change lesson 4.

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Flame_Phoenix said:

 Guys, great job, but i really don't understand the lesson number 4 - If/Then/Else. I just think you are moving to fast for unexperienced people to learn it. Also the challenge is too difficult ... I really think you should post the solutions for this exercises ... Not everyone is as professional as you are.


Please change lesson 4.


Click to expand...


After reading the lesson over, I can definitely see what you mean by lesson 4 being a bit confusing. As for the challenge, not enough information was provided to give you a fair chance in solving it either. I'll probably go back and edit lesson 4 when I get the chance, but for now you'll just have to wait a bit.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Thanks wyrmlord. Btw i also posted a comment in your section about MUI spells. I think you should see it as well.


Another thing, errmm in my humble opinion (you are the experts here) i think you should post the solutions somewhere. If not i understand. Just an opinion.


Another thing about lesson 2 - Variables.


According to your JASS tutorial, the local variables are not well made in this MUI example made by wyrmlord. Will some1 explain me what is wrong here ???


Can i make MUI spells with this system ??


Can i make MUI spells using the correct system you guys describe in this tutorial ??? or is it too much complicated ??


Also when i use the posted system in some spells it just doesn't work .. why ?


Again i confess i am new at JASS, but i am doing my best to learn.


 Last edited: Aug 14, 2007

#### Bobo_The_Kodo

#### Bobo_The_Kodo

 Level 5

JoinedJan 15, 2007Messages199

flame, with that mui gui, how did u get those globals to show up in the list for GUI, do you also create a global with that name in the 'Variables' thing?

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

WyrmLord teaches this in one of his tutorials. You should check them.

#### FamousPker49

#### FamousPker49

 Level 18

JoinedMay 27, 2007Messages1,689

Resources: 3

#### Resources
Models 1Skins 1Tutorials 1

Very good tutorial, I figured you saw I was getting a few lessons in the chat from HINDY lol, but this is quite helpful

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

... Helpful for those you already have JASS basic knowledge ... because those people who really know nothing about it and who want to learn it ... well, they will have a hard time here .... just check lesson 4 ... i waited months for people to improve it and i am still waiting.

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

Woah

You guys did an amazing job here with the explanations and such, and even citing helpful software


I personally love the GUI interface,  and haven't fully explored it yet, so when i just can't do it with GUI I'll be sure to re-read this tutorial.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

It seems like a serie of tutorial with no "bridge" between them ... they are not well connected. Disagree --- Also, read my last poost ...

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Update

And just when you thought I had given up on this... *BAM* another lesson. Finally decided I was going to make some progress on this. It may have taken quite awhile, but better late than never. Lesson 8 covers the previous challenge in Lesson 7 and goes over using special effects. Read it over and let me know what you think. The "JASS Tutorials.zip" file on the first page has also been updated.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

LOL, yes i really though you had given up on this ...

Ok, i will read it. Btw, such tuts you make are the reason why you are a moderator.

Another thing, i have been leraning stuff about JASS, and i know a lot more know .. Is there any change for me to do a JASS tutorial for here ?

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

The problem with multiple people working on the same tutorial series is that they don't flow as well and each can be in a different format from each other as I'm sure you've noticed. In the end, additional work is done just to make each tutorial fit in more than if one person were to just do them.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

Yes, i notest that ... a lot ... still you didn0t answer my question ...

#### Just call me Js

#### Just call me Js

 Level 3

JoinedOct 8, 2007Messages41

Flame_Phoenix said:

 Guys, great job, but i really don't understand the lesson number 4 - If/Then/Else. I just think you are moving to fast for unexperienced people to learn it. Also the challenge is too difficult ... I really think you should post the solutions for this exercises ... Not everyone is as professional as you are. By the way, the JASS program you mentioned really helps. Thanks.


Please change lesson 4.


Click to expand...


i agree with Flame_Pheonix , probably the challenge for Lesson4 - If/Then/Else is a bit too hard. I tried to do the challenge, got a variation of the answer but dont know if it is correct or how to improve it. Therefore i really think you should post a solution for beginners like me and others.

#### Just call me Js

#### Just call me Js

 Level 3

JoinedOct 8, 2007Messages41

I want to ask a few questions about the Channel spell


First i would like to know a few things in the "Data-Options"


1.Targetting Imge


2.Physical Spell


3.Universal Spell


4.Unique Cast


Could anyone please explain to me what these options do?


Secondly i would like to ask, SHould i always base spells off Channel? Lets say i want to make a spell like storm bolt but it also mana-burns, should i trigger enhance the Storm-bolt or make a new spell based off Channel?


EDIT: one more question


When putting in the Spell Id like 'A000', in the object editor i saw something like ' A000:ANcl ' i put the spell Id as 'A000' in my trigger, it worked but i would like to know what the ANcl is.


 Last edited: Nov 16, 2007

#### Persian_Devil13

P

#### Persian_Devil13

Hi guys, first off, nice guide, i've started learning, my only problem is with the challenges, the first ones were easy enough, im pretty sure i got them all right, but the one for lesson 4 is killing me, i just cant figure out how to do it and ive been at it all day . . . i've looked through JassCraft (including at the Modulo Integer) but no matter what i try it wont work, im ridiculously confused with the challenge, not so much the lesson , in any case could you post up answers somewhere, maybe at the end of the tutorial or beneath the challenges a button you can click to show the answer? pleaaaaaaaaase, also if anyone gets a chance before he gets round to putting answers can u give any hints?

#### daw_guy

#### daw_guy

 Level 5

JoinedDec 12, 2006Messages128

Resources: 2

#### Resources
Maps 2

Ok i'm stuck on tutorial 3's challenge.


I found a_message


I found that the order should be reversed


And that there should be a space after the ='Something is wrong'


What are the other two. I couldn't find the answer.

#### Persian_Devil13

P

#### Persian_Devil13

I think the other 2 are where Function2 has to have a 'return string' or something and isnt 'Something is wrong' meant to be in brackets?

#### daw_guy

#### daw_guy

 Level 5

JoinedDec 12, 2006Messages128

Resources: 2

#### Resources
Maps 2

Hmm the first one sounds right.


I don't know about the second one though.

#### Just call me Js

#### Just call me Js

 Level 3

JoinedOct 8, 2007Messages41

I know this might be abit off-topic but if Channel is a good spell to base your custom spells off then, are there any good Units or Building or Buff to base your things off from?

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

Yeah

The second part needs to be in "" type of thing, not ' '


You can check it by putting it in your triggers, and if you can't save your map, it'll give you a reason and highlight it


Although, I tried to use the If/then/Else example that was given, and i think my WE bugged because i was missing "endif" at the end


In that case (it didn't show a message, just went right to close) it won't show an error message X_x


___________________________________________________________________


I re-read the tutorial, and have been creating, testing, and troubleshooting JASS triggers for the past 3 days, this tutorial is ok, not as amazing as I think it could be,


But the JASScraft is a tremendous supplement, making this tutorial truly awesome


There are about a billion ways to teach a person, but for the learner, it all comes down to time and effort, more the more time you use to learn, at the same time keeping consistent effort (you want it badly enough) your success rate is much higher (like ten-fold)


I don't have much of a reputation, but I tutor for a living (math), where the dynamic between tutor and student is far different from teacher and student


(teachers get paid by the gov't, where tutors get paid by the student,


at the same time, elementary to high school grade levels, the students don't have any immediate cost for their learning, where as tutoring, it is their time and money)


Anyways, the simplest advice I could give on the tutorial, is add more challenges, more like 3-4 exercises per lesson, the first 3 are easy and just flexing to build confidence, the next is about a medium difficulty, while the last is a challenge designed to test, although make the challenge possible (like a 65%-75% success rate per person)


Do this for each lesson, shouldn't take too long as long as they are simple and based solely off what you just learned (not on size, or complexity)


There are alot of other things you could do, but they are pretty minimal and the JASScraft covers anything else that would make it better


Also, a mini-tutorial on how to use the JASScraft efficiently somewhere in the tutorial (like a whole new lesson) would probably benefit this tutorial a sh*tload


 Last edited: Dec 3, 2007

#### sonkan1

#### sonkan1

 Level 8

JoinedJan 18, 2007Messages331

Resources: 2

#### Resources
Models 2

eehm... culd you think of making a dash spell in jass? im bad in it even after this wonderful toto=)

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

 JASS: Moving From GUI to Jass, the Start


Rheias

Sep 26, 2007

Scripting Tutorials
2

Replies61Views32K

Jul 22, 2008
Vercas

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