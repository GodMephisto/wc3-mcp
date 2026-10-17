---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/first-step-to-modeling-under-construction.260380/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# First Step to Modeling Under Construction
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/first-step-to-modeling-under-construction.260380/>
#### TinkerLord

#### TinkerLord

 Level 44

JoinedOct 20, 2010Messages2,934

Resources: 104

#### Resources
Models 83Icons 17Spells and Systems 3Tutorials 1

Before we start:

In This Tutorial, I'm Going to Cover the some basic of modeling,common sense of model, Some important fact that You need to know, terminology and the Tools you need to get before enter the Gate of Modeling.

Introduction:

Warcraft Model Have two Format, that is .Mdx and .Mdl. They are both usable when Imported to world editor, But Here are the Different I know Between them

When Making Models, We need to Control the Filesize of model can keep them low, because Warcraft 3 Have a 8MB Limit.

Difference Between .Mdx and .Mdl

.mdlDifferences.mdx

Bigger FilesizeFilesizeLow Filesize

Readable

and Editable

in Text Form

in NotepadReasonsCan Only be

Read by Computer

because is more

Compressed

(I think)

So .Mdl or .Mdx?

Some People usually Edit model in .Mdl Form because they can tweak some stuff in Text Form anytime, For me, I just Edit them in .Mdx form, since I don't usually edit them in Text Form. But Worry Not, you can always Convert them withMDLX Converter

What Tool Can you Use?

Free Tools and Download Link

Magos War3Model Editor:

For Finalizing, Geomerge, Export Model from Warcraft and Viewing Model.

Download

Mdlvis:

For Edit/Tweak Create mesh, rigging/boning and Animating, also Optimizing Model.

Download

Matrix Eater:

For Edit Mesh, Transfer Animation and Geomerge. Useful and Powerful WIP tool for Warcraft Model.

Download

Blender:

Advanced Mesh Creation

Download

MDLX Converter:

Convert between .mdx and .mdl.

Download

3DS/OBJ to MDX Converter:

Download

Convert Obj/3ds to Mdx, (Blender can export to .obj format.

Gimp:

For Making Texture

Download

Note: I don't use the tool Below

Gmax:

For animating, can use to create mesh.

Download

Neodex:

GMax addon Support for Warcraft Model

Download

Other Tools Choice (Not Free):

- Milkshape 3d

- 3ds Max

- Photoshop

Mdlvis and Magos Model Editor is Definitely A Must Have in Warcraft Modeling

while you can Choose Milkshape, Blender or 3DS Max as Big Guns

Fact Need to Know in Model Editor,

### Geoset

### Material

### Texture

### Bone

### Animation

### Basic Control in Model Editor

### Particle Emitter

### Geoset Animation

### Wrapping/UVMapping

Geoset is a Set of vertex, By Using Model Editor, you can Export and Import it between model, In Magos Model Editor, in every model, Geoset is Attached with a Material which gives The Geoset/Mesh it's Texture/Colour, if there is no Material attached, the Geoset will be invisible. This method of Modeling is called Geomerging.

In Model Editor, there is a Geoset Manager

Texture is a 2D Picture that is Wrapped on Model/Meshes, It's Like you wrap a cloth/Paper with colors onto a Empty Sculpture,

There is a Texture Editor in Magos Model Editor. There is In-game texture and Custom Texture, Most of my Model Uses In-game texture because Custom Texture takes up Plenty of Space where Warcraft III Has a 8mb Limit, But Custom Texture Usually Look Better than In-game Texture.

Bone's are related to animation, you attach the Vertex on it, and when you rotate the bone, the Attached Vertex will follow as well. Think of them as Center of Rotation to Place them at the Right Place.

Animation is what make the Model Alive, It means the Movement of the Model, The Model walks, Attack and Death, this all is Animation. This part is Tricky to get started and flaws are very easy to be noticed. But Good animation can change what you think about the model.

I'm Rather weak in this part, but I will explain what I know here, you know those explosion, dust and sparks, you notice in model? Those are particle emitter. Good Particle Emitter can make your model looks powerful.

Geoset Animation is the thing that decide which Geoset appear in which animation, and you can use it to Tint your Model as well.For Examples, notice that corpse in footman model only appear when they decay? or Peasant only has gold Shown when they are in "Stand Gold" animation?

First Step, Exporting Model:

We Now Have the Tools, before we learn to create Model From Scratch, Lets Learn how to Edit the Model First,

First, We need to get a Model to Edit, We can Do that In Magos War3ModelEditor,

Go to Windows->MPQBrowser

At The MPQBrowser,

Go to File and Choose War3.mpq/War3x.mpq/War3xLocal.mpq/War3Patch.mpq

(Choose one, you can swap anytime you like, they have different Models in it)

Model Usually Locate in War3.mpq and War3x.mpq

Choose the Model You Want to Edit (Double Click)

Save and Rename it

File->Save As

You can Change the Internal Name (Optional)

Now You Had Exported the Model

...................................................................................................

Second Step, Geomerging

Geomerging is the basic way of Modeling, from what I understand, It is the Way you Mix Model Together, For Example, You Take the Grunt Head and Replace the Footman Head.

(Note: Such Simple Edit I say Above like Replacing Head are not good enough to get approval In the Model Section.)

So Lets Make A simple Edit On Making a Footman Wolf Rider. (I'm Not Sure model Like this Is Approvable or not, but At Least it is Kinda Useful)

Now, 1st Step is to Export the Model You needed, In this case, The Footman and the Orc Raider

 Path


Footman's : War3.mpq->Units\Human\Footman\Footman.mdx

Orc Raider's : War3.mpq->Units\Orc\Wolfrider\Wolfrider.mdx


Then, You choose which base model you want (Base on the Animation), Since we are using the Orc Raider's Animation, It will be the Base Model.

We Open the Footman Model with Magos War3ModelEditor, And then go to

Windows->Geoset Manager

Geoset is a Group of Vertex that you can Export or Import into Other Model

Since We Want to Mount the Footman, We are going to Export all the Geoset Other than the Corpse geoset.

Just Send this To Graveyard or something First, I really can't stand the Stress of Losing the Wall of Words and I don't know how to Save this thing other than posting it, there are alot more Content Im Going to Add

#### Attachments

5.png

 4.4 KB

 · Views: 187




 Last edited: Jul 17, 2015

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Holy shit that's a lot of tools needed. I only got some very basic modeling experience so I can't really tell much. However I am able to create models in Maya, apply a texture to it, and animate it if I want to, and then import it to Unity (Game Engine). All in the same software.

I am quite amazed that you need 5 programs to do stuff in wc3.

enough whining and getting to the tutorial. It looks pretty neat even in the unfinished state. However I suggest using

#### TinkerLord

#### TinkerLord

 Level 44

JoinedOct 20, 2010Messages2,934

Resources: 104

#### Resources
Models 83Icons 17Spells and Systems 3Tutorials 1

You can just use 1 or 2 if you want though, I use Blender for scratch made model,

Basicly Like Heavy Factory,

Editing Model and Geomerge, I usually just use Mdlvis and war3modeleditor

Matrix eater, Once use it alot, but now I rarely use it, (mY dAMN Antivirus)

MDLX Converter

For Proper Guide ^ (You can just use war3modeleditor to convert it actually)

3DS/OBJ to MDX Converter:

I don't actually use this ^

Gimp

I make Model with In-game texture, But some ppl wanted to make custom texture

The Tutorial Will Update next week btw,

it seem I can't do it what i say up there, I will do this definitely, but.. I can't promise when it will be done


 Last edited: Dec 25, 2014

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Send me a PM or VM if you want it moved back so you can make edits.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

How cool.

 You must log in or register to reply here.


### Similar threads

 Crystal Effect for models


olofmoleman

Dec 24, 2005

Tutorial Archive

Replies3Views3K

Dec 29, 2006
Proll_Troll

P

 Rip models from WoW into Warcraft 3


Maldiran

Dec 7, 2020

Tutorial Archive

Replies25Views23K

Sep 20, 2025
deepstrasz

 Basic Geomerge Tutorial


acolyteofdoom

Dec 30, 2004

Tutorial Archive

Replies12Views8K

Mar 8, 2007
Wolverabid

 Accessorizing Units: Custom Weapons and Shields


The Kings

Apr 9, 2008

Tutorial Archive

Replies4Views3K

Apr 11, 2008
PurplePoot

 Invisible Model - Visible Attachment


I34w90_238j

Jun 3, 2009

Tutorial Archive

Replies4Views4K

Jun 22, 2009
BlinkBoy

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