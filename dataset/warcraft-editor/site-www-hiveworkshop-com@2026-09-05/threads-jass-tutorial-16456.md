---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/jass-tutorial.16456/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# JASS Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/jass-tutorial.16456/>
#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Jass Tutorial

1.Introduction


Most of you probably heard of JASS and that it is an efficient way to develop spells. But the real question is probably, what exactly is jass and how can you learn it? Like many other programming languages, JASS is a language developed by Blizzard. If you have basic knowledge in C++, Pascal or any other non-object-oriented programming languages, then this tutorial will be piece of cake. If not, you will have some difficulties but in the end, you will make it. Before continuing with this tutorial, I suggest you first learn how to basically work with triggers. The tutorial will be based on triggering knowledge so without knowing triggers, it is most likely that you won’t get much.


Try this tutorial about trigger enhanced spells if you don't know how to make this type of spells: Basics of Trigger Enhancing Spells

Note: If you ever hear the concept of “Custom script” in Warcraft, then that’s an alternative name for JASS.

Note 2: Before you continue, make sure you have this JASS editor.

2.Why JASS?


Simple, because first of all, advanced JASS can help you get multi-instanceable spells. Multi-instaceability is the only way, which can help you in case you want to do normal unit spells and you can’t make them Multi-instanceable with triggers. Multi-instaceability for those who don’t know is the capacity of a spell to be cast by more than one unit of the same player at the same time. Another use of jass is speed. Once you get jass, you can get spells much faster, and to be honest, they are sometimes more efficient. And another thing I noticed about JASS is how to obtain awesome effects like a smooth Knockback. Believe me, I’ve tried with triggers but you’ll never get the awesome effects you will get with JASS.


Where you shouldn’t use JASS because you don’t need to use JASS at everything. During campaigns when you make quests, cinematics and other things which can be easily done with triggers. You don’t need to bother with JASS because sometimes it can be buggy.


Where you should use JASS… In totally recommend it in spellmaking, if you want to make cool minigames in maps and of course cool effects. You have more freedom and you are no longer restricted to FOR repetitive actions. If this is not enough for you, just try it and you will not regret.

3.Functions


Functions are parts of a program (script) which execute a limited number of actions (they can be made to execute an unlimited number if you use an infinite for but then the script isn’t correct), one after another. Imagine them as the actions of a function, the actions following the singular action “Pick Up Every Unit In Group and do Actions” etc. To create a function you have to use the following structure:

 JASS:

function name takes nothing returns nothing
//actions executed by the function
endfunction


And now to explain. The words function and endfunction start the function, respectively close it. In place of name you can put the name of the function. It can be anything as long as you don’t leave spaces. "Oz02” is a valid name while “Oz 02” is not a valid one. However, all the functions must have an unique name. If two functions have the same name in the whole map then you will get an error (this includes different scripts).

Note: Comments in JASS must have “//” before them. Everything on the row after the “//” is ignored by the script.


Between “function” and “endfunction” you can put all the actions you want that function to execute. Take them as the actions of a trigger by now.

Note: You may create as many functions as you like, but keep in mind that you cannot create a function into another function. Example:

 JASS:

function sleep takes nothing returns nothing
 function entangle takes nothing returns nothing
 endfunction
endfunction


This is not a valid script. A correct script would be like this;

 JASS:

function entangle takes nothing returns nothing
endfunction
function sleep takes nothing returns nothing
endfunction

4.Executing a function


Functions must be executed just like triggers. But unlike triggers they must be directly called and do not start to execute if a certain event takes place. Consider them as an initially off trigger which can be executed only through

 Run Trigger Ignoring Conditions


Click to expand...

 action. There are exceptions of such functions, which will be mentioned later.


To call a function, you must use the following structure:

 JASS:

call name

Again, name is the name of the function.


A function can call another function but keep in mind that the function called must appear into the script before the “calling” function.


The following script is incorrect:

 JASS:

function sleep takes nothing returns nothing
call entangle
endfunction
function entangle takes nothing returns nothing
endfunction


The following script is correct:

 JASS:

function entangle takes nothing returns nothing
endfunction
function sleep takes nothing returns nothing
call entangle
endfunction


All the actions you execute in triggers are functions as well. They are premade actions and can directly be called.


Example: the function “TriggerSleepAction” is the wait function but it will be explained later.

Note: Your functions’ name may not be the name of a premade function. To see all the premade functions look into the JASS editor.

5.Variables


Variables are labeled places in the memory which store a certain value. By being labeled, they obviously have a name just like functions. Like in GUI (triggers), there are many types of variables. The most important ones are:

 lightning – lightning in gui


location – point in gui


unit – unit in gui


effect – special effect in gui


item – item in gui


doodad – doodad in gui


timer – timer in gui


group – unit group in gui


trigger – triggers themselves in gui (this will be discussed later)


Click to expand...

integer – integer in gui


real – real in gui


boolean – boolean in gui


There are two types of variables:


a) Local variables – variables declared in functions and which can be used only in that certain function. To declare a variable you must use the following syntax:

 JASS:

local type name

.


 Type is the type of the variable and name is the name of the variable. Keep in mind that you can’t have more than one local variable with the same name in the same function. However, you can have local variables with the same name, as long as they are in different functions. Example:

 JASS:

function sleep takes nothing returns nothing
local unit cast
endfunction
function entangle takes nothing returns nothing
local trigger cast
endfunction
function arrow takes nothing returns nothing
local unit cast
endfunction


To refer to local variables just use their name. We will discuss referring in the next chapter.

Warning: locals must be declared at the beginning of the function before any other actions are executed. The following example is wrong:

 JASS:

function entangle takes nothing returns nothing
endfunction
function sleep takes nothing returns nothing
local trigger t
call entangle
local unit cast
endfunction


b) Global variables – the normal GUI (trigger) variables, created just like you created them when you used triggers. To refer to global variables use the structure “udg_name” where name is the name of the global variable.

Note: Local and Global variables can have the same name. They won’t get confused because globals are referred with the udg_ in front of them while locals are referred only through their name.


There is also a special variable type called handlers. This type of variable exists and can be used only as local. It is special because it can store all kind of variables. I personally experimented only with doodads, items and units but it should work with most of the variable types. However, I doubt that it works with integer and real.

6.Decisional and Repetitive structures


Let’s talk a little bit about the decisional (if) and repetitive (loop) structures.


a) Decisional structure


 Decisional structure in JASS is pretty simple. It has the following structure:

 JASS:

if <condition> then
//Actions then
else
//Actions else
endif

Note: The condition was put between <> so you can see it better. The script itself must not include the <>.


Ok, the condition must be a comparision. If that comparision is true, then the “actions then” are being executed. If not, the “actions else” are executed. However, keep in mind that the else is not mandatory. But if you don’t put an else, don’t put the “Actions else” either. Also, don’t forget the endif because it closes the if.


b) Repetitive structure


For JASS, we only have the while structure in C++ and Pascal, which here is called “loop”. It has the following syntax:

 JASS:

loop
 exitwhen <condition>
 //Actions
endloop

WARNING!: If you forget the endloop and you don’t close the loop, then the WE will cause a crash and you will lose all the data if you didn’t save before starting to work on the loop. So make sure you close each loop you start.


Not much to say. The loop makes the actions repeat until the comparision after the “exitwhen” returns true. Until then, it will keep executing the actions.

7.Functions which take and/or return a value


By now we have talked only about plain functions which take and return no value. Such functions rarely exists and are usually used in triggers (yes… we are going to use triggers in JASS as well so practically the “trigger method” will be mentioned from now only as GUI so we don’t mix them up.


Ok, I told you before that the structure of a function begins with “function name takes nothing returns nothing”. In reality, in place of the two nothings you can put one or more variable types (followed by the variable’s name in the case of the taken variables). The variables after the takes are values which are taken by the function when you call it, while the ones after return are values returned by the function.

Note: Functions may return only one value.


Let’s take an example of function, which takes a value. In this function we will work for good with some variables, I will show you a premade function (those functions which are actions in GUI) and show you how to call a function which takes a value.

 JASS:

function remove takes unit cast, real wait returns nothing
call TriggerSleepAction(wait)
call RemoveUnit(cast)
endfunction

function start takes nothing returns nothing
call remove(udg_caster, 1.00)
endfunction


Let’s analyze this jass script a bit. The start function calls the remove function and it includes the two parameters. As you can see, the parameters are placed between brackets and are separated between each other through a coma. The first parameter is another global variable, while the second one is a real constant.

Note: Constants are values, which don’t change their value no matter what.


Let’s have a look into the second function. As you can see the cast and wait values no longer need to be mentioned as locals. That is because all taken values are already considered locals. And now let’s look at the premade functions.


As I mentioned before, the call TriggerSleepAction function is a wait, and makes the function wait a number of seconds before going further. The value in the brackets (a parameter) is the number of seconds you have the TriggerSleepAction waits.

Note: the minimum parameter for the wait is 0.10. If you put lower, it will be automatically set to 0.10 when the function is executed.


The second function “RemoveUnit” is the gui “Unit – Remove Unit’ action and like there, it removes the parameter unit. In this case, it removes the “cast” local unit.

Warning! Everytime you call a function, which takes a value you must put mention all the parameters and the parameters must have the same type as the values taken into the function called.


In the case of returning a value, the returned value must be either taken into consideration either into a logical expression or stored into an integer. Let’s take another example:

 JASS:

function condition takes unit cast returns boolean
return IsUnitDeadBJ(cast)
endfunction

function start takes nothing returns nothing
call TriggerSleepAction(0.01)
if condition(udg_cast)==true
call RemoveUnit(udg_cast)
endif
endfunction


When function start begins, it waits 0.10 seconds (because values under 0.10 in TriggerSleepAction are taken as 0.10) and then it calls the condition function. Since the function takes a value, the call has a parameter as well.


The second function returns a boolean. This means that it will return a boolean value. It uses the function IsUnitDeadBJ(cast) which checks if an unit is dead. If that unit is dead, then it returns true. If not, if returns false. Moreover, the function returns the value returned by the IsUnitDeadBJ function itself. If it sounds too complicated, try to imagine that in place of the function, there is a boolean value, the value which is actually returned by that function.


And lastly, you have the return followed by the value returned. This makes your function return a value. Keep in mind that the value after the return must be the same value returned by the function itself.

Observation: Once your function returns a value, it will skip over all its further actions.

Note: You don’t and mustn’t use the call before functions whenever you directly use the value of that function. If you don’t use the value returned by the function, you can use the call.

Warning: Everytime a function must return a value, you must assure that it returns that value. If you don’t, it will make the WE crash. There are some strange situations in which, even though the function should always return a value, you get an error. Let’s have a look at the example below:

 JASS:

function bool takes nothing returns boolean
if udg_a==true then
return true
else
return false
endif
endfunction


Strangely, the WE doesn’t take in consideration that a value is returned anyway. It will crash. So you must assure that you have a return outside any FORs, Ifs and other such structures. This will prevent crashes. The IF trap is the most common and most annoying. So be careful!

8.Working with variables


This chapter is also very important because we are going to learn how to give variables a certain value. Let’s take the following example:

 JASS:

function entangle takes nothing returns nothing
local unit cast
set cast = GetTriggerUnit()
call RemoveUnit(cast)
endfunction


What does it do? We take a local unit with the name cast. Now, we can give variables a value by typing “set”, leave a blank and then the name of the variable. After that we have an “=”, and then what you give that variable. In this case we gave it the value of the function GetTriggerUnit(). This function is another premade and returns the Triggering Unit (GUI). After that we removed the cast unit.


I wrote down the most important such premade functions which work fine with unit variables.

 GetTriggerUnit() = Triggering Unit


GetEventDamageSource() = Damage Source


GetLastCreatedUnit() = Last Created Unit


GetAttackedUnitBJ() = Attacked Unit


GetAttacker() = Attacking Unit


GetSpellAbilityUnit() = Casting Unit


GetDyingUnit() = Dying Unit


GetEnteringUnit() = Entering Unit


GetManipulatingUnit() = Hero Manipulating Unit


GetKillingUnitBJ() = Killing Unit


GetOrderedUnit() = Ordered Unit


GetSummonedUnit() = Summoned Unit


GetSummoningUnit() = Summoning Unit


GetSpellTargetUnit() = Target Unit of Ability Being Cast


GetOrderTargetUnit() = Target Unit of Issued Order


Click to expand...

Note: GetSpellTargetUnit(), GetOrderTargetUnit() and other such functions do not return a value after a GetTriggerSleepAction() no matter how small it is (how short it lasts). Exception is GetTriggerUnit(). That is why I suggest you store all the units you will need into local variables.

9.Triggers


Ok, by now we haven’t worked with real JASS scripts, but with small parts. If you would try these scripts, you would probably get a series of errors or atleast they wouldn’t work. That is because your functions must be first triggered by something. So yes, triggers exist in JASS as I said before. There are two types of triggers: Global triggers (the ones seen in GUI) and Local triggers. We will talk first about global triggers and of course, how to create your first JASS script.


a) Global triggerss


First thing’s first. Create a GUI trigger. Give it as an event, “An Unit Starts the Effect of an Ability” and as a condition “Ability Being Cast equal to Sleep”.


 Now, you will be amazed how simple can JASS be. Go to Edit – Convert to Custom Text and click on “Ok”. Custom script will appear in the right. Amazing, eh? Let’s have a look at it.

 JASS:

function Trig_Untitled_Trigger_001_Conditions takes nothing returns boolean
 if ( not ( GetSpellAbilityId() == 'AUsl' ) ) then
 return false
 endif
 return true
endfunction

function Trig_Untitled_Trigger_001_Actions takes nothing returns nothing
endfunction

//===========================================================================
function InitTrig_Untitled_Trigger_001 takes nothing returns nothing
 set gg_trg_Untitled_Trigger_001 = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( gg_trg_Untitled_Trigger_001, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition( gg_trg_Untitled_Trigger_001, Condition( function Trig_Untitled_Trigger_001_Conditions ) )
 call TriggerAddAction( gg_trg_Untitled_Trigger_001, function Trig_Untitled_Trigger_001_Actions )
endfunction


It looks terrible and perhaps you don’t get a lot of stuff. So let’s have a look at it together. It starts with a conditional function, which returns a boolean. It uses a function GetSpellAbilityId() which gets the rawcode of the ability being cast. This function returns a a rawcode so imagine in place of it a rawcode. Now, if the rawcode returned is equal to ‘Ausl’ then the function will return a false value and will skip remaining actions. It doesn’t sound correct, doesn’t it? But look at the not in front of the equality. It actually negates the value of the equality. So if the equality is true, it returns a false value. Not very efficient but later I will teach you how to make it simpler. Blizzard like to complicate themselves.


Back to the function, if it doesn’t return the false value, the function will continue, it will exit the if and will return a true value.


The second function represents the actions of your trigger. By now it is empty because you didn’t input any actions. We will discuss this chapter later as well.


And now the function follows some strange comment. It separates the functions with the global trigger. Let’s look beyond them. The function following is like a trigger. It activates at the initialization of the map. It must have the same name as the trigger. I haven’t manipulate such functions before so you don’t have to worry about them just by now.


Below you will see a global trigger being created. Note that all triggers must be created before they can be used. Before that, they will simply exist as variables but they will never be used. The same goes with timers.


Now, we have three interesting functions, which will be frequently used. The first registers an event. The second one registers conditions. The third one registers an action. They are more complicated so we will take them one by one. Before we start, open the JASS Editor.


I. Functions registering events


It is obvious that there are very many events but once you learn how to work with them, it will be very simple. All events start with the following syntax:

 TriggerRegister


Click to expand...

.


The syntax is then followed by different keywords. To see all the events existent type into the JASS editor TriggerRegister. Make sure that the “functions” button is pressed. To make it easier, if you still don’t get which event is which, make a GUI trigger, put the event you want and then convert the GUI trigger. You will see the event in JASS.

Note: You can register as many events you like. Let’s take an example:

 JASS:

function main takes nothing returns nothing
set gg_trg_t = CreateTrigger()
call TriggerRegisterAnyUnitEventBJ( gg_trg_t, EVENT_PLAYER_UNIT_DEATH )
call TriggerRegisterAnyUnitEventBJ( gg_trg_t, EVENT_PLAYER_UNIT_SPELL_EFFECT


In this trigger, we have two events: one, which activates when an unit dies and the other when an unit starts the effect of an ability. Usually all event initialization functions have as a first parameter the name of the trigger. After that it depends on the event. I cannot really tell you much about this chapter. Just experiment by converting from GUI. I am only helping you implement these.


II. Functions registering conditions


These functions are much simpler and much easier to explain. We have already talked about them into the “Functions which take and/or return a value” chapter. Check it if you are unsure yet.


Now, the structure to add a condition is only one:

 JASS:

call TriggerAddCondition(trigger_name, Condition (function name))


In place of trigger_name you will put the name of the trigger whose Condition it will be. In place of function name you will put the function which will check a certain condition.

Warning: in this case, the function which will take place of the condition may not take any values (parameters). That is why you won’t be able to transfer locals from one trigger to another, atleast not yet.

Note: the condition function MUST return a boolean value. If it doesn’t, you will get an error.


Again, you have to experiment here before you can get good results. I will show you how to obtain efficient conditions, and not inefficient ones (like Blizzard uses). We will take a more elaborated trigger, which activates only when an unit casts a certain ability.

 JASS:

function Cond takes nothing returns boolean
return GetSpellAbilityId() == ‘A000’
endfunction

function trigger takes nothing returns nothing
set gg_trg_t = CreateTrigger()
call TriggerRegisterAnyUnitEventBJ( gg_trg_t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
call TriggerAddCondition(gg_trg_t, Condition(function Cond))
endfunction


Examining it is simple. The first function returns the value returned by the equality GetSpellAbilityId() == ‘A000’. About the second one, it creates the global trigger and adds the event and the condition. If you’re having problems try to

Warning: You can add only one condition! But you can manipulate them into the function better!


III. Functions registering Actions


This is the shortest and simplest chapter. It works like conditions but has a slightly different syntax:

 JASS:

call TriggerAddAction(trigger_name, function name)


So practically you no longer need the Condition in front of the function name. That is all. And now I will take a more complex trigger, which simply kills the target unit when attacked:

 JASS:

function Cond takes nothing returns boolean
return GetSpellAbilityId() == ‘A000’
endfunction

function Act takes nothing returns nothing
local unit targ
set targ = GetSpellTargetUnit()
call KillUnit(targ)
endfunction

function main takes nothing returns nothing
set gg_trg_t = CreateTrigger()
call TriggerRegisterAnyUnitEventBJ(gg_trg_t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
call TriggerAddCondition(gg_trg_t, Condition(function Cond))
call TriggerAddAction(gg_trg_t, function Act)
endfunction


Note: You can add only one action. Moreover, the function must not take or return any value.


b) Local triggers


There are triggers, which can be created and declared into functions. In this way, they can take place only for certain units, and you can make this multi-instance by using the locals. Local triggers work just like globals, which some slight differences:

 JASS:

function Remove takes nothing returns nothing
call RemoveUnit(GetTriggerUnit())
endfunction

function act takes nothing returns nothing
local trigger t
set t = CreateTrigger()
call TriggerRegisterUnitEvent(t, GetTriggerUnit(), EVENT_UNIT_SPELL_ENDCAST)
call TriggerAddAction(t, function Remove)
endfunction


Not very complex. The second function has a local trigger. It activates when the Trigger Unit of the function stops casting an ability. The action of the trigger is function Remove, which actually removes the Triggering Unit of that function, which is of course the Triggering Unit in the second function who stops casting an ability.

10.Variable Leaking


One disadvantage of variables (either units, doodads, items, triggers or anything else except numeric values like integers and reals) is that they leak. What does this mean? That even after they are used, if not removed properly from the memory, they will remain there and so, consume your resources. Too many JASS triggers that leak can cause extreme lag of the map, thing unwanted by any programmer. So how can we prevent leaking?


a) Nullify variables after they have been used


Not very suggestive the title, isn’t it? Ok, the whole idea is to actually get the variable out of the memory by setting its value to what it has initially been before the trigger was executed. That value is called “null” and it obviously means that the variable is empty, that it doesn’t contain anything. Let’s take an example;

 JASS:

function Trap takes nothing returns nothing
local unit cast
set cast = GetTriggerUnit()
call TriggerSleepAction(1.50)
call RemoveUnit(cast)
set cast = null
endfunction


At the beginning of the function where we declared the variable through the syntax “local unit cast”, the cast variable received the value null. This means that it doesn’t occupy anything in the memory but its type. However, when we gave the variable a value (Triggering Unit), we stored that certain unit into the memory under a label: the name of the variable. After the wait we have removed the unit. However, the memory still contains something, even though the unit is removed. And so, we need to give it the initial value: null.


I’m not really sure that if you remove the unit, it will still leak but one thing is clear: if you want to manipulate the “Target Unit of Ability Being Cast”, and manipulate it through waits, your only option is locals (or globals which btw, will soon be almost completely out whenever you will se JASS). If you don’t actually remove the Target Unit of Ability Being Cast, it is clear that it will remain in the memory, stored through the variable. And then the only way to remove it from the memory, is to nullify the variable.

Note: You may not nullify integer and real variables, even if you want to. That is because they don’t leak.


b) Destroying timers, effects, lightning and triggers


In some cases, simply nullifying a variable is not enough. This is the case of the timers, special effects, lightning and triggers. In their case, you will have to destroy them before you can remove them. Why is that? Because for example, triggers are more than simple objects. They activate depending on certain actions and so, they don’t depend on themselves like objects do. So how do you destroy these things?

 Timers – call DestroyTimer(name)


Effects – call DestroyEffect(name)


Lightning – call DestroyLightning(name)


Triggers – call DestroyTrigger(name)


Click to expand...


I can tell you that in the case of the timers for example, if they are repetitive and they call a certain action after each expiration, they will continue to repeat and call that action even if you nullify the variable in which they are stored. In case of triggers, they will start to occur even if you nullify the variable. Special Effects and Lightning cause severe lagging if they are not destroyed once they have been used. Try to use a spell with many Special Effects. Summon that spell more than once but let the effects be removed before summoning it again. You will notice that after some time the game will start to lag.

Note: You MUST NOT nullify the variables before you destroyed what they contained. If you do so, you will not be able to destroy that anymore. Example;

 JASS:

function Remove takes nothing returns nothing
call RemoveUnit(GetTriggerUnit())
endfunction

function triggy takes nothing returns nothing
local trigger t
set t = CreateTrigger()
call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
call TriggerAddAction(t, function Remove)
call TriggerSleepAction(10.00)
set t = null
call DestroyTrigger(t)
endfunction


This trigger removes any unit that starts the effect of an ability. However, usually it should be destroyed after the 10 seconds, making it to no longer exist. But in this case, you first nullified the variable and THEN you destroyed the trigger. That is why the trigger will not be destroyed because you will practically destroy something null.


To find out more about leaks, check this Tutorial. Its the best about leaks I've ever seen.

11.Timers


Timers work somehow like triggers but the only difference is that they start the actions after a period of time, regardless of the circumstances. Moreover, they can start that action more than one if they are repetitive timers. And to make things greater, the timers do not take in consideration the 0.10 seconds limit which is set for TriggerSleepAction(). They can be pushed down to 0.01 and that is why you can obtain smooth effects like knockbacks, and moving units again smoothly. Let’s take the following script. It may look very complex but in reality, you mostly know what happens.

 JASS:

function Cond takes nothing returns boolean
return GetSpellAbilityId() == ‘A000’
endfunction

function Knockback takes nothing returns nothing
call SetUnitPositionLoc(udg_targ, PolarProjectionBJ(GetUnitLoc(udg_cast), DistanceBetweenPoints(GetUnitLoc(udg_cast), GetUnitLoc(udg_targ))+10, AngleBetweenPoints(GetUnitLoc(udg_cast), GetUnitLoc(udg_targ))))
endfunction

function Start takes nothing returns nothing
local timer t
set udg_cast = GetTriggerUnit()
set udg_targ = GetSpellTargetUnit()
set t = CreateTimer()
call TimerStart(t,0.03,true,function Knockback)
call TriggerSleepAction(2.00)
call DestroyTimer(t)
set udg_cast=null
set udg_targ=null
set t = null
endfunction

function InitTrig_trigger takes nothing returns nothing
local trigger t
set t= CreateTrigger()
call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
call TriggerAddCondition(t, Condition(function Cond))
call TriggerAddAction(t, function Start)
endfunction


Warning: In order to make the spell work you need to follow these instructions:


- Base your spell off cripple and make sure it is the only and first spell created in your map so it can have the rawcode ‘A000’


- Make sure that the name of the trigger is trigger. The name is case sensitive so be careful.


- Make sure that you have two global unit variables with the following names (they are case sensitive as well): cast and targ.


Analysis is necessary here. You probably get the conditional function and the last one so we’ll get to action function called Start. Well, we took the two global variables and gave them the values of the target unit, respectively, triggering unit. To make this multi-instance (locals), we need to study the chapter Handlers which by now is a little bit too advanced I will update this tutorial later once I assure it is efficient and that you understand everything by now.


Now, we have a timer which we create to last 0.03 seconds, be repetitive (the yes), and once it expires, to have him start the function Knockback. We can say that it works just like a timed trigger but which works better since it is repetitive. We then have a 2 seconds wait, so practically the timer will launch 66 times. After that, we destroy the timer to assure that it no longer launches and after that we set both the globals and the local to null, to save memory.


As for the Knockback function, it may look bizarre but again, it is very logical. Everytime it is launched it moves the targeted unit instantly (SetUnitPosition function) using the Polar Offset (PolarProjectionBJ in JASS). The Polar Offset function also has the following parameters: The center point (position of the casting unit), the distance between the first and the second point (which in this case it is the distance between the two units + 20) and the angle between the two points (which is the same because the unit is only pushed back so it retains its angle). I hope you got it!

12. Pick Up Every Unit


I’ve decided to make a separate section for this function since it is very tricky and only if you take a smart alternative, you can obtain multi-instanceability. Only lately I’ve discovered it. Thanks to Vexorian to clearing things up!


This time, we are going to use the group variable. We are going to take the following script. It is an AoE sleep, which causes all the units in an area of effect to get affected by sleep. Before we begin, I suggest you have a dummy unit, a dummy spell based off silence with 0.01 duration and a sleep spell for the dummy unit. I will consider the following rawcodes:

 dummy – ‘h000’


sleep - ‘ A001’


mass sleep (silence) – ‘A000’


Click to expand...


And now let’s make the script. Your trigger will be called masssleep (CASE SENSITIVE).

 JASS:

function Cond takes nothing returns boolean
return GetSpellAbilityId()==’A000’
endfunction

function Mass_Sleep takes nothing returns nothing
local group g
local unit u
local unit cast
local unit dumb
local location p

set cast = GetTriggerUnit()
set p = GetSpellTargetLoc()
set g = GetUnitsInRangeOfLocAll(800.00, p)
set u = FirstOfGroup(g)
loop
 exitwhen u==null
 set u = FirstOfGroup(g)
 if IsUnitEnemy(u, GetOwningPlayer(cast))==true then
 call GroupRemoveUnit(g,u)
 set dumb = CreateUnitAtLoc(GetOwningPlayer(cast), ‘h000’, GetUnitLoc(u), 0.00)
 call IssueTargetOrderBJ(dumb, “sleep”, u)
 call UnitApplyTimedLifeBJ (1.50, ‘BTLF’, dumb)
 set dumb = null
 endif
endloop

set g = null
set u = null
set cast = null
set p = null
endfunction

function InitTrig_masssleep takes nothing returns nothing
local trigger t
set t=CreateTrigger()
call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SPELL_EFFECT)
call TriggerAddCondition(t, Condition(function Cond))
call TriggerAddAction(t, function Mass_Sleep)
endfunction


Much to explain but hopefully this information will help you. I’ll go directly to the actions. The function GetUnitInRangeOfLocAll gets all the units in the range of a point. If you wanted to select only the enemy ones, you would’ve needed to use another function, and for that you would’ve lost the target point.


Now, FirstOfGroup gets the first unit into an unit group. It doesn’t matter which is the first unit since in the end we will get all of them. Now, you will wonder why I gave the variable u the value of FirstOfGroup(g) both outside and inside the loop. If I would’ve given it the value only inside, the unit would’ve been null when we would’ve entered the loop. And since the condition that u==null would’ve been true, the loop would’ve been skipped.


IsUnitEnemy function checks if a certain unit is an enemy of a player. GetOwningPlayer gets the owner of a unit, in this case the Triggering Unit stored into a variable. Function GroupRemoveUnit removes an unit from a group. If I wouldn’t have done this, then everytime the loop would’ve selected the first unit into the group which would the already stored in u and so, it would keep picking u and storing it into himself. If you remove it from the group, then another first unit would appear in the group. Keep in mind that if there are no units left into the group, then u will be null. That is the trick of ending the loop when u is null.


Now, about creating the dummy unit, CreateUnitAtLoc function doesn’t allow you to use GetLastCreatedUnit() so you can get it after it was created. The function itself returns a value but it doesn’t matter since you could’ve still used it in the form “call CreateUnitAtLoc” but in this way you wouldn’t have been able to track that unit anymore. The ExpirationTimer function assures that the unit disappears after a number of seconds. Usually this was made before to achieve multi-instanceability. In this case, keep in mind that the dummy unit cannot be detected after the loop repeats because another unit is created and another unit is stored into that variable.


You can notice that then I nullified all variables except u because when you exited from the loop, it was already null.

13.Arrays


Arrays are close to any other programming language. The only problem in JASS is that you cannot have more than a unidimensional array. So you cannot obtain matrixes, atleast in the nice and classical way. Let’s see how variables are declared and used:

 JASS:

local unit array a
set a[0]=GetTriggerUnit()
set a[1]=GetDamageSource()


So you declare them with the syntax local, variable type, array, variable name. And to get them you use the variable’s name and then the parameter where you stored the value. That’s all you have to know about arrays.

14. Conclusions


I suggest you experiment with premade functions and JASS since this only gives you an idea of what custom script means. It’s up to you to experiment further actions. Everytime you can’t get an action, combine the JASS editor and the conversion from GUI to JASS. Even I use it sometimes since it is impossible to remember so many functions. I am waiting for your replies and suggestions to improve this tutorial. "Handlers" chapter will hopefully be added soon. I have finally discovered what handlers are, though I still don't see the whole point of using them except in some situations.


~Daelin


 Last edited by a moderator: Mar 11, 2007

#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

All examples before I start showing you triggers don't work standalone since they were made only to show you how scripts look. Only after they start to!


And moreover this script doesn't really work if you don't change the trigger's name accurately and other stuff. So ignore it:

 JASS:

function Trig_Untitled_Trigger_001_Conditions takes nothing returns boolean
if ( not ( GetSpellAbilityId() == 'AUsl' ) ) then
return false
endif
return true
endfunction

function Trig_Untitled_Trigger_001_Actions takes nothing returns nothing
endfunction

//===========================================================================
function InitTrig_Untitled_Trigger_001 takes nothing returns nothing
set gg_trg_Untitled_Trigger_001 = CreateTrigger( )
call TriggerRegisterAnyUnitEventBJ( gg_trg_Untitled_Trigger_001, EVENT_PLAYER_UNIT_SPELL_EFFECT )
call TriggerAddCondition( gg_trg_Untitled_Trigger_001, Condition( function Trig_Untitled_Trigger_001_Conditions ) )
call TriggerAddAction( gg_trg_Untitled_Trigger_001, function Trig_Untitled_Trigger_001_Actions )
endfunction


~Daelin

#### Draqz

#### Draqz

 Level 7

JoinedFeb 15, 2005Messages183

Resources: 5

#### Resources
Maps 4Tutorials 1

Could you explain in a little more detail, how to make spells "multi-instancible"? And I really hope i don't have to convert my entire trigger to JASS, seriously, it would be like pages.


 Last edited by a moderator: Apr 18, 2007

#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Multi-instanceability assures that more than one unit can cast the spell without any variable problems. It will be cast correctly and the variables will be assigned values correctly.


And for the lightning, it is strange. Did you try the JASS like this:

 JASS:

local lightning l
CreateLightning() //I won't enter in details here, I don't even know if this is the function
set l = GetLastCreatedLightningBJ() //Again, not sure if it is the right script... convert and look
call TriggerSleepAction(1.00)
call DestroyLightningBJ(l)
set l = null


Use convertion from GUI to JASS to check the CreateLightning and GetLastCreatedLightning and the DestroyLightning. I don't know Lightning very well since they are very new.


And yes, you have to convert the WHOLE TRIGGER into JASS. That is why I suggest you make your spells from 0 (umm... sounds sucky but yep, its better) and whenever you don't know an action, create a separate GUI trigger, get that as a single action and try it.


Multi-instanceability is very simple (unless you have very advanced spells which are not necessary by now). If you use the PickUpEveryUnit system mentioned by me you shouldn't have problems. And multi-instance is accomplished only through locals. So you will just have to convert your GUI trigger and replace all global variables with local variables. Make sure you create the locals at the beginning of each function.


It is very hard at the beginning, I know. But don't give up. Start with simple spells. Try to comprehend how to make simple spells (just like you started in GUI) and don't give up. You may need to go step by step like in GUI. Once you get the hang of it (maximum a month), you can work with JASS like a dream.


Tutorials are not enough. Work is needed a lot. You need to work with functions, though I think I kinda explained things here. It looks strange but try to look at it line by line. That is why it is very complicated to do with just GUI => JASS conversion. The best way is to make it line by line and eventually use comments near each action.


Experiment and have fun.


~Daelin

#### mems

#### mems

 Level 5

JoinedMay 2, 2005Messages40

Resources: 8

#### Resources
Icons 6Maps 2

i also have a question about memory leaking


Does giving a unit expiration timer removes the memorial things about that unit?

#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

It removes the unit itself cleanly, but you still need to clear the variables pointing at that unit.


~Daelin

#### mems

#### mems

 Level 5

JoinedMay 2, 2005Messages40

Resources: 8

#### Resources
Icons 6Maps 2

and how to carry local variables between local triggers or is it possibe?

#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Yes it is, using Kattana's handlers system. Check my Defensive Matrix spell since it give an example about how they can be used. I will try to get some time to add that chapter too!


~Daelin

#### Modeler

#### Modeler

 Level 9

JoinedAug 27, 2004Messages471

Resources: 1

#### Resources
Maps 1

Finaly got around to asking theyse:


Lets say i had a trigger:

 JASS:

function A_001 takes nothing returns nothing
if (A > 1)
call DoNothing( )
else
set A = 1
endif
endfunction

function A_001_TEST takes nothing returns nothing
local real A = 1
call A_001()
endfunction

//================

function IniTrig_Amp_001 takes nothing returns nothing
local trigger t
set t = CreateTrigger( )
call TriggerAddAction( t, function Trig_ A_001_TEST)
endfunction


Probably doesnt work. But what im asking is, does that local variable still exist, or do i need to transfer it via: "takes var"?


And i notice, in the trigger editor, there is the name of the map, and when you click it, theres a box where comments usualy go that says:


Enter map-specific custom script code below. This text will be included in the map script after variables are declared and before any trigger code.


-Does this mean that you can call functions from it, rather than putting them into the script that calls it (Just so long as the call isint being made from this location)?


Btw great tutorial. ^,^ (Still working on your campaign?)

#### BertTheJasser

#### BertTheJasser

 Level 4

JoinedMar 19, 2005Messages95

No it doesn't work!


You must use HandleVars which use the blizzard bug(maybe it wasn't a real bug,maybe blizzard wanted that, nobody knows)


function H2I takes handle h returns integer


return h


return 0


endfunction


and


function I2H takes integer i returns handle


return i


return null


endfunction


or


if you instantly want to use them again


(maybe call triggerExecute(WhichTrigger) or ExecuteFunc("functionname"))


you can use blizzard globas as


bj_cineFadeContinueBlue


bj_forLoopBIndex


bj_lastCreatedGroup


bj_lastCreatedItem


bj_lastCreatedUnit


..


..


and more and more and more


Hope you understand

#### Modeler

#### Modeler

 Level 9

JoinedAug 27, 2004Messages471

Resources: 1

#### Resources
Maps 1

I dont get what theyse I2H H2I do. How do they pass locals?

 JASS:

function H2I takes handle h returns integer
return h
return 0
endfuncion


This gives me a bunch of errors. And again, how is it used?


same for I2H. And im guessing theres a trick for real to handle ect. But i dont get how they work. You didint really explain it

#### BertTheJasser

#### BertTheJasser

 Level 4

JoinedMar 19, 2005Messages95

With H2I you can discribe a unit, a unit grou, and all other handles as integers and these Integers can be stored in a gamecache. If you load them, just reconvert them with I2H.


(You can't use I2H directly in you triggers, you must create extra functions which return the handle EXTENSIONS:

 JASS:

function GiveMeInt_GetUnit takes integer i returns unit
return I2H(GetStoredInteger(YourCacheHere,cathegory,..))
endfunction

)

#### Modeler

#### Modeler

 Level 9

JoinedAug 27, 2004Messages471

Resources: 1

#### Resources
Maps 1

Ok - Heres a question, you didint say much about multi-instacing spells. But to multi-instance a spell, all you need to do is make every variable a local, because locals are created per use right?


But, i got to thinking, if a local is created per use, couldent you use a local trigger, and then just use globals (So long as you set the value directly before the query)? So if you really didint want to use the handler system, you could just use a local trigger with global variables right?


(I dont see why you would, but it would be nice to know you could)


Also, for local arrays, how do you give them a set array amount? Like if i wanted a real array with 7 defualt slots:

 JASS:

local real array Alpha = 7


How do i set it correctly? (Also is this a cap of the array, or just its defaulty created amount?)

 Lots of questions, ...Lots of time to .


Well thanks for the help in advanced, and thank you again bert.

#### BertTheJasser

#### BertTheJasser

 Level 4

JoinedMar 19, 2005Messages95

To set a array you must do following.

 JASS:

local integer array i //your given array
local integer n=0 //you need this var as a counter
local integer d=7 //your defaultvalue
local integer c
//how many values you want to
//adjust(beginning from 0)

loop
 exitwhen n>=c
 set [n]=d //your number
 set n=n+1
endloop


Q1: Yes, locals are created per use.


Q2: Yes, but your global must be an array, and yoou must count the array index, so you'll need another global. That's lame.(I didn't actually get what you want to know. :?: )


Q3: look at the top, there is no other way to give locals default values. Blame Blizzard for that.


Hope I could help you. :roll:

#### Modeler

#### Modeler

 Level 9

JoinedAug 27, 2004Messages471

Resources: 1

#### Resources
Maps 1

Err...


My question was that if you used a local trigger, could you make every other variable global, so you could pass them without a game cache?


And, you said every global is an array? >.>. Of course you could always make an array for one of every variable... But thats sooo confusing.


This was a dumb question anways...

#### BertTheJasser

#### BertTheJasser

 Level 4

JoinedMar 19, 2005Messages95

Q1: Yes, you can. BUT only if you want to use it IMMEDIATLY, if not, the global can be overwritten by another of your triggers(maybe you use the var multiple times).


Q2: To prvent things like that, you must make your globals arrays and you need a counter for the array index. That was what you missunderstood.


Clear?

#### Thunder_eye

#### Thunder_eye

 Level 6

JoinedMay 19, 2004Messages265

Whats wrong with this, its just a test trigger for using locals


the errors I get only contains the ones with the


local a, what to I do wrong?


Is the only problem that the local is after the first function?

 JASS:

function Trig_Untitled_Trigger_003_Actions takes nothing returns nothing
 set a = ( a + "1" )
 call DisplayTextToForce( GetPlayersAll(), a )
endfunction

//===========================================================================
function InitTrig_Untitled_Trigger_003 takes nothing returns nothing
 set gg_trg_Untitled_Trigger_003 = CreateTrigger( )
 local string a
 call TriggerRegisterTimerEventPeriodic( gg_trg_Untitled_Trigger_003, 1.00 )
 call TriggerAddAction( gg_trg_Untitled_Trigger_003, function Trig_Untitled_Trigger_003_Actions )
endfunction

#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Well, your problem is more than obvious. Locals are visible (can be used) only in the function in which you declare them, not trigger. Conclusion? Your code should look like this:

 JASS:

function Trig_Untitled_Trigger_003_Actions takes nothing returns nothing
local string a
set a = ( a + "1" )
call DisplayTextToForce( GetPlayersAll(), a )
endfunction

//===========================================================================
function InitTrig_Untitled_Trigger_003 takes nothing returns nothing
set gg_trg_Untitled_Trigger_003 = CreateTrigger( )
call TriggerRegisterTimerEventPeriodic( gg_trg_Untitled_Trigger_003, 1.00 )
call TriggerAddAction( gg_trg_Untitled_Trigger_003, function Trig_Untitled_Trigger_003_Actions )
endfunction


Oh, and be careful, you must declare locals in a function BEFORE doing anything else. For example, in your section function you did this:

 JASS:

set gg_trg_Untitled_Trigger_003 = CreateTrigger( )
local string a


INCORRECT! Setting a value to a variable is also an action. This is correct:

 JASS:

local string a
set gg_trg_Untitled_Trigger_003 = CreateTrigger( )


Hope this helped!


~Daelin

#### vicky2004

#### vicky2004

 Level 6

JoinedSep 9, 2004Messages152

Resources: 3

#### Resources
Skins 3

When u have local vars that you won't set again and u'll only use as data locals,you can set them right when u make them.So:

 JASS:

local string a
set a = ( a + "1" )

turns to:

 JASS:

local string a=a+"1" //Or a=a+I2S(1)

Not nessecery but it's good to arrange your script.

#### Morrow's Grief

M

#### Morrow's Grief

How do you determine "take" and "return" values for a function?

#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

For Blizzard functions? Use Jass Shop Pro!


~Daelin

#### TroubleShooter

#### TroubleShooter

 Level 2

JoinedApr 23, 2006Messages17

Resources: 1

#### Resources
Skins 1

well i dont know sh*t about jass i just dont get it :S


why is there


blablabla TAKES nothing blablabla RETURNS nothing


and why sometimes TAKES something and RETURNS something


and sometimes Takes nothing RETURNS something :S:S:S


and what can u do with JASS taht u cant with GUI? only local variables? actualy its 4 15 o clock ill read it agian when im less sleepy.

#### Dota-Demon[aS]

D

#### Dota-Demon[aS]

i got a language problem if anyone could help me i will be very happy lol. my problem is that i got spanish version and i dont know wich hability is locust if u could tell me in spanish or tell me one unit allreay has it to me to look for it it will be cool, ty m8s

#### Marcelo Hossomi

#### Marcelo Hossomi

 Level 7

JoinedDec 31, 2005Messages712

Resources: 3

#### Resources
Models 2Spells and Systems 1

Can I post a topic translating this tutorial to Portuguese?

#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Dota-Demon[aS]... The rawcode is 'Aloc' and I think rawcodes are the same for all the languages. You can shift+double click on the abilities field and manually add Aloc yourself.


Marcelo Hossomi... Go ahead. Though this tutorial is quite rusty and could use some improvements... Though I don't have time for that right now.


TroubleShooter... Infact, there are a lot of things that make JASS better than GUI. Here are some of the reasons (by Vexorian):


 Vexorian said:



 JASS Advantages


If you use JASS directly instead of GUI you win:

Functions: you can make your own functions and that will save you time

Efficiency: Typing is faster than clicking with the mouse

Ability to write more optimized code: GUI's automatization is not flexible enough to make well optimized code and it most of the times will generate really slow code.

Local variables and multi instancibility : GUI is simply not good to make things multi player proof.

Access to 'hidden' native functions : For some reasons GUI does not allow you to use a lot of functions like those related to memory leaks (you won't be able to fix a big quantity of memory leaks in GUI)


Click to expand...


~Daelin

 You must log in or register to reply here.


### Similar threads

 Beginning JASS Tutorial Series


wyrmlord

Feb 16, 2007

Scripting Tutorials
234

Replies181Views124K

Feb 18, 2025
Eastern Programmer yahoo

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