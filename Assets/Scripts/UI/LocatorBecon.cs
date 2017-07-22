using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class LocatorBecon : MonoBehaviour {

    private ParticleSystem ps;

    void Start() {
        ps = GetComponent<ParticleSystem>();
    }
    
    public void EnableLocator() {
        ps.Play();
    }

    public void DisableLocator() {
        ps.Stop();
    }
}
