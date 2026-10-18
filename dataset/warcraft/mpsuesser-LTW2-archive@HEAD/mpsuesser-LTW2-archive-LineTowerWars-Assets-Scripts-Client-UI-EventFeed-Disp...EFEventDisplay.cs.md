---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/EventFeed/Displays/EFEventDisplay.cs"
ref: HEAD
---

﻿using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public abstract class EFEventDisplay : MonoBehaviour {
    private const float FlashDuration = 0.5f;
    
    private Image backgroundImage;

    private bool isFlashing;
    private float initialFlashTime;
    private Color initialFlashColor;
    private Color finalFlashColor;

    protected virtual void Awake() {
        backgroundImage = GetComponent<Image>();
        
        initialFlashTime = Time.time - FlashDuration - 1;
    }

    protected virtual void Update() {
        if (!isFlashing) {
            return;
        }

        float timeSinceLastFlash = Time.time - initialFlashTime;
        if (timeSinceLastFlash > FlashDuration) {
            isFlashing = false;
            backgroundImage.color = finalFlashColor;
            return;
        }

        backgroundImage.color = Color.Lerp(
            initialFlashColor,
            finalFlashColor,
            timeSinceLastFlash / FlashDuration
        );
    }
    
    protected void FlashPositive() {
        initialFlashColor = ClientPrefabs.Singleton.pfPositiveFlashColor;
        FlashGenericAfterColorHasBeenSet();
    }

    protected void FlashNegative() {
        initialFlashColor = ClientPrefabs.Singleton.pfNegativeFlashColor;
        FlashGenericAfterColorHasBeenSet();
    }

    private void FlashGenericAfterColorHasBeenSet() {
        initialFlashTime = Time.time;
        isFlashing = true;
        finalFlashColor = new Color(
            initialFlashColor.r,
            initialFlashColor.g,
            initialFlashColor.b,
            0
        );
    }
}