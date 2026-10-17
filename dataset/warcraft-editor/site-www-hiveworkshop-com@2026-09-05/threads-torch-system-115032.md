---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/torch-system.115032/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Torch System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/torch-system.115032/>
#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

##
 Torch System

no 360 degree torches.

Note: this can/should only be used with the call SetDayNightModels("","") trigger.

##
 What You Need.

All you need is this - View attachment Light.mdx the light model. It and a few VERY simple triggers will be all that's used here.

So - why a torch system? Because, since when do torches light up the area ALL around you? They light it up in front of you.

##
 Preparation.

Make a unit called Light with the Light model, give it locust and invulnerability, set all its shadow values to 0.00.

Open the variable editor (the yellow X), then make a new variable (green X) a Unit variable, call it LightUnit. (This will be the light)

Make another new variable, Location/Point (I forgot the English version) and call it TempPoint. (This will be the position of the person holding the light)

Use a Map Initialization trigger, or wherever you want the "lights off" sign, and create 1 unit with the model of the Light (View attachment Light.mdx) at the position of your torch holder. Then set LightUnit = Last Created Unit.

Then use one trigger like this:

Custom Script: call SetDayNightModels("","")

Make another trigger that looks like this.

Events
Time - Every 0.02 seconds of game time

Conditions
(Life of (Your Unit)) Greater than or equal to 1.0

Actions
Set TempPoint = (Position of YourUnit)

Set TempPoint2 = TempPoint offset by 300.00 towards (Facing of Castaway 0001 <gen>) degrees)

Unit - Move LightUnit instantly to TempPoint2

Custom script: call RemoveLocation(udg_TempPoint)

Custom script: call RemoveLocation(udg_TempPoint2)

And that's it. Your results will be somewhat like this.

Terrain Tips to go with Dark Forests etc:

use big summer trees, tinted darkish

use 5000 end fogs, with a deep, deep purple (maybe 30,0,30 at max)

use a lot of shrubs and rocks, sized up

use the ashenvale tileset.

 Thank you for reading the tutorial. I hope it helped you


 Last edited: Jun 10, 2009

#### AirHead

#### AirHead

 Level 9

JoinedApr 5, 2008Messages441

Resources: 8

#### Resources
Icons 1Skins 5Maps 1Tutorials 1

This is brilliant! Just what I need to limit the sight of the units in a map I am making.

+rep for sure.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Nice, too bad there is not a jass version. Moving it every 0.01 seconds is pointless, as most displays update only at 60 FPS, so only really modern 100 Hz+ screens will benifit from it. Thus a time of 0.02 is more recommend for half the opperation number per second.


Remember however, that the WC3 lighting system bugs if there are too many light models near each other, resulting it aweful rectandle borders for the light to become visable.

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

I'll be sure to change the 0.01 to 0.02. Thanks

And yeah, the lighting does get annoying - but this is meant for maps with a lack of lighting.

#### AirHead

#### AirHead

 Level 9

JoinedApr 5, 2008Messages441

Resources: 8

#### Resources
Icons 1Skins 5Maps 1Tutorials 1

Oh, I just saw something! The condition of your units HP being over 1.00 would be better off as the boolean comparison of


'Unit - Unit is Alive' like this

Trigger
Events
Bla

Conditions
((YourUnit) is Alive) Equal to True

Actions
Bla

#### TheBlooddancer

#### TheBlooddancer

 Level 22

JoinedJun 24, 2008Messages3,041

Resources: 2

#### Resources
Maps 1Tutorials 1

Nice tutorial.

There is a system like this in the spells section.

You might wanna add a way to make the light to get blocked, by cliffs, and units.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

TheBlooddancer, such realisitc lighting has been drempt of for ages. However doing so efficently is impossiable. Even the Diablo 3 programmers were unable to do it, let alone within a WC3 map where you have no ability to modify the render chain.


The closest that could be done to that would be a system involving using unit shadows as the projected shadow and it works by calculating a shadow offset based on the direction of the light from the torch. Overall quite unrealistic, however it should be possiable as it is not that demanding and also adds more realisim.

#### TheBlooddancer

#### TheBlooddancer

 Level 22

JoinedJun 24, 2008Messages3,041

Resources: 2

#### Resources
Maps 1Tutorials 1

This guy did it pretty well then.

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

Added, airhead. Thanks.

Blooddancer, that's a sight cone. This is a torch system. Completely different. Here, it's for dark maps. His is just for seeing up front.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

His uses LOS not lighting so does not count.

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Nice tutorial.

#### HINDYhat

#### HINDYhat

 Level 20

JoinedApr 22, 2007Messages1,960

Resources: 4

#### Resources
Maps 3Spells 1

Unit - Move LightUnit instantly to (TempPoint offset by 300.00 towards (Facing of Castaway 0001 <gen>) degrees)
Leaks.

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

What leaks? The location? I removed the location afterwards.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

No you dont.


You remove the base location, not the polar projection one.

#### busterkomo

#### busterkomo

 Level 17

JoinedJun 17, 2007Messages1,433

Resources: 5

#### Resources
JASS 1Tutorials 4

Belgarath said:

 What leaks? The location? I removed the location afterwards.


Click to expand...

You need another temp point to deal with the point after it is offset. Point with polar offset creates another point from the source. So just modify it:

Events
Time - Every 0.02 seconds of game time

Conditions
(Life of (Your Unit)) Greater than or equal to 1.0

Actions
Set TempPoint = (Position of YourUnit)

Set TempPoint2 = TempPoint offset by 300.00 towards (Facing of Castaway 0001 <gen>) degrees)

Unit - Move LightUnit instantly to TempPoint2

Custom script: call RemoveLocation(udg_TempPoint)

Custom script: call RemoveLocation(udg_TempPoint2)

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

Okay, thanks.

EDIT: Woops fuck, deleted half of the tutorial on accident. I'll rewrite it later on >__>


 Last edited: Jan 30, 2009

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Approved

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

The final picture doesn't show, is it just me?

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Nah, it's a broken attachment. Send him a PM.

#### Rainbow Evil

#### Rainbow Evil

 Level 9

JoinedOct 2, 2008Messages406

Resources: 2

#### Resources
Maps 2

can I turn this trigger off,like to turn it on during the night and off during the day?

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

You can use a timer then. There's an event (I think) that fires when the time reaches a certain value. You can have three triggers, one would fire when the night begins, this is when you can start the timer (repeating, 0.02 seconds timeout), then the second would be when the night is over, that's when you pause the timer, and finally the third would be the actual trigger that makes a torch. The third trigger should run every time that timer expires (there's an event for that).

It's as simple as that (if I didn't screw up anywhere).

#### Zibu

Z

#### Zibu

Belgarath said:

 Because, since when do torches light up the area ALL around you?


Click to expand...


Ummmm since the beggining of time?!??

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

Easy way to do the cycling on and off is this:

Unbezeichneter Auslöser 001
Ereignisse
Spiel - The in-game time of day becomes Gleich 18.00

Spiel - The in-game time of day becomes Gleich 6.00

Bedingungen

Aktionen
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
'IF'-Bedingungen
(In-game time of day) GrÃ¶ÃŸer gleich (>=) 18.00

'THEN'-Aktionen
Trigger - Turn on Torch <gen>

'ELSE'-Aktionen
Trigger - Turn off Torch <gen>

You could also remove and add people from a unit/player group to toggle it at any time.

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

Thanks, this might be very useful for my map. Is there a way to increase or decrease the intensity of the light? Can it be done easily with Magos Model Editor? I'm not into that sort of things, so I don't want to screw something up.

 "do you know any gooder terrain than DOTA? IceFrog omg best terrain ever!!"


Click to expand...

Argh, dota is really lame, everything about it, map, heroes, loading screen... yet it's popular...

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

I suggest simply adding the Special Effect with the lightning model. It does the same and does not leak or anything.

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

Who did you answer to?

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Nobody. But instead using this system, you could also just attach the lightning model. Its much easier and better. (I mean this system is just a waste of timer and cache)

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

What lightning model?

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

I meant light. You know the UTM Map? It has light. Attaching the light model would do the same as this system.

#### Silvenon

#### Silvenon

 Level 13

JoinedNov 22, 2006Messages1,260

Resources: 1

#### Resources
Tutorials 1

Yeah I know the UTM Map. Well, that's interesting, I forgot about that. Thanks.

#### togera90

#### togera90

 Level 4

JoinedMay 20, 2009Messages96

how would you make a saw type thing


every time day comes you lose 2hp per second at night time you lose 1hp per .70 second's HOW WOULD YOU DO SOMETHIN LIKE THAT!

#### Just_Spectating

#### Just_Spectating

 Level 39

JoinedMay 16, 2007Messages7,272

Resources: 49

#### Resources
Models 13Icons 10Skins 26

ask in the right thread next time.


Btw,


i think 300 is a bit too foward, its not like there is absolutely no light behind you when you hold a torch.

#### Spet

#### Spet

 Level 9

JoinedJun 3, 2009Messages484

Very cool

#### Anachron

#### Anachron

 Level 23

JoinedSep 9, 2007Messages6,749

Resources: 109

#### Resources
Icons 80Packs 2Tools 1Maps 6Spells and Systems 12Ported Icons 2JASS 1Tutorials 5

Just use this if you need it

#### anarchianbedlam

#### anarchianbedlam

 Level 26

JoinedApr 13, 2008Messages2,046

Resources: 2

#### Resources
Icons 1Tutorials 1

lol for the record, torches DO spread light 360 degrees, so this is more of a flashlight system


but it looks like a very awesome and useful system, especially for like zombie survival maps or stealth maps.

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

some people call flashlights torches, i'm one of them

#### Angry Murloc

#### Angry Murloc

 Level 4

JoinedJun 29, 2009Messages28

Resources: 2

#### Resources
Maps 2

I don't see the point of adding the light through triggering if you can easily put it in an aura and add that to the unit.

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

this makes the light in front of the unit. like a flashlight.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Anachron said:

Just use this if you need it

Click to expand...


I guess he uses that same model its just that instead of attaching it to the unit he makes a dummy unit which uses the light model and moves it every second.

#### Lee_Go

#### Lee_Go

 Level 6

JoinedMay 1, 2009Messages156

Hi, the tutorials great btw, but how do you turn of the black mask? If it was day and I would want to take off that script that turns everything dark... how could I do this?

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

call SetDayNightModels ("Environment\DNC\DNCAshenvale\DNCAshenvaleTerrain\DNCAshenvaleTerrain.mdx","Environment\DNC\DNCAshenvale\DNCAshenvaleUnit\DNCAshenvaleUnit.mdx")

ashenvale may not be your base lighting though, but they all look pretty similar

#### drdgvhbh

#### drdgvhbh

 Level 2

JoinedMay 13, 2009Messages19

What's this for? Custom Script: call SetDayNightModels("","")

Also i think you shouldn't have light behind you because you don't have eyes in the back of your head. =.=

#### Cobbot

#### Cobbot

 Level 5

JoinedMar 15, 2008Messages35

Resources: 5

#### Resources
Maps 1Tutorials 4

+ rep

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

drdgvhbh said:

 What's this for? Custom Script: call SetDayNightModels("","")


Also i think you shouldn't have light behind you because you don't have eyes in the back of your head. =.=


Click to expand...


that's why the light's in front (sorry if you're agreeing with me)


the call setdaynightmodels("","") is to "turn off" the lights - as you see, in the screenshots, its extremely dark.


this is because wc3 uses lighting models of a sort - with that custom script, the models are gone, and custom models can be used properly to replace the lighting. thats why using "call SetDayNightModels ("Environment\DNC\DNCAshenvale\DNCAshenvaleTerrain\DNCAshenvaleTerrain.mdx","Environment\DNC\DNCAshe nvale\DNCAshenvaleUnit\DNCAshenvaleUnit.mdx")"


turns the light back on - under the Ashenvale lighting. you can check through the MPQ for different lighting settings, ie. village, underground etc.

#### orcyngiser

#### orcyngiser

 Level 3

JoinedApr 9, 2010Messages60

Resources: 1

#### Resources
Maps 1

thanks. really nice trigger.

#### ORLYowl

#### ORLYowl

 Level 9

JoinedJul 11, 2009Messages294

Resources: 4

#### Resources
Maps 4

Is there a way to make this MUI

and also make the light bigger?

I've tried scaling it, it just works a little.

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

mui involves local players and shit

never really figured it out all that well, you'd have to ask in the trigger section.

try raising the light to make it seem bigger.

#### phoenixfire

#### phoenixfire

 Level 12

JoinedJun 10, 2008Messages1,043

Can you make this effect only one a particular region?

#### WindowsVistaSucks

#### WindowsVistaSucks

 Level 16

JoinedJul 21, 2010Messages916

Resources: 4

#### Resources
Icons 3Tutorials 1

Nice, works fine 7.5/10

#### Yuil

#### Yuil

 Level 2

JoinedDec 1, 2007Messages11

Just wondering, is it possible to add a new light to a newly trained unit/ spell use?

I'm using a car with the spell "Turn on lights".

Can I add a light to that car, and still make the light move?

-Yuil

 You must log in or register to reply here.


### Similar threads

Locked

 Simple Spell Triggers


Lee_Go

Mar 21, 2010

Requests

Replies6Views2K

Mar 23, 2010
Pharaoh_

Locked

 Item Transfer Fixing


Spartipilo

Sep 6, 2011

Triggers & Scripts

Replies2Views1K

Sep 8, 2011
Spartipilo

Locked

 Torch Light Radius


Gold Bahamut

Oct 24, 2007

World Editor Help Zone

Replies5Views2K

Oct 24, 2007
Gold Bahamut

 Making a Grenade Type Spell


PrisonLove

Jul 15, 2008

Trigger (GUI) Editor Tutorials

Replies18Views19K

Mar 17, 2018
Bobastian

B

Locked

 Light sources


Paultaker

May 7, 2017

World Editor Help Zone

Replies3Views960

May 8, 2017
Paultaker

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