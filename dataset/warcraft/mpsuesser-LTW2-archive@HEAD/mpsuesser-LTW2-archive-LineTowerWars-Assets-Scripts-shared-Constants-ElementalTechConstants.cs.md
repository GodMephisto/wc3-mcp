---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Constants/ElementalTechConstants.cs"
ref: HEAD
---

﻿using System.Collections.Generic;

public class ElementalTechConstants {
    public const int InitialTechCost = 10;

    public static readonly Dictionary<ElementalTechType, string> DisplayName =
        new Dictionary<ElementalTechType, string>() {
            { ElementalTechType.Arcane0, "Arcane Technology (Basic)" },
            { ElementalTechType.Arcane1, "Arcane Technology: Archmage" },
            { ElementalTechType.Arcane2, "Arcane Technology: Arcane Pylon" },
            { ElementalTechType.Fire0, "Fire Technology (Basic)" },
            { ElementalTechType.Fire1, "Fire Technology: Meteor Attractor" },
            { ElementalTechType.Fire2, "Fire Technology: Living Flame" },
            { ElementalTechType.Ice0, "Ice Technology (Basic)" },
            { ElementalTechType.Ice1, "Ice Technology: Frozen Watcher" },
            { ElementalTechType.Ice2, "Ice Technology: Icicle" },
            { ElementalTechType.Earth0, "Earth Technology (Basic)" },
            { ElementalTechType.Earth1, "Earth Technology: Earth Guardian" },
            { ElementalTechType.Earth2, "Earth Technology: Noxious Weed" },
            { ElementalTechType.Lightning0, "Lightning Technology (Basic)" },
            { ElementalTechType.Lightning1, "Lightning Technology: Lightning Beacon" },
            { ElementalTechType.Lightning2, "Lightning Technology: Voltage" },
            { ElementalTechType.Holy0, "Holy Technology (Basic)" },
            { ElementalTechType.Holy1, "Holy Technology: Glowshroom" },
            { ElementalTechType.Holy2, "Holy Technology: Sunray Tower" },
            { ElementalTechType.Unholy0, "Unholy Technology (Basic)" },
            { ElementalTechType.Unholy1, "Unholy Technology: Septic Tank" },
            { ElementalTechType.Unholy2, "Unholy Technology: Obsidian Destroyer" },
            { ElementalTechType.Void0, "Void Technology (Basic)" },
            { ElementalTechType.Void1, "Void Technology: Riftweaver" },
            { ElementalTechType.Void2, "Void Technology: Lasher" },
            { ElementalTechType.Water0, "Water Technology (Basic)" },
            { ElementalTechType.Water1, "Water Technology: Water Elemental" },
            { ElementalTechType.Water2, "Water Technology: Tide Lurker" },
        };
}