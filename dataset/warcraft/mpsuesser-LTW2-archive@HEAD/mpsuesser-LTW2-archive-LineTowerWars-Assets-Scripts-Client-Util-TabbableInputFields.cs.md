---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/Util/TabbableInputFields.cs"
ref: HEAD
---

﻿using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

// Exists for the purpose of tab navigation and enter submission
public class TabbableInputFields : MonoBehaviour
{
    [SerializeField] private TMP_InputField[] InputFields;
    [SerializeField] private bool autoFocusFirstField;

    private EventSystem system;

    private void OnEnable() {
        system = EventSystem.current;
        
        if (autoFocusFirstField && InputFields.Length >= 1) {
            StartCoroutine(AutoFocusFirstField());
        }
    }

    private IEnumerator AutoFocusFirstField() {
        // This is unnecessary, but I've reached by "fuck it" moment for the day.
        // AutoFocus was working for CreateAccountInterface but not LoginInterface
        // Everything was the same, object was being selected but could not type. Could not track down wtf was going on
        yield return new WaitForSeconds(0.1f);
        SelectInputFieldAtIndex(0);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            SelectNextInputField();
        }
    }

    private void SelectNextInputField() {
        GameObject selectedObject = system.currentSelectedGameObject;
        if (selectedObject == null) {
            SelectInputFieldAtIndex(0);
            return;
        }

        TMP_InputField selectedInputField = selectedObject.GetComponent<TMP_InputField>();
        if (selectedInputField == null) {
            SelectInputFieldAtIndex(0);
            return;
        }

        for (int i = 0; i < InputFields.Length; i++) {
            if (selectedInputField == InputFields[i]) {
                SelectInputFieldAtIndex((i + 1) % InputFields.Length);
                return;
            }
        }

        SelectInputFieldAtIndex(0);
    }

    // https://forum.unity.com/threads/tab-between-input-fields.263779/#post-1745981
    private void SelectInputFieldAtIndex(int i) {
        InputFields[i].OnPointerClick(new PointerEventData(system));
        system.SetSelectedGameObject(InputFields[i].gameObject, new BaseEventData(system));
    }
}