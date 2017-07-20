using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemsCollectedModule))]
public class ItemsClearModule : TerminationModule {
   [Range(0,100)] public float targetProgress = 50;

    private ItemsCollectedModule itemsMod;

    private int totalItems;

    public int TotalItems { get { return totalItems; } }

    public override string TerminationReason {
        get {
            return "Target percentage ("+ targetProgress +"% of " + totalItems +") of items collected.";
        }
    }

    public new void Start() {
        base.Start();
        itemsMod = GetComponent<ItemsCollectedModule>();
    }

    public override bool TestOver() {
        return (float)CollectedItemsCount() / (float)totalItems >= (targetProgress / 100f)   ;
    }

    public int CollectedItemsCount() {
        return itemsMod.ItemsCollected;
    }

    public override void Setup() {
        totalItems = FindObjectsOfType<Item>().Length;
        Debug.Log("Getting this many " + totalItems);
    }
}
