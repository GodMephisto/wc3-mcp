---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/world-editor-tips.5899/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# World Editor Tips
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/world-editor-tips.5899/>
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


Introduction


This is a collection of several frequently asked questions and short tutorials about performing different tasks in the World Editor. This FAQ is by no means definitive; it serves as a basic reference and should assist mapmakers of all skill levels.

Object Editor

Speed up Object Editor


Open Terrain Editor, Click the menu called Window, then disable the brushlist.


NOTE: This makes the Object Editor up to 10 times faster.

Copy objects (doodads, units etc) from map to map


Right-click the object you want to move, press Copy, then open the new map, right-click and press paste.


NOTE: Some units custom abilities may be messed up, just reselect it's abilities. This wont delete any of your objects.

Make new heros


Pick a hero (paladin, warden etc.), rightclick and press copy, then rightclick again and press paste, then edit the new heros stats as you like.


NOTE: This is the only way to make a new hero.

Heros above level 10


Open the Terrain Editor and click Advanced, then Gameplay Constants, then find Hero Maximum Level and edit it to what you feel like.


NOTE: Maximum is 10,000.

Heros creeping above level 5


Open the Terrain Editor and click Advanced, then Gameplay Constants, then find Hero XP Gained - Creep Reduction Table and remove the last value 0.

Viewing raw object name


If you need an objects raw name, hit CTRL + D to see it.

Autofill information


When making skills with many levels it's good to rightclick on the first level and click Auto Fill Levels and set how you want it to be.

Terrain Editor

Placing Objects without Collision-size


Hold down SHIFT key and place the doodads.

Changing doodads height


CTRL + PgUp for moving the doodad Up.


CTRL + PgDn for moving the doodad Down.


NOTE: this is very useful for placing bridges correctly.

Rotate an object


Select the object you want to rotate, hold down CTRL, then left-click on the terrain in front of the object where you want it to point; or you may use the keyboard number pad keys / and * to rotate objects.


NOTE: You can only rotate some objects.

Placing doodads close


Hold down Shift and place your doodads, this will make them ignore collision size.

Controlling camera


Zooming: Use the scrolling wheel to zoom in and out.


Changing angle: Hold down CTRL and hold down rightclick while you rotate camera in any direction with the mouse.


When zooming far out, black mask will appear, hold down CTRL and use the scrolling wheel to make it disappear.

Different views


x change to letterbox view.


v change to game view.


c change to camera view.

Selecting / deselecting objects


Hit SPACE to select the last selected object and again to deselect it.

Quick move objects or regions


Hold down ALT and click wherever you want to move the selected thing.

Speed up terrain editor


When working with many doodads / units in the Terrain Editor, it's a good idea to hit i to turn off player start locations.

Select ALL objects


CTRL + A selects all objects in Terrain Editor According to which palette you use.

General Editor


 VergilThazaar said:



 Ctrl + Z = Undo (go back / remove the last action)


Ctrl + Y = Redo (go forward / recover the last action)


Click to expand...

Easy backup


If your map has been corrupt, go look in the Maps/Test folder, there is always a backup of the last time you tested.

Disable auto-save


File/Preferences/General and uncheck Autosave.


NOTE: Disabling autosave is usually a good idea, it can often be annoying and can also ruin your map if you do something wrong and it suddenly autosaves.

Taking screenshots


Hit PrintScreen (just over the Insert button) to take a screenshot.


NOTE: Screenshots will be saved in the Warcraft 3/Screenshots Directory.

Trigger Editor

Remove starting units


Remove the Action "Melee Game - Create starting units (for all players)" from the trigger "Melee Initialization".

Remove starting resources


Remove the Action "Melee Game - Set starting resources (for all players)" from the trigger "Melee Initialization".


 Last edited by a moderator: May 4, 2007

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

An Intruder!

So, that, the brushlist is the shit that caused hero copying so loooooooooooong. Its 14798/3+135613575642-5.256+1365789*465297-5+4567893456345X FASTER!

 ...and yes... Dont give me ban because of exclamations! I've started WE Obj Editor tutor for n00bs... Just Kidding... BEGGINERS!


 Last edited by a moderator: Nov 3, 2006

#### VergilThazaar

#### VergilThazaar

 Level 10

JoinedOct 22, 2006Messages599

Resources: 5

#### Resources
Maps 5

Very Good, but you forgot the most important and usefull keys.


Ctrl + Z = Undo (go back / remove the last action)


Ctrl + Y = Redo (go forward / recover the last action)


*edit by Wolve*


Thanks VergilThazaar!


I included your addition. In a few days, I'll delete this post (your post count will not be reduced).


 Last edited by a moderator: May 4, 2007

#### myydraal

#### myydraal

 Level 3

JoinedMay 1, 2007Messages29

Reopen Tool Palette

This is really important, I think. If you accidentally closed the Tool Palette for editing Terrain, Doodads, regions, etc. go to


Window --> New Palette --> Terrain.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Wow, this can be extremely useful to people new to the editor, gj! +rep

Ralle said:

Taking screenshots

Hit PrintScreen (just over the Insert button) to take a screenshot.

NOTE: Screenshots will be saved in the Warcraft 3/Screenshots Directory.


Click to expand...

That isn't entirely true though. Not all screenshots are saved into the directory unless it is a picture of the terrain. I strongly suggest that you include that when you are taking screenshots of something such as the object editor/trigger editor, it will not say screen captured. You must click CTRL + C then open an art program such as Photoshop, Ifranview, Paint Shop Pro, even Paint and click CTRL + V then crop it and save it as JPEG or something that takes up less space. This has been troubling many people before because many times I hear people saying "Sorry, I can't take screenshots" though they can, but they don't know how. :emote_smile:


 myydraal said:



 This is really important, I think. If you accidentally closed the Tool Palette for editing Terrain, Doodads, regions, etc. go to

Window --> New Palette --> Terrain.


Click to expand...

You can also go to Layer | Terrain, though sometimes you can't always use that.


 Last edited: Jul 8, 2014

#### Herio-san

#### Herio-san

 Level 13

JoinedAug 31, 2005Messages823

Resources: 14

#### Resources
Models 3Maps 9Tutorials 2

Great tutorial for beginners. Something I use that you left out. If you want to speed up the load time on WE you can go under File -> Preferences and uncheck 'Create New Map on Start-Up'.


Also, you didnt say anything about enabling negative values in the WE which allows you to create all sorts of crazy abilities and rotate doodads along the z axis.

#### VergilThazaar

#### VergilThazaar

 Level 10

JoinedOct 22, 2006Messages599

Resources: 5

#### Resources
Maps 5

Ehm... Why would the post be deleted?

#### KingHeartless

K

#### KingHeartless

Ty for these tips! (even though i knew most except for how to make my object editor faster) +rep for u!

#### lordolly

#### lordolly

 Level 2

JoinedSep 5, 2007Messages14

WC3 World Editor Icons?

hey im sure this is relevent but i want to find out how to change the small icons like the gold lumber food upkeep Icons and the small icons on the units next to the DMG and Armor, if anyone know how to change them can you please give mke a reply

#### lordolly

#### lordolly

 Level 2

JoinedSep 5, 2007Messages14

also seeing people saying they close the Pallets you can just press the U,T,D keys to bring up unit terrain doodads pallet ^^

#### Bubba Ex

#### Bubba Ex

 Level 5

JoinedAug 16, 2007Messages149

@ lordolly to change the food/lumber/gold icons (can change text too) go to advanced/game interface, check the use custom game interface checkbox then scroll down to Icon-Food, Icon-Gold or Icon-Lumber and change them to what you want. Theres loads more in there that you can change to enhance the game interface besides just those.

#### Super-Sheep

#### Super-Sheep

 Level 20

JoinedOct 21, 2006Messages3,224

Resources: 2

#### Resources
Maps 2

lol

Thanks for Ctrl+Page up/down.

#### PA3OPx2

#### PA3OPx2

 Level 5

JoinedNov 1, 2007Messages26

Resources: 2

#### Resources
Skins 1Spells and Systems 1

how to sett hero limits

#### Slingran

S

#### Slingran

How to make items stack?

Hello, my question: I want to find out how to get items to stack like it does in DotA, for example: if you buy a healing pot (x1), then you buy another one it will instead of be one and one (fill 2 slots) be 2 in one (fill one slot) .


Short version  : I want to know how to make x2 of an item stack to x1 with 2 charges if you know what I mean. It's a trigger, isn't it?


If you have time please mail me the answer at "[email protected]"


Thank you!

#### forzamoto

#### forzamoto

 Level 9

JoinedApr 14, 2008Messages191

Resources: 13

#### Resources
Maps 8Tutorials 5

Try looking for ( Item stacking of DOOM )

#### TheCheat

#### TheCheat

 Level 2

JoinedJun 15, 2007Messages30

That is not the only way to make a new hero.

#### Squiggy

#### Squiggy

 Level 28

JoinedMar 25, 2008Messages2,954

Resources: 24

#### Resources
Maps 3Spells and Systems 20Tutorials 1

TheCheat said:

 That is not the only way to make a new hero.


Click to expand...

Thanks for correcting a two-year old tutorial

Sure you can just create a new hero by clicking Make a new unit -> Choose Hero -> yay

#### TheCheat

#### TheCheat

 Level 2

JoinedJun 15, 2007Messages30

I was just browsing... I dunno.

(btw) 3rd way: click on the hero you want/click create new hero/type the hero name (not proper name) and ok.

Oh and sorry if I came off like an ass commenting on a 2 year old tutorial.

#### cHeRoL

#### cHeRoL

 Level 11

JoinedMar 30, 2008Messages650

Resources: 5

#### Resources
Models 1Maps 2Spells and Systems 2

I like them and useful for begginers .

#### NinjasDontFight

#### NinjasDontFight

 Level 10

JoinedApr 13, 2008Messages711

Resources: 2

#### Resources
Tutorials 2

Out of curiousty, wasn't something similiar like this being made a long time ago? By you?

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

TheCheat said:

 That is not the only way to make a new hero.


Click to expand...

Yeah, he's right! You can create a custom hero by hitting the New Custom Unit button and then choose an existing hero and not just by selecting a hero then copy paste

Also using the New Custom Unit button lets you change the hero name (not proper name!) before creating the unit while if you copy paste you should change the name directly in the unit property.

I think this is also the same way that Squiggy and TheCheat is saying.

PS: For those who don't know what proper names are here's an example:

Hero Name: Lich

Proper Name: Kel'Thuzad

#### Blueman127

B

#### Blueman127

How do i protect my map when its done?

#### RaijinSpark

#### RaijinSpark

 Level 19

JoinedAug 31, 2008Messages1,774

Resources: 24

#### Resources
Models 1Icons 19Tools 1Maps 3

 + Z = Undo (go back / remove the last action)


Ctrl + Y = Redo (go forward / recover the last action)


Click to expand...


pretty noobish some terrainers know more <.<

#### TheDreadlord

#### TheDreadlord

 Level 4

JoinedJul 29, 2009Messages93

Good tutorial i already got a considerable experience of the WE but still i found some good advices in this post  Good job and thanks.

#### Luis Antony

#### Luis Antony

 Level 1

JoinedOct 24, 2013Messages8

he forgot how to pitch all roll in less


doing the steps (well is short but I goes well):


-Go to object editor


-Go to editor - all pitch roll and press


-press - with sustained shift


, then press the number (no shift)


, then press the button located on the right of the p


with shift held (not drop), continue holding until you press enter.

#### Dark_Fuse

#### Dark_Fuse

 Level 2

JoinedDec 29, 2014Messages19

Appreciate it mate; needa learn this shite so I can advance more quickly with my campaign! Keep it up.

#### your568473

#### your568473

 Level 4

JoinedSep 19, 2025Messages34

I'm learning to make my own map. It doesn't matter at first with everything since I'm just starting out. But when I wanted things bigger, I ran into trouble with the insurmountable basic limits that belonged to the object, there are limits like how much hp i can put on heroes and items. So I want to ask, what should I do to remove those limitations?

#### Ravager16829

#### Ravager16829

 Level 40

JoinedMay 14, 2021Messages2,455

your568473 said:

I'm learning to make my own map. It doesn't matter at first with everything since I'm just starting out. But when I wanted things bigger, I ran into trouble with the insurmountable basic limits that belonged to the object, there are limits like how much hp i can put on heroes and items. So I want to ask, what should I do to remove those limitations?

Click to expand...

I don't think it's possible to remove those HP limitations in World Editor, unless you have a third-party WEs like JNGP (which is unsupported in favor of the improved vanilla WE).

Also, it's usually better to make a new thread on WEHZ than to do a necro-post in such an old thread like this.

#### your568473

#### your568473

 Level 4

JoinedSep 19, 2025Messages34

Sorry, because I'm new to the community so I don't know much, while searching I found similar content so I thought to leave a question. And now back to the issue, according to your answer you mean there is no way or better to stop using the original WE and replace it with a third party tool. Actually your answer is a bit too short for me to come to a conclusion, and that's only partly because I've looked at other maps, those maps have a number of values up to 1000.000.000, although i can't edit those values because when i edit it it will go back to the limit value. I tried using triggers to increment the values but that was quite conservative and created many disadvantages and errors, so I gave up on using triggers to break through. I know it's going to be a hassle, but if you know please give me some suggestions

Ravager16829 said:

 I don't think it's possible to remove those HP limitations in World Editor, unless you have a third-party WEs like JNGP (which is unsupported in favor of the improved vanilla WE).

Also, it's usually better to make a new thread on WEHZ than to do a necro-post in such an old thread like this.


Click to expand...

 You must log in or register to reply here.


### Similar threads

 World Editor doodads and destrucibles hints


Archian

Apr 23, 2006

Misc/General Mapping Tutorials

Replies7Views41K

Jun 16, 2018
Wark

Locked

Sticky

 World Editor FAQ - Work in Progress


Ralle

Oct 26, 2006

Misc/General Mapping Tutorials

Replies0Views59K

Oct 26, 2006
Ralle

 iPeez's Complete Terraing Tutorial v1.0


iPeez

Jan 18, 2009

Misc/General Mapping Tutorials

Replies19Views20K

Mar 20, 2025
Filson

 Beginners F.A.Q To Terraining


iPeez

May 22, 2009

Misc/General Mapping Tutorials

Replies19Views16K

Nov 2, 2010
_StarScreaM_

S

Locked

 Rotating objects in editor


Sephalo

May 19, 2010

StarCraft II Modding

Replies4Views12K

May 20, 2010
Sephalo

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top