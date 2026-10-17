---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/the-wonderful-world-of-special-effect-modifications.319960/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# The Wonderful World of Special Effect Modifications
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/the-wonderful-world-of-special-effect-modifications.319960/>
#### Devalut

#### Devalut

 Level 25

JoinedFeb 9, 2009Messages1,814

Resources: 6

#### Resources
Spells and Systems 5Tutorials 1

My Intention with this tutorial is to inform users of the many useful ways they can modify special effects to their advantage, we will be exploring what, why, and whatever these modifications may pose to our merit.

Something of note:

@ThisPOT made a rough draft of a projectile system that uses only special effects & both my spell resources Bone Armor & Teeth use strictly special effects, if this long worded tutorial isn't your thing I would suggest taking a look at the triggers and fiddling with it yourself!

Special Effects - Creation

Creating A Special Effect at Point

Special Effect - Create a special effect at Point, using units\undead\Acolyte\Acolyte.mdl

The start of any special effect created by triggers is going to be "Create Special Effect"; however, only one will be able to be modified, that is "Special Effect - Create Special Effect at Point".

Creating A Special Effect at Attachment Point of a Unit

Special Effect - Create Special Effect Attached to the (string) of (X unit)

Creating a special effect on a unit does not (for some strange reason) seem to be modifiable, that is not to say it is completely useless... an example would be to use it in a "Hit" effect from various abilities or outside sources:

Special Effect - Create a special effect attached to the origin of (Point) using Abilities\Spells\Other\Monsoon\MonsoonBoltTarget.mdl

Click to expand...

Note that it is attached to a point on the unit not near or somewhere related to the unit, but on a specific model based "Attachment" point, the names of these points vary from things like "hand, left" or "Weapon" majority of Key Attachment points I use and should be lodged in your noggin should be:

 "origin" - Touch the space between the soles of your feet, that's your "origin" useful for, effects like the Paladin's Holy light spell or the Above mentioned Monsoon bolt use this, as other points may seem weird

"overhead" - self explanatory

 "chest"

"origin and overhead usually do not move with the unit during animations, unlike chest and all the other body parts" - Mythic

Click to expand...

Do note these are "generic" names that have been standardized by Blizzard, models will sometimes have some, or in the cases of missiles and special effect models, none at all.

Lastly on this subject, the spell bloodlust adds effects the the hands of the enchanted unit, if you go to the buff page on bloodlust you can see the attachment point names, refer to them if ever a special effect you create sits inappropriately, otherwise... experiment!

TRIGGERS

And what you will find in the trigger editor

Destroy

Special Effect - Destroy (Last Created Special Effect)
Leaks, read about them and why they can harm your map, every effect you create, you must also destroy at some point.

Memory Leaks

Click to expand...

Set Color (Player)

Special Effect - Set Color of (Last created special effect) to color of Player 1 (Red)
This Varies on certain models, units & specific projectiles (Hero Priestess Arrow Missile for one), essentially you can change the color of a model depending it has team colours built in.


Click to expand...

Set Color RGB

Special Effect - Set Color of (Last created special effect) to r: 255, g: 255, b: 255
Change the "Hue" of a special effect, note particle effects will not be affected by this, but there are a few particular models that can be changed to great effect.


Click to expand...

Alpha

Here's what 10 of the Hero Lich Basic Attacks look like with Alpha set to zero:

Special Effect - Set Alpha of (Last created special effect) to 255
Changes the transparency, make an effect dimmer by setting this lower

I've used this to change a shield effect to correlate to the health of the shield by using this:

ShieldDim = ShieldHP(50) / (ShieldMAX(100) / 255) = 127.5

By using the Shields current value "50" and the maximum health "100" we can get 255 levels of dimness!


Click to expand...

Scale

gif example

Special Effect - Set Scale of (Last created special effect) to 1.00
Simply makes the model bigger or smaller, with 1.00 being the default.


Click to expand...

Set Position (Coordinates)

Special Effect - Set Position of (Last created special effect) to x: 0.00, y: 0.00, z: 0.00

Special Effect - Set Position - X of (Last created special effect) to 0.00

Special Effect - Set Position - Y of (Last created special effect) to 0.00

Special Effect - Set Position - Z of (Last created special effect) to 0.00
This allows you to move the special effect based on coordinates, find the value "X of Point" or it's sister "Y of Point" Otherwise you can scroll down further and find "X of Special effect" & it's dear cousin "Y of Special Effect" and their rather dim brother "Z of Special Effect"


Click to expand...

Set Position (Point)

Special Effect - Set Position of (Last created special effect) to (Center of (Playable map area))
Much like the previous trigger, this simply allows you to set by variables or basic points


Click to expand...

Sub-Animation

Special Effect - Clear All Sub-Animation from Special Effect: (Last created special effect)

Special Effect - For Special Effect: (Last created special effect), Remove Sub-Animation: Victory]

Special Effect - For Special Effect: (Last created special effect), Add Sub-Animation: Victory
l'm not entirely sure what these do, if a kind fellow explains it I will add it here

New Explanation by _Guhun_ 2020/02/10!

Click to expand...

Play Special Effect ___ Animation

Special Effect - Play Special Effect: (Last created special effect), Animation: Birth
Units and majority of projectiles will have "Animations Names", much like attachment points, this points to an action the effect will play, ex. creating a skeleton and using this trigger to give it a birth animation, or causing a projectile to play it's "death" animation. NOTE: models have varying animation names, an easy way to see these would be to open world editor and select a model or unit:

Screenshot

Click to expand...

Height

Special Effect - Set Height of (Last created special effect) to: 0.00
I've had mixed Success with this one, probably best to keep to the aforementioned "Set Position Z"

but do experiment all the same.


Click to expand...

ORIENTATIONS (YAW, PITCH, & ROLL)

Gif Example

(the green flare is the default spirit touch with it's pitch & RGB hue altered)

Special Effect - Set Orientation of (Last created special effect) to yaw: 0.00, pitch: 0.00, roll: 0.00[/B]

Special Effect - Set Yaw of (Last created special effect) to: 0.00

Special Effect - Set Pitch of (Last created special effect) to: 0.00

Special Effect - Set Roll of (Last created special effect) to: 0.00
**WARNING**

Orientations do not use default 360.00 degrees, they instead use radians which can be used by selecting the real on the Special Effect - Set Orientation of (Last created special effect) to yaw: 0.00, pitch: 0.00, roll: 0.00 Use the function "Convert Degrees to Radians" and simply put your angle there.

Pic of the Function's location

With this you can change how the orientation or "facing" of a special effect will point, this is very helpful for a number of reasons but one in particular is taking a long column effect like "Resurrection target" and making it into a laser by changing the pitch 90 degrees.

Image example of the orientations

**SEE THE ATTACHED MAP FOR A CONFIGURABLE EXAMPLE**

Click to expand...

Time Scale

Special Effect - Set Time Scale of (Last created special effect) to 1.00
This Affects the "speed" of the special effect, 1.00 is the default for 100% normal speed.

Do note that some models will not show if they are either too fast or too slow, so... experiment!


Click to expand...

Time

Special Effect - Set Time of (Last created special effect) to 0.00
This, despite it's name is very different from the above modification, imagine a video, this is the timer of the video, you can control how the model will appear, be it half way through it's animation or reverse it all together with some periodic timers and some elbow grease.

A good use for this Example would be taking This HPBar model and setting it's time to 0.00 - 1.00 in representation of a value. Credit - @Tasyen

Click to expand...

#### Attachments

Orientation Example.w3x

 35.3 KB

 · Views: 183




 Last edited: Feb 10, 2020

#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


For the sub animations I believe it refers to the tiny animations attached to a main effect. Not sure how to better explain it. Look at flame strike to better understand what I am talking about. It consists of the actual flame strike, then 4 (I think it's 4) sub animations that appears around the center.

This is why in some special effects like the blue and black flame strike the sub animations hasn't been edited properly so they still appear in their original colors.

I might be wrong about all this though


 Last edited: Nov 8, 2019

#### Devalut

#### Devalut

 Level 25

JoinedFeb 9, 2009Messages1,814

Resources: 6

#### Resources
Spells and Systems 5Tutorials 1

Interesting, always wondered about those little pillars!

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Helpful and informative.

I'd like to note that origin and overhead usually do not move with the unit during animations, unlike chest and all the other body parts.

#### deepstrasz

#### deepstrasz

Map Reviewer

 Level 84

JoinedJun 4, 2009Messages22,555

Resources: 7

#### Resources
Maps 4Spells and Systems 2Sounds 1

Mythic said:

 I'd like to note that origin and overhead usually do not move with the unit during animations, unlike chest and all the other body parts.


Click to expand...

Hmm, does that work for floating text as well?

#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


deepstrasz said:

 Hmm, does that work for floating text as well?


Click to expand...

Floating text will be created at a point, then you can change its velocity and angle at which it should move. If you want floating text to follow a unit you have to move it with a periodic trigger.

#### deepstrasz

#### deepstrasz

Map Reviewer

 Level 84

JoinedJun 4, 2009Messages22,555

Resources: 7

#### Resources
Maps 4Spells and Systems 2Sounds 1

FeelsGoodMan said:

 Floating text will be created at a point, then you can change its velocity and angle at which it should move. If you want floating text to follow a unit you have to move it with a periodic trigger.


Click to expand...

I know that. I was asking if it moves automatically via creating it on a unit's chest.

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

@deepstrasz I'm not aware of any action that attaches text to a unit's reference points. FeelsGoodMan's method is the only way.

#### deepstrasz

#### deepstrasz

Map Reviewer

 Level 84

JoinedJun 4, 2009Messages22,555

Resources: 7

#### Resources
Maps 4Spells and Systems 2Sounds 1

Mythic said:

@deepstrasz I'm not aware of any action that attaches text to a unit's reference points. FeelsGoodMan's method is the only way.


Click to expand...

Ah crap, you're right. It can only be created above units. What lamery. It's basically as creating it on a point.

#### _Guhun_

#### _Guhun_

 Level 12

JoinedJun 12, 2010Messages413

Resources: 17

#### Resources
Models 1Maps 1Spells and Systems 9JASS 4Tutorials 2

Sub-animations are the same thing as animation tags for units, the flame strike smaller flames have nothing to do with it. For example, if you created an SFX with the Illidan model, you could turn it into his demon form by adding the Root/Alternate subanimation (not sure what it is called in GUI).

#### Devalut

#### Devalut

 Level 25

JoinedFeb 9, 2009Messages1,814

Resources: 6

#### Resources
Spells and Systems 5Tutorials 1

Excellent, lovely to have that mystery explained!

Thank you @_Guhun_!

I've updated it to include your post, as well as a little more on Time since I've been using it thanks to @Tasyen

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Simple and clean.

My only two nitpicks are that there should be a title at the top like "Introduction" or something like that.

And that sub titles should be indented for easier readability

Approved and sent to general mapping as I feel like this is overall useful even if it involves triggers.

You can poke me if you feel otherwise @Devalut

 You must log in or register to reply here.


### Similar threads

 Special Effects - Manipulation and Modification


N1ghthawk

May 30, 2010

Misc/General Mapping Tutorials

Replies12Views45K

May 20, 2024
Mekanical

Locked

 Remove Special Effect


Aeroblyctos

Apr 28, 2008

Map Development

Replies7Views3K

Apr 29, 2008
Zack1996

Locked

[Solved]
 Special Effect Height


BradPittlord

Jun 17, 2019

World Editor Help Zone

Replies6Views3K

Jun 19, 2019
BradPittlord

 Basic Knowledge in Regions


eubz

May 4, 2011

Misc/General Mapping Tutorials

Replies19Views25K

Jan 30, 2012
Ken-E

 Special Effects Memory Management


the.Axolotl

Mar 4, 2026

World Editor Help Zone

Replies2Views336

Mar 4, 2026
the.Axolotl

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top