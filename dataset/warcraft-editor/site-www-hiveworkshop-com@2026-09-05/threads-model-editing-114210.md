---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/model-editing.114210/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Model Editing
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/model-editing.114210/>
#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2

 How To: Edit Models

By Fabioz

Because this tutorial is so long, I am releasing this as an UNFINISHED product.

AGAIN: This is a work in progress!

But, constructive criticism is appreciated.

Introduction:

What will we be doing?

We will edit Blizzard's models. NOTE:Without animation.​

Are these programs free? And completely free, there isn't any trail version?

Yes, these programs are completely free!​

Tools required:

Magos War3 Model Editor

Oinkerwinkle's Vertex Modifier

BLP/TGA Converter

IrfanView

Warcraft III World Editor

Notepad Usually comes with your computer

Vertex Modifier:

Selection:
Select: When highlighted, any points you select will light up, and will be modified. But, if you select new points, these points will be unselected.

Add: When highlighted, any points you select will light up, and will be modified. But, if you select new points, the new points and the old points will all be slected.

Deselct: When highlighted, any points you select will be unselected. But not any of the other already selected points.

Transformations:

NOTE: When you want to translate certain vertexes, right-click, not left.
Move: When highlighted, any points selected will translate.

Rotate: When highlighted, any points selected will rotate around the center of mass.

Scale: When highlighted, any points selected will shrink or grow around the center of mass.

Extrude: When highlighted, any points selected will be cloned in the same position as the older ones, but translated to a new position. This is very useful.

Other Useful Things:

X: When highlighted, any points will not be affected in the X axis, during a transformation.

Y: When highlighted, any points will not be affected in the Y axis, during a transformation.

Z: When highlighted, any points will not be affected in the Z axis, during a transformation.

War3 Model Editor:

File:
New: Open a blank model. Not very useful.

Open: Open an existing model.  Extensions: .mdl or .mdx

Save: Save the open model. Extentions: .mdl or .mdx

Save as: Save a model under a certain file name.

Close: Close the currently open model. If you did not save the model, it will prompt you.

Import: Open a model from these extensions: .md2 and .ms3d.

Exit: Close the entire program.

Recent Files:
Clear: Clears the recent files list.

Edit:
Register Extensions: Register extensions so you can double click and quickly open files.

Calculate Extents: Calculates how much space (X,Y,Z) the model will use. You will probably never use this.

Properties: Edits the way the War3 Model Editor looks. Not very useful.

Model Properties: Here you can see the extent of you model and rename it.

Model Info: Here you can see statistics of your model.

View:
Resize Window...: You can return the window size to default.

Reset Camera: Set the camera to default.

Team Color:
Set the color that the current model will use as its team color.

Windows:
Model Editor: Here you can edit the mesh of the model. I will go through this in depth later.

Animation Controller: Control what animation the current model is doing.

Node Manager: This is very useful and can do many things, from particle emitters to creating omnilight. I will go through this in depth later.

Camera Manager: Creates camera angles for portraits.

Geoset Manager: Manage specific parts of a mesh, called geosets.

Geoset Animation Manager: Same as the geoset manager, but causes the model to change color or animate.

Texture Manager: Use, delete, or import textures from here.

Texture Animation Manager: Allows you to animate a texture. Like water.

Material Manager: Choose where you want to use textures along with team color.

Sequence Manager: Manage the animations.

MPQ Browser: A very important tool, allows you to browse through the MPQ file of warcraft. I will go through this in depth later.

Extras:
Loading Screen Creator: Assists in creating loading screens.

Colored Text: Creates the code for gradient and solid text colors.

Re-texturing a Model:

Acquire Resources:

 Go to windows and open the MPQ Browser. At the bottom of the list.

 A window should pop up, looking like this:

 Go to file and select either: War3.mpq or War3x.mpq. These will be the range of models/scripts/etc. you will browse. War3x.mpq is for the expansion.

 For this tutorial, I will be using War3x.mpq and using the model "Unit\Creep\Zombie\Zombie.mdx".

 Double click the model you want to use and look at the War3 Model Editor screen.

 Save the model as a .mdl file. Type: "Whateveryouwanthere.mdl" or select file type as .mdl.

 Next, open the MPQ Browser again and find the texture you want to use. Mine will be "Unit\Undead\Ghoul\Ghoul.blp".

 Don't open your file! Right-click the file and select "export". Save your file as whatever you want and where ever.

 Because the Vertex Modifier can't open .blp's, you will have to convert the file into .jpg.

 Copy and paste the BLP/TGA converter into the folder where your .blp file is.

 Click on your .blp file and drag it on top of the file convert program. Like this:

 Now that you have the .blp in .tga format, open the file in IrfanView.

 In IrfanView, save the .tga file as a .jpg file.

Re-texturing the Model:

 Open the Vertex Modifier and open your model file. Mine is the zombie

 It may ask which geoset. A geoset is a certain part of the model mesh, the part you want to retexture can be any of the geosets, you will have to find this on your own. My geoset I want is in the 1st geoset.

 Once you find the geoset you want to work in, go to tools and UV mapping. It should look something like this:

 Once the UV mapping window pops up, click on "Load Image" at the top right.

 Load your .jpg file of the texture you want to use. It should then look something like this:

 Now, all you need to do is move around the vertexs (the squares) and put the faces (triangles that make up the mesh) onto the parts of the texture that you want to show. Mine looks like this:

TIPS:
 Select certain vertexes and use the drop-down menu that says "Top", "Front", and "Left" and click the "Remap" button. This will make the selected vertexes look like if you were looking at the model from those views.

 Select vertexes outside of the UV mapper and they will be selected in the UV mapper when you go to it.

 When you are satisfied, click "Save Changes".

 Save your model and exit the Vertex Modifier.

 Open the War3 Model Editor and open your model. If your model looks like crap, don't worry. We haven't told the model to even use the texture.

 Go to the MPQ browser and look for the texture that you used. Right-click this texture and select "Use as texture".

 Go back and go to "Windows" and select "Material Manager". It should look like this:

 Double-click on one of the materials, and when a new window pops up, double click the material layer in this window. It looks like this:

In the next window you should see the texture that that material uses. If the texture is the one you re-textured, go into the drop-down menu and find your new texture. Mine looked like this:

 Now press ok and get back to the main screen. Your model should look beautiful! Mine look like this:

Editing the Mesh of a Model:

Editing the mesh of the model is easy, while re-texturing is not so easy.

 Open the War3 Model Editor and go to the MPQ browser, or open the file you want to edit.

 For this tutorial, I will be using "war3.mpq" and "Unit/Human/Knight/Knight.mdx"

 Now, with your model successfully loaded, save it as a .mdl.

 Open the .mdl file in Vertex Modifier.

 All I am doing is deleting the legs of the model. Here is where I selected and deleted:

 I had to delete the legs in two of the geosets, so check all the geosets! I also deleted the joust.

 This is where you can edit the mesh all you want. Make things bigger, delete things, shorten things, do anything you want!

 Now, my model looks like this:

Adding Particle Emitters:

Thank you to BlinkBoy for the explanation of many of these.

 With this model, I am going to make it "Ghosty". So, I am going to add a particle emitter.

 Go to the Node manager and right click in the window and select "Create Particle Emitter 2".

 Now, right-click the particle emitter and rename it (if you want) and type in the coordinates of where you want the emitter to be. Mine is at 0, 0, 20.

 Now double-click the emitter and edit the variables to your liking.

Particle Emitter Options
Visibility: Visinility can be animated with 4 types of interpolations: DontInterp(Default for On/Off visibility), Linear, Bezier and Hermite. Using a Global sequence on a linear/bezier/hermite animation frame of visibility ranging from 0 to 1 to 0, produces fading In-Out particles, which is quite nice to tingle effects.

Emission Rate: How many particles are released per second.

Speed: How many units per second the particles move

Variation: This varies the speed you have set. So if you set the speed to 30 and the variation to 20, each particle will travel at a speed between 10 and 50.

Latitude: The range goes from 0 to 180 degrees, the size of the degree will increase the degree of spread.

Width: Width of the emitter plane where particles are created randomly from its surface area.

Length: Length of the emitter plane where particles are created randomly from its surface area.

Note: Magos mixed width and length.

Gravity: This is how much gravity will play a part in your emitter. Setting this to a positive number will make the particles go down, while a negative number will make it go up.

Texture ID: This specifies the texture that is used as the particles.

Filter Mode: This changes the art of the texture, as much as I could figure out. This is almost always set to "Additive".

Segment 1: This is not seen. Unless "Time" is set to a different variable.

Segment 2: This is seen first (or second). This color mixes with segment 1 (If time is set correctly)

Segment 3: This is seen last, this color mixes with segment 2.

Alpha (0 - 255): This is how much of this emitter is visible. 0 = Invisible while 255 = completely visible.

Scale: This is the size of the particle.

Lifespan Head UV Anim: Starting and ending cell number for animating texture sequence of the first half of the Head particle’s life. Repeat will cause this animating texture to cycle.

Decay Head UV Anim: Starting and ending cell number for animating texture sequence of the second half of the Head particle’s life. Repeat will cause this animating texture to cycle.

Lifespan Tail UV Anim: Starting and ending cell number for animating texture sequence of the first half of the Tail particle’s life. Repeat will cause this animating texture to cycle.

Decay Tail UV Anim: Starting and ending cell number for animating texture sequence of the second half of the Tail particle’s life. Repeat will cause this animating texture to cycle.

Rows and Columns: Rows and Columms cut the image being emulated by rows and columms. This is made for animated texture particles. An example of how to use it is in the texture: "Textures\Clouds8x8.blp" you must set the rows to 8 and columms to 8, this will produce adecuate particles and can be animated.

Life Span: Starting and ending cell number for animating texture sequence of the first half of the Head particle’s life. Repeat will cause this animating texture to cycle.

Tail Length: I did not notice any changes when editing this variable.

Time: This is the amount of time each segment is to be seen. This allows segment one to be seen, if you set it to one.

Flags:
Unshaded: The particles do not use lighting and are always full bright.

Unfogged: Particles ignore distance fog.

Alpha Key: I do not notice any changes. Do not check

Line Emitter: Whether or not particles move in three directions, or two.

Sort Primitives Far Z: Keeps particles "in order". Keep this checked

Model Space: Whether or not particles move with the emitter.

XY Quad: Causes the particles to be a type of tail that orients to the XY grid.

Squirt: I do not notice any changes.

Head: The particle is a billboarded square quad.

Tail: A tail particle is billboarded along the axis of motion and stretches in length based on speed.

 My Particle Emitter Node looks like this:

 My model now looks like this:

Making the Model "Ghostly"

 Now that we have the particle emitter, the solid model looks weird, lets make it transparent.

 Go to "Windows" and select Material Manager.

 Double-click one of the Materials. Looks like this:

 Double-click the material in the next window, as such:

 In this window change the "Filter Mode" to Additive. Like this:

 Go through each material and change them all to additive. Then your model should look something like this:

 Next, I will tint the model blue so it looks ghostly.

 Go to windows and Geoset Animation manager.

 Open every single one and change the color to whatever color you want and check the "Use Color" box.

 With each one colored, check out the model by using the Animation Controller in "Windows".

 The model should now look like this:

View attachment 45872

That is the end of this part!

 This tutorial is presented to The Hive and the Helper!

More will be coming soon!

Please take a look at my other tutorials:

Voting with Dialog Boxes, Model Weapon Attachments with Particle Emitters, and Modeling Day and Night Models with Omnilight!

Attachments:


 Last edited by a moderator: Nov 26, 2011

#### Devine

#### Devine

 Level 30

JoinedSep 2, 2007Messages3,688

Resources: 10

#### Resources
Models 3Icons 6Tutorials 1

Very simple, you might want to give some more info.

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2

DevineArmy said:

 Very simple, you might want to give some more info.


Click to expand...


Well, I am explaining a simple concept; unless you are completely new to modeling, which is most of the audience I am trying to "teach".

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Well you have a lot of mistakes in the particle emitter's concepts, hope you can fix them.

 Visibility: 0 = Not Visible; 1-Infinity = Visible.


Click to expand...


Not wrong but incomplete. Visinility can be animated with 4 types of interpolations: DontInterp(Default for On/Off visibility), Linear, Bezier and Hermite. Using a Global sequence on a linear/bezier/hermite animation frame of visibility ranging from 0 to 1 to 0, produces fading In-Out particles, which is quite nice to tingle effects.

 Lattitude: This one is really neat! The measurments for this variable is degrees. 0 means all particles will travel up. If set on 360 particles will start from the middle and travel out in any direction.


Click to expand...


the range for angle is 0 to 180, 180 produces a wide spreading as well, a 90º produces a widespread at the facing of the emitter.

 Width: This is how long you want your particle emitter.


Length: This is how thick you want your particle emitter.


Click to expand...


"Width and Length: Width and Length of the emitter plane where particles are created randomly from it's surface." -ArtTools.

 Rows: This sets how many rows of particles are emitted. I do not recomend changing this. It will look boxy if you do.


Columns: This sets how many columns of particles are emitted. I do not recomend changing this. It will look boxy if you do.


Click to expand...


Rows and Columms cut the image being emulated by rows and columms. This is made for animated texture particles. An example of how to use it is in the texture: "Textures\Clouds8x8.blp" you must set the rows to 8 and columms to 8, this will produce adecuate particles and can be animated.


I guess I'll quote the Art Tools documentation instead:

 BlizParticle options

Count – Has no effect.

Speed – Initial velocity of created particles. Can be animated.

Variation % – Random variation % of speed.

Cone Angle – Angle of random spray pattern from center. A value of 180 will cause particles to emit in a 360-degree ball.


Timing options

Life – Number of seconds each particle continues to be drawn after its creation.

Parts/Sec – Number of particles created per second. Can be animated.

Squirt – When the Squirt option is checked and an animated stepped Bezier controller is used on parts/sec, a large number of particles will emit simultaneously on the parts/sec keys in the amount of the key value. This is good for explosion effects.

Width and Length – Width and length of the emitter plane where particles are created randomly from its surface area.

Line Emitter – Cause the cone spray pattern to lock to one axis.

Texure options


Blend, Add, Modulate, Mod, Mod 2X, and Alpha Key - Identical to the options in the Warcraft III material. Alpha Key is also known as 2 Bit Alpha.

Texture Filename – The .TGA texture being used for the particles.

Rows and Cols – If the particle is using sections of the texture at a time, you can use these options to set the number of rows and columns in a texture. Animating texture particles can be made to have frames all on one texture map that start at the top left and go to the right.

Unshaded – The particles do not use lighting and are always full bright.

Unfogged – Particles ignore distance fog.

Replaceable texture – Can be set to team glow to create procedurally colored glowing particles for a unit.

Particle options

Head – The particle is a billboarded square quad.

Tail – A tail particle is billboarded along the axis of motion and stretches in length based on speed.

Both – Draws both heads and tails.

Tail Length – A multiplier to the calculated tail particle length.

Time (parametric) Middle – The relative time of the middle key to the life.

Color, Start/Middle/End – Three values of RGB Multiply for the particle.

Alpha, Start/Middle/End – Three alpha values for the particle. Zero is transparent.

Scale, Start/Middle/End – Scale of the particle in units across.

Lifespan Head UV Anim – Starting and ending cell number for animating texture sequence of the first half of the Head particle’s life. Repeat will cause this animating texture to cycle.

Decay Head UV Anim - Starting and ending cell number for animating texture sequence of the second half of the Head particle’s life. Repeat will cause this animating texture to cycle.

Lifespan Tail UV Anim - Starting and ending cell number for animating texture sequence of the first half of the Tail particle’s life. Repeat will cause this animating texture to cycle.

Decay Tail UV Anim - Starting and ending cell number for animating texture sequence of the second half of the Tail particle’s life. Repeat will cause this animating texture to cycle.


Other options

Priority Plane – When you are using blend mode, the Priority Plane option can cause particles to draw in a specific order with other blend objects in the same model. The lower the number, the earlier the particles will draw.

Particles in Model Space – Causes animation of the particle emitter to be carried over to the particles.

XY Quads – Causes the particles to be a type of tail that orients to the XY grid.


Click to expand...


Specially notice Head LifeSpan UV Anim, Head Decay UV Anim and Tails respectively. You completely got those concepts wrong


Anyways I would suggest you to add some animation transfering, Geoset merging and creation(creating geos for your model) in order for it to be approved. Also change the title to Model Editing: Beginners Start.

#### Devine

#### Devine

 Level 30

JoinedSep 2, 2007Messages3,688

Resources: 10

#### Resources
Models 3Icons 6Tutorials 1

Fabioz said:

 Well, I am explaining a simple concept; unless you are completely new to modeling, which is most of the audience I am trying to "teach".


Click to expand...

Info, not as in adding content but as in being clear.

and That was quite a nice review, Blinkboy.

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2

Very nice, BlinkBoy.


I will add what you have told me and I will put your name on it for reference.

#### Flame_Phoenix

#### Flame_Phoenix

 Level 20

JoinedMay 4, 2007Messages2,260

Resources: 12

#### Resources
Tools 1Maps 1Spells and Systems 7Tutorials 3

I am a complete newb to 3D, butI found the 2nd part of the tutorial quite easy and nice. IMHO the 1st section jumps some steps that could be important for newbies like me, but the 2nd is really good. I like the knight ghost model, keep on the good job!

#### Bloodezach

#### Bloodezach

 Level 3

JoinedMar 29, 2009Messages35

I tried downloading that Oinkerwinkle's Vertex Modifier and for some reason i says im missing a dependency and i have gone to other websites downloading and still says that. is there any other programs i have to download for it to work i would really like to remodel but i got this giant wall in my way. Plz anyone help me.

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2

what operating system do you have? Windows XP, Windows Vista, Macintosh, Linux?

#### Bloodezach

#### Bloodezach

 Level 3

JoinedMar 29, 2009Messages35

Windows XP

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2


I don't know why then, I also have windows XP... Perhaps completely remove it with uninstall and reinstall..

#### Dumbdumb36

#### Dumbdumb36

 Level 7

JoinedJan 13, 2008Messages201

Resources: 1

#### Resources
Maps 1

So how would I flip the face? its showing the wrong half

#### RaijinSpark

#### RaijinSpark

 Level 19

JoinedAug 31, 2008Messages1,774

Resources: 24

#### Resources
Models 1Icons 19Tools 1Maps 3

i can't open Model Editor T.T

#### Nublu

N

#### Nublu

I can open it just fine, i have windows XP. I just downloaded it from this forum and opened it in winzip.

#### vegetation

#### vegetation

 Level 5

JoinedJul 20, 2008Messages59

Resources: 3

#### Resources
Maps 3

I can open everything except when i click on the model i want to edit ( i took the same as you, the zombie) it doesn't open in the war3 model editor can you help me please?

#### Fabioz

#### Fabioz

 Level 5

JoinedOct 18, 2007Messages19

Resources: 2

#### Resources
Tutorials 2

Are you opening it with the mpq browser or opening it when you have the program open? I know that if you simply savea model into your documents or something and try to double click it, it does not open.... Other than this, I dont know why its not working.

#### sieved_19

#### sieved_19

 Level 3

JoinedJul 28, 2009Messages23

Resources: 1

#### Resources
Tools 1

In The Vertex Modifier What Decimal I Will Use In Regional Option In Windows 7 And I Have The File _____________.OCX

#### Ravernomus

#### Ravernomus

 Level 3

JoinedJan 23, 2008Messages37

Little note about the following:

Re-texturing a Model:

Acquire Resources:

 1. Go to windows and open the MPQ Browser. At the bottom of the list.

 2. A window should pop up, looking like this:

 blankmpq.jpg

 3. Go to file and select either: War3.mpq or War3x.mpq. These will be the range of models/scripts/etc. you will browse. War3x.mpq is for the expansion.

 4. For this tutorial, I will be using War3x.mpq and using the model "Unit\Creep\Zombie\Zombie.mdx".

 5. Double click the model you want to use and look at the War3 Model Editor screen.

 6. Save the model as a .mdl file. Type: "Whateveryouwanthere.mdl" or select file type as .mdl.

 7. Next, open the MPQ Browser again and find the texture you want to use. Mine will be "Unit\Undead\Ghoul\Ghoul.blp".

 8. Don't open your file! Right-click the file and select "export". Save your file as whatever you want and where ever.

 9. Because the Vertex Modifier can't open .blp's, you will have to convert the file into .jpg.

I "found" the way how to do it with out converting, you can do it so: Right-Click on the texture and choose Use It As Textures, then you go to Texture Manager and Right-Click on the that texture choosing 'Export'. While exporting, you'll have to choose the type of file you want that to texture to be exported as. I guess the options of choice depend on computer, but my PC (Win Vista) gives me next options to save the exported file as: *.bmp, *.tga, *.png, *.jpg, *.jpeg, *.pcx, *.blp. Simply type a dot right after the file name and type the desired file type you want that file to be exported as (e.g. Kinght.blp or Knight.jpg)

#### Decist

#### Decist

 Level 3

JoinedApr 5, 2008Messages41

Thanks for this!

#### Deathruler0166

D

#### Deathruler0166

umm wtf when i try to download IrfanView it says resource unavalible or whatever??

#### Zary256

Z

#### Zary256

I dont get it.where is the MPQ browser?


Its not at the list!!!!

#### Deathskall

#### Deathskall

 Level 2

JoinedAug 24, 2009Messages18

you are neglecting to even mention how to move the vertices btw...

#### ubran

#### ubran

 Level 5

JoinedSep 29, 2008Messages170

So... do we HAVE to use oinkerwinkle's or can we actually edit/add to the model in War3Modeleditor? I'd like to save some trips and just edit the mesh of the huntress into using a sword/spear instead of the three bladed thing. Any way to add new vertices into a model or am I stuck with the ones in the huntress file from the mpq?


 Last edited: Jan 26, 2010

#### clockwork2

#### clockwork2

 Level 15

JoinedJan 26, 2010Messages1,629

Resources: 4

#### Resources
Models 2Maps 2

Hey, I run through your steps with the zombie etc and when i try to load the zombie in MDL Vertex Modifier, it gives me the error message "Run-time error '9': Subscript out of range" Any ideas?


dude help my blp converter isn't working it only shows up for one sec then it vanishes!!!!HELP!!!!


ubran i believe that is a shurikun the huntress usually uses.a weapon favored by ninjas.and huntresses apparently.lol


zary if i am correct that is in the windows i think


yup at the very bottom of the windows tab


holy cow help people help help help!!!!my model is starting to mess up i wanted to make a captain paladin!and i don't know what to do..


also i hate my model he's currently weird


 Last edited by a moderator: May 1, 2010

#### Snacker

#### Snacker

 Level 6

JoinedMar 5, 2010Messages149

Resources: 2

#### Resources
Maps 2

I always got a problem when I'm trying to start Oinkerwinkle's Vertex Modifier. And I'm using Windows XP SP3.

#### Yagami_MK

Y

#### Yagami_MK

Great tutorial! Im starting now with model and i learned a lot! Thank you all! =D

#### nixlas

N

#### nixlas

Umm...

My Vertexmodifier doesn't work when i open it it says ''Warning: You are using commas as a decimal instead of periods. The program will not work with that setting. Go to http://www.geocities.com/oinkertools/tutorials.html for instuctions on changing that'' but the site is closed so i press ''OK'' in the warning screen and when i try opening my model (name is ''Zombie.mdx'') it says ''Run-time error '9': Subscript out of range'' what shall i do?


PS. I have vista if it helps

#### Snacker

#### Snacker

 Level 6

JoinedMar 5, 2010Messages149

Resources: 2

#### Resources
Maps 2

nixlas said:

 My Vertexmodifier doesn't work when i open it it says ''Warning: You are using commas as a decimal instead of periods. The program will not work with that setting. Go to http://www.geocities.com/oinkertools/tutorials.html for instuctions on changing that'' but the site is closed so i press ''OK'' in the warning screen and when i try opening my model (name is ''Zombie.mdx'') it says ''Run-time error '9': Subscript out of range'' what shall i do?


PS. I have vista if it helps


Click to expand...


I had it too, but I found a tutorial in German (I know the basics in the German language) and here's how you do:


I'm not exactly sure about this, since I use XP, but I hope this will atleast help you:


Control Panel, or whatever it's called, Regional and language settings, Customize, something with decimal and replace the "," with ".".


If it helped I would be glad if you +Rep me.


EDIT: Link: http://warcraft.ingame.de/forum/printthread.php?threadid=91741


Picture guide:

#### Attachments

1.PNG

 88.4 KB

 · Views: 368



2.PNG

 103.2 KB

 · Views: 398



3.PNG

 72.1 KB

 · Views: 335



4.PNG

 79.5 KB

 · Views: 363



5.PNG

 791.6 KB

 · Views: 6,262



6.PNG

 791.5 KB

 · Views: 6,316



#### Kid123

K

#### Kid123

Whenever I try to browse for MPQ files an error comes up saying that Windows cannot open the file. Why does it do that?

#### Miss_Foxy

#### Miss_Foxy

 Level 19

JoinedOct 15, 2008Messages3,228

How do you find the Model File from the Vertex Modifier after opening it, I can't seem to locate the file...

#### blazefreeze

B

#### blazefreeze

I have a problem. When I save a file from the Model Editor as mdl file. I get a Runtime Error that says "R6002 Floating Point Support Error Not Loaded". When I tried to export a unit mdx as mdl and open it with Vertex Modifier, I got an error named "Runtime error 9 Subscript out of range."


Can someone help me with this

#### NeW v0id

N

#### NeW v0id

It says component COMDLG32.OCX missing.

 what is that mean?

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

NeW v0id said:

 It says component COMDLG32.OCX missing.

 what is that mean?


Click to expand...

It does it to me too. I called a friend who got XP and 7 and he says it works to him in both systems. I'm going to him to use animtransfer. lol

EDIT: i did it on my own. Go to oinkerwinkle site, then download the file from there and run the program as administrator.


 Last edited: Jan 4, 2011

#### Thunderblood

#### Thunderblood

 Level 3

JoinedJan 2, 2011Messages34

BLP/TGA converter not working for me.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

For those with .OCX errors, I suggest you go to this site:

http://www.ascentive.com/support/new/support_dll.phtml?dllname=COMDLG32.OCX


Follow the instructions from there. However, I suggest downloading this instead:

http://www.ascentive.com/support/new/libraryfiles.exe


Which will do it for you, and will install a bundle of other library files needed to prevent errors.

#### Twosocks

#### Twosocks

 Level 2

JoinedMar 22, 2011Messages12

Uber helpful! +Rep

#### clockwork2

#### clockwork2

 Level 15

JoinedJan 26, 2010Messages1,629

Resources: 4

#### Resources
Models 2Maps 2

this is a nice tutorial...could you make one on how to make an axe????also could you make on how to use different textures for different parts of a models body??

if you can,THANK YOU THANK YOU THANK YOU

and also  thats my thumbs ups bro...

also the irfanviews resource is gone...what program should i use instead????i don't have any old version and for some reason i can't import the knights dust texture!!!!someone help me with this problem,it does the same thing with everything else,it also says some bogus about the file not existing!!!nevermind apparently the texture actually has to be in the mpq tab weird.....HHHHHHHHHEEEEEEEEEEEELLLLLLLLLLLLLPPPPPPPPPPPPP

Lordperenolde i'd just like to say....your campaigns rock but...i keep having to starting over???was it like that with warcraft orcs and humans??? as i had reached grand hamlet attack on the orc campaign..and i quit and now i have start over at the swamp of sorrows also did you really die??


 Last edited: Apr 19, 2011

#### Cakedog345

#### Cakedog345

 Level 12

JoinedJun 13, 2011Messages738

Resources: 3

#### Resources
Skins 3

How to open UV mapping?

#### uracccp

U

#### uracccp

please show me how to rename each animation of a model

thanks a lot

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

@cakedog - depends on the program ur using...

@racccp - you can do that on the sequence manager of Magos Model Editor...

#### Lucasvn8

L

#### Lucasvn8

Oinkerwinkle's Vertex Modifier This Account Has Been Suspended wtf help!

#### owenfitz

#### owenfitz

 Level 6

JoinedApr 20, 2010Messages26

Resources: 7

#### Resources
Models 6Packs 1

i created a custom model following these instructions. However my model does not show up in world editor or game. i have already tried testing map and restating world editor. What do i do?


update: I messed around with it and he shows up ingame but not in world editor. also the portrait of this on you bottom bar ingame is just gray, it has no portrait


 Last edited: Nov 20, 2011

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Lucasvn8 said:

 Oinkerwinkle's Vertex Modifier This Account Has Been Suspended wtf help!


Click to expand...


Sorry, I've updated the link. Apparently Oinkerwinkle took down his site. (or it may just be down) Luckily, wc3c.net still has the links for download.

owenfitz said:

 i created a custom model following these instructions. However my model does not show up in world editor or game. i have already tried testing map and restating world editor. What do i do?


update: I messed around with it and he shows up ingame but not in world editor. also the portrait of this on you bottom bar ingame is just gray, it has no portrait


Click to expand...


It could be for a number of reasons. I recommend that you create a new thread in the modeling forum to get your question answered.


Anyway, try deleting it, removing the object editor references to it, saving, closing WE, reopening, then importing it again. It may fix the problem of not showing the editor.


You also should be sure that the default animation has the model visibly shown.

#### Biglover

#### Biglover

 Level 2

JoinedFeb 14, 2020Messages19

Excellent tutorial. Just what I needed.

#### Draktar

#### Draktar

 Level 12

JoinedMar 10, 2013Messages114

Resources: 2

#### Resources
Skins 1Ported Models 1

nixlas said:

Umm...

My Vertexmodifier doesn't work when i open it it says ''Warning: You are using commas as a decimal instead of periods. The program will not work with that setting. Go to http://www.geocities.com/oinkertools/tutorials.html for instuctions on changing that'' but the site is closed so i press ''OK'' in the warning screen and when i try opening my model (name is ''Zombie.mdx'') it says ''Run-time error '9': Subscript out of range'' what shall i do?

PS. I have vista if it helps


Click to expand...

heres a tutorial if if needed

How to Customize Numbers Format on Windows 10

#### KaiZen7th

#### KaiZen7th

 Level 11

JoinedMay 25, 2021Messages407

Resources: 3

#### Resources
Icons 1Spells and Systems 1Reforged HD Icons 1

Hello.

Thank you so much for your massive contribution.

But I'm afraid this link is DEAD:

IrfanView

 You must log in or register to reply here.


### Similar threads

 How to make XY Quad Particles Emitter ( And Rain Drop Effect Tutorial )


Anvil

Aug 27, 2012

3D Modeling Tutorials

Replies10Views14K

Jul 9, 2017
Veronnis

 Teamcolor Particles


Anvil

Nov 28, 2012

3D Modeling Tutorials

Replies4Views7K

Dec 19, 2012
Anvil

 Scratch-Made Weapon Attachments with Particle Emitters


Fabioz

Apr 8, 2008

3D Modeling Tutorials
2

Replies52Views42K

Oct 13, 2010
Ikithe

 Create Models only with free tools


D a w n

Nov 16, 2008

3D Modeling Tutorials
234

Replies174Views87K

Jun 10, 2014
clockwork2

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

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