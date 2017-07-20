using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ItemsCollectedView : MonoBehaviour {

    ItemsClearModule itemsMod;
    Text text;
	// Use this for initialization
	void Start () {
        itemsMod = FindObjectOfType<ItemsClearModule>();
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        text.text = "Items: " + itemsMod.CollectedItemsCount() + " / " + itemsMod.TotalItems;
	}
}
