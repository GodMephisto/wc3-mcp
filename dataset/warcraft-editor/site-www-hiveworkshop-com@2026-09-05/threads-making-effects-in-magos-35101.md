---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-effects-in-magos.35101/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making Effects in Magos
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-effects-in-magos.35101/>
#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

 SFX in MAGOS


Here I will show you the basice knowledge of Magos and how to make special effects with it. Now, I expect you to know some basic modding knowledge, I won't talk to you about how blp's work, what alpha layers are and the like, so if you encounter something you don't know about, feel free to search for a definition/tutorial on it.

GETTING STARTED:


Get Magos' Model Editor.


The first time you open it, you should see a big gray screen, with a tab bar.


In this tutorial, we will make a completly new FX, with only particles, and no geosets. Then we'll move on to geosets and other things Magos can do.


Now, if there's one thing to keep in mind is:


Messing around with Magos, and generaly testing new things is perfect way to learn how it works. In fact, I suggest you always try out different combinations, so you get to know exactly what it can do.

SFX EXAMPLE 1: Random fiery explosion

Step 1:


Click on "file", then on "New"


First thing to understand is the frame system. Warcraft 3 uses a speed of 960 frames per second, don't know why.


Now go to Windows/Sequence Manager and right click in the blank screen, and then click "Create New". A new box will open, where you will see different things. Name the new sequence Birth.

NOTE: Remember, birth anims are single anims, which happen once and stop, like the Steam tank effect.


The first part (upper) is the extent section. Don't worry about it for now.


The second part (middle) has two empty fields: Start and End.


A sequence is basically animations. They start at a certain frame number and end at another. So for our sequence, we'll start it at "0", and end it at "2000" (which means roughly 2 seconds of animation). Then we're going to check "non-looping", to make sure our FX doesn't repeat uselessly. This tag shouldn't be checked for repeating anims like stand anims.


We're done with the sequence manager, now you can close it.


=== === === === === === === === === === === === === === ===

Step 2:


Next part is textures.


Open up the texture manager and the MPQ browser. We'll start with the MPQ browser:


Open the MPQ you want (go to file and click on the MPQ you want to open) and open up the "textures" folder. This is the folder with the most textures. Right-click one of the textures and select "use as texture". This makes sure the texture you're using is ingame, and you won't need to import anything.


However, if you need a custom texture, import it in the texture manager using "right-click/import texture".


Some very commonly used textures include (either in War3.mpq or War3x.mpq):


-cloud8x8.blp (and all other variations)


-Genericglow64.blp


-Blueglow.blp, redstar.blp and similar


-Shockwave10.blp and similar

Keep in mind both MPQ have variants of these textures, and some textures are exclusive to an MPQ.


Ok, so select Cloud8x8fire.blp, Cloud8x8.blp, shockwave10.blp and genericglow64.blp for our example.


=== === === === === === === === === === === === === === ===

Step 3:


Here starts the real editing....


Open up the animation controler. It's easy enough to work with, I doubt I need to explain it. Then open up the node manager, again, it should be a blank page. Right-click on that blank and select "Create Particle Emmiter 2". There are other types of nodes you can create, but we'll stick to this one right now.


So you click and BAM! You have loads of different settings. Stop hyperventilating and calm down, everything will make sense soon.


First thing we'll do is set the texture parameters (left part with the scroll down thingy and filter selector):


Set the scrolly thingy to Cloud8x8fire.blp and the filter to additive.

 Here's some useful info for you, so you can better understand how MDX's work. (Thanks to RDZ for the info, but I won't list it all here, its a bit complicated...)


Filters are ways of removing those annoying parts from a texture, like the black background, or the alpha layer. These filters also apply to matrial layers, something that doesn't really habve to to with this. Here's a list of the available filters and how (I think) they work:

-Transparent: This alpha's out the alpha layer. Actually, RDZ just explained what it does... Just remember it doesn't do any fading from opaque to transparent.

-Blend: Works like Transparent, but much more useful. This DOES have some good fading. Anyways, its good for smoke, and dust effects.

-Additive: This removes the black from textures, more or less (see RDZ's post for a full explanation). When used on a regular skin, it will appear ghosty, and does somewhat the same thing to the particles. Good for fire, water, lights and a lot of other things.

-Add Alpha: Works like Additive, however not that much. I haven't experienced much with it, so I'd say you might as well stick to additive.

-Modulate: Does the opposite of Additive, as in, removes the white, and makes the texture darker. This is good for shadowy glows, or other black textures. There aren't many textures that work with this however. Oh, and remember that modulate doesn't allow for a particle to fade out (in the segments part)...


Click to expand...

=== === === === === === === === === === === === === === === === === ===


Then we're going to look at the upper parts.


First things first:

-Visibility: controls whether the emmiter emits or not. Right now, click "animate" and you'll see something like:

 Code:

0:1

This means the emmitter will be emitting at frame 0, and won't stop until its told to. (remember, 1 means that the emmiter is there, 0 means it isn't)


So the question is, how do we stop it from emmiting? Lets say we want a short burst of particles:

 Code:

0:1
100:1

This means it will be only emmiting for 1/10 of a second (from frame 0 to 100), which means a short burst of said particles.


You can "animate" loads of other things this way, and not only with 1 and 0.

-Emmission rate: controls the speed at which particles are emmitted. Again this can be animated as well (doesn't have to) and if you want it to grow from a certain value to another value, simply set the setting at the bottom of the animate box to "linear" (bexier and hermite are other types, slightly more complicated).


For now we'll set it to 700, since we want it to release a lot of particles at once (it won't release 700 at once though, since the emmiting time is so short). If you have a constant emmitter, I suggest to go lower, since high values means that many particles at the same time, and some computers can't take that.

-Speed: Controls the speed at which particles travel through 3d space. Simple, no? Set this to 150.

-Variation: Controls the variation of the speed setting. I don't know exactly what the values stand for, but I think they're percentages (1 means it can vary from twice its normal speed, etc...) Experiment with it to get a hang of how it works. Set this to 0.

-Latitude: This is a bit... complicated. This controls whether particles travel directly upwards, in a cone or in a sphere. Directly upwards is the "0" value, than from there to a certain value its all about how wide the emmiting cone is, and at a certain value it becomes a whole sphere. Again, experimenting with it helps. Set this 125 (for a half-sphere).

-Width and Length: Controls the width and length of the emmitter. From there you can make emmitters go from large squares or long bands. Set this to 0 each.

!NOTE! Magos has these two fields mixed! Width ingame is actually length, and length ingame is actually width!

-Gravity: Gravity is the rate at which particles are pulled down. it works just like the real world (you do know what that is, right?). Since fire has a tendency to go up, lets se this to -35

=== === === === === === === === === === === === === === === === === ===


Now lets check out the "Flags" part (left, middle to bottom):

-Unshaded: If it is checked, particles will not be affected by ingame lighting.

-Unfogged: Tells whether fog affects the particles or no. Check this.

-Alpha Key: No idea. Don't check it.

-Line emmitter: Tells whether the particles move in a 3 directional way or 2 directional way.

-Sort Primitive Far Z's: Don't really know, supposedly it fixes the "which particle is in front" problem... Check it (or don't, doesn't matter much)

-Model space: Lets say we put this on a unit, well when its moving, this tells if the particles move with it, or stay behind.

-XY Quad: Allows for Flat particles. NOTE: Only works ingame if Line Emitter is checked as well.

-Squirt: I think this makes the particles face where its going.

-Head and Tail: Particles have two parts, the head and the tail. The head is just the texture, and the tail is a trail left behind the particles. The length is controled by the Tail length setting, and unfortunatly, tails aren't shown in the Model Editor, so the only way you can see the texture is with Head checked. Check Head only for our example. Actually, I hate to make the reference, but a particle is kinda like a spermatozoon.

=== === === === === === === === === === === === === === === === === ===


Now we'll take care of the bottom part...

-Rows and columns: If you had bothered opening cloud8x8fire.blp, you'd see that its composed of 64 little fire images disposed in a grid. So these two field are exactly for that: we tell the editor that the texture is divided in a grid of 8 rows and 8 columns. You'll soon understand how we can use that to our advantage.

-Life Span: Tells the editor how long we want the particle to live. Set this to 0.7.

-Tail length: This is the length of the tail.

-Priority Plane: Let say I have two emmiters, one of smoke and the other fire. I want the fire to always appear on top of the smoke, so I'll set Priority plane to higher than the other ammiters priority plane value. Set this to 1.

-Replaceable ID: Used for team colored particles. Set this to 0.

-Time: Notice the "segments" in the middle of the window. Every particle goes from segment 1 to segment 2 to segment 3 gradually. Time determines what percentage of the life span value is spent going from 1 to 2. So if set to 0.5, this means S1 will go to S2 at the same speed S2 will go to S3. If its 0.1, then S1 will go really rapidly to S2, but very slowly to S3 and etcetera. Set this to 0.2 since we want our fire to rapidly pop into existence.

=== === === === === === === === === === === === === === === === === ===


Lets look at the part right over the one we just took care of...


You'll notice 4 little boxes with three fields each. Remember that the 8X8 texture is like a grid? Well here we can choose whether we want to cycle through the mutiple images, or choose one in particular. I THINK the life span section controls the span of time between segment 1 and 2, while decay controls the span between S2 and S3.


So we want our fire to always be animated, but we don't have a tail so we don't need to modify it, set your fields to this:


=== === === === === === === === === === === === === === === === === ===


Alright next step is to edit the segments... Set the colors to anything you like, it really doesn't matter what you want.

NOTE: remember alpha is the transparency of the particle, meaning 0 means invisible particle, and 255 means completly visible.


But lets say we want our fire to suddenly grow into existence, but then fade away slowly.

IMPORTANT!


Two things can make a computer lag with particles, too many particles and giant particles. So always make sure your particles aren't too big, and that you don't have that many. Usually, 1000 particles is huge, even if they are small. Its important to have a low "lag factor" so your effect can be used more than once at a time without killing someone's computer.


Click to expand...


You should get something like this:


=== === === === === === === === === === === === === === === === === ===

WE'RE DONE!


Exit the node editor.


Ok, before viewing your model, do some things:


-Go to the tab "EDIT" on the toolbar, click on properties and go check the "debug messages" flag. This will show stats on the corner of your screen, where you can see the amount of particles, the zoom distance and etc... You can also swith on the "use ground texture" to see where the ground is on your model.


-Zoom out until you get to a distance of about 1600. If you are full screen, 1600 is the average ingame camera distance. This is to make sure your model is of a decent size.


-Go to the animation controller, and pick out your birth anim. Play it, enjoy your FX.


What? Don't find it too amazing? Don't worry, I plan to make new tutorials, explaining other Model editor functions, so you can know exactly how it works and become a l33t modeller.


 Last edited by a moderator: Dec 26, 2008

#### Deleted member 126647

D

#### Deleted member 126647

I look forward to this.

#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

Done, I guess.

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Making Effects in Magos looks really good WILL. This document is sure to help a great many aspiring modelers.


There a couple of minor spelling errors, I can fix them with your permission.


I would like Chriz and Werewulf to comment on the tutorial before its approved. You might consider sending them (or RDZ and TDR as well) a friendy PM to ask for their input: our best modelers should review this.

#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

Wolverabid said:

Making Effects in Magos looks really good WILL. This document is sure to help a great many aspiring modelers.


There a couple of minor spelling errors, I can fix them with your permission.


I would like Chriz and Werewulf to comment on the tutorial before its approved. You might consider sending them (or RDZ and TDR as well) a friendy PM to ask for their input: our best modelers should review this.


Click to expand...


Go ahead and fix them if you want.


I'll PM them right away.

#### Rao Dao Zao

#### Rao Dao Zao

 Level 24

JoinedMar 29, 2004Messages2,009

Resources: 5

#### Resources
Maps 4Tutorials 1

Dude, WC3 is on 1000 fps. Models are, at any rate.

You might want to mention about the 'filters' on the MPQ browser.

The MPQ browser has branches, not tabs. A minor point, but potentially confusing to nublets.

Filtermodes:

Transparent only has a setting of 0 or 1 (or about 0.99 or something, but it's negligible); basically, it won't do any nice fading work in-between -- things are there or not.

Blend is the same, except it goes all the way in-between (it sometimes has Z-order issues, but these shouldn't matter with particles).

Additive is exactly what it says on the tin -- it adds the pixel colours together. Black is, of course, 0, 0, 0, so 0 plus anything is anything, making black invisible. All other colours go closer to white, where things become 'fully' opaque.

Add Alpha is like additive, but it takes account of the alpha channel in the normal manner too (not sure about this one, though, never used it).

Modulate isn't the opposite of additive -- but as far as I know, it is similar. It's a multiplication rather than addition. Since all colours are stored between 0 and 1 rather than 0 and 255, a multiplication will take colours closer to 0 -- black.

Flags:

Unshaded means the particle will not be affected by in-game lighting -- it'll be the same exact colours as the texture (bar filtermodes etc). Useful for fire and such, or modulated emitters (unpleastant purple stains instead of black clouds being one thing that got me recently, when 'unshaded' was not checked).

Unfogged: perhaps note that it's the terrain fog, and not the fog of war.

SortPrimsFarZ: I believe this tag is suppose to sort the Z-order problems I mentioned earlier (the game being unable to determine what is in front of what), but in all my experience it has had no effect at all (it has been rumoured that the tag is in fact broken or unimplemented).

XYQuad does actually work; you can see it in the Spell Breaker's feedback emitter. However, when creating emitters from scratch, they fail. I am not entirely sure why; but butchering an existing XYQuad emitter rather than starting from zero makes them work perfectly.

Also note that 'head' must be checked for the particle to be visible at all in Magos'.

Talking about segments, you should perhaps note that the transparency doesn't fully work with some filtermodes; notably None, Transparent and Modulate. With modulate, however, there are some specific cloud textures with built-in fade-outs.

Hmm hmm hmm, hope that helps. Learned a few things myself reading it over, so you're doing good.

#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

Actually, it's 960 fps...


Magos says so in his tutorial for GMAX... And when you make a sequence of 2000 frames, in WE Previewer you can see it's lower than 2 seconds.


Anyways, thanks for clearing up some things. I updated the tutorial with some of that information. I didn't include it all, since new users will probably not understand that much, and besides they can go see your post.

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

I think it is dependant on exporter, i mean modeling program, because in ms3d, 1 keyframe == 0.041 seconds or something

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Quite useful, I approve.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Awesome. I created a pretty nice effect with this. IMO, it looks best with shockwave10.blp. Unless I did something wrong.

Anyways, sweet tutorial.

+Rep!

#### CoffinPuppet

#### CoffinPuppet

 Level 5

JoinedOct 7, 2007Messages61

Resources: 3

#### Resources
Models 3

PurgeandFire111 said:

 Awesome. I created a pretty nice effect with this. IMO, it looks best with shockwave10.blp. Unless I did something wrong.


Anyways, sweet tutorial.


+Rep!

Click to expand...


Jaha ^^


I just started Modelling, and This Tutorial realy helped me out


+Rep =)

#### NatDis

#### NatDis

 Level 8

JoinedJan 11, 2008Messages164

Resources: 5

#### Resources
Models 4Icons 1

You mentioned that the squirt flag makes particles face where they go but it is wrong. It doesn't have any effect in W3ME but in the game, it spawns the particles at the same time. Like the blast particle.

#### varsaigen

#### varsaigen

 Level 24

JoinedJan 25, 2008Messages7,782

Resources: 27

#### Resources
Models 24Skins 3

wow. this is geat. Just as I needed to update my model too! Thx!

#### Devine

#### Devine

 Level 30

JoinedSep 2, 2007Messages3,688

Resources: 10

#### Resources
Models 3Icons 6Tutorials 1

this is a nice, good, awesome good tutorial for model editing. Its an excelent addition to the hive's tutorials.

+rep for this one

nice

#### varsaigen

#### varsaigen

 Level 24

JoinedJan 25, 2008Messages7,782

Resources: 27

#### Resources
Models 24Skins 3

One question, how do i aply a blazing fire to a model and make dure it lasts until death?

#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

Have you tried making the duration longer?


Instead of:


0:1


100:1


use:


(frame number where it starts):1


(frame number where it ends):0

#### varsaigen

#### varsaigen

 Level 24

JoinedJan 25, 2008Messages7,782

Resources: 27

#### Resources
Models 24Skins 3

good point. No, i didn't. I'll see if that helps. Thanks!

#### Dark Hunter1357

#### Dark Hunter1357

 Level 20

JoinedFeb 20, 2007Messages843

Resources: 92

#### Resources
Models 63Icons 25Maps 2Ported Models 1Tutorials 1

I have a problem. I want a small fire to appear at the stand alternative animation only. And a big explosion in the death animation only. In mangos i made it, but when i import in game, both fires are always there.

Animations:

Stand anim (no fire effect):

From: 0

To: 333

Stand Alternative anim (only small fire effect):

From: 333

To: 1333

Death (only explosion effect):

From: 1666

To: 1800

Particle Emmiters:

Fire (the small fire effect):

0: 0

333: 1

1333: 0

Explosion:

0: 0

333: 0

1600: 1

1800: 0

I have uploaded the model, please replay.

#### Attachments

Dinomyte.mdx

 4.9 KB

 · Views: 344



#### WILL THE ALMIGHTY

#### WILL THE ALMIGHTY

 Level 33

JoinedSep 27, 2006Messages3,746

Resources: 56

#### Resources
Models 42Icons 8Packs 1Skins 1Maps 1Tutorials 3

Emmitters are very stupid. If you don't tell them to not emit during a certain anim, they'll emit anyway. You have to give it a value for every anim of the model. So if you have two anims:


Stand: 0 to 300


Death: 500 to 600


And an emmitter that you want to have appear during death, but not stand you need:


Visibility:


0:1


333:0


500:0


You have to tell the emmitter to stop emmitting at the death anim, else it'll keep going anyway.


ALSO, VERY IMPORTANT:


An emmitter cannot ALWAYS be active. It has to be told to stop at some time. What you have for your explosion is:


0: 0


333: 0


1600: 1


What you tell it to do is start emmitting at 1600, but never stop. It should have an additional line that says "1800:0" or something in the like.


Also, another problem might be that the frames aren't exactly the sequence's frames. try making it appear at exactly 1666.

#### Dark Hunter1357

#### Dark Hunter1357

 Level 20

JoinedFeb 20, 2007Messages843

Resources: 92

#### Resources
Models 63Icons 25Maps 2Ported Models 1Tutorials 1

I got a pm from DonDustin, who already told me this, but still tank you for replying.

#### zaniac199

Z

#### zaniac199

is it supposed to appear in the blank gray box at the main mangos thing? if it didnt' work for me. i'm kind of new at this so i have pretty much no idea what i'm doing but i did everything you said.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

openz0r

Status
 Not open for further replies.


### Similar threads

 Making Effects in Magos - Advanced Techniques


WILL THE ALMIGHTY

Feb 23, 2008

3D Modeling Tutorials

Replies14Views10K

Nov 2, 2008
3rgytr54

 Making Effects in Magos - Advanced techniques 2


WILL THE ALMIGHTY

May 3, 2008

3D Modeling Tutorials

Replies5Views8K

Oct 9, 2014
ChaoticHunter

 Model Editing


Fabioz

Jan 18, 2009

3D Modeling Tutorials

Replies45Views66K

Oct 10, 2021
KaiZen7th

 Scratch-Made Weapon Attachments with Particle Emitters


Fabioz

Apr 8, 2008

3D Modeling Tutorials
2

Replies52Views42K

Oct 13, 2010
Ikithe

 Classical Ribbons vs Particles Ribbons


Daelin

Nov 21, 2006

3D Modeling Tutorials

Replies0Views25K

Nov 21, 2006
Daelin

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