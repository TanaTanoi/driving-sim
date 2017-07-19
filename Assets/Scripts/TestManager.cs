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
    public Canvas resultsMenu;
    public Transform startPosition; // DEBUG - GET BETTER SYSTEM FOR THIS
    public GameObject carPrefab;
    public AnalyticsController analytics;

    private GameObject car;

    // CONTEXT IDS: 0 HMD, 1 CAVE, 2 SINGLE
    private int context = VRContext.SINGLE;

	// Use this for initialization
	void Start () {
        ShowMainMenu();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetContext(Dropdown contextID) {
        context = contextID.value;
    }

    public void ShowMainMenu() {
        startMenu.gameObject.SetActive(true);
        resultsMenu.gameObject.SetActive(false);
    }

    // Sets up the test with analytics and a car, then starts it after a delay
    public void StartTest() {
        // Hide start menu
        startMenu.gameObject.SetActive(false);

        SetupCar();
        SetupAnalytics();

        Countdown cd = car.GetComponentInChildren<Countdown>();
        cd.StartCountdown(1, EnableControl);
    }

    public void EnableControl() {
        // TODO rework car control parts
        UnityStandardAssets.Vehicles.Car.CarController controller = car.GetComponent<UnityStandardAssets.Vehicles.Car.CarController>();
        controller.enabled = true;

        analytics.StartTracking();
    }

    private void SetupCar() {
        // Create car with correct context
        car = Instantiate(carPrefab);
        car.transform.position = startPosition.position;
        VRCameraPod cameras = car.GetComponentInChildren<VRCameraPod>();
        if(context == VRContext.SINGLE) {
            cameras.SetSingleCamera();
        } else if(context == VRContext.HMD) {
            cameras.SetVrCameras();
        } else if (context == VRContext.CAVE) {
            cameras.SetCaveCameras();
        }

        // TODO rework
        UnityStandardAssets.Vehicles.Car.CarController controller = car.GetComponent<UnityStandardAssets.Vehicles.Car.CarController>();
        controller.enabled = false;
    }

    // Sets up any modules with additional requirements (e.g. DisplacementModule needs a target)
    private void SetupAnalytics() {

        analytics.ClearTracking();
        
        // Ensure modules have required stuff, if equipt
        DisplacementModule dm = analytics.gameObject.GetComponent<DisplacementModule>();
        if(dm != null) {
            dm.SetTarget(car);
        }
    }

    // Stops tracking, destroys the car, and displays the results menu
    public void EndTest() {
        analytics.StopTracking();
        Destroy(car);
        resultsMenu.gameObject.SetActive(true);
    }

    public void SaveResults(InputField input) {
        string filename = FilenameFor(input.text);
        WriteToFile(filename, analytics.Data());
    }


    // Returns the full filename for a given name in the format:
    //  results_name_yyyy_MM_dd_HH_mm.txt 
    private string FilenameFor(string name) {
        if(name.Length == 0) {
            name = "unlabelled";
        }
        name = name.Replace(' ', '_');
        return "results_" + name + "_" + System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm") + ".txt";
    }

    // Writes contents to a file. Returns false if it failed
    private bool WriteToFile(string filename, string[] content) {
        try {
            System.IO.File.WriteAllLines(filename, content);
        } catch( System.IO.IOException e) {
            Debug.Log("Error - Could not write: " + e);
            return false;
        }
        return true;
    }
}
