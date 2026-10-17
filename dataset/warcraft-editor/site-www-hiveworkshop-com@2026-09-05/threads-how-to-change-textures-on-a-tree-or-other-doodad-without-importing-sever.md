---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-change-textures-on-a-tree-or-other-doodad-without-importing-several-models.222212/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Change Textures on a Tree or Other Doodad Without Importing Several Models
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-change-textures-on-a-tree-or-other-doodad-without-importing-several-models.222212/>
#### InfernalTater

#### InfernalTater

 Level 26

JoinedJul 12, 2008Messages3,123

Resources: 43

#### Resources
Models 35Icons 3Packs 1Skins 1Tutorials 3

How to Change Textures on a Destructible

For The Impatient:

For those of you who are experienced or impatient, you don't need to read through all of this talking. Text that is italicized will usually be either discussion, or details. Main points will not be italicized, so impatient users can easily skip through further explanations and such that they may not need to read in order to understand the main points.

Introduction

Some players enjoy having several types of environments on their maps, and want good-looking trees for each type of environment. As a result, they import a new model for each and every tree.

It's actually possible to change the textures used on one tree, so this isn't necessary.

Using Replaceable Textures

Before you even import any models, know that you can actually make some nice looking trees in-game simply by creating new Ashenvale trees with different replaceable textures. Why Ashenvale? Because those trees have branches, making them the most realistic, and they can actually look pretty nice with other textures.

How will I do this?

Open the Object Editor. Click the Destructibles tab. Click the Ashenvale tree. Copy it. Paste it So that your new tree will not replace the old one.

Look for Art - Replaceable Texture File. Change this to whatever texture you want to apply to it. Some of the textures, such as those used on bridges or mushrooms, won't wrap properly, but normal tree textures will. Change your Editor - Tilsets to whatever tilsets you want it to appear on. Or just change it to All, I find that to be more convenient.

Using Imported Trees With Different Textures

Odds are, if you're reading a tutorial about custom trees, you're probably not satisfied with the in-game models. If you're willing to import a model, you can make your terrain look much better. These next few steps will show you how to change a custom model to use any texture.

Getting Ready

You will need:

A Tree Model

[thread=62876]Magos' War3 Model Editor[/thread]

Blizzard's Warcraft III World Editor

For this example, I will be using the [thread=114724]Snow Pine[/thread] model.

Be sure to save a copy of the original model just in case anything goes wrong.

Open your tree model in War3 Model Editor by clicking File => Open and finding your model.

Changing the Texture

Click Windows => Texture Manager.

Right click in the box, and click Create Empty. You should see a new texture in the list, named Replaceable ID 0.

Right click the new Replaceable ID 0, and click Edit.

In the new box, towards the middle you should see: Replaceable ID, and beneath it the text ID: with a type-in box with 0 currently entered. Change that 0 to 100 or whatever value you prefer; just make sure it's not a value used by anything else, and make sure you will either remember the value, or have the value recorded somewhere for reminding yourself.

Determine which texture the leaves are using. For most trees, there will only be one or two textures, and it will be easy to see which one is the texture for the leaves. Most trees use in-game tree textures for the leaves. Make sure you remember this texture for later.

Click Window => Material Manager.

Determine which material is using the texture that the leaves are using. Do this by double-clicking each material, clicking the material layer in the next box, and then checking the first arrow-box to see which texture it is using.

When you have found which material uses the texture, change the texture that it uses to your new Replaceable ID 100 by clicking the arrow and changing the texture.

If you have done this correctly, you will notice that the leaves of the tree are now simply blank rectangles everywhere. This is normal; they will be replaced in-game.

Now that you've done this, you can remove the previous texture that the leaves were using, in order to reduce the file size. Remove the previous texture by going back to the Texture Manager, right-clicking that texture, and clicking remove. If a warning box pops up, that means that another Material layer is using the texture, so you'll need to repeat the above steps to change the other material layers to the Replaceable ID, until you can remove the texture without a warning box.

When all of this is done properly, save the new model.

Implementing the New Model

Import the new model into your map by opening up the World Editor, opening your map, clicking Module => Import Manager, right clicking in the empty box, clicking Import File, and finding your tree.

Once you've imported the model, you will need to save your map and restart the editor because imported content needs to be loaded upon the initiation of the editor.

Now that you've done this, simply copy and paste a barrel. I use them just in case, since they are not set up with multiple variations, and other reasons. Variations can be changed on any destructible.

Change the model of the barrel to your imported tree. Your imported model will not be in the normal list of models. To the left of the massive list of models, you will see three words with click bubbles next to them: Preset, Import and Custom. Be sure to click the box next to Import, and find your tree model.

Now, set the Art - Replaceable Texture to any tree texture, set the Replaceable Texture ID to 100, and then enjoy your new tree.

You may also want to change other values, such as the tileset, or, for advanced terrainers, setting the Pathing Texture to Mushroom to allow for placement without collision.

Now, you've got a bunch of high-quality trees with only one tiny import! These methods can be applied to any sort of decoration.

Remember, if you edit someone else's tree to use in your map, you should still not take the credit for it in your map, they made the original! They still did all of the work on mesh and wrap, and they deserve the credit.

Using This Method for 2-D Models

These steps can also be used to create as many 2-D models as you want, and only have to import one model.

How? Open any image file in the War3 Model Editor. Preferably an icon, so that it will be square, but you can also open any image of the proportions that you need, and a 2-D plane will be created with those proportions.

Go through the same steps listed above to change the texture with a Replaceable ID, save it, import it, and such.

Now you have a square that you can apply any texture you want to. This is useful for creating interfaces, with as many clickable icons as you want, simply by setting the Replaceable Texture to any icon's texture path.

Adapting This Method for Unit Models

Check out Ender's similar "How to make a model change textures ingame" tutorial to learn how to use this method combined with the War Club ability to change the textures of units in-game.

You can also check out Shadow Flux's helpful tutorial Change Model Texture Using Alternate Animations to learn how to edit a model to use different textures during different animations.

I hope you can use this knowledge to fit untold amounts of content into your maps!


 Last edited: Jul 3, 2015

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Good job overall, this is pretty useful.

Approved.

#### clockwork2

#### clockwork2

 Level 15

JoinedJan 26, 2010Messages1,629

Resources: 4

#### Resources
Models 2Maps 2

Does this method work with units?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

You can use a similar method as this combined with War Club to make it work with units.

https://www.hiveworkshop.com/forums...how-make-model-change-textures-ingame-222258/

Or you can use alternate animations:

https://www.hiveworkshop.com/forums...el-texture-using-alternate-animations-264323/

#### InfernalTater

#### InfernalTater

 Level 26

JoinedJul 12, 2008Messages3,123

Resources: 43

#### Resources
Models 35Icons 3Packs 1Skins 1Tutorials 3

PurgeandFire said:

 You can use a similar method as this combined with War Club to make it work with units.

https://www.hiveworkshop.com/forums...how-make-model-change-textures-ingame-222258/

Or you can use alternate animations:

https://www.hiveworkshop.com/forums...el-texture-using-alternate-animations-264323/

Click to expand...

I didn't know this. This is quite useful! I'll add your links to the bottom of my tutorial in case others might find this information equally valuable.

#### clockwork2

#### clockwork2

 Level 15

JoinedJan 26, 2010Messages1,629

Resources: 4

#### Resources
Models 2Maps 2

thanks purge and infernal!

#### fostaa

#### fostaa

 Level 6

JoinedMar 18, 2011Messages109

Still can't understand how to make a new custom tree and just change its model properly into one of the models in Hive.

Sorry, but instructions were not clear.

 You must log in or register to reply here.


### Similar threads

 New Trees in World Editor


Tails96

Nov 18, 2009

Misc/General Mapping Tutorials

Replies24Views34K

Oct 4, 2018
Raka Uthada

 How to make a model change textures ingame


Ender

Sep 4, 2012

Misc/General Mapping Tutorials

Replies8Views16K

May 27, 2021
maddeem

 Fixing Invisible/White Tree Models


Slaydon

Jul 22, 2009

Misc/General Mapping Tutorials

Replies5Views10K

Jan 21, 2026
Fiko13

 Animated Loading Screens


N00byStance

Dec 27, 2005

Misc/General Mapping Tutorials

Replies1Views27K

Oct 23, 2013
Gamegear

 (Pseudo-update to skin how-to) How to: import a replacement texture


BogdanW3

Jan 6, 2022

Tutorial Archive

Replies1Views3K

Jan 10, 2022
Chaosy

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