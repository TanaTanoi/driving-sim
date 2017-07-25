using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultsUI : MonoBehaviour {

    public TestManager manager;

    public void ViewMapPressed() {
        manager.ShowObserverMenu();
    }
}
