using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsCollectedModule : AnalyticModule {

    private int itemsCollected = 0;

    public int ItemsCollected { get { return itemsCollected; } }

    public void CollectItem() {
        itemsCollected++;
    }
    public override void StartTracking() {
        itemsCollected = 0;
    }

    public override void StopTracking() {
    }

    public override string AnalyticName() {
        return "Items Collected";
    }

    public override string AnalyticValue() {
        return ItemsCollected.ToString();
    }

    public override string AnalyticData() {
        return AnalyticName() + " : " + AnalyticValue();
    }

}
