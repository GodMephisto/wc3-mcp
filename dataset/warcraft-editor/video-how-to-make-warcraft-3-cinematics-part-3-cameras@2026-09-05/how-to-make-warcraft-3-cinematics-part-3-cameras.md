---
type: vendor-doc
package: video-turnro
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=a-LR0WYqr80
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 1777
chapters: 16
timestamped: yes
---

# How to make Warcraft 3 Cinematics Part 3 - Cameras

Source: <https://www.youtube.com/watch?v=a-LR0WYqr80>

Uploader: Turnro | Duration: 29m37s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

The third video of Turnro's Warcraft 3 cinematic tutorial series.
This video goes into depth about cameras and how they can be used to improve Warcraft 3 cutscenes.

## Chapters

- [0:00](https://www.youtube.com/watch?v=a-LR0WYqr80&t=0s) <Untitled Chapter 1>
- [0:46](https://www.youtube.com/watch?v=a-LR0WYqr80&t=46s) Create a Camera
- [1:35](https://www.youtube.com/watch?v=a-LR0WYqr80&t=95s) Edit Camera Properties
- [3:58](https://www.youtube.com/watch?v=a-LR0WYqr80&t=238s) Z Offset
- [4:55](https://www.youtube.com/watch?v=a-LR0WYqr80&t=295s) Rotation
- [6:27](https://www.youtube.com/watch?v=a-LR0WYqr80&t=387s) Angle of Attack
- [8:38](https://www.youtube.com/watch?v=a-LR0WYqr80&t=518s) Distance
- [10:12](https://www.youtube.com/watch?v=a-LR0WYqr80&t=612s) Far Clipping
- [12:27](https://www.youtube.com/watch?v=a-LR0WYqr80&t=747s) Field of View
- [15:32](https://www.youtube.com/watch?v=a-LR0WYqr80&t=932s) Preview Values
- [17:05](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1025s) Create the Camera
- [18:57](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1137s) Camera Panning
- [21:56](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1316s) Camera Field
- [25:57](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1557s) Rgb Colors
- [27:31](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1651s) Lock Camera Target 2 Units
- [28:16](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1696s) Shaky Camera

## Transcript

**[0:00](https://www.youtube.com/watch?v=a-LR0WYqr80&t=0s)** hello everyone it's turn row here bringing you the next video in my series of Warcraft 3 cinematic tutorial videos where we'll be going over cameras now for the first couple of videos we have just been concentrating on the essentials when it comes to making cinematics in Warcraft 3 which includes how to get a cinematic to work through the triggers as well as things we need to be aware of to prevent problems but

**[0:26](https://www.youtube.com/watch?v=a-LR0WYqr80&t=26s)** now that we've got that all done and dusted we can now concentrate on the specific areas that allow us to improve and enhance our cinematics and one of the fundamental ways in which this can be achieved is with cameras in the previous tutorial videos we learned that we can create a camera by going into the cameras panel in the world / side and clicking on create and then we can set this camera to be used within our

**[0:56](https://www.youtube.com/watch?v=a-LR0WYqr80&t=56s)** cinematics by going into the trigger and then going apply camera for a specific player but there is so much more we can do with cameras rather than creating them and setting them within our triggers for example we may want to alter the angle of the camera to give it a different look or we want the camera to be more zoomed in on a specific character or a location and we can do this by checking through its settings

**[1:25](https://www.youtube.com/watch?v=a-LR0WYqr80&t=85s)** which you can access by either double-clicking on the camera within the world editor or by right-clicking on the camera in the camera panel and going edit camera properties and we'll bring up a window that will allow you to set a whole bunch of variety of different options which we will go through one by one obviously you can set a name if you want to target X and target Y essentially these refer to the

**[1:55](https://www.youtube.com/watch?v=a-LR0WYqr80&t=115s)** coordinate positions of the cameras on the map referring to its specific locations what x and y mean if you look at the bottom left-hand corner when I have my mouse over the world editor map you will see it will say points and it will display a bunch of numbers the first number 187 9.0 refers to the x-coordinate of where the mouse currently is and the second number which is negative 270 36.2 refers to the

**[2:32](https://www.youtube.com/watch?v=a-LR0WYqr80&t=152s)** y-coordinate and as you see as I move my mouse the coordinates of this of the X&Y change so essentially speaking if we go back into the camera properties Target X refers to the horizontal position of the camera on the map so giving the camera x-coordinate a larger number will move the camera to the right else giving it a lower number will move the camera to the left likewise the y-coordinate refers to

**[3:06](https://www.youtube.com/watch?v=a-LR0WYqr80&t=186s)** the vertical position of the camera on the map so giving it a larger number will move the camera up and giving the camera a lower number will bring the camera down now the good news is you don't need to be too concerned about the x and y position of the camera on the map because its purpose is to move the camera and if all you want to do is just that then an easy way of achieving that is by left-click and holding on the

**[3:37](https://www.youtube.com/watch?v=a-LR0WYqr80&t=217s)** camera that you want and then using the mouse to drag the camera to the specific position on the map that you want so again it's not necessary to use x and y-coordinates to determine the position of the camera on the map but it is handy to know why they're there and what they do Z offset is another word for saying the height of the camera so at the moment it's set to 0 if we set it to a higher number say for example 50 and we

**[4:12](https://www.youtube.com/watch?v=a-LR0WYqr80&t=252s)** click on OK you'll notice that it moves the camera up so we can demonstrate this a lot better by changing the camera angle we see on the ward editor by holding down control all on the keyboard and then using the right mouse button in order to scroll up so we can have a better view on what the camera looks like when we change its setting so if we go back and we change the Z offset to 50 you'll notice that

**[4:43](https://www.youtube.com/watch?v=a-LR0WYqr80&t=283s)** the camera has been moved up and if we give it a much larger number like 200 it moves it up again in regards to rotation if we go back to the main view so obviously the camera can be rotated but more specifically you have the option of rotating the camera in a 360 degree angle so in a circle there's 360 degrees and you can use any number between 0 and 360 to specify a specific angle of the camera so if we for example set the

**[5:27](https://www.youtube.com/watch?v=a-LR0WYqr80&t=327s)** rotation to 0 the rotation is set right here and if we click on set it back to 90 the camera will be set to this position likewise we could set it to 180 or 270 now at any time if you would like to keep track of what the camera angle actually look like in the game we can do that by double clicking on the camera within the camera panel so for example this is the current default camera that we have and if we change its rotation to

**[6:09](https://www.youtube.com/watch?v=a-LR0WYqr80&t=369s)** 0 for example and we double click on camera oh two it will then change the position of the camera to the rotation that we have set it to which is handy for us to know for testing purposes the next property we can set when the camera properties is angle of attack now angle of attack is similar to rotation in that it allows you to rotate the camera between 0 to 360 degrees the only difference is the circle is instead flipped so we can

**[6:43](https://www.youtube.com/watch?v=a-LR0WYqr80&t=403s)** now rotate it from a side perspective so imagine where my mouse is hovering over we have a circle that is half of it is appearing above the ground and half of it is appearing below the ground for this reason when you're setting the actual value of the angle of attack it is recommended that you set it between a number of 0 and 270 degrees which is from anywhere between this zero degree angle to 270 great angle right here and

**[7:18](https://www.youtube.com/watch?v=a-LR0WYqr80&t=438s)** I'll explain why so if we set the attack on angle to 90 degrees for example and then we double-click on the camera in the camera panel to see what it looks like you'll notice we get a black screen and that is because we have set the angle of the camera of the angle of attack to inside the ground which is between 0 and 180 degrees so if you set the angle of attack to a number between these two values

**[7:52](https://www.youtube.com/watch?v=a-LR0WYqr80&t=472s)** you'll have the camera positioned inside the ground likewise if we altered this rotate the angle of attack so that it is a number between 180 and 270 degrees such as 200 you'll notice that the camera is now appearing above the surface that the camera is upside down which may in some situations be what you intend but in most times you ideally don't want to have the camera appear upside down so a number between 0 and

**[8:29](https://www.youtube.com/watch?v=a-LR0WYqr80&t=509s)** 270 will work fine such as 325 so the next setting is distance which is relatively straightforward we're giving the camera a distance of a lower number such as 1 1 500 we'll move the camera in and a much larger number we'll move the camera out and we can see this one when we look at the camera by double-clicking on it we have a zoomed out view and if we set this to a lower number and then see what it looks like again by

**[9:08](https://www.youtube.com/watch?v=a-LR0WYqr80&t=548s)** double-clicking on it we can see the camera is nelle zoomed in and if you are looking at the camera from a certain perspective and you wanted to go back to the default view you can do this by clicking on the scroll on your mouse so the next setting is the roll so this will actually invert or rotate the actual camera itself rather than the angle of the camera so at the moment it's set to zero if we set it to for

**[9:43](https://www.youtube.com/watch?v=a-LR0WYqr80&t=583s)** example 90 degrees you'll see that the camera is rotated on a 90 degree angle and we can alter this again by giving it a different number making it look upside down on another angle and in a top right position so if we for example look at it from a 90 degree angle this is what it will look like so in order to understand what far clipping means I'm going to demonstrate it by using a simple concept alright so

**[10:20](https://www.youtube.com/watch?v=a-LR0WYqr80&t=620s)** in this map you'll notice that there are a bunch of trees that I have set which is a fair distance away from the camera so for camera one for example if we set these settings so that we can see the trees from a distance so let's say for example we set the distance to about 135 oh and we set the angle of attack to about 340 delete this one and then we double click on camera one you'll notice that the cam the trees don't show up and

**[10:55](https://www.youtube.com/watch?v=a-LR0WYqr80&t=655s)** that is to do with how Warcraft rig works in that in order to minimize rendering our 3d models that it doesn't need in order to save processing time and CPU usage on your computer it will only render 3d models that are within the view of the current camera so let's say for example we are where sama is so imagine we are in game and we are playing a Semeru 3d models will only be rendered within a specific radius of

**[11:33](https://www.youtube.com/watch?v=a-LR0WYqr80&t=693s)** where the player is currently at anytime so if we have a base at the very top right hand corner of the map the 3d model is associated at that specific point in the map right actually get rendered so if there is a situation where we have a cinematic and we want the camera to view 3d models at an extended distance then we can achieve this by extending the far clipping value so if for example we set this value from

**[12:04](https://www.youtube.com/watch?v=a-LR0WYqr80&t=724s)** 5000 to 6500 and then we double-click on the camera you can now see the trees in the background so we'll set this to a closer field of angle of attack and there we go the last setting in cameras is to do with field of view now field of view is a setting that I personally do not use because you can essentially get away with that by using a combination of distance and far clipping but what it essentially does is it combines the

**[12:43](https://www.youtube.com/watch?v=a-LR0WYqr80&t=763s)** effects of distance and far clipping as one so let's say for example we reset far clipping we set it back to 5000 and then we reset what the camera will look like by double-clicking on it if we change the distance of the camera so let's say 900 900 we will now see the trees because when we set the distance of the camera it automatically adjusts the far clipping you in order to determine which 3d models to render so if we revert this

**[13:27](https://www.youtube.com/watch?v=a-LR0WYqr80&t=807s)** back to how it was we can no longer see the trees however if we wish to increase the distance of the camera to give a wider view but we want to maintain the field of view so that models that we can currently see will still be seen when we zoom the camera out then we can make use of the field of view so for example we can set this value to a whole number as 120 for example and then we double-click on the camera and we can see that the

**[14:02](https://www.youtube.com/watch?v=a-LR0WYqr80&t=842s)** camera has zoomed out but any models that we saw previously in its previous setting are still maintained now one word of caution when it comes to setting the field of view and the far clipping value of the camera in that the reason why Warcraft 3 is limited to how much models can be read at at any one time is for computer hardware purposes so if you deliberately set these values to an extremely high number then it means that

**[14:37](https://www.youtube.com/watch?v=a-LR0WYqr80&t=877s)** potentially when a player plays the cinematic and if they have a poor computer the game will either lag or in worst case scenarios it will crash so in most situations you'll find that you actually don't need to edit these two values in order to set the camera up however if you want to do like a wide screen shot or if a certain situation where you have to increase the zoom out of the camera and maintain the 3d models

**[15:10](https://www.youtube.com/watch?v=a-LR0WYqr80&t=910s)** that are on the screen then only incremented by a reasonable amount such as the current default far clipping is 5000 setting a number 260 or 6,500 is a reasonable number as we can see in this example here and then finally there is also a preview values in window so as you know each time that we want to see a change that we've made we've clicked on OK to apply the changes and then we double-click on the camera

**[15:43](https://www.youtube.com/watch?v=a-LR0WYqr80&t=943s)** in order to see what changes were made and what the camera will look like however we can instead as an alternative click on preview values in my window so that if we change its values while we're in the camera properties such as its distance and then we click out we can then see the immediate effects our pop will happen if we change the camera settings and so if we set this is zero for example and we can continue to make

**[16:17](https://www.youtube.com/watch?v=a-LR0WYqr80&t=977s)** these changes to see exactly what the camera will look like as we continue to make its changes so throughout these settings it may be a little confusing at first exactly what they do considering that we I have just gone through and explained what they do if you are still a little confused then it is recommended that you modify and play with these values such as say on set setting it to 50 with preview values in main window on

**[16:49](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1009s)** and seeing what exactly it will do when we change the camera value now the next question then comes in mind is so every time we want to create a camera we have to create the camera and then modify the individual settings in order to get the specific angle and distance and other properties that we want for the camera and this is sort of true but there is an alternative way in order to help you get the want so that you don't have to

**[17:25](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1045s)** modify its values as much so what we can do in order to make it easier for ourselves is you'll notice that within the world editor you can do things such as use the scroll in order to scroll in with your mouse or you can click on ctrl on your keyboard in order to run Tate the camera both from a rotating point of view and a angle of attack point of view and you'll find that if you are doing this and you click on

**[18:03](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1083s)** create camera it will create the current camera in the current view in how you're viewing it in the world editor so you can use this technique in order to have a base camera of what you essentially want it to look like and then you can go ahead and edit its properties as you wish others how we can set the different properties of the camera to get the specific angle and distance and other properties that we want the next part

**[18:37](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1117s)** we're going to go through is the different ways that we can use our cameras to make the cutscenes that we make more interesting for the player and when it comes to me personally when I'm using cameras in triggers there's really only two main actions that I use and the first one is called camera panning and camera panning is another term that is simply means we want to move the camera from one position of the map to another

**[19:08](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1148s)** so before we get into the triggers we're just gonna go into the region panel and just create a basic region we'll just created here we'll give it a name by double clicking on it and we'll call it camera point and then as we go into the trigger editor and we go to the cinematic trigger where we have the start of the cutscene at the very beginning we will add a new action and we will click on see for camera and we

**[19:48](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1188s)** click on pan camera will set the camera to the current player and we'll move it to the position of the region that we just created over two seconds so the initial effect that we want is that the camera is currently here and we want the camera to move to this region over two seconds when it comes to setting the actual value of pan camera whether or not how fast or how slow the camera moves to one position to another

**[20:22](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1222s)** depends on two factors it depends first of all on the amount of seconds you allow the camera to move so ideally giving the amount of seconds a larger number will mean that the camera will move at a much slower pace than if you gave it a lower number but in addition to the amount of seconds you give for the camera to move how fast or how slow the camera moves also depends on the distance between the camera and the

**[20:50](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1250s)** region so if we have a camera here for example and we move the region all the way up here because the distance between the two points is greater then it means that the camera will be forced to move faster in order to reach this region over the specified number of seconds so in general there is no specific rule in how long you can have the camera pan fall you'll personally have to play around with it in order to have the camera move

**[21:22](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1282s)** but not have it move too fast or too slow depending on how many seconds you allow it and the distance between the camera and the region so once we have the camera move the other camera action that I use in triggers is if we go into safer camera again for actions I make use of the set camera field so whereas we have a specific camera action for moving the camera from one point to another SEC camera field allows us to

**[21:58](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1318s)** change the actual properties of the camera on the fly or in real time so when we have the camera here it allows us to say for example change the rotation of the camera or the angle of attack or even the distance to give it to create some interesting effects when it comes to viewing the cinema so if we click on distance to target it will essentially allow us to use the different settings of the camera to say

**[22:31](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1351s)** for example increase its height to change the rotation of the camera if we want to flip the camera upside down for example in this particular cinematic we are just going to zoom the camera in slightly so if we double-click on the camera we can see that it's currently set to a value of 1350 and for this particular example we'll be setting it to 1150 and we will be setting the camera over eight seconds we essentially

**[23:10](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1390s)** have eight seconds chosen because it takes about 6 seconds for the rest of the cutscene to play along with the cutscene to fade out with its fade-out animation so essentially we have the camera moved from one point to another over two seconds and then we have the camera continuously zoom in until the cinematic finishes so what we can do now is we can say what we have done and we can test this in actual Warcraft 3 to

**[23:43](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1423s)** see how it works so as you can see we have semeru the blade master where we move them into the rocky region in order to start the cinematic and then once the cinematic plays we first of all happen move slightly forward for a couple seconds and then we have the camera zoom in over the remainder of the cutscene and as you can already see this is the exact same cutscene that we made in the first tutorial video with the only

**[24:12](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1452s)** change that we've made is with the camera angles and you'll notice that this has made the cutscene look far more interesting to watch then when we had it previously made in the first tutorial video now while watching the cinematic you would have noticed that the trees in the background while they did display and were visible to the player they were sort of black or in a faded out State and this is to do with a fog of war in

**[24:43](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1483s)** Warcraft 3 which occurs automatically if for example we want to reset or change the fog of war so that models in a far enough distance display crystal clear we can do this by changing the fog effect within the actual cinematic so we can go into the trigger editor and say after environments at sky we click on new action we go efore environment and then we go set fog now in regards to what settings to choose from you would

**[25:19](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1519s)** generally leave Lee linear as it is as well as density Zed start and Z end refer to the range for the far clipping of the camera so the idea is with this number 6500 in this case you want it to be between the start end value which is 1000 and the Z end value which is 8000 and since 6500 falls within that range of two numbers then we can leave it as it is likewise we can set the RGB colors so we can set the fog of war to be a

**[26:02](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1562s)** different color other than black such as if we change it to yellow or green or purple etc in this case we don't want to apply a specific color we just want the models to appear as they normally do in which case we can just set these RGB colors to all be 0 and then at the end of the cinematic we want to reset the fog back to its usual state so we go to the end of the cutscene in the trigger we click

**[26:34](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1594s)** on a for environment again and then we click on reset fog and now if we save that and we retest this cinematic within our league and again we can now say the cinematic that we have previously made before but the only difference being is that the trees in the background will now display as normal instead of in a blackish or faded out fog effect so in addition to what we've covered over setting the camera properties as well as

**[27:11](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1631s)** using cameras within our triggers there are a few other commands that we can use that I personally don't really use much when it comes to making my own maps but you may find them useful so if we go back into camera a few other options we have include lock camera target 2 units so this trigger can be used if you want the camera to be locked on to a unit so that the unit appears in the middle of the screen so that whenever the unit

**[27:46](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1666s)** moves the camera will move alongside with it and in the end if you want to reset the camera so that it no longer locks onto the unit you can either make use of the commands stop the camera or you can do what we have done in previous tutorial videos where you make use of the reset game camera action command another camera action that we can use that you could find useful is shaky camera so this can be used if you want

**[28:20](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1700s)** to create an effect of earthquake for example where you have the camera shake to demonstrate what is happening on screen so magnitude refers to how heavy the camera shakes so giving it a value of three means that the shake of the camera will be fairly light whereas if we change it to a value of 10 for example then the shaking of the camera will be more predominant and in regards to other camera actions there

**[28:54](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1734s)** are a lot more that we can use like for example rotate camera around point but personally through my experience the main camera actions they are useful is pan camera and set camera field and that's going to pretty much wrap up the end of this tutorial video when it comes to using cameras in cinematics I hope you guys found this tutorial video useful if you did leave a like on the video as well as subscribe to my youtube

**[29:26](https://www.youtube.com/watch?v=a-LR0WYqr80&t=1766s)** channel for notifications on future videos in the series but until the next video I'll catch you guys next time bye now
