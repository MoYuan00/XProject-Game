using UnityEngine;

public class CameraAction : MonoBehaviour
{
    public CameraMode cameraState;

    public bool isDefault = false;

    private void Start()
    {
        OnCameraChange(CameraManager.CurrentCameraMode);
        CameraManager.OnCameraModeChange.AddListener(OnCameraChange);
        
        if(isDefault) CameraManager.ChangeCameraState(cameraState);
    }

    private void OnCameraChange(CameraMode activeState)
    {
        gameObject.SetActive(cameraState == activeState);
    }
}
