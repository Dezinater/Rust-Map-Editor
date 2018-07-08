using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LayerOptions))]
public class LayerOptionEditor : Editor
{
    MapIO mapIO;

    public override void OnInspectorGUI()
    {
        LayerOptions script = (LayerOptions)target;
        if (mapIO == null)
            mapIO = GameObject.FindGameObjectWithTag("MapIO").GetComponent<MapIO>();

        GUILayout.Label("Land Options", EditorStyles.boldLabel);

        //GUILayout.Label("Show Bounds", EditorStyles.boldLabel);
        LayerOptions.showBounds = EditorGUILayout.Toggle("Show Bounds", LayerOptions.showBounds);

        string oldLandLayer = mapIO.landLayer;
        string[] options = { "Ground", "Biome", "Alpha", "Topology" };
        mapIO.landSelectIndex = EditorGUILayout.Popup("Select Land Layer:", mapIO.landSelectIndex, options);
        mapIO.landLayer = options[mapIO.landSelectIndex];
        if (mapIO.landLayer != oldLandLayer)
        {
            mapIO.changeLandLayer();
            Repaint();
        }
        if (mapIO.landLayer.Equals("Topology"))
        {
            GUILayout.Label("Topology Option", EditorStyles.boldLabel);
            mapIO.oldTopologyLayer = mapIO.topologyLayer;
            mapIO.topologyLayer = (TerrainTopology.Enum)EditorGUILayout.EnumPopup("Select Topology Layer:", mapIO.topologyLayer);
            if (mapIO.topologyLayer != mapIO.oldTopologyLayer)
            {
                //script.saveTopologyLayer();
                mapIO.changeLandLayer();
                Repaint();
            }
            if (GUILayout.Button("Clear topology layer"))
            {
                mapIO.clearTopologyLayer();
            }
        }
    }
}
