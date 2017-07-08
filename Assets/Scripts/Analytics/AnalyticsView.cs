using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsView : MonoBehaviour {
    AnalyticsController controller;
	// Use this for initialization
	void Start () {
        controller = gameObject.GetComponent<AnalyticsController>();
	}
	
	// Update is called once per frame
	void Update () {
        string[] names = controller.Names();
        string[] values = controller.Values();
        for(int i = 0; i < names.Length; i++) {
            Debug.Log(names[i] + " : " + values[i]);
        }
	}
}
