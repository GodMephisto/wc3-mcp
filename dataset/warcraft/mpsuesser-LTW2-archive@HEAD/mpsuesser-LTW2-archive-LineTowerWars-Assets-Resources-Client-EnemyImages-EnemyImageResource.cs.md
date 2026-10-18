---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Resources/Client/EnemyImages/EnemyImageResource.cs"
ref: HEAD
---

using UnityEngine;

[CreateAssetMenu(fileName = "EnemyNameImageResource", menuName = "ScriptableObjects/EnemyImageResource", order = 1)]
public class EnemyImageResource : ScriptableObject
{
    [Space(5)]
    [SerializeField] public EnemyType Enemy;
    [SerializeField] public Sprite Image;
}