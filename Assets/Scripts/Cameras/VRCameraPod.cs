using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRCameraPod : MonoBehaviour {

    public GameObject vrCameras;
    public CaveCameraPod caveCameras;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetSingleCamera() {
        vrCameras.SetActive(false);
        caveCameras.SetFrontCamera();
    }

    public void SetCaveCameras() {
        vrCameras.SetActive(false);
        caveCameras.SetAllCameras();
    }

    public void SetVrCameras() {
        vrCameras.SetActive(true);
        caveCameras.DisableCameras();
    }
}
