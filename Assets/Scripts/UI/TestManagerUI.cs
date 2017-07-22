using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(TestManager))]
public class TestManagerUI : MonoBehaviour {

    public Text saveResultsTextbox;
    public SimulatorSettingsStore settings;
    public Dropdown layoutSelectionDropdown;

    private TestManager manager;
    // CONTEXT IDS: 0 HMD, 1 CAVE, 2 SINGLE
    private int context = VRContext.SINGLE;

    // What Layout to use 
    private int layout = 0;

	// Use this for initialization
	void Start () {
        PopulateLayoutMenu();

        manager = GetComponent<TestManager>();
        manager.ShowMainMenu();
	}

    public void StartTestPressed() {
        manager.StartTest(context, settings.maps[layout]);
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

    // Sets the dropdown names based on the images provided.
    private void PopulateLayoutMenu() {
        layoutSelectionDropdown.ClearOptions();
        List<string> layoutNames = new List<string>();
        foreach(Texture2D map in settings.maps) {
            layoutNames.Add(map.name);
        }
        layoutSelectionDropdown.AddOptions(layoutNames);
    }
}
