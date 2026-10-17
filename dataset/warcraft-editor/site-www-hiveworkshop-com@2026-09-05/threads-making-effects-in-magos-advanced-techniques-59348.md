---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-effects-in-magos-advanced-techniques.59348/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making Effects in Magos Advanced Techniques
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-effects-in-magos-advanced-techniques.59348/>
#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

Before reading this tutorial, make sure to read and FULLY UNDERSTAND my previous tutorial. I'm serious here, you need the basics to understand, because I won't go over every option again.

_________________________________________________________________________________________________________________________________________________

 SFX in MAGOS - Advanced Techniques

--- Making a nuclear explosion ---

Now, I know what you're thinking, why the hell a nuclear explosion? Well, because it uses a lot of techniques that can be used elsewhere. This tutorial will cover rotating and moving bones and will also provide practice.

Stay with me, and try not to skip anything.

PART 1: Setting up the editor

First things first, let's set up Magos' editor.

-Open Magos (Duh) and click File--New

-Go to Edit--Properties and set the background to default, check "Debug Information", "Coordinate system axises" and "Bones". This is so we know the number of particles on screen, and so we can see our "bones".

 REMINDER:

Bones are like real "bones", they usually are linked to geometry and control their movements, just like a real skeleton. In this tutorial we don't have geometry, so the bones will simply define where our emmitters are.


Click to expand...

-Go to Windows--MPQ Browser to select the textures we will use. Right click on a texture and select "use as texture" (so we don't have to import it again in the editor). Go in the War3x.mpq (by clicking File--War3x.mpq), go to the "textures" folder and import the textures Dust3x.blp, Cloud8x8Fire.blp and GenericGlow64.blp.

-Go to Windows--Sequence editor to creat a new sequence. Right-click and select "Create Sequence". Now edit the new sequence so it looks like this.

From now on, we can use the "Animation Controller" (select it in the windows tab) to see our animations. Of course, right now, it won't show anything, because we haven't made any emmitters yet.

_________________________________________________________________________________________________________________________________________________

PART 2: Making the Mushroom Cloud

This section will explain to you the basics of bones in Effects.

Go to Windows--Node Manager

Right-click on the empty space and create a bone. You'll see a little teal bone appear with UNNAMED next to it. First we'll rename it: Right-click on it and select "Edit Node", then change it's name.

Don't close the window yet, we'll move directly to how to move bones.

Click the "Translation" button and a new window should open up. In this window you should see:

 Code:

0: { 0, 0, 0 }

The first 0 means represents the frame 0, which is the very first frame of our animation. (If you don't remember how the fram system works, please refer to my other tutorial).

The other 3 zeros represent coordinates. A position in 3d space is defined by 3 coordinates: X, Y and Z. X is front to back, Y is left to right and Z is up and down. Our three 0s actually meant that at frame "0", the bone's X, Y and Z values are all 0, so it should be right in the center of our model.

Leave the coordinates there, but add a few other lines:

 0: { 0, 0, 0 }

2000: { 0, 0, 500 }

20000: { 0, 0, 800 }


Click to expand...

This means that at frame 2000, the bone will have moved by 500 up, and at 20000 (the end of our animation, it will be at 800 height. Why we add this little line at the end is so the bone still moves a little, kinda like a real mushroom cloud, and also so the the bone will not move gently back to 0 height during the animation.

We're not done here. In the bottom right corner of the "Translation" window, you should see a small "Interpolation Type" bar. This is what defines HOW the bone will move. Set it to linear, which will simply give a smooth transition without slowing down or using curves (which is what the other two, hermite and Bezier do).

Now, we add emmitters. I'm going to assume you know how they work (since you obviously read my other tutorial, right?).

Right-click on your bone and create a particle emmitter 2 (it should be under the bone, which means it will move with it).

Make a simple "Fire" emmitter that should appear from frame 100 to 200, and the rest I'll just show you (since it's a bit long to make).

And we have the top of our mushroom cloud.

Now, we could add a smoke and a glow emmitter to it. So let's do it. The smoke emmitter should use the Dust3x.blp texture and a "blend" filter. It should also last for 15 seconds or so, plus it should appear gradually (so the time setting would be 0.5). Make sure to set the grid settings (bottom left and middle "Head" settings) to the right values. If you want to be a perfectionist, make the smoke emmit from a larger area and also appear by fading and not by growing. The Glow setting should be pretty much the same as fire, without the 8x8 grid and with less particles.

You should get something like these emmitters.

MAKE SURE TO CHECK THE "MODEL SPACE" TAG.

I'm going to end this tutorial here.

The nuke isn't completed, but you'll get to finish it in the next part. You can already use these techniques for a lot of other things, like making straight lines of particles (by setting the coordinates to something to other directions than up and unchecking "Model Space" (so the particles stay behind))


 Last edited by a moderator: Mar 21, 2008

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I am definitely going to read this. I'll hopefully find time to read it and edit my post with any comments.

#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

I think I'll stop this tutorial here, the next part is huge and more complicated (and also very useful), so I'll just name this tutorial "Part 1".


The next one should explain Bezier/Hermite Movement, how to use it to make perfec arcs and also how to make the fire/smoke column for the nuke.

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

I look forward to it =)


it was very useful for me because I didn´t know how to use those cloud textures :/

#### blazing-inferno

#### blazing-inferno

 Level 7

JoinedDec 29, 2006Messages365

Resources: 1

#### Resources
Tutorials 1

a screen shot on how the effect looks would be nice, so we know we did it right.

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Fixed the link to your "previous tutorial".


Anyway, I approve this. Looking forward to part 2 ^^

#### JetFangInferno

#### JetFangInferno

 Level 37

JoinedJul 22, 2004Messages1,002

Resources: 170

#### Resources
Models 170

I can't wait to learn bezier and hermite movement

#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

Until I completly understand that part of SFX, I won't make a tutorial about it. I simply can't explain it, because I know nothing about it.

#### JetFangInferno

#### JetFangInferno

 Level 37

JoinedJul 22, 2004Messages1,002

Resources: 170

#### Resources
Models 170

Hmm, I thought you were making a tutorial on it. Guess not.

#### ShadowMage016

S

#### ShadowMage016

This tutorial was very helpful to me as was the previous one. Just one problem. The flame and smoke and stuff don't rise with the bone. (I don't think they do anyway) Wats up?

#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

The Model Space tag doesn't work in Magos, only ingame. So the smoke won't rise in the moel viewer, but it will in WE.


Okay, I lied.


I am making a tutorial on circular translations.


 Last edited by a moderator: May 4, 2008

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Merged your posts. Double posting isn't very nice...

#### Zack1996

#### Zack1996

 Level 15

JoinedDec 18, 2007Messages1,098

Resources: 4

#### Resources
Models 2Spells and Systems 1Tutorials 1

This tutorial would prove useful (I haven't read it ).


And is your project (Goblin BattleFronts) still on? The thread looks dead!

#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

Yeah, it's pretty much dead. I might revive it once in a while, maybe the map will actually get finished.

#### 3rgytr54

#### 3rgytr54

 Level 11

JoinedApr 6, 2008Messages562

Resources: 12

#### Resources
Models 10Packs 1Tutorials 1

Great Tutorial, when I put this in game and play its birth animation it just lifts off as a big yelowish glow..... or is that meant to happen?

 You must log in or register to reply here.


### Similar threads

 Making Effects in Magos - Advanced techniques 2


WILL THE ALMIGHTY

May 3, 2008

3D Modeling Tutorials

Replies5Views8K

Oct 9, 2014
ChaoticHunter

 Making a dissipate animation using Magos' War3ModelEditor


Callahan

Apr 15, 2007

3D Modeling Tutorials

Replies19Views22K

Apr 23, 2019
strongwill

Locked

 Making Effects in Magos


WILL THE ALMIGHTY

Jun 10, 2007

3D Modeling Tutorials

Replies21Views26K

Jul 1, 2009
Pyritie

 In-depth Animation Tutorial [3ds max]


BlinkBoy

Apr 11, 2009

3D Modeling Tutorials
23

Replies123Views153K

Feb 8, 2017
Kazeon

 Mdlvis: model edition/creation, animation and UV Mapping


jk2pach

Sep 20, 2010

3D Modeling Tutorials

Replies13Views88K

Mar 2, 2020
wisanggeni

W

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