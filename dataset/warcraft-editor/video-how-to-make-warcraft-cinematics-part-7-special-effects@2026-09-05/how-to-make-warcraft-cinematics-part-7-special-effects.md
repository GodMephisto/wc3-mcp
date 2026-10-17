---
type: vendor-doc
package: video-turnro
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=q-9dL7AJnaE
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 657
timestamped: yes
---

# How to make Warcraft Cinematics Part 7 - Special Effects

Source: <https://www.youtube.com/watch?v=q-9dL7AJnaE>

Uploader: Turnro | Duration: 10m57s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

The seventh video of Turnro's Warcraft 3 cinematic tutorial series.
This video explains how to go about adding special effect animations into cinematics.

## Transcript

**[0:00](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=0s)** hello everyone it's Turnbow here bringing you the next video in my series of how to make cinematics using the warcraft 3 world a little in the last video we concentrated on how incorporating animations in regards to units destructible Zoar doodads can greatly enhance the quality of our cutscenes and in this video we'll be going over how we can use spell animations or special effects in Warcraft 3 and apply them to our

**[0:29](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=29s)** cinematics so on a basis level there's only a couple of things you need to understand when incorporating special effects into our cinematics through the use of triggers so first of all if we go into actions go to s for special effects pretty much all the special effect actions you want to use are contained right here when creating a special effect you have the choice of either spawning the special effect on a

**[0:58](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=58s)** specific location of the map or specifically within a region or you also have the option of creating a special effect on a specific unit in which case you can specify what part of the unit you want to have the special effect appear usually its overhead or we can have it at the origin of the units etc if we actually click on the special effects we are able to toggle and choose what model we want to display so what

**[1:34](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=94s)** we're going to do is we're going to move this trigger window over slightly and then we go back into the special effect model options and the actual specific special effect will play on the left-hand side now you're able to choose from a variety of different models per se that exist already in the game or alternatively if you would like to make use of your own special effect model you will be able to do so within the

**[2:06](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=126s)** imported drop-down providing you have already pre-loaded and loaded or imported the model within the import manager first there is quite a variety of different special effects we can choose from and will be up to you to explore the different models that is currently in the game for you to make use of yourself but essentially speaking the main sort of special effect models that you want to use will depend on the

**[2:41](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=161s)** following so there is couple sections called abilities which are specifically for model sets born as a result of casting spells then there is also buffs which refers to After Effects of casting spells or abilities a few other categorizations that you may want to make use of includes the unit special different types of animations and the spawned effect types of animations so for this specific tutorial what we're

**[3:21](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=201s)** going to concentrate on is incorporate special effects within our already made cutscenes that we concentrated on within the previous video so for the first cutscene where similar attacks the villager we want to make it so that a normal special effect spawns as a result of the villager dying so what we're going to do within our actual similar trigger is we're going to scroll down to the area where the villager actually

**[3:46](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=226s)** dies and then we are going to play the special effect animation so we go to create special effect attached to the origin of the villager and then from here we are free to use whatever special effect model we want but for this specific example we're going to make use of the anim dead special effect animation now another thing you'll need to keep in mind is that when you're creating special effects through

**[4:20](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=260s)** triggers it is important that you have them destroyed after you have done using them and the reason why this is the case is because it creates memory lakes which is a major cause of lag and even the game crashing so as soon as we have created the special effect we want to immediately destroy it after it's done playing so now we go back into s and we go to destroy special effect so what will happen is that the special effect

**[4:53](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=293s)** will play and then the special effect will be queued so that after the anime dead animation has played the special effect will automatically be destroyed so let's save that and test it within the actual game itself so we'll report a samurai to go into the rocky region in order to initiate the cutscene and the animation place is inspected now let us apply a different special effect to the other cutscene that we have created

**[5:28](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=328s)** consisting of cinereous summoning or reviving a bunch of trees from high ground so what we want to do in this sort of scenario is we want to create a tranquility special effect so within the area where cinereous begins to cast his tranquility spell per se we will create this special effect here so what we're going to do is we're gonna go create special effect on the origin of fake cinereous and we are going to go when

**[6:01](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=361s)** choosing the model will find the tranquility special effect per se within the buffs area and then we click on the tranquility effect and then as before we want to make sure that the special effect is destroyed once it's finished playing so now we will save our map and then test what it looks like so we'll order a similar to go into the lush green area [Music] okay so we added in the special effect into the cinereous cutscene but the

**[6:45](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=405s)** special effect didn't actually display what went wrong well it is to do with the fact that we ordered the special effect to destroy straightaway when the special effect came into effect with some special effect models if they are a once-off animation such as animate debt so it is okay to immediately destroy the special effect after you have created it because it will finish playing that special effects animation before

**[7:17](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=437s)** actually destroying it but with some type of special effect models if you destroy it straight away it will completely remove it from the moment you specify it and this is common for such animation such as channeling animations or effects which specific types of special effect models are affected by this you will have to test for yourself but in this particular scenario we want the special effect to play but we also

**[7:51](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=471s)** still want to destroy it so an possible solution to this problem is to delay destroying a special effect until much later on so in here for example after quite a few seconds within leading into the cutscene we will then destroy the special effect and this work but it does create another problem let's say for example because there is the three-second delay between when the special effect is created and when it's

**[8:24](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=504s)** destroyed how are we certain that if we call this destroy last created special effect whether this will actually refer to the special effect that we want an example could be within this block of actions we create another special effect action which creates an entirely separate special effect so we have two special effects occurring within the same time frame and when we refer to destroy last created special

**[8:54](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=534s)** effect it will remove this one but not this one and the same also occurs for special effects that are created in game as well whenever a spell is cast for example so it doesn't necessarily have to be trigger related so in order to get around this problem we need a way to specify this will refer to the special effect once it's been created and we can do that with the use of variables so we're going to create a new variable

**[9:25](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=565s)** we're going to call it tranquility the variable type we will assign it as a special effect and then within our actions if we go to set variable we will go set tranquility to last created special effect and now when we go to destroy last special effect we can instead refer to the tranquility variable that we have specified also on a side notes when you play a particular spell animation or special effect the

**[10:00](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=600s)** sound associated with that animation will also play so this play tranquility is no longer needed for our purposes so now with that slight improvement we will now test the map to see how it will go and there you go now the tranquility animation plays and we also successfully destroyed this the tranquility special effect to prevent memory leaks and that pretty much concludes this videos topic in regards to how to apply special

**[10:39](https://www.youtube.com/watch?v=q-9dL7AJnaE&t=639s)** effects within cinematics I hope you found this tutorial video useful if you did feel free to like the video and subscribe to my youtube channel for future or craft 3 tutorial videos but until the next video I'll catch you guys next time bye now
