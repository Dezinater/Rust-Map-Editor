using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerOptions : MonoBehaviour
{
    public bool showBounds = false;
    MapIO mapIO;


    private void OnDrawGizmos()
    {
        if (mapIO == null)
            mapIO = GameObject.FindGameObjectWithTag("MapIO").GetComponent<MapIO>();

        if (showBounds)
        {
            Gizmos.DrawWireCube(MapIO.getMapOffset(), MapIO.getTerrainSize());
        }
    }

}
