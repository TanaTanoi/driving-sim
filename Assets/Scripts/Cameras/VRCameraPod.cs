using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
* Controls CAVE and Headset Cameras using SteamVR.
* Allows setting single screen, three secreens, or headset VR.
* Left Camera:    Display 2
* Front Camera:   Display 3
* Right Camera:   Display 4
* Headset Camera: Display 
* 
* Author: Tana Tanoi
*/

public class VRCameraPod : MonoBehaviour {

    public GameObject headsetCameraWrapper;
    public CaveCameraPod caveCameras;
    public Transform idealHeadsetPosition;
    
    // Enables single front screen
    public void SetSingleCamera() {
        headsetCameraWrapper.SetActive(false);
        caveCameras.SetFrontCamera();
    }

    // Enables CAVE Cameras
    public void SetCaveCameras() {
        headsetCameraWrapper.SetActive(false);
        caveCameras.SetAllCameras();
    }

    // Enables Headset camera
    public void SetVrCameras() {
        headsetCameraWrapper.SetActive(true);
        caveCameras.DisableCameras();
    }

    // Move's the headset wrapper such that the headset camera is in the ideal position, specified by the transform
    public void CalibrateHeadset() {
        // Assumes the headset wrapper only contains the SteamVR camera.
        GameObject headsetCamera = headsetCameraWrapper.transform.GetChild(0).gameObject;
        headsetCameraWrapper.transform.position += (idealHeadsetPosition.position - headsetCamera.transform.position);
    }
}
