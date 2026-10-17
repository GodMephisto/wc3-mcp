---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/advanced-terraining-tutorial.203428/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Advanced Terraining Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/advanced-terraining-tutorial.203428/>
#### Blood Raven

#### Blood Raven

 Level 47

JoinedAug 1, 2010Messages1,740

Resources: 235

#### Resources
Models 144Icons 53Packs 7Skins 13Maps 16Templates 1Tutorials 1

 Hello there, terrainers! This tutorial reveals all points of the good terrain, which includes playable terrain and templates. For good terrain I will recommend you using The Ultimate Terraining Map (UTM).

And for better results and more freedom, use the JassNewGen World Editor.

If you don't know the basics of the World Editor, you should read This Tutorial.

 The playable terrain is terrain on which can be played. It can be used for almost all types of maps: RPGs, Campaigns, Arenas, Mazes, even for Melle maps and other types. I will show you the difference between the GOOD terrain and the BAD terrain. I will show you what to use and what not to use. Now, enough talking, lets get started.

A.Custom Cliffs.

I will recommend you that you never use the blizzard cliffs. They look ugly and bad. Now I will create a custom cliff, using the raise/lower tool and some tilesets. Dont forget about the pathing!

Note: you must have a file named MiscData in folder named UI in your WC3 folder. If you dont have UI folder, simply create it and download the MiscData file attached at the bottom of this tutorial. This will allow you to create infinite steep cliffs.

Note: If you already have the MiscData.txt in the "UI" folder, but it still doesn't allow you to make steep cliffs, open the MiscData.txt in Notepad or Wordpad, scroll down to the place where it says [Terrain]. below should be "Maximum slope in degrees". Below should be some number. Make it more than 86 and save it.

Step1: Use the raise/lower tool to raise the region in which you need the cliff.

Step2: Use some rocky tilesets on the cliff, so it looks more realistic.

Step3: You may use some doodads to enchant your cliff. Id recommend you to use rocks, shrubs and few trees if there is place. Dont forget to remove the pathing from the OE.

Step4: (optional) use the smooth tool to smoothen the terrain where you want it to be walkable.

Note: In some cases the Blizzard Clifs may look actually better, like when you're making cityscape terrain and such. However, they look too empty without doodads, dont forget about them

Note: The Blizz and the custom cliffs have a diffirent view range for the ground units. This can be a problem...

B. Rolling Shores

Placing water with blizzard cliffs looks ugly (as I said in A.) I will recommend you to place water, using the raise/lower tool. This is called rolling shores.

Note: You must have started the map with Water and used the Raise tool to make it land. If you havent used this method, it is hard to use rolling shores (you can also use doodad water, which can be found in the UTM).

Note: You can find more about the Rolling Shores Here.

Step1: Use the raise/lower tool to lower the terrain until you reach the water.

Step2: Use the Smoothen tool to make the shores smooth.

Step3: Change the tileset. Dirt and grassy dirt near the water, grass and think grass everywhere else (and rocks if there are custom cliffs)

Step4: Put some doodads like River rushes, some rocks, shrubs, lily pads, etc.

C. Smooth Terrain Tiles

Smooth terrain tiles are very important when doing natural terrain. It is looking bad if you put thick grass and next to it dirt. This is called sharp terrain and I dislike it. To do a smooth terrain, you will need to use the tiles in order. Something like:

 Cracked Dirt -> Dirt -> Grassy Dirt -> Grass -> Thick Grass

Note: the grassy dirt need to go over the grass and the thick grass (not the reverse), so it dont looks sharp. To do this go to Advanced > Modify Tileset. Then check the box Use custom Tileset and then move the grassy dirt right of the grass and thick grass.

Note: Don't use this method if you're making non-natural terrain, it may not look right.

Note: You can find more about the "Terrain Overgo" Here.

D. Doodad Manipulating

Doodads can be manipulated by 4 different ways: Raise/lower, Rotate, Size and Recolor.

a. Raise/lower

Raising/lowering the doodads is achieved very easily: just by pressing Ctrl+PageUp or Ctrl+PageDown. This will help you setting the doodads as you wish them, fitting the theme, hiding part of them under the ground or making them flying or put them on another doodad.

b. Rotate

Rotating a doodad is not that easy. It can be achieved only with the OE (Object Editor) by two ways: depending on the terrain and free rotate. Depending on the terrain is rotating that will follow the terrain lines. Increasing the Maximum Pitch Angle (degrees) and "Max Roll Angle (degrees)" allows the doodads to follow the terrain lines. It will look like that:

The Free rotate is harder. First of all go to File>Preferences and check the box saying: Allow negative real values in Object Editor. Then go to the OE and Shift-Click Max Roll Angle (degrees) to rotate the doodad you wish to. Do the same with the "Max Roll Angle (degrees), if you wish to.

-3.14 rotates the doodad by 180 degrees. By the way, -1.57 rotates it by 90 degrees and -0.79 rotates it by 45 degrees.

Note: After rotating the doodads, they will be buried in the ground. Simply raise them to see their full form. Also, the rotate depending on terrain will take effect too.

Note: You can find more about Doodad Rotating Here.

c. Size

Sizing a doodad is easy. Almost all blizzard doodads have a minimum size of 0.80 and maximum of 1.20. By changing these numbers on a placed doodad you can deform it. Like setting X and Y to 0.80 (0.8 x the normal size) and setting the Z (height) to 1.20, like that the doodad will look thinner and higher than normal. Also, you can change the normal size of the doodad with the OE by changing the Art- default Scale. The Default is 1.00, but you can make it larger or smaller. You can also change the limits of deforming a doodad (0.80 and 1.20) by changing the Editor Maximum scale and Editor Minimum scale. This will allow you to deform the doodad by your needs. You can also use the "Page Up" and "Page Down" keys to change the height and the "Home" and "End" for the X and Y.

Note: The X and Y (2Ds) depend on the rotation of the doodad, while the Z (height 3D) does NOT depend on anything.

d. Recolor (Tint)

Recoloring doodads (Also called Tinting doodads) is also simple. You just need to change the Art- tinting color. If the doodad have much variations you can recolor them different. Like Variation 1 Red, Variation 2- Blue, etc.

To recolor a doodad, you need to change the color variables (the Max is 255). For Example, if you leave Red to 255, and set the others to 0, the doodad will look all red. The following picture will show you the basic tint colors:

E: Creating Indoors

Creating Indoors isnt that easy. Indoor is inside a building, which can be castle, tower, orc fortress, cave or even a poor house. Most of the people make the building with Blizzard cliffs, which is more easier, however I dislike and I dont think a high quality terrain would have such indoor. The harder way to create an indoor is with doodads. The UTM (Ultimate Terraining Map) offers many types of walls (with windows, doors, columns) which can be used to create an indoor. Also, indoors need another tile. I would prefer using a doodad as tile too, but you must be very accurate to connect the tiles, as well as the walls. Every wall must be connected with a column, so the walls wont have cut edges, but make sure the column and the wall fit perfectly. Also the walls must NOT be too high, so they wont stay in the way of the camera. The following images will show you how to do nice indoor. The UTM contains all needed doodads.

F. Pathing ground

Pathing ground is the most important thing for the playable terrain. Pathing is there, where the Units can walk/fly/build and where they can't. There are more types pathing: Fly, Boundnary, ground, unbuildable. When you click P in the editor, it shows you the pathing in different colors.

The pathing can be achieved by three ways: Doodad pathing, Cliff Pathing and Pathing Blockers. The doodad pathing is when you place a doodad, you will see purple or blue pathing under it. You can always change that pathing from the OE. I would prefer to turn it off and do the pathing myself with blockers. In the UTM, the pathing of all doodads is off. The Cliff pathing is when you place blizzard cliffs. The Blizzard cliffs have their own pathing, but since I am using custom cliffs, I am doing it with pathing blockers. The pathing blockers are small doodads with click helper that dont allow the units to pass trough. There are different types of blockers: ground, fly, both, etc. They block different units from passing. There is a unique type blocker The line of sight blocker. The line of sight blocker blocks ground units too, but it dont allow units to see trough it. It is placed mainly on cliffs and indoor walls.

Note: You must put the blockers INSIDE the doodads/terrain you want to be unwalkable, not outside it, the image shows the diffirence:

Note:You can find more about Pathing Here.

G. To the Infinite and beyond

The JassNewGen World Editor offers more ways to edit maps and gives more freedom to your possibilities.

a. No limits

One of the priorities of the JNG WE is that it gives the possibility to put more than the limits for doodads, units, builings and so on (for example the limit of doodads is around 8500, of neutral buildings around 30 etc.) If you use the normal WE, if you put more than the limit, you can't save the map unitil you fix this. With the JNG WE you can actually. You just need to go to Grimoire>Enable no limits. You can find other useful things there as well. The only bad thing about the JNG WE is that you can't test the map from the "Test map" button, because the JNG Warcraft don't works.

b. Up to 16 Tilesets

Another Priority of the JassNewGen WE is, that you can use up to 16 Tilesets in one Map., while the normal WE allows you only up to 13 Tileset Spaces (not tilesets). You can use up to 16, no matter if the Tileset uses 1 or 2 spaces. Just go to Extensions>Edit Tileset. There you'l find a wide menu. Choose what you need by clicking on the tilesets you need and remove by clicking the selected ones.

Note: You must have saved the map at least once to open the menu.

Note: Be careful though. You can add Cliff Tiles, but you CANNOT remove them. If you need cliff tiles, go to Advanced>Modify Tileset and after all Tilesets there are "Cliff Base Tiles" - add them from there.

After you've finished with your choices, click "Ok". DO NOT save after that, it will save for you automatically. Then don't do anything and rerun the JassNewGen WE. And so here you have up to 16 Tilesets. You can arrange them way you want from Advanced>Modify Tileset with "Turn Left" and "Turn Right". You can also find more useful priorities in the Extensions Menu, such as change the tileset pathing and other.

Also, you can download the JassNewGen WorldEditor here.

 The Template Terrain is used to take a Screenshot of the terrain to show your skills or for making a Cinematic. The Template terrain dont needs any pathing. It must look good only from one view point, and bad from all other view points. This allows the use of fogs, effects, glows, and such other things that enchants your terrain with awesomeness.

A. Fog

The Fog is unique thing that allows you to cover the more distant areas in color. The Fog can be generated by Scenario>Map Options>Use Terrain Fog

After checking the box Use Terrain Fog some options will appear:

Start and End Distance: The start distance is on what distance of the screen the fog will start being more dense until it reaches the End distance, where it will be pure color (except the sky). I will recommend that you set the start distance to 0, so it wont be sharp. Density: leave it as it is, I dont see any diffirence. Color: the color of the fog. The colors are like tinting doodads. Example:

Note: The Fog can also be used in playable terrains!

Note: You can find more about the fog Here.

B. Sky

The sky is a picture that is put on the background in the palce of the black abyss when you click V or Ingame. There are much skies which can be found in Scenario>Map Preferences>Sky Display. You may also put custom sky by importing it. The sky is visible in the upper Picture.

Note: The Sky does NOT gets affected by the fog, if you havent put Fogged sky

Note: You can find out how to create custom skies Here.

C. Effects

The Effects a Unique Objects like Glows, Light Rays, Shadow Affecters, Stars, fires and others. They are often kept in the Cinematic selection is the doodad palette (as in the UTM). They can enchant your terrain greatly, but most of them cant be used in the playable terrain. The bad thing is that they cannot be tinted or just the most of them cannot. It isnt so easy to use effects because the most of them (except the fires) must be rotated, raised/lowered and so on. Here is example of Sun, made with effects:

I also used sky and fog to achieve this.

Note: The effects dont gets affected by the Fog.

D. The Mighty Alpha Tile"

The Mighty Alpha Tile" is a simple invisible tile. It is transparent and shows the sky under it and can be used for few things: Flying Castles and buildings or Water Reflection. It can also be used in Playable terrains, but its more often used in Templates. Creating flying objects in the sky is simple. Just paint everything in the MAT (Mighty Alpha Tile") and place some wall doodads and such things. Here is a example of it:

Note: You must put something under the castle, so it wont look flat. Here I put some rocks, rotated upside-down (look at rotating, Doodad Manipulating, b2)

The MAT" can also be used to achieve river reflection. You just need to put it in shore water with Rolling Shores (1B) and copy and paste the doodads, upside-down. Also, the use of sky here is very important, so it will look like reflection, not black hole in the water. Example:

Note: The Rotating upside-down can be done by the following way:

1. Make custom doodads in the Object Editor with the same models as the ones you wish to reflect.

2. Remove thier pathing if there is any and rotate them by 180 Degrees (look at Doodad Manipulating, b2). (Note: you must do them with the same doodad what it is made from)

3. Select all doodads you want to reflect and Copy them (Ctrl+C)

4. Go to Advanced>Replace Doodads and replace every doodad type with it's reflected version.

5. Paste the doodads on thier old places (Ctrl+V) (You must be accurate.)

Note: You can download the Mighty Alpha Tile at the bottom of this tutorial. I has the name "Outland_Abyss.blp" and is imported as any other normal tile. (128x128, not 128x256!)

Note: You can find more about Reflections Here.

Here you can find more tutorials for how to make other specific locations:

Jungle/Oasis

Forests

Path by the Sea

Orc Encampment

Natural Environments

Caves

Realistic Lava

Easy Waterfalls

Dungeon Enterance

Thanks for Reading! Hope you learned something. If I missed anything, feel free to tell me and I will add it. Happy terraining!

#### Attachments

MiscData.txt

 6.9 KB

 · Views: 4,151



Outland_Abyss.blp

 3.3 KB

 · Views: 1,689




 Last edited: Jul 11, 2012

#### Drunken_Jackal

#### Drunken_Jackal

 Level 19

JoinedApr 10, 2010Messages2,789

Resources: 3

#### Resources
Maps 1Spells and Systems 2

Very nice tutorial Blood Raven.

This will be helpful for many terrainers.

Hopes this gets approved.

Also, MAT seems a nice name for the Alpha Tile.

EDIT:

I suggest you change the font you use in your pics. Maybe Arial, Tahoma or Verdana. Something easily read.

#### Luorax

#### Luorax

 Level 16

JoinedAug 7, 2009Messages1,406

Resources: 2

#### Resources
Maps 1Tutorials 1

"Smooth Terrain Tiles" hell yea, that was something new for me. I don't think I'm a good terrainer, but I know a lot of things (I'd need some practice tho' in order to get better)

You deserve some rep for this tutorial

#### Amargaard

#### Amargaard

 Level 19

JoinedJun 29, 2009Messages6,759

Resources: 16

#### Resources
Icons 16

Awzum tutorial matey! I have only skimmed it, and spotted a few typos and stuff. I can help you point some of them out later, but right now I'm busy as f**k. +rep

#### Asip

#### Asip

 Level 6

JoinedNov 14, 2010Messages248

Resources: 1

#### Resources
Spells 1

Niceeeeeeeeee

#### HOooDy

#### HOooDy

 Level 4

JoinedJan 29, 2011Messages709

Resources: 5

#### Resources
Skins 1Maps 4

Nice one ! Not Bad !

#### Drunken_Jackal

#### Drunken_Jackal

 Level 19

JoinedApr 10, 2010Messages2,789

Resources: 3

#### Resources
Maps 1Spells and Systems 2

Also the reflection part really helped me.

#### Ilike

#### Ilike

 Level 8

JoinedDec 17, 2010Messages504

Awesome! o.o.. rep+

#### Painkiller91

#### Painkiller91

 Level 3

JoinedJan 19, 2009Messages27

Resources: 1

#### Resources
Models 1

Very nice, but I didn't understand this last section, with reflections. What is this MAT exactly, and how to use it?

#### Blood Raven

#### Blood Raven

 Level 47

JoinedAug 1, 2010Messages1,740

Resources: 235

#### Resources
Models 144Icons 53Packs 7Skins 13Maps 16Templates 1Tutorials 1

MAT is the Mighty Alpha Tile (attached to the tutorial) it is used as ordinaly tile and is invisible... To make reflections, cover the water in the Mighty Alpha Tile and rotate the doodads upside down, thats it

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

Compliments on covering so many aspects, but hasn't all this been covered already? I seem to recall seeing all this material before.

For instance the links provided in: https://www.hiveworkshop.com/forums...e-terraining-hives-terraining-section-184427/

Anyhow, my real issue lies in the sense that you're refering to it as advanced terraining. If you apply the term advanced I would expect it to cover a) more than the fundamentals, or/and b) an in depth coverage of certain fundamentals. This seems to be an even mixture of both, which I think is, aaaah, inauspicious in the sense that it might be confusing. For instance, reflections and skies are certainly advanced procedures, but you're not justifying them by just briefly touching the obvious. You have to indulge your readers, at least to the extent where there aren't any unattached strings [1].

 The MAT™ can also be used to achieve river reflection. You just need to put it in shore water with Rolling Shores (1B) and copy and paste the doodads, upside-down. Also, the use of sky here is very important, so it will look like reflection, not black hole in the water. Example:

Click to expand...

This is rather misleading (refer to bold text). It seems to suggest that you can flip doodads in the process of the copy/paste procedure. In fact, I really can't tell what you're trying to impart with this sentence. Case in point, such in depth procedures as reflections are refered to as "advanced" because they require a certain level of elaborate and interactive instructions. This doesn't really suffice as far as I'm concerned, I mean you hardly even mention that you have to use the object editor.

In general there are so many aspects you forgot to consider or neglected that I hardly think people can derive any relevant source of knowledge from this particular paragraph.

B. Sky

The sky is a “picture” that is put on the background in the palce of the black abyss when you click “V” or Ingame. There are much skies which can be found in Scenario>Map Preferences>Sky Display. You may also put custom sky by importing it. The sky is visible in the upper Picture.

Note: The Sky does NOT gets affected by the fog, if you haven’t put “Fogged sky”


Click to expand...

Certainly, you do manage to impart some relevant information. But again you neglect several aspects (example) and again I really don't feel like this constitutes advanced terraining. I mean for instance you don't go to mention how you import and use a custom sky (what I would refer to as an unattached string [1]). Either link to existing material or step up to the plate and elaborate.

A.Custom Cliffs.

I will recommend you that you never use the blizzard cliffs. They look ugly and bad. Now I will create a custom cliff, using the raise/lower tool and some tilesets. Don’t forget about the pathing!

Click to expand...

Might be a minor flaw, but there are several floating rocks in both examples. Plus you indirectly contradict point 1 c. But that's just me being picky mind you, at least you demonstrated the aspect which was meant to be demonstrated. Call it, aaaah, conveying a certain level of professionalism... Yes, do refer to it as such.

I guess what I'm trying to say is, either focus on several aspects which are lightly elaborated (note: no unattached strings) and call it a beginner's guide, or make sure you actually cover all the necessary aspects to call it advanced. I would mainly suggest the former, because the latter would take you a while to accomplish. At least if you want to do things properly and thoroughly. Oh and don't get the wrong idea, I'm not trying to question your integrity as much as I want you to (re)consider certain aspects which you neglected or didn't entirely justify. In general you've done a fine job, and I hope you find this post constructive.

-


 Last edited: Sep 19, 2011

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Nice one. You could mention the easy shortcuts: Page Up and Page Down, as well as Home and End - for scaling doodads.

#### Dalsie

D

#### Dalsie

Wow... The difference on the custom cliffs and rolling shores are astounding.

Thanks for this, il be putting it into practice

#### tobyfat50

#### tobyfat50

 Level 17

JoinedSep 19, 2011Messages829

Resources: 6

#### Resources
Models 4Maps 1Tutorials 1

Nicely done!

i found a different way to make water reflections!

Check my map out (no reflections yet

):https://www.hiveworkshop.com/forums/map-development-202/asian-garden-204004/

#### strach195

#### strach195

 Level 2

JoinedSep 3, 2011Messages17

Great tut, really helped me while im now terraining my map, also ur pics really inspirated me . +rep

#### vuongkkk

#### vuongkkk

 Level 5

JoinedOct 13, 2010Messages84

Resources: 2

#### Resources
Maps 1JASS 1

I like fladdermasken's comment, it's the sample of a good advice and enough with me to make it to become a tutorial )

Blood Raven

I think it's good for you, dont make it so hard, man

And, i like how simple you wrote a advance tut !

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

I never knew that the tile order changes the way they overlap... nice

and I wonder why Blizzard called that line Max Roll Angle (degress) when it takes radians...

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Blizzard said that the Warcraft III will be actively developed and updated, but then... Meh

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Sorry for the very late reply. Nice tutorial, very useful information to those who want to create better-than-average terrains.

However, the images must be attached to the hive rather than imageshack. Also, I recommend that you use some of the colors described here: https://www.hiveworkshop.com/forums...t-tutorial-please-read-before-posting-119320/

Instead of the red and blue, as they are a bit too vibrant. =)

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Actually blizzard cliffs can look better than custom ones in some cases, if you want to create underwater cannon, blizzard cliff with some shadow effect can look 100 times more cooler than any custom cliff (Don't put models into this).

#### Blood Raven

#### Blood Raven

 Level 47

JoinedAug 1, 2010Messages1,740

Resources: 235

#### Resources
Models 144Icons 53Packs 7Skins 13Maps 16Templates 1Tutorials 1

Yes, indeed they can look better in some situations, really, thats why im using the blizz cliffs in my project, but im overgrowing them with many doodads like rocks and grasses etc. If it is only blizz cliff, without doodads, it looks bad. In the most of the cases the custom cliffs look better, thats why i reccomend them

And about the pics, i will upload them to hive alblum, not in imageshack, thanks  i will also try to improve the tutorial if i can

Also, there can be some grammar mistakes, im not from english-speaking country, so if you want fix them

EDIT: Reuploaded the pictures to hive and changed the img paths

anything else to fix?

EDIT2: Recently i learned how to use up to 16 tilesets in one map, i will add it


 Last edited: Dec 5, 2011

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

While Flad brings up really good points about this (especially the naming of the tutorial), I think I'm going to go ahead and approve it. It follows the guidelines for the tutorial, and many seem to have benefited off of it. Whether or not it is advanced is subjective so I don't think I should judge upon that.

Some of the grammar/explanations are off, but it is still readable and understandable.

~Approved. =)

#### Blood Raven

#### Blood Raven

 Level 47

JoinedAug 1, 2010Messages1,740

Resources: 235

#### Resources
Models 144Icons 53Packs 7Skins 13Maps 16Templates 1Tutorials 1

So, the Tutorial has been updated, fixed little grammar, added links to JassNewGen WorldEditor, Added "how to rotate the doodads upside-down" in the reflecions part, and finaly, added the "To the Infinite and Beyond" part, which explains the main epic functions of the JassNewGen WE.

Enjoy!

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Hmm, I don't agree with some of your recommendations.


"Sharp" tile variation can be very effective and realistic, if not entirely necessary for good looking terrain, if done properly. Smooth looks forced.


Also, for the custom cliffs part, you might want to make use of large rock doodads to give the cliff a little more shape, or at least take the small ones you placed and lower them so they aren't floating.

#### SA Dashie

#### SA Dashie

 Level 22

JoinedApr 3, 2010Messages1,889

Resources: 17

#### Resources
Maps 11Spells and Systems 3Templates 1User Interface 2

 "Sharp" tile variation can be very effective and realistic, if not entirely necessary for good looking terrain, if done properly. Smooth looks forced.


Click to expand...

I was going to say something like this so therefore I also agree. Sharp tile organization can look good on a terrain if done well as for example with a map of mine that I did some time ago when terraining with default blizzard terrain functions(doodads , tile setting ect).

#### Sin'dorei300

#### Sin'dorei300

 Level 50

JoinedAug 20, 2011Messages4,120

Resources: 210

#### Resources
Models 87Icons 108Packs 6Skins 1Maps 2Spells and Systems 1Templates 1User Interface 3Tutorials 1

Awesome tutorial !

#### patsiderenios

P

#### patsiderenios

That was what i needed!Thank you very much!Ι have revised my thoughts on terraining!

#### Hellx-Magnus

#### Hellx-Magnus

 Level 14

JoinedJun 10, 2010Messages71

Resources: 17

#### Resources
Icons 17

tilesets

I wanted to try out JassNewGen WE for the mentioned 16 tilesets thing and I can see the new menus and options, but adding more tiles doesn't seem to work. After I'd chosen tiles and pressed OK, saving happens but nothing else, my additional tiles are nowhere. Should this even work with 1.26a? My game is not cracked nor using any other modifications.

#### Blood Raven

#### Blood Raven

 Level 47

JoinedAug 1, 2010Messages1,740

Resources: 235

#### Resources
Models 144Icons 53Packs 7Skins 13Maps 16Templates 1Tutorials 1

rerun the WE without saving and open the map in jass new gen we again, and you should have all these you've chosen.... haven't i wrote it?

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Here's what Blood Raven said in a step by step manner

1)Open NewGenWE

2)Open your map

3)Open Edit Tileset

4)Pick your tiles

5)Click OK

6)Close NewGenWE (WITHOUT HITTING THE SAVE BUTTON)

7)Reopen NewGenWE

8)Open your map

#### Hellx-Magnus

#### Hellx-Magnus

 Level 14

JoinedJun 10, 2010Messages71

Resources: 17

#### Resources
Icons 17

Yeah I figured it out in meanwhile, I was being hasty.. Sorry and thanks for the answers guys!

#### Blood Raven

#### Blood Raven

 Level 47

JoinedAug 1, 2010Messages1,740

Resources: 235

#### Resources
Models 144Icons 53Packs 7Skins 13Maps 16Templates 1Tutorials 1

Updated the Tutorial hardly and put links to other tutorials as well, hope this can be of use

Enjoy

Also, no problem, Hellx-Magnus, it can happen to anyone

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

You can add links to other similar tutorials, to help users browse and learn more quickly. You can also remove those hidden tags and add small previews of images which will enlarge upon click, like in this tutorial: https://www.hiveworkshop.com/forums/general-mapping-tutorials-278/aesthetics-33717/

It can also be a good idea to make those notes merged into one, if possible. It's rather unusual to see two notes, one after another, which can be merged. And please recolor those Dark Blue text chapters, they're very difficult to be red. You can use one of color sets I provided in my other tutorial, in here: https://www.hiveworkshop.com/forums...ptions-creation-process-207782/#Sixth Chapter

Looking Forward to your update.

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

I'd use these.


 Pyritie said:



 Good colors you could use:

#D0F680

#60A600

#CCAA00

Click to expand...

 - https://www.hiveworkshop.com/forums...t-tutorial-please-read-before-posting-119320/

#### DragonVein

D

#### DragonVein

Thanks,... helped me so much

#### Foronisus

#### Foronisus

 Level 11

JoinedJul 5, 2010Messages779

Resources: 4

#### Resources
Maps 3Spells 1

this tutorial is really helpful  one more question where to find Alpha tile or what was that for flying rocks etc...

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

just put Alpha tile on the search bar, its on some thread here...

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

Or...


(one more reply etc...)


...navigate to the bottom of the first post where attachments usually latch onto.

#### LAYDK

#### LAYDK

 Level 2

JoinedMar 30, 2012Messages28

Lol'z..

This tutorial should just be on Basic Terrainiing.. Not Advanced....

#### HFR

#### HFR

 Level 22

JoinedApr 12, 2008Messages3,381

Resources: 7

#### Resources
Icons 1Packs 1Maps 5

Go away.

#### Blood Raven

#### Blood Raven

 Level 47

JoinedAug 1, 2010Messages1,740

Resources: 235

#### Resources
Models 144Icons 53Packs 7Skins 13Maps 16Templates 1Tutorials 1

LAYDK, well, would you use these methods in basic terrains? i don't think so. Maybe some of them are a bit easy and basic, yes, like rolling shores and such, but the other things are only for advanced terrains. I just want to explain everything step by step so there won't be any hows and whys.

And to say for some other people. the alpha tile and Misc Data are attached at the bottom of the tutorials where you can download them. (the alpha tile is under the name Outland Abyss.blp)


 Last edited: Apr 10, 2012

#### Gary0607

G

#### Gary0607

lol cool

#### Mgeterno11

#### Mgeterno11

 Level 15

JoinedJan 12, 2011Messages1,196

Resources: 4

#### Resources
Icons 2Skins 1Maps 1

Nicely done and well understandable tutorial. It helped me a lot about terraining a map.

#### Justin Weaver

#### Justin Weaver

 Level 4

JoinedMar 1, 2014Messages68

Resources: 1

#### Resources
Spells 1

Dalsie said:

 Wow... The difference on the custom cliffs and rolling shores are astounding.


Thanks for this, il be putting it into practice

Click to expand...


Is it OUTSTANDING ?  haha LoL

#### CreatorD3292

#### CreatorD3292

 Level 15

JoinedAug 23, 2013Messages285

Resources: 29

#### Resources
Models 20Icons 9

 b. Up to 16 Tilesets


Another Priority of the JassNewGen WE is, that you can use up to 16 Tilesets in one Map., while the normal WE allows you only up to 13 Tileset Spaces (not tilesets). You can use up to 16, no matter if the Tileset uses 1 or 2 spaces. Just go to Extensions>Edit Tileset. There you'l find a wide menu. Choose what you need by clicking on the tilesets you need and remove by clicking the selected ones.


Picture


Note: You must have saved the map at least once to open the menu.


Note: Be careful though. You can add Cliff Tiles, but you CANNOT remove them. If you need cliff tiles, go to Advanced>Modify Tileset and after all Tilesets there are "Cliff Base Tiles" - add them from there.


After you've finished with your choices, click "Ok". DO NOT save after that, it will save for you automatically. Then don't do anything and rerun the JassNewGen WE. And so here you have up to 16 Tilesets. You can arrange them way you want from Advanced>Modify Tileset with "Turn Left" and "Turn Right". You can also find more useful priorities in the Extensions Menu, such as change the tileset pathing and other.


Also, you can download the JassNewGen WorldEditor here.


Click to expand...


I try this many time but it's not working...

#### IncredibleMan

#### IncredibleMan

 Level 3

JoinedJun 26, 2015Messages7

Resources: 3

#### Resources
Maps 3

Sweet now i can make my terraining more epic!

#### mysteriis

#### mysteriis

 Level 4

JoinedMay 2, 2011Messages40

Resources: 1

#### Resources
Maps 1

Could you update the pictures on the post? Thanks.

#### KOYK GR

#### KOYK GR

 Level 5

JoinedMay 8, 2009Messages88

Resources: 1

#### Resources
Maps 1

re upload the photos?

#### L4wless

#### L4wless

 Level 1

JoinedJul 15, 2017Messages6

fakt!!! dis is da best tutorial i've ever seen!!!!! and it'll help me in my terraining skills!!!!

#### Cake is a Lie

#### Cake is a Lie

 Level 4

JoinedOct 1, 2017Messages96

What path do I write to make this "Misc Data.txt" work?

 You must log in or register to reply here.


### Similar threads

 Beginners F.A.Q To Terraining


iPeez

May 22, 2009

Misc/General Mapping Tutorials

Replies19Views16K

Nov 2, 2010
_StarScreaM_

S

 iPeez's Complete Terraing Tutorial v1.0


iPeez

Jan 18, 2009

Misc/General Mapping Tutorials

Replies19Views20K

Mar 20, 2025
Filson

 Terrain: Advanced Lighting


~Void~

Feb 13, 2008

Misc/General Mapping Tutorials

Replies47Views32K

Jul 25, 2011
Skycraft

 How to Make Easy Waterfalls


RoboHippo

Nov 10, 2009

Misc/General Mapping Tutorials

Replies13Views20K

Jul 18, 2015
The Nightraven

 Terrain: Playable Forests/Villages


Bugbuster96

Jul 16, 2010

Misc/General Mapping Tutorials

Replies6Views13K

Feb 14, 2011
Orcnet

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