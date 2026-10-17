---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/beginners-guide-to-lightning-effects.220370/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Beginners Guide to Lightning Effects
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/beginners-guide-to-lightning-effects.220370/>
#### -Derp-

#### -Derp-

 Level 11

JoinedJun 28, 2011Messages540

Resources: 3

#### Resources
Spells 2Tutorials 1

Beginner's Guide to Lightning Effects

Table of Contents

1. Lightning Effects - What and Where

2. What is a lightning effect?​

3. Where are they?​
4. A Triggered Lightning Spell

5. Custom Script​

6. Moving Lightning​

7. Example Trigger​

8. Colorizing Lightning​
9. Miscellaneous Tips and Reminders

10. Lightning Code List​

11. Lightning Circles​

12. The extra effort​

13. A hearty reminder​
14. Special Thanks

Lightning Effects - What and Where

What is a lightning effect?

For those that have just started babbling with the editor - lightning effects are what you'd expect. Electrifying lines that shoot across the screen. Now, mind you - we're not talking about the doodads. We're talking about those snazzy blue and green ones that are created when you cast certain spells, like Chain Lightning. The intention of this tutorial is to show you how to properly use lightning effects.

That sounds nice - now where are they?

Blatantly put - in the editor. However, there's quite a few ways for getting lightning effects to show up in-game. You can do so by clicking on the 'Art - Lightning Effects' field in the Ability editor, as displayed below.

This is the simplest way of adding lightning effects and the abilities that can have them are:

• Chain Lightning

• Healing Wave

• Spirit Link

• Aerial Shackles

• Lightning Attack

• Mana Flare

• Mana Burn

• Finger of Death

• Forked Lightning

• Drain

Not a whole lot, hmm? Notably, you don't have a circular area of effect spell.

So let's make one.

A Triggered Lightning Spell

For those speed racers that have browsed through the trigger editor, you might've noticed a few GUI lightning triggers. If not, open the trigger editor and do so. Add a new action and search "lightning" and you'll come across these:

Lightning - Create a Chain Lightning - Primary lightning effect from source (Position of (Triggering unit)) to target (Center of (Playable map area))

Lightning - Move (Last created lightning effect) to source (Position of (Triggering unit)) and target (Center of (Playable map area))

Lightning - Change color of (Last created lightning effect) to (1.00 1.00 1.00) with 1.00 alpha

Lightning - Destroy (Last created lightning effect)

Hashtable - Save Handle Of(Last created lightning effect) as 0 of 0 in (Last created hashtable)
Three out of the five can and should be used. Two of those shouldn't, and there's a good reason as to why.

Lightning - Create a Chain Lightning - Primary lightning effect from source (Position of (Triggering unit)) to target (Center of (Playable map area))
This creates the lightning effect from two points - but you cannot apply height to the lightning. It'll always be on the ground and it looks bad. So what should you do? You should use the following custom script function that allows you to apply height.

Custom script: set udg_YOURVARIABLE = AddLightningEx(LIGHTNING TYPE, CHECKVISIBILITY, X1, Y1, Z1, X2, Y2, Z2)

Custom Script?!

Yes. It's a real scary thing at first, so we'll dissect this beast together.

set udg_YOURVARIABLE

You have to create a lightning variable to use this custom script. (Last created lightning effect) will NOT work. So hit that Ctrl+B, New Variable, and set the variable type to Lightning. Hit a-okay twice and put the name of your variable after the "udg_" part of the code.

AddLightningEx

This is the name of the lightning function in Jass. There are in fact two functions similar to this one - AddLightning() and AddLightningLoc(). Both do not use the height option (Z) and so they should never be used.

LIGHTNING TYPE

This is where you define what type of lightning you want. Note that this is a string, so you must put the type of lightning inside quotes. An example of this would be "FORK" - the lightning effect will show up in-game as the Forked Lightning's lightning. If you forget about the " " an error will show up.

CHECKVISIBILITY

This is where we decide if the lightning effects will show through the fog of war and black mask. Type in true and the lightning will not appear through the fog - type in false and it will.

X1, Y1, Z1

This is where we determine where the lightning will start at. X,Y,Z are real values. If you plan on using location variables to decide where the lightning effects will be you'll have to get the location's X,Y,Z values. This can be done by using the following:

 JASS:

GetLocationX(udg_YOURPOINT)
GetLocationY(udg_YOURPOINT)
GetLocationZ(udg_YOURPOINT)

You'll have to substitute the "YOURPOINT" with the location variable's name and then you'll be good to go.

X2, Y2, Z2

Like X1, Y1, and Z1 except this is where you want your lightning to end. Use the GetLocation trick with a second location variable.

What about moving the lightning?

The second code to never use is the following:

Lightning - Move (Last created lightning effect) to source (Position of (Triggering unit)) and target (Center of (Playable map area))
It too doesn't allow you to add height to the lightning. Yet again there's a work around - but don't worry it's nearly the same process as above. The code to use is:

Custom script: call MoveLightningEx(udg_YOURLIGHTNING, CHECKVISIBILITY, X1, Y1, Z1, X2, Y2, Z2)
Just substitute the "YOURLIGHTNING" with your lightning's variable, and for the other values (like X,Y,Z) do what you did above.

Action Shots!

Here's an example spell that uses the above techniques to create a MUI lightning spell:

Lightning AoE Spell
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Test Lightning Target Spell

Actions
-------- Remember that (Triggering Unit) = (Casting Unit) --------

Set Unit = (Triggering unit)

-------- Gets the unit's handle ID --------

Set Handle_Lightning = (Key (Triggering unit))

-------- Set up your location variables... --------

Set Point = (Position of Unit)

Set Point2 = (Target point of ability being cast)

-------- == Create the lightning effect == --------

-------- L2 is my lightning variable. --------

Custom script: set udg_L2 = AddLightningEx("FORK", true , GetLocationX(udg_Point), GetLocationY(udg_Point), GetLocationZ(udg_Point) + 70, GetLocationX(udg_Point2), GetLocationY(udg_Point2), GetLocationZ(udg_Point2) + 70)

-------- Save my lightning effect in a hashtable --------

Hashtable - Save Handle OfL2 as 1 of Handle_Lightning in Hash_Lightning

-------- Save the maximum duration of the lightning effect --------

Hashtable - Save 40 as 2 of Handle_Lightning in Hash_Lightning

-------- Save the location of the lightning effect --------

Hashtable - Save Handle OfPoint2 as 3 of Handle_Lightning in Hash_Lightning

-------- Adds the caster to a unit group. --------

Unit Group - Add Unit to Spell_Group

-------- Turns on the looping trigger --------

Trigger - Turn on Lightning Spell Loop <gen>

-------- Turns on the looping trigger --------

Set LightningSpell_TurnOff = (LightningSpell_TurnOff + 1)

-------- Deal damage and remove leaks --------

Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick every unit in (Units within 300.00 of Point2 matching ((Owner of (Matching unit)) Not equal to Player 1 (Red))) and do (Actions)
Loop - Actions
Unit - Cause Unit to damage (Picked unit), dealing 25.00 damage of attack type Spells and damage type Normal

Special Effect - Create a special effect attached to the origin of (Picked unit) using Abilities\Weapons\Bolt\BoltImpact.mdl

Special Effect - Destroy (Last created special effect)

Special Effect - Create a special effect at Point2 using Abilities\Spells\Human\ThunderClap\ThunderClapCaster.mdl

Special Effect - Destroy (Last created special effect)

Custom script: call RemoveLocation(udg_Point)

Lightning Spell Loop
Events
Time - Every 0.03 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in Spell_Group and do (Actions)
Loop - Actions
-------- Get the caster --------

Set Unit = (Picked unit)

-------- Set up the handle --------

Set Handle_Lightning = (Key (Picked unit))

-------- Get the position of the caster --------

Set Point = (Position of Unit)

-------- Load the target point --------

Set Point2 = (Load 3 of Handle_Lightning in Hash_Lightning)

-------- Load the lightning --------

Set L2 = (Load 1 of Handle_Lightning in Hash_Lightning)

-------- Move the lightning to where the caster is standing --------

Custom script: call MoveLightningEx(udg_L2, true , GetLocationX(udg_Point), GetLocationY(udg_Point), GetLocationZ(udg_Point) + 70, GetLocationX(udg_Point2), GetLocationY(udg_Point2), GetLocationZ(udg_Point2) + 70)

-------- Increase the timer, when the timer is greater than the maximum time the lightning is destroyed --------

Set Integer = (Load 5 of Handle_Lightning from Hash_Lightning)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Integer Greater than (Load 2 of Handle_Lightning from Hash_Lightning)

Then - Actions
-------- Destroy the lightning --------

Lightning - Destroy L2

-------- Turn off the spell, clear the hashtable --------

Unit Group - Remove Unit from Spell_Group

Set LightningSpell_TurnOff = (LightningSpell_TurnOff - 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
LightningSpell_TurnOff Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

Hashtable - Clear all child hashtables of child Handle_Lightning in Hash_Lightning

-------- Remove the target point --------

Custom script: call RemoveLocation(udg_Point2)

Else - Actions
Hashtable - Save (Integer + 1) as 5 of Handle_Lightning in Hash_Lightning

-------- Remove the leaks --------

Custom script: call RemoveLocation(udg_Point)

Colorizing Lightning

Want to customize your lightning effects? You can by using this wonderful code:

Lightning - Change color of (Last created lightning effect) to (1.00 1.00 1.00) with 1.00 alpha
It goes Red, Green, Blue and Alpha. The colors are fairly self-explanatory, whereas alpha isn't. Alpha is how transparent your lightning is - at 1.00 it is not transparent, at 0 it is completely invisible.

Here's what happened to the 'Example Spell' after changing the colors around:

Tips, Reminders, and Helpful Things

Lightning Code List

For your convenience I've compiled a list of all the lightning names, what they look like, and their code. Here you go:

Or alternatively, here's a text list:

"Chain Lightning Primary" - "CLPB"

"Chain Lightning Secondary" - "CLSB"

"Drain" - "DRAB"

"Drain Life" - "DRAL"

"Drain Mana" - "DRAM"

"Finger of Death" - "AFOD"

"Forked Lightning" - "FORK"

"Healing Wave Primary" - "HWPB"

"Healing Wave Secondary" - "HWSB"

"Lightning Attack" - "CHIM"

"Magic Leash" - "LEAS"

"Mana Burn" - "MBUR"

"Mana Flare" - "MFPB"

"Spirit Link" - "SPLK"

Lightning Circles

Circles can be made out of lightning effects - yet it's difficult and can prove laggy. If you absolutely have to have one, the trick to making one is using a loop function that creates a lightning effect. The loop makes a lightning, then creates another slightly turned, and another, until you've got a circle.

A good example of this being done is Omega Wave made by Maker.

The extra effort

When using lightning effects, you may want to add dummy units at the ends of the lightning effects and have the unit use a missile/special effect model. Apparently, it makes them look pretty. Spell moderators *may* push you to do it, so it's good practice to go ahead and add that extra special effect.

A good example of this is Chaos Singularity by Tank-Commander.

A hearty reminder

Entire spells can be made out of lightning effects. They often become the basis for advanced spells, but if you do plan on making a lightning spell make sure that it's destroyed properly. No one likes seeing a random lightning effect on the map that doesn't do anything. Make sure you use the destroy function. It's there for a reason.

Lightning - Destroy (Last created lightning effect)

[rainbow]Special Thanks[/rainbow]

I'd like to give a shout out to Radamantus for requesting me to make this, Jazztastic for his hashtable guide layout, and PurgeandFire111 for seeing if there already is a guide on lightning effects.

Back to top

 CHANGE LOG

v1.0 - Uploaded

v1.1 - Added Table of Contents


Click to expand...


 Last edited by a moderator: Feb 14, 2016

#### -Derp-

#### -Derp-

 Level 11

JoinedJun 28, 2011Messages540

Resources: 3

#### Resources
Spells 2Tutorials 1

Test map - https://www.hiveworkshop.com/forums/pastebin.php?id=rndwza

Radamantus wanted to physically upload it; I'll edit the tutorial and add a link to the map when he does so.


 Last edited: Jul 31, 2012

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

This is a very solid and worthwhile tutorial as far as I can see. I've had questions on custom lightnings before (especially the Z) and seeing this would've saved me from a long and confusing thread.


As far as the information goes, there is a thing you should link to, "how to add different lightning", the softmints tutorial about editing or adding new lightning effects. [self=https://www.hiveworkshop.com/forums/general-mapping-tutorials-278/how-customise-lightning-effects-203171/]link[/self]. I'm not seeing any triggering errors so that part is just peachy.


If you really wanted to make the tutorial slick you could add a table of contents with links to each head section. Imo it isn't large enough to necessitate the use of a table of contents but is up to you, after all.


I vote for approval.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great job. A very nice tutorial--it is detailed and the samples are nice + GUI friendly.

The layout is nice and that screenshot with the lightnings listed is very useful for anyone who wants to use lightning effects.

Approved.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Nice tutorial.


It would be cool if you added some colors and hidden tags, and a table of contents to make it seem more organized.


You can even make use of the goto/point tags to make your table of content link to parts of the page ^_^


(To figure out how it works, you can "Quote" one of my tutorials)

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

By the way,i want to add this one

#
 Lightning Heights(like a column)

How to make it?

This is simple

1st,you must set the variables needed for the column.

Ex.

[jass=]set udg_X = GetUnitX(udg_Unit) or set udg_X = GetLocationX(udg_Location)

set udg_Y = GetUnitY(udg_Unit) or set udg_Y = GetLocationY(udg_Location)[/code]

There it is,the "from" location,how about the heights?

[jass=]set udg_Z = GetUnitFlyHeight(udg_Unit) or set udg_GetLocationZ(udg_Location)[/code]

We will not use different variables due to that we have only one location and making a lightning column.

Here is the other height for the "to" location.

[jass=]set udg_Z_2 = GetUnitFlyHeight(udg_Unit) + udg_YourDesiredHeight

Or

set udg_Z_2 = GetLocationZ(udg_Location) + udg_YourDesiredHeight[/code]

Note:All of the variables are real variable.

Now creating the lightning.We will put all the variables in the script but they must be arranged in "from" to "to" Location.

[jass=]set udg_Lightning = AddLightningEx(udg_LightningModel,Visibility of the lightning(true or false),udg_X,udg_Y,udg_Z,udg_X,udg_Y,udg_Z_2)[/code]

Of course,after creating,we must save the lightning so that we can destroy it after the loop

Like this:

[trigger=]Hashtable - Save Handle OfLightning as 0 of Key in ExampleHash[/trigger]

For the indexers,you must add the id also

Like this:

[jass=]set udg_Lightning[udg_ID][/code]

To destroy:

[trigger=] Lightning - Destroy(Load 0 of Key in ExampleHash)[/trigger]

Or

[trigger=] Lightning - Destroy Lightning[ID][/trigger]

There you have it.

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

Something that bugs me...

I usually only use the GetLocationZ and I abandon the GetLocationX/Y

Okay I understand when you use the GetLocationZ, it's to calculate the terrain height and after that if we want to apply to a unit, we simply +50.00 or something, to suit that unit (so the Lightning does not appear at the target's feet).

But what is the use of setting GetLocationX/Y ?

I just used GetUnitX/Y to apply the Lightning since it doesn't have to do with unit's Height.

Custom script: set udg_L2 = AddLightningEx("FORK", true , GetLocationX(udg_Point), GetLocationY(udg_Point), GetLocationZ(udg_Point) + 70, GetLocationX(udg_Point2), GetLocationY(udg_Point2), GetLocationZ(udg_Point2) + 70)
Can be changed to;

Custom script: set x1 = GetUnitX(udg_Caster)

Custom script: set y1 = GetUnitY(udg_Caster)

Custom script: set x2 = GetUnitX(udg_Target)

Custom script: set y2 = GetUnitY(udg_Target)

Custom script: set udg_L2 = AddLightningEx("FORK", true , x1, y1, GetLocationZ(udg_Point) + 70, x2, y2, GetLocationZ(udg_Point2) + 70)
Coordinates > Location

But you can't avoid the GetLocationZ, you must use it in order to get current terrain height to apply Lightning's Height.

There is really no use of GetLocationX/Y because it's the same as GetUnitX/Y (again, Reals > Points)

Using GetUnitX/Y

Imagine if you have 2 Flying Units (both of them have 300 Flying Height), Unit A cast spell to Unit B, Lightning is created

Terrain Height is 0 for all area of map

x1 = x-axis coordinate

y1 = y-axis coordinate

z1 = 300

x2 = x-axis coordinate

y2 = y-axis coordinate

z2 = 300

Custom script: set udg_L2 = AddLightningEx("FORK", true , x1, y1, GetLocationZ(udg_Point) + GetUnitFlyHeight(udg_Caster), x2, y2, GetLocationZ(udg_Point2) GetUnitFlyHeight(udg_Target)
Now the problem is only the Current Terrain Height, now which value is responsible for vertical height ?

Yeah that's it, the Z value, meaning that it's a waste to use Point to check it by using GetLocationX/Y since we can use GetUnitX/Y.

Caster's Lightning

Lightning is created at X of X-axis

Lightning is created at Y of Y-axis

Lightning is created at 300 of Z-axis

Target's Lightning

Lightning is created at X of X-axis

Lightning is created at Y of Y-axis

Lightning is created at 300 of Z-axis

NOTE: Now maybe I'm wrong in this statement, because I don't fully experiment on it yet.

Also, you should at least mention what should the value "70" is there for, well, my choice is 50, but that's not gonna be a problem.

The problem is, people would be dazed with the "70" value, you should explain that, I mean it is for adjusting Height of Lightning for Ground Unit.

Speaking about Ground Unit.....

You know there is a difference between Ground Unit and Flying Unit ?

What you currently doing only supports Ground Unit (the addition of 70 Height to Ground Unit) but you're definitately not doing the Flying Unit.

Okay let's say we have Flying Unit, that has 300 Flying Height, if you add just 70 Height to the Lightning, it would be at 70, and now the range from Lightning to the Target has a gap (300 - 70 = 260) wheras it should be 0 gap.

How to solve this problem ?

First you differentiate either the unit is a Flying Unit or a Ground Unit (Boolean Condition)

If it is Ground Unit, you can leave it as it is (your current trigger)

But if it's a Flying Unit... (pretend z1 and z2 are local real variable)

(pretend z1 and z2 are local real variable)

Set z1 = GetLocationZ(udg_CasterLoc) + GetUnitFlyHeight(udg_Caster)

Set z2 = GetLocationZ(udg_TargetLoc) + GetUnitFlyHeight(udg_Target)
Do this when the Lightning created for the first time (Cast - AddLightningEx), and do this for every interval in looping trigger (Loop - MoveLightningEx).

By doing this, if the Flying Unit (Caster) or Flying Unit (Target) OR both of them changes Terrain Height (if your map has high ground and low ground), it will update correctly with the terrain height and UnitFlyHeight.

You should make this check for both Caster and Target because there will be 4 situations;

Ground to Ground (both will have a value of 70 (your height value))

Ground to Flying (only Ground unit will have a value of 70 height value, for the Flying, it has Current Terrain Height + FlyingHeight of Target)

Flying to Ground (just a reverse of Ground to Flying)

Flying to Flying (just a reverse of Ground to Ground)

Sorry for the messy comments though :/

#### -Derp-

#### -Derp-

 Level 11

JoinedJun 28, 2011Messages540

Resources: 3

#### Resources
Spells 2Tutorials 1

...and Advanced Lightning Tutorial, brought to you by defskull! Hah.

Ah, well, let's stretch a few braincells.

>I just used GetUnitX/Y to apply the Lightning since it doesn't have to do with unit's Height.

Yes, you can do that when the lightning goes from one unit to another.

Using locations is simply easier to explain and easier to use, and I gain very little efficiency if I used GetUnitX/Y as I'm going to set the location variable anyway. It really is just a preference.

>Coordinates > Location

Yes, in the grand scheme of things coordinates end up being more useful. However, for the sake of simplicity I tried to avoid using them the best I could. Coordinates tend to associate with jass and locations associate with GUI.

>"70"

The funny thing is that it doesn't even show up in the test map (Radamantus, upload pls) as that - it's actually a variable called "Height" and "Height2". I'll change that.

>Ground/Flying

The spell itself is a point target, regardless I'll try my best to update the guide with a way to support flying units.

----

Radmantus, I'll try to add a section in the "tips" part on Lightning Columns.

---

Thank you Jazztastic and Magtheridon, I'll add a table of contents soon and add a link to the other tutorial in the "tips" section.

And thank you PurgeandFire111 for approving it!


 Last edited: Aug 1, 2012

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

 >I just used GetUnitX/Y to apply the Lightning since it doesn't have to do with unit's Height.

Yes, you can do that when the lightning goes from one unit to another.

Using locations is simply easier to explain and easier to use, and I gain very little efficiency if I used GetUnitX/Y as I'm going to set the location variable anyway. It really is just a preference.


Click to expand...

But I think that in tutorials, we must make it as efficient as possible ?

Because it would affect many other people reading it.

GetUnitX/Y is not that bad, you can understand it do you, you naughty Derp >:

Seriously, just use GetUnitX/Y, since you already perform JASS function Add/MoveLightningEx, why not GetUnitX/Y, grrr.

 >Coordinates > Location

Yes, in the grand scheme of things coordinates end up being more useful. However, for the sake of simplicity I tried to avoid using them the best I could. Coordinates tend to associate with jass and locations associate with GUI.


Click to expand...

Just like above, since you already used JASS function, why not ?

 >Ground/Flying

The spell itself is a point target, regardless I'll try my best to update the guide with a way to support flying units.


Click to expand...

Ah I guess it doesn't hurt to add another Lightning Tutorial to use on Location, instead Unit - Unit, you can add Location - Location.

#### -Derp-

#### -Derp-

 Level 11

JoinedJun 28, 2011Messages540

Resources: 3

#### Resources
Spells 2Tutorials 1

The efficiency difference is ridiculously small ;_;. Since it appears I'm going to need another argument to waddle my way through this one, I'll state this:

GetLocationX/Y can be used without units, GetUnitX/Y requires the use of units.

If it pleases you I'll mention the GetUnitX/Y in the tips section, how about that?

EDIT: I will get around to updating this soon; life's been busy for me lately.

EDIT2: I've added the Table of Contents, I will be adding Radmantus' and defskull's stuff soon.


 Last edited: Aug 5, 2012

#### Rogue Kessler

#### Rogue Kessler

 Level 10

JoinedApr 4, 2010Messages288

Resources: 5

#### Resources
Maps 4Tutorials 1

This tells me exactly what I needed answered. Thank you so much for this. +Rep

#### Meatmuffin

#### Meatmuffin

 Level 11

JoinedJul 25, 2014Messages490

Resources: 10

#### Resources
Maps 2Spells and Systems 7Tutorials 1

Came across this by accident, though there's a typo:

 X2, Y2, Z2

Like X1, Y2, and Z2 except this is where you want your lightning to end. Use the GetLocation trick with a second location variable.


Click to expand...

X1, Y1 and Z1*

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Meatmuffin said:

 Came across this by accident, though there's a typo:


X1, Y1 and Z1*


Click to expand...


Fixed. Thanks!

#### Jay-B

#### Jay-B

 Level 5

JoinedAug 5, 2016Messages39

Resources: 4

#### Resources
Maps 1Spells and Systems 3

Can someone help me with this?

#### Attachments

Untitled.png

 129.7 KB

 · Views: 663



#### Neruvatar

#### Neruvatar

 Level 11

JoinedNov 25, 2014Messages526

Resources: 3

#### Resources
Maps 3

Got that error too

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

It's AddLightningEx(), not AddLightning().

 You must log in or register to reply here.


### Similar threads

Locked

 Chain Lightning Advanced Spell Request


PrinceYaser

Jan 27, 2017

Requests

Replies16Views3K

Jan 29, 2017
PrinceYaser

Locked

 Lightning effects


Jorko

May 14, 2010

Triggers & Scripts

Replies6Views2K

May 14, 2010
Axarion

N

Locked

 [GUI]Lightning Spell help


Nethrezim

Mar 2, 2010

Triggers & Scripts

Replies0Views949

Mar 2, 2010
Nethrezim

N

Locked

 How to delate lightning effects?


MateuszD

Sep 8, 2008

Triggers & Scripts

Replies7Views2K

Sep 8, 2008
MateuszD

N

Locked

[Trigger]
 Shock effect on damaged units


Nethrezim

Mar 2, 2010

Triggers & Scripts

Replies1Views840

Mar 10, 2010
karlcoxx

K

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