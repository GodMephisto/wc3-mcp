---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/basic-animation-transfers.31703/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Basic Animation Transfers
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/basic-animation-transfers.31703/>
#### Xarwin

#### Xarwin

 Level 19

JoinedNov 16, 2006Messages2,162

Resources: 7

#### Resources
Icons 3Maps 1Tutorials 3

Basic Animation Transfer Tutorial

Hello all. In this tutorial I will explain how to edit a model covering the following basic subjects:

- Transferring animation from one model (warcraft) to another (warcraft).

- Adding a new animation to a model (warcraft).

- Removing a useless animation (warcraft).

Tools we need :

- Warcraft 3 Viewer

 -> Download here.

- MDLX Converter

 -> Download here.

- Oinkerwinkle's Animation Transfer

 -> Download here.

- Warcraft 3 Model Editor

 -> Download here.

Tutorial Start

Getting your model

First of all, you must get your model that you want to edit.

For this step we will use the Warcraft 3 Viewer.

Open Your Warcraft 3 Viewer and browse to the Treeview (If you can not see this window, press CONTROL+H).

Select the model/unit you prefer and right-click on it to extract it.

Extract it to a known location, so you can find it again quickly.

Hint: Can't find your model? Press Control+F and try to search for it by name.

Converting your model into mdl

You will need to convert your model into .mdl format because Oinkerwinkle's Animation Transfer does not support mdx formats!

Now, Open the MDLX Converter and load your model.

Be sure you selected the model you wanted, and that the type of the file is MDX 3D FILES. Open up your file and it will automatically convert your mdx model into mdl.

There: you have finished exporting the model you wish to edit!

Lets go to the next steps.

Transferring an animation from one model to another

If you followed the previous steps correctly, you already have the model file you want to change animations with.

In order to transfer your model's animation to other animations, you will need to first search for a suitable model with fitting animations. (An example would be taking the footman animations for the priest.)

Export the model with the animation you want again as I explained in the previous steps.

There, now you should have two models:

-> The model (your model which will receive the new animations).

-> Animation Model (this model's animations will be transfered upon your model, The model).

Now open Oinkerwinkle's Animation Transfer and do the following steps:

Fill in the Base fieldThe model you want to change animations with.

Fill in the Animation file the Animation Model which animations will be transfered to your model (The Model).

Fill in the Output file the location of where you want to save the model. Also change the name (if you want).

(Notice that there are three buttons, one behind each that is named ... . I selected them with a blue color, press here to select the models, and choose the save location.)

Press Transfer and wait a few seconds until the program gives the response : Done.

When done, you can find your model at the location you saved it with the name you gave it!

Warcraft 3 Model Editor is useful to view your model!

Double click on your file, and open it with the Warcraft 3 Model editor or simply load it into the program.

You might see your model in it, then press:

1) Windows

 2) Animation Controller

A window pops up with some options.

Select the arrow (to the right of Unanimated) and pick the animation that you would like to see on your model!

Adding a new animation to a model

There are three ways (or more) to actually do this.

I will explain here the easiest way to do it.

Check the previous steps, and get again

two models:

 Code:

-> [color=red]The model[/color] (your model which will receive the new animations).
-> [color=royalblue]Animation Model[/color] (this model its animations will be transfered upon your model, [color=red]The model[/color]).

Open the model (with the animation you want to transfer) in the Warcraft 3 Model Editor and enter the animation screen.

You may see a lot of animations, select the one you want to transfer to your model and write (or keep in mind) the animation name. I will use as example:

Spell - Channel.

If you have your models then you may open Oinkerwinkle's Animation Transfer and:

Fill in the Base field of the model you want to add a new animation to.

Fill in the Animation file of the Animation Model where the animation(s) will be taken from.

Fill in the Output file the location of where you want to save the model. Also again, change the name if you want.

Attention! A new step here,

Select the "Transfer single animation" and fill in the Animation to transfer field with your chosen animation name. In this case, it would be

Spell - Channel.

Press "Transfer" and wait a few seconds until Done pops up.

There, You just added a new animation to your model!

Removing a useless animation

The last subject of this tutorial.

Again, there are several options to remove animations from a model but I will show the easiest way for beginners.

Export again a model, the one you would like to remove one or several animations from.

(This model does not have to be converted into .mdl format, since it works with both file types *mdx & *mdl).

Open your model with the Warcraft 3 Model Editor.

Go to :

1) Windows

 2) Sequence Manager

A window pops up again and you may see several animation names like walk, death, attack -1, ... and so on.

To remove an animation, simply right click at it and press remove.

The animation gets removed now. Save the file and you're done!

If you don't remember the animation name you wanted to remove, you may also view it again by using the Animation Controller.

--------------------------------------------------------------------------

Thats it! I hope you enjoyed it!

Hints:

- Not all animations can be transfered and will show wrong animations.

- Is your model giving randomly corpses? visit this link then.

- Sometimes you are unable to open converted mdl files with the Warcraft 3 Model Editor. Simply open the mdl file with the MDLX Converter and it will be converted back to a mdx file.

~- Xarwin


 Last edited by a moderator: Jun 26, 2012

#### 1)ark_NiTe

#### 1)ark_NiTe

Patron

 Level 18

JoinedNov 1, 2006Messages1,612

Resources: 6

#### Resources
Maps 1Spells and Systems 4Tutorials 1

Awesome Heero, great job. It's interesting that this is exactly what I needed for the Mini-Game contest and I had no idea how to do it, but I found it as the first thread in this section!

EDIT: This does not work for me, each time I try to add all the animations it will erase all the others that were originally with the base model, and if I try to add one animation from the animation model to the base model, and then try to open it up I get the error: Unable to load "C:\ETC.", unknown group "20100:"!


 Last edited: May 16, 2007

#### Nasrudin

#### Nasrudin

 Level 17

JoinedSep 25, 2004Messages508

Resources: 31

#### Resources
Models 30Maps 1

Heero said:

Removing a useless animation


The last subject of this tutorial.


Again, there are several options to remove animations from a model but I will show the easiest way for beginners.


Export again a model, the one you would like to remove one or several animations from.


(This model does not have to be converted into .mdl format, since it works with both file types *mdx & *mdl).


Open your model with the Warcraft 3 Model Editor.


Go to :


1) Windows


 2) Sequence Manager


A window pops up again and you may see several animation names like walk, death, attack -1, ... and so on.


To remove an animation, simply right click at it and press remove.


The animation gets removed now. Save the file and you're done!


If you don't remember the animation name you wanted to remove, you may also view it again by using the Animation Controller.


Click to expand...


 Deleting a sequence makes an animation unplayable, it does not remove it. All the bones' keyframes are still there taking up space.


 To remove them use the look at the program in this post.

#### Dracula

#### Dracula

 Level 9

JoinedJun 26, 2005Messages509

Resources: 2

#### Resources
Maps 2

how come when i tried to add one animation it gives me error? do i remove the animation before adding or does it overwrite, it gives the the error everytime i use add one animation.

#### Kitabatake

#### Kitabatake

 Level 39

JoinedDec 30, 2005Messages1,383

Resources: 168

#### Resources
Models 152Icons 3Packs 1Maps 11Spells and Systems 1

Fixing screwed up transfers

Dracula said:

 how come when i tried to add one animation it gives me error? do i remove the animation before adding or does it overwrite, it gives the the error everytime i use add one animation.


Click to expand...


That is because Anim transfer screws up the model when only one animation is transferred. And magos model editor can only open somewhat perfect models.


BUT you can still fix it.

Get yobguls converter here

Convert the .mdl model that gives the error into .mdx with the converter

Try to open the .mdx model with magos


Without this converter the transfer of a single animation will always fail.


With this converter the transfer of a single animation will sometimes fail.


meaning: sometimes the model will not work even if you try to convert it to .mdx


Then you should try renaming the bones or getting the animations from another model.

#### gargaeus

G

#### gargaeus

Failure...

whenever i try to transfer animations it says done but when i open the model nothing happened

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

wow. More details?

#### Deep Sea Kraken

#### Deep Sea Kraken

 Level 15

JoinedDec 29, 2006Messages985

Resources: 10

#### Resources
Models 9Icons 1

Im trying to add a single animation to a model, i did what Kita said by converting the .mdl to .mdx but the animation doesnt show up at all.

#### Kitabatake

#### Kitabatake

 Level 39

JoinedDec 30, 2005Messages1,383

Resources: 168

#### Resources
Models 152Icons 3Packs 1Maps 11Spells and Systems 1

Deep Sea Kraken said:

 Im trying to add a single animation to a model, i did what Kita said by converting the .mdl to .mdx but the animation doesnt show up at all.


Click to expand...


Like I said,


 Kitabatake said:



 Without this converter the transfer of a single animation will always fail.


With this converter the transfer of a single animation will sometimes fail.


Click to expand...


If you do what I said, you have a better chance to succeed, but there are certain unit's whose animations just don't match because of different bone names.


Also some times the problem can be that the bones have same names, but in the other model they are helpers instead of bones.


Then you can open the model (.mdl) in a notepad (before transferring animations) and replacing every "helper" word with "bone", except the very first one which is "number of helpers".


Then you'll just have to change the number of helpers to 0 and number of bones to number of bones + number of helpers.


But even this doesn't work with ALL models.

#### Ravencore

#### Ravencore

 Level 2

JoinedDec 29, 2007Messages16

Error!

i'm getting this error after I transfer a single anim:


Expected "}" , got "2" in group "Bone" in "model.mdl"


PLZ HELP

#### Kitabatake

#### Kitabatake

 Level 39

JoinedDec 30, 2005Messages1,383

Resources: 168

#### Resources
Models 152Icons 3Packs 1Maps 11Spells and Systems 1

Ravencore said:

 i'm getting this error after I transfer a single anim:

Expected "}" , got "2" in group "Bone" in "model.mdl"

PLZ HELP


Click to expand...

Like I said before:

Kitabatake said:

 That is because Anim transfer screws up the model when only one animation is transferred. And magos model editor can only open somewhat perfect models.

BUT you can still fix it.

Get yobguls converter here

Convert the .mdl model that gives the error into .mdx with the converter

Try to open the .mdx model with magos

Without this converter the transfer of a single animation will always fail.

With this converter the transfer of a single animation will sometimes fail.

meaning: sometimes the model will not work even if you try to convert it to .mdx

Then you should try renaming the bones or getting the animations from another model.


Click to expand...

You have transferred a single anim -> model gets bugged

next you should convert the model to .mdx by using Yubguls converter which I mentioned in my earlier post.

Then try opening your model in magos model editor.

#### NatDis

#### NatDis

 Level 8

JoinedJan 11, 2008Messages164

Resources: 5

#### Resources
Models 4Icons 1

Eh I thought Magos' Model Editor could rename bones but the anim still did not transfer.

#### Lillerinmuna

#### Lillerinmuna

 Level 6

JoinedAug 25, 2007Messages27

Resources: 5

#### Resources
Models 2Maps 3

Maxium number of animations

Is there some maxium for the animation?? is it 11??

#### vhx

#### vhx

 Level 3

JoinedJan 15, 2008Messages36

Is this suppose to transfer the animated model to it as well? Like say I wanted to take a Doodad model, and add a build animation to it from another model. Is this suppose to transfer the build animation over to it? Like changing the model associated with the animation? If not, what guide should that fall into?

#### XS-Archangel

#### XS-Archangel

 Level 12

JoinedDec 16, 2007Messages450

Resources: 12

#### Resources
Icons 1Skins 6Maps 5

Heero can you post this on you "know which site"?

We put tutorial section there

#### Drazalon

#### Drazalon

 Level 2

JoinedAug 15, 2006Messages21

hmmm. an interesting process, very quite useful indeed nontheless.

However is there a way to rename the animation using the programs required? Or must i go into the mdl txt and rename it from there and what not?

A final note Bravo!

EDIT: ahhh nevermind you do have to open up a Text Program and edit the names from there =]

Love this tutorial. By far is helping my map the most.

#### Lord_Firage

#### Lord_Firage

 Level 11

JoinedApr 26, 2008Messages365

Resources: 16

#### Resources
Models 6Skins 6Maps 2User Interface 2

- Warcraft 3 Model Editor


-> Download here.


Tool doesn't Exist

#### qqluqq

#### qqluqq

 Level 13

JoinedJun 4, 2008Messages299

Resources: 14

#### Resources
Models 5Icons 2Skins 1Tools 1Maps 5

it all works but when i want to import my model in a map i get a fatal error

#### Verdun

#### Verdun

 Level 16

JoinedJun 9, 2008Messages350

Resources: 11

#### Resources
Models 11

##
 Shortlegs


Basically the new model needs the same bones as the model you take the animations from. Question, if I use gmax, can I just copy/paste (by merging) these bones from the original model into my new one? Or will this only screw up everything?

#### apaka

#### apaka

 Level 13

JoinedDec 18, 2007Messages63

Resources: 15

#### Resources
Models 15

You jus use https://www.hiveworkshop.com/forums/tools.php?id=yzpnex


When it shows eror line when you try to convert the model you jus open it in notepad and delete the line and it works.

#### Shadeus

#### Shadeus

 Level 2

JoinedSep 6, 2008Messages18

Hey! that Oinker thingy doesnt work to download!

#### FirstLegacy

#### FirstLegacy

 Level 7

JoinedNov 8, 2008Messages173

Resources: 8

#### Resources
Models 3Icons 1Skins 2Maps 2

wow this tutorials is really useful!!!THNKZ!!!!!

#### unclenig

U

#### unclenig

I transferred a single animation, and it showed perfectly on Magos. But it(the transferring anim) doesn't show up on WE or in game. Help please?

#### novasmage

N

#### novasmage

Yay !! lol it worked ive been tryin to figure how to do this so long.....thanks alot for this tutorial!!!

#### Cicija16

#### Cicija16

 Level 4

JoinedJan 20, 2009Messages36

Resources: 3

#### Resources
Models 1Spells and Systems 2

When i opened this 2 combined,whitch i combined with animation transfer program,it only opened priest and i tryed to open model editor,then windows,animation controler,untaimed there just writed attack1,attack2 and that shits no sing of other animation why?????

#### Cicija16

#### Cicija16

 Level 4

JoinedJan 20, 2009Messages36

Resources: 3

#### Resources
Models 1Spells and Systems 2

Oh i get it now lol=Djust adds to footman spell attack,or opposite adds to priest lets say defend walk lol

#### zikko

#### zikko

 Level 3

JoinedApr 1, 2009Messages15

Resources: 1

#### Resources
Skins 1

if the animation looks fucked up in model editor, does that mean I have to find other animation? I was thinking of changing footman attack animation to paladin

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

Instead of transferring a single anim, couldn't you transfer all of them and delete them? (then again, the AnimRemover tool never worked right for me...)

#### zikko

#### zikko

 Level 3

JoinedApr 1, 2009Messages15

Resources: 1

#### Resources
Skins 1

a late post here  so I'm on another project and I would want to make a priest model into wielding a 2handed sword animation, when Im done with the transfer (Arthas wielding frostmourne) the priest just looks like hes having somekinda stroke when he attacks. :S

#### lord_Turin

#### lord_Turin

 Level 11

JoinedOct 30, 2004Messages338

Resources: 1

#### Resources
Maps 1

for removing anims cant you just open with textpad and remove the anims that are listed there? or does that leave code which just takes up space but isnt used in the model?


just wondering..

#### Twisted_Firestarter

T

#### Twisted_Firestarter

Does it work somehow with wow models? 'Cause if it does I haven't figured it out yet. Anyway, if it doesn't work, do you know a tool or a method of transfering animations between wow characters? Please help!

#### demon.spider

#### demon.spider

 Level 3

JoinedMay 15, 2008Messages47

Resources: 2

#### Resources
Maps 2

help please. i added one new animation to a unit (both custom units) but when i open the model with the transfered animation in magos world editor, to check out the new animation, the model with the new animation does nothing...it takes the 'unanimated' stance....

#### pkw

#### pkw

 Level 9

JoinedMay 28, 2008Messages455

Resources: 1

#### Resources
Models 1

Perhaps the model does not have the similiar bones with the other model.

The tool basicly uses the same model bones to transfer animations. You could rename the bone thou.

#### Boogie666

B

#### Boogie666

Nice tut man... I tried tranfering the blademaster's anim to a viliger male. it look good in model editor but when I import it to world editor and run the map it crashes wc3. any ideea why? i did everythig just like in the tutorial

#### BloodStorm2262

#### BloodStorm2262

 Level 8

JoinedMar 18, 2005Messages168

Resources: 3

#### Resources
Skins 1Maps 2

Can't seem to get the animation transfer program to work, nor animation remover, it says there is an error reading some file or something. I'm using 64 bit Vista, but tried to run it in compatability mode.

#### Leazy

#### Leazy

 Level 4

JoinedJan 7, 2008Messages61

Sounds cool, could I get a tauren chieftain to play a good bladestorm animation with this?

#### Teaspoon

#### Teaspoon

 Level 14

JoinedOct 1, 2009Messages596

Resources: 14

#### Resources
Models 14

Hey I got a problem with this, I tried to give blademaster attack anim to a unit with paladin attack anim but when I press Transfer a message pops up saying that the file is already open.

#### machris13

#### machris13

 Level 7

JoinedMar 22, 2010Messages228

Well, it works for me...i got no errors received but a sorceress with a bandit animation is.....(--,)

#### Bommelding

#### Bommelding

 Level 2

JoinedAug 31, 2009Messages21

Kitabatake said:

 That is because Anim transfer screws up the model when only one animation is transferred. And magos model editor can only open somewhat perfect models.


BUT you can still fix it.

Get yobguls converter here

Convert the .mdl model that gives the error into .mdx with the converter

Try to open the .mdx model with magos


Without this converter the transfer of a single animation will always fail.


With this converter the transfer of a single animation will sometimes fail.


meaning: sometimes the model will not work even if you try to convert it to .mdx


Then you should try renaming the bones or getting the animations from another model.


Click to expand...


when i try to open that converter it says it can't find GAME.DLL

#### Chewii

#### Chewii

 Level 11

JoinedDec 5, 2009Messages846

Resources: 1

#### Resources
Maps 1

So when youre done can you just import the mdl model into your map then ? Cuz it doesn't work for me.

Edit it works now.

But i have another thing. My animation transferer is black when you should write path for single animation. I can't see what im writing. But when i select the text i can see what im writing and even if im writing correct it doesn't work. it only stands Error wrong path.


 Last edited: Jul 12, 2010

#### blackdragon55

#### blackdragon55

 Level 2

JoinedJul 14, 2010Messages25

D: the tutorial didnt work for me can someone make a peasant with footman animations?

#### rama462

#### rama462

 Level 3

JoinedNov 10, 2010Messages9

Resources: 2

#### Resources
Maps 2

Hey when I change nothing happens.


STAND put the animation does not rotate.


PLEASE HELP !!!!! ¡¡¡¡¡


Reasons for not working, Graphic:


Model Editor: llllllllll


MdlxConv: llllllllll


Warcraft3Viewer: llllllllll


Anim Transfer: llllllllll

#### TheWhiteWolf

#### TheWhiteWolf

 Level 17

JoinedJul 5, 2010Messages1,132

Resources: 20

#### Resources
Models 12Maps 6User Interface 1Tutorials 1

So, I have two models with exactly the same bones. After converting .mdx to .mdl, transfered the animation, and converting back to .mdx, this happens:


-Names of animations are transfered


-Not a single keyframe is transfered


Any thoughts? Or could anyone try to do this for me?

#### sigmaceti

#### sigmaceti

 Level 2

JoinedJun 21, 2014Messages11

this dont work, when I convert model back to .mdx file and add to map, model read as .mdl and it dont work on map, anyone can help?

#### Screamernail

S

#### Screamernail

so you can give Illidan a murloc attack animation?

#### sarumanthecursed

#### sarumanthecursed

 Level 9

JoinedJun 10, 2013Messages472

Resources: 1

#### Resources
Maps 1

whenever i try to transfer animations it says done but when i open the model in world editor theres no new animations but there there in the model editor

EDIT: Works in game so its all good XD

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

sarumanthecursed said:

 whenever i try to transfer animations it says done but when i open the model in world editor theres no new animations but there there in the model editor

EDIT: Works in game so its all good XD


Click to expand...

You might need to close the map and reopen it to see the changes (or restart the editor). The editor has some weird caching implementations that can get buggy at times. For example, if you open map A with a custom footman skin and then open some other map B, then map B will have the custom skin on its footmen. (if you test it in game, the skin won't be applied. It is just a world editor bug)

But that bug can be fixed if you restart the editor. It might be a similar case here.

#### HappyTauren

#### HappyTauren

 Level 26

JoinedNov 3, 2006Messages8,077

Resources: 91

#### Resources
Models 63Icons 23Packs 1Tutorials 4

You do not have to restart the editor, changing model quality in video settings (for editor) is enough to re-render it.

#### Kyrbi0

#### Kyrbi0

 Level 53

JoinedJul 29, 2008Messages10,313

Resources: 5

#### Resources
Models 2Maps 2Spells and Systems 1

HappyTauren said:

 You do not have to restart the editor, changing model quality in video settings (for editor) is enough to re-render it.


Click to expand...

!


This sounds like something I wish I knew years ago. Are you serious? That would be amazing.

#### SNIper of DARKness

#### SNIper of DARKness

 Level 13

JoinedApr 9, 2013Messages764

Resources: 1

#### Resources
Tutorials 1

Oinker's tool doesn't work, I get error with "COMDLG missing", Please help.

 You must log in or register to reply here.


### Similar threads

 Geosetanims - fix those randomly appearing corpses!


Horn

Jul 10, 2004

3D Modeling Tutorials

Replies0Views25K

Jul 10, 2004
Horn

Locked

 Animation Transfer Problem


TheWhiteWolf

Feb 22, 2014

Requests

Replies5Views2K

Feb 24, 2014
TheWhiteWolf

Locked

 Basic Weapon Removal


Kimbo

Mar 14, 2007

3D Modeling Tutorials

Replies5Views28K

Sep 6, 2007
Kimbo

B

Locked

 Animation transfer error


-Bliksem-

Sep 1, 2013

Modeling & Animation

Replies1Views1K

Sep 2, 2013
StoPCampinGn00b

Locked

 Single animation transfer help!


deafir

May 25, 2013

Modeling & Animation

Replies2Views2K

May 25, 2013
General Frank

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