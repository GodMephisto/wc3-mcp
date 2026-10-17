---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/exporting-models-from-blender-using-the-mdl-exporter-plugin.308012/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Exporting Models from Blender Using the MDL Exporter Plugin
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/exporting-models-from-blender-using-the-mdl-exporter-plugin.308012/>
#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

Creating and Exporting Warcraft MDL models with Blender

By Fingolfin

Have you ever wanted to create your own awesome 3D models for Warcraft 3? Up until now, the only officially supported tool has been 3ds Max5 with Blizzard's Art Tools plugin, which is problematic considering that this version of 3ds Max is over 15 years old and very hard to get your hands on. Another awesome plugin - NeoDex - has been made by @BlinkBoy to support newer versions of 3ds Max, but this has also been limited to users who are either willing to pay upwards to 3000€ a year, have a student license, or are unscrupulous enough to download it illegally. Other tools like MDLVis and Vertex Modify have helped produce a lot of appreciated content over the years, but creating models in these is tedious since they can't leverage the great variety of tools provided by proper 3D-modeling software.

For this reason, i started developing an MDL exporter plugin for Blender, which although still a work in progress, is now ready to see some use. Blender is an amazing, free, open-source 3D art program with great capabilities, and learning it can be useful for more purposes than just Warcraft modding. I this tutorial i aim to teach you how it works and how you can use it to export cool 3D models for Warcraft 3.

For this tutorial, you will need the following programs:

Blender:blender.org - Home of the Blender project - Free and Open 3D Creation Software

Blender MDL Exporter Plugin:khalv/mdl-exporter

MDLX Converter:MDLX Converter

1. Getting Started

Once you have installed Blender, visit the github page for the MDL exporter. The readme contains most of the instructions covered in this tutorial and is good for quick reference. For now, just click the button that says "Download or Clone" and press "Download Zip".

Next, unzip it, and enter the folder so that you see the three files shown above. Right-click the folder called "export_mdl" and go to Send To>Compressed Folder. Open up Blender, go to File>User Preferences and press "Install Add-On from File..." in the bottom bar.

Select your newly created zip folder and restart Blender. If you go to File>Export, you should now see "Warcraft MDL" as an option. Note that the first time you install the plugin, you will need to activate it from the import/export plugins list by ckecking the box next to it.

By default, Blender uses right click for selection and left click for moving the 3D cursor. Since this is completely counter-intuitive to what you know from any other software, you might want to go to File>User Preferences, hit the "Input" tab, and set "Select With" to "Left Click".

2. Creating your model

Teaching all the aspects of creating a model in Blender is beyond the scope of this tutorial, but my recommendation is to check out BlenderGuru on youtube and start from there:

My plan is to later create a tutorial on 3D modeling techniques as well. For this tutorial, I'll make a simple wine glass without going too much in details on how to use the Blender UI - therefore i recommend that you at least watch the first two episodes of the series i linked before you start. If you already have a model which you want to export, you can skip this part.

To start off, press the A key to select everything, then press X for delete, and click to confirm. This leaves us with an empty scene. Next, hit Shift+A to open the "Create" menu, go into "Mesh" and select "Cylinder". Our cylinder will appear at the current location of the 3D cursor, which unless you have accidentally misclicked should be at the center. In the bottom left corner of your screen, there's now a small window where you can configure the parameters of our cylinder. Here, I'll set it to have 8 segments, a radius of 0.05 and a height of 0.2. Set "Cap Fill Type" to "Nothing" to get an empty tube. It can be helpful to configure Blender to use real-world units, you can find instructions on how to do so here.

To make it easier to create smooth shapes, we will be using a Subdivision Surface modifier, which essentially smooths the mesh while adding more polygons. Press the tab which looks like a wrench in the menu to the right, press "Add Modifier", and select "Subdivision Surface". You can also just press CTRL+1 to add it automatically. You will notice how the cylinder looks more smooth. To make it even smoother, you can press the little button which says "Smooth" in the middle of the left toolbar, under "Shading" - this makes the model look smooth by averaging the normals without actually adding any more geometry. You can also set an angle threshold for the smoothing by pressing "Auto Smooth" in the "Object Data" tab (which looks like a triangle).

With our cylinder selected, hit TAB to toggle edit mode. Press A to deselect everything, and then CTRL+R to enter "loop cut" mode. If you hold your cursor over the cylinder, you will now see a purple line dividing it. As you press, this line will become yellow and follow your cursor as you slide up and down. Move it to where you want and click again to place it. With the vertices selected, you can now scale them by pressing the S key and moving your cursor around. If you ever want to select the same loop again, you can hold the Alt key while clicking one of the edges to select the whole loop. Remember that the A key deselects everything (or selects everything, if nothing is selected). Put down some loops and scale them until you have something which looks like a glass. You can also press G (for "Grab") to move vertices around, and also press X, Y or Z to lock movement to that axis. Notice how the Subdivision Surface modifier smooths out the shape - if this creates smoothing where you don't want it, try adding an extra cut close to the edge you wish to preserve.

CTRL+RMB+DRAG: Paint a selection loop with your cursor. This is great for precision.

C: Circle select. Use the scroll wheel to shrink/grow the circle, and drag with LMB to "paint" selection with it. Press RMB to stop selecting.

B: Box select, lets you drag a box.

SHIFT+LMB: Add to selection.

ALT+LMB: Select loop.

And now we are pretty much done! If you want to create an inside for the glass, you can select the top loop (again, by ALT+clicking it) and press E and S to extrude it and scale inwards. Pressing F while a loop is selected will seal it off with a polygon, so you can use this to fill the bottom. Another final thing we might want to do is to move the model so that the base is at the origin point - do this by pressing G followed by Z (to lock to Z axis), move it up, and then click to place.

3. Unwrapping

Next, we need to map how the texture should be applied to the model in 2D space - think of it as unraveling a paper model and flattening it to fit onto a piece of paper. This process is called "Unwrapping".

Similar to how clothes have seams where the fabric is cut, we can add seams to define where to cut up our model when unwrapping it. In our case, this is as simple as ALT-clicking along one of the vertical lines of our glass and press CTRL+E (E is for Edge Options), and select "Mark as Seam" from the menu. If you have sealed off the inside and added a bottom cap in the previous step, you will also need to mark the loop around the polygon as a seam. Seams are marked with red.

Next, press A two times so that you have your entire mesh selected. Now press U and select Unwrap. It may not look as if anything has happened - but that's just because what we need to see is in a different UI screen. Notice the little cube icon at the bottom left of your window? Click it to open a drop-down menu with different screens, and select "UV/Image Editor" (hotkey for this is Shift+F10). Now you see how it looks when the model is unfolded! To get more overview, you can pull the little corner at the top left of the window to split it into two and have one as the UV editor and one as the model (you can press T and N in the UV editor window to toggle off both the toolbars if it gets cluttered). In the image editor, you can press the button which says "Open" to select an image to use as background - this way we can move the vertices around to match the texture we want to use. In this case, we will be using a plain white texture, so we won't go into the details of this.

Finally, hit TAB again to toggle back into object mode.

L - Select Linked

G - Grab

S - Scale

R - Rotate

Y - Separate

3. Material setup

In order to export properly, our model needs a material. Go to the Material tab in the properties window and press the button which says "New". At the bottom of the panel, some settings will appear labeled "MDL Material Settings". MDL Materials consist of one or several texture layers, which can be blended together for different effects. To create one, click on the "+" button next to the empty box which says "Layers". The property inspector below displays the properties of the currently selected layer - here, we want to enter "Textures\white.blp" as the path, "Additive" as filter mode, and make sure that "Two Sided" is checked. That's all we need for our basic glass material!

When you make your own models, you'll probably want to use other textures than "white.blp" - you can then use Magos Model Editor's MPQ browser to browse through and preview all the textures which exist in the game files. When right-clicking on a texture in the list, you can choose to "copy path to clipboard" and paste the path into the texture field in Blender.

You might notice that there's an option called "Texture Type" for the material - here you can choose whether to use an image as texture, a team color, or a replaceable texture (such as a cliff or tree texture). To add team color to a unit model, you create two layers - one bottom layer set to "Team Color", and one top layer with filter mode set to "Blend" and a texture path of your choice - the team color will then shine through where the texture is transparent (determined by its alpha channel). Note that the layer list goes from bottom to top, so layers at the top of the list are rendered under those below them in the list.

Some models, such as the Water Elemental, use texture animations to make it seem as if the water is moving across its body. The exporter allows you to make texture animations by adding a Mapping node to the node graph, naming it the same as your layer, and then animating its values. This way you can preview the animation by feeding it into the texture slot of your actual material (MDL materials are just data containers and won't affect how the mesh is rendered in blender). To edit the shader graph, open the graph editor in a window (hotkey: Shift+F3) and press the button which says "Use Nodes" in the material panel. You can create nodes by pressing Shift+A. You can rename nodes by selecting them and typing in a name in the panel to the left of the graph viewport.

4. Animated color tint

Warcraft 3 materials can have a tint which is applied as a vertex color to the whole mesh. In Blender, we can get the same effect by animating the "Object Color" of our glass. Our color won't actually be changing anything during the animation - but for the exporter to recognize it as a color tint, we need to apply it as an animation. First of all, in order to export animations, we need to define their start and end times. To do this, we mouse over the timeline at the bottom, and right click in it (or left, depending on your mouse settings) to move the cursor. Set it to frame 1 (or 0, whichever you prefer) and press M to place a timeline marker. Next, press CTRL+M to rename it - let's just name it "Stand". To mark the end of this animation, we move the timeline cursor to frame 100 and repeat the same process again - making sure to also name this one "Stand" (each animation NEEDS a start and end frame or the exporter will report an error). If you ever want to have multiple sequences, you can just place another pair of frames somewhere else in the timeline and name them something else - as long as they don't overlap with each other. Note that although the exporter will also export rotation, scale, and translation animations, this is currently buggy and might not always present the correct result. Most other properties can be animated, though - see the readme for more info.

The next step is to make our actual animation. To do this, go to the "Object" tab in the right toolbar (the icon looks like an orange box) and find a field which says "Object Color". We will change this field to be a light blue color. Then, with the timeline cursor at frame 1, mouse over the color field and hit I on your keyboard to insert a keyframe. You will notice that the color field now gets a yellow border - this means that its values are keyed at this frame. Move the timeline cursor to frame 100 (or whatever your end frame is) and repeat the process. Note that ALL animations need to have start and end frames to display properly - however, the exporter currently won't notify you of these are missing, so it's your responsibility to remember to add these.

On the topic of animations - if you ever need to animate whether you want to show or hide an object, you can easily do this by animating the render visibility. You do this by mousing over the little camera icon you see next to the object in the outliner (top right corner) - click the camera to toggle on/off, and press I while mousing over it to set a keyframe. It is also possible to create a custom property called "visibility" and render its value.

NOTE: As of the current version, animating render visibility is bugged, in the sense that the values are inverted. In other words, keying render visibility to "on" will result in the geoset being hidden, and vice versa. This will be fixed soon.

5. Exporting

When you feel done with the model, go to File>Export>Warcraft MDL and save it. Note that you have some options in the left panel:

Selected objects: if this box is checked, only selected objects will be exported.

Forward/Up: This is used to rotate the model as it is exported to correct for the different conventions of Warcraft 3 and Blender. By default, it should make it so that the forward view in Blender corresponds to the forward direction of Warcraft 3 (you can set the viewport to view "Forward" perspective by pressing 1 on your num pad).

Scale: This is used to apply a global scale to all objects in the scene. In Blender, one unit corresponds to one meter, while in Warcraft, it is a much smaller distance. To correct for this, the default value is set to 60, which is roughly half the height of Jaina (my reference), so this means that if you model a character to be 2 meters tall in Blender it will become about as tall as her (which is probably not her real height, but it's a nice round number).

Normally, there's no need to change these values, and it should be safe to simply export. If you encounter any errors in this process, please write them in the comments and i will try to address them. Finally, convert your model to MDX using MDLX for a smaller file size. Now you can admire your model in a MDL/X viewing software such as Magos Model Editor!

So... that's it! Have fun! I will update this tutorial further on as i update the plugin, make sure to keep the exported updated - you do this simply by checking if there's recent activity on the github page, and if so, just install it again using the steps described. Don't hesitate to ask questions in the comments or by PM.


 Last edited: Dec 19, 2018

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

Hmm... I am not at home right now but I will try this out when I am home. But I assume the exporting plugin can only export mesh data? Or can it export bone data and vertex groups?

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

It can export literally everything supported by the format, though animations are currently a bit dodgy. You should still be able to export a rigged model though and transfer animations to it. I recently successfully did some animating on a battleship model, so it works for basic hierarchies. Bezier interpolation is buggy though.

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

Well then, I've absolutely no idea why this wasn't more known! There's also a fairly recent MDX importer plugin for Blender somewhere on github. Finally, modeling on Blender for WC3 can be a possibility.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Super duper awesome!

Approved!

#### RanchBurner

#### RanchBurner

 Level 11

JoinedJul 3, 2010Messages242

Resources: 12

#### Resources
Skins 4Maps 1StarCraft II Resources 4Ported Skins 2Tutorials 1

About the materials setup, if it's of plain color, would we check "Unshaded"?

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

Unshaded is used when you want your material to appear glowing or self-illuminated. It prevents surfaces directed away from the light from appearing darker. For a glass I would not recommend using it. For particles, glows, or UI stuff (like the "silence" overhead buff) it is useful.

#### NightStalker

#### NightStalker

 Level 19

JoinedApr 21, 2013Messages1,194

Resources: 5

#### Resources
Icons 4Spells and Systems 1

I might start learning modelling for WC3, I loved this tutorial and thanks for working on such a project.

May I ask what the difference between MDL and MDX is. Can I use my MDL model in the game as a MDX model?

And as another question; let's say I made a MDL model rigged it and exported it with no animations, since you mentioned animations can be buggy with rotations and such, and create all animations on MDLVIS, and saved it. Will it be working as intended in the game?

EDIT: oh ignore the first question, I just saw the MDX MDL converter app!

NEW EDIT: Even when I follow the installment steps, the MDL export won't show up.

ANOTHER NEW EDIT: I had to find and enable it manually. Let's test with a simple model now!


 Last edited: Dec 19, 2018

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

Hey! Glad you liked it! To answer your questions:

MDL is a text-based format, while MDX is a binary format. Because of this, MDX files have much lower file size. As you found though, it is easy to convert between the formats using tools like MDLX.

Yes, you can export MDL files from Blender and animate them using MDLVis. For mechanical objects, you can typically use empties for bones, but for organic stuff you want to use an armature and skin the mesh to it. You can do this by selecting first your mesh, then shift-select the armature, and hit "CTRL+P>Armature deform with automatic weights". Now the vertices will correctly follow their bones when you pose them.

#### NightStalker

#### NightStalker

 Level 19

JoinedApr 21, 2013Messages1,194

Resources: 5

#### Resources
Icons 4Spells and Systems 1

Fingolfin said:

 Hey! Glad you liked it! To answer your questions:

MDL is a text-based format, while MDX is a binary format. Because of this, MDX files have much lower file size. As you found though, it is easy to convert between the formats using tools like MDLX.

Yes, you can export MDL files from Blender and animate them using MDLVis. For mechanical objects, you can typically use empties for bones, but for organic stuff you want to use an armature and skin the mesh to it. You can do this by selecting first your mesh, then shift-select the armature, and hit "CTRL+P>Armature deform with automatic weights". Now the vertices will correctly follow their bones when you pose them.

Click to expand...

Thanks a lot! I am so very new to modelling so I understood like only half of what you said, but I will be first trying to create a mechanical building which will be able to move. So in this case, my building will need a few moving parts, but not like arms or legs which can collide and cause deformations for the texture.

#### RanchBurner

#### RanchBurner

 Level 11

JoinedJul 3, 2010Messages242

Resources: 12

#### Resources
Skins 4Maps 1StarCraft II Resources 4Ported Skins 2Tutorials 1

Fingolfin said:

 Unshaded is used when you want your material to appear glowing or self-illuminated. It prevents surfaces directed away from the light from appearing darker. For a glass I would not recommend using it. For particles, glows, or UI stuff (like the "silence" overhead buff) it is useful.


Click to expand...

Well that answers another question.  I've also been trying to make glowing lightsabers.

#### sonphamthe11H

#### sonphamthe11H

 Level 6

JoinedMar 6, 2017Messages190

Will it support a option to choose export full bones or optimized?

#### sonphamthe11H

#### sonphamthe11H

 Level 6

JoinedMar 6, 2017Messages190

And it’ll be a tool for rendering animated animation from importing?

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

sonphamthe11H said:

 Will it support a option to choose export full bones or optimized?


Click to expand...

I have a plan for this, yes, optionally i will just remove the optimization since it is a bit hard to maintain that feature.

As for your other post, i don't know what you're trying to ask?

#### sonphamthe11H

#### sonphamthe11H

 Level 6

JoinedMar 6, 2017Messages190

U should google this word “dota2 model ripping”

We save a animation into a max file then render it

Idk how describe it exactly

#### sonphamthe11H

#### sonphamthe11H

 Level 6

JoinedMar 6, 2017Messages190

#### sonphamthe11H

#### sonphamthe11H

 Level 6

JoinedMar 6, 2017Messages190

Fingolfin said:

 I have a plan for this, yes, optionally i will just remove the optimization since it is a bit hard to maintain that feature.

As for your other post, i don't know what you're trying to ask?


Click to expand...

Fingolfin said:

 I have a plan for this, yes, optionally i will just remove the optimization since it is a bit hard to maintain that feature.

As for your other post, i don't know what you're trying to ask?


Click to expand...

more clear right?

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

Please don't double post, much less TRIPLE post. Press the edit button and add to your first post instead.

I am just confused with your wording. Do you mean importing wc3 models into Blender?

"Rendering" means generating a 2D image from a 3D model. Do you want to create cool 3D-rendered images of Wc3 models?

#### sonphamthe11H

#### sonphamthe11H

 Level 6

JoinedMar 6, 2017Messages190

yeah,importing

#### sonphamthe11H

#### sonphamthe11H

 Level 6

JoinedMar 6, 2017Messages190

why your exporter only export the translation but not the rotation and scaling which is messed up my model see the attachment

i'll prefer a option to export only the "rotation"

#### Attachments

buggy model.rar

 1.2 MB

 · Views: 493



#### sonphamthe11H

#### sonphamthe11H

 Level 6

JoinedMar 6, 2017Messages190

@Fingolfin

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

The current version has limited support for animations and cannot export euler rotations, only quaternions. There are plug-ins for converting animations between euler and quaternion.

The next version will support all animation types, I plan to release it as soon as i solve an issue I've been having.

#### sonphamthe11H

#### sonphamthe11H

 Level 6

JoinedMar 6, 2017Messages190

Updated but still messy

U can use my .blend file to test

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

Just tested it, and yeah, it does look messed up. I will look into this. I managed to get Hayates model to work though, so the issues might be due to the way the animations are imported from your source model. I noticed there are a lot of tracks, and a lot of frames for these tracks. There might also be issues with the conversion between euler and quaternion rotations - either way, i will see what i can do.

#### sonphamthe11H

#### sonphamthe11H

 Level 6

JoinedMar 6, 2017Messages190

could u edit the neodex 0.78b to export all the bones and their animations? IDK how to make it work

the problem is: trolboy's script put the animations into the rotations,not the scalings and translations

may this fix my problem: make it able to choose export the scalings or rotations or translations


 Last edited: Jan 3, 2019

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

I have no idea what you are talking about, also, please don't double post, use the edit button.

I am not responsible for the NeoDex plugin and i wouldn't know how to edit it even if i wanted to.

#### sonphamthe11H

#### sonphamthe11H

 Level 6

JoinedMar 6, 2017Messages190

I mean,make the option to choose what animation type they want to keep “rotation” or “translation” or “scaling”

If u can make it animate only in the rotation,the model’ll lighter

#### Ljusdahl

#### Ljusdahl

 Level 6

JoinedApr 28, 2005Messages156

Resources: 1

#### Resources
Maps 1

Thanks for your awesome tool!

I'm trying to make a simple glow effect. The full billboarding box is ticked as instructed. The plane is offset from its bone, so that it doesn't clip through the model it's attached to. But it seems like it doesn't look straight at the camera, but rather has a parallel sight line(visible on the peasant to the right). Is that how it's supposed to be?

#### Attachments

billboarding.jpg

 324.5 KB

 · Views: 284



BB_Glow.mdl

 3.6 KB

 · Views: 177



#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

I'm not really sure what you've done here, though i noticed that your pivot point is not centered... is the bone also offset from the center? If you want to get the effect you want, the bone has to be positioned at the point you want it to pivot around (i'm guessing the center), and the plane offset from it strictly in the forwards direction. Another option is to make two bones - one at the center of the attachment, and one at the center of the glow - parent the glow to this bone, then parent the glow bone to the root bone, and set them both to billboard.

By the way, your name is very reminiscent of that of my hometown... coincidence??

#### Ljusdahl

#### Ljusdahl

 Level 6

JoinedApr 28, 2005Messages156

Resources: 1

#### Resources
Maps 1

It's only off-center on the Z-axis, but I just tried centering it completely and the same problem is still there.

The plane is offset in the forward direction.

My nickname was picked at random many years ago, I' have no connection to the town. Cue X-Files theme song?

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

Hmm, strange... can you send me the .blend file? Also, i noticed from the file that you have used a cube and just flattened it. You might want to use a plane primitive instead.

I made my own attempt at this, does it behave the same in-game?

#### Attachments

BillboardGlow.blend

 499.6 KB

 · Views: 208



BillboardGlow.mdl

 1.5 KB

 · Views: 195



#### Ljusdahl

#### Ljusdahl

 Level 6

JoinedApr 28, 2005Messages156

Resources: 1

#### Resources
Maps 1

I've messed around too much with the blender file since then, but I looked at yours and yes, that's what I did. I tested your .mdl and it has the same issue in-game.

So I guess it's just the way Wc3 does billboarding. It would've been less noticeable back when it didn't support widescreen.

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

One thing you can try is to place the glow at the origin, but check the box called "no depth test" in the material layer. This will make it draw as overlay... Then again, it will also draw over everything else too.

#### Ljusdahl

#### Ljusdahl

 Level 6

JoinedApr 28, 2005Messages156

Resources: 1

#### Resources
Maps 1

Yep, that compromise worked!

Are you happy to help with more little issues via PM's, or is it time to stop bugging you?

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

PM away! I'm glad to help.

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

Hey@sonphamthe11H, i found the issue that was screwing up your export and i have fixed it. The problem was that i was converting Euler rotations to radians, even though they were already stored as radians internally (Blender just views them as degrees). The animation looks almost right now, but there is a bug with his weapon. You can get the latest version off github if you want to try and see if you can fix it. The file still becomes pretty large though due to the large amount of keyframes (2mb).

#### sonphamthe11H

#### sonphamthe11H

 Level 6

JoinedMar 6, 2017Messages190

idk how,i'm just waiting for the update

#### GriMw0lf

#### GriMw0lf

 Level 1

JoinedJan 17, 2019Messages6

Hey @Fingolfin , I've been using your blender add-on (Awesome job, by the way!) and it seems the animations aren't working properly once exported. I'm using Quaternion rotation, so as far as I know it should work. I don't have a whole lot of animation experience with Blender, so I was hoping to get some advice.

Essentially the issue seems to be that the animations overshoot massively, both for rotations and translations. Any advice as to how to fix this issue?

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

@GriMw0lf Are you using the most recent version? I animated stuff yesterday and it worked like a charm. Can you send me the blend file?

#### GriMw0lf

#### GriMw0lf

 Level 1

JoinedJan 17, 2019Messages6

@Fingolfin I pulled it from Github on the 17th, so I believe it's the newest version.

That being said, here is the blender file, mdl file, and blp file. I was testing out the animations.

Thank you for the help, by the way!

#### Attachments

Broly.blend

 1.1 MB

 · Views: 243



broly.mdl

 373.9 KB

 · Views: 201



BrolyBase.blp

 128.6 KB

 · Views: 200



#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

Hmm, yeah, that does look weird. My best guess is that it is related to the scale of the armature, it currently has a scale of 0.025. When i applied its scale, the animation in Blender started looking the same as in the MDL file. If it's not too much hassle, try applying the scale (or create a new armature) and redo the animation like that.

EDIT: Just pushed an update which adds support for armature scaling, and it does fix some of your issues. However, there is still an issue where the arms rotate inwards in the stand animation. I am not sure what causes this, though i will look closer into it tomorrow. At first i thought it was because your armature was rotated, but i tried applying its rotation with no effect.

EDIT2: If anything though, now that i think of it: the exporter uses the pose at frame 0 as reference pose and converts all rotations to be relative to this. it is possible that this causes some problems since your animation starts, with his arms rotated and keyed, at frame 0. Can you try creating a rotation frame for each bone at frame 0, where the character is in his rest pose ("T-pose"), and then just start his regular animations at some other frame? Your sequences can start and end wherever you want. You can move keyframes easily in the action editor.

Also btw, Euler rotations are supported now also, they are automatically converted to Quaternions, so you don't need to worry about that.


 Last edited: Jan 20, 2019

#### GriMw0lf

#### GriMw0lf

 Level 1

JoinedJan 17, 2019Messages6

@Fingolfin Thank you, it worked! That was pretty damn quick, I appreciate it.

I looked through your code a bit and I noticed you've been working on this since at least April, that's a lot of dedication, so good job! You're a legend man, this plug-in is a real lifesaver.

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

Thanks a lot! Initially, it was mostly for myself, and for the fun problem solving and programming. I swapped from 3ds Max to Blender a couple of years ago, and now that I've gotten used to the fast, hotkey-based workflow of Blender, i can't stand working in Max anymore... so i basically had to write a plugin for it (even though had never programmed in python before, haha). It's been a struggle, but seeing people make use of it makes it all worth it! Good luck, and just let me know if you have any more issues!

#### GriMw0lf

#### GriMw0lf

 Level 1

JoinedJan 17, 2019Messages6

Thankfully Python isn't too difficult to learn, would have been a lot worse if Blender had used C++ for its plugins.

That being said, I don't know Python but if you ever need any help with C#, Go, or JS/TS (I work as a full-time dev) let me know and I'd be happy to help, it's the least I could do to repay you for this awesome plugin you've made!

If I find any issues with the exporter, I'll make sure to let you know.

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

Thanks for the offer, but I am a full-time C# dev myself, so I think I'll be fine on that front! Feel free to give a shoutout to this plugin if you ever upload a model made with it though, it would be much appreciated.

#### Astrella

#### Astrella

 Level 8

JoinedOct 3, 2008Messages189

Resources: 1

#### Resources
Maps 1

Would it be feasible to make your model in Blender; export it and then animate it using Mdlvis?

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

Yes! I like to think that the current animation support is at least as good as that of MDLVis, though.

#### Astrella

#### Astrella

 Level 8

JoinedOct 3, 2008Messages189

Resources: 1

#### Resources
Maps 1

Alright; thank you. I'm not really sure what I'm going to do yet cause I'm new with this. Just wanted to know my options. I'm very grateful for you making this plugin though cause I feel Blender probably has wider use than the made-for-Warcraft tools handy as they might be so will definitely give both options a go.

#### NightStalker

#### NightStalker

 Level 19

JoinedApr 21, 2013Messages1,194

Resources: 5

#### Resources
Icons 4Spells and Systems 1

Hey Fingolfin, the plug in worked flawlessly for my model. Just a quick question though! It is from my lack of knowledge; I mapped each and every nook n cranny of my spaceship model, they show on the model as I want it to be. But I don't know how to export each texture separately with the UV layouts. When I select all and try to export the layout all I get is a blank background and many lines depicting my vertices.

So question is; how can i export the layouts easily without re wrapping them one by one for each part, so that the Magos Model Editor can detect the textures as well? Right now Magos sees it as just a solid spaceship with no textures.

#### Fingolfin

#### Fingolfin

 Level 47

JoinedJan 11, 2009Messages3,411

Resources: 168

#### Resources
Models 156Icons 1Packs 4Skins 2Maps 1Spells and Systems 3Tutorials 1

Hey, glad it works for you! What you need to do with the textures is to assign a material for each part which you want to have a separate texture for. It is possible to have multiple materials on the same mesh and assign them to different faces by selecting them in edit mode and pressing "assign" with the material selected. Then, you need to set up the MDL data for each material in the bottom of the panel where it says "MDL Material Settings", as described in the manual. Make sure to set your texture paths correctly in the material layer. Finally, you need to use a BLP converter to convert your textures and place them in the same folder where you export your model. If you export it and open it in Magos, it should now display the textures.

Also note that in the image/uv editor, you can go to UV>EXPORT UV LAYOUT to get an image with your uv layout to base your texture on - just remember to set opacity to 1. There is also a plugin called "texture atlas" which lets you unwrap multiple meshes at the same time.


 Last edited: Feb 14, 2019

 You must log in or register to reply here.


### Similar threads

Sticky

 MDL Exporter for Blender3D!


Fingolfin

Aug 26, 2018

Modeling & Animation
234

Replies156Views44K

Mar 12, 2026
Chucky

Locked

 Blender 2.83+ MDL/MDX importer/exporter


twilac

Nov 9, 2020

Warcraft Editing Tools
2

Replies53Views42K

Jul 11, 2023
Nyctaeus

Locked

 Blender 2.6 Import/Export Plugin


Killabong

Nov 6, 2011

Warcraft Editing Tools

Replies26Views24K

Jul 10, 2016
RanchBurner

Locked

 Blender modeling export tutorial request


Alince

Nov 24, 2014

Requests

Replies1Views2K

Nov 27, 2014
TinkerLord

A

Locked

 [Magos][Blender] Export/Import ms3d gone wrong


Ardenian

Apr 10, 2016

Warcraft Editing Tools

Replies4Views3K

Apr 23, 2016
Ardenian

A

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