﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AnalyticModule : MonoBehaviour {

    public const string DATA_SEPERATOR = ", ";

    // Tells the module to begin keeping track of what it wants
    public abstract void StartTracking();

    // Tells the module to stop keeping track of what it wants
    public abstract void StopTracking();

    // The name of the calculation
    public abstract string AnalyticName();

    // The value of this calculation
    public abstract string AnalyticValue();

    // The total data stored by this module. Can include more than Name and Value
    public abstract string AnalyticData();
}