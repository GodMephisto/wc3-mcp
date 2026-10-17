---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/exploration-of-vertex-coloring.29970/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Exploration of Vertex Coloring
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/exploration-of-vertex-coloring.29970/>
#### chovynz

#### chovynz

 Level 3

JoinedJan 18, 2007Messages12

Resources: 1

#### Resources
Tutorials 1

Contents of this Tutorial

What is Vertex Coloring?

Technical stuff:

Important Info:

How To use Vertex Coloring in Warcraft3

Formulas of RGB

Addendum:

Attached Map Specifics & Smooth Coloring Walkthrough (1st and 2nd Triggers):

3rd and 4th Triggers

Taking The Color Slider Further: (Sequencing colors)

Possible Uses of Vertex Color Sliding

Credits & Download Vertex Sliding map

I'v'e been playing with the vertex coloring of units. Vertex coloring changes the entire color of the unit. Player color is different and only changes the specific team colored parts of a model i.e flags, hands, feet. Vertex coloring can make the unit(s) and map look ugly if overused or used incorrectly. Because of the way vertex coloring changes the look of the unit, I recommend using player colour or team color instead. The methods below do not work with player colors (as far as I know) However, this post is about vertex coloring so on with the show.

The Vertex Coloring function can be found in:

Events > Animation > Change Unit Vertex Coloring

TopWhat is Vertex Coloring?

Vertex Coloring is a color mask applied over the model. Vertex Coloring is used by changing the values of the (Red%, Green%, Blue%) and Transparency fields. Vertex coloring applies to EVERYTHING on a unit - this includes buffs, attachments, auras...everything. (Although, strangly enough when vertex coloring is applied to a wisp, you can still see some other colors coming through. This also happens to some buff effects. I dont know if this is something Blizzard missed or if it's intentional.)

TopTechnical stuff: - Skip if you dont care.

Vertex Coloring is used by changing the values of the (Red%, Green%, Blue%) and Transparency fields. These are known as RGB and is a screen specific color system. This applies to TV's, computer monitors, LCD screens, projectors and other visual display equipment.

Why is it important to know this for Warcraft3? (if you're coloring units this applies to you - if you're not coloring anything skip this whole thread) Warcraft3 is obviously played on computer monitors or TV's of some description. So Wc3 uses RGB values. When we color using RGB the values range from 0 - 255. 0 is totally off. 255 is totally on. Another way of saying it is 0 means your using none of the color. 255 means your using as much as you can!!

For those who want to check out some more technical information on color systems click this link for the wiki color theory webpage.Some more useful information for RGB can be found at these two pages here. RGB is an additive color system, meaning the more color you put in....the whiter it goes. Imagine a room that has no windows and 3 light switches in it. When all three lights are on you can see everything in a normal color. When all 3 lights are off you cant see anything.

TopImportant Info:

Note: The RGB color range uses 0 - 255, but Warcraft3 vertex coloring defines it using a %.

0 = 0%, 255 = 100%

You need to know the above if you want to change the vertex coloring to specific colors.

TopHow To use Vertex Coloring in Warcraft3

Vertex coloring in Warcraft3 is simple really. Make a trigger with the following:

Change Unit Color
Events
Put your event here

Conditions

Actions
Animation - Change Footman 0000 <gen>'s vertex coloring to (100.00%, 0.00%, 0.00%) with 0.00% transparency

That's it. That's as easy as it can be to change the vertex color of a unit. If you want to make your unit specific colors then you can use some of the formulas below. Note: All of the following formulas use 0.00% transparency.

TopFormulas of RGB

As I said before, Vertex Coloring uses a MASK of color over the model. This mask of color is defined by setting the values of the RGB fields in the Actions > Animation > Change Units Vertex Coloring function. For the purpose of this tutorial I will show the values like this RGB (100%, 100%, 100%). Remember: The value range is from 0 - 255. RGB is Red, Green, Blue.

RGB (0%, 0%, 0%) This makes the RGB vertex coloring black, which makes the unit black. It's like turning all 3 lightswitches off in our room. There's no light so you cant see!

RGB (100%, 100%, 100%)

This sets the RGB vertex coloring to white. It's like turning all 3 lightswitches on in our room.

So this 'formula' will make a unit total white? No, sorry. What this does is set the vertex colouring MASK to white. Wc3 treats a WHITE MASK as transparent. This transparent WHITE MASK is different to the 'transparency' mentioned in the vertex coloring function. The unit will be the normal unit color if you use the above formula. This is the default RGB values when you create a new vertex coloring function. *Does this idea need clarity? -comment if you can rephrase this idea better / simplier.*

RGB (100%, 0%, 0%)

This formula will make your unit red.

RGB (0%, 100%, 0%)

This formula will make your unit green.

RGB (0%, 0%, 100%)

This formula will make your unit blue.

RGB (100%, 100%, 0%)

This formula will make your unit yellow.

RGB (100%, 0%, 100%)

This formula will make your unit magenta.

RGB (0%, 100%, 100%)

This formula will make your unit cyan.

TopAddendum:

1) If you apply a buff then vertex coloring, the buff will be colored as well.

2) If you apply vertex coloring then a buff, the buff will be normal color, but the unit will be the vertex color.

3) Wisp's and certain other units still display other colors when vertex coloring is applied to them. Without having tested what these odd colors are, I'm assuming these colors are the player team colors.

TopAttached Map Specifics & Smooth Coloring Walkthrough:

After having played around with coloring for some time now, I've gotten sick of the instant color change that happens when the vertex color function is applied to a unit. I find it jarring and unpleasant. I have made some triggers that apply vertex coloring smoothly, giving a nice transition from one color to another!

Imagine you're at a light-desk / sound-desk at a concert. You are the controller. You have 3 sliders infront of you. The control panel looks like this:

 Code:

 __|__R__|__G__|__B__|__
255 | ^ | ^ | ^ | 100%
 | : | : | : |
 | : | : | : |
^ | : | : | : | ^
 | : | : | : |
0 __|__:__|__:__|__:__|__ 0%
off/on X O X O X O

This is effectively what is happening in the attached map. Instead of instant jarring color changes we have individual color sliders and toggles.

Following is how to do smooth color changes:

First, on an empty map go to variables and make 4 variables with the variable type set to Integer.:

TriggerRun

vred

vgreen

vblue

You need to set the variables first so that the sliding color variables will work. You can do this either at the variable creation (variable initial value) or using the following triggers.

Initialisation
Events
Map initialization

Conditions

Actions
Set TriggerRun = 0

Set vred = 255

Set vgreen = 255

Set vblue = 255

For each color we need 4 triggers. I'll use red as an example, but the same code and format applies to green and blue colors as well. Just copy and paste once you have the red slider completed, then change the red references to green or blue.

Remember that we made the variables 255 at the begining? This is the maximum value for the color range. Because we start the variable vred at 255, the red is already turned on full. So the first thing to do is slide the red light down - effectively dimming it until no red shows. To do so we create an initiating trigger first:

reddn init
Events
Player - Player 1 (Red) types a chat message containing reddn as An exact match

Conditions

Actions
Set TriggerRun = 0

Set vred = 255

Trigger - Turn on Red Down <gen>

The reddn init trigger does this: When Player 1 types "reddn" (red down truncated) - TriggerRun is set to 0, vred is set to 255 and the Red Down trigger is turned on.

Those of you with sharp eyes will have noticed that vred has already been set to 255 at the variable initial value. So why are we doing it again here? And what is happening with TriggerRun? You will see the answer to these questions shortly.

Here is the Red Down trigger - the slider if you will:

Red Down
Events
Time - Every 0.01 seconds of game time

Conditions

Actions
Set vred = (vred - 1)

Set TriggerRun = (TriggerRun + 1)

Animation - Change (Last created unit)'s vertex coloring to ((Real(vred))%, (Real(vgreen))%, (Real(vblue))%) with 0.00% transparency

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
TriggerRun Greater than or equal to 255

Then - Actions
Trigger - Remove (This trigger) from the trigger queue

Trigger - Remove reddn init <gen> from the trigger queue

Trigger - Turn off (This trigger)

Else - Actions
Do nothing

Note: Make sure that Red Down trigger is set to 'Initally Off' - uncheck the little 'Initially On' check box. The Red Down trigger does this:

Every .01 sec of game-time:

Subtract 1 from vred

Add 1 to TriggerRun

Change the Last Created Unit's Vertex Coloring to the updated variables.

Loop this trigger until TriggerRun hits or goes beyond 255 then:

- Remove this trigger from the trigger queue

- Remove the reddn init trigger from the trigger queue

- Stop this trigger

What this trigger does is count vred down from 255 to 0. At the same time it counts TriggerRun up from 0 to 255. When TriggerRun hits 255 the trigger stops.

But what happens if this trigger is run twice? (The very scientific method of typing this trigger - twice, in game - came up with some very interesting problems.) In earlier test runs of earlier versions of this trigger I typed 'reddn, reddn, redup'.

 the redup trigger didn't run!

I was about to ask for help again on TheHelper when during typing my question I realised what was happening. I had run the reddn trigger once. This meant that vred was now at 0, and TriggerRun was now at 255. Then I ran reddn again. The trigger did what it was told to do and vred subtracted 255 from 0. This made the value of vred = -255. So of course when I typed redup, the trigger ran. But since we had already gone beyond 0 into the negatives, there was nothing to show for it - because 0 is the lowest integer in the red color range. 0 = 0, -250 < 0 so nothing showed.

This is why we set vred to 255 again in the reddn init trigger. And at the same time as doing so, we put a limiter on TriggerRun AS WELL, so that everytime this trigger is run we can see a result. Now if you type reddn reddn reddn you will see the red fade out, jump back to full strength, fade out, jump back to full strength, fade out. It looks wierd - I know, but this is exactly what we want. The red color is set to 0. It is totally dim, theres is no red light showing, the red lightswitch is off. If we were at our light-desk / sound-desk again it would look like this:

 Code:

 __|__R__|__G__|__B__|__
255 | : | ^ | ^ | 100%
 | : | : | : |
 | : | : | : |
^ | : | : | : | ^
 | : | : | : |
0 __|__^__|__:__|__:__|__ 0%
off/on X O X O X O

Because we have turned vred to 0, to nothing, effectively turning it off, we need a trigger to turn it back on again.

TopNow we come to the 3rd and 4th triggers of the Smooth Red Vertex Coloring Sliding Triggers.

Again make an initiating trigger that un-dims the red light so we can see it again:

redup init
Events
Player - Player 1 (Red) types a chat message containing redup as An exact match

Conditions

Actions
Set TriggerRun = 0

Set vred = 0

Trigger - Turn on Red Up <gen>

Notice that we have set vred to equal 0 in this redup init trigger? The reason for this is simply; the red is already at 0. So we tell the init trigger that red should be set to 0. Otherwise we could get vred = 510 if someone runs redup twice. Seeing as how 255 is the maximum value for red, nothing would display for the 2nd redup command, but it would still activate and ... screw things up.

Its just like before ... only in reverse. And again the actual trigger:

Red Up
Events
Time - Every 0.01 seconds of game time

Conditions

Actions
Set vred = (vred + 1)

Set TriggerRun = (TriggerRun + 1)

Animation - Change (Last created unit)'s vertex coloring to ((Real(vred))%, (Real(vgreen))%, (Real(vblue))%) with 0.00% transparency

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
TriggerRun Greater than or equal to 255

Then - Actions
Trigger - Remove (This trigger) from the trigger queue

Trigger - Remove redup init <gen> from the trigger queue

Trigger - Turn off (This trigger)

Else - Actions
Do nothing

Notice the only things that are different in this trigger? Lets look again:

Set vred = (vred + 1)

...

Trigger - Remove redup init <gen> from the trigger queue
This redup trigger is exactly the same as reddn trigger, only it's reversed.

It ADDS 1 to vred every .01 second of gametime. Since vred has been set to 0 in the init trigger, vred will count up to 255 and stop because TriggerRun stops this trigger at 255. Just like before. Now, we must explain 1 additional function. This function makes the whole process (the 2 init triggers and two Red dn/up triggers) work like it should.

Animation - Change (Last created unit)'s vertex coloring to ((Real(vred))%, (Real(vgreen))%, (Real(vblue))%) with 0.00% transparency
In the RGB (100%, 100%, 100%) fields of Vertex Unit Coloring, those 100% fields are REAL fields. Integer variables will not be selectable in REAL fields. So we must convert our integer variables (vred, vgreen, vblue are integers set from 0 - 255) to an input that RGB can understand. Fortunatly there is a function for this already inbuilt into World Editor. Bless you Blizzard.

When you create this command:

Animation - Change (Unit)'s vertex coloring to (100%, 100%, 100%) with 0.00% transparency
Click on one of the 100% fields and use this function:

Function > Conversion - Convert Integer to Real
The text box underneath will display :

Real(Point Value of Footman)

Click on Point Value of Footman and then select Variable > vred. Now click Ok and Ok. Do the same for the other 100% fields and select the other color variables.

Your function should now look like this:

Animation - Change (Unit)'s vertex coloring to ((Real(vred))%, (Real(vgreen))%, (Real(vblue))%) with 0.00% transparency
Click on the Unit field and select Function > Last Created Unit. Now your function should look like this:

Animation - Change (Last created unit)'s vertex coloring to ((Real(vred))%, (Real(vgreen))%, (Real(vblue))%) with 0.00% transparency
This will now work, no matter what unit is created, no matter what value the vred, vgreen and vblue is set to. All you need to do now is leave it right where it is, on the third row of the actions of any color up or down trigger you use.

Right! Now to see it in action. Here's a script I have used in this map:

paladin
Events
Player - Player 1 (Red) types a chat message containing paladin as An exact match

Conditions

Actions
Unit - Remove (Last created unit) from the game

Unit - Create 1 Paladin for Player 1 (Red) at (Center of (Playable map area)) facing Default building facing degrees

Animation - Change (Last created unit)'s vertex coloring to ((Real(vred))%, (Real(vgreen))%, (Real(vblue))%) with 0.00% transparency

See that? There's that animation function again. Well, That's the red-color-vertex-slider-toggle-trigger-function-upperer that we want to use to make smooth color changes to our red vertex color changing units done!:shades: All you need to do now is Copy and paste the red triggers (reddn init, Red Down, redup init, Red Up) and change any reference from vred to vblue or vgreen. Thats it your done! Congratulations! You don't need to continue with the rest of this tutorial. You have fully functional vertex coloring sliders that will work quite well to Player 1 typing in certain commands. You can of course use any events yo uwish such as dialogs, or clicking on other units. There are a few good ideas down there though, so do feel free to carry on.

TopTaking The Color Slider Further:

Going back to the light / sound desk analogy, I don't want to be sliding my fingers up and down the control board for the whole concert (as fun as that is) especially when I could be sitting back relaxing, enjoying the music. So in taking this slider idea further I have programmed my control board to do the sliding for me.

 Code:

 __|__R__|__G__|__B__|__
255 | ^ | ^ | ^ | 100%
 | : | : | : |
 | : | : | : |
^ | : | : | : | ^
 | : | : | : |
0 __|__:__|__:__|__:__|__ 0%
off/on X O X O X O O < - Big Red Button. Push it! You know you want to!!

In Warcraft3 terms I have made a sequencer out of my triggers. Specifically:

color loop
Events
Player - Player 1 (Red) types a chat message containing loop as An exact match

Conditions

Actions
Trigger - Add reddn init <gen> to the trigger queue (Checking conditions)

Trigger - Add greendn init <gen> to the trigger queue (Checking conditions)

Trigger - Add redup init <gen> to the trigger queue (Checking conditions)

Trigger - Add bluedn init <gen> to the trigger queue (Checking conditions)

Trigger - Add greenup init <gen> to the trigger queue (Checking conditions)

Trigger - Add reddn init <gen> to the trigger queue (Checking conditions)

Trigger - Add greendn init <gen> to the trigger queue (Checking conditions)

Trigger - Add blueup init <gen> to the trigger queue (Checking conditions)

Trigger - Add greenup init <gen> to the trigger queue (Checking conditions)

Trigger - Add redup init <gen> to the trigger queue (Checking conditions)

This plays the color init triggers in the order they are added to the trigger queue. This particular sequence has been set to go from full on to full off and back again using a binary sequence. Only one slider moves at a time in this sequence. If you wanted to, you could have them moving all at the same time in different directions and speeds or linked so they are the same value.

Here is a good trigger to include in ANY map that uses color sliders like this, especially if there are sequences involved:

reset
Events
Player - Player 1 (Red) types a chat message containing reset as An exact match

Conditions

Actions
Trigger - Clear the trigger queue

Trigger - Clear the trigger queue

Set vred = 255

Set vgreen = 255

Set vblue = 255

Animation - Change (Last created unit)'s vertex coloring to ((Real(vred))%, (Real(vgreen))%, (Real(vblue))%) with 0.00% transparency

This trigger interrupts any loops or seqences we have set up, resets the color variables to their default maximum and changes the last created unit's vertex coloring to the updated color values.

TopIdeas not included in this map but generated from these sliders and tutorial:

Feel free to use any ideas in or generated by this tutorial! No credit needed.

1) Sliding function for transparent units. Good for smooth stealthing of units. (I think some spells actually already do this in DOTA Allstars. And Im not sure but I think Windwalk does this as well)

2) Camouflage game where you hunt the other players. All players have a toggleable auto ability that can sense what color the ground is and the ability will copy the value accordingly and smoothly. You could then 'hide in plain sight.' Especially if there are other units not moving on the same ground. Stand still and your opponents would have a hard time seeing where you specifically are. I would set a time of 2-3 seconds for the color change so that they have a chance to see you when you cross borders of different colored ground.

3) Slow changing sun, environment or unit color from region to region. (vred = distance from center of region elf_land to center of orc_land)

4) Pulsing Transparencies ... possibly applied to dummy units or abilites for cool effects in spells, or hunter seeker missles.

5) Got an obnoxious player in your game? Turn all his units vertex colored Red (smoothly) .

6) Disease / danger / coldness visual warnings over extended periods of time in RPG's, AoS and other types of games.

7) Changing TD towers to a different sort of tower. (i.e Fire to Cold)

TopCredits & Last Testament and Will:

Credits and Thanks to Dax and Scythetleppo @ TheHelper.net - I've been able to come up with these sliding triggers because of their comments and help.

Download the Map attached below to see - this post and the learning from it - in action.

If you see any mistakes in this post / tutorial Please let me know!

#### Attachments

Vertex coloring.w3x

 22.4 KB

 · Views: 631




 Last edited: Feb 24, 2007

#### Cheshire

#### Cheshire

 Level 26

JoinedMar 29, 2020Messages1,482

Resources: 4

#### Resources
Icons 3Maps 1

here's a super useful link with a full sample palette of a ton of different RGB recipes

Percent Color Codes

 You must log in or register to reply here.


### Similar threads

 Effortless and Efficient Unit Group functions for GUI Triggers


Spacebuns

Jan 6, 2022

Trigger (GUI) Editor Tutorials

Replies4Views4K

Jan 16, 2022
Spacebuns

Locked

 Change unit's vertex coloring


-SoulBurn

Aug 15, 2010

Modeling & Animation

Replies7Views2K

Aug 27, 2010
Thyrael

 [Snippet] GetUnitVertexColor


Magtheridon96

Mar 3, 2012

Graveyard

Replies13Views3K

Oct 17, 2012
Magtheridon96

[Spell]
 Temporarily changing unit color


Astral_JG

Jan 28, 2026

World Editor Help Zone

Replies2Views310

Jan 30, 2026
Astral_JG

Locked

 Water elemental vertex coloring fixed?!


Abelhawk

May 6, 2017

World Editor Help Zone

Replies2Views1K

May 6, 2017
Dr Super Good

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Trigger (GUI) Editor Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top