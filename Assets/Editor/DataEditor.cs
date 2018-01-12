using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Data))]
public class DataEditor : Editor {

	public override void OnInspectorGUI()
    {
		DrawDefaultInspector();
        if(GUILayout.Button("Reset"))
		{
			Data data = (Data)target;
			data.ResetData();
        }
    }
}
