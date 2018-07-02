using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[Serializable]
public class PrefabDataHolder : MonoBehaviour {
    
    public WorldSerialization.PrefabData prefabData;
    public bool spawnOnPlay;


	void Update ()
    {
        prefabData.position = gameObject.transform.position - MapIO.getMapOffset();
        prefabData.rotation = transform.rotation;
        prefabData.scale = transform.localScale;
    }

}

