---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Constants/TowerConstants.cs"
ref: HEAD
---

using System.Collections.Generic;
using UnityEngine;

public class TowerConstants {
    public const double SellReturnValue = .70;
    public const double SellDuration = 1.5;

    public static readonly Dictionary<TowerType, int> BuildCost = new Dictionary<TowerType, int>() {
        { TowerType.Archer, 10 },
        { TowerType.Cutter, 10 },
        { TowerType.ElementalCore, 200 },
        { TowerType.TechnologyDisc, 2500 },
    };
    
    public static readonly Dictionary<TowerType, string> DisplayName = new Dictionary<TowerType, string>() {
        { TowerType.Archer, "Archer" },
        { TowerType.Gunner, "Gunner" },
        { TowerType.WatchTower, "Watch Tower" },
        { TowerType.GuardTower, "Guard Tower" },
        { TowerType.WardTower, "Ward Tower" },
        { TowerType.UltimateWardTower, "Ultimate Ward Tower" },
        { TowerType.CannonTower, "Cannon Tower" },
        { TowerType.BombardTower, "Bombard Tower" },
        { TowerType.ArtilleryTower, "Artillery Tower" },
        { TowerType.UltimateArtilleryTower, "Ultimate Artillery Tower" },
        { TowerType.Cutter, "Cutter" },
        { TowerType.Grinder, "Grinder" },
        { TowerType.Carver, "Carver" },
        { TowerType.Executioner, "Executioner" },
        { TowerType.Mauler, "Mauler" },
        { TowerType.UltimateMauler, "Ultimate Mauler" },
        { TowerType.Crusher, "Crusher" },
        { TowerType.Wrecker, "Wrecker" },
        { TowerType.Mangler, "Mangler" },
        { TowerType.UltimateMangler, "Ultimate Mangler" },
        { TowerType.ElementalCore, "Elemental Core" },
        { TowerType.Spellslinger, "Spellslinger" },
        { TowerType.SpellslingerMaster, "Spellslinger Master" },
        { TowerType.ArcanePylon, "Arcane Pylon" },
        { TowerType.ArcaneRepository, "Arcane Repository" },
        { TowerType.UltimateArcaneOrb, "Ultimate Arcane Orb" },
        { TowerType.Archmage, "Archmage" },
        { TowerType.GrandArchmage, "Grand Archmage" },
        { TowerType.UltimateKirinTorWizard, "Ultimate Kirin Tor Wizard" },
        { TowerType.FirePit, "Fire Pit" },
        { TowerType.MagmaWell, "Magma Well" },
        { TowerType.LivingFlame, "Living Flame" },
        { TowerType.Hellfire, "Hellfire" },
        { TowerType.UltimateFirelord, "Ultimate Firelord" },
        { TowerType.MeteorAttractor, "Meteor Attractor" },
        { TowerType.Armageddon, "Armageddon" },
        { TowerType.UltimateMoonbeamProjector, "Ultimate Moonbeam Projector" },
        { TowerType.Rockfall, "Rockfall" },
        { TowerType.Avalanche, "Avalanche" },
        { TowerType.EarthGuardian, "Earth Guardian" },
        { TowerType.AncientProtector, "Ancient Protector" },
        { TowerType.UltimateAncientWarden, "Ultimate Ancient Warden" },
        { TowerType.NoxiousWeed, "Noxious Weed" },
        { TowerType.NoxiousThorn, "Noxious Thorn" },
        { TowerType.UltimateScorpion, "Ultimate Scorpion" },
        { TowerType.Voidling, "Voidling" },
        { TowerType.Voidalisk, "Voidalisk" },
        { TowerType.Riftweaver, "Riftweaver" },
        { TowerType.RiftLord, "Rift Lord" },
        { TowerType.UltimateDevourer, "Ultimate Devourer" },
        { TowerType.Lasher, "Lasher" },
        { TowerType.Ravager, "Ravager" },
        { TowerType.UltimateLeviathan, "Ultimate Leviathan" },
        { TowerType.Splasher, "Splasher" },
        { TowerType.Tidecaller, "Tidecaller" },
        { TowerType.WaterElemental, "Water Elemental" },
        { TowerType.SeaElemental, "Sea Elemental" },
        { TowerType.UltimateHurricaneElemental, "Ultimate Hurricane Elemental" },
        { TowerType.TideLurker, "Tide Lurker" },
        { TowerType.AbyssStalker, "Abyss Stalker" },
        { TowerType.UltimateSludgeMonstrosity, "Ultimate Sludge Monstrosity" },
        { TowerType.ShockParticle, "Shock Particle" },
        { TowerType.ShockGenerator, "Shock Generator" },
        { TowerType.Voltage, "Voltage" },
        { TowerType.HighVoltage, "High Voltage" },
        { TowerType.UltimateOrbKeeper, "Ultimate Orb Keeper" },
        { TowerType.LightningBeacon, "Lightning Beacon" },
        { TowerType.LightningGenerator, "Lightning Generator" },
        { TowerType.UltimateAnnihilationGlyph, "Ultimate Annihilation Glyph" },
        { TowerType.LightFlies, "Light Flies" },
        { TowerType.HolyLantern, "Holy Lantern" },
        { TowerType.SunrayTower, "Sunray Tower" },
        { TowerType.SunbeamTower, "Sunbeam Tower" },
        { TowerType.UltimateTitanVault, "Ultimate Titan Vault" },
        { TowerType.Glowshroom, "Glowshroom" },
        { TowerType.Lightshroom, "Lightshroom" },
        { TowerType.UltimateDivineshroom, "Ultimate Divineshroom" },
        { TowerType.FrozenObelisk, "Frozen Obelisk" },
        { TowerType.RunicObelisk, "Runic Obelisk" },
        { TowerType.FrozenWatcher, "Frozen Watcher" },
        { TowerType.FrozenCore, "Frozen Core" },
        { TowerType.UltimateLich, "Ultimate Lich" },
        { TowerType.Icicle, "Icicle" },
        { TowerType.Tricicle, "Tricicle" },
        { TowerType.UltimateCrystal, "Ultimate Crystal" },
        { TowerType.PlagueWell, "Plague Well" },
        { TowerType.Catacomb, "Catacomb" },
        { TowerType.SepticTank, "Septic Tank" },
        { TowerType.PlagueFanatic, "Plague Fanatic" },
        { TowerType.UltimateGravedigger, "Ultimate Gravedigger" },
        { TowerType.ObsidianDestroyer, "Obsidian Destroyer" },
        { TowerType.DecayedHorror, "Decayed Horror" },
        { TowerType.UltimateDiabolist, "Ultimate Diabolist" },
        { TowerType.TechnologyDisc, "Technology Disc" },
        { TowerType.TechnologyDisc_Arcane, "Technology Disc: Arcane" },
        { TowerType.TechnologyDisc_Fire, "Technology Disc: Fire" },
        { TowerType.TechnologyDisc_Ice, "Technology Disc: Ice" },
        { TowerType.TechnologyDisc_Earth, "Technology Disc: Earth" },
        { TowerType.TechnologyDisc_Lightning, "Technology Disc: Lightning" },
        { TowerType.TechnologyDisc_Holy, "Technology Disc: Holy" },
        { TowerType.TechnologyDisc_Water, "Technology Disc: Water" },
        { TowerType.TechnologyDisc_Unholy, "Technology Disc: Unholy" },
        { TowerType.TechnologyDisc_Void, "Technology Disc: Void" },
        { TowerType.TechnologyDisc_Arcane_Ultimate, "Technology Disc: Advanced Arcane" },
        { TowerType.TechnologyDisc_Fire_Ultimate, "Technology Disc: Advanced Fire" },
        { TowerType.TechnologyDisc_Ice_Ultimate, "Technology Disc: Advanced Ice" },
        { TowerType.TechnologyDisc_Earth_Ultimate, "Technology Disc: Advanced Earth" },
        { TowerType.TechnologyDisc_Lightning_Ultimate, "Technology Disc: Advanced Lightning" },
        { TowerType.TechnologyDisc_Holy_Ultimate, "Technology Disc: Advanced Holy" },
        { TowerType.TechnologyDisc_Water_Ultimate, "Technology Disc: Advanced Water" },
        { TowerType.TechnologyDisc_Unholy_Ultimate, "Technology Disc: Advanced Unholy" },
        { TowerType.TechnologyDisc_Void_Ultimate, "Technology Disc: Advanced Void" },
    };

    // Translation: 400 WC3 = ~3.15 boxes horizontal = ~2.25 boxes diagonal = 31.5 Unity
    // WC3 scale is 128 x 128, our scale is 100 x 100
    // 100 / 128 = .78125
    // Translation from WC3 to LTW2 range: WC3 * .78125 = LTW
    public static readonly Dictionary<TowerType, double> Range = new Dictionary<TowerType, double>() {
        { TowerType.Archer, 312.5 },
        { TowerType.Gunner, 390 },
        { TowerType.WatchTower, 545 },
        { TowerType.GuardTower, 545 },
        { TowerType.WardTower, 625 },
        { TowerType.UltimateWardTower, 625 },
        { TowerType.CannonTower, 390 },
        { TowerType.BombardTower, 430 },
        { TowerType.ArtilleryTower, 470 },
        { TowerType.UltimateArtilleryTower, 510 },
        { TowerType.Cutter, 117 },
        { TowerType.Grinder, 117 },
        { TowerType.Carver, 117 },
        { TowerType.Executioner, 117 },
        { TowerType.Mauler, 155 },
        { TowerType.UltimateMauler, 155 },
        { TowerType.Crusher, 117 },
        { TowerType.Wrecker, 117 },
        { TowerType.Mangler, 117 },
        { TowerType.UltimateMangler, 117 },
        { TowerType.ElementalCore, 470 },
        { TowerType.Spellslinger, 780 },
        { TowerType.SpellslingerMaster, 780 },
        { TowerType.ArcanePylon, 703 },
        { TowerType.ArcaneRepository, 742 },
        { TowerType.UltimateArcaneOrb, 780 },
        { TowerType.Archmage, 625 },
        { TowerType.GrandArchmage, 625 },
        { TowerType.UltimateKirinTorWizard, 625 },
        { TowerType.FirePit, 313 },
        { TowerType.MagmaWell, 313 },
        { TowerType.LivingFlame, 313 },
        { TowerType.Hellfire, 313 },
        { TowerType.UltimateFirelord, 390 },
        { TowerType.MeteorAttractor, 625 },
        { TowerType.Armageddon, 762 },
        { TowerType.UltimateMoonbeamProjector, 900 },
        { TowerType.Rockfall, 235 },
        { TowerType.Avalanche, 235 },
        { TowerType.EarthGuardian, 313 },
        { TowerType.AncientProtector, 350 },
        { TowerType.UltimateAncientWarden, 390 },
        { TowerType.NoxiousWeed, 510 },
        { TowerType.NoxiousThorn, 510 },
        { TowerType.UltimateScorpion, 545 },
        { TowerType.Voidling, 275 },
        { TowerType.Voidalisk, 275 },
        { TowerType.Riftweaver, 470 },
        { TowerType.RiftLord, 470 },
        { TowerType.UltimateDevourer, 625 },
        { TowerType.Lasher, 313 },
        { TowerType.Ravager, 313 },
        { TowerType.UltimateLeviathan, 313 },
        { TowerType.Splasher, 390 },
        { TowerType.Tidecaller, 390 },
        { TowerType.WaterElemental, 390 },
        { TowerType.SeaElemental, 390 },
        { TowerType.UltimateHurricaneElemental, 390 },
        { TowerType.TideLurker, 470 },
        { TowerType.AbyssStalker, 470 },
        { TowerType.UltimateSludgeMonstrosity, 470 },
        { TowerType.ShockParticle, 155 },
        { TowerType.ShockGenerator, 155 },
        { TowerType.Voltage, 155 },
        { TowerType.HighVoltage, 155 },
        { TowerType.UltimateOrbKeeper, 155 },
        { TowerType.LightningBeacon, 780 },
        { TowerType.LightningGenerator, 975 },
        { TowerType.UltimateAnnihilationGlyph, 1170 },
        { TowerType.LightFlies, 625 },
        { TowerType.HolyLantern, 665 },
        { TowerType.SunrayTower, 700 },
        { TowerType.SunbeamTower, 780 },
        { TowerType.UltimateTitanVault, 780 },
        { TowerType.Glowshroom, 313 },
        { TowerType.Lightshroom, 313 },
        { TowerType.UltimateDivineshroom, 313 },
        { TowerType.FrozenObelisk, 390 },
        { TowerType.RunicObelisk, 470 },
        { TowerType.FrozenWatcher, 470 },
        { TowerType.FrozenCore, 470 },
        { TowerType.UltimateLich, 470 },
        { TowerType.Icicle, 545 },
        { TowerType.Tricicle, 545 },
        { TowerType.UltimateCrystal, 625 },
        { TowerType.PlagueWell, 470 },
        { TowerType.Catacomb, 510 },
        { TowerType.SepticTank, 545 },
        { TowerType.PlagueFanatic, 545 },
        { TowerType.UltimateGravedigger, 545 },
        { TowerType.ObsidianDestroyer, 235 },
        { TowerType.DecayedHorror, 235 },
        { TowerType.UltimateDiabolist, 313 },
        { TowerType.TechnologyDisc, 0 },
        { TowerType.TechnologyDisc_Arcane, 0 },
        { TowerType.TechnologyDisc_Fire, 0 },
        { TowerType.TechnologyDisc_Ice, 0 },
        { TowerType.TechnologyDisc_Earth, 0 },
        { TowerType.TechnologyDisc_Lightning, 0 },
        { TowerType.TechnologyDisc_Holy, 0 },
        { TowerType.TechnologyDisc_Water, 0 },
        { TowerType.TechnologyDisc_Unholy, 0 },
        { TowerType.TechnologyDisc_Void, 0 },
        { TowerType.TechnologyDisc_Arcane_Ultimate, 0 },
        { TowerType.TechnologyDisc_Fire_Ultimate, 0 },
        { TowerType.TechnologyDisc_Ice_Ultimate, 0 },
        { TowerType.TechnologyDisc_Earth_Ultimate, 0 },
        { TowerType.TechnologyDisc_Lightning_Ultimate, 0 },
        { TowerType.TechnologyDisc_Holy_Ultimate, 0 },
        { TowerType.TechnologyDisc_Water_Ultimate, 0 },
        { TowerType.TechnologyDisc_Unholy_Ultimate, 0 },
        { TowerType.TechnologyDisc_Void_Ultimate, 0 },
    };

    public static readonly Dictionary<TowerType, double> Cooldown = new Dictionary<TowerType, double>() {
        { TowerType.Archer, .66 },
        { TowerType.Gunner, .63 },
        { TowerType.WatchTower, .5 },
        { TowerType.GuardTower, .5 },
        { TowerType.WardTower, .5 },
        { TowerType.UltimateWardTower, .5 },
        { TowerType.CannonTower, 2 },
        { TowerType.BombardTower, 2 },
        { TowerType.ArtilleryTower, 2 },
        { TowerType.UltimateArtilleryTower, 2 },
        { TowerType.Cutter, .33 },
        { TowerType.Grinder, .33 },
        { TowerType.Carver, .33 },
        { TowerType.Executioner, .33 },
        { TowerType.Mauler, .33 },
        { TowerType.UltimateMauler, .33 },
        { TowerType.Crusher, 5 },
        { TowerType.Wrecker, 5 },
        { TowerType.Mangler, 5 },
        { TowerType.UltimateMangler, 5 },
        { TowerType.ElementalCore, 1 },
        { TowerType.Spellslinger, .33 },
        { TowerType.SpellslingerMaster, .33 },
        { TowerType.ArcanePylon, .5 },
        { TowerType.ArcaneRepository, .5 },
        { TowerType.UltimateArcaneOrb, .33 },
        { TowerType.Archmage, 1 },
        { TowerType.GrandArchmage, 1 },
        { TowerType.UltimateKirinTorWizard, 1 },
        { TowerType.FirePit, 2.5 },
        { TowerType.MagmaWell, 2.5 },
        { TowerType.LivingFlame, 0.5 },
        { TowerType.Hellfire, 0.4 },
        { TowerType.UltimateFirelord, 0.4 },
        { TowerType.MeteorAttractor, 2.5 },
        { TowerType.Armageddon, 2.5 },
        { TowerType.UltimateMoonbeamProjector, 2.5 },
        { TowerType.Rockfall, 2.5 },
        { TowerType.Avalanche, 2.5 },
        { TowerType.EarthGuardian, 2.5 },
        { TowerType.AncientProtector, 2.5 },
        { TowerType.UltimateAncientWarden, 2.5 },
        { TowerType.NoxiousWeed, 0.7 },
        { TowerType.NoxiousThorn, 0.7 },
        { TowerType.UltimateScorpion, 0.4 },
        { TowerType.Voidling, 3 },
        { TowerType.Voidalisk, 3 },
        { TowerType.Riftweaver, 2 },
        { TowerType.RiftLord, 2 },
        { TowerType.UltimateDevourer, 2 },
        { TowerType.Lasher, .7 },
        { TowerType.Ravager, .7 },
        { TowerType.UltimateLeviathan, .7 },
        { TowerType.Splasher, .5 },
        { TowerType.Tidecaller, .5 },
        { TowerType.WaterElemental, .5 },
        { TowerType.SeaElemental, .5 },
        { TowerType.UltimateHurricaneElemental, .5 },
        { TowerType.TideLurker, 1.5 },
        { TowerType.AbyssStalker, 1.5 },
        { TowerType.UltimateSludgeMonstrosity, 1.5 },
        { TowerType.ShockParticle, .5 },
        { TowerType.ShockGenerator, .5 },
        { TowerType.Voltage, .33 },
        { TowerType.HighVoltage, .33 },
        { TowerType.UltimateOrbKeeper, .33 },
        { TowerType.LightningBeacon, 2 },
        { TowerType.LightningGenerator, 2 },
        { TowerType.UltimateAnnihilationGlyph, 2 },
        { TowerType.LightFlies, 2 },
        { TowerType.HolyLantern, 2 },
        { TowerType.SunrayTower, 2 },
        { TowerType.SunbeamTower, 2 },
        { TowerType.UltimateTitanVault, 2 },
        { TowerType.Glowshroom, 1 },
        { TowerType.Lightshroom, 1 },
        { TowerType.UltimateDivineshroom, 1 },
        { TowerType.FrozenObelisk, 1.5 },
        { TowerType.RunicObelisk, 1.5 },
        { TowerType.FrozenWatcher, 1 },
        { TowerType.FrozenCore, 1 },
        { TowerType.UltimateLich, 1 },
        { TowerType.Icicle, 1 },
        { TowerType.Tricicle, 1 },
        { TowerType.UltimateCrystal, 1 },
        { TowerType.PlagueWell, 1 },
        { TowerType.Catacomb, 1 },
        { TowerType.SepticTank, .5 },
        { TowerType.PlagueFanatic, .5 },
        { TowerType.UltimateGravedigger, .5 },
        { TowerType.ObsidianDestroyer, 1 },
        { TowerType.DecayedHorror, 1 },
        { TowerType.UltimateDiabolist, 1 },
        { TowerType.TechnologyDisc, 1 },
        { TowerType.TechnologyDisc_Arcane, 1 },
        { TowerType.TechnologyDisc_Fire, 1 },
        { TowerType.TechnologyDisc_Ice, 1 },
        { TowerType.TechnologyDisc_Earth, 1 },
        { TowerType.TechnologyDisc_Lightning, 1 },
        { TowerType.TechnologyDisc_Holy, 1 },
        { TowerType.TechnologyDisc_Water, 1 },
        { TowerType.TechnologyDisc_Unholy, 1 },
        { TowerType.TechnologyDisc_Void, 1 },
        { TowerType.TechnologyDisc_Arcane_Ultimate, 1 },
        { TowerType.TechnologyDisc_Fire_Ultimate, 1 },
        { TowerType.TechnologyDisc_Ice_Ultimate, 1 },
        { TowerType.TechnologyDisc_Earth_Ultimate, 1 },
        { TowerType.TechnologyDisc_Lightning_Ultimate, 1 },
        { TowerType.TechnologyDisc_Holy_Ultimate, 1 },
        { TowerType.TechnologyDisc_Water_Ultimate, 1 },
        { TowerType.TechnologyDisc_Unholy_Ultimate, 1 },
        { TowerType.TechnologyDisc_Void_Ultimate, 1 },
    };

    public static readonly Dictionary<TowerType, double> PlacementHeight = new Dictionary<TowerType, double>() {
        { TowerType.Archer, 1 },
        { TowerType.Gunner, 1 },
        { TowerType.WatchTower, 1 },
        { TowerType.GuardTower, 1 },
        { TowerType.WardTower, 1 },
        { TowerType.UltimateWardTower, 1 },
        { TowerType.CannonTower, 1 },
        { TowerType.BombardTower, 1 },
        { TowerType.ArtilleryTower, 1 },
        { TowerType.UltimateArtilleryTower, 1 },
        { TowerType.Cutter, 1 },
        { TowerType.Grinder, 1 },
        { TowerType.Carver, 1 },
        { TowerType.Executioner, 1 },
        { TowerType.Mauler, 1 },
        { TowerType.UltimateMauler, 1 },
        { TowerType.Crusher, 1 },
        { TowerType.Wrecker, 1 },
        { TowerType.Mangler, 1 },
        { TowerType.UltimateMangler, 1 },
        { TowerType.ElementalCore, 1 },
        { TowerType.Spellslinger, 1 },
        { TowerType.SpellslingerMaster, 1 },
        { TowerType.ArcanePylon, 1 },
        { TowerType.ArcaneRepository, 1 },
        { TowerType.UltimateArcaneOrb, 1 },
        { TowerType.Archmage, 1 },
        { TowerType.GrandArchmage, 1 },
        { TowerType.UltimateKirinTorWizard, 1 },
        { TowerType.FirePit, 1 },
        { TowerType.MagmaWell, 1 },
        { TowerType.LivingFlame, 1 },
        { TowerType.Hellfire, 1 },
        { TowerType.UltimateFirelord, 1 },
        { TowerType.MeteorAttractor, 1 },
        { TowerType.Armageddon, 1 },
        { TowerType.UltimateMoonbeamProjector, 1 },
        { TowerType.Rockfall, 1 },
        { TowerType.Avalanche, 1 },
        { TowerType.EarthGuardian, 1 },
        { TowerType.AncientProtector, 1 },
        { TowerType.UltimateAncientWarden, 1 },
        { TowerType.NoxiousWeed, 1 },
        { TowerType.NoxiousThorn, 1 },
        { TowerType.UltimateScorpion, 1 },
        { TowerType.Voidling, 1 },
        { TowerType.Voidalisk, 1 },
        { TowerType.Riftweaver, 1 },
        { TowerType.RiftLord, 1 },
        { TowerType.UltimateDevourer, 1 },
        { TowerType.Lasher, 1 },
        { TowerType.Ravager, 1 },
        { TowerType.UltimateLeviathan, 1 },
        { TowerType.Splasher, 1 },
        { TowerType.Tidecaller, 1 },
        { TowerType.WaterElemental, 1 },
        { TowerType.SeaElemental, 1 },
        { TowerType.UltimateHurricaneElemental, 1 },
        { TowerType.TideLurker, 1 },
        { TowerType.AbyssStalker, 1 },
        { TowerType.UltimateSludgeMonstrosity, 1 },
        { TowerType.ShockParticle, 1 },
        { TowerType.ShockGenerator, 1 },
        { TowerType.Voltage, 1 },
        { TowerType.HighVoltage, 1 },
        { TowerType.UltimateOrbKeeper, 1 },
        { TowerType.LightningBeacon, 1 },
        { TowerType.LightningGenerator, 1 },
        { TowerType.UltimateAnnihilationGlyph, 1 },
        { TowerType.LightFlies, 1 },
        { TowerType.HolyLantern, 1 },
        { TowerType.SunrayTower, 1 },
        { TowerType.SunbeamTower, 1 },
        { TowerType.UltimateTitanVault, 1 },
        { TowerType.Glowshroom, 1 },
        { TowerType.Lightshroom, 1 },
        { TowerType.UltimateDivineshroom, 1 },
        { TowerType.FrozenObelisk, 1 },
        { TowerType.RunicObelisk, 1 },
        { TowerType.FrozenWatcher, 1 },
        { TowerType.FrozenCore, 1 },
        { TowerType.UltimateLich, 1 },
        { TowerType.Icicle, 1 },
        { TowerType.Tricicle, 1 },
        { TowerType.UltimateCrystal, 1 },
        { TowerType.PlagueWell, 1 },
        { TowerType.Catacomb, 1 },
        { TowerType.SepticTank, 1 },
        { TowerType.PlagueFanatic, 1 },
        { TowerType.UltimateGravedigger, 1 },
        { TowerType.ObsidianDestroyer, 1 },
        { TowerType.DecayedHorror, 1 },
        { TowerType.UltimateDiabolist, 1 },
        { TowerType.TechnologyDisc, 0 },
        { TowerType.TechnologyDisc_Arcane, 0 },
        { TowerType.TechnologyDisc_Fire, 0 },
        { TowerType.TechnologyDisc_Ice, 0 },
        { TowerType.TechnologyDisc_Earth, 0 },
        { TowerType.TechnologyDisc_Lightning, 0 },
        { TowerType.TechnologyDisc_Holy, 0 },
        { TowerType.TechnologyDisc_Water, 0 },
        { TowerType.TechnologyDisc_Unholy, 0 },
        { TowerType.TechnologyDisc_Void, 0 },
        { TowerType.TechnologyDisc_Arcane_Ultimate, 0 },
        { TowerType.TechnologyDisc_Fire_Ultimate, 0 },
        { TowerType.TechnologyDisc_Ice_Ultimate, 0 },
        { TowerType.TechnologyDisc_Earth_Ultimate, 0 },
        { TowerType.TechnologyDisc_Lightning_Ultimate, 0 },
        { TowerType.TechnologyDisc_Holy_Ultimate, 0 },
        { TowerType.TechnologyDisc_Water_Ultimate, 0 },
        { TowerType.TechnologyDisc_Unholy_Ultimate, 0 },
        { TowerType.TechnologyDisc_Void_Ultimate, 0 },
    };

    public static readonly Dictionary<TowerType, int> HP = new Dictionary<TowerType, int>() {
        { TowerType.Archer, 25 },
        { TowerType.Gunner, 35 },
        { TowerType.WatchTower, 75 },
        { TowerType.GuardTower, 175 },
        { TowerType.WardTower, 575 },
        { TowerType.UltimateWardTower, 1 },
        { TowerType.CannonTower, 75 },
        { TowerType.BombardTower, 175 },
        { TowerType.ArtilleryTower, 575 },
        { TowerType.UltimateArtilleryTower, 1 },
        { TowerType.Cutter, 25 },
        { TowerType.Grinder, 35 },
        { TowerType.Carver, 75 },
        { TowerType.Executioner, 175 },
        { TowerType.Mauler, 575 },
        { TowerType.UltimateMauler, 1 },
        { TowerType.Crusher, 75 },
        { TowerType.Wrecker, 175 },
        { TowerType.Mangler, 575 },
        { TowerType.UltimateMangler, 1 },
        { TowerType.ElementalCore, 100 },
        { TowerType.Spellslinger, 100 },
        { TowerType.SpellslingerMaster, 1 },
        { TowerType.ArcanePylon, 1 },
        { TowerType.ArcaneRepository, 1 },
        { TowerType.UltimateArcaneOrb, 1 },
        { TowerType.Archmage, 1 },
        { TowerType.GrandArchmage, 1 },
        { TowerType.UltimateKirinTorWizard, 1 },
        { TowerType.FirePit, 100 },
        { TowerType.MagmaWell, 1 },
        { TowerType.LivingFlame, 1 },
        { TowerType.Hellfire, 1 },
        { TowerType.UltimateFirelord, 1 },
        { TowerType.MeteorAttractor, 1 },
        { TowerType.Armageddon, 1 },
        { TowerType.UltimateMoonbeamProjector, 1 },
        { TowerType.Rockfall, 100 },
        { TowerType.Avalanche, 1 },
        { TowerType.EarthGuardian, 1 },
        { TowerType.AncientProtector, 1 },
        { TowerType.UltimateAncientWarden, 1 },
        { TowerType.NoxiousWeed, 1 },
        { TowerType.NoxiousThorn, 1 },
        { TowerType.UltimateScorpion, 1 },
        { TowerType.Voidling, 100 },
        { TowerType.Voidalisk, 1 },
        { TowerType.Riftweaver, 1 },
        { TowerType.RiftLord, 1 },
        { TowerType.UltimateDevourer, 1 },
        { TowerType.Lasher, 1 },
        { TowerType.Ravager, 1 },
        { TowerType.UltimateLeviathan, 1 },
        { TowerType.Splasher, 100 },
        { TowerType.Tidecaller, 1 },
        { TowerType.WaterElemental, 1 },
        { TowerType.SeaElemental, 1 },
        { TowerType.UltimateHurricaneElemental, 1 },
        { TowerType.TideLurker, 1 },
        { TowerType.AbyssStalker, 1 },
        { TowerType.UltimateSludgeMonstrosity, 1 },
        { TowerType.ShockParticle, 100 },
        { TowerType.ShockGenerator, 1 },
        { TowerType.Voltage, 1 },
        { TowerType.HighVoltage, 1 },
        { TowerType.UltimateOrbKeeper, 1 },
        { TowerType.LightningBeacon, 1 },
        { TowerType.LightningGenerator, 1 },
        { TowerType.UltimateAnnihilationGlyph, 1 },
        { TowerType.LightFlies, 100 },
        { TowerType.HolyLantern, 1 },
        { TowerType.SunrayTower, 1 },
        { TowerType.SunbeamTower, 1 },
        { TowerType.UltimateTitanVault, 1 },
        { TowerType.Glowshroom, 1 },
        { TowerType.Lightshroom, 1 },
        { TowerType.UltimateDivineshroom, 1 },
        { TowerType.FrozenObelisk, 100 },
        { TowerType.RunicObelisk, 1 },
        { TowerType.FrozenWatcher, 1 },
        { TowerType.FrozenCore, 1 },
        { TowerType.UltimateLich, 1 },
        { TowerType.Icicle, 1 },
        { TowerType.Tricicle, 1 },
        { TowerType.UltimateCrystal, 1 },
        { TowerType.PlagueWell, 100 },
        { TowerType.Catacomb, 1 },
        { TowerType.SepticTank, 1 },
        { TowerType.PlagueFanatic, 1 },
        { TowerType.UltimateGravedigger, 1 },
        { TowerType.ObsidianDestroyer, 1 },
        { TowerType.DecayedHorror, 1 },
        { TowerType.UltimateDiabolist, 1 },
        { TowerType.TechnologyDisc, 1 },
        { TowerType.TechnologyDisc_Arcane, 1 },
        { TowerType.TechnologyDisc_Fire, 1 },
        { TowerType.TechnologyDisc_Ice, 1 },
        { TowerType.TechnologyDisc_Earth, 1 },
        { TowerType.TechnologyDisc_Lightning, 1 },
        { TowerType.TechnologyDisc_Holy, 1 },
        { TowerType.TechnologyDisc_Water, 1 },
        { TowerType.TechnologyDisc_Unholy, 1 },
        { TowerType.TechnologyDisc_Void, 1 },
        { TowerType.TechnologyDisc_Arcane_Ultimate, 1 },
        { TowerType.TechnologyDisc_Fire_Ultimate, 1 },
        { TowerType.TechnologyDisc_Ice_Ultimate, 1 },
        { TowerType.TechnologyDisc_Earth_Ultimate, 1 },
        { TowerType.TechnologyDisc_Lightning_Ultimate, 1 },
        { TowerType.TechnologyDisc_Holy_Ultimate, 1 },
        { TowerType.TechnologyDisc_Water_Ultimate, 1 },
        { TowerType.TechnologyDisc_Unholy_Ultimate, 1 },
        { TowerType.TechnologyDisc_Void_Ultimate, 1 },
    };
    
    // 0 means the unit does not have mana
    public static readonly Dictionary<TowerType, int> MP = new Dictionary<TowerType, int>() {
        { TowerType.Archer, 0 },
        { TowerType.Gunner, 0 },
        { TowerType.WatchTower, 0 },
        { TowerType.GuardTower, 0 },
        { TowerType.WardTower, 0 },
        { TowerType.UltimateWardTower, 0 },
        { TowerType.CannonTower, 0 },
        { TowerType.BombardTower, 0 },
        { TowerType.ArtilleryTower, 0 },
        { TowerType.UltimateArtilleryTower, 0 },
        { TowerType.Cutter, 0 },
        { TowerType.Grinder, 0 },
        { TowerType.Carver, 0 },
        { TowerType.Executioner, 0 },
        { TowerType.Mauler, 0 },
        { TowerType.UltimateMauler, 0 },
        { TowerType.Crusher, 0 },
        { TowerType.Wrecker, 0 },
        { TowerType.Mangler, 0 },
        { TowerType.UltimateMangler, 0 },
        { TowerType.ElementalCore, 0 },
        { TowerType.Spellslinger, 50 },
        { TowerType.SpellslingerMaster, 50 },
        { TowerType.ArcanePylon, 100 },
        { TowerType.ArcaneRepository, 100 },
        { TowerType.UltimateArcaneOrb, 250 },
        { TowerType.Archmage, 30 },
        { TowerType.GrandArchmage, 30 },
        { TowerType.UltimateKirinTorWizard, 30 },
        { TowerType.FirePit, 0 },
        { TowerType.MagmaWell, 0 },
        { TowerType.LivingFlame, 0 },
        { TowerType.Hellfire, 0 },
        { TowerType.UltimateFirelord, 0 },
        { TowerType.MeteorAttractor, 0 },
        { TowerType.Armageddon, 0 },
        { TowerType.UltimateMoonbeamProjector, 25 },
        { TowerType.Rockfall, 0 },
        { TowerType.Avalanche, 0 },
        { TowerType.EarthGuardian, 0 },
        { TowerType.AncientProtector, 0 },
        { TowerType.UltimateAncientWarden, 0 },
        { TowerType.NoxiousWeed, 0 },
        { TowerType.NoxiousThorn, 0 },
        { TowerType.UltimateScorpion, 0 },
        { TowerType.Voidling, 30 },
        { TowerType.Voidalisk, 30 },
        { TowerType.Riftweaver, 50 },
        { TowerType.RiftLord, 50 },
        { TowerType.UltimateDevourer, 450 },
        { TowerType.Lasher, 0 },
        { TowerType.Ravager, 0 },
        { TowerType.UltimateLeviathan, 0 },
        { TowerType.Splasher, 0 },
        { TowerType.Tidecaller, 0 },
        { TowerType.WaterElemental, 0 },
        { TowerType.SeaElemental, 0 },
        { TowerType.UltimateHurricaneElemental, 0 },
        { TowerType.TideLurker, 0 },
        { TowerType.AbyssStalker, 0 },
        { TowerType.UltimateSludgeMonstrosity, 0 },
        { TowerType.ShockParticle, 0 },
        { TowerType.ShockGenerator, 0 },
        { TowerType.Voltage, 30 },
        { TowerType.HighVoltage, 30 },
        { TowerType.UltimateOrbKeeper, 50 },
        { TowerType.LightningBeacon, 0 },
        { TowerType.LightningGenerator, 0 },
        { TowerType.UltimateAnnihilationGlyph, 0 },
        { TowerType.LightFlies, 0 },
        { TowerType.HolyLantern, 0 },
        { TowerType.SunrayTower, 0 },
        { TowerType.SunbeamTower, 0 },
        { TowerType.UltimateTitanVault, 0 },
        { TowerType.Glowshroom, 100 },
        { TowerType.Lightshroom, 100 },
        { TowerType.UltimateDivineshroom, 100 },
        { TowerType.FrozenObelisk, 0 },
        { TowerType.RunicObelisk, 0 },
        { TowerType.FrozenWatcher, 0 },
        { TowerType.FrozenCore, 0 },
        { TowerType.UltimateLich, 0 },
        { TowerType.Icicle, 0 },
        { TowerType.Tricicle, 0 },
        { TowerType.UltimateCrystal, 0 },
        { TowerType.PlagueWell, 0 },
        { TowerType.Catacomb, 0 },
        { TowerType.SepticTank, 2 },
        { TowerType.PlagueFanatic, 3 },
        { TowerType.UltimateGravedigger, 5 },
        { TowerType.ObsidianDestroyer, 100 },
        { TowerType.DecayedHorror, 150 },
        { TowerType.UltimateDiabolist, 150 },
        { TowerType.TechnologyDisc, 0 },
        { TowerType.TechnologyDisc_Arcane, 0 },
        { TowerType.TechnologyDisc_Fire, 0 },
        { TowerType.TechnologyDisc_Ice, 0 },
        { TowerType.TechnologyDisc_Earth, 0 },
        { TowerType.TechnologyDisc_Lightning, 0 },
        { TowerType.TechnologyDisc_Holy, 0 },
        { TowerType.TechnologyDisc_Water, 0 },
        { TowerType.TechnologyDisc_Unholy, 0 },
        { TowerType.TechnologyDisc_Void, 0 },
        { TowerType.TechnologyDisc_Arcane_Ultimate, 0 },
        { TowerType.TechnologyDisc_Fire_Ultimate, 0 },
        { TowerType.TechnologyDisc_Ice_Ultimate, 0 },
        { TowerType.TechnologyDisc_Earth_Ultimate, 0 },
        { TowerType.TechnologyDisc_Lightning_Ultimate, 0 },
        { TowerType.TechnologyDisc_Holy_Ultimate, 0 },
        { TowerType.TechnologyDisc_Water_Ultimate, 0 },
        { TowerType.TechnologyDisc_Unholy_Ultimate, 0 },
        { TowerType.TechnologyDisc_Void_Ultimate, 0 },
    };

    public static readonly Dictionary<TowerType, (double, double)> DamageRange = new Dictionary<TowerType, (double, double)>() {
        { TowerType.Archer, (1,1) },
        { TowerType.Gunner, (3,3)},
        { TowerType.WatchTower, (8,9) },
        { TowerType.GuardTower, (37,38) },
        { TowerType.WardTower, (143,144) },
        { TowerType.UltimateWardTower, (538,539) },
        { TowerType.CannonTower, (14,16) },
        { TowerType.BombardTower, (70,74) },
        { TowerType.ArtilleryTower, (285,289) },
        { TowerType.UltimateArtilleryTower, (1071,1077) },
        { TowerType.Cutter, (1,1) },
        { TowerType.Grinder, (3,3) },
        { TowerType.Carver, (13,15) },
        { TowerType.Executioner, (69,75) },
        { TowerType.Mauler, (291,299) },
        { TowerType.UltimateMauler, (1180,1191) },
        { TowerType.Crusher, (24,27) },
        { TowerType.Wrecker, (112,117) },
        { TowerType.Mangler, (494,501) },
        { TowerType.UltimateMangler, (1996,2004) },
        { TowerType.ElementalCore, (6,7) },
        { TowerType.Spellslinger, (4,4) },
        { TowerType.SpellslingerMaster, (10,11) },
        { TowerType.ArcanePylon, (39,41) },
        { TowerType.ArcaneRepository, (123,125) },
        { TowerType.UltimateArcaneOrb, (180,182) },
        { TowerType.Archmage, (45,48) },
        { TowerType.GrandArchmage, (196,198) },
        { TowerType.UltimateKirinTorWizard, (812,815) },
        { TowerType.FirePit, (20,22) },
        { TowerType.MagmaWell, (70,75) },
        { TowerType.LivingFlame, (81,85) },
        { TowerType.Hellfire, (153,157) },
        { TowerType.UltimateFirelord, (399,403) },
        { TowerType.MeteorAttractor, (185,189) },
        { TowerType.Armageddon, (408,412) },
        { TowerType.UltimateMoonbeamProjector, (1252,1255) },
        { TowerType.Rockfall, (16,20) },
        { TowerType.Avalanche, (75,87) },
        { TowerType.EarthGuardian, (218,236) },
        { TowerType.AncientProtector, (511,530) },
        { TowerType.UltimateAncientWarden, (1475,1542) },
        { TowerType.NoxiousWeed, (131,141) },
        { TowerType.NoxiousThorn, (344,352) },
        { TowerType.UltimateScorpion, (905,932) },
        { TowerType.Voidling, (40,44) },
        { TowerType.Voidalisk, (171,175) },
        { TowerType.Riftweaver, (265,266) },
        { TowerType.RiftLord, (812,813) },
        { TowerType.UltimateDevourer, (2248,2254) },
        { TowerType.Lasher, (84,86) },
        { TowerType.Ravager, (217,219) },
        { TowerType.UltimateLeviathan, (414,416) },
        { TowerType.Splasher, (6,7) },
        { TowerType.Tidecaller, (20,22) },
        { TowerType.WaterElemental, (80,82) },
        { TowerType.SeaElemental, (186,188) },
        { TowerType.UltimateHurricaneElemental, (536,538) },
        { TowerType.TideLurker, (330,333) },
        { TowerType.AbyssStalker, (803,806) },
        { TowerType.UltimateSludgeMonstrosity, (2385,2435) },
        { TowerType.ShockParticle, (20,20) },
        { TowerType.ShockGenerator, (75,75) },
        { TowerType.Voltage, (130,130) },
        { TowerType.HighVoltage, (350,350) },
        { TowerType.UltimateOrbKeeper, (560,560) },
        { TowerType.LightningBeacon, (348,362) },
        { TowerType.LightningGenerator, (810,824) },
        { TowerType.UltimateAnnihilationGlyph, (1964,1976) },
        { TowerType.LightFlies, (12,13) },
        { TowerType.HolyLantern, (30,32) },
        { TowerType.SunrayTower, (85,86) },
        { TowerType.SunbeamTower, (215,216) },
        { TowerType.UltimateTitanVault, (624,648) },
        { TowerType.Glowshroom, (90,91) },
        { TowerType.Lightshroom, (402,403) },
        { TowerType.UltimateDivineshroom, (1042,1043) },
        { TowerType.FrozenObelisk, (11,14) },
        { TowerType.RunicObelisk, (39,43) },
        { TowerType.FrozenWatcher, (104,108) },
        { TowerType.FrozenCore, (307,311) },
        { TowerType.UltimateLich, (581,585) },
        { TowerType.Icicle, (102,104) },
        { TowerType.Tricicle, (257,259) },
        { TowerType.UltimateCrystal, (815,820) },
        { TowerType.PlagueWell, (15,16) },
        { TowerType.Catacomb, (47,48) },
        { TowerType.SepticTank, (115,119) },
        { TowerType.PlagueFanatic, (323,327) },
        { TowerType.UltimateGravedigger, (1259,1261) },
        { TowerType.ObsidianDestroyer, (80,85) },
        { TowerType.DecayedHorror, (230,235) },
        { TowerType.UltimateDiabolist, (534,539) },
        { TowerType.TechnologyDisc, (0,0) },
        { TowerType.TechnologyDisc_Arcane, (0,0) },
        { TowerType.TechnologyDisc_Fire, (0,0) },
        { TowerType.TechnologyDisc_Ice, (0,0) },
        { TowerType.TechnologyDisc_Earth, (0,0) },
        { TowerType.TechnologyDisc_Lightning, (0,0) },
        { TowerType.TechnologyDisc_Holy, (0,0) },
        { TowerType.TechnologyDisc_Water, (0,0) },
        { TowerType.TechnologyDisc_Unholy, (0,0) },
        { TowerType.TechnologyDisc_Void, (0,0) },
        { TowerType.TechnologyDisc_Arcane_Ultimate, (0,0) },
        { TowerType.TechnologyDisc_Fire_Ultimate, (0,0) },
        { TowerType.TechnologyDisc_Ice_Ultimate, (0,0) },
        { TowerType.TechnologyDisc_Earth_Ultimate, (0,0) },
        { TowerType.TechnologyDisc_Lightning_Ultimate, (0,0) },
        { TowerType.TechnologyDisc_Holy_Ultimate, (0,0) },
        { TowerType.TechnologyDisc_Water_Ultimate, (0,0) },
        { TowerType.TechnologyDisc_Unholy_Ultimate, (0,0) },
        { TowerType.TechnologyDisc_Void_Ultimate, (0,0) },
    };

    public static readonly Dictionary<TowerType, double> SplashDamageRadius = new Dictionary<TowerType, double>() {
        { TowerType.CannonTower, 117 },
        { TowerType.BombardTower, 155 },
        { TowerType.ArtilleryTower, 195 },
        { TowerType.UltimateArtilleryTower, 235 },
        { TowerType.Crusher, 215 },
        { TowerType.Wrecker, 275 },
        { TowerType.Mangler, 332 },
        { TowerType.UltimateMangler, 390 },
        { TowerType.FirePit, 117 },
        { TowerType.MagmaWell, 155 },
        { TowerType.LivingFlame, 60 },
        { TowerType.Hellfire, 80 },
        { TowerType.UltimateFirelord, 117 },
        { TowerType.MeteorAttractor, 235 },
        { TowerType.Armageddon, 313 },
        { TowerType.UltimateMoonbeamProjector, 390 },
        { TowerType.Rockfall, 195 },
        { TowerType.Avalanche, 195 },
        { TowerType.EarthGuardian, 235 },
        { TowerType.AncientProtector, 275 },
        { TowerType.UltimateAncientWarden, 313 },
        { TowerType.Lasher, 117 },
        { TowerType.Ravager, 156 },
        { TowerType.UltimateLeviathan, 195 },
        { TowerType.Splasher, 60 },
        { TowerType.Tidecaller, 60 },
        { TowerType.WaterElemental, 80 },
        { TowerType.SeaElemental, 100 },
        { TowerType.UltimateHurricaneElemental, 120 },
        { TowerType.Glowshroom, 235 },
        { TowerType.Lightshroom, 235 },
        { TowerType.UltimateDivineshroom, 235 },
        { TowerType.FrozenObelisk, 40 },
        { TowerType.RunicObelisk, 40 },
        { TowerType.FrozenWatcher, 60 },
        { TowerType.FrozenCore, 80 },
        { TowerType.UltimateLich, 120 },
        { TowerType.ObsidianDestroyer, 80 },
        { TowerType.DecayedHorror, 155 },
        { TowerType.UltimateDiabolist, 155 },
    };

    public static readonly Dictionary<TowerType, AttackType> AttackModifier = new Dictionary<TowerType, AttackType>() {
        { TowerType.Archer, AttackType.Piercing },
        { TowerType.Gunner, AttackType.Piercing },
        { TowerType.WatchTower, AttackType.Piercing },
        { TowerType.GuardTower, AttackType.Piercing },
        { TowerType.WardTower, AttackType.Piercing },
        { TowerType.UltimateWardTower, AttackType.Piercing },
        { TowerType.CannonTower, AttackType.Siege },
        { TowerType.BombardTower, AttackType.Siege },
        { TowerType.ArtilleryTower, AttackType.Siege },
        { TowerType.UltimateArtilleryTower, AttackType.Siege },
        { TowerType.Cutter, AttackType.Normal },
        { TowerType.Grinder, AttackType.Normal },
        { TowerType.Carver, AttackType.Normal },
        { TowerType.Executioner, AttackType.Normal },
        { TowerType.Mauler, AttackType.Normal },
        { TowerType.UltimateMauler, AttackType.Normal },
        { TowerType.Crusher, AttackType.Normal },
        { TowerType.Wrecker, AttackType.Normal },
        { TowerType.Mangler, AttackType.Normal },
        { TowerType.UltimateMangler, AttackType.Normal },
        { TowerType.ElementalCore, AttackType.Flux },
        { TowerType.Spellslinger, AttackType.Flux },
        { TowerType.SpellslingerMaster, AttackType.Flux },
        { TowerType.ArcanePylon, AttackType.Flux },
        { TowerType.ArcaneRepository, AttackType.Flux },
        { TowerType.UltimateArcaneOrb, AttackType.Flux },
        { TowerType.Archmage, AttackType.Flux },
        { TowerType.GrandArchmage, AttackType.Flux },
        { TowerType.UltimateKirinTorWizard, AttackType.Flux },
        { TowerType.FirePit, AttackType.Normal },
        { TowerType.MagmaWell, AttackType.Normal },
        { TowerType.LivingFlame, AttackType.Chaos },
        { TowerType.Hellfire, AttackType.Chaos },
        { TowerType.UltimateFirelord, AttackType.Chaos },
        { TowerType.MeteorAttractor, AttackType.Siege },
        { TowerType.Armageddon, AttackType.Siege },
        { TowerType.UltimateMoonbeamProjector, AttackType.Siege },
        { TowerType.Rockfall, AttackType.Siege },
        { TowerType.Avalanche, AttackType.Siege },
        { TowerType.EarthGuardian, AttackType.Siege },
        { TowerType.AncientProtector, AttackType.Siege },
        { TowerType.UltimateAncientWarden, AttackType.Siege },
        { TowerType.NoxiousWeed, AttackType.Piercing },
        { TowerType.NoxiousThorn, AttackType.Piercing },
        { TowerType.UltimateScorpion, AttackType.Piercing },
        { TowerType.Voidling, AttackType.Chaos },
        { TowerType.Voidalisk, AttackType.Chaos },
        { TowerType.Riftweaver, AttackType.Flux },
        { TowerType.RiftLord, AttackType.Flux },
        { TowerType.UltimateDevourer, AttackType.Flux },
        { TowerType.Lasher, AttackType.Chaos },
        { TowerType.Ravager, AttackType.Chaos },
        { TowerType.UltimateLeviathan, AttackType.Chaos },
        { TowerType.Splasher, AttackType.Normal },
        { TowerType.Tidecaller, AttackType.Normal },
        { TowerType.WaterElemental, AttackType.Normal },
        { TowerType.SeaElemental, AttackType.Normal },
        { TowerType.UltimateHurricaneElemental, AttackType.Normal },
        { TowerType.TideLurker, AttackType.Normal },
        { TowerType.AbyssStalker, AttackType.Normal },
        { TowerType.UltimateSludgeMonstrosity, AttackType.Normal },
        { TowerType.ShockParticle, AttackType.Chaos },
        { TowerType.ShockGenerator, AttackType.Chaos },
        { TowerType.Voltage, AttackType.Chaos },
        { TowerType.HighVoltage, AttackType.Chaos },
        { TowerType.UltimateOrbKeeper, AttackType.Chaos },
        { TowerType.LightningBeacon, AttackType.Chaos },
        { TowerType.LightningGenerator, AttackType.Chaos },
        { TowerType.UltimateAnnihilationGlyph, AttackType.Chaos },
        { TowerType.LightFlies, AttackType.Piercing },
        { TowerType.HolyLantern, AttackType.Piercing },
        { TowerType.SunrayTower, AttackType.Normal },
        { TowerType.SunbeamTower, AttackType.Normal },
        { TowerType.UltimateTitanVault, AttackType.Normal },
        { TowerType.Glowshroom, AttackType.Piercing },
        { TowerType.Lightshroom, AttackType.Piercing },
        { TowerType.UltimateDivineshroom, AttackType.Piercing },
        { TowerType.FrozenObelisk, AttackType.Flux },
        { TowerType.RunicObelisk, AttackType.Flux },
        { TowerType.FrozenWatcher, AttackType.Flux },
        { TowerType.FrozenCore, AttackType.Flux },
        { TowerType.UltimateLich, AttackType.Flux },
        { TowerType.Icicle, AttackType.Piercing },
        { TowerType.Tricicle, AttackType.Piercing },
        { TowerType.UltimateCrystal, AttackType.Piercing },
        { TowerType.PlagueWell, AttackType.Chaos },
        { TowerType.Catacomb, AttackType.Chaos },
        { TowerType.SepticTank, AttackType.Chaos },
        { TowerType.PlagueFanatic, AttackType.Chaos },
        { TowerType.UltimateGravedigger, AttackType.Chaos },
        { TowerType.ObsidianDestroyer, AttackType.Chaos },
        { TowerType.DecayedHorror, AttackType.Chaos },
        { TowerType.UltimateDiabolist, AttackType.Chaos },
        { TowerType.TechnologyDisc, AttackType.None },
        { TowerType.TechnologyDisc_Arcane, AttackType.None },
        { TowerType.TechnologyDisc_Fire, AttackType.None },
        { TowerType.TechnologyDisc_Ice, AttackType.None },
        { TowerType.TechnologyDisc_Earth, AttackType.None },
        { TowerType.TechnologyDisc_Lightning, AttackType.None },
        { TowerType.TechnologyDisc_Holy, AttackType.None },
        { TowerType.TechnologyDisc_Water, AttackType.None },
        { TowerType.TechnologyDisc_Unholy, AttackType.None },
        { TowerType.TechnologyDisc_Void, AttackType.None },
        { TowerType.TechnologyDisc_Arcane_Ultimate, AttackType.None },
        { TowerType.TechnologyDisc_Fire_Ultimate, AttackType.None },
        { TowerType.TechnologyDisc_Ice_Ultimate, AttackType.None },
        { TowerType.TechnologyDisc_Earth_Ultimate, AttackType.None },
        { TowerType.TechnologyDisc_Lightning_Ultimate, AttackType.None },
        { TowerType.TechnologyDisc_Holy_Ultimate, AttackType.None },
        { TowerType.TechnologyDisc_Water_Ultimate, AttackType.None },
        { TowerType.TechnologyDisc_Unholy_Ultimate, AttackType.None },
        { TowerType.TechnologyDisc_Void_Ultimate, AttackType.None },
    };

    public static readonly Dictionary<TowerType, AttackDeliveryModifiers> AttackDelivery = new Dictionary<TowerType, AttackDeliveryModifiers>() {
        { TowerType.Archer,
            new ProjectileFromSourceAttackDelivery {
                InitialAnimationDelay = .5,
                ProjectileSpeed = 60,
                TrackTarget = true,
                ProjectileInitialOffset = Vector3.zero,
                MaxDistance = 200,
                MaxSeconds = 2
            }
        },
        { TowerType.Gunner,
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.WatchTower,
            new ProjectileFromSourceAttackDelivery {
                InitialAnimationDelay = .3,
                ProjectileSpeed = 60,
                TrackTarget = true,
                ProjectileInitialOffset = Vector3.zero,
                MaxDistance = 200,
                MaxSeconds = 2
            }
        },
        { TowerType.GuardTower,
            new ProjectileFromSourceAttackDelivery {
                InitialAnimationDelay = .3,
                ProjectileSpeed = 80,
                TrackTarget = true,
                ProjectileInitialOffset = Vector3.zero,
                MaxDistance = 200,
                MaxSeconds = 2
            }
        },
        { TowerType.WardTower,
            new ProjectileFromSourceAttackDelivery {
                InitialAnimationDelay = .3,
                ProjectileSpeed = 100,
                TrackTarget = true,
                ProjectileInitialOffset = Vector3.zero,
                MaxDistance = 200,
                MaxSeconds = 2
            }
        },
        { TowerType.UltimateWardTower,
            new ProjectileFromSourceAttackDelivery {
                InitialAnimationDelay = .3,
                ProjectileSpeed = 120,
                TrackTarget = true,
                ProjectileInitialOffset = Vector3.zero,
                MaxDistance = 200,
                MaxSeconds = 2
            }
        },
        { TowerType.CannonTower,
            new ProjectileFromSourceAttackDelivery {
                InitialAnimationDelay = .8,
                ProjectileSpeed = 50,
                TrackTarget = true,
                ProjectileInitialOffset = Vector3.zero,
                MaxDistance = 200,
                MaxSeconds = 2.5
            }
        },
        { TowerType.BombardTower,
            new ProjectileFromSourceAttackDelivery {
                InitialAnimationDelay = .66,
                ProjectileSpeed = 60,
                TrackTarget = true,
                ProjectileInitialOffset = Vector3.zero,
                MaxDistance = 200,
                MaxSeconds = 2.5
            }
        },
        { TowerType.ArtilleryTower,
            new ProjectileFromSourceAttackDelivery {
                InitialAnimationDelay = .53,
                ProjectileSpeed = 70,
                TrackTarget = true,
                ProjectileInitialOffset = Vector3.zero,
                MaxDistance = 200,
                MaxSeconds = 2.5
            }
        },
        { TowerType.UltimateArtilleryTower,
            new ProjectileFromSourceAttackDelivery {
                InitialAnimationDelay = .4,
                ProjectileSpeed = 80,
                TrackTarget = true,
                ProjectileInitialOffset = Vector3.zero,
                MaxDistance = 200,
                MaxSeconds = 2.5
            }
        },
        { TowerType.Cutter,
            new InstantAttackDelivery {
                InitialAnimationDelay = 0,
            }
        },
        { TowerType.Grinder,
            new InstantAttackDelivery {
                InitialAnimationDelay = 0,
            }
        },
        { TowerType.Carver,
            new InstantAttackDelivery {
                InitialAnimationDelay = 0,
            }
        },
        { TowerType.Executioner,
            new InstantAttackDelivery {
                InitialAnimationDelay = 0,
            }
        },
        { TowerType.Mauler,
            new InstantAttackDelivery {
                InitialAnimationDelay = 0,
            }
        },
        { TowerType.UltimateMauler,
            new InstantAttackDelivery {
                InitialAnimationDelay = 0,
            }
        },
        { TowerType.Crusher,
            new InstantAttackDelivery {
                InitialAnimationDelay = .5,
            }
        },
        { TowerType.Wrecker, 
            new InstantAttackDelivery {
                InitialAnimationDelay = .5,
            }
        },
        { TowerType.Mangler,
            new InstantAttackDelivery {
                InitialAnimationDelay = .5,
            }
        },
        { TowerType.UltimateMangler,
            new InstantAttackDelivery {
                InitialAnimationDelay = .5,
            }
        },
        { TowerType.ElementalCore,
            new ProjectileFromSourceAttackDelivery {
                InitialAnimationDelay = 0.1,
                ProjectileSpeed = 25,
                TrackTarget = true,
                ProjectileInitialOffset = Vector3.zero,
                MaxDistance = 200,
                MaxSeconds = 5
            }
        },
        { TowerType.Spellslinger,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.SpellslingerMaster,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.ArcanePylon,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.ArcaneRepository,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateArcaneOrb,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Archmage,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.GrandArchmage,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateKirinTorWizard,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.FirePit,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.MagmaWell,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.LivingFlame,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Hellfire,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateFirelord,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.MeteorAttractor,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Armageddon,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateMoonbeamProjector,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Rockfall,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Avalanche,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.EarthGuardian,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.AncientProtector,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateAncientWarden,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.NoxiousWeed,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.NoxiousThorn,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateScorpion,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Voidling,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Voidalisk,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Riftweaver,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.RiftLord,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateDevourer,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Lasher,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Ravager,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateLeviathan,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Splasher,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Tidecaller,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.WaterElemental,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.SeaElemental,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateHurricaneElemental,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.TideLurker,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.AbyssStalker,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateSludgeMonstrosity,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.ShockParticle,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.ShockGenerator,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Voltage,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.HighVoltage,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateOrbKeeper,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.LightningBeacon,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.LightningGenerator,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateAnnihilationGlyph,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.LightFlies,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.HolyLantern,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.SunrayTower,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.SunbeamTower,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateTitanVault,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Glowshroom, 
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Lightshroom,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateDivineshroom,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.FrozenObelisk,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.RunicObelisk,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.FrozenWatcher,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.FrozenCore,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateLich,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Icicle,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Tricicle,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateCrystal,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.PlagueWell,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.Catacomb,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.SepticTank,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.PlagueFanatic,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateGravedigger,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.ObsidianDestroyer,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.DecayedHorror,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.UltimateDiabolist,
            // Placeholder
            new InstantAttackDelivery {
                InitialAnimationDelay = 0
            }
        },
        { TowerType.TechnologyDisc,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Arcane,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Fire,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Ice,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Earth,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Lightning,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Holy,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Water,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Unholy,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Void,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Arcane_Ultimate,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Fire_Ultimate,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Ice_Ultimate,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Earth_Ultimate,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Lightning_Ultimate,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Holy_Ultimate,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Water_Ultimate,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Unholy_Ultimate,
            new NoAttackDelivery()
        },
        { TowerType.TechnologyDisc_Void_Ultimate,
            new NoAttackDelivery()
        },
    };
}
