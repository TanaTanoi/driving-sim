using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float moveSpeed = 1;
    public float rotateSpeed = 1;
    public float pitch = -20;
    public float yaw = 138;

    // Use this for initialization
    void Start () {
        pitch = transform.rotation.eulerAngles.x;
        yaw = transform.rotation.eulerAngles.y;
        //transform.rotation = transform.rotation.eulerAngles;// Quaternion.Euler(pitch, yaw, 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            // move
            transform.position += transform.forward * moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
            transform.position += transform.right * moveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
            if (Input.GetKey(KeyCode.E)) transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.Q)) transform.position -= Vector3.up * moveSpeed * Time.deltaTime;
        }

        else
        {
            // move
            pitch -= rotateSpeed * Time.deltaTime * Input.GetAxis("Vertical");
            yaw += rotateSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        }

    }
}
