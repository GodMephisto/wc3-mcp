---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/nether-swap.33979/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Nether Swap
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/nether-swap.33979/>
#### Fulla

#### Fulla

 Level 12

JoinedAug 3, 2005Messages744

Resources: 2

#### Resources
Tutorials 2

Nether Swap

Instantly swaps location with target unit:

Intro


This is a very basic and simplistic spell, which makes it ideal for starting out with jass.


This tutorial does not cover the introduction to jass but rather goes straight into making a spell.


It also has a brief introduction to the Channel ability

Starters


Firstly go into the object editor and create a nether swap ability.


I recommend basing it of Channel, which is found under Neutral Hostile> Hero Abilities


Here is a screenshot to show how you should modify it

1 & 2 - These are the special effects that will be added to Caster and Target, I chose Blink, its up to you.

3 - Make sure you have the same options.


> Disable other abilities = false


> Follow Through Time = 0.00


> Options = Visible


> Target Type = Unit


Next we need to create the trigger


Go into the Trigger Editor and create a trigger called: Nether Swap


Please Note: NOT NetherSwap or nether swap etc.but Nether Swap


Should look like this:


Next you need to converted it to custom script:


For now we will skip the Conditions/Actions bit as this is quite boring, Once weve made a few spells I'll explain this to you


So Ill give you a base code to copy paste.


This is what you should currently have:


Copy Paste this:

 JASS:

function Trig_Nether_Swap_Conditions takes nothing returns boolean
 return GetSpellAbilityId() == 'A00P'
endfunction

function Trig_Nether_Swap_Actions takes nothing returns nothing

endfunction

//===========================================================================
function InitTrig_Nether_Swap takes nothing returns nothing
 set gg_trg_Nether_Swap = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(gg_trg_Nether_Swap, EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(gg_trg_Nether_Swap, Condition(function Trig_Nether_Swap_Conditions))
 call TriggerAddAction(gg_trg_Nether_Swap, function Trig_Nether_Swap_Actions)
endfunction


So now it should look like this:


Now we need to make sure the correct ability is in the condition.


For this go back into the Object Editor and click


View > Display Values as Raw Data


Next we need to get the raw data for the ability


In this case it is A000


Note: In your map if may differ so make sure to check.


Next go back into the trigger editor and lets alter the condition


Notice how Ive changed it to 'A000'


Ok we are now ready to start coding the spell

Method


This is how we will do the spell:


- Create a local to store: Casting Unit


- Create a local to store: Target Unit of Ability Being Cast Unit


- Create locals to store: Casting Unit's co-ordinates


- Create locals to store: Target Unit of Ability Being Cast's co-ordinates


- Move Casting Unit


- Move Target Unit of ability being cast


- Clean up leaks

Jass Code


The following code will go inside the Actions section:


Where the cursor is begin typing in there


Firstly we declare the locals

 JASS:

 local unit c = GetTriggerUnit()
 local unit t = GetSpellTargetUnit()


c = the Casting Unit


t = The Target Unit of ability being cast


Next we create locals for their co-ordinates

 JASS:

 local real xc = GetUnitX(c)
 local real yc = GetUnitY(c)
 local real xt = GetUnitX(t)
 local real yt = GetUnitY(t)


xc & yc = the co-ordinates of the Casting Unit


xt & yt = the co-ordinates of the Target Unit of ability being cast


Now we will set each unit's new position

 JASS:

 call SetUnitX(c, xt)
 call SetUnitY(c, yt)
 call SetUnitX(t, xc)
 call SetUnitY(t, yc)


This will place:


- Casting unit at co-ordinates stored for the Target Unit of ability being cast


- Target Unit of ability being cast at co-ordinates stored for the Casting Unit


Finally we clean up the leaks

 JASS:

 set c = null
 set t = null


This is overall how your code should look

 JASS:

function Trig_Nether_Swap_Conditions takes nothing returns boolean
 return GetSpellAbilityId() == 'A00P'
endfunction

function Trig_Nether_Swap_Actions takes nothing returns nothing
 local unit c = GetTriggerUnit()
 local unit t = GetSpellTargetUnit()
 local real xc = GetUnitX(c)
 local real yc = GetUnitY(c)
 local real xt = GetUnitX(t)
 local real yt = GetUnitY(t)
 call SetUnitX(c, xt)
 call SetUnitY(c, yt)
 call SetUnitX(t, xc)
 call SetUnitY(t, yc)
 set c = null
 set t = null
endfunction

//===========================================================================
function InitTrig_Nether_Swap takes nothing returns nothing
 set gg_trg_Nether_Swap = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(gg_trg_Nether_Swap, EVENT_PLAYER_UNIT_SPELL_EFFECT)
 call TriggerAddCondition(gg_trg_Nether_Swap, Condition(function Trig_Nether_Swap_Conditions))
 call TriggerAddAction(gg_trg_Nether_Swap, function Trig_Nether_Swap_Actions)
endfunction


You can make the code, slightly more efficient by replacing:

 JASS:

 local real xt = GetUnitX(t)
 local real yt = GetUnitY(t)
 call SetUnitX(c, xt)
 call SetUnitY(c, yt)


with:

 JASS:

 call SetUnitX(c, GetUnitX(t))
 call SetUnitY(c, GetUnitY(t))


Your jass spell should now work, have fun and enjoy and stay tuned for more step by step mini jass tuts


thx


Fulla


 Last edited by a moderator: Jul 1, 2007

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

you can skip either (xt,yt) or (xc,yc), since SetUnitX/Y ignore pathing

#### Fulla

#### Fulla

 Level 12

JoinedAug 3, 2005Messages744

Resources: 2

#### Resources
Tutorials 2

What do you mean by skip sorry?


Anyways to improve the tut?

#### Diablo-dk

#### Diablo-dk

 Level 10

JoinedNov 10, 2004Messages351

Resources: 3

#### Resources
Maps 1Spells and Systems 1Tutorials 1

Use jass tags instead of code tags?

By skipping i think he means that you can shorten it to this:

 JASS:

 local real x = GetUnitX(c)
 local real y = GetUnitY(c)
 call SetUnitX(c, GetUnitX(t))
 call SetUnitY(c, GetUnitY(t))
 call SetUnitX(t, x)
 call SetUnitY(t, y)

#### Fulla

#### Fulla

 Level 12

JoinedAug 3, 2005Messages744

Resources: 2

#### Resources
Tutorials 2

- I intitially used jass tags, but it kept placing the entire code in 1 single line.


- I thought for beginners, would be best to keep it very basic/simple.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Yeah, diablo, that's what I meant.

And less variables would be simpler for beginners, I would imagine. However, that's your choice.

And yes, JASS tags are preferrable. try replacing [code][/code] with [code=jass][/code] and making no other changes, and seeing if it works. If not, you should probably report it in the bug report forum.

#### Fulla

#### Fulla

 Level 12

JoinedAug 3, 2005Messages744

Resources: 2

#### Resources
Tutorials 2

Ok updated, jass tags & added that bit at the end.


Is it possible to get the topic title changed


from: Jass Tut: (Level 1) - Nether Swap


to: Jass Tut: 1.01 - Nether Swap


thx

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

try double-clicking right beside the name, in the thread list. If a thing pops up where you can change it, change it. Otherwise, PM wolverabid or someone.

I know the double-click thing works on some vBulletin forums, but I'm not sure about THW

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

I renamed the thread for you Fulla: Jass Tut: 1.01 - Nether Swap


Before we can approve it you will have to review:


[self="http://hiveworkshop.com/forums/announcement.php?f=283"]General Tutorial Submitting Rules and Guidelines - Read Before Posting![/self]


and bring the tutorial up to those standards.

All image files must be attached to your post. We don't wish to be dependent on other sites (to avoid situations in which the server that hosts images fails, thus damaging the tutorial).

No signature files may be attached to tutorials. Instead, edit your tutorial to include your signature's contents (no attached or linked images are permitted).

Click to expand...


You should detach your siggy from the tut and use the [ATTACH][/ATTACH] tags instead of the [IMG][/IMG] tags.

#### paskovich

#### paskovich

 Level 11

JoinedJul 12, 2005Messages764

Resources: 3

#### Resources
Maps 1Tutorials 2

IMO, this tutorial is way too long for one spell only. Too detailed, and the main thing (triggering part) is pretty simple.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

It is supposed to be introducing basic concepts, after all

#### RedBlade

#### RedBlade

 Level 8

JoinedMay 13, 2007Messages392

Resources: 8

#### Resources
Models 2Skins 2Maps 2Tutorials 2

Wait... cant you just do it with GUI ?


Event - Unit castes and ability


Condition - Casting ability equal to Nether Swap


Action - Create Effect over casting unit (an effect)


 - Move instantly casting unit to position of (the unit targeted by ability)


 - Create Effect over unit targeted by ability (an effect)


I did it, i used it in a map of mine, i played it multiplayer and i had no leaks.

#### GST_Nemisis

#### GST_Nemisis

 Level 21

JoinedJan 5, 2005Messages3,515

Resources: 1

#### Resources
Tutorials 1

RedBlade said:

 Wait... cant you just do it with GUI?


Click to expand...


its not about the spell its about teaching someone to learn jass.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Also, it's a helluva lot harder, due to some *ahem* issues *ahem* with "Move Unit"

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Submissions

Wolverabid said:

 I renamed the thread for you Fulla: Jass Tut: 1.01 - Nether Swap

Click to expand...

[EDIT] ~ Tutorial approved and moved to JASS/AI Scripts Tutorials.


 Last edited: Sep 6, 2007

 You must log in or register to reply here.


### Similar threads

Locked

[JASS]
 Can't get this spell to run


ZIR

Feb 20, 2010

Triggers & Scripts

Replies4Views1K

Feb 20, 2010
ZIR

Locked

 any way to save "code"?


WereElf

Feb 18, 2016

World Editor Help Zone

Replies11Views3K

Feb 27, 2016
Anitarf

Locked

 This trigger seems to run twice per cast o.O


WereElf

Feb 17, 2016

Triggers & Scripts

Replies1Views926

Feb 17, 2016
PurgeandFire

Locked

 [Spell] Remade impale


shadowvzs

Jan 3, 2012

World Editor Help Zone

Replies3Views3K

Jan 4, 2012
-Kobas-

 [System] Dummy Cast


overcold_ice

Feb 18, 2012

Graveyard

Replies13Views2K

Feb 27, 2012
overcold_ice

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