---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/vanilla-jass-header-initialization.240155/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Vanilla JASS Header Initialization
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/vanilla-jass-header-initialization.240155/>
#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Vanilla Jass Header Initialization

Introduction


In this tutorial, I'm going to teach you some of the variant and effective inits or initialization of the Map Header, and also a technique of mine which is somewhat unique.


One of the problems with vanilla JASS is that you can't create initialization functions in the map's header. It isn't as simple as creating an initialization function in a trigger. However, there are ways to get around this problem.


Before we start, if you are coding right now, leave the Initialization function of your script in the Header and read the following.

Tip #1 : Use an initializing trigger.


This technique is the most commonly used. What it does is simply like this:

 JASS:

// A Header code
function InitMe takes nothing returns nothing
 set udg_Hash = InitHashtable()
endfunction

// Seperate code, inside a trigger
function InitTrig_Init takes nothing returns nothing
 call InitMe()
endfunction


As you can see, you used a trigger to initialize a code inside the header via executing it inside the Initialization function of a trigger.


Another example:

 JASS:

function Init1 takes nothing returns nothing
 call BJDebugMsg("First Initialization complete")
endfunction

function Init2 takes nothing returns nothing
 call BJDebugMsg("Second Initialization complete")
endfunction


// a seperate code, inside a trigger
function InitTrig_Hello takes nothing returns nothing
 call Init1()
 call Init2()
endfunction

Tip #2 : Use the trigger itself as a customized Header


Somewhat impossible you say? For normal WE, yes, because this doesn't compile, though if you look into this method/tutorial:

https://www.hiveworkshop.com/forums...ials-280/use-functions-other-triggers-232537/ , it will tell you how to use functions from other triggers, or making it a customized Header.


Here's an example of how to do it:

 JASS:

function Add takes real r, real r2 returns real
 call SaveReal(udg_Hash, 0, 0, r + r2)
 return r + r2
endfunction

function Sub takes real r, real r2 returns real
 call SaveReal(udg_Hash, 0, 0, r - r2)
 return r - r2
endfunction

function Mul takes real r, real r2 returns real
 call SaveReal(udg_Hash, 0, 0, r * r2)
 return r * r2
endfunction

function Div takes real r, real r2 returns real
 if r2 == 0 then
 call SaveReal(udg_Hash, 0, 0, 0)
 return 0
 endif
 call SaveReal(udg_Hash, 0, 0, r/r2)
 return r/r2
endfunction

function InitTrig_MyHeader takes nothing returns nothing
 set udg_Hash = InitHashtable()
endfunction


use the script from other script:

 JASS:

function Print takes nothing returns nothing
 local real r
 set r = Div(1, 2)
 set r = Mul(r, r)
 set r = Sub(r, r)
 set r = Add(r, r)
 call BJDebugMsg(R2S(r))
endfunction

function InitTrig_Hello takes nothing returns nothing
 call Print()
endfunction

Tip #3 : My Method


This method is actually not initialized on Game Start.


I'm going to show you how I initialize my header without using any triggers, or perhaps, if someone use one of the functions in the code. My method is somewhat unique because I haven't seen somebody use this kind of technique.


Here's how it goes, you need first a variable that can be constant(real, boolean, integer, etc.) but I'll suggest boolean. This boolean will be our Initialization checker. So how to use it? Here's an example:

 JASS:

//Whole script in header
function InitError takes nothing returns nothing
 if not udg_InitError then // check whether InitError is initialized
 // if not, set InitError to true
 set udg_InitError = true

 // Code constants or initialized values, example:
 set udg_Hash = InitHashtable()
 endif
endfunction

function Error takes string message returns nothing
 call InitError() // calls everytime Error is called, but executes once.
 set udg_Error_Stack = udg_Error_Stack + 1
 call SaveStr(udg_Hash, 0, udg_Error_Stack, message) // working
endfunction


As you can see in the script above, the code initializes when the function Error is called,though the initialization is kept executed once due to the variable "udg_InitError"


Here are some codes that uses this kind of method:

https://www.hiveworkshop.com/forums/spells-569/queuetable-v1-0-a-236053/?prev=status=a&u=Almia

https://www.hiveworkshop.com/forums/spells-569/macro-v1-2-a-236764/?prev=status=a&u=Almia

https://www.hiveworkshop.com/forums/spells-569/stacktable-v1-0-a-235990/?prev=status=a&u=Almia

https://www.hiveworkshop.com/forums...-indexer-v1-5-a-227314/?prev=status=a&u=Almia

https://www.hiveworkshop.com/forums...st-table-v1-4-a-230076/?prev=status=a&u=Almia


As you can see, some of the codes don't have Init functions, this is because they are merged in the main function of the system, which is the first function to be called, e.g Register..... , Create......


 Last edited by a moderator: Aug 28, 2013

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

nice tutorial, even though i'm not understand jass, i can understand this..

OT:

 also a technique of mine which is somewhat unique


Click to expand...

don't get too cocky

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Thanks

Well, that method I use can't be found in some users.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

no problem, buddy

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Looks good. A similar technique was also used for local handle vars:

 JASS:

function LocalVars takes nothing returns gamecache
 if udg_GameCache == null then
 set udg_GameCache = InitGameCache("LocalVars.w3v")
 endif
 return udg_GameCache
endfunction

It is a good technique overall without too much overhead.

You can still claim it as your own, though. After all, you discovered it by yourself.  I just posted that to show that it is indeed a reliable method.


 Almia said:



 One of the problems in Vanilla Jass is the Header Initialization. The problem is that the Header can't be initialized inside the Map Header. But, there are some ways of initializing them.


Click to expand...

Could be rephrased. I'd recommend:

One of the problems with vanilla JASS is that you can't create initialization functions in the map's header. It isn't as simple as creating an initialization function in a trigger. However, there are ways to get around this problem.

Something like that.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Thanks for the rephrase

Yeah, I feel that phrase is much better.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice nice. Approved.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Thanks

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

congrats, dude!

 You must log in or register to reply here.


### Similar threads

 All About Inlining


Flux

Mar 25, 2016

JASS Tutorials

Replies3Views5K

Apr 23, 2016
PurgeandFire

Locked

 S2R vs JassParser.S2R (so much for code reuse...)


Aniki

Aug 29, 2016

Advanced Scripting

Replies0Views1K

Aug 29, 2016
Aniki

Locked

[JASS]
 Hero revival


NeonBladeMaster

Jun 23, 2008

Triggers & Scripts

Replies0Views950

Jun 23, 2008
NeonBladeMaster

Locked

 Freeze Ground Spell JASS code check


D4RK_G4ND4LF

Jan 22, 2010

Triggers & Scripts

Replies1Views1K

Feb 2, 2010
Darkness-4ever

Locked

 execute_func


Aniki

Feb 5, 2016

Advanced Scripting

Replies13Views4K

Aug 12, 2016
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