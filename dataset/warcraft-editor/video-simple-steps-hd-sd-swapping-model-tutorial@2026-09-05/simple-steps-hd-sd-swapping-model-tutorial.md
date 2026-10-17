---
type: vendor-doc
package: video-retera
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=l8fX51xj2Ko
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 919
chapters: 3
timestamped: yes
---

# Simple Steps HD/SD Swapping Model Tutorial

Source: <https://www.youtube.com/watch?v=l8fX51xj2Ko>

Uploader: Retera | Duration: 15m19s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

1.) Import HD version to World Editor's Asset Manager
2.) Import SD version to World Editor's Asset Manager
3.) Change HD version in Asset Manager to have "Mode" column dropdown changed to "HD"
4.) Rename HD version and SD version to the same file path
5.) Never use the HD version of the World Editor
6.) Success

## Chapters

- [0:00](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=0s) Introduction
- [1:50](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=110s) Importing Models
- [6:20](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=380s) Testing

## Transcript

**[0:00](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=0s)** in this video I'm gonna give an example of a pretty simple concept in the Warcraft 3 World Editor after reforged and how to achieve it a very basic concept that I think a lot of people probably want to achieve when they're making a custom map is to use a different model a different custom model for a reforged asset or for a classic asset based on the user's mode setting whether the user is chosen to be in the

**[0:23](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=23s)** classic mode or the reforged mode so to give an example of that here and something the map I already made let's start in the classic mode and I have this map I was testing with just called peasant engine fixed and what I did is I made a model and so it's custom model file that includes a bit essentially it was about like a peasant riding on a siege engine that has no gun and so this peasant riding on a siege engine you can

**[0:49](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=49s)** see here I have played the game and I set my game graphic settings to the SD or classic mode and you can see that here I have a peasant engine that is a peasant riding on a siege any now the I think the goal that a lot of people want to achieve is to have it that if one of their players who tries to play their custom map goes in the options and chooses the reforged option you know it backs out of those settings and saves

**[1:13](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=73s)** them then when that player goes back to play that exact same map peasant engine fixed here and click start that it'll play the exact same game and and have the exact same characters you know in cross-compatible between you know maybe some players or in the SD mode and some are in the HD mode but it's going to have the HD version of that same custom asset in this case you know the HD custom peasant riding on a siege engine

**[1:40](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=100s)** so now I want to talk a little bit about how to achieve this now you can see what the goal is and we can see that it is possible there the developers the World Editor wanted this to be possible they wanted it to be the case that it would be hopefully very straightforward to do what we just saw so in order to do that you can see we have here what used to be called the import manager and they've changed the name to the asset manager

**[2:03](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=123s)** which essentially has this capability to do that so based on how it was designed if I just go to import file here I'll say I'm just gonna make a map and maybe I'll import I have this like peasant engine underscore HD and peasant dungeon underscore SD and you can see these are the two different versions of the model that I've created both but you might notice the first thing you'll see is when I import both of them the mode

**[2:27](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=147s)** column here and the asset manager says SD for both that's not because it's found anything in the file that is or isn't HD that's simply because every single file when you import it defaults to the SD mode when we change it to the HD mode which you can do by just clicking it that all that actually does under the hood is when it is saving the models into your map it puts them in a special subdirectory for HD mode so

**[2:53](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=173s)** because of that if I put both of the two models on the SD mode which is that default when you first import them you can see I can actually go into the unit editor here and copy the siege engine and essentially make a custom siege engine called peasant engine that's going to use this custom character mode but you can see when I've imported both of them like that in my list of imported models that I can put on the character

**[3:14](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=194s)** there's two options right the SD and HD ultimately this isn't what you want but we could sort of demo how that functions here just to make it really clear right I can make a unit that's like the peasant engine a HD and make a separate unit you know a separate character who's a little character named is like peasant engine SD right and if I do that and I go in here I and you can definitely do that you know the model supports that the

**[3:36](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=216s)** engine supports both kinds of models you know I can have those two different characters and I can put them in the game world side-by-side and you can see you know most definitely these are two different three model files and I can I can save them both into my map you know we could go ahead and save this map on the hard drive here somewhere let's go ahead and give it a name we'll put it in this test folder and call it

**[3:58](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=238s)** HD underscore SD underscore peasant engine that's great another good thing to do to help you still find the map later is gonna be to name it the same thing in the map description here so HD underscore SD underscore peasant engine this is a good thing to do so as you can see from that now I have these two different characters which is again you know it's not what we want right like it works it definitely is something that

**[4:20](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=260s)** you can do you can import both of these three model files into the world editor and you can load both of them here even though I just click test so we can see as an example I'll launch the game and you'll see I have both of those characters and then both those they're totally legal files in the game engine and they can be sitting next to each other like that but you know for in terms of our goals this isn't the goal

**[4:42](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=282s)** the goal is to have one character who can sort of you know dynamically swap between either one of these based on on whatever particular setting you're in so the the intention of the game developers on how to do this was really pretty straightforward essentially because do we have this mode drop-down what you have to do is a user is go to the mode drop-down and set it to HD and when you do that it adds this subdirectory on the

**[5:08](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=308s)** HD version model so we want to make sure the models name the same you at that point you get rid of the difference in the file name because it's already when it actually saves internally it's already in a different folder and it's the same thing with the sd1 right you can delete that file name off of both of them and when one of them is SD and the other is HD like this it allows you to give them the same file name again

**[5:27](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=327s)** because it actually knows that this one internally is in a subdirectory called HD so because of that internal change I can go in here and I can you know where I used to have two different units I can get rid of them and just have one single unit and you'll see the world editor understands that if I go to the list of imported models it just shows one single model and I can select that and push OK and then at this point because of a

**[5:48](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=348s)** malfunction in the world editor you'll notice that even though I'm in the S there in the HD graphics environment it loads the SD version the model for display that's just kind of an indicator that some of the stuff in the actual world editor is is malfunctioning but in general if you save the map at that point and you try to play it you'll probably see that it'll start doing what you want so let's test that right now

**[6:09](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=369s)** I'm going to launch the game and we'll make sure to test our map without saving it both in the SD and the HD mode so you can see an example of this so I'll just go into single-player custom game and here we go so I saved this map in the folder called test so we'll know when we go in there we have the HD underscore SD underscore peasant engine and this is our new map so I'll go ahead and start playing it I'm pretty

**[6:36](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=396s)** sure we're still in the HD mode from earlier and in our testing so we should then see as we would expect we have our map and we have our little peasant engine right here so the next trick with our with our peasant engine is to be able to leave the game and without changing the save map go into the options toggle to the classic graphics mode save it and this this is a real test right when I do that can I launch

**[7:04](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=424s)** the map again and see the SD version of that same character model and that is of course the intention with what we just did in the world editor now you'll see it actually worked right and it loads so we've essentially achieved our goal we have a character model that can load in the SD or the HD mode but this is where things get a little bit interesting after the first time that I save this map especially you'll see this if I

**[7:28](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=448s)** close the map and then I open the map again sometimes it'll have other behaviors but this is a really clear example of it you'll notice when I close the map and open it again the world editor is now showing our character as having an HD model which is a little bit different right when I saved it showed an SD model again sort of indicative of this world editor malfunction because of the World Editor malfunction even though

**[7:49](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=469s)** we just played this map and it was very clear to us that it worked as you would intend the act of saving the map again from the world editor after opening it will actually corrupt and destroy the contents so if you see now if I play the game and this is just because of a malfunction in the world editor that very same map that we just played with nothing different other than just opening it and saving it again is gonna

**[8:11](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=491s)** have this issue where it no longer functions in the SD mode so let's let's go ahead and demonstrate that right here if I just go to this test folder and we go to HD SD peasant engine remembering of course to start off that we are on the the classic graphics mode so we could test that one first go ahead and start the game and as it launches you can begin to see here that the the character is our character is

**[8:38](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=518s)** invisible unfortunately right now if you wonder why are they invisible you can quit out of this map and you can go into the options and you can toggle to reforge and ask yourself is the entire model gone you know what is it that the World Editor did due to its malfunction when I just opened and save the map so if I launch the game again having just done that and having just changed now to the reforged

**[9:00](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=540s)** or HD graphics mode what you'll see is it works still in this mode so what happens is essentially that due to this malfunction in the world editor the act of opening and saving one of these maps that has a model capable of swapping like this damages and destroys the data inside of the map so in order to avoid that what you can do is always use the eight the SD version of the World Editor now in order to access that version of

**[9:26](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=566s)** the world editor you have to go into file preferences down here and in this asset mode you toggle the asset mode to SD and it'll tell you that won't take effect until you restart the editor so once you do that you close the editor out again you launch it and once you've done that you have a different version of the world editor that that looks you know very different it's in if it's in that SD mode and most importantly this

**[9:49](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=589s)** version of the world editor is able to maintain the distinction between a SD and HD content so for example we could go ahead and save ourselves a map here that is just exactly the same as the one we made before except we'll call it HD underscore SD underscore peasant's engine underscore fixed because we're going to essentially show that you can fix this problem and still be able to continue working on your app so we're

**[10:11](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=611s)** gonna call this HD underscore SD underscore peasant engine fixed so this is the fixed one now it's blank we haven't done the work in it yet but essentially when we go in here and we import our character model and we'll just go to that same folder will import peasant engine HD peasant engine SD will set the HD one to the HD mode and will delete the difference in naming so they have the very same name and should

**[10:37](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=637s)** behave the exact same likewise we'll do as we did before we the unit and we give the unit of this new model you'll see again there's only one option for the model because that's the only thing that we imported once I do that I should be able to place this custom character in the game world and you'll see right there shows up because the World Editor is in the SD mode there are not any particular malfunctions yet

**[11:00](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=660s)** so I can just essentially save that map and then we can start trying to play it now again to make it very clear whether I test the map with my game and the HD or SD mode I haven't been using the test map function right I've been launching the game deliberately through the single-player view so that it's really clear before we start which mode we're testing with because it makes a big difference now I'm going to get back in and we

**[11:24](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=684s)** should see our map but one thing you might notice I think I may have alright here we go cool so here's the map HD SD peasant engine fixed and what I'm going to do now is double check my settings so you can see my settings are on the reforged mode last time we tested we did classic than before so let's start with the classic mode so now I'm in the classic mode we're gonna test the map it was saved with the SD version of the editor

**[11:48](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=708s)** so we would expect that should probably work for sure that the editor version the game version and it matched tends to work so here you can see that I've saved the map and it's working again in that SD version so assuming that the world editor works in this case how it did in the HD version you would totally expect that when we go and play the map what we're going to see is again everything's working correctly and that was the same

**[12:10](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=730s)** from the HD world editor for the first time that we saved it so here I'm starting off now in the HD graphics mode having saved in the SD World Editor after doing that what you find is that the custom model does work we are able to have our character with our peasant riding on it's teaching so this is where it gets interesting because we can clearly see that in both modes in the SD and the HD world editor saving the map the first

**[12:37](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=757s)** time works but what we observed in the HD world editor is that if I just close that new map that I've made and I open it again and then we see our character and I save the map when the World Editor is in the HD mode a world editor malfunction will then just Troi the contents of the map and make it so that it will never work in the asset emote because my world editor is in the SD mode it is currently the case that

**[13:02](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=782s)** even though I just opened the map and saved it I should be able to play the game and not have the issue so we could check that again to make sure we get our correct map we want to go in the test folder where I saved it HD SD peasant engine fixed so as we see here if I go to the classic mode and save and start the game we should see that it will still work in game and there you go character is still present now something

**[13:50](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=830s)** else is I was kind of clicking fast when I did save in the world editor let's go saving the world editor again just to kind of you know double check that this is a pathway to where you would be able to maintain your map and keep working on it so I open it again I'm saving again just for good measure and we're just kind of triple checking that even after I do that is this still gonna work anyway so here I am again let's test our map

**[14:13](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=853s)** again and click start game and again the game engine is still in the SD mode so we're still gonna see the SD character as you can see our character model is still there still present and so now I'm going to do one last test I'm going to put the game engine in the HD mode and you can see we've saved this map several times in the SD World Editor and unlike the HD World Editor the map has not been damaged to our knowledge and so we

**[14:38](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=878s)** double check here to make sure that the SD World Editor did not damage the HD version of the model either but as we would expect it hasn't damaged it so this essentially shows you that it is possible to create this kind of content in a very simple way as intended with the drop downs in the asset manager where you choose HD and SD for your files but what's important to keep in mind is that due to the malfunction in

**[15:02](https://www.youtube.com/watch?v=l8fX51xj2Ko&t=902s)** the HD version of the World Editor you will not be able to save your map without destroying it or damaging its contents unless you just simply don't use that version of the world editor and always use the SD version of the world editor
