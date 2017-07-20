using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores variables such as maps and car speed.
[CreateAssetMenu(fileName = "Data", menuName = "DrivingSimulator/SimulationSettings", order = 1)]
public class SimulatorSettingsStore : ScriptableObject {
    public Texture2D[] maps;
}
