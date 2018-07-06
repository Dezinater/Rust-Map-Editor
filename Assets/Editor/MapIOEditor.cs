using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapIO))]
public class MapIOEditor : Editor
{
    string loadFile = "";

    string saveFile = "";
    string mapName = "";

    string bundleFile = "No bundle file selected";

    public override void OnInspectorGUI()
    {
        MapIO script = (MapIO)target;

        GUILayout.Label("Load Map", EditorStyles.boldLabel);
        if (GUILayout.Button("Import .map file"))
        {
            loadFile = UnityEditor.EditorUtility.OpenFilePanel("Import Map File", loadFile, "map");

            var blob = new WorldSerialization();
            Debug.Log("Importing map " + loadFile);
            if (loadFile == "")
            {
                Debug.LogError("Empty load path");
                return;
            }
            blob.Load(loadFile);
            script.Load(blob);
        }

        GUILayout.Label("Save Map", EditorStyles.boldLabel);
        if (GUILayout.Button("Export .map file"))
        {
            saveFile = UnityEditor.EditorUtility.SaveFilePanel("Export Map File", saveFile, mapName, "map");
            if(saveFile == "")
            {
                Debug.LogError("Empty save path");
            }
            Debug.Log("Exported map " + saveFile);
            script.Save(saveFile);
        }
/*
        GUILayout.Label("Select Bundle file", EditorStyles.boldLabel);

        bundleFile = GUILayout.TextField(bundleFile);
        if (GUILayout.Button("Select Bundle file (Rust\\Bundles\\Bundles)"))
        {
            bundleFile = UnityEditor.EditorUtility.OpenFilePanel("Select Bundle file (Rust\\Bundles\\Bundles)", bundleFile, "map");
        }
*/
        
       

        GUILayout.Label("Heightmap Options", EditorStyles.boldLabel);

        GUILayout.Label("Land Heightmap Offset (Move Land to correct position)");
        if (GUILayout.Button("Click here to bake heightmap values"))
        {
            script.offsetHeightmap();
        }

        GUILayout.Label("Land Heightmap Scale");
        script.scale = float.Parse(GUILayout.TextField(script.scale + ""));
        script.scale = GUILayout.HorizontalSlider(script.scale, 0.1f, 2);
        if (GUILayout.Button("Scale Map"))
        {
            script.scaleHeightmap();
            script.scale = 1f;
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Rotate CW"))
        {
            script.rotateHeightmap(true);
        }
        if (GUILayout.Button("Rotate CCW"))
        {
            script.rotateHeightmap(false);
        }

        if (GUILayout.Button("Flip Heightmap"))
        {
            script.flipHeightmap();
        }
        if (GUILayout.Button("Transpose Heightmap"))
        {
            script.transposeHeightmap();
        }
        EditorGUILayout.EndHorizontal();
    }
}
