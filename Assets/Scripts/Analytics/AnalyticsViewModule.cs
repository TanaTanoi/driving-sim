using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnalyticsViewModule : MonoBehaviour {

    public abstract void StartDisplay();

    public abstract void StopDisplay();

    public abstract void ClearDisplay();
}
