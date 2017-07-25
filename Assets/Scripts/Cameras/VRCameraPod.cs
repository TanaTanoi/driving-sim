using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCameraPod : MonoBehaviour {

    public GameObject headsetCameraWrapper;
    public CaveCameraPod caveCameras;
    public Transform idealHeadsetPosition;

    private GameObject headsetCamera;

    public void SetSingleCamera() {
        headsetCameraWrapper.SetActive(false);
        caveCameras.SetFrontCamera();
    }

    public void SetCaveCameras() {
        headsetCameraWrapper.SetActive(false);
        caveCameras.SetAllCameras();
    }

    public void SetVrCameras() {
        headsetCameraWrapper.SetActive(true);
        caveCameras.DisableCameras();
    }

    // Move's the headset wrapper such that the headset camera is in the ideal position
    public void CalibrateHeadset() {
        headsetCamera = headsetCameraWrapper.transform.GetChild(0).gameObject;
        headsetCameraWrapper.transform.position += (idealHeadsetPosition.position - headsetCamera.transform.position);
    }
}
