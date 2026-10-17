---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/comprehensive-tutorial-on-fonts.348087/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Comprehensive Tutorial on Fonts
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/comprehensive-tutorial-on-fonts.348087/>
#### Remixer

#### Remixer

Map Reviewer

 Level 34

JoinedFeb 19, 2011Messages2,130

Resources: 85

#### Resources
Models 31Icons 7Packs 1Skins 5Maps 39Templates 1Tutorials 1

## Comprehensive Tutorial on Fonts​

This tutorial aims to explain how fonts work in Warcraft III and how to change and alter them to your needs. I utilized many older posts and threads to structure this tutorial and to gain more knowledge myself. In this tutorial, I have attempted to update some of the knowledge and bring all that information into one place. This tutorial focuses on customizing fonts on map basis, so if you are looking to work with fonts outside that scope this tutorial might not answer your questions directly, but it might still contain very useful information.

 Table of Contents


1.0 Fonts of Warcraft - Brief overview of fonts' file format.

1.1 Default Font Directories - Where to find fonts Warcraft III uses.

1.2 Default Fonts - How Warcraft III uses its default fonts

1.3 Changing Fonts - How to customize font usage

1.4 Fonts and Language - How language settings affect fonts

1.5 Scaling Fonts - How to further modify fonts

1.6. Example Applications - Examples on how one can use fonts

1.7 Importing Textfiles - Notes and instructions on working with .txt imports

1.8 Credits and acknowdlegments

## 1.0 Fonts of Warcraft​

Warcraft III comes natively with a number of fonts. All default fonts of Warcraft III are in TrueType Font (.ttf) format but the game can also work with OpenType Font (.otf) format [Source] as well as TrueType Font Collection (.ttc) format. The fonts directory (file path) depends on whether the game version used is based on the MPQ archive format (Pre-Reforged) or the Casc archive format (Post-Reforged).

## 1.1 Default Font Directories​

In order to access the default font files you need appropriate tools to open those file archives. You can use tools such as Magos' Model Editor for older game versions - for the newer versions you can use Ladik's Casc Viewer or similar tools. The older game versions contain seven fonts, while the new Reforged version contains 24 different font files. Various fonts come with different variants - often indicated by abbreviations in the fonts' names - this guide can help you read numerous abbreviations used in font naming.

Pre-Reforged fonts are located in war3mpq/Fonts

Post-Reforged fonts are stored in war3.w3mod/fonts

Note, however, that to edit said font files you will need a separate program - one that can edit font files. One example of such a tool is FontForge, and it is free to use. Creating your own fonts, however, is not easy!

## 1.2 Default Fonts​

Warcraft III defines which fonts it uses and where through the war3skins.txtfile which is located in war3mpq/UI (Pre-Reforged) and war3.w3mod/ui (Reforged). In this file, the game defines a few areas of the game interface where different fonts are used. Each of the font lines below represents a distinct font that Warcraft III uses in a specific place:

 Font Classes of Warcraft III


MasterFont - Changes the font used in areas not specified in the lines below such as life, mana, and resource bar.

MasterFontBold - Change unknown - only available in Reforged.

MessageFont - Changes the font of game messages (not player chat font).

ChatFont - Changes the "Message:" and "All/Allies" while typing a chat message. Does not change the chat message font.

EscMenuTextFont - Changes the font of interface buttons and interface elements.

InfoPanelTextFont - Changes the font in the user interface info panel (Unit Name, Attack, Armor, Hero Stats Fonts).

TextTagFont - Changes the font of game-generated text tags such as critical hits, misses, and experience yields.

Accessing this file however is not really relevant if you wish to change the font within your map since that can be done by importing a war3mapskin.txt file - note that this text file is not the same as war3skins.txt. More about this file is in the next section of this tutorial.

## 1.3 Changing Fonts​

While creating your own fonts can be challenging, it is also something you do not need to do. The internet offers you countless ready-made fonts, and even your operating system offers dozens of different fonts. On Windows, you can find your system's fonts from the C:\Windows\Fonts folder. When choosing to use one of these fonts for Warcraft III, remember to make sure that the file format is suitable!

Customizing font usage in a map is relatively easy. It can be done through the Asset Manager (previously Import Manager) in the World Editor. In addition to the font file itself, the map needs a text file to tell which font to use where, if you wish the game to use non-default font settings. This text file, war3mapskin.txt, is simple to create - or you can download it from the attached files below. This process consists of two parts:

a) Importing a desired font into your map. For ease of use and clarity, you should remove the "war3mapImported\" part of the name after importing, since it's unnecessary and allows our text files (below) to look cleaner and easier to edit.

b) Editing your war3mapskin.txt accordingly and importing it to your map. Instructions to do this are below.

Warning: Take a backup of the map you work with! Text files such as war3mapMisc.txt may already exist in your map, and importing a new one will replace the old one! This may potentially lead to loss of data!
The text file must be named "war3mapskin.txt" in the Asset Manager and in it, you get to define the usage of fonts as follows:

 Example 1:


[CustomSkin]

MasterFont=NameOfYourFont.ttf

MasterFontBold=NameOfYourFont.ttf

MessageFont=NameOfYourFont.ttf

ChatFont=NameOfYourFont.ttf

EscMenuTextFont=NameOfYourFont.ttf

InfoPanelTextFont=NameOfYourFont.ttf

TextTagFont=NameOfYourFont.ttf

You can copy-paste the text from the box above into your war3mapskin.txt. Depending on which fonts you wish to change you can choose which of the mentioned lines you wish to include in your war3mapskin.txt file. The MasterFontBold option is only found in Reforged versions, so it might not work in classic versions of the game. If, for example, you only wish to change the font on the menu elements into Arial font then your text file should look like this:

 Example 2:


[CustomSkin]

EscMenuTextFont=Arial.ttf

If you wish to change the font of the players' chat messages you need to modify the map's war3mapMisc.txt file the same principle works there - add a couple of lines of text to change the players' chat font as follows:

 Example 3


 [ChatFonts]

ChatFont0=NameOfYourFont.ttf


## 1.4. Fonts and Language​

Different language versions use different fonts by default - while the English version of Warcraft defines BlizQuadrata Web as the default font (on all of the different font classes), the Spanish version for example replaces this default font with FrizQuadrata TT font. The differentiation of different languages with fonts allows the usage of fonts that do not have all the symbols often used in certain languages.

In addition to the default font definitions in war3skin.txt, you can also define fonts for a specific language mode of the game. The language-specific definitions can be set in the war3mapskin.txt file by further modification of the file. Note however that this feature might only work in Warcraft III Reforged - previous game versions do not list these data options in their files.

 Example 4


MasterFont:enUS=NameOfYourFont.ttf

As you can see the additional ":enUS" lets us define that for United States English the MasterFont uses the defined font (remember to include the colon after the font class). Possible language-specific follow the ISO Language Code and possible prefixes used by Warcraft III are:

csCZ (Czech)

deDE (German)

enUS (English - United States)

esES (Spanish)

esMX (Spanish - Mexico)

frFR (French)

itIT (Italian)

jaJP (Japanese)

koKR (Korean)

plPL (Polish)

ptBR (Portuguese)

ruRU (Russia)

thTH (Thai)

zhCN (Chinese)

zhTW (Chinese - Taiwan)

## 1.5 Scaling Fonts​

Naturally different fonts suit different purposes - fonts that create long sentences or rows of text should be easily readable, whereas more artistic decorative fonts may work in smaller quantities, for example as a text font in the user interface elements. Due to how different fonts may look, you may need to adjust them when applying them for use in Warcraft III.

While scaling the fonts themselves in the editor is not possible, we can scale the text elements of the game. This can be done through the war3mapMisc.txt file - note that it is not the same as the war3mapskin.txt file! The following list contains the possible font scaling options that one can define in war3mapMisc.txt, set to their default values. Note that some of mentioned font scalings are heavily restricted - such as the PortraitStats, which sets the font size of the unit's health and mana below the portrait. Values (using default fonts) above 17 make the numbers disappear completely, as they no longer fit into the frame. You can also make the text disappear by setting the value of the respective text section to 0.

Data Line:Explanation:

[FontHeights]

 ToolTipName=0.011

 ToolTipDesc=0.011

 ToolTipCost=0.011

 ToolTipHotkey=0.011

 ChatEditBar=0.013

 ChatHistory=0.011

 CommandButtonNumber=0.009

 WorldFrameMessage=0.015

 WorldFrameTopMessage=0.024

 WorldFrameUnitMessage=0.015

 WorldFrameChatMessage=0.013

 Inventory=0.011

 LeaderBoard=0.011

 PortraitStats=0.011

 QuestDescription=0.011

 UnitTipPlayerName=0.011

 UnitTipUnitName=0.011

 UnitTipDesc=0.011

 CommandButtonCooldownNum=0.016

 HeroFrameLevel=0.012

 CheckpointReached=0.015This line tells Warcraft which values are below it (what it is reading).

 Tooltip name ("Footman").

 Tooltip description ("Versatile foot soldier").

 Tooltip cost (gold, lumber, food, and mana cost).

Doesn't seem to affect anything.

 Chat edit bar text.

 Chat history dialog.

 The little number in the bottom right of the command buttons and items.

 A single line of text which appears above the console ("Invalid target!").

 Upkeep notification text ("High Upkeep!").

 Triggered dialog text of units ("Arthas: We must purge this city!").

 In-game chat text.

Doesn't seem to affect anything.

 Leader board text.

 Health and mana below the portrait - restricted by frame size.

 Quest description text.

 Player name on unit tooltips.

 Unit name on unit tooltips.

 Description of units on tooltips.

 The cooldown countdown number, if enabled.

 A small number that indicates Hero's level on the hero frame.

 Checkpoint reached.

In addition to these text section settings, you can also modify additional texts through the same war3mapMisc.txt file, but this time in the [Misc] section. These are texts that appear within the game scene window - like the "miss" text caused by evasion.

Data Line:Explanation:

[Misc]

 GoldTextHeight=0.024

 LumberTextHeight=0.024

 BountyTextHeight=0.024

 LumberBountyTextHeight=0.024

 XPTextHeight=0.020

 MissTextHeight=0.020

 CriticalStrikeTextHeight=0.024

 ShadowStrikeTextHeight=0.024

 ManaBurnTextHeight=0.024

 BashTextHeight=0.024This line tells Warcraft III what data it is reading.

 Gold gained when workers return it, or gold gained from items.

 Lumber gained when workers return it, or lumber gained from items.

 Gold gained from killing unit or creeps - "bounty".

 Lumber gained from killing units or creeps - "bounty".

 The experience gained font size.

 "Miss" caused by evasion.

 Critical strike hit text.

 Shadow strike damage tag.

 Mana burn damage tag.

 Bash hit text.

The same logic applies to these definitions as with the fonts: different language settings use different fonts, which is why one might want to define a separate value different from default ones for a certain language - this is also what Blizzard did. So, if for example, you would want to change the size of the Quest description text only for English players you would declare it with the line "QuestDescription:enUS=YourValueHere". This can be a smart choice especially if you do not force a font universally on all players - if someone is playing with a different font setting due to their game's language setting, the game can become unreadable from their perspective, while it might look good to you.

## 1.6 Example Applications​

With all these options available one can customize players' experience quite a lot, from changing the visual look of game messages, and menus as well as ability descriptions. Through altering existing fonts and re-importing them into the map one can also make subtle changes - such as creating new symbols to indicate things such as physical and magical damage, ability range, or for example, hero attributes. One user can also be to switch a symbol to another for ability descriptions - such as the %-symbol, which may cause problems in LUA coding. Hypothetically, one could replace a symbol not used anywhere in ability tooltips with a % symbol, thus allowing you to use the %-sign even in LUA strings.

Through these methods, users can also make the infamous critical hit floating texts disappear once and for all - although they can be altered also through other text files, which may be a more efficient way of ridding them.

Below is a screenshot that tries to showcase examples of what areas represent which font settings and which areas you can, for example, modify individually.

## 1.7 Importing of Textfiles​

Now, unlike usual imports of files such as models or textures, textfiles may be a staple part of a map. Customizing Gameplay constants for example creates a war3mapMisc.txt file in the map's archive and thus, it becomes an integral part of the map's file archive. In this scenario, even if you remove the imported war3mapMisc.txt file, the changes it applied are still part of the map. In order to undo the changes caused by this import, you need to edit the war3mapMisc.txt file inside the map's archive or replace the changes by re-importing the file again with the wanted changes. Warcraft III World Editor does a lousy job at keeping track of imports and thus, I recommend always testing fonts and their changes on a brand-new, empty map. This is because, even if you import a new war3mapMisc.txt file into your map, its data might not replace the previous data, and you don't apply any changes, even if you think you do. One option is to work with the Folder saving mode, but even this does not always bring over the newest data.

Also note that if your map already has either war3mapMisc.txt or war3mapskin.txt file in its archives, you should not re-import them, but instead edit the existing ones (either export them and edit them with notepad or open the files directly inside the archive through a tool).

## 1.8 Credits and acknowledgments​

Thank you for reading the tutorial. It was largely created due to my personal needs and interest in the topic. This tutorial utilizes a number of posts, threads, tutorials, and websites and it would not have been done without them.

### Thanks to:​
Hodge-Podge for their tutorial.

ap0calypsefor their previous contribution to the topic.

M0RT for their previous contribution to the topic.

Svenski for their previous contribution to the topic.

#### Attachments

war3mapMisc.txt

 723 bytes

 · Views: 180



war3mapskin.txt

 212 bytes

 · Views: 173




 Last edited: Feb 24, 2026

#### Darkcranio

#### Darkcranio

 Level 5

JoinedJun 25, 2020Messages26

Thanks! I want to play the campaign on 1.29 for widescreen, but I noticed the font spacing in messed up. They are mashed together, where can I modify that?


 Last edited: Apr 8, 2024

#### Remixer

#### Remixer

Map Reviewer

 Level 34

JoinedFeb 19, 2011Messages2,130

Resources: 85

#### Resources
Models 31Icons 7Packs 1Skins 5Maps 39Templates 1Tutorials 1

Darkcranio said:

 Thanks! I want to play the campaign on 1.29 for widescreen, but I noticed the font spacing in messed up. They are mashed together, where can I modify that?


Click to expand...

Hello! It is hard to say what causes the issues without seeing them: is the font issue or some aspect ratio issue? The font spacing was changed in 1.33 (with the old one accessible through -legacykerning -command), but this should not affect you, taking you are playing on version 1.29.

#### Darkcranio

#### Darkcranio

 Level 5

JoinedJun 25, 2020Messages26

Remixer said:

 Hello! It is hard to say what causes the issues without seeing them: is the font issue or some aspect ratio issue? The font spacing was changed in 1.33 (with the old one accessible through -legacykerning -command), but this should not affect you, taking you are playing on version 1.29.


Click to expand...

Def aspect ratio, since changing back to lower res solved it in the while, but again that's not a definitive solution cause I want to play on 4k. I'm on 1.31(changed it cause 1.29 were missing ambiance, but 1.31 still has the same issue).


 Last edited: Apr 11, 2024

#### Xecutor_ShamanX

#### Xecutor_ShamanX

 Level 12

JoinedDec 14, 2016Messages125

Resources: 5

#### Resources
Models 5

Great tutorial! I got it right in my first attempt, though I faced one small problem. The unit names, attack and armor type have their fonts in default but the rest are modified. I was thinking if there must be another thing that needs to be defined that wasn't mentioned in the tutorial.

Screen messages are good !

Still not have been changed :/

EDIT: Nevermind. Fixed it because I put an equal sign here, from this...

 InfoPanelTextFontNameOfYourFont.ttf


Click to expand...

To this.

 InfoPanelTextFont=NameOfYourFont.ttf


Click to expand...

Please update that part.


 Last edited: Jan 2, 2025

 You must log in or register to reply here.


### Similar threads

 In-Game Font Overhaul


Hodge-Podge

Feb 22, 2010

Misc/General Mapping Tutorials
2

Replies62Views71K

Nov 24, 2017
datsatthune

 The Sound Editor


Bob27

Aug 29, 2007

Misc/General Mapping Tutorials

Replies11Views33K

Feb 21, 2015
Map Designer

 Advanced Terraining Tutorial


Blood Raven

Sep 9, 2011

Misc/General Mapping Tutorials
2

Replies55Views83K

Oct 1, 2020
Blood Raven

 How to add custom background to your custom campaign


CustomCreations

Feb 6, 2023

Misc/General Mapping Tutorials

Replies4Views4K

Feb 16, 2025
MapMaker14

 Importing/Exporting Object Data from WE


RedBlade

May 24, 2007

Misc/General Mapping Tutorials

Replies17Views21K

Aug 7, 2021
ThePerfectIdiot

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