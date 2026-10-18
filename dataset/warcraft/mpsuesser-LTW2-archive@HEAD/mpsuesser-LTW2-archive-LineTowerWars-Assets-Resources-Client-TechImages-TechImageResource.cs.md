---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Resources/Client/TechImages/TechImageResource.cs"
ref: HEAD
---

using UnityEngine;

[CreateAssetMenu(fileName = "TechImageResource", menuName = "ScriptableObjects/TechImageResource", order = 1)]
public class TechImageResource : ScriptableObject
{
    [Space(5)]
    [SerializeField] public ElementalTechType TechType;
    [SerializeField] public Sprite Image;
}