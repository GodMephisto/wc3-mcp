---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-swap-reforged-model-parts.336382/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Swap Reforged Model Parts
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-swap-reforged-model-parts.336382/>
#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


How to Swap Reforged Model Parts

In this tutorial, we will learn how to swap parts of a Reforged model. I was chatting with the user MayDay on Discord, and he asked me how to give the Jaina hero the legs of the Succubus demon. I was able to do this in a couple of minutes, and he seemed surprised, so I think we should encourage for this to be common knowledge.

Requirements

Retera Model Studio

Warcraft III: Reforged
Step 1: Open the Model Files

I will assume you have your Retera Model Studio setup completed from a previous tutorial.

Personally I like to disable the "Quick Browse" setting in the options menu. You can access the preferences by going to "Edit > Preferences".

Once the Quick Browse is disabled, any time that you open a model the model will remain open and continue to consume memory and resources on your PC until you close the Retera Model Studio. This is not desirable if you are going to do a lot of model browsing without editing anything. But if you are a user who already is familiar with the model database, and you want to use this program for editing files, I find it more convenient if any model file that I open remains open.

Enter Reforged models mode if you have not already. If your program did not automatically setup to use this mode, you should be able to go into "Preferences > Warcraft Data" and then find the UI option for "Reforged Graphics Mode". Simply click this button, then press OK on the Preferences window.

Open Jaina by navigating the Unit Browser on the right-hand side to find "Standard Units / Human / Campaign / Heroes / Jaina". Double click on the Jaina icon and you should see Jaina appear in the 3D preview in front of you. Because we disabled Quick Browse, this has created an editor context to modify Jaina that will remain open even if we browse to a different model file.

Now navigate to "Standard Units / Neutral Hostile / Melee / Units / Succubus". Double click on the Succubus icon and you should see the Succubus appear in the 3D preview in front of you. Because we disabled Quick Browse, again this has created a second editor context to modify the Succubus. We will use this to delete everything besides her legs. Then when we have a "legs model" like this, we will be able to put it onto Jaina.

Step 2: Remove Unused Geometry

In order to make a model of Jaina's body with the Succubus legs, we need to have a model of the Jaina body without legs and a model of the Succubus legs so that the Retera Model Studio automation can combine them for us.

Let us begin by creating the Jaina model that has no legs. To do this, assuming your 3D previewer still is showing a Succubus, click on "Window > Jaina" in the menu bar across the top of the program.
(Similar to how the Warcraft III World Editor can have multiple maps open at a time, the Retera Model Studio can have multiple models open at a time. You will probably notice on the dropdown that four models have been opened: Jaina, Jaina_Portrait, Demoness, and Demoness_Portrait. For now we can ignore the portrait model files because they are outside the scope of this tutorial.)

Now that Jaina is the active context for editing, click on the Edit tab to begin customizing the model file. In this view, you will see the model in several 2d editor views as well as a 3d editor preview.

I usually begin this kind of model edit by expanding out the Mesh section on the Outliner that we see to our left. In particular, in this tutorial we are not interested in modifying the animation data of Jaina so we will not touch the Nodes box.

Under the Mesh section, we can see a list of Jaina's 3d components. If you hover the mouse over the name of a component, that component will highlight in green and yellow in the four views. This can help to get a better sense of how the different 3d components are divided.

Also notice how some of the geometry of Jaina is hidden by default. The program hides the geometry that is named "_LOD1" or "_LOD2" or "_LOD3" because these are lower Levels of Detail of the Jaina geometry. According to a former Blizzard Employee Reforged producer, and long-time member of the Hive Workshop, these Level of Detail geometries inside of the Reforged model are completely unused. Because of this, it is safe to ignore them completely, or even delete them if you would like. For the purposes of this tutorial I will ignore them.

For this sample model, we can pretty quickly see that only the first row, titled human_jaina_rig_ingame:main_geo contains the legs of Jaina.

Uncheck all of the boxes except for the first one, so that we will use the editor to modify only the first row of geometry on Jaina. The row we will modify is the first one titled human_jaina_rig_ingame:main_geo. This geoset of data contains the legs. So, to remove them, we need only modify this geoset.

Now that the editor is set to only modify the main body of Jaina, use a left-click drag to draw a box across most of Jaina's legs. This will select them and offer us an option to delete the legs, but do not delete them just yet.

Before deleting the legs, we should make sure that we select every part of the legs that we want to remove, and nothing more. To do this, press CTRL+E on the keyboard or use "Edit > Expand Selection." This option will select more of Jaina, selecting outwards until it reaches a seam. In this case, the seam that will end the automated selection will be at her waist, allowing us to exactly select her legs -- nothing more, and nothing less.

Now press DEL on your keyboard to delete the legs, or use "Edit > Delete" from the menu. Jaina's legs should immediately disappear, leaving us with a view of her remaining upper body in the editor. If you would like, you could pause to make sure no model data was damaged and click back on the View tab to see a preview of the current model. It should look like Jaina, but with no legs.

Now that we have created a model of Jaina with no legs, we need to create a model of the Succubus legs only. First, click on "Window > Demoness" in the top menu. Because the Retera Model Studio supports having multiple editor contexts open at a time, our modified Jaina model will remain open and waiting for us when we get back. If you are worried that your computer might lose power, now would be a good time to save a backup of this Jaina model to save your progress. However, in general this is not necessary until we are finished because Jaina will remain open while we change over to work on the Succubus model.

Return again to the Edit tab and expand the Mesh section like we did on the Jaina model. This time, we can see that the legs we want are in the third geoset listed under Mesh rather than the first. Similar to Jaina, the legs are in a geoset whose name ends in "main_geo": in this case "VileTormentor_rig:main_geo". Again, uncheck the boxes other than this one geoset so that we can select only her legs.

Left-click drag a select box across the legs of the model, and then expand selection once again like we did with the Jaina model. If you are like me, you may have accidentally selected the tail, too. Do not worry about that yet.

Now that we have the legs of the Succubus selected, we need to do something different than what we did for Jaina. In this case, we want to isolate this part of the model to keep it. To do that, press CTRL+I on the keyboard or use "Edit > Invert Selection" from the program menus. Now you should have the top half of the Succubus selected.

Press DEL (or use "Edit > Delete") to delete the top half of the Succubus. However, after doing this we will still see the legs and tail of our character showing. To remove the tail, drag select a small part of it -- even just the very tip -- and use the Expand Selection feature again (CTRL+E or "Edit > Expand Selection"). Then, press DEL to delete the tail. If part of the tail remains showing (floating in the air), repeat the process of selecting it and expanding your select and deleting what remains.

Now, you should be left with the legs of the Succubus showing.

Notice: this model has many other geosets that are currently not shown. We need to remove those as well. To do this, double tap the checkbox labelled Mesh in the upper-left hand corner of the Outliner. This will hide all mesh, then show all mesh. You should see the entire Succubus re-appear (because of the unused LOD copies of the mesh).

We want to delete all of this stuff, except for the customized Legs that we prepared on the third geoset listed. To do this, click on the checkbox for the third geoset listed to toggle it to OFF so that we will not modify it. As a result, when we delete everything, the customized legs will not be deleted.

Now that we are set to edit everything we do not want, select everything. You can do this by doing a left-click drag. If you want to be slightly more efficient, you can use the CTRL+A hotkey or "Edit > Select All". It may be necessary to click once on the background in one of the 2D views to ensure that the 2D view receives the CTRL+A hotkey instead of some other part of the program consuming the keyboard input.

Once you have selected everything, delete it! It should be impossible to delete our customized leg geometry, because that was toggled to OFF so it is hidden from us and is not editable. At this point, we should see our custom leg geoset as the only item listed under the Mesh checkbox.

Step 3: Combining Models

This is where the magic happens.

Return to the Jaina model by using "Window > Jaina" from the menu bars. On this model, use the menu option "Scripts > Skin Splice into Current > Workspace".

At this point, a popup will appear from the program. Choose the Demoness in the popup as the source of mesh.

Press OK on the popup. At this point, you should see a combined model, but the scale of the geometry probably does not fit.

Uncheck the box for the Jaina top half. Then drag select the Succubus legs, which should be the only part of the model remaining visible. Once you have the legs selected, check the box for Jaina's top half once again. This way, you can preview the Jaina top half while editing the legs.

Click on the Scale button or use the hotkey R to enter the Scale mode. Your selection will be preserved, so you should see a Scale widget appear on her legs in the 2D edit planes. To change the size of the legs you can click drag on that widget, or if it is simpler you can right-click drag anywhere else in the 2D view. Using the scale tool and also the Move tool (hotkey W) you can scale and move the legs until they look like they are in generally the correct location to match where the Jaina legs that we deleted were previously.

Now that the legs have been moved to the correct location, click on the View tab to preview our character! If all has gone well, you should see Jaina with a custom set of fully animated Succubus legs!

At this point we have a mostly functional model! You can try "File > Save as" and choosing the MDX format type for the model if you would like to test the model in-game or in the Warcraft III: Reforged World Editor.

Step 4: Configure Visibility

At this stage the model is good enough that you could probably show it off to your friend, but there is a problem you will quickly notice if you attempt to use this model in-game.

If you played around with the model in the preview, or in game, you will quickly notice a dumb bug. In the "Stand 2" and "Stand Second" animations, the legs of our hero will mysteriously disappear! Why does this happen?

The problem is that the leg mesh still has its visibility information that was left from the Succubus model "Decay" animation when she fades into the ground. However, the numeric animation time information has become meaningless in the Jaina model and is causing the legs to disappear at the wrong time.

To overwrite this, we can use an old feature of Retera Model Studio which can fix our issue using a bit of a workaround. First, to use the workaround feature, make sure that you have saved the model to your hard drive.

Now, after the file has been saved, choose "Scripts > Edit/delete model components."

This feature is a legacy option that I included in my program from the early development days, but it should still work fine for our purposes. After a short load time, you should see a popup appear with lots of different options on it.

In this popup, do not touch anything. Even a single click could break your model because selecting items in the viewing box will not only select but also re-assign model information.

Instead, only click on the tab that is labelled "Visibility". To view this tab, you may wish to expand the window to be full screen, and make the left-hand selection list wider so that it is easy to read. To widen it, simply drag on the divider between the two parts of the Visibility tab.

In this menu, click on the item that says Units/Demon/Demoness/Demoness_Main_Diffuse layer (mode Transparent). This should change the title across the top of the right-hand panel to match what you have selected.

Now on this view, change the second dropdown on the right-hand side which is labelled Imported animation visibility from: so that the dropdown option will read visibility data from something else. In this case, we want to overwrite the "visibility of her legs" with the "visibility of her body". So, since we have the leg layer selected, we should choose Units/Human/Jaina/Human_JainaProudmoore_Main_Diffuse layer (mode Transparent). Click on that item and see that it fills the box.

After changing this visibility item, go ahead and immediately click on "Finish" down at the bottom of the UI without changing anything else.
Now you are done, and the model is fixed! Go ahead and try "File > Save as" and then import this model into your Warcraft III: Reforged World Editor and assign it to a unit!

Congratulations! Now you know how to swap Warcraft III: Reforged model parts between models, while keeping their animations!

#### Attachments

1635580176656.png

 158.7 KB

 · Views: 174



#### Ustanak

#### Ustanak

 Level 66

JoinedDec 29, 2014Messages2,091

Resources: 175

#### Resources
Models 167Icons 8

This is really useful! Though, I am confused about the part where you import the legs from Demoness to Jaina model. You don't need to rig the legs to the bones at all?

#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


Stefan.K said:

 You don't need to rig the legs to the bones at all?


Click to expand...

Correct. This script feature will do an animation transfer to make the legs use the animations that are already in the Jaina model. These automated decisions are based on the names of the nodes inside the model.

#### Superfrycook

#### Superfrycook

 Level 47

JoinedApr 12, 2019Messages832

Resources: 291

#### Resources
Packs 1Skins 1Maps 29Reforged HD Models 231Reforged HD Skins 18Reforged HD Icons 8Sounds 3

##
 Republic Anthology


The 0.04 version does not have the skin splice window. Why?

#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


It was added in 0.04.1 if I recall. You can get the feature if you simply download again from the official download link

#### Superfrycook

#### Superfrycook

 Level 47

JoinedApr 12, 2019Messages832

Resources: 291

#### Resources
Packs 1Skins 1Maps 29Reforged HD Models 231Reforged HD Skins 18Reforged HD Icons 8Sounds 3

##
 Republic Anthology


Retera said:

 It was added in 0.04.1 if I recall. You can get the feature if you simply download again from the official download link


Click to expand...

Are you sure there is an update?

#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


Yeah there's even a changelog on the tools page download that includes a note about 0.04.1 and how it added this feature.

###
 Retera's Model Studio


This is Retera's Model Studio v0.04 Public Beta. It is not finished! But it is something that works and is better than the Matrix Eater application, and is compatible with Patch 1.32. Retera's Model Studio is the second evolution of the Matrix Eater project. It is a standalone MDL and MDX model...

 www.hiveworkshop.com


#### vindorei

#### vindorei

 Level 57

JoinedApr 23, 2021Messages477

Resources: 163

#### Resources
Models 2Skins 1Maps 1Reforged HD Models 150Ported Models 7Reforged HD Icons 2

Wait what? I’ve been selecting legs, copy and paste and reassign bones for so long! Such a great function, too pity I found it so late.

#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


vindorei said:

 Such a great function, too pity I found it so late.


Click to expand...

It was only added about 2 months ago or something. So, in case it makes you feel better, for a long time this option wasn't offered in the UI of the program

#### vindorei

#### vindorei

 Level 57

JoinedApr 23, 2021Messages477

Resources: 163

#### Resources
Models 2Skins 1Maps 1Reforged HD Models 150Ported Models 7Reforged HD Icons 2

Retera said:

 It was only added about 2 months ago or something. So, in case it makes you feel better, for a long time this option wasn't offered in the UI of the program


Click to expand...

Always feel good to see any update! Thank you so much for RMS.

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

Very useful, partner. Great contribution Retera.

In the future, we will definitely have HD models with more varied and differential customizations in our galleries.

To this day, true variety and creativity only exists among classic models, our HD galleries are only full of models with changes to standard parts and in their simplest form, ideally, the community would start making use of new polygons, extracted from other platforms, making these external meshes totally new proposals.

To the community, we only lack good tutorials like this one for manipulating reforged textures, their main details, different ways to save them in the most varied programs (Gimp/PSP...), particularities of each of their properties and application of functionality HD for new Geosets (of 3D externals, within the R.M.S), and etc..


 Last edited: Dec 27, 2021

#### SevLlanto

#### SevLlanto

 Level 14

JoinedAug 19, 2020Messages70

Resources: 10

#### Resources
Reforged HD Models 7Reforged HD Skins 3

It is a great option. If it had only been around a year ago it would have saved me a lot of headaches haha. I will experience it now. Thank you very much for the detailed tutorial, it is very useful especially for those of us who are of another language.

#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


SevLlanto said:

 If it had only been around a year ago it would have saved me a lot of headaches haha.


Click to expand...

Sorry. I knew how to do this for a long time.

Reforged came out in 2020. But here is a 2019 video of Reforged model part swapping: (Grunt head + sheep body + nether dragon wing animations)

Here is a model that was from summer 2020 that uses the specific technique from this tutorial:

###
 Thrall Riding (Black) Spiritwolf


I wrote a one-off computer program script to generate this model a while ago while discussing it with some guy on Discord. Essentially, this model recycles the mesh of the Spirit Wolf but with the animationset from Thrall's wolf and Thrall riding on it. In particular, the "matrices" tab on the...

 www.hiveworkshop.com


So this was around a year ago, but I did not think to make a tutorial -- nor a UI button on the RMS tool.

#### Heisenberg

#### Heisenberg

 Level 5

JoinedMay 7, 2022Messages59

that was very useful

thanks for this Tutorial

#### Chonky Dwarf

#### Chonky Dwarf

 Level 6

JoinedJan 19, 2023Messages41

Hey, first of I want to thank you for this amazing tool. I have a problem that probably has a very simple solution but I just started using the program and can't figure it out. I tried to transfer knight's head onto bandits body following your tutorial and this happened. I suspect it has something to do with animations but I have no idea. Can you maybe help me out?

#### warbucket

#### warbucket

 Level 5

JoinedDec 29, 2008Messages61

This is perfect. I'm making an RPG where the heroes are all paladins and I wanted to be able to swap out different weapon sets

#### DX789

#### DX789

 Level 1

JoinedFeb 19, 2023Messages3

How to change the death sound effect of a unit with RMS?

#### queensnix

#### queensnix

 Level 2

JoinedMar 2, 2023Messages2

Hello Retera, thank you for this wonderful tutorial! I just got into the Warcraft 3 editor yesterday and I'm currently working on a player model.

I combined different meshes (Arthas base) and did some mesh editing to make it all look good. So far so good, as far as your tutorial goes, it went amazing. My only problem is that in game/war3 editor my character is stuck in t-pose between animations and also doesn't walk correctly. (all works correctly in your model editor)

I assume this is in relation to that part of the tutorial with the "Visibility"? I did try that, although the UI is all wonky and not scalable (bug?), but it just gave me the green box when using the version that used that step.

#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


queensnix said:

 I assume this is in relation to that part of the tutorial with the "Visibility"?


Click to expand...

For anyone else using the tutorial who gets this far, I replied to queensnix on Discord and helped solve the problem. The model had some empty animation sequences that were causing it to jump back to T-pose. If you need to delete animations from your model, you can use the attached GIF.

#### queensnix

#### queensnix

 Level 2

JoinedMar 2, 2023Messages2

Retera said:

 For anyone else using the tutorial who gets this far, I replied to queensnix on Discord and helped solve the problem. The model had some empty animation sequences that were causing it to jump back to T-pose. If you need to delete animations from your model, you can use the attached GIF.View attachment 427175

Click to expand...

Addition to this for everyone technologically illiterate (like me lol) who couldn't figure out which animation it could be, or how to check: Just go into the view tab of the model and click trough the list. Write down and delete everything that t-poses.

#### FatCatTrap

#### FatCatTrap

 Level 7

JoinedMar 4, 2023Messages15

Resources: 12

#### Resources
Templates 1Reforged HD Models 3Reforged HD Skins 8

Hey Retera, I'm having an issue with a couple heroes I'm trying to create where the in game certain model components seem to flash, not disappearing in visibility its like the lighting flickers on them. In the map editor and your model studios animation previews there is no issues.

Would you happen to know what this issue could be? They are both based on beastmaster animations.

I've attached a video as screenshots don't capture the issue.

Cheers mate.

#### Attachments

2023-03-13 08-12-39.wmv

 18.6 MB

 · Views: 82



 You must log in or register to reply here.


### Similar threads

 How to edit models- a complete guide


Horn

Oct 4, 2004

3D Modeling Tutorials

Replies5Views49K

Dec 1, 2013
animation31

 In-depth Animation Tutorial [3ds max]


BlinkBoy

Apr 11, 2009

3D Modeling Tutorials
23

Replies123Views153K

Feb 8, 2017
Kazeon

 Milkshape tutorial - A simple way to unwrap your models.


Mulgrim

Jan 23, 2010

3D Modeling Tutorials

Replies4Views14K

Jun 16, 2010
makai

 Create Models only with free tools


D a w n

Nov 16, 2008

3D Modeling Tutorials
234

Replies174Views87K

Jun 10, 2014
clockwork2

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