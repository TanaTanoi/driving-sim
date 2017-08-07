using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserGridProximity : MonoBehaviour {
    bool nearbyPlayer = false;
    GameObject player;
    Material mat;
    private AudioSource hum;
    private float radius;
	// Use this for initialization
	void Start () {
        mat = GetComponent<Renderer>().material;
        radius = GetComponent<SphereCollider>().radius * transform.localScale.x;
        hum = GetComponent<AudioSource>();
        SetAlpha(0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	    if(nearbyPlayer) {
            // 1 if close
            float proximity = (radius - (player.transform.position - transform.position).magnitude) / radius;
            SetAlpha(proximity);
            hum.volume = proximity;
        }	
	}

    void SetAlpha(float a) {
        Color x = mat.color;
        x.a = a;
        mat.color = x;
    }

    void OnTriggerEnter(Collider other) {
        if(other.transform.root.CompareTag("Player")){
        Debug.Log("Player here");
            nearbyPlayer = true;
            player = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other) {
        if(other.transform.root.CompareTag("Player")){
            nearbyPlayer = false;
            player = null;
        }
    }
}
