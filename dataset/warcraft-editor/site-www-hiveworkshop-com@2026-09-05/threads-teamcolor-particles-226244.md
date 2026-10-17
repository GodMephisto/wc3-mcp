---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/teamcolor-particles.226244/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Teamcolor Particles
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/teamcolor-particles.226244/>
#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

I searched for this some time ago and didn't find anything, then when I need this today I searched again without results. So, I take some time to figure out how this works.


What do you need?

- Magos (War 3 Model Editor)


I'll make a simple edit to Jaina's model, to fill up this short tutorial.

1) Open magos, then go to Window/MPQ Browser. In the new window go to File/War3.mpq


Find the Jaina's model in "Units\Human\Jaina\". Double click to open it.


Don't close this window, you need to add 1 texture, it's GenericGlow64.blp and you will find it under "Textures\".


Right click on it and select "Use as texture".

2) Now, you need to create the emitter, go to Window/NodeManager. Right click in a blank area in the new window and


select Create Particle Emitter 2. Right click in the new emitter and select Edit Node, under Pivot Point section, give to Z a value of 20. Press Ok and then double click on it.

Important:


Things that make your model crash your game and the editor.

- Putting in the texture path a "ReplaceableId 2"(Created in the texture manager) and set Replaceable Id to 2 (Under Priority Plane).

- Leaving the texture path to "none" and set Replaceable Id to 2 (Under Priority Plane).


You must put a texture to the emitter to get it work (not a replaceable one), GenericGlow64.blp work just fine for the explanation. And don't forget to put 2 in Replaceable Id (Under Priority Plane).


Knowing this, set the values like this, then press Ok:


In magos, the particles will show white, don't worry, in the editor and in game will display fine.


Save the model, import the model to your map and you should get this:


Credits as always to Magos.


Pd: If there something related to this, please give me the link.


Edit: Attached the modified Jaina's model.

#### Attachments

00.jpg

 189.5 KB

 · Views: 1,508



02.jpg

 214.5 KB

 · Views: 1,457



03.jpg

 364.4 KB

 · Views: 1,452



Jaina.mdx

 152.8 KB

 · Views: 264




 Last edited: Nov 28, 2012

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Wonderful.

Detailed and explained well. Good job. Approved.

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

Wow, that was fast. Thanks for approve this Purge.

#### AlienAtSystem

#### AlienAtSystem

 Level 15

JoinedDec 26, 2009Messages647

Resources: 40

#### Resources
Models 8Icons 29Packs 1Spells and Systems 1User Interface 1

Some Research into further details:

The ReplaceableTexture completely overrides the Texture chosen in the Dialogue Field. Only ReplaceableId 1 and 2 seem to work, with 1 of course having the problem of being square.

The engine will still heed the subdivision-animation that is used for the animated clouds. That will of course produce stupid results with ReplaceableId 2, so set the Fields "Start" and "End" for Head (Life Span), Head (Decay), Tail (Life Span) and Tail (Decay) to 0, and Rows and Collumns under Misc to 1.

Filter Modes unfortunately produce bad results, thanks to ReplaceableId 2 having no alpha channel and black background.
-None, Transparent and Additive produce the same result, with the particles being additive.

-Blend and Add Alpha show the black parts of the texture

-Modulate and Modulate2x (has to be edited into the mdl, Magos doesn't support it) produce completely black squares.​

Most other settings should work fine no matter the values.

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

This only supports TeamGlow, magos let you put any number in the id field... and is better using a glow texture to see the effect in it.

Well, nobody gonna put more than 1 colum and row, everybody knows the teamglow texture is a single one and like any other texture with no alpha, blend, modulate and add alpha filters will don't work in particles.

 You must log in or register to reply here.


### Similar threads

 How to make XY Quad Particles Emitter ( And Rain Drop Effect Tutorial )


Anvil

Aug 27, 2012

3D Modeling Tutorials

Replies10Views14K

Jul 9, 2017
Veronnis

 Teamcolored Particles


Jacek

Jan 28, 2006

Tutorial Archive

Replies9Views3K

Apr 1, 2007
Kimbo

 Model Editing


Fabioz

Jan 18, 2009

3D Modeling Tutorials

Replies45Views66K

Oct 10, 2021
KaiZen7th

 Scratch-Made Weapon Attachments with Particle Emitters


Fabioz

Apr 8, 2008

3D Modeling Tutorials
2

Replies52Views42K

Oct 13, 2010
Ikithe

 Customizing In-game SFX


sniper_zero

Jul 18, 2013

3D Modeling Tutorials

Replies5Views6K

Oct 21, 2013
MN Lahmar

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