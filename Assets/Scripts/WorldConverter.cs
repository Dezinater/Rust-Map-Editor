
using UnityEngine;

public class WorldConverter {
    
	// #fuckErrn
	
    public struct MapInfo
    {
        public int resolution;
        public Vector3 size;
        public float[,,] splatMap;
        public float[,,] biomeMap;
        public float[,,] alphaMap;
        public TerrainInfo terrain;
        public TerrainInfo land;
        public TerrainInfo water;
        public TerrainMap<int> topology;
        public WorldSerialization.PrefabData[] prefabData;
        public WorldSerialization.PathData[] pathData;
    }

    public struct TerrainInfo
    {
        //put splatmaps in here if swamps and oceans add textures to water
        public float[,] heights;
    }

    
    

    public static MapInfo worldToTerrain(WorldSerialization blob)
    {
        MapInfo terrains = new MapInfo();

        var terrainSize = new Vector3(blob.world.size, 1000, blob.world.size);
        var terrainMap = new TerrainMap<short>(blob.GetMap("terrain").data, 1);
        var heightMap = new TerrainMap<short>(blob.GetMap("height").data, 1);
        var waterMap = new TerrainMap<short>(blob.GetMap("water").data, 1);
        var splatMap = new TerrainMap<byte>(blob.GetMap("splat").data, 8);
        var topologyMap = new TerrainMap<int>(blob.GetMap("topology").data, 1);
        var biomeMap = new TerrainMap<byte>(blob.GetMap("biome").data, 4);
        var alphaMap = new TerrainMap<byte>(blob.GetMap("alpha").data, 1);

        terrains.topology = topologyMap;

        terrains.pathData = blob.world.paths.ToArray();
        terrains.prefabData = blob.world.prefabs.ToArray();

        terrains.resolution = heightMap.res;
        terrains.size = terrainSize;

        terrains.terrain.heights = TypeConverter.shortMapToFloatArray(terrainMap);
        terrains.land.heights = TypeConverter.shortMapToFloatArray(terrainMap);
        terrains.water.heights = TypeConverter.shortMapToFloatArray(waterMap);

        terrains.splatMap = new float[splatMap.res, splatMap.res, 8];
        for (int i = 0; i < terrains.splatMap.GetLength(0); i++)
        {
            for (int j = 0; j < terrains.splatMap.GetLength(1); j++)
            {
                for (int k = 0; k < 8; k++)
                {
                    terrains.splatMap[i, j, k] = BitUtility.Byte2Float(splatMap[k, i, j]);
                }
            }
        }

        terrains.biomeMap = new float[biomeMap.res, biomeMap.res, 4];
        for (int i = 0; i < terrains.biomeMap.GetLength(0); i++)
        {
            for (int j = 0; j < terrains.biomeMap.GetLength(1); j++)
            {
                for (int k = 0; k < 4; k++)
                {
                    terrains.biomeMap[i, j, k] = BitUtility.Byte2Float(biomeMap[k, i, j]);
                }
            }
        }
        terrains.alphaMap = new float[alphaMap.res, alphaMap.res, 1];
        for (int i = 0; i < terrains.alphaMap.GetLength(0); i++)
        {
            for (int j = 0; j < terrains.alphaMap.GetLength(1); j++)
            {
                for (int k = 0; k < 1; k++)
                {
                    terrains.alphaMap[i, j, k] = BitUtility.Byte2Float(alphaMap[k, i, j]);
                }
            }
        }

        return terrains;
        
    }
    

    public static WorldSerialization terrainToWorld(Terrain land, Terrain water)
    {
        WorldSerialization world = new WorldSerialization();
        world.world.size = (uint) land.terrainData.size.x;

        
        byte[] landHeightBytes = TypeConverter.floatArrayToByteArray(land.terrainData.GetHeights(0, 0, land.terrainData.heightmapWidth, land.terrainData.heightmapHeight));
        byte[] waterHeightBytes = TypeConverter.floatArrayToByteArray(water.terrainData.GetHeights(0, 0, water.terrainData.heightmapWidth, water.terrainData.heightmapHeight));
    
        var textureResolution = Mathf.NextPowerOfTwo((int)(world.world.size * 0.50f));


        float[,,] splatMapValues = TypeConverter.singleToMulti(GameObject.FindGameObjectWithTag("Land").transform.Find("Ground").GetComponent<LandData>().splatMap, 8);
        byte[] splatBytes = new byte[textureResolution * textureResolution * 8];
        var splatMap = new TerrainMap<byte>(splatBytes, 8);

        for(int i = 0; i < 8; i++)
        {
            for(int j = 0; j < textureResolution; j++)
            {
                for(int k = 0; k < textureResolution; k++)
                {
                    splatMap[i, j, k] = BitUtility.Float2Byte(splatMapValues[j, k, i]);
                }
            }
        }

        byte[] biomeBytes = new byte[textureResolution * textureResolution * 4];
        var biomeMap = new TerrainMap<byte>(biomeBytes, 4);
        float[,,] biomeArray = TypeConverter.singleToMulti(GameObject.FindGameObjectWithTag("Land").transform.Find("Biome").GetComponent<LandData>().splatMap, 4);

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < textureResolution; j++)
            {
                for (int k = 0; k < textureResolution; k++)
                {
                    biomeMap[i, j, k] = BitUtility.Float2Byte(biomeArray[j, k, i]);
                }
            }
        }


        byte[] alphaBytes = new byte[textureResolution * textureResolution * 1];
        var alphaMap = new TerrainMap<byte>(alphaBytes, 1);
        float[,,] alphaArray = TypeConverter.singleToMulti(GameObject.FindGameObjectWithTag("Land").transform.Find("Alpha").GetComponent<LandData>().splatMap, 1);
        for (int i = 0; i < 1; i++)
        {
            for (int j = 0; j < textureResolution; j++)
            {
                for (int k = 0; k < textureResolution; k++)
                {
                    alphaMap[i, j, k] = BitUtility.Float2Byte(alphaArray[j, k, i]);
                }
            }
        }

        var topologyMap = GameObject.FindGameObjectWithTag("Topology").GetComponent<TopologyMesh>().getTerrainMap();


        world.AddMap("terrain", landHeightBytes);
        world.AddMap("height", landHeightBytes);
        world.AddMap("splat", splatMap.ToByteArray());
        world.AddMap("biome", biomeMap.ToByteArray());
        world.AddMap("topology", topologyMap.ToByteArray());
        world.AddMap("alpha", alphaMap.ToByteArray());
        world.AddMap("water", waterHeightBytes);

        PrefabDataHolder[] prefabs = GameObject.FindObjectsOfType<PrefabDataHolder>();

        foreach (PrefabDataHolder p in prefabs)
        {
            if (p.prefabData != null)
                world.world.prefabs.Insert(0,p.prefabData);
        }

        PathDataHolder[] paths = GameObject.FindObjectsOfType<PathDataHolder>();

        foreach (PathDataHolder p in paths)
        {
            if (p.pathData != null)
                world.world.paths.Insert(0,p.pathData);
        }

       
        return world;
    }

}
