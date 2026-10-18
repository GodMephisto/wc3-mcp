---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/SendMenu/SendUnitOption.cs"
ref: HEAD
---

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SendUnitOption : HoverTooltip
{
    public static event Action<EnemyType> OnSendUnitPressed;

    [SerializeField] private EnemyType EnemyTypeToSend;

    [Space(5)]
    [SerializeField] private Image PositiveImage;
    [SerializeField] private Image NegativeImage;
    [SerializeField] private TMP_Text HotkeyText;
    [SerializeField] private TMP_Text StockText;
    [SerializeField] private TMP_Text StockCooldownText;

    private KeyCode Hotkey { get; set; }
    
    private Button Btn { get; set; }

    protected override void Awake() {
        Btn = GetComponentInChildren<Button>(true);

        Btn.onClick.AddListener(SendButtonClicked);
        UpdateHotkeyText();

        base.Awake();
    }

    protected override void Start() {
        Sprite creepSprite = ClientResources.Singleton.GetSpriteForEnemyType(EnemyTypeToSend);
        PositiveImage.sprite = creepSprite;
        NegativeImage.sprite = creepSprite;

        base.Start();
    }

    private void SendButtonClicked() => SendClicked();
    public bool SendClicked() {
        OnSendUnitPressed?.Invoke(EnemyTypeToSend);
        return true;
    }

    private void Update() {
        Lane lane = ClientLaneTracker.Singleton.MyLane;
        int stockAmount = lane.Stock.GetStockForCreep(EnemyTypeToSend);
        StockText.SetText(stockAmount < 1 ? "" : stockAmount.ToString());

        if (stockAmount > 0) {
            PositiveImage.fillAmount = 1;
            StockCooldownText.SetText("");
        }
        else if (stockAmount == CreepStock.InitialDelayNotYetFinished) {
            float timeOfLastIncrement =
                lane.Stock.GetMostRecentIncrementTimeForCreep(EnemyTypeToSend);
            float timeSinceLastIncrement =
                Time.time - timeOfLastIncrement;

            float initialAvailabilityTime = (float) EnemyConstants.InitialStockDelay[EnemyTypeToSend];
            float timeRemaining = Mathf.Max(0, initialAvailabilityTime - timeSinceLastIncrement);
            int timeRemainingCeil = (int)Mathf.Ceil(timeRemaining);
            
            StockCooldownText.SetText(timeRemainingCeil.ToString());
            PositiveImage.fillAmount = 1 - (timeRemaining / initialAvailabilityTime);
        }
        else { // initial delay is over with but we're out of stock
            float timeOfLastIncrement =
                lane.Stock.GetMostRecentIncrementTimeForCreep(EnemyTypeToSend);
            float timeSinceLastIncrement =
                Time.time - timeOfLastIncrement;

            float incrementTime = (float) EnemyConstants.StockIncrementTimer[EnemyTypeToSend];
            float timeRemaining = Mathf.Max(0, incrementTime - timeSinceLastIncrement);
            int timeRemainingCeil = (int)Mathf.Ceil(timeRemaining);
            
            StockCooldownText.SetText(timeRemainingCeil.ToString());
            PositiveImage.fillAmount = 1 - (timeRemaining / incrementTime);
        }
    }

    public void SetHotkey(KeyCode kc) {
        Hotkey = kc;
        UpdateHotkeyText();
    }

    private void UpdateHotkeyText() {
        if (Hotkey == KeyCode.None) {
            return;
        }
        
        HotkeyText.SetText(ClientUtil.GetKeyCodeStringRepresentation(Hotkey));
    }

    protected override Tooltip GetTooltipContent() {
        return new SendUnitTooltip(EnemyTypeToSend);
    }
}
