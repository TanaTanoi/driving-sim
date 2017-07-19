using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(TestManager))]
public class TestManagerUI : MonoBehaviour {

    private TestManager manager;

    public Text saveResultsTextbox;

    // CONTEXT IDS: 0 HMD, 1 CAVE, 2 SINGLE
    private int context = VRContext.SINGLE;

    // What Layout to use 
    private int layout = 0;

	// Use this for initialization
	void Start () {
        manager = GetComponent<TestManager>();
        manager.ShowMainMenu();
	}

    public void StartTestPressed() {
        manager.StartTest(context, layout);
    }

    public void SaveResultsPressed(InputField input) {
        if(manager.SaveResults(input.text)) {
            input.text = "";
            saveResultsTextbox.text = "Save successful!";
        } else {
            saveResultsTextbox.text = "Error writing results!";
        }
    }

    public void ContextDropdownChanged(Dropdown contextID) {
        context = contextID.value;
    }

    public void LayoutDropdownChanged(Dropdown layoutID) {
        layout = layoutID.value;
    }

    public void RestartPressed() {
        ResetResultsMenu();
        manager.ShowMainMenu();
    }

    private void ResetResultsMenu() {
        saveResultsTextbox.text = "";
    }
}
