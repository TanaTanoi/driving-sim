using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveCameraPod : MonoBehaviour {

    public GameObject frontCamera;
    public GameObject leftCamera;
    public GameObject rightCamera;

    public void SetAllCameras() {
        frontCamera.SetActive(true);
        leftCamera.SetActive(true);
        rightCamera.SetActive(true);
    }

    public void SetFrontCamera() {
        frontCamera.SetActive(true);
        leftCamera.SetActive(false);
        rightCamera.SetActive(false);
    }

    public void DisableCameras() {
        frontCamera.SetActive(false);
        leftCamera.SetActive(false);
        rightCamera.SetActive(false);
    }
}

