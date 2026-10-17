---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/geosetanims-fix-those-randomly-appearing-corpses.8930/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Geosetanims Fix Those Randomly Appearing Corpses
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/geosetanims-fix-those-randomly-appearing-corpses.8930/>
#### Horn

#### Horn

 Level 15

JoinedMar 31, 2004Messages860

Resources: 20

#### Resources
Models 14Icons 1Skins 3Tutorials 2

This tutorial assumes you know how to convert an mdx into mdl, and have transferred animations across models, else you cant actually get to this error


Scenario:


You transferred some animations from one unit to another, probably using Oinkerwinkle's animation transfer program. However, corpses, blood, or some form of weird object is appearing at random points.


Cause:


The source and destination models have different animation times. Where one unit dies at a point, the other units walk animation is played. Therefore, your destination model's corpse appears on the walk animation.


Solution: You need to edit the Geosetanim for the offending mesh.


Example/Guide:


Near the start of your mdl file is a section called 'Sequences'. This is a list of animations for the model, and their start/stop times. For this example, I'll use the Acolyte:

 Code:

Sequences 13 {
 Anim "Stand - 1" {
 Interval { 167, 1667 },
 MinimumExtent { -19.5992, -33.1796, -0.28233 },
 MaximumExtent { 52.4008, 36.7176, 99.0722 },
 BoundsRadius 62.4483,
 }


Next, familarise yourself with a Geosetanim. These list a number of animation times, followed by a number, representing whether that geoset appears or not, or sometimes even at a certain % transparency.

 Code:

GeosetAnim {
 Alpha 3 { [b]<-- number is total anim times listed below[/b]
 DontInterp,
 26667: 1, [b]<-- animation times, 1=visible, 0=invisible[/b]
 86567: 0,
 87333: 0,
 }
 GeosetId 0, [b]<-- relates to Geoset ID[/b]
}


Using this information, you can begin to hunt your corpse error.


First you need to locate which geoset is your corpse. Load up your model in the Warcraft Image Viewer II. Keep it at animation 'none' so that you can see the corpse.


Next, you need to find which mesh is the corpse, by loading the tree view in viewer. Deselect the meshes (the tick boxes) one at a time until you find which mesh it is. If the corpse disappears when you uncheck a box, that mesh is obviously the corpse. Make a note of the mesh number. The acolyte's corpse is mesh 4.


In Mdl, your Geoset will be 1 number less than in the viewer (as code runs from 0, rather than from 1). This is the Geoset's ID. 3 for the Acolyte.


Now you need to find at which point your unit is dead. Go to sequences and find Decay Flesh and Decay Bone. Note the start times down.

 Code:

Anim "Decay Flesh" {
 Interval { [b]26667[/b], 86667 },


Then open your mdl file, and search (Control+F) for GeosetAnim. Find the Geosetanim that has your Geoset ID. This is the Geosetanim you will be editing.


You need to set-up the anim times for decay flesh and decay bone to show the corpse. So for decay bone, enter/find the start time and change the number after the colon to 1. Then, for decay flesh, enter/find a number shortly after the start time (to allow a short delay), set to 1. Make sure all other times are set to 0 so that the corpse does not appear.

IMPORTANT


As you transferred animations from a unit, your geosetanim key times will be wrong. You need to copy the list of keytimes from the source animation model, to use for the geosetanims on the new model.

 You must log in or register to reply here.


### Similar threads

Locked

 Fixing Decay Bone anims on Geomerges


iristle

Jun 1, 2007

3D Modeling Tutorials

Replies1Views6K

Jun 27, 2007
Wolverabid

Locked

 Random corpse


Garfield1337

Aug 15, 2011

Modeling & Animation

Replies18Views3K

Aug 17, 2011
Garfield1337

 Fixing Faulty Death / Decay Animations


General Frank

Sep 30, 2007

3D Modeling Tutorials

Replies21Views19K

Sep 11, 2021
Liu

Locked

 Adding Animated Textures with NOTEPAD ALONE


alfredx_sotn

Apr 30, 2007

3D Modeling Tutorials

Replies0Views16K

Apr 30, 2007
alfredx_sotn

Locked

 Show Hide Meshes in Wordpad?


communist_orc

Jun 16, 2008

Modeling & Animation

Replies3Views2K

Jun 24, 2008
Mephestrial

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

3D Modeling Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top