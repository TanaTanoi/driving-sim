using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsClearModule : TerminationModule {
    public int targetItems = 3;

    private int collectedItems = 0;
    private int startItems;

    public new void Start() {
        base.Start();
        startItems = FindObjectsOfType<Item>().Length;
        Debug.Log("Getting this many " + startItems);
    }

    public override bool TestOver() {
        return startItems - FindObjectsOfType<Item>().Length >= targetItems;
    }
}
