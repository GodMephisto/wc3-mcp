---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/hiding-extended-ui-panels.325835/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Hiding Extended UI Panels
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/hiding-extended-ui-panels.325835/>
#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Within this tutorial, you'll find how to remove the extended interface panels on Classic Graphics.
To clarify, we're talking about what is displayed in the image below.

To remove these panels, only two imports are required, depending on the amount of races you're looking to remove the panels for.
NOTE: These imports are dependent on the race selected by the user, as they overwrite Human/Orc/Undead/Night Elf interface individually.

If you're looking to only remove one races interface, you only need to import one set of the two files. However, if you're looking to remove it from every race, you'll need to import the set multiple times under different paths.

Download Here!

Tile05.dds

Tile06.dds

HumanPath:

UI\Console\Human\HumanUITile05.dds

UI\Console\Human\HumanUITile06.dds

OrcPath:

UI\Console\Orc\OrcUITile05.dds

UI\Console\Orc\OrcUITile06.dds

UndeadPath:

UI\Console\Undead\UndeadUITile05.dds

UI\Console\Undead\UndeadUITile06.dds

Night ElfPath:

UI\Console\NightElf\NightElfUITile05.dds

UI\Console\NightElf\NightElfUITile06.dds

Once you have the files imported under these paths, save your map and then launch it to discover there is no longer any extended panels, with no side effects such as Hit Point bars displaying underneath the main frame.

We're now finished with removing the extended panels, enjoy!

If you're curious how this works with Reforged enabled, it will slightly cut off a portion of what looks to be the main frame, but is actually reaching into Tile05 & Tile06.

Looking to have your interface trimmed but not be straight-edged? I've created some alternate UI elements to use. Check them out here!

NEW:

With Reforged 2.0, they have added an option to add/remove the tiles manually. If you do not wish for the user to have access to this in your map, you can overwrite the in-game files, using a similar method as listed above.

Simply import the Tile05/Tile06 again, and set the paths to these:

HumanPath:

UI\Console\Human\HumanUITile05Overlay.dds

UI\Console\Human\HumanUITile06Overlay.dds

OrcPath:

UI\Console\Orc\OrcUITile05Overlay.dds

UI\Console\Orc\OrcUITile06Overlay.dds

UndeadPath:

UI\Console\Undead\UndeadUITile05Overlay.dds

UI\Console\Undead\UndeadUITile06Overlay.dds

Night ElfPath:

UI\Console\NightElf\NightElfUITile05Overlay.dds

UI\Console\NightElf\NightElfUITile06Overlay.dds

#### Attachments

Tile05.dds

 144 bytes

 · Views: 619



Tile06.dds

 144 bytes

 · Views: 550




 Last edited: Nov 17, 2024

#### SinisterLuffy

#### SinisterLuffy

 Level 1

JoinedApr 8, 2020Messages109

I am unable to download the files for the Tiles. I just made a discord account for this  That might explain why I am not send to the discord server that contains the files. Why don't upload them here as a zip file via attachments? You know, just in case the links die in some future.

And your simple and to the point method should work in the latest legacy version as well, right? After all, the method seems to me just texture changes.

I must say finally a solution appears for that baggage.


 Last edited: Jul 4, 2020

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

SinisterLuffy said:

 I am unable to download the files for the Tiles. I just made a discord account for this  That might explain why I am not send to the discord server that contains the files. Why don't upload them here as a zip file via attachments? You know, just in case the links die in some future.

And your simple and to the point method should work in the latest legacy version as well, right? After all, the method seems to me just texture changes.

I must say finally a solution appears for that baggage.


Click to expand...

My apologies, that was left over from the paste bin copy. I'll upload it via attachment to the post now.

edit: sorry again, did not see your question about legacy. No, this will not work prior on patches prior to reforged because the black box underneath extends across the entire screen. You can do other things to remove the black box, but are left with issues such as HP bars being shown underneath the entire interface, etc.


 Last edited: Jul 4, 2020

#### ESTO

#### ESTO

 Level 3

JoinedApr 17, 2017Messages29

 Works, nice and easy. Thank you very much !

Untitled3

#### Boar

#### Boar

 Level 13

JoinedOct 10, 2009Messages439

Resources: 2

#### Resources
Maps 1Tutorials 1

a comment

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great tutorial. Works like a charm! Approved.

#### Parzival

#### Parzival

 Level 3

JoinedDec 18, 2021Messages16

I did as instructed, but I still can't hide it when entering the game .

#### Attachments

1652684489642.png

 3.2 MB

 · Views: 233



1652684522175.png

 62.7 KB

 · Views: 227



#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Parzival said:

 I did as instructed, but I still can't hide it when entering the game .


Click to expand...

perhaps you would like to use the standard asset manager, or if you want to do it locally, do it for Warcraft 3 and not the map file you have opened there

#### Parzival

#### Parzival

 Level 3

JoinedDec 18, 2021Messages16

Mayday said:

 perhaps you would like to use the standard asset manager, or if you want to do it locally, do it for Warcraft 3 and not the map file you have opened there


Click to expand...

I don't know where it is

#### Attachments

1652713453228.png

 46.2 KB

 · Views: 166



1652713446741.png

 22.5 KB

 · Views: 153



#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Parzival said:

 I don't know where it is


Click to expand...

I don't know what you're trying to do; modify your map, or every map?

If you want to modify your map, you simply open the editor and import the files as explained in the tutorial.

If you want to modify every map in Warcraft 3, you need to modify local files, which you can read about, here.

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

Added a note at the bottom for 2.0, allowing you to ignore the new setting to show/hide the extended panels.

#### SoulHarvester

#### SoulHarvester

 Level 3

JoinedJul 29, 2022Messages11

Hi, i have v1.31.1 and simply don't get where i have to install the files, what is the complete location? or do i need to use the map editor for this?

I only have 2 UI folders in the whole Warcraft III directory.

#### Attachments

1732893201080.png

 10.5 KB

 · Views: 83



#### Ravager16829

#### Ravager16829

 Level 40

JoinedMay 14, 2021Messages2,455

SoulHarvester said:

 Hi, i have v1.31.1 and simply don't get where i have to install the files, what is the complete location? or do i need to use the map editor for this?

I only have 2 UI folders in the whole Warcraft III directory.


Click to expand...

You have to use Ladik's Casc Viewer, then locate this path:

"war3.w3mod\ui\console\"

For a safe option, make sure you create a folder yourself with the path "ui\console\[your-race]" in your game root, place the required BLP files on that folder, and then activate the Local Files feature.

If you want to revert the changes, simply delete that folder in your game root OR disable the Local Files feature.

I highly suggest NOT using any third-party mods like yours (you are using the Rebirth), as it can mess up the game.

#### SoulHarvester

#### SoulHarvester

 Level 3

JoinedJul 29, 2022Messages11

Ravager16829 said:

 You have to use Ladik's Casc Viewer, then locate this path:

"war3.w3mod\ui\console\"

View attachment 499644

For a safe option, make sure you create a folder yourself with the path "ui\console\[your-race]" in your game root, place the required BLP files on that folder, and then activate the Local Files feature.

If you want to revert the changes, simply delete that folder in your game root OR disable the Local Files feature.

I highly suggest NOT using any third-party mods like yours (you are using the Rebirth), as it can mess up the game.


Click to expand...

Really appreciate the fast reply and explanation! Will follow these steps and let you know if it works or not.

#### SoulHarvester

#### SoulHarvester

 Level 3

JoinedJul 29, 2022Messages11

Ravager16829 said:

 You have to use Ladik's Casc Viewer, then locate this path:

"war3.w3mod\ui\console\"

View attachment 499644

For a safe option, make sure you create a folder yourself with the path "ui\console\[your-race]" in your game root, place the required BLP files on that folder, and then activate the Local Files feature.

If you want to revert the changes, simply delete that folder in your game root OR disable the Local Files feature.

I highly suggest NOT using any third-party mods like yours (you are using the Rebirth), as it can mess up the game.


Click to expand...

Unfortunately after i do the steps i get the green texture sidebars like in the below example, see attached screenshot

Tried the method with both vanilla 1.31.1 & modded Rebirth.

In this thread it says that you need v1.32+ for the fix to work but i found another thread where they say that it works on 1.31 as well

###
 [Solved] - Removing Extended Ui Help [Patch 1.30.4] (Edit: not possible without having Black Bars, upgrade to 1.31.1 to remove them)


Hello long time lurker hopefully someone can help me with this issue since im losing my mind over this... I couldn't find a thread about the Issue i am having and have tried many searches and most of the Solutions were for the current Version 1.32+ but since i downgraded to 1.30.4 for some...

 www.hiveworkshop.com


Do you know if there's any workaround?

Thanks in advance

#### Attachments

Untitled.png

 2.2 MB

 · Views: 123



#### Ravager16829

#### Ravager16829

 Level 40

JoinedMay 14, 2021Messages2,455

SoulHarvester said:

 Unfortunately after i do the steps i get the green texture sidebars like in the below example, see attached screenshot

Tried the method with both vanilla 1.31.1 & modded Rebirth.

In this thread it says that you need v1.32+ for the fix to work but i found another thread where they say that it works on 1.31 as well

###
 [Solved] - Removing Extended Ui Help [Patch 1.30.4] (Edit: not possible without having Black Bars, upgrade to 1.31.1 to remove them)


Hello long time lurker hopefully someone can help me with this issue since im losing my mind over this... I couldn't find a thread about the Issue i am having and have tried many searches and most of the Solutions were for the current Version 1.32+ but since i downgraded to 1.30.4 for some...

 www.hiveworkshop.com


Do you know if there's any workaround?

Thanks in advance


Click to expand...

First, make sure that your DDS files are converted into BLP. This because in older versions (pre-1.32), the game only reads models written in BLP format.

In the thread you mentioned here, you have to also extract this file:

"war3.mod/ui/framedef/consoleui.fdf"

(I don't know if it's a correct one, but according to Retera, it's a file for the Extended UI Panels)

Like this:

Now, this is really complicated. You have to modify that file so that both console 5 and 6 boxes will be removed. Those boxes are the green textures that's causing your problem with this mod.

After that, place your modified "UI/Console/[Race]/[Race]UITile05.blp" and "UI/Console/[Race]/[Race]UITile06.blp" into your game root with the Local Files being enabled.

I can't guarantee if this trick still works on pre-1.32. Retera said in the thread that if you do this on pre-1.31, the black bars will remain intact.

#### SoulHarvester

#### SoulHarvester

 Level 3

JoinedJul 29, 2022Messages11

I tried the exact steps:

1. Converted the .dds to .blp

2. Extracted the consoleui.fdf and deleted the blocks of code for overlay 5 & 6 which are the side panels

3. Pasted the ui folder to the game root

Sadly i get both black and green bars now

Messaged Retera as well and this was his explanation:

"I think this can be solved although I have not used 1.31 in quite some time and most of the time was derping around on my Warsmash thing which is sort of its own creative space in terms of which patches it uses.

The modified Azeroth Grand Prix in the video there was running on patch 1.31 and what I did was a very simple change using the UI functions that patch 1.31 added for custom maps. Using these functions, you can get the 3D game world into a program variable. Something like:

Code:

local framehandle myWorldFrame = BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0)

or something. Once we have a reference to that, we can use BlzFrameSetPoint or whatever, and set the points of the 3D frame to the corners of screen instead of the offsets with the "black bars." Basically, if we think about it this way, the black bars are the absence of rendering, rather than a black area drawn over top of a render. So we are making the 3D world render larger.

If you can create this function to run in a custom world editor map like this, such as I did on Azeroth Grand Prix racing game, then you can actually insert the same function into the game to run on every map using Local Files similar to how you can override the UI art. To do this, edit the code for Scripts\Blizzard.j file -- copying what was in the map, to put it into this file instead. However, you might want to be careful. I have heard rumors that these UI functions that were added in 1.31 might actually crash if the game program is minimized during the load screen. So if you close Warcraft 3 to browse the web while the game loads, this code might cause a crash. To fix it, there is a different map script API function added to check if the game is minimized or not - so I think you could check it repeatedly after the map starts and then expand the "World Frame" render to fill in the black bars once the game is full screen.

However, I forget if the function to check if the game is minimized or not was added in 1.32 or 1.31, so you probably want to check.

I believe that what I have outlined here is an accurate and very real solution to this problem that you can use without further advanced hacking knowledge, and basically only with World Editor knowledge and then with Local Files to apply to all games. However, because I was feeling lazy at this moment, and my 1.31 backup is away at my other house which I will not visit for a week or two, I did not try to accomplish this change for you yet myself. My suggestion is to have a go and see if you can figure it out, and if not, maybe check back with me in a bit. I am extremely confident the premise for this solution is sound, and should be technically possible."

To my understanding i need to insert the above mentioned line of code in the blizzard.j file but what was not clear is if i need to open file in the world editor or in a third party app like notepad++

Also not clear where exactly i must add the line of code, because if i add it to the end of the file for example, i get black screen upon loading any mission/map.

#### Ravager16829

#### Ravager16829

 Level 40

JoinedMay 14, 2021Messages2,455

SoulHarvester said:

 I tried the exact steps:

1. Converted the .dds to .blp

2. Extracted the consoleui.fdf and deleted the blocks of code for overlay 5 & 6 which are the side panels

3. Pasted the ui folder to the game root

Sadly i get both black and green bars now

Messaged Retera as well and this was his explanation:

"I think this can be solved although I have not used 1.31 in quite some time and most of the time was derping around on my Warsmash thing which is sort of its own creative space in terms of which patches it uses.

The modified Azeroth Grand Prix in the video there was running on patch 1.31 and what I did was a very simple change using the UI functions that patch 1.31 added for custom maps. Using these functions, you can get the 3D game world into a program variable. Something like:

Code:

local framehandle myWorldFrame = BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0)

or something. Once we have a reference to that, we can use BlzFrameSetPoint or whatever, and set the points of the 3D frame to the corners of screen instead of the offsets with the "black bars." Basically, if we think about it this way, the black bars are the absence of rendering, rather than a black area drawn over top of a render. So we are making the 3D world render larger.

If you can create this function to run in a custom world editor map like this, such as I did on Azeroth Grand Prix racing game, then you can actually insert the same function into the game to run on every map using Local Files similar to how you can override the UI art. To do this, edit the code for Scripts\Blizzard.j file -- copying what was in the map, to put it into this file instead. However, you might want to be careful. I have heard rumors that these UI functions that were added in 1.31 might actually crash if the game program is minimized during the load screen. So if you close Warcraft 3 to browse the web while the game loads, this code might cause a crash. To fix it, there is a different map script API function added to check if the game is minimized or not - so I think you could check it repeatedly after the map starts and then expand the "World Frame" render to fill in the black bars once the game is full screen.

However, I forget if the function to check if the game is minimized or not was added in 1.32 or 1.31, so you probably want to check.

I believe that what I have outlined here is an accurate and very real solution to this problem that you can use without further advanced hacking knowledge, and basically only with World Editor knowledge and then with Local Files to apply to all games. However, because I was feeling lazy at this moment, and my 1.31 backup is away at my other house which I will not visit for a week or two, I did not try to accomplish this change for you yet myself. My suggestion is to have a go and see if you can figure it out, and if not, maybe check back with me in a bit. I am extremely confident the premise for this solution is sound, and should be technically possible."

To my understanding i need to insert the above mentioned line of code in the blizzard.j file but what was not clear is if i need to open file in the world editor or in a third party app like notepad++

Also not clear where exactly i must add the line of code, because if i add it to the end of the file for example, i get black screen upon loading any mission/map.


Click to expand...

Sadly, I can't provide further solution for your problem, considering the fact the original author in this thread had made this mod to be applicable on Reforged.

Plus, if you are using the latest patch (2.0), you can toggle on/off side panels manually.

#### SoulHarvester

#### SoulHarvester

 Level 3

JoinedJul 29, 2022Messages11

Ravager16829 said:

 Sadly, I can't provide further solution for your problem, considering the fact the original author in this thread had made this mod to be applicable on Reforged.

Plus, if you are using the latest patch (2.0), you can toggle on/off side panels manually.


Click to expand...

Understood,

Thank you very much for all your help in trying to solve the topic.

 You must log in or register to reply here.


### Similar threads

Locked

 Changing the time indicator skin


Super Mario

Jun 6, 2023

Texturing & Surfacing

Replies2Views2K

Oct 9, 2023
Insanity_AI

[Solved]
 How do you replace the UI? (SD, latest game version)


loktar

Aug 12, 2025

World Editor Help Zone

Replies0Views405

Aug 12, 2025
loktar

Locked

 war3mapskin.txt UI images


FeelsGoodMan

Aug 18, 2019

World Editor Help Zone

Replies1Views2K

Aug 18, 2019
AbsolutePsycho

Locked

 UI bugged?


Chaosy

Dec 21, 2013

World Editor Help Zone

Replies2Views2K

Dec 21, 2013
Chaosy

Locked

[Miscellanous / Other]
 Console UI Minimizer (Modification)


A Void

Mar 25, 2014

Map Development

Replies12Views4K

Jun 23, 2014
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