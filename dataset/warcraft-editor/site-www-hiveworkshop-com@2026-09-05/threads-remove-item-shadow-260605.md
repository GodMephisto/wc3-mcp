---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/remove-item-shadow.260605/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Remove Item Shadow
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/remove-item-shadow.260605/>
#### Imadori

#### Imadori

 Level 14

JoinedOct 8, 2012Messages498

Resources: 9

#### Resources
Icons 5Spells and Systems 2Tutorials 2

Remove Item Shadow

### All Items

### Single Item

 Read Me:


About this Method:

* Use this method if you want to hide absolutely all items' shadows in your map.

* It's best to use this method before you start modifying the Gameplay Constants.

* The testmap available below is made for the second method. This is too simple already.

Warning:

* This method is detrimental if you have already heavily modified the Gameplay Constants, because once you save and reopen your map, it completely resets all values to default.

There are some things you can do to avoid that, but I won't discuss it here since it's a completely different topic.

The requirements needed for this method:

Notepad, or any text editing software

Basic importing skills

 Method A:


1.) Create .txt File:

* Open Notepad or any other text editing software. Save the empty note and name it war3mapMisc.

​
2.) Edit Text:

* Now edit it. Copy and paste the code below, and save it again.

 Code:

[Misc]
ItemShadowFile=Null

* If you want to undo it, just follow this method again, except you put ItemShadowFile=Shadow instead.

 Code:

[Misc]
ItemShadowFile=Shadow

3.) Import:

* After that just import that war3mapMisc.txt in your map. By then, its name in the Import Manager will be "war3mapImported\war3mapMisc.txt". Double-click to rename and erase the "war3mapImported\" part.

​
4.) Reload the Map:

* Save your map and close it. Then re-open that map. Put some items on the ground and press CTRL+F9 to test, it should be working by now.

​
5.) Delete Note:

* Delete the war3mapMisc.txt you just imported earlier. Its job is finished, you don't need it anymore.

* If you don't delete it, any modification in Gameplay Constants except ItemShadow will not apply, since that .txt will always overwrite those changes to default.

​
Done! Save the map!​

 Read Me:


About this Method:

* Use this method if you want to hide only a specific number of shadows.

* This method only works for trigger created items, so plan ahead.

* The test map is like an extension of this, I recommend you to download it for clarification.

* Works on units.

Warning:

* If you don't follow the steps carefully, you might experience the selection circle bug. Which causes selection circles of random units and items to be misplaced or not appear sometimes.

The requirements needed for this method:

Basic knowledge of GUI Triggering

 Method B:


1.) Dummy Item:

* If your map already contains at least one item (on the ground, not carried), then skip this step.

* Put one Item inside the map (any type will do; i.e. Cheese, Boots of Speed), doesn't matter where you put it since you'll be hiding it for the rest of the game anyway.

* Open the Trigger Editor. In your

Map Initialization trigger, hide that item:

Events
Map initialization

Conditions

Actions
Item - Hide Cheese 0001 <gen>

2.) Destroy Image:

* Whenever you attempt to create a shadowless item, you must first destroy the last created image:

Image - Destroy (Last created image)
Why? I have no idea. What I only know (for now) is that, the 4th Step won't work unless you do this. If someone has a clue, please do tell.

​
3.) Create Item:

* Create the item you want but do not give it to a Hero/Unit yet. Just create it from point.

Item - Create Starfish at (Position of HERO)

4.) Make Image Invisible:

* After you create an item you'll need to make the last created image (destroyed earlier) invisible.

Image - Change the color of (Last created image) to (0.00%, 0.00%, 0.00%) with 100.00% transparency
Don't bother changing the color, doesn't work on shadows anyway.

​
It should look like this:

Image - Destroy (Last created image)

Item - Create Starfish at (Position of HERO)

Image - Change the color of (Last created image) to (0.00%, 0.00%, 0.00%) with 100.00% transparency
If you skipped the 1st Step because you have more than one item proceed to Step 5,

otherwise, you're done.

5th Step - Find the Dummy:

* Make sure you're able to create at least one shadowless item in-game (either by events or Map Initialization).

* Save and Test your map. Create a shadowless item (assuming you didn't on Initialization). Among the map's starting items, find the one that doesn't have a shadow.

* After finding, quit the game, open Trigger Editor in WE, and hide that item in

Map Initialization trigger, just like what I did on the 1st Step.

​
Now you're done for real!​

#### Attachments

RemoveItemShadow.w3x

 20 KB

 · Views: 338




 Last edited: Jan 6, 2018

#### Deleted member 219079

D

#### Deleted member 219079

This is such a hack you've found out ^^

It even works with:

 JASS:

 call DestroyImage(null)
 call CreateItem(u,x,y)
 call SetImageColor(null,0,0,0,0)

#### Anvil

#### Anvil

 Level 13

JoinedJun 8, 2011Messages472

Resources: 10

#### Resources
Models 7StarCraft II Resources 1Tutorials 2

You can hide these shadows adding to war3mapMisc.txt this line:

 JASS:

[Misc]
ItemShadowFile=Null

Default value is ItemShadowFile=Shadow


You can replace "Null" with any shadow. (See Shadows folder, path "ReplaceableTextures\Shadows\")


You can set the size/position if needed too:

 JASS:

[Misc]
ItemShadowSize=120,120
ItemShadowOffset=50,50


If war3mapMisc.txt is not present in your map, you can create one.


This file appears when you modify the gameplay constants under Advanced/Gameplay Constants.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

If you would like, you could just update the guide with Anvil's method, and leave the rest as a separate section for removing a specific item/unit's shadows. I doubt very many people know about the image trick--I've personally only used it once, but it was useful to know for the issue I was having.

Anyway, the tutorial is organized quite well, it would be a shame to have to graveyard it.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Both methods got one advantage over each other.

Using miscdata will make every item shadow in the map go away, but you might want the shadow on certain items for whatever reason.

You could cover both.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

This doesn't need to be graveyard-ed, it allows fullsome control over item shadows.

Edit: Almost full control... Just selection circles can randomly appear on items.


 Last edited: Feb 28, 2015

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Dat-C3 said:

 This doesn't need to be graveyard-ed, it allows fullsome control over item shadows.


Edit: Almost full control... Just selection circles can randomly appear on items.


Click to expand...


Can you explain when this issue happens? If that happens, then the tutorial may need an adjustment.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

PurgeandFire said:

 Can you explain when this issue happens? If that happens, then the tutorial may need an adjustment.


Click to expand...


Nothing can be done really since we lack detection for this kind of thing. People will just see random selection circles once in a while.

#### Imadori

#### Imadori

 Level 14

JoinedOct 8, 2012Messages498

Resources: 9

#### Resources
Icons 5Spells and Systems 2Tutorials 2

Did you put an item inside the map and hid it just like said in the tutorial?

That one item should not be detectable by the mouse, otherwise the selection-circle bug will happen.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

@Imadori

Do you want to include the tequinique to easily remove shadows for all items with the import? It might be useful to share, too.

#### Imadori

#### Imadori

 Level 14

JoinedOct 8, 2012Messages498

Resources: 9

#### Resources
Icons 5Spells and Systems 2Tutorials 2

Well the thing is, I think the all-method overshadowed the tutorial completely. Even I switched to using it. Misc method should be naturally the main and best way right, and should be put at the top.

And the method I introduced will be just an alternative. But having an alternative which is more of a pain and takes longer to set up, I don't see why it's useful now.

If I should rewrite it, it'll be like writing a tutorial that I didn't came up with, so I'm worried about that.

Edit:Updated.


 Last edited: Nov 23, 2016

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

It's good to show a method how to remove the shadow from a specific item. Both tequiniques together would be perfect imo.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

The introduction's list looks a bit wild. Both methods are listed after each other between introduction list-elements, and even

the method's steps/description is also listed in same order. Use indentions or sub-lists, or something to point out the structure.

I think it's on a good way then. Could you provide a .txt file attachment to your tutorial as example?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Submission:Remove Item Shadow

Date:23 Jan 17

Status:Approved

Note:

I would like you to make the tiny changes mentioned in the post above.

Though, I think it's already in an aproveable state, and worth a tutorial.

#### Wark

#### Wark

 Level 13

JoinedOct 12, 2016Messages769

Resources: 3

#### Resources
Maps 3

I noticed a problem with the first method:

it automatically cancels out specific modified gameplay constants in the editor, such as max hero level.

I have no idea why it does that.

#### Imadori

#### Imadori

 Level 14

JoinedOct 8, 2012Messages498

Resources: 9

#### Resources
Icons 5Spells and Systems 2Tutorials 2

Ah. Delete the imported war3mapMisc.txt. I think it's only needed once after save and closing your map, then after reopening again, you can delete it.

If it doesn't work here's what you hafta do:

First, open Gameplay Constants and put a check on the Display Values As Raw Data.

Notice that the names change.

Now, while it is checked. Find the RAW name that you want to modify.

Add it on the war3mapMisc.

 Code:

[Misc]
ItemShadowSize=120,120
ItemShadowOffset=50,50
MaxHeroLevel=25

Then replace the one you imported before.

#### Wark

#### Wark

 Level 13

JoinedOct 12, 2016Messages769

Resources: 3

#### Resources
Maps 3

Alright Imadori, the second thing worked.

This method is detrimental you you heavily modify the gameplay constants, since once you save it then reopen the map it completely resets all values to default.

You might want to add a warning about that, and append that second method (specifically, you'll need to copy raw data for all modified values).

#### Imadori

#### Imadori

 Level 14

JoinedOct 8, 2012Messages498

Resources: 9

#### Resources
Icons 5Spells and Systems 2Tutorials 2

Updated!

#### SpiritTauren

#### SpiritTauren

 Level 8

JoinedMar 10, 2009Messages213

Resources: 3

#### Resources
Icons 1Skins 1Maps 1

Bless you for this wonderful discovery! A word of advice to ppl: saving your modifications to constants/interface in a spare map copy/txt file etc. is always a good idea, no matter what. Always back up your maps, especially large projects.

 You must log in or register to reply here.


### Similar threads

 How to fix Attack Speed reset in custom campaigns


Warseeker

Apr 13, 2026

Misc/General Mapping Tutorials

Replies5Views564

Apr 19, 2026
Chaosy

 Items - All About Items


XeNiM666

Sep 21, 2008

Misc/General Mapping Tutorials

Replies23Views43K

May 19, 2019
Null

 Shop Within a Shop


Adiktuz

Apr 7, 2011

Misc/General Mapping Tutorials

Replies18Views21K

Apr 29, 2024
SadOnix

 All About Multiboards


inverted

May 29, 2008

Misc/General Mapping Tutorials
2

Replies65Views38K

Apr 24, 2017
Hotwer

Locked

 Over Time Item Removal


Lee_Go

Apr 9, 2010

Requests

Replies10Views2K

Apr 10, 2010
Lee_Go

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