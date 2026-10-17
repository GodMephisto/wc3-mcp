---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/icons-guide.7123/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Icons Guide
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/icons-guide.7123/>
#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


Introduction


Warcraft icons are used for many things. They are used for abilities, items, units and buildings. Everything in Warcraft have an icon. Icons are in the format of 64x64 pixels in .blp or .tga format. This guide will tell you about how to make an icon and what you need to have in being able to make it. I will also be explaining how to import an icon correctly and how to use one disabled (DISBTN) icon for more than one enabled icon. I will also give you some helpful resources to easier make a good-looking icon.

Downloading and installing custom icons


There exists many websites for downloading custom icons. Here I will list some of them.

Wc3campaigns.net


This website spends a lot of energy in only having the very best icons. This also results in a resource section with not many icons.

HiveWorkshop.com


I will also like to introduce you to the Hive's version of a resource section. This section is not that filled yet but you can help me fill it out with lots of great icons.

After the download


When you have downloaded an icon, you need to know where to put it, in order to make the icon being displayed correctly. Many people makes an ability/item/etc. link to an icon that isn't in the right path and may be displayed incorrectly. All icons also require a disabled version of itself. A disabled version is one looking like the original one but just a lot darker to show that this icon is not enabled at the moment. Disabled icons are used when looking at someone else's hero and when you have opened the game menu. If an icon doesn't have a disabled version of itself, a bright green color will be displayed instead, this is not a cool feature so always remember your disabled icons.


You start by opening the "Import Manager" in World Editor. Then you press "Import" and import the icons you have downloaded. Then double-click on each file and tick the "Custom Path" and set the path to the below settings according to which icon it is.

The Enabled Icon


The enabled icon is the most used icon, it is used for everything, but when you are looking on some other players unit or when you have opened a dialog or the game menu, every icon will be switched to the disabled version.


The enabled icon is usually located in:

ReplaceableTextures\CommandButtons\BTNIconName.blp


But it actually doesn't matter where you put the icon. So I will save it as:

TheFunnyIcon/IconName.blp

The Disabled Icon


The disabled icon is often skipped. Some people just delete it to save room but it really makes the custom icon feel more real when you also can see a disabled version instead of just some bright green color which will be displayed when a disabled icon is not found. The disabled icon should ALWAYS be located in:

ReplaceableTextures\CommandButtonsDisabled\DISIconName.blp


Replace the "IconName" with the IconName from the enabled icon and it will always work. Even if the Enabled icon is in some other path, the disabled version should always be located here. But remember if the IconName begins with "BTN" the disabled IconName should also be named with "BTN" but after the "DIS".

Several Icons With Same Disabled Icon


You can actually have more enabled icons using the same disabled icon as long as they have the same IconName. Here's an example:

ReplaceableTextures\CommandButtons\BTNRalle.blp

SomeRandomName\BTNRalle.blp

BurningCow\BTNRalle.blp


All these icons have the same iconname and they will all look for the disabled version in the path:

ReplaceableTextures\CommandButtonsDisabled\DISBTNRalle.blp


This feature is very good to save some room if you have many icons which just have different colors, then make the disabled version black and white.

Green Icon Problem(by shados)


Occaisonally when using custom icons a flouro green square will appear instead of your icon, this green icon is what Warcraft III shows when it can't find the icon. Often you will get a green default icon when Warcraft III attempts to show the disabled version of an icon you've imported and cannot find it, this could be because you haven't imported a disabled version of the icon or simply because your icons aren't named correctly. So you have to be more careful and maybe read through this tutorial again or post your paths for getting help.

Creating An Icon


Being able to make your own icons is often quite nescessary when making your own map. But it's actually not that hard. You only need one tool and that's the Warcraft III Image Extractor II.


Firstly you need to decide whether it is a passive, active or autocast icon. For the active icon, you need to use this border: View attachment Icon_Border.TGA.


And for the autocast icon, you need this border: View attachment Icon_Border_Autocast.TGA.


Passive icons doesn't have a border. They should be fading to black where the borders are.


All disabled icons should look like the passive icon just more darkened. Here's Try browsing around on the websites listed in the top to see how other people make their icons before you start.


When you have made your icons, you need to convert them to either .tga or .blp and import them as written above.

Sources

Archian's tutorial ~ Importing an Icon

Green Icon Fix


[THREAD=7082]Healingpoison's tutorial ~ Making an Icon[/THREAD]


by Ralle

#### Attachments

Icon_Borders.psd

 34.6 KB

 · Views: 1,551




 Last edited by a moderator: Aug 10, 2007

 You must log in or register to reply here.


### Similar threads

S

 Button Manager Tutorial (Custom Icons)


sentrywiz

Sep 27, 2014

2D Art Tutorials

Replies11Views11K

Jun 22, 2015
PurgeandFire

 Full Icon Tutorial (Reforged) + ¨tool¨


Barorque

Feb 21, 2020

2D Art Tutorials

Replies39Views26K

Jan 22, 2026
Michael Peppers

Locked

 Green Icon Fix


Shados

Oct 26, 2006

Tutorial Archive

Replies6Views3K

Oct 27, 2006
ragingspeedhorn

 Importing And Fixing Icons (RoC and TFT)


ElfWarfare

Feb 12, 2005

2D Art Tutorials

Replies0Views11K

Feb 12, 2005
ElfWarfare

 Complete Icon Tutorial - All About Icons


Flame_Phoenix

Aug 5, 2007

2D Art Tutorials
23

Replies129Views115K

Aug 25, 2020
TheFireMage

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

2D Art Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top