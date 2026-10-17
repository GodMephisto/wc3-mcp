---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-your-own-tools-for-3dsmax-gmax.239562/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making Your Own Tools for 3dsmax Gmax
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-your-own-tools-for-3dsmax-gmax.239562/>
#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

#
 Making Tools for 3DS Max / GMAX

By BlinkBoy

##
 Overview

###
 Difficulty

4/10 Easy-Normal difficulty

###
 Tools Required

3d Studio Max [Any Version]

GMAX

Optional: NeoDex 2.5 or higher

Maxscript reference

###
 Knowledge Requirements

Basic knowledge on managing 3dsmax and it's interface.

Some basic knowledge on programming. knowing GUI or JASS is enough. Knowing JASS or any programming language is a plus

###
 This tutorial will teach you to:

Grasp the basic of Maxscript's syntax and tools.

Learn to create a basic tool for your every day modeling.

Learn to gather information on your scene objects.

###
 Content

Section 1: Introduction to Maxscript
Section 1.1: Variables

Section 1.2: Conditionals

Section 1.3: Loops

Section 1.4: Arrays

Section 1.5: Making Simple Scripts

Section 2: Getting Started with Macroscripts

Section 3: Programming Logic to your Tools

Section 4: Configuring your tool as a NeoDex Plugin

###
 Author's Note

Whenever you get stuck, don't hesitate to post. I'll be looking into the tutorial constantly.

##
 Section 1: Introduction to Maxscript:

What's Maxscript?


Maxscript is an embedded language for manipulating 3dsmax/gmax added to 3ds max ever since version 3 (thus gmax has it too).

Why learning Maxscript?


Learning Maxscript will help you a lot while modeling. Just knowing some basic maxscript will help you do simple task which could be time consuming and annoying. Things like changing names creating multiple objects in certain pattern, embedding animations to other objects, etc.

Great! I'm all up for that, I'm tired of renaming the 30 bones of the model that my partner who uses Milkshape made

Note: I do have something personal against Milkshape


Sure, how about we start opening 3dsmax/gmax? Shall we?


okay, I'll suppose two things: first you already know how to manage on 3ds max/gmax and 2nd, you have programmed a bit before. Let's start going to the utilities panel -> Maxscript.


A beautiful chart for describing the basic tools in maxscript


The Chart:

1. Open listener: self Explanatory.

2. The Listener: the listener is a tool used to send commands to 3dsmax/gmax. It's nice for doing simple operation

3. The Macro recorder. This tool is a beast. If you turn on the macro recorder (MacroRecorder->Enable), anything you do within your scene will be printed as code in it. it's great if you want to learn how to modify something you know manually through code.

4. New Script: This is for creating Maxscript Scripts which you can use later on.

 5 and 6 are self explanatory.


okay let's throw the boring chart apart. time to learn some maxscript for Christ's sake.


Maxscript is an interpreted language, similar to C. It has a lot of nice stuffs for programming and is quite easy to learn. First we will start with variables.


Open the listener, we gonna play a bit with our toy. I suggest you save and close anything important. We may feel like crashing 3dsmax sometimes just for fun or misfortune.

###
 Variables:


Variables in Maxscript are just data associated to a name. They can have any type annexed to them, you can even change their type if you feel like by changing their value.


declaring a variable is pretty much this:

 Code:

a = 4

 yeah, variables are simply created when you magically decided they existed. In this case I thought of telling max that there was a variable called whose value is 4.


Now go to the listener, let's play a bit


So I write 'a = 2'


and hit enter. The listener responds me with 2. it's his lovely way of saying, the variable was assigned.


I think you can pretty much grasp the idea. Go playing with the listener as in the picture. remember, 1 command at a time.


nice, we now know a bit of variables. let's learn a bit more of them.


First variables are typesafe this means that if you try to + (plus) a  number  with a  string , maxscript will gently show you the middle finger and tell you to fuck off. You don't believe me try it:


(Write each command 1 by 1)

 Code:

a = "Hello"
b = -101
a + b


You should have this on the listener:

 Code:

a = "Hello"
"Hello"
b = -101
-101
a + b
-- Unable to convert: -101 to type: String


There, he gave you the error you expected.


Now there is one last thing to know about variables.


To know the type of a variable we have a "command" (actually is a function, but that comes later) called classof. this beauty is perfect when we store shit and we forgot what was it.


Try this:

 Code:

a = 9
classOf a


the listener will say:

 Code:

a = 9
9
classOf a
Integer


there you go.


I almost forgot to tell you. variable names are case insensitive in maxscript the variable "IEatPizza" is the same as "ieatpizza". Case of letters is ignored.


Now let's go to control flow structures.

###
 Conditionals:


Conditions are the typical if this happens then do this else do this


In Maxscript the syntax is the same:

 Code:

if <condition> then <dothis> else <dothisOtherShit>


here's a simple serious example on the listener:


why it printed 2 values? don't worry it is not important.


that's how you do conditionals in maxscript.


try it if you feel like with a simple example:

 Code:

a = 2
b = 1
if b < a then print "less" else print "greater"
if a != b then print "the variables are different"


Now let's go to loops

###
 Loops:


Maxscript offers many different kind of loops. It has for-loops- while-loops and do-while loops. Here's a fast example of for loops:


The syntax of the loop structures:

 Code:

for <var> = <value> to <value> do <action>
while <condition> do <action>
do <action> while <condition>


Did you try that for loop I showed you? well let's try an annoying example with while loops.

 Code:

do print "tick... tick..." while (queryBox "Am I annoying you?")

I think that gives you the notion of loops, xD.


Last thing before going to do some real shit:

###
 Arrays:


Defining arrays in maxscript is pretty easy. they use this syntax: #()


Example:

 Code:

a = #()


You can also fill it with some members

 Code:

a = #(8,9,5,7)


Not all the members need to be of the same type:

 Code:

a = #(5,6.8,"Hola!")


(this is not recommended, though)


Now let's try accessing a member

 Code:

a = #(8,9,6)
a[2]
a[4]


The listener will say:

 Code:

a = #(8,9,6)
#(8, 9, 6)
a[2]
9
a[4]
undefined

Important Note! Arrays in maxscript are 1 index based. If you try accessing a negative or zero member, maxscript will go rude and show you the middle finger again!


Notice that the array has 3 members. accessing member 4, returns undefined, because it's not within the array bounds.


In arrays you can also change and define new members by simply assigning the members. try this:

 Code:

a = #(8,9,6)
a[2] = -5
a[4] = 8
a
a[7] = 9000
a


the listener should say this:

 Code:

a = #(8,9,6)
#(8, 9, 6)
a[2] = -5
-5
a[4] = 8
8
a
#(8, -5, 6, 8)
a[7] = 9000
9000
a
#(8, -5, 6, 8, undefined, undefined, 9000)


so as you can see adding members is quite easy.

Important Note! whenever you add a new member which is not within the array bounds, maxscript will internally increase the size of the array. This operation is a bit slow. If you know the size of an array, specified it immediately by setting the last member. Example:

 Code:

a = #()
a[100] = undefined
a
for i = 1 to 100 do a[i] = random 1 100
a


Listener says:

 Code:

a = #()
#()
a[100] = undefined
undefined
a
#(undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, undefined, ...)
for i = 1 to 100 do a[i] = random 1 100
OK
a
#(77, 80, 63, 42, 83, 95, 38, 44, 17, 59, 37, 96, 68, 63, 62, 84, 17, 15, 96, 27, ...)


There you go. Now to know the size of an array, we do this:

 Code:

a.count


listener:

 Code:

a.count
100


Great, now let's learn some useful functions.


function: append


append is used for adding a member at the end of an array. it's perfect if we don't know how big it should be at the end.


Example:

 Code:

a = #(8,9,6)
a
append a 2
a


Listener:

 Code:

a = #(8,9,6)
#(8, 9, 6)
a
#(8, 9, 6)
append a 2
#(8, 9, 6, 2)
a
#(8, 9, 6, 2)


function: findItem


This function is used to find an item in an array. if the item is found it returns it's index, if not, it returns 0.


Simple Example:

 Code:

a = #(8,9,2)
findItem a 9
findItem a 7


Listener will say:

 Code:

a = #(8,9,2)
#(8, 9, 2)
findItem a 9
2
findItem a 7
0


for more information on arrays go the maxscript reference and follow: MAXScript Language Reference -> Collections -> Collections Type -> Array Values.


Now time to start making some concrete stuffs. Let's play a bit with object creation.

###
 Creating Simple Scripts:


Ok, let's click "New Script" in Maxscript panel. (utility Panel -> Maxscript, remember?)


You should get a windows like this one:


We can now start writing down our scripts.


Let's start writing some simple code from what we know. we'll write a simple script that creates 10 boxes along a vertical line in 3d space.


Code:

 Code:

-- we want to create 10 boxes
for i = 1 to 10 do
 box pos:[0,0,i*10] length:5 width:5 height:5
-- what we are doing is we are creating a box of 5x5x5 (a cube)
-- every 10 'centimeters' from floor.


Notice somethings here:

 'box' is an object in 3ds max. It's the class used for creating box primitives. The rest things like pos, length, width, height are optional parameters.

 pos uses a Point3, a type of data for 3D points/vectors. They are defined as [<x coordinate>, <y coordinate>, <z coordinate>]. We are fitting the boxes to the center of the scene in x,y coordinates and to every 10 in z coordinate.

 length, width, height are properties for the box. We set them all to 5 tpo make it a cube.


Now since we are in the script editor, to run our code. We have two options, evaluate it or save the script and run it.


In this case we will evaluate it. to do that go to Tools->Evaluate All


After you evaluate you should have this on your viewport:

 Great! isn't it? Now delete them! xD. Wait don't use the interface, use maxscript


go to the listener and type:

 Code:

for o in objects do delete o

 da fuq is that? I'll explain lil by lil.


first:

 Code:

objects

is a max global variable which stores all your scene objects inside a big array.

 Code:

for <var> in <collection> do <action>


is a type of for-loop for 'collections' a collection is any type of object which's purpose is to store multiple values. An Array is a collection.


in human what that sentence is saying. "For every object called o within objects, delete that o".


Great, now let's do more things with our script. Let's change color of our boxes


Let's fix a bit our script before doing some more changes:

 Code:

-- we want to create 10 boxes
for i = 1 to 10 do
(
 local b = box pos:[0,0,i*10] length:5 width:5 height:5
 -- what we are doing is we are creating a box of 5x5x5 (a cube)
 -- every 10 'centimeters' from floor.

)

What did I just do? first added a instruction block and defined a local variable for storing the box.


An instruction block is used for serialization of instructions. Syntax:

 Code:

(
 <DoThisFirst>
 <NowDoThis>
 <FinallyDoThis>
)


Here's an example you can try on the listener (Don't copy paste it, type it all from start to end)

 Code:

(
 a = 9
 b = 8
 a + b
)


There you go. Now let's talk of local variables.


A local variable is a variable that only exists within a context. It cannot be accesed from the outside. This is great if we just need variables for temporal storing.


now lets look back on our script:

 Code:

-- we want to create 10 boxes
for i = 1 to 10 do
(
 local b = box pos:[0,0,i*10] length:5 width:5 height:5
 -- what we are doing is we are creating a box of 5x5x5 (a cube)
 -- every 10 'centimeters' from floor.

)


we now have our block and our boxed saved in a local variable. We want to give our box a set of colors. For that we will create another local variable for our new color and assign it a color value. Then we will assign this value to a property of the box called wiredColor.

 Code:

-- we want to create 10 boxes
(
 -- we created another block to keep all our variables local
 local partition = 255/10
 -- this variable divides colors by scale
 for i = 1 to 10 do
 (
 local b = box pos:[0,0,i*10] length:5 width:5 height:5
 -- what we are doing is we are creating a box of 5x5x5 (a cube)
 -- every 10 'centimeters' from floor.
 local factor = partition*i
 local c = color factor factor 255 --Red Green Blue
 b.wireColor = c
 )
)


Now evaluate it:


Nice, isn't it? we made a blue color gradient.


You may be wondering, how did I know a box had the property of wireColor? is there anyway to find out about objects and actions? actually there is! and not only one, but many! let's try a simple experiment. Go to the listener, macroRecorder->Enable. Now create a box, change it's color and move it a bit:


and now look at the macro recorder:


Great, isn't it? You know now the code related to anything you do in 3dsmax/gmax.


Now, what happens if you want to know the properties of an specific object? Simple select the object and typde this in the listener:

 Code:

show $


I selected just one box and here is what the listener shouted:


Listener:

 Code:

show $
 .height : float
 .length : float
 .lengthsegs : integer
 .width : float
 .widthsegs : integer
 .mapcoords : boolean
 .heightsegs : integer
 .realWorldMapSize : boolean
false


Notes:

 $ is a global variable in max which stores the selected object/objects. If only one object is selected, it contains the object, if many then it contains a collection of the objects. It has a synonym variable called selection, both have the same data.

 show is a function which takes an object and shows ONLY it's own properties, it won't show the properties of it's 'SuperClass'


back to the boxes script. Now what happens if we want to save this behavior all the time? Simple, we turn it into a function. Functions in maxscript are easy to declare. The syntax is this one:

 Code:

function MyFunc <param> optionalParam:<default value> =
(
 <instructions>
)

-- or

fn MyFunc <param> optionalParam:<default value> =
(
 <instructions>
)


here's an example:

 Code:

fn Sum a b = a + b


Now let's convert our box creation code into a function:


Original Code:

 Code:

-- we want to create 10 boxes
(
 -- we created another block to keep all our variables local
 local partition = 255/10
 -- this variable divides colors by scale
 for i = 1 to 10 do
 (
 local b = box pos:[0,0,i*10] length:5 width:5 height:5
 -- what we are doing is we are creating a box of 5x5x5 (a cube)
 -- every 10 'centimeters' from floor.
 local factor = partition*i
 local c = color factor factor 255 --Red Green Blue
 b.wireColor = c
 )
)


As a function:

 Code:

fn createBoxes boxNum x y =
(
 -- we created another block to keep all our variables local
 local partition = 255/boxNum
 -- this variable divides colors by scale
 for i = 1 to boxNum do
 (
 local b = box pos:[x,y,i*10] length:5 width:5 height:5
 -- what we are doing is we are creating a box of 5x5x5 (a cube)
 -- every 10 'centimeters' from floor.
 local factor = partition*i
 local c = color factor factor 255 --Red Green Blue
 b.wireColor = c
 )
)


Now if you evaluate it. No boxes will be created, but instead this will appear in the listener:

 Code:

createBoxes()


now we can use this function anywhere in max, let's try this:

 Code:

createBoxes 3 20 20
createBoxes 4 -20 20
createBoxes 5 -20 -20
createBoxes 6 20 -20


You should get something looking like this:


If you've made it up to here, you are awesome. this was the most difficult part of the tutorial. From here on, we will keep things easy.


It's time to start our tool.

##
 Section 2: Getting Started with Macroscripts:


Ok time to make our little tool. First The Problem. All tools start with a problem, something we want to automatize.

###
 The Problem:

many times when we finish rigging our bones we need to name them, The thing is it takes some time. 5 or 10 minutes of our precious time! Time is a resource and we need to save it up. So we have this:


Now the thing is We want to automatize this. Normally, we always name bones the same, Bone_Root, Bone_Arm1_L, etc. (or whatever nomenclature you use). For this reason we want to produce a tool.

###
 The Solution:


The idea would be to make a simple tool with standard skeleton of how we normally rigg most of our models. We also want to offer a tool for auto-naming bone chains which are not standard, like for long hair, capes.


So with all that said. Let's start creating our tool.

###
 Creating the tool's interface:

first create a new script (Utility Panel -> Maxscript->New Script, remember?).


in 3dsmax and GMAX, there are two types of basic tools: macroscripts and utilities. For this tutorial we will use macroscripts.


To create a macroscript here's the definition syntax:

 Code:

macroscript <Tool Name>
 buttonText:"<Name in interface>"
 category:"<3dsmax's category>"
 internalCategory:"<Inner Category>"
(

)


That simple, now let's make our's

 Code:

macroscript NeoDex_Bone_Namer
 buttonText:"Bone Namer"
 category:"NeoDex Toolkit"
 internalCategory:"Tools"
(

)


I've made it so it be a plugin for NeoDex. All NeoDex plugins should be in the "NeoDex Toolkit" category. You are not really forced to make it a neodex plugin if you want.


Anyways, let's make the basic interface. For that select the center and go to Tools-> New rollout.


You should now have a window like this one showing up:


Rename the rollout to mainRollout.


Now we will create some pick buttons. A pick button is a type of button which allows the user select a node or object in the scene. We want to create a button for each generic bone name, like head, chest, neck, arm1 left, etc.


Create a pick button at top. (you create it by clicking the "pick button" and clicking on the rollout)


change it's caption to "0_0". It will be the head button.


Now create a group box to keep everything organized:


name it "Basic Skeleton".


Now create the rest of the pick buttons for the rest of the body pieces, this ones will have no caption.


Now let's rename each button so we know which one is which:


Here's the list of names I used:

HeadBtn

NeckBtn

Arm1LBtn

Arm2LBtn

HandLBtn

Arm1RBtn

Arm2RBtn

HandRBtn

ChestBtn

RootBtn

PelvisBtn

Leg1LBtn

Leg2LBtn

FootLBtn

Leg1RBtn

Leg2RBtn

FootRBtn

ShoulderLBtn


Finaly we will turn their event on, on event handlers for each button.


Finally save the rollout and close the editor. The code for your interface should be generated. Probably it will be messy, so you may have to select it and do TABs to align it. Here's mine after ordering:

 Code:

macroscript NeoDex_Bone_Namer
 buttonText:"Bone Namer"
 category:"NeoDex Toolkit"
 internalCategory:"Tools"
(
 rollout mainRollout "Untitled" width:364 height:352
 (
 pickbutton HeadBtn "O_O" pos:[86,26] width:50 height:41
 GroupBox grp1 "Basic Skeleton" pos:[13,8] width:203 height:334
 pickbutton NeckBtn "" pos:[100,74] width:19 height:16
 pickbutton Arm1LBtn "" pos:[180,83] width:19 height:39
 pickbutton Arm2LBtn "" pos:[180,132] width:20 height:39
 pickbutton HandLBtn "" pos:[177,179] width:28 height:16
 pickbutton Arm1RBtn "" pos:[23,83] width:19 height:39
 pickbutton Arm2RBtn "" pos:[22,132] width:20 height:39
 pickbutton HandRBtn "" pos:[19,179] width:28 height:16
 pickbutton ChestBtn "" pos:[71,99] width:80 height:71
 pickbutton RootBtn "" pos:[100,179] width:24 height:14
 pickbutton PelvisBtn "" pos:[77,199] width:69 height:21
 pickbutton Leg1LBtn "" pos:[137,228] width:21 height:34
 pickbutton Leg2LBtn "" pos:[140,267] width:22 height:36
 pickbutton FootLBtn "" pos:[146,311] width:38 height:17
 pickbutton Leg1RBtn "" pos:[69,228] width:21 height:34
 pickbutton Leg2RBtn "" pos:[60,267] width:22 height:36
 pickbutton FootRBtn "" pos:[37,311] width:38 height:17
 pickbutton ShoulderLBtn "" pos:[129,76] width:44 height:12
 pickbutton ShoulderRBtn "" pos:[50,76] width:44 height:12
 on HeadBtn picked obj do
 (

 )
 on NeckBtn picked obj do
 (

 )
 on Arm1LBtn picked obj do
 (

 )
 on Arm2LBtn picked obj do
 (

 )
 on HandLBtn picked obj do
 (

 )
 on Arm1RBtn picked obj do
 (

 )
 on Arm2RBtn picked obj do
 (

 )
 on HandRBtn picked obj do
 (

 )
 on ChestBtn picked obj do
 (

 )
 on RootBtn picked obj do
 (

 )
 on PelvisBtn picked obj do
 (

 )
 on Leg1LBtn picked obj do
 (

 )
 on Leg2LBtn picked obj do
 (

 )
 on FootLBtn picked obj do
 (

 )
 on Leg1RBtn picked obj do
 (

 )
 on Leg2RBtn picked obj do
 (

 )
 on FootRBtn picked obj do
 (

 )
 on ShoulderLBtn picked obj do
 (

 )
 )
)


Oh looks like we forgot to change the title. No problem! Go to the line:

 Code:

 rollout mainRollout "Untitled" width:364 height:352


and name it "Bone namer"

 Code:

 rollout mainRollout "Bone Namer" width:364 height:352


Great now that we have our rollout, we must make our macroscript create a dialogwindow for it. This is simply done with the function:

 Code:

createDialog <your rollout>


yeah, so if our rollout is called mainRollout we must add

 Code:

createDialog mainRollout

just after the rollout declaration.


The final code:

 Code:

macroscript NeoDex_Bone_Namer
 buttonText:"Bone Namer"
 category:"NeoDex Toolkit"
 internalCategory:"Tools"
(
 rollout mainRollout "Bone Namer" width:364 height:352
 (
 pickbutton HeadBtn "O_O" pos:[86,26] width:50 height:41
 GroupBox grp1 "Basic Skeleton" pos:[13,8] width:203 height:334
 pickbutton NeckBtn "" pos:[100,74] width:19 height:16
 pickbutton Arm1LBtn "" pos:[180,83] width:19 height:39
 pickbutton Arm2LBtn "" pos:[180,132] width:20 height:39
 pickbutton HandLBtn "" pos:[177,179] width:28 height:16
 pickbutton Arm1RBtn "" pos:[23,83] width:19 height:39
 pickbutton Arm2RBtn "" pos:[22,132] width:20 height:39
 pickbutton HandRBtn "" pos:[19,179] width:28 height:16
 pickbutton ChestBtn "" pos:[71,99] width:80 height:71
 pickbutton RootBtn "" pos:[100,179] width:24 height:14
 pickbutton PelvisBtn "" pos:[77,199] width:69 height:21
 pickbutton Leg1LBtn "" pos:[137,228] width:21 height:34
 pickbutton Leg2LBtn "" pos:[140,267] width:22 height:36
 pickbutton FootLBtn "" pos:[146,311] width:38 height:17
 pickbutton Leg1RBtn "" pos:[69,228] width:21 height:34
 pickbutton Leg2RBtn "" pos:[60,267] width:22 height:36
 pickbutton FootRBtn "" pos:[37,311] width:38 height:17
 pickbutton ShoulderLBtn "" pos:[129,76] width:44 height:12
 pickbutton ShoulderRBtn "" pos:[50,76] width:44 height:12
 on HeadBtn picked obj do
 (

 )
 on NeckBtn picked obj do
 (

 )
 on Arm1LBtn picked obj do
 (

 )
 on Arm2LBtn picked obj do
 (

 )
 on HandLBtn picked obj do
 (

 )
 on Arm1RBtn picked obj do
 (

 )
 on Arm2RBtn picked obj do
 (

 )
 on HandRBtn picked obj do
 (

 )
 on ChestBtn picked obj do
 (

 )
 on RootBtn picked obj do
 (

 )
 on PelvisBtn picked obj do
 (

 )
 on Leg1LBtn picked obj do
 (

 )
 on Leg2LBtn picked obj do
 (

 )
 on FootLBtn picked obj do
 (

 )
 on Leg1RBtn picked obj do
 (

 )
 on Leg2RBtn picked obj do
 (

 )
 on FootRBtn picked obj do
 (

 )
 on ShoulderLBtn picked obj do
 (

 )
 )
 -- Initialization code

 createDialog mainRollout
)


There it goes. Now let's evaluate it (Script Window -> Tools -> evaluate all, remember?). Great, we should now try it out. Let's execute our macroscript.


To execute a macroscript you must use the macros.run command. See example above:

 Code:

macros.run "<Macroscript's internalCategory>" ""<Macroscript's name>"


Now type it in the listener for your tool. in my case I did:

 Code:

macros.run "Tools" "NeoDex_Bone_Namer"


and finally he shows up! He's happy to see you .


Here's the script until now:

bone Namer Interface


Now all we have left is add logic. Make it actually work.

##
 Section 3: Programming Logic to your Tools:


Well finally it's time to make our little tool do something for humanity's sake.


What we need to do now is make it change the names. That's simple. we should first declare a function for changing the names and then whenever a button is used, it should pass the picked object and the name related to that object.


our function will look simple:

 Code:

 fn configureBone obj newName =
 (
 obj.name = "Bone_" + newName
 )


now for each pick button, we must call it. Let's take Head event handler for example (an event handler is a code that is run when an action is performed).

 Code:

 on HeadBtn picked obj do
 (
 configureBone obj "Head"
 )


Do it for each button. The final result:

 Code:

macroscript NeoDex_Bone_Namer
 buttonText:"Bone Namer"
 category:"NeoDex Toolkit"
 internalCategory:"Tools"
(

 fn configureBone obj newName =
 (
 obj.name = "Bone_" + newName
 )

 rollout mainRollout "Bone Namer" width:364 height:352
 (
 pickbutton HeadBtn "O_O" pos:[86,26] width:50 height:41
 GroupBox grp1 "Basic Skeleton" pos:[13,8] width:203 height:334
 pickbutton NeckBtn "" pos:[100,74] width:19 height:16
 pickbutton Arm1LBtn "" pos:[180,83] width:19 height:39
 pickbutton Arm2LBtn "" pos:[180,132] width:20 height:39
 pickbutton HandLBtn "" pos:[177,179] width:28 height:16
 pickbutton Arm1RBtn "" pos:[23,83] width:19 height:39
 pickbutton Arm2RBtn "" pos:[22,132] width:20 height:39
 pickbutton HandRBtn "" pos:[19,179] width:28 height:16
 pickbutton ChestBtn "" pos:[71,99] width:80 height:71
 pickbutton RootBtn "" pos:[100,179] width:24 height:14
 pickbutton PelvisBtn "" pos:[77,199] width:69 height:21
 pickbutton Leg1LBtn "" pos:[137,228] width:21 height:34
 pickbutton Leg2LBtn "" pos:[140,267] width:22 height:36
 pickbutton FootLBtn "" pos:[146,311] width:38 height:17
 pickbutton Leg1RBtn "" pos:[69,228] width:21 height:34
 pickbutton Leg2RBtn "" pos:[60,267] width:22 height:36
 pickbutton FootRBtn "" pos:[37,311] width:38 height:17
 pickbutton ShoulderLBtn "" pos:[129,76] width:44 height:12
 pickbutton ShoulderRBtn "" pos:[50,76] width:44 height:12
 on HeadBtn picked obj do
 (
 configureBone obj "Head"
 )
 on NeckBtn picked obj do
 (
 configureBone obj "Neck"
 )
 on Arm1LBtn picked obj do
 (
 configureBone obj "Arm1_L"
 )
 on Arm2LBtn picked obj do
 (
 configureBone obj "Arm2_L"
 )
 on HandLBtn picked obj do
 (
 configureBone obj "Hand_L"
 )
 on Arm1RBtn picked obj do
 (
 configureBone obj "Arm1_R"
 )
 on Arm2RBtn picked obj do
 (
 configureBone obj "Arm2_R"
 )
 on HandRBtn picked obj do
 (
 configureBone obj "Hand_R"
 )
 on ChestBtn picked obj do
 (
 configureBone obj "Chest"
 )
 on RootBtn picked obj do
 (
 configureBone obj "Root"
 )
 on PelvisBtn picked obj do
 (
 configureBone obj "Pelvis"
 )
 on Leg1LBtn picked obj do
 (
 configureBone obj "Leg1_L"
 )
 on Leg2LBtn picked obj do
 (
 configureBone obj "Leg2_L"
 )
 on FootLBtn picked obj do
 (
 configureBone obj "Foot_L"
 )
 on Leg1RBtn picked obj do
 (
 configureBone obj "Leg1_R"
 )
 on Leg2RBtn picked obj do
 (
 configureBone obj "Leg2_R"
 )
 on FootRBtn picked obj do
 (
 configureBone obj "Foot_R"
 )
 on ShoulderLBtn picked obj do
 (
 configureBone obj "Shoulder_L"
 )
 )
 -- Initialization code

 createDialog mainRollout

)


nice let's try it. Evaluate it and run it with macros.run on the listener as I told you before (remember?)


Great! How about we improve it further?


Let's make bones be colored if they are on right, the left or in the middle. For that we must pick 3 colors. I think the perfect colors are: red for left, blue for right and green for middle (This is just a coincidence, no political references here). For that we must declare our color variables for LEFT, RIGHT, CENTER:

 Code:

-- I got these colors by using the listener on a box and asking for its wirecolor.
local RIGHT_COLOR = (color 28 89 177)
local LEFT_COLOR = (color 176 26 26)
local CENTER_COLOR = (color 27 177 27)


You can also use maxscript's constants if you want:

 Code:

-- I got these colors by using the listener on a box and asking for its wirecolor.
local RIGHT_COLOR = blue
local LEFT_COLOR = red
local CENTER_COLOR = green


I'll use the first one.


Now we must fix our configureBone function to also change the color.

 Code:

 fn configureBone obj newName newColor =
 (
 obj.name = "Bone_" + newName
 obj.wirecolor = newColor
 )


Great, now it can change color, but there's a little detail. Not all the objects have a wirecolor, so if you select for instance a dummy object, we may have a problem with it. How do we solve it? Simple we check if they have the property. the function is called "IsProperty" it takes an object and a property and returns true if he has it or false if it does not have it. Our fixed function should look like this:

 Code:

 fn configureBone obj newName newColor =
 (
 obj.name = "Bone_" + newName
 if (isProperty obj #wireColor) then
 obj.wirecolor = newColor
 )


All that's left is use our color variables for saying how a button colors an specific bone. Go to the pick buttons' event handlers and add the new parameter needed, using the color variables. Example:

 Code:

 on HeadBtn picked obj do
 (
 configureBone obj "Head" CENTER_COLOR
 )


Finally the latest code:

 Code:

macroscript NeoDex_Bone_Namer
 buttonText:"Bone Namer"
 category:"NeoDex Toolkit"
 internalCategory:"Tools"
(

 local RIGHT_COLOR = (color 28 89 177)
 local LEFT_COLOR = (color 176 26 26)
 local CENTER_COLOR = (color 27 177 27)

 fn configureBone obj newName newColor =
 (
 obj.name = "Bone_" + newName
 if (isProperty obj #wireColor) then
 obj.wirecolor = newColor
 )

 rollout mainRollout "Bone Namer" width:228 height:352
 (
 pickbutton HeadBtn "O_O" pos:[86,26] width:50 height:41
 GroupBox grp1 "Basic Skeleton" pos:[13,8] width:203 height:334
 pickbutton NeckBtn "" pos:[100,74] width:19 height:16
 pickbutton Arm1LBtn "" pos:[180,83] width:19 height:39
 pickbutton Arm2LBtn "" pos:[180,132] width:20 height:39
 pickbutton HandLBtn "" pos:[177,179] width:28 height:16
 pickbutton Arm1RBtn "" pos:[23,83] width:19 height:39
 pickbutton Arm2RBtn "" pos:[22,132] width:20 height:39
 pickbutton HandRBtn "" pos:[19,179] width:28 height:16
 pickbutton ChestBtn "" pos:[71,99] width:80 height:71
 pickbutton RootBtn "" pos:[100,179] width:24 height:14
 pickbutton PelvisBtn "" pos:[77,199] width:69 height:21
 pickbutton Leg1LBtn "" pos:[137,228] width:21 height:34
 pickbutton Leg2LBtn "" pos:[140,267] width:22 height:36
 pickbutton FootLBtn "" pos:[146,311] width:38 height:17
 pickbutton Leg1RBtn "" pos:[69,228] width:21 height:34
 pickbutton Leg2RBtn "" pos:[60,267] width:22 height:36
 pickbutton FootRBtn "" pos:[37,311] width:38 height:17
 pickbutton ShoulderLBtn "" pos:[129,76] width:44 height:12
 pickbutton ShoulderRBtn "" pos:[50,76] width:44 height:12
 on HeadBtn picked obj do
 (
 configureBone obj "Head" CENTER_COLOR
 )
 on NeckBtn picked obj do
 (
 configureBone obj "Neck" CENTER_COLOR
 )
 on Arm1LBtn picked obj do
 (
 configureBone obj "Arm1_L" LEFT_COLOR
 )
 on Arm2LBtn picked obj do
 (
 configureBone obj "Arm2_L" LEFT_COLOR
 )
 on HandLBtn picked obj do
 (
 configureBone obj "Hand_L" LEFT_COLOR
 )
 on Arm1RBtn picked obj do
 (
 configureBone obj "Arm1_R" RIGHT_COLOR
 )
 on Arm2RBtn picked obj do
 (
 configureBone obj "Arm2_R" RIGHT_COLOR
 )
 on HandRBtn picked obj do
 (
 configureBone obj "Hand_R" RIGHT_COLOR
 )
 on ChestBtn picked obj do
 (
 configureBone obj "Chest" CENTER_COLOR
 )
 on RootBtn picked obj do
 (
 configureBone obj "Root" CENTER_COLOR
 )
 on PelvisBtn picked obj do
 (
 configureBone obj "Pelvis" CENTER_COLOR
 )
 on Leg1LBtn picked obj do
 (
 configureBone obj "Leg1_L" LEFT_COLOR
 )
 on Leg2LBtn picked obj do
 (
 configureBone obj "Leg2_L" LEFT_COLOR
 )
 on FootLBtn picked obj do
 (
 configureBone obj "Foot_L" LEFT_COLOR
 )
 on Leg1RBtn picked obj do
 (
 configureBone obj "Leg1_R" RIGHT_COLOR
 )
 on Leg2RBtn picked obj do
 (
 configureBone obj "Leg2_R" RIGHT_COLOR
 )
 on FootRBtn picked obj do
 (
 configureBone obj "Foot_R" RIGHT_COLOR
 )
 on ShoulderLBtn picked obj do
 (
 configureBone obj "Shoulder_L" LEFT_COLOR
 )
 on ShoulderRBtn picked obj do
 (
 configureBone obj "Shoulder_R" RIGHT_COLOR
 )
 )
 -- Initialization code

 createDialog mainRollout

)


Great evaluate and run it! (You should know by know)

 Beautiful isn't it? How much would have taken by hand? 5 minutes maybe?


Now you got some homework:

Easy: Add a simple small button at top with caption "?" which tells the user how to use your tool. To do that use the 'messageBox' function. (it takes just an string, you can always look it up on maxscript reference).

Easy: Adopt the tool to more complex riggs.

Hard: Normally some people will add bones for hair or capes, is there any easier way for naming this? could you figure out the interface and logic for it?
Final code:Bone Namer Final


Now we'll talk on how to install it as a NeoDex Plugin.

##
 Section 4: Configuring your tool as a NeoDex Plugin:


To configure your tool, it's quite easy. You just need to add one line of code:

 Code:

--name of macroscript, internalCategory, compatibility.
declareNeoDexPlugin "NeoDex_Bone_Namer" "Tools" NeoDexFullCompatibility


This line will tell the installer about your plug-in. The first parameter is the script's name, the second is the script's innerCategory and third is compatibility.


There are three types of compatibility modes:

NeoDexFullCompatibility

NeoDex3dsmaxCompatibility

NeoDexGmaxCompatibility


Full means your tool supports both 3dsmax and gmax. The others are self explanatory.


Here's the tool as as a NeoDexPlugin:

 Code:

--name of macroscript, internalCategory, compatibility.
declareNeoDexPlugin "NeoDex_Bone_Namer" "Tools" NeoDexFullCompatibility

macroscript NeoDex_Bone_Namer
 buttonText:"Bone Namer"
 category:"NeoDex Toolkit"
 internalCategory:"Tools"
(

 local RIGHT_COLOR = (color 28 89 177)
 local LEFT_COLOR = (color 176 26 26)
 local CENTER_COLOR = (color 27 177 27)

 fn configureBone obj newName newColor =
 (
 obj.name = "Bone_" + newName
 if (isProperty obj #wireColor) then
 obj.wirecolor = newColor
 )

 rollout mainRollout "Bone Namer" width:228 height:352
 (
 pickbutton HeadBtn "O_O" pos:[86,26] width:50 height:41
 GroupBox grp1 "Basic Skeleton" pos:[13,8] width:203 height:334
 pickbutton NeckBtn "" pos:[100,74] width:19 height:16
 pickbutton Arm1LBtn "" pos:[180,83] width:19 height:39
 pickbutton Arm2LBtn "" pos:[180,132] width:20 height:39
 pickbutton HandLBtn "" pos:[177,179] width:28 height:16
 pickbutton Arm1RBtn "" pos:[23,83] width:19 height:39
 pickbutton Arm2RBtn "" pos:[22,132] width:20 height:39
 pickbutton HandRBtn "" pos:[19,179] width:28 height:16
 pickbutton ChestBtn "" pos:[71,99] width:80 height:71
 pickbutton RootBtn "" pos:[100,179] width:24 height:14
 pickbutton PelvisBtn "" pos:[77,199] width:69 height:21
 pickbutton Leg1LBtn "" pos:[137,228] width:21 height:34
 pickbutton Leg2LBtn "" pos:[140,267] width:22 height:36
 pickbutton FootLBtn "" pos:[146,311] width:38 height:17
 pickbutton Leg1RBtn "" pos:[69,228] width:21 height:34
 pickbutton Leg2RBtn "" pos:[60,267] width:22 height:36
 pickbutton FootRBtn "" pos:[37,311] width:38 height:17
 pickbutton ShoulderLBtn "" pos:[129,76] width:44 height:12
 pickbutton ShoulderRBtn "" pos:[50,76] width:44 height:12
 on HeadBtn picked obj do
 (
 configureBone obj "Head" CENTER_COLOR
 )
 on NeckBtn picked obj do
 (
 configureBone obj "Neck" CENTER_COLOR
 )
 on Arm1LBtn picked obj do
 (
 configureBone obj "Arm1_L" LEFT_COLOR
 )
 on Arm2LBtn picked obj do
 (
 configureBone obj "Arm2_L" LEFT_COLOR
 )
 on HandLBtn picked obj do
 (
 configureBone obj "Hand_L" LEFT_COLOR
 )
 on Arm1RBtn picked obj do
 (
 configureBone obj "Arm1_R" RIGHT_COLOR
 )
 on Arm2RBtn picked obj do
 (
 configureBone obj "Arm2_R" RIGHT_COLOR
 )
 on HandRBtn picked obj do
 (
 configureBone obj "Hand_R" RIGHT_COLOR
 )
 on ChestBtn picked obj do
 (
 configureBone obj "Chest" CENTER_COLOR
 )
 on RootBtn picked obj do
 (
 configureBone obj "Root" CENTER_COLOR
 )
 on PelvisBtn picked obj do
 (
 configureBone obj "Pelvis" CENTER_COLOR
 )
 on Leg1LBtn picked obj do
 (
 configureBone obj "Leg1_L" LEFT_COLOR
 )
 on Leg2LBtn picked obj do
 (
 configureBone obj "Leg2_L" LEFT_COLOR
 )
 on FootLBtn picked obj do
 (
 configureBone obj "Foot_L" LEFT_COLOR
 )
 on Leg1RBtn picked obj do
 (
 configureBone obj "Leg1_R" RIGHT_COLOR
 )
 on Leg2RBtn picked obj do
 (
 configureBone obj "Leg2_R" RIGHT_COLOR
 )
 on FootRBtn picked obj do
 (
 configureBone obj "Foot_R" RIGHT_COLOR
 )
 on ShoulderLBtn picked obj do
 (
 configureBone obj "Shoulder_L" LEFT_COLOR
 )
 on ShoulderRBtn picked obj do
 (
 configureBone obj "Shoulder_R" RIGHT_COLOR
 )
 )
 -- Initialization code

 createDialog mainRollout

)


Now save your tool in the folder Scripts\NeoDexExtraTools


Run the installer and it should be added to NeoDex's Menu as Neodex Extra.

#### Attachments

Conditionals.jpg

 36.1 KB

 · Views: 1,934



ForLoops.jpg

 25 KB

 · Views: 1,978



IntroChart.jpg

 88.7 KB

 · Views: 2,615



Variables.jpg

 31.1 KB

 · Views: 2,017



NewScript.jpg

 33.5 KB

 · Views: 2,020



Script Editor.jpg

 75.4 KB

 · Views: 1,935



Script Evaluate.jpg

 83.1 KB

 · Views: 1,840



Script Boxes1.jpg

 94.7 KB

 · Views: 2,018



Script Boxes2.jpg

 123.5 KB

 · Views: 1,803



MoveBox.gif

 325 KB

 · Views: 1,985



MacroRecorder.jpg

 55.4 KB

 · Views: 1,944



Script Boxes3.jpg

 88.8 KB

 · Views: 1,738



Problem.jpg

 129.6 KB

 · Views: 1,942



Interface1.jpg

 111.4 KB

 · Views: 2,023



Interface2.jpg

 100.6 KB

 · Views: 1,917



Interface3.jpg

 105.4 KB

 · Views: 1,898



Interface4.jpg

 112.9 KB

 · Views: 1,909



Interface5.jpg

 34.3 KB

 · Views: 1,878



Interface6.jpg

 44.4 KB

 · Views: 1,846



Interface7.jpg

 60.8 KB

 · Views: 1,940



Interface8.jpg

 98.2 KB

 · Views: 1,978



Bone Namer Interface.zip

 692 bytes

 · Views: 372



ToolTest1.gif

 150.1 KB

 · Views: 1,926



TestTool2.gif

 257.3 KB

 · Views: 1,993



Tools3.jpg

 120.4 KB

 · Views: 1,810



Bone Namer Final.zip

 911 bytes

 · Views: 391



NeoDexPlugin.jpg

 67 KB

 · Views: 1,848




 Last edited by a moderator: Aug 23, 2013

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Reserved for me .

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

wow this is a nice tutorial... maybe I'd visit my 3ds max sometime soon and try this things out...

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Okay is pretty much finished now. all left is fixing the syntax highlighting, but I must wait for ralle's call.

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


It looks good

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

So any comments on what's needed for approval?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It looks really good. I just have to find the time to read through it. I was hoping to get feedback from a modeler, but this is on the caliber of your other tutorial (very high), so I don't think there will be anything to fix.

#### GhostWolf

#### GhostWolf

 Level 29

JoinedJul 29, 2007Messages5,174

Resources: 2

#### Resources
Tools 1Tutorials 1

I liked the UI, very imaginative.

Run it through a spell check, there are all sorts of spelling errors spread over the text.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

GhostWolf said:

 I liked the UI, very imaginative.

Run it through a spell check, there are all sorts of spelling errors spread over the text.


Click to expand...

Thanks. Yeah I guessed so, this time I didn't use word to write it. I sent the link to a spell checking service, but it takes some long time.


 PurgeandFire said:



 It looks really good. I just have to find the time to read through it. I was hoping to get feedback from a modeler, but this is on the caliber of your other tutorial (very high), so I don't think there will be anything to fix.

Click to expand...

Well is not really that great compared to the Animation tutorial. I actualy got one on production that's even more useful and bigger (even bigger than the basic animation's tutorial). Haven't really finished writting it. I only have written the first half of it. It's a tutorial for beginners using GMAX and NeoDex. It shows everything from model theory, color theory, gmax basic interface, creating your first model in it and adding effects and animations to it. Pretty big challenge.

EDIT: Fixed most spelling mistakes.


 Last edited: Aug 20, 2013

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

This tutorial was magically approved by the spirit of the hive (maybe it was Ralle).

But anyway, I read through it and it looks good. Double approved.

#### gorillabull

#### gorillabull

 Level 17

JoinedJul 17, 2011Messages1,863

Resources: 5

#### Resources
Skins 1Spells 4

epic tut is epic

 You must log in or register to reply here.


### Similar threads

Locked

 [WIP] Multi-level platform system


Kazeon

Jan 6, 2016

Advanced Scripting

Replies8Views3K

Aug 2, 2022
Tal'Rarity

Locked

 New Tool Needed for me to use my Models!


PimpMunkeh

Jun 17, 2008

Requests

Replies7Views1K

Jun 20, 2008
Bloody_Turds

Locked

 [VJASS] 2-D Arrays give error


sethmachine

Jan 4, 2014

Warcraft Editing Tools

Replies9Views3K

Jan 5, 2014
sethmachine

Sticky

 NeoDex 2.7 Wc3 Modeling Kit for 3dsmax & GMAX


BlinkBoy

Feb 1, 2009

Modeling & Animation
151617

Replies829Views281K

Sep 9, 2025
HolyWolf

 [Snippet] Rects


Bannar

Mar 6, 2014

JASS Resources

Replies13Views5K

May 19, 2018
Spellbound

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

3D Modeling Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top