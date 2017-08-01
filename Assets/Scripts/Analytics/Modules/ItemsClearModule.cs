using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemsCollectedModule))]
public class ItemsClearModule : TerminationModule {
   [Range(0,100)] public float targetProgress = 50;

    private ItemsCollectedModule itemsMod;

    public override string TerminationReason {
        get {
            return "Target percentage ("+ targetProgress +"% of " + TotalItems() +") of items collected.";
        }
    }

    public new void Start() {
        base.Start();
        itemsMod = GetComponent<ItemsCollectedModule>();
    }

    public override bool TestOver() {
        return (float)CollectedItemsCount() / (float)TotalItems() >= (targetProgress / 100f)   ;
    }

    public int CollectedItemsCount() {
        return itemsMod.ItemsCollected;
    }

    public int TotalItems() {
        return itemsMod.TotalItems;
    }

    public override void Setup() {
    }
}
