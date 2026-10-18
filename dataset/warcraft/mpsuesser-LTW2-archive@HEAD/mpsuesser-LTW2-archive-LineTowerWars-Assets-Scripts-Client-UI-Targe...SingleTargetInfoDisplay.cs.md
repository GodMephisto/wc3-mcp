---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/Frame/SingleTargetInfoDisplay.cs"
ref: HEAD
---

using UnityEngine;
using TMPro;

public class SingleTargetInfoDisplay : MonoBehaviour
{
    [SerializeField] private EntityImage Image;
    [SerializeField] private TMP_Text Name;
    [SerializeField] private ProgressBar HealthBar;
    [SerializeField] private ProgressBar SecondaryBar;

    private ClientEntity Target { get; set; }

    public void Load(ClientEntity target) {
        SetTarget(target);

        Image.LoadImageForEntity(target);
        LoadNameForEntity(target);

        UpdateStatusBars(target);
    }

    private void LoadNameForEntity(ClientEntity target) {
        switch (target) {
            case ClientTower tower:
                Name.SetText(TowerConstants.DisplayName[tower.Type]);
                break;
            case ClientEnemy creep:
                Name.SetText(EnemyConstants.DisplayName[creep.Type]);
                break;
            default:
                Name.SetText(target.gameObject.name);
                break;
        }
    }

    private void UpdateStatusBars(ClientEntity entity) {
        HealthBar.UpdateProgress(entity.HP, entity.MaxHP);
        HealthBar.UpdateColorByFillAmount();

        if (entity.MaxMP == 0) {
            SecondaryBar.gameObject.SetActive(false);
            return;
        }
        
        SecondaryBar.UpdateProgress(entity.MP, entity.MaxMP);
        SecondaryBar.gameObject.SetActive(true);
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void Hide() {
        UnsetTarget();

        gameObject.SetActive(false);
    }

    private void SetTarget(ClientEntity entityToTarget) {
        UnsetTarget();

        Target = entityToTarget;
        Target.OnStatusUpdated += UpdateStatusBars;
    }

    private void UnsetTarget() {
        if (Target == null) {
            return;
        }

        Target.OnStatusUpdated -= UpdateStatusBars;
        Target = null;
    }
}
