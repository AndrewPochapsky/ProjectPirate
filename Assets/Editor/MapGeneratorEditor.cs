using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (IslandGenerator))]
public class MapGeneratorEditor : Editor {

    public override void OnInspectorGUI()
    {
        IslandGenerator generator = (IslandGenerator)target;

        if (DrawDefaultInspector())
        {
            if (generator.AutoUpdate)
            {
                generator.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            generator.GenerateMap();
            EditorUtility.SetDirty(target);
        }
    }

}
