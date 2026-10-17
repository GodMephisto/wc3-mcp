---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/projectile-missile-tutorial.254856/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Projectile Missile Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/projectile-missile-tutorial.254856/>
#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Tools: World Editor, Object Editor.

To start off you must understand what a projectile or a missile is, a missile or projectile is a ranged attack, some sort of object or anything is thrown, pushed or moved at a unit damaging the targeted unit from afar, thus it is called a ranged attack. This tutorial will cover everything you need to know about using and customizing missiles and projectiles for your ranged units.

As you may already know that every ranged unit has projectiles and missiles, some of them are different from each other. But the main thing is still the same in each and every one of them.

Definitions:

Projectile Impact - is a point where the missile will hit the targeted unit.

Ex: Rifleman shoots a peasant, this is the point the peasant will receive a physical missile at (the model of the missile.).

Projectile Launch - is a point where the unit will launch the missile from.

Ex: Rifleman shoots a peasant, in the launch point shall show up a bullet (missile/ projectile) that will head towards the peasant. This is the point where the missile will come from.

Z - (Height) Above/below the unit.

X - (Vertical) In front of/behind the unit.

Y - (Horizontal) Next to the unit.

Scale Projectiles - is an option to scale projectiles and missiles according to the unit's size and scale.

Ex: If you scale Archer to 2.00 her missile/ projectile will also be scaled to match her size.

You can also disable this option to keep the missile's original size.

Projectile Arc - is a value to change projectile's angle.

Projectile Art - is the model of the projectile/ missile this is how the projectile will show itself, it is a physical appearance of the projectile/ missile that will move towards the targeted unit.

Ex: Rifleman starts shooting the peasant, the projectile art otherwise called art will show up onto the target. Imagine it as a bullet.

Projectile Homing Enabled - is an option that enables homing on the target.

Ex: This will auto home-in the targeted unit's current location and will fire the missile at the target precisely. Otherwise if you disabled this option it will fire the missile at the old location of the unit. Another example would be a running peasant, if you disable this and attack the peasant, the missile will hit the old location of the peasant- when it was acquired in the first place.

Projectile Speed - is the speed of the missile/ projectile.

Ex: If you want the bullet or the missile to be fast you can set the speed of it in this field above- the same goes for making it slow.

Weapon Sound - this is the sound that the unit will play once he attacked / hit the targeted unit. Missiles usually don't use any weapon sound, it is mainly for a melee attack.

Weapon Type -

Artillery: missile will be fired up in the sky and will drop down to hit the targeted unit.

Ex: Meat Wagon, Catapult.

Artillery (Line): missile will be fired straight in the line to hit the targeted unit.

Ex: Glaive Thrower, Ballista.

Artillery attacks will also cause units to explode on death.

Missile: normal option for making a ranged attack.

Missile (Splash): missile will deal a bonus of splash AOE damage around the targeted unit.

Missile (Bounce): missile will bounce trough targeted nearby units hitting them like a chain.

Missile (Line): missile will get fired straight in a line.


 Last edited: Jul 17, 2016

#### Bannar

#### Bannar

 Level 26

JoinedMar 19, 2008Messages3,141

Resources: 23

#### Resources
Spells and Systems 5JASS 15Tutorials 1Wurst 2

Simple, but enough for user to get what are those stats about.

There are indeed many fields in Object Editor (e.g Units), mostly only few are used but it's good to know definition of all of them - we can take advantage of such knowledge when the time comes.

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

Drop the italics and use a regular font colour.

 Z - Height

X - Vertical

Y - Horizontal


Click to expand...

Doesn't properly describe it.

Z - above/below the unit

X - in front of/behind the unit

Y - next to the unit

Weapon Type:

Artillery - No, it will hit the ground where the unit was standing when the attack was launched. Artillery attacks will also cause units to explode on death.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

rulerofiron99 said:

 Drop the italics and use a regular font colour.

Doesn't properly describe it.

Z - above/below the unit

X - in front of/behind the unit

Y - next to the unit

Weapon Type:

Artillery - No, it will hit the ground where the unit was standing when the attack was launched. Artillery attacks will also cause units to explode on death.


Click to expand...

I wont change the style of the tutorial, It was made to match. Artillery attack missile will be thrown in the sky where it will hit the targeted unit and this is not a tutorial about "deaths". For example you can see how Meat Wagon works with Artillery attack type..

I will properly describe Z, X, Y definitions later.

#### Heinvers

#### Heinvers

 Level 53

JoinedMay 7, 2010Messages9,267

Resources: 101

#### Resources
Models 3Icons 65Packs 3Skins 27Templates 3

I was just going to say the same about the color.

Anyway this is quite useful.

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

No, the artillery attack usually won't hit a moving unit.


You seem to have a fundamental misunderstanding of what Artillery and Artillery (Line) are. They have zero to do with the motion of the projectile as you describe. It's the projectile arc, as you mention earlier, which determines whether the projectile will "be thrown in the sky" or be "fired in a straight line".


Artillery attacks are mechanically different.


Artillery: Enables Attack Ground ability, projectiles will land where the unit was when launched, targets will explode.


Artillery (line): As above, with the difference being that you can use Spill Range and Spill Radius to have the attack deal damage in a line behind the point of impact.

#### Bannar

#### Bannar

 Level 26

JoinedMar 19, 2008Messages3,141

Resources: 23

#### Resources
Spells and Systems 5JASS 15Tutorials 1Wurst 2

Example: in map Ninja vs Samurai this combination: Artillery attack and flag: Homing - false, is used to allow micro-based playstyle. Afterall, all units have 1 hp

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Bannar said:

 Example: in map Ninja vs Samurai this combination: Artillery attack and flag: Homing - false, is used to allow micro-based playstyle. Afterall, all units have 1 hp

Click to expand...


yeah, you can experiment with various setting and see what makes out.

#### Zed

#### Zed

 Level 7

JoinedFeb 22, 2014Messages280

Nice tut.

#### TheWretchedCreator

T

#### TheWretchedCreator

I am sorry, but I have to report some errors here - not concerning the artillery issue


Let's start with the more important one:


"Projectile Impact" defines NOT the projectile impact point of own missiles towards other targets, BUT defines where projectiles will hit the unit you set the value for.


Example: you edit an Rifleman, Projectile Impact - Z will determine on which height missiles thrown AT THE RIFLEMAN will hit him.


So, there is no option to customize where your edited unit shoots at ... kinda pitty though, sometimes that would come handy


The second error in the tutorial is more of systematic kind:


If you write an tut about projectiles missiles etc., it's not very senseful to use in examples exactly the one unit with a ranged attack that DOES NOT fire projectiles -- the riflemans weapon type is "Missile Instant", meaning he attacks, and immediately the target will be hit and take the damage, without any flying Thing - so there is no "bullet" in here on which could be applied the discussed values in this section :x

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

TheWretchedCreator said:

 I am sorry, but I have to report some errors here - not concerning the artillery issue


Let's start with the more important one:


"Projectile Impact" defines NOT the projectile impact point of own missiles towards other targets, BUT defines where projectiles will hit the unit you set the value for.


Example: you edit an Rifleman, Projectile Impact - Z will determine on which height missiles thrown AT THE RIFLEMAN will hit him.


So, there is no option to customize where your edited unit shoots at ... kinda pitty though, sometimes that would come handy


The second error in the tutorial is more of systematic kind:


If you write an tut about projectiles missiles etc., it's not very senseful to use in examples exactly the one unit with a ranged attack that DOES NOT fire projectiles -- the riflemans weapon type is "Missile Instant", meaning he attacks, and immediately the target will be hit and take the damage, without any flying Thing - so there is no "bullet" in here on which could be applied the discussed values in this section :x


Click to expand...


sorry, your point is irrelevant. Projectile Impact does what it says, it is the value point of impact (location) of the missile (where it will hit).

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

TheWretchedCreator said:

 So, there is no option to customize where your edited unit shoots at ... kinda pitty though, sometimes that would come handy

Click to expand...


You can easily edit which point the projectile is launched from by changing the Art - Projectile Launch values.


This should also be in this tutorial.(not saying it is)

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Approved.

P.S. Adding "tutorial" to the title isn't necessary if it is submitted to the tutorials section. :x

#### Legal_Ease

#### Legal_Ease

 Level 20

JoinedFeb 8, 2013Messages1,476

Resources: 33

#### Resources
Icons 13Packs 1Skins 3Maps 11Templates 1Tutorials 4

Can any of this be changed by trigger? For example, if I want to cast Slow Missile. Can we slow all the unit's projectile speed in an area for an amount of time? Or, if I want to change the unit's missile art upon an event? I guess Orbs would work for that. But, what about Arc? Can any of these be changed by trigger?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Legal_Ease said:

 Can any of this be changed by trigger? For example, if I want to cast Slow Missile. Can we slow all the unit's projectile speed in an area for an amount of time? Or, if I want to change the unit's missile art upon an event? I guess Orbs would work for that. But, what about Arc? Can any of these be changed by trigger?


Click to expand...

You can't change missile properties through triggers, so unfortunately no.

The most you can do is create hacky workarounds using orbs, attack 2, and morphing. The best solution (imo) is to trigger missiles and damage completely. That way you have full control over everything.

#### Seedro

#### Seedro

 Level 6

JoinedDec 4, 2019Messages49

Resources: 1

#### Resources
Tutorials 1

Z is height (positive is up)

Y is front to back (positive moves forward)

X is side to side (positive moves to the right)

#### Alry FireBlade

#### Alry FireBlade

 Level 4

JoinedDec 23, 2023Messages47

Can someone pls tell me, what number I need to make it look correct?

Currently, the Projectile starts and the point I framed red.

I changed so much on the projectile launch Numbers, but it seems to don't have an effect.

 You must log in or register to reply here.


### Similar threads

Locked

 "Future position" of target of missile system projectile


Herod

Jan 27, 2024

World Editor Help Zone

Replies3Views1K

Jan 30, 2024
Uncle

Locked

 missile (LINE)


operation3000

Nov 20, 2010

World Editor Help Zone

Replies2Views2K

Nov 21, 2010
Noobsmoke

Locked

 How can I store several values whitin a unit?


streetpunk

Jan 13, 2017

World Editor Help Zone

Replies3Views1K

Jan 13, 2017
Pyrogasm

Locked

 Harmless Projectile


HungryHusky

Jun 14, 2014

World Editor Help Zone

Replies2Views2K

Jun 14, 2014
Death Adder

Locked

 Custom Missile Systems and Performance


Spellbound

Aug 27, 2015

Triggers & Scripts

Replies7Views2K

Aug 28, 2015
Zwiebelchen

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