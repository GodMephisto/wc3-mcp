---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/modeling-theory-110.92023/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Modeling Theory 110
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/modeling-theory-110.92023/>
#### Draelren

#### Draelren

 Level 7

JoinedJul 20, 2008Messages70

Resources: 3

#### Resources
Models 2Tutorials 1

##
 Modeling Theory 110

This tutorial is more of hints, tips, and tricks for modeling. I will be using Autodesk 3ds max 8, and 3ds max 2008, but most of the same ideas still are accepted for most traditional 3d modeling tools.

This is meant to supplement the already existing tutorial for Modeling, found at: https://www.hiveworkshop.com/forums/f282/modeling-animation-101-a-41840/

This has some good basic information, but misses a lot of various theory, definitions, and as well as no-no's to do in 3d.

This will be divided into several sections.

Chapter 1: Definitions

- Vertex, Edge, Polygon

- Normals

- Rigging, Skinning

Chapter 2: Polygonal Theory

- Multi-point polygons and Star Junctions

 - Fixing

More to be added as I think of it!

Chapter 1: Definitions

Vertex: A point in space, does not necessarily have to lead to anywhere, but is the founding bases for all edges and polygons.

Edge: A "line" essentially, this is composed of 2 verticies connected together.

Polygon: Consisting of several Verticies, and edges, this is what you will be dealing most with. All polygons must have at least 3 points, and NEVER more than 4 - which I will go into in Chapter 2.

Normals: Normals are the direction your polygons "up" face is. In many different applications, when you're looking at the backside of a polygon, it is either see through, or is pitch black. This is to show you that your normals are flipped.

Rigging: Rigging is basically setting up a bone system for your 3d model, either through a biped chain, or a bone chain.

Skinning: The biggest mistake I see when it comes to people in the WarCraft III modding world, is there constant abuse of this word. It has NOTHING to do with a texture, material, or any type of coloring at all. Skinning is applying a "vertex weight" to each point on a model, telling the mesh how to deform according to the bone. This is the next step after rigging, and WarCraft III does not even support this.

I repeat, Skinning, skins, etc etc - does NOT mean texture. Yes, you heard me.

Chapter 2: Polygonal Theory

###
 Multi-point Polygons

Alright, so - you've gone through tutorials, and have this beautiful model right?

Skyline Rim, low poly:

Is this accepted as a 3d model? Possibly - but it could cause problems in some games, especially if there's smoothing.

Rim with smoothing on:

What's with the black smudges? It's because you have a 20 point polygon! Stay away from this for this reason!

So how do you fix it? In 3ds max there is a button called "Inset", go to your polygon selection tool, and select that polygon. (Refer to the first attachment in this chapter)

Now, select the inset button dialog box:

This will bring up a window, increase the number a bit and watch what happens to your polygon, shown below:

Now, click Apply two or so times, and then Ok, you should have the following result:

But, there is still a 20 point polygon isn't there? You're right. Now comes the time to fix this.

Delete that very inner polygon, and then Select all of the edges around it:

Now, go to your scale mode:

Hold down SHIFT, and drag IN, scaling in on all 3 axis, keep going until you no longer can see anything.

Now, select all of those new verticies you just made, and select Weld.

Make sure that when you weld, it now says that only one vertex is selected afterwards.

Perfection:

You now have all 4, or 3 sided polygon's throughout the whole model. and smooths beautifully:

###
 Star Junctions

What is a star junction? It simply is a point where more than 4 points converge together on a model, which will also cause problems when smoothing is applied.

Star Junction example:

This can be easily fixed, I will not walk you through it step by step, because it is different for every different model, depending on the purpose of it.

Here is it fixed:

Now, we have all "quads", except for one which had to be turned into a triangle, which with more work you could fix that into a quad as well.

Stick away from too many triangles as well, this can be a problem that can lead to star junctions.

If you've seen the first Shrek, pause frame when he is waving, or is working with his arm. If you're fast, and know what to look for - you can see black spots underneath his arms. This is because of star junctions, which give off weird results when smoothed for rendering.

That's it for now, I will be reviewing this and adding more as I see fit. Stay tuned for more information.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Approved.

 You must log in or register to reply here.


### Similar threads

[Lua]
 Polygons


Beckx

Mar 30, 2021

Lua Resources

Replies12Views5K

Mar 13, 2025
maneco2

Locked

 Lua - Polygons


Beckx

Mar 29, 2021

World Editor Help Zone

Replies2Views1K

Mar 30, 2021
Beckx

Locked

 Model Animations


davylink

Oct 14, 2005

Modeling & Animation

Replies2Views1K

Oct 16, 2005
Pike

[vJASS]
 [System] Polygon


Flux

Feb 23, 2016

JASS Resources

Replies47Views10K

Nov 3, 2016
Flux

Locked

 [Java] need help detecting line intersection


PrisonLove

Feb 3, 2011

Computer Tech

Replies1Views2K

Feb 3, 2011
Dr Super Good

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