---
type: vendor-doc
package: video-turnro
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=uejMc5_ZDIE
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 870
chapters: 5
timestamped: yes
---

# How to make Warcraft 3 Cinematics Part 1 - Basics

Source: <https://www.youtube.com/watch?v=uejMc5_ZDIE>

Uploader: Turnro | Duration: 14m30s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

The start of a new Warcraft 3 World Editor tutorial series about cinematics.
In this first video, we cover the basics of setting up and playing cinematics through the triggers.

## Chapters

- [0:00](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=0s) Intro
- [0:55](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=55s) Cinematic Setup
- [5:30](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=330s) Camera Setup
- [6:30](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=390s) Sky Setup
- [7:40](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=460s) Region Setup

## Transcript

**[0:00](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=0s)** hey guys it's turmeric here bringing you a new warcraft 3 World Editor tutorial series and over the next series of videos we'll be covering how to make cinematics now this will be a complete series from beginning to end on how to incorporate and improve good quality Warcraft 3 cinematics within your own Maps and in this series there'll be a series of eight different topics that we'll cover so there will be something

**[0:31](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=31s)** of benefit whether or not you are a novice or whether you are someone who knows how to make cinematics but you would like to know a few tips on how to improve them so for this first video we'll just be covering the beginning of the absolute basics the minimum you need to know if you just want to get a cinematic to work so the first thing we need is we need a plan whether or not it is in our head or on paper on what we

**[1:02](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=62s)** want to achieve for simplicity purposes we're just going to have a cutscene of summary the blade master walk from one point to another and maybe some sound will play fairly straightforward once you know how the cinematic is going to play out we can start implementing it so first we're going to head into the trigger added up area and we are just going to delete all this default actions next we're going to create a new trigger

**[1:31](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=91s)** and we're going to call it cinematic and you have two options what we'll be doing is we'll be having the cinematic play once the player orders some Errol to reach a specific area so let's say let's go into the terrain editor and we put in some rocks there we go and then we're going to create a region that allows us to identify when summer is going to enter the region so we call this cinematic region and then we go back

**[2:12](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=132s)** into the trigger editor and for the events we click on new events we go to unit enters region so when the unit enters either we select it from the drop-down list or we go select a region and just click to select the region and we also want to do a check to make sure that the you know who is entering this region is the correct unit so we can either go let's go unit comparison so if the owner of the entering units the

**[2:45](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=165s)** person who is going to be entering it is equal to the actual player itself next we're going to go to new actions but go need to go t for trigger and we're going to turn it off just so the cinematic only runs once and then we can proceed with the actual cinematic if you intend to have the cinematic play at the beginning of the map then all you need to do is just delete these and in the melee initialization trigger just go

**[3:16](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=196s)** actions Tifa trigger run cinematic so either way will work depending on if we want it to be at the beginning or the end the first thing we're going to do with this cinematic is we're going to have a fade in/fade out animation just to showcase to the player that we are actually transitioning into a cutscene so we go into actions we go to c-4 cinematic we go to fade filter and then we click on fade out over two seconds which is an

**[3:54](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=234s)** acceptable amount and you don't need to worry about this one what these three numbers mean with the percentages is they are mean red green and blue so at the moment they're at 0% meaning that when the screen fades out it will be black if we want it to be white we would change these numbers to 100% 100 or if we want them to be a specific color then we can choose red for example so we make sure that red is at a hundred

**[4:28](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=268s)** percent and the other two green and blue are at zero you can learn more about our chibi colors by looking it up on the internet but for this purpose we're just going to have the screen fade out to black and transparency refers to whether or not the fade out and is invisible we don't want it to be invisible so we leave it at zero percent we click on OK the next thing we're going to do is we're going to do a new action we're

**[4:54](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=294s)** going to go wait for two seconds just to allow the game to finish doing the fade out animation before it runs the cinematic then the next thing we're going to do once the screen has faded out the new action we're going to do is we're going to go see for cinematic we're going to go cinematic mode turn on for all players so this is where it will actually bring up the necessary bars at the top and bottom of the screen to

**[5:24](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=324s)** showcase that you an actual cinematic is playing next we're going to create a camera so we want a certain area on the map where the cinematic is going to play so we make sure we go to that area and we'll just do some basic training to just demonstrate that this is the area where we are going to showcase the cinematic we're going to go to the camera panel and where we want the camera to be positioned we make sure

**[5:55](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=355s)** that we are in the correct spot and then we just click on the create camera button and a camera will be created then we head back in to the trigger editor we go into new actions C and we go apply either selector from the drop down or go select camera and just select it by left clicking on the mouse and we want to apply it straight away so we leave it at zero seconds next we're going to set the sky now in Warcraft 3 the sky is black

**[6:28](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=388s)** which you don't notice because the game is positioned at a bird's eye view if however we change the look of the camera by just holding down control on the keyboard and right-clicking you'll notice that you can see the sky if we want to change this to be an natural sky then we go into new actions we go to eve for environment and we click on set sky and we have multiple different options to choose from so let's say we want

**[7:07](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=427s)** there to be just a generic sky with a blue sky with white clouds and you can experiment yourself to know what each of these skies will look like so for example felwood sky will be a dark green misty sky and Dalaran will be like a evening orange purple sort of sky etc the next thing we're going to do is we're going to set up the units so we just click on the scroll mouse to go back to the normal view we want to move

**[7:40](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=460s)** some row the Blademaster who was over here to over here so we're going to create a new region we go over to the region panel we create a new region we call it by double click on it Sam Moreau region one and then we create another region where he is going to move to so we'll just go ahead and do that and then we also rename it to summer region two next we're going to go into the new action going going you for unit

**[8:22](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=502s)** and we'll scroll down to where we want to issue some row to move to a location so we go click on the summary from the drop down or go select a unit from the actual map we order him to move to the center of this region and then we want him to move instantly to that region so we go back into the new event and we go unit move unit instantly facing 270 degrees or leave it as default okay so we move him into this

**[9:04](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=544s)** region here now the reason why we are ordering Samara to move to that region before moving him is so if at the beginning of the cinematic the player orders samurai to go to a specific location it means that if we do not have this then the game will move some row to that region but then he'll begin to start moving to the point where the player last ordered him to go into which is not an effect we want so we'll just

**[9:36](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=576s)** order him to move to a location forcefully and then moving him instantly there so now we can actually start making the actual cutscene and we'll just demonstrate that by just creating a comment which says start of cutscene so for this particular cutscene we're going to copy this particular command and paste it since we're going to be showing some arrow moving into a particular location which is the second region next

**[10:14](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=614s)** we are going to copy this fade out and we're going to paste it just before the cutscene and we go fade in so we want the cinematic to fade in to the screen next we're going to wait a couple of seconds just to allow similar to reach the area before we actually play the sound and then after the two seconds is up we're going to play a sound to demonstrate that samer has reached his particular point which we can do so by

**[10:49](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=649s)** first of all we go into the sound editor which is where we can make use of sounds in order to use within our triggers for this particular one I'll be going into the am unit sounds going to do that effects and I'll be playing the Walker peer sound just right click on it go users sound get out of that we go back into the trigger editor we go into new action s4 sound and we go play warlock appears and we will just wait a

**[11:26](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=686s)** certain amount of seconds we wait five seconds let's see how long that okay we'll probably wait six seconds just to allow the sound to finish play and so that is the actual cinematic will have so then we go and do a new comment which will demonstrate that this is the end of the cutscene then we're going to go back and go new action and go safe cinematic we're going to fade out again so we just fade out and then we go new action wait

**[12:06](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=726s)** two more seconds and then the next action we do is we reverse everything we just did so in terms of the environment we go into a we go set sky to none with the cinematic mode we go into sea and we go turn cinematic mode off for all players and then we also go into cameras and we want to reset the game camera so any changes we've made with the actual camera it will reset back to the normal default view for the player over 0

**[12:47](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=767s)** seconds which is immediately and then once we've done that then we can go ahead and we could go into cinematic fade back in and there we have it so here we have a basic cinematic from beginning to end on where it fades out and then it turns on cinematic mode where it sets the camera right changes the sky it moves Sam Errol to where he needs to be and then the actual cinematic of where Samara moves to a

**[13:20](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=800s)** specific region it plays a sound and then it fades out and it resets everything back to normal now we're just going to save this and we're going to test it and see how it plays so now we're loading into the map and we'll get to see how our cinematic plays so once summer reaches this rocky region then the cinematic will play we see Sammy moving across and the sound playing in the background wait six seconds and then

**[13:51](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=831s)** it fades out and then it fades back in and here we are so so far the cinematic we've made is pretty straightforward and in the next series of videos we'll be covering more in depth on how we can optimize and improve our cinematics but for now this will do I hope you guys enjoyed this tutorial video if you did feel free to like the video and subscribe to my youtube channel so you can be informed of the next series of

**[14:23](https://www.youtube.com/watch?v=uejMc5_ZDIE&t=863s)** videos that will be coming but until then I'll catch you guys next time bye now
