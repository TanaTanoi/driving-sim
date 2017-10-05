using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour {
    TextMesh text;
    float deltaTime = 0;
    float minimum = 1200;
    float maximum = 0;
	// Use this for initialization
	void Start () {
        text = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        if(fps < minimum)
        	minimum = fps;
    	if(fps > maximum)
    		maximum = fps;

        text.text = String.Format("{0:#.##}\n{1:#.##}\n{2:#.##}", minimum, fps, maximum);
    }
}
