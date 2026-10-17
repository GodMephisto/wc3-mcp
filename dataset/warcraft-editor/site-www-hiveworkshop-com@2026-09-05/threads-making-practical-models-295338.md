---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-practical-models.295338/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making Practical Models
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-practical-models.295338/>
#### SNIper of DARKness

#### SNIper of DARKness

 Level 13

JoinedApr 9, 2013Messages764

Resources: 1

#### Resources
Tutorials 1

*This guide is aimed at experienced modellers, and will not contain information about the making of models but on things that aren't always addressed and may seem insignificant. Furthermore this guide will contain information only about "Building" type of models. And how mapmakers may view your models.

Building Models

Greetings fellow modellers and other kind of people.

You may think that making models is all about making them all look nice and not crash the world editor, but evidently as I put "you may think" in the beginning of it, it automatically implies that it is in fact incorrect.

I will now proceed to disprove this by showing you a lot of screenshots (preferably fancy) to help me push my agenda.

Model Angle

Model Angle is an important factor to consider when making building models.

Much like in photography, you will want to find a suitable angle which will help show off all the details and features the model possess.

What better example could be given than one made by Blizzard itself, The Human Town Hall.

Now let's say it was rotated and faced an angle of 360* ish, or maybe a dull 90* angle.

Now comparing them to the original, we may observe.

Looking at Variant A, we can see how many of the model features and details are lost, appearing quite bland for the eyes of the player.

Variant B manages to keep it's features and details such as the main entrance and the entirety of the clock tower. However when grouped up with more buildings we can see that it isn't looking quite right, and it won't assuming most other buildings will have the standard vanilla angle.

Examples with custom models

The models in question I would argue are of great quality, but some things are better than others, not for the reasons you may think though.

Let's compare 2 custom models, one made by @Mike and one made by @skrab .

As I have explained in the picture, having a 360* angle can damage your model, in this case specifically as you will not be able to see many of the small little props and other details you put in to it.

Therefore it is important that when making a model you think which will be the main dominant angle (could be any angle) of your building and which yields the best outcome.

By changing the angle of the archery range we can see many more of the props and features, giving it more life making it fit much better.

Sometimes, adjusting the angle may be able to fix pathing problems while also fixing the aesthetical:

Model by Mike

Model positioning

Another problem which was partially adressed above is model positioning, this may happen when the model is not properly centered or/and when the model expands from what appears to be a small point in to something very large resulting in some unpleasant looking pathing.

Models by @Mike

This is dominant in tiered buildings, the idea of having a small hut become with a few upgrade in to a grand palace is very good, but may not prove such in practice.

When making tiered buildings It is important to define the boundaries in which the model expands, generally one should try having the first tier represent the largest the model may get:

Model by Massivemaster

Other

These may not be as important as the above, but can still be quite troubling.

Models by MassiveMaster

This occurs due to Z fighting, for fragments from any 2 surfaces that are close enough to each other that the Z buffer cannot reliably distinguish which is in front. The distance one can consider close increases the further away from the camera the surfaces are due to floating point machine epsilon.

Model by Fingolfin

This usually happens when the model doesn't have a base, it is important for the model to have a base for the off chance of it being built on hilly terrain and giving the impression it is floating.

 This concludes that.


 Last edited: Dec 31, 2017

#### Mister_Haudrauf

#### Mister_Haudrauf

 Level 32

JoinedMar 14, 2014Messages1,212

Resources: 69

#### Resources
Models 62Icons 4Packs 3

The Pictures aren't visible.

It look's like this:

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Works for me.

#### Mister_Haudrauf

#### Mister_Haudrauf

 Level 32

JoinedMar 14, 2014Messages1,212

Resources: 69

#### Resources
Models 62Icons 4Packs 3

KILLCIDE said:

 Works for me.


Click to expand...

Now it works for me too.

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

SNIper of DARKness said:

 This occurs when the base is too high up, it can be fixed by lowering the said base.


Click to expand...

That occurs due to Z fighting. It occurs for fragments from any 2 surfaces that are close enough to each other that the Z buffer cannot reliably distinguish which is in front. The distance one can consider close increases the further away from the camera the surfaces are due to floating point machine epsilon.

#### SNIper of DARKness

#### SNIper of DARKness

 Level 13

JoinedApr 9, 2013Messages764

Resources: 1

#### Resources
Tutorials 1

Well that's another way of putting it, I think I may use that instead if you won't mind.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

SNIper of DARKness said:

 This usually happens when the model doesn't have a base, it is important for the model to have a base for the off chance of it being built on hilly terrain and giving the impression it is floating.

Click to expand...

Terrain-bent models analogous to ubersplats would be cool. Well in W3 only with precomputation I guess.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Neat tutorial. Could be useful as a reference/link when people orient things poorly.

I think it'd have a broader appeal by showing people how to easily adjust the rotation, position, etc. but it is fine as it is. Approved.

 You must log in or register to reply here.


### Similar threads

Locked

 How to Import Custom Models


PurgeandFire

Apr 6, 2007

3D Modeling Tutorials

Replies0Views114K

Apr 6, 2007
PurgeandFire

 How to Swap Reforged Model Parts


Retera

Oct 30, 2021

3D Modeling Tutorials

Replies20Views9K

Mar 12, 2023
FatCatTrap

 Collision Shapes, How to Make Your Model Selectable


Zerox

Jan 25, 2010

3D Modeling Tutorials

Replies19Views28K

May 20, 2024
Dark-Zalor

 How to edit models- a complete guide


Horn

Oct 4, 2004

3D Modeling Tutorials

Replies5Views49K

Dec 1, 2013
animation31

 Create Models only with free tools


D a w n

Nov 16, 2008

3D Modeling Tutorials
234

Replies174Views87K

Jun 10, 2014
clockwork2

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

3D Modeling Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top