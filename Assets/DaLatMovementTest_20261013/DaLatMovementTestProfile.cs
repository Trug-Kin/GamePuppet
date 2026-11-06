using System;
using System.IO;
using UnityEngine;

// Scene-local test profile. No physics or global InputManager changes.
[DefaultExecutionOrder(-20000)]
public sealed class DaLatMovementTestProfile : MonoBehaviour
{
    public DieuKhienDiChuyen player;
    public string expectedSceneName = "MovementTest_ImmediateInput";
    bool previousRawInput;
    bool applied;

    void Awake()
    {
        if (gameObject.scene.name != expectedSceneName || player == null)
        {
            Debug.LogError("DaLat immediate-input profile only supports its dedicated test scene.");
            enabled = false;
            return;
        }
        previousRawInput = player.useImmediateMovementInput;
        applied = true;
        player.useImmediateMovementInput = true;
        foreach (var compliance in player.GetComponentsInChildren<NhichCameraKhiDungYen>(true))
        {
            compliance.StopAllCoroutines();
            compliance.enabled = false;
        }
    }
    void OnDestroy()
    {
        if (applied && player != null) player.useImmediateMovementInput = previousRawInput;
    }
}
