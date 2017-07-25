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

    private const float MOVE_ACCELERATION_TIME_THRESHOLD = 1f;
    private const float MOVE_ACCELERATION_SPEED = 0.7f;
    private const float MOVE_ACCELERATION_DEFAULT = 1f;

    public Text consoleBox;
    private ScrollRect scroll;

    private TestManager manager;
    public ObserverCameraController observerCam;
    public Button goToResultsButton;

    private float moveAcceleration = 1;
    private float moveAccelerationTime = 0;
    private bool lockConsoleScrollbar = true;

    private bool locatorBeconToggle = false;
    private LocatorBecon becon;

    private bool followCar = false;

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
        if(Time.time - moveAccelerationTime < MOVE_ACCELERATION_TIME_THRESHOLD) {
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
        if(dir == IN) {
            observerCam.ZoomIn();
        } else {
            observerCam.ZoomOut();
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

    private void FindBecon() {
        if(becon == null) {
            becon = FindObjectOfType<LocatorBecon>();
        }
    }
}
