---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/an-unit-immune-to-stuns-and-certain-effects-using-pauseunit.331892/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# An Unit Immune to Stuns and Certain Effects Using Pauseunit
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/an-unit-immune-to-stuns-and-certain-effects-using-pauseunit.331892/>
#### AngelCraft

#### AngelCraft

 Level 8

JoinedMar 14, 2021Messages54

Resources: 2

#### Resources
Tutorials 2

 Well I did not find a similar topic so I post it here, sure that some will know this trick, but hey, it will be more useful with an explanation of its use. I found this trick looking for a way to perform quick attacks, I guess some people may find this trick useful.


Click to expand...

Previously, from what I knew in previous versions of Warcraft 3, before Blizzard gave the BlzPauseUnitEx feature, units could be immune to certain stuns using morph, mainly this is demonstrated in Dota 1 with the hero Alchemist and his transformation. But was there really just that way to make a unit avoid certain stun effects? This is apparently all due to the command "851973" that is given before sending a stun effect to a unit, so if someone interrupts this effect or the stun counter, it could make the unit unaffected by stuns.

The method

As I said before by interrupting a certain order you can avoid giving this effect of being immune to stuns in general. So in this case we could simply use PauseUnit (your_unit, true), which simply generates before pausing a unit the command "851973", which is given in the event EVENT_PLAYER_UNIT_ISSUED_POINT_ORDER or EVENT_UNIT_ISSUED_POINT_ORDER (a bit strange, no), well if we give it a command to a point or target other than the unit to be paused within the command event ID "851973", a glitch will be given. If you want to know about this error you can see this post made by Eikonium [Trigger] - Pause/Order Bug that with a map shows what happens with this glitch.

The idea is simply to do what Eikonium does but after the PauseUnit function we put a call IssueImmediateOrder (your_unit, "stop") so that it stops doing the last order given within the "851973" event, so we will have our Unstoppable and controllable unit.

 JASS:

scope TestUnstoppableUnit initializer Init

 globals
 private unit Test_Unstoppable = null
 private unit Test_AnotherUnit = null
 endglobals

 private function Unstoppable_Order takes nothing returns nothing
 if GetTriggerEventId() == EVENT_PLAYER_UNIT_ISSUED_POINT_ORDER and GetIssuedOrderId() == 851973 then
 if GetOrderedUnit() == Test_Unstoppable then
 call IssuePointOrder(GetOrderedUnit(),"move",GetUnitX(Test_AnotherUnit),GetUnitY(Test_AnotherUnit))
 endif
 endif
 endfunction

 private function Unstoppable_Actions takes nothing returns nothing
 call TriggerSleepAction(4)
 call PauseUnit(Test_Unstoppable,true)
 call IssueImmediateOrder(Test_Unstoppable,"stop")
 endfunction

 private function Init takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_ISSUED_POINT_ORDER)
 call TriggerAddAction(t, function Unstoppable_Order)
 set t = null

 set Test_Unstoppable = gg_unit_h000_0001
 set Test_AnotherUnit = gg_unit_hgry_0012
 call Unstoppable_Actions.execute()
 endfunction

endscope

As you can see in this code, simply Test_Unstoppable is the unit that will be paused, and it will send the order before that event occurs, within this, Test_Unstoppable will be ordered to move where this other unit is, in this case, Test_AnotherUnit. After performing these events, the unit will be unstoppable, but we must stop the given order in case the previous order is repeated due to pause using the code line IssueImmediateOrder (Test_Unstoppable, "stop") after PauseUnit and that would be everything.

Curiosities

- The unit this fault applies to is actually paused, and with the IsUnitPaused function it can be checked, even though it is paused, the unit can be controlled normally.

- To undo the effect of this state, it is simply to use the false PauseUnit function so that the unit stops being paused as in this example:

 JASS:

 private function Unstoppable_Actions takes nothing returns nothing
 call TriggerSleepAction(4)
 call PauseUnit(Test_Unstoppable,true)
 call IssueImmediateOrder(Test_Unstoppable,"stop")
 call TriggerSleepAction(10)
 call PauseUnit(Test_Unstoppable,false)
 endfunction

- Units that are in this state, for some reason will not be able to use non-directed abilities (such as berserk), they will only give orders, this could be good in the case of metamorphosis, they will not be able to activate On and Off of the orders of an ability of automatic launch.

- As you know, since there is no way to stun the unit in this state, nor with XD pause, this method could be used [vJASS] - [Snippet] SuspendUnit made by IcemanBo, which is also applicable to illusions.

- Beware that sometimes when detecting the command "851973" it can be generated by the ability to sleep (or others that I have not seen yet), which will also be taken within the event that captures the order, so it is good to have something that identifies to a unit that is about to enter this state, such as using an ability or variable.

- Also illusions can have this state.

- There are certain situations in which this state cannot be applied to a unit, and they are: If they cannot move, neither attack, nor cast spells.

A Glicht that spawns when a unit has Root.

- If the unit that uses this state and has root with the value to turn in false, this when giving the order to attack a unit will be able to move, even if the movement buttons are hidden, it will be able to follow the unit attacked. This is interesting since "smart" does not work but I find it useful in the field of units like Castle Fight to eliminate this command.

- If the unit is affected by abilities such as Cobweb or Trap, it will attack again without turning, only for the duration of the buff, and will not be able to move.

Credits

- To Eikonium for confirming the existence of this glitch, to Fenix140 for the method of illusions that can use certain abilities.

I attach the test map with what was explained above.

#### Attachments

An Unstoppable using Pause Library.w3x

 40.5 KB

 · Views: 108




 Last edited by a moderator: Jan 11, 2022

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I cannot reproduce this.

Melee Initialization
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
Unit - Pause Footman 0000 <gen>

Unit - Order Footman 0000 <gen> to Stop.

This makes the unit uncontrollable for me.

Casting stormbolt puts an infinite duration stun buff on the unit but since I cannot control the unit even before the spell I cannot see if the unit is controllable anyway.

So, I thought that maybe it is only immune while it moves. Or that maybe you can only move it with code.

So I added the following:

Untitled Trigger 001
Events
Unit - A unit Is issued an order with no target

Conditions

Actions
Game - Display to (All players) the text: (Name of (Triggering unit))

Unit - Order (Ordered unit) to Move To (Position of Footman 0001 <gen>)

The game displays "footman" but does not run anywhere.

Not sure I am stupid or it was patched but it does not work for me.

I then downloaded your test map but it contains a lot of other things that I don't think belongs in a test map.

#### AngelCraft

#### AngelCraft

 Level 8

JoinedMar 14, 2021Messages54

Resources: 2

#### Resources
Tutorials 2

Chaosy said:

 I then downloaded your test map but it contains a lot of other things that I don't think belongs in a test map.


Click to expand...

This one I didn't update and forgot to remove other padding triggers. But the library works, since by itself it usually happens that some stuns such as sleep or loop, collide with the command "GetIssuedOrderId () == 851973" since these abilities also produce these commands, so the library is there to fix these command crossovers, it's all in vjass code, not normal XD jass. I leave you another map with the sniper unit so that you can test the stun effect when the sorceress throws a stun at it. But as you know, by mixing in other stuns, you can make the unit pause until you remove the pause. This topic is primarily for testing this effect and for the community to see if there are any bugs. (Since I haven't found one about it yet e.e)

(Update the main post, so that the "Unstoppable Spell" library works, and the "TestUnstoppableUnit" deto is disabled)

#### Attachments

An Unstoppable using Pause Test.w3x

 37.1 KB

 · Views: 88




 Last edited: Jan 10, 2022

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Nice, it works now.

For those who wants to try this in GUI:

Melee Initialization
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
Set VariableSet u = Knight 0000 <gen>

Unit - Pause u

Unit - Order u to Stop.

Untitled Trigger 001
Events
Unit - A unit Is issued an order targeting a point

Conditions

Actions
Custom script: if GetIssuedOrderId() == 851973 then

Unit - Order (Triggering unit) to Move To (Center of (Playable map area))

Custom script: endif

It's not perfect, the stun effect is permanently above the head when stormbolting for example.

But it's cool and useful.

#### AngelCraft

#### AngelCraft

 Level 8

JoinedMar 14, 2021Messages54

Resources: 2

#### Resources
Tutorials 2

Chaosy said:

 It's not perfect, the stun effect is permanently above the head when stormbolting for example.


Click to expand...

Correct, I made some conditions in the library so that only units that are not paused and do not have an ability (which allows me to use it as a detector if I convert a unit to type Unstoppable) to add this effect. It's a matter of playing around with this glich a bit and you've got some great stuff. Also as I explained in the main post, the Unstoppable unit if it has the root ability can move if I do the attack action and it will follow this unit Lol, even though the movement command buttons are hidden, very curious XD.

Thanks for the GUI deto Chaosy.

Greetings...


 Last edited: Jan 11, 2022

 You must log in or register to reply here.


### Similar threads

Locked

 Pausing a unit


RobertMKD

Jun 16, 2016

World Editor Help Zone

Replies2Views3K

Jun 16, 2016
IcemanBo

Locked

 Pause stunned unit's animation


MN Lahmar

Jul 2, 2017

World Editor Help Zone

Replies2Views2K

Jul 2, 2017
A]mun

Locked

[Trigger]
 Pause Unit without Pause Command (aka: bizarre pause+stun bug)


Soul Reaver

Dec 28, 2010

Triggers & Scripts

Replies17Views8K

Dec 29, 2010
Soul Reaver

[Lua]
 PauseUnits


Wrda

Jun 29, 2022

Old Submissions

Replies10Views2K

Nov 12, 2022
Wrda

 [System] Stun


iAyanami

Jun 5, 2011

JASS Resources

Replies45Views19K

Jun 7, 2021
Daffa

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