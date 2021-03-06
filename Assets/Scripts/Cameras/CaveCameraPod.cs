﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
*  Controls the individual CAVE cameras, if multiple displays are active.
*  Left Camera:    Display 2
*  Front Camera:   Display 3
*  Right Camera:   Display 4
*  Author: Tana Tanoi
*/
public class CaveCameraPod : MonoBehaviour {

    public GameObject frontCamera;
    public GameObject leftCamera;
    public GameObject rightCamera;

    public void SetAllCameras() {
        EnableAllDisplays();
        frontCamera.SetActive(true);
        leftCamera.SetActive(true);
        rightCamera.SetActive(true);
    }

    public void SetFrontCamera() {
        EnableAllDisplays();
        frontCamera.SetActive(true);
        leftCamera.SetActive(false);
        rightCamera.SetActive(false);
    }

    public void DisableCameras() {
        frontCamera.SetActive(false);
        leftCamera.SetActive(false);
        rightCamera.SetActive(false);
    }

    private void EnableAllDisplays() {
        Display[] displays = Display.displays;

        if (displays.Length >= 4) {
            displays[1].Activate(1920, 1080, 60);
            displays[2].Activate(1920, 1080, 60);
            displays[3].Activate(1920, 1080, 60);
        }
    }
}

