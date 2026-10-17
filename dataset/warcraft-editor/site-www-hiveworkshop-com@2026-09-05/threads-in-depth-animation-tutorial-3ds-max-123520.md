---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/in-depth-animation-tutorial-3ds-max.123520/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# In Depth Animation Tutorial 3ds Max
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/in-depth-animation-tutorial-3ds-max.123520/>
#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

#
 Basic Animation Tutorial

By BlinkBoy

##
 Overview

###
 Difficulty

5/10 Normal difficulty

###
 Tools Required

3d Studio Max [Any Version]

3dsmax 4,5 -> Art Tools

3dsmax 6-up -> NeoDex 2

###
 Knowledge Requirements

Basic knowledge on managing 3dsmax and it's interface.

Some basic geometry and mathematical functions knowledge.

An understanding of motion.

a basic understanding of physics.

###
 This tutorial will teach you to:

Produce basic and natural humanoid animations.

Work with curve editors, trajectories and other 3dsmax animation related tools.

Create a basic Skeleton and edit it.

Skin your mesh correctly for warcraft 3 [k]and starcraft 2[/k].

###
 Content

Section 1: Introduction

Section 2: Rigging the Skeleton

Section 3: Skinning the Mesh
Section 3.1: Skinning with Skin Modifier

Section 3.2: Skinning using AT Skinning method

Section 4: Animating
Section 4.1: Stand - 1

Section 4.2: Stand - 2

Section 4.3: Walk

Section 4.4: Stand Ready

Section 4.5: Attack - 1

Section 4.6: Death

Section 5: Going Further

###
 Author's Note

Whenever you get stuck, don't hesitate to post. I'll be looking into the tutorial constantly.

###
 Suggestion:

this tutorial may a bit heavy to load, I suggest you to enter and reenter the tutorial's thread until every image has loaded. Don't use refresh or clean your internet temporary files.

##
 Section 1: Introduction:


Ok, let's get started.


We are going to reanimate the Village Kid from wc3. The first thing which we will do is to import the village kid into 3d studio max. I assume you already know enough to do that, otherwise look for an import tutorial. However, don't worry too much about it, you can use any humanoid model for this tutorial).


Here's our scene. The next thing that we will do is to learn a bit more from 3d studio max. Look at this chart:


I'll enumerate you the parts. All these have to do with animation, some will be used in this tutorial and others won't be used.

The Elements:

Time Configuration: With this tool, you can manipulate your scene's timing and settings. You will use this to move from one animation to another, as well as configure the speed in which you will visualize your animation.

Auto Key: by turning on this button, any transformation we do to our model will be recorded as a key in our frame bar.

Set Key: by turning on this button, you will be able to set a key of the last transformation done, by clicking the Key Button (Left to Set Key, has a key icon)

Time Slider: this tool is very important; with it we'll be able to move from one frame to another without any trouble.

Animation Toolbar: this toolbar contains many options like setting controllers, bone tools, IK Solvers, etc.

Graph Editor Toolbar: this contains all the graph editors. We'll use dope sheet and curve editor later in this tutorial.

Motion Panel: the motion panel has a lot of options to control the interpolation among the keys and how they interact in each bone.

Reactor (max 5- up): reactor is a physics simulation engine, as good as it is, we won't use it in this tutorial. Maybe I will later in another tutorial.

Maximize Viewport Toggle: Selfexplainative.
Well now a bit more of theory before getting into action. The time controls:


I won't explain these controls much; you can guess them with some intuition. We'll use them to play and run our animation. Notice that the number in the middle is the number of frame in which we are.


Okay let's start now. The first thing which we will do (look in the main chart) is to press Time Configuration. Now look into the new window:


We must set the settings in a way which it be easy for us to navigate through our scene. I suggest using an FPS of 30 f/s (NTSC), but you may decide here the FPS which best fits you. Some suggested numbers: 50/10/20/100/25/24/32/300. Now set the animation range from 0 to 100. Look at the next image:


Ok, now let's keep preparing our scene. We must now select our mesh (the village kid) and make him semi-transparent (alt + X), afterwards, right click him got to object properties and take off 'Show Frozen in Gray' click ok, then right click again and freeze its selection.


Now, the mesh won't be selectable, but seen. Let's go to Animation Toolbar (in max4-5 the Bone Tools are in Character Toolbar) and open the Bone Tools:


Here's the chart:


Now turn on Bone Edit Mode and click Create Bone:


Ok now we create a dummy bone in a random place. Do one click then another and right click. You should get 2 bones.


Now select the down bone and go into the motion panel (look at main chart)


Now click assign controller:


Click the button and select TCB position, then click Make Default and click yes.


Now do this also for rotation, make it TCB as well. For scale, if it isn't Bezier, then do the same you did to the other 2. Here's the final chart:


Now delete the 2 bones. You must be asking "then why we did this?" by making the controller defaults in a bone object, it will make that every bone which we create gets these settings automatically.

##
 Section 2: Rigging the Skeleton:


Okay, finally it's time to get into the ACTION. Let's start studying our model. Look at every viewport and start visualizing the structure of the skeleton. I'll help you out:


The blue dots are the bones, the blue lines are the links, the red dots identify edges, and the red lines are use to cut the limbs. This will be our structure. Let's get started.

Creating the bones:


Turn on Bone Edit Mode from Bone Tools (Remember?), go to the left Viewport, click Maximize Viewport Toggle (remember? Look at main chart) and click "Create Bone". We'll start by the legs. Watch the image:


The sequence is simple, you left click to place bones and to end the placing you right click. It creates an end bone afterwards witch we�ll remove later.


Now we have this:


As you see in the arrows, we must move the bones and fit them into the position. With the help of the viewports (front recommended) we�ll move them and fit them. Make sure Edit Bone Mode (Bone Tools panel) is on, or moving a bone will be affected by the links. This is what you should get:


Now we must make the other Leg. If this wasn't for warcraft, I would teach you about mirror, but mirror messes up 2 tools: Dexporter and Ik2FK converter, thus we can't use it. We must copy our bone chain in order for us to make a new leg.


Select all your Bone Chain and click Edit -> Clone. Look at the chart, make sure nothing is instance; everything must be set to copy:


Okay, click OK and select the old chain, then hide it:


Once hided, select each of the remaining bones and inverse it's X position. Here is an image so you see how:


Add a '-' to X position of each so 9.663 turns into -9.663 then press enter, and it moves. Go one by one and you'll have a mirrored bone chain without using mirror. Move the chain a bit off, if it's not on the correct position. Here's the result:


Now unhide all. (Left click nowhere and click unhide all) The result is:


Now we are going to go a bit faster. Using the same techniques you will create the arms. Here's how it should look at the end:


Now we must remove the dummy bones that are in the end. Normally, I wouldn't remove them since I use them for applying IK solvers, but in this tutorial we will only work on Forward Kinematics (FK). Ok select each end bone and click Remove Bone from the Bone Tools.


Now we will create the main body and finish the rig. Let's create the Chest first then Head and an end bone. Then we remove the end bone. Watch the image to help guide yourself:


Now we create a separated bone and make the pelvis, (we remove end bone). This may be confusing, I'll try to explain: Create a single bone where the pelvis should be, do an extra click to set it's range and left click, then you'll have 2 bones, remove one and the one that remains should be the pelvis.


Now we must create Bone_Root, our main bone. The problem of this bone is that it converts into a helper and our Exporters (Art Tools and NeoDex) export Helpers differently. I'll show for each side how to:


 In these versions, we create a cylinder in the middle and name it "Bone_Root", don't forget to set it as a bone in Art Tool's User property.


 We go to the Create Panel and we get into the helpers section, then we create a simple grid.


And we create it:


Okay, now we are back on the same route from both sides. For max 5 and 4 users, you'll have to imagine that the grid I made in the middle is your Bone_Root, youll follow it like if it was the circle.


It is time to link all the bones. Each chain is already linked, but now we must link all the chains together. Let's start by selecting the top bone of the right leg and then click the link button

 and link it towards the pelvis.


Now we'll link as it appears in the image:


As you see, both legs must be linked with pelvis; both arms, with the chest and both the pelvis and the chest should be linked with the root. That's how you do it. With this we are done. TURN OFF Bone Edit Mode now and save .


Now we'll name the bones to the correct nomenclature. Here's how you name them:

 Bone Nomenclature:


Head: Bone_Head

Chest: Bone_Chest

Pelvis: Bone_Pelvis

Leg Top: Bone_Leg1_(S)

Leg Bottom: Bone_Leg2_(S),

Foot: Foot_(S)

Arm Top: Bone_Arm1_(S)

Arm Bottom: Bone_Arm2_(S)

Hand: Bone_Hand_(S)

(S) = side, if it's on left side then we write an L, if it's on right side then write an R.


Example: Bone_Arm2_R (bottom arm right).


Now here's how your hierarchy should look like (to see this chart type h or click the button [k]"Select by Name"[/k] next to select) [To make it short this way, click Display-Children or, for older versions, turn on Display Subtree]:

##
 Section 3: Skinning the Mesh:


Finally we finished the skeleton, now we can progress to link the mesh (geometry) to the skeleton. For this part, I'll branch it into 2 areas: one in which I'll explain: how to skin in max 6 & up, and another one which I'll leave for Art Tool's tutorial(for max 4-5 users)


For skinning in max 4 and max 5, you must detach the mesh into pieces and link them to the bones. Since Art Tool's Guide already covers it, I will not explain it, please look at Art Tools guide and advance to part 3 of this tutorial: Animating.


For further information consult the Art Tools Guide: Here

Skinning in Max 6 and further:


Skinning in Max 6 and up has two options: Skin Modifiers and AT Skinning. Skin Modifiers skin the vertices directly to the mesh, while in AT Skinning we link meshes to bones and later NeoDex 2 takes care of connecting them

###
 Section 3.1: Skinning with the Skin Modifier:


Let's get started. Start by right clicking the view and then clicking "Unfreeze all". Now select the model's mesh and go to the modify panel, make sure the model is an "Editable Mesh" (SideNote: I would suggest you to weld the verteces first, it could optimize the model) afterwards add the modifier Skin.


Ok now that we have added the Skin modifier let's take a look into it.


Let's analyze a bit the controller. We have a lot of options of which we will only focus in 4 that interest us. First is the Add and Remove Bones, that list will contain the bones which will affect our mesh. Afterwards, we have Edit Envelopes; I would call it edit mode because it enables vertex selection. Next, there's an option for enabling Vertices within the Select panel, there's where the magic comes, from it we will disable envelopes and enable vertex selection. Now our final tool, the Weight Table. Thanks to the weight table we can assign the correct vertexes to the correct bones.


Time to get going, after seeing the chart; click the Button 'Add' and select every bone except "Bone_Root" then click select.


Okay now we will click "Edit Envelopes" and turn it on. Then we will go to the "Select" frame, turn on Vertices and turn off Envelopes and Cross Sections. Here's an image to help you out:


Now go and select the bottom vertices of the left foot. Look at the image. To know that they are selected, white squares will appear.


I have highlighted the selected vertices as you see. The red dots are the unselected vertexes; I marked them so you could contrast them. Now let's get back to the skin modifier and select the Weight Table:


Ok click it; this is what it should look like:


Do you see where it says "All vertices"? Change it for "Selected Vertices".


Ok as you see all vertices have 1.0 except one (You may have this different, it will depend on your rigging, so don't get scared). I'll explain what we must do here: We must assign all the selected vertices to Bone_Foot_L but one of them is assigned to Bone_Leg2_L, in order for passing it to Bone_Foot_L you must set that value to 0.0 and then go to the column of Bone_Foot_L and set the value to 1.0.


That's how it looks, but we still have a problem. The vertex is still assigned to Bone_Leg2_L and thus the exporter will still identify it as affected by that bone. Having the value as 0.0 click Edit-Remove Zero Weights and the vertex will be unassigned.


And that's the way the big cookie crumbles. Before I leave you on your own to finish the skinning of the mesh, I'll tell you how to skin correctly. The first part will be what I call "Dominion Collision". When 2 bones are next to each other and they have meshes which we are uncertain to which it belongs, we must assign vertices to 2 bones at the same time. Do you remember when I showed you the scheme for making the rig? I drew there red lines which separated the limbs, those separations represent the vertices which will be assigned to 2 or more bones. Let me illustrate you with an example:


I think you get what I mean now. Let's get on work start by selecting those vertices and then going to the weight table:


Now we must change everything at the same value. I'll give you a tip to do the same action to every vertex: click Edit-> Select all and by just modifying one the rest mimic it.


Now something important, the value the represents how much does the bone affects the vertex. In warcraft 3 vertices are affected proportionally, in other words, all will be affected the same way. So since it is 2 vertices we must divide our value on 2, so 1.0/2 = 0.5. Assign 0.5 to the first vertex in the column Bone_Foot_L. This way:


This is how it looks after we apply it, but one vertex is still assigned only to foot_L. Add a 0.5 at the unassigned vertex and it should be done. Since I know that you could have found different values, the idea is that the 3 vertices there all have 0.5 values for both Bone_Leg2_L and Bone_Foot_L.


Now we are going to do the rest of the skinning on our own, but before doing that we are going to focus on a particular vertex. See:


As you see the black spot represents only 1 vertex which is influenced by 4 bone dominions. In order for us to set it we must select it and go to the weight table.


Before you go to put the values, let me warn you why I decided to example a 4 dominion vertex. When we apply values to vertexes with more than 2 dominions we must do a technique called "chaining" we will do first a 2 dominion setting:


Now we apply the value 0.33333333333 to Bone_Foot_L. This is how it sets:


And finally we set the last bone the value 0.25:


And that's the way the Big Cookie Crumbles. Now do the rest of the skinning on your own. You got enough knowledge to do it on your own.


After you be done, test if everything is fine, like rotating the bones and then undoing.


This should be all for the skinning part. If you face troubles in that part you can always post, since sometimes Skinning acts weird. By the way, remember to remove the 0.0 values as I told.

###
 Section 3.2: Skinning using AT Skinning method:


Now I'll show you the second method: AT Skinning. In may opinion, this technique is easier but way less precise. This technique is supported by NeoDex 2 only.


In order to do an AT Skinning, we must detach the meshes into little limbs and then attach each limb to a bone. Let’s get going. First thing firsts, Turn on edged faces and hide the bones so we can detach the mesh without anything bothering us.


Now go to Modify Panel -> Editable Mesh and go to either polygon or element mode. We’ll start by selecting the head and the ears.


Then we roll down in the mod panel and click Detach.


Call it Head and click Ok.


Now repeat this process for each limb (Hands, Arms, Chest, Pelvis, Legs and Feet). Once you are done, you should have something like this:


They should be named this way:


Now that we have sepparated the limbs, we must tell neodex that they will use AT Skinning instead of normal skinning.


For starters, click the NeoDex Menu and click on “Objects Settings”


Select all your meshes (IMPORTANT, make sure you are selecting them all) and change the Skin setting from Normal Skinning to AT Skinning.


We are almost done. We just need to select each limb and link it with the corresponding.


I’ll start with the head, select, list, link to corresponding bone.


Now repeat the process for each remaining limb until they are all linked.


Here's the final list:


Notice that once you start animating, the limbs will look detached. Don't worry about that, the vertices will be connected when you export the model.


Now you can leave it that way and let NeoDex them on export, OR you can use the NeoDex Skin Tools to convert it into a normal skin. Here's How


Go to NeoDex -> Skin Tools.


This dialog will appear:


Turn on "delete old AT Skin Objects" 8in case you want to delete the little submeshes.


and click convert.


Now you should have a normal skin as usual.

##
 Section 4: Animating:


Just so you know, before we get into this topic, we can make things easier. If you give a material to the mesh, then select it and do alt + X to get them back to normal. Then set the opacity of the material to 50/30. It should make your model look better for animating.


Make sure to freeze the mesh first. Before we start animating, as with the last 2 sections, we are going to speak some bullshit xD. Pay close attention to the theory, it is important.


We have a new chart, this time it is animation specific. Let's look a bit on it:

Time Slider: with this tool, we'll be able to move from one frame to another without any trouble. If we select an object and we right click the time slider, the "create key tool" will appear, with it we can copy keys from one frame to another.

Auto Key: by turning on this button, any transformation we do to our model will be recorded as a key in our frame bar.

Set Key: by turning on this button, you will be able to set a key of the last transformation done, by clicking the Key Button (Left to Set Key, has a key icon)

Key Button: Use this when you have Set Key on to place a key for the last action done.

Add Time Tags: This little application allows us to mark our timeline. It�s quite useful to move from one animation to another.

Key Info: this rollout contains important information of our keys. We�ll use it to change the way our keys will interpolate and act.

PRS Parameters: allow us to create and delete keys quickly, it will also set the type of which key we want to edit, either if it�s rotation, position or scale.

Assign Controller: Here we will reassign animation controllers to our objects.

Curve Editor: The curve editor is a powerful tool which we will use to edit interpolation among keys in an easy manner. We will also use this later for making Global sequences, but maybe not in this tutorial.

Dope Sheet: dope sheet, like the curve editor, is also a track viewer; we will use this tool to add notes to our animations as well as visibility and other values.


Now before we get going to the practice, I will talk a bit about the animation techniques and how things are classified.


I classify animations in 3 ways.

 Classification according to order of Events:


Order by Interval: order by interval consists on creating an animation between 2 poses which are the same. Example of this are Stand - 1 and Stand Ready, in which the start and end positions are the same and we add new poses between the 2 intervals.

Order through Cycle: in order through cycle, we start from a key position we keep adding new poses and positions and we end in the same start. It's very similar to Order by Interval, but instead of creating the 2 poses from the start, we go from one pose to the other and we create the last pose at the end instead of the start. Examples of these are: Stand - 2, Attack, Spell.

Order to an end: in this one we start from one pose and we end in a complete different pose. Example of this type: Death, Morph, Decay, etc.

 Classification according to pose:


Pose2pose: in pose to pose we create poses from one frame to another. These poses consist on the complete skeleton moving from one stance to another. Perhaps you don't understand it yet, but once we start animating you'll understand.

Limb Flow: Limb flow is used by more advanced animators, instead of going from pose to pose, you move every limb independently and you set different frames for moving them. This concept is a lot more complicated to understand, we won't use it but only in one animation; Attack.

 Classification according to game:


Looping animations: Looping animations are animations which keep looping indefinitely, examples of these are: Stands and Walks.

NonLooping Animations: Nonlooping animations are animations which never loop. Examples: Attacks, Spells, Death, Birth, Morph, etc.

Okay time to get into practice.


First we will reset controllers for Bone_Root (only it), select Bone_Root go to motion panel (remember?) and click assign controller, then select Position and change it to Bezier Position, this time don't click "Make Default" but "Ok" instead.


Nice, now let's set the Frame 0. Select everything except the mesh (make sure you made the mesh unselectable) and right click the time slider:


Now disable everything from the window except Rotation, then click OK. I'll explain a bit this powerful tool. This tool allows us to copy keyframes from a selected object and paste them somewhere else in time. The source time refers to the frame which we want to copy, the Destination time is where we want to paste that key, and the Position, Rotation, and Scale boxes are the type of keys which we want to copy.


Something about this tool: First, You can copy keys from a time which aren't created and, second, you can copy and paste keys from the same frame to itself, thus it "Sets a key". Learn this well, it's important.


Now select Bone_Root and do the same once more, but this time enable "Position".


Okay, no we are done on the basic settings. We may now start animating

###
 Section 4.1: Stand - 1:


First we will reset our time interval from 0 to 100, to 10 to 40. Go into time configuration and do just like in the picture:


Click Ok.


Now move the time Slider to frame 10 and click Add Time Tag, then click Add Tag, afterwards a new window appears. Name it 'Stand 1 S' S symbolizes Start. Then, click Ok.


Now do the same with frame 40 but call the new tag "Stand 1 E". Now why doing this? We won't use this in the tutorial, but it is to teach easy ways to navigate your scene, if you click add time tag and then one of the 2 tags we added, the time slider will move there; it's perfect for editing our animations once we be done.


Ok, it is necessary that you add the notes for the animations, so Art Tools or NeoDex knows how to export them. Let's start by clicking Graph Editors -> Dope Sheet (It may also be in the Animation Toolbar) [In max 4 it's called Track View]. Now open Dope sheet and roll down to "objects", select it and click Track-> Note Track-> Add.


Now we must add 2 keys for start and end. Click the move button, select both keys (drag selection) and right click one of them, insert the value "Stand - 1" and close the window. Now we are done. Close Dope Sheet.


NOTE: Look at the Animation Names aprendix to learn a bit more of animation naming.


Okay, now we can finally start moving the bones and make some motion. First, select everything and set a key (like we did in frame 0), remember that we will only use rotation except for Bone_Root who will use position.


And add position for Bone_Root, the same way as before.


Now we will set our stand stance, maximize the perspective viewport, deselect everything and turn on Auto Key:


[WARNNING: make sure to have Auto Key ON]Now let's select Bone_Chest and let�s rotate it a bit, and fix the other bones to make the stance. I'll help you. First we rotate the chest a bit to the left, the head to the contrary side (right), then we rotate the chest down and later the head up. Notice how we move the limbs in contrary directions. Afterwards we move both arms, one backward, the other forwards and we fix them over. Finally, we move a bit the head


Continue doing this and fix the left arm, the same way I did to the right arm. Then improve the stance and try getting it to a normal position. Here�s my final:


Congrats, you have made your first Pose, this is what we call a pose. Now I'll teach you a fast trick for deleting keys before continuing.Move the time slider to frame 15 and (having auto key on) rotate any limb to any direction:


Okay, now we will delete that key. If you remember well there was a tool in motion panel which allows us to create and delete keys, we won't use it since it only deletes key by key.


I'll teach you 2 ways to do it, the first is using the time slider and right clicking the key (not the time slider) then clicking on delete key-> [Object Name]: Rotation. Good for deleting specific key and key types. Now I'll teach you how to do it in masses:


As you see we drag select, we get the keys selected, and we type the hotkey "Delete" or "Supr"(for spanish speaking users), depends on your keyboard. Notice that if you select keys this way, you can drag them from one frame to another, perfect for moving keys.


Now back to topic. Let's continue.


If you remember well, I said something about animations being classified by order of event? I talked of Order by Interval and that we will use it for Stand - 1 and Stand Ready. We will apply the theory now. Let's copy the pose from frame 10 to frame 40. Stand animations for being looping animations must start and end the same way, look forward:


And DON't forget of Bone_Root's position (remember you had to copy it alone?)


Okay now we have our interval, time to add the inner keys. Move the time slider to the middle, frame: 25 and Select Bone_Root.


Ok, now we will right click the move icon:

 and on the new window we will insert the value 0.5 to the z offset and type enter, so the transformation applies (Learn this tool perfectly, we'll use it later, same with rotation):


Ok, once you be done, open the curve editor and go to Bone_Root, open its transformation and look at position. Look at the image to help yourself. The curve editor may be found in a toolbar different to Graph Editor in older versions of 3dsmax.


Now let's zoom in extend the curve editor's window and lets use the zooming tool to see the curve better.


Ok, now we will edit the tangents so it moves natural. If you remember in physics how the graphic of the distance of a falling object is made. You would remember that as more time passes the derivate/pendent increases, in the case of an ascending object, the derivate/pendent keeps decreasing. We must make our curve simulate that pattern. Now we will select the tangents and while we hold "Shift" we will move them. Look at the gif (Don't make the curve that open, close it more, I had troubles remaking the gif):


This is the reason why I prefer Bezier over TCB controllers; in Bezier we can control the curve completely. In TCB we don't have that luck, we must change 3 values and the curve depends also on the rest of the keys.


Anyways, if you play the animation, you'll notice how it looks more natural, but wait a minute. The legs aren't moving, he's just moving up and down. Right look at this image:


If you see well as the Bone_Root goes up and down, the leg bones rotate but the feet stay static. This is how our model should move. Now staying on frame 25, we will rotate the legs and try to make the feet look static, use the grid to help yourself. I would suggest you to copy select the legs only and copy the frame from 10, so TCB doesn't affect them. I'll tell you the trick to make this effect, you rotate the top leg backward for a certain angle a, then you rotate the bottom leg for -a, in other words, the contrary angle and direction. The angle you'll use should be 2.5 or 3.0 degrees (use the transformation table). In the gif I'll use 5.0 because it's default for rotation.


The last thing we must do to Stand - 1 is to tweak a bit the animation, rotate the chest a little bit backward and the head the same amount forward at frame 25, that should be all. Here's how Stand - 1 looks pretty much like. Notice that the arms move on their own, even though the end and start keys are the same, the reason is because we are using TCB interpolation, which automatically adds that effect.


Ok, rotate both the chest and the head in contrary directions (like in the legs) for a value of 1.0. Now if you play the animation, you'll notice that something is bad about it, but you can't tell at all. The reason of this is because now the Chest and the Head are being interpolated by the TCB controller at 15 frames of distance, thus it looks quite bad. We must fix this.


To fix it select, first, Bone_Chest and go to frame 10, open the Motion Panel and roll down to PRS Parameters, set it to Rotation, then go down in the rollout and set the continuity to 0.0 then type "enter" and it should set. This is how you convert certain keys of TCB to linear. Now do this for every frame which you animated: 10, 25, and 40. After you have done it, do it to Bone_Head too.


Now we are done, this is how the animation should look like:

###
 Section 4.2: Stand - 2:


It looks like we finally reached the time to start animating something more complex. This time I won't help you as much as before, from now on I won't remember you how to copy or delete keys. Make sure that you have read everything in the past. You can always go back to remember what you have done, I'll just tell you to do it. Let's get started:


About Stand - 2: We are going to make the kid look at both sides like looking around himself.


First change the interval through time configuration from 10 to 40, to 50 to 140. Afterwards, mark the end and the start with a Time Tag, respectively called 'Stand 2 S' and 'Stand 2 E'.


Add the note tracks, but this time add this flag:


Rarity means how rare will the animation be played. It ranges from 1 to 9. 1 means very usual, 9 means very rare.


Okay, with everything set, we'll do the technique Order by Event. We will only set the starting pose. DON'T SET the end pose or you will have troubles while animating. Copy the Start pose from frame 10. All Stands must start and end the same way. This how things should be:


Ok now move to frame 60 (make sure Auto Key is on). We'll make him start turning. Look at the image to help yourself:


Ok, keep tweaking it to make it look better. You'll keep it up with the same techniques. As you know we are doing things Pose2Pose we aren't doing it individually by limb, so from now on I'll give you each frame's pose to help you. I may say something or give you some tips.


I forgot about turning the opacity of the material to 50, I'm sorry about it, I normally don't use it, but I'm in this tutorial to help you. Turn it back, or keep it. I'll turn it to 30 so you can see the bone positions better.


Anyways here's how frame 60 looks at the end:


Now we will make frame 70. For this one we will rotate the chest a bit more to the left and the head as well, we are also going to rotate a bit the arms and the hands. Leave the legs alone for now, we will fix them afterwards.


For frame 80, we will copy frame 70 (the same way as when we placed starts) and we will do some tweakings for there to be some motion. Also copy Bone_Root's position from frame 60.


For frame 90 we are going to copy frame 60, this time we'll rotate the chest in such way that it looks almost like it's looking forward, but not completely. We will copy Bone_Root's position from frame 50 and we will copy the 4 Bone_Leg's Rotation from frame 50. Here's how it looks:


Now for frame 100, we will select all the bones, except Bone_Root and we will copy the rotation from frame 90, then we select Bone_Root and we copy position from frame 90. Now we will rotate the Bone_Chest to the right and the head to the right, what we are doing is making a pose similar to the one on frame 90, but changing the facing side of the kid.


Now let's go to frame 110, we will do the same we did for frame 70 but instead of facing left, he faces right more. Try looking at my old instructions. You'll do the same, but this time you'll move the Bone_Root up and fix the legs like we did at frame 60.You can copy the position for the root from 60 and the rotation for the legs if you wish. (I fixed a bit the right hand which doesn't appear in the screenshot)


For frame 120, we will copy frame 110 (the same way as when we placed starts) and we will do some tweakings for there to be some motion. Also copy Bone_Root's position from frame 110.


For frame 130, we will assimilate it to frame 100, copy only the rotation from it, but becareful with the legs, we aren't lowering Bone_Root so they must stay firm. Copy the legs' rotation from frame 110. Now fix a bit the chest rotate it a bit and make it look a bit forward. Here are the scenes.


For the last frame, frame 140, we must copy the Start frame which is frame 50. Select everything and copy the rotation from 50 and then select Bone_Root and copy the position from frame 50. This is how it ends.


Now let's look at the animation from Preview.


If you look well, the arms aren't fully moving, the animation is not very smooth and the Legs need some more movement. We still need to work a lot on tweaking the animation. The idea of this technique is to make the basic animation and then improving it little by little. We are going to improve it now and we shall start by the Bone_Root.


Let's start by fixing the position curve a bit:


And change it to:


Yeah make the sides look like parabolas.


Now we will fix the legs to match the bone_root increases and decreases. Use the method I told you when we were finishing Stand - 1. But first, in case you also copied rotation keys to them, delete the rotation keys. (Remember? Drag the selection and select the green keys in the slider and delete every key except ending key)


Once we have used that method, we are adding a new effect. Notice it (I'm at frame 60):


Do it towards the boy's facing on both legs at both facing times. Don't forget of the end bones.


Afterwards, do some tweaking to the model, fix the arms, the head and rotate the Bone_Root to justify the new leg movement.


Here's final scene:

###
 Section 4.3: Walk:


Well finally we are getting heavy. From now on you will be pretty much on your own, you must try to make the animation this time without much of the schedule I'm leaving you. From this point I'll just upload the screenshots of the poses and you'll try to follow them, yet, for walk animation, we'll do some stepping; we aren't doing everything directly.


When I start animating a model, the first animation that I ever do is walk. The reason is mainly because walk is one of the most detailed animations, but don't worry we will make it simple.


Start by setting a new interval(Remember?, time configuration menu), this time it will be 160-184, yeah 24 frames is quite enough, almost 1 second. For this time we set Speed to be 1/2x(it makes the animation play slower), we want to watch closely what we are doing. Sample:


Don't forget to add the Walk notes. Remember that now we use a tag called MoveSpeed, it defines how fast walk plays according to the unit's speed.


Now we should start animating. Copy the first frame from frame 10(or 0 if you want) to all bones as usual (all rotations are copied, and we copy Bone_Root's position).


Let's start setting the first pose; we want to start moving the legs ONLY (base frame: 0) do it on frame 160, here's the sample:

Explanation: We must start at a running position, for this we will move Bone_Root a bit down and rotate it forward. Afterwards, rotate the chest and head to look better, now rotate the legs according to the figure. I won't longer use 3 view poses to illustrate you how you'll follow, I'll just use side poses now. Also here's a scheme of how the legs should rotate at running:


Ok now let's continue, go to frame 166, rotate the legs and move Bone_Root a bit down, and also rotate Bone_Root backwards by 1° (almost nothing, just adding motion):


Ok move to frame 172, copy Bone_Root's rotation and position from 160 to 172 and rotate the legs according to the picture or scheme:


Now, move to frame 178, copy Bone_Root's rotation and position from 166 to 178 and rotate the legs according to the picture or scheme:


Ok and finally copy the legs' rotation from frame 160 to 184.


Here's the preview of what we have until now:


[I suggest you to save here, you may confuse in this part]


Well as you notice legs are moving kind of smooth, but it's obviously not complete. The legs aren�t complete yet and we must animate the chest and the arms. Let's finish the legs. This time we will animate the Pelvis to make it move.


Set frame to 160 and rotate the pelvis 10° on the Z axi, the idea is to shake the pelvis from side to side.


You are going to rotate according to the frame. (the first value is how it was before , the second value is how it is on the frame; the value is the final angle, not the amount)


[IMPORTANT NOTE: don't modify the Absolute World values, use Offset World to apply changes, just apply the angle result of (after - before) it should then set the correct rotation]

 Rotations:



160: before: 0 after: 10


166: before: 10 after: 20


172: before: 20 after: -10


178: before: -10 after: -20


184: before: -20 after: 10

It should produce a shaky effect. Now add an extra effect, let�s rotate the pelvis on the X axi by a little:

 Rotations:



160: before: 0 after: 0


166: before: 0 after: 5


172: before: 5 after: 0


178: before: 0 after: -5


184: before: -5 after: 0

Ok here's the effect:


Well legs look more realistic, if you feel the legs are moving rarely, it's natural. At the end, we do some fixings to make things look more natural.


Now let's go on chest and head. We will do a technique used before in which the angles of a child and its parent contradict. You remember when we rotated leg1 down by 5° and then we rotated leg2 by -5°? The effect was that leg2 stayed kind off unaffected. Since we want the head to always look frontally, we must do this as well.


Let's get started, go to frame 160 and do everything you did to Bone_pelvis on the Z axi to the Bone_chest. But this time we will do 15° variation instead of 10°.

 Rotations:



160: before: 0 after: 15


166: before: 15 after: 30


172: before: 30 after: -15


178: before: -15 after: -30


184: before: -30 after: 15

Only do it to Z axi, X axi will be ignored.


Now do the same to Bone_Head but contrary:

 Rotations:



160: before: 0 after: -15


166: before: -15 after: -30


172: before: -30 after: 15


178: before: 15 after: 30


184: before: 30 after: 15

And the Result:


Now you are going to animate the Arms, follow the images:

The Technique:


The technique is quite simple, the arms move with the legs: when the legs go forward, the arms go forward; when the legs are going backwards, the arms go backwards. Something important to notice is contraction. The arms contract when they go backwards. Look at the animated Gif to get an idea of the movement.


Looking nice, isn't it? But it's quite far from ready. We must now give it FEELING. The last step is synchronizing everything. Things we must do: Rotate the head according to chest hitting, add some motion to feet, they must move up before hitting ground, fix the Bone_Root position and rotation to adapt to ground hitting. You�ll be on your own now, do all that through "trial and error" this is the moment for you to test your skills. Here�s the final result:


Maybe you are wondering why our animations look so different to the original model. The reason is simple; in this tutorial we don't care for the theme. What you have learned until now is how to make generic animations, once you get more used, you'll be able to make your animations adapt to the model's theme.

###
 Section 4.4: Stand Ready:


Stand ready is quite easy. It's just like Stand - 1, the difference is that we are going to have him in an attack position. The new interval for this animation is: 200-230, add the animation like the rest and copy the frame 0 or 10(first of stand - 1) to start posing the model. That should be it. Now let's prepare him: I suggest we pose him in boxing style, it's quite easy to animate that pose.


Getting to that pose is not an easy job; it took me 2 frustrating minutes to get the guy on the correct position (well I had to restart the scene, forgot to turn on AUTO Key, just so you see what happens if you don't turn it on). I'll give you some suggestions:

Turn ON Autokey!

Start using frame 0 pose as template.

Stretch the legs up by moving Bone_Root up and rotating the legs down.

Rotate the chest backwards and the head forward.

Move the root to center and rotate it diagonally, then fit the legs to the position.

Later set the arms and finish posing everything.


Now, like in Stand -1 copy frame 200 to 230 and move to frame 215 to start giving motion. I would suggest you to take nullify the continuities (Remember? TCB controllers?) to have a better control over the model's flow.


Now just start giving it motion, move the root and rotate the legs, arms, head and chest.


And the Final result:

###
 Section 4.5: Attack - 1:


For attacks we normally use intervals of less than or exactly 1 second. I normally vary from 20-35 frames, don't feel restricted to these values, you can even make attacks of 5 seconds, but remember that this will depend on the unit's attack speed. It's better to make attacks as fast as possible.


Okay time to get started. First, we will talk a bit of our attack animation. It's always good to think this kind of things. I decided that since we made stand ready be a boxing Peek-a-Boo (a guard position in boxing), we should make a jab as the main attack. A jab is quite simple; all he must do is extend his left arm forward and backwards fast. Not much of a task. Let's create a new animation from 250 to 270.


Yeah and set the settings this way:


Now we will be able to edit our animation at slow motion. Start copying Start pose of Stand Ready (frame 200) to frame 250. Remember that you'll only copy rotation and for the bone_root you'll copy position.


Now let's start on frame 250:


We just start with attack position.


Now move to frame 255 (remember to have AutoKey ON) and start making him prepare the punch, rotate the chest backwards, relax the punches and get them nearer to the chest. Flex the legs:


Time to get to the hit point. Hit points are made usually in less than 10 frames; the idea is that they be fast and surprising. To make it, extend the left arm the more you can. Afterwards, rotate the chest a bit down and to the side, so the arm has higher reach. Next, move the bone_root in direction to the attack and down as well, then flez the legs. Try to make the leg left on back keep its position while the front leg moves forward and contracts.


Now just contract and relax the limbs. Normally, we would use this frame to fix the punch and give the final feeling, but jabs are fast and aren't intended to do a lot of damage. Therefore, we are going to make this frame be an intermediate between attack position and hit point.


Now just copy frame 250 to 270 and that's it:


Here's the current result:


Looking good? Actually no, he looks like he's fixing the knuckle and not really hitting. In order to produce the correct effect, we must animate to a higher level; we must animate the arm more than our poses let us. To do this, we must change the frame system to frames and ticks. To do that open Time configuration window and change the time display settings from Frames to Frames:Ticks. This will give us more precision:


Now move to frame 253:80, we are going to work out the left arm in order for him to do more accurate movements. Select the entire left arm (arm1, arm2 and hand) and copy from frame 255 to the current (253:80), only rotation of course. Now move to frame 255:0 and reanimate the arm to "kinda" prepare the punch like in the image:


That's it, now move to frame 256:80 and start making him charge the punch. Also rotate the chest a bit to the side and the head counter side.


Now let's start giving the impulse to that arm, move to frame 258:0 and start moving (not literally, just do rotations) that fist forwards. Image  :


Now we are going to modify the hit stance, frame 260:0, and improve it to be more linear:


That's it, now we will move to frame 262:80 and try to keep the pose so the impact be visible, but of course change it a bit.


Ok now fix frame 265:0 and make it like if he's returning the punch to stand ready position.


Ok now we are kinda finished. To end use the same techniques on the legs and try to make them touch the ground along the animation, if possible edit the TCB values to help you out. Here's pretty much the final result, perhaps we should make it slower, but I'll leave it up to you, if you wish. All you have to do is increase the interval by N amount of frames, select the bones and move the keys along the slider to fit the new range.


Well let's do our last animation.

###
 Section 4.6: Death:


Death, in my own opinion, it's the hardest of the animations. It requires a lot of understanding of physics and mechanics in order to produce a quite natural effect. Don't worry; we will do it simple and easy, just like most of blizzard's death animations. Let's first look at this chart:


Don't get scared, all that is important (for animating) are the notes. We are studying the case in which the kid gets hit and he can no longer encore the damage, in other words, he be dead. In this case, we will threat him as a rigid body. If he gets hit, the impulse will give him a start velocity in x axis, in other words he'll move backwards, until stopping due to friction.


I understand that physics could confuse you a lot more, but don't worry, as we start animating you will notice what's happening and the relation between the theory and the practice.


Let's get started, create a new animation from 300 to 350. I'm not sure about this interval, so be ware that we may increase it. Copy the Stand Ready pose. Yep, Death starts on Stand Ready, not on Stand.


Ok set frame 300. Just copy frame 200 from Stand Ready as we did before in Attack.


Now move to frame 305, and make him look like he's about to be impacted, notice that I rotated bone root backwards and pelvis forward:


Now move the bone_root a bit backwards and down, and rotate the corpse as if he had received a direct impact on chest.


Now move it even more backwards and down, exaggerate the last position a bit, and keep opening the arms.


Make him hit floor and set the limbs up. Fix the chest and the rest as in the picture:


Move him up a bit, since he collisioned with the floor, he'll bounce up a bit, fix everything as in the picture:


And once more, fit him to ground, this time let the arms touch ground.


That's it, now he'll bounce once more, but a very little distance:


And finally comes to this position:


Copy the frame 340 to frame 350:


That's it, we are almost done. The last thing we must do is apply the physics concept that we talked about at the start. We will try assimilating Bone Root's position according to the forces applied, which in this case will be gravity and acceleration. Here's how the curves look at the end:


Look how the Z position is affected by gravity, just like we did on Stand - 1.


Now if you want to preview the trajectory to make sure you are setting the curve correctly, you can do it by selecting Bone_Root, right clicking it, going into Object Properties and turning ON the field called Trajectory. It should look like this afterwards:


That should be all. Sorry for making this animation in a rush, we could have actually made it way better, but the tutorial needs to finish already xD. Okay here's the final product:


That's all, hope you enjoyed the tutorial and learned a lot from it. Now here's your home work:

Make 2 more boxing attack animations; one should be a cross and the other a direct.

Add 2 more Stand animations completely original in which he does something a kid would normally do.

Deposit 3000$ at blinkboy's bank account, you didn't think this was free, did you? xD. Joking.


Here is the finished model: BoxingVillageKid

##
 Section 5: Going Further:


Now that you've learned the basics of animation. You are ready to go into other type of animation systems called "IK Systems".


In my tutorial: Happy Animating with IK You'll learn more complex rigging techniques to make the animation process easier.

The END

Special thanks to:

Jigrael, whose inspiring work taught me many things about animation, even though none of those elements are inside these pages, I hope I can share some day what I learned from him.

Elenai, who inspired me to write this.

Skrab, Mr Bob, Pyritie, LiOneSS, and Hawkwing who read and tested this tutorial.

#### Attachments

BoneChart.jpg

 80.2 KB

 · Views: 22,170



Structure2.jpg

 77.5 KB

 · Views: 21,276



MakeBones.gif

 1.1 MB

 · Views: 21,910



Rigging1.jpg

 91.2 KB

 · Views: 23,634



Rigging2.jpg

 159.2 KB

 · Views: 22,273



Rigging3.jpg

 67.1 KB

 · Views: 20,703



Rigging8.jpg

 180.3 KB

 · Views: 20,224



Rigging7.jpg

 36.7 KB

 · Views: 20,033



Rigging6.jpg

 27.9 KB

 · Views: 20,231



Rigging5.jpg

 20.1 KB

 · Views: 20,271



Rigging4.jpg

 67.5 KB

 · Views: 20,508



Rigging13.jpg

 88.6 KB

 · Views: 19,708



Rigging12.jpg

 43.5 KB

 · Views: 18,472



Rigging11.jpg

 50 KB

 · Views: 20,149



Rigging10.jpg

 53.8 KB

 · Views: 18,758



Rigging9.jpg

 79.3 KB

 · Views: 20,855



MakeBone2.gif

 958.7 KB

 · Views: 22,316



LinkBone.gif

 440.6 KB

 · Views: 19,966



LinkButton.jpg

 12.7 KB

 · Views: 31,333



HierarchyChart.jpg

 133.7 KB

 · Views: 19,367



Skinning5.jpg

 47.4 KB

 · Views: 18,546



Skinning4.jpg

 66.4 KB

 · Views: 18,806



Skinning3.jpg

 48.3 KB

 · Views: 18,620



Skinning2.jpg

 117.6 KB

 · Views: 18,740



Skinning1.jpg

 105.5 KB

 · Views: 19,305



Skinning6.jpg

 39.5 KB

 · Views: 18,470



Skinning7.jpg

 134 KB

 · Views: 18,909



Skinning8.jpg

 76.6 KB

 · Views: 18,283



Skinning9.jpg

 34.1 KB

 · Views: 18,286



Skinning10.jpg

 133.1 KB

 · Views: 18,331



Skinning15.jpg

 30.1 KB

 · Views: 18,141



Skinning14.jpg

 47.8 KB

 · Views: 18,110



Skinning13.jpg

 153.8 KB

 · Views: 18,387



Skinning12.jpg

 42.3 KB

 · Views: 18,007



Skinning11.jpg

 95.4 KB

 · Views: 18,815



Skinning16.jpg

 31.9 KB

 · Views: 17,796



Skinning17.jpg

 31 KB

 · Views: 17,920



Weight Table.jpg

 183.1 KB

 · Views: 18,980



Check.gif

 1.2 MB

 · Views: 19,883



Skin Modifier.jpg

 34.4 KB

 · Views: 18,575



Animating3.jpg

 89.9 KB

 · Views: 17,885



Animating4.jpg

 110.3 KB

 · Views: 17,792



Animating5.jpg

 89.6 KB

 · Views: 17,605



Animating6.jpg

 102 KB

 · Views: 17,669



Animating12.jpg

 53.2 KB

 · Views: 17,373



Animating11.jpg

 111.3 KB

 · Views: 18,287



Animating10.jpg

 49.4 KB

 · Views: 17,512



Animating9.jpg

 124.7 KB

 · Views: 18,402



Animating8.jpg

 74 KB

 · Views: 17,422



Animating13.jpg

 74.7 KB

 · Views: 17,482



Animating14.jpg

 53 KB

 · Views: 17,418



Animating15.jpg

 58 KB

 · Views: 18,428



Animating16.jpg

 46 KB

 · Views: 17,381



Animating17.jpg

 35.8 KB

 · Views: 17,456



Animating22.jpg

 69.1 KB

 · Views: 16,253



Animating21.jpg

 27.5 KB

 · Views: 16,291



Animating20.jpg

 81.5 KB

 · Views: 16,637



Animating19.jpg

 211.4 KB

 · Views: 19,236



Animating18.jpg

 36.2 KB

 · Views: 17,207



Animating27.jpg

 53.3 KB

 · Views: 15,933



Animating26.jpg

 53.1 KB

 · Views: 16,237



Animating25.jpg

 52.9 KB

 · Views: 15,952



Animating24.jpg

 53.8 KB

 · Views: 16,006



Animating23.jpg

 67.4 KB

 · Views: 15,929



Animating28.jpg

 53.1 KB

 · Views: 15,934



Animating29.jpg

 57.7 KB

 · Views: 15,846



Animating30.jpg

 32 KB

 · Views: 15,840



Animating31.jpg

 36.3 KB

 · Views: 15,798



Animating32.jpg

 59.4 KB

 · Views: 15,924



Animating37.jpg

 30.8 KB

 · Views: 15,385



Animating36.jpg

 31 KB

 · Views: 15,416



Animating35.jpg

 19.5 KB

 · Views: 15,626



Animating34.jpg

 56.2 KB

 · Views: 15,619



Animating33.jpg

 27.9 KB

 · Views: 15,512



Animating38.jpg

 36.2 KB

 · Views: 15,493



Animating39.jpg

 59.1 KB

 · Views: 15,400



Animating40.jpg

 59.4 KB

 · Views: 15,273



Animating41.jpg

 58.8 KB

 · Views: 15,130



Animating42.jpg

 56.3 KB

 · Views: 15,170



Animating47.jpg

 37.3 KB

 · Views: 15,092



Animating46.jpg

 40.5 KB

 · Views: 15,193



Animating45.jpg

 18.6 KB

 · Views: 15,040



Animating44.jpg

 51 KB

 · Views: 15,167



Animating43.jpg

 54 KB

 · Views: 15,142



Animating48.jpg

 37.9 KB

 · Views: 15,151



Animating49.jpg

 25.5 KB

 · Views: 14,967



Animating50.jpg

 73.5 KB

 · Views: 15,071



Animating51.jpg

 55.3 KB

 · Views: 15,177



Animating52.jpg

 56.1 KB

 · Views: 15,028



Animating57.jpg

 54.6 KB

 · Views: 14,982



Animating56.jpg

 58.1 KB

 · Views: 14,908



Animating55.jpg

 55.6 KB

 · Views: 14,971



Animating54.jpg

 55.9 KB

 · Views: 14,929



Animating53.jpg

 55.7 KB

 · Views: 14,896



Animating62.jpg

 51.3 KB

 · Views: 14,850



Animating61.jpg

 48.2 KB

 · Views: 14,867



Animating60.jpg

 50.5 KB

 · Views: 14,944



Animating59.jpg

 51.3 KB

 · Views: 15,024



Animating58.jpg

 51.1 KB

 · Views: 14,852



Animating64.jpg

 52.3 KB

 · Views: 14,666



Animating65.jpg

 53.1 KB

 · Views: 14,943



Animating66.jpg

 50.1 KB

 · Views: 14,617



Animating67.jpg

 48.7 KB

 · Views: 14,579



Animating72.jpg

 47.4 KB

 · Views: 14,655



Animating71.jpg

 47.2 KB

 · Views: 14,673



Animating70.jpg

 47.3 KB

 · Views: 14,676



Animating69.jpg

 49.7 KB

 · Views: 14,552



Animating68.jpg

 48.6 KB

 · Views: 14,658



Animating73.jpg

 47.4 KB

 · Views: 14,565



Animating74.jpg

 58.3 KB

 · Views: 14,820



Animating75.jpg

 40.7 KB

 · Views: 14,590



XtraAprendix1.jpg

 93.2 KB

 · Views: 879



XtraAprendix2.jpg

 37 KB

 · Views: 875



XtraAprendix3.jpg

 74.7 KB

 · Views: 835



Stand1.gif

 3.1 MB

 · Views: 18,341



deletekey.gif

 31.5 KB

 · Views: 17,084



Move Icon.jpg

 12.7 KB

 · Views: 29,297



Curve Editor.gif

 1.1 MB

 · Views: 17,634



Curve Editor2.gif

 993.5 KB

 · Views: 18,241



Legs.gif

 25.1 KB

 · Views: 21,367



Leg_Rotate.gif

 1.2 MB

 · Views: 17,231



Stand1_finished.gif

 194.6 KB

 · Views: 16,519



Stand2 Set1.gif

 2.1 MB

 · Views: 16,875



Stand2_Preview.gif

 180.4 KB

 · Views: 667



SetLegs.gif

 1.3 MB

 · Views: 16,093



stand2.gif

 596.4 KB

 · Views: 15,640



Walk Set5.gif

 264.6 KB

 · Views: 15,126



Walk Set4.gif

 145.1 KB

 · Views: 15,250



Walk Set3.gif

 158.3 KB

 · Views: 15,247



Walk Set2.gif

 163.1 KB

 · Views: 15,294



Walk Set1.gif

 162.1 KB

 · Views: 15,501



Walk FINAL.gif

 144.1 KB

 · Views: 510



Stand_Ready_Set1.gif

 306.3 KB

 · Views: 15,108



Stand_Ready_Set2.gif

 64.6 KB

 · Views: 14,911



Stand_Ready_Final.gif

 154.6 KB

 · Views: 14,907



Attack1_Set5.gif

 36.4 KB

 · Views: 14,664



Attack1_Set4.gif

 153.5 KB

 · Views: 14,890



Attack1_Set3.gif

 153.8 KB

 · Views: 14,866



Attack1_Set2.gif

 35.5 KB

 · Views: 14,835



Attack1_Set1.gif

 144.6 KB

 · Views: 14,956



Attack1_Final.gif

 129 KB

 · Views: 14,713



Attack1_Final2.gif

 153.3 KB

 · Views: 14,678



Animating63.jpg

 124.3 KB

 · Views: 14,675



Death_Set1.gif

 473.8 KB

 · Views: 14,763



Death_Set2.gif

 89.4 KB

 · Views: 14,813



Death_Final.gif

 230.3 KB

 · Views: 14,630



Rigged.max

 264 KB

 · Views: 975



Skinned.max

 300 KB

 · Views: 780



Start.max

 216 KB

 · Views: 831



Tutorial.max

 364 KB

 · Views: 875



Main Chart2.jpg

 317.6 KB

 · Views: 19,271



Main Chart.jpg

 172.4 KB

 · Views: 29,164



Step5.jpg

 114.8 KB

 · Views: 21,594



Step4.jpg

 78.5 KB

 · Views: 21,193



Step3.jpg

 53.4 KB

 · Views: 21,867



Step2.jpg

 87.2 KB

 · Views: 22,676



Step1.jpg

 171.1 KB

 · Views: 26,300



TimeConfig.jpg

 105.6 KB

 · Views: 22,911



TimeControls.jpg

 19.7 KB

 · Views: 22,789



Step8.jpg

 36.4 KB

 · Views: 20,665



Step7.jpg

 81.3 KB

 · Views: 21,154



Step6.jpg

 42.2 KB

 · Views: 20,773



TimeConfig2.jpg

 108.8 KB

 · Views: 23,048



BoxingVillageKid.mdx

 43.7 KB

 · Views: 1,677



BoxingVillageKid.mdl

 130.1 KB

 · Views: 782



Tutorial.rar

 21.5 MB

 · Views: 1,110



ATSkin5.jpg

 77.9 KB

 · Views: 6,069



ATSkin4.jpg

 31.9 KB

 · Views: 6,046



ATSkin3.jpg

 32.6 KB

 · Views: 6,220



ATSkin2.jpg

 118.6 KB

 · Views: 6,670



ATSkin1.jpg

 82.6 KB

 · Views: 6,275



ATSkin6.jpg

 108.9 KB

 · Views: 6,249



ATSkin10.jpg

 116 KB

 · Views: 6,246



ATSkin9.jpg

 83.7 KB

 · Views: 6,385



ATSkin8.jpg

 122.9 KB

 · Views: 6,233



ATSkin7.jpg

 39.9 KB

 · Views: 6,010



ATSkin11.jpg

 133.2 KB

 · Views: 6,184



Skinned AT.max

 304 KB

 · Views: 479



Tutorial AT.max

 372 KB

 · Views: 480



ATSkin12.jpg

 167.4 KB

 · Views: 3,882



ATSkin13.jpg

 69.1 KB

 · Views: 3,760



ATSkin14.jpg

 170.5 KB

 · Views: 3,508




 Last edited: Aug 28, 2014

#### skrab

#### skrab

 Level 25

JoinedMay 31, 2007Messages1,442

Resources: 23

#### Resources
Models 21Icons 1Maps 1

There is a lot of empty space at the bottom.

In walk section i only see 5 pictures and few sentences.

Could you also upload max5 formats?

and maybe pdf format for those who don't want to scroll in browser

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

skrab said:

 There is a lot of empty space at the bottom.


In walk section i only see 5 pictures and few sentences.


Could you also upload max5 formats?


and maybe pdf format for those who don't want to scroll in browser

Click to expand...


There's not much that can be added to Walk, since it's most likely technique.


hmm, I don't see the empty space.


I'll see if I can produce max 5 scenes, the problem is that max 5 is on my shitbox, so it's a pain in the arse to work with it.


about pdf format, hmm who knows, maybe. I just don't know how to produce it, I could transform the word format to pdf, but still the Gifs would stay static.

#### Devine

#### Devine

 Level 30

JoinedSep 2, 2007Messages3,688

Resources: 10

#### Resources
Models 3Icons 6Tutorials 1

Awesome

Now I might consider using 3dsmax again o:

+rep (+ rep may take some time because i'm still scrolling up)

nuff said.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

let's see if the empty space had been fixed.


EDIT: added an advice for Firefox users.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

DUDE this tutorial's awesome.

I'm not really liking the boxes at the top though. Mhh.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

what should I make for approval? change the boxes?

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Yeah, get rid of the boxes and use headers instead.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Pyritie said:

 Yeah, get rid of the boxes and use headers instead.


Click to expand...


Done, I got rid of the intial boxes.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

FUCKING approved.

#### Super-Sheep

#### Super-Sheep

 Level 20

JoinedOct 21, 2006Messages3,224

Resources: 2

#### Resources
Maps 2

Seems like you used some time on it.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Super-Sheep said:

 Seems like you used some time on it.


Click to expand...


Yeah, quite a lot. I started it on December.


SN: Fixed an issue in which 20 images were corrupted and lost.

#### Elenai

#### Elenai

 Level 35

JoinedMay 22, 2007Messages5,364

Resources: 36

#### Resources
Models 14Icons 22

Flawless.

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

HOLY FUCKING SHIT. This deserves "Tutorial of the Year" award icon. Fuck yeah, it's amazing!

AND HOW OFTEN DO YOU SEE ME AMAZED ENOUGH TO WRITE IN CAPS?!

+LOVE

#### Linaze

#### Linaze

 Level 24

JoinedFeb 28, 2007Messages3,478

Resources: 4

#### Resources
Models 1Tutorials 3

Awesome tutorial.

#### expresso

#### expresso

 Level 5

JoinedAug 25, 2007Messages44

Resources: 1

#### Resources
Models 1

Do you prefer to use Max 6+ since you don't have to detach and link the faces? Actually I have a bunch of questions for you as this thread is very recent and a lot of the information I've been looking at is from years ago, but maybe I should just PM you?

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

expresso said:

 Do you prefer to use Max 6+ since you don't have to detach and link the faces? Actually I have a bunch of questions for you as this thread is very recent and a lot of the information I've been looking at is from years ago, but maybe I should just PM you?


Click to expand...


do as you wish.


and I prefer to use it, because it gives me more liberties. Also, I have Vista on my computer and it does not support max 5, and I don't like working on the other computer which has max 5.

#### pkw

#### pkw

 Level 9

JoinedMay 28, 2008Messages455

Resources: 1

#### Resources
Models 1

Finally  Amazing Tutorials for 3ds Max animating. Gotta try it out.

#### expresso

#### expresso

 Level 5

JoinedAug 25, 2007Messages44

Resources: 1

#### Resources
Models 1

Well I'm on vista using Max 5 =P I've learned to use pretty much all aspects of character model animation the art tools allow. If I switched over to a higher Max version and then had to use Dexporter would I just be missing out on some particle emitter options and few other things listed there? The ability alone of being able to use a skin modifier makes me want to switch over I guess is what I'm saying... Just wondering if there are any other huge hurdles I should be aware of

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

expresso said:

 Well I'm on vista using Max 5 =P I've learned to use pretty much all aspects of character model animation the art tools allow. If I switched over to a higher Max version and then had to use Dexporter would I just be missing out on some particle emitter options and few other things listed there? The ability alone of being able to use a skin modifier makes me want to switch over I guess is what I'm saying... Just wondering if there are any other huge hurdles I should be aware of


Click to expand...


well if you install NeoDex thjere will be few feutures which you'll lose, like a previewer, Tvertex Animations, TCB rotations and billboarding. Not much.


Btw how did you make cdilla run on vista?

#### expresso

#### expresso

 Level 5

JoinedAug 25, 2007Messages44

Resources: 1

#### Resources
Models 1

I'm not sure if I had a problem with c-dilla, it's clearly on here. I did have to update to 5.1 and run the 5.1 update patch, and after all that I have to rename combust_select.dlu to combust_select.dlu.bk to overcome an initial loading error. It runs fine after that. And actually before service pack 1 it would crash constantly which forced me to dual boot to my copy of WinXP, but after service pack 1 it hasn't crashed for me yet.

#### Kuhneghetz

#### Kuhneghetz

 Level 47

JoinedJul 27, 2004Messages3,685

Resources: 239

#### Resources
Models 122Icons 104Packs 3Skins 8Maps 1Templates 1

Instead of doing a pdf document of the tutorial, maybe you can do a html document instead and then submit all the important files in a zip/rar file, thus you can keep the animated pictures?

Interesting tutorial btw, have yet to test it myself due to the loading times of certain images (and therfore I would prefer a downloadable document of the tutorial).

#### skrab

#### skrab

 Level 25

JoinedMay 31, 2007Messages1,442

Resources: 23

#### Resources
Models 21Icons 1Maps 1

Here is one with the right hand LoL

#### Attachments

attack 2.gif

 232.4 KB

 · Views: 371



#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

good work skrab, you still need to work out your skinning and Legs. I also think the arms are too noisy. Also, I doubt a boxing player loses the sight of his opponent during a hook. Anyways, work out the legs.


DvO: I'll see what I can do.

#### Ner-Zhul

#### Ner-Zhul

 Level 2

JoinedAug 31, 2007Messages14

I cant see some of the pictures they come out as little [x]'s. Would you please update this as I really need to learn how to do this.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

well i fixed it a bit, but some images won't load, you can try refreshing in those cases or taking the link and loading the image on other window.

I suggest you to try loading the tutorial while the website has less trafic, that way it's more certain to load. (around 6:00am meridian time) Also, since iumages are saved in temporal files, try entering and reentering the thread until every image be loaded.


 Last edited: May 17, 2009

#### Sephiroth_VII

#### Sephiroth_VII

 Level 15

JoinedJun 10, 2007Messages389

Resources: 12

#### Resources
Models 8Icons 2Maps 2

This is definitely a great tutorial. I read up to the stand animation part and learned a few things. Making the curve to simulate real rise and fall was the most eye opening. Add time flag is a great option I've never used before. Only negitive so far is you didn't mention the key filters option.


Maybe this will help me make better animations than the crap ones I've done in the past!

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Sephiroth_VII said:

 This is definitely a great tutorial. I read up to the stand animation part and learned a few things. Making the curve to simulate real rise and fall was the most eye opening. Add time flag is a great option I've never used before. Only negitive so far is you didn't mention the key filters option.


Maybe this will help me make better animations than the crap ones I've done in the past!


Click to expand...


key filters are for using Set Key mode and that mode is frustating, not to mention that you must change filtering all the time.

#### Ner-Zhul

#### Ner-Zhul

 Level 2

JoinedAug 31, 2007Messages14

Mine won't assign the verts to the bones. I click on Weight Table and all the verts are correct but when I move the bone the mesh itself does not move. ??


Jack

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

are you sure that the skin modifier is activated?

#### Ner-Zhul

#### Ner-Zhul

 Level 2

JoinedAug 31, 2007Messages14

Yes, I'm sure. Is there another button I should press to turn it on. If not then it is definatly on.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

I mean like this:


is the bulb on?

#### Ner-Zhul

#### Ner-Zhul

 Level 2

JoinedAug 31, 2007Messages14

Yes it is on.


I could send you a OBJ file?

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

very weird, I wouldn't know what's wrong unless i could see your scene. Yet, I imagine it's on max 2009, so on I won't be able to open it.


Restart the Skinning part from the beginning.

#### dickxunder

#### dickxunder

 Level 15

JoinedSep 6, 2008Messages65

Resources: 8

#### Resources
Models 5Icons 3

hello, off-topic of appreciation,

I was able to do this in Gmax... well who cares? the few who use Gmax! It's a little different (the part about skinning and the dope sheet) but this tut nevertheless crossed barriers.

The animation interface of Gmax isn't exactly intuitive (for me) for making War3 compatible models but you made it so clear. Thank you, may Gmax users go here too (armed with dex scripts you perfected) and start animating and oh yeah, moar power to you.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

good that you point that out. However, I imagine most GMaX users will have troubles on Rigging and Skinning, that's why I didn't suggested this tutorial for Gmax.

#### Tauer

#### Tauer

Patron

 Level 41

JoinedJul 26, 2004Messages1,480

Resources: 45

#### Resources
Models 35Icons 2Skins 8

Wow, crazy tutorial! I love it, definately going to use it when I'm going to try out animating!

#### Tauer

#### Tauer

Patron

 Level 41

JoinedJul 26, 2004Messages1,480

Resources: 45

#### Resources
Models 35Icons 2Skins 8

Sorry for double posting, but this is driving me nuts. I'm animating the Stand animation for my model but I must have done something I shouldn't because whenever I move the Time Slider the camera moves! I can't figure out what the hell I did and it's impossible for me to work when it does this.

Help?

For some reason I can't zoom either.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

very weird, maybe you should reset your 3dsmax configuration.

#### Venmar

V

#### Venmar

Idle animation woes

Great tutorial, thanks.


 Last edited by a moderator: Jun 8, 2009

#### MeKC

#### MeKC

 Level 9

JoinedJul 3, 2008Messages495

Resources: 2

#### Resources
Models 1Tools 1

Do you have this tutorial in a word document so I can print it?

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

MeKC said:

 Do you have this tutorial in a word document so I can print it?


Click to expand...


can't you try printing the post?

#### MeKC

#### MeKC

 Level 9

JoinedJul 3, 2008Messages495

Resources: 2

#### Resources
Models 1Tools 1

I tried, but some of the text at the right side is missing. So I had to select all the text and pictures and only print the selected.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

hmm, anyways.


I have updated the tutorial for version .77 of neodex. Each time neodex grows better; soon it will be at pair with Art Tools.

#### bboy-tiger-

#### bboy-tiger-

 Level 12

JoinedJul 28, 2007Messages920

Resources: 2

#### Resources
Maps 2

Yea you go boy. Even better than Art Tools  .

EDIT: Btw seems like 3dsmax"9" has some issues with weight table when you follow this tut. Same problem as Ner-Zhul.


 Last edited: Jul 5, 2009

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

bboy-tiger- said:

 Yea you go boy. Even better than Art Tools  .


EDIT: Btw seems like 3dsmax"9" has some issues with weight table when you follow this tut. Same problem as Ner-Zhul.


Click to expand...


that's not really a bug it has to do with editing the model and applying skin over. Just reconvert the mesh to editable mesh, unselect, reselect it and add the skin modifier.

#### illidan92

#### illidan92

 Level 7

JoinedOct 23, 2005Messages90

Resources: 7

#### Resources
Maps 7

Wow. Very in-depth tutorial and quite descriptive (now I can get kids to punch eachother ). The numerous pictures (especially the GIFs that really show how you do it) are what really make the learning experience thorough and easy. I've always been in love with animating.

Too bad I don't have $3500 to blow off on a modeling program. I could buy another car for that, or a sweet motorcycle.

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

illidan92 said:

 Wow. Very in-depth tutorial and quite descriptive (now I can get kids to punch eachother ). The numerous pictures (especially the GIFs that really show how you do it) are what really make the learning experience thorough and easy. I've always been in love with animating.


Too bad I don't have $3500 to blow off on a modeling program. I could buy another car for that, or a sweet motorcycle.


Click to expand...


or get an student license for 50$

#### Kenichi_101

K

#### Kenichi_101

Is there a way to get


3dsmax 4,5 -> Art Tools


to work on macs?

#### Kenichi_101

K

#### Kenichi_101

does any of those work for macs? DX

and crap sorry for double posting. DX

 You must log in or register to reply here.


### Similar threads

 Animation In 3ds Max 5 For Beginners


blazing-inferno

Jan 22, 2008

3D Modeling Tutorials

Replies9Views12K

Jan 23, 2008
skrab

 Mdlvis: model edition/creation, animation and UV Mapping


jk2pach

Sep 20, 2010

3D Modeling Tutorials

Replies13Views88K

Mar 2, 2020
wisanggeni

W

 Basics of Special Effects (SFX)


Daelin

Nov 21, 2006

3D Modeling Tutorials

Replies1Views30K

Nov 21, 2006
Archian

 Making Effects in Magos - Advanced Techniques


WILL THE ALMIGHTY

Feb 23, 2008

3D Modeling Tutorials

Replies14Views10K

Nov 2, 2008
3rgytr54

 Animated Textures Tutorial


Amigurumi

Feb 9, 2010

3D Modeling Tutorials

Replies39Views49K

Dec 24, 2024
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