---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/tree-revival-system.31805/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Tree Revival System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/tree-revival-system.31805/>
#### Hero12341234

#### Hero12341234

 Level 2

JoinedDec 6, 2006Messages72

Resources: 3

#### Resources
Icons 1Tutorials 2

Tutorial: Creating a Tree Revival System


This will show you how to write a Tree Revival System in JASS.


Please give some kind of credit if you use this system.

Question:

I don't know JASS can you make a GUI one?

Answer:

If you don't know JASS it's OK. I explained everything: you can't possibly make a mistake... if you do try to copy and paste. Or use the included sample map.


Copy and Paste these functions below into your map's custom header.

 JASS:

constant function TreeRegrowTime takes nothing returns real
 return 10.00 //the amount of time you want before the tree respawns
endfunction

//To add more trees press CTRl+D while looking at the Object Editor
//then follow the same format and add your tree's RAWCODE inside single quotes (Example ' ' )

function IsDesTree takes destructable a returns boolean
 local integer d=GetDestructableTypeId(a)
 if d =='ATtr' then
 return true
 elseif d=='BTtw' then
 return true
 elseif d=='KTtw' then
 return true
 elseif d=='YTft' then
 return true
 elseif d=='JTct' then
 return true
 elseif d=='YTst' then
 return true
 elseif d=='YTct' then
 return true
 elseif d=='YTwt' then
 return true
 elseif d=='JTwt' then
 return true
 elseif d=='DTsh' then
 return true
 elseif d=='FTtw' then
 return true
 elseif d=='CTtr' then
 return true
 elseif d=='ITtw' then
 return true
 elseif d=='NTtw' then
 return true
 elseif d=='OTtw' then
 return true
 elseif d=='ZTtw' then
 return true
 elseif d=='WTst' then
 return true
 elseif d=='LTlt' then
 return true
 elseif d=='GTsh' then
 return true
 elseif d=='Xtlt' then
 return true
 elseif d=='WTtw' then
 return true
 elseif d=='Attc' then
 return true
 elseif d=='BTtc' then
 return true
 elseif d=='CTtc' then
 return true
 elseif d=='ITtc' then
 return true
 elseif d=='NTtc' then
 return true
 elseif d=='ZTtc' then
 return true
 else
 return false
endif
endfunction

function RegrowTrees takes nothing returns nothing
 local destructable tree=GetDyingDestructable()
 call TriggerSleepAction(TreeRegrowTime())
 call DestructableRestoreLife( tree, GetDestructableMaxLife(tree), true )
 set tree=null
endfunction

function Trig_Int_Tree_Revival takes nothing returns nothing
 local trigger t
 if IsDesTree(GetEnumDestructable())==true then
 set t=CreateTrigger()
 call TriggerRegisterDeathEvent( t, GetEnumDestructable() )
 call TriggerAddAction(t,function RegrowTrees)
endif
endfunction

function Int_Tree_Revive takes nothing returns nothing
 call EnumDestructablesInRectAll( GetPlayableMapRect(), function Trig_Int_Tree_Revival )
endfunction


This may look kind of confusing but I will explain what it does.

 JASS:

function Int_Tree_Revive takes nothing returns nothing
 call EnumDestructablesInRectAll( GetPlayableMapRect(), function Trig_Int_Tree_Revival )
endfunction


This is VERY vital to the system. It starts it all. How does it do it you ask?


Simple: it Picks Every Destructible In The Playable Map...and then it calls the function Trig_Int_Tree Revival.

 JASS:

function IsDesTree takes destructable a returns boolean
 local integer d=GetDestructableTypeId(a)
 if d =='ATtr' then
 return true
 elseif d=='BTtw' then
 return true
 elseif d=='KTtw' then
 return true
 elseif d=='YTft' then
 return true
 elseif d=='JTct' then
 return true
 elseif d=='YTst' then
 return true
 elseif d=='YTct' then
 return true
 elseif d=='YTwt' then
 return true
 elseif d=='JTwt' then
 return true
 elseif d=='DTsh' then
 return true
 elseif d=='FTtw' then
 return true
 elseif d=='CTtr' then
 return true
 elseif d=='ITtw' then
 return true
 elseif d=='NTtw' then
 return true
 elseif d=='OTtw' then
 return true
 elseif d=='ZTtw' then
 return true
 elseif d=='WTst' then
 return true
 elseif d=='LTlt' then
 return true
 elseif d=='GTsh' then
 return true
 elseif d=='Xtlt' then
 return true
 elseif d=='WTtw' then
 return true
 elseif d=='Attc' then
 return true
 elseif d=='BTtc' then
 return true
 elseif d=='CTtc' then
 return true
 elseif d=='ITtc' then
 return true
 elseif d=='NTtc' then
 return true
 elseif d=='ZTtc' then
 return true
 else
 return false
endif
endfunction

This is the rawcode for every single tree in Warcraft...you can add your own easily by pressing CTRl+D while looking at the Object Editor


//then follow the same format and add your tree's RAWCODE inside single quotes (Example ' ' )

 JASS:

function Trig_Int_Tree_Revival takes nothing returns nothing
 local trigger t
 if IsDesTree(GetEnumDestructable())==true then
 set t=CreateTrigger()
 call TriggerRegisterDeathEvent( t, GetEnumDestructable() )
 call TriggerAddAction(t,function RegrowTrees)
endif
endfunction


This is pretty simple also. It is called at Map Initialization. What it does is create a local trigger IF the destructible is a tree. This is found out by using the function IsDesTree which is found in this tutorial, "t" in this case. It adds the Event a Destructable dies and adds an action. Actually it will call function RegrowTrees everytime the event is activated.

 JASS:

function RegrowTrees takes nothing returns nothing
 local destructable tree=GetDyingDestructable()
 call TriggerSleepAction(TreeRegrowTime())
 call DestructableRestoreLife( tree, GetDestructableMaxLife(tree), true )
 set tree=null
endfunction


What this does is to get the dying destructable(s) and sets it to local destructable tree. It then waits TreeRegrowTime seconds..if you don't know what this is yet...it will be explained. After the wait is done it will Revive the Destructable SHOWING birth animations. If you don't want it to show birth animations then set the TRUE to FALSE.

 JASS:

constant function TreeRegrowTime takes nothing returns real
 return 10.00 //the amount of time you want before the tree respawns
endfunction


This constant function is the Wait that is used in the RegrowTrees function.


It returns 10.00 seconds to wait. This return can never change while the system is running. If you want the trees to revive after a different alloted time then edit the 10.00 to whatever number you please.

Example:

 JASS:

constant function TreeRegrowTime takes nothing returns real
 return 30.00 //the amount of time you want before the tree respawns
endfunction


I just editted the 10.00 to 30.00 now after 30.00 seconds the trees will respawn instead of after 10.00 seconds.


If you followed the tutorial you should have this in your map header by now.


Now comes the easy part...


Inside your Map Initialization trigger add the action:

 JASS:

Custom script: call Int_Tree_Revive()


Ok..so here is a screen shot of the Map Initialization trigger in the sample map.


Thanks for reading this tutorial. I hope it helped you learn how to create a tree revival system in JASS.


-Hero


Sample Map: View attachment Tree Revival System.w3x


 Last edited by a moderator: May 2, 2007

#### Bannar

#### Bannar

 Level 26

JoinedMar 19, 2008Messages3,141

Resources: 23

#### Resources
Spells and Systems 5JASS 15Tutorials 1Wurst 2

This should be graveyarded. Hasn't been updated for long time.

The idea "let me show you how to write system just in jass" is rather unacceptable, such approach leads to spell/system submission instead - which this actually is not.

The system itself, described by author is horribly inefficienct and method is medieval. Goal is to use IsDestructableTree + simple revival script that adds everything that has been correctly filtered by IDT. We do not need any "switch -case" spam.

On top of that, this can be easily tricked to return false results

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

This is actually really useful, even though I'd use another method for it.

I guess it could be graveyard-ed because of that..

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

I don't use JASS, I'm a GUI practitioner, but I tested this script by inserting it into the startup map and to my incredible surprise, it worked perfectly.

I don't understand why for Bannar the system doesn't seem to work.

I have just one note about it: What if we want to have trees that can revive every 10.00 seconds and Rawcodes for others to revive trees every 80.00 seconds?

I don't understand anything about JASS, I tried to switch codes and add a new timer for new rawcodes, but it didn't work.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Next to it being 6 years old, the main issue is the use of TriggerSleepAction. TriggerSleepAction generates net traffic and is not an accurate delay as it does not scale with game speed or track game time. For example if one killed a tree then was stuck in the waiting for player window or the game was paused for 60 seconds then when gameplay resumes the trees might already be revived despite only a few seconds of gameplay since the tree was killed.

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

Dear friend, if this's 6 years, but if your cause is still sustained ... possibly it is because there is a problem of very great effectiveness for reviving trees, it is not such an easy system to find anywhere.

As for the lack of effectiveness in the accuracy of time, this for me is not a problem through TriggerSleepAction, after all, my search is for a Singleplayer system, I will not be leaving or I will not have problems with disconnections and if I save the game returning with a break seconds, it also won't make a difference if only in that short time there will be a new spontaneous event. My main problem is the inefficiency in GUI systems and making sure to clean up leaks. Most triggers do not work perfectly in "Regions" or in "Destructible die" condition, I just need the most standard trees in some period of the game to be reborn, regardless of the delay being for a short or long period, still sporadically ...

Because a BloodMage can fire a FlameStrike and bring down all the trees and the GUI system in a region that does not read dead trees correctly, it will end at once and never revive trees again.

My only problem is that I am not familiar with JASS, I understood the option for the rebirth interval and the placement of rawcodes for each desired tree.

What I just wanted was to be able to separate a longer interval period (900.0 sec) for standard type trees and insert another period (20.0 sec) for smaller type trees (customized) for a harvesting system. My map has an agricultural economic system and I need it to be reborn in short periods.

But it also has a long-term revival of standard trees.

Would it be possible to differentiate between intervals of time and types of trees?


 Last edited: Jul 8, 2020

#### GIMLI_2

#### GIMLI_2

 Level 18

JoinedMar 21, 2011Messages1,622

Resources: 3

#### Resources
Maps 2Spells and Systems 1

you can do something like this

 JASS:

globals
 Table t
endglobals

function TreeRegrowTime takes destructable d returns real
 return t[GetDestructableTypeId(d)]
endfunction

function Init takes nothing returns nothing
 set t = Table.create()
 set t.real['1234'] = 10.0
 set t.real['abcd'] = 25.0
 ...
endfunction

requires Table:

[Snippet] New Table

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

GIMLI_2 said:

 you can do something like this

requires Table:

[Snippet] New Table

Click to expand...

Sorry friend, do you mean that I would need to insert a new table code ([Snippet] New Table) to activate the possibility of such a function? or just create a new table variable with the array specification?

Pardon, but I'm a zero on the left in JASS subjects... I didn't understand much, I'll see if I read Bribe's post to be able to reason something ...

I'm brazilian, also disregard my English.

#### GIMLI_2

#### GIMLI_2

 Level 18

JoinedMar 21, 2011Messages1,622

Resources: 3

#### Resources
Maps 2Spells and Systems 1

All you have to do is copy&paste the Table Snippet into your map.

Basically, Table is used here as an array, but arrays wont work due to their "low" index limitations.

GetDestructableTypeId(Destructable d) returns a large number that wont fit into an array index, thats why we use table.

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

GIMLI_2 said:

 All you have to do is copy&paste the Table Snippet into your map.

Basically, Table is used here as an array, but arrays wont work due to their "low" index limitations.

GetDestructableTypeId(Destructable d) returns a large number that wont fit into an array index, thats why we use table.


Click to expand...

I think I understand. So:

Regarding what you say "require" [Snippet] New Table, I would need to copy the snippet table script, the JASS Code by Bribe: "library Table ... endlibrary" to the header of my map.

And then I will need to remove (or not) the "TreeRegrowTime" function from the author of the post and then replace (or add) with the description you passed on:

"globals

 Table t

endglobals

. . .

function TreeRegrowTime

t.real set ['1234'] = 10.0

 . . .

endfunction

"

But where ['abcd'] would be Rawcode

and "= 25.0" are the time for each one?

#### GIMLI_2

#### GIMLI_2

 Level 18

JoinedMar 21, 2011Messages1,622

Resources: 3

#### Resources
Maps 2Spells and Systems 1

Exactly. You dont have to put it in map header tho.

You have to initialize the table data somewhere (for example with an initialization event)

set t.real['BTtw'] = 20.0

set t.real['....

this would take a tree of type 'BTtw' 20 seconds to respawn.

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

@GIMLI_2 , Sorry to be bothering you, but still no success!

In the GUI initialization trigger I fired the call to the function "Custom script: call Init ()".

I removed the TreeRegrowTime function (from the author) and added the data you suggested using the new TreeRegrowTime function next to the table with the new raw codes, but even so the map does not save and creates an error for the type of table.

And if I add all the data from the "[Snippet] New Table" to the code, it also creates an error for the redeclared identifier "t", I think it is unnecessary to insert that.

If you have any solution, thank you, here are images of the error attached.

https://drive.google.com/file/d/131IAnUMOSVZJAz9mM0S0Iuw0JzbFRgPQ/view?usp=sharing

https://drive.google.com/file/d/1Ylhhd7h8uYRsUDnXrI2IYT4i03gUyMcl/view?usp=sharing

https://drive.google.com/file/d/1lxdbso6ibGEIj0b4u8108RQ6dQzw2s84/view?usp=sharing

#### GIMLI_2

#### GIMLI_2

 Level 18

JoinedMar 21, 2011Messages1,622

Resources: 3

#### Resources
Maps 2Spells and Systems 1

 @GIMLI_2 , Sorry to be bothering you, but still no success!


Click to expand...

You are not bothering me^^

Always declare globals on top.

The first error is because of not having Table Snippet in your map.

About the second error.. hmm, try to rename your table instance to something else than "t" and try again

You can upload your map if it still doesnt work, so i can have a look at it.

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

@GIMLI_2

I tried several ways, but it always comes down to some script error. I definitely don't know JASS.

I'm sending you a test map. Consider only what is in the "custom script code" and the customscript calls in the first trigger of the Map_Initializacion folder.

Disregarding all other remaining triggers, this is not a real playable map, it is for testing purposes only.

Thank you.

OBS. In this custom script code, the code "Tree Revival System" from the author of the post was added. I also added your table template to what we aim for here. I also added the Snippet-Table at the end.

Under these conditions, it is impossible to save the map without creating errors.

#### Attachments

MapTest_TreeEdit.rar

 3.9 MB

 · Views: 138




 Last edited: Jul 14, 2020

#### GIMLI_2

#### GIMLI_2

 Level 18

JoinedMar 21, 2011Messages1,622

Resources: 3

#### Resources
Maps 2Spells and Systems 1

Try this out, i didnt test it but it should work

#### Attachments

MapTest_TreeEdit.w3x

 4.5 MB

 · Views: 125



#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

GIMLI_2 said:

 Try this out, i didnt test it but it should work


Click to expand...

The trees are even reborn normally. But instantly, every 1 second is reborn, unfortunately it seems that the 10 or 25 second timer is not identified by the table. I even changed it to 120 sec, but it still resurrects quickly. Perhaps some specification was missing...

As for the codes, the organization by separate triggers really got a lot better.

EDIT: For those interested, I leave an alternative tip to revive different trees.

I was researching a little more and for this case, using "Waits" fits very well. If you want to use a time over 10s, which is normal as there are cases where we need trees to be reborn for more than 90s. Using GUI with "Waits" will not have a big impact on time, as it creates a fraction of a second difference in delay... only!

The timer may not be as accurate as "TriggerSleepAction", however if the game pauses, Waits will pause as well. Therefore,

it's more advantageous to use "Waits"!

I leave the link of an unknown author of the Epic website that has a map with a good example, in this case the trees are reborn at about 5sec, if you want only one specific tree to be reborn, create triggers for each tree by placing the condition

"dying destructible is a tree of type xxx"

and in the action changing to create the same type of tree in the position dying.

Ps. Maybe there is a Leak in the position that reborn the tree, I don't remember if the map author used Script to remove the point, I think the ideal is to create a point variable and clean the variable afterwards.

Map: Tree Regrow & Plant Tree

Tree Regrow & Plant Tree - Warcraft 3 Maps - Epic War.com


 Last edited: Jul 22, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

This has not aged well enough.

The visuals do not hold up, this is fix-able however as I can just edit the thread.

More importantly, the way to detect a tree is not best practice. Last I checked, using a dummy unit and ordering it to harvest a destructable scales better because you do not have to have hard-coded list. Example and shameless advertisement: IsDestructableTree GUI 1.1

Additionally, as pointed out this is borderline a system submission at the moment. While some concepts are explained, I would argue that if you know JASS and can follow the instructions you do not need a tutorial because it's quite basic. More than likely you just copy the code and don't read, at which point its just a system.

 You must log in or register to reply here.


### Similar threads

Locked

 I go away for a year . . . (Huge saving errors)


Black-Templar

Dec 23, 2010

Triggers & Scripts

Replies5Views1K

Dec 23, 2010
PurgeandFire

Locked

 Mutable strings


Aniki

Mar 28, 2018

Advanced Scripting

Replies4Views1K

Apr 14, 2018
Waffle

Locked

[JASS]
 Import GUI into JASS


AlwaysAndForever

Nov 23, 2018

Triggers & Scripts

Replies6Views2K

Nov 24, 2018
AlwaysAndForever

Locked

 Index System with a replace function?


Pinzu

Feb 2, 2013

Triggers & Scripts

Replies4Views1K

Feb 3, 2013
Pinzu

Locked

[JASS]
 Lag


Zeatherann

Jan 20, 2010

Triggers & Scripts

Replies4Views2K

Jan 26, 2010
Zeatherann

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top