using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapLoader : MonoBehaviour{

    public SplatPrototype[] getTextures()
    {
        SplatPrototype[] textures = new SplatPrototype[8];
        for (int i=0;i<textures.Length; i++)
        {
            textures[i] = new SplatPrototype();
        }

        textures[0].texture = Resources.Load<Texture2D>("Textures/dirt");
        textures[1].texture = Resources.Load<Texture2D>("Textures/snow");
        textures[2].texture = Resources.Load<Texture2D>("Textures/sand");
        textures[3].texture = Resources.Load<Texture2D>("Textures/rock");
        textures[4].texture = Resources.Load<Texture2D>("Textures/grass");
        textures[5].texture = Resources.Load<Texture2D>("Textures/forest");
        textures[6].texture = Resources.Load<Texture2D>("Textures/stones");
        textures[7].texture = Resources.Load<Texture2D>("Textures/gravel");

        return textures;
    }

    public void LoadLand()
    {
        
    }

	public void Load(WorldSerialization blob)
    {
        var terrainSize = new Vector3(blob.world.size, 1000, blob.world.size);
        var terrainPosition = 0.5f * terrainSize;
        
        Terrain land = GameObject.FindGameObjectWithTag("Land").GetComponent<Terrain>();
        Terrain water = GameObject.FindGameObjectWithTag("Water").GetComponent<Terrain>();

        WorldConverter.MapInfo terrains = WorldConverter.worldToTerrain(blob);
        
        land.terrainData.heightmapResolution = terrains.resolution;
        land.terrainData.size = terrains.size;

        water.terrainData.heightmapResolution = terrains.resolution;
        water.terrainData.size = terrains.size;
        
        land.terrainData.SetHeights(0, 0, terrains.land.heights);
        water.terrainData.SetHeights(0, 0, terrains.water.heights);

        
        land.terrainData.alphamapResolution = terrains.resolution;
        land.terrainData.size = terrains.size;
        land.terrainData.splatPrototypes = getTextures();
        land.terrainData.SetAlphamaps(0, 0, terrains.splatMap);

        GameObject defaultObj = Resources.Load<GameObject>("Prefabs/DefaultPrefab");
        for(int i = 0; i < terrains.prefabData.Length; i++)
        {
            Vector3 pos = new Vector3(terrains.prefabData[i].position.x, terrains.prefabData[i].position.y, terrains.prefabData[i].position.z);
            Vector3 scale = new Vector3(terrains.prefabData[i].scale.x, terrains.prefabData[i].scale.y, terrains.prefabData[i].scale.z);
            Quaternion rotation = Quaternion.Euler(new Vector3(terrains.prefabData[i].rotation.x, terrains.prefabData[i].rotation.y, terrains.prefabData[i].rotation.z));

            GameObject newObject = Instantiate(defaultObj, pos+terrainPosition, rotation);
            newObject.transform.localScale = scale;
            newObject.GetComponent<PrefabDataHolder>().prefabData = terrains.prefabData[i];
        }

        GameObject pathObj = Resources.Load<GameObject>("Paths/Path");
        for (int i = 0; i < terrains.pathData.Length; i++)
        {
            Vector3 averageLocation = Vector3.zero;
            for (int j = 0; j < terrains.pathData[i].nodes.Length; j++)
            {
                averageLocation += terrains.pathData[i].nodes[j];
            }
            averageLocation /= terrains.pathData[i].nodes.Length;
            GameObject newObject = Instantiate(pathObj, averageLocation + terrainPosition, Quaternion.identity);
            newObject.GetComponent<PathDataHolder>().pathData = terrains.pathData[i];
            newObject.GetComponent<PathDataHolder>().offset = terrainPosition;
        }
    }


    private string filename = string.Empty;
    private string result = string.Empty;
    protected void OnGUI()
    {
        const float padding = 10;

        GUILayout.BeginArea(new Rect(padding, padding, Screen.width - padding - padding, Screen.height - padding - padding));
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Map File");
        filename = GUILayout.TextField(filename, GUILayout.MinWidth(100));
        #if UNITY_EDITOR
        if (GUILayout.Button("Browse")) filename = UnityEditor.EditorUtility.OpenFilePanel("Select Map File", filename, "map");
#endif
        if (GUILayout.Button("Load"))
        {
            var blob = new WorldSerialization();
            Debug.Log("Loading");
           
            blob.Load(filename);

            Load(blob);
       
        }
        if (GUILayout.Button("Save"))
        {
            Terrain terrain = GameObject.FindGameObjectWithTag("Land").GetComponent<Terrain>();
            Terrain water = GameObject.FindGameObjectWithTag("Water").GetComponent<Terrain>();
            WorldSerialization world =  WorldConverter.terrainToWorld(terrain, water);

            Debug.Log("Out: " + world.world.maps.Count);
            Debug.Log("Out: " + world.world.prefabs.Count);
            Debug.Log("Out: " + world.world.paths.Count);

            world.Save(@"C:\Users\Jason\rust test\test.map");
            Debug.Log(world.Checksum);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        //GUILayout.TextArea(result);

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
