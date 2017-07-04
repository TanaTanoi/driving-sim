using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SmallCityBuilder))]
public class SmallCityBuilderEditor : Editor {

     public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SmallCityBuilder block = (SmallCityBuilder)target;
        if(GUILayout.Button("Generate"))
        {
            block.BuildCity();
        }
    }
}
