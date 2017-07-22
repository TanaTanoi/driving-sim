using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObserverUI : MonoBehaviour {
    public enum Direction { UP, DOWN, LEFT, RIGHT }

    public Text consoleBox;
    private ScrollRect scroll;

    private TestManager manager;

    private bool lockConsoleScrollbar = true;

    void Start() {
        manager = FindObjectOfType<TestManager>();
        scroll = GetComponentInChildren<ScrollRect>();
    }

    public void CameraMovePressed(Direction dir) {
        // TODO
    }

    public void CameraZoomPressed(Direction dir) {
        if(!(dir == Direction.UP || dir == Direction.DOWN)) {
            Debug.Log("Invalid option for camera zoom!");
            return;
        }                                                
        // TODO
    }

    public void CameraCenterPressed() {
        // TODO
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
