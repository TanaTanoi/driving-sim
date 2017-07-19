using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsClearModule : TerminationModule {
   [Range(0,100)] public float targetProgress = 50;

    private int collectedItems = 0;
    private int totalItems;

    public override string TerminationReason {
        get {
            return "Target percentage ("+ targetProgress +"% of " + totalItems +") of items collected.";
        }
    }

    public new void Start() {
        base.Start();
        totalItems = FindObjectsOfType<Item>().Length;
        Debug.Log("Getting this many " + totalItems);
    }

    public override bool TestOver() {
        return (float)CollectedItemsCount() / (float)totalItems > (targetProgress / 100f)   ;
    }

    private int CollectedItemsCount() {
        return totalItems - FindObjectsOfType<Item>().Length;
    }
}
