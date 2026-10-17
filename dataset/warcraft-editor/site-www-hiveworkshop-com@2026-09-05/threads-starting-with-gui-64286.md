---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/starting-with-gui.64286/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Starting with GUI
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/starting-with-gui.64286/>
#### Lord_of_sausage

#### Lord_of_sausage

 Level 15

JoinedAug 18, 2007Messages1,390

Resources: 6

#### Resources
Tools 1Maps 4Tutorials 1

Welcome


Hi all, i saw alot of people asking for this, so i'd made a tutorial of it.

Getting Started


Triggers looks advanced on first look, ill remmember my first try, wich spawned a peon every second on the map, where it should only spawn one


But away from that, i'll introduce you.


Each trigger is divided into 3 parts, Events, Conditions and Actions


Events is WHEN the trigger is excecuted, when the trigger happens


Conditions is what NEEDs to be fullfilled before the actions takes place


Actions is WHAT happens


All Actions, Events and Conditions does excactly what they say, like the action called Unit - Kill, kills a unit


That makes sence right?


I will guide you through a simple spawn trigger, then youre on your own!


I want this trigger, to create 3 Peasants at the middle of my map, when a Paladin enters a "region"

Makeing the "region"


First, ill make a region:


1: Click "R" when you're watching your terrain.


2: This will popup:


3: Click this button wich means "Create Region"


4: Pull out an area on the terrain, like when selecting multiple units ingame.


5: Go rename the region by Right clicking the region name and choose "Rename Region" or "Region Properties"


Now type in Spawn 3 peasants


Now, we got a Region called "Spawn 3 peasants". This is the region we want to have the paladin enter, for spawning some peasants.


We want to make the trigger now, so open the trigger editor, looking like an "A"


Now click the piece of paper, wich got a BOLD frame.

Event, Condition and Action


Now lets make the trigger:


1: Click the Event icon, it looks like a red flag.


2: Find the text called Unit - Unit enters Region


3: You see that Region is red, thats because "Region" is invalid. Click the red text, and under "Varibles" you'll see the Spawn 3 Peasants region!


4: Click the Spawn 3 peasants region, and press "OK"


5: Click "OK" again, as our Event is done! HOORAY!


Now, we need a condition, to tell that we only want it to spawn when its a Paladin who enters.


So, Click the Condition icon, looking like a piece of paper with a green arrow on.


1: Find the text called Unit - type comparison


2: Click the text "Footman" and click the icon of the paladin


3: Press "OK" twice.


(The text wich says "Triggering Unit" means that its the unit were talking about in the event, we could change it to "Entering unit", but right here its unneseary. If we would like to make a trigger wich created a unit for a plyaer, when his unit dies, and kills the unit who attacked, we would have to use "Killing unit" and "Dying Unit" as the event only talks about one of them.)


Now lets make our Action. Press the Action icon, it looks like one of thoose funny movie things.


1: Find the text Unit - Create units facing angle


2: Footman is'ent the unit we want, so click the text, and then the icon of a peasant.


3 now, we would like 3, not one, so click the text "1" and type in "3" instead. Notice that thoose 3 is spawned at the same point!


4: Press ok twice.

DONE!


Voila, were done!


it should look like this:


When a paladin enters our region, the map creates a unit at the "center of playable map area" wich is the middle of our map.


Thats all from me, if you wonna test the trigger, place a paladin using the Unitpalette, wich can be assced by pressing "U" when looking at the terrain


or by clicking the drop menu at the region pallete and click "Unit".


//Lord_of_Sausage


 Last edited: Mar 21, 2008

#### redmarine

#### redmarine

 Level 19

JoinedSep 4, 2007Messages2,826

Resources: 1

#### Resources
Maps 1

Shouldn't you replace triggering unit with entering unit to make it easier for people to understand? Maybe it is just me.

#### Lord_of_sausage

#### Lord_of_sausage

 Level 15

JoinedAug 18, 2007Messages1,390

Resources: 6

#### Resources
Tools 1Maps 4Tutorials 1

redmarine said:

 Shouldn't you replace triggering unit with entering unit to make it easier for people to understand? Maybe it is just me.


Click to expand...


hmm, you can be right, i'll go edit it abit.

#### GhostWolf

#### GhostWolf

 Level 29

JoinedJul 29, 2007Messages5,174

Resources: 2

#### Resources
Tools 1Tutorials 1

Looks good and usefull. Maybe add a bit of colors and some more examples.


Would also be nice if you add the pic of the trigger editor button.


Beside that, please add a section on variables

#### Lord_of_sausage

#### Lord_of_sausage

 Level 15

JoinedAug 18, 2007Messages1,390

Resources: 6

#### Resources
Tools 1Maps 4Tutorials 1

GhostWolf said:

 Looks good and usefull. Maybe add a bit of colors and some more examples.


Would also be nice if you add the pic of the trigger editor button.


Beside that, please add a section on variables

Click to expand...


Variables? aint they a little more advanced? Or is it just me who found out what i could with them after i made maps for a half year?


well, i might come up with that, but not yet, i will also add screenies of the different buttons.


Also thanks for rep  (just noticed ;D)

#### underscore

#### underscore

 Level 6

JoinedMar 17, 2008Messages150

Resources: 6

#### Resources
Skins 1Tutorials 5

GhostWolf is right

Lord_of_sausage said:

 Variables? aint they a little more advanced? Or is it just me who found out what i could with them after i made maps for a half year?


well, i might come up with that, but not yet, i will also add screenies of the different buttons.


Also thanks for rep  (just noticed ;D)


Click to expand...


Yep, Variables are just easy when you understand them a bit. =0

#### Lord_of_sausage

#### Lord_of_sausage

 Level 15

JoinedAug 18, 2007Messages1,390

Resources: 6

#### Resources
Tools 1Maps 4Tutorials 1

underscore said:

 Yep, Variables are just easy when you understand them a bit. =0


Click to expand...


Yea, i were just slow

#### Narasha

#### Narasha

 Level 2

JoinedFeb 25, 2016Messages20

redmarine said:

 Shouldn't you replace triggering unit with entering unit to make it easier for people to understand? Maybe it is just me.


Click to expand...

I think using Triggering Unit is much faster than the Entering Unit

#### Ragnar9

#### Ragnar9

 Level 5

JoinedJan 15, 2018Messages130

This is great, exactly the kind of thing im looking for as a beginner.

Could you do one where unit enters region and buildings spawn?

- ive seen it before where they come out of nowhere, and also where they are invulnerable and then change when the unit enters or when the research is completed.

 You must log in or register to reply here.


### Similar threads

 Basic Triggering


Ralle

Jan 3, 2006

Trigger (GUI) Editor Tutorials

Replies4Views39K

Jul 7, 2016
billel

 How to begin in triggering, for the complete noob


hawk_767

Jan 24, 2009

Trigger (GUI) Editor Tutorials

Replies23Views32K

Dec 28, 2014
Madcapercaillie

 Chaosy's GUI tutorial for beginners


Chaosy

Apr 6, 2012

Trigger (GUI) Editor Tutorials

Replies9Views8K

Dec 25, 2016
Chaosy

 Reviving Heroes at a Changeable Region


dOT145

Mar 23, 2010

Trigger (GUI) Editor Tutorials

Replies12Views10K

Apr 30, 2010
PurgeandFire

 Making a full TD [Reviewed: Ralle]


Ostaph13

Jun 27, 2008

Trigger (GUI) Editor Tutorials
2

Replies50Views21K

Jan 20, 2011
Orcnet

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