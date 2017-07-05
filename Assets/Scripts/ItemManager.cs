using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour {

    public List<Item> items;

	// Use this for initialization
	void Start () {
        Reset();
	}
	
    public void Reset() {
        items = new List<Item>();
        items.AddRange(GameObject.FindObjectsOfType<Item>());
    }

	// Update is called once per frame
	void Update () {
		
	}


}
