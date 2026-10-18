---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Resources/Client/TowerImages/TowerImageResource.cs"
ref: HEAD
---

using UnityEngine;

[CreateAssetMenu(fileName = "TowerNameImageResource", menuName = "ScriptableObjects/TowerImageResource", order = 1)]
public class TowerImageResource : ScriptableObject
{
    [Space(5)]
    [SerializeField] public TowerType Tower;
    [SerializeField] public Sprite Image;
}