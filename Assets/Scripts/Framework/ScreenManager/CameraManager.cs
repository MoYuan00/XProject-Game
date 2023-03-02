using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 每个相机自主监听相机修改事件
/// 相机是否激活由相机自身进行决定
/// </summary>
public static class CameraManager
{
    public static CameraMode CurrentCameraMode { get; private set; }

    public static readonly UnityEvent<CameraMode> OnCameraModeChange = new UnityEvent<CameraMode>();

    public static void ChangeCameraState(CameraMode cameraState)
    {
        if(cameraState == CurrentCameraMode) return;
        
        CurrentCameraMode = cameraState;
        Debug.Log($"ChangeCameraState:{cameraState}");
        OnCameraModeChange.Invoke(cameraState);
    }
}

public enum CameraMode
{
    WalkingAim,
    Free,
    FreeHigh
}