---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/variables-in-detail.7338/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Variables in Detail
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/variables-in-detail.7338/>
#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Variables in detail

1. Introduction

It is a known fact that variables are used to “store” data into them, for a later use. But how do they do this and what does this storing exactly mean? You will find out more by reading this tutorial. You should have basic knowledge in either GUI or JASS. Note that chapters, notes and paragraphs starting with a * are strictly for JASS coding.

2. The elements of a variable in GUI/JASS

Each variable has three very important elements, and without understand each it is impossible to work with variables at their maximum potential.

a) First element is the logical name of the variable. It’s pretty logical that you need something to make difference between one variable and another, in an understandable manner, making variables easier to use and manipulate. The name is simply used in the code and you are the only one working with it. The computer does not care what name you use so you should some suggestive names.

Note: Valid names can contain only alphanumeric characters along with the character “_”.

*All names must begin with a letter.

Example: An appropriate name for a variable in which you store the (Target Unit of Ability Being Cast)/GetSpellTargetUnit() would be targ or target. A name such as ubv, an_unit or wing_ding are not good ones. Also try to avoid too long names, in case you don’t want to make the code messier and harder to understand.

b) Stored type is the second element of variable. Being a relatively organized scripting language, JASS (that will include GUI, because all such codes are converted to JASS) requests the programmer to specify the type of each variable. Let it be integer, real, boolean, string, code or handle (with all extended types: unit, unit group/group, point/location etc.), a variable can hold a single type (respectively extension) which must be mentioned when the variable is declared. You cannot store a different type to a variable than the one specified at the declaration. Trying to do so will result in an error.

A global variables’ types are declared in the variable editor.

*Local variables’ types are declared along with the variables themselves.

*Example

 JASS:

local [b]unit[/b] cast
local [b]boolean [/b] b

c) Value/Address

The third and probably most important element of variables, since it is the purpose of using them. The value refers to what a variable contains. Now you may ask why I added address too. That’s because handle variables do not store a value, yet instead the address of one. We’ll see in a moment what’s the difference.

All temporary data is stored into the memory (RAM) of the computer. The memory is split into bits (small pieces which can store values of 1 and 0 =>binary code). Every bit is made out of two elements: the address and the value it contains. The address is like its name, and is unique for each bit. You can access a bit through its address, just like you can access a variable through its name. It is very important to make difference between address and value. The address for a bit is always the same. The only element that changes is the value. Just like for a variable the name is always the same, only its value changes. You can have bits with the same value, but you can’t have bits with the same address. Just like two variables can contain the same value, but cannot have the same name.

Now, it is clear that a value is stored on multiple bits. The value type is necessary to know on how many bits is the value stored. When storing a value to a variable, you actually refer to the address where it is stored into the memory. Then when accessing the variable, you practically access the bits containing the value “stored” into the variable.

Now, you cannot control this mechanism in JASS and you can’t make difference between address and value. However, you should know that integer, real, boolean and string variables when assigned a value either directly or from another variable, into the variable the value itself is stored.

Example: We have the global variables Anumber and Bnumber. Left code is GUI, right code is the equivalent JASS.

 JASS:

Set Anumber = 100 set udg_Anumber=100
Set Bnumber = Anumber set udg_Bnumber=udgAnumber
Set Anumber = 200 set udg_Anumber =200

At the end of the code, Anumber will have stored the integer 200, while Bnumber integer 100. Why? Because the the two variables are independent. When I said Bnumber=Anumber I actually copied the value of Anumber into Bnumber. The value from Bnumber and Anumber are at different addresses in the memory, even when they have the same value. This is how integer, real, string,boolean and code variables behave.

Now, there are handle variables which unlike the other types are “pointers”. This means that instead of values, they store addresses, which means that if the value of one changes, so does the value of the other one. Let’s analyze this code (first is GUI, second is JASS). I have two unit group global variables: AGroup and BGroup.

Actions
Unit - Create 1 Footman for Player 1 (Red) at (Center of (Playable map area)) facing Default building facing degrees

Unit Group - Add (Last created unit) to AGroup

Set BGroup = AGroup

Unit Group - Remove (Last created unit) from AGroup

Unit - Remove (Random unit from BGroup) from the game

 JASS:

 local unit u = CreateUnit( Player(0), 'hfoo', 0,0,0 )
 call GroupAddUnit(udg_AGroup, u)
 set udg_BGroup = udg_AGroup
 call GroupRemoveUnit( udg_AGroup, u )
 call RemoveUnit( GroupPickRandomUnit(udg_BGroup) )

Normally even though we removed the unit from group AGroup, it should still exist to BGroup, so the unit should be removed from the map. But strangely as it is, the unit still remains on the map. That means that the unit was removed from both BGroup and AGroup. But how could this be possible? That is because when I actually said BGroup = AGroup I made BGroup point (contain) at the address of the same group AGroup pointed. As a conclusion, the two “contain” the same group, so assigning a handle variable a value means making it point to that value, and not copying it elsewhere in memory like it happened with non-handles. This is really important behavior and you will find out in the next chapters why.

3. Variable extensions

Even though this would be a subject about value types, I think I can discuss about it here. I mentioned before that all variable types except boolean, integer, real, string and code are extensions of the type handle. What is an extension?

Well, an extended type is one which inherits all the properties of the root type (handle in this case) and has some of its own. But then what are the properties of a handle? Well… in fact, a handle is an object that before you can use must be created (usually through functions). Once you no longer need it, the handle can be destroyed. Handles are more complex than a number, a true/false value or a serie of characters. These complex objects can have many properties. For example, a point is a pair of two reals, coordinates of a point. Groups contain addresses to multiple units. Timers are objects which have two real value (timeout and time left), one boolean value (repetitive or not) and the address of a code (to execute once the time left reaches 0).

The best example to understand inheritance (extending a type) is the value type “widget”. It is an extension of handle that besides the fact that it is an object, it is an object which has a life and a position. There are then other types extending a “widget” and more specifically a unit, a destructable and an item. The difference between them is obvious in-game so I won’t bother explaining it.

4. Leaks

First problem that usually occurs to handles is the “leaking” stuff. You all heard about it, you know that it consumes memory and blah blah, but ever asked yourself how and why does this happen? When creating an object (handle) we know that it is stored into the memory and so, of course it consumes part of it. However in order for us to be able to know where each object is created, every such creation functions usually return the address at which the object is stored, so it is utterly important that you store it immediately to a variable.

What happens if you don’t do this and you just use the function (for example Polar Offset/PolarProjection)? Well, the value is stored at an address in the memory, but we do not know exactly where. Integers, booleans and reals if stored into local variables are discarded from the memory when the memory. Perfectly logical because you can always mention those values, considering that they do not have variable properties like handles.

Note: It is said that strings themselves leak because once added to the memory, they are never removed. At this moment, there is no way to prevent this leak.

When it comes to objects though, their value does not “disappear” once variable points to another place in memory and is not discarded from locals once the function ends. Pretty obvious, considering that let’s say you would store the response unit (Triggering Unit)/GetTriggerUnit() to a local variable. It is unlogical to remove it from the game because the function where it was stored into a local ended.

However, there are certain handles (such as locations and groups) which should be removed once they are used, or else they will consume the memory, even though you no longer need them. Let’s take for example Polar Offset. You use it to move an unit in circle. Well, every position of the circle will be stored somewhere in the memory. Considering that there are 100 such units rotating and each rotation means 180 points, that’s 18000 locations stored into the memory when they are not needed for now. How can we solve the problem? We move the unit to the location and then we remove the location itself. Result? The memory is free and it is not uselessly consumed and kept “busy”.

Another known leak is the fact that handle-type local variables need to be nullified a.k.a removed at the end of the function, or else they will keep memory “busy” too. Why is that? Well, usually when creating a new object the program searches for a place into the memory to store it. Of course if a place in the memory is not free, it will not create the object over it, because it would overlap the value and cause problems.

As we know, variables point at a place in the memory. Apparently if the function ends while local is still pointing at a certain address, that address will be considered occupied and will not be used to store another value, even though it should be available. As a conclusion, before the function ends and the locals disappear, it is recommended that you make them point at the value “null”. It is a constant handle value that is never removed from the game. Forcing locals to point at it makes it free the previous byte at who’s address it was pointing at (of course if there is no object and there are no other variables pointing at it), not keeping it “busy” uselessly.

6. Conversion from one type to another

Even though this has again more to do with types than with variables, I believe it is utterly important to understand conversion from one type to another, in order to create more complex coding.

a) Numeric & String conversions

These are the basic and most important conversions, because they allow you to turn numbers (let them be real or integer) into text or vice-versa. You can also do numeric conversions (real to integer and integer to real). The procedure is very simple. Simply by using a function that takes as a parameter a type, you will get back the equivalent type.

In GUI when you want to do a conversion it is enough to replace the desired field with the appropriate function, which can be found under “Conversion – “. In JASS use the functions R2I & I2R (for real-integer conversions), R2S & S2R (for real-string conversions) and I2S & S2I (for integer-string conversions).

Trick:

If you want to turn a real value to string, the resulting string will contain the point and the decimals after it too. To get rid of them, you will first have to convert the real to integer, and then convert the resulting integer to string. Like this:

String(Integer(REAL_VALUE)) / I2S(R2I(REAL_VALUE))

*b) Any type of conversions

Due to a famous bug in JASS, it is possible to convert any type to any type (though sometimes this may lead to crash so be careful what you convert to what). First I’ll explain you the bug and then we will take some examples.

If in a function more than one “return” is used, the compiler will strictly check if the type returned by the last instruction is correct! Obviously a function that is supposed to return an unit cannot return a timer. However, if the type returned by the last instruction is correct, the compiler does not check the previous “return”s and you can put any type of value you want there, because you will get no warning. And to make things better, you can even use two returns one after another. The first one will be taken in consideration, while the second one will not (because a return ends the execution of the instructions after it into the function).

Example: The classical Handle to Integer conversion function looks like this.

 JASS:

function H2I takes handle h returns integer
 return h
 return 0
endfunction

Even though it may sound hilarious and silly, the function converts the handle to an integer. And that’s not all! Two different handles will always give two different integers. The explanation would be the fact that the returned value is the ID of the handle.

Even though this bug may not seem useful, it was used in systems such as Kattana’s HandleVars and Vexorian’s CSCache. My tutorial unfortunately does not cover these chapter but you can always check the code and see how they were made.

*7. Handling groups

Groups are probably the most difficult handles to operate with. They store the addresses of multiple units, and are very handy because instead of using the non-conventional unit arrays which are slow and hard to manipulate, the groups can easily work with the units they “store” because of the native functions operating with them. However, there are some important issues you should be careful when operating with them.

The most important one is how you grab the units from the group. The easiest method is using the native function ForGroup. However, it is pretty unhandy in some cases because it uses a code as a parameter for the list of instructions done upon each unit. So there is an algorithm to do this manually without losing locals and it looks like this:

 JASS:

loop
 set u = FirstOfGroup(g)
 exitwhen u==null
 call GroupRemoveUnit(g,u)
 //do what you have to do with unit u
endloop

The only problem is that once you do something with an unit from the group, you also remove it from the group. Hmm… not very good, isn’t it? The alternative people usually would choose is copying the group to another group. And I suggest that myself. But you must know how to do this. Because we know that doing something like w=g will not create a separate group.

First solution would be to use the native function GroupAddGroup which actually copies the units from a group into another group, while keeping the two handles distinct objects. To do this you will have to create first the destination group. It would look like this:

 JASS:

local group w = CreateGroup()
call GroupAddGroup(w,g) //copy group to another one
//do the sequence above
call DestroyGroup(g)
set g = w //it is safe now to make g point at the copy.

For those who do not like using non-natives I have another solution.

 JASS:

local group w = CreateGroup()
loop
 set u = FirstOfGroup(g)
 exitwhen u==null
 call GroupAddUnit(w,u)
 call GroupRemoveUnit(g,u)
 //do what you have to do with unit u
endloop
call DestroyGroup(g)
set g = w

Instead of adding the units to the secure group through a function, I made it manually. It’s quite the same thing, dunno which is faster, so you can use any you want and find better for your needs.

Well, that’s about it. If I think there are other things that I should add to this tutorial I certainly will. Feel free to post suggestions and comments. I’ll do my best to fulfill your needs.

~Daelin


 Last edited by a moderator: Feb 1, 2007

#### ragingspeedhorn

#### ragingspeedhorn

 Level 30

JoinedApr 30, 2005Messages5,499

Resources: 17

#### Resources
Icons 5Maps 7Tutorials 5

Very nice tutorial but couldn't you make use of the bold tags and such for headlines?

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Nice jass tutorial...


Stop making tutorials Daelin! You are gonna take all the memory on server!

#### ragingspeedhorn

#### ragingspeedhorn

 Level 30

JoinedApr 30, 2005Messages5,499

Resources: 17

#### Resources
Icons 5Maps 7Tutorials 5

You seem to have skipped a number in the headlines "1-2-3-4-6-7". Missing the 5'er

#### 1)ark_NiTe

#### 1)ark_NiTe

Patron

 Level 18

JoinedNov 1, 2006Messages1,612

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

they're always so long daelin, i wish i had the patience .

*edit* Now I have to read your tutorials for my classes


 Last edited: Dec 24, 2006

#### Kaedus

#### Kaedus

 Level 3

JoinedAug 31, 2005Messages29

Sorry to be a pain but i was trying to read these Jass thingys and i saw this


set j=(GetPlayers()-3)


Can you tell me what this is I can't seem to get this variable thing working. I gotta convert this to GUI.

#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

 JASS:

native GetPlayers takes nothing returns integer


This returns the number of available players in the game, so it is an integer value. Therefore, make sure j is an integer variable.


~Daelin

#### Mechanical Man

#### Mechanical Man

 Level 35

JoinedJan 18, 2005Messages5,781

Resources: 42

#### Resources
Models 32Icons 3Packs 1Maps 5Tutorials 1

Let me clear one thing:


In wc, oneshot* variable is not cleared after the end of trigger? Example:


Events:


Time - Unit Dies


Conditions:


Owner of (dying unit) equal to Player 6


Action:


Unit Group - Pick all units owned by Player 12 and order(picked unit) to move to Position of (dying unit)


This means that this trigger after repeated executions will cause game to lag ever more?


*oneshot - not a well-chosen term, I mean the variables that are created by certain functions and are not avalaible to user.

#### Kaedus

#### Kaedus

 Level 3

JoinedAug 31, 2005Messages29

Ok thanks for that. Also for the conditions theres the equal to and not equal to.


Does "=" mean equal to and "!=" mean not equal to?

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

no:


== is equal


!= is not equal


>= is grater or equal


=< is lesser or equal


> is greater


< is lesser

#### khopesh

#### khopesh

 Level 9

JoinedDec 20, 2025Messages129

Great tutorial, I have a few questions:

How many bits do the variables weigh?

Player group

Point

Unit group


 Last edited: Jan 31, 2026

 You must log in or register to reply here.


### Similar threads

 Local Variables


jonadrian619

May 19, 2007

Tutorial Archive

Replies7Views28K

Jun 27, 2007
Wolverabid

 Beginning JASS Tutorial Series


wyrmlord

Feb 16, 2007

Scripting Tutorials
234

Replies181Views124K

Feb 18, 2025
Eastern Programmer yahoo

 JASS Tutorial


Daelin

Mar 20, 2005

Scripting Tutorials

Replies24Views38K

Nov 4, 2006
Daelin

 Typecasting - Handle to integer


Diablo-dk

Jun 7, 2007

Graveyard
2

Replies53Views11K

Aug 5, 2009
aznricepuff

 Variables


Ralle

Jan 3, 2006

Trigger (GUI) Editor Tutorials

Replies11Views42K

Jul 17, 2021
Daffa

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