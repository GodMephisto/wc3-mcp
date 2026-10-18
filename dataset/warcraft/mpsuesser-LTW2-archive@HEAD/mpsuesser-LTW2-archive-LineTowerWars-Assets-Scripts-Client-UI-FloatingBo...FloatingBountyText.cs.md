---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/FloatingBountyText/FloatingBountyText.cs"
ref: HEAD
---

using UnityEngine;
using TMPro;

public class FloatingBountyText : MonoBehaviour
{
    public static FloatingBountyText Create(int bounty, Vector3 location) {
        FloatingBountyText fbt = Instantiate(
            ClientPrefabs.Singleton.pfFloatingBountyText,
            location,
            Quaternion.identity,
            DynamicObjects.Singleton.BountyText
        );

        fbt.SetBountyAmount(bounty);
        return fbt;
    }

    private TMP_Text BountyText;

    private static float FloatSpeed => 5;
    private static float LifetimeInSeconds => 2;
    private float _spawnTime;

    private void Awake() {
        BountyText = GetComponentInChildren<TMP_Text>(true);

        _spawnTime = Time.time;

        if (CameraController.Singleton != null) {
            RotateTowardCamera(CameraController.Singleton);
        }
        
        EventBus.OnCameraMovement += RotateTowardCamera;
    }

    private void OnDestroy() {
        EventBus.OnCameraMovement -= RotateTowardCamera;
    }

    private void SetBountyAmount(int bountyAmount) {
        BountyText.SetText($"+{bountyAmount}");
    }

    private void Update() {
        if (Time.time - _spawnTime > LifetimeInSeconds) {
            Destroy(gameObject);
            return;
        }

        MoveUpward();
    }

    private void MoveUpward() {
        transform.position = Vector3.MoveTowards(
            transform.position, 
            transform.position + (Vector3.up * 10), 
            FloatSpeed * Time.deltaTime
        );
    }

    private void RotateTowardCamera(CameraController cc) {
        if (cc == null) {
            LTWLogger.Log($"Camera current was null");
            return;
        }

        Vector3 dirToCamera = (transform.position - cc.transform.position).normalized;

        transform.rotation = Quaternion.Euler(
            Quaternion.LookRotation(dirToCamera).eulerAngles.x, 
            cc.transform.rotation.eulerAngles.y, 
            0
        );
    }
}
