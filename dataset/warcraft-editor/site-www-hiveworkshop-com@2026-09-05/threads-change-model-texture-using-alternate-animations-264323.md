---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/change-model-texture-using-alternate-animations.264323/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Change Model Texture Using Alternate Animations
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/change-model-texture-using-alternate-animations.264323/>
#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

 Change Model Texture Using Alternate Animations

It is indeed possible to change a model's texture in game whether in object editor or via Triggers  without using WAR CLUB ability.

CONTENTS:

1. Pros and Cons

2. Modeling

 2.1 Outcome

3. Triggering.

4. Make a unit use different texture without Triggering

5. Sample Map

6. Multiple Destructable Texture one Model (Different Tutorial)

What will I do?

I will make a unit (Jaina Proudmoore) change texture in game whenever she obtained an item. In Normal form, she will use the Holy Mage Texture and when she acquired a certain item, she will change into a Forsaken Jaina. Even users with no experience using Magos War3Model Editor and Retera's Matrix Eater can follow and learn from this tutorial.

 Required Tools


Magos War3 Model Editor

Retera's Matrix Eater

 Optional Tool


MDX Pather

1. Pros and Cons

PROS:

1. Allows Changing of Texture via Trigger and at the same time change animation and particles if necessary.

 Example

 - Your unit will alternate between fire and ice elemental, the particles on the unit can also be changed)

 - Your unit will alternate between normal form and demon form, the demon form can have alternate stand, attack, etc. animation different from normal form.

2. Portrait Model also change.

3. A single model file for 2 different units. Help save file size without importing the same model with only a different texture.

4. Allows up to 3 alternate texture (using "upgrade", "fast", "alternate") if I'm not mistaken.

5. Easy to trigger and does not conflict with War Club Ability.

CONS:

1. Cannot be applied on models in the MPQ. You need to import a new model with modified animations.

2. Increased the model's size depending on the Model's copied animation.

2. Modeling

1. First find the model you will modifiy using Magos War3 Model Editor MPQ Browser

(Go to Windows>MPQ Browser)

2. Save it as an MDL file

3. Make a copy

4. Open Retera's Matrix Eater Animation Transfer

(Go to File>Simple Import)

5. In the Animation Transfer, copy the animation using the MDL file you saved and its copy

6. Copy all remaining animations, but to do it easier, set the the BASE FILE the same as the OUTPUT FILE.

7. After all of that is done, you should have something like this

(Go to Windows>Animation Controller)

8. Now Rename all the animation copies you made by adding the keyword "Alternate" to the animation name and take note of the "Interval From: "

It is best to write them in NotePad like in the screenshot

(Go to Windows>Sequence Manager)

9. Now Import the New Alternate Texture and take note of how many texture's are there. The ForsakeJaina.blp is the 4th Texture.

(Go to Windows>Texture Manager)

10. Go to Materials Manager and select each Materials there.

(Go to Windows>Material Manager)

11. Check the Texture ID Animated, click the Texture ID button and input there the animation key frame of all alternate animations.

In the right, put the texture's sequence number (which is 4)

It is important that the numbers are arranged from lowest to highest.

12. (OPTIONAL) You can change the default skin to something custom using MDX Pather

13. Create a Camera for the Portrait. Right-click and select 'View Camera' to show the camera's current view.

(Windows>Camera Manager)

14. Or you can also edit the Portrait Model. Note that sometimes you have to put all the animation frames in the Texture ID Animation as shown in the Screenshot.

2.1 Outcome

3. Triggering

To change the unit's texture model via trigger, we use this function:

Animation - Add the upgrade alternate tag to UNIT
To change back just use

Animation - Remove the upgrade alternate tag to UNIT

4. Make a unit use different texture without Triggering

To make a unit use the alternate texture from the start, just edit the:

Art - Required Animation Names to "alternate".

in the Units Tab of Object Manager.

5. Sample Map

Changing Texture.w3x

The sample map has the model and the textures.

CREDITS for the Textures:

Apheraz Lucent

Leopard

6. Multiple Destructable Texture one Model (Different Tutorial)

To use one model file for destructables with more than one texture, check out InfernalTater's How To Change Textures on a Tree or other Doodad (Without importing several models).

#### Attachments

2.JPG

 39.4 KB

 · Views: 1,601



3.JPG

 11.1 KB

 · Views: 1,549



4.jpg

 26.4 KB

 · Views: 1,540



5.JPG

 29.3 KB

 · Views: 1,549



6.JPG

 32.9 KB

 · Views: 1,554



7.jpg

 56.4 KB

 · Views: 1,549



8.JPG

 75.4 KB

 · Views: 1,537



9.JPG

 75.2 KB

 · Views: 1,535



10.JPG

 63.7 KB

 · Views: 1,569



11.JPG

 60.3 KB

 · Views: 1,512



12.JPG

 22.4 KB

 · Views: 1,492



21.JPG

 38.4 KB

 · Views: 1,559



22.JPG

 39 KB

 · Views: 1,546



13.JPG

 56.5 KB

 · Views: 1,518



14.JPG

 48.3 KB

 · Views: 1,510



Changing Texture.w3x

 337.6 KB

 · Views: 783




 Last edited: Jul 3, 2015

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

This certainly is interesting. However, the usage feels limited. But either way, there are some really neat usages with it.

Rep+, I am sure I will find this useful if I ever start another project.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

I have searched for related tutorials and a lot of people wants to change the model's texture via triggers. if i know how to easily copy animations instead of frame by frame, we can have any model in the mpq have alternating texture by copying its animation to an upgrade tag animation. The only downside is to import the model in the map.

In fact, people believed it was impossible according to this post Here

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

That's pretty stupid to say that it's impossible. Just look at Grom Hellscream, he got the red skin and the green one.

However, it's indeed impossible with purely triggers.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Chaosy said:

 That's pretty stupid to say that it's impossible. Just look at Grom Hellscream, he got the red skin and the green one.


However, it's indeed impossible with purely triggers.


Click to expand...


Wait, isn't Grom Hellscream have 2 model files? One for the green and one for the red. The post I linked says that it is impossible to have interchanging texture using one model. As Dr SuperGood suggested, he will have to import a separate model file which is not very efficient since it is the same model only with different Texture but it is a working solution to Miles111's problem.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Check Grom's model's stand fourth animation. That shows us that you can do it with animations

#### WhiteFang

#### WhiteFang

 Level 21

JoinedJul 6, 2014Messages6,786

I didn't know this,its very usefull.Thanks for sharing

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Good to know you found it useful archimonde supreme.


 Last edited: Apr 5, 2015

#### jonbon29

#### jonbon29

 Level 13

JoinedAug 19, 2014Messages1,111

Yeah this is very useful, indeed it gives a nice way to change textures.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Tutorial completely modified. Now users with no knowledge in Modeling can follow and learn from the tutorial.

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

@Chaosy, Grom has two models. That's just a morph animation of the green one, you're wrong in there. That example and this system aren't a same thing. Your example is like a Hyppogryph Archer, where a unit HAS to be completely replaced in map. This is something more similar to Druid of the Talon Morph, where the unit changes and holds on to all buffs, items, etc.

All in one, a fantastical and very useful tutorial.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

@Aph

Sorry if you missunderstood. But in that morph animation the skin is changing, aka the texture is changing. I just stated that that specific animation shows that it can be done

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Excellent tutorial. Great organization--I love that you added an introduction and the pros/cons list. The method certainly works, and it is great that you added the information on copying the animations & creating the camera.

Approved. I may make some spelling/grammar changes later on, but I'll notify you of them when that time comes. At the moment, it is perfectly readable and well organized, so I'll go ahead and move it.

EDIT: I moved it to 3D modeling tutorials since the majority of the tutorial deals with model editing. As for the title, I was thinking: "Change model texture using alternate animations", what do you think?

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Yeah that's a good title suggestion, please change it.

#### InfernalTater

#### InfernalTater

 Level 26

JoinedJul 12, 2008Messages3,123

Resources: 43

#### Resources
Models 35Icons 3Packs 1Skins 1Tutorials 3

Thank you very much for posting this tutorial. I hope people use the shit out of it.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

InfernalTater said:

 Thank you very much for posting this tutorial. I hope people use the shit out of it.

Click to expand...


I hope so. It goes great with your tutorial and linking this in your tutorial was a brilliant idea.

#### InfernalTater

#### InfernalTater

 Level 26

JoinedJul 12, 2008Messages3,123

Resources: 43

#### Resources
Models 35Icons 3Packs 1Skins 1Tutorials 3

Shadow Flux said:

 I hope so. It goes great with your tutorial and linking this in your tutorial was a brilliant idea.


Click to expand...


Yeah, I thought anyone who was helped by my tutorial would definitely be interested in yours. Thank Purge for his post on my tutorial linking your tutorial and someone else's tutorial while answering someone's post with a question about my tutorial vw_death. You can link mine at the bottom of yours, too, if you want.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

InfernalTater said:

 Yeah, I thought anyone who was helped by my tutorial would definitely be interested in yours. Thank Purge for his post on my tutorial linking your tutorial and someone else's tutorial while answering someone's post with a question about my tutorial vw_death. You can link mine at the bottom of yours, too, if you want.


Click to expand...


Yeah I'll link yours too when I'm on my PC.

#### TinkerLord

#### TinkerLord

 Level 44

JoinedOct 20, 2010Messages2,934

Resources: 104

#### Resources
Models 83Icons 17Spells and Systems 3Tutorials 1

 CONS:

1. Cannot be applied on models in the MPQ. You need to import a new model with modified animations.

2. Slightly increased the model's size due to the added animation.

Click to expand...

This is sure a useful tutorial some people

but one thing, I personally think the model size will increase alot rather than Slightly, that's just my opinion

But still, Good job

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Hayate said:

 This is sure a useful tutorial some people


but one thing, I personally think the model size will increase alot rather than Slightly, that's just my opinion


But still, Good job

Click to expand...

Yeah, animations make up basicly 80% of the model size, so you don't save much with animation duplicates.


But it's great for models with a low number of animations, like vehicles or special effects.

#### TinkerLord

#### TinkerLord

 Level 44

JoinedOct 20, 2010Messages2,934

Resources: 104

#### Resources
Models 83Icons 17Spells and Systems 3Tutorials 1

Shadow Flux said:

 Yeah I'll link yours too when I'm on my PC.


Click to expand...

Zwiebelchen said:

 Yeah, animations make up basicly 80% of the model size, so you don't save much with animation duplicates.

But it's great for models with a low number of animations, like vehicles or special effects.


Click to expand...

yes... Im not sure, but I think animation indeed is the major reasons for the Filesize,

And Yeah... Anim like Vehicle that Don't have too much movement, or Trees, Item, doodad building, wall, ward, and Rocks or Projectile and Such would work perfectly

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Hayate said:

 This is sure a useful tutorial some people


but one thing, I personally think the model size will increase alot rather than Slightly, that's just my opinion


But still, Good job

Click to expand...


Oh you're right. The first time I wrote this tutorial, the model I worked with only had a Birth, Stand and Death animation so I forgot to change that.

#### TinkerLord

#### TinkerLord

 Level 44

JoinedOct 20, 2010Messages2,934

Resources: 104

#### Resources
Models 83Icons 17Spells and Systems 3Tutorials 1

Also, Anyway, A little Trick here, If you want the Model is like a Improved version or Tweaked Version which are meant to replace the existing model, (Like Ujimasa's Model or small edit, or Fixes) and they have separated portrait file, You can Import the Model as the same Filepath as the model you want to replace, and it will use it's portrait file, so that you don't need to import portrait that take up filesize

(Bump this down so that you won't missed this)

Example, A Improved Grunt model, You can Import it as Units\Orc\Grunt\grunt.mdx and It will use Units\Orc\Grunt\grunt_portrait.mdx as portrait

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Hayate said:

 Also, Anyway, A little Trick here, If you want the Model is like a Improved version or Tweaked Version which are meant to replace the existing model, (Like Ujimasa's Model or small edit, or Fixes) and they have separated portrait file, You can Import the Model as the same Filepath as the model you want to replace, and it will use it's portrait file, so that you don't need to import portrait that take up filesize


(Bump this down so that you won't missed this)


Example, A Improved Grunt model, You can Import it as Units\Orc\Grunt\grunt.mdx and It will use Units\Orc\Grunt\grunt_portrait.mdx as portrait


Click to expand...


But if you only import the model and not the portrait, the model won't fit with the portrait. Example, I import Jaina model to Units\Orc\Grunt\grunt.mdx, her portrait will be Grunt which clearly don't fit with the Jaina model.

#### TinkerLord

#### TinkerLord

 Level 44

JoinedOct 20, 2010Messages2,934

Resources: 104

#### Resources
Models 83Icons 17Spells and Systems 3Tutorials 1

Shadow Flux said:

 But if you only import the model and not the portrait, the model won't fit with the portrait. Example, I import Jaina model to Units\Orc\Grunt\grunt.mdx, her portrait will be Grunt which clearly don't fit with the Jaina model.


Click to expand...

I mean, If you Make a Small tweak that is Meant to replace the Existing Grunt, For example, A grunt with Better animation or A Better Axe or whatever,

Or maybe, If you have a Mounted Jaina, And you won't be using any Jaina on Foot Model, You can just Import the Mounted Jaina As the Jaina's Path, and it will use that Portrait, (Tho it's pointless if the Model itself already have Portrait Include, Like Catapult or Bristleback)

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Hayate said:

 I mean, If you Make a Small tweak that is Meant to replace the Existing Grunt, For example, A grunt with Better animation or A Better Axe or whatever,


Click to expand...


Ahh ok. I get what you mean

#### Shar Dundred

#### Shar Dundred

 Level 81

JoinedMay 6, 2009Messages6,446

Resources: 15

#### Resources
Packs 1Skins 1Maps 12Tutorials 1

##
 Legends of Arkain Series


I hate being the one reviving in this thread, but I got a question:

Is it possible to combine two models the same way, even when they're using different animations? Like the Demon Hunter and its alternate, I mean.

Example:

I'd love to combine the standard Human Barracks model with Ujimasa's several Human Barracks. Problem: Ujimasa isn't using skins, but ingame textures. Also, the models differ slightly in some points.

Is it still possible?

#### Ardenian

A

#### Ardenian

Yes, but I think it is different than this method. You would add Geoset Animations to hide the whole models not used.

It is the same like the Scout Tower, that is an in-built model. Every upgrade is, like the town halls and so on.

#### Emm-A-

#### Emm-A-

 Level 14

JoinedJul 1, 2008Messages1,314

Resources: 1

#### Resources
Spells 1

This is an excellent tutorial! It is very easy to follow as it explains everything to great detail.

Unfortunately, the model I tried it with, does not work ingame despite showing the correct textures in magos at the end.

Anyway I think this tutorial is very useful! Thank you for sharing!

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Which model you tried and what went wrong? Let us help

#### Emm-A-

#### Emm-A-

 Level 14

JoinedJul 1, 2008Messages1,314

Resources: 1

#### Resources
Spells 1

Apheraz Lucent said:

 Which model you tried and what went wrong? Let us help

Click to expand...


How kind of yours!  Would be great, if you took a look!


So, I used the elfenwood pine by Nasrudin and tried to give it a snowy alternate texture.


It worked perfectly up to the magos part, so I can see the pine having a snowy alternate form. Unfortunately, when I change animations ingame, the pine just looses its leaves  ... I mean, its pretty obvious that I made a mistake somewhere, but I cant find it :/

This is the thread, where I posted this issue originally (last post), and this is the modified pine model with the new snowy texture (and orig texture).


Hope, you can see, what went wrong?

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Emm-A- said:

 How kind of yours!  Would be great, if you took a look!


So, I used the elfenwood pine by Nasrudin and tried to give it a snowy alternate texture.


It worked perfectly up to the magos part, so I can see the pine having a snowy alternate form. Unfortunately, when I change animations ingame, the pine just looses its leaves  ... I mean, its pretty obvious that I made a mistake somewhere, but I cant find it :/

This is the thread, where I posted this issue originally (last post), and this is the modified pine model with the new snowy texture (and orig texture).


Hope, you can see, what went wrong?

Click to expand...


So you're using it as a unit? And it doesn't work on Magos for me.


If you're gonna use destructable, refer to this tutorial

#### Emm-A-

#### Emm-A-

 Level 14

JoinedJul 1, 2008Messages1,314

Resources: 1

#### Resources
Spells 1

Flux said:

 So you're using it as a unit? And it doesn't work on Magos for me.


If you're gonna use destructable, refer to this tutorial

Click to expand...


I want to use it as a doodad, and change the animation via the SetDoodadAnimationRect native.


Its strange that you cant see it, because in my magos editor, I can see the snowy texture (SnowyPines) on the tree..

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Emm-A- said:

 I want to use it as a doodad, and change the animation via the SetDoodadAnimationRect native.


Click to expand...

What do you input in the SetDoodadAnimationRect?

Emm-A- said:

 Its strange that you cant see it, because in my magos editor, I can see the snowy texture (SnowyPines) on the tree..


Click to expand...

Maybe you uploaded the wrong file? Materials doesn't have animated textures ticked and the textures path appears to be the default path.

#### infrenus

#### infrenus

 Level 20

JoinedMay 6, 2007Messages670

Resources: 24

#### Resources
Models 22Icons 1Skins 1

Erm, I dont know if someone pointed out about it, but you can also use ye old

Upgrade first\second\third\fourth\fifth

And so on for alternate animations, so I think using alternate and upgrade, you end up with like 10 more alternate texture slots.

#### Emm-A-

#### Emm-A-

 Level 14

JoinedJul 1, 2008Messages1,314

Resources: 1

#### Resources
Spells 1

Hi Flux,

thanks for trying to help me with your tutorial about alternate textures.

Unfortunately none of your suggestions helped, but I appreciate you wanted to help anyway

I managed to make a few trees use alternate snowy textures now, but despite I do the same to every model, it does only work in around 3 out of 6 cases in game. In 10 out of 10 cases, magos shows the textures though ..

I guess, there must be some little detail, that Wc3 does not like here ..

Anyway, the good thing is, it works sometimes

#### gusanomental

#### gusanomental

 Level 11

JoinedOct 16, 2013Messages244

Resources: 3

#### Resources
Icons 1Maps 2

Cool tutorial, thanks for sharing.

 You must log in or register to reply here.


### Similar threads

 Animated Textures Tutorial


Amigurumi

Feb 9, 2010

3D Modeling Tutorials

Replies39Views49K

Dec 24, 2024
Greedy Martyrdom

Locked

 Different Texture for Demon hunter Alternate Animations


odemmah

Sep 15, 2016

World Editor Help Zone

Replies0Views1K

Sep 15, 2016
odemmah

Locked

 Unwanted Animation switch ingame


lolz

Apr 30, 2019

Modeling & Animation

Replies5Views1K

May 11, 2019
Hermit

 Putting what you learned about Animated Textures into action


Spacebuns

Dec 15, 2015

3D Modeling Tutorials

Replies3Views4K

Jun 29, 2026
Spacebuns

 Model Editing


Fabioz

Jan 18, 2009

3D Modeling Tutorials

Replies45Views66K

Oct 10, 2021
KaiZen7th

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