using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    AudioSource pickupSound;

	// Use this for initialization
	void Start () {
        pickupSound = GetComponent<AudioSource>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnTriggerEnter(Collider other) {
        pickupSound.Play();
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, pickupSound.clip.length);
    }
}
