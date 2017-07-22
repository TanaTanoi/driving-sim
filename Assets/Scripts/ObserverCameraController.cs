using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ObserverCameraController : MonoBehaviour {

    private Vector3 origin;
    private float originalSize;
    private const float MOVE_SPEED = 1f;
    private const float ZOOM_SPEED = 5;

    private Vector3 desiredPos;
    private float desiredSize;

    private Camera cam;

    void Start() {
        cam = GetComponent<Camera>();
        origin = transform.position;
        desiredPos = origin;
        originalSize = cam.orthographicSize;
        desiredSize = originalSize;
    }

    void Update() {
        transform.position += (desiredPos - transform.position) * 0.5f;
        cam.orthographicSize += (desiredSize - cam.orthographicSize) * 0.5f;
    }

    public void SetDesiredLocation(Vector3 desired) {
        desiredPos = new Vector3(desired.x, origin.y, desired.z);
    }

    public void MoveUp(float multiplier = 1) {
        Move(transform.up, multiplier * MOVE_SPEED);    
    }
    public void MoveDown(float multiplier = 1) {
        Move(-transform.up, multiplier * MOVE_SPEED);    
    }

    public void MoveRight(float multiplier = 1) {
        Move(transform.right, multiplier * MOVE_SPEED);    
    }

    public void MoveLeft(float multiplier = 1) {
        Move(-transform.right, multiplier * MOVE_SPEED);    
    }

    public void ZoomIn() {
        desiredSize += ZOOM_SPEED;
    }

    public void ZoomOut() {
        desiredSize -= ZOOM_SPEED;
    }

    public void Center() {
        desiredSize = originalSize;
        desiredPos = origin;
    }

    private void Move(Vector3 dir, float multiplier) {
        desiredPos += dir * multiplier;
    }
    
}
