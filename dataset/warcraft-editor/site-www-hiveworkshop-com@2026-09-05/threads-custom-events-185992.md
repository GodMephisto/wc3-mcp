---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/custom-events.185992/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Custom Events
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/custom-events.185992/>
#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Note: The code snippets should only represent the text as a basic shape. They are not jass nor any other specific language nor the final realization.

 Exposition


 Let's suppose we wanted to create the following spell: A unit shall get buffed - not a standard buff from the object editor - but maybe a timer counts down once from 10 seconds and then, after its expiration, splash damage gets dealt around the target unit. As usual the buff shall be interrupted when the unit dies in the meantime.

scenario 1:

unit is buffed

10 seconds elapse

damage is successfully dealt and buff vanishes

scenario 2:

unit is buffed

4 seconds elapse

unit dies because of reasons, buff is forced to be cleaned up

Now, most mapmakers do it this way that they create a "A unit Dies"-Event and then ask in conditions whether the dying unit possesses the buff. If so, the buff gets removed else the code is terminated at this point.

I see two disadvantages when using this method: It's not very performant and messy to just generically run this trigger for every unit that has nothing to do with it, that did not possess the buff in the first place. Second: You would sometimes like to set a priority in which order triggers using this event are going to fire. Imagine there was a second trigger that immediately deletes the unit upon death and in case this one gets run first, you could not see anymore if the unit has the buff, hence be unable to remove the belongings like the timer. At most, the timer could check it in the ending, which is again filthy and is based on what you can read out of a destroyed unit.

That's why I take another approach: the dynamical call of code. Thereby, when the unit gets the buff, I would attach the code snippet that shall be called upon death directly to the unit. You can also deattach it again, meaning that would be some form of removing events.


 Dynamic Code


 What are the possibilities to dynamically run code?

You know it's a big flaw that you cannot have arrays of the variable type code and nor are there other ways to directly assign code values. What you can do is to use the native containers BoolExpr and TriggerAction. These get called by diverse functions:

via BoolExpr:

TriggerEvaluate, the different Enum-functions for Destructables/Items/Players/Units, the And/Or/Not-Functions and when a trigger is fired by an event, it triggers previously added BoolExprs that were defined using TriggerAddCondition, on some events you can even add a specific, personal BoolExpr

via TriggerAction:

TriggerExecute or again by one of the trigger's events

Another possibility would be to utilize the ExecuteFunc-function which rather takes the function's name as string instead of a code parameter. ExecuteFunc is slow btw.


 So what do we need?


An event object with
a type (when shall it start)

a priority (in which order in relation to other events of the same type shall it start)

a target function to trigger then

an eventHolder/subject that contains the object (in this case it shall be stored on the unit)

a source point/function that reacts on the original unit death event and starts all the events of this type

we would also like to pass information about the event like killer and dying unit in case of a unit death event,

this shall be called an eventResponse object

 JASS:

struct EventPriority
 static thistype HEADER //suggestions, you would usually like to react faster to specific cases of content stuff, so HEADER-events are usually run with lower priority
 static thistype CONTENT
 static thistype AI
endstruct

 JASS:

struct EventType
 static thistype UNIT_DEATH //of course, in reality, you would not really accumulate all types of events in a shared struct, it's only a simplified example here
endstruct

 JASS:

struct Event
 static method create(type, priority, action) //events are usually created during init and during runtime get assigned to eventHolders/subjects, which is the buffed unit in the above scenario
endstruct

 JASS:

struct EventResponse
 static integer COUNTER = 0

 boolean hasReincarnation
 unit killer
 unit triggerUnit

 function getCurrent
 return thistype(thistype.COUNTER)
 endfunction

 function destroy
 set thistype.COUNTER = thistype.COUNTER - 1
 endfunction

 function create
 set thistype.COUNTER = thistype.COUNTER + 1

 return thistype(thistype.COUNTER)
 endfunction
endstruct

This EventResponse-struct is written more explicitly to show a performant way of passing the data to the target function. See post below.

 JASS:

function UnitDies_TriggerEvents
 local EventResponse params = EventResponse.create()

 params.hasReincarnation = hasReincarnation
 params.killer = killer
 params.triggerUnit = triggerUnit

 for event in eventHolder.events do
 event.run() //run event's actions in a new thread
 next

 params.destroy()
endfunction

 JASS:

function TargetFunction
 local EventResponse params = EventResponse.GetCurrent()

 local unit killer = params.killer
 local unit triggerUnit = params.triggerUnit

 //do actions
endfunction

 Sensitive Events


Now, before finishing, I want to extend the scope. Besides simple events like "UnitDies" you know there are such as "Unit's mana becomes greater than x". Of course, I won't declare EventTypes MANA_GREATER_0, MANA_GREATER_1, MANA_GREATER_2, ..., MANA_GREATER_9999. I would rather assign the arrogated amount of mana to the Event object plus the comparison operator for best. So more parameters can add up or it can already make up an extended type. This here would maybe be called a "LimitEvent".


 EventCombination


 Only a short introduction. I might expand this paragraph at a later date.

This concept merges different Event objects to a single gateway which runs action code. You know in artificial intelligence-programming for example, you would want the mage to cast his spell when he has enough mana, cooldown ready, during combat etc. --> when several conditions are fulfilled. However, checking these in intervals via timer is not very performant/senceful, so maybe you would only check them when any related event occurs. But I think it is not that neat either to have both an event and a condition for each element or check everything everytime. An EventCombination would contrast "positive" and "negative" events that constitute a required condition (like cooldown state). A counter tracks how many of these conditions are fulfilled and only when the number hits zero, the current and further triggerings of the added events run your action code.



 Last edited: Nov 10, 2012

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Comment your code so that us dumb folk can read it.


I don't feel like spending an hour trying to figure out how this works -.-.


And yea, I'd prefer reg structs and scopes over your weird textmacro stuff on StructFollowers or w/e. I'd prefer Table (if I were to use a single hashtable) over your convoluted shared memory, and I would prefer (if I were to use a chain of events) GTrigger or Event over your convoluted hardcore custom events that I can't even read.


And that is if I were to use other people's systems. I typically code this stuff from scratch.


That's just my opinion. Trying to make sense of CustomEvents put me in a bad mood.


 Last edited: Dec 29, 2010

#### Sevion

#### Sevion

 Level 5

JoinedDec 4, 2006Messages110

Resources: 3

#### Resources
Tools 1JASS 1Tutorials 1

I'm with Nestharus on this one...

I don't feel like having 3 tabs open and using Control + F to find what the hell each textmacro does.

And what the hell is with StructFollowers?

 Code:

//! runtextmacro Folder("myFolder")

vs

 Code:

scope myScope

As far as I can understand, that's all it does...

Which would you prefer? Typing out a textmacro call or the textmacro code?

Macros are supposed be "shortcuts" of sorts.

That's more like a "longcut" if you will...

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Nestharus said:

 Comment your code so that us dumb folk can read it.

I don't feel like spending an hour trying to figure out how this works -.-.


Click to expand...

Guess I don't need to read past this line then D: You make assumptions of what's better or worse without understanding it, great. This here shows a possibility how you can do it. I have not dealt with your mentioned systems before but I permanently spot jumbled scriptings with mass abbreviations and content does not properly get separated by a generic system but instead it's brute-force-solved to mass conditions/case differentiation. That's why they need that much commentary. I write in plain/uniform text but @Sevion of course you should understand the required things first. Other systems have requirements too. You have to know vJass first before you can try to read any code written in it.

But you seem to have successfully ignored my introductory note:

 Note: The code used here is generic, the concept is of relevance. I claim, however, that I have written comprehensible enough for others to understand.


Click to expand...

I have written it using Structfollowers here because I use it that way and did not want to rewrite it in a way that pleases me less. It shows a way how you could do it if you were to use Structfollowers.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

->I have written it using Structfollowers here because I use it that way and did not want to rewrite it in a way that pleases me less. It shows a way how you could do it if you were to use Structfollowers.


A way that I'm sure we all know that only you would use is the point me and sev were making. Who else would run a textmacro over writing out the plain scope (ex)? Most people hate to write textmacros, why I've yet to meet a person who actually enjoys writing them. I know the mod of this section (azlier) especially hates writing them out ; P.


You are going to have a harder time convincing him of your 3 resources than me and azlier ; P, especially with Table, GTrigger, and Event already out there. You know how long it took me to get UnitIndexer approved? And it's actually better than the competition >.<.


Next time, try submitting something that is either better than what's already done or new + useful and people won't meet you with harshness.


Also, general textmacro code stuff is typically disliked (people really despise macros). If you look at my Stacked Fields, it has very few views (and I doubt anyone has actually used it besides me). Also look at how long the code is in the background (about the only reason it was approved was that it made a difficult and cumbersome thing to do a lot easier with a decent template and 1 macro).


Only time people accept macros is when lua is involved. Take a look at TH. Just about any system there that uses macros and is liked has to do with lua.


Also, your API is meant to be used in a very precise way, yet you fail to provide commented examples. Sure, you have examples, but none of us have any clue of what you are doing because the code isn't commented. If you look at the things where I have provided walk through examples, I will typically have 3-5 lines of comments per 1 line of code.


All I can tell is that this thing appears to be ridiculously cumbersome compared to what I'd normally use... not only does it appear to add extra overhead, but it is ridiculously difficult to use as it seems to require a nigh unreadable template that does not include any explanation as to what the various pieces of the template do.


I'm sure that I'm not alone in thinking this.


Vets will typically not use code unless they know what it's doing in the background. Before Bribe used UnitIndexer or UnitEvent, he read through all of the code to make sure he was ok with it. I know before I use any system, I read through all of the code to make sure it's up to my standards. Most coders do this. If they can't understand the code and the author refuses to cooperate, they just won't use it. And rest assured, nobody is going to spend an hour trying to figure out a small chunk of code -.-. You know how long I spent remembering how to do Stacked Fields? <2 mins because of the comments.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

 A way that I'm sure we all know that only you would use is the point me and sev were making. Who else would run a textmacro over writing out the plain scope (ex)? Most people hate to write textmacros, why I've yet to meet a person who actually enjoys writing them. I know the mod of this section (azlier) especially hates writing them out ; P.


Click to expand...

I also wonder why some people like to write in Zinc. Some systems establish, others do not. vJass in its whole is just a textmacro.

 You are going to have a harder time convincing him of your 3 resources than me and azlier ; P, especially with Table, GTrigger, and Event already out there. You know how long it took me to get UnitIndexer approved? And it's actually better than the competition >.<.


Click to expand...

I only wanted to offer something to the community, seeing that the activity lowered a bit with the launch of Starcraft2. Also, like you, I want to understand deeplier how things work and not just take everything at face value. Mapmaking in Wc3 can often be done in several ways and to advance in mastering it I think that you should spend thoughts in it and face problems of different nature. I do not expect that it gets approved, I do not think of this whole approval system highly anyway, just wanted it to be able to read here.

Maybe someone will have understood that it would not need vJass-textmacros and obfuscate the code this way if they saw that this could be done by an extension of vJass. Or they would have just have thought of an own realization when they read the basic idea.

 I know before I use any system, I read through all of the code to make sure it's up to my standards. Most coders do this. If they can't understand the code and the author refuses to cooperate, they just won't use it. And rest assured, nobody is going to spend an hour trying to figure out a small chunk of code -.-. You know how long I spent remembering how to do Stacked Fields? <2 mins because of the comments.


Click to expand...

Hm, and I have some projects with ten thousands lines of code that I immediately recall again without any comments~ That's just because I use a very uniform style, deploy additional syntax rules for myself and have basic ideas of how I would have named things or solved problems.

I do not even get what you think is so complex about this code, already striked some things off to limit it to the basics.

Anyway, I hid the code above now to focus readers on the concepts rather than the exemplary implementation.

Discussion related to Structfollowers should be discussed in its individual thread.


 Last edited: Dec 29, 2010

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

This is the code resource section. We focus on code here, not concepts. Try the tutorials section if you want to focus on concepts.


If you don't want to bother explaining your code, you shouldn't have submitted it in the first place. If all of the code you posted was a demo of your concept, then this submission has 0 code in it and is not a code resource, meaning it is a tutorial >.>. The other 2 also seem to have very simple macros in it, and given there are easier systems to use out their with easier APIs and less overhead that do just about the exact same thing >.>, I don't see why you submitted them in the first place ;P.


I know there are some resources where I provide demonstrations of what I expect people to use it for, and others where I go into full scale templates, but those resources have actual code in the background and actually do things and are actually built for a specific purpose. If you want to go into dynamic code generation, go into Lua, not textmacros. If you want to do new syntax, go into some Delphi and add stuff to vjass.


If you want to discuss concepts that use textmacros, write a tutorial.


I just think this was the wrong section to post your thingies ; P. This is probably how this major misunderstanding began = P.


->Discussion related to Structfollowers should be discussed in its individual thread.


This discussion entails all three of your submissions.


I am just wondering, am I being crazy to think that a code resource should be a resource with actual code in it and not just a tutorial?


In these resources, I just want to see the code, a simple paragraph of what the code is supposed to do as well as an API, a descriptive API, a template and a fully commented template.


The overhead of using your thing should be clear and the complexity involved should be apparent. All I see is a wall of text and incomprehensible code, which leads to frustration as I'm not going to read your wall of text nor am I going to bother trying to figure out what your code actually does.


Look at this resource as an example

https://www.hiveworkshop.com/forums/submissions-414/snippet-position-184578/


Notice you don't even have to read the introductory paragraph. You can skip to the samples to learn the entire API, the purpose, and complexity. The introduction talks about the overhead (boxed, meaning if statements). The code can be clearly found and the extensions below it are all placed into a quote box. It also includes a descriptive API.


Please, I hope you can begin to understand my frustrations at you ; P.


This is another form-

https://www.hiveworkshop.com/forums/jass-functions-413/isunitmoving-178341/


The functions are named so that it is apparent as to what they do. The purpose is clear, the code is clear, and each function has a description. The code is also easy to read.


String Parser goes so far as to create a 7 chapter tutorial on how to use it, a list API (no descriptive as reading the tutorial gives description), and a slew of well commented demonstrations with outputs.


I don't think there's anything more to be said regarding this matter ; P


 Last edited: Dec 29, 2010

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Where have you read that this is "Code resources"?


The forum titles mention The Hive Workshop > Warcraft III Resources > Submissions, which implies that all War3 Resources fit here


I did read the subtitle of Submissions "Submit JASS resources! If approved, they will be moved to their proper section.", however, the rules state "systems to aid with coding fit here.". That is pretty dead on.


You are right, I did already use this elsewhere out of hiveworkshop for a tutorial.


Thanks, will ask a moderator then to move it (and maybe have the thread cleaned).

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Opening.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Would like to present a more performant approach of passing data from event base to target functions.


Above, I had illustrated it this way:

 JASS:

for event in storedEvents do
 globalEventParam1 = localEventParam1
 globalEventParam2 = localEventParam2
 globalEventParam3 = localEventParam3
 ...

 event.run()
next


meaning that the eventResponses are updated before each individual event. This was done in order to prevent that the global variables would be overwritten from recursions or whatever the specific events fire.


But I think there is a better way.


Since long ago, there was the idea to have it like Blizzard --> bind the data to threads/trigger calls. Such calls cannot simply be indexed because the engine itself provides no unique id, which is why you have to use local variables and those are only bound to the instance of the function.


Theoretically, when an event shall be run, you could copy the trigger it bases on, so this trigger is a unique id given to and it cannot fire twice simultaneously to make it distinct. The array of triggers per event might be recyclable, still it's probably very slow and you have to release the data in the end.


So now a strategy that should be more performant in most cases:


The setting of event responses is only done once before the execution loop. They are stored in arrays to allow overlapping. Now every occurence of an eventType requires a free id you can attach data to --> eventReponse object.


Only this object reference is passed to the target function.


Since the application works this way that nested calls of functions or even executions are completed first before returning to the parent, this means it would be enough to just save the last eventResponse the direct parent possesses and restore this value before returning to the parent.

 JASS:

function TriggerEvents
 local EventResponse params = EventResponse.CURRENT

 //run events

 set EventResponse.CURRENT = params
endfunction


Of course, this could have been done with the single response values as well but it's more effective to have it unified in an object. You only need to pass that event then. Because of this nesting principle it's even enough to have a global counter for the allocation of the ids. You just increment it by 1 everytime at the start and the previous value is obviously current value - 1. This way, it does not require a local variable at all.

 JASS:

struct EventResponse
 static integer COUNTER = 0

 boolean hasReincarnation
 unit killer
 unit triggerUnit

 function getCurrent
 return thistype(thistype.COUNTER)
 endfunction

 function destroy
 set thistype.COUNTER = thistype.COUNTER - 1
 endfunction

 function create
 set thistype.COUNTER = thistype.COUNTER + 1

 return thistype(thistype.COUNTER)
 endfunction
endstruct

function UnitDies_TriggerEvents
 local EventResponse params = EventResponse.create()

 params.hasReincarnation = hasReincarnation
 params.killer = killer
 params.triggerUnit = triggerUnit

 for event in events do
 event.run()
 next

 params.destroy()
endfunction

function onUnitDeath
 local EventResponse params = EventResponse.getCurrent()
endfunction


Since the target functions run in own threads, there will be no conflicts with the counter. Still you have to request the eventResponse object before any Waits (who uses Waits anyway?), else you might get wrong values.


Further disadvantages: Arrays in jass only provide space up to around index 8k. That's why you should not stack the code tremendeously since we do not want hashtables steal our performance. The reads of the responses in the target function are now array reads + local var reads. That may be marginally slower than before. On the other hand, the more expensive setting of the different responses and the reads they needed have been omitted. It might be wise to check if there are any events at all to account for such cases even better.

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

And this is the most performant way

 JASS:

globals
 integer eventData = 0
endglobals

function Run takes integer data returns nothing
 local integer prev = eventData
 set eventData = data

 //run event

 set eventData = prev
endfunction

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Is not the easiest and most logical way just to check is unit alive before dealing damage (for example).

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

-Kobas- said:

 Is not the easiest and most logical way just to check is unit alive before dealing damage (for example).


Click to expand...


In which regard? To know if the unit dies during damage events and shall therefore not run further events? I have barely touched that topic, but you may as well revive after having died and before the events were handled.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

So idea is next:

- Unit cast spell

- wait 10 seconds

- blow some area

but what happens

while wait unit dies so this blow goes random location or makes no sense and so on...

now something like this fix it:

- Unit cast spell

- wait 10 seconds

- (if Unit alive) blow some area

again this fail if unit die and get revived before blow event right?

Well in this case we can have some more checking.

we create global function that do next:

- unit dies

- set death_counter [ unit id ] = death_counter [ unit id ] + 1

(note: this is example, can be done with arrays, table etc, depends on what we need right)

and here we go our spell

- Unit cast spell (+ local integer fake_death_counter = death_counter [ unit id ] )

- wait 10 seconds

- (if Unit alive and fake_death_counter == death_counter [ unit id ]) blow some area

Problem solved eh?

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

That is one solution to ensure the unit is in the same period of state. It can be used to avoid having whole objects.

For example, when you have an amount of stun spells and a breakStun functionality. To know whether the unit is stunned, every stun effect increments a shared counter by 1. Of course, when they expire, they have to decrement the counter again but breaking the stun would reset the counter to 0 immediately. Directly dissolving all the different stun effects would require a dynamical approach --> listing objects. Instead, you can give a token based on the calls of breakStun to each new stun effect, save it and decrement the value in the end only if the token is the same.

If you do it this way, you have to preserve the token holder, the unit in this case until all references have vanished. Else the next unit/object could be doomed by non-expired data or grant it a new token.

Reasonably, the pool of tokens should not overflow.

The main reason I would rather directly dissolve stuff is because I do not want having unnecessary things running in the background. Also, the alive check in your case needs to be evaluated everytime. Imagine this would not be a one-shot timer but fire frequently and it would multiply with more cancel conditions. Furthermore, the difference in flexibility is obvious. Finally, the spell could alternatively have to act actively on the unit's death, triggering the damage effect immediately for example. Checking it via a timer which would not be required otherwise would be pure overkill, inaccurate etc.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

When I write a system that has possible effects on any other different code (or even multiple opnes,

which is mandatory for this concept to exist) then the best thing might be always to handle everything

in the execution function of the instance when it expires/or any other event.

When a unit has a buff.

Why would I create 2 seperated OnDeathEvent triggers that deal with the possibilits of the unit having that buff?

I would try to handle everything inside one death trigger, so it is structered the best.

I mean the user anyways manuly needs to care about event types and the appropriated priorities,

so maybe the better step was instantly to fix the possible malfunctions by structering the different affected code parts (or function calls) inside one determinition function.

The idea of prioritising is sure good or even essential in cases, but I'm not sure I need a structure like shown in examples to achieve it.

I know it's a very old thread, but I thought I start discussing it again since it's still under submissions.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

If I understand you right, you want to statically link the handler functions to the event dispatcher by rallying up the lines there?


like

 JASS:

//module A
public function onDeath takes nothing returns nothing
endfunction

//module B
public function onDeath takes nothing returns nothing
endfunction

//event dispatcher
private function dispatch takes nothing returns nothing
 call moduleA.onDeath()
 call moduleB.onDeath()
endfunction

private function init takes nothing returns nothing
 local trigger t = CreateTrigger()

 call TriggerAddAction(t, function dispatch)
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_DEATH)
endfunction


This way, everytime you want to register or change an event handler, you have to jump between files. It easily becomes a mess, the example above is a simple one, you may know from GUI systems, for instance, that there may be something like event consumption or other propagating stuff. That's quick to bloat your code and introduces redundancy when not done dynamically.


The static approach requires your handler functions to be public for the world to see and increases coupling, your central dispatcher becomes dependent on the module that uses it, when the module cannot compile, nor can the system. You have to look out for cyclic dependencies because if it's not a native event like above but a custom one, there may be no dynamic call in between.


It's highly inefficient in case there are lots of handlers when their individual trigger probability is minor. The complexity grows with each entry. You should create a new thread anyway since you want to avoid the OP limit and have the contexts not affect each other. A thread break would kill everything otherwise.


The "onAnyUnitDeath" event type is mundane and generic. What if you want to track that a specified attribute of a specific unit changes value where both are not known at compile time? Shifting the parameters to the dispatcher would rob explanatory description from the module and install additional data transfers, keeping it in the module would conceal the parameters from the system unless you again install channels.


Lastly, the invoker may not even be your own resource, in which case it is not to be altered by you.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

More or less, yes, you understood and explained it right, what I said.


Also I basicly see where you're comning from and your argumentation is reasoned.


For example having a bit more modularity is good


But also:


Why is hitting OP limit a problem?


I might use trigger evaluations e.g. for new thread creation.

 The static approach requires your handler functions to be public


Click to expand...

I'm not sure I correctly understand.


Your approach doesn't?


It would highly depend on what you are exactly doing to become a code bloat or not.


In shown examples we simply could give the unit as parameter and all would be okay.


But if each binded module needs seperated global initializations or specific handles, then it becomes more messy.


But as clean setup, even it's as said not often needed, your way seems better.


And yes, the invoker is an external one, but that is actually what it is about.


My say was that it just can be exclusivly an internal one(s).


------


I see your method as an other, cleaner alternative for certain approaches, but not necessary as a one for general usage.


In comparison it lacks some simplicity in my eyes, but instead might be useful for complex codes and systems if they are binded to each other.


It has definitly some right to exist I think. But if I was you I would write a bit more detailed demonstration,


which shows in the end a better example that might be used in reality. (without too much content, but the structure must be clear)


For this example, it seems like a bit much hassle honestly.


(Just to clarify, in case you misunderstand. This is not from a moderator for approval, but just user critique)

#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

 Let's suppose we wanted to create the following spell: A unit shall get buffed - not a standard buff from the object editor - but maybe a timer counts down once from 10 seconds and then, after its expiration, splash damage gets dealt around the target unit. As usual the buff shall be interrupted when the unit dies in the meantime.


scenario 1:


unit is buffed


10 seconds elapse


damage is successfully dealt and buff vanishes


scenario 2:


unit is buffed


4 seconds elapse


unit dies because of reasons, buff is forced to be cleaned up


Now, most mapmakers do it this way that they create a "A unit Dies"-Event and then ask in conditions whether the dying unit possesses the buff. If so, the buff gets removed else the code is terminated at this point.


Click to expand...


They attach the spell struct to the timer, and after it expires check to see if the unit is present (not removed) and alive and then do the damage, no?


Not sure how you went from this example to a tangent about custom events.


Speaking about "custom events", a recent example would be Flux's Quads, and they do require a timer/periodic checks. The other way of triggering "custom events" would be from the triggering of native events (spell casts, etc.).


A silly example might be a unit casts a healing spell, check if the time of day is 06:00 (fgamestate GAME_STATE_TIME_OF_DAY) if so fire the "EVENT_MORNING_HEAL".

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Aniki, there is some information about the first suggestion in thread already.

And the meaning of threads's purpose might be a bit misinterpreted by you, I guess.


 Last edited: Feb 24, 2016

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Very simple, written in 1 min so there may be some spelling flaws.

Of course it must be done much more professional than posted here by me.

 JASS:

struct TriggerList
 static method operator [] takes handle h returns thistype
 if table.has(GetHandleId(h) then
 return table[GetHandleId(h)]
 endif
 return create(h)// Create and reference to the event.
 endmethod
 // List

endstruct

function RegisterDeathEvent takes integer priority, code func returns nothing
 // add at the proper position of the list.
 // Must loop through all added triggers and their priorities.
 // Then insert this one at the proper position.
 call TriggerList[EVENT_PLAYER_UNIT_DEATH].add(priority, Condition(func))
endfunction

private function dispatch takes nothing returns nothing
 local TriggerList node = TriggerList[EVENT_PLAYER_UNIT_DEATH].listGetFirstNode(GetTriggeringTrigger())
 loop
 exitwhen node == 0
 call TriggerEvaluate(node.triggerHandle)
 set node = node.next
 endloop
endfunction

private function Init takes nothing returns nothing
 local trigger t = CreateTrigger()

 call TriggerAddAction(t, function dispatch)
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_DEATH)
endfunction

Actually what I posted above is only important if the game hasn't started yet.

You can use that one priority event

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

IcemanBo said:

 Why is hitting OP limit a problem?

I might use trigger evaluations e.g. for new thread creation.

I'm not sure I correctly understand.

Your approach doesn't?


Click to expand...

As in the example above, it does not. But I just meant that you cannot phrase it as a viable advantage.

IcemanBo said:

 In comparison it lacks some simplicity in my eyes, but instead might be useful for complex codes and systems if they are binded to each other.


Click to expand...

I use it like everywhere in my map, especially for additive content like spells, items. Since you do not need to touch the other files (however you are required to know the API, hurray for auto completion), development is fast and convenient. I am not quite happy with the priority setup either but found order necessary in some places.

Aniki said:

 They attach the spell struct to the timer, and after it expires check to see if the unit is present (not removed) and alive and then do the damage, no?

Not sure how you went from this example to a tangent about custom events.


Click to expand...

That solution is improper, the unit could have died in the meantime and revived again -> clean up immediately or at least mark the unit/spell instance so it can be distinguished.

This reminds me of another problem I had in the past. I wanted an effect that would be able to break existing stuns on the unit. I had a simple count method in place before. Whenever a stun effect was applied, it increased by 1 and was decremented by 1 when wearing off. So the unit was stunned with the counter > 0. How to introduce stun break now? If I simply reset the counter to 0, all the spells and features that applied their stun would not be aware of it, trying to lift it later on, messing up the counter. If the counter value is kept but the stun revoked, it won't work either, one stun added and removed with the counter still > 0, unit still paralyzed. Lifting the stun and setting the new threshold to the current value sounds better but this fails again in case a previously applied stun ends before a new one. You have to somehow differ stuns from before the break from new ones - or kill all references immediately.

#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

 That solution is improper, the unit could have died in the meantime and revived again -> clean up immediately or at least mark the unit/spell instance so it can be distinguished.


Click to expand...

Yeah okay I see, the unit died and lost the buff but got revived... then maybe store the "revived" count of the unit

into the spell struct and check not only for "alivness" but also if the spell struct's revive count matches the current revive count of the unit, if they don't then do nothing (the unit died, lost the buff and got revied), like you said.

 And the meaning of threads's purpose might be a bit misinterpreted by you, I guess.


Click to expand...

I guess so, perhaps WaterKnight could elaborate further if he wants.

 JASS:

 // add at the proper position of the list.
 // Must loop through all added triggers and their priorities.
 // Then insert this one at the proper position.

Click to expand...

The Binary Heap article says they are used to implement priority queues, so they are probably more efficient.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Well, the thread was never about presenting a specific solution but raising awareness for the subscription pattern because, at least at that time, I rarely saw custom events at all and rather such that cancellation conditions were checked for in timer functions or they used generic native events that triggered everytime and filtered for relevance.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Submission:Custom Events

Date:22 November 2016

Status:Approved

Note:

Even maybe it won't be used by too much people, it's for sure an interesting concept to read and discuss.

Due the in-thread discussion with WaterKnight I think it should be public. Approved

Even it's not specified to JASS, it would probably fit there the best.

 You must log in or register to reply here.


### Similar threads

Locked

[JASS]
 vJass struct template


Deaod

Apr 14, 2009

Triggers & Scripts

Replies11Views3K

Apr 16, 2009
Viikuna

 Structs for Dummies


Magtheridon96

Jun 15, 2011

JASS Tutorials

Replies19Views26K

May 3, 2016
Zwiebelchen

Locked

[vJASS]
 Event (library)


Starquizer

Jan 5, 2013

Triggers & Scripts

Replies25Views5K

Jan 6, 2013
Starquizer

Locked

[vJASS]
 Group clearing unexpectedly


Titanhex

Apr 7, 2012

Triggers & Scripts

Replies4Views1K

Apr 8, 2012
Titanhex

Locked

[vJASS]
 Fucked up my double linked index help!


Pinzu

Aug 29, 2015

Triggers & Scripts

Replies1Views1K

Aug 29, 2015
IcemanBo

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

JASS Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top