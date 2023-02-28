using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Framework;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 每个相机自主监听相机修改事件
/// 相机是否激活由相机自身进行决定
/// </summary>
public static class CameraManager
{
    private static FSM<CamerasActiveState> _fsm = new FSM<CamerasActiveState>();

    public static UnityEvent<CamerasActiveState> onCameraActiveStateChange = new UnityEvent<CamerasActiveState>();

    public static CamerasActiveState currentCameraState;

    public static void ChangeCameraState(CamerasActiveState activeState)
    {
        currentCameraState = activeState;
        Debug.Log($"ChangeCameraState:{activeState}");
        onCameraActiveStateChange?.Invoke(activeState);
    }
}

public enum CamerasActiveState
{
    TherePeopleView,
    AimingView,
}