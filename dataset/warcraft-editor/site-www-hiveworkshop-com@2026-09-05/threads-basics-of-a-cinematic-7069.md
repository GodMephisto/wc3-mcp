---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/basics-of-a-cinematic.7069/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Basics of a Cinematic
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/basics-of-a-cinematic.7069/>
#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Basics of a Cinematic

By Archian

Introduction:

In this tutorial you will learn about starting and ending a cinematic, aswell as some few basic hints.

Starting a cinematic:

A cinematic can have diffrent kinds of events, for instance a unit entering a region or time elapsed etc. this is all up to you what you wish to start the cinematic with.

Cinematic Mode:

You have to be in cinematic mode in order to make a cinematic.

"Turn cinematic mode On for (All players)" This will disable Fog of War,The Black Mask and disable the user control.

Cinematic Mode
Events

Conditions

Actions
Cinematic - Turn cinematic mode On for (All players)

(Example of getting to the "Cinematic Mode")

Using the "Cinematic - Turn on letterbox mode (hide interface) for (Your and/or All Players): fade out over 2.00 seconds" will fade the interface over the amount of seconds you've choosen (In this case 2.00 seconds) this will make it look like a more "clean, smooth" fade over.

Using the "Cinematic - Disable user control for (Your and/or All Players)" removes all units from the selection of the player, removing the normal circle under the units when you select them. This will also ensure that when the cinematic is over, they won't have their previously selected units selected.

Starting the Cinematic
Events

Conditions

Actions
Cinematic - Turn cinematic mode On for (All players)

Cinematic - Turn on letterbox mode (hide interface) for (All players): fade out over 2.00 seconds

Cinematic - Disable user control for (All players)

Cinematic - Clear the screen of text messages for (All players)

Cinematic - Turn subtitle display override On

(These are some of the triggers im using to start the cinematic)

"Scenes":

When you're in the "Cinematic Mode" you can now begin making the cinematic.

Notice, that scenes are a custom term, you decide what the cinematic is going to contain, the editor isn't going to make it for you.

Cameras:

You have to create custom cameras to change the cinematic view.

Do this under "Camera Pallete" and click "Create Camera".

Place the camera where you want it on the map. (Double click the camera to view the available options for it).

Camera Name:

The camera name is actually irrelevant; though it would be smart if you're useing additional cameras.

Camera Values:

Target X: The X location of the camera's target.

Target Y: The Y location of the camera's target.

Z Offset: The location of the camera itself (height).

Rotation: Rotation of the camera around the target.

Angle of Attack (AoA): The angle that the camera is pointing towards the target.

Distance: The distance between the camera and the camera target.

Roll: Roll is the angle that the camera is turned. This can turn it upside down, or on its side.

Field of View (FoV): The width that the camera can see. Changing this value 'zooms' the camera in and out.

Far Clipping (FarZ): Total distance which the camera can see.

To apply a camera, use a trigger, for instance the one shown bellow.

Apply Camera
Events

Conditions

Actions
Camera - Apply Camera 001 <gen> for Player 1 (Red) over 0.00 seconds

You will discover that there are many ways to apply a camera as well as to use it.

NOTE - Many cinematics use multiple cameras, not just one. You will probably come to discover that as you work on your cinematic.

"Conversations/Text Message":

Many people who's creating a cinematic (for a role playing game for instance) usually use "conversations" between the characters.

This can be done by using for instance the trigger:

Conversation
Events

Conditions

Actions
Cinematic - Cinematic - Send transmission to (All players) from Footman 0000 <gen> named Footman: Play No sound and display Why hello there!. Modify duration: Add 0.00 seconds and Wait

Notice you can actually use sounds for the characters. Simply import the sounds you wish the unit to "say" and change the "Play No sound" field to the sound you wish.

Ending the cinematic:

When you wish to end your cinematic, you should do the opposite of what you did to start the cinematic.

Also this can have different kinds of events, which it needs to end the cinematic.

"Cinematic - Enable user control for (All players)" makes you able to control units again.

"Camera - Reset camera for Player 1 (Red) to standard game-view over 0.00 seconds" will reset the camera to the standard game-view of warcraft.

Ending the Cinematic
Events

Conditions

Actions
Cinematic - Turn cinematic mode Off for (All players)

Camera - Reset camera for Player 1 (Red) to standard game-view over 0.00 seconds

Good luck furthermore in the creation of your cinematic, and may you improve your skills <


 Last edited: Oct 4, 2022

#### Gogiman42

#### Gogiman42

 Level 4

JoinedAug 24, 2017Messages31

Nice job, good tutorial!

#### raypack

#### raypack

 Level 31

JoinedDec 3, 2020Messages1,261

Resources: 10

#### Resources
Models 1Maps 8Tutorials 1

Gogiman42 said:

 Nice job, good tutorial!


Click to expand...

Bro casually responding 16 years later

 You must log in or register to reply here.


### Similar threads

 Making Cinematics Skippable


TDA

Feb 25, 2011

Trigger (GUI) Editor Tutorials

Replies24Views29K

Mar 8, 2012
The Wizard

Locked

 [Cinematic Problem] - Where is the failure?


Sporling

Aug 9, 2009

Triggers & Scripts

Replies3Views2K

Aug 9, 2009
Sporling

Locked

 Cinematic


Razosh

Jun 19, 2013

World Editor Help Zone

Replies6Views2K

Jun 19, 2013
Razosh

Locked

 Cinematic Starting Problem


kids711

Mar 22, 2009

World Editor Help Zone

Replies16Views3K

Mar 27, 2009
kids711

Locked

 Cinematic Question!


reason 3

Oct 31, 2009

Triggers & Scripts

Replies16Views3K

Nov 5, 2009
ALiEN95

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