---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/use-functions-from-other-triggers.232537/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Use Functions from Other Triggers
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/use-functions-from-other-triggers.232537/>
#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Use functions from other triggers [Standard JASS]

I. Introduction

This tutorial allows you to use the functions from other triggers.

There are problems we experience when we want to use other functions from other triggers.Examples is that we get syntax error when we try to use functions from other triggers with or without arguements. You will probably need a JNGP to use the functions without conflicts/problems that you will experience.

II. What are the problems?

 JASS:

function Message takes string s returns nothing
 call BJDebugMsg(s)
endfunction

//===========================================================================
function InitTrig_Untitled_Trigger_001 takes nothing returns nothing
 set gg_trg_Untitled_Trigger_001 = CreateTrigger( )
endfunction

This example of snippet will explain it.

We want to use the function MessageX to display messages, but the thing is that if we try to use this like this from other triggers : call Message("Hello"), this will give us a syntax error " Expected a function name ".But if you use it inside the trigger it is located, it will work perfectly.

III. The Solution

I discovered the solution when I tried to code a system using scripts. The solution to this problem is to use a simple snippet of function inside the Map Header(the top-of-the-script Script, located in the upper-most of the Trigger Editor with the map icon). We shall add a dummy function there so that we can use the functions properly. Example : Let's add this simple snippet of code :









 JASS:

function EnableScriptUsage takes nothing returns nothing
endfunction

So what does this do?

Nothing. But when a map header has a function inside it, it will allow any functions from the triggers to be used by external triggers.

Simple huh?

IV. Known bugs

You can experience some bugs when using this trick. Try removing any function in the map header while other triggers uses the functions from the external triggers.Enable/Disable the trigger that uses external functions (to check syntax), this won't show any syntax error.But when you tried to save the map, a syntax error message will show.

There is a bug also when attempting to move the scripts. The trigger you are going to move in must be the first trigger to be created on your map,or else you are doomed.

The solution to the problem is replacing your first trigger,then move all of the actions to a replica.Then move the scripts you want to the trigger replaced.

Note: All of the triggers in your map will be moved in the header,and then will be arranged based on how old they are in the Trigger Editor,starting from the oldest,which will be on top,and the newest,will be on the bottom.


 Last edited by a moderator: May 1, 2013

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

I'm not sure if this warrants a tutorial.

Additionally I don't encourage people to rely on bunch of map header code, as it can get very cluttered. Just stick to gui or use vjass.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

They will just use the map header code for a 2-lined function.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

so if I put a empty function inside the map header I wont get expected function name?

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Correct.

This is good to prevent the map header with so much API inside it.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

that is really useful, rep +

#### looking_for_help

#### looking_for_help

 Level 14

JoinedDec 12, 2012Messages1,007

Resources: 5

#### Resources
Spells and Systems 2JASS 3

Almia said:

 They will just use the map header code for a 2-lined function.


Click to expand...

You don't even have to keep the function.

Once you saved your map with a function inside the map header external scripts get unlocked even if you remove the function again (you get a message like "own script code is used...").

So you can keep your map header completly clean.

-> Very useful.


 Last edited: Apr 9, 2013

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Thanks for the feedback

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Updated.

I found some bugs.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Sorry about the super late response. I had to test this myself.

It seems to work for the most part. There are some issues (e.g. if you have A call B, then B call C or something like that), but I seemed to work around them by deleting and recreating the triggers as you said.

I remember hearing someone say that WE compiles it in a random order, it would be cool if you could figure out the exact order WE compiles it in (or confirm that it is random). Would be interesting to know.

As for the tutorial, it is well-explained. The only issues are the signature and the font + color. However, those are somewhat minor. I will detach the signature, and then it is approved.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Actually,the scripts are ordered based on how old the trigger is(just like what i have said)

It goes like this:

Oldest Trigger A

Older Trigger B

Old Trigger C

Young Trigger D

Younger Trigger E

Youngest Trigger F

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

This doesn't apply if ExecuteFunc is used (which is ugly with their slow response though), and the TriggerExecute thingy as well. Sadly, they're not so similar with executing the function with their returns and such.

#### Wietlol

#### Wietlol

 Level 24

JoinedAug 1, 2013Messages4,658

Resources: 8

#### Resources
Maps 1Spells and Systems 7

@Daffa the Mage

I did make a system so ExecuteFunc() can have return values, parameters, etc.

I made a String2JASS system

It is supposed to convert chat messages to JASS so you can get variable values, set variable values, execute functions, etc during gameplay to test the map.

However... it is hard to implement and is not yet finished.

But someone said it was bull shit to have... so I stopped working on it.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

This no longer works fully for me btw. Even with a function in the header the editor still saves the oldest trigger first. So I get undeclared function name in quite a lot of my projects these days. I have to keep copy pasting stuff :-/

#### Wietlol

#### Wietlol

 Level 24

JoinedAug 1, 2013Messages4,658

Resources: 8

#### Resources
Maps 1Spells and Systems 7

A function in the header is always saved at the top (with the exception of libraries).

I guess you are missing something.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Maybe you didn't read my instruction

I rather suggest pasting first the systems where other codes rely, codes that rely to systems that will be relied, and codes that rely to other codes

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Easier said than done

#### SAUS

#### SAUS

 Level 12

JoinedMay 22, 2015Messages1,051

Hey, I found something out about the trigger order when saving:

When you add a new trigger and save, it is saved at the bottom of the script. This means it will break if you create a new trigger that has a function that you want to use in a trigger that already existed or a trigger that you created in the current session before this new trigger. This is the known problem that forces you to put your global functions inside the header script.

However, I found out something that might be useful. If you save your map, and then close the editor, reopen the editor, load the map, and then write a syntax error in the header script (then try to save), you will see that the triggers are now defined in the order that they appear in your trigger editor window. This allows some purposeful ordering of trigger creation (the InitTrig functions) and also allows you to write libraries in regular JASS and use them in all your triggers.

The steps to make it work are:

1) Create the new trigger that will be your system or whatever.

2) Leave your trigger blank, and move it in the trigger editor to the spot you want it to be loaded.

3) Save your map.

4) Close the world editor.

5) Open the world editor and load your map.

6) Add any functions you want to use to the blank trigger.

Then you can use those functions from any other trigger below it in your code. Just remember that if you add a new trigger above this system trigger, it might work if you use functions from it on the first save, but it might also start to report errors if you try to save again after closing and reopening.

Anyway, I thought some people might like this.

#### Wietlol

#### Wietlol

 Level 24

JoinedAug 1, 2013Messages4,658

Resources: 8

#### Resources
Maps 1Spells and Systems 7

Aka... use libraries.

When you save the map, the editor compiles all triggers/scripts into one big file, known as the "war3map.j".

Thinking code-wise for the editor, all triggers are inside a list (array) and when you create a new trigger, you just increase the array by 1 and place your new trigger in the last slot.

"But hey! Doesnt that mess with the order of the triggers?" Not really, because you dont want to rely on something like the order of triggers, so we dont care how it is.

If you want to use functions globally, then use either the header script or a precompiler like JASSHelper (vJASS).

That the order of how the triggers are used in the order of how they are shown is probably because the array will be recreated in a certain moment of the steps... which is a glitch... or at least, it looks like it.

So... It is funny to know such things, but not really useful as there are way better things to rely on.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

@SAUS: Pretty cool! But is it in the order specified in the trigger editor, or is it based on when the trigger was made? (e.g. older ones go first?)

The reason I ask is because Almia's note at the end seems to conflict with your theory:

 Note: All of the triggers in your map will be moved in the header,and then will be arranged based on how old they are in the Trigger Editor,starting from the oldest,which will be on top,and the newest,will be on the bottom.


Click to expand...

If you can verify that it behaves like you said, I'll link your post in the main tutorial for the benefit of other users.

#### SAUS

#### SAUS

 Level 12

JoinedMay 22, 2015Messages1,051

PurgeandFire said:

 @SAUS: Pretty cool! But is it in the order specified in the trigger editor, or is it based on when the trigger was made? (e.g. older ones go first?)


The reason I ask is because Almia's note at the end seems to conflict with your theory:


If you can verify that it behaves like you said, I'll link your post in the main tutorial for the benefit of other users.

Click to expand...


It seems to be the order specified in the trigger editor - you just have to save and reload your map if you add new triggers (if the order matters). It's sort of hacky the way it is, and I am uncertain if it is completely reliable or not, but so far it has worked. I will test in a new blank map maybe tonight or tomorrow night.


Steps should be:


Create a trigger (TriggerA), convert it to JASS.


Create another trigger(TriggerB), convert it to JASS.


Move TriggerB above TriggerA in the trigger editor.


Add a function to TriggerB:

 JASS:

function triggerOrderTest takes nothing returns nothing
endfunction

Call the funciton in TriggerA's InitTrig function.


Try to save the map (it should get an error).


Comment out the call to triggerOrderTest in TriggerA's InitTrig funciton.


Try to save the map (should work).


Close wc3 map editor and reopen it -> load your test map.


Uncomment the function call and try to save it again.


That was more steps than I thought haha. Anyway, I will do it myself when I get the chance, but someone else can try it as well if they want to.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

It's pretty simple to solve actually. We just need to move trigger A (where we call the function) beneath trigger B (that contains the function) on compilation. How? Just click on trigger A > Copy (Ctrl+C) > Delete (Del) > Paste (Ctrl+V). Then try to save the map. And voila! There you go...

But sometimes the trigger order will be re-arranged by it self in some random occasions, causing the problem to reappear. That's why, FOR GOODNESS SAKE PLEASE DON'T USE GUI!!

#### SAUS

#### SAUS

 Level 12

JoinedMay 22, 2015Messages1,051

Quilnez said:

 It's pretty simple to solve actually. We just need to move trigger A (where we call the function) beneath trigger B (that contains the function) on compilation. How? Just click on trigger A > Copy (Ctrl+C) > Delete (Del) > Paste (Ctrl+V). Then try to save the map. And voila! There you go...


But sometimes the trigger order will be re-arranged by it self in some random occasions, causing the problem to reappear. That's why, FOR GOODNESS SAKE PLEASE DON'T USE GUI!!


Click to expand...


I don't use GUI. I am wondering what these random occasions are or if they are even, indeed, random.


From what I've seen, any time you make a new trigger in the current session, no matter where you put it in your list, it will be at the bottom of the final map script. However, when you reload your map (close the map after successful save and reopen your map), it is where you put it in your list of triggers in the trigger editor.


This means that you can then put triggers above the trigger with the global functions and use those functions without problem until you reload the map. This is consistent with how I've seen the order of the triggers being saved. It has not been random so far and I am even using it for multiple system in my own map without problems.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

I hate it when someone bumps a 3 year old thread in which I have forgotten now :v

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

SAUS said:

 I don't use GUI. I am wondering what these random occasions are or if they are even, indeed, random.

From what I've seen, any time you make a new trigger in the current session, no matter where you put it in your list, it will be at the bottom of the final map script. However, when you reload your map (close the map after successful save and reopen your map), it is where you put it in your list of triggers in the trigger editor.

This means that you can then put triggers above the trigger with the global functions and use those functions without problem until you reload the map. This is consistent with how I've seen the order of the triggers being saved. It has not been random so far and I am even using it for multiple system in my own map without problems.


Click to expand...

I don't know in what occasion or is it random or not because I'm not very experienced about functions in GUI. The last time I created a map in GUI was when I didn't even know what function is.

Almia said:

 I hate it when someone bumps a 3 year old thread in which I have forgotten now :v


Click to expand...

Well, this is tutorial section. You shouldn't be surprised when finally somebody find this useful 3 years later and want to ask something about it.

#### SAUS

#### SAUS

 Level 12

JoinedMay 22, 2015Messages1,051

Quilnez said:

 I don't know in what occasion or is it random or not because I'm not very experienced about functions in GUI. The last time I created a map in GUI was when I didn't even know what function is.


Click to expand...


I'm writing all my stuff in JASS, but in the regular editor. I guess you could use this for full GUI libraries (with custom script to make the functions), but it would be ugly to implement into your map since you would have to fiddle with saving until it orders the way you want it to.


I can't be certain it doesn't mess up after some time or from some specific circumstances, but it has been consistent for me so far (in my map - haven't made the pure test map yet). I'll probably try to throw some screenshots together if there's anything I can show to help explain what is happening.

 You must log in or register to reply here.


### Similar threads

 Vanilla Jass Header Initialization


Almia

Aug 27, 2013

JASS Tutorials

Replies8Views6K

Aug 31, 2013
Rheiko

 Converting GUI into Efficient JASS


deathismyfriend

Apr 18, 2013

JASS Tutorials
2

Replies58Views22K

Feb 22, 2015
ZiBitheWand3r3r

Locked

 Custom Function Inside GUI Trigger


defskull

Aug 24, 2012

World Editor Help Zone

Replies40Views7K

Aug 30, 2012
Erkki2

A

Locked

[JASS]
 Create a function in a converted GUI trigger


Ardenian

Feb 20, 2016

Triggers & Scripts

Replies5Views1K

Feb 21, 2016
GIMLI_2

 Recursion Safety


Magtheridon96

Jul 30, 2013

JASS Tutorials

Replies3Views6K

Aug 18, 2013
Magtheridon96

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