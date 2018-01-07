using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleData))]
public class BattleDataEditor : Editor {

	public override void OnInspectorGUI()
    {
		DrawDefaultInspector();
        if(GUILayout.Button("Reset"))
		{
			BattleData data = (BattleData)target;
			data.ResetData();
        }
    }
}
