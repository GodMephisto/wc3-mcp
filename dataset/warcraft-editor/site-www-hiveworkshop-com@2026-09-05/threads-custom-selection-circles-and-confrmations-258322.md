---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/custom-selection-circles-and-confrmations.258322/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Custom Selection Circles and Confrmations
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/custom-selection-circles-and-confrmations.258322/>
#### Legal_Ease

#### Legal_Ease

 Level 20

JoinedFeb 8, 2013Messages1,476

Resources: 33

#### Resources
Icons 13Packs 1Skins 3Maps 11Templates 1Tutorials 4

NEW COLORS FOR SELECTION CIRCLES AND ORDER CONFIRMATIONS

Have you ever wanted to have a different color for the selection circles? While many models exist to change the texture of selection circles, no tutorial or thread I could find gave a clear answer about how this could be done. Here is my formula for changing the in-game colors of selection circles and target confirmations.

WHAT YOU WILL NEED:

WE or JNPG

notepad

pixlr.com or Photoshop

BlpLab (available in THW Tools)

1. First open notepad.

2. Then add the following text:

[SelectionCircle]

// Defines different textures to be used based on selection circle size.

// The Size## value defines the largest size which uses the given textures.

// Note that the last size does not need a Size## value since anything bigger

// than the next size down uses it.

NumSizes=3

Size00=100

Texture00=ReplaceableTextures\Selection\SelectionCircleSmall

TextureDotted00=ReplaceableTextures\Selection\SelectionCircleSmallDotted

Size01=250

Texture01=war3mapImported\BlankIcon.blp

TextureDotted01=war3mapImported\BlankIcon.blp

Size01=400

Texture02=ReplaceableTextures\Selection\SelectionCircleLarge

TextureDotted02=ReplaceableTextures\Selection\SelectionCircleLargeDotted

// This value is multiplied by the SLK scale field to get the circle size

ScaleFactor=72

// This value is added to the selection circle height

// when on walkable doodads.

// Note: this is also applied to shadow images

ImageWalkableZOffset=15

// Colors

ColorFriend=255,75,150,200

ColorNeutral=255,255,255,200

ColorEnemy=255,0,0,200

AllyOffset=0.2

[TargetingColors]

TargetLightColor=255,75,150,200

SelTargetColor=255,255,150,0

NOTE: This creates a light blue color, but feel free to adjust the (rgb) values at =x,x,x,x. Note: alpha, red, green, blue

3. The save as war3mapMisc.txt and import into your map and delete the war3imported/ prefix on the pathname.

TA-DA! Now you have a custom color for your target confirmation indicator.

Now for that pesky green selection circle. This will not be as easy. Some people might use a trigger to:

Game - Enable selection and deselection functionality (Disable selection circles)

 but this will remove every selection circle in the game. I only care about the Hero today, but you can use this for every unit if you want.

4. Now you need to make (or download) your new colored selection circle and import it and overwrite the pathname:

Abilities\Spells\Undead\VampiricAura\AuraRune6.blp​

5. Now create a dummy spell with no icon. I like to use Item Armor Bonus (+1) and give the the ability the target attachment vampiric aura.

6. Then trigger the selection circle to appear when a unit is selected. Here is a simplified example. You should create conditions for neutral and enemy units.

SelectionCircle
Events
Player - Player 1 (Red) Selects a unit

Player - Player 2 (Blue) Selects a unit

Player - Player 3 (Teal) Selects a unit

Player - Player 4 (Purple) Selects a unit

Player - Player 5 (Yellow) Selects a unit

Player - Player 6 (Orange) Selects a unit

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Owner of (Triggering unit)) Equal to (Triggering player)

((Triggering unit) is A Hero) Equal to True

Then - Actions
Unit - Add Item Armor Bonus (+1) to (Temp_Unit)

Else - Actions

SelectionCircle off
Events
Player - Player 1 (Red) Deselects a unit

Player - Player 2 (Blue) Deselects a unit

Player - Player 3 (Teal) Deselects a unit

Player - Player 4 (Purple) Deselects a unit

Player - Player 5 (Yellow) Deselects a unit

Player - Player 6 (Orange) Deselects a unit

Conditions
(Owner of (Triggering unit)) Equal to (Triggering player)

Actions
Unit - Remove Item Armor Bonus (+1) from (Triggering unit)

And there you have it. Now I realize there are much cleaner and probably better ways to do this. I was just shocked that there was almost nothing on this topic so I wanted to share what I learned. Also, the GUI above isn't quite right, I am not trying to give a complete GUI explanation. If you can code, then you can do it better than me. Also, if you know how to model, then you could make a better model. I am using vampiric aura, but certainly a better model would help. If someone wants to be kind enough to edit the selectioncircle.mdl and post it here with a new path for custom skins, well that would be fantastic. However, I tried and failed to create my own so this is what I have for now. Hope you all like it!


 Last edited: Oct 8, 2015

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Useful, simple, good looking.

Rep+, wonderful job.

However wont this interfere with the misc that we use for terraining? the one that improves the raise and lower tool. I also think it's better to add a special effect attached to the unit rather than adding an ability. It requires less work in the object editor that way.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

This was the method I was using for War3 Legends project, I used an animated selection circle. It worked quite well.

#### Legal_Ease

#### Legal_Ease

 Level 20

JoinedFeb 8, 2013Messages1,476

Resources: 33

#### Resources
Icons 13Packs 1Skins 3Maps 11Templates 1Tutorials 4

Chaosy said:

 Useful, simple, good looking.


Rep+, wonderful job.


However wont this interfere with the misc that we use for terraining? the one that improves the raise and lower tool. I also think it's better to add a special effect attached to the unit rather than adding an ability. It requires less work in the object editor that way.


Click to expand...


Yes, it could interfere with other game interface constants. What I did was I exported my changes and then used a complete war3mapMisc.txt file from my map. I can explain how to do that if necessary.


Special effects didn't work very well because when you destroy - special effect, it plays the death animation and it takes about 2 seconds for the aura to disappear. Using add/remove ability makes it turn off fast.


Thanks for checking this out. Honestly, I am not happy with my method. Sure it looks kinda cool, but I know there is a better way. If someone could create a simple template model that allowed for team colors, there might be a way to really make this look wonderful. For now, all I have is this trick.

#### Sunreaver

#### Sunreaver

 Level 18

JoinedAug 22, 2013Messages1,290

Resources: 16

#### Resources
Models 2Packs 1Maps 13

This is fantastic and great

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Wait what? The circle got a death animation?

#### Legal_Ease

#### Legal_Ease

 Level 20

JoinedFeb 8, 2013Messages1,476

Resources: 33

#### Resources
Icons 13Packs 1Skins 3Maps 11Templates 1Tutorials 4

Chaosy said:

 Wait what? The circle got a death animation?


Click to expand...


No no, the aura I used to replace it does. I might be editing this in a second. I've learned something new.


EDIT: I mixed up the rgba. I fixed it. For whatever reason, in Misc.txt it goes Alpha, red, green, blue ... so it's a,r,g,b.


 Last edited: Oct 15, 2014

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Cool tutorial, only thing better would be a team colored mdx to represent the circle like you said. =)

Thanks for sharing this information.

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Dat-C3 said:

 Cool tutorial, only thing better would be a team colored mdx to represent the circle like you said. =)


Thanks for sharing this information.


Click to expand...


Thats possible with custom selection model. But you can't do that on actual game-draw-en circles.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

 Have you ever wanted to have a different color for the selection circles?


Click to expand...

No. But this is a nice and cool tuts. And rep+ for that nice screenie

#### Legal_Ease

#### Legal_Ease

 Level 20

JoinedFeb 8, 2013Messages1,476

Resources: 33

#### Resources
Icons 13Packs 1Skins 3Maps 11Templates 1Tutorials 4

Dat-C3 said:

 Cool tutorial, only thing better would be a team colored mdx to represent the circle like you said. =)


Thanks for sharing this information.


Click to expand...


Maybe, someone will make a nice team-colored selection circle model that allows you to attach custom skin to it... hint hint.


They might even add it to the pastebin and link it here to this tutorial ...


 Last edited: Oct 15, 2014

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Legal_Ease said:

 Maybe, someone will make a nice team-colored selection circle model that allows you to attach custom skin to it... hint hint.

They might even add it to the pastebin and link it here to this tutorial ...


Click to expand...

You cant exactly do that, since team color is a square as a texture. Your selection must be a square to do that. If you make a selection circle team colored it will have outlined box edges and it would look weird...

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Can you thoroughly explain the difference between "green selection circle" and "target confirmation"? It doesn't need to be in the tutorial itself, you can just explain it in a post. I was a bit confused as to what they are referring to exactly.

#### Legal_Ease

#### Legal_Ease

 Level 20

JoinedFeb 8, 2013Messages1,476

Resources: 33

#### Resources
Icons 13Packs 1Skins 3Maps 11Templates 1Tutorials 4

"Selection Circle" refers to the green circle around the unit's feet that indicates that the unit has been selected. A "Confirmation" refers to a target point on the terrain, which is the target of a move order or the target of a spell that allows you to target the ground. Both are show in the picture I posted in the tutorial being light blue.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Won't the selection circle model through Item Armor Bonus be visible to all players?

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Yes, they will be visible.

However, attaching them via local Special Effect (which is of course, more complicated) can fix that.

#### rulerofiron99

#### rulerofiron99

 Level 25

JoinedJul 10, 2006Messages3,315

Resources: 20

#### Resources
Maps 4Spells and Systems 14Tutorials 2

Rather use dummy units and get local player to show/hide.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Daffa the Mage said:

 Yes, they will be visible.


However, attaching them via local Special Effect (which is of course, more complicated) can fix that.


Click to expand...


This tutorial should probably mention that then. Otherwise this will be limited to single player.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

rulerofiron99 said:

 Rather use dummy units and get local player to show/hide.


Click to expand...


I presume it could desync.

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


Legal_Ease, please attach the image to the tutorial instead of using an external site for it.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I like this tutorial overall, but it is pretty limited if it only works in single-player. I'm going to move it to the graveyard for now since my last q wasn't addressed, but feel free to PM me if you'd like me to move it back and make changes.

 You must log in or register to reply here.


### Similar threads

 Basic Click Hero Chooser -Not Created By me-


MapperForLife

Jun 12, 2007

Tutorial Archive

Replies8Views2K

Jul 13, 2007
Wolverabid

[GUI]
 Scrolling hero selction


mj1994

Oct 31, 2009

Tutorial Archive

Replies0Views1K

Oct 31, 2009
mj1994

Locked

[Trigger]
 Unit selection Marker


CodeBlack

Mar 19, 2018

Triggers & Scripts

Replies6Views2K

Mar 21, 2018
_Guhun_

Locked

 Refreshing Selection without releasing trigger?


Pinzu

Jan 14, 2019

Triggers & Scripts

Replies8Views2K

Jan 15, 2019
IcemanBo

Locked

 Lock Selection for specific Players


CroMoX

Aug 15, 2015

World Editor Help Zone

Replies4Views2K

Aug 15, 2015
CroMoX

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top