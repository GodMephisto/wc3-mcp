---
type: vendor-doc
package: video-turnro
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=6Eg3qLISI_E
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 1330
chapters: 9
timestamped: yes
---

# Warcraft 3 World Editor Tutorial - Quests

Source: <https://www.youtube.com/watch?v=6Eg3qLISI_E>

Uploader: Turnro | Duration: 22m10s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

A simple tutorial on how to create your own quests for Warcraft 3.

Quest Message Codes:
|cffffcc00MAIN QUEST|r (Sample)
ffcc00 - Gold Color
808080 - Gray Color

## Chapters

- [0:00](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=0s) Intro
- [0:15](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=15s) Create a Quest
- [2:47](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=167s) Assign Quest Type
- [3:33](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=213s) New Actions
- [4:55](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=295s) Mark Quest as Discovered
- [5:15](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=315s) Quest Requirements
- [6:42](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=402s) Quest Messages
- [13:57](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=837s) Quest Update
- [16:29](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=989s) Completed Quest

## Transcript

**[0:00](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=0s)** hey guys it's turn row here and today I'll be covering a short video on how to make your own quests in Warcraft 3 now the very first thing we're going to do once you've got your map open is you're going to go into triggers and you're going to create a new trigger called quests undiscovered so the very first thing we're going to be doing is create the quests but have them as undiscovered to the player just so that when they are

**[0:35](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=35s)** get into the game and when they're playing the map they can click on the quests log button and they can see a series of quests that they have lined up for the rest of the mission now you don't necessarily have to do this but it does give a good sense of progression for the player and denied the eye on how many quests they're going to get in the mission so we're going to run trigger and run quests undiscovered now the

**[1:05](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=65s)** first action we want to do is you want to click on queue for quest and you want to go to create quest we want to create a required undiscovered or optional undiscovered creating just a required or optional we'll make it so that the quest will appear automatically to the player when it click on the quest log you want to give it a title we'll call it the quest a description so when the player clicks on the quest log tab and they

**[1:44](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=104s)** view the quest this is the description that will appear so you want to say this is a description just to see what it looks like and then the final part is where you can select an icon to be associated with the quest now you can use any item you want in the entire game whether it be a unit or ability whatever you want if you want to make use of your own custom icons then you can do so by going back into this

**[2:25](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=145s)** pop-up and clicking on imported now for this particular map I haven't included any custom icons but if you have included them in the import manager they will appear here for you to select otherwise you just make use of an icon that exists in the game and we call it next that we want to do is we want to assign the quest as a variable just so if we want to refer to this quest later on when the player actually accomplishes

**[3:02](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=182s)** objectives for the quest which you will inevitably do we want to be able to refer to this actual quest especially if there's multiple quests in the map as well so we're going to give it a name of quest 1 we are going to assign the type as a quest and we click OK next step we go into new actions we go set variable set quest one to last created quest and essentially if you have multiple quests in your mission

**[3:38](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=218s)** then you would repeat this step or these steps for all of the requests that will appear in the map so we have triggered this to run at the beginning of the map so it will show up the instance that the player enters the map the next thing we want to do is we want to mark these quests as discovered or mark a quest as discovered and show it to the player so the next thing I'm going to do is go and kind of create a new trigger we're going

**[4:10](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=250s)** to call it Qwest discovered and in this Mele initialization trigger we are going to for example have the game wait a certain amount of time now what campaign Quest delay is is it's just a placeholder for three or four seconds just to save us from having to type in three or four seconds it's entirely up to you if you want to use it or not I make use of it within my own campaigns and then we're going to run a new action go to trigger

**[4:49](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=289s)** run quest discovered now in order to make mark a quest as discovered you will have to go into new action quest and then mark quest as discovered so we select quest 1 which is the variable with the signs of the quest now there is a couple of other things women you do in addition to this the first one being is that a quest usually has objectives which is something like go here or destroy this enemy base so once your

**[5:31](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=331s)** quest has been discovered you will need to create these quest requirements and you can do so by going into questing actions and we go create quest requirements we create it for quest 1 and then we say for example go to point a and we can create another one which I will do so by clicking on ctrl C and then ctrl V for copy and paste and then I'll go go to point B now you will need to assign variables to these Crest

**[6:12](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=372s)** requirements as well for the same reason why we assign variables to the actual main quest so we go quest requirement 1 and we give it a type of a quest requirements go copy-and-paste and we'll name this two number two so we go set variable set quest requirement one two last created quests and we do so for the second cult quest requirement so in regards to having a quest discovered and having their quest requirements show up

**[6:49](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=409s)** this is all you need to do but there is one important part that is missing and that is actually alerting the player that the quest has been discovered so one other thing we need to do in addition to this is we need to display a message on the screen alerting the player that the quest is available and we do this by going into actions quest and making use of this quest message action so there's only two things you

**[7:26](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=446s)** need to worry about with this particular message the first one is you click on the quest update link and you will notice that you have a list of all these different options we will obviously be making use of the quest discovered for this particular trigger but you also notice that you have the options of quest update completed even hints warnings new units acquired what this all means is that when the quest message

**[7:54](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=474s)** displays to the player it will play a spur ticular sound depending on the type you've selected so if we choose quest discovered then when the message displays it will play the sounds that is consistently used in the Warcraft 3 campaigns of the sounds that is used when a quest is discovered or if we collect it on us different one like hint it will play the hint sound that is used consistently through Warcraft 3

**[8:26](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=506s)** campaigns when a hint is displayed on the screen so that's what this particular type means and then we get to type in the actual message now we can type in the message for example we go main quest to outline that it is a main quest that is available to the player next we type in the quest which is the name of the quest and then we type in their requirements which we go go to point a and go to point B now

**[9:06](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=546s)** there is no restricted way on writing this message but the way I'm showing you is pretty much the way that you will consistently see messages in regards to quests displayed within the original Warcraft through campaigns and in my own campaigns as well where you have the title which is what the message is about you have the name of the quest and then you have each of the quest requirements in order that they appear now one thing

**[9:43](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=583s)** we can do in addition to this is we can color coordinate this main quest text in order to distinguish it from the rest of the message which is also something that is consistently used in the original Warcraft 3 campaigns and how you do this is by making use of particular characters which is pretty much this text here now what this text does is it is a placeholder for text you want to display in a particular color so if we

**[10:23](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=623s)** copy over this it will mean that this main quest text will now display in a different color and you can specify what this color is through these six digits and this color is for a goldish sort of color but if you want to make use of your own when color-coordinating text then all you need to do is go into google for example and you type in something like RGB hex or blue for example now for those of you who don't know what an RGB

**[11:12](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=672s)** color is it's essentially a way of identifying a color in the world of computers so there's different forms of identifying a code or a color one way is by using hex hex is essentially a six digit character representing a color so we can click on the first link that shows up for example and you can see that we can make use of blue by using these six digits if we want to use several colors as well we can type in at

**[11:51](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=711s)** the top we can type in green press ENTER and we can use these six characters in order to identify green for example in which we can do so by copying into the text area so we delete these six digits that are just before our main text where you go backspace six times and you click on paste and now we have changed the color from a gold color to a green but of course we want to keep the colors consistent with how they are in Warcraft

**[12:32](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=752s)** 3 so we'll just undo that now one very important note to be aware of is that you will notice that there are a bunch of strange characters at the beginning and at the end of the title that must be there in order for the color coordinating to work it's very easy to accidentally delete these characters or to input characters between these code that will prevent it from working so if you test this in Warcraft 3 and there is

**[13:12](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=792s)** a problem with how this particular line is displaying it means that a mistake was made when either pasting the code in or editing it and you have to correct it so we've pretty much created our own message we click on OK and so here we have a very basic usage of we have a quest discovered we create the quest requirements for the player so that they will appear within the quest tab and then we display a message to the

**[13:48](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=828s)** player alerting them that the quest has been discovered and detailing them the name of the quest along with its requirements so having a quest discovered is all well and good but what if we want to for example have the quest update well in that case what we need to do is we'll need to create a new trigger and call a quest update and we are assuming for this particular trigger that the player reaches this point so we

**[14:24](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=864s)** want to display and mark this request requirement as completed which we can do so by going into the quest update trigger we go to new action we go quest and we go mark quest requirement as completed so we mark quest requirement one has finished and then we simply display the message to the player that the quest requirement has been completed so again we go into the quest message we go quest update since a different sound

**[15:05](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=905s)** will play and then we go back into this message here we can leave this line as it is we leave the quest actual name as it is and for this we want to color coordinator gray to represent it to the player that it has actually been completed so what we're going to do is we're gonna go to copy and paste them and we're going to delete this the main inquest again be wary of not deleting any characters you're not supposed to can we go to

**[15:41](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=941s)** paste whoops accidentally left out of character there now we want this to display as grey which you can do so by looking up on Google on how to get the hex color for gray but I have already got the color within X text file so all I need to do is just copy delete these last six characters and go paste and so now this quest message will display to the player of quest update and it will have this quest requirement

**[16:20](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=980s)** greyed out indicating that the quest the player has completed the quest requirement we click on ok and then the last thing we would probably want to do is we want to have the quest complete for the player so again we create another trigger we call quests completed we do the same we go to new action quest and we go to mark quest requirement as completed then we go back into this quest update to get the message we

**[17:04](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=1024s)** select the quest message has quest completed now we change this title to indicate that the player has finished the quest there's no point in displaying the quest requirements so we can get rid of that and this will essentially complete all the remaining or the last request Quan Minh for the quest and if message will display to the player alerting them that the quest has been completed now one thing we haven't done is we haven't

**[17:40](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=1060s)** made it said that these triggers run at all so what we're going to do is we're going to go into units and we're going to place a paladin on the map and since our quest requirements is for the player to reach a certain point we're going to have this achieved by making use of regions so if the player reaches this region here we double click on it to give it a name we'll call it region a and then we create another

**[18:20](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=1100s)** region over here double click on it call it region B so we are going to create an additional trigger we're going to call it player reaches point a we're going to set a new event that will make this trigger run when a unit enters region a we're going to a condition check to make sure that the unit that is entering the region actually belongs to the player so we go if the owner of the entering unit which

**[19:05](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=1145s)** is the unit entering region a is equal to the player which in this case is player one now the first action we're going to do is we're going to go to trigger and go turn off this trigger and what this will do is it will prevent this trigger from running several times if it has already ran or if the play has already reached the region and then we're going to go to the next trigger and we go run Qwest update and this will make it so

**[19:43](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=1183s)** that when the player reaches this region with one of their units it will mark the first quest requirement as completed and display the appropriate message and likewise we can do the same with point B where we just rename this trigger a unit enters region B and instead of displaying the quest update we display quest completed so here we have a basic functioning quest which simply at the beginning of the map after a few seconds

**[20:19](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=1219s)** it will display this quest as discovered and it will show what the player needs to do and then we have it so that once the player reaches certain regions or points in the map the quest will either be marked as update and if the player reaches both those regions then the quest will be completed and the appropriate message will display appropriately now one thing I've forgotten to do when creating this quest

**[20:54](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=1254s)** completed trigger is that we have marked the quest requirement the last remaining quest requirement to be completed but we haven't actually set the overall quest to be completed as well since they're two totally different actions so we can correct this by going into new action quest and then marking quest as completed and so now when the quest completes or when this trigger runs it will show both the quest and its

**[21:26](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=1286s)** remaining quest requirements as completed by the player one final thing to let you know is that I will be including these special characters and codes I have been using to color coordinate the quest message texts just to make it easier for you guys to incorporate it into your own map but other than that that pretty much finishes everything we need to know about making a standard quest in Warcraft 3 I hope you guys enjoyed this

**[22:00](https://www.youtube.com/watch?v=6Eg3qLISI_E&t=1320s)** tutorial video and if you did feel free to leave a like and subscribe to my youtube channel otherwise I'll catch you guys next time bye now
