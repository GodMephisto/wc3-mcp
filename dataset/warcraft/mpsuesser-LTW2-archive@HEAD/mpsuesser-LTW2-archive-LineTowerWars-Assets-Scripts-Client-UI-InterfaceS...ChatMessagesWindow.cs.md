---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/common/ChatInterface/ChatMessagesWindow.cs"
ref: HEAD
---

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatMessagesWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private Image windowBackground;
    
    private Color ActivatedColor { get; set; }
    private Color DeactivatedColor { get; set; }

    private bool MessageEntryOpen { get; set; }
    private bool Hovering { get; set; }

    private const float DeactivationTotalDuration = 1f;
    private bool _deactivating;
    private float _timeOfMostRecentDeactivation;

    private void Awake() {
        ActivatedColor = windowBackground.color;
        DeactivatedColor = new Color(0, 0, 0, .02f);
        
        MessageEntryOpen = false;
        Hovering = false;

        _deactivating = false;
        _timeOfMostRecentDeactivation = Time.time;
    }

    private void Start() {
        DeactivateBackground(true);
    }

    private void Update() {
        if (_deactivating) {
            float timeSinceDeactivation = Time.time - _timeOfMostRecentDeactivation;
            if (timeSinceDeactivation > DeactivationTotalDuration) {
                windowBackground.color = DeactivatedColor;
                _deactivating = false;
            }
            else {
                windowBackground.color = Color.Lerp(
                    ActivatedColor, 
                    DeactivatedColor,
                    (timeSinceDeactivation / DeactivationTotalDuration)    
                );
            }
        }
    }

    public void SetMessageEntryOpen() {
        MessageEntryOpen = true;

        ActivateBackground();
    }

    public void SetMessageEntryClosed() {
        MessageEntryOpen = false;

        DeactivateBackground(true);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Hovering = true;

        ActivateBackground();
    }

    public void OnPointerExit(PointerEventData eventData) {
        Hovering = false;

        DeactivateBackground();
    }

    private void ActivateBackground() {
        if (MessageEntryOpen || Hovering) {
            windowBackground.color = ActivatedColor;
            
            _deactivating = false;
        }
    }

    private void DeactivateBackground(bool snapClose = false) {
        if (!MessageEntryOpen && !Hovering) {
            if (snapClose) {
                windowBackground.color = DeactivatedColor;
            }
            else {
                _deactivating = true;
                _timeOfMostRecentDeactivation = Time.time;
            }
        }
    }
}