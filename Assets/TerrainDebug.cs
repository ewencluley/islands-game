using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainDebug : MonoBehaviour
{
    private Terrain _terrain;

    // Start is called before the first frame update
    void Start()
    {
        _terrain = GetComponent<Terrain>();
    }

    // Update is called once per frame
    void Update()
    {
        var x = 1;
    }
}
