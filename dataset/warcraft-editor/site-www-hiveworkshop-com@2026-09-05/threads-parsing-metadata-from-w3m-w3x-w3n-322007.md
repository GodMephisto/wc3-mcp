---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/parsing-metadata-from-w3m-w3x-w3n.322007/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Parsing Metadata from W3m W3x W3n
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/parsing-metadata-from-w3m-w3x-w3n.322007/>
#### Firstrun70

#### Firstrun70

 Level 5

JoinedFeb 2, 2020Messages12

Resources: 4

#### Resources
Tools 2Tutorials 2

General information about parsing metadata from w3m w3x w3n

The author of the article: Firstrun70

mpq - They are like .rar and .zip files. Can be opened with different programs (Ladik's MPQ Editor, WinMPQ)

w3m - this is mpq file, RoC Map. They are playable in RoC and in TFT.

w3x - this is mpq file, TFT Map. They are only playable in TFT.

w3n - this is mpq file, Custom Campaign Map. They are only playable in TFT.

File Format

Many people think that maps have only two formats.: This is 12(18) and 19(25). In fact, there are many more:

HEX 08 (DEC 08) - ~total number of maps 18 - ROC Beta 100 103 110

HEX 0A (DEC 10) - ~total number of maps 3 - ROC Beta 111 112

HEX 0B (DEC 11) - ~total number of maps 5 - ROC Beta 112

HEX 0F (DEC 15) - ~total number of maps 9 - ROC Beta 120

HEX 12 (DEC 18) - ~total number of maps 323 - ROC Beta 130 134, TFT Beta 312 313, ROC-RF Beta 1.32, Demo, ROC 1.00-ROC 1.32

HEX 17 (DEC 23) - ~total number of maps 9 - TFT Beta 299

HEX 18 (DEC 24) - ~total number of maps 16 - TFT Beta 302 304 304a

HEX 19 (DEC 25) - ~total number of maps 618 - TFT Beta 306 310 311 312 314 315, TFT-RF Beta 1.32, TFT 1.07-TFT 1.32

HEX 1A (DEC 26) - ~total number of maps 6 - ROC 1.31.0.12071, TFT 1.31.0.12071

HEX 1B (DEC 27) - ~total number of maps 1 - TFT 1.31.0.12071

HEX 1C (DEC 28) - ~total number of maps 130 - TFT-RF Beta 1.32, ROC 1.31.0.12071, TFT 1.31.0.12071 1.31.1.12164 1.32.0.14481

HEX 1F (DEC 31) - ~total number of maps 879 - ROC-RF Beta 1.32, TFT-RF Beta 1.32, ROC 1.32.0.14481, TFT 1.32.0.14481

- ~total number of maps - this is the approximate number of different official enUS maps at the time of release 1.32.0.14481

- HEX is a hexadecimal value, and DEC is a decimal value

- ROC BETA maps are not visible in regular Warcraft, they need ROC BETA, but they can be launched in regular Warcraft via -loadfile

8\Beta ROC 100-(4)Lost Temple.w3m

8\Beta ROC 100-(4)Tranquil Paths.w3m

8\Beta ROC 100-(6)Dark Forest.w3m

8\Beta ROC 100-(6)Gnoll Wood.w3m

8\Beta ROC 100-(8)Battleground.w3m

8\Beta ROC 100-(8)Golems in the Mist.w3m

8\Beta ROC 103-(4)Lost Temple.w3m

8\Beta ROC 103-(4)Tranquil Paths.w3m

8\Beta ROC 103-(6)Dark Forest.w3m

8\Beta ROC 103-(6)Gnoll Wood.w3m

8\Beta ROC 103-(8)Battleground.w3m

8\Beta ROC 103-(8)Golems in the Mist.w3m

8\Beta ROC 110-(4)Lost Temple.w3m

8\Beta ROC 110-(4)Tranquil Paths.w3m

8\Beta ROC 110-(6)Dark Forest.w3m

8\Beta ROC 110-(6)Gnoll Wood.w3m

8\Beta ROC 110-(8)Battleground.w3m

8\Beta ROC 110-(8)Golems in the Mist.w3m

10\Beta ROC 111-(4)Lost Temple.w3m

10\Beta ROC 112-(6)Dark Forest.w3m

10\Beta ROC 112-(8)Battleground.w3m

11\Beta ROC 112-(12)Divide and Conquer.w3m

11\Beta ROC 112-(4)Lost Temple.w3m

11\Beta ROC 112-(4)Tranquil Paths.w3m

11\Beta ROC 112-(6)Gnoll Wood.w3m

11\Beta ROC 112-(8)Golems in the Mist.w3m

15\Beta ROC 120-(12)Divide and Conquer.w3m

15\Beta ROC 120-(4)Legends.w3m

15\Beta ROC 120-(4)Lost Temple.w3m

15\Beta ROC 120-(4)Tranquil Paths.w3m

15\Beta ROC 120-(6)Dark Forest.w3m

15\Beta ROC 120-(6)Dragon Fire.w3m

15\Beta ROC 120-(6)Gnoll Wood.w3m

15\Beta ROC 120-(8)Battleground.w3m

15\Beta ROC 120-(8)Golems in the Mist.w3m

23\Beta TFT 299-(4)BridgetooNear.w3x

23\Beta TFT 299-(4)HailStone.w3x

23\Beta TFT 299-(4)IceFlow.w3x

23\Beta TFT 299-(4)Islands.w3x

23\Beta TFT 299-(4)Tanaris.w3x

23\Beta TFT 299-(4)TurtleRock.w3x

23\Beta TFT 299-(4)Whirlwind.w3x

23\Beta TFT 299-(6)StranglethornVale.w3x

23\Beta TFT 299-(6)WheelofChaos.w3x

24\Beta TFT 302-(4)HailStone.w3x

24\Beta TFT 302-(4)IceFlow.w3x

24\Beta TFT 302-(4)TurtleRock.w3x

24\Beta TFT 302-(4)Whirlwind.w3x

24\Beta TFT 302-(6)StranglethornVale.w3x

24\Beta TFT 304-(4)BridgeTooNear.w3x

24\Beta TFT 304-(4)HailStone.w3x

24\Beta TFT 304-(4)IceFlow.w3x

24\Beta TFT 304-(4)Islands.w3x

24\Beta TFT 304-(4)Tanaris.w3x

24\Beta TFT 304-(4)TurtleRock.w3x

24\Beta TFT 304-(4)Whirlwind.w3x

24\Beta TFT 304-(6)WheelOfChaos.w3x

24\Beta TFT 304a-(4)Whirlwind.w3x

24\Beta TFT 304a-(6)RiceFields.w3x

24\Beta TFT 304a-(6)StranglethornVale.w3x

26\ROC 1.31.0.12071 multi-(4)Deadlock.w3m

26\ROC 1.31.0.12071 multi-(8)RichMansGarden.w3m

26\TFT 1.31.0.12071 multi-(2)Justice.w3x

26\TFT 1.31.0.12071 multi-(4)BrutalWinter.w3x

26\TFT 1.31.0.12071 multi-(4)Deadfall.w3x

26\TFT 1.31.0.12071 multi-(8)BanewoodBog.w3x

27\TFT 1.31.0.12071 multi-(8)GoldRush.w3x

Rare official maps wc3 file-format 8 10 11 15 23 24 26 27:

mirror - hive (attached to this post)

mirror - wc3 file-format 8 10 11 15 23 24 26 27 by Firstrun70.zip

mirror - wc3 file-format 8 10 11 15 23 24 26 27 by Firstrun70

mirror - MEGA

Common Information:

Open the mpq file through Ladik's MPQ Editor

Open war3map.w3i from there via the HEX editor. (a lot of different, Fairdell HexCmp2 quite comfortable, it is also possible to compare two files with each other)

Addressing in the HEX editor is hexadecimal.

Offset - 16 columns and unlimited rows.

Here it is advisable to learn the material, according to the hexadecimal system in order to understand the principles of the number system.

The number 15 in the decimal system is 0F in the hexadecimal system, and A0 in the hexadecimal is 160 in the decimal.

But for our task, deep knowledge is not needed, we will be guided by rows and columns.

16 columns 0123456789ABCDEF and rows. For example, the address 4D3 is row 4D0 and column 03

The address offset of the values often floats (not stable), because The "Name of the map" or "Author" may be different in length, thereby shifting all other values (as well as for other reasons, for example, various map formats).

In a text editor it is better to open file in Unicode

The best separator between data is 00, you can navigate by it.

In w3i, values can be written both with text and with a link to this text TRIGSTR_xxx

-------------- W3M / W3X -----------------

Below is an example of the value addresses for the map - \TFT 1.26a en\(2)EchoIsles.w3x (map attached to article)

All data for World Editor 1.26

w3x(UTF65001) - Line 1:9 - it means the w3x file is opened in a text editor encoded with Unicode UTF65001, line 1, character 9

The image should be opened in a separate window, but it is better to download (5000px x 8000px):

● Blizzard logo - It is not stored in the map itself, it appears when certain conditions are met

● Map type (Melee - two swords / Custom Scenario - yellow gear) - war3map.j(UTF65001) - ForcePlayerStartLocation is missing = Melee

If in the file war3map.j there is a line "ForcePlayerStartLocation" - then Custom Scenario, otherwise Melee (determines with 95% accuracy)

When saving a map, the World Editor says "This map is no longer considered a melee map because" - it means there will be a Custom Scenario (yellow gear)

The easiest way to create a Melee map (two swords) is to create a new map, add 2 mines, add a second player (Scenario - Player properties), save the map with automatic generating of the starting locations.

● Map format (ROC/TFT) - war3map.w3i(HEX) - Offset 00 - Value HEX 19 (DEC 25) = TFT

● Amount of map saves - war3map.w3i(HEX) - Offset 04 - Value HEX 1F (DEC 31)

● World Editor version - war3map.w3i(HEX) - Offset 08 - Value HEX 17A3 (DEC 6051)

● Name - w3x(UTF65001) - Line 1:9 - Value Echo Isles

● Name - war3map.w3i(UTF65001/HEX)+war3map.wts(UTF65001) - Offset 0C - Value TRIGSTR_004 - STRING 4{Echo Isles}

● Name - war3map.j(UTF65001)+war3map.wts(UTF65001) - call SetMapName - Line 1161:23 - Value TRIGSTR_004 - STRING 4{Echo Isles}

● Author - war3map.w3i(UTF65001/HEX)+war3map.wts(UTF65001) - Offset 18 - Value TRIGSTR_007 - STRING 7{Blizzard Entertainment}

● Author - war3map.j(UTF65001) - Map Author: - Line 8:18 - Value Blizzard Entertainment

● Description - war3map.w3i(UTF65001/HEX)+war3map.wts(UTF65001) - Offset 24 - Value TRIGSTR_006 - STRING 6{Though these islands are surrounded by deep...}

● Description - war3map.j(UTF65001)+war3map.wts(UTF65001) - call SetMapDescription - Line 1162:30 - Value TRIGSTR_006 - STRING 6{Though these islands are surrounded by deep...}

● Tileset - war3map.w3i(HEX) - Offset 78 - Value HEX 4C = L = Lordaeron Summer (see Tileset list)

● Tileset - war3map.w3e(ANSI) - Line 1:9 - Value L = Lordaeron Summer (see Tileset list)

● Recommended Number of Players - war3map.w3i(UTF65001/HEX)+war3map.wts(UTF65001) - Offset 30 - Value TRIGSTR_005 - STRING 5{1v1}

● Maximum Number of Players - w3x(HEX) - Offset 17 - Value HEX 02 (DEC 02)

● Maximum Number of Players - war3map.w3i(HEX) - Offset A7 - Value HEX 02 (DEC 02)

● Maximum Number of Players - war3map.j(UTF65001) - call SetPlayers - Line 1633:22 - Value 2

● Maximum Number of Players - war3map.wts(UTF65001) - Line 13:1 - player names (Player 2)

● Map Size (Playable)(Camera Bounds) - war3map.w3i(HEX):

E = Offset 6C - Value HEX 74 (DEC 116)

F = Offset 70 - Value HEX 54 (DEC 84)

Map Size Playable = E x F = 116 x 84

● Map Size (Word) - war3map.w3i(HEX):

E = Offset 6C - Value HEX 74 (DEC 116)

F = Offset 70 - Value HEX 54 (DEC 84)

Map Size Playable = E x F = 116 x 84 = 9744 = Small (see Size Matching Table (below))

● Map Size (Full)(Number) - war3map.w3i(HEX):

A = Offset 5C - Value HEX 06 (DEC 06)

B = Offset 60 - Value HEX 06 (DEC 06)

C = Offset 64 - Value HEX 04 (DEC 04)

D = Offset 68 - Value HEX 08 (DEC 08)

E = Offset 6C - Value HEX 74 (DEC 116)

F = Offset 70 - Value HEX 54 (DEC 84)

Width (DEC) = A + B + E = 6 + 6 + 116 = 128

Height (DEC) = C + D + F = 4 + 8 + 84 = 96

Map Size Full = Width x Height = 128 x 96

● Image of the waiting lobby room (poster) - war3mapPreview.tga or war3mapMap.tga or war3mapMap.blp (written in priority order)

● Image of the loading screen (poster) - war3mapPreview.tga or war3mapMap.tga or war3mapMap.blp (written in priority order)

● Image of the loading screen (full screen) - usually FullScreen.blp (link usually from LoadingScreen.mdx)

● Image of the minimap - war3mapMap.tga or war3mapMap.blp (written in priority order)

war3mapPreview.tga - Image waiting lobby room (poster), loading screen (poster)

war3mapMap.tga - Image minimap, waiting lobby room (poster), loading screen (poster)

war3mapMap.blp - Image minimap, waiting lobby room (poster), loading screen (poster)

war3mapMap.b00 - This file is found only in ROC Beta maps

FullScreen.blp - Image loading screen (full screen)

LoadingScreen.mdx - This is not a picture, but a link to a full-screen picture. It is located in the root /w3x/ or in the folder /w3x/war3imported/ , and the picture itself is usually FullScreen.blp or LoadingScreenBL.tga+LoadingScreenBR.tga+LoadingScreenTL.tga+LoadingScreenTR.tga

Fullscreen loading screen takes precedence over poster screen

Extract image from war3mapMap.blp:

1. extract "war3mapMap.blp" from map file (using StromLib or Strom.dll or MPQ Editor - see MPQ Archives - StormLib)

2. open this blp-file and filter header

3. decompress jpeg data (using a jpeglib, e.g ijl.dll from warcraft dir) (Intel Jpeg Library Version 1.5.4.36)

4. or BLP Lab programm

World Editor:

Blizzard logo - in World Editor is not configurable

Map type (Melee - two swords / Custom Scenario - yellow gear) - Module - Object Manager - Map Type - Melee/Custom / World Editor - Right bottom corner - Melee Map: Yes/No / Module - Trigger Editor - Initialization - Melee Initialization

Map format (ROC/TFT) - Scenario - Map Description - Expansion Required - Yes/No (Automatically configured) / Module - Object Manager - Expansion Required - Yes/No

Amount of map saves - Scenario - Map Description - Map Version

World Editor version - Scenario - Map Description - Saved with Editor Version / Help - About Warcraft III World Editor

Name - Scenario - Map Description - Name / Module - Object Manager - Map Name

Author - Scenario - Map Description - Author

Description - Scenario - Map Description - Description

Tileset - Advanced - Modify Tileset / File - New Map / Module - Object Manager - Tileset

Recommended Number of Players - Scenario - Map Description - Suggested Players

Maximum Number of Players - Scenario - Player Properties

Map Size (Playable)(Camera Bounds) - Scenario - Map Size and Camera Bounds - Playable / Module - Object Manager - Playable Map Size

Map Size (Word) - Scenario - Map Size and Camera Bounds - Description / Module - Object Manager - Map Size Description

Map Size (Full)(Number) - Scenario - Map Size and Camera Bounds - Full / File - New Map

Image of the waiting lobby room (poster) - Scenario - Map Loading Screen - Use Default Screen / Module - Import Manager

Image of the loading screen (poster) - Scenario - Map Loading Screen - Use Default Screen / Module - Import Manager

Image of the loading screen (full screen) - Scenario - Map Loading Screen - Use Imported File / Module - Import Manager

Image of the minimap - Scenario - Map Loading Screen - Use Default Screen / Module - Import Manager

Tileset:

HEX 41 (DEC 65) - A - Ashenvale

HEX 42 (DEC 66) - B - Barrens

HEX 43 (DEC 67) - C - Felwood

HEX 44 (DEC 68) - D - Dungeon

HEX 46 (DEC 70) - F - Lordaeron Fall

HEX 47 (DEC 71) - G - Underground

HEX 49 (DEC 73) - I - Icecrown

HEX 4A (DEC 74) - J - Dalaran Ruins

HEX 4B (DEC 75) - K - Black Citadel

HEX 4C (DEC 76) - L - Lordaeron Summer

HEX 4E (DEC 78) - N - Northrend

HEX 4F (DEC 79) - O - Outland

HEX 51 (DEC 81) - Q - Village Fall

HEX 56 (DEC 86) - V - Village

HEX 57 (DEC 87) - W - Lordaeron Winter

HEX 58 (DEC 88) - X - Dalaran

HEX 59 (DEC 89) - Y - Cityscape

HEX 5A (DEC 90) - Z - Sunken Ruins

Size Matching Table:

The size of the map depends on the size of the Playable Area, this area is obtained by multiplying the dimensions width and height

1.00-1.28:

Tiny - from 45 to 7500

Small - from 7501 to 13500

Medium - from 13501 to 22000

Large - from 22001 to 32500

Huge (TFT) - from 32501 to 45000

Epic (TFT) - from 45001 to 62997

Starting from 1.29.0.9055:

Tiny - from 45 to 6600

Extra Small - from 6601 to 12800

Small - from 12801 to 21000

Medium - from 21001 to 31000

Large - from 31001 to 43500

Extra Large - from 43501 to 74000

Huge - from 74001 to 135000

Epic - from 135001 to 215000

Legendary - from 215001 to 230400

Map Size (Full):

1.00-1.28:

32x32 - Tiny

64x64 - Tiny

96x96 - Tiny

128x128 - Small

160x160 - Medium

192x192 - Large

224x224 - Huge (TFT)

256x256 - Epic (TFT)

Starting from 1.29.0.9055:

32x32 - Tiny

64x64 - Tiny

96x96 - Extra Small

128x128 - Small

160x160 - Medium

192x192 - Large

224x224 - Extra Large

256x256 - Extra Large

288x288 - Huge

320x320 - Huge

352x352 - Huge

384x384 - Epic

416x416 - Epic

448x448 - Epic

480x480 - Legendary

Additional Information:

An effective way to find out where values are located - is to manually change the map and compare files before and after this change.

war3map.j is located in the root of the map or in scripts\war3map.j

Through parsing, you can see the add-on of the map (ROC or TFT) if you have a file without the w3m/w3x extension and you need to find out which extension to assign to it.

You can see through which World Editor the map was made and on which version of Warcraft it is best to play it (on other versions a different balance or map may not start). For example, the campaign "WarCraft 3: Rise Of Krill" runs on version ROC 1.04

The maximum length of a map name can be 44 characters + 4 characters ".w3x" = 48 = (4)LostTemple1111111111111111111111111111111.w3x

If 45 characters are in the name, then the map is not visible in the list of maps in the game

For some maps, this value is not 44 but 43

The length of the subdirectory and slashes is added to the name of the map.

(4)LostTemple1111111111111111111111111111111.w3x - 48

11111\99999\(4)LostTemple1111111111111111111.w3x - 48

folder1\folder2\(4)LostTemple111111111111111.w3x - 48

In w3m w3x campaigns, the transition to the next chapter is in the war3map.j function SetNextLevelBJ (these functions can be several in one file)

Search the contents of mpq w3m w3x w3n archives:

Total Commander + MPQ plugin (download separately)

Install plugin MPQ plugin (Copy to folder \Total commander\Plugins\wcx\MPQ\ )

Configuration - Options - Packer - Configure packer extension WCXs - Write in the upper right corner "mpq" - New type - Choose the path to the plugin "MPQ plugin" - OK

Search (Alt + F7) - Search archives (all except for UC2) - check

----------------- W3N --------------------

Below is an example of the value addresses for the campaign. - \TFT en\Campaigns\DemoCampaign.w3n (campaign is attached)

● World Editor version - war3campaign.w3f(HEX) - Offset 08 - Value HEX 178E (DEC 6030)

● Amount of campaign saves - war3campaign.w3f(HEX) - Offset 04 - Value HEX 9E (DEC 158)

● Name - war3campaign.w3f(HEX)+war3campaign.wts(UTF65001) - Offset 0C - Value TRIGSTR_001 - STRING 1{Exodus of the Horde}

● Difficulty - war3campaign.w3f(HEX)+war3campaign.wts(UTF65001) - Offset 18 - Value TRIGSTR_002 - STRING 2{Normal}

● Author - war3campaign.w3f(HEX)+war3campaign.wts(UTF65001) - Offset 24 - Value TRIGSTR_004 - STRING 4{Blizzard Entertainment}

● Description - war3campaign.w3f(HEX)+war3campaign.wts(UTF65001) - Offset 30 - Value TRIGSTR_003 - STRING 3{This is the Prologue Campaign...}

● Number of chapters - war3campaign.w3f(HEX)+war3campaign.wts(UTF65001) - Offset 8E - Value HEX 03 (DEC 03)

● Image of the waiting lobby room (poster) - OrcSymbol.tga

World Editor:

World Editor version - Module - Campaign Editor - Saved with Editor Version / Help - About Warcraft III World Editor

Amount of campaign saves - Module - Campaign Editor - Campaign Version

Name - Module - Campaign Editor - Name

Difficulty - Module - Campaign Editor - Difficulty

Author - Module - Campaign Editor - Author

Description - Module - Campaign Editor - Description

Number of chapters - Module - Campaign Editor - Map Files / Module - Campaign Editor - Loading Screen

Image of the waiting lobby room (poster) - Module - Campaign Editor - Image (Minimap)

Related Links:

this article in Russian Парсинг метаданных из w3m/w3x/w3n - Статьи - WarCraft 3 / Модмейкинг - XGM: eXtreme Gamedev & Modmaking

this article in English Parsing metadata from w3m w3x w3n

mpq MPQ (file format) - Wikipedia

w3m w3x (ru) Формат карт Warcraft 3 - Статьи - WarCraft 3 / Модмейкинг - XGM: eXtreme Gamedev & Modmaking

w3m w3x (ru) InsideTheW3M Формат файлов W3M и W3X - Статьи - WarCraft 3 / Модмейкинг - XGM: eXtreme Gamedev & Modmaking

w3m w3x InsideTheW3M W3M and W3X Files Format

w3m w3x Guide - Explanation of W3M and W3X Files

w3m w3x W3M and W3X Files Format - Статьи - WarCraft 3 / Модмейкинг - XGM: eXtreme Gamedev & Modmaking

w3m w3x http://www.wc3c.net/tools/specs/index.html

w3n Campaign Basics and Creating a Campaign

World Editor (ru) Самоучитель по редактору WorldEditor - Статьи - WarCraft 3 / Модмейкинг - XGM: eXtreme Gamedev & Modmaking

World Editor (ru) Создание карты и параметры - Статьи - WarCraft 3 / Модмейкинг - XGM: eXtreme Gamedev & Modmaking

filelist MPQ Archivy - Name breaking

protection MPQ Archives - Warcraft III Maps

parse uakfdotb/ghostpp

w3g http://w3g.deepnode.de/files/w3g_format.txt

LoadingScreen.mdx Custom Loading Screen Tutorial V1.5

LoadingScreen.mdx (ru) [Пособие] Экран загрузки 1 файлом + превьюшка - Wc3-Maps Форум

LoadingScreen.mdx (ru) Загрузочный экран HD (1080) - Статьи - WarCraft 3 / Модмейкинг - XGM: eXtreme Gamedev & Modmaking

LoadingScreen (ru) Загрузочные экраны. - Статьи - WarCraft 3 / Модмейкинг - XGM: eXtreme Gamedev & Modmaking

online maps dataminer WC3 Data + d07RiV/wc3data

WinMPQ Batcher 1.0 thread WinMPQ Batcher 1.0 - w3x unpacker

WinMPQ Batcher 1.0 file WinMPQ Batcher 1.0 - w3x unpacker

pw3i Batcher 1.0 thread pw3i Batcher 1.0 - w3x parser

pw3i Batcher 1.0 file pw3i Batcher 1.0 - w3x parser

Changelog:

1.01 - added HEX and DEC values to the "Tileset" list

#### Attachments

TFT 1.07 en-Campaigns-DemoCampaign.w3n

 2.5 MB

 · Views: 210



TFT 1.26a en-(2)EchoIsles.w3x

 114.3 KB

 · Views: 240



wc3 file-format 8 10 11 15 23 24 26 27.zip

 13.2 MB

 · Views: 271



00 Info_en 1.01.png

 5.5 MB

 · Views: 315




 Last edited: Feb 3, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Although this tutorial has a lot of information, I am unsure what exactly it is supposed to teach.

You can pickup on bits and pieces of useful stuff but as I understand it you cannot just read this and go and do something with it.

To me it seems that a lot of the tutorial is just handing out hex values and assume you know what to do with them. This might indeed be useful to a select few, but calling it a tutorial seem misleading in my mind.

I will say however, that this is not an area that I am very knowledgeable.

So I might be wrong, and in which case perhaps convinced that it is considered a tutorial.

In that case, I would still say this needs some heavy restructuring.

The images need to be put in hidden tags and/or reduced in size.

I would also say a lot of the lists could be put in hidden tags.

The titles should probably be increased in size for better readability.

I think there is a lot one COULD salvage from this but it's not easy to navigate, very discouraging to see a wall of text.

Regardless, as I said it might be worth approval but needs updating and since the author has been gone for half a year at this point I am graveyarding it.

Poke a staff member if you intend to update this and they'll put it back.

#### Drake53

#### Drake53

 Level 20

JoinedJan 1, 2018Messages777

To be honest, all this stuff about hex values only seems useful to me if you're writing a parser for the war3map file formats, but then you start writing about using a hex editor and I just think to myself, people still use hex editors in 2020? I mean no offense, but I honestly believe learning a programming language and using an existing open source parser (for example here's mine) is a lot easier than trying to extract the information yourself using a hex editor.

Firstrun70 said:

Map type (Melee - two swords / Custom Scenario - yellow gear) - war3map.j(UTF65001) - ForcePlayerStartLocation is missing = Melee


Click to expand...

A more reliable way to check this is by parsing war3map.w3i and check if the MapFlags contains the melee map flag (0x0004).

I did learn some new things from this tutorial though. I was only aware of version 18, 25, 28, and 31 for the .w3i file, but it appears there are a lot more, and you also have example maps for each. Thanks for that.

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

The Mega link for old rare maps is already dead. I've downloaded the map archive, if all links go down please tell me to reupload.

PS: Excellent work.

#### Firstrun70

#### Firstrun70

 Level 5

JoinedFeb 2, 2020Messages12

Resources: 4

#### Resources
Tools 2Tutorials 2

Luashine said:

 The Mega link for old rare maps is already dead.


Click to expand...

###
 File on MEGA


 mega.nz



 Last edited: Aug 5, 2022

 You must log in or register to reply here.


### Similar threads

 WarCraft Files (w3m, w3x, mpq..)


Ralle

Jan 3, 2006

Tutorial Archive

Replies1Views27K

Mar 26, 2007
Wolverabid

Locked

 WinMPQ Batcher 1.0 - w3x unpacker


Firstrun70

Feb 2, 2020

Warcraft Editing Tools

Replies0Views3K

Feb 2, 2020
Firstrun70

Locked

 pw3i Batcher 1.0 - w3x parser


Firstrun70

Feb 2, 2020

Warcraft Editing Tools

Replies3Views3K

Feb 3, 2020
Cheezeman

 Extracting game files (MPQ and CASC)


BogdanW3

Feb 25, 2021

Tutorial Archive

Replies5Views23K

Apr 15, 2021
pyf

Locked

 List of official maps for Warcraft 3


Firstrun70

Feb 4, 2020

Warcraft Discussion

Replies0Views21K

Feb 4, 2020
Firstrun70

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