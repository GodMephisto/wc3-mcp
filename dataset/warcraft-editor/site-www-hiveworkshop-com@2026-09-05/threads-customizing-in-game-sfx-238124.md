---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/customizing-in-game-sfx.238124/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Customizing in Game Sfx
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/customizing-in-game-sfx.238124/>
#### sniper_zero

#### sniper_zero

 Level 3

JoinedSep 9, 2009Messages658

Resources: 3

#### Resources
Spells 2Tutorials 1

Hi, I would just like to share something I find nifty. Some people may already know this but for those who don't, you're in luck because you'll learn something today...hopefully.

Before we start, you'll need to download Magos' Model Editor found here https://www.hiveworkshop.com/forums/tools-560/war3-model-editor-62876/. Now, on with the actual tutorial.

I'll be showing how to customize in-game SFX. What's the purpose of this? Well, there are times when an sfx would be perfect for a spell if it weren't for its colors or some other things. First, take a look at this. This the is the Doomtarget model and this is what we'll be using. Of course, you can choose a model you like.

It looks nice doesn't it? However, let's say you want an unholy or ice themed DoT spell. Making a spell like that would be aesthetically challenging as there aren't a lot of sfx options to choose from either in the models section or in the game itself. So we'll have to make our own using DoomTarget. Let's go over this step by step.

Step 1:

Open Magos, click the Windows tab and click MPQ Browser.

Step 2:

Find the path of your desire model. In our case it's DoomTarget. If you don't know how to find the path, then go to object editor, go to any ability or unit, double click the Art - Model File field and select whatever sfx you want to edit then press OK. Then double click again and look at the custom path.

Step 3:

Double click DoomTarget.mdx, then go to Windows -> Node Manager. You'll see small boxes with arrows. They're particle emitters and we'll be editing those.

Step 4:

Click on one of them, and you'll see this.

Click the "..." under segment 1. You'll be shown a color selection window. Choose any color which you think will suit your needs. Since I'm editing it with an unholy theme in mind, I'll choose green.

Now, take note here. It's best to match the shade of the new color with the shade of the original. In other words, for segment 1, the color is red, thus it's the darkest shade of color there is. Be sure to pick a color that's also dark in shading to match it.

Do this step for each segment of every particle emitter while keeping in mind to match the shading.

No picture here, as it's up to you what color you like to pick.

Step 5: Once you're done with editing the particle emitters, we'll now go to the geoset animation manager. It's also under the Windows tab. Doomtarget has two geoset animations. Double click on any one of them. You'll see this.

Click on the "..." again and edit it while keeping in mind shading. Sometimes, you'll see the color is just white. In that case, choose what you think is the best color as you'll really have no way of knowing how it'll turn out until you test your new model ingame. You can always edit your model after you test it ingame.

IMPORTANT TIP: Make sure you have use color checked. Otherwise, your edit won't show properly.

If you're done with the edits, you'll have something like these...

Now you have a pair of sfx which you can use for frost DoT or unholy and fel DoTs. I'll also attach the two models so you can further study them.

FINAL NOTE: For some models, like the AncientExplode model, you'll have to remove either particle emitters or textures. That's because models like the AncientExplode show leaves when they explode and that could get in the way of whatever theme you have in mind.

And that wraps up this tutorial. I hope it was helpful.

EDIT: How do you show attached pictures? I don't know how to do that which is why I'm currently linking the pictures to my photobucket account.

EDIT: Now uses the attachments as pictures instead of linking to another site.

#### Attachments

DoomTargetBlue.mdx

 5.8 KB

 · Views: 193



DoomTargetGreen.mdx

 5.8 KB

 · Views: 190



Doom.png

 71.2 KB

 · Views: 1,203



Doom Path.png

 29.8 KB

 · Views: 1,091



MPQ Browser.png

 22.9 KB

 · Views: 1,163



Geoset Animation.png

 12.1 KB

 · Views: 1,110



Node Manager.png

 95.8 KB

 · Views: 1,167



DoomTargets.png

 117.8 KB

 · Views: 1,137



Particle Emitter.png

 42.5 KB

 · Views: 1,155




 Last edited: Jul 19, 2013

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

very useful and simple even if I'm not a modeler!

rep+

#### Veritas 117

#### Veritas 117

 Level 14

JoinedNov 17, 2010Messages1,266

sniper_zero said:

 How do you show attached pictures? I don't know how to do that which is why I'm currently linking the pictures to my photobucket account.


Click to expand...


To attach pictures just click on the little paper clip at the top called attachments. (make sure to 'go advanced' so all the little tools show up) Now you can manage your attachments. I think you can upload 5 at a time, but once they are loaded you can keep uploading more.


Hope this helps, also that tutorial was very helpful, I've been toying with the model editor so I have been wanting a tutorial like this.

#### sniper_zero

#### sniper_zero

 Level 3

JoinedSep 9, 2009Messages658

Resources: 3

#### Resources
Spells 2Tutorials 1

I'll get to it once I have access to my computer.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great tutorial. I'm sure this will be useful.

Approved.

#### MN Lahmar

#### MN Lahmar

 Level 22

JoinedJul 28, 2013Messages768

Resources: 25

#### Resources
Models 16Icons 6Skins 1Maps 1Reforged HD Skins 1

Thanx for this tutorial, it helped me alot.

 You must log in or register to reply here.


### Similar threads

 Teamcolor Particles


Anvil

Nov 28, 2012

3D Modeling Tutorials

Replies4Views7K

Dec 19, 2012
Anvil

 How to make XY Quad Particles Emitter ( And Rain Drop Effect Tutorial )


Anvil

Aug 27, 2012

3D Modeling Tutorials

Replies10Views14K

Jul 9, 2017
Veronnis

 Model Editing


Fabioz

Jan 18, 2009

3D Modeling Tutorials

Replies45Views66K

Oct 10, 2021
KaiZen7th

Locked

 Making Effects in Magos


WILL THE ALMIGHTY

Jun 10, 2007

3D Modeling Tutorials

Replies21Views26K

Jul 1, 2009
Pyritie

 Scratch-Made Weapon Attachments with Particle Emitters


Fabioz

Apr 8, 2008

3D Modeling Tutorials
2

Replies52Views42K

Oct 13, 2010
Ikithe

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