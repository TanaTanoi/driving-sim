using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(TestManager))]
public class TestManagerUI : MonoBehaviour {

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

    public void ContextDropdownChanged(Dropdown contextID) {
        context = contextID.value;
    }

    public void LayoutDropdownChanged(Dropdown layoutID) {
        layout = layoutID.value;
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
