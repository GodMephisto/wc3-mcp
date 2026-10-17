---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/fundamental-principles-of-lighting.263152/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Fundamental Principles of Lighting
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/fundamental-principles-of-lighting.263152/>
#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

#
 Fundamental Principles of Lighting

This is meant to give you an in-depth coverage of some tried out lighting techniques. Believable lighting is the A and O of terrain art, and a determining factor between average joe and extraordinary joseph. When working with lighting, there are a few key models that are used, barring of course the terrain fog tool which won't be covered here to a great extent. For information about the ins and outs of the fog tool, see All about fog by Volvox.

Note: This lighting guide is primarily for terrain art. That is, working from a set camera angle and doing one screen only. i.e. not playable. Some concepts can be directly translated to playable terraining too, but most lighting techniques rely entirely on a set angle and a clear sky view which you won't get with the isometric in-game camera settings.

Table of contents:

Tools & Background

Ignore Model Clicks

Native Editor Settings

Glow Model

Flares

Light (Affects Shadows)

Animated DNC Light Models

Conclusion & Other SFX

(1) Tools and Background

What is lighting?

Not to be confused with lightning. Lightning is an electrostatic discharge during a thunder storm. Lighting is basically illumination. The deliberate use of light to achieve a practical or aesthetic effect. Be that ambient lighting for the general illumination of an area or task lighting which is functional, often concentrated, lighting to illuminate certain areas, e.g. a reading light.

Models

As mentioned, we will be working with a set of lighting models that are key to good lighting.

Glows

Flares

White Light (Affects Shadows)

Animated DNC light
They are collectively included in the Ultimate Terraining Map 4.0 under the Cinematic tab of the doodad palette. Most notably under the Particle subgroup.​

Knowledge

This will be an advanced guide for intermediate terrainers. If that doesn't make sense, I mean to explain advanced concepts for intermediate terrainers that want to take it up to the next level. But I at least assume you are familiar with the editor and environmental terraining already. For obvious reasons, having some artistic sense is also key. You can learn all the tools of the trade and it still won't accomplish squat if you seriously can't spot the difference between good and terrible. You should prepare yourself for some sleepless nights dealing with crash-on-select issues and working with getting the exact right shine. A great deal of patience will be required.​

Make it easy on yourself

Save! Save again! Save one more time for good measure! While lighting can be the making of your terrain, it can conversely also be the breaking of it. With large clumps of huge light models, selecting one or two specific ones for deletion is practically suicide. Add crash-on-select issues to this and you have yourself a great mess to untangle. So whenever you take a leap of faith, make a backup of the map before the change. Make several copies of your lighting objects too. It's not uncommon that I have 10 different copies of glows in the object editor just so I can more easily delete portions of all glows without touching the rest of the map. e.g. one set of glows for the background, one for the foreground, one for the mid ground, one for critical sections, one for small ambient additions, etc.

Take my word for it, you won't regret it.​

(2) Ignoring Model Clicks

This is such a huge time saver that I figure I will cover this straight away. Most lighting models are going to be scaled up ridiculously and therefore make it almost impossible to select anything else. Some even to the point where they crash the editor when you select them, what I call a crash-on-select issue. Luckily, there is a really simple solution for this natively in the object editor.

It's a simple boolean. When set to true, the object will no longer be selectable. This can obviously easily be unticked if you want to edit it later on. It's amazing how many pointless crashes and select issues this fixes with the click of a button.

[highlight]Don't drink too much of the Kool Aid:[/code]

This will only disable clicking. When you multiselect with either the Select All (Ctrl + A) option or when you select by dragging your mouse over several doodads, even objects with this option set will be selected.​

Deleting objects that will crash on select

There are two viable ways that I know of. If you feel confident that you can delete all instances of the object, make a copy of the object and delete the original version. This will destroy all instances of the placed object.

If you want to delete individual objects, you can for instance change the model file to one that won't cause a crash, delete what you want then change it back. You can also set the default scale of the object to a small number, e.g. 0.1. Delete what you want, then change it back.

​

(3) Native Editor Settings

Some native editor settings affect the lighting heavily. These settings will play a major role in the end result, because it affects everything from the color scheme to the atmosphere. Objects may change greatly in color/intensity depending on lighting settings, so switching back and forth is not a good idea once you've started fine tuning the lighting.

Custom Light Environment

Custom Light Environment is an option found under Scenario > Map Options. There are a set of selectable light environment options. More specifically, there is one for every tileset. This may sound like too much to play with, but most of them are, if not completely then almost, the same. By some simple elimination, we can summarize this item with five notable options: Ashenvale, Dungeon, Dalaran Ruins, Village and Underground.

To give you a taste of the change switching these settings on finished products can make, here are four versions of one of my terrains. The only thing that separates the screens is different custom light environment settings. In order of appearance: the original one I used (Dalaran Ruins), switched to Ashenvale, switched to Dungeon and switched to Underground.

Time of Day

Another thing that drastically changes the lighting and appearance of a terrain, is the Time of Day setting. Found under Scenario > Map Preferences, there are six selectable options in a dropdown menu.

Use Editor Preference
Uses editor's native settings.

Not Fixed
Day/Night Cycle. The editor cycles periodocally between the four as a normal day would go (only faster, obviously).

Fixed - Dawn

Fixed - Noon

Fixed - Dusk

Fixed - Midnight
You may have guessed that Not Fixed is not a good option. Being that this option alters the outcome heavily, we wouldn't want any discrepancy when setting up the screen just the way we want it. This option affects everything from how shadows appear to brightness and even the color of objects.

Like with custom light environment, I will show the effects by using an old terrain of mine and switching the settings around. In order of appearance from left to right: dawn, noon dusk, midnight.

There is usually no clear distinction of right and wrong here. It's all dependant on the terrain you want to do. The only way to really master it is by screwing up a couple times. Experiment away!​

(4) Glow Model

The glow model can be used in various ways. When you hear people saying that it really is just spamming the glow model for great results, that isn't actually far from the truth. It really is an awesome mean for great results in itself. But knowing some tricks behind it won't hurt. For reference, you can use the glow model to convey more depth in your terrain, fool around with the color scheme, use it to make lighting from torches and other light sources more believable, etc. It wouldn't be a crime to say that without the glow model, terrain art would not be what it is today.

From now on, this is your best friend. Get familiar with it.

White Noise

You will normally be stacking glows for greater effect. This why the objects usually have a low RGB value set in the editor. More specifically, when placed cumulatively ontop of each other, one phenomena gets more apparent: the intesity of the glow. When the glow gets too intense, you will get what some call a white noise effect. As a rule of thumb, you want to avoid this. There are circumstances when you want small portions of it, e.g. blinding light. But generally it's just plain bad. This animation should show the effect of the glow after more glows are being placed ontop of it successively.

(Click to enlarge)

Fog Support

One great usage of the glow model is to support the fog by intensifying different areas or giving it an ambient glow with large objects. You can fight off the end of the world feeling and give it several layers of depth more than it actually has. You can even force perspective, e.g. that an object close to the camera source actually looks far away.

Here are three versions of a terrain, one without glow (left), one with just a couple glow objects tinted 50/50/50 (middle) and one with a slight color variation 80/50/50 to introduce a color blend (right).

[highlight]Don't drink too much of the Kool Aid:[/code]

Too much glow can make the whole thing look blurred out and out of focus. Unless this is an effect you want, be wary of this. You also don't want disjoint glows that doesn't appear to have a source. When you use it for ambient lighting and fog support (above examples), make sure you make it big enough so you don't see the edges clearly.​

It also works really well in combination with the cloudspheres to e.g. intensify portions of the cloud coverage. For cloudspheres, see Enhancing the Skies. It also affects doodads. This would be how you introduce more depth into your terrain.

Depth

Generally, you want to include some depth. Using the glow model is perfect for just that. The more an object is affected by fog and glow, the further away it looks. The effect you want is a fade into the horizon, making the objects help convey the depth of the field. This is a simple example with some trees. The lines (roughly) separate foreground, midground and background. It is possible to convey even more layers of depth by making objects more and more affected by the fog/glow gradient.

​

(5) Flares

In many ways, the flare model can be used as a replacement, or in combination, with the glow model. It yields slightly different results. For one, the flare usually blur quicker. Secondly, the light of the flare itself has a clear center and exponentially fades off towards the edges. I won't cover all the same steps I did with the glow object. If you want to use it similarly, refer to that section. The principles are, pretty much, analogous. Instead I will show you where the flare model has the jump on the glow model.

This is the flare model.

Torches and Light Sources

This is a critical example where the flare is to prefer over the glow, AFAIC. The yield is much more satisfying primarily because of the unfocused edges. With the glow model, you can pretty much always tell where the model "stops". To get the best effect for torch light and the likes, use a couple flares in increasing size but with the same center. This animation shows the development in stages.

(Click to enlarge)

Sun

I'm going to couple this with the Star White model, which also yields a nice result for this effect. The trick is simple and also helps depth perception. Basically, what we are going to do is place a series of aligned flare/star objects in succession from background to freground. This builds intesnity and size and you also get a nice 3D effect from it.

Here are two examples: one with sun flares, one with stars.

Sun Flares

Shocker, right? But yes, obvious or not, the sun flare model actually fills its original purpose in a good way. With that in mind, I won't demonstrate that myself here. For now, I will piggyback on 2-P's old lighting experiments. It shows the intended effect well and figuring out how it works shouldn't be a hassle.

http://www.wc3c.net/showthread.php?t=90226​

(6) Light: White (Affects Shadows)

This is perhaps the trickiest piece of lighting to work with. First of all, it isn't selectable at all natively. But we can fix that. Editor - Use Clicker Helper. A simple boolean in the object editor, like Ignore Model Clicks. When set to true, the object will have an Info Display box showing that is easily selectable (that is, if you have Info Display enabled under View).

Onwards. What we will be working with now is what's called an omnilight. Omnilights illuminate their surroundings. Specifically, they affect nearby shadows. In addition, they also intensify the glows of other nearby lighting objects including cloudspheres, making them great for adding some highlights to the sky. In general, you can use these to add brightness, contrast and even color to your terrains. The number of applications for this is pretty much innumerable. For reference, and mostly to show off the effect, here are two pictures of a wall: one without light (left) and one with an omnilight placed next to it.

It can also light up objects that are too obscured by the terrain fog that it's barely visible. This can be extremely helpful. Note however that objects that are completely fog-covered will still not be visible in most cases. The omnilight does affect it, it's only too weak to light up the entire thing.

[highlight]Don't drink too much of the Kool Aid:[/code]

Overuse can yield a weird clipping effect. It is also fairly difficult to control and may do you more harm than good if you use too many. Stay within a one digit limit.​

You can probably guess by this point the use this has. This is pure unadulterated illumination. Given how many ways you can use this, I won't exemplify. Mostly becuase there would too much content for a single page tutorial, but also because most of them are extremely obvious. Trial and error is your friend here!​

(7) Animated DNC light models

The only item not included in the Ultimate Terraining Map. These are basically models made to simulate the Day/Night Cycle, hence DNC. For a full description of what this really is and what it does, see HappyTauren's coverage here. Note that knowing the ins and outs of animated DNC light models won't be necessary for using this technique, the link is more directed at interested parties and to prevent any unattached strings. We will simply be using this as ambient light. Specifically, blacking out the entire map and adding light sources manually with other lighting objects. This can have a great atmospheric effect perfect for darker scenes. Demonstrated here by, once again, one of my own pieces from some time back.

https://www.hiveworkshop.com/forums/members/133407-albums4068-picture83880.png

The Model

Import (View attachment DNCAshenValeTerrain.mdx) into your map. Download it by right-clicking the link to the left and save as mdx. After importing it, give it the path Environment\DNC\DNCAshenvale\DNCAshenValeTerrain\DNCAshenValeTerrain.mdx. Save and re-open the map as you would normally after importing material.

The Effect

When you open the map up again, nothing appears to have changed. This specific DNC model will only take effect when you set custom light settings to Ashenvale, like we did in step (3).

As you can see, the entire map blacks out and all doodads placed will be affected by it too. This is almost the same effect fog of war has, apart from one crucial difference: we can use the light object in step (6) to brighten up parts of it. To the left we have the default map with the custom light environment box set to Ashenvale but no light placed, to the right we have the same scene with a light object placed next to the trees.

This can yield some really authentic dark scenes. Note that all parts lit up by light objects can host other visible objects, but they will naturally fade into more shaded around the edges of the lit up area. Ergo placed objects will, basically, have natural shading, which is partly the reason that makes this effect so believable.

Larger Areas

Remember how we said placing too many light objects in step (6) can yield an ugly clipping effect? This would mean that lighting up larger areas could be pretty annoying. Well good news, the light object is actually, to some extent, scalable. It only has to be pretty large to notice the difference. One weird side effect is also that by scaling it up, the actual illumination seemingly gets pushed further away from its center. This will take a few tries to get right and you may have to move each object around for a bit before you get the effect you want.

To help you with moving an object without dragging and dropping, you can enable Num Lock and use the numpad to move selected objects around. Specifically, 2, 4, 6, and 8 will move it down, left, right, and up respectively. 1, 3, 7 and 9 will move it diagonally in respective direction as the key placement suggests. Note that it won't move it according to your camera settings, but according to the map's native grid. To enable the grid, press G or go under View > Grid.

To the left you can see the clipping effect and to the right you can see a big illuminated area using only three light objects.

Other Animated DNC Light Models?

Yes, there are others. Similarly to how this one emits a black shade, you can theoretically give it any other color too that is capable of emitting light. This simple guide by NullCurrent from thehelper.net should show you the simple steps of creating one yourself. Otherwise just use the Request forum.

[highlight]Don't drink too much of the Kool Aid:[/code]

As you can see in the tutorial over at thehelper.net, doodads that are unrevealed will show as the color of your DNC light model, sometimes giving it a weird effect.​

(8) Conclusion & Other SFX

This is the conclusion for this tutorial, but not the conclusion for lighting. Following this guide should give you adequate scope to branch out on your own and use lighting the way you see fit. Trial and error is your friend, practice makes perfect and all that junk. The WarCraft platform really sucks on the lighting-side so don't rely on getting good results with conventional and native means. That's partly why the Terrain Board has spent so much time over the years trying to force good lighting onto a terrible platform. Use all the magic tricks in the book. I have some helpers to cover before wrapping this up.

Disjoint Lighting (Lights without Light Source)

Either which way you do lighting, remember that light doesn't just happen by itself. You always need a source. Placing light models aimlessly will make people question where the hell the light is coming from. Similarly, if you do a sun effect like we did with the flares you shouldn't e.g. have rays coming in from above (because two suns?). Think hard and well how light would appear normally.

Other SFX Models

Don't underestimate the standard SFX (Spell/Effect) models in the editor. They can help sell a scene really well in combination with extraneous lighting. The point here is that any piece of lighting that looks good, is good. This also ties back to what I said at the start. I can teach you the tools of the trade, but I can't help you be a great terrainer. Learning the brush strokes doesn't make you Leonardo da Vinci. The making or breaking of your terraining will be your own artistic style and sense, and no external source can teach you that.​

Final Words

It usually helps to look at other works if you are uncertain of your own skills. You can borrow some ideas while training, you can even try and completely copy other works for learning's sake.

If there is anything else you want covered here, just say so in the thread and I will look into it. Happy terraining!


 Last edited: Feb 16, 2016

#### StarGreen

#### StarGreen

 Level 4

JoinedNov 25, 2014Messages87

Super helpful already!

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

You can always rely on Fladder when it comes to terraining the good stuff.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Awesome tutorial and lovely examples. It is funny how the modding community has expanded upon lighting so immensely considering it is something hardly supported in wc3. You somehow manage to make it work out. I assume this tutorial is still being expanded (either that or it just has an abrupt ending), but I just wanted to comment to re-entice you to complete the tutorial.  Let me know when it is finished!

#### fladdermasken

#### fladdermasken

Off-Topic Moderator

 Level 39

JoinedDec 27, 2006Messages3,689

Resources: 3

#### Resources
Tutorials 3

And it's done.

StarGreen said:

 Super helpful already!

Click to expand...

Zwiebelchen said:

 You can always rely on Fladder when it comes to terraining the good stuff.

Click to expand...

PurgeandFire said:

 Awesome tutorial and lovely examples.


Click to expand...

PurgeandFire said:

 It is funny how the modding community has expanded upon lighting so immensely considering it is something hardly supported in wc3. You somehow manage to make it work out.


Click to expand...

That's probably why people have made so much headway with lighting. Because without it, terraining in the editor really looks shitty. Necessity is the mother of invention, yadda yadda.

 I assume this tutorial is still being expanded (either that or it just has an abrupt ending), but I just wanted to comment to re-entice you to complete the tutorial.  Let me know when it is finished!


Click to expand...

Done!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great update. I especially like that you covered the annoying clipping from large lights--it seems to be a pretty big issue that detracts people from dealing with lights at all.

Approved!

#### Heinvers

#### Heinvers

 Level 53

JoinedMay 7, 2010Messages9,267

Resources: 101

#### Resources
Models 3Icons 65Packs 3Skins 27Templates 3

Good stuff.

#### Ezekiel12

#### Ezekiel12

 Level 12

JoinedMar 13, 2012Messages1,121

Very good overview! My only lament is that every thumbnail takes aaageeeeees to load but I guess that's Hives server fault.

#### N.Knight

#### N.Knight

 Level 11

JoinedJan 30, 2010Messages547

Resources: 1

#### Resources
Models 1

A very neat and organized tutorial with enough material to let newbies fiddle around with for another decade of wc3 modding. Keep it up Fladdy!

#### SA Dashie

#### SA Dashie

 Level 22

JoinedApr 3, 2010Messages1,889

Resources: 17

#### Resources
Maps 11Spells and Systems 3Templates 1User Interface 2

Who the hell are you?!?


Edit: Great tutorial btw fladder


 Last edited: Mar 27, 2015

#### Deleted member 238226

D

#### Deleted member 238226

sexy

#### WhiteFang

#### WhiteFang

 Level 21

JoinedJul 6, 2014Messages6,786

Awesome tutorial that has helped me a lot

#### Emm-A-

#### Emm-A-

 Level 14

JoinedJul 1, 2008Messages1,314

Resources: 1

#### Resources
Spells 1

great tutorial, thank you for sharing this!

#### RedLord

R

#### RedLord

#### XEdmondX

#### XEdmondX

 Level 4

JoinedJan 14, 2016Messages63

Omg that 4 time of day image like Skyrime... nice! And of course, great and helpful tutorial. Thanks!

 You must log in or register to reply here.


### Similar threads

 Terrain: Advanced Lighting


~Void~

Feb 13, 2008

Misc/General Mapping Tutorials

Replies47Views32K

Jul 25, 2011
Skycraft

 Terraining: Principles of Pitch & Roll


Belgarath

Jan 18, 2012

Misc/General Mapping Tutorials

Replies14Views15K

Jan 20, 2022
AbsolutePsycho

 Fade Filter Fix


Imadori

May 18, 2017

Misc/General Mapping Tutorials

Replies9Views3K

Jan 6, 2018
IcemanBo

Locked

 Possible setting the light intensity with animation index for light model(ex.tourch)


shadowvzs

Feb 5, 2013

World Editor Help Zone

Replies7Views2K

Feb 7, 2013
WaterKnight

 Fake HDR Lighting in Warcraft 3


A Void

Jul 20, 2015

Misc/General Mapping Tutorials

Replies14Views12K

Mar 15, 2016
Dr Super Good

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