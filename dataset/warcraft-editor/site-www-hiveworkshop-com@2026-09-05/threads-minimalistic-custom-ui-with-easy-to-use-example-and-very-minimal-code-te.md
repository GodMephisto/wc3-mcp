---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/minimalistic-custom-ui-with-easy-to-use-example-and-very-minimal-code-tested-for-reforged.336291/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Minimalistic Custom UI with Easy to Use Example and Very Minimal Code Tested for Reforged
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/minimalistic-custom-ui-with-easy-to-use-example-and-very-minimal-code-tested-for-reforged.336291/>
#### Seedro

#### Seedro

 Level 6

JoinedDec 4, 2019Messages49

Resources: 1

#### Resources
Tutorials 1

I started with Gothic Minimalistic UI Widescreen and used some basic easy filters to make it as minimal as possible while still showing all info in paint.net (could probably go further), I converted it using BLP Lab v0.5.0 and used https://www.hiveworkshop.com/attachments/teamglow-blp.19941/ as a blank .blp. I also used the Reforged UI Maker to get the default UI removed when possible.

Instructions:

Import TeamGlow.blp linked above and the 4 other ".blp" files attached to this post into the Asset Manager -> Import File

Go into Advanced -> Game Interface

Check "Use Custom Game Interface"

Scroll down to these values and change them to the ones from downloaded attachments and TeamGlow.blp clicking Edit Value -> Import and search for the imported files (the default will have "war3mapImported\" in front, it is common to remove that part but not necessary for this tutorial):

- Now input the JASS code below into a "New Custom Script" in Trigger Editor:

Code:

 JASS:

library NewGUILIB initializer init
private function init takes nothing returns nothing

call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT, 0), false) // Remove portrait, causes update issues

call BlzFrameSetVisible(BlzGetFrameByName("ConsoleUIBackdrop",0), false) // Remove black bar

endfunction
endlibrary

- That's it!

End Result:

https://www.hiveworkshop.com/attachments/ss-raptor-t1-left-png.434660/ (has some reshade and uses the code below to change hero icon/life/mana position, from the project in my signature)

Interesting alternative JASS code if you want your hero life/mana more centered in the empty portrait spot (looks bad if you have more than two heroes, good for RPGs):

 JASS:

library NewGUILIB initializer init
private function init takes nothing returns nothing

call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT, 0), false) // Remove portrait, causes update issues

call BlzFrameSetVisible(BlzGetFrameByName("ConsoleUIBackdrop",0), false) // Remove black bar

call BlzFrameSetAbsPoint(BlzGetOriginFrame(ORIGIN_FRAME_HERO_BAR,0), FRAMEPOINT_TOPLEFT, 0.26, 0.16) // Top left corner of new bar position
call BlzFrameSetAbsPoint(BlzGetOriginFrame(ORIGIN_FRAME_HERO_BAR,0), FRAMEPOINT_BOTTOMRIGHT, 0.31, -0.18) // Bottom right corner

call BlzEnableUIAutoPosition(false) // Prevents resizing window changing GUI position

endfunction
endlibrary

Issues:

The health doesn't update as often as it should due to disabled portrait, I might make a portrait version that updates if anyone is interested, I went without portrait for space and the fact that my projects had bugs with the portrait. Removing the line "call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT, 0), false)" will fix it but then you'll have to make a good looking .blp to work with it.

#### Attachments

HumanUITile01.blp

 79.6 KB

 · Views: 118



HumanUITile02.blp

 103.8 KB

 · Views: 95



HumanUITile03.blp

 54.3 KB

 · Views: 101



HumanUITile04.blp

 22.1 KB

 · Views: 84




 Last edited: Jun 5, 2023

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I.. don't think this quite classifies as a tutorial.

In my opinion, the difficult part is skimmed over.

A tutorial should teach you something.

What can I take away from this? how to import files? how to replace files in the interface settings?

#### Seedro

#### Seedro

 Level 6

JoinedDec 4, 2019Messages49

Resources: 1

#### Resources
Tutorials 1

noted and changed

#### Mayday

#### Mayday

 Level 22

JoinedMay 1, 2018Messages333

Resources: 17

#### Resources
Maps 3Spells and Systems 3Reforged HD Icons 2User Interface 4Lua 1Tutorials 4

I also believe as Chaosy does, this doesn't really belong as a tutorial. We do have a Skins section, with a subsection for User Interface though, where this could be used.

My issues with the post:

1) You're using a texture type the game does not use anymore, we've moved all in-game files from .blp to .dds; there are filesize and performance benefits to this change.

2) No link to the tool used to make any changes, and no link to the Reforged UI Maker. Also using a deprecated tool, BLPLab.

3) This point is subjective, but you don't ever need to open any of the menu's you have shown to change these files. You can handle this directly inside the Import Manager like so:

4) That code looks entirely useless and abysmally made -

We have technology,

 vJASS:

library SomethingMoreUsefulHere initializer init
 private function init takes nothing returns nothing
 call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT, 0), false) // This line hides the portrait
 call BlzFrameSetVisible(BlzGetFrameByName("ConsoleUIBackdrop",0), false) // This line hides the giant black bar
 endfunction
endlibrary

You had globals, but it was empty, I do not know why it was there.

We also have more than one language these days, it's nice to consider those people too.

 Lua:

do
 local timer = CreateTimer()
 TimerStart( timer, 0, false, function()
 BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT, 0), false) -- This line hides the portrait
 BlzFrameSetVisible(BlzGetFrameByName("ConsoleUIBackdrop",0), false) -- This line hides the giant black bar
 DestroyTimer(timer)
 end)
end

5) You don't have any mention of the Tile05 and Tile06, yet they are in your screenshot, which should also be directly presented in the tutorial and not through another link.

Those two textures kind of stuck out as they are not even really connected, here are two blank imports you can use to hide them (attached at bottom)

The path for those is:

 Human Path:

UI\Console\Human\HumanUITile05.dds

UI\Console\Human\HumanUITile06.dds

Orc Path:

UI\Console\Orc\OrcUITile05.dds

UI\Console\Orc\OrcUITile06.dds

Undead Path:

UI\Console\Undead\UndeadUITile05.dds

UI\Console\Undead\UndeadUITile06.dds

Night Elf Path:

UI\Console\NightElf\NightElfUITile05.dds

UI\Console\NightElf\NightElfUITile06.dds

Click to expand...

6) I don't really understand the purpose of "TeamGlow.blp" replacing the inventory, can you explain what that does a bit more?

7) You mention you "reshade" the image, if you don't plan on telling us how to mimic the end result behavior, I think it would be best to not include that in that presentation.

#### Attachments

Tile05.dds

 144 bytes

 · Views: 85



Tile06.dds

 144 bytes

 · Views: 82



#### Seedro

#### Seedro

 Level 6

JoinedDec 4, 2019Messages49

Resources: 1

#### Resources
Tutorials 1

UPDATE Nov 7, 2022: Using JASS for my project is no longer working due to syntax error, I instead converted it to a trigger with custom script actions, with no need for the library lines, it should look like something like this depending which lines you want to use:

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


If you are on patch 1.33 and you opened your map for the first time on this patch, you need to enable JASSHelper to remove syntax errors.

 You must log in or register to reply here.


### Similar threads

Locked

 unable to resize "ConsoleUIBackdrop" if playing in Online game.


KenArok

Sep 6, 2024

Triggers & Scripts

Replies0Views492

Sep 6, 2024
KenArok

Locked

 Custom UI - find clock and items frames


MyDadBeatsMe

Jan 12, 2022

World Editor Help Zone

Replies2Views1K

Jan 13, 2022
AbsolutePsycho

Locked

[General]
 Blizzard Original Frames Not Hiding.


C.B.King

Jun 25, 2022

World Editor Help Zone

Replies8Views2K

Jul 1, 2022
C.B.King

Locked

[JASS]
 UI - Adjusting Frames


xorkatoss

Dec 4, 2019

Triggers & Scripts

Replies4Views3K

Dec 4, 2019
xorkatoss

Locked

 Broken UI


xorkatoss

Mar 9, 2020

Warcraft Discussion

Replies3Views2K

Mar 10, 2020
xorkatoss

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