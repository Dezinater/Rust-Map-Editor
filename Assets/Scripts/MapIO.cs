using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static WorldSerialization;

[Serializable]
public class MapIO : MonoBehaviour {
    
    public TerrainTopology.Enum topologyLayer;
    public TerrainTopology.Enum oldTopologyLayer;

    public int landSelectIndex = 0;
    public string landLayer = "ground";
    LandData selectedLandLayer;

    TopologyMesh topology;

    public void saveTopologyLayer()
    {
        if (topology == null)
            topology = GameObject.FindGameObjectWithTag("Topology").GetComponent<TopologyMesh>();

        LandData topologyData = GameObject.FindGameObjectWithTag("Land").transform.Find("Topology").GetComponent<LandData>();
        TerrainMap<int> topologyMap = new TerrainMap<int>(topology.top,1);
        float[,,] splatMap = TypeConverter.singleToMulti(topologyData.splatMap,2);
        
        for (int i = 0; i < topologyMap.res; i++)
        {
            for (int j = 0; j < topologyMap.res; j++)
            {
                if(splatMap[i,j,0] > 0)
                {
                    topologyMap[i, j] = topologyMap[i, j] | (int)oldTopologyLayer;
                }
                if (splatMap[i, j, 1] > 0)
                {
                    topologyMap[i, j] = topologyMap[i, j] & ~(int)oldTopologyLayer;
                }
            }
        }


        topology.top = topologyMap.ToByteArray();
    }

    public void changeLandLayer()
    {
        if (topology == null)
            topology = GameObject.FindGameObjectWithTag("Topology").GetComponent<TopologyMesh>();

        if (selectedLandLayer != null)
            selectedLandLayer.save();

        switch (landLayer.ToLower())
        {
            case "ground":
                selectedLandLayer = GameObject.FindGameObjectWithTag("Land").transform.Find("Ground").GetComponent<LandData>();
                break;
            case "biome":
                selectedLandLayer = GameObject.FindGameObjectWithTag("Land").transform.Find("Biome").GetComponent<LandData>();
                break;
            case "alpha":
                selectedLandLayer = GameObject.FindGameObjectWithTag("Land").transform.Find("Alpha").GetComponent<LandData>();
                break;
            case "topology":
                //updated topology values
                //selectedLandLayer.splatMap;
                saveTopologyLayer();
                selectedLandLayer = GameObject.FindGameObjectWithTag("Land").transform.Find("Topology").GetComponent<LandData>();
                selectedLandLayer.setData(topology.getSplatMap((int)topologyLayer), "topology");
                break;
        }
        selectedLandLayer.setLayer();
    }



    public GameObject spawnPrefab(GameObject g, PrefabData prefabData, Transform parent = null)
    {
        Vector3 pos = new Vector3(prefabData.position.x, prefabData.position.y, prefabData.position.z);
        Vector3 scale = new Vector3(prefabData.scale.x, prefabData.scale.y, prefabData.scale.z);
        Quaternion rotation = Quaternion.Euler(new Vector3(prefabData.rotation.x, prefabData.rotation.y, prefabData.rotation.z));

        
        GameObject newObj = Instantiate(g, pos + getMapOffset(), rotation, parent);
        newObj.transform.localScale = scale;

        return newObj;
    }

    private void cleanUpMap()
    {
        selectedLandLayer = null;
        foreach(PrefabDataHolder g in GameObject.FindObjectsOfType<PrefabDataHolder>())
        {
            DestroyImmediate(g.gameObject);
        }

        foreach (PathDataHolder g in GameObject.FindObjectsOfType<PathDataHolder>())
        {
            DestroyImmediate(g.gameObject);
        }
    }


    public static Vector3 getTerrainSize()
    {
        return GameObject.FindGameObjectWithTag("Land").GetComponent<Terrain>().terrainData.size;
    }
    public static Vector3 getMapOffset()
    {
        return 0.5f * getTerrainSize();
    }

    public void Load(WorldSerialization blob)
    {
        Debug.Log("Map hash: " + blob.Checksum);
        cleanUpMap();
        var terrainSize = new Vector3(blob.world.size, 1000, blob.world.size);
        var terrainPosition = 0.5f * terrainSize;

        LandData groundLandData = GameObject.FindGameObjectWithTag("Land").transform.Find("Ground").GetComponent<LandData>();
        LandData biomeLandData = GameObject.FindGameObjectWithTag("Land").transform.Find("Biome").GetComponent<LandData>();
        LandData alphaLandData = GameObject.FindGameObjectWithTag("Land").transform.Find("Alpha").GetComponent<LandData>();
        
        Terrain land = GameObject.FindGameObjectWithTag("Land").GetComponent<Terrain>();
        Terrain water = GameObject.FindGameObjectWithTag("Water").GetComponent<Terrain>();

        TopologyMesh topography = GameObject.FindGameObjectWithTag("Topology").GetComponent<TopologyMesh>();
        
        WorldConverter.MapInfo terrains = WorldConverter.worldToTerrain(blob);

        topography.InitMesh(terrains.topology);

        land.terrainData.heightmapResolution = terrains.resolution;
        land.terrainData.size = terrains.size;

        water.terrainData.heightmapResolution = terrains.resolution;
        water.terrainData.size = terrains.size;
        
        land.terrainData.SetHeights(0, 0, terrains.land.heights);
        water.terrainData.SetHeights(0, 0, terrains.water.heights);

        land.terrainData.alphamapResolution = terrains.resolution;
        //land.terrainData.baseMapResolution = terrains.resolution;
        //land.terrainData.SetDetailResolution(terrains.resolution,8);

        land.terrainData.size = terrains.size;
        
        groundLandData.setData(terrains.splatMap, "ground");
        
        biomeLandData.setData(terrains.biomeMap, "biome");
        
        alphaLandData.setData(terrains.alphaMap, "alpha");

        changeLandLayer();

        Transform prefabsParent = GameObject.FindGameObjectWithTag("Prefabs").transform;
        GameObject defaultObj = Resources.Load<GameObject>("Prefabs/DefaultPrefab");
        for(int i = 0; i < terrains.prefabData.Length; i++)
        {
            GameObject newObj = spawnPrefab(defaultObj, terrains.prefabData[i], prefabsParent);
            newObj.GetComponent<PrefabDataHolder>().prefabData = terrains.prefabData[i];
        }


        Transform pathsParent = GameObject.FindGameObjectWithTag("Paths").transform;
        GameObject pathObj = Resources.Load<GameObject>("Paths/Path");
        for (int i = 0; i < terrains.pathData.Length; i++)
        {
            Vector3 averageLocation = Vector3.zero;
            for (int j = 0; j < terrains.pathData[i].nodes.Length; j++)
            {
                averageLocation += terrains.pathData[i].nodes[j];
            }
            averageLocation /= terrains.pathData[i].nodes.Length;
            GameObject newObject = Instantiate(pathObj, averageLocation + terrainPosition, Quaternion.identity, pathsParent);
            newObject.GetComponent<PathDataHolder>().pathData = terrains.pathData[i];
            newObject.GetComponent<PathDataHolder>().offset = terrainPosition;
        }
    }


    public void Save(string path)
    {
        selectedLandLayer.save();
        saveTopologyLayer();
        Terrain terrain = GameObject.FindGameObjectWithTag("Land").GetComponent<Terrain>();
        Terrain water = GameObject.FindGameObjectWithTag("Water").GetComponent<Terrain>();
        if (water == null)
            Debug.LogError("Water object is not enabled");
        if (terrain == null)
            Debug.LogError("Land object is not enabled");

        WorldSerialization world = WorldConverter.terrainToWorld(terrain, water);
        
        world.Save(path);
        //Debug.Log("Map hash: " + world.Checksum);
    }
        

    public void Awake()
    {
        
        FileSystem.iface = new FileSystem_AssetBundles(@"C:\Program Files (x86)\Steam\steamapps\common\RustStaging\Bundles\Bundles");

        Transform prefabsParent = GameObject.FindGameObjectWithTag("Prefabs").transform;
        
        foreach(PrefabDataHolder pdh in GameObject.FindObjectsOfType<PrefabDataHolder>())
        {
            if (!pdh.spawnOnPlay)
                continue;

            Debug.Log(StringPool.Get((pdh.prefabData.id)));
            
            GameObject g = FileSystem.Load<GameObject>(StringPool.Get((pdh.prefabData.id)));
            
            GameObject newObject = spawnPrefab(g, pdh.prefabData, prefabsParent);

            PrefabDataHolder prefabData = newObject.GetComponent<PrefabDataHolder>();
            if (prefabData == null)
            {
                prefabData = newObject.AddComponent<PrefabDataHolder>();
            }
            
            prefabData.prefabData = pdh.prefabData;

            Destroy(pdh.gameObject);
        }
        
    }

    void OnApplicationQuit()
    {
        Transform prefabsParent = GameObject.FindGameObjectWithTag("Prefabs").transform;
        foreach (PrefabDataHolder pdh in GameObject.FindObjectsOfType<PrefabDataHolder>())
        {
            GameObject defaultObj = Resources.Load<GameObject>("Prefabs/DefaultPrefab");
            GameObject newObject = spawnPrefab(defaultObj, pdh.prefabData, prefabsParent);
            

            PrefabDataHolder prefabData = newObject.GetComponent<PrefabDataHolder>();
            if (prefabData == null)
            {
                newObject.AddComponent<PrefabDataHolder>();
            }
            prefabData.prefabData = pdh.prefabData;

            Destroy(pdh.gameObject);
        }
    }
}
