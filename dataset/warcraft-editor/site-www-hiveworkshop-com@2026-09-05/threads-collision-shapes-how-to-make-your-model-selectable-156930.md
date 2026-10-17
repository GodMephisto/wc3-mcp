---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/collision-shapes-how-to-make-your-model-selectable.156930/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Collision Shapes How to Make Your Model Selectable
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/collision-shapes-how-to-make-your-model-selectable.156930/>
#### Zerox

#### Zerox

 Level 20

JoinedJun 6, 2008Messages987

Resources: 27

#### Resources
Models 21Icons 2Skins 1Ported Models 2Tutorials 1

How to Make Proper Collision Shapes/How to Make Your Model Easily Selectable:

1. First open your model with War3ModelEditor (Magos).

2. Then go to Edit>Properties.

This will open up a window which looks like this:

3. Check the Collision Shapes Box under the display category on the left side.

4. Now press the Okay button. You may or may not see anything different. If you don't see anything then move onto the next step.

5. Go to Windows>Node Manager.

6. Right click at the bottom and choose Create Collision Shape.

7.A new blue box will appear at the bottom. Make sure it's all the way over to the left so it isn't attached to anything. Double click it or right click it and choose Edit.

A window will appear like this:

8. Choose Sphere as we'll start out with that as they are easier to do and more widely used.

9. For starters set your Bonus Radius to 40 and the Z coordinate to 40 as well. Typically you can leave X and Y alone for most models unless they aren't centered properly.

10. Press Okay and then go back to your main model window. You should now see a blue sphere like this:

Now most likely your sphere will not cover your entire model. This is usual and varies between model. That blue sphere is the area that will be selectable on your model. We want the entire model or most of the model to be covered so your unit is easily selectable.

11. So to finish up we go back to Node Manager and you can either expand your Bonus Radius and set the Z coordinate to the same amount as your Bonus Radius, (Keep in mind having the same Z coordinate as the Bonus Radius isn't required but I find it usually keeps the sphere positioned well.) Or you can create a second sphere. I'm going to create a second sphere for this model because he is somewhat tall and I don't want too much space to be selectable around him.

12. Just experiment to try to make your sphere's cover the model. For this specific model I found that spheres of Bonus Radius 60 worked best. So I created a second one and put it's Z coordinate at 140 like so:

My model now looks like this:

See how the spheres expand out beyond the model a bit? This is totally okay because your model will be moving around and will at times occupy that space. This is about how you want your spheres to look on your model. But you don't want to make your sphere's too large.If you make it so your sphere so huge it expands way beyond the model players will select it when they aren't trying to and that can be really frustrating. To show you an example of what you don't want look at this:

You should easily be able to tell the difference.

13. Now go back to Node Manager and right click your collision shapes that currently say UNNAMED. Choose 'Edit Node' not 'Edit'.

14. You now can enter a name for the collision shape. You could just leave it as it is because the name doesn't affect anything. Or you could put CollisionSphere1 or something of that sort. But what I like to do is put my model's name and "By Zerox", so that if someone deprotects a map my model is in or just takes it out of an non-protected map they can still open up my model and have a chance to see who made it.

Don't bother messing with the Vertex Coordinates in here (X, Y, Z). They don't make any difference to your collision shape.

15. Now you can save your model and your done. Your model will now be selectable in-game. You may want to go back to Edit>Properties and uncheck the collision shapes box so you don't have to look at the collision shapes while your just viewing models. But it's up to you.

Collision Boxes:

Now I'll attempt to explain Box collision shapes. Here is an example of where one should be used:

The box goes around the Dragon's long body that is tall and wide. The box is the better choice here because the dragon's body doesn't move much and is too tall and wide for a sphere to be really effective without making it take up too much space you wouldn't want as shown with the over-sized sphere with Cell above. You may also notice that the dragon has a sphere as well on it's head.

This is because it extends away from the rest of the body and is more easily covered in a separate sphere. That way too much excess space isn't used. Don't ever be afraid to try a combination of boxes and spheres. Now moving on to how you set up your own box.

1. Repeat steps 1-7 above and then select Box rather than Sphere.

Now boxes work pretty differently as far as setting them up goes. You now have Vertex 1 and Vertex 2. These will be the extents of your box. Always start Vertex 1's Z coordinate at 0. That will keep the bottom of the box at the bottom of the model. Obviously if your doing a box for something elevated this doesn't apply but just keep it at the bottom of the model.

Look at how I've set up my box for this model:

See how the X's and Y's are identical except for one is negative (-) and the other is positive? This is how I recommend you set them up because this causes the box to extend in equal distances both ways. Now the to figure out which direction is which you may have to experiment with them for yourself. But I'll try to explain them as best I can. X is from front to back and Y is from side to side, Z is up and down.

2. Now set your Z so it goes to the proper height so it is just above where the model ends. You may have to play around with all of these until you get the box to fit your model properly.

Also having identical X's and Y's isn't always going to work. For this model equal Y's didn't work because the back of the model was still outside the box so I had to adjust it to fit like so:

3. After you've got your box set up the way you want it save and then your done.

Notice: Boxes should be used on mostly stationary objects. In most cases spheres are typically better because they allow for a full range of movement. Use good judgment for what your model needs. But keep in mind most of the time you should be using spheres rather than boxes. Boxes more often than not will be used on doodads not characters/units.

Hopefully my tutorial has helped you and if you have any questions just ask.


 Last edited by a moderator: Apr 30, 2010

#### HappyCockroach

#### HappyCockroach

 Level 40

JoinedFeb 4, 2009Messages3,170

Resources: 76

#### Resources
Models 34Icons 40Packs 1Tutorials 1

Good tutorial! This is something people don't care about sometimes, and not adding them to the model makes the wc3 engine calculating them, not most accurately.

#### Zerox

#### Zerox

 Level 20

JoinedJun 6, 2008Messages987

Resources: 27

#### Resources
Models 21Icons 2Skins 1Ported Models 2Tutorials 1

Thanks glad you found it useful. I tried to cover everything with many screenshots. Any idea how I can embed them so I can get this approved? I don't ever put a model in my map without making sure it has these and if it doesn't I add them for my own use. XD I like being able to select my unit.

#### HappyCockroach

#### HappyCockroach

 Level 40

JoinedFeb 4, 2009Messages3,170

Resources: 76

#### Resources
Models 34Icons 40Packs 1Tutorials 1

Try this:

[attach]12345[/attach]

While 12345 stands for the attachment number. I guess you should attach'em to your post to do this.

Also, that maximum-sized font isn't needed. Just set the size to 3 or 4.

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

looks like a good tutorial. The creation of a collisions shape isn't difficult at all, but it is good for beginners to know that they are really important.

#### Zerox

#### Zerox

 Level 20

JoinedJun 6, 2008Messages987

Resources: 27

#### Resources
Models 21Icons 2Skins 1Ported Models 2Tutorials 1

Thanks DonDustin. I'll attach the images soon. Any way to attach them full size as in not a thumbnail?

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

huhh sorry going off topic!


butZerox i thought you only be online at wc3r.forumcircle.com 0.0


any way this tutorial is good like you posted there!

#### Zerox

#### Zerox

 Level 20

JoinedJun 6, 2008Messages987

Resources: 27

#### Resources
Models 21Icons 2Skins 1Ported Models 2Tutorials 1

I was here at the Hive first.  But it's true I'm more active there, 900 posts here 7000+ there.

Anyway glad you like the tutorial and find it useful.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

you know they don't make units more selectable, but make missile colission calculations way faster for the game (in other words, improve performance).

#### Zerox

#### Zerox

 Level 20

JoinedJun 6, 2008Messages987

Resources: 27

#### Resources
Models 21Icons 2Skins 1Ported Models 2Tutorials 1

BlinkBoy said:

 you know they don't make units more selectable, but make missile colission calculations way faster for the game (in other words, improve performance).


Click to expand...


Your both correct and incorrect. It is a proven fact that many models are selectable without these. But it's also been tested and proven that models with Collision shapes are 99x easier to select if done properly because they are targetable when selecting a unit. Some models without them are nearly impossible/are impossible to select without one unless you click and drag or use the F1, F2, etc. if it's a hero. But if you give these unselectable models Collision Shapes they can be selected with ease. As the CollisionShapes themselves are selectable. There's a reason every normal wc3 model Blizzard did has them.

#### HappyCockroach

#### HappyCockroach

 Level 40

JoinedFeb 4, 2009Messages3,170

Resources: 76

#### Resources
Models 34Icons 40Packs 1Tutorials 1

Zerox said:

 Your both correct and incorrect. It is a proven fact that many models are selectable without these. But it's also been tested and proven that models with Collision shapes are 99x easier to select if done properly because they are targetable when selecting a unit. Some models without them are nearly impossible/are impossible to select without one unless you click and drag or use the F1, F2, etc. if it's a hero. But if you give these unselectable models Collision Shapes they can be selected with ease. As the CollisionShapes themselves are selectable. There's a reason every normal wc3 model Blizzard did has them.


Click to expand...

Collision Shapes are really needed when substantial parts of the model aren't made of geometry, as of ribbon effects, particle emmiters, spawned effects or even attachments, if that is the case.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Zerox said:

 Your both correct and incorrect. It is a proven fact that many models are selectable without these. But it's also been tested and proven that models with Collision shapes are 99x easier to select if done properly because they are targetable when selecting a unit. Some models without them are nearly impossible/are impossible to select without one unless you click and drag or use the F1, F2, etc. if it's a hero. But if you give these unselectable models Collision Shapes they can be selected with ease. As the CollisionShapes themselves are selectable. There's a reason every normal wc3 model Blizzard did has them.


Click to expand...

You are wrong. What makes a model selectable is the extents. Adding colission shapes won't do anything but reduce inner calculations.

Happy: actually by defining extents on animations, you can select models made only by lights, particles, etc.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Zerox said:

 I was here at the Hive first.  But it's true I'm more active there, 900 posts here 7000+ there.


Anyway glad you like the tutorial and find it useful.


Click to expand...


nice tutorial...


just wondering why your kingdom hearts models aren't uploaded here at hive...

#### HappyCockroach

#### HappyCockroach

 Level 40

JoinedFeb 4, 2009Messages3,170

Resources: 76

#### Resources
Models 34Icons 40Packs 1Tutorials 1

BlinkBoy said:

 You are wrong. What makes a model selectable is the extents. Adding colission shapes won't do anything but reduce inner calculations.

Happy: actually by defining extents on animations, you can select models made only by lights, particles, etc.


Click to expand...

When did I say I was talking about selecting them?

(missile collision calculation instead...?)

Anyhow...BlinkBoy always coming up with new stuff for us to learn... =)

its mai modeling mentor =3

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great tutorial. Simple to follow and concise.

It is very detailed and I can't find anything really wrong with it.

Detached your signature. ~Approved.

EDIT: However, the images should be attached to the hive workshop. Not the other area. I'll approve it for now, but please update it to be attached to THW if you can.

#### Walle

#### Walle

 Level 13

JoinedJan 27, 2009Messages360

Resources: 22

#### Resources
Models 8Icons 11Skins 2Maps 1

Very good tutorial, really helped me ^^

#### HappyCockroach

#### HappyCockroach

 Level 40

JoinedFeb 4, 2009Messages3,170

Resources: 76

#### Resources
Models 34Icons 40Packs 1Tutorials 1

Zerox, as BlinkBoy said, this tutorial has a mistake right at its title.

I guess you edit the parts that say that it makes the model more easily selectable and mention that it's used for the missile collision calculation. You would also need to ask a mod to rename the thread itself.

But it would be nice if instead you gave tips on how to set custom extents or change screwed up extents in addition to the collision shape subject. After all both subjects have much in common. And very likely, a lot of great modellers here in THW don't have a clue about what are the extents and what are they used for.

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

Followed the tutorial to the letter, but was unable to make my model selectable.

Then I read this:

BlinkBoy said:

 You are wrong. What makes a model selectable is the extents.


Click to expand...

Went back into magos, calculated extents, and the model is now selectable.

This should be noted in the OP - it's much quicker, and works.

#### sunshinex3

#### sunshinex3

 Level 12

JoinedMar 1, 2009Messages372

Resources: 5

#### Resources
Maps 5

huh, working on this for 2 days... thank you for saying something rulerofiron99

"Went back into magos, calculated extents, and the model is now selectable."

works

#### Dark-Zalor

#### Dark-Zalor

 Level 19

JoinedOct 10, 2011Messages580

Resources: 28

#### Resources
Maps 25Spells and Systems 2Sounds 1

Hello everyone

it's important to add the step "Calculate Extends" after it

Then it works

 You must log in or register to reply here.


### Similar threads

Locked

 Collision Shapes vs. Collision Size


Mechanical Man

Jan 5, 2007

Modeling & Animation

Replies0Views2K

Jan 5, 2007
Mechanical Man

 Animation In 3ds Max 5 For Beginners


blazing-inferno

Jan 22, 2008

3D Modeling Tutorials

Replies9Views12K

Jan 23, 2008
skrab

Locked

 Only drag collision?


stan0033

Oct 29, 2018

Modeling & Animation

Replies0Views895

Oct 29, 2018
stan0033

Locked

[General]
 editing model's selection circle / collision shape


bnb2005

Apr 4, 2021

World Editor Help Zone

Replies1Views2K

Apr 11, 2021
Razorclaw_X

Locked

 Modeling and Animation 101


donut3.5

Oct 21, 2007

3D Modeling Tutorials
2

Replies70Views72K

May 14, 2015
wcity

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