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

    public CanvasGroup startMenu;
    public CanvasGroup resultsMenu;
    public CanvasGroup observerMenu;

    public SmallCityBuilder city;
    public GameObject carPrefab;
    public AnalyticsController analytics;

    private GameObject car;

    string output;
    string stack;

    // Sets up the test with analytics and a car, then starts it after a delay
    public void StartTest(int context, Texture2D layout) {

        //Application.RegisterLogCallback(HandleLog); // for Debugging in build mode

        city.map = layout;
        city.BuildCity();

        ShowObserverMenu();

        SetupCar(context);
        SetupAnalytics();

        Countdown cd = car.GetComponentInChildren<Countdown>();
        cd.StartCountdown(1, EnableControl);
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        output += logString;
        stack = stackTrace;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), output);
    }

    public void EnableControl() {
        // TODO rework car control parts
        UnityStandardAssets.Vehicles.Car.CarController controller = car.GetComponent<UnityStandardAssets.Vehicles.Car.CarController>();
        controller.enabled = true;
        car.GetComponentInChildren<VRCameraPod>().CalibrateHeadset();

        analytics.StartTracking();
    }

    private void SetupCar(int context) {
        // Create car with correct context
        car = Instantiate(carPrefab);
        car.transform.position = city.GetStartPosition();
        VRCameraPod cameras = car.GetComponentInChildren<VRCameraPod>();
        if(context == VRContext.SINGLE) {
            cameras.SetSingleCamera();
        } else if(context == VRContext.HMD) {
            cameras.SetVrCameras();
        } else if (context == VRContext.CAVE) {
            cameras.SetCaveCameras();
        } else {
            throw new System.Exception("Unknown Context " + context);
        }

        // TODO rework
        //UnityStandardAssets.Vehicles.Car.CarController controller = car.GetComponent<UnityStandardAssets.Vehicles.Car.CarController>();
        //controller.enabled = false;
    }

    // Sets up any modules with additional requirements (e.g. DisplacementModule needs a target)
    private void SetupAnalytics() {

        analytics.ClearTracking();
        analytics.SetupTracking();
        
        // Ensure modules have required stuff, if equipt
        DisplacementModule dm = analytics.gameObject.GetComponent<DisplacementModule>();
        if(dm != null) {
            dm.SetTarget(car);
        }
    }

    // Stops tracking, destroys the car, and displays the results menu
    public void EndTest(string reason = "Unknown Reason.") {
        Debug.Log("Test terminated for :" + reason);
        analytics.StopTracking();
        Destroy(car);
        ShowResultsMenu();
    }

    // Returns if the write was successful
    public bool SaveResults(string input) {
        string filename = FilenameFor(input);
        return WriteToFile(filename, analytics.Data());
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
    public void ShowMainMenu() {
        SetMenuEnabled(startMenu, true);
        SetMenuEnabled(observerMenu, false);
        SetMenuEnabled(resultsMenu, false);
    }

    public void ShowObserverMenu() {
        SetMenuEnabled(startMenu, false);
        SetMenuEnabled(observerMenu, true);
        SetMenuEnabled(resultsMenu, false);
    }

    public void ShowResultsMenu() {
        SetMenuEnabled(startMenu, false);
        SetMenuEnabled(observerMenu, false);
        SetMenuEnabled(resultsMenu, true);
    }

    private void SetMenuEnabled(CanvasGroup canvas, bool enabled) {
        canvas.interactable = enabled;
        canvas.blocksRaycasts = enabled;
        canvas.alpha = enabled ? 1 : 0;
    }
}
