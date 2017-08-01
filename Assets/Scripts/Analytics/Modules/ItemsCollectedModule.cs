using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ItemsCollectedModule : AnalyticModule {

    private int itemsCollected = 0;
    private List<float> itemCollectionTimes;
    private float startTime = 0;
    private int totalItems;

    public int ItemsCollected { get { return itemsCollected; } }
    public int TotalItems { get { return totalItems; } }

    public void CollectItem() {
        itemsCollected++;
        itemCollectionTimes.Add(Time.time - startTime);
    }

    public override void StartTracking() {
        itemsCollected = 0;
        itemCollectionTimes = new List<float>();
        startTime = Time.time;
        totalItems = FindObjectsOfType<Item>().Length;
    }

    public override void StopTracking() {
    }

    public override string AnalyticName() {
        return "Items Collected";
    }

    public override string AnalyticValue() {
        return itemsCollected.ToString() + " / " + totalItems.ToString();
    }

    public override string AnalyticData() {
        // Outputs when an item was collected
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Items Over Time: (out of " + totalItems + ")");
        sb.AppendLine("ID" + DATA_SEPERATOR + "TIME");

        for(int i = 0; i < itemCollectionTimes.Count; i++) {
            sb.AppendLine(i + DATA_SEPERATOR + itemCollectionTimes[i]);
        }

        return sb.ToString();
    }

}
