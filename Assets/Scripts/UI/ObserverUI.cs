using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObserverUI : MonoBehaviour {
    public const string UP = "UP";
    public const string DOWN = "DOWN";
    public const string LEFT = "LEFT";
    public const string RIGHT = "RIGHT";

    public const string IN = "IN";
    public const string OUT = "OUT";

    private const float ACCELERATION_TIME_THRESHOLD = 0.7f;
    private const float MOVE_ACCELERATION_SPEED = 4f;
    private const float MOVE_ACCELERATION_DEFAULT = 2f;

    private const float ZOOM_ACCELERATION_SPEED = 0.2f;
    private const float ZOOM_MULTIPLIER_DEFAULT = 1;

    public Text consoleBox;
    private ScrollRect scroll;

    private TestManager manager;
    public ObserverCameraController observerCam;
    public Button goToResultsButton;

    private float moveAcceleration = MOVE_ACCELERATION_DEFAULT;
    private float moveAccelerationTime = 0;
    private float zoomAcceleration = ZOOM_MULTIPLIER_DEFAULT;
    private float zoomAccelerationTime = 0;
    private bool lockConsoleScrollbar = true;

    private bool locatorBeconToggle = false;
    private LocatorBecon becon;

    private bool followCar = false;

    private bool overrideCar = false; // Enables keyboard control

    void Start() {
        manager = FindObjectOfType<TestManager>();
        scroll = GetComponentInChildren<ScrollRect>();
    }

    void FixedUpdate() {
        if(followCar) {
            FindBecon(); // Ensures becon exists
            observerCam.SetDesiredLocation(becon.transform.position);

        }
    }

    // TODO add key movement as well
    public void CameraMovePressed(string dir) {
        if(Time.time - moveAccelerationTime < ACCELERATION_TIME_THRESHOLD) {
            moveAcceleration += MOVE_ACCELERATION_SPEED;
        } else {
            moveAcceleration = MOVE_ACCELERATION_DEFAULT;
        }
        moveAccelerationTime = Time.time;

        switch(dir) {
            case UP:
                observerCam.MoveUp(moveAcceleration);
                break;
            case DOWN:
                observerCam.MoveDown(moveAcceleration);
                break;
            case LEFT:
                observerCam.MoveLeft(moveAcceleration);
                break;
            case RIGHT:
                observerCam.MoveRight(moveAcceleration);
                break;
            default:
                throw new System.Exception("Direction " + dir + " not supported in Camera Move");
        }                  
    }

    public void CameraZoomPressed(string dir) {
        if(!(dir == OUT || dir == IN)) {
            Debug.Log("Invalid option for camera zoom!");
            return;
        }                                                

        if(Time.time - zoomAccelerationTime < ACCELERATION_TIME_THRESHOLD) {
            zoomAcceleration += ZOOM_ACCELERATION_SPEED;
        } else {
            zoomAcceleration = ZOOM_MULTIPLIER_DEFAULT;
        }
        zoomAccelerationTime= Time.time;

        if(dir == IN) {
            observerCam.ZoomIn(zoomAcceleration);
        } else {
            observerCam.ZoomOut(zoomAcceleration);
        }
    }

    public void CameraCenterPressed() {
        observerCam.Center();
    }

    public void CameraHighlightCarPressed() {
        FindBecon();
        locatorBeconToggle = !locatorBeconToggle;
        if(locatorBeconToggle) {
            becon.EnableLocator();
        } else {
            becon.DisableLocator();
        }
    }

    public void SetGoToResultsButtonActive(bool enabled) {
        goToResultsButton.gameObject.SetActive(enabled);
    }

    public void GoToResultsPressed() {
        manager.ShowResultsMenu();
    }

    public void CameraFollowCarPressed() {
        followCar = !followCar;
    }

    public void EndTestPressed() {
        manager.EndTest("Observer Termination.");
    }

    public void SetConsoleText(string text) {
        consoleBox.text = text;
    }

    public void OverrideCarPressed() {
        overrideCar = !overrideCar;
        manager.EnableKeyboardOverride(overrideCar);
    }

    private void FindBecon() {
        if(becon == null) {
            becon = FindObjectOfType<LocatorBecon>();
        }
    }
}
