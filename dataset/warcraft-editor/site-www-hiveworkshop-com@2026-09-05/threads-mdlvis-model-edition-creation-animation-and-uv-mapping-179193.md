---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/mdlvis-model-edition-creation-animation-and-uv-mapping.179193/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Mdlvis Model Edition Creation Animation and Uv Mapping
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/mdlvis-model-edition-creation-animation-and-uv-mapping.179193/>
#### jk2pach

#### jk2pach

 Level 21

JoinedMar 19, 2009Messages444

Resources: 88

#### Resources
Models 78Packs 1Maps 7Tutorials 2

How to use Mdlvis

#
 I/  Introduction

a)  Generalities


Mdlvis is a free tool to create models for Warcraft III ... and it is probably the most comprehensive and the most buggy version in English.


Here's the download link:

http://www.trishroom.com/threads/mdlvis-english-full-easy-install-archive.30150/

View attachment Mdlvis.zip


This software has the following functions:


-vertex edition (clouds of points forming a model)


-Application of textures (UV Mapping)


-Changing GeoSets (vertex groups)


-Creating and Editing Bones (vertex arrays connected to a center of gravity, bone)


Designing and editing sequence (animation)


- models optimisation (weight reduction).


In fact, combined with War3ModelEditor he can do everything . War3ModelEditor with Mdlvis is essential for the following things: creation of particles, easier use of the hierarchy of bones (interdependence of bones between them), Geoset importation (MdlVIs allows to create from the existing vertex, but no importing).


Note the English version is quite bugged for the display mode.


Technique to permanently retain: to avoid blinking textures at times, where the amendment to a vertex / set of points is visible (it tends not to appear instantaneously), the simplest thing is to achieve a Right click with the mouse: one, by default, moves the camera observation model and solves the bug

b) This tutorial


Did not aim to show you everything, because the software is kind of "powerful" and I just want to help to understand the base.


Also, for you to discover MdlVis, I rely on simple examples. We will first try to add a nice attack animation to the model of the Assassin:


Units\Creeps\Assassin\Assassin.mdx


Use any MPQ reader to extract this model.

#
 II/  Discovering Mdlvis


Do not run away, this tool looks really weird, but usefull.


Nothing hard : we will open our model by clicking on the folder icon or File+Open, or CTRL+O, like in any other tool. There, we open our Assassin model.


You should get this :


Okay, a quick description


The menu File, like in any tool, allows to save, load , open a file etc…


The menu Edition allow to select/copy/paste/cut objects. Warning, Mdlvis allows to modify anything, so use those shortcuts carefully. For example, when you try to cut a cloud of points, you may get an entire geoset without seing it.


Please not that Copy-Paste does work for the frames (animations), but we will describe them later.


The third menu, Aèä on my computer (a minor bug) should be “Display” or “View”, I do not know.


The option « small grids » allows to select a display mode, depending on the following options : XZ, YZ, XY Grid, complete grid. I never use this mode to avoid to destroy my little head, but it is usefull for simetra works or precision works.


The option “Axis” allows to display the axis for width, length and depth. The X axis is red, Y green, Z blue. I suggest you always leave them turned on to see which side of your model you work.


The option “surface” allows to work with a sense of relief in the model, but without displaying the textures of models.


The option “Textured View” allows you to easily work on the model using the textures of the model. Indeed MdlVis detects mpq archive materials on Warcraft III, which is quite handy.


The option "Normals" can show the direction of lights and shadows from each vertex. Tip: Never use any tools related to the Normals with Mdlvis, they are particularly buggy.


The Modules menu allows you to switch between the Vertex Editor, ie the tool that lets you change the structure of a model, and the submenu Sequence Editor, which will then help you take care of animations.


In between, we have, by default not available, the submenu UV Mapping. In fact it will only be available when we have selected points.


Optimization menu, very useful but very dangerous, is only available when working on the Vertex Editor (it disappears when you are in the animation editor). "Optimize" is an action that will make the list of default actions that you find in the menu "Optimizer ..." Canonization is shorthand for a particular action. I suggest you always go through the menu ... Optimizer 'for what you do. An other tip: Always back up the non-optimized model with another name in order to get a backup. Optimization, if it saves up to 50% less space on a basic model of Warcraft III (and more when it was purified), sometimes change things which were not expected. We learn its effects empirically. The descriptions are clear, but succinct.


Let’s go on. You can see the 4 little buttons.


Quite simply, they allow to vary the view.


The arrow (Work Mode) can return to work model, while the Zoom, Rotate and Move can move the camera around the model without having to pay attention to the ongoing work.


Moreover, to the left of these buttons, you see the menu "perspective." It allows you to quickly center the camera in one way or another.


This menu is very useful because it is extremely difficult to work in 3D without congealing on a plane, because the display of MdlVis is buggy.


The display of geosets. Simple enough. The Show All option displays all geosets a model. A geoset is a grouping of points that we decided to hold together for various reasons (eg texture). Then you see check boxes with numbers, there are 4, one by geoset, actually. So if you click on one, you will see vertex of the first geoset.


If you uncheck it, you'll simply see the forms of the model, without vertex. And if you uncheck Show all ... You will not see anything.


Under the circumstances, it will be easier to work on a single geoset, or seeing others in the background to maintain consistency of a model etc. ... Each display mode has its pros and its cons.

#
 III/  Vertex Modification


Well. Just for practice, we will remove the cloak of the assassin and enlarge his shoulder.


The cloack : we open the vertex editor if it is not already on it, and choose to display only the first Geoset.


We select what looks like the cloak. I am in “Right” perspective, to look the side of the unit.


I went through the persectives "Right" to see select what looks like the cloack the profile unit.


You'll notice that new options appeared on the menu circled in the screenshot above.


Before you remove the cloak, simply by pressing the button on your keyboard Supr (surprising huh?), Quick description of the options.


Selecting : allows to select a cloud of points. We can add points to a current selection by pressing the key “Shift”.


Move : it will allow to move the selected points.


Very important note: you may notice that once a group of points is selected, a new symbol appears on the model displayed (green circle on the diagram)


Tip: when you want to move a cloud of points, place your mouse with precision like in this screenshot. There, we are in Right perspective. If we would like to move the selected cloack (the vertex displayed in red), it would be better to place the mouse to the top left of the gravity center of the selected points. Through the example, it’s top right (i.e X and Z greater than 0 on the new axis grid)


 So why? Because otherwise, you will move the group of points through an other axis, at the moment invisible, the Y axis (width). It means that without seing it, you will move the cloak.


Mdlvis indeed


En effet, Mdlvis understood on what axis of the space we want to work, depending on the position of the mouse compared to the gravity center of the selected points. It’s kinda hard to handdle when you discover the tool. In our example, if we had been in “Left” perspective, it would have been better to place the move to the top left of the gravity center.


Rotate : it allows to … rotate the selected points arround the gravity center. Take care to work in 2D (defined perspectives); it allows to avoid to rotate the model on more than 1 axis.


Zoom: it raises or reduces the size of a cloud of points with the gravity center as origin. Warn, contrary to the other actions, it will works on the 3 axis at the same time.


Rotate normal : In theory, it works, it allows changing the orientation of light on the walls of the model. In practice, if you do it, I will not assume what it does. I almost always get a corrupted model after this, and War3 Model Editor is no more able to read it.


Delete : simple, it allows to delete the selected points.


Uncouple : should be used for a confirmed user. It allows to separate a 3D selection into several planes.


Mirror Verticles in Workplane : you may not use this a lot. It’s usefull to reverse selections.


Extrude : it allows to create a 3D object by extending a 2D selection to a 3D model. Really usefull, even if it is not easy to redo the textures after this.


Detach as a new geoset : one of my favorite actions, since I hate work on models with too much points on the same view. It will simply separte the selection from the current geoset and create a new one with the selection.


Create Triangle et Delete Triangle : well, I do not know . It is supposed to create a vertex from scratch, but I never achieved this.


Collapse : quite usefull when we get useless vertex (such as a box whose faces are not connected) since it allows to fusion several points in a unic one. Generally, the new point will not be to the coordonates you want, because Mdlvis create the collapsed point at the position of the center of gravity of the selection.


Weld , Reverse Normals, Average Normals, Delete normals modifiers : same that for Rotate normals : I advice you to avoid these actions, since Mdlvis and vertex usage seems buggy.


UV-Map : it’s quite simple. It will display the texture attached to the geoset, and the way the selected points are placed on the texture. It’s quite usefull when we want to use ingame textures, to reduce the size of the model and set properly the textures.


Take care, contrary to Vertex Modifier, Mdlvis modify in real time the UV-Maps, so it will be hard to cancel any change.


That were the main actions. Okay, let’s go back to our cloak. We press the del key, in order to clean the model (and then enlight it, since the biggest part of a model is the vertex).


So, now, our assassin is without cloak.


In order to end the presentation of the Vertex Editor, we will raise the size of the spaulders.


They are in th geoset 3. I suggest to select “Front” perspective, and to select the two spaulders, and, finally, to use the scale action to enlarge them.


All of this has this unic objective to show you how to modify a model. It’s up to you to discover all you can do (anything except the vertex creation from scratch). To sum up, all will depend about what you want to do.


What I generally do is to register the model and to see it through War3 Model Editor, since this tool is more stable and better to watch the final rendering (especially for animations).


We get now this :


So Mdlvis is usefull since it allows to let complete tools like oinkerwinkle, powerfull but separated in X subtools, or to escape to learn to use 3DSMAX, expansive, requiring a lot of CPU Power, and hard to handdle.


It is now time to go on what Mdlvis can do the best (more than any other free tool for Wacraft III), the animations.

#
 IV/  Sequence Editor


Sequence Editor


There we go. Open the sequence editor.


You arrive there :


The defaut menu opened is : « anims »


Quick introduce :

a)  Menu Anims


A new menu appears near the classic menus, called « Frames ».


A frame is a concept in animation. Animations of the models are the same than the ones of cartoons. It is image after image, and the animation is only the reading of all images, creating the illusion of the movement. Do not worry, Mdlvis is enough powerfull to avoid you to have to make each frame. I mean the tool will calculate the moves to do between two frames you would have created. (Even if it is not always like you want).


In other words, an animation is equal to X frames, such as 1000 frames for a quite short animation. On some of these 1000 friends, you will set your unit in different positions (move, rotation, etc…) These settings are applied to the group of objects called Bones. Between each frames you modified to make the unit look different, you do not have to do anything: Mdlvis will make the projection itself. Simple, right?


To sum up, you only have to set the start position and the end position of each bone you want to see moving. Of course, some movements are more complicated to set, such has those of human beings.


Let’s see the options of the Frame menu.


Clear Frame : clear the coordonates stored on the current frame selection.


Delete Frame : idem.


Add Frame : adds a frame to the sequence. If the sequence was a succession of 1000 frames, it will now be a succession of 1001 frames. So this will implement the entire start/end of the sequence.


Since Mdlvis is powerfull but not godlike, sometimes the visbility of the particle created with an other tool such as War3 Model Editor will be disturbed. Just anticipate the thing you will probably have to set again the changes made by Mdlvis.


Cut Frame : delete the frame. It is the contrary of Add Frame.


Emitters Off : no idea, it is supposed to turn the particle emitter off, but anyway they are bugged in mdlvis.


Copy and Copy Frames : one can say copy is better, because it stores all the informations of the frame. It’s kinda usefull for any animation you want to copy/paste, for example when you want to have X animations having the same start position for your unit. Example: the Decay animation has to start with the same frame than the last frame of the Death animation. In this way, you will have no rupture between the two animations.


Blend : no idea.


Let’s see the other things.


On the right of the visualisation screen, you have a tittle: Current Sequence. In this window, you can select: All Line (defaut, the entire sequence of a model) or any subsequence: Attack-1, Walk… Now you will be able to understand how the animations work.


We call an animation “sequence” because it’s a chain of frames glued to each other. That’s all.


Two other buttons : Create and Global. Create allows to create (of course) a new sequence, defaut is with 90 frames (really fast, it’s invisible ingame). So you will have to add more frames.


Global allows to add a global sequence (a way to get a sequence used whatever the unit is doing, other words: a sequence used in any sequence).


On the bottom, you can see a long white bar with a button “lecture”’ (top right) (with white color and blue bars).


It is the visualisation of a sequence. You read them from left to right, like a film of a camera. The blue bars are frames where some parameters have been stocked. White are blank frames or frames where Mdlvis make itself the moves. IFI allows to set the interval between each frame.


Frame: this menu allows to move from a frame to an other with precision, contrary to use your mouse to place it on our sequence bar.


There we are. Use “Create”.


You should get this after picked Right perspective + texture display


The window “Current sequence” now displays a new sequence, “New”. Click on “New” and rename it depending what you need and the Warcraft III format (“bankai” is invalide, W3 will not recognize it  )


I want an attack with a critical strike: I call it “attack slam”.


Check the menu « Non looping », in order to have an animation which is not played in loop until a new order is given to the unit : Attack Slam will only be played on time through this.


Use rarity : I do not remember what it does.


Appply Mouse Speed : never take care of this, it allows Mdlvis to interpretate the move of the bones with the ones of your mouse.


Frame Interval : allows to set the start and end frame. Just do not touch this.


Delete : allows to delete the entire sequence.


You can notice we still have, and permantly have the little display window for the geosets.

b)  Menu Movements


it seems we have the same display of our model. The Movement menu allows to create the movements for the bones of a model. There, since you are supposed to have the Attack Slam sequence selected (the one you created), we are still on this sequence in the Movements menu. Of course, this sequence is blank.


You can see on the bottom this sequence goes from the frame 203000 to the frame 203010 (for me), which means…. 10 frames. No human eye can see this. Let’s make a right click on the sequence bar and :


Hop ! We validate Add Frame, and select 1000 frames to begin (meaning a sequence of 1010 frames).


What changes is the right window in mdlvis: we get again a window like the Vertex Editor. Do you see the scrolling menu on the top right? (A white bar with a defaut name: Objects). It allows to select some strange things: the first, on our Assassin, is called Bone_Arm_1_L


We are lucky since the names of the bones of the Assassin are easy to understand (which is not always the case).


A definition : a bone is a group of point, a bit like a geoset, except the geoset allows to select points together to apply them material (textures) or global animations. The bone is a matrix of points, working like a gravity center where you will set some parameters affecting all the points it regroups.


Move the bone = move all points stored in the bone depending on its gravity center.


It’s a bit more complicated on advanced models like Assassin: some points are linked to several bones. (We will go back on them later). And, even harder, there is a hierarchy between bones: if you move the shoulder, you will move the arm, and the arm will move the hand. It’s logic, but keep this in mind.


In our example, Bone_Arm_1_L means “the group of points linking between them all points of the superior part of the left arm. Blizzard usually call 1 the superior part, 2 the inferior. So there, this bone regroup more or less the left spaulder and the top of the left arm. If you go in Left perspective, you will be able to discover the transformations you can do with the bones.


Like in the Vertex Editor, you have the possibility to select something (useless for the moves), to move (translation), to rotate (pivot), to scale (raise/reduce the size of the model).


The other options are more for advanced modelists and allow to restrict the actions to only one kind of modifcation and avoid to make stupid things. Just have fun with the different transformations used on a bone, to understand what you can do.


Please note that for a human model, you should not use a lot “Move” and “Scale” since these actions will deformate the model, which is not really realistic. Rotate should be the most used action (to start)


I advice you to look how Blizzard animated its models, because they are well made. Of course, Mdlvis is not perfect, and it does not allow to see what is stored on a frame. So it’s kinda hard when you get 100 frames with modifications used on the model to understand what frame does what.


That’s why it’s better to create your own animation.


We will make a simple thing (and weird-looking, but the objective of my tutorial is only to show you the way to start with Mdlvis). We will try to move down the right arm in diagonal. (like this \ ). There, your own way to use Mdlvis takes more and more importance: you will switch between perspective modes a lot.


Note : Your model is stiff as a ramrod default. Okay. But if you create an animation in the middle of your sequence (a frame so) where the model has a certain position, and you stop there, there will be no movement. Mdlvis therefore assign all frames of your movie ... the same position.


So the golden rule of animation in Mdlvis: for each bone that you move, you should at least have a beginning, a change, and an arrival. A unic modification alone is not sufficient to attribute the entire sequence the same position.


Here is a little twisted: to create the frame of departure and arrival, you will simply have to select your bone, and take any transform tool (move, rotate, scale) and to "pretend" to turn your Bone. (In the case of a rotation, rotate the smallest possible). Mdlvis will then note the initial position of the bone, and compare, then, to frame the amendment.


In our example :


Step 1: you got to the first frame of the animation. One begins to rotate the bone Bone_Arm_1_R up, but right after we deliver it to its starting position. As it was made clear to the good old Mdlvis that we would like the starting position is this one. Beware, there have been information that and only that for this particular bone. The operation must be repeated for each bone you want to change in a sequence.


Step 2: we rotate our bone to get the whole arm (Because Bone_Arm_2_R and Bone_Hand_R for example are attached to the bones Bone_Arm_1_R) perpendicular to the trunk of our assassin.


Step 3: I remind you that it is better for a sequence to have the beginning and end like this. So you either do it by hand (by rotating the bone down again) or, as lazy people like me, you copy paste the first frame to the end of the sequence.


And now, you can watch your first animation in W3Model Editor. It is ugly, useless, but it is animated: D

C)  Menu Bones


Despite appearances, even if it’s a menu that looks like the others, it is probably the most difficult tool to use since it is vital but buggy


Another new menu: Create.


Here you can create a Bone, an attachment, or duplicate the selected object.


Create Bone: will create a center of gravity that you will be able to move with the usual translation action. Subsequently we will attach the vertex.


Create Attachment: attachment is a center of gravity which will remain empty. At this point you put in the desired location for that in game, you can add special effects. The attachment will become a center of gravity in this special effect.


Duplicate Subject: not very complicated to translate: copies the selected bone.


Right: we still have our usual window: menu to select the clips.


To avoid to damage our model we will start from 0. Go therefore on Create / Create Bone.


A new step is created by default: bone_newX where X is an integer that increments alone. Of course you can rename it.


So then, as our Bone is empty, we will have to add vertex. Select our first geoset as geoset displayed. The vertex of the head will appear. Select them.


You'll notice that on the bottom right, a window appears. It says two pieces of information: Object04 and Mesh_03


This means that the selected vertex (here, the head) are already linked, directly or indirectly, to bones and Mesh03 Object04.


If you click the action button Reattach verticles, the cloud of points selected (head here) will be released from other bone and attached to the selected one on our menu at the top right. With me, it is bone_new1. Use sparingly: with a transaction of this kind, you may kill your animations, since the head will be attached to another bone for whom we have not done anything.


You can also use the "Attach to verticle bones”, pretty convenient to add new bone to our head without emptying it of its ancient centers of gravity.


And hop! The head is attached to another bone. The fastest will have realized that my bone (red cube) is located at the foot of the murderer. It might give entertaining results if we move our bone during an animation. To you, then, to adjust the location of your Bones as you want. Note especially the hierarchy, again. I prefer to do this on War3Model Editor, since it is sufficiently well made


We see also that our bone is attached to the root of the model: it is independent. To make it dependent, simply right click on it, and move using the Move Up / Down / Left / Right. The hierarchy will be automatically made.


Under Mdlvis the hierarchy, I do not control, but then not everything is available, once the menu Bone in View (menu whose title does not pass from me), Hierarchy (Shortcut: H).


Clearly at the top was the first level of hierarchy (Body, the whole model).


Below: We have the folders of the bones. By double clicking it opens the dependencies of each. But I hardly see the logic of the display: p

#
 V/  Advices for animations


Again, the Blizzard animations are good in general, but made with more powerful tools (ie 3DsMax) Mdlvis being bugged, do not expect to do as well. It is a story of empowerment.


My method to make a complete animation on a model that already exists:


1-Make a move myself (when possible: p)


2-Copy of starting positions of the model if it has one, often stand, which is useful as a starting point of any natural-animation if the model is a Blizzard model.


3-Copy the same position at the end of the sequence. For the duration of it, I go on the principle of: 500: Ultra short animation. 1000: the minimum for most. 2000: Advanced animation a bit. More than 3000, excluding animation like Decay, which lasts a lot in general, is for very complex animation, really good when you want the show to go on.


4-Move from the largest to smallest. Example: To do a somersault to a unit: I know that the entire body turns on itself. So I created four frames, each with 90 ° rotation of the Bone (as it Bone_Root in some models Blizzards)


Then I attack the legs, which follow suit, placing their changes of position on the frames created for the Bone_Root if possible. Then the calves, feet. Then for the shoulders, arms, hands, etc. .. all depends on the model.


5-I check in Model Editor that what is displayed by Mdlvis is the reality, not an anticipation of the movements of Bones that I will forget to set "by hand" and which therefore will not be taken into account by Warcraft III.


6- When I have to create animations, I prefer to do much, but pretty simple: do not forget that during the game, an animated "attack" is anyway often quite rapidly. No need to rotate 36 x times a sword around the cops a soldier, a good blow dynamic is enough.


7-Take into account the time. For example, to make a good sword, the movement that will lift the arm must be much longer time than that will see the arm lower, to make the power of the shot.


8-Avoid translation / changes of scale: they involve a transformation of non-natural body, so may break the consistency of the model. Except cons examples, such as the Blizzard Marine rifle which is completely independent of the body and must be moved continuously by hand to keep it in the hands of a soldier.


9-Always look at your model: we often realized too important transformation, and unit, if it was intended to be realistic, will loose all credibility. A few degrees of rotation is sometimes enough to get the elbow in the chest of a unit: p


10-Optimizing model BEFORE adding particles in W3 Model Editor. Indeed, a good optimization will linearize the model. So the textures too, and you'll be unpleasantly surprised. If your model uses 30 particle moving, you'll win the right back behind the 30 particles, line visibility, and remove the display style "linear". Anticipate so.


11-Last tip: for each animation that you create, save your template under a new version. Mdlvis is very powerful, but really ***** from time to time, and we sometimes want to go back to the version before to start again from scratch on an animation!


 Last edited by a moderator: Apr 28, 2014

#### GhostThruster

#### GhostThruster

 Level 42

JoinedJul 24, 2009Messages5,688

Resources: 77

#### Resources
Models 14Icons 50Packs 1Skins 3Maps 6Spells and Systems 3

Good tutorial, but the download link doesn't 'work'. It says some bullshit about joining forumcircle. BTW, another place to download MdlVis is XGM(not allowed to link it though).

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

I suggest to use some colors and [go to]

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Yeah, it needs a new download link. Otherwise, it seems good.

#### jk2pach

#### jk2pach

 Level 21

JoinedMar 19, 2009Messages444

Resources: 88

#### Resources
Models 78Packs 1Maps 7Tutorials 2

Download Link done.

About colours, I guess it's a bit subjective. Personally, I hate getting attackec by dozen colours with flasy fonts

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Very nice. Thanks for the contribution and quick reply. =)


~Approved.

#### ZeusLT

#### ZeusLT

 Level 5

JoinedAug 5, 2010Messages114

Resources: 2

#### Resources
Maps 2

I think link is still bad. I think you wanted this - http://wc3r.forumcircle.com/viewtopic.php?t=13007

#### Sage Chow

#### Sage Chow

 Level 8

JoinedJan 22, 2010Messages115

Resources: 1

#### Resources
Maps 1

To newbies like me: Here are some random things I encountered while trying to learn to animate. Basically you've already read it in the tutorial, but you forgot how important it was

- Less is more!!

Making intervals between the positions longer makes smoother animations. I actually had a more succes with a reduced number of controll points.

I also had a lot of succes by setting the controller to linear, especially while editting existing animations.

- Tinker with the bones for optimal result.

I fixed a lot of hick-ups in the animations by either reattaching nodes to a different bone or repositioning the bone's centre of gravity. Hot tip: Use W3 viewer and press Ctrl+R to reload the model for a quick view. You can also attach multiple bones to a single vertex so you'll have a smooth transition (this can be vital!).

When rigging a new model to a pre-existing blizzard bone (eg footman or whatever), you may want to compare to the original model to attach similar vertices to similar bones. If you happen to put a foot vertex on a bone that behaves as an arm, or even forget to attach the foot to the knee, your animations will suffer greatly. Also, these bones could be outside the centre of gravity in the newly rigged model. This means that when you turn the 'arm' in the animation, it may end up sticking out of the shoulder. You can fix this by moving the bone, but this will also directly influence all of the blizzard animations that you wish to use. You could also try to make the new model resemble the base footman as much as possible (move the arms in the similar position, make sure the size is almost the same, etc).

- Use reattach vertex if you placed the wrong bone on a vertex.

Also, if you created/merged your model in oinkerwinkle, it will attach all vertices to a random bone. You will have to fix this by reattaching them to a new bone, and subsequently use add bone if you want it to use several.

- Rotate

This is by far the most natural way of creating a unit animation, many blizzard animations use only rotate on stuff like arms, heads and legs: Just set a few controll points and rotate the chest, all arm-bones and the head into key positions. Copy the first position into the last (for attack/stand anims) Set the controllers to linear and there you go: simple yet useful animation is completed.

- I messed up and created a double controller (like at 1333 and 1334)

Sometimes you miss the controll point and accidently create two adjacent positions, causing a hickup in the animation. I couldn't find a way to delete the information at 1334, so I turned to notepad instead (read forther below).

- Edit exisiting animations

You can check blizzards controll points by selecting a bone, selecting rotate (commonly) and then scrolling down to click the box 'highlight KF'. The blue lines will now only show the points where a rotation of the selected bone occurs: This is extremely handy when you want to make minor edits to an existing animation. If you edit all of the blue times of a single bone, you will effectively overwrite the pre-existing blizzard animations.

- How to delete controll points

Sometimes it can be very annoying to have to create a lot of poses for your animation to overwrite an existing animation. For instance, I tried editting the Keeper of the Grove model and had to correct for a stupid jumping effect.. Instead of making 30 positions to correct, I found it easier to make 4 good ones and delete the information in that section of the animation that is unwanted. I didn't know how to delete a pre-existing position without making a new one, so I did the following:

1. Check the timeframe of the animation that is really out of place and note the interval. Ex: 35167 to 35700.

2. Now check the bones that are messing up your desired effect and write down the names.

3. Save and convert to .mdl to open the file in Notepad*.

4. Use Ctrl+F to find the relevant bones. You will see something like this:

Translation 5 { (Sets 5 controll points for the given bone*)

 Hermite, (--> This is what you can set to 'linear' in MDLvis)

 34967: { -35.814899, 6.914600, 0.000000 }, (This is the timeframe and translation)

 InTan { -0.274757, 6.175210, 0.000000 }, (This gives information regarding smoothness)

 OutTan { -0.001572, 0.035321, 0.000000 }, (idem, this info is absent for linear controllers)

 35167: { -0.276329, 6.210530, 0.000000 },

 InTan { 29.526199, 8.365360, 13.235100 },

 OutTan { 29.526199, 8.365360, 13.235100 },

 35367: { 23.237499, 23.645300, 26.470301 },

 InTan { 8.817670, 7.858160, 9.926350 },

 OutTan { 14.696100, 13.096900, 16.543900 },

 35700: { 23.237499, 27.165600, 26.470301 },

 InTan { 0.000000, -4.668250, 0.000000 },

 OutTan { 0.000000, -6.068720, 0.000000 },

 36133: { 23.237499, 12.908300, 26.470301 },

 InTan { 0.000000, -18.351601, 0.000000 },

 OutTan { 0.000000, 0.000000, 0.000000 },

 }

* Note that Translation, sets a movement of the centre of gravity of a set of vertices (aka the bone). Expect things like Rotation and Visibility in a similar format. This section is the result of the postions you create in MDLvis by moving and rotating bones around. If there are two sets of rotations, you may have created a new one, with a linear function instead of a Hermite.

The position per interval is the 5 digit number before the {}, the InTan/OutTan give the movement vector.

5. Now you can manually delete the interval that was wrong (the middle three).

6. Don't forget to replace "Translation 5" by "Translation 2", as there are now two vectors instead of five. It's tedious, but quite effective.

7. Convert to mdx and re animate the deleted stuff with your own positions.

- A geoset is appearing in all animations.

Replace 'GeosetAnimID = none' at the relevant bone sections in the .mdl file by 'GeosetAnimID = 0', with 0 being the proper geoset anim you want the bone to use.

*aka your favourite modelling programme by far. It's so powerfull..


 Last edited: Mar 15, 2011

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Is the MDLVis file here up to date?

#### Kaien Shiba

#### Kaien Shiba

 Level 2

JoinedSep 9, 2015Messages19

thanks for the tutorial... i learn how to animate things... )

#### Emm-A-

#### Emm-A-

 Level 14

JoinedJul 1, 2008Messages1,314

Resources: 1

#### Resources
Spells 1

Hey guys, is there still someone working with MdlVis nowadays? I thought, this may relate to this thread, if not, I am sorry for posting in the wrong place.

Issue: Does someone know, if there is some models, that you cannot animate in MdlVis? If I try to animate an icicle model from the hive, I always get this error:

It displays whenever I do any action in Sequence Editor

#### Atasha JG

#### Atasha JG

 Level 8

JoinedNov 10, 2017Messages102

Resources: 4

#### Resources
Models 3Skins 1

I have problem,why i can't use uv map editor?

like its disable or something.

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Atasha JG said:

 I have problem,why i can't use uv map editor?

like its disable or something.


Click to expand...

You must select the vertices first before you can access the UV map editor.

#### wisanggeni

W

#### wisanggeni

Emm-A- said:

 Hey guys, is there still someone working with MdlVis nowadays? I thought, this may relate to this thread, if not, I am sorry for posting in the wrong place.

Issue: Does someone know, if there is some models, that you cannot animate in MdlVis? If I try to animate an icicle model from the hive, I always get this error:

View attachment 241168

It displays whenever I do any action in Sequence Editor


Click to expand...

Adr 613, model protected by copyright.

 You must log in or register to reply here.


### Similar threads

Locked

 Modeling and Animation 101


donut3.5

Oct 21, 2007

3D Modeling Tutorials
2

Replies70Views72K

May 14, 2015
wcity

 In-depth Animation Tutorial [3ds max]


BlinkBoy

Apr 11, 2009

3D Modeling Tutorials
23

Replies123Views153K

Feb 8, 2017
Kazeon

 How to make cool portraits without needing a separate file


Spacebuns

Sep 5, 2014

3D Modeling Tutorials

Replies11Views11K

Nov 27, 2017
Diegoit

 Model Editing


Fabioz

Jan 18, 2009

3D Modeling Tutorials

Replies45Views66K

Oct 10, 2021
KaiZen7th

[3D Art]
 Smoothing linear animations in Mdlvis


tillinghast

Jun 30, 2023

Tutorial Submission

Replies2Views2K

Jun 3, 2025
Greedy Martyrdom

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