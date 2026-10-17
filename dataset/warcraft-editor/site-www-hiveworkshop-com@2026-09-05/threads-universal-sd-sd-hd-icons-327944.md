---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/universal-sd-sd-hd-icons.327944/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Universal SD SD HD Icons
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/universal-sd-sd-hd-icons.327944/>
#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Within this tutorial, you'll find how to use all in-game icons of any graphic mode, across any graphic mode. This includes brute forcing icon paths, to make them display in the mode you are looking to use.

This is useful for maps that want to retain a bit of similarity between supported modes for it, or have a Reforged icon in use that won't display in Classic.

You don't need to own Reforged to do this.

It's super simple, but can be tedious.

This works for all in-game icons, although some icons may be missing a specific version of an icon.

To do this, you'll want to navigate to the Object Editor inside the World Editor, and locate the object you want to change.

Note: While the Object Editor may display icon paths as .blp, all icons use .dds. You may use these interchangeably.

For this example, we'll use a Faerie Dragon. Currently, I am in SD+, so the icon displayed above is the SD+ Faerie Dragon.

To use the SD Faerie Dragon icon, we'll set the path to - war3.w3mod:ReplaceableTextures\CommandButtons\BTNFaerieDragon.dds

To apply this to your icon of choice, you'll want to prefix it with -

war3.w3mod:

To use the SD+ Faerie Dragon icon, we'll set the path to - _addons\hd2.w3addon\icons.w3mod:_hd.w3mod:ReplaceableTextures\CommandButtons\BTNFaerieDragon.dds

To apply this to your icon of choice, you'll want to prefix it with -

_addons\hd2.w3addon\icons.w3mod:_hd.w3mod:

To use the HD Faerie Dragon icon, we'll set the path to - war3.w3mod:_hd.w3mod:ReplaceableTextures\CommandButtons\BTNFaerieDragon.dds

To apply this to your icon of choice, you'll want to prefix it with -

war3.w3mod:_hd.w3mod:

Now, we can see that three separate objects will use each type of icon independently, regardless of the players graphical mode settings.

That concludes this tutorial, however there is one known bug by using this method. If you are to pause the game, it will show a default disabled button of the original icon depending on the players current graphical mode.


 Last edited: Nov 19, 2024

#### _Guhun_

#### _Guhun_

 Level 12

JoinedJun 12, 2010Messages413

Resources: 17

#### Resources
Models 1Maps 1Spells and Systems 9JASS 4Tutorials 2

Might be a good idea to add that you can also force classic icons in Reforged mode by prepending war3.w3mod: to the path. In this case you can even force classic models with the same technique.

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

_Guhun_ said:

 Might be a good idea to add that you can also force classic icons in Reforged mode by prepending war3.w3mod: to the path. In this case you can even force classic models with the same technique.


Click to expand...

Done

#### StormKnight

#### StormKnight

 Level 19

JoinedNov 12, 2016Messages834

Noice

#### _lycan

#### _lycan

 Level 19

JoinedJan 12, 2011Messages1,537

Resources: 4

#### Resources
Maps 3Tutorials 1

Just want to point out that if you really wanted you can fix the bug with the menu, by changing the path in gameplay interface to disabled icons, add _hd.w3mod: before the path, however you will need to change the import path of every disabled icon in your map to contain _hd.w3mod: before it, in addition to that, you will also now need to change the active icon of every icon in used in the game for example:

units, abilities, upgrades, etc.

most likely you would need to do it with a script... but it can be done and it's not that time consuming.

here's an example of the disabled version working with icons off

as you can see I haven't changed the imported disabled icons yet to include the path but yeah it can be done.

#### _Guhun_

#### _Guhun_

 Level 12

JoinedJun 12, 2010Messages413

Resources: 17

#### Resources
Models 1Maps 1Spells and Systems 9JASS 4Tutorials 2

That works only as long as you don't mix both types of icons though. But if you're only planning on using Reforged icons in classic mode, it's a nice workaround.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Quick thoughts

Mayday said:

 Tools Required: None

Click to expand...

Then this does not need to be mentioned, as unless it is specifically mentioned you assume there are no requirements.

______________

I am not a fan of using the "list" layout when not actually listing things.

I've tried to reason out why, and I have rewritten it 4 times now.

The short not-to-specific reason is that I think it affects readability for various reasons I have trouble formulating with words.

You can contest me on this as it's possibly subjective.

Mayday said:

That concludes this tutorial, however there is one known bug by using this method.

If you are to pause the game, it will show a default disabled button of the original Classic icon.

Click to expand...

I do not think images are required to get this point across.

_____________

Also the images are waay to big


 Last edited: Oct 24, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Similarily to UI: Save&Load Frames I feel like this tutorial can be summarized in a single sentence.

"Edit the icon path in the object editor to _hd.w3mod:<normal icon path here> or war3.w3mod:<path> for classic icons"

If you remove the pictures you realize the tutorial is indeed very short, and thus arguably too simple.

However, it is incredibly useful which is why I think it can have a place regardless.

PurgeandFire said:

Avoid overly-simple topics.

For example, someone explaining how to open the trigger editor may be deemed as too simple. Those tutorials usually are not needed. The exception to this rule is if enough users deem it useful.


Click to expand...

If I had a team to consult, I would.

Approved.

#### Ahega0

#### Ahega0

 Level 2

JoinedNov 28, 2020Messages6

Resources: 1

#### Resources
Maps 1

How can I change the internal files of the game? so that you can play the story mode and custom campaign with all the changes

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Ahega0 said:

 How can I change the internal files of the game? so that you can play the story mode and custom campaign with all the changes


Click to expand...

Well, the easiest method to run the campaign in reforged mode is, shockingly, turn on reforged mode.

I'm not sure what you mean at all. It sounds like you just want to play in Reforged mode. If you are asking how to play with Reforged graphics in classic because you do not own it, this method only works for map developers who can alter the data themselves. If you want to change the files for only yourself, you can research changing local files.

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

updated for patch 2.0

 You must log in or register to reply here.


### Similar threads

[Solved]
 SD & HD Model/Icons when switching


Zenonoth

Dec 31, 2025

World Editor Help Zone

Replies0Views343

Dec 31, 2025
Zenonoth

 How to: Import an Icon


Archian

Nov 7, 2006

Misc/General Mapping Tutorials

Replies4Views59K

Jan 22, 2023
Shawwy

Locked

 Reforged Textures Problem


xorkatoss

Jan 1, 2021

Texturing & Surfacing

Replies1Views3K

Apr 4, 2021
Ardenaso

 High quality Classic SD-style autocast icon is needed


Makkad

Aug 17, 2025

Art and Graphic Designs

Replies1Views1K

Aug 19, 2025
Makkad

Locked

 Icon Borderizer SD broken?


Daffa

Sep 29, 2022

Site Discussion

Replies1Views1K

Sep 30, 2022
Ralle

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