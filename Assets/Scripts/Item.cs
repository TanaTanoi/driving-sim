using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

    AudioSource pickupSound;
    ParticleSystem ps;

    private bool destroy = false;

	// Use this for initialization
	void Start () {
        pickupSound = GetComponent<AudioSource>();
        ps = GetComponentInChildren<ParticleSystem>();
	}

    public void FixedUpdate() {
        if(destroy && ps.particleCount < 10) {
            Destroy(gameObject, pickupSound.clip.length);
        }
    }

    public void OnTriggerEnter(Collider other) {
        FindObjectOfType<ItemsCollectedModule>().CollectItem();
        ps.Stop();
        pickupSound.Play();
        GetComponent<Renderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        // Once the particles are removed, the item destroys itself.
        // Allows a clean disapear.
        destroy = true;
    }
}
