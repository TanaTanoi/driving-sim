using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AnalyticsController))]
public abstract class TerminationModule : MonoBehaviour {

    public abstract string TerminationReason { get; }

    public TestManager manager;

    private AnalyticsController controller;

    public void Start() {
        controller = GetComponent<AnalyticsController>();
    }

    public abstract void Setup();

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
