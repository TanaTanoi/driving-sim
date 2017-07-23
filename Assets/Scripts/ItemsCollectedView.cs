using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsCollectedView : MonoBehaviour {

    ItemsClearModule itemsMod;
    TextMesh text;
	// Use this for initialization
	void Start () {
        itemsMod = FindObjectOfType<ItemsClearModule>();
        text = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if(itemsMod != null) {
            text.text = PaddedNumber(itemsMod.CollectedItemsCount()) + " / " + PaddedNumber(itemsMod.TotalItems);
        } else {
            text.text = "Error";
        }
	}

    private string PaddedNumber(int input) {
        return input.ToString("D3");
    }
}
