---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/interactive-environments.187317/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Interactive Environments
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/interactive-environments.187317/>
#### Cobbot

#### Cobbot

 Level 5

JoinedMar 15, 2008Messages35

Resources: 5

#### Resources
Maps 1Tutorials 4

Interactive Environments


Difficulty - Knowledge of GUI and Variables

Info


Well I came across this idea whilst bored to death, so I decided to give it a try. And I'll be honest, I was shocked with how nicely it came out.


Basicly what I mean by Interactive Objects is giving the player the ability to interact with his/her environment. As oppose to just having regions that trigger the events.


My system is an ability called "Interact" which is used when a prompt appears on the screen, Via Floating Text. When used in the correct areas the ability can be used to trigger anything from cinematics to traps and doors.


Just so you don't get lost in abreviations heres my list;


Sequence - 3 Triggers; Activate, De-Activate and the Main Trigger


Main Trigger - The trigger in which you trigger the event, for example the door opening or a cinematic.


I 00x - I stands for Interact and I give each one a number, eg. I 001 is the first Variable/Region used for trigger I 001 A/B/C


 Thats all for now


Right lets get to work, don't worry this won't take long.

Stage 1: Setting the Stage


If you already have a map with your objects and stuff already in it then there is no reason to do this. This is just so you can follow what I do.


So start by creating a brand new map, I'm using dungeon as my setting as is seems the best place to use some doors and things.


Now just create a small room and place a character of your choice inside it. Knock out a wall and place a gate of your choice in the gap.


Thats the first room done. Now I'm creating a bigger room with another door and a lever.


You may of course want to make your doors invulnerable or the player can just break them...


Unsurprisingly we're done here. Simple.

Stage 2: Variables


The second stage is to set up our variables we will be using.


So open up your variables window. Now we don't need to plan ahead too much right now so just create 5 Boolean Variables, And make them the following


I_A - I_A Stands for Interact_Activate, this is for the floating text.


I_001 - I_001 is Interact 001


I_001_C - I_001_C Is Interact 001 Activate. This is vital in activating and de-activating the trigger when the unit enter/leaves the region. This is also used to limit when the player can use cirtain objects. I will explain this in more depth later.


I_002 - I_002 is Interact 002


I_002_C - I_002_C Is Interact 002 Activate. Also vital, same reason as I_001_C.


So now that we've done those we have to create a Floating Text Variable


I_Prompt - Clues in the name, this is Interact Prompt. The floating text that tells the player when he/she can interact with something.


Thats all for this part lets move onto the regions.

Stage 3: Regions


This stage is going to vary depending on your setup but its the same general business.


Now we aren't going to need many regions, at least not for my map anyway.


1 Region per trigger should do fine.


Create a region around the gate, going over it a moderate amount. Giving the unit enough room to fit in so the prompt apears in a reasonable distance.


Name it I 001 A.


If by any chance your Sequence requires more than one region, create your new region with a reasonable area for the prompt to appear, and name it I 001 B.


Now create a region around the Lever. Going over a moderate amount with enough room for the prompt to appear and the player to react.


Name it I 002 A.


If you need 2 regions for your Sequence make it with the reasonable area previously mentioned and name it I 002 B.


Thats all for regions. Now we've got to do the ability and then we can move onto the triggers.

Stage 4: The Ability to Interact


This should be a pretty short stage as the ones before.


Bring open the Object Manager. Go to the Abilities tab. Create a new ability using the Bladestorm ability as a template. Name it Interact

I say use Bladestorm because not many units have an animation for it an please don't comment saying use a different ability because I'm not too bothered. They do the same thing in the end.


Remove the, Area of Effect, Damage per Second, Buff, Mana Cost and Duration. UNTICK the Hero Ability Tickbox, Set the cooldown to 1.00 and Change the hotkey to whatever you want. I reccomend "E" but thats because I've played lots of FPS Games.


Now go to the Units tab. Locate the unit you are using, or create a new one, and give it the Interact ability that we created.


I said this would be short. We're done here. Now we can move on to the fun bit. Triggers!

Stage 5: Triggers


Here is the longest part of the tutorial. But not too long


Each "Sequence" is 3 triggers, we will be creating 2.

I 001 A


This first trigger is to activate the prompt for Interactive Object 001. Here it is

I 001 A
Events
Unit - A unit enters I 001 A <gen>

Conditions
(I_001_C Equal to False) and ((Triggering unit) Equal to Demo-Man 0000 <gen>)

Actions
Set I_001 = True

Set I_A = 1.00

I 001 B


This second trigger de-activates the prompt for Interactive Object 001, as a result of the player leaving the area.

I 001 B
Events
Unit - A unit leaves I 001 A <gen>

Conditions
(I_001_C Equal to False) and ((Triggering unit) Equal to Demo-Man 0000 <gen>)

Actions
Set I_001 = False

Set I_A = 0.00

I 001 C


This trigger is the Main Trigger or "Event" caused by the Interact Ability being used in the area, or "On the Door"

I 001 C
Events
Unit - Demo-Man 0000 <gen> Begins casting an ability

Conditions
(I_001 Equal to True) and ((Ability being cast) Equal to Interact )

Actions
Destructible - Open Dungeon Gate (Horizontal) 0000 <gen>

Set I_001 = False

Set I_A = 0.00

Set I_001_C = True

Thats the first Sequence complete, Time to move on to the second.


This second Sequence is for the level outside the door which opens the second door.

I 002 A


First trigger of the Sequence. Almost Identical to I 001 A. Just a few minor changes and its ready for use.

I 002 A
Events
Unit - A unit enters I 002 A <gen>

Conditions
(I_001_C Equal to False) and ((Triggering unit) Equal to Demo-Man 0000 <gen>)

Actions
Set I_001 = True

Set I_A = 1.00

I 002 B


Second trigger, once again almost identical to I 001 B. Small changes in variable and region and we're done with this trigger.

I 002 B
Events
Unit - A unit leaves I 002 A <gen>

Conditions
(I_001_C Equal to False) and ((Triggering unit) Equal to Demo-Man 0000 <gen>)

Actions
Set I_001 = False

Set I_A = 0.00

I 002 C


Last trigger of the Sequence. This is the Main Trigger, or "Event." Usually these vary massively from each other as normal triggers would, however in this tutorial they are quite similar so only some small changes and


we're done with this sequence.

I 002 C
Events
Unit - Demo-Man 0000 <gen> Begins casting an ability

Conditions
(I_002 Equal to True) and ((Ability being cast) Equal to Interact )

Actions
Destructible - Open Dungeon Gate (Horizontal) 0001 <gen>

Destructible - Kill Lever 0002 <gen>

Set I_002 = False

Set I_A = 0.00

Set I_002_C = True

 CONGRATULATIONS


Yep we're done. Go ahead and test it. I'll attach my test map and you can play around with it.


I hope to see someone use this in a map because I think its a brilliant idea that should be used more.

#### Attachments

Interactive Environments Demo.w3x

 16.5 KB

 · Views: 373




 Last edited: Jan 21, 2011

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Without initialization triggers with this system or whatever we will need 9 triggers for only 2 gates! What happens if we create 50 gates and same like objects...

Why not to use Boolean instead of Real variable?

#### Cobbot

#### Cobbot

 Level 5

JoinedMar 15, 2008Messages35

Resources: 5

#### Resources
Maps 1Tutorials 4

Well if it counts for anything you can merge all the C triggers into one by using If/Then/Else actions. I just like keeping mine tidy, although it sounds alot I find I can make these pretty quickly. I love Copy and Paste.

Also I'm not quite a master of triggering so I've still not got around to learning Booleans. I'll try to do it with Booleans and if I'm succesful I'll let you know

 - Thanks for the feedback

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Booleans are kind of similar to what you are doing in your trigger. They are basically a primitive variable type that has two values: true and false. Conditions like this:

(I_001_C Equal to 0.00) and ((Triggering unit) Equal to Demo-Man 0000 <gen>)
Are essentially booleans. If the conditions are met, the boolean value returned will be true, but if not, it will be false and the actions will not execute. Take this for example:

Set boolVar = (Triggering Unit) is A structure
If the triggering unit is a structure, it will return true. If the triggering unit is not a structure, boolVar will have a value of false. Boolean variables can also be set directly to true or false, like so:

Set boolVar = True

Set boolVar = False
Although the efficiency difference between using reals/integers/booleans in this case is non-existent (they are all 32-bit), it is best to use booleans as they will be most clear for tutorial purposes.


Update that when you can, but otherwise this tutorial is looking good.

#### Orcnet

#### Orcnet

 Level 30

JoinedJul 31, 2010Messages5,248

Resources: 5

#### Resources
Maps 4Tutorials 1

Unique Tutorial

I like it. thanks for the tutorial dude!

#### Cobbot

#### Cobbot

 Level 5

JoinedMar 15, 2008Messages35

Resources: 5

#### Resources
Maps 1Tutorials 4

Right. Just changed it into Booleans and attached the Updated Map File.

So I guess I just learned Booleans. Thanks PurgeandFire111 for the boolean info. =)

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice job. It looks good to me, unless I overlooked anything.


~Approved.

#### Cobbot

#### Cobbot

 Level 5

JoinedMar 15, 2008Messages35

Resources: 5

#### Resources
Maps 1Tutorials 4

Hooray! Tyvm =)

 You must log in or register to reply here.


### Similar threads

Locked

 Why doesn't this work?


Ramza

Oct 27, 2010

World Editor Help Zone

Replies8Views1K

Oct 27, 2010
defskull

Locked

 how can it work?


saio

Sep 23, 2007

World Editor Help Zone

Replies20Views3K

Sep 25, 2007
Allcor

Locked

 Spell Help


DarkFrog

Mar 30, 2009

Requests

Replies6Views1K

Mar 31, 2009
xXMephistoXx

Locked

[Trigger]
 Is this good??


turaak

Jun 18, 2008

Triggers & Scripts

Replies23Views2K

Jun 20, 2008
GhostWolf

Locked

 Ctf


iown_azz

Aug 14, 2010

Triggers & Scripts

Replies6Views1K

Aug 14, 2010
iown_azz

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