---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/create-models-only-with-free-tools.107544/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Create Models Only with Free Tools
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/create-models-only-with-free-tools.107544/>
#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

this tutorial is not finished yet, but it´s already finished enough to learn something from it.

First of all I´m going to tell you what you could learn in this tutorial:

you will learn how to create a model with the help of the free modeling programs available for warcraft.

The following tools will be used:

War3 Model Editor

MDLX Converter - The Hive Workshop - A Warcraft III Modding Site

Vertex Modifier, Geoset Merger and Animation Transfer from this site: Oinkerwinkle's Warcraft 3 MDL Tools Download Page

Creating a model contains of 2 parts which I will talk about: 1. the idea and the modeling

1. the idea:

this can be the most important part, because the idea can already determine if the model will be good or not.

Some examples: a bad idea would be if you want to create a footman model with Arthas head and grunt animations because it´s not creative.

But a good idea would be to create an animal soldier or space orcs like General Frank did.

But this problem could appear: if you cannot create your own textures (which means that you will use the ones that are already ingame) it could be possible that you won´t be able to realize your concept. But usually this will not happen.

And this leads me to the 2. point:

2. the modeling.

First of all I will give you a overview of the different processes:

2.1 Download the tools that are listed at the top of the tutorial.

2.2 Open Magos model editor and search for good geosets.

2.3 Extract the geoset into a new folder and convert them from mdx to mdl with the file converter.

2.4 Open them with verttex modifier and delete unneeded parts.

2.5 Change the used texture into another one and make it fit the model shape ( changing the uv map).

2.6 Merge all of our geoset into 1 model.

2.7 Give the model animations.

2.8 Add some eye candy to it.

2.9 Convert it back into mdx.

2.10 Finished.

this sound really confusing right now, doesn´t it?

But it will be clear when I go into detail.

after you have finished 2.1 we will move to 2.2:

the Magos model editor.

before we do anything with it I will explain you every part of it and will tell you which options are important and which aren´t.

this is the head line of the editor.

I will tell you what you can do with the not so needed options now.

under File you can: open a model and save a model.

under Recent Files you can: open the last open models faster.

under View you can: change the camera angle back to the usual one.

under Team Color you can: Change the shown team color of the model you can see.

under Extras you can: create a Loading Screen and you can create colored warcraft texts with it.

under Help you can read about the model editor and there are also some small tutorials.

But now I will tell and explain you you the important parts of he model editor:

Edit:the important things are: Properties, Model Properties and Model Info.

Properties: this is useful because you can see how many particles are created when you enable Debug Information.

you can also create a ground texture to see if your model would fly in the air or if the legs would be on the ground ingame.

Model Properties: this is useful because we will be able to change the model name which we will have to do if we want to upload our model.

Model Info: this is useful because we can see how many polies, geosets, animations and a lot of other stuff our model has.

this is the most important part because here we will be able to edit our model but I won´t tell you now how to do it, because this will be done at a later point.

Right now we just need: Texture Manager, Material Manager and MPQ Browser.

Under Texture Manager you can see the used textures and you can also add team color and glow to your model there. You can also extract textures there.

Under Material Manager you can change the used texture of a selected geoset.

Under MPQ Browser you can search for: models and their textures.

When you browse for models you should search at: Buildings, Doodads and Units, but when you search for Textures you should search at: Buildings,Doodads,Textures,UI, and Units but there are also 1 or two more places where you could find a good texture.

Sometimes it can take really long to find good geosets and textures for you idea.

We will search for fitting geosets for our model now. We could search under war.mpq war3x.mpq or war3patch.mpq for them.

After some time I found a good shape for the head: the Knight_Portrait model. ( war3.mpq->units->human->Knight)

We will extract this model now by selecting Files-> Save as

after you have saved it under a new folder we will have to change the file from mdx into mdl with the file converter we have downloaded.

Then we will open it with the vertex modifier.(select file-> open)

You will see this:

open geoset 1.

you can see 3 windows with a +, a - and 4 arrows next to it

first zoom out(-) and you will see some lines. use ther arrows the to get those lines into the center of each window. the zoom in(+)

you will see something like this:

now you can see the shape of the model.

With the left mouse button you can select and with the right mouse button you can edit the selected points(verteces)

select 1 point of the bottom of the model and now select: tools-> expand selection. You will see that the program seleted more points now because the are connected with each other. select delete now.

You will continue this now untill you only have something like this left:

what I´ve done is: deleting the body,the eyes and the beard. ( the model is also attached at the end of this tutorial)

Now we will save the model. Select: file-> save geoset

When you open the model with magos model editor now you will see that it looks like this now:

Now we will search for a good texture for the head because we have 1 goal: we want to have as much work as possible because of the lack of a vertex creating program we will already work with finished meshes and if we would take the Arthas head for the Arthas texture we would have even less work and would mean that we even used more of blizzard finished stuff.

I found the magnataur texture for the model ( war3x.mpq->units->creeps->magnataurblack)

we right click the texture in the mpq browser now and selct: use as texture.

Then we will hold ctrl and left click the head.

We open the Material Editor now and select the blue marked Material.

Now we change the texture to the magnataur texture.

The head will look like this now:

as you can see: It´s a mess. That why we will open the Texture manager now and we will extract the knight and magnataur texutre now.( right click the texture and select: export and save it as .bmp)

we will save the model now with the messy uv map.

Then we will open vertex modifier again and open our model.

We will select this now:

you will see that a new window opened that looks like this:

we will open the knight texture in this window now.( load image)

you will see that those with lines connected dots are on the knight head, but if we open the magnatuar texture now, the head if it will be at another place. That´s why we have to remap the model. First select all those dots and then you can select 3 options under Remap: top, front and left. But we will select front.

It will look like this now:

You can see the whole front of the head now, but without any texture.

But we have 1 problem now: Usually every texture does not show both halfs of something ( for example the left and right arm) but only 1 side. In our example it would be like this: The Knight texture has the head on the right side of it--> It only shows the left half of the whole head. But the Magnataur head is on the left side and that´s why it only shows the right half of the face. If both would have shown the same half of the face there wouldn´t have been any reason to remap the head, but this is not the case in this example.

Here is are some small drawing to show what to do:

That´s why we have to put 1 half of the face on the other half. We are mirroring the face that way. That way we will have the whole head covered with the texture and not only 1 half of it.

It will look like this now:

Now we can move it to the position where the Magnatuar head is.

But we will still have to change the postition of many dots to get the best result. Save the new uv map.

It could be also possible that we want the back of the head to have another part of the texture. Then we will have to get to the vertex modifier and select the back of the head. Then we select Edit-> Invert Selection. You will see that the red dots become black and the black dots become red. Now we select delete and save the model as: 1modelname.mdl.

Then we select edit-> undo xyz. In this case it will be undo delete.

Then we select invert selection again and select delete again. you will see that this time we have removed the other part of the model.

We now we select: file-> merge geoset and will choose the 1modelname.mdl.

It could be possible now that the merged part is not selected.

That why we check the undo part.If there stands: undo merge then it´s ok but if there stands: undo selection we will have to select it so we only have the merged part selected. If our selected part is the front of the head we will select invert selection again.

Now we are using the uvmapping editor again. This time you will see that there are already some red dots.Thos are the selected verteces of our model. We will select remap now and if we choose front again then we will have to mirror the new uvmap again just we did it last time.

In our example the new uv map looks like this:

Now we could change the model a bit like: making it rounder or just larger.

Now we will change our model/geoset.

When we open it in Magos model editor now it looks like this:

But if we change the used texture to the one of the knight again it will look like this:

Update: you can download a video about unwrapping here: https://www.hiveworkshop.com/forums/1360740-post118.html

Here is a post that discribes how to add merged parts of a model to the bones it is supposted to use:

https://www.hiveworkshop.com/forums/712584-post4.html

if you want to make a single animations transfer with oinkerwinkles animations transferer, you will have to care for these points to make it work: https://www.hiveworkshop.com/forums/1437134-post3.html

#### Attachments

Knight_PortraitAfterChanges.mdl

 22.4 KB

 · Views: 697



Knight_PortraitBeforeChanges.mdl

 21.5 KB

 · Views: 753




 Last edited by a moderator: Jul 1, 2013

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

Changelog:

1.03 New Link to the Model Editor because there is a newer version of it.

1.02 Improved a small part of the tutorial. (removed typos, used beter words etc.)

1.01 Changed the images to thumbnails because they are quite big.

 Improved 1 part of the tutorial that is quoted in this post: https://www.hiveworkshop.com/forums/912929-post4.html

1.00 Uploaded the first parts of the tutorial

#### Attachments

magosmodeleditoredit.JPG

 26.9 KB

 · Views: 3,132



magosmodeleditorwindows.JPG

 39.2 KB

 · Views: 3,188



magosmodeleditor.JPG

 19.6 KB

 · Views: 3,214



open.JPG

 39.8 KB

 · Views: 3,341



windows.JPG

 46.7 KB

 · Views: 3,100



head.JPG

 47.7 KB

 · Views: 606



headmagos.JPG

 39.7 KB

 · Views: 732



knightmagnataurbefore.JPG

 18.3 KB

 · Views: 775



uvmapping.JPG

 49.2 KB

 · Views: 623



uvmappingscreen.JPG

 61.9 KB

 · Views: 617



uvmaphead.JPG

 57 KB

 · Views: 625



uvmapheadhalf.JPG

 36 KB

 · Views: 620



newmap.JPG

 64.5 KB

 · Views: 766



knightmagnataurafter.JPG

 24.7 KB

 · Views: 811



knightknightafter.JPG

 30.5 KB

 · Views: 808



Knight_PortraitBeforeChanges.mdl

 21.5 KB

 · Views: 515



Knight_PortraitAfterChanges.mdl

 22.4 KB

 · Views: 522



whattodo.JPG

 40 KB

 · Views: 810



uvmappicture.jpg

 32.8 KB

 · Views: 928




 Last edited: Jan 9, 2009

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Wow, cool. I mean, I don't have time to really learn from it right now, but I didn't really understand UV Mapping, and this makes some sense.


To change it, I'd suggest spelling/grammar. I'd even offer to help, if I didn't have a research paper to right.


But cool stuff.

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

Well....I think I´ll finish the whole thing first and then I will improve grammar and spelling or I will create a request thread for it.

But it would be really good if you would tell me if something was not clear so I could improve it in the tutorial.

Btw: Do you think I should also add some short video tutorials between the actual one to show different functions of the tools??

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Hmm...


1. The pictures are rather huge; while it's nice to see all the action. Also, some of them are big pictures of a small part of the screen (like just the toolbar on Magos).


I'd suggest finding some way to "thumbnail" the pictures so they take up less room (makes the text easier to read and less "jumpy"). Maybe, also, take a screenshot of Magos entirely, but then somehow "highlight" (like a big red circle, or an arrow, or actually highlighting) the button you want us to push.


Videos could also be cool. Do they let you upload and link them directly to this site?


2. This part:


 DonDustin said:



 as you can see we can see the whole head, but we have to mirror the head because the magnataur texture only shows the right side of the head.First disable the y-axis and then always select 1 dot on the left and move it to the dot which is on the right side of it.


Click to expand...

Is kinda confusing, for people who have never done it. I get what you're trying to do, but not really how you did it. Better explanation, plz (or just a movie??)

#### Devine

#### Devine

 Level 30

JoinedSep 2, 2007Messages3,688

Resources: 10

#### Resources
Models 3Icons 6Tutorials 1

I only read a few lines ...

what a bunch of typo's!

#### Vestras

#### Vestras

 Level 9

JoinedDec 26, 2007Messages202

Resources: 13

#### Resources
Models 9Tools 1Maps 2Tutorials 1

 You can see the whole front of the head now, but without any texture.


But we have 1 problem now: Usually every texture does not show both halfs of something ( for example the left and right arm) but only 1 side. In our example it would be like this: The Knight texture has the head on the right side of it--> It only shows the left half of the whole head. But the Magnataur head is on the left side and that´s why it only shows the right half of the face. If both would have shown the same half of the face there wouldn´t have been any reason to remap the head, but this is not the case in this example.


Click to expand...


That's where I fell off. Please describe yourself better.

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

I already improved that part D:

It basicly means: in every Texture there is not drawn the whole head but only 1 side of it. And the reason for it is to save space for other parts. And we have to do the same with the model parts we want to have that part of the texture.

Ok it´s time for some drawing now^^

Edit: Added a drawing to show you what I meant in that part


 Last edited: Nov 17, 2008

#### 3rgytr54

#### 3rgytr54

 Level 11

JoinedApr 6, 2008Messages562

Resources: 12

#### Resources
Models 10Packs 1Tutorials 1

Great, there is the occasional typo but not everyone is prefect. This makes more sense than any other uvw mapping tutorial around. (I have read most of them trying to figure this out) I wish this was around when i was trying to learn it, rg. +rep

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Approved.

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

......and it is not even finished at the moment

I will change it into some kind of lessons which will contain merging different parts together, adding a skeleton and some other stuff.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

lolwhoops

Should I move it back then? D:

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

I don´t mind because 1 part is already finished

#### You_suck

Y

#### You_suck

I made an account today for one purpose, to tell you (Mr. Don) that you are terrible at tutorials, it doesn't even make sense. You replace words like Material manager, with material editor, and Magos Editor with War3model viewer, it's terrible. You jump from place to place without explaining ANYthing. Not to mention all of the typos as well. I would hope you fix this, or SOMEONE on here can link me to something Semi useful, on making models.

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

You_suck said:

 I made an account today for one purpose, to tell you (Mr. Don) that you are terrible at tutorials, it doesn't even make sense. You replace words like Material manager, with material editor, and Magos Editor with War3model viewer, it's terrible. You jump from place to place without explaining ANYthing. Not to mention all of the typos as well. I would hope you fix this, or SOMEONE on here can link me to something Semi useful, on making models.


Click to expand...

1. Even if I used "Material Manager" instead of "Material Editor" you could still think that I mean exactly this thing if there is nothing else with the word material in it.

And Magos Editor= War3model viewer ( I mainly called it like that because it was created by that guy)

2. Yes I know that there are typos and I already improved a part of this tutorial some time ago.

3. I told you that it is not finished at the moment.

4. Thank you for your comment. It was a bit useful for me.

#### 3rgytr54

#### 3rgytr54

 Level 11

JoinedApr 6, 2008Messages562

Resources: 12

#### Resources
Models 10Packs 1Tutorials 1

Then, you suck, write a tutorial your self BEFORE you can criticize others. If DonDustin's native language is English, well then he might need to go back to school. And if not, between the typos it gets the point a across.

If you want the whole thing 99.9% typo free, I would be glad to copy and paste it into a post of mine here and fix the tutorial in

 tags.


 Last edited by a moderator: Jan 4, 2009

#### RipVanWinkl

R

#### RipVanWinkl

I have one qustion:


How can I load the Image in the Vertex Modifier (UV Mapping)


The first problem is that i can only find jpeg pictures.


The second problem is that i converted the magnataur.blp.mdx (or .mdl) to a Jpeg but if i load This Image i get an Error.


Would be nice if ucould help me.

#### Petepet

#### Petepet

 Level 2

JoinedJun 29, 2008Messages18

When I open the .mdl file with the war3 model editor I dont see something. Every thing there is grey and I can't see the model (the head in your example)

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

Petepet said:

 When I open the .mdl file with the war3 model editor I dont see something. Every thing there is grey and I can't see the model (the head in your example)


Click to expand...

could you show a screenshot of it please??

#### Petepet

#### Petepet

 Level 2

JoinedJun 29, 2008Messages18

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

here are 2 interesting things that could help you:

https://www.hiveworkshop.com/forums/560003-post6.html

and there is a new version of it:

War3 Model Editor

#### Petepet

#### Petepet

 Level 2

JoinedJun 29, 2008Messages18

mmh i downloaded the new editor(1.07) but it didn't work.


I have Direct X 9.0c ... is this the problem?

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

I don´t really think so, bt you could try to update it and then we will see if it works.

Oh and another important thing: You need roc and tft to use this tool :/

#### Petepet

#### Petepet

 Level 2

JoinedJun 29, 2008Messages18

i have tft an roc. The tft is 1.22 and so on

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

Oh this is what I found on the site of the creator( Magos)

http://home.magosx.com/index.php?PHPSESSID=78577b05f1af2bfa95832aeb5a9bf084&action=profile;u=25

Leve Newbie

 Offline

 Posts: 1

Re: War3 Model Editor

 « Reply #21 on: July 12, 2008, 03:52:15 AM »

 Hmm I've just got some problem..the screen turns gray when I try to open it..I have no idea where it comes from and I have DirectX ofc.

 Logged Magos Godlike

 Administrator

 Newbie

 Offline

 Posts: 38

Re: War3 Model Editor

 « Reply #22 on: July 12, 2008, 06:21:50 AM »

 Some poeple have reported that problem and sadly I have no solution for it. Are you running on a laptop, or have some kind of integrated graphics card?

#### Petepet

#### Petepet

 Level 2

JoinedJun 29, 2008Messages18

f***^^ i have a notebook but I can try it at my computer

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Holy cow, I just realized I was about to ask you how you make models with free-use programs... Dang.


Ok, now I really have a vested interest in seeing this completed. Especially the animating part (although UV mapping is still difficult); I've never understood how you can get anims to work right.


More power to ya, Don.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

If it matters, I'm on a laptop and I've never had problems like this.

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

And personally I don´t understand why it shouldn´t work on it.

Too bad that we can´t find the reason for it :/

#### s4nji

#### s4nji

 Level 15

JoinedSep 1, 2008Messages475

Resources: 23

#### Resources
Models 20Packs 2Spells and Systems 1

Umm, I have the War3ModelEditor with the version 1.05, but when I see the newer version (1.07?) I downloaded it, after I extracted it, I open the War3ModelEditor.exe It says that "d3dx9_35" was not found, I see on the magosx but it says that I must download the DirectX 9, can someone attach it please, but only the "d3dx9_35" component not all of them.

EDIT : I have the DirectX 9 but the "d3dx9_35" isn't there, I cannot find it in the DX Diagnose, the only thing it has only "d3d9.dll" but not the "d3dx9_35" please somebody attach this...


 Last edited: Jan 10, 2009

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

s4nji said:

 Umm, I have the War3ModelEditor with the version 1.05, but when I see the newer version (1.07?) I downloaded it, after I extracted it, I open the War3ModelEditor.exe It says that "d3dx9_35" was not found, I see on the magosx but it says that I must download the DirectX 9, can someone attach it please, but only the "d3dx9_35" component not all of them.


EDIT : I have the DirectX 9 but the "d3dx9_35" isn't there, I cannot find it in the DX Diagnose, the only thing it has only "d3d9.dll" but not the "d3dx9_35" please somebody attach this...

Click to expand...

Just Fucking Google It

#### Petepet

#### Petepet

 Level 2

JoinedJun 29, 2008Messages18

... at the computer it doesn't works too. I can see the model in the .mdl file but after i used the VertexModify i doesn't works

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

hm.....

so you have the same problem that you have with the laptop??

#### Petepet

#### Petepet

 Level 2

JoinedJun 29, 2008Messages18

yes  I only have to open the .mdl file than delete some points and than save geoeset. Is this right?

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

In this tutorial I did not talked about deleting yet^^

and it is always better to use the "save" or the "save as" function instead of the "save geoset" function because a model always has more than 1 geoset and when you choose "save geoset" you will delete all geosets you don´t see

Example: A model has 6 geosets and you choose geoset 3. When you select save geoset now you will delete geoset No 1,2,4,5 and 6 and only geoset 3 will be there.

#### Petepet

#### Petepet

 Level 2

JoinedJun 29, 2008Messages18

thats mystery^^ i can see your uploaded knight-head

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

yes^^

#### Petepet

#### Petepet

 Level 2

JoinedJun 29, 2008Messages18

is it possible that i save the mdl model wrong? or is this a wrong version?

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

explain it please??^^

#### roguo

#### roguo

 Level 6

JoinedJul 13, 2007Messages173

Resources: 1

#### Resources
Icons 1

i cant open any of the programs from oinkerwinkler. when i try to run them it says that i miss a file called COMDLG32.OCX


can anyone help me??

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

yes here you go:

Running the programs

#### roguo

#### roguo

 Level 6

JoinedJul 13, 2007Messages173

Resources: 1

#### Resources
Icons 1

i tryed that it didnt work it still says its missin that file after i downloaded it

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

do you have windows vista??

#### roguo

#### roguo

 Level 6

JoinedJul 13, 2007Messages173

Resources: 1

#### Resources
Icons 1

yes doesnt it work on that??

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

it does work with vista, but you will have to follow this tutorial first^^:

https://www.hiveworkshop.com/forums/f283/get-older-warcraft-3-tools-work-100486/

#### roguo

#### roguo

 Level 6

JoinedJul 13, 2007Messages173

Resources: 1

#### Resources
Icons 1

ok i try that thank you


edit: ok i got the oinker tools to work but when i open the knight portrait in vertex modifer it says run-time error 6 overflow and then it just close what do i do wrong?


 Last edited: Jan 17, 2009

#### Chief-Oblivion

#### Chief-Oblivion

 Level 11

JoinedMar 20, 2008Messages485

Resources: 11

#### Resources
Icons 9Spells 1Tutorials 1

I downloaded the model editor but when i tried to open it, it said unable to open file because its missing.. some file.. bunch of letters and numbers.. .dll =O

#### D a w n

#### D a w n

 Level 26

JoinedJun 15, 2006Messages2,641

Resources: 1

#### Resources
Tutorials 1

do you have roc and tft??

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Everyone seems to be missing this dll thing. I don't know where I got it (maybe from the earlier version of Magos, 1.05).


But here, I think this should work. (?) Just take the dll-thingy out and put it in the ModelEditor folder, right next to the Executable file (the thing you double-click on to open Magos itself).

#### Attachments

The dll Thing.zip

 1.6 MB

 · Views: 163



#### brayden430

#### brayden430

 Level 3

JoinedMay 12, 2008Messages64

Uh i serched for nearly half a hour now i cant find material editor i can find manager. please help i also coudlent get the thing from the website it glitches so i dl from hive

 You must log in or register to reply here.


### Similar threads

 Geoset Importing and Exporting Guide


-Peper-

Oct 30, 2010

3D Modeling Tutorials

Replies31Views38K

Apr 14, 2012
-Peper-

 Geomerging


eubz

Apr 21, 2012

3D Modeling Tutorials

Replies19Views32K

Jan 22, 2019
Amigoltu

 How to Swap Reforged Model Parts


Retera

Oct 30, 2021

3D Modeling Tutorials

Replies20Views9K

Mar 12, 2023
FatCatTrap

 Creating Ghosts


Daelin

Jul 14, 2005

3D Modeling Tutorials

Replies10Views27K

Aug 14, 2023
DREADMAN

 Creating Doodads in Gmax (simple grass)


eubz

Aug 14, 2011

3D Modeling Tutorials

Replies8Views12K

Mar 7, 2012
stonneash

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