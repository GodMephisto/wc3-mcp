---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/MessageDisplay/ScreenMessage.cs"
ref: HEAD
---

﻿using UnityEngine;
using System;
using TMPro;

public class ScreenMessage : MonoBehaviour {
    public static ScreenMessage Create(string text, bool isNegativeMessage, Transform parent) {
        ScreenMessage sm = Instantiate(ClientPrefabs.Singleton.pfScreenMessage, parent);
        sm.SetText(text);
        sm.SetMessageEmotion(isNegativeMessage);
        return sm;
    }
    
    private Color InitialFlashColor { get; set; }
    private Color BaseColor { get; set; }
    private Color FadeTargetColor { get; set; }

    public event Action OnDestroyed;

    // InitialFlashDuration + FadeDuration must be <= DisplayDuration
    private const float InitialFlashDuration = 0.5f;
    private const float DisplayDuration = 5f;
    private const float FadeDuration = 1f;
    
    private float creationTime;
    private TMP_Text MyText { get; set; }

    private void Awake() {
        MyText = GetComponent<TMP_Text>();
        BaseColor = MyText.color;
        FadeTargetColor = new Color(
            BaseColor.r,
            BaseColor.g,
            BaseColor.b, 
            0
        );
        
        creationTime = Time.time;
    }

    private void Update() {
        float activeDuration = Time.time - creationTime;
        if (activeDuration > DisplayDuration) {
            Destroy(gameObject);
        } else if (activeDuration < InitialFlashDuration) {
            MyText.color = Color.Lerp(
                InitialFlashColor,
                BaseColor,
                activeDuration / InitialFlashDuration
            );
        } else if (activeDuration > DisplayDuration - FadeDuration) {
            MyText.color = Color.Lerp(
                FadeTargetColor,
                BaseColor,
                (DisplayDuration - activeDuration) / FadeDuration
            );
        }
    }

    private void OnDestroy() {
        OnDestroyed?.Invoke();
    }

    private void SetText(string s) {
        MyText.SetText(s);
    }

    private void SetMessageEmotion(bool isNegativeMessage) {
        InitialFlashColor =
            isNegativeMessage
                ? ClientPrefabs.Singleton.pfNegativeFlashColor
                : ClientPrefabs.Singleton.pfPositiveFlashColor;
    }
}