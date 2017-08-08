using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class VRContext {
    public const int SINGLE = 0;
    public const int HMD = 1;
    public const int CAVE = 2;

    public static string WordForContextID(int id) {
        switch(id) {
            case 0:
                return "Single Screen";
            case 1:
                return "Headmounted Display";
            case 2:
                return "CAVE";
            default:
                throw new Exception("Unknown Context ID " + id);
        }
    }

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

    private string terminationReason = "";
    private string layoutName = "";
    private int contextID;

    string output;
    string stack;

    void Start() {
        startMenu.gameObject.SetActive(true);
        resultsMenu.gameObject.SetActive(true);
        observerMenu.gameObject.SetActive(true);
    }

    // Sets up the test with analytics and a car, then starts it after a delay
    public void StartTest(int context, Texture2D layout) {

        //Application.RegisterLogCallback(HandleLog); // for Debugging in build mode

        layoutName = layout.name;
        contextID = context;

        city.map = layout;
        city.BuildCity();

        ShowObserverMenu();

        SetupCar(context);
        SetupAnalytics();

        StartCoroutine(StartWhenReady());
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        output += logString;
        stack = stackTrace;
    }

    void OnGUI() {
        // For Debugging only
        //GUI.Label(new Rect(0, 0, Screen.width, Screen.height), output);
    }

    IEnumerator StartWhenReady() {
        while (!CarReady()) {
            yield return new WaitForEndOfFrame();
        }

        Countdown cd = car.GetComponentInChildren<Countdown>();
        cd.StartCountdown(3, EnableCarControl);
    }

    public void EnableCarControl() {
        SetCarActive(true);
        analytics.StartTracking();
    }

    private void SetCarActive(bool enabled) {
        CarUserControl().canMove = enabled;
        car.GetComponent<Rigidbody>().isKinematic = !enabled;
    }

    private bool CarReady() {
        return CarUserControl().ready;
    }

    public void EnableKeyboardOverride(bool enabled) {
        CarUserControl().keyboardOverride = enabled;
    }

    private UnityStandardAssets.Vehicles.Car.CarUserControl CarUserControl() {
        return car.GetComponent<UnityStandardAssets.Vehicles.Car.CarUserControl>();
    }

    private void SetupCar(int context) {
        // Disable audio listener on the main cam
        ObserverCameraController cam = GameObject.FindObjectOfType<ObserverCameraController>();
        cam.GetComponent<AudioListener>().enabled = false;
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

        SetCarActive(false);
    }

    // Sets up any modules with additional requirements (e.g. DisplacementModule needs a target)
    private void SetupAnalytics() {

        analytics.SetupTracking();

        VRCameraPod cameras = car.GetComponentInChildren<VRCameraPod>();

        // Ensure modules have required stuff, if equipt
        DisplacementModule dm = analytics.gameObject.GetComponent<DisplacementModule>();
        if(dm != null) {
            dm.SetTarget(car);
        }

        HeadTurnsModule htm = analytics.gameObject.GetComponent<HeadTurnsModule>();
        if(htm != null){
            htm.SetHeadset(cameras.GetHeadsetTransform());
            htm.enabled = contextID == VRContext.HMD;
        }
    }

    // Cleans the test by removing any test specific items
    public void CleanupTest() {

        observerMenu.GetComponent<ObserverUI>().SetGoToResultsButtonActive(false);
        analytics.ClearTracking();
        if(car != null) {
            ObserverCameraController cam = GameObject.FindObjectOfType<ObserverCameraController>();
            cam.GetComponent<AudioListener>().enabled = true;
            Destroy(car);
        }
    }

    // Stops tracking, destroys the car, and displays the results menu
    public void EndTest(string reason = "Unknown Reason.") {
        terminationReason = reason;
        analytics.StopTracking();
        SetCarActive(false);
        observerMenu.GetComponent<ObserverUI>().SetGoToResultsButtonActive(true);
        resultsMenu.GetComponent<ResultsUI>().SetOverviewContent(FinalResultsOverviewText(reason));
        ShowResultsMenu();
    }


    // Consolidate with the Observer UI console display
    private string FinalResultsOverviewText(string terminationReason) {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("Termination Reason: " + terminationReason);
        sb.AppendLine(String.Format("{0, -15}{1, 7}", "Context", VRContext.WordForContextID(contextID)));
        sb.AppendLine(String.Format("{0, -15}{1, 7}", "Layout", layoutName));

        string[] names = analytics.Names();
        string[] values = analytics.Values();
        for(int i = 0; i < names.Length; i++) {
            sb.AppendLine(String.Format("{0, -15}{1, 7}", names[i], values[i]));
        }

        return sb.ToString();
    }

    // Returns if the write was successful
    public bool SaveResults(string input) {
        ResultWriter writer = new ResultWriter(input, VRContext.WordForContextID(contextID), layoutName);

        writer.AddData("Overview", FinalResultsOverviewText(terminationReason));

        string[] names = analytics.Names();
        string[] data = analytics.Data();
        for(int i = 0; i < names.Length; i++) {
            writer.AddData(names[i], data[i]);
        }

        return writer.WriteData();
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

    // Potentially refactor to use their own scripts with a enable/disable on the UI scripts?
    private void SetMenuEnabled(CanvasGroup canvas, bool enabled) {
        canvas.interactable = enabled;
        canvas.blocksRaycasts = enabled;
        canvas.alpha = enabled ? 1 : 0;
    }

    public void TakeScreenshotOfMap(string filepath) {
        
        observerMenu.GetComponent<ObserverUI>().StopCameraFollowCar();


        StartCoroutine(TakeScreenshotWhenAvailable(filepath));
    }

    private IEnumerator TakeScreenshotWhenAvailable(string filepath) {
        ObserverCameraController cam = GameObject.FindObjectOfType<ObserverCameraController>();
        cam.Center();
        ShowObserverMenu();
        yield return new WaitForSeconds(0.3f);
        //Application.CaptureScreenshot(filepath);
        yield return new WaitForEndOfFrame();
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 16);
        cam.GetComponent<Camera>().targetTexture = rt;
        RenderTexture.active = rt;

        cam.GetComponent<Camera>().Render();
        Texture2D t = new Texture2D(Screen.width, Screen.height);
        t.ReadPixels(new Rect(0,0, Screen.width, Screen.height), 0,0);
        t.Apply();

        RenderTexture.active = null;
        cam.GetComponent<Camera>().targetTexture = null;

        Byte[] bytes = t.EncodeToPNG();
        Destroy(t);
        System.IO.File.WriteAllBytes(filepath, bytes);

        yield return new WaitForSeconds(0.1f);
        ShowResultsMenu();
    }
}
