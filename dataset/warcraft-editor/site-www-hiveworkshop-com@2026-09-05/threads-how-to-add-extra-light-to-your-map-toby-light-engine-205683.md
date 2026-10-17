---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-add-extra-light-to-your-map-toby-light-engine.205683/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Add Extra Light to Your Map Toby Light Engine
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-add-extra-light-to-your-map-toby-light-engine.205683/>
#### tobyfat50

#### tobyfat50

 Level 17

JoinedSep 19, 2011Messages829

Resources: 6

#### Resources
Models 4Maps 1Tutorials 1

DESCRIPTION


This is the method I use in my maps to add some extra light. The effect that you get from using it is similar to modern light engines. However I am not suggesting it's actually a "Engine" but an effect. It also has its downs but if used properly you can produce some really beautiful results.

This technique works perfectly well on dark maps whit no fog, could be useful in caves and such.


--------------------------------------------------------------------------

REQUIRED TOOLS

https://www.hiveworkshop.com/forums/tools-560/war3-model-editor-62876/


--------------------------------------------------------------------------

STEP BY STEP

Step 1


Open a model in the model editor.

Step 2


Go to material editor and enter the first material, create new material layer in the same material and click ok.

Step 3


In the 2sd material you crated use same texture and ad additive filter, click ok.


You can also use a map for better results for example leaves materials with transparent textures need a "s_map" otherwise the leaves just look whitened.

(explanation to be added later)

Step 4


You you should now have 2 material layers in the parent material. 1 is for difuse texture, 2 is the additive (under 1st).

Finish!


--------------------------------FINISH------------------------------------

DISADVANTAGES

Unfortunately, this method has a small glitch, Fog does not display on additive materials so u have to be careful when using this technique.


--------------------------------------------------------------------------

THAT'S IT HAVE FUN!


 Last edited: Nov 18, 2011

#### noob

#### noob

 Level 14

JoinedSep 28, 2011Messages968

Resources: 5

#### Resources
Maps 4Tutorials 1

Good tutorial.this is usefull.


But it is WC3 engine not yours engine so the tutorial name is incorrect

#### tobyfat50

#### tobyfat50

 Level 17

JoinedSep 19, 2011Messages829

Resources: 6

#### Resources
Models 4Maps 1Tutorials 1

I know that! but I couldn't help myself so I had to name it somehow.

Don't bash me please!

#### noob

#### noob

 Level 14

JoinedSep 28, 2011Messages968

Resources: 5

#### Resources
Maps 4Tutorials 1

Because you called it toby light engine no one understand this is a warcraft 3 tutorial, think it is a new game and does not see this tutorial


 Last edited: Nov 17, 2011

#### tobyfat50

#### tobyfat50

 Level 17

JoinedSep 19, 2011Messages829

Resources: 6

#### Resources
Models 4Maps 1Tutorials 1

Oh! I'm such a dummy!

Can I change the name?

-------------------------------------------------------------------------------------------

Wait! but I cleary wrote "How to add extra light to your map! (Toby Light engine)" [Mapping]

GAH! this is so annoying!


 Last edited: Oct 21, 2011

#### RiotZ

#### RiotZ

 Level 22

JoinedJul 25, 2009Messages3,087

Resources: 3

#### Resources
Maps 2Tutorials 1

Epic. This will be perfect for Black Winter.

#### tobyfat50

#### tobyfat50

 Level 17

JoinedSep 19, 2011Messages829

Resources: 6

#### Resources
Models 4Maps 1Tutorials 1

Well, the tutorial is pretty simple to understand and using the technique won't drain space or resources so I don't see a reason why anybody can't use it. go for it! but I don't know how well it match in Black Winter. From the look of your project you have a lot of gray models and you will surely get a ugly whitening effect, I would suggest you use "s_maps"!!!

#### RiotZ

#### RiotZ

 Level 22

JoinedJul 25, 2009Messages3,087

Resources: 3

#### Resources
Maps 2Tutorials 1

tobyfat50 said:

 Well, the tutorial is pretty simple to understand and using the technique won't drain space or resources so I don't see a reason why anybody can't use it. go for it! but I don't know how well it match in Black Winter. From the look of your project you have a lot of gray models and you will surely get a ugly whitening effect, I would suggest you use "s_maps"!!!


Click to expand...


That is not the map I'm using it for, I'm rebooting it.

#### Softmints

#### Softmints

 Level 14

JoinedJun 3, 2005Messages209

Resources: 11

#### Resources
Models 1Maps 9Tutorials 1

This is a pretty neat technique; note that it can also be done (with some care) by modifying the texture in an image editing suite and overlaying the additive effect. This means you only have to import a texture, not a new model as well.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It is indeed a pretty nice trick.

 In the 2sd material you crated use same texture and ad additive filter, click ok.

You can also use a map for better results for example leaves materials with transparent textures need a "s_map" otherwise the leaves just look whitened.

(explanation to be added later)


Click to expand...

You should add that explanation.

#### tobyfat50

#### tobyfat50

 Level 17

JoinedSep 19, 2011Messages829

Resources: 6

#### Resources
Models 4Maps 1Tutorials 1

Yes Thank you! (Toby Light engine) is just the name of this showcase and the technique I used, although it's not my personal creation. It's a single property you apply to a model. Apparently some other people are using it, as well as some better techniques.

I will add the explanation!

Your tutorial are great, I can learn from them a lot!

This is my first tutorial so...

#### Talavaj

#### Talavaj

 Level 17

JoinedJan 18, 2010Messages1,122

Resources: 6

#### Resources
Models 6

Softmints said:

 -snip


Click to expand...

while you can, technically get similar result with just color dodge/brightness/contrast image tweaks under certain circumstances (unshaded mesh). the final result will always look worse than additive blending.


that is because global/omni lightning and mesh/vertex shading reacts to additive blending differently compared to regular blending.


in short, additive white stays white and is not affected by global lighting (such as during in-game night) and even if you are jesus you won't achieve a result similar to this with just a regular texture. thus additive blend gives much richer color range than just using a regular texture, downside of which is essentially duplicating the amount of polygons.


with a little clever usage of this technique (and modulate blending) you can easy create spectacular HDR imagery even within this shitty game engine.

#### tobyfat50

#### tobyfat50

 Level 17

JoinedSep 19, 2011Messages829

Resources: 6

#### Resources
Models 4Maps 1Tutorials 1

Yes it's true what you said. I can't really get your technique in lighting mesh as it's still a mystery to me how you do it but I tell you this, I do not use duplicate mesh models because I don't want to slow game playing quality so I just stick to the aditive material. Currently I have fixed the whitening problem I just changed the colour of the s_map a little bit.

Yes I was using modulate blending as well but am not using it anymore.

#### Talavaj

#### Talavaj

 Level 17

JoinedJan 18, 2010Messages1,122

Resources: 6

#### Resources
Models 6

@toby

if I'm not mistaken, each layer in a material actually raises the polygons that are rendered by the amount of polygons of the mesh that it is applied to.

so a geoset with 100 polys and 3 additive layers is actually 400 polygons rendered in game. (so does making the mesh two-sided)

#### tobyfat50

#### tobyfat50

 Level 17

JoinedSep 19, 2011Messages829

Resources: 6

#### Resources
Models 4Maps 1Tutorials 1

Are you sure?! I saw no differences especially in size or polygons when I checked a model.

#### Blood Raven

#### Blood Raven

 Level 47

JoinedAug 1, 2010Messages1,740

Resources: 235

#### Resources
Models 144Icons 53Packs 7Skins 13Maps 16Templates 1Tutorials 1

looks nice, but i have one suggestion: remove your siganture, it is in the tutorial rules. If you dont know how, Edit> Uncheck the box "Show signature" > save

otherwise, really great, your terrains are nice

#### tobyfat50

#### tobyfat50

 Level 17

JoinedSep 19, 2011Messages829

Resources: 6

#### Resources
Models 4Maps 1Tutorials 1

yes thank you!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Cool technique overall to give your map a certain kind of contrasty-feel.

~Approved.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Cool tutorial

#### NOSAKIAS

#### NOSAKIAS

 Level 3

JoinedApr 10, 2009Messages32

Resources: 1

#### Resources
Maps 1

Amazing!

#### OVOgenez

#### OVOgenez

 Level 38

JoinedJul 7, 2019Messages382

Resources: 27

#### Resources
Models 24Packs 2Spells and Systems 1

!NECROPOST!

As Talavaj mentioned, u can also use Modulate/Modulate2x blending to achive interesting results.

Important thing that light affects each layer separately.

For example, how this Additive trick works:

C = To*L + Ta*L

here, To is first Opauqe layer (diffuse texture), Ta is second Additive layer (diffuse texture), and L is light modificator,

then C is result of adding colors.

Because To and Ta have identical textures we can mathematically represent that

y = x*L + x*L = 2*x*L

BUT, as mentioned, Additive layers are rendering over fog...

So, u can avoid this glitch by using Modulate2x mode instead:

C = To*L * Wmd*2

here, To is first Opauqe layer (diffuse texture), Wmd is second Modulate2x layer (unshaded White.blp texture), and L is light modificator,

then C is result of multiplying colors.

Because Wmd is fully white texture we can mathematically represent that

y = x*L * 1*2 = 2*x*L

2*x*L = 2*x*L, so To*L + Ta*L = To*L * Wmd*2

 Original/Modified

If u want, u can disable unshaded flag to make Wmd layer dependent on light:

C = To*L * Wmd*2*L => y = x*L * 1*2*L = 2*x*L²

This means the light modifier will be applied to the material twice, tinting it even more.

 Original/Modified

Also to make model more glisten u can add third layer of unshaded diffuse texture with Modulate2x (or just Modulate) mode:

C = To*L * Wmd*2*L * Tmd*2 => y = x*L * 1*2*L * x*2 = 4*x²*L²

This means the material will have more contrast.

 Original/Modified

#### Attachments

Grave0.mdx

 10.3 KB

 · Views: 7




 Last edited: Jul 14, 2026

 You must log in or register to reply here.


### Similar threads

 Putting what you learned about Animated Textures into action


Spacebuns

Dec 15, 2015

3D Modeling Tutorials

Replies3Views4K

Jun 29, 2026
Spacebuns

 How to make cool portraits without needing a separate file


Spacebuns

Sep 5, 2014

3D Modeling Tutorials

Replies11Views11K

Nov 27, 2017
Diegoit

 Creating Ghosts


Daelin

Jul 14, 2005

3D Modeling Tutorials

Replies10Views27K

Aug 14, 2023
DREADMAN

 Add Multiple Skins for a Single Model


Sylvie

Feb 17, 2015

3D Modeling Tutorials

Replies34Views18K

Mar 20, 2021
Em-Rock

[2D Art]
 How to Add Team Color To Textures


stan0033

Aug 3, 2026

Tutorial Submission

Replies0Views127

Aug 3, 2026
stan0033

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