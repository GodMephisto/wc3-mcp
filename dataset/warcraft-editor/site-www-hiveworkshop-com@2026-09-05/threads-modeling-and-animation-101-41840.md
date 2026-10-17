---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/modeling-and-animation-101.41840/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Modeling and Animation 101
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/modeling-and-animation-101.41840/>
#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

Modeling and Animation 101

Alright, I figured every newbie needs a beginning, and this would help them get them off on the right foot. This tutorial teaches very little advanced tricks, and should be referenced more as a glossary. However, there will be tips and tricks that I'm sure almost anyone could learn from.

 Recommended Tools:


Magos Model Editor (for section 1 and 3)

Oinkerwinkle's Vertex Modifier (for section 1)

Oinkerwinkle's Geoset Merger (for section 1.4)

Milkshape 3D (for section 2)

Guesst's MDL/X Exporter (for section 2.3)

Table of Contents

Section 1 - How to construct a mesh.

Section 1.1 - Anatomy of a Mesh

Section 1.2 - Scratch modeling in Milkshape

Section 1.3 - Editing a Mesh

Section 1.4 - Geomerging​
Section 2 - Animating a Mesh

Section 2.1 - Animating the Skeleton

Section 2.2 - Animating the Skeleton

Section 2.3 - Exporting using Guesst's Exporter​
Section 3 - Finalizing a Model

Section 3.1 - Introduction to finalizing

Section 3.2 - Getting textures onto your model

Section 3.3 - Nodes

Section 3.4 - Other Finalizations​
Section 4 - Misc

Section 4.1 - FAQ

Section 4.2 - Credits

​

Section 1 - How to construct a mesh.

This section will teach you the properties of meshes.

Section 1.1 - Anatomy of a Mesh

 Words to know:


Vertex (plural vertices) - A 0 dimensional dot. These do not show in a model's appearance, but construct faces.

Faces - Constructed by 3 vertices connected together to make a visible 2 Dimensional plane.

Mesh - A 3 dimensional creation that is composed of faces. Anything that is created by 2 or more faces is a mesh.

What composes a model? Apart from animation, texture, and nodes, we have what is known as the mesh. The mesh is what everyone sees. But what composes a mesh? Let's take a look. If you open up your model in Magos Model Editor, and go to Edit -> Properties and look for Fill Mode. It should currently say "fill". If you check that dialog to be "point", you will notice your model is hardly even visible!

Why?

Each one of the faint little dots you see is a vertex. A vertex is the most basic dimension in the world that has been theorized, it is 0 Dimensional. In truth, these should not be visible since vertices have no width and no length whatsoever. Vertices form the outline of the model.

Now, go back to Edit -> Properties, and select "wireframe" instead of "point". You'll see something brand new, a bunch of lines connecting the points. If you are a careful observer, you should notice something: each and every one of those connections for a triangle. The triangles are also known as faces, polygons, polys, tris, or many other names. Faces are triangles that fill in the model. Once you get to texturing the model, the skin is actually applied to these faces. 2 or more faces makes a mesh.

And that is simply the anatomy of a Warcraft 3 model's mesh.

Section 1.2 - Scratch modeling in Milkshape

 Words to know:


Face - The face tool of Milkshape 3D takes 3 vertices and creates a face between them.

Backfaces - The opposite side of the face, created by clockwise creation.

Scratch modeling can be done very easily. All you have to do is create 3 vertices using the Vertex tool. Then, face them together using the Face tool. With the Face tool selected, click on 3 vertices.

Note: It is important to face the vertices together in a counter-clockwise direction. If you do it in a clockwise direction, you will get a black backface, which will appear as see-through unless you have a two-sided texture applied to it. To turn a backface into a normal face (thus flipping the face around), select the face and press Ctrl+Shift+F.​
Now, face these faces to more vertices, and before long, you will have something that resembles a mesh.

Another option is to play around with the different primitives. Primitives are basic shapes that you can transform to fit your needs.

 Primitives


Sphere - Creates a 3 Dimensional circle, fairly high face count. Not recommended to use in masses.

Geosphere - Less faces, more smoothness than the normal sphere. The downside? No easy way to UV map this.

Box - Creates a rectangular prism consisting of 8 vertices and 12 faces.

Cylinder - With this, you should use as little stacks as possible for lower poly count, however that will still fit your need for modifying and animating. Slices are mainly just to improve roundness, but don't do more than you need!

Plane - Creates a basic rectangular plane, consisting of x*y*2 faces, where x is the HDivs, or horizontal divides, and y is VDivs, or vertical divides.

Extended Primitives - These can be accesses in Tools. Most of these have a lot of faces, so use these with caution.

There aren't many instructions for this part, just practice until you get better!

Remember not to go too crazy with this, as too high poly models do not belong in Warcraft 3. Usually models should be under 1500 faces. To check how many faces you have, go to Tools -> Show Model Statistics.

Section 1.3 - Editing a Mesh

 Words to know:


Axis - Comes in X, Y, and Z. The X axis is Front and Back, The Y axis is Left and Right, and the Z axis is Up and Down.

Translation - Sliding vertices, faces, or meshes amongst the X, Y, and Z axis.

Rotation - Turning the vertices, faces, or meshes about the X, Y, or Z axis.

Scaling - Increasing or decreasing the size of the faces or meshes.

You should get familiar with how to edit a mesh before you start on any scratch projects. The main idea is to edit the position of the vertices, since vertices dictate the shape of the faces, and faces dictate the shape of the mesh. Forthis portion, you may use Magos, but I would highly recommend using Oinkerwinkle's Vertex Modifier or Milkshape 3D.

Note: Warcraft 3 models come in the extensions .MDX and .MDL. MS3D can only import .MDX, where as Oinkerwinkle's can only read .MDL. You can switch between the two using Yobgul's MDL/X Converter.

Note 2: It is not recommended to import to MS3D unless you are willing to redo the animations. There is NO exporter that exists that will transfer anything but the mesh and textures to MS3D, and unless you tinker with the textures a lot, you can't import those either (unless you do it manually. They cannot be in .blp format)

​
Now, most modeling tools will come with 3 or 4 viewports. Milkshape has 4, Oinkerwinkle's has 3. You can set the ones in Milkshape to whatever axis projection you want it to be. This is the X axis (projecting the front or back of a mesh), the Y axis (projecting to either the left or right of the mesh), or the Z axis (prjecting either the top or underside of a mesh).

From this point on in editing the mesh, I will be referring to Oinkerwinkles, although most steps can be done easily with Milkshape, 3DS max, or anything else.

In Oinkerwinkle's, it is locked to certain axises. (see picture below)

After you've become friends with your model editor's viewports, you may now do the fun part: editing the mesh!

In the screenshot above, you'll see that there are 10 buttons, plus a set of 3 buttons with X, Y, and Z on them all off to the right. To use the top 3, use left click and drag on the axis projection windows, and to use the rest, hold down the right click button and drag.

Here's an overview of what they do:

 Basic Overview of Oinkerwinkle's VM Buttons:


Select - Will select all the vertices selected with a drag-selection box and deselects everything outside of that box.

Add - Adds the vertices selected with a drag-selection box to your current selection.

Deselect - Does the inverse of add.

Move - Moves selected verices about the axis. Also known as translation.

Rotate - Rotates selected vertices about the axis.

Scale - Scales selected vertices on all axis, unless some axis are locked (see below)

Extrude - Extrudes the selected vertices.

Snap - Finds the midpoint between every single vertex selected and moves the selected vertices to that point. Tip: Welding (under tools) attaches the selected vertices together. It is a good idea to weld snapped vertices.

Delete - Deletes selected vertices

Clone - Works in conjunction with move, scale, or rotate, and duplicates the vertices and faces and then applies the movement, rotation, or scaling.

Lock X, Y, and Z - This restricts movement, scaling, or rotation.

With all of this in mind, the next step is to simply have fun editing the mesh you want. However, don't make only a few simple edits, such as making a sword bigger, and call it a day! Read on to see what you can do further.

Section 1.4 - Geomerging

 Words to know:


Geoset - A section of a mesh. Geosets can only use one texture and one geoset animation (more on geoset animations in section 3).

Bone - More in section 2, however, these are good to know for Geoset tranfers. Bones dictate movement in a mesh.

Marker - A blue dot in Oinkerwinkle's that you can use to save a spot. These come in handy for positioning geosets, however they are not a requirement.

This next section is quite popular amongst the Hive. In fact, if you have ever seen the admirable works of General Frank and Elenai, those are mainly geomerges done very well. To do this portion, you can either snap and weld faces together in Milshape or also, you can use Oinkerwinkle's Geoset Merger, which is what I'll use in my tutorial here.

Preparing a Geoset is the first step to geomerging. Find out where you want the geoset by placing a marker graphically using View -> Add marker (you can also do this graphically or at the center of selection). Then, move the edited geoset to where it needs to be. Once you have done all of this, you

Oinkerwinkle's Geoset Merger has a very simple interface, here's what it looks like:

The fields are pretty simple, and explained in the picture. You get the model you want to add the geoset onto, the geoset seperated from its orginal mesh, and then make a save name for it.

NOTE: Always name your filename different from the last modification. Always.​
The Texture path is the path for the skin, you can find the ingame ones with Magos by using the mpq browser, finding your skin, right clicking on it and selecting copy to clipboard, then pasting in the path field. Two sided means that it will be applied to the back faces as well as the front faces.

The filter types are explained in the picture, and will also be explained in more detail in section 3.

The Bone is the bone to attach it to, Oinkerwinkle was friendly and gave the helper name it was linked to.

Open the file up in Magos, and see if you go the desired effect.

If not, make amends and try again!

Once you have gotten a result you like, you may either:

Go to Section 2 if you want to learn animation (should probably have Milkshape 3D)

Or if you are content with the Blizzard animations, go to Section 3.

Section 2 - Animating a Mesh

This next section will go over basic animation techniques. This next section will be done ENTIRELY in Milkshape 3D.

Section 2.1 - Creating a Skeleton

 Words to know:


Bone - A bone, as previously discussed, dictates the movement of vertices in a mesh.

Joint - A milkshape term for a bone, these essentially dictate the movments of bones. In Milkshape, you use joints as opposed to bones.

Parent and Child bones - Parent bones dictate the movement of their child bones, however, child bones can be moved freely without affecting its parent bone.

Skeletons are a required part in the human body. Without them, we would no be able to move at all!

While most anatomy to model analogies fail, the skeleton does not. Like the human body, a model would not move without a skeleton.

A skeleton is composed of bones if you hadn't guessed, and each of these bones dictates a certain group of vertices. However, before you get to assigning vertices, the first step is to create the skeleton.

First, decide what your first bone will be. The first should be a root bone that can be used for easy rotation of the entire model. Then you need to pick your next bone. Usually, I pick a place like the pelvis or chest as a center of gravity.

In order to place a parent bone: Deselect all joints and click on the joint bone. Click on one of the non-3D viewports to place the bone, then select and move it.

In order to place a child bone: Select the joint you want to be the parent. Place and move your joint like you did with the parent bone.

Here is an example skeleton:

Depending on the complexity of the model, the number of bones will vary, however to give you a good idea, most Blizzard models average 20-30 bones.

Once you are done and happy with your skeleton, you must assign the vertices to the joints. To do this, click the select tool (make sure its set to select vertices!). With select mode on, go to the joints tab and select the first bone. Select ALL the vertices you want to be attached to this bone. Do what you think is logical.

Repeat this step for every bone until you are done.

Note: Make sure NO vertices are unassigned, vertices left unassigned are known to cause bugs.​
Check to make sure you are content with the way your vertices are assigned, then after that, go to tools and run Zero Joints. This is a very crucial step, do not proceed without doing this. You have been warned.

Once you are done, begin animating by clicking the Anim button at the bottom right corner of the Milkshape 3D window, and move onto the next section.

Section 2.2 - Animating the Skeleton

 Words to know:


 Frame - A single frame of animation, the basic unit of time. To change the frames per second (fps), go to preferences->Frames Per Second.

Keyframe - Keyframes are markers, whenever a keyframe is set, all the joints that are selected to have a keyframe made upon them will be at that position for that frame. Difference between keyframes, not frames, is what causes motion and animation.

Timeline - A visual display of frames.

At the bottom of your Milkshape window, you'll see something like this, called a timeline:

 Proceeding from left to right, here is what everything is:


The green slider is your current frame. You can move this around to change your frame.

Each small tick mark on the timeline is a single frame.

The first number is your current frame, the second one is total frames. Total frames can be changed at any point with no consequence.

The buttons are (from left to right): first frame, last keyframe, play backwards, play forwards, next keyframe, last frame.

Move and rotate joints like you would with vertices. You may not, however, scale joints in Milkshape.

Whenever you want to save a pose of a skeleton as a keyframe, go up to Animation, and with all the joints you want a keyframe for selected, click on Create Keyframe, or Ctrl+K. More keyframes makes a smoother animation!

Note: Leave 5-10 frames per each animation.​
Animating is a part at which you can only try and improve upon your works. After a lot of practice, you will eventually become quite good at animation, if you have patience.

Some tips:

Since Milkshape can only deal with linear animation, it is up to the animator to use easing, or smoothing out the animations.

Move more things than you may think you need to. Every detail counts!

Look over your animation a LOT.

Act out your animations while looking at yourself in a mirror. As silly as this may sound, it helps.

Section 2.3 - Exporting using Guesst's Exporter

 Words to know:


 None.


Once you are done animating, you're going to need Guesst's MDL/X Exporter. Put it in your Milkshape folder (Most likely in C:/Program Files/Milkshape3D or something of the like).

There are many options you can do for the exporter, all must be done in Milkshape comments. To see more, read the page where you downloaded the exporter from.

However, this tutorial will cover the ones required for exporting animations.

Go to the Model tab, and click on comment. Now, for each animation, put the animation name, then the frames it plays during.

Ex.:







 Code:

Stand,1-30
Walk,40-60
Death,80-100
Attack - 1,110-150
Attack - 2,160-200
etc.

Note how there are no spaces, except in the animation names.

Now, go to the joints tab. For every single joint, make click on comment and write

 Code:

Linear

This "Linear" is important because it sets the bone's interpolation to linear, meaning it will go up and down smoothly. The other two interpolations are Bezier and Hermite, but they are quite complex.

After that, go to the first frame of animation, turn animation off, and go to file -> export -> Warcraft3 MDL/X

View it in Magos, and if you are content with your animations, continue onto Section 3.

Section 3 - Finalizing a Model

This section will teach you all about finalizing a model..

Section 3.1 - Introduction to finalizing

 Words to know:


Alpha - How visible something is. For most things its either rendered as 0 (invisible) or 1 (visible), however for particle emitters it is rendered 0-255, with 1-254 being only partially visible.

Flags - Small check boxes. Most of these hardly impact the model and will usually want to be left alone, however there will be a small explanation on what the important flags do for each section.

In nearly every section of finalizing, you will see some recurring terms. This section will go over those.

Filter modes are used whenever a texture is being used. The filter modes are:

 Filter Modes:


None - Only displays what the texture looks like.

Blend - Blends the texture's alphaed sections with textures behind it. Typically used with Replaceable IDs to make team color.

Transparent - Not shown in the picture below, but simply does not show the alphaed areas.

Additive - Ghosty effect. Typically used with particle emitters.

Add Alpha - Adds a lighter ghost effect.

Modulate - An inverse additive filter.

Here are all of the transparencies excluding Transparent:

For finalizing, it is also important to know frames in Warcraft Models. These can be easily viewed in the sequence manager, you can view the first and last frame of a model. They will be listed as Start and End.

Many values can be "Animated" by checking a tick box. This is different from normal animation, you must list it as FrameNumber: Value

Ex:

 Code:

100: 1
1600: 0
1800: 1

Tip: You can also do animations with this. Curious? Go to Node Manager, right click on a bone, select edit node, and click on translation. If you chose a bone with any translation, you will see that it has codes like the ones listed above, if they use linear or no interpolation. This tutorial will not explain Bezier and Hermite interpolation, as they are complex, and in the words of RDZ, "Let's just say they are evil little piggies. Only super math nerds would understand them." If you see that at the bottom of the window, it is Bezier or Hermite interpolation, you can try to figure it out, but you don't have to.​
This is often used with Alpha, which is synonomous with visibility.

Also, there will be checkboxes known as flags in many things you must finalize. Ignore most of these unless you feel like playing around, or this tutorial tells you to check certain ones.

Section 3.2 - Getting textures onto your model

 Words to know:


Texture: A 2D art that can be applied to a 3D mesh.

Material: Textures and combinations of textures that are assigned to geosets.

Replaceable ID: A blank texture that can be told, depending on its ID number, to become various useful skins.

This tutorial will not review how to make textures for models, so for this section it is assumed you have made and UV wrapped all of the textures you need. If you need any from ingame, use the .mpq browser to find them, right click ,and select use as texture.

Go to your texture manager. Gather up all the skins, or textures that you will need for your model. The textures give your model an appearance, as opposed to just being white.

In order to attach the textures, you must attach them through materials. Go to Magos Material Manager and right click on the blank white panel, and select Create New. Right click in that blank box and select again, create new. Select your texture.

If you want backfaces to be shown, then check the two-sided flag. Then, all you must do is select your filter mode, and you should be set. Play around until you get an effect you like.

Multiple layers are used for things such as team color. In order to make a Team Color, first you must right click in your Texture Manager. Select Create Empty. Right click on the new blank texture and select Edit. Change the Replaceable ID in it from 0 to 1.

Now, go to your material that you want to be team colored, and right click, and create a new layer. This layer should be on the top. Make it replaceable ID, no filter mode. Make the bottom layer have either blend or transparent. If the alpha channels were done properly in the texture, this should work.

Section 3.3 - Nodes

 Words to know:


Node: Nodes are viewed in your node manager of Magos.

Pivot Point: Where the nodes are positioned.

Nodes are probably the largest part of finalizing. These nodes do various things, most of them are spicing up your model, such as particle emitters and event objects, however, if you'll notice, bones are nodes too!

The different types of nodes are listed here:

 Nodes:


Attachment*: Used ingame to define where buffs can be attached to the model.

Bone: Dictates mesh movement.

Collision Shape*: Makes the mesh easier to select by adding regions that can be selectable along with the model.

Helpers: Can parent and organize.

Light*: Creates an ingame glow.

Particle Emitter*: Can create particles of a model or texture that have a constant and random spawn rate.

Particle Emitter 2: Creates a generator that creates a specified texture. Frequently seen in explosions or fire.

Ribbon Emitter*: Leaves a trail of the chosen texture.

*Only displays/has an effect ingame, not in Magos. Collision shapes can be viewed under properties.

Every Node has something called a pivot point, or where the node is positions before any animating takes place. Additionally, each one has translation, rotation, and scaling, all of which can be animated, by doing the "Animated" code like in section 3.1.

Pivot points have X, Y, and Z. As a refresher, X is back and forth, Y is left and right, and Z is up and down.

Most of the nodes are just fun to experiment with, however some need to have specific information.

Attachments - Attachments must have certain names, or else they won't function. These names are:

 Attachment names


OverHead Ref

Origin Ref

Head Ref

Chest Ref

Hand Left Ref, Hand Right Ref, Foot Left Ref, Foot Right Ref

Weapon Ref

Sprite 1/2/3/etc. Ref

There are many others, but those are the main ones.

Additionally, you can name these simpler names, such as origin or overhead, or hand,left. This tutorial just lists the Blizzard names for the attachments.

Tip: If you make a collision shape, and make collision shapes visible under properties, you can see where you place your attachments by making a sphere collision shape with the vertex being where you think you should place your attachment, and the bounds radius 10. This will serve as a marker so you can get more accuracy with your nodes that aren't normally visible.​
Particle Emitter 2 is a very frequently used node in warcraft models. Most of this is just experimentation, however this will tell you what all the fields do.

 Particle Emitter 2


A: Visibility. Works like Alpha discussed in section 3.1

B: Emission Rate: How many particles are produced/second.

C: Speed: How fast the particles travel.

D: Variation: More variation makes more of a difference in particles. 0.92 is usually a good field for this.

E: Latitude: The variation in height differences when moving. No latitude means a straight line, and the more latitude, the more spherical.

F: Width: Variation in left and right of the relative rotation. Note that Magos interprets width and length and vice versa.

G: Length: Variation from back to front of the relative rotation. Note that Magos interprets width and length and vice versa.

H: Gravity: Reverse speed that gradually makes the particle fall (or rise with negative speed) as time goes on. This is useful for making arcs in particle movement.

I: Texture ID: What texture this particle emitter will emit.

J: Filter Mode: The exact same filters from section 3.2.

K: Segment coloring, size, and alpha: This will change the color and how visibile the particle is. It will play Segment 1, then smoothly transition into Segment 2, and then into Segment 3.

L: Head: Head is the start of a particle.

M: Tail: The tail of a particle will be dragged behind the particle, giving it a stretched effect. Note: Tails will not display in Magos.

N: Flags: The most important one to check is Head, as this will make the particle appear. Tail indicates it has a tail, and XY Quad makes it so they are flat on the Z plane. The rest you can just experiment with.

O: Rows and Columns: When using a texture that ends with 4x4, 8x8, or anything of the like, use this so that the particles will display properly.

P: Life Span: How long the particle will last before disappearing.

Q: Tail Length: How far the tail will be dragged behind.

R: Priority Plane: I've never learned what this does, leaving it as is is perfectly fine.

S: Replaceable ID: will set its texture to a replaceable ID that gets the properties of it, such as team color. This is not recommended, as scratch made emitters with Replaceable ID's tend to crash.

T: Time: How quickly the particle will pass through the three segments.

Collision shapes can come in either Boxes or Spheres. For boxes, you must define the two vertices at which is stretches, and with spheres you must set one vertex (the center) and then the radius of the sphere.

Event Objects are important in a model. They play sounds, leave footprints, squirt blood, or create ubersplats. In order to make one, all you must do is double click and edit the kind of Event Object, then create a time at which it is played. Right click on the white box and select create new, then enter in your frame number. this is hard to do from scratch, considering they wont play in Magos, only ingame.

Those are the most commonly used Nodes.

Section 3.4 - Other Finalizations

 Words to know:


Geoset animations: Can change the color and visibility of geosets.

Portrait: Ingame, will display the portrait as the little window next to the unit's stats.

For geoset animations, there are two basic fields for them:

Color, which can be easily changed by clicking on the [...] Box under color. Note these will only play during animations, not in the Unanimated phase in Magos.

Visibility: General Frank has written an excellent tutorial on Geoset Animations and visibility, go here to see it.

Some tips: Placing Geoset animations with no geosets, or two geoset animations for one geoset will crash the game when you import it. MAKE SURE NOT TO MAKE THIS MISTAKE!

Portraits - Making a portrait is simple, all it is is a model with a camera. In fact, if you made a portrait animation on your model, all you must do is get the view you want in Magos, go up to the Camera Manager, right click, and select create new from current view. If you have a seperate model, you do the same thing.

The portrait for a model must be the .mdx file name + _Portrait.

Example:

Model.mdx

Model_Portrait.mdx

blah.mdx

blah_Portrait.mdx

Renaming the model: This is easy, go up to Edit -> Model Properties and change the model name.

Once you are done with this, you may pat yourself on the back, you have made a complete model!

Test it ingame, tweak it, and make any final changes.

Section 4 - Misc

Section 4.1 - FAQ

Q:

 whats a good program to use???

-a Free program-


Click to expand...

A:
Magos Model Editor (for section 1 and 3)

Oinkerwinkle's Vertex Modifier (for section 1)

Oinkerwinkle's Geoset Merger (for section 1.4)
These are all free. Go check out all of the other Oinkerwinkle Tools too. They are extremely useful.

Q:

 "Component 'COMDLG32.OCX or one of its dependencies not correctly registered; a file is missng or invalid"


Click to expand...

A: Google search COMDLG32.OCX and see if it has a download, and if it has instructions of where to put it.

Q:

Candy said:

 I don't have any model loaded. Where can I get that?


Click to expand...

A: In Magos, you can either open a custom made model by going to Open and finding your model, or if you want a blizzard one, go to Window -> Mpq Browser. Go to File, and select either War3.mpq, War3x.mpq, or War3Patch. Then, browse through the folders until you find a model you like. Also, if you want additional models, get the MPQ Master. It can extract any model or skin flawlessly, including the Blizzard Cars.

Q:

 after i cr8 new current view, then must i do next?


Click to expand...

A: Right click on that, and just change the name to portrait. If there is an animation labeled "Portrait", you're good to go. If not, you need to make one.

Q:

 i want to merge samurai head to gangster body, n wanna use gangster animation.


Click to expand...

A: Use gangster as the base model, samurai head as the geoset.

Q:

 how to make mdx portrait? i want make portrait use box, n he no talk, just box picture?


Click to expand...

A: There's a short section on portraits in the tutorial, I think what you want is the part about cameras at the top.

 Portraits - Making a portrait is simple, all it is is a model with a camera. In fact, if you made a portrait animation on your model, all you must do is get the view you want in Magos, go up to the Camera Manager, right click, and select create new from current view. If you have a seperate model, you do the same thing.

The portrait for a model must be the .mdx file name + _Portrait.

Example:

Model.mdx

Model_Portrait.mdx

blah.mdx

blah_Portrait.mdx


Click to expand...

Q:

 How to make particales apper only in death animation ?


Click to expand...

A: With particles, they have either visibilities of 1 (visible) or 0 (invisible).

 Many values can be "Animated" by checking a tick box. This is different from normal animation, you must list it as FrameNumber: Value

Ex:

 Code:

100: 1
1600: 0
1800: 1

Click to expand...

You can view the different frame numbers from the Sequence Manager.

For every sequence, you need to type in FrameNumber: 0 besides death. Death must have FrameNumber: 1

Section 4.2 - Credits

I, donut3.5, wrote this entire tutorial from scratch.

Thanks to General Frank for the Geoset Animation tutorial.

Thanks to RDZ for looking over, proofreading, and adding tips to this tutorial.

Thanks to Wolverabid for cleaning and repairing this tutorial when it was in need.

Thanks to Chumbalum Soft, Oinkerwinkle, Guesst, Magos, and Blizzard for such excellent tools.

Feel free to contribute any information, ESPECIALLY on the nodes. Many fields were lacking. Anyone with decent knowledge on anything may report to me. I will add them to the credits if I feel the information helped, and they will get +Repped!

Report any mistakes and typos you see!

Happy Modeling!

--donut3.5--


 Last edited: Jun 6, 2008

#### Deleted member 126647

D

#### Deleted member 126647

Unfortunately I am unfamiliar with Milkshape, perhaps RDZ would be a better candidate for reviewing the Milkshape portions.

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Cowabunga. This tutorial is detailed, informative and nearly all-encompassing as an introduction to modeling.


As users assimilate the information included in the document, they are sure to become more proficient modelers, will create fewer new question threads, and will be able to help others answer many questions.


The eventual approval of this tutorial seems certain. I would like to get some more input from our best modelers and as they and all interested users) chime in, the FAQ section of the tut will surely become more powerful.

» REP DUMP «

[EDIT] Thread reported as containing spam content. ~ Thread cleaned, FAQ updated.


 Last edited: Mar 10, 2008

#### Kaitech[SanD]

#### Kaitech[SanD]

 Level 17

JoinedApr 11, 2007Messages1,816

Resources: 4

#### Resources
Skins 2Tutorials 2

Ok, I tried it through, it all works fine. I couldnt be bothered to change anything major so I changed the sheeps animes a tiny bit... Still, it works, I suggest it to any new modeller! I pointed out the 2 mistakes which you have now fixed so all is fine!

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

K, since everyone seems to love it, I've approved it.

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

Don't triple post please.

Requests go in the "Requests" section.

I do not take requests anymore.

--donut3.5--

#### Kamikazzee

#### Kamikazzee

 Level 6

JoinedJan 15, 2005Messages188

Resources: 1

#### Resources
Maps 1

donut3.5 said:

 Indeed, he recently changed his URL.


Try, however looking in the tools section.


--donut3.5--


Click to expand...


Ah, I got it... but new question:


I tried importing a .mdl AND .mdx file into MS3D, but neither worked. I used "Open With..." and also the import options for anything I could find but nothing opened the model. Care to give a better example on how to get a model into MS3D for animations?

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

There should be an option under import for Warcraft 3 MDX imports.


Put this into your Milkshape 3D programs folder:

View attachment msWar3MDXImporter.zip


Tell me if that works.


--donut3.5--

#### Deathcom3s

#### Deathcom3s

 Level 15

JoinedDec 24, 2007Messages1,403

Resources: 2

#### Resources
Models 1Icons 1

If you're still having problems with importing a model into MS3D, here's how:


To get models into MS3D they must first be in the .mdx format, NOT .mdl format. To import a file into MS3D simply go to file>Import and then way down at the bottom of the list of game formats is Wc3, I think it's the second from the bottom, that opens up a window. Just click on >> to browse for the model, maybe something like this: C/local disk/Warcraft3/Models or something like that.


Hope that helped,


~Deathcom3s

#### ASDnametaken

A

#### ASDnametaken

Exporter problem

Hi

Theres no exporter in File->Export-> ,got both msWcMdlExporter.dll and msZero.dll files in Milkshape main folder. I got Milkshape versio 1.7 does it work with this versio ??? or do i have to do something before it shows up there?

sry bad english, not native.

EDIT: It works only? with Milkshape 3D 1.8.2


 Last edited by a moderator: Aug 9, 2008

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

it works, you just have to scroll, keep your mouse on the bottom of the exporter list and it should appear at once.

#### Kingzachery

#### Kingzachery

 Level 5

JoinedOct 6, 2007Messages54

Resources: 13

#### Resources
Models 12Maps 1

I can't downlode Magos Model Editor the Link is nonexistent.


can some one help me Because i have ben trying to mack models for 6 months now and i can't get the info or tools i need the start to lern.

#### ASDnametaken

A

#### ASDnametaken

hmm

HappyTauren said:

 it works, you just have to scroll, keep your mouse on the bottom of the exporter list and it should appear at once.


Click to expand...

I tried to keep mouse there... nothin happens.. and there no "elevator" so scroll doesnt do anything either, i try to add screen shot attachment to this post.

Edit: maybe theres too many exporters ?? i try to temporarily remove some from there.

EDIT: It works only? with Milkshape 3D 1.8.2

#### Attachments

Milkshape_problem.JPG

 88.4 KB

 · Views: 6,673




 Last edited by a moderator: Aug 9, 2008

#### nickex

#### nickex

 Level 5

JoinedApr 12, 2008Messages57

Resources: 3

#### Resources
Skins 2Maps 1

Trial

too bad milkshape only has 30 day trial ...

#### Kwaliti

#### Kwaliti

 Level 37

JoinedJun 7, 2007Messages2,394

Resources: 31

#### Resources
Models 13Icons 5Skins 7User Interface 3Tutorials 3

Buy it, perhaps?

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

It is likely it was downloaded at an earlier time. Sorry, there's not much you can do. If you want to just test it out, by all means, grab a box and just check out animations/mesh building. It wont, however, get you any final products.

--donut3.5--

#### Verdun

#### Verdun

 Level 16

JoinedJun 9, 2008Messages350

Resources: 11

#### Resources
Models 11

##
 Shortlegs


nickex said:

 too bad milkshape only has 30 day trial ...


Click to expand...


Well, that's just it. I seem to have a 0 day trial.


I've just installed it on my computer, making sure to chose "free trial version 30 days", and still it doesn't want me to import or to save anything, for I haven't registered yet (WTF?!)


any suggestions?

#### Verdun

#### Verdun

 Level 16

JoinedJun 9, 2008Messages350

Resources: 11

#### Resources
Models 11

##
 Shortlegs


It's me again


well the problem just got away after a week, don't ask me why.


Anyhow, I have another question: How do you get the original WarcraftIII model files? For now I have only edited models I have downloaded.

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

They can be accessed from any MPQ opener. Magos Model Editor works just fine.

--donut3.5--

#### Sippe

S

#### Sippe

I dont get it My friend sayed that i need these programs to make models .

-Magos

-Vertex tool

-Gimp

-MilkShape 3d and now i have it i just dont undestand this and i only want to make one piece models..

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

How about reading a tutorial instead of asking such questions?

#### Sippe

S

#### Sippe

HappyTauren said:

 How about reading a tutorial instead of asking such questions?


Click to expand...


Thats the problem i dont get those tutorials there is so many things there and that i only wanna learn how to make models that they look like anime/manga characters and there isnt enything to where to start and what to do first....

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Err. Dude. To model a full animated character, you need shitload of knowledge. And it is all here. So, read it. You can't model all of sudden either, you need something called PRACTICE.

#### Tarandan

T

#### Tarandan

those i really need those programs i have photoshop can use it?


 Last edited by a moderator: Jul 20, 2008

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

Photoshop is an image manipulating program, not a 3D modeling tool.

--donut3.5--

#### blackricc

#### blackricc

 Level 5

JoinedJul 6, 2008Messages65

Resources: 2

#### Resources
Maps 2

i need help i dont get the programms to work could sommone tell me exactly how to make War3ModelEditor work cze when i try to start it it ust comes up a grey squere.

as a start i need help with the others to but could sommone help me start with this?

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

What about learning to read, and make sense?

First, it DOES start as a gray square, cause that is PREVIEWER window. All others are found under window menu. Also, you want someone to basically TEACH you how to do something on the INTERNET? That is just wrong, man.

#### Frost_Mourne

#### Frost_Mourne

 Level 2

JoinedApr 15, 2008Messages25

I've tried to import a model into milkshape3D but every time i tell it to open a WC3 model and click ok it closes out. I read about the file in the folder thing earlier but I have it in the file, am i missing something? i'm going to re-read just in case but if anyone knows whats going on please tell me

#### Softy

#### Softy

 Level 5

JoinedMar 27, 2008Messages107

what program is magos material manager? ;o i've searched for it alot but cant seem to find it

#### Softy

#### Softy

 Level 5

JoinedMar 27, 2008Messages107

Frost_Mourne said:

 I've tried to import a model into milkshape3D but every time i tell it to open a WC3 model and click ok it closes out. I read about the file in the folder thing earlier but I have it in the file, am i missing something? i'm going to re-read just in case but if anyone knows whats going on please tell me

Click to expand...


are u doing like this? "click import then find Warcraft 3 MDX, far down in the list, then find ur MDX model wherever u've saved it" if u did like this and that doesnt work im afraid i dunno why its not working m8

#### SuperCow

#### SuperCow

 Level 6

JoinedAug 27, 2008Messages177

Resources: 1

#### Resources
Maps 1

The tools don't work

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Cool Ways to Kill Yourself

#### Newuser

#### Newuser

 Level 6

JoinedJun 14, 2008Messages176

Resources: 1

#### Resources
Maps 1

If I wanted to edit a model, and, say a peasant for example, and edit the attack animation so instead of attacking with only one hand, he attacks with both hands(lets say, I add weapon attachment to both hands so he has 2 weapons).


I would need some program like Milkshape right, but does anyone know if there are free programs(well, trial is fine too, as long as it's not Milkshape)?


My Milkshape trial expired, bad thing is, is that I learned how to start editing skeletons of models to make animations AFTER the trial version expired.

#### Linaze

#### Linaze

 Level 24

JoinedFeb 28, 2007Messages3,478

Resources: 4

#### Resources
Models 1Tutorials 3

This is probably one of the most useful tutorials about modeling that there is, it covers the most stuff you will need to know when beginning modeling for warcraft 3. It is also easy to follow, things are well described, tools are linked good. I see no reason to say anything negative on this tutorial.

#### jadegolem

#### jadegolem

 Level 7

JoinedFeb 1, 2008Messages295

Resources: 2

#### Resources
Models 1Maps 1

.... isnt milkshake 3d for half life? half life and half life 2 use mdl and mdx files i think

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

It is most popularly used for half life, however there are exporters to other games available, such as an .mdx exporter for WC3.

--donut3.5--

#### SuperCow

#### SuperCow

 Level 6

JoinedAug 27, 2008Messages177

Resources: 1

#### Resources
Maps 1

When i'm trying to open vertex modifyer and geoset merger i get an error.


''component 'COMDLG32.OCX' or one of its dependencies not correctly registered: a file is missing or invalid''


Can anyone help me to fix this or tell me what's wrong?


would be very happy then, i rly want to learn how to make models

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Dude. Ever heard of Google You just need to download that file and put it in system32.

#### SuperCow

#### SuperCow

 Level 6

JoinedAug 27, 2008Messages177

Resources: 1

#### Resources
Maps 1

well, i've done that but it don't work, so i am not sure what to do

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

If you google COMDLG32.OCX, you should be able to find a link to download it. Put that in this location:

C:\WINDOWS\system32

Everything should work fine from there.

--donut3.5--

#### SuperCow

#### SuperCow

 Level 6

JoinedAug 27, 2008Messages177

Resources: 1

#### Resources
Maps 1

wll, it don't work

#### Tyreal.F.E.A.R.

#### Tyreal.F.E.A.R.

 Level 2

JoinedSep 22, 2008Messages21

im sorry but ive been reading this for info and happy taurein cool ways to kill ure self totally fed up until i got to the end funny and perfect for how you used it and if i may STFU whiny bitches google what you want like import .mdx into milkshape3d its not hard or like how to climb the white house in a wheel chair come on how hard is this?

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

happy taurein is the cause of all trouble.

#### donut3.5

#### donut3.5

 Level 28

JoinedFeb 22, 2006Messages3,051

Resources: 32

#### Resources
Models 20Icons 10Maps 1Tutorials 1

Tyreal.F.E.A.R. said:

 im sorry but ive been reading this for info and happy taurein cool ways to kill ure self totally fed up until i got to the end funny and perfect for how you used it and if i may STFU whiny bitches google what you want like import .mdx into milkshape3d its not hard or like how to climb the white house in a wheel chair come on how hard is this?


Click to expand...

I tried to translate the post above, but this is the best I could do:

Tyreal.F.E.A.R. said:

Click to expand...

(replace "picture" with "post")

--donut3.5--

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

Yeah, looks like some words, piled up.

#### SABREWULF

#### SABREWULF

 Level 8

JoinedJul 29, 2006Messages173

Resources: 2

#### Resources
Maps 1Spells and Systems 1

who of u guys have the best knpwledge about program Milkshape?

Super Cow i know your problem i think u must to delete warcraft 3 and install it once again and the problem will dispere. Try

#### Zorga

#### Zorga

 Level 7

JoinedSep 7, 2006Messages97

donut3.5 said:

 I tried to translate the post above, but this is the best I could do:


(replace "picture" with "post")


--donut3.5--


Click to expand...


HAhahahaha! Great tutorial btw!

#### 1FarmRanger

#### 1FarmRanger

 Level 3

JoinedDec 22, 2008Messages25

I get an error I can't open the .exe file -.- it says : Unable to open "d:\program files\warcraft 3\war3.mpq"!


Make sure that wacraft 3 is installed and that the registry key"Hkey_current_user\blizzard Entertainment\warcraft III\install path"exists


BUT GGC works no the D:\ and i tried downloading it 4 times :/

#### LSI27

L

#### LSI27

i can't get to guesst's exporter. the link times out.

anyone know where to find it or an equivalent?

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Mmm, in the particles part you should add something about "Model Space", and "XY Quad" and "Line Emitter".

(If you want a non-billboarded particle where the planes are facing upwards, you need XY Quad and Line Emitter checked. Both of them.)

Status
 Not open for further replies.


### Similar threads

 Mdlvis: model edition/creation, animation and UV Mapping


jk2pach

Sep 20, 2010

3D Modeling Tutorials

Replies13Views88K

Mar 2, 2020
wisanggeni

W

 Model Editing


Fabioz

Jan 18, 2009

3D Modeling Tutorials

Replies45Views66K

Oct 10, 2021
KaiZen7th

 Putting what you learned about Animated Textures into action


Spacebuns

Dec 15, 2015

3D Modeling Tutorials

Replies3Views4K

Jun 29, 2026
Spacebuns

 In-depth Animation Tutorial [3ds max]


BlinkBoy

Apr 11, 2009

3D Modeling Tutorials
23

Replies123Views153K

Feb 8, 2017
Kazeon

 Fixing Faulty Death / Decay Animations


General Frank

Sep 30, 2007

3D Modeling Tutorials

Replies21Views19K

Sep 11, 2021
Liu

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