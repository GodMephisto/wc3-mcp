---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/deflection-angle.194703/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Deflection Angle
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/deflection-angle.194703/>
#### emjlr3

#### emjlr3

 Level 12

JoinedMar 28, 2005Messages160

Resources: 7

#### Resources
Maps 4Spells and Systems 1Templates 1Tutorials 1

###
 Introduction

I was asked about stuff bouncing off other stuff a while back, and posted a script of which I couldn't recall the source, and which I wasn't sure how functioned.

To that end, I decided I wanted my own script for doing this, one that I understood, and could manipulate in the future to my needs.

###
 Description

 Code:

A missile (or some other assumingly circular object) is traveling along minding its own business.
From out of nowhere comes a building, or perhaps some other projectile, or maybe even a wall out ahead.
Preferably in a somewhat realistic fashion.
If such a collision were to take place, and the missile interacts in an [U]elastic[/U] fashion,
what should be its new and improved, direction of travel?

 JASS:

// Returns angle in radians
function GetDeflectionAngle takes unit missile, unit target returns real
 return 2.*Atan2(GetUnitY(target)-GetUnitY(missile),GetUnitX(target)-GetUnitX(missile))+bj_PI-GetUnitFacing(missile)*bj_DEGTORAD
endfunction

// Wrapper, returns angle in degrees
function GetDeflectionAngleDeg takes unit missile, unit target returns real
 return bj_RADTODEG*GetDeflectionAngle(missile,target)
endfunction

We are presumably colliding a missile with some static object. You can always replace the target with a set of x and y coordinates, or some other projectile.

It's important to note, however, that this is entirely dependant upon the direction the missile is traveling, which I assume is its facing. If this is not the case, update the script to account for this irregularity.

On a second and equally important note, the first function returns the angle in radians, whereas the wrapper returns the angle in degrees. Remember that, it's important!!!

Lastly, a word from the wise, for aesthetics sake, don't update your projectiles facing, its slow and ugly. Simply create a new one at the correct angle in its place.

Simple enough.

Feel free to stop reading here, and just start your collisions.

========================================

Then again, if you are feeling bold, try the rest on for size:

Given what we know about deflections, we can see that so long as our entity collides with some flat surface, given we know the angular relationship between the two (incident angle), we can determine the exiting (accidental) angle.

Well, we don't have a flat surface. We have a building, or something like it. Additionally, however, we have a center position, or an x and y coordinate system, for our target, the direction our missile is traveling (its facing), and the angle between it and the target its interacting with (which we can calculate using simple trigonometry).

We are therefore going to assume that at some distance out from our center point (collision radius), the missile is interacting with a flat surface. A silly assumption, I know, but it's the only one that works.

Ok, now the goal is to eventually determine our accidental angle (B in the picture), which is directly related to the incidental angle

Ergo, if we can calculate our incidental angle, we will be able to calculate our accidental angle. Easy enough, pretty much pie infact, with some simple arithmetic and geometry.

First, we need a flat surface. Well, in fact, we don't really need the surface, simply it's surface normal, which for our purposes is the angle between the missile and the building:

 JASS:

bj_RADTODEG*Atan2(GetUnitY(target)-GetUnitY(missile),GetUnitX(target)-GetUnitX(missile))

This provides us with the WC3 world angle between these two entities, which is the angle perpendicular to our interaction "surface".

Now the difference between the surface normal and the missile traveling angle is our incident angle:

 JASS:

angle-face

This can be negative, in fact, it needs to be, considering we're working on the positive direction (in other words, clockwise about our WC3 world), so don't fret.

Now that we have this difference, doubling it will give us the difference between the incident and accident angles, since they are both equally spaced apart from the surface normal:

 JASS:

2.*(angle-face)

Not so fast, yes this is the difference between the two, but we can't simply add this to our current direction angle to give us the new direction, that won't work at all.

We are bouncing off of and moving in the opposite direction from the target. Therefore our new angle is going to be in the opposite direction of the current:

 JASS:

face+180.

There are 360 degrees in a circle, +180 will give us the opposite direction.

Now that we have our opposite direction, adding the multiple of our surface/incident difference will give us the new angle of deflection:

 JASS:

face+180.+(2.*(angle-face))

Using simple algebra, this can be further optimized through expansion, into the following form:

 JASS:

face+180.+(2.*(angle-face))=face+180.+2.*angle-2.*face
=-face+180.+2.*angle //or,
=2.*angle+180.-face

***

Let's do a simple example on a sheet of paper, go ahead, take one out, I'll wait......

Our missile is traveling at 45 degrees, and when it interacts with the building they are at a 90-degree angle. Draw a compass, label the axis. Draw a dot somewhere, facing 45 degrees, and draw another at a 90-degree angle from the first.

 JASS:

ang=90.
face=45.

=face+180.+(2.*(angle-face))
=45.+180.+(2.*(90.-45.))
=225.+(2.*45.)
=225.+90.
=315. degrees

Draw a line directed outward from your first dot at a 315-degree angle. Looks pretty good, no? Well, thats the idea, I knew you'd see things my way.

Enjoy,

Hulk3


 Last edited by a moderator: Dec 3, 2021

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Seems more appropriate as a tutorial =).

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

too short for a jass resource... this would really be more useful as a tutorial...

#### BoneBreaker

#### BoneBreaker

 Level 10

JoinedJun 26, 2005Messages236

Your arithmetic can be simplified:

 JASS:

face+180.+(2.*(angle-face)) = face+180.+2.*angle - 2.*face
 = -face+180.+2.*angle //OR, Neater:
 = 2.*angle+180.-face

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

BoneBreaker said:

 Your arithmetic can be simplified:

 JASS:

face+180.+(2.*(angle-face)) = face+180.+2.*angle - 2.*face
 = -face+180.+2.*angle //OR, Neater:
 = 2.*angle+180.-face

Click to expand...


I have tested this in Excel and can confirm. In fact it can be written as:

 JASS:

2 * angle + 180 - face //The "." is not needed unless avoiding floor division

#### emjlr3

#### emjlr3

 Level 12

JoinedMar 28, 2005Messages160

Resources: 7

#### Resources
Maps 4Spells and Systems 1Templates 1Tutorials 1

nice pickup, expansion for the win. also allows the function to be inlined.

what does the . matter, other then defining the return type?

*Updated the thread topic post

fyi - I could return a value in radians, but then I'd just be swapping the RADTODEG for a DEGTORAD, and then would need to work with less intuitive units

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Instead of 180 it would be bj_PI. But that's the only change aside from also turning the "facing" angle into radians.

The . turns the value into a real, else it is treated as an integer. Integers function more quickly than reals, making it a bit more efficient. On top, it's easier to read, quicker to type and lovingly reduces the map file size by the most insignificant amount.

This will display 0.5:

 JASS:

BJDebugMsg(I2S(0.25 * 2))

#### emjlr3

#### emjlr3

 Level 12

JoinedMar 28, 2005Messages160

Resources: 7

#### Resources
Maps 4Spells and Systems 1Templates 1Tutorials 1

perhaps two versions.....

is a map really needed? - I think the purpose is clearly stated.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

I really think that this isn't suffice to be in the JASS resource as its more of a tutorial...

#### emjlr3

#### emjlr3

 Level 12

JoinedMar 28, 2005Messages160

Resources: 7

#### Resources
Maps 4Spells and Systems 1Templates 1Tutorials 1

isn't that where it is?

and its only "tutorialesque" because I actually explained how it works, and how I came to derive the function. Had I just posted the snippet it would simply be a JASS resource, or JASS snippet.

throw it in the tutorial section and it may get lost

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

@Adiktuz, I moved this to the tutorial submissions a long while ago.

@emjlr3 If you link to the tutorial from your signature instead of using a picture it will attract people to use it. Signatures, if made to look nice, are a good eye-catcher.

I suggest doing the primary function in radians and doing a wrapper function with degrees.

 JASS:

function GetDeflectionAngle takes unit missile, unit target returns real
 return 2 * Atan2(GetUnitY(target) - GetUnitY(missile), GetUnitX(target) - GetUnitX(missile)) + bj_PI - GetUnitFacing(missile) * bj_DEGTORAD
endfunction

function GetDeflectionAngleDeg takes unit missile, unit target returns real
 return bj_RADTODEG * GetDeflectionAngle(missile, target)
endfunction

#### emjlr3

#### emjlr3

 Level 12

JoinedMar 28, 2005Messages160

Resources: 7

#### Resources
Maps 4Spells and Systems 1Templates 1Tutorials 1

agreed and updated.

the only example I could find of a blizzard function that follows a similar nomenclature is GetRandomDirectionDeg. Therefore it looks as though we are continuing with the general trends in Blizzard's API.


 Last edited: May 5, 2011

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Yeah that was the idea. Even though Blizzard made weird structure with DialogCreate, CreateUnit, InitHashtable, Location, instead of a uniform CreateDialog, CreateUnit, CreateHashtable, CreateLocation.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Very useful. I will link to this whenever someone asks of this topic. (it is asked a lot)


~Approved. I just detached the signature from your main post.

#### wc3_n00b

#### wc3_n00b

 Level 4

JoinedFeb 18, 2009Messages18

Resources: 2

#### Resources
Spells 2

im new to jass and still learning about it... how do i get this system to work properly in my map?

I am assuming that it takes more than just miporting the code into my map and expect it to work, so do i need to create some sort of trigger and a custom script to get the code or ?

#### emjlr3

#### emjlr3

 Level 12

JoinedMar 28, 2005Messages160

Resources: 7

#### Resources
Maps 4Spells and Systems 1Templates 1Tutorials 1

no actually you just use it when you want something to bounce, pretty straight forward

 You must log in or register to reply here.


### Similar threads

Locked

 Need help with angle


chasin225

Nov 16, 2014

World Editor Help Zone

Replies3Views1K

Nov 17, 2014
chasin225

Locked

[JASS]
 Deflection Off Cliffs


emjlr3

Jan 24, 2012

Triggers & Scripts

Replies7Views2K

Feb 3, 2012
emjlr3

Locked

[vJASS]
 Angle Finding


Almia

Oct 29, 2012

Triggers & Scripts

Replies11Views2K

Oct 31, 2012
Almia

Locked

 Beware with cone spells!


WereElf

Jan 26, 2016

Advanced Scripting

Replies3Views2K

Jan 27, 2016
Wietlol

Locked

 A problem to do with Angles


CheatEnabled

Jun 25, 2010

World Editor Help Zone

Replies3Views1K

Jun 26, 2010
sephiroth1234

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