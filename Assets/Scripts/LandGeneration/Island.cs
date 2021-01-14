using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LandGeneration
{
    public class Island
    {
        public float[] Heights;
        public Vector3 TerrainSize;
        public int HeightMapResolution;
        public Vector3[] trees;
        public Vector2 Position;
        public int Seed;
        public Vector3[] shoreline;
        public List<List<Vector3>> ShorelinePaths = new List<List<Vector3>>();

        public Island(float[] heights, int heightMapResolution, Vector3 terrainSize, Vector2 position, int seed)
        {
            this.Heights = heights;
            this.TerrainSize = terrainSize;
            this.Position = position;
            this.Seed = seed;
            this.HeightMapResolution = heightMapResolution;
        }
    }
}