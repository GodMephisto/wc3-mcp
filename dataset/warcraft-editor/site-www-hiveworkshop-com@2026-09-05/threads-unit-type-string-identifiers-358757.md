---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/unit-type-string-identifiers.358757/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Unit Type String Identifiers
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/unit-type-string-identifiers.358757/>
#### Yevhen Snizhko

#### Yevhen Snizhko

 Level 12

JoinedMay 13, 2023Messages157

Resources: 4

#### Resources
Models 1Maps 2Tutorials 1

Sometimes a user may need to use a string identifier for creation, replacement, or other interactions with unit types.

For example, this is good for preloading custom unit types for your map, or testing their interactions. But making one trigger per unit type (of which there are more than 300) is not really effective.

Or you want user to input unit type, but don't want to write a fancy interface for it.

The solution: use string identifier.

t1
Events
Player - Player 1 (Red) types a chat message containing -create as A substring

Conditions

Actions
Set VariableSet str_main = (Substring((Entered chat string), 9, (Length of (Entered chat string))))

Unit - Create 1 (Unit-type(str_main)) for Player 1 (Red) at (Target of current camera view) facing Default building facing degrees

(the website does not show that there is an extra space in '-create ')

In this example, we use conversion of string to unit type.

The substring function at the start of the trigger takes a string, goes set number of symbols from start (dash (-) is symbol 1, empty space ( ) is symbol 8 in this case), and takes everything up to the stated symbol position (provided it is not earlier than the starting symbol). In this case, we just read until the end.

But what can we input so that the game knows what to create?

Let's start with custom units.

All custom units use 4-symbol identifiers.

When you create a custom unit, you will need to write (or just accept) a 4-symbol alpha-numeric code - meaning it can contain letters a-z (no special characters) and numbers 0-9, exactly four total.

In Object Editor in raw view, you'll see that a custom unit has 9 symbols before their name, for example:

n000:hpea (Custom Peasant name)

The first four symbols are identifier of this unit type, which are the part that interests us.

These identifiers have to be prefixed by 'custom_'.

So, to activate the trigger we checked out earlier and create an 'Archmage sd+', we need to input in chat:

-create custom_h001

The identifiers are not case-sensitive, so having 'h' instead of 'H' is not an issue.

What will be an issue is having two different unit types with same identifiers - then the type added to the map earlier will be created. The editor does won't block you from doing this, so you should be careful.

Using string identifiers for custom units is easy and is useful if you don't want to constantly look for your custom unit among hundreds of them when working with GUI.

Let's go to standard units.

Standard units never use their 4-symbol codes as identifiers.

Most standard units use their names as identifiers.

For example, Arthas uses string 'Arthas'.

Creating him with our trigger will require input:

-create arthas

If a unit has a space, dash sign (-) or apostrophe (') in their name, they have to be omitted from the identifier.

Examples:

Mortar Team - mortarteam

Mal'Ganis - malganis

Earth-Fury Tower - earthfurytower

Summoned units and units with several variations almost always use numbers at the end of their identifiers - see below.

Some units from TFT or added for Reforged use identifiers based on old units, so they can't be used by their names. They will never get created if you are using their identifier, and they will not be discussed here.

Some units that usually have timed life may have such timer when created in this way. For now I've only seen Militia do this constantly.

Ancients only use one identifier for rooted and uprooted versions, and will always create a rooted version.

Melee heroes use their type names (Paladin, Blood Mage, Crypt Lord, Priestess of the Moon, etc.) with removed spaces.

Most campaign heroes use their names as seen in the game (Muradin Bronzebeard, Jaina, Arthas, Cairne Bloodhoof) as their identifier - with the usual rules.

Here's a list of standard units that use identifiers that are not similar to their names, don't follow the rules above or have important changes (like word order changed or map-specific alterations) as of Reforged 2.0.1 (SPOILER ALERT FOR SOME CHARACTERS!!!):

Unit name (with editor suffix)String Identifier (not case-sensitive, capital letters indicate what is unusual compared to how the unit is seen in-game)Additional comments

Kobold ShovelertheshovelerNot available through UI. Very strong melee unit.

Sammy!sammycubeNot available through UI. Very strong ranged unit.

Abomination (cinematic)abominationCINEMATICAn unmodified abomination for Human3 boss fight.

Admiral ProudmooreadmiralproudmooreThis is a core version without any abilities. The Rexxar's campaign boss uses same type with map-specific changes.

Advanced Boulder TowerBOULDERtowerUPGRADEThese towers have some changes between campaign levels.

Advanced Cold TowerCOLDtowerUPGRADEThese towers have some changes between campaign levels.

Advanced Death TowerDEATHtowerUPGRADEThese towers have some changes between campaign levels.

Advanced Energy TowerENERGYtowerUPGRADEThese towers have some changes between campaign levels.

Advanced Flame TowerFLAMEtowerUPGRADEThese towers have some changes between campaign levels.

Alchemist (morph level 1)alchemistMORPHThis unit is a morph form for Chemical Rage ability of corresponding level.

Alchemist (morph level 2)alchemistMORPH2This unit is a morph form for Chemical Rage ability of corresponding level.

Alchemist (morph level 3)alchemistMORPH3This unit is a morph form for Chemical Rage ability of corresponding level.

AntonidasantonidUsThe archmage boss from Undead07. Definitely not a Nidus Worm.

Antonidas (Ghost)GHOSTLYarchmageThe archmage miniboss template from Humanx03.

Arachnathid Earth-BorerEARTHBORERarachnathid

Arcane TowerHUMANarcanetowerHuman melee tower from TFT.

ArcherHIGHELVENarcherThe same archer is used in Silvermoon missions and in TFT Blood Elf campaign. Night Elven Archer is just archer.

Arthas (Evil)EVILarthasArthas from Undead campaigns.

Arthas (with Frostmourne)arthasWITHFROSTMOURNEThe unit used for final section of Human09.

Avatar of VengeanceSPIRITofvengeanceThe summoned unit from Warden's ultimate ability.

AzgalorOLDpitlordPit Lord from NightElf07.

BarracksHIGHELFbarracksThe Barracks used for Silvermoon missions.

BarracksHUMANbarracksThe melee Barracks for Human faction.

BarracksORCbarracksThe melee Barracks for Orc faction.

BatriderTROLLbatriderThe expansion flying unit for Orc faction.

Bear (level 1)GRIZZLYbear1Summoned unit for Beastmaster's ability.

Bear (level 2)GRIZZLYbear2Summoned unit for Beastmaster's ability.

Bear (level 3)GRIZZLYbear3Summoned unit for Beastmaster's ability.

Berserk Quilbeastquillbeast4Summoned unit for Rexxar's fully upgraded ability.

Berserk WildkinberserkOWLBEARIdentifier name is based on original name.

Black CitadelNECROPOLIS2The only town hall that has wrong identifiers for tier 3.

Black SpiderSPIDERblack

Blademaster of the Blackrock ClanBLACKROCKblademasterThe red Blademaster from Undead06.

Blue DragonAZUREdragonInterestingly, the Blue Dragon Roost uses a proper identifier.

Blue Dragon WhelpAZUREdragonwhelpOnly proper Blue Dragons are called Azure.

Blue Dragonspawn SorcererbluedragonspawnsorcerOrA lot of units with 'Sorcerer' in their name use 'sorcerOr' in identifier.

Blue DrakeAZUREdrakeOnly proper Blue Dragons are called Azure.

Book of Summoning PedestalbookofSUMMONINGPoor Pedestal. Nobody remembers him.

BrewmasterPANDARENbrewmasterPromotional materials definitely call him Pandaren Brewmaster.

BurrowTROLLburrowThe food supply building for Orc faction.

Burrowed Barbed ArachnathidbarbedarachnathidBURROWED

Burrowed Carrion Beetle (level 2)carrionSCARABBURROWED2A morphed version of a summoned unit from Crypt Lord ability

Burrowed Carrion Beetle (level 3)carrionSCARABBURROWED3A morphed version of a summoned unit from Crypt Lord ability

Burrowed Crypt FiendcryptfiendMORPH

Cairne (cinematic)cairnebloodhoofCINEMATICUnmodified RoC Cairne from TFT cinematics

Cairne (expansion)cairnebloodhoofEXPTFT Cairne with Attribute Bonus learnable ability

CaptainTHEcaptainThe one who is either Falric, Marwyn or Luke Valonforth.

Carrion Beetle (level 1)carrionSCARAB1A summoned unit from Crypt Lord ability

Carrion Beetle (level 2)carrionSCARAB2A summoned unit from Crypt Lord ability

Carrion Beetle (level 3)carrionSCARAB3A summoned unit from Crypt Lord ability

Centaur SorcerercentaursorcerOrA lot of units with 'Sorcerer' in their name use 'sorcerOr' in identifier.

Centaur TentcentaurtentUnits with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Centaur Tent 2centaurtent2

Centaur Tent 3centaurtent3

Chen StormstoutSINJOstormstoutBrewmaster from Rexxar's campaign

ChildVILLAGERKIDUnits with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Child 2VILLAGERKID2This is their basic function, after all.

Circle of Power (large)circleofpowerLARGESize matters.

Circle of Power (medium)circleofpowerMEDIUMSize matters.

Circle of Power (small)circleofpowerSome are embarrassed by their size. They shouldn't be.

City Building 0citybuildingSMALL02-story straight building, like ones seen in Human06.

City Building 1citybuildingSMALL12-story straight building, like ones seen in Human06.

City Building 2citybuildingSMALL22-story straight building, like ones seen in Human06.

City Building 3citybuildingSMALL32-story straight building, like ones seen in Human06.

City Building 4citybuildingSMALL42-story straight building, like ones seen in Human06.

City Building 5citybuildingSMALL52-story straight building, like ones seen in Human06.

City Building 6citybuildingSMALL62-story straight building, like ones seen in Human06.

City Building 7citybuildingSMALL72-story straight building, like ones seen in Human06.

City Building 8citybuildingSMALL82-story straight building, like ones seen in Human06.

City Building 9citybuildingSMALL92-story straight building, like ones seen in Human06.

City Building 10citybuildingSMALLA2-story straight building, like ones seen in Human06. Has a letter as an index.

City Building 11citybuildingSMALLB2-story straight building, like ones seen in Human06. Has a letter as an index.

City Building 12citybuildingLARGE02-story angular building, like one from the sheep-in-a-box secret from Human05.

City Building 13citybuildingLARGE12-story angular building, like one from the sheep-in-a-box secret from Human05.

City Building 14citybuildingLARGE22-story angular building, like one from the sheep-in-a-box secret from Human05.

City Building 15citybuildingLARGE32-story angular building, like one from the sheep-in-a-box secret from Human05.

Clockwerk Goblin (level 1)clockwerkgoblin1An exploding summoned unit, produced by Pocket Factory from Tinker's ability.

Clockwerk Goblin (level 2)clockwerkgoblin2An exploding summoned unit, produced by Pocket Factory from Tinker's ability.

Clockwerk Goblin (level 3)clockwerkgoblin3An exploding summoned unit, produced by Pocket Factory from Tinker's ability.

Clockwerk Goblin (level 4)clockwerkgoblin4Unused. It is same as the level 3 clockwerk goblin.

Corrupted TreantcorruptedENTTreants tend to be identified as Ents.

CouatlnagaCOUTLThe 'a' is missing.

Dagren the OrcslayerDAGRENOne of paladins from Undeadx01.

DalvengyrVENGYRDreadlord from Humanx02.

Dark Miniondarkminion2A summoned unit from Dark Ranger's Black Arrow ability.

Demon Hunter (Demon Form)demonhunterMORPHEDCreated through Metamorphosis ability.

DestroyerOBSIDIANdestroyerMorph form of Obsidian Statue.

Dimensional Gate (facing South-East)DARKPORTALseDemonic portal from Humanx05.

Dimensional Gate (facing South-West)DARKPORTALswDemonic portal from Humanx05.

Dire Frost WolfWHITEdirewolf

Dire Frost WolfWHITEdirewolf

Dire QuilbeastquilLbeast2A summoned unit from Beastmaster's ability. Has an extra 'L'.

Dire WolfSPIRITwolf2A summoned unit from Far Seer's ability.

Disease CloudPLAGUEWARDThis is left by Meat Wagon's upgraded attacks.

Doom Guard (summoned)doomguardsummonedStats of summoned units tend to be identical to those of usual units.

Draenei BarracksdraeneiWARRIORSHALLUsed in Humanx05 by Draenei village.

Draenei Chieftain's HutCHIEFTAINhutUsed in Humanx05 by Draenei village. Missing an 's'.

Draenei DemolisherDEMOLISHERdraeneiUsed in Humanx06.

Draenei HutDRANEIhut0Missing an 'e'.

Draenei Hut 2DRANEIhut1Missing an 'e'.

Dragon HawkWINDSERPENTFlying unit from Silvermoon missions. Identifier name based on original name for this unit.

Dragon RoostdragonBUILDINGBuilding from Undead06 that simulates WC2 Dragon Roost.

Dragon TurtleCAMPAIGNturtleNaga siege unit.

Dragonhawk RiderdragonhawkThe expansion air unit for Human facction.

Druid of the Claw (Bear form)druidoftheclawMORPHED

Druid of the Talon (Raven form)druidofthetalonMORPHED

Earth (level 1)PANDA3One of three units created by Brewmaster's ultimate.

Earth (level 2)PANDA6One of three units created by Chen's upgraded ultimate.

Enchanted GemstonegemstoneOBELISKUsed in Orc07, activated by Cairne.

Enraged WildkinenragedOWLBEARIdentifier name is based on original name.

Eredar SorcerereredarsorcerOrA lot of units with 'Sorcerer' in their name use 'sorcerOr' in identifier.

Fel BeastpurplefelstalkerLogical way to describe them.

Fel BurrowCHAOSORCburrowChaos form of Burrow from Orc faction. Note that it is not Troll anymore. Since in RoC they were called 'Chaos' and not 'Fel', it uses this identifier.

Fel GruntCHAOSgruntChaos form of Grunt from Orc faction. Since in RoC they were called 'Chaos' and not 'Fel', it uses this identifier.

Fel Kodo BeastCHAOSkoTobeastChaos form of Kodo Beast from Orc faction. Since in RoC they were called 'Chaos' and not 'Fel', it uses this identifier. Kodo Beasts are often called 'Koto' in game data.

Fel PeonCHAOSpeonChaos form of Peon from Orc faction. Since in RoC they were called 'Chaos' and not 'Fel', it uses this identifier.

Fel RaiderCHAOSWOLFRIDERChaos form of Raider from Orc faction. Since in RoC they were called 'Chaos' and not 'Fel', it uses this identifier.

Fel WarlockCHAOSwarlockChaos form of Warlock. In Orc06 it is used as an upgrade for Shamans. Since in RoC they were called 'Chaos' and not 'Fel', it uses this identifier.

Fire (level 1)PANDA1One of three units created by Brewmaster's ultimate.

Fire (level 2)PANDA4One of three units created by Chen's upgraded ultimate.

Earth (level 1)PANDA3One of three units created by Brewmaster's ultimate.

Forest Troll Hutforesttrollhut0Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Forest Troll Hut 2foresttrollhut1Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Fountain of BloodBLOODfountainUsed for Orc06.

Fountain of PowerPURPLEfountainDo not confuse With Fountain of Mana.

Frost WolfWHITEwolf

Frostmourne PedestalFROSTMOURNEUsed in Human09.

Furbolg Hutfurbolghut1Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Furbolg Hut 2furbolghut2Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Furbolg Ursa WarriorfurbolgANCIENTNo, it is not a tree.

FurionfurionRoC version of Malfurion Stormrage. Since Reforged the old model is missing and this unit is called Malfurion.

GhostkeeperofthegroveGHOSTA basic version of Ancient Protectors from NightElf03, used as a template.

Giant Frost WolfgiantWHITEwolf

Giant Skeleton WarriorgiantSKELETON

Gnollgnoll1Some creeps have numbers instead of professions. Some even have numbers despite being plain simple creeps.

Gnoll HutgnollhutUnits with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Gnoll Hut 2gnollhut2Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Gnoll OverseergnollKING

Gnoll Wardengnoll3Some creeps have numbers instead of professions.

Greater Dark MinionDARKminion3A summoned unit from Dark Ranger's Black Arrow ability. Doesn't put his size in front.

Greater Spirit BeastSPIRITbeast3

Grom HellscreamgromhellscreamThere is only one person who tried to call him Grommash, and that person is dead.

Grom Hellscream (possessed)POSSESSEDgromhellscreamChaos form of Grom Hellscream. There is only one person who tried to call him Grommash, and that person is dead.

Halahk the LifebringerHALAHKPaladin from Undeadx01.

Halls of the DeadNECROPOLIS1The only town hall that has wrong identifiers for tier 2.

Harpy Storm-HagharpyHAGThey aren't supposed to know what type of hag you are until it is too late.

Harpy Storm-HagharpyHAGThey aren't supposed to know what type of hag you are until it is too late.

Harpy WindwitchharpyWITCHThey aren't supposed to know what type of witch you are until it is too late.

Haunted Gold MineUNDEADgoldmine2Undead really add their flavor to everything.

HawkWAREAGLE1Summoned unit from Beastmaster's ability.

High Elf (female)highELVENFEMALEVillager from Silvermoon missions.

High Elf (male)highELVENMALEVillager from Silvermoon missions.

High Elven FarmhighELFfarmFood-providing building from Silvermoon missions. Has team color.

High Elven Farm 1highELFfarm0Neutral building from Silvermoon missions. Doesn't have team color. Index and suffix don't match.

High Elven Farm 2highELFfarm1Neutral building from Silvermoon missions. Doesn't have team color. Index and suffix don't match.

High Elven Farm 3highELFfarm2Neutral building from Silvermoon missions. Doesn't have team color. Index and suffix don't match.

High Elven Farm 4highELFfarm3Neutral building from Silvermoon missions. Doesn't have team color. Index and suffix don't match.

High Elven Farm 5highELFfarm4Neutral building from Silvermoon missions. Doesn't have team color. Index and suffix don't match.

High Elven Farm 6highELFfarm5Neutral building from Silvermoon missions. Doesn't have team color. Index and suffix don't match.

High Elven Farm 7highELFfarm6Neutral building from Silvermoon missions. Doesn't have team color. Index and suffix don't match.

High Elven Farm 8highELFfarm7Neutral building from Silvermoon missions. Doesn't have team color. Index and suffix don't match.

High Elven Guard TowerELVENguardtowerTower from Silvermoon missions.

Hippogryph RiderRIDDENhippogryph

Horn of Cenarius PedestalhornofCENARIUSPoor Pedestal. Nobody remembers him.

Human FrigatehumanDESTROYERAll Frigates are called 'Destroyer'. Probably based on Destroyers from WC2.

Ice Troll Huticetrollhut0Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Ice Troll Hut 2icetrollhut1Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions. Index and suffix don't match.

Ice Troll PriesticetrollSHADOWpriestIce Trolls don't want other people to know that their priests are aligned with Shadow. Other trolls don't mind.

Icy Treasure BoxTREASUREboxUnused.

Illidan (demon form)illidanDEMONFORMUsed for NightElf06 to transform Illidan.

Illidan (morphed)illidanMORPHEDUnused. Identical to the Demon Form.

Illidan (Evil)EVILillidanIllidan with wings from TFT.

Illidan (Evil) (demon form)EVILillidanDEMONFORMCreated through Metamorphosis for TFT version of Illidan.

Illidan (Evil) (demon form)EVILillidanDEMONFORMCreated through Metamorphosis for TFT version of Illidan.

Infected GranaryGRANARYinfectedUsed in RoC Human campaign.

Juggernaut (cinematic)ORCjuggernautUsed in RoC for cinematics.

Kel'ThuzadkelthuzadNECROUsed in Human03 and Human04.

Kel'Thuzad (ghost)kelthuzadGHOSTUsed in RoC Undead campaign.

Kel'Thuzad (Lich)kelthuzadLICHUsed in Undead campaigns.

Kel'Thuzad (Lich, cinematic)kelthuzadlichCINEMATICUsed in Undead05Interude.

Kil'Jaeden (cinematic)kiljaedenCINEMATICUsed in Humanx06Finale in modified form.

Kodo BeastkoTobeastKodo Beasts are often called 'Koto' in game data.

Kodo Beast (riderless)koTobeastNORIDERKodo Beasts are often called 'Koto' in game data.

Lava Spawn (level 1)lavaSPAWNSummoned unit from Firelord's ability.

Lava Spawn (level 2)lavaSPAWN2Summoned unit from Firelord's ability.

Lava Spawn (level 3)lavaSPAWN3Summoned unit from Firelord's ability.

Lesser Dark MinionDARKminion1A summoned unit from Dark Ranger's Black Arrow ability.

Lesser Spirit BeastSPIRITBEAST

Lich King (cinematic campaign unit)lichKINGUsed in TFT Undead campaign for portrait during relevant cutscenes.

Lord Nicholas BuzanPALADINBOSS1Paladin from Undead02, known as Gavinrad the Dire.

Lumber MillHUMANlumbermillIn previous games all factions had a lumber mill, so this way to see which one it is is very logical.

Magroth the DefenderMAGROTHPaladin from NightElfx07.

Makrura DeepseerLOBSTROKKdeepseerAll Makruras are called 'Lobstrokk' in some internal files.

Makrura PooldwellerLOBSTROKKpooldwellerAll Makruras are called 'Lobstrokk' in some internal files.

Makrura PrawnLOBSTROKKprawnAll Makruras are called 'Lobstrokk' in some internal files.

Makrura SnapperLOBSTROKKsnapperAll Makruras are called 'Lobstrokk' in some internal files.

Makrura Tidal LordLOBSTROKKtidallordAll Makruras are called 'Lobstrokk' in some internal files.

Makrura TidecallerLOBSTROKKtidecallerAll Makruras are called 'Lobstrokk' in some internal files.

Malfurion (TFT)MALfurionThe main version used in TFT Night Elf campaign.

Malfurion (TFT without stag)MALfurionNOSTAGThe stagless version used in NightElfx06Interlude. Since Reforged uses a model with stag.

MarinemarineThe actual Terran Marine. Used for secret scene in RoC credits.

Medivh (raven form)medivhMORPHED

Mercenary Camp (Ashenvale)mercenarycampAMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Barrens)mercenarycampBMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Black Citadel)mercenarycampKMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Cityscape)mercenarycampYMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Dalaran)mercenarycampXAlso used for Dalaran Ruins tileset. Mercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Dungeon)mercenarycampDMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Felwood)mercenarycampCMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Icecrown Glacier)mercenarycampIMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Lordaeron Fall)mercenarycampFMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Lordaeron Summer)mercenarycampLMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Lordaeron Winter)mercenarycampWMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Northrend)mercenarycampNMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Outland)mercenarycampOMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Sunken Ruins)mercenarycampZMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Underground)mercenarycampGMercenary camps use identifier indexes based on the tileset identifiers.

Mercenary Camp (Village)mercenarycampVAlso used for Village Fall tileset. Mercenary camps use identifier indexes based on the tileset identifiers.

MishamishaUnused weaker version of Rexxar's companion.

Misha (level 1)misha1Summoned unit from Rexxar's ability.

Misha (level 2)misha2Summoned unit from Rexxar's ability.

Misha (level 3)misha3Summoned unit from Rexxar's ability.

Misha (level 4)misha4Summoned unit from Rexxar's ability.

Mur'Gul Hut 0murgulhut0Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Mur'Gul Hut 1murgulhut1Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Mur'Gul MaraudermurgulREAVERAbsolutely unexpected.

Mur'Gul ReaverNAGAMURGULSomewhat expected.

Murloc Hut 0murgulhut0Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Murloc Hut 1murgulhut1Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Naga SirenSIRENSome are ashamed of their tails.

NecropolisnecropolisThe only Undead Town Hall that follows the rules.

Nerubian TowerFROSTtowerNot a cold tower.

Nether Dragon RoostOUTLANDdragonroostThe only time a non-blue dragon name is modified.

Night Elf Fisging Village (crested, southeast facing)ELVENfishingvillage4Used for NightElfx01. Does not follow other naming conventions.

Night Elf Fisging Village (crested, southwest facing)ELVENfishingvillage1Used for NightElfx01. Does not follow other naming conventions.

Night Elf Fisging Village (one-story)ELVENfishingvillage2Used for NightElfx01. Does not follow other naming conventions.

Night Elf Fisging Village (ruined, crested)RUINEDELVENfishingvillage1Used for NightElfx01. Does not follow other naming conventions.

Night Elf Fisging Village (ruined, one-story)RUINEDELVENfishingvillage2Used for NightElfx01. Does not follow other naming conventions.

Night Elf Fisging Village (ruined, two-story)RUINEDELVENfishingvillage0Used for NightElfx01. Does not follow other naming conventions.

Night Elf Fisging Village (two-story, southeast facing)ELVENfishingvillage3Used for NightElfx01. Does not follow other naming conventions.

Night Elf Fisging Village (two-story, southwest facing)ELVENfishingvillage0Used for NightElfx01. Does not follow other naming conventions.

Night Elf FrigatenightelfDESTROYERAll Frigates are called 'Destroyer'. Probably based on Destroyers from WC2.

Ogre Maulerogre2Some creeps have numbers instead of professions.

Ogre Warriorogre1Some creeps have numbers instead of professions.

Orc FrigateorcISHDESTROYEROrcs like to use proper grammer. All Frigates are called 'Destroyer'. Probably based on Destroyers from WC2.

Orc JuggernautJUGGERNAUTBattleship for Orc faction. Identifier avoids and issues with WC2 ogre juggernauts.

Orc Transport ShiporcISHtransportshipOrcs like to use proper grammer.

Orc WarchiefCHAOSWARLORDRed orc on a horse. Never used even as a direct template.

Orc WarlockWAR2warlockIn RoC used only for cinematics. In TFT used as a creep in Rexxar campaign.

Owl Scout (level 1)OWLSummoned unit for Priestess of the Moon's ability.

Owl Scout (level 1)OWLSummoned unit for Priestess of the Moon's ability.

Owl Scout (level 2)OWL2Summoned unit for Priestess of the Moon's ability.

Owl Scout (level 3)OWL3Summoned unit for Priestess of the Moon's ability.

Penguin (waterborne)WATERpenguin

Plague TreantplagueENTTreants tend to be identified as Ents.

Pocket Factory (level 1)POCKETFACTORYSummoned building from Tinker's ability.

Pocket Factory (level 2)POCKETFACTORY2Summoned building from Tinker's ability.

Pocket Factory (level 3)POCKETFACTORY3Summoned building from Tinker's ability.

Poison TreantpoisonENTTreants tend to be identified as Ents.

PortalSHIMMERINGportalBig purple 'stargate' used for cinematics.

Primal PandarenFURBOLGPANDAUsed for Humanx03s.

Prison WagonILLIDANwagonUsed for Humanx04.

QuilbeastquilLbeast1Summoned unit for Beastmaster's ability.

Raging QuilbeastquilLbeast3Summoned unit for Beastmaster's ability.

RaiderWOLFRIDER

Resurrection Stone (facing south-east)BINDstoneSEUsed for Rexxar's campaign.

Resurrection Stone (facing south-west)BINDstoneSWUsed for Rexxar's campaign.

SalamanderDRAENEIsalamanderTrainable by Draenei in Humanx05.

SapperGOBLINsapperThey identify as one and are better than Demolition Squad.

Sapphiron (living)AZURELOREDRAGONHe IS technically a blue dragon.

Sapphiron (undead)AZUREFROSTWYRMHe IS technically a blue dragon.

Sea Witch (original)seawitch2This is the witch from Prologue05. Since Reforged she uses a Sea Witch model instead of Ghost.

Seer's DenDRAENEIseerDENUsed by Draenei base in Humanx05.

SentryWATCHERFrom Tyrande's section of NightElf05.

Serpent Ward (level 1)serpentward1Summoned unit from Shadow Hunter's ability.

Serpent Ward (level 2)serpentward2Summoned unit from Shadow Hunter's ability.

Serpent Ward (level 3)serpentward3Summoned unit from Shadow Hunter's ability.

Serpent Ward (level 4)serpentward4Summoned unit from Rokhan's fully upgraded ability.

Shadow WolfSPIRITwolf3A summoned unit from Far Seer's ability.

Sheep (amphibious)AMPHIBIOUSsheep

ShipBATTLEshipUsed by Thrall in Prologue02.

ShredderGOBLINshredderThose dwarves will never steal goblin technology.

Siege Engine (Barrage)ROCKETengineAn upgraded Steam Tank, added in TFT.

Siege EnginesiegeengineSteam Tank from RoC. I don't know what identifier was used before TFT.

Siege Engine (Barrage)ROCKETengineAn upgraded Steam Tank, added in TFT.

Sir Gregory EdmundsonPALADINBOSS2Paladin from Undead02, known as Sage Truthbearer.

Skeleton ArcherskeletALarcherHe will not admit he is (un)dead.

Skeleton Archer (summoned)skeletALarcherSUMMONEDStats are identical to the usual one.

Skeleton WarriorSKELETONWon't admit he's not an elite archer. Raw ID - nske.

Skeleton Warrior (summoned)skeletonSUMMONEDHas no suffix. Raw ID - nsce.

Sky BargeAIRbargeNot a water barge.

Terran MarineCHAOSSPACEORCNot a real marine - Before Reforged this was a Chaos Space Orc. Since Reforged the original model is missing.

Spider Crab Behemothspidercrab3He started writing 'B', but chose to stop.

Spider Crab Limbripperspidercrab2He started writing 'L', but chose to stop and found a way to fix it.

Spider Crab ShorecrawlerspidercrabDoesn't want people to think he's homeless.

Spirit Beastspiritbeast2

Spirit HawkWAREAGLE3Summoned unit from Beastmaster's ability.

Spirit of VengeanceVENGEANCESummoned unit from Avatar of Vengeance, a summoned unit from Warden's ultimate.

Spirit Walker (ethereal)spiritwalkerMORPHMorph form of Spirit Walker - an expansioln ground unit for Orc faction. Now trained as a basic form.

Spirit Wolfspiritwolf1Summoned unit from Far Seer's ability.

StagDEERThere are no does to be found.

Stasis TrapstasistrapTOTEMA summoned unit from Witch Doctor's ability.

Stone Form GargoyleGARGOYLEMORPHEDNot the defenders of the night.

Stone Form GargoyleGARGOYLEMORPHEDNot the defenders of the night.

Storm (level 1)PANDA2One of three units created by Brewmaster's ultimate.

Storm (level 2)PANDA5One of three units created by Chen's upgraded ultimate.

Submerged Naga MyrmidonnagamyrmidonMORPH

Submerged Naga Royal GuardnagaroyalguardMORPH

Submerged Snap DragonsnapdragonMORPH

Summoned PrawnLOBSTROKKprawnSUMMONEDAll Makruras are called 'Lobstrokk' in some internal files.

SwordsmanHIGHELVENswordsmanHigh Elven version of Footman. Used in Silvermoon missions and in TFT Human campaign.

Sylvanas Windrunner (banshee)sylvanUsbansheeThis is a non-hero version from original Undead05. Featuring the 'u' instead of 'a'.

SylvanasEVILsylvanasFeaturing classic 'Evil' prefix, and with fixed 'a'.

Sylvanas WindrunnersylvanUswindrunnerLiving version from Silvermoon missions. Featuring the 'u' instead of 'a'.

Tauren TenttaurententUnits with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Tauren Tent 2taurentent2Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

TavernCANTINAJazz not included.

TenttentUnits with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Tent 2tent2Units with same names and numbers in their suffixes tend to have a corresponding number in their identifier, with some exceptions.

Thrall (unmounted)thrall unmountedThe only unit with space in identifier. Added in Reforged.

Thunder HawkWAREAGLE2Summoned unit from Beastmaster's ability.

Tinker (morphed version)ROBOGOBLINtinkerMorphed Tinker from his ultimate.

TreantFORCEOFNATURESummoned unit from Keeper of the Grove's ability. The only treant not identified as an Ent.

Tuskarr SorcerertuskarrsorcerOrA lot of units with 'Sorcerer' in their name use 'sorcerOr' in identifier.

Undead FrigateundeadDESTROYERAll Frigates are called 'Destroyer'. Probably based on Destroyers from WC2.

Villager (Female)villagerWOMAN

Villager (Female)villagerWOMANA template for some named characters.

Villager (Male)villagerMANA template for some named characters.

Villager (Male 2)villagerMAN2A template for some named characters.

WagonBLOODELFwagonUsed in NightElfx06.

WagonBLOODELFwagonUsed in NightElfx06.

WardenNIGHTELFASSASSINA non-hero unit used in NightElf05.

Water Elemental (level 1)waterelemental1A summoned unit from Archmage's ability.

Water Elemental (level 2)waterelemental2A summoned unit from Archmage's ability.

Water Elemental (level 3)waterelemental3A summoned unit from Archmage's ability.

Watery minion (level 1)wateryminionCLIFFRUNNERA summoned unit Naga Siren's 'Parasite' ability. Has an extra job.

Watery minion (level 2)wateryminionTIDEWARRIORUnused. Has an extra job.

Watery minion (level 3)wateryminionSNARECASTERUnused. Has an extra job.

Wendigowendigo1Some creeps have numbers instead of professions. Some even have numbers despite being plain simple creeps.

WildkinOWLBEARBefore TFT, they were Owlbears.

Wind RiderwindriderBefore TFT, they probably were identified as Wyverns.

Wind Rider (riderless)RIDERLESSWYVERNThe only unit who is propoerly identified as 'rider;ess'. But who rides the Wind Rider? Identifier based on original name.

Wind Rider Cage 1wyverncage1Identifier based on original name. Thrall freed wyverns, not riders.

Wind Rider Cage 2wyverncage2Identifier based on original name. Thrall freed wyverns, not riders.

Wind Rider Cage 3wyverncage3Identifier based on original name. Thrall freed wyverns, not riders.

Wind Rider Cage 4wyverncage4Identifier based on original name. Thrall freed wyverns, not riders.

Worker (blood elf)BLOODELFworkerUsed in TFT Human campaign in place of Peasant.

ZeppelinGOBLINzeppelinNot Led.

Zombie (Female)zombieFEMALEAdded in Reforged for Human06.

Feel free to contact me if there are any mistakes here or if there is new data.


 Last edited: Jun 19, 2025

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I find this quite interesting because I've never felt the need to make something like this myself.

Admittedly I have never had a map with 300 units so I understand that some traditional approaches do not scale, such as navigating the UI with 300+ icons or the object editor with endless rows of units.

I'll mention for potential future readers that if you're not bound by GUI limits there is a better solution for those willing to learn Wurst (and possibly typescript etc, I am a bit out of date).

Because you can generate objects with code you can also save their ids on-generation as a variable.

Then, assuming you're using VS Code or an equally competent editor the auto complete and good naming will make you find things very quickly such as HUMAN_HERO_<NAME>. You can then combine this with hot reload with allows you to update code without restarting the client (the game) for very quick testing with specific units.

That said, with the spirit of the tutorial I think there are some other useful things that could be mentioned here.

-duplicate could spawn a new unit of currently selected unit

emulate chat history by using up arrow you can cycle through previous commands.

There are also some more oldschool solutions that not only help with this specific use case but also productivity in general. For example you use proper editor categories to seperate your units so they're easy to find, use the editor only prefix to easily group units.. things like that.

edit: I don't remember if hot reload works for GUI or only JASS but that might be worth looking into for any GUI user that does not use it already.

#### Yevhen Snizhko

#### Yevhen Snizhko

 Level 12

JoinedMay 13, 2023Messages157

Resources: 4

#### Resources
Models 1Maps 2Tutorials 1

Chaosy said:

 There are also some more oldschool solutions that not only help with this specific use case but also productivity in general. For example you use proper editor categories to seperate your units so they're easy to find, use the editor only prefix to easily group units.. things like that.


Click to expand...

I agree that there are better options to do this on small scale. I, however, got interested in this topic because of my specific use case - allowing a user to make a custom tech-tree in 'Settings' level.

Since I was unable to make a good-looking UI for that, I chose to implement the 'Enter chat message' method.

The only useful generalized information was here: conversasion - convert string to unit-type [how?], but I had to check how multi-word unit names worked and what identifiers were set for units that are changed in TFT.

I found out is that there are rules, and unit types that break them.

So I think that the most important part of this tutorial is the table at the end of it. But without the context at the beginning, the table would mean nothing to pretty much anyone.

Still, I appreciate your comment about more advanced ways of achieving similar results.

Chaosy said:

 That said, with the spirit of the tutorial I think there are some other useful things that could be mentioned here.

-duplicate could spawn a new unit of currently selected unit

emulate chat history by using up arrow you can cycle through previous commands.


Click to expand...

I'll think how to add descriptions of other stuff people can do with this. So far I've only created or replaced units, but it could also be a mechanic for game modes - like typing a name of unit to buff allied units of that type or kill enemy units of that type (like 'Magic Touch' game by Nitrome).

Just duplicating a unit using chat is not really dependent on identifiers, as unit type of selected unit can be found without the strings in GUI.

Pressing arrow to cycle through chat - out of scope of this tutorial, but it is an interesting topic. I've seen similar things done by other people some time ago, but have never used it.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I don't think it has to be a one or the other.

I think this can be extended to a bigger umbrella topic that covers this and by extension other things.

Regardless if you have 100, 200 or even more object editor entries, categorizing them is good practice and helps with everything related to development because this problem occur every time you want to reference something in the object editor which is obviously a lot when you're coding and especially when you're using GUI which is the primary target for this tutorial.

This does not exclude your idea of being able to summon them by text for testing but rather it works in addition to.

I am not going to try force you to write it this way, if you just want to cover the more narrow aspect that's cool with me I am just pointing out there is the option to cover it more broadly as well.

#### Yevhen Snizhko

#### Yevhen Snizhko

 Level 12

JoinedMay 13, 2023Messages157

Resources: 4

#### Resources
Models 1Maps 2Tutorials 1

Added Sammy! (Sam Didier cube) and Kobold Shoveler (kobold with human face).

EDIT:

2025 Jun 19: added a mention that this is useful for preloading custom units.


 Last edited: Jun 19, 2025

 You must log in or register to reply here.


### Similar threads

Locked

 correctly identify the name of the player ?


nhattuanbl

Feb 14, 2013

World Editor Help Zone

Replies4Views1K

Feb 14, 2013
nhattuanbl

Locked

 Convert Unit-Type to String problem


claptomanic

Jul 30, 2012

Triggers & Scripts

Replies1Views2K

Jul 30, 2012
Garfield1337

Locked

 Unit's hp regen per second?


bear_369

Sep 10, 2015

World Editor Help Zone

Replies3Views1K

Sep 11, 2015
Dr Super Good

 A Collection of GUI Systems > Basic to Advanced (Incomplete)


Kwah

Aug 28, 2008

Trigger (GUI) Editor Tutorials

Replies4Views14K

Aug 29, 2008
Kwah

Locked

 Can't create custom units using their string name.


Ghullie

Nov 13, 2019

Triggers & Scripts

Replies7Views2K

Nov 20, 2019
Pyrogasm

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Trigger (GUI) Editor Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top