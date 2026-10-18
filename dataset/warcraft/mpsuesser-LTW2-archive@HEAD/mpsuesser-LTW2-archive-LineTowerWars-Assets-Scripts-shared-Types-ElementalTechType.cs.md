---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Types/ElementalTechType.cs"
ref: HEAD
---

﻿using System.Collections.Generic;

public enum ElementalTechType {
    Arcane0 = 0,
    Arcane1,
    Arcane2,
    Fire0,
    Fire1,
    Fire2,
    Ice0,
    Ice1,
    Ice2,
    Earth0,
    Earth1,
    Earth2,
    Lightning0,
    Lightning1,
    Lightning2,
    Holy0,
    Holy1,
    Holy2,
    Water0,
    Water1,
    Water2,
    Unholy0,
    Unholy1,
    Unholy2,
    Void0,
    Void1,
    Void2,
}

public class ElementalTech {
    public static Dictionary<ElementalTechType, ElementalTechType> Prerequisite = new Dictionary<ElementalTechType, ElementalTechType>() {
        { ElementalTechType.Arcane1, ElementalTechType.Arcane0 },
        { ElementalTechType.Arcane2, ElementalTechType.Arcane0 },
        { ElementalTechType.Fire1, ElementalTechType.Fire0 },
        { ElementalTechType.Fire2, ElementalTechType.Fire0 },
        { ElementalTechType.Ice1, ElementalTechType.Ice0 },
        { ElementalTechType.Ice2, ElementalTechType.Ice0 },
        { ElementalTechType.Earth1, ElementalTechType.Earth0 },
        { ElementalTechType.Earth2, ElementalTechType.Earth0 },
        { ElementalTechType.Lightning1, ElementalTechType.Lightning0 },
        { ElementalTechType.Lightning2, ElementalTechType.Lightning0 },
        { ElementalTechType.Holy1, ElementalTechType.Holy0 },
        { ElementalTechType.Holy2, ElementalTechType.Holy0 },
        { ElementalTechType.Water1, ElementalTechType.Water0 },
        { ElementalTechType.Water2, ElementalTechType.Water0 },
        { ElementalTechType.Unholy1, ElementalTechType.Unholy0 },
        { ElementalTechType.Unholy2, ElementalTechType.Unholy0 },
        { ElementalTechType.Void1, ElementalTechType.Void0 },
        { ElementalTechType.Void2, ElementalTechType.Void0 },
    };
}
