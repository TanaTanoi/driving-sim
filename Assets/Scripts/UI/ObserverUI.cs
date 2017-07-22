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

    public Text consoleBox;
    private ScrollRect scroll;

    private TestManager manager;

    public ObserverCameraController observerCam;

    private bool lockConsoleScrollbar = true;

    void Start() {
        manager = FindObjectOfType<TestManager>();
        scroll = GetComponentInChildren<ScrollRect>();
    }

    public void CameraMovePressed(string dir) {
        switch(dir) {
            case UP:
                observerCam.MoveUp();
                break;
            case DOWN:
                observerCam.MoveDown();
                break;
            case LEFT:
                observerCam.MoveLeft();
                break;
            case RIGHT:
                observerCam.MoveRight();
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
        // TODO
    }

    public void EndTestPressed() {
        manager.EndTest("Observer Termination.");
    }

    public void SetConsoleText(string text) {
        consoleBox.text = text;
    }
}
