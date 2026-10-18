---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/Frame/EntityImage.cs"
ref: HEAD
---

using UnityEngine;
using UnityEngine.UI;

public class EntityImage : HoverTooltip
{
    private Image Image { get; set; }

    private ClientEntity LoadedEntity { get; set; }

    protected override void Awake() {
        GetImageRef();

        base.Awake();
    }

    public void LoadImageForEntity(ClientEntity e) {
        if (Image == null) {
            GetImageRef();
        }

        if (e is ClientTower tower) {
            Image.sprite = ClientResources.Singleton.GetSpriteForTowerType(tower.Type);
        } else if (e is ClientEnemy enemy) {
            Image.sprite = ClientResources.Singleton.GetSpriteForEnemyType(enemy.Type);
        }

        LoadedEntity = e;
    }

    private void GetImageRef() {
        Image = GetComponentInChildren<Image>();
    }

    protected override Tooltip GetTooltipContent() {
        if (LoadedEntity == null) {
            throw new NotFoundException();
        }

        if (LoadedEntity is ClientEnemy e) {
            return new UnitTooltip(e.Type);
        } else if (LoadedEntity is ClientTower t) {
            return new TowerTooltip(t.Type);
        } else {
            throw new NotFoundException();
        }
    }
}
