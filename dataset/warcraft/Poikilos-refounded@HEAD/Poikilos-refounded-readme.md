---
type: vendor-doc
source_repo: "https://github.com/Poikilos/refounded"
source_path: "readme.md"
ref: HEAD
---

# refounded

Be aware that using your "maps" (full games) on Warcraft 3: Reforged
transfers your rights over them to Blizzard as per their ToS!
Therefore, lets not run them there but instead make code to get your
w3m/w3x games running on other engines.

This project has been in the planning stage for a long time, but may
become an engine that runs JASS2, a language that is usually called
JASS (which is actually the name of the first version of the language).

See the "JASS projects" sections below for more information.

JASS2 could be considered a substandard and deprecated language:
- https://sc2mapster.gamepedia.com/Language_Overview
- https://github.com/fullmetal-a/cjass2lua

However, many full games are written in JASS2, though they are
unfortunately called "maps" due to the way they were used in
Warcraft III's online "Custom Maps" system. Some full games include:
- DoTA "Defense of the Ancients" and many DoTA clones and other tower
  offense games.
  - DoTA led to DoTA 2 and gameplay seems similar to early versions of League of Legends.
- Hardcore/survival games (games with detailed gameplay and character
  and world attributes) such as:
  - [Bug Evolution](https://www.epicwar.com/maps/4043/)
  - [Ice Troll Tribe Wars](https://www.epicwar.com/maps/19469/)
  - Real Life (and easier versions such as [Real Life [for Morons]](https://www.epicwar.com/maps/1252/)
  - [Spellcraft](https://www.epicwar.com/maps/487/)
- Hero arena games such as:
  - [Angel Arena X](https://www.epicwar.com/maps/8346/)
  - [MonstAr.Arena](https://www.epicwar.com/maps/139667/)
- OpenRPG and other large RPGs such as:
  - [Fantasy Life](https://www.epicwar.com/maps/91725/)
  - [Mystica ORPG](https://www.epicwar.com/maps/7577/)
  - [Legends Open RPG](https://www.epicwar.com/maps/130439/)
  - [Ragnarok RPG](https://www.epicwar.com/maps/80733/)
- Single-goal "party games" such as:
  - Assassins
  - [Pyramid Escape](https://www.epicwar.com/maps/165011/) (and other
    "Escape" games)
  - [Kodo Tag](https://www.epicwar.com/maps/103272/)
  - [Missile War](https://www.epicwar.com/maps/58603/)
  - Wolves vs Sheep (and other "vs Sheep" games)
  - [Vampirism](https://www.epicwar.com/maps/51266/)
- Tower Defense (and various variations, which seem to have spawned or
  popularized the tower defense genre itself).
- Many clone games, and others:
  [Maps on Epic War sorted by
  rating](https://www.epicwar.com/maps/?order=desc&sort=rating&page=1)


## Related projects
- [war3structs](https://github.com/sides/war3structs) "Python construct definitions for Warcraft III file formats"
  - Includes a Lark-based parses for JASS: [war3structs/plaintext/jass.py](https://github.com/sides/war3structs/blob/master/war3structs/plaintext/jass.py)
- jass2 (MIT) "Jass2 interpreter for exercise purpose"
- w3xparser (Apache-2.0)
  - https://github.com/actboy168/w3xparser
- MPQExtractor (MIT) https://github.com/Kanma/MPQExtractor
- pympq (BSD 2-Clause) https://github.com/eagleflo/mpyq
- python-mpq (MIT) https://github.com/jleclanche/python-mpq
  "StormLib-based Python MPQ bindings"
  - requires StormLib (MIT) https://github.com/ladislav-zezula/StormLib
- BananaMPQ (MIT) https://github.com/sgraf812/BananaMpq
  "Extracting WoW's geometry data reliably"
- https://github.com/chadkila/Blizzard-Hash-Map

### Node.js
- mpq-extract (MIT) https://github.com/doggan/mpq-extract
  - pulls mech-mpq
    - requires the C++ version of StormLib
- ghost.js https://github.com/w3gh/ghost.js
  "Try to port Warcraft3 Game Host Ghost++ to js"
- Wc3MapWorker (MIT) "Wc3MapWorker is a npm package written in
  TypeScript to enable you to export and import your wc3 map files data
  easily via ts/js. Used only for the MPQ extraction and creation! If
  you are looking to extract the content of the extracted files into
  jason format use WC3MapTranslator instead."
  - TypeScript
  - Resolved license issue:
    https://github.com/rufreakde/Wc3MapWorker/issues/1
- WC3MapTranslator (MIT) https://github.com/ChiefOfGxBxL/WC3MapTranslator
  "WC3MapTranslator is a module to convert a JSON representation of
  WarCraft III (.w3x) data to their war3map files, and vice-versa."
  - Node.js
  - TypeScript
  - License issue:
    https://github.com/ChiefOfGxBxL/WC3MapTranslator/issues/44


### Rust
- ceres-mpq (MIT) https://github.com/ceres-wc3/ceres-mpq
  "A pure-rust implementation of an MPQ archive reader and builder"


## JASS projects with license issues
(These projects have licenses that differ from MIT, so changing
this project to match a different license may be the best way forward.)

### C++
- JASS Tools (sourceforge project)
  https://sourceforge.net/projects/jass/ is a project that contains some
  "tools for parsing and analyzing the JASS scripting language, which
  used for scripting Maps and AI files in Blizzard Entertainment's (R)
  Warcraft III (R)," notably:
  - pjass (BSD 2-Clause)
    https://sourceforge.net/projects/jass/files/a%20pjass/
    "currently the best JASS syntax checker" according to
    http://jass.sourceforge.net/ (October 26, 2005).
    "To test this program:
    `pjass common.j common.ai Blizzard.j`" And yet, pjass is also
    below.
- pjass (BSD 2-Clause) https://github.com/lep/pjass
  "A lightweight and fast Jass2 parser for bison/yacc"
  - This could be ported to Python and use PLY (Python Lex-Yacc).
  - translations: pjass-chs
    - [pjass-chs is a Chinese translation, but otherwise is the same
      as pjass,
      apparently.](https://github.com/actboy168/pjass-chs/issues/1)

### PHP
- PHP-MPQ (GPL-3.0) https://github.com/cipherxof/PHP-MPQ
- php-mpq (BSD 2-Clause) https://github.com/Rogiel/php-mpq
  "A PHP library for MPQ reading"
  - PHP
  - Resolved license issue:
    https://github.com/Rogiel/php-mpq/issues/1

### JASS projects with incompatible functions
(These projects may be too far from the design goals to become
dependencies.)
- cjass2lua
  - This translator does not allow inheritance, classes, structures,
    scopes, modules, preprocessor directives, cJass `for` loops, or
    macros ("Macroses are translated into global variables" [sic]).
    Also note that Lua array indices start at 1.
  - https://github.com/fullmetal-a/cjass2lua
- c-jasshelper (ZLib/PNG license - OSI approved)
  https://sourceforge.net/projects/c-jasshelper/
  "A proof of concept vJass compiler, vJass is an extension to the Jass
  syntax enabling a bunch of crazy things like libraries, structs and
  textmacros. Also adds the zinc scripting language."
  Compile vJass, a meta language (or something) for JASS.
- JASSEdit (GPLv2) https://sourceforge.net/projects/jassedit/
  "JASSEdit is an editor for the WarCraft 3 WorldEdit Scripting Language
  JASS" with code completion (with function documentation), syntax
  highlighting, multiple script loading, and syntax checking.

## JASS projects with no license
- Blizzard-Hash-Map https://github.com/chadkila/Blizzard-Hash-Map/issues/1
- war3mappreviewextractor-php https://github.com/an-zie/war3mappreviewextractor-php/issues/1
  - PHP
- mpqtools https://github.com/limitedWish/mpqtools/issues/1
  "Inflate .mpq archives and convert .blp textures to ordinary format"
  - C

## JASS projects with unknown purposes
- LangUMS fork of stormlib https://github.com/LangUMS/stormlib/issues/1
- https://github.com/jleclanche/mpqt/tree/master/mpqt
  - Python
  - archived
