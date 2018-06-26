using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldConverter {
    
	// #fuckErrn
	
    public struct MapInfo
    {
        public int resolution;
        public Vector3 size;
        public float[,,] splatMap;
        public TerrainInfo terrain;
        public TerrainInfo land;
        public TerrainInfo water;
        public WorldSerialization.PrefabData[] prefabData;
        public WorldSerialization.PathData[] pathData;
    }

    public struct TerrainInfo
    {
        public float[,] heights;
    }

    static float[,] shortMapToFloatArray(TerrainMap<short> terrainMap)
    {
        float[,] heights = new float[terrainMap.res, terrainMap.res];
        for (int i = 0; i < heights.GetLength(0); i++)
        {
            for (int j = 0; j < heights.GetLength(1); j++)
            {
                heights[i, j] = BitUtility.Short2Float(terrainMap[i, j]);
            }
        }
        return heights;
    }

    static byte[] floatArrayToByteArray(float[,] floatArray)
    {
        short[] shortArray = new short[floatArray.GetLength(0) * floatArray.GetLength(1)];
        
        for(int i = 0; i < floatArray.GetLength(0); i++)
        {
            for (int j = 0; j < floatArray.GetLength(1); j++)
            {
                shortArray[(i* floatArray.GetLength(0)) + j] = BitUtility.Float2Short(floatArray[i, j]);
            }
        }

        byte[] byteArray = new byte[shortArray.Length * 2];

        Buffer.BlockCopy(shortArray, 0, byteArray, 0, byteArray.Length);

        return byteArray;
    }
    

    public static MapInfo worldToTerrain(WorldSerialization blob)
    {
        MapInfo terrains = new MapInfo();

        var terrainSize = new Vector3(blob.world.size, 1000, blob.world.size);
        var terrainMap = new TerrainMap<short>(blob.GetMap("terrain").data, 1);
        var heightMap = new TerrainMap<short>(blob.GetMap("height").data, 1);
        var waterMap = new TerrainMap<short>(blob.GetMap("water").data, 1);
        var splatMap = new TerrainMap<byte>(blob.GetMap("splat").data, 8);

        terrains.pathData = blob.world.paths.ToArray();
        terrains.prefabData = blob.world.prefabs.ToArray();

        terrains.resolution = heightMap.res;
        terrains.size = terrainSize;

        //heightmap has monuments imprinted
        //terrainmap is whats generated and then deformed to create heightmap?

        terrains.terrain.heights = shortMapToFloatArray(terrainMap);
        terrains.land.heights = shortMapToFloatArray(terrainMap);
        terrains.water.heights = shortMapToFloatArray(waterMap);

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

        return terrains;
        
    }
    

    public static WorldSerialization terrainToWorld(Terrain land, Terrain water)
    {
        WorldSerialization world = new WorldSerialization();
        world.world.size = (uint) land.terrainData.size.x;

        byte[] landHeightBytes = floatArrayToByteArray(land.terrainData.GetHeights(0, 0, land.terrainData.heightmapWidth, land.terrainData.heightmapHeight));
        byte[] waterHeightBytes = floatArrayToByteArray(water.terrainData.GetHeights(0, 0, water.terrainData.heightmapWidth, water.terrainData.heightmapHeight));

        
        float[,,] splatMapValues = land.terrainData.GetAlphamaps(0, 0, land.terrainData.alphamapWidth, land.terrainData.alphamapHeight);

        var textureResolution = Mathf.NextPowerOfTwo((int)(world.world.size * 0.50f));

        byte[] splatBytes = new byte[textureResolution * textureResolution * 8];
        var splatMap = new TerrainMap<byte>(splatBytes, 8);

        for(int i = 0; i < 8; i++)
        {
            
            for(int j = 0; j < textureResolution; j++)
            {
                for(int k = 0; k < textureResolution; k++)
                {
                    splatMap[i, j, k] = 
                        BitUtility.Float2Byte(splatMapValues[j, k, i]);
                }
            }
        }

        byte[] biomeBytes = new byte[textureResolution * textureResolution * 4];
        var biomeMap = new TerrainMap<byte>(biomeBytes, 4);


            for (int j = 0; j < textureResolution; j++)
            {
                for (int k = 0; k < textureResolution; k++)
                {
                    biomeMap[TerrainBiome.ARID_IDX, j, k] = BitUtility.Float2Byte(1f);
                    biomeMap[TerrainBiome.ARCTIC_IDX, j, k] = BitUtility.Float2Byte(0f);
                    biomeMap[TerrainBiome.TEMPERATE_IDX, j, k] = BitUtility.Float2Byte(0f);
                    biomeMap[TerrainBiome.TUNDRA_IDX, j, k] = BitUtility.Float2Byte(0f);
                }
            }

        byte[] alphaBytes = new byte[textureResolution * textureResolution];
        var alphaMap = new TerrainMap<byte>(alphaBytes, 1);
        for (int k = 0; k < alphaBytes.Length; k++)
        {
            alphaBytes[k] = 255;
        }
        byte[] topologyBytes = new byte[textureResolution * textureResolution * 4];
        var topologyMap = new TerrainMap<int>(topologyBytes, 1);
        for (int k = 0; k < topologyBytes.Length; k++)
        {
            topologyBytes[k] = 0x0;
        }

        Debug.Log(alphaMap.res);

        

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

        
        /*

        byte[] emptyBiome = new byte[16777216];
        byte[] emptyAlpha = new byte[4194304];


        world.AddMap("terrain", landHeightBytes);
        world.AddMap("height", landHeightBytes);
        world.AddMap("splat", splatMap.ToByteArray());
        world.AddMap("biome", emptyBiome);
        world.AddMap("topology", emptyBiome);
        world.AddMap("alpha", emptyAlpha);
        world.AddMap("water", waterHeightBytes);*/
        return world;
    }

}
