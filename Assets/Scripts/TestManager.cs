using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRContext {
    public const int SINGLE = 0;
    public const int HMD = 1;
    public const int CAVE = 2;
}

// This class runs the tests
public class TestManager : MonoBehaviour {


    public Canvas startMenu;
    public Transform startPosition; // DEBUG - GET BETTER SYSTEM FOR THIS
    public GameObject carPrefab;
    public AnalyticsController analytics;

    // CONTEXT IDS: 0 HMD, 1 CAVE, 2 SINGLE
    private int context = VRContext.SINGLE;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetContext(Dropdown contextID) {
        context = contextID.value;
    }

    public void StartTest() {
        // Hide menu
        startMenu.gameObject.SetActive(false);

        // Create car with correct context
        GameObject car = Instantiate(carPrefab);
        car.transform.position = startPosition.position;
        VRCameraPod cameras = car.GetComponentInChildren<VRCameraPod>();
        if(context == VRContext.SINGLE) {
            cameras.SetSingleCamera();
        } else if(context == VRContext.HMD) {
            cameras.SetVrCameras();
        } else if (context == VRContext.CAVE) {
            cameras.SetCaveCameras();
        }

        // Ensure modules have required stuff, if equipt
        DisplacementModule dm = analytics.gameObject.GetComponent<DisplacementModule>();
        if(dm != null) {
            dm.SetTarget(car);
        }

        // Start analytics
        analytics.StartTracking();
    }
}
