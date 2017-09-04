using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//TODO remove this?
[CustomEditor (typeof (IslandGenerator))]
public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        IslandGenerator generator = (IslandGenerator)target;

        if (GUILayout.Button("Generate"))
        {
            generator.GenerateIsland();
            EditorUtility.SetDirty(target);
        }
    }

}
