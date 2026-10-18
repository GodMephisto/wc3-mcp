---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/ClientPrefabs.cs"
ref: HEAD
---

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

public class ClientPrefabs : SingletonBehaviour<ClientPrefabs>
{
    public static Dictionary<EnemyType, ClientEnemy> Enemies { get; private set; }
    public static Dictionary<TowerType, ClientTower> Towers { get; private set; }

    // UI
    [SerializeField] public ChatMessageText pfChatMessageText;

    [SerializeField] public ScreenMessage pfScreenMessage;
    
    [SerializeField] public ScoreboardRow pfScoreboardRow;
    [SerializeField] public EntityImage pfMultipleTargetEntityImage;
    [SerializeField] public TowerUpgradeOption pfTowerUpgradeOption;

    [SerializeField] public SelectionSpriteIndicator pfHoveringSpritePrefab;
    [SerializeField] public SelectionSpriteIndicator pfSelectedSpritePrefab;

    [SerializeField] public FloatingBountyText pfFloatingBountyText;

    [SerializeField] public GridCellOutline pfGridCellOutline;
    [SerializeField] public GridRowNumber pfGridRowNumber;

    [SerializeField] public BuffDisplay pfBuffDisplay;

    [SerializeField] public EFEventDisplay_LivesExchanged pfEventDisplayLivesExchanged;

    [SerializeField] public Color pfNegativeFlashColor;
    [SerializeField] public Color pfPositiveFlashColor;
    
    [Space(10)]
    [Header("Enemies")]
    [SerializeField] private ClientSheep pfSheep;
    [SerializeField] private ClientWolf pfWolf;
    [SerializeField] private ClientSkeleton pfSkeleton;
    [SerializeField] private ClientAcolyte pfAcolyte;
    [SerializeField] private ClientSpider pfSpider;
    [SerializeField] private ClientSwordsman pfSwordsman;
    [SerializeField] private ClientGrunt pfGrunt;
    [SerializeField] private ClientTemptress pfTemptress;
    [SerializeField] private ClientShade pfShade;
    [SerializeField] private ClientMudGolem pfMudGolem;
    [SerializeField] private ClientTreant pfTreant;
    [SerializeField] private ClientRotGolem pfRotGolem;
    [SerializeField] private ClientKnight pfKnight;
    [SerializeField] private ClientVengefulSpirit pfVengefulSpirit;
    [SerializeField] private ClientForestTroll pfForestTroll;
    [SerializeField] private ClientWyvern pfWyvern;
    [SerializeField] private ClientVoidwalker pfVoidwalker;
    [SerializeField] private ClientFacelessOne pfFacelessOne;
    [SerializeField] private ClientDragonspawn pfDragonspawn;
    [SerializeField] private ClientSeaTurtle pfSeaTurtle;
    [SerializeField] private ClientBanshee pfBanshee;
    [SerializeField] private ClientSiegeEngine pfSiegeEngine;
    [SerializeField] private ClientKoboldGeomancer pfKoboldGeomancer;
    [SerializeField] private ClientInfernal pfInfernal;
    [SerializeField] private ClientDeathRevenant pfDeathRevenant;
    [SerializeField] private ClientSatyrShadowdancer pfSatyrShadowdancer;
    [SerializeField] private ClientCryptFiend pfCryptFiend;
    [SerializeField] private ClientSpiritWalker pfSpiritWalker;
    [SerializeField] private ClientNecromancer pfNecromancer;
    [SerializeField] private ClientAncientWendigo pfAncientWendigo;
    [SerializeField] private ClientGryphonRider pfGryphonRider;
    [SerializeField] private ClientOgreMagi pfOgreMagi;
    [SerializeField] private ClientAbomination pfAbomination;
    [SerializeField] private ClientChaosWarden pfChaosWarden;
    [SerializeField] private ClientMountainGiant pfMountainGiant;
    [SerializeField] private ClientGoblinShredder pfGoblinShredder;
    [SerializeField] private ClientKodoRider pfKodoRider;
    [SerializeField] private ClientFrostWyrm pfFrostWyrm;
    [SerializeField] private ClientPhoenix pfPhoenix;

    // Towers
    [Space(4)]
    [Header("Towers")]
    [Space(5)]
    [Header("Archer Base")]
    [SerializeField] private ClientArcher pfArcher;
    [SerializeField] private ClientGunner pfGunner;
    [SerializeField] private ClientWatchTower pfWatchTower;
    [SerializeField] private ClientGuardTower pfGuardTower;
    [SerializeField] private ClientWardTower pfWardTower;
    [SerializeField] private ClientUltimateWardTower pfUltimateWardTower;
    [SerializeField] private ClientCannonTower pfCannonTower;
    [SerializeField] private ClientBombardTower pfBombardTower;
    [SerializeField] private ClientArtilleryTower pfArtilleryTower;
    [SerializeField] private ClientUltimateArtilleryTower pfUltimateArtilleryTower;

    [Space(5)]
    [Header("Cutter Base")]
    [SerializeField] private ClientCutter pfCutter;
    [SerializeField] private ClientGrinder pfGrinder;
    [SerializeField] private ClientCarver pfCarver;
    [SerializeField] private ClientExecutioner pfExecutioner;
    [SerializeField] private ClientMauler pfMauler;
    [SerializeField] private ClientUltimateMauler pfUltimateMauler;
    [SerializeField] private ClientCrusher pfCrusher;
    [SerializeField] private ClientWrecker pfWrecker;
    [SerializeField] private ClientMangler pfMangler;
    [SerializeField] private ClientUltimateMangler pfUltimateMangler;

    [Space(5)]
    [Header("Elemental Core Base")]
    [SerializeField] private ClientElementalCore pfElementalCore;
    
    [Space(3)]
    [Header("Arcane")]
    [SerializeField] private ClientSpellslinger pfSpellslinger;
    [SerializeField] private ClientSpellslingerMaster pfSpellslingerMaster;
    [SerializeField] private ClientArcanePylon pfArcanePylon;
    [SerializeField] private ClientArcaneRepository pfArcaneRepository;
    [SerializeField] private ClientArchmage pfArchmage;
    [SerializeField] private ClientGrandArchmage pfGrandArchmage;
    [SerializeField] private ClientUltimateKirinTorWizard pfUltimateKirinTorWizard;
    [SerializeField] private ClientUltimateArcaneOrb pfUltimateArcaneOrb;
    
    [Space(3)]
    [Header("Earth")]
    [SerializeField] private ClientRockfall pfRockfall;
    [SerializeField] private ClientAvalanche pfAvalanche;
    [SerializeField] private ClientEarthGuardian pfEarthGuardian;
    [SerializeField] private ClientAncientProtector pfAncientProtector;
    [SerializeField] private ClientUltimateAncientWarden pfUltimateAncientWarden;
    [SerializeField] private ClientNoxiousWeed pfNoxiousWeed;
    [SerializeField] private ClientNoxiousThorn pfNoxiousThorn;
    [SerializeField] private ClientUltimateScorpion pfUltimateScorpion;
    
    [Space(3)]
    [Header("Fire")]
    [SerializeField] private ClientFirePit pfFirePit;
    [SerializeField] private ClientMagmaWell pfMagmaWell;
    [SerializeField] private ClientLivingFlame pfLivingFlame;
    [SerializeField] private ClientHellfire pfHellfire;
    [SerializeField] private ClientUltimateFirelord pfUltimateFirelord;
    [SerializeField] private ClientMeteorAttractor pfMeteorAttractor;
    [SerializeField] private ClientArmageddon pfArmageddon;
    [SerializeField] private ClientUltimateMoonbeamProjector pfUltimateMoonbeamProjector;
    
    [Space(3)]
    [Header("Holy")]
    [SerializeField] private ClientLightFlies pfLightFlies;
    [SerializeField] private ClientHolyLantern pfHolyLantern;
    [SerializeField] private ClientSunrayTower pfSunrayTower;
    [SerializeField] private ClientSunbeamTower pfSunbeamTower;
    [SerializeField] private ClientUltimateTitanVault pfUltimateTitanVault;
    [SerializeField] private ClientGlowshroom pfGlowshroom;
    [SerializeField] private ClientLightshroom pfLightshroom;
    [SerializeField] private ClientUltimateDivineshroom pfUltimateDivineshroom;
    
    [Space(3)]
    [Header("Ice")]
    [SerializeField] private ClientFrozenObelisk pfFrozenObelisk;
    [SerializeField] private ClientRunicObelisk pfRunicObelisk;
    [SerializeField] private ClientFrozenWatcher pfFrozenWatcher;
    [SerializeField] private ClientFrozenCore pfFrozenCore;
    [SerializeField] private ClientUltimateLich pfUltimateLich;
    [SerializeField] private ClientIcicle pfIcicle;
    [SerializeField] private ClientTricicle pfTricicle;
    [SerializeField] private ClientUltimateCrystal pfUltimateCrystal;

    [Space(3)]
    [Header("Lightning")]
    [SerializeField] private ClientShockParticle pfShockParticle;
    [SerializeField] private ClientShockGenerator pfShockGenerator;
    [SerializeField] private ClientVoltage pfVoltage;
    [SerializeField] private ClientHighVoltage pfHighVoltage;
    [SerializeField] private ClientUltimateOrbKeeper pfUltimateOrbKeeper;
    [SerializeField] private ClientLightningBeacon pfLightningBeacon;
    [SerializeField] private ClientLightningGenerator pfLightningGenerator;
    [SerializeField] private ClientUltimateAnnihilationGlyph pfUltimateAnnihilationGlyph;
    
    [Space(3)]
    [Header("Unholy")]
    [SerializeField] private ClientPlagueWell pfPlagueWell;
    [SerializeField] private ClientCatacomb pfCatacomb;
    [SerializeField] private ClientSepticTank pfSepticTank;
    [SerializeField] private ClientPlagueFanatic pfPlagueFanatic;
    [SerializeField] private ClientUltimateGravedigger pfUltimateGravedigger;
    [SerializeField] private ClientObsidianDestroyer pfObsidianDestroyer;
    [SerializeField] private ClientDecayedHorror pfDecayedHorror;
    [SerializeField] private ClientUltimateDiabolist pfUltimateDiabolist;
    
    [Space(3)]
    [Header("Void")]
    [SerializeField] private ClientVoidling pfVoidling;
    [SerializeField] private ClientVoidalisk pfVoidalisk;
    [SerializeField] private ClientRiftweaver pfRiftweaver;
    [SerializeField] private ClientRiftLord pfRiftLord;
    [SerializeField] private ClientUltimateDevourer pfUltimateDevourer;
    [SerializeField] private ClientLasher pfLasher;
    [SerializeField] private ClientRavager pfRavager;
    [SerializeField] private ClientUltimateLeviathan pfUltimateLeviathan;
    
    [Space(3)]
    [Header("Water")]
    [SerializeField] private ClientSplasher pfSplasher;
    [SerializeField] private ClientTidecaller pfTidecaller;
    [SerializeField] private ClientWaterElemental pfWaterElemental;
    [SerializeField] private ClientSeaElemental pfSeaElemental;
    [SerializeField] private ClientUltimateHurricaneElemental pfUltimateHurricaneElemental;
    [SerializeField] private ClientTideLurker pfTideLurker;
    [SerializeField] private ClientAbyssStalker pfAbyssStalker;
    [SerializeField] private ClientUltimateSludgeMonstrosity pfUltimateSludgeMonstrosity;

    [Space(5)]
    [Header("Technology Disc Base")]
    [SerializeField] private ClientTechnologyDisc pfTechnologyDisc;
    [SerializeField] private ClientTechnologyDisc_Arcane pfTechnologyDisc_Arcane;
    [SerializeField] private ClientTechnologyDisc_Fire pfTechnologyDisc_Fire;
    [SerializeField] private ClientTechnologyDisc_Ice pfTechnologyDisc_Ice;
    [SerializeField] private ClientTechnologyDisc_Earth pfTechnologyDisc_Earth;
    [SerializeField] private ClientTechnologyDisc_Lightning pfTechnologyDisc_Lightning;
    [SerializeField] private ClientTechnologyDisc_Holy pfTechnologyDisc_Holy;
    [SerializeField] private ClientTechnologyDisc_Water pfTechnologyDisc_Water;
    [SerializeField] private ClientTechnologyDisc_Unholy pfTechnologyDisc_Unholy;
    [SerializeField] private ClientTechnologyDisc_Void pfTechnologyDisc_Void;
    [SerializeField] private ClientTechnologyDisc_Arcane_Ultimate pfTechnologyDisc_Arcane_Ultimate;
    [SerializeField] private ClientTechnologyDisc_Fire_Ultimate pfTechnologyDisc_Fire_Ultimate;
    [SerializeField] private ClientTechnologyDisc_Ice_Ultimate pfTechnologyDisc_Ice_Ultimate;
    [SerializeField] private ClientTechnologyDisc_Earth_Ultimate pfTechnologyDisc_Earth_Ultimate;
    [SerializeField] private ClientTechnologyDisc_Lightning_Ultimate pfTechnologyDisc_Lightning_Ultimate;
    [SerializeField] private ClientTechnologyDisc_Holy_Ultimate pfTechnologyDisc_Holy_Ultimate;
    [SerializeField] private ClientTechnologyDisc_Water_Ultimate pfTechnologyDisc_Water_Ultimate;
    [SerializeField] private ClientTechnologyDisc_Unholy_Ultimate pfTechnologyDisc_Unholy_Ultimate;
    [SerializeField] private ClientTechnologyDisc_Void_Ultimate pfTechnologyDisc_Void_Ultimate;

    [Space(5)]
    [Header("Builder")]
    [SerializeField] public ClientBuilder pfBuilder;
    [SerializeField] public TowerBuildProjection pfTowerBuildProjection;
    
    [Space(5)]
    [Header("Visual Effects")]
    [SerializeField] public VisualEffect pfEntityMoveCommandIndicator;
    [SerializeField] public VisualEffect pfEntityAttackCommandIndicator;
    
    private void Awake() {
        InitializeSingleton(this);

        Enemies = new Dictionary<EnemyType, ClientEnemy>() {
            { EnemyType.Sheep, pfSheep },
            { EnemyType.Wolf, pfWolf },
            { EnemyType.Skeleton, pfSkeleton },
            { EnemyType.Acolyte, pfAcolyte },
            { EnemyType.Spider, pfSpider },
            { EnemyType.Swordsman, pfSwordsman },
            { EnemyType.Grunt, pfGrunt },
            { EnemyType.Temptress, pfTemptress },
            { EnemyType.Shade, pfShade },
            { EnemyType.MudGolem, pfMudGolem },
            { EnemyType.Treant, pfTreant },
            { EnemyType.RotGolem, pfRotGolem },
            { EnemyType.Knight, pfKnight },
            { EnemyType.VengefulSpirit, pfVengefulSpirit },
            { EnemyType.ForestTroll, pfForestTroll },
            { EnemyType.Wyvern, pfWyvern },
            { EnemyType.Voidwalker, pfVoidwalker },
            { EnemyType.FacelessOne, pfFacelessOne },
            { EnemyType.Dragonspawn, pfDragonspawn },
            { EnemyType.SeaTurtle, pfSeaTurtle },
            { EnemyType.Banshee, pfBanshee },
            { EnemyType.SiegeEngine, pfSiegeEngine },
            { EnemyType.KoboldGeomancer, pfKoboldGeomancer },
            { EnemyType.Infernal, pfInfernal },
            { EnemyType.DeathRevenant, pfDeathRevenant },
            { EnemyType.SatyrShadowdancer, pfSatyrShadowdancer },
            { EnemyType.CryptFiend, pfCryptFiend },
            { EnemyType.SpiritWalker, pfSpiritWalker },
            { EnemyType.Necromancer, pfNecromancer },
            { EnemyType.AncientWendigo, pfAncientWendigo },
            { EnemyType.GryphonRider, pfGryphonRider },
            { EnemyType.OgreMagi, pfOgreMagi },
            { EnemyType.Abomination, pfAbomination },
            { EnemyType.ChaosWarden, pfChaosWarden },
            { EnemyType.MountainGiant, pfMountainGiant },
            { EnemyType.GoblinShredder, pfGoblinShredder },
            { EnemyType.KodoRider, pfKodoRider },
            { EnemyType.FrostWyrm, pfFrostWyrm },
            { EnemyType.Phoenix, pfPhoenix },
        };

        Towers = new Dictionary<TowerType, ClientTower>() {
            { TowerType.Archer, pfArcher },
            { TowerType.Gunner, pfGunner },
            { TowerType.WatchTower, pfWatchTower },
            { TowerType.GuardTower, pfGuardTower },
            { TowerType.WardTower, pfWardTower },
            { TowerType.UltimateWardTower, pfUltimateWardTower },
            { TowerType.CannonTower, pfCannonTower },
            { TowerType.BombardTower, pfBombardTower },
            { TowerType.ArtilleryTower, pfArtilleryTower },
            { TowerType.UltimateArtilleryTower, pfUltimateArtilleryTower },
            { TowerType.Cutter, pfCutter },
            { TowerType.Grinder, pfGrinder },
            { TowerType.Carver, pfCarver },
            { TowerType.Executioner, pfExecutioner },
            { TowerType.Mauler, pfMauler },
            { TowerType.UltimateMauler, pfUltimateMauler },
            { TowerType.Crusher, pfCrusher },
            { TowerType.Wrecker, pfWrecker },
            { TowerType.Mangler, pfMangler },
            { TowerType.UltimateMangler, pfUltimateMangler },
            { TowerType.ElementalCore, pfElementalCore },
            { TowerType.Spellslinger, pfSpellslinger },
            { TowerType.SpellslingerMaster, pfSpellslingerMaster },
            { TowerType.ArcanePylon, pfArcanePylon },
            { TowerType.ArcaneRepository, pfArcaneRepository },
            { TowerType.Archmage, pfArchmage },
            { TowerType.GrandArchmage, pfGrandArchmage },
            { TowerType.UltimateKirinTorWizard, pfUltimateKirinTorWizard },
            { TowerType.UltimateArcaneOrb, pfUltimateArcaneOrb },
            { TowerType.FirePit, pfFirePit },
            { TowerType.MagmaWell, pfMagmaWell },
            { TowerType.LivingFlame, pfLivingFlame },
            { TowerType.Hellfire, pfHellfire },
            { TowerType.UltimateFirelord, pfUltimateFirelord },
            { TowerType.MeteorAttractor, pfMeteorAttractor },
            { TowerType.Armageddon, pfArmageddon },
            { TowerType.UltimateMoonbeamProjector, pfUltimateMoonbeamProjector },
            { TowerType.FrozenObelisk, pfFrozenObelisk },
            { TowerType.RunicObelisk, pfRunicObelisk },
            { TowerType.FrozenWatcher, pfFrozenWatcher },
            { TowerType.FrozenCore, pfFrozenCore },
            { TowerType.UltimateLich, pfUltimateLich },
            { TowerType.Icicle, pfIcicle },
            { TowerType.Tricicle, pfTricicle },
            { TowerType.UltimateCrystal, pfUltimateCrystal },
            { TowerType.Rockfall, pfRockfall },
            { TowerType.Avalanche, pfAvalanche },
            { TowerType.EarthGuardian, pfEarthGuardian },
            { TowerType.AncientProtector, pfAncientProtector },
            { TowerType.UltimateAncientWarden, pfUltimateAncientWarden },
            { TowerType.NoxiousWeed, pfNoxiousWeed },
            { TowerType.NoxiousThorn, pfNoxiousThorn },
            { TowerType.UltimateScorpion, pfUltimateScorpion },
            { TowerType.ShockParticle, pfShockParticle },
            { TowerType.ShockGenerator, pfShockGenerator },
            { TowerType.Voltage, pfVoltage },
            { TowerType.HighVoltage, pfHighVoltage },
            { TowerType.UltimateOrbKeeper, pfUltimateOrbKeeper },
            { TowerType.LightningBeacon, pfLightningBeacon },
            { TowerType.LightningGenerator, pfLightningGenerator },
            { TowerType.UltimateAnnihilationGlyph, pfUltimateAnnihilationGlyph },
            { TowerType.LightFlies, pfLightFlies },
            { TowerType.HolyLantern, pfHolyLantern },
            { TowerType.SunrayTower, pfSunrayTower },
            { TowerType.SunbeamTower, pfSunbeamTower },
            { TowerType.UltimateTitanVault, pfUltimateTitanVault },
            { TowerType.Glowshroom, pfGlowshroom },
            { TowerType.Lightshroom, pfLightshroom },
            { TowerType.UltimateDivineshroom, pfUltimateDivineshroom },
            { TowerType.Splasher, pfSplasher },
            { TowerType.Tidecaller, pfTidecaller },
            { TowerType.WaterElemental, pfWaterElemental },
            { TowerType.SeaElemental, pfSeaElemental },
            { TowerType.UltimateHurricaneElemental, pfUltimateHurricaneElemental },
            { TowerType.TideLurker, pfTideLurker },
            { TowerType.AbyssStalker, pfAbyssStalker },
            { TowerType.UltimateSludgeMonstrosity, pfUltimateSludgeMonstrosity },
            { TowerType.PlagueWell, pfPlagueWell },
            { TowerType.Catacomb, pfCatacomb },
            { TowerType.SepticTank, pfSepticTank },
            { TowerType.PlagueFanatic, pfPlagueFanatic },
            { TowerType.UltimateGravedigger, pfUltimateGravedigger },
            { TowerType.ObsidianDestroyer, pfObsidianDestroyer },
            { TowerType.DecayedHorror, pfDecayedHorror },
            { TowerType.UltimateDiabolist, pfUltimateDiabolist },
            { TowerType.Voidling, pfVoidling },
            { TowerType.Voidalisk, pfVoidalisk },
            { TowerType.Riftweaver, pfRiftweaver },
            { TowerType.RiftLord, pfRiftLord },
            { TowerType.UltimateDevourer, pfUltimateDevourer },
            { TowerType.Lasher, pfLasher },
            { TowerType.Ravager, pfRavager },
            { TowerType.UltimateLeviathan, pfUltimateLeviathan },
            { TowerType.TechnologyDisc, pfTechnologyDisc },
            { TowerType.TechnologyDisc_Arcane, pfTechnologyDisc_Arcane },
            { TowerType.TechnologyDisc_Fire, pfTechnologyDisc_Fire },
            { TowerType.TechnologyDisc_Ice, pfTechnologyDisc_Ice },
            { TowerType.TechnologyDisc_Lightning, pfTechnologyDisc_Lightning },
            { TowerType.TechnologyDisc_Holy, pfTechnologyDisc_Holy },
            { TowerType.TechnologyDisc_Earth, pfTechnologyDisc_Earth },
            { TowerType.TechnologyDisc_Unholy, pfTechnologyDisc_Unholy },
            { TowerType.TechnologyDisc_Water, pfTechnologyDisc_Water },
            { TowerType.TechnologyDisc_Void, pfTechnologyDisc_Void },
            { TowerType.TechnologyDisc_Arcane_Ultimate, pfTechnologyDisc_Arcane_Ultimate },
            { TowerType.TechnologyDisc_Fire_Ultimate, pfTechnologyDisc_Fire_Ultimate },
            { TowerType.TechnologyDisc_Ice_Ultimate, pfTechnologyDisc_Ice_Ultimate },
            { TowerType.TechnologyDisc_Lightning_Ultimate, pfTechnologyDisc_Lightning_Ultimate },
            { TowerType.TechnologyDisc_Holy_Ultimate, pfTechnologyDisc_Holy_Ultimate },
            { TowerType.TechnologyDisc_Earth_Ultimate, pfTechnologyDisc_Earth_Ultimate },
            { TowerType.TechnologyDisc_Unholy_Ultimate, pfTechnologyDisc_Unholy_Ultimate },
            { TowerType.TechnologyDisc_Water_Ultimate, pfTechnologyDisc_Water_Ultimate },
            { TowerType.TechnologyDisc_Void_Ultimate, pfTechnologyDisc_Void_Ultimate },
        };
    }
}