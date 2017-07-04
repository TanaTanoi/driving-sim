using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Building))]
public class BuildingEditor : Editor {

     public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        Building building = (Building)target;
        if(GUILayout.Button("Regenerate"))
        {
            building.Build();
        }
    }
}
