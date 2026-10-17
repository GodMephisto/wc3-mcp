---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/scratch-made-weapon-attachments-with-particle-emitters.65525/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Scratch Made Weapon Attachments with Particle Emitters
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/scratch-made-weapon-attachments-with-particle-emitters.65525/>
#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2

Scratch-Made Weapon Attachments with Particle Emitters

By: Fabioz (U.S. East)

Introduction:

Today we are going to make a weapon that will be attached to a unit. The model that we are going to make is going to be completly from scratch. We are going to use only in-game textures. The weapon will also have a particle emitter to show "fire" or "blue flames". Neat!​

Tools required:

Magos War3 Model Editor

Oinkerwinkle's Vertex Modifier

Oinkerwinkle's PrimitivesThese are starting models

Warcraft III Image Extractor II

Warcraft III World Editor Duh...

Notepad Usually comes with your computor

Getting to Know the Vertex Modifier:

Selection:
Select: When highlighted, any points you select will light up, and will be modified. But, if you select new points, these points will be unselected.

Add: When highlighted, any points you select will light up, and will be modified. But, if you select new points, the new points and the old points will all be slected.

Deselct: When highlighted, any points you select will be unselected. But not any of the other already selected points.

Transformations:

Move: When highlighted, any points selected will translate.

Rotate: When highlighted, any points selected will rotate around the center of mass.

Scale: When highlighted, any points selected will shrink or grow around the center of mass.

Extrude: When highlighted, any points selected will be cloned in the same position as the older ones, but translated to a new position. This is very useful.

Other Useful Things:

X: When highlighted, any points will not be affected in the X axis, during a transformation.

Y: When highlighted, any points will not be affected in the Y axis, during a transformation.

Z: When highlighted, any points will not be affected in the Z axis, during a transformation.

Making Your Model:

Open up your Vertex Modifier

You should see three blank "viewports"

Go to "File" and select "Open". Find where you saved the Primitives.

Open one of the primitives that you want to use. I will be using a cube to create a sword.

Let us save the primitive. I named mine "Sword". Your screen should look a bit like this:

Transforming the Model:
So the sword will be facing the right way when attached to a unit, the weapon should face upright in the top right viewport.

The part of the weapon that will be in the unit's hand, should be right on the origin. (very middle)

For my sword, I first used the rotate tool to make the sword turn about 45 degrees. So the sides will be sharp

Then I used the scale tool and made my hilt (cube on the origin) long and skinny; press in X, Y, or Z to make it long, but not thick.

Next, I used the extrude tool to make the hilt extend into the other parts of the sword.

That is how I used the tools, there is no need for me to explain how I got to the finished product. All you have to do is play around with the tools, and eventually you will have a weapon. This is what I ended up with:

Adding Texture to Your Weapon:
Open up your War3 Image Extractor II

We are going to use in-game textures, because, most people don't like to mess with out of game textures. So, find a texture that you want use for your new weapon model.

I found the captain texture in the file: "Textures" and the picture name was: "DungeonDoodad0.blp". Remember where you found your texture! Write it down! Mine is "Texture\DungeonDoodad0.blp".

Once you find a good texture, right click the file and save it. Save it somewhere you will remember. Here is a screenshot:

Go back to Vertex Modifier and open the weapon you want to texture.

First, go to "tools" and select "UV Mapping". Click "Load Image" and load the texture that you saved. Exit out of that window and select your entire weapon, so each vertex (point) is lighted.

Go back into the UV mapping window. In the drop-down box, select how you want your model to face while texturing it. Then click "Remap".

Edit the points so they suround what you want to be on the model. I ended up with this:

Now, go to Notepad and open your model. You might have to change the type of files to "All Files".

Go to textures and there should be a part that says "Image", and after that it says "Textures\white.blp". When typing in the file, use "\" not "/"!

Change that to the name of the texture you used. Remember when I said to remember this, and to write it down? Here is what it looks like, the highlighted part is what you change.

Open War3 Model Editor and open your model. Now look at your beautiful weapon... ahh, so pretty. But, it can be even better!

!Note!: Before adding the particle emitters, you may want to test your weapon in the World Editor to make sure that it is "to scale". Make sure that it isn't too big or too small for the units.

If it is too big, or too small, all you have to do is use the scale tool in the Vertex Modifier. A sword from 70~90 units from the origin is all right.

Adding Particle Emitters:
Now that we have our weapon, let us make it into a fire sword, or maybe a water sword! Don't know what I mean? You'll soon find out.

I will be using 1 particle emitters. Go to "Windows" and select "Node Manager". You should see a blank screen with "Box01" at the top.

Right-click anywhere in the screen, but not on the word object "Box01" and select "Create Particle Emitter 2" Remember to select the one that says "2"!

Now, go back to the Vertex Modifier. Use your mouse to hover over the place you want your particle emitter, look at the bottom left part of the screen and write down the coordnates.

Go back to War3 Model Editor, right-click one of the particle emitters and select "Edit Node" change the name if you want. (to organize)

Change the "Pivot Points" to the coordinates you wrote down. Do this for This is what I have:

Now that you have specified where you want your emitter, we are going to choose what our emitter looks like.

Open up the "MPQ Browser" find a texture that you like. I would use SingleCloud.blp, because that is the most basic particle emitter that can work for pretty much everything. SingleCloud.blp is in the file "Textures".

Right-click your texture and select "Use as Texture" You can use more that one texture.

Exit out of the "MPQ Browser" and go back to the "Node Manager"

Double-click your emitter. A screen like this should show up:

This is what it all means:

Visibility: 0 = Not Visible; 1-Infinity = Visible. Usually always set to 1. 2 does the same thing as 3, etc.

Emission Rate: How many particles are released per second.

Speed: How many units per second the particles move

Variation: This varies the speed you have set. So if you set the speed to 30 and the variation to 20, each particle will travel at a speed between 10 and 50.

Latitude: This one is really neat! The measurments for this variable is degrees. 0 means all particles will travel up. If set on 360 particles will start from the middle and travel out in any direction.

Width: This is how long you want your particle emitter.

Length: This is how thick you want your particle emitter.

Note: Magos mixed width and length.

Gravity: This is how much gravity will play a part in your emitter. Setting this to a positive number will make the particles go down, while a negative number will make it go up.

Texture ID: This specifies the texture that is used as the particles.

Filter Mode: This changes the art of the texture, as much as I could figure out. This is almost always set to "Additive".

Segment 1: This is not seen. Unless "Time" is set to a different variable.

Segment 2: This is seen first (or second). This color mixes with segment 1 (If time is set correctly)

Segment 3: This is seen last, this color mixes with segment 2.

Alpha (0 - 255): This is how much of this emitter is visible. 0 = Invisible while 255 = completely visible.

Scale: This is the size of the particle.

Head (Life Span): This is how long you can see the head part of the particle.

Head (Decay): This is when the particle begins to fade (decay).

Tail (Life Span): This sets how long the tail can be seen.

Tail (Decay): This is the time when the particle begins to fade.

Rows: This sets how many rows of particles are emitted. I do not recomend changing this. It will look boxy if you do.

Columns: This sets how many columns of particles are emitted. I do not recomend changing this. It will look boxy if you do.

Life Span: This is how long (in seconds) one particle takes to disappear.

Tail Length: I did not notice any changes when editing this variable.

Time: This is the amount of time each segment is to be seen. This allows segment one to be seen, if you set it to one.

Flags:
Unshaded: If checked, particles will not be affected by lighting.

Unfogged: If checked, paticles will not be affected by fog.

Alpha Key: I do not notice any changes. Do not check

Line Emitter: Whether or not particles move in three directions, or two.

Sort Primitives Far Z: Keeps particles in order from color to color. Keep this checked

Model Space: Whether or not particles move with the emitter/model.

XY Quad: This makes it so the emitter can only be seen from the top, in XY coordinates; not XYZ coordinates

Squirt: Squirt: particles are created more rapidly, they Squirt out.

Head: Head: Shows the head (The current part ) of the emmitter

Tail: Shows the tail, AKA, the decay and such of the emitter. If you set time to a time longer than the head's lifespan, you could create some cool smoke effects. (As time effects the time it takes for the particle to go through it's entire color span)

Thank you to Lord_T for help with the flags

If anyone could help me out with some of these, help is appreciated.

There are a lot of sections to fill out, so I will show you the basic fire particle emitter settings:

Set the Visibility to 1

Set the Emission Rate to 100~200

Set the Speed to 30~50

Set the Variation to 0.02~0.10

Set the Latitude to 0

Set the Width to how thick you want it, mine is 80.

Set the Length to how long you want it, mine is 10.

Set the Gravity to 0

Set the Texture to the texture you want to use, min is: Textures\CloudSingle.blp

Set the Filter Mode to Additive

Set the Segment 1 color to dark yellow

Set the Segment 2 color to red

Set the Segment 3 color to dark red

Set all the Segment Alpha values to 255

Set the Segment Scaling values to 5~30

Set all the Start and End to 0

Set the rest of the Repeat to 1

Set the Rows to 1

Set the Columns to 1

Set the Life Span to 1

Set the Tail Length to 1

Set the Priority Plane to 0

Set the Replaceable ID to 0

Set the Time to 0

Check Flags: Unshaded, Sort Primitives Far Z, and Head
Here is a filled out Particle Emitter Node:

And this is what it looks like:

The possibilities are endless. Just change the texture and/or the particle emitter.

Here are some other screenshots:

Step 1:

Step 2:

Step 3:

Step 4:

Step 5:

Step 6:

Step 7:

An Ice sword.

Added June 22, 2008

Creating an Unequiped Model:

Here, I will teach you how to edit a model to look "unequiped" or weaponless by edinting the previously made Blizzard model. This unit will be holding our weapon. Awesome!​

Open Magos' War3 Model Editor and go to "Windows" and select "MPQ Browser"

Go to "File" and select either "War3.mpq" or "War3x.mpq" The latter is the expansion models.

Select the file "Units" and choose any model that is holding a weapon.

I went to "War3.mpq" and choose "Units\Demons\ChaosWarlock\ChaosWarlock.mdx"

Now, save your .mdx file as a .mdl.

Open your file in Vertex Modifier. It may ask you which geoset you want to open. Find the one with the weapons in it. For my model, the shield and staff thing are in two different geosets.

Now, we will remove the mesh that makes the weapon. Select the vertexs that makes up the mesh of the weapon. Then press "Delete".

This geoset two, where the shield is:

This is what geoset one looks like, after I remove the staff:

Save your model in the Vertex Modifier.

Open your model in War3 Model Editor

Note: The portrait of your unit will be messed up, to fix this, go to the node manager and create a new camera view with your desired portrait view.

Save your model as .mdx file.

Open your World Editor and go to the import manager.

Select "Import File" and import your .mdx file.

Save your World Editor and exit out.

Reopen your World Editor and map and go to the Objects Manager.

Select the Units tab and right-click in the left column and select create a new custom unit.

Select your new unit and go to "Art - Model File" and double-click that part.

Select your imported model in the "Import" drop menu.

Right-click your unit in the left column and select "Select in tool Palette.

Theres your new unit! Good job!

Equiping your Weapon:

Import both your unit(s) and weapon(s), save, exit, and reopen.

Go to the Object Manager, and go to the "abilities" tab.

Right-click the left column and select: "New Custom Ability..."

Choose an item ability, such as: "Item Damage Bonus", "Item Armor Bonus", or "Item Stat Bonus". We use these abilities because they don't show a buff.

With your new ability, remove any "bonuses" it has. Such as the "Item Stat Bonus Agility +1: go to Data - Agility Bonus, and remove that.

Set "Art - Target" to the weapon you want to give the unit.

Set "Art - Attachment Point 1" to:
hand
right

left

foot
right

left

overhead

head

chest

origin

For my weapon, I will use "hand" "right". Two different sets of strings.

Now, you can either make this ability an item ability or unit ability.
Item: you can match a weapon attachment with an item. Whenever an item with this ability is picked up, the attachment will be shown.

Unit: You can give or remove these attachments through triggers, or if you want to always have the unit to equip that weapon.

Now, you can create an item with the object manager, and give the item the ability we just created. If you use an item ability, you will need to give the new unit an invetory.

Please post any mistakes or if you have any questions!

Below are two models that are shown in the tutorial.

Look forward to other tutorials by Fabioz!

[Tutorial] Voting with Dialog Boxes - GUI by Fabioz

This Tutorial can be found on thehelper.net, wc3campaigns.net, and thehiveworkshop.com.


 Last edited: Apr 17, 2009

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

very nice tutorial.I bet it will be useful for a lot of people

#### ElectricSaiyan

#### ElectricSaiyan

 Level 9

JoinedJul 20, 2005Messages414

Resources: 4

#### Resources
Models 2Tutorials 2

mega tight!

Good tutorial, the effect is pretty nice.

Insta-win.

#### ike_ike

#### ike_ike

 Level 26

JoinedNov 19, 2007Messages1,175

Resources: 10

#### Resources
Models 6Icons 2Skins 2

You should probably write a little notice somewhere reminding people that to use move, rotate and other translations involves using the right-mouse button, not the left. It took me a week to work that out >_>

Also, I think that your particle emitters turn out differently in the game from Magos, because Magos' editor switched the Y axis with the Z axis (I think it was those 2)

Tail particles are not shown in Magos for some reason, but they turn up in World Editor. Personally, I wouldn't use them.

#### JetFangInferno

#### JetFangInferno

 Level 37

JoinedJul 22, 2004Messages1,002

Resources: 170

#### Resources
Models 170

Interesting, perhaps I should start modelling

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2

Would you like me to do anything with this so it can be approved?

#### jadegolem

#### jadegolem

 Level 7

JoinedFeb 1, 2008Messages295

Resources: 2

#### Resources
Models 1Maps 1

Good Tutorial. I made this using it but i screwed up becuz its my first scratch model

https://www.hiveworkshop.com/forums/models.php?id=xv391j

#### Ghan_04

#### Ghan_04

 Level 22

JoinedFeb 26, 2008Messages893

Resources: 2

#### Resources
Tutorials 2

Nice explanations. Very thorough.


Approved.

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2

@JadeGolem: Im glad that it is useful. It looks very good for your first model.

#### berryberry

B

#### berryberry

i screwed up

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2


 berryberry said:



 i screwed up

Click to expand...


No other way to learn =]

#### Lord_T

#### Lord_T

 Level 23

JoinedOct 27, 2004Messages911

Resources: 46

#### Resources
Models 36Icons 8Packs 2

Nice Tut, I think i can clarify some stuff on emitters tho.


 * Unshaded: Shades the emmiter ingame (Look at emitter at day and at midnight with this unchecked)


 * Unfogged: Shows in fog or not


 * Alpha Key: Not sure...


 * Line Emitter: Not sure...


 * Sort Primitives Far Z: Not too sure...


 * Model Space: Are the Emitters part of the model? Do they move with it?


 * XY Quad: This makes it so the emitter can only be seen from the top, in XY coordinates; not XYZ coordinates


 * Squirt: particles are created more rapidly, they Squirt out.


 * Head: Shows the head (The current part ) of the emmitter


 * Tail: Shows the tail, AKA, the decay and such of the emitter. If you set time to a time longer than the head's lifespan, you could create some cool smoke effects. (As time effects the time it takes for the particle to go through it's entire color span)

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2

Thanks, I added it! I also added you to credits after that section =]

#### Elxmonster

E

#### Elxmonster

if u attach it to to model it should move with the model ((Vertex Modifier))



 [B said:



 [/B]* Model Space: Are the Emitters part of the model? Do they move with it?


 QUOTE]


Click to expand...

#### wolfman

#### wolfman

 Level 11

JoinedMay 31, 2008Messages698

Resources: 2

#### Resources
Spells and Systems 2

once you make the actual sword how do you make a unit that uses the sword? or how do you replace an already existing units weapon with this one

#### wolfman

#### wolfman

 Level 11

JoinedMay 31, 2008Messages698

Resources: 2

#### Resources
Spells and Systems 2

when i open the image extractor it does not look the same as in the pic above does any1 know why??

#### Misha

#### Misha

 Level 49

JoinedJun 9, 2008Messages16,523

Resources: 73

#### Resources
Models 64Icons 1Packs 2Skins 4StarCraft II Resources 2

if yo replacin' the mdl's weapons first yo gotta shrink the mdl's weapons into oblivion

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2


 wolfman said:



 once you make the actual sword how do you make a unit that uses the sword? or how do you replace an already existing units weapon with this one


Click to expand...

Basic Weapon Removal Tutorial

Attachments Tutorial


 wolfman said:



 when i open the image extractor it does not look the same as in the pic above does any1 know why??


Click to expand...

I have no idea...


 Elxmonster said:



 [quote


* Model Space: Are the Emitters part of the model? Do they move with it?


QUOTE]


Click to expand...

Yes, the particle emitters move with the attachment. Unless the weapon is animated.

#### Misha

#### Misha

 Level 49

JoinedJun 9, 2008Messages16,523

Resources: 73

#### Resources
Models 64Icons 1Packs 2Skins 4StarCraft II Resources 2

Originally Posted by wolfman


when i open the image extractor it does not look the same as in the pic above does any1 know why??


What is really your problem with the imm. extr. i wonder 0_o?

#### GrimmReaper_VI

G

#### GrimmReaper_VI

Guys I'm using Illidan's blades. I have no idea which side of them is upright, since they are sort of 2D. I try to import them but they keep wobbling around. How do I make them stop??

(I took the blades by removing anything else on the original Illidan Model)

#### Misha

#### Misha

 Level 49

JoinedJun 9, 2008Messages16,523

Resources: 73

#### Resources
Models 64Icons 1Packs 2Skins 4StarCraft II Resources 2

GrimmReaper_VI said:

 Guys I'm using Illidan's blades. I have no idea which side of them is upright, since they are sort of 2D. I try to import them but they keep wobbling around. How do I make them stop??


(I took the blades by removing anything else on the original Illidan Model)


Click to expand...


Better trace your steps to what you were doing with the blades. Maybe you forgot to tackle anims of the Illidan mdl. Or just tell me, or someone else a bit more about your problem.


Althou some waepons do wobble a bit


PS rep me a bit. plz

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2


 GrimmReaper_VI said:



 Guys I'm using Illidan's blades. I have no idea which side of them is upright, since they are sort of 2D. I try to import them but they keep wobbling around. How do I make them stop??


(I took the blades by removing anything else on the original Illidan Model)


Click to expand...


The animations are still being used. You will have to create Illidan's sword thingies from scratch, or, you can import illidan into Milkshape 3d and remove everything but one sword, the export. (milkshape will remove all previously made animations). I recomend scratch modeling, then using Illidan's texture.


 Misha said:



 PS rep me a bit. plz


Click to expand...


...That makes people add bad rep...

#### RedFuser

#### RedFuser

 Level 6

JoinedJun 29, 2008Messages16

Resources: 4

#### Resources
Models 3Tutorials 1

Lol I added my sword to peasant's hand and now peasant is holding sword in it's middle. How do I change it?

Edit: Ok sorry I got it, there are colored lines - blue red green, they are center of your sword, shield etc.


 Last edited: Jul 2, 2008

#### RedFuser

#### RedFuser

 Level 6

JoinedJun 29, 2008Messages16

Resources: 4

#### Resources
Models 3Tutorials 1

How can I use my own textures?

#### CyberDuelX16

C

#### CyberDuelX16

Good Tutorial, even though i know how to model this still gave me lots of improvement and help. Thanks alot, and thanks to this i make my models much faster =D.

#### deathchiken

D

#### deathchiken

When i open image extractor it asks me to open up a mpq, where is that located cause i cant find it

#### RedFuser

#### RedFuser

 Level 6

JoinedJun 29, 2008Messages16

Resources: 4

#### Resources
Models 3Tutorials 1

MPQ is located in folder where Warcraft III is installed :/

#### deathchiken

D

#### deathchiken

when i try to right click on theskin i want nothing will happen and i cant save it, i try the other save on the top left of the wc3 image tool but that when i save the image that way it wont go onto the sword? Also when i downloaded to image extractor and when i opened it up it doesnt look like the one on the tutorial, and i downloaded it from this tutorial.


 Last edited by a moderator: Jul 27, 2008

#### Fiziks

F

#### Fiziks

I found it easy to follow, but the only problem is that the coordinates dont appear in the bottom left corner for me or anywhere at all for that matter.

#### Velmarshal

#### Velmarshal

 Level 16

JoinedMar 9, 2008Messages2,175

I did quite understand step 2.

I can't rotate in vertex modifier.

#### XeNiM666

#### XeNiM666

 Level 4

JoinedJul 19, 2008Messages37

Resources: 2

#### Resources
Icons 1Tutorials 1

[RAINBOW]nice tutorial!![/RAINBOW]


but i have some questions..


1. can you explain step 2??? i didnt quite understand how you did that sword.


2. do we have to use custom textures??

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2

Wow, there has been alot of posts since ive been on. Im usually on thehelper.net.


Anyways, ill try to answer your questions and add some stuff to my tutorial.


 XeNiM666 said:



 1. can you explain step 2??? i didnt quite understand how you did that sword./QUOTE]


Ok, once you have the vertex modifier open and create a little cube from the primitives, you start to mess with it, with the tools available, and I believe that is where everyone is getting confused...


(Remember: What I am saying here is not manditory! Have fun and create some wacky weapons)


First, I turned the cube on its side about 45 degrees. Because, this will make the sides pointy, not like your unit is carrying a 2x4.


Look at this picture, hopefully this will help you understand:


Look at the sword, see how the middle is bulged and as it goes to the sides, it slims down? That is the main reason for turning the cube...


Next, I use the EXTRUDE tool, and I make the sword longer using about 5 different segments. The main little cube will be my handle, while the next two segments will be the hand gaurd. Then the next 3 will be the blade, going out then back into a point, using the SCALE tool to make the point at the 4 end vectors.


Lastly, I used the SCALE and MOVE tools to edit it minorly to make it how I wanted it.


Hopefully this cleared up some of the confusion, thank you for posting and asking questions!


 XeNiM666 said:



 2. do we have to use custom textures??


Click to expand...

We are not using custom textures. We are using in-game textures. You can use custom textures, but when you are creating simple weapons, people dont like to have to import new textures...


NEW:


I will be adding a new part that will explain how to remove weapons and add your attachments to models!!! Take a look!


Click to expand...

#### Ribenamania

#### Ribenamania

 Level 19

JoinedMay 14, 2008Messages816

Resources: 54

#### Resources
Models 14Icons 4Skins 23Maps 5Spells and Systems 5StarCraft II Resources 1User Interface 1Tutorials 1

Yay! I finally know how to make emitters..

Any suggestion which texture should I use for icey steam?

Btw, I followed this tutorial and I failed to make it.

OIC, I need to play animation  It worked


 Last edited: Oct 21, 2008

#### redragex

#### redragex

 Level 8

JoinedDec 17, 2008Messages403

a long but good tutorial this will help me out plenty

#### Blaxor

#### Blaxor

 Level 10

JoinedSep 6, 2008Messages423

Resources: 9

#### Resources
Models 1Icons 8

this tutorial is very nice but can someone tell me why the vertex modifier crashes and screwes my model up by making the mdl file unopenable with the vertex modifier when i press Tools -> UV Mapping? it gives me an error message that says "Runtime error '6' Overflow". Someone please help.

EDIT: Found out how to fix the crashing  just had to change some setting on my computer.


 Last edited: Jan 31, 2009

#### Solecompany

#### Solecompany

 Level 5

JoinedJan 8, 2009Messages61

Resources: 5

#### Resources
Models 1Skins 2Maps 2

cool tutorial ...

#### Linaze

#### Linaze

 Level 24

JoinedFeb 28, 2007Messages3,478

Resources: 4

#### Resources
Models 1Tutorials 3

Broken images = not good.

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2

Linaze said:

 Broken images = not good.


Click to expand...


Thank you for telling me there. I would have never have found it. All the pics should be fixed.


+Rep for giving me a heads up!

#### Vakama_Lynx

#### Vakama_Lynx

 Level 7

JoinedMay 8, 2009Messages102

Resources: 2

#### Resources
Models 1Templates 1

How can you combine two vertices\vertexes (whatever it's called in plural form) to make a face (is that what the line between two vertexes\vertices called?) thanks in advance!

#### AutoRefresh

A

#### AutoRefresh

dudee...what path do I need to use to import my weapons to the map, if I use none they are invisible


 Last edited by a moderator: Jul 6, 2009

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2

Vakama_Lynx said:

 How can you combine two vertices\vertexes (whatever it's called in plural form) to make a face (is that what the line between two vertexes\vertices called?) thanks in advance!


Click to expand...


In vertex modifier, you can select as many vertices as you want and go to Edit and select "Snap" This will bring all of the vertices to one point and then you can go to Tools and go down to "Weld" this will bring the vertices together for good.


 AutoRefresh said:



 dudee...what path do I need to use to import my weapons to the map, if I use none they are invisible

Click to expand...


I don't think you need to set the path to anything specific, unless you use a custom texture, then you need to make sure they label to eachother... but if you use in-game textures, you shouldnt have to set it to anything...

#### Dumbdumb36

#### Dumbdumb36

 Level 7

JoinedJan 13, 2008Messages201

Resources: 1

#### Resources
Maps 1

I followed all the steps that you layed out... but it doesnt show in the model editor like yours did... I'll try it ingame...

ok it worked ingame ^^

#### Dumbdumb36

#### Dumbdumb36

 Level 7

JoinedJan 13, 2008Messages201

Resources: 1

#### Resources
Maps 1

I followed all the steps that you layed out... but it doesnt show in the model editor like yours did... I'll try it ingame...

#### warlockjc

#### warlockjc

 Level 2

JoinedOct 23, 2009Messages15

How come the link for the Vertex Modifier. I mean, the download works, but the program doesnt. It keeps saying "Component 'COMDLG32.OCX' or one of its dependencies not correctly registered: a file is missing or invalid." Does anyone know why?

#### medion555

#### medion555

 Level 9

JoinedJun 20, 2008Messages476

Resources: 2

#### Resources
Icons 1Maps 1

I cant open the textre i saved .;..


 Last edited: Jan 9, 2010

#### Malcous

#### Malcous

 Level 13

JoinedApr 2, 2008Messages1,246

Very nice tutorial =)

#### sonicspeedx13

S

#### sonicspeedx13

i cant download the secound thing that is needed, when i try to open it it says that there is a file missing. anyone know what thats all about?

#### Mulgrim

#### Mulgrim

 Level 15

JoinedMar 31, 2009Messages1,556

Resources: 4

#### Resources
Models 3Tutorials 1

this one helped me, +rep

#### Cablez

C

#### Cablez

Nice tutorial!

#### makai

#### makai

 Level 9

JoinedApr 30, 2010Messages323

Resources: 3

#### Resources
Models 2Icons 1

why when i click rotate or whatever nothing happen?? i already click at x axis or whatever axis.. i try all button and nothing happen...

Edit: hahaha.. now i know why... coz must use mouse!! =.=


 Last edited: Jun 16, 2010

 You must log in or register to reply here.


### Similar threads

 Particle Emitters 2


Vinz

Dec 18, 2020

3D Modeling Tutorials

Replies19Views14K

Mar 13, 2026
OVOgenez

 Model Editing


Fabioz

Jan 18, 2009

3D Modeling Tutorials

Replies45Views66K

Oct 10, 2021
KaiZen7th

 Teamcolor Particles


Anvil

Nov 28, 2012

3D Modeling Tutorials

Replies4Views7K

Dec 19, 2012
Anvil

 How to make XY Quad Particles Emitter ( And Rain Drop Effect Tutorial )


Anvil

Aug 27, 2012

3D Modeling Tutorials

Replies10Views14K

Jul 9, 2017
Veronnis

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