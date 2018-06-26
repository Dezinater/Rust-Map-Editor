using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrefabDataHolder))]
public class PrefabDataHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PrefabDataHolder myTarget = (PrefabDataHolder)target;
        if (myTarget.prefabData == null)
            return;

        EditorGUILayout.LabelField("Category", myTarget.prefabData.category);
        EditorGUILayout.LabelField("Id", myTarget.prefabData.id+"");
    }
}

