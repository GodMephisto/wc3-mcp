---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/particle-emitters-2.329335/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Particle Emitters 2
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/particle-emitters-2.329335/>
#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Credits: @Hermit for Filter Mode research; @tillinghast for animated Gravity error; @GhostWolf for Time & Sort Primitives Far Z definitions, and PE2 Alpha Key functionality; @doom_sheep for Squirt-Visibility interaction

This SD Magos'-centric thread attempts to document Particle Emitters 2.

Orientation axes are Warcraft 3's.

X - Forward

Y - Rightward

Z - Upward

Velocity = Distance covered; influenced by speed, variation, and gravity

Required: A pre-1.29 copy of the game containing the old MPQs that Magos' Editor can navigate.

* = Not observable in Magos', and will have to be tested in-game.

VISIBILITY

Toggle: 0 turns off emission, while values over 0 turn it on.

EMISSION RATE

The amount of particles generated per second. The interval between emissions is capped at ~0.05s (?). Going past this threshold will emit those excess particles in bulk.

Emission rates below 1 will only create particles if the sequence is longer than 1 second.

Visibility and Emission Rate react differently to animation, and so are useful for different situations. For sequences during which they're blank; similar to Geoset & Material Alpha Animations, Visibility toggles on, while Emission Rate results in 0.

For convenience - if you want an emitter turned OFF for a minority of animations, animate Visibility. If you want an emitter turned ON for a minority of animations, animate Emission Rate.

SPEED

By default, causes the particle to move towards its Z. This direction can be changed through emitter or parent rotation.

Animated changes in speed will only affect newly emitted particles; thus gravity is the only way to accelerate.

VARIATION

The percentage of randomness with which the particle can deviate from its base speed. 1 means speed can go down to zero, or double; and 2 means it can go -1x negative*, or triple.

High variation can lead to bottleneck emission when used with Latitude.

LATITUDE

Angles to which the particles will fly, deviating from the emitter's facing direction. A value of 90 means the particles will go towards a semi-spherical area, while 180 removes angular restrictions.

 90 Latitude

 180 Latitude

WIDTH - Along the X Axis (frontward/backward)

LENGTH - Along the Y Axis (left/right)

Width and Length determine the rectangular field in which particles are spawned. Magos' Editor displays these properly with MDXs, but not so with MDLs, where they are reversed.

 100 Width; 10 Length

 100 Length; 10 Width

GRAVITY

An accelerating force that points downwards in the absence of Model Space rotations. Negative values will make the particles go up.

Magos' will not save animated Gravity in MDX; only in MDL. Nor will it open any MDX with animated Gravity.

Flags

UNSHADED* - Will be unaffected by the game's lights. Generally always on for effects.

 The 'sun' comes from the south west by default.

UNFOGGED* - Will not be tinted nor shrouded by terrain fog.

LINE EMITTER* - Whether or not the particles will fly towards two axes instead of three. By default, checking this means the particles will fly YZ.

With Latitude, the angles will expand sideways.

Example usage in Thunder Clap, where it's used with an emitter rotation to make the particles fly XY.

Rotation: { 0, -0.707107, 0, 0.707107 }

SORT PRIMITIVES FAR Z* - Renders the furthest particles on top.

MODEL SPACE* - Whether or not the particle moves and rotates with its parent nodes and the model.

Model Space sees extensive use in Radiance.

XY QUAD - Whether or not the particle will be rendered as a flat, one-sided plane facing the Z axis. For consistent rendering, XY emitters require both Latitude and Velocity at 1 or higher; they might not be visible otherwise.

XY Quads will rotate to XY angles within Latitude.*

With Model Space, they will change facing according to the node's rotation.*

XY Quads see extensive use in Void Disc.

SQUIRT* - Creates all the particles indicated by the Emission Rate at the first keyframe available per animation, instead of over time. If the opening frame of the animation is not set to 0, it will ignore your frame settings and squirt when the animation begins.

Visibility animation may be used for sequence-wide settings, but Squirt cannot be timed properly with that alone. Use Emission Rate for this purpose.

Used by War Stomp and Thunder Clap.

HEAD- The default. When rendering an emitter without Head nor Tail settings, the game will consider it a Head emitter. Leaving the Head box unchecked will hide the particles in Magos' only.

TAIL* - Whether or not the particle will be followed by a tail facing its flying direction. Tails consider the texture's left as their front.

In MDL, mdlvis saves the head setting properly, while changing tails to heads. If both are checked, it saves them both properly. If neither are checked, it checks heads.

In MDX, mdlvis saves both settings properly.

Tail emitters see extensive use in Piercing Thrust.

Segments

Each particle from this emitter will fade through the three colors, sizes, and visibilities (alphas), with each segment's duration decided by <Time>.

Particles will not scale with the models they're attached to, even if they're Model Space. Scale will still affect emitters' spawn field and velocity.

Misc

SPRITES

ROWS & COLUMNS - Used to indicate what's displayed for sprite textures, starting from the upper left.

 Textures\Sparkle_Anim.blp

 ReplaceableTextures\Weather\Clouds8x8.blp

 4x4 sprite; 4 rows & 4 columns

 8x8 sprite

 Layout for a 4x4 texture

HEAD (Life Span & Decay) - Animates sprite textures for head particles, starting from 0. "Life Span" refers to the transition between Segments 1 and 2; while "Decay" refers to that between 2 and 3.

REPEAT - How many times the component repeats its run during the particle's lifetime.

 Inputting 0 in all fields will only display the first image in the sprite.

 Set this way, ReplaceableTextures\Weather\Clouds8x8.blp will fade through its first and last cell throughout the particle's life.

LIFE SPAN - Time before the particle disappears and expires.

TAIL LENGTH* - Segment size only determines width for tails. This is the length; influenced by velocity.

PRIORITY PLANE* - Used to layer/manually determine which particles are displayed on top of each other.

Unlike its Material counterpart, Magos' will not save this field in MDL; only in MDX. Adding it to an MDL through other means will make the model unopenable in Magos'.

Like its Material counterpart, mdlvis will erase its data.

REPLACEABLE ID* - Inputting the following values will replace the TextureID:

1 - Team Color

2 - Team Glow

11 - Cliff

31 - Lordaeron Tree

32 - Ashenvale Tree

33 - Barrens Tree

34 - Northrend Tree

35 - Mushroom Tree

36 - Sunken Ruins Tree

37 - Outland Tree

Example usage in the Wisp, which uses Team Glow tails.

Magos' will not save this field in MDL; only in MDX. Adding it to an MDL through other means will make the model unopenable in Magos'.

TIME - The decimal percentage of the lifespan that serves as a midpoint for particle transition. Ideally above 0.00 and below 1.00.

The lower the particle Time, the shorter it takes to transition between Segments 1 & 2; while increasing transition time for Segments 2 & 3.

Filter Modes

Hermit said:

Filter ModeBehaviourCan be used in material layersCan be used in particle emittersOpacity (Alpha)Draw orderDepth setSort order errors with its own geoset

None (aka Opaque)The geoset draws as solid, with no blend to the frame buffer.YesNoFixedOpaque queueOnNo

Transparent (aka 2 Color Alpha)The geoset draws as solid except for regions over which the texture map alpha channel is black, where the geoset is completely transparent. The cutoff from transparent to solid is 75% white in the alpha channel.YesNoFixed or animatedOpaque queueOnNo

BlendStandard filter mode using an 8-bit alpha channel texture and/or opacity channel. New pixel color and the current frame buffer pixel color are blended together using the new pixel alpha value as a blending factor.YesYesFixed or animatedTransparent queueOffYes

AdditiveThe geoset adds its RGB color values to the frame buffer (saturating at pure white),

causing a ghost-like glowing effect. Most spells use this to make glowing effects.YesYesFixed or animatedTransparent queueOffNo

AddAlphaBehaves the same as Additive, but cannot be used in particle emitters.YesNoFixed or animatedTransparent queueOffNo

ModulatePixels' color values are multiplied together; The geoset multiplies its RGB values to the

frame buffer, making a semi-transparent darkening of the background.YesYesFixedTransparent queueOffNo

Modulate2xThe geoset first doubles its initial RGB values, then multiplies them against the frame

buffer. This technique causes both brightening and darkening in one pass.YesYesFixedTransparent queueOffNo

AlphaKey (aka 2 Bit Alpha)Behaves the same as Transparent, but cannot be used in material layers.NoYesFixed or animatedTransparent queueOff

Some filter modes that can be selected from the drop-down list in particle emitters cannot actually be used for particles; they only work in material layers.

Alpha Key was mistakenly designed by Magos as a particle emitter flag, even though it is actually a filter mode for particles; as a result, the flag is lost after saving MDX files; it remains when MDL files are saved, but the checkmark will not be visible when the model is reopened.

Modulate2x filter mode cannot be chosen anywhere and has to be added by editing the MDL file with a text editor, e.g. Notepad. MDL files with this filter mode cannot be opened by War3 Model Editor, while MDX files can be opened, but Modulate2x is converted to None after saving.

MDLX Converter can convert models that use Modulate2x in material layers, but only Retera's Matrix Eater and Model Studio can open and convert models with particle emitters that use this filter mode.

Click to expand...

Blend removes the transparent parts. It is a staple of blood spurts, dark glows, and black smoke.

Additive removes the black parts, and lightens the texture. It 'adds' to what's in the background. The darker/the more colored an additive particle becomes, the weaker it is. Additive is a staple of light glows.

Modulate removes the white parts, and is used for GenericGlowModX.blp, among others. Cannot fade.

Blends are incompatible with other particles for the most part, and will flicker through each other in-game, if not in the Model Editor as well. This can be circumvented through the use of Priority Planes.

Flare.blp is a comparatively smooth and versatile light, and can be used with Blend in addition to Additive, as opposed to other glows.

Some textures with alpha channels can be used with Blend, while certain black ones are fitted for Modulate. Most of the rest are only viable with Additive.

What is listed as "Add Alpha" in Magos' Particle Emitters is actually Alpha Key, where alphas below 0.75 are invisible.

Changing an emitter's filter mode to AddAlpha will cause the entire model to display as a green checkered cube in-game.

Tips

Black and white colors are useful to prevent your effect from looking monotone, whatever your primary hue is. Use darker particles as cushions below the other particles, or colder colors as outliers, and lighter, warmer ones for the centers. For segment colors, it's recommended to go from light to dark as particles fade.

Use color variation to your advantage. Fade an emitter to a nearby color over its lifetime.

It is oftentimes better not to have constant particle size. For sparks, consider having their scaling at 0 for the third segment, while retaining high alpha. On the inverse, dust clouds look better when they fade to 0 alpha in bigger sizes.

Glows, glows, glows. Use them to give your effect atmosphere. Use smooth glows such as flare.blp for generic light, and thicker ones such as LavaLump2.blp for sparks. Some of the default colored glows are weaker and unsuitable for sparks, so consider using custom ones when necessary.

Things to Know

Individual particles cannot rotate independently, apart from tails forced to turn by gravity.

Using MDL/X Converter will remove Billboard settings for Particle Emitter nodes.

In-Game Discrepancies

Gravity and Speed are calculated differently

Particles and their movement are rendered smoother in-game; even moreso in the World Editor
Testing in-game/in-editor often is integral to effects, and modeling at large. Back up often. Good luck, have fun!


 Last edited: Sep 2, 2021

#### Veritas 117

#### Veritas 117

 Level 14

JoinedNov 17, 2010Messages1,266

This is incredibly useful. Understanding what each of those fields actually means is really helpful when it comes to making and manipulating existing particles.

nice job on this.

#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


For any further reading I also recommend reading the code that powers open source rendering of WC3 particle emitters in replica technologies (below is written by Ghostwolf):

flowtsohg/mdx-m3-viewer

I've been working with a desktop game port of the code above on the LibGDX engine and it looks very War3-alike:

To me, it really shows how hard this guy worked researching not just how to edit the format, but exhaustively how to display it in a technical sense.


 Last edited: Dec 19, 2020

#### Jaccouille

#### Jaccouille

 Level 26

JoinedFeb 1, 2019Messages468

Resources: 66

#### Resources
Models 16Icons 3Tools 2Maps 2Ported Models 29Ported Icons 14

An update on current tutorial is much appreciated, I don't really understand the effect of the XY Quad even by reading the description you provided, most likely because I am not used with this context, haven't toyed enough with particle emitter, maybe add an example, e.g "This is usually checked for projectile effect".

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

Jaccouille said:

 An update on current tutorial is much appreciated, I don't really understand the effect of the XY Quad even by reading the description you provided, most likely because I am not used with this context, haven't toyed enough with particle emitter, maybe add an example, e.g "This is usually checked for projectile effect".


Click to expand...

Edited the description and added an example.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

This is fantastic. I don't know how we went so many years as a community without attempting to codify & understand such an integral part of an integral part of the community's tool-base.

The only thing this needs now, in my mind, is a copious amount of pictures (providing examples, both with in-game/standard SFX & perhaps a single "dummy SFX" that showcases each change), and a test map with a bunch of custom 'dummy SFX' to showcase stuff as well.

#### Hermit

#### Hermit

 Level 22

JoinedMay 29, 2013Messages1,573

Resources: 8

#### Resources
Models 4Icons 3Sounds 1

Good job; it's about time someone made this.

You might want to explain the Rows and Columns (that are used when the particles are using sections of the texture at a time) and update the list of replaceable textures since it is incomplete; Replaceable ID 36 is used by sunken ruins trees (Doodads\Terrain\RuinsTree) and Replaceable ID 37 is used by outland mushroom trees (Doodads\Terrain\OutlandMushroomTree).

I suggest going a bit more into detail about filter modes; at least mention all of them that can be used in particle emitters.

Filter ModeBehaviourCan be used in material layersCan be used in particle emitters

Opacity (Alpha)Draw orderDepth setSort order errors with its own geoset

None (aka Opaque)The geoset draws as solid, with no blend to the frame buffer.YesNoFixedOpaque queueOnNo

Transparent (aka 2 Color Alpha)The geoset draws as solid except for regions over which the texture map alpha channel is black, where the geoset is completely transparent. The cutoff from transparent to solid is 75% white in the alpha channel.YesNoFixed or animatedOpaque queueOnNo

BlendStandard filter mode using an 8-bit alpha channel texture and/or opacity channel. New pixel color and the current frame buffer pixel color are blended together using the new pixel alpha value as a blending factor.YesYesFixed or animatedTransparent queueOffYes

AdditiveThe geoset adds its RGB color values to the frame buffer (saturating at pure white),

causing a ghost-like glowing effect. Most spells use this to make glowing effects.YesYesFixed or animatedTransparent queueOffNo

AddAlphaBehaves the same as Additive, but cannot be used in particle emitters.YesNoFixed or animatedTransparent queueOffNo

ModulatePixels' color values are multiplied together; The geoset multiplies its RGB values to the

frame buffer, making a semi-transparent darkening of the background.

YesYesFixedTransparent queueOffNo

Modulate2xThe geoset first doubles its initial RGB values, then multiplies them against the frame

 buffer. This technique causes both brightening and darkening in one pass.YesYesFixedTransparent queueOffNo

AlphaKey (aka 2 Bit Alpha)Behaves the same as Transparent, but cannot be used in material layers.NoYesFixed or animatedTransparent queueOff

Some filter modes that can be selected from the drop-down list in particle emitters cannot actually be used for particles; they only work in material layers.

Alpha Key was mistakenly designed by Magos as a particle emitter flag, even though it is actually a filter mode for particles; as a result, the flag is lost after saving MDX files; it remains when MDL files are saved, but the checkmark will not be visible when the model is reopened.

Modulate2x filter mode cannot be chosen anywhere and has to be added by editing the MDL file with a text editor, e.g. Notepad. MDL files with this filter mode cannot be opened by War3 Model Editor, while MDX files can be opened, but Modulate2x is converted to None after saving.

MDLX Converter can convert models that use Modulate2x in material layers, but only Retera's Matrix Eater and Model Studio can open and convert models with particle emitters that use this filter mode.


 Last edited: Dec 19, 2020

#### Vinz

#### Vinz

 Level 84

JoinedApr 24, 2012Messages9,842

Resources: 262

#### Resources
Models 208Icons 26Skins 3Maps 8Spells and Systems 8Ported Icons 4Reforged HD Icons 1User Interface 1Tutorials 3

@Hermit this is great! Quoting it in the main post. Thanks for the input.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Lovin' the pics.

Just a minor note: very good of you to credit & link back to Hermit's contribution, but may I suggest not doing the whole "giant quote" thing? Not only does it mess with the formatting a bit (tries to squeeze that whole table into a Quote block), but it italicizes everything which is weird.

Instead, you can just quote it directly (BBCode & text & such), with a bit at the top crediting Hermit (hyperlinked to his comment).

#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


Mythic said:

 Priority Plane* - Used to layer/manually determine which particles are displayed on top of each other.

Unlike its Material counterpart, Magos' will not save this field in MDL; only in MDX. Adding it to an MDL through other means will make the model unopenable in Magos'.

Like its Material counterpart, mdlvis will erase its data.


Click to expand...

if you know this, just go and fix them, okay??? it's our civic duty

Edit: or do you want us to suffer?

#### Attachments

War3ModelEditorSource.7z

 158.8 KB

 · Views: 211



visSRC.rar

 528 KB

 · Views: 519



#### maddeem

#### maddeem

 Level 24

JoinedJan 1, 2011Messages1,506

Resources: 34

#### Resources
Models 5Icons 3Skins 1Maps 6Spells and Systems 18Lua 1

Finally particle emitters make sense, thank you!

#### tillinghast

#### tillinghast

 Level 55

JoinedJun 2, 2008Messages946

Resources: 99

#### Resources
Models 97Icons 2

What's with animated gravity? Magos reverts it to static after i reload the model, if i add it through mdl text editing then it gives an error. Looks like RMS can handle it without problems and yet sanity checker gives a yellow warning about it. I guess it's better to just create a copy of emitter with different static gravity?

#### Bleeq

#### Bleeq

Patron

 Level 33

JoinedJun 4, 2023Messages471

Resources: 22

#### Resources
Models 21Skins 1

There seems to be a minimum combination of Emissionrate(ER) and LifeSpan(LS) in-game. When I had a tail emitter with ER 6, and LS 0.15, I would get emissions, but dropping ER to 5 simply broke it and I got nothing.

With LS 0.8, I could drop ER down to 2 or 3. Some other options like speed and tail length did vary though, too.

#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


tillinghast said:

 Magos reverts it to static after i reload the model


Click to expand...

It's probably broken. Knowing that certain parts of the model are lost when opening and saving in Magos is one of the reasons that I stopped using Magos more than a dozen years ago.

Edit:

Bleeq said:

 With LS 0.8, I could drop ER down to 2 or 3. Some other options like speed and tail length did vary though, too.


Click to expand...

Which game version was this on? Did you check if your particle detail was set to Low, Medium, or High?

#### Bleeq

#### Bleeq

Patron

 Level 33

JoinedJun 4, 2023Messages471

Resources: 22

#### Resources
Models 21Skins 1

Retera said:

 Which game version was this on? Did you check if your particle detail was set to Low, Medium, or High?


Click to expand...

I assume it's whatever the latest version is, given I play it through the Blizzard launcher, and they've blocked me before from even testing my maps without updating -_-.

No Idea about the detail, I didn't know that was a thing. That sounds possible, though. If I discover that was the problem I'll post back.

#### Retera

#### Retera

Banned

 Level 69

JoinedApr 19, 2008Messages2,951

Resources: 148

#### Resources
Models 91Icons 1Skins 8Tools 6Maps 14Templates 2Reforged HD Models 16Ported Models 3User Interface 1Sounds 5Tutorials 1

##
 Warsmash Mod Engine


Bleeq said:

 I didn't know that was a thing.


Click to expand...

It may have been removed on 1.32+, so it's possible that on your version the particle detail level is always maximum. It was a 2002 feature for supporting lower end hardware. Maybe the World Editor would still have a setting for it.

#### OVOgenez

#### OVOgenez

 Level 38

JoinedJul 7, 2019Messages382

Resources: 27

#### Resources
Models 24Packs 2Spells and Systems 1

From my observations:

EmissionRate can be negative, which will direct the particles in the opposite direction. This can be useful to avoid rotating the emitter by 180 degrees.

TailLength can be negative, which will make the tail appear in front of the particle instead of behind it.

#### OVOgenez

#### OVOgenez

 Level 38

JoinedJul 7, 2019Messages382

Resources: 27

#### Resources
Models 24Packs 2Spells and Systems 1

Share pls your experience if you have dealt with scaling particle emitters (for example, to create enlarged Medium/Large versions).

From my experience, simply using the Scale modifier on the node is not enough. The particle size and gravity also need to be scaled accordingly.

While gravity could be animated, particle size cannot be handled the same way. Instead, you would need to create separate copies of the emitter for each size and switch(animate) their visibility depending on the animation. Only then will the scaled effect appear visually identical to the original.

#### wiselen

#### wiselen

 Level 11

JoinedJun 30, 2021Messages68

Resources: 7

#### Resources
Maps 5Spells and Systems 1Reforged HD Models 1

Try the Tools -> Scale Model feature in TRMS, I believe it scales all values, including emitters

#### OVOgenez

#### OVOgenez

 Level 38

JoinedJul 7, 2019Messages382

Resources: 27

#### Resources
Models 24Packs 2Spells and Systems 1

wiselen said:

 Try the Tools -> Scale Model feature in TRMS, I believe it scales all values, including emitters


Click to expand...

Yes, but the point is how the standard Scale node modifier affects the particle emitter.

 You must log in or register to reply here.


### Similar threads

 How to make XY Quad Particles Emitter ( And Rain Drop Effect Tutorial )


Anvil

Aug 27, 2012

3D Modeling Tutorials

Replies10Views14K

Jul 9, 2017
Veronnis

 Scratch-Made Weapon Attachments with Particle Emitters


Fabioz

Apr 8, 2008

3D Modeling Tutorials
2

Replies52Views42K

Oct 13, 2010
Ikithe

Locked

 Particle emitter help


Sephiroth_VII

Jul 15, 2007

Modeling & Animation

Replies5Views2K

Jul 17, 2007
Sephiroth_VII

Locked

 Particle Emitter Trouble


Raj91

Dec 10, 2009

Modeling & Animation

Replies0Views973

Dec 10, 2009
Raj91

 Classical Ribbons vs Particles Ribbons


Daelin

Nov 21, 2006

3D Modeling Tutorials

Replies0Views25K

Nov 21, 2006
Daelin

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