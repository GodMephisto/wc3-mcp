---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/advanced-jass-tips.22710/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Advanced JASS Tips
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/advanced-jass-tips.22710/>
#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Advanced JASS Tips


This “tutorial” is meant for those people who already know how to make JASS codes and got the hang of them. It should help you improve your code, and make it more advanced and efficient than it is. It contains a couple of tips, and it’s not really a tutorial.


1. Use Native Functions


For those who do not know, native functions are fast, and do not cause lag! You should use native functions and avoid the others whenever possible! Now, the problem comes, how you know which functions are native, and which aren’t. Well, firstly, all BJ functions are non-native. Try to avoid them.


However, the rest are pretty complicated. So how do we make difference? And even more important, how do we know what natives does the non-native use? The solution comes with an editor: Jass Shop Pro.


Let’s analyze the tool a bit. In the right side, there is a search function. That’s all we need. Search for the function you are not sure if it’s a native or not. As an example, I will take ShowUnitHide. Type this in the search bar and only this function should appear in the list. Click on it! On the bottom window, the function will appear. It is very important that the function’s name appears in italic. This means that your function is not a native! Moreover, for non-native functions you will see their code as well. Natives do not have a JASS “writeable” code and so, the program will only display the parameters and the value they return.


In this case, you will notice that the function simply calls the ShowUnit() function, for the parameter of the function, and the value false. You could do that manually, couldn’t you? It is much faster, and even though the code may not seem simpler, in time you will realize that it’s better this way.


Another way to detect the non-native functions in your code is by copy-pasting it in the program. You will notice that keywords are highlighted. Well, all function names (made by Blizzard) appear in green. Non-natives appear in Italic. Conclusion? You can replace those with natives, if the coding is not too complicated (and in most of the cases, it isn’t). You should be able to make difference between the problem-causing non-natives, and non-problem-causing non-natives.


And now, a last question will surely pop into your mind: “Why were natives created by Blizzard, if all they do is slow the game”? Hehe, it’s not really like this. When Blizzard created JASS, nobody knew the language. But then, people started learning it, and soon enough, Blizzard released it to the public. These non-natives are supposed to make things easier for the users, and that is why, most GUI functions are non-natives. I'll give you an example:

 JASS:

 function UnitApplyTimedLifeBJ takes real duration, integer buffId, unit whichUnit returns nothing
 call UnitApplyTimedLife(whichUnit, buffId, duration)
endfunction


In this case, some people may find it easier to have first the duration, then the buffId and then the unit. I personally do not get the purpose of this BJ, but it might have one. On the other hand:

 JASS:

 function UnitCanSleepBJ takes unit whichUnit returns boolean
 return UnitCanSleep(whichUnit)
endfunction


What’s the purpose of this function anyway? It simply calls another function, and so, slows things up a little! That is why I really suggest that you avoid natives whenever you don’t find it extremely difficult. Just replacing things a little doesn’t hurt.


2. Use Coordinates instead of Locations


In a lot of cases, coordinates are faster than locations. Even though Locations are supposed to be faster, the fact that you must remove them makes things slower. But first, let’s see the differences.


Locations automatically store an X and Y value inside them, and represent a point on the map. They are much easier to work with, because you do not need two variables, but instead only one. On the other hand, they are handlers and so, must be removed once you no longer need them.


A coordinate, either X or Y is actually a real value. Coordinates as type of variable do not exist, they exist only in theory. The main disadvantage is the fact that you no longer with only one variable, but instead with two. However, they do not need to be removed or destroyed since they are not handlers and do not take much memory either.


There is also a way to transform coordinates into locations and vice-versa.

 JASS:

Coordinates into Location
local location l
local real x = 0.00
local real y = 0.00
local location l = Location(x,y)


The native function Location takes two real values (coordinates) and returns the equivalent location. Pretty simple! In the code above, I took the coordinates of the central point of the map, and stored it as a location into the variable l.

 JASS:

Location into Coordinates
local location l = GetRectCenter(GetPlayableMapRect())
local real x = GetLocationX(l)
local real y = GetLocationY(l)


The native functions GetLocationX and GetLocationY take a location and return the required coordinate (X in the first case, and Y in the second). In this case, I got into l the center of the map, and then by using the other native functions I got it’s X and Y coordinate.


What I wrote above is pure theoretical and are things I rarely use anymore. However, it should give you a general view upon the difference between Coordinates and Locations. Now, let’s see how can we replace locations with coordinates, by taking some special cases.


a) PolarProjection – Yes, it’s true, PolarProjection is an extremely useful function, but getting the X and Y value of the location it returns… You still have to remove the location and so, you gain absolutely nothing. Even worse, you have to call two useless functions, and you carry two coordinates instead of carrying a single location. So, how can we solve the problem?


Those who studied common.j, will notice that PolarProjection is not a native. I will post it here, so that everybody can see how it looks:

 JASS:

 function PolarProjectionBJ takes location source, real dist, real angle returns location
 local real x = GetLocationX(source) + dist * Cos(angle * bj_DEGTORAD)
 local real y = GetLocationY(source) + dist * Sin(angle * bj_DEGTORAD)
 return Location(x, y)
endfunction


The solution now is pretty simple. We have to create our own PolarProjectionX and PolarProjectionY functions, to get the X and Y coordinates of the point. The functions would look like this:

 JASS:

 function PolarProjectionX takes real x, real dist, real angle returns real
 return x+dist*Cos(angle*bj_DEGTORAD)
 endfunction

function PolarProjectionY takes real y, real dist, real angle returns real
 return y+dist*Sin(angle*bj_DEGTORAD)
 endfunction


Now, to use them, simply call the right function for the X, respectively the Y coordinate. As parameters, use the same dist and angle, and as for the source, use the x, respectively the y coordinate of the point. That should solve your PolarProjection problem.


b) Using coordinates for functions that ask as a parameter location


This is an extremely frequent problem that comes when replacing locations with coordinates. You have used functions that require as parameters locations a lot already, and it’s obvious to know them. However, the problem comes when you want to use coordinates. Using as a parameter Location(x,y) is useless as I said before, because you automatically use a location, which needs to be cleared, and all the coordinates work was in vain. However, Blizzard, smart as they were, created both Locations and Coordinates native functions, or maybe even better, location functions based on coordinates natives.


I’ll give you an example.

 JASS:

function AddLightningLoc takes string codeName, location where1, location where2 returns lightning
 set bj_lastCreatedLightning = AddLightningEx(codeName, true, GetLocationX(where1),
 GetLocationY(where1), GetLocationZ(where1), GetLocationX(where2),
 GetLocationY(where2), GetLocationZ(where2))
 return bj_lastCreatedLightning
endfunction


I think it’s pretty obvious that this function uses a location native. All you have to do is search for the correct function. That’s what Jass Shop Pro is good at!


There are two additional functions I am willing to give you here, just because they do not exist for coordinates, and because they are very important. I’m writing them because some of you may not figure it out how to make them. One of them is calculating distance between two points, and another one angle between two points.

 JASS:

function GetDistanceBetweenPoints takes real x1, real y1, real x2, real y2 returns real
 return SquareRoot((x2-x1)*(x2-x1) + (y2-y1)*(y2-y1))
 endfunction

 function GetAngleBetweenPoints takes real x1, real y1, real x2, real y2 returns real
 return bj_RADTODEG * Atan2(y2-y1, x2-x1)
 endfunction


3. Functions to avoid


There are certain functions I would suggest you avoid. There were people who tested these functions (such as Vexorian, thanks Vex) and came to the conclusion that the alternative is faster.


a) Function Pow(x,2) vs x*x


I think I mentioned that natives are usually fast and do not cause lag. Well, in some cases avoiding even the native can prove to be faster. This is a case! Calling a function makes the computer jump somewhere else in the memory, transmitting the parameters and all the stuff, and so, in this case, just using x*x instead of the equivalent Pow() function is faster. I am not saying that functions do not have their advantages! Of course they do! You do not have to write a piece of code 10 times, it is much harder to make mistakes with functions. But in some few cases, when you can avoid them, it is suggested that you do so.


b) GetTriggerUnit() vs their equivalents


Once again, it has been proven that GetTriggerUnit() is faster than any other equivalent native. Do not ask me why, I do not know! But it has been proved! Equivalents are usually units who execute the function, such as GetDyingUnit() (responding to An Unit Dies) and GetSpellAbilityUnit() (responds to An Unit Begins casting an ability). GetTriggerUnit() remains even unaffected by waits, so it is much safer, and it seems even faster.


4. Constant Functions


Other JASS subtleties are the constant functions. By adding the prefix constant, in front of the keyword function, you make a function constant. What does this mean? That the function returns a constant value, and that it’s value does not vary. For example, GetTriggerUnit() is a constant function, because it always returns the unit which triggered the actions.


The advantage is that constant functions are slightly faster. Sometimes it is significant, sometimes it is not. I suggest you make rawcode functions constant, because those truly return a constant value. Example:

 JASS:

constant function Dummy_Raw takes nothing returns integer
return ‘h000’
endfunction

Note: Functions calling other functions that are not constant cannot be made constant themselves. Also, constant functions do not modify the value of the parameters.


5. Kattana’s Local Handle Vars


Some of you might’ve heard about them, some might not. I think a lot of people use them right now, and they proved to give an extreme improvement to JASS. Why? Because through them you are able to access a local variable through another local, and so you can avoid global variables even more than ever. This is an extremely vast subject, but I will try to keep it short.


First of all, get the system from here. You will need to copy-paste it into the header of your map, so that you can use it!


Now, let’s study the whole linking concept. By linking, we understand that you can access a handle or value through another handle (but not value). Example of handles include units, timers and triggers. There are the mostly used handles for linking, and you will soon see why.


Let’s take a classical example in which this system proves extremely useful: smoothly moving units. You probably all know that the minimum value for TriggerSleepAction is 0.27. Quite high value I might add! If you try to move units and simulate some kind of gliding you will fail. Moving the unit every 0.27 seconds is simply not enough. So how can we solve this problem? By using timers.


Timers are handles that are like a chronometer. They wait a limited number of seconds, and after that, they force the execution of a function. You have the option to even make a timer repeat, by resetting its value and again countdown to 0, until you destroy it. Timers value can be brought down to a very small value. But I think 0.035 is smooth enough. Do not give a too small value, because the game will lag, because of the high number of executions of the function.


So, the advantage of timers is the fact that they can simulate a very short wait. But another advantage is the fact that waits interrupt the execution of the following actions, thing unwanted maybe by some. So in this case, a timer could once again prove useful.


Now, let’s take the knockback example, and see how you can use the Handle Vars system. I won’t write the entire code, but only the part you need to understand.

 JASS:

function Knockback takes nothing returns nothing
local timer t = GetExpiredTimer()
local unit caster = GetHandleUnit(t, “caster”)
local unit target = GetHandleUnit(t, “target”)
local real x1 = GetUnitX(caster)
local real y1 = GetUnitY(caster)
local real x2 = GetUnitX(target)
local real y2 = GetUnitY(target)
local real angle = GetAngleBetweenPoints(x1,y1,x2,y2)
local real dist = GetDistanceBetweenPoints(x1,y1,x2,y2)
call SetUnitPosition(target, PolarProjectionX(x2, dist+10.00, angle), PolarProjectionY(y2, dist+10.00, angle))

set t = null
set caster = null
set target = null
endfunction

function Execute_Knockback takes nothing returns nothing
local timer t = CreateTimer()
local unit caster = GetTriggerUnit()
local unit target = GetSpellTargetUnit()
call SetHandleHandle(t, “caster”, caster)
call SetHandleHandle(t, “target”, target)
call TimerStart(t, 0.035, true, function Knockback)
call TriggerSleepAction(0.5)
call FlushHandleLocals(t)
call DestroyTimer(t)
set t = null
set caster = null
set target = null
endfunction


Ok, it may look complicated but it will soon clear up. Execute_Knockback is the function which starts when the caster casts the knockbacking spell at a target. Now, we create the timer, and get the caster and target into two separate variables. Now comes the complicate part: the linking.


I told you before that by linking, we assure that we can access a value or handle, through another handle. Well, the process of linking has three steps: Selecting the handle to which you want to link the value or the other handle. In this case, the linker is the timer. You will soon understand why.


The second step is choosing a label under which to link the variable. If you link more values/handles to something, how can you then make difference when accessing them? They must have different labels, which are strings. Make sure they are always different, or else you may mess things up.


Third and last step is simple, because you must choose the value or handle, which you want to link. In this case, we linked the caster and the target. Once again, be patient and you will see why.


Now, we start the timer. It has a 0.035 duration, it repeats, and after each expiraion it executes the function Knockback. And now comes the first problem. When executing a function and not calling it like you normally do, you cannot use parameters. So how can you use the caster and target? Using globals is not a solution because it ruins your multiinstanceability.


THAT is why we use linkage. Let’s go to the Knockback function and see what happens. Since the function responds to the expiration of a timer, we can access that timer through a native: GetExpiredTimer(). Well now, I told you that linkage means being able to access a value or handle through another handle. Since we have that another handle (the timer), we can now access our two units, the caster and the target.


How do we do that? Simple, by using the appropriate function. In this case, we know that we have to get an unit, so we use the GetHandleUnit function. The first parameter is the handle to which you linked the unit, and the second one is the label. You know the label, since you chose it! So by using the label and the timer, voila, you got the unit!


That’s mainly what you have to know about linkage! If you didn’t understand it, I suggest you study the process in detail again and again. You will get it in the end; it’s not that complicated. Do not read further until you get how it works!!!


Now, the last problem we will get is if we no longer need the value/handle linked. Linkage consumes memory, and not unlinking it will be considered leak. In the case above, if I would just destroy the timer and nullify it, the unit will remain linked to something in the memory, but with no purpose or effect. Conclusion, we have to break the link!


There are two ways to do it: Either by breaking the links one by one, or by breaking all the links done by a handle. Usually use the first variant when you link things to units, destructables, items, or other things you do not use in a single execution. For example, casting three simultaneous spells which link things to an unit, and breaking all the links for that unit in one of the spells will bug the other two, because all the links will disappear. However, in the case of a local timer or trigger, which is used only in that single execution and then destroyed is much faster!


a) Breaking all links


Extremely simple! Simply use the FlushHandleLocals() function, and give as a parameter the handle to which you want to break all the links.


b) Breaking links one by one


Simple as well. Take the links one by one, and for their label, link to the handle the value null (in the case of the handles). I’ll give you an example of such unlinking:

 JASS:

local timer t = CreateTimer()
local unit u = GetTriggerUnit()
local unit v = GetSpellTargetUnit()
call SetHandleHandle(t,”cast”,u)
call SetHandleHandle(t,“targ”,v)
//And now comes the unlinking
call SetHandleHandle(t, “cast”, null)
call SetHandleHandle(t, “targ”, null)


Consider this equivalent to nullifying local variables.


Note: The linking system uses a game cache. Blizzard put a limit of 255 elements per cache, so you can link only 255 elements at the same time, unless you use a separate cache for each different spell. In that case, you will have to modify Kattana’s system a little. I am not going to present that here, just telling you that it exists.


I hope these tips have proven useful! If you have any other questions or improvements, tell me and I will do my best to make this tutorial better!


~Daelin


 Last edited by a moderator: Mar 9, 2007

 You must log in or register to reply here.


### Similar threads

 Jass, or "What the func?"


GhostWolf

Jul 21, 2013

Scripting Tutorials

Replies9Views9K

Oct 1, 2013
deathismyfriend

 Basic JASS Tips


Daelin

Jan 8, 2006

Scripting Tutorials

Replies1Views15K

Apr 10, 2006
PurplePoot

 Coding an efficient knockback (vJass)


Silvenon

Oct 13, 2007

Scripting Tutorials

Replies39Views21K

Sep 6, 2008
Herman

 JASS Tutorial


Daelin

Mar 20, 2005

Scripting Tutorials

Replies24Views38K

Nov 4, 2006
Daelin

 JASS: A Better Understanding


Ghan_04

May 31, 2008

Scripting Tutorials

Replies15Views10K

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