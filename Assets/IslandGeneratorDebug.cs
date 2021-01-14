using System;
using System.Collections;
using System.Collections.Generic;
using LandGeneration;
using UnityEngine;

public class IslandGeneratorDebug : MonoBehaviour
{
    public int octaves;
    public float scale = 20f;
    public int seed = 0;
    [Range(0, 1)] public float persistence;
    [Range(0, 10)] public float lacunarity;
    
    
    public int heightmapResolution = 1024;
    private Color[] _pixels;


    private void OnValidate()
    {
        var heights = Noise.GenerateNoiseMap(heightmapResolution, heightmapResolution, seed, scale, octaves, persistence, lacunarity, Vector2.zero);
        _pixels = new Color[heights.Length];
        for (int i = 0; i < heights.Length; i++)
        {
            var h = heights[i];
            _pixels[i] = new Color(h,h,h);
        }
    }

    private void OnDrawGizmos()
    {
        
            
        Texture2D texture2D = new Texture2D(heightmapResolution, heightmapResolution);
        texture2D.SetPixels(_pixels);
        texture2D.Apply();
        
        
        Gizmos.DrawGUITexture(new Rect(0,0,500,500), texture2D);
    }
}
