using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUI : MonoBehaviour {
    public Text saveResultsTextbox;
    public TestManager manager;
    public Text resultsOverviewTextbox;

    public void ViewMapPressed() {
        manager.ShowObserverMenu();
    }

    public void RestartPressed() {
        ResetResultsMenu();
        manager.CleanupTest();
        manager.ShowMainMenu();
    }

    public void SetOverviewContent(string s) {
        resultsOverviewTextbox.text = s;
    }

    private void ResetResultsMenu() {
        saveResultsTextbox.text = "";
    }

    public void SaveResultsPressed(InputField input) {
        if(manager.SaveResults(input.text)) {
            input.text = "";
            saveResultsTextbox.text = "Save successful!";
        } else {
            saveResultsTextbox.text = "Error writing results!";
        }
    }
}
