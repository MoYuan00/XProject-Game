using System;
using UnityEngine;

public class CameraAction : MonoBehaviour
{
    // public CameraActionItem[] actionList;
    public CamerasActiveState cameraState;

    private void Start()
    {
        OnCameraChange(CameraManager.currentCameraState);
        CameraManager.onCameraActiveStateChange.AddListener(OnCameraChange);
    }

    private void OnCameraChange(CamerasActiveState activeState)
    {
        gameObject.SetActive(cameraState == activeState);
    }
}

// [System.Serializable]
// public struct CameraActionItem
// {
// public CamerasActiveState cameraState;
// public bool isActive;
// }
