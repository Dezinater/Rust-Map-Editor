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
            blob.Load(loadFile);
            script.Load(blob);
        }

        GUILayout.Label("Save Map", EditorStyles.boldLabel);
        if (GUILayout.Button("Export .map file"))
        {
            saveFile = UnityEditor.EditorUtility.SaveFilePanel("Export Map File", saveFile, mapName, "map");
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
        
        GUILayout.Label("Land Option", EditorStyles.boldLabel);
        string oldLandLayer = script.landLayer;
        string[] options = { "Ground", "Biome", "Alpha", "Topology" };
        script.landSelectIndex = EditorGUILayout.Popup("Select Land Layer:", script.landSelectIndex,  options);
        script.landLayer = options[script.landSelectIndex];
        if (script.landLayer != oldLandLayer)
        {
            script.changeLandLayer();
            Repaint();
        }
        if (script.landLayer.Equals("Topology"))
        {
            GUILayout.Label("Topology Option", EditorStyles.boldLabel);
            script.oldTopologyLayer = script.topologyLayer;
            script.topologyLayer = (TerrainTopology.Enum)EditorGUILayout.EnumPopup("Select Topology Layer:", script.topologyLayer);
            if (script.topologyLayer != script.oldTopologyLayer)
            {
                //script.saveTopologyLayer();
                script.changeLandLayer();
                Repaint();
            }
        }
    }
}
