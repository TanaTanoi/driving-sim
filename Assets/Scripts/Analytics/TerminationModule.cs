using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TerminationModule : MonoBehaviour {

    public abstract string TerminationReason { get; }

    public TestManager manager;

    private AnalyticsController controller;

    public void Start() {
        controller = GetComponent<AnalyticsController>();
        if(controller == null) {
            throw new System.Exception("Termination Modules require AnalyticsControllers to be present!");
        }
    }

	// Update is called once per frame
	void Update () {
		if(controller.Tracking() && TestOver()) {
            TerminateTest();
        }
	}

    public void TerminateTest() {
        manager.EndTest(TerminationReason);
    }

    public abstract bool TestOver();
}
