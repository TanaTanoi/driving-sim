using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheelMovement : MonoBehaviour {

    private Quaternion restPosition;

    public float sensitivity = 20;

    private float desiredAngle = 0;
    private float currentAngle = 0;


	// Use this for initialization
	void Start () {
        restPosition = transform.localRotation;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        desiredAngle = Input.GetAxisRaw("ThrustmasterWheel") * -1;
        currentAngle += (desiredAngle - currentAngle) * 0.4f;
        transform.localRotation = restPosition * Quaternion.Euler(Vector3.forward * currentAngle * sensitivity);
	}
}
